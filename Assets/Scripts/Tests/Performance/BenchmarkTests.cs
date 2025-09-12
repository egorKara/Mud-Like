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
using MudLike.Pooling.Components;

namespace MudLike.Tests.Performance
{
    /// <summary>
    /// Бенчмарк тесты для измерения производительности
    /// </summary>
    [TestFixture]
    public class BenchmarkTests : ECSTestFixture
    {
        private const int BENCHMARK_ITERATIONS = 1000;
        private const int ENTITY_COUNT = 10000;
        
        [SetUp]
        public override void Setup()
        {
            base.Setup();
            
            // Создаем тестовые данные для бенчмарка
            CreateBenchmarkData();
        }
        
        [Test]
        public void Benchmark_VehicleMovementSystem()
        {
            // Arrange
            var system = World.CreateSystemManaged<VehicleMovementSystem>();
            
            // Act & Measure
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            
            for (int i = 0; i < BENCHMARK_ITERATIONS; i++)
            {
                system.Update();
            }
            
            stopwatch.Stop();
            
            // Assert
            var averageTime = stopwatch.ElapsedMilliseconds / (float)BENCHMARK_ITERATIONS;
            Assert.Less(averageTime, 1f, $"VehicleMovementSystem took {averageTime}ms per update on average");
            
            Debug.Log($"VehicleMovementSystem: {averageTime}ms per update, {stopwatch.ElapsedMilliseconds}ms total");
        }
        
        [Test]
        public void Benchmark_WheelPhysicsSystem()
        {
            // Arrange
            var system = World.CreateSystemManaged<AdvancedWheelPhysicsSystem>();
            
            // Act & Measure
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            
            for (int i = 0; i < BENCHMARK_ITERATIONS; i++)
            {
                system.Update();
            }
            
            stopwatch.Stop();
            
            // Assert
            var averageTime = stopwatch.ElapsedMilliseconds / (float)BENCHMARK_ITERATIONS;
            Assert.Less(averageTime, 2f, $"WheelPhysicsSystem took {averageTime}ms per update on average");
            
            Debug.Log($"WheelPhysicsSystem: {averageTime}ms per update, {stopwatch.ElapsedMilliseconds}ms total");
        }
        
        [Test]
        public void Benchmark_TerrainDeformationSystem()
        {
            // Arrange
            var system = World.CreateSystemManaged<TerrainDeformationSystem>();
            
            // Act & Measure
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            
            for (int i = 0; i < BENCHMARK_ITERATIONS; i++)
            {
                system.Update();
            }
            
            stopwatch.Stop();
            
            // Assert
            var averageTime = stopwatch.ElapsedMilliseconds / (float)BENCHMARK_ITERATIONS;
            Assert.Less(averageTime, 3f, $"TerrainDeformationSystem took {averageTime}ms per update on average");
            
            Debug.Log($"TerrainDeformationSystem: {averageTime}ms per update, {stopwatch.ElapsedMilliseconds}ms total");
        }
        
        [Test]
        public void Benchmark_UIHUDSystem()
        {
            // Arrange
            var system = World.CreateSystemManaged<UIHUDSystem>();
            
            // Act & Measure
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            
            for (int i = 0; i < BENCHMARK_ITERATIONS; i++)
            {
                system.Update();
            }
            
            stopwatch.Stop();
            
            // Assert
            var averageTime = stopwatch.ElapsedMilliseconds / (float)BENCHMARK_ITERATIONS;
            Assert.Less(averageTime, 0.5f, $"UIHUDSystem took {averageTime}ms per update on average");
            
            Debug.Log($"UIHUDSystem: {averageTime}ms per update, {stopwatch.ElapsedMilliseconds}ms total");
        }
        
        [Test]
        public void Benchmark_EngineAudioSystem()
        {
            // Arrange
            var system = World.CreateSystemManaged<EngineAudioSystem>();
            
            // Act & Measure
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            
            for (int i = 0; i < BENCHMARK_ITERATIONS; i++)
            {
                system.Update();
            }
            
            stopwatch.Stop();
            
            // Assert
            var averageTime = stopwatch.ElapsedMilliseconds / (float)BENCHMARK_ITERATIONS;
            Assert.Less(averageTime, 1f, $"EngineAudioSystem took {averageTime}ms per update on average");
            
            Debug.Log($"EngineAudioSystem: {averageTime}ms per update, {stopwatch.ElapsedMilliseconds}ms total");
        }
        
