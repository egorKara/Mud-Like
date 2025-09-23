### **Assembly Definition Files:**
```
MudLike.Core (базовый модуль)
├── Unity.Entities
├── Unity.Collections
├── Unity.Jobs
├── Unity.Burst
├── Unity.Mathematics
└── Unity.Transforms

MudLike.Vehicles (транспорт)
├── MudLike.Core
├── Unity.Physics
└── Unity.Rendering.Hybrid

MudLike.Terrain (террейн)
├── MudLike.Core
└── Unity.Physics

MudLike.Networking (сеть)
├── MudLike.Core
└── Unity.NetCode
```
## 🎮 **КРИТИЧЕСКИЕ СИСТЕМЫ И ФИЧИ**

### **MudManager API - Основа деформации террейна:**
- **Критически важный метод:** `QueryContact(wheelPosition, radius) → sinkDepth, tractionModifier`
- **Основа взаимодействия** колес с грязью
- **Реалистичная физика** транспорта

```csharp
// MudManager API - QueryContact метод
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
```

### **Система лебедки:**
- **Полное завершение** с реализацией `ApplyForceToObject()`
- **Интеграция с Unity Physics**
- **Применение сил** к физическим телам

```csharp
// WinchSystem - ApplyForceToObject метод
private void ApplyForceToObject(Entity targetEntity, float3 force, float3 point)
{
    // Получение RigidBody компонента
    if (EntityManager.HasComponent<PhysicsVelocity>(targetEntity))
    {
        var physicsVelocity = EntityManager.GetComponentData<PhysicsVelocity>(targetEntity);
        var physicsMass = EntityManager.GetComponentData<PhysicsMass>(targetEntity);
        
        // Применение силы к физическому телу
        var impulse = force * SystemAPI.Time.fixedDeltaTime;
        physicsVelocity.Linear += impulse / physicsMass.InverseMass;
        
        // Обновление компонента
        EntityManager.SetComponentData(targetEntity, physicsVelocity);
        
        // Создание эффекта лебедки
        CreateWinchEffect(point, force);
    }
}

private void CreateWinchEffect(float3 point, float3 force)
{
    // Создание визуального эффекта лебедки
    var effectEntity = EntityManager.CreateEntity();
    EntityManager.AddComponentData(effectEntity, new WinchEffectData
    {
        Position = point,
        Force = force,
        Duration = 1f,
        Intensity = math.length(force)
    });
}
```

### **Деформация террейна:**
- **Полная реализация** методов `ApplyDeformationToChunk()` и `UpdateChunkHeights()`
- **Интеграция с Unity Terrain API**
- **Система управления высотами**

```csharp
// TerrainDeformationSystem - ApplyDeformationToChunk метод
private void ApplyDeformationToChunk(Terrain terrain, int x, int z, int radius, float depth)
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
}

// UpdateChunkHeights метод
private void UpdateChunkHeights(Terrain terrain, int chunkIndex)
{
    var terrainData = terrain.terrainData;
    var chunkSize = 16; // 16x16 блоки
    var chunkX = (chunkIndex % 16) * chunkSize;
    var chunkZ = (chunkIndex / 16) * chunkSize;
    
    // Получение высот чанка
    var heights = terrainData.GetHeights(chunkX, chunkZ, chunkSize, chunkSize);
    
    // Обновление высот чанка
    for (int x = 0; x < chunkSize; x++)
    {
        for (int z = 0; z < chunkSize; z++)
        {
            // Применение сглаживания
            heights[z, x] = ApplySmoothing(heights, x, z, chunkSize);
        }
    }
    
    // Установка обновленных высот
    terrainData.SetHeights(chunkX, chunkZ, heights);
    
    // Синхронизация с TerrainCollider
    terrainData.SyncHeightmap();
}
```

