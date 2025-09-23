using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.NetCode;
using Unity.Collections;
using NUnit.Framework;
using MudLike.Networking.Components;
using MudLike.Core.Components;
using MudLike.Networking.Systems;
using static MudLike.Core.Components.Position;

namespace MudLike.Tests.Integration
{
    /// <summary>
    /// Интеграционные тесты для компенсации задержек в NFE
    /// Тестирует системы компенсации задержек и предсказания позиций
    /// </summary>
    [TestFixture]
    public class LagCompensationTests : MudLikeTestFixture
    {
        private Entity _clientEntity;
        private Entity _serverEntity;
        private LagCompensationSystem _lagCompensationSystem;
        
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            
            _lagCompensationSystem = World.GetOrCreateSystemManaged<LagCompensationSystem>();
            _clientEntity = CreateClientEntity();
            _serverEntity = CreateServerEntity();
        }
        
        #region Тесты базовой компенсации задержек
        
        /// <summary>
        /// Тест компенсации задержек для позиций
        /// </summary>
        [Test]
        public void CompensateLag_ForPosition_ShouldCompensateCorrectly()
        {
            // Arrange
            var networkPosition = new NetworkPosition
            {
                Value = new float3(0f, 0f, 0f),
                Velocity = new float3(10f, 0f, 5f),
                LastUpdateTime = (float)Time.time - 0.1f, // 100мс задержка
                HasChanged = true
            };
            
            EntityManager.SetComponentData(_clientEntity, networkPosition);
            
            // Act
            _lagCompensationSystem.Update();
            
            // Assert
            var compensatedPosition = EntityManager.GetComponentData<NetworkPosition>(_clientEntity);
            // Позиция должна быть скомпенсирована на основе скорости и задержки
            Assert.IsTrue(compensatedPosition.Value.x > 0f);
            Assert.IsTrue(compensatedPosition.Value.z > 0f);
        }
        
        /// <summary>
        /// Тест компенсации задержек для поворота
        /// </summary>
        [Test]
        public void CompensateLag_ForRotation_ShouldCompensateCorrectly()
        {
            // Arrange
            var networkPosition = new NetworkPosition
            {
                Value = new float3(0f, 0f, 0f),
                Rotation = quaternion.identity,
                AngularVelocity = new float3(0f, 2f, 0f), // Поворот вокруг Y
                LastUpdateTime = (float)Time.time - 0.05f, // 50мс задержка
                HasChanged = true
            };
            
            EntityManager.SetComponentData(_clientEntity, networkPosition);
            
            // Act
            _lagCompensationSystem.Update();
            
            // Assert
            var compensatedPosition = EntityManager.GetComponentData<NetworkPosition>(_clientEntity);
            // Поворот должен быть скомпенсирован
            Assert.IsFalse(compensatedPosition.Rotation.Equals(quaternion.identity));
        }
        
        /// <summary>
        /// Тест компенсации задержек с различными RTT
        /// </summary>
        [Test]
        public void CompensateLag_WithDifferentRTT_ShouldCompensateDifferently()
        {
            // Arrange
            var entities = new Entity[3];
            var rttValues = new float[] { 0.05f, 0.1f, 0.2f }; // 50мс, 100мс, 200мс
            
            for (int i = 0; i < 3; i++)
            {
                entities[i] = CreateClientEntity();
                var networkPosition = new NetworkPosition
                {
                    Value = new float3(0f, 0f, 0f),
                    Velocity = new float3(10f, 0f, 0f),
                    LastUpdateTime = (float)Time.time - rttValues[i],
                    HasChanged = true
                };
                
                EntityManager.SetComponentData(entities[i], networkPosition);
            }
            
            // Act
            _lagCompensationSystem.Update();
            
            // Assert
            var compensatedPositions = new NetworkPosition[3];
            for (int i = 0; i < 3; i++)
            {
                compensatedPositions[i] = EntityManager.GetComponentData<NetworkPosition>(entities[i]);
            }
            
            // Больший RTT должен давать большую компенсацию
            Assert.IsTrue(compensatedPositions[2].Value.x > compensatedPositions[1].Value.x);
            Assert.IsTrue(compensatedPositions[1].Value.x > compensatedPositions[0].Value.x);
        }
        
        #endregion
        
        #region Тесты предсказания позиций
        
        /// <summary>
        /// Тест предсказания позиций на основе скорости
        /// </summary>
        [Test]
        public void PredictPosition_WithVelocity_ShouldPredictCorrectly()
        {
            // Arrange
            var networkPosition = new NetworkPosition
            {
                Value = new float3(0f, 0f, 0f),
                Velocity = new float3(5f, 0f, 3f),
                LastUpdateTime = (float)Time.time,
                HasChanged = true
            };
            
            EntityManager.SetComponentData(_clientEntity, networkPosition);
            
            // Act
            _lagCompensationSystem.Update();
            
            // Assert
            var predictedPosition = EntityManager.GetComponentData<NetworkPosition>(_clientEntity);
            // Позиция должна быть предсказана на основе скорости
            Assert.IsTrue(predictedPosition.Value.x > 0f);
            Assert.IsTrue(predictedPosition.Value.z > 0f);
        }
        
