using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;
using NUnit.Framework;
using MudLike.Vehicles.Components;
using MudLike.Terrain.Components;
using MudLike.Weather.Components;
using MudLike.UI.Components;
using MudLike.Audio.Components;
using MudLike.Core.Components;

namespace MudLike.Tests.Infrastructure
{
    /// <summary>
    /// Расширенный тестовый фикстура для проекта Mud-Like
    /// </summary>
    public class MudLikeTestFixture : ECSTestFixture
    {
        /// <summary>
        /// Создает транспортное средство для тестирования
        /// </summary>
        protected Entity CreateVehicle(float3 position = default, quaternion rotation = default)
        {
            var entity = EntityManager.CreateEntity();
            
            // Основные компоненты
            EntityManager.AddComponentData(entity, new VehicleTag());
            EntityManager.AddComponentData(entity, new LocalTransform
            {
                Position = position,
                Rotation = rotation,
                Scale = 1f
            });
            
            // Конфигурация
            EntityManager.AddComponentData(entity, new VehicleConfig
            {
                MaxSpeed = 100f,
                Acceleration = 10f,
                TurnSpeed = 2f,
                Mass = 1500f,
                Drag = 0.3f,
                AngularDrag = 5f,
                TurnRadius = 5f,
                CenterOfMassHeight = 0.5f
            });
            
            // Физика
            EntityManager.AddComponentData(entity, new VehiclePhysics
            {
                Velocity = float3.zero,
                AngularVelocity = float3.zero,
                Acceleration = float3.zero,
                AngularAcceleration = float3.zero,
                AppliedForce = float3.zero,
                AppliedTorque = float3.zero,
                ForwardSpeed = 0f,
                TurnSpeed = 0f,
                CurrentGear = 1,
                EngineRPM = 800f,
                EnginePower = 0f,
                EngineTorque = 0f
            });
            
            // Ввод
            EntityManager.AddComponentData(entity, new VehicleInput
            {
                Vertical = 0f,
                Horizontal = 0f,
                Throttle = 0f,
                Brake = 0f,
                Steering = 0f,
                Handbrake = false,
                GearUp = false,
                GearDown = false,
                EngineToggle = false,
                ShiftUp = false,
                ShiftDown = false,
                Neutral = false,
                Reverse = false
            });
            
            // Двигатель
            EntityManager.AddComponentData(entity, new EngineData
            {
                CurrentRPM = 800f,
                MaxRPM = 6000f,
                IdleRPM = 800f,
                MinRPM = 0f,
                Torque = 300f,
                Power = 200f,
                MaxTorque = 300f,
                MaxPower = 200f,
                ThrottlePosition = 0f,
                GasPedal = 0f,
                TargetRPM = 800f,
                RPMSpeed = 5f,
                CurrentPower = 0f,
                CurrentTorque = 0f,
                FuelLevel = 1f,
                FuelConsumption = 0f,
                Temperature = 20f,
                MaxTemperature = 100f,
                IsRunning = false,
                IsStarting = false,
                IsStalled = false,
                RunningTime = 0f,
                TimeSinceStart = 0f,
                PowerCurve = new float2(0.8f, 1.2f),
                TorqueCurve = new float2(1.2f, 0.8f)
            });
            
            // Трансмиссия
            EntityManager.AddComponentData(entity, new TransmissionData
            {
                CurrentGear = 1,
                TargetGear = 1,
                MinGear = 1,
                MaxGear = 5,
                NeutralGear = 0,
                ReverseGear = -1,
                GearRatios = new float4(3.5f, 2.1f, 1.4f, 1.0f),
                FinalDriveRatio = 3.5f,
                Efficiency = 0.95f,
                OutputTorque = 0f,
                OutputPower = 0f,
                IsShifting = false,
                CurrentShiftTime = 0f,
                ShiftTime = 0.5f,
                UpshiftRPM = 5500f,
                DownshiftRPM = 2000f
            });
            
            return entity;
        }
        
