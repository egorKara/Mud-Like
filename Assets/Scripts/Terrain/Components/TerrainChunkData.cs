using Unity.Entities;
using Unity.Mathematics;

namespace MudLike.Terrain.Components
{
    /// <summary>
    /// Данные чанка террейна
    /// </summary>
    public struct TerrainChunkData : IComponentData
    {
        /// <summary>
        /// Координата чанка
        /// </summary>
        public int2 ChunkCoordinate;
        
        /// <summary>
        /// Размер чанка
        /// </summary>
        public int Size;
        
        /// <summary>
        /// Разрешение чанка
        /// </summary>
        public float Resolution;
        
        /// <summary>
        /// Чанк загружен
        /// </summary>
        public bool IsLoaded;
        
        /// <summary>
        /// Требует обновления
        /// </summary>
        public bool NeedsUpdate;
        
        /// <summary>
        /// Время последнего обновления
        /// </summary>
        public float LastUpdateTime;
    }
}