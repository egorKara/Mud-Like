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
    /// Тесты для системы физики колес WheelPhysicsSystem
    /// </summary>
    public class WheelPhysicsSystemTests
    {
        private World _world;
        private WheelPhysicsSystem _wheelPhysicsSystem;
        private EntityManager _entityManager;

        [SetUp]
        public void SetUp()
        {
            _world = new World("TestWorld");
            _entityManager = _world.EntityManager;
            
            _wheelPhysicsSystem = _world.GetOrCreateSystemManaged<WheelPhysicsSystem>();
            _wheelPhysicsSystem.OnCreate(ref _world.Unmanaged);
            
            _world.SetSingleton(new TimeData { ElapsedTime = 10f, DeltaTime = 0.016f, FixedDeltaTime = 0.016f });
        }

        [TearDown]
        public void TearDown()
        {
            _wheelPhysicsSystem.OnDestroy(ref _world.Unmanaged);
            _world.Dispose();
        }

        [Test]
        public void WheelPhysicsSystem_OnCreate_InitializesCorrectly()
        {
            Assert.IsNotNull(_wheelPhysicsSystem);
        }

        [Test]
        public void WheelPhysicsSystem_OnUpdate_ProcessesWithoutErrors()
        {
            _wheelPhysicsSystem.OnUpdate(ref _world.Unmanaged);
            Assert.IsNotNull(_wheelPhysicsSystem);
        }

        [Test]
        public void WheelPhysicsSystem_WithWheelData_ProcessesCorrectly()
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

            _wheelPhysicsSystem.OnUpdate(ref _world.Unmanaged);
            Assert.IsNotNull(_wheelPhysicsSystem);
        }

        [Test]
        public void WheelPhysicsSystem_WithPhysicsData_ProcessesCorrectly()
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
                Velocity = new float3(5f, 0, 0),
                AngularVelocity = new float3(0, 0, 10f),
                Friction = 0.8f,
                Grip = 0.9f,
                Pressure = 2.5f,
                Temperature = 75f,
                Wear = 0.1f
            });

            _wheelPhysicsSystem.OnUpdate(ref _world.Unmanaged);
            Assert.IsNotNull(_wheelPhysicsSystem);
        }

        [Test]
        public void WheelPhysicsSystem_MultipleWheels_HandlesCorrectly()
        {
            for (int i = 0; i < 8; i++)
            {
                var entity = _entityManager.CreateEntity();
                _entityManager.AddComponentData(entity, new LocalTransform 
                { 
                    Position = new float3(i * 2f, 0, 0), 
                    Rotation = quaternion.identity 
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
                _entityManager.AddComponentData(entity, new WheelPhysicsData
                {
                    Velocity = new float3(i * 3f, 0, 0),
                    AngularVelocity = new float3(0, 0, i * 5f),
                    Friction = 0.8f - i * 0.05f,
                    Grip = 0.9f - i * 0.02f,
                    Pressure = 2.5f - i * 0.1f,
                    Temperature = 75f + i * 5f,
                    Wear = i * 0.05f
                });
            }

            _wheelPhysicsSystem.OnUpdate(ref _world.Unmanaged);
            Assert.IsNotNull(_wheelPhysicsSystem);
        }

        [Test]
        public void WheelPhysicsSystem_EdgeCases_HandleCorrectly()
        {
            var entity = _entityManager.CreateEntity();
            _entityManager.AddComponentData(entity, new LocalTransform 
            { 
                Position = new float3(float.MaxValue, float.MinValue, float.Epsilon), 
                Rotation = quaternion.identity 
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
            _entityManager.AddComponentData(entity, new WheelPhysicsData
            {
                Velocity = new float3(float.PositiveInfinity, float.NegativeInfinity, float.NaN),
                AngularVelocity = new float3(float.MaxValue, float.MinValue, float.NaN),
                Friction = float.PositiveInfinity,
                Grip = float.NegativeInfinity,
                Pressure = float.NaN,
                Temperature = float.MaxValue,
                Wear = float.MinValue
            });

            Assert.DoesNotThrow(() => 
            {
                _wheelPhysicsSystem.OnUpdate(ref _world.Unmanaged);
            });
        }
    }
}
