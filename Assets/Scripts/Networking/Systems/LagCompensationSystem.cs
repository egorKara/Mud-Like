using Unity.Entities;
using Unity.NetCode;
using Unity.Mathematics;
using Unity.Transforms;
using MudLike.Vehicles.Components;
using MudLike.Networking.Components;

namespace MudLike.Networking.Systems
{
    /// <summary>
    /// Система лаг-компенсации для справедливого геймплея
    /// </summary>
    [UpdateInGroup(typeof(NetCodeReceiveSystemGroup))]
    public partial class LagCompensationSystem : SystemBase
    {
        /// <summary>
        /// Максимальное время отката для компенсации лага
        /// </summary>
        private const float MAX_REWIND_TIME = 0.2f; // 200мс
        
        /// <summary>
        /// Обрабатывает лаг-компенсацию
        /// </summary>
        protected override void OnUpdate()
        {
            if (!HasSingleton<NetworkStreamInGame>()) return;
            
            // Применяем лаг-компенсацию для грузовиков
            ApplyLagCompensation();
        }
        
        /// <summary>
        /// Применяет лаг-компенсацию
        /// </summary>
        private void ApplyLagCompensation()
        {
            // Получаем средний ping всех игроков
            float averagePing = CalculateAveragePing();
            float rewindTime = math.min(averagePing / 1000f, MAX_REWIND_TIME);
            
            // Применяем компенсацию для всех грузовиков
            Entities
                .WithAll<TruckData, NetworkedTruckData>()
                .ForEach((ref LocalTransform transform, in NetworkedTruckData networkedData) =>
                {
                    // Вычисляем компенсированную позицию
                    float3 compensatedPosition = CalculateCompensatedPosition(
                        networkedData.Position, 
                        networkedData.Velocity, 
                        rewindTime
                    );
                    
                    // Применяем компенсацию
                    transform.Position = compensatedPosition;
                }).WithoutBurst().Run();
        }
        
        /// <summary>
        /// Вычисляет средний ping всех игроков
        /// </summary>
        private float CalculateAveragePing()
        {
            float totalPing = 0f;
            int playerCount = 0;
            
            Entities
                .WithAll<PlayerConnectionData>()
                .ForEach((in PlayerConnectionData connectionData) =>
                {
                    totalPing += connectionData.Ping;
                    playerCount++;
                }).WithoutBurst().Run();
            
            return playerCount > 0 ? totalPing / playerCount : 0f;
        }
        
        /// <summary>
        /// Вычисляет компенсированную позицию
        /// </summary>
        private static float3 CalculateCompensatedPosition(float3 position, float3 velocity, float rewindTime)
        {
            // Простая линейная экстраполяция
            return position - velocity * rewindTime;
        }
    }
}