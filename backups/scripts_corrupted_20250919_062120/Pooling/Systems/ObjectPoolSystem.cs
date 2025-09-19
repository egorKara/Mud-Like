using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;
using Unity.Jobs;
using Unity.Collections;
using MudLike.Pooling.Components;

namespace MudLike.Pooling.Systems
{
    /// <summary>
    /// Система пула объектов для оптимизации памяти
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [BurstCompile(CompileSynchronously = true)]
    public partial class ObjectPoolSystem : SystemBase
    {
        private EntityQuery _poolQuery;
        private EntityQuery _pooledObjectQuery;
        private EntityQuery _particleQuery;
        
        protected override void OnCreate()
        {
            _poolQuery = GetEntityQuery(
                ComponentType.ReadWrite<ObjectPoolData>()
            );
            
            _pooledObjectQuery = GetEntityQuery(
                ComponentType.ReadWrite<PooledObjectData>(),
                ComponentType.ReadWrite<LocalTransform>()
            );
            
            _particleQuery = GetEntityQuery(
                ComponentType.ReadWrite<PooledParticleData>(),
                ComponentType.ReadWrite<LocalTransform>()
            );
        }
        
        protected override void OnUpdate()
        {
            float deltaTime = Time.deltaTime;
            
            // Обновляем пулы объектов
            UpdateObjectPools(deltaTime);
            
            // Обновляем объекты в пулах
            UpdatePooledObjects(deltaTime);
            
            // Обновляем частицы в пулах
            UpdatePooledParticles(deltaTime);
        }
        
        /// <summary>
        /// Обновляет пулы объектов
        /// </summary>
        private void UpdateObjectPools(float deltaTime)
        {
            var poolJob = new ObjectPoolJob
            {
                DeltaTime = deltaTime
            };
            
            Dependency = poolJob.ScheduleParallel(_poolQuery, Dependency);
        }
        
        /// <summary>
        /// Обновляет объекты в пулах
        /// </summary>
        private void UpdatePooledObjects(float deltaTime)
        {
            var pooledObjectJob = new PooledObjectJob
            {
                DeltaTime = deltaTime
            };
            
            Dependency = pooledObjectJob.ScheduleParallel(_pooledObjectQuery, Dependency);
        }
        
        /// <summary>
        /// Обновляет частицы в пулах
        /// </summary>
        private void UpdatePooledParticles(float deltaTime)
        {
            var particleJob = new PooledParticleJob
            {
                DeltaTime = deltaTime
            };
            
            Dependency = particleJob.ScheduleParallel(_particleQuery, Dependency);
        }
        
        /// <summary>
        /// Job для обработки пулов объектов
        /// </summary>
        [BurstCompile]
        public partial struct ObjectPoolJob : IJobEntity
        {
            public float DeltaTime;
            
            public void Execute(ref ObjectPoolData poolData)
            {
                if (!poolData.IsActive) return;
                
                // Обновляем статистику пула
                UpdatePoolStatistics(ref poolData);
                
                // Проверяем, нужно ли очистить пул
                if (poolData.AutoDestroy)
                {
                    CleanupPool(ref poolData);
                }
                
                poolData.NeedsUpdate = true;
            }
            
            /// <summary>
            /// Обновляет статистику пула
            /// </summary>
            private void UpdatePoolStatistics(ref ObjectPoolData poolData)
            {
                // Подсчитываем активные и неактивные объекты
                CountActiveAndInactiveObjects();
                // Это требует доступа к EntityManager, что сложно в Job
            }
            
            /// <summary>
            /// Очищает пул от старых объектов
            /// </summary>
            private void CleanupPool(ref ObjectPoolData poolData)
            {
                // Очищаем пул
                ClearPool();
                // Это требует доступа к EntityManager, что сложно в Job
            }
        }
        
        /// <summary>
        /// Job для обработки объектов в пулах
        /// </summary>
        [BurstCompile]
        public partial struct PooledObjectJob : IJobEntity
        {
            public float DeltaTime;
            
            public void Execute(ref PooledObjectData objectData, ref LocalTransform transform)
            {
                if (!objectData.IsActive) return;
                
                // Обновляем время последнего использования
                objectData.LastUsedTime = Time.time;
                
                // Проверяем, нужно ли деактивировать объект
                if (ShouldDeactivateObject(objectData))
                {
                    DeactivateObject(ref objectData, ref transform);
                }
                
                objectData.NeedsUpdate = true;
            }
            
            /// <summary>
            /// Проверяет, нужно ли деактивировать объект
            /// </summary>
            private bool ShouldDeactivateObject(PooledObjectData objectData)
            {
                // Деактивируем объект, если он не использовался долгое время
                return Time.time - objectData.LastUsedTime > 30f; // 30 секунд
            }
            
            /// <summary>
            /// Деактивирует объект
            /// </summary>
            private void DeactivateObject(ref PooledObjectData objectData, ref LocalTransform transform)
            {
                objectData.IsActive = false;
                transform.Position = new float3(0f, -1000f, 0f); // Перемещаем в невидимое место
            }
        }
        
        /// <summary>
        /// Job для обработки частиц в пулах
        /// </summary>
        [BurstCompile]
        public partial struct PooledParticleJob : IJobEntity
        {
            public float DeltaTime;
            
            public void Execute(ref PooledParticleData particleData, ref LocalTransform transform)
            {
                if (!particleData.IsActive) return;
                
                // Обновляем частицу
                UpdateParticle(ref particleData, ref transform);
                
                // Проверяем, нужно ли деактивировать частицу
                if (ShouldDeactivateParticle(particleData))
                {
                    DeactivateParticle(ref particleData, ref transform);
                }
                
                particleData.NeedsUpdate = true;
            }
            
            /// <summary>
            /// Обновляет частицу
            /// </summary>
            private void UpdateParticle(ref PooledParticleData particleData, ref LocalTransform transform)
            {
                // Обновляем время жизни
                particleData.Lifetime += DeltaTime;
                
                // Обновляем позицию
                particleData.Position += particleData.Velocity * DeltaTime;
                transform.Position = particleData.Position;
                
                // Обновляем скорость
                particleData.Velocity += particleData.Acceleration * DeltaTime;
                
                // Обновляем размер
                float sizeMultiplier = 1f - (particleData.Lifetime / particleData.MaxLifetime);
                particleData.Size *= sizeMultiplier;
                
                // Обновляем прозрачность
                particleData.Alpha = 1f - (particleData.Lifetime / particleData.MaxLifetime);
            }
            
            /// <summary>
            /// Проверяет, нужно ли деактивировать частицу
            /// </summary>
            private bool ShouldDeactivateParticle(PooledParticleData particleData)
            {
                return particleData.Lifetime >= particleData.MaxLifetime;
            }
            
            /// <summary>
            /// Деактивирует частицу
            /// </summary>
            private void DeactivateParticle(ref PooledParticleData particleData, ref LocalTransform transform)
            {
                particleData.IsActive = false;
                particleData.Lifetime = 0f;
                transform.Position = new float3(0f, -1000f, 0f); // Перемещаем в невидимое место
            }
        }
    }
}