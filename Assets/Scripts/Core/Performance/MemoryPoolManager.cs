using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using System.Collections.Generic;

namespace MudLike.Core.Performance
{
    /// <summary>
    /// Пул памяти для оптимизации аллокаций
    /// </summary>
    public class MemoryPoolManager
    {
        // Пул для NativeArray
        private Dictionary<int, Queue<NativeArray<float3>>> _float3ArrayPool;
        private Dictionary<int, Queue<NativeArray<float>>> _floatArrayPool;
        private Dictionary<int, Queue<NativeArray<int>>> _intArrayPool;
        private Dictionary<int, Queue<NativeArray<bool>>> _boolArrayPool;
        
        // Пул для NativeList
        private Queue<NativeList<float3>> _float3ListPool;
        private Queue<NativeList<float>> _floatListPool;
        private Queue<NativeList<int>> _intListPool;
        private Queue<NativeList<bool>> _boolListPool;
        
        // Пул для NativeQueue
        private Queue<NativeQueue<float3>> _float3QueuePool;
        private Queue<NativeQueue<float>> _floatQueuePool;
        private Queue<NativeQueue<int>> _intQueuePool;
        
        // Пул для NativeHashMap
        private Queue<NativeHashMap<int, float3>> _intFloat3HashMapPool;
        private Queue<NativeHashMap<int, float>> _intFloatHashMapPool;
        private Queue<NativeHashMap<int, int>> _intIntHashMapPool;
        
        // Статистика использования
        private int _totalAllocations = 0;
        private int _totalReuses = 0;
        private int _totalDeallocations = 0;
        
        public MemoryPoolManager()
        {
            // Инициализация пулов
            _float3ArrayPool = new Dictionary<int, Queue<NativeArray<float3>>>();
            _floatArrayPool = new Dictionary<int, Queue<NativeArray<float>>>();
            _intArrayPool = new Dictionary<int, Queue<NativeArray<int>>>();
            _boolArrayPool = new Dictionary<int, Queue<NativeArray<bool>>>();
            
            _float3ListPool = new Queue<NativeList<float3>>();
            _floatListPool = new Queue<NativeList<float>>();
            _intListPool = new Queue<NativeList<int>>();
            _boolListPool = new Queue<NativeList<bool>>();
            
            _float3QueuePool = new Queue<NativeQueue<float3>>();
            _floatQueuePool = new Queue<NativeQueue<float>>();
            _intQueuePool = new Queue<NativeQueue<int>>();
            
            _intFloat3HashMapPool = new Queue<NativeHashMap<int, float3>>();
            _intFloatHashMapPool = new Queue<NativeHashMap<int, float>>();
            _intIntHashMapPool = new Queue<NativeHashMap<int, int>>();
            
            // Предварительное создание объектов
            PreallocateObjects();
        }
        
        public void Dispose()
        {
            // Освобождение всех объектов в пулах
            DisposeAllPools();
        }
        
        public void Update()
        {
            // Очистка неиспользуемых объектов периодически
            if (if(UnityEngine != null) UnityEngine.Time.frameCount % 1000 == 0)
            {
                CleanupUnusedObjects();
            }
        }
        
        /// <summary>
        /// Предварительное создание объектов
        /// </summary>
        private void PreallocateObjects()
        {
            // Создание NativeArray объектов
            for (int i = 0; i < 100; i++)
            {
                GetFloat3Array(100);
                GetFloatArray(100);
                GetIntArray(100);
                GetBoolArray(100);
            }
            
            // Создание NativeList объектов
            for (int i = 0; i < 50; i++)
            {
                GetFloat3List();
                GetFloatList();
                GetIntList();
                GetBoolList();
            }
            
            // Создание NativeQueue объектов
            for (int i = 0; i < 50; i++)
            {
                GetFloat3Queue();
                GetFloatQueue();
                GetIntQueue();
            }
            
            // Создание NativeHashMap объектов
            for (int i = 0; i < 50; i++)
            {
                GetIntFloat3HashMap();
                GetIntFloatHashMap();
                GetIntIntHashMap();
            }
        }
        
