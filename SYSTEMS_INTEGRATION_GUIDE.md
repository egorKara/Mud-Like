# 🔗 Systems Integration Guide

## 📋 **РУКОВОДСТВО ПО ИНТЕГРАЦИИ СИСТЕМ**

Документ описывает правильную интеграцию всех систем проекта Mud-Like, порядок их выполнения и зависимости между компонентами.

## 🎯 **АРХИТЕКТУРА ИНТЕГРАЦИИ**

### **Поток данных между системами:**
```
🎮 Игровой цикл
├── 📥 Ввод (VehicleInputSystem)
├── 🎯 Управление (VehicleControlSystem)
├── ⚙️ Двигатель (EngineSystem)
├── 🔄 Трансмиссия (TransmissionSystem)
├── 🚗 Физика (VehiclePhysicsSystem)
└── 📹 Камера (VehicleCameraSystem)
```

## 🔧 **ПОРЯДОК ВЫПОЛНЕНИЯ СИСТЕМ**

### **1. InitializationSystemGroup (Инициализация)**
```csharp
// Порядок выполнения в InitializationSystemGroup
1. GameBootstrapSystem      // Создание игроков и начального состояния
2. VehicleInputSystem       // Обработка ввода Unity → PlayerInput
3. VehicleSpawningSystem    // Создание транспорта для игроков
4. SceneManagementSystem    // Управление сценами
```

### **2. FixedStepSimulationSystemGroup (Основная симуляция)**
```csharp
// Порядок выполнения в FixedStepSimulationSystemGroup
1. VehicleControlSystem     // Управление транспортом (PlayerInput → VehiclePhysics)
2. EngineSystem            // Работа двигателя (PlayerInput → EngineData)
3. TransmissionSystem      // Трансмиссия (PlayerInput → TransmissionData)
4. VehiclePhysicsSystem    // Физика транспорта (EngineData + TransmissionData → VehiclePhysics)
```

### **3. LateSimulationSystemGroup (Поздняя симуляция)**
```csharp
// Порядок выполнения в LateSimulationSystemGroup
1. VehicleCameraSystem     // Камера транспорта (PlayerInput + VehiclePhysics → Camera)
```

## 📊 **КОМПОНЕНТЫ И ИХ ЗАВИСИМОСТИ**

### **PlayerInput - Единый компонент ввода**
```csharp
/// <summary>
/// Компонент ввода игрока для управления транспортом
/// Игрок управляет только транспортом, не ходит пешком
/// </summary>
public struct PlayerInput : IComponentData
{
    // Движение транспорта (WASD)
    public float2 VehicleMovement;
    
    // Ускорение и торможение
    public bool Accelerate;
    public bool Brake;
    public bool Handbrake;
    public float Steering;
    
    // Дополнительные действия
    public bool Action1;        // E - лебедка
    public bool Action2;        // Tab - камера
    public bool Action3;        // F - полный привод
    public bool Action4;        // G - блокировка дифференциала
    
    // Управление камерой
    public float2 CameraLook;   // Поворот камеры мышью
    public float CameraZoom;    // Зум камеры колесиком мыши
    
    // Функции транспорта
    public bool EngineToggle;   // Включение/выключение двигателя
    public bool ShiftUp;        // Переключение передачи вверх
    public bool ShiftDown;      // Переключение передачи вниз
    public bool Neutral;        // Нейтральная передача
}
```

### **VehiclePhysics - Центральный компонент физики**
```csharp
/// <summary>
/// Физика транспортного средства
/// </summary>
public struct VehiclePhysics : IComponentData
{
    public float3 Velocity;           // Скорость транспорта
    public float3 AngularVelocity;    // Угловая скорость
    public float Mass;               // Масса транспорта
    public float MaxSpeed;           // Максимальная скорость
    public float Acceleration;       // Ускорение
    public float Deceleration;       // Торможение
    public float EnginePower;        // Мощность двигателя
    public float MaxEnginePower;     // Максимальная мощность
    public float SteeringAngle;      // Угол поворота руля
    public float MaxSteeringAngle;   // Максимальный угол поворота
    public float TurnSpeedMultiplier; // Множитель скорости поворота
    public float EngineBraking;      // Торможение двигателем
    public float SteeringReturnSpeed; // Скорость возврата руля
    public quaternion Rotation;      // Поворот транспорта
}
```

