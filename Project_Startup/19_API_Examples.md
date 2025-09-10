# 🔧 Mud-Like API Examples

## 🚛 **API ГРУЗОВИКА**

### **Создание грузовика**
```csharp
// Создание сущности грузовика
var truckEntity = EntityManager.CreateEntity();

// Добавление основных компонентов
EntityManager.AddComponent<PlayerTag>(truckEntity);
EntityManager.AddComponent<TruckData>(truckEntity);
EntityManager.AddComponent<TruckControl>(truckEntity);

// Настройка данных грузовика
var truckData = new TruckData
{
    Mass = 8000f,
    EnginePower = 300f,
    MaxTorque = 1200f,
    MaxSpeed = 80f,
    MaxSteeringAngle = 35f,
    CurrentGear = 1,
    EngineRPM = 800f,
    CurrentSpeed = 0f,
    SteeringAngle = 0f,
    TractionCoefficient = 0.8f,
    FuelLevel = 1f,
    EngineRunning = false,
    HandbrakeOn = false,
    LockFrontDifferential = false,
    LockMiddleDifferential = false,
    LockRearDifferential = false,
    LockCenterDifferential = false
};

EntityManager.SetComponentData(truckEntity, truckData);
```

### **Управление грузовиком**
```csharp
// Получение ввода
var input = new TruckControl
{
    Throttle = Input.GetAxis("Vertical"),
    Brake = Input.GetKey(KeyCode.Space) ? 1f : 0f,
    Steering = Input.GetAxis("Horizontal"),
    Handbrake = Input.GetKey(KeyCode.LeftShift),
    ShiftUp = Input.GetKeyDown(KeyCode.E),
    ShiftDown = Input.GetKeyDown(KeyCode.Q),
    ToggleEngine = Input.GetKeyDown(KeyCode.R),
    Clutch = Input.GetKey(KeyCode.LeftControl) ? 1f : 0f,
    FourWheelDrive = Input.GetKey(KeyCode.F),
    LockFrontDifferential = Input.GetKey(KeyCode.Alpha1),
    LockMiddleDifferential = Input.GetKey(KeyCode.Alpha2),
    LockRearDifferential = Input.GetKey(KeyCode.Alpha3),
    LockCenterDifferential = Input.GetKey(KeyCode.Alpha4)
};

// Применение ввода
EntityManager.SetComponentData(truckEntity, input);
```

### **Создание колес**
```csharp
// Позиции колес (6 колес для КРАЗ)
var wheelPositions = new float3[]
{
    new float3(-1.2f, 0.8f, 2.5f),   // Переднее левое
    new float3(1.2f, 0.8f, 2.5f),    // Переднее правое
    new float3(-1.2f, 0.8f, 0f),     // Среднее левое
    new float3(1.2f, 0.8f, 0f),      // Среднее правое
    new float3(-1.2f, 0.8f, -2.5f),  // Заднее левое
    new float3(1.2f, 0.8f, -2.5f)    // Заднее правое
};

for (int i = 0; i < wheelPositions.Length; i++)
{
    var wheelEntity = EntityManager.CreateEntity();
    
    // Добавление компонентов колеса
    EntityManager.AddComponent<WheelData>(wheelEntity);
    EntityManager.AddComponent<SuspensionData>(wheelEntity);
    
    // Настройка данных колеса
    var wheelData = new WheelData
    {
        LocalPosition = wheelPositions[i],
        Radius = 0.5f,
        Width = 0.3f,
        AngularVelocity = 0f,
        SteerAngle = 0f,
        Torque = 0f,
        BrakeTorque = 0f,
        TractionCoefficient = 0.8f,
        SinkDepth = 0f,
        TractionForce = float3.zero,
        SlipRatio = 0f,
        IsDriven = true, // Все колеса ведущие
        IsSteerable = i < 2, // Только передние поворотные
        WheelIndex = i
    };
    
    EntityManager.SetComponentData(wheelEntity, wheelData);
    
    // Связывание с грузовиком
    EntityManager.AddComponent<Parent>(wheelEntity);
    EntityManager.SetComponentData(wheelEntity, new Parent { Value = truckEntity });
}
```

