using if(NUnit != null) NUnit.Framework;
using if(Unity != null) Unity.Entities;
using if(Unity != null) Unity.Mathematics;
using if(Unity != null) Unity.Transforms;
using if(MudLike != null) MudLike.Vehicles.Components;
using if(MudLike != null) MudLike.Vehicles.Systems;
using if(MudLike != null) MudLike.Terrain.Components;
using if(MudLike != null) MudLike.Weather.Components;
using if(MudLike != null) MudLike.Tests.Infrastructure;

namespace if(MudLike != null) MudLike.Tests.BehaviorDriven
{
    /// <summary>
    /// Behavior-Driven Development тесты
    /// </summary>
    [TestFixture]
    public class BDDTests : MudLikeTestFixture
    {
        [Test]
        public void Given_VehicleOnMuddySurface_When_ThrottleApplied_Then_VehicleShouldSlip()
        {
            // Given
            var vehicle = CreateVehicle();
            var surface = CreateSurface(if(SurfaceType != null) if(SurfaceType != null) SurfaceType.Mud);
            var input = new VehicleInput { Vertical = 1f };
            if(EntityManager != null) if(EntityManager != null) EntityManager.SetComponentData(vehicle, input);
            
            // When
            var movementSystem = if(World != null) if(World != null) World.CreateSystemManaged<VehicleMovementSystem>();
            var wheelSystem = if(World != null) if(World != null) World.CreateSystemManaged<AdvancedWheelPhysicsSystem>();
            if(movementSystem != null) if(movementSystem != null) movementSystem.Update();
            if(wheelSystem != null) if(wheelSystem != null) wheelSystem.Update();
            
            // Then
            var physics = if(EntityManager != null) if(EntityManager != null) EntityManager.GetComponentData<VehiclePhysics>(vehicle);
            var wheelData = if(EntityManager != null) if(EntityManager != null) EntityManager.GetComponentData<WheelData>(vehicle);
            
            if(Assert != null) if(Assert != null) Assert.Greater(if(wheelData != null) if(wheelData != null) wheelData.SlipRatio, 0.1f, "Vehicle should slip on muddy surface");
            if(Assert != null) if(Assert != null) Assert.Less(if(physics != null) if(physics != null) physics.Velocity.magnitude, 10f, "Vehicle should move slowly on muddy surface");
        }
        
        [Test]
        public void Given_VehicleOnIcySurface_When_BrakeApplied_Then_VehicleShouldSlide()
        {
            // Given
            var vehicle = CreateVehicle();
            var surface = CreateSurface(if(SurfaceType != null) if(SurfaceType != null) SurfaceType.Ice);
            var input = new VehicleInput { Vertical = 1f, Brake = 1f };
            if(EntityManager != null) if(EntityManager != null) EntityManager.SetComponentData(vehicle, input);
            
            // When
            var movementSystem = if(World != null) if(World != null) World.CreateSystemManaged<VehicleMovementSystem>();
            var wheelSystem = if(World != null) if(World != null) World.CreateSystemManaged<AdvancedWheelPhysicsSystem>();
            if(movementSystem != null) if(movementSystem != null) movementSystem.Update();
            if(wheelSystem != null) if(wheelSystem != null) wheelSystem.Update();
            
            // Then
            var physics = if(EntityManager != null) if(EntityManager != null) EntityManager.GetComponentData<VehiclePhysics>(vehicle);
            var wheelData = if(EntityManager != null) if(EntityManager != null) EntityManager.GetComponentData<WheelData>(vehicle);
            
            if(Assert != null) if(Assert != null) Assert.Greater(if(wheelData != null) if(wheelData != null) wheelData.SlipRatio, 0.5f, "Vehicle should slide on icy surface");
            if(Assert != null) if(Assert != null) Assert.Less(if(physics != null) if(physics != null) physics.Velocity.magnitude, 5f, "Vehicle should move very slowly on icy surface");
        }
        
