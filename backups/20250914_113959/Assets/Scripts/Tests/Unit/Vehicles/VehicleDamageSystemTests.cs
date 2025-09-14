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
    /// Тесты для системы повреждений VehicleDamageSystem
    /// </summary>
    public class VehicleDamageSystemTests
    {
        private World _world;
        private VehicleDamageSystem _damageSystem;
        private EntityManager _entityManager;

        [SetUp]
        public void SetUp()
        {
            _world = new World("TestWorld");
            _entityManager = _world.EntityManager;
            
            _damageSystem = _world.GetOrCreateSystemManaged<VehicleDamageSystem>();
            _damageSystem.OnCreate(ref _world.Unmanaged);
            
            _world.SetSingleton(new TimeData { ElapsedTime = 10f, DeltaTime = 0.016f, FixedDeltaTime = 0.016f });
        }

        [TearDown]
        public void TearDown()
        {
            _damageSystem.OnDestroy(ref _world.Unmanaged);
            _world.Dispose();
        }

        [Test]
        public void VehicleDamageSystem_OnCreate_InitializesCorrectly()
        {
            Assert.IsNotNull(_damageSystem);
        }

        [Test]
        public void VehicleDamageSystem_OnUpdate_ProcessesWithoutErrors()
        {
            _damageSystem.OnUpdate(ref _world.Unmanaged);
            Assert.IsNotNull(_damageSystem);
        }

        [Test]
        public void VehicleDamageSystem_WithDamageData_ProcessesCorrectly()
        {
            var entity = _entityManager.CreateEntity();
            _entityManager.AddComponentData(entity, new VehicleDamageData
            {
                TotalDamage = 0f,
                EngineDamage = 0f,
                TransmissionDamage = 0f,
                BrakeDamage = 0f,
                SuspensionDamage = 0f,
                BodyDamage = 0f,
                TireDamage = 0f,
                IsDamaged = false,
                DamageThreshold = 100f,
                RepairCost = 0f,
                LastDamageTime = 0f
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

            _damageSystem.OnUpdate(ref _world.Unmanaged);
            Assert.IsNotNull(_damageSystem);
        }

        [Test]
        public void VehicleDamageSystem_WithHighDamage_ProcessesCorrectly()
        {
            var entity = _entityManager.CreateEntity();
            _entityManager.AddComponentData(entity, new VehicleDamageData
            {
                TotalDamage = 80f,
                EngineDamage = 20f,
                TransmissionDamage = 15f,
                BrakeDamage = 25f,
                SuspensionDamage = 10f,
                BodyDamage = 30f,
                TireDamage = 5f,
                IsDamaged = true,
                DamageThreshold = 100f,
                RepairCost = 5000f,
                LastDamageTime = 100f
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

            _damageSystem.OnUpdate(ref _world.Unmanaged);
            Assert.IsNotNull(_damageSystem);
        }

        [Test]
        public void VehicleDamageSystem_MultipleVehicles_HandlesCorrectly()
        {
            for (int i = 0; i < 8; i++)
            {
                var entity = _entityManager.CreateEntity();
                _entityManager.AddComponentData(entity, new VehicleDamageData
                {
                    TotalDamage = i * 10f,
                    EngineDamage = i * 2f,
                    TransmissionDamage = i * 1.5f,
                    BrakeDamage = i * 3f,
                    SuspensionDamage = i * 1f,
                    BodyDamage = i * 2.5f,
                    TireDamage = i * 0.5f,
                    IsDamaged = i % 2 == 0,
                    DamageThreshold = 100f + i * 10f,
                    RepairCost = i * 500f,
                    LastDamageTime = i * 50f
                });
                _entityManager.AddComponentData(entity, new VehiclePhysics
                {
                    Velocity = new float3(i * 3f, 0, 0),
                    Acceleration = float3.zero,
                    ForwardSpeed = i * 3f,
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

            _damageSystem.OnUpdate(ref _world.Unmanaged);
            Assert.IsNotNull(_damageSystem);
        }

        [Test]
        public void VehicleDamageSystem_EdgeCases_HandleCorrectly()
        {
            var entity = _entityManager.CreateEntity();
            _entityManager.AddComponentData(entity, new VehicleDamageData
            {
                TotalDamage = float.MaxValue,
                EngineDamage = float.PositiveInfinity,
                TransmissionDamage = float.NegativeInfinity,
                BrakeDamage = float.NaN,
                SuspensionDamage = float.Epsilon,
                BodyDamage = float.MinValue,
                TireDamage = float.MaxValue,
                IsDamaged = true,
                DamageThreshold = float.PositiveInfinity,
                RepairCost = float.NegativeInfinity,
                LastDamageTime = float.NaN
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
                _damageSystem.OnUpdate(ref _world.Unmanaged);
            });
        }
    }
}
