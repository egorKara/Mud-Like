# 🌊 MudParticlePoolSystem - Пулинг частиц грязи

## 📋 **ОБЗОР СИСТЕМЫ**

`MudParticlePoolSystem` - это система пулинга объектов для частиц грязи в проекте Mud-Like. Система обеспечивает эффективное переиспользование частиц для высокой производительности при создании визуальных эффектов грязи.

## 🎯 **КЛЮЧЕВЫЕ ВОЗМОЖНОСТИ**

### **1. Создание частиц грязи**
Основной метод системы - `CreateMudParticle(position, velocity, size, lifetime)` - создает частицу грязи с заданными параметрами.

```csharp
// Пример создания частицы грязи
var particlePool = SystemAPI.GetSingleton<MudParticlePoolSystem>();
var particle = particlePool.CreateMudParticle(
    position: new float3(10f, 0f, 5f),    // Позиция создания
    velocity: new float3(2f, 5f, 1f),     // Начальная скорость
    size: 0.5f,                           // Размер частицы
    lifetime: 3.0f                        // Время жизни в секундах
);

// Проверка успешности создания
if (particle != Entity.Null)
{
    Debug.Log("Частица грязи успешно создана");
}
```

### **2. Управление пулом частиц**
Система автоматически управляет пулом частиц:

```csharp
// Получение статистики пула
int activeParticles = particlePool.GetActiveParticleCount();
int availableParticles = particlePool.GetAvailableParticleCount();

Debug.Log($"Активных частиц: {activeParticles}, Доступных: {availableParticles}");

// Очистка всех частиц
particlePool.ClearAllParticles();
```

### **3. Возврат частиц в пул**
Система автоматически возвращает частицы в пул по истечении времени жизни:

```csharp
// Ручной возврат частицы в пул
var particle = CreateMudParticle(position, velocity, size, lifetime);
particlePool.ReturnParticleToPool(particle);
```

## 🏗️ **АРХИТЕКТУРА СИСТЕМЫ**

### **Компоненты системы:**
```csharp
public partial class MudParticlePoolSystem : SystemBase
{
    private NativeQueue<Entity> _availableParticles;        // Доступные частицы
    private NativeList<Entity> _activeParticles;            // Активные частицы
    private NativeHashMap<Entity, ParticleData> _particleData; // Данные частиц
    private EntityArchetype _particleArchetype;             // Архетип частицы
    private int _maxParticles = 1000;                       // Максимум частиц
    private int _initialPoolSize = 100;                     // Начальный размер пула
}
```

### **ECS компоненты частиц:**
```csharp
// Тег частицы грязи
public struct MudParticleTag : IComponentData { }

// Скорость частицы
public struct ParticleVelocity : IComponentData
{
    public float3 Value;
}

// Время жизни частицы
public struct ParticleLifetime : IComponentData
{
    public float Current;
    public float Max;
}

// Размер частицы
public struct ParticleSize : IComponentData
{
    public float Current;
    public float Max;
}

// Цвет частицы
public struct ParticleColor : IComponentData
{
    public float4 Value;
}

// Активность частицы
public struct ParticleActive : IComponentData
{
    public bool Value;
}
```

### **Основные методы:**
```csharp
// Основной API метод
public Entity CreateMudParticle(float3 position, float3 velocity, float size, float lifetime)

// Управление пулом
public void ReturnParticleToPool(Entity particle)
public int GetActiveParticleCount()
public int GetAvailableParticleCount()
public void ClearAllParticles()

// Внутренние методы
private void InitializeParticlePool()
private void SetupInactiveParticle(Entity particle)
private void ActivateParticle(Entity particle, float3 position, float3 velocity, float size, float lifetime)
```

## 🔧 **ИНТЕГРАЦИЯ С ДРУГИМИ СИСТЕМАМИ**

### **1. Интеграция с TerrainDeformationSystem**
```csharp
// В TerrainDeformationSystem при создании деформации
protected override void OnUpdate()
{
    Entities
        .WithAll<DeformationData>()
        .ForEach((in DeformationData deformation) =>
        {
            // Создаем частицы грязи при сильной деформации
            if (deformation.Force > 500f)
            {
                var particlePool = SystemAPI.GetSingleton<MudParticlePoolSystem>();
                
                // Создаем несколько частиц в области деформации
                for (int i = 0; i < 5; i++)
                {
                    float3 randomOffset = new float3(
                        Random.Range(-deformation.Radius, deformation.Radius),
                        0f,
                        Random.Range(-deformation.Radius, deformation.Radius)
                    );
                    
                    float3 particlePosition = deformation.Position + randomOffset;
                    float3 particleVelocity = new float3(
                        Random.Range(-2f, 2f),
                        Random.Range(3f, 8f),
                        Random.Range(-2f, 2f)
                    );
                    
                    particlePool.CreateMudParticle(
                        particlePosition,
                        particleVelocity,
                        Random.Range(0.2f, 0.8f),
                        Random.Range(2f, 5f)
                    );
                }
            }
        }).Schedule();
}
```

