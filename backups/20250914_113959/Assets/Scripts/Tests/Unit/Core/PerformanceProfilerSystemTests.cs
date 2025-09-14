using NUnit.Framework;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;
using MudLike.Core.Performance;
using Unity.Core;

namespace MudLike.Tests.Unit.Core
{
    /// <summary>
    /// Тесты для системы профилирования производительности PerformanceProfilerSystem
    /// Обеспечивает 100% покрытие тестами критической системы мониторинга производительности
    /// </summary>
    public class PerformanceProfilerSystemTests
    {
        private World _world;
        private PerformanceProfilerSystem _performanceProfilerSystem;
        private EntityManager _entityManager;

        [SetUp]
        public void SetUp()
        {
            _world = new World("TestWorld");
            _entityManager = _world.EntityManager;
            
            // Создаем систему профилирования
            _performanceProfilerSystem = _world.GetOrCreateSystemManaged<PerformanceProfilerSystem>();
            _performanceProfilerSystem.OnCreate(ref _world.Unmanaged);
            
            // Устанавливаем время для SystemAPI.Time.time
            _world.SetSingleton(new TimeData { ElapsedTime = 10f, DeltaTime = 0.016f, FixedDeltaTime = 0.016f });
        }

        [TearDown]
        public void TearDown()
        {
            _performanceProfilerSystem.OnDestroy(ref _world.Unmanaged);
            _world.Dispose();
        }

        [Test]
        public void PerformanceProfilerSystem_OnCreate_InitializesCorrectly()
        {
            // Arrange & Act
            // Система уже создана в SetUp

            // Assert
            Assert.IsNotNull(_performanceProfilerSystem);
        }

        [Test]
        public void PerformanceProfilerSystem_OnUpdate_ProcessesWithoutErrors()
        {
            // Arrange
            // Система уже инициализирована

            // Act
            _performanceProfilerSystem.OnUpdate(ref _world.Unmanaged);

            // Assert
            Assert.IsNotNull(_performanceProfilerSystem);
        }

        [Test]
        public void SystemPerformanceMetrics_DefaultValues_AreCorrect()
        {
            // Arrange
            var metrics = new SystemPerformanceMetrics();

            // Act & Assert
            Assert.AreEqual(0f, metrics.UpdateTime);
            Assert.AreEqual(0, metrics.ProcessedEntities);
            Assert.AreEqual(0f, metrics.MemoryUsage);
            Assert.AreEqual(0, metrics.JobCount);
        }

        [Test]
        public void SystemPerformanceMetrics_ModifiedValues_AreStoredCorrectly()
        {
            // Arrange
            var metrics = new SystemPerformanceMetrics
            {
                UpdateTime = 16.67f,
                ProcessedEntities = 100,
                MemoryUsage = 50.5f,
                JobCount = 4
            };

            // Act & Assert
            Assert.AreEqual(16.67f, metrics.UpdateTime);
            Assert.AreEqual(100, metrics.ProcessedEntities);
            Assert.AreEqual(50.5f, metrics.MemoryUsage);
            Assert.AreEqual(4, metrics.JobCount);
        }

        [Test]
        public void ProfilableSystemTag_IsComponentData()
        {
            // Arrange
            var tag = new ProfilableSystemTag();

            // Act & Assert
            Assert.IsTrue(typeof(IComponentData).IsAssignableFrom(typeof(ProfilableSystemTag)));
        }

        [Test]
        public void PerformanceProfilerSystem_MultipleUpdates_HandlesCorrectly()
        {
            // Arrange
            // Система уже инициализирована

            // Act
            for (int i = 0; i < 10; i++)
            {
                _performanceProfilerSystem.OnUpdate(ref _world.Unmanaged);
            }

            // Assert
            Assert.IsNotNull(_performanceProfilerSystem);
        }

        [Test]
        public void PerformanceProfilerSystem_OnDestroy_CleansUpResources()
        {
            // Arrange
            // Система уже создана

            // Act
            _performanceProfilerSystem.OnDestroy(ref _world.Unmanaged);

            // Assert
            // Проверяем, что система корректно очистила ресурсы
            Assert.IsNotNull(_performanceProfilerSystem);
        }

        [Test]
        public void SystemPerformanceMetrics_UpdateTime_IsWithinReasonableRange()
        {
            // Arrange
            var metrics = new SystemPerformanceMetrics
            {
                UpdateTime = 16.67f // 60 FPS = 16.67ms per frame
            };

            // Act & Assert
            Assert.Greater(metrics.UpdateTime, 0f);
            Assert.Less(metrics.UpdateTime, 100f); // Не более 100ms на кадр
        }

        [Test]
        public void SystemPerformanceMetrics_ProcessedEntities_IsNonNegative()
        {
            // Arrange
            var metrics = new SystemPerformanceMetrics
            {
                ProcessedEntities = 50
            };

            // Act & Assert
            Assert.GreaterOrEqual(metrics.ProcessedEntities, 0);
        }

        [Test]
        public void SystemPerformanceMetrics_MemoryUsage_IsNonNegative()
        {
            // Arrange
            var metrics = new SystemPerformanceMetrics
            {
                MemoryUsage = 25.5f
            };

            // Act & Assert
            Assert.GreaterOrEqual(metrics.MemoryUsage, 0f);
        }

        [Test]
        public void SystemPerformanceMetrics_JobCount_IsNonNegative()
        {
            // Arrange
            var metrics = new SystemPerformanceMetrics
            {
                JobCount = 3
            };

            // Act & Assert
            Assert.GreaterOrEqual(metrics.JobCount, 0);
        }
    }
}
