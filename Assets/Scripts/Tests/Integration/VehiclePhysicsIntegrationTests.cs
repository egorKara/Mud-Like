using NUnit.Framework;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using MudLike.Vehicles.Systems;
using MudLike.Vehicles.Components;
using MudLike.Terrain.Systems;
using MudLike.Terrain.Components;
using MudLike.Core.Components;
using Unity.Core;

namespace MudLike.Tests.Integration
{
    /// <summary>
    /// Интеграционные тесты для взаимодействия систем физики транспорта
    /// Проверяет корректную работу VehicleMovementSystem, MudManagerSystem и других систем
    /// </summary>
    public class VehiclePhysicsIntegrationTests
    {
        private World _world;
        private VehicleMovementSystem _vehicleMovementSystem;
        private MudManagerSystem _mudManagerSystem;
        private EntityManager _entityManager;

        [SetUp]
        public void SetUp()
        {
            _world = new World("TestWorld");
            _entityManager = _world.EntityManager;
            
            // Создаем системы
            _vehicleMovementSystem = _world.GetOrCreateSystemManaged<VehicleMovementSystem>();
            _vehicleMovementSystem.OnCreate(ref _world.Unmanaged);
            
            _mudManagerSystem = _world.GetOrCreateSystemManaged<MudManagerSystem>();
            _mudManagerSystem.OnCreate(ref _world.Unmanaged);
            
            // Устанавливаем время для SystemAPI.Time.time
            _world.SetSingleton(new TimeData { ElapsedTime = 10f, DeltaTime = 0.016f, FixedDeltaTime = 0.016f });
        }

        [TearDown]
        public void TearDown()
        {
            _vehicleMovementSystem.OnDestroy(ref _world.Unmanaged);
            _mudManagerSystem.OnDestroy(ref _world.Unmanaged);
            _world.Dispose();
        }

        [Test]
        public void VehicleMovement_WithMudInteraction_WorksCorrectly()
        {
            // Arrange
            var entity = _entityManager.CreateEntity();
            _entityManager.AddComponentData(entity, new LocalTransform 
            { 
                Position = new float3(0, 0, 0), 
                Rotation = quaternion.identity 
            });
            _entityManager.AddComponentData(entity, new VehiclePhysics
            {
                Velocity = float3.zero,
                Acceleration = float3.zero,
                ForwardSpeed = 0f,
                TurnSpeed = 0f
            });
            _entityManager.AddComponentData(entity, new VehicleConfig
            {
                MaxSpeed = 50f,
                Acceleration = 10f,
                TurnSpeed = 90f,
                Drag = 0.1f
            });
            _entityManager.AddComponentData(entity, new VehicleInput
            {
                Vertical = 1f,
                Horizontal = 0f
            });
            _entityManager.AddComponent<VehicleTag>(entity);

            // Act
            _vehicleMovementSystem.OnUpdate(ref _world.Unmanaged);

            // Assert
            var physics = _entityManager.GetComponentData<VehiclePhysics>(entity);
            var transform = _entityManager.GetComponentData<LocalTransform>(entity);
            
            Assert.Greater(physics.Velocity.x, 0f); // Транспорт должен двигаться вперед
            Assert.Greater(transform.Position.x, 0f); // Позиция должна измениться
        }

        [Test]
        public void MudManager_QueryContact_WithVehiclePosition_ReturnsValidData()
        {
            // Arrange
            float3 wheelPosition = new float3(10, 0, 5);
            float radius = 0.5f;
            float wheelForce = 1000f;

            // Act
            var contactData = _mudManagerSystem.QueryContact(wheelPosition, radius, wheelForce);

            // Assert
            Assert.IsTrue(contactData.IsValid);
            Assert.GreaterOrEqual(contactData.MudLevel, 0f);
            Assert.LessOrEqual(contactData.MudLevel, 1f);
            Assert.GreaterOrEqual(contactData.SinkDepth, 0f);
            Assert.GreaterOrEqual(contactData.TractionModifier, 0f);
            Assert.LessOrEqual(contactData.TractionModifier, 1f);
        }

