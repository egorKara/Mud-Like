using Unity.Entities;
using Unity.Mathematics;

namespace MudLike.Vehicles.Components
{
    /// <summary>
    /// Расширенные физические данные колеса
    /// </summary>
    public struct WheelPhysicsData : IComponentData
    {
        /// <summary>
        /// Коэффициент проскальзывания
        /// </summary>
        public float SlipRatio;
        
        /// <summary>
        /// Угол проскальзывания
        /// </summary>
        public float SlipAngle;
        
        /// <summary>
        /// Сцепление с поверхностью
        /// </summary>
        public float SurfaceTraction;
        
        /// <summary>
        /// Глубина погружения в поверхность
        /// </summary>
        public float SinkDepth;
        
        /// <summary>
        /// Сопротивление качению
        /// </summary>
        public float RollingResistance;
        
        /// <summary>
        /// Вязкое сопротивление
        /// </summary>
        public float ViscousResistance;
        
        /// <summary>
        /// Сила плавучести
        /// </summary>
        public float BuoyancyForce;
        
        /// <summary>
        /// Сопротивление повороту
        /// </summary>
        public float SteeringResistance;
        
        /// <summary>
        /// Температура колеса
        /// </summary>
        public float WheelTemperature;
        
        /// <summary>
        /// Износ протектора
        /// </summary>
        public float TreadWear;
        
        /// <summary>
        /// Давление в шине
        /// </summary>
        public float TirePressure;
        
        /// <summary>
        /// Максимальное давление в шине
        /// </summary>
        public float MaxTirePressure;
        
        /// <summary>
        /// Минимальное давление в шине
        /// </summary>
        public float MinTirePressure;
        
        /// <summary>
        /// Скорость нагрева
        /// </summary>
        public float HeatingRate;
        
        /// <summary>
        /// Скорость охлаждения
        /// </summary>
        public float CoolingRate;
        
        /// <summary>
        /// Время контакта с поверхностью
        /// </summary>
        public float ContactTime;
        
        /// <summary>
        /// Последний тип поверхности
        /// </summary>
        public int LastSurfaceType;
        
        /// <summary>
        /// Количество частиц грязи
        /// </summary>
        public int MudParticleCount;
        
        /// <summary>
        /// Масса грязи на колесе
        /// </summary>
        public float MudMass;
        
        /// <summary>
        /// Скорость очистки
        /// </summary>
        public float CleaningRate;
        
        /// <summary>
        /// Критическая скорость проскальзывания
        /// </summary>
        public float CriticalSlipSpeed;
        
        /// <summary>
        /// Максимальная сила сцепления
        /// </summary>
        public float MaxTractionForce;
        
        /// <summary>
        /// Текущая сила сцепления
        /// </summary>
        public float CurrentTractionForce;
        
        /// <summary>
        /// Боковая сила сцепления
        /// </summary>
        public float LateralTractionForce;
        
        /// <summary>
        /// Продольная сила сцепления
        /// </summary>
        public float LongitudinalTractionForce;
        
        /// <summary>
        /// Угловая скорость проскальзывания
        /// </summary>
        public float SlipAngularVelocity;
        
        /// <summary>
        /// Линейная скорость проскальзывания
        /// </summary>
        public float SlipLinearVelocity;
        
        /// <summary>
        /// Направление проскальзывания
        /// </summary>
        public float3 SlipDirection;
        
        /// <summary>
        /// Энергия проскальзывания
        /// </summary>
        public float SlipEnergy;
        
        /// <summary>
        /// Время последнего обновления
        /// </summary>
        public float LastUpdateTime;
        
        /// <summary>
        /// Требует обновления
        /// </summary>
        public bool NeedsUpdate;
    }
}