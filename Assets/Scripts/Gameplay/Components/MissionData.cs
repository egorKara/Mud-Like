using Unity.Entities;

namespace MudLike.Gameplay.Components
{
    /// <summary>
    /// Данные миссии
    /// </summary>
    public struct MissionData : IComponentData
    {
        /// <summary>
        /// Тип миссии
        /// </summary>
        public MissionType Type;
        
        /// <summary>
        /// Название миссии
        /// </summary>
        public FixedString128Bytes Title;
        
        /// <summary>
        /// Описание миссии
        /// </summary>
        public FixedString512Bytes Description;
        
        /// <summary>
        /// Статус миссии
        /// </summary>
        public MissionStatus Status;
        
        /// <summary>
        /// Прогресс миссии
        /// </summary>
        public float Progress;
        
        /// <summary>
        /// Максимальный прогресс
        /// </summary>
        public float MaxProgress;
        
        /// <summary>
        /// Награда за миссию
        /// </summary>
        public float Reward;
        
        /// <summary>
        /// Бонус за миссию
        /// </summary>
        public float Bonus;
        
        /// <summary>
        /// Штраф за миссию
        /// </summary>
        public float Penalty;
        
        /// <summary>
        /// Временной лимит миссии
        /// </summary>
        public float TimeLimit;
        
        /// <summary>
        /// Время начала миссии
        /// </summary>
        public float StartTime;
        
        /// <summary>
        /// Время окончания миссии
        /// </summary>
        public float EndTime;
        
        /// <summary>
        /// Миссия активна
        /// </summary>
        public bool IsActive;
        
        /// <summary>
        /// Миссия приостановлена
        /// </summary>
        public bool IsPaused;
        
        /// <summary>
        /// Миссия завершена
        /// </summary>
        public bool IsCompleted;
        
        /// <summary>
        /// Миссия провалена
        /// </summary>
        public bool IsFailed;
        
        /// <summary>
        /// Миссия брошена
        /// </summary>
        public bool IsAbandoned;
        
        /// <summary>
        /// Миссия перезапущена
        /// </summary>
        public bool IsRestarted;
        
        /// <summary>
        /// Миссия обновлена
        /// </summary>
        public bool IsUpdated;
        
        /// <summary>
        /// Цель миссии выполнена
        /// </summary>
        public bool ObjectiveCompleted;
        
        /// <summary>
        /// Цель миссии провалена
        /// </summary>
        public bool ObjectiveFailed;
        
        /// <summary>
        /// Награда получена
        /// </summary>
        public bool RewardEarned;
        
        /// <summary>
        /// Бонус получен
        /// </summary>
        public bool BonusEarned;
        
        /// <summary>
        /// Штраф применен
        /// </summary>
        public bool PenaltyApplied;
        
        /// <summary>
        /// Время последнего обновления
        /// </summary>
        public float LastUpdateTime;
        
        /// <summary>
        /// Требует обновления
        /// </summary>
        public bool NeedsUpdate;
    }
    
    /// <summary>
    /// Типы миссий
    /// </summary>
    public enum MissionType
    {
        Delivery,       // Доставка
        Transport,      // Транспортировка
        Rescue,         // Спасение
        Exploration,    // Исследование
        Collection,     // Сбор
        Construction,   // Строительство
        Maintenance,    // Обслуживание
        Racing,         // Гонки
        TimeTrial,      // Заезд на время
        Endurance       // Выносливость
    }
    
    /// <summary>
    /// Статусы миссий
    /// </summary>
    public enum MissionStatus
    {
        NotStarted,     // Не начата
        InProgress,     // В процессе
        Paused,         // Приостановлена
        Completed,      // Завершена
        Failed,         // Провалена
        Abandoned,      // Брошена
        Cancelled       // Отменена
    }
}