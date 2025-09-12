using NUnit.Framework;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Profiling;
using MudLike.Vehicles.Components;
using MudLike.Vehicles.Systems;
using MudLike.Terrain.Components;
using MudLike.Weather.Components;
using MudLike.Tests.Infrastructure;

namespace MudLike.Tests.Performance
{
    /// <summary>
    /// Продвинутые тесты производительности
    /// </summary>
    [TestFixture]
    public class AdvancedPerformanceTests : MudLikeTestFixture
    {
        private const int VEHICLE_COUNT = 1000;
        private const int WHEEL_COUNT = 4000;
        private const int SURFACE_COUNT = 100;
        private const int WEATHER_COUNT = 10;
        
        [SetUp]
        public override void Setup()
        {
            base.Setup();
            CreateTestData();
        }
        
        [Test]
        public void PerformanceTest_VehicleMovementSystem_1000Vehicles()
        {
            // Arrange
            var system = World.CreateSystemManaged<VehicleMovementSystem>();
            
            // Act & Assert
            using (var profiler = new ProfilerMarker("VehicleMovementSystem_1000Vehicles"))
            {
                profiler.Begin();
                
                for (int i = 0; i < 60; i++) // 60 frames
                {
                    system.Update();
                }
                
                profiler.End();
            }
            
            AssertSystemPerformance<VehicleMovementSystem>(16.67f); // 60 FPS
        }
        
        [Test]
        public void PerformanceTest_WheelPhysicsSystem_4000Wheels()
        {
            // Arrange
            var system = World.CreateSystemManaged<AdvancedWheelPhysicsSystem>();
            
            // Act & Assert
            using (var profiler = new ProfilerMarker("WheelPhysicsSystem_4000Wheels"))
            {
                profiler.Begin();
                
                for (int i = 0; i < 60; i++) // 60 frames
                {
                    system.Update();
                }
                
                profiler.End();
            }
            
            AssertSystemPerformance<AdvancedWheelPhysicsSystem>(16.67f); // 60 FPS
        }
        
        [Test]
        public void PerformanceTest_TerrainDeformationSystem_100Surfaces()
        {
            // Arrange
            var system = World.CreateSystemManaged<TerrainDeformationSystem>();
            
            // Act & Assert
            using (var profiler = new ProfilerMarker("TerrainDeformationSystem_100Surfaces"))
            {
                profiler.Begin();
                
                for (int i = 0; i < 60; i++) // 60 frames
                {
                    system.Update();
                }
                
                profiler.End();
            }
            
            AssertSystemPerformance<TerrainDeformationSystem>(16.67f); // 60 FPS
        }
        
        [Test]
        public void PerformanceTest_WeatherSystem_10Weathers()
        {
            // Arrange
            var system = World.CreateSystemManaged<WeatherSystem>();
            
            // Act & Assert
            using (var profiler = new ProfilerMarker("WeatherSystem_10Weathers"))
            {
                profiler.Begin();
                
                for (int i = 0; i < 60; i++) // 60 frames
                {
                    system.Update();
                }
                
                profiler.End();
            }
            
            AssertSystemPerformance<WeatherSystem>(16.67f); // 60 FPS
        }
        
        [Test]
        public void PerformanceTest_AllSystems_ShouldMaintain60FPS()
        {
            // Arrange
            var vehicleSystem = World.CreateSystemManaged<VehicleMovementSystem>();
            var wheelSystem = World.CreateSystemManaged<AdvancedWheelPhysicsSystem>();
            var terrainSystem = World.CreateSystemManaged<TerrainDeformationSystem>();
            var weatherSystem = World.CreateSystemManaged<WeatherSystem>();
            
            // Act & Assert
            using (var profiler = new ProfilerMarker("AllSystems_60FPS"))
            {
                profiler.Begin();
                
                for (int frame = 0; frame < 60; frame++)
                {
                    vehicleSystem.Update();
                    wheelSystem.Update();
                    terrainSystem.Update();
                    weatherSystem.Update();
                }
                
                profiler.End();
            }
            
            // Проверяем, что все системы работают за разумное время
            AssertSystemPerformance<VehicleMovementSystem>(16.67f);
            AssertSystemPerformance<AdvancedWheelPhysicsSystem>(16.67f);
            AssertSystemPerformance<TerrainDeformationSystem>(16.67f);
            AssertSystemPerformance<WeatherSystem>(16.67f);
        }
        
