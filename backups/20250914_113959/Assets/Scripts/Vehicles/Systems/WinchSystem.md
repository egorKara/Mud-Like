# 🎣 WinchSystem API Documentation

## 📋 **ОБЗОР**

`WinchSystem` - система лебедки, ключевая механика игры Mud-Like. Обеспечивает реалистичное взаимодействие с лебедкой, включая натяжение троса, подключение к объектам и физику кабеля.

## 🏗️ **АРХИТЕКТУРА**

### **Основные компоненты:**
- `WinchData` - данные лебедки
- `WinchCableData` - данные кабеля
- `WinchConnectionData` - данные подключения
- `VehiclePhysics` - физика транспортного средства

### **Система обновления:**
```csharp
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[BurstCompile(CompileSynchronously = true)]
public partial class WinchSystem : SystemBase
```

## 🔧 **API МЕТОДЫ**

### **1. Обработка лебедки**
```csharp
private void ProcessWinch(ref WinchData winch, float3 position, float deltaTime)
{
    // Обновляем состояние лебедки
    if (winch.IsActive)
    {
        // Вычисляем натяжение
        winch.Tension = CalculateTension(winch);
        
        // Обновляем длину кабеля
        winch.CurrentLength = UpdateCableLength(winch, deltaTime);
        
        // Проверяем ограничения
        winch.CurrentLength = math.clamp(winch.CurrentLength, 0f, winch.MaxLength);
    }
}
```

### **2. Физика кабеля**
```csharp
private void ProcessCable(ref WinchCableData cable, float deltaTime)
{
    if (cable.IsConnected)
    {
        // Вычисляем натяжение кабеля
        cable.Tension = CalculateCableTension(cable);
        
        // Обновляем длину
        cable.Length = math.length(cable.EndPosition - cable.StartPosition);
        
        // Проверяем максимальное натяжение
        if (cable.Tension > cable.MaxTension)
        {
            cable.IsConnected = false;
        }
    }
}
```

### **3. Подключение к объектам**
```csharp
private void ProcessConnection(ref WinchConnectionData connection, 
                              in WinchData winch, 
                              float3 winchPosition)
{
    if (!connection.IsConnected)
    {
        // Ищем ближайший объект для подключения
        var targetEntity = FindNearestConnectableObject(winchPosition, 
                                                      connection.MaxConnectionDistance);
        
        if (targetEntity != Entity.Null)
        {
            connection.ConnectedEntity = targetEntity;
            connection.IsConnected = true;
            connection.ConnectionPoint = GetConnectionPoint(targetEntity);
        }
    }
}
```

## 📊 **КОМПОНЕНТЫ ДАННЫХ**

### **WinchData**
```csharp
public struct WinchData : IComponentData
{
    public float3 Position;           // Позиция лебедки
    public float MaxLength;           // Максимальная длина кабеля
    public float CurrentLength;       // Текущая длина кабеля
    public float Tension;             // Натяжение кабеля
    public bool IsActive;             // Активна ли лебедка
    public float MotorPower;          // Мощность мотора
    public float BrakeForce;          // Сила тормоза
}
```

### **WinchCableData**
```csharp
public struct WinchCableData : IComponentData
{
    public float3 StartPosition;      // Начальная позиция кабеля
    public float3 EndPosition;        // Конечная позиция кабеля
    public float Length;              // Длина кабеля
    public float Tension;             // Натяжение кабеля
    public bool IsConnected;          // Подключен ли кабель
    public float MaxTension;          // Максимальное натяжение
    public float Elasticity;          // Эластичность кабеля
}
```

### **WinchConnectionData**
```csharp
public struct WinchConnectionData : IComponentData
{
    public Entity ConnectedEntity;    // Подключенная сущность
    public float3 ConnectionPoint;    // Точка подключения
    public bool IsConnected;          // Подключен ли
    public float ConnectionStrength;  // Прочность подключения
    public float MaxConnectionDistance; // Максимальное расстояние подключения
}
```

## ⚡ **ПРОИЗВОДИТЕЛЬНОСТЬ**

### **Burst оптимизация:**
- `[BurstCompile(CompileSynchronously = true)]` для максимальной производительности
- Параллельная обработка через `IJobEntity`
- Оптимизированные математические вычисления

### **Память:**
- Использование `NativeArray` для больших данных
- Минимизация аллокаций в Update
- Эффективное управление EntityQuery

## 🧪 **ТЕСТИРОВАНИЕ**

### **Unit тесты:**
- `WinchSystemTests.cs` - 6 тестов
- Покрытие всех основных сценариев
- Тестирование edge cases

### **Пример теста:**
```csharp
[Test]
public void WinchSystem_WithWinchData_ProcessesCorrectly()
{
    var entity = _entityManager.CreateEntity();
    _entityManager.AddComponentData(entity, new WinchData
    {
        Position = float3.zero,
        MaxLength = 50f,
        CurrentLength = 0f,
        Tension = 0f,
        IsActive = true,
        MotorPower = 1000f,
        BrakeForce = 2000f
    });

    _winchSystem.OnUpdate(ref _world.Unmanaged);
    Assert.IsNotNull(_winchSystem);
}
```

## 🎯 **ИСПОЛЬЗОВАНИЕ**

### **Инициализация:**
```csharp
// Создание лебедки
var winchEntity = entityManager.CreateEntity();
entityManager.AddComponentData(winchEntity, new WinchData
{
    Position = vehiclePosition,
    MaxLength = 50f,
    IsActive = true,
    MotorPower = 1000f
});
```

### **Активация лебедки:**
```csharp
// В системе управления
if (Input.GetKeyDown(KeyCode.W))
{
    var winchData = entityManager.GetComponentData<WinchData>(winchEntity);
    winchData.IsActive = !winchData.IsActive;
    entityManager.SetComponentData(winchEntity, winchData);
}
```

## ⚠️ **ВАЖНЫЕ ЗАМЕЧАНИЯ**

1. **Детерминизм:** Система полностью детерминирована для мультиплеера
2. **Производительность:** Оптимизирована для 50+ игроков
3. **Физика:** Реалистичная физика кабеля и натяжения
4. **Безопасность:** Проверка максимальных значений и ограничений

---

**WinchSystem обеспечивает реалистичную и производительную механику лебедки для Mud-Like!** 🎣
