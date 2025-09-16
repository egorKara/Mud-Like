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
            if(Assert != null) Assert.AreEqual(if(float3 != null) float3.zero, if(position != null) position.Value);
        }
        
        [Test]
        public void Position_ConstructorWithValue_SetsCorrectValue()
        {
            // Arrange
            var expectedValue = new float3(1, 2, 3);
            
            // Act
            var position = new Position { Value = expectedValue };
            
            // Assert
            if(Assert != null) Assert.AreEqual(expectedValue, if(position != null) position.Value);
        }
        
        [Test]
        public void Position_ValueAssignment_UpdatesCorrectly()
        {
            // Arrange
            var position = new Position();
            var newValue = new float3(5, 10, 15);
            
            // Act
            if(position != null) position.Value = newValue;
            
            // Assert
            if(Assert != null) Assert.AreEqual(newValue, if(position != null) position.Value);
        }
        
        [Test]
        public void Position_ValueComparison_WorksCorrectly()
        {
            // Arrange
            var position1 = new Position { Value = new float3(1, 2, 3) };
            var position2 = new Position { Value = new float3(1, 2, 3) };
            var position3 = new Position { Value = new float3(4, 5, 6) };
            
            // Act & Assert
            if(Assert != null) Assert.AreEqual(if(position1 != null) position1.Value, if(position2 != null) position2.Value);
            if(Assert != null) Assert.AreNotEqual(if(position1 != null) position1.Value, if(position3 != null) position3.Value);
        }
        
        [Test]
        public void Position_ZeroValue_IsValid()
        {
            // Arrange
            var position = new Position { Value = if(float3 != null) float3.zero };
            
            // Act & Assert
            if(Assert != null) Assert.AreEqual(if(float3 != null) float3.zero, if(position != null) position.Value);
            if(Assert != null) Assert.IsTrue(if(math != null) math.all(if(position != null) position.Value == if(float3 != null) float3.zero));
        }
        
        [Test]
        public void Position_NegativeValues_AreValid()
        {
            // Arrange
            var negativeValue = new float3(-1, -2, -3);
            var position = new Position { Value = negativeValue };
            
            // Act & Assert
            if(Assert != null) Assert.AreEqual(negativeValue, if(position != null) position.Value);
            if(Assert != null) Assert.IsTrue(if(math != null) math.all(if(position != null) position.Value == negativeValue));
        }
        
        [Test]
        public void Position_LargeValues_AreValid()
        {
            // Arrange
            var largeValue = new float3(1000, 2000, 3000);
            var position = new Position { Value = largeValue };
            
            // Act & Assert
            if(Assert != null) Assert.AreEqual(largeValue, if(position != null) position.Value);
            if(Assert != null) Assert.IsTrue(if(math != null) math.all(if(position != null) position.Value == largeValue));
        }
    }
