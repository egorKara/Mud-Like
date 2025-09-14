using Unity.Entities;
using Unity.Mathematics;

namespace MudLike.Vehicles.Components
{
    /// <summary>
    /// Конфигурация транспортного средства
    /// </summary>
    public struct VehicleConfig : IComponentData
    {
        /// <summary>
        /// Максимальная скорость транспорта
        /// </summary>
        public float MaxSpeed;
        
        /// <summary>
        /// Ускорение транспорта
        /// </summary>
        public float Acceleration;
        
        /// <summary>
        /// Скорость поворота транспорта
        /// </summary>
        public float TurnSpeed;
        
        /// <summary>
        /// Масса транспорта
        /// </summary>
        public float Mass;
        
        /// <summary>
        /// Сопротивление транспорта
        /// </summary>
        public float Drag;
        
        /// <summary>
        /// Угловое сопротивление транспорта
        /// </summary>
        public float AngularDrag;
        
        /// <summary>
        /// Радиус поворота транспорта
        /// </summary>
        public float TurnRadius;
        
        /// <summary>
        /// Высота центра масс
        /// </summary>
        public float CenterOfMassHeight;
    }
}