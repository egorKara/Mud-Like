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
        /// Текущая передача
        /// </summary>
        public int CurrentGear;
        
        /// <summary>
        /// Целевая передача
        /// </summary>
        public int TargetGear;
        
        /// <summary>
        /// Минимальная передача
        /// </summary>
        public int MinGear;
        
        /// <summary>
        /// Максимальная передача
        /// </summary>
        public int MaxGear;
        
        /// <summary>
        /// Нейтральная передача
        /// </summary>
        public int NeutralGear;
        
        /// <summary>
        /// Задняя передача
        /// </summary>
        public int ReverseGear;
        
        /// <summary>
        /// Передаточные числа (x, y, z, w для 4 передач)
        /// </summary>
        public float4 GearRatios;
        
        /// <summary>
        /// Главная передача
        /// </summary>
        public float FinalDriveRatio;
        
        /// <summary>
        /// КПД трансмиссии
        /// </summary>
        public float Efficiency;
        
        /// <summary>
        /// Выходной крутящий момент
        /// </summary>
        public float OutputTorque;
        
        /// <summary>
        /// Выходная мощность
        /// </summary>
        public float OutputPower;
        
        /// <summary>
        /// Происходит переключение передачи
        /// </summary>
        public bool IsShifting;
        
        /// <summary>
        /// Текущее время переключения
        /// </summary>
        public float CurrentShiftTime;
        
        /// <summary>
        /// Время переключения передачи
        /// </summary>
        public float ShiftTime;
        
        /// <summary>
        /// Обороты для переключения вверх
        /// </summary>
        public float UpshiftRPM;
        
        /// <summary>
        /// Обороты для переключения вниз
        /// </summary>
        public float DownshiftRPM;
    }
}