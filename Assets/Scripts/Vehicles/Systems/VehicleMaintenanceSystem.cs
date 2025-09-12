using Unity.Entities;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Jobs;
using MudLike.Vehicles.Components;

namespace MudLike.Vehicles.Systems
{
    /// <summary>
    /// Система технического обслуживания транспортного средства
    /// </summary>
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile]
    public partial class VehicleMaintenanceSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            float deltaTime = SystemAPI.Time.fixedDeltaTime;
            
            var maintenanceJob = new VehicleMaintenanceJob
            {
                DeltaTime = deltaTime
            };
            
            Dependency = maintenanceJob.ScheduleParallel(this, Dependency);
        }
        
        /// <summary>
        /// Job для обработки технического обслуживания
        /// </summary>
        [BurstCompile]
        public partial struct VehicleMaintenanceJob : IJobEntity
        {
            public float DeltaTime;
            
            public void Execute(ref VehicleMaintenanceData maintenance, 
                              in VehiclePhysics physics, 
                              in AdvancedVehicleConfig config)
            {
                ProcessVehicleMaintenance(ref maintenance, physics, config);
            }
            
            /// <summary>
            /// Обрабатывает техническое обслуживание транспортного средства
            /// </summary>
            private void ProcessVehicleMaintenance(ref VehicleMaintenanceData maintenance,
                                                 in VehiclePhysics physics,
                                                 in AdvancedVehicleConfig config)
            {
                // Обновляем время
                maintenance.LastUpdateTime += DeltaTime;
                
                // Обновляем пробег
                maintenance.MileageSinceLastMaintenance += math.length(physics.Velocity) * DeltaTime;
                
                // Обновляем время работы двигателя
                maintenance.EngineHoursSinceLastMaintenance += DeltaTime;
                
                // Обрабатываем износ жидкостей
                ProcessFluidWear(ref maintenance, physics);
                
                // Обрабатываем износ шин
                ProcessTireWear(ref maintenance, physics);
                
                // Обрабатываем износ подвески
                ProcessSuspensionWear(ref maintenance, physics);
                
                // Обрабатываем износ тормозов
                ProcessBrakeWear(ref maintenance, physics);
                
                // Обрабатываем износ рулевого управления
                ProcessSteeringWear(ref maintenance, physics);
                
                // Проверяем необходимость обслуживания
                CheckMaintenanceNeeds(ref maintenance, config);
                
                // Устанавливаем флаг обновления
                maintenance.NeedsUpdate = true;
            }
            
            /// <summary>
            /// Обрабатывает износ жидкостей
            /// </summary>
            private void ProcessFluidWear(ref VehicleMaintenanceData maintenance, in VehiclePhysics physics)
            {
                // Износ масла
                float oilWear = physics.EngineRPM * 0.0001f * DeltaTime;
                maintenance.OilLevel -= oilWear;
                maintenance.OilQuality -= oilWear * 0.1f;
                
                // Износ охлаждающей жидкости
                float coolantWear = physics.EngineTemperature * 0.0001f * DeltaTime;
                maintenance.CoolantLevel -= coolantWear;
                maintenance.CoolantQuality -= coolantWear * 0.1f;
                
                // Износ тормозной жидкости
                float brakeFluidWear = math.abs(physics.BrakeTorque) * 0.0001f * DeltaTime;
                maintenance.BrakeFluidLevel -= brakeFluidWear;
                maintenance.BrakeFluidQuality -= brakeFluidWear * 0.1f;
                
                // Износ топлива
                float fuelWear = math.length(physics.Velocity) * 0.0001f * DeltaTime;
                maintenance.FuelLevel -= fuelWear;
                maintenance.FuelQuality -= fuelWear * 0.1f;
                
                // Ограничиваем значения
                maintenance.OilLevel = math.clamp(maintenance.OilLevel, 0f, 1f);
                maintenance.OilQuality = math.clamp(maintenance.OilQuality, 0f, 1f);
                maintenance.CoolantLevel = math.clamp(maintenance.CoolantLevel, 0f, 1f);
                maintenance.CoolantQuality = math.clamp(maintenance.CoolantQuality, 0f, 1f);
                maintenance.BrakeFluidLevel = math.clamp(maintenance.BrakeFluidLevel, 0f, 1f);
                maintenance.BrakeFluidQuality = math.clamp(maintenance.BrakeFluidQuality, 0f, 1f);
                maintenance.FuelLevel = math.clamp(maintenance.FuelLevel, 0f, 1f);
                maintenance.FuelQuality = math.clamp(maintenance.FuelQuality, 0f, 1f);
            }
            
            /// <summary>
            /// Обрабатывает износ шин
            /// </summary>
            private void ProcessTireWear(ref VehicleMaintenanceData maintenance, in VehiclePhysics physics)
            {
                // Износ протектора
                float treadWear = math.length(physics.Velocity) * 0.0001f * DeltaTime;
                maintenance.TireTreadWear += new float4(treadWear, treadWear, treadWear, treadWear);
                
                // Износ давления
                float pressureWear = math.abs(physics.Velocity.x) * 0.0001f * DeltaTime;
                maintenance.TirePressure -= new float4(pressureWear, pressureWear, pressureWear, pressureWear);
                
                // Износ температуры
                float temperatureWear = math.length(physics.Velocity) * 0.0001f * DeltaTime;
                maintenance.TireTemperature += new float4(temperatureWear, temperatureWear, temperatureWear, temperatureWear);
                
                // Износ балансировки
                float balanceWear = math.length(physics.AngularVelocity) * 0.0001f * DeltaTime;
                maintenance.WheelBalance -= new float4(balanceWear, balanceWear, balanceWear, balanceWear);
                
                // Износ выравнивания
                float alignmentWear = math.abs(physics.TurnSpeed) * 0.0001f * DeltaTime;
                maintenance.WheelAlignment -= new float4(alignmentWear, alignmentWear, alignmentWear, alignmentWear);
                
                // Ограничиваем значения
                maintenance.TireTreadWear = math.clamp(maintenance.TireTreadWear, 0f, 1f);
                maintenance.TirePressure = math.clamp(maintenance.TirePressure, 0f, 1000f);
                maintenance.TireTemperature = math.clamp(maintenance.TireTemperature, 0f, 200f);
                maintenance.WheelBalance = math.clamp(maintenance.WheelBalance, 0f, 1f);
                maintenance.WheelAlignment = math.clamp(maintenance.WheelAlignment, 0f, 1f);
            }
            
            /// <summary>
            /// Обрабатывает износ подвески
            /// </summary>
            private void ProcessSuspensionWear(ref VehicleMaintenanceData maintenance, in VehiclePhysics physics)
            {
                // Износ подвески
                float suspensionWear = math.length(physics.Velocity) * 0.0001f * DeltaTime;
                maintenance.SuspensionCondition -= suspensionWear;
                
                // Износ амортизаторов
                float shockWear = math.length(physics.Velocity) * 0.0001f * DeltaTime;
                maintenance.ShockAbsorberCondition -= new float4(shockWear, shockWear, shockWear, shockWear);
                
                // Износ пружин
                float springWear = math.length(physics.Velocity) * 0.0001f * DeltaTime;
                maintenance.SpringCondition -= new float4(springWear, springWear, springWear, springWear);
                
                // Износ стабилизаторов
                float stabilizerWear = math.abs(physics.TurnSpeed) * 0.0001f * DeltaTime;
                maintenance.StabilizerCondition -= new float4(stabilizerWear, stabilizerWear, stabilizerWear, stabilizerWear);
                
                // Ограничиваем значения
                maintenance.SuspensionCondition = math.clamp(maintenance.SuspensionCondition, 0f, 1f);
                maintenance.ShockAbsorberCondition = math.clamp(maintenance.ShockAbsorberCondition, 0f, 1f);
                maintenance.SpringCondition = math.clamp(maintenance.SpringCondition, 0f, 1f);
                maintenance.StabilizerCondition = math.clamp(maintenance.StabilizerCondition, 0f, 1f);
            }
            
            /// <summary>
            /// Обрабатывает износ тормозов
            /// </summary>
            private void ProcessBrakeWear(ref VehicleMaintenanceData maintenance, in VehiclePhysics physics)
            {
                // Износ тормозов
                float brakeWear = math.abs(physics.BrakeTorque) * 0.0001f * DeltaTime;
                maintenance.BrakeCondition -= new float4(brakeWear, brakeWear, brakeWear, brakeWear);
                
                // Износ тормозных дисков
                float discWear = math.abs(physics.BrakeTorque) * 0.0001f * DeltaTime;
                maintenance.BrakeDiscCondition -= new float4(discWear, discWear, discWear, discWear);
                
                // Износ тормозных колодок
                float padWear = math.abs(physics.BrakeTorque) * 0.0001f * DeltaTime;
                maintenance.BrakePadCondition -= new float4(padWear, padWear, padWear, padWear);
                
                // Ограничиваем значения
                maintenance.BrakeCondition = math.clamp(maintenance.BrakeCondition, 0f, 1f);
                maintenance.BrakeDiscCondition = math.clamp(maintenance.BrakeDiscCondition, 0f, 1f);
                maintenance.BrakePadCondition = math.clamp(maintenance.BrakePadCondition, 0f, 1f);
            }
            
            /// <summary>
            /// Обрабатывает износ рулевого управления
            /// </summary>
            private void ProcessSteeringWear(ref VehicleMaintenanceData maintenance, in VehiclePhysics physics)
            {
                // Износ рулевого управления
                float steeringWear = math.abs(physics.TurnSpeed) * 0.0001f * DeltaTime;
                maintenance.SteeringCondition -= steeringWear;
                
                // Износ рулевой рейки
                float rackWear = math.abs(physics.TurnSpeed) * 0.0001f * DeltaTime;
                maintenance.SteeringRackCondition -= rackWear;
                
                // Износ рулевых тяг
                float tieRodWear = math.abs(physics.TurnSpeed) * 0.0001f * DeltaTime;
                maintenance.SteeringTieRodCondition -= new float4(tieRodWear, tieRodWear, tieRodWear, tieRodWear);
                
                // Ограничиваем значения
                maintenance.SteeringCondition = math.clamp(maintenance.SteeringCondition, 0f, 1f);
                maintenance.SteeringRackCondition = math.clamp(maintenance.SteeringRackCondition, 0f, 1f);
                maintenance.SteeringTieRodCondition = math.clamp(maintenance.SteeringTieRodCondition, 0f, 1f);
            }
            
            /// <summary>
            /// Проверяет необходимость обслуживания
            /// </summary>
            private void CheckMaintenanceNeeds(ref VehicleMaintenanceData maintenance, in AdvancedVehicleConfig config)
            {
                // Проверяем пробег
                if (maintenance.MileageSinceLastMaintenance >= maintenance.MaxMileageBetweenMaintenance)
                {
                    // Требуется обслуживание по пробегу
                    maintenance.NeedsUpdate = true;
                }
                
                // Проверяем время работы двигателя
                if (maintenance.EngineHoursSinceLastMaintenance >= maintenance.MaxEngineHoursBetweenMaintenance)
                {
                    // Требуется обслуживание по времени работы двигателя
                    maintenance.NeedsUpdate = true;
                }
                
                // Проверяем уровень жидкостей
                if (maintenance.OilLevel < 0.2f || maintenance.CoolantLevel < 0.2f || 
                    maintenance.BrakeFluidLevel < 0.2f || maintenance.FuelLevel < 0.1f)
                {
                    // Требуется доливка жидкостей
                    maintenance.NeedsUpdate = true;
                }
                
                // Проверяем качество жидкостей
                if (maintenance.OilQuality < 0.3f || maintenance.CoolantQuality < 0.3f || 
                    maintenance.BrakeFluidQuality < 0.3f || maintenance.FuelQuality < 0.3f)
                {
                    // Требуется замена жидкостей
                    maintenance.NeedsUpdate = true;
                }
                
                // Проверяем состояние шин
                if (math.any(maintenance.TireTreadWear > 0.8f) || math.any(maintenance.TirePressure < 200f) || 
                    math.any(maintenance.TireTemperature > 150f) || math.any(maintenance.WheelBalance < 0.3f) || 
                    math.any(maintenance.WheelAlignment < 0.3f))
                {
                    // Требуется обслуживание шин
                    maintenance.NeedsUpdate = true;
                }
                
                // Проверяем состояние подвески
                if (maintenance.SuspensionCondition < 0.3f || math.any(maintenance.ShockAbsorberCondition < 0.3f) || 
                    math.any(maintenance.SpringCondition < 0.3f) || math.any(maintenance.StabilizerCondition < 0.3f))
                {
                    // Требуется обслуживание подвески
                    maintenance.NeedsUpdate = true;
                }
                
                // Проверяем состояние тормозов
                if (math.any(maintenance.BrakeCondition < 0.3f) || math.any(maintenance.BrakeDiscCondition < 0.3f) || 
                    math.any(maintenance.BrakePadCondition < 0.3f))
                {
                    // Требуется обслуживание тормозов
                    maintenance.NeedsUpdate = true;
                }
                
                // Проверяем состояние рулевого управления
                if (maintenance.SteeringCondition < 0.3f || maintenance.SteeringRackCondition < 0.3f || 
                    math.any(maintenance.SteeringTieRodCondition < 0.3f))
                {
                    // Требуется обслуживание рулевого управления
                    maintenance.NeedsUpdate = true;
                }
            }
        }
    }
}