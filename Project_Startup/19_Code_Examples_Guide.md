# 💻 Mud-Like Code Examples Guide

## 📋 **ОБЗОР**

Практические примеры кода для разработки в проекте Mud-Like с использованием ECS архитектуры.

## 🚗 **ПРИМЕРЫ ECS КОМПОНЕНТОВ**

### **1. Создание компонента позиции**

```csharp
using Unity.Entities;
using Unity.Mathematics;

namespace MudLike.Core.Components
{
    /// <summary>
    /// Компонент позиции в мире
    /// </summary>
    public struct Position : IComponentData
    {
        public float3 Value;
        
        /// <summary>
        /// Создает позицию с заданными координатами
        /// </summary>
        public static Position Create(float x, float y, float z)
        {
            return new Position { Value = new float3(x, y, z) };
        }
        
        /// <summary>
        /// Создает позицию из вектора
        /// </summary>
        public static Position FromVector3(float3 vector)
        {
            return new Position { Value = vector };
        }
    }
}
```

### **2. Создание компонента скорости**

```csharp
using Unity.Entities;
using Unity.Mathematics;

namespace MudLike.Core.Components
{
    /// <summary>
    /// Компонент скорости сущности
    /// </summary>
    public struct Velocity : IComponentData
    {
        public float3 Value;
        public float Magnitude => math.length(Value);
        
        /// <summary>
        /// Нормализует скорость
        /// </summary>
        public float3 Normalized => math.normalize(Value);
        
        /// <summary>
        /// Создает скорость с заданными компонентами
        /// </summary>
        public static Velocity Create(float x, float y, float z)
        {
            return new Velocity { Value = new float3(x, y, z) };
        }
    }
}
```

## 🏗️ **ПРИМЕРЫ ECS СИСТЕМ**

### **1. Система движения**

```csharp
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;
using Unity.Jobs;

namespace MudLike.Core.Systems
{
    /// <summary>
    /// Система движения сущностей
    /// </summary>
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile]
    public partial class MovementSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            float deltaTime = Time.fixedDeltaTime;
            
            Entities
                .WithAll<Position, Velocity>()
                .ForEach((ref LocalTransform transform, 
                         in Position position, 
                         in Velocity velocity) =>
                {
                    // Обновляем позицию
                    transform.Position = position.Value + velocity.Value * deltaTime;
                }).Schedule();
        }
    }
}
```

### **2. Оптимизированная система с Job**

```csharp
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;
using Unity.Jobs;

namespace MudLike.Core.Systems
{
    /// <summary>
    /// Оптимизированная система движения с Job System
    /// </summary>
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile]
    public partial class OptimizedMovementSystem : SystemBase
    {
        private EntityQuery _movementQuery;
        
        protected override void OnCreate()
        {
            _movementQuery = GetEntityQuery(
                ComponentType.ReadWrite<LocalTransform>(),
                ComponentType.ReadOnly<Position>(),
                ComponentType.ReadOnly<Velocity>()
            );
        }
        
        protected override void OnUpdate()
        {
            float deltaTime = Time.fixedDeltaTime;
            
            var movementJob = new MovementJob
            {
                DeltaTime = deltaTime
            };
            
            Dependency = movementJob.ScheduleParallel(_movementQuery, Dependency);
        }
        
        [BurstCompile]
        public partial struct MovementJob : IJobEntity
        {
            public float DeltaTime;
            
            public void Execute(ref LocalTransform transform, 
                              in Position position, 
                              in Velocity velocity)
            {
                transform.Position = position.Value + velocity.Value * DeltaTime;
            }
        }
    }
}
```

## 🚗 **ПРИМЕРЫ ТРАНСПОРТНЫХ СИСТЕМ**

### **1. Система управления транспортом**

