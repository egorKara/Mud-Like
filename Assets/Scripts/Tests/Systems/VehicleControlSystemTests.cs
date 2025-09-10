using NUnit.Framework;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using MudLike.Vehicles.Components;
using MudLike.Vehicles.Systems;

namespace MudLike.Tests.Systems
{
    /// <summary>
    /// Unit тесты для системы управления транспортным средством
    /// </summary>
    [TestFixture]
    public class VehicleControlSystemTests
    {
        private World _world;
        private EntityManager _entityManager;
        private VehicleControlSystem _vehicleControlSystem;

        [SetUp]
        public void SetUp()
        {
            // Создание тестового мира
            _world = new World("TestWorld");
            _entityManager = _world.EntityManager;
            
            // Создание системы
            _vehicleControlSystem = _world.GetOrCreateSystemManaged<VehicleControlSystem>();
        }

        [TearDown]
        public void TearDown()
        {
            // Очистка тестового мира
            _world.Dispose();
        }

        [Test]
        public void VehicleControlSystem_Update_ShouldProcessControlInput()
        {
            // Arrange
            var entity = _entityManager.CreateEntity();
            var vehicle = new VehicleComponent
            {
                mass = 1000f,
                enginePower = 0f,
                maxEnginePower = 200000f,
                engineTorque = 0f,
                maxEngineTorque = 500f,
                engineRPM = 0f,
                maxEngineRPM = 6000f,
                gear = 1,
                gearCount = 6,
                engineOn = true
            };
            var control = new VehicleControlComponent
            {
                throttleInput = 0.5f,
                brakeInput = 0f,
                steerInput = 0f,
                handbrakeInput = 0f,
                fourWheelDriveInput = false,
                differentialLockInput = false,
                gearUpInput = false,
                gearDownInput = false,
                engineToggleInput = false,
                isControlActive = true
            };
            var transform = new LocalTransform
            {
                Position = new float3(0, 0, 0),
                Rotation = quaternion.identity,
                Scale = 1f
            };

            _entityManager.AddComponentData(entity, vehicle);
            _entityManager.AddComponentData(entity, control);
            _entityManager.AddComponentData(entity, transform);

            // Act
            _vehicleControlSystem.Update();

            // Assert
            var updatedVehicle = _entityManager.GetComponentData<VehicleComponent>(entity);
            var updatedControl = _entityManager.GetComponentData<VehicleControlComponent>(entity);
            
            Assert.IsTrue(updatedControl.isControlActive, "Управление должно быть активным");
            Assert.Greater(updatedVehicle.enginePower, 0f, "Мощность двигателя должна увеличиться при нажатии газа");
        }

        [Test]
        public void VehicleControlSystem_Update_ShouldUpdateEngine()
        {
            // Arrange
            var entity = _entityManager.CreateEntity();
            var vehicle = new VehicleComponent
            {
                mass = 1000f,
                enginePower = 0f,
                maxEnginePower = 200000f,
                engineTorque = 0f,
                maxEngineTorque = 500f,
                engineRPM = 0f,
                maxEngineRPM = 6000f,
                gear = 1,
                gearCount = 6,
                engineOn = true
            };
            var control = new VehicleControlComponent
            {
                throttleInput = 1f, // Полный газ
                brakeInput = 0f,
                steerInput = 0f,
                handbrakeInput = 0f,
                fourWheelDriveInput = false,
                differentialLockInput = false,
                gearUpInput = false,
                gearDownInput = false,
                engineToggleInput = false,
                isControlActive = true
            };
            var transform = new LocalTransform
            {
                Position = new float3(0, 0, 0),
                Rotation = quaternion.identity,
                Scale = 1f
            };

            _entityManager.AddComponentData(entity, vehicle);
            _entityManager.AddComponentData(entity, control);
            _entityManager.AddComponentData(entity, transform);

            // Act
            _vehicleControlSystem.Update();

            // Assert
            var updatedVehicle = _entityManager.GetComponentData<VehicleComponent>(entity);
            Assert.Greater(updatedVehicle.enginePower, 0f, "Мощность двигателя должна увеличиться");
            Assert.Greater(updatedVehicle.engineTorque, 0f, "Крутящий момент должен увеличиться");
            Assert.Greater(updatedVehicle.engineRPM, 0f, "Обороты двигателя должны увеличиться");
        }

        [Test]
        public void VehicleControlSystem_Update_ShouldTurnOffEngine()
        {
            // Arrange
            var entity = _entityManager.CreateEntity();
            var vehicle = new VehicleComponent
            {
                mass = 1000f,
                enginePower = 100000f,
                maxEnginePower = 200000f,
                engineTorque = 250f,
                maxEngineTorque = 500f,
                engineRPM = 3000f,
                maxEngineRPM = 6000f,
                gear = 1,
                gearCount = 6,
                engineOn = true
            };
            var control = new VehicleControlComponent
            {
                throttleInput = 0f,
                brakeInput = 0f,
                steerInput = 0f,
                handbrakeInput = 0f,
                fourWheelDriveInput = false,
                differentialLockInput = false,
                gearUpInput = false,
                gearDownInput = false,
                engineToggleInput = true, // Включение/выключение двигателя
                isControlActive = true
            };
            var transform = new LocalTransform
            {
                Position = new float3(0, 0, 0),
                Rotation = quaternion.identity,
                Scale = 1f
            };

            _entityManager.AddComponentData(entity, vehicle);
            _entityManager.AddComponentData(entity, control);
            _entityManager.AddComponentData(entity, transform);

            // Act
            _vehicleControlSystem.Update();

            // Assert
            var updatedVehicle = _entityManager.GetComponentData<VehicleComponent>(entity);
            var updatedControl = _entityManager.GetComponentData<VehicleControlComponent>(entity);
            
            Assert.IsFalse(updatedVehicle.engineOn, "Двигатель должен быть выключен");
            Assert.AreEqual(0f, updatedVehicle.enginePower, "Мощность двигателя должна быть 0");
            Assert.AreEqual(0f, updatedVehicle.engineTorque, "Крутящий момент должен быть 0");
            Assert.AreEqual(0f, updatedVehicle.engineRPM, "Обороты двигателя должны быть 0");
            Assert.IsFalse(updatedControl.engineToggleInput, "Флаг переключения двигателя должен быть сброшен");
        }

