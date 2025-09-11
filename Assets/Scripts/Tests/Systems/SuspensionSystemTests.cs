using NUnit.Framework;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using MudLike.Vehicles.Components;
using MudLike.Vehicles.Systems;

namespace MudLike.Tests.Systems
{
    /// <summary>
    /// Unit тесты для системы подвески
    /// </summary>
    [TestFixture]
    public class SuspensionSystemTests
    {
        private World _world;
        private EntityManager _entityManager;
        private SuspensionSystem _suspensionSystem;

        [SetUp]
        public void SetUp()
        {
            // Создание тестового мира
            _world = new World("TestWorld");
            _entityManager = _world.EntityManager;
            
            // Создание системы
            _suspensionSystem = _world.GetOrCreateSystemManaged<SuspensionSystem>();
        }

        [TearDown]
        public void TearDown()
        {
            // Очистка тестового мира
            _world.Dispose();
        }

        [Test]
        public void SuspensionSystem_Update_ShouldUpdateSuspensionLength()
        {
            // Arrange
            var entity = _entityManager.CreateEntity();
            var suspension = new SuspensionComponent
            {
                restLength = 1f,
                maxLength = 1.5f,
                minLength = 0.5f,
                currentLength = 1f,
                springStiffness = 1000f,
                damping = 100f,
                maxDamping = 200f,
                mountPoint = new float3(0, 0, 0),
                suspensionDirection = new float3(0, -1, 0),
                isActive = true
            };
            var transform = new LocalTransform
            {
                Position = new float3(0, 1, 0),
                Rotation = quaternion.identity,
                Scale = 1f
            };

            _entityManager.AddComponentData(entity, suspension);
            _entityManager.AddComponentData(entity, transform);

            // Act
            _suspensionSystem.Update();

            // Assert
            var updatedSuspension = _entityManager.GetComponentData<SuspensionComponent>(entity);
            Assert.GreaterOrEqual(updatedSuspension.currentLength, suspension.minLength, "Длина подвески должна быть больше или равна минимуму");
            Assert.LessOrEqual(updatedSuspension.currentLength, suspension.maxLength, "Длина подвески должна быть меньше или равна максимуму");
        }

        [Test]
        public void SuspensionSystem_Update_ShouldCalculateSpringForce()
        {
            // Arrange
            var entity = _entityManager.CreateEntity();
            var suspension = new SuspensionComponent
            {
                restLength = 1f,
                maxLength = 1.5f,
                minLength = 0.5f,
                currentLength = 0.8f, // Сжатие
                springStiffness = 1000f,
                damping = 100f,
                maxDamping = 200f,
                mountPoint = new float3(0, 0, 0),
                suspensionDirection = new float3(0, -1, 0),
                isActive = true
            };
            var transform = new LocalTransform
            {
                Position = new float3(0, 1, 0),
                Rotation = quaternion.identity,
                Scale = 1f
            };

            _entityManager.AddComponentData(entity, suspension);
            _entityManager.AddComponentData(entity, transform);

            // Act
            _suspensionSystem.Update();

            // Assert
            var updatedSuspension = _entityManager.GetComponentData<SuspensionComponent>(entity);
            Assert.Greater(updatedSuspension.springForce, 0f, "Сила пружины должна быть положительной при сжатии");
        }

        [Test]
        public void SuspensionSystem_Update_ShouldCalculateDamperForce()
        {
            // Arrange
            var entity = _entityManager.CreateEntity();
            var suspension = new SuspensionComponent
            {
                restLength = 1f,
                maxLength = 1.5f,
                minLength = 0.5f,
                currentLength = 1f,
                springStiffness = 1000f,
                damping = 100f,
                maxDamping = 200f,
                compressionVelocity = 5f, // Скорость сжатия
                mountPoint = new float3(0, 0, 0),
                suspensionDirection = new float3(0, -1, 0),
                isActive = true
            };
            var transform = new LocalTransform
            {
                Position = new float3(0, 1, 0),
                Rotation = quaternion.identity,
                Scale = 1f
            };

            _entityManager.AddComponentData(entity, suspension);
            _entityManager.AddComponentData(entity, transform);

            // Act
            _suspensionSystem.Update();

            // Assert
            var updatedSuspension = _entityManager.GetComponentData<SuspensionComponent>(entity);
            Assert.Less(updatedSuspension.damperForce, 0f, "Сила демпфера должна быть отрицательной при сжатии");
        }

