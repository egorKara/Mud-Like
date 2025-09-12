using NUnit.Framework;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Profiling;
using Unity.Collections;
using MudLike.Vehicles.Components;
using MudLike.Terrain.Components;
using MudLike.Weather.Components;
using MudLike.UI.Components;
using MudLike.Audio.Components;
using MudLike.Networking.Components;
using MudLike.Tests.Infrastructure;

namespace MudLike.Tests.Performance
{
    /// <summary>
    /// Продвинутые тесты производительности для всех систем
    /// </summary>
    [TestFixture]
    public class AdvancedPerformanceTests : MudLikeTestFixture
    {
        private const int VEHICLE_COUNT = 2000;
        private const int WHEEL_COUNT = 8000; // 4 колеса на транспорт
        private const int SURFACE_COUNT = 500;
        private const int PARTICLE_COUNT = 50000;
        private const int PLAYER_COUNT = 100;
        
        [SetUp]
        public override void Setup()
        {
            base.Setup();
            
            // Создаем тестовые данные
            CreateTestVehicles();
            CreateTestWheels();
            CreateTestSurfaces();
            CreateTestWeather();
            CreateTestParticles();
            CreateTestPlayers();
        }
        
        [Test]
        public void PerformanceTest_VehicleMovementSystem_2000Vehicles()
        {
            // Arrange
            var system = World.CreateSystemManaged<VehicleMovementSystem>();
            
            // Act & Assert
            using (var profiler = new ProfilerMarker("VehicleMovementSystem_2000Vehicles"))
            {
                profiler.Begin();
                
                for (int i = 0; i < 100; i++)
                {
                    system.Update();
                }
                
                profiler.End();
            }
            
            Assert.DoesNotThrow(() => system.Update());
        }
        
        [Test]
        public void PerformanceTest_AdvancedWheelPhysicsSystem_8000Wheels()
        {
            // Arrange
            var system = World.CreateSystemManaged<AdvancedWheelPhysicsSystem>();
            
            // Act & Assert
            using (var profiler = new ProfilerMarker("AdvancedWheelPhysicsSystem_8000Wheels"))
            {
                profiler.Begin();
                
                for (int i = 0; i < 100; i++)
                {
                    system.Update();
                }
                
                profiler.End();
            }
            
            Assert.DoesNotThrow(() => system.Update());
        }
        
        [Test]
        public void PerformanceTest_TerrainDeformationSystem_500Surfaces()
        {
            // Arrange
            var system = World.CreateSystemManaged<TerrainDeformationSystem>();
            
            // Act & Assert
            using (var profiler = new ProfilerMarker("TerrainDeformationSystem_500Surfaces"))
            {
                profiler.Begin();
                
                for (int i = 0; i < 100; i++)
                {
                    system.Update();
                }
                
                profiler.End();
            }
            
            Assert.DoesNotThrow(() => system.Update());
        }
        
        [Test]
        public void PerformanceTest_MudParticlePoolSystem_50000Particles()
        {
            // Arrange
            var system = World.CreateSystemManaged<MudParticlePoolSystem>();
            
            // Act & Assert
            using (var profiler = new ProfilerMarker("MudParticlePoolSystem_50000Particles"))
            {
                profiler.Begin();
                
                for (int i = 0; i < 100; i++)
                {
                    system.Update();
                }
                
                profiler.End();
            }
            
            Assert.DoesNotThrow(() => system.Update());
        }
        
        [Test]
        public void PerformanceTest_InputValidationSystem_100Players()
        {
            // Arrange
            var system = World.CreateSystemManaged<InputValidationSystem>();
            
            // Act & Assert
            using (var profiler = new ProfilerMarker("InputValidationSystem_100Players"))
            {
                profiler.Begin();
                
                for (int i = 0; i < 100; i++)
                {
                    system.Update();
                }
                
                profiler.End();
            }
            
            Assert.DoesNotThrow(() => system.Update());
        }
        
        [Test]
        public void PerformanceTest_LagCompensationSystem_100Players()
        {
            // Arrange
            var system = World.CreateSystemManaged<LagCompensationSystem>();
            
            // Act & Assert
            using (var profiler = new ProfilerMarker("LagCompensationSystem_100Players"))
            {
                profiler.Begin();
                
                for (int i = 0; i < 100; i++)
                {
                    system.Update();
                }
                
                profiler.End();
            }
            
            Assert.DoesNotThrow(() => system.Update());
        }
        