        /// <summary>
        /// Создает колесо для тестирования
        /// </summary>
        protected Entity CreateWheel(float3 position = default, float radius = 0.5f)
        {
            var entity = EntityManager.CreateEntity();
            
            // Основные компоненты
            EntityManager.AddComponentData(entity, new LocalTransform
            {
                Position = position,
                Rotation = quaternion.identity,
                Scale = 1f
            });
            
            // Данные колеса
            EntityManager.AddComponentData(entity, new WheelData
            {
                LocalPosition = position,
                Radius = radius,
                Width = 0.2f,
                SuspensionLength = 0.5f,
                SpringForce = 50000f,
                DampingForce = 5000f,
                TargetPosition = 0f,
                CurrentPosition = 0f,
                SuspensionVelocity = 0f,
                IsGrounded = true,
                GroundPoint = position - new float3(0, 0.1f, 0),
                GroundNormal = new float3(0, 1, 0),
                GroundDistance = 0.1f,
                Traction = 1f,
                AngularVelocity = 0f,
                SteerAngle = 0f,
                MaxSteerAngle = 30f,
                MotorTorque = 0f,
                BrakeTorque = 0f,
                FrictionForce = float3.zero,
                SuspensionForce = float3.zero
            });
            
            // Расширенная физика колеса
            EntityManager.AddComponentData(entity, new WheelPhysicsData
            {
                SlipRatio = 0f,
                SlipAngle = 0f,
                SurfaceTraction = 1f,
                SinkDepth = 0f,
                RollingResistance = 0f,
                ViscousResistance = 0f,
                BuoyancyForce = 0f,
                SteeringResistance = 0f,
                WheelTemperature = 20f,
                TreadWear = 0f,
                TirePressure = 250f,
                MaxTirePressure = 300f,
                MinTirePressure = 200f,
                HeatingRate = 0.1f,
                CoolingRate = 0.05f,
                ContactTime = 0f,
                LastSurfaceType = 0,
                MudParticleCount = 0,
                MudMass = 0f,
                CleaningRate = 0.1f,
                CriticalSlipSpeed = 10f,
                MaxTractionForce = 1000f,
                CurrentTractionForce = 0f,
                LateralTractionForce = 0f,
                LongitudinalTractionForce = 0f,
                SlipAngularVelocity = 0f,
                SlipLinearVelocity = 0f,
                SlipDirection = float3.zero,
                SlipEnergy = 0f,
                LastUpdateTime = 0f,
                NeedsUpdate = true
            });
            
            return entity;
        }
        
        /// <summary>
        /// Создает поверхность для тестирования
        /// </summary>
        protected Entity CreateSurface(SurfaceType surfaceType, float3 position = default)
        {
            var entity = EntityManager.CreateEntity();
            
            // Основные компоненты
            EntityManager.AddComponentData(entity, new LocalTransform
            {
                Position = position,
                Rotation = quaternion.identity,
                Scale = 1f
            });
            
            // Данные поверхности
            var surfaceData = SurfaceProperties.GetSurfaceProperties(surfaceType);
            surfaceData.Position = position;
            surfaceData.IsActive = true;
            surfaceData.NeedsUpdate = true;
            EntityManager.AddComponentData(entity, surfaceData);
            
            return entity;
        }
        
        /// <summary>
        /// Создает погоду для тестирования
        /// </summary>
        protected Entity CreateWeather(WeatherType weatherType = WeatherType.Clear)
        {
            var entity = EntityManager.CreateEntity();
            
            // Данные погоды
            EntityManager.AddComponentData(entity, new WeatherData
            {
                Type = weatherType,
                Temperature = 20f,
                Humidity = 0.5f,
                WindSpeed = 5f,
                WindDirection = 0f,
                RainIntensity = 0f,
                SnowIntensity = 0f,
                SnowDepth = 0f,
                IceThickness = 0f,
                Visibility = 1000f,
                AtmosphericPressure = 101.3f,
                UVIndex = 5f,
                TimeOfDay = 12f,
                Season = Season.Summer,
                LastUpdateTime = 0f,
                NeedsUpdate = true
            });
            
            return entity;
        }
        
        /// <summary>
        /// Создает HUD для тестирования
        /// </summary>
        protected Entity CreateHUD()
        {
            var entity = EntityManager.CreateEntity();
            
            // Данные HUD
            EntityManager.AddComponentData(entity, new UIHUDData
            {
                Speed = 0f,
                RPM = 0f,
                CurrentGear = 0,
                Health = 1f,
                FuelLevel = 1f,
                EngineTemperature = 0.5f,
                WeatherInfo = new WeatherInfo
                {
                    Type = WeatherType.Clear,
                    Temperature = 20f,
                    Humidity = 0.5f
                },
                IsActive = true,
                NeedsUpdate = false
            });
            
            return entity;
        }
        
        /// <summary>
        /// Создает аудио для тестирования
        /// </summary>
        protected Entity CreateAudio()
        {
            var entity = EntityManager.CreateEntity();
            
            // Данные аудио
            EntityManager.AddComponentData(entity, new EngineAudioData
            {
                RPM = 0f,
                Power = 0f,
                Temperature = 0f,
                Volume = 0f,
                Pitch = 0f,
                IsPlaying = false,
                NeedsUpdate = false
            });
            
            return entity;
        }
        
