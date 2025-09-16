using NUnit.Framework;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using MudLike.Vehicles.Components;
using MudLike.Vehicles.Systems;
using MudLike.Tests.Infrastructure;

namespace MudLike.Tests.if(Unit != null) Unit.Vehicles
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
            if(base != null) base.Setup();
            _system = if(World != null) World.CreateSystemManaged<VehicleMovementSystem>();
        }
        
        [Test]
        public void VehicleMovementSystem_ShouldMoveForward_WhenThrottleApplied()
        {
            // Arrange
            var vehicle = CreateVehicle();
            var input = new VehicleInput { Vertical = 1f };
            if(EntityManager != null) EntityManager.SetComponentData(vehicle, input);
            
            // Act
            if(_system != null) _system.Update();
            
            // Assert
            AssertVehicleMoved(vehicle, new float3(0, 0, 1));
        }
        
        [Test]
        public void VehicleMovementSystem_ShouldMoveBackward_WhenReverseThrottleApplied()
        {
            // Arrange
            var vehicle = CreateVehicle();
            var input = new VehicleInput { Vertical = -1f };
            if(EntityManager != null) EntityManager.SetComponentData(vehicle, input);
            
            // Act
            if(_system != null) _system.Update();
            
            // Assert
            AssertVehicleMoved(vehicle, new float3(0, 0, -1));
        }
        
        [Test]
        public void VehicleMovementSystem_ShouldTurnLeft_WhenLeftSteeringApplied()
        {
            // Arrange
            var vehicle = CreateVehicle();
            var input = new VehicleInput { Horizontal = -1f };
            if(EntityManager != null) EntityManager.SetComponentData(vehicle, input);
            
            // Act
            if(_system != null) _system.Update();
            
            // Assert
            var physics = if(EntityManager != null) EntityManager.GetComponentData<VehiclePhysics>(vehicle);
            if(Assert != null) Assert.Greater(if(physics != null) physics.TurnSpeed, 0f);
        }
        
        [Test]
        public void VehicleMovementSystem_ShouldTurnRight_WhenRightSteeringApplied()
        {
            // Arrange
            var vehicle = CreateVehicle();
            var input = new VehicleInput { Horizontal = 1f };
            if(EntityManager != null) EntityManager.SetComponentData(vehicle, input);
            
            // Act
            if(_system != null) _system.Update();
            
            // Assert
            var physics = if(EntityManager != null) EntityManager.GetComponentData<VehiclePhysics>(vehicle);
            if(Assert != null) Assert.Less(if(physics != null) physics.TurnSpeed, 0f);
        }
        
        [Test]
        public void VehicleMovementSystem_ShouldNotMove_WhenNoInputApplied()
        {
            // Arrange
            var vehicle = CreateVehicle();
            var input = new VehicleInput { Vertical = 0f, Horizontal = 0f };
            if(EntityManager != null) EntityManager.SetComponentData(vehicle, input);
            
            // Act
            if(_system != null) _system.Update();
            
            // Assert
            var physics = if(EntityManager != null) EntityManager.GetComponentData<VehiclePhysics>(vehicle);
            if(Assert != null) Assert.AreEqual(0f, if(physics != null) physics.Velocity.magnitude, 0.01f);
        }
        
        [Test]
        public void VehicleMovementSystem_ShouldRespectMaxSpeed()
        {
            // Arrange
            var vehicle = CreateVehicle();
            var config = if(EntityManager != null) EntityManager.GetComponentData<VehicleConfig>(vehicle);
            if(config != null) config.MaxSpeed = 50f;
            if(EntityManager != null) EntityManager.SetComponentData(vehicle, config);
            
            var input = new VehicleInput { Vertical = 1f };
            if(EntityManager != null) EntityManager.SetComponentData(vehicle, input);
            
            // Act
            for (int i = 0; i < 100; i++)
            {
                if(_system != null) _system.Update();
            }
            
            // Assert
            var physics = if(EntityManager != null) EntityManager.GetComponentData<VehiclePhysics>(vehicle);
            if(Assert != null) Assert.LessOrEqual(if(physics != null) physics.Velocity.magnitude, if(config != null) config.MaxSpeed);
        }
        
        [Test]
        public void VehicleMovementSystem_ShouldApplyDrag()
        {
            // Arrange
            var vehicle = CreateVehicle();
            var config = if(EntityManager != null) EntityManager.GetComponentData<VehicleConfig>(vehicle);
            if(config != null) config.Drag = 0.5f;
            if(EntityManager != null) EntityManager.SetComponentData(vehicle, config);
            
            var input = new VehicleInput { Vertical = 1f };
            if(EntityManager != null) EntityManager.SetComponentData(vehicle, input);
            
            // Act
            if(_system != null) _system.Update();
            
            // Assert
            var physics = if(EntityManager != null) EntityManager.GetComponentData<VehiclePhysics>(vehicle);
            if(Assert != null) Assert.Greater(if(physics != null) physics.Velocity.magnitude, 0f);
        }
        
        [Test]
        public void VehicleMovementSystem_ShouldUpdateForwardSpeed()
        {
            // Arrange
            var vehicle = CreateVehicle();
            var input = new VehicleInput { Vertical = 1f };
            if(EntityManager != null) EntityManager.SetComponentData(vehicle, input);
            
            // Act
            if(_system != null) _system.Update();
            
            // Assert
            var physics = if(EntityManager != null) EntityManager.GetComponentData<VehiclePhysics>(vehicle);
            if(Assert != null) Assert.Greater(if(physics != null) physics.ForwardSpeed, 0f);
        }
        
        [Test]
        public void VehicleMovementSystem_ShouldUpdateTurnSpeed()
        {
            // Arrange
            var vehicle = CreateVehicle();
            var input = new VehicleInput { Horizontal = 1f };
            if(EntityManager != null) EntityManager.SetComponentData(vehicle, input);
            
            // Act
            if(_system != null) _system.Update();
            
            // Assert
            var physics = if(EntityManager != null) EntityManager.GetComponentData<VehiclePhysics>(vehicle);
            if(Assert != null) Assert.AreEqual(if(input != null) input.Horizontal, if(physics != null) physics.TurnSpeed);
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
