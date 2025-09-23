using Unity.Entities;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Profiling;
using System.Collections.Generic;
using System.IO;

namespace MudLike.Profiler
{
    /// <summary>
    /// Стартер профилирования для Mud-Like
    /// Автоматически настраивает и запускает Unity Profiler
    /// </summary>
    public static class ProfilerStarter
    {
        private static readonly ProfilerMarker _profilerMarker = new ProfilerMarker("MudLike.Profiler");
        
        /// <summary>
        /// Запускает профилирование в Unity Editor
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void StartProfiling()
        {
            Debug.Log("🚀 Mud-Like Profiler Starter: Инициализация профилирования");
            
            // Настраиваем профилирование
            SetupProfilerSettings();
            
            // Запускаем профилирование
            StartProfilerConnection();
            
            // Настраиваем мониторинг производительности
            SetupPerformanceMonitoring();
            
            Debug.Log("✅ Профилирование запущено успешно");
        }
        
        /// <summary>
        /// Запускает headless профилирование
        /// </summary>
        public static void StartHeadlessProfiling()
        {
            Debug.Log("🚀 Mud-Like Profiler Starter: Headless профилирование");
            
            // Настраиваем профилирование
            SetupProfilerSettings();
            
            // Запускаем headless профилирование
            StartHeadlessProfiler();
            
            // Настраиваем мониторинг производительности
            SetupPerformanceMonitoring();
            
            Debug.Log("✅ Headless профилирование запущено успешно");
        }
        
        /// <summary>
        /// Настраивает параметры профилирования
        /// </summary>
        private static void SetupProfilerSettings()
        {
            // Включаем профилирование
            Profiler.enabled = true;
            Profiler.logFile = Path.Combine(Application.dataPath, "..", "ProfilerData", "profiler_data.raw");
            
            // Создаем папку для данных профилирования
            var profilerDataPath = Path.Combine(Application.dataPath, "..", "ProfilerData");
            if (!Directory.Exists(profilerDataPath))
            {
                Directory.CreateDirectory(profilerDataPath);
            }
            
            // Настраиваем параметры профилирования
            Profiler.maxUsedMemory = 1024 * 1024 * 1024; // 1GB
            Profiler.enableBinaryLog = true;
            Profiler.enableAllocationCallstacks = true;
            
            Debug.Log($"📊 Профилирование настроено. Данные: {profilerDataPath}");
        }
        
        /// <summary>
        /// Запускает подключение к профилировщику
        /// </summary>
        private static void StartProfilerConnection()
        {
            // Настраиваем подключение к профилировщику
            Profiler.SetConnectionProfiling(true);
            
            // Запускаем профилирование
            Profiler.BeginThreadProfiling("MudLike", "MainThread");
            
            Debug.Log("🔗 Подключение к профилировщику установлено");
        }
        
        /// <summary>
        /// Запускает headless профилирование
        /// </summary>
        private static void StartHeadlessProfiler()
        {
            // Настраиваем headless профилирование
            Profiler.SetConnectionProfiling(false);
            Profiler.enableBinaryLog = true;
            
            // Запускаем профилирование
            Profiler.BeginThreadProfiling("MudLike", "MainThread");
            
            Debug.Log("🔗 Headless профилирование запущено");
        }
        
        /// <summary>
        /// Настраивает мониторинг производительности
        /// </summary>
        private static void SetupPerformanceMonitoring()
        {
            // Создаем систему мониторинга производительности
            var world = World.DefaultGameObjectInjectionWorld;
            if (world != null)
            {
                world.GetOrCreateSystem<PerformanceMonitoringSystem>();
            }
            
            // Настраиваем мониторинг FPS
            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 1;
            
            Debug.Log("📈 Мониторинг производительности настроен");
        }
    }
    
