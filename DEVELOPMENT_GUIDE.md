# 🚀 Mud-Like Development Guide

## 📋 **РУКОВОДСТВО ПО РАЗРАБОТКЕ**

Полное руководство для разработчиков проекта Mud-Like с примерами кода, лучшими практиками и стандартами.

---

## 🎯 **БЫСТРЫЙ СТАРТ**

### **1. Настройка окружения**
```bash
# Клонирование репозитория
git clone https://github.com/your-username/Mud-Like.git
cd Mud-Like

# Открытие в Unity
# Unity 2022.3.62f1 автоматически установит пакеты
```

### **2. Первый запуск**
1. Откройте проект в Unity 2022.3.62f1
2. Дождитесь установки пакетов
3. Откройте сцену `Assets/Scenes/Main.unity`
4. Нажмите Play

### **3. Запуск тестов**
```bash
# В Unity: Window → General → Test Runner
# Или через командную строку:
unity -batchmode -quit -projectPath . -runTests -testResults results.xml
```

---

## 🏗️ **АРХИТЕКТУРНЫЕ ПРИНЦИПЫ**

### **1. ECS (Entity Component System)**
```csharp
// ✅ ПРАВИЛЬНО - создание ECS компонента
public struct VehicleConfig : IComponentData
{
    public float MaxSpeed;
    public float Acceleration;
    public float Mass;
    public float Drag;
    public float AngularDrag;
    public float TurnRadius;
    public float CenterOfMassHeight;
}

// ✅ ПРАВИЛЬНО - создание ECS системы
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[BurstCompile]
public partial class VehicleMovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = SystemAPI.Time.fixedDeltaTime;
        
        Entities
            .WithAll<VehicleTag>()
            .ForEach((ref LocalTransform transform, 
                     ref VehiclePhysics physics, 
                     in VehicleConfig config, 
                     in VehicleInput input) =>
            {
                ProcessVehicleMovement(ref transform, ref physics, config, input, deltaTime);
            }).Schedule();
    }
}
```

### **2. Детерминизм**
```csharp
// ✅ ПРАВИЛЬНО - детерминированное время
float deltaTime = SystemAPI.Time.fixedDeltaTime;

// ❌ НЕПРАВИЛЬНО - недетерминированное время
float deltaTime = Time.deltaTime;

// ✅ ПРАВИЛЬНО - детерминированная группа систем
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public partial class VehicleMovementSystem : SystemBase
```

### **3. Производительность**
```csharp
// ✅ ПРАВИЛЬНО - Burst компиляция
[BurstCompile(CompileSynchronously = true)]
public partial class VehicleMovementSystem : SystemBase

// ✅ ПРАВИЛЬНО - Job System
[BurstCompile]
public struct VehicleMovementJob : IJobEntity
{
    public float DeltaTime;
    
    public void Execute(ref LocalTransform transform, 
                       ref VehiclePhysics physics, 
                       in VehicleConfig config, 
                       in VehicleInput input)
    {
        // Параллельная обработка
    }
}
```

---

## 📁 **СТРУКТУРА ПРОЕКТА**

### **1. Модульная архитектура**
```
Assets/Scripts/
├── Core/                    # Базовые системы
│   ├── Components/         # ECS компоненты
│   ├── Systems/           # ECS системы
│   ├── Data/              # Структуры данных
│   └── Configs/           # Конфигурации
├── Vehicles/              # Транспорт
│   ├── Components/        # Компоненты транспорта
│   ├── Systems/          # Системы транспорта
│   └── Converters/       # Конвертеры
├── Terrain/              # Террейн и деформация
│   ├── Components/       # Компоненты террейна
│   └── Systems/         # Системы террейна
├── Networking/           # Мультиплеер
│   ├── Components/      # Сетевые компоненты
│   └── Systems/        # Сетевые системы
├── UI/                  # Пользовательский интерфейс
│   ├── Components/     # UI компоненты
│   └── Systems/       # UI системы
└── Tests/              # Тестирование
    ├── Unit/          # Unit тесты
    ├── Integration/   # Интеграционные тесты
    └── Performance/   # Тесты производительности
```

### **2. Assembly Definitions**
```csharp
// MudLike.Core.asmdef
{
    "name": "MudLike.Core",
    "references": [
        "Unity.Entities",
        "Unity.Mathematics",
        "Unity.Transforms",
        "Unity.Physics"
    ],
    "includePlatforms": [],
    "excludePlatforms": [],
    "allowUnsafeCode": true,
    "overrideReferences": false,
    "precompiledReferences": [],
    "autoReferenced": true,
    "defineConstraints": [],
    "versionDefines": [],
    "noEngineReferences": false
}
```