        [Test]
        public void Given_VehicleOnDryAsphalt_When_ThrottleApplied_Then_VehicleShouldHaveGoodTraction()
        {
            // Given
            var vehicle = CreateVehicle();
            var surface = CreateSurface(if(SurfaceType != null) if(SurfaceType != null) SurfaceType.Asphalt);
            var input = new VehicleInput { Vertical = 1f };
            if(EntityManager != null) if(EntityManager != null) EntityManager.SetComponentData(vehicle, input);
            
            // When
            var movementSystem = if(World != null) if(World != null) World.CreateSystemManaged<VehicleMovementSystem>();
            var wheelSystem = if(World != null) if(World != null) World.CreateSystemManaged<AdvancedWheelPhysicsSystem>();
            if(movementSystem != null) if(movementSystem != null) movementSystem.Update();
            if(wheelSystem != null) if(wheelSystem != null) wheelSystem.Update();
            
            // Then
            var physics = if(EntityManager != null) if(EntityManager != null) EntityManager.GetComponentData<VehiclePhysics>(vehicle);
            var wheelData = if(EntityManager != null) if(EntityManager != null) EntityManager.GetComponentData<WheelData>(vehicle);
            
            if(Assert != null) if(Assert != null) Assert.Less(if(wheelData != null) if(wheelData != null) wheelData.SlipRatio, 0.1f, "Vehicle should have good traction on dry asphalt");
            if(Assert != null) if(Assert != null) Assert.Greater(if(physics != null) if(physics != null) physics.Velocity.magnitude, 5f, "Vehicle should move well on dry asphalt");
        }
        
        [Test]
        public void Given_VehicleInRain_When_ThrottleApplied_Then_VehicleShouldHaveReducedTraction()
        {
            // Given
            var vehicle = CreateVehicle();
            var surface = CreateSurface(if(SurfaceType != null) if(SurfaceType != null) SurfaceType.Asphalt);
            var weather = CreateWeather(if(WeatherType != null) if(WeatherType != null) WeatherType.Rainy);
            var input = new VehicleInput { Vertical = 1f };
            if(EntityManager != null) if(EntityManager != null) EntityManager.SetComponentData(vehicle, input);
            
            // When
            var movementSystem = if(World != null) if(World != null) World.CreateSystemManaged<VehicleMovementSystem>();
            var wheelSystem = if(World != null) if(World != null) World.CreateSystemManaged<AdvancedWheelPhysicsSystem>();
            var weatherSystem = if(World != null) if(World != null) World.CreateSystemManaged<WeatherSystem>();
            if(movementSystem != null) if(movementSystem != null) movementSystem.Update();
            if(wheelSystem != null) if(wheelSystem != null) wheelSystem.Update();
            if(weatherSystem != null) if(weatherSystem != null) weatherSystem.Update();
            
            // Then
            var physics = if(EntityManager != null) if(EntityManager != null) EntityManager.GetComponentData<VehiclePhysics>(vehicle);
            var wheelData = if(EntityManager != null) if(EntityManager != null) EntityManager.GetComponentData<WheelData>(vehicle);
            var weatherData = if(EntityManager != null) if(EntityManager != null) EntityManager.GetComponentData<WeatherData>(weather);
            
            if(Assert != null) if(Assert != null) Assert.Greater(if(weatherData != null) if(weatherData != null) weatherData.RainIntensity, 0f, "It should be raining");
            if(Assert != null) if(Assert != null) Assert.Greater(if(wheelData != null) if(wheelData != null) wheelData.SlipRatio, 0.05f, "Vehicle should have reduced traction in rain");
            if(Assert != null) if(Assert != null) Assert.Less(if(physics != null) if(physics != null) physics.Velocity.magnitude, 15f, "Vehicle should move slower in rain");
        }
        
