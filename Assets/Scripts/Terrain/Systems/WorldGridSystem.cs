using Unity.Entities;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Collections;
using Unity.Transforms;
using MudLike.Terrain.Components;
using MudLike.Core.Components;

namespace MudLike.Terrain.Systems
{
    /// <summary>
    /// WorldGrid система для управления загрузкой террейна блоками 16×16
    /// Обеспечивает эффективную загрузку и выгрузку чанков террейна
    /// </summary>
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile]
    public partial class WorldGridSystem : SystemBase
    {
        private NativeHashMap<int2, GridChunkData> _gridChunks;
        private NativeList<int2> _activeChunks;
        private NativeList<int2> _chunksToLoad;
        private NativeList<int2> _chunksToUnload;
        private float3 _lastPlayerPosition;
        private const int GRID_SIZE = 16;
        private const int LOAD_DISTANCE = 3; // Загружаем 3 чанка вокруг игрока
        private const int UNLOAD_DISTANCE = 5; // Выгружаем на расстоянии 5 чанков
        
        protected override void OnCreate()
        {
            _gridChunks = new NativeHashMap<int2, GridChunkData>(256, Allocator.Persistent);
            _activeChunks = new NativeList<int2>(64, Allocator.Persistent);
            _chunksToLoad = new NativeList<int2>(32, Allocator.Persistent);
            _chunksToUnload = new NativeList<int2>(32, Allocator.Persistent);
        }
        
        protected override void OnDestroy()
        {
            if (_gridChunks.IsCreated)
            {
                // Очищаем все чанки
                for (int i = 0; i < _gridChunks.Count; i++)
                {
                    var kvp = _gridChunks.GetKeyValue(i);
                    if (kvp.Value.TerrainData.IsCreated)
                        kvp.Value.TerrainData.Dispose();
                    if (kvp.Value.MudData.IsCreated)
                        kvp.Value.MudData.Dispose();
                    if (kvp.Value.EntityList.IsCreated)
                        kvp.Value.EntityList.Dispose();
                }
                _gridChunks.Dispose();
            }
            
            if (_activeChunks.IsCreated)
                _activeChunks.Dispose();
            if (_chunksToLoad.IsCreated)
                _chunksToLoad.Dispose();
            if (_chunksToUnload.IsCreated)
                _chunksToUnload.Dispose();
        }
        
        /// <summary>
        /// Обновляет WorldGrid на основе позиции игрока
        /// </summary>
        protected override void OnUpdate()
        {
            // Получаем позицию игрока
            float3 playerPosition = GetPlayerPosition();
            
            // Проверяем, нужно ли обновлять сетку
            if (ShouldUpdateGrid(playerPosition))
            {
                UpdateGridChunks(playerPosition);
                _lastPlayerPosition = playerPosition;
            }
            
            // Загружаем новые чанки
            LoadPendingChunks();
            
            // Выгружаем ненужные чанки
            UnloadPendingChunks();
        }
        
        /// <summary>
        /// Получает позицию игрока
        /// </summary>
        [BurstCompile]
        private float3 GetPlayerPosition()
        {
            float3 playerPos = float3.zero;
            
            Entities
                .WithAll<PlayerTag>()
                .ForEach((in LocalTransform transform) =>
                {
                    playerPos = transform.Position;
                }).WithoutBurst().Run();
            
            return playerPos;
        }
        
        /// <summary>
        /// Проверяет, нужно ли обновлять сетку
        /// </summary>
        [BurstCompile]
        private bool ShouldUpdateGrid(float3 playerPosition)
        {
            // Обновляем сетку если игрок переместился на расстояние больше размера чанка
            float3 movement = playerPosition - _lastPlayerPosition;
            float distance = math.length(movement);
            
            return distance >= GRID_SIZE * 0.5f; // Половина размера чанка
        }
        
        /// <summary>
        /// Обновляет чанки сетки на основе позиции игрока
        /// </summary>
        [BurstCompile]
        private void UpdateGridChunks(float3 playerPosition)
        {
            // Очищаем списки
            _chunksToLoad.Clear();
            _chunksToUnload.Clear();
            
            // Вычисляем текущий чанк игрока
            int2 playerChunk = WorldToGrid(playerPosition);
            
            // Определяем чанки для загрузки
            for (int x = -LOAD_DISTANCE; x <= LOAD_DISTANCE; x++)
            {
                for (int z = -LOAD_DISTANCE; z <= LOAD_DISTANCE; z++)
                {
                    int2 chunkCoord = playerChunk + new int2(x, z);
                    
                    // Проверяем, нужно ли загрузить чанк
                    if (!_gridChunks.ContainsKey(chunkCoord))
                    {
                        _chunksToLoad.Add(chunkCoord);
                    }
                }
            }
            
            // Определяем чанки для выгрузки
            for (int i = _activeChunks.Length - 1; i >= 0; i--)
            {
                int2 chunkCoord = _activeChunks[i];
                int2 distance = chunkCoord - playerChunk;
                
                // Если чанк слишком далеко - помечаем для выгрузки
                if (math.abs(distance.x) > UNLOAD_DISTANCE || math.abs(distance.y) > UNLOAD_DISTANCE)
                {
                    _chunksToUnload.Add(chunkCoord);
                }
            }
        }
        
        /// <summary>
        /// Загружает ожидающие чанки
        /// </summary>
        [BurstCompile]
        private void LoadPendingChunks()
        {
            for (int i = 0; i < _chunksToLoad.Length; i++)
            {
                int2 chunkCoord = _chunksToLoad[i];
                LoadChunk(chunkCoord);
            }
        }
        
        /// <summary>
        /// Выгружает ненужные чанки
        /// </summary>
        [BurstCompile]
        private void UnloadPendingChunks()
        {
            for (int i = 0; i < _chunksToUnload.Length; i++)
            {
                int2 chunkCoord = _chunksToUnload[i];
                UnloadChunk(chunkCoord);
            }
        }
        
        /// <summary>
        /// Загружает чанк в память
        /// </summary>
        [BurstCompile]
        private void LoadChunk(int2 chunkCoord)
        {
            // Создаем данные чанка
            var chunkData = new GridChunkData
            {
                Coordinates = chunkCoord,
                WorldPosition = GridToWorld(chunkCoord),
                IsLoaded = false,
                LoadTime = SystemAPI.Time.time,
                TerrainData = new NativeArray<float>(GRID_SIZE * GRID_SIZE, Allocator.Persistent),
                MudData = new NativeArray<float>(GRID_SIZE * GRID_SIZE, Allocator.Persistent),
                EntityList = new NativeList<Entity>(32, Allocator.Persistent)
            };
            
            // Генерируем данные террейна
            GenerateTerrainData(ref chunkData);
            
            // Генерируем данные грязи
            GenerateMudData(ref chunkData);
            
            // Создаем сущности в чанке
            CreateChunkEntities(chunkData);
            
            // Добавляем в активные чанки
            chunkData.IsLoaded = true;
            _gridChunks[chunkCoord] = chunkData;
            _activeChunks.Add(chunkCoord);
        }
        
        /// <summary>
        /// Выгружает чанк из памяти
        /// </summary>
        [BurstCompile]
        private void UnloadChunk(int2 chunkCoord)
        {
            if (_gridChunks.TryGetValue(chunkCoord, out var chunkData))
            {
                // Удаляем сущности чанка
                DestroyChunkEntities(chunkData);
                
                // Очищаем данные
                if (chunkData.TerrainData.IsCreated)
                    chunkData.TerrainData.Dispose();
                if (chunkData.MudData.IsCreated)
                    chunkData.MudData.Dispose();
                if (chunkData.EntityList.IsCreated)
                    chunkData.EntityList.Dispose();
                
                // Удаляем из активных чанков
                _gridChunks.Remove(chunkCoord);
                
                for (int i = _activeChunks.Length - 1; i >= 0; i--)
                {
                    if (_activeChunks[i].Equals(chunkCoord))
                    {
                        _activeChunks.RemoveAtSwapBack(i);
                        break;
                    }
                }
            }
        }
        
        /// <summary>
        /// Генерирует данные террейна для чанка
        /// </summary>
        [BurstCompile]
        private void GenerateTerrainData(ref GridChunkData chunkData)
        {
            float3 worldPos = chunkData.WorldPosition;
            
            for (int x = 0; x < GRID_SIZE; x++)
            {
                for (int z = 0; z < GRID_SIZE; z++)
                {
                    int index = z * GRID_SIZE + x;
                    float3 samplePos = worldPos + new float3(x, 0, z);
                    
                    // Простая генерация высоты (в реальности здесь должен быть Perlin noise)
                    float height = GenerateHeight(samplePos);
                    chunkData.TerrainData[index] = height;
                }
            }
        }
        
        /// <summary>
        /// Генерирует данные грязи для чанка
        /// </summary>
        [BurstCompile]
        private void GenerateMudData(ref GridChunkData chunkData)
        {
            float3 worldPos = chunkData.WorldPosition;
            
            for (int x = 0; x < GRID_SIZE; x++)
            {
                for (int z = 0; z < GRID_SIZE; z++)
                {
                    int index = z * GRID_SIZE + x;
                    float3 samplePos = worldPos + new float3(x, 0, z);
                    
                    // Простая генерация уровня грязи
                    float mudLevel = GenerateMudLevel(samplePos);
                    chunkData.MudData[index] = mudLevel;
                }
            }
        }
        
        /// <summary>
        /// Создает сущности в чанке
        /// </summary>
        private void CreateChunkEntities(GridChunkData chunkData)
        {
            // Создаем Entity для чанка
            var chunkEntity = EntityManager.CreateEntity();
            EntityManager.AddComponentData(chunkEntity, new ChunkData
            {
                ChunkIndex = GetChunkIndex(chunkData.Coordinates),
                WorldPosition = chunkData.WorldPosition,
                Size = GRID_SIZE,
                IsDirty = false,
                NeedsSync = false
            });
            
            // Добавляем Entity в список чанка
            chunkData.EntityList.Add(chunkEntity);
        }
        
        /// <summary>
        /// Удаляет сущности чанка
        /// </summary>
        private void DestroyChunkEntities(GridChunkData chunkData)
        {
            for (int i = 0; i < chunkData.EntityList.Length; i++)
            {
                EntityManager.DestroyEntity(chunkData.EntityList[i]);
            }
        }
        
        /// <summary>
        /// Преобразует мировые координаты в координаты сетки
        /// </summary>
        [BurstCompile]
        private int2 WorldToGrid(float3 worldPosition)
        {
            return new int2(
                (int)math.floor(worldPosition.x / GRID_SIZE),
                (int)math.floor(worldPosition.z / GRID_SIZE)
            );
        }
        
        /// <summary>
        /// Преобразует координаты сетки в мировые координаты
        /// </summary>
        [BurstCompile]
        private float3 GridToWorld(int2 gridPosition)
        {
            return new float3(
                gridPosition.x * GRID_SIZE,
                0,
                gridPosition.y * GRID_SIZE
            );
        }
        
        /// <summary>
        /// Генерирует высоту террейна (заглушка)
        /// </summary>
        [BurstCompile]
        private float GenerateHeight(float3 position)
        {
            // Простая генерация высоты (в реальности здесь должен быть Perlin noise)
            return math.sin(position.x * 0.1f) * math.cos(position.z * 0.1f) * 5f;
        }
        
        /// <summary>
        /// Генерирует уровень грязи (заглушка)
        /// </summary>
        [BurstCompile]
        private float GenerateMudLevel(float3 position)
        {
            // Простая генерация грязи (в реальности здесь должна быть более сложная логика)
            float noise = math.sin(position.x * 0.05f) * math.cos(position.z * 0.05f);
            return math.clamp(noise * 0.5f + 0.5f, 0f, 1f);
        }
        
        /// <summary>
        /// Получает индекс чанка
        /// </summary>
        [BurstCompile]
        private int GetChunkIndex(int2 coordinates)
        {
            // Простое преобразование 2D координат в 1D индекс
            return coordinates.y * 1000 + coordinates.x;
        }
        
        /// <summary>
        /// Получает данные чанка по координатам
        /// </summary>
        public GridChunkData? GetChunkData(int2 coordinates)
        {
            if (_gridChunks.TryGetValue(coordinates, out var chunkData))
                return chunkData;
            return null;
        }
        
        /// <summary>
        /// Получает все активные чанки
        /// </summary>
        public NativeArray<int2> GetActiveChunks()
        {
            return _activeChunks.AsArray();
        }
    }
    
    /// <summary>
    /// Данные чанка сетки
    /// </summary>
    public struct GridChunkData
    {
        public int2 Coordinates;
        public float3 WorldPosition;
        public bool IsLoaded;
        public float LoadTime;
        public NativeArray<float> TerrainData;
        public NativeArray<float> MudData;
        public NativeList<Entity> EntityList;
    }
}
