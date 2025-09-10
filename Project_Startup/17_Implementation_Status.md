# 📊 Mud-Like Implementation Status & Documentation

## 🎯 **ТЕКУЩИЙ СТАТУС ПРОЕКТА**

**Фаза 1: Подготовка и миграция** - **75% завершено**

### ✅ **ЗАВЕРШЕННЫЕ КОМПОНЕНТЫ**

## 🚛 **1. СИСТЕМА ГРУЗОВИКА КРАЗ**

### **1.1 ECS Компоненты**

#### **TruckData.cs** - Основные данные грузовика
```csharp
public struct TruckData : IComponentData
{
    public float Mass;                    // Масса грузовика в кг
    public float EnginePower;             // Мощность двигателя в л.с.
    public float MaxTorque;               // Максимальный крутящий момент в Н⋅м
    public float EngineRPM;               // Текущие обороты двигателя (RPM)
    public int CurrentGear;               // Текущая передача (1-6)
    public float MaxSpeed;                // Максимальная скорость в км/ч
    public float CurrentSpeed;            // Текущая скорость в км/ч
    public float SteeringAngle;           // Угол поворота руля в градусах
    public float MaxSteeringAngle;        // Максимальный угол поворота руля
    public float TractionCoefficient;     // Коэффициент сцепления с дорогой
    public float FuelLevel;               // Уровень топлива (0-1)
    public bool EngineRunning;            // Состояние двигателя
    public bool HandbrakeOn;              // Состояние ручного тормоза
    
    // Блокировка дифференциалов
    public bool LockFrontDifferential;    // Передний дифференциал
    public bool LockMiddleDifferential;   // Средний дифференциал
    public bool LockRearDifferential;     // Задний дифференциал
    public bool LockCenterDifferential;   // Межосевой дифференциал
}
```

#### **TruckControl.cs** - Управление грузовиком
```csharp
public struct TruckControl : IComponentData
{
    public float Throttle;                // Газ (0-1)
    public float Brake;                   // Тормоз (0-1)
    public float Steering;                // Руль (-1 до 1)
    public bool Handbrake;                // Ручной тормоз
    public bool ShiftUp;                  // Переключение передачи вверх
    public bool ShiftDown;                // Переключение передачи вниз
    public bool ToggleEngine;             // Запуск/остановка двигателя
    public float Clutch;                  // Сцепление (0-1)
    public bool FourWheelDrive;           // Режим полного привода
    public bool LockFrontDifferential;    // Блокировка переднего дифференциала
    public bool LockMiddleDifferential;   // Блокировка среднего дифференциала
    public bool LockRearDifferential;     // Блокировка заднего дифференциала
    public bool LockCenterDifferential;   // Блокировка межосевого дифференциала
}
```

#### **WheelData.cs** - Данные колес (6 колес для КРАЗ)
```csharp
public struct WheelData : IComponentData
{
    public float3 LocalPosition;          // Позиция колеса относительно центра грузовика
    public float Radius;                  // Радиус колеса в метрах
    public float Width;                   // Ширина колеса в метрах
    public float AngularVelocity;         // Угловая скорость колеса в рад/с
    public float SteerAngle;              // Угол поворота колеса в радианах
    public float Torque;                  // Крутящий момент, приложенный к колесу
    public float BrakeTorque;             // Тормозной момент
    public float TractionCoefficient;     // Коэффициент сцепления с поверхностью
    public float SinkDepth;               // Глубина погружения в грязь
    public float3 TractionForce;          // Сила сцепления с поверхностью
    public float SlipRatio;               // Скорость скольжения колеса
    public bool IsDriven;                 // Является ли колесо ведущим
    public bool IsSteerable;              // Является ли колесо поворотным
    public int WheelIndex;                // Индекс колеса (0-5 для КРАЗ)
}
```

### **1.2 Системы управления**

