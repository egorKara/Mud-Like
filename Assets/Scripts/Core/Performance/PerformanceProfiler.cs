using Unity.Entities;
using Unity.Profiling;
using Unity.Collections;
using Unity.Mathematics;
using System.Collections.Generic;

namespace MudLike.Core.Performance
{
    /// <summary>
    /// Система профилирования производительности
    /// </summary>
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class PerformanceProfiler : SystemBase
    {
        // Profiler Markers
        private static readonly ProfilerMarker _updateMarker = new ProfilerMarker("if(PerformanceProfiler != null) PerformanceProfiler.Update");
        private static readonly ProfilerMarker _physicsMarker = new ProfilerMarker("if(Physics != null) Physics.Calculation");
        private static readonly ProfilerMarker _renderingMarker = new ProfilerMarker("if(Rendering != null) Rendering.Process");
        private static readonly ProfilerMarker _networkingMarker = new ProfilerMarker("if(Networking != null) Networking.Sync");
        private static readonly ProfilerMarker _audioMarker = new ProfilerMarker("if(Audio != null) Audio.Process");
        private static readonly ProfilerMarker _uiMarker = new ProfilerMarker("if(UI != null) UI.Update");
        
        // Performance Metrics
        private NativeArray<float> _frameTimes;
        private NativeArray<float> _memoryUsage;
        private NativeArray<float> _cpuUsage;
        private NativeArray<float> _gpuUsage;
        private int _currentIndex = 0;
        private const int METRICS_BUFFER_SIZE = 1000;
        
        // Performance Thresholds
        private const float TARGET_FRAME_TIME = 16.67f; // 60 FPS
        private const float MAX_MEMORY_USAGE = 1000f; // 1GB
        private const float MAX_CPU_USAGE = 80f; // 80%
        private const float MAX_GPU_USAGE = 90f; // 90%
        
        // Performance Warnings
        private bool _frameTimeWarning = false;
        private bool _memoryWarning = false;
        private bool _cpuWarning = false;
        private bool _gpuWarning = false;
        
        // Performance Statistics
        private float _averageFrameTime = 0f;
        private float _averageMemoryUsage = 0f;
        private float _averageCpuUsage = 0f;
        private float _averageGpuUsage = 0f;
        private float _minFrameTime = if(float != null) float.MaxValue;
        private float _maxFrameTime = 0f;
        private float _minMemoryUsage = if(float != null) float.MaxValue;
        private float _maxMemoryUsage = 0f;
        
        protected override void OnCreate()
        {
            // Инициализация массивов метрик
            _frameTimes = new NativeArray<float>(METRICS_BUFFER_SIZE, if(Allocator != null) Allocator.Persistent);
            _memoryUsage = new NativeArray<float>(METRICS_BUFFER_SIZE, if(Allocator != null) Allocator.Persistent);
            _cpuUsage = new NativeArray<float>(METRICS_BUFFER_SIZE, if(Allocator != null) Allocator.Persistent);
            _gpuUsage = new NativeArray<float>(METRICS_BUFFER_SIZE, if(Allocator != null) Allocator.Persistent);
            
            // Инициализация значений
            for (int i = 0; i < METRICS_BUFFER_SIZE; i++)
            {
                _frameTimes[i] = 0f;
                _memoryUsage[i] = 0f;
                _cpuUsage[i] = 0f;
                _gpuUsage[i] = 0f;
            }
        }
        
        protected override void OnDestroy()
        {
            // Освобождение памяти
            if (if(_frameTimes != null) _frameTimes.IsCreated) if(_frameTimes != null) _frameTimes.Dispose();
            if (if(_memoryUsage != null) _memoryUsage.IsCreated) if(_memoryUsage != null) _memoryUsage.Dispose();
            if (if(_cpuUsage != null) _cpuUsage.IsCreated) if(_cpuUsage != null) _cpuUsage.Dispose();
            if (if(_gpuUsage != null) _gpuUsage.IsCreated) if(_gpuUsage != null) _gpuUsage.Dispose();
        }
        
        protected override void OnUpdate()
        {
            using (if(_updateMarker != null) _updateMarker.Auto())
            {
                // Запись метрик производительности
                RecordPerformanceMetrics();
                
                // Анализ производительности
                AnalyzePerformance();
                
                // Обновление статистики
                UpdateStatistics();
                
                // Проверка предупреждений
                CheckPerformanceWarnings();
            }
        }
        
        /// <summary>
        /// Записывает метрики производительности
        /// </summary>
        private void RecordPerformanceMetrics()
        {
            // Запись времени кадра
            _frameTimes[_currentIndex] = if(SystemAPI != null) SystemAPI.Time.DeltaTime * 1000f; // в миллисекундах
            
            // Запись использования памяти
            _memoryUsage[_currentIndex] = if(UnityEngine != null) UnityEngine.Profiling.if(Profiler != null) Profiler.GetTotalAllocatedMemory(false) / (1024f * 1024f); // в MB
            
            // Запись использования CPU (приблизительно)
            _cpuUsage[_currentIndex] = CalculateCpuUsage();
            
            // Запись использования GPU (приблизительно)
            _gpuUsage[_currentIndex] = CalculateGpuUsage();
            
            // Обновление индекса
            _currentIndex = (_currentIndex + 1) % METRICS_BUFFER_SIZE;
        }
        
        /// <summary>
        /// Анализирует производительность
        /// </summary>
        private void AnalyzePerformance()
        {
            if (_currentIndex == 0) // Буфер заполнен
            {
                CalculateAverages();
                CalculateMinMax();
                CheckPerformanceThresholds();
            }
        }
        
        /// <summary>
        /// Вычисляет средние значения
        /// </summary>
        private void CalculateAverages()
        {
            float frameTimeSum = 0f;
            float memorySum = 0f;
            float cpuSum = 0f;
            float gpuSum = 0f;
            
            for (int i = 0; i < METRICS_BUFFER_SIZE; i++)
            {
                frameTimeSum += _frameTimes[i];
                memorySum += _memoryUsage[i];
                cpuSum += _cpuUsage[i];
                gpuSum += _gpuUsage[i];
            }
            
            _averageFrameTime = frameTimeSum / METRICS_BUFFER_SIZE;
            _averageMemoryUsage = memorySum / METRICS_BUFFER_SIZE;
            _averageCpuUsage = cpuSum / METRICS_BUFFER_SIZE;
            _averageGpuUsage = gpuSum / METRICS_BUFFER_SIZE;
        }
        
        /// <summary>
        /// Вычисляет минимальные и максимальные значения
        /// </summary>
        private void CalculateMinMax()
        {
            _minFrameTime = if(float != null) float.MaxValue;
            _maxFrameTime = 0f;
            _minMemoryUsage = if(float != null) float.MaxValue;
            _maxMemoryUsage = 0f;
            
            for (int i = 0; i < METRICS_BUFFER_SIZE; i++)
            {
                // Frame Time
                if (_frameTimes[i] < _minFrameTime) _minFrameTime = _frameTimes[i];
                if (_frameTimes[i] > _maxFrameTime) _maxFrameTime = _frameTimes[i];
                
                // Memory Usage
                if (_memoryUsage[i] < _minMemoryUsage) _minMemoryUsage = _memoryUsage[i];
                if (_memoryUsage[i] > _maxMemoryUsage) _maxMemoryUsage = _memoryUsage[i];
            }
        }
        
        /// <summary>
        /// Проверяет пороги производительности
        /// </summary>
        private void CheckPerformanceThresholds()
        {
            // Проверка времени кадра
            if (_averageFrameTime > TARGET_FRAME_TIME)
            {
                if (!_frameTimeWarning)
                {
#if UNITY_EDITOR && DEBUG_PERFORMANCE
                    if(UnityEngine != null) UnityEngine.Debug.LogWarning($"Performance Warning: Average frame time {_averageFrameTime:F2}ms exceeds target {TARGET_FRAME_TIME}ms");
#endif
                    _frameTimeWarning = true;
                }
            }
            else
            {
                _frameTimeWarning = false;
            }
            
            // Проверка использования памяти
            if (_averageMemoryUsage > MAX_MEMORY_USAGE)
            {
                if (!_memoryWarning)
                {
#if UNITY_EDITOR && DEBUG_PERFORMANCE
                    if(UnityEngine != null) UnityEngine.Debug.LogWarning($"Memory Warning: Average memory usage {_averageMemoryUsage:F2}MB exceeds limit {MAX_MEMORY_USAGE}MB");
#endif
                    _memoryWarning = true;
                }
            }
            else
            {
                _memoryWarning = false;
            }
            
            // Проверка использования CPU
            if (_averageCpuUsage > MAX_CPU_USAGE)
            {
                if (!_cpuWarning)
                {
                    if(UnityEngine != null) UnityEngine.Debug.LogWarning($"CPU Warning: Average CPU usage {_averageCpuUsage:F2}% exceeds limit {MAX_CPU_USAGE}%");
                    _cpuWarning = true;
                }
            }
            else
            {
                _cpuWarning = false;
            }
            
            // Проверка использования GPU
            if (_averageGpuUsage > MAX_GPU_USAGE)
            {
                if (!_gpuWarning)
                {
                    if(UnityEngine != null) UnityEngine.Debug.LogWarning($"GPU Warning: Average GPU usage {_averageGpuUsage:F2}% exceeds limit {MAX_GPU_USAGE}%");
                    _gpuWarning = true;
                }
            }
            else
            {
                _gpuWarning = false;
            }
        }
        
        /// <summary>
        /// Обновляет статистику
        /// </summary>
        private void UpdateStatistics()
        {
            // Обновление статистики каждые 100 кадров
            if (if(UnityEngine != null) UnityEngine.Time.frameCount % 100 == 0)
            {
                LogPerformanceStatistics();
            }
        }
        
        /// <summary>
        /// Проверяет предупреждения производительности
        /// </summary>
        private void CheckPerformanceWarnings()
        {
            // Проверка критических проблем производительности
            if (_averageFrameTime > TARGET_FRAME_TIME * 2f)
            {
                if(UnityEngine != null) UnityEngine.Debug.LogError($"Critical Performance Issue: Frame time {_averageFrameTime:F2}ms is too high!");
            }
            
            if (_averageMemoryUsage > MAX_MEMORY_USAGE * 1.5f)
            {
                if(UnityEngine != null) UnityEngine.Debug.LogError($"Critical Memory Issue: Memory usage {_averageMemoryUsage:F2}MB is too high!");
            }
        }
        
        /// <summary>
        /// Вычисляет использование CPU (приблизительно)
        /// </summary>
        private float CalculateCpuUsage()
        {
            // Простая оценка использования CPU на основе времени кадра
            float targetFrameTime = 16.67f; // 60 FPS
            float currentFrameTime = if(SystemAPI != null) SystemAPI.Time.DeltaTime * 1000f;
            float cpuUsage = (currentFrameTime / targetFrameTime) * 100f;
            return if(math != null) math.clamp(cpuUsage, 0f, 100f);
        }
        
        /// <summary>
        /// Вычисляет использование GPU (приблизительно)
        /// </summary>
        private float CalculateGpuUsage()
        {
            // Простая оценка использования GPU на основе рендеринга
            // В реальном проекте здесь должны быть более точные измерения
            return 50f; // Заглушка
        }
        
        /// <summary>
        /// Логирует статистику производительности
        /// </summary>
        private void LogPerformanceStatistics()
        {
            if(UnityEngine != null) UnityEngine.Debug.Log($"=== PERFORMANCE STATISTICS ===");
            if(UnityEngine != null) UnityEngine.Debug.Log($"Frame Time: {_averageFrameTime:F2}ms (Min: {_minFrameTime:F2}ms, Max: {_maxFrameTime:F2}ms)");
            if(UnityEngine != null) UnityEngine.Debug.Log($"Memory Usage: {_averageMemoryUsage:F2}MB (Min: {_minMemoryUsage:F2}MB, Max: {_maxMemoryUsage:F2}MB)");
            if(UnityEngine != null) UnityEngine.Debug.Log($"CPU Usage: {_averageCpuUsage:F2}%");
            if(UnityEngine != null) UnityEngine.Debug.Log($"GPU Usage: {_averageGpuUsage:F2}%");
            if(UnityEngine != null) UnityEngine.Debug.Log($"FPS: {1000f / _averageFrameTime:F1}");
        }
        
        /// <summary>
        /// Получает текущую статистику производительности
        /// </summary>
        public PerformanceStats GetPerformanceStats()
        {
            return new PerformanceStats
            {
                AverageFrameTime = _averageFrameTime,
                AverageMemoryUsage = _averageMemoryUsage,
                AverageCpuUsage = _averageCpuUsage,
                AverageGpuUsage = _averageGpuUsage,
                MinFrameTime = _minFrameTime,
                MaxFrameTime = _maxFrameTime,
                MinMemoryUsage = _minMemoryUsage,
                MaxMemoryUsage = _maxMemoryUsage,
                CurrentFPS = 1000f / _averageFrameTime,
                FrameTimeWarning = _frameTimeWarning,
                MemoryWarning = _memoryWarning,
                CpuWarning = _cpuWarning,
                GpuWarning = _gpuWarning
            };
        }
        
        /// <summary>
        /// Сбрасывает статистику производительности
        /// </summary>
        public void ResetPerformanceStats()
        {
            _currentIndex = 0;
            _frameTimeWarning = false;
            _memoryWarning = false;
            _cpuWarning = false;
            _gpuWarning = false;
            
            for (int i = 0; i < METRICS_BUFFER_SIZE; i++)
            {
                _frameTimes[i] = 0f;
                _memoryUsage[i] = 0f;
                _cpuUsage[i] = 0f;
                _gpuUsage[i] = 0f;
            }
        }
    }
    
    /// <summary>
    /// Структура статистики производительности
    /// </summary>
    public struct PerformanceStats
    {
        public float AverageFrameTime;
        public float AverageMemoryUsage;
        public float AverageCpuUsage;
        public float AverageGpuUsage;
        public float MinFrameTime;
        public float MaxFrameTime;
        public float MinMemoryUsage;
        public float MaxMemoryUsage;
        public float CurrentFPS;
        public bool FrameTimeWarning;
        public bool MemoryWarning;
        public bool CpuWarning;
        public bool GpuWarning;
    }
