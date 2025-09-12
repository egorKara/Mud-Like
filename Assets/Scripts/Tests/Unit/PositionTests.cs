using NUnit.Framework;
using Unity.Mathematics;
using MudLike.Core.Components;

namespace MudLike.Tests.Unit
{
    /// <summary>
    /// Unit тесты для компонента Position
    /// </summary>
    public class PositionTests
    {
        [Test]
        public void Position_DefaultConstructor_InitializesToZero()
        {
            // Arrange & Act
            var position = new Position();
            
            // Assert
            Assert.AreEqual(float3.zero, position.Value);
        }
        
        [Test]
        public void Position_ConstructorWithValue_SetsCorrectValue()
        {
            // Arrange
            var expectedValue = new float3(1, 2, 3);
            
            // Act
            var position = new Position { Value = expectedValue };
            
            // Assert
            Assert.AreEqual(expectedValue, position.Value);
        }
        
        [Test]
        public void Position_ValueAssignment_UpdatesCorrectly()
        {
            // Arrange
            var position = new Position();
            var newValue = new float3(5, 10, 15);
            
            // Act
            position.Value = newValue;
            
            // Assert
            Assert.AreEqual(newValue, position.Value);
        }
        
        [Test]
        public void Position_ValueComparison_WorksCorrectly()
        {
            // Arrange
            var position1 = new Position { Value = new float3(1, 2, 3) };
            var position2 = new Position { Value = new float3(1, 2, 3) };
            var position3 = new Position { Value = new float3(4, 5, 6) };
            
            // Act & Assert
            Assert.AreEqual(position1.Value, position2.Value);
            Assert.AreNotEqual(position1.Value, position3.Value);
        }
        
        [Test]
        public void Position_ZeroValue_IsValid()
        {
            // Arrange
            var position = new Position { Value = float3.zero };
            
            // Act & Assert
            Assert.AreEqual(float3.zero, position.Value);
            Assert.IsTrue(math.all(position.Value == float3.zero));
        }
        
        [Test]
        public void Position_NegativeValues_AreValid()
        {
            // Arrange
            var negativeValue = new float3(-1, -2, -3);
            var position = new Position { Value = negativeValue };
            
            // Act & Assert
            Assert.AreEqual(negativeValue, position.Value);
            Assert.IsTrue(math.all(position.Value == negativeValue));
        }
        
        [Test]
        public void Position_LargeValues_AreValid()
        {
            // Arrange
            var largeValue = new float3(1000, 2000, 3000);
            var position = new Position { Value = largeValue };
            
            // Act & Assert
            Assert.AreEqual(largeValue, position.Value);
            Assert.IsTrue(math.all(position.Value == largeValue));
        }
    }
}