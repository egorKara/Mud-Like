# 🏗️ План модульной архитектуры Mud-Like

## 📋 Принципы модульной архитектуры

### ✅ Основные принципы:
- **Независимость модулей** - каждый модуль самодостаточен
- **Четкие интерфейсы** - общение через контракты
- **Минимальные зависимости** - избегание циклических связей
- **ECS-ориентированность** - все модули следуют DOTS принципам
- **Тестируемость** - каждый модуль тестируется изолированно

## 🎯 Модульная структура проекта

### 1. **CoreModule** (Базовый модуль)
```
Assets/Scripts/Core/
├── Components/           # Базовые ECS компоненты
├── Systems/             # Системы инициализации
├── Contracts/           # Интерфейсы и контракты
├── Events/              # Система событий
├── Performance/         # Профилирование и оптимизация
└── Tests/               # Unit тесты
```
**Ответственность:** Базовые утилиты, события, контракты
**Зависимости:** Только Unity DOTS
**Интерфейсы:** Предоставляет базовые контракты другим модулям

### 2. **PhysicsModule** (Физика транспорта)
```
Assets/Scripts/Physics/
├── Components/          # WheelPhysics, Suspension, Transmission
├── Systems/             # PhysicsUpdateSystem, WheelSystem
├── Jobs/                # Burst-оптимизированные Jobs
├── Configs/             # ScriptableObject конфигурации
└── Tests/               # Физические тесты
```
**Ответственность:** Реалистичная физика колес, подвески, трансмиссии
**Зависимости:** CoreModule
**Вход:** Позиция колес, тип поверхности, пользовательский ввод
**Выход:** SinkDepth, tractionModifier, обновленные WheelStateComponent

### 3. **MudDeformationModule** (Деформация террейна)
```
Assets/Scripts/MudDeformation/
├── Components/          # TerrainData, DeformationData
├── Systems/             # TerrainDeformationSystem
├── Jobs/                # DeformationJobs
├── API/                 # MudManager API
└── Tests/               # Деформационные тесты
```
**Ответственность:** Деформация heightfield, MudManager API
**Зависимости:** CoreModule, PhysicsModule (для данных колес)
**Вход:** WheelPosition, radius, текущий heightfield
**Выход:** Обновленный heightfield, drag/traction modifiers

### 4. **NetworkModule** (Мультиплеер)
```
Assets/Scripts/Network/
├── Components/          # NetworkId, GhostComponents
├── Systems/             # NetworkSyncSystem, LagCompensationSystem
├── Jobs/                # NetworkJobs
├── Configs/             # Network настройки
└── Tests/               # Сетевые тесты
```
**Ответственность:** Netcode for Entities, синхронизация, lag compensation
**Зависимости:** CoreModule
**Вход:** Локальные изменения ECS, сетевые пакеты
**Выход:** Синхронизированные entities, подтверждения

### 5. **VehicleModule** (Транспорт)
```
Assets/Scripts/Vehicle/
├── Components/          # VehicleConfig, WheelData, EngineData
├── Systems/             # VehicleControlSystem, VehicleSpawningSystem
├── Jobs/                # VehicleJobs
├── Configs/             # Vehicle конфигурации (КрАЗ-255Б)
└── Tests/               # Тесты транспорта
```
**Ответственность:** Логика транспорта, спавнинг, управление
**Зависимости:** CoreModule, PhysicsModule
**Вход:** Пользовательский ввод, физические данные
**Выход:** Команды управления, события транспорта

### 6. **ParticleModule** (Эффекты)
```
Assets/Scripts/Particle/
├── Components/          # ParticleComponent, ChunkComponent
├── Systems/             # ParticleSystem, ObjectPoolSystem
├── Jobs/                # ParticleJobs
├── Configs/             # Particle настройки
└── Tests/               # Тесты эффектов
```
**Ответственность:** Object Pooling, частицы грязи, визуальные эффекты
**Зависимости:** CoreModule
**Вход:** События взаимодействия, позиции
**Выход:** Пул частиц, визуальные эффекты

## 🔄 Межмодульное взаимодействие

### Оркестратор (GameManager)
```csharp
// Центральный BootstrapSystem в CoreModule
public partial class GameBootstrapSystem : SystemBase
{
    protected override void OnCreate()
    {
        // Инициализация всех модулей
        InitializePhysicsModule();
        InitializeMudDeformationModule();
        InitializeNetworkModule();
        InitializeVehicleModule();
        InitializeParticleModule();
    }
}
```

### Механизмы взаимодействия:
1. **События/Сообщения** - модули публикуют события через EventSystem
2. **Shared Components** - общие данные через SingletonEntity
3. **ECS Queries** - модули читают данные других модулей через компоненты

## 📁 Структура Assembly Definitions

### Зависимости модулей:
```
CoreModule (базовый)
├── PhysicsModule → CoreModule
├── VehicleModule → CoreModule, PhysicsModule
├── MudDeformationModule → CoreModule, PhysicsModule
├── NetworkModule → CoreModule
└── ParticleModule → CoreModule
```

### Пример .asmdef файлов:
```json
// PhysicsModule.asmdef
{
    "name": "MudLike.Physics",
    "references": ["MudLike.Core"],
    "includePlatforms": [],
    "allowUnsafeCode": true
}
```

## 🎯 План реализации

### Фаза 1: Реорганизация существующих модулей (1 неделя)
1. **Создать правильную структуру папок**
2. **Настроить Assembly Definitions**
3. **Переместить файлы в соответствующие модули**
4. **Обновить namespace и зависимости**

### Фаза 2: Создание интерфейсов (1 неделя)
1. **Определить контракты между модулями**
2. **Создать интерфейсы для межмодульного взаимодействия**
3. **Реализовать систему событий**
4. **Настроить оркестратор**

### Фаза 3: Тестирование модулей (1 неделя)
1. **Создать unit тесты для каждого модуля**
2. **Настроить integration тесты**
3. **Проверить независимость модулей**
4. **Достичь 100% покрытия тестами**

## 🛠️ Инструменты для работы с модулями

### Cursor IDE настройки:
1. **Отдельные чаты для модулей** - @PhysicsModule, @MudDeformationModule
2. **Фокус на контексте** - работа с одним модулем за раз
3. **Шаблоны для ECS** - генерация компонентов и систем
4. **Автоматическое тестирование** - тесты после каждого изменения

### Git workflow:
1. **Feature branches по модулям** - feature/physics-module
2. **Модульные commit messages** - feat(physics): add wheel physics
3. **Code review по модулям** - проверка зависимостей
4. **Release по модулям** - независимые версии модулей

## 📊 Метрики успеха

- **Независимость:** Каждый модуль компилируется отдельно
- **Тестируемость:** 100% покрытие тестами для каждого модуля
- **Производительность:** 60+ FPS с всеми модулями
- **Масштабируемость:** Легкое добавление новых модулей
- **Поддерживаемость:** Четкие интерфейсы и документация

---

**Цель:** Создать модульную архитектуру, которая позволит независимо разрабатывать, тестировать и масштабировать каждый аспект игры Mud-Like.
