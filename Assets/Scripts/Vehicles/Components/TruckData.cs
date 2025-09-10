using Unity.Entities;
using Unity.Mathematics;

namespace MudLike.Vehicles.Components
{
    /// <summary>
    /// Основные данные грузовика КРАЗ
    /// </summary>
    public struct TruckData : IComponentData
    {
        /// <summary>
        /// Масса грузовика в кг
        /// </summary>
        public float Mass;
        
        /// <summary>
        /// Мощность двигателя в л.с.
        /// </summary>
        public float EnginePower;
        
        /// <summary>
        /// Максимальный крутящий момент в Н⋅м
        /// </summary>
        public float MaxTorque;
        
        /// <summary>
        /// Текущие обороты двигателя (RPM)
        /// </summary>
        public float EngineRPM;
        
        /// <summary>
        /// Текущая передача (1-6)
        /// </summary>
        public int CurrentGear;
        
        /// <summary>
        /// Максимальная скорость в км/ч
        /// </summary>
        public float MaxSpeed;
        
        /// <summary>
        /// Текущая скорость в км/ч
        /// </summary>
        public float CurrentSpeed;
        
        /// <summary>
        /// Угол поворота руля в градусах
        /// </summary>
        public float SteeringAngle;
        
        /// <summary>
        /// Максимальный угол поворота руля
        /// </summary>
        public float MaxSteeringAngle;
        
        /// <summary>
        /// Коэффициент сцепления с дорогой
        /// </summary>
        public float TractionCoefficient;
        
        /// <summary>
        /// Уровень топлива (0-1)
        /// </summary>
        public float FuelLevel;
        
        /// <summary>
        /// Состояние двигателя (включен/выключен)
        /// </summary>
        public bool EngineRunning;
        
        /// <summary>
        /// Состояние ручного тормоза
        /// </summary>
        public bool HandbrakeOn;
        
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