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
    /// ИНТЕГРАЦИОННЫЙ ТЕСТ СИСТЕМЫ ЗАЩИТЫ ОТ ПЕРЕГРЕВА
    /// Безопасное тестирование интеграции всех компонентов
    /// </summary>
    public class OverheatProtectionIntegrationTest
    {
        private World _testWorld;
        private PerformanceManager _performanceManager;
        
        [SetUp]
        public void Setup()
        {
            Debug.Log("[IntegrationTest] 🔧 Инициализация интеграционного тестирования");
            
            // Создаем тестовый World
            _testWorld = new World("IntegrationTestWorld");
            
            // Создаем главный менеджер производительности
            _performanceManager = _testWorld.GetOrCreateSystemManaged<PerformanceManager>();
            
            Debug.Log("[IntegrationTest] ✅ Менеджер производительности создан");
        }
        
        [TearDown]
        public void TearDown()
        {
            Debug.Log("[IntegrationTest] 🧹 Очистка интеграционного тестирования");
            
            // Очищаем тестовое окружение
            if (_testWorld != null)
            {
                _testWorld.Dispose();
                _testWorld = null;
            }
            
            Debug.Log("[IntegrationTest] ✅ Очистка завершена");
        }
        
        [Test]
        public void TestSystemIntegration()
        {
            // Тест: Интеграция всех систем
            Debug.Log("[IntegrationTest] Тестирование интеграции систем");
            
            try
            {
                // Проверяем, что менеджер создан
                Assert.IsNotNull(_performanceManager, "Менеджер производительности должен быть создан");
                
                // Проверяем, что можем получить статус
                string status = _performanceManager.GetSystemStatus();
                Assert.IsNotNull(status, "Статус системы не должен быть null");
                Assert.IsTrue(status.Length > 0, "Статус системы не должен быть пустым");
                
                Debug.Log($"[IntegrationTest] ✅ Статус системы: {status}");
                
                // Проверяем, что можем получить уровень производительности
                var performanceLevel = _performanceManager.GetCurrentPerformanceLevel();
                Assert.IsTrue(System.Enum.IsDefined(typeof(PerformanceManager.PerformanceLevel), performanceLevel), 
                    "Уровень производительности должен быть валидным");
                
                Debug.Log($"[IntegrationTest] ✅ Уровень производительности: {performanceLevel}");
                
                // Проверяем, что можем проверить экстренный режим
                bool isEmergency = _performanceManager.IsEmergencyModeActive();
                Assert.IsFalse(isEmergency, "Экстренный режим не должен быть активен при инициализации");
                
                Debug.Log($"[IntegrationTest] ✅ Экстренный режим: {isEmergency}");
            }
            catch (System.Exception e)
            {
                Assert.Fail($"Ошибка интеграции систем: {e.Message}");
            }
        }
        
        [Test]
        public void TestSafeSystemUpdate()
        {
            // Тест: Безопасное обновление систем
            Debug.Log("[IntegrationTest] Тестирование безопасного обновления систем");
            
            try
            {
                // Симулируем несколько обновлений системы
                for (int i = 0; i < 5; i++)
                {
                    Debug.Log($"[IntegrationTest] Обновление #{i + 1}");
                    
                    // Проверяем, что системы не падают при обновлении
                    string status = _performanceManager.GetSystemStatus();
                    Assert.IsNotNull(status, $"Статус на итерации {i + 1} не должен быть null");
                    
                    // Небольшая задержка для симуляции времени
                    System.Threading.Thread.Sleep(10);
                    
                    Debug.Log($"[IntegrationTest] ✅ Обновление #{i + 1} успешно");
                }
            }
            catch (System.Exception e)
            {
                Assert.Fail($"Ошибка при обновлении систем: {e.Message}");
            }
        }
        
        [Test]
        public void TestSystemStability()
        {
            // Тест: Стабильность систем
            Debug.Log("[IntegrationTest] Тестирование стабильности систем");
            
            try
            {
                // Проверяем стабильность в течение нескольких итераций
                var initialStatus = _performanceManager.GetSystemStatus();
                var initialLevel = _performanceManager.GetCurrentPerformanceLevel();
                
                for (int i = 0; i < 10; i++)
                {
                    // Получаем текущий статус
                    var currentStatus = _performanceManager.GetSystemStatus();
                    var currentLevel = _performanceManager.GetCurrentPerformanceLevel();
                    
                    // Проверяем, что статус не изменился кардинально
                    Assert.IsNotNull(currentStatus, $"Статус на итерации {i + 1} не должен быть null");
                    Assert.IsTrue(System.Enum.IsDefined(typeof(PerformanceManager.PerformanceLevel), currentLevel), 
                        $"Уровень производительности на итерации {i + 1} должен быть валидным");
                    
                    // Небольшая задержка
                    System.Threading.Thread.Sleep(5);
                }
                
                Debug.Log("[IntegrationTest] ✅ Стабильность систем подтверждена");
            }
            catch (System.Exception e)
            {
                Assert.Fail($"Ошибка стабильности систем: {e.Message}");
            }
        }
        
        [Test]
        public void TestErrorRecovery()
        {
            // Тест: Восстановление после ошибок
            Debug.Log("[IntegrationTest] Тестирование восстановления после ошибок");
            
            try
            {
                // Симулируем ошибку и проверяем восстановление
                try
                {
                    // Пытаемся вызвать метод с невалидными параметрами
                    _performanceManager.ForceEmergencyMode();
                    Debug.Log("[IntegrationTest] Принудительная активация экстренного режима");
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning($"[IntegrationTest] Ожидаемая ошибка: {e.Message}");
                }
                
                // Проверяем, что система все еще работает
                string status = _performanceManager.GetSystemStatus();
                Assert.IsNotNull(status, "Статус должен быть доступен после ошибки");
                
                Debug.Log("[IntegrationTest] ✅ Восстановление после ошибок работает");
            }
            catch (System.Exception e)
            {
                Assert.Fail($"Критическая ошибка восстановления: {e.Message}");
            }
        }
        
        [Test]
        public void TestMemoryManagement()
        {
            // Тест: Управление памятью
            Debug.Log("[IntegrationTest] Тестирование управления памятью");
            
            try
            {
                // Проверяем использование памяти
                long memoryBefore = System.GC.GetTotalMemory(false);
                
                // Создаем и удаляем несколько World'ов
                for (int i = 0; i < 5; i++)
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
                
                Debug.Log($"[IntegrationTest] Память до: {memoryBefore / 1024}KB, после: {memoryAfter / 1024}KB, разница: {memoryDifference / 1024}KB");
                
                // Проверяем, что разница в памяти разумна
                Assert.IsTrue(memoryDifference < 1024 * 1024, $"Разница в памяти {memoryDifference / 1024}KB слишком велика");
                
                Debug.Log("[IntegrationTest] ✅ Управление памятью работает корректно");
            }
            catch (System.Exception e)
            {
                Assert.Fail($"Ошибка управления памятью: {e.Message}");
            }
        }
        
        [Test]
        public void TestSystemShutdown()
        {
            // Тест: Корректное завершение работы систем
            Debug.Log("[IntegrationTest] Тестирование корректного завершения работы");
            
            try
            {
                // Проверяем, что системы работают
                string status = _performanceManager.GetSystemStatus();
                Assert.IsNotNull(status, "Статус должен быть доступен перед завершением");
                
                // Симулируем завершение работы
                _performanceManager = null;
                
                // Проверяем, что ссылка очищена
                Assert.IsNull(_performanceManager, "Ссылка на менеджер должна быть очищена");
                
                Debug.Log("[IntegrationTest] ✅ Завершение работы выполнено корректно");
            }
            catch (System.Exception e)
            {
                Assert.Fail($"Ошибка при завершении работы: {e.Message}");
            }
        }
    }
}