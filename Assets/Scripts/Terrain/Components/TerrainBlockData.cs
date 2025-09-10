using Unity.Entities;
using Unity.Mathematics;

namespace MudLike.Terrain.Components
{
    /// <summary>
    /// Данные блока террейна
    /// </summary>
    public struct TerrainBlockData : IComponentData
    {
        /// <summary>
        /// Позиция блока в сетке
        /// </summary>
        public int2 GridPosition;
        
        /// <summary>
        /// Размер блока в метрах
        /// </summary>
        public float BlockSize;
        
        /// <summary>
        /// Разрешение высоты блока
        /// </summary>
        public int2 HeightResolution;
        
        /// <summary>
        /// Минимальная высота блока
        /// </summary>
        public float MinHeight;
        
        /// <summary>
        /// Максимальная высота блока
        /// </summary>
        public float MaxHeight;
        
        /// <summary>
        /// Флаг активности блока
        /// </summary>
        public bool IsActive;
        
        /// <summary>
        /// Время последнего обновления
        /// </summary>
        public float LastUpdateTime;
    }
}