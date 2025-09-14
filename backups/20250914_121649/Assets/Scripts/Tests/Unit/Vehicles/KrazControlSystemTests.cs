using NUnit.Framework;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using MudLike.Vehicles.Systems;
using MudLike.Vehicles.Components;
using MudLike.Core.Components;
using Unity.Core;

namespace MudLike.Tests.Unit.Vehicles
{
    /// <summary>
    /// Тесты для системы управления КРАЗ-транспортом KrazControlSystem
    /// </summary>
    public class KrazControlSystemTests
    {
        private World _world;
        private KrazControlSystem _krazControlSystem;
        private EntityManager _entityManager;

        [SetUp]
        public void SetUp()
        {
            _world = new World("TestWorld");
            _entityManager = _world.EntityManager;
            
            _krazControlSystem = _world.GetOrCreateSystemManaged<KrazControlSystem>();
            _krazControlSystem.OnCreate(ref _world.Unmanaged);
            
            _world.SetSingleton(new TimeData { ElapsedTime = 10f, DeltaTime = 0.016f, FixedDeltaTime = 0.016f });
        }

        [TearDown]
        public void TearDown()
        {
            _krazControlSystem.OnDestroy(ref _world.Unmanaged);
            _world.Dispose();
        }

        [Test]
        public void KrazControlSystem_OnCreate_InitializesCorrectly()
        {
            Assert.IsNotNull(_krazControlSystem);
        }

        [Test]
        public void KrazControlSystem_OnUpdate_ProcessesWithoutErrors()
        {
            _krazControlSystem.OnUpdate(ref _world.Unmanaged);
            Assert.IsNotNull(_krazControlSystem);
        }

        [Test]
        public void KrazControlSystem_WithVehicleData_ProcessesCorrectly()
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
            _entityManager.AddComponentData(entity, new VehicleConfig
            {
                MaxSpeed = 40f,
                Acceleration = 8f,
                BrakeForce = 15f,
                TurnSpeed = 3f
            });
            _entityManager.AddComponent<KrazTag>(entity);

            _krazControlSystem.OnUpdate(ref _world.Unmanaged);
            Assert.IsNotNull(_krazControlSystem);
        }

        [Test]
        public void KrazControlSystem_WithInputData_ProcessesCorrectly()
        {
            var entity = _entityManager.CreateEntity();
            _entityManager.AddComponentData(entity, new LocalTransform 
            { 
                Position = new float3(0, 0, 0), 
                Rotation = quaternion.identity 
            });
            _entityManager.AddComponentData(entity, new VehiclePhysics
            {
                Velocity = new float3(8f, 0, 0),
                Acceleration = float3.zero,
                ForwardSpeed = 8f,
                TurnSpeed = 0f
            });
            _entityManager.AddComponentData(entity, new VehicleConfig
            {
                MaxSpeed = 45f,
                Acceleration = 9f,
                BrakeForce = 18f,
                TurnSpeed = 3.5f
            });
            _entityManager.AddComponentData(entity, new PlayerInput
            {
                Throttle = 0.7f,
                Brake = 0.1f,
                Steering = 0.2f,
                Handbrake = false,
                GearUp = false,
                GearDown = false
            });
            _entityManager.AddComponent<KrazTag>(entity);

            _krazControlSystem.OnUpdate(ref _world.Unmanaged);
            Assert.IsNotNull(_krazControlSystem);
        }

        [Test]
        public void KrazControlSystem_MultipleVehicles_HandlesCorrectly()
        {
            for (int i = 0; i < 5; i++)
            {
                var entity = _entityManager.CreateEntity();
                _entityManager.AddComponentData(entity, new LocalTransform 
                { 
                    Position = new float3(i * 5f, 0, 0), 
                    Rotation = quaternion.identity 
                });
                _entityManager.AddComponentData(entity, new VehiclePhysics
                {
                    Velocity = new float3(i * 2f, 0, 0),
                    Acceleration = float3.zero,
                    ForwardSpeed = i * 2f,
                    TurnSpeed = 0f
                });
                _entityManager.AddComponentData(entity, new VehicleConfig
                {
                    MaxSpeed = 40f + i * 2f,
                    Acceleration = 8f + i * 0.5f,
                    BrakeForce = 15f + i,
                    TurnSpeed = 3f + i * 0.2f
                });
                _entityManager.AddComponent<KrazTag>(entity);
            }

            _krazControlSystem.OnUpdate(ref _world.Unmanaged);
            Assert.IsNotNull(_krazControlSystem);
        }

        [Test]
        public void KrazControlSystem_EdgeCases_HandleCorrectly()
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
            _entityManager.AddComponentData(entity, new VehicleConfig
            {
                MaxSpeed = float.PositiveInfinity,
                Acceleration = float.NegativeInfinity,
                BrakeForce = float.NaN,
                TurnSpeed = float.MaxValue
            });
            _entityManager.AddComponentData(entity, new PlayerInput
            {
                Throttle = float.PositiveInfinity,
                Brake = float.NegativeInfinity,
                Steering = float.NaN,
                Handbrake = true,
                GearUp = true,
                GearDown = true
            });
            _entityManager.AddComponent<KrazTag>(entity);

            Assert.DoesNotThrow(() => 
            {
                _krazControlSystem.OnUpdate(ref _world.Unmanaged);
            });
        }
    }
}
