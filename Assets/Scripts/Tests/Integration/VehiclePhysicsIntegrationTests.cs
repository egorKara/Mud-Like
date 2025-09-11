using NUnit.Framework;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using MudLike.Vehicles.Components;
using MudLike.Vehicles.Systems;
using MudLike.Terrain.Components;
using MudLike.Weather.Components;
using MudLike.Tests.Infrastructure;

namespace MudLike.Tests.Integration
{
    /// <summary>
    /// Интеграционные тесты для физики транспортных средств
    /// </summary>
    [TestFixture]
    public class VehiclePhysicsIntegrationTests : MudLikeTestFixture
    {
        private VehicleMovementSystem _movementSystem;
        private AdvancedWheelPhysicsSystem _wheelSystem;
        private TerrainDeformationSystem _terrainSystem;
        private WeatherSystem _weatherSystem;
        
        [SetUp]
        public override void Setup()
        {
            base.Setup();
            _movementSystem = World.CreateSystemManaged<VehicleMovementSystem>();
            _wheelSystem = World.CreateSystemManaged<AdvancedWheelPhysicsSystem>();
            _terrainSystem = World.CreateSystemManaged<TerrainDeformationSystem>();
            _weatherSystem = World.CreateSystemManaged<WeatherSystem>();
        }
        
        [Test]
        public void IntegrationTest_VehicleWheelSurface_ShouldWorkTogether()
        {
            // Arrange
            var vehicle = CreateVehicle();
            var wheel = CreateWheel();
            var surface = CreateSurface(SurfaceType.Mud);
            var weather = CreateWeather(WeatherType.Rainy);
            
            // Act
            _movementSystem.Update();
            _wheelSystem.Update();
            _terrainSystem.Update();
            _weatherSystem.Update();
            
            // Assert
            AssertVehicleMoved(vehicle, new float3(0, 0, 1));
            AssertWheelGrounded(wheel);
            AssertSurfaceType(surface, SurfaceType.Mud);
            AssertWeatherType(weather, WeatherType.Rainy);
        }
        
        [Test]
        public void IntegrationTest_VehicleOnMuddySurface_ShouldSlip()
        {
            // Arrange
            var vehicle = CreateVehicle();
            var surface = CreateSurface(SurfaceType.Mud);
            var weather = CreateWeather(WeatherType.Rainy);
            
            var input = new VehicleInput { Vertical = 1f };
            EntityManager.SetComponentData(vehicle, input);
            
            // Act
            _movementSystem.Update();
            _wheelSystem.Update();
            _terrainSystem.Update();
            _weatherSystem.Update();
            
            // Assert
            var physics = EntityManager.GetComponentData<VehiclePhysics>(vehicle);
            var wheelData = EntityManager.GetComponentData<WheelData>(vehicle);
            
            Assert.Greater(wheelData.SlipRatio, 0.1f);
            Assert.Less(physics.Velocity.magnitude, 10f);
        }
        
        [Test]
        public void IntegrationTest_VehicleOnIcySurface_ShouldSlide()
        {
            // Arrange
            var vehicle = CreateVehicle();
            var surface = CreateSurface(SurfaceType.Ice);
            var weather = CreateWeather(WeatherType.Cold);
            
            var input = new VehicleInput { Vertical = 1f };
            EntityManager.SetComponentData(vehicle, input);
            
            // Act
            _movementSystem.Update();
            _wheelSystem.Update();
            _terrainSystem.Update();
            _weatherSystem.Update();
            
            // Assert
            var physics = EntityManager.GetComponentData<VehiclePhysics>(vehicle);
            var wheelData = EntityManager.GetComponentData<WheelData>(vehicle);
            
            Assert.Greater(wheelData.SlipRatio, 0.5f);
            Assert.Less(physics.Velocity.magnitude, 5f);
        }
        
        [Test]
        public void IntegrationTest_VehicleOnDryAsphalt_ShouldHaveGoodTraction()
        {
            // Arrange
            var vehicle = CreateVehicle();
            var surface = CreateSurface(SurfaceType.Asphalt);
            var weather = CreateWeather(WeatherType.Clear);
            
            var input = new VehicleInput { Vertical = 1f };
            EntityManager.SetComponentData(vehicle, input);
            
            // Act
            _movementSystem.Update();
            _wheelSystem.Update();
            _terrainSystem.Update();
            _weatherSystem.Update();
            
            // Assert
            var physics = EntityManager.GetComponentData<VehiclePhysics>(vehicle);
            var wheelData = EntityManager.GetComponentData<WheelData>(vehicle);
            
            Assert.Less(wheelData.SlipRatio, 0.1f);
            Assert.Greater(physics.Velocity.magnitude, 5f);
        }
        
