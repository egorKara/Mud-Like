using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace MudLike.Core.Systems
{
    /// <summary>
    /// Система адаптивного качества
    /// Автоматически регулирует настройки для предотвращения перегрева
    /// </summary>
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class AdaptiveQualitySystem : SystemBase
    {
        private SystemResourceMonitor _resourceMonitor;
        private OverheatProtectionSystem _overheatProtection;
        
        // Настройки адаптации
        private const float ADAPTATION_INTERVAL = 3f;        // Адаптируем каждые 3 секунды
        private const float STABILITY_THRESHOLD = 2f;        // Стабильность в секундах
        private const float IMPROVEMENT_DELAY = 5f;           // Задержка улучшения качества
        
        // Состояние адаптации
        private float _lastAdaptationTime;
        private float _stableTime;
        private float _lastImprovementTime;
        private int _currentQualityLevel;
        private bool _isStable;
        
        // История производительности
        private float[] _fpsHistory = new float[10];
        private int _historyIndex;
        private float _averageFPS;
        
        protected override void OnCreate()
        {
            _resourceMonitor = World.GetExistingSystemManaged<SystemResourceMonitor>();
            _overheatProtection = World.GetExistingSystemManaged<OverheatProtectionSystem>();
            
            _currentQualityLevel = QualitySettings.GetQualityLevel();
            _lastAdaptationTime = Time.time;
            _lastImprovementTime = Time.time;
            
            Debug.Log("[AdaptiveQuality] Система адаптивного качества активирована");
        }
        
        protected override void OnUpdate()
        {
            float currentTime = Time.time;
            
            // Обновляем историю FPS
            UpdateFPSHistory();
            
            // Адаптируем качество периодически
            if (currentTime - _lastAdaptationTime >= ADAPTATION_INTERVAL)
            {
                AdaptQuality();
                _lastAdaptationTime = currentTime;
            }
            
            // Проверяем стабильность
            CheckStability();
        }
        
        private void UpdateFPSHistory()
        {
            float currentFPS = 1f / Time.unscaledDeltaTime;
            _fpsHistory[_historyIndex] = currentFPS;
            _historyIndex = (_historyIndex + 1) % _fpsHistory.Length;
            
            // Вычисляем средний FPS
            float sum = 0f;
            int count = 0;
            for (int i = 0; i < _fpsHistory.Length; i++)
            {
                if (_fpsHistory[i] > 0f)
                {
                    sum += _fpsHistory[i];
                    count++;
                }
            }
            _averageFPS = count > 0 ? sum / count : 0f;
        }
        
        private void AdaptQuality()
        {
            if (_resourceMonitor == null) return;
            
            var stats = _resourceMonitor.GetCurrentStats();
            var loadLevel = stats.LoadLevel;
            
            // Определяем необходимость изменения качества
            bool shouldReduce = ShouldReduceQuality(stats);
            bool shouldImprove = ShouldImproveQuality(stats);
            
            if (shouldReduce)
            {
                ReduceQuality();
            }
            else if (shouldImprove && _isStable)
            {
                ImproveQuality();
            }
            
            // Логируем изменения
            if (shouldReduce || shouldImprove)
            {
                LogQualityChange();
            }
        }
        
        private bool ShouldReduceQuality(SystemResourceMonitor.SystemResourceStats stats)
        {
            // Критерии для снижения качества
            if (stats.LoadLevel >= SystemResourceMonitor.ResourceLoadLevel.Critical)
            {
                return true;
            }
            
            if (stats.CPUUsage > 80f || stats.RAMUsage > 85f || stats.GPUUsage > 90f)
            {
                return true;
            }
            
            if (_averageFPS < 30f && _currentQualityLevel > 0)
            {
                return true;
            }
            
            return false;
        }
        
        private bool ShouldImproveQuality(SystemResourceMonitor.SystemResourceStats stats)
        {
            // Критерии для улучшения качества
            if (stats.LoadLevel <= SystemResourceMonitor.ResourceLoadLevel.Medium)
            {
                return true;
            }
            
            if (stats.CPUUsage < 60f && stats.RAMUsage < 70f && stats.GPUUsage < 80f)
            {
                return true;
            }
            
            if (_averageFPS > 50f && _currentQualityLevel < 3)
            {
                return true;
            }
            
            return false;
        }
        
        private void ReduceQuality()
        {
            if (_currentQualityLevel > 0)
            {
                _currentQualityLevel--;
                ApplyQualitySettings();
                _lastImprovementTime = Time.time; // Сбрасываем таймер улучшения
            }
        }
        
        private void ImproveQuality()
        {
            // Улучшаем качество только если прошло достаточно времени с последнего улучшения
            if (Time.time - _lastImprovementTime >= IMPROVEMENT_DELAY)
            {
                if (_currentQualityLevel < 3)
                {
                    _currentQualityLevel++;
                    ApplyQualitySettings();
                    _lastImprovementTime = Time.time;
                }
            }
        }
        
        private int _lastAppliedQualityLevel = -1;
        private float _lastQualityChangeTime;
        private const float MIN_QUALITY_CHANGE_INTERVAL = 2f; // Минимум 2 секунды между изменениями
        
        private void ApplyQualitySettings()
        {
            // Защита от частых изменений качества
            float currentTime = Time.time;
            if (_currentQualityLevel == _lastAppliedQualityLevel || 
                (currentTime - _lastQualityChangeTime < MIN_QUALITY_CHANGE_INTERVAL))
            {
                return; // Пропускаем изменение если оно слишком частое или то же самое
            }
            
            try
            {
                QualitySettings.SetQualityLevel(_currentQualityLevel);
                _lastAppliedQualityLevel = _currentQualityLevel;
                _lastQualityChangeTime = currentTime;
                
                // Дополнительные настройки в зависимости от уровня качества
                switch (_currentQualityLevel)
                {
                    case 0: // Минимальное качество
                        ApplyMinimalQuality();
                        break;
                    case 1: // Низкое качество
                        ApplyLowQuality();
                        break;
                    case 2: // Среднее качество
                        ApplyMediumQuality();
                        break;
                    case 3: // Высокое качество
                        ApplyHighQuality();
                        break;
                }
                
                // Устанавливаем соответствующий FPS
                SetTargetFPSForQuality(_currentQualityLevel);
                
                Debug.Log($"[AdaptiveQuality] Качество применено: {_currentQualityLevel}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[AdaptiveQuality] Ошибка применения качества: {e.Message}");
            }
        }
        
        private void ApplyMinimalQuality()
        {
            QualitySettings.pixelLightCount = 1;
            QualitySettings.shadowResolution = ShadowResolution.Low;
            QualitySettings.shadowDistance = 20f;
            QualitySettings.lodBias = 3f;
            QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
            QualitySettings.antiAliasing = 0;
        }
        
        private void ApplyLowQuality()
        {
            QualitySettings.pixelLightCount = 2;
            QualitySettings.shadowResolution = ShadowResolution.Low;
            QualitySettings.shadowDistance = 50f;
            QualitySettings.lodBias = 2f;
            QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
            QualitySettings.antiAliasing = 0;
        }
        
        private void ApplyMediumQuality()
        {
            QualitySettings.pixelLightCount = 4;
            QualitySettings.shadowResolution = ShadowResolution.Medium;
            QualitySettings.shadowDistance = 100f;
            QualitySettings.lodBias = 1.5f;
            QualitySettings.anisotropicFiltering = AnisotropicFiltering.Enable;
            QualitySettings.antiAliasing = 2;
        }
        
        private void ApplyHighQuality()
        {
            QualitySettings.pixelLightCount = 8;
            QualitySettings.shadowResolution = ShadowResolution.High;
            QualitySettings.shadowDistance = 150f;
            QualitySettings.lodBias = 1f;
            QualitySettings.anisotropicFiltering = AnisotropicFiltering.Enable;
            QualitySettings.antiAliasing = 4;
        }
        
        private void SetTargetFPSForQuality(int qualityLevel)
        {
            int targetFPS;
            switch (qualityLevel)
            {
                case 0: targetFPS = 30; break;
                case 1: targetFPS = 45; break;
                case 2: targetFPS = 60; break;
                case 3: targetFPS = 90; break;
                default: targetFPS = 60; break;
            }
            
            Application.targetFrameRate = targetFPS;
        }
        
        private void CheckStability()
        {
            // Проверяем стабильность FPS
            float fpsVariance = CalculateFPSVariance();
            _isStable = fpsVariance < 10f; // FPS не должен колебаться более чем на 10
            
            if (_isStable)
            {
                _stableTime += Time.deltaTime;
            }
            else
            {
                _stableTime = 0f;
            }
        }
        
        private float CalculateFPSVariance()
        {
            if (_fpsHistory.Length < 2) return 0f;
            
            float sum = 0f;
            int count = 0;
            for (int i = 0; i < _fpsHistory.Length; i++)
            {
                if (_fpsHistory[i] > 0f)
                {
                    sum += _fpsHistory[i];
                    count++;
                }
            }
            
            if (count < 2) return 0f;
            
            float average = sum / count;
            float variance = 0f;
            
            for (int i = 0; i < _fpsHistory.Length; i++)
            {
                if (_fpsHistory[i] > 0f)
                {
                    float diff = _fpsHistory[i] - average;
                    variance += diff * diff;
                }
            }
            
            return math.sqrt(variance / count);
        }
        
        private void LogQualityChange()
        {
            string qualityName = GetQualityName(_currentQualityLevel);
            Debug.Log($"[AdaptiveQuality] Качество изменено на: {qualityName} " +
                     $"(FPS: {_averageFPS:F1}, Стабильно: {_isStable})");
        }
        
        private string GetQualityName(int level)
        {
            switch (level)
            {
                case 0: return "Минимальное";
                case 1: return "Низкое";
                case 2: return "Среднее";
                case 3: return "Высокое";
                default: return "Неизвестно";
            }
        }
        
        /// <summary>
        /// Принудительно установить уровень качества
        /// </summary>
        public void SetQualityLevel(int level)
        {
            _currentQualityLevel = math.clamp(level, 0, 3);
            ApplyQualitySettings();
            Debug.Log($"[AdaptiveQuality] Принудительно установлено качество: {GetQualityName(_currentQualityLevel)}");
        }
        
        /// <summary>
        /// Получить текущий уровень качества
        /// </summary>
        public int GetCurrentQualityLevel()
        {
            return _currentQualityLevel;
        }
        
        /// <summary>
        /// Получить средний FPS
        /// </summary>
        public float GetAverageFPS()
        {
            return _averageFPS;
        }
        
        /// <summary>
        /// Проверить, стабильна ли система
        /// </summary>
        public bool IsSystemStable()
        {
            return _isStable;
        }
    }
}