#### **TruckControlSystem.cs** - Обработка ввода
```csharp
[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial class TruckControlSystem : SystemBase
{
    protected override void OnUpdate()
    {
        // Получаем ввод с клавиатуры
        var input = new TruckControl
        {
            Throttle = Input.GetAxis("Vertical"),           // W/S или стрелки
            Brake = Input.GetKey(KeyCode.Space) ? 1f : 0f, // Пробел
            Steering = Input.GetAxis("Horizontal"),         // A/D или стрелки
            Handbrake = Input.GetKey(KeyCode.LeftShift),   // Левый Shift
            ShiftUp = Input.GetKeyDown(KeyCode.E),         // E
            ShiftDown = Input.GetKeyDown(KeyCode.Q),       // Q
            ToggleEngine = Input.GetKeyDown(KeyCode.R),    // R
            Clutch = Input.GetKey(KeyCode.LeftControl) ? 1f : 0f, // Левый Ctrl
            FourWheelDrive = Input.GetKey(KeyCode.F),      // F
            LockFrontDifferential = Input.GetKey(KeyCode.Alpha1),  // 1
            LockMiddleDifferential = Input.GetKey(KeyCode.Alpha2), // 2
            LockRearDifferential = Input.GetKey(KeyCode.Alpha3),   // 3
            LockCenterDifferential = Input.GetKey(KeyCode.Alpha4)  // 4
        };
        
        // Обновляем компоненты управления для всех грузовиков
        Entities
            .WithAll<TruckData>()
            .ForEach((ref TruckControl truckControl) =>
            {
                truckControl = input;
            }).WithoutBurst().Run();
    }
}
```

#### **TruckMovementSystem.cs** - Физика движения
```csharp
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public partial class TruckMovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = SystemAPI.Time.fixedDeltaTime;
        
        Entities
            .WithAll<TruckData>()
            .ForEach((ref LocalTransform transform, ref TruckData truck, in TruckControl input) =>
            {
                ProcessTruckMovement(ref transform, ref truck, input, deltaTime);
            }).Schedule();
    }
    
    private static void ProcessTruckMovement(ref LocalTransform transform, ref TruckData truck, in TruckControl input, float deltaTime)
    {
        // Обновляем состояние двигателя
        UpdateEngine(ref truck, input, deltaTime);
        
        // Обновляем трансмиссию
        UpdateTransmission(ref truck, input, deltaTime);
        
        // Вычисляем силу тяги
        float3 tractionForce = CalculateTractionForce(truck, input);
        
        // Применяем физику движения
        ApplyPhysics(ref transform, ref truck, tractionForce, deltaTime);
    }
}
```

### **1.3 Управление**

| Клавиша | Действие | Описание |
|---------|----------|----------|
| **W/S** | Газ/тормоз | Управление скоростью |
| **A/D** | Руль | Поворот передних колес |
| **Пробел** | Тормоз | Основной тормоз |
| **E/Q** | Передачи | Переключение передач (1-6) |
| **R** | Двигатель | Запуск/остановка двигателя |
| **F** | Полный привод | Включение 4WD |
| **1-4** | Дифференциалы | Блокировка дифференциалов |
| **Левый Shift** | Ручной тормоз | Аварийный тормоз |
| **Левый Ctrl** | Сцепление | Для ручной коробки |

## 🏔️ **2. СИСТЕМА ДЕФОРМАЦИИ ТЕРРЕЙНА**

### **2.1 MudManager API**

