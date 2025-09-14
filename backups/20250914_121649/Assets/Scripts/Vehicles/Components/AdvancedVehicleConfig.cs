using Unity.Entities;
using Unity.Mathematics;

namespace MudLike.Vehicles.Components
{
    /// <summary>
    /// Расширенная конфигурация транспортного средства с дополнительными параметрами
    /// </summary>
    public struct AdvancedVehicleConfig : IComponentData
    {
        /// <summary>
        /// Тип транспортного средства
        /// </summary>
        public VehicleType Type;
        
        /// <summary>
        /// Класс транспортного средства
        /// </summary>
        public VehicleClass Class;
        
        /// <summary>
        /// Привод (2WD, 4WD, AWD)
        /// </summary>
        public DriveType DriveType;
        
        /// <summary>
        /// Количество колес
        /// </summary>
        public int WheelCount;
        
        /// <summary>
        /// База колес (расстояние между осями)
        /// </summary>
        public float Wheelbase;
        
        /// <summary>
        /// Колея (расстояние между колесами)
        /// </summary>
        public float TrackWidth;
        
        /// <summary>
        /// Высота центра масс
        /// </summary>
        public float CenterOfMassHeight;
        
        /// <summary>
        /// Смещение центра масс по X
        /// </summary>
        public float CenterOfMassOffsetX;
        
        /// <summary>
        /// Смещение центра масс по Z
        /// </summary>
        public float CenterOfMassOffsetZ;
        
        /// <summary>
        /// Момент инерции по X
        /// </summary>
        public float InertiaX;
        
        /// <summary>
        /// Момент инерции по Y
        /// </summary>
        public float InertiaY;
        
        /// <summary>
        /// Момент инерции по Z
        /// </summary>
        public float InertiaZ;
        
        /// <summary>
        /// Коэффициент аэродинамического сопротивления
        /// </summary>
        public float DragCoefficient;
        
        /// <summary>
        /// Площадь лобового сопротивления
        /// </summary>
        public float FrontalArea;
        
        /// <summary>
        /// Коэффициент подъемной силы
        /// </summary>
        public float LiftCoefficient;
        
        /// <summary>
        /// Высота подвески
        /// </summary>
        public float GroundClearance;
        
        /// <summary>
        /// Угол въезда
        /// </summary>
        public float ApproachAngle;
        
        /// <summary>
        /// Угол съезда
        /// </summary>
        public float DepartureAngle;
        
        /// <summary>
        /// Угол рампы
        /// </summary>
        public float RampAngle;
        
        /// <summary>
        /// Максимальный угол наклона
        /// </summary>
        public float MaxTiltAngle;
        
        /// <summary>
        /// Максимальная глубина воды
        /// </summary>
        public float MaxWaterDepth;
        
        /// <summary>
        /// Максимальная глубина грязи
        /// </summary>
        public float MaxMudDepth;
        
        /// <summary>
        /// Максимальная глубина снега
        /// </summary>
        public float MaxSnowDepth;
        
        /// <summary>
        /// Вместимость топливного бака
        /// </summary>
        public float FuelTankCapacity;
        
        /// <summary>
        /// Расход топлива на 100км
        /// </summary>
        public float FuelConsumption;
        
        /// <summary>
        /// Запас хода
        /// </summary>
        public float Range;
        
        /// <summary>
        /// Время разгона 0-100 км/ч
        /// </summary>
        public float AccelerationTime;
        
        /// <summary>
        /// Тормозной путь с 100 км/ч
        /// </summary>
        public float BrakingDistance;
        
        /// <summary>
        /// Максимальная нагрузка
        /// </summary>
        public float MaxLoad;
        
        /// <summary>
        /// Грузоподъемность
        /// </summary>
        public float Payload;
        
        /// <summary>
        /// Буксировочная способность
        /// </summary>
        public float TowingCapacity;
        
        /// <summary>
        /// Включен ли полный привод
        /// </summary>
        public bool Is4WDEnabled;
        
        /// <summary>
        /// Включена ли блокировка дифференциала
        /// </summary>
        public bool IsDiffLockEnabled;
        
        /// <summary>
        /// Включена ли блокировка межосевого дифференциала
        /// </summary>
        public bool IsCenterDiffLockEnabled;
        
