# 🛡️ InputValidationSystem - Валидация ввода для мультиплеера

## 📋 **ОБЗОР СИСТЕМЫ**

`InputValidationSystem` - это система серверной валидации ввода для мультиплеера в проекте Mud-Like. Система обеспечивает защиту от читов, валидацию действий игроков и обнаружение подозрительной активности.

## 🎯 **КЛЮЧЕВЫЕ ВОЗМОЖНОСТИ**

### **1. Валидация ввода игроков**
Основной метод системы - `ValidatePlayerInput(playerId, input, timestamp)` - проверяет корректность действий игрока.

```csharp
// Пример использования валидации ввода управления транспортом
var validationSystem = SystemAPI.GetSingleton<InputValidationSystem>();
var input = new PlayerInput
{
    VehicleMovement = new float2(0.5f, 0.3f),
    Accelerate = true,
    Brake = false,
    Handbrake = false,
    Steering = 0.2f,
    Action1 = false, // E - лебедка
    Action2 = false  // Tab - камера
};

var result = validationSystem.ValidatePlayerInput(playerId, input, SystemAPI.Time.time);

if (result.IsValid)
{
    // Применяем ввод игрока
    ApplyPlayerInput(playerId, input);
}
else
{
    // Обрабатываем невалидный ввод
    HandleInvalidInput(playerId, result.Reason, result.Details);
}
```

### **2. Rate Limiting (Ограничение частоты)**
Система предотвращает спам и слишком частые вводы:

```csharp
// Настройки rate limiting для управления транспортом
const float minInputInterval = 0.016f; // ~60 FPS
const int maxInputsPerSecond = 100;    // Максимум 100 вводов в секунду

// Проверка частоты ввода
float timeSinceLastInput = timestamp - validationData.LastInputTime;
if (timeSinceLastInput < minInputInterval)
{
    result.IsValid = false;
    result.Reason = ValidationReason.TooFrequentInput;
    return result;
}
```

### **3. Физическая валидация**
Проверка физической возможности действий:

```csharp
// Пример физической валидации для транспорта
private bool ValidatePhysicalPossibility(int playerId, PlayerInput input, ref ValidationResult result)
{
    var vehicleData = GetVehicleData(playerId);
    
    // Проверка застревания транспорта
    if (vehicleData.IsStuck && math.length(input.VehicleMovement) > 0.1f)
    {
        result.IsValid = false;
        result.Reason = ValidationReason.PhysicallyImpossible;
        result.Details = "Vehicle is stuck but trying to move";
        return false;
    }
    
    // Проверка скорости транспорта
    float maxPossibleSpeed = vehicleData.MaxSpeed * 1.1f; // 10% допуск
    float inputSpeed = math.length(input.VehicleMovement) * vehicleData.MaxSpeed;
    
    if (inputSpeed > maxPossibleSpeed)
    {
        result.IsValid = false;
        result.Reason = ValidationReason.PhysicallyImpossible;
        result.Details = $"Vehicle speed too high: {inputSpeed:F2} > {maxPossibleSpeed:F2}";
        return false;
    }
    
    // Проверка угла поворота руля
    if (math.abs(input.Steering) > vehicleData.MaxSteeringAngle)
    {
        result.IsValid = false;
        result.Reason = ValidationReason.PhysicallyImpossible;
        result.Details = $"Steering angle too high: {input.Steering:F2} > {vehicleData.MaxSteeringAngle:F2}";
        return false;
    }
    
    return true;
}
```

### **4. Поведенческий анализ**
Обнаружение ботов и автоматизации:

```csharp
// Обнаружение повторяющихся паттернов
private bool DetectRepeatingPatterns(PlayerInput currentInput, InputHistory history)
{
    const int patternLength = 3;
    int matches = 0;
    
    // Проверяем последние patternLength вводов на повторение
    for (int i = 0; i < patternLength; i++)
    {
        int currentIndex = (history.InputCount - 1 - i) % history.LastInputs.Length;
        int previousIndex = (history.InputCount - 1 - i - patternLength) % history.LastInputs.Length;
        
        if (InputsAreEqual(history.LastInputs[currentIndex], history.LastInputs[previousIndex]))
        {
            matches++;
        }
    }
    
    // Если большинство вводов повторяются - подозрительно
    return matches >= patternLength * 0.8f;
}

// Обнаружение автоматизированных движений
private bool DetectAutomatedMovement(PlayerInput currentInput, InputHistory history)
{
    // Проверяем на слишком точные углы (кратные 15, 30, 45 градусам)
    float angle = math.atan2(currentInput.Movement.y, currentInput.Movement.x);
    float degrees = math.degrees(angle);
    
    float tolerance = 1.0f;
    for (int i = 0; i <= 360; i += 15)
    {
        if (math.abs(degrees - i) < tolerance)
        {
            return true; // Подозрительно точный угол
        }
    }
    
    return false;
}
```

