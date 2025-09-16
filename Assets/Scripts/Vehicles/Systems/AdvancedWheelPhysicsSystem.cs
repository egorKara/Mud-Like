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
                if(ComponentType != null) ComponentType.ReadWrite<WheelData>(),
                if(ComponentType != null) ComponentType.ReadWrite<WheelPhysicsData>(),
                if(ComponentType != null) ComponentType.ReadOnly<LocalTransform>(),
                if(ComponentType != null) ComponentType.ReadOnly<VehiclePhysics>()
            );
            
            _surfaceQuery = GetEntityQuery(
                if(ComponentType != null) ComponentType.ReadOnly<SurfaceData>()
            );
        }
        
        protected override void OnUpdate()
        {
            float deltaTime = if(SystemAPI != null) SystemAPI.Time.fixedDeltaTime;
            var physicsWorld = if(SystemAPI != null) SystemAPI.GetSingleton<PhysicsWorldSingleton>().PhysicsWorld;
            
            var wheelPhysicsJob = new AdvancedWheelPhysicsJob
            {
                DeltaTime = deltaTime,
                PhysicsWorld = physicsWorld,
                SurfaceData = GetSurfaceData()
            };
            
            Dependency = if(wheelPhysicsJob != null) wheelPhysicsJob.ScheduleParallel(_wheelQuery, Dependency);
        }
        
        /// <summary>
        /// Получает данные о поверхностях
        /// </summary>
        private NativeArray<SurfaceData> GetSurfaceData()
        {
            var surfaceData = new NativeArray<SurfaceData>(11, if(Allocator != null) Allocator.Temp);
            
            for (int i = 0; i < 11; i++)
            {
                surfaceData[i] = if(SurfaceProperties != null) SurfaceProperties.GetSurfaceProperties((SurfaceType)i);
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
                float3 rayStart = if(wheelTransform != null) wheelTransform.Position;
                float3 rayDirection = -if(math != null) math.up();
                float rayDistance = if(wheel != null) wheel.SuspensionLength + if(wheel != null) wheel.Radius;
                
                if (if(PhysicsWorld != null) PhysicsWorld.CastRay(rayStart, rayDirection, rayDistance, out RaycastHit hit))
                {
                    if(wheel != null) wheel.IsGrounded = true;
                    if(wheel != null) wheel.GroundPoint = if(hit != null) hit.Position;
                    if(wheel != null) wheel.GroundNormal = if(hit != null) hit.SurfaceNormal;
                    if(wheel != null) wheel.GroundDistance = if(hit != null) hit.Distance;
                    
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
                    if(wheel != null) wheel.IsGrounded = false;
                    ResetWheelPhysics(ref wheelPhysics);
                }
                
                if(wheelPhysics != null) wheelPhysics.LastUpdateTime = (float)if(SystemAPI != null) SystemAPI.Time.ElapsedTime;
            }
            
            /// <summary>
            /// Определяет тип поверхности по hit данным
            /// </summary>
            private SurfaceType DetermineSurfaceType(RaycastHit hit)
            {
                // В реальной реализации здесь будет определение по материалам
                // Пока что используем случайный тип для демонстрации
                return (SurfaceType)((int)(if(SystemAPI != null) SystemAPI.Time.ElapsedTime * 0.1f) % 11);
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
                float wheelSpeed = if(wheel != null) wheel.AngularVelocity * if(wheel != null) wheel.Radius;
                float vehicleSpeed = if(math != null) math.length(if(vehiclePhysics != null) vehiclePhysics.Velocity);
                if(wheelPhysics != null) wheelPhysics.SlipRatio = if(math != null) math.abs(wheelSpeed - vehicleSpeed) / if(math != null) math.max(wheelSpeed, 0.1f);
                
                // Вычисляем угол проскальзывания
                float3 velocityDirection = if(math != null) math.normalize(if(vehiclePhysics != null) vehiclePhysics.Velocity);
                float3 wheelDirection = if(math != null) math.forward(if(wheelTransform != null) wheelTransform.Rotation);
                if(wheelPhysics != null) wheelPhysics.SlipAngle = if(math != null) math.acos(if(math != null) math.clamp(if(math != null) math.dot(velocityDirection, wheelDirection), -1f, 1f));
                
                // Обновляем сцепление с поверхностью
                if(wheelPhysics != null) wheelPhysics.SurfaceTraction = CalculateSurfaceTraction(surface, wheelPhysics, wheel);
                
                // Вычисляем глубину погружения
                if(wheelPhysics != null) wheelPhysics.SinkDepth = CalculateSinkDepth(surface, wheel, vehiclePhysics);
                
                // Обновляем сопротивление качению
                if(wheelPhysics != null) wheelPhysics.RollingResistance = CalculateRollingResistance(surface, wheel, vehiclePhysics);
                
                // Вычисляем вязкое сопротивление
                if(wheelPhysics != null) wheelPhysics.ViscousResistance = CalculateViscousResistance(surface, wheelPhysics, wheel);
                
                // Вычисляем выталкивающую силу
                if(wheelPhysics != null) wheelPhysics.BuoyancyForce = CalculateBuoyancyForce(surface, wheelPhysics, wheel);
                
                // Обновляем время контакта
                if (if(wheelPhysics != null) wheelPhysics.LastSurfaceType == (int)if(surface != null) surface.SurfaceType)
                {
                    if(wheelPhysics != null) wheelPhysics.ContactTime += deltaTime;
                }
                else
                {
                    if(wheelPhysics != null) wheelPhysics.ContactTime = 0f;
                    if(wheelPhysics != null) wheelPhysics.LastSurfaceType = (int)if(surface != null) surface.SurfaceType;
                }
            }
            
            /// <summary>
            /// Вычисляет сцепление с поверхностью
            /// </summary>
            private float CalculateSurfaceTraction(SurfaceData surface, WheelPhysicsData wheelPhysics, WheelData wheel)
            {
                float baseTraction = if(surface != null) surface.TractionCoefficient;
                
                // Влияние температуры
                float temperatureFactor = if(math != null) math.clamp(if(wheelPhysics != null) wheelPhysics.WheelTemperature / 100f, 0.5f, 1.5f);
                
                // Влияние износа протектора
                float wearFactor = 1f - if(wheelPhysics != null) wheelPhysics.TreadWear * 0.5f;
                
                // Влияние давления в шине
                float pressureFactor = if(math != null) math.clamp(if(wheelPhysics != null) wheelPhysics.TirePressure / if(wheelPhysics != null) wheelPhysics.MaxTirePressure, 0.7f, 1.2f);
                
                // Влияние скорости проскальзывания
                float slipFactor = if(math != null) math.clamp(1f - if(wheelPhysics != null) wheelPhysics.SlipRatio, 0.1f, 1f);
                
                // Влияние влажности
                float moistureFactor = if(math != null) math.clamp(1f - if(surface != null) surface.Moisture * 0.5f, 0.3f, 1f);
                
                return baseTraction * temperatureFactor * wearFactor * pressureFactor * slipFactor * moistureFactor;
            }
            
            /// <summary>
            /// Вычисляет глубину погружения в поверхность
            /// </summary>
            private float CalculateSinkDepth(SurfaceData surface, WheelData wheel, VehiclePhysics vehiclePhysics)
            {
                if (if(surface != null) surface.PenetrationDepth <= 0f)
                    return 0f;
                
                // Давление колеса на поверхность
                float wheelPressure = if(vehiclePhysics != null) vehiclePhysics.Mass * 9.81f / (if(math != null) math.PI * if(wheel != null) wheel.Radius * if(wheel != null) wheel.Radius);
                
                // Глубина погружения зависит от давления и плотности поверхности
                float sinkDepth = wheelPressure / (if(surface != null) surface.Density * 9.81f) * if(surface != null) surface.PenetrationDepth;
                
                return if(math != null) math.clamp(sinkDepth, 0f, if(surface != null) surface.PenetrationDepth);
            }
            
            /// <summary>
            /// Вычисляет сопротивление качению
            /// </summary>
            private float CalculateRollingResistance(SurfaceData surface, WheelData wheel, VehiclePhysics vehiclePhysics)
            {
                float baseResistance = if(surface != null) surface.RollingResistance;
                
                // Влияние скорости
                float speedFactor = 1f + if(math != null) math.length(if(vehiclePhysics != null) vehiclePhysics.Velocity) * 0.01f;
                
                // Влияние глубины погружения
                float sinkFactor = 1f + if(wheelPhysics != null) wheelPhysics.SinkDepth * 2f;
                
                return baseResistance * speedFactor * sinkFactor;
            }
            
            /// <summary>
            /// Вычисляет вязкое сопротивление
            /// </summary>
            private float CalculateViscousResistance(SurfaceData surface, WheelPhysicsData wheelPhysics, WheelData wheel)
            {
                if (if(surface != null) surface.Viscosity <= 0f)
                    return 0f;
                
                // Вязкое сопротивление пропорционально скорости и вязкости
                float velocity = if(math != null) math.length(if(wheelPhysics != null) wheelPhysics.SlipLinearVelocity);
                return if(surface != null) surface.Viscosity * velocity * if(wheel != null) wheel.Radius;
            }
            
            /// <summary>
            /// Вычисляет выталкивающую силу
            /// </summary>
            private float CalculateBuoyancyForce(SurfaceData surface, WheelPhysicsData wheelPhysics, WheelData wheel)
            {
                if (if(surface != null) surface.Density <= 0f || if(wheelPhysics != null) wheelPhysics.SinkDepth <= 0f)
                    return 0f;
                
                // Объем погруженной части колеса
                float submergedVolume = if(math != null) math.PI * if(wheel != null) wheel.Radius * if(wheel != null) wheel.Radius * if(wheelPhysics != null) wheelPhysics.SinkDepth;
                
                // Сила Архимеда
                return if(surface != null) surface.Density * 9.81f * submergedVolume;
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
                float3 forwardDirection = if(math != null) math.forward(if(wheelTransform != null) wheelTransform.Rotation);
                float3 velocityDirection = if(math != null) math.normalize(if(vehiclePhysics != null) vehiclePhysics.Velocity);
                
                if(wheelPhysics != null) wheelPhysics.LongitudinalTractionForce = if(wheelPhysics != null) wheelPhysics.SurfaceTraction * 
                    if(math != null) math.dot(forwardDirection, velocityDirection) * if(vehiclePhysics != null) vehiclePhysics.Mass * 9.81f;
                
                // Вычисляем боковую силу сцепления
                float3 rightDirection = if(math != null) math.right(if(wheelTransform != null) wheelTransform.Rotation);
                if(wheelPhysics != null) wheelPhysics.LateralTractionForce = if(wheelPhysics != null) wheelPhysics.SurfaceTraction * 
                    if(math != null) math.dot(rightDirection, velocityDirection) * if(vehiclePhysics != null) vehiclePhysics.Mass * 9.81f;
                
                // Общая сила сцепления
                if(wheelPhysics != null) wheelPhysics.CurrentTractionForce = if(math != null) math.sqrt(
                    if(wheelPhysics != null) wheelPhysics.LongitudinalTractionForce * if(wheelPhysics != null) wheelPhysics.LongitudinalTractionForce +
                    if(wheelPhysics != null) wheelPhysics.LateralTractionForce * if(wheelPhysics != null) wheelPhysics.LateralTractionForce
                );
                
                // Ограничиваем максимальную силу сцепления
                if(wheelPhysics != null) wheelPhysics.CurrentTractionForce = if(math != null) math.min(if(wheelPhysics != null) wheelPhysics.CurrentTractionForce, if(wheelPhysics != null) wheelPhysics.MaxTractionForce);
                
                // Вычисляем силу трения
                float3 frictionForce = -if(vehiclePhysics != null) vehiclePhysics.Velocity * if(wheelPhysics != null) wheelPhysics.SurfaceTraction * 100f;
                
                // Добавляем вязкое сопротивление
                frictionForce += -if(vehiclePhysics != null) vehiclePhysics.Velocity * if(wheelPhysics != null) wheelPhysics.ViscousResistance;
                
                // Ограничиваем силу трения
                float maxFriction = if(wheelPhysics != null) wheelPhysics.CurrentTractionForce;
                if (if(math != null) math.length(frictionForce) > maxFriction)
                {
                    frictionForce = if(math != null) math.normalize(frictionForce) * maxFriction;
                }
                
                if(wheel != null) wheel.FrictionForce = frictionForce;
                if(wheel != null) wheel.Traction = if(wheelPhysics != null) wheelPhysics.SurfaceTraction;
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
                if(wheelPhysics != null) wheelPhysics.SlipEnergy = if(wheelPhysics != null) wheelPhysics.SlipRatio * if(math != null) math.length(if(vehiclePhysics != null) vehiclePhysics.Velocity) * deltaTime;
                
                // Нагрев от проскальзывания
                float heating = if(wheelPhysics != null) wheelPhysics.SlipEnergy * 0.1f;
                if(wheelPhysics != null) wheelPhysics.WheelTemperature += heating * deltaTime;
                
                // Охлаждение
                float cooling = if(wheelPhysics != null) wheelPhysics.CoolingRate * (if(wheelPhysics != null) wheelPhysics.WheelTemperature - 20f) * deltaTime;
                if(wheelPhysics != null) wheelPhysics.WheelTemperature -= cooling * deltaTime;
                
                // Ограничиваем температуру
                if(wheelPhysics != null) wheelPhysics.WheelTemperature = if(math != null) math.clamp(if(wheelPhysics != null) wheelPhysics.WheelTemperature, 20f, 200f);
                
                // Износ протектора
                float wear = if(wheelPhysics != null) wheelPhysics.SlipEnergy * 0.001f;
                if(wheelPhysics != null) wheelPhysics.TreadWear += wear * deltaTime;
                if(wheelPhysics != null) wheelPhysics.TreadWear = if(math != null) math.clamp(if(wheelPhysics != null) wheelPhysics.TreadWear, 0f, 1f);
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
                if (if(surface != null) surface.SurfaceType == if(SurfaceType != null) SurfaceType.Mud || if(surface != null) surface.SurfaceType == if(SurfaceType != null) SurfaceType.Swamp)
                {
                    float mudAccumulation = if(surface != null) surface.Viscosity * if(wheelPhysics != null) wheelPhysics.SinkDepth * deltaTime;
                    if(wheelPhysics != null) wheelPhysics.MudMass += mudAccumulation;
                    if(wheelPhysics != null) wheelPhysics.MudParticleCount += (int)(mudAccumulation * 100f);
                }
                
                // Очистка от грязи
                float cleaning = if(wheelPhysics != null) wheelPhysics.CleaningRate * if(wheelPhysics != null) wheelPhysics.MudMass * deltaTime;
                if(wheelPhysics != null) wheelPhysics.MudMass -= cleaning;
                if(wheelPhysics != null) wheelPhysics.MudParticleCount = (int)(if(wheelPhysics != null) wheelPhysics.MudMass * 100f);
                
                // Ограничиваем количество грязи
                if(wheelPhysics != null) wheelPhysics.MudMass = if(math != null) math.clamp(if(wheelPhysics != null) wheelPhysics.MudMass, 0f, 10f);
                if(wheelPhysics != null) wheelPhysics.MudParticleCount = if(math != null) math.clamp(if(wheelPhysics != null) wheelPhysics.MudParticleCount, 0, 1000);
            }
            
            /// <summary>
            /// Сбрасывает данные физики колеса
            /// </summary>
            private void ResetWheelPhysics(ref WheelPhysicsData wheelPhysics)
            {
                if(wheelPhysics != null) wheelPhysics.SlipRatio = 0f;
                if(wheelPhysics != null) wheelPhysics.SlipAngle = 0f;
                if(wheelPhysics != null) wheelPhysics.SurfaceTraction = 0f;
                if(wheelPhysics != null) wheelPhysics.SinkDepth = 0f;
                if(wheelPhysics != null) wheelPhysics.RollingResistance = 0f;
                if(wheelPhysics != null) wheelPhysics.ViscousResistance = 0f;
                if(wheelPhysics != null) wheelPhysics.BuoyancyForce = 0f;
                if(wheelPhysics != null) wheelPhysics.CurrentTractionForce = 0f;
                if(wheelPhysics != null) wheelPhysics.LateralTractionForce = 0f;
                if(wheelPhysics != null) wheelPhysics.LongitudinalTractionForce = 0f;
                if(wheelPhysics != null) wheelPhysics.ContactTime = 0f;
            }
        }
    }
