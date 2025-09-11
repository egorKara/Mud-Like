# 📁 Mud-Like Project Structure

## 🎯 **ОБЩАЯ СТРУКТУРА**

### **Корневая структура проекта**
```
Mud-Like/
├── Assets/                    # Основные ресурсы Unity
├── Packages/                  # Пакеты Unity
├── ProjectSettings/           # Настройки проекта
├── UserSettings/             # Пользовательские настройки
├── .github/                  # GitHub Actions
├── Docs/                     # Документация
└── README.md                 # Основной README
```

## 📂 **СТРУКТУРА ASSETS**

### **Основные директории**
```
Assets/
├── Scripts/                  # Все скрипты проекта
│   ├── Core/                 # Основные системы
│   ├── Vehicles/             # Транспорт (ECS + Unity Physics)
│   ├── Terrain/              # Террейн и деформация (DOTS)
│   ├── UI/                   # Пользовательский интерфейс
│   ├── Pooling/              # Object Pooling
│   ├── Networking/           # Мультиплеер (Netcode)
│   ├── Audio/                # Звуковая система
│   ├── Effects/              # Визуальные эффекты
│   ├── Tests/                # Тесты
│   └── DOTS/                 # DOTS системы (активные)
├── Prefabs/                  # Префабы
├── Materials/                # Материалы
├── Textures/                 # Текстуры
├── Audio/                    # Аудио файлы
├── Scenes/                   # Сцены
└── Tests/                    # Тестовые сцены
```

## 🏗️ **СТРУКТУРА SCRIPTS**

### **1. Core/ - Основные системы**

#### **Текущее состояние (реальное):**
```
Scripts/Core/
├── Components/               # ECS компоненты
│   ├── PlayerInput.cs        # Компонент ввода игрока
│   ├── PlayerTag.cs          # Тег игрока
│   ├── Position.cs           # Компонент позиции
│   └── Velocity.cs           # Компонент скорости
├── Systems/                  # ECS системы
│   └── PlayerMovementSystem.cs # Система движения игрока
└── MudLike.Core.asmdef       # Assembly Definition
```

#### **Планируемая структура (ориентир):**
```
Scripts/Core/
├── BaseSingleton.cs          # Базовый класс Singleton
├── GameManager.cs            # Основной менеджер игры
├── EventSystem.cs            # Система событий
├── Pooling/                  # Object Pooling
│   ├── PooledObject.cs       # Базовый класс для пулинга
│   ├── GenericObjectPool.cs  # Универсальный пул объектов
│   ├── ObjectPool.cs         # Пул объектов
│   ├── PoolManager.cs        # Менеджер пулов
│   └── PoolStats.cs          # Статистика пулов
└── Utils/                    # Утилиты
    ├── MathUtils.cs          # Математические утилиты
    ├── DebugUtils.cs         # Утилиты отладки
    └── ExtensionMethods.cs   # Методы расширения
```

### **2. Vehicles/ - Транспорт**

#### **Текущее состояние (реальное):**
```
Scripts/Vehicles/
├── MudLike.Vehicles.asmdef   # Assembly Definition (пустой)
```

#### **Планируемая структура (ориентир):**
```
Scripts/Vehicles/
├── Components/               # ECS компоненты
│   ├── VehicleData.cs        # Данные транспорта
│   ├── WheelData.cs          # Данные колес
│   ├── SuspensionData.cs     # Данные подвески
│   └── EngineData.cs         # Данные двигателя
├── Systems/                  # ECS системы
│   ├── VehicleMovementSystem.cs
│   ├── WheelPhysicsSystem.cs
│   ├── SuspensionSystem.cs
│   └── EngineSystem.cs
├── Controllers/              # Контроллеры
│   ├── VehicleController.cs  # Основной контроллер
│   ├── WheelController.cs    # Контроллер колес
│   └── CameraController.cs   # Контроллер камеры
└── Prefabs/                  # Префабы транспорта
    ├── Vehicle.prefab
    ├── Wheel.prefab
    └── Camera.prefab
```

### **3. Terrain/ - Террейн и деформация**

#### **Текущее состояние (реальное):**
```
Scripts/Terrain/
├── MudLike.Terrain.asmdef    # Assembly Definition (пустой)
```

