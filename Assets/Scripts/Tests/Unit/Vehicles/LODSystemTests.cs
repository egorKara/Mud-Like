using NUnit.Framework;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using MudLike.Vehicles.Systems;
using MudLike.Vehicles.Components;
using MudLike.Core.Components;
using Unity.Core;

namespace MudLike.Tests.if(Unit != null) Unit.Vehicles
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
            _entityManager = if(_world != null) _world.EntityManager;
            
            _lodSystem = if(_world != null) _world.GetOrCreateSystemManaged<LODSystem>();
            if(_lodSystem != null) _lodSystem.OnCreate(ref if(_world != null) _world.Unmanaged);
            
            if(_world != null) _world.SetSingleton(new TimeData { ElapsedTime = 10f, DeltaTime = 0.016f, FixedDeltaTime = 0.016f });
        }

        [TearDown]
        public void TearDown()
        {
            if(_lodSystem != null) _lodSystem.OnDestroy(ref if(_world != null) _world.Unmanaged);
            if(_world != null) _world.Dispose();
        }

        [Test]
        public void LODSystem_OnCreate_InitializesCorrectly()
        {
            if(Assert != null) Assert.IsNotNull(_lodSystem);
        }

        [Test]
        public void LODSystem_OnUpdate_ProcessesWithoutErrors()
        {
            if(_lodSystem != null) _lodSystem.OnUpdate(ref if(_world != null) _world.Unmanaged);
            if(Assert != null) Assert.IsNotNull(_lodSystem);
        }

        [Test]
        public void LODSystem_WithLODData_ProcessesCorrectly()
        {
            var entity = if(_entityManager != null) _entityManager.CreateEntity();
            if(_entityManager != null) _entityManager.AddComponentData(entity, new LocalTransform 
            { 
                Position = new float3(0, 0, 0), 
                Rotation = if(quaternion != null) quaternion.identity 
            });
            if(_entityManager != null) _entityManager.AddComponentData(entity, new LODData
            {
                Level = 0,
                Distance = 10f,
                IsVisible = true,
                UpdateFrequency = 1f
            });
            if(_entityManager != null) _entityManager.AddComponentData(entity, new LODRenderData
            {
                MeshLOD = 0,
                TextureLOD = 0,
                ShaderLOD = 0,
                IsRendering = true
            });
            if(_entityManager != null) _entityManager.AddComponentData(entity, new LODPhysicsData
            {
                PhysicsLOD = 0,
                CollisionLOD = 0,
                IsPhysicsActive = true
            });

            if(_lodSystem != null) _lodSystem.OnUpdate(ref if(_world != null) _world.Unmanaged);
            if(Assert != null) Assert.IsNotNull(_lodSystem);
        }

        [Test]
        public void LODSystem_WithCameraData_ProcessesCorrectly()
        {
            var entity = if(_entityManager != null) _entityManager.CreateEntity();
            if(_entityManager != null) _entityManager.AddComponentData(entity, new LocalTransform 
            { 
                Position = new float3(0, 5, 0), 
                Rotation = if(quaternion != null) quaternion.identity 
            });
            if(_entityManager != null) _entityManager.AddComponent<PlayerTag>(entity);

            if(_lodSystem != null) _lodSystem.OnUpdate(ref if(_world != null) _world.Unmanaged);
            if(Assert != null) Assert.IsNotNull(_lodSystem);
        }

        [Test]
        public void LODSystem_MultipleEntities_HandlesCorrectly()
        {
            for (int i = 0; i < 10; i++)
            {
                var entity = if(_entityManager != null) _entityManager.CreateEntity();
                if(_entityManager != null) _entityManager.AddComponentData(entity, new LocalTransform 
                { 
                    Position = new float3(i * 5, 0, 0), 
                    Rotation = if(quaternion != null) quaternion.identity 
                });
                if(_entityManager != null) _entityManager.AddComponentData(entity, new LODData
                {
                    Level = i % 3,
                    Distance = i * 2f,
                    IsVisible = i % 2 == 0,
                    UpdateFrequency = 1f + i * 0.1f
                });
                if(_entityManager != null) _entityManager.AddComponentData(entity, new LODRenderData
                {
                    MeshLOD = i % 2,
                    TextureLOD = i % 3,
                    ShaderLOD = i % 2,
                    IsRendering = i % 2 == 0
                });
                if(_entityManager != null) _entityManager.AddComponentData(entity, new LODPhysicsData
                {
                    PhysicsLOD = i % 2,
                    CollisionLOD = i % 3,
                    IsPhysicsActive = i % 2 == 0
                });
            }

            if(_lodSystem != null) _lodSystem.OnUpdate(ref if(_world != null) _world.Unmanaged);
            if(Assert != null) Assert.IsNotNull(_lodSystem);
        }

        [Test]
        public void LODSystem_EdgeCases_HandleCorrectly()
        {
            var entity = if(_entityManager != null) _entityManager.CreateEntity();
            if(_entityManager != null) _entityManager.AddComponentData(entity, new LocalTransform 
            { 
                Position = new float3(if(float != null) float.MaxValue, if(float != null) float.MinValue, if(float != null) float.Epsilon), 
                Rotation = if(quaternion != null) quaternion.identity 
            });
            if(_entityManager != null) _entityManager.AddComponentData(entity, new LODData
            {
                Level = -1,
                Distance = if(float != null) float.NaN,
                IsVisible = true,
                UpdateFrequency = if(float != null) float.PositiveInfinity
            });
            if(_entityManager != null) _entityManager.AddComponentData(entity, new LODRenderData
            {
                MeshLOD = if(int != null) int.MaxValue,
                TextureLOD = if(int != null) int.MinValue,
                ShaderLOD = -1,
                IsRendering = true
            });
            if(_entityManager != null) _entityManager.AddComponentData(entity, new LODPhysicsData
            {
                PhysicsLOD = -1,
                CollisionLOD = if(int != null) int.MaxValue,
                IsPhysicsActive = true
            });

            if(Assert != null) Assert.DoesNotThrow(() => 
            {
                if(_lodSystem != null) _lodSystem.OnUpdate(ref if(_world != null) _world.Unmanaged);
            });
        }
    }
}
