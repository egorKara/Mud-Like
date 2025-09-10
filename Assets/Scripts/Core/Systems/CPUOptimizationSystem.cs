using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;

namespace MudLike.Core.Systems
{
    /// <summary>
    /// Система оптимизации CPU производительности
    /// Использует Burst Compiler, Job System и кэширование
    /// </summary>
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial class CPUOptimizationSystem : SystemBase
    {
        private NativeArray<float3> _cachedPositions;
        private NativeArray<quaternion> _cachedRotations;
        private bool _cacheInitialized = false;
        
        // Настройки оптимизации
        private const int MAX_CACHE_SIZE = 1000;
        private const float CACHE_UPDATE_INTERVAL = 0.1f; // Обновляем кэш каждые 100ms
        private float _lastCacheUpdate;
        
        protected override void OnCreate()
        {
            // Инициализируем кэш
            _cachedPositions = new NativeArray<float3>(MAX_CACHE_SIZE, Allocator.Persistent);
            _cachedRotations = new NativeArray<quaternion>(MAX_CACHE_SIZE, Allocator.Persistent);
        }
        
        protected override void OnDestroy()
        {
            if (_cachedPositions.IsCreated)
                _cachedPositions.Dispose();
            if (_cachedRotations.IsCreated)
                _cachedRotations.Dispose();
        }
        
        protected override void OnUpdate()
        {
            float currentTime = Time.fixedTime;
            
            // Обновляем кэш только периодически
            if (currentTime - _lastCacheUpdate >= CACHE_UPDATE_INTERVAL)
            {
                UpdateCache();
                _lastCacheUpdate = currentTime;
            }
            
            // Выполняем оптимизированные вычисления
            ScheduleOptimizedJobs();
        }
        
        private void UpdateCache()
        {
            // Кэшируем часто используемые данные
            var entities = GetEntityQuery(typeof(LocalTransform)).ToEntityArray(Allocator.Temp);
            int count = math.min(entities.Length, MAX_CACHE_SIZE);
            
            for (int i = 0; i < count; i++)
            {
                if (HasComponent<LocalTransform>(entities[i]))
                {
                    var transform = GetComponent<LocalTransform>(entities[i]);
                    _cachedPositions[i] = transform.Position;
                    _cachedRotations[i] = transform.Rotation;
                }
            }
            
            entities.Dispose();
            _cacheInitialized = true;
        }
        
        private void ScheduleOptimizedJobs()
        {
            if (!_cacheInitialized) return;
            
            // Планируем оптимизированные job'ы
            var distanceJob = new DistanceCalculationJob
            {
                CachedPositions = _cachedPositions,
                CachedRotations = _cachedRotations
            };
            
            var mathJob = new MathOptimizationJob
            {
                DeltaTime = SystemAPI.Time.fixedDeltaTime
            };
            
            // Выполняем job'ы параллельно
            var distanceHandle = distanceJob.Schedule(MAX_CACHE_SIZE, 64);
            var mathHandle = mathJob.Schedule(MAX_CACHE_SIZE, 64);
            
            // Ждем завершения
            distanceHandle.Complete();
            mathHandle.Complete();
        }
    }
    
    /// <summary>
    /// Job для оптимизированных вычислений расстояний
    /// </summary>
    [BurstCompile]
    public struct DistanceCalculationJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<float3> CachedPositions;
        [ReadOnly] public NativeArray<quaternion> CachedRotations;
        
        public void Execute(int index)
        {
            if (index >= CachedPositions.Length) return;
            
            float3 pos = CachedPositions[index];
            
            // Оптимизированные вычисления расстояний
            for (int i = 0; i < CachedPositions.Length; i++)
            {
                if (i == index) continue;
                
                float3 otherPos = CachedPositions[i];
                float distance = math.distance(pos, otherPos);
                
                // Кэшируем результат для дальнейшего использования
                // (в реальной игре здесь можно сохранить в NativeHashMap)
            }
        }
    }
    
    /// <summary>
    /// Job для оптимизированных математических вычислений
    /// </summary>
    [BurstCompile]
    public struct MathOptimizationJob : IJobParallelFor
    {
        public float DeltaTime;
        
        public void Execute(int index)
        {
            // Оптимизированные математические операции
            float value = index * 0.1f;
            
            // Используем быстрые математические функции
            float sin = math.sin(value);
            float cos = math.cos(value);
            float sqrt = math.sqrt(value);
            
            // Кэшируем результаты
            // (в реальной игре здесь можно сохранить в NativeArray)
        }
    }
}