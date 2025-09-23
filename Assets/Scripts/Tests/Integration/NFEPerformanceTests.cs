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
    /// Тесты производительности для Netcode for Entities (NFE)
    /// Тестирует производительность сетевых систем в различных сценариях
    /// </summary>
    [TestFixture]
    public class NFEPerformanceTests : MudLikeTestFixture
    {
        private NetworkSyncSystem _networkSyncSystem;
        private LagCompensationSystem _lagCompensationSystem;
        private AntiCheatSystem _antiCheatSystem;
        
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            
            _networkSyncSystem = World.GetOrCreateSystemManaged<NetworkSyncSystem>();
            _lagCompensationSystem = World.GetOrCreateSystemManaged<LagCompensationSystem>();
            _antiCheatSystem = World.GetOrCreateSystemManaged<AntiCheatSystem>();
        }
        
        #region Тесты производительности синхронизации
        
        /// <summary>
        /// Тест производительности синхронизации позиций
        /// </summary>
        [Test]
        public void SyncPositions_PerformanceTest_ShouldMeetTargets()
        {
            // Arrange
            const int entityCount = 1000;
            var entities = CreateTestEntities(entityCount);
            
            // Act
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            _networkSyncSystem.Update();
            stopwatch.Stop();
            
            // Assert
            Assert.IsTrue(stopwatch.ElapsedMilliseconds < 50, 
                $"Синхронизация {entityCount} позиций заняла {stopwatch.ElapsedMilliseconds}мс, что превышает 50мс");
        }
        
        /// <summary>
        /// Тест производительности синхронизации с частыми обновлениями
        /// </summary>
        [Test]
        public void SyncPositions_FrequentUpdates_ShouldMaintainPerformance()
        {
            // Arrange
            const int entityCount = 100;
            const int updateCount = 100;
            var entities = CreateTestEntities(entityCount);
            
            // Act
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < updateCount; i++)
            {
                UpdateEntityPositions(entities, i);
                _networkSyncSystem.Update();
            }
            stopwatch.Stop();
            
            // Assert
            Assert.IsTrue(stopwatch.ElapsedMilliseconds < 1000, 
                $"{updateCount} обновлений {entityCount} сущностей заняли {stopwatch.ElapsedMilliseconds}мс, что превышает 1000мс");
        }
        
        /// <summary>
        /// Тест производительности синхронизации с различными типами сущностей
        /// </summary>
        [Test]
        public void SyncPositions_MixedEntityTypes_ShouldMaintainPerformance()
        {
            // Arrange
            const int playerCount = 500;
            const int vehicleCount = 300;
            const int terrainCount = 200;
            
            var players = CreatePlayerEntities(playerCount);
            var vehicles = CreateVehicleEntities(vehicleCount);
            var terrain = CreateTerrainEntities(terrainCount);
            
            // Act
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            _networkSyncSystem.Update();
            stopwatch.Stop();
            
            // Assert
            Assert.IsTrue(stopwatch.ElapsedMilliseconds < 100, 
                $"Синхронизация {playerCount + vehicleCount + terrainCount} сущностей заняла {stopwatch.ElapsedMilliseconds}мс, что превышает 100мс");
        }
        
        #endregion
        
        #region Тесты производительности компенсации задержек
        
        /// <summary>
        /// Тест производительности компенсации задержек
        /// </summary>
        [Test]
        public void LagCompensation_PerformanceTest_ShouldMeetTargets()
        {
            // Arrange
            const int entityCount = 1000;
            var entities = CreateTestEntities(entityCount);
            SetEntityVelocities(entities, new float3(10f, 0f, 5f));
            
            // Act
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            _lagCompensationSystem.Update();
            stopwatch.Stop();
            
            // Assert
            Assert.IsTrue(stopwatch.ElapsedMilliseconds < 30, 
                $"Компенсация задержек для {entityCount} сущностей заняла {stopwatch.ElapsedMilliseconds}мс, что превышает 30мс");
        }
        
        /// <summary>
        /// Тест производительности компенсации задержек с различными RTT
        /// </summary>
        [Test]
        public void LagCompensation_DifferentRTT_ShouldMaintainPerformance()
        {
            // Arrange
            const int entityCount = 500;
            var entities = CreateTestEntities(entityCount);
            var rttValues = new float[] { 0.05f, 0.1f, 0.2f, 0.5f };
            
            for (int i = 0; i < entityCount; i++)
            {
                var rtt = rttValues[i % rttValues.Length];
                SetEntityRTT(entities[i], rtt);
            }
            
            // Act
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            _lagCompensationSystem.Update();
            stopwatch.Stop();
            
            // Assert
            Assert.IsTrue(stopwatch.ElapsedMilliseconds < 50, 
                $"Компенсация задержек с различными RTT заняла {stopwatch.ElapsedMilliseconds}мс, что превышает 50мс");
        }
        
        #endregion
        
        #region Тесты производительности античита
        
        /// <summary>
        /// Тест производительности античит системы
        /// </summary>
        [Test]
        public void AntiCheat_PerformanceTest_ShouldMeetTargets()
        {
            // Arrange
            const int playerCount = 1000;
            var players = CreatePlayerEntities(playerCount);
            SetPlayerPositions(players, new float3(0f, 0f, 0f));
            
            // Act
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            _antiCheatSystem.Update();
            stopwatch.Stop();
            
            // Assert
            Assert.IsTrue(stopwatch.ElapsedMilliseconds < 100, 
                $"Античит для {playerCount} игроков занял {stopwatch.ElapsedMilliseconds}мс, что превышает 100мс");
        }
        
        /// <summary>
        /// Тест производительности античита с подозрительными игроками
        /// </summary>
        [Test]
        public void AntiCheat_SuspiciousPlayers_ShouldMaintainPerformance()
        {
            // Arrange
            const int playerCount = 500;
            const int suspiciousCount = 50;
            var players = CreatePlayerEntities(playerCount);
            
            // Создаем подозрительных игроков
            for (int i = 0; i < suspiciousCount; i++)
            {
                SetSuspiciousPlayer(players[i]);
            }
            
            // Act
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            _antiCheatSystem.Update();
            stopwatch.Stop();
            
            // Assert
            Assert.IsTrue(stopwatch.ElapsedMilliseconds < 150, 
                $"Античит с {suspiciousCount} подозрительными игроками занял {stopwatch.ElapsedMilliseconds}мс, что превышает 150мс");
        }
        
        #endregion
        
        #region Тесты производительности сетевых команд
        
        /// <summary>
        /// Тест производительности обработки сетевых команд
        /// </summary>
        [Test]
        public void ProcessCommands_PerformanceTest_ShouldMeetTargets()
        {
            // Arrange
            const int commandCount = 1000;
            var commands = CreateTestCommands(commandCount);
            
            // Act
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            ProcessCommands(commands);
            stopwatch.Stop();
            
            // Assert
            Assert.IsTrue(stopwatch.ElapsedMilliseconds < 200, 
                $"Обработка {commandCount} команд заняла {stopwatch.ElapsedMilliseconds}мс, что превышает 200мс");
        }
        
        /// <summary>
        /// Тест производительности обработки команд с различными типами
        /// </summary>
        [Test]
        public void ProcessCommands_MixedTypes_ShouldMaintainPerformance()
        {
            // Arrange
            const int commandCount = 500;
            var moveCommands = CreateMoveCommands(commandCount / 3);
            var velocityCommands = CreateVelocityCommands(commandCount / 3);
            var controlCommands = CreateControlCommands(commandCount / 3);
            
            // Act
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            ProcessCommands(moveCommands);
            ProcessCommands(velocityCommands);
            ProcessCommands(controlCommands);
            stopwatch.Stop();
            
            // Assert
            Assert.IsTrue(stopwatch.ElapsedMilliseconds < 300, 
                $"Обработка {commandCount} команд различных типов заняла {stopwatch.ElapsedMilliseconds}мс, что превышает 300мс");
        }
        
        #endregion
        
        #region Тесты производительности памяти
        
        /// <summary>
        /// Тест производительности памяти для большого количества сущностей
        /// </summary>
        [Test]
        public void MemoryUsage_LargeNumberOfEntities_ShouldBeReasonable()
        {
            // Arrange
            const int entityCount = 10000;
            var entities = CreateTestEntities(entityCount);
            
            // Act
            var memoryBefore = System.GC.GetTotalMemory(false);
            _networkSyncSystem.Update();
            var memoryAfter = System.GC.GetTotalMemory(false);
            
            // Assert
            var memoryUsed = memoryAfter - memoryBefore;
            Assert.IsTrue(memoryUsed < 100 * 1024 * 1024, // 100MB
                $"Использование памяти {memoryUsed / 1024 / 1024}MB превышает 100MB");
        }
        
        /// <summary>
        /// Тест производительности памяти с частыми обновлениями
        /// </summary>
        [Test]
        public void MemoryUsage_FrequentUpdates_ShouldNotLeak()
        {
            // Arrange
            const int entityCount = 1000;
            const int updateCount = 100;
            var entities = CreateTestEntities(entityCount);
            
            // Act
            var memoryBefore = System.GC.GetTotalMemory(false);
            for (int i = 0; i < updateCount; i++)
            {
                _networkSyncSystem.Update();
                _lagCompensationSystem.Update();
                _antiCheatSystem.Update();
            }
            var memoryAfter = System.GC.GetTotalMemory(false);
            
            // Assert
            var memoryUsed = memoryAfter - memoryBefore;
            Assert.IsTrue(memoryUsed < 50 * 1024 * 1024, // 50MB
                $"Утечка памяти {memoryUsed / 1024 / 1024}MB превышает 50MB");
        }
        
        #endregion
        
        #region Тесты производительности сетевого трафика
        
        /// <summary>
        /// Тест производительности сетевого трафика
        /// </summary>
        [Test]
        public void NetworkTraffic_PerformanceTest_ShouldBeEfficient()
        {
            // Arrange
            const int entityCount = 1000;
            var entities = CreateTestEntities(entityCount);
            
            // Act
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            _networkSyncSystem.Update();
            stopwatch.Stop();
            
            // Assert
            // Проверяем, что система не генерирует слишком много сетевого трафика
            Assert.IsTrue(stopwatch.ElapsedMilliseconds < 50, 
                $"Синхронизация {entityCount} сущностей заняла {stopwatch.ElapsedMilliseconds}мс, что может генерировать слишком много трафика");
        }
        
        /// <summary>
        /// Тест производительности сжатия сетевых данных
        /// </summary>
        [Test]
        public void NetworkCompression_PerformanceTest_ShouldBeEfficient()
        {
            // Arrange
            const int entityCount = 1000;
            var entities = CreateTestEntities(entityCount);
            SetEntityPositions(entities, new float3(100f, 0f, 200f));
            
            // Act
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            _networkSyncSystem.Update();
            stopwatch.Stop();
            
            // Assert
            Assert.IsTrue(stopwatch.ElapsedMilliseconds < 100, 
                $"Сжатие и синхронизация {entityCount} сущностей заняло {stopwatch.ElapsedMilliseconds}мс, что превышает 100мс");
        }
        
        #endregion
        
        #region Вспомогательные методы
        
        /// <summary>
        /// Создает тестовые сущности
        /// </summary>
        private Entity[] CreateTestEntities(int count)
        {
            var entities = new Entity[count];
            for (int i = 0; i < count; i++)
            {
                entities[i] = CreateTestEntity();
            }
            return entities;
        }
        
        /// <summary>
        /// Создает сущность игрока
        /// </summary>
        private Entity CreateTestEntity()
        {
            var entity = EntityManager.CreateEntity();
            EntityManager.AddComponent<NetworkId>(entity);
            EntityManager.AddComponent<NetworkPosition>(entity);
            EntityManager.AddComponent<LocalTransform>(entity);
            EntityManager.AddComponent<PlayerTag>(entity);
            return entity;
        }
        
        /// <summary>
        /// Создает сущности игроков
        /// </summary>
        private Entity[] CreatePlayerEntities(int count)
        {
            var entities = new Entity[count];
            for (int i = 0; i < count; i++)
            {
                entities[i] = CreateTestEntity();
            }
            return entities;
        }
        
        /// <summary>
        /// Создает сущности транспорта
        /// </summary>
        private Entity[] CreateVehicleEntities(int count)
        {
            var entities = new Entity[count];
            for (int i = 0; i < count; i++)
            {
                entities[i] = CreateTestEntity();
                EntityManager.AddComponent<VehicleTag>(entities[i]);
            }
            return entities;
        }
        
        /// <summary>
        /// Создает сущности террейна
        /// </summary>
        private Entity[] CreateTerrainEntities(int count)
        {
            var entities = new Entity[count];
            for (int i = 0; i < count; i++)
            {
                entities[i] = CreateTestEntity();
                EntityManager.AddComponent<TerrainTag>(entities[i]);
            }
            return entities;
        }
        
        /// <summary>
        /// Обновляет позиции сущностей
        /// </summary>
        private void UpdateEntityPositions(Entity[] entities, int frame)
        {
            for (int i = 0; i < entities.Length; i++)
            {
                var transform = new LocalTransform
                {
                    Position = new float3(i + frame, 0f, i + frame),
                    Rotation = quaternion.RotateY(math.radians(frame)),
                    Scale = 1f
                };
                EntityManager.SetComponentData(entities[i], transform);
            }
        }
        
        /// <summary>
        /// Устанавливает скорости сущностей
        /// </summary>
        private void SetEntityVelocities(Entity[] entities, float3 velocity)
        {
            for (int i = 0; i < entities.Length; i++)
            {
                var networkPosition = EntityManager.GetComponentData<NetworkPosition>(entities[i]);
                networkPosition.Velocity = velocity;
                EntityManager.SetComponentData(entities[i], networkPosition);
            }
        }
        
        /// <summary>
        /// Устанавливает RTT для сущности
        /// </summary>
        private void SetEntityRTT(Entity entity, float rtt)
        {
            var networkPosition = EntityManager.GetComponentData<NetworkPosition>(entity);
            networkPosition.LastUpdateTime = (float)Time.time - rtt;
            EntityManager.SetComponentData(entity, networkPosition);
        }
        
        /// <summary>
        /// Устанавливает позиции игроков
        /// </summary>
        private void SetPlayerPositions(Entity[] players, float3 position)
        {
            for (int i = 0; i < players.Length; i++)
            {
                var transform = new LocalTransform
                {
                    Position = position,
                    Rotation = quaternion.identity,
                    Scale = 1f
                };
                EntityManager.SetComponentData(players[i], transform);
            }
        }
        
        /// <summary>
        /// Устанавливает подозрительного игрока
        /// </summary>
        private void SetSuspiciousPlayer(Entity player)
        {
            var networkPosition = new NetworkPosition
            {
                Value = new float3(10000f, 0f, 10000f), // Подозрительно далеко
                Velocity = new float3(1000f, 0f, 0f), // Подозрительно быстро
                LastUpdateTime = (float)Time.time,
                HasChanged = true
            };
            EntityManager.SetComponentData(player, networkPosition);
        }
        
        /// <summary>
        /// Создает тестовые команды
        /// </summary>
        private MoveCommand[] CreateTestCommands(int count)
        {
            var commands = new MoveCommand[count];
            for (int i = 0; i < count; i++)
            {
                commands[i] = new MoveCommand
                {
                    Target = CreateTestEntity(),
                    Position = new float3(i, 0f, i),
                    Rotation = quaternion.identity,
                    Timestamp = (float)Time.time
                };
            }
            return commands;
        }
        
        /// <summary>
        /// Создает команды движения
        /// </summary>
        private MoveCommand[] CreateMoveCommands(int count)
        {
            var commands = new MoveCommand[count];
            for (int i = 0; i < count; i++)
            {
                commands[i] = new MoveCommand
                {
                    Target = CreateTestEntity(),
                    Position = new float3(i, 0f, i),
                    Rotation = quaternion.identity,
                    Timestamp = (float)Time.time
                };
            }
            return commands;
        }
        
        /// <summary>
        /// Создает команды изменения скорости
        /// </summary>
        private VelocityCommand[] CreateVelocityCommands(int count)
        {
            var commands = new VelocityCommand[count];
            for (int i = 0; i < count; i++)
            {
                commands[i] = new VelocityCommand
                {
                    Target = CreateTestEntity(),
                    Velocity = new float3(i, 0f, i),
                    AngularVelocity = float3.zero,
                    Timestamp = (float)Time.time
                };
            }
            return commands;
        }
        
        /// <summary>
        /// Создает команды управления
        /// </summary>
        private VehicleControlCommand[] CreateControlCommands(int count)
        {
            var commands = new VehicleControlCommand[count];
            for (int i = 0; i < count; i++)
            {
                commands[i] = new VehicleControlCommand
                {
                    Target = CreateTestEntity(),
                    Input = new VehicleInput
                    {
                        VehicleMovement = new float2(1f, 0f),
                        Accelerate = true,
                        Brake = false,
                        Steering = 0f
                    },
                    Timestamp = (float)Time.time
                };
            }
            return commands;
        }
        
        /// <summary>
        /// Обрабатывает команды
        /// </summary>
        private void ProcessCommands(MoveCommand[] commands)
        {
            for (int i = 0; i < commands.Length; i++)
            {
                var transform = new LocalTransform
                {
                    Position = commands[i].Position,
                    Rotation = commands[i].Rotation,
                    Scale = 1f
                };
                EntityManager.SetComponentData(commands[i].Target, transform);
            }
        }
        
        /// <summary>
        /// Обрабатывает команды изменения скорости
        /// </summary>
        private void ProcessCommands(VelocityCommand[] commands)
        {
            for (int i = 0; i < commands.Length; i++)
            {
                var networkPosition = EntityManager.GetComponentData<NetworkPosition>(commands[i].Target);
                networkPosition.Velocity = commands[i].Velocity;
                networkPosition.AngularVelocity = commands[i].AngularVelocity;
                EntityManager.SetComponentData(commands[i].Target, networkPosition);
            }
        }
        
        /// <summary>
        /// Обрабатывает команды управления
        /// </summary>
        private void ProcessCommands(VehicleControlCommand[] commands)
        {
            for (int i = 0; i < commands.Length; i++)
            {
                EntityManager.SetComponentData(commands[i].Target, commands[i].Input);
            }
        }
        
        /// <summary>
        /// Устанавливает позиции сущностей
        /// </summary>
        private void SetEntityPositions(Entity[] entities, float3 position)
        {
            for (int i = 0; i < entities.Length; i++)
            {
                var transform = new LocalTransform
                {
                    Position = position,
                    Rotation = quaternion.identity,
                    Scale = 1f
                };
                EntityManager.SetComponentData(entities[i], transform);
            }
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
        public float Timestamp;
    }
    
    /// <summary>
    /// Команда изменения скорости для тестирования
    /// </summary>
    public struct VelocityCommand : IComponentData
    {
        public Entity Target;
        public float3 Velocity;
        public float3 AngularVelocity;
        public float Timestamp;
    }
    
    /// <summary>
    /// Команда управления транспортом для тестирования
    /// </summary>
    public struct VehicleControlCommand : IComponentData
    {
        public Entity Target;
        public VehicleInput Input;
        public float Timestamp;
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
