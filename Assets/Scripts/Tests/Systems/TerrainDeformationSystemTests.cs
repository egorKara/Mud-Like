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
    /// Unit тесты для системы деформации местности
    /// </summary>
    [TestFixture]
    public class TerrainDeformationSystemTests
    {
        private World _world;
        private EntityManager _entityManager;
        private TerrainDeformationSystem _terrainDeformationSystem;

        [SetUp]
        public void SetUp()
        {
            // Создание тестового мира
            _world = new World("TestWorld");
            _entityManager = _world.EntityManager;
            
            // Создание системы
            _terrainDeformationSystem = _world.GetOrCreateSystemManaged<TerrainDeformationSystem>();
        }

        [TearDown]
        public void TearDown()
        {
            // Очистка тестового мира
            _world.Dispose();
        }

        [Test]
        public void TerrainDeformationSystem_Update_ShouldUpdateActiveDeformations()
        {
            // Arrange
            var entity = _entityManager.CreateEntity();
            var terrain = new TerrainComponent
            {
                gridSizeX = 100,
                gridSizeZ = 100,
                cellSize = 1f,
                terrainSizeX = 100f,
                terrainSizeZ = 100f,
                minHeight = 0f,
                maxHeight = 10f,
                surfaceStiffness = 1000f,
                surfaceDamping = 50f,
                surfaceFriction = 0.5f,
                surfaceGrip = 0.4f,
                materialDensity = 2000f,
                materialStrength = 100000f,
                materialPlasticity = 0.3f,
                materialViscosity = 100f,
                deformationEnabled = true,
                erosionEnabled = false,
                recoveryRate = 0.1f,
                recoveryTime = 10f,
                maxDeformationDepth = 2f,
                deformationRadius = 1f,
                deformationForce = 1000f,
                activeDeformations = 5,
                totalDeformationEnergy = 500f
            };

            _entityManager.AddComponentData(entity, terrain);

            // Act
            _terrainDeformationSystem.Update();

            // Assert
            var updatedTerrain = _entityManager.GetComponentData<TerrainComponent>(entity);
            Assert.LessOrEqual(updatedTerrain.activeDeformations, 5, "Количество активных деформаций должно уменьшиться или остаться прежним");
            Assert.Less(updatedTerrain.totalDeformationEnergy, 500f, "Общая энергия деформации должна уменьшиться");
        }

        [Test]
        public void TerrainDeformationSystem_Update_ShouldRecoverSurface()
        {
            // Arrange
            var entity = _entityManager.CreateEntity();
            var terrain = new TerrainComponent
            {
                gridSizeX = 100,
                gridSizeZ = 100,
                cellSize = 1f,
                terrainSizeX = 100f,
                terrainSizeZ = 100f,
                minHeight = 0f,
                maxHeight = 10f,
                surfaceStiffness = 1000f,
                surfaceDamping = 50f,
                surfaceFriction = 0.5f,
                surfaceGrip = 0.4f,
                materialDensity = 2000f,
                materialStrength = 100000f,
                materialPlasticity = 0.3f,
                materialViscosity = 100f,
                deformationEnabled = true,
                erosionEnabled = false,
                recoveryRate = 0.5f, // Высокая скорость восстановления
                recoveryTime = 10f,
                maxDeformationDepth = 2f,
                deformationRadius = 1f,
                deformationForce = 1000f,
                activeDeformations = 10,
                totalDeformationEnergy = 1000f
            };

            _entityManager.AddComponentData(entity, terrain);

            // Act
            _terrainDeformationSystem.Update();

            // Assert
            var updatedTerrain = _entityManager.GetComponentData<TerrainComponent>(entity);
            Assert.Less(updatedTerrain.activeDeformations, 10, "Количество активных деформаций должно уменьшиться при восстановлении");
        }

        [Test]
        public void TerrainDeformationSystem_Update_ShouldNotRecoverWhenRecoveryRateIsZero()
        {
            // Arrange
            var entity = _entityManager.CreateEntity();
            var terrain = new TerrainComponent
            {
                gridSizeX = 100,
                gridSizeZ = 100,
                cellSize = 1f,
                terrainSizeX = 100f,
                terrainSizeZ = 100f,
                minHeight = 0f,
                maxHeight = 10f,
                surfaceStiffness = 1000f,
                surfaceDamping = 50f,
                surfaceFriction = 0.5f,
                surfaceGrip = 0.4f,
                materialDensity = 2000f,
                materialStrength = 100000f,
                materialPlasticity = 0.3f,
                materialViscosity = 100f,
                deformationEnabled = true,
                erosionEnabled = false,
                recoveryRate = 0f, // Нулевая скорость восстановления
                recoveryTime = 10f,
                maxDeformationDepth = 2f,
                deformationRadius = 1f,
                deformationForce = 1000f,
                activeDeformations = 5,
                totalDeformationEnergy = 500f
            };

            _entityManager.AddComponentData(entity, terrain);

            // Act
            _terrainDeformationSystem.Update();

            // Assert
            var updatedTerrain = _entityManager.GetComponentData<TerrainComponent>(entity);
            Assert.AreEqual(5, updatedTerrain.activeDeformations, "Количество активных деформаций не должно измениться при нулевой скорости восстановления");
        }

        [Test]
        public void TerrainDeformationSystem_Update_ShouldUpdateDeformationEnergy()
        {
            // Arrange
            var entity = _entityManager.CreateEntity();
            var terrain = new TerrainComponent
            {
                gridSizeX = 100,
                gridSizeZ = 100,
                cellSize = 1f,
                terrainSizeX = 100f,
                terrainSizeZ = 100f,
                minHeight = 0f,
                maxHeight = 10f,
                surfaceStiffness = 1000f,
                surfaceDamping = 50f,
                surfaceFriction = 0.5f,
                surfaceGrip = 0.4f,
                materialDensity = 2000f,
                materialStrength = 100000f,
                materialPlasticity = 0.3f,
                materialViscosity = 100f,
                deformationEnabled = true,
                erosionEnabled = false,
                recoveryRate = 0.1f,
                recoveryTime = 10f,
                maxDeformationDepth = 2f,
                deformationRadius = 1f,
                deformationForce = 1000f,
                activeDeformations = 3,
                totalDeformationEnergy = 300f
            };

            _entityManager.AddComponentData(entity, terrain);

            // Act
            _terrainDeformationSystem.Update();

            // Assert
            var updatedTerrain = _entityManager.GetComponentData<TerrainComponent>(entity);
            Assert.Greater(updatedTerrain.totalDeformationEnergy, 0f, "Общая энергия деформации должна быть положительной");
        }

        [Test]
        public void TerrainDeformationSystem_CreateDeformation_ShouldCreateNewDeformation()
        {
            // Arrange
            var entity = _entityManager.CreateEntity();
            var terrain = new TerrainComponent
            {
                gridSizeX = 100,
                gridSizeZ = 100,
                cellSize = 1f,
                terrainSizeX = 100f,
                terrainSizeZ = 100f,
                minHeight = 0f,
                maxHeight = 10f,
                surfaceStiffness = 1000f,
                surfaceDamping = 50f,
                surfaceFriction = 0.5f,
                surfaceGrip = 0.4f,
                materialDensity = 2000f,
                materialStrength = 100000f,
                materialPlasticity = 0.3f,
                materialViscosity = 100f,
                deformationEnabled = true,
                erosionEnabled = false,
                recoveryRate = 0.1f,
                recoveryTime = 10f,
                maxDeformationDepth = 2f,
                deformationRadius = 1f,
                deformationForce = 1000f,
                activeDeformations = 0,
                totalDeformationEnergy = 0f
            };

            _entityManager.AddComponentData(entity, terrain);

            // Act
            TerrainDeformationSystem.CreateDeformation(
                ref terrain,
                new float3(10, 0, 10),
                2000f,
                1.5f,
                DeformationType.WheelTrack,
                SurfaceMaterial.Mud
            );

            // Assert
            Assert.Greater(terrain.activeDeformations, 0, "Количество активных деформаций должно увеличиться");
            Assert.Greater(terrain.totalDeformationEnergy, 0f, "Общая энергия деформации должна увеличиться");
        }

        [Test]
        public void TerrainDeformationSystem_CreateDeformation_ShouldNotCreateWhenDisabled()
        {
            // Arrange
            var entity = _entityManager.CreateEntity();
            var terrain = new TerrainComponent
            {
                gridSizeX = 100,
                gridSizeZ = 100,
                cellSize = 1f,
                terrainSizeX = 100f,
                terrainSizeZ = 100f,
                minHeight = 0f,
                maxHeight = 10f,
                surfaceStiffness = 1000f,
                surfaceDamping = 50f,
                surfaceFriction = 0.5f,
                surfaceGrip = 0.4f,
                materialDensity = 2000f,
                materialStrength = 100000f,
                materialPlasticity = 0.3f,
                materialViscosity = 100f,
                deformationEnabled = false, // Деформация отключена
                erosionEnabled = false,
                recoveryRate = 0.1f,
                recoveryTime = 10f,
                maxDeformationDepth = 2f,
                deformationRadius = 1f,
                deformationForce = 1000f,
                activeDeformations = 0,
                totalDeformationEnergy = 0f
            };

            _entityManager.AddComponentData(entity, terrain);

            // Act
            TerrainDeformationSystem.CreateDeformation(
                ref terrain,
                new float3(10, 0, 10),
                2000f,
                1.5f,
                DeformationType.WheelTrack,
                SurfaceMaterial.Mud
            );

            // Assert
            Assert.AreEqual(0, terrain.activeDeformations, "Количество активных деформаций не должно измениться при отключенной деформации");
            Assert.AreEqual(0f, terrain.totalDeformationEnergy, "Общая энергия деформации не должна измениться при отключенной деформации");
        }

        [Test]
        public void TerrainDeformationSystem_CreateDeformation_ShouldNotCreateWhenMaxDeformationsReached()
        {
            // Arrange
            var entity = _entityManager.CreateEntity();
            var terrain = new TerrainComponent
            {
                gridSizeX = 100,
                gridSizeZ = 100,
                cellSize = 1f,
                terrainSizeX = 100f,
                terrainSizeZ = 100f,
                minHeight = 0f,
                maxHeight = 10f,
                surfaceStiffness = 1000f,
                surfaceDamping = 50f,
                surfaceFriction = 0.5f,
                surfaceGrip = 0.4f,
                materialDensity = 2000f,
                materialStrength = 100000f,
                materialPlasticity = 0.3f,
                materialViscosity = 100f,
                deformationEnabled = true,
                erosionEnabled = false,
                recoveryRate = 0.1f,
                recoveryTime = 10f,
                maxDeformationDepth = 2f,
                deformationRadius = 1f,
                deformationForce = 1000f,
                activeDeformations = 1000, // Максимум деформаций
                totalDeformationEnergy = 100000f
            };

            _entityManager.AddComponentData(entity, terrain);

            // Act
            TerrainDeformationSystem.CreateDeformation(
                ref terrain,
                new float3(10, 0, 10),
                2000f,
                1.5f,
                DeformationType.WheelTrack,
                SurfaceMaterial.Mud
            );

            // Assert
            Assert.AreEqual(1000, terrain.activeDeformations, "Количество активных деформаций не должно превысить максимум");
        }
    }
}
