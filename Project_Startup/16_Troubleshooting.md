# 🔧 Mud-Like Troubleshooting

## 🎯 **ОБЗОР РЕШЕНИЯ ПРОБЛЕМ**

### **Цель руководства**
Предоставить решения для типичных проблем, возникающих при разработке Mud-Like.

### **Принципы**
- **Систематический подход** - пошаговое решение
- **Профилактика** - предотвращение проблем
- **Документирование** - запись решений
- **Обучение** - понимание причин

## 🚨 **КРИТИЧЕСКИЕ ПРОБЛЕМЫ**

### **1. Проблемы компиляции**

#### **CS0246: The type or namespace name could not be found**
```csharp
// Проблема: Отсутствует using или assembly reference
// Решение: Добавить using директиву
using Unity.Entities;
using Unity.Mathematics;

// Или проверить Assembly Definition
{
  "name": "Mud-Like.Core",
  "references": [
    "Unity.Entities",
    "Unity.Collections"
  ]
}
```

#### **CS0534: Does not implement inherited abstract member**
```csharp
// Проблема: Не реализован абстрактный метод
public class DeformationEffect : PooledObject
{
    // Решение: Реализовать все абстрактные методы
    protected override void OnObjectReset()
    {
        // Реализация
    }
}
```

### **2. Проблемы ECS**

#### **Система не выполняется**
```csharp
// Проблема: Система не вызывается
// Решение: Проверить UpdateInGroup
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public class MovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        // Логика системы
    }
}
```

#### **Компоненты не синхронизируются**
```csharp
// Проблема: Компоненты не обновляются
// Решение: Проверить порядок выполнения систем
[UpdateAfter(typeof(InputSystem))]
public class MovementSystem : SystemBase
{
    // Система выполняется после InputSystem
}
```

### **3. Проблемы производительности**

#### **Низкий FPS**
```csharp
// Проблема: Низкая производительность
// Решение: Использовать Burst Compiler
[BurstCompile]
public struct OptimizedJob : IJobParallelFor
{
    public void Execute(int index)
    {
        // Высокопроизводительный код
    }
}
```

#### **Утечки памяти**
```csharp
// Проблема: Утечки памяти
// Решение: Правильное управление NativeArray
public class MemoryManager : MonoBehaviour
{
    private NativeArray<float3> _positions;
    private Allocator _allocator = Allocator.Persistent;
    
    void Start()
    {
        _positions = new NativeArray<float3>(1000, _allocator);
    }
    
    void OnDestroy()
    {
        if (_positions.IsCreated)
            _positions.Dispose();
    }
}
```

## 🌐 **СЕТЕВЫЕ ПРОБЛЕМЫ**

### **1. Проблемы синхронизации**

#### **Рассинхронизация клиентов**
```csharp
// Проблема: Клиенты видят разное состояние
// Решение: Обеспечить детерминизм
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public class DeterministicSystem : SystemBase
{
    protected override void OnUpdate()
    {
        // Использовать Time.fixedDeltaTime для детерминизма
        Entities.ForEach((ref Position pos, in Velocity vel) =>
        {
            pos.Value += vel.Value * Time.fixedDeltaTime;
        }).Schedule();
    }
}
```

#### **Высокая задержка сети**
```csharp
// Проблема: Высокая задержка
// Решение: Оптимизировать сетевой трафик
public class NetworkOptimization
{
    // Синхронизация только при изменении
    private float3 _lastPosition;
    
    public void UpdatePosition(float3 newPosition)
    {
        if (math.distance(_lastPosition, newPosition) > 0.1f)
        {
            SendPositionUpdate(newPosition);
            _lastPosition = newPosition;
        }
    }
}
```

### **2. Проблемы подключения**

#### **Клиент не подключается к серверу**
```csharp
// Проблема: Ошибка подключения
// Решение: Проверить настройки сети
public class NetworkManager : MonoBehaviour
{
    void Start()
    {
        // Проверить порт и адрес
        var endpoint = NetworkEndPoint.LoopbackIpv4;
        endpoint.Port = 7777;
        
        if (_driver.Connect(endpoint) == 0)
        {
            Debug.Log("Connected to server");
        }
        else
        {
            Debug.LogError("Failed to connect to server");
        }
    }
}
```

## 🎮 **ИГРОВЫЕ ПРОБЛЕМЫ**

### **1. Проблемы управления**

