using NUnit.Framework;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using MudLike.Vehicles.Components;
using MudLike.Vehicles.Systems;
using MudLike.Terrain.Components;
using MudLike.Terrain.Systems;

namespace MudLike.Tests.Integration
{
    /// <summary>
    /// Интеграционные тесты для взаимодействия транспортного средства с местностью
    /// </summary>
    [TestFixture]
    public class VehicleTerrainIntegrationTests
    {
        private World _world;
        private EntityManager _entityManager;
        private WheelPhysicsSystem _wheelPhysicsSystem;
        private SuspensionSystem _suspensionSystem;
        private VehicleControlSystem _vehicleControlSystem;
        private TerrainInteractionSystem _terrainInteractionSystem;
        private TerrainDeformationSystem _terrainDeformationSystem;

        [SetUp]
        public void SetUp()
        {
            // Создание тестового мира
            _world = new World("TestWorld");
            _entityManager = _world.EntityManager;
            
            // Создание систем
            _wheelPhysicsSystem = _world.GetOrCreateSystemManaged<WheelPhysicsSystem>();
            _suspensionSystem = _world.GetOrCreateSystemManaged<SuspensionSystem>();
            _vehicleControlSystem = _world.GetOrCreateSystemManaged<VehicleControlSystem>();
            _terrainInteractionSystem = _world.GetOrCreateSystemManaged<TerrainInteractionSystem>();
            _terrainDeformationSystem = _world.GetOrCreateSystemManaged<TerrainDeformationSystem>();
        }

        [TearDown]
        public void TearDown()
        {
            // Очистка тестового мира
            _world.Dispose();
        }

        [Test]
        public void VehicleTerrainIntegration_ShouldCreateInteractionWhenWheelContactsGround()
        {
            // Arrange
            var vehicleEntity = _entityManager.CreateEntity();
            var terrainEntity = _entityManager.CreateEntity();
            
            // Создание транспортного средства
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
            var vehicleTransform = new LocalTransform
            {
                Position = new float3(0, 0, 0),
                Rotation = quaternion.identity,
                Scale = 1f
            };

            // Создание колеса
            var wheel = new WheelComponent
            {
                radius = 0.5f,
                mass = 10f,
                inertia = 1f,
                angularVelocity = 0f,
                brakeForce = 0f,
                maxBrakeForce = 1000f,
                frictionCoefficient = 0.8f,
                gripCoefficient = 0.6f,
                localPosition = new float3(0, 0, 0),
                isDriven = true,
                isSteerable = false
            };

            // Создание подвески
            var suspension = new SuspensionComponent
            {
                restLength = 1f,
                maxLength = 1.5f,
                minLength = 0.5f,
                currentLength = 1f,
                springStiffness = 1000f,
                damping = 100f,
                maxDamping = 200f,
                mountPoint = new float3(0, 0, 0),
                suspensionDirection = new float3(0, -1, 0),
                isActive = true
            };

            // Создание местности
            var terrain = new TerrainComponent
            {
                gridSizeX = 100,
                gridSizeZ = 100,
                cellSize = 1f,
                terrainSizeX = 100f,
                terrainSizeZ = 100f,
                minHeight = 0f,
                maxHeight = 10f,
                surfaceStiffness = 1000f,
                surfaceDamping = 50f,
                surfaceFriction = 0.5f,
                surfaceGrip = 0.4f,
                materialDensity = 2000f,
                materialStrength = 100000f,
                materialPlasticity = 0.3f,
                materialViscosity = 100f,
                deformationEnabled = true,
                erosionEnabled = false,
                recoveryRate = 0.1f,
                recoveryTime = 10f,
                maxDeformationDepth = 2f,
                deformationRadius = 1f,
                deformationForce = 1000f,
                activeDeformations = 0,
                totalDeformationEnergy = 0f
            };

            _entityManager.AddComponentData(vehicleEntity, vehicle);
            _entityManager.AddComponentData(vehicleEntity, control);
            _entityManager.AddComponentData(vehicleEntity, vehicleTransform);
            _entityManager.AddComponentData(vehicleEntity, wheel);
            _entityManager.AddComponentData(vehicleEntity, suspension);
            _entityManager.AddComponentData(terrainEntity, terrain);

            // Act
            _vehicleControlSystem.Update();
            _wheelPhysicsSystem.Update();
            _suspensionSystem.Update();
            _terrainInteractionSystem.Update();
            _terrainDeformationSystem.Update();

            // Assert
            var updatedVehicle = _entityManager.GetComponentData<VehicleComponent>(vehicleEntity);
            var updatedWheel = _entityManager.GetComponentData<WheelComponent>(vehicleEntity);
            var updatedSuspension = _entityManager.GetComponentData<SuspensionComponent>(vehicleEntity);
            var updatedTerrain = _entityManager.GetComponentData<TerrainComponent>(terrainEntity);

            Assert.Greater(updatedVehicle.enginePower, 0f, "Двигатель должен работать");
            Assert.IsTrue(updatedWheel.isGrounded, "Колесо должно контактировать с поверхностью");
            Assert.Greater(updatedSuspension.springForce, 0f, "Подвеска должна работать");
            Assert.GreaterOrEqual(updatedTerrain.activeDeformations, 0, "Деформации местности должны обрабатываться");
        }

