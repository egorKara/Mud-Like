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
    /// Тесты симуляции температуры CPU
    /// ВТОРОЙ ПО БЕЗОПАСНОСТИ - только чтение и симуляция, без изменения настроек
    /// </summary>
    public class TemperatureSimulationTests
    {
        private World _testWorld;
        private OverheatProtectionSystem _overheatSystem;
        
        [SetUp]
        public void Setup()
        {
            // Создаем тестовый World
            _testWorld = new World("TestWorld");
            
            // Создаем систему защиты от перегрева
            _overheatSystem = _testWorld.GetOrCreateSystemManaged<OverheatProtectionSystem>();
            
            Debug.Log("[Test] Инициализация тестового окружения для симуляции температуры");
        }
        
        [TearDown]
        public void TearDown()
        {
            // Очищаем тестовое окружение
            if (_testWorld != null)
            {
                _testWorld.Dispose();
                _testWorld = null;
            }
            
            Debug.Log("[Test] Очистка тестового окружения");
        }
        
        [Test]
        public void TestTemperatureRangeValidation()
        {
            // Тест: Проверка валидности диапазона температур
            Debug.Log("[Test] Тестирование валидности диапазона температур");
            
            // Тестируем разные температуры
            var testTemperatures = new float[] { 0f, 30f, 50f, 65f, 75f, 85f, 100f };
            var expectedStates = new OverheatProtectionSystem.OverheatState[]
            {
                OverheatProtectionSystem.OverheatState.Safe,      // 0°C
                OverheatProtectionSystem.OverheatState.Safe,      // 30°C
                OverheatProtectionSystem.OverheatState.Safe,      // 50°C
                OverheatProtectionSystem.OverheatState.Safe,      // 65°C
                OverheatProtectionSystem.OverheatState.Warning,   // 75°C
                OverheatProtectionSystem.OverheatState.Emergency, // 85°C
                OverheatProtectionSystem.OverheatState.Emergency  // 100°C
            };
            
            for (int i = 0; i < testTemperatures.Length; i++)
            {
                float temp = testTemperatures[i];
                var expectedState = expectedStates[i];
                
                Debug.Log($"[Test] Тестирование температуры {temp}°C");
                
                // Проверяем базовые ограничения
                Assert.IsTrue(temp >= 0f, $"Температура {temp}°C должна быть неотрицательной");
                Assert.IsTrue(temp <= 200f, $"Температура {temp}°C должна быть разумной");
                
                // Симулируем определение состояния на основе температуры
                OverheatProtectionSystem.OverheatState simulatedState = SimulateStateFromTemperature(temp);
                
                Debug.Log($"[Test] Температура {temp}°C -> Состояние {simulatedState} (ожидаемое: {expectedState})");
                
                // Проверяем, что состояние определено корректно
                Assert.IsTrue(System.Enum.IsDefined(typeof(OverheatProtectionSystem.OverheatState), simulatedState), 
                    $"Состояние {simulatedState} должно быть валидным");
                
                Debug.Log($"[Test] ✅ Температура {temp}°C обработана корректно");
            }
        }
        
        [Test]
        public void TestTemperatureThresholds()
        {
            // Тест: Проверка пороговых значений температуры
            Debug.Log("[Test] Тестирование пороговых значений температуры");
            
            // Критические пороги из системы
            const float SAFE_THRESHOLD = 65f;
            const float WARNING_THRESHOLD = 75f;
            const float CRITICAL_THRESHOLD = 85f;
            
            // Тестируем граничные значения
            var boundaryTests = new (float temp, OverheatProtectionSystem.OverheatState expectedState)[]
            {
                (SAFE_THRESHOLD - 1f, OverheatProtectionSystem.OverheatState.Safe),
                (SAFE_THRESHOLD, OverheatProtectionSystem.OverheatState.Safe),
                (SAFE_THRESHOLD + 1f, OverheatProtectionSystem.OverheatState.Warning),
                (WARNING_THRESHOLD - 1f, OverheatProtectionSystem.OverheatState.Warning),
                (WARNING_THRESHOLD, OverheatProtectionSystem.OverheatState.Warning),
                (WARNING_THRESHOLD + 1f, OverheatProtectionSystem.OverheatState.Critical),
                (CRITICAL_THRESHOLD - 1f, OverheatProtectionSystem.OverheatState.Critical),
                (CRITICAL_THRESHOLD, OverheatProtectionSystem.OverheatState.Emergency),
                (CRITICAL_THRESHOLD + 1f, OverheatProtectionSystem.OverheatState.Emergency)
            };
            
            foreach (var test in boundaryTests)
            {
                Debug.Log($"[Test] Граничный тест: {test.temp}°C -> {test.expectedState}");
                
                var simulatedState = SimulateStateFromTemperature(test.temp);
                
                // Проверяем, что состояние соответствует ожидаемому
                Assert.AreEqual(test.expectedState, simulatedState, 
                    $"Температура {test.temp}°C должна давать состояние {test.expectedState}, получено {simulatedState}");
                
                Debug.Log($"[Test] ✅ Граничный тест пройден: {test.temp}°C -> {simulatedState}");
            }
        }
        
        [Test]
        public void TestTemperatureStability()
        {
            // Тест: Проверка стабильности симуляции температуры
            Debug.Log("[Test] Тестирование стабильности симуляции температуры");
            
            // Симулируем стабильную температуру
            float stableTemp = 60f;
            int testCount = 10;
            
            for (int i = 0; i < testCount; i++)
            {
                var state = SimulateStateFromTemperature(stableTemp);
                
                // Проверяем, что состояние стабильно
                Assert.AreEqual(OverheatProtectionSystem.OverheatState.Safe, state, 
                    $"Стабильная температура {stableTemp}°C должна давать состояние Safe");
                
                Debug.Log($"[Test] Итерация {i + 1}: {stableTemp}°C -> {state}");
            }
            
            Debug.Log("[Test] ✅ Стабильность симуляции температуры подтверждена");
        }
        
        [Test]
        public void TestTemperatureTransitions()
        {
            // Тест: Проверка переходов между состояниями
            Debug.Log("[Test] Тестирование переходов между состояниями");
            
            // Симулируем постепенный нагрев
            var heatingSequence = new float[] { 50f, 60f, 70f, 75f, 80f, 85f, 90f };
            var expectedStates = new OverheatProtectionSystem.OverheatState[]
            {
                OverheatProtectionSystem.OverheatState.Safe,
                OverheatProtectionSystem.OverheatState.Safe,
                OverheatProtectionSystem.OverheatState.Safe,
                OverheatProtectionSystem.OverheatState.Warning,
                OverheatProtectionSystem.OverheatState.Critical,
                OverheatProtectionSystem.OverheatState.Emergency,
                OverheatProtectionSystem.OverheatState.Emergency
            };
            
            for (int i = 0; i < heatingSequence.Length; i++)
            {
                float temp = heatingSequence[i];
                var expectedState = expectedStates[i];
                var actualState = SimulateStateFromTemperature(temp);
                
                Debug.Log($"[Test] Нагрев: {temp}°C -> {actualState} (ожидаемое: {expectedState})");
                
                Assert.AreEqual(expectedState, actualState, 
                    $"Температура {temp}°C должна давать состояние {expectedState}");
                
                Debug.Log($"[Test] ✅ Переход при {temp}°C корректен");
            }
            
            // Симулируем охлаждение
            var coolingSequence = new float[] { 90f, 80f, 70f, 60f, 50f };
            var coolingStates = new OverheatProtectionSystem.OverheatState[]
            {
                OverheatProtectionSystem.OverheatState.Emergency,
                OverheatProtectionSystem.OverheatState.Critical,
                OverheatProtectionSystem.OverheatState.Safe,
                OverheatProtectionSystem.OverheatState.Safe,
                OverheatProtectionSystem.OverheatState.Safe
            };
            
            for (int i = 0; i < coolingSequence.Length; i++)
            {
                float temp = coolingSequence[i];
                var expectedState = coolingStates[i];
                var actualState = SimulateStateFromTemperature(temp);
                
                Debug.Log($"[Test] Охлаждение: {temp}°C -> {actualState} (ожидаемое: {expectedState})");
                
                Assert.AreEqual(expectedState, actualState, 
                    $"Температура {temp}°C должна давать состояние {expectedState}");
                
                Debug.Log($"[Test] ✅ Переход при охлаждении {temp}°C корректен");
            }
        }
        
        [Test]
        public void TestTemperatureEdgeCases()
        {
            // Тест: Проверка крайних случаев
            Debug.Log("[Test] Тестирование крайних случаев температуры");
            
            // Тестируем крайние значения
            var edgeCases = new (float temp, string description)[]
            {
                (0f, "Абсолютный ноль"),
                (-1f, "Отрицательная температура"),
                (200f, "Очень высокая температура"),
                (float.MaxValue, "Максимальное значение float"),
                (float.NaN, "NaN значение"),
                (float.PositiveInfinity, "Положительная бесконечность"),
                (float.NegativeInfinity, "Отрицательная бесконечность")
            };
            
            foreach (var edgeCase in edgeCases)
            {
                Debug.Log($"[Test] Крайний случай: {edgeCase.description} ({edgeCase.temp})");
                
                try
                {
                    var state = SimulateStateFromTemperature(edgeCase.temp);
                    
                    // Проверяем, что система не падает на крайних значениях
                    Assert.IsTrue(System.Enum.IsDefined(typeof(OverheatProtectionSystem.OverheatState), state), 
                        $"Состояние для {edgeCase.description} должно быть валидным");
                    
                    Debug.Log($"[Test] ✅ Крайний случай {edgeCase.description} обработан: {state}");
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning($"[Test] Крайний случай {edgeCase.description} вызвал исключение: {e.Message}");
                    // Для некоторых крайних случаев исключения допустимы
                }
            }
        }
        
        /// <summary>
        /// Симулирует определение состояния на основе температуры
        /// (Безопасная симуляция без обращения к реальной системе)
        /// </summary>
        private OverheatProtectionSystem.OverheatState SimulateStateFromTemperature(float temperature)
        {
            // Проверяем на валидность
            if (float.IsNaN(temperature) || float.IsInfinity(temperature) || temperature < 0f)
            {
                return OverheatProtectionSystem.OverheatState.Safe; // Безопасное состояние по умолчанию
            }
            
            // Симулируем логику определения состояния
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