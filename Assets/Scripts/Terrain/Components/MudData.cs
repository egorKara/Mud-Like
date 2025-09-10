using Unity.Entities;
using Unity.Mathematics;

namespace MudLike.Terrain.Components
{
    /// <summary>
    /// Данные грязи для блока террейна
    /// </summary>
    public struct MudData : IComponentData
    {
        /// <summary>
        /// Высота грязи в блоке
        /// </summary>
        public float Height;
        
        /// <summary>
        /// Коэффициент сцепления с грязью
        /// </summary>
        public float TractionModifier;
        
        /// <summary>
        /// Вязкость грязи
        /// </summary>
        public float Viscosity;
        
        /// <summary>
        /// Плотность грязи
        /// </summary>
        public float Density;
        
        /// <summary>
        /// Влажность грязи (0-1)
        /// </summary>
        public float Moisture;
        
        /// <summary>
        /// Время последнего обновления
        /// </summary>
        public float LastUpdateTime;
        
        /// <summary>
        /// Флаг изменения блока
        /// </summary>
        public bool IsDirty;
    }
}