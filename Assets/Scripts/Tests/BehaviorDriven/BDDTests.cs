using NUnit.Framework;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using MudLike.Vehicles.Components;
using MudLike.Vehicles.Systems;
using MudLike.Terrain.Components;
using MudLike.Weather.Components;
using MudLike.Tests.Infrastructure;

namespace MudLike.Tests.BehaviorDriven
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
            var surface = CreateSurface(SurfaceType.Mud);
            var input = new VehicleInput { Vertical = 1f };
            EntityManager.SetComponentData(vehicle, input);
            
            // When
            var movementSystem = World.CreateSystemManaged<VehicleMovementSystem>();
            var wheelSystem = World.CreateSystemManaged<AdvancedWheelPhysicsSystem>();
            movementSystem.Update();
            wheelSystem.Update();
            
            // Then
            var physics = EntityManager.GetComponentData<VehiclePhysics>(vehicle);
            var wheelData = EntityManager.GetComponentData<WheelData>(vehicle);
            
            Assert.Greater(wheelData.SlipRatio, 0.1f, "Vehicle should slip on muddy surface");
            Assert.Less(physics.Velocity.magnitude, 10f, "Vehicle should move slowly on muddy surface");
        }
        
        [Test]
        public void Given_VehicleOnIcySurface_When_BrakeApplied_Then_VehicleShouldSlide()
        {
            // Given
            var vehicle = CreateVehicle();
            var surface = CreateSurface(SurfaceType.Ice);
            var input = new VehicleInput { Vertical = 1f, Brake = 1f };
            EntityManager.SetComponentData(vehicle, input);
            
            // When
            var movementSystem = World.CreateSystemManaged<VehicleMovementSystem>();
            var wheelSystem = World.CreateSystemManaged<AdvancedWheelPhysicsSystem>();
            movementSystem.Update();
            wheelSystem.Update();
            
            // Then
            var physics = EntityManager.GetComponentData<VehiclePhysics>(vehicle);
            var wheelData = EntityManager.GetComponentData<WheelData>(vehicle);
            
            Assert.Greater(wheelData.SlipRatio, 0.5f, "Vehicle should slide on icy surface");
            Assert.Less(physics.Velocity.magnitude, 5f, "Vehicle should move very slowly on icy surface");
        }
        
        [Test]
        public void Given_VehicleOnDryAsphalt_When_ThrottleApplied_Then_VehicleShouldHaveGoodTraction()
        {
            // Given
            var vehicle = CreateVehicle();
            var surface = CreateSurface(SurfaceType.Asphalt);
            var input = new VehicleInput { Vertical = 1f };
            EntityManager.SetComponentData(vehicle, input);
            
            // When
            var movementSystem = World.CreateSystemManaged<VehicleMovementSystem>();
            var wheelSystem = World.CreateSystemManaged<AdvancedWheelPhysicsSystem>();
            movementSystem.Update();
            wheelSystem.Update();
            
            // Then
            var physics = EntityManager.GetComponentData<VehiclePhysics>(vehicle);
            var wheelData = EntityManager.GetComponentData<WheelData>(vehicle);
            
            Assert.Less(wheelData.SlipRatio, 0.1f, "Vehicle should have good traction on dry asphalt");
            Assert.Greater(physics.Velocity.magnitude, 5f, "Vehicle should move well on dry asphalt");
        }
        
        [Test]
        public void Given_VehicleInRain_When_ThrottleApplied_Then_VehicleShouldHaveReducedTraction()
        {
            // Given
            var vehicle = CreateVehicle();
            var surface = CreateSurface(SurfaceType.Asphalt);
            var weather = CreateWeather(WeatherType.Rainy);
            var input = new VehicleInput { Vertical = 1f };
            EntityManager.SetComponentData(vehicle, input);
            
            // When
            var movementSystem = World.CreateSystemManaged<VehicleMovementSystem>();
            var wheelSystem = World.CreateSystemManaged<AdvancedWheelPhysicsSystem>();
            var weatherSystem = World.CreateSystemManaged<WeatherSystem>();
            movementSystem.Update();
            wheelSystem.Update();
            weatherSystem.Update();
            
            // Then
            var physics = EntityManager.GetComponentData<VehiclePhysics>(vehicle);
            var wheelData = EntityManager.GetComponentData<WheelData>(vehicle);
            var weatherData = EntityManager.GetComponentData<WeatherData>(weather);
            
            Assert.Greater(weatherData.RainIntensity, 0f, "It should be raining");
            Assert.Greater(wheelData.SlipRatio, 0.05f, "Vehicle should have reduced traction in rain");
            Assert.Less(physics.Velocity.magnitude, 15f, "Vehicle should move slower in rain");
        }
        
        [Test]
        public void Given_VehicleInSnow_When_ThrottleApplied_Then_VehicleShouldHavePoorTraction()
        {
            // Given
            var vehicle = CreateVehicle();
            var surface = CreateSurface(SurfaceType.Snow);
            var weather = CreateWeather(WeatherType.Snowy);
            var input = new VehicleInput { Vertical = 1f };
            EntityManager.SetComponentData(vehicle, input);
            
            // When
            var movementSystem = World.CreateSystemManaged<VehicleMovementSystem>();
            var wheelSystem = World.CreateSystemManaged<AdvancedWheelPhysicsSystem>();
            var weatherSystem = World.CreateSystemManaged<WeatherSystem>();
            movementSystem.Update();
            wheelSystem.Update();
            weatherSystem.Update();
            
            // Then
            var physics = EntityManager.GetComponentData<VehiclePhysics>(vehicle);
            var wheelData = EntityManager.GetComponentData<WheelData>(wheel);
            var weatherData = EntityManager.GetComponentData<WeatherData>(weather);
            
            Assert.Greater(weatherData.SnowIntensity, 0f, "It should be snowing");
            Assert.Greater(wheelData.SlipRatio, 0.2f, "Vehicle should have poor traction in snow");
            Assert.Less(physics.Velocity.magnitude, 8f, "Vehicle should move very slowly in snow");
        }
        
        [Test]
        public void Given_VehicleInFog_When_Driving_Then_VisibilityShouldBeReduced()
        {
            // Given
            var vehicle = CreateVehicle();
            var weather = CreateWeather(WeatherType.Foggy);
            
            // When
            var weatherSystem = World.CreateSystemManaged<WeatherSystem>();
            weatherSystem.Update();
            
            // Then
            var weatherData = EntityManager.GetComponentData<WeatherData>(weather);
            Assert.Less(weatherData.Visibility, 500f, "Visibility should be reduced in fog");
        }
        
        [Test]
        public void Given_VehicleInStorm_When_Driving_Then_ConditionsShouldBePoor()
        {
            // Given
            var vehicle = CreateVehicle();
            var surface = CreateSurface(SurfaceType.Mud);
            var weather = CreateWeather(WeatherType.Stormy);
            var input = new VehicleInput { Vertical = 1f };
            EntityManager.SetComponentData(vehicle, input);
            
            // When
            var movementSystem = World.CreateSystemManaged<VehicleMovementSystem>();
            var wheelSystem = World.CreateSystemManaged<AdvancedWheelPhysicsSystem>();
            var weatherSystem = World.CreateSystemManaged<WeatherSystem>();
            movementSystem.Update();
            wheelSystem.Update();
            weatherSystem.Update();
            
            // Then
            var physics = EntityManager.GetComponentData<VehiclePhysics>(vehicle);
            var wheelData = EntityManager.GetComponentData<WheelData>(wheel);
            var weatherData = EntityManager.GetComponentData<WeatherData>(weather);
            
            Assert.Greater(weatherData.WindSpeed, 10f, "Wind should be strong in storm");
            Assert.Greater(wheelData.SlipRatio, 0.3f, "Vehicle should have very poor traction in storm");
            Assert.Less(physics.Velocity.magnitude, 5f, "Vehicle should move very slowly in storm");
        }
        
        [Test]
        public void Given_VehicleWithLowTirePressure_When_Driving_Then_PerformanceShouldBeReduced()
        {
            // Given
            var vehicle = CreateVehicle();
            var wheel = CreateWheel();
            var wheelPhysics = EntityManager.GetComponentData<WheelPhysicsData>(wheel);
            wheelPhysics.TirePressure = 150f; // Low pressure
            EntityManager.SetComponentData(wheel, wheelPhysics);
            
            var input = new VehicleInput { Vertical = 1f };
            EntityManager.SetComponentData(vehicle, input);
            
            // When
            var movementSystem = World.CreateSystemManaged<VehicleMovementSystem>();
            var wheelSystem = World.CreateSystemManaged<AdvancedWheelPhysicsSystem>();
            movementSystem.Update();
            wheelSystem.Update();
            
            // Then
            var physics = EntityManager.GetComponentData<VehiclePhysics>(vehicle);
            var updatedWheelPhysics = EntityManager.GetComponentData<WheelPhysicsData>(wheel);
            
            Assert.Less(updatedWheelPhysics.TirePressure, 200f, "Tire pressure should be low");
            Assert.Less(physics.Velocity.magnitude, 10f, "Vehicle should perform poorly with low tire pressure");
        }
        
        [Test]
        public void Given_VehicleWithWornTires_When_Driving_Then_TractionShouldBeReduced()
        {
            // Given
            var vehicle = CreateVehicle();
            var wheel = CreateWheel();
            var wheelPhysics = EntityManager.GetComponentData<WheelPhysicsData>(wheel);
            wheelPhysics.TreadWear = 0.8f; // Worn tires
            EntityManager.SetComponentData(wheel, wheelPhysics);
            
            var input = new VehicleInput { Vertical = 1f };
            EntityManager.SetComponentData(vehicle, input);
            
            // When
            var movementSystem = World.CreateSystemManaged<VehicleMovementSystem>();
            var wheelSystem = World.CreateSystemManaged<AdvancedWheelPhysicsSystem>();
            movementSystem.Update();
            wheelSystem.Update();
            
            // Then
            var physics = EntityManager.GetComponentData<VehiclePhysics>(vehicle);
            var updatedWheelPhysics = EntityManager.GetComponentData<WheelPhysicsData>(wheel);
            
            Assert.Greater(updatedWheelPhysics.TreadWear, 0.5f, "Tires should be worn");
            Assert.Less(physics.Velocity.magnitude, 12f, "Vehicle should perform poorly with worn tires");
        }
        
        [Test]
        public void Given_VehicleWithHotTires_When_Driving_Then_TemperatureShouldAffectPerformance()
        {
            // Given
            var vehicle = CreateVehicle();
            var wheel = CreateWheel();
            var wheelPhysics = EntityManager.GetComponentData<WheelPhysicsData>(wheel);
            wheelPhysics.WheelTemperature = 150f; // Hot tires
            EntityManager.SetComponentData(wheel, wheelPhysics);
            
            var input = new VehicleInput { Vertical = 1f };
            EntityManager.SetComponentData(vehicle, input);
            
            // When
            var movementSystem = World.CreateSystemManaged<VehicleMovementSystem>();
            var wheelSystem = World.CreateSystemManaged<AdvancedWheelPhysicsSystem>();
            movementSystem.Update();
            wheelSystem.Update();
            
            // Then
            var physics = EntityManager.GetComponentData<VehiclePhysics>(vehicle);
            var updatedWheelPhysics = EntityManager.GetComponentData<WheelPhysicsData>(wheel);
            
            Assert.Greater(updatedWheelPhysics.WheelTemperature, 100f, "Tires should be hot");
            Assert.IsTrue(physics.Velocity.magnitude >= 0f, "Vehicle should still be able to move");
        }
        
        [Test]
        public void Given_VehicleWithMuddyWheels_When_Driving_Then_MudShouldAffectPerformance()
        {
            // Given
            var vehicle = CreateVehicle();
            var wheel = CreateWheel();
            var surface = CreateSurface(SurfaceType.Mud);
            var wheelPhysics = EntityManager.GetComponentData<WheelPhysicsData>(wheel);
            wheelPhysics.MudMass = 5f; // Muddy wheels
            EntityManager.SetComponentData(wheel, wheelPhysics);
            
            var input = new VehicleInput { Vertical = 1f };
            EntityManager.SetComponentData(vehicle, input);
            
            // When
            var movementSystem = World.CreateSystemManaged<VehicleMovementSystem>();
            var wheelSystem = World.CreateSystemManaged<AdvancedWheelPhysicsSystem>();
            var terrainSystem = World.CreateSystemManaged<TerrainDeformationSystem>();
            movementSystem.Update();
            wheelSystem.Update();
            terrainSystem.Update();
            
            // Then
            var physics = EntityManager.GetComponentData<VehiclePhysics>(vehicle);
            var updatedWheelPhysics = EntityManager.GetComponentData<WheelPhysicsData>(wheel);
            
            Assert.Greater(updatedWheelPhysics.MudMass, 0f, "Wheels should be muddy");
            Assert.Less(physics.Velocity.magnitude, 8f, "Vehicle should perform poorly with muddy wheels");
        }
        
        [Test]
        public void Given_VehicleWithAllSystems_When_AllInputsApplied_Then_AllSystemsShouldWorkTogether()
        {
            // Given
            var vehicle = CreateVehicle();
            var wheel = CreateWheel();
            var surface = CreateSurface(SurfaceType.Grass);
            var weather = CreateWeather(WeatherType.Clear);
            var hud = CreateHUD();
            var audio = CreateAudio();
            
            var input = new VehicleInput { Vertical = 1f, Horizontal = 0.5f };
            EntityManager.SetComponentData(vehicle, input);
            
            // When
            var movementSystem = World.CreateSystemManaged<VehicleMovementSystem>();
            var wheelSystem = World.CreateSystemManaged<AdvancedWheelPhysicsSystem>();
            var terrainSystem = World.CreateSystemManaged<TerrainDeformationSystem>();
            var weatherSystem = World.CreateSystemManaged<WeatherSystem>();
            var hudSystem = World.CreateSystemManaged<UIHUDSystem>();
            var audioSystem = World.CreateSystemManaged<EngineAudioSystem>();
            
            movementSystem.Update();
            wheelSystem.Update();
            terrainSystem.Update();
            weatherSystem.Update();
            hudSystem.Update();
            audioSystem.Update();
            
            // Then
            AssertVehicleMoved(vehicle, new float3(0, 0, 1));
            AssertWheelGrounded(wheel);
            AssertSurfaceType(surface, SurfaceType.Grass);
            AssertWeatherType(weather, WeatherType.Clear);
            AssertHUDUpdated(hud);
            AssertAudioPlaying(audio);
        }
    }
}