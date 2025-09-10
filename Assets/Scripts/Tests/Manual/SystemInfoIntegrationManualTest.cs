using UnityEngine;
using MudLike.Core.Systems;

namespace MudLike.Tests.Manual
{
    /// <summary>
    /// Ручное тестирование SystemInfoIntegration
    /// Безопасный способ проверки без Unity Test Runner
    /// </summary>
    public class SystemInfoIntegrationManualTest : MonoBehaviour
    {
        [Header("Настройки тестирования")]
        [SerializeField] private bool runTestOnStart = true;
        [SerializeField] private float testInterval = 2f;
        [SerializeField] private int maxTestRuns = 10;
        
        private int currentTestRun = 0;
        private float lastTestTime = 0f;
        
        void Start()
        {
            if (runTestOnStart)
            {
                Debug.Log("[SystemInfoManualTest] Запуск ручного тестирования SystemInfoIntegration");
                RunBasicTest();
            }
        }
        
        void Update()
        {
            if (runTestOnStart && currentTestRun < maxTestRuns)
            {
                if (Time.time - lastTestTime >= testInterval)
                {
                    RunBasicTest();
                    lastTestTime = Time.time;
                    currentTestRun++;
                }
            }
        }
        
        [ContextMenu("Запустить базовый тест")]
        public void RunBasicTest()
        {
            Debug.Log($"[SystemInfoManualTest] === Тест #{currentTestRun + 1} ===");
            
            try
            {
                // Тест 1: Проверка доступности утилит
                string utilities = SystemInfoIntegration.GetAvailableUtilities();
                Debug.Log($"[SystemInfoManualTest] Доступные утилиты: {utilities}");
                
                // Тест 2: Получение информации о системе
                var systemInfo = SystemInfoIntegration.GetSystemInfo();
                
                Debug.Log($"[SystemInfoManualTest] Результат теста:");
                Debug.Log($"  CPU Model: {systemInfo.CPUModel}");
                Debug.Log($"  CPU Cores: {systemInfo.CPUCores}");
                Debug.Log($"  CPU Load: {systemInfo.CPULoad:F1}%");
                Debug.Log($"  CPU Temperature: {systemInfo.CPUTemperature:F1}°C");
                Debug.Log($"  RAM Usage: {systemInfo.RAMUsage:F1}% ({systemInfo.UsedRAM / 1024 / 1024}MB / {systemInfo.TotalRAM / 1024 / 1024}MB)");
                Debug.Log($"  System Uptime: {systemInfo.Uptime:F1} hours");
                Debug.Log($"  Load Average: {systemInfo.LoadAverage:F2}");
                
                // Проверка разумности значений
                bool valuesReasonable = true;
                
                if (systemInfo.CPUCores < 1)
                {
                    Debug.LogWarning("[SystemInfoManualTest] Предупреждение: Количество ядер CPU < 1");
                    valuesReasonable = false;
                }
                
                if (systemInfo.CPULoad < 0f || systemInfo.CPULoad > 100f)
                {
                    Debug.LogWarning($"[SystemInfoManualTest] Предупреждение: Нагрузка CPU вне диапазона 0-100%: {systemInfo.CPULoad:F1}%");
                    valuesReasonable = false;
                }
                
                if (systemInfo.CPUTemperature < 0f || systemInfo.CPUTemperature > 200f)
                {
                    Debug.LogWarning($"[SystemInfoManualTest] Предупреждение: Температура CPU вне разумного диапазона: {systemInfo.CPUTemperature:F1}°C");
                    valuesReasonable = false;
                }
                
                if (systemInfo.RAMUsage < 0f || systemInfo.RAMUsage > 100f)
                {
                    Debug.LogWarning($"[SystemInfoManualTest] Предупреждение: Использование RAM вне диапазона 0-100%: {systemInfo.RAMUsage:F1}%");
                    valuesReasonable = false;
                }
                
                if (valuesReasonable)
                {
                    Debug.Log("[SystemInfoManualTest] ✅ Все значения в разумных пределах");
                }
                else
                {
                    Debug.LogWarning("[SystemInfoManualTest] ⚠️ Обнаружены подозрительные значения");
                }
                
                Debug.Log($"[SystemInfoManualTest] === Тест #{currentTestRun + 1} завершен успешно ===");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[SystemInfoManualTest] ❌ Ошибка в тесте #{currentTestRun + 1}: {e.Message}");
                Debug.LogError($"[SystemInfoManualTest] Stack trace: {e.StackTrace}");
            }
        }
        
        [ContextMenu("Остановить тестирование")]
        public void StopTesting()
        {
            runTestOnStart = false;
            Debug.Log("[SystemInfoManualTest] Тестирование остановлено");
        }
        
        [ContextMenu("Сбросить счетчик тестов")]
        public void ResetTestCounter()
        {
            currentTestRun = 0;
            lastTestTime = 0f;
            Debug.Log("[SystemInfoManualTest] Счетчик тестов сброшен");
        }
        
        void OnGUI()
        {
            if (Application.isPlaying)
            {
                GUILayout.BeginArea(new Rect(10, 10, 300, 200));
                GUILayout.Label($"SystemInfoIntegration Manual Test");
                GUILayout.Label($"Test Run: {currentTestRun + 1}/{maxTestRuns}");
                GUILayout.Label($"Next Test In: {testInterval - (Time.time - lastTestTime):F1}s");
                
                if (GUILayout.Button("Run Test Now"))
                {
                    RunBasicTest();
                }
                
                if (GUILayout.Button("Stop Testing"))
                {
                    StopTesting();
                }
                
                if (GUILayout.Button("Reset Counter"))
                {
                    ResetTestCounter();
                }
                
                GUILayout.EndArea();
            }
        }
    }
}