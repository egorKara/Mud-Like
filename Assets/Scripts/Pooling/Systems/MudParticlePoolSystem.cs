using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;
using Unity.Collections;
using MudLike.Pooling.Components;
using MudLike.Effects.Components;

namespace MudLike.Pooling.Systems
{
    /// <summary>
    /// Система пулинга частиц грязи для оптимизации производительности
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [BurstCompile]
    public partial class MudParticlePoolSystem : SystemBase
    {
        private const int INITIAL_POOL_SIZE = 1000;
        private const int MAX_POOL_SIZE = 10000;
        private const float PARTICLE_LIFETIME = 5.0f;
        private const float PARTICLE_FADE_TIME = 1.0f;
        private const float PARTICLE_SIZE_MIN = 0.01f;
        private const float PARTICLE_SIZE_MAX = 0.1f;
        private const float PARTICLE_VELOCITY_MIN = 1.0f;
        private const float PARTICLE_VELOCITY_MAX = 10.0f;
        
        private EntityQuery _activeParticlesQuery;
        private EntityQuery _inactiveParticlesQuery;
        private NativeArray<Entity> _particlePool;
        private int _poolIndex;
        private bool _isPoolInitialized;
        
        protected override void OnCreate()
        {
            // Создаем запросы для активных и неактивных частиц
            _activeParticlesQuery = GetEntityQuery(
                ComponentType.ReadWrite<MudParticleData>(),
                ComponentType.ReadWrite<LocalTransform>(),
                ComponentType.ReadOnly<ActiveParticleTag>()
            );
            
            _inactiveParticlesQuery = GetEntityQuery(
                ComponentType.ReadWrite<MudParticleData>(),
                ComponentType.ReadWrite<LocalTransform>(),
                ComponentType.ReadOnly<InactiveParticleTag>()
            );
            
            // Инициализируем пул частиц
            InitializeParticlePool();
        }
        
        protected override void OnUpdate()
        {
            float deltaTime = SystemAPI.Time.DeltaTime;
            
            // Обновляем активные частицы
            UpdateActiveParticles(deltaTime);
            
            // Очищаем неактивные частицы
            CleanupInactiveParticles();
            
            // Управляем пулом
            ManageParticlePool();
        }
        
        /// <summary>
        /// Инициализирует пул частиц
        /// </summary>
        private void InitializeParticlePool()
        {
            if (_isPoolInitialized)
                return;
            
            _particlePool = new NativeArray<Entity>(INITIAL_POOL_SIZE, Allocator.Persistent);
            _poolIndex = 0;
            
            // Создаем неактивные частицы
            for (int i = 0; i < INITIAL_POOL_SIZE; i++)
            {
                var entity = EntityManager.CreateEntity();
                EntityManager.AddComponentData(entity, new MudParticleData
                {
                    Position = float3.zero,
                    Velocity = float3.zero,
                    Size = 0.01f,
                    Lifetime = 0f,
                    Mass = 0.001f,
                    Temperature = 20f,
                    Viscosity = 0.5f,
                    IsActive = false
                });
                EntityManager.AddComponentData(entity, new LocalTransform
                {
                    Position = float3.zero,
                    Rotation = quaternion.identity,
                    Scale = 1f
                });
                EntityManager.AddComponentData(entity, new InactiveParticleTag());
                
                _particlePool[i] = entity;
            }
            
            _isPoolInitialized = true;
        }
        
        /// <summary>
        /// Обновляет активные частицы
        /// </summary>
        [BurstCompile]
        private void UpdateActiveParticles(float deltaTime)
        {
            Entities
                .WithAll<ActiveParticleTag>()
                .ForEach((ref MudParticleData particle, ref LocalTransform transform) =>
                {
                    UpdateParticle(ref particle, ref transform, deltaTime);
                }).Schedule();
        }
        
        /// <summary>
        /// Обновляет отдельную частицу
        /// </summary>
        [BurstCompile]
        private static void UpdateParticle(ref MudParticleData particle, ref LocalTransform transform, float deltaTime)
        {
            if (!particle.IsActive)
                return;
            
            // Обновляем время жизни
            particle.Lifetime += deltaTime;
            
            // Проверяем, не истекло ли время жизни
            if (particle.Lifetime >= PARTICLE_LIFETIME)
            {
                particle.IsActive = false;
                return;
            }
            
            // Применяем гравитацию
            particle.Velocity.y -= 9.81f * deltaTime;
            
            // Применяем сопротивление воздуха
            particle.Velocity *= 0.99f;
            
            // Обновляем позицию
            particle.Position += particle.Velocity * deltaTime;
            transform.Position = particle.Position;
            
            // Обновляем размер на основе времени жизни
            float lifeRatio = particle.Lifetime / PARTICLE_LIFETIME;
            float sizeMultiplier = 1f - (lifeRatio * 0.5f); // Уменьшаем размер со временем
            transform.Scale = particle.Size * sizeMultiplier;
            
            // Обновляем вязкость на основе температуры
            particle.Viscosity = CalculateViscosity(particle.Temperature);
        }
        
        /// <summary>
        /// Очищает неактивные частицы
        /// </summary>
        private void CleanupInactiveParticles()
        {
            Entities
                .WithAll<InactiveParticleTag>()
                .ForEach((Entity entity, ref MudParticleData particle, ref LocalTransform transform) =>
                {
                    // Сбрасываем частицу
                    particle.Position = float3.zero;
                    particle.Velocity = float3.zero;
                    particle.Lifetime = 0f;
                    particle.IsActive = false;
                    
                    transform.Position = float3.zero;
                    transform.Scale = 0f;
                }).WithoutBurst().Run();
        }
        
