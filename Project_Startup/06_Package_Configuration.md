# 📦 Mud-Like Package Configuration

## 🎯 **ОБЗОР ПАКЕТОВ**

### **Основные категории пакетов**
- **DOTS Core** - базовая функциональность ECS
- **Physics** - физическая симуляция
- **Networking** - сетевая функциональность
- **Testing** - тестирование и качество
- **UI** - пользовательский интерфейс
- **Utilities** - вспомогательные инструменты

## 🏗️ **DOTS CORE ПАКЕТЫ**

### **1. Unity Entities (com.unity.entities)**
```json
{
  "com.unity.entities": "1.3.14"
}
```

**Назначение:** Основной пакет ECS для управления сущностями, компонентами и системами.

**Ключевые возможности:**
- **EntityManager** - управление сущностями
- **IComponentData** - интерфейс для компонентов данных
- **SystemBase** - базовый класс для систем
- **World** - контейнер для систем и сущностей

**Настройка:**
```csharp
// Создание World
var world = new World("GameWorld");
World.DefaultGameObjectInjectionWorld = world;

// Регистрация систем
world.GetOrCreateSystem<MovementSystem>();
world.GetOrCreateSystem<PhysicsSystem>();
```

### **2. Unity Collections (com.unity.collections)**
```json
{
  "com.unity.collections": "2.5.7"
}
```

**Назначение:** Коллекции данных для высокопроизводительной работы с памятью.

**Ключевые возможности:**
- **NativeArray<T>** - управляемые массивы
- **NativeList<T>** - динамические списки
- **NativeHashMap<TKey, TValue>** - хеш-таблицы
- **UnsafeList<T>** - небезопасные списки

**Пример использования:**
```csharp
using Unity.Collections;

// Создание NativeArray
var positions = new NativeArray<float3>(1000, Allocator.Persistent);

// Использование в Job
var job = new ProcessPositionsJob
{
    positions = positions
};
job.Schedule(positions.Length, 64).Complete();

// Освобождение памяти
positions.Dispose();
```

### **3. Unity Jobs (com.unity.jobs)**
```json
{
  "com.unity.jobs": "1.0.0"
}
```

**Назначение:** Система параллельных задач для многопоточности.

**Ключевые возможности:**
- **IJob** - однопоточные задачи
- **IJobParallelFor** - параллельные задачи
- **JobHandle** - управление выполнением
- **Dependency** - управление зависимостями

**Пример использования:**
```csharp
using Unity.Jobs;

// Параллельная обработка позиций
public struct ProcessPositionsJob : IJobParallelFor
{
    public NativeArray<float3> positions;
    public float deltaTime;
    
    public void Execute(int index)
    {
        positions[index] += new float3(0, -9.81f * deltaTime, 0);
    }
}

// Запуск задачи
var job = new ProcessPositionsJob
{
    positions = positions,
    deltaTime = Time.fixedDeltaTime
};
job.Schedule(positions.Length, 64).Complete();
```

### **4. Unity Burst (com.unity.burst)**
```json
{
  "com.unity.burst": "1.8.12"
}
```

**Назначение:** Компилятор для высокопроизводительного кода.

**Ключевые возможности:**
- **BurstCompile** - атрибут для компиляции
- **SIMD инструкции** - векторные операции
- **Оптимизация** - автоматическая оптимизация кода
- **Производительность** - до 10x ускорение

**Пример использования:**
```csharp
using Unity.Burst;

[BurstCompile]
public struct ProcessPositionsJob : IJobParallelFor
{
    public NativeArray<float3> positions;
    public float deltaTime;
    
    public void Execute(int index)
    {
        // Высокопроизводительный код
        positions[index] += new float3(0, -9.81f * deltaTime, 0);
    }
}
```

## ⚡ **PHYSICS ПАКЕТЫ**

### **1. Unity Physics (com.unity.physics)**
```json
{
  "com.unity.physics": "1.3.14"
}
```

**Назначение:** Высокопроизводительная физическая симуляция для DOTS.

**Ключевые возможности:**
- **PhysicsWorld** - физический мир
- **CollisionWorld** - мир коллизий
- **RigidBody** - физические тела
- **Collider** - коллайдеры

**Настройка:**
```csharp
using Unity.Physics;

// Создание физического мира
var physicsWorld = new PhysicsWorld();
var collisionWorld = new CollisionWorld();

// Настройка физических систем
world.GetOrCreateSystem<PhysicsStepSystem>();
world.GetOrCreateSystem<PhysicsBodySystem>();
```

### **2. Unity Rendering Hybrid (com.unity.rendering.hybrid)**
```json
{
  "com.unity.rendering.hybrid": "1.1.0"
}
```

**Назначение:** Интеграция ECS с системой рендеринга Unity.

**Ключевые возможности:**
- **RenderingSystem** - система рендеринга
- **MeshRenderer** - рендеринг мешей
- **MaterialPropertyBlock** - свойства материалов
- **LOD System** - система уровней детализации

