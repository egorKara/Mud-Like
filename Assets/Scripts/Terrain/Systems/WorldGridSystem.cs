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
    /// Система управления загрузкой террейна блоками
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [BurstCompile]
    public partial class WorldGridSystem : SystemBase
    {
        private const int GRID_SIZE = 16;
        private const int LOAD_RADIUS = 3;
        private const int UNLOAD_RADIUS = 5;
        private const float UPDATE_INTERVAL = 1.0f;
        
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
            
            return entity;
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