#### **Планируемая структура (ориентир):**
```
Scripts/Terrain/
├── Components/               # ECS компоненты
│   ├── TerrainData.cs        # Данные террейна
│   ├── DeformationData.cs    # Данные деформации
│   ├── MudData.cs            # Данные грязи
│   └── ChunkData.cs          # Данные чанков
├── Systems/                  # ECS системы
│   ├── TerrainDeformationSystem.cs
│   ├── MudGenerationSystem.cs
│   ├── ChunkManagementSystem.cs
│   └── TerrainSyncSystem.cs
├── Managers/                 # Менеджеры
│   ├── TerrainManager.cs     # Менеджер террейна
│   ├── MudManager.cs         # Менеджер грязи
│   └── ChunkManager.cs       # Менеджер чанков
└── Utils/                    # Утилиты
    ├── TerrainUtils.cs       # Утилиты террейна
    ├── DeformationUtils.cs   # Утилиты деформации
    └── MudUtils.cs           # Утилиты грязи
```

### **4. UI/ - Пользовательский интерфейс**

#### **Текущее состояние (реальное):**
```
Scripts/UI/
# Пустая папка для будущих UI компонентов
```

#### **Планируемая структура (ориентир):**
```
Scripts/UI/
├── Components/               # UI компоненты
│   ├── MainMenuUI.cs         # Главное меню
│   ├── LobbyUI.cs            # Лобби
│   ├── GameHUD.cs            # Игровой HUD
│   └── SettingsUI.cs         # Настройки
├── Managers/                 # UI менеджеры
│   ├── UIManager.cs          # Основной UI менеджер
│   ├── MenuManager.cs        # Менеджер меню
│   └── HUDManager.cs         # Менеджер HUD
├── UXML/                     # UXML файлы
│   ├── MainMenu.uxml
│   ├── Lobby.uxml
│   ├── GameHUD.uxml
│   └── Settings.uxml
└── USS/                      # USS стили
    ├── MainMenu.uss
    ├── Lobby.uss
    ├── GameHUD.uss
    └── Settings.uss
```

### **5. Pooling/ - Object Pooling**

#### **Текущее состояние (реальное):**
```
Scripts/Pooling/
# Пустая папка для будущих систем пулинга
```

#### **Планируемая структура (ориентир):**
```
Scripts/Pooling/
├── PooledObject.cs           # Базовый класс для пулинга
├── GenericObjectPool.cs      # Универсальный пул
├── ObjectPool.cs             # Пул объектов
├── PoolManager.cs            # Менеджер пулов
├── PoolStats.cs              # Статистика пулов
└── Effects/                  # Эффекты для пулинга
    ├── ParticleEffect.cs     # Эффект частиц
    ├── DeformationEffect.cs  # Эффект деформации
    └── MudSplashEffect.cs    # Эффект брызг грязи
```

### **6. Networking/ - Мультиплеер**

#### **Текущее состояние (реальное):**
```
Scripts/Networking/
├── MudLike.Networking.asmdef # Assembly Definition (пустой)
```

#### **Планируемая структура (ориентир):**
```
Scripts/Networking/
├── Components/               # Сетевые компоненты
│   ├── NetworkPosition.cs    # Сетевая позиция
│   ├── NetworkRotation.cs    # Сетевое вращение
│   ├── NetworkVelocity.cs    # Сетевая скорость
│   └── PlayerTag.cs          # Тег игрока
├── Systems/                  # Сетевые системы
│   ├── SendPositionToServerSystem.cs
│   ├── ReceivePositionFromServerSystem.cs
│   ├── NetworkSyncSystem.cs
│   └── LagCompensationSystem.cs
├── Commands/                 # Команды
│   ├── MoveCommand.cs        # Команда движения
│   ├── ShootCommand.cs       # Команда стрельбы
│   └── InteractCommand.cs    # Команда взаимодействия
├── Events/                   # События
│   ├── PositionEvent.cs      # Событие позиции
│   ├── DamageEvent.cs        # Событие урона
│   └── TerrainEvent.cs       # Событие террейна
└── Managers/                 # Сетевые менеджеры
    ├── NetworkManager.cs     # Основной сетевой менеджер
    ├── ClientManager.cs      # Менеджер клиента
    └── ServerManager.cs      # Менеджер сервера
```

### **7. Audio/ - Звуковая система**

#### **Текущее состояние (реальное):**
```
Scripts/Audio/
# Пустая папка для будущих аудио компонентов
```

#### **Планируемая структура (ориентир):**
```
Scripts/Audio/
├── Components/               # Аудио компоненты
│   ├── AudioSourceData.cs    # Данные аудио источника
│   ├── AudioClipData.cs      # Данные аудио клипа
│   └── AudioMixerData.cs     # Данные аудио микшера
├── Systems/                  # Аудио системы
│   ├── AudioPlaybackSystem.cs
│   ├── Audio3DSystem.cs
│   └── AudioPoolingSystem.cs
├── Managers/                 # Аудио менеджеры
│   ├── AudioManager.cs       # Основной аудио менеджер
│   ├── MusicManager.cs       # Менеджер музыки
│   └── SFXManager.cs         # Менеджер звуковых эффектов
└── Clips/                    # Аудио клипы
    ├── Engine/               # Звуки двигателя
    ├── Wheels/               # Звуки колес
    ├── Environment/          # Звуки окружения
    └── UI/                   # Звуки интерфейса
```

