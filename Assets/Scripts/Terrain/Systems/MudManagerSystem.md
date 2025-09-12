# 🌍 MudManagerSystem - API деформации террейна

## 📋 **ОБЗОР СИСТЕМЫ**

`MudManagerSystem` - это центральная система управления грязью и деформацией террейна в проекте Mud-Like. Система предоставляет API для взаимодействия колес транспорта с грязью, обеспечивая реалистичную физику и детерминированную симуляцию.

## 🎯 **КЛЮЧЕВЫЕ ВОЗМОЖНОСТИ**

### **1. QueryContact API**
Основной метод системы - `QueryContact(wheelPosition, radius, wheelForce)` - возвращает данные о взаимодействии колеса с грязью.

```csharp
// Пример использования QueryContact API
var mudManager = SystemAPI.GetSingleton<MudManagerSystem>();
var contactData = mudManager.QueryContact(wheelPosition, wheelRadius, wheelForce);

// Использование результатов
if (contactData.IsValid)
{
    float sinkDepth = contactData.SinkDepth;
    float tractionModifier = contactData.TractionModifier;
    float drag = contactData.Drag;
    SurfaceType surfaceType = contactData.SurfaceType;
}
```

### **2. Физические расчеты**
Система выполняет комплексные физические расчеты:

```csharp
// Расчет глубины погружения
float sinkDepth = CalculateSinkDepth(wheelPosition, radius, wheelForce, mudLevel, terrainData);

// Расчет модификатора тяги
float tractionModifier = CalculateTractionModifier(sinkDepth, mudLevel, terrainData);

// Расчет сопротивления
float drag = CalculateDrag(sinkDepth, mudLevel, terrainData);
```

### **3. Типы поверхностей**
Система поддерживает 7 типов поверхностей с разными свойствами:

```csharp
public enum SurfaceType
{
    Rock,        // Камень - высокая твердость, отличная тяга
    DryGround,   // Сухая земля - хорошая тяга
    WetGround,   // Мокрая земля - средняя тяга
    Mud,         // Грязь - низкая тяга, высокое сопротивление
    DeepMud,     // Глубокая грязь - очень низкая тяга
    Water,       // Вода - минимальная тяга
    Sand         // Песок - переменная тяга
}
```

## 🏗️ **АРХИТЕКТУРА СИСТЕМЫ**

### **Компоненты системы:**
```csharp
public partial class MudManagerSystem : SystemBase
{
    private TerrainHeightManager _terrainManager;           // Менеджер высот террейна
    private NativeHashMap<int, MudContactData> _mudContacts; // Кэш контактов с грязью
}
```

### **Основные методы:**
```csharp
// Основной API метод
public MudContactData QueryContact(float3 wheelPosition, float radius, float wheelForce)

// Вспомогательные методы
private TerrainData GetTerrainDataAtPosition(float3 position)
private float CalculateMudLevel(float3 position, float radius, TerrainData terrainData)
private float CalculateSinkDepth(float3 position, float radius, float wheelForce, float mudLevel, TerrainData terrainData)
private float CalculateTractionModifier(float sinkDepth, float mudLevel, TerrainData terrainData)
private float CalculateDrag(float sinkDepth, float mudLevel, TerrainData terrainData)
```

## 🔧 **ИНТЕГРАЦИЯ С ДРУГИМИ СИСТЕМАМИ**

### **1. Интеграция с TerrainHeightManager:**
```csharp
protected override void OnCreate()
{
    _terrainManager = SystemAPI.GetSingleton<TerrainHeightManager>();
}

// Использование в расчетах
float height = _terrainManager.GetChunkHeight(chunkIndex, position.x, position.z);
float mudLevel = _terrainManager.GetChunkMudLevel(chunkIndex, position.x, position.z);
```

### **2. Интеграция с VehiclePhysicsSystem:**
```csharp
// В VehiclePhysicsSystem
var mudManager = SystemAPI.GetSingleton<MudManagerSystem>();
var contactData = mudManager.QueryContact(wheelPosition, wheelRadius, wheelForce);

// Применение результатов к физике
wheelPhysics.Traction *= contactData.TractionModifier;
wheelPhysics.Drag += contactData.Drag;
wheelPhysics.SinkDepth = contactData.SinkDepth;
```

