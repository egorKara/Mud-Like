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
                ComponentType.ReadWrite<CargoData>(),
                ComponentType.ReadOnly<LocalTransform>()
            );
            
            _cargoBayQuery = GetEntityQuery(
                ComponentType.ReadWrite<CargoBayData>()
            );
            
            _missionQuery = GetEntityQuery(
                ComponentType.ReadWrite<CargoMissionData>()
            );
            
            _vehicleQuery = GetEntityQuery(
                ComponentType.ReadWrite<VehiclePhysics>(),
                ComponentType.ReadOnly<LocalTransform>()
            );
        }
        
        protected override void OnUpdate()
        {
            float deltaTime = Time.deltaTime;
            
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
            
            Dependency = cargoJob.ScheduleParallel(_cargoQuery, Dependency);
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
            
            Dependency = cargoBayJob.ScheduleParallel(_cargoBayQuery, Dependency);
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
            
            Dependency = missionJob.ScheduleParallel(_missionQuery, Dependency);
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
                if (!cargoData.IsLoaded) return;
                
                // Обновляем состояние груза
                UpdateCargoCondition(ref cargoData);
                
                // Обновляем время до порчи
                UpdateSpoilTime(ref cargoData);
                
                // Проверяем повреждения
                CheckDamage(ref cargoData, transform);
                
                cargoData.NeedsUpdate = true;
            }
            
            /// <summary>
            /// Обновляет состояние груза
            /// </summary>
            private void UpdateCargoCondition(ref CargoData cargoData)
            {
                // Базовое ухудшение состояния
                float baseDecay = 0.0001f * DeltaTime;
                
                // Дополнительное ухудшение для хрупких грузов
                if (cargoData.IsFragile)
                {
                    baseDecay *= 2f;
                }
                
                // Дополнительное ухудшение для опасных грузов
                if (cargoData.IsHazardous)
                {
                    baseDecay *= 1.5f;
                }
                
                cargoData.Condition -= baseDecay;
                cargoData.Condition = math.clamp(cargoData.Condition, 0f, 1f);
                
                // Проверяем, не испортился ли груз
                if (cargoData.Condition <= 0f)
                {
                    cargoData.IsDamaged = true;
                }
            }
            
            /// <summary>
            /// Обновляет время до порчи
            /// </summary>
            private void UpdateSpoilTime(ref CargoData cargoData)
            {
                if (cargoData.TimeToSpoil <= 0f) return;
                
                cargoData.TimeToSpoil -= DeltaTime;
                cargoData.TimeToSpoil = math.max(cargoData.TimeToSpoil, 0f);
                
                // Проверяем, не испортился ли груз
                if (cargoData.TimeToSpoil <= 0f)
                {
                    cargoData.IsDamaged = true;
                    cargoData.Condition = 0f;
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
                if (cargoBayData.CurrentLoad > cargoBayData.MaxCapacity)
                {
                    // Обрабатываем перегрузку - снижаем эффективность
                    cargo.LoadEfficiency *= 0.8f;
                    cargo.IsOverloaded = true;
                }
                
                // Проверяем переполнение объема
                if (cargoBayData.CurrentVolume > cargoBayData.MaxVolume)
                {
                    // Обрабатываем переполнение объема - ограничиваем добавление
                    float availableVolume = cargo.MaxVolume - cargo.CurrentVolume;
                    float volumeToAdd = math.min(volume, availableVolume);
                    cargo.CurrentVolume += volumeToAdd;
                }
                
                // Проверяем превышение количества грузов
                if (cargoBayData.CargoCount > cargoBayData.MaxCargoCount)
                {
                    // Обрабатываем превышение количества - отказываем в добавлении
                    return false; // Не удалось добавить груз
                }
                
                cargoBayData.NeedsUpdate = true;
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
                if (!missionData.IsActive || missionData.IsCompleted || missionData.IsFailed) return;
                
                // Обновляем оставшееся время
                missionData.RemainingTime -= DeltaTime;
                missionData.RemainingTime = math.max(missionData.RemainingTime, 0f);
                
                // Проверяем, не истекло ли время
                if (missionData.RemainingTime <= 0f)
                {
                    missionData.IsFailed = true;
                    missionData.IsActive = false;
                }
                
                // Проверяем выполнение миссии
                CheckMissionCompletion(ref cargoData, transform);
                // Например, доставка груза в нужное место
                
                missionData.NeedsUpdate = true;
            }
        }
    }
}