        /// <summary>
        /// Тест предсказания позиций с ускорением
        /// </summary>
        [Test]
        public void PredictPosition_WithAcceleration_ShouldPredictCorrectly()
        {
            // Arrange
            var networkPosition = new NetworkPosition
            {
                Value = new float3(0f, 0f, 0f),
                Velocity = new float3(0f, 0f, 0f),
                AngularVelocity = new float3(0f, 0f, 0f),
                LastUpdateTime = (float)Time.time,
                HasChanged = true
            };
            
            // Добавляем компонент ускорения
            EntityManager.AddComponent<AccelerationData>(_clientEntity);
            var acceleration = new AccelerationData
            {
                Linear = new float3(2f, 0f, 1f),
                Angular = new float3(0f, 1f, 0f)
            };
            EntityManager.SetComponentData(_clientEntity, acceleration);
            EntityManager.SetComponentData(_clientEntity, networkPosition);
            
            // Act
            _lagCompensationSystem.Update();
            
            // Assert
            var predictedPosition = EntityManager.GetComponentData<NetworkPosition>(_clientEntity);
            // Позиция должна быть предсказана с учетом ускорения
            Assert.IsTrue(predictedPosition.Value.x > 0f);
            Assert.IsTrue(predictedPosition.Value.z > 0f);
        }
        
        /// <summary>
        /// Тест предсказания позиций с ограничениями
        /// </summary>
        [Test]
        public void PredictPosition_WithConstraints_ShouldRespectLimits()
        {
            // Arrange
            var networkPosition = new NetworkPosition
            {
                Value = new float3(0f, 0f, 0f),
                Velocity = new float3(1000f, 0f, 0f), // Очень высокая скорость
                LastUpdateTime = (float)Time.time,
                HasChanged = true
            };
            
            EntityManager.SetComponentData(_clientEntity, networkPosition);
            
            // Act
            _lagCompensationSystem.Update();
            
            // Assert
            var predictedPosition = EntityManager.GetComponentData<NetworkPosition>(_clientEntity);
            // Позиция должна быть ограничена разумными пределами
            Assert.IsTrue(predictedPosition.Value.x < 100f); // Максимальное предсказание
        }
        
        #endregion
        
        #region Тесты интерполяции
        
        /// <summary>
        /// Тест интерполяции между позициями
        /// </summary>
        [Test]
        public void InterpolatePosition_BetweenTwoPoints_ShouldInterpolateCorrectly()
        {
            // Arrange
            var startPosition = new float3(0f, 0f, 0f);
            var endPosition = new float3(100f, 0f, 0f);
            
            var networkPosition = new NetworkPosition
            {
                Value = startPosition,
                LastUpdateTime = (float)Time.time - 0.1f,
                HasChanged = true
            };
            
            EntityManager.SetComponentData(_clientEntity, networkPosition);
            
            // Симулируем получение новой позиции
            var newNetworkPosition = new NetworkPosition
            {
                Value = endPosition,
                LastUpdateTime = (float)Time.time,
                HasChanged = true
            };
            
            // Act
            _lagCompensationSystem.Update();
            
            // Assert
            var interpolatedPosition = EntityManager.GetComponentData<NetworkPosition>(_clientEntity);
            // Позиция должна быть интерполирована между начальной и конечной
            Assert.IsTrue(interpolatedPosition.Value.x > startPosition.x);
            Assert.IsTrue(interpolatedPosition.Value.x < endPosition.x);
        }
        
        /// <summary>
        /// Тест интерполяции с различными скоростями
        /// </summary>
        [Test]
        public void InterpolatePosition_WithDifferentSpeeds_ShouldInterpolateDifferently()
        {
            // Arrange
            var slowPosition = new NetworkPosition
            {
                Value = new float3(0f, 0f, 0f),
                Velocity = new float3(1f, 0f, 0f), // Медленно
                LastUpdateTime = (float)Time.time - 0.1f,
                HasChanged = true
            };
            
            var fastPosition = new NetworkPosition
            {
                Value = new float3(0f, 0f, 0f),
                Velocity = new float3(10f, 0f, 0f), // Быстро
                LastUpdateTime = (float)Time.time - 0.1f,
                HasChanged = true
            };
            
            var slowEntity = CreateClientEntity();
            var fastEntity = CreateClientEntity();
            
            EntityManager.SetComponentData(slowEntity, slowPosition);
            EntityManager.SetComponentData(fastEntity, fastPosition);
            
            // Act
            _lagCompensationSystem.Update();
            
            // Assert
            var slowResult = EntityManager.GetComponentData<NetworkPosition>(slowEntity);
            var fastResult = EntityManager.GetComponentData<NetworkPosition>(fastEntity);
            
            // Быстрая сущность должна иметь большую компенсацию
            Assert.IsTrue(fastResult.Value.x > slowResult.Value.x);
        }
        