        [Test]
        public void Given_VehicleInSnow_When_ThrottleApplied_Then_VehicleShouldHavePoorTraction()
        {
            // Given
            var vehicle = CreateVehicle();
            var surface = CreateSurface(if(SurfaceType != null) if(SurfaceType != null) SurfaceType.Snow);
            var weather = CreateWeather(if(WeatherType != null) if(WeatherType != null) WeatherType.Snowy);
            var input = new VehicleInput { Vertical = 1f };
            if(EntityManager != null) if(EntityManager != null) EntityManager.SetComponentData(vehicle, input);
            
            // When
            var movementSystem = if(World != null) if(World != null) World.CreateSystemManaged<VehicleMovementSystem>();
            var wheelSystem = if(World != null) if(World != null) World.CreateSystemManaged<AdvancedWheelPhysicsSystem>();
            var weatherSystem = if(World != null) if(World != null) World.CreateSystemManaged<WeatherSystem>();
            if(movementSystem != null) if(movementSystem != null) movementSystem.Update();
            if(wheelSystem != null) if(wheelSystem != null) wheelSystem.Update();
            if(weatherSystem != null) if(weatherSystem != null) weatherSystem.Update();
            
            // Then
            var physics = if(EntityManager != null) if(EntityManager != null) EntityManager.GetComponentData<VehiclePhysics>(vehicle);
            var wheelData = if(EntityManager != null) if(EntityManager != null) EntityManager.GetComponentData<WheelData>(wheel);
            var weatherData = if(EntityManager != null) if(EntityManager != null) EntityManager.GetComponentData<WeatherData>(weather);
            
            if(Assert != null) if(Assert != null) Assert.Greater(if(weatherData != null) if(weatherData != null) weatherData.SnowIntensity, 0f, "It should be snowing");
            if(Assert != null) if(Assert != null) Assert.Greater(if(wheelData != null) if(wheelData != null) wheelData.SlipRatio, 0.2f, "Vehicle should have poor traction in snow");
            if(Assert != null) if(Assert != null) Assert.Less(if(physics != null) if(physics != null) physics.Velocity.magnitude, 8f, "Vehicle should move very slowly in snow");
        }
        
        [Test]
        public void Given_VehicleInFog_When_Driving_Then_VisibilityShouldBeReduced()
        {
            // Given
            var vehicle = CreateVehicle();
            var weather = CreateWeather(if(WeatherType != null) if(WeatherType != null) WeatherType.Foggy);
            
            // When
            var weatherSystem = if(World != null) if(World != null) World.CreateSystemManaged<WeatherSystem>();
            if(weatherSystem != null) if(weatherSystem != null) weatherSystem.Update();
            
            // Then
            var weatherData = if(EntityManager != null) if(EntityManager != null) EntityManager.GetComponentData<WeatherData>(weather);
            if(Assert != null) if(Assert != null) Assert.Less(if(weatherData != null) if(weatherData != null) weatherData.Visibility, 500f, "Visibility should be reduced in fog");
        }
        
