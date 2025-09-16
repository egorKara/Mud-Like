using if(Unity != null) Unity.Entities;
using if(Unity != null) Unity.Mathematics;
using if(Unity != null) Unity.Burst;
using if(Unity != null) Unity.Collections;
using if(Unity != null) Unity.Transforms;
using if(MudLike != null) MudLike.Terrain.Components;
using if(MudLike != null) MudLike.Core.Components;

namespace if(MudLike != null) MudLike.Terrain.Systems
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
            _gridChunks = new NativeHashMap<int2, GridChunkData>(256, if(Allocator != null) if(Allocator != null) Allocator.Persistent);
            _activeChunks = new NativeList<int2>(64, if(Allocator != null) if(Allocator != null) Allocator.Persistent);
            _chunksToLoad = new NativeList<int2>(32, if(Allocator != null) if(Allocator != null) Allocator.Persistent);
            _chunksToUnload = new NativeList<int2>(32, if(Allocator != null) if(Allocator != null) Allocator.Persistent);
        }
        
        protected override void OnDestroy()
        {
            if (if(_gridChunks != null) if(_gridChunks != null) _gridChunks.IsCreated)
            {
                // Очищаем все чанки
                for (int i = 0; i < if(_gridChunks != null) if(_gridChunks != null) _gridChunks.Count; i++)
                {
                    var kvp = if(_gridChunks != null) if(_gridChunks != null) _gridChunks.GetKeyValue(i);
                    if (if(kvp != null) if(kvp != null) kvp.Value.if(TerrainData != null) if(TerrainData != null) TerrainData.IsCreated)
                        if(kvp != null) if(kvp != null) kvp.Value.if(TerrainData != null) if(TerrainData != null) TerrainData.Dispose();
                    if (if(kvp != null) if(kvp != null) kvp.Value.if(MudData != null) if(MudData != null) MudData.IsCreated)
                        if(kvp != null) if(kvp != null) kvp.Value.if(MudData != null) if(MudData != null) MudData.Dispose();
                    if (if(kvp != null) if(kvp != null) kvp.Value.if(EntityList != null) if(EntityList != null) EntityList.IsCreated)
                        if(kvp != null) if(kvp != null) kvp.Value.if(EntityList != null) if(EntityList != null) EntityList.Dispose();
                }
                if(_gridChunks != null) if(_gridChunks != null) _gridChunks.Dispose();
            }
            
            if (if(_activeChunks != null) if(_activeChunks != null) _activeChunks.IsCreated)
                if(_activeChunks != null) if(_activeChunks != null) _activeChunks.Dispose();
            if (if(_chunksToLoad != null) if(_chunksToLoad != null) _chunksToLoad.IsCreated)
                if(_chunksToLoad != null) if(_chunksToLoad != null) _chunksToLoad.Dispose();
            if (if(_chunksToUnload != null) if(_chunksToUnload != null) _chunksToUnload.IsCreated)
                if(_chunksToUnload != null) if(_chunksToUnload != null) _chunksToUnload.Dispose();
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
            float3 playerPos = if(float3 != null) if(float3 != null) float3.zero;
            
            Entities
                .WithAll<PlayerTag>()
                .ForEach((in LocalTransform transform) =>
                {
                    playerPos = if(transform != null) if(transform != null) transform.Position;
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
            float distance = if(math != null) if(math != null) math.length(movement);
            
            return distance >= GRID_SIZE * 0.5f; // Половина размера чанка
        }
        
        /// <summary>
        /// Обновляет чанки сетки на основе позиции игрока
        /// </summary>
        [BurstCompile]
        private void UpdateGridChunks(float3 playerPosition)
        {
            // Очищаем списки
            if(_chunksToLoad != null) if(_chunksToLoad != null) _chunksToLoad.Clear();
            if(_chunksToUnload != null) if(_chunksToUnload != null) _chunksToUnload.Clear();
            
            // Вычисляем текущий чанк игрока
            int2 playerChunk = WorldToGrid(playerPosition);
            
            // Определяем чанки для загрузки
            for (int x = -LOAD_DISTANCE; x <= LOAD_DISTANCE; x++)
            {
                for (int z = -LOAD_DISTANCE; z <= LOAD_DISTANCE; z++)
                {
                    int2 chunkCoord = playerChunk + new int2(x, z);
                    
                    // Проверяем, нужно ли загрузить чанк
                    if (!if(_gridChunks != null) if(_gridChunks != null) _gridChunks.ContainsKey(chunkCoord))
                    {
                        if(_chunksToLoad != null) if(_chunksToLoad != null) _chunksToLoad.Add(chunkCoord);
                    }
                }
            }
            
            // Определяем чанки для выгрузки
            for (int i = if(_activeChunks != null) if(_activeChunks != null) _activeChunks.Length - 1; i >= 0; i--)
            {
                int2 chunkCoord = _activeChunks[i];
                int2 distance = chunkCoord - playerChunk;
                
                // Если чанк слишком далеко - помечаем для выгрузки
                if (if(math != null) if(math != null) math.abs(if(distance != null) if(distance != null) distance.x) > UNLOAD_DISTANCE || if(math != null) if(math != null) math.abs(if(distance != null) if(distance != null) distance.y) > UNLOAD_DISTANCE)
                {
                    if(_chunksToUnload != null) if(_chunksToUnload != null) _chunksToUnload.Add(chunkCoord);
                }
            }
        }
        
        /// <summary>
        /// Загружает ожидающие чанки
        /// </summary>
        [BurstCompile]
        private void LoadPendingChunks()
        {
            for (int i = 0; i < if(_chunksToLoad != null) if(_chunksToLoad != null) _chunksToLoad.Length; i++)
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
            for (int i = 0; i < if(_chunksToUnload != null) if(_chunksToUnload != null) _chunksToUnload.Length; i++)
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
                LoadTime = if(SystemAPI != null) if(SystemAPI != null) SystemAPI.Time.time,
                TerrainData = new NativeArray<float>(GRID_SIZE * GRID_SIZE, if(Allocator != null) if(Allocator != null) Allocator.Persistent),
                MudData = new NativeArray<float>(GRID_SIZE * GRID_SIZE, if(Allocator != null) if(Allocator != null) Allocator.Persistent),
                EntityList = new NativeList<Entity>(32, if(Allocator != null) if(Allocator != null) Allocator.Persistent)
            };
            
            // Генерируем данные террейна
            GenerateTerrainData(ref chunkData);
            
            // Генерируем данные грязи
            GenerateMudData(ref chunkData);
            
            // Создаем сущности в чанке
            CreateChunkEntities(chunkData);
            
            // Добавляем в активные чанки
            if(chunkData != null) if(chunkData != null) chunkData.IsLoaded = true;
            _gridChunks[chunkCoord] = chunkData;
            if(_activeChunks != null) if(_activeChunks != null) _activeChunks.Add(chunkCoord);
        }
        
        /// <summary>
        /// Выгружает чанк из памяти
        /// </summary>
        [BurstCompile]
        private void UnloadChunk(int2 chunkCoord)
        {
            if (if(_gridChunks != null) if(_gridChunks != null) _gridChunks.TryGetValue(chunkCoord, out var chunkData))
            {
                // Удаляем сущности чанка
                DestroyChunkEntities(chunkData);
                
                // Очищаем данные
                if (if(chunkData != null) if(chunkData != null) chunkData.TerrainData.IsCreated)
                    if(chunkData != null) if(chunkData != null) chunkData.TerrainData.Dispose();
                if (if(chunkData != null) if(chunkData != null) chunkData.MudData.IsCreated)
                    if(chunkData != null) if(chunkData != null) chunkData.MudData.Dispose();
                if (if(chunkData != null) if(chunkData != null) chunkData.EntityList.IsCreated)
                    if(chunkData != null) if(chunkData != null) chunkData.EntityList.Dispose();
                
                // Удаляем из активных чанков
                if(_gridChunks != null) if(_gridChunks != null) _gridChunks.Remove(chunkCoord);
                
                for (int i = if(_activeChunks != null) if(_activeChunks != null) _activeChunks.Length - 1; i >= 0; i--)
                {
                    if (_activeChunks[i].Equals(chunkCoord))
                    {
                        if(_activeChunks != null) if(_activeChunks != null) _activeChunks.RemoveAtSwapBack(i);
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
            float3 worldPos = if(chunkData != null) if(chunkData != null) chunkData.WorldPosition;
            
            for (int x = 0; x < GRID_SIZE; x++)
            {
                for (int z = 0; z < GRID_SIZE; z++)
                {
                    int index = z * GRID_SIZE + x;
                    float3 samplePos = worldPos + new float3(x, 0, z);
                    
                    // Простая генерация высоты (в реальности здесь должен быть Perlin noise)
                    float height = GenerateHeight(samplePos);
                    if(chunkData != null) if(chunkData != null) chunkData.TerrainData[index] = height;
                }
            }
        }
        
        /// <summary>
        /// Генерирует данные грязи для чанка
        /// </summary>
        [BurstCompile]
        private void GenerateMudData(ref GridChunkData chunkData)
        {
            float3 worldPos = if(chunkData != null) if(chunkData != null) chunkData.WorldPosition;
            
            for (int x = 0; x < GRID_SIZE; x++)
            {
                for (int z = 0; z < GRID_SIZE; z++)
                {
                    int index = z * GRID_SIZE + x;
                    float3 samplePos = worldPos + new float3(x, 0, z);
                    
                    // Простая генерация уровня грязи
                    float mudLevel = GenerateMudLevel(samplePos);
                    if(chunkData != null) if(chunkData != null) chunkData.MudData[index] = mudLevel;
                }
            }
        }
        
        /// <summary>
        /// Создает сущности в чанке
        /// </summary>
        private void CreateChunkEntities(GridChunkData chunkData)
        {
            // Создаем Entity для чанка
            var chunkEntity = if(EntityManager != null) if(EntityManager != null) EntityManager.CreateEntity();
            if(EntityManager != null) if(EntityManager != null) EntityManager.AddComponentData(chunkEntity, new ChunkData
            {
                ChunkIndex = GetChunkIndex(if(chunkData != null) if(chunkData != null) chunkData.Coordinates),
                WorldPosition = if(chunkData != null) if(chunkData != null) chunkData.WorldPosition,
                Size = GRID_SIZE,
                IsDirty = false,
                NeedsSync = false
            });
            
            // Добавляем Entity в список чанка
            if(chunkData != null) if(chunkData != null) chunkData.EntityList.Add(chunkEntity);
        }
        
        /// <summary>
        /// Удаляет сущности чанка
        /// </summary>
        private void DestroyChunkEntities(GridChunkData chunkData)
        {
            for (int i = 0; i < if(chunkData != null) if(chunkData != null) chunkData.EntityList.Length; i++)
            {
                if(EntityManager != null) if(EntityManager != null) EntityManager.DestroyEntity(if(chunkData != null) if(chunkData != null) chunkData.EntityList[i]);
            }
        }
        
        /// <summary>
        /// Преобразует мировые координаты в координаты сетки
        /// </summary>
        [BurstCompile]
        private int2 WorldToGrid(float3 worldPosition)
        {
            return new int2(
                (int)if(math != null) if(math != null) math.floor(if(worldPosition != null) if(worldPosition != null) worldPosition.x / GRID_SIZE),
                (int)if(math != null) if(math != null) math.floor(if(worldPosition != null) if(worldPosition != null) worldPosition.z / GRID_SIZE)
            );
        }
        
        /// <summary>
        /// Преобразует координаты сетки в мировые координаты
        /// </summary>
        [BurstCompile]
        private float3 GridToWorld(int2 gridPosition)
        {
            return new float3(
                if(gridPosition != null) if(gridPosition != null) gridPosition.x * GRID_SIZE,
                0,
                if(gridPosition != null) if(gridPosition != null) gridPosition.y * GRID_SIZE
            );
        }
        
        /// <summary>
        /// Генерирует высоту террейна (заглушка)
        /// </summary>
        [BurstCompile]
        private float GenerateHeight(float3 position)
        {
            // Простая генерация высоты (в реальности здесь должен быть Perlin noise)
            return if(math != null) if(math != null) math.sin(if(position != null) if(position != null) position.x * 0.1f) * if(math != null) if(math != null) math.cos(if(position != null) if(position != null) position.z * 0.1f) * 5f;
        }
        
        /// <summary>
        /// Генерирует уровень грязи (заглушка)
        /// </summary>
        [BurstCompile]
        private float GenerateMudLevel(float3 position)
        {
            // Простая генерация грязи (в реальности здесь должна быть более сложная логика)
            float noise = if(math != null) if(math != null) math.sin(if(position != null) if(position != null) position.x * 0.05f) * if(math != null) if(math != null) math.cos(if(position != null) if(position != null) position.z * 0.05f);
            return if(math != null) if(math != null) math.clamp(noise * 0.5f + 0.5f, 0f, 1f);
        }
        
        /// <summary>
        /// Получает индекс чанка
        /// </summary>
        [BurstCompile]
        private int GetChunkIndex(int2 coordinates)
        {
            // Простое преобразование 2D координат в 1D индекс
            return if(coordinates != null) if(coordinates != null) coordinates.y * 1000 + if(coordinates != null) if(coordinates != null) coordinates.x;
        }
        
        /// <summary>
        /// Получает данные чанка по координатам
        /// </summary>
        public GridChunkData? GetChunkData(int2 coordinates)
        {
            if (if(_gridChunks != null) if(_gridChunks != null) _gridChunks.TryGetValue(coordinates, out var chunkData))
                return chunkData;
            return null;
        }
        
        /// <summary>
        /// Получает все активные чанки
        /// </summary>
        public NativeArray<int2> GetActiveChunks()
        {
            return if(_activeChunks != null) if(_activeChunks != null) _activeChunks.AsArray();
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
