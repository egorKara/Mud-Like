# 🎯 Unified Development Guide - Единое руководство по разработке Mud-Like

## 📋 **ОБЗОР**

Этот документ объединяет все правила разработки, именования и архитектурные принципы проекта Mud-Like. Строгое соблюдение этих правил критически важно для предотвращения конфликтов Unity ECS source generators и поддержания чистой архитектуры проекта.

## 🏗️ **АРХИТЕКТУРНЫЕ ПРИНЦИПЫ**

### **1. Разделение по модулям**
- **ECS модули** - только для ECS систем, компонентов и Jobs
- **Performance модули** - для менеджеров ресурсов и оптимизации
- **Gameplay модули** - для игровой логики
- **UI модули** - для пользовательского интерфейса

### **2. Namespace структура**
```csharp
// ECS модули
namespace MudLike.Core.ECS
namespace MudLike.Vehicles.ECS
namespace MudLike.Terrain.ECS

// Performance модули
namespace MudLike.Core.Performance
namespace MudLike.Vehicles.Performance
namespace MudLike.Terrain.Performance

// Gameplay модули
namespace MudLike.Core.Gameplay
namespace MudLike.Vehicles.Gameplay
namespace MudLike.Terrain.Gameplay
```

## 🔧 **ПРАВИЛА ИМЕНОВАНИЯ**

### **ECS Системы**
```csharp
// ✅ ПРАВИЛЬНО
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public partial class MovementSystem : SystemBase

[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial class ParticleSystem : SystemBase

[UpdateInGroup(typeof(PresentationSystemGroup))]
public partial class RenderingSystem : SystemBase

// ❌ НЕПРАВИЛЬНО
public class ObjectPool : SystemBase  // Конфликт с source generators
public class Manager : SystemBase     // Неясное назначение
public class PoolSystem : SystemBase // Смешивание концепций
```

### **ECS Компоненты**
```csharp
// ✅ ПРАВИЛЬНО
public struct Position : IComponentData
public struct Velocity : IComponentData
public struct PlayerTag : IComponentData
public struct WheelData : IComponentData

// ❌ НЕПРАВИЛЬНО
public struct Data : IComponentData     // Слишком общее
public struct Info : IComponentData     // Неясное назначение
public struct Pool : IComponentData     // Конфликт с пулингом
```

### **Менеджеры и Пулы**
```csharp
// ✅ ПРАВИЛЬНО
public class MudLikeMemoryPool
public class ParticlePoolManager
public class TerrainChunkManager
public class VehiclePhysicsManager

// ❌ НЕПРАВИЛЬНО
public class Pool : SystemBase          // Конфликт с ECS
public class Manager : SystemBase       // Неясное назначение
public class MudLikeObjectPool          // Устаревшее имя
```

### **Jobs и Burst**
```csharp
// ✅ ПРАВИЛЬНО
[BurstCompile]
public struct MovementJob : IJobEntity

[BurstCompile]
public struct ParticleUpdateJob : IJobParallelFor

[BurstCompile]
public struct TerrainDeformationJob : IJob

// ❌ НЕПРАВИЛЬНО
public struct Job : IJobEntity          // Слишком общее
public struct Work : IJobParallelFor    // Неясное назначение
```

## 📁 **СТРУКТУРА ФАЙЛОВ**

### **Именование файлов**
```
Assets/Scripts/Core/ECS/
├── MovementSystem.cs           # ECS система
├── Position.cs                 # ECS компонент
├── MovementJob.cs              # ECS Job
└── MudLike.ECS.asmdef         # Assembly Definition

Assets/Scripts/Core/Performance/
├── MemoryPoolManager.cs        # Менеджер пула
├── PerformanceProfiler.cs      # Профилировщик
└── MudLike.Performance.asmdef # Assembly Definition
```

### **Правила именования файлов**
- **ECS системы**: `[Name]System.cs`
- **ECS компоненты**: `[Name].cs`
- **ECS Jobs**: `[Name]Job.cs`
- **Менеджеры**: `[Name]Manager.cs`
- **Пулы**: `[Name]Pool.cs` (НЕ ECS системы!)

## 🚫 **ЗАПРЕЩЕННЫЕ ИМЕНА**

### **Конфликты с Unity ECS**
```csharp
// ❌ ЗАПРЕЩЕНО - конфликтует с source generators
ObjectPool
Pool
Manager
System
Data
Info
Work
Job
```

### **Конфликты с Unity API**
```csharp
// ❌ ЗАПРЕЩЕНО - конфликтует с Unity
Transform
GameObject
MonoBehaviour
Component
Behaviour
```

### **Конфликты с проектом**
```csharp
// ❌ ЗАПРЕЩЕНО - устаревшие имена
MudLikeObjectPool
ResourcePool
GenericPool
```

## ✅ **РЕКОМЕНДУЕМЫЕ ИМЕНА**

### **Для ECS систем**
```csharp
MovementSystem
PhysicsSystem
RenderingSystem
ParticleSystem
TerrainSystem
VehicleSystem
InputSystem
NetworkSystem
```

### **Для менеджеров**
```csharp
MemoryPoolManager
ParticlePoolManager
TerrainChunkManager
VehiclePhysicsManager
NetworkManager
UIManager
AudioManager
```

