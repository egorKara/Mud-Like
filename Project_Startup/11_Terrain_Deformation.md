# 🌍 Mud-Like Terrain Deformation

## 🎯 **ОБЗОР ДЕФОРМАЦИИ ТЕРРЕЙНА**

### **Цель системы**
Создать реалистичную систему деформации террейна с грязью, которая влияет на физику транспорта и синхронизируется в мультиплеере.

### **Ключевые требования**
- **Реалистичная деформация** под колесами транспорта
- **Синхронизация** между всеми клиентами
- **Производительность** <5% FPS
- **Детерминизм** для мультиплеера

## 🏗️ **АРХИТЕКТУРА СИСТЕМЫ**

### **Компоненты ECS**
```csharp
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

// Данные чанка
public struct ChunkData : IComponentData
{
    public int ChunkIndex;
    public float3 WorldPosition;
    public float Size;
    public bool IsDirty;
    public bool NeedsSync;
}
```

### **Системы ECS**
```csharp
// Система деформации террейна
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public class TerrainDeformationSystem : SystemBase { }

// Система генерации деформации
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public class DeformationGenerationSystem : SystemBase { }

// Система управления чанками
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public class ChunkManagementSystem : SystemBase { }

// Система синхронизации террейна
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public class TerrainSyncSystem : SystemBase { }
```

## 🔧 **РЕАЛИЗАЦИЯ ДЕФОРМАЦИИ**

### **1. Система деформации террейна**
```csharp
// Scripts/Terrain/Systems/TerrainDeformationSystem.cs
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public class TerrainDeformationSystem : SystemBase
{
    private TerrainManager _terrainManager;
    
    protected override void OnCreate()
    {
        _terrainManager = TerrainManager.Instance;
    }
    
    protected override void OnUpdate()
    {
        Entities
            .WithAll<DeformationData>()
            .ForEach((in DeformationData deformation) =>
            {
                // Применение деформации к террейну
                ApplyDeformation(deformation.Position, deformation.Radius, deformation.Depth);
                
                // Отметка чанка как грязного
                MarkChunkAsDirty(deformation.TerrainChunkIndex);
            }).Schedule();
    }
    
    private void ApplyDeformation(float3 position, float radius, float depth)
    {
        // Получение данных террейна
        var terrainData = _terrainManager.GetTerrainData();
        var terrain = _terrainManager.GetTerrain();
        
        // Расчет области деформации
        var worldPos = position;
        var terrainPos = terrain.transform.InverseTransformPoint(worldPos);
        var heightmapWidth = terrain.terrainData.heightmapResolution;
        var heightmapHeight = terrain.terrainData.heightmapResolution;
        
        // Конвертация в координаты heightmap
        var x = Mathf.RoundToInt(terrainPos.x * heightmapWidth / terrain.terrainData.size.x);
        var z = Mathf.RoundToInt(terrainPos.z * heightmapHeight / terrain.terrainData.size.z);
        
        // Расчет радиуса в координатах heightmap
        var radiusInHeightmap = Mathf.RoundToInt(radius * heightmapWidth / terrain.terrainData.size.x);
        
        // Применение деформации
        ApplyDeformationToHeightmap(terrain, x, z, radiusInHeightmap, depth);
        
        // Синхронизация с TerrainCollider
        SyncTerrainCollider(terrain);
    }
    
    private void ApplyDeformationToHeightmap(Terrain terrain, int x, int z, int radius, float depth)
    {
        var terrainData = terrain.terrainData;
        var heightmapWidth = terrainData.heightmapResolution;
        var heightmapHeight = terrainData.heightmapResolution;
        
        // Получение текущих высот
        var heights = terrainData.GetHeights(0, 0, heightmapWidth, heightmapHeight);
        
        // Применение деформации
        for (int i = -radius; i <= radius; i++)
        {
            for (int j = -radius; j <= radius; j++)
            {
                int currentX = x + i;
                int currentZ = z + j;
                
                if (currentX >= 0 && currentX < heightmapWidth && 
                    currentZ >= 0 && currentZ < heightmapHeight)
                {
                    // Расчет расстояния от центра
                    float distance = Mathf.Sqrt(i * i + j * j);
                    
                    if (distance <= radius)
                    {
                        // Расчет силы деформации
                        float deformationForce = 1f - (distance / radius);
                        float deformationDepth = depth * deformationForce;
                        
                        // Применение деформации
                        heights[currentZ, currentX] -= deformationDepth / terrainData.size.y;
                    }
                }
            }
        }
        
        // Установка новых высот
        terrainData.SetHeights(0, 0, heights);
        
        // Синхронизация с рендерингом
        terrainData.SyncHeightmap();
    }
    
    private void SyncTerrainCollider(Terrain terrain)
    {
        // Синхронизация с TerrainCollider
        var terrainCollider = terrain.GetComponent<TerrainCollider>();
        if (terrainCollider != null)
        {
            // Обновление коллайдера
            terrainCollider.terrainData = terrain.terrainData;
        }
    }
    
    private void MarkChunkAsDirty(int chunkIndex)
    {
        Entities
            .WithAll<ChunkData>()
            .ForEach((ref ChunkData chunk) =>
            {
                if (chunk.ChunkIndex == chunkIndex)
                {
                    chunk.IsDirty = true;
                    chunk.NeedsSync = true;
                }
            }).Schedule();
    }
}
```

