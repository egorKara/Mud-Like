using Unity.Entities;
using Unity.Mathematics;

namespace MudLike.Vehicles.Components
{
    /// <summary>
    /// Данные двигателя транспортного средства
    /// </summary>
    public struct EngineData : IComponentData
    {
        /// <summary>
        /// Максимальная мощность двигателя
        /// </summary>
        public float MaxPower;
        
        /// <summary>
        /// Максимальный крутящий момент двигателя
        /// </summary>
        public float MaxTorque;
        
        /// <summary>
        /// Обороты максимальной мощности
        /// </summary>
        public float PowerRPM;
        
        /// <summary>
        /// Обороты максимального крутящего момента
        /// </summary>
        public float TorqueRPM;
        
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
        /// Текущая мощность двигателя
        /// </summary>
        public float CurrentPower;
        
        /// <summary>
        /// Текущий крутящий момент двигателя
        /// </summary>
        public float CurrentTorque;
        
        /// <summary>
        /// Положение дроссельной заслонки (0-1)
        /// </summary>
        public float ThrottlePosition;
        
        /// <summary>
        /// Положение педали газа (0-1)
        /// </summary>
        public float GasPedal;
        
        /// <summary>
        /// Двигатель включен
        /// </summary>
        public bool IsRunning;
        
        /// <summary>
        /// Двигатель запускается
        /// </summary>
        public bool IsStarting;
        
        /// <summary>
        /// Двигатель заглох
        /// </summary>
        public bool IsStalled;
        
        /// <summary>
        /// Время работы двигателя
        /// </summary>
        public float RunningTime;
        
        /// <summary>
        /// Время с последнего запуска
        /// </summary>
        public float TimeSinceStart;
        
        /// <summary>
        /// Температура двигателя
        /// </summary>
        public float Temperature;
        
        /// <summary>
        /// Максимальная температура двигателя
        /// </summary>
        public float MaxTemperature;
        
        /// <summary>
        /// Уровень масла
        /// </summary>
        public float OilLevel;
        
        /// <summary>
        /// Уровень топлива
        /// </summary>
        public float FuelLevel;
        
        /// <summary>
        /// Максимальный уровень топлива
        /// </summary>
        public float MaxFuelLevel;
        
        /// <summary>
        /// Расход топлива
        /// </summary>
        public float FuelConsumption;
        
        /// <summary>
        /// Эффективность двигателя
        /// </summary>
        public float Efficiency;
        
        /// <summary>
        /// Кривая мощности двигателя
        /// </summary>
        public float4 PowerCurve;
        
        /// <summary>
        /// Кривая крутящего момента двигателя
        /// </summary>
        public float4 TorqueCurve;
    }
}