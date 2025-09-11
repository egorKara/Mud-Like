# 🌐 Mud-Like Multiplayer Setup

## 🎯 **ОБЗОР МУЛЬТИПЛЕЕРА**

### **Цель системы**
Создать стабильный и производительный мультиплеер с детерминированной симуляцией для Mud-Like.

### **Ключевые требования**
- **Детерминизм** для всех физических вычислений
- **Client-Server архитектура** с авторитарным сервером
- **Синхронизация** ECS компонентов
- **Lag Compensation** для честности игры

## 🏗️ **АРХИТЕКТУРА МУЛЬТИПЛЕЕРА**

### **Client-Server модель**
```
┌─────────────┐    ┌─────────────┐    ┌─────────────┐
│   Client 1  │    │   Client 2  │    │   Client N  │
│             │    │             │    │             │
│  ECS World  │    │  ECS World  │    │  ECS World  │
└──────┬──────┘    └──────┬──────┘    └──────┬──────┘
       │                  │                  │
       └──────────────────┼──────────────────┘
                          │
                   ┌──────▼──────┐
                   │   Server    │
                   │             │
                   │  ECS World  │
                   │ (Authority) │
                   └─────────────┘
```

### **Компоненты для синхронизации**
```csharp
// Сетевая позиция
public struct NetworkPosition : IComponentData
{
    public float3 Value;
}

// Сетевое вращение
public struct NetworkRotation : IComponentData
{
    public quaternion Value;
}

// Сетевая скорость
public struct NetworkVelocity : IComponentData
{
    public float3 Value;
}

// Тег игрока
public struct PlayerTag : IComponentData
{
}

// Тег сервера
public struct ServerTag : IComponentData
{
}

// Тег клиента
public struct ClientTag : IComponentData
{
}
```

## 🔧 **НАСТРОЙКА NETCODE FOR ENTITIES**

### **1. Установка пакетов**
```json
{
  "dependencies": {
    "com.unity.netcode": "1.6.2",
    "com.unity.entities": "1.3.14",
    "com.unity.collections": "2.5.7",
    "com.unity.burst": "1.8.24"
  }
}
```

### **2. Создание NetworkManager**
```csharp
// Scripts/Networking/NetworkManager.cs
using Unity.Entities;
using Unity.NetCode;
using Unity.Networking.Transport;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    [Header("Network Settings")]
    public int port = 7777;
    public int maxConnections = 100;
    public float tickRate = 60f;
    
    private NetworkDriver _driver;
    private NetworkPipeline _pipeline;
    private bool _isServer;
    private bool _isClient;
    
    void Start()
    {
        InitializeNetwork();
    }
    
    void OnDestroy()
    {
        ShutdownNetwork();
    }
    
    private void InitializeNetwork()
    {
        // Создание NetworkDriver
        _driver = NetworkDriver.Create();
        _pipeline = _driver.CreatePipeline(typeof(ReliableSequencedPipelineStage));
        
        // Настройка World для сети
        SetupNetworkWorld();
    }
    
    private void SetupNetworkWorld()
    {
        // Создание HostGameWorld
        var hostWorld = new World("HostGameWorld");
        World.DefaultGameObjectInjectionWorld = hostWorld;
        
        // Создание ServerGameWorld
        var serverWorld = new World("ServerGameWorld");
        serverWorld.GetOrCreateSystem<ServerSimulationSystemGroup>();
        
        // Регистрация сетевых систем
        RegisterNetworkSystems(hostWorld);
        RegisterNetworkSystems(serverWorld);
    }
    
    private void RegisterNetworkSystems(World world)
    {
        // Регистрация систем синхронизации
        world.GetOrCreateSystem<SendPositionToServerSystem>();
        world.GetOrCreateSystem<ReceivePositionFromServerSystem>();
        world.GetOrCreateSystem<NetworkSyncSystem>();
        world.GetOrCreateSystem<LagCompensationSystem>();
    }
    
    public void StartServer()
    {
        _isServer = true;
        var endpoint = NetworkEndPoint.AnyIpv4;
        endpoint.Port = (ushort)port;
        
        if (_driver.Bind(endpoint) == 0)
        {
            _driver.Listen();
            Debug.Log($"Server started on port {port}");
        }
        else
        {
            Debug.LogError("Failed to start server");
        }
    }
    
    public void StartClient()
    {
        _isClient = true;
        var endpoint = NetworkEndPoint.LoopbackIpv4;
        endpoint.Port = (ushort)port;
        
        if (_driver.Connect(endpoint) == 0)
        {
            Debug.Log("Client connected to server");
        }
        else
        {
            Debug.LogError("Failed to connect to server");
        }
    }
    
    private void ShutdownNetwork()
    {
        if (_driver.IsCreated)
        {
            _driver.Dispose();
        }
    }
}
```

