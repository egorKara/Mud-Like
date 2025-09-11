using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using System.Collections.Generic;

namespace MudLike.Core.Performance
{
    /// <summary>
    /// Пул памяти для оптимизации аллокаций
    /// </summary>
    public class MemoryPool : SystemBase
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
        
        protected override void OnCreate()
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
        
        protected override void OnDestroy()
        {
            // Освобождение всех объектов в пулах
            DisposeAllPools();
        }
        
        protected override void OnUpdate()
        {
            // Очистка неиспользуемых объектов периодически
            if (Time.frameCount % 1000 == 0)
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
        public NativeArray<float3> GetFloat3Array(int size, Allocator allocator = Allocator.TempJob)
        {
            if (_float3ArrayPool.TryGetValue(size, out Queue<NativeArray<float3>> pool) && pool.Count > 0)
            {
                var array = pool.Dequeue();
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
            if (!array.IsCreated) return;
            
            int size = array.Length;
            if (!_float3ArrayPool.ContainsKey(size))
            {
                _float3ArrayPool[size] = new Queue<NativeArray<float3>>();
            }
            
            _float3ArrayPool[size].Enqueue(array);
        }
        
        /// <summary>
        /// Получает NativeArray<float> из пула
        /// </summary>
        public NativeArray<float> GetFloatArray(int size, Allocator allocator = Allocator.TempJob)
        {
            if (_floatArrayPool.TryGetValue(size, out Queue<NativeArray<float>> pool) && pool.Count > 0)
            {
                var array = pool.Dequeue();
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
            if (!array.IsCreated) return;
            
            int size = array.Length;
            if (!_floatArrayPool.ContainsKey(size))
            {
                _floatArrayPool[size] = new Queue<NativeArray<float>>();
            }
            
            _floatArrayPool[size].Enqueue(array);
        }
        
        /// <summary>
        /// Получает NativeArray<int> из пула
        /// </summary>
        public NativeArray<int> GetIntArray(int size, Allocator allocator = Allocator.TempJob)
        {
            if (_intArrayPool.TryGetValue(size, out Queue<NativeArray<int>> pool) && pool.Count > 0)
            {
                var array = pool.Dequeue();
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
            if (!array.IsCreated) return;
            
            int size = array.Length;
            if (!_intArrayPool.ContainsKey(size))
            {
                _intArrayPool[size] = new Queue<NativeArray<int>>();
            }
            
            _intArrayPool[size].Enqueue(array);
        }
        
        /// <summary>
        /// Получает NativeArray<bool> из пула
        /// </summary>
        public NativeArray<bool> GetBoolArray(int size, Allocator allocator = Allocator.TempJob)
        {
            if (_boolArrayPool.TryGetValue(size, out Queue<NativeArray<bool>> pool) && pool.Count > 0)
            {
                var array = pool.Dequeue();
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
            if (!array.IsCreated) return;
            
            int size = array.Length;
            if (!_boolArrayPool.ContainsKey(size))
            {
                _boolArrayPool[size] = new Queue<NativeArray<bool>>();
            }
            
            _boolArrayPool[size].Enqueue(array);
        }
        
        /// <summary>
        /// Получает NativeList<float3> из пула
        /// </summary>
        public NativeList<float3> GetFloat3List(Allocator allocator = Allocator.TempJob)
        {
            if (_float3ListPool.Count > 0)
            {
                var list = _float3ListPool.Dequeue();
                list.Clear();
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
            if (!list.IsCreated) return;
            
            list.Clear();
            _float3ListPool.Enqueue(list);
        }
        
        /// <summary>
        /// Получает NativeList<float> из пула
        /// </summary>
        public NativeList<float> GetFloatList(Allocator allocator = Allocator.TempJob)
        {
            if (_floatListPool.Count > 0)
            {
                var list = _floatListPool.Dequeue();
                list.Clear();
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
            if (!list.IsCreated) return;
            
            list.Clear();
            _floatListPool.Enqueue(list);
        }
        
        /// <summary>
        /// Получает NativeList<int> из пула
        /// </summary>
        public NativeList<int> GetIntList(Allocator allocator = Allocator.TempJob)
        {
            if (_intListPool.Count > 0)
            {
                var list = _intListPool.Dequeue();
                list.Clear();
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
            if (!list.IsCreated) return;
            
            list.Clear();
            _intListPool.Enqueue(list);
        }
        
        /// <summary>
        /// Получает NativeList<bool> из пула
        /// </summary>
        public NativeList<bool> GetBoolList(Allocator allocator = Allocator.TempJob)
        {
            if (_boolListPool.Count > 0)
            {
                var list = _boolListPool.Dequeue();
                list.Clear();
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
            if (!list.IsCreated) return;
            
            list.Clear();
            _boolListPool.Enqueue(list);
        }
        
        /// <summary>
        /// Получает NativeQueue<float3> из пула
        /// </summary>
        public NativeQueue<float3> GetFloat3Queue(Allocator allocator = Allocator.TempJob)
        {
            if (_float3QueuePool.Count > 0)
            {
                var queue = _float3QueuePool.Dequeue();
                while (queue.TryDequeue(out _)) { } // Очистка очереди
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
            if (!queue.IsCreated) return;
            
            while (queue.TryDequeue(out _)) { } // Очистка очереди
            _float3QueuePool.Enqueue(queue);
        }
        
        /// <summary>
        /// Получает NativeQueue<float> из пула
        /// </summary>
        public NativeQueue<float> GetFloatQueue(Allocator allocator = Allocator.TempJob)
        {
            if (_floatQueuePool.Count > 0)
            {
                var queue = _floatQueuePool.Dequeue();
                while (queue.TryDequeue(out _)) { } // Очистка очереди
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
            if (!queue.IsCreated) return;
            
            while (queue.TryDequeue(out _)) { } // Очистка очереди
            _floatQueuePool.Enqueue(queue);
        }
        
        /// <summary>
        /// Получает NativeQueue<int> из пула
        /// </summary>
        public NativeQueue<int> GetIntQueue(Allocator allocator = Allocator.TempJob)
        {
            if (_intQueuePool.Count > 0)
            {
                var queue = _intQueuePool.Dequeue();
                while (queue.TryDequeue(out _)) { } // Очистка очереди
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
            if (!queue.IsCreated) return;
            
            while (queue.TryDequeue(out _)) { } // Очистка очереди
            _intQueuePool.Enqueue(queue);
        }
        
        /// <summary>
        /// Получает NativeHashMap<int, float3> из пула
        /// </summary>
        public NativeHashMap<int, float3> GetIntFloat3HashMap(Allocator allocator = Allocator.TempJob)
        {
            if (_intFloat3HashMapPool.Count > 0)
            {
                var hashMap = _intFloat3HashMapPool.Dequeue();
                hashMap.Clear();
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
            if (!hashMap.IsCreated) return;
            
            hashMap.Clear();
            _intFloat3HashMapPool.Enqueue(hashMap);
        }
        
        /// <summary>
        /// Получает NativeHashMap<int, float> из пула
        /// </summary>
        public NativeHashMap<int, float> GetIntFloatHashMap(Allocator allocator = Allocator.TempJob)
        {
            if (_intFloatHashMapPool.Count > 0)
            {
                var hashMap = _intFloatHashMapPool.Dequeue();
                hashMap.Clear();
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
            if (!hashMap.IsCreated) return;
            
            hashMap.Clear();
            _intFloatHashMapPool.Enqueue(hashMap);
        }
        
        /// <summary>
        /// Получает NativeHashMap<int, int> из пула
        /// </summary>
        public NativeHashMap<int, int> GetIntIntHashMap(Allocator allocator = Allocator.TempJob)
        {
            if (_intIntHashMapPool.Count > 0)
            {
                var hashMap = _intIntHashMapPool.Dequeue();
                hashMap.Clear();
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
            if (!hashMap.IsCreated) return;
            
            hashMap.Clear();
            _intIntHashMapPool.Enqueue(hashMap);
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
                var queue = kvp.Value;
                while (queue.Count > 10) // Оставляем только 10 объектов
                {
                    var array = queue.Dequeue();
                    if (array.IsCreated)
                    {
                        array.Dispose();
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
                var queue = kvp.Value;
                while (queue.Count > 0)
                {
                    var array = queue.Dequeue();
                    if (array.IsCreated)
                    {
                        array.Dispose();
                    }
                }
            }
        }
        
        /// <summary>
        /// Освобождает пул списков
        /// </summary>
        private void DisposeListPool<T>(Queue<NativeList<T>> pool) where T : unmanaged
        {
            while (pool.Count > 0)
            {
                var list = pool.Dequeue();
                if (list.IsCreated)
                {
                    list.Dispose();
                }
            }
        }
        
        /// <summary>
        /// Освобождает пул очередей
        /// </summary>
        private void DisposeQueuePool<T>(Queue<NativeQueue<T>> pool) where T : unmanaged
        {
            while (pool.Count > 0)
            {
                var queue = pool.Dequeue();
                if (queue.IsCreated)
                {
                    queue.Dispose();
                }
            }
        }
        
        /// <summary>
        /// Освобождает пул хэш-карт
        /// </summary>
        private void DisposeHashMapPool<T>(Queue<NativeHashMap<int, T>> pool) where T : unmanaged
        {
            while (pool.Count > 0)
            {
                var hashMap = pool.Dequeue();
                if (hashMap.IsCreated)
                {
                    hashMap.Dispose();
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
}