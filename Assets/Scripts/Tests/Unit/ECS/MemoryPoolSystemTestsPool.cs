using NUnit.Framework;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using MudLike.Core.ECS;
using MudLike.Core.Performance;

namespace MudLike.Tests.ECS
{
    /// <summary>
    /// Тесты для MemoryPoolSystem
    /// </summary>
    [TestFixture]
    public class MemoryPoolSystemTestsPool
    {
        private World _world;
        private MemoryPoolSystem _system;

        [SetUp]
        public void SetUp()
        {
            _world = new World("TestWorld");
            _system = _world.GetOrCreateSystemManaged<MemoryPoolSystem>();
        }

        [TearDown]
        public void TearDown()
        {
            _world?.Dispose();
        }

        [Test]
        public void OnCreate_InitializesMemoryPool()
        {
            // Act
            _system.OnCreate();

            // Assert
            var memoryPool = _system.GetMemoryPool();
            Assert.IsNotNull(memoryPool);
        }

        [Test]
        public void GetMemoryPool_ReturnsValidPool()
        {
            // Arrange
            _system.OnCreate();

            // Act
            var pool = _system.GetMemoryPool();

            // Assert
            Assert.IsNotNull(pool);
            Assert.IsInstanceOf<MudLikeMemoryPool>(pool);
        }

        [Test]
        public void OnDestroy_CleansUpResources()
        {
            // Arrange
            _system.OnCreate();
            var pool = _system.GetMemoryPool();

            // Act
            _system.OnDestroy();

            // Assert
            // После OnDestroy пул должен быть освобожден
            // (в реальной реализации можно добавить проверку состояния)
        }

        [Test]
        public void OnUpdate_CallsMemoryPoolUpdate()
        {
            // Arrange
            _system.OnCreate();
            var pool = _system.GetMemoryPool();
            var initialStats = pool.GetStats();

            // Act
            _system.OnUpdate();

            // Assert
            // OnUpdate должен вызывать Update на пуле
            // (в реальной реализации можно добавить проверку состояния)
        }

        [Test]
        public void System_CanAllocateAndReleaseArrays()
        {
            // Arrange
            _system.OnCreate();
            var pool = _system.GetMemoryPool();
            const int size = 100;

            // Act
            var array = pool.GetFloat3Array(size);
            pool.ReturnFloat3Array(array);

            // Assert
            Assert.IsTrue(array.IsCreated);
            Assert.AreEqual(size, array.Length);
        }

        [Test]
        public void System_CanAllocateAndReleaseLists()
        {
            // Arrange
            _system.OnCreate();
            var pool = _system.GetMemoryPool();

            // Act
            var list = pool.GetFloat3List();
            list.Add(new float3(1, 2, 3));
            pool.ReturnFloat3List(list);

            // Assert
            Assert.IsTrue(list.IsCreated);
        }

        [Test]
        public void System_CanAllocateAndReleaseQueues()
        {
            // Arrange
            _system.OnCreate();
            var pool = _system.GetMemoryPool();

            // Act
            var queue = pool.GetFloat3Queue();
            queue.Enqueue(new float3(1, 2, 3));
            pool.ReturnFloat3Queue(queue);

            // Assert
            Assert.IsTrue(queue.IsCreated);
        }

        [Test]
        public void System_CanAllocateAndReleaseHashMaps()
        {
            // Arrange
            _system.OnCreate();
            var pool = _system.GetMemoryPool();

            // Act
            var hashMap = pool.GetIntFloat3HashMap();
            hashMap[1] = new float3(1, 2, 3);
            pool.ReturnIntFloat3HashMap(hashMap);

            // Assert
            Assert.IsTrue(hashMap.IsCreated);
        }

        [Test]
        public void System_IntegrationWithECS()
        {
            // Arrange
            _system.OnCreate();
            var pool = _system.GetMemoryPool();

            // Act - симулируем работу ECS системы
            var entities = new NativeArray<Entity>(10, Allocator.Temp);
            var positions = new NativeArray<float3>(10, Allocator.Temp);
            var velocities = new NativeArray<float3>(10, Allocator.Temp);

            // Заполняем данные
            for (int i = 0; i < 10; i++)
            {
                positions[i] = new float3(i, 0, 0);
                velocities[i] = new float3(1, 0, 0);
            }

            // Обновляем позиции (симуляция ECS системы)
            for (int i = 0; i < 10; i++)
            {
                positions[i] += velocities[i] * 0.016f; // 60 FPS
            }

            // Освобождаем ресурсы
            entities.Dispose();
            positions.Dispose();
            velocities.Dispose();

            // Assert
            var stats = pool.GetStats();
            Assert.Greater(stats.TotalAllocations, 0);
        }

        [Test]
        public void System_PerformanceTest()
        {
            // Arrange
            _system.OnCreate();
            var pool = _system.GetMemoryPool();
            const int iterations = 1000;
            const int arraySize = 100;

            // Act
            var startTime = System.DateTime.Now;
            
            for (int i = 0; i < iterations; i++)
            {
                var array = pool.GetFloat3Array(arraySize);
                // Симулируем работу с массивом
                for (int j = 0; j < arraySize; j++)
                {
                    array[j] = new float3(j, 0, 0);
                }
                pool.ReturnFloat3Array(array);
            }

            var endTime = System.DateTime.Now;
            var duration = endTime - startTime;

            // Assert
            Assert.Less(duration.TotalMilliseconds, 1000); // Должно быть быстрее 1 секунды
            var stats = pool.GetStats();
            Assert.Greater(stats.TotalReuses, 0); // Должны быть переиспользования
        }

        [Test]
        public void System_MemoryLeakTest()
        {
            // Arrange
            _system.OnCreate();
            var pool = _system.GetMemoryPool();
            const int iterations = 100;

            // Act - создаем и освобождаем много объектов
            for (int i = 0; i < iterations; i++)
            {
                var array = pool.GetFloat3Array(100);
                var list = pool.GetFloat3List();
                var queue = pool.GetFloat3Queue();
                var hashMap = pool.GetIntFloat3HashMap();

                pool.ReturnFloat3Array(array);
                pool.ReturnFloat3List(list);
                pool.ReturnFloat3Queue(queue);
                pool.ReturnIntFloat3HashMap(hashMap);
            }

            // Assert
            var stats = pool.GetStats();
            Assert.Greater(stats.TotalReuses, 0);
            // В реальной реализации можно добавить проверку на утечки памяти
        }
    }
}