### **8. Effects/ - Визуальные эффекты**

#### **Текущее состояние (реальное):**
```
Scripts/Effects/
# Пустая папка для будущих визуальных эффектов
```

#### **Планируемая структура (ориентир):**
```
Scripts/Effects/
├── Components/               # Компоненты эффектов
│   ├── ParticleEffectData.cs # Данные эффекта частиц
│   ├── TrailEffectData.cs    # Данные эффекта следа
│   └── DecalEffectData.cs    # Данные эффекта декали
├── Systems/                  # Системы эффектов
│   ├── ParticleEffectSystem.cs
│   ├── TrailEffectSystem.cs
│   └── DecalEffectSystem.cs
├── Managers/                 # Менеджеры эффектов
│   ├── EffectManager.cs      # Основной менеджер эффектов
│   ├── ParticleManager.cs    # Менеджер частиц
│   └── DecalManager.cs       # Менеджер декалей
└── Prefabs/                  # Префабы эффектов
    ├── MudSplash.prefab
    ├── WheelTrail.prefab
    └── DeformationDecal.prefab
```

### **9. Tests/ - Тесты**

#### **Текущее состояние (реальное):**
```
Scripts/Tests/
├── MudLike.Tests.asmdef      # Assembly Definition (пустой)
```

#### **Планируемая структура (ориентир):**
```
Scripts/Tests/
├── Unit/                     # Модульные тесты
│   ├── MovementSystemTests.cs
│   ├── TerrainDeformationTests.cs
│   └── NetworkSyncTests.cs
├── Integration/              # Интеграционные тесты
│   ├── VehiclePhysicsTests.cs
│   ├── TerrainInteractionTests.cs
│   └── NetworkCommunicationTests.cs
├── Performance/              # Тесты производительности
│   ├── PerformanceTests.cs
│   ├── MemoryTests.cs
│   └── NetworkPerformanceTests.cs
└── PlayMode/                 # PlayMode тесты
    ├── GameplayTests.cs
    ├── MultiplayerTests.cs
    └── UITests.cs
```

### **10. DOTS/ - DOTS системы**

#### **Текущее состояние (реальное):**
```
Scripts/DOTS/
├── MudLike.DOTS.asmdef       # Assembly Definition (пустой)
```

#### **Планируемая структура (ориентир):**
```
Scripts/DOTS/
├── Components/               # ECS компоненты
│   ├── Position.cs           # Позиция
│   ├── Velocity.cs           # Скорость
│   ├── Rotation.cs           # Вращение
│   ├── Health.cs             # Здоровье
│   └── PlayerInput.cs        # Ввод игрока
├── Systems/                  # ECS системы
│   ├── MovementSystem.cs     # Система движения
│   ├── PhysicsSystem.cs      # Физическая система
│   ├── InputSystem.cs        # Система ввода
│   └── RenderingSystem.cs    # Система рендеринга
├── Tags/                     # Теги
│   ├── PlayerTag.cs          # Тег игрока
│   ├── VehicleTag.cs         # Тег транспорта
│   └── TerrainTag.cs         # Тег террейна
└── Data/                     # Данные
    ├── GameData.cs           # Игровые данные
    ├── NetworkData.cs        # Сетевые данные
    └── PhysicsData.cs        # Физические данные
```

## 🎨 **СТРУКТУРА РЕСУРСОВ**

### **Prefabs/ - Префабы**
```
Prefabs/
├── Vehicles/                 # Префабы транспорта
│   ├── Car.prefab
│   ├── Truck.prefab
│   └── Trailer.prefab
├── Terrain/                  # Префабы террейна
│   ├── MudChunk.prefab
│   ├── TerrainChunk.prefab
│   └── DeformationMarker.prefab
├── Effects/                  # Префабы эффектов
│   ├── MudSplash.prefab
│   ├── WheelTrail.prefab
│   └── DeformationDecal.prefab
└── UI/                       # Префабы UI
    ├── MainMenu.prefab
    ├── Lobby.prefab
    └── GameHUD.prefab
```

