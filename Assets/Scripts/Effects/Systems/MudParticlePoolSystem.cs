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
    /// Система пулинга частиц грязи
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [BurstCompile]
    public partial class MudParticlePoolSystem : SystemBase
    {
        private const int INITIAL_POOL_SIZE = 1000;
        private const int MAX_POOL_SIZE = 10000;
        private const float PARTICLE_LIFETIME = 5.0f;
        
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
            float sizeMultiplier = 1f - (lifeRatio * 0.5f);
            transform.Scale = particle.Size * sizeMultiplier;
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
        
        protected override void OnDestroy()
        {
            if (_particlePool.IsCreated)
            {
                _particlePool.Dispose();
            }
        }
    }
}