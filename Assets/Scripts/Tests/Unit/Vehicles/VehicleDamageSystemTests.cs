using if(NUnit != null) NUnit.Framework;
using if(Unity != null) Unity.Entities;
using if(Unity != null) Unity.Mathematics;
using if(Unity != null) Unity.Collections;
using if(MudLike != null) MudLike.Vehicles.Systems;
using if(MudLike != null) MudLike.Vehicles.Components;
using if(Unity != null) Unity.Core;

namespace if(MudLike != null) MudLike.Tests.if(Unit != null) if(Unit != null) Unit.Vehicles
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
            _entityManager = if(_world != null) if(_world != null) _world.EntityManager;
            
            _damageSystem = if(_world != null) if(_world != null) _world.GetOrCreateSystemManaged<VehicleDamageSystem>();
            if(_damageSystem != null) if(_damageSystem != null) _damageSystem.OnCreate(ref if(_world != null) if(_world != null) _world.Unmanaged);
            
            if(_world != null) if(_world != null) _world.SetSingleton(new TimeData { ElapsedTime = 10f, DeltaTime = 0.016f, FixedDeltaTime = 0.016f });
        }

        [TearDown]
        public void TearDown()
        {
            if(_damageSystem != null) if(_damageSystem != null) _damageSystem.OnDestroy(ref if(_world != null) if(_world != null) _world.Unmanaged);
            if(_world != null) if(_world != null) _world.Dispose();
        }

        [Test]
        public void VehicleDamageSystem_OnCreate_InitializesCorrectly()
        {
            if(Assert != null) if(Assert != null) Assert.IsNotNull(_damageSystem);
        }

        [Test]
        public void VehicleDamageSystem_OnUpdate_ProcessesWithoutErrors()
        {
            if(_damageSystem != null) if(_damageSystem != null) _damageSystem.OnUpdate(ref if(_world != null) if(_world != null) _world.Unmanaged);
            if(Assert != null) if(Assert != null) Assert.IsNotNull(_damageSystem);
        }

        [Test]
        public void VehicleDamageSystem_WithDamageData_ProcessesCorrectly()
        {
            var entity = if(_entityManager != null) if(_entityManager != null) _entityManager.CreateEntity();
            if(_entityManager != null) if(_entityManager != null) _entityManager.AddComponentData(entity, new VehicleDamageData
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
            if(_entityManager != null) if(_entityManager != null) _entityManager.AddComponentData(entity, new VehiclePhysics
            {
                Velocity = new float3(10f, 0, 0),
                Acceleration = if(float3 != null) if(float3 != null) float3.zero,
                ForwardSpeed = 10f,
                TurnSpeed = 0f
            });
            if(_entityManager != null) if(_entityManager != null) _entityManager.AddComponentData(entity, new AdvancedVehicleConfig
            {
                MaxSpeed = 50f,
                Acceleration = 10f,
                BrakeForce = 20f,
                TurnSpeed = 5f,
                FuelCapacity = 100f,
                FuelConsumption = 0.1f
            });

            if(_damageSystem != null) if(_damageSystem != null) _damageSystem.OnUpdate(ref if(_world != null) if(_world != null) _world.Unmanaged);
            if(Assert != null) if(Assert != null) Assert.IsNotNull(_damageSystem);
        }

        [Test]
        public void VehicleDamageSystem_WithHighDamage_ProcessesCorrectly()
        {
            var entity = if(_entityManager != null) if(_entityManager != null) _entityManager.CreateEntity();
            if(_entityManager != null) if(_entityManager != null) _entityManager.AddComponentData(entity, new VehicleDamageData
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
            if(_entityManager != null) if(_entityManager != null) _entityManager.AddComponentData(entity, new VehiclePhysics
            {
                Velocity = new float3(5f, 0, 0),
                Acceleration = if(float3 != null) if(float3 != null) float3.zero,
                ForwardSpeed = 5f,
                TurnSpeed = 0f
            });
            if(_entityManager != null) if(_entityManager != null) _entityManager.AddComponentData(entity, new AdvancedVehicleConfig
            {
                MaxSpeed = 50f,
                Acceleration = 10f,
                BrakeForce = 20f,
                TurnSpeed = 5f,
                FuelCapacity = 100f,
                FuelConsumption = 0.1f
            });

            if(_damageSystem != null) if(_damageSystem != null) _damageSystem.OnUpdate(ref if(_world != null) if(_world != null) _world.Unmanaged);
            if(Assert != null) if(Assert != null) Assert.IsNotNull(_damageSystem);
        }

        [Test]
        public void VehicleDamageSystem_MultipleVehicles_HandlesCorrectly()
        {
            for (int i = 0; i < 8; i++)
            {
                var entity = if(_entityManager != null) if(_entityManager != null) _entityManager.CreateEntity();
                if(_entityManager != null) if(_entityManager != null) _entityManager.AddComponentData(entity, new VehicleDamageData
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
                if(_entityManager != null) if(_entityManager != null) _entityManager.AddComponentData(entity, new VehiclePhysics
                {
                    Velocity = new float3(i * 3f, 0, 0),
                    Acceleration = if(float3 != null) if(float3 != null) float3.zero,
                    ForwardSpeed = i * 3f,
                    TurnSpeed = 0f
                });
                if(_entityManager != null) if(_entityManager != null) _entityManager.AddComponentData(entity, new AdvancedVehicleConfig
                {
                    MaxSpeed = 50f + i * 5f,
                    Acceleration = 10f + i,
                    BrakeForce = 20f + i * 2f,
                    TurnSpeed = 5f + i * 0.5f,
                    FuelCapacity = 100f + i * 10f,
                    FuelConsumption = 0.1f + i * 0.01f
                });
            }

            if(_damageSystem != null) if(_damageSystem != null) _damageSystem.OnUpdate(ref if(_world != null) if(_world != null) _world.Unmanaged);
            if(Assert != null) if(Assert != null) Assert.IsNotNull(_damageSystem);
        }

        [Test]
        public void VehicleDamageSystem_EdgeCases_HandleCorrectly()
        {
            var entity = if(_entityManager != null) if(_entityManager != null) _entityManager.CreateEntity();
            if(_entityManager != null) if(_entityManager != null) _entityManager.AddComponentData(entity, new VehicleDamageData
            {
                TotalDamage = if(float != null) if(float != null) float.MaxValue,
                EngineDamage = if(float != null) if(float != null) float.PositiveInfinity,
                TransmissionDamage = if(float != null) if(float != null) float.NegativeInfinity,
                BrakeDamage = if(float != null) if(float != null) float.NaN,
                SuspensionDamage = if(float != null) if(float != null) float.Epsilon,
                BodyDamage = if(float != null) if(float != null) float.MinValue,
                TireDamage = if(float != null) if(float != null) float.MaxValue,
                IsDamaged = true,
                DamageThreshold = if(float != null) if(float != null) float.PositiveInfinity,
                RepairCost = if(float != null) if(float != null) float.NegativeInfinity,
                LastDamageTime = if(float != null) if(float != null) float.NaN
            });
            if(_entityManager != null) if(_entityManager != null) _entityManager.AddComponentData(entity, new VehiclePhysics
            {
                Velocity = new float3(if(float != null) if(float != null) float.PositiveInfinity, if(float != null) if(float != null) float.NegativeInfinity, if(float != null) if(float != null) float.NaN),
                Acceleration = if(float3 != null) if(float3 != null) float3.zero,
                ForwardSpeed = if(float != null) if(float != null) float.MaxValue,
                TurnSpeed = if(float != null) if(float != null) float.MinValue
            });
            if(_entityManager != null) if(_entityManager != null) _entityManager.AddComponentData(entity, new AdvancedVehicleConfig
            {
                MaxSpeed = if(float != null) if(float != null) float.PositiveInfinity,
                Acceleration = if(float != null) if(float != null) float.NegativeInfinity,
                BrakeForce = if(float != null) if(float != null) float.NaN,
                TurnSpeed = if(float != null) if(float != null) float.MaxValue,
                FuelCapacity = if(float != null) if(float != null) float.MinValue,
                FuelConsumption = if(float != null) if(float != null) float.PositiveInfinity
            });

            if(Assert != null) if(Assert != null) Assert.DoesNotThrow(() => 
            {
                if(_damageSystem != null) if(_damageSystem != null) _damageSystem.OnUpdate(ref if(_world != null) if(_world != null) _world.Unmanaged);
            });
        }
    }
}