        /// <summary>
        /// Включена ли блокировка заднего дифференциала
        /// </summary>
        public bool IsRearDiffLockEnabled;
        
        /// <summary>
        /// Включена ли блокировка переднего дифференциала
        /// </summary>
        public bool IsFrontDiffLockEnabled;
        
        /// <summary>
        /// Включена ли понижающая передача
        /// </summary>
        public bool IsLowRangeEnabled;
        
        /// <summary>
        /// Включена ли система стабилизации
        /// </summary>
        public bool IsStabilityControlEnabled;
        
        /// <summary>
        /// Включена ли антиблокировочная система
        /// </summary>
        public bool IsABSEnabled;
        
        /// <summary>
        /// Включена ли система помощи при торможении
        /// </summary>
        public bool IsBrakeAssistEnabled;
        
        /// <summary>
        /// Включена ли система контроля тяги
        /// </summary>
        public bool IsTractionControlEnabled;
        
        /// <summary>
        /// Включена ли система помощи при старте в гору
        /// </summary>
        public bool IsHillStartAssistEnabled;
        
        /// <summary>
        /// Включена ли система помощи при спуске
        /// </summary>
        public bool IsHillDescentControlEnabled;
        
        /// <summary>
        /// Включена ли система контроля давления в шинах
        /// </summary>
        public bool IsTirePressureMonitoringEnabled;
        
        /// <summary>
        /// Включена ли система контроля температуры двигателя
        /// </summary>
        public bool IsEngineTemperatureMonitoringEnabled;
        
        /// <summary>
        /// Включена ли система контроля уровня масла
        /// </summary>
        public bool IsOilLevelMonitoringEnabled;
        
        /// <summary>
        /// Включена ли система контроля уровня охлаждающей жидкости
        /// </summary>
        public bool IsCoolantLevelMonitoringEnabled;
        
        /// <summary>
        /// Включена ли система контроля уровня тормозной жидкости
        /// </summary>
        public bool IsBrakeFluidLevelMonitoringEnabled;
        
        /// <summary>
        /// Включена ли система контроля уровня топлива
        /// </summary>
        public bool IsFuelLevelMonitoringEnabled;
        
        /// <summary>
        /// Включена ли система контроля заряда аккумулятора
        /// </summary>
        public bool IsBatteryChargeMonitoringEnabled;
        
        /// <summary>
        /// Включена ли система контроля напряжения
        /// </summary>
        public bool IsVoltageMonitoringEnabled;
        
        /// <summary>
        /// Включена ли система контроля температуры аккумулятора
        /// </summary>
        public bool IsBatteryTemperatureMonitoringEnabled;
        
        /// <summary>
        /// Включена ли система контроля температуры трансмиссии
        /// </summary>
        public bool IsTransmissionTemperatureMonitoringEnabled;
        
        /// <summary>
        /// Включена ли система контроля температуры дифференциала
        /// </summary>
        public bool IsDifferentialTemperatureMonitoringEnabled;
        
        /// <summary>
        /// Включена ли система контроля температуры подвески
        /// </summary>
        public bool IsSuspensionTemperatureMonitoringEnabled;
        
        /// <summary>
        /// Включена ли система контроля температуры тормозов
        /// </summary>
        public bool IsBrakeTemperatureMonitoringEnabled;
        
        /// <summary>
        /// Включена ли система контроля температуры шин
        /// </summary>
        public bool IsTireTemperatureMonitoringEnabled;
        
        /// <summary>
        /// Включена ли система контроля износа шин
        /// </summary>
        public bool IsTireWearMonitoringEnabled;
        
        /// <summary>
        /// Включена ли система контроля давления в шинах
        /// </summary>
        public bool IsTirePressureMonitoringEnabled2;
        
        /// <summary>
        /// Включена ли система контроля балансировки колес
        /// </summary>
        public bool IsWheelBalanceMonitoringEnabled;
        
        /// <summary>
        /// Включена ли система контроля выравнивания колес
        /// </summary>
        public bool IsWheelAlignmentMonitoringEnabled;
        
        /// <summary>
        /// Включена ли система контроля подвески
        /// </summary>
        public bool IsSuspensionMonitoringEnabled;
        
        /// <summary>
        /// Включена ли система контроля амортизаторов
        /// </summary>
        public bool IsShockAbsorberMonitoringEnabled;
        
