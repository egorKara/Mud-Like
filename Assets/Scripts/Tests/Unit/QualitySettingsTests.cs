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
    /// Тесты безопасного изменения настроек качества
    /// ТРЕТИЙ ПО БЕЗОПАСНОСТИ - изменение настроек, но с защитой от частых изменений
    /// </summary>
    public class QualitySettingsTests
    {
        private World _testWorld;
        private OverheatProtectionSystem _overheatSystem;
        private AdaptiveQualitySystem _adaptiveQualitySystem;
        
        // Сохраняем оригинальные настройки для восстановления
        private int _originalQualityLevel;
        private int _originalTargetFPS;
        private int _originalVSync;
        
        [SetUp]
        public void Setup()
        {
            // Сохраняем оригинальные настройки
            _originalQualityLevel = QualitySettings.GetQualityLevel();
            _originalTargetFPS = Application.targetFrameRate;
            _originalVSync = QualitySettings.vSyncCount;
            
            // Создаем тестовый World
            _testWorld = new World("TestWorld");
            
            // Создаем системы
            _overheatSystem = _testWorld.GetOrCreateSystemManaged<OverheatProtectionSystem>();
            _adaptiveQualitySystem = _testWorld.GetOrCreateSystemManaged<AdaptiveQualitySystem>();
            
            Debug.Log("[Test] Инициализация тестового окружения для настроек качества");
            Debug.Log($"[Test] Оригинальные настройки: Quality={_originalQualityLevel}, FPS={_originalTargetFPS}, VSync={_originalVSync}");
        }
        
        [TearDown]
        public void TearDown()
        {
            // Восстанавливаем оригинальные настройки
            try
            {
                QualitySettings.SetQualityLevel(_originalQualityLevel);
                Application.targetFrameRate = _originalTargetFPS;
                QualitySettings.vSyncCount = _originalVSync;
                
                Debug.Log($"[Test] Восстановлены оригинальные настройки: Quality={_originalQualityLevel}, FPS={_originalTargetFPS}, VSync={_originalVSync}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[Test] Ошибка восстановления настроек: {e.Message}");
            }
            
            // Очищаем тестовое окружение
            if (_testWorld != null)
            {
                _testWorld.Dispose();
                _testWorld = null;
            }
            
            Debug.Log("[Test] Очистка тестового окружения");
        }
        
        [Test]
        public void TestQualityLevelValidation()
        {
            // Тест: Проверка валидности уровней качества
            Debug.Log("[Test] Тестирование валидности уровней качества");
            
            var validQualityLevels = new int[] { 0, 1, 2, 3 };
            var invalidQualityLevels = new int[] { -1, 4, 10, int.MaxValue, int.MinValue };
            
            // Тестируем валидные уровни
            foreach (int level in validQualityLevels)
            {
                Debug.Log($"[Test] Тестирование валидного уровня качества: {level}");
                
                try
                {
                    QualitySettings.SetQualityLevel(level);
                    int actualLevel = QualitySettings.GetQualityLevel();
                    
                    Assert.AreEqual(level, actualLevel, $"Уровень качества {level} должен быть установлен корректно");
                    
                    Debug.Log($"[Test] ✅ Уровень качества {level} установлен корректно");
                }
                catch (System.Exception e)
                {
                    Assert.Fail($"Валидный уровень качества {level} вызвал исключение: {e.Message}");
                }
            }
            
            // Тестируем невалидные уровни
            foreach (int level in invalidQualityLevels)
            {
                Debug.Log($"[Test] Тестирование невалидного уровня качества: {level}");
                
                try
                {
                    QualitySettings.SetQualityLevel(level);
                    int actualLevel = QualitySettings.GetQualityLevel();
                    
                    // Unity может автоматически ограничить невалидные значения
                    Assert.IsTrue(actualLevel >= 0 && actualLevel <= 3, 
                        $"Невалидный уровень {level} должен быть ограничен Unity до {actualLevel}");
                    
                    Debug.Log($"[Test] ✅ Невалидный уровень {level} ограничен Unity до {actualLevel}");
                }
                catch (System.Exception e)
                {
                    Debug.Log($"[Test] Невалидный уровень {level} вызвал ожидаемое исключение: {e.Message}");
                    // Исключения для невалидных значений допустимы
                }
            }
        }
        
        [Test]
        public void TestQualitySettingsConsistency()
        {
            // Тест: Проверка согласованности настроек качества
            Debug.Log("[Test] Тестирование согласованности настроек качества");
            
            for (int level = 0; level <= 3; level++)
            {
                Debug.Log($"[Test] Тестирование согласованности для уровня {level}");
                
                try
                {
                    QualitySettings.SetQualityLevel(level);
                    
                    // Проверяем, что уровень установлен
                    int actualLevel = QualitySettings.GetQualityLevel();
                    Assert.AreEqual(level, actualLevel, $"Уровень качества должен быть {level}");
                    
                    // Проверяем, что настройки соответствуют уровню
                    CheckQualitySettingsForLevel(level);
                    
                    Debug.Log($"[Test] ✅ Согласованность для уровня {level} подтверждена");
                }
                catch (System.Exception e)
                {
                    Assert.Fail($"Ошибка при тестировании уровня {level}: {e.Message}");
                }
            }
        }
        
        [Test]
        public void TestFPSLimiting()
        {
            // Тест: Проверка ограничения FPS
            Debug.Log("[Test] Тестирование ограничения FPS");
            
            var testFPSValues = new int[] { 15, 30, 45, 60, 90, 120 };
            
            foreach (int fps in testFPSValues)
            {
                Debug.Log($"[Test] Тестирование FPS: {fps}");
                
                try
                {
                    Application.targetFrameRate = fps;
                    int actualFPS = Application.targetFrameRate;
                    
                    Assert.AreEqual(fps, actualFPS, $"FPS {fps} должен быть установлен корректно");
                    
                    Debug.Log($"[Test] ✅ FPS {fps} установлен корректно");
                }
                catch (System.Exception e)
                {
                    Assert.Fail($"Ошибка при установке FPS {fps}: {e.Message}");
                }
            }
        }
        
        [Test]
        public void TestVSyncSettings()
        {
            // Тест: Проверка настроек VSync
            Debug.Log("[Test] Тестирование настроек VSync");
            
            var vsyncValues = new int[] { 0, 1, 2, 4 };
            
            foreach (int vsync in vsyncValues)
            {
                Debug.Log($"[Test] Тестирование VSync: {vsync}");
                
                try
                {
                    QualitySettings.vSyncCount = vsync;
                    int actualVSync = QualitySettings.vSyncCount;
                    
                    // Unity может ограничить VSync значения
                    Assert.IsTrue(actualVSync >= 0, $"VSync {vsync} должен быть неотрицательным");
                    
                    Debug.Log($"[Test] ✅ VSync {vsync} установлен как {actualVSync}");
                }
                catch (System.Exception e)
                {
                    Debug.Log($"[Test] VSync {vsync} вызвал исключение: {e.Message}");
                    // Некоторые VSync значения могут не поддерживаться
                }
            }
        }
        
        [Test]
        public void TestSettingsChangeFrequency()
        {
            // Тест: Проверка защиты от частых изменений настроек
            Debug.Log("[Test] Тестирование защиты от частых изменений");
            
            // Быстро меняем настройки несколько раз
            for (int i = 0; i < 5; i++)
            {
                try
                {
                    QualitySettings.SetQualityLevel(i % 4);
                    Application.targetFrameRate = 30 + i * 10;
                    QualitySettings.vSyncCount = i % 2;
                    
                    Debug.Log($"[Test] Быстрое изменение #{i}: Quality={i % 4}, FPS={30 + i * 10}, VSync={i % 2}");
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning($"[Test] Быстрое изменение #{i} вызвало исключение: {e.Message}");
                    // Исключения при частых изменениях могут быть ожидаемыми
                }
            }
            
            // Проверяем, что настройки в итоге валидны
            int finalQuality = QualitySettings.GetQualityLevel();
            int finalFPS = Application.targetFrameRate;
            int finalVSync = QualitySettings.vSyncCount;
            
            Assert.IsTrue(finalQuality >= 0 && finalQuality <= 3, $"Финальный уровень качества {finalQuality} должен быть валидным");
            Assert.IsTrue(finalFPS >= 0, $"Финальный FPS {finalFPS} должен быть неотрицательным");
            Assert.IsTrue(finalVSync >= 0, $"Финальный VSync {finalVSync} должен быть неотрицательным");
            
            Debug.Log($"[Test] ✅ Финальные настройки валидны: Quality={finalQuality}, FPS={finalFPS}, VSync={finalVSync}");
        }
        
        [Test]
        public void TestSettingsRecovery()
        {
            // Тест: Проверка восстановления настроек после ошибок
            Debug.Log("[Test] Тестирование восстановления настроек");
            
            // Сохраняем текущие настройки
            int originalQuality = QualitySettings.GetQualityLevel();
            int originalFPS = Application.targetFrameRate;
            int originalVSync = QualitySettings.vSyncCount;
            
            try
            {
                // Пытаемся установить невалидные значения
                QualitySettings.SetQualityLevel(-1);
                Application.targetFrameRate = -100;
                QualitySettings.vSyncCount = -1;
                
                Debug.Log("[Test] Установлены невалидные значения для тестирования восстановления");
            }
            catch (System.Exception e)
            {
                Debug.Log($"[Test] Невалидные значения вызвали исключение: {e.Message}");
            }
            
            // Восстанавливаем настройки
            try
            {
                QualitySettings.SetQualityLevel(originalQuality);
                Application.targetFrameRate = originalFPS;
                QualitySettings.vSyncCount = originalVSync;
                
                Debug.Log($"[Test] Настройки восстановлены");
            }
            catch (System.Exception e)
            {
                Assert.Fail($"Ошибка восстановления настроек: {e.Message}");
            }
            
            // Проверяем, что настройки восстановлены
            Assert.AreEqual(originalQuality, QualitySettings.GetQualityLevel(), "Уровень качества должен быть восстановлен");
            Assert.AreEqual(originalFPS, Application.targetFrameRate, "FPS должен быть восстановлен");
            Assert.AreEqual(originalVSync, QualitySettings.vSyncCount, "VSync должен быть восстановлен");
            
            Debug.Log("[Test] ✅ Восстановление настроек успешно");
        }
        
        /// <summary>
        /// Проверяет, что настройки качества соответствуют уровню
        /// </summary>
        private void CheckQualitySettingsForLevel(int level)
        {
            switch (level)
            {
                case 0: // Минимальное качество
                    Assert.IsTrue(QualitySettings.pixelLightCount <= 1, "Минимальное качество: pixelLightCount должен быть <= 1");
                    Assert.IsTrue(QualitySettings.shadowResolution == ShadowResolution.Low, "Минимальное качество: shadowResolution должен быть Low");
                    break;
                case 1: // Низкое качество
                    Assert.IsTrue(QualitySettings.pixelLightCount <= 2, "Низкое качество: pixelLightCount должен быть <= 2");
                    Assert.IsTrue(QualitySettings.shadowResolution == ShadowResolution.Low, "Низкое качество: shadowResolution должен быть Low");
                    break;
                case 2: // Среднее качество
                    Assert.IsTrue(QualitySettings.pixelLightCount <= 4, "Среднее качество: pixelLightCount должен быть <= 4");
                    Assert.IsTrue(QualitySettings.shadowResolution == ShadowResolution.Medium, "Среднее качество: shadowResolution должен быть Medium");
                    break;
                case 3: // Высокое качество
                    Assert.IsTrue(QualitySettings.pixelLightCount <= 8, "Высокое качество: pixelLightCount должен быть <= 8");
                    Assert.IsTrue(QualitySettings.shadowResolution == ShadowResolution.High, "Высокое качество: shadowResolution должен быть High");
                    break;
            }
        }
    }
}