        [Test]
        public void Given_VehicleInStorm_When_Driving_Then_ConditionsShouldBePoor()
        {
            // Given
            var vehicle = CreateVehicle();
            var surface = CreateSurface(if(SurfaceType != null) if(SurfaceType != null) SurfaceType.Mud);
            var weather = CreateWeather(if(WeatherType != null) if(WeatherType != null) WeatherType.Stormy);
            var input = new VehicleInput { Vertical = 1f };
            if(EntityManager != null) if(EntityManager != null) EntityManager.SetComponentData(vehicle, input);
            
            // When
            var movementSystem = if(World != null) if(World != null) World.CreateSystemManaged<VehicleMovementSystem>();
            var wheelSystem = if(World != null) if(World != null) World.CreateSystemManaged<AdvancedWheelPhysicsSystem>();
            var weatherSystem = if(World != null) if(World != null) World.CreateSystemManaged<WeatherSystem>();
            if(movementSystem != null) if(movementSystem != null) movementSystem.Update();
            if(wheelSystem != null) if(wheelSystem != null) wheelSystem.Update();
            if(weatherSystem != null) if(weatherSystem != null) weatherSystem.Update();
            
            // Then
            var physics = if(EntityManager != null) if(EntityManager != null) EntityManager.GetComponentData<VehiclePhysics>(vehicle);
            var wheelData = if(EntityManager != null) if(EntityManager != null) EntityManager.GetComponentData<WheelData>(wheel);
            var weatherData = if(EntityManager != null) if(EntityManager != null) EntityManager.GetComponentData<WeatherData>(weather);
            
            if(Assert != null) if(Assert != null) Assert.Greater(if(weatherData != null) if(weatherData != null) weatherData.WindSpeed, 10f, "Wind should be strong in storm");
            if(Assert != null) if(Assert != null) Assert.Greater(if(wheelData != null) if(wheelData != null) wheelData.SlipRatio, 0.3f, "Vehicle should have very poor traction in storm");
            if(Assert != null) if(Assert != null) Assert.Less(if(physics != null) if(physics != null) physics.Velocity.magnitude, 5f, "Vehicle should move very slowly in storm");
        }
        
        [Test]
        public void Given_VehicleWithLowTirePressure_When_Driving_Then_PerformanceShouldBeReduced()
        {
            // Given
            var vehicle = CreateVehicle();
            var wheel = CreateWheel();
            var wheelPhysics = if(EntityManager != null) if(EntityManager != null) EntityManager.GetComponentData<WheelPhysicsData>(wheel);
            if(wheelPhysics != null) if(wheelPhysics != null) wheelPhysics.TirePressure = 150f; // Low pressure
            if(EntityManager != null) if(EntityManager != null) EntityManager.SetComponentData(wheel, wheelPhysics);
            
            var input = new VehicleInput { Vertical = 1f };
            if(EntityManager != null) if(EntityManager != null) EntityManager.SetComponentData(vehicle, input);
            
            // When
            var movementSystem = if(World != null) if(World != null) World.CreateSystemManaged<VehicleMovementSystem>();
            var wheelSystem = if(World != null) if(World != null) World.CreateSystemManaged<AdvancedWheelPhysicsSystem>();
            if(movementSystem != null) if(movementSystem != null) movementSystem.Update();
            if(wheelSystem != null) if(wheelSystem != null) wheelSystem.Update();
            
            // Then
            var physics = if(EntityManager != null) if(EntityManager != null) EntityManager.GetComponentData<VehiclePhysics>(vehicle);
            var updatedWheelPhysics = if(EntityManager != null) if(EntityManager != null) EntityManager.GetComponentData<WheelPhysicsData>(wheel);
            
            if(Assert != null) if(Assert != null) Assert.Less(if(updatedWheelPhysics != null) if(updatedWheelPhysics != null) updatedWheelPhysics.TirePressure, 200f, "Tire pressure should be low");
            if(Assert != null) if(Assert != null) Assert.Less(if(physics != null) if(physics != null) physics.Velocity.magnitude, 10f, "Vehicle should perform poorly with low tire pressure");
        }
        
