# 📝 Mud-Like Coding Standards

## 🎯 **ОБЗОР СТАНДАРТОВ**

### **Цель стандартов**
Обеспечить единообразие, читаемость и качество кода в проекте Mud-Like.

### **Принципы**
- **Единообразие** - одинаковый стиль во всем проекте
- **Читаемость** - код должен быть понятен
- **Качество** - следование лучшим практикам
- **Документация** - все API должны быть задокументированы

## 🏗️ **АРХИТЕКТУРНЫЕ ПРИНЦИПЫ**

### **1. Data-Oriented Design принципы**
```csharp
// Компоненты - только данные
public struct Position : IComponentData
{
    public float3 Value;
}

public struct Velocity : IComponentData
{
    public float3 Value;
}

// Системы - только логика
public class MovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        // Обрабатывает данные компонентов
        Entities.ForEach((ref Position pos, in Velocity vel) =>
        {
            pos.Value += vel.Value * Time.fixedDeltaTime;
        }).Schedule();
    }
}

// Разделение данных и логики
public struct VehicleConfig : IComponentData
{
    public float MaxSpeed;
    public float Acceleration;
}

// Системы работают с данными, а не с объектами
public class VehicleSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref Position pos, in VehicleConfig config) =>
        {
            // Логика обработки данных
        }).Schedule();
    }
}
```

### **2. Clean Architecture**
```csharp
// Domain Layer - бизнес-логика
public struct Position : IComponentData
{
    public float3 Value;
}

// Application Layer - системы
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public class MovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        // Логика движения
    }
}

// Infrastructure Layer - внешние зависимости
public class NetworkManager : SystemBase
{
    // Сетевая функциональность через ECS
    protected override void OnUpdate()
    {
        // Логика сетевой синхронизации
    }
}
```

## 📝 **СТИЛЬ КОДИРОВАНИЯ**

### **1. Именование**
```csharp
// Классы - PascalCase
public class PlayerController { }
public class TerrainDeformationSystem { }

// Методы - PascalCase
public void MovePlayer(float3 direction) { }
public void ApplyDeformation(float3 position, float radius) { }

// Поля - camelCase
private float _currentSpeed;
private bool _isGrounded;

// Константы - UPPER_CASE
public const float MAX_SPEED = 10f;
public const int MAX_PLAYERS = 100;

// Переменные - camelCase
float moveSpeed = 5f;
bool isMoving = false;
```

### **2. Структура файлов**
```csharp
// Порядок элементов в классе:
// 1. Константы
// 2. Поля
// 3. Свойства
// 4. Конструкторы
// 5. Методы
// 6. События

public class ExampleClass
{
    // 1. Константы
    public const float DEFAULT_SPEED = 5f;
    
    // 2. Поля
    [SerializeField] private float _speed;
    [SerializeField] private bool _isActive;
    
    // 3. Свойства
    public float Speed => _speed;
    public bool IsActive => _isActive;
    
    // 4. Конструкторы
    public ExampleClass()
    {
        _speed = DEFAULT_SPEED;
        _isActive = true;
    }
    
    // 5. Методы
    public void Move(float3 direction)
    {
        // Реализация
    }
    
    // 6. События
    public event Action OnMove;
}
```

### **3. Комментарии**
```csharp
/// <summary>
/// Система движения игрока в ECS архитектуре
/// </summary>
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public class PlayerMovementSystem : SystemBase
{
    /// <summary>
    /// Обрабатывает движение всех игроков
    /// </summary>
    protected override void OnUpdate()
    {
        // Обработка ввода и движение
        Entities
            .WithAll<PlayerTag>()
            .ForEach((ref Translation translation, in PlayerInput input) =>
            {
                // Вычисление направления движения
                float3 movement = new float3(input.Movement.x, 0, input.Movement.y);
                movement = math.normalize(movement) * 5f;
                
                // Применение движения
                translation.Value += movement * Time.fixedDeltaTime;
            }).Schedule();
    }
}
```

## 🎮 **UNITY-СПЕЦИФИЧНЫЕ ПРАВИЛА**

### **1. ECS компоненты (вместо MonoBehaviour)**
```csharp
// Использование ECS компонентов вместо MonoBehaviour
public struct VehicleConfig : IComponentData
{
    public float MaxSpeed;
    public float Acceleration;
    public float TurnSpeed;
}

public struct VehiclePhysics : IComponentData
{
    public float3 Velocity;
    public float3 AngularVelocity;
    public float Mass;
    public float Drag;
}
```

### **2. ECS системы**
```csharp
// Компоненты данных - только данные
public struct Position : IComponentData
{
    public float3 Value;
}

// Теги - пустые структуры
public struct PlayerTag : IComponentData
{
}

// Системы - только логика
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public class MovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        // Логика системы
    }
}
```

### **3. ScriptableObjects**
```csharp
// Конфигурация через ScriptableObjects
[CreateAssetMenu(fileName = "VehicleConfig", menuName = "Mud-Like/Vehicle Config")]
public class VehicleConfig : ScriptableObject
{
    [Header("Movement Settings")]
    public float maxSpeed = 10f;
    public float acceleration = 5f;
    public float turnSpeed = 2f;
    
    [Header("Physics Settings")]
    public float mass = 1000f;
    public float drag = 0.3f;
    public float angularDrag = 5f;
}
```

