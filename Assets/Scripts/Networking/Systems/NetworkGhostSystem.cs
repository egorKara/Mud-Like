using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.NetCode;
using Unity.Burst;
using Unity.Collections;
using MudLike.Networking.Components;
using MudLike.Core.Components;
using static MudLike.Core.Components.Position;

namespace MudLike.Networking.Systems
{
    /// <summary>
    /// Система управления Ghost сущностями для Unity 6
    /// Интегрирована с GhostSystem для оптимизации сетевого трафика
    /// </summary>
    [UpdateInGroup(typeof(NetCodeClientAndServerSystemGroup))]
    [BurstCompile(CompileSynchronously = true)]
    public partial class NetworkGhostSystem : SystemBase
    {
        private EntityQuery _ghostQuery;
        private EntityQuery _interpolationQuery;
        private EntityQuery _predictionQuery;
        
        protected override void OnCreate()
        {
            // Создаем запросы для различных типов Ghost сущностей
            _ghostQuery = GetEntityQuery(
                ComponentType.ReadWrite<NetworkPosition>(),
                ComponentType.ReadOnly<NetworkId>(),
                ComponentType.ReadOnly<LocalTransform>()
            );
            
            _interpolationQuery = GetEntityQuery(
                ComponentType.ReadWrite<NetworkPosition>(),
                ComponentType.ReadOnly<NetworkId>(),
                ComponentType.ReadOnly<LocalTransform>()
            );
            
            _predictionQuery = GetEntityQuery(
                ComponentType.ReadWrite<NetworkPosition>(),
                ComponentType.ReadOnly<NetworkId>(),
                ComponentType.ReadOnly<LocalTransform>()
            );
        }
        
        protected override void OnUpdate()
        {
            // Проверяем, что мы в сетевой игре
            if (!HasSingleton<NetworkStreamInGame>()) return;
            
            // Обновляем Ghost сущности
            UpdateGhostEntities();
            
            // Интерполируем позиции
            InterpolatePositions();
            
            // Предсказываем позиции
            PredictPositions();
        }
        
        /// <summary>
        /// Обновляет Ghost сущности
        /// </summary>
        private void UpdateGhostEntities()
        {
            var job = new UpdateGhostJob
            {
                DeltaTime = SystemAPI.Time.DeltaTime,
                CurrentTime = (float)Time.time
            };
            
            Dependency = job.ScheduleParallel(_ghostQuery, Dependency);
        }
        
        /// <summary>
        /// Интерполирует позиции для плавного движения
        /// </summary>
        private void InterpolatePositions()
        {
            var job = new InterpolationJob
            {
                DeltaTime = SystemAPI.Time.DeltaTime,
                InterpolationTime = 0.1f // 100ms интерполяция
            };
            
            Dependency = job.ScheduleParallel(_interpolationQuery, Dependency);
        }
        
        /// <summary>
        /// Предсказывает позиции для компенсации задержек
        /// </summary>
        private void PredictPositions()
        {
            var job = new PredictionJob
            {
                DeltaTime = SystemAPI.Time.DeltaTime,
                PredictionTime = 0.05f // 50ms предсказание
            };
            
            Dependency = job.ScheduleParallel(_predictionQuery, Dependency);
        }
        
        /// <summary>
        /// Job для обновления Ghost сущностей
        /// </summary>
        [BurstCompile]
        public partial struct UpdateGhostJob : IJobEntity
        {
            public float DeltaTime;
            public float CurrentTime;
            
            public void Execute(ref NetworkPosition networkPos, 
                              in NetworkId networkId, 
                              in LocalTransform transform)
            {
                // Обновляем только если сущность изменилась
                if (HasPositionChanged(networkPos, transform))
                {
                    networkPos.Value = transform.Position;
                    networkPos.Rotation = transform.Rotation;
                    networkPos.HasChanged = true;
                    networkPos.LastUpdateTime = CurrentTime;
                    networkPos.Tick = (uint)CurrentTime;
                }
                
                // Обновляем приоритет синхронизации
                UpdateSyncPriority(ref networkPos, networkId);
            }
            
            /// <summary>
            /// Обновляет приоритет синхронизации
            /// </summary>
            private static void UpdateSyncPriority(ref NetworkPosition networkPos, in NetworkId networkId)
            {
                // Высокий приоритет для авторитетных сущностей
                if (networkId.IsAuthoritative)
                {
                    networkPos.SyncPriority = 255;
                }
                else
                {
                    // Приоритет на основе скорости изменения
                    float velocity = math.length(networkPos.Velocity);
                    networkPos.SyncPriority = (byte)math.clamp(velocity * 10f, 0f, 255f);
                }
            }
            
            /// <summary>
            /// Проверяет, изменилась ли позиция
            /// </summary>
            private static bool HasPositionChanged(in NetworkPosition networkPos, in LocalTransform transform)
            {
                const float threshold = 0.01f;
                return math.distance(networkPos.Value, transform.Position) > threshold ||
                       math.distance(networkPos.Rotation.value, transform.Rotation.value) > threshold;
            }
        }
        
        /// <summary>
        /// Job для интерполяции позиций
        /// </summary>
        [BurstCompile]
        public partial struct InterpolationJob : IJobEntity
        {
            public float DeltaTime;
            public float InterpolationTime;
            
            public void Execute(ref NetworkPosition networkPos, 
                              in NetworkId networkId, 
                              in LocalTransform transform)
            {
                // Интерполируем только если включена интерполяция
                if (!networkPos.EnableInterpolation) return;
                
                // Вычисляем целевую позицию
                float3 targetPosition = networkPos.Value;
                quaternion targetRotation = networkPos.Rotation;
                
                // Интерполируем позицию
                float interpolationSpeed = 1f / InterpolationTime;
                networkPos.Value = math.lerp(transform.Position, targetPosition, interpolationSpeed * DeltaTime);
                networkPos.Rotation = math.slerp(transform.Rotation, targetRotation, interpolationSpeed * DeltaTime);
            }
        }
        
        /// <summary>
        /// Job для предсказания позиций
        /// </summary>
        [BurstCompile]
        public partial struct PredictionJob : IJobEntity
        {
            public float DeltaTime;
            public float PredictionTime;
            
            public void Execute(ref NetworkPosition networkPos, 
                              in NetworkId networkId, 
                              in LocalTransform transform)
            {
                // Предсказываем только если есть скорость
                if (math.length(networkPos.Velocity) < 0.1f) return;
                
                // Вычисляем предсказанную позицию
                float3 predictedPosition = networkPos.Value + networkPos.Velocity * PredictionTime;
                quaternion predictedRotation = networkPos.Rotation;
                
                // Применяем предсказание с весовым коэффициентом
                float predictionWeight = 0.7f;
                networkPos.Value = math.lerp(networkPos.Value, predictedPosition, predictionWeight);
                networkPos.Rotation = math.slerp(networkPos.Rotation, predictedRotation, predictionWeight);
            }
        }
    }
}
