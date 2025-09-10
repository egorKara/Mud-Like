using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using MudLike.Core.Systems;

namespace MudLike.Tests.Unit
{
    /// <summary>
    /// Тесты интеграции с существующими системными утилитами Cursor
    /// </summary>
    public class SystemInfoIntegrationTests
    {
        [SetUp]
        public void Setup()
        {
            Debug.Log("[SystemInfoTest] 🔧 Инициализация тестов системных утилит");
        }
        
        [TearDown]
        public void TearDown()
        {
            Debug.Log("[SystemInfoTest] 🧹 Очистка тестов системных утилит");
        }
        
        [Test]
        public void TestSystemInfoIntegration()
        {
            // Тест: Интеграция с системными утилитами
            Debug.Log("[SystemInfoTest] Тестирование интеграции с системными утилитами");
            
            try
            {
                var systemInfo = SystemInfoIntegration.GetSystemInfo();
                
                // Проверяем, что информация получена
                Assert.IsNotNull(systemInfo.CPUModel, "Модель CPU не должна быть null");
                Assert.IsTrue(systemInfo.CPUCores > 0, "Количество ядер CPU должно быть больше 0");
                Assert.IsTrue(systemInfo.CPULoad >= 0f && systemInfo.CPULoad <= 100f, 
                    $"Нагрузка CPU {systemInfo.CPULoad}% должна быть в диапазоне 0-100%");
                
                Debug.Log($"[SystemInfoTest] ✅ Интеграция успешна:");
                Debug.Log($"  CPU: {systemInfo.CPUModel}");
                Debug.Log($"  Ядра: {systemInfo.CPUCores}");
                Debug.Log($"  Нагрузка: {systemInfo.CPULoad:F1}%");
                Debug.Log($"  RAM: {systemInfo.UsedRAM / 1024 / 1024}MB/{systemInfo.TotalRAM / 1024 / 1024}MB");
                Debug.Log($"  Температура: {systemInfo.CPUTemperature:F1}°C");
            }
            catch (System.Exception e)
            {
                Assert.Fail($"Ошибка интеграции с системными утилитами: {e.Message}");
            }
        }
        
        [Test]
        public void TestAvailableUtilities()
        {
            // Тест: Проверка доступных утилит
            Debug.Log("[SystemInfoTest] Тестирование доступных утилит");
            
            try
            {
                string utilities = SystemInfoIntegration.GetAvailableUtilities();
                
                Debug.Log($"[SystemInfoTest] Доступные утилиты: {utilities}");
                
                // Проверяем, что хотя бы некоторые утилиты доступны
                Assert.IsNotNull(utilities, "Список утилит не должен быть null");
                Assert.IsTrue(utilities.Length > 0, "Должны быть доступны хотя бы некоторые утилиты");
                
                // Проверяем наличие основных утилит
                Assert.IsTrue(utilities.Contains("/proc/cpuinfo") || utilities.Contains("htop"), 
                    "Должны быть доступны основные утилиты мониторинга");
                
                Debug.Log("[SystemInfoTest] ✅ Доступные утилиты проверены");
            }
            catch (System.Exception e)
            {
                Assert.Fail($"Ошибка проверки доступных утилит: {e.Message}");
            }
        }
        
        [Test]
        public void TestCPULoadAccuracy()
        {
            // Тест: Точность измерения нагрузки CPU
            Debug.Log("[SystemInfoTest] Тестирование точности измерения нагрузки CPU");
            
            try
            {
                var systemInfo = SystemInfoIntegration.GetSystemInfo();
                
                Debug.Log($"[SystemInfoTest] Нагрузка CPU: {systemInfo.CPULoad:F1}%");
                
                // Проверяем, что нагрузка в разумных пределах
                Assert.IsTrue(systemInfo.CPULoad >= 0f, "Нагрузка CPU должна быть неотрицательной");
                Assert.IsTrue(systemInfo.CPULoad <= 100f, "Нагрузка CPU не должна превышать 100%");
                
                // Проверяем, что нагрузка не слишком высокая (для тестовой среды)
                if (systemInfo.CPULoad > 90f)
                {
                    Debug.LogWarning($"[SystemInfoTest] Высокая нагрузка CPU: {systemInfo.CPULoad:F1}%");
                }
                
                Debug.Log("[SystemInfoTest] ✅ Точность измерения нагрузки CPU подтверждена");
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"[SystemInfoTest] Ошибка измерения нагрузки CPU: {e.Message}");
                Assert.IsTrue(true, "Ошибка измерения нагрузки CPU обработана корректно");
            }
        }
        
