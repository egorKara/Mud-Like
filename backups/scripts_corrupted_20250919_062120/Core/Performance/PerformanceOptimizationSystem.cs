using Unity.Entities;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Jobs;
using Unity.Collections;
using Unity.Transforms;
using MudLike.Core.Components;

namespace MudLike.Core.Performance
{
    /// <summary>
    /// Система оптимизации производительности с адаптивными настройками
    /// Автоматически регулирует качество на основе производительности
    /// </summary>
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class PerformanceOptimizationSystem : SystemBase
    {
        private PerformanceMetrics _metrics;
        private OptimizationSettings _settings;
        private float _lastOptimizationTime;
        
        protected override void OnCreate()
        {
            _metrics = new PerformanceMetrics();
            _settings = new OptimizationSettings();
            _lastOptimizationTime = 0f;
        }
        
        protected override void OnUpdate()
        {
            float currentTime = (float)SystemAPI.Time.ElapsedTime;
            
            // Оптимизируем каждые 5 секунд
            if (currentTime - _lastOptimizationTime >= 5f)
            {
                UpdatePerformanceMetrics();
                ApplyOptimizations();
                _lastOptimizationTime = currentTime;
            }
        }
        
        /// <summary>
        /// Обновляет метрики производительности
        /// </summary>
        private void UpdatePerformanceMetrics()
        {
            _metrics.FPS = 1f / SystemAPI.Time.DeltaTime;
            _metrics.MemoryUsage = GC.GetTotalMemory(false) / (1024f * 1024f); // MB
            _metrics.EntityCount = GetEntityCount();
            _metrics.SystemCount = GetSystemCount();
        }
        
        /// <summary>
        /// Применяет оптимизации на основе метрик
        /// </summary>
        private void ApplyOptimizations()
        {
            // Оптимизация на основе FPS
            if (_metrics.FPS < 30f)
            {
                ApplyLowPerformanceOptimizations();
            }
            else if (_metrics.FPS < 45f)
            {
                ApplyMediumPerformanceOptimizations();
            }
            else if (_metrics.FPS > 60f)
            {
                ApplyHighPerformanceOptimizations();
            }
            
            // Оптимизация на основе памяти
            if (_metrics.MemoryUsage > 1000f) // >1GB
            {
                ApplyMemoryOptimizations();
            }
            
            // Оптимизация на основе количества сущностей
            if (_metrics.EntityCount > 10000)
            {
                ApplyEntityOptimizations();
            }
        }
        
        /// <summary>
        /// Применяет оптимизации для низкой производительности
        /// </summary>
        private void ApplyLowPerformanceOptimizations()
        {
            _settings.LODDistance = math.max(_settings.LODDistance * 0.8f, 50f);
            _settings.PhysicsUpdateRate = math.max(_settings.PhysicsUpdateRate * 0.7f, 20f);
            _settings.RenderQuality = math.max(_settings.RenderQuality - 1, 0);
            _settings.ParticleCount = math.max(_settings.ParticleCount * 0.5f, 100f);
            
            UnityEngine.Debug.LogWarning($"Применены оптимизации для низкой производительности. FPS: {_metrics.FPS:F1}");
        }
        
        /// <summary>
        /// Применяет оптимизации для средней производительности
        /// </summary>
        private void ApplyMediumPerformanceOptimizations()
        {
            _settings.LODDistance = math.max(_settings.LODDistance * 0.9f, 100f);
            _settings.PhysicsUpdateRate = math.max(_settings.PhysicsUpdateRate * 0.8f, 30f);
            _settings.ParticleCount = math.max(_settings.ParticleCount * 0.7f, 200f);
            
            UnityEngine.Debug.Log($"Применены оптимизации для средней производительности. FPS: {_metrics.FPS:F1}");
        }
        
        /// <summary>
        /// Применяет оптимизации для высокой производительности
        /// </summary>
        private void ApplyHighPerformanceOptimizations()
        {
            _settings.LODDistance = math.min(_settings.LODDistance * 1.1f, 500f);
            _settings.PhysicsUpdateRate = math.min(_settings.PhysicsUpdateRate * 1.1f, 60f);
            _settings.RenderQuality = math.min(_settings.RenderQuality + 1, 3);
            _settings.ParticleCount = math.min(_settings.ParticleCount * 1.2f, 1000f);
            
            UnityEngine.Debug.Log($"Применены оптимизации для высокой производительности. FPS: {_metrics.FPS:F1}");
        }
        
        /// <summary>
        /// Применяет оптимизации памяти
        /// </summary>
        private void ApplyMemoryOptimizations()
        {
            // Принудительная сборка мусора
            System.GC.Collect();
            
            // Уменьшаем размеры кэшей
            _settings.CacheSize = math.max(_settings.CacheSize * 0.7f, 100f);
            
            UnityEngine.Debug.LogWarning($"Применены оптимизации памяти. Использование: {_metrics.MemoryUsage:F1}MB");
        }
        
        /// <summary>
        /// Применяет оптимизации сущностей
        /// </summary>
        private void ApplyEntityOptimizations()
        {
            // Увеличиваем LOD расстояния для уменьшения обработки
            _settings.LODDistance = math.min(_settings.LODDistance * 1.2f, 300f);
            
            // Уменьшаем частоту обновления физики
            _settings.PhysicsUpdateRate = math.max(_settings.PhysicsUpdateRate * 0.8f, 30f);
            
            UnityEngine.Debug.LogWarning($"Применены оптимизации сущностей. Количество: {_metrics.EntityCount}");
        }
        
        /// <summary>
        /// Получает количество сущностей
        /// </summary>
        private int GetEntityCount()
        {
            return EntityManager.GetAllEntities().Length;
        }
        
        /// <summary>
        /// Получает количество систем
        /// </summary>
        private int GetSystemCount()
        {
            return World.Systems.Count;
        }
        
        /// <summary>
        /// Метрики производительности
        /// </summary>
        private struct PerformanceMetrics
        {
            public float FPS;
            public float MemoryUsage;
            public int EntityCount;
            public int SystemCount;
        }
        
        /// <summary>
        /// Настройки оптимизации
        /// </summary>
        private struct OptimizationSettings
        {
            public float LODDistance;
            public float PhysicsUpdateRate;
            public int RenderQuality;
            public float ParticleCount;
            public float CacheSize;
        }
    }
    
