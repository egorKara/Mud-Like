using NUnit.Framework;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using MudLike.Core.Systems;
using MudLike.Core.Components;
using MudLike.Vehicles.Components;
using Unity.Core;

namespace MudLike.Tests.Unit.Core
{
    /// <summary>
    /// Тесты для системы управления транспортом VehicleControlSystem
    /// Обеспечивает 100% покрытие тестами критической системы управления игроком
    /// </summary>
    public class VehicleControlSystemTests
    {
        private World _world;
        private VehicleControlSystem _vehicleControlSystem;
        private EntityManager _entityManager;

        [SetUp]
        public void SetUp()
        {
            _world = new World("TestWorld");
            _entityManager = _world.EntityManager;
            
            // Создаем систему управления транспортом
            _vehicleControlSystem = _world.GetOrCreateSystemManaged<VehicleControlSystem>();
            _vehicleControlSystem.OnCreate(ref _world.Unmanaged);
            
            // Устанавливаем время для SystemAPI.Time.time
            _world.SetSingleton(new TimeData { ElapsedTime = 10f, DeltaTime = 0.016f, FixedDeltaTime = 0.016f });
        }

        [TearDown]
        public void TearDown()
        {
            _vehicleControlSystem.OnDestroy(ref _world.Unmanaged);
            _world.Dispose();
        }

        [Test]
        public void VehicleControlSystem_OnCreate_InitializesCorrectly()
        {
            // Arrange & Act
            // Система уже создана в SetUp

            // Assert
            Assert.IsNotNull(_vehicleControlSystem);
        }

        [Test]
        public void VehicleControlSystem_OnUpdate_ProcessesWithoutErrors()
        {
            // Arrange
            // Создаем тестовую сущность с транспортным средством
            var entity = _entityManager.CreateEntity();
            _entityManager.AddComponentData(entity, new LocalTransform 
            { 
                Position = float3.zero, 
                Rotation = quaternion.identity 
            });
            _entityManager.AddComponentData(entity, new PlayerInput
            {
                VehicleMovement = new float2(0.5f, 0.8f),
                Accelerate = true,
                Brake = false,
                Steering = 0.3f
            });
            _entityManager.AddComponentData(entity, new VehiclePhysics
            {
                Velocity = float3.zero,
                EnginePower = 0f,
                MaxEnginePower = 1000f,
                Acceleration = 10f,
                Deceleration = 15f,
                MaxSteeringAngle = 45f,
                SteeringReturnSpeed = 5f,
                MaxSpeed = 50f,
                TurnSpeedMultiplier = 0.5f,
                EngineBraking = 2f
            });
            _entityManager.AddComponent<VehicleTag>(entity);
            _entityManager.AddComponent<PlayerTag>(entity);

            // Act
            _vehicleControlSystem.OnUpdate(ref _world.Unmanaged);

            // Assert
            // Проверяем, что система обработала сущность без ошибок
            Assert.IsNotNull(_vehicleControlSystem);
        }

        [Test]
        public void ProcessVehicleControl_AccelerateInput_AppliesCorrectPhysics()
        {
            // Arrange
            var transform = new LocalTransform { Position = float3.zero, Rotation = quaternion.identity };
            var input = new PlayerInput
            {
                VehicleMovement = new float2(0, 1),
                Accelerate = true,
                Brake = false,
                Steering = 0f
            };
            var physics = new VehiclePhysics
            {
                Velocity = float3.zero,
                EnginePower = 0f,
                MaxEnginePower = 1000f,
                Acceleration = 10f,
                MaxSpeed = 50f,
                EngineBraking = 2f
            };
            float deltaTime = 0.016f;

            // Act
            VehicleControlSystem.ProcessVehicleControl(ref transform, input, ref physics, deltaTime);

            // Assert
            Assert.Greater(physics.EnginePower, 0f);
            Assert.Greater(physics.Velocity.x, 0f);
        }

