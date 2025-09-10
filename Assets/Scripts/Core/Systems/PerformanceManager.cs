using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace MudLike.Core.Systems
{
    /// <summary>
    /// ГЛАВНАЯ СИСТЕМА УПРАВЛЕНИЯ ПРОИЗВОДИТЕЛЬНОСТЬЮ
    /// Координирует все системы оптимизации для предотвращения перегрева ноутбука
    /// </summary>
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class PerformanceManager : SystemBase
    {
        // Ссылки на системы
        private OverheatProtectionSystem _overheatProtection;
        private SystemResourceMonitor _resourceMonitor;
        private AdaptiveQualitySystem _adaptiveQuality;
        private PerformanceMonitorSystem _performanceMonitor;
        private FrameRateLimiterSystem _frameRateLimiter;
        private GPUOptimizationSystem _gpuOptimizer;
        private MemoryOptimizationSystem _memoryOptimizer;
        private CPUOptimizationSystem _cpuOptimizer;
        private PerformanceUISystem _performanceUI;
        
        // Настройки
        private const float COORDINATION_INTERVAL = 1f;  // Координируем каждую секунду
        private float _lastCoordinationTime;
        
        // Состояние системы
        private bool _allSystemsInitialized;
        private bool _emergencyModeActive;
        private PerformanceLevel _currentPerformanceLevel;
        
        public enum PerformanceLevel
        {
            Emergency,  // Экстренный режим - минимальная нагрузка
            Critical,   // Критический режим - очень низкая нагрузка
            Low,        // Низкая производительность - низкая нагрузка
            Medium,     // Средняя производительность - средняя нагрузка
            High        // Высокая производительность - высокая нагрузка
        }
        
        protected override void OnCreate()
        {
            Debug.Log("[PerformanceManager] 🚀 Инициализация системы управления производительностью");
            Debug.Log("[PerformanceManager] ⚠️ ПРИОРИТЕТ: Предотвращение перегрева ноутбука!");
            
            // Инициализируем системы в правильном порядке
            InitializeSystems();
        }
        
        protected override void OnUpdate()
        {
            if (!_allSystemsInitialized) return;
            
            float currentTime = Time.time;
            
            // Координируем системы периодически
            if (currentTime - _lastCoordinationTime >= COORDINATION_INTERVAL)
            {
                CoordinateSystems();
                _lastCoordinationTime = currentTime;
            }
            
            // Проверяем экстренные ситуации
            CheckEmergencyConditions();
        }
        
        private void InitializeSystems()
        {
            try
            {
                // 1. Сначала инициализируем системы мониторинга
                _overheatProtection = World.GetOrCreateSystemManaged<OverheatProtectionSystem>();
                _resourceMonitor = World.GetOrCreateSystemManaged<SystemResourceMonitor>();
                _performanceMonitor = World.GetOrCreateSystemManaged<PerformanceMonitorSystem>();
                
                // 2. Затем системы оптимизации
                _adaptiveQuality = World.GetOrCreateSystemManaged<AdaptiveQualitySystem>();
                _frameRateLimiter = World.GetOrCreateSystemManaged<FrameRateLimiterSystem>();
                _gpuOptimizer = World.GetOrCreateSystemManaged<GPUOptimizationSystem>();
                _memoryOptimizer = World.GetOrCreateSystemManaged<MemoryOptimizationSystem>();
                _cpuOptimizer = World.GetOrCreateSystemManaged<CPUOptimizationSystem>();
                
                // 3. Наконец UI
                _performanceUI = World.GetOrCreateSystemManaged<PerformanceUISystem>();
                
                _allSystemsInitialized = true;
                
                // Устанавливаем консервативные настройки по умолчанию
                SetConservativeSettings();
                
                Debug.Log("[PerformanceManager] ✅ Все системы инициализированы");
                LogSystemStatus();
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[PerformanceManager] ❌ Ошибка инициализации: {e.Message}");
            }
        }
        
        private void CoordinateSystems()
        {
            if (_resourceMonitor == null) return;
            
            var stats = _resourceMonitor.GetCurrentStats();
            var loadLevel = stats.LoadLevel;
            
            // Определяем уровень производительности
            PerformanceLevel newLevel = DeterminePerformanceLevel(stats);
            
            if (newLevel != _currentPerformanceLevel)
            {
                _currentPerformanceLevel = newLevel;
                ApplyPerformanceLevel(newLevel);
                LogPerformanceLevelChange(newLevel);
            }
            
            // Координируем системы в зависимости от уровня
            CoordinateBasedOnLevel(newLevel, stats);
        }
        
        private PerformanceLevel DeterminePerformanceLevel(SystemResourceMonitor.SystemResourceStats stats)
        {
            // Критерии для определения уровня производительности
            if (stats.LoadLevel == SystemResourceMonitor.ResourceLoadLevel.Overloaded ||
                stats.CPUUsage > 90f || stats.RAMUsage > 95f || stats.GPUUsage > 95f)
            {
                return PerformanceLevel.Emergency;
            }
            
            if (stats.LoadLevel == SystemResourceMonitor.ResourceLoadLevel.Critical ||
                stats.CPUUsage > 80f || stats.RAMUsage > 85f || stats.GPUUsage > 90f)
            {
                return PerformanceLevel.Critical;
            }
            
            if (stats.LoadLevel == SystemResourceMonitor.ResourceLoadLevel.High ||
                stats.CPUUsage > 70f || stats.RAMUsage > 75f || stats.GPUUsage > 80f)
            {
                return PerformanceLevel.Low;
            }
            
            if (stats.LoadLevel == SystemResourceMonitor.ResourceLoadLevel.Medium ||
                stats.CPUUsage > 50f || stats.RAMUsage > 60f || stats.GPUUsage > 60f)
            {
                return PerformanceLevel.Medium;
            }
            
            return PerformanceLevel.High;
        }
        
        private void ApplyPerformanceLevel(PerformanceLevel level)
        {
            switch (level)
            {
                case PerformanceLevel.Emergency:
                    ApplyEmergencySettings();
                    break;
                case PerformanceLevel.Critical:
                    ApplyCriticalSettings();
                    break;
                case PerformanceLevel.Low:
                    ApplyLowSettings();
                    break;
                case PerformanceLevel.Medium:
                    ApplyMediumSettings();
                    break;
                case PerformanceLevel.High:
                    ApplyHighSettings();
                    break;
            }
        }
        
        private void ApplyEmergencySettings()
        {
            Debug.LogError("[PerformanceManager] 🚨 ЭКСТРЕННЫЙ РЕЖИМ АКТИВИРОВАН!");
            
            // Минимальные настройки
            Application.targetFrameRate = 15;
            QualitySettings.SetQualityLevel(0);
            QualitySettings.vSyncCount = 0;
            
            // Отключаем несущественные системы
            if (_performanceUI != null)
                _performanceUI.SetUIEnabled(false);
            
            _emergencyModeActive = true;
        }
        
        private void ApplyCriticalSettings()
        {
            Debug.LogWarning("[PerformanceManager] ⚠️ КРИТИЧЕСКИЙ РЕЖИМ АКТИВИРОВАН!");
            
            Application.targetFrameRate = 30;
            QualitySettings.SetQualityLevel(0);
            QualitySettings.vSyncCount = 0;
            
            _emergencyModeActive = false;
        }
        
        private void ApplyLowSettings()
        {
            Debug.Log("[PerformanceManager] 🔶 Низкий уровень производительности");
            
            Application.targetFrameRate = 45;
            QualitySettings.SetQualityLevel(1);
            QualitySettings.vSyncCount = 0;
            
            _emergencyModeActive = false;
        }
        
        private void ApplyMediumSettings()
        {
            Debug.Log("[PerformanceManager] 🔷 Средний уровень производительности");
            
            Application.targetFrameRate = 60;
            QualitySettings.SetQualityLevel(2);
            QualitySettings.vSyncCount = 1;
            
            _emergencyModeActive = false;
        }
        
        private void ApplyHighSettings()
        {
            Debug.Log("[PerformanceManager] 🔵 Высокий уровень производительности");
            
            Application.targetFrameRate = 90;
            QualitySettings.SetQualityLevel(3);
            QualitySettings.vSyncCount = 1;
            
            _emergencyModeActive = false;
        }
        
        private void CoordinateBasedOnLevel(PerformanceLevel level, SystemResourceMonitor.SystemResourceStats stats)
        {
            // Координируем системы в зависимости от уровня производительности
            switch (level)
            {
                case PerformanceLevel.Emergency:
                    CoordinateEmergencyMode(stats);
                    break;
                case PerformanceLevel.Critical:
                    CoordinateCriticalMode(stats);
                    break;
                case PerformanceLevel.Low:
                    CoordinateLowMode(stats);
                    break;
                case PerformanceLevel.Medium:
                    CoordinateMediumMode(stats);
                    break;
                case PerformanceLevel.High:
                    CoordinateHighMode(stats);
                    break;
            }
        }
        
        private void CoordinateEmergencyMode(SystemResourceMonitor.SystemResourceStats stats)
        {
            // В экстренном режиме максимально снижаем нагрузку
            if (_memoryOptimizer != null)
            {
                _memoryOptimizer.ClearAllCaches();
                System.GC.Collect();
            }
            
            if (_gpuOptimizer != null)
            {
                _gpuOptimizer.SetRenderQuality(0);
            }
        }
        
        private void CoordinateCriticalMode(SystemResourceMonitor.SystemResourceStats stats)
        {
            // В критическом режиме активно оптимизируем
            if (_adaptiveQuality != null)
            {
                _adaptiveQuality.SetQualityLevel(0);
            }
            
            if (_frameRateLimiter != null)
            {
                _frameRateLimiter.SetTargetFrameRate(30);
                _frameRateLimiter.SetAdaptiveFPS(true);
            }
        }
        
        private void CoordinateLowMode(SystemResourceMonitor.SystemResourceStats stats)
        {
            // В низком режиме умеренно оптимизируем
            if (_adaptiveQuality != null)
            {
                _adaptiveQuality.SetQualityLevel(1);
            }
            
            if (_frameRateLimiter != null)
            {
                _frameRateLimiter.SetTargetFrameRate(45);
            }
        }
        
        private void CoordinateMediumMode(SystemResourceMonitor.SystemResourceStats stats)
        {
            // В среднем режиме балансируем
            if (_frameRateLimiter != null)
            {
                _frameRateLimiter.SetTargetFrameRate(60);
            }
        }
        
        private void CoordinateHighMode(SystemResourceMonitor.SystemResourceStats stats)
        {
            // В высоком режиме можем позволить больше
            if (_frameRateLimiter != null)
            {
                _frameRateLimiter.SetTargetFrameRate(90);
            }
        }
        
        private void CheckEmergencyConditions()
        {
            if (_overheatProtection != null)
            {
                var overheatState = _overheatProtection.GetCurrentState();
                if (overheatState == OverheatProtectionSystem.OverheatState.Emergency)
                {
                    if (!_emergencyModeActive)
                    {
                        Debug.LogError("[PerformanceManager] 🚨 ЭКСТРЕННАЯ СИТУАЦИЯ! Активируем максимальную защиту!");
                        ApplyEmergencySettings();
                    }
                }
            }
        }
        
        private void SetConservativeSettings()
        {
            // Устанавливаем консервативные настройки по умолчанию
            Application.targetFrameRate = 45;
            QualitySettings.SetQualityLevel(1);
            QualitySettings.vSyncCount = 1;
            
            Debug.Log("[PerformanceManager] 🛡️ Установлены консервативные настройки для защиты от перегрева");
        }
        
        private void LogSystemStatus()
        {
            Debug.Log("[PerformanceManager] 📊 Статус систем:");
            Debug.Log($"  - Защита от перегрева: {(_overheatProtection != null ? "✅" : "❌")}");
            Debug.Log($"  - Мониторинг ресурсов: {(_resourceMonitor != null ? "✅" : "❌")}");
            Debug.Log($"  - Адаптивное качество: {(_adaptiveQuality != null ? "✅" : "❌")}");
            Debug.Log($"  - Ограничение FPS: {(_frameRateLimiter != null ? "✅" : "❌")}");
            Debug.Log($"  - Оптимизация GPU: {(_gpuOptimizer != null ? "✅" : "❌")}");
            Debug.Log($"  - Оптимизация памяти: {(_memoryOptimizer != null ? "✅" : "❌")}");
            Debug.Log($"  - Оптимизация CPU: {(_cpuOptimizer != null ? "✅" : "❌")}");
            Debug.Log($"  - UI производительности: {(_performanceUI != null ? "✅" : "❌")}");
        }
        
        private void LogPerformanceLevelChange(PerformanceLevel level)
        {
            string levelName = GetPerformanceLevelName(level);
            string icon = GetPerformanceLevelIcon(level);
            Debug.Log($"[PerformanceManager] {icon} Уровень производительности изменен на: {levelName}");
        }
        
        private string GetPerformanceLevelName(PerformanceLevel level)
        {
            switch (level)
            {
                case PerformanceLevel.Emergency: return "Экстренный";
                case PerformanceLevel.Critical: return "Критический";
                case PerformanceLevel.Low: return "Низкий";
                case PerformanceLevel.Medium: return "Средний";
                case PerformanceLevel.High: return "Высокий";
                default: return "Неизвестно";
            }
        }
        
        private string GetPerformanceLevelIcon(PerformanceLevel level)
        {
            switch (level)
            {
                case PerformanceLevel.Emergency: return "🚨";
                case PerformanceLevel.Critical: return "🔥";
                case PerformanceLevel.Low: return "🔶";
                case PerformanceLevel.Medium: return "🔷";
                case PerformanceLevel.High: return "🔵";
                default: return "❓";
            }
        }
        
        /// <summary>
        /// Принудительно активировать экстренный режим
        /// </summary>
        public void ForceEmergencyMode()
        {
            Debug.LogError("[PerformanceManager] 🚨 ПРИНУДИТЕЛЬНАЯ АКТИВАЦИЯ ЭКСТРЕННОГО РЕЖИМА!");
            ApplyEmergencySettings();
        }
        
        /// <summary>
        /// Получить текущий уровень производительности
        /// </summary>
        public PerformanceLevel GetCurrentPerformanceLevel()
        {
            return _currentPerformanceLevel;
        }
        
        /// <summary>
        /// Проверить, активен ли экстренный режим
        /// </summary>
        public bool IsEmergencyModeActive()
        {
            return _emergencyModeActive;
        }
        
        /// <summary>
        /// Получить статус всех систем
        /// </summary>
        public string GetSystemStatus()
        {
            return $"Уровень: {GetPerformanceLevelName(_currentPerformanceLevel)} | " +
                   $"Экстренный режим: {(_emergencyModeActive ? "ДА" : "НЕТ")} | " +
                   $"FPS: {Application.targetFrameRate} | " +
                   $"Качество: {QualitySettings.GetQualityLevel()}";
        }
    }
}