## 🌐 **NETWORKING ПАКЕТЫ**

### **1. Netcode for Entities (com.unity.netcode.entities)**
```json
{
  "com.unity.netcode.entities": "1.2.0"
}
```

**Назначение:** Сетевая функциональность для ECS приложений.

**Ключевые возможности:**
- **NetworkManager** - управление сетью
- **ICommandTargetData** - команды от клиента к серверу
- **IEventData** - события от сервера к клиентам
- **NetworkId** - идентификация сетевых объектов

**Настройка:**
```csharp
using Unity.NetCode;

// Создание NetworkManager
var networkManager = new NetworkManager();
networkManager.Initialize();

// Настройка World для сети
var hostWorld = new World("HostGameWorld");
var serverWorld = new World("ServerGameWorld");
serverWorld.GetOrCreateSystem<ServerSimulationSystemGroup>();
```

## 🧪 **TESTING ПАКЕТЫ**

### **1. Unity Test Framework (com.unity.test-framework)**
```json
{
  "com.unity.test-framework": "1.4.0"
}
```

**Назначение:** Фреймворк для написания и запуска тестов.

**Ключевые возможности:**
- **NUnit** - фреймворк тестирования
- **UnityTest** - тесты для Unity
- **Assert** - проверки в тестах
- **Test Runner** - запуск тестов

**Пример теста:**
```csharp
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class MovementSystemTests
{
    [Test]
    public void MovementSystem_ValidInput_MovesEntity()
    {
        // Arrange
        var entity = CreateTestEntity();
        var input = new float2(1, 0);
        
        // Act
        var movementSystem = new MovementSystem();
        movementSystem.ProcessInput(entity, input);
        
        // Assert
        Assert.Greater(entity.Position.x, 0);
    }
}
```

### **2. Code Coverage (com.unity.testtools.codecoverage)**
```json
{
  "com.unity.testtools.codecoverage": "1.3.0"
}
```

**Назначение:** Измерение покрытия кода тестами.

**Настройка:**
```json
// ProjectSettings/EditorSettings.asset
m_CodeCoverageEnabled: 1
m_CodeCoveragePackagePath: Packages/com.unity.testtools.codecoverage
```

## 🎨 **UI ПАКЕТЫ**

### **1. UI Toolkit (com.unity.ui.toolkit)**
```json
{
  "com.unity.ui.toolkit": "1.1.0"
}
```

**Назначение:** Современная система UI для Unity.

**Ключевые возможности:**
- **UXML** - разметка UI
- **USS** - стили UI
- **C#** - логика UI
- **UI Builder** - визуальный редактор

**Пример UI:**
```xml
<!-- MainMenu.uxml -->
<ui:UXML>
    <ui:VisualElement name="MainMenu">
        <ui:Button name="StartGame" text="Start Game" />
        <ui:Button name="Settings" text="Settings" />
        <ui:Button name="Exit" text="Exit" />
    </ui:VisualElement>
</ui:UXML>
```

### **2. UI Builder (com.unity.ui.builder)**
```json
{
  "com.unity.ui.builder": "1.1.0"
}
```

**Назначение:** Визуальный редактор для UI Toolkit.

**Возможности:**
- **Drag & Drop** - перетаскивание элементов
- **Property Inspector** - настройка свойств
- **Live Preview** - предварительный просмотр
- **Code Generation** - генерация кода

## 🔧 **УТИЛИТЫ**

### **1. Unity Input System (com.unity.inputsystem)**
```json
{
  "com.unity.inputsystem": "1.6.0"
}
```

**Назначение:** Современная система ввода Unity.

**Настройка:**
```csharp
// PlayerInput.cs
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    private InputAction moveAction;
    private InputAction jumpAction;
    
    void Start()
    {
        moveAction = new InputAction("Move", InputActionType.Value);
        jumpAction = new InputAction("Jump", InputActionType.Button);
        
        moveAction.Enable();
        jumpAction.Enable();
    }
}
```

### **2. Unity Mathematics (com.unity.mathematics)**
```json
{
  "com.unity.mathematics": "1.3.0"
}
```

**Назначение:** Высокопроизводительные математические операции.

**Ключевые возможности:**
- **float3, float4** - векторы
- **quaternion** - кватернионы
- **math** - математические функции
- **SIMD** - векторные операции

## 📋 **ПОЛНАЯ КОНФИГУРАЦИЯ**

