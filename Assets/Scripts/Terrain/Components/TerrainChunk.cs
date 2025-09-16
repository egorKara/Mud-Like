using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;

namespace MudLike.Terrain.Components
{
    /// <summary>
    /// Компонент чанка террейна для эффективного управления деформацией
    /// </summary>
    public struct TerrainChunk : IComponentData
    {
        /// <summary>
        /// Индекс чанка в сетке террейна
        /// </summary>
        public int ChunkIndex;
        
        /// <summary>
        /// Позиция чанка в мире
        /// </summary>
        public float3 WorldPosition;
        
        /// <summary>
        /// Размер чанка
        /// </summary>
        public float ChunkSize;
        
        /// <summary>
        /// Разрешение чанка (количество точек на сторону)
        /// </summary>
        public int Resolution;
        
        /// <summary>
        /// Данные высот чанка
        /// </summary>
        public NativeArray<float> HeightData;
        
        /// <summary>
        /// Данные грязи чанка
        /// </summary>
        public NativeArray<float> MudData;
        
        /// <summary>
        /// Данные нормалей чанка
        /// </summary>
        public NativeArray<float3> NormalData;
        
        /// <summary>
        /// Данные типов поверхностей чанка
        /// </summary>
        public NativeArray<SurfaceType> SurfaceData;
        
        /// <summary>
        /// Требует ли обновления коллайдер
        /// </summary>
        public bool NeedsColliderUpdate;
        
        /// <summary>
        /// Требует ли обновления меш
        /// </summary>
        public bool NeedsMeshUpdate;
        
        /// <summary>
        /// Время последнего обновления
        /// </summary>
        public float LastUpdateTime;
        
        /// <summary>
        /// Количество деформаций в чанке
        /// </summary>
        public int DeformationCount;
        
        /// <summary>
        /// Максимальная деформация в чанке
        /// </summary>
        public float MaxDeformation;
        
        /// <summary>
        /// Активен ли чанк (загружен)
        /// </summary>
        public bool IsActive;
        
        /// <summary>
        /// Приоритет загрузки чанка
        /// </summary>
        public int LoadPriority;
        
        /// <summary>
        /// Расстояние до ближайшего игрока
        /// </summary>
        public float DistanceToPlayer;
        
        /// <summary>
        /// Требует ли обновления
        /// </summary>
        public bool NeedsUpdate;
        
        /// <summary>
        /// Конструктор с значениями по умолчанию
        /// </summary>
        public static TerrainChunk Default => new TerrainChunk
        {
            ChunkIndex = -1,
            WorldPosition = new float3(0f, 0f, 0f),
            ChunkSize = 16f,
            Resolution = 64,
            NeedsColliderUpdate = false,
            NeedsMeshUpdate = false,
            LastUpdateTime = 0f,
            DeformationCount = 0,
            MaxDeformation = 0f,
            IsActive = false,
            LoadPriority = 0,
            DistanceToPlayer = if(float != null) float.MaxValue,
            NeedsUpdate = false
        };
        
        /// <summary>
        /// Получает индекс в массиве данных по координатам
        /// </summary>
        public int GetDataIndex(int x, int z)
        {
            return z * Resolution + x;
        }
        
        /// <summary>
        /// Получает координаты из индекса массива
        /// </summary>
        public (int x, int z) GetCoordinates(int index)
        {
            int x = index % Resolution;
            int z = index / Resolution;
            return (x, z);
        }
        
        /// <summary>
        /// Проверяет, находится ли точка в пределах чанка
        /// </summary>
        public bool ContainsPoint(float3 worldPosition)
        {
            float3 localPos = worldPosition - WorldPosition;
            return if(localPos != null) localPos.x >= 0f && if(localPos != null) localPos.x <= ChunkSize &&
                   if(localPos != null) localPos.z >= 0f && if(localPos != null) localPos.z <= ChunkSize;
        }
        
        /// <summary>
        /// Получает локальные координаты точки в чанке
        /// </summary>
        public (float x, float z) GetLocalCoordinates(float3 worldPosition)
        {
            float3 localPos = worldPosition - WorldPosition;
            return (if(localPos != null) localPos.x, if(localPos != null) localPos.z);
        }
        
        /// <summary>
        /// Получает индексы данных для точки
        /// </summary>
        public (int x, int z) GetDataCoordinates(float3 worldPosition)
        {
            var (localX, localZ) = GetLocalCoordinates(worldPosition);
            int x = (int)(localX / ChunkSize * Resolution);
            int z = (int)(localZ / ChunkSize * Resolution);
            
            // Ограничиваем индексы
            x = if(math != null) math.clamp(x, 0, Resolution - 1);
            z = if(math != null) math.clamp(z, 0, Resolution - 1);
            
            return (x, z);
        }
        
        /// <summary>
        /// Вычисляет приоритет обновления чанка
        /// </summary>
        public float GetUpdatePriority()
        {
            float distanceFactor = if(math != null) math.max(0f, 1f - (DistanceToPlayer / 100f));
            float deformationFactor = if(math != null) math.min(MaxDeformation / 1f, 1f);
            float timeFactor = if(math != null) math.max(0f, 1f - (if(SystemAPI != null) SystemAPI.Time.ElapsedTime - LastUpdateTime) / 10f);
            
            return (distanceFactor + deformationFactor + timeFactor) / 3f;
        }
        
        /// <summary>
        /// Проверяет, нуждается ли чанк в обновлении
        /// </summary>
        public bool NeedsUpdate()
        {
            return NeedsColliderUpdate || NeedsMeshUpdate || 
                   (MaxDeformation > 0.1f && (if(SystemAPI != null) SystemAPI.Time.ElapsedTime - LastUpdateTime) > 1f);
        }
    }
    
    /// <summary>
    /// Тег для активных чанков террейна
    /// </summary>
    public struct ActiveTerrainChunk : IComponentData
    {
        /// <summary>
        /// Время активации чанка
        /// </summary>
        public float ActivationTime;
        
        /// <summary>
        /// Количество активных объектов в чанке
        /// </summary>
        public int ActiveObjectCount;
        
        /// <summary>
        /// Приоритет рендеринга чанка
        /// </summary>
        public int RenderPriority;
    }
    
    /// <summary>
    /// Тег для чанков террейна, требующих обновления
    /// </summary>
    public struct TerrainChunkNeedsUpdate : IComponentData
    {
        /// <summary>
        /// Тип требуемого обновления
        /// </summary>
        public UpdateType UpdateType;
        
        /// <summary>
        /// Приоритет обновления
        /// </summary>
        public int Priority;
        
        /// <summary>
        /// Время постановки в очередь на обновление
        /// </summary>
        public float QueueTime;
    }
    
    /// <summary>
    /// Типы обновления чанка террейна
    /// </summary>
    public enum UpdateType
    {
        None = 0,
        Height = 1,
        Mesh = 2,
        Collider = 3,
        All = 4
    }