        /// <summary>
        /// Создает лебедку для тестирования
        /// </summary>
        protected Entity CreateWinch(float3 position = default)
        {
            var entity = EntityManager.CreateEntity();
            
            // Основные компоненты
            EntityManager.AddComponentData(entity, new LocalTransform
            {
                Position = position,
                Rotation = quaternion.identity,
                Scale = 1f
            });
            
            // Данные лебедки
            EntityManager.AddComponentData(entity, new WinchData
            {
                MaxLength = 50f,
                CurrentLength = 0f,
                MaxForce = 10000f,
                CurrentForce = 0f,
                Speed = 5f,
                IsAttached = false,
                IsPulling = false,
                IsReleasing = false,
                IsBroken = false,
                IsOverloaded = false,
                IsStuck = false,
                Tension = 0f,
                Angle = 0f,
                Efficiency = 0.9f,
                Wear = 0f,
                Temperature = 20f,
                Lubrication = 1f,
                MaintenanceRequired = false,
                LastMaintenanceTime = 0f,
                NextMaintenanceTime = 1000f,
                IsActive = true,
                NeedsUpdate = false
            });
            
            return entity;
        }
        
        /// <summary>
        /// Создает груз для тестирования
        /// </summary>
        protected Entity CreateCargo(CargoType cargoType = CargoType.General, float3 position = default)
        {
            var entity = EntityManager.CreateEntity();
            
            // Основные компоненты
            EntityManager.AddComponentData(entity, new LocalTransform
            {
                Position = position,
                Rotation = quaternion.identity,
                Scale = 1f
            });
            
            // Данные груза
            EntityManager.AddComponentData(entity, new CargoData
            {
                Type = cargoType,
                Weight = 100f,
                Volume = 1f,
                Value = 1000f,
                Fragility = 0.5f,
                IsLoaded = false,
                IsDamaged = false,
                IsLost = false,
                IsDelivered = false,
                IsStolen = false,
                IsFound = false,
                IsHidden = false,
                IsDiscovered = false,
                IsRevealed = false,
                IsConcealed = false,
                IsExposed = false,
                IsProtected = false,
                IsSecured = false,
                IsReleased = false,
                IsFreed = false,
                IsCaptured = false,
                IsEscaped = false,
                IsRescued = false,
                IsAbandoned = false,
                LoadTime = 0f,
                UnloadTime = 0f,
                DeliveryTime = 0f,
                LastUpdateTime = 0f,
                NeedsUpdate = false
            });
            
            return entity;
        }
        
        /// <summary>
        /// Создает миссию для тестирования
        /// </summary>
        protected Entity CreateMission(MissionType missionType = MissionType.Delivery)
        {
            var entity = EntityManager.CreateEntity();
            
            // Данные миссии
            EntityManager.AddComponentData(entity, new MissionData
            {
                Type = missionType,
                Title = "Test Mission",
                Description = "Test mission description",
                Status = MissionStatus.NotStarted,
                Progress = 0f,
                MaxProgress = 100f,
                Reward = 1000f,
                Bonus = 500f,
                Penalty = 200f,
                TimeLimit = 3600f,
                StartTime = 0f,
                EndTime = 0f,
                IsActive = true,
                IsPaused = false,
                IsCompleted = false,
                IsFailed = false,
                IsAbandoned = false,
                IsRestarted = false,
                IsUpdated = false,
                ObjectiveCompleted = false,
                ObjectiveFailed = false,
                RewardEarned = false,
                BonusEarned = false,
                PenaltyApplied = false,
                LastUpdateTime = 0f,
                NeedsUpdate = false
            });
            
            return entity;
        }
        
        /// <summary>
        /// Проверяет, что транспортное средство движется в заданном направлении
        /// </summary>
        protected void AssertVehicleMoved(Entity vehicle, float3 expectedDirection, float minSpeed = 0.1f)
        {
            var physics = EntityManager.GetComponentData<VehiclePhysics>(vehicle);
            var transform = EntityManager.GetComponentData<LocalTransform>(vehicle);
            
            Assert.Greater(physics.Velocity.magnitude, minSpeed, 
                "Vehicle should be moving with speed greater than {0}", minSpeed);
            
            if (expectedDirection.magnitude > 0.1f)
            {
                var normalizedVelocity = math.normalize(physics.Velocity);
                var normalizedDirection = math.normalize(expectedDirection);
                var dotProduct = math.dot(normalizedVelocity, normalizedDirection);
                
                Assert.Greater(dotProduct, 0.5f, 
                    "Vehicle should be moving in expected direction. Expected: {0}, Actual: {1}", 
                    expectedDirection, physics.Velocity);
            }
        }
        