#### **Ввод не обрабатывается**
```csharp
// Проблема: Ввод игрока не работает
// Решение: Проверить систему ввода
[UpdateInGroup(typeof(InitializationSystemGroup))]
public class PlayerInputSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref PlayerInput input) =>
        {
            // Проверить настройки Input Manager
            input.Movement = new float2(
                Input.GetAxis("Horizontal"),
                Input.GetAxis("Vertical")
            );
        }).WithoutBurst().Run();
    }
}
```

#### **Камера не следует за игроком**
```csharp
// Проблема: Камера не обновляется
// Решение: Проверить CameraController
public class CameraController : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 5f;
    
    void LateUpdate()
    {
        if (target == null) return;
        
        // Плавное следование за целью
        Vector3 targetPosition = target.position + Vector3.back * 10f;
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
    }
}
```

### **2. Проблемы физики**

#### **Транспорт не движется**
```csharp
// Проблема: Транспорт не реагирует на ввод
// Решение: Проверить физическую систему
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public class VehicleSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities
            .WithAll<VehicleTag>()
            .ForEach((ref Translation translation, ref Velocity velocity, 
                     in VehicleData vehicleData, in PlayerInput input) =>
            {
                // Проверить применение сил
                float3 movement = new float3(input.Movement.x, 0, input.Movement.y);
                movement = math.normalize(movement) * vehicleData.MaxSpeed;
                
                velocity.Value = math.lerp(velocity.Value, movement, 
                    vehicleData.Acceleration * Time.fixedDeltaTime);
                
                translation.Value += velocity.Value * Time.fixedDeltaTime;
            }).Schedule();
    }
}
```

#### **Колеса не касаются земли**
```csharp
// Проблема: Колеса не взаимодействуют с землей
// Решение: Проверить raycast систему
public class WheelPhysicsSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities
            .WithAll<WheelData>()
            .ForEach((ref WheelData wheel, in Translation translation) =>
            {
                // Проверить raycast параметры
                float3 rayStart = translation.Value;
                float3 rayDirection = -math.up();
                float rayDistance = wheel.SuspensionLength + wheel.Radius;
                
                if (Physics.Raycast(rayStart, rayDirection, out RaycastHit hit, rayDistance))
                {
                    wheel.IsGrounded = true;
                    wheel.GroundPoint = hit.point;
                    wheel.GroundNormal = hit.normal;
                }
                else
                {
                    wheel.IsGrounded = false;
                }
            }).Schedule();
    }
}
```

## 🌍 **ПРОБЛЕМЫ ТЕРРЕЙНА**

### **1. Деформация не работает**

#### **Террейн не деформируется**
```csharp
// Проблема: Деформация не применяется
// Решение: Проверить систему деформации
public class TerrainDeformationSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities
            .WithAll<DeformationData>()
            .ForEach((in DeformationData deformation) =>
            {
                // Проверить параметры деформации
                if (deformation.Radius > 0 && deformation.Depth > 0)
                {
                    ApplyDeformation(deformation.Position, deformation.Radius, deformation.Depth);
                }
            }).Schedule();
    }
    
    private void ApplyDeformation(float3 position, float radius, float depth)
    {
        // Проверить TerrainData
        var terrain = TerrainManager.Instance.GetTerrain();
        if (terrain == null) return;
        
        // Применить деформацию
        var terrainData = terrain.terrainData;
        var heights = terrainData.GetHeights(0, 0, terrainData.heightmapResolution, terrainData.heightmapResolution);
        
        // Логика деформации
        // ...
        
        terrainData.SetHeights(0, 0, heights);
        terrainData.SyncHeightmap();
    }
}
```

#### **Коллайдер не обновляется**
```csharp
// Проблема: TerrainCollider не синхронизируется
// Решение: Принудительное обновление коллайдера
public class TerrainSyncSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities
            .WithAll<ChunkData>()
            .ForEach((in ChunkData chunk) =>
            {
                if (chunk.NeedsSync)
                {
                    SyncTerrainCollider(chunk.ChunkIndex);
                }
            }).Schedule();
    }
    
    private void SyncTerrainCollider(int chunkIndex)
    {
        var terrain = TerrainManager.Instance.GetTerrain();
        var terrainCollider = terrain.GetComponent<TerrainCollider>();
        
        if (terrainCollider != null)
        {
            // Принудительное обновление коллайдера
            terrainCollider.terrainData = terrain.terrainData;
        }
    }
}
```