## 🏔️ **API ДЕФОРМАЦИИ ТЕРРЕЙНА**

### **Запрос контакта с грязью**
```csharp
// Получение данных грязи для колеса
public static MudContact QueryContact(float3 wheelPosition, float wheelRadius)
{
    var result = new MudContact
    {
        SinkDepth = 0f,
        TractionModifier = 0.8f,
        Viscosity = 0.5f,
        Density = 1.2f
    };
    
    // Вычисление глубины погружения
    float terrainHeight = GetTerrainHeight(wheelPosition.xz);
    float sinkDepth = terrainHeight - wheelPosition.y;
    
    if (sinkDepth > 0)
    {
        result.SinkDepth = math.min(sinkDepth, MAX_SINK_DEPTH);
        result.TractionModifier = math.lerp(0.8f, 0.2f, result.SinkDepth / MAX_SINK_DEPTH);
        
        // Влияние вязкости на сцепление
        float viscosityFactor = math.lerp(1f, 0.5f, result.Viscosity);
        result.TractionModifier *= viscosityFactor;
    }
    
    return result;
}
```

### **Создание блока террейна**
```csharp
// Создание блока террейна
var terrainEntity = EntityManager.CreateEntity();

// Добавление компонентов террейна
EntityManager.AddComponent<TerrainBlockData>(terrainEntity);
EntityManager.AddComponent<MudData>(terrainEntity);

// Настройка данных блока
var terrainData = new TerrainBlockData
{
    GridPosition = int2.zero,
    BlockSize = 16f,
    HeightResolution = new int2(64, 64),
    MinHeight = -2f,
    MaxHeight = 2f,
    IsActive = true,
    LastUpdateTime = 0f
};

var mudData = new MudData
{
    Height = 0f,
    TractionModifier = 0.3f,
    Viscosity = 0.5f,
    Density = 1.2f,
    Moisture = 0.8f,
    LastUpdateTime = 0f,
    IsDirty = false
};

EntityManager.SetComponentData(terrainEntity, terrainData);
EntityManager.SetComponentData(terrainEntity, mudData);
```

### **Применение деформации**
```csharp
// Применение деформации террейна
private void ApplyTerrainDeformation(int2 blockCoords, float3 wheelPosition, float wheelRadius, float sinkDepth)
{
    if (sinkDepth <= 0f) return;
    
    // Получение данных высоты блока
    if (!HeightCache.TryGetValue(blockCoords, out var heightData))
    {
        heightData = new NativeArray<float>(HeightResolution * HeightResolution, Allocator.Persistent);
        HeightCache[blockCoords] = heightData;
    }
    
    // Вычисление локальных координат в блоке
    float2 localPos = wheelPosition.xz - blockCoords * BlockSize;
    float2 normalizedPos = localPos / BlockSize;
    
    // Применение деформации в радиусе колеса
    float deformationRadius = wheelRadius / BlockSize;
    int radiusInPixels = (int)math.ceil(deformationRadius * HeightResolution);
    
    for (int dy = -radiusInPixels; dy <= radiusInPixels; dy++)
    {
        for (int dx = -radiusInPixels; dx <= radiusInPixels; dx++)
        {
            int2 pixelPos = new int2(
                (int)(normalizedPos.x * HeightResolution) + dx,
                (int)(normalizedPos.y * HeightResolution) + dy
            );
            
            if (pixelPos.x < 0 || pixelPos.x >= HeightResolution ||
                pixelPos.y < 0 || pixelPos.y >= HeightResolution)
                continue;
            
            // Вычисление расстояния от центра колеса
            float2 pixelWorldPos = (pixelPos + 0.5f) / HeightResolution;
            float distance = math.length(pixelWorldPos - normalizedPos);
            
            if (distance <= deformationRadius)
            {
                // Вычисление силы деформации
                float deformationStrength = 1f - (distance / deformationRadius);
                float deformationAmount = sinkDepth * deformationStrength * MaxDeformationPerFrame;
                
                // Применение деформации
                int index = pixelPos.y * HeightResolution + pixelPos.x;
                heightData[index] = math.max(heightData[index] - deformationAmount, -MaxSinkDepth);
            }
        }
    }
}
```

