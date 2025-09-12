using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;
using Unity.Collections;
using MudLike.Terrain.Components;
using MudLike.Vehicles.Components;

namespace MudLike.Terrain.Systems
{
    /// <summary>
    /// Система управления загрузкой террейна блоками для оптимизации производительности
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [BurstCompile]
    public partial class WorldGridSystem : SystemBase
    {
        private const int GRID_SIZE = 16; // Размер блока в единицах
        private const int LOAD_RADIUS = 3; // Радиус загрузки вокруг игрока
        private const int UNLOAD_RADIUS = 5; // Радиус выгрузки вокруг игрока
        private const float UPDATE_INTERVAL = 1.0f; // Интервал обновления в секундах
        
        private NativeHashMap<int2, Entity> _loadedChunks;
        private NativeHashMap<int2, bool> _chunkStates;
        private NativeList<int2> _chunksToLoad;
        private NativeList<int2> _chunksToUnload;
        private float _lastUpdateTime;
        
        protected override void OnCreate()
        {
            _loadedChunks = new NativeHashMap<int2, Entity>(100, Allocator.Persistent);
            _chunkStates = new NativeHashMap<int2, bool>(100, Allocator.Persistent);
            _chunksToLoad = new NativeList<int2>(50, Allocator.Persistent);
            _chunksToUnload = new NativeList<int2>(50, Allocator.Persistent);
        }
        
        protected override void OnUpdate()
        {
            float deltaTime = SystemAPI.Time.DeltaTime;
            _lastUpdateTime += deltaTime;
            
            // Обновляем сетку только через определенные интервалы
            if (_lastUpdateTime >= UPDATE_INTERVAL)
            {
                UpdateWorldGrid();
                _lastUpdateTime = 0f;
            }
        }
        
        /// <summary>
        /// Обновляет сетку мира
        /// </summary>
        private void UpdateWorldGrid()
        {
            // Очищаем списки
            _chunksToLoad.Clear();
            _chunksToUnload.Clear();
            
            // Находим всех игроков
            var players = GetEntityQuery(typeof(PlayerTag), typeof(LocalTransform));
            var playerEntities = players.ToEntityArray(Allocator.Temp);
            var playerTransforms = players.ToComponentDataArray<LocalTransform>(Allocator.Temp);
            
            // Определяем, какие чанки нужно загрузить/выгрузить
            for (int i = 0; i < playerEntities.Length; i++)
            {
                var playerPos = playerTransforms[i].Position;
                var playerChunk = GetChunkCoordinate(playerPos);
                
                // Определяем чанки для загрузки
                for (int x = -LOAD_RADIUS; x <= LOAD_RADIUS; x++)
                {
                    for (int z = -LOAD_RADIUS; z <= LOAD_RADIUS; z++)
                    {
                        var chunkCoord = new int2(playerChunk.x + x, playerChunk.y + z);
                        
                        if (!_loadedChunks.ContainsKey(chunkCoord))
                        {
                            _chunksToLoad.Add(chunkCoord);
                        }
                    }
                }
                
                // Определяем чанки для выгрузки
                var chunksToCheck = new NativeList<int2>(Allocator.Temp);
                foreach (var kvp in _loadedChunks)
                {
                    chunksToCheck.Add(kvp.Key);
                }
                
                foreach (var chunkCoord in chunksToCheck)
                {
                    if (math.distance(chunkCoord, playerChunk) > UNLOAD_RADIUS)
                    {
                        _chunksToUnload.Add(chunkCoord);
                    }
                }
                
                chunksToCheck.Dispose();
            }
            
            // Загружаем чанки
            LoadChunks();
            
            // Выгружаем чанки
            UnloadChunks();
            
            // Освобождаем временные массивы
            playerEntities.Dispose();
            playerTransforms.Dispose();
        }
        
        /// <summary>
        /// Загружает чанки
        /// </summary>
        private void LoadChunks()
        {
            foreach (var chunkCoord in _chunksToLoad)
            {
                if (_loadedChunks.ContainsKey(chunkCoord))
                    continue;
                
                var chunk = CreateChunk(chunkCoord);
                _loadedChunks[chunkCoord] = chunk;
                _chunkStates[chunkCoord] = true;
            }
        }
        
