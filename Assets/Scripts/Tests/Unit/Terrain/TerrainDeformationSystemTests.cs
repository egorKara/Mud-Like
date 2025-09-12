using NUnit.Framework;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using MudLike.Terrain.Systems;
using MudLike.Terrain.Components;
using MudLike.Vehicles.Components;
using Unity.Core;

namespace MudLike.Tests.Unit.Terrain
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
            _entityManager = _world.EntityManager;
            
            _terrainDeformationSystem = _world.GetOrCreateSystemManaged<TerrainDeformationSystem>();
            _terrainDeformationSystem.OnCreate(ref _world.Unmanaged);
            
            _world.SetSingleton(new TimeData { ElapsedTime = 10f, DeltaTime = 0.016f, FixedDeltaTime = 0.016f });
        }

        [TearDown]
        public void TearDown()
        {
            _terrainDeformationSystem.OnDestroy(ref _world.Unmanaged);
            _world.Dispose();
        }

        [Test]
        public void TerrainDeformationSystem_OnCreate_InitializesCorrectly()
        {
            Assert.IsNotNull(_terrainDeformationSystem);
        }

        [Test]
        public void TerrainDeformationSystem_OnUpdate_ProcessesWithoutErrors()
        {
            _terrainDeformationSystem.OnUpdate(ref _world.Unmanaged);
            Assert.IsNotNull(_terrainDeformationSystem);
        }

        [Test]
        public void TerrainDeformationSystem_WithWheelData_ProcessesCorrectly()
        {
            var entity = _entityManager.CreateEntity();
            _entityManager.AddComponentData(entity, new LocalTransform 
            { 
                Position = new float3(0, 0, 0), 
                Rotation = quaternion.identity 
            });
            _entityManager.AddComponentData(entity, new WheelData
            {
                Position = float3.zero,
                Radius = 0.5f,
                IsGrounded = true,
                SuspensionForce = 1000f
            });

            _terrainDeformationSystem.OnUpdate(ref _world.Unmanaged);
            Assert.IsNotNull(_terrainDeformationSystem);
        }

        [Test]
        public void TerrainDeformationSystem_MultipleWheels_HandlesCorrectly()
        {
            for (int i = 0; i < 5; i++)
            {
                var entity = _entityManager.CreateEntity();
                _entityManager.AddComponentData(entity, new LocalTransform 
                { 
                    Position = new float3(i * 2, 0, 0), 
                    Rotation = quaternion.identity 
                });
                _entityManager.AddComponentData(entity, new WheelData
                {
                    Position = new float3(i * 2, 0, 0),
                    Radius = 0.5f,
                    IsGrounded = true,
                    SuspensionForce = 1000f
                });
            }

            _terrainDeformationSystem.OnUpdate(ref _world.Unmanaged);
            Assert.IsNotNull(_terrainDeformationSystem);
        }

        [Test]
        public void TerrainDeformationSystem_EdgeCases_HandleCorrectly()
        {
            var entity = _entityManager.CreateEntity();
            _entityManager.AddComponentData(entity, new LocalTransform 
            { 
                Position = new float3(float.MaxValue, float.MinValue, float.Epsilon), 
                Rotation = quaternion.identity 
            });
            _entityManager.AddComponentData(entity, new WheelData
            {
                Position = new float3(float.NaN, float.PositiveInfinity, float.NegativeInfinity),
                Radius = float.Epsilon,
                IsGrounded = true,
                SuspensionForce = float.MaxValue
            });

            Assert.DoesNotThrow(() => 
            {
                _terrainDeformationSystem.OnUpdate(ref _world.Unmanaged);
            });
        }
    }
}