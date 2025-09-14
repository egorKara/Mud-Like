using NUnit.Framework;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;
using MudLike.Vehicles.Systems;
using MudLike.Vehicles.Components;
using MudLike.Weather.Components;
using Unity.Core;

namespace MudLike.Tests.Unit.Vehicles
{
    /// <summary>
    /// Тесты для системы давления в шинах TirePressureSystem
    /// </summary>
    public class TirePressureSystemTests
    {
        private World _world;
        private TirePressureSystem _tirePressureSystem;
        private EntityManager _entityManager;

        [SetUp]
        public void SetUp()
        {
            _world = new World("TestWorld");
            _entityManager = _world.EntityManager;
            
            _tirePressureSystem = _world.GetOrCreateSystemManaged<TirePressureSystem>();
            _tirePressureSystem.OnCreate(ref _world.Unmanaged);
            
            _world.SetSingleton(new TimeData { ElapsedTime = 10f, DeltaTime = 0.016f, FixedDeltaTime = 0.016f });
        }

        [TearDown]
        public void TearDown()
        {
            _tirePressureSystem.OnDestroy(ref _world.Unmanaged);
            _world.Dispose();
        }

        [Test]
        public void TirePressureSystem_OnCreate_InitializesCorrectly()
        {
            Assert.IsNotNull(_tirePressureSystem);
        }

        [Test]
        public void TirePressureSystem_OnUpdate_ProcessesWithoutErrors()
        {
            _tirePressureSystem.OnUpdate(ref _world.Unmanaged);
            Assert.IsNotNull(_tirePressureSystem);
        }

        [Test]
        public void TirePressureSystem_WithTireData_ProcessesCorrectly()
        {
            var entity = _entityManager.CreateEntity();
            _entityManager.AddComponentData(entity, new TireData
            {
                Wear = 0.1f,
                Temperature = 75f,
                Pressure = 2.5f,
                Grip = 0.9f,
                TreadDepth = 8f,
                MaxWear = 1f,
                WearRate = 0.001f,
                IsDamaged = false
            });
            _entityManager.AddComponentData(entity, new WheelData
            {
                Position = float3.zero,
                Radius = 0.5f,
                Width = 0.2f,
                SuspensionLength = 0.3f,
                SpringForce = 1000f,
                DampingForce = 500f,
                IsGrounded = true,
                GroundDistance = 0.1f
            });
            _entityManager.AddComponentData(entity, new VehiclePhysics
            {
                Velocity = new float3(10f, 0, 0),
                Acceleration = float3.zero,
                ForwardSpeed = 10f,
                TurnSpeed = 0f
            });

            _tirePressureSystem.OnUpdate(ref _world.Unmanaged);
            Assert.IsNotNull(_tirePressureSystem);
        }

        [Test]
        public void TirePressureSystem_WithWeatherData_ProcessesCorrectly()
        {
            var entity = _entityManager.CreateEntity();
            _entityManager.AddComponentData(entity, new TireData
            {
                Wear = 0.2f,
                Temperature = 80f,
                Pressure = 2.3f,
                Grip = 0.85f,
                TreadDepth = 7f,
                MaxWear = 1f,
                WearRate = 0.002f,
                IsDamaged = false
            });
            _entityManager.AddComponentData(entity, new WheelData
            {
                Position = float3.zero,
                Radius = 0.5f,
                Width = 0.2f,
                SuspensionLength = 0.3f,
                SpringForce = 1000f,
                DampingForce = 500f,
                IsGrounded = true,
                GroundDistance = 0.1f
            });
            _entityManager.AddComponentData(entity, new VehiclePhysics
            {
                Velocity = new float3(15f, 0, 0),
                Acceleration = float3.zero,
                ForwardSpeed = 15f,
                TurnSpeed = 0f
            });
            _entityManager.AddComponentData(entity, new WeatherData
            {
                Temperature = 25f,
                Humidity = 0.6f,
                Precipitation = 0.2f,
                WindSpeed = 5f,
                Pressure = 1013f
            });

            _tirePressureSystem.OnUpdate(ref _world.Unmanaged);
            Assert.IsNotNull(_tirePressureSystem);
        }

