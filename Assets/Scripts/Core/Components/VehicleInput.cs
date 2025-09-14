using Unity.Entities;
using Unity.Mathematics;

namespace MudLike.Core.Components
{
    /// <summary>
    /// Компонент ввода для транспорта
    /// </summary>
    public struct VehicleInput : IComponentData
    {
        /// <summary>
        /// Ускорение (газ)
        /// </summary>
        public bool Accelerate;
        
        /// <summary>
        /// Торможение
        /// </summary>
        public bool Brake;
        
        /// <summary>
        /// Ручной тормоз
        /// </summary>
        public bool Handbrake;
        
        /// <summary>
        /// Поворот руля (-1 до 1)
        /// </summary>
        public float Steering;
        
        /// <summary>
        /// Скорость поворота
        /// </summary>
        public float RotationSpeed;
        
        /// <summary>
        /// Движение (для совместимости)
        /// </summary>
        public float3 Movement;
        
        /// <summary>
        /// Поворот (для совместимости)
        /// </summary>
        public float Rotation;
    }
}
