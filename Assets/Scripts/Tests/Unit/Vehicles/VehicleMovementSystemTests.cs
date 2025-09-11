using NUnit.Framework;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using MudLike.Vehicles.Components;
using MudLike.Vehicles.Systems;
using MudLike.Tests.Infrastructure;

namespace MudLike.Tests.Unit.Vehicles
{
    /// <summary>
    /// Unit тесты для VehicleMovementSystem
    /// </summary>
    [TestFixture]
    public class VehicleMovementSystemTests : MudLikeTestFixture
    {
        private VehicleMovementSystem _system;
        
        [SetUp]
        public override void Setup()
        {
            base.Setup();
            _system = World.CreateSystemManaged<VehicleMovementSystem>();
        }
        
        [Test]
        public void VehicleMovementSystem_ShouldMoveForward_WhenThrottleApplied()
        {
            // Arrange
            var vehicle = CreateVehicle();
            var input = new VehicleInput { Vertical = 1f };
            EntityManager.SetComponentData(vehicle, input);
            
            // Act
            _system.Update();
            
            // Assert
            AssertVehicleMoved(vehicle, new float3(0, 0, 1));
        }
        
        [Test]
        public void VehicleMovementSystem_ShouldMoveBackward_WhenReverseThrottleApplied()
        {
            // Arrange
            var vehicle = CreateVehicle();
            var input = new VehicleInput { Vertical = -1f };
            EntityManager.SetComponentData(vehicle, input);
            
            // Act
            _system.Update();
            
            // Assert
            AssertVehicleMoved(vehicle, new float3(0, 0, -1));
        }
        
        [Test]
        public void VehicleMovementSystem_ShouldTurnLeft_WhenLeftSteeringApplied()
        {
            // Arrange
            var vehicle = CreateVehicle();
            var input = new VehicleInput { Horizontal = -1f };
            EntityManager.SetComponentData(vehicle, input);
            
            // Act
            _system.Update();
            
            // Assert
            var physics = EntityManager.GetComponentData<VehiclePhysics>(vehicle);
            Assert.Greater(physics.TurnSpeed, 0f);
        }
        
        [Test]
        public void VehicleMovementSystem_ShouldTurnRight_WhenRightSteeringApplied()
        {
            // Arrange
            var vehicle = CreateVehicle();
            var input = new VehicleInput { Horizontal = 1f };
            EntityManager.SetComponentData(vehicle, input);
            
            // Act
            _system.Update();
            
            // Assert
            var physics = EntityManager.GetComponentData<VehiclePhysics>(vehicle);
            Assert.Less(physics.TurnSpeed, 0f);
        }
        
        [Test]
        public void VehicleMovementSystem_ShouldNotMove_WhenNoInputApplied()
        {
            // Arrange
            var vehicle = CreateVehicle();
            var input = new VehicleInput { Vertical = 0f, Horizontal = 0f };
            EntityManager.SetComponentData(vehicle, input);
            
            // Act
            _system.Update();
            
            // Assert
            var physics = EntityManager.GetComponentData<VehiclePhysics>(vehicle);
            Assert.AreEqual(0f, physics.Velocity.magnitude, 0.01f);
        }
        
        [Test]
        public void VehicleMovementSystem_ShouldRespectMaxSpeed()
        {
            // Arrange
            var vehicle = CreateVehicle();
            var config = EntityManager.GetComponentData<VehicleConfig>(vehicle);
            config.MaxSpeed = 50f;
            EntityManager.SetComponentData(vehicle, config);
            
            var input = new VehicleInput { Vertical = 1f };
            EntityManager.SetComponentData(vehicle, input);
            
            // Act
            for (int i = 0; i < 100; i++)
            {
                _system.Update();
            }
            
            // Assert
            var physics = EntityManager.GetComponentData<VehiclePhysics>(vehicle);
            Assert.LessOrEqual(physics.Velocity.magnitude, config.MaxSpeed);
        }
        
        [Test]
        public void VehicleMovementSystem_ShouldApplyDrag()
        {
            // Arrange
            var vehicle = CreateVehicle();
            var config = EntityManager.GetComponentData<VehicleConfig>(vehicle);
            config.Drag = 0.5f;
            EntityManager.SetComponentData(vehicle, config);
            
            var input = new VehicleInput { Vertical = 1f };
            EntityManager.SetComponentData(vehicle, input);
            
            // Act
            _system.Update();
            
            // Assert
            var physics = EntityManager.GetComponentData<VehiclePhysics>(vehicle);
            Assert.Greater(physics.Velocity.magnitude, 0f);
        }
        
        [Test]
        public void VehicleMovementSystem_ShouldUpdateForwardSpeed()
        {
            // Arrange
            var vehicle = CreateVehicle();
            var input = new VehicleInput { Vertical = 1f };
            EntityManager.SetComponentData(vehicle, input);
            
            // Act
            _system.Update();
            
            // Assert
            var physics = EntityManager.GetComponentData<VehiclePhysics>(vehicle);
            Assert.Greater(physics.ForwardSpeed, 0f);
        }
        
        [Test]
        public void VehicleMovementSystem_ShouldUpdateTurnSpeed()
        {
            // Arrange
            var vehicle = CreateVehicle();
            var input = new VehicleInput { Horizontal = 1f };
            EntityManager.SetComponentData(vehicle, input);
            
            // Act
            _system.Update();
            
            // Assert
            var physics = EntityManager.GetComponentData<VehiclePhysics>(vehicle);
            Assert.AreEqual(input.Horizontal, physics.TurnSpeed);
        }
        
        [Test]
        public void VehicleMovementSystem_ShouldRunWithoutErrors()
        {
            // Arrange
            var vehicle = CreateVehicle();
            
            // Act & Assert
            AssertSystemRunsWithoutErrors<VehicleMovementSystem>();
        }
        
        [Test]
        public void VehicleMovementSystem_ShouldMeetPerformanceRequirements()
        {
            // Arrange
            for (int i = 0; i < 1000; i++)
            {
                CreateVehicle();
            }
            
            // Act & Assert
            AssertSystemPerformance<VehicleMovementSystem>(16.67f); // 60 FPS
        }
    }
}