### **2. Интеграция с VehiclePhysicsSystem**
```csharp
// В VehiclePhysicsSystem при движении по грязи
protected override void OnUpdate()
{
    Entities
        .WithAll<VehicleTag>()
        .ForEach((ref VehiclePhysics physics, in LocalTransform transform) =>
        {
            // Создаем частицы при движении по грязи
            if (physics.SinkDepth > 0.1f && physics.Speed > 2f)
            {
                var particlePool = SystemAPI.GetSingleton<MudParticlePoolSystem>();
                
                // Создаем частицы за колесами
                for (int i = 0; i < 2; i++)
                {
                    float3 wheelPosition = transform.Position + new float3(i * 2f - 1f, 0f, -2f);
                    float3 particleVelocity = new float3(
                        Random.Range(-1f, 1f),
                        Random.Range(2f, 5f),
                        Random.Range(-3f, -1f)
                    );
                    
                    particlePool.CreateMudParticle(
                        wheelPosition,
                        particleVelocity,
                        Random.Range(0.3f, 0.7f),
                        Random.Range(1f, 3f)
                    );
                }
            }
        }).Schedule();
}
```

### **3. Интеграция с MudManagerSystem**
```csharp
// В MudManagerSystem при сильном взаимодействии с грязью
public MudContactData QueryContact(float3 wheelPosition, float radius, float wheelForce)
{
    // ... существующая логика ...
    
    // Создаем частицы при сильном воздействии
    if (wheelForce > 1000f && sinkDepth > 0.2f)
    {
        var particlePool = SystemAPI.GetSingleton<MudParticlePoolSystem>();
        
        // Создаем взрыв частиц
        for (int i = 0; i < 8; i++)
        {
            float angle = (float)i / 8f * 2f * math.PI;
            float3 direction = new float3(math.cos(angle), 0.5f, math.sin(angle));
            float3 velocity = direction * Random.Range(3f, 8f);
            
            particlePool.CreateMudParticle(
                wheelPosition,
                velocity,
                Random.Range(0.2f, 0.6f),
                Random.Range(1.5f, 4f)
            );
        }
    }
    
    return contactData;
}
```

## ⚡ **ПРОИЗВОДИТЕЛЬНОСТЬ И ОПТИМИЗАЦИЯ**

### **1. Эффективное управление памятью**
```csharp
// Переиспользование Entity вместо создания новых
if (_availableParticles.TryDequeue(out particle))
{
    // Используем существующую частицу
    ActivateParticle(particle, position, velocity, size, lifetime);
}
else if (_activeParticles.Length < _maxParticles)
{
    // Создаем новую частицу только если не достигнут лимит
    particle = EntityManager.CreateEntity(_particleArchetype);
    ActivateParticle(particle, position, velocity, size, lifetime);
}
else
{
    // Переиспользуем самую старую частицу
    particle = _activeParticles[0];
    _activeParticles.RemoveAtSwapBack(0);
    ActivateParticle(particle, position, velocity, size, lifetime);
}
```

### **2. Burst Compiler оптимизация**
```csharp
[BurstCompile]
public partial class MudParticlePoolSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = SystemAPI.Time.fixedDeltaTime;
        
        // Обновление частиц в Burst-оптимизированном коде
        for (int i = _activeParticles.Length - 1; i >= 0; i--)
        {
            Entity particle = _activeParticles[i];
            
            if (!_particleData.TryGetValue(particle, out var data))
                continue;
            
            // Обновляем время жизни
            data.Lifetime -= deltaTime;
            
            if (data.Lifetime <= 0f)
            {
                ReturnParticleToPool(particle);
                continue;
            }
            
            // Обновляем позицию и скорость
            data.Position += data.Velocity * deltaTime;
            data.Velocity.y -= 9.81f * deltaTime; // Гравитация
            
            _particleData[particle] = data;
        }
    }
}
```

### **3. Оптимизированные структуры данных**
```csharp
// Использование Native Collections для производительности
private NativeQueue<Entity> _availableParticles;        // O(1) операции
private NativeList<Entity> _activeParticles;            // O(1) добавление/удаление
private NativeHashMap<Entity, ParticleData> _particleData; // O(1) поиск
```

## 🧪 **ТЕСТИРОВАНИЕ СИСТЕМЫ**

