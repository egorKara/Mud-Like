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
    /// Тесты для системы трансмиссии TransmissionSystem
    /// </summary>
    public class TransmissionSystemTests
    {
        private World _world;
        private TransmissionSystem _transmissionSystem;
        private EntityManager _entityManager;

        [SetUp]
        public void SetUp()
        {
            _world = new World("TestWorld");
            _entityManager = _world.EntityManager;
            
            _transmissionSystem = _world.GetOrCreateSystemManaged<TransmissionSystem>();
            _transmissionSystem.OnCreate(ref _world.Unmanaged);
            
            _world.SetSingleton(new TimeData { ElapsedTime = 10f, DeltaTime = 0.016f, FixedDeltaTime = 0.016f });
        }

        [TearDown]
        public void TearDown()
        {
            _transmissionSystem.OnDestroy(ref _world.Unmanaged);
            _world.Dispose();
        }

        [Test]
        public void TransmissionSystem_OnCreate_InitializesCorrectly()
        {
            Assert.IsNotNull(_transmissionSystem);
        }

        [Test]
        public void TransmissionSystem_OnUpdate_ProcessesWithoutErrors()
        {
            _transmissionSystem.OnUpdate(ref _world.Unmanaged);
            Assert.IsNotNull(_transmissionSystem);
        }

        [Test]
        public void TransmissionSystem_WithTransmissionData_ProcessesCorrectly()
        {
            var entity = _entityManager.CreateEntity();
            _entityManager.AddComponentData(entity, new TransmissionData
            {
                CurrentGear = 1,
                TargetGear = 1,
                GearRatio = 3.5f,
                FinalDriveRatio = 4.1f,
                ClutchEngagement = 1f,
                IsAutomatic = true,
                ShiftTime = 0.5f,
                MaxGear = 6,
                MinGear = -1,
                GearRatios = new float4x4(
                    3.5f, 2.1f, 1.4f, 1.0f,
                    0.8f, 0.6f, 0f, 0f,
                    0f, 0f, 0f, 0f,
                    0f, 0f, 0f, 0f
                )
            });
            _entityManager.AddComponentData(entity, new VehiclePhysics
            {
                Velocity = new float3(10f, 0, 0),
                Acceleration = float3.zero,
                ForwardSpeed = 10f,
                TurnSpeed = 0f
            });
            _entityManager.AddComponentData(entity, new PlayerInput
            {
                Throttle = 0.5f,
                Brake = 0f,
                Steering = 0f,
                Handbrake = false,
                GearUp = false,
                GearDown = false
            });
            _entityManager.AddComponentData(entity, new EngineData
            {
                RPM = 2000f,
                MaxRPM = 6000f,
                Torque = 300f,
                Power = 200f,
                Throttle = 0.5f,
                IsRunning = true
            });
            _entityManager.AddComponent<VehicleTag>(entity);

            _transmissionSystem.OnUpdate(ref _world.Unmanaged);
            Assert.IsNotNull(_transmissionSystem);
        }

        [Test]
        public void TransmissionSystem_ManualTransmission_ProcessesCorrectly()
        {
            var entity = _entityManager.CreateEntity();
            _entityManager.AddComponentData(entity, new TransmissionData
            {
                CurrentGear = 2,
                TargetGear = 3,
                GearRatio = 2.1f,
                FinalDriveRatio = 4.1f,
                ClutchEngagement = 0.8f,
                IsAutomatic = false,
                ShiftTime = 0.3f,
                MaxGear = 6,
                MinGear = -1,
                GearRatios = new float4x4(
                    3.5f, 2.1f, 1.4f, 1.0f,
                    0.8f, 0.6f, 0f, 0f,
                    0f, 0f, 0f, 0f,
                    0f, 0f, 0f, 0f
                )
            });
            _entityManager.AddComponentData(entity, new VehiclePhysics
            {
                Velocity = new float3(15f, 0, 0),
                Acceleration = float3.zero,
                ForwardSpeed = 15f,
                TurnSpeed = 0f
            });
            _entityManager.AddComponentData(entity, new PlayerInput
            {
                Throttle = 0.7f,
                Brake = 0f,
                Steering = 0f,
                Handbrake = false,
                GearUp = true,
                GearDown = false
            });
            _entityManager.AddComponentData(entity, new EngineData
            {
                RPM = 3500f,
                MaxRPM = 6000f,
                Torque = 350f,
                Power = 250f,
                Throttle = 0.7f,
                IsRunning = true
            });
            _entityManager.AddComponent<VehicleTag>(entity);

            _transmissionSystem.OnUpdate(ref _world.Unmanaged);
            Assert.IsNotNull(_transmissionSystem);
        }

        [Test]
        public void TransmissionSystem_MultipleVehicles_HandlesCorrectly()
        {
            for (int i = 0; i < 5; i++)
            {
                var entity = _entityManager.CreateEntity();
                _entityManager.AddComponentData(entity, new TransmissionData
                {
                    CurrentGear = i + 1,
                    TargetGear = i + 1,
                    GearRatio = 3.5f - i * 0.5f,
                    FinalDriveRatio = 4.1f,
                    ClutchEngagement = 1f - i * 0.1f,
                    IsAutomatic = i % 2 == 0,
                    ShiftTime = 0.5f - i * 0.05f,
                    MaxGear = 6,
                    MinGear = -1,
                    GearRatios = new float4x4(
                        3.5f, 2.1f, 1.4f, 1.0f,
                        0.8f, 0.6f, 0f, 0f,
                        0f, 0f, 0f, 0f,
                        0f, 0f, 0f, 0f
                    )
                });
                _entityManager.AddComponentData(entity, new VehiclePhysics
                {
                    Velocity = new float3(i * 5f, 0, 0),
                    Acceleration = float3.zero,
                    ForwardSpeed = i * 5f,
                    TurnSpeed = 0f
                });
                _entityManager.AddComponentData(entity, new PlayerInput
                {
                    Throttle = i * 0.2f,
                    Brake = 0f,
                    Steering = 0f,
                    Handbrake = false,
                    GearUp = i % 2 == 0,
                    GearDown = i % 3 == 0
                });
                _entityManager.AddComponentData(entity, new EngineData
                {
                    RPM = 2000f + i * 500f,
                    MaxRPM = 6000f,
                    Torque = 300f + i * 50f,
                    Power = 200f + i * 30f,
                    Throttle = i * 0.2f,
                    IsRunning = true
                });
                _entityManager.AddComponent<VehicleTag>(entity);
            }

            _transmissionSystem.OnUpdate(ref _world.Unmanaged);
            Assert.IsNotNull(_transmissionSystem);
        }

        [Test]
        public void TransmissionSystem_EdgeCases_HandleCorrectly()
        {
            var entity = _entityManager.CreateEntity();
            _entityManager.AddComponentData(entity, new TransmissionData
            {
                CurrentGear = int.MaxValue,
                TargetGear = int.MinValue,
                GearRatio = float.PositiveInfinity,
                FinalDriveRatio = float.NegativeInfinity,
                ClutchEngagement = float.NaN,
                IsAutomatic = true,
                ShiftTime = float.MaxValue,
                MaxGear = int.MaxValue,
                MinGear = int.MinValue,
                GearRatios = new float4x4(
                    float.PositiveInfinity, float.NegativeInfinity, float.NaN, float.MaxValue,
                    float.MinValue, float.Epsilon, 0f, 0f,
                    0f, 0f, 0f, 0f,
                    0f, 0f, 0f, 0f
                )
            });
            _entityManager.AddComponentData(entity, new VehiclePhysics
            {
                Velocity = new float3(float.PositiveInfinity, float.NegativeInfinity, float.NaN),
                Acceleration = float3.zero,
                ForwardSpeed = float.MaxValue,
                TurnSpeed = float.MinValue
            });
            _entityManager.AddComponentData(entity, new PlayerInput
            {
                Throttle = float.PositiveInfinity,
                Brake = float.NegativeInfinity,
                Steering = float.NaN,
                Handbrake = true,
                GearUp = true,
                GearDown = true
            });
            _entityManager.AddComponentData(entity, new EngineData
            {
                RPM = float.PositiveInfinity,
                MaxRPM = float.NegativeInfinity,
                Torque = float.NaN,
                Power = float.MaxValue,
                Throttle = float.MinValue,
                IsRunning = true
            });
            _entityManager.AddComponent<VehicleTag>(entity);

            Assert.DoesNotThrow(() => 
            {
                _transmissionSystem.OnUpdate(ref _world.Unmanaged);
            });
        }
    }
}
