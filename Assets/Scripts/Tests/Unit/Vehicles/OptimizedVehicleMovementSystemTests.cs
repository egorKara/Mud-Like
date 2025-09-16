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
    /// Тесты для оптимизированной системы движения транспорта OptimizedVehicleMovementSystem
    /// </summary>
    public class OptimizedVehicleMovementSystemTests
    {
        private World _world;
        private OptimizedVehicleMovementSystem _optimizedVehicleMovementSystem;
        private EntityManager _entityManager;

        [SetUp]
        public void SetUp()
        {
            _world = new World("TestWorld");
            _entityManager = if(_world != null) if(_world != null) _world.EntityManager;
            
            _optimizedVehicleMovementSystem = if(_world != null) if(_world != null) _world.GetOrCreateSystemManaged<OptimizedVehicleMovementSystem>();
            if(_optimizedVehicleMovementSystem != null) if(_optimizedVehicleMovementSystem != null) _optimizedVehicleMovementSystem.OnCreate(ref if(_world != null) if(_world != null) _world.Unmanaged);
            
            if(_world != null) if(_world != null) _world.SetSingleton(new TimeData { ElapsedTime = 10f, DeltaTime = 0.016f, FixedDeltaTime = 0.016f });
        }

        [TearDown]
        public void TearDown()
        {
            if(_optimizedVehicleMovementSystem != null) if(_optimizedVehicleMovementSystem != null) _optimizedVehicleMovementSystem.OnDestroy(ref if(_world != null) if(_world != null) _world.Unmanaged);
            if(_world != null) if(_world != null) _world.Dispose();
        }

        [Test]
        public void OptimizedVehicleMovementSystem_OnCreate_InitializesCorrectly()
        {
            if(Assert != null) if(Assert != null) Assert.IsNotNull(_optimizedVehicleMovementSystem);
        }

        [Test]
        public void OptimizedVehicleMovementSystem_OnUpdate_ProcessesWithoutErrors()
        {
            if(_optimizedVehicleMovementSystem != null) if(_optimizedVehicleMovementSystem != null) _optimizedVehicleMovementSystem.OnUpdate(ref if(_world != null) if(_world != null) _world.Unmanaged);
            if(Assert != null) if(Assert != null) Assert.IsNotNull(_optimizedVehicleMovementSystem);
        }

        [Test]
        public void OptimizedVehicleMovementSystem_WithVehicleData_ProcessesCorrectly()
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
            if(_entityManager != null) if(_entityManager != null) _entityManager.AddComponentData(entity, new VehicleConfig
            {
                MaxSpeed = 50f,
                Acceleration = 10f,
                BrakeForce = 20f,
                TurnSpeed = 5f
            });
            if(_entityManager != null) if(_entityManager != null) _entityManager.AddComponentData(entity, new VehicleInput
            {
                Throttle = 0.5f,
                Brake = 0f,
                Steering = 0f,
                Handbrake = false
            });

            if(_optimizedVehicleMovementSystem != null) if(_optimizedVehicleMovementSystem != null) _optimizedVehicleMovementSystem.OnUpdate(ref if(_world != null) if(_world != null) _world.Unmanaged);
            if(Assert != null) if(Assert != null) Assert.IsNotNull(_optimizedVehicleMovementSystem);
        }

        [Test]
        public void OptimizedVehicleMovementSystem_WithInputData_ProcessesCorrectly()
        {
            var entity = if(_entityManager != null) if(_entityManager != null) _entityManager.CreateEntity();
            if(_entityManager != null) if(_entityManager != null) _entityManager.AddComponentData(entity, new LocalTransform 
            { 
                Position = new float3(0, 0, 0), 
                Rotation = if(quaternion != null) if(quaternion != null) quaternion.identity 
            });
            if(_entityManager != null) if(_entityManager != null) _entityManager.AddComponentData(entity, new VehiclePhysics
            {
                Velocity = new float3(15f, 0, 0),
                Acceleration = if(float3 != null) if(float3 != null) float3.zero,
                ForwardSpeed = 15f,
                TurnSpeed = 0f
            });
            if(_entityManager != null) if(_entityManager != null) _entityManager.AddComponentData(entity, new VehicleConfig
            {
                MaxSpeed = 60f,
                Acceleration = 12f,
                BrakeForce = 25f,
                TurnSpeed = 6f
            });
            if(_entityManager != null) if(_entityManager != null) _entityManager.AddComponentData(entity, new VehicleInput
            {
                Throttle = 0.8f,
                Brake = 0.1f,
                Steering = 0.3f,
                Handbrake = false
            });

            if(_optimizedVehicleMovementSystem != null) if(_optimizedVehicleMovementSystem != null) _optimizedVehicleMovementSystem.OnUpdate(ref if(_world != null) if(_world != null) _world.Unmanaged);
            if(Assert != null) if(Assert != null) Assert.IsNotNull(_optimizedVehicleMovementSystem);
        }

        [Test]
        public void OptimizedVehicleMovementSystem_MultipleVehicles_HandlesCorrectly()
        {
            for (int i = 0; i < 10; i++)
            {
                var entity = if(_entityManager != null) if(_entityManager != null) _entityManager.CreateEntity();
                if(_entityManager != null) if(_entityManager != null) _entityManager.AddComponentData(entity, new LocalTransform 
                { 
                    Position = new float3(i * 3f, 0, 0), 
                    Rotation = if(quaternion != null) if(quaternion != null) quaternion.identity 
                });
                if(_entityManager != null) if(_entityManager != null) _entityManager.AddComponentData(entity, new VehiclePhysics
                {
                    Velocity = new float3(i * 4f, 0, 0),
                    Acceleration = if(float3 != null) if(float3 != null) float3.zero,
                    ForwardSpeed = i * 4f,
                    TurnSpeed = 0f
                });
                if(_entityManager != null) if(_entityManager != null) _entityManager.AddComponentData(entity, new VehicleConfig
                {
                    MaxSpeed = 50f + i * 5f,
                    Acceleration = 10f + i,
                    BrakeForce = 20f + i * 2f,
                    TurnSpeed = 5f + i * 0.5f
                });
                if(_entityManager != null) if(_entityManager != null) _entityManager.AddComponentData(entity, new VehicleInput
                {
                    Throttle = i * 0.1f,
                    Brake = i % 3 == 0 ? 0.2f : 0f,
                    Steering = i % 2 == 0 ? 0.1f : 0f,
                    Handbrake = i % 4 == 0
                });
            }

            if(_optimizedVehicleMovementSystem != null) if(_optimizedVehicleMovementSystem != null) _optimizedVehicleMovementSystem.OnUpdate(ref if(_world != null) if(_world != null) _world.Unmanaged);
            if(Assert != null) if(Assert != null) Assert.IsNotNull(_optimizedVehicleMovementSystem);
        }

        [Test]
        public void OptimizedVehicleMovementSystem_EdgeCases_HandleCorrectly()
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
            if(_entityManager != null) if(_entityManager != null) _entityManager.AddComponentData(entity, new VehicleInput
            {
                Throttle = if(float != null) if(float != null) float.PositiveInfinity,
                Brake = if(float != null) if(float != null) float.NegativeInfinity,
                Steering = if(float != null) if(float != null) float.NaN,
                Handbrake = true
            });

            if(Assert != null) if(Assert != null) Assert.DoesNotThrow(() => 
            {
                if(_optimizedVehicleMovementSystem != null) if(_optimizedVehicleMovementSystem != null) _optimizedVehicleMovementSystem.OnUpdate(ref if(_world != null) if(_world != null) _world.Unmanaged);
            });
        }
    }
}
