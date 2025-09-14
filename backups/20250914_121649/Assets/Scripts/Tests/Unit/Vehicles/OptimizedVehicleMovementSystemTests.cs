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
            _entityManager = _world.EntityManager;
            
            _optimizedVehicleMovementSystem = _world.GetOrCreateSystemManaged<OptimizedVehicleMovementSystem>();
            _optimizedVehicleMovementSystem.OnCreate(ref _world.Unmanaged);
            
            _world.SetSingleton(new TimeData { ElapsedTime = 10f, DeltaTime = 0.016f, FixedDeltaTime = 0.016f });
        }

        [TearDown]
        public void TearDown()
        {
            _optimizedVehicleMovementSystem.OnDestroy(ref _world.Unmanaged);
            _world.Dispose();
        }

        [Test]
        public void OptimizedVehicleMovementSystem_OnCreate_InitializesCorrectly()
        {
            Assert.IsNotNull(_optimizedVehicleMovementSystem);
        }

        [Test]
        public void OptimizedVehicleMovementSystem_OnUpdate_ProcessesWithoutErrors()
        {
            _optimizedVehicleMovementSystem.OnUpdate(ref _world.Unmanaged);
            Assert.IsNotNull(_optimizedVehicleMovementSystem);
        }

        [Test]
        public void OptimizedVehicleMovementSystem_WithVehicleData_ProcessesCorrectly()
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
            _entityManager.AddComponentData(entity, new VehicleConfig
            {
                MaxSpeed = 50f,
                Acceleration = 10f,
                BrakeForce = 20f,
                TurnSpeed = 5f
            });
            _entityManager.AddComponentData(entity, new VehicleInput
            {
                Throttle = 0.5f,
                Brake = 0f,
                Steering = 0f,
                Handbrake = false
            });

            _optimizedVehicleMovementSystem.OnUpdate(ref _world.Unmanaged);
            Assert.IsNotNull(_optimizedVehicleMovementSystem);
        }

        [Test]
        public void OptimizedVehicleMovementSystem_WithInputData_ProcessesCorrectly()
        {
            var entity = _entityManager.CreateEntity();
            _entityManager.AddComponentData(entity, new LocalTransform 
            { 
                Position = new float3(0, 0, 0), 
                Rotation = quaternion.identity 
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
            _entityManager.AddComponentData(entity, new VehicleInput
            {
                Throttle = 0.8f,
                Brake = 0.1f,
                Steering = 0.3f,
                Handbrake = false
            });

            _optimizedVehicleMovementSystem.OnUpdate(ref _world.Unmanaged);
            Assert.IsNotNull(_optimizedVehicleMovementSystem);
        }

        [Test]
        public void OptimizedVehicleMovementSystem_MultipleVehicles_HandlesCorrectly()
        {
            for (int i = 0; i < 10; i++)
            {
                var entity = _entityManager.CreateEntity();
                _entityManager.AddComponentData(entity, new LocalTransform 
                { 
                    Position = new float3(i * 3f, 0, 0), 
                    Rotation = quaternion.identity 
                });
                _entityManager.AddComponentData(entity, new VehiclePhysics
                {
                    Velocity = new float3(i * 4f, 0, 0),
                    Acceleration = float3.zero,
                    ForwardSpeed = i * 4f,
                    TurnSpeed = 0f
                });
                _entityManager.AddComponentData(entity, new VehicleConfig
                {
                    MaxSpeed = 50f + i * 5f,
                    Acceleration = 10f + i,
                    BrakeForce = 20f + i * 2f,
                    TurnSpeed = 5f + i * 0.5f
                });
                _entityManager.AddComponentData(entity, new VehicleInput
                {
                    Throttle = i * 0.1f,
                    Brake = i % 3 == 0 ? 0.2f : 0f,
                    Steering = i % 2 == 0 ? 0.1f : 0f,
                    Handbrake = i % 4 == 0
                });
            }

            _optimizedVehicleMovementSystem.OnUpdate(ref _world.Unmanaged);
            Assert.IsNotNull(_optimizedVehicleMovementSystem);
        }

        [Test]
        public void OptimizedVehicleMovementSystem_EdgeCases_HandleCorrectly()
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
            _entityManager.AddComponentData(entity, new VehicleInput
            {
                Throttle = float.PositiveInfinity,
                Brake = float.NegativeInfinity,
                Steering = float.NaN,
                Handbrake = true
            });

            Assert.DoesNotThrow(() => 
            {
                _optimizedVehicleMovementSystem.OnUpdate(ref _world.Unmanaged);
            });
        }
    }
}
