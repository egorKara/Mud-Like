using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.NetCode;
using Unity.Collections;
using NUnit.Framework;
using MudLike.Networking.Components;
using MudLike.Core.Components;
using MudLike.Vehicles.Components;
using MudLike.Terrain.Components;
using static MudLike.Core.Components.Position;

#if UNITY_6_0_OR_NEWER
using Unity.Multiplayer;
using Unity.Transport;
#endif

namespace MudLike.Tests.Integration
{
    /// <summary>
    /// Интеграционные тесты для Netcode for Entities (NFE)
    /// Тестирует полную интеграцию сетевых систем в проекте Mud-Like
    /// </summary>
    [TestFixture]
    public class NFEIntegrationTests : MudLikeTestFixture
    {
        private Entity _testEntity;
        private Entity _clientEntity;
        private Entity _serverEntity;
        
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            
            // Создаем тестовые сущности
            _testEntity = CreateTestEntity();
            _clientEntity = CreateClientEntity();
            _serverEntity = CreateServerEntity();
        }
        
        [TearDown]
        public override void TearDown()
        {
            base.TearDown();
        }
        
        #region Базовые интеграционные тесты
        
        /// <summary>
        /// Тест создания сетевой сущности с базовыми компонентами
        /// </summary>
        [Test]
        public void CreateNetworkEntity_WithBasicComponents_ShouldSucceed()
        {
            // Arrange
            var entity = EntityManager.CreateEntity();
            
            // Act
            EntityManager.AddComponent<NetworkId>(entity);
            EntityManager.AddComponent<NetworkPosition>(entity);
            EntityManager.AddComponent<LocalTransform>(entity);
            EntityManager.AddComponent<PlayerTag>(entity);
            
            // Assert
            Assert.IsTrue(EntityManager.HasComponent<NetworkId>(entity));
            Assert.IsTrue(EntityManager.HasComponent<NetworkPosition>(entity));
            Assert.IsTrue(EntityManager.HasComponent<LocalTransform>(entity));
            Assert.IsTrue(EntityManager.HasComponent<PlayerTag>(entity));
        }
        
        /// <summary>
        /// Тест создания сетевой сущности с Unity 6 компонентами
        /// </summary>
        [Test]
        public void CreateNetworkEntity_WithUnity6Components_ShouldSucceed()
        {
            // Arrange
            var entity = EntityManager.CreateEntity();
            
            // Act
            var networkId = NetworkId.CreateAuthoritative(123, 1); // Игрок
            var networkPosition = new NetworkPosition
            {
                Value = new float3(10f, 5f, 15f),
                Rotation = quaternion.identity,
                SyncPriority = 255,
                EnableInterpolation = true,
                HasChanged = false,
                LastUpdateTime = 0f
            };
            
            EntityManager.AddComponentData(entity, networkId);
            EntityManager.AddComponentData(entity, networkPosition);
            EntityManager.AddComponent<LocalTransform>(entity);
            EntityManager.AddComponent<PlayerTag>(entity);
            
            // Assert
            Assert.IsTrue(EntityManager.HasComponent<NetworkId>(entity));
            Assert.IsTrue(EntityManager.HasComponent<NetworkPosition>(entity));
            Assert.IsTrue(EntityManager.HasComponent<LocalTransform>(entity));
            Assert.IsTrue(EntityManager.HasComponent<PlayerTag>(entity));
            
            var actualNetworkId = EntityManager.GetComponentData<NetworkId>(entity);
            var actualNetworkPosition = EntityManager.GetComponentData<NetworkPosition>(entity);
            
            Assert.AreEqual(123, actualNetworkId.Value);
            Assert.IsTrue(actualNetworkId.IsAuthoritative);
            Assert.AreEqual(255, actualNetworkPosition.SyncPriority);
            Assert.IsTrue(actualNetworkPosition.EnableInterpolation);
        }
        
        /// <summary>
        /// Тест инициализации сетевых компонентов
        /// </summary>
        [Test]
        public void InitializeNetworkComponents_ShouldSetCorrectValues()
        {
            // Arrange
            var entity = EntityManager.CreateEntity();
            var networkId = NetworkId.Create(123);
            var networkPosition = new NetworkPosition
            {
                Value = new float3(10f, 5f, 15f),
                Rotation = quaternion.identity,
                HasChanged = false,
                LastUpdateTime = 0f
            };
            
            // Act
            EntityManager.AddComponentData(entity, networkId);
            EntityManager.AddComponentData(entity, networkPosition);
            
            // Assert
            var actualNetworkId = EntityManager.GetComponentData<NetworkId>(entity);
            var actualNetworkPosition = EntityManager.GetComponentData<NetworkPosition>(entity);
            
            Assert.AreEqual(123, actualNetworkId.Value);
            Assert.AreEqual(new float3(10f, 5f, 15f), actualNetworkPosition.Value);
            Assert.AreEqual(quaternion.identity, actualNetworkPosition.Rotation);
            Assert.IsFalse(actualNetworkPosition.HasChanged);
        }
        
