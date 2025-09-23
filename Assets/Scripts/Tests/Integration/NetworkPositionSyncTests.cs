using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.NetCode;
using Unity.Collections;
using NUnit.Framework;
using MudLike.Networking.Components;
using MudLike.Core.Components;
using MudLike.Networking.Systems;
using static MudLike.Core.Components.Position;

namespace MudLike.Tests.Integration
{
    /// <summary>
    /// Интеграционные тесты для синхронизации позиций в NFE
    /// Тестирует полный цикл синхронизации позиций между клиентом и сервером
    /// </summary>
    [TestFixture]
    public class NetworkPositionSyncTests : MudLikeTestFixture
    {
        private Entity _clientEntity;
        private Entity _serverEntity;
        private NetworkSyncSystem _networkSyncSystem;
        
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            
            _networkSyncSystem = World.GetOrCreateSystemManaged<NetworkSyncSystem>();
            _clientEntity = CreateClientEntity();
            _serverEntity = CreateServerEntity();
        }
        
        #region Тесты синхронизации позиций
        
        /// <summary>
        /// Тест синхронизации позиции от клиента к серверу
        /// </summary>
        [Test]
        public void SyncPosition_ClientToServer_ShouldUpdateServerPosition()
        {
            // Arrange
            var clientPosition = new float3(100f, 5f, 200f);
            var clientRotation = quaternion.RotateY(math.radians(45f));
            
            SetClientPosition(_clientEntity, clientPosition, clientRotation);
            
            // Act
            _networkSyncSystem.Update();
            
            // Assert
            var serverNetworkPosition = EntityManager.GetComponentData<NetworkPosition>(_serverEntity);
            Assert.AreEqual(clientPosition, serverNetworkPosition.Value);
            Assert.AreEqual(clientRotation, serverNetworkPosition.Rotation);
            Assert.IsTrue(serverNetworkPosition.HasChanged);
        }
        
        /// <summary>
        /// Тест синхронизации позиции от сервера к клиенту
        /// </summary>
        [Test]
        public void SyncPosition_ServerToClient_ShouldUpdateClientPosition()
        {
            // Arrange
            var serverPosition = new float3(300f, 10f, 400f);
            var serverRotation = quaternion.RotateY(math.radians(90f));
            
            SetServerPosition(_serverEntity, serverPosition, serverRotation);
            
            // Act
            _networkSyncSystem.Update();
            
            // Assert
            var clientNetworkPosition = EntityManager.GetComponentData<NetworkPosition>(_clientEntity);
            Assert.AreEqual(serverPosition, clientNetworkPosition.Value);
            Assert.AreEqual(serverRotation, clientNetworkPosition.Rotation);
            Assert.IsTrue(clientNetworkPosition.HasChanged);
        }
        
        /// <summary>
        /// Тест синхронизации с пороговыми значениями
        /// </summary>
        [Test]
        public void SyncPosition_WithThreshold_ShouldOnlySyncWhenExceeded()
        {
            // Arrange
            var smallChange = new float3(0.005f, 0f, 0f); // Меньше порога
            var largeChange = new float3(0.02f, 0f, 0f); // Больше порога
            
            // Тест с малыми изменениями
            SetClientPosition(_clientEntity, smallChange, quaternion.identity);
            _networkSyncSystem.Update();
            
            var networkPosition1 = EntityManager.GetComponentData<NetworkPosition>(_serverEntity);
            Assert.IsFalse(networkPosition1.HasChanged);
            
            // Тест с большими изменениями
            SetClientPosition(_clientEntity, largeChange, quaternion.identity);
            _networkSyncSystem.Update();
            
            var networkPosition2 = EntityManager.GetComponentData<NetworkPosition>(_serverEntity);
            Assert.IsTrue(networkPosition2.HasChanged);
        }
        
        /// <summary>
        /// Тест синхронизации скорости
        /// </summary>
        [Test]
        public void SyncPosition_WithVelocity_ShouldSyncVelocity()
        {
            // Arrange
            var position = new float3(50f, 0f, 100f);
            var velocity = new float3(10f, 0f, 5f);
            var rotation = quaternion.identity;
            
            SetClientPosition(_clientEntity, position, rotation);
            SetClientVelocity(_clientEntity, velocity);
            
            // Act
            _networkSyncSystem.Update();
            
            // Assert
            var networkPosition = EntityManager.GetComponentData<NetworkPosition>(_serverEntity);
            Assert.AreEqual(velocity, networkPosition.Velocity);
        }
        
        /// <summary>
        /// Тест синхронизации угловой скорости
        /// </summary>
        [Test]
        public void SyncPosition_WithAngularVelocity_ShouldSyncAngularVelocity()
        {
            // Arrange
            var position = new float3(0f, 0f, 0f);
            var rotation = quaternion.RotateY(math.radians(30f));
            var angularVelocity = new float3(0f, 2f, 0f);
            
            SetClientPosition(_clientEntity, position, rotation);
            SetClientAngularVelocity(_clientEntity, angularVelocity);
            
            // Act
            _networkSyncSystem.Update();
            
            // Assert
            var networkPosition = EntityManager.GetComponentData<NetworkPosition>(_serverEntity);
            Assert.AreEqual(angularVelocity, networkPosition.AngularVelocity);
        }
        