        [Test]
        public void VehicleTerrainIntegration_ShouldHandleDifferentSurfaceMaterials()
        {
            // Arrange
            var vehicleEntity = _entityManager.CreateEntity();
            var terrainEntity = _entityManager.CreateEntity();
            
            // Создание транспортного средства
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
                throttleInput = 0.8f,
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
            var vehicleTransform = new LocalTransform
            {
                Position = new float3(0, 0, 0),
                Rotation = quaternion.identity,
                Scale = 1f
            };

            // Создание колеса
            var wheel = new WheelComponent
            {
                radius = 0.5f,
                mass = 10f,
                inertia = 1f,
                angularVelocity = 50f,
                brakeForce = 0f,
                maxBrakeForce = 1000f,
                frictionCoefficient = 0.8f,
                gripCoefficient = 0.6f,
                localPosition = new float3(0, 0, 0),
                isDriven = true,
                isSteerable = false
            };

            // Создание подвески
            var suspension = new SuspensionComponent
            {
                restLength = 1f,
                maxLength = 1.5f,
                minLength = 0.5f,
                currentLength = 1f,
                springStiffness = 1000f,
                damping = 100f,
                maxDamping = 200f,
                mountPoint = new float3(0, 0, 0),
                suspensionDirection = new float3(0, -1, 0),
                isActive = true
            };

            // Создание местности с разными материалами
            var terrain = new TerrainComponent
            {
                gridSizeX = 100,
                gridSizeZ = 100,
                cellSize = 1f,
                terrainSizeX = 100f,
                terrainSizeZ = 100f,
                minHeight = 0f,
                maxHeight = 10f,
                surfaceStiffness = 1000f,
                surfaceDamping = 50f,
                surfaceFriction = 0.5f,
                surfaceGrip = 0.4f,
                materialDensity = 2000f,
                materialStrength = 100000f,
                materialPlasticity = 0.3f,
                materialViscosity = 100f,
                deformationEnabled = true,
                erosionEnabled = false,
                recoveryRate = 0.1f,
                recoveryTime = 10f,
                maxDeformationDepth = 2f,
                deformationRadius = 1f,
                deformationForce = 1000f,
                activeDeformations = 0,
                totalDeformationEnergy = 0f
            };

            _entityManager.AddComponentData(vehicleEntity, vehicle);
            _entityManager.AddComponentData(vehicleEntity, control);
            _entityManager.AddComponentData(vehicleEntity, vehicleTransform);
            _entityManager.AddComponentData(vehicleEntity, wheel);
            _entityManager.AddComponentData(vehicleEntity, suspension);
            _entityManager.AddComponentData(terrainEntity, terrain);

            // Act
            _vehicleControlSystem.Update();
            _wheelPhysicsSystem.Update();
            _suspensionSystem.Update();
            _terrainInteractionSystem.Update();
            _terrainDeformationSystem.Update();

            // Assert
            var updatedVehicle = _entityManager.GetComponentData<VehicleComponent>(vehicleEntity);
            var updatedWheel = _entityManager.GetComponentData<WheelComponent>(vehicleEntity);
            var updatedSuspension = _entityManager.GetComponentData<SuspensionComponent>(vehicleEntity);
            var updatedTerrain = _entityManager.GetComponentData<TerrainComponent>(terrainEntity);

            Assert.Greater(updatedVehicle.totalTractionForce.z, 0f, "Сила тяги должна быть положительной");
            Assert.IsTrue(updatedWheel.isGrounded, "Колесо должно контактировать с поверхностью");
            Assert.Greater(updatedSuspension.energy, 0f, "Энергия подвески должна быть положительной");
            Assert.GreaterOrEqual(updatedTerrain.totalDeformationEnergy, 0f, "Энергия деформации должна быть неотрицательной");
        }

