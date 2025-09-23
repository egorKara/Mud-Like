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
    /// Система синхронизации сетевых данных для Unity 6
    /// Оптимизирована для GhostSystem и новых возможностей NFE
    /// </summary>
    [UpdateInGroup(typeof(NetCodeClientAndServerSystemGroup))]
    [BurstCompile(CompileSynchronously = true)]
    public partial class NetworkSyncSystem : SystemBase
    {
        /// <summary>
        /// Обрабатывает синхронизацию сетевых данных для Unity 6
        /// </summary>
        protected override void OnUpdate()
        {
            // Проверяем, что мы в сетевой игре
            if (!HasSingleton<NetworkStreamInGame>()) return;
            
            // Синхронизируем позиции с приоритизацией
            SyncPositionsWithPriority();
            
            // Синхронизируем транспортные средства
            SyncVehicles();
            
            // Синхронизируем деформации террейна
            SyncTerrainDeformations();
            
            // Синхронизируем грязь
            SyncMud();
            
            // Обновляем статистику сети (Unity 6)
            UpdateNetworkStats();
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
        
        /// <summary>
        /// Синхронизирует позиции с приоритизацией (Unity 6)
        /// </summary>
        private void SyncPositionsWithPriority()
        {
            Entities
                .WithAll<NetworkPosition, NetworkId>()
                .ForEach((ref NetworkPosition networkPos, 
                         in LocalTransform transform, 
                         in NetworkId networkId) =>
                {
                    // Проверяем приоритет синхронизации
                    if (ShouldSyncPosition(networkId, networkPos))
                    {
                        // Проверяем, изменилась ли позиция
                        if (HasPositionChanged(networkPos, transform))
                        {
                            // Обновляем сетевые данные
                            networkPos.Value = transform.Position;
                            networkPos.Rotation = transform.Rotation;
                            networkPos.HasChanged = true;
                            networkPos.LastUpdateTime = (float)Time.time;
                            networkPos.Tick = (uint)Time.time;
                        }
                    }
                }).Schedule();
        }
        
        /// <summary>
        /// Определяет, нужно ли синхронизировать позицию (Unity 6)
        /// </summary>
        private static bool ShouldSyncPosition(in NetworkId networkId, in NetworkPosition networkPos)
        {
            // Высокий приоритет для авторитетных сущностей
            if (networkId.IsAuthoritative) return true;
            
            // Проверяем приоритет синхронизации
            if (networkId.UpdatePriority > 128) return true;
            
            // Проверяем флаг интерполяции
            if (networkPos.EnableInterpolation) return true;
            
            // Проверяем время с последнего обновления
            return (Time.time - networkPos.LastUpdateTime) > (1f / networkId.UpdatePriority);
        }
        
        /// <summary>
        /// Обновляет статистику сети (Unity 6)
        /// </summary>
        private void UpdateNetworkStats()
        {
            // Здесь можно добавить обновление статистики сети
            // Например, количество синхронизированных сущностей, пропускную способность и т.д.
        }
    }
}