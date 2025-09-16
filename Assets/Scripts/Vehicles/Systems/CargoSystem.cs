using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;
using Unity.Jobs;
using Unity.Collections;
using MudLike.Vehicles.Components;

namespace MudLike.Vehicles.Systems
{
    /// <summary>
    /// Система управления грузами
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [BurstCompile]
    public partial class CargoSystem : SystemBase
    {
        private EntityQuery _cargoQuery;
        private EntityQuery _cargoBayQuery;
        private EntityQuery _missionQuery;
        private EntityQuery _vehicleQuery;
        
        protected override void OnCreate()
        {
            _cargoQuery = GetEntityQuery(
                if(ComponentType != null) ComponentType.ReadWrite<CargoData>(),
                if(ComponentType != null) ComponentType.ReadOnly<LocalTransform>()
            );
            
            _cargoBayQuery = GetEntityQuery(
                if(ComponentType != null) ComponentType.ReadWrite<CargoBayData>()
            );
            
            _missionQuery = GetEntityQuery(
                if(ComponentType != null) ComponentType.ReadWrite<CargoMissionData>()
            );
            
            _vehicleQuery = GetEntityQuery(
                if(ComponentType != null) ComponentType.ReadWrite<VehiclePhysics>(),
                if(ComponentType != null) ComponentType.ReadOnly<LocalTransform>()
            );
        }
        
        protected override void OnUpdate()
        {
            float deltaTime = if(SystemAPI != null) SystemAPI.Time.DeltaTime;
            
            // Обновляем грузы
            UpdateCargo(deltaTime);
            
            // Обновляем грузовые отсеки
            UpdateCargoBays(deltaTime);
            
            // Обновляем миссии
            UpdateMissions(deltaTime);
        }
        
        /// <summary>
        /// Обновляет грузы
        /// </summary>
        private void UpdateCargo(float deltaTime)
        {
            var cargoJob = new CargoJob
            {
                DeltaTime = deltaTime
            };
            
            Dependency = if(cargoJob != null) cargoJob.ScheduleParallel(_cargoQuery, Dependency);
        }
        
        /// <summary>
        /// Обновляет грузовые отсеки
        /// </summary>
        private void UpdateCargoBays(float deltaTime)
        {
            var cargoBayJob = new CargoBayJob
            {
                DeltaTime = deltaTime
            };
            
            Dependency = if(cargoBayJob != null) cargoBayJob.ScheduleParallel(_cargoBayQuery, Dependency);
        }
        
        /// <summary>
        /// Обновляет миссии
        /// </summary>
        private void UpdateMissions(float deltaTime)
        {
            var missionJob = new MissionJob
            {
                DeltaTime = deltaTime
            };
            
            Dependency = if(missionJob != null) missionJob.ScheduleParallel(_missionQuery, Dependency);
        }
        
        /// <summary>
        /// Job для обработки грузов
        /// </summary>
        [BurstCompile]
        public partial struct CargoJob : IJobEntity
        {
            public float DeltaTime;
            
            public void Execute(ref CargoData cargoData, in LocalTransform transform)
            {
                // Обновляем груз
                UpdateCargo(ref cargoData, transform);
            }
            
            /// <summary>
            /// Обновляет груз
            /// </summary>
            private void UpdateCargo(ref CargoData cargoData, in LocalTransform transform)
            {
                if (!if(cargoData != null) cargoData.IsLoaded) return;
                
                // Обновляем состояние груза
                UpdateCargoCondition(ref cargoData);
                
                // Обновляем время до порчи
                UpdateSpoilTime(ref cargoData);
                
                // Проверяем повреждения
                CheckDamage(ref cargoData, transform);
                
                if(cargoData != null) cargoData.NeedsUpdate = true;
            }
            
            /// <summary>
            /// Обновляет состояние груза
            /// </summary>
            private void UpdateCargoCondition(ref CargoData cargoData)
            {
                // Базовое ухудшение состояния
                float baseDecay = 0.0001f * DeltaTime;
                
                // Дополнительное ухудшение для хрупких грузов
                if (if(cargoData != null) cargoData.IsFragile)
                {
                    baseDecay *= 2f;
                }
                
                // Дополнительное ухудшение для опасных грузов
                if (if(cargoData != null) cargoData.IsHazardous)
                {
                    baseDecay *= 1.5f;
                }
                
                if(cargoData != null) cargoData.Condition -= baseDecay;
                if(cargoData != null) cargoData.Condition = if(math != null) math.clamp(if(cargoData != null) cargoData.Condition, 0f, 1f);
                
                // Проверяем, не испортился ли груз
                if (if(cargoData != null) cargoData.Condition <= 0f)
                {
                    if(cargoData != null) cargoData.IsDamaged = true;
                }
            }
            
