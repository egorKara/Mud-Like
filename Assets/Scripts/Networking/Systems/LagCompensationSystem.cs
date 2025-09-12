using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;
using Unity.Collections;
using MudLike.Networking.Components;
using MudLike.Vehicles.Components;

namespace MudLike.Networking.Systems
{
    /// <summary>
    /// Система компенсации задержек для мультиплеера
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [BurstCompile]
    public partial class LagCompensationSystem : SystemBase
    {
        private const float MAX_LAG_COMPENSATION_TIME = 1.0f;
        private const float MIN_PING_FOR_COMPENSATION = 50f;
        private const float LAG_COMPENSATION_FACTOR = 0.5f;
        private const int MAX_HISTORY_SIZE = 100;
        
        protected override void OnUpdate()
        {
            float deltaTime = SystemAPI.Time.DeltaTime;
            
            Entities
                .WithAll<PlayerTag, NetworkData>()
                .ForEach((ref LocalTransform transform, 
                         ref VehiclePhysics physics, 
                         ref NetworkData networkData) =>
                {
                    CompensateLag(ref transform, ref physics, ref networkData, deltaTime);
                }).Schedule();
        }
        
        /// <summary>
        /// Компенсирует задержку для игрока
        /// </summary>
        [BurstCompile]
        private static void CompensateLag(ref LocalTransform transform, 
                                        ref VehiclePhysics physics, 
                                        ref NetworkData networkData, 
                                        float deltaTime)
        {
            // Проверяем, нужна ли компенсация
            if (networkData.Ping < MIN_PING_FOR_COMPENSATION)
            {
                networkData.CompensationFactor = 1f;
                return;
            }
            
            // Вычисляем время компенсации
            float compensationTime = networkData.Ping / 1000f * LAG_COMPENSATION_FACTOR;
            compensationTime = math.min(compensationTime, MAX_LAG_COMPENSATION_TIME);
            
            // Применяем компенсацию к физике
            ApplyPhysicsCompensation(ref physics, compensationTime);
            
            // Применяем компенсацию к позиции
            ApplyPositionCompensation(ref transform, ref physics, compensationTime);
            
            // Обновляем фактор компенсации
            networkData.CompensationFactor = 1f + compensationTime;
            
            // Обновляем время последнего обновления
            networkData.LastUpdateTime = 0f;
        }
        
        /// <summary>
        /// Применяет компенсацию к физике
        /// </summary>
        [BurstCompile]
        private static void ApplyPhysicsCompensation(ref VehiclePhysics physics, float compensationTime)
        {
            // Компенсируем скорость
            physics.Velocity *= (1f + compensationTime);
            
            // Компенсируем угловую скорость
            physics.AngularVelocity *= (1f + compensationTime);
            
            // Компенсируем ускорение
            physics.Acceleration *= (1f + compensationTime);
            physics.AngularAcceleration *= (1f + compensationTime);
        }
        
        /// <summary>
        /// Применяет компенсацию к позиции
        /// </summary>
        [BurstCompile]
        private static void ApplyPositionCompensation(ref LocalTransform transform, 
                                                    ref VehiclePhysics physics, 
                                                    float compensationTime)
        {
            // Предсказываем позицию с учетом задержки
            float3 predictedPosition = transform.Position + physics.Velocity * compensationTime;
            
            // Применяем сглаживание для избежания резких скачков
            transform.Position = math.lerp(transform.Position, predictedPosition, 0.1f);
        }
    }
}