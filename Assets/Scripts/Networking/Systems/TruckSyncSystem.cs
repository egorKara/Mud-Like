using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;
using MudLike.Vehicles.Components;
using MudLike.Networking.Components;

namespace MudLike.Networking.Systems
{
    /// <summary>
    /// Система синхронизации грузовика
    /// </summary>
    [UpdateInGroup(typeof(GhostUpdateSystemGroup))]
    public partial class TruckSyncSystem : SystemBase
    {
        /// <summary>
        /// Синхронизирует данные грузовика
        /// </summary>
        protected override void OnUpdate()
        {
            // Синхронизация с сервера на клиенты
            if (HasSingleton<NetworkStreamInGame>())
            {
                SyncTruckData();
            }
        }
        
        /// <summary>
        /// Синхронизирует данные грузовика
        /// </summary>
        private void SyncTruckData()
        {
            Entities
                .WithAll<TruckData, NetworkedTruckData>()
                .ForEach((ref NetworkedTruckData networkedData, in TruckData truckData, in LocalTransform transform) =>
                {
                    // Обновляем сетевые данные из локальных данных
                    networkedData.Position = transform.Position;
                    networkedData.Rotation = transform.Rotation;
                    networkedData.Velocity = truckData.CurrentSpeed * math.forward(transform.Rotation);
                    networkedData.AngularVelocity = float3.zero; // Упрощенно
                    networkedData.CurrentGear = truckData.CurrentGear;
                    networkedData.EngineRPM = truckData.EngineRPM;
                    networkedData.CurrentSpeed = truckData.CurrentSpeed;
                    networkedData.SteeringAngle = truckData.SteeringAngle;
                    networkedData.EngineRunning = truckData.EngineRunning;
                    networkedData.HandbrakeOn = truckData.HandbrakeOn;
                    networkedData.LockFrontDifferential = truckData.LockFrontDifferential;
                    networkedData.LockMiddleDifferential = truckData.LockMiddleDifferential;
                    networkedData.LockRearDifferential = truckData.LockRearDifferential;
                    networkedData.LockCenterDifferential = truckData.LockCenterDifferential;
                    networkedData.FuelLevel = truckData.FuelLevel;
                }).WithoutBurst().Run();
        }
    }
}