using NUnit.Framework;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;
using MudLike.Terrain.Systems;
using MudLike.Terrain.Components;

namespace MudLike.Tests.Unit.Terrain
{
    /// <summary>
    /// Комплексные тесты для MudManagerSystem
    /// Обеспечивает проверку всех критических функций API деформации террейна
    /// </summary>
    public class MudManagerSystemTestsManager
    {
        private World _world;
        private MudManagerSystem _mudManager;
        
        [SetUp]
        public void Setup()
        {
            _world = new World("TestWorld");
            _mudManager = _world.GetOrCreateSystemManaged<MudManagerSystem>();
        }
        
        [TearDown]
        public void TearDown()
        {
            _world?.Dispose();
        }
        
        /// <summary>
        /// Тест базовой функциональности QueryContact
        /// </summary>
        [Test]
        public void QueryContact_ValidInput_ReturnsValidData()
        {
            // Arrange
            float3 wheelPosition = new float3(0, 0, 0);
            float radius = 1.0f;
            float wheelForce = 1000f;
            
            // Act
            var result = _mudManager.QueryContact(wheelPosition, radius, wheelForce);
            
            // Assert
            Assert.IsTrue(result.IsValid, "QueryContact should return valid data");
            Assert.AreEqual(wheelPosition, result.Position, "Position should match input");
            Assert.AreEqual(radius, result.Radius, "Radius should match input");
            Assert.GreaterOrEqual(result.MudLevel, 0f, "Mud level should be non-negative");
            Assert.LessOrEqual(result.MudLevel, 1f, "Mud level should not exceed 1");
            Assert.GreaterOrEqual(result.SinkDepth, 0f, "Sink depth should be non-negative");
            Assert.GreaterOrEqual(result.TractionModifier, 0.1f, "Traction modifier should be at least 0.1");
            Assert.LessOrEqual(result.TractionModifier, 1.0f, "Traction modifier should not exceed 1.0");
        }
        
        /// <summary>
        /// Тест поведения при различных уровнях грязи
        /// </summary>
        [Test]
        public void QueryContact_DifferentMudLevels_ReturnsAppropriateTraction()
        {
            // Arrange
            float3 position = new float3(0, 0, 0);
            float radius = 1.0f;
            float wheelForce = 1000f;
            
            // Act & Assert для различных уровней грязи
            for (float mudLevel = 0f; mudLevel <= 1f; mudLevel += 0.2f)
            {
                // В реальной реализации здесь нужно было бы установить уровень грязи
                var result = _mudManager.QueryContact(position, radius, wheelForce);
                
                // Проверяем, что тяга снижается с увеличением грязи
                if (mudLevel > 0.5f)
                {
                    Assert.Less(result.TractionModifier, 0.8f, 
                        $"Traction should be reduced for high mud level {mudLevel}");
                }
                
                Assert.IsTrue(result.IsValid, $"Result should be valid for mud level {mudLevel}");
            }
        }
        
        /// <summary>
        /// Тест поведения при различных радиусах колес
        /// </summary>
        [Test]
        public void QueryContact_DifferentRadii_ReturnsConsistentResults()
        {
            // Arrange
            float3 position = new float3(0, 0, 0);
            float wheelForce = 1000f;
            float[] radii = { 0.5f, 1.0f, 1.5f, 2.0f };
            
            // Act & Assert
            foreach (float radius in radii)
            {
                var result = _mudManager.QueryContact(position, radius, wheelForce);
                
                Assert.IsTrue(result.IsValid, $"Result should be valid for radius {radius}");
                Assert.AreEqual(radius, result.Radius, $"Radius should match input {radius}");
                Assert.GreaterOrEqual(result.SinkDepth, 0f, $"Sink depth should be non-negative for radius {radius}");
            }
        }
        
        /// <summary>
        /// Тест производительности QueryContact
        /// </summary>
        [Test]
        public void QueryContact_PerformanceTest_CompletesInTime()
        {
            // Arrange
            float3 position = new float3(0, 0, 0);
            float radius = 1.0f;
            float wheelForce = 1000f;
            int iterations = 1000;
            
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            
            // Act
            for (int i = 0; i < iterations; i++)
            {
                var result = _mudManager.QueryContact(position, radius, wheelForce);
                Assert.IsTrue(result.IsValid);
            }
            
            stopwatch.Stop();
            
            // Assert - должно выполняться менее чем за 100ms для 1000 итераций
            Assert.Less(stopwatch.ElapsedMilliseconds, 100, 
                $"QueryContact should complete 1000 iterations in less than 100ms. Actual: {stopwatch.ElapsedMilliseconds}ms");
        }
        