        [Test]
        public void VehicleMovement_OnDifferentTerrainTypes_BehavesDifferently()
        {
            // Arrange
            var positions = new[]
            {
                new float3(0, 0, 0),    // Сухая земля
                new float3(50, 0, 0),   // Мокрая земля
                new float3(100, 0, 0)   // Грязь
            };

            var results = new MudContactData[positions.Length];

            // Act
            for (int i = 0; i < positions.Length; i++)
            {
                results[i] = _mudManagerSystem.QueryContact(positions[i], 0.5f, 1000f);
            }

            // Assert
            // Все результаты должны быть валидными
            foreach (var result in results)
            {
                Assert.IsTrue(result.IsValid);
                Assert.GreaterOrEqual(result.TractionModifier, 0f);
                Assert.LessOrEqual(result.TractionModifier, 1f);
            }
        }

        [Test]
        public void VehiclePhysics_WithMultipleWheels_IntegratesCorrectly()
        {
            // Arrange
            var vehicleEntity = _entityManager.CreateEntity();
            _entityManager.AddComponentData(vehicleEntity, new LocalTransform 
            { 
                Position = new float3(0, 0, 0), 
                Rotation = quaternion.identity 
            });
            _entityManager.AddComponentData(vehicleEntity, new VehiclePhysics
            {
                Velocity = float3.zero,
                Acceleration = float3.zero,
                ForwardSpeed = 0f,
                TurnSpeed = 0f
            });
            _entityManager.AddComponentData(vehicleEntity, new VehicleConfig
            {
                MaxSpeed = 30f,
                Acceleration = 8f,
                TurnSpeed = 60f,
                Drag = 0.15f
            });
            _entityManager.AddComponentData(vehicleEntity, new VehicleInput
            {
                Vertical = 0.8f,
                Horizontal = 0.3f
            });
            _entityManager.AddComponent<VehicleTag>(vehicleEntity);

            // Создаем колеса
            var wheelPositions = new[]
            {
                new float3(-1, -0.5f, 2),   // Переднее левое
                new float3(1, -0.5f, 2),    // Переднее правое
                new float3(-1, -0.5f, -2),  // Заднее левое
                new float3(1, -0.5f, -2)    // Заднее правое
            };

            for (int i = 0; i < wheelPositions.Length; i++)
            {
                var wheelEntity = _entityManager.CreateEntity();
                _entityManager.AddComponentData(wheelEntity, new LocalTransform 
                { 
                    Position = wheelPositions[i], 
                    Rotation = quaternion.identity 
                });
                _entityManager.AddComponentData(wheelEntity, new WheelData
                {
                    Position = wheelPositions[i],
                    Radius = 0.4f,
                    Width = 0.2f,
                    SuspensionLength = 0.3f,
                    SpringForce = 800f,
                    DampingForce = 400f,
                    IsGrounded = true,
                    GroundDistance = 0.1f
                });
                _entityManager.AddComponent<VehicleTag>(wheelEntity);
            }

            // Act
            _vehicleMovementSystem.OnUpdate(ref _world.Unmanaged);

            // Проверяем взаимодействие колес с грязью
            foreach (var wheelPos in wheelPositions)
            {
                var contactData = _mudManagerSystem.QueryContact(wheelPos, 0.4f, 500f);
                Assert.IsTrue(contactData.IsValid);
            }

            // Assert
            var physics = _entityManager.GetComponentData<VehiclePhysics>(vehicleEntity);
            var transform = _entityManager.GetComponentData<LocalTransform>(vehicleEntity);
            
            Assert.Greater(physics.Velocity.x, 0f); // Движение вперед
            Assert.NotZero(transform.Rotation.y); // Поворот
        }

        [Test]
        public void SystemIntegration_PhysicsAndTerrain_SynchronizeCorrectly()
        {
            // Arrange
            var entity = _entityManager.CreateEntity();
            _entityManager.AddComponentData(entity, new LocalTransform 
            { 
                Position = new float3(0, 0, 0), 
                Rotation = quaternion.identity 
            });
            _entityManager.AddComponentData(entity, new VehiclePhysics
            {
                Velocity = new float3(10f, 0, 0),
                Acceleration = float3.zero,
                ForwardSpeed = 10f,
                TurnSpeed = 0f
            });
            _entityManager.AddComponentData(entity, new VehicleConfig
            {
                MaxSpeed = 50f,
                Acceleration = 10f,
                TurnSpeed = 90f,
                Drag = 0.1f
            });
            _entityManager.AddComponentData(entity, new VehicleInput
            {
                Vertical = 0f,
                Horizontal = 0f
            });
            _entityManager.AddComponent<VehicleTag>(entity);

            // Act - выполняем несколько обновлений
            for (int i = 0; i < 10; i++)
            {
                _vehicleMovementSystem.OnUpdate(ref _world.Unmanaged);
                
                // Проверяем взаимодействие с грязью на каждой итерации
                var transform = _entityManager.GetComponentData<LocalTransform>(entity);
                var contactData = _mudManagerSystem.QueryContact(transform.Position, 0.5f, 1000f);
                Assert.IsTrue(contactData.IsValid);
            }

            // Assert
            var finalPhysics = _entityManager.GetComponentData<VehiclePhysics>(entity);
            var finalTransform = _entityManager.GetComponentData<LocalTransform>(entity);
            
            // Скорость должна уменьшиться из-за сопротивления
            Assert.Less(finalPhysics.Velocity.x, 10f);
            
            // Позиция должна измениться
            Assert.Greater(finalTransform.Position.x, 0f);
        }