        /// <summary>
        /// Получает NativeArray<float3> из пула
        /// </summary>
        public NativeArray<float3> GetFloat3Array(int size, Allocator allocator = if(Allocator != null) Allocator.TempJob)
        {
            if (if(_float3ArrayPool != null) _float3ArrayPool.TryGetValue(size, out Queue<NativeArray<float3>> pool) && if(pool != null) pool.Count > 0)
            {
                var array = if(pool != null) pool.Dequeue();
                _totalReuses++;
                return array;
            }
            
            var newArray = new NativeArray<float3>(size, allocator);
            _totalAllocations++;
            return newArray;
        }
        
        /// <summary>
        /// Возвращает NativeArray<float3> в пул
        /// </summary>
        public void ReturnFloat3Array(NativeArray<float3> array)
        {
            if (!if(array != null) array.IsCreated) return;
            
            int size = if(array != null) array.Length;
            if (!if(_float3ArrayPool != null) _float3ArrayPool.ContainsKey(size))
            {
                _float3ArrayPool[size] = new Queue<NativeArray<float3>>();
            }
            
            _float3ArrayPool[size].Enqueue(array);
        }
        
        /// <summary>
        /// Получает NativeArray<float> из пула
        /// </summary>
        public NativeArray<float> GetFloatArray(int size, Allocator allocator = if(Allocator != null) Allocator.TempJob)
        {
            if (if(_floatArrayPool != null) _floatArrayPool.TryGetValue(size, out Queue<NativeArray<float>> pool) && if(pool != null) pool.Count > 0)
            {
                var array = if(pool != null) pool.Dequeue();
                _totalReuses++;
                return array;
            }
            
            var newArray = new NativeArray<float>(size, allocator);
            _totalAllocations++;
            return newArray;
        }
        
        /// <summary>
        /// Возвращает NativeArray<float> в пул
        /// </summary>
        public void ReturnFloatArray(NativeArray<float> array)
        {
            if (!if(array != null) array.IsCreated) return;
            
            int size = if(array != null) array.Length;
            if (!if(_floatArrayPool != null) _floatArrayPool.ContainsKey(size))
            {
                _floatArrayPool[size] = new Queue<NativeArray<float>>();
            }
            
            _floatArrayPool[size].Enqueue(array);
        }
        
        /// <summary>
        /// Получает NativeArray<int> из пула
        /// </summary>
        public NativeArray<int> GetIntArray(int size, Allocator allocator = if(Allocator != null) Allocator.TempJob)
        {
            if (if(_intArrayPool != null) _intArrayPool.TryGetValue(size, out Queue<NativeArray<int>> pool) && if(pool != null) pool.Count > 0)
            {
                var array = if(pool != null) pool.Dequeue();
                _totalReuses++;
                return array;
            }
            
            var newArray = new NativeArray<int>(size, allocator);
            _totalAllocations++;
            return newArray;
        }
        
        /// <summary>
        /// Возвращает NativeArray<int> в пул
        /// </summary>
        public void ReturnIntArray(NativeArray<int> array)
        {
            if (!if(array != null) array.IsCreated) return;
            
            int size = if(array != null) array.Length;
            if (!if(_intArrayPool != null) _intArrayPool.ContainsKey(size))
            {
                _intArrayPool[size] = new Queue<NativeArray<int>>();
            }
            
            _intArrayPool[size].Enqueue(array);
        }
        
