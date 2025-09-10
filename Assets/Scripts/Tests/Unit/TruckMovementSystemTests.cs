using NUnit.Framework;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using MudLike.Vehicles.Components;
using MudLike.Vehicles.Systems;

namespace MudLike.Tests.Unit
{
    /// <summary>
    /// Unit тесты для системы движения грузовика
    /// </summary>
    public class TruckMovementSystemTests
    {
        private World _world;
        private EntityManager _entityManager;
        private TruckMovementSystem _truckSystem;

        [SetUp]
        public void Setup()
        {
            // Создаем тестовый мир
            _world = new World("TestWorld");
            _entityManager = _world.EntityManager;
            
            // Создаем систему движения грузовика
            _truckSystem = _world.GetOrCreateSystemManaged<TruckMovementSystem>();
        }

        [TearDown]
        public void TearDown()
        {
            // Очищаем тестовый мир
            _world?.Dispose();
        }

        [Test]
        public void CalculateEngineTorque_ValidInput_ReturnsCorrectTorque()
        {
            // Arrange
            var truck = new TruckData
            {
                EngineRPM = 1500f,
                MaxTorque = 1200f
            };
            
            var input = new TruckControl
            {
                Throttle = 0.5f
            };

            // Act
            var result = TruckMovementSystem.CalculateEngineTorque(truck, input);

            // Assert
            Assert.Greater(result, 0f);
            Assert.LessOrEqual(result, truck.MaxTorque);
        }

        [Test]
        public void GetGearRatio_ValidGear_ReturnsCorrectRatio()
        {
            // Act & Assert
            Assert.AreEqual(3.5f, TruckMovementSystem.GetGearRatio(1), 0.001f);
            Assert.AreEqual(2.1f, TruckMovementSystem.GetGearRatio(2), 0.001f);
            Assert.AreEqual(1.4f, TruckMovementSystem.GetGearRatio(3), 0.001f);
            Assert.AreEqual(1.0f, TruckMovementSystem.GetGearRatio(4), 0.001f);
            Assert.AreEqual(0.8f, TruckMovementSystem.GetGearRatio(5), 0.001f);
            Assert.AreEqual(0.6f, TruckMovementSystem.GetGearRatio(6), 0.001f);
            Assert.AreEqual(0f, TruckMovementSystem.GetGearRatio(0), 0.001f);
        }

        [Test]
        public void CalculateTargetRPM_ThrottleInput_ReturnsCorrectRPM()
        {
            // Arrange
            var truck = new TruckData
            {
                EngineRPM = 1000f
            };
            
            var input = new TruckControl
            {
                Throttle = 0.8f
            };

            // Act
            var result = TruckMovementSystem.CalculateTargetRPM(truck, input);

            // Assert
            Assert.Greater(result, 1000f); // Должно быть больше холостого хода
            Assert.LessOrEqual(result, 2500f); // Не должно превышать максимум
        }

        [Test]
        public void CalculateTargetRPM_NoThrottle_ReturnsIdleRPM()
        {
            // Arrange
            var truck = new TruckData
            {
                EngineRPM = 1000f
            };
            
            var input = new TruckControl
            {
                Throttle = 0f
            };

            // Act
            var result = TruckMovementSystem.CalculateTargetRPM(truck, input);

            // Assert
            Assert.AreEqual(800f, result, 0.001f); // Холостой ход
        }

        [Test]
        public void CalculateTractionForce_EngineOff_ReturnsZero()
        {
            // Arrange
            var truck = new TruckData
            {
                EngineRunning = false,
                Mass = 8000f,
                MaxTorque = 1200f,
                CurrentGear = 1,
                TractionCoefficient = 0.8f
            };
            
            var input = new TruckControl
            {
                Throttle = 1f
            };

            // Act
            var result = TruckMovementSystem.CalculateTractionForce(truck, input);

            // Assert
            Assert.AreEqual(float3.zero, result);
        }

        [Test]
        public void CalculateTractionForce_HandbrakeOn_ReturnsZero()
        {
            // Arrange
            var truck = new TruckData
            {
                EngineRunning = true,
                HandbrakeOn = true,
                Mass = 8000f,
                MaxTorque = 1200f,
                CurrentGear = 1,
                TractionCoefficient = 0.8f
            };
            
            var input = new TruckControl
            {
                Throttle = 1f
            };

            // Act
            var result = TruckMovementSystem.CalculateTractionForce(truck, input);

            // Assert
            Assert.AreEqual(float3.zero, result);
        }

        [Test]
        public void CalculateTractionForce_ValidInput_ReturnsForwardForce()
        {
            // Arrange
            var truck = new TruckData
            {
                EngineRunning = true,
                HandbrakeOn = false,
                Mass = 8000f,
                MaxTorque = 1200f,
                CurrentGear = 1,
                TractionCoefficient = 0.8f,
                LockFrontDifferential = false,
                LockMiddleDifferential = false,
                LockRearDifferential = false,
                LockCenterDifferential = false
            };
            
            var input = new TruckControl
            {
                Throttle = 0.5f
            };

            // Act
            var result = TruckMovementSystem.CalculateTractionForce(truck, input);

            // Assert
            Assert.Greater(result.z, 0f); // Должна быть сила вперед
            Assert.AreEqual(0f, result.x, 0.001f); // Нет боковой силы
            Assert.AreEqual(0f, result.y, 0.001f); // Нет вертикальной силы
        }
        
        [Test]
        public void UpdateTransmission_WithDifferentialLocks_UpdatesCorrectly()
        {
            // Arrange
            var truck = new TruckData
            {
                CurrentGear = 1,
                LockFrontDifferential = false,
                LockMiddleDifferential = false,
                LockRearDifferential = false,
                LockCenterDifferential = false
            };
            
            var input = new TruckControl
            {
                LockFrontDifferential = true,
                LockMiddleDifferential = true,
                LockRearDifferential = false,
                LockCenterDifferential = true
            };

            // Act
            TruckMovementSystem.UpdateTransmission(ref truck, input, 0.1f);

            // Assert
            Assert.IsTrue(truck.LockFrontDifferential);
            Assert.IsTrue(truck.LockMiddleDifferential);
            Assert.IsFalse(truck.LockRearDifferential);
            Assert.IsTrue(truck.LockCenterDifferential);
        }
    }
}