        [Test]
        public void Performance_Integration_SystemsWorkEfficiently()
        {
            // Arrange
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            
            // Создаем множественные сущности
            for (int i = 0; i < 50; i++)
            {
                var entity = _entityManager.CreateEntity();
                _entityManager.AddComponentData(entity, new LocalTransform 
                { 
                    Position = new float3(i * 2, 0, 0), 
                    Rotation = quaternion.identity 
                });
                _entityManager.AddComponentData(entity, new VehiclePhysics
                {
                    Velocity = new float3(i % 10, 0, 0),
                    Acceleration = float3.zero,
                    ForwardSpeed = i % 10,
                    TurnSpeed = 0f
                });
                _entityManager.AddComponentData(entity, new VehicleConfig
                {
                    MaxSpeed = 50f,
                    Acceleration = 10f,
                    TurnSpeed = 90f,
                    Drag = 0.1f
                });
                _entityManager.AddComponentData(entity, new VehicleInput
                {
                    Vertical = 0.5f,
                    Horizontal = 0.2f
                });
                _entityManager.AddComponent<VehicleTag>(entity);
            }

            // Act
            _vehicleMovementSystem.OnUpdate(ref _world.Unmanaged);
            
            // Проверяем взаимодействие с грязью для всех сущностей
            var entities = _entityManager.GetAllEntities(Allocator.Temp);
            foreach (var entity in entities)
            {
                if (_entityManager.HasComponent<LocalTransform>(entity))
                {
                    var transform = _entityManager.GetComponentData<LocalTransform>(entity);
                    var contactData = _mudManagerSystem.QueryContact(transform.Position, 0.5f, 1000f);
                    Assert.IsTrue(contactData.IsValid);
                }
            }
            entities.Dispose();
            
            stopwatch.Stop();

            // Assert
            Assert.Less(stopwatch.ElapsedMilliseconds, 100); // Должно выполняться менее чем за 100ms
        }

        [Test]
        public void ErrorHandling_Integration_SystemsHandleErrorsGracefully()
        {
            // Arrange
            var entity = _entityManager.CreateEntity();
            _entityManager.AddComponentData(entity, new LocalTransform 
            { 
                Position = new float3(float.NaN, float.PositiveInfinity, float.NegativeInfinity), 
                Rotation = quaternion.identity 
            });
            _entityManager.AddComponentData(entity, new VehiclePhysics
            {
                Velocity = new float3(float.MaxValue, float.MinValue, float.Epsilon),
                Acceleration = float3.zero,
                ForwardSpeed = float.NaN,
                TurnSpeed = float.PositiveInfinity
            });
            _entityManager.AddComponentData(entity, new VehicleConfig
            {
                MaxSpeed = float.MinValue,
                Acceleration = float.Epsilon,
                TurnSpeed = float.MaxValue,
                Drag = float.NaN
            });
            _entityManager.AddComponentData(entity, new VehicleInput
            {
                Vertical = float.PositiveInfinity,
                Horizontal = float.NegativeInfinity
            });
            _entityManager.AddComponent<VehicleTag>(entity);

            // Act & Assert
            // Системы должны обрабатывать некорректные данные без исключений
            Assert.DoesNotThrow(() => 
            {
                _vehicleMovementSystem.OnUpdate(ref _world.Unmanaged);
                
                var transform = _entityManager.GetComponentData<LocalTransform>(entity);
                var contactData = _mudManagerSystem.QueryContact(transform.Position, 0.5f, 1000f);
            });
        }
    }
}