using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Burst;
using Unity.Jobs;
using UnityEngine;

namespace MudLike.Core.Performance
{
    /// <summary>
    /// Адаптивная система производительности с автоматической оптимизацией
    /// </summary>
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class AdaptivePerformanceSystem : SystemBase
    {
        // Performance Metrics
        private float _targetFrameTime = 16.67f; // 60 FPS
        private float _currentFrameTime = 16.67f;
        private float _averageFrameTime = 16.67f;
        private float _frameTimeVariance = 0f;
        
        // Performance Levels
        private PerformanceLevel _currentLevel = PerformanceLevel.High;
        private PerformanceLevel _targetLevel = PerformanceLevel.High;
        
        // Adaptation Parameters
        private float _adaptationSpeed = 0.1f;
        private float _stabilityThreshold = 0.05f;
        private int _framesToStabilize = 60;
        private int _currentStabilizationFrames = 0;
        
        // Performance History
        private NativeArray<float> _frameTimeHistory;
        private int _historyIndex = 0;
        private const int HISTORY_SIZE = 120; // 2 seconds at 60 FPS
        
        // System References
        private PerformanceProfiler _profiler;
        private MemoryPool _memoryPool;
        
        protected override void OnCreate()
        {
            // Инициализация истории производительности
            _frameTimeHistory = new NativeArray<float>(HISTORY_SIZE, Allocator.Persistent);
            
            // Инициализация значений истории
            for (int i = 0; i < HISTORY_SIZE; i++)
            {
                _frameTimeHistory[i] = _targetFrameTime;
            }
            
            // Получение ссылок на системы
            _profiler = World.GetExistingSystemManaged<PerformanceProfiler>();
            _memoryPool = World.GetExistingSystemManaged<MemoryPool>();
        }
        
        protected override void OnDestroy()
        {
            // Освобождение истории производительности
            if (_frameTimeHistory.IsCreated)
                _frameTimeHistory.Dispose();
        }
        
        protected override void OnUpdate()
        {
            // Обновление метрик производительности
            UpdatePerformanceMetrics();
            
            // Анализ производительности
            AnalyzePerformance();
            
            // Адаптация производительности
            AdaptPerformance();
            
            // Применение изменений
            ApplyPerformanceChanges();
        }
        
        /// <summary>
        /// Обновляет метрики производительности
        /// </summary>
        private void UpdatePerformanceMetrics()
        {
            // Запись текущего времени кадра
            _currentFrameTime = SystemAPI.Time.DeltaTime * 1000f; // в миллисекундах
            _frameTimeHistory[_historyIndex] = _currentFrameTime;
            _historyIndex = (_historyIndex + 1) % HISTORY_SIZE;
            
            // Вычисление среднего времени кадра
            float sum = 0f;
            for (int i = 0; i < HISTORY_SIZE; i++)
            {
                sum += _frameTimeHistory[i];
            }
            _averageFrameTime = sum / HISTORY_SIZE;
            
            // Вычисление дисперсии времени кадра
            float varianceSum = 0f;
            for (int i = 0; i < HISTORY_SIZE; i++)
            {
                float diff = _frameTimeHistory[i] - _averageFrameTime;
                varianceSum += diff * diff;
            }
            _frameTimeVariance = varianceSum / HISTORY_SIZE;
        }
        
        /// <summary>
        /// Анализирует производительность
        /// </summary>
        private void AnalyzePerformance()
        {
            // Определение целевого уровня производительности
            if (_averageFrameTime > _targetFrameTime * 1.2f)
            {
                _targetLevel = PerformanceLevel.Low;
            }
            else if (_averageFrameTime > _targetFrameTime * 1.1f)
            {
                _targetLevel = PerformanceLevel.Medium;
            }
            else if (_averageFrameTime < _targetFrameTime * 0.9f)
            {
                _targetLevel = PerformanceLevel.High;
            }
            else
            {
                _targetLevel = _currentLevel; // Сохраняем текущий уровень
            }
            
            // Проверка стабильности
            if (_frameTimeVariance < _stabilityThreshold)
            {
                _currentStabilizationFrames++;
            }
            else
            {
                _currentStabilizationFrames = 0;
            }
        }
        
        /// <summary>
        /// Адаптирует производительность
        /// </summary>
        private void AdaptPerformance()
        {
            // Адаптация только при стабильной производительности
            if (_currentStabilizationFrames >= _framesToStabilize)
            {
                if (_targetLevel != _currentLevel)
                {
                    // Плавный переход к целевому уровню
                    _currentLevel = _targetLevel;
                    _currentStabilizationFrames = 0;
                    
                    Debug.Log($"Performance Level Changed: {_currentLevel}");
                }
            }
        }
        
        /// <summary>
        /// Применяет изменения производительности
        /// </summary>
        private void ApplyPerformanceChanges()
        {
            switch (_currentLevel)
            {
                case PerformanceLevel.High:
                    ApplyHighPerformanceSettings();
                    break;
                case PerformanceLevel.Medium:
                    ApplyMediumPerformanceSettings();
                    break;
                case PerformanceLevel.Low:
                    ApplyLowPerformanceSettings();
                    break;
            }
        }
        
        /// <summary>
        /// Применяет настройки высокой производительности
        /// </summary>
        private void ApplyHighPerformanceSettings()
        {
            // Максимальное качество
            QualitySettings.SetQualityLevel(5); // Ultra
            
            // Высокое разрешение
            QualitySettings.pixelLightCount = 4;
            QualitySettings.shadowResolution = ShadowResolution.VeryHigh;
            QualitySettings.shadowDistance = 150f;
            QualitySettings.shadowCascades = 4;
            
            // Высокое качество рендеринга
            QualitySettings.antiAliasing = 4;
            QualitySettings.anisotropicFiltering = AnisotropicFiltering.Enable;
            QualitySettings.softVegetation = true;
            QualitySettings.realtimeReflectionProbes = true;
            
            // Высокое качество физики
            // Time.fixedDeltaTime = 0.01f; // 100 Hz - только для чтения в ECS
            Physics.defaultSolverIterations = 8;
            Physics.defaultSolverVelocityIterations = 2;
            
            // Высокое качество аудио
            AudioSettings.SetDSPBufferSize(256, 4);
            
            // Высокое качество частиц
            QualitySettings.particleRaycastBudget = 4096;
        }
        
        /// <summary>
        /// Применяет настройки средней производительности
        /// </summary>
        private void ApplyMediumPerformanceSettings()
        {
            // Среднее качество
            QualitySettings.SetQualityLevel(3); // Good
            
            // Среднее разрешение
            QualitySettings.pixelLightCount = 2;
            QualitySettings.shadowResolution = ShadowResolution.High;
            QualitySettings.shadowDistance = 100f;
            QualitySettings.shadowCascades = 2;
            
            // Среднее качество рендеринга
            QualitySettings.antiAliasing = 2;
            QualitySettings.anisotropicFiltering = AnisotropicFiltering.Enable;
            QualitySettings.softVegetation = false;
            QualitySettings.realtimeReflectionProbes = false;
            
            // Среднее качество физики
            // Time.fixedDeltaTime = 0.02f; // 50 Hz - только для чтения в ECS
            Physics.defaultSolverIterations = 6;
            Physics.defaultSolverVelocityIterations = 1;
            
            // Среднее качество аудио
            AudioSettings.SetDSPBufferSize(512, 4);
            
            // Среднее качество частиц
            QualitySettings.particleRaycastBudget = 2048;
        }
        
        /// <summary>
        /// Применяет настройки низкой производительности
        /// </summary>
        private void ApplyLowPerformanceSettings()
        {
            // Низкое качество
            QualitySettings.SetQualityLevel(1); // Fast
            
            // Низкое разрешение
            QualitySettings.pixelLightCount = 1;
            QualitySettings.shadowResolution = ShadowResolution.Low;
            QualitySettings.shadowDistance = 50f;
            QualitySettings.shadowCascades = 1;
            
            // Низкое качество рендеринга
            QualitySettings.antiAliasing = 0;
            QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
            QualitySettings.softVegetation = false;
            QualitySettings.realtimeReflectionProbes = false;
            
            // Низкое качество физики
            // Time.fixedDeltaTime = 0.033f; // 30 Hz - только для чтения в ECS
            Physics.defaultSolverIterations = 4;
            Physics.defaultSolverVelocityIterations = 1;
            
            // Низкое качество аудио
            AudioSettings.SetDSPBufferSize(1024, 2);
            
            // Низкое качество частиц
            QualitySettings.particleRaycastBudget = 1024;
        }
        
        /// <summary>
        /// Получает текущий уровень производительности
        /// </summary>
        public PerformanceLevel GetCurrentPerformanceLevel()
        {
            return _currentLevel;
        }
        
        /// <summary>
        /// Получает целевой уровень производительности
        /// </summary>
        public PerformanceLevel GetTargetPerformanceLevel()
        {
            return _targetLevel;
        }
        
        /// <summary>
        /// Получает среднее время кадра
        /// </summary>
        public float GetAverageFrameTime()
        {
            return _averageFrameTime;
        }
        
        /// <summary>
        /// Получает дисперсию времени кадра
        /// </summary>
        public float GetFrameTimeVariance()
        {
            return _frameTimeVariance;
        }
        
        /// <summary>
        /// Получает текущий FPS
        /// </summary>
        public float GetCurrentFPS()
        {
            return 1000f / _averageFrameTime;
        }
        
        /// <summary>
        /// Получает статистику производительности
        /// </summary>
        public AdaptivePerformanceStats GetPerformanceStats()
        {
            return new AdaptivePerformanceStats
            {
                CurrentLevel = _currentLevel,
                TargetLevel = _targetLevel,
                AverageFrameTime = _averageFrameTime,
                FrameTimeVariance = _frameTimeVariance,
                CurrentFPS = GetCurrentFPS(),
                StabilizationFrames = _currentStabilizationFrames,
                IsStable = _currentStabilizationFrames >= _framesToStabilize
            };
        }
        
        /// <summary>
        /// Принудительно устанавливает уровень производительности
        /// </summary>
        public void SetPerformanceLevel(PerformanceLevel level)
        {
            _currentLevel = level;
            _targetLevel = level;
            _currentStabilizationFrames = 0;
            
            ApplyPerformanceChanges();
            
            Debug.Log($"Performance Level Manually Set: {_currentLevel}");
        }
        
        /// <summary>
        /// Сбрасывает адаптивную систему производительности
        /// </summary>
        public void ResetAdaptivePerformance()
        {
            _currentLevel = PerformanceLevel.High;
            _targetLevel = PerformanceLevel.High;
            _currentStabilizationFrames = 0;
            _averageFrameTime = _targetFrameTime;
            _frameTimeVariance = 0f;
            
            // Сброс истории
            for (int i = 0; i < HISTORY_SIZE; i++)
            {
                _frameTimeHistory[i] = _targetFrameTime;
            }
            
            ApplyHighPerformanceSettings();
            
            Debug.Log("Adaptive Performance System Reset");
        }
    }
    
    /// <summary>
    /// Уровни производительности
    /// </summary>
    public enum PerformanceLevel
    {
        Low,     // Низкая производительность - максимальная оптимизация
        Medium,  // Средняя производительность - сбалансированные настройки
        High     // Высокая производительность - максимальное качество
    }
    
    /// <summary>
    /// Статистика адаптивной производительности
    /// </summary>
    public struct AdaptivePerformanceStats
    {
        public PerformanceLevel CurrentLevel;
        public PerformanceLevel TargetLevel;
        public float AverageFrameTime;
        public float FrameTimeVariance;
        public float CurrentFPS;
        public int StabilizationFrames;
        public bool IsStable;
    }
}