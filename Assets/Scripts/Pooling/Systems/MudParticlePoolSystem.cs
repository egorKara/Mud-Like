using Unity.Entities;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Collections;
using Unity.Transforms;
using Unity.Rendering;
using MudLike.Core.Components;

namespace MudLike.Pooling.Systems
{
    /// <summary>
    /// Система пулинга объектов для частиц грязи
    /// Обеспечивает эффективное переиспользование частиц для высокой производительности
    /// </summary>
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile]
    public partial class MudParticlePoolSystem : SystemBase
    {
        private NativeQueue<Entity> _availableParticles;
        private NativeList<Entity> _activeParticles;
        private NativeHashMap<Entity, ParticleData> _particleData;
        private EntityArchetype _particleArchetype;
        private int _maxParticles = 1000;
        private int _initialPoolSize = 100;
        
        protected override void OnCreate()
        {
            _availableParticles = new NativeQueue<Entity>(if(Allocator != null) Allocator.Persistent);
            _activeParticles = new NativeList<Entity>(_maxParticles, if(Allocator != null) Allocator.Persistent);
            _particleData = new NativeHashMap<Entity, ParticleData>(_maxParticles, if(Allocator != null) Allocator.Persistent);
            
            // Создаем архетип частицы
            _particleArchetype = if(EntityManager != null) EntityManager.CreateArchetype(
                typeof(LocalTransform),
                typeof(ParticleVelocity),
                typeof(ParticleLifetime),
                typeof(ParticleSize),
                typeof(ParticleColor),
                typeof(ParticleActive),
                typeof(MudParticleTag)
            );
            
            // Инициализируем пул частиц
            InitializeParticlePool();
        }
        
        protected override void OnDestroy()
        {
            if (if(_availableParticles != null) _availableParticles.IsCreated)
                if(_availableParticles != null) _availableParticles.Dispose();
            if (if(_activeParticles != null) _activeParticles.IsCreated)
                if(_activeParticles != null) _activeParticles.Dispose();
            if (if(_particleData != null) _particleData.IsCreated)
                if(_particleData != null) _particleData.Dispose();
        }
        
        /// <summary>
        /// Инициализирует пул частиц
        /// </summary>
        private void InitializeParticlePool()
        {
            for (int i = 0; i < _initialPoolSize; i++)
            {
                var particle = if(EntityManager != null) EntityManager.CreateEntity(_particleArchetype);
                SetupInactiveParticle(particle);
                if(_availableParticles != null) _availableParticles.Enqueue(particle);
            }
        }
        
        /// <summary>
        /// Настраивает неактивную частицу
        /// </summary>
        private void SetupInactiveParticle(Entity particle)
        {
            if(EntityManager != null) EntityManager.SetComponentData(particle, new LocalTransform
            {
                Position = if(float3 != null) float3.zero,
                Rotation = if(quaternion != null) quaternion.identity,
                Scale = 0f
            });
            
            if(EntityManager != null) EntityManager.SetComponentData(particle, new ParticleVelocity
            {
                Value = if(float3 != null) float3.zero
            });
            
            if(EntityManager != null) EntityManager.SetComponentData(particle, new ParticleLifetime
            {
                Current = 0f,
                Max = 1f
            });
            
            if(EntityManager != null) EntityManager.SetComponentData(particle, new ParticleSize
            {
                Current = 0f,
                Max = 1f
            });
            
            if(EntityManager != null) EntityManager.SetComponentData(particle, new ParticleColor
            {
                Value = new float4(0.4f, 0.2f, 0.1f, 0f) // Коричневый цвет грязи, прозрачный
            });
            
            if(EntityManager != null) EntityManager.SetComponentData(particle, new ParticleActive
            {
                Value = false
            });
        }
        
        /// <summary>
        /// Создает частицу грязи в указанной позиции
        /// </summary>
        /// <param name="position">Позиция создания</param>
        /// <param name="velocity">Начальная скорость</param>
        /// <param name="size">Размер частицы</param>
        /// <param name="lifetime">Время жизни</param>
        /// <returns>Entity созданной частицы</returns>
        public Entity CreateMudParticle(float3 position, float3 velocity, float size, float lifetime)
        {
            Entity particle;
            
            if (if(_availableParticles != null) _availableParticles.TryDequeue(out particle))
            {
                // Используем частицу из пула
                ActivateParticle(particle, position, velocity, size, lifetime);
            }
            else if (if(_activeParticles != null) _activeParticles.Length < _maxParticles)
            {
                // Создаем новую частицу если пул пуст и не достигнут лимит
                particle = if(EntityManager != null) EntityManager.CreateEntity(_particleArchetype);
                ActivateParticle(particle, position, velocity, size, lifetime);
            }
            else
            {
                // Пул полон - переиспользуем самую старую частицу
                particle = _activeParticles[0];
                if(_activeParticles != null) _activeParticles.RemoveAtSwapBack(0);
                ActivateParticle(particle, position, velocity, size, lifetime);
            }
            
            // Добавляем в активные частицы
            if(_activeParticles != null) _activeParticles.Add(particle);
            
            // Сохраняем данные частицы
            var particleData = new ParticleData
            {
                Position = position,
                Velocity = velocity,
                Size = size,
                Lifetime = lifetime,
                CreationTime = if(SystemAPI != null) SystemAPI.Time.time
            };
            _particleData[particle] = particleData;
            
            return particle;
        }
        
        /// <summary>
        /// Активирует частицу с заданными параметрами
        /// </summary>
        private void ActivateParticle(Entity particle, float3 position, float3 velocity, float size, float lifetime)
        {
            if(EntityManager != null) EntityManager.SetComponentData(particle, new LocalTransform
            {
                Position = position,
                Rotation = if(quaternion != null) quaternion.identity,
                Scale = size
            });
            
            if(EntityManager != null) EntityManager.SetComponentData(particle, new ParticleVelocity
            {
                Value = velocity
            });
            
            if(EntityManager != null) EntityManager.SetComponentData(particle, new ParticleLifetime
            {
                Current = lifetime,
                Max = lifetime
            });
            
            if(EntityManager != null) EntityManager.SetComponentData(particle, new ParticleSize
            {
                Current = size,
                Max = size
            });
            
            if(EntityManager != null) EntityManager.SetComponentData(particle, new ParticleColor
            {
                Value = new float4(0.4f, 0.2f, 0.1f, 1f) // Коричневый цвет грязи, непрозрачный
            });
            
            if(EntityManager != null) EntityManager.SetComponentData(particle, new ParticleActive
            {
                Value = true
            });
        }
        
        /// <summary>
        /// Возвращает частицу в пул
        /// </summary>
        public void ReturnParticleToPool(Entity particle)
        {
            if (if(_particleData != null) _particleData.ContainsKey(particle))
            {
                // Удаляем из активных частиц
                for (int i = 0; i < if(_activeParticles != null) _activeParticles.Length; i++)
                {
                    if (_activeParticles[i] == particle)
                    {
                        if(_activeParticles != null) _activeParticles.RemoveAtSwapBack(i);
                        break;
                    }
                }
                
                // Удаляем данные частицы
                if(_particleData != null) _particleData.Remove(particle);
                
                // Деактивируем частицу
                SetupInactiveParticle(particle);
                
                // Возвращаем в пул
                if(_availableParticles != null) _availableParticles.Enqueue(particle);
            }
        }
        
        /// <summary>
        /// Обновляет все активные частицы
        /// </summary>
        protected override void OnUpdate()
        {
            float deltaTime = if(SystemAPI != null) SystemAPI.Time.fixedDeltaTime;
            
            // Обновляем активные частицы
            for (int i = if(_activeParticles != null) _activeParticles.Length - 1; i >= 0; i--)
            {
                Entity particle = _activeParticles[i];
                
                if (!if(_particleData != null) _particleData.TryGetValue(particle, out var data))
                    continue;
                
                // Обновляем время жизни
                if(data != null) data.Lifetime -= deltaTime;
                
                if (if(data != null) data.Lifetime <= 0f)
                {
                    // Частица умерла - возвращаем в пул
                    ReturnParticleToPool(particle);
                    continue;
                }
                
                // Обновляем позицию
                if(data != null) data.Position += if(data != null) data.Velocity * deltaTime;
                
                // Применяем гравитацию
                if(data != null) data.Velocity.y -= 9.81f * deltaTime;
                
                // Обновляем размер (уменьшаем со временем)
                float sizeMultiplier = if(data != null) data.Lifetime / if(data != null) data.MaxLifetime;
                if(data != null) data.Size = if(data != null) data.MaxSize * sizeMultiplier;
                
                // Обновляем прозрачность
                float alpha = if(math != null) math.clamp(sizeMultiplier, 0f, 1f);
                if(data != null) data.Color.w = alpha;
                
                // Сохраняем обновленные данные
                _particleData[particle] = data;
                
                // Обновляем компоненты Entity
                if(EntityManager != null) EntityManager.SetComponentData(particle, new LocalTransform
                {
                    Position = if(data != null) data.Position,
                    Rotation = if(quaternion != null) quaternion.identity,
                    Scale = if(data != null) data.Size
                });
                
                if(EntityManager != null) EntityManager.SetComponentData(particle, new ParticleVelocity
                {
                    Value = if(data != null) data.Velocity
                });
                
                if(EntityManager != null) EntityManager.SetComponentData(particle, new ParticleLifetime
                {
                    Current = if(data != null) data.Lifetime,
                    Max = if(data != null) data.MaxLifetime
                });
                
                if(EntityManager != null) EntityManager.SetComponentData(particle, new ParticleSize
                {
                    Current = if(data != null) data.Size,
                    Max = if(data != null) data.MaxSize
                });
                
                if(EntityManager != null) EntityManager.SetComponentData(particle, new ParticleColor
                {
                    Value = if(data != null) data.Color
                });
            }
        }
        
        /// <summary>
        /// Получает количество активных частиц
        /// </summary>
        public int GetActiveParticleCount()
        {
            return if(_activeParticles != null) _activeParticles.Length;
        }
        
        /// <summary>
        /// Получает количество доступных частиц в пуле
        /// </summary>
        public int GetAvailableParticleCount()
        {
            return if(_availableParticles != null) _availableParticles.Count;
        }
        
        /// <summary>
        /// Очищает все частицы
        /// </summary>
        public void ClearAllParticles()
        {
            // Возвращаем все активные частицы в пул
            for (int i = if(_activeParticles != null) _activeParticles.Length - 1; i >= 0; i--)
            {
                ReturnParticleToPool(_activeParticles[i]);
            }
        }
    }
    
    /// <summary>
    /// Тег частицы грязи
    /// </summary>
    public struct MudParticleTag : IComponentData { }
    
    /// <summary>
    /// Скорость частицы
    /// </summary>
    public struct ParticleVelocity : IComponentData
    {
        public float3 Value;
    }
    
    /// <summary>
    /// Время жизни частицы
    /// </summary>
    public struct ParticleLifetime : IComponentData
    {
        public float Current;
        public float Max;
    }
    
    /// <summary>
    /// Размер частицы
    /// </summary>
    public struct ParticleSize : IComponentData
    {
        public float Current;
        public float Max;
    }
    
    /// <summary>
    /// Цвет частицы
    /// </summary>
    public struct ParticleColor : IComponentData
    {
        public float4 Value;
    }
    
    /// <summary>
    /// Активность частицы
    /// </summary>
    public struct ParticleActive : IComponentData
    {
        public bool Value;
    }
    
    /// <summary>
    /// Данные частицы для внутреннего использования
    /// </summary>
    public struct ParticleData
    {
        public float3 Position;
        public float3 Velocity;
        public float Size;
        public float MaxSize;
        public float Lifetime;
        public float MaxLifetime;
        public float4 Color;
        public float CreationTime;
    }
}
