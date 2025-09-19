using NUnit.Framework;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using MudLike.Core.Components;
using MudLike.Core.Systems;

namespace MudLike.Tests.Unit
{
    /// <summary>
    /// Unit тесты для системы PlayerMovementSystem
    /// </summary>
    public class PlayerMovementSystemTests
    {
        [Test]
        public void CalculateMovement_ZeroInput_ReturnsZero()
        {
            // Arrange
            var input = new PlayerInput { Movement = float2.zero };
            
            // Act
            var result = PlayerMovementSystem.CalculateMovement(input);
            
            // Assert
            Assert.AreEqual(float3.zero, result);
        }
        
        [Test]
        public void CalculateMovement_ForwardInput_ReturnsForwardDirection()
        {
            // Arrange
            var input = new PlayerInput { Movement = new float2(0, 1) };
            var expected = new float3(0, 0, 1) * 5f; // Нормализованное направление * скорость
            
            // Act
            var result = PlayerMovementSystem.CalculateMovement(input);
            
            // Assert
            Assert.AreEqual(expected, result);
        }
        
        [Test]
        public void CalculateMovement_BackwardInput_ReturnsBackwardDirection()
        {
            // Arrange
            var input = new PlayerInput { Movement = new float2(0, -1) };
            var expected = new float3(0, 0, -1) * 5f;
            
            // Act
            var result = PlayerMovementSystem.CalculateMovement(input);
            
            // Assert
            Assert.AreEqual(expected, result);
        }
        
        [Test]
        public void CalculateMovement_RightInput_ReturnsRightDirection()
        {
            // Arrange
            var input = new PlayerInput { Movement = new float2(1, 0) };
            var expected = new float3(1, 0, 0) * 5f;
            
            // Act
            var result = PlayerMovementSystem.CalculateMovement(input);
            
            // Assert
            Assert.AreEqual(expected, result);
        }
        
        [Test]
        public void CalculateMovement_LeftInput_ReturnsLeftDirection()
        {
            // Arrange
            var input = new PlayerInput { Movement = new float2(-1, 0) };
            var expected = new float3(-1, 0, 0) * 5f;
            
            // Act
            var result = PlayerMovementSystem.CalculateMovement(input);
            
            // Assert
            Assert.AreEqual(expected, result);
        }
        
        [Test]
        public void CalculateMovement_DiagonalInput_ReturnsNormalizedDirection()
        {
            // Arrange
            var input = new PlayerInput { Movement = new float2(1, 1) };
            var expected = math.normalize(new float3(1, 0, 1)) * 5f;
            
            // Act
            var result = PlayerMovementSystem.CalculateMovement(input);
            
            // Assert
            Assert.IsTrue(math.all(math.abs(result - expected) < 0.0001f));
        }
        
        [Test]
        public void CalculateMovement_LargeInput_ReturnsNormalizedDirection()
        {
            // Arrange
            var input = new PlayerInput { Movement = new float2(10, 10) };
            var expected = math.normalize(new float3(10, 0, 10)) * 5f;
            
            // Act
            var result = PlayerMovementSystem.CalculateMovement(input);
            
            // Assert
            Assert.IsTrue(math.all(math.abs(result - expected) < 0.0001f));
        }
        
        [Test]
        public void CalculateMovement_SmallInput_ReturnsNormalizedDirection()
        {
            // Arrange
            var input = new PlayerInput { Movement = new float2(0.1f, 0.1f) };
            var expected = math.normalize(new float3(0.1f, 0, 0.1f)) * 5f;
            
            // Act
            var result = PlayerMovementSystem.CalculateMovement(input);
            
            // Assert
            Assert.IsTrue(math.all(math.abs(result - expected) < 0.0001f));
        }
        
        [Test]
        public void CalculateMovement_SpeedIsCorrect()
        {
            // Arrange
            var input = new PlayerInput { Movement = new float2(1, 0) };
            var expectedSpeed = 5f;
            
            // Act
            var result = PlayerMovementSystem.CalculateMovement(input);
            
            // Assert
            Assert.AreEqual(expectedSpeed, math.length(result));
        }
        
        [Test]
        public void CalculateMovement_YComponentIsZero()
        {
            // Arrange
            var input = new PlayerInput { Movement = new float2(1, 1) };
            
            // Act
            var result = PlayerMovementSystem.CalculateMovement(input);
            
            // Assert
            Assert.AreEqual(0f, result.y);
        }
        
        [Test]
        public void ProcessMovement_ValidInput_MovesTransform()
        {
            // Arrange
            var transform = new LocalTransform
            {
                Position = float3.zero,
                Rotation = quaternion.identity,
                Scale = 1f
            };
            var input = new PlayerInput { Movement = new float2(1, 0) };
            var deltaTime = 1f;
            var expectedPosition = new float3(5, 0, 0); // 1 * 5f * 1f
            
            // Act
            PlayerMovementSystem.ProcessMovement(ref transform, input, deltaTime);
            
            // Assert
            Assert.AreEqual(expectedPosition, transform.Position);
        }
        
        [Test]
        public void ProcessMovement_ZeroDeltaTime_DoesNotMove()
        {
            // Arrange
            var transform = new LocalTransform
            {
                Position = float3.zero,
                Rotation = quaternion.identity,
                Scale = 1f
            };
            var input = new PlayerInput { Movement = new float2(1, 0) };
            var deltaTime = 0f;
            
            // Act
            PlayerMovementSystem.ProcessMovement(ref transform, input, deltaTime);
            
            // Assert
            Assert.AreEqual(float3.zero, transform.Position);
        }
        
        [Test]
        public void ProcessMovement_HalfDeltaTime_MovesHalfDistance()
        {
            // Arrange
            var transform = new LocalTransform
            {
                Position = float3.zero,
                Rotation = quaternion.identity,
                Scale = 1f
            };
            var input = new PlayerInput { Movement = new float2(1, 0) };
            var deltaTime = 0.5f;
            var expectedPosition = new float3(2.5f, 0, 0); // 1 * 5f * 0.5f
            
            // Act
            PlayerMovementSystem.ProcessMovement(ref transform, input, deltaTime);
            
            // Assert
            Assert.AreEqual(expectedPosition, transform.Position);
        }
    }
}