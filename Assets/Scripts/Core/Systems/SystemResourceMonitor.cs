using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace MudLike.Core.Systems
{
    /// <summary>
    /// Система мониторинга ресурсов системы
    /// Отслеживает CPU, RAM, GPU нагрузку для предотвращения перегрева
    /// </summary>
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class SystemResourceMonitor : SystemBase
    {
        // Пороги нагрузки
        private const float HIGH_CPU_USAGE = 80f;      // Высокая нагрузка CPU
        private const float HIGH_RAM_USAGE = 85f;      // Высокая нагрузка RAM
        private const float HIGH_GPU_USAGE = 90f;      // Высокая нагрузка GPU
        
        // Интервалы проверки
        private const float RESOURCE_CHECK_INTERVAL = 0.5f;  // Проверяем каждые 500ms
        private const float DETAILED_CHECK_INTERVAL = 2f;    // Детальная проверка каждые 2 секунды
        
        // Состояние системы
        private float _lastResourceCheck;
        private float _lastDetailedCheck;
        private SystemResourceStats _currentStats;
        private List<float> _cpuHistory = new List<float>();
        private List<float> _ramHistory = new List<float>();
        private const int HISTORY_SIZE = 10;
        
        // Процесс Unity
        private Process _unityProcess;
        private PerformanceCounter _cpuCounter;
        private PerformanceCounter _ramCounter;
        
        public struct SystemResourceStats
        {
            public float CPUUsage;
            public float RAMUsage;
            public float GPUUsage;
            public long TotalRAM;
            public long UsedRAM;
            public float Temperature;
            public bool IsOverloaded;
            public ResourceLoadLevel LoadLevel;
        }
        
        public enum ResourceLoadLevel
        {
            Low,        // < 50%
            Medium,     // 50-70%
            High,       // 70-85%
            Critical,   // 85-95%
            Overloaded  // > 95%
        }
        
        protected override void OnCreate()
        {
            InitializeMonitoring();
            Debug.Log("[SystemResourceMonitor] Мониторинг ресурсов системы активирован");
        }
        
        protected override void OnDestroy()
        {
            CleanupMonitoring();
        }
        
        protected override void OnUpdate()
        {
            float currentTime = Time.time;
            
            // Быстрая проверка ресурсов
            if (currentTime - _lastResourceCheck >= RESOURCE_CHECK_INTERVAL)
            {
                UpdateResourceStats();
                _lastResourceCheck = currentTime;
            }
            
            // Детальная проверка
            if (currentTime - _lastDetailedCheck >= DETAILED_CHECK_INTERVAL)
            {
                PerformDetailedCheck();
                _lastDetailedCheck = currentTime;
            }
            
            // Проверяем перегрузку
            CheckForOverload();
        }
        
        private void InitializeMonitoring()
        {
            try
            {
                // Получаем процесс Unity
                _unityProcess = Process.GetCurrentProcess();
                
                // Инициализируем счетчики производительности (Windows)
                if (Application.platform == RuntimePlatform.WindowsEditor || 
                    Application.platform == RuntimePlatform.WindowsPlayer)
                {
                    _cpuCounter = new PerformanceCounter("Process", "% Processor Time", _unityProcess.ProcessName);
                    _ramCounter = new PerformanceCounter("Process", "Working Set", _unityProcess.ProcessName);
                }
                
                // Инициализируем статистику
                _currentStats = new SystemResourceStats();
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"[SystemResourceMonitor] Ошибка инициализации мониторинга: {e.Message}");
            }
        }
        
        private void CleanupMonitoring()
        {
            _cpuCounter?.Dispose();
            _ramCounter?.Dispose();
        }
        
        private void UpdateResourceStats()
        {
            try
            {
                // CPU нагрузка
                float cpuUsage = GetCPUUsage();
                _currentStats.CPUUsage = cpuUsage;
                AddToHistory(_cpuHistory, cpuUsage);
                
                // RAM использование
                long ramUsage = GetRAMUsage();
                _currentStats.UsedRAM = ramUsage;
                _currentStats.TotalRAM = SystemInfo.systemMemorySize * 1024 * 1024; // MB to bytes
                _currentStats.RAMUsage = (float)ramUsage / _currentStats.TotalRAM * 100f;
                AddToHistory(_ramHistory, _currentStats.RAMUsage);
                
                // GPU нагрузка (упрощенная)
                _currentStats.GPUUsage = GetGPUUsage();
                
                // Температура (если доступна)
                _currentStats.Temperature = GetSystemTemperature();
                
                // Определяем уровень нагрузки
                _currentStats.LoadLevel = DetermineLoadLevel();
                _currentStats.IsOverloaded = _currentStats.LoadLevel >= ResourceLoadLevel.Critical;
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"[SystemResourceMonitor] Ошибка обновления статистики: {e.Message}");
            }
        }
        
        private void PerformDetailedCheck()
        {
            // Логируем детальную статистику
            LogDetailedStats();
            
            // Проверяем тренды
            CheckResourceTrends();
            
            // Рекомендуем действия
            RecommendActions();
        }
        
        private void CheckForOverload()
        {
            if (_currentStats.IsOverloaded)
            {
                Debug.LogError($"[SystemResourceMonitor] 🚨 ПЕРЕГРУЗКА СИСТЕМЫ! " +
                              $"CPU: {_currentStats.CPUUsage:F1}% | " +
                              $"RAM: {_currentStats.RAMUsage:F1}% | " +
                              $"GPU: {_currentStats.GPUUsage:F1}%");
                
                // Активируем экстренные меры
                ActivateEmergencyMeasures();
            }
        }
        
        private float GetCPUUsage()
        {
            try
            {
                if (_cpuCounter != null)
                {
                    return _cpuCounter.NextValue();
                }
                
                // Альтернативный метод через Process
                return _unityProcess.TotalProcessorTime.TotalMilliseconds / 
                       (System.Environment.TickCount - _unityProcess.StartTime.Ticks) * 100f;
            }
            catch
            {
                return 0f;
            }
        }
        
        private long GetRAMUsage()
        {
            try
            {
                if (_ramCounter != null)
                {
                    return (long)_ramCounter.NextValue();
                }
                
                // Альтернативный метод
                return _unityProcess.WorkingSet64;
            }
            catch
            {
                return 0;
            }
        }
        
        private float GetGPUUsage()
        {
            // Упрощенная оценка GPU нагрузки
            // В реальном проекте можно использовать GPU profiling API
            return QualitySettings.GetQualityLevel() * 25f; // Примерная оценка
        }
        
        private float GetSystemTemperature()
        {
            // Упрощенная реализация
            // В реальном проекте можно использовать системные API
            return 50f + (_currentStats.CPUUsage * 0.3f); // Примерная оценка
        }
        
        private ResourceLoadLevel DetermineLoadLevel()
        {
            float maxUsage = math.max(_currentStats.CPUUsage, _currentStats.RAMUsage);
            maxUsage = math.max(maxUsage, _currentStats.GPUUsage);
            
            if (maxUsage >= 95f) return ResourceLoadLevel.Overloaded;
            if (maxUsage >= 85f) return ResourceLoadLevel.Critical;
            if (maxUsage >= 70f) return ResourceLoadLevel.High;
            if (maxUsage >= 50f) return ResourceLoadLevel.Medium;
            return ResourceLoadLevel.Low;
        }
        
        private void AddToHistory(List<float> history, float value)
        {
            history.Add(value);
            if (history.Count > HISTORY_SIZE)
            {
                history.RemoveAt(0);
            }
        }
        
        private void CheckResourceTrends()
        {
            // Проверяем тренды CPU
            if (_cpuHistory.Count >= 3)
            {
                float cpuTrend = CalculateTrend(_cpuHistory);
                if (cpuTrend > 10f) // Растущая нагрузка
                {
                    Debug.LogWarning($"[SystemResourceMonitor] ⚠️ Растущая нагрузка CPU: +{cpuTrend:F1}%");
                }
            }
            
            // Проверяем тренды RAM
            if (_ramHistory.Count >= 3)
            {
                float ramTrend = CalculateTrend(_ramHistory);
                if (ramTrend > 5f) // Растущее использование RAM
                {
                    Debug.LogWarning($"[SystemResourceMonitor] ⚠️ Растущее использование RAM: +{ramTrend:F1}%");
                }
            }
        }
        
        private float CalculateTrend(List<float> history)
        {
            if (history.Count < 2) return 0f;
            
            float sum = 0f;
            for (int i = 1; i < history.Count; i++)
            {
                sum += history[i] - history[i - 1];
            }
            return sum / (history.Count - 1);
        }
        
        private void RecommendActions()
        {
            if (_currentStats.LoadLevel >= ResourceLoadLevel.High)
            {
                string recommendations = GetRecommendations();
                Debug.Log($"[SystemResourceMonitor] 💡 Рекомендации: {recommendations}");
            }
        }
        
        private string GetRecommendations()
        {
            List<string> recommendations = new List<string>();
            
            if (_currentStats.CPUUsage > HIGH_CPU_USAGE)
            {
                recommendations.Add("Снизить качество графики");
                recommendations.Add("Уменьшить FPS");
            }
            
            if (_currentStats.RAMUsage > HIGH_RAM_USAGE)
            {
                recommendations.Add("Очистить кэш");
                recommendations.Add("Закрыть другие приложения");
            }
            
            if (_currentStats.GPUUsage > HIGH_GPU_USAGE)
            {
                recommendations.Add("Отключить VSync");
                recommendations.Add("Снизить разрешение");
            }
            
            return string.Join(", ", recommendations);
        }
        
        private void ActivateEmergencyMeasures()
        {
            // Активируем систему защиты от перегрева
            var overheatProtection = World.GetExistingSystemManaged<OverheatProtectionSystem>();
            if (overheatProtection != null)
            {
                overheatProtection.ForceEmergencyMode();
            }
            
            // Дополнительные экстренные меры
            Application.targetFrameRate = 15;
            QualitySettings.SetQualityLevel(0);
            QualitySettings.vSyncCount = 0;
            
            // Принудительная сборка мусора
            System.GC.Collect();
        }
        
        private void LogDetailedStats()
        {
            string statusIcon = GetLoadLevelIcon(_currentStats.LoadLevel);
            Debug.Log($"[SystemResourceMonitor] {statusIcon} " +
                     $"CPU: {_currentStats.CPUUsage:F1}% | " +
                     $"RAM: {_currentStats.RAMUsage:F1}% ({_currentStats.UsedRAM / (1024 * 1024)}MB) | " +
                     $"GPU: {_currentStats.GPUUsage:F1}% | " +
                     $"Temp: {_currentStats.Temperature:F1}°C | " +
                     $"Level: {_currentStats.LoadLevel}");
        }
        
        private string GetLoadLevelIcon(ResourceLoadLevel level)
        {
            switch (level)
            {
                case ResourceLoadLevel.Low: return "🟢";
                case ResourceLoadLevel.Medium: return "🟡";
                case ResourceLoadLevel.High: return "🟠";
                case ResourceLoadLevel.Critical: return "🔴";
                case ResourceLoadLevel.Overloaded: return "🚨";
                default: return "❓";
            }
        }
        
        /// <summary>
        /// Получить текущую статистику ресурсов
        /// </summary>
        public SystemResourceStats GetCurrentStats()
        {
            return _currentStats;
        }
        
        /// <summary>
        /// Проверить, перегружена ли система
        /// </summary>
        public bool IsSystemOverloaded()
        {
            return _currentStats.IsOverloaded;
        }
        
        /// <summary>
        /// Получить уровень нагрузки
        /// </summary>
        public ResourceLoadLevel GetLoadLevel()
        {
            return _currentStats.LoadLevel;
        }
    }
}