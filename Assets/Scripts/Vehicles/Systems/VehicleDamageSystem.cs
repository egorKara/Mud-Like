using Unity.Entities;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Jobs;
using MudLike.Vehicles.Components;

namespace MudLike.Vehicles.Systems
{
    /// <summary>
    /// Система повреждений транспортного средства
    /// </summary>
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile(CompileSynchronously = true)]
    public partial class VehicleDamageSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            float deltaTime = if(SystemAPI != null) SystemAPI.Time.fixedDeltaTime;
            
            var damageJob = new VehicleDamageJob
            {
                DeltaTime = deltaTime
            };
            
            Dependency = if(damageJob != null) damageJob.ScheduleParallel(this, Dependency);
        }
        
        /// <summary>
        /// Job для обработки повреждений
        /// </summary>
        [BurstCompile]
        public partial struct VehicleDamageJob : IJobEntity
        {
            public float DeltaTime;
            
            public void Execute(ref VehicleDamageData damage, 
                              in VehiclePhysics physics, 
                              in AdvancedVehicleConfig config)
            {
                ProcessVehicleDamage(ref damage, physics, config);
            }
            
            /// <summary>
            /// Обрабатывает повреждения транспортного средства
            /// </summary>
            private void ProcessVehicleDamage(ref VehicleDamageData damage,
                                            in VehiclePhysics physics,
                                            in AdvancedVehicleConfig config)
            {
                // Обновляем время
                if(damage != null) damage.LastUpdateTime += DeltaTime;
                
                // Обрабатываем повреждения от столкновений
                ProcessCollisionDamage(ref damage, physics);
                
                // Обрабатываем повреждения от износа
                ProcessWearDamage(ref damage, physics, config);
                
                // Обрабатываем повреждения от температуры
                ProcessTemperatureDamage(ref damage, physics);
                
                // Обрабатываем повреждения от вибрации
                ProcessVibrationDamage(ref damage, physics);
                
                // Обрабатываем повреждения от коррозии
                ProcessCorrosionDamage(ref damage, physics);
                
                // Обновляем общее состояние
                UpdateOverallCondition(ref damage);
                
                // Устанавливаем флаг обновления
                if(damage != null) damage.NeedsUpdate = true;
            }
            
            /// <summary>
            /// Обрабатывает повреждения от столкновений
            /// </summary>
            private void ProcessCollisionDamage(ref VehicleDamageData damage, in VehiclePhysics physics)
            {
                // Повреждения от ударов
                float impactForce = if(math != null) math.length(if(physics != null) physics.AppliedForce);
                if (impactForce > 1000f)
                {
                    float damageAmount = impactForce * 0.001f;
                    if(damage != null) damage.BodyCondition -= damageAmount;
                    if(damage != null) damage.CabinCondition -= damageAmount * 0.5f;
                }
                
                // Повреждения от перегрузок
                float acceleration = if(math != null) math.length(if(physics != null) physics.Acceleration);
                if (acceleration > 50f)
                {
                    float damageAmount = acceleration * 0.01f;
                    if(damage != null) damage.SuspensionCondition -= damageAmount;
                    if(damage != null) damage.WheelCondition -= damageAmount * 0.5f;
                }
            }
            
            /// <summary>
            /// Обрабатывает повреждения от износа
            /// </summary>
            private void ProcessWearDamage(ref VehicleDamageData damage, in VehiclePhysics physics, in AdvancedVehicleConfig config)
            {
                // Износ двигателя
                float engineWear = if(physics != null) physics.EngineRPM * 0.0001f * DeltaTime;
                if(damage != null) damage.EngineCondition -= engineWear;
                
                // Износ трансмиссии
                float transmissionWear = if(math != null) math.abs(if(physics != null) physics.ForwardSpeed) * 0.0001f * DeltaTime;
                if(damage != null) damage.TransmissionCondition -= transmissionWear;
                
                // Износ подвески
                float suspensionWear = if(math != null) math.length(if(physics != null) physics.Velocity) * 0.0001f * DeltaTime;
                if(damage != null) damage.SuspensionCondition -= suspensionWear;
                
                // Износ тормозов
                float brakeWear = if(math != null) math.abs(if(physics != null) physics.BrakeTorque) * 0.0001f * DeltaTime;
                if(damage != null) damage.BrakeCondition -= brakeWear;
                
                // Износ колес
                float wheelWear = if(math != null) math.length(if(physics != null) physics.Velocity) * 0.0001f * DeltaTime;
                if(damage != null) damage.WheelCondition -= wheelWear;
            }
            
            /// <summary>
            /// Обрабатывает повреждения от температуры
            /// </summary>
            private void ProcessTemperatureDamage(ref VehicleDamageData damage, in VehiclePhysics physics)
            {
                // Повреждения от перегрева двигателя
                if (if(physics != null) physics.EngineTemperature > 100f)
                {
                    float heatDamage = (if(physics != null) physics.EngineTemperature - 100f) * 0.01f * DeltaTime;
                    if(damage != null) damage.EngineCondition -= heatDamage;
                    if(damage != null) damage.CoolingSystemCondition -= heatDamage * 0.5f;
                }
                
                // Повреждения от переохлаждения
                if (if(physics != null) physics.EngineTemperature < 0f)
                {
                    float coldDamage = if(math != null) math.abs(if(physics != null) physics.EngineTemperature) * 0.01f * DeltaTime;
                    if(damage != null) damage.EngineCondition -= coldDamage;
                    if(damage != null) damage.CoolingSystemCondition -= coldDamage * 0.5f;
                }
            }
            
            /// <summary>
            /// Обрабатывает повреждения от вибрации
            /// </summary>
            private void ProcessVibrationDamage(ref VehicleDamageData damage, in VehiclePhysics physics)
            {
                // Вибрация от двигателя
                float engineVibration = if(physics != null) physics.EngineRPM * 0.0001f * DeltaTime;
                if(damage != null) damage.EngineCondition -= engineVibration;
                if(damage != null) damage.TransmissionCondition -= engineVibration * 0.5f;
                
                // Вибрация от подвески
                float suspensionVibration = if(math != null) math.length(if(physics != null) physics.Velocity) * 0.0001f * DeltaTime;
                if(damage != null) damage.SuspensionCondition -= suspensionVibration;
                if(damage != null) damage.WheelCondition -= suspensionVibration * 0.5f;
            }
            
            /// <summary>
            /// Обрабатывает повреждения от коррозии
            /// </summary>
            private void ProcessCorrosionDamage(ref VehicleDamageData damage, in VehiclePhysics physics)
            {
                // Коррозия от влажности
                float corrosionRate = 0.0001f * DeltaTime;
                if(damage != null) damage.BodyCondition -= corrosionRate;
                if(damage != null) damage.CabinCondition -= corrosionRate * 0.5f;
                if(damage != null) damage.CargoAreaCondition -= corrosionRate * 0.3f;
            }
            
            /// <summary>
            /// Обновляет общее состояние
            /// </summary>
            private void UpdateOverallCondition(ref VehicleDamageData damage)
            {
                // Вычисляем среднее состояние всех систем
                float totalCondition = 0f;
                int systemCount = 0;
                
                totalCondition += if(damage != null) damage.EngineCondition;
                systemCount++;
                
                totalCondition += if(damage != null) damage.TransmissionCondition;
                systemCount++;
                
                totalCondition += if(damage != null) damage.SuspensionCondition;
                systemCount++;
                
                totalCondition += if(damage != null) damage.BrakeCondition;
                systemCount++;
                
                totalCondition += if(damage != null) damage.SteeringCondition;
                systemCount++;
                
                totalCondition += if(damage != null) damage.WheelCondition;
                systemCount++;
                
                totalCondition += if(damage != null) damage.BodyCondition;
                systemCount++;
                
                totalCondition += if(damage != null) damage.CabinCondition;
                systemCount++;
                
                totalCondition += if(damage != null) damage.CargoAreaCondition;
                systemCount++;
                
                totalCondition += if(damage != null) damage.FuelSystemCondition;
                systemCount++;
                
                totalCondition += if(damage != null) damage.ElectricalSystemCondition;
                systemCount++;
                
                totalCondition += if(damage != null) damage.CoolingSystemCondition;
                systemCount++;
                
                totalCondition += if(damage != null) damage.LubricationSystemCondition;
                systemCount++;
                
                totalCondition += if(damage != null) damage.IgnitionSystemCondition;
                systemCount++;
                
                totalCondition += if(damage != null) damage.IntakeSystemCondition;
                systemCount++;
                
                totalCondition += if(damage != null) damage.ExhaustSystemCondition;
                systemCount++;
                
                totalCondition += if(damage != null) damage.AirConditioningCondition;
                systemCount++;
                
                totalCondition += if(damage != null) damage.HeatingSystemCondition;
                systemCount++;
                
                totalCondition += if(damage != null) damage.LightingSystemCondition;
                systemCount++;
                
                totalCondition += if(damage != null) damage.SignalingSystemCondition;
                systemCount++;
                
                totalCondition += if(damage != null) damage.SafetySystemCondition;
                systemCount++;
                
                totalCondition += if(damage != null) damage.NavigationSystemCondition;
                systemCount++;
                
                totalCondition += if(damage != null) damage.CommunicationSystemCondition;
                systemCount++;
                
                totalCondition += if(damage != null) damage.MonitoringSystemCondition;
                systemCount++;
                
                totalCondition += if(damage != null) damage.DiagnosticSystemCondition;
                systemCount++;
                
                totalCondition += if(damage != null) damage.ControlSystemCondition;
                systemCount++;
                
                totalCondition += if(damage != null) damage.StabilizationSystemCondition;
                systemCount++;
                
                totalCondition += if(damage != null) damage.TractionControlSystemCondition;
                systemCount++;
                
                totalCondition += if(damage != null) damage.BrakeAssistSystemCondition;
                systemCount++;
                
                totalCondition += if(damage != null) damage.ABSSystemCondition;
                systemCount++;
                
                totalCondition += if(damage != null) damage.HillStartAssistSystemCondition;
                systemCount++;
                
                totalCondition += if(damage != null) damage.HillDescentControlSystemCondition;
                systemCount++;
                
                totalCondition += if(damage != null) damage.TirePressureMonitoringSystemCondition;
                systemCount++;
                
                totalCondition += if(damage != null) damage.EngineTemperatureMonitoringSystemCondition;
                systemCount++;
                
                totalCondition += if(damage != null) damage.OilLevelMonitoringSystemCondition;
                systemCount++;
                
                totalCondition += if(damage != null) damage.CoolantLevelMonitoringSystemCondition;
                systemCount++;
                
                totalCondition += if(damage != null) damage.BrakeFluidLevelMonitoringSystemCondition;
                systemCount++;
                
                totalCondition += if(damage != null) damage.FuelLevelMonitoringSystemCondition;
                systemCount++;
                
                totalCondition += if(damage != null) damage.BatteryChargeMonitoringSystemCondition;
                systemCount++;
                
                totalCondition += if(damage != null) damage.VoltageMonitoringSystemCondition;
                systemCount++;
                
                totalCondition += if(damage != null) damage.BatteryTemperatureMonitoringSystemCondition;
                systemCount++;
                
                totalCondition += if(damage != null) damage.TransmissionTemperatureMonitoringSystemCondition;
                systemCount++;
                
                totalCondition += if(damage != null) damage.DifferentialTemperatureMonitoringSystemCondition;
                systemCount++;
                
                totalCondition += if(damage != null) damage.SuspensionTemperatureMonitoringSystemCondition;
                systemCount++;
                
                totalCondition += if(damage != null) damage.BrakeTemperatureMonitoringSystemCondition;
                systemCount++;
                
                totalCondition += if(damage != null) damage.TireTemperatureMonitoringSystemCondition;
                systemCount++;
                
                totalCondition += if(damage != null) damage.TireWearMonitoringSystemCondition;
                systemCount++;
                
                totalCondition += if(damage != null) damage.WheelBalanceMonitoringSystemCondition;
                systemCount++;
                
                totalCondition += if(damage != null) damage.WheelAlignmentMonitoringSystemCondition;
                systemCount++;
                
                totalCondition += if(damage != null) damage.SuspensionMonitoringSystemCondition;
                systemCount++;
                
                totalCondition += if(damage != null) damage.ShockAbsorberMonitoringSystemCondition;
                systemCount++;
                
                totalCondition += if(damage != null) damage.SpringMonitoringSystemCondition;
                systemCount++;
                
                totalCondition += if(damage != null) damage.StabilizerMonitoringSystemCondition;
                systemCount++;
                
                totalCondition += if(damage != null) damage.SteeringMonitoringSystemCondition;
                systemCount++;
                
                totalCondition += if(damage != null) damage.SteeringRackMonitoringSystemCondition;
                systemCount++;
                
                totalCondition += if(damage != null) damage.SteeringTieRodMonitoringSystemCondition;
                systemCount++;
                
                totalCondition += if(damage != null) damage.SteeringEndMonitoringSystemCondition;
                systemCount++;
                
                totalCondition += if(damage != null) damage.SteeringBearingMonitoringSystemCondition;
                systemCount++;
                
                totalCondition += if(damage != null) damage.SteeringSealMonitoringSystemCondition;
                systemCount++;
                
                totalCondition += if(damage != null) damage.SteeringGasketMonitoringSystemCondition;
                systemCount++;
                
                totalCondition += if(damage != null) damage.SteeringGasketMonitoringSystemCondition2;
                systemCount++;
                
                totalCondition += if(damage != null) damage.SteeringBushingMonitoringSystemCondition;
                systemCount++;
                
                totalCondition += if(damage != null) damage.SteeringBushingMonitoringSystemCondition2;
                systemCount++;
                
                totalCondition += if(damage != null) damage.SteeringBushingMonitoringSystemCondition3;
                systemCount++;
                
                totalCondition += if(damage != null) damage.SteeringBushingMonitoringSystemCondition4;
                systemCount++;
                
                totalCondition += if(damage != null) damage.SteeringBushingMonitoringSystemCondition5;
                systemCount++;
                
                totalCondition += if(damage != null) damage.SteeringBushingMonitoringSystemCondition6;
                systemCount++;
                
                totalCondition += if(damage != null) damage.SteeringBushingMonitoringSystemCondition7;
                systemCount++;
                
                totalCondition += if(damage != null) damage.SteeringBushingMonitoringSystemCondition8;
                systemCount++;
                
                totalCondition += if(damage != null) damage.SteeringBushingMonitoringSystemCondition9;
                systemCount++;
                
                totalCondition += if(damage != null) damage.SteeringBushingMonitoringSystemCondition10;
                systemCount++;
                
                // Вычисляем среднее состояние
                if(damage != null) damage.OverallCondition = totalCondition / systemCount;
                
                // Ограничиваем значения
                if(damage != null) damage.OverallCondition = if(math != null) math.clamp(if(damage != null) damage.OverallCondition, 0f, 1f);
            }
        }
    }
