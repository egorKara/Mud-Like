using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Burst;
using Unity.Collections;
using MudLike.Networking.Components;
using MudLike.Core.Components;

namespace MudLike.Networking.Systems
{
    /// <summary>
    /// Система Unity 6 Multiplayer Center
    /// Интегрирована с новыми возможностями Unity 6 для мультиплеера
    /// </summary>
    [UpdateInGroup(typeof(NetCodeClientAndServerSystemGroup))]
    [BurstCompile(CompileSynchronously = true)]
    public partial class Unity6MultiplayerSystem : SystemBase
    {
        private EntityQuery _playerQuery;
        private EntityQuery _vehicleQuery;
        private EntityQuery _terrainQuery;
        
        protected override void OnCreate()
        {
            // Создаем запросы для различных типов сущностей
            _playerQuery = GetEntityQuery(
                ComponentType.ReadWrite<NetworkPosition>(),
                ComponentType.ReadOnly<NetworkId>(),
                ComponentType.ReadOnly<PlayerTag>()
            );
            
            _vehicleQuery = GetEntityQuery(
                ComponentType.ReadWrite<NetworkVehicle>(),
                ComponentType.ReadOnly<NetworkId>()
            );
            
            _terrainQuery = GetEntityQuery(
                ComponentType.ReadWrite<NetworkDeformation>(),
                ComponentType.ReadOnly<NetworkId>()
            );
        }
        
        protected override void OnUpdate()
        {
            // Проверяем, что мы в сетевой игре
            if (!HasSingleton<NetworkStreamInGame>()) return;
            
            // Обновляем игроков
            UpdatePlayers();
            
            // Обновляем транспорт
            UpdateVehicles();
            
            // Обновляем террейн
            UpdateTerrain();
            
            // Обновляем статистику мультиплеера
            UpdateMultiplayerStats();
        }
        
        /// <summary>
        /// Обновляет игроков
        /// </summary>
        private void UpdatePlayers()
        {
            var job = new UpdatePlayerJob
            {
                DeltaTime = SystemAPI.Time.DeltaTime,
                CurrentTime = (float)Time.time
            };
            
            Dependency = job.ScheduleParallel(_playerQuery, Dependency);
        }
        
        /// <summary>
        /// Обновляет транспорт
        /// </summary>
        private void UpdateVehicles()
        {
            var job = new UpdateVehicleJob
            {
                DeltaTime = SystemAPI.Time.DeltaTime,
                CurrentTime = (float)Time.time
            };
            
            Dependency = job.ScheduleParallel(_vehicleQuery, Dependency);
        }
        
        /// <summary>
        /// Обновляет террейн
        /// </summary>
        private void UpdateTerrain()
        {
            var job = new UpdateTerrainJob
            {
                DeltaTime = SystemAPI.Time.DeltaTime,
                CurrentTime = (float)Time.time
            };
            
            Dependency = job.ScheduleParallel(_terrainQuery, Dependency);
        }
        
        /// <summary>
        /// Обновляет статистику мультиплеера
        /// </summary>
        private void UpdateMultiplayerStats()
        {
            // Здесь можно добавить обновление статистики мультиплеера
            // Например, количество игроков, транспортных средств, деформаций и т.д.
        }
        
        /// <summary>
        /// Job для обновления игроков
        /// </summary>
        [BurstCompile]
        public partial struct UpdatePlayerJob : IJobEntity
        {
            public float DeltaTime;
            public float CurrentTime;
            
            public void Execute(ref NetworkPosition networkPos, 
                              in NetworkId networkId, 
                              in PlayerTag playerTag)
            {
                // Обновляем приоритет для игроков
                networkPos.SyncPriority = 255; // Высший приоритет для игроков
                networkPos.EnableInterpolation = true;
                
                // Обновляем время последнего обновления
                networkPos.LastUpdateTime = CurrentTime;
                networkPos.Tick = (uint)CurrentTime;
            }
        }
        
        /// <summary>
        /// Job для обновления транспорта
        /// </summary>
        [BurstCompile]
        public partial struct UpdateVehicleJob : IJobEntity
        {
            public float DeltaTime;
            public float CurrentTime;
            
            public void Execute(ref NetworkVehicle networkVehicle, 
                              in NetworkId networkId)
            {
                // Обновляем приоритет для транспорта
                networkVehicle.SyncPriority = 200; // Высокий приоритет для транспорта
                networkVehicle.EnableInterpolation = true;
                
                // Обновляем время последнего обновления
                networkVehicle.LastUpdateTime = CurrentTime;
                
                // Обновляем состояние транспорта
                UpdateVehicleState(ref networkVehicle);
            }
            
            /// <summary>
            /// Обновляет состояние транспорта
            /// </summary>
            private static void UpdateVehicleState(ref NetworkVehicle networkVehicle)
            {
                // Определяем состояние транспорта на основе физики
                if (math.length(networkVehicle.Physics.Velocity) > 0.1f)
                {
                    networkVehicle.VehicleState = 1; // Движется
                }
                else
                {
                    networkVehicle.VehicleState = 0; // Стоит
                }
            }
        }
        
        /// <summary>
        /// Job для обновления террейна
        /// </summary>
        [BurstCompile]
        public partial struct UpdateTerrainJob : IJobEntity
        {
            public float DeltaTime;
            public float CurrentTime;
            
            public void Execute(ref NetworkDeformation networkDeformation, 
                              in NetworkId networkId)
            {
                // Обновляем приоритет для террейна
                networkDeformation.SyncPriority = 100; // Средний приоритет для террейна
                
                // Обновляем время последнего обновления
                networkDeformation.LastUpdateTime = CurrentTime;
            }
        }
    }
}