        [Test]
        public void PerformanceTest_MemoryUsage_ShouldNotLeak()
        {
            // Arrange
            var initialMemory = System.GC.GetTotalMemory(false);
            
            // Act
            for (int i = 0; i < 1000; i++)
            {
                var vehicle = CreateVehicle();
                var wheel = CreateWheel();
                var surface = CreateSurface(SurfaceType.Asphalt);
                var weather = CreateWeather();
                
                // Симулируем работу систем
                var vehicleSystem = World.CreateSystemManaged<VehicleMovementSystem>();
                var wheelSystem = World.CreateSystemManaged<AdvancedWheelPhysicsSystem>();
                var terrainSystem = World.CreateSystemManaged<TerrainDeformationSystem>();
                var weatherSystem = World.CreateSystemManaged<WeatherSystem>();
                
                vehicleSystem.Update();
                wheelSystem.Update();
                terrainSystem.Update();
                weatherSystem.Update();
                
                // Удаляем сущности
                EntityManager.DestroyEntity(vehicle);
                EntityManager.DestroyEntity(wheel);
                EntityManager.DestroyEntity(surface);
                EntityManager.DestroyEntity(weather);
            }
            
            // Принудительная сборка мусора
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
            System.GC.Collect();
            
            var finalMemory = System.GC.GetTotalMemory(false);
            var memoryIncrease = finalMemory - initialMemory;
            
            // Assert
            Assert.Less(memoryIncrease, 10 * 1024 * 1024, // Менее 10MB
                "Memory should not increase by more than 10MB, actual increase: {0}MB", 
                memoryIncrease / (1024 * 1024));
        }
        
        [Test]
        public void PerformanceTest_EntityQuery_ShouldBeFast()
        {
            // Arrange
            var query = EntityManager.CreateEntityQuery(typeof(VehiclePhysics));
            
            // Act & Assert
            using (var profiler = new ProfilerMarker("EntityQuery_Performance"))
            {
                profiler.Begin();
                
                for (int i = 0; i < 1000; i++)
                {
                    var entities = query.ToEntityArray(Unity.Collections.Allocator.Temp);
                    entities.Dispose();
                }
                
                profiler.End();
            }
            
            Assert.DoesNotThrow(() => 
            {
                var entities = query.ToEntityArray(Unity.Collections.Allocator.Temp);
                entities.Dispose();
            });
        }
        
        [Test]
        public void PerformanceTest_JobSystem_ShouldScaleWithCores()
        {
            // Arrange
            var system = World.CreateSystemManaged<AdvancedWheelPhysicsSystem>();
            
            // Act & Assert
            using (var profiler = new ProfilerMarker("JobSystem_Scaling"))
            {
                profiler.Begin();
                
                for (int i = 0; i < 1000; i++)
                {
                    system.Update();
                }
                
                profiler.End();
            }
            
            AssertSystemPerformance<AdvancedWheelPhysicsSystem>(100f); // 1000 iterations in 100ms
        }
        
        [Test]
        public void PerformanceTest_BurstCompilation_ShouldBeFast()
        {
            // Arrange
            var system = World.CreateSystemManaged<AdvancedWheelPhysicsSystem>();
            
            // Act & Assert
            using (var profiler = new ProfilerMarker("BurstCompilation_Performance"))
            {
                profiler.Begin();
                
                for (int i = 0; i < 10000; i++)
                {
                    system.Update();
                }
                
                profiler.End();
            }
            
            AssertSystemPerformance<AdvancedWheelPhysicsSystem>(1000f); // 10000 iterations in 1000ms
        }
        
        [Test]
        public void PerformanceTest_StressTest_ShouldHandleExtremeLoad()
        {
            // Arrange
            var vehicleSystem = World.CreateSystemManaged<VehicleMovementSystem>();
            var wheelSystem = World.CreateSystemManaged<AdvancedWheelPhysicsSystem>();
            var terrainSystem = World.CreateSystemManaged<TerrainDeformationSystem>();
            var weatherSystem = World.CreateSystemManaged<WeatherSystem>();
            
            // Act & Assert
            using (var profiler = new ProfilerMarker("StressTest_ExtremeLoad"))
            {
                profiler.Begin();
                
                for (int frame = 0; frame < 300; frame++) // 5 seconds at 60 FPS
                {
                    vehicleSystem.Update();
                    wheelSystem.Update();
                    terrainSystem.Update();
                    weatherSystem.Update();
                }
                
                profiler.End();
            }
            
            // Проверяем, что системы не падают под нагрузкой
            AssertSystemRunsWithoutErrors<VehicleMovementSystem>();
            AssertSystemRunsWithoutErrors<AdvancedWheelPhysicsSystem>();
            AssertSystemRunsWithoutErrors<TerrainDeformationSystem>();
            AssertSystemRunsWithoutErrors<WeatherSystem>();
        }
        
        private void CreateTestData()
        {
            // Создаем тестовые данные
            for (int i = 0; i < VEHICLE_COUNT; i++)
            {
                CreateVehicle();
            }
            
            for (int i = 0; i < WHEEL_COUNT; i++)
            {
                CreateWheel();
            }
            
            for (int i = 0; i < SURFACE_COUNT; i++)
            {
                CreateSurface(SurfaceType.Asphalt);
            }
            
            for (int i = 0; i < WEATHER_COUNT; i++)
            {
                CreateWeather();
            }
        }
    }
}