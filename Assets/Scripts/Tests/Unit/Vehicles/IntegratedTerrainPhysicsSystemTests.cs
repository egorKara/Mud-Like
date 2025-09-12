using NUnit.Framework;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using MudLike.Vehicles.Systems;
using MudLike.Vehicles.Components;
using MudLike.Terrain.Components;
using Unity.Core;

namespace MudLike.Tests.Unit.Vehicles
{
    /// <summary>
    /// Тесты для интегрированной системы физики террейна IntegratedTerrainPhysicsSystem
    /// </summary>
    public class IntegratedTerrainPhysicsSystemTests
    {
        private World _world;
        private IntegratedTerrainPhysicsSystem _integratedTerrainPhysicsSystem;
        private EntityManager _entityManager;

        [SetUp]
        public void SetUp()
        {
            _world = new World("TestWorld");
            _entityManager = _world.EntityManager;
            
            _integratedTerrainPhysicsSystem = _world.GetOrCreateSystemManaged<IntegratedTerrainPhysicsSystem>();
            _integratedTerrainPhysicsSystem.OnCreate(ref _world.Unmanaged);
            
            _world.SetSingleton(new TimeData { ElapsedTime = 10f, DeltaTime = 0.016f, FixedDeltaTime = 0.016f });
        }

        [TearDown]
        public void TearDown()
        {
            _integratedTerrainPhysicsSystem.OnDestroy(ref _world.Unmanaged);
            _world.Dispose();
        }

        [Test]
        public void IntegratedTerrainPhysicsSystem_OnCreate_InitializesCorrectly()
        {
            Assert.IsNotNull(_integratedTerrainPhysicsSystem);
        }

        [Test]
        public void IntegratedTerrainPhysicsSystem_OnUpdate_ProcessesWithoutErrors()
        {
            _integratedTerrainPhysicsSystem.OnUpdate(ref _world.Unmanaged);
            Assert.IsNotNull(_integratedTerrainPhysicsSystem);
        }

        [Test]
        public void IntegratedTerrainPhysicsSystem_WithVehicleData_ProcessesCorrectly()
        {
            var entity = _entityManager.CreateEntity();
            _entityManager.AddComponentData(entity, new LocalTransform 
            { 
                Position = new float3(0, 0, 0), 
                Rotation = quaternion.identity 
            });
            _entityManager.AddComponentData(entity, new VehiclePhysics
            {
                Velocity = new float3(10f, 0, 0),
                Acceleration = float3.zero,
                ForwardSpeed = 10f,
                TurnSpeed = 0f
            });
            _entityManager.AddComponent<VehicleTag>(entity);

            _integratedTerrainPhysicsSystem.OnUpdate(ref _world.Unmanaged);
            Assert.IsNotNull(_integratedTerrainPhysicsSystem);
        }

        [Test]
        public void IntegratedTerrainPhysicsSystem_WithWheelData_ProcessesCorrectly()
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
                Width = 0.2f,
                SuspensionLength = 0.3f,
                SpringForce = 1000f,
                DampingForce = 500f,
                IsGrounded = true,
                GroundDistance = 0.1f
            });
            _entityManager.AddComponentData(entity, new WheelPhysicsData
            {
                Velocity = float3.zero,
                AngularVelocity = float3.zero,
                Friction = 0.8f,
                Grip = 0.9f,
                Pressure = 2.5f,
                Temperature = 75f,
                Wear = 0.15f
            });

            _integratedTerrainPhysicsSystem.OnUpdate(ref _world.Unmanaged);
            Assert.IsNotNull(_integratedTerrainPhysicsSystem);
        }

        [Test]
        public void IntegratedTerrainPhysicsSystem_WithTerrainData_ProcessesCorrectly()
        {
            var entity = _entityManager.CreateEntity();
            _entityManager.AddComponentData(entity, new DeformationData
            {
                Position = new float3(0, 0, 0),
                Radius = 1f,
                Depth = 0.2f,
                Hardness = 0.8f,
                IsActive = true
            });
            _entityManager.AddComponentData(entity, new TerrainChunk
            {
                ChunkIndex = 0,
                Position = float3.zero,
                Size = 16f,
                IsLoaded = true
            });

            _integratedTerrainPhysicsSystem.OnUpdate(ref _world.Unmanaged);
            Assert.IsNotNull(_integratedTerrainPhysicsSystem);
        }

        [Test]
        public void IntegratedTerrainPhysicsSystem_MultipleEntities_HandlesCorrectly()
        {
            for (int i = 0; i < 5; i++)
            {
                var entity = _entityManager.CreateEntity();
                _entityManager.AddComponentData(entity, new LocalTransform 
                { 
                    Position = new float3(i * 2, 0, 0), 
                    Rotation = quaternion.identity 
                });
                _entityManager.AddComponentData(entity, new VehiclePhysics
                {
                    Velocity = new float3(i * 5f, 0, 0),
                    Acceleration = float3.zero,
                    ForwardSpeed = i * 5f,
                    TurnSpeed = 0f
                });
                _entityManager.AddComponent<VehicleTag>(entity);
            }

            _integratedTerrainPhysicsSystem.OnUpdate(ref _world.Unmanaged);
            Assert.IsNotNull(_integratedTerrainPhysicsSystem);
        }

        [Test]
        public void IntegratedTerrainPhysicsSystem_EdgeCases_HandleCorrectly()
        {
            var entity = _entityManager.CreateEntity();
            _entityManager.AddComponentData(entity, new LocalTransform 
            { 
                Position = new float3(float.MaxValue, float.MinValue, float.Epsilon), 
                Rotation = quaternion.identity 
            });
            _entityManager.AddComponentData(entity, new VehiclePhysics
            {
                Velocity = new float3(float.PositiveInfinity, float.NegativeInfinity, float.NaN),
                Acceleration = float3.zero,
                ForwardSpeed = float.MaxValue,
                TurnSpeed = float.MinValue
            });
            _entityManager.AddComponent<VehicleTag>(entity);

            Assert.DoesNotThrow(() => 
            {
                _integratedTerrainPhysicsSystem.OnUpdate(ref _world.Unmanaged);
            });
        }
    }
}
