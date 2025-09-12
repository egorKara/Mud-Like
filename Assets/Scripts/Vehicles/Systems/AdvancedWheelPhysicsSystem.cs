using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;
using Unity.Collections;
using Unity.Burst;
using Unity.Jobs;
using MudLike.Vehicles.Components;
using MudLike.Terrain.Components;

namespace MudLike.Vehicles.Systems
{
    /// <summary>
    /// Продвинутая система физики колес с реалистичным взаимодействием с грязью
    /// </summary>
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile]
    public partial class AdvancedWheelPhysicsSystem : SystemBase
    {
        private EntityQuery _wheelQuery;
        private EntityQuery _surfaceQuery;
        
        protected override void OnCreate()
        {
            RequireForUpdate<PhysicsWorldSingleton>();
            
            _wheelQuery = GetEntityQuery(
                ComponentType.ReadWrite<WheelData>(),
                ComponentType.ReadWrite<WheelPhysicsData>(),
                ComponentType.ReadOnly<LocalTransform>(),
                ComponentType.ReadOnly<VehiclePhysics>()
            );
            
            _surfaceQuery = GetEntityQuery(
                ComponentType.ReadOnly<SurfaceData>()
            );
        }
        
        protected override void OnUpdate()
        {
            float deltaTime = SystemAPI.Time.fixedDeltaTime;
            var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().PhysicsWorld;
            
            var wheelPhysicsJob = new AdvancedWheelPhysicsJob
            {
                DeltaTime = deltaTime,
                PhysicsWorld = physicsWorld,
                SurfaceData = GetSurfaceData()
            };
            
            Dependency = wheelPhysicsJob.ScheduleParallel(_wheelQuery, Dependency);
        }
        
        /// <summary>
        /// Получает данные о поверхностях
        /// </summary>
        private NativeArray<SurfaceData> GetSurfaceData()
        {
            var surfaceData = new NativeArray<SurfaceData>(11, Allocator.TempJob);
            
            for (int i = 0; i < 11; i++)
            {
                surfaceData[i] = SurfaceProperties.GetSurfaceProperties((SurfaceType)i);
            }
            
            return surfaceData;
        }
        
        /// <summary>
        /// Job для продвинутой физики колес
        /// </summary>
        [BurstCompile]
        public partial struct AdvancedWheelPhysicsJob : IJobEntity
        {
            public float DeltaTime;
            [ReadOnly] public PhysicsWorld PhysicsWorld;
            [ReadOnly] public NativeArray<SurfaceData> SurfaceData;
            
            public void Execute(ref WheelData wheel, 
                              ref WheelPhysicsData wheelPhysics,
                              in LocalTransform wheelTransform,
                              in VehiclePhysics vehiclePhysics)
            {
                ProcessAdvancedWheelPhysics(ref wheel, ref wheelPhysics, wheelTransform, vehiclePhysics);
            }
            
            /// <summary>
            /// Обрабатывает продвинутую физику колеса
            /// </summary>
            private void ProcessAdvancedWheelPhysics(ref WheelData wheel,
                                                   ref WheelPhysicsData wheelPhysics,
                                                   in LocalTransform wheelTransform,
                                                   in VehiclePhysics vehiclePhysics)
            {
                // Выполняем raycast для определения поверхности
                float3 rayStart = wheelTransform.Position;
                float3 rayDirection = -math.up();
                float rayDistance = wheel.SuspensionLength + wheel.Radius;
                
                if (PhysicsWorld.CastRay(rayStart, rayDirection, rayDistance, out RaycastHit hit))
                {
                    wheel.IsGrounded = true;
                    wheel.GroundPoint = hit.Position;
                    wheel.GroundNormal = hit.SurfaceNormal;
                    wheel.GroundDistance = hit.Distance;
                    
                    // Определяем тип поверхности
                    SurfaceType surfaceType = DetermineSurfaceType(hit);
                    SurfaceData surface = SurfaceData[(int)surfaceType];
                    
                    // Обновляем данные физики колеса
                    UpdateWheelPhysicsData(ref wheelPhysics, surface, wheel, vehiclePhysics, DeltaTime);
                    
                    // Вычисляем силы взаимодействия с поверхностью
                    CalculateSurfaceForces(ref wheel, ref wheelPhysics, surface, vehiclePhysics, DeltaTime);
                    
                    // Обновляем температуру и износ колеса
                    UpdateWheelTemperatureAndWear(ref wheelPhysics, wheel, vehiclePhysics, DeltaTime);
                    
                    // Обрабатываем грязь на колесе
                    ProcessMudOnWheel(ref wheelPhysics, surface, wheel, DeltaTime);
                }
                else
                {
                    // Колесо не касается земли
                    wheel.IsGrounded = false;
                    ResetWheelPhysics(ref wheelPhysics);
                }
                
                wheelPhysics.LastUpdateTime = (float)SystemAPI.Time.ElapsedTime;
            }
            
            /// <summary>
            /// Определяет тип поверхности по hit данным
            /// </summary>
            private SurfaceType DetermineSurfaceType(RaycastHit hit)
            {
                // В реальной реализации здесь будет определение по материалам
                // Пока что используем случайный тип для демонстрации
                return (SurfaceType)((int)(SystemAPI.Time.ElapsedTime * 0.1f) % 11);
            }
            
            /// <summary>
            /// Обновляет данные физики колеса
            /// </summary>
            private void UpdateWheelPhysicsData(ref WheelPhysicsData wheelPhysics,
                                             SurfaceData surface,
                                             WheelData wheel,
                                             VehiclePhysics vehiclePhysics,
                                             float deltaTime)
            {
                // Вычисляем скорость проскальзывания
                float wheelSpeed = wheel.AngularVelocity * wheel.Radius;
                float vehicleSpeed = math.length(vehiclePhysics.Velocity);
                wheelPhysics.SlipRatio = math.abs(wheelSpeed - vehicleSpeed) / math.max(wheelSpeed, 0.1f);
                
                // Вычисляем угол проскальзывания
                float3 velocityDirection = math.normalize(vehiclePhysics.Velocity);
                float3 wheelDirection = math.forward(wheelTransform.Rotation);
                wheelPhysics.SlipAngle = math.acos(math.clamp(math.dot(velocityDirection, wheelDirection), -1f, 1f));
                
                // Обновляем сцепление с поверхностью
                wheelPhysics.SurfaceTraction = CalculateSurfaceTraction(surface, wheelPhysics, wheel);
                
                // Вычисляем глубину погружения
                wheelPhysics.SinkDepth = CalculateSinkDepth(surface, wheel, vehiclePhysics);
                
                // Обновляем сопротивление качению
                wheelPhysics.RollingResistance = CalculateRollingResistance(surface, wheel, vehiclePhysics);
                
                // Вычисляем вязкое сопротивление
                wheelPhysics.ViscousResistance = CalculateViscousResistance(surface, wheelPhysics, wheel);
                
                // Вычисляем выталкивающую силу
                wheelPhysics.BuoyancyForce = CalculateBuoyancyForce(surface, wheelPhysics, wheel);
                
                // Обновляем время контакта
                if (wheelPhysics.LastSurfaceType == (int)surface.SurfaceType)
                {
                    wheelPhysics.ContactTime += deltaTime;
                }
                else
                {
                    wheelPhysics.ContactTime = 0f;
                    wheelPhysics.LastSurfaceType = (int)surface.SurfaceType;
                }
            }
            
            /// <summary>
            /// Вычисляет сцепление с поверхностью
            /// </summary>
            private float CalculateSurfaceTraction(SurfaceData surface, WheelPhysicsData wheelPhysics, WheelData wheel)
            {
                float baseTraction = surface.TractionCoefficient;
                
                // Влияние температуры
                float temperatureFactor = math.clamp(wheelPhysics.WheelTemperature / 100f, 0.5f, 1.5f);
                
                // Влияние износа протектора
                float wearFactor = 1f - wheelPhysics.TreadWear * 0.5f;
                
                // Влияние давления в шине
                float pressureFactor = math.clamp(wheelPhysics.TirePressure / wheelPhysics.MaxTirePressure, 0.7f, 1.2f);
                
                // Влияние скорости проскальзывания
                float slipFactor = math.clamp(1f - wheelPhysics.SlipRatio, 0.1f, 1f);
                
                // Влияние влажности
                float moistureFactor = math.clamp(1f - surface.Moisture * 0.5f, 0.3f, 1f);
                
                return baseTraction * temperatureFactor * wearFactor * pressureFactor * slipFactor * moistureFactor;
            }
            
            /// <summary>
            /// Вычисляет глубину погружения в поверхность
            /// </summary>
            private float CalculateSinkDepth(SurfaceData surface, WheelData wheel, VehiclePhysics vehiclePhysics)
            {
                if (surface.PenetrationDepth <= 0f)
                    return 0f;
                
                // Давление колеса на поверхность
                float wheelPressure = vehiclePhysics.Mass * 9.81f / (math.PI * wheel.Radius * wheel.Radius);
                
                // Глубина погружения зависит от давления и плотности поверхности
                float sinkDepth = wheelPressure / (surface.Density * 9.81f) * surface.PenetrationDepth;
                
                return math.clamp(sinkDepth, 0f, surface.PenetrationDepth);
            }
            
            /// <summary>
            /// Вычисляет сопротивление качению
            /// </summary>
            private float CalculateRollingResistance(SurfaceData surface, WheelData wheel, VehiclePhysics vehiclePhysics)
            {
                float baseResistance = surface.RollingResistance;
                
                // Влияние скорости
                float speedFactor = 1f + math.length(vehiclePhysics.Velocity) * 0.01f;
                
                // Влияние глубины погружения
                float sinkFactor = 1f + wheelPhysics.SinkDepth * 2f;
                
                return baseResistance * speedFactor * sinkFactor;
            }
            
            /// <summary>
            /// Вычисляет вязкое сопротивление
            /// </summary>
            private float CalculateViscousResistance(SurfaceData surface, WheelPhysicsData wheelPhysics, WheelData wheel)
            {
                if (surface.Viscosity <= 0f)
                    return 0f;
                
                // Вязкое сопротивление пропорционально скорости и вязкости
                float velocity = math.length(wheelPhysics.SlipLinearVelocity);
                return surface.Viscosity * velocity * wheel.Radius;
            }
            
            /// <summary>
            /// Вычисляет выталкивающую силу
            /// </summary>
            private float CalculateBuoyancyForce(SurfaceData surface, WheelPhysicsData wheelPhysics, WheelData wheel)
            {
                if (surface.Density <= 0f || wheelPhysics.SinkDepth <= 0f)
                    return 0f;
                
                // Объем погруженной части колеса
                float submergedVolume = math.PI * wheel.Radius * wheel.Radius * wheelPhysics.SinkDepth;
                
                // Сила Архимеда
                return surface.Density * 9.81f * submergedVolume;
            }
            
            /// <summary>
            /// Вычисляет силы взаимодействия с поверхностью
            /// </summary>
            private void CalculateSurfaceForces(ref WheelData wheel,
                                             ref WheelPhysicsData wheelPhysics,
                                             SurfaceData surface,
                                             VehiclePhysics vehiclePhysics,
                                             float deltaTime)
            {
                // Вычисляем продольную силу сцепления
                float3 forwardDirection = math.forward(wheelTransform.Rotation);
                float3 velocityDirection = math.normalize(vehiclePhysics.Velocity);
                
                wheelPhysics.LongitudinalTractionForce = wheelPhysics.SurfaceTraction * 
                    math.dot(forwardDirection, velocityDirection) * vehiclePhysics.Mass * 9.81f;
                
                // Вычисляем боковую силу сцепления
                float3 rightDirection = math.right(wheelTransform.Rotation);
                wheelPhysics.LateralTractionForce = wheelPhysics.SurfaceTraction * 
                    math.dot(rightDirection, velocityDirection) * vehiclePhysics.Mass * 9.81f;
                
                // Общая сила сцепления
                wheelPhysics.CurrentTractionForce = math.sqrt(
                    wheelPhysics.LongitudinalTractionForce * wheelPhysics.LongitudinalTractionForce +
                    wheelPhysics.LateralTractionForce * wheelPhysics.LateralTractionForce
                );
                
                // Ограничиваем максимальную силу сцепления
                wheelPhysics.CurrentTractionForce = math.min(wheelPhysics.CurrentTractionForce, wheelPhysics.MaxTractionForce);
                
                // Вычисляем силу трения
                float3 frictionForce = -vehiclePhysics.Velocity * wheelPhysics.SurfaceTraction * 100f;
                
                // Добавляем вязкое сопротивление
                frictionForce += -vehiclePhysics.Velocity * wheelPhysics.ViscousResistance;
                
                // Ограничиваем силу трения
                float maxFriction = wheelPhysics.CurrentTractionForce;
                if (math.length(frictionForce) > maxFriction)
                {
                    frictionForce = math.normalize(frictionForce) * maxFriction;
                }
                
                wheel.FrictionForce = frictionForce;
                wheel.Traction = wheelPhysics.SurfaceTraction;
            }
            
            /// <summary>
            /// Обновляет температуру и износ колеса
            /// </summary>
            private void UpdateWheelTemperatureAndWear(ref WheelPhysicsData wheelPhysics,
                                                     WheelData wheel,
                                                     VehiclePhysics vehiclePhysics,
                                                     float deltaTime)
            {
                // Вычисляем энергию проскальзывания
                wheelPhysics.SlipEnergy = wheelPhysics.SlipRatio * math.length(vehiclePhysics.Velocity) * deltaTime;
                
                // Нагрев от проскальзывания
                float heating = wheelPhysics.SlipEnergy * 0.1f;
                wheelPhysics.WheelTemperature += heating * deltaTime;
                
                // Охлаждение
                float cooling = wheelPhysics.CoolingRate * (wheelPhysics.WheelTemperature - 20f) * deltaTime;
                wheelPhysics.WheelTemperature -= cooling * deltaTime;
                
                // Ограничиваем температуру
                wheelPhysics.WheelTemperature = math.clamp(wheelPhysics.WheelTemperature, 20f, 200f);
                
                // Износ протектора
                float wear = wheelPhysics.SlipEnergy * 0.001f;
                wheelPhysics.TreadWear += wear * deltaTime;
                wheelPhysics.TreadWear = math.clamp(wheelPhysics.TreadWear, 0f, 1f);
            }
            
            /// <summary>
            /// Обрабатывает грязь на колесе
            /// </summary>
            private void ProcessMudOnWheel(ref WheelPhysicsData wheelPhysics,
                                        SurfaceData surface,
                                        WheelData wheel,
                                        float deltaTime)
            {
                // Накопление грязи
                if (surface.SurfaceType == SurfaceType.Mud || surface.SurfaceType == SurfaceType.Swamp)
                {
                    float mudAccumulation = surface.Viscosity * wheelPhysics.SinkDepth * deltaTime;
                    wheelPhysics.MudMass += mudAccumulation;
                    wheelPhysics.MudParticleCount += (int)(mudAccumulation * 100f);
                }
                
                // Очистка от грязи
                float cleaning = wheelPhysics.CleaningRate * wheelPhysics.MudMass * deltaTime;
                wheelPhysics.MudMass -= cleaning;
                wheelPhysics.MudParticleCount = (int)(wheelPhysics.MudMass * 100f);
                
                // Ограничиваем количество грязи
                wheelPhysics.MudMass = math.clamp(wheelPhysics.MudMass, 0f, 10f);
                wheelPhysics.MudParticleCount = math.clamp(wheelPhysics.MudParticleCount, 0, 1000);
            }
            
            /// <summary>
            /// Сбрасывает данные физики колеса
            /// </summary>
            private void ResetWheelPhysics(ref WheelPhysicsData wheelPhysics)
            {
                wheelPhysics.SlipRatio = 0f;
                wheelPhysics.SlipAngle = 0f;
                wheelPhysics.SurfaceTraction = 0f;
                wheelPhysics.SinkDepth = 0f;
                wheelPhysics.RollingResistance = 0f;
                wheelPhysics.ViscousResistance = 0f;
                wheelPhysics.BuoyancyForce = 0f;
                wheelPhysics.CurrentTractionForce = 0f;
                wheelPhysics.LateralTractionForce = 0f;
                wheelPhysics.LongitudinalTractionForce = 0f;
                wheelPhysics.ContactTime = 0f;
            }
        }
    }
}