### **Materials/ - Материалы**
```
Materials/
├── Vehicles/                 # Материалы транспорта
│   ├── CarBody.mat
│   ├── Wheel.mat
│   └── Glass.mat
├── Terrain/                  # Материалы террейна
│   ├── Mud.mat
│   ├── Grass.mat
│   └── Rock.mat
├── Effects/                  # Материалы эффектов
│   ├── MudSplash.mat
│   ├── WheelTrail.mat
│   └── DeformationDecal.mat
└── UI/                       # Материалы UI
    ├── Button.mat
    ├── Panel.mat
    └── Text.mat
```

### **Textures/ - Текстуры**
```
Textures/
├── Vehicles/                 # Текстуры транспорта
│   ├── CarBody_Diffuse.png
│   ├── CarBody_Normal.png
│   └── Wheel_Diffuse.png
├── Terrain/                  # Текстуры террейна
│   ├── Mud_Diffuse.png
│   ├── Mud_Normal.png
│   └── Grass_Diffuse.png
├── Effects/                  # Текстуры эффектов
│   ├── MudSplash_Diffuse.png
│   ├── WheelTrail_Diffuse.png
│   └── DeformationDecal_Diffuse.png
└── UI/                       # Текстуры UI
    ├── Button_Diffuse.png
    ├── Panel_Diffuse.png
    └── Icon_Diffuse.png
```

### **Audio/ - Аудио файлы**
```
Audio/
├── Engine/                   # Звуки двигателя
│   ├── EngineIdle.wav
│   ├── EngineRev.wav
│   └── EngineStart.wav
├── Wheels/                   # Звуки колес
│   ├── WheelRoll.wav
│   ├── WheelSkid.wav
│   └── WheelSplash.wav
├── Environment/              # Звуки окружения
│   ├── Wind.wav
│   ├── Rain.wav
│   └── Birds.wav
└── UI/                       # Звуки интерфейса
    ├── ButtonClick.wav
    ├── ButtonHover.wav
    └── Notification.wav
```

## 🎬 **СТРУКТУРА СЦЕН**

### **Scenes/ - Основные сцены**
```
Scenes/
├── MainMenu.unity            # Главное меню
├── Lobby.unity               # Лобби
├── Game.unity                # Основная игровая сцена
├── Test.unity                # Тестовая сцена
└── Multiplayer.unity         # Мультиплеер сцена
```

### **Tests/ - Тестовые сцены**
```
Tests/
├── UnitTest.unity            # Сцена для модульных тестов
├── IntegrationTest.unity     # Сцена для интеграционных тестов
├── PerformanceTest.unity     # Сцена для тестов производительности
└── MultiplayerTest.unity     # Сцена для тестов мультиплеера
```

## 📋 **ASSEMBLY DEFINITIONS**

### **Основные Assembly Definitions**
```
Scripts/
├── Mud-Like.Core.asmdef       # Основные системы
├── Mud-Like.Vehicles.asmdef   # Транспорт
├── Mud-Like.Terrain.asmdef    # Террейн
├── Mud-Like.UI.asmdef         # UI
├── Mud-Like.Networking.asmdef # Сеть
├── Mud-Like.Audio.asmdef      # Аудио
├── Mud-Like.Effects.asmdef    # Эффекты
├── Mud-Like.Tests.asmdef      # Тесты
└── Mud-Like.DOTS.asmdef       # DOTS системы
```

### **Пример Assembly Definition**
```json
{
  "name": "Mud-Like.Core",
  "rootNamespace": "Mud-Like.Core",
  "references": [
    "Unity.Entities",
    "Unity.Collections",
    "Unity.Jobs",
    "Unity.Burst"
  ],
  "includePlatforms": [],
  "excludePlatforms": [],
  "allowUnsafeCode": true,
  "overrideReferences": false,
  "precompiledReferences": [],
  "autoReferenced": true,
  "defineConstraints": [],
  "versionDefines": [],
  "noEngineReferences": false
}
```

## 🎯 **ПРИНЦИПЫ ОРГАНИЗАЦИИ**

### **1. Модульность**
- **Каждый модуль** имеет свою папку
- **Четкое разделение** ответственности
- **Минимальные зависимости** между модулями

### **2. Масштабируемость**
- **Легко добавлять** новые модули
- **Простое расширение** существующих систем
- **Гибкая архитектура** для изменений

### **3. Тестируемость**
- **Отдельные папки** для тестов
- **Изолированные модули** для тестирования
- **Четкие интерфейсы** между модулями

### **4. Производительность**
- **DOTS системы** в отдельных папках
- **Оптимизированная структура** для кэширования
- **Минимальные зависимости** для быстрой компиляции

---

**Структура проекта Mud-Like организована для обеспечения модульности, масштабируемости и производительности. Каждый модуль имеет четкую ответственность и минимальные зависимости от других модулей.**
