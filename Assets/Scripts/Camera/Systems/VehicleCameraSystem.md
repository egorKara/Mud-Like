using Unity.Entities;
using UnityEngine.InputSystem;
# 📹 VehicleCameraSystem - Система камеры для транспорта

## 📋 **ОБЗОР СИСТЕМЫ**

`VehicleCameraSystem` - это система камеры для транспорта в проекте Mud-Like. Система обеспечивает переключение между видами камеры (от первого/третьего лица) и следование за транспортом игрока.

## 🎯 **КЛЮЧЕВЫЕ ВОЗМОЖНОСТИ**

### **1. Переключение режимов камеры**
Система поддерживает два режима камеры:
- **FirstPerson** - от первого лица (из кабины)
- **ThirdPerson** - от третьего лица (за транспортом)

```csharp
// Переключение режима камеры
if (input.Action2) // Tab
{
    settings.CameraMode = (settings.CameraMode == CameraMode.FirstPerson) 
        ? CameraMode.ThirdPerson 
        : CameraMode.FirstPerson;
}
```

### **2. Следование за транспортом**
Камера автоматически следует за транспортом игрока:

```csharp
// Позиция камеры от третьего лица
float3 forward = math.forward(vehicleTransform.Rotation);
float3 lookDirection = math.length(physics.Velocity) > 0.1f ? velocityDirection : forward;

targetPosition = vehicleTransform.Position + 
               new float3(0f, _cameraSettings.ThirdPersonHeight, 0f) -
               lookDirection * _cameraSettings.ThirdPersonDistance;
```

### **3. Управление камерой**
- **Мышь** - поворот камеры
- **Колесико мыши** - зум камеры
- **Tab** - переключение режима

## 🏗️ **АРХИТЕКТУРА СИСТЕМЫ**

### **Основные компоненты:**
```csharp
public partial class VehicleCameraSystem : SystemBase
{
    private Camera _mainCamera;                    // Главная камера Unity
    private VehicleCameraSettings _cameraSettings; // Настройки камеры
}
```

### **Настройки камеры:**
```csharp
public struct VehicleCameraSettings
{
    public float ThirdPersonDistance;    // Расстояние от транспорта
    public float ThirdPersonHeight;      // Высота камеры
    public float FirstPersonHeight;      // Высота от первого лица
    public float CameraSmoothness;       // Плавность движения
    public float MouseSensitivity;       // Чувствительность мыши
    public CameraMode CameraMode;        // Текущий режим
}
```

### **Режимы камеры:**
```csharp
public enum CameraMode
{
    FirstPerson,     // От первого лица
    ThirdPerson      // От третьего лица
}
```

## 🔧 **ИНТЕГРАЦИЯ С ДРУГИМИ СИСТЕМАМИ**

### **1. Интеграция с VehicleInputSystem:**
```csharp
// Получение ввода камеры
var input = SystemAPI.GetComponent<PlayerInput>(playerVehicle);

// Поворот камеры мышью
if (math.abs(input.CameraLook.x) > 0.1f || math.abs(input.CameraLook.y) > 0.1f)
{
    float mouseX = input.CameraLook.x * _cameraSettings.MouseSensitivity;
    float mouseY = input.CameraLook.y * _cameraSettings.MouseSensitivity;
    
    Quaternion mouseRotation = Quaternion.Euler(-mouseY, mouseX, 0f);
    targetRotation = targetRotation * mouseRotation;
}

// Зум камеры колесиком
if (input.CameraZoom != 0f)
{
    settings.ThirdPersonDistance = math.clamp(
        settings.ThirdPersonDistance + input.CameraZoom * 2f * deltaTime,
        3f, 15f);
}
```

### **2. Интеграция с VehiclePhysicsSystem:**
```csharp
// Предсказание движения транспорта
float3 velocityDirection = math.normalize(physics.Velocity);
float3 lookDirection = math.length(physics.Velocity) > 0.1f ? velocityDirection : forward;

// Позиция камеры с учетом скорости
targetPosition = vehicleTransform.Position + 
               new float3(0f, _cameraSettings.ThirdPersonHeight, 0f) -
               lookDirection * _cameraSettings.ThirdPersonDistance;
```

## ⚡ **ПРОИЗВОДИТЕЛЬНОСТЬ И ОПТИМИЗАЦИЯ**

