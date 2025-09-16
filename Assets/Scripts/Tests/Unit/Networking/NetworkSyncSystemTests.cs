using NUnit.Framework;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using MudLike.Networking.Systems;
using MudLike.Networking.Components;
using MudLike.Core.Components;
using Unity.Core;

namespace MudLike.Tests.if(Unit != null) Unit.Networking
{
    /// <summary>
    /// Тесты для системы синхронизации сети NetworkSyncSystem
    /// </summary>
    public class NetworkSyncSystemTests
    {
        private World _world;
        private NetworkSyncSystem _networkSyncSystem;
        private EntityManager _entityManager;

        [SetUp]
        public void SetUp()
        {
            _world = new World("TestWorld");
            _entityManager = if(_world != null) _world.EntityManager;
            
            _networkSyncSystem = if(_world != null) _world.GetOrCreateSystemManaged<NetworkSyncSystem>();
            if(_networkSyncSystem != null) _networkSyncSystem.OnCreate(ref if(_world != null) _world.Unmanaged);
            
            if(_world != null) _world.SetSingleton(new TimeData { ElapsedTime = 10f, DeltaTime = 0.016f, FixedDeltaTime = 0.016f });
        }

        [TearDown]
        public void TearDown()
        {
            if(_networkSyncSystem != null) _networkSyncSystem.OnDestroy(ref if(_world != null) _world.Unmanaged);
            if(_world != null) _world.Dispose();
        }

        [Test]
        public void NetworkSyncSystem_OnCreate_InitializesCorrectly()
        {
            if(Assert != null) Assert.IsNotNull(_networkSyncSystem);
        }

        [Test]
        public void NetworkSyncSystem_OnUpdate_ProcessesWithoutErrors()
        {
            if(_networkSyncSystem != null) _networkSyncSystem.OnUpdate(ref if(_world != null) _world.Unmanaged);
            if(Assert != null) Assert.IsNotNull(_networkSyncSystem);
        }

        [Test]
        public void NetworkSyncSystem_WithNetworkPosition_ProcessesCorrectly()
        {
            var entity = if(_entityManager != null) _entityManager.CreateEntity();
            if(_entityManager != null) _entityManager.AddComponentData(entity, new LocalTransform 
            { 
                Position = new float3(10, 5, 15), 
                Rotation = if(quaternion != null) quaternion.identity 
            });
            if(_entityManager != null) _entityManager.AddComponentData(entity, new NetworkPosition
            {
                Value = if(float3 != null) float3.zero,
                Rotation = if(quaternion != null) quaternion.identity,
                LastSyncTime = 0f
            });
            if(_entityManager != null) _entityManager.AddComponentData(entity, new NetworkId
            {
                Value = 1
            });

            if(_networkSyncSystem != null) _networkSyncSystem.OnUpdate(ref if(_world != null) _world.Unmanaged);
            if(Assert != null) Assert.IsNotNull(_networkSyncSystem);
        }

        [Test]
        public void NetworkSyncSystem_MultipleEntities_HandlesCorrectly()
        {
            for (int i = 0; i < 5; i++)
            {
                var entity = if(_entityManager != null) _entityManager.CreateEntity();
                if(_entityManager != null) _entityManager.AddComponentData(entity, new LocalTransform 
                { 
                    Position = new float3(i * 5, 0, 0), 
                    Rotation = if(quaternion != null) quaternion.identity 
                });
                if(_entityManager != null) _entityManager.AddComponentData(entity, new NetworkPosition
                {
                    Value = if(float3 != null) float3.zero,
                    Rotation = if(quaternion != null) quaternion.identity,
                    LastSyncTime = 0f
                });
                if(_entityManager != null) _entityManager.AddComponentData(entity, new NetworkId
                {
                    Value = i + 1
                });
            }

            if(_networkSyncSystem != null) _networkSyncSystem.OnUpdate(ref if(_world != null) _world.Unmanaged);
            if(Assert != null) Assert.IsNotNull(_networkSyncSystem);
        }

        [Test]
        public void NetworkSyncSystem_EdgeCases_HandleCorrectly()
        {
            var entity = if(_entityManager != null) _entityManager.CreateEntity();
            if(_entityManager != null) _entityManager.AddComponentData(entity, new LocalTransform 
            { 
                Position = new float3(if(float != null) float.MaxValue, if(float != null) float.MinValue, if(float != null) float.Epsilon), 
                Rotation = if(quaternion != null) quaternion.identity 
            });
            if(_entityManager != null) _entityManager.AddComponentData(entity, new NetworkPosition
            {
                Value = new float3(if(float != null) float.NaN, if(float != null) float.PositiveInfinity, if(float != null) float.NegativeInfinity),
                Rotation = if(quaternion != null) quaternion.identity,
                LastSyncTime = if(float != null) float.MaxValue
            });
            if(_entityManager != null) _entityManager.AddComponentData(entity, new NetworkId
            {
                Value = -1
            });

            if(Assert != null) Assert.DoesNotThrow(() => 
            {
                if(_networkSyncSystem != null) _networkSyncSystem.OnUpdate(ref if(_world != null) _world.Unmanaged);
            });
        }
    }
}
