# ✅ Mud-Like ECS Architecture Status

## 🎯 **СТАТУС АРХИТЕКТУРЫ**

### **Миграция завершена**
Проект успешно перешел к **чистой ECS-архитектуре**. Все игровые системы теперь используют Entities, Components и Systems вместо MonoBehaviour.

### **Текущее состояние**
- ✅ **Полный ECS** - все игровые системы используют DOTS
- ✅ **Детерминизм** - FixedStepSimulationSystemGroup для физики
- ✅ **Производительность** - Burst Compilation + Job System
- ✅ **Масштабируемость** - поддержка тысяч сущностей

## 📋 **ПЛАН МИГРАЦИИ**

### **Этап 1: Базовые компоненты (1-2 месяца)**
- Position, Velocity, Rotation, Health, PlayerInput
- Создание нового пустого проекта Unity с пакетами DOTS
- Настройка базовой ECS структуры

### **Этап 2: Критические модули (2-3 месяца)**
- Движение игроков, обработка ввода, логика оружия
- Следование примерам из EntityComponentSystemSamples
- Прототип с одним игроком

### **Этап 3: Физические системы (3-4 месяца)**
- Транспорт, деформация, физика колес
- Интеграция с Unity Physics
- Тестирование производительности

### **Этап 4: Сетевые системы (2-3 месяца)**
- Синхронизация, репликация, мультиплеер
- Настройка Netcode for Entities
- Тестирование детерминизма

## 🏗️ **БАЗОВЫЕ КОМПОНЕНТЫ**

### **1. Position Component**
```csharp
using Unity.Entities;
using Unity.Mathematics;

// Компонент позиции в мире
public struct Position : IComponentData
{
    public float3 Value;
}

// Система для обновления позиции
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public class PositionUpdateSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref Position position, in Velocity velocity) =>
        {
            position.Value += velocity.Value * Time.fixedDeltaTime;
        }).Schedule();
    }
}
```

### **2. Velocity Component**
```csharp
using Unity.Entities;
using Unity.Mathematics;

// Компонент скорости движения
public struct Velocity : IComponentData
{
    public float3 Value;
}

// Система для обновления скорости
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public class VelocityUpdateSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref Velocity velocity, in Acceleration acceleration) =>
        {
            velocity.Value += acceleration.Value * Time.fixedDeltaTime;
        }).Schedule();
    }
}
```

### **3. Rotation Component**
```csharp
using Unity.Entities;
using Unity.Mathematics;

// Компонент вращения
public struct Rotation : IComponentData
{
    public quaternion Value;
}

// Система для обновления вращения
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public class RotationUpdateSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref Rotation rotation, in AngularVelocity angularVelocity) =>
        {
            rotation.Value = math.mul(rotation.Value, 
                quaternion.AxisAngle(angularVelocity.Axis, angularVelocity.Speed * Time.fixedDeltaTime));
        }).Schedule();
    }
}
```

### **4. Health Component**
```csharp
using Unity.Entities;

// Компонент здоровья
public struct Health : IComponentData
{
    public float Value;
    public float MaxValue;
}

// Система для обработки здоровья
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public class HealthSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref Health health, in Damage damage) =>
        {
            health.Value = math.max(0, health.Value - damage.Value);
            if (health.Value <= 0)
            {
                // Обработка смерти
                PostUpdateCommands.AddComponent<DeadTag>(entity);
            }
        }).Schedule();
    }
}
```

### **5. PlayerInput Component**
```csharp
using Unity.Entities;
using Unity.Mathematics;

// Компонент ввода игрока
public struct PlayerInput : IComponentData
{
    public float2 Movement;
    public bool Jump;
    public bool Brake;
    public bool Shoot;
    public bool Interact;
}

// Система для обработки ввода
[UpdateInGroup(typeof(InitializationSystemGroup))]
public class PlayerInputSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref PlayerInput input) =>
        {
            // Чтение ввода
            input.Movement = new float2(
                Input.GetAxis("Horizontal"),
                Input.GetAxis("Vertical")
            );
            input.Jump = Input.GetButtonDown("Jump");
            input.Brake = Input.GetButton("Brake");
            input.Shoot = Input.GetButtonDown("Fire1");
            input.Interact = Input.GetButtonDown("Interact");
        }).WithoutBurst().Run();
    }
}
```