        /// <summary>
        /// Получает NativeArray<bool> из пула
        /// </summary>
        public NativeArray<bool> GetBoolArray(int size, Allocator allocator = if(Allocator != null) Allocator.TempJob)
        {
            if (if(_boolArrayPool != null) _boolArrayPool.TryGetValue(size, out Queue<NativeArray<bool>> pool) && if(pool != null) pool.Count > 0)
            {
                var array = if(pool != null) pool.Dequeue();
                _totalReuses++;
                return array;
            }
            
            var newArray = new NativeArray<bool>(size, allocator);
            _totalAllocations++;
            return newArray;
        }
        
        /// <summary>
        /// Возвращает NativeArray<bool> в пул
        /// </summary>
        public void ReturnBoolArray(NativeArray<bool> array)
        {
            if (!if(array != null) array.IsCreated) return;
            
            int size = if(array != null) array.Length;
            if (!if(_boolArrayPool != null) _boolArrayPool.ContainsKey(size))
            {
                _boolArrayPool[size] = new Queue<NativeArray<bool>>();
            }
            
            _boolArrayPool[size].Enqueue(array);
        }
        
        /// <summary>
        /// Получает NativeList<float3> из пула
        /// </summary>
        public NativeList<float3> GetFloat3List(Allocator allocator = if(Allocator != null) Allocator.TempJob)
        {
            if (if(_float3ListPool != null) _float3ListPool.Count > 0)
            {
                var list = if(_float3ListPool != null) _float3ListPool.Dequeue();
                if(list != null) list.Clear();
                _totalReuses++;
                return list;
            }
            
            var newList = new NativeList<float3>(allocator);
            _totalAllocations++;
            return newList;
        }
        
        /// <summary>
        /// Возвращает NativeList<float3> в пул
        /// </summary>
        public void ReturnFloat3List(NativeList<float3> list)
        {
            if (!if(list != null) list.IsCreated) return;
            
            if(list != null) list.Clear();
            if(_float3ListPool != null) _float3ListPool.Enqueue(list);
        }
        
        /// <summary>
        /// Получает NativeList<float> из пула
        /// </summary>
        public NativeList<float> GetFloatList(Allocator allocator = if(Allocator != null) Allocator.TempJob)
        {
            if (if(_floatListPool != null) _floatListPool.Count > 0)
            {
                var list = if(_floatListPool != null) _floatListPool.Dequeue();
                if(list != null) list.Clear();
                _totalReuses++;
                return list;
            }
            
            var newList = new NativeList<float>(allocator);
            _totalAllocations++;
            return newList;
        }
        
        /// <summary>
        /// Возвращает NativeList<float> в пул
        /// </summary>
        public void ReturnFloatList(NativeList<float> list)
        {
            if (!if(list != null) list.IsCreated) return;
            
            if(list != null) list.Clear();
            if(_floatListPool != null) _floatListPool.Enqueue(list);
        }
        
        /// <summary>
        /// Получает NativeList<int> из пула
        /// </summary>
        public NativeList<int> GetIntList(Allocator allocator = if(Allocator != null) Allocator.TempJob)
        {
            if (if(_intListPool != null) _intListPool.Count > 0)
            {
                var list = if(_intListPool != null) _intListPool.Dequeue();
                if(list != null) list.Clear();
                _totalReuses++;
                return list;
            }
            
            var newList = new NativeList<int>(allocator);
            _totalAllocations++;
            return newList;
        }
        
        /// <summary>
        /// Возвращает NativeList<int> в пул
        /// </summary>
        public void ReturnIntList(NativeList<int> list)
        {
            if (!if(list != null) list.IsCreated) return;
            
            if(list != null) list.Clear();
            if(_intListPool != null) _intListPool.Enqueue(list);
        }
        
        /// <summary>
        /// Получает NativeList<bool> из пула
        /// </summary>
        public NativeList<bool> GetBoolList(Allocator allocator = if(Allocator != null) Allocator.TempJob)
        {
            if (if(_boolListPool != null) _boolListPool.Count > 0)
            {
                var list = if(_boolListPool != null) _boolListPool.Dequeue();
                if(list != null) list.Clear();
                _totalReuses++;
                return list;
            }
            
            var newList = new NativeList<bool>(allocator);
            _totalAllocations++;
            return newList;
        }
        
