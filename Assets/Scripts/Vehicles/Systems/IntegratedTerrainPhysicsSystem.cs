using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;
using Unity.Collections;
using Unity.Burst;
using Unity.Jobs;
using MudLike.Vehicles.Components;
using MudLike.Terrain.Components;
using MudLike.Terrain.Systems;

namespace MudLike.Vehicles.Systems
{
    /// <summary>
    /// Интегрированная система физики транспорта с деформацией террейна
    /// Объединяет физику колес, деформацию террейна и взаимодействие с грязью
    /// </summary>
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile(CompileSynchronously = true)]
    public partial class IntegratedTerrainPhysicsSystem : SystemBase
    {
        private EntityQuery _vehicleQuery;
        private EntityQuery _wheelQuery;
        private EntityQuery _terrainQuery;
        
        protected override void OnCreate()
        {
            RequireForUpdate<PhysicsWorldSingleton>();
            
            _vehicleQuery = GetEntityQuery(
                ComponentType.ReadWrite<VehiclePhysics>(),
                ComponentType.ReadOnly<VehicleTag>(),
                ComponentType.ReadOnly<LocalTransform>()
            );
            
            _wheelQuery = GetEntityQuery(
                ComponentType.ReadWrite<WheelData>(),
                ComponentType.ReadWrite<WheelPhysicsData>(),
                ComponentType.ReadOnly<LocalTransform>()
            );
            
            _terrainQuery = GetEntityQuery(
                ComponentType.ReadWrite<DeformationData>(),
                ComponentType.ReadOnly<TerrainChunk>()
            );
        }
        
        protected override void OnUpdate()
        {
            float deltaTime = SystemAPI.Time.fixedDeltaTime;
            var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().PhysicsWorld;
            
            // Получаем систему MudManager для взаимодействия с грязью
            var mudManager = SystemAPI.GetSingleton<MudManagerSystem>();
            
            var integratedPhysicsJob = new IntegratedTerrainPhysicsJob
            {
                DeltaTime = deltaTime,
                PhysicsWorld = physicsWorld,
                MudManager = mudManager
            };
            
            Dependency = integratedPhysicsJob.ScheduleParallel(_vehicleQuery, Dependency);
        }
        
        /// <summary>
        /// Job для интегрированной физики террейна
        /// </summary>
        [BurstCompile]
        public partial struct IntegratedTerrainPhysicsJob : IJobEntity
        {
            public float DeltaTime;
            [ReadOnly] public PhysicsWorld PhysicsWorld;
            [ReadOnly] public MudManagerSystem MudManager;
            
            public void Execute(ref VehiclePhysics vehiclePhysics, 
                              in VehicleTag vehicleTag,
                              in LocalTransform vehicleTransform)
            {
                ProcessIntegratedTerrainPhysics(ref vehiclePhysics, vehicleTransform);
            }
            
            /// <summary>
            /// Обрабатывает интегрированную физику террейна для транспорта
            /// </summary>
            private void ProcessIntegratedTerrainPhysics(ref VehiclePhysics vehiclePhysics,
                                                       in LocalTransform vehicleTransform)
            {
                // Получаем все колеса транспорта
                var wheels = GetVehicleWheels(vehicleTransform);
                
                // Обрабатываем каждое колесо
                for (int i = 0; i < wheels.Length; i++)
                {
                    ProcessWheelTerrainInteraction(ref wheels[i], vehiclePhysics, vehicleTransform, DeltaTime);
                }
                
                // Обновляем общую физику транспорта на основе колес
                UpdateVehiclePhysicsFromWheels(ref vehiclePhysics, wheels, DeltaTime);
            }
            
            /// <summary>
            /// Обрабатывает взаимодействие колеса с террейном
            /// </summary>
            private void ProcessWheelTerrainInteraction(ref WheelTerrainData wheelData,
                                                      VehiclePhysics vehiclePhysics,
                                                      LocalTransform vehicleTransform,
                                                      float deltaTime)
            {
                // Вычисляем позицию колеса в мире
                float3 wheelWorldPosition = vehicleTransform.Position + wheelData.LocalPosition;
                
                // Используем MudManager API для получения данных о контакте
                var mudContact = MudManager.QueryContact(wheelWorldPosition, wheelData.Radius, vehiclePhysics.Mass);
                
                if (mudContact.IsValid)
                {
                    // Обновляем данные колеса на основе контакта с грязью
                    UpdateWheelFromMudContact(ref wheelData, mudContact, vehiclePhysics, deltaTime);
                    
                    // Применяем деформацию террейна
                    ApplyTerrainDeformation(wheelWorldPosition, wheelData, vehiclePhysics, deltaTime);
                    
                    // Обновляем физические параметры колеса
                    UpdateWheelPhysics(ref wheelData, mudContact, vehiclePhysics, deltaTime);
                }
                else
                {
                    // Колесо в воздухе
                    ResetWheelInAir(ref wheelData);
                }
            }
            
            /// <summary>
            /// Обновляет данные колеса на основе контакта с грязью
            /// </summary>
            private void UpdateWheelFromMudContact(ref WheelTerrainData wheelData,
                                                 MudContactData mudContact,
                                                 VehiclePhysics vehiclePhysics,
                                                 float deltaTime)
            {
                wheelData.IsGrounded = true;
                wheelData.SinkDepth = mudContact.SinkDepth;
                wheelData.TractionModifier = mudContact.TractionModifier;
                wheelData.MudLevel = mudContact.MudLevel;
                wheelData.SurfaceType = mudContact.SurfaceType;
                
                // Вычисляем сопротивление движению
                float speed = math.length(vehiclePhysics.Velocity);
                wheelData.Drag = mudContact.Drag * speed * speed;
                
                // Обновляем температуру колеса от трения
                float frictionHeat = wheelData.Drag * deltaTime * 0.001f;
                wheelData.Temperature += frictionHeat;
                wheelData.Temperature = math.clamp(wheelData.Temperature, 20f, 120f);
                
                // Накопление грязи на колесе
                if (mudContact.SurfaceType == SurfaceType.Mud || mudContact.SurfaceType == SurfaceType.Swamp)
                {
                    float mudAccumulation = mudContact.MudLevel * deltaTime * 0.5f;
                    wheelData.MudMass += mudAccumulation;
                    wheelData.MudMass = math.clamp(wheelData.MudMass, 0f, 5f);
                }
                else
                {
                    // Очистка от грязи на твердых поверхностях
                    float cleaning = deltaTime * 0.2f;
                    wheelData.MudMass -= cleaning;
                    wheelData.MudMass = math.max(wheelData.MudMass, 0f);
                }
            }
            
            /// <summary>
            /// Применяет деформацию террейна
            /// </summary>
            private void ApplyTerrainDeformation(float3 wheelPosition,
                                               WheelTerrainData wheelData,
                                               VehiclePhysics vehiclePhysics,
                                               float deltaTime)
            {
                // Вычисляем силу деформации
                float deformationForce = vehiclePhysics.Mass * 9.81f / 4f; // Распределяем массу на 4 колеса
                
                // Увеличиваем силу в зависимости от скорости и погружения
                deformationForce *= (1f + wheelData.SinkDepth * 2f);
                deformationForce *= (1f + math.length(vehiclePhysics.Velocity) * 0.1f);
                
                // Создаем данные деформации
                var deformationData = new DeformationData
                {
                    Position = wheelPosition,
                    Force = deformationForce,
                    Radius = wheelData.Radius * 1.5f, // Немного больше радиуса колеса
                    Depth = wheelData.SinkDepth,
                    SurfaceType = wheelData.SurfaceType,
                    Time = (float)SystemAPI.Time.ElapsedTime,
                    IsActive = true
                };
                
                // Применяем деформацию через систему террейна
                ApplyTerrainDeformationData(deformationData);
            }
            
            /// <summary>
            /// Обновляет физические параметры колеса
            /// </summary>
            private void UpdateWheelPhysics(ref WheelTerrainData wheelData,
                                          MudContactData mudContact,
                                          VehiclePhysics vehiclePhysics,
                                          float deltaTime)
            {
                // Вычисляем эффективное сцепление
                float baseTraction = GetSurfaceTraction(mudContact.SurfaceType);
                float traction = baseTraction * wheelData.TractionModifier;
                
                // Влияние грязи на сцепление
                traction *= (1f - wheelData.MudMass * 0.1f);
                
                // Влияние температуры на сцепление
                float temperatureFactor = math.clamp(1f - (wheelData.Temperature - 20f) / 100f, 0.5f, 1f);
                traction *= temperatureFactor;
                
                wheelData.EffectiveTraction = traction;
                
                // Вычисляем сопротивление качению
                float rollingResistance = GetSurfaceRollingResistance(mudContact.SurfaceType);
                rollingResistance *= (1f + wheelData.SinkDepth * 3f);
                rollingResistance *= (1f + wheelData.MudMass * 0.5f);
                
                wheelData.RollingResistance = rollingResistance;
                
                // Вычисляем вязкое сопротивление
                float viscosity = GetSurfaceViscosity(mudContact.SurfaceType);
                float velocity = math.length(vehiclePhysics.Velocity);
                wheelData.ViscousResistance = viscosity * velocity * wheelData.Radius;
            }
            
            /// <summary>
            /// Обновляет общую физику транспорта на основе колес
            /// </summary>
            private void UpdateVehiclePhysicsFromWheels(ref VehiclePhysics vehiclePhysics,
                                                      NativeArray<WheelTerrainData> wheels,
                                                      float deltaTime)
            {
                float totalTraction = 0f;
                float totalResistance = 0f;
                float totalDrag = 0f;
                
                // Суммируем воздействие всех колес
                for (int i = 0; i < wheels.Length; i++)
                {
                    var wheel = wheels[i];
                    
                    if (wheel.IsGrounded)
                    {
                        totalTraction += wheel.EffectiveTraction;
                        totalResistance += wheel.RollingResistance;
                        totalDrag += wheel.Drag;
                    }
                }
                
                // Усредняем значения
                float wheelCount = wheels.Length;
                vehiclePhysics.EffectiveTraction = totalTraction / wheelCount;
                vehiclePhysics.RollingResistance = totalResistance / wheelCount;
                vehiclePhysics.MudDrag = totalDrag / wheelCount;
                
                // Обновляем максимальную скорость на основе сцепления
                vehiclePhysics.MaxSpeed = vehiclePhysics.BaseMaxSpeed * vehiclePhysics.EffectiveTraction;
                
                // Обновляем ускорение
                vehiclePhysics.Acceleration = vehiclePhysics.BaseAcceleration * vehiclePhysics.EffectiveTraction;
                
                // Применяем сопротивление к скорости
                float resistanceForce = vehiclePhysics.RollingResistance + vehiclePhysics.MudDrag;
                float3 resistanceVector = -math.normalize(vehiclePhysics.Velocity) * resistanceForce;
                vehiclePhysics.Velocity += resistanceVector * deltaTime;
            }
            
            /// <summary>
            /// Сбрасывает данные колеса в воздухе
            /// </summary>
            private void ResetWheelInAir(ref WheelTerrainData wheelData)
            {
                wheelData.IsGrounded = false;
                wheelData.SinkDepth = 0f;
                wheelData.EffectiveTraction = 0f;
                wheelData.RollingResistance = 0f;
                wheelData.Drag = 0f;
                wheelData.ViscousResistance = 0f;
            }
            
            /// <summary>
            /// Получает колеса транспорта (заглушка - в реальности через EntityQuery)
            /// </summary>
            private NativeArray<WheelTerrainData> GetVehicleWheels(LocalTransform vehicleTransform)
            {
                // В реальной реализации здесь будет запрос к EntityManager
                // для получения всех колес данного транспорта
                var wheels = new NativeArray<WheelTerrainData>(4, Allocator.Temp);
                
                // Заглушка - создаем 4 колеса в стандартных позициях
                float wheelbase = 2.5f; // База транспорта
                float track = 1.8f;     // Колея
                
                wheels[0] = new WheelTerrainData // Переднее левое
                {
                    LocalPosition = new float3(-track/2f, -0.5f, wheelbase/2f),
                    Radius = 0.4f,
                    IsGrounded = false
                };
                
                wheels[1] = new WheelTerrainData // Переднее правое
                {
                    LocalPosition = new float3(track/2f, -0.5f, wheelbase/2f),
                    Radius = 0.4f,
                    IsGrounded = false
                };
                
                wheels[2] = new WheelTerrainData // Заднее левое
                {
                    LocalPosition = new float3(-track/2f, -0.5f, -wheelbase/2f),
                    Radius = 0.4f,
                    IsGrounded = false
                };
                
                wheels[3] = new WheelTerrainData // Заднее правое
                {
                    LocalPosition = new float3(track/2f, -0.5f, -wheelbase/2f),
                    Radius = 0.4f,
                    IsGrounded = false
                };
                
                return wheels;
            }
            
            /// <summary>
            /// Применяет данные деформации террейна (заглушка)
            /// </summary>
            private void ApplyTerrainDeformationData(DeformationData deformationData)
            {
                // В реальной реализации здесь будет интеграция с TerrainDeformationSystem
                // Пока что просто логируем
                UnityEngine.Debug.Log($"Applying deformation at {deformationData.Position} with force {deformationData.Force}");
            }
            
            // Вспомогательные методы для получения свойств поверхностей
            [BurstCompile]
            private float GetSurfaceTraction(SurfaceType surfaceType)
            {
                return surfaceType switch
                {
                    SurfaceType.Asphalt => 0.9f,
                    SurfaceType.Concrete => 0.85f,
                    SurfaceType.Dirt => 0.7f,
                    SurfaceType.Mud => 0.4f,
                    SurfaceType.Sand => 0.5f,
                    SurfaceType.Grass => 0.6f,
                    SurfaceType.Water => 0.2f,
                    SurfaceType.Ice => 0.15f,
                    SurfaceType.Snow => 0.3f,
                    SurfaceType.Rock => 0.8f,
                    SurfaceType.Gravel => 0.7f,
                    SurfaceType.Swamp => 0.25f,
                    _ => 0.5f
                };
            }
            
            [BurstCompile]
            private float GetSurfaceRollingResistance(SurfaceType surfaceType)
            {
                return surfaceType switch
                {
                    SurfaceType.Asphalt => 0.02f,
                    SurfaceType.Concrete => 0.025f,
                    SurfaceType.Dirt => 0.05f,
                    SurfaceType.Mud => 0.15f,
                    SurfaceType.Sand => 0.2f,
                    SurfaceType.Grass => 0.08f,
                    SurfaceType.Water => 0.3f,
                    SurfaceType.Ice => 0.05f,
                    SurfaceType.Snow => 0.1f,
                    SurfaceType.Rock => 0.03f,
                    SurfaceType.Gravel => 0.06f,
                    SurfaceType.Swamp => 0.25f,
                    _ => 0.05f
                };
            }
            
            [BurstCompile]
            private float GetSurfaceViscosity(SurfaceType surfaceType)
            {
                return surfaceType switch
                {
                    SurfaceType.Mud => 0.8f,
                    SurfaceType.Swamp => 0.9f,
                    SurfaceType.Water => 0.0f,
                    _ => 0.0f
                };
            }
        }
        
        /// <summary>
        /// Данные колеса для взаимодействия с террейном
        /// </summary>
        public struct WheelTerrainData
        {
            public float3 LocalPosition;
            public float Radius;
            public bool IsGrounded;
            public float SinkDepth;
            public float TractionModifier;
            public float MudLevel;
            public SurfaceType SurfaceType;
            public float Drag;
            public float Temperature;
            public float MudMass;
            public float EffectiveTraction;
            public float RollingResistance;
            public float ViscousResistance;
        }
    }
}
