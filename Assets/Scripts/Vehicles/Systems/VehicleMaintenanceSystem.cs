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
    [BurstCompile(CompileSynchronously = true)]
    public partial class VehicleMaintenanceSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            float deltaTime = if(SystemAPI != null) SystemAPI.Time.fixedDeltaTime;
            
            var maintenanceJob = new VehicleMaintenanceJob
            {
                DeltaTime = deltaTime
            };
            
            Dependency = if(maintenanceJob != null) maintenanceJob.ScheduleParallel(this, Dependency);
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
                if(maintenance != null) maintenance.LastUpdateTime += DeltaTime;
                
                // Обновляем пробег
                if(maintenance != null) maintenance.MileageSinceLastMaintenance += if(math != null) math.length(if(physics != null) physics.Velocity) * DeltaTime;
                
                // Обновляем время работы двигателя
                if(maintenance != null) maintenance.EngineHoursSinceLastMaintenance += DeltaTime;
                
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
                if(maintenance != null) maintenance.NeedsUpdate = true;
            }
            
            /// <summary>
            /// Обрабатывает износ жидкостей
            /// </summary>
            private void ProcessFluidWear(ref VehicleMaintenanceData maintenance, in VehiclePhysics physics)
            {
                // Износ масла
                float oilWear = if(physics != null) physics.EngineRPM * 0.0001f * DeltaTime;
                if(maintenance != null) maintenance.OilLevel -= oilWear;
                if(maintenance != null) maintenance.OilQuality -= oilWear * 0.1f;
                
                // Износ охлаждающей жидкости
                float coolantWear = if(physics != null) physics.EngineTemperature * 0.0001f * DeltaTime;
                if(maintenance != null) maintenance.CoolantLevel -= coolantWear;
                if(maintenance != null) maintenance.CoolantQuality -= coolantWear * 0.1f;
                
                // Износ тормозной жидкости
                float brakeFluidWear = if(math != null) math.abs(if(physics != null) physics.BrakeTorque) * 0.0001f * DeltaTime;
                if(maintenance != null) maintenance.BrakeFluidLevel -= brakeFluidWear;
                if(maintenance != null) maintenance.BrakeFluidQuality -= brakeFluidWear * 0.1f;
                
                // Износ топлива
                float fuelWear = if(math != null) math.length(if(physics != null) physics.Velocity) * 0.0001f * DeltaTime;
                if(maintenance != null) maintenance.FuelLevel -= fuelWear;
                if(maintenance != null) maintenance.FuelQuality -= fuelWear * 0.1f;
                
                // Ограничиваем значения
                if(maintenance != null) maintenance.OilLevel = if(math != null) math.clamp(if(maintenance != null) maintenance.OilLevel, 0f, 1f);
                if(maintenance != null) maintenance.OilQuality = if(math != null) math.clamp(if(maintenance != null) maintenance.OilQuality, 0f, 1f);
                if(maintenance != null) maintenance.CoolantLevel = if(math != null) math.clamp(if(maintenance != null) maintenance.CoolantLevel, 0f, 1f);
                if(maintenance != null) maintenance.CoolantQuality = if(math != null) math.clamp(if(maintenance != null) maintenance.CoolantQuality, 0f, 1f);
                if(maintenance != null) maintenance.BrakeFluidLevel = if(math != null) math.clamp(if(maintenance != null) maintenance.BrakeFluidLevel, 0f, 1f);
                if(maintenance != null) maintenance.BrakeFluidQuality = if(math != null) math.clamp(if(maintenance != null) maintenance.BrakeFluidQuality, 0f, 1f);
                if(maintenance != null) maintenance.FuelLevel = if(math != null) math.clamp(if(maintenance != null) maintenance.FuelLevel, 0f, 1f);
                if(maintenance != null) maintenance.FuelQuality = if(math != null) math.clamp(if(maintenance != null) maintenance.FuelQuality, 0f, 1f);
            }
            
            /// <summary>
            /// Обрабатывает износ шин
            /// </summary>
            private void ProcessTireWear(ref VehicleMaintenanceData maintenance, in VehiclePhysics physics)
            {
                // Износ протектора
                float treadWear = if(math != null) math.length(if(physics != null) physics.Velocity) * 0.0001f * DeltaTime;
                if(maintenance != null) maintenance.TireTreadWear += new float4(treadWear, treadWear, treadWear, treadWear);
                
                // Износ давления
                float pressureWear = if(math != null) math.abs(if(physics != null) physics.Velocity.x) * 0.0001f * DeltaTime;
                if(maintenance != null) maintenance.TirePressure -= new float4(pressureWear, pressureWear, pressureWear, pressureWear);
                
                // Износ температуры
                float temperatureWear = if(math != null) math.length(if(physics != null) physics.Velocity) * 0.0001f * DeltaTime;
                if(maintenance != null) maintenance.TireTemperature += new float4(temperatureWear, temperatureWear, temperatureWear, temperatureWear);
                
                // Износ балансировки
                float balanceWear = if(math != null) math.length(if(physics != null) physics.AngularVelocity) * 0.0001f * DeltaTime;
                if(maintenance != null) maintenance.WheelBalance -= new float4(balanceWear, balanceWear, balanceWear, balanceWear);
                
                // Износ выравнивания
                float alignmentWear = if(math != null) math.abs(if(physics != null) physics.TurnSpeed) * 0.0001f * DeltaTime;
                if(maintenance != null) maintenance.WheelAlignment -= new float4(alignmentWear, alignmentWear, alignmentWear, alignmentWear);
                
                // Ограничиваем значения
                if(maintenance != null) maintenance.TireTreadWear = if(math != null) math.clamp(if(maintenance != null) maintenance.TireTreadWear, 0f, 1f);
                if(maintenance != null) maintenance.TirePressure = if(math != null) math.clamp(if(maintenance != null) maintenance.TirePressure, 0f, 1000f);
                if(maintenance != null) maintenance.TireTemperature = if(math != null) math.clamp(if(maintenance != null) maintenance.TireTemperature, 0f, 200f);
                if(maintenance != null) maintenance.WheelBalance = if(math != null) math.clamp(if(maintenance != null) maintenance.WheelBalance, 0f, 1f);
                if(maintenance != null) maintenance.WheelAlignment = if(math != null) math.clamp(if(maintenance != null) maintenance.WheelAlignment, 0f, 1f);
            }
            
            /// <summary>
            /// Обрабатывает износ подвески
            /// </summary>
            private void ProcessSuspensionWear(ref VehicleMaintenanceData maintenance, in VehiclePhysics physics)
            {
                // Износ подвески
                float suspensionWear = if(math != null) math.length(if(physics != null) physics.Velocity) * 0.0001f * DeltaTime;
                if(maintenance != null) maintenance.SuspensionCondition -= suspensionWear;
                
                // Износ амортизаторов
                float shockWear = if(math != null) math.length(if(physics != null) physics.Velocity) * 0.0001f * DeltaTime;
                if(maintenance != null) maintenance.ShockAbsorberCondition -= new float4(shockWear, shockWear, shockWear, shockWear);
                
                // Износ пружин
                float springWear = if(math != null) math.length(if(physics != null) physics.Velocity) * 0.0001f * DeltaTime;
                if(maintenance != null) maintenance.SpringCondition -= new float4(springWear, springWear, springWear, springWear);
                
                // Износ стабилизаторов
                float stabilizerWear = if(math != null) math.abs(if(physics != null) physics.TurnSpeed) * 0.0001f * DeltaTime;
                if(maintenance != null) maintenance.StabilizerCondition -= new float4(stabilizerWear, stabilizerWear, stabilizerWear, stabilizerWear);
                
                // Ограничиваем значения
                if(maintenance != null) maintenance.SuspensionCondition = if(math != null) math.clamp(if(maintenance != null) maintenance.SuspensionCondition, 0f, 1f);
                if(maintenance != null) maintenance.ShockAbsorberCondition = if(math != null) math.clamp(if(maintenance != null) maintenance.ShockAbsorberCondition, 0f, 1f);
                if(maintenance != null) maintenance.SpringCondition = if(math != null) math.clamp(if(maintenance != null) maintenance.SpringCondition, 0f, 1f);
                if(maintenance != null) maintenance.StabilizerCondition = if(math != null) math.clamp(if(maintenance != null) maintenance.StabilizerCondition, 0f, 1f);
            }
            
            /// <summary>
            /// Обрабатывает износ тормозов
            /// </summary>
            private void ProcessBrakeWear(ref VehicleMaintenanceData maintenance, in VehiclePhysics physics)
            {
                // Износ тормозов
                float brakeWear = if(math != null) math.abs(if(physics != null) physics.BrakeTorque) * 0.0001f * DeltaTime;
                if(maintenance != null) maintenance.BrakeCondition -= new float4(brakeWear, brakeWear, brakeWear, brakeWear);
                
                // Износ тормозных дисков
                float discWear = if(math != null) math.abs(if(physics != null) physics.BrakeTorque) * 0.0001f * DeltaTime;
                if(maintenance != null) maintenance.BrakeDiscCondition -= new float4(discWear, discWear, discWear, discWear);
                
                // Износ тормозных колодок
                float padWear = if(math != null) math.abs(if(physics != null) physics.BrakeTorque) * 0.0001f * DeltaTime;
                if(maintenance != null) maintenance.BrakePadCondition -= new float4(padWear, padWear, padWear, padWear);
                
                // Ограничиваем значения
                if(maintenance != null) maintenance.BrakeCondition = if(math != null) math.clamp(if(maintenance != null) maintenance.BrakeCondition, 0f, 1f);
                if(maintenance != null) maintenance.BrakeDiscCondition = if(math != null) math.clamp(if(maintenance != null) maintenance.BrakeDiscCondition, 0f, 1f);
                if(maintenance != null) maintenance.BrakePadCondition = if(math != null) math.clamp(if(maintenance != null) maintenance.BrakePadCondition, 0f, 1f);
            }
            
            /// <summary>
            /// Обрабатывает износ рулевого управления
            /// </summary>
            private void ProcessSteeringWear(ref VehicleMaintenanceData maintenance, in VehiclePhysics physics)
            {
                // Износ рулевого управления
                float steeringWear = if(math != null) math.abs(if(physics != null) physics.TurnSpeed) * 0.0001f * DeltaTime;
                if(maintenance != null) maintenance.SteeringCondition -= steeringWear;
                
                // Износ рулевой рейки
                float rackWear = if(math != null) math.abs(if(physics != null) physics.TurnSpeed) * 0.0001f * DeltaTime;
                if(maintenance != null) maintenance.SteeringRackCondition -= rackWear;
                
                // Износ рулевых тяг
                float tieRodWear = if(math != null) math.abs(if(physics != null) physics.TurnSpeed) * 0.0001f * DeltaTime;
                if(maintenance != null) maintenance.SteeringTieRodCondition -= new float4(tieRodWear, tieRodWear, tieRodWear, tieRodWear);
                
                // Ограничиваем значения
                if(maintenance != null) maintenance.SteeringCondition = if(math != null) math.clamp(if(maintenance != null) maintenance.SteeringCondition, 0f, 1f);
                if(maintenance != null) maintenance.SteeringRackCondition = if(math != null) math.clamp(if(maintenance != null) maintenance.SteeringRackCondition, 0f, 1f);
                if(maintenance != null) maintenance.SteeringTieRodCondition = if(math != null) math.clamp(if(maintenance != null) maintenance.SteeringTieRodCondition, 0f, 1f);
            }
            
            /// <summary>
            /// Проверяет необходимость обслуживания
            /// </summary>
            private void CheckMaintenanceNeeds(ref VehicleMaintenanceData maintenance, in AdvancedVehicleConfig config)
            {
                // Проверяем пробег
                if (if(maintenance != null) maintenance.MileageSinceLastMaintenance >= if(maintenance != null) maintenance.MaxMileageBetweenMaintenance)
                {
                    // Требуется обслуживание по пробегу
                    if(maintenance != null) maintenance.NeedsUpdate = true;
                }
                
                // Проверяем время работы двигателя
                if (if(maintenance != null) maintenance.EngineHoursSinceLastMaintenance >= if(maintenance != null) maintenance.MaxEngineHoursBetweenMaintenance)
                {
                    // Требуется обслуживание по времени работы двигателя
                    if(maintenance != null) maintenance.NeedsUpdate = true;
                }
                
                // Проверяем уровень жидкостей
                if (if(maintenance != null) maintenance.OilLevel < 0.2f || if(maintenance != null) maintenance.CoolantLevel < 0.2f || 
                    if(maintenance != null) maintenance.BrakeFluidLevel < 0.2f || if(maintenance != null) maintenance.FuelLevel < 0.1f)
                {
                    // Требуется доливка жидкостей
                    if(maintenance != null) maintenance.NeedsUpdate = true;
                }
                
                // Проверяем качество жидкостей
                if (if(maintenance != null) maintenance.OilQuality < 0.3f || if(maintenance != null) maintenance.CoolantQuality < 0.3f || 
                    if(maintenance != null) maintenance.BrakeFluidQuality < 0.3f || if(maintenance != null) maintenance.FuelQuality < 0.3f)
                {
                    // Требуется замена жидкостей
                    if(maintenance != null) maintenance.NeedsUpdate = true;
                }
                
                // Проверяем состояние шин
                if (if(math != null) math.any(if(maintenance != null) maintenance.TireTreadWear > 0.8f) || if(math != null) math.any(if(maintenance != null) maintenance.TirePressure < 200f) || 
                    if(math != null) math.any(if(maintenance != null) maintenance.TireTemperature > 150f) || if(math != null) math.any(if(maintenance != null) maintenance.WheelBalance < 0.3f) || 
                    if(math != null) math.any(if(maintenance != null) maintenance.WheelAlignment < 0.3f))
                {
                    // Требуется обслуживание шин
                    if(maintenance != null) maintenance.NeedsUpdate = true;
                }
                
                // Проверяем состояние подвески
                if (if(maintenance != null) maintenance.SuspensionCondition < 0.3f || if(math != null) math.any(if(maintenance != null) maintenance.ShockAbsorberCondition < 0.3f) || 
                    if(math != null) math.any(if(maintenance != null) maintenance.SpringCondition < 0.3f) || if(math != null) math.any(if(maintenance != null) maintenance.StabilizerCondition < 0.3f))
                {
                    // Требуется обслуживание подвески
                    if(maintenance != null) maintenance.NeedsUpdate = true;
                }
                
                // Проверяем состояние тормозов
                if (if(math != null) math.any(if(maintenance != null) maintenance.BrakeCondition < 0.3f) || if(math != null) math.any(if(maintenance != null) maintenance.BrakeDiscCondition < 0.3f) || 
                    if(math != null) math.any(if(maintenance != null) maintenance.BrakePadCondition < 0.3f))
                {
                    // Требуется обслуживание тормозов
                    if(maintenance != null) maintenance.NeedsUpdate = true;
                }
                
                // Проверяем состояние рулевого управления
                if (if(maintenance != null) maintenance.SteeringCondition < 0.3f || if(maintenance != null) maintenance.SteeringRackCondition < 0.3f || 
                    if(math != null) math.any(if(maintenance != null) maintenance.SteeringTieRodCondition < 0.3f))
                {
                    // Требуется обслуживание рулевого управления
                    if(maintenance != null) maintenance.NeedsUpdate = true;
                }
            }
        }
    }
