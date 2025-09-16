using Unity.Entities;
using Unity.Mathematics;

namespace MudLike.Vehicles.Components
{
    /// <summary>
    /// Компонент ввода управления транспортом
    /// Игрок управляет только транспортом, не ходит пешком
    /// </summary>
    public struct VehicleInput : IComponentData
    {
        /// <summary>
        /// Ускорение (W/Up)
        /// </summary>
        public bool Accelerate;
        
        /// <summary>
        /// Торможение (S/Down)
        /// </summary>
        public bool Brake;
        
        /// <summary>
        /// Ручной тормоз (Space)
        /// </summary>
        public bool Handbrake;
        
        /// <summary>
        /// Управление рулем (-1.0 до 1.0, A/D или Left/Right)
        /// </summary>
        public float Steering;
        
        /// <summary>
        /// Переключение передач (Shift/Ctrl)
        /// </summary>
        public bool ShiftUp;
        public bool ShiftDown;
        
        /// <summary>
        /// Включение/выключение полного привода (F)
        /// </summary>
        public bool Toggle4WD;
        
        /// <summary>
        /// Включение/выключение блокировки дифференциала (G)
        /// </summary>
        public bool ToggleDiffLock;
        
        /// <summary>
        /// Использование лебедки (E)
        /// </summary>
        public bool UseWinch;
        
        /// <summary>
        /// Переключение камеры (Tab)
        /// </summary>
        public bool SwitchCamera;
    }
}