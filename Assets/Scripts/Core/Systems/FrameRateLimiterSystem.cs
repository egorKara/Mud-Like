using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace MudLike.Core.Systems
{
    /// <summary>
    /// Система ограничения FPS и адаптивного качества
    /// Предотвращает перегрузку системы и зависания
    /// </summary>
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class FrameRateLimiterSystem : SystemBase
    {
        private PerformanceMonitorSystem _performanceMonitor;
        private GPUOptimizationSystem _gpuOptimizer;
        private MemoryOptimizationSystem _memoryOptimizer;
        
        // Настройки FPS
        private int _targetFrameRate = 60;
        private int _minFrameRate = 30;
        private int _maxFrameRate = 120;
        
        // Адаптивные настройки
        private bool _enableAdaptiveFPS = true;
        private bool _enableVSync = true;
        private bool _enableFrameRateLimiting = true;
        
        // Статистика
        private float _currentFrameRate;
        private float _averageFrameRate;
        private float _frameRateVariance;
        private int _frameCount;
        private float _accumulatedFrameTime;
        
        // Настройки адаптации
        private const float ADAPTATION_INTERVAL = 2f; // Адаптируем каждые 2 секунды
        private const float LOW_FPS_THRESHOLD = 45f;
        private const float HIGH_FPS_THRESHOLD = 55f;
        private const float STABLE_FPS_THRESHOLD = 5f; // Разброс FPS
        
        private float _lastAdaptationTime;
        
        protected override void OnCreate()
        {
            // Получаем ссылки на другие системы
            _performanceMonitor = World.GetExistingSystemManaged<PerformanceMonitorSystem>();
            _gpuOptimizer = World.GetExistingSystemManaged<GPUOptimizationSystem>();
            _memoryOptimizer = World.GetExistingSystemManaged<MemoryOptimizationSystem>();
            
            // Инициализируем настройки
            InitializeFrameRateSettings();
        }
        
        protected override void OnUpdate()
        {
            UpdateFrameRateStats();
            UpdateAdaptiveSettings();
            ApplyFrameRateLimits();
        }
        
        private void InitializeFrameRateSettings()
        {
            // Устанавливаем начальные настройки
            Application.targetFrameRate = _targetFrameRate;
            QualitySettings.vSyncCount = _enableVSync ? 1 : 0;
            
            // Настраиваем качество в зависимости от производительности
            int qualityLevel = GetOptimalQualityLevel();
            SetQualityLevel(qualityLevel);
            
            Debug.Log($"[FrameRate] Инициализация: Target FPS = {_targetFrameRate}, VSync = {_enableVSync}, Quality = {qualityLevel}");
        }
        
        private void UpdateFrameRateStats()
        {
            float currentTime = Time.unscaledTime;
            float deltaTime = Time.unscaledDeltaTime;
            
            _frameCount++;
            _accumulatedFrameTime += deltaTime;
            
            // Обновляем статистику каждые 0.5 секунды
            if (_accumulatedFrameTime >= 0.5f)
            {
                _currentFrameRate = _frameCount / _accumulatedFrameTime;
                
                // Вычисляем средний FPS
                _averageFrameRate = (_averageFrameRate + _currentFrameRate) * 0.5f;
                
                // Вычисляем разброс FPS
                float variance = math.abs(_currentFrameRate - _averageFrameRate);
                _frameRateVariance = (_frameRateVariance + variance) * 0.5f;
                
                _frameCount = 0;
                _accumulatedFrameTime = 0f;
            }
        }
        
        private void UpdateAdaptiveSettings()
        {
            if (!_enableAdaptiveFPS) return;
            
            float currentTime = Time.time;
            
            // Адаптируем настройки периодически
            if (currentTime - _lastAdaptationTime >= ADAPTATION_INTERVAL)
            {
                AdaptFrameRateSettings();
                _lastAdaptationTime = currentTime;
            }
        }
        
        private void AdaptFrameRateSettings()
        {
            bool settingsChanged = false;
            
            // Адаптируем FPS
            if (_currentFrameRate < LOW_FPS_THRESHOLD)
            {
                // Снижаем FPS если производительность низкая
                int newTargetFPS = math.max(_minFrameRate, _targetFrameRate - 10);
                if (newTargetFPS != _targetFrameRate)
                {
                    _targetFrameRate = newTargetFPS;
                    settingsChanged = true;
                    Debug.Log($"[FrameRate] Снижение FPS: {_targetFrameRate} (текущий: {_currentFrameRate:F1})");
                }
            }
            else if (_currentFrameRate > HIGH_FPS_THRESHOLD && _frameRateVariance < STABLE_FPS_THRESHOLD)
            {
                // Повышаем FPS если производительность стабильная
                int newTargetFPS = math.min(_maxFrameRate, _targetFrameRate + 5);
                if (newTargetFPS != _targetFrameRate)
                {
                    _targetFrameRate = newTargetFPS;
                    settingsChanged = true;
                    Debug.Log($"[FrameRate] Повышение FPS: {_targetFrameRate} (текущий: {_currentFrameRate:F1})");
                }
            }
            
            // Адаптируем качество
            if (_currentFrameRate < LOW_FPS_THRESHOLD)
            {
                // Снижаем качество если FPS низкий
                int currentQuality = QualitySettings.GetQualityLevel();
                if (currentQuality > 0)
                {
                    SetQualityLevel(currentQuality - 1);
                    settingsChanged = true;
                    Debug.Log($"[FrameRate] Снижение качества: {currentQuality} -> {currentQuality - 1}");
                }
            }
            else if (_currentFrameRate > HIGH_FPS_THRESHOLD && _frameRateVariance < STABLE_FPS_THRESHOLD)
            {
                // Повышаем качество если FPS стабильный
                int currentQuality = QualitySettings.GetQualityLevel();
                if (currentQuality < 3)
                {
                    SetQualityLevel(currentQuality + 1);
                    settingsChanged = true;
                    Debug.Log($"[FrameRate] Повышение качества: {currentQuality} -> {currentQuality + 1}");
                }
            }
            
            if (settingsChanged)
            {
                ApplyFrameRateLimits();
            }
        }
        
        private int _lastAppliedFPS = -1;
        private float _lastFPSChangeTime;
        private const float MIN_FPS_CHANGE_INTERVAL = 1f; // Минимум 1 секунда между изменениями FPS
        
        private void ApplyFrameRateLimits()
        {
            if (!_enableFrameRateLimiting) return;
            
            // Защита от частых изменений FPS
            float currentTime = Time.time;
            if (_targetFrameRate != _lastAppliedFPS || 
                (currentTime - _lastFPSChangeTime >= MIN_FPS_CHANGE_INTERVAL))
            {
                try
                {
                    Application.targetFrameRate = _targetFrameRate;
                    _lastAppliedFPS = _targetFrameRate;
                    _lastFPSChangeTime = currentTime;
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"[FrameRate] Ошибка установки FPS: {e.Message}");
                }
            }
            
            // Настраиваем VSync
            QualitySettings.vSyncCount = _enableVSync ? 1 : 0;
            
            // Применяем настройки качества
            int qualityLevel = GetOptimalQualityLevel();
            SetQualityLevel(qualityLevel);
        }
        
        private int GetOptimalQualityLevel()
        {
            // Определяем оптимальный уровень качества на основе производительности
            if (_currentFrameRate < 30f) return 0; // Low
            if (_currentFrameRate < 45f) return 1; // Medium
            if (_currentFrameRate < 60f) return 2; // High
            return 3; // Ultra
        }
        
        private void SetQualityLevel(int level)
        {
            level = math.clamp(level, 0, 3);
            QualitySettings.SetQualityLevel(level);
            
            // Применяем дополнительные настройки
            switch (level)
            {
                case 0: // Low
                    QualitySettings.pixelLightCount = 1;
                    QualitySettings.shadowResolution = ShadowResolution.Low;
                    QualitySettings.shadowDistance = 50f;
                    break;
                case 1: // Medium
                    QualitySettings.pixelLightCount = 2;
                    QualitySettings.shadowResolution = ShadowResolution.Medium;
                    QualitySettings.shadowDistance = 100f;
                    break;
                case 2: // High
                    QualitySettings.pixelLightCount = 4;
                    QualitySettings.shadowResolution = ShadowResolution.High;
                    QualitySettings.shadowDistance = 150f;
                    break;
                case 3: // Ultra
                    QualitySettings.pixelLightCount = 8;
                    QualitySettings.shadowResolution = ShadowResolution.VeryHigh;
                    QualitySettings.shadowDistance = 200f;
                    break;
            }
        }
        
        /// <summary>
        /// Установить целевой FPS
        /// </summary>
        public void SetTargetFrameRate(int fps)
        {
            int newFPS = math.clamp(fps, _minFrameRate, _maxFrameRate);
            
            // Защита от частых изменений FPS
            float currentTime = Time.time;
            if (newFPS == _lastAppliedFPS || 
                (currentTime - _lastFPSChangeTime < MIN_FPS_CHANGE_INTERVAL))
            {
                return; // Пропускаем изменение если оно слишком частое или то же самое
            }
            
            try
            {
                _targetFrameRate = newFPS;
                Application.targetFrameRate = _targetFrameRate;
                _lastAppliedFPS = _targetFrameRate;
                _lastFPSChangeTime = currentTime;
                
                Debug.Log($"[FrameRate] Установлен целевой FPS: {_targetFrameRate}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[FrameRate] Ошибка установки FPS: {e.Message}");
            }
        }
        
        /// <summary>
        /// Включить/выключить адаптивный FPS
        /// </summary>
        public void SetAdaptiveFPS(bool enabled)
        {
            _enableAdaptiveFPS = enabled;
            Debug.Log($"[FrameRate] Адаптивный FPS: {enabled}");
        }
        
        /// <summary>
        /// Включить/выключить VSync
        /// </summary>
        public void SetVSync(bool enabled)
        {
            _enableVSync = enabled;
            QualitySettings.vSyncCount = enabled ? 1 : 0;
            Debug.Log($"[FrameRate] VSync: {enabled}");
        }
        
        /// <summary>
        /// Включить/выключить ограничение FPS
        /// </summary>
        public void SetFrameRateLimiting(bool enabled)
        {
            _enableFrameRateLimiting = enabled;
            if (!enabled)
            {
                Application.targetFrameRate = -1; // Без ограничений
            }
            Debug.Log($"[FrameRate] Ограничение FPS: {enabled}");
        }
        
        /// <summary>
        /// Получить текущую статистику FPS
        /// </summary>
        public FrameRateStats GetFrameRateStats()
        {
            return new FrameRateStats
            {
                CurrentFPS = _currentFrameRate,
                AverageFPS = _averageFrameRate,
                TargetFPS = _targetFrameRate,
                FrameRateVariance = _frameRateVariance,
                IsStable = _frameRateVariance < STABLE_FPS_THRESHOLD
            };
        }
    }
    
    /// <summary>
    /// Статистика FPS
    /// </summary>
    public struct FrameRateStats
    {
        public float CurrentFPS;
        public float AverageFPS;
        public int TargetFPS;
        public float FrameRateVariance;
        public bool IsStable;
    }
}