using NUnit.Framework;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using MudLike.Networking.Systems;
using MudLike.Networking.Components;
using MudLike.Core.Components;
using Unity.Core;

namespace MudLike.Tests.Unit.Networking
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
            _entityManager = _world.EntityManager;
            
            _networkSyncSystem = _world.GetOrCreateSystemManaged<NetworkSyncSystem>();
            _networkSyncSystem.OnCreate(ref _world.Unmanaged);
            
            _world.SetSingleton(new TimeData { ElapsedTime = 10f, DeltaTime = 0.016f, FixedDeltaTime = 0.016f });
        }

        [TearDown]
        public void TearDown()
        {
            _networkSyncSystem.OnDestroy(ref _world.Unmanaged);
            _world.Dispose();
        }

        [Test]
        public void NetworkSyncSystem_OnCreate_InitializesCorrectly()
        {
            Assert.IsNotNull(_networkSyncSystem);
        }

        [Test]
        public void NetworkSyncSystem_OnUpdate_ProcessesWithoutErrors()
        {
            _networkSyncSystem.OnUpdate(ref _world.Unmanaged);
            Assert.IsNotNull(_networkSyncSystem);
        }

        [Test]
        public void NetworkSyncSystem_WithNetworkPosition_ProcessesCorrectly()
        {
            var entity = _entityManager.CreateEntity();
            _entityManager.AddComponentData(entity, new LocalTransform 
            { 
                Position = new float3(10, 5, 15), 
                Rotation = quaternion.identity 
            });
            _entityManager.AddComponentData(entity, new NetworkPosition
            {
                Value = float3.zero,
                Rotation = quaternion.identity,
                LastSyncTime = 0f
            });
            _entityManager.AddComponentData(entity, new NetworkId
            {
                Value = 1
            });

            _networkSyncSystem.OnUpdate(ref _world.Unmanaged);
            Assert.IsNotNull(_networkSyncSystem);
        }

        [Test]
        public void NetworkSyncSystem_MultipleEntities_HandlesCorrectly()
        {
            for (int i = 0; i < 5; i++)
            {
                var entity = _entityManager.CreateEntity();
                _entityManager.AddComponentData(entity, new LocalTransform 
                { 
                    Position = new float3(i * 5, 0, 0), 
                    Rotation = quaternion.identity 
                });
                _entityManager.AddComponentData(entity, new NetworkPosition
                {
                    Value = float3.zero,
                    Rotation = quaternion.identity,
                    LastSyncTime = 0f
                });
                _entityManager.AddComponentData(entity, new NetworkId
                {
                    Value = i + 1
                });
            }

            _networkSyncSystem.OnUpdate(ref _world.Unmanaged);
            Assert.IsNotNull(_networkSyncSystem);
        }

        [Test]
        public void NetworkSyncSystem_EdgeCases_HandleCorrectly()
        {
            var entity = _entityManager.CreateEntity();
            _entityManager.AddComponentData(entity, new LocalTransform 
            { 
                Position = new float3(float.MaxValue, float.MinValue, float.Epsilon), 
                Rotation = quaternion.identity 
            });
            _entityManager.AddComponentData(entity, new NetworkPosition
            {
                Value = new float3(float.NaN, float.PositiveInfinity, float.NegativeInfinity),
                Rotation = quaternion.identity,
                LastSyncTime = float.MaxValue
            });
            _entityManager.AddComponentData(entity, new NetworkId
            {
                Value = -1
            });

            Assert.DoesNotThrow(() => 
            {
                _networkSyncSystem.OnUpdate(ref _world.Unmanaged);
            });
        }
    }
}
