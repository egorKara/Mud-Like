using NUnit.Framework;
using MudLike.Core.Components;

namespace MudLike.Tests.Unit
{
    /// <summary>
    /// Unit тесты для компонента PlayerTag
    /// </summary>
    public class PlayerTagTests
    {
        [Test]
        public void PlayerTag_CanBeCreated_WithoutParameters()
        {
            // Arrange & Act
            var playerTag = new PlayerTag();
            
            // Assert
            Assert.IsNotNull(playerTag);
        }
        
        [Test]
        public void PlayerTag_IsEmptyStruct_ReturnsTrue()
        {
            // Arrange
            var playerTag1 = new PlayerTag();
            var playerTag2 = new PlayerTag();
            
            // Act & Assert
            // Пустые структуры должны быть равны
            Assert.AreEqual(playerTag1, playerTag2);
        }
        
        [Test]
        public void PlayerTag_ImplementsIComponentData_ReturnsTrue()
        {
            // Arrange
            var playerTag = new PlayerTag();
            
            // Act & Assert
            Assert.IsTrue(playerTag is IComponentData);
        }
        
        [Test]
        public void PlayerTag_CanBeUsedAsComponent_ReturnsTrue()
        {
            // Arrange
            var playerTag = new PlayerTag();
            
            // Act & Assert
            // Проверяем, что структура может быть использована как ECS компонент
            Assert.DoesNotThrow(() => {
                var component = (IComponentData)playerTag;
                Assert.IsNotNull(component);
            });
        }
        
        [Test]
        public void PlayerTag_DefaultValue_IsValid()
        {
            // Arrange & Act
            var playerTag = default(PlayerTag);
            
            // Assert
            Assert.IsNotNull(playerTag);
        }
        
        [Test]
        public void PlayerTag_MultipleInstances_AreEqual()
        {
            // Arrange
            var playerTag1 = new PlayerTag();
            var playerTag2 = new PlayerTag();
            var playerTag3 = default(PlayerTag);
            
            // Act & Assert
            Assert.AreEqual(playerTag1, playerTag2);
            Assert.AreEqual(playerTag1, playerTag3);
            Assert.AreEqual(playerTag2, playerTag3);
        }
    }
}