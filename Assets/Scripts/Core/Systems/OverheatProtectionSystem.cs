using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace MudLike.Core.Systems
{
    /// <summary>
    /// СИСТЕМА ЗАЩИТЫ ОТ ПЕРЕГРЕВА НОУТБУКА
    /// Критически важная система для предотвращения перегрева IDE Cursor
    /// </summary>
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class OverheatProtectionSystem : SystemBase
    {
        // Критические пороги температуры
        private const float CRITICAL_TEMP_THRESHOLD = 85f; // Критическая температура CPU
        private const float WARNING_TEMP_THRESHOLD = 75f;  // Предупреждение о перегреве
        private const float SAFE_TEMP_THRESHOLD = 65f;     // Безопасная температура
        
        // Настройки защиты
        private const float TEMP_CHECK_INTERVAL = 1f;      // Проверяем каждую секунду
        private const float EMERGENCY_COOLDOWN_TIME = 5f;  // Экстренное охлаждение 5 секунд
        private const int MAX_FPS_EMERGENCY = 15;          // Максимальный FPS в экстренном режиме
        private const int MAX_FPS_WARNING = 30;            // Максимальный FPS при предупреждении
        
        // Состояние системы
        private float _lastTempCheck;
        private float _currentCPUTemp;
        private OverheatState _currentState;
        private float _emergencyStartTime;
        private bool _emergencyModeActive;
        
        // Системные настройки
        private int _originalTargetFPS;
        private int _originalQualityLevel;
        private bool _originalVSync;
        
        public enum OverheatState
        {
            Safe,           // Безопасно
            Warning,        // Предупреждение
            Critical,       // Критично
            Emergency       // Экстренный режим
        }
        
        protected override void OnCreate()
        {
            // Сохраняем оригинальные настройки
            _originalTargetFPS = Application.targetFrameRate;
            _originalQualityLevel = QualitySettings.GetQualityLevel();
            _originalVSync = QualitySettings.vSyncCount > 0;
            
            // Устанавливаем консервативные настройки по умолчанию
            SetConservativeSettings();
            
            Debug.Log("[OverheatProtection] Система защиты от перегрева активирована!");
            Debug.Log($"[OverheatProtection] Оригинальные настройки: FPS={_originalTargetFPS}, Quality={_originalQualityLevel}, VSync={_originalVSync}");
        }
        
        protected override void OnUpdate()
        {
            float currentTime = Time.time;
            
            // Проверяем температуру каждую секунду
            if (currentTime - _lastTempCheck >= TEMP_CHECK_INTERVAL)
            {
                CheckSystemTemperature();
                UpdateProtectionMeasures();
                _lastTempCheck = currentTime;
            }
            
            // В экстренном режиме дополнительно ограничиваем нагрузку
            if (_emergencyModeActive)
            {
                EnforceEmergencyLimits();
            }
        }
        
        private void CheckSystemTemperature()
        {
            _currentCPUTemp = GetCPUTemperature();
            
            // Определяем состояние системы
            if (_currentCPUTemp >= CRITICAL_TEMP_THRESHOLD)
            {
                if (_currentState != OverheatState.Emergency)
                {
                    _currentState = OverheatState.Emergency;
                    _emergencyModeActive = true;
                    _emergencyStartTime = Time.time;
                    ActivateEmergencyMode();
                }
            }
            else if (_currentCPUTemp >= WARNING_TEMP_THRESHOLD)
            {
                if (_currentState != OverheatState.Critical)
                {
                    _currentState = OverheatState.Critical;
                    ActivateCriticalMode();
                }
            }
            else if (_currentCPUTemp >= SAFE_TEMP_THRESHOLD)
            {
                if (_currentState != OverheatState.Warning)
                {
                    _currentState = OverheatState.Warning;
                    ActivateWarningMode();
                }
            }
            else
            {
                if (_currentState != OverheatState.Safe)
                {
                    _currentState = OverheatState.Safe;
                    _emergencyModeActive = false;
                    RestoreNormalMode();
                }
            }
            
            // Логируем состояние каждые 5 секунд
            if (Time.time % 5f < 1f)
            {
                LogTemperatureStatus();
            }
        }
        
        private void UpdateProtectionMeasures()
        {
            switch (_currentState)
            {
                case OverheatState.Safe:
                    // Восстанавливаем нормальные настройки
                    RestoreNormalMode();
                    break;
                    
                case OverheatState.Warning:
                    // Снижаем качество и FPS
                    SetQualityLevel(1);
                    SetTargetFPS(45);
                    break;
                    
                case OverheatState.Critical:
                    // Значительно снижаем нагрузку
                    SetQualityLevel(0);
                    SetTargetFPS(30);
                    DisableVSync();
                    break;
                    
                case OverheatState.Emergency:
                    // Экстренные меры
                    SetQualityLevel(0);
                    SetTargetFPS(MAX_FPS_EMERGENCY);
                    DisableVSync();
                    ForceGarbageCollection();
                    break;
            }
        }
        
        private void ActivateEmergencyMode()
        {
            Debug.LogError($"[OverheatProtection] 🚨 ЭКСТРЕННЫЙ РЕЖИМ! Температура CPU: {_currentCPUTemp:F1}°C");
            Debug.LogError("[OverheatProtection] Применяются экстренные меры охлаждения!");
            
            // Минимальные настройки
            SetQualityLevel(0);
            SetTargetFPS(MAX_FPS_EMERGENCY);
            DisableVSync();
            
            // Принудительная сборка мусора
            ForceGarbageCollection();
            
            // Останавливаем все несущественные системы
            PauseNonEssentialSystems();
        }
        
        private void ActivateCriticalMode()
        {
            Debug.LogWarning($"[OverheatProtection] ⚠️ КРИТИЧЕСКАЯ ТЕМПЕРАТУРА! CPU: {_currentCPUTemp:F1}°C");
            
            SetQualityLevel(0);
            SetTargetFPS(MAX_FPS_WARNING);
            DisableVSync();
        }
        
        private void ActivateWarningMode()
        {
            Debug.Log($"[OverheatProtection] ⚡ Предупреждение о перегреве. CPU: {_currentCPUTemp:F1}°C");
            
            SetQualityLevel(1);
            SetTargetFPS(45);
        }
        
        private void RestoreNormalMode()
        {
            if (_currentCPUTemp < SAFE_TEMP_THRESHOLD)
            {
                Debug.Log($"[OverheatProtection] ✅ Температура нормализовалась. CPU: {_currentCPUTemp:F1}°C");
                
                // Постепенно восстанавливаем настройки
                SetQualityLevel(_originalQualityLevel);
                SetTargetFPS(_originalTargetFPS);
                if (_originalVSync) EnableVSync();
            }
        }
        
        private void EnforceEmergencyLimits()
        {
            // В экстренном режиме дополнительно ограничиваем нагрузку
            if (Time.time - _emergencyStartTime > EMERGENCY_COOLDOWN_TIME)
            {
                // Проверяем, можно ли выйти из экстренного режима
                if (_currentCPUTemp < WARNING_TEMP_THRESHOLD)
                {
                    _emergencyModeActive = false;
                    Debug.Log("[OverheatProtection] Выход из экстренного режима");
                }
            }
            
            // Принудительно ограничиваем FPS
            Application.targetFrameRate = MAX_FPS_EMERGENCY;
            
            // Минимальное качество
            QualitySettings.SetQualityLevel(0);
        }
        
        private void SetConservativeSettings()
        {
            // Устанавливаем консервативные настройки по умолчанию
            SetQualityLevel(1);
            SetTargetFPS(45);
            EnableVSync();
        }
        
        private int _lastAppliedQualityLevel = -1;
        private float _lastQualityChangeTime;
        private const float MIN_QUALITY_CHANGE_INTERVAL = 1f; // Минимум 1 секунда между изменениями
        
        private void SetQualityLevel(int level)
        {
            level = math.clamp(level, 0, 3);
            
            // Защита от частых изменений качества
            float currentTime = Time.time;
            if (level == _lastAppliedQualityLevel || 
                (currentTime - _lastQualityChangeTime < MIN_QUALITY_CHANGE_INTERVAL))
            {
                return; // Пропускаем изменение если оно слишком частое или то же самое
            }
            
            try
            {
                QualitySettings.SetQualityLevel(level);
                _lastAppliedQualityLevel = level;
                _lastQualityChangeTime = currentTime;
                
                // Дополнительные настройки для снижения нагрузки
                ApplyQualitySettings(level);
                
                Debug.Log($"[OverheatProtection] Качество изменено на уровень: {level}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[OverheatProtection] Ошибка изменения качества: {e.Message}");
            }
        }
        
        private void ApplyQualitySettings(int level)
        {
            try
            {
                switch (level)
                {
                    case 0: // Минимальное качество
                        QualitySettings.pixelLightCount = 1;
                        QualitySettings.shadowResolution = ShadowResolution.Low;
                        QualitySettings.shadowDistance = 20f;
                        QualitySettings.lodBias = 2f;
                        QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
                        QualitySettings.antiAliasing = 0;
                        break;
                    case 1: // Низкое качество
                        QualitySettings.pixelLightCount = 2;
                        QualitySettings.shadowResolution = ShadowResolution.Low;
                        QualitySettings.shadowDistance = 50f;
                        QualitySettings.lodBias = 1.5f;
                        QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
                        QualitySettings.antiAliasing = 0;
                        break;
                    case 2: // Среднее качество
                        QualitySettings.pixelLightCount = 4;
                        QualitySettings.shadowResolution = ShadowResolution.Medium;
                        QualitySettings.shadowDistance = 100f;
                        QualitySettings.lodBias = 1f;
                        QualitySettings.anisotropicFiltering = AnisotropicFiltering.Enable;
                        QualitySettings.antiAliasing = 2;
                        break;
                    case 3: // Высокое качество
                        QualitySettings.pixelLightCount = 8;
                        QualitySettings.shadowResolution = ShadowResolution.High;
                        QualitySettings.shadowDistance = 150f;
                        QualitySettings.lodBias = 1f;
                        QualitySettings.anisotropicFiltering = AnisotropicFiltering.Enable;
                        QualitySettings.antiAliasing = 4;
                        break;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[OverheatProtection] Ошибка применения настроек качества: {e.Message}");
            }
        }
        
        private int _lastAppliedFPS = -1;
        private float _lastFPSChangeTime;
        private const float MIN_FPS_CHANGE_INTERVAL = 1f; // Минимум 1 секунда между изменениями FPS
        
        private void SetTargetFPS(int fps)
        {
            // Защита от частых изменений FPS
            float currentTime = Time.time;
            if (fps == _lastAppliedFPS || 
                (currentTime - _lastFPSChangeTime < MIN_FPS_CHANGE_INTERVAL))
            {
                return; // Пропускаем изменение если оно слишком частое или то же самое
            }
            
            try
            {
                Application.targetFrameRate = fps;
                _lastAppliedFPS = fps;
                _lastFPSChangeTime = currentTime;
                
                Debug.Log($"[OverheatProtection] FPS установлен: {fps}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[OverheatProtection] Ошибка установки FPS: {e.Message}");
            }
        }
        
        private int _lastAppliedVSync = -1;
        private float _lastVSyncChangeTime;
        private const float MIN_VSYNC_CHANGE_INTERVAL = 1f; // Минимум 1 секунда между изменениями VSync
        
        private void EnableVSync()
        {
            // Защита от частых изменений VSync
            float currentTime = Time.time;
            if (1 == _lastAppliedVSync || 
                (currentTime - _lastVSyncChangeTime < MIN_VSYNC_CHANGE_INTERVAL))
            {
                return; // Пропускаем изменение если оно слишком частое или то же самое
            }
            
            try
            {
                QualitySettings.vSyncCount = 1;
                _lastAppliedVSync = 1;
                _lastVSyncChangeTime = currentTime;
                
                Debug.Log("[OverheatProtection] VSync включен");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[OverheatProtection] Ошибка включения VSync: {e.Message}");
            }
        }
        
        private void DisableVSync()
        {
            // Защита от частых изменений VSync
            float currentTime = Time.time;
            if (0 == _lastAppliedVSync || 
                (currentTime - _lastVSyncChangeTime < MIN_VSYNC_CHANGE_INTERVAL))
            {
                return; // Пропускаем изменение если оно слишком частое или то же самое
            }
            
            try
            {
                QualitySettings.vSyncCount = 0;
                _lastAppliedVSync = 0;
                _lastVSyncChangeTime = currentTime;
                
                Debug.Log("[OverheatProtection] VSync отключен");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[OverheatProtection] Ошибка отключения VSync: {e.Message}");
            }
        }
        
        private void ForceGarbageCollection()
        {
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
            System.GC.Collect();
        }
        
        private void PauseNonEssentialSystems()
        {
            // Здесь можно приостановить несущественные системы
            // Например, системы частиц, аудио, эффекты
        }
        
        private void LogTemperatureStatus()
        {
            string statusIcon = GetStatusIcon(_currentState);
            Debug.Log($"[OverheatProtection] {statusIcon} CPU: {_currentCPUTemp:F1}°C | " +
                     $"FPS: {Application.targetFrameRate} | Quality: {QualitySettings.GetQualityLevel()} | " +
                     $"State: {_currentState}");
        }
        
        private string GetStatusIcon(OverheatState state)
        {
            switch (state)
            {
                case OverheatState.Safe: return "✅";
                case OverheatState.Warning: return "⚠️";
                case OverheatState.Critical: return "🔥";
                case OverheatState.Emergency: return "🚨";
                default: return "❓";
            }
        }
        
        private float GetCPUTemperature()
        {
            // Используем РЕАЛЬНЫЙ датчик температуры
            try
            {
                float realTemp = RealTemperatureSensor.GetRealCPUTemperature();
                
                // Проверяем валидность температуры
                if (realTemp > 0f && realTemp < 200f)
                {
                    Debug.Log($"[OverheatProtection] Реальная температура CPU: {realTemp:F1}°C");
                    return realTemp;
                }
                else
                {
                    Debug.LogWarning($"[OverheatProtection] Некорректная температура: {realTemp:F1}°C, используем оценку");
                    return EstimateTemperatureFromSystemLoad();
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[OverheatProtection] Критическая ошибка получения температуры: {e.Message}");
                return EstimateTemperatureFromSystemLoad();
            }
        }
        
        /// <summary>
        /// Оценить температуру на основе нагрузки системы
        /// </summary>
        private float EstimateTemperatureFromSystemLoad()
        {
            try
            {
                // Получаем нагрузку CPU через Process
                using (Process process = Process.GetCurrentProcess())
                {
                    long totalTime = process.TotalProcessorTime.TotalMilliseconds;
                    long elapsedTime = System.Environment.TickCount - process.StartTime.Ticks;
                    
                    if (elapsedTime > 0)
                    {
                        float cpuLoad = (float)totalTime / elapsedTime * 100f;
                        cpuLoad = math.clamp(cpuLoad, 0f, 100f);
                        
                        // Оцениваем температуру: базовая + влияние нагрузки
                        float baseTemp = 45f; // Базовая температура
                        float loadFactor = cpuLoad / 100f;
                        float estimatedTemp = baseTemp + (loadFactor * 25f); // До +25°C при 100% нагрузке
                        
                        Debug.Log($"[OverheatProtection] Оценочная температура: {estimatedTemp:F1}°C (нагрузка CPU: {cpuLoad:F1}%)");
                        return estimatedTemp;
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"[OverheatProtection] Ошибка оценки температуры: {e.Message}");
            }
            
            // Безопасное значение по умолчанию
            return 50f;
        }
        
        /// <summary>
        /// Принудительно активировать экстренный режим
        /// </summary>
        public void ForceEmergencyMode()
        {
            _currentState = OverheatState.Emergency;
            _emergencyModeActive = true;
            _emergencyStartTime = Time.time;
            ActivateEmergencyMode();
        }
        
        /// <summary>
        /// Получить текущее состояние системы
        /// </summary>
        public OverheatState GetCurrentState()
        {
            return _currentState;
        }
        
        /// <summary>
        /// Получить текущую температуру CPU
        /// </summary>
        public float GetCurrentTemperature()
        {
            return _currentCPUTemp;
        }
    }
}