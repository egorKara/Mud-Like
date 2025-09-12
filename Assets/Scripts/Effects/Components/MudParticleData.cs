using Unity.Entities;
using Unity.Mathematics;

namespace MudLike.Effects.Components
{
    /// <summary>
    /// Данные частицы грязи
    /// </summary>
    public struct MudParticleData : IComponentData
    {
        /// <summary>
        /// Позиция частицы
        /// </summary>
        public float3 Position;
        
        /// <summary>
        /// Скорость частицы
        /// </summary>
        public float3 Velocity;
        
        /// <summary>
        /// Размер частицы
        /// </summary>
        public float Size;
        
        /// <summary>
        /// Время жизни частицы
        /// </summary>
        public float Lifetime;
        
        /// <summary>
        /// Масса частицы
        /// </summary>
        public float Mass;
        
        /// <summary>
        /// Температура частицы
        /// </summary>
        public float Temperature;
        
        /// <summary>
        /// Вязкость частицы
        /// </summary>
        public float Viscosity;
        
        /// <summary>
        /// Частица активна
        /// </summary>
        public bool IsActive;
    }
}