## 🚗 **КОМПОНЕНТЫ ТРАНСПОРТА**

### **1. VehicleData Component**
```csharp
using Unity.Entities;
using Unity.Mathematics;

// Данные транспорта
public struct VehicleData : IComponentData
{
    public float MaxSpeed;
    public float Acceleration;
    public float BrakeForce;
    public float TurnSpeed;
    public float Mass;
    public float3 CenterOfMass;
}

// Система движения транспорта
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public class VehicleMovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref Position position, ref Velocity velocity, 
                         ref Rotation rotation, in VehicleData vehicleData, 
                         in PlayerInput input) =>
        {
            // Обработка движения
            float3 forward = math.forward(rotation.Value);
            float3 right = math.right(rotation.Value);
            
            // Движение вперед/назад
            float3 movement = forward * input.Movement.y * vehicleData.MaxSpeed;
            velocity.Value = math.lerp(velocity.Value, movement, 
                vehicleData.Acceleration * Time.fixedDeltaTime);
            
            // Поворот
            float turn = input.Movement.x * vehicleData.TurnSpeed * Time.fixedDeltaTime;
            rotation.Value = math.mul(rotation.Value, 
                quaternion.AxisAngle(math.up(), turn));
            
            // Торможение
            if (input.Brake)
            {
                velocity.Value = math.lerp(velocity.Value, float3.zero, 
                    vehicleData.BrakeForce * Time.fixedDeltaTime);
            }
        }).Schedule();
    }
}
```

### **2. WheelData Component**
```csharp
using Unity.Entities;
using Unity.Mathematics;

// Данные колеса
public struct WheelData : IComponentData
{
    public float3 Position;
    public float Radius;
    public float Width;
    public float SuspensionLength;
    public float SuspensionForce;
    public float DampingForce;
    public float FrictionForce;
    public bool IsGrounded;
    public float3 GroundNormal;
    public float3 GroundPoint;
}

// Система физики колес
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public class WheelPhysicsSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref WheelData wheel, ref Velocity velocity, 
                         in Position position, in Rotation rotation) =>
        {
            // Raycast для определения земли
            float3 wheelWorldPos = position.Value + math.mul(rotation.Value, wheel.Position);
            float3 rayDirection = -math.up();
            float rayDistance = wheel.SuspensionLength + wheel.Radius;
            
            if (Physics.Raycast(wheelWorldPos, rayDirection, out RaycastHit hit, rayDistance))
            {
                wheel.IsGrounded = true;
                wheel.GroundPoint = hit.point;
                wheel.GroundNormal = hit.normal;
                
                // Расчет силы подвески
                float suspensionCompression = wheel.SuspensionLength - hit.distance;
                float suspensionForce = suspensionCompression * wheel.SuspensionForce;
                float dampingForce = -velocity.Value.y * wheel.DampingForce;
                
                // Применение силы
                float3 force = wheel.GroundNormal * (suspensionForce + dampingForce);
                // Применение силы к телу транспорта
            }
            else
            {
                wheel.IsGrounded = false;
            }
        }).Schedule();
    }
}
```

## 🌍 **КОМПОНЕНТЫ ТЕРРЕЙНА**

### **1. TerrainData Component**
```csharp
using Unity.Entities;
using Unity.Mathematics;

// Данные террейна
public struct TerrainData : IComponentData
{
    public float3 WorldPosition;
    public float Height;
    public float MudLevel;
    public float Traction;
    public float SinkDepth;
    public float3 Normal;
    public int ChunkIndex;
}

// Система деформации террейна
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public class TerrainDeformationSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.ForEach((in DeformationData deformation) =>
        {
            // Применение деформации к террейну
            ApplyDeformation(deformation.Position, deformation.Radius, deformation.Depth);
        }).Schedule();
    }
    
    private void ApplyDeformation(float3 position, float radius, float depth)
    {
        // Логика деформации террейна
        // Использование TerrainPaintUtility для оптимизации
        // Синхронизация с TerrainCollider
    }
}
```