        #endregion
        
        #region Тесты производительности компенсации
        
        /// <summary>
        /// Тест производительности компенсации для большого количества сущностей
        /// </summary>
        [Test]
        public void CompensateLag_LargeNumberOfEntities_ShouldCompleteInReasonableTime()
        {
            // Arrange
            const int entityCount = 1000;
            var entities = new Entity[entityCount];
            
            for (int i = 0; i < entityCount; i++)
            {
                entities[i] = CreateClientEntity();
                var networkPosition = new NetworkPosition
                {
                    Value = new float3(i, 0f, i),
                    Velocity = new float3(1f, 0f, 1f),
                    LastUpdateTime = (float)Time.time - 0.1f,
                    HasChanged = true
                };
                
                EntityManager.SetComponentData(entities[i], networkPosition);
            }
            
            // Act
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            _lagCompensationSystem.Update();
            stopwatch.Stop();
            
            // Assert
            Assert.IsTrue(stopwatch.ElapsedMilliseconds < 100, 
                $"Компенсация задержек для {entityCount} сущностей заняла {stopwatch.ElapsedMilliseconds}мс, что превышает 100мс");
        }
        
        /// <summary>
        /// Тест производительности с частыми обновлениями
        /// </summary>
        [Test]
        public void CompensateLag_FrequentUpdates_ShouldMaintainPerformance()
        {
            // Arrange
            var entity = CreateClientEntity();
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            
            // Act - выполняем множество обновлений
            for (int i = 0; i < 100; i++)
            {
                var networkPosition = new NetworkPosition
                {
                    Value = new float3(i, 0f, i),
                    Velocity = new float3(1f, 0f, 1f),
                    LastUpdateTime = (float)Time.time - 0.1f,
                    HasChanged = true
                };
                
                EntityManager.SetComponentData(entity, networkPosition);
                _lagCompensationSystem.Update();
            }
            
            stopwatch.Stop();
            
            // Assert
            Assert.IsTrue(stopwatch.ElapsedMilliseconds < 200, 
                $"100 обновлений компенсации заняли {stopwatch.ElapsedMilliseconds}мс, что превышает 200мс");
        }
        
        #endregion
        
        #region Тесты валидации компенсации
        
        /// <summary>
        /// Тест валидации компенсированных позиций
        /// </summary>
        [Test]
        public void ValidateCompensatedPosition_WithValidData_ShouldPass()
        {
            // Arrange
            var networkPosition = new NetworkPosition
            {
                Value = new float3(50f, 0f, 100f),
                Velocity = new float3(10f, 0f, 5f),
                LastUpdateTime = (float)Time.time - 0.1f,
                HasChanged = true
            };
            
            EntityManager.SetComponentData(_clientEntity, networkPosition);
            
            // Act
            _lagCompensationSystem.Update();
            
            // Assert
            var compensatedPosition = EntityManager.GetComponentData<NetworkPosition>(_clientEntity);
            Assert.IsTrue(IsValidPosition(compensatedPosition.Value));
        }
        
        /// <summary>
        /// Тест валидации компенсированных позиций с недопустимыми данными
        /// </summary>
        [Test]
        public void ValidateCompensatedPosition_WithInvalidData_ShouldReject()
        {
            // Arrange
            var networkPosition = new NetworkPosition
            {
                Value = new float3(float.PositiveInfinity, 0f, 0f),
                Velocity = new float3(10f, 0f, 5f),
                LastUpdateTime = (float)Time.time - 0.1f,
                HasChanged = true
            };
            
            EntityManager.SetComponentData(_clientEntity, networkPosition);
            
            // Act
            _lagCompensationSystem.Update();
            
            // Assert
            var compensatedPosition = EntityManager.GetComponentData<NetworkPosition>(_clientEntity);
            // Система должна отклонить недопустимые данные
            Assert.IsFalse(IsValidPosition(compensatedPosition.Value));
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
        /// Проверяет, является ли позиция валидной
        /// </summary>
        private bool IsValidPosition(float3 position)
        {
            return !float.IsNaN(position.x) && !float.IsInfinity(position.x) &&
                   !float.IsNaN(position.y) && !float.IsInfinity(position.y) &&
                   !float.IsNaN(position.z) && !float.IsInfinity(position.z);
        }
        
        #endregion
    }
    
    /// <summary>
    /// Данные ускорения для тестирования
    /// </summary>
    public struct AccelerationData : IComponentData
    {
        public float3 Linear;
        public float3 Angular;
    }
    
    /// <summary>
    /// Тег сервера для тестирования
    /// </summary>
    public struct ServerTag : IComponentData
    {
    }
}