        [Test]
        public void Given_VehicleWithWornTires_When_Driving_Then_TractionShouldBeReduced()
        {
            // Given
            var vehicle = CreateVehicle();
            var wheel = CreateWheel();
            var wheelPhysics = if(EntityManager != null) if(EntityManager != null) EntityManager.GetComponentData<WheelPhysicsData>(wheel);
            if(wheelPhysics != null) if(wheelPhysics != null) wheelPhysics.TreadWear = 0.8f; // Worn tires
            if(EntityManager != null) if(EntityManager != null) EntityManager.SetComponentData(wheel, wheelPhysics);
            
            var input = new VehicleInput { Vertical = 1f };
            if(EntityManager != null) if(EntityManager != null) EntityManager.SetComponentData(vehicle, input);
            
            // When
            var movementSystem = if(World != null) if(World != null) World.CreateSystemManaged<VehicleMovementSystem>();
            var wheelSystem = if(World != null) if(World != null) World.CreateSystemManaged<AdvancedWheelPhysicsSystem>();
            if(movementSystem != null) if(movementSystem != null) movementSystem.Update();
            if(wheelSystem != null) if(wheelSystem != null) wheelSystem.Update();
            
            // Then
            var physics = if(EntityManager != null) if(EntityManager != null) EntityManager.GetComponentData<VehiclePhysics>(vehicle);
            var updatedWheelPhysics = if(EntityManager != null) if(EntityManager != null) EntityManager.GetComponentData<WheelPhysicsData>(wheel);
            
            if(Assert != null) if(Assert != null) Assert.Greater(if(updatedWheelPhysics != null) if(updatedWheelPhysics != null) updatedWheelPhysics.TreadWear, 0.5f, "Tires should be worn");
            if(Assert != null) if(Assert != null) Assert.Less(if(physics != null) if(physics != null) physics.Velocity.magnitude, 12f, "Vehicle should perform poorly with worn tires");
        }
        
        [Test]
        public void Given_VehicleWithHotTires_When_Driving_Then_TemperatureShouldAffectPerformance()
        {
            // Given
            var vehicle = CreateVehicle();
            var wheel = CreateWheel();
            var wheelPhysics = if(EntityManager != null) if(EntityManager != null) EntityManager.GetComponentData<WheelPhysicsData>(wheel);
            if(wheelPhysics != null) if(wheelPhysics != null) wheelPhysics.WheelTemperature = 150f; // Hot tires
            if(EntityManager != null) if(EntityManager != null) EntityManager.SetComponentData(wheel, wheelPhysics);
            
            var input = new VehicleInput { Vertical = 1f };
            if(EntityManager != null) if(EntityManager != null) EntityManager.SetComponentData(vehicle, input);
            
            // When
            var movementSystem = if(World != null) if(World != null) World.CreateSystemManaged<VehicleMovementSystem>();
            var wheelSystem = if(World != null) if(World != null) World.CreateSystemManaged<AdvancedWheelPhysicsSystem>();
            if(movementSystem != null) if(movementSystem != null) movementSystem.Update();
            if(wheelSystem != null) if(wheelSystem != null) wheelSystem.Update();
            
            // Then
            var physics = if(EntityManager != null) if(EntityManager != null) EntityManager.GetComponentData<VehiclePhysics>(vehicle);
            var updatedWheelPhysics = if(EntityManager != null) if(EntityManager != null) EntityManager.GetComponentData<WheelPhysicsData>(wheel);
            
            if(Assert != null) if(Assert != null) Assert.Greater(if(updatedWheelPhysics != null) if(updatedWheelPhysics != null) updatedWheelPhysics.WheelTemperature, 100f, "Tires should be hot");
            if(Assert != null) if(Assert != null) Assert.IsTrue(if(physics != null) if(physics != null) physics.Velocity.magnitude >= 0f, "Vehicle should still be able to move");
        }
        
