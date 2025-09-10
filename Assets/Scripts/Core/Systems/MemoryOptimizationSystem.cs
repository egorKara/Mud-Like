using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using System.Collections.Generic;

namespace MudLike.Core.Systems
{
    /// <summary>
    /// Система оптимизации памяти
    /// Управляет object pooling, native collections и сборкой мусора
    /// </summary>
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class MemoryOptimizationSystem : SystemBase
    {
        // Object pools для часто создаваемых объектов
        private Dictionary<System.Type, Queue<Entity>> _entityPools;
        private Dictionary<System.Type, int> _poolSizes;
        
        // Native collections для кэширования
        private NativeArray<float3> _positionCache;
        private NativeArray<quaternion> _rotationCache;
        private NativeHashMap<int, float3> _terrainHeightCache;
        
        // Настройки памяти
        private const int DEFAULT_POOL_SIZE = 100;
        private const int MAX_CACHE_SIZE = 10000;
        private const float GC_INTERVAL = 5f; // Сборка мусора каждые 5 секунд
        private float _lastGC;
        
        // Статистика памяти
        public long TotalMemoryUsage { get; private set; }
        public int ActivePooledObjects { get; private set; }
        public int CachedPositions { get; private set; }
        
        protected override void OnCreate()
        {
            InitializePools();
            InitializeCaches();
        }
        
        protected override void OnDestroy()
        {
            CleanupPools();
            CleanupCaches();
        }
        
        protected override void OnUpdate()
        {
            UpdateMemoryStats();
            ManageGarbageCollection();
            OptimizeNativeCollections();
        }
        
        private void InitializePools()
        {
            _entityPools = new Dictionary<System.Type, Queue<Entity>>();
            _poolSizes = new Dictionary<System.Type, int>();
            
            // Инициализируем пулы для разных типов объектов
            InitializePool<LocalTransform>(DEFAULT_POOL_SIZE);
            InitializePool<RenderBounds>(DEFAULT_POOL_SIZE);
            // Добавляем другие типы по необходимости
        }
        
        private void InitializePool<T>(int size) where T : unmanaged, IComponentData
        {
            var pool = new Queue<Entity>();
            _entityPools[typeof(T)] = pool;
            _poolSizes[typeof(T)] = size;
        }
        
        private void InitializeCaches()
        {
            _positionCache = new NativeArray<float3>(MAX_CACHE_SIZE, Allocator.Persistent);
            _rotationCache = new NativeArray<quaternion>(MAX_CACHE_SIZE, Allocator.Persistent);
            _terrainHeightCache = new NativeHashMap<int, float3>(MAX_CACHE_SIZE, Allocator.Persistent);
        }
        
        private void CleanupPools()
        {
            foreach (var pool in _entityPools.Values)
            {
                while (pool.Count > 0)
                {
                    var entity = pool.Dequeue();
                    if (Exists(entity))
                    {
                        EntityManager.DestroyEntity(entity);
                    }
                }
            }
            _entityPools.Clear();
            _poolSizes.Clear();
        }
        
        private void CleanupCaches()
        {
            if (_positionCache.IsCreated)
                _positionCache.Dispose();
            if (_rotationCache.IsCreated)
                _rotationCache.Dispose();
            if (_terrainHeightCache.IsCreated)
                _terrainHeightCache.Dispose();
        }
        
        private void UpdateMemoryStats()
        {
            TotalMemoryUsage = System.GC.GetTotalMemory(false);
            ActivePooledObjects = CountActivePooledObjects();
            CachedPositions = _positionCache.Length;
        }
        
        private int CountActivePooledObjects()
        {
            int count = 0;
            foreach (var pool in _entityPools.Values)
            {
                count += pool.Count;
            }
            return count;
        }
        
        private void ManageGarbageCollection()
        {
            float currentTime = Time.time;
            
            // Периодическая сборка мусора
            if (currentTime - _lastGC >= GC_INTERVAL)
            {
                // Принудительная сборка мусора только при необходимости
                if (TotalMemoryUsage > 100 * 1024 * 1024) // 100MB
                {
                    System.GC.Collect();
                    Debug.Log($"[Memory] Принудительная сборка мусора. Память: {TotalMemoryUsage / (1024 * 1024)}MB");
                }
                
                _lastGC = currentTime;
            }
        }
        
        private void OptimizeNativeCollections()
        {
            // Оптимизируем размеры native collections
            if (_positionCache.Length > MAX_CACHE_SIZE * 2)
            {
                ResizeNativeArray(ref _positionCache, MAX_CACHE_SIZE);
            }
            
            if (_rotationCache.Length > MAX_CACHE_SIZE * 2)
            {
                ResizeNativeArray(ref _rotationCache, MAX_CACHE_SIZE);
            }
            
            // Очищаем старые записи из кэша
            if (_terrainHeightCache.Count() > MAX_CACHE_SIZE)
            {
                ClearOldCacheEntries();
            }
        }
        
        private void ResizeNativeArray<T>(ref NativeArray<T> array, int newSize) where T : unmanaged
        {
            var newArray = new NativeArray<T>(newSize, Allocator.Persistent);
            int copySize = math.min(array.Length, newSize);
            
            for (int i = 0; i < copySize; i++)
            {
                newArray[i] = array[i];
            }
            
            array.Dispose();
            array = newArray;
        }
        
        private void ClearOldCacheEntries()
        {
            var keysToRemove = new List<int>();
            var enumerator = _terrainHeightCache.GetKeyEnumerator();
            
            int count = 0;
            while (enumerator.MoveNext() && count < MAX_CACHE_SIZE / 2)
            {
                keysToRemove.Add(enumerator.Current);
                count++;
            }
            
            foreach (var key in keysToRemove)
            {
                _terrainHeightCache.Remove(key);
            }
        }
        
        /// <summary>
        /// Получить объект из пула
        /// </summary>
        public Entity GetFromPool<T>() where T : unmanaged, IComponentData
        {
            if (!_entityPools.ContainsKey(typeof(T)))
            {
                InitializePool<T>(DEFAULT_POOL_SIZE);
            }
            
            var pool = _entityPools[typeof(T)];
            
            if (pool.Count > 0)
            {
                return pool.Dequeue();
            }
            
            // Создаем новый объект если пул пуст
            return CreateNewEntity<T>();
        }
        
        /// <summary>
        /// Вернуть объект в пул
        /// </summary>
        public void ReturnToPool<T>(Entity entity) where T : unmanaged, IComponentData
        {
            if (!_entityPools.ContainsKey(typeof(T)))
            {
                InitializePool<T>(DEFAULT_POOL_SIZE);
            }
            
            var pool = _entityPools[typeof(T)];
            
            if (pool.Count < _poolSizes[typeof(T)])
            {
                pool.Enqueue(entity);
            }
            else
            {
                // Уничтожаем объект если пул переполнен
                if (Exists(entity))
                {
                    EntityManager.DestroyEntity(entity);
                }
            }
        }
        
        private Entity CreateNewEntity<T>() where T : unmanaged, IComponentData
        {
            var entity = EntityManager.CreateEntity();
            EntityManager.AddComponent<T>(entity);
            return entity;
        }
        
        /// <summary>
        /// Кэшировать позицию
        /// </summary>
        public void CachePosition(int index, float3 position)
        {
            if (index >= 0 && index < _positionCache.Length)
            {
                _positionCache[index] = position;
            }
        }
        
        /// <summary>
        /// Получить кэшированную позицию
        /// </summary>
        public float3 GetCachedPosition(int index)
        {
            if (index >= 0 && index < _positionCache.Length)
            {
                return _positionCache[index];
            }
            return float3.zero;
        }
        
        /// <summary>
        /// Кэшировать высоту terrain
        /// </summary>
        public void CacheTerrainHeight(int2 coordinates, float3 height)
        {
            int key = coordinates.x * 1000 + coordinates.y; // Простой hash
            _terrainHeightCache[key] = height;
        }
        
        /// <summary>
        /// Получить кэшированную высоту terrain
        /// </summary>
        public bool TryGetCachedTerrainHeight(int2 coordinates, out float3 height)
        {
            int key = coordinates.x * 1000 + coordinates.y;
            return _terrainHeightCache.TryGetValue(key, out height);
        }
        
        /// <summary>
        /// Очистить все кэши
        /// </summary>
        public void ClearAllCaches()
        {
            _terrainHeightCache.Clear();
            
            // Очищаем массивы
            for (int i = 0; i < _positionCache.Length; i++)
            {
                _positionCache[i] = float3.zero;
            }
            
            for (int i = 0; i < _rotationCache.Length; i++)
            {
                _rotationCache[i] = quaternion.identity;
            }
        }
    }
}