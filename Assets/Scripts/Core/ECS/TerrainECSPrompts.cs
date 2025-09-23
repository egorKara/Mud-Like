using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;
using Unity.Jobs;
using Unity.Collections;
using MudLike.Terrain.Components;
using MudLike.Core.Components;
using static MudLike.Core.Components.Position;

namespace MudLike.Core.ECS
{
    /// <summary>
    /// Контекстные промпты для работы с террейновыми ECS модулями
    /// </summary>
    public static class TerrainECSPromptContext
    {
        /// <summary>
        /// Контекст для создания террейновых систем
        /// </summary>
        public const string TerrainSystemContext = @"
@context Terrain System Creation:
Используй следующие компоненты для террейновых систем:

Террейновые компоненты:
- TerrainData: данные террейна (worldPosition, height, width, length)
- DeformationData: данные деформации (depth, radius, position, strength)
- MudData: данные грязи (level, viscosity, temperature, density)
- SurfaceData: данные поверхности (type, friction, hardness, moisture)
- WorldGridData: данные мировых координат (gridSize, chunkSize, offset)

Террейновые системы:
- TerrainDeformationSystem: деформация террейна
- MudManagerSystem: управление грязью
- TerrainSyncSystem: синхронизация террейна
- WorldGridSystem: система мировых координат
- TerrainHeightManagerSystem: управление высотами

Пример создания террейновой системы:
```csharp
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[BurstCompile(CompileSynchronously = true)]
public partial class YourTerrainSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = SystemAPI.Time.fixedDeltaTime;
        
        Entities
            .WithAll<DeformationData>()
            .ForEach((ref TerrainData terrain, in DeformationData deformation) =>
            {
                // Ваша логика деформации здесь
                ApplyDeformation(ref terrain, deformation);
            }).Schedule();
    }
}
```

Всегда используй:
- FixedStepSimulationSystemGroup для детерминизма
- BurstCompile для производительности
- MudManager API для взаимодействия с грязью
- Синхронизацию TerrainData + TerrainCollider
";

        /// <summary>
        /// Контекст для деформации террейна
        /// </summary>
        public const string TerrainDeformationContext = @"
@context Terrain Deformation:
Используй следующие техники для деформации террейна:

Деформация террейна:
- Heightfield per block: деформация по блокам
- Radius-based deformation: деформация по радиусу
- Strength-based deformation: деформация по силе
- Time-based recovery: восстановление со временем

Пример деформации:
```csharp
[BurstCompile]
public partial struct TerrainDeformationJob : IJobEntity
{
    public NativeArray<float> TerrainHeights;
    public float DeltaTime;
    public int TerrainSize;
    
    public void Execute(in WheelData wheel, in VehiclePhysics physics, in LocalTransform transform)
    {
        // Вычисляем позицию колеса
        float3 wheelPosition = transform.Position + wheel.Position;
        
        // Получаем индекс в массиве террейна
        int index = GetTerrainIndex(wheelPosition);
        if (index < 0 || index >= TerrainHeights.Length) return;
        
        // Вычисляем силу воздействия
        float wheelForce = math.length(physics.Velocity) * physics.Mass;
        
        // Применяем деформацию
        ApplyDeformation(index, wheelForce, wheel.Radius, DeltaTime);
    }
    
    [BurstCompile]
    private void ApplyDeformation(int index, float force, float radius, float deltaTime)
    {
        // Простая модель деформации
        float deformationAmount = force * 0.001f * deltaTime;
        
        // Обновляем высоту террейна
        TerrainHeights[index] -= deformationAmount;
    }
}
```

Всегда используй:
- Индексацию по координатам
- Ограничение силы деформации
- Временные коэффициенты
- Проверку границ массива
";

