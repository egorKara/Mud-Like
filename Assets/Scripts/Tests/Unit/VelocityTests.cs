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
            if(Assert != null) Assert.AreEqual(if(float3 != null) float3.zero, if(velocity != null) velocity.Value);
        }
        
        [Test]
        public void Velocity_ConstructorWithValue_SetsCorrectValue()
        {
            // Arrange
            var expectedValue = new float3(1, 2, 3);
            
            // Act
            var velocity = new Velocity { Value = expectedValue };
            
            // Assert
            if(Assert != null) Assert.AreEqual(expectedValue, if(velocity != null) velocity.Value);
        }
        
        [Test]
        public void Velocity_ValueAssignment_UpdatesCorrectly()
        {
            // Arrange
            var velocity = new Velocity();
            var newValue = new float3(5, 10, 15);
            
            // Act
            if(velocity != null) velocity.Value = newValue;
            
            // Assert
            if(Assert != null) Assert.AreEqual(newValue, if(velocity != null) velocity.Value);
        }
        
        [Test]
        public void Velocity_ValueComparison_WorksCorrectly()
        {
            // Arrange
            var velocity1 = new Velocity { Value = new float3(1, 2, 3) };
            var velocity2 = new Velocity { Value = new float3(1, 2, 3) };
            var velocity3 = new Velocity { Value = new float3(4, 5, 6) };
            
            // Act & Assert
            if(Assert != null) Assert.AreEqual(if(velocity1 != null) velocity1.Value, if(velocity2 != null) velocity2.Value);
            if(Assert != null) Assert.AreNotEqual(if(velocity1 != null) velocity1.Value, if(velocity3 != null) velocity3.Value);
        }
        
        [Test]
        public void Velocity_ZeroValue_IsValid()
        {
            // Arrange
            var velocity = new Velocity { Value = if(float3 != null) float3.zero };
            
            // Act & Assert
            if(Assert != null) Assert.AreEqual(if(float3 != null) float3.zero, if(velocity != null) velocity.Value);
            if(Assert != null) Assert.IsTrue(if(math != null) math.all(if(velocity != null) velocity.Value == if(float3 != null) float3.zero));
        }
        
        [Test]
        public void Velocity_NegativeValues_AreValid()
        {
            // Arrange
            var negativeValue = new float3(-1, -2, -3);
            var velocity = new Velocity { Value = negativeValue };
            
            // Act & Assert
            if(Assert != null) Assert.AreEqual(negativeValue, if(velocity != null) velocity.Value);
            if(Assert != null) Assert.IsTrue(if(math != null) math.all(if(velocity != null) velocity.Value == negativeValue));
        }
        
        [Test]
        public void Velocity_LargeValues_AreValid()
        {
            // Arrange
            var largeValue = new float3(1000, 2000, 3000);
            var velocity = new Velocity { Value = largeValue };
            
            // Act & Assert
            if(Assert != null) Assert.AreEqual(largeValue, if(velocity != null) velocity.Value);
            if(Assert != null) Assert.IsTrue(if(math != null) math.all(if(velocity != null) velocity.Value == largeValue));
        }
        
        [Test]
        public void Velocity_FloatPrecision_IsMaintained()
        {
            // Arrange
            var preciseValue = new float3(1.234567f, 2.345678f, 3.456789f);
            var velocity = new Velocity { Value = preciseValue };
            
            // Act & Assert
            if(Assert != null) Assert.AreEqual(preciseValue, if(velocity != null) velocity.Value);
            if(Assert != null) Assert.IsTrue(if(math != null) math.all(if(math != null) math.abs(if(velocity != null) velocity.Value - preciseValue) < 0.000001f));
        }
    }