        #endregion
        
        #region Тесты множественной синхронизации
        
        /// <summary>
        /// Тест синхронизации множественных сущностей
        /// </summary>
        [Test]
        public void SyncMultipleEntities_ShouldSyncAll()
        {
            // Arrange
            var entities = new Entity[5];
            var positions = new float3[5];
            var rotations = new quaternion[5];
            
            for (int i = 0; i < 5; i++)
            {
                entities[i] = CreateClientEntity();
                positions[i] = new float3(i * 10f, 0f, i * 20f);
                rotations[i] = quaternion.RotateY(math.radians(i * 30f));
                
                SetClientPosition(entities[i], positions[i], rotations[i]);
            }
            
            // Act
            _networkSyncSystem.Update();
            
            // Assert
            for (int i = 0; i < 5; i++)
            {
                var networkPosition = EntityManager.GetComponentData<NetworkPosition>(entities[i]);
                Assert.AreEqual(positions[i], networkPosition.Value);
                Assert.AreEqual(rotations[i], networkPosition.Rotation);
                Assert.IsTrue(networkPosition.HasChanged);
            }
        }
        
        /// <summary>
        /// Тест синхронизации с различными типами сущностей
        /// </summary>
        [Test]
        public void SyncDifferentEntityTypes_ShouldHandleAll()
        {
            // Arrange
            var playerEntity = CreateClientEntity();
            var vehicleEntity = CreateVehicleEntity();
            var terrainEntity = CreateTerrainEntity();
            
            SetClientPosition(playerEntity, new float3(0f, 0f, 0f), quaternion.identity);
            SetClientPosition(vehicleEntity, new float3(100f, 0f, 0f), quaternion.identity);
            SetClientPosition(terrainEntity, new float3(200f, 0f, 0f), quaternion.identity);
            
            // Act
            _networkSyncSystem.Update();
            
            // Assert
            Assert.IsTrue(EntityManager.GetComponentData<NetworkPosition>(playerEntity).HasChanged);
            Assert.IsTrue(EntityManager.GetComponentData<NetworkPosition>(vehicleEntity).HasChanged);
            Assert.IsTrue(EntityManager.GetComponentData<NetworkPosition>(terrainEntity).HasChanged);
        }
        
        #endregion
        
        #region Тесты производительности синхронизации
        
        /// <summary>
        /// Тест производительности синхронизации большого количества сущностей
        /// </summary>
        [Test]
        public void SyncLargeNumberOfEntities_ShouldCompleteInReasonableTime()
        {
            // Arrange
            const int entityCount = 1000;
            var entities = new Entity[entityCount];
            
            for (int i = 0; i < entityCount; i++)
            {
                entities[i] = CreateClientEntity();
                var position = new float3(i, 0f, i);
                var rotation = quaternion.RotateY(math.radians(i));
                SetClientPosition(entities[i], position, rotation);
            }
            
            // Act
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            _networkSyncSystem.Update();
            stopwatch.Stop();
            
            // Assert
            Assert.IsTrue(stopwatch.ElapsedMilliseconds < 100, 
                $"Синхронизация {entityCount} сущностей заняла {stopwatch.ElapsedMilliseconds}мс, что превышает 100мс");
        }
        
        /// <summary>
        /// Тест производительности с частыми обновлениями
        /// </summary>
        [Test]
        public void SyncFrequentUpdates_ShouldMaintainPerformance()
        {
            // Arrange
            var entity = CreateClientEntity();
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            
            // Act - выполняем множество обновлений
            for (int i = 0; i < 100; i++)
            {
                var position = new float3(i, 0f, i);
                var rotation = quaternion.RotateY(math.radians(i));
                SetClientPosition(entity, position, rotation);
                _networkSyncSystem.Update();
            }
            
            stopwatch.Stop();
            
            // Assert
            Assert.IsTrue(stopwatch.ElapsedMilliseconds < 1000, 
                $"100 обновлений заняли {stopwatch.ElapsedMilliseconds}мс, что превышает 1000мс");
        }
        
        #endregion
        
        #region Тесты сетевых команд
        
        /// <summary>
        /// Тест обработки команд движения
        /// </summary>
        [Test]
        public void ProcessMoveCommands_ShouldUpdatePositions()
        {
            // Arrange
            var entity = CreateClientEntity();
            var command = new MoveCommand
            {
                Target = entity,
                Position = new float3(50f, 0f, 100f),
                Rotation = quaternion.RotateY(math.radians(45f))
            };
            
            // Act
            var commandBuffer = new EntityCommandBuffer(Allocator.Temp);
            commandBuffer.AddComponent(entity, command);
            commandBuffer.Playback(EntityManager);
            commandBuffer.Dispose();
            
            _networkSyncSystem.Update();
            
            // Assert
            var networkPosition = EntityManager.GetComponentData<NetworkPosition>(entity);
            Assert.AreEqual(command.Position, networkPosition.Value);
            Assert.AreEqual(command.Rotation, networkPosition.Rotation);
        }
        