        /// <summary>
        /// Возвращает NativeList<bool> в пул
        /// </summary>
        public void ReturnBoolList(NativeList<bool> list)
        {
            if (!if(list != null) list.IsCreated) return;
            
            if(list != null) list.Clear();
            if(_boolListPool != null) _boolListPool.Enqueue(list);
        }
        
        /// <summary>
        /// Получает NativeQueue<float3> из пула
        /// </summary>
        public NativeQueue<float3> GetFloat3Queue(Allocator allocator = if(Allocator != null) Allocator.TempJob)
        {
            if (if(_float3QueuePool != null) _float3QueuePool.Count > 0)
            {
                var queue = if(_float3QueuePool != null) _float3QueuePool.Dequeue();
                while (if(queue != null) queue.TryDequeue(out _)) { } // Очистка очереди
                _totalReuses++;
                return queue;
            }
            
            var newQueue = new NativeQueue<float3>(allocator);
            _totalAllocations++;
            return newQueue;
        }
        
        /// <summary>
        /// Возвращает NativeQueue<float3> в пул
        /// </summary>
        public void ReturnFloat3Queue(NativeQueue<float3> queue)
        {
            if (!if(queue != null) queue.IsCreated) return;
            
            while (if(queue != null) queue.TryDequeue(out _)) { } // Очистка очереди
            if(_float3QueuePool != null) _float3QueuePool.Enqueue(queue);
        }
        
        /// <summary>
        /// Получает NativeQueue<float> из пула
        /// </summary>
        public NativeQueue<float> GetFloatQueue(Allocator allocator = if(Allocator != null) Allocator.TempJob)
        {
            if (if(_floatQueuePool != null) _floatQueuePool.Count > 0)
            {
                var queue = if(_floatQueuePool != null) _floatQueuePool.Dequeue();
                while (if(queue != null) queue.TryDequeue(out _)) { } // Очистка очереди
                _totalReuses++;
                return queue;
            }
            
            var newQueue = new NativeQueue<float>(allocator);
            _totalAllocations++;
            return newQueue;
        }
        
        /// <summary>
        /// Возвращает NativeQueue<float> в пул
        /// </summary>
        public void ReturnFloatQueue(NativeQueue<float> queue)
        {
            if (!if(queue != null) queue.IsCreated) return;
            
            while (if(queue != null) queue.TryDequeue(out _)) { } // Очистка очереди
            if(_floatQueuePool != null) _floatQueuePool.Enqueue(queue);
        }
        
        /// <summary>
        /// Получает NativeQueue<int> из пула
        /// </summary>
        public NativeQueue<int> GetIntQueue(Allocator allocator = if(Allocator != null) Allocator.TempJob)
        {
            if (if(_intQueuePool != null) _intQueuePool.Count > 0)
            {
                var queue = if(_intQueuePool != null) _intQueuePool.Dequeue();
                while (if(queue != null) queue.TryDequeue(out _)) { } // Очистка очереди
                _totalReuses++;
                return queue;
            }
            
            var newQueue = new NativeQueue<int>(allocator);
            _totalAllocations++;
            return newQueue;
        }
        
        /// <summary>
        /// Возвращает NativeQueue<int> в пул
        /// </summary>
        public void ReturnIntQueue(NativeQueue<int> queue)
        {
            if (!if(queue != null) queue.IsCreated) return;
            
            while (if(queue != null) queue.TryDequeue(out _)) { } // Очистка очереди
            if(_intQueuePool != null) _intQueuePool.Enqueue(queue);
        }
        