### **Для компонентов**
```csharp
Position
Velocity
Acceleration
Rotation
Scale
Health
Damage
PlayerTag
EnemyTag
```

## 🧪 **ОБЯЗАТЕЛЬНЫЕ ПРАВИЛА ТЕСТИРОВАНИЯ**

### **Требования к тестам**
- **Запуск тестов** перед каждым коммитом
- **100% coverage** для всех новых классов
- **Unit тесты** для ECS систем и менеджеров

### **Структура тестов**
```csharp
[Test]
public void MethodName_ValidInput_ReturnsExpectedResult()
{
    // Arrange
    var input = CreateTestInput();
    
    // Act
    var result = MethodUnderTest(input);
    
    // Assert
    Assert.AreEqual(expected, result);
}
```

## 📚 **ДОКУМЕНТАЦИЯ**

### **Обязательная документация**
- **Ведение документации** изменений в `Project_Startup/`
- **Архитектурные решения** фиксировать в README
- **Примеры использования** для сложных систем
- **XML комментарии** для всех публичных API

## 🔍 **ПРОВЕРКА КОНФЛИКТОВ**

### **Автоматическая проверка**
```bash
# Поиск потенциальных конфликтов
grep -r "class.*Pool.*SystemBase" Assets/Scripts/
grep -r "class.*Manager.*SystemBase" Assets/Scripts/
grep -r "class.*System.*SystemBase" Assets/Scripts/

# Поиск конфликтов имен
find Assets/Scripts -name "*.cs" -exec grep -l "class.*Pool.*SystemBase" {} \;

# Проверка генерируемых файлов
find Library -name "*.g.cs" -exec grep -l "MudLike" {} \;
```

### **Ручная проверка**
1. **Unity Console** - проверка ошибок компиляции
2. **Solution Explorer** - поиск дублирующихся имен
3. **References → Analyzers** - проверка генерируемых файлов

## 📋 **ЧЕКЛИСТ ПРОВЕРКИ**

### **Перед созданием нового класса**
- [ ] Имя уникально и не конфликтует с Unity API
- [ ] Имя не конфликтует с source generators
- [ ] Namespace соответствует модулю
- [ ] Файл находится в правильной папке
- [ ] Assembly Definition настроен корректно

### **Перед коммитом**
- [ ] Нет ошибок компиляции
- [ ] Нет предупреждений в Unity Console
- [ ] Все тесты проходят
- [ ] Документация обновлена
- [ ] Naming conventions соблюдены

## 🎯 **ПРИМЕРЫ ПРАВИЛЬНОЙ АРХИТЕКТУРЫ**

### **ParticleModule**
```csharp
// Performance модуль
namespace MudLike.Particles.Performance
{
    public class ParticlePoolManager
    {
        public ParticleData AllocateParticle() { }
        public void ReleaseParticle(ParticleData particle) { }
    }
}

// ECS модуль
namespace MudLike.Particles.ECS
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class ParticleSystem : SystemBase
    {
        private ParticlePoolManager _poolManager;
    }
}
```

### **VehicleModule**
```csharp
// Performance модуль
namespace MudLike.Vehicles.Performance
{
    public class VehiclePhysicsManager
    {
        public void UpdatePhysics() { }
    }
}

// ECS модуль
namespace MudLike.Vehicles.ECS
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial class VehicleMovementSystem : SystemBase
    {
        private VehiclePhysicsManager _physicsManager;
    }
}
```

## 🚀 **ИНТЕГРАЦИЯ С CURSOR IDE**

### **Настройка Cursor**
1. **Custom Prompts** для ECS:
   ```
   "Generate ECS system with Burst support for [module]"
   "Create performance manager for [resource] pooling"
   ```

2. **Ограничение контекста**:
   ```
   @Assets/Scripts/Core/ECS/ - для ECS кода
   @Assets/Scripts/Core/Performance/ - для менеджеров
   ```

3. **Автоматическая проверка**:
   - Настройте Cursor для проверки naming conventions
   - Добавьте правила в .cursorrules

### **Диагностика проблем**
```bash
# Поиск конфликтов имен
find Assets/Scripts -name "*.cs" -exec grep -l "class.*Pool.*SystemBase" {} \;

# Проверка генерируемых файлов
find Library -name "*.g.cs" -exec grep -l "MudLike" {} \;
```

## 🚫 **ЗАПРЕЩЕНО**

### **Имена классов**
- `ObjectPool` (конфликт с source generators)
- `Pool` (конфликт с source generators)
- `Manager` (неясное назначение)
- `System` (слишком общее)
- `Data` (слишком общее)

### **Архитектурные ошибки**
- Object Pooling как ECS система
- MonoBehaviour в игровом коде
- Смешивание ECS и не-ECS концепций
- Игнорирование Assembly Definitions

## 📚 **ДОПОЛНИТЕЛЬНЫЕ РЕСУРСЫ**

- `ECS_Conflict_Resolution.md` - решение конфликтов Unity ECS
- `Cursor_IDE_Integration.md` - интеграция с Cursor IDE
- `Resolution_Summary.md` - резюме решений

---

**Эти правила обеспечивают стабильную работу Unity ECS, предотвращают конфликты source generators и поддерживают чистую архитектуру проекта Mud-Like!**
