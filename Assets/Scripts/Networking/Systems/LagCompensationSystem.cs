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
    /// Система компенсации задержек для обеспечения честного мультиплеера
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [BurstCompile]
    public partial class LagCompensationSystem : SystemBase
    {
        private const float MAX_LAG_COMPENSATION_TIME = 1.0f; // Максимальное время компенсации (секунды)
        private const float MIN_PING_FOR_COMPENSATION = 50f; // Минимальный пинг для компенсации (мс)
        private const float LAG_COMPENSATION_FACTOR = 0.5f; // Фактор компенсации задержки
        private const int MAX_HISTORY_SIZE = 100; // Максимальный размер истории состояний
        
        /// <summary>
        /// Обрабатывает компенсацию задержек для всех игроков
        /// </summary>
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
        
        /// <summary>
        /// Вычисляет оптимальное время компенсации
        /// </summary>
        [BurstCompile]
        private static float CalculateOptimalCompensationTime(float ping, float jitter)
        {
            // Базовое время компенсации на основе пинга
            float baseCompensation = ping / 1000f * 0.5f;
            
            // Добавляем компенсацию для джиттера
            float jitterCompensation = jitter / 1000f * 0.3f;
            
            // Итоговое время компенсации
            float totalCompensation = baseCompensation + jitterCompensation;
            
            // Ограничиваем максимальным временем
            return math.min(totalCompensation, MAX_LAG_COMPENSATION_TIME);
        }
        
        /// <summary>
        /// Применяет компенсацию к вводу
        /// </summary>
        [BurstCompile]
        private static void ApplyInputCompensation(ref VehicleInput input, 
                                                 ref NetworkData networkData, 
                                                 float compensationTime)
        {
            // Усиливаем ввод для компенсации задержки
            input.Vertical *= (1f + compensationTime);
            input.Horizontal *= (1f + compensationTime);
            
            // Применяем сглаживание для избежания резких изменений
            input.Vertical = math.clamp(input.Vertical, -1f, 1f);
            input.Horizontal = math.clamp(input.Horizontal, -1f, 1f);
        }
        
        /// <summary>
        /// Предсказывает будущее состояние
        /// </summary>
        [BurstCompile]
        private static void PredictFutureState(ref LocalTransform transform, 
                                             ref VehiclePhysics physics, 
                                             float predictionTime)
        {
            // Предсказываем позицию
            transform.Position += physics.Velocity * predictionTime;
            
            // Предсказываем поворот
            quaternion rotationDelta = quaternion.Euler(physics.AngularVelocity * predictionTime);
            transform.Rotation = math.mul(transform.Rotation, rotationDelta);
            
            // Предсказываем скорость
            physics.Velocity += physics.Acceleration * predictionTime;
            physics.AngularVelocity += physics.AngularAcceleration * predictionTime;
        }
        
        /// <summary>
        /// Корректирует состояние на основе серверных данных
        /// </summary>
        [BurstCompile]
        private static void CorrectState(ref LocalTransform transform, 
                                       ref VehiclePhysics physics, 
                                       in LocalTransform serverTransform, 
                                       in VehiclePhysics serverPhysics, 
                                       float correctionFactor)
        {
            // Корректируем позицию
            transform.Position = math.lerp(transform.Position, serverTransform.Position, correctionFactor);
            
            // Корректируем поворот
            transform.Rotation = math.slerp(transform.Rotation, serverTransform.Rotation, correctionFactor);
            
            // Корректируем скорость
            physics.Velocity = math.lerp(physics.Velocity, serverPhysics.Velocity, correctionFactor);
            physics.AngularVelocity = math.lerp(physics.AngularVelocity, serverPhysics.AngularVelocity, correctionFactor);
        }
        
        /// <summary>
        /// Вычисляет качество соединения
        /// </summary>
        [BurstCompile]
        private static float CalculateConnectionQuality(float ping, float jitter, float packetLoss)
        {
            // Нормализуем значения
            float normalizedPing = math.clamp(1f - (ping / 200f), 0f, 1f);
            float normalizedJitter = math.clamp(1f - (jitter / 50f), 0f, 1f);
            float normalizedPacketLoss = math.clamp(1f - packetLoss, 0f, 1f);
            
            // Вычисляем общее качество
            float quality = (normalizedPing + normalizedJitter + normalizedPacketLoss) / 3f;
            
            return math.clamp(quality, 0f, 1f);
        }
        
        /// <summary>
        /// Применяет адаптивную компенсацию
        /// </summary>
        [BurstCompile]
        private static void ApplyAdaptiveCompensation(ref NetworkData networkData, 
                                                    float connectionQuality, 
                                                    float deltaTime)
        {
            // Адаптируем фактор компенсации на основе качества соединения
            float targetCompensationFactor = 1f + (1f - connectionQuality) * 0.5f;
            
            // Плавно изменяем фактор компенсации
            networkData.CompensationFactor = math.lerp(networkData.CompensationFactor, 
                                                     targetCompensationFactor, 
                                                     deltaTime * 2f);
        }
    }
}