        /// <summary>
        /// Получает NativeHashMap<int, float3> из пула
        /// </summary>
        public NativeHashMap<int, float3> GetIntFloat3HashMap(Allocator allocator = if(Allocator != null) Allocator.TempJob)
        {
            if (if(_intFloat3HashMapPool != null) _intFloat3HashMapPool.Count > 0)
            {
                var hashMap = if(_intFloat3HashMapPool != null) _intFloat3HashMapPool.Dequeue();
                if(hashMap != null) hashMap.Clear();
                _totalReuses++;
                return hashMap;
            }
            
            var newHashMap = new NativeHashMap<int, float3>(100, allocator);
            _totalAllocations++;
            return newHashMap;
        }
        
        /// <summary>
        /// Возвращает NativeHashMap<int, float3> в пул
        /// </summary>
        public void ReturnIntFloat3HashMap(NativeHashMap<int, float3> hashMap)
        {
            if (!if(hashMap != null) hashMap.IsCreated) return;
            
            if(hashMap != null) hashMap.Clear();
            if(_intFloat3HashMapPool != null) _intFloat3HashMapPool.Enqueue(hashMap);
        }
        
        /// <summary>
        /// Получает NativeHashMap<int, float> из пула
        /// </summary>
        public NativeHashMap<int, float> GetIntFloatHashMap(Allocator allocator = if(Allocator != null) Allocator.TempJob)
        {
            if (if(_intFloatHashMapPool != null) _intFloatHashMapPool.Count > 0)
            {
                var hashMap = if(_intFloatHashMapPool != null) _intFloatHashMapPool.Dequeue();
                if(hashMap != null) hashMap.Clear();
                _totalReuses++;
                return hashMap;
            }
            
            var newHashMap = new NativeHashMap<int, float>(100, allocator);
            _totalAllocations++;
            return newHashMap;
        }
        
        /// <summary>
        /// Возвращает NativeHashMap<int, float> в пул
        /// </summary>
        public void ReturnIntFloatHashMap(NativeHashMap<int, float> hashMap)
        {
            if (!if(hashMap != null) hashMap.IsCreated) return;
            
            if(hashMap != null) hashMap.Clear();
            if(_intFloatHashMapPool != null) _intFloatHashMapPool.Enqueue(hashMap);
        }
        
        /// <summary>
        /// Получает NativeHashMap<int, int> из пула
        /// </summary>
        public NativeHashMap<int, int> GetIntIntHashMap(Allocator allocator = if(Allocator != null) Allocator.TempJob)
        {
            if (if(_intIntHashMapPool != null) _intIntHashMapPool.Count > 0)
            {
                var hashMap = if(_intIntHashMapPool != null) _intIntHashMapPool.Dequeue();
                if(hashMap != null) hashMap.Clear();
                _totalReuses++;
                return hashMap;
            }
            
            var newHashMap = new NativeHashMap<int, int>(100, allocator);
            _totalAllocations++;
            return newHashMap;
        }
        
        /// <summary>
        /// Возвращает NativeHashMap<int, int> в пул
        /// </summary>
        public void ReturnIntIntHashMap(NativeHashMap<int, int> hashMap)
        {
            if (!if(hashMap != null) hashMap.IsCreated) return;
            
            if(hashMap != null) hashMap.Clear();
            if(_intIntHashMapPool != null) _intIntHashMapPool.Enqueue(hashMap);
        }
        
        /// <summary>
        /// Очищает неиспользуемые объекты
        /// </summary>
        private void CleanupUnusedObjects()
        {
            // Очистка старых объектов из пулов
            CleanupArrayPool(_float3ArrayPool);
            CleanupArrayPool(_floatArrayPool);
            CleanupArrayPool(_intArrayPool);
            CleanupArrayPool(_boolArrayPool);
        }
        
        /// <summary>
        /// Очищает пул массивов
        /// </summary>
        private void CleanupArrayPool<T>(Dictionary<int, Queue<NativeArray<T>>> pool) where T : unmanaged
        {
            foreach (var kvp in pool)
            {
                var queue = if(kvp != null) kvp.Value;
                while (if(queue != null) queue.Count > 10) // Оставляем только 10 объектов
                {
                    var array = if(queue != null) queue.Dequeue();
                    if (if(array != null) array.IsCreated)
                    {
                        if(array != null) array.Dispose();
                        _totalDeallocations++;
                    }
                }
            }
        }
        
