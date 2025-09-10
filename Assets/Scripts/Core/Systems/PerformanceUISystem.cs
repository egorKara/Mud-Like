using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MudLike.Core.Systems
{
    /// <summary>
    /// Система UI для отображения производительности в реальном времени
    /// Показывает FPS, использование памяти, CPU/GPU нагрузку
    /// </summary>
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public partial class PerformanceUISystem : SystemBase
    {
        private PerformanceMonitorSystem _performanceMonitor;
        private FrameRateLimiterSystem _frameRateLimiter;
        private MemoryOptimizationSystem _memoryOptimizer;
        
        // UI элементы
        private GameObject _performanceUI;
        private TextMeshProUGUI _fpsText;
        private TextMeshProUGUI _memoryText;
        private TextMeshProUGUI _qualityText;
        private Slider _fpsSlider;
        private Toggle _adaptiveToggle;
        private Toggle _vsyncToggle;
        
        // Настройки UI
        private const float UI_UPDATE_INTERVAL = 0.1f; // Обновляем UI каждые 100ms
        private float _lastUIUpdate;
        private bool _uiEnabled = true;
        
        protected override void OnCreate()
        {
            // Получаем ссылки на другие системы
            _performanceMonitor = World.GetExistingSystemManaged<PerformanceMonitorSystem>();
            _frameRateLimiter = World.GetExistingSystemManaged<FrameRateLimiterSystem>();
            _memoryOptimizer = World.GetExistingSystemManaged<MemoryOptimizationSystem>();
            
            // Создаем UI
            CreatePerformanceUI();
        }
        
        protected override void OnUpdate()
        {
            if (!_uiEnabled || _performanceUI == null) return;
            
            float currentTime = Time.time;
            
            // Обновляем UI периодически
            if (currentTime - _lastUIUpdate >= UI_UPDATE_INTERVAL)
            {
                UpdatePerformanceUI();
                _lastUIUpdate = currentTime;
            }
        }
        
        private void CreatePerformanceUI()
        {
            // Создаем Canvas для UI
            var canvasGO = new GameObject("PerformanceCanvas");
            var canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 1000;
            
            var canvasScaler = canvasGO.AddComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(1920, 1080);
            
            var graphicRaycaster = canvasGO.AddComponent<GraphicRaycaster>();
            
            // Создаем панель производительности
            _performanceUI = new GameObject("PerformancePanel");
            _performanceUI.transform.SetParent(canvasGO.transform, false);
            
            var panelRect = _performanceUI.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0, 1);
            panelRect.anchorMax = new Vector2(0, 1);
            panelRect.pivot = new Vector2(0, 1);
            panelRect.anchoredPosition = new Vector2(10, -10);
            panelRect.sizeDelta = new Vector2(300, 200);
            
            var panelImage = _performanceUI.AddComponent<Image>();
            panelImage.color = new Color(0, 0, 0, 0.7f);
            
            // Создаем текстовые элементы
            CreateTextElements();
            CreateControlElements();
            
            // Создаем кнопку переключения
            CreateToggleButton();
        }
        
        private void CreateTextElements()
        {
            // FPS текст
            var fpsGO = new GameObject("FPSText");
            fpsGO.transform.SetParent(_performanceUI.transform, false);
            
            var fpsRect = fpsGO.AddComponent<RectTransform>();
            fpsRect.anchorMin = new Vector2(0, 1);
            fpsRect.anchorMax = new Vector2(1, 1);
            fpsRect.pivot = new Vector2(0, 1);
            fpsRect.anchoredPosition = new Vector2(10, -10);
            fpsRect.sizeDelta = new Vector2(-20, 30);
            
            _fpsText = fpsGO.AddComponent<TextMeshProUGUI>();
            _fpsText.text = "FPS: --";
            _fpsText.fontSize = 16;
            _fpsText.color = Color.white;
            _fpsText.alignment = TextAlignmentOptions.Left;
            
            // Memory текст
            var memoryGO = new GameObject("MemoryText");
            memoryGO.transform.SetParent(_performanceUI.transform, false);
            
            var memoryRect = memoryGO.AddComponent<RectTransform>();
            memoryRect.anchorMin = new Vector2(0, 1);
            memoryRect.anchorMax = new Vector2(1, 1);
            memoryRect.pivot = new Vector2(0, 1);
            memoryRect.anchoredPosition = new Vector2(10, -40);
            memoryRect.sizeDelta = new Vector2(-20, 30);
            
            _memoryText = memoryGO.AddComponent<TextMeshProUGUI>();
            _memoryText.text = "Memory: --";
            _memoryText.fontSize = 16;
            _memoryText.color = Color.white;
            _memoryText.alignment = TextAlignmentOptions.Left;
            
            // Quality текст
            var qualityGO = new GameObject("QualityText");
            qualityGO.transform.SetParent(_performanceUI.transform, false);
            
            var qualityRect = qualityGO.AddComponent<RectTransform>();
            qualityRect.anchorMin = new Vector2(0, 1);
            qualityRect.anchorMax = new Vector2(1, 1);
            qualityRect.pivot = new Vector2(0, 1);
            qualityRect.anchoredPosition = new Vector2(10, -70);
            qualityRect.sizeDelta = new Vector2(-20, 30);
            
            _qualityText = qualityGO.AddComponent<TextMeshProUGUI>();
            _qualityText.text = "Quality: --";
            _qualityText.fontSize = 16;
            _qualityText.color = Color.white;
            _qualityText.alignment = TextAlignmentOptions.Left;
        }
        
        private void CreateControlElements()
        {
            // FPS Slider
            var sliderGO = new GameObject("FPSSlider");
            sliderGO.transform.SetParent(_performanceUI.transform, false);
            
            var sliderRect = sliderGO.AddComponent<RectTransform>();
            sliderRect.anchorMin = new Vector2(0, 1);
            sliderRect.anchorMax = new Vector2(1, 1);
            sliderRect.pivot = new Vector2(0, 1);
            sliderRect.anchoredPosition = new Vector2(10, -100);
            sliderRect.sizeDelta = new Vector2(-20, 20);
            
            _fpsSlider = sliderGO.AddComponent<Slider>();
            _fpsSlider.minValue = 30;
            _fpsSlider.maxValue = 120;
            _fpsSlider.value = 60;
            _fpsSlider.onValueChanged.AddListener(OnFPSSliderChanged);
            
            // Adaptive Toggle
            var adaptiveGO = new GameObject("AdaptiveToggle");
            adaptiveGO.transform.SetParent(_performanceUI.transform, false);
            
            var adaptiveRect = adaptiveGO.AddComponent<RectTransform>();
            adaptiveRect.anchorMin = new Vector2(0, 1);
            adaptiveRect.anchorMax = new Vector2(1, 1);
            adaptiveRect.pivot = new Vector2(0, 1);
            adaptiveRect.anchoredPosition = new Vector2(10, -130);
            adaptiveRect.sizeDelta = new Vector2(-20, 20);
            
            _adaptiveToggle = adaptiveGO.AddComponent<Toggle>();
            _adaptiveToggle.isOn = true;
            _adaptiveToggle.onValueChanged.AddListener(OnAdaptiveToggleChanged);
            
            // VSync Toggle
            var vsyncGO = new GameObject("VSyncToggle");
            vsyncGO.transform.SetParent(_performanceUI.transform, false);
            
            var vsyncRect = vsyncGO.AddComponent<RectTransform>();
            vsyncRect.anchorMin = new Vector2(0, 1);
            vsyncRect.anchorMax = new Vector2(1, 1);
            vsyncRect.pivot = new Vector2(0, 1);
            vsyncRect.anchoredPosition = new Vector2(10, -160);
            vsyncRect.sizeDelta = new Vector2(-20, 20);
            
            _vsyncToggle = vsyncGO.AddComponent<Toggle>();
            _vsyncToggle.isOn = true;
            _vsyncToggle.onValueChanged.AddListener(OnVSyncToggleChanged);
        }
        
        private void CreateToggleButton()
        {
            var buttonGO = new GameObject("ToggleButton");
            buttonGO.transform.SetParent(_performanceUI.transform, false);
            
            var buttonRect = buttonGO.AddComponent<RectTransform>();
            buttonRect.anchorMin = new Vector2(1, 1);
            buttonRect.anchorMax = new Vector2(1, 1);
            buttonRect.pivot = new Vector2(1, 1);
            buttonRect.anchoredPosition = new Vector2(-10, -10);
            buttonRect.sizeDelta = new Vector2(20, 20);
            
            var button = buttonGO.AddComponent<Button>();
            var buttonImage = buttonGO.AddComponent<Image>();
            buttonImage.color = Color.red;
            
            button.onClick.AddListener(() => {
                _uiEnabled = !_uiEnabled;
                _performanceUI.SetActive(_uiEnabled);
            });
        }
        
        private void UpdatePerformanceUI()
        {
            if (_performanceMonitor == null || _frameRateLimiter == null || _memoryOptimizer == null) return;
            
            // Обновляем FPS
            var frameRateStats = _frameRateLimiter.GetFrameRateStats();
            _fpsText.text = $"FPS: {frameRateStats.CurrentFPS:F1} / {frameRateStats.TargetFPS}";
            _fpsText.color = GetFPSColor(frameRateStats.CurrentFPS);
            
            // Обновляем память
            long memoryMB = _memoryOptimizer.TotalMemoryUsage / (1024 * 1024);
            _memoryText.text = $"Memory: {memoryMB}MB";
            _memoryText.color = GetMemoryColor(memoryMB);
            
            // Обновляем качество
            int qualityLevel = QualitySettings.GetQualityLevel();
            string qualityName = GetQualityName(qualityLevel);
            _qualityText.text = $"Quality: {qualityName}";
            _qualityText.color = GetQualityColor(qualityLevel);
            
            // Обновляем слайдер
            _fpsSlider.value = frameRateStats.TargetFPS;
        }
        
        private Color GetFPSColor(float fps)
        {
            if (fps >= 60f) return Color.green;
            if (fps >= 45f) return Color.yellow;
            return Color.red;
        }
        
        private Color GetMemoryColor(long memoryMB)
        {
            if (memoryMB < 100) return Color.green;
            if (memoryMB < 200) return Color.yellow;
            return Color.red;
        }
        
        private Color GetQualityColor(int qualityLevel)
        {
            switch (qualityLevel)
            {
                case 0: return Color.red;
                case 1: return Color.yellow;
                case 2: return Color.green;
                case 3: return Color.cyan;
                default: return Color.white;
            }
        }
        
        private string GetQualityName(int qualityLevel)
        {
            switch (qualityLevel)
            {
                case 0: return "Low";
                case 1: return "Medium";
                case 2: return "High";
                case 3: return "Ultra";
                default: return "Unknown";
            }
        }
        
        private void OnFPSSliderChanged(float value)
        {
            if (_frameRateLimiter != null)
            {
                _frameRateLimiter.SetTargetFrameRate((int)value);
            }
        }
        
        private void OnAdaptiveToggleChanged(bool value)
        {
            if (_frameRateLimiter != null)
            {
                _frameRateLimiter.SetAdaptiveFPS(value);
            }
        }
        
        private void OnVSyncToggleChanged(bool value)
        {
            if (_frameRateLimiter != null)
            {
                _frameRateLimiter.SetVSync(value);
            }
        }
        
        /// <summary>
        /// Показать/скрыть UI производительности
        /// </summary>
        public void SetUIEnabled(bool enabled)
        {
            _uiEnabled = enabled;
            if (_performanceUI != null)
            {
                _performanceUI.SetActive(enabled);
            }
        }
    }
}