---

## 🧪 **ТЕСТИРОВАНИЕ**

### **1. Unit Tests**
```csharp
[TestFixture]
public class VehicleMovementSystemTests : MudLikeTestFixture
{
    private VehicleMovementSystem _system;
    
    [SetUp]
    public override void Setup()
    {
        base.Setup();
        _system = World.CreateSystemManaged<VehicleMovementSystem>();
    }
    
    [Test]
    public void VehicleMovementSystem_ShouldMoveForward_WhenThrottleApplied()
    {
        // Arrange
        var vehicle = CreateVehicle();
        var input = new VehicleInput { Vertical = 1f };
        EntityManager.SetComponentData(vehicle, input);
        
        // Act
        _system.Update();
        
        // Assert
        AssertVehicleMoved(vehicle, new float3(0, 0, 1));
    }
}
```

### **2. Performance Tests**
```csharp
[Test]
public void PerformanceTest_VehicleMovementSystem()
{
    // Arrange
    var system = World.CreateSystemManaged<VehicleMovementSystem>();
    
    // Act & Assert
    using (var profiler = new ProfilerMarker("VehicleMovementSystem"))
    {
        profiler.Begin();
        
        for (int i = 0; i < 100; i++)
        {
            system.Update();
        }
        
        profiler.End();
    }
    
    Assert.DoesNotThrow(() => system.Update());
}
```

### **3. Integration Tests**
```csharp
[Test]
public void IntegrationTest_VehiclePhysicsIntegration()
{
    // Arrange
    var vehicleSystem = World.CreateSystemManaged<VehicleMovementSystem>();
    var wheelSystem = World.CreateSystemManaged<AdvancedWheelPhysicsSystem>();
    
    // Act
    vehicleSystem.Update();
    wheelSystem.Update();
    
    // Assert
    // Проверка интеграции систем
}
```

---

## 🚀 **ПРОИЗВОДИТЕЛЬНОСТЬ**

### **1. Burst Compilation**
```csharp
// ✅ ПРАВИЛЬНО - Burst компиляция
[BurstCompile(CompileSynchronously = true)]
public partial class VehicleMovementSystem : SystemBase

// ✅ ПРАВИЛЬНО - Burst Job
[BurstCompile]
public struct VehicleMovementJob : IJobEntity
{
    public void Execute(ref LocalTransform transform, ref VehiclePhysics physics)
    {
        // Оптимизированный код
    }
}
```

### **2. Job System**
```csharp
// ✅ ПРАВИЛЬНО - параллельная обработка
Dependency = new VehicleMovementJob
{
    DeltaTime = deltaTime
}.ScheduleParallel(_vehicleQuery, Dependency);

// ✅ ПРАВИЛЬНО - Entity Query
private EntityQuery _vehicleQuery = GetEntityQuery(
    ComponentType.ReadWrite<LocalTransform>(),
    ComponentType.ReadWrite<VehiclePhysics>(),
    ComponentType.ReadOnly<VehicleConfig>()
);
```

### **3. Memory Management**
```csharp
// ✅ ПРАВИЛЬНО - NativeArray
using (var entities = _vehicleQuery.ToEntityArray(Allocator.Temp))
{
    // Обработка сущностей
}

// ✅ ПРАВИЛЬНО - Dispose pattern
public void Dispose()
{
    if (_nativeArray.IsCreated)
    {
        _nativeArray.Dispose();
    }
}
```

---

## 🌐 **МУЛЬТИПЛЕЕР**

### **1. Client-Server Architecture**
```csharp
// ✅ ПРАВИЛЬНО - серверная валидация
[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial class InputValidationSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities
            .WithAll<NetworkId>()
            .ForEach((ref VehicleInput input, in NetworkId networkId) =>
            {
                // Валидация ввода на сервере
                ValidateInput(ref input, networkId.Value);
            }).Schedule();
    }
}
```

### **2. Lag Compensation**
```csharp
// ✅ ПРАВИЛЬНО - компенсация задержек
[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial class LagCompensationSystem : SystemBase
{
    protected override void OnUpdate()
    {
        // Компенсация сетевых задержек
        CompensateLag();
    }
}
```

### **3. Network Synchronization**
```csharp
// ✅ ПРАВИЛЬНО - сетевые компоненты
public struct NetworkVehicle : IComponentData
{
    public int NetworkId;
    public float3 Position;
    public quaternion Rotation;
    public float3 Velocity;
    public float3 AngularVelocity;
}
```

---