        /// <summary>
        /// Контекст для управления грязью
        /// </summary>
        public const string MudManagementContext = @"
@context Mud Management:
Используй следующие техники для управления грязью:

Управление грязью:
- MudManager API: API для взаимодействия с грязью
- Viscosity calculation: расчет вязкости
- Temperature effects: влияние температуры
- Density changes: изменение плотности

Пример управления грязью:
```csharp
[BurstCompile]
public partial struct MudManagementJob : IJobEntity
{
    public NativeArray<MudData> MudLevels;
    public float DeltaTime;
    public int TerrainSize;
    
    public void Execute(in WheelData wheel, in VehiclePhysics physics, in LocalTransform transform)
    {
        // Вычисляем позицию колеса
        float3 wheelPosition = transform.Position + wheel.Position;
        
        // Получаем индекс в массиве грязи
        int index = GetMudIndex(wheelPosition);
        if (index < 0 || index >= MudLevels.Length) return;
        
        // Обновляем грязь
        UpdateMud(index, wheel, physics, DeltaTime);
    }
    
    [BurstCompile]
    private void UpdateMud(int index, in WheelData wheel, in VehiclePhysics physics, float deltaTime)
    {
        var mudData = MudLevels[index];
        
        // Вычисляем влияние колеса на грязь
        float wheelInfluence = math.length(physics.Velocity) * wheel.Width * 0.001f;
        
        // Обновляем уровень грязи
        mudData.Level += wheelInfluence * deltaTime;
        mudData.Level = math.clamp(mudData.Level, 0f, 1f);
        
        // Обновляем вязкость
        mudData.Viscosity = CalculateViscosity(mudData.Level, mudData.Temperature);
        
        MudLevels[index] = mudData;
    }
}
```

Всегда используй:
- MudManager API для взаимодействия
- Расчет вязкости на основе уровня и температуры
- Ограничение значений грязи
- Влияние скорости и размера колеса
";

        /// <summary>
        /// Контекст для синхронизации террейна
        /// </summary>
        public const string TerrainSyncContext = @"
@context Terrain Synchronization:
Используй следующие техники для синхронизации террейна:

Синхронизация террейна:
- TerrainData sync: синхронизация данных террейна
- TerrainCollider sync: синхронизация коллайдера
- Heightfield sync: синхронизация высотного поля
- Chunk-based sync: синхронизация по чанкам

Пример синхронизации:
```csharp
[BurstCompile]
public partial struct TerrainSyncJob : IJobEntity
{
    public NativeArray<TerrainData> TerrainData;
    public NativeArray<float> TerrainHeights;
    public float DeltaTime;
    
    public void Execute(ref TerrainData terrain, in DeformationData deformation)
    {
        // Проверяем, изменился ли террейн
        if (HasTerrainChanged(terrain, deformation))
        {
            // Обновляем данные террейна
            UpdateTerrainData(ref terrain, deformation);
            
            // Синхронизируем высотное поле
            SyncHeightfield(terrain, deformation);
        }
    }
    
    [BurstCompile]
    private static bool HasTerrainChanged(in TerrainData terrain, in DeformationData deformation)
    {
        // Проверяем изменения в радиусе деформации
        float distance = math.distance(terrain.worldPosition, deformation.position);
        return distance <= deformation.radius;
    }
}
```

Всегда используй:
- Синхронизацию TerrainData + TerrainCollider
- Проверку изменений перед синхронизацией
- Обновление высотного поля
- Оптимизацию по чанкам
";

        /// <summary>
        /// Контекст для мировых координат
        /// </summary>
        public const string WorldGridContext = @"
@context World Grid System:
Используй следующие техники для мировых координат:

Мировые координаты:
- Grid-based positioning: позиционирование по сетке
- Chunk management: управление чанками
- Coordinate conversion: преобразование координат
- Spatial indexing: пространственная индексация

Пример мировых координат:
```csharp
[BurstCompile]
public partial struct WorldGridJob : IJobEntity
{
    public float GridSize;
    public float ChunkSize;
    public float3 WorldOffset;
    
    public void Execute(ref WorldGridData gridData, in LocalTransform transform)
    {
        // Вычисляем мировые координаты
        float3 worldPosition = transform.Position + WorldOffset;
        
        // Преобразуем в координаты сетки
        int3 gridPosition = WorldToGrid(worldPosition);
        
        // Вычисляем индекс чанка
        int chunkIndex = GridToChunkIndex(gridPosition);
        
        // Обновляем данные сетки
        gridData.GridPosition = gridPosition;
        gridData.ChunkIndex = chunkIndex;
        gridData.WorldPosition = worldPosition;
    }
    
    [BurstCompile]
    private int3 WorldToGrid(float3 worldPosition)
    {
        return new int3(
            (int)math.floor(worldPosition.x / GridSize),
            (int)math.floor(worldPosition.y / GridSize),
            (int)math.floor(worldPosition.z / GridSize)
        );
    }
    
    [BurstCompile]
    private int GridToChunkIndex(int3 gridPosition)
    {
        int chunkSize = (int)(ChunkSize / GridSize);
        return (gridPosition.z / chunkSize) * chunkSize + (gridPosition.x / chunkSize);
    }
}
```

Всегда используй:
- Преобразование мировых координат в сетку
- Индексацию чанков
- Оптимизацию пространственных запросов
- Кэширование вычислений
";
    }
}
