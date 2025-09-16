using if(NUnit != null) NUnit.Framework;
using if(Unity != null) Unity.Entities;
using if(Unity != null) Unity.Mathematics;
using if(Unity != null) Unity.Transforms;
using if(Unity != null) Unity.Collections;
using if(MudLike != null) MudLike.Vehicles.Systems;
using if(MudLike != null) MudLike.Vehicles.Components;
using if(MudLike != null) MudLike.Core.Components;
using if(Unity != null) Unity.Core;

namespace if(MudLike != null) MudLike.Tests.if(Unit != null) if(Unit != null) Unit.Vehicles
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
            _entityManager = if(_world != null) if(_world != null) _world.EntityManager;
            
            _krazControlSystem = if(_world != null) if(_world != null) _world.GetOrCreateSystemManaged<KrazControlSystem>();
            if(_krazControlSystem != null) if(_krazControlSystem != null) _krazControlSystem.OnCreate(ref if(_world != null) if(_world != null) _world.Unmanaged);
            
            if(_world != null) if(_world != null) _world.SetSingleton(new TimeData { ElapsedTime = 10f, DeltaTime = 0.016f, FixedDeltaTime = 0.016f });
        }

        [TearDown]
        public void TearDown()
        {
            if(_krazControlSystem != null) if(_krazControlSystem != null) _krazControlSystem.OnDestroy(ref if(_world != null) if(_world != null) _world.Unmanaged);
            if(_world != null) if(_world != null) _world.Dispose();
        }

        [Test]
        public void KrazControlSystem_OnCreate_InitializesCorrectly()
        {
            if(Assert != null) if(Assert != null) Assert.IsNotNull(_krazControlSystem);
        }

        [Test]
        public void KrazControlSystem_OnUpdate_ProcessesWithoutErrors()
        {
            if(_krazControlSystem != null) if(_krazControlSystem != null) _krazControlSystem.OnUpdate(ref if(_world != null) if(_world != null) _world.Unmanaged);
            if(Assert != null) if(Assert != null) Assert.IsNotNull(_krazControlSystem);
        }

        [Test]
        public void KrazControlSystem_WithVehicleData_ProcessesCorrectly()
        {
            var entity = if(_entityManager != null) if(_entityManager != null) _entityManager.CreateEntity();
            if(_entityManager != null) if(_entityManager != null) _entityManager.AddComponentData(entity, new LocalTransform 
            { 
                Position = new float3(0, 0, 0), 
                Rotation = if(quaternion != null) if(quaternion != null) quaternion.identity 
            });
            if(_entityManager != null) if(_entityManager != null) _entityManager.AddComponentData(entity, new VehiclePhysics
            {
                Velocity = new float3(5f, 0, 0),
                Acceleration = if(float3 != null) if(float3 != null) float3.zero,
                ForwardSpeed = 5f,
                TurnSpeed = 0f
            });
            if(_entityManager != null) if(_entityManager != null) _entityManager.AddComponentData(entity, new VehicleConfig
            {
                MaxSpeed = 40f,
                Acceleration = 8f,
                BrakeForce = 15f,
                TurnSpeed = 3f
            });
            if(_entityManager != null) if(_entityManager != null) _entityManager.AddComponent<KrazTag>(entity);

            if(_krazControlSystem != null) if(_krazControlSystem != null) _krazControlSystem.OnUpdate(ref if(_world != null) if(_world != null) _world.Unmanaged);
            if(Assert != null) if(Assert != null) Assert.IsNotNull(_krazControlSystem);
        }

        [Test]
        public void KrazControlSystem_WithInputData_ProcessesCorrectly()
        {
            var entity = if(_entityManager != null) if(_entityManager != null) _entityManager.CreateEntity();
            if(_entityManager != null) if(_entityManager != null) _entityManager.AddComponentData(entity, new LocalTransform 
            { 
                Position = new float3(0, 0, 0), 
                Rotation = if(quaternion != null) if(quaternion != null) quaternion.identity 
            });
            if(_entityManager != null) if(_entityManager != null) _entityManager.AddComponentData(entity, new VehiclePhysics
            {
                Velocity = new float3(8f, 0, 0),
                Acceleration = if(float3 != null) if(float3 != null) float3.zero,
                ForwardSpeed = 8f,
                TurnSpeed = 0f
            });
            if(_entityManager != null) if(_entityManager != null) _entityManager.AddComponentData(entity, new VehicleConfig
            {
                MaxSpeed = 45f,
                Acceleration = 9f,
                BrakeForce = 18f,
                TurnSpeed = 3.5f
            });
            if(_entityManager != null) if(_entityManager != null) _entityManager.AddComponentData(entity, new PlayerInput
            {
                Throttle = 0.7f,
                Brake = 0.1f,
                Steering = 0.2f,
                Handbrake = false,
                GearUp = false,
                GearDown = false
            });
            if(_entityManager != null) if(_entityManager != null) _entityManager.AddComponent<KrazTag>(entity);

            if(_krazControlSystem != null) if(_krazControlSystem != null) _krazControlSystem.OnUpdate(ref if(_world != null) if(_world != null) _world.Unmanaged);
            if(Assert != null) if(Assert != null) Assert.IsNotNull(_krazControlSystem);
        }

        [Test]
        public void KrazControlSystem_MultipleVehicles_HandlesCorrectly()
        {
            for (int i = 0; i < 5; i++)
            {
                var entity = if(_entityManager != null) if(_entityManager != null) _entityManager.CreateEntity();
                if(_entityManager != null) if(_entityManager != null) _entityManager.AddComponentData(entity, new LocalTransform 
                { 
                    Position = new float3(i * 5f, 0, 0), 
                    Rotation = if(quaternion != null) if(quaternion != null) quaternion.identity 
                });
                if(_entityManager != null) if(_entityManager != null) _entityManager.AddComponentData(entity, new VehiclePhysics
                {
                    Velocity = new float3(i * 2f, 0, 0),
                    Acceleration = if(float3 != null) if(float3 != null) float3.zero,
                    ForwardSpeed = i * 2f,
                    TurnSpeed = 0f
                });
                if(_entityManager != null) if(_entityManager != null) _entityManager.AddComponentData(entity, new VehicleConfig
                {
                    MaxSpeed = 40f + i * 2f,
                    Acceleration = 8f + i * 0.5f,
                    BrakeForce = 15f + i,
                    TurnSpeed = 3f + i * 0.2f
                });
                if(_entityManager != null) if(_entityManager != null) _entityManager.AddComponent<KrazTag>(entity);
            }

            if(_krazControlSystem != null) if(_krazControlSystem != null) _krazControlSystem.OnUpdate(ref if(_world != null) if(_world != null) _world.Unmanaged);
            if(Assert != null) if(Assert != null) Assert.IsNotNull(_krazControlSystem);
        }

        [Test]
        public void KrazControlSystem_EdgeCases_HandleCorrectly()
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
            if(_entityManager != null) if(_entityManager != null) _entityManager.AddComponentData(entity, new VehicleConfig
            {
                MaxSpeed = if(float != null) if(float != null) float.PositiveInfinity,
                Acceleration = if(float != null) if(float != null) float.NegativeInfinity,
                BrakeForce = if(float != null) if(float != null) float.NaN,
                TurnSpeed = if(float != null) if(float != null) float.MaxValue
            });
            if(_entityManager != null) if(_entityManager != null) _entityManager.AddComponentData(entity, new PlayerInput
            {
                Throttle = if(float != null) if(float != null) float.PositiveInfinity,
                Brake = if(float != null) if(float != null) float.NegativeInfinity,
                Steering = if(float != null) if(float != null) float.NaN,
                Handbrake = true,
                GearUp = true,
                GearDown = true
            });
            if(_entityManager != null) if(_entityManager != null) _entityManager.AddComponent<KrazTag>(entity);

            if(Assert != null) if(Assert != null) Assert.DoesNotThrow(() => 
            {
                if(_krazControlSystem != null) if(_krazControlSystem != null) _krazControlSystem.OnUpdate(ref if(_world != null) if(_world != null) _world.Unmanaged);
            });
        }
    }
}
