# 🎯 Cursor IDE Integration - Интеграция с Cursor IDE

## 🚀 **ОБЗОР**

Рекомендации по эффективному использованию Cursor IDE для разработки проекта Mud-Like с учетом Unity ECS и предотвращения конфликтов source generators.

## 🔧 **НАСТРОЙКА CURSOR IDE**

### **1. Custom Prompts для ECS**
Создайте шаблоны для быстрой генерации правильного кода:

#### **ECS Системы**
```
Prompt: "Generate ECS system with Burst support for [module]"
Example: "Generate ECS system with Burst support for particle pooling"
```

#### **Performance Менеджеры**
```
Prompt: "Create performance manager for [resource] pooling"
Example: "Create performance manager for particle pooling"
```

#### **Тесты**
```
Prompt: "Generate unit tests for [class] with [coverage]"
Example: "Generate unit tests for MemoryPoolSystem with 100% coverage"
```

### **2. Ограничение контекста**
Используйте `@` для ограничения области поиска:

```bash
# ECS код
@Assets/Scripts/Core/ECS/

# Performance код
@Assets/Scripts/Core/Performance/

# Тесты
@Assets/Scripts/Tests/

# Конкретный модуль
@Assets/Scripts/Vehicles/
```

### **3. Настройка .cursorrules**
Создайте файл `.cursorrules` в корне проекта:

```bash
# Mud-Like Project Rules
- Always use ECS architecture for game logic
- Never use MonoBehaviour in game code
- Follow strict naming conventions
- Use Assembly Definitions for modules
- Generate tests for all new code
- Use Russian language for documentation
- Follow Clean Architecture principles
```

## 🎯 **ЭФФЕКТИВНЫЕ ПРАКТИКИ**

### **1. Диагностика проблем**
```bash
# Поиск конфликтов имен
Ctrl+Shift+F: "class.*Pool.*SystemBase"
Ctrl+Shift+F: "MudLikeObjectPool"
Ctrl+Shift+F: "__System_"

# Проверка генерируемых файлов
Ctrl+Shift+F: "*.g.cs"
```

### **2. Автоматическая проверка**
Настройте Cursor для проверки:
- Naming conventions
- ECS architecture compliance
- Assembly Definition consistency
- Test coverage

### **3. Быстрая навигация**
```bash
# Переход к определениям
Ctrl+Click на класс/метод

# Поиск по символам
Ctrl+T: "MemoryPool"

# Поиск по файлам
Ctrl+P: "MemoryPoolSystem"
```

## 🏗️ **АРХИТЕКТУРНЫЕ ПАТТЕРНЫ**

### **1. ECS + Performance разделение**
```csharp
// Performance модуль (не ECS)
namespace MudLike.Core.Performance
{
    public class MudLikeMemoryPool
    {
        // Менеджер ресурсов
    }
}

// ECS модуль (интеграция)
namespace MudLike.Core.ECS
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial class MemoryPoolSystem : SystemBase
    {
        private MudLikeMemoryPool _memoryPool;
    }
}
```

### **2. Правильная структура модулей**
```
Assets/Scripts/
├── Core/
│   ├── ECS/                    # ECS системы
│   │   ├── MemoryPoolSystem.cs
│   │   └── MudLike.ECS.asmdef
│   ├── Performance/            # Менеджеры
│   │   ├── MemoryPoolManager.cs
│   │   └── MudLike.Performance.asmdef
│   └── Gameplay/               # Игровая логика
├── Vehicles/
│   ├── ECS/                    # ECS системы транспорта
│   └── Performance/            # Менеджеры транспорта
└── Tests/
    ├── Unit/
    │   ├── ECS/
    │   └── Performance/
    └── MudLike.Tests.asmdef
```

## 🚫 **ИЗБЕГАНИЕ КОНФЛИКТОВ**

### **1. Запрещенные имена**
```csharp
// ❌ НЕ ИСПОЛЬЗОВАТЬ
ObjectPool
Pool
Manager
System
Data
Info
Work
Job
```

