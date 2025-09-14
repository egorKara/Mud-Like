using Unity.Entities;
using Unity.Mathematics;

namespace MudLike.Vehicles.Components
{
    /// <summary>
    /// Данные шины транспортного средства
    /// </summary>
    public struct TireData : IComponentData
    {
        /// <summary>
        /// Тип шины
        /// </summary>
        public TireType Type;
        
        /// <summary>
        /// Материал шины
        /// </summary>
        public TireMaterial Material;
        
        /// <summary>
        /// Размер шины (ширина/высота/диаметр в мм)
        /// </summary>
        public float3 Size;
        
        /// <summary>
        /// Радиус шины
        /// </summary>
        public float Radius;
        
        /// <summary>
        /// Ширина шины
        /// </summary>
        public float Width;
        
        /// <summary>
        /// Высота профиля шины
        /// </summary>
        public float ProfileHeight;
        
        /// <summary>
        /// Текущее давление в шине (кПа)
        /// </summary>
        public float CurrentPressure;
        
        /// <summary>
        /// Рекомендуемое давление в шине (кПа)
        /// </summary>
        public float RecommendedPressure;
        
        /// <summary>
        /// Минимальное давление в шине (кПа)
        /// </summary>
        public float MinPressure;
        
        /// <summary>
        /// Максимальное давление в шине (кПа)
        /// </summary>
        public float MaxPressure;
        
        /// <summary>
        /// Температура шины (°C)
        /// </summary>
        public float Temperature;
        
        /// <summary>
        /// Максимальная рабочая температура (°C)
        /// </summary>
        public float MaxTemperature;
        
        /// <summary>
        /// Износ протектора (0-1, где 1 = полностью изношен)
        /// </summary>
        public float TreadWear;
        
        /// <summary>
        /// Глубина протектора (мм)
        /// </summary>
        public float TreadDepth;
        
        /// <summary>
        /// Новая глубина протектора (мм)
        /// </summary>
        public float NewTreadDepth;
        
        /// <summary>
        /// Минимальная глубина протектора (мм)
        /// </summary>
        public float MinTreadDepth;
        
        /// <summary>
        /// Площадь контакта с поверхностью (м²)
        /// </summary>
        public float ContactArea;
        
        /// <summary>
        /// Коэффициент трения с текущей поверхностью
        /// </summary>
        public float FrictionCoefficient;
        
        /// <summary>
        /// Коэффициент сцепления с текущей поверхностью
        /// </summary>
        public float TractionCoefficient;
        
        /// <summary>
        /// Сопротивление качению
        /// </summary>
        public float RollingResistance;
        
        /// <summary>
        /// Жесткость шины
        /// </summary>
        public float Stiffness;
        
        /// <summary>
        /// Демпфирование шины
        /// </summary>
        public float Damping;
        
        /// <summary>
        /// Эластичность шины
        /// </summary>
        public float Elasticity;
        
        /// <summary>
        /// Плотность материала шины (кг/м³)
        /// </summary>
        public float Density;
        
        /// <summary>
        /// Теплопроводность материала шины
        /// </summary>
        public float ThermalConductivity;
        
        /// <summary>
        /// Удельная теплоемкость материала шины
        /// </summary>
        public float SpecificHeatCapacity;
        
        /// <summary>
        /// Скорость нагрева шины (°C/с)
        /// </summary>
        public float HeatingRate;
        
        /// <summary>
        /// Скорость охлаждения шины (°C/с)
        /// </summary>
        public float CoolingRate;
        
        /// <summary>
        /// Скорость износа протектора (мм/с)
        /// </summary>
        public float WearRate;
        
        /// <summary>
        /// Время в контакте с текущей поверхностью (с)
        /// </summary>
        public float ContactTime;
        
        /// <summary>
        /// Последний тип поверхности
        /// </summary>
        public int LastSurfaceType;
        
        /// <summary>
        /// Последний тип погоды
        /// </summary>
        public int LastWeatherType;
        
        /// <summary>
        /// Количество частиц грязи на шине
        /// </summary>
        public int MudParticleCount;
        
        /// <summary>
        /// Масса грязи на шине (кг)
        /// </summary>
        public float MudMass;
        
        /// <summary>
        /// Скорость очистки от грязи (кг/с)
        /// </summary>
        public float CleaningRate;
        
        /// <summary>
        /// Влажность шины (0-1)
        /// </summary>
        public float Moisture;
        
        /// <summary>
        /// Скорость высыхания шины (1/с)
        /// </summary>
        public float DryingRate;
        
        /// <summary>
        /// Возраст шины (дни)
        /// </summary>
        public float Age;
        
        /// <summary>
        /// Максимальный возраст шины (дни)
        /// </summary>
        public float MaxAge;
        
        /// <summary>
        /// Пробег шины (км)
        /// </summary>
        public float Mileage;
        
        /// <summary>
        /// Максимальный пробег шины (км)
        /// </summary>
        public float MaxMileage;
        
        /// <summary>
        /// Состояние шины
        /// </summary>
        public TireCondition Condition;
        
        /// <summary>
        /// Требует ли обновления
        /// </summary>
        public bool NeedsUpdate;
        
        /// <summary>
        /// Активна ли шина
        /// </summary>
        public bool IsActive;
    }
    
    /// <summary>
    /// Типы шин
    /// </summary>
    public enum TireType
    {
        Summer,     // Летние шины
        Winter,     // Зимние шины
        AllSeason,  // Всесезонные шины
        OffRoad,    // Внедорожные шины
        Mud,        // Грязевые шины
        Snow,       // Снежные шины
        Ice,        // Ледовые шины
        Street      // Дорожные шины
    }
    
    /// <summary>
    /// Материалы шин
    /// </summary>
    public enum TireMaterial
    {
        NaturalRubber,      // Натуральный каучук
        SyntheticRubber,    // Синтетический каучук
        Silica,            // Кремнезем
        CarbonBlack,       // Сажа
        Steel,             // Сталь (каркас)
        Polyester,         // Полиэстер (корд)
        Nylon,             // Нейлон (корд)
        Aramid,            // Арамид (корд)
        Hybrid             // Гибридный материал
    }
    
    /// <summary>
    /// Состояние шины
    /// </summary>
    public enum TireCondition
    {
        New,        // Новая
        Good,       // Хорошее
        Fair,       // Удовлетворительное
        Poor,       // Плохое
        Worn,       // Изношенная
        Damaged,    // Поврежденная
        Blown       // Лопнувшая
    }
}