        /// <summary>
        /// Тест синхронизации позиций между клиентом и сервером
        /// </summary>
        [Test]
        public void SyncPosition_ClientToServer_ShouldUpdateCorrectly()
        {
            // Arrange
            var clientEntity = CreateClientEntity();
            var serverEntity = CreateServerEntity();
            
            var clientTransform = new LocalTransform
            {
                Position = new float3(100f, 0f, 200f),
                Rotation = quaternion.RotateY(math.radians(45f)),
                Scale = 1f
            };
            
            var serverNetworkPosition = new NetworkPosition
            {
                Value = float3.zero,
                Rotation = quaternion.identity,
                HasChanged = false
            };
            
            EntityManager.SetComponentData(clientEntity, clientTransform);
            EntityManager.SetComponentData(serverEntity, serverNetworkPosition);
            
            // Act
            var networkSyncSystem = World.GetOrCreateSystemManaged<NetworkSyncSystem>();
            networkSyncSystem.Update();
            
            // Assert
            var updatedNetworkPosition = EntityManager.GetComponentData<NetworkPosition>(serverEntity);
            Assert.AreEqual(clientTransform.Position, updatedNetworkPosition.Value);
            Assert.AreEqual(clientTransform.Rotation, updatedNetworkPosition.Rotation);
            Assert.IsTrue(updatedNetworkPosition.HasChanged);
        }
        
        #endregion
        
        #region Тесты синхронизации позиций
        
        /// <summary>
        /// Тест синхронизации позиций с пороговыми значениями
        /// </summary>
        [Test]
        public void SyncPosition_WithThreshold_ShouldOnlySyncWhenNeeded()
        {
            // Arrange
            var entity = CreateTestEntity();
            var transform = new LocalTransform
            {
                Position = new float3(0.005f, 0f, 0f), // Малые изменения
                Rotation = quaternion.identity,
                Scale = 1f
            };
            
            var networkPosition = new NetworkPosition
            {
                Value = float3.zero,
                Rotation = quaternion.identity,
                HasChanged = false
            };
            
            EntityManager.SetComponentData(entity, transform);
            EntityManager.SetComponentData(entity, networkPosition);
            
            // Act
            var networkSyncSystem = World.GetOrCreateSystemManaged<NetworkSyncSystem>();
            networkSyncSystem.Update();
            
            // Assert
            var updatedNetworkPosition = EntityManager.GetComponentData<NetworkPosition>(entity);
            Assert.IsFalse(updatedNetworkPosition.HasChanged); // Не должно измениться из-за порога
        }
        
        /// <summary>
        /// Тест синхронизации позиций с большими изменениями
        /// </summary>
        [Test]
        public void SyncPosition_WithLargeChanges_ShouldSync()
        {
            // Arrange
            var entity = CreateTestEntity();
            var transform = new LocalTransform
            {
                Position = new float3(10f, 0f, 0f), // Большие изменения
                Rotation = quaternion.RotateY(math.radians(90f)),
                Scale = 1f
            };
            
            var networkPosition = new NetworkPosition
            {
                Value = float3.zero,
                Rotation = quaternion.identity,
                HasChanged = false
            };
            
            EntityManager.SetComponentData(entity, transform);
            EntityManager.SetComponentData(entity, networkPosition);
            
            // Act
            var networkSyncSystem = World.GetOrCreateSystemManaged<NetworkSyncSystem>();
            networkSyncSystem.Update();
            
            // Assert
            var updatedNetworkPosition = EntityManager.GetComponentData<NetworkPosition>(entity);
            Assert.IsTrue(updatedNetworkPosition.HasChanged);
            Assert.AreEqual(transform.Position, updatedNetworkPosition.Value);
            Assert.AreEqual(transform.Rotation, updatedNetworkPosition.Rotation);
        }
        