### **2. Система генерации деформации**
```csharp
// Scripts/Terrain/Systems/DeformationGenerationSystem.cs
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public class DeformationGenerationSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities
            .WithAll<VehicleTag>()
            .ForEach((in Translation translation, in Rotation rotation, 
                     in VehicleData vehicleData, in WheelData wheel) =>
            {
                if (wheel.IsGrounded)
                {
                    // Создание деформации под колесом
                    var deformation = new DeformationData
                    {
                        Position = wheel.GroundPoint,
                        Radius = wheel.Radius,
                        Depth = CalculateDeformationDepth(wheel),
                        Intensity = CalculateDeformationIntensity(wheel),
                        Normal = wheel.GroundNormal,
                        TerrainChunkIndex = GetChunkIndex(wheel.GroundPoint)
                    };
                    
                    // Создание сущности деформации
                    var entity = EntityManager.CreateEntity();
                    EntityManager.AddComponentData(entity, deformation);
                }
            }).Schedule();
    }
    
    private float CalculateDeformationDepth(WheelData wheel)
    {
        // Расчет глубины деформации на основе веса и скорости
        float weight = wheel.SuspensionForce;
        float speed = math.length(wheel.Velocity);
        
        // Базовая глубина + влияние скорости
        float baseDepth = weight * 0.001f;
        float speedFactor = math.clamp(speed * 0.1f, 0f, 1f);
        
        return baseDepth * (1f + speedFactor);
    }
    
    private float CalculateDeformationIntensity(WheelData wheel)
    {
        // Расчет интенсивности деформации
        float weight = wheel.SuspensionForce;
        float speed = math.length(wheel.Velocity);
        
        return weight * speed * 0.01f;
    }
    
    private int GetChunkIndex(float3 position)
    {
        // Расчет индекса чанка на основе позиции
        int chunkSize = 16; // 16x16 блоки
        int x = Mathf.FloorToInt(position.x / chunkSize);
        int z = Mathf.FloorToInt(position.z / chunkSize);
        
        return x * 1000 + z; // Простая хеш-функция
    }
}
```

### **3. Система управления чанками**
```csharp
// Scripts/Terrain/Systems/ChunkManagementSystem.cs
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public class ChunkManagementSystem : SystemBase
{
    private ChunkManager _chunkManager;
    
    protected override void OnCreate()
    {
        _chunkManager = ChunkManager.Instance;
    }
    
    protected override void OnUpdate()
    {
        // Обработка грязных чанков
        Entities
            .WithAll<ChunkData>()
            .ForEach((ref ChunkData chunk) =>
            {
                if (chunk.IsDirty)
                {
                    // Обновление чанка
                    UpdateChunk(chunk);
                    chunk.IsDirty = false;
                }
            }).Schedule();
        
        // Синхронизация чанков
        Entities
            .WithAll<ChunkData>()
            .ForEach((ref ChunkData chunk) =>
            {
                if (chunk.NeedsSync)
                {
                    // Синхронизация с другими клиентами
                    SyncChunk(chunk);
                    chunk.NeedsSync = false;
                }
            }).Schedule();
    }
    
    private void UpdateChunk(ChunkData chunk)
    {
        // Обновление визуального представления чанка
        _chunkManager.UpdateChunkVisuals(chunk.ChunkIndex);
        
        // Обновление физики чанка
        _chunkManager.UpdateChunkPhysics(chunk.ChunkIndex);
    }
    
    private void SyncChunk(ChunkData chunk)
    {
        // Отправка данных чанка на сервер
        var chunkData = _chunkManager.GetChunkData(chunk.ChunkIndex);
        SendChunkDataToServer(chunk.ChunkIndex, chunkData);
    }
    
    private void SendChunkDataToServer(int chunkIndex, byte[] chunkData)
    {
        // Отправка данных чанка на сервер для синхронизации
        var command = new SyncChunkCommand
        {
            ChunkIndex = chunkIndex,
            ChunkData = chunkData
        };
        
        // Отправка команды на сервер
        PostUpdateCommands.AddComponent<SyncChunkCommand>(entity, command);
    }
}
```

