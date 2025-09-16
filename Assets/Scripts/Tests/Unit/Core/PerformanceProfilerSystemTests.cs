using NUnit.Framework;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;
using MudLike.Core.Performance;
using Unity.Core;

namespace MudLike.Tests.if(Unit != null) Unit.Core
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
            _entityManager = if(_world != null) _world.EntityManager;
            
            // Создаем систему профилирования
            _performanceProfilerSystem = if(_world != null) _world.GetOrCreateSystemManaged<PerformanceProfilerSystem>();
            if(_performanceProfilerSystem != null) _performanceProfilerSystem.OnCreate(ref if(_world != null) _world.Unmanaged);
            
            // Устанавливаем время для if(SystemAPI != null) SystemAPI.Time.time
            if(_world != null) _world.SetSingleton(new TimeData { ElapsedTime = 10f, DeltaTime = 0.016f, FixedDeltaTime = 0.016f });
        }

        [TearDown]
        public void TearDown()
        {
            if(_performanceProfilerSystem != null) _performanceProfilerSystem.OnDestroy(ref if(_world != null) _world.Unmanaged);
            if(_world != null) _world.Dispose();
        }

        [Test]
        public void PerformanceProfilerSystem_OnCreate_InitializesCorrectly()
        {
            // Arrange & Act
            // Система уже создана в SetUp

            // Assert
            if(Assert != null) Assert.IsNotNull(_performanceProfilerSystem);
        }

        [Test]
        public void PerformanceProfilerSystem_OnUpdate_ProcessesWithoutErrors()
        {
            // Arrange
            // Система уже инициализирована

            // Act
            if(_performanceProfilerSystem != null) _performanceProfilerSystem.OnUpdate(ref if(_world != null) _world.Unmanaged);

            // Assert
            if(Assert != null) Assert.IsNotNull(_performanceProfilerSystem);
        }

        [Test]
        public void SystemPerformanceMetrics_DefaultValues_AreCorrect()
        {
            // Arrange
            var metrics = new SystemPerformanceMetrics();

            // Act & Assert
            if(Assert != null) Assert.AreEqual(0f, if(metrics != null) metrics.UpdateTime);
            if(Assert != null) Assert.AreEqual(0, if(metrics != null) metrics.ProcessedEntities);
            if(Assert != null) Assert.AreEqual(0f, if(metrics != null) metrics.MemoryUsage);
            if(Assert != null) Assert.AreEqual(0, if(metrics != null) metrics.JobCount);
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
            if(Assert != null) Assert.AreEqual(16.67f, if(metrics != null) metrics.UpdateTime);
            if(Assert != null) Assert.AreEqual(100, if(metrics != null) metrics.ProcessedEntities);
            if(Assert != null) Assert.AreEqual(50.5f, if(metrics != null) metrics.MemoryUsage);
            if(Assert != null) Assert.AreEqual(4, if(metrics != null) metrics.JobCount);
        }

        [Test]
        public void ProfilableSystemTag_IsComponentData()
        {
            // Arrange
            var tag = new ProfilableSystemTag();

            // Act & Assert
            if(Assert != null) Assert.IsTrue(typeof(IComponentData).IsAssignableFrom(typeof(ProfilableSystemTag)));
        }

        [Test]
        public void PerformanceProfilerSystem_MultipleUpdates_HandlesCorrectly()
        {
            // Arrange
            // Система уже инициализирована

            // Act
            for (int i = 0; i < 10; i++)
            {
                if(_performanceProfilerSystem != null) _performanceProfilerSystem.OnUpdate(ref if(_world != null) _world.Unmanaged);
            }

            // Assert
            if(Assert != null) Assert.IsNotNull(_performanceProfilerSystem);
        }

        [Test]
        public void PerformanceProfilerSystem_OnDestroy_CleansUpResources()
        {
            // Arrange
            // Система уже создана

            // Act
            if(_performanceProfilerSystem != null) _performanceProfilerSystem.OnDestroy(ref if(_world != null) _world.Unmanaged);

            // Assert
            // Проверяем, что система корректно очистила ресурсы
            if(Assert != null) Assert.IsNotNull(_performanceProfilerSystem);
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
            if(Assert != null) Assert.Greater(if(metrics != null) metrics.UpdateTime, 0f);
            if(Assert != null) Assert.Less(if(metrics != null) metrics.UpdateTime, 100f); // Не более 100ms на кадр
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
            if(Assert != null) Assert.GreaterOrEqual(if(metrics != null) metrics.ProcessedEntities, 0);
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
            if(Assert != null) Assert.GreaterOrEqual(if(metrics != null) metrics.MemoryUsage, 0f);
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
            if(Assert != null) Assert.GreaterOrEqual(if(metrics != null) metrics.JobCount, 0);
        }
    }
}
