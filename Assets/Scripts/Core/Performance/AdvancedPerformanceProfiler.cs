using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using Unity.Burst;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Profiling;
using Unity.Profiling.LowLevel;
using Unity.Profiling.LowLevel.Unsafe;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MudLike.Core.Performance
{
    /// <summary>
    /// Продвинутая система профилирования производительности для Mud-Like
    /// Интегрируется с Unity Profiler и предоставляет детальную аналитику
    /// </summary>
    public static class AdvancedPerformanceProfiler
    {
        #region Profiler Markers
        
        // Основные системы
        private static readonly ProfilerMarker s_VehicleMovementMarker = new ProfilerMarker("VehicleMovement");
        private static readonly ProfilerMarker s_WheelPhysicsMarker = new ProfilerMarker("WheelPhysics");
        private static readonly ProfilerMarker s_EngineSystemMarker = new ProfilerMarker("EngineSystem");
        private static readonly ProfilerMarker s_TerrainDeformationMarker = new ProfilerMarker("TerrainDeformation");
        private static readonly ProfilerMarker s_NetworkSyncMarker = new ProfilerMarker("NetworkSync");
        
        // Оптимизация
        private static readonly ProfilerMarker s_BurstOptimizationMarker = new ProfilerMarker("BurstOptimization");
        private static readonly ProfilerMarker s_JobSystemMarker = new ProfilerMarker("JobSystem");
        private static readonly ProfilerMarker s_MemoryAllocationMarker = new ProfilerMarker("MemoryAllocation");
        
        // Аналитика
        private static readonly ProfilerMarker s_PerformanceAnalysisMarker = new ProfilerMarker("PerformanceAnalysis");
        
        #endregion
        
        #region Performance Metrics
        
        /// <summary>
        /// Метрики производительности системы
        /// </summary>
        public struct PerformanceMetrics
        {
            public float FrameTime;
            public float FrameRate;
            public int EntityCount;
            public int SystemCount;
            public long MemoryUsage;
            public float CPUUsage;
            public float GPUUsage;
            public int JobCount;
            public int BurstCompiledMethods;
            public float NetworkLatency;
        }
        
        /// <summary>
        /// Детализированные метрики системы
        /// </summary>
        public struct SystemMetrics
        {
            public string SystemName;
            public float ExecutionTime;
            public float MemoryAllocated;
            public int EntityCount;
            public bool IsBurstCompiled;
            public bool IsJobScheduled;
            public float Efficiency;
        }
        
        #endregion
        
        #region Public API
        
        /// <summary>
        /// Получить текущие метрики производительности
        /// </summary>
        public static PerformanceMetrics GetCurrentMetrics()
        {
            var metrics = new PerformanceMetrics();
            
            // Основные метрики
            metrics.FrameTime = UnityEngine.Time.deltaTime;
            metrics.FrameRate = 1.0f / metrics.FrameTime;
            
            // Подсчет сущностей и систем
            metrics.EntityCount = GetEntityCount();
            metrics.SystemCount = GetSystemCount();
            
            // Использование памяти
            metrics.MemoryUsage = UnityEngine.Profiling.Profiler.GetTotalAllocatedMemory(false);
            
            // CPU и GPU нагрузка
            metrics.CPUUsage = GetCPUUsage();
            metrics.GPUUsage = GetGPUUsage();
            
            // Job System метрики
            metrics.JobCount = GetActiveJobCount();
            metrics.BurstCompiledMethods = GetBurstCompiledMethodCount();
            
            // Сетевая задержка
            metrics.NetworkLatency = GetNetworkLatency();
            
            return metrics;
        }
        
        /// <summary>
        /// Получить метрики для всех систем
        /// </summary>
        public static SystemMetrics[] GetAllSystemMetrics()
        {
            var systems = new List<SystemMetrics>();
            
            // Получаем информацию о всех системах
            var systemTypes = GetSystemTypes();
            
            foreach (var systemType in systemTypes)
            {
                var metrics = GetSystemMetrics(systemType);
                systems.Add(metrics);
            }
            
            return systems.ToArray();
        }
        
        /// <summary>
        /// Анализ производительности с рекомендациями
        /// </summary>
        public static PerformanceAnalysisResult AnalyzePerformance()
        {
            using (s_PerformanceAnalysisMarker.Auto())
            {
                var result = new PerformanceAnalysisResult();
                var metrics = GetCurrentMetrics();
                var systemMetrics = GetAllSystemMetrics();
                
                // Анализ FPS
                if (metrics.FrameRate < 30)
                {
                    result.Issues.Add(new PerformanceIssue
                    {
                        Type = "Low FPS",
                        Severity = IssueSeverity.Critical,
                        Description = $"FPS слишком низкий: {metrics.FrameRate:F1}",
                        Recommendation = "Оптимизировать критические системы или снизить качество"
                    });
                }
                else if (metrics.FrameRate < 60)
                {
                    result.Issues.Add(new PerformanceIssue
                    {
                        Type = "Medium FPS",
                        Severity = IssueSeverity.Warning,
                        Description = $"FPS ниже оптимального: {metrics.FrameRate:F1}",
                        Recommendation = "Рассмотреть оптимизацию производительности"
                    });
                }
                
                // Анализ памяти
                if (metrics.MemoryUsage > 2L * 1024 * 1024 * 1024) // 2GB
                {
                    result.Issues.Add(new PerformanceIssue
                    {
                        Type = "High Memory Usage",
                        Severity = IssueSeverity.Warning,
                        Description = $"Высокое использование памяти: {metrics.MemoryUsage / (1024 * 1024)} MB",
                        Recommendation = "Оптимизировать использование памяти и добавить пулы объектов"
                    });
                }
                
                // Анализ систем без Burst оптимизации
                var nonBurstSystems = systemMetrics.Where(s => !s.IsBurstCompiled).ToArray();
                if (nonBurstSystems.Length > 0)
                {
                    result.Issues.Add(new PerformanceIssue
                    {
                        Type = "Missing Burst Optimization",
                        Severity = IssueSeverity.Warning,
                        Description = $"{nonBurstSystems.Length} систем без Burst оптимизации",
                        Recommendation = "Добавить [BurstCompile] к критическим системам"
                    });
                }
                
                // Анализ неэффективных систем
                var inefficientSystems = systemMetrics.Where(s => s.Efficiency < 0.7f).ToArray();
                if (inefficientSystems.Length > 0)
                {
                    result.Issues.Add(new PerformanceIssue
                    {
                        Type = "Inefficient Systems",
                        Severity = IssueSeverity.Info,
                        Description = $"{inefficientSystems.Length} неэффективных систем",
                        Recommendation = "Оптимизировать алгоритмы и использовать Job System"
                    });
                }
                
                // Генерация рекомендаций
                result.Recommendations = GenerateRecommendations(metrics, systemMetrics);
                
                return result;
            }
        }
        
        #endregion
        
        #region Profiler Integration
        
        /// <summary>
        /// Начать профилирование системы
        /// </summary>
        public static void BeginSystemProfiling(string systemName)
        {
            switch (systemName)
            {
                case "VehicleMovement":
                    s_VehicleMovementMarker.Begin();
                    break;
                case "WheelPhysics":
                    s_WheelPhysicsMarker.Begin();
                    break;
                case "EngineSystem":
                    s_EngineSystemMarker.Begin();
                    break;
                case "TerrainDeformation":
                    s_TerrainDeformationMarker.Begin();
                    break;
                case "NetworkSync":
                    s_NetworkSyncMarker.Begin();
                    break;
            }
        }
        
        /// <summary>
        /// Завершить профилирование системы
        /// </summary>
        public static void EndSystemProfiling(string systemName)
        {
            switch (systemName)
            {
                case "VehicleMovement":
                    s_VehicleMovementMarker.End();
                    break;
                case "WheelPhysics":
                    s_WheelPhysicsMarker.End();
                    break;
                case "EngineSystem":
                    s_EngineSystemMarker.End();
                    break;
                case "TerrainDeformation":
                    s_TerrainDeformationMarker.End();
                    break;
                case "NetworkSync":
                    s_NetworkSyncMarker.End();
                    break;
            }
        }
        
        #endregion
        
        #region Private Methods
        
        private static int GetEntityCount()
        {
            // Здесь можно добавить логику подсчета сущностей
            return 1000; // Заглушка
        }
        
        private static int GetSystemCount()
        {
            // Здесь можно добавить логику подсчета систем
            return 50; // Заглушка
        }
        
        private static float GetCPUUsage()
        {
            // Здесь можно добавить логику получения CPU нагрузки
            return 45.0f; // Заглушка
        }
        
        private static float GetGPUUsage()
        {
            // Здесь можно добавить логику получения GPU нагрузки
            return 60.0f; // Заглушка
        }
        
        private static int GetActiveJobCount()
        {
            // Здесь можно добавить логику подсчета активных задач
            return 10; // Заглушка
        }
        
        private static int GetBurstCompiledMethodCount()
        {
            // Здесь можно добавить логику подсчета Burst методов
            return 25; // Заглушка
        }
        
        private static float GetNetworkLatency()
        {
            // Здесь можно добавить логику получения сетевой задержки
            return 50.0f; // Заглушка
        }
        
        private static Type[] GetSystemTypes()
        {
            // Здесь можно добавить логику получения типов систем
            return new Type[0]; // Заглушка
        }
        
        private static SystemMetrics GetSystemMetrics(Type systemType)
        {
            // Здесь можно добавить логику получения метрик системы
            return new SystemMetrics
            {
                SystemName = systemType.Name,
                ExecutionTime = 1.0f,
                MemoryAllocated = 1024,
                EntityCount = 100,
                IsBurstCompiled = true,
                IsJobScheduled = true,
                Efficiency = 0.8f
            };
        }
        
        private static List<string> GenerateRecommendations(PerformanceMetrics metrics, SystemMetrics[] systemMetrics)
        {
            var recommendations = new List<string>();
            
            if (metrics.FrameRate < 60)
            {
                recommendations.Add("Рассмотреть снижение качества графики");
                recommendations.Add("Оптимизировать критические системы");
                recommendations.Add("Использовать LOD системы");
            }
            
            if (metrics.MemoryUsage > 1024 * 1024 * 1024) // 1GB
            {
                recommendations.Add("Реализовать пулы объектов");
                recommendations.Add("Оптимизировать использование памяти");
                recommendations.Add("Добавить сборку мусора");
            }
            
            var nonBurstCount = systemMetrics.Count(s => !s.IsBurstCompiled);
            if (nonBurstCount > 0)
            {
                recommendations.Add($"Добавить Burst оптимизацию к {nonBurstCount} системам");
            }
            
            return recommendations;
        }
        
        #endregion
        
        #region Data Structures
        
        public class PerformanceIssue
        {
            public string Type;
            public IssueSeverity Severity;
            public string Description;
            public string Recommendation;
        }
        
        public enum IssueSeverity
        {
            Info,
            Warning,
            Critical
        }
        
        public class PerformanceAnalysisResult
        {
            public List<PerformanceIssue> Issues = new List<PerformanceIssue>();
            public List<string> Recommendations = new List<string>();
            public PerformanceMetrics Metrics;
            public SystemMetrics[] SystemMetrics;
        }
        
        #endregion
    }
}