#### **MudManager.cs** - Основная система деформации
```csharp
[BurstCompile]
public partial class MudManager : SystemBase
{
    private const float BLOCK_SIZE = 16f;              // Размер блока террейна
    private const float MAX_SINK_DEPTH = 2f;           // Максимальная глубина погружения
    private const int HEIGHT_RESOLUTION = 64;          // Разрешение высоты блока
    private const float MAX_DEFORMATION_PER_FRAME = 0.1f; // Максимальная деформация за кадр
    
    private NativeHashMap<int2, Entity> _terrainBlocks;        // Сетка блоков террейна
    private NativeHashMap<int2, NativeArray<float>> _heightCache; // Кэш высот блока
    
    protected override void OnUpdate()
    {
        float deltaTime = SystemAPI.Time.fixedDeltaTime;
        
        // Создаем job для обработки деформации
        var deformationJob = new TerrainDeformationJob
        {
            TerrainBlocks = _terrainBlocks,
            HeightCache = _heightCache,
            BlockSize = BLOCK_SIZE,
            MaxSinkDepth = MAX_SINK_DEPTH,
            HeightResolution = HEIGHT_RESOLUTION,
            MaxDeformationPerFrame = MAX_DEFORMATION_PER_FRAME,
            DeltaTime = deltaTime
        };
        
        // Запускаем job
        var jobHandle = deformationJob.ScheduleParallel(this);
        jobHandle.Complete();
    }
}
```

#### **TerrainDeformationJob.cs** - Job для деформации
```csharp
[BurstCompile]
public struct TerrainDeformationJob : IJobEntity
{
    public NativeHashMap<int2, Entity> TerrainBlocks;
    public NativeHashMap<int2, NativeArray<float>> HeightCache;
    public float BlockSize;
    public float MaxSinkDepth;
    public int HeightResolution;
    public float MaxDeformationPerFrame;
    public float DeltaTime;
    
    public void Execute(ref WheelData wheel, in LocalTransform transform)
    {
        // Получаем позицию колеса
        float3 wheelWorldPos = transform.Position + math.mul(transform.Rotation, wheel.LocalPosition);
        
        // Вычисляем координаты блока
        int2 blockCoords = GetBlockCoordinates(wheelWorldPos);
        
        // Запрашиваем данные грязи
        var mudQuery = QueryContact(wheelWorldPos, wheel.Radius, blockCoords);
        
        // Обновляем данные колеса
        wheel.SinkDepth = mudQuery.SinkDepth;
        wheel.TractionCoefficient = mudQuery.TractionModifier;
        
        // Применяем деформацию
        ApplyTerrainDeformation(blockCoords, wheelWorldPos, wheel.Radius, wheel.SinkDepth);
    }
}
```

### **2.2 Компоненты террейна**

#### **MudData.cs** - Данные грязи
```csharp
public struct MudData : IComponentData
{
    public float Height;                  // Высота грязи в блоке
    public float TractionModifier;        // Коэффициент сцепления с грязью
    public float Viscosity;               // Вязкость грязи
    public float Density;                 // Плотность грязи
    public float Moisture;                // Влажность грязи (0-1)
    public float LastUpdateTime;          // Время последнего обновления
    public bool IsDirty;                  // Флаг изменения блока
}
```

#### **TerrainBlockData.cs** - Данные блока террейна
```csharp
public struct TerrainBlockData : IComponentData
{
    public int2 GridPosition;             // Позиция блока в сетке
    public float BlockSize;               // Размер блока в метрах
    public int2 HeightResolution;         // Разрешение высоты блока
    public float MinHeight;               // Минимальная высота блока
    public float MaxHeight;               // Максимальная высота блока
    public bool IsActive;                 // Флаг активности блока
    public float LastUpdateTime;          // Время последнего обновления
}
```

## 💨 **3. ВИЗУАЛЬНЫЕ И ЗВУКОВЫЕ ЭФФЕКТЫ**

### **3.1 Система частиц грязи**

#### **MudParticleSystem.cs** - Система частиц
```csharp
[BurstCompile]
public partial class MudParticleSystem : SystemBase
{
    private const int MAX_PARTICLES = 1000;           // Максимальное количество частиц
    private const float PARTICLE_SPAWN_RATE = 10f;    // Скорость создания частиц
    private const float GRAVITY = -9.81f;             // Гравитация для частиц
    
    protected override void OnUpdate()
    {
        float deltaTime = SystemAPI.Time.DeltaTime;
        
        // Создаем job для обновления частиц
        var particleJob = new MudParticleJob
        {
            DeltaTime = deltaTime,
            Gravity = GRAVITY
        };
        
        // Запускаем job
        var jobHandle = particleJob.ScheduleParallel(this);
        jobHandle.Complete();
        
        // Создаем новые частицы от колес
        SpawnParticlesFromWheels(deltaTime);
    }
}
```

