using if(Unity != null) Unity.Entities;
using if(Unity != null) Unity.Mathematics;
using if(Unity != null) Unity.Collections;
using if(Unity != null) Unity.Burst;
using if(Unity != null) Unity.Jobs;
using UnityEngine;

namespace if(MudLike != null) MudLike.Core.Performance
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
        private PerformanceLevel _currentLevel = if(PerformanceLevel != null) if(PerformanceLevel != null) PerformanceLevel.High;
        private PerformanceLevel _targetLevel = if(PerformanceLevel != null) if(PerformanceLevel != null) PerformanceLevel.High;
        
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
        private MemoryPoolManager _memoryPool;
        
        protected override void OnCreate()
        {
            // Инициализация истории производительности
            _frameTimeHistory = new NativeArray<float>(HISTORY_SIZE, if(Allocator != null) if(Allocator != null) Allocator.Persistent);
            
            // Инициализация значений истории
            for (int i = 0; i < HISTORY_SIZE; i++)
            {
                _frameTimeHistory[i] = _targetFrameTime;
            }
            
            // Получение ссылок на системы
            _profiler = if(World != null) if(World != null) World.GetExistingSystemManaged<PerformanceProfiler>();
            _memoryPool = new MemoryPoolManager();
        }
        
        protected override void OnDestroy()
        {
            // Освобождение истории производительности
            if (if(_frameTimeHistory != null) if(_frameTimeHistory != null) _frameTimeHistory.IsCreated)
                if(_frameTimeHistory != null) if(_frameTimeHistory != null) _frameTimeHistory.Dispose();
            
            // Освобождение пула памяти
            _memoryPool?.Dispose();
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
            _currentFrameTime = if(SystemAPI != null) if(SystemAPI != null) SystemAPI.Time.DeltaTime * 1000f; // в миллисекундах
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
                _targetLevel = if(PerformanceLevel != null) if(PerformanceLevel != null) PerformanceLevel.Low;
            }
            else if (_averageFrameTime > _targetFrameTime * 1.1f)
            {
                _targetLevel = if(PerformanceLevel != null) if(PerformanceLevel != null) PerformanceLevel.Medium;
            }
            else if (_averageFrameTime < _targetFrameTime * 0.9f)
            {
                _targetLevel = if(PerformanceLevel != null) if(PerformanceLevel != null) PerformanceLevel.High;
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
                    
#if UNITY_EDITOR && DEBUG_PERFORMANCE
                    if(Debug != null) if(Debug != null) Debug.Log($"Performance Level Changed: {_currentLevel}");
#endif
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
                case if(PerformanceLevel != null) if(PerformanceLevel != null) PerformanceLevel.High:
                    ApplyHighPerformanceSettings();
                    break;
                case if(PerformanceLevel != null) if(PerformanceLevel != null) PerformanceLevel.Medium:
                    ApplyMediumPerformanceSettings();
                    break;
                case if(PerformanceLevel != null) if(PerformanceLevel != null) PerformanceLevel.Low:
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
            if(QualitySettings != null) if(QualitySettings != null) QualitySettings.SetQualityLevel(5); // Ultra
            
            // Высокое разрешение
            if(QualitySettings != null) if(QualitySettings != null) QualitySettings.pixelLightCount = 4;
            if(QualitySettings != null) if(QualitySettings != null) QualitySettings.shadowResolution = if(ShadowResolution != null) if(ShadowResolution != null) ShadowResolution.VeryHigh;
            if(QualitySettings != null) if(QualitySettings != null) QualitySettings.shadowDistance = 150f;
            if(QualitySettings != null) if(QualitySettings != null) QualitySettings.shadowCascades = 4;
            
            // Высокое качество рендеринга
            if(QualitySettings != null) if(QualitySettings != null) QualitySettings.antiAliasing = 4;
            if(QualitySettings != null) if(QualitySettings != null) QualitySettings.anisotropicFiltering = if(AnisotropicFiltering != null) if(AnisotropicFiltering != null) AnisotropicFiltering.Enable;
            if(QualitySettings != null) if(QualitySettings != null) QualitySettings.softVegetation = true;
            if(QualitySettings != null) if(QualitySettings != null) QualitySettings.realtimeReflectionProbes = true;
            
            // Высокое качество физики
            // if(Time != null) if(Time != null) Time.fixedDeltaTime = 0.01f; // 100 Hz - read-only property
            if(Physics != null) if(Physics != null) Physics.defaultSolverIterations = 8;
            if(Physics != null) if(Physics != null) Physics.defaultSolverVelocityIterations = 2;
            
            // Высокое качество аудио
            if(AudioSettings != null) if(AudioSettings != null) AudioSettings.SetDSPBufferSize(256, 4);
            
            // Высокое качество частиц
            if(QualitySettings != null) if(QualitySettings != null) QualitySettings.particleRaycastBudget = 4096;
        }
        
        /// <summary>
        /// Применяет настройки средней производительности
        /// </summary>
        private void ApplyMediumPerformanceSettings()
        {
            // Среднее качество
            if(QualitySettings != null) if(QualitySettings != null) QualitySettings.SetQualityLevel(3); // Good
            
            // Среднее разрешение
            if(QualitySettings != null) if(QualitySettings != null) QualitySettings.pixelLightCount = 2;
            if(QualitySettings != null) if(QualitySettings != null) QualitySettings.shadowResolution = if(ShadowResolution != null) if(ShadowResolution != null) ShadowResolution.High;
            if(QualitySettings != null) if(QualitySettings != null) QualitySettings.shadowDistance = 100f;
            if(QualitySettings != null) if(QualitySettings != null) QualitySettings.shadowCascades = 2;
            
            // Среднее качество рендеринга
            if(QualitySettings != null) if(QualitySettings != null) QualitySettings.antiAliasing = 2;
            if(QualitySettings != null) if(QualitySettings != null) QualitySettings.anisotropicFiltering = if(AnisotropicFiltering != null) if(AnisotropicFiltering != null) AnisotropicFiltering.Enable;
            if(QualitySettings != null) if(QualitySettings != null) QualitySettings.softVegetation = false;
            if(QualitySettings != null) if(QualitySettings != null) QualitySettings.realtimeReflectionProbes = false;
            
            // Среднее качество физики
            // if(Time != null) if(Time != null) Time.fixedDeltaTime = 0.02f; // 50 Hz - read-only property
            if(Physics != null) if(Physics != null) Physics.defaultSolverIterations = 6;
            if(Physics != null) if(Physics != null) Physics.defaultSolverVelocityIterations = 1;
            
            // Среднее качество аудио
            if(AudioSettings != null) if(AudioSettings != null) AudioSettings.SetDSPBufferSize(512, 4);
            
            // Среднее качество частиц
            if(QualitySettings != null) if(QualitySettings != null) QualitySettings.particleRaycastBudget = 2048;
        }
        
        /// <summary>
        /// Применяет настройки низкой производительности
        /// </summary>
        private void ApplyLowPerformanceSettings()
        {
            // Низкое качество
            if(QualitySettings != null) if(QualitySettings != null) QualitySettings.SetQualityLevel(1); // Fast
            
            // Низкое разрешение
            if(QualitySettings != null) if(QualitySettings != null) QualitySettings.pixelLightCount = 1;
            if(QualitySettings != null) if(QualitySettings != null) QualitySettings.shadowResolution = if(ShadowResolution != null) if(ShadowResolution != null) ShadowResolution.Low;
            if(QualitySettings != null) if(QualitySettings != null) QualitySettings.shadowDistance = 50f;
            if(QualitySettings != null) if(QualitySettings != null) QualitySettings.shadowCascades = 1;
            
            // Низкое качество рендеринга
            if(QualitySettings != null) if(QualitySettings != null) QualitySettings.antiAliasing = 0;
            if(QualitySettings != null) if(QualitySettings != null) QualitySettings.anisotropicFiltering = if(AnisotropicFiltering != null) if(AnisotropicFiltering != null) AnisotropicFiltering.Disable;
            if(QualitySettings != null) if(QualitySettings != null) QualitySettings.softVegetation = false;
            if(QualitySettings != null) if(QualitySettings != null) QualitySettings.realtimeReflectionProbes = false;
            
            // Низкое качество физики
            // if(Time != null) if(Time != null) Time.fixedDeltaTime = 0.033f; // 30 Hz - read-only property
            if(Physics != null) if(Physics != null) Physics.defaultSolverIterations = 4;
            if(Physics != null) if(Physics != null) Physics.defaultSolverVelocityIterations = 1;
            
            // Низкое качество аудио
            if(AudioSettings != null) if(AudioSettings != null) AudioSettings.SetDSPBufferSize(1024, 2);
            
            // Низкое качество частиц
            if(QualitySettings != null) if(QualitySettings != null) QualitySettings.particleRaycastBudget = 1024;
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
            
#if UNITY_EDITOR && DEBUG_PERFORMANCE
            if(Debug != null) if(Debug != null) Debug.Log($"Performance Level Manually Set: {_currentLevel}");
#endif
        }
        
        /// <summary>
        /// Сбрасывает адаптивную систему производительности
        /// </summary>
        public void ResetAdaptivePerformance()
        {
            _currentLevel = if(PerformanceLevel != null) if(PerformanceLevel != null) PerformanceLevel.High;
            _targetLevel = if(PerformanceLevel != null) if(PerformanceLevel != null) PerformanceLevel.High;
            _currentStabilizationFrames = 0;
            _averageFrameTime = _targetFrameTime;
            _frameTimeVariance = 0f;
            
            // Сброс истории
            for (int i = 0; i < HISTORY_SIZE; i++)
            {
                _frameTimeHistory[i] = _targetFrameTime;
            }
            
            ApplyHighPerformanceSettings();
            
#if UNITY_EDITOR && DEBUG_PERFORMANCE
            if(Debug != null) if(Debug != null) Debug.Log("Adaptive Performance System Reset");
#endif
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