        /// <summary>
        /// Выгружает чанки
        /// </summary>
        private void UnloadChunks()
        {
            foreach (var chunkCoord in _chunksToUnload)
            {
                if (!_loadedChunks.ContainsKey(chunkCoord))
                    continue;
                
                var chunk = _loadedChunks[chunkCoord];
                if (EntityManager.Exists(chunk))
                {
                    EntityManager.DestroyEntity(chunk);
                }
                
                _loadedChunks.Remove(chunkCoord);
                _chunkStates.Remove(chunkCoord);
            }
        }
        
        /// <summary>
        /// Создает чанк террейна
        /// </summary>
        private Entity CreateChunk(int2 chunkCoord)
        {
            var entity = EntityManager.CreateEntity();
            
            // Добавляем компоненты чанка
            EntityManager.AddComponentData(entity, new TerrainChunkData
            {
                ChunkCoordinate = chunkCoord,
                Size = GRID_SIZE,
                Resolution = 1.0f,
                IsLoaded = true,
                NeedsUpdate = false,
                LastUpdateTime = 0f
            });
            
            EntityManager.AddComponentData(entity, new LocalTransform
            {
                Position = new float3(chunkCoord.x * GRID_SIZE, 0, chunkCoord.y * GRID_SIZE),
                Rotation = quaternion.identity,
                Scale = 1f
            });
            
            // Создаем поверхность для чанка
            CreateChunkSurface(entity, chunkCoord);
            
            return entity;
        }
        
        /// <summary>
        /// Создает поверхность для чанка
        /// </summary>
        private void CreateChunkSurface(Entity chunk, int2 chunkCoord)
        {
            // Создаем сетку поверхностей для чанка
            for (int x = 0; x < GRID_SIZE; x++)
            {
                for (int z = 0; z < GRID_SIZE; z++)
                {
                    var surfaceEntity = EntityManager.CreateEntity();
                    
                    // Определяем тип поверхности на основе координат
                    var surfaceType = DetermineSurfaceType(chunkCoord, x, z);
                    
                    EntityManager.AddComponentData(surfaceEntity, new SurfaceData
                    {
                        SurfaceType = surfaceType,
                        FrictionCoefficient = GetFrictionCoefficient(surfaceType),
                        TractionCoefficient = GetTractionCoefficient(surfaceType),
                        RollingResistance = GetRollingResistance(surfaceType),
                        Viscosity = GetViscosity(surfaceType),
                        Density = GetDensity(surfaceType),
                        PenetrationDepth = GetPenetrationDepth(surfaceType),
                        DryingRate = GetDryingRate(surfaceType),
                        FreezingPoint = GetFreezingPoint(surfaceType),
                        Temperature = 20f,
                        Moisture = GetMoisture(surfaceType),
                        NeedsUpdate = false
                    });
                    
                    EntityManager.AddComponentData(surfaceEntity, new LocalTransform
                    {
                        Position = new float3(
                            chunkCoord.x * GRID_SIZE + x,
                            0,
                            chunkCoord.y * GRID_SIZE + z
                        ),
                        Rotation = quaternion.identity,
                        Scale = 1f
                    });
                    
                    EntityManager.AddComponentData(surfaceEntity, new ChunkSurfaceTag
                    {
                        ChunkEntity = chunk,
                        LocalX = x,
                        LocalZ = z
                    });
                }
            }
        }
        
        /// <summary>
        /// Определяет тип поверхности на основе координат
        /// </summary>
        [BurstCompile]
        private static SurfaceType DetermineSurfaceType(int2 chunkCoord, int x, int z)
        {
            // Простая генерация на основе координат
            float noise = math.sin(chunkCoord.x * 0.1f + x * 0.01f) * 
                         math.cos(chunkCoord.y * 0.1f + z * 0.01f);
            
            if (noise > 0.5f)
                return SurfaceType.Grass;
            else if (noise > 0.0f)
                return SurfaceType.Dirt;
            else if (noise > -0.3f)
                return SurfaceType.Mud;
            else
                return SurfaceType.Water;
        }
        