### **manifest.json**
```json
{
  "dependencies": {
    "com.unity.2d.sprite": "1.0.0",
    "com.unity.2d.tilemap": "1.0.0",
    "com.unity.ads": "4.12.0",
    "com.unity.ai.navigation": "2.0.9",
    "com.unity.analytics": "3.8.1",
    "com.unity.burst": "1.8.24",
    "com.unity.collab-proxy": "2.8.2",
    "com.unity.collections": "2.5.7",
    "com.unity.entities": "1.3.14",
    "com.unity.entities.graphics": "1.4.12",
    "com.unity.ide.rider": "3.0.37",
    "com.unity.ide.visualstudio": "2.0.23",
    "com.unity.inputsystem": "1.14.2",
    "com.unity.mathematics": "1.3.2",
    "com.unity.multiplayer.center": "1.0.0",
    "com.unity.netcode": "1.6.2",
    "com.unity.physics": "1.3.14",
    "com.unity.render-pipelines.core": "17.2.0",
    "com.unity.render-pipelines.universal": "17.2.0",
    "com.unity.shadergraph": "17.2.0",
    "com.unity.test-framework": "1.5.1",
    "com.unity.testtools.codecoverage": "1.2.6",
    "com.unity.timeline": "1.8.9",
    "com.unity.toolchain.linux-x86_64": "2.0.10",
    "com.unity.ugui": "2.0.0",
    "com.unity.ui.builder": "2.0.0",
    "com.unity.visualscripting": "1.9.8",
    "com.unity.xr.legacyinputhelpers": "2.1.12",
    "com.unity.modules.accessibility": "1.0.0",
    "com.unity.modules.ai": "1.0.0",
    "com.unity.modules.androidjni": "1.0.0",
    "com.unity.modules.animation": "1.0.0",
    "com.unity.modules.assetbundle": "1.0.0",
    "com.unity.modules.audio": "1.0.0",
    "com.unity.modules.cloth": "1.0.0",
    "com.unity.modules.director": "1.0.0",
    "com.unity.modules.imageconversion": "1.0.0",
    "com.unity.modules.imgui": "1.0.0",
    "com.unity.modules.jsonserialize": "1.0.0",
    "com.unity.modules.particlesystem": "1.0.0",
    "com.unity.modules.physics": "1.0.0",
    "com.unity.modules.physics2d": "1.0.0",
    "com.unity.modules.screencapture": "1.0.0",
    "com.unity.modules.terrain": "1.0.0",
    "com.unity.modules.terrainphysics": "1.0.0",
    "com.unity.modules.tilemap": "1.0.0",
    "com.unity.modules.ui": "1.0.0",
    "com.unity.modules.uielements": "1.0.0",
    "com.unity.modules.umbra": "1.0.0",
    "com.unity.modules.unityanalytics": "1.0.0",
    "com.unity.modules.unitywebrequest": "1.0.0",
    "com.unity.modules.unitywebrequestassetbundle": "1.0.0",
    "com.unity.modules.unitywebrequestaudio": "1.0.0",
    "com.unity.modules.unitywebrequesttexture": "1.0.0",
    "com.unity.modules.unitywebrequestwww": "1.0.0",
    "com.unity.modules.vehicles": "1.0.0",
    "com.unity.modules.video": "1.0.0",
    "com.unity.modules.vr": "1.0.0",
    "com.unity.modules.wind": "1.0.0",
    "com.unity.modules.xr": "1.0.0"
  }
}
```

### **packages-lock.json**
```json
{
  "dependencies": {
    "com.unity.entities": {
      "version": "1.0.0",
      "depth": 0,
      "source": "registry",
      "dependencies": {
        "com.unity.collections": "2.1.0",
        "com.unity.jobs": "1.0.0",
        "com.unity.burst": "1.8.12"
      }
    }
  }
}
```

## ⚠️ **ВАЖНЫЕ ЗАМЕЧАНИЯ**

### **1. Совместимость версий**
- **Entities 1.0.0** требует **Collections 1.4.0**
- **Physics 1.0.0** требует **Entities 1.0.0**
- **Netcode 1.0.0** требует **Entities 1.0.0**

### **2. Порядок установки**
1. **DOTS Core** (Entities, Collections, Jobs, Burst)
2. **Physics** (Physics, Rendering Hybrid)
3. **Networking** (Netcode for Entities)
4. **Testing** (Test Framework, Code Coverage)
5. **UI** (UI Toolkit, UI Builder)

### **3. Настройка Assembly Definitions**
```json
// Mud-Like.Core.asmdef
{
  "name": "Mud-Like.Core",
  "references": [
    "Unity.Entities",
    "Unity.Collections",
    "Unity.Jobs",
    "Unity.Burst"
  ],
  "allowUnsafeCode": true
}
```

## 🎯 **ПРОВЕРКА КОНФИГУРАЦИИ**

### **1. Проверка установки**
- [ ] Все пакеты установлены без ошибок
- [ ] Нет конфликтов версий
- [ ] Assembly Definitions настроены
- [ ] Проект компилируется без ошибок

### **2. Проверка функциональности**
- [ ] ECS системы работают
- [ ] Physics симуляция работает
- [ ] Сетевая функциональность работает
- [ ] Тесты запускаются
- [ ] UI отображается корректно

---

**Правильная конфигурация пакетов критически важна для стабильной работы Mud-Like. Следуйте этому руководству для настройки всех необходимых пакетов и их зависимостей.**
