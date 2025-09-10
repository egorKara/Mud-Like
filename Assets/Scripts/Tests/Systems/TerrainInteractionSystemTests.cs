using NUnit.Framework;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using MudLike.Terrain.Components;
using MudLike.Terrain.Systems;

namespace MudLike.Tests.Systems
{
    /// <summary>
    /// Unit тесты для системы взаимодействия с местностью
    /// </summary>
    [TestFixture]
    public class TerrainInteractionSystemTests
    {
        private World _world;
        private EntityManager _entityManager;
        private TerrainInteractionSystem _terrainInteractionSystem;

        [SetUp]
        public void SetUp()
        {
            // Создание тестового мира
            _world = new World("TestWorld");
            _entityManager = _world.EntityManager;
            
            // Создание системы
            _terrainInteractionSystem = _world.GetOrCreateSystemManaged<TerrainInteractionSystem>();
        }

        [TearDown]
        public void TearDown()
        {
            // Очистка тестового мира
            _world.Dispose();
        }

        [Test]
        public void TerrainInteractionSystem_Update_ShouldUpdateInteractionPosition()
        {
            // Arrange
            var entity = _entityManager.CreateEntity();
            var interaction = new TerrainInteractionComponent
            {
                worldPosition = new float3(0, 0, 0),
                interactionForce = 1000f,
                interactionRadius = 0.5f,
                interactionVelocity = new float3(0, 0, 10f),
                interactionType = InteractionType.WheelContact,
                surfaceMaterial = SurfaceMaterial.Mud,
                isActive = true,
                createsDeformation = true,
                affectsPhysics = true
            };
            var transform = new LocalTransform
            {
                Position = new float3(5, 0, 5),
                Rotation = quaternion.identity,
                Scale = 1f
            };

            _entityManager.AddComponentData(entity, interaction);
            _entityManager.AddComponentData(entity, transform);

            // Act
            _terrainInteractionSystem.Update();

            // Assert
            var updatedInteraction = _entityManager.GetComponentData<TerrainInteractionComponent>(entity);
            Assert.AreEqual(transform.Position, updatedInteraction.worldPosition, "Позиция взаимодействия должна обновиться");
        }

        [Test]
        public void TerrainInteractionSystem_Update_ShouldGetSurfaceProperties()
        {
            // Arrange
            var entity = _entityManager.CreateEntity();
            var interaction = new TerrainInteractionComponent
            {
                worldPosition = new float3(0, 0, 0),
                interactionForce = 1000f,
                interactionRadius = 0.5f,
                interactionVelocity = new float3(0, 0, 10f),
                interactionType = InteractionType.WheelContact,
                surfaceMaterial = SurfaceMaterial.Mud,
                isActive = true,
                createsDeformation = true,
                affectsPhysics = true
            };
            var transform = new LocalTransform
            {
                Position = new float3(0, 0, 0),
                Rotation = quaternion.identity,
                Scale = 1f
            };

            _entityManager.AddComponentData(entity, interaction);
            _entityManager.AddComponentData(entity, transform);

            // Act
            _terrainInteractionSystem.Update();

            // Assert
            var updatedInteraction = _entityManager.GetComponentData<TerrainInteractionComponent>(entity);
            Assert.AreEqual(SurfaceMaterial.Mud, updatedInteraction.surfaceMaterial, "Материал поверхности должен быть установлен");
            Assert.AreEqual(new float3(0, 1, 0), updatedInteraction.surfaceNormal, "Нормаль поверхности должна быть направлена вверх");
            Assert.Greater(updatedInteraction.localFriction, 0f, "Коэффициент трения должен быть положительным");
            Assert.Greater(updatedInteraction.localGrip, 0f, "Коэффициент сцепления должен быть положительным");
        }

        [Test]
        public void TerrainInteractionSystem_Update_ShouldCalculateInteractionForces()
        {
            // Arrange
            var entity = _entityManager.CreateEntity();
            var interaction = new TerrainInteractionComponent
            {
                worldPosition = new float3(0, 0, 0),
                interactionForce = 1000f,
                interactionRadius = 0.5f,
                interactionVelocity = new float3(0, 0, 10f), // Движение вперед
                interactionType = InteractionType.WheelContact,
                surfaceMaterial = SurfaceMaterial.Mud,
                isActive = true,
                createsDeformation = true,
                affectsPhysics = true
            };
            var transform = new LocalTransform
            {
                Position = new float3(0, 0, 0),
                Rotation = quaternion.identity,
                Scale = 1f
            };

            _entityManager.AddComponentData(entity, interaction);
            _entityManager.AddComponentData(entity, transform);

            // Act
            _terrainInteractionSystem.Update();

            // Assert
            var updatedInteraction = _entityManager.GetComponentData<TerrainInteractionComponent>(entity);
            Assert.Greater(updatedInteraction.interactionEnergy, 0f, "Энергия взаимодействия должна быть положительной");
            Assert.Greater(updatedInteraction.interactionWork, 0f, "Работа взаимодействия должна быть положительной");
        }

