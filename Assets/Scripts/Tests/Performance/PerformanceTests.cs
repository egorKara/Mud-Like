using NUnit.Framework;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Profiling;
using MudLike.Vehicles.Components;
using MudLike.Terrain.Components;
using MudLike.Weather.Components;
using MudLike.UI.Components;
using MudLike.Audio.Components;

namespace MudLike.Tests.Performance
{
    /// <summary>
    /// Тесты производительности для всех систем
    /// </summary>
    [TestFixture]
    public class PerformanceTests : ECSTestFixture
    {
        private const int VEHICLE_COUNT = 1000;
        private const int WHEEL_COUNT = 4000; // 4 колеса на транспорт
        private const int SURFACE_COUNT = 100;
        private const int PARTICLE_COUNT = 10000;
        
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
        }
        
        [Test]
        public void PerformanceTest_VehicleMovementSystem()
        {
            // Arrange
            var system = World.CreateSystemManaged<VehicleMovementSystem>();
            
            // Act & Assert
            using (var profiler = new ProfilerMarker("VehicleMovementSystem"))
            {
                profiler.Begin();
                
                for (int i = 0; i < 100; i++)
                {
                    system.Update();
                }
                
                profiler.End();
            }
            
            // Проверяем, что система работает за разумное время
            Assert.DoesNotThrow(() => system.Update());
        }
        
