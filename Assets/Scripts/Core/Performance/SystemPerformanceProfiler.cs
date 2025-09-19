using Unity.Entities;
using Unity.Collections;
using Unity.Jobs;
using Unity.Burst;
using Unity.Mathematics;
using System.Collections.Generic;
using UnityEngine;

namespace MudLike.Core.Performance
{
    /// <summary>
    /// Система профилирования производительности ECS систем
    /// Отслеживает время выполнения и оптимизирует производительность
    /// </summary>
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class SystemPerformanceProfiler : SystemBase
    {
        private Dictionary<string, float> _systemExecutionTimes;
        private Dictionary<string, int> _entityCounts;
        private Dictionary<string, bool> _burstCompiledSystems;
        
        protected override void OnCreate()
        {
            _systemExecutionTimes = new Dictionary<string, float>();
            _entityCounts = new Dictionary<string, int>();
            _burstCompiledSystems = new Dictionary<string, bool>();
            
            // Включаем профилирование только в Development сборке
            #if UNITY_EDITOR || DEVELOPMENT_BUILD
            Debug.Log("[SystemPerformanceProfiler] Профилирование производительности активировано");
            #endif
        }
        
        /// <summary>
        /// Записывает время выполнения системы
        /// </summary>
        public void RecordSystemExecution(string systemName, float executionTime, int entityCount, bool isBurstCompiled)
        {
            _systemExecutionTimes[systemName] = executionTime;
            _entityCounts[systemName] = entityCount;
            _burstCompiledSystems[systemName] = isBurstCompiled;
            
            // Предупреждаем о медленных системах
            if (executionTime > 16.67f) // Более 1 кадра при 60 FPS
            {
                Debug.LogWarning($"[SystemPerformanceProfiler] Медленная система: {systemName} ({executionTime:F2}ms)");
            }
        }
        
        /// <summary>
        /// Получает отчет о производительности
        /// </summary>
        public PerformanceReport GetPerformanceReport()
        {
            var report = PerformanceReport.Create();
            
            foreach (var kvp in _systemExecutionTimes)
            {
                var systemName = kvp.Key;
                var executionTime = kvp.Value;
                var entityCount = _entityCounts[systemName];
                var isBurstCompiled = _burstCompiledSystems[systemName];
                
                report.SystemData.Add(new SystemPerformanceData
                {
                    SystemName = systemName,
                    ExecutionTime = executionTime,
                    EntityCount = entityCount,
                    IsBurstCompiled = isBurstCompiled,
                    PerformanceScore = CalculatePerformanceScore(executionTime, entityCount, isBurstCompiled)
                });
            }
            
            return report;
        }
        
        /// <summary>
        /// Вычисляет оценку производительности системы
        /// </summary>
        private float CalculatePerformanceScore(float executionTime, int entityCount, bool isBurstCompiled)
        {
            float baseScore = 100f;
            
            // Штраф за время выполнения
            if (executionTime > 16.67f) baseScore -= 50f;
            else if (executionTime > 8.33f) baseScore -= 25f;
            else if (executionTime > 4.17f) baseScore -= 10f;
            
            // Бонус за Burst Compilation
            if (isBurstCompiled) baseScore += 20f;
            
            // Штраф за большое количество сущностей без Burst
            if (entityCount > 1000 && !isBurstCompiled) baseScore -= 30f;
            
            return math.max(0f, baseScore);
        }
        
        protected override void OnUpdate()
        {
            // Обновляем профилирование каждые 60 кадров
            if (SystemAPI.Time.ElapsedTime % 1.0f < SystemAPI.Time.DeltaTime)
            {
                LogPerformanceSummary();
            }
        }
        
        /// <summary>
        /// Выводит сводку производительности
        /// </summary>
        private void LogPerformanceSummary()
        {
            var report = GetPerformanceReport();
            
            Debug.Log($"[SystemPerformanceProfiler] === СВОДКА ПРОИЗВОДИТЕЛЬНОСТИ ===");
            Debug.Log($"[SystemPerformanceProfiler] Всего систем: {report.SystemData.Count}");
            
            foreach (var systemData in report.SystemData)
            {
                Debug.Log($"[SystemPerformanceProfiler] {systemData.SystemName}: " +
                         $"{systemData.ExecutionTime:F2}ms, " +
                         $"{systemData.EntityCount} entities, " +
                         $"Burst: {systemData.IsBurstCompiled}, " +
                         $"Score: {systemData.PerformanceScore:F1}");
            }
        }
    }
    
    /// <summary>
    /// Отчет о производительности
    /// </summary>
    public struct PerformanceReport
    {
        public List<SystemPerformanceData> SystemData;
        
        public static PerformanceReport Create()
        {
            return new PerformanceReport
            {
                SystemData = new List<SystemPerformanceData>()
            };
        }
    }
    
    /// <summary>
    /// Данные производительности системы
    /// </summary>
    public struct SystemPerformanceData
    {
        public string SystemName;
        public float ExecutionTime;
        public int EntityCount;
        public bool IsBurstCompiled;
        public float PerformanceScore;
    }
}
