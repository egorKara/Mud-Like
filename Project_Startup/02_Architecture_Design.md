# 🏗️ Mud-Like Architecture Design

## 🎯 **АРХИТЕКТУРНЫЕ ПРИНЦИПЫ**

### **1. ECS-архитектура (ОБЯЗАТЕЛЬНА)**
- **ECS-архитектура (обязательна)** - НИКАКИХ MonoBehaviour в игровом коде
- **Детерминизм (критически важен)** для мультиплеера
- **Производительность** через DOTS
- **Масштабируемость** для множества игроков

### **2. Принцип "Сложное из простого"**
- **Сложная система** = множество простых модулей
- **Каждый модуль** понятен и тестируем
- **Связи между модулями** минимальны
- **DOTS архитектура** строится из простых ECS компонентов

### **3. Data-Oriented Design**
- **Разделение данных и логики** (DOTS)
- **Модульность** и тестируемость (100% покрытие)
- **Независимость** от фреймворков
- **Гибкость** для изменений

## 🏛️ **ОБЩАЯ АРХИТЕКТУРА**

### **Слои архитектуры**

```
┌─────────────────────────────────────────┐
│              Presentation Layer          │
│  (UI, Input, Rendering, Audio)          │
├─────────────────────────────────────────┤
│              Application Layer           │
│  (Game Logic, Systems, Controllers)     │
├─────────────────────────────────────────┤
│              Domain Layer                │
│  (Entities, Components, Business Logic) │
├─────────────────────────────────────────┤
│              Infrastructure Layer        │
│  (Physics, Networking, Persistence)     │
└─────────────────────────────────────────┘
```

### **ECS компоненты (Domain Layer)**

#### **Базовые компоненты**
```csharp
// Позиция в мире
public struct Position : IComponentData
{
    public float3 Value;
}

// Скорость движения
public struct Velocity : IComponentData
{
    public float3 Value;
}

// Поворот объекта
public struct Rotation : IComponentData
{
    public quaternion Value;
}

// Здоровье объекта
public struct Health : IComponentData
{
    public float Value;
    public float MaxValue;
}
```

#### **Компоненты игрока**
```csharp
// Тег игрока
public struct PlayerTag : IComponentData {}

// Ввод игрока
public struct PlayerInput : IComponentData
{
    public float2 Movement;
    public bool Jump;
    public bool Brake;
}

// Сетевая позиция
public struct NetworkPosition : IComponentData
{
    public float3 Value;
}
```

#### **Компоненты транспорта**
```csharp
// Данные колеса
public struct WheelData : IComponentData
{
    public float3 Position;
    public float Radius;
    public float Width;
    public float SuspensionLength;
    public float SuspensionForce;
}

// Данные подвески
public struct SuspensionData : IComponentData
{
    public float SpringForce;
    public float DampingForce;
    public float TargetPosition;
}
```

#### **Компоненты деформации**
```csharp
// Данные деформации
public struct DeformationData : IComponentData
{
    public float depth;
    public float radius;
    public float3 position;
}

// Данные террейна
public struct TerrainData : IComponentData
{
    public float3 worldPosition;
    public float height;
    public float mudLevel;
}
```

## 🔧 **СИСТЕМЫ (Application Layer)**

### **Системы движения**
```csharp
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public class PlayerMovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        // Обработка ввода и движение игрока
        Entities
            .WithAll<PlayerTag>()
            .ForEach((ref Position position, ref Velocity velocity, in PlayerInput input) =>
            {
                // Логика движения
                float3 movement = new float3(input.Movement.x, 0, input.Movement.y);
                velocity.Value = movement * 5f;
                position.Value += velocity.Value * Time.fixedDeltaTime;
            }).Schedule();
    }
}
```

### **Системы физики**
```csharp
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public class WheelPhysicsSystem : SystemBase
{
    protected override void OnUpdate()
    {
        // Физика колес и подвески
        Entities
            .WithAll<WheelData>()
            .ForEach((ref WheelData wheel, ref SuspensionData suspension) =>
            {
                // Логика подвески
                float springForce = suspension.SpringForce * (suspension.TargetPosition - wheel.Position.y);
                float dampingForce = suspension.DampingForce * wheel.Velocity.y;
                // Применение сил
            }).Schedule();
    }
}
```

### **Системы деформации**
```csharp
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public class TerrainDeformationSystem : SystemBase
{
    protected override void OnUpdate()
    {
        // Деформация террейна
        Entities
            .WithAll<DeformationData>()
            .ForEach((in DeformationData deformation) =>
            {
                // Применение деформации к террейну
                ApplyDeformation(deformation.position, deformation.radius, deformation.depth);
            }).Schedule();
    }
}
```

