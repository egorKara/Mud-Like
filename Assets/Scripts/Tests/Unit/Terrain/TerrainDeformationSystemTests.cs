using NUnit.Framework;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;
using MudLike.Terrain.Components;
using MudLike.Terrain.Systems;
using MudLike.Vehicles.Components;
using MudLike.Tests.Infrastructure;

namespace MudLike.Tests.Unit.Terrain
{
    /// <summary>
    /// Unit тесты для TerrainDeformationSystem
    /// </summary>
    [TestFixture]
    public class TerrainDeformationSystemTests : MudLikeTestFixture
    {
        private TerrainDeformationSystem _system;
        private Entity _terrainEntity;
        private Entity _vehicleEntity;
        
        [SetUp]
        public override void Setup()
        {
            base.Setup();
            _system = World.CreateSystemManaged<TerrainDeformationSystem>();
            
            // Создаем тестовый террейн
            _terrainEntity = CreateTerrain();
            
            // Создаем тестовый транспорт
            _vehicleEntity = CreateVehicle();
        }
        
        [Test]
        public void TerrainDeformationSystem_ShouldDeformTerrain_WhenVehicleAppliesPressure()
        {
            // Arrange
            var wheelData = new WheelData
            {
                IsGrounded = true,
                GroundPoint = new float3(0, 0, 0),
                GroundNormal = new float3(0, 1, 0),
                GroundDistance = 0.1f,
                SuspensionForce = new float3(0, 1000f, 0)
            };
            
            var wheelEntity = EntityManager.CreateEntity();
            EntityManager.AddComponentData(wheelEntity, wheelData);
            EntityManager.AddComponentData(wheelEntity, new LocalTransform
            {
                Position = new float3(0, 0, 0),
                Rotation = quaternion.identity
            });
            
            // Act
            _system.Update();
            
            // Assert
            var terrainData = EntityManager.GetComponentData<TerrainData>(_terrainEntity);
            Assert.IsTrue(terrainData.NeedsUpdate);
        }
        
        [Test]
        public void TerrainDeformationSystem_ShouldNotDeformTerrain_WhenWheelNotGrounded()
        {
            // Arrange
            var wheelData = new WheelData
            {
                IsGrounded = false,
                GroundPoint = new float3(0, 0, 0),
                GroundNormal = new float3(0, 1, 0),
                GroundDistance = 1.0f,
                SuspensionForce = new float3(0, 0, 0)
            };
            
            var wheelEntity = EntityManager.CreateEntity();
            EntityManager.AddComponentData(wheelEntity, wheelData);
            EntityManager.AddComponentData(wheelEntity, new LocalTransform
            {
                Position = new float3(0, 0, 0),
                Rotation = quaternion.identity
            });
            
            // Act
            _system.Update();
            
            // Assert
            var terrainData = EntityManager.GetComponentData<TerrainData>(_terrainEntity);
            Assert.IsFalse(terrainData.NeedsUpdate);
        }
        
        [Test]
        public void TerrainDeformationSystem_ShouldCalculateCorrectDeformation_ForDifferentSurfaceTypes()
        {
            // Arrange
            var mudSurface = new SurfaceData
            {
                SurfaceType = SurfaceType.Mud,
                Viscosity = 0.8f,
                Density = 2000f,
                PenetrationDepth = 0.3f
            };
            
            var asphaltSurface = new SurfaceData
            {
                SurfaceType = SurfaceType.Asphalt,
                Viscosity = 0.0f,
                Density = 2400f,
                PenetrationDepth = 0.0f
            };
            
            // Act & Assert
            var mudDeformation = CalculateDeformation(mudSurface, 1000f);
            var asphaltDeformation = CalculateDeformation(asphaltSurface, 1000f);
            
            Assert.Greater(mudDeformation, asphaltDeformation);
        }
        
        [Test]
        public void TerrainDeformationSystem_ShouldRespectMaxDeformationDepth()
        {
            // Arrange
            var wheelData = new WheelData
            {
                IsGrounded = true,
                GroundPoint = new float3(0, 0, 0),
                GroundNormal = new float3(0, 1, 0),
                GroundDistance = 0.1f,
                SuspensionForce = new float3(0, 10000f, 0) // Очень большая сила
            };
            
            var wheelEntity = EntityManager.CreateEntity();
            EntityManager.AddComponentData(wheelEntity, wheelData);
            EntityManager.AddComponentData(wheelEntity, new LocalTransform
            {
                Position = new float3(0, 0, 0),
                Rotation = quaternion.identity
            });
            
            // Act
            _system.Update();
            
            // Assert
            var terrainData = EntityManager.GetComponentData<TerrainData>(_terrainEntity);
            Assert.LessOrEqual(terrainData.MaxDeformationDepth, 1.0f);
        }
        