### **1. Burst Compiler:**
```csharp
[BurstCompile]
public partial class VehicleCameraSystem : SystemBase
{
    [BurstCompile]
    private void ProcessCameraInput(ref VehicleCameraSettings settings, in PlayerInput input, float deltaTime)
    
    [BurstCompile]
    private void UpdateCameraPosition(in LocalTransform vehicleTransform, in VehiclePhysics physics, float deltaTime)
}
```

### **2. Плавная анимация:**
```csharp
// Плавное перемещение камеры
Vector3 smoothPosition = Vector3.Lerp(currentPosition, targetPosition, 
    _cameraSettings.CameraSmoothness * deltaTime);

// Плавный поворот камеры
Quaternion smoothRotation = Quaternion.Lerp(_mainCamera.transform.rotation, targetRotation,
    _cameraSettings.CameraSmoothness * deltaTime);
```

## 🧪 **ТЕСТИРОВАНИЕ СИСТЕМЫ**

### **Unit тесты:**
```csharp
[Test]
public void VehicleCameraSystem_ValidInput_UpdatesCamera()
{
    // Arrange
    var cameraSystem = new VehicleCameraSystem();
    var input = new PlayerInput
    {
        Action2 = true, // Tab - переключение камеры
        CameraLook = new float2(1f, 0.5f),
        CameraZoom = 1f
    };
    
    // Act
    cameraSystem.ProcessCameraInput(ref settings, input, deltaTime);
    
    // Assert
    Assert.AreEqual(CameraMode.FirstPerson, settings.CameraMode);
    Assert.Greater(settings.ThirdPersonDistance, 3f);
}

[Test]
public void VehicleCameraSystem_ThirdPersonMode_CalculatesCorrectPosition()
{
    // Тест позиции камеры от третьего лица
}
```

### **Integration тесты:**
```csharp
[Test]
public void VehicleCamera_WithVehicleMovement_FollowsCorrectly()
{
    // Тест следования камеры за движущимся транспортом
}
```

## 📊 **КОНФИГУРАЦИЯ СИСТЕМЫ**

### **Параметры камеры:**
```csharp
public class VehicleCameraConfig : IComponentData
{
    public float ThirdPersonDistance = 8f;    // Расстояние от транспорта
    public float ThirdPersonHeight = 3f;      // Высота камеры
    public float FirstPersonHeight = 1.8f;    // Высота от первого лица
    public float CameraSmoothness = 5f;       // Плавность движения
    public float MouseSensitivity = 2f;       // Чувствительность мыши
    public float MinDistance = 3f;            // Минимальное расстояние
    public float MaxDistance = 15f;           // Максимальное расстояние
}
```

### **Настройки ввода:**
```csharp
public class CameraInputConfig : IComponentData
{
    public KeyCode CameraToggleKey = KeyCode.Tab;        // Переключение камеры
    public float MouseSensitivity = 2f;                  // Чувствительность мыши
    public float ScrollSensitivity = 2f;                 // Чувствительность колесика
    public bool InvertMouseY = false;                    // Инверсия мыши по Y
}
```

## 🚨 **ВАЖНЫЕ ЗАМЕЧАНИЯ**

### **1. Производительность:**
- Система использует `WithoutBurst().Run()` для работы с Unity Camera
- Все математические вычисления оптимизированы Burst Compiler
- Плавная анимация предотвращает резкие движения

### **2. Пользовательский опыт:**
- Автоматическое следование за транспортом
- Плавные переходы между режимами
- Настраиваемая чувствительность мыши

### **3. Совместимость:**
- Работает с любым транспортом с тегами `VehicleTag` и `PlayerTag`
- Поддерживает все типы ввода (клавиатура, мышь, геймпад)
- Совместима с Unity Camera API

## 📚 **СВЯЗАННАЯ ДОКУМЕНТАЦИЯ**

- [VehicleInputSystem.md](../Input/Systems/VehicleInputSystem.md) - Система ввода
- [VehicleControlSystem.md](../../Core/Systems/VehicleControlSystem.md) - Управление транспортом
- [VehiclePhysicsSystem.md](../../Vehicles/Systems/VehiclePhysicsSystem.md) - Физика транспорта
- [SceneManagementSystem.md](../../Gameplay/Systems/SceneManagementSystem.md) - Управление сценами