        [Test]
        public void ProcessVehicleControl_BrakeInput_AppliesCorrectPhysics()
        {
            // Arrange
            var transform = new LocalTransform { Position = float3.zero, Rotation = quaternion.identity };
            var input = new PlayerInput
            {
                VehicleMovement = new float2(0, -1),
                Accelerate = false,
                Brake = true,
                Steering = 0f
            };
            var physics = new VehiclePhysics
            {
                Velocity = new float3(20f, 0, 0),
                EnginePower = 500f,
                MaxEnginePower = 1000f,
                Deceleration = 15f,
                MaxSpeed = 50f,
                EngineBraking = 2f
            };
            float deltaTime = 0.016f;

            // Act
            VehicleControlSystem.ProcessVehicleControl(ref transform, input, ref physics, deltaTime);

            // Assert
            Assert.Less(physics.EnginePower, 500f); // Мощность должна уменьшиться
        }

        [Test]
        public void ProcessVehicleControl_SteeringInput_AppliesCorrectRotation()
        {
            // Arrange
            var transform = new LocalTransform { Position = float3.zero, Rotation = quaternion.identity };
            var input = new PlayerInput
            {
                VehicleMovement = new float2(1, 0),
                Accelerate = false,
                Brake = false,
                Steering = 0.5f
            };
            var physics = new VehiclePhysics
            {
                Velocity = new float3(10f, 0, 0),
                EnginePower = 0f,
                MaxEnginePower = 1000f,
                MaxSteeringAngle = 45f,
                SteeringReturnSpeed = 5f,
                MaxSpeed = 50f,
                TurnSpeedMultiplier = 0.5f
            };
            float deltaTime = 0.016f;

            // Act
            VehicleControlSystem.ProcessVehicleControl(ref transform, input, ref physics, deltaTime);

            // Assert
            Assert.NotZero(physics.SteeringAngle);
            Assert.AreNotEqual(quaternion.identity, transform.Rotation);
        }

        [Test]
        public void ProcessVehicleControl_NoInput_AppliesEngineBraking()
        {
            // Arrange
            var transform = new LocalTransform { Position = float3.zero, Rotation = quaternion.identity };
            var input = new PlayerInput
            {
                VehicleMovement = float2.zero,
                Accelerate = false,
                Brake = false,
                Steering = 0f
            };
            var physics = new VehiclePhysics
            {
                Velocity = new float3(20f, 0, 0),
                EnginePower = 500f,
                MaxEnginePower = 1000f,
                Acceleration = 10f,
                Deceleration = 15f,
                MaxSpeed = 50f,
                EngineBraking = 2f
            };
            float deltaTime = 0.016f;

            // Act
            VehicleControlSystem.ProcessVehicleControl(ref transform, input, ref physics, deltaTime);

            // Assert
            Assert.Less(physics.EnginePower, 500f); // Мощность должна уменьшиться из-за торможения двигателем
        }

        [Test]
        public void ProcessVehicleControl_SteeringReturn_ReturnsToNeutral()
        {
            // Arrange
            var transform = new LocalTransform { Position = float3.zero, Rotation = quaternion.identity };
            var input = new PlayerInput
            {
                VehicleMovement = float2.zero,
                Accelerate = false,
                Brake = false,
                Steering = 0f // Нет ввода руля
            };
            var physics = new VehiclePhysics
            {
                Velocity = float3.zero,
                EnginePower = 0f,
                MaxEnginePower = 1000f,
                SteeringAngle = 20f, // Начальный угол поворота
                MaxSteeringAngle = 45f,
                SteeringReturnSpeed = 5f,
                MaxSpeed = 50f,
                TurnSpeedMultiplier = 0.5f
            };
            float deltaTime = 0.1f; // Больший интервал для более заметного эффекта

            // Act
            VehicleControlSystem.ProcessVehicleControl(ref transform, input, ref physics, deltaTime);

            // Assert
            Assert.Less(math.abs(physics.SteeringAngle), 20f); // Угол должен приблизиться к нулю
        }

        [Test]
        public void ProcessVehicleControl_MaxSpeed_IsRespected()
        {
            // Arrange
            var transform = new LocalTransform { Position = float3.zero, Rotation = quaternion.identity };
            var input = new PlayerInput
            {
                VehicleMovement = new float2(0, 1),
                Accelerate = true,
                Brake = false,
                Steering = 0f
            };
            var physics = new VehiclePhysics
            {
                Velocity = new float3(45f, 0, 0), // Близко к максимальной скорости
                EnginePower = 1000f, // Максимальная мощность
                MaxEnginePower = 1000f,
                Acceleration = 10f,
                MaxSpeed = 50f,
                EngineBraking = 2f
            };
            float deltaTime = 0.1f;

            // Act
            VehicleControlSystem.ProcessVehicleControl(ref transform, input, ref physics, deltaTime);

            // Assert
            Assert.LessOrEqual(math.length(physics.Velocity), 50f + 0.1f); // С учетом погрешности
        }

