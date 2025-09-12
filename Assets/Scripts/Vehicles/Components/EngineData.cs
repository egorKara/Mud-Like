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
        /// Текущие обороты двигателя
        /// </summary>
        public float CurrentRPM;
        
        /// <summary>
        /// Максимальные обороты двигателя
        /// </summary>
        public float MaxRPM;
        
        /// <summary>
        /// Обороты холостого хода
        /// </summary>
        public float IdleRPM;
        
        /// <summary>
        /// Минимальные обороты двигателя
        /// </summary>
        public float MinRPM;
        
        /// <summary>
        /// Крутящий момент двигателя
        /// </summary>
        public float Torque;
        
        /// <summary>
        /// Мощность двигателя
        /// </summary>
        public float Power;
        
        /// <summary>
        /// Максимальный крутящий момент
        /// </summary>
        public float MaxTorque;
        
        /// <summary>
        /// Максимальная мощность
        /// </summary>
        public float MaxPower;
        
        /// <summary>
        /// Положение дроссельной заслонки
        /// </summary>
        public float ThrottlePosition;
        
        /// <summary>
        /// Положение педали газа
        /// </summary>
        public float GasPedal;
        
        /// <summary>
        /// Целевые обороты
        /// </summary>
        public float TargetRPM;
        
        /// <summary>
        /// Скорость изменения оборотов
        /// </summary>
        public float RPMSpeed;
        
        /// <summary>
        /// Текущая мощность
        /// </summary>
        public float CurrentPower;
        
        /// <summary>
        /// Текущий крутящий момент
        /// </summary>
        public float CurrentTorque;
        
        /// <summary>
        /// Уровень топлива (0-1)
        /// </summary>
        public float FuelLevel;
        
        /// <summary>
        /// Расход топлива
        /// </summary>
        public float FuelConsumption;
        
        /// <summary>
        /// Температура двигателя
        /// </summary>
        public float Temperature;
        
        /// <summary>
        /// Максимальная температура
        /// </summary>
        public float MaxTemperature;
        
        /// <summary>
        /// Двигатель работает
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
        /// Время с момента запуска
        /// </summary>
        public float TimeSinceStart;
        
        /// <summary>
        /// Кривая мощности (x: RPM, y: Power)
        /// </summary>
        public float2 PowerCurve;
        
        /// <summary>
        /// Кривая крутящего момента (x: RPM, y: Torque)
        /// </summary>
        public float2 TorqueCurve;
    }
}