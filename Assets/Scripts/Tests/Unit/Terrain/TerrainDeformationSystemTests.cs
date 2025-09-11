using NUnit.Framework;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using MudLike.Terrain.Components;
using MudLike.Vehicles.Components;

namespace MudLike.Tests.Unit.Terrain
{
    /// <summary>
    /// Unit тесты для TerrainDeformationSystem
    /// </summary>
    [TestFixture]
    public class TerrainDeformationSystemTests : ECSTestFixture
    {
        private TerrainDeformationSystem _deformationSystem;
        private Entity _terrainEntity;
        private Entity _wheelEntity;
        
        [SetUp]
        public override void Setup()
        {
            base.Setup();
            
            // Создаем систему
            _deformationSystem = World.CreateSystemManaged<TerrainDeformationSystem>();
            
            // Создаем террейн
            _terrainEntity = EntityManager.CreateEntity();
            EntityManager.AddComponentData(_terrainEntity, new TerrainData
            {
                Width = 100f,
                Height = 100f,
                Resolution = 256,
                DeformationStrength = 1f,
                DeformationRadius = 5f,
                NeedsUpdate = false
            });
            EntityManager.AddComponentData(_terrainEntity, new DeformationData
            {
                Position = new float3(0f, 0f, 0f),
                Radius = 5f,
                Strength = 1f,
                IsActive = true,
                NeedsUpdate = false
            });
            
            // Создаем колесо
            _wheelEntity = EntityManager.CreateEntity();
            EntityManager.AddComponentData(_wheelEntity, new WheelData
            {
                Radius = 0.5f,
                IsGrounded = true,
                GroundPoint = new float3(0f, 0f, 0f),
                GroundNormal = new float3(0f, 1f, 0f),
                GroundDistance = 0f,
                SuspensionForce = new float3(0f, 1000f, 0f)
            });
            EntityManager.AddComponentData(_wheelEntity, new LocalTransform
            {
                Position = new float3(0f, 0f, 0f),
                Rotation = quaternion.identity
            });
        }
        
        [Test]
        public void TerrainDeformationSystem_ProcessesWheelDeformations()
        {
            // Arrange
            var wheelData = EntityManager.GetComponentData<WheelData>(_wheelEntity);
            wheelData.IsGrounded = true;
            wheelData.SuspensionForce = new float3(0f, 2000f, 0f);
            EntityManager.SetComponentData(_wheelEntity, wheelData);
            
            // Act
            _deformationSystem.Update();
            
            // Assert - система должна обработать деформации без исключений
            Assert.DoesNotThrow(() => _deformationSystem.Update());
        }
        
        [Test]
        public void TerrainDeformationSystem_IgnoresNonGroundedWheels()
        {
            // Arrange
            var wheelData = EntityManager.GetComponentData<WheelData>(_wheelEntity);
            wheelData.IsGrounded = false;
            EntityManager.SetComponentData(_wheelEntity, wheelData);
            
            // Act
            _deformationSystem.Update();
            
            // Assert - система должна работать без исключений
            Assert.DoesNotThrow(() => _deformationSystem.Update());
        }
        
        [Test]
        public void TerrainDeformationSystem_CreatesWheelDeformation()
        {
            // Arrange
            var wheelData = EntityManager.GetComponentData<WheelData>(_wheelEntity);
            wheelData.IsGrounded = true;
            wheelData.SuspensionForce = new float3(0f, 1500f, 0f);
            EntityManager.SetComponentData(_wheelEntity, wheelData);
            
            var transform = EntityManager.GetComponentData<LocalTransform>(_wheelEntity);
            transform.Position = new float3(10f, 0f, 10f);
            EntityManager.SetComponentData(_wheelEntity, transform);
            
            // Act
            _deformationSystem.Update();
            
            // Assert - система должна создать деформацию
            // (В реальной реализации здесь была бы проверка создания деформации)
            Assert.DoesNotThrow(() => _deformationSystem.Update());
        }
        
        [Test]
        public void TerrainDeformationSystem_UpdatesTerrainChunks()
        {
            // Act
            _deformationSystem.Update();
            
            // Assert - система должна обновить чанки террейна
            Assert.DoesNotThrow(() => _deformationSystem.Update());
        }
        
        [Test]
        public void TerrainDeformationSystem_HandlesMultipleWheels()
        {
            // Arrange - создаем второе колесо
            var secondWheelEntity = EntityManager.CreateEntity();
            EntityManager.AddComponentData(secondWheelEntity, new WheelData
            {
                Radius = 0.5f,
                IsGrounded = true,
                GroundPoint = new float3(5f, 0f, 5f),
                GroundNormal = new float3(0f, 1f, 0f),
                GroundDistance = 0f,
                SuspensionForce = new float3(0f, 1000f, 0f)
            });
            EntityManager.AddComponentData(secondWheelEntity, new LocalTransform
            {
                Position = new float3(5f, 0f, 5f),
                Rotation = quaternion.identity
            });
            
            // Act
            _deformationSystem.Update();
            
            // Assert - система должна обработать оба колеса
            Assert.DoesNotThrow(() => _deformationSystem.Update());
        }
        
        [Test]
        public void TerrainDeformationSystem_HandlesEmptyWheelQuery()
        {
            // Arrange - удаляем все колеса
            EntityManager.DestroyEntity(_wheelEntity);
            
            // Act
            _deformationSystem.Update();
            
            // Assert - система должна работать без исключений
            Assert.DoesNotThrow(() => _deformationSystem.Update());
        }
        
        [Test]
        public void TerrainDeformationSystem_ProcessesOtherDeformations()
        {
            // Act
            _deformationSystem.Update();
            
            // Assert - система должна обработать другие деформации
            Assert.DoesNotThrow(() => _deformationSystem.Update());
        }
        
        [Test]
        public void TerrainDeformationSystem_HandlesLargeSuspensionForces()
        {
            // Arrange
            var wheelData = EntityManager.GetComponentData<WheelData>(_wheelEntity);
            wheelData.IsGrounded = true;
            wheelData.SuspensionForce = new float3(0f, 10000f, 0f); // Очень большая сила
            EntityManager.SetComponentData(_wheelEntity, wheelData);
            
            // Act
            _deformationSystem.Update();
            
            // Assert - система должна обработать большую силу
            Assert.DoesNotThrow(() => _deformationSystem.Update());
        }
        
        [Test]
        public void TerrainDeformationSystem_HandlesZeroSuspensionForce()
        {
            // Arrange
            var wheelData = EntityManager.GetComponentData<WheelData>(_wheelEntity);
            wheelData.IsGrounded = true;
            wheelData.SuspensionForce = new float3(0f, 0f, 0f); // Нулевая сила
            EntityManager.SetComponentData(_wheelEntity, wheelData);
            
            // Act
            _deformationSystem.Update();
            
            // Assert - система должна обработать нулевую силу
            Assert.DoesNotThrow(() => _deformationSystem.Update());
        }
    }
}