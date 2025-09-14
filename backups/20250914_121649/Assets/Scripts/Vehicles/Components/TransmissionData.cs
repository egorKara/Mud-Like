using Unity.Entities;
using Unity.Mathematics;

namespace MudLike.Vehicles.Components
{
    /// <summary>
    /// Данные трансмиссии транспортного средства
    /// </summary>
    public struct TransmissionData : IComponentData
    {
        /// <summary>
        /// Передаточные числа коробки передач
        /// </summary>
        public float4 GearRatios;
        
        /// <summary>
        /// Передаточное число главной передачи
        /// </summary>
        public float FinalDriveRatio;
        
        /// <summary>
        /// Передаточное число дифференциала
        /// </summary>
        public float DifferentialRatio;
        
        /// <summary>
        /// Текущая передача
        /// </summary>
        public int CurrentGear;
        
        /// <summary>
        /// Минимальная передача
        /// </summary>
        public int MinGear;
        
        /// <summary>
        /// Максимальная передача
        /// </summary>
        public int MaxGear;
        
        /// <summary>
        /// Передача заднего хода
        /// </summary>
        public int ReverseGear;
        
        /// <summary>
        /// Нейтральная передача
        /// </summary>
        public int NeutralGear;
        
        /// <summary>
        /// Время переключения передачи
        /// </summary>
        public float ShiftTime;
        
        /// <summary>
        /// Текущее время переключения
        /// </summary>
        public float CurrentShiftTime;
        
        /// <summary>
        /// Переключение передачи в процессе
        /// </summary>
        public bool IsShifting;
        
        /// <summary>
        /// Целевая передача для переключения
        /// </summary>
        public int TargetGear;
        
        /// <summary>
        /// Обороты двигателя для переключения вверх
        /// </summary>
        public float UpshiftRPM;
        
        /// <summary>
        /// Обороты двигателя для переключения вниз
        /// </summary>
        public float DownshiftRPM;
        
        /// <summary>
        /// Минимальные обороты двигателя
        /// </summary>
        public float MinRPM;
        
        /// <summary>
        /// Максимальные обороты двигателя
        /// </summary>
        public float MaxRPM;
        
        /// <summary>
        /// Обороты холостого хода
        /// </summary>
        public float IdleRPM;
        
        /// <summary>
        /// Текущие обороты двигателя
        /// </summary>
        public float CurrentRPM;
        
        /// <summary>
        /// Целевые обороты двигателя
        /// </summary>
        public float TargetRPM;
        
        /// <summary>
        /// Скорость изменения оборотов
        /// </summary>
        public float RPMSpeed;
        
        /// <summary>
        /// Крутящий момент на выходе трансмиссии
        /// </summary>
        public float OutputTorque;
        
        /// <summary>
        /// Мощность на выходе трансмиссии
        /// </summary>
        public float OutputPower;
        
        /// <summary>
        /// Эффективность трансмиссии
        /// </summary>
        public float Efficiency;
    }
}