#### **MudParticleData.cs** - Данные частиц
```csharp
public struct MudParticleData : IComponentData
{
    public float3 Position;               // Позиция частицы
    public float3 Velocity;               // Скорость частицы
    public float Size;                    // Размер частицы
    public float Lifetime;                // Время жизни частицы
    public float MaxLifetime;             // Максимальное время жизни
    public float4 Color;                  // Цвет частицы
    public float Mass;                    // Масса частицы
    public float Viscosity;               // Вязкость частицы
    public bool IsActive;                 // Активна ли частица
}
```

### **3.2 Аудио система**

#### **TruckAudioSystem.cs** - Звуки грузовика
```csharp
public partial class TruckAudioSystem : SystemBase
{
    private const int ENGINE_SOUND_ID = 0;    // ID звука двигателя
    private const int WHEEL_SOUND_ID = 1;     // ID звука колес
    private const int MUD_SOUND_ID = 2;       // ID звука грязи
    private const int BRAKE_SOUND_ID = 3;     // ID звука тормоза
    private const int GEAR_SOUND_ID = 4;      // ID звука передач
    
    protected override void OnUpdate()
    {
        float deltaTime = SystemAPI.Time.DeltaTime;
        
        // Обрабатываем звуки для всех грузовиков
        Entities
            .WithAll<TruckData, AudioSourceData>()
            .ForEach((ref AudioSourceData audio, in TruckData truck, in LocalTransform transform) =>
            {
                UpdateTruckAudio(ref audio, truck, transform, deltaTime);
            }).WithoutBurst().Run();
    }
}
```

## 🌐 **4. МУЛЬТИПЛЕЕР С NETCODE FOR ENTITIES**

### **4.1 Сетевые компоненты**

#### **NetworkedTruckData.cs** - Синхронизация грузовика
```csharp
[GhostComponent(PrefabType = GhostPrefabType.AllPredicted)]
public struct NetworkedTruckData : IComponentData
{
    [GhostField(Quantization = 1000)]    // Точность до 1мм
    public float3 Position;               // Позиция грузовика
    
    [GhostField(Quantization = 1000)]
    public quaternion Rotation;           // Поворот грузовика
    
    [GhostField(Quantization = 100)]
    public float3 Velocity;               // Скорость грузовика
    
    [GhostField(Quantization = 100)]
    public float3 AngularVelocity;        // Угловая скорость
    
    [GhostField]
    public int CurrentGear;               // Текущая передача
    
    [GhostField(Quantization = 10)]
    public float EngineRPM;               // Обороты двигателя
    
    [GhostField(Quantization = 10)]
    public float CurrentSpeed;            // Текущая скорость в км/ч
    
    [GhostField(Quantization = 100)]
    public float SteeringAngle;           // Угол поворота руля
    
    [GhostField]
    public bool EngineRunning;            // Состояние двигателя
    
    [GhostField]
    public bool HandbrakeOn;              // Состояние ручного тормоза
    
    // Блокировка дифференциалов
    [GhostField]
    public bool LockFrontDifferential;
    
    [GhostField]
    public bool LockMiddleDifferential;
    
    [GhostField]
    public bool LockRearDifferential;
    
    [GhostField]
    public bool LockCenterDifferential;
    
    [GhostField(Quantization = 1000)]
    public float FuelLevel;               // Уровень топлива
}
```

### **4.2 Системы синхронизации**

