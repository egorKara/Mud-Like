using NUnit.Framework;
using Unity.Mathematics;
using MudLike.Core.Components;

namespace MudLike.Tests.Unit
{
    /// <summary>
    /// Unit тесты для компонента PlayerInput
    /// </summary>
    public class PlayerInputTests
    {
        [Test]
        public void PlayerInput_DefaultConstructor_InitializesCorrectly()
        {
            // Arrange & Act
            var input = new PlayerInput();
            
            // Assert
            Assert.AreEqual(float2.zero, input.Movement);
            Assert.IsFalse(input.Jump);
            Assert.IsFalse(input.Brake);
        }
        
        [Test]
        public void PlayerInput_ConstructorWithValues_SetsCorrectValues()
        {
            // Arrange
            var movement = new float2(1, 2);
            var jump = true;
            var brake = false;
            
            // Act
            var input = new PlayerInput
            {
                Movement = movement,
                Jump = jump,
                Brake = brake
            };
            
            // Assert
            Assert.AreEqual(movement, input.Movement);
            Assert.AreEqual(jump, input.Jump);
            Assert.AreEqual(brake, input.Brake);
        }
        
        [Test]
        public void PlayerInput_MovementAssignment_UpdatesCorrectly()
        {
            // Arrange
            var input = new PlayerInput();
            var newMovement = new float2(5, 10);
            
            // Act
            input.Movement = newMovement;
            
            // Assert
            Assert.AreEqual(newMovement, input.Movement);
        }
        
        [Test]
        public void PlayerInput_JumpAssignment_UpdatesCorrectly()
        {
            // Arrange
            var input = new PlayerInput();
            
            // Act
            input.Jump = true;
            
            // Assert
            Assert.IsTrue(input.Jump);
        }
        
        [Test]
        public void PlayerInput_BrakeAssignment_UpdatesCorrectly()
        {
            // Arrange
            var input = new PlayerInput();
            
            // Act
            input.Brake = true;
            
            // Assert
            Assert.IsTrue(input.Brake);
        }
        
        [Test]
        public void PlayerInput_AllValuesAssignment_UpdatesCorrectly()
        {
            // Arrange
            var input = new PlayerInput();
            var movement = new float2(3, 4);
            var jump = true;
            var brake = true;
            
            // Act
            input.Movement = movement;
            input.Jump = jump;
            input.Brake = brake;
            
            // Assert
            Assert.AreEqual(movement, input.Movement);
            Assert.AreEqual(jump, input.Jump);
            Assert.AreEqual(brake, input.Brake);
        }
        
        [Test]
        public void PlayerInput_ZeroMovement_IsValid()
        {
            // Arrange
            var input = new PlayerInput { Movement = float2.zero };
            
            // Act & Assert
            Assert.AreEqual(float2.zero, input.Movement);
            Assert.IsTrue(math.all(input.Movement == float2.zero));
        }
        
        [Test]
        public void PlayerInput_NegativeMovement_IsValid()
        {
            // Arrange
            var negativeMovement = new float2(-1, -2);
            var input = new PlayerInput { Movement = negativeMovement };
            
            // Act & Assert
            Assert.AreEqual(negativeMovement, input.Movement);
            Assert.IsTrue(math.all(input.Movement == negativeMovement));
        }
        
        [Test]
        public void PlayerInput_JumpAndBrakeBothTrue_IsValid()
        {
            // Arrange
            var input = new PlayerInput
            {
                Jump = true,
                Brake = true
            };
            
            // Act & Assert
            Assert.IsTrue(input.Jump);
            Assert.IsTrue(input.Brake);
        }
        
        [Test]
        public void PlayerInput_JumpAndBrakeBothFalse_IsValid()
        {
            // Arrange
            var input = new PlayerInput
            {
                Jump = false,
                Brake = false
            };
            
            // Act & Assert
            Assert.IsFalse(input.Jump);
            Assert.IsFalse(input.Brake);
        }
    }
}