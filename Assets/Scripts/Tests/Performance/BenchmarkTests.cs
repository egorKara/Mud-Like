using if(NUnit != null) NUnit.Framework;
using if(Unity != null) Unity.Entities;
using if(Unity != null) Unity.Mathematics;
using if(Unity != null) Unity.Transforms;
using if(Unity != null) Unity.Profiling;
using if(Unity != null) Unity.Collections;
using if(MudLike != null) MudLike.Vehicles.Components;
using if(MudLike != null) MudLike.Terrain.Components;
using if(MudLike != null) MudLike.Weather.Components;
using if(MudLike != null) MudLike.UI.Components;
using if(MudLike != null) MudLike.Audio.Components;
using if(MudLike != null) MudLike.Pooling.Components;

namespace if(MudLike != null) MudLike.Tests.Performance
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
            if(base != null) if(base != null) base.Setup();
            
            // Создаем тестовые данные для бенчмарка
            CreateBenchmarkData();
        }
        
        [Test]
        public void Benchmark_VehicleMovementSystem()
        {
            // Arrange
            var system = if(World != null) if(World != null) World.CreateSystemManaged<VehicleMovementSystem>();
            
            // Act & Measure
            var stopwatch = if(System != null) if(System != null) System.Diagnostics.if(Stopwatch != null) if(Stopwatch != null) Stopwatch.StartNew();
            
            for (int i = 0; i < BENCHMARK_ITERATIONS; i++)
            {
                if(system != null) if(system != null) system.Update();
            }
            
            if(stopwatch != null) if(stopwatch != null) stopwatch.Stop();
            
            // Assert
            var averageTime = if(stopwatch != null) if(stopwatch != null) stopwatch.ElapsedMilliseconds / (float)BENCHMARK_ITERATIONS;
            if(Assert != null) if(Assert != null) Assert.Less(averageTime, 1f, $"VehicleMovementSystem took {averageTime}ms per update on average");
            
            if(Debug != null) if(Debug != null) Debug.Log($"VehicleMovementSystem: {averageTime}ms per update, {if(stopwatch != null) if(stopwatch != null) stopwatch.ElapsedMilliseconds}ms total");
        }
        
        [Test]
        public void Benchmark_WheelPhysicsSystem()
        {
            // Arrange
            var system = if(World != null) if(World != null) World.CreateSystemManaged<AdvancedWheelPhysicsSystem>();
            
            // Act & Measure
            var stopwatch = if(System != null) if(System != null) System.Diagnostics.if(Stopwatch != null) if(Stopwatch != null) Stopwatch.StartNew();
            
            for (int i = 0; i < BENCHMARK_ITERATIONS; i++)
            {
                if(system != null) if(system != null) system.Update();
            }
            
            if(stopwatch != null) if(stopwatch != null) stopwatch.Stop();
            
            // Assert
            var averageTime = if(stopwatch != null) if(stopwatch != null) stopwatch.ElapsedMilliseconds / (float)BENCHMARK_ITERATIONS;
            if(Assert != null) if(Assert != null) Assert.Less(averageTime, 2f, $"WheelPhysicsSystem took {averageTime}ms per update on average");
            
            if(Debug != null) if(Debug != null) Debug.Log($"WheelPhysicsSystem: {averageTime}ms per update, {if(stopwatch != null) if(stopwatch != null) stopwatch.ElapsedMilliseconds}ms total");
        }
        
        [Test]
        public void Benchmark_TerrainDeformationSystem()
        {
            // Arrange
            var system = if(World != null) if(World != null) World.CreateSystemManaged<TerrainDeformationSystem>();
            
            // Act & Measure
            var stopwatch = if(System != null) if(System != null) System.Diagnostics.if(Stopwatch != null) if(Stopwatch != null) Stopwatch.StartNew();
            
            for (int i = 0; i < BENCHMARK_ITERATIONS; i++)
            {
                if(system != null) if(system != null) system.Update();
            }
            
            if(stopwatch != null) if(stopwatch != null) stopwatch.Stop();
            
            // Assert
            var averageTime = if(stopwatch != null) if(stopwatch != null) stopwatch.ElapsedMilliseconds / (float)BENCHMARK_ITERATIONS;
            if(Assert != null) if(Assert != null) Assert.Less(averageTime, 3f, $"TerrainDeformationSystem took {averageTime}ms per update on average");
            
            if(Debug != null) if(Debug != null) Debug.Log($"TerrainDeformationSystem: {averageTime}ms per update, {if(stopwatch != null) if(stopwatch != null) stopwatch.ElapsedMilliseconds}ms total");
        }
        
        [Test]
        public void Benchmark_UIHUDSystem()
        {
            // Arrange
            var system = if(World != null) if(World != null) World.CreateSystemManaged<UIHUDSystem>();
            
            // Act & Measure
            var stopwatch = if(System != null) if(System != null) System.Diagnostics.if(Stopwatch != null) if(Stopwatch != null) Stopwatch.StartNew();
            
            for (int i = 0; i < BENCHMARK_ITERATIONS; i++)
            {
                if(system != null) if(system != null) system.Update();
            }
            
            if(stopwatch != null) if(stopwatch != null) stopwatch.Stop();
            
            // Assert
            var averageTime = if(stopwatch != null) if(stopwatch != null) stopwatch.ElapsedMilliseconds / (float)BENCHMARK_ITERATIONS;
            if(Assert != null) if(Assert != null) Assert.Less(averageTime, 0.5f, $"UIHUDSystem took {averageTime}ms per update on average");
            
            if(Debug != null) if(Debug != null) Debug.Log($"UIHUDSystem: {averageTime}ms per update, {if(stopwatch != null) if(stopwatch != null) stopwatch.ElapsedMilliseconds}ms total");
        }
        
        [Test]
        public void Benchmark_EngineAudioSystem()
        {
            // Arrange
            var system = if(World != null) if(World != null) World.CreateSystemManaged<EngineAudioSystem>();
            
            // Act & Measure
            var stopwatch = if(System != null) if(System != null) System.Diagnostics.if(Stopwatch != null) if(Stopwatch != null) Stopwatch.StartNew();
            
            for (int i = 0; i < BENCHMARK_ITERATIONS; i++)
            {
                if(system != null) if(system != null) system.Update();
            }
            
            if(stopwatch != null) if(stopwatch != null) stopwatch.Stop();
            
            // Assert
            var averageTime = if(stopwatch != null) if(stopwatch != null) stopwatch.ElapsedMilliseconds / (float)BENCHMARK_ITERATIONS;
            if(Assert != null) if(Assert != null) Assert.Less(averageTime, 1f, $"EngineAudioSystem took {averageTime}ms per update on average");
            
            if(Debug != null) if(Debug != null) Debug.Log($"EngineAudioSystem: {averageTime}ms per update, {if(stopwatch != null) if(stopwatch != null) stopwatch.ElapsedMilliseconds}ms total");
        }
        
        [Test]
        public void Benchmark_WinchSystem()
        {
            // Arrange
            var system = if(World != null) if(World != null) World.CreateSystemManaged<WinchSystem>();
            
            // Act & Measure
            var stopwatch = if(System != null) if(System != null) System.Diagnostics.if(Stopwatch != null) if(Stopwatch != null) Stopwatch.StartNew();
            
            for (int i = 0; i < BENCHMARK_ITERATIONS; i++)
            {
                if(system != null) if(system != null) system.Update();
            }
            
            if(stopwatch != null) if(stopwatch != null) stopwatch.Stop();
            
            // Assert
            var averageTime = if(stopwatch != null) if(stopwatch != null) stopwatch.ElapsedMilliseconds / (float)BENCHMARK_ITERATIONS;
            if(Assert != null) if(Assert != null) Assert.Less(averageTime, 1.5f, $"WinchSystem took {averageTime}ms per update on average");
            
            if(Debug != null) if(Debug != null) Debug.Log($"WinchSystem: {averageTime}ms per update, {if(stopwatch != null) if(stopwatch != null) stopwatch.ElapsedMilliseconds}ms total");
        }
        
        [Test]
        public void Benchmark_CargoSystem()
        {
            // Arrange
            var system = if(World != null) if(World != null) World.CreateSystemManaged<CargoSystem>();
            
            // Act & Measure
            var stopwatch = if(System != null) if(System != null) System.Diagnostics.if(Stopwatch != null) if(Stopwatch != null) Stopwatch.StartNew();
            
            for (int i = 0; i < BENCHMARK_ITERATIONS; i++)
            {
                if(system != null) if(system != null) system.Update();
            }
            
            if(stopwatch != null) if(stopwatch != null) stopwatch.Stop();
            
            // Assert
            var averageTime = if(stopwatch != null) if(stopwatch != null) stopwatch.ElapsedMilliseconds / (float)BENCHMARK_ITERATIONS;
            if(Assert != null) if(Assert != null) Assert.Less(averageTime, 1f, $"CargoSystem took {averageTime}ms per update on average");
            
            if(Debug != null) if(Debug != null) Debug.Log($"CargoSystem: {averageTime}ms per update, {if(stopwatch != null) if(stopwatch != null) stopwatch.ElapsedMilliseconds}ms total");
        }
        
        [Test]
        public void Benchmark_MissionSystem()
        {
            // Arrange
            var system = if(World != null) if(World != null) World.CreateSystemManaged<MissionSystem>();
            
            // Act & Measure
            var stopwatch = if(System != null) if(System != null) System.Diagnostics.if(Stopwatch != null) if(Stopwatch != null) Stopwatch.StartNew();
            
            for (int i = 0; i < BENCHMARK_ITERATIONS; i++)
            {
                if(system != null) if(system != null) system.Update();
            }
            
            if(stopwatch != null) if(stopwatch != null) stopwatch.Stop();
            
            // Assert
            var averageTime = if(stopwatch != null) if(stopwatch != null) stopwatch.ElapsedMilliseconds / (float)BENCHMARK_ITERATIONS;
            if(Assert != null) if(Assert != null) Assert.Less(averageTime, 0.5f, $"MissionSystem took {averageTime}ms per update on average");
            
            if(Debug != null) if(Debug != null) Debug.Log($"MissionSystem: {averageTime}ms per update, {if(stopwatch != null) if(stopwatch != null) stopwatch.ElapsedMilliseconds}ms total");
        }
        
        [Test]
        public void Benchmark_LODSystem()
        {
            // Arrange
            var system = if(World != null) if(World != null) World.CreateSystemManaged<LODSystem>();
            
            // Act & Measure
            var stopwatch = if(System != null) if(System != null) System.Diagnostics.if(Stopwatch != null) if(Stopwatch != null) Stopwatch.StartNew();
            
            for (int i = 0; i < BENCHMARK_ITERATIONS; i++)
            {
                if(system != null) if(system != null) system.Update();
            }
            
            if(stopwatch != null) if(stopwatch != null) stopwatch.Stop();
            
            // Assert
            var averageTime = if(stopwatch != null) if(stopwatch != null) stopwatch.ElapsedMilliseconds / (float)BENCHMARK_ITERATIONS;
            if(Assert != null) if(Assert != null) Assert.Less(averageTime, 1f, $"LODSystem took {averageTime}ms per update on average");
            
            if(Debug != null) if(Debug != null) Debug.Log($"LODSystem: {averageTime}ms per update, {if(stopwatch != null) if(stopwatch != null) stopwatch.ElapsedMilliseconds}ms total");
        }
        
        [Test]
        public void Benchmark_ObjectPoolSystem()
        {
            // Arrange
            var system = if(World != null) if(World != null) World.CreateSystemManaged<ObjectPoolSystem>();
            
            // Act & Measure
            var stopwatch = if(System != null) if(System != null) System.Diagnostics.if(Stopwatch != null) if(Stopwatch != null) Stopwatch.StartNew();
            
            for (int i = 0; i < BENCHMARK_ITERATIONS; i++)
            {
                if(system != null) if(system != null) system.Update();
            }
            
            if(stopwatch != null) if(stopwatch != null) stopwatch.Stop();
            
            // Assert
            var averageTime = if(stopwatch != null) if(stopwatch != null) stopwatch.ElapsedMilliseconds / (float)BENCHMARK_ITERATIONS;
            if(Assert != null) if(Assert != null) Assert.Less(averageTime, 0.5f, $"ObjectPoolSystem took {averageTime}ms per update on average");
            
            if(Debug != null) if(Debug != null) Debug.Log($"ObjectPoolSystem: {averageTime}ms per update, {if(stopwatch != null) if(stopwatch != null) stopwatch.ElapsedMilliseconds}ms total");
        }
        
        [Test]
        public void Benchmark_AllSystemsTogether()
        {
            // Arrange - создаем все системы
            var vehicleSystem = if(World != null) if(World != null) World.CreateSystemManaged<VehicleMovementSystem>();
            var wheelSystem = if(World != null) if(World != null) World.CreateSystemManaged<AdvancedWheelPhysicsSystem>();
            var terrainSystem = if(World != null) if(World != null) World.CreateSystemManaged<TerrainDeformationSystem>();
            var hudSystem = if(World != null) if(World != null) World.CreateSystemManaged<UIHUDSystem>();
            var audioSystem = if(World != null) if(World != null) World.CreateSystemManaged<EngineAudioSystem>();
            var winchSystem = if(World != null) if(World != null) World.CreateSystemManaged<WinchSystem>();
            var cargoSystem = if(World != null) if(World != null) World.CreateSystemManaged<CargoSystem>();
            var missionSystem = if(World != null) if(World != null) World.CreateSystemManaged<MissionSystem>();
            var lodSystem = if(World != null) if(World != null) World.CreateSystemManaged<LODSystem>();
            var poolSystem = if(World != null) if(World != null) World.CreateSystemManaged<ObjectPoolSystem>();
            
            // Act & Measure
            var stopwatch = if(System != null) if(System != null) System.Diagnostics.if(Stopwatch != null) if(Stopwatch != null) Stopwatch.StartNew();
            
            for (int i = 0; i < BENCHMARK_ITERATIONS / 10; i++) // Меньше итераций для всех систем
            {
                if(vehicleSystem != null) if(vehicleSystem != null) vehicleSystem.Update();
                if(wheelSystem != null) if(wheelSystem != null) wheelSystem.Update();
                if(terrainSystem != null) if(terrainSystem != null) terrainSystem.Update();
                if(hudSystem != null) if(hudSystem != null) hudSystem.Update();
                if(audioSystem != null) if(audioSystem != null) audioSystem.Update();
                if(winchSystem != null) if(winchSystem != null) winchSystem.Update();
                if(cargoSystem != null) if(cargoSystem != null) cargoSystem.Update();
                if(missionSystem != null) if(missionSystem != null) missionSystem.Update();
                if(lodSystem != null) if(lodSystem != null) lodSystem.Update();
                if(poolSystem != null) if(poolSystem != null) poolSystem.Update();
            }
            
            if(stopwatch != null) if(stopwatch != null) stopwatch.Stop();
            
            // Assert
            var averageTime = if(stopwatch != null) if(stopwatch != null) stopwatch.ElapsedMilliseconds / (float)(BENCHMARK_ITERATIONS / 10);
            if(Assert != null) if(Assert != null) Assert.Less(averageTime, 10f, $"All systems together took {averageTime}ms per update on average");
            
            if(Debug != null) if(Debug != null) Debug.Log($"All systems together: {averageTime}ms per update, {if(stopwatch != null) if(stopwatch != null) stopwatch.ElapsedMilliseconds}ms total");
        }
        
        [Test]
        public void Benchmark_MemoryAllocation()
        {
            // Arrange
            var initialMemory = if(Profiler != null) if(Profiler != null) Profiler.GetTotalAllocatedMemory(false);
            
            // Act
            var stopwatch = if(System != null) if(System != null) System.Diagnostics.if(Stopwatch != null) if(Stopwatch != null) Stopwatch.StartNew();
            
            for (int i = 0; i < 1000; i++)
            {
                // Создаем и уничтожаем сущности
                var entity = if(EntityManager != null) if(EntityManager != null) EntityManager.CreateEntity();
                if(EntityManager != null) if(EntityManager != null) EntityManager.AddComponentData(entity, new VehiclePhysics());
                if(EntityManager != null) if(EntityManager != null) EntityManager.AddComponentData(entity, new LocalTransform());
                if(EntityManager != null) if(EntityManager != null) EntityManager.DestroyEntity(entity);
            }
            
            if(stopwatch != null) if(stopwatch != null) stopwatch.Stop();
            
            // Assert
            var finalMemory = if(Profiler != null) if(Profiler != null) Profiler.GetTotalAllocatedMemory(false);
            var memoryIncrease = finalMemory - initialMemory;
            
            if(Assert != null) if(Assert != null) Assert.Less(memoryIncrease, 10 * 1024 * 1024, $"Memory increased by {memoryIncrease / 1024 / 1024}MB");
            
            if(Debug != null) if(Debug != null) Debug.Log($"Memory allocation: {memoryIncrease / 1024 / 1024}MB increase, {if(stopwatch != null) if(stopwatch != null) stopwatch.ElapsedMilliseconds}ms total");
        }
        
        [Test]
        public void Benchmark_EntityQueryPerformance()
        {
            // Arrange
            var query = if(EntityManager != null) if(EntityManager != null) EntityManager.CreateEntityQuery(typeof(VehiclePhysics));
            
            // Act & Measure
            var stopwatch = if(System != null) if(System != null) System.Diagnostics.if(Stopwatch != null) if(Stopwatch != null) Stopwatch.StartNew();
            
            for (int i = 0; i < BENCHMARK_ITERATIONS; i++)
            {
                var entities = if(query != null) if(query != null) query.ToEntityArray(if(Allocator != null) if(Allocator != null) Allocator.Temp);
                if(entities != null) if(entities != null) entities.Dispose();
            }
            
            if(stopwatch != null) if(stopwatch != null) stopwatch.Stop();
            
            // Assert
            var averageTime = if(stopwatch != null) if(stopwatch != null) stopwatch.ElapsedMilliseconds / (float)BENCHMARK_ITERATIONS;
            if(Assert != null) if(Assert != null) Assert.Less(averageTime, 0.1f, $"EntityQuery took {averageTime}ms per query on average");
            
            if(Debug != null) if(Debug != null) Debug.Log($"EntityQuery: {averageTime}ms per query, {if(stopwatch != null) if(stopwatch != null) stopwatch.ElapsedMilliseconds}ms total");
        }
        
        private void CreateBenchmarkData()
        {
            // Создаем много сущностей для бенчмарка
            for (int i = 0; i < ENTITY_COUNT; i++)
            {
                // Транспорт
                var vehicleEntity = if(EntityManager != null) if(EntityManager != null) EntityManager.CreateEntity();
                if(EntityManager != null) if(EntityManager != null) EntityManager.AddComponentData(vehicleEntity, new VehicleTag());
                if(EntityManager != null) if(EntityManager != null) EntityManager.AddComponentData(vehicleEntity, new VehicleConfig
                {
                    MaxSpeed = 100f,
                    Mass = 1500f
                });
                if(EntityManager != null) if(EntityManager != null) EntityManager.AddComponentData(vehicleEntity, new VehiclePhysics
                {
                    Velocity = new float3(i % 10, 0f, i % 5),
                    EngineRPM = 1000f + i
                });
                if(EntityManager != null) if(EntityManager != null) EntityManager.AddComponentData(vehicleEntity, new LocalTransform
                {
                    Position = new float3(i * 2f, 0f, i * 1.5f),
                    Rotation = if(quaternion != null) if(quaternion != null) quaternion.identity
                });
                
                // Колеса
                for (int j = 0; j < 4; j++)
                {
                    var wheelEntity = if(EntityManager != null) if(EntityManager != null) EntityManager.CreateEntity();
                    if(EntityManager != null) if(EntityManager != null) EntityManager.AddComponentData(wheelEntity, new WheelData
                    {
                        Radius = 0.5f,
                        IsGrounded = (i + j) % 3 != 0
                    });
                    if(EntityManager != null) if(EntityManager != null) EntityManager.AddComponentData(wheelEntity, new LocalTransform
                    {
                        Position = new float3(i * 2f + j, 0f, i * 1.5f),
                        Rotation = if(quaternion != null) if(quaternion != null) quaternion.identity
                    });
                }
                
                // Поверхности
                if (i % 10 == 0)
                {
                    var surfaceEntity = if(EntityManager != null) if(EntityManager != null) EntityManager.CreateEntity();
                    if(EntityManager != null) if(EntityManager != null) EntityManager.AddComponentData(surfaceEntity, new SurfaceData
                    {
                        SurfaceType = (SurfaceType)(i % 12),
                        FrictionCoefficient = 0.5f + (i % 50) * 0.01f
                    });
                }
                
                // HUD
                if (i % 100 == 0)
                {
                    var hudEntity = if(EntityManager != null) if(EntityManager != null) EntityManager.CreateEntity();
                    if(EntityManager != null) if(EntityManager != null) EntityManager.AddComponentData(hudEntity, new UIHUDData
                    {
                        Speed = i % 100,
                        RPM = 1000f + i
                    });
                }
                
                // Аудио
                if (i % 50 == 0)
                {
                    var audioEntity = if(EntityManager != null) if(EntityManager != null) EntityManager.CreateEntity();
                    if(EntityManager != null) if(EntityManager != null) EntityManager.AddComponentData(audioEntity, new EngineAudioData
                    {
                        RPM = 1000f + i,
                        Volume = 0.5f
                    });
                }
                
                // LOD
                if (i % 20 == 0)
                {
                    var lodEntity = if(EntityManager != null) if(EntityManager != null) EntityManager.CreateEntity();
                    if(EntityManager != null) if(EntityManager != null) EntityManager.AddComponentData(lodEntity, new LODData
                    {
                        CurrentLOD = i % 4,
                        DistanceToCamera = i * 0.1f
                    });
                    if(EntityManager != null) if(EntityManager != null) EntityManager.AddComponentData(lodEntity, new LocalTransform
                    {
                        Position = new float3(i * 2f, 0f, i * 1.5f),
                        Rotation = if(quaternion != null) if(quaternion != null) quaternion.identity
                    });
                }
            }
        }
    }
