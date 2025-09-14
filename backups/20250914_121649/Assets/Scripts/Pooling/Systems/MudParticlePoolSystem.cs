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
            _availableParticles = new NativeQueue<Entity>(Allocator.Persistent);
            _activeParticles = new NativeList<Entity>(_maxParticles, Allocator.Persistent);
            _particleData = new NativeHashMap<Entity, ParticleData>(_maxParticles, Allocator.Persistent);
            
            // Создаем архетип частицы
            _particleArchetype = EntityManager.CreateArchetype(
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
            if (_availableParticles.IsCreated)
                _availableParticles.Dispose();
            if (_activeParticles.IsCreated)
                _activeParticles.Dispose();
            if (_particleData.IsCreated)
                _particleData.Dispose();
        }
        
        /// <summary>
        /// Инициализирует пул частиц
        /// </summary>
        private void InitializeParticlePool()
        {
            for (int i = 0; i < _initialPoolSize; i++)
            {
                var particle = EntityManager.CreateEntity(_particleArchetype);
                SetupInactiveParticle(particle);
                _availableParticles.Enqueue(particle);
            }
        }
        
        /// <summary>
        /// Настраивает неактивную частицу
        /// </summary>
        private void SetupInactiveParticle(Entity particle)
        {
            EntityManager.SetComponentData(particle, new LocalTransform
            {
                Position = float3.zero,
                Rotation = quaternion.identity,
                Scale = 0f
            });
            
            EntityManager.SetComponentData(particle, new ParticleVelocity
            {
                Value = float3.zero
            });
            
            EntityManager.SetComponentData(particle, new ParticleLifetime
            {
                Current = 0f,
                Max = 1f
            });
            
            EntityManager.SetComponentData(particle, new ParticleSize
            {
                Current = 0f,
                Max = 1f
            });
            
            EntityManager.SetComponentData(particle, new ParticleColor
            {
                Value = new float4(0.4f, 0.2f, 0.1f, 0f) // Коричневый цвет грязи, прозрачный
            });
            
            EntityManager.SetComponentData(particle, new ParticleActive
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
            
            if (_availableParticles.TryDequeue(out particle))
            {
                // Используем частицу из пула
                ActivateParticle(particle, position, velocity, size, lifetime);
            }
            else if (_activeParticles.Length < _maxParticles)
            {
                // Создаем новую частицу если пул пуст и не достигнут лимит
                particle = EntityManager.CreateEntity(_particleArchetype);
                ActivateParticle(particle, position, velocity, size, lifetime);
            }
            else
            {
                // Пул полон - переиспользуем самую старую частицу
                particle = _activeParticles[0];
                _activeParticles.RemoveAtSwapBack(0);
                ActivateParticle(particle, position, velocity, size, lifetime);
            }
            
            // Добавляем в активные частицы
            _activeParticles.Add(particle);
            
            // Сохраняем данные частицы
            var particleData = new ParticleData
            {
                Position = position,
                Velocity = velocity,
                Size = size,
                Lifetime = lifetime,
                CreationTime = SystemAPI.Time.time
            };
            _particleData[particle] = particleData;
            
            return particle;
        }
        
        /// <summary>
        /// Активирует частицу с заданными параметрами
        /// </summary>
        private void ActivateParticle(Entity particle, float3 position, float3 velocity, float size, float lifetime)
        {
            EntityManager.SetComponentData(particle, new LocalTransform
            {
                Position = position,
                Rotation = quaternion.identity,
                Scale = size
            });
            
            EntityManager.SetComponentData(particle, new ParticleVelocity
            {
                Value = velocity
            });
            
            EntityManager.SetComponentData(particle, new ParticleLifetime
            {
                Current = lifetime,
                Max = lifetime
            });
            
            EntityManager.SetComponentData(particle, new ParticleSize
            {
                Current = size,
                Max = size
            });
            
            EntityManager.SetComponentData(particle, new ParticleColor
            {
                Value = new float4(0.4f, 0.2f, 0.1f, 1f) // Коричневый цвет грязи, непрозрачный
            });
            
            EntityManager.SetComponentData(particle, new ParticleActive
            {
                Value = true
            });
        }
        
        /// <summary>
        /// Возвращает частицу в пул
        /// </summary>
        public void ReturnParticleToPool(Entity particle)
        {
            if (_particleData.ContainsKey(particle))
            {
                // Удаляем из активных частиц
                for (int i = 0; i < _activeParticles.Length; i++)
                {
                    if (_activeParticles[i] == particle)
                    {
                        _activeParticles.RemoveAtSwapBack(i);
                        break;
                    }
                }
                
                // Удаляем данные частицы
                _particleData.Remove(particle);
                
                // Деактивируем частицу
                SetupInactiveParticle(particle);
                
                // Возвращаем в пул
                _availableParticles.Enqueue(particle);
            }
        }
        
        /// <summary>
        /// Обновляет все активные частицы
        /// </summary>
        protected override void OnUpdate()
        {
            float deltaTime = SystemAPI.Time.fixedDeltaTime;
            
            // Обновляем активные частицы
            for (int i = _activeParticles.Length - 1; i >= 0; i--)
            {
                Entity particle = _activeParticles[i];
                
                if (!_particleData.TryGetValue(particle, out var data))
                    continue;
                
                // Обновляем время жизни
                data.Lifetime -= deltaTime;
                
                if (data.Lifetime <= 0f)
                {
                    // Частица умерла - возвращаем в пул
                    ReturnParticleToPool(particle);
                    continue;
                }
                
                // Обновляем позицию
                data.Position += data.Velocity * deltaTime;
                
                // Применяем гравитацию
                data.Velocity.y -= 9.81f * deltaTime;
                
                // Обновляем размер (уменьшаем со временем)
                float sizeMultiplier = data.Lifetime / data.MaxLifetime;
                data.Size = data.MaxSize * sizeMultiplier;
                
                // Обновляем прозрачность
                float alpha = math.clamp(sizeMultiplier, 0f, 1f);
                data.Color.w = alpha;
                
                // Сохраняем обновленные данные
                _particleData[particle] = data;
                
                // Обновляем компоненты Entity
                EntityManager.SetComponentData(particle, new LocalTransform
                {
                    Position = data.Position,
                    Rotation = quaternion.identity,
                    Scale = data.Size
                });
                
                EntityManager.SetComponentData(particle, new ParticleVelocity
                {
                    Value = data.Velocity
                });
                
                EntityManager.SetComponentData(particle, new ParticleLifetime
                {
                    Current = data.Lifetime,
                    Max = data.MaxLifetime
                });
                
                EntityManager.SetComponentData(particle, new ParticleSize
                {
                    Current = data.Size,
                    Max = data.MaxSize
                });
                
                EntityManager.SetComponentData(particle, new ParticleColor
                {
                    Value = data.Color
                });
            }
        }
        
        /// <summary>
        /// Получает количество активных частиц
        /// </summary>
        public int GetActiveParticleCount()
        {
            return _activeParticles.Length;
        }
        
        /// <summary>
        /// Получает количество доступных частиц в пуле
        /// </summary>
        public int GetAvailableParticleCount()
        {
            return _availableParticles.Count;
        }
        
        /// <summary>
        /// Очищает все частицы
        /// </summary>
        public void ClearAllParticles()
        {
            // Возвращаем все активные частицы в пул
            for (int i = _activeParticles.Length - 1; i >= 0; i--)
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