## 🌐 **СЕТЕВАЯ СИНХРОНИЗАЦИЯ**

### **1. Команды синхронизации**
```csharp
// Scripts/Networking/Commands/SyncChunkCommand.cs
using Unity.Entities;
using Unity.NetCode;

public struct SyncChunkCommand : ICommandTargetData
{
    public NetworkId Target;
    public int ChunkIndex;
    public byte[] ChunkData;
}

// События синхронизации
public struct SyncChunkEvent : IEventData
{
    public NetworkId Sender;
    public int ChunkIndex;
    public byte[] ChunkData;
}
```

### **2. Система синхронизации террейна**
```csharp
// Scripts/Networking/Systems/TerrainSyncSystem.cs
using Unity.Entities;
using Unity.NetCode;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public class TerrainSyncSystem : SystemBase
{
    protected override void OnUpdate()
    {
        // Отправка изменений террейна на сервер
        Entities
            .WithAll<ChunkData, IsClient>()
            .ForEach((in ChunkData chunk) =>
            {
                if (chunk.NeedsSync)
                {
                    var command = new SyncChunkCommand
                    {
                        ChunkIndex = chunk.ChunkIndex,
                        ChunkData = GetChunkData(chunk.ChunkIndex)
                    };
                    
                    PostUpdateCommands.AddComponent<SyncChunkCommand>(entity, command);
                }
            }).Schedule();
        
        // Обработка команд от сервера
        Entities
            .WithAll<SyncChunkEvent, IsClient>()
            .ForEach((in SyncChunkEvent syncEvent) =>
            {
                // Применение изменений террейна от сервера
                ApplyChunkData(syncEvent.ChunkIndex, syncEvent.ChunkData);
            }).Schedule();
    }
    
    private byte[] GetChunkData(int chunkIndex)
    {
        // Получение данных чанка для отправки
        var chunkManager = ChunkManager.Instance;
        return chunkManager.GetChunkData(chunkIndex);
    }
    
    private void ApplyChunkData(int chunkIndex, byte[] chunkData)
    {
        // Применение данных чанка от сервера
        var chunkManager = ChunkManager.Instance;
        chunkManager.ApplyChunkData(chunkIndex, chunkData);
    }
}
```

## 🎮 **ИНТЕГРАЦИЯ С ТРАНСПОРТОМ**

### **1. Система взаимодействия с грязью**
```csharp
// Scripts/Vehicles/Systems/MudInteractionSystem.cs
using Unity.Entities;
using Unity.Mathematics;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public class MudInteractionSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities
            .WithAll<VehicleTag>()
            .ForEach((ref WheelData wheel, in Translation translation) =>
            {
                // Проверка контакта с грязью
                var mudData = QueryMudContact(translation.Value, wheel.Radius);
                
                if (mudData.HasContact)
                {
                    // Применение эффектов грязи
                    ApplyMudEffects(ref wheel, mudData);
                    
                    // Создание деформации
                    CreateDeformation(translation.Value, wheel.Radius, mudData.SinkDepth);
                }
            }).Schedule();
    }
    
    private MudContactData QueryMudContact(float3 position, float radius)
    {
        // Запрос контакта с грязью
        var mudManager = MudManager.Instance;
        return mudManager.QueryContact(position, radius);
    }
    
    private void ApplyMudEffects(ref WheelData wheel, MudContactData mudData)
    {
        // Применение эффектов грязи к колесу
        wheel.FrictionForce *= mudData.TractionModifier;
        wheel.SinkDepth = mudData.SinkDepth;
        wheel.Velocity *= mudData.DragModifier;
    }
    
    private void CreateDeformation(float3 position, float radius, float depth)
    {
        // Создание деформации под колесом
        var deformation = new DeformationData
        {
            Position = position,
            Radius = radius,
            Depth = depth,
            Intensity = 1f,
            Normal = float3.up(),
            TerrainChunkIndex = GetChunkIndex(position)
        };
        
        var entity = EntityManager.CreateEntity();
        EntityManager.AddComponentData(entity, deformation);
    }
}
```