        [Test]
        public void TerrainInteractionSystem_Update_ShouldNotCalculateForcesWhenInactive()
        {
            // Arrange
            var entity = _entityManager.CreateEntity();
            var interaction = new TerrainInteractionComponent
            {
                worldPosition = new float3(0, 0, 0),
                interactionForce = 1000f,
                interactionRadius = 0.5f,
                interactionVelocity = new float3(0, 0, 10f),
                interactionType = InteractionType.WheelContact,
                surfaceMaterial = SurfaceMaterial.Mud,
                isActive = false, // Неактивное взаимодействие
                createsDeformation = true,
                affectsPhysics = true
            };
            var transform = new LocalTransform
            {
                Position = new float3(0, 0, 0),
                Rotation = quaternion.identity,
                Scale = 1f
            };

            _entityManager.AddComponentData(entity, interaction);
            _entityManager.AddComponentData(entity, transform);

            // Act
            _terrainInteractionSystem.Update();

            // Assert
            var updatedInteraction = _entityManager.GetComponentData<TerrainInteractionComponent>(entity);
            Assert.AreEqual(0f, updatedInteraction.interactionEnergy, "Энергия взаимодействия должна быть 0 для неактивного взаимодействия");
        }

        [Test]
        public void TerrainInteractionSystem_Update_ShouldHandleDifferentSurfaceMaterials()
        {
            // Arrange
            var entity = _entityManager.CreateEntity();
            var interaction = new TerrainInteractionComponent
            {
                worldPosition = new float3(0, 0, 0),
                interactionForce = 1000f,
                interactionRadius = 0.5f,
                interactionVelocity = new float3(0, 0, 10f),
                interactionType = InteractionType.WheelContact,
                surfaceMaterial = SurfaceMaterial.Rock, // Камень
                isActive = true,
                createsDeformation = true,
                affectsPhysics = true
            };
            var transform = new LocalTransform
            {
                Position = new float3(0, 0, 0),
                Rotation = quaternion.identity,
                Scale = 1f
            };

            _entityManager.AddComponentData(entity, interaction);
            _entityManager.AddComponentData(entity, transform);

            // Act
            _terrainInteractionSystem.Update();

            // Assert
            var updatedInteraction = _entityManager.GetComponentData<TerrainInteractionComponent>(entity);
            Assert.AreEqual(SurfaceMaterial.Rock, updatedInteraction.surfaceMaterial, "Материал поверхности должен быть камень");
            Assert.Greater(updatedInteraction.localFriction, 0.7f, "Коэффициент трения для камня должен быть высоким");
            Assert.Greater(updatedInteraction.localGrip, 0.8f, "Коэффициент сцепления для камня должен быть высоким");
        }

        [Test]
        public void TerrainInteractionSystem_Update_ShouldHandleIceSurface()
        {
            // Arrange
            var entity = _entityManager.CreateEntity();
            var interaction = new TerrainInteractionComponent
            {
                worldPosition = new float3(0, 0, 0),
                interactionForce = 1000f,
                interactionRadius = 0.5f,
                interactionVelocity = new float3(0, 0, 10f),
                interactionType = InteractionType.WheelContact,
                surfaceMaterial = SurfaceMaterial.Ice, // Лед
                isActive = true,
                createsDeformation = true,
                affectsPhysics = true
            };
            var transform = new LocalTransform
            {
                Position = new float3(0, 0, 0),
                Rotation = quaternion.identity,
                Scale = 1f
            };

            _entityManager.AddComponentData(entity, interaction);
            _entityManager.AddComponentData(entity, transform);

            // Act
            _terrainInteractionSystem.Update();

            // Assert
            var updatedInteraction = _entityManager.GetComponentData<TerrainInteractionComponent>(entity);
            Assert.AreEqual(SurfaceMaterial.Ice, updatedInteraction.surfaceMaterial, "Материал поверхности должен быть лед");
            Assert.Less(updatedInteraction.localFriction, 0.2f, "Коэффициент трения для льда должен быть низким");
            Assert.Less(updatedInteraction.localGrip, 0.1f, "Коэффициент сцепления для льда должен быть очень низким");
        }

        [Test]
        public void TerrainInteractionSystem_Update_ShouldHandleWaterSurface()
        {
            // Arrange
            var entity = _entityManager.CreateEntity();
            var interaction = new TerrainInteractionComponent
            {
                worldPosition = new float3(0, 0, 0),
                interactionForce = 1000f,
                interactionRadius = 0.5f,
                interactionVelocity = new float3(0, 0, 10f),
                interactionType = InteractionType.WheelContact,
                surfaceMaterial = SurfaceMaterial.Water, // Вода
                isActive = true,
                createsDeformation = true,
                affectsPhysics = true
            };
            var transform = new LocalTransform
            {
                Position = new float3(0, 0, 0),
                Rotation = quaternion.identity,
                Scale = 1f
            };

            _entityManager.AddComponentData(entity, interaction);
            _entityManager.AddComponentData(entity, transform);

            // Act
            _terrainInteractionSystem.Update();

            // Assert
            var updatedInteraction = _entityManager.GetComponentData<TerrainInteractionComponent>(entity);
            Assert.AreEqual(SurfaceMaterial.Water, updatedInteraction.surfaceMaterial, "Материал поверхности должен быть вода");
            Assert.Less(updatedInteraction.localFriction, 0.1f, "Коэффициент трения для воды должен быть очень низким");
            Assert.Less(updatedInteraction.localGrip, 0.05f, "Коэффициент сцепления для воды должен быть очень низким");
        }
    }
}
