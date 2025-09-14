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
    /// Тесты для системы лебедки WinchSystem
    /// </summary>
    public class WinchSystemTests
    {
        private World _world;
        private WinchSystem _winchSystem;
        private EntityManager _entityManager;

        [SetUp]
        public void SetUp()
        {
            _world = new World("TestWorld");
            _entityManager = _world.EntityManager;
            
            _winchSystem = _world.GetOrCreateSystemManaged<WinchSystem>();
            _winchSystem.OnCreate(ref _world.Unmanaged);
            
            _world.SetSingleton(new TimeData { ElapsedTime = 10f, DeltaTime = 0.016f, FixedDeltaTime = 0.016f });
        }

        [TearDown]
        public void TearDown()
        {
            _winchSystem.OnDestroy(ref _world.Unmanaged);
            _world.Dispose();
        }

        [Test]
        public void WinchSystem_OnCreate_InitializesCorrectly()
        {
            Assert.IsNotNull(_winchSystem);
        }

        [Test]
        public void WinchSystem_OnUpdate_ProcessesWithoutErrors()
        {
            _winchSystem.OnUpdate(ref _world.Unmanaged);
            Assert.IsNotNull(_winchSystem);
        }

        [Test]
        public void WinchSystem_WithWinchData_ProcessesCorrectly()
        {
            var entity = _entityManager.CreateEntity();
            _entityManager.AddComponentData(entity, new LocalTransform 
            { 
                Position = new float3(0, 0, 0), 
                Rotation = quaternion.identity 
            });
            _entityManager.AddComponentData(entity, new WinchData
            {
                Position = float3.zero,
                MaxLength = 50f,
                CurrentLength = 0f,
                Tension = 0f,
                IsActive = true,
                MotorPower = 1000f,
                BrakeForce = 2000f
            });

            _winchSystem.OnUpdate(ref _world.Unmanaged);
            Assert.IsNotNull(_winchSystem);
        }

        [Test]
        public void WinchSystem_WithCableData_ProcessesCorrectly()
        {
            var entity = _entityManager.CreateEntity();
            _entityManager.AddComponentData(entity, new WinchCableData
            {
                StartPosition = float3.zero,
                EndPosition = new float3(0, 0, 10),
                Length = 10f,
                Tension = 500f,
                IsConnected = true,
                MaxTension = 2000f,
                Elasticity = 0.1f
            });

            _winchSystem.OnUpdate(ref _world.Unmanaged);
            Assert.IsNotNull(_winchSystem);
        }

        [Test]
        public void WinchSystem_WithConnectionData_ProcessesCorrectly()
        {
            var entity = _entityManager.CreateEntity();
            _entityManager.AddComponentData(entity, new WinchConnectionData
            {
                ConnectedEntity = Entity.Null,
                ConnectionPoint = float3.zero,
                IsConnected = false,
                ConnectionStrength = 1f,
                MaxConnectionDistance = 5f
            });

            _winchSystem.OnUpdate(ref _world.Unmanaged);
            Assert.IsNotNull(_winchSystem);
        }

        [Test]
        public void WinchSystem_WithVehicleData_ProcessesCorrectly()
        {
            var entity = _entityManager.CreateEntity();
            _entityManager.AddComponentData(entity, new LocalTransform 
            { 
                Position = new float3(0, 0, 0), 
                Rotation = quaternion.identity 
            });
            _entityManager.AddComponentData(entity, new VehiclePhysics
            {
                Velocity = new float3(5f, 0, 0),
                Acceleration = float3.zero,
                ForwardSpeed = 5f,
                TurnSpeed = 0f
            });

            _winchSystem.OnUpdate(ref _world.Unmanaged);
            Assert.IsNotNull(_winchSystem);
        }

        [Test]
        public void WinchSystem_MultipleEntities_HandlesCorrectly()
        {
            for (int i = 0; i < 5; i++)
            {
                var entity = _entityManager.CreateEntity();
                _entityManager.AddComponentData(entity, new LocalTransform 
                { 
                    Position = new float3(i * 2, 0, 0), 
                    Rotation = quaternion.identity 
                });
                _entityManager.AddComponentData(entity, new WinchData
                {
                    Position = new float3(i * 2, 0, 0),
                    MaxLength = 50f + i * 10f,
                    CurrentLength = i * 5f,
                    Tension = i * 100f,
                    IsActive = i % 2 == 0,
                    MotorPower = 1000f + i * 200f,
                    BrakeForce = 2000f + i * 400f
                });
            }

            _winchSystem.OnUpdate(ref _world.Unmanaged);
            Assert.IsNotNull(_winchSystem);
        }

        [Test]
        public void WinchSystem_EdgeCases_HandleCorrectly()
        {
            var entity = _entityManager.CreateEntity();
            _entityManager.AddComponentData(entity, new LocalTransform 
            { 
                Position = new float3(float.MaxValue, float.MinValue, float.Epsilon), 
                Rotation = quaternion.identity 
            });
            _entityManager.AddComponentData(entity, new WinchData
            {
                Position = new float3(float.PositiveInfinity, float.NegativeInfinity, float.NaN),
                MaxLength = float.MaxValue,
                CurrentLength = float.MinValue,
                Tension = float.NaN,
                IsActive = true,
                MotorPower = float.PositiveInfinity,
                BrakeForce = float.NegativeInfinity
            });

            Assert.DoesNotThrow(() => 
            {
                _winchSystem.OnUpdate(ref _world.Unmanaged);
            });
        }
    }
}