```csharp
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;
using MudLike.Vehicles.Components;

namespace MudLike.Vehicles.Systems
{
    /// <summary>
    /// Система управления транспортным средством
    /// </summary>
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile]
    public partial class VehicleControlSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            float deltaTime = Time.fixedDeltaTime;
            
            Entities
                .WithAll<VehicleTag, VehicleInput, VehiclePhysics>()
                .ForEach((ref LocalTransform transform,
                         ref VehiclePhysics physics,
                         in VehicleInput input) =>
                {
                    ProcessVehicleControl(ref transform, ref physics, input, deltaTime);
                }).Schedule();
        }
        
        private static void ProcessVehicleControl(ref LocalTransform transform,
                                                ref VehiclePhysics physics,
                                                in VehicleInput input,
                                                float deltaTime)
        {
            // Вычисляем направление движения
            float3 forward = math.forward(transform.Rotation);
            float3 right = math.right(transform.Rotation);
            
            // Применяем ввод игрока
            float3 movement = forward * input.Throttle + right * input.Steering;
            
            // Обновляем скорость
            physics.Velocity += movement * physics.Acceleration * deltaTime;
            
            // Ограничиваем максимальную скорость
            if (math.length(physics.Velocity) > physics.MaxSpeed)
            {
                physics.Velocity = math.normalize(physics.Velocity) * physics.MaxSpeed;
            }
            
            // Применяем сопротивление
            physics.Velocity *= (1f - physics.Drag * deltaTime);
            
            // Обновляем позицию
            transform.Position += physics.Velocity * deltaTime;
        }
    }
}
```

## 🌍 **ПРИМЕРЫ СИСТЕМ ТЕРРЕЙНА**

### **1. Система деформации террейна**

```csharp
using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Burst;
using MudLike.Terrain.Components;

namespace MudLike.Terrain.Systems
{
    /// <summary>
    /// Система деформации террейна
    /// </summary>
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile]
    public partial class TerrainDeformationSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            float deltaTime = Time.fixedDeltaTime;
            
            Entities
                .WithAll<DeformationData, TerrainData>()
                .ForEach((ref TerrainData terrain, in DeformationData deformation) =>
                {
                    ProcessTerrainDeformation(ref terrain, deformation, deltaTime);
                }).Schedule();
        }
        
        private static void ProcessTerrainDeformation(ref TerrainData terrain,
                                                     in DeformationData deformation,
                                                     float deltaTime)
        {
            // Вычисляем радиус деформации
            float radius = deformation.Radius;
            float3 center = deformation.Position;
            
            // Применяем деформацию к высотной карте
            for (int x = 0; x < terrain.HeightMap.Length; x++)
            {
                for (int z = 0; z < terrain.HeightMap[x].Length; z++)
                {
                    float3 worldPos = new float3(x, 0, z) * terrain.ChunkSize;
                    float distance = math.distance(worldPos, center);
                    
                    if (distance <= radius)
                    {
                        // Вычисляем силу деформации
                        float strength = 1f - (distance / radius);
                        float deformationAmount = deformation.Strength * strength * deltaTime;
                        
                        // Применяем деформацию
                        terrain.HeightMap[x][z] += deformationAmount;
                    }
                }
            }
        }
    }
}
```

## 🌐 **ПРИМЕРЫ СЕТЕВЫХ СИСТЕМ**

### **1. Система синхронизации позиций**

```csharp
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;
using MudLike.Networking.Components;

namespace MudLike.Networking.Systems
{
    /// <summary>
    /// Система синхронизации позиций по сети
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [BurstCompile]
    public partial class NetworkPositionSyncSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            float currentTime = Time.time;
            
            Entities
                .WithAll<NetworkPosition, LocalTransform>()
                .ForEach((ref NetworkPosition networkPos, in LocalTransform transform) =>
                {
                    // Проверяем, нужно ли обновление
                    if (ShouldUpdatePosition(networkPos, transform, currentTime))
                    {
                        // Обновляем сетевые данные
                        networkPos.Value = transform.Position;
                        networkPos.Rotation = transform.Rotation;
                        networkPos.HasChanged = true;
                        networkPos.LastUpdateTime = currentTime;
                    }
                }).Schedule();
        }
        
        private static bool ShouldUpdatePosition(NetworkPosition networkPos, 
                                               LocalTransform transform, 
                                               float currentTime)
        {
            // Обновляем, если позиция изменилась значительно
            float positionDelta = math.distance(networkPos.Value, transform.Position);
            float rotationDelta = math.distance(networkPos.Rotation.value, transform.Rotation.value);
            
            return positionDelta > 0.1f || rotationDelta > 0.01f || 
                   (currentTime - networkPos.LastUpdateTime) > 0.1f;
        }
    }
}
```