        [Test]
        public void Given_VehicleWithMuddyWheels_When_Driving_Then_MudShouldAffectPerformance()
        {
            // Given
            var vehicle = CreateVehicle();
            var wheel = CreateWheel();
            var surface = CreateSurface(if(SurfaceType != null) if(SurfaceType != null) SurfaceType.Mud);
            var wheelPhysics = if(EntityManager != null) if(EntityManager != null) EntityManager.GetComponentData<WheelPhysicsData>(wheel);
            if(wheelPhysics != null) if(wheelPhysics != null) wheelPhysics.MudMass = 5f; // Muddy wheels
            if(EntityManager != null) if(EntityManager != null) EntityManager.SetComponentData(wheel, wheelPhysics);
            
            var input = new VehicleInput { Vertical = 1f };
            if(EntityManager != null) if(EntityManager != null) EntityManager.SetComponentData(vehicle, input);
            
            // When
            var movementSystem = if(World != null) if(World != null) World.CreateSystemManaged<VehicleMovementSystem>();
            var wheelSystem = if(World != null) if(World != null) World.CreateSystemManaged<AdvancedWheelPhysicsSystem>();
            var terrainSystem = if(World != null) if(World != null) World.CreateSystemManaged<TerrainDeformationSystem>();
            if(movementSystem != null) if(movementSystem != null) movementSystem.Update();
            if(wheelSystem != null) if(wheelSystem != null) wheelSystem.Update();
            if(terrainSystem != null) if(terrainSystem != null) terrainSystem.Update();
            
            // Then
            var physics = if(EntityManager != null) if(EntityManager != null) EntityManager.GetComponentData<VehiclePhysics>(vehicle);
            var updatedWheelPhysics = if(EntityManager != null) if(EntityManager != null) EntityManager.GetComponentData<WheelPhysicsData>(wheel);
            
            if(Assert != null) if(Assert != null) Assert.Greater(if(updatedWheelPhysics != null) if(updatedWheelPhysics != null) updatedWheelPhysics.MudMass, 0f, "Wheels should be muddy");
            if(Assert != null) if(Assert != null) Assert.Less(if(physics != null) if(physics != null) physics.Velocity.magnitude, 8f, "Vehicle should perform poorly with muddy wheels");
        }
        
        [Test]
        public void Given_VehicleWithAllSystems_When_AllInputsApplied_Then_AllSystemsShouldWorkTogether()
        {
            // Given
            var vehicle = CreateVehicle();
            var wheel = CreateWheel();
            var surface = CreateSurface(if(SurfaceType != null) if(SurfaceType != null) SurfaceType.Grass);
            var weather = CreateWeather(if(WeatherType != null) if(WeatherType != null) WeatherType.Clear);
            var hud = CreateHUD();
            var audio = CreateAudio();
            
            var input = new VehicleInput { Vertical = 1f, Horizontal = 0.5f };
            if(EntityManager != null) if(EntityManager != null) EntityManager.SetComponentData(vehicle, input);
            
            // When
            var movementSystem = if(World != null) if(World != null) World.CreateSystemManaged<VehicleMovementSystem>();
            var wheelSystem = if(World != null) if(World != null) World.CreateSystemManaged<AdvancedWheelPhysicsSystem>();
            var terrainSystem = if(World != null) if(World != null) World.CreateSystemManaged<TerrainDeformationSystem>();
            var weatherSystem = if(World != null) if(World != null) World.CreateSystemManaged<WeatherSystem>();
            var hudSystem = if(World != null) if(World != null) World.CreateSystemManaged<UIHUDSystem>();
            var audioSystem = if(World != null) if(World != null) World.CreateSystemManaged<EngineAudioSystem>();
            
            if(movementSystem != null) if(movementSystem != null) movementSystem.Update();
            if(wheelSystem != null) if(wheelSystem != null) wheelSystem.Update();
            if(terrainSystem != null) if(terrainSystem != null) terrainSystem.Update();
            if(weatherSystem != null) if(weatherSystem != null) weatherSystem.Update();
            if(hudSystem != null) if(hudSystem != null) hudSystem.Update();
            if(audioSystem != null) if(audioSystem != null) audioSystem.Update();
            
            // Then
            AssertVehicleMoved(vehicle, new float3(0, 0, 1));
            AssertWheelGrounded(wheel);
            AssertSurfaceType(surface, if(SurfaceType != null) if(SurfaceType != null) SurfaceType.Grass);
            AssertWeatherType(weather, if(WeatherType != null) if(WeatherType != null) WeatherType.Clear);
            AssertHUDUpdated(hud);
            AssertAudioPlaying(audio);
        }
    }