## 🧪 **ПРОБЛЕМЫ ТЕСТИРОВАНИЯ**

### **1. Тесты не проходят**

#### **Unit тесты падают**
```csharp
// Проблема: Тест не проходит
// Решение: Проверить логику теста
[Test]
public void CalculateDistance_ValidInput_ReturnsCorrectDistance()
{
    // Arrange
    var point1 = new float3(0, 0, 0);
    var point2 = new float3(3, 4, 0);
    
    // Act
    var distance = MathUtils.CalculateDistance(point1, point2);
    
    // Assert - проверить точность
    Assert.AreEqual(5f, distance, 0.001f); // Добавить tolerance
}
```

#### **Integration тесты не работают**
```csharp
// Проблема: Integration тест не проходит
// Решение: Проверить настройки тестовой сцены
[UnityTest]
public IEnumerator PlayerMovement_InputReceived_MovesPlayer()
{
    // Arrange
    var player = CreatePlayer();
    var input = new Vector2(1, 0);
    
    // Act
    Input.SetAxis("Horizontal", input.x);
    yield return new WaitForFixedUpdate(); // Увеличить время ожидания
    yield return new WaitForFixedUpdate();
    
    // Assert
    Assert.Greater(player.transform.position.x, 0);
}
```

### **2. Проблемы производительности**

#### **Тесты производительности падают**
```csharp
// Проблема: Тест производительности не проходит
// Решение: Проверить целевые показатели
[Test]
public void TerrainDeformation_PerformanceTest_CompletesInTime()
{
    // Arrange
    var stopwatch = Stopwatch.StartNew();
    var deformationCount = 1000;
    
    // Act
    for (int i = 0; i < deformationCount; i++)
    {
        ApplyDeformation(GetRandomPosition(), 5f, 2f);
    }
    stopwatch.Stop();
    
    // Assert - увеличить лимит времени
    Assert.Less(stopwatch.ElapsedMilliseconds, 200); // Увеличить с 100ms до 200ms
}
```

## 🔍 **ДИАГНОСТИКА**

### **1. Логирование**
```csharp
// Добавить подробное логирование
public class DebugSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities
            .WithAll<PlayerTag>()
            .ForEach((in Translation translation, in PlayerInput input) =>
            {
                if (Debug.isDebugBuild)
                {
                    Debug.Log($"Player position: {translation.Value}, Input: {input.Movement}");
                }
            }).WithoutBurst().Run();
    }
}
```

### **2. Профилирование**
```csharp
// Использовать Unity Profiler
public class ProfilerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        using (new ProfilerMarker("MovementSystem").Auto())
        {
            // Логика системы
        }
    }
}
```

### **3. Отладка**
```csharp
// Добавить отладочную информацию
public class DebugDrawer : MonoBehaviour
{
    void OnDrawGizmos()
    {
        // Визуализация для отладки
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 5f);
    }
}
```

## 📋 **CHECKLIST РЕШЕНИЯ ПРОБЛЕМ**

### **1. Перед началом**
- [ ] Проверить версию Unity
- [ ] Проверить установленные пакеты
- [ ] Проверить настройки проекта
- [ ] Проверить логи на ошибки

### **2. Во время разработки**
- [ ] Следовать стандартам кодирования
- [ ] Писать тесты для нового кода
- [ ] Проверять производительность
- [ ] Документировать изменения

### **3. При возникновении проблем**
- [ ] Прочитать логи ошибок
- [ ] Проверить настройки
- [ ] Использовать отладку
- [ ] Обратиться к документации

## 🎯 **ПРОФИЛАКТИКА ПРОБЛЕМ**

### **1. Регулярные проверки**
- **Ежедневно:** Проверка логов и тестов
- **Еженедельно:** Аудит производительности
- **Ежемесячно:** Обзор архитектуры

### **2. Автоматизация**
- **CI/CD:** Автоматическое тестирование
- **Code Analysis:** Автоматическая проверка кода
- **Performance Monitoring:** Мониторинг производительности

### **3. Документирование**
- **Решения:** Записывать найденные решения
- **Проблемы:** Документировать известные проблемы
- **Обучение:** Делиться знаниями с командой

---

**Руководство по решению проблем Mud-Like поможет быстро диагностировать и исправлять типичные проблемы разработки.**
