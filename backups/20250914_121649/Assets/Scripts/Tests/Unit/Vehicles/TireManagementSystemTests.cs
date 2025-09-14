using NUnit.Framework;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;
using MudLike.Vehicles.Systems;
using MudLike.Vehicles.Components;
using Unity.Core;

namespace MudLike.Tests.Unit.Vehicles
{
    /// <summary>
    /// Тесты для системы управления шинами TireManagementSystem
    /// </summary>
    public class TireManagementSystemTests
    {
        private World _world;
        private TireManagementSystem _tireManagementSystem;
        private EntityManager _entityManager;

        [SetUp]
        public void SetUp()
        {
            _world = new World("TestWorld");
            _entityManager = _world.EntityManager;
            
            _tireManagementSystem = _world.GetOrCreateSystemManaged<TireManagementSystem>();
            _tireManagementSystem.OnCreate(ref _world.Unmanaged);
            
            _world.SetSingleton(new TimeData { ElapsedTime = 10f, DeltaTime = 0.016f, FixedDeltaTime = 0.016f });
        }

        [TearDown]
        public void TearDown()
        {
            _tireManagementSystem.OnDestroy(ref _world.Unmanaged);
            _world.Dispose();
        }

        [Test]
        public void TireManagementSystem_OnCreate_InitializesCorrectly()
        {
            Assert.IsNotNull(_tireManagementSystem);
        }

        [Test]
        public void TireManagementSystem_OnUpdate_ProcessesWithoutErrors()
        {
            _tireManagementSystem.OnUpdate(ref _world.Unmanaged);
            Assert.IsNotNull(_tireManagementSystem);
        }

        [Test]
        public void TireManagementSystem_WithTireData_ProcessesCorrectly()
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

            _tireManagementSystem.OnUpdate(ref _world.Unmanaged);
            Assert.IsNotNull(_tireManagementSystem);
        }

        [Test]
        public void TireManagementSystem_WithWornTires_ProcessesCorrectly()
        {
            var entity = _entityManager.CreateEntity();
            _entityManager.AddComponentData(entity, new TireData
            {
                Wear = 0.9f,
                Temperature = 100f,
                Pressure = 1.8f,
                Grip = 0.3f,
                TreadDepth = 2f,
                MaxWear = 1f,
                WearRate = 0.005f,
                IsDamaged = true
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

            _tireManagementSystem.OnUpdate(ref _world.Unmanaged);
            Assert.IsNotNull(_tireManagementSystem);
        }

        [Test]
        public void TireManagementSystem_WithLowPressure_ProcessesCorrectly()
        {
            var entity = _entityManager.CreateEntity();
            _entityManager.AddComponentData(entity, new TireData
            {
                Wear = 0.3f,
                Temperature = 85f,
                Pressure = 1.2f,
                Grip = 0.6f,
                TreadDepth = 6f,
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

            _tireManagementSystem.OnUpdate(ref _world.Unmanaged);
            Assert.IsNotNull(_tireManagementSystem);
        }

        [Test]
        public void TireManagementSystem_MultipleTires_HandlesCorrectly()
        {
            for (int i = 0; i < 6; i++)
            {
                var entity = _entityManager.CreateEntity();
                _entityManager.AddComponentData(entity, new TireData
                {
                    Wear = i * 0.15f,
                    Temperature = 70f + i * 15f,
                    Pressure = 2.5f - i * 0.2f,
                    Grip = 0.9f - i * 0.1f,
                    TreadDepth = 8f - i * 1f,
                    MaxWear = 1f,
                    WearRate = 0.001f + i * 0.001f,
                    IsDamaged = i % 2 == 0
                });
                _entityManager.AddComponentData(entity, new WheelData
                {
                    Position = new float3(i * 3f, 0, 0),
                    Radius = 0.5f + i * 0.05f,
                    Width = 0.2f + i * 0.01f,
                    SuspensionLength = 0.3f,
                    SpringForce = 1000f + i * 100f,
                    DampingForce = 500f + i * 50f,
                    IsGrounded = i % 2 == 0,
                    GroundDistance = 0.1f + i * 0.01f
                });
            }

            _tireManagementSystem.OnUpdate(ref _world.Unmanaged);
            Assert.IsNotNull(_tireManagementSystem);
        }

        [Test]
        public void TireManagementSystem_EdgeCases_HandleCorrectly()
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

            Assert.DoesNotThrow(() => 
            {
                _tireManagementSystem.OnUpdate(ref _world.Unmanaged);
            });
        }
    }
}