        /// <summary>
        /// Получает координату чанка для позиции
        /// </summary>
        [BurstCompile]
        private static int2 GetChunkCoordinate(float3 position)
        {
            return new int2(
                (int)math.floor(position.x / GRID_SIZE),
                (int)math.floor(position.z / GRID_SIZE)
            );
        }
        
        /// <summary>
        /// Получает коэффициент трения для типа поверхности
        /// </summary>
        [BurstCompile]
        private static float GetFrictionCoefficient(SurfaceType surfaceType)
        {
            return surfaceType switch
            {
                SurfaceType.Asphalt => 0.8f,
                SurfaceType.Concrete => 0.75f,
                SurfaceType.Dirt => 0.6f,
                SurfaceType.Mud => 0.3f,
                SurfaceType.Sand => 0.4f,
                SurfaceType.Grass => 0.5f,
                SurfaceType.Water => 0.1f,
                SurfaceType.Ice => 0.1f,
                SurfaceType.Snow => 0.2f,
                SurfaceType.Rock => 0.7f,
                SurfaceType.Gravel => 0.6f,
                SurfaceType.Swamp => 0.2f,
                _ => 0.5f
            };
        }
        
        /// <summary>
        /// Получает коэффициент сцепления для типа поверхности
        /// </summary>
        [BurstCompile]
        private static float GetTractionCoefficient(SurfaceType surfaceType)
        {
            return surfaceType switch
            {
                SurfaceType.Asphalt => 0.9f,
                SurfaceType.Concrete => 0.85f,
                SurfaceType.Dirt => 0.7f,
                SurfaceType.Mud => 0.4f,
                SurfaceType.Sand => 0.5f,
                SurfaceType.Grass => 0.6f,
                SurfaceType.Water => 0.2f,
                SurfaceType.Ice => 0.15f,
                SurfaceType.Snow => 0.3f,
                SurfaceType.Rock => 0.8f,
                SurfaceType.Gravel => 0.7f,
                SurfaceType.Swamp => 0.25f,
                _ => 0.6f
            };
        }
        
        /// <summary>
        /// Получает сопротивление качению для типа поверхности
        /// </summary>
        [BurstCompile]
        private static float GetRollingResistance(SurfaceType surfaceType)
        {
            return surfaceType switch
            {
                SurfaceType.Asphalt => 0.02f,
                SurfaceType.Concrete => 0.025f,
                SurfaceType.Dirt => 0.05f,
                SurfaceType.Mud => 0.15f,
                SurfaceType.Sand => 0.2f,
                SurfaceType.Grass => 0.08f,
                SurfaceType.Water => 0.3f,
                SurfaceType.Ice => 0.05f,
                SurfaceType.Snow => 0.1f,
                SurfaceType.Rock => 0.03f,
                SurfaceType.Gravel => 0.06f,
                SurfaceType.Swamp => 0.25f,
                _ => 0.05f
            };
        }
        
        /// <summary>
        /// Получает вязкость для типа поверхности
        /// </summary>
        [BurstCompile]
        private static float GetViscosity(SurfaceType surfaceType)
        {
            return surfaceType switch
            {
                SurfaceType.Asphalt => 0.0f,
                SurfaceType.Concrete => 0.0f,
                SurfaceType.Dirt => 0.1f,
                SurfaceType.Mud => 0.8f,
                SurfaceType.Sand => 0.0f,
                SurfaceType.Grass => 0.0f,
                SurfaceType.Water => 0.0f,
                SurfaceType.Ice => 0.0f,
                SurfaceType.Snow => 0.0f,
                SurfaceType.Rock => 0.0f,
                SurfaceType.Gravel => 0.0f,
                SurfaceType.Swamp => 0.9f,
                _ => 0.0f
            };
        }
        
