using Unity.Entities;

namespace MudLike.Gameplay.Components
{
    /// <summary>
    /// Данные лебедки
    /// </summary>
    public struct WinchData : IComponentData
    {
        /// <summary>
        /// Максимальная длина троса
        /// </summary>
        public float MaxLength;
        
        /// <summary>
        /// Текущая длина троса
        /// </summary>
        public float CurrentLength;
        
        /// <summary>
        /// Максимальная сила
        /// </summary>
        public float MaxForce;
        
        /// <summary>
        /// Текущая сила
        /// </summary>
        public float CurrentForce;
        
        /// <summary>
        /// Скорость работы
        /// </summary>
        public float Speed;
        
        /// <summary>
        /// Лебедка прикреплена
        /// </summary>
        public bool IsAttached;
        
        /// <summary>
        /// Лебедка тянет
        /// </summary>
        public bool IsPulling;
        
        /// <summary>
        /// Лебедка отпускает
        /// </summary>
        public bool IsReleasing;
        
        /// <summary>
        /// Лебедка сломана
        /// </summary>
        public bool IsBroken;
        
        /// <summary>
        /// Лебедка перегружена
        /// </summary>
        public bool IsOverloaded;
        
        /// <summary>
        /// Лебедка застряла
        /// </summary>
        public bool IsStuck;
        
        /// <summary>
        /// Натяжение троса
        /// </summary>
        public float Tension;
        
        /// <summary>
        /// Угол наклона
        /// </summary>
        public float Angle;
        
        /// <summary>
        /// КПД лебедки
        /// </summary>
        public float Efficiency;
        
        /// <summary>
        /// Износ лебедки
        /// </summary>
        public float Wear;
        
        /// <summary>
        /// Температура лебедки
        /// </summary>
        public float Temperature;
        
        /// <summary>
        /// Смазка лебедки
        /// </summary>
        public float Lubrication;
        
        /// <summary>
        /// Требуется обслуживание
        /// </summary>
        public bool MaintenanceRequired;
        
        /// <summary>
        /// Время последнего обслуживания
        /// </summary>
        public float LastMaintenanceTime;
        
        /// <summary>
        /// Время следующего обслуживания
        /// </summary>
        public float NextMaintenanceTime;
        
        /// <summary>
        /// Лебедка активна
        /// </summary>
        public bool IsActive;
        
        /// <summary>
        /// Требует обновления
        /// </summary>
        public bool NeedsUpdate;
    }
}