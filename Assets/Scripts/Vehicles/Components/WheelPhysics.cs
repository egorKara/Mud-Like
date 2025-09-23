using Unity.Entities;
using Unity.Mathematics;

namespace MudLike.Vehicles.Components
{
    /// <summary>
    /// Расширенные данные физики колеса для взаимодействия с грязью
    /// </summary>
    public struct WheelPhysics : IComponentData
    {
        /// <summary>
        /// Скорость проскальзывания (0-1)
        /// </summary>
        public float SlipRatio;
        
        /// <summary>
        /// Угол проскальзывания (радианы)
        /// </summary>
        public float SlipAngle;
        
        /// <summary>
        /// Коэффициент сцепления с текущей поверхностью
        /// </summary>
        public float SurfaceTraction;
        
        /// <summary>
        /// Глубина погружения в поверхность
        /// </summary>
        public float SinkDepth;
        
        /// <summary>
        /// Сила сопротивления качению
        /// </summary>
        public float RollingResistance;
        
        /// <summary>
        /// Сила сопротивления вязкости
        /// </summary>
        public float ViscousResistance;
        
        /// <summary>
        /// Сила выталкивания (Архимед)
        /// </summary>
        public float BuoyancyForce;
        
        /// <summary>
        /// Момент сопротивления повороту
        /// </summary>
        public float SteeringResistance;
        
        /// <summary>
        /// Температура колеса (влияет на сцепление)
        /// </summary>
        public float WheelTemperature;
        
        /// <summary>
        /// Износ протектора (0-1)
        /// </summary>
        public float TreadWear;
        
        /// <summary>
        /// Давление в шине (кПа)
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
        /// Скорость нагрева колеса
        /// </summary>
        public float HeatingRate;
        
        /// <summary>
        /// Скорость охлаждения колеса
        /// </summary>
        public float CoolingRate;
        
        /// <summary>
        /// Время в контакте с текущей поверхностью
        /// </summary>
        public float ContactTime;
        
        /// <summary>
        /// Последний тип поверхности
        /// </summary>
        public int LastSurfaceType;
        
        /// <summary>
        /// Количество частиц грязи на колесе
        /// </summary>
        public int MudParticleCount;
        
        /// <summary>
        /// Масса грязи на колесе (кг)
        /// </summary>
        public float MudMass;
        
        /// <summary>
        /// Скорость очистки от грязи
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
        /// Сила бокового сцепления
        /// </summary>
        public float LateralTractionForce;
        
        /// <summary>
        /// Сила продольного сцепления
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
        /// Энергия проскальзывания (для нагрева)
        /// </summary>
        public float SlipEnergy;
        
        /// <summary>
        /// Время последнего обновления
        /// </summary>
        public float LastUpdateTime;
        
        /// <summary>
        /// Требует ли обновления
        /// </summary>
        public bool NeedsUpdate;
    }
}