        [Test]
        public void PerformanceTest_AllSystemsTogether_StressTest()
        {
            // Arrange - создаем все системы
            var vehicleSystem = World.CreateSystemManaged<VehicleMovementSystem>();
            var wheelSystem = World.CreateSystemManaged<AdvancedWheelPhysicsSystem>();
            var terrainSystem = World.CreateSystemManaged<TerrainDeformationSystem>();
            var particleSystem = World.CreateSystemManaged<MudParticlePoolSystem>();
            var inputSystem = World.CreateSystemManaged<InputValidationSystem>();
            var lagSystem = World.CreateSystemManaged<LagCompensationSystem>();
            var hudSystem = World.CreateSystemManaged<UIHUDSystem>();
            var audioSystem = World.CreateSystemManaged<EngineAudioSystem>();
            
            // Act & Assert
            using (var profiler = new ProfilerMarker("AllSystemsTogether_StressTest"))
            {
                profiler.Begin();
                
                for (int i = 0; i < 10; i++)
                {
                    vehicleSystem.Update();
                    wheelSystem.Update();
                    terrainSystem.Update();
                    particleSystem.Update();
                    inputSystem.Update();
                    lagSystem.Update();
                    hudSystem.Update();
                    audioSystem.Update();
                }
                
                profiler.End();
            }
            
            Assert.DoesNotThrow(() => World.Update());
        }
        
        [Test]
        public void PerformanceTest_MemoryUsage_StressTest()
        {
            // Arrange
            var initialMemory = UnityEngine.Profiling.Profiler.GetTotalAllocatedMemory(false);
            
            // Act - создаем много сущностей
            for (int i = 0; i < 50000; i++)
            {
                var entity = EntityManager.CreateEntity();
                EntityManager.AddComponentData(entity, new VehiclePhysics
                {
                    Velocity = new float3(UnityEngine.Random.Range(-10f, 10f), 0, UnityEngine.Random.Range(-10f, 10f)),
                    AngularVelocity = new float3(0, UnityEngine.Random.Range(-5f, 5f), 0),
                    EngineRPM = UnityEngine.Random.Range(800f, 6000f),
                    CurrentGear = UnityEngine.Random.Range(1, 6)
                });
                EntityManager.AddComponentData(entity, new LocalTransform
                {
                    Position = new float3(UnityEngine.Random.Range(-1000f, 1000f), 0, UnityEngine.Random.Range(-1000f, 1000f)),
                    Rotation = quaternion.identity
                });
            }
            
            var peakMemory = UnityEngine.Profiling.Profiler.GetTotalAllocatedMemory(false);
            var memoryIncrease = peakMemory - initialMemory;
            
            // Assert - проверяем, что память не растет слишком быстро
            Assert.Less(memoryIncrease, 200 * 1024 * 1024); // Менее 200MB
        }
        
        [Test]
        public void PerformanceTest_EntityQueryPerformance_StressTest()
        {
            // Arrange
            var query = EntityManager.CreateEntityQuery(typeof(VehiclePhysics));
            
            // Act & Assert
            using (var profiler = new ProfilerMarker("EntityQueryPerformance_StressTest"))
            {
                profiler.Begin();
                
                for (int i = 0; i < 10000; i++)
                {
                    var entities = query.ToEntityArray(Allocator.Temp);
                    entities.Dispose();
                }
                
                profiler.End();
            }
            
            Assert.DoesNotThrow(() => query.ToEntityArray(Allocator.Temp).Dispose());
        }
        
        [Test]
        public void PerformanceTest_JobSystemPerformance_StressTest()
        {
            // Arrange
            var system = World.CreateSystemManaged<AdvancedWheelPhysicsSystem>();
            
            // Act & Assert
            using (var profiler = new ProfilerMarker("JobSystemPerformance_StressTest"))
            {
                profiler.Begin();
                
                // Запускаем систему много раз для тестирования Job System
                for (int i = 0; i < 5000; i++)
                {
                    system.Update();
                }
                
                profiler.End();
            }
            
            Assert.DoesNotThrow(() => system.Update());
        }
        
        [Test]
        public void PerformanceTest_BurstCompilation_StressTest()
        {
            // Arrange
            var system = World.CreateSystemManaged<VehicleMovementSystem>();
            
            // Act & Assert
            using (var profiler = new ProfilerMarker("BurstCompilation_StressTest"))
            {
                profiler.Begin();
                
                // Тестируем Burst компиляцию
                for (int i = 0; i < 10000; i++)
                {
                    system.Update();
                }
                
                profiler.End();
            }
            
            Assert.DoesNotThrow(() => system.Update());
        }
        