        /// <summary>
        /// Получает плотность для типа поверхности
        /// </summary>
        [BurstCompile]
        private static float GetDensity(SurfaceType surfaceType)
        {
            return surfaceType switch
            {
                SurfaceType.Asphalt => 2400f,
                SurfaceType.Concrete => 2500f,
                SurfaceType.Dirt => 1800f,
                SurfaceType.Mud => 2000f,
                SurfaceType.Sand => 1600f,
                SurfaceType.Grass => 1200f,
                SurfaceType.Water => 1000f,
                SurfaceType.Ice => 900f,
                SurfaceType.Snow => 300f,
                SurfaceType.Rock => 2800f,
                SurfaceType.Gravel => 2000f,
                SurfaceType.Swamp => 1500f,
                _ => 1500f
            };
        }
        
        /// <summary>
        /// Получает глубину проникновения для типа поверхности
        /// </summary>
        [BurstCompile]
        private static float GetPenetrationDepth(SurfaceType surfaceType)
        {
            return surfaceType switch
            {
                SurfaceType.Asphalt => 0.0f,
                SurfaceType.Concrete => 0.0f,
                SurfaceType.Dirt => 0.1f,
                SurfaceType.Mud => 0.3f,
                SurfaceType.Sand => 0.2f,
                SurfaceType.Grass => 0.05f,
                SurfaceType.Water => 0.5f,
                SurfaceType.Ice => 0.0f,
                SurfaceType.Snow => 0.4f,
                SurfaceType.Rock => 0.0f,
                SurfaceType.Gravel => 0.05f,
                SurfaceType.Swamp => 0.6f,
                _ => 0.1f
            };
        }
        
        /// <summary>
        /// Получает скорость высыхания для типа поверхности
        /// </summary>
        [BurstCompile]
        private static float GetDryingRate(SurfaceType surfaceType)
        {
            return surfaceType switch
            {
                SurfaceType.Asphalt => 1.0f,
                SurfaceType.Concrete => 1.0f,
                SurfaceType.Dirt => 0.3f,
                SurfaceType.Mud => 0.1f,
                SurfaceType.Sand => 0.8f,
                SurfaceType.Grass => 0.5f,
                SurfaceType.Water => 0.0f,
                SurfaceType.Ice => 0.0f,
                SurfaceType.Snow => 0.0f,
                SurfaceType.Rock => 1.0f,
                SurfaceType.Gravel => 0.7f,
                SurfaceType.Swamp => 0.05f,
                _ => 0.5f
            };
        }
        
        /// <summary>
        /// Получает точку замерзания для типа поверхности
        /// </summary>
        [BurstCompile]
        private static float GetFreezingPoint(SurfaceType surfaceType)
        {
            return surfaceType switch
            {
                SurfaceType.Water => 0f,
                SurfaceType.Ice => 0f,
                SurfaceType.Snow => 0f,
                _ => 0f
            };
        }
        
        /// <summary>
        /// Получает влажность для типа поверхности
        /// </summary>
        [BurstCompile]
        private static float GetMoisture(SurfaceType surfaceType)
        {
            return surfaceType switch
            {
                SurfaceType.Asphalt => 0.0f,
                SurfaceType.Concrete => 0.0f,
                SurfaceType.Dirt => 0.3f,
                SurfaceType.Mud => 0.8f,
                SurfaceType.Sand => 0.1f,
                SurfaceType.Grass => 0.4f,
                SurfaceType.Water => 1.0f,
                SurfaceType.Ice => 0.0f,
                SurfaceType.Snow => 0.0f,
                SurfaceType.Rock => 0.1f,
                SurfaceType.Gravel => 0.2f,
                SurfaceType.Swamp => 0.95f,
                _ => 0.3f
            };
        }
        
        protected override void OnDestroy()
        {
            if (_loadedChunks.IsCreated)
                _loadedChunks.Dispose();
            if (_chunkStates.IsCreated)
                _chunkStates.Dispose();
            if (_chunksToLoad.IsCreated)
                _chunksToLoad.Dispose();
            if (_chunksToUnload.IsCreated)
                _chunksToUnload.Dispose();
        }
    }
}