## 📝 **СТАНДАРТЫ КОДИРОВАНИЯ**

### **1. Именование**
```csharp
// ✅ ПРАВИЛЬНО - PascalCase для классов
public class VehicleMovementSystem : SystemBase

// ✅ ПРАВИЛЬНО - camelCase для полей
private EntityQuery _vehicleQuery;

// ✅ ПРАВИЛЬНО - UPPER_CASE для констант
private const float MAX_SPEED = 100f;
```

### **2. Документация**
```csharp
/// <summary>
/// Система движения транспортного средства
/// </summary>
/// <remarks>
/// Обрабатывает движение всех транспортных средств в игре.
/// Использует детерминированную физику для мультиплеера.
/// </remarks>
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public partial class VehicleMovementSystem : SystemBase
{
    /// <summary>
    /// Обрабатывает движение всех транспортных средств
    /// </summary>
    protected override void OnUpdate()
    {
        // Реализация
    }
}
```

### **3. Комментарии**
```csharp
// ✅ ПРАВИЛЬНО - объяснение сложной логики
// Вычисляем деформацию террейна на основе давления колеса
var deformation = CalculateDeformation(wheelPressure, terrainHardness);

// ❌ НЕПРАВИЛЬНО - очевидные комментарии
// Увеличиваем позицию
position += velocity * deltaTime;
```

---

## 🔧 **ИНСТРУМЕНТЫ РАЗРАБОТКИ**

### **1. Unity Profiler**
```csharp
// Профилирование производительности
using (var profiler = new ProfilerMarker("VehicleMovementSystem"))
{
    profiler.Begin();
    
    // Код для профилирования
    
    profiler.End();
}
```

### **2. Memory Profiler**
```csharp
// Анализ использования памяти
var initialMemory = UnityEngine.Profiling.Profiler.GetTotalAllocatedMemory(false);

// Код для анализа

var peakMemory = UnityEngine.Profiling.Profiler.GetTotalAllocatedMemory(false);
var memoryIncrease = peakMemory - initialMemory;
```

### **3. Burst Inspector**
```csharp
// Анализ Burst кода
[BurstCompile]
public struct OptimizedJob : IJobEntity
{
    public void Execute(ref LocalTransform transform, ref VehiclePhysics physics)
    {
        // Код для анализа в Burst Inspector
    }
}
```

---

## 🐛 **ОТЛАДКА**

### **1. Entity Debugger**
```csharp
// Отладка ECS сущностей
#if UNITY_EDITOR
using Unity.Entities.Editor;
#endif

// В Editor: Window → DOTS → Entity Debugger
```

### **2. Job Debugger**
```csharp
// Отладка Jobs
#if UNITY_EDITOR
using Unity.Jobs.Editor;
#endif

// В Editor: Window → DOTS → Job Debugger
```

### **3. Logging**
```csharp
// ✅ ПРАВИЛЬНО - структурированное логирование
Debug.Log($"[VehicleMovementSystem] Vehicle {entity.Index} moved to {position}");

// ❌ НЕПРАВИЛЬНО - неструктурированное логирование
Debug.Log("Vehicle moved");
```

---

## 📚 **РЕСУРСЫ**

### **1. Документация Unity**
- [Unity DOTS Documentation](https://docs.unity3d.com/Packages/com.unity.entities@latest)
- [Unity Physics Documentation](https://docs.unity3d.com/Packages/com.unity.physics@latest)
- [Unity NetCode Documentation](https://docs.unity3d.com/Packages/com.unity.netcode@latest)

### **2. Лучшие практики**
- [ECS Best Practices](https://docs.unity3d.com/Packages/com.unity.entities@latest/manual/ecs_best_practices.html)
- [Job System Best Practices](https://docs.unity3d.com/Manual/JobSystemBestPractices.html)
- [Burst Compiler Best Practices](https://docs.unity3d.com/Packages/com.unity.burst@latest/manual/best-practices.html)

### **3. Сообщество**
- [Unity DOTS Forum](https://forum.unity.com/forums/dots.47/)
- [Unity Discord](https://discord.gg/unity)
- [GitHub Issues](https://github.com/your-username/Mud-Like/issues)

---

## 🎯 **ЗАКЛЮЧЕНИЕ**

Следуйте этим принципам для качественной разработки:
- **ECS архитектура** - основа всего
- **Детерминизм** - критически важен
- **Производительность** - заложена в архитектуру
- **Тестирование** - качество с первого дня

Удачи в разработке! 🚀

---

**Версия руководства:** 1.0  
**Дата обновления:** $(date)  
**Статус:** ✅ АКТУАЛЬНО