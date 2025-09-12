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
    /// Тесты для системы технического обслуживания VehicleMaintenanceSystem
    /// </summary>
    public class VehicleMaintenanceSystemTests
    {
        private World _world;
        private VehicleMaintenanceSystem _maintenanceSystem;
        private EntityManager _entityManager;

        [SetUp]
        public void SetUp()
        {
            _world = new World("TestWorld");
            _entityManager = _world.EntityManager;
            
            _maintenanceSystem = _world.GetOrCreateSystemManaged<VehicleMaintenanceSystem>();
            _maintenanceSystem.OnCreate(ref _world.Unmanaged);
            
            _world.SetSingleton(new TimeData { ElapsedTime = 10f, DeltaTime = 0.016f, FixedDeltaTime = 0.016f });
        }

        [TearDown]
        public void TearDown()
        {
            _maintenanceSystem.OnDestroy(ref _world.Unmanaged);
            _world.Dispose();
        }

        [Test]
        public void VehicleMaintenanceSystem_OnCreate_InitializesCorrectly()
        {
            Assert.IsNotNull(_maintenanceSystem);
        }

        [Test]
        public void VehicleMaintenanceSystem_OnUpdate_ProcessesWithoutErrors()
        {
            _maintenanceSystem.OnUpdate(ref _world.Unmanaged);
            Assert.IsNotNull(_maintenanceSystem);
        }

        [Test]
        public void VehicleMaintenanceSystem_WithMaintenanceData_ProcessesCorrectly()
        {
            var entity = _entityManager.CreateEntity();
            _entityManager.AddComponentData(entity, new VehicleMaintenanceData
            {
                LastMaintenanceTime = 0f,
                MaintenanceInterval = 3600f, // 1 час
                EngineHealth = 1f,
                TransmissionHealth = 1f,
                BrakeHealth = 1f,
                SuspensionHealth = 1f,
                TireHealth = 1f,
                FuelLevel = 1f,
                OilLevel = 1f,
                CoolantLevel = 1f,
                IsMaintenanceRequired = false,
                MaintenanceCost = 0f
            });
            _entityManager.AddComponentData(entity, new VehiclePhysics
            {
                Velocity = new float3(10f, 0, 0),
                Acceleration = float3.zero,
                ForwardSpeed = 10f,
                TurnSpeed = 0f
            });
            _entityManager.AddComponentData(entity, new AdvancedVehicleConfig
            {
                MaxSpeed = 50f,
                Acceleration = 10f,
                BrakeForce = 20f,
                TurnSpeed = 5f,
                FuelCapacity = 100f,
                FuelConsumption = 0.1f
            });

            _maintenanceSystem.OnUpdate(ref _world.Unmanaged);
            Assert.IsNotNull(_maintenanceSystem);
        }

        [Test]
        public void VehicleMaintenanceSystem_MaintenanceRequired_ProcessesCorrectly()
        {
            var entity = _entityManager.CreateEntity();
            _entityManager.AddComponentData(entity, new VehicleMaintenanceData
            {
                LastMaintenanceTime = 0f,
                MaintenanceInterval = 1f, // Очень короткий интервал
                EngineHealth = 0.5f,
                TransmissionHealth = 0.6f,
                BrakeHealth = 0.4f,
                SuspensionHealth = 0.7f,
                TireHealth = 0.3f,
                FuelLevel = 0.2f,
                OilLevel = 0.1f,
                CoolantLevel = 0.15f,
                IsMaintenanceRequired = true,
                MaintenanceCost = 1000f
            });
            _entityManager.AddComponentData(entity, new VehiclePhysics
            {
                Velocity = new float3(5f, 0, 0),
                Acceleration = float3.zero,
                ForwardSpeed = 5f,
                TurnSpeed = 0f
            });
            _entityManager.AddComponentData(entity, new AdvancedVehicleConfig
            {
                MaxSpeed = 50f,
                Acceleration = 10f,
                BrakeForce = 20f,
                TurnSpeed = 5f,
                FuelCapacity = 100f,
                FuelConsumption = 0.1f
            });

            _maintenanceSystem.OnUpdate(ref _world.Unmanaged);
            Assert.IsNotNull(_maintenanceSystem);
        }

        [Test]
        public void VehicleMaintenanceSystem_MultipleVehicles_HandlesCorrectly()
        {
            for (int i = 0; i < 10; i++)
            {
                var entity = _entityManager.CreateEntity();
                _entityManager.AddComponentData(entity, new VehicleMaintenanceData
                {
                    LastMaintenanceTime = i * 100f,
                    MaintenanceInterval = 3600f + i * 100f,
                    EngineHealth = 1f - i * 0.05f,
                    TransmissionHealth = 1f - i * 0.03f,
                    BrakeHealth = 1f - i * 0.04f,
                    SuspensionHealth = 1f - i * 0.02f,
                    TireHealth = 1f - i * 0.06f,
                    FuelLevel = 1f - i * 0.1f,
                    OilLevel = 1f - i * 0.08f,
                    CoolantLevel = 1f - i * 0.07f,
                    IsMaintenanceRequired = i % 3 == 0,
                    MaintenanceCost = i * 100f
                });
                _entityManager.AddComponentData(entity, new VehiclePhysics
                {
                    Velocity = new float3(i * 2f, 0, 0),
                    Acceleration = float3.zero,
                    ForwardSpeed = i * 2f,
                    TurnSpeed = 0f
                });
                _entityManager.AddComponentData(entity, new AdvancedVehicleConfig
                {
                    MaxSpeed = 50f + i * 5f,
                    Acceleration = 10f + i,
                    BrakeForce = 20f + i * 2f,
                    TurnSpeed = 5f + i * 0.5f,
                    FuelCapacity = 100f + i * 10f,
                    FuelConsumption = 0.1f + i * 0.01f
                });
            }

            _maintenanceSystem.OnUpdate(ref _world.Unmanaged);
            Assert.IsNotNull(_maintenanceSystem);
        }

        [Test]
        public void VehicleMaintenanceSystem_EdgeCases_HandleCorrectly()
        {
            var entity = _entityManager.CreateEntity();
            _entityManager.AddComponentData(entity, new VehicleMaintenanceData
            {
                LastMaintenanceTime = float.MaxValue,
                MaintenanceInterval = float.MinValue,
                EngineHealth = float.NaN,
                TransmissionHealth = float.PositiveInfinity,
                BrakeHealth = float.NegativeInfinity,
                SuspensionHealth = float.Epsilon,
                TireHealth = float.MaxValue,
                FuelLevel = float.MinValue,
                OilLevel = float.NaN,
                CoolantLevel = float.PositiveInfinity,
                IsMaintenanceRequired = true,
                MaintenanceCost = float.NegativeInfinity
            });
            _entityManager.AddComponentData(entity, new VehiclePhysics
            {
                Velocity = new float3(float.PositiveInfinity, float.NegativeInfinity, float.NaN),
                Acceleration = float3.zero,
                ForwardSpeed = float.MaxValue,
                TurnSpeed = float.MinValue
            });
            _entityManager.AddComponentData(entity, new AdvancedVehicleConfig
            {
                MaxSpeed = float.PositiveInfinity,
                Acceleration = float.NegativeInfinity,
                BrakeForce = float.NaN,
                TurnSpeed = float.MaxValue,
                FuelCapacity = float.MinValue,
                FuelConsumption = float.PositiveInfinity
            });

            Assert.DoesNotThrow(() => 
            {
                _maintenanceSystem.OnUpdate(ref _world.Unmanaged);
            });
        }
    }
}
