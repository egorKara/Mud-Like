using Unity.Entities;
using Unity.Mathematics;

namespace MudLike.Vehicles.Components
{
    /// <summary>
    /// Управление грузовиком
    /// </summary>
    public struct TruckControl : IComponentData
    {
        /// <summary>
        /// Газ (0-1)
        /// </summary>
        public float Throttle;
        
        /// <summary>
        /// Тормоз (0-1)
        /// </summary>
        public float Brake;
        
        /// <summary>
        /// Руль (-1 до 1, -1 = лево, 1 = право)
        /// </summary>
        public float Steering;
        
        /// <summary>
        /// Ручной тормоз
        /// </summary>
        public bool Handbrake;
        
        /// <summary>
        /// Переключение передачи вверх
        /// </summary>
        public bool ShiftUp;
        
        /// <summary>
        /// Переключение передачи вниз
        /// </summary>
        public bool ShiftDown;
        
        /// <summary>
        /// Запуск/остановка двигателя
        /// </summary>
        public bool ToggleEngine;
        
        /// <summary>
        /// Сцепление (для ручной коробки)
        /// </summary>
        public float Clutch;
        
        /// <summary>
        /// Режим полного привода
        /// </summary>
        public bool FourWheelDrive;
        
        /// <summary>
        /// Блокировка переднего дифференциала
        /// </summary>
        public bool LockFrontDifferential;
        
        /// <summary>
        /// Блокировка среднего дифференциала
        /// </summary>
        public bool LockMiddleDifferential;
        
        /// <summary>
        /// Блокировка заднего дифференциала
        /// </summary>
        public bool LockRearDifferential;
        
        /// <summary>
        /// Блокировка межосевого дифференциала
        /// </summary>
        public bool LockCenterDifferential;
    }
}