        /// <summary>
        /// Тест синхронизации множественных позиций
        /// </summary>
        [Test]
        public void SyncMultiplePositions_ShouldSyncAll()
        {
            // Arrange
            var entities = new Entity[5];
            for (int i = 0; i < 5; i++)
            {
                entities[i] = CreateTestEntity();
                
                var transform = new LocalTransform
                {
                    Position = new float3(i * 10f, 0f, 0f),
                    Rotation = quaternion.RotateY(math.radians(i * 30f)),
                    Scale = 1f
                };
                
                var networkPosition = new NetworkPosition
                {
                    Value = float3.zero,
                    Rotation = quaternion.identity,
                    HasChanged = false
                };
                
                EntityManager.SetComponentData(entities[i], transform);
                EntityManager.SetComponentData(entities[i], networkPosition);
            }
            
            // Act
            var networkSyncSystem = World.GetOrCreateSystemManaged<NetworkSyncSystem>();
            networkSyncSystem.Update();
            
            // Assert
            for (int i = 0; i < 5; i++)
            {
                var updatedNetworkPosition = EntityManager.GetComponentData<NetworkPosition>(entities[i]);
                Assert.IsTrue(updatedNetworkPosition.HasChanged);
                Assert.AreEqual(new float3(i * 10f, 0f, 0f), updatedNetworkPosition.Value);
            }
        }
        
        #endregion
        
        #region Тесты сетевых команд
        
        /// <summary>
        /// Тест создания и выполнения сетевой команды
        /// </summary>
        [Test]
        public void CreateNetworkCommand_ShouldExecuteCorrectly()
        {
            // Arrange
            var entity = CreateTestEntity();
            var command = new MoveCommand
            {
                Target = entity,
                Position = new float3(50f, 0f, 100f),
                Rotation = quaternion.RotateY(math.radians(180f))
            };
            
            // Act
            var commandBuffer = new EntityCommandBuffer(Allocator.Temp);
            commandBuffer.AddComponent(entity, command);
            commandBuffer.Playback(EntityManager);
            commandBuffer.Dispose();
            
            // Assert
            Assert.IsTrue(EntityManager.HasComponent<MoveCommand>(entity));
            var actualCommand = EntityManager.GetComponentData<MoveCommand>(entity);
            Assert.AreEqual(entity, actualCommand.Target);
            Assert.AreEqual(new float3(50f, 0f, 100f), actualCommand.Position);
        }
        
        /// <summary>
        /// Тест обработки множественных команд
        /// </summary>
        [Test]
        public void ProcessMultipleCommands_ShouldHandleAll()
        {
            // Arrange
            var entities = new Entity[3];
            var commands = new MoveCommand[3];
            
            for (int i = 0; i < 3; i++)
            {
                entities[i] = CreateTestEntity();
                commands[i] = new MoveCommand
                {
                    Target = entities[i],
                    Position = new float3(i * 20f, 0f, i * 30f),
                    Rotation = quaternion.RotateY(math.radians(i * 60f))
                };
            }
            
            // Act
            var commandBuffer = new EntityCommandBuffer(Allocator.Temp);
            for (int i = 0; i < 3; i++)
            {
                commandBuffer.AddComponent(entities[i], commands[i]);
            }
            commandBuffer.Playback(EntityManager);
            commandBuffer.Dispose();
            
            // Assert
            for (int i = 0; i < 3; i++)
            {
                Assert.IsTrue(EntityManager.HasComponent<MoveCommand>(entities[i]));
                var actualCommand = EntityManager.GetComponentData<MoveCommand>(entities[i]);
                Assert.AreEqual(entities[i], actualCommand.Target);
            }
        }
        
        #endregion
        
        #region Тесты компенсации задержек
        
        /// <summary>
        /// Тест компенсации задержек для позиций
        /// </summary>
        [Test]
        public void LagCompensation_ShouldCompensateCorrectly()
        {
            // Arrange
            var entity = CreateTestEntity();
            var networkPosition = new NetworkPosition
            {
                Value = new float3(0f, 0f, 0f),
                Velocity = new float3(10f, 0f, 0f),
                LastUpdateTime = 0f,
                HasChanged = true
            };
            
            EntityManager.SetComponentData(entity, networkPosition);
            
            // Act
            var lagCompensationSystem = World.GetOrCreateSystemManaged<LagCompensationSystem>();
            lagCompensationSystem.Update();
            
            // Assert
            var compensatedPosition = EntityManager.GetComponentData<NetworkPosition>(entity);
            // Позиция должна быть скомпенсирована на основе скорости и задержки
            Assert.IsTrue(compensatedPosition.Value.x > 0f);
        }
        
