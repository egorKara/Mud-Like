using Unity.Entities;
using Unity.Mathematics;

namespace MudLike.Vehicles.Components
{
    /// <summary>
    /// Данные груза
    /// </summary>
    public struct CargoData : IComponentData
    {
        /// <summary>
        /// ID груза
        /// </summary>
        public int CargoId;
        
        /// <summary>
        /// Тип груза
        /// </summary>
        public CargoType Type;
        
        /// <summary>
        /// Название груза
        /// </summary>
        public FixedString64Bytes Name;
        
        /// <summary>
        /// Описание груза
        /// </summary>
        public FixedString128Bytes Description;
        
        /// <summary>
        /// Масса груза (кг)
        /// </summary>
        public float Mass;
        
        /// <summary>
        /// Объем груза (м³)
        /// </summary>
        public float Volume;
        
        /// <summary>
        /// Ценность груза
        /// </summary>
        public int Value;
        
        /// <summary>
        /// Состояние груза (0-1)
        /// </summary>
        public float Condition;
        
        /// <summary>
        /// Груз загружен
        /// </summary>
        public bool IsLoaded;
        
        /// <summary>
        /// Груз поврежден
        /// </summary>
        public bool IsDamaged;
        
        /// <summary>
        /// Груз требует особого обращения
        /// </summary>
        public bool IsFragile;
        
        /// <summary>
        /// Груз опасен
        /// </summary>
        public bool IsHazardous;
        
        /// <summary>
        /// Груз требует холодильника
        /// </summary>
        public bool RequiresRefrigeration;
        
        /// <summary>
        /// Груз требует герметичности
        /// </summary>
        public bool RequiresSealing;
        
        /// <summary>
        /// Время до порчи (секунды)
        /// </summary>
        public float TimeToSpoil;
        
        /// <summary>
        /// Максимальное время до порчи (секунды)
        /// </summary>
        public float MaxTimeToSpoil;
        
        /// <summary>
        /// Груз требует обновления
        /// </summary>
        public bool NeedsUpdate;
    }
    
    /// <summary>
    /// Данные грузового отсека
    /// </summary>
    public struct CargoBayData : IComponentData
    {
        /// <summary>
        /// Максимальная грузоподъемность (кг)
        /// </summary>
        public float MaxCapacity;
        
        /// <summary>
        /// Максимальный объем (м³)
        /// </summary>
        public float MaxVolume;
        
        /// <summary>
        /// Текущая загрузка (кг)
        /// </summary>
        public float CurrentLoad;
        
        /// <summary>
        /// Текущий объем (м³)
        /// </summary>
        public float CurrentVolume;
        
        /// <summary>
        /// Количество грузов
        /// </summary>
        public int CargoCount;
        
        /// <summary>
        /// Максимальное количество грузов
        /// </summary>
        public int MaxCargoCount;
        
        /// <summary>
        /// Грузовой отсек открыт
        /// </summary>
        public bool IsOpen;
        
        /// <summary>
        /// Грузовой отсек заблокирован
        /// </summary>
        public bool IsLocked;
        
        /// <summary>
        /// Грузовой отсек требует обновления
        /// </summary>
        public bool NeedsUpdate;
    }
    
    /// <summary>
    /// Данные миссии с грузом
    /// </summary>
    public struct CargoMissionData : IComponentData
    {
        /// <summary>
        /// ID миссии
        /// </summary>
        public int MissionId;
        
        /// <summary>
        /// Тип миссии
        /// </summary>
        public CargoMissionType Type;
        
        /// <summary>
        /// Название миссии
        /// </summary>
        public FixedString64Bytes Name;
        
        /// <summary>
        /// Описание миссии
        /// </summary>
        public FixedString128Bytes Description;
        
        /// <summary>
        /// Позиция загрузки
        /// </summary>
        public float3 LoadPosition;
        
        /// <summary>
        /// Позиция разгрузки
        /// </summary>
        public float3 UnloadPosition;
        
        /// <summary>
        /// Время на выполнение (секунды)
        /// </summary>
        public float TimeLimit;
        
        /// <summary>
        /// Оставшееся время (секунды)
        /// </summary>
        public float RemainingTime;
        
        /// <summary>
        /// Награда за выполнение
        /// </summary>
        public int Reward;
        
        /// <summary>
        /// Штраф за невыполнение
        /// </summary>
        public int Penalty;
        
        /// <summary>
        /// Миссия активна
        /// </summary>
        public bool IsActive;
        
        /// <summary>
        /// Миссия выполнена
        /// </summary>
        public bool IsCompleted;
        
        /// <summary>
        /// Миссия провалена
        /// </summary>
        public bool IsFailed;
        
        /// <summary>
        /// Миссия требует обновления
        /// </summary>
        public bool NeedsUpdate;
    }
    
    /// <summary>
    /// Тип груза
    /// </summary>
    public enum CargoType
    {
        General,        // Общий груз
        Food,           // Продукты питания
        Medicine,       // Медикаменты
        Fuel,           // Топливо
        Construction,   // Строительные материалы
        Electronics,    // Электроника
        Chemicals,      // Химические вещества
        Livestock,      // Скот
        Perishable,     // Скоропортящиеся товары
        Hazardous,      // Опасные грузы
        Fragile,        // Хрупкие грузы
        Oversized,      // Крупногабаритные грузы
        Liquid,         // Жидкие грузы
        Gas,            // Газообразные грузы
        Solid           // Твердые грузы
    }
    
    /// <summary>
    /// Тип миссии с грузом
    /// </summary>
    public enum CargoMissionType
    {
        Delivery,       // Доставка
        Pickup,         // Забор
        Transport,      // Транспортировка
        Emergency,      // Экстренная доставка
        Special,        // Специальная миссия
        TimeCritical,   // Срочная доставка
        LongDistance,   // Дальняя доставка
        Local,          // Местная доставка
        International,  // Международная доставка
        Military        // Военная доставка
    }
}