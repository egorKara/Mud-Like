using Unity.Entities;
using Unity.Mathematics;

namespace MudLike.Core.Components
{
    /// <summary>
    /// Компонент ввода игрока для управления транспортом
    /// Игрок управляет только транспортом, не ходит пешком
    /// </summary>
    public struct PlayerInput : IComponentData
    {
        /// <summary>
        /// Движение транспорта (WASD)
        /// </summary>
        public float2 VehicleMovement;
        
        /// <summary>
        /// Ускорение транспорта
        /// </summary>
        public bool Accelerate;
        
        /// <summary>
        /// Торможение транспорта
        /// </summary>
        public bool Brake;
        
        /// <summary>
        /// Ручной тормоз
        /// </summary>
        public bool Handbrake;
        
        /// <summary>
        /// Управление рулем
        /// </summary>
        public float Steering;
        
        /// <summary>
        /// Дополнительные действия (лебедка, переключение камеры и т.д.)
        /// </summary>
        public bool Action1; // E - лебедка
        public bool Action2; // Tab - камера
        public bool Action3; // F - полный привод
        public bool Action4; // G - блокировка дифференциала
        
        /// <summary>
        /// Управление камерой
        /// </summary>
        public float2 CameraLook;    // Поворот камеры мышью
        public float CameraZoom;     // Зум камеры колесиком мыши
        
        /// <summary>
        /// Дополнительные функции транспорта
        /// </summary>
        public bool EngineToggle;    // Включение/выключение двигателя
        public bool ShiftUp;         // Переключение передачи вверх
        public bool ShiftDown;       // Переключение передачи вниз
        public bool Neutral;         // Нейтральная передача
    }
}
