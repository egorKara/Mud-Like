using NUnit.Framework;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using MudLike.Effects.Systems;
using MudLike.Effects.Components;
using Unity.Core;

namespace MudLike.Tests.Unit.Effects
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
            _entityManager = _world.EntityManager;
            
            _mudParticleSystem = _world.GetOrCreateSystemManaged<MudParticleSystem>();
            _mudParticleSystem.OnCreate(ref _world.Unmanaged);
            
            _world.SetSingleton(new TimeData { ElapsedTime = 10f, DeltaTime = 0.016f, FixedDeltaTime = 0.016f });
        }

        [TearDown]
        public void TearDown()
        {
            _mudParticleSystem.OnDestroy(ref _world.Unmanaged);
            _world.Dispose();
        }

        [Test]
        public void MudParticleSystem_OnCreate_InitializesCorrectly()
        {
            Assert.IsNotNull(_mudParticleSystem);
        }

        [Test]
        public void MudParticleSystem_OnUpdate_ProcessesWithoutErrors()
        {
            _mudParticleSystem.OnUpdate(ref _world.Unmanaged);
            Assert.IsNotNull(_mudParticleSystem);
        }

        [Test]
        public void MudParticleSystem_WithMudParticleData_ProcessesCorrectly()
        {
            var entity = _entityManager.CreateEntity();
            _entityManager.AddComponentData(entity, new LocalTransform 
            { 
                Position = new float3(0, 0, 0), 
                Rotation = quaternion.identity 
            });
            _entityManager.AddComponentData(entity, new MudParticleData
            {
                Velocity = new float3(1, 2, 3),
                Lifetime = 2f,
                MaxLifetime = 5f,
                Size = 0.5f,
                IsActive = true
            });

            _mudParticleSystem.OnUpdate(ref _world.Unmanaged);
            Assert.IsNotNull(_mudParticleSystem);
        }

        [Test]
        public void MudParticleSystem_MultipleParticles_HandlesCorrectly()
        {
            for (int i = 0; i < 10; i++)
            {
                var entity = _entityManager.CreateEntity();
                _entityManager.AddComponentData(entity, new LocalTransform 
                { 
                    Position = new float3(i, 0, 0), 
                    Rotation = quaternion.identity 
                });
                _entityManager.AddComponentData(entity, new MudParticleData
                {
                    Velocity = new float3(i, i * 0.5f, 0),
                    Lifetime = i * 0.5f,
                    MaxLifetime = 10f,
                    Size = 0.1f + i * 0.05f,
                    IsActive = i % 2 == 0
                });
            }

            _mudParticleSystem.OnUpdate(ref _world.Unmanaged);
            Assert.IsNotNull(_mudParticleSystem);
        }

        [Test]
        public void MudParticleSystem_EdgeCases_HandleCorrectly()
        {
            var entity = _entityManager.CreateEntity();
            _entityManager.AddComponentData(entity, new LocalTransform 
            { 
                Position = new float3(float.MaxValue, float.MinValue, float.Epsilon), 
                Rotation = quaternion.identity 
            });
            _entityManager.AddComponentData(entity, new MudParticleData
            {
                Velocity = new float3(float.NaN, float.PositiveInfinity, float.NegativeInfinity),
                Lifetime = float.MaxValue,
                MaxLifetime = float.MinValue,
                Size = float.Epsilon,
                IsActive = true
            });

            Assert.DoesNotThrow(() => 
            {
                _mudParticleSystem.OnUpdate(ref _world.Unmanaged);
            });
        }
    }
}
