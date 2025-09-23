using Unity.Entities;
using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;
using MudLike.Core.Components;

namespace MudLike.Examples.Systems
{
    /// <summary>
    /// ECS система для примеров
    /// Заменяет MonoBehaviour примеры
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class ExampleSystem : SystemBase
    {
        private EntityQuery _exampleQuery;
        private EntityQuery _testQuery;
        
        protected override void OnCreate()
        {
            // Создаем запросы для примеров
            _exampleQuery = GetEntityQuery(typeof(ExampleData), typeof(UIElement));
            _testQuery = GetEntityQuery(typeof(TestData), typeof(UIElement));
        }
        
        protected override void OnUpdate()
        {
            // Обрабатываем примеры
            ProcessExamples();
            
            // Обрабатываем тесты
            ProcessTests();
        }
        
        /// <summary>
        /// Обрабатывает примеры
        /// </summary>
        private void ProcessExamples()
        {
            Entities
                .WithAll<ExampleData, UIElement>()
                .ForEach((ref ExampleData example, ref UIElement element) =>
                {
                    if (example.IsActive)
                    {
                        UpdateExample(example);
                    }
                }).WithoutBurst().Run();
        }
        
        /// <summary>
        /// Обрабатывает тесты
        /// </summary>
        private void ProcessTests()
        {
            Entities
                .WithAll<TestData, UIElement>()
                .ForEach((ref TestData test, ref UIElement element) =>
                {
                    if (test.IsRunning)
                    {
                        UpdateTest(test);
                    }
                }).WithoutBurst().Run();
        }
        
        /// <summary>
        /// Обновляет пример
        /// </summary>
        private void UpdateExample(ExampleData example)
        {
            // Логика обновления примера
            example.UpdateTime += SystemAPI.Time.DeltaTime;
            
            if (example.UpdateTime >= example.Duration)
            {
                example.IsActive = false;
                example.IsCompleted = true;
            }
        }
        
        /// <summary>
        /// Обновляет тест
        /// </summary>
        private void UpdateTest(TestData test)
        {
            // Логика обновления теста
            test.ElapsedTime += SystemAPI.Time.DeltaTime;
            
            if (test.ElapsedTime >= test.Timeout)
            {
                test.IsRunning = false;
                test.IsCompleted = true;
                test.Result = TestResult.Timeout;
            }
        }
    }
    
    /// <summary>
    /// Данные примера
    /// </summary>
    public struct ExampleData : IComponentData
    {
        public int ExampleId;
        public bool IsActive;
        public bool IsCompleted;
        public float UpdateTime;
        public float Duration;
        public int StepCount;
        public int CurrentStep;
    }
    
    /// <summary>
    /// Данные теста
    /// </summary>
    public struct TestData : IComponentData
    {
        public int TestId;
        public bool IsRunning;
        public bool IsCompleted;
        public float ElapsedTime;
        public float Timeout;
        public TestResult Result;
        public int PassedChecks;
        public int TotalChecks;
    }
    
    /// <summary>
    /// Результат теста
    /// </summary>
    public enum TestResult
    {
        None,
        Passed,
        Failed,
        Timeout,
        Error
    }
}
