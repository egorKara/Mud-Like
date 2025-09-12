using Unity.Entities;

namespace MudLike.Vehicles.Components
{
    /// <summary>
    /// Ввод пользователя для управления транспортным средством
    /// </summary>
    public struct VehicleInput : IComponentData
    {
        /// <summary>
        /// Вертикальный ввод (газ/тормоз)
        /// </summary>
        public float Vertical;
        
        /// <summary>
        /// Горизонтальный ввод (рулевое управление)
        /// </summary>
        public float Horizontal;
        
        /// <summary>
        /// Положение педали газа (0-1)
        /// </summary>
        public float Throttle;
        
        /// <summary>
        /// Положение педали тормоза (0-1)
        /// </summary>
        public float Brake;
        
        /// <summary>
        /// Угол поворота руля (-1 до 1)
        /// </summary>
        public float Steering;
        
        /// <summary>
        /// Ручной тормоз
        /// </summary>
        public bool Handbrake;
        
        /// <summary>
        /// Переключение передачи вверх
        /// </summary>
        public bool GearUp;
        
        /// <summary>
        /// Переключение передачи вниз
        /// </summary>
        public bool GearDown;
        
        /// <summary>
        /// Включение/выключение двигателя
        /// </summary>
        public bool EngineToggle;
        
        /// <summary>
        /// Переключение вверх
        /// </summary>
        public bool ShiftUp;
        
        /// <summary>
        /// Переключение вниз
        /// </summary>
        public bool ShiftDown;
        
        /// <summary>
        /// Нейтральная передача
        /// </summary>
        public bool Neutral;
        
        /// <summary>
        /// Задняя передача
        /// </summary>
        public bool Reverse;
    }
}