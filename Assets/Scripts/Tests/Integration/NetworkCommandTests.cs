using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.NetCode;
using Unity.Collections;
using NUnit.Framework;
using MudLike.Networking.Components;
using MudLike.Core.Components;
using MudLike.Vehicles.Components;
using static MudLike.Core.Components.Position;

namespace MudLike.Tests.Integration
{
    /// <summary>
    /// Интеграционные тесты для сетевых команд в NFE
    /// Тестирует создание, отправку и обработку сетевых команд
    /// </summary>
    [TestFixture]
    public class NetworkCommandTests : MudLikeTestFixture
    {
        private Entity _clientEntity;
        private Entity _serverEntity;
        private Entity _commandEntity;
        
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            
            _clientEntity = CreateClientEntity();
            _serverEntity = CreateServerEntity();
            _commandEntity = CreateCommandEntity();
        }
        
        #region Тесты базовых команд
        
        /// <summary>
        /// Тест создания команды движения
        /// </summary>
        [Test]
        public void CreateMoveCommand_ShouldSetCorrectValues()
        {
            // Arrange
            var targetPosition = new float3(100f, 0f, 200f);
            var targetRotation = quaternion.RotateY(math.radians(45f));
            
            // Act
            var command = new MoveCommand
            {
                Target = _clientEntity,
                Position = targetPosition,
                Rotation = targetRotation,
                Timestamp = (float)Time.time
            };
            
            EntityManager.AddComponentData(_commandEntity, command);
            
            // Assert
            var actualCommand = EntityManager.GetComponentData<MoveCommand>(_commandEntity);
            Assert.AreEqual(_clientEntity, actualCommand.Target);
            Assert.AreEqual(targetPosition, actualCommand.Position);
            Assert.AreEqual(targetRotation, actualCommand.Rotation);
            Assert.IsTrue(actualCommand.Timestamp > 0f);
        }
        
        /// <summary>
        /// Тест создания команды изменения скорости
        /// </summary>
        [Test]
        public void CreateVelocityCommand_ShouldSetCorrectValues()
        {
            // Arrange
            var targetVelocity = new float3(10f, 0f, 5f);
            var targetAngularVelocity = new float3(0f, 2f, 0f);
            
            // Act
            var command = new VelocityCommand
            {
                Target = _clientEntity,
                Velocity = targetVelocity,
                AngularVelocity = targetAngularVelocity,
                Timestamp = (float)Time.time
            };
            
            EntityManager.AddComponentData(_commandEntity, command);
            
            // Assert
            var actualCommand = EntityManager.GetComponentData<VelocityCommand>(_commandEntity);
            Assert.AreEqual(_clientEntity, actualCommand.Target);
            Assert.AreEqual(targetVelocity, actualCommand.Velocity);
            Assert.AreEqual(targetAngularVelocity, actualCommand.AngularVelocity);
        }
        
        /// <summary>
        /// Тест создания команды управления транспортом
        /// </summary>
        [Test]
        public void CreateVehicleControlCommand_ShouldSetCorrectValues()
        {
            // Arrange
            var input = new VehicleInput
            {
                Vertical = 1f,
                Horizontal = 0.5f,
                Brake = false,
                Accelerate = true
            };
            
            // Act
            var command = new VehicleControlCommand
            {
                Target = _clientEntity,
                Input = input,
                Timestamp = (float)Time.time
            };
            
            EntityManager.AddComponentData(_commandEntity, command);
            
            // Assert
            var actualCommand = EntityManager.GetComponentData<VehicleControlCommand>(_commandEntity);
            Assert.AreEqual(_clientEntity, actualCommand.Target);
            Assert.AreEqual(input.Vertical, actualCommand.Input.Vertical);
            Assert.AreEqual(input.Horizontal, actualCommand.Input.Horizontal);
            Assert.AreEqual(input.Brake, actualCommand.Input.Brake);
            Assert.AreEqual(input.Accelerate, actualCommand.Input.Accelerate);
        }
        
        #endregion
        
        #region Тесты обработки команд
        
        /// <summary>
        /// Тест обработки команды движения
        /// </summary>
        [Test]
        public void ProcessMoveCommand_ShouldUpdatePosition()
        {
            // Arrange
            var command = new MoveCommand
            {
                Target = _clientEntity,
                Position = new float3(50f, 0f, 100f),
                Rotation = quaternion.RotateY(math.radians(90f)),
                Timestamp = (float)Time.time
            };
            
            EntityManager.AddComponentData(_commandEntity, command);
            
            // Act
            ProcessCommand(command);
            
            // Assert
            var transform = EntityManager.GetComponentData<LocalTransform>(_clientEntity);
            Assert.AreEqual(command.Position, transform.Position);
            Assert.AreEqual(command.Rotation, transform.Rotation);
        }
        
