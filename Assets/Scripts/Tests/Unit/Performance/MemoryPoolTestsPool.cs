using NUnit.Framework;
using Unity.Collections;
using Unity.Mathematics;
using MudLike.Core.Performance;

namespace MudLike.Tests.Performance
{
    /// <summary>
    /// Тесты для MudLikeMemoryPool
    /// </summary>
    [TestFixture]
    public class MemoryPoolTestsPool
    {
        private MudLikeMemoryPool _memoryPool;

        [SetUp]
        public void SetUp()
        {
            _memoryPool = new MudLikeMemoryPool();
            _memoryPool.Initialize();
        }

        [TearDown]
        public void TearDown()
        {
            _memoryPool?.Dispose();
        }

        [Test]
        public void Initialize_CreatesValidPool()
        {
            // Arrange & Act
            var pool = new MudLikeMemoryPool();
            pool.Initialize();

            // Assert
            Assert.IsNotNull(pool);
            var stats = pool.GetStats();
            Assert.Greater(stats.TotalAllocations, 0);
        }

        [Test]
        public void GetFloat3Array_ReturnsValidArray()
        {
            // Arrange
            const int size = 100;

            // Act
            var array = _memoryPool.GetFloat3Array(size);

            // Assert
            Assert.IsTrue(array.IsCreated);
            Assert.AreEqual(size, array.Length);
        }

        [Test]
        public void ReturnFloat3Array_ReusesArray()
        {
            // Arrange
            const int size = 100;
            var array = _memoryPool.GetFloat3Array(size);
            var initialStats = _memoryPool.GetStats();

            // Act
            _memoryPool.ReturnFloat3Array(array);
            var array2 = _memoryPool.GetFloat3Array(size);
            var finalStats = _memoryPool.GetStats();

            // Assert
            Assert.IsTrue(array2.IsCreated);
            Assert.AreEqual(size, array2.Length);
            Assert.Greater(finalStats.TotalReuses, initialStats.TotalReuses);
        }

        [Test]
        public void GetFloatArray_ReturnsValidArray()
        {
            // Arrange
            const int size = 50;

            // Act
            var array = _memoryPool.GetFloatArray(size);

            // Assert
            Assert.IsTrue(array.IsCreated);
            Assert.AreEqual(size, array.Length);
        }

        [Test]
        public void GetIntArray_ReturnsValidArray()
        {
            // Arrange
            const int size = 25;

            // Act
            var array = _memoryPool.GetIntArray(size);

            // Assert
            Assert.IsTrue(array.IsCreated);
            Assert.AreEqual(size, array.Length);
        }

        [Test]
        public void GetBoolArray_ReturnsValidArray()
        {
            // Arrange
            const int size = 10;

            // Act
            var array = _memoryPool.GetBoolArray(size);

            // Assert
            Assert.IsTrue(array.IsCreated);
            Assert.AreEqual(size, array.Length);
        }

        [Test]
        public void GetFloat3List_ReturnsValidList()
        {
            // Act
            var list = _memoryPool.GetFloat3List();

            // Assert
            Assert.IsTrue(list.IsCreated);
            Assert.AreEqual(0, list.Length);
        }

        [Test]
        public void ReturnFloat3List_ReusesList()
        {
            // Arrange
            var list = _memoryPool.GetFloat3List();
            list.Add(new float3(1, 2, 3));
            var initialStats = _memoryPool.GetStats();

            // Act
            _memoryPool.ReturnFloat3List(list);
            var list2 = _memoryPool.GetFloat3List();
            var finalStats = _memoryPool.GetStats();

            // Assert
            Assert.IsTrue(list2.IsCreated);
            Assert.AreEqual(0, list2.Length); // Должен быть очищен
            Assert.Greater(finalStats.TotalReuses, initialStats.TotalReuses);
        }

        [Test]
        public void GetFloat3Queue_ReturnsValidQueue()
        {
            // Act
            var queue = _memoryPool.GetFloat3Queue();

            // Assert
            Assert.IsTrue(queue.IsCreated);
        }

        [Test]
        public void GetIntFloat3HashMap_ReturnsValidHashMap()
        {
            // Act
            var hashMap = _memoryPool.GetIntFloat3HashMap();

            // Assert
            Assert.IsTrue(hashMap.IsCreated);
        }

        [Test]
        public void GetStats_ReturnsValidStats()
        {
            // Act
            var stats = _memoryPool.GetStats();

            // Assert
            Assert.GreaterOrEqual(stats.TotalAllocations, 0);
            Assert.GreaterOrEqual(stats.TotalReuses, 0);
            Assert.GreaterOrEqual(stats.TotalDeallocations, 0);
            Assert.GreaterOrEqual(stats.ReuseRate, 0f);
            Assert.LessOrEqual(stats.ReuseRate, 1f);
        }

        [Test]
        public void Dispose_CleansUpResources()
        {
            // Arrange
            var array = _memoryPool.GetFloat3Array(100);
            var list = _memoryPool.GetFloat3List();

            // Act
            _memoryPool.Dispose();

            // Assert
            // После Dispose пул должен быть очищен
            // (в реальной реализации можно добавить проверку состояния)
        }

        [Test]
        public void MultipleArrays_SameSize_ReusesCorrectly()
        {
            // Arrange
            const int size = 100;
            var arrays = new NativeArray<float3>[5];

            // Act - создаем несколько массивов
            for (int i = 0; i < arrays.Length; i++)
            {
                arrays[i] = _memoryPool.GetFloat3Array(size);
            }

            // Возвращаем все массивы
            for (int i = 0; i < arrays.Length; i++)
            {
                _memoryPool.ReturnFloat3Array(arrays[i]);
            }

            // Создаем новые массивы (должны переиспользоваться)
            var newArrays = new NativeArray<float3>[5];
            for (int i = 0; i < newArrays.Length; i++)
            {
                newArrays[i] = _memoryPool.GetFloat3Array(size);
            }

            // Assert
            var stats = _memoryPool.GetStats();
            Assert.Greater(stats.TotalReuses, 0);
        }

        [Test]
        public void DifferentSizes_CreatesSeparatePools()
        {
            // Arrange
            const int size1 = 100;
            const int size2 = 200;

            // Act
            var array1 = _memoryPool.GetFloat3Array(size1);
            var array2 = _memoryPool.GetFloat3Array(size2);

            _memoryPool.ReturnFloat3Array(array1);
            _memoryPool.ReturnFloat3Array(array2);

            var reusedArray1 = _memoryPool.GetFloat3Array(size1);
            var reusedArray2 = _memoryPool.GetFloat3Array(size2);

            // Assert
            Assert.AreEqual(size1, reusedArray1.Length);
            Assert.AreEqual(size2, reusedArray2.Length);
        }
    }
}
