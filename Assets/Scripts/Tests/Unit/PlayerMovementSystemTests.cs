using NUnit.Framework;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using MudLike.Core.Components;
using MudLike.Core.Systems;

namespace MudLike.Tests.Unit
{
    /// <summary>
    /// Unit тесты для системы движения игрока
    /// </summary>
    public class PlayerMovementSystemTests
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
        public void CalculateMovement_ValidInput_ReturnsCorrectDirection()
        {
            // Arrange
            var input = new PlayerInput
            {
                Movement = new float2(1, 0), // Движение вправо
                Jump = false,
                Brake = false
            };

            // Act
            var result = PlayerMovementSystem.CalculateMovement(input);

            // Assert
            Assert.AreEqual(5f, math.length(result)); // Скорость должна быть 5
            Assert.AreEqual(1f, result.x, 0.001f); // X компонент должен быть 1
            Assert.AreEqual(0f, result.y, 0.001f); // Y компонент должен быть 0
            Assert.AreEqual(0f, result.z, 0.001f); // Z компонент должен быть 0
        }

        [Test]
        public void CalculateMovement_DiagonalInput_ReturnsNormalizedDirection()
        {
            // Arrange
            var input = new PlayerInput
            {
                Movement = new float2(1, 1), // Диагональное движение
                Jump = false,
                Brake = false
            };

            // Act
            var result = PlayerMovementSystem.CalculateMovement(input);

            // Assert
            Assert.AreEqual(5f, math.length(result), 0.001f); // Скорость должна быть 5
            // Проверяем, что направление нормализовано
            var normalizedDirection = math.normalize(new float3(1, 0, 1));
            Assert.AreEqual(normalizedDirection.x, result.x / 5f, 0.001f);
            Assert.AreEqual(normalizedDirection.z, result.z / 5f, 0.001f);
        }

        [Test]
        public void CalculateMovement_ZeroInput_ReturnsZero()
        {
            // Arrange
            var input = new PlayerInput
            {
                Movement = new float2(0, 0), // Нет движения
                Jump = false,
                Brake = false
            };

            // Act
            var result = PlayerMovementSystem.CalculateMovement(input);

            // Assert
            Assert.AreEqual(0f, result.x, 0.001f);
            Assert.AreEqual(0f, result.y, 0.001f);
            Assert.AreEqual(0f, result.z, 0.001f);
        }

        [Test]
        public void ProcessMovement_ValidInput_UpdatesPosition()
        {
            // Arrange
            var transform = new LocalTransform
            {
                Position = new float3(0, 0, 0),
                Rotation = quaternion.identity,
                Scale = 1f
            };
            
            var input = new PlayerInput
            {
                Movement = new float2(1, 0),
                Jump = false,
                Brake = false
            };
            
            float deltaTime = 1f; // 1 секунда

            // Act
            PlayerMovementSystem.ProcessMovement(ref transform, input, deltaTime);

            // Assert
            Assert.AreEqual(5f, transform.Position.x, 0.001f); // Должен сдвинуться на 5 единиц
            Assert.AreEqual(0f, transform.Position.y, 0.001f);
            Assert.AreEqual(0f, transform.Position.z, 0.001f);
        }

        [Test]
        public void ProcessMovement_ZeroDeltaTime_DoesNotChangePosition()
        {
            // Arrange
            var transform = new LocalTransform
            {
                Position = new float3(0, 0, 0),
                Rotation = quaternion.identity,
                Scale = 1f
            };
            
            var input = new PlayerInput
            {
                Movement = new float2(1, 0),
                Jump = false,
                Brake = false
            };
            
            float deltaTime = 0f; // Нет времени

            // Act
            PlayerMovementSystem.ProcessMovement(ref transform, input, deltaTime);

            // Assert
            Assert.AreEqual(0f, transform.Position.x, 0.001f);
            Assert.AreEqual(0f, transform.Position.y, 0.001f);
            Assert.AreEqual(0f, transform.Position.z, 0.001f);
        }
    }
}