        /// <summary>
        /// Тест обработки команды изменения скорости
        /// </summary>
        [Test]
        public void ProcessVelocityCommand_ShouldUpdateVelocity()
        {
            // Arrange
            var command = new VelocityCommand
            {
                Target = _clientEntity,
                Velocity = new float3(15f, 0f, 10f),
                AngularVelocity = new float3(0f, 3f, 0f),
                Timestamp = (float)Time.time
            };
            
            EntityManager.AddComponentData(_commandEntity, command);
            
            // Act
            ProcessCommand(command);
            
            // Assert
            var networkPosition = EntityManager.GetComponentData<NetworkPosition>(_clientEntity);
            Assert.AreEqual(command.Velocity, networkPosition.Velocity);
            Assert.AreEqual(command.AngularVelocity, networkPosition.AngularVelocity);
        }
        
        /// <summary>
        /// Тест обработки команды управления транспортом
        /// </summary>
        [Test]
        public void ProcessVehicleControlCommand_ShouldUpdateInput()
        {
            // Arrange
            var input = new VehicleInput
            {
                Vertical = 0.8f,
                Horizontal = -0.3f,
                Brake = true,
                Accelerate = false
            };
            
            var command = new VehicleControlCommand
            {
                Target = _clientEntity,
                Input = input,
                Timestamp = (float)Time.time
            };
            
            EntityManager.AddComponentData(_commandEntity, command);
            
            // Act
            ProcessCommand(command);
            
            // Assert
            var actualInput = EntityManager.GetComponentData<VehicleInput>(_clientEntity);
            Assert.AreEqual(input.Vertical, actualInput.Vertical);
            Assert.AreEqual(input.Horizontal, actualInput.Horizontal);
            Assert.AreEqual(input.Brake, actualInput.Brake);
            Assert.AreEqual(input.Accelerate, actualInput.Accelerate);
        }
        
        #endregion
        
        #region Тесты множественных команд
        
        /// <summary>
        /// Тест обработки множественных команд
        /// </summary>
        [Test]
        public void ProcessMultipleCommands_ShouldHandleAll()
        {
            // Arrange
            var entities = new Entity[5];
            var commands = new MoveCommand[5];
            
            for (int i = 0; i < 5; i++)
            {
                entities[i] = CreateClientEntity();
                commands[i] = new MoveCommand
                {
                    Target = entities[i],
                    Position = new float3(i * 10f, 0f, i * 20f),
                    Rotation = quaternion.RotateY(math.radians(i * 30f)),
                    Timestamp = (float)Time.time
                };
                
                EntityManager.AddComponentData(entities[i], commands[i]);
            }
            
            // Act
            for (int i = 0; i < 5; i++)
            {
                ProcessCommand(commands[i]);
            }
            
            // Assert
            for (int i = 0; i < 5; i++)
            {
                var transform = EntityManager.GetComponentData<LocalTransform>(entities[i]);
                Assert.AreEqual(commands[i].Position, transform.Position);
                Assert.AreEqual(commands[i].Rotation, transform.Rotation);
            }
        }
        
        /// <summary>
        /// Тест обработки команд с различными типами
        /// </summary>
        [Test]
        public void ProcessDifferentCommandTypes_ShouldHandleAll()
        {
            // Arrange
            var moveCommand = new MoveCommand
            {
                Target = _clientEntity,
                Position = new float3(100f, 0f, 200f),
                Rotation = quaternion.identity,
                Timestamp = (float)Time.time
            };
            
            var velocityCommand = new VelocityCommand
            {
                Target = _clientEntity,
                Velocity = new float3(5f, 0f, 3f),
                AngularVelocity = float3.zero,
                Timestamp = (float)Time.time
            };
            
            // Act
            ProcessCommand(moveCommand);
            ProcessCommand(velocityCommand);
            
            // Assert
            var transform = EntityManager.GetComponentData<LocalTransform>(_clientEntity);
            var networkPosition = EntityManager.GetComponentData<NetworkPosition>(_clientEntity);
            
            Assert.AreEqual(moveCommand.Position, transform.Position);
            Assert.AreEqual(velocityCommand.Velocity, networkPosition.Velocity);
        }
        
        #endregion
        
