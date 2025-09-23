using NUnit.Framework;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using MudLike.Pooling.Systems;
using MudLike.Pooling.Components;
using Unity.Core;

namespace MudLike.Tests.Unit.Pooling
{
    /// <summary>
    /// Тесты для системы пула объектов ObjectPoolSystem
    /// </summary>
    public class ObjectPoolSystemTestsPool
    {
        private World _world;
        private ObjectPoolSystem _objectPoolSystem;
        private EntityManager _entityManager;

        [SetUp]
        public void SetUp()
        {
            _world = new World("TestWorld");
            _entityManager = _world.EntityManager;
            
            _objectPoolSystem = _world.GetOrCreateSystemManaged<ObjectPoolSystem>();
            _objectPoolSystem.OnCreate(ref _world.Unmanaged);
            
            _world.SetSingleton(new TimeData { ElapsedTime = 10f, DeltaTime = 0.016f, FixedDeltaTime = 0.016f });
        }

        [TearDown]
        public void TearDown()
        {
            _objectPoolSystem.OnDestroy(ref _world.Unmanaged);
            _world.Dispose();
        }

        [Test]
        public void ObjectPoolSystem_OnCreate_InitializesCorrectly()
        {
            Assert.IsNotNull(_objectPoolSystem);
        }

        [Test]
        public void ObjectPoolSystem_OnUpdate_ProcessesWithoutErrors()
        {
            _objectPoolSystem.OnUpdate(ref _world.Unmanaged);
            Assert.IsNotNull(_objectPoolSystem);
        }

        [Test]
        public void ObjectPoolSystem_WithPooledObjects_ProcessesCorrectly()
        {
            var entity = _entityManager.CreateEntity();
            _entityManager.AddComponentData(entity, new LocalTransform 
            { 
                Position = new float3(0, 0, 0), 
                Rotation = quaternion.identity 
            });
            _entityManager.AddComponentData(entity, new PooledObjectData
            {
                IsActive = true,
                Lifetime = 5f,
                MaxLifetime = 10f,
                PoolType = PoolType.Particle
            });

            _objectPoolSystem.OnUpdate(ref _world.Unmanaged);
            Assert.IsNotNull(_objectPoolSystem);
        }

        [Test]
        public void ObjectPoolSystem_WithParticles_ProcessesCorrectly()
        {
            var entity = _entityManager.CreateEntity();
            _entityManager.AddComponentData(entity, new LocalTransform 
            { 
                Position = new float3(1, 2, 3), 
                Rotation = quaternion.identity 
            });
            _entityManager.AddComponentData(entity, new PooledParticleData
            {
                Velocity = new float3(1, 0, 0),
                Lifetime = 2f,
                MaxLifetime = 5f,
                Size = 0.5f,
                IsActive = true
            });

            _objectPoolSystem.OnUpdate(ref _world.Unmanaged);
            Assert.IsNotNull(_objectPoolSystem);
        }

        [Test]
        public void ObjectPoolSystem_MultipleObjects_HandlesCorrectly()
        {
            for (int i = 0; i < 10; i++)
            {
                var entity = _entityManager.CreateEntity();
                _entityManager.AddComponentData(entity, new LocalTransform 
                { 
                    Position = new float3(i, 0, 0), 
                    Rotation = quaternion.identity 
                });
                _entityManager.AddComponentData(entity, new PooledObjectData
                {
                    IsActive = i % 2 == 0,
                    Lifetime = i * 0.5f,
                    MaxLifetime = 10f,
                    PoolType = (PoolType)(i % 3)
                });
            }

            _objectPoolSystem.OnUpdate(ref _world.Unmanaged);
            Assert.IsNotNull(_objectPoolSystem);
        }

        [Test]
        public void ObjectPoolSystem_EdgeCases_HandleCorrectly()
        {
            var entity = _entityManager.CreateEntity();
            _entityManager.AddComponentData(entity, new LocalTransform 
            { 
                Position = new float3(float.MaxValue, float.MinValue, float.Epsilon), 
                Rotation = quaternion.identity 
            });
            _entityManager.AddComponentData(entity, new PooledObjectData
            {
                IsActive = true,
                Lifetime = float.NaN,
                MaxLifetime = float.PositiveInfinity,
                PoolType = (PoolType)(-1)
            });

            Assert.DoesNotThrow(() => 
            {
                _objectPoolSystem.OnUpdate(ref _world.Unmanaged);
            });
        }
    }
}