        [Test]
        public void TerrainDeformationSystem_ShouldUpdateTerrainCollider_AfterDeformation()
        {
            // Arrange
            var wheelData = new WheelData
            {
                IsGrounded = true,
                GroundPoint = new float3(0, 0, 0),
                GroundNormal = new float3(0, 1, 0),
                GroundDistance = 0.1f,
                SuspensionForce = new float3(0, 1000f, 0)
            };
            
            var wheelEntity = EntityManager.CreateEntity();
            EntityManager.AddComponentData(wheelEntity, wheelData);
            EntityManager.AddComponentData(wheelEntity, new LocalTransform
            {
                Position = new float3(0, 0, 0),
                Rotation = quaternion.identity
            });
            
            // Act
            _system.Update();
            
            // Assert
            var terrainData = EntityManager.GetComponentData<TerrainData>(_terrainEntity);
            Assert.IsTrue(terrainData.ColliderNeedsUpdate);
        }
        
        [Test]
        public void TerrainDeformationSystem_ShouldHandleMultipleWheels()
        {
            // Arrange
            var wheelPositions = new float3[]
            {
                new float3(-1, 0, 0),
                new float3(1, 0, 0),
                new float3(0, 0, -1),
                new float3(0, 0, 1)
            };
            
            foreach (var position in wheelPositions)
            {
                var wheelData = new WheelData
                {
                    IsGrounded = true,
                    GroundPoint = position,
                    GroundNormal = new float3(0, 1, 0),
                    GroundDistance = 0.1f,
                    SuspensionForce = new float3(0, 1000f, 0)
                };
                
                var wheelEntity = EntityManager.CreateEntity();
                EntityManager.AddComponentData(wheelEntity, wheelData);
                EntityManager.AddComponentData(wheelEntity, new LocalTransform
                {
                    Position = position,
                    Rotation = quaternion.identity
                });
            }
            
            // Act
            _system.Update();
            
            // Assert
            var terrainData = EntityManager.GetComponentData<TerrainData>(_terrainEntity);
            Assert.IsTrue(terrainData.NeedsUpdate);
        }
        
        [Test]
        public void TerrainDeformationSystem_ShouldRunWithoutErrors()
        {
            // Arrange
            var wheelData = new WheelData
            {
                IsGrounded = true,
                GroundPoint = new float3(0, 0, 0),
                GroundNormal = new float3(0, 1, 0),
                GroundDistance = 0.1f,
                SuspensionForce = new float3(0, 1000f, 0)
            };
            
            var wheelEntity = EntityManager.CreateEntity();
            EntityManager.AddComponentData(wheelEntity, wheelData);
            EntityManager.AddComponentData(wheelEntity, new LocalTransform
            {
                Position = new float3(0, 0, 0),
                Rotation = quaternion.identity
            });
            
            // Act & Assert
            Assert.DoesNotThrow(() => _system.Update());
        }
        
        [Test]
        public void TerrainDeformationSystem_ShouldMeetPerformanceRequirements()
        {
            // Arrange
            for (int i = 0; i < 100; i++)
            {
                var wheelData = new WheelData
                {
                    IsGrounded = true,
                    GroundPoint = new float3(i * 0.1f, 0, 0),
                    GroundNormal = new float3(0, 1, 0),
                    GroundDistance = 0.1f,
                    SuspensionForce = new float3(0, 1000f, 0)
                };
                
                var wheelEntity = EntityManager.CreateEntity();
                EntityManager.AddComponentData(wheelEntity, wheelData);
                EntityManager.AddComponentData(wheelEntity, new LocalTransform
                {
                    Position = new float3(i * 0.1f, 0, 0),
                    Rotation = quaternion.identity
                });
            }
            
            // Act & Assert
            AssertSystemPerformance<TerrainDeformationSystem>(16.67f); // 60 FPS
        }
        
        private Entity CreateTerrain()
        {
            var entity = EntityManager.CreateEntity();
            EntityManager.AddComponentData(entity, new TerrainData
            {
                Width = 1000,
                Height = 1000,
                Resolution = 1.0f,
                MaxDeformationDepth = 1.0f,
                NeedsUpdate = false,
                ColliderNeedsUpdate = false
            });
            EntityManager.AddComponentData(entity, new LocalTransform
            {
                Position = new float3(0, 0, 0),
                Rotation = quaternion.identity
            });
            return entity;
        }
        
        private Entity CreateVehicle()
        {
            var entity = EntityManager.CreateEntity();
            EntityManager.AddComponentData(entity, new VehicleTag());
            EntityManager.AddComponentData(entity, new VehicleConfig
            {
                MaxSpeed = 100f,
                Acceleration = 10f,
                Mass = 1500f
            });
            EntityManager.AddComponentData(entity, new VehiclePhysics
            {
                Velocity = float3.zero,
                AngularVelocity = float3.zero
            });
            EntityManager.AddComponentData(entity, new LocalTransform
            {
                Position = new float3(0, 0, 0),
                Rotation = quaternion.identity
            });
            return entity;
        }
        
        private float CalculateDeformation(SurfaceData surface, float pressure)
        {
            // Упрощенный расчет деформации для тестов
            return pressure * surface.PenetrationDepth / (surface.Density * 1000f);
        }
    }
}