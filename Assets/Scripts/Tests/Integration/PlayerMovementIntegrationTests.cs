using NUnit.Framework;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using MudLike.Core.Components;
using MudLike.Core.Systems;

namespace MudLike.Tests.Integration
{
    /// <summary>
    /// Integration тесты для системы движения игрока
    /// </summary>
    public class PlayerMovementIntegrationTests
    {
        private World _world;
        private EntityManager _entityManager;
        private PlayerMovementSystem _movementSystem;

        [SetUp]
        public void Setup()
        {
            // Создаем тестовый мир
            _world = new World("TestWorld");
            _entityManager = _world.EntityManager;
            
            // Создаем систему движения
            _movementSystem = _world.GetOrCreateSystemManaged<PlayerMovementSystem>();
        }

        [TearDown]
        public void TearDown()
        {
            // Очищаем тестовый мир
            _world?.Dispose();
        }

        [Test]
        public void PlayerMovement_WithPlayerEntity_MovesCorrectly()
        {
            // Arrange
            var entity = _entityManager.CreateEntity();
            _entityManager.AddComponent<PlayerTag>(entity);
            _entityManager.AddComponent<PlayerInput>(entity);
            _entityManager.AddComponent<LocalTransform>(entity);
            
            // Устанавливаем начальную позицию
            var transform = new LocalTransform
            {
                Position = new float3(0, 0, 0),
                Rotation = quaternion.identity,
                Scale = 1f
            };
            _entityManager.SetComponentData(entity, transform);
            
            // Устанавливаем ввод
            var input = new PlayerInput
            {
                Movement = new float2(1, 0),
                Jump = false,
                Brake = false
            };
            _entityManager.SetComponentData(entity, input);

            // Act
            _movementSystem.Update();

            // Assert
            var updatedTransform = _entityManager.GetComponentData<LocalTransform>(entity);
            Assert.Greater(updatedTransform.Position.x, 0f); // Должен сдвинуться вправо
        }

        [Test]
        public void PlayerMovement_MultiplePlayers_MoveIndependently()
        {
            // Arrange
            var entity1 = CreatePlayerEntity(new float3(0, 0, 0), new float2(1, 0));
            var entity2 = CreatePlayerEntity(new float3(0, 0, 5), new float2(0, 1));

            // Act
            _movementSystem.Update();

            // Assert
            var transform1 = _entityManager.GetComponentData<LocalTransform>(entity1);
            var transform2 = _entityManager.GetComponentData<LocalTransform>(entity2);
            
            Assert.Greater(transform1.Position.x, 0f); // Первый игрок движется вправо
            Assert.Greater(transform2.Position.z, 5f); // Второй игрок движется вперед
        }

        [Test]
        public void PlayerMovement_NoInput_DoesNotMove()
        {
            // Arrange
            var entity = CreatePlayerEntity(new float3(0, 0, 0), new float2(0, 0));

            // Act
            _movementSystem.Update();

            // Assert
            var transform = _entityManager.GetComponentData<LocalTransform>(entity);
            Assert.AreEqual(0f, transform.Position.x, 0.001f);
            Assert.AreEqual(0f, transform.Position.y, 0.001f);
            Assert.AreEqual(0f, transform.Position.z, 0.001f);
        }

        [Test]
        public void PlayerMovement_JumpInput_DoesNotAffectMovement()
        {
            // Arrange
            var entity = CreatePlayerEntity(new float3(0, 0, 0), new float2(1, 0));
            
            // Устанавливаем ввод с прыжком
            var input = new PlayerInput
            {
                Movement = new float2(1, 0),
                Jump = true,
                Brake = false
            };
            _entityManager.SetComponentData(entity, input);

            // Act
            _movementSystem.Update();

            // Assert
            var transform = _entityManager.GetComponentData<LocalTransform>(entity);
            Assert.Greater(transform.Position.x, 0f); // Должен двигаться вправо
            Assert.AreEqual(0f, transform.Position.y, 0.001f); // Y не должен измениться (прыжок не реализован)
        }

        [Test]
        public void PlayerMovement_BrakeInput_DoesNotAffectMovement()
        {
            // Arrange
            var entity = CreatePlayerEntity(new float3(0, 0, 0), new float2(1, 0));
            
            // Устанавливаем ввод с торможением
            var input = new PlayerInput
            {
                Movement = new float2(1, 0),
                Jump = false,
                Brake = true
            };
            _entityManager.SetComponentData(entity, input);

            // Act
            _movementSystem.Update();

            // Assert
            var transform = _entityManager.GetComponentData<LocalTransform>(entity);
            Assert.Greater(transform.Position.x, 0f); // Должен двигаться вправо
            // Торможение пока не реализовано, поэтому движение должно быть обычным
        }

        /// <summary>
        /// Создает сущность игрока с заданными параметрами
        /// </summary>
        private Entity CreatePlayerEntity(float3 position, float2 movement)
        {
            var entity = _entityManager.CreateEntity();
            _entityManager.AddComponent<PlayerTag>(entity);
            _entityManager.AddComponent<PlayerInput>(entity);
            _entityManager.AddComponent<LocalTransform>(entity);
            
            // Устанавливаем позицию
            var transform = new LocalTransform
            {
                Position = position,
                Rotation = quaternion.identity,
                Scale = 1f
            };
            _entityManager.SetComponentData(entity, transform);
            
            // Устанавливаем ввод
            var input = new PlayerInput
            {
                Movement = movement,
                Jump = false,
                Brake = false
            };
            _entityManager.SetComponentData(entity, input);
            
            return entity;
        }
    }
}