### **2. Рекомендуемые имена**
```csharp
// ✅ ИСПОЛЬЗОВАТЬ
MemoryPoolManager
ParticleSystem
MovementSystem
Position
Velocity
PlayerTag
```

### **3. Namespace изоляция**
```csharp
// ECS модули
namespace MudLike.Core.ECS
namespace MudLike.Vehicles.ECS
namespace MudLike.Terrain.ECS

// Performance модули
namespace MudLike.Core.Performance
namespace MudLike.Vehicles.Performance
namespace MudLike.Terrain.Performance
```

## 🔍 **ДИАГНОСТИКА И ОТЛАДКА**

### **1. Проверка конфликтов**
```bash
# Поиск дублирующихся имен
grep -r "class.*Pool" Assets/Scripts/
grep -r "class.*Manager" Assets/Scripts/
grep -r "class.*System" Assets/Scripts/
```

### **2. Анализ генерируемых файлов**
```bash
# Проверка Unity ECS генерации
find Library -name "*.g.cs" -exec grep -l "MudLike" {} \;
find Library -name "*__System_*.cs" -exec ls -la {} \;
```

### **3. Очистка кэша**
```bash
# Очистка Unity кэша
rm -rf Library/
rm -rf Temp/

# Перезапуск Unity
# File → Reimport All
```

## 📋 **ЧЕКЛИСТ РАЗРАБОТКИ**

### **Перед созданием нового класса**
- [ ] Проверить имя на конфликты
- [ ] Выбрать правильный namespace
- [ ] Определить модуль (ECS/Performance/Gameplay)
- [ ] Создать Assembly Definition
- [ ] Написать тесты

### **Перед коммитом**
- [ ] Нет ошибок компиляции
- [ ] Нет предупреждений Unity
- [ ] Все тесты проходят
- [ ] Документация обновлена
- [ ] Naming conventions соблюдены

### **При возникновении ошибок**
- [ ] Проверить конфликты имен
- [ ] Очистить кэш Unity
- [ ] Проверить Assembly Definitions
- [ ] Перезапустить Cursor IDE

## 🎮 **ПРИМЕРЫ ИСПОЛЬЗОВАНИЯ**

### **1. Создание ECS системы**
```
Prompt: "Create ECS system for vehicle movement with Burst support"
Context: @Assets/Scripts/Vehicles/ECS/
```

### **2. Создание Performance менеджера**
```
Prompt: "Create performance manager for terrain chunk pooling"
Context: @Assets/Scripts/Terrain/Performance/
```

### **3. Генерация тестов**
```
Prompt: "Generate unit tests for VehicleMovementSystem with 100% coverage"
Context: @Assets/Scripts/Tests/Unit/Vehicles/
```

## 🚀 **ПРОДВИНУТЫЕ ТЕХНИКИ**

### **1. Автоматическая генерация документации**
```
Prompt: "Generate README for ParticleModule with usage examples"
Context: @Assets/Scripts/Particles/
```

### **2. Рефакторинг кода**
```
Prompt: "Refactor ObjectPool to follow ECS architecture"
Context: @Assets/Scripts/Core/Performance/
```

### **3. Оптимизация производительности**
```
Prompt: "Optimize MemoryPoolSystem for 60+ FPS"
Context: @Assets/Scripts/Core/ECS/
```

## 📚 **ДОПОЛНИТЕЛЬНЫЕ РЕСУРСЫ**

### **Unity ECS**
- [Unity ECS Documentation](https://docs.unity3d.com/Packages/com.unity.entities@latest)
- [Source Generators Guide](https://docs.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/source-generators-overview)

### **Cursor IDE**
- [Cursor Documentation](https://cursor.sh/docs)
- [AI Code Generation](https://cursor.sh/docs/ai-features)

### **Производительность**
- [Unity Performance Best Practices](https://docs.unity3d.com/Manual/PerformanceOptimization.html)
- [Burst Compiler](https://docs.unity3d.com/Packages/com.unity.burst@latest)

---

**Эти рекомендации обеспечивают эффективную работу с Cursor IDE и предотвращают конфликты Unity ECS source generators.**
