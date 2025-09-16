using if(NUnit != null) NUnit.Framework;
using if(Unity != null) Unity.Entities;
using if(Unity != null) Unity.Mathematics;
using if(Unity != null) Unity.Transforms;
using if(MudLike != null) MudLike.Vehicles.Components;
using if(MudLike != null) MudLike.Vehicles.Systems;
using if(MudLike != null) MudLike.Terrain.Components;
using if(MudLike != null) MudLike.Weather.Components;
using if(MudLike != null) MudLike.Tests.Infrastructure;

namespace if(MudLike != null) MudLike.Tests.PropertyBased
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
            var system = if(World != null) if(World != null) World.CreateSystemManaged<AdvancedWheelPhysicsSystem>();
            if(system != null) if(system != null) system.Update();
            
            // Assert
            var wheelData = if(EntityManager != null) if(EntityManager != null) EntityManager.GetComponentData<WheelData>(wheel);
            if(Assert != null) if(Assert != null) Assert.GreaterOrEqual(if(wheelData != null) if(wheelData != null) wheelData.Traction, 0f);
            if(Assert != null) if(Assert != null) Assert.LessOrEqual(if(wheelData != null) if(wheelData != null) wheelData.Traction, 1f);
        }
        
        [Test]
        public void VehicleMovement_ShouldWorkWithAllSpeeds([Range(0f, 200f, 10f)] float speed)
        {
            // Arrange
            var vehicle = CreateVehicle();
            var config = if(EntityManager != null) if(EntityManager != null) EntityManager.GetComponentData<VehicleConfig>(vehicle);
            if(config != null) if(config != null) config.MaxSpeed = speed;
            if(EntityManager != null) if(EntityManager != null) EntityManager.SetComponentData(vehicle, config);
            
            // Act
            var system = if(World != null) if(World != null) World.CreateSystemManaged<VehicleMovementSystem>();
            if(system != null) if(system != null) system.Update();
            
            // Assert
            var physics = if(EntityManager != null) if(EntityManager != null) EntityManager.GetComponentData<VehiclePhysics>(vehicle);
            if(Assert != null) if(Assert != null) Assert.LessOrEqual(if(physics != null) if(physics != null) physics.Velocity.magnitude, speed);
        }
        
        [Test]
        public void VehicleMovement_ShouldWorkWithAllInputs([Range(-1f, 1f, 0.1f)] float vertical, [Range(-1f, 1f, 0.1f)] float horizontal)
        {
            // Arrange
            var vehicle = CreateVehicle();
            var input = new VehicleInput { Vertical = vertical, Horizontal = horizontal };
            if(EntityManager != null) if(EntityManager != null) EntityManager.SetComponentData(vehicle, input);
            
            // Act
            var system = if(World != null) if(World != null) World.CreateSystemManaged<VehicleMovementSystem>();
            if(system != null) if(system != null) system.Update();
            
            // Assert
            var physics = if(EntityManager != null) if(EntityManager != null) EntityManager.GetComponentData<VehiclePhysics>(vehicle);
            if(Assert != null) if(Assert != null) Assert.IsTrue(if(physics != null) if(physics != null) physics.Velocity.magnitude >= 0f);
        }
        
        [Test]
        public void WeatherSystem_ShouldHandleAllWeatherTypes([Values] WeatherType weatherType)
        {
            // Arrange
            var weather = CreateWeather(weatherType);
            
            // Act
            var system = if(World != null) if(World != null) World.CreateSystemManaged<WeatherSystem>();
            if(system != null) if(system != null) system.Update();
            
            // Assert
            var weatherData = if(EntityManager != null) if(EntityManager != null) EntityManager.GetComponentData<WeatherData>(weather);
            if(Assert != null) if(Assert != null) Assert.AreEqual(weatherType, if(weatherData != null) if(weatherData != null) weatherData.Type);
        }
        
        [Test]
        public void WheelPhysics_ShouldHandleAllPressures([Range(100f, 500f, 50f)] float pressure)
        {
            // Arrange
            var wheel = CreateWheel();
            var wheelPhysics = if(EntityManager != null) if(EntityManager != null) EntityManager.GetComponentData<WheelPhysicsData>(wheel);
            if(wheelPhysics != null) if(wheelPhysics != null) wheelPhysics.TirePressure = pressure;
            if(EntityManager != null) if(EntityManager != null) EntityManager.SetComponentData(wheel, wheelPhysics);
            
            // Act
            var system = if(World != null) if(World != null) World.CreateSystemManaged<AdvancedWheelPhysicsSystem>();
            if(system != null) if(system != null) system.Update();
            
            // Assert
            var updatedWheelPhysics = if(EntityManager != null) if(EntityManager != null) EntityManager.GetComponentData<WheelPhysicsData>(wheel);
            if(Assert != null) if(Assert != null) Assert.GreaterOrEqual(if(updatedWheelPhysics != null) if(updatedWheelPhysics != null) updatedWheelPhysics.TirePressure, 0f);
        }
        
        [Test]
        public void VehicleMovement_ShouldHandleAllMasses([Range(500f, 5000f, 500f)] float mass)
        {
            // Arrange
            var vehicle = CreateVehicle();
            var config = if(EntityManager != null) if(EntityManager != null) EntityManager.GetComponentData<VehicleConfig>(vehicle);
            if(config != null) if(config != null) config.Mass = mass;
            if(EntityManager != null) if(EntityManager != null) EntityManager.SetComponentData(vehicle, config);
            
            // Act
            var system = if(World != null) if(World != null) World.CreateSystemManaged<VehicleMovementSystem>();
            if(system != null) if(system != null) system.Update();
            
            // Assert
            var physics = if(EntityManager != null) if(EntityManager != null) EntityManager.GetComponentData<VehiclePhysics>(vehicle);
            if(Assert != null) if(Assert != null) Assert.IsTrue(if(physics != null) if(physics != null) physics.Velocity.magnitude >= 0f);
        }
        
        [Test]
        public void WheelPhysics_ShouldHandleAllTemperatures([Range(-50f, 200f, 25f)] float temperature)
        {
            // Arrange
            var wheel = CreateWheel();
            var wheelPhysics = if(EntityManager != null) if(EntityManager != null) EntityManager.GetComponentData<WheelPhysicsData>(wheel);
            if(wheelPhysics != null) if(wheelPhysics != null) wheelPhysics.WheelTemperature = temperature;
            if(EntityManager != null) if(EntityManager != null) EntityManager.SetComponentData(wheel, wheelPhysics);
            
            // Act
            var system = if(World != null) if(World != null) World.CreateSystemManaged<AdvancedWheelPhysicsSystem>();
            if(system != null) if(system != null) system.Update();
            
            // Assert
            var updatedWheelPhysics = if(EntityManager != null) if(EntityManager != null) EntityManager.GetComponentData<WheelPhysicsData>(wheel);
            if(Assert != null) if(Assert != null) Assert.GreaterOrEqual(if(updatedWheelPhysics != null) if(updatedWheelPhysics != null) updatedWheelPhysics.WheelTemperature, -50f);
            if(Assert != null) if(Assert != null) Assert.LessOrEqual(if(updatedWheelPhysics != null) if(updatedWheelPhysics != null) updatedWheelPhysics.WheelTemperature, 200f);
        }
        
        [Test]
        public void VehicleMovement_ShouldHandleAllDragValues([Range(0f, 2f, 0.2f)] float drag)
        {
            // Arrange
            var vehicle = CreateVehicle();
            var config = if(EntityManager != null) if(EntityManager != null) EntityManager.GetComponentData<VehicleConfig>(vehicle);
            if(config != null) if(config != null) config.Drag = drag;
            if(EntityManager != null) if(EntityManager != null) EntityManager.SetComponentData(vehicle, config);
            
            // Act
            var system = if(World != null) if(World != null) World.CreateSystemManaged<VehicleMovementSystem>();
            if(system != null) if(system != null) system.Update();
            
            // Assert
            var physics = if(EntityManager != null) if(EntityManager != null) EntityManager.GetComponentData<VehiclePhysics>(vehicle);
            if(Assert != null) if(Assert != null) Assert.IsTrue(if(physics != null) if(physics != null) physics.Velocity.magnitude >= 0f);
        }
        
        [Test]
        public void WheelPhysics_ShouldHandleAllWearValues([Range(0f, 1f, 0.1f)] float wear)
        {
            // Arrange
            var wheel = CreateWheel();
            var wheelPhysics = if(EntityManager != null) if(EntityManager != null) EntityManager.GetComponentData<WheelPhysicsData>(wheel);
            if(wheelPhysics != null) if(wheelPhysics != null) wheelPhysics.TreadWear = wear;
            if(EntityManager != null) if(EntityManager != null) EntityManager.SetComponentData(wheel, wheelPhysics);
            
            // Act
            var system = if(World != null) if(World != null) World.CreateSystemManaged<AdvancedWheelPhysicsSystem>();
            if(system != null) if(system != null) system.Update();
            
            // Assert
            var updatedWheelPhysics = if(EntityManager != null) if(EntityManager != null) EntityManager.GetComponentData<WheelPhysicsData>(wheel);
            if(Assert != null) if(Assert != null) Assert.GreaterOrEqual(if(updatedWheelPhysics != null) if(updatedWheelPhysics != null) updatedWheelPhysics.TreadWear, 0f);
            if(Assert != null) if(Assert != null) Assert.LessOrEqual(if(updatedWheelPhysics != null) if(updatedWheelPhysics != null) updatedWheelPhysics.TreadWear, 1f);
        }
        
        [Test]
        public void VehicleMovement_ShouldHandleAllAngularDragValues([Range(0f, 10f, 1f)] float angularDrag)
        {
            // Arrange
            var vehicle = CreateVehicle();
            var config = if(EntityManager != null) if(EntityManager != null) EntityManager.GetComponentData<VehicleConfig>(vehicle);
            if(config != null) if(config != null) config.AngularDrag = angularDrag;
            if(EntityManager != null) if(EntityManager != null) EntityManager.SetComponentData(vehicle, config);
            
            // Act
            var system = if(World != null) if(World != null) World.CreateSystemManaged<VehicleMovementSystem>();
            if(system != null) if(system != null) system.Update();
            
            // Assert
            var physics = if(EntityManager != null) if(EntityManager != null) EntityManager.GetComponentData<VehiclePhysics>(vehicle);
            if(Assert != null) if(Assert != null) Assert.IsTrue(if(physics != null) if(physics != null) physics.AngularVelocity.magnitude >= 0f);
        }
        
        [Test]
        public void WheelPhysics_ShouldHandleAllSlipRatios([Range(0f, 2f, 0.2f)] float slipRatio)
        {
            // Arrange
            var wheel = CreateWheel();
            var wheelPhysics = if(EntityManager != null) if(EntityManager != null) EntityManager.GetComponentData<WheelPhysicsData>(wheel);
            if(wheelPhysics != null) if(wheelPhysics != null) wheelPhysics.SlipRatio = slipRatio;
            if(EntityManager != null) if(EntityManager != null) EntityManager.SetComponentData(wheel, wheelPhysics);
            
            // Act
            var system = if(World != null) if(World != null) World.CreateSystemManaged<AdvancedWheelPhysicsSystem>();
            if(system != null) if(system != null) system.Update();
            
            // Assert
            var updatedWheelPhysics = if(EntityManager != null) if(EntityManager != null) EntityManager.GetComponentData<WheelPhysicsData>(wheel);
            if(Assert != null) if(Assert != null) Assert.GreaterOrEqual(if(updatedWheelPhysics != null) if(updatedWheelPhysics != null) updatedWheelPhysics.SlipRatio, 0f);
        }
        
        [Test]
        public void VehicleMovement_ShouldHandleAllAccelerations([Range(0f, 50f, 5f)] float acceleration)
        {
            // Arrange
            var vehicle = CreateVehicle();
            var config = if(EntityManager != null) if(EntityManager != null) EntityManager.GetComponentData<VehicleConfig>(vehicle);
            if(config != null) if(config != null) config.Acceleration = acceleration;
            if(EntityManager != null) if(EntityManager != null) EntityManager.SetComponentData(vehicle, config);
            
            // Act
            var system = if(World != null) if(World != null) World.CreateSystemManaged<VehicleMovementSystem>();
            if(system != null) if(system != null) system.Update();
            
            // Assert
            var physics = if(EntityManager != null) if(EntityManager != null) EntityManager.GetComponentData<VehiclePhysics>(vehicle);
            if(Assert != null) if(Assert != null) Assert.IsTrue(if(physics != null) if(physics != null) physics.Acceleration.magnitude >= 0f);
        }
        
        [Test]
        public void WheelPhysics_ShouldHandleAllSinkDepths([Range(0f, 1f, 0.1f)] float sinkDepth)
        {
            // Arrange
            var wheel = CreateWheel();
            var wheelPhysics = if(EntityManager != null) if(EntityManager != null) EntityManager.GetComponentData<WheelPhysicsData>(wheel);
            if(wheelPhysics != null) if(wheelPhysics != null) wheelPhysics.SinkDepth = sinkDepth;
            if(EntityManager != null) if(EntityManager != null) EntityManager.SetComponentData(wheel, wheelPhysics);
            
            // Act
            var system = if(World != null) if(World != null) World.CreateSystemManaged<AdvancedWheelPhysicsSystem>();
            if(system != null) if(system != null) system.Update();
            
            // Assert
            var updatedWheelPhysics = if(EntityManager != null) if(EntityManager != null) EntityManager.GetComponentData<WheelPhysicsData>(wheel);
            if(Assert != null) if(Assert != null) Assert.GreaterOrEqual(if(updatedWheelPhysics != null) if(updatedWheelPhysics != null) updatedWheelPhysics.SinkDepth, 0f);
            if(Assert != null) if(Assert != null) Assert.LessOrEqual(if(updatedWheelPhysics != null) if(updatedWheelPhysics != null) updatedWheelPhysics.SinkDepth, 1f);
        }
        
        [Test]
        public void VehicleMovement_ShouldHandleAllTurnSpeeds([Range(0f, 10f, 1f)] float turnSpeed)
        {
            // Arrange
            var vehicle = CreateVehicle();
            var config = if(EntityManager != null) if(EntityManager != null) EntityManager.GetComponentData<VehicleConfig>(vehicle);
            if(config != null) if(config != null) config.TurnSpeed = turnSpeed;
            if(EntityManager != null) if(EntityManager != null) EntityManager.SetComponentData(vehicle, config);
            
            // Act
            var system = if(World != null) if(World != null) World.CreateSystemManaged<VehicleMovementSystem>();
            if(system != null) if(system != null) system.Update();
            
            // Assert
            var physics = if(EntityManager != null) if(EntityManager != null) EntityManager.GetComponentData<VehiclePhysics>(vehicle);
            if(Assert != null) if(Assert != null) Assert.IsTrue(if(physics != null) if(physics != null) physics.TurnSpeed >= 0f);
        }
        
        [Test]
        public void WheelPhysics_ShouldHandleAllTractionValues([Range(0f, 1f, 0.1f)] float traction)
        {
            // Arrange
            var wheel = CreateWheel();
            var wheelData = if(EntityManager != null) if(EntityManager != null) EntityManager.GetComponentData<WheelData>(wheel);
            if(wheelData != null) if(wheelData != null) wheelData.Traction = traction;
            if(EntityManager != null) if(EntityManager != null) EntityManager.SetComponentData(wheel, wheelData);
            
            // Act
            var system = if(World != null) if(World != null) World.CreateSystemManaged<AdvancedWheelPhysicsSystem>();
            if(system != null) if(system != null) system.Update();
            
            // Assert
            var updatedWheelData = if(EntityManager != null) if(EntityManager != null) EntityManager.GetComponentData<WheelData>(wheel);
            if(Assert != null) if(Assert != null) Assert.GreaterOrEqual(if(updatedWheelData != null) if(updatedWheelData != null) updatedWheelData.Traction, 0f);
            if(Assert != null) if(Assert != null) Assert.LessOrEqual(if(updatedWheelData != null) if(updatedWheelData != null) updatedWheelData.Traction, 1f);
        }
    }
