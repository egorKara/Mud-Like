using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;

namespace MudLike.Terrain.Components
{
    /// <summary>
    /// Компонент чанка террейна для ECS
    /// </summary>
    public struct TerrainChunk : IComponentData
    {
        /// <summary>
        /// Индекс чанка в сетке
        /// </summary>
        public int Index;
        
        /// <summary>
        /// Позиция чанка в мире
        /// </summary>
        public float3 WorldPosition;
        
        /// <summary>
        /// Размер чанка
        /// </summary>
        public int Size;
        
        /// <summary>
        /// Высоты в чанке (NativeArray индексы)
        /// </summary>
        public int HeightDataIndex;
        
        /// <summary>
        /// Данные грязи в чанке (NativeArray индексы)
        /// </summary>
        public int MudDataIndex;
        
        /// <summary>
        /// Данные нормалей в чанке (NativeArray индексы)
        /// </summary>
        public int NormalDataIndex;
        
        /// <summary>
        /// Флаг грязности чанка (нужно ли обновление)
        /// </summary>
        public bool IsDirty;
        
        /// <summary>
        /// Флаг необходимости синхронизации
        /// </summary>
        public bool NeedsSync;
        
        /// <summary>
        /// Время последнего обновления
        /// </summary>
        public float LastUpdateTime;
        
        /// <summary>
        /// Уровень детализации
        /// </summary>
        public int LODLevel;
    }
    
    /// <summary>
    /// Данные террейна (Singleton)
    /// </summary>
    public struct TerrainData : IComponentData
    {
        /// <summary>
        /// Количество чанков по X
        /// </summary>
        public int ChunkCountX;
        
        /// <summary>
        /// Количество чанков по Z
        /// </summary>
        public int ChunkCountZ;
        
        /// <summary>
        /// Размер чанка
        /// </summary>
        public int ChunkSize;
        
        /// <summary>
        /// Общий размер террейна по X
        /// </summary>
        public int TotalSizeX;
        
        /// <summary>
        /// Общий размер террейна по Z
        /// </summary>
        public int TotalSizeZ;
        
        /// <summary>
        /// Масштаб высот
        /// </summary>
        public float HeightScale;
        
        /// <summary>
        /// Масштаб текстур
        /// </summary>
        public float TextureScale;
        
        /// <summary>
        /// Индекс NativeArray с данными высот
        /// </summary>
        public int HeightDataArrayIndex;
        
        /// <summary>
        /// Индекс NativeArray с данными грязи
        /// </summary>
        public int MudDataArrayIndex;
        
        /// <summary>
        /// Индекс NativeArray с данными нормалей
        /// </summary>
        public int NormalDataArrayIndex;
    }
}