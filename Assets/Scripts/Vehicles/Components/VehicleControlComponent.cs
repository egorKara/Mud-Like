using Unity.Entities;
using Unity.Mathematics;

namespace MudLike.Vehicles.Components
{
    /// <summary>
    /// ECS компонент для управления транспортным средством
    /// </summary>
    public struct VehicleControlComponent : IComponentData
    {
        /// <summary>
        /// Входное значение газа (0-1)
        /// </summary>
        public float throttleInput;
        
        /// <summary>
        /// Входное значение тормоза (0-1)
        /// </summary>
        public float brakeInput;
        
        /// <summary>
        /// Входное значение рулевого управления (-1 до 1)
        /// </summary>
        public float steerInput;
        
        /// <summary>
        /// Входное значение ручного тормоза (0-1)
        /// </summary>
        public float handbrakeInput;
        
        /// <summary>
        /// Включен ли полный привод
        /// </summary>
        public bool fourWheelDriveInput;
        
        /// <summary>
        /// Включена ли блокировка дифференциала
        /// </summary>
        public bool differentialLockInput;
        
        /// <summary>
        /// Переключение передачи вверх
        /// </summary>
        public bool gearUpInput;
        
        /// <summary>
        /// Переключение передачи вниз
        /// </summary>
        public bool gearDownInput;
        
        /// <summary>
        /// Включение/выключение двигателя
        /// </summary>
        public bool engineToggleInput;
        
        /// <summary>
        /// Время последнего обновления ввода
        /// </summary>
        public float lastInputTime;
        
        /// <summary>
        /// Флаг: активно ли управление
        /// </summary>
        public bool isControlActive;
    }
}