        /// <summary>
        /// Включена ли система контроля пружин
        /// </summary>
        public bool IsSpringMonitoringEnabled;
        
        /// <summary>
        /// Включена ли система контроля стабилизаторов
        /// </summary>
        public bool IsStabilizerMonitoringEnabled;
        
        /// <summary>
        /// Включена ли система контроля рулевого управления
        /// </summary>
        public bool IsSteeringMonitoringEnabled;
        
        /// <summary>
        /// Включена ли система контроля рулевой рейки
        /// </summary>
        public bool IsSteeringRackMonitoringEnabled;
        
        /// <summary>
        /// Включена ли система контроля рулевых тяг
        /// </summary>
        public bool IsSteeringTieRodMonitoringEnabled;
        
        /// <summary>
        /// Включена ли система контроля рулевых наконечников
        /// </summary>
        public bool IsSteeringEndMonitoringEnabled;
        
        /// <summary>
        /// Включена ли система контроля рулевых подшипников
        /// </summary>
        public bool IsSteeringBearingMonitoringEnabled;
        
        /// <summary>
        /// Включена ли система контроля рулевых сальников
        /// </summary>
        public bool IsSteeringSealMonitoringEnabled;
        
        /// <summary>
        /// Включена ли система контроля рулевых уплотнений
        /// </summary>
        public bool IsSteeringGasketMonitoringEnabled;
        
        /// <summary>
        /// Включена ли система контроля рулевых прокладок
        /// </summary>
        public bool IsSteeringGasketMonitoringEnabled2;
        
        /// <summary>
        /// Включена ли система контроля рулевых втулок
        /// </summary>
        public bool IsSteeringBushingMonitoringEnabled;
        
        /// <summary>
        /// Включена ли система контроля рулевых вкладышей
        /// </summary>
        public bool IsSteeringBushingMonitoringEnabled2;
        
        /// <summary>
        /// Включена ли система контроля рулевых втулок
        /// </summary>
        public bool IsSteeringBushingMonitoringEnabled3;
        
        /// <summary>
        /// Включена ли система контроля рулевых вкладышей
        /// </summary>
        public bool IsSteeringBushingMonitoringEnabled4;
        
        /// <summary>
        /// Включена ли система контроля рулевых втулок
        /// </summary>
        public bool IsSteeringBushingMonitoringEnabled5;
        
        /// <summary>
        /// Включена ли система контроля рулевых вкладышей
        /// </summary>
        public bool IsSteeringBushingMonitoringEnabled6;
        
        /// <summary>
        /// Включена ли система контроля рулевых втулок
        /// </summary>
        public bool IsSteeringBushingMonitoringEnabled7;
        
        /// <summary>
        /// Включена ли система контроля рулевых вкладышей
        /// </summary>
        public bool IsSteeringBushingMonitoringEnabled8;
        
        /// <summary>
        /// Включена ли система контроля рулевых втулок
        /// </summary>
        public bool IsSteeringBushingMonitoringEnabled9;
        
        /// <summary>
        /// Включена ли система контроля рулевых вкладышей
        /// </summary>
        public bool IsSteeringBushingMonitoringEnabled10;
    }
    
    /// <summary>
    /// Типы транспортных средств
    /// </summary>
    public enum VehicleType
    {
        Truck,      // Грузовик
        SUV,        // Внедорожник
        Pickup,     // Пикап
        Van,        // Фургон
        Bus,        // Автобус
        Tractor,    // Трактор
        Tank,       // Танк
        APC,        // БТР
        ATV,        // Квадроцикл
        Motorcycle  // Мотоцикл
    }
    
    /// <summary>
    /// Классы транспортных средств
    /// </summary>
    public enum VehicleClass
    {
        Light,      // Легкий
        Medium,     // Средний
        Heavy,      // Тяжелый
        ExtraHeavy, // Сверхтяжелый
        Military,   // Военный
        Commercial, // Коммерческий
        Industrial, // Промышленный
        Agricultural // Сельскохозяйственный
    }
    
    /// <summary>
    /// Типы привода
    /// </summary>
    public enum DriveType
    {
        FWD,        // Передний привод
        RWD,        // Задний привод
        AWD,        // Полный привод
        FourWD,     // 4WD
        SixWD,      // 6WD
        EightWD     // 8WD
    }
}