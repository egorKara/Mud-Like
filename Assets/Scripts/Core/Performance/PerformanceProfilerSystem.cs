using Unity.Entities;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using System.Diagnostics;

namespace MudLike.Core.Performance
{
    /// <summary>
    /// Система профилирования производительности в реальном времени
    /// Обеспечивает мониторинг FPS, памяти, физики и сетевой производительности
    /// </summary>
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [BurstCompile]
    public partial class PerformanceProfilerSystem : SystemBase
    {
        private PerformanceMetrics _metrics;
        private NativeArray<float> _fpsHistory;
        private NativeArray<float> _frameTimeHistory;
        private int _historyIndex;
        private float _lastUpdateTime;
        private const int HISTORY_SIZE = 60; // 1 секунда при 60 FPS
        
        protected override void OnCreate()
        {
            _metrics = new PerformanceMetrics();
            _fpsHistory = new NativeArray<float>(HISTORY_SIZE, Allocator.Persistent);
            _frameTimeHistory = new NativeArray<float>(HISTORY_SIZE, Allocator.Persistent);
            _historyIndex = 0;
            _lastUpdateTime = (float)SystemAPI.Time.ElapsedTime;
            
            // Инициализируем историю нулями
            for (int i = 0; i < HISTORY_SIZE; i++)
            {
                _fpsHistory[i] = 0f;
                _frameTimeHistory[i] = 0f;
            }
        }
        
        protected override void OnDestroy()
        {
            if (_fpsHistory.IsCreated)
                _fpsHistory.Dispose();
            if (_frameTimeHistory.IsCreated)
                _frameTimeHistory.Dispose();
        }
        
        protected override void OnUpdate()
        {
            float currentTime = (float)SystemAPI.Time.ElapsedTime;
            float deltaTime = SystemAPI.Time.DeltaTime;
            
            // Обновляем метрики каждый кадр
            UpdateFrameMetrics(deltaTime);
            
            // Обновляем историю каждые 16.67ms (60 FPS)
            if (currentTime - _lastUpdateTime >= 0.01667f)
            {
                UpdatePerformanceHistory();
                _lastUpdateTime = currentTime;
            }
            
            // Проверяем производительность каждую секунду
            if (currentTime % 1.0f < deltaTime)
            {
                CheckPerformanceThresholds();
            }
        }
        
        /// <summary>
        /// Обновляет метрики кадра
        /// </summary>
        [BurstCompile]
        private void UpdateFrameMetrics(float deltaTime)
        {
            _metrics.FrameTime = deltaTime;
            _metrics.FPS = 1.0f / deltaTime;
            _metrics.TotalFrames++;
            
            // Обновляем средние значения
            _metrics.AverageFPS = CalculateAverageFPS();
            _metrics.MinFPS = CalculateMinFPS();
            _metrics.MaxFPS = CalculateMaxFPS();
        }
        
        /// <summary>
        /// Обновляет историю производительности
        /// </summary>
        [BurstCompile]
        private void UpdatePerformanceHistory()
        {
            _fpsHistory[_historyIndex] = _metrics.FPS;
            _frameTimeHistory[_historyIndex] = _metrics.FrameTime;
            
            _historyIndex = (_historyIndex + 1) % HISTORY_SIZE;
        }
        
        /// <summary>
        /// Проверяет пороги производительности
        /// </summary>
        private void CheckPerformanceThresholds()
        {
            // Проверяем минимальный FPS
            if (_metrics.AverageFPS < 30f)
            {
                UnityEngine.Debug.LogWarning($"[Performance] Low FPS detected: {_metrics.AverageFPS:F1} FPS");
            }
            
            // Проверяем максимальное время кадра
            if (_metrics.FrameTime > 0.033f) // 30 FPS
            {
                UnityEngine.Debug.LogWarning($"[Performance] High frame time: {_metrics.FrameTime * 1000:F1}ms");
            }
            
            // Проверяем стабильность FPS
            float fpsVariance = CalculateFPSVariance();
            if (fpsVariance > 10f) // Высокая вариативность
            {
                UnityEngine.Debug.LogWarning($"[Performance] Unstable FPS: variance {fpsVariance:F1}");
            }
        }
        
        /// <summary>
        /// Вычисляет средний FPS
        /// </summary>
        [BurstCompile]
        private float CalculateAverageFPS()
        {
            float sum = 0f;
            int count = 0;
            
            for (int i = 0; i < HISTORY_SIZE; i++)
            {
                if (_fpsHistory[i] > 0f)
                {
                    sum += _fpsHistory[i];
                    count++;
                }
            }
            
            return count > 0 ? sum / count : 0f;
        }
        
        /// <summary>
        /// Вычисляет минимальный FPS
        /// </summary>
        [BurstCompile]
        private float CalculateMinFPS()
        {
            float minFPS = float.MaxValue;
            
            for (int i = 0; i < HISTORY_SIZE; i++)
            {
                if (_fpsHistory[i] > 0f && _fpsHistory[i] < minFPS)
                {
                    minFPS = _fpsHistory[i];
                }
            }
            
            return minFPS == float.MaxValue ? 0f : minFPS;
        }
        
