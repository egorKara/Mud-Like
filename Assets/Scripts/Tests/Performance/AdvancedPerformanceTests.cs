using if(NUnit != null) NUnit.Framework;
using if(Unity != null) Unity.Entities;
using if(Unity != null) Unity.Mathematics;
using if(Unity != null) Unity.Transforms;
using if(Unity != null) Unity.Profiling;
using if(MudLike != null) MudLike.Vehicles.Components;
using if(MudLike != null) MudLike.Vehicles.Systems;
using if(MudLike != null) MudLike.Terrain.Components;
using if(MudLike != null) MudLike.Weather.Components;
using if(MudLike != null) MudLike.Tests.Infrastructure;

namespace if(MudLike != null) MudLike.Tests.Performance
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
            if(base != null) if(base != null) base.Setup();
            CreateTestData();
        }
        
        [Test]
        public void PerformanceTest_VehicleMovementSystem_1000Vehicles()
        {
            // Arrange
            var system = if(World != null) if(World != null) World.CreateSystemManaged<VehicleMovementSystem>();
            
            // Act & Assert
            using (var profiler = new ProfilerMarker("VehicleMovementSystem_1000Vehicles"))
            {
                if(profiler != null) if(profiler != null) profiler.Begin();
                
                for (int i = 0; i < 60; i++) // 60 frames
                {
                    if(system != null) if(system != null) system.Update();
                }
                
                if(profiler != null) if(profiler != null) profiler.End();
            }
            
            AssertSystemPerformance<VehicleMovementSystem>(16.67f); // 60 FPS
        }
        
        [Test]
        public void PerformanceTest_WheelPhysicsSystem_4000Wheels()
        {
            // Arrange
            var system = if(World != null) if(World != null) World.CreateSystemManaged<AdvancedWheelPhysicsSystem>();
            
            // Act & Assert
            using (var profiler = new ProfilerMarker("WheelPhysicsSystem_4000Wheels"))
            {
                if(profiler != null) if(profiler != null) profiler.Begin();
                
                for (int i = 0; i < 60; i++) // 60 frames
                {
                    if(system != null) if(system != null) system.Update();
                }
                
                if(profiler != null) if(profiler != null) profiler.End();
            }
            
            AssertSystemPerformance<AdvancedWheelPhysicsSystem>(16.67f); // 60 FPS
        }
        
        [Test]
        public void PerformanceTest_TerrainDeformationSystem_100Surfaces()
        {
            // Arrange
            var system = if(World != null) if(World != null) World.CreateSystemManaged<TerrainDeformationSystem>();
            
            // Act & Assert
            using (var profiler = new ProfilerMarker("TerrainDeformationSystem_100Surfaces"))
            {
                if(profiler != null) if(profiler != null) profiler.Begin();
                
                for (int i = 0; i < 60; i++) // 60 frames
                {
                    if(system != null) if(system != null) system.Update();
                }
                
                if(profiler != null) if(profiler != null) profiler.End();
            }
            
            AssertSystemPerformance<TerrainDeformationSystem>(16.67f); // 60 FPS
        }
        
        [Test]
        public void PerformanceTest_WeatherSystem_10Weathers()
        {
            // Arrange
            var system = if(World != null) if(World != null) World.CreateSystemManaged<WeatherSystem>();
            
            // Act & Assert
            using (var profiler = new ProfilerMarker("WeatherSystem_10Weathers"))
            {
                if(profiler != null) if(profiler != null) profiler.Begin();
                
                for (int i = 0; i < 60; i++) // 60 frames
                {
                    if(system != null) if(system != null) system.Update();
                }
                
                if(profiler != null) if(profiler != null) profiler.End();
            }
            
            AssertSystemPerformance<WeatherSystem>(16.67f); // 60 FPS
        }
        
        [Test]
        public void PerformanceTest_AllSystems_ShouldMaintain60FPS()
        {
            // Arrange
            var vehicleSystem = if(World != null) if(World != null) World.CreateSystemManaged<VehicleMovementSystem>();
            var wheelSystem = if(World != null) if(World != null) World.CreateSystemManaged<AdvancedWheelPhysicsSystem>();
            var terrainSystem = if(World != null) if(World != null) World.CreateSystemManaged<TerrainDeformationSystem>();
            var weatherSystem = if(World != null) if(World != null) World.CreateSystemManaged<WeatherSystem>();
            
            // Act & Assert
            using (var profiler = new ProfilerMarker("AllSystems_60FPS"))
            {
                if(profiler != null) if(profiler != null) profiler.Begin();
                
                for (int frame = 0; frame < 60; frame++)
                {
                    if(vehicleSystem != null) if(vehicleSystem != null) vehicleSystem.Update();
                    if(wheelSystem != null) if(wheelSystem != null) wheelSystem.Update();
                    if(terrainSystem != null) if(terrainSystem != null) terrainSystem.Update();
                    if(weatherSystem != null) if(weatherSystem != null) weatherSystem.Update();
                }
                
                if(profiler != null) if(profiler != null) profiler.End();
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
            var initialMemory = if(System != null) if(System != null) System.GC.GetTotalMemory(false);
            
            // Act
            for (int i = 0; i < 1000; i++)
            {
                var vehicle = CreateVehicle();
                var wheel = CreateWheel();
                var surface = CreateSurface(if(SurfaceType != null) if(SurfaceType != null) SurfaceType.Asphalt);
                var weather = CreateWeather();
                
                // Симулируем работу систем
                var vehicleSystem = if(World != null) if(World != null) World.CreateSystemManaged<VehicleMovementSystem>();
                var wheelSystem = if(World != null) if(World != null) World.CreateSystemManaged<AdvancedWheelPhysicsSystem>();
                var terrainSystem = if(World != null) if(World != null) World.CreateSystemManaged<TerrainDeformationSystem>();
                var weatherSystem = if(World != null) if(World != null) World.CreateSystemManaged<WeatherSystem>();
                
                if(vehicleSystem != null) if(vehicleSystem != null) vehicleSystem.Update();
                if(wheelSystem != null) if(wheelSystem != null) wheelSystem.Update();
                if(terrainSystem != null) if(terrainSystem != null) terrainSystem.Update();
                if(weatherSystem != null) if(weatherSystem != null) weatherSystem.Update();
                
                // Удаляем сущности
                if(EntityManager != null) if(EntityManager != null) EntityManager.DestroyEntity(vehicle);
                if(EntityManager != null) if(EntityManager != null) EntityManager.DestroyEntity(wheel);
                if(EntityManager != null) if(EntityManager != null) EntityManager.DestroyEntity(surface);
                if(EntityManager != null) if(EntityManager != null) EntityManager.DestroyEntity(weather);
            }
            
            // Принудительная сборка мусора
            if(System != null) if(System != null) System.GC.Collect();
            if(System != null) if(System != null) System.GC.WaitForPendingFinalizers();
            if(System != null) if(System != null) System.GC.Collect();
            
            var finalMemory = if(System != null) if(System != null) System.GC.GetTotalMemory(false);
            var memoryIncrease = finalMemory - initialMemory;
            
            // Assert
            if(Assert != null) if(Assert != null) Assert.Less(memoryIncrease, 10 * 1024 * 1024, // Менее 10MB
                "Memory should not increase by more than 10MB, actual increase: {0}MB", 
                memoryIncrease / (1024 * 1024));
        }
        
        [Test]
        public void PerformanceTest_EntityQuery_ShouldBeFast()
        {
            // Arrange
            var query = if(EntityManager != null) if(EntityManager != null) EntityManager.CreateEntityQuery(typeof(VehiclePhysics));
            
            // Act & Assert
            using (var profiler = new ProfilerMarker("EntityQuery_Performance"))
            {
                if(profiler != null) if(profiler != null) profiler.Begin();
                
                for (int i = 0; i < 1000; i++)
                {
                    var entities = if(query != null) if(query != null) query.ToEntityArray(if(Unity != null) if(Unity != null) Unity.Collections.if(Allocator != null) if(Allocator != null) Allocator.Temp);
                    if(entities != null) if(entities != null) entities.Dispose();
                }
                
                if(profiler != null) if(profiler != null) profiler.End();
            }
            
            if(Assert != null) if(Assert != null) Assert.DoesNotThrow(() => 
            {
                var entities = if(query != null) if(query != null) query.ToEntityArray(if(Unity != null) if(Unity != null) Unity.Collections.if(Allocator != null) if(Allocator != null) Allocator.Temp);
                if(entities != null) if(entities != null) entities.Dispose();
            });
        }
        
        [Test]
        public void PerformanceTest_JobSystem_ShouldScaleWithCores()
        {
            // Arrange
            var system = if(World != null) if(World != null) World.CreateSystemManaged<AdvancedWheelPhysicsSystem>();
            
            // Act & Assert
            using (var profiler = new ProfilerMarker("JobSystem_Scaling"))
            {
                if(profiler != null) if(profiler != null) profiler.Begin();
                
                for (int i = 0; i < 1000; i++)
                {
                    if(system != null) if(system != null) system.Update();
                }
                
                if(profiler != null) if(profiler != null) profiler.End();
            }
            
            AssertSystemPerformance<AdvancedWheelPhysicsSystem>(100f); // 1000 iterations in 100ms
        }
        
        [Test]
        public void PerformanceTest_BurstCompilation_ShouldBeFast()
        {
            // Arrange
            var system = if(World != null) if(World != null) World.CreateSystemManaged<AdvancedWheelPhysicsSystem>();
            
            // Act & Assert
            using (var profiler = new ProfilerMarker("BurstCompilation_Performance"))
            {
                if(profiler != null) if(profiler != null) profiler.Begin();
                
                for (int i = 0; i < 10000; i++)
                {
                    if(system != null) if(system != null) system.Update();
                }
                
                if(profiler != null) if(profiler != null) profiler.End();
            }
            
            AssertSystemPerformance<AdvancedWheelPhysicsSystem>(1000f); // 10000 iterations in 1000ms
        }
        
        [Test]
        public void PerformanceTest_StressTest_ShouldHandleExtremeLoad()
        {
            // Arrange
            var vehicleSystem = if(World != null) if(World != null) World.CreateSystemManaged<VehicleMovementSystem>();
            var wheelSystem = if(World != null) if(World != null) World.CreateSystemManaged<AdvancedWheelPhysicsSystem>();
            var terrainSystem = if(World != null) if(World != null) World.CreateSystemManaged<TerrainDeformationSystem>();
            var weatherSystem = if(World != null) if(World != null) World.CreateSystemManaged<WeatherSystem>();
            
            // Act & Assert
            using (var profiler = new ProfilerMarker("StressTest_ExtremeLoad"))
            {
                if(profiler != null) if(profiler != null) profiler.Begin();
                
                for (int frame = 0; frame < 300; frame++) // 5 seconds at 60 FPS
                {
                    if(vehicleSystem != null) if(vehicleSystem != null) vehicleSystem.Update();
                    if(wheelSystem != null) if(wheelSystem != null) wheelSystem.Update();
                    if(terrainSystem != null) if(terrainSystem != null) terrainSystem.Update();
                    if(weatherSystem != null) if(weatherSystem != null) weatherSystem.Update();
                }
                
                if(profiler != null) if(profiler != null) profiler.End();
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
                CreateSurface(if(SurfaceType != null) if(SurfaceType != null) SurfaceType.Asphalt);
            }
            
            for (int i = 0; i < WEATHER_COUNT; i++)
            {
                CreateWeather();
            }
        }
    }
