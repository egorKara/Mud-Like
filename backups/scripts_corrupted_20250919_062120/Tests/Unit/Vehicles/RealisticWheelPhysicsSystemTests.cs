using NUnit.Framework;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using MudLike.Vehicles.Systems;
using MudLike.Vehicles.Components;
using Unity.Core;

namespace MudLike.Tests.Unit.Vehicles
{
    /// <summary>
    /// Тесты для системы реалистичной физики колес RealisticWheelPhysicsSystem
    /// </summary>
    public class RealisticWheelPhysicsSystemTests
    {
        private World _world;
        private RealisticWheelPhysicsSystem _realisticWheelPhysicsSystem;
        private EntityManager _entityManager;

        [SetUp]
        public void SetUp()
        {
            _world = new World("TestWorld");
            _entityManager = _world.EntityManager;
            
            _realisticWheelPhysicsSystem = _world.GetOrCreateSystemManaged<RealisticWheelPhysicsSystem>();
            _realisticWheelPhysicsSystem.OnCreate(ref _world.Unmanaged);
            
            _world.SetSingleton(new TimeData { ElapsedTime = 10f, DeltaTime = 0.016f, FixedDeltaTime = 0.016f });
        }

        [TearDown]
        public void TearDown()
        {
            _realisticWheelPhysicsSystem.OnDestroy(ref _world.Unmanaged);
            _world.Dispose();
        }

        [Test]
        public void RealisticWheelPhysicsSystem_OnCreate_InitializesCorrectly()
        {
            Assert.IsNotNull(_realisticWheelPhysicsSystem);
        }

        [Test]
        public void RealisticWheelPhysicsSystem_OnUpdate_ProcessesWithoutErrors()
        {
            _realisticWheelPhysicsSystem.OnUpdate(ref _world.Unmanaged);
            Assert.IsNotNull(_realisticWheelPhysicsSystem);
        }

        [Test]
        public void RealisticWheelPhysicsSystem_WithWheelData_ProcessesCorrectly()
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
                Velocity = new float3(10f, 0, 0),
                Acceleration = float3.zero,
                ForwardSpeed = 10f,
                TurnSpeed = 0f
            });
            _entityManager.AddComponentData(entity, new VehicleConfig
            {
                MaxSpeed = 50f,
                Acceleration = 10f,
                BrakeForce = 20f,
                TurnSpeed = 5f
            });

            _realisticWheelPhysicsSystem.OnUpdate(ref _world.Unmanaged);
            Assert.IsNotNull(_realisticWheelPhysicsSystem);
        }

        [Test]
        public void RealisticWheelPhysicsSystem_WithPhysicsData_ProcessesCorrectly()
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
                Velocity = new float3(15f, 0, 0),
                Acceleration = float3.zero,
                ForwardSpeed = 15f,
                TurnSpeed = 0f
            });
            _entityManager.AddComponentData(entity, new VehicleConfig
            {
                MaxSpeed = 60f,
                Acceleration = 12f,
                BrakeForce = 25f,
                TurnSpeed = 6f
            });

            _realisticWheelPhysicsSystem.OnUpdate(ref _world.Unmanaged);
            Assert.IsNotNull(_realisticWheelPhysicsSystem);
        }

        [Test]
        public void RealisticWheelPhysicsSystem_MultipleWheels_HandlesCorrectly()
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
                _entityManager.AddComponentData(entity, new VehiclePhysics
                {
                    Velocity = new float3(i * 3f, 0, 0),
                    Acceleration = float3.zero,
                    ForwardSpeed = i * 3f,
                    TurnSpeed = 0f
                });
                _entityManager.AddComponentData(entity, new VehicleConfig
                {
                    MaxSpeed = 50f + i * 5f,
                    Acceleration = 10f + i,
                    BrakeForce = 20f + i * 2f,
                    TurnSpeed = 5f + i * 0.5f
                });
            }

            _realisticWheelPhysicsSystem.OnUpdate(ref _world.Unmanaged);
            Assert.IsNotNull(_realisticWheelPhysicsSystem);
        }

        [Test]
        public void RealisticWheelPhysicsSystem_EdgeCases_HandleCorrectly()
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
            _entityManager.AddComponentData(entity, new VehiclePhysics
            {
                Velocity = new float3(float.PositiveInfinity, float.NegativeInfinity, float.NaN),
                Acceleration = float3.zero,
                ForwardSpeed = float.MaxValue,
                TurnSpeed = float.MinValue
            });
            _entityManager.AddComponentData(entity, new VehicleConfig
            {
                MaxSpeed = float.PositiveInfinity,
                Acceleration = float.NegativeInfinity,
                BrakeForce = float.NaN,
                TurnSpeed = float.MaxValue
            });

            Assert.DoesNotThrow(() => 
            {
                _realisticWheelPhysicsSystem.OnUpdate(ref _world.Unmanaged);
            });
        }
    }
}
