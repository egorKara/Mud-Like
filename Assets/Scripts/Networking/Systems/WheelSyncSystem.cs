using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;
using MudLike.Vehicles.Components;
using MudLike.Networking.Components;

namespace MudLike.Networking.Systems
{
    /// <summary>
    /// Система синхронизации колес
    /// </summary>
    [UpdateInGroup(typeof(GhostUpdateSystemGroup))]
    public partial class WheelSyncSystem : SystemBase
    {
        /// <summary>
        /// Синхронизирует данные колес
        /// </summary>
        protected override void OnUpdate()
        {
            if (HasSingleton<NetworkStreamInGame>())
            {
                SyncWheelData();
            }
        }
        
        /// <summary>
        /// Синхронизирует данные колес
        /// </summary>
        private void SyncWheelData()
        {
            Entities
                .WithAll<WheelData, NetworkedWheelData>()
                .ForEach((ref NetworkedWheelData networkedData, in WheelData wheelData, in LocalTransform transform) =>
                {
                    // Обновляем сетевые данные из локальных данных
                    networkedData.Position = transform.Position;
                    networkedData.Rotation = transform.Rotation;
                    networkedData.AngularVelocity = wheelData.AngularVelocity;
                    networkedData.SteerAngle = wheelData.SteerAngle;
                    networkedData.Torque = wheelData.Torque;
                    networkedData.BrakeTorque = wheelData.BrakeTorque;
                    networkedData.TractionCoefficient = wheelData.TractionCoefficient;
                    networkedData.SinkDepth = wheelData.SinkDepth;
                    networkedData.SlipRatio = wheelData.SlipRatio;
                    networkedData.WheelIndex = wheelData.WheelIndex;
                }).WithoutBurst().Run();
        }
    }
}