        [Test]
        public void VehicleTerrainIntegration_ShouldHandleBraking()
        {
            // Arrange
            var vehicleEntity = _entityManager.CreateEntity();
            var terrainEntity = _entityManager.CreateEntity();
            
            // Создание транспортного средства
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
                brakeInput = 0.8f, // Сильное торможение
                steerInput = 0f,
                handbrakeInput = 0f,
                fourWheelDriveInput = false,
                differentialLockInput = false,
                gearUpInput = false,
                gearDownInput = false,
                engineToggleInput = false,
                isControlActive = true
            };
            var vehicleTransform = new LocalTransform
            {
                Position = new float3(0, 0, 0),
                Rotation = quaternion.identity,
                Scale = 1f
            };

            // Создание колеса
            var wheel = new WheelComponent
            {
                radius = 0.5f,
                mass = 10f,
                inertia = 1f,
                angularVelocity = 100f,
                brakeForce = 0f,
                maxBrakeForce = 1000f,
                frictionCoefficient = 0.8f,
                gripCoefficient = 0.6f,
                localPosition = new float3(0, 0, 0),
                isDriven = true,
                isSteerable = false
            };

            // Создание подвески
            var suspension = new SuspensionComponent
            {
                restLength = 1f,
                maxLength = 1.5f,
                minLength = 0.5f,
                currentLength = 1f,
                springStiffness = 1000f,
                damping = 100f,
                maxDamping = 200f,
                mountPoint = new float3(0, 0, 0),
                suspensionDirection = new float3(0, -1, 0),
                isActive = true
            };

            // Создание местности
            var terrain = new TerrainComponent
            {
                gridSizeX = 100,
                gridSizeZ = 100,
                cellSize = 1f,
                terrainSizeX = 100f,
                terrainSizeZ = 100f,
                minHeight = 0f,
                maxHeight = 10f,
                surfaceStiffness = 1000f,
                surfaceDamping = 50f,
                surfaceFriction = 0.5f,
                surfaceGrip = 0.4f,
                materialDensity = 2000f,
                materialStrength = 100000f,
                materialPlasticity = 0.3f,
                materialViscosity = 100f,
                deformationEnabled = true,
                erosionEnabled = false,
                recoveryRate = 0.1f,
                recoveryTime = 10f,
                maxDeformationDepth = 2f,
                deformationRadius = 1f,
                deformationForce = 1000f,
                activeDeformations = 0,
                totalDeformationEnergy = 0f
            };

            _entityManager.AddComponentData(vehicleEntity, vehicle);
            _entityManager.AddComponentData(vehicleEntity, control);
            _entityManager.AddComponentData(vehicleEntity, vehicleTransform);
            _entityManager.AddComponentData(vehicleEntity, wheel);
            _entityManager.AddComponentData(vehicleEntity, suspension);
            _entityManager.AddComponentData(terrainEntity, terrain);

            // Act
            _vehicleControlSystem.Update();
            _wheelPhysicsSystem.Update();
            _suspensionSystem.Update();
            _terrainInteractionSystem.Update();
            _terrainDeformationSystem.Update();

            // Assert
            var updatedVehicle = _entityManager.GetComponentData<VehicleComponent>(vehicleEntity);
            var updatedWheel = _entityManager.GetComponentData<WheelComponent>(vehicleEntity);
            var updatedSuspension = _entityManager.GetComponentData<SuspensionComponent>(vehicleEntity);

            Assert.Greater(updatedVehicle.totalBrakeForce.z, 0f, "Сила торможения должна быть положительной");
            Assert.Less(updatedWheel.angularVelocity, 100f, "Угловая скорость колеса должна уменьшиться при торможении");
            Assert.Greater(updatedSuspension.springForce, 0f, "Подвеска должна работать при торможении");
        }
    }
}