## 🧪 **ПРИМЕРЫ ТЕСТИРОВАНИЯ**

### **1. Unit тест для системы движения**

```csharp
using NUnit.Framework;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using MudLike.Core.Components;
using MudLike.Core.Systems;

namespace MudLike.Tests.Unit
{
    /// <summary>
    /// Unit тесты для системы движения
    /// </summary>
    public class MovementSystemTests
    {
        [Test]
        public void MovementSystem_ValidInput_MovesEntity()
        {
            // Arrange
            var transform = new LocalTransform
            {
                Position = float3.zero,
                Rotation = quaternion.identity,
                Scale = 1f
            };
            var position = new Position { Value = float3.zero };
            var velocity = new Velocity { Value = new float3(1, 0, 0) };
            float deltaTime = 1f;
            
            // Act
            MovementSystem.ProcessMovement(ref transform, position, velocity, deltaTime);
            
            // Assert
            Assert.AreEqual(new float3(1, 0, 0), transform.Position);
        }
        
        [Test]
        public void MovementSystem_ZeroVelocity_DoesNotMove()
        {
            // Arrange
            var transform = new LocalTransform
            {
                Position = float3.zero,
                Rotation = quaternion.identity,
                Scale = 1f
            };
            var position = new Position { Value = float3.zero };
            var velocity = new Velocity { Value = float3.zero };
            float deltaTime = 1f;
            
            // Act
            MovementSystem.ProcessMovement(ref transform, position, velocity, deltaTime);
            
            // Assert
            Assert.AreEqual(float3.zero, transform.Position);
        }
    }
}
```

## 📊 **ПРИМЕРЫ ПРОФИЛИРОВАНИЯ**

### **1. Система метрик производительности**

```csharp
using Unity.Entities;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Collections;

namespace MudLike.Core.Systems
{
    /// <summary>
    /// Система сбора метрик производительности
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [BurstCompile]
    public partial class PerformanceMetricsSystem : SystemBase
    {
        private NativeArray<PerformanceMetrics> _metrics;
        
        protected override void OnCreate()
        {
            _metrics = new NativeArray<PerformanceMetrics>(1, Allocator.Persistent);
        }
        
        protected override void OnUpdate()
        {
            var metrics = _metrics[0];
            
            // Обновляем метрики
            metrics.FrameTime = Time.deltaTime;
            metrics.EntityCount = EntityManager.UniversalQuery.CalculateEntityCount();
            metrics.MemoryUsage = UnityEngine.Profiling.Profiler.GetTotalAllocatedMemory(false);
            
            _metrics[0] = metrics;
        }
        
        protected override void OnDestroy()
        {
            if (_metrics.IsCreated)
                _metrics.Dispose();
        }
    }
    
    public struct PerformanceMetrics : IComponentData
    {
        public float FrameTime;
        public int EntityCount;
        public float MemoryUsage;
    }
}
```

## 🎯 **ЛУЧШИЕ ПРАКТИКИ**

### **1. Использование Burst Compilation**
- Всегда добавляйте `[BurstCompile]` к системам
- Используйте `IJobEntity` для параллельной обработки
- Избегайте managed типов в Burst коде

### **2. Оптимизация памяти**
- Используйте `NativeArray` вместо managed массивов
- Применяйте `Allocator.Persistent` для долгоживущих данных
- Освобождайте память в `OnDestroy()`

### **3. Производительность**
- Группируйте компоненты в EntityQuery
- Используйте `ScheduleParallel` для параллельной обработки
- Кэшируйте часто используемые значения

---

**Дата создания**: $(date)
**Версия**: 1.0
**Статус**: Готов к использованию
**Unity Version**: 6000.0.57f1