## 🔄 **ИНТЕГРАЦИЯ СИСТЕМ**

### **1. VehicleInputSystem → VehicleControlSystem**
```csharp
// VehicleInputSystem обрабатывает Unity Input
private void ProcessVehicleInput(ref PlayerInput playerInput)
{
    playerInput.VehicleMovement = new float2(
        Input.GetAxis("Horizontal"),    // A/D - руль
        Input.GetAxis("Vertical")       // W/S - газ/тормоз
    );
    
    playerInput.Accelerate = Input.GetKey(KeyCode.W);
    playerInput.Brake = Input.GetKey(KeyCode.S);
    playerInput.Steering = Input.GetAxis("Horizontal");
}

// VehicleControlSystem использует PlayerInput
Entities
    .WithAll<VehicleTag, PlayerTag>()
    .ForEach((ref LocalTransform transform, in PlayerInput input, ref VehiclePhysics physics) =>
    {
        ProcessVehicleControl(ref transform, input, ref physics, deltaTime);
    }).Schedule();
```

### **2. VehicleControlSystem → EngineSystem**
```csharp
// VehicleControlSystem обновляет VehiclePhysics
private static void ApplyVehicleInput(ref VehiclePhysics physics, in PlayerInput input, float deltaTime)
{
    if (input.Accelerate)
    {
        physics.EnginePower = math.min(physics.EnginePower + physics.Acceleration * deltaTime, physics.MaxEnginePower);
    }
}

// EngineSystem использует PlayerInput и обновляет EngineData
Entities
    .WithAll<VehicleTag>()
    .ForEach((ref EngineData engine, ref VehiclePhysics physics, in PlayerInput input, in VehicleConfig config) =>
    {
        ProcessEngine(ref engine, ref physics, input, config, deltaTime);
    }).Schedule();
```

### **3. EngineSystem → TransmissionSystem**
```csharp
// EngineSystem обновляет EngineData
private static void ProcessEngine(ref EngineData engine, ref VehiclePhysics physics, in PlayerInput input, in VehicleConfig config, float deltaTime)
{
    if (input.EngineToggle)
    {
        engine.IsRunning = !engine.IsRunning;
    }
    
    // Обновляем RPM на основе мощности двигателя
    engine.RPM = engine.IsRunning ? engine.RPM + (physics.EnginePower * 100f * deltaTime) : 0f;
}

// TransmissionSystem использует EngineData
Entities
    .WithAll<VehicleTag>()
    .ForEach((ref TransmissionData transmission, ref VehiclePhysics physics, in PlayerInput input, in EngineData engine) =>
    {
        ProcessTransmission(ref transmission, ref physics, input, engine, deltaTime);
    }).Schedule();
```

### **4. VehiclePhysics → VehicleCameraSystem**
```csharp
// VehicleCameraSystem использует VehiclePhysics для позиционирования камеры
private void UpdateCameraPosition(in LocalTransform vehicleTransform, in VehiclePhysics physics, float deltaTime)
{
    // Предсказание движения транспорта
    float3 velocityDirection = math.normalize(physics.Velocity);
    float3 lookDirection = math.length(physics.Velocity) > 0.1f ? velocityDirection : forward;
    
    // Позиция камеры с учетом скорости
    targetPosition = vehicleTransform.Position + 
                   new float3(0f, _cameraSettings.ThirdPersonHeight, 0f) -
                   lookDirection * _cameraSettings.ThirdPersonDistance;
}
```

## 🚀 **СИСТЕМА БУТСТРАПА**

