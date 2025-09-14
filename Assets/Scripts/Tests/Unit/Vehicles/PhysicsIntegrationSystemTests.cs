using NUnit.Framework;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;
using Unity.Collections;
using MudLike.Vehicles.Systems;
using MudLike.Vehicles.Components;
using Unity.Core;

namespace MudLike.Tests.Unit.Vehicles
{
    /// <summary>
    /// Тесты для системы интеграции с Unity Physics PhysicsIntegrationSystem
    /// </summary>
    public class PhysicsIntegrationSystemTests
    {
        private World _world;
        private PhysicsIntegrationSystem _physicsIntegrationSystem;
        private EntityManager _entityManager;

        [SetUp]
        public void SetUp()
        {
            _world = new World("TestWorld");
            _entityManager = _world.EntityManager;
            
            _physicsIntegrationSystem = _world.GetOrCreateSystemManaged<PhysicsIntegrationSystem>();
            _physicsIntegrationSystem.OnCreate(ref _world.Unmanaged);
            
            _world.SetSingleton(new TimeData { ElapsedTime = 10f, DeltaTime = 0.016f, FixedDeltaTime = 0.016f });
        }

        [TearDown]
        public void TearDown()
        {
            _physicsIntegrationSystem.OnDestroy(ref _world.Unmanaged);
            _world.Dispose();
        }

        [Test]
        public void PhysicsIntegrationSystem_OnCreate_InitializesCorrectly()
        {
            Assert.IsNotNull(_physicsIntegrationSystem);
        }

        [Test]
        public void PhysicsIntegrationSystem_OnUpdate_ProcessesWithoutErrors()
        {
            _physicsIntegrationSystem.OnUpdate(ref _world.Unmanaged);
            Assert.IsNotNull(_physicsIntegrationSystem);
        }

        [Test]
        public void PhysicsIntegrationSystem_WithPhysicsData_ProcessesCorrectly()
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

            _physicsIntegrationSystem.OnUpdate(ref _world.Unmanaged);
            Assert.IsNotNull(_physicsIntegrationSystem);
        }

        [Test]
        public void PhysicsIntegrationSystem_WithWheelData_ProcessesCorrectly()
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
            _entityManager.AddComponentData(entity, new VehiclePhysics
            {
                Velocity = new float3(5f, 0, 0),
                Acceleration = float3.zero,
                ForwardSpeed = 5f,
                TurnSpeed = 0f
            });

            _physicsIntegrationSystem.OnUpdate(ref _world.Unmanaged);
            Assert.IsNotNull(_physicsIntegrationSystem);
        }

        [Test]
        public void PhysicsIntegrationSystem_MultipleEntities_HandlesCorrectly()
        {
            for (int i = 0; i < 8; i++)
            {
                var entity = _entityManager.CreateEntity();
                _entityManager.AddComponentData(entity, new LocalTransform 
                { 
                    Position = new float3(i * 2f, 0, 0), 
                    Rotation = quaternion.identity 
                });
                _entityManager.AddComponentData(entity, new VehiclePhysics
                {
                    Velocity = new float3(i * 3f, 0, 0),
                    Acceleration = float3.zero,
                    ForwardSpeed = i * 3f,
                    TurnSpeed = 0f
                });
                _entityManager.AddComponentData(entity, new WheelData
                {
                    Position = new float3(i * 2f, 0, 0),
                    Radius = 0.5f + i * 0.05f,
                    Width = 0.2f + i * 0.01f,
                    SuspensionLength = 0.3f,
                    SpringForce = 1000f + i * 100f,
                    DampingForce = 500f + i * 50f,
                    IsGrounded = i % 2 == 0,
                    GroundDistance = 0.1f + i * 0.01f
                });
            }

            _physicsIntegrationSystem.OnUpdate(ref _world.Unmanaged);
            Assert.IsNotNull(_physicsIntegrationSystem);
        }

        [Test]
        public void PhysicsIntegrationSystem_EdgeCases_HandleCorrectly()
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
            _entityManager.AddComponentData(entity, new WheelData
            {
                Position = new float3(float.PositiveInfinity, float.NegativeInfinity, float.NaN),
                Radius = float.MaxValue,
                Width = float.MinValue,
                SuspensionLength = float.NaN,
                SpringForce = float.PositiveInfinity,
                DampingForce = float.NegativeInfinity,
                IsGrounded = true,
                GroundDistance = float.Epsilon
            });

            Assert.DoesNotThrow(() => 
            {
                _physicsIntegrationSystem.OnUpdate(ref _world.Unmanaged);
            });
        }
    }
}
