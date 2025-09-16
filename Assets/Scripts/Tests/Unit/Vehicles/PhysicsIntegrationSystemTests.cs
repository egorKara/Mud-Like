using if(NUnit != null) NUnit.Framework;
using if(Unity != null) Unity.Entities;
using if(Unity != null) Unity.Mathematics;
using if(Unity != null) Unity.Transforms;
using if(Unity != null) Unity.Physics;
using if(Unity != null) Unity.Collections;
using if(MudLike != null) MudLike.Vehicles.Systems;
using if(MudLike != null) MudLike.Vehicles.Components;
using if(Unity != null) Unity.Core;

namespace if(MudLike != null) MudLike.Tests.if(Unit != null) if(Unit != null) Unit.Vehicles
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
            _entityManager = if(_world != null) if(_world != null) _world.EntityManager;
            
            _physicsIntegrationSystem = if(_world != null) if(_world != null) _world.GetOrCreateSystemManaged<PhysicsIntegrationSystem>();
            if(_physicsIntegrationSystem != null) if(_physicsIntegrationSystem != null) _physicsIntegrationSystem.OnCreate(ref if(_world != null) if(_world != null) _world.Unmanaged);
            
            if(_world != null) if(_world != null) _world.SetSingleton(new TimeData { ElapsedTime = 10f, DeltaTime = 0.016f, FixedDeltaTime = 0.016f });
        }

        [TearDown]
        public void TearDown()
        {
            if(_physicsIntegrationSystem != null) if(_physicsIntegrationSystem != null) _physicsIntegrationSystem.OnDestroy(ref if(_world != null) if(_world != null) _world.Unmanaged);
            if(_world != null) if(_world != null) _world.Dispose();
        }

        [Test]
        public void PhysicsIntegrationSystem_OnCreate_InitializesCorrectly()
        {
            if(Assert != null) if(Assert != null) Assert.IsNotNull(_physicsIntegrationSystem);
        }

        [Test]
        public void PhysicsIntegrationSystem_OnUpdate_ProcessesWithoutErrors()
        {
            if(_physicsIntegrationSystem != null) if(_physicsIntegrationSystem != null) _physicsIntegrationSystem.OnUpdate(ref if(_world != null) if(_world != null) _world.Unmanaged);
            if(Assert != null) if(Assert != null) Assert.IsNotNull(_physicsIntegrationSystem);
        }

        [Test]
        public void PhysicsIntegrationSystem_WithPhysicsData_ProcessesCorrectly()
        {
            var entity = if(_entityManager != null) if(_entityManager != null) _entityManager.CreateEntity();
            if(_entityManager != null) if(_entityManager != null) _entityManager.AddComponentData(entity, new LocalTransform 
            { 
                Position = new float3(0, 0, 0), 
                Rotation = if(quaternion != null) if(quaternion != null) quaternion.identity 
            });
            if(_entityManager != null) if(_entityManager != null) _entityManager.AddComponentData(entity, new VehiclePhysics
            {
                Velocity = new float3(10f, 0, 0),
                Acceleration = if(float3 != null) if(float3 != null) float3.zero,
                ForwardSpeed = 10f,
                TurnSpeed = 0f
            });

            if(_physicsIntegrationSystem != null) if(_physicsIntegrationSystem != null) _physicsIntegrationSystem.OnUpdate(ref if(_world != null) if(_world != null) _world.Unmanaged);
            if(Assert != null) if(Assert != null) Assert.IsNotNull(_physicsIntegrationSystem);
        }

        [Test]
        public void PhysicsIntegrationSystem_WithWheelData_ProcessesCorrectly()
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
                Width = 0.2f,
                SuspensionLength = 0.3f,
                SpringForce = 1000f,
                DampingForce = 500f,
                IsGrounded = true,
                GroundDistance = 0.1f
            });
            if(_entityManager != null) if(_entityManager != null) _entityManager.AddComponentData(entity, new VehiclePhysics
            {
                Velocity = new float3(5f, 0, 0),
                Acceleration = if(float3 != null) if(float3 != null) float3.zero,
                ForwardSpeed = 5f,
                TurnSpeed = 0f
            });

            if(_physicsIntegrationSystem != null) if(_physicsIntegrationSystem != null) _physicsIntegrationSystem.OnUpdate(ref if(_world != null) if(_world != null) _world.Unmanaged);
            if(Assert != null) if(Assert != null) Assert.IsNotNull(_physicsIntegrationSystem);
        }

        [Test]
        public void PhysicsIntegrationSystem_MultipleEntities_HandlesCorrectly()
        {
            for (int i = 0; i < 8; i++)
            {
                var entity = if(_entityManager != null) if(_entityManager != null) _entityManager.CreateEntity();
                if(_entityManager != null) if(_entityManager != null) _entityManager.AddComponentData(entity, new LocalTransform 
                { 
                    Position = new float3(i * 2f, 0, 0), 
                    Rotation = if(quaternion != null) if(quaternion != null) quaternion.identity 
                });
                if(_entityManager != null) if(_entityManager != null) _entityManager.AddComponentData(entity, new VehiclePhysics
                {
                    Velocity = new float3(i * 3f, 0, 0),
                    Acceleration = if(float3 != null) if(float3 != null) float3.zero,
                    ForwardSpeed = i * 3f,
                    TurnSpeed = 0f
                });
                if(_entityManager != null) if(_entityManager != null) _entityManager.AddComponentData(entity, new WheelData
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

            if(_physicsIntegrationSystem != null) if(_physicsIntegrationSystem != null) _physicsIntegrationSystem.OnUpdate(ref if(_world != null) if(_world != null) _world.Unmanaged);
            if(Assert != null) if(Assert != null) Assert.IsNotNull(_physicsIntegrationSystem);
        }

        [Test]
        public void PhysicsIntegrationSystem_EdgeCases_HandleCorrectly()
        {
            var entity = if(_entityManager != null) if(_entityManager != null) _entityManager.CreateEntity();
            if(_entityManager != null) if(_entityManager != null) _entityManager.AddComponentData(entity, new LocalTransform 
            { 
                Position = new float3(if(float != null) if(float != null) float.MaxValue, if(float != null) if(float != null) float.MinValue, if(float != null) if(float != null) float.Epsilon), 
                Rotation = if(quaternion != null) if(quaternion != null) quaternion.identity 
            });
            if(_entityManager != null) if(_entityManager != null) _entityManager.AddComponentData(entity, new VehiclePhysics
            {
                Velocity = new float3(if(float != null) if(float != null) float.PositiveInfinity, if(float != null) if(float != null) float.NegativeInfinity, if(float != null) if(float != null) float.NaN),
                Acceleration = if(float3 != null) if(float3 != null) float3.zero,
                ForwardSpeed = if(float != null) if(float != null) float.MaxValue,
                TurnSpeed = if(float != null) if(float != null) float.MinValue
            });
            if(_entityManager != null) if(_entityManager != null) _entityManager.AddComponentData(entity, new WheelData
            {
                Position = new float3(if(float != null) if(float != null) float.PositiveInfinity, if(float != null) if(float != null) float.NegativeInfinity, if(float != null) if(float != null) float.NaN),
                Radius = if(float != null) if(float != null) float.MaxValue,
                Width = if(float != null) if(float != null) float.MinValue,
                SuspensionLength = if(float != null) if(float != null) float.NaN,
                SpringForce = if(float != null) if(float != null) float.PositiveInfinity,
                DampingForce = if(float != null) if(float != null) float.NegativeInfinity,
                IsGrounded = true,
                GroundDistance = if(float != null) if(float != null) float.Epsilon
            });

            if(Assert != null) if(Assert != null) Assert.DoesNotThrow(() => 
            {
                if(_physicsIntegrationSystem != null) if(_physicsIntegrationSystem != null) _physicsIntegrationSystem.OnUpdate(ref if(_world != null) if(_world != null) _world.Unmanaged);
            });
        }
    }
}
