using NUnit.Framework;
using Unity.Mathematics;
using MudLike.Core.Components;

namespace MudLike.Tests.Unit
{
    /// <summary>
    /// Unit тесты для компонента Velocity
    /// </summary>
    public class VelocityTests
    {
        [Test]
        public void Velocity_DefaultConstructor_InitializesToZero()
        {
            // Arrange & Act
            var velocity = new Velocity();
            
            // Assert
            Assert.AreEqual(float3.zero, velocity.Value);
        }
        
        [Test]
        public void Velocity_ConstructorWithValue_SetsCorrectValue()
        {
            // Arrange
            var expectedValue = new float3(1, 2, 3);
            
            // Act
            var velocity = new Velocity { Value = expectedValue };
            
            // Assert
            Assert.AreEqual(expectedValue, velocity.Value);
        }
        
        [Test]
        public void Velocity_ValueAssignment_UpdatesCorrectly()
        {
            // Arrange
            var velocity = new Velocity();
            var newValue = new float3(5, 10, 15);
            
            // Act
            velocity.Value = newValue;
            
            // Assert
            Assert.AreEqual(newValue, velocity.Value);
        }
        
        [Test]
        public void Velocity_ValueComparison_WorksCorrectly()
        {
            // Arrange
            var velocity1 = new Velocity { Value = new float3(1, 2, 3) };
            var velocity2 = new Velocity { Value = new float3(1, 2, 3) };
            var velocity3 = new Velocity { Value = new float3(4, 5, 6) };
            
            // Act & Assert
            Assert.AreEqual(velocity1.Value, velocity2.Value);
            Assert.AreNotEqual(velocity1.Value, velocity3.Value);
        }
        
        [Test]
        public void Velocity_ZeroValue_IsValid()
        {
            // Arrange
            var velocity = new Velocity { Value = float3.zero };
            
            // Act & Assert
            Assert.AreEqual(float3.zero, velocity.Value);
            Assert.IsTrue(math.all(velocity.Value == float3.zero));
        }
        
        [Test]
        public void Velocity_NegativeValues_AreValid()
        {
            // Arrange
            var negativeValue = new float3(-1, -2, -3);
            var velocity = new Velocity { Value = negativeValue };
            
            // Act & Assert
            Assert.AreEqual(negativeValue, velocity.Value);
            Assert.IsTrue(math.all(velocity.Value == negativeValue));
        }
        
        [Test]
        public void Velocity_LargeValues_AreValid()
        {
            // Arrange
            var largeValue = new float3(1000, 2000, 3000);
            var velocity = new Velocity { Value = largeValue };
            
            // Act & Assert
            Assert.AreEqual(largeValue, velocity.Value);
            Assert.IsTrue(math.all(velocity.Value == largeValue));
        }
        
        [Test]
        public void Velocity_FloatPrecision_IsMaintained()
        {
            // Arrange
            var preciseValue = new float3(1.234567f, 2.345678f, 3.456789f);
            var velocity = new Velocity { Value = preciseValue };
            
            // Act & Assert
            Assert.AreEqual(preciseValue, velocity.Value);
            Assert.IsTrue(math.all(math.abs(velocity.Value - preciseValue) < 0.000001f));
        }
    }
}