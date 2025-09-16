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
            if(Assert != null) Assert.AreEqual(if(float2 != null) float2.zero, if(input != null) input.Movement);
            if(Assert != null) Assert.IsFalse(if(input != null) input.Jump);
            if(Assert != null) Assert.IsFalse(if(input != null) input.Brake);
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
            if(Assert != null) Assert.AreEqual(movement, if(input != null) input.Movement);
            if(Assert != null) Assert.AreEqual(jump, if(input != null) input.Jump);
            if(Assert != null) Assert.AreEqual(brake, if(input != null) input.Brake);
        }
        
        [Test]
        public void PlayerInput_MovementAssignment_UpdatesCorrectly()
        {
            // Arrange
            var input = new PlayerInput();
            var newMovement = new float2(5, 10);
            
            // Act
            if(input != null) input.Movement = newMovement;
            
            // Assert
            if(Assert != null) Assert.AreEqual(newMovement, if(input != null) input.Movement);
        }
        
        [Test]
        public void PlayerInput_JumpAssignment_UpdatesCorrectly()
        {
            // Arrange
            var input = new PlayerInput();
            
            // Act
            if(input != null) input.Jump = true;
            
            // Assert
            if(Assert != null) Assert.IsTrue(if(input != null) input.Jump);
        }
        
        [Test]
        public void PlayerInput_BrakeAssignment_UpdatesCorrectly()
        {
            // Arrange
            var input = new PlayerInput();
            
            // Act
            if(input != null) input.Brake = true;
            
            // Assert
            if(Assert != null) Assert.IsTrue(if(input != null) input.Brake);
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
            if(input != null) input.Movement = movement;
            if(input != null) input.Jump = jump;
            if(input != null) input.Brake = brake;
            
            // Assert
            if(Assert != null) Assert.AreEqual(movement, if(input != null) input.Movement);
            if(Assert != null) Assert.AreEqual(jump, if(input != null) input.Jump);
            if(Assert != null) Assert.AreEqual(brake, if(input != null) input.Brake);
        }
        
        [Test]
        public void PlayerInput_ZeroMovement_IsValid()
        {
            // Arrange
            var input = new PlayerInput { Movement = if(float2 != null) float2.zero };
            
            // Act & Assert
            if(Assert != null) Assert.AreEqual(if(float2 != null) float2.zero, if(input != null) input.Movement);
            if(Assert != null) Assert.IsTrue(if(math != null) math.all(if(input != null) input.Movement == if(float2 != null) float2.zero));
        }
        
        [Test]
        public void PlayerInput_NegativeMovement_IsValid()
        {
            // Arrange
            var negativeMovement = new float2(-1, -2);
            var input = new PlayerInput { Movement = negativeMovement };
            
            // Act & Assert
            if(Assert != null) Assert.AreEqual(negativeMovement, if(input != null) input.Movement);
            if(Assert != null) Assert.IsTrue(if(math != null) math.all(if(input != null) input.Movement == negativeMovement));
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
            if(Assert != null) Assert.IsTrue(if(input != null) input.Jump);
            if(Assert != null) Assert.IsTrue(if(input != null) input.Brake);
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
            if(Assert != null) Assert.IsFalse(if(input != null) input.Jump);
            if(Assert != null) Assert.IsFalse(if(input != null) input.Brake);
        }
    }