        /// <summary>
        /// Проверяет, что колесо касается земли
        /// </summary>
        protected void AssertWheelGrounded(Entity wheel, bool expectedGrounded = true)
        {
            var wheelData = EntityManager.GetComponentData<WheelData>(wheel);
            Assert.AreEqual(expectedGrounded, wheelData.IsGrounded, 
                "Wheel should {0} be grounded", expectedGrounded ? "" : "not");
        }
        
        /// <summary>
        /// Проверяет, что поверхность имеет правильный тип
        /// </summary>
        protected void AssertSurfaceType(Entity surface, SurfaceType expectedType)
        {
            var surfaceData = EntityManager.GetComponentData<SurfaceData>(surface);
            Assert.AreEqual(expectedType, surfaceData.SurfaceType, 
                "Surface should be of type {0}", expectedType);
        }
        
        /// <summary>
        /// Проверяет, что погода имеет правильный тип
        /// </summary>
        protected void AssertWeatherType(Entity weather, WeatherType expectedType)
        {
            var weatherData = EntityManager.GetComponentData<WeatherData>(weather);
            Assert.AreEqual(expectedType, weatherData.Type, 
                "Weather should be of type {0}", expectedType);
        }
        
        /// <summary>
        /// Проверяет, что HUD обновлен
        /// </summary>
        protected void AssertHUDUpdated(Entity hud, bool expectedUpdated = true)
        {
            var hudData = EntityManager.GetComponentData<UIHUDData>(hud);
            Assert.AreEqual(expectedUpdated, hudData.NeedsUpdate, 
                "HUD should {0} need update", expectedUpdated ? "" : "not");
        }
        
        /// <summary>
        /// Проверяет, что аудио воспроизводится
        /// </summary>
        protected void AssertAudioPlaying(Entity audio, bool expectedPlaying = true)
        {
            var audioData = EntityManager.GetComponentData<EngineAudioData>(audio);
            Assert.AreEqual(expectedPlaying, audioData.IsPlaying, 
                "Audio should {0} be playing", expectedPlaying ? "" : "not");
        }
        
        /// <summary>
        /// Проверяет, что лебедка прикреплена
        /// </summary>
        protected void AssertWinchAttached(Entity winch, bool expectedAttached = true)
        {
            var winchData = EntityManager.GetComponentData<WinchData>(winch);
            Assert.AreEqual(expectedAttached, winchData.IsAttached, 
                "Winch should {0} be attached", expectedAttached ? "" : "not");
        }
        
        /// <summary>
        /// Проверяет, что груз загружен
        /// </summary>
        protected void AssertCargoLoaded(Entity cargo, bool expectedLoaded = true)
        {
            var cargoData = EntityManager.GetComponentData<CargoData>(cargo);
            Assert.AreEqual(expectedLoaded, cargoData.IsLoaded, 
                "Cargo should {0} be loaded", expectedLoaded ? "" : "not");
        }
        
        /// <summary>
        /// Проверяет, что миссия активна
        /// </summary>
        protected void AssertMissionActive(Entity mission, bool expectedActive = true)
        {
            var missionData = EntityManager.GetComponentData<MissionData>(mission);
            Assert.AreEqual(expectedActive, missionData.IsActive, 
                "Mission should {0} be active", expectedActive ? "" : "not");
        }
        
        /// <summary>
        /// Запускает систему и проверяет, что она не выбрасывает исключений
        /// </summary>
        protected void AssertSystemRunsWithoutErrors<T>() where T : SystemBase
        {
            var system = World.CreateSystemManaged<T>();
            Assert.DoesNotThrow(() => system.Update(), 
                "System {0} should run without errors", typeof(T).Name);
        }
        
        /// <summary>
        /// Запускает систему и проверяет производительность
        /// </summary>
        protected void AssertSystemPerformance<T>(float maxExecutionTimeMs) where T : SystemBase
        {
            var system = World.CreateSystemManaged<T>();
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            
            system.Update();
            
            stopwatch.Stop();
            var executionTime = stopwatch.ElapsedMilliseconds;
            
            Assert.Less(executionTime, maxExecutionTimeMs, 
                "System {0} should execute in less than {1}ms, actual: {2}ms", 
                typeof(T).Name, maxExecutionTimeMs, executionTime);
        }
    }
}