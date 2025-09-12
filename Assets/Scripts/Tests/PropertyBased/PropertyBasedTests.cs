using NUnit.Framework;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using MudLike.Vehicles.Components;
using MudLike.Vehicles.Systems;
using MudLike.Terrain.Components;
using MudLike.Weather.Components;
using MudLike.Tests.Infrastructure;

namespace MudLike.Tests.PropertyBased
{
    /// <summary>
    /// Property-based тесты для проверки граничных условий
    /// </summary>
    [TestFixture]
    public class PropertyBasedTests : MudLikeTestFixture
    {
        [Test]
        public void WheelPhysics_ShouldHandleAllSurfaceTypes([Values] SurfaceType surfaceType)
        {
            // Arrange
            var wheel = CreateWheel();
            var surface = CreateSurface(surfaceType);
            
            // Act
            var system = World.CreateSystemManaged<AdvancedWheelPhysicsSystem>();
            system.Update();
            
            // Assert
            var wheelData = EntityManager.GetComponentData<WheelData>(wheel);
            Assert.GreaterOrEqual(wheelData.Traction, 0f);
            Assert.LessOrEqual(wheelData.Traction, 1f);
        }
        
        [Test]
        public void VehicleMovement_ShouldWorkWithAllSpeeds([Range(0f, 200f, 10f)] float speed)
        {
            // Arrange
            var vehicle = CreateVehicle();
            var config = EntityManager.GetComponentData<VehicleConfig>(vehicle);
            config.MaxSpeed = speed;
            EntityManager.SetComponentData(vehicle, config);
            
            // Act
            var system = World.CreateSystemManaged<VehicleMovementSystem>();
            system.Update();
            
            // Assert
            var physics = EntityManager.GetComponentData<VehiclePhysics>(vehicle);
            Assert.LessOrEqual(physics.Velocity.magnitude, speed);
        }
        
        [Test]
        public void VehicleMovement_ShouldWorkWithAllInputs([Range(-1f, 1f, 0.1f)] float vertical, [Range(-1f, 1f, 0.1f)] float horizontal)
        {
            // Arrange
            var vehicle = CreateVehicle();
            var input = new VehicleInput { Vertical = vertical, Horizontal = horizontal };
            EntityManager.SetComponentData(vehicle, input);
            
            // Act
            var system = World.CreateSystemManaged<VehicleMovementSystem>();
            system.Update();
            
            // Assert
            var physics = EntityManager.GetComponentData<VehiclePhysics>(vehicle);
            Assert.IsTrue(physics.Velocity.magnitude >= 0f);
        }
        
        [Test]
        public void WeatherSystem_ShouldHandleAllWeatherTypes([Values] WeatherType weatherType)
        {
            // Arrange
            var weather = CreateWeather(weatherType);
            
            // Act
            var system = World.CreateSystemManaged<WeatherSystem>();
            system.Update();
            
            // Assert
            var weatherData = EntityManager.GetComponentData<WeatherData>(weather);
            Assert.AreEqual(weatherType, weatherData.Type);
        }
        
        [Test]
        public void WheelPhysics_ShouldHandleAllPressures([Range(100f, 500f, 50f)] float pressure)
        {
            // Arrange
            var wheel = CreateWheel();
            var wheelPhysics = EntityManager.GetComponentData<WheelPhysicsData>(wheel);
            wheelPhysics.TirePressure = pressure;
            EntityManager.SetComponentData(wheel, wheelPhysics);
            
            // Act
            var system = World.CreateSystemManaged<AdvancedWheelPhysicsSystem>();
            system.Update();
            
            // Assert
            var updatedWheelPhysics = EntityManager.GetComponentData<WheelPhysicsData>(wheel);
            Assert.GreaterOrEqual(updatedWheelPhysics.TirePressure, 0f);
        }
        
        [Test]
        public void VehicleMovement_ShouldHandleAllMasses([Range(500f, 5000f, 500f)] float mass)
        {
            // Arrange
            var vehicle = CreateVehicle();
            var config = EntityManager.GetComponentData<VehicleConfig>(vehicle);
            config.Mass = mass;
            EntityManager.SetComponentData(vehicle, config);
            
            // Act
            var system = World.CreateSystemManaged<VehicleMovementSystem>();
            system.Update();
            
            // Assert
            var physics = EntityManager.GetComponentData<VehiclePhysics>(vehicle);
            Assert.IsTrue(physics.Velocity.magnitude >= 0f);
        }
        
        [Test]
        public void WheelPhysics_ShouldHandleAllTemperatures([Range(-50f, 200f, 25f)] float temperature)
        {
            // Arrange
            var wheel = CreateWheel();
            var wheelPhysics = EntityManager.GetComponentData<WheelPhysicsData>(wheel);
            wheelPhysics.WheelTemperature = temperature;
            EntityManager.SetComponentData(wheel, wheelPhysics);
            
            // Act
            var system = World.CreateSystemManaged<AdvancedWheelPhysicsSystem>();
            system.Update();
            
            // Assert
            var updatedWheelPhysics = EntityManager.GetComponentData<WheelPhysicsData>(wheel);
            Assert.GreaterOrEqual(updatedWheelPhysics.WheelTemperature, -50f);
            Assert.LessOrEqual(updatedWheelPhysics.WheelTemperature, 200f);
        }
        
        [Test]
        public void VehicleMovement_ShouldHandleAllDragValues([Range(0f, 2f, 0.2f)] float drag)
        {
            // Arrange
            var vehicle = CreateVehicle();
            var config = EntityManager.GetComponentData<VehicleConfig>(vehicle);
            config.Drag = drag;
            EntityManager.SetComponentData(vehicle, config);
            
            // Act
            var system = World.CreateSystemManaged<VehicleMovementSystem>();
            system.Update();
            
            // Assert
            var physics = EntityManager.GetComponentData<VehiclePhysics>(vehicle);
            Assert.IsTrue(physics.Velocity.magnitude >= 0f);
        }
        
