using Unity.Entities;
using Unity.Burst;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Burst.Intrinsics;

namespace MudLike.Core.Optimization
{
    /// <summary>
    /// Базовый класс для универсальных систем
    /// </summary>
    /// <typeparam name="T">Тип компонента для обработки</typeparam>
    public abstract partial class GenericSystem<T> : SystemBase where T : unmanaged, IComponentData
    {
        protected EntityQuery _query;
        protected bool _useJobSystem = true;
        protected bool _useBurstCompile = true;
        
        protected override void OnCreate()
        {
            _query = GetEntityQuery(ComponentType.ReadWrite<T>());
            ConfigureSystem();
        }
        
        /// <summary>
        /// Настройка системы (переопределяется в наследниках)
        /// </summary>
        protected virtual void ConfigureSystem()
        {
            // Базовая настройка
        }
        
        protected override void OnUpdate()
        {
            if (_useJobSystem)
            {
                ProcessWithJobSystem();
            }
            else
            {
                ProcessWithForEach();
            }
        }
        
        /// <summary>
        /// Обработка с использованием Job System
        /// </summary>
        private void ProcessWithJobSystem()
        {
            var job = CreateJob();
            Dependency = job.ScheduleParallel(_query, Dependency);
        }
        
        /// <summary>
        /// Обработка с использованием ForEach
        /// </summary>
        private void ProcessWithForEach()
        {
            Entities
                .WithAll<T>()
                .ForEach((ref T component) =>
                {
                    ProcessComponent(ref component);
                }).Schedule();
        }
        
        /// <summary>
        /// Создает job для обработки (переопределяется в наследниках)
        /// </summary>
        protected abstract IJobEntity CreateJob();
        
        /// <summary>
        /// Обрабатывает компонент (переопределяется в наследниках)
        /// </summary>
        protected abstract void ProcessComponent(ref T component);
    }
    
    /// <summary>
    /// Универсальная система с Burst оптимизацией
    /// </summary>
    /// <typeparam name="T">Тип компонента</typeparam>
    public abstract partial class BurstGenericSystem<T> : GenericSystem<T> where T : unmanaged, IComponentData
    {
        protected override void ConfigureSystem()
        {
            base.ConfigureSystem();
            _useBurstCompile = true;
        }
        
        /// <summary>
        /// Создает Burst-оптимизированный job
        /// </summary>
        protected override IJobEntity CreateJob()
        {
            return new BurstGenericJob<T>
            {
                ProcessFunction = GetProcessFunctionPointer()
            };
        }
        
        /// <summary>
        /// Получает FunctionPointer для обработки компонентов
        /// Переопределяется в наследниках для предоставления конкретной реализации
        /// </summary>
        protected abstract Unity.Burst.FunctionPointer<ProcessComponentDelegate<T>> GetProcessFunctionPointer();
        
        /// <summary>
        /// Burst-оптимизированная обработка компонента
        /// </summary>
        [BurstCompile]
        protected abstract void ProcessComponentBurst(ref T component);
        
        /// <summary>
        /// Обработка компонента (делегирует к Burst версии)
        /// </summary>
        protected override void ProcessComponent(ref T component)
        {
            ProcessComponentBurst(ref component);
        }
    }
    
    /// <summary>
    /// Универсальная система с SIMD оптимизацией
    /// </summary>
    /// <typeparam name="T">Тип компонента</typeparam>
    public abstract partial class SIMDGenericSystem<T> : BurstGenericSystem<T> where T : unmanaged, IComponentData
    {
        protected override void ConfigureSystem()
        {
            base.ConfigureSystem();
            _useJobSystem = true;
        }
        
        /// <summary>
        /// SIMD-оптимизированная обработка компонента
        /// </summary>
        [BurstCompile]
        protected override void ProcessComponentBurst(ref T component)
        {
            ProcessComponentSIMD(ref component);
        }
        
        /// <summary>
        /// SIMD-оптимизированная обработка (переопределяется в наследниках)
        /// </summary>
        [BurstCompile]
        protected abstract void ProcessComponentSIMD(ref T component);
    }
    
    /// <summary>
    /// Универсальная система с кэшированием
    /// </summary>
    /// <typeparam name="T">Тип компонента</typeparam>
    public abstract partial class CachedGenericSystem<T> : SIMDGenericSystem<T> where T : unmanaged, IComponentData
    {
        private NativeArray<T> _cachedComponents;
        private bool _cacheInitialized = false;
        
        protected override void OnCreate()
        {
            base.OnCreate();
            InitializeCache();
        }
        
        protected override void OnDestroy()
        {
            if (_cacheInitialized && _cachedComponents.IsCreated)
            {
                _cachedComponents.Dispose();
            }
            base.OnDestroy();
        }
        
        /// <summary>
        /// Инициализация кэша
        /// </summary>
        private void InitializeCache()
        {
            int maxEntities = 10000; // Максимальное количество сущностей
            _cachedComponents = new NativeArray<T>(maxEntities, Allocator.Persistent);
            _cacheInitialized = true;
        }
        
        /// <summary>
        /// SIMD-оптимизированная обработка с кэшированием
        /// </summary>
        [BurstCompile]
        protected override void ProcessComponentSIMD(ref T component)
        {
            ProcessComponentCached(ref component);
        }
        
        /// <summary>
        /// Обработка с кэшированием (переопределяется в наследниках)
        /// </summary>
        [BurstCompile]
        protected abstract void ProcessComponentCached(ref T component);
    }
    