### **Сетевые системы**
```csharp
[UpdateAfter(typeof(PlayerMovementSystem))]
public class SendPositionToServerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        // Отправка позиции на сервер
        var networkManager = GetSingleton<NetworkManager>();
        Entities
            .WithAll<PlayerTag, IsClient>()
            .ForEach((ref NetworkPosition position) =>
            {
                if (networkManager.IsConnectedClient)
                {
                    // Отправка команды на сервер
                    var command = new SetNetworkPositionCommand
                    {
                        Position = position.Value
                    };
                    PostUpdateCommands.AddComponent<SetNetworkPositionCommand>(entity, command);
                }
            }).Schedule();
    }
}
```

## 🌐 **СЕТЕВАЯ АРХИТЕКТУРА**

### **Client-Server модель**
```
┌─────────────┐    ┌─────────────┐    ┌─────────────┐
│   Client 1  │    │   Client 2  │    │   Client N  │
│             │    │             │    │             │
│  ECS World  │    │  ECS World  │    │  ECS World  │
└──────┬──────┘    └──────┬──────┘    └──────┬──────┘
       │                  │                  │
       └──────────────────┼──────────────────┘
                          │
                   ┌──────▼──────┐
                   │   Server    │
                   │             │
                   │  ECS World  │
                   │ (Authority) │
                   └─────────────┘
```

### **Команды и события**
```csharp
// Команда от клиента к серверу
public struct SetNetworkPositionCommand : ICommandTargetData
{
    public NetworkId Target;
    public float3 Position;
}

// Событие от сервера к клиентам
public struct SetNetworkPositionEvent : IEventData
{
    public NetworkId Sender;
    public float3 Position;
}
```

## 📁 **СТРУКТУРА ПРОЕКТА**

### **Обязательные директории**
```
Assets/
├── Scripts/
│   ├── Core/           # Основные системы
│   ├── Vehicles/       # Транспорт (ECS + Unity Physics)
│   ├── Terrain/        # Террейн и деформация (DOTS)
│   ├── UI/             # Пользовательский интерфейс
│   ├── Pooling/        # Object Pooling
│   ├── Networking/     # Мультиплеер (Netcode)
│   ├── Audio/          # Звуковая система
│   ├── Effects/        # Визуальные эффекты
│   ├── Tests/          # Тесты
│   └── DOTS/           # DOTS системы (активные)
├── Prefabs/            # Префабы
├── Materials/          # Материалы
├── Textures/           # Текстуры
├── Audio/              # Аудио файлы
├── Scenes/             # Сцены
└── Tests/              # Тестовые сцены
```

## 🔄 **ПОТОК ДАННЫХ**

### **1. Ввод игрока**
```
Input → PlayerInput Component → Movement System → Position Component
```

### **2. Физика транспорта**
```
WheelData → Physics System → SuspensionData → Position Update
```

### **3. Деформация террейна**
```
DeformationData → Terrain System → TerrainData → Visual Update
```

### **4. Сетевая синхронизация**
```
Client Input → Network Command → Server Processing → Network Event → Client Update
```

## ⚡ **ПРОИЗВОДИТЕЛЬНОСТЬ**

### **DOTS оптимизации**
- **Burst Compiler** для вычислений
- **Job System** для параллельных операций
- **NativeArray<T>** для управления памятью
- **Entity Component System** для кэширования

### **Сетевые оптимизации**
- **Сжатие данных** для уменьшения трафика
- **Предсказание** для плавности
- **Адаптивная синхронизация** по важности
- **Lag Compensation** для честности

## 🧪 **ТЕСТИРОВАНИЕ**

### **Многоуровневое тестирование**
- **Unit тесты** для изолированной логики
- **Integration тесты** для взаимодействия компонентов
- **Functional тесты** для полных рабочих процессов
- **Performance тесты** для анализа производительности

### **Тестирование детерминизма**
- **unity-deterministic-physics** для кроссплатформенного тестирования
- **Идентичные результаты** на разных клиентах
- **Порядок выполнения систем** для детерминизма

## 🎯 **МИГРАЦИЯ НА ECS**

### **План миграции**
1. **Базовые компоненты** (Position, Velocity, Rotation)
2. **Критические модули** (движение, ввод)
3. **Физические системы** (транспорт, деформация)
4. **Сетевые системы** (синхронизация, репликация)

### **Итерационный подход**
- **Постепенная миграция** модулей
- **Тестирование** на каждом этапе
- **Профилирование** производительности
- **Качество** с первого дня

---

**Архитектура Mud-Like построена на принципах ECS, детерминизма и производительности. Каждый компонент и система имеют четкую ответственность, что обеспечивает масштабируемость и тестируемость проекта.**