        [Test]
        public void Benchmark_WinchSystem()
        {
            // Arrange
            var system = World.CreateSystemManaged<WinchSystem>();
            
            // Act & Measure
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            
            for (int i = 0; i < BENCHMARK_ITERATIONS; i++)
            {
                system.Update();
            }
            
            stopwatch.Stop();
            
            // Assert
            var averageTime = stopwatch.ElapsedMilliseconds / (float)BENCHMARK_ITERATIONS;
            Assert.Less(averageTime, 1.5f, $"WinchSystem took {averageTime}ms per update on average");
            
            Debug.Log($"WinchSystem: {averageTime}ms per update, {stopwatch.ElapsedMilliseconds}ms total");
        }
        
        [Test]
        public void Benchmark_CargoSystem()
        {
            // Arrange
            var system = World.CreateSystemManaged<CargoSystem>();
            
            // Act & Measure
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            
            for (int i = 0; i < BENCHMARK_ITERATIONS; i++)
            {
                system.Update();
            }
            
            stopwatch.Stop();
            
            // Assert
            var averageTime = stopwatch.ElapsedMilliseconds / (float)BENCHMARK_ITERATIONS;
            Assert.Less(averageTime, 1f, $"CargoSystem took {averageTime}ms per update on average");
            
            Debug.Log($"CargoSystem: {averageTime}ms per update, {stopwatch.ElapsedMilliseconds}ms total");
        }
        
        [Test]
        public void Benchmark_MissionSystem()
        {
            // Arrange
            var system = World.CreateSystemManaged<MissionSystem>();
            
            // Act & Measure
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            
            for (int i = 0; i < BENCHMARK_ITERATIONS; i++)
            {
                system.Update();
            }
            
            stopwatch.Stop();
            
            // Assert
            var averageTime = stopwatch.ElapsedMilliseconds / (float)BENCHMARK_ITERATIONS;
            Assert.Less(averageTime, 0.5f, $"MissionSystem took {averageTime}ms per update on average");
            
            Debug.Log($"MissionSystem: {averageTime}ms per update, {stopwatch.ElapsedMilliseconds}ms total");
        }
        
        [Test]
        public void Benchmark_LODSystem()
        {
            // Arrange
            var system = World.CreateSystemManaged<LODSystem>();
            
            // Act & Measure
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            
            for (int i = 0; i < BENCHMARK_ITERATIONS; i++)
            {
                system.Update();
            }
            
            stopwatch.Stop();
            
            // Assert
            var averageTime = stopwatch.ElapsedMilliseconds / (float)BENCHMARK_ITERATIONS;
            Assert.Less(averageTime, 1f, $"LODSystem took {averageTime}ms per update on average");
            
            Debug.Log($"LODSystem: {averageTime}ms per update, {stopwatch.ElapsedMilliseconds}ms total");
        }
        
        [Test]
        public void Benchmark_ObjectPoolSystem()
        {
            // Arrange
            var system = World.CreateSystemManaged<ObjectPoolSystem>();
            
            // Act & Measure
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            
            for (int i = 0; i < BENCHMARK_ITERATIONS; i++)
            {
                system.Update();
            }
            
            stopwatch.Stop();
            
            // Assert
            var averageTime = stopwatch.ElapsedMilliseconds / (float)BENCHMARK_ITERATIONS;
            Assert.Less(averageTime, 0.5f, $"ObjectPoolSystem took {averageTime}ms per update on average");
            
            Debug.Log($"ObjectPoolSystem: {averageTime}ms per update, {stopwatch.ElapsedMilliseconds}ms total");
        }
        
        [Test]
        public void Benchmark_AllSystemsTogether()
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
            var lodSystem = World.CreateSystemManaged<LODSystem>();
            var poolSystem = World.CreateSystemManaged<ObjectPoolSystem>();
            
            // Act & Measure
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            
            for (int i = 0; i < BENCHMARK_ITERATIONS / 10; i++) // Меньше итераций для всех систем
            {
                vehicleSystem.Update();
                wheelSystem.Update();
                terrainSystem.Update();
                hudSystem.Update();
                audioSystem.Update();
                winchSystem.Update();
                cargoSystem.Update();
                missionSystem.Update();
                lodSystem.Update();
                poolSystem.Update();
            }
            
            stopwatch.Stop();
            
            // Assert
            var averageTime = stopwatch.ElapsedMilliseconds / (float)(BENCHMARK_ITERATIONS / 10);
            Assert.Less(averageTime, 10f, $"All systems together took {averageTime}ms per update on average");
            
            Debug.Log($"All systems together: {averageTime}ms per update, {stopwatch.ElapsedMilliseconds}ms total");
        }
        