        #region Тесты валидации команд
        
        /// <summary>
        /// Тест валидации команды с недопустимыми значениями
        /// </summary>
        [Test]
        public void ValidateCommand_WithInvalidValues_ShouldReject()
        {
            // Arrange
            var invalidCommand = new MoveCommand
            {
                Target = _clientEntity,
                Position = new float3(float.PositiveInfinity, 0f, 0f), // Недопустимое значение
                Rotation = quaternion.identity,
                Timestamp = (float)Time.time
            };
            
            // Act
            bool isValid = ValidateCommand(invalidCommand);
            
            // Assert
            Assert.IsFalse(isValid);
        }
        
        /// <summary>
        /// Тест валидации команды с устаревшим временем
        /// </summary>
        [Test]
        public void ValidateCommand_WithOldTimestamp_ShouldReject()
        {
            // Arrange
            var oldCommand = new MoveCommand
            {
                Target = _clientEntity,
                Position = new float3(50f, 0f, 100f),
                Rotation = quaternion.identity,
                Timestamp = (float)Time.time - 10f // 10 секунд назад
            };
            
            // Act
            bool isValid = ValidateCommand(oldCommand);
            
            // Assert
            Assert.IsFalse(isValid);
        }
        
        /// <summary>
        /// Тест валидации команды с несуществующей целью
        /// </summary>
        [Test]
        public void ValidateCommand_WithInvalidTarget_ShouldReject()
        {
            // Arrange
            var invalidTargetCommand = new MoveCommand
            {
                Target = Entity.Null, // Несуществующая цель
                Position = new float3(50f, 0f, 100f),
                Rotation = quaternion.identity,
                Timestamp = (float)Time.time
            };
            
            // Act
            bool isValid = ValidateCommand(invalidTargetCommand);
            
            // Assert
            Assert.IsFalse(isValid);
        }
        
        #endregion
        
        #region Тесты производительности команд
        
        /// <summary>
        /// Тест производительности обработки большого количества команд
        /// </summary>
        [Test]
        public void ProcessLargeNumberOfCommands_ShouldCompleteInReasonableTime()
        {
            // Arrange
            const int commandCount = 1000;
            var commands = new MoveCommand[commandCount];
            
            for (int i = 0; i < commandCount; i++)
            {
                commands[i] = new MoveCommand
                {
                    Target = _clientEntity,
                    Position = new float3(i, 0f, i),
                    Rotation = quaternion.identity,
                    Timestamp = (float)Time.time
                };
            }
            
            // Act
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < commandCount; i++)
            {
                ProcessCommand(commands[i]);
            }
            stopwatch.Stop();
            
