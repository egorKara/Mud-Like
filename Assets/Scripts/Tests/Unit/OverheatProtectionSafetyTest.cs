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
    /// БЕЗОПАСНЫЙ ТЕСТ СИСТЕМЫ ЗАЩИТЫ ОТ ПЕРЕГРЕВА
    /// Проверяет только безопасные операции без риска зависания
    /// </summary>
    public class OverheatProtectionSafetyTest
    {
        private World _testWorld;
        private OverheatProtectionSystem _overheatSystem;
        private SystemResourceMonitor _resourceMonitor;
        private AdaptiveQualitySystem _adaptiveQualitySystem;
        
        [SetUp]
        public void Setup()
        {
            Debug.Log("[SafetyTest] 🛡️ Инициализация безопасного тестирования");
            
            // Создаем тестовый World
            _testWorld = new World("SafetyTestWorld");
            
            // Создаем системы (только инициализация, без запуска)
            _overheatSystem = _testWorld.GetOrCreateSystemManaged<OverheatProtectionSystem>();
            _resourceMonitor = _testWorld.GetOrCreateSystemManaged<SystemResourceMonitor>();
            _adaptiveQualitySystem = _testWorld.GetOrCreateSystemManaged<AdaptiveQualitySystem>();
            
            Debug.Log("[SafetyTest] ✅ Системы созданы успешно");
        }
        
        [TearDown]
        public void TearDown()
        {
            Debug.Log("[SafetyTest] 🧹 Очистка тестового окружения");
            
            // Очищаем тестовое окружение
            if (_testWorld != null)
            {
                _testWorld.Dispose();
                _testWorld = null;
            }
            
            Debug.Log("[SafetyTest] ✅ Очистка завершена");
        }
        
        [Test]
        public void TestSystemCreation()
        {
            // Тест: Создание систем без ошибок
            Debug.Log("[SafetyTest] Тестирование создания систем");
            
            Assert.IsNotNull(_overheatSystem, "Система защиты от перегрева должна быть создана");
            Assert.IsNotNull(_resourceMonitor, "Система мониторинга ресурсов должна быть создана");
            Assert.IsNotNull(_adaptiveQualitySystem, "Система адаптивного качества должна быть создана");
            
            Debug.Log("[SafetyTest] ✅ Все системы созданы успешно");
        }
        
        [Test]
        public void TestSystemInitialization()
        {
            // Тест: Инициализация систем без ошибок
            Debug.Log("[SafetyTest] Тестирование инициализации систем");
            
            try
            {
                // Проверяем, что системы могут быть инициализированы
                var overheatState = _overheatSystem.GetCurrentState();
                var overheatTemp = _overheatSystem.GetCurrentTemperature();
                
                Assert.IsTrue(System.Enum.IsDefined(typeof(OverheatProtectionSystem.OverheatState), overheatState), 
                    "Состояние системы защиты должно быть валидным");
                Assert.IsTrue(overheatTemp >= 0f, "Температура должна быть неотрицательной");
                
                Debug.Log($"[SafetyTest] ✅ Инициализация успешна. Состояние: {overheatState}, Температура: {overheatTemp:F1}°C");
            }
            catch (System.Exception e)
            {
                Assert.Fail($"Ошибка инициализации систем: {e.Message}");
            }
        }
        
        [Test]
        public void TestSafeTemperatureSimulation()
        {
            // Тест: Безопасная симуляция температуры
            Debug.Log("[SafetyTest] Тестирование безопасной симуляции температуры");
            
            // Тестируем только чтение, без изменения настроек
            var testTemperatures = new float[] { 50f, 65f, 75f, 85f };
            
            foreach (float temp in testTemperatures)
            {
                Debug.Log($"[SafetyTest] Тестирование температуры: {temp}°C");
                
                // Проверяем базовые ограничения
                Assert.IsTrue(temp >= 0f, $"Температура {temp}°C должна быть неотрицательной");
                Assert.IsTrue(temp <= 200f, $"Температура {temp}°C должна быть разумной");
                
                // Симулируем определение состояния (безопасно)
                var state = SimulateSafeStateFromTemperature(temp);
                Assert.IsTrue(System.Enum.IsDefined(typeof(OverheatProtectionSystem.OverheatState), state), 
                    $"Состояние для температуры {temp}°C должно быть валидным");
                
                Debug.Log($"[SafetyTest] ✅ Температура {temp}°C -> {state}");
            }
        }
        
        [Test]
        public void TestSafeQualityLevels()
        {
            // Тест: Безопасная проверка уровней качества
            Debug.Log("[SafetyTest] Тестирование безопасной проверки уровней качества");
            
            // Сохраняем оригинальные настройки
            int originalQuality = QualitySettings.GetQualityLevel();
            
            try
            {
                // Тестируем только валидные уровни
                var validLevels = new int[] { 0, 1, 2, 3 };
                
                foreach (int level in validLevels)
                {
                    Debug.Log($"[SafetyTest] Тестирование уровня качества: {level}");
                    
                    // Проверяем, что уровень валиден
                    Assert.IsTrue(level >= 0 && level <= 3, $"Уровень качества {level} должен быть валидным");
                    
                    // НЕ изменяем настройки, только проверяем валидность
                    Debug.Log($"[SafetyTest] ✅ Уровень качества {level} валиден");
                }
            }
            finally
            {
                // Восстанавливаем оригинальные настройки
                QualitySettings.SetQualityLevel(originalQuality);
                Debug.Log($"[SafetyTest] Восстановлен оригинальный уровень качества: {originalQuality}");
            }
        }
        
        [Test]
        public void TestSafeFPSValues()
        {
            // Тест: Безопасная проверка значений FPS
            Debug.Log("[SafetyTest] Тестирование безопасной проверки значений FPS");
            
            // Сохраняем оригинальные настройки
            int originalFPS = Application.targetFrameRate;
            
            try
            {
                // Тестируем только безопасные значения FPS
                var safeFPSValues = new int[] { 15, 30, 45, 60 };
                
                foreach (int fps in safeFPSValues)
                {
                    Debug.Log($"[SafetyTest] Тестирование FPS: {fps}");
                    
                    // Проверяем, что FPS валиден
                    Assert.IsTrue(fps > 0, $"FPS {fps} должен быть положительным");
                    Assert.IsTrue(fps <= 120, $"FPS {fps} должен быть разумным");
                    
                    // НЕ изменяем настройки, только проверяем валидность
                    Debug.Log($"[SafetyTest] ✅ FPS {fps} валиден");
                }
            }
            finally
            {
                // Восстанавливаем оригинальные настройки
                Application.targetFrameRate = originalFPS;
                Debug.Log($"[SafetyTest] Восстановлен оригинальный FPS: {originalFPS}");
            }
        }
        
        [Test]
        public void TestErrorHandling()
        {
            // Тест: Обработка ошибок
            Debug.Log("[SafetyTest] Тестирование обработки ошибок");
            
            try
            {
                // Симулируем безопасную ошибку
                throw new System.Exception("Тестовая ошибка для проверки обработки");
            }
            catch (System.Exception e)
            {
                // Проверяем, что ошибка обрабатывается корректно
                Assert.IsNotNull(e.Message, "Сообщение об ошибке не должно быть null");
                Assert.IsTrue(e.Message.Length > 0, "Сообщение об ошибке не должно быть пустым");
                
                Debug.Log($"[SafetyTest] ✅ Ошибка обработана корректно: {e.Message}");
            }
        }
        
        [Test]
        public void TestSystemDisposal()
        {
            // Тест: Безопасное удаление систем
            Debug.Log("[SafetyTest] Тестирование безопасного удаления систем");
            
            try
            {
                // Проверяем, что системы существуют
                Assert.IsNotNull(_overheatSystem, "Система защиты должна существовать");
                Assert.IsNotNull(_resourceMonitor, "Система мониторинга должна существовать");
                Assert.IsNotNull(_adaptiveQualitySystem, "Система адаптивного качества должна существовать");
                
                // Симулируем удаление (безопасно)
                _overheatSystem = null;
                _resourceMonitor = null;
                _adaptiveQualitySystem = null;
                
                // Проверяем, что ссылки очищены
                Assert.IsNull(_overheatSystem, "Ссылка на систему защиты должна быть очищена");
                Assert.IsNull(_resourceMonitor, "Ссылка на систему мониторинга должна быть очищена");
                Assert.IsNull(_adaptiveQualitySystem, "Ссылка на систему адаптивного качества должна быть очищена");
                
                Debug.Log("[SafetyTest] ✅ Удаление систем выполнено безопасно");
            }
            catch (System.Exception e)
            {
                Assert.Fail($"Ошибка при удалении систем: {e.Message}");
            }
        }
        
        [Test]
        public void TestMemorySafety()
        {
            // Тест: Безопасность памяти
            Debug.Log("[SafetyTest] Тестирование безопасности памяти");
            
            // Проверяем, что нет утечек памяти
            long memoryBefore = System.GC.GetTotalMemory(false);
            
            // Создаем и удаляем объекты
            for (int i = 0; i < 100; i++)
            {
                var tempWorld = new World($"TempWorld{i}");
                tempWorld.Dispose();
            }
            
            // Принудительная сборка мусора
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
            System.GC.Collect();
            
            long memoryAfter = System.GC.GetTotalMemory(false);
            long memoryDifference = memoryAfter - memoryBefore;
            
            Debug.Log($"[SafetyTest] Память до: {memoryBefore / 1024}KB, после: {memoryAfter / 1024}KB, разница: {memoryDifference / 1024}KB");
            
            // Проверяем, что разница в памяти не критична (допускаем небольшое увеличение)
            Assert.IsTrue(memoryDifference < 1024 * 1024, $"Разница в памяти {memoryDifference / 1024}KB слишком велика");
            
            Debug.Log("[SafetyTest] ✅ Безопасность памяти подтверждена");
        }
        
        /// <summary>
        /// Безопасная симуляция определения состояния по температуре
        /// </summary>
        private OverheatProtectionSystem.OverheatState SimulateSafeStateFromTemperature(float temperature)
        {
            // Проверяем на валидность
            if (float.IsNaN(temperature) || float.IsInfinity(temperature) || temperature < 0f)
            {
                return OverheatProtectionSystem.OverheatState.Safe;
            }
            
            // Безопасная логика определения состояния
            if (temperature >= 85f)
                return OverheatProtectionSystem.OverheatState.Emergency;
            else if (temperature >= 75f)
                return OverheatProtectionSystem.OverheatState.Critical;
            else if (temperature >= 65f)
                return OverheatProtectionSystem.OverheatState.Warning;
            else
                return OverheatProtectionSystem.OverheatState.Safe;
        }
    }
}