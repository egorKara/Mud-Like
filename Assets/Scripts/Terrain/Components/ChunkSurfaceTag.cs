using Unity.Entities;

namespace MudLike.Terrain.Components
{
    /// <summary>
    /// Тег для поверхностей чанка
    /// </summary>
    public struct ChunkSurfaceTag : IComponentData
    {
        /// <summary>
        /// Сущность чанка
        /// </summary>
        public Entity ChunkEntity;
        
        /// <summary>
        /// Локальная координата X
        /// </summary>
        public int LocalX;
        
        /// <summary>
        /// Локальная координата Z
        /// </summary>
        public int LocalZ;
    }
}