        [Test]
        public void PerformanceTest_NetworkSynchronization_StressTest()
        {
            // Arrange
            var system = World.CreateSystemManaged<LagCompensationSystem>();
            
            // Act & Assert
            using (var profiler = new ProfilerMarker("NetworkSynchronization_StressTest"))
            {
                profiler.Begin();
                
                // Тестируем сетевую синхронизацию
                for (int i = 0; i < 1000; i++)
                {
                    system.Update();
                }
                
                profiler.End();
            }
            
            Assert.DoesNotThrow(() => system.Update());
        }
        
        [Test]
        public void PerformanceTest_FrameRate_60FPS_Requirement()
        {
            // Arrange
            var system = World.CreateSystemManaged<VehicleMovementSystem>();
            var startTime = UnityEngine.Time.realtimeSinceStartup;
            
            // Act
            for (int i = 0; i < 60; i++)
            {
                system.Update();
            }
            
            var endTime = UnityEngine.Time.realtimeSinceStartup;
            var frameTime = (endTime - startTime) / 60f;
            var fps = 1f / frameTime;
            
            // Assert
            Assert.GreaterOrEqual(fps, 60f, $"FPS: {fps}, Frame Time: {frameTime * 1000f}ms");
        }
        
        private void CreateTestVehicles()
        {
            for (int i = 0; i < VEHICLE_COUNT; i++)
            {
                var entity = EntityManager.CreateEntity();
                EntityManager.AddComponentData(entity, new VehicleTag());
                EntityManager.AddComponentData(entity, new VehicleConfig
                {
                    MaxSpeed = 100f,
                    Acceleration = 10f,
                    Mass = 1500f,
                    Drag = 0.3f,
                    AngularDrag = 5f
                });
                EntityManager.AddComponentData(entity, new VehiclePhysics
                {
                    Velocity = new float3(UnityEngine.Random.Range(-10f, 10f), 0, UnityEngine.Random.Range(-10f, 10f)),
                    AngularVelocity = new float3(0, UnityEngine.Random.Range(-5f, 5f), 0),
                    EngineRPM = UnityEngine.Random.Range(800f, 6000f),
                    CurrentGear = UnityEngine.Random.Range(1, 6)
                });
                EntityManager.AddComponentData(entity, new VehicleInput
                {
                    Vertical = UnityEngine.Random.Range(-1f, 1f),
                    Horizontal = UnityEngine.Random.Range(-1f, 1f),
                    Brake = UnityEngine.Random.value > 0.8f,
                    Handbrake = UnityEngine.Random.value > 0.9f
                });
                EntityManager.AddComponentData(entity, new LocalTransform
                {
                    Position = new float3(UnityEngine.Random.Range(-1000f, 1000f), 0, UnityEngine.Random.Range(-1000f, 1000f)),
                    Rotation = quaternion.identity
                });
            }
        }
        
        private void CreateTestWheels()
        {
            for (int i = 0; i < WHEEL_COUNT; i++)
            {
                var entity = EntityManager.CreateEntity();
                EntityManager.AddComponentData(entity, new WheelData
                {
                    Radius = 0.5f,
                    Width = 0.2f,
                    IsGrounded = UnityEngine.Random.value > 0.1f,
                    SuspensionForce = new float3(0, UnityEngine.Random.Range(1000f, 5000f), 0),
                    Traction = UnityEngine.Random.Range(0.5f, 1.0f),
                    AngularVelocity = UnityEngine.Random.Range(-50f, 50f)
                });
                EntityManager.AddComponentData(entity, new WheelPhysicsData
                {
                    SlipSpeed = UnityEngine.Random.Range(0f, 10f),
                    Temperature = UnityEngine.Random.Range(20f, 80f),
                    TirePressure = UnityEngine.Random.Range(2.0f, 3.0f)
                });
                EntityManager.AddComponentData(entity, new LocalTransform
                {
                    Position = new float3(UnityEngine.Random.Range(-1000f, 1000f), 0, UnityEngine.Random.Range(-1000f, 1000f)),
                    Rotation = quaternion.identity
                });
            }
        }
        
