using Unity.Entities;
using Unity.Mathematics;

namespace MudLike.Core.Components
{
    /// <summary>
    /// Компонент скорости движения игрока
    /// </summary>
    public struct MovementSpeed : IComponentData
    {
        /// <summary>
        /// Скорость движения в единицах в секунду
        /// </summary>
        public float Value;
        
        /// <summary>
        /// Максимальная скорость
        /// </summary>
        public float MaxSpeed;
        
        /// <summary>
        /// Ускорение
        /// </summary>
        public float Acceleration;
        
        /// <summary>
        /// Торможение
        /// </summary>
        public float Deceleration;
    }
}
