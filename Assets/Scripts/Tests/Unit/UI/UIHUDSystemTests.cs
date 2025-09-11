using NUnit.Framework;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using MudLike.UI.Components;
using MudLike.Vehicles.Components;
using MudLike.Weather.Components;
using MudLike.Core.Components;

namespace MudLike.Tests.Unit.UI
{
    /// <summary>
    /// Unit тесты для UIHUDSystem
    /// </summary>
    [TestFixture]
    public class UIHUDSystemTests : ECSTestFixture
    {
        private UIHUDSystem _hudSystem;
        private Entity _hudEntity;
        private Entity _vehicleEntity;
        private Entity _weatherEntity;
        
        [SetUp]
        public override void Setup()
        {
            base.Setup();
            
            // Создаем систему
            _hudSystem = World.CreateSystemManaged<UIHUDSystem>();
            
            // Создаем HUD сущность
            _hudEntity = EntityManager.CreateEntity();
            EntityManager.AddComponentData(_hudEntity, new UIHUDData
            {
                Speed = 0f,
                RPM = 0f,
                CurrentGear = 0,
                Health = 1f,
                FuelLevel = 1f,
                EngineTemperature = 0.5f,
                IsActive = true,
                NeedsUpdate = false
            });
            
            // Создаем транспорт
            _vehicleEntity = EntityManager.CreateEntity();
            EntityManager.AddComponentData(_vehicleEntity, new VehiclePhysics
            {
                Velocity = new float3(10f, 0f, 0f),
                EngineRPM = 2000f,
                CurrentGear = 3
            });
            EntityManager.AddComponentData(_vehicleEntity, new VehicleConfig
            {
                MaxSpeed = 100f,
                Mass = 1500f
            });
            EntityManager.AddComponentData(_vehicleEntity, new LocalTransform
            {
                Position = new float3(0f, 0f, 0f),
                Rotation = quaternion.identity
            });
            
            // Создаем погоду
            _weatherEntity = EntityManager.CreateEntity();
            EntityManager.AddComponentData(_weatherEntity, new WeatherData
            {
                Type = WeatherType.Clear,
                Temperature = 20f,
                Humidity = 0.5f,
                WindSpeed = 5f,
                RainIntensity = 0f,
                SnowIntensity = 0f,
                Visibility = 1f
            });
        }
        
        [Test]
        public void UIHUDSystem_UpdatesSpeedCorrectly()
        {
            // Arrange
            var vehiclePhysics = EntityManager.GetComponentData<VehiclePhysics>(_vehicleEntity);
            vehiclePhysics.Velocity = new float3(15f, 0f, 0f);
            EntityManager.SetComponentData(_vehicleEntity, vehiclePhysics);
            
            // Act
            _hudSystem.Update();
            
            // Assert
            var hudData = EntityManager.GetComponentData<UIHUDData>(_hudEntity);
            Assert.AreEqual(54f, hudData.Speed, 0.1f); // 15 м/с = 54 км/ч
        }
        
        [Test]
        public void UIHUDSystem_UpdatesRPMCorrectly()
        {
            // Arrange
            var vehiclePhysics = EntityManager.GetComponentData<VehiclePhysics>(_vehicleEntity);
            vehiclePhysics.EngineRPM = 3000f;
            EntityManager.SetComponentData(_vehicleEntity, vehiclePhysics);
            
            // Act
            _hudSystem.Update();
            
            // Assert
            var hudData = EntityManager.GetComponentData<UIHUDData>(_hudEntity);
            Assert.AreEqual(3000f, hudData.RPM);
        }
        
        [Test]
        public void UIHUDSystem_UpdatesGearCorrectly()
        {
            // Arrange
            var vehiclePhysics = EntityManager.GetComponentData<VehiclePhysics>(_vehicleEntity);
            vehiclePhysics.CurrentGear = 4;
            EntityManager.SetComponentData(_vehicleEntity, vehiclePhysics);
            
            // Act
            _hudSystem.Update();
            
            // Assert
            var hudData = EntityManager.GetComponentData<UIHUDData>(_hudEntity);
            Assert.AreEqual(4, hudData.CurrentGear);
        }
        
        [Test]
        public void UIHUDSystem_UpdatesWeatherInfoCorrectly()
        {
            // Arrange
            var weatherData = EntityManager.GetComponentData<WeatherData>(_weatherEntity);
            weatherData.Temperature = 25f;
            weatherData.Humidity = 0.7f;
            EntityManager.SetComponentData(_weatherEntity, weatherData);
            
            // Act
            _hudSystem.Update();
            
            // Assert
            var hudData = EntityManager.GetComponentData<UIHUDData>(_hudEntity);
            Assert.AreEqual(WeatherType.Clear, hudData.WeatherInfo.Type);
            Assert.AreEqual(25f, hudData.WeatherInfo.Temperature);
            Assert.AreEqual(0.7f, hudData.WeatherInfo.Humidity);
        }
        
        [Test]
        public void UIHUDSystem_SetsNeedsUpdateFlag()
        {
            // Act
            _hudSystem.Update();
            
            // Assert
            var hudData = EntityManager.GetComponentData<UIHUDData>(_hudEntity);
            Assert.IsTrue(hudData.NeedsUpdate);
        }
        
        [Test]
        public void UIHUDSystem_HandlesMissingVehicleData()
        {
            // Arrange - удаляем транспорт
            EntityManager.DestroyEntity(_vehicleEntity);
            
            // Act
            _hudSystem.Update();
            
            // Assert - не должно быть исключений
            var hudData = EntityManager.GetComponentData<UIHUDData>(_hudEntity);
            Assert.IsTrue(hudData.NeedsUpdate);
        }
        
        [Test]
        public void UIHUDSystem_HandlesMissingWeatherData()
        {
            // Arrange - удаляем погоду
            EntityManager.DestroyEntity(_weatherEntity);
            
            // Act
            _hudSystem.Update();
            
            // Assert - не должно быть исключений
            var hudData = EntityManager.GetComponentData<UIHUDData>(_hudEntity);
            Assert.IsTrue(hudData.NeedsUpdate);
        }
    }
}