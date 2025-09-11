using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.NetCode;
using MudLike.Networking.Components;

namespace MudLike.Networking.Systems
{
    /// <summary>
    /// Система компенсации задержек сети
    /// </summary>
    [UpdateInGroup(typeof(NetCodeClientAndServerSystemGroup))]
    public partial class LagCompensationSystem : SystemBase
    {
        /// <summary>
        /// Обрабатывает компенсацию задержек
        /// </summary>
        protected override void OnUpdate()
        {
            // Компенсируем задержки для движения
            CompensateMovementLag();
            
            // Компенсируем задержки для взаимодействий
            CompensateInteractionLag();
        }
        
        /// <summary>
        /// Компенсирует задержки движения
        /// </summary>
        private void CompensateMovementLag()
        {
            Entities
                .WithAll<NetworkPosition, NetworkId>()
                .ForEach((ref NetworkPosition networkPos, 
                         ref LocalTransform transform, 
                         in NetworkId networkId) =>
                {
                    // Вычисляем задержку
                    float lag = CalculateNetworkLag(networkId);
                    
                    // Применяем компенсацию
                    ApplyMovementCompensation(ref networkPos, ref transform, lag);
                }).Schedule();
        }
        
        /// <summary>
        /// Компенсирует задержки взаимодействий
        /// </summary>
        private void CompensateInteractionLag()
        {
            Entities
                .WithAll<NetworkDeformation, NetworkId>()
                .ForEach((ref NetworkDeformation networkDeformation, 
                         in NetworkId networkId) =>
                {
                    // Вычисляем задержку
                    float lag = CalculateNetworkLag(networkId);
                    
                    // Применяем компенсацию
                    ApplyInteractionCompensation(ref networkDeformation, lag);
                }).Schedule();
        }
        
        /// <summary>
        /// Вычисляет задержку сети
        /// </summary>
        private static float CalculateNetworkLag(in NetworkId networkId)
        {
            // Упрощенная реализация - в реальности нужно использовать RTT
            return 0.1f; // 100ms задержка
        }
        
        /// <summary>
        /// Применяет компенсацию движения
        /// </summary>
        private static void ApplyMovementCompensation(ref NetworkPosition networkPos, 
                                                    ref LocalTransform transform, 
                                                    float lag)
        {
            // Предсказываем позицию с учетом задержки
            float3 predictedPosition = networkPos.Value + networkPos.Velocity * lag;
            quaternion predictedRotation = math.mul(transform.Rotation, 
                quaternion.RotateY(networkPos.AngularVelocity.y * lag));
            
            // Применяем предсказание
            transform.Position = predictedPosition;
            transform.Rotation = predictedRotation;
        }
        
        /// <summary>
        /// Применяет компенсацию взаимодействий
        /// </summary>
        private static void ApplyInteractionCompensation(ref NetworkDeformation networkDeformation, float lag)
        {
            // Корректируем время деформации с учетом задержки
            networkDeformation.Deformation.Time += lag;
        }
    }
}