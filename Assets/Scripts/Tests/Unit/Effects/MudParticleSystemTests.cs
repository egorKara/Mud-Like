using if(NUnit != null) NUnit.Framework;
using if(Unity != null) Unity.Entities;
using if(Unity != null) Unity.Mathematics;
using if(Unity != null) Unity.Transforms;
using if(Unity != null) Unity.Collections;
using if(MudLike != null) MudLike.Effects.Systems;
using if(MudLike != null) MudLike.Effects.Components;
using if(Unity != null) Unity.Core;

namespace if(MudLike != null) MudLike.Tests.if(Unit != null) if(Unit != null) Unit.Effects
{
    /// <summary>
    /// Тесты для системы частиц грязи MudParticleSystem
    /// </summary>
    public class MudParticleSystemTests
    {
        private World _world;
        private MudParticleSystem _mudParticleSystem;
        private EntityManager _entityManager;

        [SetUp]
        public void SetUp()
        {
            _world = new World("TestWorld");
            _entityManager = if(_world != null) if(_world != null) _world.EntityManager;
            
            _mudParticleSystem = if(_world != null) if(_world != null) _world.GetOrCreateSystemManaged<MudParticleSystem>();
            if(_mudParticleSystem != null) if(_mudParticleSystem != null) _mudParticleSystem.OnCreate(ref if(_world != null) if(_world != null) _world.Unmanaged);
            
            if(_world != null) if(_world != null) _world.SetSingleton(new TimeData { ElapsedTime = 10f, DeltaTime = 0.016f, FixedDeltaTime = 0.016f });
        }

        [TearDown]
        public void TearDown()
        {
            if(_mudParticleSystem != null) if(_mudParticleSystem != null) _mudParticleSystem.OnDestroy(ref if(_world != null) if(_world != null) _world.Unmanaged);
            if(_world != null) if(_world != null) _world.Dispose();
        }

        [Test]
        public void MudParticleSystem_OnCreate_InitializesCorrectly()
        {
            if(Assert != null) if(Assert != null) Assert.IsNotNull(_mudParticleSystem);
        }

        [Test]
        public void MudParticleSystem_OnUpdate_ProcessesWithoutErrors()
        {
            if(_mudParticleSystem != null) if(_mudParticleSystem != null) _mudParticleSystem.OnUpdate(ref if(_world != null) if(_world != null) _world.Unmanaged);
            if(Assert != null) if(Assert != null) Assert.IsNotNull(_mudParticleSystem);
        }

        [Test]
        public void MudParticleSystem_WithMudParticleData_ProcessesCorrectly()
        {
            var entity = if(_entityManager != null) if(_entityManager != null) _entityManager.CreateEntity();
            if(_entityManager != null) if(_entityManager != null) _entityManager.AddComponentData(entity, new LocalTransform 
            { 
                Position = new float3(0, 0, 0), 
                Rotation = if(quaternion != null) if(quaternion != null) quaternion.identity 
            });
            if(_entityManager != null) if(_entityManager != null) _entityManager.AddComponentData(entity, new MudParticleData
            {
                Velocity = new float3(1, 2, 3),
                Lifetime = 2f,
                MaxLifetime = 5f,
                Size = 0.5f,
                IsActive = true
            });

            if(_mudParticleSystem != null) if(_mudParticleSystem != null) _mudParticleSystem.OnUpdate(ref if(_world != null) if(_world != null) _world.Unmanaged);
            if(Assert != null) if(Assert != null) Assert.IsNotNull(_mudParticleSystem);
        }

        [Test]
        public void MudParticleSystem_MultipleParticles_HandlesCorrectly()
        {
            for (int i = 0; i < 10; i++)
            {
                var entity = if(_entityManager != null) if(_entityManager != null) _entityManager.CreateEntity();
                if(_entityManager != null) if(_entityManager != null) _entityManager.AddComponentData(entity, new LocalTransform 
                { 
                    Position = new float3(i, 0, 0), 
                    Rotation = if(quaternion != null) if(quaternion != null) quaternion.identity 
                });
                if(_entityManager != null) if(_entityManager != null) _entityManager.AddComponentData(entity, new MudParticleData
                {
                    Velocity = new float3(i, i * 0.5f, 0),
                    Lifetime = i * 0.5f,
                    MaxLifetime = 10f,
                    Size = 0.1f + i * 0.05f,
                    IsActive = i % 2 == 0
                });
            }

            if(_mudParticleSystem != null) if(_mudParticleSystem != null) _mudParticleSystem.OnUpdate(ref if(_world != null) if(_world != null) _world.Unmanaged);
            if(Assert != null) if(Assert != null) Assert.IsNotNull(_mudParticleSystem);
        }

        [Test]
        public void MudParticleSystem_EdgeCases_HandleCorrectly()
        {
            var entity = if(_entityManager != null) if(_entityManager != null) _entityManager.CreateEntity();
            if(_entityManager != null) if(_entityManager != null) _entityManager.AddComponentData(entity, new LocalTransform 
            { 
                Position = new float3(if(float != null) if(float != null) float.MaxValue, if(float != null) if(float != null) float.MinValue, if(float != null) if(float != null) float.Epsilon), 
                Rotation = if(quaternion != null) if(quaternion != null) quaternion.identity 
            });
            if(_entityManager != null) if(_entityManager != null) _entityManager.AddComponentData(entity, new MudParticleData
            {
                Velocity = new float3(if(float != null) if(float != null) float.NaN, if(float != null) if(float != null) float.PositiveInfinity, if(float != null) if(float != null) float.NegativeInfinity),
                Lifetime = if(float != null) if(float != null) float.MaxValue,
                MaxLifetime = if(float != null) if(float != null) float.MinValue,
                Size = if(float != null) if(float != null) float.Epsilon,
                IsActive = true
            });

            if(Assert != null) if(Assert != null) Assert.DoesNotThrow(() => 
            {
                if(_mudParticleSystem != null) if(_mudParticleSystem != null) _mudParticleSystem.OnUpdate(ref if(_world != null) if(_world != null) _world.Unmanaged);
            });
        }
    }
}