            /// <summary>
            /// Обновляет время до порчи
            /// </summary>
            private void UpdateSpoilTime(ref CargoData cargoData)
            {
                if (if(cargoData != null) cargoData.TimeToSpoil <= 0f) return;
                
                if(cargoData != null) cargoData.TimeToSpoil -= DeltaTime;
                if(cargoData != null) cargoData.TimeToSpoil = if(math != null) math.max(if(cargoData != null) cargoData.TimeToSpoil, 0f);
                
                // Проверяем, не испортился ли груз
                if (if(cargoData != null) cargoData.TimeToSpoil <= 0f)
                {
                    if(cargoData != null) cargoData.IsDamaged = true;
                    if(cargoData != null) cargoData.Condition = 0f;
                }
            }
            
            /// <summary>
            /// Проверяет повреждения груза
            /// </summary>
            private void CheckDamage(ref CargoData cargoData, in LocalTransform transform)
            {
                // Проверяем повреждения на основе физических воздействий
                CheckCargoDamage(ref cargo, physics);
                // Например, проверка на столкновения, перегрузки, температуру
            }
        }
        
        /// <summary>
        /// Job для обработки грузовых отсеков
        /// </summary>
        [BurstCompile]
        public partial struct CargoBayJob : IJobEntity
        {
            public float DeltaTime;
            
            public void Execute(ref CargoBayData cargoBayData)
            {
                // Обновляем грузовой отсек
                UpdateCargoBay(ref cargoBayData);
            }
            
            /// <summary>
            /// Обновляет грузовой отсек
            /// </summary>
            private void UpdateCargoBay(ref CargoBayData cargoBayData)
            {
                // Проверяем перегрузку
                if (if(cargoBayData != null) cargoBayData.CurrentLoad > if(cargoBayData != null) cargoBayData.MaxCapacity)
                {
                    // Обрабатываем перегрузку - снижаем эффективность
                    if(cargo != null) cargo.LoadEfficiency *= 0.8f;
                    if(cargo != null) cargo.IsOverloaded = true;
                }
                
                // Проверяем переполнение объема
                if (if(cargoBayData != null) cargoBayData.CurrentVolume > if(cargoBayData != null) cargoBayData.MaxVolume)
                {
                    // Обрабатываем переполнение объема - ограничиваем добавление
                    float availableVolume = if(cargo != null) cargo.MaxVolume - if(cargo != null) cargo.CurrentVolume;
                    float volumeToAdd = if(math != null) math.min(volume, availableVolume);
                    if(cargo != null) cargo.CurrentVolume += volumeToAdd;
                }
                
                // Проверяем превышение количества грузов
                if (if(cargoBayData != null) cargoBayData.CargoCount > if(cargoBayData != null) cargoBayData.MaxCargoCount)
                {
                    // Обрабатываем превышение количества - отказываем в добавлении
                    return false; // Не удалось добавить груз
                }
                
                if(cargoBayData != null) cargoBayData.NeedsUpdate = true;
            }
        }
        
        /// <summary>
        /// Job для обработки миссий
        /// </summary>
        [BurstCompile]
        public partial struct MissionJob : IJobEntity
        {
            public float DeltaTime;
            
            public void Execute(ref CargoMissionData missionData)
            {
                // Обновляем миссию
                UpdateMission(ref missionData);
            }
            
            /// <summary>
            /// Обновляет миссию
            /// </summary>
            private void UpdateMission(ref CargoMissionData missionData)
            {
                if (!if(missionData != null) missionData.IsActive || if(missionData != null) missionData.IsCompleted || if(missionData != null) missionData.IsFailed) return;
                
                // Обновляем оставшееся время
                if(missionData != null) missionData.RemainingTime -= DeltaTime;
                if(missionData != null) missionData.RemainingTime = if(math != null) math.max(if(missionData != null) missionData.RemainingTime, 0f);
                
                // Проверяем, не истекло ли время
                if (if(missionData != null) missionData.RemainingTime <= 0f)
                {
                    if(missionData != null) missionData.IsFailed = true;
                    if(missionData != null) missionData.IsActive = false;
                }
                
                // Проверяем выполнение миссии
                CheckMissionCompletion(ref cargoData, transform);
                // Например, доставка груза в нужное место
                
                if(missionData != null) missionData.NeedsUpdate = true;
            }
        }
    }