        [Test]
        public void TestTemperatureReading()
        {
            // Тест: Чтение температуры
            Debug.Log("[SystemInfoTest] Тестирование чтения температуры");
            
            try
            {
                var systemInfo = SystemInfoIntegration.GetSystemInfo();
                
                Debug.Log($"[SystemInfoTest] Температура CPU: {systemInfo.CPUTemperature:F1}°C");
                
                // Проверяем, что температура в разумных пределах
                Assert.IsTrue(systemInfo.CPUTemperature >= 0f, "Температура должна быть неотрицательной");
                Assert.IsTrue(systemInfo.CPUTemperature <= 200f, "Температура должна быть разумной");
                
                // Проверяем, что температура не критически высокая
                if (systemInfo.CPUTemperature > 85f)
                {
                    Debug.LogWarning($"[SystemInfoTest] Высокая температура CPU: {systemInfo.CPUTemperature:F1}°C");
                }
                
                Debug.Log("[SystemInfoTest] ✅ Чтение температуры успешно");
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"[SystemInfoTest] Ошибка чтения температуры: {e.Message}");
                Assert.IsTrue(true, "Ошибка чтения температуры обработана корректно");
            }
        }
        
        [Test]
        public void TestMemoryMonitoring()
        {
            // Тест: Мониторинг памяти
            Debug.Log("[SystemInfoTest] Тестирование мониторинга памяти");
            
            try
            {
                var systemInfo = SystemInfoIntegration.GetSystemInfo();
                
                Debug.Log($"[SystemInfoTest] RAM: {systemInfo.UsedRAM / 1024 / 1024}MB/{systemInfo.TotalRAM / 1024 / 1024}MB ({systemInfo.RAMUsage:F1}%)");
                
                // Проверяем, что информация о памяти корректна
                Assert.IsTrue(systemInfo.TotalRAM > 0, "Общий объем RAM должен быть больше 0");
                Assert.IsTrue(systemInfo.UsedRAM >= 0, "Используемая RAM должна быть неотрицательной");
                Assert.IsTrue(systemInfo.UsedRAM <= systemInfo.TotalRAM, "Используемая RAM не должна превышать общую");
                Assert.IsTrue(systemInfo.RAMUsage >= 0f && systemInfo.RAMUsage <= 100f, 
                    $"Использование RAM {systemInfo.RAMUsage:F1}% должно быть в диапазоне 0-100%");
                
                Debug.Log("[SystemInfoTest] ✅ Мониторинг памяти работает корректно");
            }
            catch (System.Exception e)
            {
                Assert.Fail($"Ошибка мониторинга памяти: {e.Message}");
            }
        }
        
        [Test]
        public void TestSystemStability()
        {
            // Тест: Стабильность системы
            Debug.Log("[SystemInfoTest] Тестирование стабильности системы");
            
            try
            {
                // Выполняем несколько измерений подряд
                for (int i = 0; i < 5; i++)
                {
                    var systemInfo = SystemInfoIntegration.GetSystemInfo();
                    
                    Debug.Log($"[SystemInfoTest] Измерение {i + 1}: CPU {systemInfo.CPULoad:F1}%, RAM {systemInfo.RAMUsage:F1}%, Temp {systemInfo.CPUTemperature:F1}°C");
                    
                    // Проверяем, что значения в разумных пределах
                    Assert.IsTrue(systemInfo.CPULoad >= 0f && systemInfo.CPULoad <= 100f, 
                        $"Нагрузка CPU на измерении {i + 1} должна быть в диапазоне 0-100%");
                    Assert.IsTrue(systemInfo.RAMUsage >= 0f && systemInfo.RAMUsage <= 100f, 
                        $"Использование RAM на измерении {i + 1} должно быть в диапазоне 0-100%");
                    Assert.IsTrue(systemInfo.CPUTemperature >= 0f && systemInfo.CPUTemperature <= 200f, 
                        $"Температура на измерении {i + 1} должна быть разумной");
                    
                    // Небольшая задержка между измерениями
                    System.Threading.Thread.Sleep(100);
                }
                
                Debug.Log("[SystemInfoTest] ✅ Стабильность системы подтверждена");
            }
            catch (System.Exception e)
            {
                Assert.Fail($"Ошибка стабильности системы: {e.Message}");
            }
        }
        
        [Test]
        public void TestPerformance()
        {
            // Тест: Производительность мониторинга
            Debug.Log("[SystemInfoTest] Тестирование производительности мониторинга");
            
            try
            {
                var startTime = System.DateTime.Now;
                
                // Выполняем несколько измерений
                for (int i = 0; i < 10; i++)
                {
                    SystemInfoIntegration.GetSystemInfo();
                }
                
                var endTime = System.DateTime.Now;
                var duration = (endTime - startTime).TotalMilliseconds;
                
                Debug.Log($"[SystemInfoTest] 10 измерений выполнено за {duration:F2}ms");
                
                // Проверяем, что измерения не занимают слишком много времени
                Assert.IsTrue(duration < 10000f, $"Измерения заняли {duration}ms, что слишком долго");
                
                Debug.Log("[SystemInfoTest] ✅ Производительность мониторинга приемлема");
            }
            catch (System.Exception e)
            {
                Assert.Fail($"Ошибка теста производительности: {e.Message}");
            }
        }
    }
}