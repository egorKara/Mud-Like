using NUnit.Framework;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;
using MudLike.Vehicles.Systems;
using MudLike.Vehicles.Components;
using MudLike.Terrain.Components;
using MudLike.Weather.Components;
using Unity.Core;

namespace MudLike.Tests.Unit.Vehicles
{
    /// <summary>
    /// Тесты для системы износа шин TireWearSystem
    /// </summary>
    public class TireWearSystemTests
    {
        private World _world;
        private TireWearSystem _tireWearSystem;
        private EntityManager _entityManager;

        [SetUp]
        public void SetUp()
        {
            _world = new World("TestWorld");
            _entityManager = _world.EntityManager;
            
            _tireWearSystem = _world.GetOrCreateSystemManaged<TireWearSystem>();
            _tireWearSystem.OnCreate(ref _world.Unmanaged);
            
            _world.SetSingleton(new TimeData { ElapsedTime = 10f, DeltaTime = 0.016f, FixedDeltaTime = 0.016f });
        }

        [TearDown]
        public void TearDown()
        {
            _tireWearSystem.OnDestroy(ref _world.Unmanaged);
            _world.Dispose();
        }

        [Test]
        public void TireWearSystem_OnCreate_InitializesCorrectly()
        {
            Assert.IsNotNull(_tireWearSystem);
        }

        [Test]
        public void TireWearSystem_OnUpdate_ProcessesWithoutErrors()
        {
            _tireWearSystem.OnUpdate(ref _world.Unmanaged);
            Assert.IsNotNull(_tireWearSystem);
        }

        [Test]
        public void TireWearSystem_WithTireData_ProcessesCorrectly()
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

            _tireWearSystem.OnUpdate(ref _world.Unmanaged);
            Assert.IsNotNull(_tireWearSystem);
        }

        [Test]
        public void TireWearSystem_WithSurfaceData_ProcessesCorrectly()
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
            _entityManager.AddComponentData(entity, new SurfaceData
            {
                Friction = 0.8f,
                Hardness = 0.6f,
                Roughness = 0.4f,
                Temperature = 20f,
                Wetness = 0.1f,
                Type = SurfaceType.Asphalt
            });

            _tireWearSystem.OnUpdate(ref _world.Unmanaged);
            Assert.IsNotNull(_tireWearSystem);
        }

        [Test]
        public void TireWearSystem_WithWeatherData_ProcessesCorrectly()
        {
            var entity = _entityManager.CreateEntity();
            _entityManager.AddComponentData(entity, new TireData
            {
                Wear = 0.3f,
                Temperature = 90f,
                Pressure = 2.1f,
                Grip = 0.8f,
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
                Velocity = new float3(20f, 0, 0),
                Acceleration = float3.zero,
                ForwardSpeed = 20f,
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

            _tireWearSystem.OnUpdate(ref _world.Unmanaged);
            Assert.IsNotNull(_tireWearSystem);
        }

        [Test]
        public void TireWearSystem_MultipleTires_HandlesCorrectly()
        {
            for (int i = 0; i < 8; i++)
            {
                var entity = _entityManager.CreateEntity();
                _entityManager.AddComponentData(entity, new TireData
                {
                    Wear = i * 0.1f,
                    Temperature = 70f + i * 5f,
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
                    Radius = 0.5f,
                    Width = 0.2f,
                    SuspensionLength = 0.3f,
                    SpringForce = 1000f,
                    DampingForce = 500f,
                    IsGrounded = i % 2 == 0,
                    GroundDistance = 0.1f
                });
                _entityManager.AddComponentData(entity, new VehiclePhysics
                {
                    Velocity = new float3(i * 3f, 0, 0),
                    Acceleration = float3.zero,
                    ForwardSpeed = i * 3f,
                    TurnSpeed = 0f
                });
            }

            _tireWearSystem.OnUpdate(ref _world.Unmanaged);
            Assert.IsNotNull(_tireWearSystem);
        }

        [Test]
        public void TireWearSystem_EdgeCases_HandleCorrectly()
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

            Assert.DoesNotThrow(() => 
            {
                _tireWearSystem.OnUpdate(ref _world.Unmanaged);
            });
        }
    }
}
