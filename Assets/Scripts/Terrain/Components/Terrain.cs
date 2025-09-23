using Unity.Entities;
using Unity.Mathematics;

namespace MudLike.Terrain.Components
{
    /// <summary>
    /// Данные террейна
    /// </summary>
    public struct Terrain : IComponentData
    {
        /// <summary>
        /// Размер террейна
        /// </summary>
        public float3 Size;
        
        /// <summary>
        /// Разрешение высотной карты
        /// </summary>
        public int HeightmapResolution;
        
        /// <summary>
        /// Размер чанка
        /// </summary>
        public float ChunkSize;
        
        /// <summary>
        /// Количество чанков по X
        /// </summary>
        public int ChunkCountX;
        
        /// <summary>
        /// Количество чанков по Z
        /// </summary>
        public int ChunkCountZ;
        
        /// <summary>
        /// Масштаб высот
        /// </summary>
        public float HeightScale;
        
        /// <summary>
        /// Базовая высота
        /// </summary>
        public float BaseHeight;
        
        /// <summary>
        /// Террейн активен
        /// </summary>
        public bool IsActive;
        
        /// <summary>
        /// Террейн требует обновления
        /// </summary>
        public bool NeedsUpdate;
    }
}