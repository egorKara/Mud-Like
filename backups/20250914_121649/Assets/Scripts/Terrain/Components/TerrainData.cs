using Unity.Entities;
using Unity.Mathematics;

namespace MudLike.Terrain.Components
{
    /// <summary>
    /// Данные террейна
    /// </summary>
    public struct TerrainData : IComponentData
    {
        /// <summary>
        /// Высота в точке
        /// </summary>
        public float Height;
        
        /// <summary>
        /// Нормаль поверхности
        /// </summary>
        public float3 Normal;
        
        /// <summary>
        /// Текстура поверхности
        /// </summary>
        public int TextureIndex;
        
        /// <summary>
        /// Жесткость поверхности
        /// </summary>
        public float Hardness;
        
        /// <summary>
        /// Коэффициент трения
        /// </summary>
        public float Friction;
    }
}