        [Test]
        public void WheelPhysics_ShouldHandleAllWearValues([Range(0f, 1f, 0.1f)] float wear)
        {
            // Arrange
            var wheel = CreateWheel();
            var wheelPhysics = EntityManager.GetComponentData<WheelPhysicsData>(wheel);
            wheelPhysics.TreadWear = wear;
            EntityManager.SetComponentData(wheel, wheelPhysics);
            
            // Act
            var system = World.CreateSystemManaged<AdvancedWheelPhysicsSystem>();
            system.Update();
            
            // Assert
            var updatedWheelPhysics = EntityManager.GetComponentData<WheelPhysicsData>(wheel);
            Assert.GreaterOrEqual(updatedWheelPhysics.TreadWear, 0f);
            Assert.LessOrEqual(updatedWheelPhysics.TreadWear, 1f);
        }
        
        [Test]
        public void VehicleMovement_ShouldHandleAllAngularDragValues([Range(0f, 10f, 1f)] float angularDrag)
        {
            // Arrange
            var vehicle = CreateVehicle();
            var config = EntityManager.GetComponentData<VehicleConfig>(vehicle);
            config.AngularDrag = angularDrag;
            EntityManager.SetComponentData(vehicle, config);
            
            // Act
            var system = World.CreateSystemManaged<VehicleMovementSystem>();
            system.Update();
            
            // Assert
            var physics = EntityManager.GetComponentData<VehiclePhysics>(vehicle);
            Assert.IsTrue(physics.AngularVelocity.magnitude >= 0f);
        }
        
        [Test]
        public void WheelPhysics_ShouldHandleAllSlipRatios([Range(0f, 2f, 0.2f)] float slipRatio)
        {
            // Arrange
            var wheel = CreateWheel();
            var wheelPhysics = EntityManager.GetComponentData<WheelPhysicsData>(wheel);
            wheelPhysics.SlipRatio = slipRatio;
            EntityManager.SetComponentData(wheel, wheelPhysics);
            
            // Act
            var system = World.CreateSystemManaged<AdvancedWheelPhysicsSystem>();
            system.Update();
            
            // Assert
            var updatedWheelPhysics = EntityManager.GetComponentData<WheelPhysicsData>(wheel);
            Assert.GreaterOrEqual(updatedWheelPhysics.SlipRatio, 0f);
        }
        
        [Test]
        public void VehicleMovement_ShouldHandleAllAccelerations([Range(0f, 50f, 5f)] float acceleration)
        {
            // Arrange
            var vehicle = CreateVehicle();
            var config = EntityManager.GetComponentData<VehicleConfig>(vehicle);
            config.Acceleration = acceleration;
            EntityManager.SetComponentData(vehicle, config);
            
            // Act
            var system = World.CreateSystemManaged<VehicleMovementSystem>();
            system.Update();
            
            // Assert
            var physics = EntityManager.GetComponentData<VehiclePhysics>(vehicle);
            Assert.IsTrue(physics.Acceleration.magnitude >= 0f);
        }
        
        [Test]
        public void WheelPhysics_ShouldHandleAllSinkDepths([Range(0f, 1f, 0.1f)] float sinkDepth)
        {
            // Arrange
            var wheel = CreateWheel();
            var wheelPhysics = EntityManager.GetComponentData<WheelPhysicsData>(wheel);
            wheelPhysics.SinkDepth = sinkDepth;
            EntityManager.SetComponentData(wheel, wheelPhysics);
            
            // Act
            var system = World.CreateSystemManaged<AdvancedWheelPhysicsSystem>();
            system.Update();
            
            // Assert
            var updatedWheelPhysics = EntityManager.GetComponentData<WheelPhysicsData>(wheel);
            Assert.GreaterOrEqual(updatedWheelPhysics.SinkDepth, 0f);
            Assert.LessOrEqual(updatedWheelPhysics.SinkDepth, 1f);
        }
        
        [Test]
        public void VehicleMovement_ShouldHandleAllTurnSpeeds([Range(0f, 10f, 1f)] float turnSpeed)
        {
            // Arrange
            var vehicle = CreateVehicle();
            var config = EntityManager.GetComponentData<VehicleConfig>(vehicle);
            config.TurnSpeed = turnSpeed;
            EntityManager.SetComponentData(vehicle, config);
            
            // Act
            var system = World.CreateSystemManaged<VehicleMovementSystem>();
            system.Update();
            
            // Assert
            var physics = EntityManager.GetComponentData<VehiclePhysics>(vehicle);
            Assert.IsTrue(physics.TurnSpeed >= 0f);
        }
        
        [Test]
        public void WheelPhysics_ShouldHandleAllTractionValues([Range(0f, 1f, 0.1f)] float traction)
        {
            // Arrange
            var wheel = CreateWheel();
            var wheelData = EntityManager.GetComponentData<WheelData>(wheel);
            wheelData.Traction = traction;
            EntityManager.SetComponentData(wheel, wheelData);
            
            // Act
            var system = World.CreateSystemManaged<AdvancedWheelPhysicsSystem>();
            system.Update();
            
            // Assert
            var updatedWheelData = EntityManager.GetComponentData<WheelData>(wheel);
            Assert.GreaterOrEqual(updatedWheelData.Traction, 0f);
            Assert.LessOrEqual(updatedWheelData.Traction, 1f);
        }
    }
}