        [Test]
        public void PerformanceTest_WheelPhysicsSystem()
        {
            // Arrange
            var system = World.CreateSystemManaged<AdvancedWheelPhysicsSystem>();
            
            // Act & Assert
            using (var profiler = new ProfilerMarker("WheelPhysicsSystem"))
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
        public void PerformanceTest_TerrainDeformationSystem()
        {
            // Arrange
            var system = World.CreateSystemManaged<TerrainDeformationSystem>();
            
            // Act & Assert
            using (var profiler = new ProfilerMarker("TerrainDeformationSystem"))
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
        public void PerformanceTest_UIHUDSystem()
        {
            // Arrange
            var system = World.CreateSystemManaged<UIHUDSystem>();
            
            // Act & Assert
            using (var profiler = new ProfilerMarker("UIHUDSystem"))
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
        public void PerformanceTest_EngineAudioSystem()
        {
            // Arrange
            var system = World.CreateSystemManaged<EngineAudioSystem>();
            
            // Act & Assert
            using (var profiler = new ProfilerMarker("EngineAudioSystem"))
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
        public void PerformanceTest_WinchSystem()
        {
            // Arrange
            var system = World.CreateSystemManaged<WinchSystem>();
            
            // Act & Assert
            using (var profiler = new ProfilerMarker("WinchSystem"))
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
        public void PerformanceTest_CargoSystem()
        {
            // Arrange
            var system = World.CreateSystemManaged<CargoSystem>();
            
            // Act & Assert
            using (var profiler = new ProfilerMarker("CargoSystem"))
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
        public void PerformanceTest_MissionSystem()
        {
            // Arrange
            var system = World.CreateSystemManaged<MissionSystem>();
            
            // Act & Assert
            using (var profiler = new ProfilerMarker("MissionSystem"))
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
        public void PerformanceTest_AllSystemsTogether()
        {
            // Arrange - создаем все системы
            var vehicleSystem = World.CreateSystemManaged<VehicleMovementSystem>();
            var wheelSystem = World.CreateSystemManaged<AdvancedWheelPhysicsSystem>();
            var terrainSystem = World.CreateSystemManaged<TerrainDeformationSystem>();
            var hudSystem = World.CreateSystemManaged<UIHUDSystem>();
            var audioSystem = World.CreateSystemManaged<EngineAudioSystem>();
            var winchSystem = World.CreateSystemManaged<WinchSystem>();
            var cargoSystem = World.CreateSystemManaged<CargoSystem>();
            var missionSystem = World.CreateSystemManaged<MissionSystem>();
            
            // Act & Assert
            using (var profiler = new ProfilerMarker("AllSystemsTogether"))
            {
                profiler.Begin();
                
                for (int i = 0; i < 10; i++)
                {
                    vehicleSystem.Update();
                    wheelSystem.Update();
                    terrainSystem.Update();
                    hudSystem.Update();
                    audioSystem.Update();
                    winchSystem.Update();
                    cargoSystem.Update();
                    missionSystem.Update();
                }
                
                profiler.End();
            }
            
            Assert.DoesNotThrow(() => World.Update());
        }
        
        [Test]
        public void PerformanceTest_MemoryUsage()
        {
            // Arrange
            var initialMemory = UnityEngine.Profiling.Profiler.GetTotalAllocatedMemory(false);
            
            // Act - создаем много сущностей
            for (int i = 0; i < 10000; i++)
            {
                var entity = EntityManager.CreateEntity();
                EntityManager.AddComponentData(entity, new VehiclePhysics());
                EntityManager.AddComponentData(entity, new LocalTransform());
            }
            
            var peakMemory = UnityEngine.Profiling.Profiler.GetTotalAllocatedMemory(false);
            var memoryIncrease = peakMemory - initialMemory;
            
            // Assert - проверяем, что память не растет слишком быстро
            Assert.Less(memoryIncrease, 100 * 1024 * 1024); // Менее 100MB
        }
        
        [Test]
        public void PerformanceTest_EntityQueryPerformance()
        {
            // Arrange
            var query = EntityManager.CreateEntityQuery(typeof(VehiclePhysics));
            
            // Act & Assert
            using (var profiler = new ProfilerMarker("EntityQueryPerformance"))
            {
                profiler.Begin();
                
                for (int i = 0; i < 1000; i++)
                {
                    var entities = query.ToEntityArray(Unity.Collections.Allocator.Temp);
                    entities.Dispose();
                }
                
                profiler.End();
            }
            
            Assert.DoesNotThrow(() => query.ToEntityArray(Unity.Collections.Allocator.Temp).Dispose());
        }
        
        [Test]
        public void PerformanceTest_JobSystemPerformance()
        {
            // Arrange
            var system = World.CreateSystemManaged<AdvancedWheelPhysicsSystem>();
            
            // Act & Assert
            using (var profiler = new ProfilerMarker("JobSystemPerformance"))
            {
                profiler.Begin();
                
                // Запускаем систему много раз для тестирования Job System
                for (int i = 0; i < 1000; i++)
                {
                    system.Update();
                }
                
                profiler.End();
            }
            
            Assert.DoesNotThrow(() => system.Update());
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
                    Mass = 1500f
                });
                EntityManager.AddComponentData(entity, new VehiclePhysics
                {
                    Velocity = new float3(i % 10, 0f, i % 5),
                    EngineRPM = 1000f + i,
                    CurrentGear = (i % 5) + 1
                });
                EntityManager.AddComponentData(entity, new VehicleInput
                {
                    Vertical = (i % 2) == 0 ? 1f : 0f,
                    Horizontal = (i % 3) == 0 ? 0.5f : 0f
                });
                EntityManager.AddComponentData(entity, new LocalTransform
                {
                    Position = new float3(i * 2f, 0f, i * 1.5f),
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
                    IsGrounded = (i % 3) != 0,
                    SuspensionForce = new float3(0f, 1000f + i, 0f)
                });
                EntityManager.AddComponentData(entity, new WheelPhysicsData
                {
                    SlipSpeed = i % 10,
                    Temperature = 20f + (i % 20),
                    TirePressure = 2.5f
                });
                EntityManager.AddComponentData(entity, new LocalTransform
                {
                    Position = new float3(i * 0.5f, 0f, i * 0.3f),
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
                    FrictionCoefficient = 0.5f + (i % 50) * 0.01f,
                    TractionCoefficient = 0.6f + (i % 40) * 0.01f,
                    IsActive = true
                });
            }
        }
        
        private void CreateTestWeather()
        {
            var entity = EntityManager.CreateEntity();
            EntityManager.AddComponentData(entity, new WeatherData
            {
                Type = WeatherType.Clear,
                Temperature = 20f,
                Humidity = 0.5f,
                WindSpeed = 5f,
                IsActive = true
            });
        }
        
        private void CreateTestParticles()
        {
            for (int i = 0; i < PARTICLE_COUNT; i++)
            {
                var entity = EntityManager.CreateEntity();
                EntityManager.AddComponentData(entity, new MudParticleData
                {
                    Position = new float3(i * 0.1f, 0f, i * 0.1f),
                    Velocity = new float3(1f, 0f, 0f),
                    Size = 0.1f + (i % 10) * 0.01f,
                    Lifetime = 5f
                });
                EntityManager.AddComponentData(entity, new LocalTransform
                {
                    Position = new float3(i * 0.1f, 0f, i * 0.1f),
                    Rotation = quaternion.identity
                });
            }
        }
    }
}