## 🏗️ **АРХИТЕКТУРА СИСТЕМЫ**

### **Компоненты системы:**
```csharp
public partial class InputValidationSystem : SystemBase
{
    private NativeHashMap<int, PlayerValidationData> _playerValidationData; // Данные валидации игроков
    private NativeHashMap<int, InputHistory> _inputHistory;                 // История ввода игроков
}
```

### **Основные методы:**
```csharp
// Основной API метод
public ValidationResult ValidatePlayerInput(int playerId, PlayerInput input, float timestamp)

// Вспомогательные методы валидации
private bool ValidateInputRate(int playerId, float timestamp, ref ValidationResult result)
private bool ValidateInputValues(PlayerInput input, ref ValidationResult result)
private bool ValidatePhysicalPossibility(int playerId, PlayerInput input, ref ValidationResult result)
private bool ValidateBehavioralPatterns(int playerId, PlayerInput input, ref ValidationResult result)

// Методы анализа поведения
private bool DetectRepeatingPatterns(PlayerInput currentInput, InputHistory history)
private bool DetectAutomatedMovement(PlayerInput currentInput, InputHistory history)
```

## 📊 **СТРУКТУРЫ ДАННЫХ**

### **ValidationResult:**
```csharp
public struct ValidationResult
{
    public bool IsValid;                    // Валидность ввода
    public ValidationReason Reason;         // Причина невалидности
    public FixedString128Bytes Details;     // Детали ошибки
}
```

### **ValidationReason (причины невалидности):**
```csharp
public enum ValidationReason : byte
{
    None,                    // Нет ошибок
    TooFrequentInput,        // Слишком частый ввод
    RateLimitExceeded,       // Превышен лимит частоты
    InvalidInputValues,      // Некорректные значения ввода
    PhysicallyImpossible,    // Физически невозможно
    SuspiciousBehavior,      // Подозрительное поведение
    PlayerNotFound          // Игрок не найден
}
```

### **PlayerValidationData:**
```csharp
public struct PlayerValidationData
{
    public float LastInputTime;           // Время последнего ввода
    public int InputCount;                // Количество вводов
    public int SuspiciousActivityCount;   // Счетчик подозрительной активности
    public bool IsBanned;                 // Статус бана
}
```

### **InputHistory:**
```csharp
public struct InputHistory
{
    public NativeArray<PlayerInput> LastInputs; // Последние вводы
    public int InputCount;                       // Количество вводов
    public float LastUpdateTime;                 // Время последнего обновления
}
```

## 🔧 **ИНТЕГРАЦИЯ С ДРУГИМИ СИСТЕМАМИ**

### **1. Интеграция с NetworkManagerSystem:**
```csharp
// В NetworkManagerSystem при получении ввода от клиента
var validationSystem = SystemAPI.GetSingleton<InputValidationSystem>();
var result = validationSystem.ValidatePlayerInput(clientId, input, timestamp);

if (result.IsValid)
{
    // Передаем ввод в игровую логику
    ProcessValidatedInput(clientId, input);
}
else
{
    // Отклоняем невалидный ввод
    RejectInvalidInput(clientId, result);
}
```

### **2. Интеграция с LagCompensationSystem:**
```csharp
// Компенсация задержек перед валидацией
var lagCompensation = SystemAPI.GetSingleton<LagCompensationSystem>();
var compensatedPosition = lagCompensation.CompensateMovement(playerId, clientTimestamp, targetPosition);

// Валидация с учетом компенсированной позиции
var result = validationSystem.ValidatePlayerInput(playerId, input, timestamp);
```