    /// <summary>
    /// Пример конкретной реализации BurstGenericSystem
    /// Показывает, как правильно использовать FunctionPointer
    /// </summary>
    /// <typeparam name="T">Тип компонента</typeparam>
    public abstract partial class ExampleBurstSystem<T> : BurstGenericSystem<T> where T : unmanaged, IComponentData
    {
        /// <summary>
        /// Статический метод для Burst-оптимизированной обработки
        /// </summary>
        [BurstCompile]
        public static void StaticProcessComponent(ref T component)
        {
            // Конкретная логика обработки компонента
            // Этот метод может быть скомпилирован в Burst
        }
        
        /// <summary>
        /// Получает FunctionPointer для статического метода
        /// </summary>
        protected override Unity.Burst.FunctionPointer<ProcessComponentDelegate<T>> GetProcessFunctionPointer()
        {
            return BurstCompiler.CompileFunctionPointer<ProcessComponentDelegate<T>>(StaticProcessComponent);
        }
        
        /// <summary>
        /// Burst-оптимизированная обработка компонента
        /// </summary>
        [BurstCompile]
        protected override void ProcessComponentBurst(ref T component)
        {
            StaticProcessComponent(ref component);
        }
    }
    
    /// <summary>
    /// Burst-оптимизированный job для универсальных систем
    /// </summary>
    /// <typeparam name="T">Тип компонента</typeparam>
    [BurstCompile]
    public partial struct BurstGenericJob<T> : IJobEntity where T : unmanaged, IComponentData
    {
        // Используем FunctionPointer для обработки компонента
        public Unity.Burst.FunctionPointer<ProcessComponentDelegate<T>> ProcessFunction;
        
        public void Execute(ref T component)
        {
            ProcessFunction.Invoke(ref component);
        }
    }
    
    /// <summary>
    /// Делегат для обработки компонентов в Burst-оптимизированных системах
    /// </summary>
    /// <typeparam name="T">Тип компонента</typeparam>
    /// <param name="component">Компонент для обработки</param>
    public delegate void ProcessComponentDelegate<T>(ref T component) where T : unmanaged, IComponentData;
    
    /// <summary>
    /// SIMD-оптимизированный job для универсальных систем
    /// </summary>
    /// <typeparam name="T">Тип компонента</typeparam>
    [BurstCompile]
    public partial struct SIMDGenericJob<T> : IJobEntity where T : unmanaged, IComponentData
    {
        public Unity.Burst.FunctionPointer<ProcessComponentDelegate<T>> ProcessFunction;
        
        public void Execute(ref T component)
        {
            ProcessFunction.Invoke(ref component);
        }
    }
    
    /// <summary>
    /// Chunk-based job для универсальных систем
    /// </summary>
    /// <typeparam name="T">Тип компонента</typeparam>
    [BurstCompile]
    public partial struct ChunkGenericJob<T> : IJobChunk where T : unmanaged, IComponentData
    {
        public ComponentTypeHandle<T> ComponentType;
        public Unity.Burst.FunctionPointer<ProcessComponentDelegate<T>> ProcessFunction;
        
        public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, 
                           bool useEnabledMask, in v128 chunkEnabledMask)
        {
            var components = chunk.GetNativeArray(ref ComponentType);
            
            for (int i = 0; i < chunk.Count; i++)
            {
                ProcessFunction.Invoke(ref components[i]);
            }
        }
    }
    
    /// <summary>
    /// Универсальная система с адаптивной производительностью
    /// </summary>
    /// <typeparam name="T">Тип компонента</typeparam>
    public abstract partial class AdaptiveGenericSystem<T> : CachedGenericSystem<T> where T : unmanaged, IComponentData
    {
        private float _performanceThreshold = 16.67f; // 60 FPS
        private int _frameCount = 0;
        private float _averageFrameTime = 0f;
        private bool _useHighPerformanceMode = true;
        
        protected override void OnUpdate()
        {
            UpdatePerformanceMetrics();
            AdaptPerformanceMode();
            
            base.OnUpdate();
        }
        
        /// <summary>
        /// Обновляет метрики производительности
        /// </summary>
        private void UpdatePerformanceMetrics()
        {
            _frameCount++;
            _averageFrameTime = (_averageFrameTime * (_frameCount - 1) + SystemAPI.Time.DeltaTime * 1000f) / _frameCount;
            
            if (_frameCount >= 60) // Обновляем каждые 60 кадров
            {
                _frameCount = 0;
            }
        }
        
        /// <summary>
        /// Адаптирует режим производительности
        /// </summary>
        private void AdaptPerformanceMode()
        {
            if (_averageFrameTime > _performanceThreshold * 1.2f)
            {
                _useHighPerformanceMode = false;
            }
            else if (_averageFrameTime < _performanceThreshold * 0.8f)
            {
                _useHighPerformanceMode = true;
            }
        }
        
        /// <summary>
        /// Обработка с адаптивной производительностью
        /// </summary>
        [BurstCompile]
        protected override void ProcessComponentCached(ref T component)
        {
            if (_useHighPerformanceMode)
            {
                ProcessComponentHighPerformance(ref component);
            }
            else
            {
                ProcessComponentLowPerformance(ref component);
            }
        }
        
        /// <summary>
        /// Высокопроизводительная обработка (переопределяется в наследниках)
        /// </summary>
        [BurstCompile]
        protected abstract void ProcessComponentHighPerformance(ref T component);
        
        /// <summary>
        /// Низкопроизводительная обработка (переопределяется в наследниках)
        /// </summary>
        [BurstCompile]
        protected abstract void ProcessComponentLowPerformance(ref T component);
    }
}