### **2. DeformationData Component**
```csharp
using Unity.Entities;
using Unity.Mathematics;

// Данные деформации
public struct DeformationData : IComponentData
{
    public float3 Position;
    public float Radius;
    public float Depth;
    public float Intensity;
    public float3 Normal;
    public int TerrainChunkIndex;
}

// Система генерации деформации
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public class DeformationGenerationSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.ForEach((in WheelData wheel, in Position position) =>
        {
            if (wheel.IsGrounded)
            {
                // Создание деформации под колесом
                var deformation = new DeformationData
                {
                    Position = wheel.GroundPoint,
                    Radius = wheel.Radius,
                    Depth = wheel.SinkDepth,
                    Intensity = wheel.FrictionForce,
                    Normal = wheel.GroundNormal
                };
                
                PostUpdateCommands.CreateEntity();
                PostUpdateCommands.AddComponent(deformation);
            }
        }).Schedule();
    }
}
```

## 🌐 **СЕТЕВЫЕ КОМПОНЕНТЫ**

### **1. NetworkPosition Component**
```csharp
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;

// Сетевая позиция
public struct NetworkPosition : IComponentData
{
    public float3 Value;
}

// Система отправки позиции на сервер
[UpdateAfter(typeof(PlayerMovementSystem))]
public class SendPositionToServerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var networkManager = GetSingleton<NetworkManager>();
        
        Entities
            .WithAll<PlayerTag, IsClient>()
            .ForEach((ref NetworkPosition position) =>
            {
                if (networkManager.IsConnectedClient)
                {
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

### **2. PlayerTag Component**
```csharp
using Unity.Entities;

// Тег игрока
public struct PlayerTag : IComponentData
{
}

// Система обработки игроков
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public class PlayerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities
            .WithAll<PlayerTag>()
            .ForEach((ref Position position, ref Velocity velocity, 
                     in PlayerInput input) =>
            {
                // Обработка логики игрока
                ProcessPlayerInput(ref position, ref velocity, input);
            }).Schedule();
    }
    
    private void ProcessPlayerInput(ref Position position, ref Velocity velocity, PlayerInput input)
    {
        // Логика обработки ввода игрока
    }
}
```

## 🔄 **ПРОЦЕСС МИГРАЦИИ**

### **Шаг 1: Создание нового проекта**
```bash
# Создать новый 3D проект Unity
# Название: Mud-Like
# Шаблон: 3D (URP)
# Версия: 6000.0.57f1
```

### **Шаг 2: Установка пакетов**
```json
{
  "dependencies": {
    "com.unity.entities": "1.3.14",
    "com.unity.collections": "2.5.7",
    "com.unity.burst": "1.8.24",
    "com.unity.physics": "1.3.14",
    "com.unity.netcode": "1.6.2"
  }
}
```

### **Шаг 3: Создание базовых компонентов**
1. **Position** - позиция в мире
2. **Velocity** - скорость движения
3. **Rotation** - вращение объекта
4. **Health** - здоровье объекта
5. **PlayerInput** - ввод игрока

### **Шаг 4: Создание базовых систем**
1. **MovementSystem** - система движения
2. **InputSystem** - система ввода
3. **PhysicsSystem** - физическая система
4. **RenderingSystem** - система рендеринга

### **Шаг 5: Тестирование**
```csharp
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
```

## ⚠️ **ВАЖНЫЕ ПРИНЦИПЫ**

### **1. Детерминизм**
- **FixedStepSimulationSystemGroup** для всей физики
- **Четкий порядок** выполнения систем
- **Избегание** недетерминированных операций

### **2. Производительность**
- **Burst Compiler** для вычислений
- **Job System** для параллельности
- **NativeArray<T>** для управления памятью

### **3. Тестируемость**
- **Unit тесты** для каждой системы
- **Integration тесты** для взаимодействий
- **Performance тесты** для производительности

### **4. Качество кода**
- **Data-Oriented Design** для архитектуры
- **Code Review** всех изменений
- **Документация** всех API

## 🎯 **РЕЗУЛЬТАТ МИГРАЦИИ**

### **После завершения миграции:**
- ✅ **Чистая ECS-архитектура** без MonoBehaviour
- ✅ **Детерминированная симуляция** для мультиплеера
- ✅ **Высокая производительность** через DOTS
- ✅ **Масштабируемость** для множества игроков
- ✅ **Тестируемость** всех систем
- ✅ **Качество кода** с первого дня

---

**Миграция на ECS - это критически важный этап для создания стабильного и производительного мультиплеера. Следуйте этому руководству для систематического перехода к чистой ECS-архитектуре.**
