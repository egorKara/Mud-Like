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
        /// Максимальное время жизни
        /// </summary>
        public float MaxLifetime;
        
        /// <summary>
        /// Цвет частицы
        /// </summary>
        public float4 Color;
        
        /// <summary>
        /// Масса частицы
        /// </summary>
        public float Mass;
        
        /// <summary>
        /// Вязкость частицы
        /// </summary>
        public float Viscosity;
        
        /// <summary>
        /// Активна ли частица
        /// </summary>
        public bool IsActive;
    }
}