        [Test]
        public void Benchmark_MemoryAllocation()
        {
            // Arrange
            var initialMemory = Profiler.GetTotalAllocatedMemory(false);
            
            // Act
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            
            for (int i = 0; i < 1000; i++)
            {
                // Создаем и уничтожаем сущности
                var entity = EntityManager.CreateEntity();
                EntityManager.AddComponentData(entity, new VehiclePhysics());
                EntityManager.AddComponentData(entity, new LocalTransform());
                EntityManager.DestroyEntity(entity);
            }
            
            stopwatch.Stop();
            
            // Assert
            var finalMemory = Profiler.GetTotalAllocatedMemory(false);
            var memoryIncrease = finalMemory - initialMemory;
            
            Assert.Less(memoryIncrease, 10 * 1024 * 1024, $"Memory increased by {memoryIncrease / 1024 / 1024}MB");
            
            Debug.Log($"Memory allocation: {memoryIncrease / 1024 / 1024}MB increase, {stopwatch.ElapsedMilliseconds}ms total");
        }
        
        [Test]
        public void Benchmark_EntityQueryPerformance()
        {
            // Arrange
            var query = EntityManager.CreateEntityQuery(typeof(VehiclePhysics));
            
            // Act & Measure
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            
            for (int i = 0; i < BENCHMARK_ITERATIONS; i++)
            {
                var entities = query.ToEntityArray(Allocator.Temp);
                entities.Dispose();
            }
            
            stopwatch.Stop();
            
            // Assert
            var averageTime = stopwatch.ElapsedMilliseconds / (float)BENCHMARK_ITERATIONS;
            Assert.Less(averageTime, 0.1f, $"EntityQuery took {averageTime}ms per query on average");
            
            Debug.Log($"EntityQuery: {averageTime}ms per query, {stopwatch.ElapsedMilliseconds}ms total");
        }
        
        private void CreateBenchmarkData()
        {
            // Создаем много сущностей для бенчмарка
            for (int i = 0; i < ENTITY_COUNT; i++)
            {
                // Транспорт
                var vehicleEntity = EntityManager.CreateEntity();
                EntityManager.AddComponentData(vehicleEntity, new VehicleTag());
                EntityManager.AddComponentData(vehicleEntity, new VehicleConfig
                {
                    MaxSpeed = 100f,
                    Mass = 1500f
                });
                EntityManager.AddComponentData(vehicleEntity, new VehiclePhysics
                {
                    Velocity = new float3(i % 10, 0f, i % 5),
                    EngineRPM = 1000f + i
                });
                EntityManager.AddComponentData(vehicleEntity, new LocalTransform
                {
                    Position = new float3(i * 2f, 0f, i * 1.5f),
                    Rotation = quaternion.identity
                });
                
                // Колеса
                for (int j = 0; j < 4; j++)
                {
                    var wheelEntity = EntityManager.CreateEntity();
                    EntityManager.AddComponentData(wheelEntity, new WheelData
                    {
                        Radius = 0.5f,
                        IsGrounded = (i + j) % 3 != 0
                    });
                    EntityManager.AddComponentData(wheelEntity, new LocalTransform
                    {
                        Position = new float3(i * 2f + j, 0f, i * 1.5f),
                        Rotation = quaternion.identity
                    });
                }
                
                // Поверхности
                if (i % 10 == 0)
                {
                    var surfaceEntity = EntityManager.CreateEntity();
                    EntityManager.AddComponentData(surfaceEntity, new SurfaceData
                    {
                        SurfaceType = (SurfaceType)(i % 12),
                        FrictionCoefficient = 0.5f + (i % 50) * 0.01f
                    });
                }
                
                // HUD
                if (i % 100 == 0)
                {
                    var hudEntity = EntityManager.CreateEntity();
                    EntityManager.AddComponentData(hudEntity, new UIHUDData
                    {
                        Speed = i % 100,
                        RPM = 1000f + i
                    });
                }
                
                // Аудио
                if (i % 50 == 0)
                {
                    var audioEntity = EntityManager.CreateEntity();
                    EntityManager.AddComponentData(audioEntity, new EngineAudioData
                    {
                        RPM = 1000f + i,
                        Volume = 0.5f
                    });
                }
                
                // LOD
                if (i % 20 == 0)
                {
                    var lodEntity = EntityManager.CreateEntity();
                    EntityManager.AddComponentData(lodEntity, new LODData
                    {
                        CurrentLOD = i % 4,
                        DistanceToCamera = i * 0.1f
                    });
                    EntityManager.AddComponentData(lodEntity, new LocalTransform
                    {
                        Position = new float3(i * 2f, 0f, i * 1.5f),
                        Rotation = quaternion.identity
                    });
                }
            }
        }
    }
}