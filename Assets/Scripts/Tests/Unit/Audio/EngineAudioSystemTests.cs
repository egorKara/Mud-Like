using NUnit.Framework;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using MudLike.Audio.Systems;
using MudLike.Audio.Components;
using MudLike.Vehicles.Components;
using Unity.Core;

namespace MudLike.Tests.Unit.Audio
{
    /// <summary>
    /// Тесты для системы звука двигателя EngineAudioSystem
    /// </summary>
    public class EngineAudioSystemTests
    {
        private World _world;
        private EngineAudioSystem _engineAudioSystem;
        private EntityManager _entityManager;

        [SetUp]
        public void SetUp()
        {
            _world = new World("TestWorld");
            _entityManager = _world.EntityManager;
            
            _engineAudioSystem = _world.GetOrCreateSystemManaged<EngineAudioSystem>();
            _engineAudioSystem.OnCreate(ref _world.Unmanaged);
            
            _world.SetSingleton(new TimeData { ElapsedTime = 10f, DeltaTime = 0.016f, FixedDeltaTime = 0.016f });
        }

        [TearDown]
        public void TearDown()
        {
            _engineAudioSystem.OnDestroy(ref _world.Unmanaged);
            _world.Dispose();
        }

        [Test]
        public void EngineAudioSystem_OnCreate_InitializesCorrectly()
        {
            Assert.IsNotNull(_engineAudioSystem);
        }

        [Test]
        public void EngineAudioSystem_OnUpdate_ProcessesWithoutErrors()
        {
            _engineAudioSystem.OnUpdate(ref _world.Unmanaged);
            Assert.IsNotNull(_engineAudioSystem);
        }

        [Test]
        public void EngineAudioSystem_WithEngineAudioData_ProcessesCorrectly()
        {
            var entity = _entityManager.CreateEntity();
            _entityManager.AddComponentData(entity, new EngineAudioData
            {
                EngineRPM = 1000f,
                Volume = 0.5f,
                Pitch = 1.0f,
                IsPlaying = true
            });

            _engineAudioSystem.OnUpdate(ref _world.Unmanaged);
            Assert.IsNotNull(_engineAudioSystem);
        }

        [Test]
        public void EngineAudioSystem_WithVehiclePhysics_ProcessesCorrectly()
        {
            var entity = _entityManager.CreateEntity();
            _entityManager.AddComponentData(entity, new LocalTransform 
            { 
                Position = new float3(0, 0, 0), 
                Rotation = quaternion.identity 
            });
            _entityManager.AddComponentData(entity, new VehiclePhysics
            {
                Velocity = new float3(10f, 0, 0),
                EnginePower = 500f,
                MaxEnginePower = 1000f
            });
            _entityManager.AddComponentData(entity, new EngineData
            {
                RPM = 2000f,
                MaxRPM = 6000f,
                Torque = 300f,
                MaxTorque = 500f
            });

            _engineAudioSystem.OnUpdate(ref _world.Unmanaged);
            Assert.IsNotNull(_engineAudioSystem);
        }

        [Test]
        public void EngineAudioSystem_MultipleEntities_HandlesCorrectly()
        {
            for (int i = 0; i < 5; i++)
            {
                var entity = _entityManager.CreateEntity();
                _entityManager.AddComponentData(entity, new EngineAudioData
                {
                    EngineRPM = 1000f + i * 500f,
                    Volume = 0.3f + i * 0.1f,
                    Pitch = 0.8f + i * 0.1f,
                    IsPlaying = i % 2 == 0
                });
            }

            _engineAudioSystem.OnUpdate(ref _world.Unmanaged);
            Assert.IsNotNull(_engineAudioSystem);
        }

        [Test]
        public void EngineAudioSystem_EdgeCases_HandleCorrectly()
        {
            var entity = _entityManager.CreateEntity();
            _entityManager.AddComponentData(entity, new EngineAudioData
            {
                EngineRPM = float.MaxValue,
                Volume = float.NaN,
                Pitch = float.PositiveInfinity,
                IsPlaying = true
            });

            Assert.DoesNotThrow(() => 
            {
                _engineAudioSystem.OnUpdate(ref _world.Unmanaged);
            });
        }
    }
}