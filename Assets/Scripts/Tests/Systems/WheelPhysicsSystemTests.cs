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
    /// Unit тесты для системы физики колес
    /// </summary>
    [TestFixture]
    public class WheelPhysicsSystemTests
    {
        private World _world;
        private EntityManager _entityManager;
        private WheelPhysicsSystem _wheelPhysicsSystem;

        [SetUp]
        public void SetUp()
        {
            // Создание тестового мира
            _world = new World("TestWorld");
            _entityManager = _world.EntityManager;
            
            // Создание системы
            _wheelPhysicsSystem = _world.GetOrCreateSystemManaged<WheelPhysicsSystem>();
        }

        [TearDown]
        public void TearDown()
        {
            // Очистка тестового мира
            _world.Dispose();
        }

        [Test]
        public void WheelPhysicsSystem_Update_ShouldUpdateWheelAngularVelocity()
        {
            // Arrange
            var entity = _entityManager.CreateEntity();
            var wheel = new WheelComponent
            {
                radius = 0.5f,
                mass = 10f,
                inertia = 1f,
                angularVelocity = 0f,
                brakeForce = 0f,
                maxBrakeForce = 1000f
            };
            var transform = new LocalTransform
            {
                Position = new float3(0, 1, 0),
                Rotation = quaternion.identity,
                Scale = 1f
            };

            _entityManager.AddComponentData(entity, wheel);
            _entityManager.AddComponentData(entity, transform);

            // Act
            _wheelPhysicsSystem.Update();

            // Assert
            var updatedWheel = _entityManager.GetComponentData<WheelComponent>(entity);
            Assert.AreEqual(0f, updatedWheel.angularVelocity, "Угловая скорость должна остаться 0 без тормозной силы");
        }

        [Test]
        public void WheelPhysicsSystem_Update_ShouldApplyBrakeForce()
        {
            // Arrange
            var entity = _entityManager.CreateEntity();
            var wheel = new WheelComponent
            {
                radius = 0.5f,
                mass = 10f,
                inertia = 1f,
                angularVelocity = 100f,
                brakeForce = 500f,
                maxBrakeForce = 1000f
            };
            var transform = new LocalTransform
            {
                Position = new float3(0, 1, 0),
                Rotation = quaternion.identity,
                Scale = 1f
            };

            _entityManager.AddComponentData(entity, wheel);
            _entityManager.AddComponentData(entity, transform);

            // Act
            _wheelPhysicsSystem.Update();

            // Assert
            var updatedWheel = _entityManager.GetComponentData<WheelComponent>(entity);
            Assert.Less(updatedWheel.angularVelocity, 100f, "Угловая скорость должна уменьшиться при торможении");
        }

        [Test]
        public void WheelPhysicsSystem_Update_ShouldClampAngularVelocity()
        {
            // Arrange
            var entity = _entityManager.CreateEntity();
            var wheel = new WheelComponent
            {
                radius = 0.5f,
                mass = 10f,
                inertia = 1f,
                angularVelocity = 2000f, // Превышает максимум
                brakeForce = 0f,
                maxBrakeForce = 1000f
            };
            var transform = new LocalTransform
            {
                Position = new float3(0, 1, 0),
                Rotation = quaternion.identity,
                Scale = 1f
            };

            _entityManager.AddComponentData(entity, wheel);
            _entityManager.AddComponentData(entity, transform);

            // Act
            _wheelPhysicsSystem.Update();

            // Assert
            var updatedWheel = _entityManager.GetComponentData<WheelComponent>(entity);
            Assert.LessOrEqual(updatedWheel.angularVelocity, 1000f, "Угловая скорость должна быть ограничена максимумом");
            Assert.GreaterOrEqual(updatedWheel.angularVelocity, -1000f, "Угловая скорость должна быть ограничена минимумом");
        }

        [Test]
        public void WheelPhysicsSystem_Update_ShouldDetectGroundContact()
        {
            // Arrange
            var entity = _entityManager.CreateEntity();
            var wheel = new WheelComponent
            {
                radius = 0.5f,
                mass = 10f,
                inertia = 1f,
                angularVelocity = 0f,
                brakeForce = 0f,
                maxBrakeForce = 1000f,
                localPosition = new float3(0, 0, 0)
            };
            var transform = new LocalTransform
            {
                Position = new float3(0, 0.4f, 0), // Высота меньше радиуса + 0.1
                Rotation = quaternion.identity,
                Scale = 1f
            };

            _entityManager.AddComponentData(entity, wheel);
            _entityManager.AddComponentData(entity, transform);

            // Act
            _wheelPhysicsSystem.Update();

            // Assert
            var updatedWheel = _entityManager.GetComponentData<WheelComponent>(entity);
            Assert.IsTrue(updatedWheel.isGrounded, "Колесо должно контактировать с поверхностью");
            Assert.AreEqual(new float3(0, 1, 0), updatedWheel.groundNormal, "Нормаль поверхности должна быть направлена вверх");
        }

        [Test]
        public void WheelPhysicsSystem_Update_ShouldNotDetectGroundContactWhenFloating()
        {
            // Arrange
            var entity = _entityManager.CreateEntity();
            var wheel = new WheelComponent
            {
                radius = 0.5f,
                mass = 10f,
                inertia = 1f,
                angularVelocity = 0f,
                brakeForce = 0f,
                maxBrakeForce = 1000f,
                localPosition = new float3(0, 0, 0)
            };
            var transform = new LocalTransform
            {
                Position = new float3(0, 2f, 0), // Высота больше радиуса + 0.1
                Rotation = quaternion.identity,
                Scale = 1f
            };

            _entityManager.AddComponentData(entity, wheel);
            _entityManager.AddComponentData(entity, transform);

            // Act
            _wheelPhysicsSystem.Update();

            // Assert
            var updatedWheel = _entityManager.GetComponentData<WheelComponent>(entity);
            Assert.IsFalse(updatedWheel.isGrounded, "Колесо не должно контактировать с поверхностью");
            Assert.AreEqual(float3.zero, updatedWheel.groundNormal, "Нормаль поверхности должна быть нулевой");
        }

        [Test]
        public void WheelPhysicsSystem_Update_ShouldCalculateFrictionForces()
        {
            // Arrange
            var entity = _entityManager.CreateEntity();
            var wheel = new WheelComponent
            {
                radius = 0.5f,
                mass = 10f,
                inertia = 1f,
                angularVelocity = 0f,
                brakeForce = 0f,
                maxBrakeForce = 1000f,
                frictionCoefficient = 0.8f,
                localPosition = new float3(0, 0, 0)
            };
            var transform = new LocalTransform
            {
                Position = new float3(0, 0.4f, 0), // Контакт с поверхностью
                Rotation = quaternion.identity,
                Scale = 1f
            };

            _entityManager.AddComponentData(entity, wheel);
            _entityManager.AddComponentData(entity, transform);

            // Act
            _wheelPhysicsSystem.Update();

            // Assert
            var updatedWheel = _entityManager.GetComponentData<WheelComponent>(entity);
            Assert.IsTrue(updatedWheel.isGrounded, "Колесо должно контактировать с поверхностью");
            Assert.Greater(updatedWheel.groundReactionForce.y, 0f, "Сила реакции поверхности должна быть положительной");
        }
    }
}
