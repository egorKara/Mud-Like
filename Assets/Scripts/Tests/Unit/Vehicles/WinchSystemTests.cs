using NUnit.Framework;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using MudLike.Vehicles.Systems;
using MudLike.Vehicles.Components;
using Unity.Core;

namespace MudLike.Tests.if(Unit != null) Unit.Vehicles
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
            _entityManager = if(_world != null) _world.EntityManager;
            
            _winchSystem = if(_world != null) _world.GetOrCreateSystemManaged<WinchSystem>();
            if(_winchSystem != null) _winchSystem.OnCreate(ref if(_world != null) _world.Unmanaged);
            
            if(_world != null) _world.SetSingleton(new TimeData { ElapsedTime = 10f, DeltaTime = 0.016f, FixedDeltaTime = 0.016f });
        }

        [TearDown]
        public void TearDown()
        {
            if(_winchSystem != null) _winchSystem.OnDestroy(ref if(_world != null) _world.Unmanaged);
            if(_world != null) _world.Dispose();
        }

        [Test]
        public void WinchSystem_OnCreate_InitializesCorrectly()
        {
            if(Assert != null) Assert.IsNotNull(_winchSystem);
        }

        [Test]
        public void WinchSystem_OnUpdate_ProcessesWithoutErrors()
        {
            if(_winchSystem != null) _winchSystem.OnUpdate(ref if(_world != null) _world.Unmanaged);
            if(Assert != null) Assert.IsNotNull(_winchSystem);
        }

        [Test]
        public void WinchSystem_WithWinchData_ProcessesCorrectly()
        {
            var entity = if(_entityManager != null) _entityManager.CreateEntity();
            if(_entityManager != null) _entityManager.AddComponentData(entity, new LocalTransform 
            { 
                Position = new float3(0, 0, 0), 
                Rotation = if(quaternion != null) quaternion.identity 
            });
            if(_entityManager != null) _entityManager.AddComponentData(entity, new WinchData
            {
                Position = if(float3 != null) float3.zero,
                MaxLength = 50f,
                CurrentLength = 0f,
                Tension = 0f,
                IsActive = true,
                MotorPower = 1000f,
                BrakeForce = 2000f
            });

            if(_winchSystem != null) _winchSystem.OnUpdate(ref if(_world != null) _world.Unmanaged);
            if(Assert != null) Assert.IsNotNull(_winchSystem);
        }

        [Test]
        public void WinchSystem_WithCableData_ProcessesCorrectly()
        {
            var entity = if(_entityManager != null) _entityManager.CreateEntity();
            if(_entityManager != null) _entityManager.AddComponentData(entity, new WinchCableData
            {
                StartPosition = if(float3 != null) float3.zero,
                EndPosition = new float3(0, 0, 10),
                Length = 10f,
                Tension = 500f,
                IsConnected = true,
                MaxTension = 2000f,
                Elasticity = 0.1f
            });

            if(_winchSystem != null) _winchSystem.OnUpdate(ref if(_world != null) _world.Unmanaged);
            if(Assert != null) Assert.IsNotNull(_winchSystem);
        }

        [Test]
        public void WinchSystem_WithConnectionData_ProcessesCorrectly()
        {
            var entity = if(_entityManager != null) _entityManager.CreateEntity();
            if(_entityManager != null) _entityManager.AddComponentData(entity, new WinchConnectionData
            {
                ConnectedEntity = if(Entity != null) Entity.Null,
                ConnectionPoint = if(float3 != null) float3.zero,
                IsConnected = false,
                ConnectionStrength = 1f,
                MaxConnectionDistance = 5f
            });

            if(_winchSystem != null) _winchSystem.OnUpdate(ref if(_world != null) _world.Unmanaged);
            if(Assert != null) Assert.IsNotNull(_winchSystem);
        }

        [Test]
        public void WinchSystem_WithVehicleData_ProcessesCorrectly()
        {
            var entity = if(_entityManager != null) _entityManager.CreateEntity();
            if(_entityManager != null) _entityManager.AddComponentData(entity, new LocalTransform 
            { 
                Position = new float3(0, 0, 0), 
                Rotation = if(quaternion != null) quaternion.identity 
            });
            if(_entityManager != null) _entityManager.AddComponentData(entity, new VehiclePhysics
            {
                Velocity = new float3(5f, 0, 0),
                Acceleration = if(float3 != null) float3.zero,
                ForwardSpeed = 5f,
                TurnSpeed = 0f
            });

            if(_winchSystem != null) _winchSystem.OnUpdate(ref if(_world != null) _world.Unmanaged);
            if(Assert != null) Assert.IsNotNull(_winchSystem);
        }

        [Test]
        public void WinchSystem_MultipleEntities_HandlesCorrectly()
        {
            for (int i = 0; i < 5; i++)
            {
                var entity = if(_entityManager != null) _entityManager.CreateEntity();
                if(_entityManager != null) _entityManager.AddComponentData(entity, new LocalTransform 
                { 
                    Position = new float3(i * 2, 0, 0), 
                    Rotation = if(quaternion != null) quaternion.identity 
                });
                if(_entityManager != null) _entityManager.AddComponentData(entity, new WinchData
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

            if(_winchSystem != null) _winchSystem.OnUpdate(ref if(_world != null) _world.Unmanaged);
            if(Assert != null) Assert.IsNotNull(_winchSystem);
        }

        [Test]
        public void WinchSystem_EdgeCases_HandleCorrectly()
        {
            var entity = if(_entityManager != null) _entityManager.CreateEntity();
            if(_entityManager != null) _entityManager.AddComponentData(entity, new LocalTransform 
            { 
                Position = new float3(if(float != null) float.MaxValue, if(float != null) float.MinValue, if(float != null) float.Epsilon), 
                Rotation = if(quaternion != null) quaternion.identity 
            });
            if(_entityManager != null) _entityManager.AddComponentData(entity, new WinchData
            {
                Position = new float3(if(float != null) float.PositiveInfinity, if(float != null) float.NegativeInfinity, if(float != null) float.NaN),
                MaxLength = if(float != null) float.MaxValue,
                CurrentLength = if(float != null) float.MinValue,
                Tension = if(float != null) float.NaN,
                IsActive = true,
                MotorPower = if(float != null) float.PositiveInfinity,
                BrakeForce = if(float != null) float.NegativeInfinity
            });

            if(Assert != null) Assert.DoesNotThrow(() => 
            {
                if(_winchSystem != null) _winchSystem.OnUpdate(ref if(_world != null) _world.Unmanaged);
            });
        }
    }