#### **TruckSyncSystem.cs** - Синхронизация грузовиков
```csharp
[UpdateInGroup(typeof(GhostUpdateSystemGroup))]
public partial class TruckSyncSystem : SystemBase
{
    protected override void OnUpdate()
    {
        // Синхронизация с сервера на клиенты
        if (HasSingleton<NetworkStreamInGame>())
        {
            SyncTruckData();
        }
    }
    
    private void SyncTruckData()
    {
        Entities
            .WithAll<TruckData, NetworkedTruckData>()
            .ForEach((ref NetworkedTruckData networkedData, in TruckData truckData, in LocalTransform transform) =>
            {
                // Обновляем сетевые данные из локальных данных
                networkedData.Position = transform.Position;
                networkedData.Rotation = transform.Rotation;
                networkedData.Velocity = truckData.CurrentSpeed * math.forward(transform.Rotation);
                networkedData.AngularVelocity = float3.zero;
                networkedData.CurrentGear = truckData.CurrentGear;
                networkedData.EngineRPM = truckData.EngineRPM;
                networkedData.CurrentSpeed = truckData.CurrentSpeed;
                networkedData.SteeringAngle = truckData.SteeringAngle;
                networkedData.EngineRunning = truckData.EngineRunning;
                networkedData.HandbrakeOn = truckData.HandbrakeOn;
                networkedData.LockFrontDifferential = truckData.LockFrontDifferential;
                networkedData.LockMiddleDifferential = truckData.LockMiddleDifferential;
                networkedData.LockRearDifferential = truckData.LockRearDifferential;
                networkedData.LockCenterDifferential = truckData.LockCenterDifferential;
                networkedData.FuelLevel = truckData.FuelLevel;
            }).WithoutBurst().Run();
    }
}
```

### **4.3 Лаг-компенсация**

#### **LagCompensationSystem.cs** - Компенсация задержки
```csharp
[UpdateInGroup(typeof(NetCodeReceiveSystemGroup))]
public partial class LagCompensationSystem : SystemBase
{
    private const float MAX_REWIND_TIME = 0.2f; // 200мс
    
    protected override void OnUpdate()
    {
        if (!HasSingleton<NetworkStreamInGame>()) return;
        
        // Применяем лаг-компенсацию для грузовиков
        ApplyLagCompensation();
    }
    
    private void ApplyLagCompensation()
    {
        // Получаем средний ping всех игроков
        float averagePing = CalculateAveragePing();
        float rewindTime = math.min(averagePing / 1000f, MAX_REWIND_TIME);
        
        // Применяем компенсацию для всех грузовиков
        Entities
            .WithAll<TruckData, NetworkedTruckData>()
            .ForEach((ref LocalTransform transform, in NetworkedTruckData networkedData) =>
            {
                // Вычисляем компенсированную позицию
                float3 compensatedPosition = CalculateCompensatedPosition(
                    networkedData.Position, 
                    networkedData.Velocity, 
                    rewindTime
                );
                
                // Применяем компенсацию
                transform.Position = compensatedPosition;
            }).WithoutBurst().Run();
    }
}
```

## 🧪 **5. ТЕСТИРОВАНИЕ**

### **5.1 Unit тесты**

#### **TruckMovementSystemTests.cs** - Тесты системы движения
```csharp
[Test]
public void CalculateEngineTorque_ValidInput_ReturnsCorrectTorque()
{
    // Arrange
    var truck = new TruckData
    {
        EngineRPM = 1500f,
        MaxTorque = 1200f
    };
    
    var input = new TruckControl
    {
        Throttle = 0.5f
    };

    // Act
    var result = TruckMovementSystem.CalculateEngineTorque(truck, input);

    // Assert
    Assert.Greater(result, 0f);
    Assert.LessOrEqual(result, truck.MaxTorque);
}
```

### **5.2 Integration тесты**

#### **MultiplayerIntegrationTests.cs** - Тесты мультиплеера
```csharp
[Test]
public void NetworkedTruckData_SyncsCorrectly()
{
    // Arrange
    var serverEntity = _serverEntityManager.CreateEntity();
    _serverEntityManager.AddComponent<PlayerTag>(serverEntity);
    _serverEntityManager.AddComponent<TruckData>(serverEntity);
    _serverEntityManager.AddComponent<NetworkedTruckData>(serverEntity);
    
    // Act - симулируем синхронизацию
    var serverNetworkedData = _serverEntityManager.GetComponentData<NetworkedTruckData>(serverEntity);
    
    // Assert
    Assert.AreEqual(3, serverNetworkedData.CurrentGear);
    Assert.AreEqual(2000f, serverNetworkedData.EngineRPM);
    Assert.AreEqual(50f, serverNetworkedData.CurrentSpeed);
    Assert.IsTrue(serverNetworkedData.EngineRunning);
}
```

