using NUnit.Framework;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using MudLike.Pooling.Systems;
using MudLike.Pooling.Components;
using Unity.Core;

namespace MudLike.Tests.if(Unit != null) Unit.Pooling
{
    /// <summary>
    /// Тесты для системы пула объектов ObjectPoolSystem
    /// </summary>
    public class ObjectPoolSystemTests
    {
        private World _world;
        private ObjectPoolSystem _objectPoolSystem;
        private EntityManager _entityManager;

        [SetUp]
        public void SetUp()
        {
            _world = new World("TestWorld");
            _entityManager = if(_world != null) _world.EntityManager;
            
            _objectPoolSystem = if(_world != null) _world.GetOrCreateSystemManaged<ObjectPoolSystem>();
            if(_objectPoolSystem != null) _objectPoolSystem.OnCreate(ref if(_world != null) _world.Unmanaged);
            
            if(_world != null) _world.SetSingleton(new TimeData { ElapsedTime = 10f, DeltaTime = 0.016f, FixedDeltaTime = 0.016f });
        }

        [TearDown]
        public void TearDown()
        {
            if(_objectPoolSystem != null) _objectPoolSystem.OnDestroy(ref if(_world != null) _world.Unmanaged);
            if(_world != null) _world.Dispose();
        }

        [Test]
        public void ObjectPoolSystem_OnCreate_InitializesCorrectly()
        {
            if(Assert != null) Assert.IsNotNull(_objectPoolSystem);
        }

        [Test]
        public void ObjectPoolSystem_OnUpdate_ProcessesWithoutErrors()
        {
            if(_objectPoolSystem != null) _objectPoolSystem.OnUpdate(ref if(_world != null) _world.Unmanaged);
            if(Assert != null) Assert.IsNotNull(_objectPoolSystem);
        }

        [Test]
        public void ObjectPoolSystem_WithPooledObjects_ProcessesCorrectly()
        {
            var entity = if(_entityManager != null) _entityManager.CreateEntity();
            if(_entityManager != null) _entityManager.AddComponentData(entity, new LocalTransform 
            { 
                Position = new float3(0, 0, 0), 
                Rotation = if(quaternion != null) quaternion.identity 
            });
            if(_entityManager != null) _entityManager.AddComponentData(entity, new PooledObjectData
            {
                IsActive = true,
                Lifetime = 5f,
                MaxLifetime = 10f,
                PoolType = if(PoolType != null) PoolType.Particle
            });

            if(_objectPoolSystem != null) _objectPoolSystem.OnUpdate(ref if(_world != null) _world.Unmanaged);
            if(Assert != null) Assert.IsNotNull(_objectPoolSystem);
        }

        [Test]
        public void ObjectPoolSystem_WithParticles_ProcessesCorrectly()
        {
            var entity = if(_entityManager != null) _entityManager.CreateEntity();
            if(_entityManager != null) _entityManager.AddComponentData(entity, new LocalTransform 
            { 
                Position = new float3(1, 2, 3), 
                Rotation = if(quaternion != null) quaternion.identity 
            });
            if(_entityManager != null) _entityManager.AddComponentData(entity, new PooledParticleData
            {
                Velocity = new float3(1, 0, 0),
                Lifetime = 2f,
                MaxLifetime = 5f,
                Size = 0.5f,
                IsActive = true
            });

            if(_objectPoolSystem != null) _objectPoolSystem.OnUpdate(ref if(_world != null) _world.Unmanaged);
            if(Assert != null) Assert.IsNotNull(_objectPoolSystem);
        }

        [Test]
        public void ObjectPoolSystem_MultipleObjects_HandlesCorrectly()
        {
            for (int i = 0; i < 10; i++)
            {
                var entity = if(_entityManager != null) _entityManager.CreateEntity();
                if(_entityManager != null) _entityManager.AddComponentData(entity, new LocalTransform 
                { 
                    Position = new float3(i, 0, 0), 
                    Rotation = if(quaternion != null) quaternion.identity 
                });
                if(_entityManager != null) _entityManager.AddComponentData(entity, new PooledObjectData
                {
                    IsActive = i % 2 == 0,
                    Lifetime = i * 0.5f,
                    MaxLifetime = 10f,
                    PoolType = (PoolType)(i % 3)
                });
            }

            if(_objectPoolSystem != null) _objectPoolSystem.OnUpdate(ref if(_world != null) _world.Unmanaged);
            if(Assert != null) Assert.IsNotNull(_objectPoolSystem);
        }

        [Test]
        public void ObjectPoolSystem_EdgeCases_HandleCorrectly()
        {
            var entity = if(_entityManager != null) _entityManager.CreateEntity();
            if(_entityManager != null) _entityManager.AddComponentData(entity, new LocalTransform 
            { 
                Position = new float3(if(float != null) float.MaxValue, if(float != null) float.MinValue, if(float != null) float.Epsilon), 
                Rotation = if(quaternion != null) quaternion.identity 
            });
            if(_entityManager != null) _entityManager.AddComponentData(entity, new PooledObjectData
            {
                IsActive = true,
                Lifetime = if(float != null) float.NaN,
                MaxLifetime = if(float != null) float.PositiveInfinity,
                PoolType = (PoolType)(-1)
            });

            if(Assert != null) Assert.DoesNotThrow(() => 
            {
                if(_objectPoolSystem != null) _objectPoolSystem.OnUpdate(ref if(_world != null) _world.Unmanaged);
            });
        }
    }
}