### **3. Интеграция с TerrainSyncSystem:**
```csharp
// Синхронизация данных деформации
var syncSystem = SystemAPI.GetSingleton<TerrainSyncSystem>();
syncSystem.SyncTerrainDeformation(deformationData, isAuthoritative);
```

## 📊 **ДАННЫЕ КОНТАКТА С ГРЯЗЬЮ**

### **MudContactData структура:**
```csharp
public struct MudContactData
{
    public float3 Position;           // Позиция контакта
    public float Radius;              // Радиус области
    public float MudLevel;            // Уровень грязи (0-1)
    public float SinkDepth;           // Глубина погружения
    public float TractionModifier;    // Модификатор тяги (0-1)
    public float Drag;                // Сопротивление движению
    public SurfaceType SurfaceType;   // Тип поверхности
    public bool IsValid;              // Валидность данных
    public float LastUpdateTime;      // Время последнего обновления
}
```

### **Пример использования данных:**
```csharp
var contactData = mudManager.QueryContact(wheelPosition, wheelRadius, wheelForce);

if (contactData.IsValid)
{
    // Применяем модификатор тяги
    float finalTraction = baseTraction * contactData.TractionModifier;
    
    // Применяем сопротивление
    float finalDrag = baseDrag + contactData.Drag;
    
    // Проверяем тип поверхности
    switch (contactData.SurfaceType)
    {
        case SurfaceType.DeepMud:
            // Дополнительные эффекты для глубокой грязи
            break;
        case SurfaceType.Water:
            // Эффекты для воды
            break;
    }
}
```

## ⚡ **ПРОИЗВОДИТЕЛЬНОСТЬ И ОПТИМИЗАЦИЯ**

### **1. Burst Compiler:**
```csharp
[BurstCompile]
public MudContactData QueryContact(float3 wheelPosition, float radius, float wheelForce)
{
    // Весь код метода оптимизирован Burst Compiler
}
```

### **2. Кэширование контактов:**
```csharp
private NativeHashMap<int, MudContactData> _mudContacts;

// Кэширование результатов для повторного использования
if (_mudContacts.TryGetValue(contactHash, out var cachedData))
{
    return cachedData;
}
```

### **3. Оптимизированные алгоритмы:**
```csharp
// Семплирование грязи в радиусе колеса
float sampleStep = radius * 0.5f;
for (float x = position.x - radius; x <= position.x + radius; x += sampleStep)
{
    // Оптимизированный цикл семплирования
}
```

## 🧪 **ТЕСТИРОВАНИЕ СИСТЕМЫ**

### **Unit тесты:**
```csharp
[Test]
public void QueryContact_ValidInput_ReturnsValidData()
{
    // Arrange
    var mudManager = new MudManagerSystem();
    float3 position = new float3(0, 0, 0);
    float radius = 1f;
    float force = 100f;
    
    // Act
    var result = mudManager.QueryContact(position, radius, force);
    
    // Assert
    Assert.IsTrue(result.IsValid);
    Assert.Greater(result.TractionModifier, 0f);
    Assert.LessOrEqual(result.TractionModifier, 1f);
}
```

### **Integration тесты:**
```csharp
[Test]
public void MudManager_WithVehiclePhysics_AppliesCorrectModifiers()
{
    // Тест интеграции с VehiclePhysicsSystem
}
```

## 🚨 **ВАЖНЫЕ ЗАМЕЧАНИЯ**

### **1. Детерминизм:**
- Все расчеты детерминированы для мультиплеера
- Используется `SystemAPI.Time.fixedDeltaTime` вместо `Time.deltaTime`

### **2. Память:**
- Используются `NativeArray` и `NativeHashMap` для эффективности
- Автоматическая очистка ресурсов в `OnDestroy()`

### **3. Производительность:**
- Методы помечены `[BurstCompile]` для оптимизации
- Кэширование результатов для избежания повторных вычислений

## 📚 **СВЯЗАННАЯ ДОКУМЕНТАЦИЯ**

- [TerrainHeightManager.md](./TerrainHeightManager.md) - Менеджер высот террейна
- [TerrainSyncSystem.md](./TerrainSyncSystem.md) - Синхронизация террейна
- [VehiclePhysicsSystem.md](../Vehicles/Systems/VehiclePhysicsSystem.md) - Физика транспорта
- [WorldGridSystem.md](./WorldGridSystem.md) - Система сетки мира