## 🧪 **ТЕСТИРОВАНИЕ**

### **1. Unit тесты**
```csharp
// Тесты для изолированной логики
[Test]
public void CalculateDistance_ValidPoints_ReturnsCorrectDistance()
{
    // Arrange
    var point1 = new float3(0, 0, 0);
    var point2 = new float3(3, 4, 0);
    
    // Act
    var distance = MathUtils.CalculateDistance(point1, point2);
    
    // Assert
    Assert.AreEqual(5f, distance, 0.001f);
}
```

### **2. Integration тесты**
```csharp
// Тесты для взаимодействия компонентов
[UnityTest]
public IEnumerator PlayerMovement_InputReceived_MovesPlayer()
{
    // Arrange
    var player = CreatePlayer();
    var input = new Vector2(1, 0);
    
    // Act
    Input.SetAxis("Horizontal", input.x);
    yield return new WaitForFixedUpdate();
    
    // Assert
    Assert.Greater(player.transform.position.x, 0);
}
```

## 📊 **МЕТРИКИ КАЧЕСТВА**

### **1. Сложность методов**
```csharp
// Хорошо - простая логика
public void Move(float3 direction)
{
    _position += direction * _speed * Time.fixedDeltaTime;
}

// Плохо - сложная логика
public void ComplexMethod()
{
    if (condition1)
    {
        if (condition2)
        {
            if (condition3)
            {
                // Слишком много вложенности
            }
        }
    }
}
```

### **2. Длина методов**
```csharp
// Хорошо - короткий метод
public float CalculateSpeed(float distance, float time)
{
    return distance / time;
}

// Плохо - длинный метод
public void ProcessEverything()
{
    // 100+ строк кода
    // Слишком много ответственности
}
```

### **3. Количество параметров**
```csharp
// Хорошо - мало параметров
public void Move(float3 direction)
{
    // Реализация
}

// Плохо - много параметров
public void ComplexMethod(float x, float y, float z, bool flag1, bool flag2, int value1, int value2)
{
    // Слишком много параметров
}
```

## 🔧 **ИНСТРУМЕНТЫ**

### **1. Code Analysis**
```csharp
// Использование атрибутов для анализа
[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
public void Method()
{
    // Подавление предупреждений анализатора
}
```

### **2. Code Coverage**
```csharp
// Покрытие тестами
[Test]
public void Method_ValidInput_ReturnsExpectedResult()
{
    // Тест должен покрывать все ветки кода
    var result = Method(true);
    Assert.AreEqual(expected, result);
    
    var result2 = Method(false);
    Assert.AreEqual(expected2, result2);
}
```

### **3. Performance**
```csharp
// Оптимизация производительности
[BurstCompile]
public struct OptimizedJob : IJobParallelFor
{
    public void Execute(int index)
    {
        // Высокопроизводительный код
    }
}
```

## 📋 **CHECKLIST ПЕРЕД КОММИТОМ**

### **1. Код**
- [ ] Следует стандартам именования
- [ ] Имеет XML документацию
- [ ] Проходит все тесты
- [ ] Не имеет предупреждений компилятора
- [ ] Соответствует архитектурным принципам

### **2. Тестирование**
- [ ] Unit тесты написаны
- [ ] Integration тесты написаны
- [ ] Code Coverage 100%
- [ ] Все тесты проходят

### **3. Производительность**
- [ ] Нет утечек памяти
- [ ] Производительность в пределах нормы
- [ ] Используется Burst Compiler где нужно
- [ ] Оптимизированы критические пути

### **4. Документация**
- [ ] README обновлен
- [ ] API задокументирован
- [ ] Примеры использования добавлены
- [ ] Changelog обновлен

## 🎯 **ПРИМЕРЫ ХОРОШЕГО КОДА**

### **1. ECS система**
```csharp
/// <summary>
/// Система движения игрока в ECS архитектуре
/// </summary>
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public class PlayerMovementSystem : SystemBase
{
    /// <summary>
    /// Обрабатывает движение всех игроков
    /// </summary>
    protected override void OnUpdate()
    {
        Entities
            .WithAll<PlayerTag>()
            .ForEach((ref Translation translation, in PlayerInput input) =>
            {
                ProcessMovement(ref translation, input);
            }).Schedule();
    }
    
    private static void ProcessMovement(ref Translation translation, in PlayerInput input)
    {
        float3 movement = CalculateMovement(input);
        translation.Value += movement * Time.fixedDeltaTime;
    }
    
    private static float3 CalculateMovement(in PlayerInput input)
    {
        float3 direction = new float3(input.Movement.x, 0, input.Movement.y);
        return math.normalize(direction) * 5f;
    }
}
```

### **2. Тест**
```csharp
/// <summary>
/// Тесты для системы движения игрока
/// </summary>
public class PlayerMovementSystemTests
{
    [Test]
    public void ProcessMovement_ValidInput_MovesPlayer()
    {
        // Arrange
        var translation = new Translation { Value = float3.zero };
        var input = new PlayerInput { Movement = new float2(1, 0) };
        
        // Act
        PlayerMovementSystem.ProcessMovement(ref translation, input);
        
        // Assert
        Assert.Greater(translation.Value.x, 0);
    }
}
```

---

**Следование стандартам кодирования Mud-Like обеспечивает единообразие, читаемость и качество кода во всем проекте.**