## 💨 **API ЭФФЕКТОВ**

### **Создание частиц грязи**
```csharp
// Создание частиц грязи от колес
private void SpawnMudParticles(float3 position, float3 velocity, float sinkDepth)
{
    int particleCount = (int)(sinkDepth * PARTICLE_SPAWN_RATE);
    particleCount = math.min(particleCount, 5);
    
    for (int i = 0; i < particleCount; i++)
    {
        var particleEntity = EntityManager.CreateEntity();
        
        // Добавление компонентов частицы
        EntityManager.AddComponent<MudParticleData>(particleEntity);
        EntityManager.AddComponent<LocalTransform>(particleEntity);
        
        // Настройка данных частицы
        var particleData = new MudParticleData
        {
            Position = position + new float3(
                (math.random() - 0.5f) * 2f,
                math.random() * 0.5f,
                (math.random() - 0.5f) * 2f
            ),
            Velocity = velocity + new float3(
                (math.random() - 0.5f) * 5f,
                math.random() * 3f,
                (math.random() - 0.5f) * 5f
            ),
            Size = math.random() * 0.2f + 0.1f,
            Lifetime = 0f,
            MaxLifetime = math.random() * 3f + 1f,
            Color = new float4(0.4f, 0.2f, 0.1f, 1f), // Коричневый цвет грязи
            Mass = math.random() * 0.1f + 0.05f,
            Viscosity = math.random() * 0.5f + 0.3f,
            IsActive = true
        };
        
        EntityManager.SetComponentData(particleEntity, particleData);
        
        // Настройка трансформации
        var transform = new LocalTransform
        {
            Position = particleData.Position,
            Rotation = quaternion.identity,
            Scale = particleData.Size
        };
        
        EntityManager.SetComponentData(particleEntity, transform);
    }
}
```

### **Обновление частиц**
```csharp
// Job для обновления частиц грязи
[BurstCompile]
public struct MudParticleJob : IJobEntity
{
    public float DeltaTime;
    public float Gravity;
    
    public void Execute(ref MudParticleData particle, ref LocalTransform transform)
    {
        if (!particle.IsActive) return;
        
        // Обновление времени жизни
        particle.Lifetime += DeltaTime;
        
        if (particle.Lifetime >= particle.MaxLifetime)
        {
            particle.IsActive = false;
            return;
        }
        
        // Применение гравитации
        particle.Velocity.y += Gravity * DeltaTime;
        
        // Обновление позиции
        particle.Position += particle.Velocity * DeltaTime;
        
        // Применение сопротивления воздуха
        particle.Velocity *= 0.98f;
        
        // Обновление трансформации
        transform.Position = particle.Position;
        transform.Scale = particle.Size * (1f - particle.Lifetime / particle.MaxLifetime);
        
        // Обновление альфы в зависимости от времени жизни
        float alpha = 1f - (particle.Lifetime / particle.MaxLifetime);
        particle.Color.w = alpha;
    }
}
```

## 🌐 **API МУЛЬТИПЛЕЕРА**

### **Создание сетевого грузовика**
```csharp
// Создание сетевого грузовика
var truckEntity = EntityManager.CreateEntity();

// Добавление основных компонентов
EntityManager.AddComponent<PlayerTag>(truckEntity);
EntityManager.AddComponent<TruckData>(truckEntity);
EntityManager.AddComponent<TruckControl>(truckEntity);

// Добавление сетевых компонентов
EntityManager.AddComponent<NetworkedTruckData>(truckEntity);

// Добавление компонентов Netcode
EntityManager.AddComponent<PredictedGhost>(truckEntity);
EntityManager.AddComponent<InterpolatedGhost>(truckEntity);
EntityManager.AddComponent<GhostOwnerComponent>(truckEntity);

// Настройка сетевых данных
var networkedData = new NetworkedTruckData
{
    Position = float3.zero,
    Rotation = quaternion.identity,
    Velocity = float3.zero,
    AngularVelocity = float3.zero,
    CurrentGear = 1,
    EngineRPM = 800f,
    CurrentSpeed = 0f,
    SteeringAngle = 0f,
    EngineRunning = false,
    HandbrakeOn = false,
    LockFrontDifferential = false,
    LockMiddleDifferential = false,
    LockRearDifferential = false,
    LockCenterDifferential = false,
    FuelLevel = 1f
};

EntityManager.SetComponentData(truckEntity, networkedData);
```

