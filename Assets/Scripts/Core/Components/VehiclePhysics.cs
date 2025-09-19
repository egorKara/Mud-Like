using Unity.Entities;
using Unity.Mathematics;

namespace MudLike.Vehicles.Components
{
    /// <summary>
    /// Компонент физики транспортного средства
    /// Содержит все физические параметры для реалистичной симуляции
    /// </summary>
    public struct VehiclePhysics : IComponentData
    {
        // Основные физические параметры
        public float3 Velocity;           // Скорость транспорта
        public float3 Acceleration;       // Ускорение транспорта
        public quaternion Rotation;       // Поворот транспорта
        public float Mass;                // Масса транспорта (кг)
        
        // Параметры двигателя
        public float EnginePower;         // Текущая мощность двигателя (Вт)
        public float MaxEnginePower;      // Максимальная мощность двигателя (Вт)
        public float EngineRPM;           // Обороты двигателя (об/мин)
        public float EngineTorque;        // Крутящий момент двигателя (Н⋅м)
        
        // Параметры трансмиссии
        public int CurrentGear;           // Текущая передача
        public float GearRatio;           // Передаточное число
        public float DifferentialRatio;   // Передаточное число дифференциала
        
        // Параметры управления
        public float SteeringAngle;       // Угол поворота руля (радианы)
        public float MaxSteeringAngle;    // Максимальный угол поворота (радианы)
        public float SteeringReturnSpeed; // Скорость возврата руля
        
        // Параметры движения
        public float MaxSpeed;            // Максимальная скорость (м/с)
        public float Acceleration;        // Ускорение (м/с²)
        public float Deceleration;        // Замедление (м/с²)
        public float TurnSpeedMultiplier; // Множитель скорости поворота
        
        // Параметры торможения
        public float EngineBraking;       // Торможение двигателем
        public float BrakeForce;          // Сила торможения
        public float MaxBrakeForce;       // Максимальная сила торможения
        
        // Параметры подвески
        public float SuspensionStiffness; // Жесткость подвески
        public float SuspensionDamping;   // Демпфирование подвески
        public float SuspensionLength;    // Длина подвески
        
        // Параметры колес
        public float WheelRadius;         // Радиус колеса (м)
        public float WheelWidth;          // Ширина колеса (м)
        public float WheelFriction;       // Коэффициент трения колеса
        
        // Параметры сопротивления
        public float Drag;                // Коэффициент аэродинамического сопротивления
        public float RollingResistance;   // Коэффициент сопротивления качению
        
        // Параметры стабилизации
        public float Stability;           // Стабильность транспорта
        public float Downforce;           // Прижимная сила
        
        // Временные параметры
        public float LastUpdateTime;      // Время последнего обновления
        public bool IsGrounded;           // Находится ли на земле
        public float GroundDistance;      // Расстояние до земли
        
        // Параметры деформации
        public float DeformationForce;    // Сила деформации террейна
        public float DeformationRadius;   // Радиус деформации
        public float MudResistance;       // Сопротивление грязи
        
        // Параметры производительности
        public bool IsOptimized;          // Оптимизирован ли для производительности
        public float LODLevel;            // Уровень детализации (0-1)
        public float UpdateFrequency;     // Частота обновления (Гц)
        
        // Параметры взаимодействия с террейном
        public float EffectiveTraction;   // Эффективное сцепление (0-1)
        public float MudDrag;             // Сопротивление грязи
        public float BaseMaxSpeed;        // Базовая максимальная скорость
        public float BaseAcceleration;    // Базовое ускорение
        
        // Конструктор со значениями по умолчанию
        public static VehiclePhysics Default => new VehiclePhysics
        {
            Mass = 1500f,                    // 1.5 тонны
            MaxEnginePower = 150000f,        // 150 кВт
            MaxSpeed = 50f,                  // 180 км/ч
            Acceleration = 8f,               // 8 м/с²
            Deceleration = 12f,              // 12 м/с²
            MaxSteeringAngle = 0.6f,         // ~35 градусов
            SteeringReturnSpeed = 2f,        // 2 рад/с
            TurnSpeedMultiplier = 1f,        // Базовый множитель
            EngineBraking = 1f,              // Полное торможение двигателем
            MaxBrakeForce = 15000f,          // 15 кН
            SuspensionStiffness = 35000f,    // 35 кН/м
            SuspensionDamping = 4500f,       // 4.5 кН⋅с/м
            SuspensionLength = 0.3f,         // 30 см
            WheelRadius = 0.4f,              // 40 см
            WheelWidth = 0.25f,              // 25 см
            WheelFriction = 1f,              // Базовое трение
            Drag = 0.3f,                     // Коэффициент сопротивления
            RollingResistance = 0.02f,       // 2% сопротивление качению
            Stability = 1f,                  // Базовая стабильность
            Downforce = 0f,                  // Без прижимной силы
            IsGrounded = true,               // На земле
            GroundDistance = 0f,             // На поверхности
            DeformationForce = 0f,           // Без деформации
            DeformationRadius = 0.2f,        // 20 см радиус
            MudResistance = 0f,              // Без сопротивления грязи
            IsOptimized = false,             // Не оптимизирован
            LODLevel = 1f,                   // Максимальная детализация
            UpdateFrequency = 60f,           // 60 Гц
            LastUpdateTime = 0f,             // Начальное время
            EffectiveTraction = 1f,          // Базовое сцепление
            MudDrag = 0f,                    // Без сопротивления грязи
            BaseMaxSpeed = 50f,              // Базовая максимальная скорость
            BaseAcceleration = 8f            // Базовое ускорение
        };
        
        /// <summary>
        /// Вычисляет текущую скорость в км/ч
        /// </summary>
        public float GetSpeedKmh()
        {
            return math.length(Velocity) * 3.6f;
        }
        
        /// <summary>
        /// Вычисляет мощность в лошадиных силах
        /// </summary>
        public float GetPowerHP()
        {
            return EnginePower / 746f; // 1 л.с. = 746 Вт
        }
        
        /// <summary>
        /// Проверяет, превышена ли максимальная скорость
        /// </summary>
        public bool IsSpeedLimitExceeded()
        {
            return math.length(Velocity) > MaxSpeed;
        }
        
        /// <summary>
        /// Вычисляет эффективность торможения
        /// </summary>
        public float GetBrakingEfficiency()
        {
            if (MaxBrakeForce <= 0f) return 0f;
            return BrakeForce / MaxBrakeForce;
        }
        
        /// <summary>
        /// Вычисляет эффективность двигателя
        /// </summary>
        public float GetEngineEfficiency()
        {
            if (MaxEnginePower <= 0f) return 0f;
            return math.abs(EnginePower) / MaxEnginePower;
        }
    }
}