    /// <summary>
    /// Система мониторинга производительности
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class PerformanceMonitoringSystem : SystemBase
    {
        private float _lastFPSCheck = 0f;
        private int _frameCount = 0;
        private float _currentFPS = 0f;
        private float _averageFPS = 0f;
        private List<float> _fpsHistory = new List<float>();
        
        private float _lastMemoryCheck = 0f;
        private long _currentMemory = 0;
        private long _peakMemory = 0;
        
        protected override void OnCreate()
        {
            Debug.Log("📊 PerformanceMonitoringSystem: Инициализация");
        }
        
        protected override void OnUpdate()
        {
            using (_profilerMarker.Auto())
            {
                float currentTime = (float)SystemAPI.Time.ElapsedTime;
                
                // Проверяем FPS каждые 0.5 секунды
                if (currentTime - _lastFPSCheck >= 0.5f)
                {
                    CheckFPS();
                    _lastFPSCheck = currentTime;
                }
                
                // Проверяем память каждые 1 секунду
                if (currentTime - _lastMemoryCheck >= 1f)
                {
                    CheckMemory();
                    _lastMemoryCheck = currentTime;
                }
                
                // Проверяем производительность ECS систем
                CheckECSSystemPerformance();
            }
        }
        
        /// <summary>
        /// Проверяет FPS
        /// </summary>
        private void CheckFPS()
        {
            _currentFPS = 1f / Time.unscaledDeltaTime;
            _fpsHistory.Add(_currentFPS);
            
            // Ограничиваем историю до 100 записей
            if (_fpsHistory.Count > 100)
            {
                _fpsHistory.RemoveAt(0);
            }
            
            // Вычисляем средний FPS
            float totalFPS = 0f;
            foreach (float fps in _fpsHistory)
            {
                totalFPS += fps;
            }
            _averageFPS = totalFPS / _fpsHistory.Count;
            
            // Логируем предупреждения
            if (_currentFPS < 30f)
            {
                Debug.LogWarning($"⚠️ Низкий FPS: {_currentFPS:F1} (средний: {_averageFPS:F1})");
            }
            else if (_currentFPS < 50f)
            {
                Debug.LogWarning($"⚠️ Средний FPS: {_currentFPS:F1} (средний: {_averageFPS:F1})");
            }
        }
        
        /// <summary>
        /// Проверяет использование памяти
        /// </summary>
        private void CheckMemory()
        {
            _currentMemory = Profiler.GetTotalAllocatedMemory(Profiler.Area.All);
            
            if (_currentMemory > _peakMemory)
            {
                _peakMemory = _currentMemory;
            }
            
            // Логируем предупреждения
            if (_currentMemory > 1024 * 1024 * 1024) // 1GB
            {
                Debug.LogWarning($"⚠️ Высокое использование памяти: {_currentMemory / (1024 * 1024):F1}MB");
            }
        }
        
        /// <summary>
        /// Проверяет производительность ECS систем
        /// </summary>
        private void CheckECSSystemPerformance()
        {
            // Проверяем производительность основных систем
            var world = World.DefaultGameObjectInjectionWorld;
            if (world != null)
            {
                // Проверяем VehicleMovementSystem
                var vehicleSystem = world.GetExistingSystemManaged<MudLike.Vehicles.Systems.VehicleMovementSystem>();
                if (vehicleSystem != null)
                {
                    // Здесь можно добавить проверку производительности системы
                }
                
                // Проверяем TerrainDeformationSystem
                var terrainSystem = world.GetExistingSystemManaged<MudLike.Terrain.Systems.TerrainDeformationSystem>();
                if (terrainSystem != null)
                {
                    // Здесь можно добавить проверку производительности системы
                }
            }
        }
        
        protected override void OnDestroy()
        {
            // Сохраняем данные профилирования
            SaveProfilerData();
        }
        
        /// <summary>
        /// Сохраняет данные профилирования
        /// </summary>
        private void SaveProfilerData()
        {
            var profilerDataPath = Path.Combine(Application.dataPath, "..", "ProfilerData", "performance_data.json");
            
            var data = new
            {
                average_fps = _averageFPS,
                peak_memory_mb = _peakMemory / (1024 * 1024),
                current_memory_mb = _currentMemory / (1024 * 1024),
                fps_history = _fpsHistory.ToArray(),
                timestamp = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };
            
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(profilerDataPath, json);
            
            Debug.Log($"📊 Данные производительности сохранены: {profilerDataPath}");
        }
    }
}