### **Синхронизация данных**
```csharp
// Синхронизация данных грузовика
private void SyncTruckData()
{
    Entities
        .WithAll<TruckData, NetworkedTruckData>()
        .ForEach((ref NetworkedTruckData networkedData, in TruckData truckData, in LocalTransform transform) =>
        {
            // Обновление сетевых данных из локальных данных
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
```

### **Лаг-компенсация**
```csharp
// Применение лаг-компенсации
private void ApplyLagCompensation()
{
    // Получение среднего ping всех игроков
    float averagePing = CalculateAveragePing();
    float rewindTime = math.min(averagePing / 1000f, MAX_REWIND_TIME);
    
    // Применение компенсации для всех грузовиков
    Entities
        .WithAll<TruckData, NetworkedTruckData>()
        .ForEach((ref LocalTransform transform, in NetworkedTruckData networkedData) =>
        {
            // Вычисление компенсированной позиции
            float3 compensatedPosition = CalculateCompensatedPosition(
                networkedData.Position, 
                networkedData.Velocity, 
                rewindTime
            );
            
            // Применение компенсации
            transform.Position = compensatedPosition;
        }).WithoutBurst().Run();
}

// Вычисление компенсированной позиции
private static float3 CalculateCompensatedPosition(float3 position, float3 velocity, float rewindTime)
{
    return position - velocity * rewindTime;
}
```

## 🧪 **API ТЕСТИРОВАНИЯ**

### **Unit тест системы движения**
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

### **Integration тест мультиплеера**
```csharp
[Test]
public void NetworkedTruckData_SyncsCorrectly()
{
    // Arrange
    var serverEntity = _serverEntityManager.CreateEntity();
    _serverEntityManager.AddComponent<PlayerTag>(serverEntity);
    _serverEntityManager.AddComponent<TruckData>(serverEntity);
    _serverEntityManager.AddComponent<NetworkedTruckData>(serverEntity);
    
    var clientEntity = _clientEntityManager.CreateEntity();
    _clientEntityManager.AddComponent<PlayerTag>(clientEntity);
    _clientEntityManager.AddComponent<TruckData>(clientEntity);
    _clientEntityManager.AddComponent<NetworkedTruckData>(clientEntity);
    
    // Установка данных на сервере
    var serverTruckData = new TruckData
    {
        CurrentGear = 3,
        EngineRPM = 2000f,
        CurrentSpeed = 50f,
        EngineRunning = true
    };
    _serverEntityManager.SetComponentData(serverEntity, serverTruckData);
    
    // Act - симуляция синхронизации
    var serverNetworkedData = _serverEntityManager.GetComponentData<NetworkedTruckData>(serverEntity);
    _clientEntityManager.SetComponentData(clientEntity, serverNetworkedData);
    
    // Assert
    var clientNetworkedData = _clientEntityManager.GetComponentData<NetworkedTruckData>(clientEntity);
    Assert.AreEqual(serverNetworkedData.CurrentGear, clientNetworkedData.CurrentGear);
    Assert.AreEqual(serverNetworkedData.EngineRPM, clientNetworkedData.EngineRPM);
    Assert.AreEqual(serverNetworkedData.CurrentSpeed, clientNetworkedData.CurrentSpeed);
    Assert.AreEqual(serverNetworkedData.EngineRunning, clientNetworkedData.EngineRunning);
}
```

---

**Эти примеры показывают, как использовать API Mud-Like для создания реалистичной игры про внедорожники! 🚛🏔️💨🌐**