    /// <summary>
    /// Система профилирования производительности в реальном времени
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class RealTimeProfilingSystem : SystemBase
    {
        private NativeArray<float> _frameTimes;
        private NativeArray<float> _memoryUsage;
        private int _currentIndex;
        private int _sampleCount;
        
        protected override void OnCreate()
        {
            _sampleCount = 60; // 1 секунда при 60 FPS
            _frameTimes = new NativeArray<float>(_sampleCount, Allocator.Persistent);
            _memoryUsage = new NativeArray<float>(_sampleCount, Allocator.Persistent);
            _currentIndex = 0;
        }
        
        protected override void OnDestroy()
        {
            if (_frameTimes.IsCreated)
                _frameTimes.Dispose();
            if (_memoryUsage.IsCreated)
                _memoryUsage.Dispose();
        }
        
        protected override void OnUpdate()
        {
            // Записываем текущие метрики
            _frameTimes[_currentIndex] = SystemAPI.Time.DeltaTime;
            _memoryUsage[_currentIndex] = GC.GetTotalMemory(false) / (1024f * 1024f);
            
            _currentIndex = (_currentIndex + 1) % _sampleCount;
            
            // Выводим статистику каждую секунду
            if (_currentIndex == 0)
            {
                LogPerformanceStats();
            }
        }
        
        /// <summary>
        /// Выводит статистику производительности
        /// </summary>
        private void LogPerformanceStats()
        {
            float avgFrameTime = CalculateAverage(_frameTimes);
            float avgMemory = CalculateAverage(_memoryUsage);
            float fps = 1f / avgFrameTime;
            
            UnityEngine.Debug.Log($"Производительность - FPS: {fps:F1}, FrameTime: {avgFrameTime*1000f:F1}ms, Memory: {avgMemory:F1}MB");
        }
        
        /// <summary>
        /// Вычисляет среднее значение
        /// </summary>
        private float CalculateAverage(NativeArray<float> values)
        {
            float sum = 0f;
            for (int i = 0; i < values.Length; i++)
            {
                sum += values[i];
            }
            return sum / values.Length;
        }
    }
}