        /// <summary>
        /// Тест обработки команд с задержкой
        /// </summary>
        [Test]
        public void ProcessDelayedCommands_ShouldHandleCorrectly()
        {
            // Arrange
            var entity = CreateClientEntity();
            var command = new MoveCommand
            {
                Target = entity,
                Position = new float3(100f, 0f, 200f),
                Rotation = quaternion.identity
            };
            
            // Симулируем задержку
            var networkPosition = EntityManager.GetComponentData<NetworkPosition>(entity);
            networkPosition.LastUpdateTime = (float)Time.time - 0.1f; // 100мс задержка
            EntityManager.SetComponentData(entity, networkPosition);
            
            // Act
            var commandBuffer = new EntityCommandBuffer(Allocator.Temp);
            commandBuffer.AddComponent(entity, command);
            commandBuffer.Playback(EntityManager);
            commandBuffer.Dispose();
            
            _networkSyncSystem.Update();
            
            // Assert
            var updatedNetworkPosition = EntityManager.GetComponentData<NetworkPosition>(entity);
            Assert.AreEqual(command.Position, updatedNetworkPosition.Value);
        }
        
        #endregion
        
        #region Вспомогательные методы
        
        /// <summary>
        /// Создает клиентскую сущность
        /// </summary>
        private Entity CreateClientEntity()
        {
            var entity = EntityManager.CreateEntity();
            EntityManager.AddComponent<NetworkId>(entity);
            EntityManager.AddComponent<NetworkPosition>(entity);
            EntityManager.AddComponent<LocalTransform>(entity);
            EntityManager.AddComponent<PlayerTag>(entity);
            return entity;
        }
        
        /// <summary>
        /// Создает серверную сущность
        /// </summary>
        private Entity CreateServerEntity()
        {
            var entity = EntityManager.CreateEntity();
            EntityManager.AddComponent<NetworkId>(entity);
            EntityManager.AddComponent<NetworkPosition>(entity);
            EntityManager.AddComponent<LocalTransform>(entity);
            EntityManager.AddComponent<ServerTag>(entity);
            return entity;
        }
        
        /// <summary>
        /// Создает транспортную сущность
        /// </summary>
        private Entity CreateVehicleEntity()
        {
            var entity = CreateClientEntity();
            EntityManager.AddComponent<VehicleTag>(entity);
            return entity;
        }
        
        /// <summary>
        /// Создает террейновую сущность
        /// </summary>
        private Entity CreateTerrainEntity()
        {
            var entity = CreateClientEntity();
            EntityManager.AddComponent<TerrainTag>(entity);
            return entity;
        }
        
        /// <summary>
        /// Устанавливает позицию клиента
        /// </summary>
        private void SetClientPosition(Entity entity, float3 position, quaternion rotation)
        {
            var transform = new LocalTransform
            {
                Position = position,
                Rotation = rotation,
                Scale = 1f
            };
            EntityManager.SetComponentData(entity, transform);
        }
        
        /// <summary>
        /// Устанавливает скорость клиента
        /// </summary>
        private void SetClientVelocity(Entity entity, float3 velocity)
        {
            var networkPosition = EntityManager.GetComponentData<NetworkPosition>(entity);
            networkPosition.Velocity = velocity;
            EntityManager.SetComponentData(entity, networkPosition);
        }
        
        /// <summary>
        /// Устанавливает угловую скорость клиента
        /// </summary>
        private void SetClientAngularVelocity(Entity entity, float3 angularVelocity)
        {
            var networkPosition = EntityManager.GetComponentData<NetworkPosition>(entity);
            networkPosition.AngularVelocity = angularVelocity;
            EntityManager.SetComponentData(entity, networkPosition);
        }
        
        /// <summary>
        /// Устанавливает позицию сервера
        /// </summary>
        private void SetServerPosition(Entity entity, float3 position, quaternion rotation)
        {
            var networkPosition = new NetworkPosition
            {
                Value = position,
                Rotation = rotation,
                HasChanged = true,
                LastUpdateTime = (float)Time.time
            };
            EntityManager.SetComponentData(entity, networkPosition);
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
    /// Тег сервера для тестирования
    /// </summary>
    public struct ServerTag : IComponentData
    {
    }
    
    /// <summary>
    /// Тег транспорта для тестирования
    /// </summary>
    public struct VehicleTag : IComponentData
    {
    }
    
    /// <summary>
    /// Тег террейна для тестирования
    /// </summary>
    public struct TerrainTag : IComponentData
    {
    }
}