        [Test]
        public void IntegrationTest_VehicleInRain_ShouldHaveReducedTraction()
        {
            // Arrange
            var vehicle = CreateVehicle();
            var surface = CreateSurface(SurfaceType.Asphalt);
            var weather = CreateWeather(WeatherType.Rainy);
            
            var input = new VehicleInput { Vertical = 1f };
            EntityManager.SetComponentData(vehicle, input);
            
            // Act
            _movementSystem.Update();
            _wheelSystem.Update();
            _terrainSystem.Update();
            _weatherSystem.Update();
            
            // Assert
            var physics = EntityManager.GetComponentData<VehiclePhysics>(vehicle);
            var wheelData = EntityManager.GetComponentData<WheelData>(vehicle);
            
            Assert.Greater(wheelData.SlipRatio, 0.05f);
            Assert.Less(physics.Velocity.magnitude, 15f);
        }
        
        [Test]
        public void IntegrationTest_VehicleInSnow_ShouldHavePoorTraction()
        {
            // Arrange
            var vehicle = CreateVehicle();
            var surface = CreateSurface(SurfaceType.Snow);
            var weather = CreateWeather(WeatherType.Snowy);
            
            var input = new VehicleInput { Vertical = 1f };
            EntityManager.SetComponentData(vehicle, input);
            
            // Act
            _movementSystem.Update();
            _wheelSystem.Update();
            _terrainSystem.Update();
            _weatherSystem.Update();
            
            // Assert
            var physics = EntityManager.GetComponentData<VehiclePhysics>(vehicle);
            var wheelData = EntityManager.GetComponentData<WheelData>(vehicle);
            
            Assert.Greater(wheelData.SlipRatio, 0.2f);
            Assert.Less(physics.Velocity.magnitude, 8f);
        }
        
        [Test]
        public void IntegrationTest_VehicleInFog_ShouldHaveReducedVisibility()
        {
            // Arrange
            var vehicle = CreateVehicle();
            var weather = CreateWeather(WeatherType.Foggy);
            
            // Act
            _weatherSystem.Update();
            
            // Assert
            var weatherData = EntityManager.GetComponentData<WeatherData>(weather);
            Assert.Less(weatherData.Visibility, 500f);
        }
        
        [Test]
        public void IntegrationTest_VehicleInStorm_ShouldHavePoorConditions()
        {
            // Arrange
            var vehicle = CreateVehicle();
            var surface = CreateSurface(SurfaceType.Mud);
            var weather = CreateWeather(WeatherType.Stormy);
            
            var input = new VehicleInput { Vertical = 1f };
            EntityManager.SetComponentData(vehicle, input);
            
            // Act
            _movementSystem.Update();
            _wheelSystem.Update();
            _terrainSystem.Update();
            _weatherSystem.Update();
            
            // Assert
            var physics = EntityManager.GetComponentData<VehiclePhysics>(vehicle);
            var wheelData = EntityManager.GetComponentData<WheelData>(vehicle);
            var weatherData = EntityManager.GetComponentData<WeatherData>(weather);
            
            Assert.Greater(wheelData.SlipRatio, 0.3f);
            Assert.Less(physics.Velocity.magnitude, 5f);
            Assert.Greater(weatherData.WindSpeed, 10f);
        }
        
        [Test]
        public void IntegrationTest_AllSystems_ShouldWorkTogether()
        {
            // Arrange
            var vehicle = CreateVehicle();
            var wheel = CreateWheel();
            var surface = CreateSurface(SurfaceType.Grass);
            var weather = CreateWeather(WeatherType.Clear);
            var hud = CreateHUD();
            var audio = CreateAudio();
            
            var input = new VehicleInput { Vertical = 1f, Horizontal = 0.5f };
            EntityManager.SetComponentData(vehicle, input);
            
            // Act
            _movementSystem.Update();
            _wheelSystem.Update();
            _terrainSystem.Update();
            _weatherSystem.Update();
            
            // Assert
            AssertVehicleMoved(vehicle, new float3(0, 0, 1));
            AssertWheelGrounded(wheel);
            AssertSurfaceType(surface, SurfaceType.Grass);
            AssertWeatherType(weather, WeatherType.Clear);
            AssertHUDUpdated(hud);
            AssertAudioPlaying(audio);
        }
        
        [Test]
        public void IntegrationTest_Performance_ShouldHandle1000Vehicles()
        {
            // Arrange
            for (int i = 0; i < 1000; i++)
            {
                CreateVehicle();
                CreateWheel();
                CreateSurface(SurfaceType.Asphalt);
            }
            
            // Act & Assert
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            
            _movementSystem.Update();
            _wheelSystem.Update();
            _terrainSystem.Update();
            _weatherSystem.Update();
            
            stopwatch.Stop();
            var executionTime = stopwatch.ElapsedMilliseconds;
            
            Assert.Less(executionTime, 100f, 
                "Integration test should complete in less than 100ms, actual: {0}ms", executionTime);
        }
    }
}