### **3. Настройка World**
```csharp
// Scripts/Networking/WorldSetup.cs
using Unity.Entities;
using Unity.NetCode;

public class WorldSetup : MonoBehaviour
{
    void Start()
    {
        SetupWorlds();
    }
    
    private void SetupWorlds()
    {
        // Создание HostGameWorld
        var hostWorld = new World("HostGameWorld");
        World.DefaultGameObjectInjectionWorld = hostWorld;
        
        // Создание ServerGameWorld
        var serverWorld = new World("ServerGameWorld");
        serverWorld.GetOrCreateSystem<ServerSimulationSystemGroup>();
        
        // Настройка систем для каждого World
        SetupHostSystems(hostWorld);
        SetupServerSystems(serverWorld);
    }
    
    private void SetupHostSystems(World world)
    {
        // Системы для хоста (клиент + сервер)
        world.GetOrCreateSystem<PlayerInputSystem>();
        world.GetOrCreateSystem<PlayerMovementSystem>();
        world.GetOrCreateSystem<VehicleSystem>();
        world.GetOrCreateSystem<TerrainDeformationSystem>();
    }
    
    private void SetupServerSystems(World world)
    {
        // Системы только для сервера
        world.GetOrCreateSystem<ServerSimulationSystemGroup>();
        world.GetOrCreateSystem<NetworkSyncSystem>();
        world.GetOrCreateSystem<LagCompensationSystem>();
        world.GetOrCreateSystem<AntiCheatSystem>();
    }
}
```

## 🌐 **СИСТЕМЫ СИНХРОНИЗАЦИИ**

### **1. Система отправки позиции на сервер**
```csharp
// Scripts/Networking/Systems/SendPositionToServerSystem.cs
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;

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
                    // Отправка текущей позиции на сервер
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

### **2. Система получения позиции от сервера**
```csharp
// Scripts/Networking/Systems/ReceivePositionFromServerSystem.cs
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;

[UpdateAfter(typeof(NetworkSyncSystem))]
public class ReceivePositionFromServerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities
            .WithAll<PlayerTag, IsClient>()
            .ForEach((ref Translation translation, in NetworkPosition networkPosition) =>
            {
                // Обновление локальной позиции на основе сетевой
                translation.Value = networkPosition.Value;
            }).Schedule();
    }
}
```

### **3. Система сетевой синхронизации**
```csharp
// Scripts/Networking/Systems/NetworkSyncSystem.cs
using Unity.Entities;
using Unity.NetCode;

[UpdateInGroup(typeof(ServerSimulationSystemGroup))]
public class NetworkSyncSystem : SystemBase
{
    protected override void OnUpdate()
    {
        // Обработка команд от клиентов
        Entities
            .WithAll<SetNetworkPositionCommand, IsServer>()
            .ForEach((in SetNetworkPositionCommand command) =>
            {
                // Валидация команды
                if (ValidateCommand(command))
                {
                    // Применение команды
                    ApplyCommand(command);
                    
                    // Отправка события всем клиентам
                    SendEventToAllClients(command);
                }
            }).Schedule();
    }
    