### **GameBootstrapSystem - Инициализация игры**
```csharp
/// <summary>
/// Система инициализации и загрузки игры
/// Обеспечивает правильный порядок загрузки и настройку начального состояния
/// </summary>
[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial class GameBootstrapSystem : SystemBase
{
    private void InitializeGame()
    {
        // Создаем игрока
        Entity playerEntity = CreatePlayer();
        
        // Настраиваем начальное состояние
        SetupInitialGameState();
    }
    
    private Entity CreatePlayer()
    {
        Entity playerEntity = EntityManager.CreateEntity();
        
        // Добавляем основные компоненты
        EntityManager.AddComponent<PlayerTag>(playerEntity);
        EntityManager.AddComponent<PlayerInput>(playerEntity);
        EntityManager.AddComponent<NetworkId>(playerEntity);
        EntityManager.AddComponent<LocalTransform>(playerEntity);
        
        return playerEntity;
    }
}
```

### **SystemOrderManager - Управление порядком**
```csharp
/// <summary>
/// Менеджер порядка выполнения систем
/// Обеспечивает правильную последовательность выполнения систем ECS
/// </summary>
private void ConfigureSystemOrder()
{
    // Настраиваем порядок выполнения систем
    var bootstrapSystem = World.GetOrCreateSystemManaged<GameBootstrapSystem>();
    var inputSystem = World.GetOrCreateSystemManaged<VehicleInputSystem>();
    var spawningSystem = World.GetOrCreateSystemManaged<VehicleSpawningSystem>();
    var controlSystem = World.GetOrCreateSystemManaged<VehicleControlSystem>();
    var cameraSystem = World.GetOrCreateSystemManaged<VehicleCameraSystem>();
}
```

## 🧪 **ТЕСТИРОВАНИЕ ИНТЕГРАЦИИ**

### **Unit тесты интеграции:**
```csharp
[Test]
public void SystemsIntegration_InputToPhysics_WorksCorrectly()
{
    // Arrange
    var inputSystem = new VehicleInputSystem();
    var controlSystem = new VehicleControlSystem();
    
    // Act
    var input = new PlayerInput { Accelerate = true, Steering = 0.5f };
    var physics = new VehiclePhysics();
    
    controlSystem.ApplyVehicleInput(ref physics, input, deltaTime);
    
    // Assert
    Assert.Greater(physics.EnginePower, 0f);
    Assert.Greater(physics.SteeringAngle, 0f);
}

[Test]
public void SystemsIntegration_EngineToTransmission_WorksCorrectly()
{
    // Тест интеграции EngineSystem → TransmissionSystem
}
```

### **Integration тесты:**
```csharp
[Test]
public void SystemsIntegration_FullGameplayFlow_WorksEndToEnd()
{
    // Тест полного игрового цикла от ввода до камеры
}
```

## 🚨 **ВАЖНЫЕ ПРИНЦИПЫ**

### **1. Единый компонент ввода:**
- **PlayerInput** - единственный компонент ввода для всех систем
- **VehicleInput** удален** - больше не используется
- Все системы используют **PlayerInput**

### **2. Правильный порядок выполнения:**
- **InitializationSystemGroup** - инициализация и ввод
- **FixedStepSimulationSystemGroup** - основная симуляция
- **LateSimulationSystemGroup** - камера и презентация

### **3. Зависимости компонентов:**
- **PlayerInput** → **VehiclePhysics** → **EngineData** → **TransmissionData**
- **VehiclePhysics** → **VehicleCameraSystem**
- **NetworkId** → **VehicleSpawningSystem**

### **4. Инициализация:**
- **GameBootstrapSystem** создает игроков
- **VehicleSpawningSystem** создает транспорт
- **SystemOrderManager** настраивает порядок

## 📚 **СВЯЗАННАЯ ДОКУМЕНТАЦИЯ**

- [VehicleInputSystem.md](./Assets/Scripts/Input/Systems/VehicleInputSystem.md) - Система ввода
- [VehicleControlSystem.md](./Assets/Scripts/Core/Systems/VehicleControlSystem.md) - Управление транспортом
- [VehicleCameraSystem.md](./Assets/Scripts/Camera/Systems/VehicleCameraSystem.md) - Система камеры
- [VehicleSpawningSystem.md](./Assets/Scripts/Gameplay/Systems/VehicleSpawningSystem.md) - Спавн транспорта
- [SYSTEMS_INTEGRATION_ARCHITECTURE.md](./SYSTEMS_INTEGRATION_ARCHITECTURE.md) - Архитектура интеграции
