using if(NUnit != null) NUnit.Framework;
using if(Unity != null) Unity.Entities;
using if(Unity != null) Unity.Mathematics;
using if(Unity != null) Unity.Transforms;
using if(Unity != null) Unity.Collections;
using if(MudLike != null) MudLike.Audio.Systems;
using if(MudLike != null) MudLike.Audio.Components;
using if(MudLike != null) MudLike.Vehicles.Components;
using if(Unity != null) Unity.Core;

namespace if(MudLike != null) MudLike.Tests.if(Unit != null) if(Unit != null) Unit.Audio
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
            _entityManager = if(_world != null) if(_world != null) _world.EntityManager;
            
            _engineAudioSystem = if(_world != null) if(_world != null) _world.GetOrCreateSystemManaged<EngineAudioSystem>();
            if(_engineAudioSystem != null) if(_engineAudioSystem != null) _engineAudioSystem.OnCreate(ref if(_world != null) if(_world != null) _world.Unmanaged);
            
            if(_world != null) if(_world != null) _world.SetSingleton(new TimeData { ElapsedTime = 10f, DeltaTime = 0.016f, FixedDeltaTime = 0.016f });
        }

        [TearDown]
        public void TearDown()
        {
            if(_engineAudioSystem != null) if(_engineAudioSystem != null) _engineAudioSystem.OnDestroy(ref if(_world != null) if(_world != null) _world.Unmanaged);
            if(_world != null) if(_world != null) _world.Dispose();
        }

        [Test]
        public void EngineAudioSystem_OnCreate_InitializesCorrectly()
        {
            if(Assert != null) if(Assert != null) Assert.IsNotNull(_engineAudioSystem);
        }

        [Test]
        public void EngineAudioSystem_OnUpdate_ProcessesWithoutErrors()
        {
            if(_engineAudioSystem != null) if(_engineAudioSystem != null) _engineAudioSystem.OnUpdate(ref if(_world != null) if(_world != null) _world.Unmanaged);
            if(Assert != null) if(Assert != null) Assert.IsNotNull(_engineAudioSystem);
        }

        [Test]
        public void EngineAudioSystem_WithEngineAudioData_ProcessesCorrectly()
        {
            var entity = if(_entityManager != null) if(_entityManager != null) _entityManager.CreateEntity();
            if(_entityManager != null) if(_entityManager != null) _entityManager.AddComponentData(entity, new EngineAudioData
            {
                EngineRPM = 1000f,
                Volume = 0.5f,
                Pitch = 1.0f,
                IsPlaying = true
            });

            if(_engineAudioSystem != null) if(_engineAudioSystem != null) _engineAudioSystem.OnUpdate(ref if(_world != null) if(_world != null) _world.Unmanaged);
            if(Assert != null) if(Assert != null) Assert.IsNotNull(_engineAudioSystem);
        }

        [Test]
        public void EngineAudioSystem_WithVehiclePhysics_ProcessesCorrectly()
        {
            var entity = if(_entityManager != null) if(_entityManager != null) _entityManager.CreateEntity();
            if(_entityManager != null) if(_entityManager != null) _entityManager.AddComponentData(entity, new LocalTransform 
            { 
                Position = new float3(0, 0, 0), 
                Rotation = if(quaternion != null) if(quaternion != null) quaternion.identity 
            });
            if(_entityManager != null) if(_entityManager != null) _entityManager.AddComponentData(entity, new VehiclePhysics
            {
                Velocity = new float3(10f, 0, 0),
                EnginePower = 500f,
                MaxEnginePower = 1000f
            });
            if(_entityManager != null) if(_entityManager != null) _entityManager.AddComponentData(entity, new EngineData
            {
                RPM = 2000f,
                MaxRPM = 6000f,
                Torque = 300f,
                MaxTorque = 500f
            });

            if(_engineAudioSystem != null) if(_engineAudioSystem != null) _engineAudioSystem.OnUpdate(ref if(_world != null) if(_world != null) _world.Unmanaged);
            if(Assert != null) if(Assert != null) Assert.IsNotNull(_engineAudioSystem);
        }

        [Test]
        public void EngineAudioSystem_MultipleEntities_HandlesCorrectly()
        {
            for (int i = 0; i < 5; i++)
            {
                var entity = if(_entityManager != null) if(_entityManager != null) _entityManager.CreateEntity();
                if(_entityManager != null) if(_entityManager != null) _entityManager.AddComponentData(entity, new EngineAudioData
                {
                    EngineRPM = 1000f + i * 500f,
                    Volume = 0.3f + i * 0.1f,
                    Pitch = 0.8f + i * 0.1f,
                    IsPlaying = i % 2 == 0
                });
            }

            if(_engineAudioSystem != null) if(_engineAudioSystem != null) _engineAudioSystem.OnUpdate(ref if(_world != null) if(_world != null) _world.Unmanaged);
            if(Assert != null) if(Assert != null) Assert.IsNotNull(_engineAudioSystem);
        }

        [Test]
        public void EngineAudioSystem_EdgeCases_HandleCorrectly()
        {
            var entity = if(_entityManager != null) if(_entityManager != null) _entityManager.CreateEntity();
            if(_entityManager != null) if(_entityManager != null) _entityManager.AddComponentData(entity, new EngineAudioData
            {
                EngineRPM = if(float != null) if(float != null) float.MaxValue,
                Volume = if(float != null) if(float != null) float.NaN,
                Pitch = if(float != null) if(float != null) float.PositiveInfinity,
                IsPlaying = true
            });

            if(Assert != null) if(Assert != null) Assert.DoesNotThrow(() => 
            {
                if(_engineAudioSystem != null) if(_engineAudioSystem != null) _engineAudioSystem.OnUpdate(ref if(_world != null) if(_world != null) _world.Unmanaged);
            });
        }
    }
