using Unity.Entities;
using Unity.Mathematics;

namespace MudLike.Terrain.Components
{
    /// <summary>
    /// Данные деформации террейна
    /// </summary>
    public struct DeformationData : IComponentData
    {
        /// <summary>
        /// Позиция деформации
        /// </summary>
        public float3 Position;
        
        /// <summary>
        /// Радиус деформации
        /// </summary>
        public float Radius;
        
        /// <summary>
        /// Глубина деформации
        /// </summary>
        public float Depth;
        
        /// <summary>
        /// Сила деформации
        /// </summary>
        public float Force;
        
        /// <summary>
        /// Тип деформации
        /// </summary>
        public DeformationType Type;
        
        /// <summary>
        /// Время деформации
        /// </summary>
        public float Time;
        
        /// <summary>
        /// Деформация активна
        /// </summary>
        public bool IsActive;
        
        /// <summary>
        /// Деформация применена
        /// </summary>
        public bool IsApplied;
    }
    
    /// <summary>
    /// Тип деформации
    /// </summary>
    public enum DeformationType
    {
        /// <summary>
        /// Вдавливание (от колес)
        /// </summary>
        Indentation,
        
        /// <summary>
        /// Выдавливание (от взрывов)
        /// </summary>
        Explosion,
        
        /// <summary>
        /// Сглаживание
        /// </summary>
        Smoothing,
        
        /// <summary>
        /// Восстановление
        /// </summary>
        Restoration
    }
}