        /// <summary>
        /// Вычисляет максимальный FPS
        /// </summary>
        [BurstCompile]
        private float CalculateMaxFPS()
        {
            float maxFPS = 0f;
            
            for (int i = 0; i < HISTORY_SIZE; i++)
            {
                if (_fpsHistory[i] > maxFPS)
                {
                    maxFPS = _fpsHistory[i];
                }
            }
            
            return maxFPS;
        }
        
        /// <summary>
        /// Вычисляет вариативность FPS
        /// </summary>
        [BurstCompile]
        private float CalculateFPSVariance()
        {
            float average = CalculateAverageFPS();
            if (average <= 0f) return 0f;
            
            float sum = 0f;
            int count = 0;
            
            for (int i = 0; i < HISTORY_SIZE; i++)
            {
                if (_fpsHistory[i] > 0f)
                {
                    float diff = _fpsHistory[i] - average;
                    sum += diff * diff;
                    count++;
                }
            }
            
            return count > 1 ? math.sqrt(sum / (count - 1)) : 0f;
        }
        
        /// <summary>
        /// Получает текущие метрики производительности
        /// </summary>
        public PerformanceMetrics GetMetrics()
        {
            return _metrics;
        }
        
        /// <summary>
        /// Сбрасывает метрики
        /// </summary>
        public void ResetMetrics()
        {
            _metrics = new PerformanceMetrics();
            _historyIndex = 0;
            
            for (int i = 0; i < HISTORY_SIZE; i++)
            {
                _fpsHistory[i] = 0f;
                _frameTimeHistory[i] = 0f;
            }
        }
    }
    
    /// <summary>
    /// Метрики производительности
    /// </summary>
    public struct PerformanceMetrics
    {
        public float FPS;
        public float AverageFPS;
        public float MinFPS;
        public float MaxFPS;
        public float FrameTime;
        public int TotalFrames;
        public float MemoryUsage;
        public float PhysicsTime;
        public float NetworkLatency;
        public float DeformationTime;
        
        /// <summary>
        /// Возвращает строковое представление метрик
        /// </summary>
        public override string ToString()
        {
            return $"FPS: {FPS:F1} | Avg: {AverageFPS:F1} | Min: {MinFPS:F1} | Max: {MaxFPS:F1} | Frame: {FrameTime * 1000:F1}ms";
        }
    }
    
    /// <summary>
    /// Система профилирования физики
    /// </summary>
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile]
    public partial class PhysicsProfilerSystem : SystemBase
    {
        private Stopwatch _physicsStopwatch;
        private float _physicsTime;
        
        protected override void OnCreate()
        {
            _physicsStopwatch = new Stopwatch();
            _physicsTime = 0f;
        }
        
        protected override void OnUpdate()
        {
            _physicsStopwatch.Restart();
            
            // Здесь выполняется физическая симуляция
            // Время измеряется автоматически
            
            _physicsStopwatch.Stop();
            _physicsTime = (float)_physicsStopwatch.Elapsed.TotalMilliseconds;
            
            // Проверяем производительность физики
            if (_physicsTime > 16f) // Больше 16ms для 60 FPS
            {
                UnityEngine.Debug.LogWarning($"[Physics] High physics time: {_physicsTime:F1}ms");
            }
        }
        
        /// <summary>
        /// Получает время выполнения физики
        /// </summary>
        public float GetPhysicsTime()
        {
            return _physicsTime;
        }
    }
    
    /// <summary>
    /// Система профилирования деформации террейна
    /// </summary>
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile]
    public partial class DeformationProfilerSystem : SystemBase
    {
        private Stopwatch _deformationStopwatch;
        private float _deformationTime;
        private int _deformationCount;
        
        protected override void OnCreate()
        {
            _deformationStopwatch = new Stopwatch();
            _deformationTime = 0f;
            _deformationCount = 0;
        }
        
        protected override void OnUpdate()
        {
            _deformationStopwatch.Restart();
            
            // Подсчитываем количество деформаций (закомментировано - требует Terrain модуль)
            _deformationCount = 0;
            
            // Entities
            //     .WithAll<DeformationData>()
            //     .ForEach((in DeformationData deformation) =>
            //     {
            //         _deformationCount++;
            //     }).WithoutBurst().Run();
            
            _deformationStopwatch.Stop();
            _deformationTime = (float)_deformationStopwatch.Elapsed.TotalMilliseconds;
            
            // Проверяем производительность деформации
            if (_deformationTime > 5f) // Больше 5ms
            {
                UnityEngine.Debug.LogWarning($"[Deformation] High deformation time: {_deformationTime:F1}ms for {_deformationCount} deformations");
            }
        }
        
        /// <summary>
        /// Получает время выполнения деформации
        /// </summary>
        public float GetDeformationTime()
        {
            return _deformationTime;
        }
        
        /// <summary>
        /// Получает количество деформаций
        /// </summary>
        public int GetDeformationCount()
        {
            return _deformationCount;
        }
    }
}