        private void CreateTestSurfaces()
        {
            for (int i = 0; i < SURFACE_COUNT; i++)
            {
                var entity = EntityManager.CreateEntity();
                EntityManager.AddComponentData(entity, new SurfaceData
                {
                    SurfaceType = (SurfaceType)(i % 12),
                    FrictionCoefficient = UnityEngine.Random.Range(0.1f, 1.0f),
                    TractionCoefficient = UnityEngine.Random.Range(0.2f, 1.0f),
                    RollingResistance = UnityEngine.Random.Range(0.01f, 0.3f),
                    Viscosity = UnityEngine.Random.Range(0f, 1f),
                    Density = UnityEngine.Random.Range(1000f, 3000f),
                    PenetrationDepth = UnityEngine.Random.Range(0f, 0.5f),
                    Temperature = UnityEngine.Random.Range(-10f, 40f),
                    Moisture = UnityEngine.Random.Range(0f, 1f),
                    NeedsUpdate = UnityEngine.Random.value > 0.5f
                });
            }
        }
        
        private void CreateTestWeather()
        {
            var entity = EntityManager.CreateEntity();
            EntityManager.AddComponentData(entity, new WeatherData
            {
                Type = WeatherType.Clear,
                Temperature = UnityEngine.Random.Range(-10f, 35f),
                Humidity = UnityEngine.Random.Range(0f, 1f),
                WindSpeed = UnityEngine.Random.Range(0f, 20f),
                WindDirection = UnityEngine.Random.Range(0f, 360f),
                RainIntensity = UnityEngine.Random.Range(0f, 1f),
                SnowIntensity = UnityEngine.Random.Range(0f, 1f),
                Visibility = UnityEngine.Random.Range(100f, 10000f),
                AtmosphericPressure = UnityEngine.Random.Range(95f, 105f),
                UVIndex = UnityEngine.Random.Range(0f, 11f),
                TimeOfDay = UnityEngine.Random.Range(0f, 24f),
                Season = Season.Spring,
                LastUpdateTime = 0f,
                NeedsUpdate = true
            });
        }
        
        private void CreateTestParticles()
        {
            for (int i = 0; i < PARTICLE_COUNT; i++)
            {
                var entity = EntityManager.CreateEntity();
                EntityManager.AddComponentData(entity, new MudParticleData
                {
                    Position = new float3(UnityEngine.Random.Range(-1000f, 1000f), 0, UnityEngine.Random.Range(-1000f, 1000f)),
                    Velocity = new float3(UnityEngine.Random.Range(-5f, 5f), UnityEngine.Random.Range(0f, 2f), UnityEngine.Random.Range(-5f, 5f)),
                    Size = UnityEngine.Random.Range(0.01f, 0.1f),
                    Lifetime = UnityEngine.Random.Range(1f, 10f),
                    Mass = UnityEngine.Random.Range(0.001f, 0.01f),
                    Temperature = UnityEngine.Random.Range(10f, 30f),
                    Viscosity = UnityEngine.Random.Range(0.1f, 1f)
                });
                EntityManager.AddComponentData(entity, new LocalTransform
                {
                    Position = new float3(UnityEngine.Random.Range(-1000f, 1000f), 0, UnityEngine.Random.Range(-1000f, 1000f)),
                    Rotation = quaternion.identity
                });
            }
        }
        
        private void CreateTestPlayers()
        {
            for (int i = 0; i < PLAYER_COUNT; i++)
            {
                var entity = EntityManager.CreateEntity();
                EntityManager.AddComponentData(entity, new PlayerTag());
                EntityManager.AddComponentData(entity, new NetworkData
                {
                    NetworkId = i + 1,
                    Ping = UnityEngine.Random.Range(10f, 200f),
                    LastUpdateTime = 0f,
                    SuspiciousInput = false,
                    InvalidInput = false,
                    SuspectedBot = false,
                    InputHistoryCount = 0,
                    CompensationFactor = 1.0f
                });
                EntityManager.AddComponentData(entity, new VehicleInput
                {
                    Vertical = UnityEngine.Random.Range(-1f, 1f),
                    Horizontal = UnityEngine.Random.Range(-1f, 1f),
                    Brake = UnityEngine.Random.value > 0.8f,
                    Handbrake = UnityEngine.Random.value > 0.9f
                });
                EntityManager.AddComponentData(entity, new LocalTransform
                {
                    Position = new float3(UnityEngine.Random.Range(-1000f, 1000f), 0, UnityEngine.Random.Range(-1000f, 1000f)),
                    Rotation = quaternion.identity
                });
            }
        }
    }
}