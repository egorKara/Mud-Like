using NUnit.Framework;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using MudLike.Networking.Components;
using MudLike.Networking.Systems;
using MudLike.Vehicles.Components;
using MudLike.Tests.Infrastructure;

namespace MudLike.Tests.Unit.Networking
{
    /// <summary>
    /// Unit тесты для InputValidationSystem
    /// </summary>
    [TestFixture]
    public class InputValidationSystemTests : MudLikeTestFixture
    {
        private InputValidationSystem _system;
        private Entity _playerEntity;
        
        [SetUp]
        public override void Setup()
        {
            base.Setup();
            _system = World.CreateSystemManaged<InputValidationSystem>();
            
            // Создаем тестового игрока
            _playerEntity = CreatePlayer();
        }
        
        [Test]
        public void InputValidationSystem_ShouldAcceptValidInput()
        {
            // Arrange
            var input = new VehicleInput
            {
                Vertical = 1.0f,
                Horizontal = 0.5f,
                Brake = false,
                Handbrake = false
            };
            
            EntityManager.SetComponentData(_playerEntity, input);
            
            // Act
            _system.Update();
            
            // Assert
            var validatedInput = EntityManager.GetComponentData<VehicleInput>(_playerEntity);
            Assert.AreEqual(1.0f, validatedInput.Vertical);
            Assert.AreEqual(0.5f, validatedInput.Horizontal);
        }
        
        [Test]
        public void InputValidationSystem_ShouldClampInputValues()
        {
            // Arrange
            var input = new VehicleInput
            {
                Vertical = 2.0f,    // Превышает максимум
                Horizontal = -2.0f, // Превышает минимум
                Brake = false,
                Handbrake = false
            };
            
            EntityManager.SetComponentData(_playerEntity, input);
            
            // Act
            _system.Update();
            
            // Assert
            var validatedInput = EntityManager.GetComponentData<VehicleInput>(_playerEntity);
            Assert.LessOrEqual(validatedInput.Vertical, 1.0f);
            Assert.GreaterOrEqual(validatedInput.Vertical, -1.0f);
            Assert.LessOrEqual(validatedInput.Horizontal, 1.0f);
            Assert.GreaterOrEqual(validatedInput.Horizontal, -1.0f);
        }
        
        [Test]
        public void InputValidationSystem_ShouldDetectRapidInputChanges()
        {
            // Arrange
            var input = new VehicleInput
            {
                Vertical = 1.0f,
                Horizontal = 0.0f,
                Brake = false,
                Handbrake = false
            };
            
            EntityManager.SetComponentData(_playerEntity, input);
            
            // Первое обновление
            _system.Update();
            
            // Резкое изменение ввода (возможный читерский ввод)
            input.Vertical = -1.0f;
            input.Horizontal = 1.0f;
            EntityManager.SetComponentData(_playerEntity, input);
            
            // Act
            _system.Update();
            
            // Assert
            var networkData = EntityManager.GetComponentData<NetworkData>(_playerEntity);
            Assert.IsTrue(networkData.SuspiciousInput);
        }
        
        [Test]
        public void InputValidationSystem_ShouldDetectImpossibleInputs()
        {
            // Arrange
            var input = new VehicleInput
            {
                Vertical = 1.0f,
                Horizontal = 1.0f,
                Brake = true,    // Одновременно газ и тормоз
                Handbrake = true
            };
            
            EntityManager.SetComponentData(_playerEntity, input);
            
            // Act
            _system.Update();
            
            // Assert
            var networkData = EntityManager.GetComponentData<NetworkData>(_playerEntity);
            Assert.IsTrue(networkData.InvalidInput);
        }
        
        [Test]
        public void InputValidationSystem_ShouldApplyInputSmoothing()
        {
            // Arrange
            var input = new VehicleInput
            {
                Vertical = 0.0f,
                Horizontal = 0.0f,
                Brake = false,
                Handbrake = false
            };
            
            EntityManager.SetComponentData(_playerEntity, input);
            
            // Резкое изменение ввода
            input.Vertical = 1.0f;
            EntityManager.SetComponentData(_playerEntity, input);
            
            // Act
            _system.Update();
            
            // Assert
            var validatedInput = EntityManager.GetComponentData<VehicleInput>(_playerEntity);
            Assert.Less(validatedInput.Vertical, 1.0f); // Должно быть сглажено
        }
        
