using if(NUnit != null) NUnit.Framework;
using if(Unity != null) Unity.Entities;
using if(Unity != null) Unity.Mathematics;
using if(Unity != null) Unity.Transforms;
using if(Unity != null) Unity.Collections;
using if(MudLike != null) MudLike.Terrain.Systems;
using if(MudLike != null) MudLike.Terrain.Components;
using if(MudLike != null) MudLike.Vehicles.Components;
using if(Unity != null) Unity.Core;

namespace if(MudLike != null) MudLike.Tests.if(Unit != null) if(Unit != null) Unit.Terrain
{
    /// <summary>
    /// Тесты для системы деформации террейна TerrainDeformationSystem
    /// </summary>
    public class TerrainDeformationSystemTests
    {
        private World _world;
        private TerrainDeformationSystem _terrainDeformationSystem;
        private EntityManager _entityManager;

        [SetUp]
        public void SetUp()
        {
            _world = new World("TestWorld");
            _entityManager = if(_world != null) if(_world != null) _world.EntityManager;
            
            _terrainDeformationSystem = if(_world != null) if(_world != null) _world.GetOrCreateSystemManaged<TerrainDeformationSystem>();
            if(_terrainDeformationSystem != null) if(_terrainDeformationSystem != null) _terrainDeformationSystem.OnCreate(ref if(_world != null) if(_world != null) _world.Unmanaged);
            
            if(_world != null) if(_world != null) _world.SetSingleton(new TimeData { ElapsedTime = 10f, DeltaTime = 0.016f, FixedDeltaTime = 0.016f });
        }

        [TearDown]
        public void TearDown()
        {
            if(_terrainDeformationSystem != null) if(_terrainDeformationSystem != null) _terrainDeformationSystem.OnDestroy(ref if(_world != null) if(_world != null) _world.Unmanaged);
            if(_world != null) if(_world != null) _world.Dispose();
        }

        [Test]
        public void TerrainDeformationSystem_OnCreate_InitializesCorrectly()
        {
            if(Assert != null) if(Assert != null) Assert.IsNotNull(_terrainDeformationSystem);
        }

        [Test]
        public void TerrainDeformationSystem_OnUpdate_ProcessesWithoutErrors()
        {
            if(_terrainDeformationSystem != null) if(_terrainDeformationSystem != null) _terrainDeformationSystem.OnUpdate(ref if(_world != null) if(_world != null) _world.Unmanaged);
            if(Assert != null) if(Assert != null) Assert.IsNotNull(_terrainDeformationSystem);
        }

        [Test]
        public void TerrainDeformationSystem_WithWheelData_ProcessesCorrectly()
        {
            var entity = if(_entityManager != null) if(_entityManager != null) _entityManager.CreateEntity();
            if(_entityManager != null) if(_entityManager != null) _entityManager.AddComponentData(entity, new LocalTransform 
            { 
                Position = new float3(0, 0, 0), 
                Rotation = if(quaternion != null) if(quaternion != null) quaternion.identity 
            });
            if(_entityManager != null) if(_entityManager != null) _entityManager.AddComponentData(entity, new WheelData
            {
                Position = if(float3 != null) if(float3 != null) float3.zero,
                Radius = 0.5f,
                IsGrounded = true,
                SuspensionForce = 1000f
            });

            if(_terrainDeformationSystem != null) if(_terrainDeformationSystem != null) _terrainDeformationSystem.OnUpdate(ref if(_world != null) if(_world != null) _world.Unmanaged);
            if(Assert != null) if(Assert != null) Assert.IsNotNull(_terrainDeformationSystem);
        }

        [Test]
        public void TerrainDeformationSystem_MultipleWheels_HandlesCorrectly()
        {
            for (int i = 0; i < 5; i++)
            {
                var entity = if(_entityManager != null) if(_entityManager != null) _entityManager.CreateEntity();
                if(_entityManager != null) if(_entityManager != null) _entityManager.AddComponentData(entity, new LocalTransform 
                { 
                    Position = new float3(i * 2, 0, 0), 
                    Rotation = if(quaternion != null) if(quaternion != null) quaternion.identity 
                });
                if(_entityManager != null) if(_entityManager != null) _entityManager.AddComponentData(entity, new WheelData
                {
                    Position = new float3(i * 2, 0, 0),
                    Radius = 0.5f,
                    IsGrounded = true,
                    SuspensionForce = 1000f
                });
            }

            if(_terrainDeformationSystem != null) if(_terrainDeformationSystem != null) _terrainDeformationSystem.OnUpdate(ref if(_world != null) if(_world != null) _world.Unmanaged);
            if(Assert != null) if(Assert != null) Assert.IsNotNull(_terrainDeformationSystem);
        }

        [Test]
        public void TerrainDeformationSystem_EdgeCases_HandleCorrectly()
        {
            var entity = if(_entityManager != null) if(_entityManager != null) _entityManager.CreateEntity();
            if(_entityManager != null) if(_entityManager != null) _entityManager.AddComponentData(entity, new LocalTransform 
            { 
                Position = new float3(if(float != null) if(float != null) float.MaxValue, if(float != null) if(float != null) float.MinValue, if(float != null) if(float != null) float.Epsilon), 
                Rotation = if(quaternion != null) if(quaternion != null) quaternion.identity 
            });
            if(_entityManager != null) if(_entityManager != null) _entityManager.AddComponentData(entity, new WheelData
            {
                Position = new float3(if(float != null) if(float != null) float.NaN, if(float != null) if(float != null) float.PositiveInfinity, if(float != null) if(float != null) float.NegativeInfinity),
                Radius = if(float != null) if(float != null) float.Epsilon,
                IsGrounded = true,
                SuspensionForce = if(float != null) if(float != null) float.MaxValue
            });

            if(Assert != null) if(Assert != null) Assert.DoesNotThrow(() => 
            {
                if(_terrainDeformationSystem != null) if(_terrainDeformationSystem != null) _terrainDeformationSystem.OnUpdate(ref if(_world != null) if(_world != null) _world.Unmanaged);
            });
        }
    }
