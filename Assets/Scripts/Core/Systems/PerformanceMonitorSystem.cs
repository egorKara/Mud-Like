using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using System.Collections.Generic;

namespace MudLike.Core.Systems
{
    /// <summary>
    /// Система мониторинга производительности в реальном времени
    /// Отслеживает FPS, CPU нагрузку, использование памяти
    /// </summary>
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class PerformanceMonitorSystem : SystemBase
    {
        private float _lastFrameTime;
        private float _fps;
        private float _frameTime;
        private int _frameCount;
        private float _accumulatedTime;
        
        // Настройки мониторинга
        private const float FPS_UPDATE_INTERVAL = 0.5f;
        private const float LOW_FPS_THRESHOLD = 30f;
        private const float HIGH_FPS_THRESHOLD = 60f;
        
        // Статистика производительности
        public float CurrentFPS => _fps;
        public float CurrentFrameTime => _frameTime;
        public bool IsLowPerformance => _fps < LOW_FPS_THRESHOLD;
        public bool IsHighPerformance => _fps > HIGH_FPS_THRESHOLD;
        
        // Настройки адаптивного качества
        public int CurrentQualityLevel { get; private set; } = 2; // 0-3 (Low, Medium, High, Ultra)
        public bool EnableAdaptiveQuality { get; set; } = true;
        
        protected override void OnCreate()
        {
            _lastFrameTime = Time.realtimeSinceStartup;
        }
        
        protected override void OnUpdate()
        {
            float currentTime = Time.realtimeSinceStartup;
            float deltaTime = currentTime - _lastFrameTime;
            _lastFrameTime = currentTime;
            
            _frameCount++;
            _accumulatedTime += deltaTime;
            
            // Обновляем FPS каждые FPS_UPDATE_INTERVAL секунд
            if (_accumulatedTime >= FPS_UPDATE_INTERVAL)
            {
                _fps = _frameCount / _accumulatedTime;
                _frameTime = _accumulatedTime / _frameCount * 1000f; // в миллисекундах
                
                _frameCount = 0;
                _accumulatedTime = 0f;
                
                // Адаптивное качество
                if (EnableAdaptiveQuality)
                {
                    UpdateAdaptiveQuality();
                }
                
                // Логирование производительности
                LogPerformanceStats();
            }
        }
        
        private void UpdateAdaptiveQuality()
        {
            int newQualityLevel = CurrentQualityLevel;
            
            if (IsLowPerformance && CurrentQualityLevel > 0)
            {
                newQualityLevel = CurrentQualityLevel - 1;
                Debug.Log($"[Performance] Снижение качества: {CurrentQualityLevel} -> {newQualityLevel} (FPS: {_fps:F1})");
            }
            else if (IsHighPerformance && CurrentQualityLevel < 3)
            {
                newQualityLevel = CurrentQualityLevel + 1;
                Debug.Log($"[Performance] Повышение качества: {CurrentQualityLevel} -> {newQualityLevel} (FPS: {_fps:F1})");
            }
            
            if (newQualityLevel != CurrentQualityLevel)
            {
                CurrentQualityLevel = newQualityLevel;
                ApplyQualitySettings();
            }
        }
        
        private void ApplyQualitySettings()
        {
            // Применяем настройки качества в зависимости от уровня
            switch (CurrentQualityLevel)
            {
                case 0: // Low
                    QualitySettings.SetQualityLevel(0);
                    Application.targetFrameRate = 30;
                    break;
                case 1: // Medium
                    QualitySettings.SetQualityLevel(1);
                    Application.targetFrameRate = 45;
                    break;
                case 2: // High
                    QualitySettings.SetQualityLevel(2);
                    Application.targetFrameRate = 60;
                    break;
                case 3: // Ultra
                    QualitySettings.SetQualityLevel(3);
                    Application.targetFrameRate = 120;
                    break;
            }
        }
        
        private void LogPerformanceStats()
        {
            if (_frameCount % 10 == 0) // Логируем каждые 5 секунд
            {
                long memoryUsage = System.GC.GetTotalMemory(false) / (1024 * 1024); // MB
                
                Debug.Log($"[Performance] FPS: {_fps:F1}, FrameTime: {_frameTime:F1}ms, " +
                         $"Memory: {memoryUsage}MB, Quality: {CurrentQualityLevel}");
            }
        }
        
        /// <summary>
        /// Принудительно установить уровень качества
        /// </summary>
        public void SetQualityLevel(int level)
        {
            CurrentQualityLevel = math.clamp(level, 0, 3);
            ApplyQualitySettings();
        }
        
        /// <summary>
        /// Включить/выключить адаптивное качество
        /// </summary>
        public void SetAdaptiveQuality(bool enabled)
        {
            EnableAdaptiveQuality = enabled;
        }
    }
}