using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.NetCode;
using MudLike.Networking.Components;
using MudLike.Core.Components;

namespace MudLike.Networking.Systems
{
    /// <summary>
    /// Система синхронизации сетевых данных
    /// </summary>
    [UpdateInGroup(typeof(NetCodeClientAndServerSystemGroup))]
    public partial class NetworkSyncSystem : SystemBase
    {
        /// <summary>
        /// Обрабатывает синхронизацию сетевых данных
        /// </summary>
        protected override void OnUpdate()
        {
            // Синхронизируем позиции
            SyncPositions();
            
            // Синхронизируем транспортные средства
            SyncVehicles();
            
            // Синхронизируем деформации террейна
            SyncTerrainDeformations();
            
            // Синхронизируем грязь
            SyncMud();
        }
        
        /// <summary>
        /// Синхронизирует позиции сущностей
        /// </summary>
        private void SyncPositions()
        {
            Entities
                .WithAll<NetworkPosition, NetworkId>()
                .ForEach((ref NetworkPosition networkPos, 
                         in LocalTransform transform, 
                         in NetworkId networkId) =>
                {
                    // Проверяем, изменилась ли позиция
                    if (HasPositionChanged(networkPos, transform))
                    {
                        // Обновляем сетевые данные
                        networkPos.Value = transform.Position;
                        networkPos.Rotation = transform.Rotation;
                        networkPos.HasChanged = true;
                        networkPos.LastUpdateTime = (float)Time.time;
                    }
                }).Schedule();
        }
        
        /// <summary>
        /// Синхронизирует транспортные средства
        /// </summary>
        private void SyncVehicles()
        {
            Entities
                .WithAll<NetworkVehicle, NetworkId>()
                .ForEach((ref NetworkVehicle networkVehicle, 
                         in VehicleConfig config, 
                         in VehiclePhysics physics, 
                         in VehicleInput input, 
                         in EngineData engine, 
                         in TransmissionData transmission) =>
                {
                    // Проверяем, изменились ли данные транспорта
                    if (HasVehicleChanged(networkVehicle, config, physics, input, engine, transmission))
                    {
                        // Обновляем сетевые данные
                        networkVehicle.Config = config;
                        networkVehicle.Physics = physics;
                        networkVehicle.Input = input;
                        networkVehicle.Engine = engine;
                        networkVehicle.Transmission = transmission;
                        networkVehicle.HasChanged = true;
                    }
                }).Schedule();
        }
        
        /// <summary>
        /// Синхронизирует деформации террейна
        /// </summary>
        private void SyncTerrainDeformations()
        {
            Entities
                .WithAll<NetworkDeformation, NetworkId>()
                .ForEach((ref NetworkDeformation networkDeformation, 
                         in DeformationData deformation) =>
                {
                    // Проверяем, изменилась ли деформация
                    if (HasDeformationChanged(networkDeformation, deformation))
                    {
                        // Обновляем сетевые данные
                        networkDeformation.Deformation = deformation;
                        networkDeformation.HasChanged = true;
                    }
                }).Schedule();
        }
        
        /// <summary>
        /// Синхронизирует грязь
        /// </summary>
        private void SyncMud()
        {
            Entities
                .WithAll<NetworkMud, NetworkId>()
                .ForEach((ref NetworkMud networkMud, in MudData mud) =>
                {
                    // Проверяем, изменилась ли грязь
                    if (HasMudChanged(networkMud, mud))
                    {
                        // Обновляем сетевые данные
                        networkMud.Mud = mud;
                        networkMud.HasChanged = true;
                    }
                }).Schedule();
        }
        
        /// <summary>
        /// Проверяет, изменилась ли позиция
        /// </summary>
        private static bool HasPositionChanged(in NetworkPosition networkPos, in LocalTransform transform)
        {
            float positionThreshold = 0.01f;
            float rotationThreshold = 0.01f;
            
            bool positionChanged = math.distance(networkPos.Value, transform.Position) > positionThreshold;
            bool rotationChanged = math.distance(networkPos.Rotation.value, transform.Rotation.value) > rotationThreshold;
            
            return positionChanged || rotationChanged;
        }
        
        /// <summary>
        /// Проверяет, изменились ли данные транспорта
        /// </summary>
        private static bool HasVehicleChanged(in NetworkVehicle networkVehicle, 
                                            in VehicleConfig config, 
                                            in VehiclePhysics physics, 
                                            in VehicleInput input, 
                                            in EngineData engine, 
                                            in TransmissionData transmission)
        {
            // Упрощенная проверка - в реальности нужно сравнивать все поля
            return !networkVehicle.Config.Equals(config) || 
                   !networkVehicle.Physics.Equals(physics) || 
                   !networkVehicle.Input.Equals(input) || 
                   !networkVehicle.Engine.Equals(engine) || 
                   !networkVehicle.Transmission.Equals(transmission);
        }
        
        /// <summary>
        /// Проверяет, изменилась ли деформация
        /// </summary>
        private static bool HasDeformationChanged(in NetworkDeformation networkDeformation, in DeformationData deformation)
        {
            return !networkDeformation.Deformation.Equals(deformation);
        }
        
        /// <summary>
        /// Проверяет, изменилась ли грязь
        /// </summary>
        private static bool HasMudChanged(in NetworkMud networkMud, in MudData mud)
        {
            return !networkMud.Mud.Equals(mud);
        }
    }
}