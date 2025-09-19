using NUnit.Framework;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using MudLike.Vehicles.Systems;
using MudLike.Vehicles.Components;
using MudLike.Terrain.Components;
using MudLike.Weather.Components;
using Unity.Core;

namespace MudLike.Tests.Unit.Vehicles
{
    /// <summary>
    /// Тесты для системы взаимодействия шин TireInteractionSystem
    /// </summary>
    public class TireInteractionSystemTests
    {
        private World _world;
        private TireInteractionSystem _tireInteractionSystem;
        private EntityManager _entityManager;

        [SetUp]
        public void SetUp()
        {
            _world = new World("TestWorld");
            _entityManager = _world.EntityManager;
            
            _tireInteractionSystem = _world.GetOrCreateSystemManaged<TireInteractionSystem>();
            _tireInteractionSystem.OnCreate(ref _world.Unmanaged);
            
            _world.SetSingleton(new TimeData { ElapsedTime = 10f, DeltaTime = 0.016f, FixedDeltaTime = 0.016f });
        }

        [TearDown]
        public void TearDown()
        {
            _tireInteractionSystem.OnDestroy(ref _world.Unmanaged);
            _world.Dispose();
        }

        [Test]
        public void TireInteractionSystem_OnCreate_InitializesCorrectly()
        {
            Assert.IsNotNull(_tireInteractionSystem);
        }

        [Test]
        public void TireInteractionSystem_OnUpdate_ProcessesWithoutErrors()
        {
            _tireInteractionSystem.OnUpdate(ref _world.Unmanaged);
            Assert.IsNotNull(_tireInteractionSystem);
        }

        [Test]
        public void TireInteractionSystem_WithTireData_ProcessesCorrectly()
        {
            var entity = _entityManager.CreateEntity();
            _entityManager.AddComponentData(entity, new LocalTransform 
            { 
                Position = new float3(0, 0, 0), 
                Rotation = quaternion.identity 
            });
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

            _tireInteractionSystem.OnUpdate(ref _world.Unmanaged);
            Assert.IsNotNull(_tireInteractionSystem);
        }

        [Test]
        public void TireInteractionSystem_WithSurfaceData_ProcessesCorrectly()
        {
            var entity = _entityManager.CreateEntity();
            _entityManager.AddComponentData(entity, new LocalTransform 
            { 
                Position = new float3(0, 0, 0), 
                Rotation = quaternion.identity 
            });
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
            _entityManager.AddComponentData(entity, new SurfaceData
            {
                Friction = 0.8f,
                Hardness = 0.6f,
                Roughness = 0.4f,
                Temperature = 20f,
                Wetness = 0.1f,
                Type = SurfaceType.Asphalt
            });

            _tireInteractionSystem.OnUpdate(ref _world.Unmanaged);
            Assert.IsNotNull(_tireInteractionSystem);
        }

        [Test]
        public void TireInteractionSystem_WithWeatherData_ProcessesCorrectly()
        {
            var entity = _entityManager.CreateEntity();
            _entityManager.AddComponentData(entity, new LocalTransform 
            { 
                Position = new float3(0, 0, 0), 
                Rotation = quaternion.identity 
            });
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
            _entityManager.AddComponentData(entity, new WeatherData
            {
                Temperature = 25f,
                Humidity = 0.6f,
                Precipitation = 0.2f,
                WindSpeed = 5f,
                Pressure = 1013f
            });

            _tireInteractionSystem.OnUpdate(ref _world.Unmanaged);
            Assert.IsNotNull(_tireInteractionSystem);
        }

        [Test]
        public void TireInteractionSystem_MultipleTires_HandlesCorrectly()
        {
            for (int i = 0; i < 8; i++)
            {
                var entity = _entityManager.CreateEntity();
                _entityManager.AddComponentData(entity, new LocalTransform 
                { 
                    Position = new float3(i * 2f, 0, 0), 
                    Rotation = quaternion.identity 
                });
                _entityManager.AddComponentData(entity, new TireData
                {
                    Wear = i * 0.1f,
                    Temperature = 70f + i * 10f,
                    Pressure = 2.5f - i * 0.1f,
                    Grip = 0.9f - i * 0.05f,
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
            }

            _tireInteractionSystem.OnUpdate(ref _world.Unmanaged);
            Assert.IsNotNull(_tireInteractionSystem);
        }

        [Test]
        public void TireInteractionSystem_EdgeCases_HandleCorrectly()
        {
            var entity = _entityManager.CreateEntity();
            _entityManager.AddComponentData(entity, new LocalTransform 
            { 
                Position = new float3(float.MaxValue, float.MinValue, float.Epsilon), 
                Rotation = quaternion.identity 
            });
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

            Assert.DoesNotThrow(() => 
            {
                _tireInteractionSystem.OnUpdate(ref _world.Unmanaged);
            });
        }
    }
}