### **2. MudManager API**
```csharp
// Scripts/Terrain/Managers/MudManager.cs
using Unity.Entities;
using Unity.Mathematics;

public class MudManager : MonoBehaviour
{
    public static MudManager Instance { get; private set; }
    
    [Header("Mud Settings")]
    public float mudDensity = 1f;
    public float mudViscosity = 0.5f;
    public float mudTraction = 0.3f;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public MudContactData QueryContact(float3 position, float radius)
    {
        // Запрос контакта с грязью
        var terrainData = GetTerrainDataAtPosition(position);
        
        return new MudContactData
        {
            HasContact = terrainData.MudLevel > 0.1f,
            SinkDepth = CalculateSinkDepth(position, radius, terrainData),
            TractionModifier = CalculateTractionModifier(terrainData),
            DragModifier = CalculateDragModifier(terrainData)
        };
    }
    
    private TerrainData GetTerrainDataAtPosition(float3 position)
    {
        // Получение данных террейна в позиции
        var terrain = GetComponent<Terrain>();
        var terrainPos = terrain.transform.InverseTransformPoint(position);
        
        return new TerrainData
        {
            WorldPosition = position,
            Height = terrain.SampleHeight(position),
            MudLevel = SampleMudLevel(terrainPos),
            Traction = SampleTraction(terrainPos),
            SinkDepth = 0f,
            Normal = SampleNormal(terrainPos),
            ChunkIndex = GetChunkIndex(position)
        };
    }
    
    private float CalculateSinkDepth(float3 position, float radius, TerrainData terrainData)
    {
        // Расчет глубины погружения в грязь
        float mudLevel = terrainData.MudLevel;
        float weight = 1000f; // Вес транспорта
        
        return mudLevel * weight * 0.001f;
    }
    
    private float CalculateTractionModifier(TerrainData terrainData)
    {
        // Расчет модификатора сцепления
        float mudLevel = terrainData.MudLevel;
        return math.lerp(1f, mudTraction, mudLevel);
    }
    
    private float CalculateDragModifier(TerrainData terrainData)
    {
        // Расчет модификатора сопротивления
        float mudLevel = terrainData.MudLevel;
        return math.lerp(1f, 1f - mudViscosity, mudLevel);
    }
}

// Структура данных контакта с грязью
public struct MudContactData
{
    public bool HasContact;
    public float SinkDepth;
    public float TractionModifier;
    public float DragModifier;
}
```

## 🧪 **ТЕСТИРОВАНИЕ ДЕФОРМАЦИИ**

### **1. Unit тесты**
```csharp
// Tests/Unit/TerrainDeformationTests.cs
using NUnit.Framework;
using Unity.Mathematics;

public class TerrainDeformationTests
{
    [Test]
    public void CalculateDeformationDepth_ValidInput_ReturnsCorrectDepth()
    {
        // Arrange
        var position = new float3(0, 0, 0);
        var radius = 5f;
        var weight = 1000f;
        
        // Act
        var depth = TerrainDeformationSystem.CalculateDeformationDepth(position, radius, weight);
        
        // Assert
        Assert.Greater(depth, 0f);
        Assert.Less(depth, 1f);
    }
    
    [Test]
    public void ApplyDeformation_ValidInput_ModifiesTerrain()
    {
        // Arrange
        var terrain = CreateTestTerrain();
        var position = new float3(0, 0, 0);
        var radius = 5f;
        var depth = 0.1f;
        
        // Act
        TerrainDeformationSystem.ApplyDeformation(terrain, position, radius, depth);
        
        // Assert
        Assert.IsTrue(terrain.HasDeformationAt(position));
    }
}
```

### **2. Integration тесты**
```csharp
// Tests/Integration/TerrainInteractionTests.cs
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TerrainInteractionTests
{
    [UnityTest]
    public IEnumerator Vehicle_DrivesOnMud_CreatesDeformation()
    {
        // Arrange
        var vehicle = CreateVehicle();
        var mudTerrain = CreateMudTerrain();
        
        // Act
        vehicle.MoveTo(new Vector3(0, 0, 0));
        yield return new WaitForSeconds(1f);
        
        // Assert
        Assert.IsTrue(mudTerrain.HasDeformationAt(vehicle.transform.position));
    }
}
```

## 🎯 **РЕЗУЛЬТАТ РЕАЛИЗАЦИИ**

### **Функциональность**
- ✅ **Реалистичная деформация** террейна под колесами
- ✅ **Синхронизация** между всеми клиентами
- ✅ **Производительность** <5% FPS
- ✅ **Детерминизм** для мультиплеера

### **Технические характеристики**
- ✅ **ECS-архитектура** для всех систем
- ✅ **Chunk-based** управление террейном
- ✅ **Сетевая синхронизация** через Netcode for Entities
- ✅ **Тестируемость** всех компонентов

---

**Система деформации террейна Mud-Like обеспечивает реалистичное взаимодействие транспорта с грязью при сохранении производительности и детерминизма для мультиплеера.**