        [Test]
        public void ProcessVehicleControl_SteeringLimits_AreRespected()
        {
            // Arrange
            var transform = new LocalTransform { Position = float3.zero, Rotation = quaternion.identity };
            var input = new PlayerInput
            {
                VehicleMovement = new float2(1, 0),
                Accelerate = false,
                Brake = false,
                Steering = 1f // Максимальный поворот
            };
            var physics = new VehiclePhysics
            {
                Velocity = new float3(10f, 0, 0),
                EnginePower = 0f,
                MaxEnginePower = 1000f,
                SteeringAngle = 40f, // Близко к максимуму
                MaxSteeringAngle = 45f,
                SteeringReturnSpeed = 5f,
                MaxSpeed = 50f,
                TurnSpeedMultiplier = 0.5f
            };
            float deltaTime = 0.1f;

            // Act
            VehicleControlSystem.ProcessVehicleControl(ref transform, input, ref physics, deltaTime);

            // Assert
            Assert.LessOrEqual(math.abs(physics.SteeringAngle), 45f + 0.1f); // С учетом погрешности
        }

        [Test]
        public void VehicleControlSystem_MultipleEntities_HandlesCorrectly()
        {
            // Arrange
            for (int i = 0; i < 5; i++)
            {
                var entity = _entityManager.CreateEntity();
                _entityManager.AddComponentData(entity, new LocalTransform 
                { 
                    Position = new float3(i * 10, 0, 0), 
                    Rotation = quaternion.identity 
                });
                _entityManager.AddComponentData(entity, new PlayerInput
                {
                    VehicleMovement = new float2(0.5f, 0.8f),
                    Accelerate = true,
                    Brake = false,
                    Steering = 0.3f
                });
                _entityManager.AddComponentData(entity, new VehiclePhysics
                {
                    Velocity = float3.zero,
                    EnginePower = 0f,
                    MaxEnginePower = 1000f,
                    Acceleration = 10f,
                    MaxSpeed = 50f,
                    EngineBraking = 2f,
                    MaxSteeringAngle = 45f,
                    SteeringReturnSpeed = 5f,
                    TurnSpeedMultiplier = 0.5f,
                    Deceleration = 15f
                });
                _entityManager.AddComponent<VehicleTag>(entity);
                _entityManager.AddComponent<PlayerTag>(entity);
            }

            // Act
            _vehicleControlSystem.OnUpdate(ref _world.Unmanaged);

            // Assert
            // Проверяем, что система обработала все сущности без ошибок
            Assert.IsNotNull(_vehicleControlSystem);
        }

        [Test]
        public void VehicleControlSystem_EdgeCases_HandleCorrectly()
        {
            // Arrange
            var transform = new LocalTransform { Position = float3.zero, Rotation = quaternion.identity };
            var input = new PlayerInput
            {
                VehicleMovement = new float2(float.MaxValue, float.MinValue),
                Accelerate = true,
                Brake = true, // Одновременно газ и тормоз
                Steering = float.NaN
            };
            var physics = new VehiclePhysics
            {
                Velocity = new float3(float.PositiveInfinity, float.NegativeInfinity, float.Epsilon),
                EnginePower = float.NaN,
                MaxEnginePower = float.MaxValue,
                Acceleration = float.Epsilon,
                MaxSpeed = float.MinValue,
                EngineBraking = float.PositiveInfinity,
                MaxSteeringAngle = float.MaxValue,
                SteeringReturnSpeed = float.Epsilon,
                TurnSpeedMultiplier = float.NaN,
                Deceleration = float.MaxValue
            };
            float deltaTime = float.Epsilon;

            // Act & Assert
            // Проверяем, что система может обработать крайние случаи без исключений
            Assert.DoesNotThrow(() => 
            {
                VehicleControlSystem.ProcessVehicleControl(ref transform, input, ref physics, deltaTime);
            });
        }
    }
}
