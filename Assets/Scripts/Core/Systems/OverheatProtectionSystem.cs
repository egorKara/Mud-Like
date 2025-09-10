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
        
        private void SetQualityLevel(int level)
        {
            level = math.clamp(level, 0, 3);
            QualitySettings.SetQualityLevel(level);
            
            // Дополнительные настройки для снижения нагрузки
            switch (level)
            {
                case 0: // Минимальное качество
                    QualitySettings.pixelLightCount = 1;
                    QualitySettings.shadowResolution = ShadowResolution.Low;
                    QualitySettings.shadowDistance = 20f;
                    QualitySettings.lodBias = 2f;
                    break;
                case 1: // Низкое качество
                    QualitySettings.pixelLightCount = 2;
                    QualitySettings.shadowResolution = ShadowResolution.Low;
                    QualitySettings.shadowDistance = 50f;
                    QualitySettings.lodBias = 1.5f;
                    break;
            }
        }
        
        private void SetTargetFPS(int fps)
        {
            Application.targetFrameRate = fps;
        }
        
        private void EnableVSync()
        {
            QualitySettings.vSyncCount = 1;
        }
        
        private void DisableVSync()
        {
            QualitySettings.vSyncCount = 0;
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
            // Попытка получить температуру CPU (работает не на всех системах)
            try
            {
                if (Application.platform == RuntimePlatform.WindowsEditor || 
                    Application.platform == RuntimePlatform.WindowsPlayer)
                {
                    return GetWindowsCPUTemperature();
                }
                else if (Application.platform == RuntimePlatform.LinuxEditor || 
                         Application.platform == RuntimePlatform.LinuxPlayer)
                {
                    return GetLinuxCPUTemperature();
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"[OverheatProtection] Не удалось получить температуру CPU: {e.Message}");
            }
            
            // Возвращаем безопасное значение по умолчанию
            return 50f;
        }
        
        private float GetWindowsCPUTemperature()
        {
            // Упрощенная реализация для Windows
            // В реальном проекте можно использовать WMI или другие API
            return 60f; // Заглушка
        }
        
        private float GetLinuxCPUTemperature()
        {
            // Упрощенная реализация для Linux
            // В реальном проекте можно читать /sys/class/thermal/
            return 55f; // Заглушка
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