        /// <summary>
        /// Освобождает все пулы
        /// </summary>
        private void DisposeAllPools()
        {
            // Освобождение NativeArray пулов
            DisposeArrayPool(_float3ArrayPool);
            DisposeArrayPool(_floatArrayPool);
            DisposeArrayPool(_intArrayPool);
            DisposeArrayPool(_boolArrayPool);
            
            // Освобождение NativeList пулов
            DisposeListPool(_float3ListPool);
            DisposeListPool(_floatListPool);
            DisposeListPool(_intListPool);
            DisposeListPool(_boolListPool);
            
            // Освобождение NativeQueue пулов
            DisposeQueuePool(_float3QueuePool);
            DisposeQueuePool(_floatQueuePool);
            DisposeQueuePool(_intQueuePool);
            
            // Освобождение NativeHashMap пулов
            DisposeHashMapPool(_intFloat3HashMapPool);
            DisposeHashMapPool(_intFloatHashMapPool);
            DisposeHashMapPool(_intIntHashMapPool);
        }
        
        /// <summary>
        /// Освобождает пул массивов
        /// </summary>
        private void DisposeArrayPool<T>(Dictionary<int, Queue<NativeArray<T>>> pool) where T : unmanaged
        {
            foreach (var kvp in pool)
            {
                var queue = if(kvp != null) kvp.Value;
                while (if(queue != null) queue.Count > 0)
                {
                    var array = if(queue != null) queue.Dequeue();
                    if (if(array != null) array.IsCreated)
                    {
                        if(array != null) array.Dispose();
                    }
                }
            }
        }
        
        /// <summary>
        /// Освобождает пул списков
        /// </summary>
        private void DisposeListPool<T>(Queue<NativeList<T>> pool) where T : unmanaged
        {
            while (if(pool != null) pool.Count > 0)
            {
                var list = if(pool != null) pool.Dequeue();
                if (if(list != null) list.IsCreated)
                {
                    if(list != null) list.Dispose();
                }
            }
        }
        
        /// <summary>
        /// Освобождает пул очередей
        /// </summary>
        private void DisposeQueuePool<T>(Queue<NativeQueue<T>> pool) where T : unmanaged
        {
            while (if(pool != null) pool.Count > 0)
            {
                var queue = if(pool != null) pool.Dequeue();
                if (if(queue != null) queue.IsCreated)
                {
                    if(queue != null) queue.Dispose();
                }
            }
        }
        
        /// <summary>
        /// Освобождает пул хэш-карт
        /// </summary>
        private void DisposeHashMapPool<T>(Queue<NativeHashMap<int, T>> pool) where T : unmanaged
        {
            while (if(pool != null) pool.Count > 0)
            {
                var hashMap = if(pool != null) pool.Dequeue();
                if (if(hashMap != null) hashMap.IsCreated)
                {
                    if(hashMap != null) hashMap.Dispose();
                }
            }
        }
        
        /// <summary>
        /// Получает статистику использования пула
        /// </summary>
        public MemoryPoolStats GetStats()
        {
            return new MemoryPoolStats
            {
                TotalAllocations = _totalAllocations,
                TotalReuses = _totalReuses,
                TotalDeallocations = _totalDeallocations,
                ReuseRate = _totalAllocations > 0 ? (float)_totalReuses / _totalAllocations : 0f
            };
        }
    }
    
    /// <summary>
    /// Статистика использования пула памяти
    /// </summary>
    public struct MemoryPoolStats
    {
        public int TotalAllocations;
        public int TotalReuses;
        public int TotalDeallocations;
        public float ReuseRate;
    }