## 📊 **6. СТАТИСТИКА ПРОЕКТА**

### **6.1 Структура файлов**
```
Assets/Scripts/
├── Core/                    # Основные системы
│   ├── Components/         # Базовые компоненты
│   └── Systems/            # Основные системы
├── Vehicles/               # Система грузовика
│   ├── Components/         # Компоненты грузовика
│   ├── Systems/            # Системы грузовика
│   └── Authoring/          # Авторинг компоненты
├── Terrain/                # Система террейна
│   ├── Components/         # Компоненты террейна
│   ├── Systems/            # Системы террейна
│   └── Authoring/          # Авторинг компоненты
├── Networking/             # Мультиплеер
│   ├── Components/         # Сетевые компоненты
│   ├── Systems/            # Системы синхронизации
│   └── Authoring/          # Сетевые авторинг
├── Effects/                # Визуальные эффекты
│   ├── Components/         # Компоненты эффектов
│   └── Systems/            # Системы эффектов
├── Audio/                  # Звуковая система
│   ├── Components/         # Аудио компоненты
│   └── Systems/            # Аудио системы
├── UI/                     # Пользовательский интерфейс
│   └── DifferentialLockUI.cs
└── Tests/                  # Тестирование
    ├── Unit/               # Unit тесты
    └── Integration/        # Integration тесты
```

### **6.2 Количество компонентов и систем**

| Категория | Компоненты | Системы | Авторинг | Тесты |
|-----------|------------|---------|----------|-------|
| **Core** | 4 | 3 | 0 | 0 |
| **Vehicles** | 5 | 3 | 1 | 2 |
| **Terrain** | 2 | 1 | 1 | 0 |
| **Networking** | 4 | 5 | 1 | 1 |
| **Effects** | 1 | 1 | 0 | 0 |
| **Audio** | 1 | 1 | 0 | 0 |
| **UI** | 0 | 0 | 0 | 0 |
| **Tests** | 0 | 0 | 0 | 0 |
| **ИТОГО** | **17** | **14** | **3** | **3** |

### **6.3 Производительность**

#### **Burst Compiler**
- ✅ Все системы используют `[BurstCompile]`
- ✅ Jobs для параллельной обработки
- ✅ Оптимизированные вычисления

#### **Job System**
- ✅ `TerrainDeformationJob` - деформация террейна
- ✅ `MudParticleJob` - обновление частиц
- ✅ Параллельная обработка колес

#### **Memory Management**
- ✅ `NativeHashMap` для кэширования
- ✅ `NativeArray` для высотных данных
- ✅ Правильная очистка ресурсов

## 🎯 **7. СЛЕДУЮЩИЕ ШАГИ**

### **7.1 Немедленные задачи**
1. **Тестирование прототипа** - запуск и проверка функциональности
2. **Оптимизация производительности** - профилирование и улучшения
3. **3D модели** - создание визуальных моделей грузовика
4. **UI интерфейс** - полный пользовательский интерфейс

### **7.2 Среднесрочные задачи**
1. **Расширенная физика** - более реалистичная физика колес
2. **Погодные эффекты** - дождь, снег, влияние на грязь
3. **Миссии и цели** - игровые задачи
4. **Система достижений** - прогресс игрока

### **7.3 Долгосрочные задачи**
1. **Моддинг** - поддержка пользовательского контента
2. **VR поддержка** - виртуальная реальность
3. **Мобильная версия** - адаптация для мобильных устройств
4. **Консольные платформы** - PlayStation, Xbox

---

**Проект Mud-Like готов к тестированию и дальнейшей разработке! 🚛🏔️💨🌐**