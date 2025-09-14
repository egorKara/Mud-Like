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
            float deltaTime = SystemAPI.Time.fixedDeltaTime;
            
            var damageJob = new VehicleDamageJob
            {
                DeltaTime = deltaTime
            };
            
            Dependency = damageJob.ScheduleParallel(this, Dependency);
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
                damage.LastUpdateTime += DeltaTime;
                
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
                damage.NeedsUpdate = true;
            }
            
            /// <summary>
            /// Обрабатывает повреждения от столкновений
            /// </summary>
            private void ProcessCollisionDamage(ref VehicleDamageData damage, in VehiclePhysics physics)
            {
                // Повреждения от ударов
                float impactForce = math.length(physics.AppliedForce);
                if (impactForce > 1000f)
                {
                    float damageAmount = impactForce * 0.001f;
                    damage.BodyCondition -= damageAmount;
                    damage.CabinCondition -= damageAmount * 0.5f;
                }
                
                // Повреждения от перегрузок
                float acceleration = math.length(physics.Acceleration);
                if (acceleration > 50f)
                {
                    float damageAmount = acceleration * 0.01f;
                    damage.SuspensionCondition -= damageAmount;
                    damage.WheelCondition -= damageAmount * 0.5f;
                }
            }
            
            /// <summary>
            /// Обрабатывает повреждения от износа
            /// </summary>
            private void ProcessWearDamage(ref VehicleDamageData damage, in VehiclePhysics physics, in AdvancedVehicleConfig config)
            {
                // Износ двигателя
                float engineWear = physics.EngineRPM * 0.0001f * DeltaTime;
                damage.EngineCondition -= engineWear;
                
                // Износ трансмиссии
                float transmissionWear = math.abs(physics.ForwardSpeed) * 0.0001f * DeltaTime;
                damage.TransmissionCondition -= transmissionWear;
                
                // Износ подвески
                float suspensionWear = math.length(physics.Velocity) * 0.0001f * DeltaTime;
                damage.SuspensionCondition -= suspensionWear;
                
                // Износ тормозов
                float brakeWear = math.abs(physics.BrakeTorque) * 0.0001f * DeltaTime;
                damage.BrakeCondition -= brakeWear;
                
                // Износ колес
                float wheelWear = math.length(physics.Velocity) * 0.0001f * DeltaTime;
                damage.WheelCondition -= wheelWear;
            }
            
            /// <summary>
            /// Обрабатывает повреждения от температуры
            /// </summary>
            private void ProcessTemperatureDamage(ref VehicleDamageData damage, in VehiclePhysics physics)
            {
                // Повреждения от перегрева двигателя
                if (physics.EngineTemperature > 100f)
                {
                    float heatDamage = (physics.EngineTemperature - 100f) * 0.01f * DeltaTime;
                    damage.EngineCondition -= heatDamage;
                    damage.CoolingSystemCondition -= heatDamage * 0.5f;
                }
                
                // Повреждения от переохлаждения
                if (physics.EngineTemperature < 0f)
                {
                    float coldDamage = math.abs(physics.EngineTemperature) * 0.01f * DeltaTime;
                    damage.EngineCondition -= coldDamage;
                    damage.CoolingSystemCondition -= coldDamage * 0.5f;
                }
            }
            
            /// <summary>
            /// Обрабатывает повреждения от вибрации
            /// </summary>
            private void ProcessVibrationDamage(ref VehicleDamageData damage, in VehiclePhysics physics)
            {
                // Вибрация от двигателя
                float engineVibration = physics.EngineRPM * 0.0001f * DeltaTime;
                damage.EngineCondition -= engineVibration;
                damage.TransmissionCondition -= engineVibration * 0.5f;
                
                // Вибрация от подвески
                float suspensionVibration = math.length(physics.Velocity) * 0.0001f * DeltaTime;
                damage.SuspensionCondition -= suspensionVibration;
                damage.WheelCondition -= suspensionVibration * 0.5f;
            }
            
            /// <summary>
            /// Обрабатывает повреждения от коррозии
            /// </summary>
            private void ProcessCorrosionDamage(ref VehicleDamageData damage, in VehiclePhysics physics)
            {
                // Коррозия от влажности
                float corrosionRate = 0.0001f * DeltaTime;
                damage.BodyCondition -= corrosionRate;
                damage.CabinCondition -= corrosionRate * 0.5f;
                damage.CargoAreaCondition -= corrosionRate * 0.3f;
            }
            
            /// <summary>
            /// Обновляет общее состояние
            /// </summary>
            private void UpdateOverallCondition(ref VehicleDamageData damage)
            {
                // Вычисляем среднее состояние всех систем
                float totalCondition = 0f;
                int systemCount = 0;
                
                totalCondition += damage.EngineCondition;
                systemCount++;
                
                totalCondition += damage.TransmissionCondition;
                systemCount++;
                
                totalCondition += damage.SuspensionCondition;
                systemCount++;
                
                totalCondition += damage.BrakeCondition;
                systemCount++;
                
                totalCondition += damage.SteeringCondition;
                systemCount++;
                
                totalCondition += damage.WheelCondition;
                systemCount++;
                
                totalCondition += damage.BodyCondition;
                systemCount++;
                
                totalCondition += damage.CabinCondition;
                systemCount++;
                
                totalCondition += damage.CargoAreaCondition;
                systemCount++;
                
                totalCondition += damage.FuelSystemCondition;
                systemCount++;
                
                totalCondition += damage.ElectricalSystemCondition;
                systemCount++;
                
                totalCondition += damage.CoolingSystemCondition;
                systemCount++;
                
                totalCondition += damage.LubricationSystemCondition;
                systemCount++;
                
                totalCondition += damage.IgnitionSystemCondition;
                systemCount++;
                
                totalCondition += damage.IntakeSystemCondition;
                systemCount++;
                
                totalCondition += damage.ExhaustSystemCondition;
                systemCount++;
                
                totalCondition += damage.AirConditioningCondition;
                systemCount++;
                
                totalCondition += damage.HeatingSystemCondition;
                systemCount++;
                
                totalCondition += damage.LightingSystemCondition;
                systemCount++;
                
                totalCondition += damage.SignalingSystemCondition;
                systemCount++;
                
                totalCondition += damage.SafetySystemCondition;
                systemCount++;
                
                totalCondition += damage.NavigationSystemCondition;
                systemCount++;
                
                totalCondition += damage.CommunicationSystemCondition;
                systemCount++;
                
                totalCondition += damage.MonitoringSystemCondition;
                systemCount++;
                
                totalCondition += damage.DiagnosticSystemCondition;
                systemCount++;
                
                totalCondition += damage.ControlSystemCondition;
                systemCount++;
                
                totalCondition += damage.StabilizationSystemCondition;
                systemCount++;
                
                totalCondition += damage.TractionControlSystemCondition;
                systemCount++;
                
                totalCondition += damage.BrakeAssistSystemCondition;
                systemCount++;
                
                totalCondition += damage.ABSSystemCondition;
                systemCount++;
                
                totalCondition += damage.HillStartAssistSystemCondition;
                systemCount++;
                
                totalCondition += damage.HillDescentControlSystemCondition;
                systemCount++;
                
                totalCondition += damage.TirePressureMonitoringSystemCondition;
                systemCount++;
                
                totalCondition += damage.EngineTemperatureMonitoringSystemCondition;
                systemCount++;
                
                totalCondition += damage.OilLevelMonitoringSystemCondition;
                systemCount++;
                
                totalCondition += damage.CoolantLevelMonitoringSystemCondition;
                systemCount++;
                
                totalCondition += damage.BrakeFluidLevelMonitoringSystemCondition;
                systemCount++;
                
                totalCondition += damage.FuelLevelMonitoringSystemCondition;
                systemCount++;
                
                totalCondition += damage.BatteryChargeMonitoringSystemCondition;
                systemCount++;
                
                totalCondition += damage.VoltageMonitoringSystemCondition;
                systemCount++;
                
                totalCondition += damage.BatteryTemperatureMonitoringSystemCondition;
                systemCount++;
                
                totalCondition += damage.TransmissionTemperatureMonitoringSystemCondition;
                systemCount++;
                
                totalCondition += damage.DifferentialTemperatureMonitoringSystemCondition;
                systemCount++;
                
                totalCondition += damage.SuspensionTemperatureMonitoringSystemCondition;
                systemCount++;
                
                totalCondition += damage.BrakeTemperatureMonitoringSystemCondition;
                systemCount++;
                
                totalCondition += damage.TireTemperatureMonitoringSystemCondition;
                systemCount++;
                
                totalCondition += damage.TireWearMonitoringSystemCondition;
                systemCount++;
                
                totalCondition += damage.WheelBalanceMonitoringSystemCondition;
                systemCount++;
                
                totalCondition += damage.WheelAlignmentMonitoringSystemCondition;
                systemCount++;
                
                totalCondition += damage.SuspensionMonitoringSystemCondition;
                systemCount++;
                
                totalCondition += damage.ShockAbsorberMonitoringSystemCondition;
                systemCount++;
                
                totalCondition += damage.SpringMonitoringSystemCondition;
                systemCount++;
                
                totalCondition += damage.StabilizerMonitoringSystemCondition;
                systemCount++;
                
                totalCondition += damage.SteeringMonitoringSystemCondition;
                systemCount++;
                
                totalCondition += damage.SteeringRackMonitoringSystemCondition;
                systemCount++;
                
                totalCondition += damage.SteeringTieRodMonitoringSystemCondition;
                systemCount++;
                
                totalCondition += damage.SteeringEndMonitoringSystemCondition;
                systemCount++;
                
                totalCondition += damage.SteeringBearingMonitoringSystemCondition;
                systemCount++;
                
                totalCondition += damage.SteeringSealMonitoringSystemCondition;
                systemCount++;
                
                totalCondition += damage.SteeringGasketMonitoringSystemCondition;
                systemCount++;
                
                totalCondition += damage.SteeringGasketMonitoringSystemCondition2;
                systemCount++;
                
                totalCondition += damage.SteeringBushingMonitoringSystemCondition;
                systemCount++;
                
                totalCondition += damage.SteeringBushingMonitoringSystemCondition2;
                systemCount++;
                
                totalCondition += damage.SteeringBushingMonitoringSystemCondition3;
                systemCount++;
                
                totalCondition += damage.SteeringBushingMonitoringSystemCondition4;
                systemCount++;
                
                totalCondition += damage.SteeringBushingMonitoringSystemCondition5;
                systemCount++;
                
                totalCondition += damage.SteeringBushingMonitoringSystemCondition6;
                systemCount++;
                
                totalCondition += damage.SteeringBushingMonitoringSystemCondition7;
                systemCount++;
                
                totalCondition += damage.SteeringBushingMonitoringSystemCondition8;
                systemCount++;
                
                totalCondition += damage.SteeringBushingMonitoringSystemCondition9;
                systemCount++;
                
                totalCondition += damage.SteeringBushingMonitoringSystemCondition10;
                systemCount++;
                
                // Вычисляем среднее состояние
                damage.OverallCondition = totalCondition / systemCount;
                
                // Ограничиваем значения
                damage.OverallCondition = math.clamp(damage.OverallCondition, 0f, 1f);
            }
        }
    }
}