### **3. Интеграция с PlayerMovementSystem:**
```csharp
// В PlayerMovementSystem
protected override void OnUpdate()
{
    Entities
        .WithAll<PlayerTag, NetworkId>()
        .ForEach((ref LocalTransform transform, in PlayerInput input, in NetworkId networkId) =>
        {
            // Валидация ввода перед применением
            var validationSystem = SystemAPI.GetSingleton<InputValidationSystem>();
            var result = validationSystem.ValidatePlayerInput(networkId.Value, input, SystemAPI.Time.time);
            
            if (result.IsValid)
            {
                ProcessMovement(ref transform, input, SystemAPI.Time.fixedDeltaTime);
            }
        }).Schedule();
}
```

## ⚡ **ПРОИЗВОДИТЕЛЬНОСТЬ И ОПТИМИЗАЦИЯ**

### **1. Burst Compiler:**
```csharp
[BurstCompile]
public ValidationResult ValidatePlayerInput(int playerId, PlayerInput input, float timestamp)
{
    // Весь код метода оптимизирован Burst Compiler
}
```

### **2. Эффективные структуры данных:**
```csharp
// Использование NativeHashMap для быстрого доступа
private NativeHashMap<int, PlayerValidationData> _playerValidationData;

// Быстрая проверка существования данных
if (_playerValidationData.TryGetValue(playerId, out var validationData))
{
    // Обработка существующих данных
}
```

### **3. Оптимизированные алгоритмы:**
```csharp
// Эффективное сравнение вводов
[BurstCompile]
private bool InputsAreEqual(PlayerInput a, PlayerInput b)
{
    float tolerance = 0.01f;
    return math.distance(a.Movement, b.Movement) < tolerance &&
           a.Jump == b.Jump && a.Brake == b.Brake;
}
```

## 🧪 **ТЕСТИРОВАНИЕ СИСТЕМЫ**

### **Unit тесты:**
```csharp
[Test]
public void ValidatePlayerInput_ValidInput_ReturnsValidResult()
{
    // Arrange
    var validationSystem = new InputValidationSystem();
    var input = new PlayerInput { Movement = new float2(0.5f, 0.3f) };
    
    // Act
    var result = validationSystem.ValidatePlayerInput(1, input, Time.time);
    
    // Assert
    Assert.IsTrue(result.IsValid);
    Assert.AreEqual(ValidationReason.None, result.Reason);
}

[Test]
public void ValidatePlayerInput_TooFrequentInput_ReturnsInvalidResult()
{
    // Arrange
    var validationSystem = new InputValidationSystem();
    var input = new PlayerInput { Movement = new float2(0.5f, 0.3f) };
    
    // Act - два ввода подряд
    var result1 = validationSystem.ValidatePlayerInput(1, input, 0.0f);
    var result2 = validationSystem.ValidatePlayerInput(1, input, 0.01f); // Слишком быстро
    
    // Assert
    Assert.IsTrue(result1.IsValid);
    Assert.IsFalse(result2.IsValid);
    Assert.AreEqual(ValidationReason.TooFrequentInput, result2.Reason);
}
```

### **Integration тесты:**
```csharp
[Test]
public void InputValidation_WithNetworkManager_ProperlyValidatesInput()
{
    // Тест интеграции с NetworkManagerSystem
}
```

## 🚨 **ВАЖНЫЕ ЗАМЕЧАНИЯ**

### **1. Безопасность:**
- Все валидации выполняются на сервере
- Клиентские данные никогда не доверяются полностью
- Подозрительная активность отслеживается и логируется

### **2. Производительность:**
- Валидация выполняется в Burst-оптимизированном коде
- Используются эффективные структуры данных
- Минимизированы аллокации памяти

### **3. Детерминизм:**
- Все расчеты детерминированы для мультиплеера
- Используется фиксированное время для валидации

## 📚 **СВЯЗАННАЯ ДОКУМЕНТАЦИЯ**

- [LagCompensationSystem.md](./LagCompensationSystem.md) - Компенсация задержек
- [NetworkManagerSystem.md](./NetworkManagerSystem.md) - Управление сетью
- [PlayerMovementSystem.md](../../Core/Systems/PlayerMovementSystem.md) - Движение игрока
- [TerrainSyncSystem.md](../Terrain/Systems/TerrainSyncSystem.md) - Синхронизация террейна