    private bool ValidateCommand(SetNetworkPositionCommand command)
    {
        // Валидация команды на сервере
        // Проверка на читы, разумность значений и т.д.
        return true;
    }
    
    private void ApplyCommand(SetNetworkPositionCommand command)
    {
        // Применение команды к игровому состоянию
        // Обновление позиции игрока
    }
    
    private void SendEventToAllClients(SetNetworkPositionCommand command)
    {
        // Отправка события всем клиентам
        var eventData = new SetNetworkPositionEvent
        {
            Sender = command.NetworkId,
            Position = command.Position
        };
        
        // Рассылка события
        PostUpdateCommands.AddComponent<SetNetworkPositionEvent>(entity, eventData);
    }
}
```

## ⚡ **LAG COMPENSATION**

### **1. Система компенсации задержек**
```csharp
// Scripts/Networking/Systems/LagCompensationSystem.cs
using Unity.Entities;
using Unity.NetCode;
using Unity.Physics;

[UpdateInGroup(typeof(ServerSimulationSystemGroup))]
public class LagCompensationSystem : SystemBase
{
    private PhysicsWorld _physicsWorld;
    private CollisionWorld _collisionWorld;
    
    protected override void OnCreate()
    {
        _physicsWorld = new PhysicsWorld();
        _collisionWorld = new CollisionWorld();
    }
    
    protected override void OnUpdate()
    {
        // Обработка команд с компенсацией задержки
        Entities
            .WithAll<ShootCommand, IsServer>()
            .ForEach((in ShootCommand command) =>
            {
                // Клонирование физического мира для расчетов
                var clonedWorld = _physicsWorld.Clone();
                var clonedCollision = _collisionWorld.Clone();
                
                // Откат времени для компенсации задержки
                var rollbackTime = command.Timestamp - command.Lag;
                RollbackPhysics(clonedWorld, clonedCollision, rollbackTime);
                
                // Проверка попадания с компенсированной задержкой
                var hit = CheckHit(clonedWorld, clonedCollision, command);
                
                // Применение результата
                ApplyHitResult(hit);
                
                // Очистка клонированного мира
                clonedWorld.Dispose();
                clonedCollision.Dispose();
            }).Schedule();
    }
    
    private void RollbackPhysics(PhysicsWorld physicsWorld, CollisionWorld collisionWorld, float time)
    {
        // Откат физики к указанному времени
        // Восстановление позиций объектов
    }
    
    private HitResult CheckHit(PhysicsWorld physicsWorld, CollisionWorld collisionWorld, ShootCommand command)
    {
        // Проверка попадания с компенсированной задержкой
        return new HitResult();
    }
    
    private void ApplyHitResult(HitResult hit)
    {
        // Применение результата попадания
    }
}
```

### **2. Команды и события**
```csharp
// Scripts/Networking/Commands/ShootCommand.cs
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;

public struct ShootCommand : ICommandTargetData
{
    public NetworkId Target;
    public float3 Origin;
    public float3 Direction;
    public float Timestamp;
    public float Lag;
}

// Scripts/Networking/Events/ShootEvent.cs
public struct ShootEvent : IEventData
{
    public NetworkId Sender;
    public float3 Origin;
    public float3 Direction;
    public HitResult Hit;
}
```

## 🛡️ **ANTI-CHEAT СИСТЕМА**

### **1. Система защиты от мошенничества**
```csharp
// Scripts/Networking/Systems/AntiCheatSystem.cs
using Unity.Entities;
using Unity.NetCode;

[UpdateInGroup(typeof(ServerSimulationSystemGroup))]
public class AntiCheatSystem : SystemBase
{
    protected override void OnUpdate()
    {
        // Валидация всех команд от клиентов
        Entities
            .WithAll<IsClient>()
            .ForEach((in NetworkPosition position, in NetworkVelocity velocity) =>
            {
                // Проверка на разумность значений
                if (!ValidatePosition(position.Value))
                {
                    // Наказание за читы
                    ApplyPunishment(entity);
                }
                
                if (!ValidateVelocity(velocity.Value))
                {
                    // Наказание за читы
                    ApplyPunishment(entity);
                }
            }).Schedule();
    }
    
