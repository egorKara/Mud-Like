using Unity.Entities;
using Unity.Mathematics;

namespace MudLike.Core.Components
{
    /// <summary>
    /// Конфигурация транспортного средства
    /// </summary>
    public struct VehicleConfig : IComponentData
    {
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
        public float BrakingForce;
        
        /// <summary>
        /// Поворот
        /// </summary>
        public float TurnSpeed;
        
        /// <summary>
        /// Масса
        /// </summary>
        public float Mass;
    }
}
