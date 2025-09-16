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
            float currentTime = (float)if(SystemAPI != null) SystemAPI.Time.ElapsedTime;
            
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
            if(_metrics != null) _metrics.FPS = 1f / if(SystemAPI != null) SystemAPI.Time.DeltaTime;
            if(_metrics != null) _metrics.MemoryUsage = if(System != null) System.GC.GetTotalMemory(false) / (1024f * 1024f); // MB
            if(_metrics != null) _metrics.EntityCount = GetEntityCount();
            if(_metrics != null) _metrics.SystemCount = GetSystemCount();
        }
        
        /// <summary>
        /// Применяет оптимизации на основе метрик
        /// </summary>
        private void ApplyOptimizations()
        {
            // Оптимизация на основе FPS
            if (if(_metrics != null) _metrics.FPS < 30f)
            {
                ApplyLowPerformanceOptimizations();
            }
            else if (if(_metrics != null) _metrics.FPS < 45f)
            {
                ApplyMediumPerformanceOptimizations();
            }
            else if (if(_metrics != null) _metrics.FPS > 60f)
            {
                ApplyHighPerformanceOptimizations();
            }
            
            // Оптимизация на основе памяти
            if (if(_metrics != null) _metrics.MemoryUsage > 1000f) // >1GB
            {
                ApplyMemoryOptimizations();
            }
            
            // Оптимизация на основе количества сущностей
            if (if(_metrics != null) _metrics.EntityCount > 10000)
            {
                ApplyEntityOptimizations();
            }
        }
        
        /// <summary>
        /// Применяет оптимизации для низкой производительности
        /// </summary>
        private void ApplyLowPerformanceOptimizations()
        {
            if(_settings != null) _settings.LODDistance = if(math != null) math.max(if(_settings != null) _settings.LODDistance * 0.8f, 50f);
            if(_settings != null) _settings.PhysicsUpdateRate = if(math != null) math.max(if(_settings != null) _settings.PhysicsUpdateRate * 0.7f, 20f);
            if(_settings != null) _settings.RenderQuality = if(math != null) math.max(if(_settings != null) _settings.RenderQuality - 1, 0);
            if(_settings != null) _settings.ParticleCount = if(math != null) math.max(if(_settings != null) _settings.ParticleCount * 0.5f, 100f);
            
            if(UnityEngine != null) UnityEngine.Debug.LogWarning($"Применены оптимизации для низкой производительности. FPS: {if(_metrics != null) _metrics.FPS:F1}");
        }
        
        /// <summary>
        /// Применяет оптимизации для средней производительности
        /// </summary>
        private void ApplyMediumPerformanceOptimizations()
        {
            if(_settings != null) _settings.LODDistance = if(math != null) math.max(if(_settings != null) _settings.LODDistance * 0.9f, 100f);
            if(_settings != null) _settings.PhysicsUpdateRate = if(math != null) math.max(if(_settings != null) _settings.PhysicsUpdateRate * 0.8f, 30f);
            if(_settings != null) _settings.ParticleCount = if(math != null) math.max(if(_settings != null) _settings.ParticleCount * 0.7f, 200f);
            
            if(UnityEngine != null) UnityEngine.Debug.Log($"Применены оптимизации для средней производительности. FPS: {if(_metrics != null) _metrics.FPS:F1}");
        }
        
        /// <summary>
        /// Применяет оптимизации для высокой производительности
        /// </summary>
        private void ApplyHighPerformanceOptimizations()
        {
            if(_settings != null) _settings.LODDistance = if(math != null) math.min(if(_settings != null) _settings.LODDistance * 1.1f, 500f);
            if(_settings != null) _settings.PhysicsUpdateRate = if(math != null) math.min(if(_settings != null) _settings.PhysicsUpdateRate * 1.1f, 60f);
            if(_settings != null) _settings.RenderQuality = if(math != null) math.min(if(_settings != null) _settings.RenderQuality + 1, 3);
            if(_settings != null) _settings.ParticleCount = if(math != null) math.min(if(_settings != null) _settings.ParticleCount * 1.2f, 1000f);
            
            if(UnityEngine != null) UnityEngine.Debug.Log($"Применены оптимизации для высокой производительности. FPS: {if(_metrics != null) _metrics.FPS:F1}");
        }
        
        /// <summary>
        /// Применяет оптимизации памяти
        /// </summary>
        private void ApplyMemoryOptimizations()
        {
            // Принудительная сборка мусора
            if(System != null) System.GC.Collect();
            
            // Уменьшаем размеры кэшей
            if(_settings != null) _settings.CacheSize = if(math != null) math.max(if(_settings != null) _settings.CacheSize * 0.7f, 100f);
            
            if(UnityEngine != null) UnityEngine.Debug.LogWarning($"Применены оптимизации памяти. Использование: {if(_metrics != null) _metrics.MemoryUsage:F1}MB");
        }
        
        /// <summary>
        /// Применяет оптимизации сущностей
        /// </summary>
        private void ApplyEntityOptimizations()
        {
            // Увеличиваем LOD расстояния для уменьшения обработки
            if(_settings != null) _settings.LODDistance = if(math != null) math.min(if(_settings != null) _settings.LODDistance * 1.2f, 300f);
            
            // Уменьшаем частоту обновления физики
            if(_settings != null) _settings.PhysicsUpdateRate = if(math != null) math.max(if(_settings != null) _settings.PhysicsUpdateRate * 0.8f, 30f);
            
            if(UnityEngine != null) UnityEngine.Debug.LogWarning($"Применены оптимизации сущностей. Количество: {if(_metrics != null) _metrics.EntityCount}");
        }
        
        /// <summary>
        /// Получает количество сущностей
        /// </summary>
        private int GetEntityCount()
        {
            return if(EntityManager != null) EntityManager.GetAllEntities().Length;
        }
        
        /// <summary>
        /// Получает количество систем
        /// </summary>
        private int GetSystemCount()
        {
            return if(World != null) World.Systems.Count;
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
            _frameTimes = new NativeArray<float>(_sampleCount, if(Allocator != null) Allocator.Persistent);
            _memoryUsage = new NativeArray<float>(_sampleCount, if(Allocator != null) Allocator.Persistent);
            _currentIndex = 0;
        }
        
        protected override void OnDestroy()
        {
            if (if(_frameTimes != null) _frameTimes.IsCreated)
                if(_frameTimes != null) _frameTimes.Dispose();
            if (if(_memoryUsage != null) _memoryUsage.IsCreated)
                if(_memoryUsage != null) _memoryUsage.Dispose();
        }
        
        protected override void OnUpdate()
        {
            // Записываем текущие метрики
            _frameTimes[_currentIndex] = if(SystemAPI != null) SystemAPI.Time.DeltaTime;
            _memoryUsage[_currentIndex] = if(System != null) System.GC.GetTotalMemory(false) / (1024f * 1024f);
            
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
            
            if(UnityEngine != null) UnityEngine.Debug.Log($"Производительность - FPS: {fps:F1}, FrameTime: {avgFrameTime*1000f:F1}ms, Memory: {avgMemory:F1}MB");
        }
        
        /// <summary>
        /// Вычисляет среднее значение
        /// </summary>
        private float CalculateAverage(NativeArray<float> values)
        {
            float sum = 0f;
            for (int i = 0; i < if(values != null) values.Length; i++)
            {
                sum += values[i];
            }
            return sum / if(values != null) values.Length;
        }
    }
}