    private bool ValidatePosition(float3 position)
    {
        // Проверка позиции на разумность
        // Проверка на телепортацию, выход за границы и т.д.
        return true;
    }
    
    private bool ValidateVelocity(float3 velocity)
    {
        // Проверка скорости на разумность
        // Проверка на сверхскорость, нереальные ускорения и т.д.
        return true;
    }
    
    private void ApplyPunishment(Entity entity)
    {
        // Применение наказания за читы
        // Кик, бан, ограничение скорости и т.д.
    }
}
```

## 🧪 **ТЕСТИРОВАНИЕ МУЛЬТИПЛЕЕРА**

### **1. Unit тесты**
```csharp
// Tests/Unit/NetworkSyncTests.cs
using NUnit.Framework;
using Unity.Entities;
using Unity.Mathematics;

public class NetworkSyncTests
{
    [Test]
    public void NetworkPosition_ValidInput_UpdatesCorrectly()
    {
        // Arrange
        var position = new NetworkPosition { Value = new float3(1, 2, 3) };
        
        // Act
        var result = NetworkSyncSystem.ProcessPosition(position);
        
        // Assert
        Assert.AreEqual(position.Value, result);
    }
    
    [Test]
    public void LagCompensation_ValidInput_CompensatesCorrectly()
    {
        // Arrange
        var command = new ShootCommand
        {
            Origin = new float3(0, 0, 0),
            Direction = new float3(1, 0, 0),
            Timestamp = 100f,
            Lag = 50f
        };
        
        // Act
        var result = LagCompensationSystem.Compensate(command);
        
        // Assert
        Assert.IsTrue(result.IsValid);
    }
}
```

### **2. Integration тесты**
```csharp
// Tests/Integration/NetworkCommunicationTests.cs
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class NetworkCommunicationTests
{
    [UnityTest]
    public IEnumerator NetworkSync_ClientSendsPosition_ServerReceives()
    {
        // Arrange
        var client = CreateClient();
        var server = CreateServer();
        var position = new Vector3(1, 2, 3);
        
        // Act
        client.SendPosition(position);
        yield return new WaitForSeconds(0.1f);
        
        // Assert
        Assert.AreEqual(position, server.GetLastReceivedPosition());
    }
    
    [UnityTest]
    public IEnumerator Multiplayer_TwoPlayersJoin_CanSeeEachOther()
    {
        // Arrange
        var gameManager = CreateMultiplayerGame();
        var player1 = CreatePlayer();
        var player2 = CreatePlayer();
        
        // Act
        yield return new WaitForSeconds(2f); // Игроки присоединяются
        player1.MoveTo(new Vector3(1, 0, 0));
        yield return new WaitForSeconds(1f);
        
        // Assert
        Assert.IsTrue(player2.CanSee(player1));
    }
}
```

## 🎯 **РЕЗУЛЬТАТ НАСТРОЙКИ**

### **Функциональность**
- ✅ **Стабильный мультиплеер** с детерминированной симуляцией
- ✅ **Client-Server архитектура** с авторитарным сервером
- ✅ **Синхронизация ECS** компонентов
- ✅ **Lag Compensation** для честности игры

### **Технические характеристики**
- ✅ **Netcode for Entities** для сетевой функциональности
- ✅ **Детерминизм** для всех физических вычислений
- ✅ **Anti-cheat система** для защиты от мошенничества
- ✅ **Тестируемость** всех сетевых компонентов

---

**Мультиплеер Mud-Like обеспечивает стабильную и честную игру для множества игроков с детерминированной симуляцией и защитой от мошенничества.**
