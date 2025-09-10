using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using MudLike.Core.Systems;
using System.Collections;

namespace MudLike.Tests.Unit
{
    /// <summary>
    /// Базовые тесты для SystemInfoIntegration
    /// Самый безопасный модуль для тестирования
    /// </summary>
    public class SystemInfoIntegrationBasicTest
    {
        [Test]
        public void TestAvailableUtilities()
        {
            // Тест 1: Проверка доступности системных утилит
            Debug.Log("[SystemInfoTest] Тестирование доступности утилит...");
            
            string utilities = SystemInfoIntegration.GetAvailableUtilities();
            
            Assert.IsNotNull(utilities, "Список утилит не должен быть null");
            Assert.IsTrue(utilities.Length > 0, "Должна быть доступна хотя бы одна утилита");
            
            Debug.Log($"[SystemInfoTest] Доступные утилиты: {utilities}");
            
            // Проверяем, что есть хотя бы базовые /proc файлы
            Assert.IsTrue(utilities.Contains("/proc/cpuinfo") || 
                         utilities.Contains("/proc/meminfo") || 
                         utilities.Contains("/proc/loadavg"), 
                         "Должны быть доступны базовые /proc файлы");
        }
        
        [Test]
        public void TestSystemInfoStructure()
        {
            // Тест 2: Проверка структуры SystemInfo
            Debug.Log("[SystemInfoTest] Тестирование структуры SystemInfo...");
            
            var systemInfo = new SystemInfo();
            
            // Проверяем, что структура инициализируется корректно
            Assert.AreEqual(0, systemInfo.CPUCores, "CPUCores должно быть 0 по умолчанию");
            Assert.AreEqual(0f, systemInfo.CPULoad, "CPULoad должно быть 0 по умолчанию");
            Assert.AreEqual(0, systemInfo.TotalRAM, "TotalRAM должно быть 0 по умолчанию");
            Assert.AreEqual(0, systemInfo.UsedRAM, "UsedRAM должно быть 0 по умолчанию");
            Assert.AreEqual(0f, systemInfo.RAMUsage, "RAMUsage должно быть 0 по умолчанию");
            Assert.AreEqual(0f, systemInfo.CPUTemperature, "CPUTemperature должно быть 0 по умолчанию");
            Assert.AreEqual(0f, systemInfo.Uptime, "Uptime должно быть 0 по умолчанию");
            Assert.AreEqual(0f, systemInfo.LoadAverage, "LoadAverage должно быть 0 по умолчанию");
            Assert.IsNull(systemInfo.CPUModel, "CPUModel должно быть null по умолчанию");
            
            Debug.Log("[SystemInfoTest] Структура SystemInfo корректна");
        }
        
        [Test]
        public void TestGetSystemInfoSafety()
        {
            // Тест 3: Безопасный вызов GetSystemInfo
            Debug.Log("[SystemInfoTest] Тестирование безопасного вызова GetSystemInfo...");
            
            try
            {
                var systemInfo = SystemInfoIntegration.GetSystemInfo();
                
                // Проверяем, что метод не выбрасывает исключений
                Assert.IsNotNull(systemInfo, "SystemInfo не должен быть null");
                
                // Проверяем разумные значения
                Assert.GreaterOrEqual(systemInfo.CPUCores, 1, "Количество ядер должно быть >= 1");
                Assert.GreaterOrEqual(systemInfo.CPULoad, 0f, "Нагрузка CPU должна быть >= 0");
                Assert.LessOrEqual(systemInfo.CPULoad, 100f, "Нагрузка CPU должна быть <= 100");
                Assert.GreaterOrEqual(systemInfo.CPUTemperature, 0f, "Температура должна быть >= 0");
                Assert.LessOrEqual(systemInfo.CPUTemperature, 200f, "Температура должна быть <= 200°C");
                Assert.GreaterOrEqual(systemInfo.RAMUsage, 0f, "Использование RAM должно быть >= 0");
                Assert.LessOrEqual(systemInfo.RAMUsage, 100f, "Использование RAM должно быть <= 100");
                
                Debug.Log($"[SystemInfoTest] GetSystemInfo выполнен успешно:");
                Debug.Log($"  CPU: {systemInfo.CPUModel}, Cores: {systemInfo.CPUCores}");
                Debug.Log($"  Load: {systemInfo.CPULoad:F1}%, Temp: {systemInfo.CPUTemperature:F1}°C");
                Debug.Log($"  RAM: {systemInfo.RAMUsage:F1}%, Uptime: {systemInfo.Uptime:F1}h");
            }
            catch (System.Exception e)
            {
                Assert.Fail($"GetSystemInfo не должен выбрасывать исключения: {e.Message}");
            }
        }
        
        [Test]
        public void TestMultipleCalls()
        {
            // Тест 4: Множественные вызовы для проверки стабильности
            Debug.Log("[SystemInfoTest] Тестирование множественных вызовов...");
            
            for (int i = 0; i < 5; i++)
            {
                try
                {
                    var systemInfo = SystemInfoIntegration.GetSystemInfo();
                    Assert.IsNotNull(systemInfo, $"Вызов {i+1}: SystemInfo не должен быть null");
                    
                    // Небольшая пауза между вызовами
                    System.Threading.Thread.Sleep(100);
                }
                catch (System.Exception e)
                {
                    Assert.Fail($"Вызов {i+1} не должен выбрасывать исключения: {e.Message}");
                }
            }
            
            Debug.Log("[SystemInfoTest] Множественные вызовы выполнены успешно");
        }
    }
}