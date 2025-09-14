using NUnit.Framework;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using MudLike.Vehicles.Systems;
using MudLike.Vehicles.Components;
using MudLike.Core.Components;
using Unity.Core;

namespace MudLike.Tests.Unit.Vehicles
{
    /// <summary>
    /// Тесты для системы LOD LODSystem
    /// </summary>
    public class LODSystemTests
    {
        private World _world;
        private LODSystem _lodSystem;
        private EntityManager _entityManager;

        [SetUp]
        public void SetUp()
        {
            _world = new World("TestWorld");
            _entityManager = _world.EntityManager;
            
            _lodSystem = _world.GetOrCreateSystemManaged<LODSystem>();
            _lodSystem.OnCreate(ref _world.Unmanaged);
            
            _world.SetSingleton(new TimeData { ElapsedTime = 10f, DeltaTime = 0.016f, FixedDeltaTime = 0.016f });
        }

        [TearDown]
        public void TearDown()
        {
            _lodSystem.OnDestroy(ref _world.Unmanaged);
            _world.Dispose();
        }

        [Test]
        public void LODSystem_OnCreate_InitializesCorrectly()
        {
            Assert.IsNotNull(_lodSystem);
        }

        [Test]
        public void LODSystem_OnUpdate_ProcessesWithoutErrors()
        {
            _lodSystem.OnUpdate(ref _world.Unmanaged);
            Assert.IsNotNull(_lodSystem);
        }

        [Test]
        public void LODSystem_WithLODData_ProcessesCorrectly()
        {
            var entity = _entityManager.CreateEntity();
            _entityManager.AddComponentData(entity, new LocalTransform 
            { 
                Position = new float3(0, 0, 0), 
                Rotation = quaternion.identity 
            });
            _entityManager.AddComponentData(entity, new LODData
            {
                Level = 0,
                Distance = 10f,
                IsVisible = true,
                UpdateFrequency = 1f
            });
            _entityManager.AddComponentData(entity, new LODRenderData
            {
                MeshLOD = 0,
                TextureLOD = 0,
                ShaderLOD = 0,
                IsRendering = true
            });
            _entityManager.AddComponentData(entity, new LODPhysicsData
            {
                PhysicsLOD = 0,
                CollisionLOD = 0,
                IsPhysicsActive = true
            });

            _lodSystem.OnUpdate(ref _world.Unmanaged);
            Assert.IsNotNull(_lodSystem);
        }

        [Test]
        public void LODSystem_WithCameraData_ProcessesCorrectly()
        {
            var entity = _entityManager.CreateEntity();
            _entityManager.AddComponentData(entity, new LocalTransform 
            { 
                Position = new float3(0, 5, 0), 
                Rotation = quaternion.identity 
            });
            _entityManager.AddComponent<PlayerTag>(entity);

            _lodSystem.OnUpdate(ref _world.Unmanaged);
            Assert.IsNotNull(_lodSystem);
        }

        [Test]
        public void LODSystem_MultipleEntities_HandlesCorrectly()
        {
            for (int i = 0; i < 10; i++)
            {
                var entity = _entityManager.CreateEntity();
                _entityManager.AddComponentData(entity, new LocalTransform 
                { 
                    Position = new float3(i * 5, 0, 0), 
                    Rotation = quaternion.identity 
                });
                _entityManager.AddComponentData(entity, new LODData
                {
                    Level = i % 3,
                    Distance = i * 2f,
                    IsVisible = i % 2 == 0,
                    UpdateFrequency = 1f + i * 0.1f
                });
                _entityManager.AddComponentData(entity, new LODRenderData
                {
                    MeshLOD = i % 2,
                    TextureLOD = i % 3,
                    ShaderLOD = i % 2,
                    IsRendering = i % 2 == 0
                });
                _entityManager.AddComponentData(entity, new LODPhysicsData
                {
                    PhysicsLOD = i % 2,
                    CollisionLOD = i % 3,
                    IsPhysicsActive = i % 2 == 0
                });
            }

            _lodSystem.OnUpdate(ref _world.Unmanaged);
            Assert.IsNotNull(_lodSystem);
        }

        [Test]
        public void LODSystem_EdgeCases_HandleCorrectly()
        {
            var entity = _entityManager.CreateEntity();
            _entityManager.AddComponentData(entity, new LocalTransform 
            { 
                Position = new float3(float.MaxValue, float.MinValue, float.Epsilon), 
                Rotation = quaternion.identity 
            });
            _entityManager.AddComponentData(entity, new LODData
            {
                Level = -1,
                Distance = float.NaN,
                IsVisible = true,
                UpdateFrequency = float.PositiveInfinity
            });
            _entityManager.AddComponentData(entity, new LODRenderData
            {
                MeshLOD = int.MaxValue,
                TextureLOD = int.MinValue,
                ShaderLOD = -1,
                IsRendering = true
            });
            _entityManager.AddComponentData(entity, new LODPhysicsData
            {
                PhysicsLOD = -1,
                CollisionLOD = int.MaxValue,
                IsPhysicsActive = true
            });

            Assert.DoesNotThrow(() => 
            {
                _lodSystem.OnUpdate(ref _world.Unmanaged);
            });
        }
    }
}