        [Test]
        public void SuspensionSystem_Update_ShouldUpdateSuspensionState()
        {
            // Arrange
            var entity = _entityManager.CreateEntity();
            var suspension = new SuspensionComponent
            {
                restLength = 1f,
                maxLength = 1.5f,
                minLength = 0.5f,
                currentLength = 0.5f, // Минимальная длина
                springStiffness = 1000f,
                damping = 100f,
                maxDamping = 200f,
                mountPoint = new float3(0, 0, 0),
                suspensionDirection = new float3(0, -1, 0),
                isActive = true
            };
            var transform = new LocalTransform
            {
                Position = new float3(0, 1, 0),
                Rotation = quaternion.identity,
                Scale = 1f
            };

            _entityManager.AddComponentData(entity, suspension);
            _entityManager.AddComponentData(entity, transform);

            // Act
            _suspensionSystem.Update();

            // Assert
            var updatedSuspension = _entityManager.GetComponentData<SuspensionComponent>(entity);
            Assert.IsTrue(updatedSuspension.isCompressed, "Подвеска должна быть сжата при минимальной длине");
            Assert.AreEqual(0f, updatedSuspension.compressionProgress, "Прогресс сжатия должен быть 0 при минимальной длине");
        }

        [Test]
        public void SuspensionSystem_Update_ShouldCalculateSuspensionEnergy()
        {
            // Arrange
            var entity = _entityManager.CreateEntity();
            var suspension = new SuspensionComponent
            {
                restLength = 1f,
                maxLength = 1.5f,
                minLength = 0.5f,
                currentLength = 0.8f, // Сжатие
                springStiffness = 1000f,
                damping = 100f,
                maxDamping = 200f,
                compressionVelocity = 2f,
                mountPoint = new float3(0, 0, 0),
                suspensionDirection = new float3(0, -1, 0),
                isActive = true
            };
            var transform = new LocalTransform
            {
                Position = new float3(0, 1, 0),
                Rotation = quaternion.identity,
                Scale = 1f
            };

            _entityManager.AddComponentData(entity, suspension);
            _entityManager.AddComponentData(entity, transform);

            // Act
            _suspensionSystem.Update();

            // Assert
            var updatedSuspension = _entityManager.GetComponentData<SuspensionComponent>(entity);
            Assert.Greater(updatedSuspension.energy, 0f, "Энергия подвески должна быть положительной");
            Assert.Greater(updatedSuspension.work, 0f, "Работа подвески должна быть положительной");
        }

        [Test]
        public void SuspensionSystem_Update_ShouldNotUpdateWhenInactive()
        {
            // Arrange
            var entity = _entityManager.CreateEntity();
            var suspension = new SuspensionComponent
            {
                restLength = 1f,
                maxLength = 1.5f,
                minLength = 0.5f,
                currentLength = 1f,
                springStiffness = 1000f,
                damping = 100f,
                maxDamping = 200f,
                mountPoint = new float3(0, 0, 0),
                suspensionDirection = new float3(0, -1, 0),
                isActive = false // Неактивная подвеска
            };
            var transform = new LocalTransform
            {
                Position = new float3(0, 1, 0),
                Rotation = quaternion.identity,
                Scale = 1f
            };

            _entityManager.AddComponentData(entity, suspension);
            _entityManager.AddComponentData(entity, transform);

            // Act
            _suspensionSystem.Update();

            // Assert
            var updatedSuspension = _entityManager.GetComponentData<SuspensionComponent>(entity);
            Assert.AreEqual(suspension.restLength, updatedSuspension.currentLength, "Длина подвески должна остаться равной длине покоя");
            Assert.AreEqual(0f, updatedSuspension.springForce, "Сила пружины должна быть 0");
            Assert.AreEqual(0f, updatedSuspension.damperForce, "Сила демпфера должна быть 0");
        }
    }
}