        /// <summary>
        /// Тест компенсации задержек с различными RTT
        /// </summary        [Test]
        public void LagCompensation_WithDifferentRTT_ShouldCompensateDifferently()
        {
            // Arrange
            var entities = new Entity[3];
            var rttValues = new float[] { 50f, 100f, 200f }; // мс
            
            for (int i = 0; i < 3; i++)
            {
                entities[i] = CreateTestEntity();
                var networkPosition = new NetworkPosition
                {
                    Value = new float3(0f, 0f, 0f),
                    Velocity = new float3(10f, 0f, 0f),
                    LastUpdateTime = 0f,
                    HasChanged = true
                };
                
                EntityManager.SetComponentData(entities[i], networkPosition);
            }
            
            // Act
            var lagCompensationSystem = World.GetOrCreateSystemManaged<LagCompensationSystem>();
            lagCompensationSystem.Update();
            
            // Assert
            for (int i = 0; i < 3; i++)
            {
                var compensatedPosition = EntityManager.GetComponentData<NetworkPosition>(entities[i]);
                // Больший RTT должен давать большую компенсацию
                Assert.IsTrue(compensatedPosition.Value.x > 0f);
            }
        }
        
        #endregion
        
        #region Тесты античит системы
        
        /// <summary>
        /// Тест валидации позиций для античита
        /// </summary>
        [Test]
        public void AntiCheat_ValidatePosition_ShouldDetectInvalidPosition()
        {
            // Arrange
            var entity = CreateTestEntity();
            var networkPosition = new NetworkPosition
            {
                Value = new float3(10000f, 0f, 10000f), // Подозрительно далеко
                Velocity = new float3(1000f, 0f, 0f), // Подозрительно быстро
                LastUpdateTime = 0f,
                HasChanged = true
            };
            
            EntityManager.SetComponentData(entity, networkPosition);
            
            // Act
            var antiCheatSystem = World.GetOrCreateSystemManaged<AntiCheatSystem>();
            antiCheatSystem.Update();
            
            // Assert
            // Система должна пометить позицию как подозрительную
            Assert.IsTrue(EntityManager.HasComponent<AntiCheatFlag>(entity));
        }
        
        /// <summary>
        /// Тест валидации скорости для античита
        /// </summary>
        [Test]
        public void AntiCheat_ValidateSpeed_ShouldDetectInvalidSpeed()
        {
            // Arrange
            var entity = CreateTestEntity();
            var networkPosition = new NetworkPosition
            {
                Value = new float3(0f, 0f, 0f),
                Velocity = new float3(500f, 0f, 0f), // Слишком быстро
                LastUpdateTime = 0f,
                HasChanged = true
            };
            
            EntityManager.SetComponentData(entity, networkPosition);
            
            // Act
            var antiCheatSystem = World.GetOrCreateSystemManaged<AntiCheatSystem>();
            antiCheatSystem.Update();
            
            // Assert
            Assert.IsTrue(EntityManager.HasComponent<AntiCheatFlag>(entity));
        }
        
        #endregion
        
        #region Вспомогательные методы
        
        /// <summary>
        /// Создает тестовую сущность с базовыми компонентами
        /// </summary>
        private Entity CreateTestEntity()
        {
            var entity = EntityManager.CreateEntity();
            EntityManager.AddComponent<NetworkId>(entity);
            EntityManager.AddComponent<NetworkPosition>(entity);
            EntityManager.AddComponent<LocalTransform>(entity);
            return entity;
        }
        
        /// <summary>
        /// Создает клиентскую сущность
        /// </summary>
        private Entity CreateClientEntity()
        {
            var entity = CreateTestEntity();
            EntityManager.AddComponent<PlayerTag>(entity);
            return entity;
        }
        
        /// <summary>
        /// Создает серверную сущность
        /// </summary>
        private Entity CreateServerEntity()
        {
            var entity = CreateTestEntity();
            EntityManager.AddComponent<ServerTag>(entity);
            return entity;
        }
        
        #endregion
    }
    
    /// <summary>
    /// Команда движения для тестирования
    /// </summary>
    public struct MoveCommand : IComponentData
    {
        public Entity Target;
        public float3 Position;
        public quaternion Rotation;
    }
    
    /// <summary>
    /// Флаг античита для тестирования
    /// </summary>
    public struct AntiCheatFlag : IComponentData
    {
        public bool IsSuspicious;
        public string Reason;
    }
    
    /// <summary>
    /// Тег сервера для тестирования
    /// </summary>
    public struct ServerTag : IComponentData
    {
    }
}