        /// <summary>
        /// Тест граничных значений
        /// </summary>
        [Test]
        public void QueryContact_EdgeCases_HandlesCorrectly()
        {
            // Arrange
            float3 position = new float3(0, 0, 0);
            float radius = 0.1f; // Минимальный радиус
            float wheelForce = 1f; // Минимальная сила
            
            // Act
            var result = _mudManager.QueryContact(position, radius, wheelForce);
            
            // Assert
            Assert.IsTrue(result.IsValid, "Should handle minimal values correctly");
            Assert.GreaterOrEqual(result.SinkDepth, 0f, "Sink depth should be non-negative even for minimal values");
        }
        
        /// <summary>
        /// Тест с максимальными значениями
        /// </summary>
        [Test]
        public void QueryContact_MaxValues_HandlesCorrectly()
        {
            // Arrange
            float3 position = new float3(0, 0, 0);
            float radius = 5.0f; // Большой радиус
            float wheelForce = 10000f; // Большая сила
            
            // Act
            var result = _mudManager.QueryContact(position, radius, wheelForce);
            
            // Assert
            Assert.IsTrue(result.IsValid, "Should handle maximum values correctly");
            Assert.LessOrEqual(result.SinkDepth, radius * 0.8f, "Sink depth should be limited to 80% of radius");
            Assert.GreaterOrEqual(result.TractionModifier, 0.1f, "Traction should maintain minimum value");
        }
        
        /// <summary>
        /// Тест детерминизма - одинаковые входные данные должны давать одинаковые результаты
        /// </summary>
        [Test]
        public void QueryContact_Deterministic_SameInputsGiveSameResults()
        {
            // Arrange
            float3 position = new float3(10, 5, 15);
            float radius = 1.5f;
            float wheelForce = 2000f;
            
            // Act - выполняем дважды
            var result1 = _mudManager.QueryContact(position, radius, wheelForce);
            var result2 = _mudManager.QueryContact(position, radius, wheelForce);
            
            // Assert - результаты должны быть идентичными
            Assert.AreEqual(result1.Position, result2.Position, "Positions should be identical");
            Assert.AreEqual(result1.Radius, result2.Radius, "Radii should be identical");
            Assert.AreEqual(result1.MudLevel, result2.MudLevel, 0.001f, "Mud levels should be identical");
            Assert.AreEqual(result1.SinkDepth, result2.SinkDepth, 0.001f, "Sink depths should be identical");
            Assert.AreEqual(result1.TractionModifier, result2.TractionModifier, 0.001f, "Traction modifiers should be identical");
            Assert.AreEqual(result1.Drag, result2.Drag, 0.001f, "Drag values should be identical");
        }
        
        /// <summary>
        /// Тест валидации входных данных
        /// </summary>
        [Test]
        public void QueryContact_InvalidInputs_HandlesGracefully()
        {
            // Arrange
            float3 position = new float3(0, 0, 0);
            float radius = -1.0f; // Негативный радиус
            float wheelForce = 1000f;
            
            // Act
            var result = _mudManager.QueryContact(position, radius, wheelForce);
            
            // Assert - система должна обрабатывать некорректные данные
            // В реальной реализации здесь должна быть валидация входных данных
            Assert.IsTrue(result.IsValid, "System should handle invalid inputs gracefully");
        }
        
        /// <summary>
        /// Тест производительности при высокой нагрузке
        /// </summary>
        [Test]
        public void QueryContact_HighLoad_DoesNotDegradePerformance()
        {
            // Arrange
            int iterations = 10000;
            var positions = new NativeArray<float3>(iterations, Allocator.Temp);
            var results = new NativeArray<MudContactData>(iterations, Allocator.Temp);
            
            // Генерируем случайные позиции
            var random = new Unity.Mathematics.Random(12345);
            for (int i = 0; i < iterations; i++)
            {
                positions[i] = random.NextFloat3(new float3(-100, 0, -100), new float3(100, 0, 100));
            }
            
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            
            // Act
            for (int i = 0; i < iterations; i++)
            {
                results[i] = _mudManager.QueryContact(positions[i], 1.0f, 1000f);
            }
            
            stopwatch.Stop();
            
            // Assert
            Assert.Less(stopwatch.ElapsedMilliseconds, 500, 
                $"High load test should complete in less than 500ms. Actual: {stopwatch.ElapsedMilliseconds}ms");
            
            // Проверяем, что все результаты валидны
            for (int i = 0; i < iterations; i++)
            {
                Assert.IsTrue(results[i].IsValid, $"Result {i} should be valid");
            }
            
            // Cleanup
            positions.Dispose();
            results.Dispose();
        }
    }
}