### **Unit тесты:**
```csharp
[Test]
public void CreateMudParticle_ValidParameters_ReturnsValidEntity()
{
    // Arrange
    var particlePool = new MudParticlePoolSystem();
    float3 position = new float3(10f, 0f, 5f);
    float3 velocity = new float3(2f, 5f, 1f);
    float size = 0.5f;
    float lifetime = 3.0f;
    
    // Act
    var particle = particlePool.CreateMudParticle(position, velocity, size, lifetime);
    
    // Assert
    Assert.AreNotEqual(Entity.Null, particle);
    Assert.AreEqual(1, particlePool.GetActiveParticleCount());
}

[Test]
public void ReturnParticleToPool_ValidParticle_ReturnsToPool()
{
    // Arrange
    var particlePool = new MudParticlePoolSystem();
    var particle = particlePool.CreateMudParticle(position, velocity, size, lifetime);
    
    // Act
    particlePool.ReturnParticleToPool(particle);
    
    // Assert
    Assert.AreEqual(0, particlePool.GetActiveParticleCount());
    Assert.AreEqual(1, particlePool.GetAvailableParticleCount());
}
```

### **Performance тесты:**
```csharp
[Test]
public void CreateMudParticle_Performance_UnderLimit()
{
    // Arrange
    var particlePool = new MudParticlePoolSystem();
    var stopwatch = Stopwatch.StartNew();
    
    // Act - создаем 1000 частиц
    for (int i = 0; i < 1000; i++)
    {
        particlePool.CreateMudParticle(
            new float3(i, 0f, 0f),
            new float3(1f, 2f, 1f),
            0.5f,
            3.0f
        );
    }
    
    stopwatch.Stop();
    
    // Assert - должно быть быстрее 100ms
    Assert.Less(stopwatch.ElapsedMilliseconds, 100);
    Assert.AreEqual(1000, particlePool.GetActiveParticleCount());
}
```

### **Integration тесты:**
```csharp
[Test]
public void MudParticlePool_WithTerrainDeformation_CreatesParticles()
{
    // Тест интеграции с TerrainDeformationSystem
    var particlePool = new MudParticlePoolSystem();
    var terrainDeformation = new TerrainDeformationSystem();
    
    // Создаем деформацию с силой
    var deformation = new DeformationData
    {
        Position = new float3(0f, 0f, 0f),
        Radius = 2f,
        Force = 1000f
    };
    
    // Применяем деформацию
    terrainDeformation.ApplyDeformation(deformation);
    
    // Проверяем создание частиц
    Assert.Greater(particlePool.GetActiveParticleCount(), 0);
}
```

## 📊 **КОНФИГУРАЦИЯ СИСТЕМЫ**

### **Параметры пула:**
```csharp
public class MudParticlePoolConfig : IComponentData
{
    public int MaxParticles = 1000;        // Максимальное количество частиц
    public int InitialPoolSize = 100;      // Начальный размер пула
    public float DefaultLifetime = 3.0f;   // Время жизни по умолчанию
    public float DefaultSize = 0.5f;       // Размер по умолчанию
    public float4 DefaultColor = new float4(0.4f, 0.2f, 0.1f, 1f); // Цвет грязи
}
```

### **Настройка эффектов:**
```csharp
public class MudParticleEffectsConfig : IComponentData
{
    public bool EnableWheelSplash = true;      // Брызги от колес
    public bool EnableDeformationBurst = true; // Взрывы при деформации
    public bool EnableStrongImpact = true;     // Эффекты при сильном воздействии
    public float SplashThreshold = 2f;         // Порог для брызг
    public float BurstThreshold = 500f;        // Порог для взрывов
    public float ImpactThreshold = 1000f;      // Порог для сильных воздействий
}
```

## 🚨 **ВАЖНЫЕ ЗАМЕЧАНИЯ**

### **1. Управление памятью:**
- Все Native Collections автоматически очищаются в `OnDestroy()`
- Частицы переиспользуются для минимизации аллокаций
- Максимальный лимит частиц предотвращает утечки памяти

### **2. Производительность:**
- Система оптимизирована для создания множества частиц
- Burst Compiler обеспечивает высокую производительность
- Эффективные структуры данных минимизируют накладные расходы

### **3. Визуальные эффекты:**
- Частицы автоматически обновляют позицию, скорость и размер
- Гравитация и время жизни управляются системой
- Цвет и прозрачность изменяются со временем

## 📚 **СВЯЗАННАЯ ДОКУМЕНТАЦИЯ**

- [TerrainDeformationSystem.md](../Terrain/Systems/TerrainDeformationSystem.md) - Деформация террейна
- [VehiclePhysicsSystem.md](../Vehicles/Systems/VehiclePhysicsSystem.md) - Физика транспорта
- [MudManagerSystem.md](../Terrain/Systems/MudManagerSystem.md) - API деформации террейна
- [ObjectPoolSystem.md](./ObjectPoolSystem.md) - Общий пулинг объектов
- [SystemPerformanceProfiler.md](../Core/Performance/SystemPerformanceProfiler.md) - Профилирование систем
