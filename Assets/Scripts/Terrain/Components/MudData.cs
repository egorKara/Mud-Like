using Unity.Entities;
using Unity.Mathematics;

namespace MudLike.Terrain.Components
{
    /// <summary>
    /// Данные грязи
    /// </summary>
    public struct MudData : IComponentData
    {
        /// <summary>
        /// Позиция грязи
        /// </summary>
        public float3 Position;
        
        /// <summary>
        /// Радиус грязи
        /// </summary>
        public float Radius;
        
        /// <summary>
        /// Уровень грязи (0-1)
        /// </summary>
        public float Level;
        
        /// <summary>
        /// Вязкость грязи
        /// </summary>
        public float Viscosity;
        
        /// <summary>
        /// Плотность грязи
        /// </summary>
        public float Density;
        
        /// <summary>
        /// Сопротивление грязи
        /// </summary>
        public float Resistance;
        
        /// <summary>
        /// Грязь активна
        /// </summary>
        public bool IsActive;
        
        /// <summary>
        /// Грязь требует обновления
        /// </summary>
        public bool NeedsUpdate;
    }
}