        /// <summary>
        /// Управляет пулом частиц
        /// </summary>
        private void ManageParticlePool()
        {
            int activeCount = _activeParticlesQuery.CalculateEntityCount();
            int inactiveCount = _inactiveParticlesQuery.CalculateEntityCount();
            
            // Если неактивных частиц мало, создаем новые
            if (inactiveCount < 100 && _particlePool.Length < MAX_POOL_SIZE)
            {
                ExpandParticlePool();
            }
        }
        
        /// <summary>
        /// Расширяет пул частиц
        /// </summary>
        private void ExpandParticlePool()
        {
            int newSize = math.min(_particlePool.Length * 2, MAX_POOL_SIZE);
            var newPool = new NativeArray<Entity>(newSize, Allocator.Persistent);
            
            // Копируем существующие частицы
            for (int i = 0; i < _particlePool.Length; i++)
            {
                newPool[i] = _particlePool[i];
            }
            
            // Создаем новые частицы
            for (int i = _particlePool.Length; i < newSize; i++)
            {
                var entity = EntityManager.CreateEntity();
                EntityManager.AddComponentData(entity, new MudParticleData
                {
                    Position = float3.zero,
                    Velocity = float3.zero,
                    Size = 0.01f,
                    Lifetime = 0f,
                    Mass = 0.001f,
                    Temperature = 20f,
                    Viscosity = 0.5f,
                    IsActive = false
                });
                EntityManager.AddComponentData(entity, new LocalTransform
                {
                    Position = float3.zero,
                    Rotation = quaternion.identity,
                    Scale = 1f
                });
                EntityManager.AddComponentData(entity, new InactiveParticleTag());
                
                newPool[i] = entity;
            }
            
            // Освобождаем старый пул
            _particlePool.Dispose();
            _particlePool = newPool;
        }
        
        /// <summary>
        /// Получает частицу из пула
        /// </summary>
        public Entity GetParticleFromPool()
        {
            if (!_isPoolInitialized)
                InitializeParticlePool();
            
            // Ищем неактивную частицу
            for (int i = 0; i < _particlePool.Length; i++)
            {
                var entity = _particlePool[i];
                if (EntityManager.HasComponent<InactiveParticleTag>(entity))
                {
                    // Активируем частицу
                    EntityManager.RemoveComponent<InactiveParticleTag>(entity);
                    EntityManager.AddComponent<ActiveParticleTag>(entity);
                    
                    return entity;
                }
            }
            
            // Если не нашли, создаем новую
            return CreateNewParticle();
        }
        
        /// <summary>
        /// Возвращает частицу в пул
        /// </summary>
        public void ReturnParticleToPool(Entity particle)
        {
            if (!EntityManager.Exists(particle))
                return;
            
            // Деактивируем частицу
            EntityManager.RemoveComponent<ActiveParticleTag>(particle);
            EntityManager.AddComponent<InactiveParticleTag>(particle);
            
            // Сбрасываем данные
            var particleData = EntityManager.GetComponentData<MudParticleData>(particle);
            particleData.IsActive = false;
            particleData.Lifetime = 0f;
            EntityManager.SetComponentData(particle, particleData);
        }
        
        /// <summary>
        /// Создает новую частицу
        /// </summary>
        private Entity CreateNewParticle()
        {
            var entity = EntityManager.CreateEntity();
            EntityManager.AddComponentData(entity, new MudParticleData
            {
                Position = float3.zero,
                Velocity = float3.zero,
                Size = 0.01f,
                Lifetime = 0f,
                Mass = 0.001f,
                Temperature = 20f,
                Viscosity = 0.5f,
                IsActive = false
            });
            EntityManager.AddComponentData(entity, new LocalTransform
            {
                Position = float3.zero,
                Rotation = quaternion.identity,
                Scale = 1f
            });
            EntityManager.AddComponentData(entity, new InactiveParticleTag());
            
            return entity;
        }
        
        /// <summary>
        /// Вычисляет вязкость на основе температуры
        /// </summary>
        [BurstCompile]
        private static float CalculateViscosity(float temperature)
        {
            // Простая модель вязкости грязи
            float baseViscosity = 0.5f;
            float temperatureFactor = 1f - (temperature - 20f) / 100f;
            return math.clamp(baseViscosity * temperatureFactor, 0.1f, 1f);
        }
        
        /// <summary>
        /// Создает частицу грязи
        /// </summary>
        public Entity CreateMudParticle(float3 position, float3 velocity, float size, float temperature)
        {
            var entity = GetParticleFromPool();
            
            var particleData = new MudParticleData
            {
                Position = position,
                Velocity = velocity,
                Size = math.clamp(size, PARTICLE_SIZE_MIN, PARTICLE_SIZE_MAX),
                Lifetime = 0f,
                Mass = size * 0.1f,
                Temperature = temperature,
                Viscosity = CalculateViscosity(temperature),
                IsActive = true
            };
            
            EntityManager.SetComponentData(entity, particleData);
            EntityManager.SetComponentData(entity, new LocalTransform
            {
                Position = position,
                Rotation = quaternion.identity,
                Scale = size
            });
            
            return entity;
        }
        
        protected override void OnDestroy()
        {
            if (_particlePool.IsCreated)
            {
                _particlePool.Dispose();
            }
        }
    }
}