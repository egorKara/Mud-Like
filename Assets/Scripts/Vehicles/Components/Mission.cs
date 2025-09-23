using Unity.Entities;
using Unity.Mathematics;

namespace MudLike.Vehicles.Components
{
    /// <summary>
    /// Данные миссии
    /// </summary>
    public struct Mission : IComponentData
    {
        /// <summary>
        /// ID миссии
        /// </summary>
        public int MissionId;
        
        /// <summary>
        /// Тип миссии
        /// </summary>
        public MissionType Type;
        
        /// <summary>
        /// Название миссии
        /// </summary>
        public FixedString64Bytes Name;
        
        /// <summary>
        /// Описание миссии
        /// </summary>
        public FixedString128Bytes Description;
        
        /// <summary>
        /// Статус миссии
        /// </summary>
        public MissionStatus Status;
        
        /// <summary>
        /// Приоритет миссии
        /// </summary>
        public MissionPriority Priority;
        
        /// <summary>
        /// Сложность миссии
        /// </summary>
        public MissionDifficulty Difficulty;
        
        /// <summary>
        /// Позиция начала миссии
        /// </summary>
        public float3 StartPosition;
        
        /// <summary>
        /// Позиция конца миссии
        /// </summary>
        public float3 EndPosition;
        
        /// <summary>
        /// Время на выполнение (секунды)
        /// </summary>
        public float TimeLimit;
        
        /// <summary>
        /// Оставшееся время (секунды)
        /// </summary>
        public float RemainingTime;
        
        /// <summary>
        /// Прогресс выполнения (0-1)
        /// </summary>
        public float Progress;
        
        /// <summary>
        /// Награда за выполнение
        /// </summary>
        public int Reward;
        
        /// <summary>
        /// Штраф за невыполнение
        /// </summary>
        public int Penalty;
        
        /// <summary>
        /// Опыт за выполнение
        /// </summary>
        public int Experience;
        
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
        /// Миссия отменена
        /// </summary>
        public bool IsCancelled;
        
        /// <summary>
        /// Миссия требует обновления
        /// </summary>
        public bool NeedsUpdate;
    }
    
    /// <summary>
    /// Данные цели миссии
    /// </summary>
    public struct MissionObjective : IComponentData
    {
        /// <summary>
        /// ID цели
        /// </summary>
        public int ObjectiveId;
        
        /// <summary>
        /// ID миссии
        /// </summary>
        public int MissionId;
        
        /// <summary>
        /// Тип цели
        /// </summary>
        public ObjectiveType Type;
        
        /// <summary>
        /// Название цели
        /// </summary>
        public FixedString64Bytes Name;
        
        /// <summary>
        /// Описание цели
        /// </summary>
        public FixedString128Bytes Description;
        
        /// <summary>
        /// Позиция цели
        /// </summary>
        public float3 Position;
        
        /// <summary>
        /// Радиус цели
        /// </summary>
        public float Radius;
        
        /// <summary>
        /// Количество для выполнения
        /// </summary>
        public int RequiredCount;
        
        /// <summary>
        /// Текущее количество
        /// </summary>
        public int CurrentCount;
        
        /// <summary>
        /// Цель выполнена
        /// </summary>
        public bool IsCompleted;
        
        /// <summary>
        /// Цель активна
        /// </summary>
        public bool IsActive;
        
        /// <summary>
        /// Цель требует обновления
        /// </summary>
        public bool NeedsUpdate;
    }
    
    /// <summary>
    /// Данные награды миссии
    /// </summary>
    public struct MissionReward : IComponentData
    {
        /// <summary>
        /// ID миссии
        /// </summary>
        public int MissionId;
        
        /// <summary>
        /// Денежная награда
        /// </summary>
        public int MoneyReward;
        
        /// <summary>
        /// Опыт
        /// </summary>
        public int ExperienceReward;
        
        /// <summary>
        /// Очки репутации
        /// </summary>
        public int ReputationReward;
        
        /// <summary>
        /// Предметы награды
        /// </summary>
        public FixedString64Bytes ItemRewards;
        
        /// <summary>
        /// Награда выдана
        /// </summary>
        public bool IsRewarded;
        
        /// <summary>
        /// Награда требует обновления
        /// </summary>
        public bool NeedsUpdate;
    }
    
    /// <summary>
    /// Тип миссии
    /// </summary>
    public enum MissionType
    {
        Delivery,       // Доставка
        Transport,      // Транспортировка
        Rescue,         // Спасение
        Exploration,    // Исследование
        Collection,     // Сбор
        Escort,         // Эскорт
        Race,           // Гонка
        TimeTrial,      // Заезд на время
        Survival,       // Выживание
        Construction,   // Строительство
        Military,       // Военная миссия
        Emergency,      // Экстренная миссия
        Special,        // Специальная миссия
        Tutorial,       // Обучение
        Story           // Сюжетная миссия
    }
    
    /// <summary>
    /// Статус миссии
    /// </summary>
    public enum MissionStatus
    {
        Available,      // Доступна
        Active,         // Активна
        Completed,      // Выполнена
        Failed,         // Провалена
        Cancelled,      // Отменена
        Locked,         // Заблокирована
        Expired         // Истекла
    }
    
    /// <summary>
    /// Приоритет миссии
    /// </summary>
    public enum MissionPriority
    {
        Low,            // Низкий
        Normal,         // Обычный
        High,           // Высокий
        Critical,       // Критический
        Emergency       // Экстренный
    }
    
    /// <summary>
    /// Сложность миссии
    /// </summary>
    public enum MissionDifficulty
    {
        Easy,           // Легкая
        Normal,         // Обычная
        Hard,           // Сложная
        Expert,         // Экспертная
        Nightmare       // Кошмарная
    }
    
    /// <summary>
    /// Тип цели
    /// </summary>
    public enum ObjectiveType
    {
        Reach,          // Достичь
        Collect,        // Собрать
        Deliver,        // Доставить
        Destroy,        // Уничтожить
        Protect,        // Защитить
        Escort,         // Эскортировать
        Survive,        // Выжить
        Complete,       // Завершить
        Time,           // Время
        Distance        // Расстояние
    }
}