        [Test]
        public void VehicleControlSystem_Update_ShouldUpdateTransmission()
        {
            // Arrange
            var entity = _entityManager.CreateEntity();
            var vehicle = new VehicleComponent
            {
                mass = 1000f,
                enginePower = 0f,
                maxEnginePower = 200000f,
                engineTorque = 0f,
                maxEngineTorque = 500f,
                engineRPM = 0f,
                maxEngineRPM = 6000f,
                gear = 1,
                gearCount = 6,
                engineOn = true
            };
            var control = new VehicleControlComponent
            {
                throttleInput = 0f,
                brakeInput = 0f,
                steerInput = 0f,
                handbrakeInput = 0f,
                fourWheelDriveInput = false,
                differentialLockInput = false,
                gearUpInput = true, // Переключение передачи вверх
                gearDownInput = false,
                engineToggleInput = false,
                isControlActive = true
            };
            var transform = new LocalTransform
            {
                Position = new float3(0, 0, 0),
                Rotation = quaternion.identity,
                Scale = 1f
            };

            _entityManager.AddComponentData(entity, vehicle);
            _entityManager.AddComponentData(entity, control);
            _entityManager.AddComponentData(entity, transform);

            // Act
            _vehicleControlSystem.Update();

            // Assert
            var updatedVehicle = _entityManager.GetComponentData<VehicleComponent>(entity);
            var updatedControl = _entityManager.GetComponentData<VehicleControlComponent>(entity);
            
            Assert.AreEqual(2, updatedVehicle.gear, "Передача должна увеличиться");
            Assert.IsFalse(updatedControl.gearUpInput, "Флаг переключения передачи вверх должен быть сброшен");
        }

        [Test]
        public void VehicleControlSystem_Update_ShouldUpdateBrakes()
        {
            // Arrange
            var entity = _entityManager.CreateEntity();
            var vehicle = new VehicleComponent
            {
                mass = 1000f,
                enginePower = 0f,
                maxEnginePower = 200000f,
                engineTorque = 0f,
                maxEngineTorque = 500f,
                engineRPM = 0f,
                maxEngineRPM = 6000f,
                gear = 1,
                gearCount = 6,
                engineOn = true,
                handbrakeOn = false
            };
            var control = new VehicleControlComponent
            {
                throttleInput = 0f,
                brakeInput = 0.5f, // Половина тормоза
                steerInput = 0f,
                handbrakeInput = 0.3f, // Частичный ручной тормоз
                fourWheelDriveInput = false,
                differentialLockInput = false,
                gearUpInput = false,
                gearDownInput = false,
                engineToggleInput = false,
                isControlActive = true
            };
            var transform = new LocalTransform
            {
                Position = new float3(0, 0, 0),
                Rotation = quaternion.identity,
                Scale = 1f
            };

            _entityManager.AddComponentData(entity, vehicle);
            _entityManager.AddComponentData(entity, control);
            _entityManager.AddComponentData(entity, transform);

            // Act
            _vehicleControlSystem.Update();

            // Assert
            var updatedVehicle = _entityManager.GetComponentData<VehicleComponent>(entity);
            Assert.IsTrue(updatedVehicle.handbrakeOn, "Ручной тормоз должен быть включен");
            Assert.Greater(updatedVehicle.totalBrakeForce.z, 0f, "Сила торможения должна быть положительной");
        }

        [Test]
        public void VehicleControlSystem_Update_ShouldCalculateTractionForces()
        {
            // Arrange
            var entity = _entityManager.CreateEntity();
            var vehicle = new VehicleComponent
            {
                mass = 1000f,
                enginePower = 100000f,
                maxEnginePower = 200000f,
                engineTorque = 250f,
                maxEngineTorque = 500f,
                engineRPM = 3000f,
                maxEngineRPM = 6000f,
                gear = 1,
                gearCount = 6,
                engineOn = true,
                finalDriveRatio = 3.5f
            };
            var control = new VehicleControlComponent
            {
                throttleInput = 0.8f, // 80% газа
                brakeInput = 0f,
                steerInput = 0f,
                handbrakeInput = 0f,
                fourWheelDriveInput = false,
                differentialLockInput = false,
                gearUpInput = false,
                gearDownInput = false,
                engineToggleInput = false,
                isControlActive = true
            };
            var transform = new LocalTransform
            {
                Position = new float3(0, 0, 0),
                Rotation = quaternion.identity,
                Scale = 1f
            };

            _entityManager.AddComponentData(entity, vehicle);
            _entityManager.AddComponentData(entity, control);
            _entityManager.AddComponentData(entity, transform);

            // Act
            _vehicleControlSystem.Update();

            // Assert
            var updatedVehicle = _entityManager.GetComponentData<VehicleComponent>(entity);
            Assert.Greater(updatedVehicle.totalTractionForce.z, 0f, "Сила тяги должна быть положительной");
        }
    }
}