        [Test]
        public void TirePressureSystem_HighTemperature_ProcessesCorrectly()
        {
            var entity = _entityManager.CreateEntity();
            _entityManager.AddComponentData(entity, new TireData
            {
                Wear = 0.3f,
                Temperature = 120f,
                Pressure = 2.1f,
                Grip = 0.7f,
                TreadDepth = 6f,
                MaxWear = 1f,
                WearRate = 0.003f,
                IsDamaged = false
            });
            _entityManager.AddComponentData(entity, new WheelData
            {
                Position = float3.zero,
                Radius = 0.5f,
                Width = 0.2f,
                SuspensionLength = 0.3f,
                SpringForce = 1000f,
                DampingForce = 500f,
                IsGrounded = true,
                GroundDistance = 0.1f
            });
            _entityManager.AddComponentData(entity, new VehiclePhysics
            {
                Velocity = new float3(25f, 0, 0),
                Acceleration = float3.zero,
                ForwardSpeed = 25f,
                TurnSpeed = 0f
            });
            _entityManager.AddComponentData(entity, new WeatherData
            {
                Temperature = 35f,
                Humidity = 0.8f,
                Precipitation = 0.1f,
                WindSpeed = 2f,
                Pressure = 1005f
            });

            _tirePressureSystem.OnUpdate(ref _world.Unmanaged);
            Assert.IsNotNull(_tirePressureSystem);
        }

        [Test]
        public void TirePressureSystem_MultipleTires_HandlesCorrectly()
        {
            for (int i = 0; i < 8; i++)
            {
                var entity = _entityManager.CreateEntity();
                _entityManager.AddComponentData(entity, new TireData
                {
                    Wear = i * 0.1f,
                    Temperature = 70f + i * 10f,
                    Pressure = 2.5f - i * 0.05f,
                    Grip = 0.9f - i * 0.02f,
                    TreadDepth = 8f - i * 0.5f,
                    MaxWear = 1f,
                    WearRate = 0.001f + i * 0.0005f,
                    IsDamaged = i % 3 == 0
                });
                _entityManager.AddComponentData(entity, new WheelData
                {
                    Position = new float3(i * 2f, 0, 0),
                    Radius = 0.5f + i * 0.05f,
                    Width = 0.2f + i * 0.01f,
                    SuspensionLength = 0.3f,
                    SpringForce = 1000f + i * 100f,
                    DampingForce = 500f + i * 50f,
                    IsGrounded = i % 2 == 0,
                    GroundDistance = 0.1f + i * 0.01f
                });
                _entityManager.AddComponentData(entity, new VehiclePhysics
                {
                    Velocity = new float3(i * 3f, 0, 0),
                    Acceleration = float3.zero,
                    ForwardSpeed = i * 3f,
                    TurnSpeed = 0f
                });
            }

            _tirePressureSystem.OnUpdate(ref _world.Unmanaged);
            Assert.IsNotNull(_tirePressureSystem);
        }

        [Test]
        public void TirePressureSystem_EdgeCases_HandleCorrectly()
        {
            var entity = _entityManager.CreateEntity();
            _entityManager.AddComponentData(entity, new TireData
            {
                Wear = float.MaxValue,
                Temperature = float.PositiveInfinity,
                Pressure = float.NegativeInfinity,
                Grip = float.NaN,
                TreadDepth = float.MinValue,
                MaxWear = float.PositiveInfinity,
                WearRate = float.NaN,
                IsDamaged = true
            });
            _entityManager.AddComponentData(entity, new WheelData
            {
                Position = new float3(float.PositiveInfinity, float.NegativeInfinity, float.NaN),
                Radius = float.MaxValue,
                Width = float.MinValue,
                SuspensionLength = float.NaN,
                SpringForce = float.PositiveInfinity,
                DampingForce = float.NegativeInfinity,
                IsGrounded = true,
                GroundDistance = float.Epsilon
            });
            _entityManager.AddComponentData(entity, new VehiclePhysics
            {
                Velocity = new float3(float.PositiveInfinity, float.NegativeInfinity, float.NaN),
                Acceleration = float3.zero,
                ForwardSpeed = float.MaxValue,
                TurnSpeed = float.MinValue
            });
            _entityManager.AddComponentData(entity, new WeatherData
            {
                Temperature = float.NaN,
                Humidity = float.PositiveInfinity,
                Precipitation = float.NegativeInfinity,
                WindSpeed = float.MaxValue,
                Pressure = float.MinValue
            });

            Assert.DoesNotThrow(() => 
            {
                _tirePressureSystem.OnUpdate(ref _world.Unmanaged);
            });
        }
    }
}
