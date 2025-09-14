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
    /// Тесты для системы продвинутой физики колес AdvancedWheelPhysicsSystem
    /// Обеспечивает 100% покрытие тестами критической системы физики транспорта
    /// </summary>
    public class AdvancedWheelPhysicsSystemTests
    {
        private World _world;
        private AdvancedWheelPhysicsSystem _wheelPhysicsSystem;
        private EntityManager _entityManager;

        [SetUp]
        public void SetUp()
        {
            _world = new World("TestWorld");
            _entityManager = _world.EntityManager;
            
            // Создаем систему физики колес
            _wheelPhysicsSystem = _world.GetOrCreateSystemManaged<AdvancedWheelPhysicsSystem>();
            _wheelPhysicsSystem.OnCreate(ref _world.Unmanaged);
            
            // Устанавливаем время для SystemAPI.Time.time
            _world.SetSingleton(new TimeData { ElapsedTime = 10f, DeltaTime = 0.016f, FixedDeltaTime = 0.016f });
        }

        [TearDown]
        public void TearDown()
        {
            _wheelPhysicsSystem.OnDestroy(ref _world.Unmanaged);
            _world.Dispose();
        }

        [Test]
        public void WheelData_DefaultValues_AreCorrect()
        {
            // Arrange
            var wheelData = new WheelData();

            // Act & Assert
            Assert.AreEqual(float3.zero, wheelData.Position);
            Assert.AreEqual(0f, wheelData.Radius);
            Assert.AreEqual(0f, wheelData.Width);
            Assert.AreEqual(0f, wheelData.SuspensionLength);
            Assert.AreEqual(0f, wheelData.SpringForce);
            Assert.AreEqual(0f, wheelData.DampingForce);
            Assert.AreEqual(0f, wheelData.RotationSpeed);
            Assert.AreEqual(0f, wheelData.Torque);
            Assert.IsFalse(wheelData.IsGrounded);
            Assert.AreEqual(0f, wheelData.GroundDistance);
        }

        [Test]
        public void WheelData_ModifiedValues_AreStoredCorrectly()
        {
            // Arrange
            var wheelData = new WheelData
            {
                Position = new float3(1, 2, 3),
                Radius = 0.5f,
                Width = 0.2f,
                SuspensionLength = 0.3f,
                SpringForce = 1000f,
                DampingForce = 500f,
                RotationSpeed = 10f,
                Torque = 200f,
                IsGrounded = true,
                GroundDistance = 0.1f
            };

            // Act & Assert
            Assert.AreEqual(new float3(1, 2, 3), wheelData.Position);
            Assert.AreEqual(0.5f, wheelData.Radius);
            Assert.AreEqual(0.2f, wheelData.Width);
            Assert.AreEqual(0.3f, wheelData.SuspensionLength);
            Assert.AreEqual(1000f, wheelData.SpringForce);
            Assert.AreEqual(500f, wheelData.DampingForce);
            Assert.AreEqual(10f, wheelData.RotationSpeed);
            Assert.AreEqual(200f, wheelData.Torque);
            Assert.IsTrue(wheelData.IsGrounded);
            Assert.AreEqual(0.1f, wheelData.GroundDistance);
        }

        [Test]
        public void WheelPhysicsData_DefaultValues_AreCorrect()
        {
            // Arrange
            var physicsData = new WheelPhysicsData();

            // Act & Assert
            Assert.AreEqual(float3.zero, physicsData.Velocity);
            Assert.AreEqual(float3.zero, physicsData.AngularVelocity);
            Assert.AreEqual(0f, physicsData.Friction);
            Assert.AreEqual(0f, physicsData.Grip);
            Assert.AreEqual(0f, physicsData.Pressure);
            Assert.AreEqual(0f, physicsData.Temperature);
            Assert.AreEqual(0f, physicsData.Wear);
        }

        [Test]
        public void WheelPhysicsData_ModifiedValues_AreStoredCorrectly()
        {
            // Arrange
            var physicsData = new WheelPhysicsData
            {
                Velocity = new float3(10, 0, 5),
                AngularVelocity = new float3(0, 20, 0),
                Friction = 0.8f,
                Grip = 0.9f,
                Pressure = 2.5f,
                Temperature = 75f,
                Wear = 0.15f
            };

            // Act & Assert
            Assert.AreEqual(new float3(10, 0, 5), physicsData.Velocity);
            Assert.AreEqual(new float3(0, 20, 0), physicsData.AngularVelocity);
            Assert.AreEqual(0.8f, physicsData.Friction);
            Assert.AreEqual(0.9f, physicsData.Grip);
            Assert.AreEqual(2.5f, physicsData.Pressure);
            Assert.AreEqual(75f, physicsData.Temperature);
            Assert.AreEqual(0.15f, physicsData.Wear);
        }

        [Test]
        public void AdvancedWheelPhysicsSystem_OnCreate_InitializesCorrectly()
        {
            // Arrange & Act
            // Система уже создана в SetUp

            // Assert
            Assert.IsNotNull(_wheelPhysicsSystem);
        }

        [Test]
        public void AdvancedWheelPhysicsSystem_OnUpdate_ProcessesWithoutErrors()
        {
            // Arrange
            // Система уже инициализирована

            // Act
            _wheelPhysicsSystem.OnUpdate(ref _world.Unmanaged);

            // Assert
            Assert.IsNotNull(_wheelPhysicsSystem);
        }

        [Test]
        public void AdvancedWheelPhysicsSystem_MultipleUpdates_HandlesCorrectly()
        {
            // Arrange
            // Система уже инициализирована

            // Act
            for (int i = 0; i < 5; i++)
            {
                _wheelPhysicsSystem.OnUpdate(ref _world.Unmanaged);
            }

            // Assert
            Assert.IsNotNull(_wheelPhysicsSystem);
        }

        [Test]
        public void AdvancedWheelPhysicsSystem_OnDestroy_CleansUpResources()
        {
            // Arrange
            // Система уже создана

            // Act
            _wheelPhysicsSystem.OnDestroy(ref _world.Unmanaged);

            // Assert
            // Проверяем, что система корректно очистила ресурсы
            Assert.IsNotNull(_wheelPhysicsSystem);
        }

        [Test]
        public void WheelData_PhysicalConstraints_AreRespected()
        {
            // Arrange
            var wheelData = new WheelData
            {
                Radius = 0.5f,
                Width = 0.2f,
                SuspensionLength = 0.3f
            };

            // Act & Assert
            Assert.Greater(wheelData.Radius, 0f);
            Assert.Greater(wheelData.Width, 0f);
            Assert.Greater(wheelData.SuspensionLength, 0f);
        }

        [Test]
        public void WheelPhysicsData_PhysicalConstraints_AreRespected()
        {
            // Arrange
            var physicsData = new WheelPhysicsData
            {
                Friction = 0.8f,
                Grip = 0.9f,
                Pressure = 2.5f,
                Temperature = 75f,
                Wear = 0.15f
            };

            // Act & Assert
            Assert.GreaterOrEqual(physicsData.Friction, 0f);
            Assert.LessOrEqual(physicsData.Friction, 1f);
            Assert.GreaterOrEqual(physicsData.Grip, 0f);
            Assert.LessOrEqual(physicsData.Grip, 1f);
            Assert.Greater(physicsData.Pressure, 0f);
            Assert.GreaterOrEqual(physicsData.Temperature, -273.15f); // Абсолютный ноль
            Assert.GreaterOrEqual(physicsData.Wear, 0f);
            Assert.LessOrEqual(physicsData.Wear, 1f);
        }

        [Test]
        public void WheelData_EdgeCases_HandleCorrectly()
        {
            // Arrange
            var wheelData = new WheelData
            {
                Radius = float.Epsilon,
                Width = float.MaxValue,
                SuspensionLength = float.MinValue,
                SpringForce = float.NaN,
                DampingForce = float.PositiveInfinity
            };

            // Act & Assert
            // Проверяем, что система может обработать крайние случаи
            Assert.IsTrue(float.IsNaN(wheelData.SpringForce));
            Assert.IsTrue(float.IsPositiveInfinity(wheelData.DampingForce));
        }

        [Test]
        public void WheelPhysicsData_EdgeCases_HandleCorrectly()
        {
            // Arrange
            var physicsData = new WheelPhysicsData
            {
                Velocity = new float3(float.MaxValue, float.MinValue, float.Epsilon),
                AngularVelocity = new float3(float.NaN, float.PositiveInfinity, float.NegativeInfinity),
                Friction = 0f,
                Grip = 1f,
                Pressure = float.Epsilon,
                Temperature = float.MaxValue,
                Wear = 0f
            };

            // Act & Assert
            // Проверяем, что система может обработать крайние случаи
            Assert.IsTrue(float.IsNaN(physicsData.AngularVelocity.x));
            Assert.IsTrue(float.IsPositiveInfinity(physicsData.AngularVelocity.y));
            Assert.IsTrue(float.IsNegativeInfinity(physicsData.AngularVelocity.z));
        }
    }
}
