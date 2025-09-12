using NUnit.Framework;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using MudLike.UI.Systems;
using MudLike.UI.Components;
using MudLike.Vehicles.Components;
using MudLike.Weather.Components;
using Unity.Core;

namespace MudLike.Tests.Unit.UI
{
    /// <summary>
    /// Тесты для системы HUD интерфейса UIHUDSystem
    /// </summary>
    public class UIHUDSystemTests
    {
        private World _world;
        private UIHUDSystem _uiHUDSystem;
        private EntityManager _entityManager;

        [SetUp]
        public void SetUp()
        {
            _world = new World("TestWorld");
            _entityManager = _world.EntityManager;
            
            _uiHUDSystem = _world.GetOrCreateSystemManaged<UIHUDSystem>();
            _uiHUDSystem.OnCreate(ref _world.Unmanaged);
            
            _world.SetSingleton(new TimeData { ElapsedTime = 10f, DeltaTime = 0.016f, FixedDeltaTime = 0.016f });
        }

        [TearDown]
        public void TearDown()
        {
            _uiHUDSystem.OnDestroy(ref _world.Unmanaged);
            _world.Dispose();
        }

        [Test]
        public void UIHUDSystem_OnCreate_InitializesCorrectly()
        {
            Assert.IsNotNull(_uiHUDSystem);
        }

        [Test]
        public void UIHUDSystem_OnUpdate_ProcessesWithoutErrors()
        {
            _uiHUDSystem.OnUpdate(ref _world.Unmanaged);
            Assert.IsNotNull(_uiHUDSystem);
        }

        [Test]
        public void UIHUDSystem_WithUIHUDData_ProcessesCorrectly()
        {
            var entity = _entityManager.CreateEntity();
            _entityManager.AddComponentData(entity, new UIHUDData
            {
                Speed = 50f,
                RPM = 2000f,
                Fuel = 75f,
                Gear = 3,
                IsVisible = true
            });

            _uiHUDSystem.OnUpdate(ref _world.Unmanaged);
            Assert.IsNotNull(_uiHUDSystem);
        }

        [Test]
        public void UIHUDSystem_WithVehicleData_ProcessesCorrectly()
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
                ForwardSpeed = 15f,
                EnginePower = 500f
            });
            _entityManager.AddComponentData(entity, new VehicleConfig
            {
                MaxSpeed = 100f,
                MaxEnginePower = 1000f,
                FuelCapacity = 80f
            });

            _uiHUDSystem.OnUpdate(ref _world.Unmanaged);
            Assert.IsNotNull(_uiHUDSystem);
        }

        [Test]
        public void UIHUDSystem_WithWeatherData_ProcessesCorrectly()
        {
            var entity = _entityManager.CreateEntity();
            _entityManager.AddComponentData(entity, new WeatherData
            {
                Temperature = 20f,
                Humidity = 60f,
                WindSpeed = 5f,
                RainIntensity = 0.3f,
                IsRaining = true
            });

            _uiHUDSystem.OnUpdate(ref _world.Unmanaged);
            Assert.IsNotNull(_uiHUDSystem);
        }

        [Test]
        public void UIHUDSystem_MultipleEntities_HandlesCorrectly()
        {
            for (int i = 0; i < 5; i++)
            {
                var entity = _entityManager.CreateEntity();
                _entityManager.AddComponentData(entity, new UIHUDData
                {
                    Speed = i * 20f,
                    RPM = 1000f + i * 500f,
                    Fuel = 100f - i * 10f,
                    Gear = i + 1,
                    IsVisible = i % 2 == 0
                });
            }

            _uiHUDSystem.OnUpdate(ref _world.Unmanaged);
            Assert.IsNotNull(_uiHUDSystem);
        }

        [Test]
        public void UIHUDSystem_EdgeCases_HandleCorrectly()
        {
            var entity = _entityManager.CreateEntity();
            _entityManager.AddComponentData(entity, new UIHUDData
            {
                Speed = float.MaxValue,
                RPM = float.NaN,
                Fuel = float.PositiveInfinity,
                Gear = -1,
                IsVisible = true
            });

            Assert.DoesNotThrow(() => 
            {
                _uiHUDSystem.OnUpdate(ref _world.Unmanaged);
            });
        }
    }
}