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
    /// Интеграционные тесты для античит системы в NFE
    /// Тестирует обнаружение и предотвращение читерства в сетевой игре
    /// </summary>
    [TestFixture]
    public class AntiCheatTests : MudLikeTestFixture
    {
        private Entity _playerEntity;
        private Entity _suspiciousEntity;
        private AntiCheatSystem _antiCheatSystem;
        
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            
            _antiCheatSystem = World.GetOrCreateSystemManaged<AntiCheatSystem>();
            _playerEntity = CreatePlayerEntity();
            _suspiciousEntity = CreateSuspiciousEntity();
        }
        
        #region Тесты валидации позиций
        
        /// <summary>
        /// Тест обнаружения телепортации
        /// </summary>
        [Test]
        public void DetectTeleportation_WithLargePositionChange_ShouldFlagAsSuspicious()
        {
            // Arrange
            var networkPosition = new NetworkPosition
            {
                Value = new float3(0f, 0f, 0f),
                LastUpdateTime = (float)Time.time - 0.1f,
                HasChanged = true
            };
            
            EntityManager.SetComponentData(_playerEntity, networkPosition);
            
            // Симулируем телепортацию
            var teleportedPosition = new NetworkPosition
            {
                Value = new float3(1000f, 0f, 1000f), // Очень далеко
                LastUpdateTime = (float)Time.time,
                HasChanged = true
            };
            
            EntityManager.SetComponentData(_playerEntity, teleportedPosition);
            
            // Act
            _antiCheatSystem.Update();
            
            // Assert
            Assert.IsTrue(EntityManager.HasComponent<AntiCheatFlag>(_playerEntity));
            var flag = EntityManager.GetComponentData<AntiCheatFlag>(_playerEntity);
            Assert.IsTrue(flag.IsSuspicious);
            Assert.AreEqual("Teleportation detected", flag.Reason);
        }
        
        /// <summary>
        /// Тест обнаружения превышения скорости
        /// </summary>
        [Test]
        public void DetectSpeedHack_WithExcessiveVelocity_ShouldFlagAsSuspicious()
        {
            // Arrange
            var networkPosition = new NetworkPosition
            {
                Value = new float3(0f, 0f, 0f),
                Velocity = new float3(500f, 0f, 0f), // Слишком быстро
                LastUpdateTime = (float)Time.time,
                HasChanged = true
            };
            
            EntityManager.SetComponentData(_playerEntity, networkPosition);
            
            // Act
            _antiCheatSystem.Update();
            
            // Assert
            Assert.IsTrue(EntityManager.HasComponent<AntiCheatFlag>(_playerEntity));
            var flag = EntityManager.GetComponentData<AntiCheatFlag>(_playerEntity);
            Assert.IsTrue(flag.IsSuspicious);
            Assert.AreEqual("Speed hack detected", flag.Reason);
        }
        
        /// <summary>
        /// Тест обнаружения полета
        /// </summary>
        [Test]
        public void DetectFlying_WithExcessiveHeight_ShouldFlagAsSuspicious()
        {
            // Arrange
            var networkPosition = new NetworkPosition
            {
                Value = new float3(0f, 1000f, 0f), // Слишком высоко
                Velocity = new float3(0f, 0f, 0f),
                LastUpdateTime = (float)Time.time,
                HasChanged = true
            };
            
            EntityManager.SetComponentData(_playerEntity, networkPosition);
            
            // Act
            _antiCheatSystem.Update();
            
            // Assert
            Assert.IsTrue(EntityManager.HasComponent<AntiCheatFlag>(_playerEntity));
            var flag = EntityManager.GetComponentData<AntiCheatFlag>(_playerEntity);
            Assert.IsTrue(flag.IsSuspicious);
            Assert.AreEqual("Flying detected", flag.Reason);
        }
        
        /// <summary>
        /// Тест обнаружения прохождения через стены
        /// </summary>
        [Test]
        public void DetectWallHack_WithImpossibleMovement_ShouldFlagAsSuspicious()
        {
            // Arrange
            var networkPosition = new NetworkPosition
            {
                Value = new float3(0f, 0f, 0f),
                Velocity = new float3(0f, 0f, 0f),
                LastUpdateTime = (float)Time.time,
                HasChanged = true
            };
            
            EntityManager.SetComponentData(_playerEntity, networkPosition);
            
            // Симулируем прохождение через стену
            var wallHackPosition = new NetworkPosition
            {
                Value = new float3(0f, 0f, 100f), // Прошел через стену
                Velocity = new float3(0f, 0f, 0f),
                LastUpdateTime = (float)Time.time + 0.1f,
                HasChanged = true
            };
            
            EntityManager.SetComponentData(_playerEntity, wallHackPosition);
            
            // Act
            _antiCheatSystem.Update();
            
            // Assert
            Assert.IsTrue(EntityManager.HasComponent<AntiCheatFlag>(_playerEntity));
            var flag = EntityManager.GetComponentData<AntiCheatFlag>(_playerEntity);
            Assert.IsTrue(flag.IsSuspicious);
            Assert.AreEqual("Wall hack detected", flag.Reason);
        }
        
        #endregion
        
        #region Тесты валидации ввода
        
        /// <summary>
        /// Тест обнаружения макросов
        /// </summary>
        [Test]
        public void DetectMacros_WithRepetitiveInput_ShouldFlagAsSuspicious()
        {
            // Arrange
            var playerInput = new PlayerInput
            {
                VehicleMovement = new float2(1f, 0f),
                Accelerate = true,
                Brake = false,
                Steering = 0f
            };
            
            EntityManager.SetComponentData(_playerEntity, playerInput);
            
            // Симулируем повторяющийся ввод
            for (int i = 0; i < 100; i++)
            {
                var input = new PlayerInput
                {
                    VehicleMovement = new float2(1f, 0f),
                    Accelerate = true,
                    Brake = false,
                    Steering = 0f
                };
                
                EntityManager.SetComponentData(_playerEntity, input);
                _antiCheatSystem.Update();
            }
            
            // Assert
            Assert.IsTrue(EntityManager.HasComponent<AntiCheatFlag>(_playerEntity));
            var flag = EntityManager.GetComponentData<AntiCheatFlag>(_playerEntity);
            Assert.IsTrue(flag.IsSuspicious);
            Assert.AreEqual("Macro detected", flag.Reason);
        }
        
        /// <summary>
        /// Тест обнаружения ботов
        /// </summary>
        [Test]
        public void DetectBot_WithPerfectInput_ShouldFlagAsSuspicious()
        {
            // Arrange
            var playerInput = new PlayerInput
            {
                VehicleMovement = new float2(1f, 0f),
                Accelerate = true,
                Brake = false,
                Steering = 0f
            };
            
            EntityManager.SetComponentData(_playerEntity, playerInput);
            
            // Симулируем идеальный ввод (слишком точный для человека)
            for (int i = 0; i < 50; i++)
            {
                var input = new PlayerInput
                {
                    VehicleMovement = new float2(1f, 0f), // Идеально точно
                    Accelerate = true,
                    Brake = false,
                    Steering = 0f
                };
                
                EntityManager.SetComponentData(_playerEntity, input);
                _antiCheatSystem.Update();
            }
            
            // Assert
            Assert.IsTrue(EntityManager.HasComponent<AntiCheatFlag>(_playerEntity));
            var flag = EntityManager.GetComponentData<AntiCheatFlag>(_playerEntity);
            Assert.IsTrue(flag.IsSuspicious);
            Assert.AreEqual("Bot detected", flag.Reason);
        }
        
        #endregion
        
        #region Тесты валидации времени
        
        /// <summary>
        /// Тест обнаружения манипуляций со временем
        /// </summary>
        [Test]
        public void DetectTimeManipulation_WithInvalidTimestamps_ShouldFlagAsSuspicious()
        {
            // Arrange
            var networkPosition = new NetworkPosition
            {
                Value = new float3(0f, 0f, 0f),
                LastUpdateTime = (float)Time.time + 1f, // Время в будущем
                HasChanged = true
            };
            
            EntityManager.SetComponentData(_playerEntity, networkPosition);
            
            // Act
            _antiCheatSystem.Update();
            
            // Assert
            Assert.IsTrue(EntityManager.HasComponent<AntiCheatFlag>(_playerEntity));
            var flag = EntityManager.GetComponentData<AntiCheatFlag>(_playerEntity);
            Assert.IsTrue(flag.IsSuspicious);
            Assert.AreEqual("Time manipulation detected", flag.Reason);
        }
        
        /// <summary>
        /// Тест обнаружения ускорения времени
        /// </summary>
        [Test]
        public void DetectTimeAcceleration_WithRapidUpdates_ShouldFlagAsSuspicious()
        {
            // Arrange
            var networkPosition = new NetworkPosition
            {
                Value = new float3(0f, 0f, 0f),
                LastUpdateTime = (float)Time.time,
                HasChanged = true
            };
            
            EntityManager.SetComponentData(_playerEntity, networkPosition);
            
            // Симулируем очень частые обновления
            for (int i = 0; i < 1000; i++)
            {
                var position = new NetworkPosition
                {
                    Value = new float3(i, 0f, i),
                    LastUpdateTime = (float)Time.time + i * 0.001f, // Очень быстро
                    HasChanged = true
                };
                
                EntityManager.SetComponentData(_playerEntity, position);
                _antiCheatSystem.Update();
            }
            
            // Assert
            Assert.IsTrue(EntityManager.HasComponent<AntiCheatFlag>(_playerEntity));
            var flag = EntityManager.GetComponentData<AntiCheatFlag>(_playerEntity);
            Assert.IsTrue(flag.IsSuspicious);
            Assert.AreEqual("Time acceleration detected", flag.Reason);
        }
        
        #endregion
        
        #region Тесты валидации данных
        
        /// <summary>
        /// Тест обнаружения недопустимых значений
        /// </summary>
        [Test]
        public void DetectInvalidValues_WithNaN_ShouldFlagAsSuspicious()
        {
            // Arrange
            var networkPosition = new NetworkPosition
            {
                Value = new float3(float.NaN, 0f, 0f), // Недопустимое значение
                LastUpdateTime = (float)Time.time,
                HasChanged = true
            };
            
            EntityManager.SetComponentData(_playerEntity, networkPosition);
            
            // Act
            _antiCheatSystem.Update();
            
            // Assert
            Assert.IsTrue(EntityManager.HasComponent<AntiCheatFlag>(_playerEntity));
            var flag = EntityManager.GetComponentData<AntiCheatFlag>(_playerEntity);
            Assert.IsTrue(flag.IsSuspicious);
            Assert.AreEqual("Invalid values detected", flag.Reason);
        }
        
        /// <summary>
        /// Тест обнаружения бесконечных значений
        /// </summary>
        [Test]
        public void DetectInfiniteValues_WithInfinity_ShouldFlagAsSuspicious()
        {
            // Arrange
            var networkPosition = new NetworkPosition
            {
                Value = new float3(float.PositiveInfinity, 0f, 0f), // Бесконечное значение
                LastUpdateTime = (float)Time.time,
                HasChanged = true
            };
            
            EntityManager.SetComponentData(_playerEntity, networkPosition);
            
            // Act
            _antiCheatSystem.Update();
            
            // Assert
            Assert.IsTrue(EntityManager.HasComponent<AntiCheatFlag>(_playerEntity));
            var flag = EntityManager.GetComponentData<AntiCheatFlag>(_playerEntity);
            Assert.IsTrue(flag.IsSuspicious);
            Assert.AreEqual("Infinite values detected", flag.Reason);
        }
        
        #endregion
        
        #region Тесты производительности античита
        
        /// <summary>
        /// Тест производительности античита для большого количества игроков
        /// </summary>
        [Test]
        public void AntiCheat_LargeNumberOfPlayers_ShouldCompleteInReasonableTime()
        {
            // Arrange
            const int playerCount = 1000;
            var players = new Entity[playerCount];
            
            for (int i = 0; i < playerCount; i++)
            {
                players[i] = CreatePlayerEntity();
                var networkPosition = new NetworkPosition
                {
                    Value = new float3(i, 0f, i),
                    Velocity = new float3(1f, 0f, 1f),
                    LastUpdateTime = (float)Time.time,
                    HasChanged = true
                };
                
                EntityManager.SetComponentData(players[i], networkPosition);
            }
            
            // Act
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            _antiCheatSystem.Update();
            stopwatch.Stop();
            
            // Assert
            Assert.IsTrue(stopwatch.ElapsedMilliseconds < 200, 
                $"Античит для {playerCount} игроков занял {stopwatch.ElapsedMilliseconds}мс, что превышает 200мс");
        }
        
        /// <summary>
        /// Тест производительности с частыми проверками
        /// </summary>
        [Test]
        public void AntiCheat_FrequentChecks_ShouldMaintainPerformance()
        {
            // Arrange
            var player = CreatePlayerEntity();
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            
            // Act - выполняем множество проверок
            for (int i = 0; i < 100; i++)
            {
                var networkPosition = new NetworkPosition
                {
                    Value = new float3(i, 0f, i),
                    Velocity = new float3(1f, 0f, 1f),
                    LastUpdateTime = (float)Time.time,
                    HasChanged = true
                };
                
                EntityManager.SetComponentData(player, networkPosition);
                _antiCheatSystem.Update();
            }
            
            stopwatch.Stop();
            
            // Assert
            Assert.IsTrue(stopwatch.ElapsedMilliseconds < 500, 
                $"100 проверок античита заняли {stopwatch.ElapsedMilliseconds}мс, что превышает 500мс");
        }
        
        #endregion
        
        #region Тесты ложных срабатываний
        
        /// <summary>
        /// Тест отсутствия ложных срабатываний для легитимного игрока
        /// </summary>
        [Test]
        public void AntiCheat_LegitimatePlayer_ShouldNotFlag()
        {
            // Arrange
            var networkPosition = new NetworkPosition
            {
                Value = new float3(0f, 0f, 0f),
                Velocity = new float3(5f, 0f, 3f), // Нормальная скорость
                LastUpdateTime = (float)Time.time,
                HasChanged = true
            };
            
            EntityManager.SetComponentData(_playerEntity, networkPosition);
            
            // Act
            _antiCheatSystem.Update();
            
            // Assert
            Assert.IsFalse(EntityManager.HasComponent<AntiCheatFlag>(_playerEntity));
        }
        
        /// <summary>
        /// Тест отсутствия ложных срабатываний для нормального ввода
        /// </summary>
        [Test]
        public void AntiCheat_NormalInput_ShouldNotFlag()
        {
            // Arrange
            var playerInput = new PlayerInput
            {
                VehicleMovement = new float2(0.5f, 0.3f), // Нормальный ввод
                Accelerate = true,
                Brake = false,
                Steering = 0.2f
            };
            
            EntityManager.SetComponentData(_playerEntity, playerInput);
            
            // Act
            _antiCheatSystem.Update();
            
            // Assert
            Assert.IsFalse(EntityManager.HasComponent<AntiCheatFlag>(_playerEntity));
        }
        
        #endregion
        
        #region Вспомогательные методы
        
        /// <summary>
        /// Создает сущность игрока
        /// </summary>
        private Entity CreatePlayerEntity()
        {
            var entity = EntityManager.CreateEntity();
            EntityManager.AddComponent<NetworkId>(entity);
            EntityManager.AddComponent<NetworkPosition>(entity);
            EntityManager.AddComponent<LocalTransform>(entity);
            EntityManager.AddComponent<PlayerTag>(entity);
            EntityManager.AddComponent<PlayerInput>(entity);
            return entity;
        }
        
        /// <summary>
        /// Создает подозрительную сущность
        /// </summary>
        private Entity CreateSuspiciousEntity()
        {
            var entity = CreatePlayerEntity();
            EntityManager.AddComponent<AntiCheatFlag>(entity);
            return entity;
        }
        
        #endregion
    }
    
    /// <summary>
    /// Флаг античита для тестирования
    /// </summary>
    public struct AntiCheatFlag : IComponentData
    {
        public bool IsSuspicious;
        public string Reason;
    }
}