        [Test]
        public void InputValidationSystem_ShouldTrackInputHistory()
        {
            // Arrange
            var input = new VehicleInput
            {
                Vertical = 0.0f,
                Horizontal = 0.0f,
                Brake = false,
                Handbrake = false
            };
            
            EntityManager.SetComponentData(_playerEntity, input);
            
            // Act - несколько обновлений
            for (int i = 0; i < 10; i++)
            {
                input.Vertical = (i % 2) == 0 ? 1.0f : 0.0f;
                EntityManager.SetComponentData(_playerEntity, input);
                _system.Update();
            }
            
            // Assert
            var networkData = EntityManager.GetComponentData<NetworkData>(_playerEntity);
            Assert.Greater(networkData.InputHistoryCount, 0);
        }
        
        [Test]
        public void InputValidationSystem_ShouldDetectBotInput()
        {
            // Arrange
            var input = new VehicleInput
            {
                Vertical = 1.0f,
                Horizontal = 0.0f,
                Brake = false,
                Handbrake = false
            };
            
            EntityManager.SetComponentData(_playerEntity, input);
            
            // Симулируем бот-ввод (идеально повторяющийся паттерн)
            for (int i = 0; i < 100; i++)
            {
                input.Vertical = 1.0f;
                input.Horizontal = 0.0f;
                EntityManager.SetComponentData(_playerEntity, input);
                _system.Update();
            }
            
            // Assert
            var networkData = EntityManager.GetComponentData<NetworkData>(_playerEntity);
            Assert.IsTrue(networkData.SuspectedBot);
        }
        
        [Test]
        public void InputValidationSystem_ShouldHandleNetworkLatency()
        {
            // Arrange
            var input = new VehicleInput
            {
                Vertical = 1.0f,
                Horizontal = 0.0f,
                Brake = false,
                Handbrake = false
            };
            
            EntityManager.SetComponentData(_playerEntity, input);
            
            // Симулируем высокую задержку
            var networkData = EntityManager.GetComponentData<NetworkData>(_playerEntity);
            networkData.Ping = 500; // 500ms задержка
            EntityManager.SetComponentData(_playerEntity, networkData);
            
            // Act
            _system.Update();
            
            // Assert
            var updatedNetworkData = EntityManager.GetComponentData<NetworkData>(_playerEntity);
            Assert.Greater(updatedNetworkData.CompensationFactor, 1.0f);
        }
        
        [Test]
        public void InputValidationSystem_ShouldRunWithoutErrors()
        {
            // Arrange
            var input = new VehicleInput
            {
                Vertical = 0.0f,
                Horizontal = 0.0f,
                Brake = false,
                Handbrake = false
            };
            
            EntityManager.SetComponentData(_playerEntity, input);
            
            // Act & Assert
            Assert.DoesNotThrow(() => _system.Update());
        }
        
        [Test]
        public void InputValidationSystem_ShouldMeetPerformanceRequirements()
        {
            // Arrange
            for (int i = 0; i < 1000; i++)
            {
                var player = CreatePlayer();
                var input = new VehicleInput
                {
                    Vertical = UnityEngine.Random.Range(-1f, 1f),
                    Horizontal = UnityEngine.Random.Range(-1f, 1f),
                    Brake = UnityEngine.Random.value > 0.5f,
                    Handbrake = UnityEngine.Random.value > 0.8f
                };
                EntityManager.SetComponentData(player, input);
            }
            
            // Act & Assert
            AssertSystemPerformance<InputValidationSystem>(16.67f); // 60 FPS
        }
        
        private Entity CreatePlayer()
        {
            var entity = EntityManager.CreateEntity();
            EntityManager.AddComponentData(entity, new PlayerTag());
            EntityManager.AddComponentData(entity, new NetworkData
            {
                NetworkId = UnityEngine.Random.Range(1, 10000),
                Ping = 50,
                LastUpdateTime = 0f,
                SuspiciousInput = false,
                InvalidInput = false,
                SuspectedBot = false,
                InputHistoryCount = 0,
                CompensationFactor = 1.0f
            });
            EntityManager.AddComponentData(entity, new VehicleInput
            {
                Vertical = 0f,
                Horizontal = 0f,
                Brake = false,
                Handbrake = false
            });
            EntityManager.AddComponentData(entity, new LocalTransform
            {
                Position = new float3(0, 0, 0),
                Rotation = quaternion.identity
            });
            return entity;
        }
    }
}