            // Assert
            Assert.IsTrue(stopwatch.ElapsedMilliseconds < 500, 
                $"Обработка {commandCount} команд заняла {stopwatch.ElapsedMilliseconds}мс, что превышает 500мс");
        }
        
        /// <summary>
        /// Тест производительности с частыми командами
        /// </summary>
        [Test]
        public void ProcessFrequentCommands_ShouldMaintainPerformance()
        {
            // Arrange
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            
            // Act - выполняем множество команд
            for (int i = 0; i < 100; i++)
            {
                var command = new MoveCommand
                {
                    Target = _clientEntity,
                    Position = new float3(i, 0f, i),
                    Rotation = quaternion.RotateY(math.radians(i)),
                    Timestamp = (float)Time.time
                };
                
                ProcessCommand(command);
            }
            
            stopwatch.Stop();
            
            // Assert
            Assert.IsTrue(stopwatch.ElapsedMilliseconds < 100, 
                $"100 команд заняли {stopwatch.ElapsedMilliseconds}мс, что превышает 100мс");
        }
        
        #endregion
        
        #region Вспомогательные методы
        
        /// <summary>
        /// Создает клиентскую сущность
        /// </summary>
        private Entity CreateClientEntity()
        {
            var entity = EntityManager.CreateEntity();
            EntityManager.AddComponent<NetworkId>(entity);
            EntityManager.AddComponent<NetworkPosition>(entity);
            EntityManager.AddComponent<LocalTransform>(entity);
            EntityManager.AddComponent<PlayerTag>(entity);
            EntityManager.AddComponent<VehicleInput>(entity);
            return entity;
        }
        
        /// <summary>
        /// Создает серверную сущность
        /// </summary>
        private Entity CreateServerEntity()
        {
            var entity = EntityManager.CreateEntity();
            EntityManager.AddComponent<NetworkId>(entity);
            EntityManager.AddComponent<NetworkPosition>(entity);
            EntityManager.AddComponent<LocalTransform>(entity);
            EntityManager.AddComponent<ServerTag>(entity);
            return entity;
        }
        
        /// <summary>
        /// Создает сущность для команд
        /// </summary>
        private Entity CreateCommandEntity()
        {
            var entity = EntityManager.CreateEntity();
            EntityManager.AddComponent<NetworkId>(entity);
            return entity;
        }
        
        /// <summary>
        /// Обрабатывает команду движения
        /// </summary>
        private void ProcessCommand(MoveCommand command)
        {
            if (!ValidateCommand(command)) return;
            
            var transform = EntityManager.GetComponentData<LocalTransform>(command.Target);
            transform.Position = command.Position;
            transform.Rotation = command.Rotation;
            EntityManager.SetComponentData(command.Target, transform);
            
            // Обновляем сетевую позицию
            var networkPosition = EntityManager.GetComponentData<NetworkPosition>(command.Target);
            networkPosition.Value = command.Position;
            networkPosition.Rotation = command.Rotation;
            networkPosition.HasChanged = true;
            networkPosition.LastUpdateTime = command.Timestamp;
            EntityManager.SetComponentData(command.Target, networkPosition);
        }
        
        /// <summary>
        /// Обрабатывает команду изменения скорости
        /// </summary>
        private void ProcessCommand(VelocityCommand command)
        {
            if (!ValidateCommand(command)) return;
            
            var networkPosition = EntityManager.GetComponentData<NetworkPosition>(command.Target);
            networkPosition.Velocity = command.Velocity;
            networkPosition.AngularVelocity = command.AngularVelocity;
            networkPosition.HasChanged = true;
            networkPosition.LastUpdateTime = command.Timestamp;
            EntityManager.SetComponentData(command.Target, networkPosition);
        }
        
        /// <summary>
        /// Обрабатывает команду управления транспортом
        /// </summary>
        private void ProcessCommand(VehicleControlCommand command)
        {
            if (!ValidateCommand(command)) return;
            
            EntityManager.SetComponentData(command.Target, command.Input);
        }
        
        /// <summary>
        /// Валидирует команду
        /// </summary>
        private bool ValidateCommand(MoveCommand command)
        {
            // Проверяем, что цель существует
            if (command.Target == Entity.Null) return false;
            if (!EntityManager.Exists(command.Target)) return false;
            
            // Проверяем, что позиция валидна
            if (float.IsNaN(command.Position.x) || float.IsInfinity(command.Position.x)) return false;
            if (float.IsNaN(command.Position.y) || float.IsInfinity(command.Position.y)) return false;
            if (float.IsNaN(command.Position.z) || float.IsInfinity(command.Position.z)) return false;
            
            // Проверяем, что время не устарело
            if (command.Timestamp < (float)Time.time - 5f) return false;
            
            return true;
        }
        
        /// <summary>
        /// Валидирует команду изменения скорости
        /// </summary>
        private bool ValidateCommand(VelocityCommand command)
        {
            if (command.Target == Entity.Null) return false;
            if (!EntityManager.Exists(command.Target)) return false;
            if (command.Timestamp < (float)Time.time - 5f) return false;
            
            return true;
        }
        
        /// <summary>
        /// Валидирует команду управления транспортом
        /// </summary>
        private bool ValidateCommand(VehicleControlCommand command)
        {
            if (command.Target == Entity.Null) return false;
            if (!EntityManager.Exists(command.Target)) return false;
            if (command.Timestamp < (float)Time.time - 5f) return false;
            
            return true;
        }
        
        #endregion
    }
    
    /// <summary>
    /// Команда движения для тестирования
    /// </summary>
    public struct MoveCommand : IComponentData
    {
        public Entity Target;
        public float3 Position;
        public quaternion Rotation;
        public float Timestamp;
    }
    
    /// <summary>
    /// Команда изменения скорости для тестирования
    /// </summary>
    public struct VelocityCommand : IComponentData
    {
        public Entity Target;
        public float3 Velocity;
        public float3 AngularVelocity;
        public float Timestamp;
    }
    
    /// <summary>
    /// Команда управления транспортом для тестирования
    /// </summary>
    public struct VehicleControlCommand : IComponentData
    {
        public Entity Target;
        public VehicleInput Input;
        public float Timestamp;
    }
    
    /// <summary>
    /// Тег сервера для тестирования
    /// </summary>
    public struct ServerTag : IComponentData
    {
    }
}
