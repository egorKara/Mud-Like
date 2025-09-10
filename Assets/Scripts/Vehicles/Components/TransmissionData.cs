using Unity.Entities;
using Unity.Mathematics;

namespace MudLike.Vehicles.Components
{
    /// <summary>
    /// Данные трансмиссии грузовика
    /// </summary>
    public struct TransmissionData : IComponentData
    {
        /// <summary>
        /// Передаточные числа коробки передач
        /// </summary>
        public float4x4 GearRatios; // 4x4 матрица для 6 передач + задняя
        
        /// <summary>
        /// Текущая передача (1-6, 0 = нейтраль, -1 = задняя)
        /// </summary>
        public int CurrentGear;
        
        /// <summary>
        /// Передаточное число главной передачи
        /// </summary>
        public float FinalDriveRatio;
        
        /// <summary>
        /// Передаточное число дифференциала
        /// </summary>
        public float DifferentialRatio;
        
        /// <summary>
        /// Эффективность трансмиссии (0-1)
        /// </summary>
        public float Efficiency;
        
        /// <summary>
        /// Время переключения передачи в секундах
        /// </summary>
        public float ShiftTime;
        
        /// <summary>
        /// Таймер переключения передачи
        /// </summary>
        public float ShiftTimer;
        
        /// <summary>
        /// Автоматическое переключение передач
        /// </summary>
        public bool AutomaticTransmission;
        
        /// <summary>
        /// Минимальные обороты для переключения вверх
        /// </summary>
        public float UpshiftRPM;
        
        /// <summary>
        /// Максимальные обороты для переключения вниз
        /// </summary>
        public float DownshiftRPM;
    }
}