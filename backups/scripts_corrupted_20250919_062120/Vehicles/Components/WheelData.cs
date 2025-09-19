using Unity.Entities;
using Unity.Mathematics;

namespace MudLike.Vehicles.Components
{
    /// <summary>
    /// Компонент данных колеса транспортного средства
    /// Содержит физические параметры и состояние колеса
    /// </summary>
    public struct WheelData : IComponentData
    {
        // Геометрические параметры
        public float3 Position;           // Позиция колеса относительно центра транспорта
        public float Radius;              // Радиус колеса (м)
        public float Width;               // Ширина колеса (м)
        public float Mass;                // Масса колеса (кг)
        
        // Параметры подвески
        public float SuspensionLength;    // Длина подвески (м)
        public float SuspensionStiffness; // Жесткость подвески (Н/м)
        public float SuspensionDamping;   // Демпфирование подвески (Н⋅с/м)
        public float SuspensionCompression; // Сжатие подвески (0-1)
        
        // Параметры трения
        public float Friction;            // Коэффициент трения
        public float LateralFriction;     // Боковое трение
        public float LongitudinalFriction; // Продольное трение
        public float RollingResistance;   // Сопротивление качению
        
        // Параметры движения
        public float AngularVelocity;     // Угловая скорость (рад/с)
        public float Torque;              // Крутящий момент (Н⋅м)
        public float BrakeTorque;         // Тормозной момент (Н⋅м)
        public float SteeringAngle;       // Угол поворота (радианы)
        
        // Параметры контакта с поверхностью
        public bool IsGrounded;           // Касается ли земли
        public float3 ContactPoint;       // Точка контакта
        public float3 ContactNormal;      // Нормаль контакта
        public float ContactDistance;     // Расстояние до контакта
        public float GroundFriction;      // Трение о землю
        
        // Параметры деформации
        public float DeformationForce;    // Сила деформации террейна
        public float DeformationRadius;   // Радиус деформации
        public float MudLevel;            // Уровень грязи (0-1)
        public float TractionModifier;    // Модификатор тяги (0-1)
        
        // Параметры износа
        public float Wear;                // Износ шины (0-1)
        public float Temperature;         // Температура шины (°C)
        public float Pressure;            // Давление в шине (кПа)
        public float OptimalPressure;     // Оптимальное давление (кПа)
        
        // Параметры производительности
        public bool IsActive;             // Активно ли колесо
        public bool NeedsUpdate;          // Требует ли обновления
        public float LastUpdateTime;      // Время последнего обновления
        public float UpdateFrequency;     // Частота обновления (Гц)
        
        // Конструктор с значениями по умолчанию
        public static WheelData Default => new WheelData
        {
            Position = new float3(0f, 0f, 0f),
            Radius = 0.4f,                // 40 см
            Width = 0.25f,                // 25 см
            Mass = 25f,                   // 25 кг
            SuspensionLength = 0.3f,      // 30 см
            SuspensionStiffness = 35000f, // 35 кН/м
            SuspensionDamping = 4500f,    // 4.5 кН⋅с/м
            SuspensionCompression = 0f,
            Friction = 1f,                // Базовое трение
            LateralFriction = 0.8f,       // Боковое трение
            LongitudinalFriction = 1.2f,  // Продольное трение
            RollingResistance = 0.02f,    // 2% сопротивление
            AngularVelocity = 0f,
            Torque = 0f,
            BrakeTorque = 0f,
            SteeringAngle = 0f,
            IsGrounded = false,
            ContactPoint = new float3(0f, 0f, 0f),
            ContactNormal = new float3(0f, 1f, 0f),
            ContactDistance = 0f,
            GroundFriction = 1f,
            DeformationForce = 0f,
            DeformationRadius = 0.2f,     // 20 см
            MudLevel = 0f,
            TractionModifier = 1f,
            Wear = 0f,
            Temperature = 20f,            // 20°C
            Pressure = 220f,              // 220 кПа
            OptimalPressure = 220f,       // 220 кПа
            IsActive = true,
            NeedsUpdate = true,
            LastUpdateTime = 0f,
            UpdateFrequency = 60f         // 60 Гц
        };
        
        /// <summary>
        /// Вычисляет линейную скорость колеса
        /// </summary>
        public float GetLinearVelocity()
        {
            return AngularVelocity * Radius;
        }
        
        /// <summary>
        /// Вычисляет эффективность торможения
        /// </summary>
        public float GetBrakingEfficiency()
        {
            if (BrakeTorque <= 0f) return 0f;
            return math.min(BrakeTorque / (Mass * 9.81f * Radius), 1f);
        }
        
        /// <summary>
        /// Проверяет, нужна ли подкачка шины
        /// </summary>
        public bool NeedsInflation()
        {
            return Pressure < OptimalPressure * 0.8f;
        }
        
        /// <summary>
        /// Проверяет, перегрета ли шина
        /// </summary>
        public bool IsOverheated()
        {
            return Temperature > 80f; // 80°C
        }
        
        /// <summary>
        /// Вычисляет эффективность тяги
        /// </summary>
        public float GetTractionEfficiency()
        {
            float wearFactor = 1f - Wear;
            float pressureFactor = math.clamp(Pressure / OptimalPressure, 0.5f, 1.5f);
            float temperatureFactor = math.clamp(1f - (Temperature - 20f) / 100f, 0.1f, 1f);
            
            return TractionModifier * wearFactor * pressureFactor * temperatureFactor;
        }
        
        /// <summary>
        /// Вычисляет силу сопротивления
        /// </summary>
        public float GetResistanceForce()
        {
            float rollingResistance = Mass * 9.81f * RollingResistance;
            float mudResistance = MudLevel * 1000f; // 1кН на полную грязь
            float deformationResistance = DeformationForce * 0.1f;
            
            return rollingResistance + mudResistance + deformationResistance;
        }
    }
}