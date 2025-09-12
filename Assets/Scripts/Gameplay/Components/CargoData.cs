using Unity.Entities;

namespace MudLike.Gameplay.Components
{
    /// <summary>
    /// Данные груза
    /// </summary>
    public struct CargoData : IComponentData
    {
        /// <summary>
        /// Тип груза
        /// </summary>
        public CargoType Type;
        
        /// <summary>
        /// Вес груза
        /// </summary>
        public float Weight;
        
        /// <summary>
        /// Объем груза
        /// </summary>
        public float Volume;
        
        /// <summary>
        /// Стоимость груза
        /// </summary>
        public float Value;
        
        /// <summary>
        /// Хрупкость груза
        /// </summary>
        public float Fragility;
        
        /// <summary>
        /// Груз загружен
        /// </summary>
        public bool IsLoaded;
        
        /// <summary>
        /// Груз поврежден
        /// </summary>
        public bool IsDamaged;
        
        /// <summary>
        /// Груз потерян
        /// </summary>
        public bool IsLost;
        
        /// <summary>
        /// Груз доставлен
        /// </summary>
        public bool IsDelivered;
        
        /// <summary>
        /// Груз украден
        /// </summary>
        public bool IsStolen;
        
        /// <summary>
        /// Груз найден
        /// </summary>
        public bool IsFound;
        
        /// <summary>
        /// Груз скрыт
        /// </summary>
        public bool IsHidden;
        
        /// <summary>
        /// Груз обнаружен
        /// </summary>
        public bool IsDiscovered;
        
        /// <summary>
        /// Груз раскрыт
        /// </summary>
        public bool IsRevealed;
        
        /// <summary>
        /// Груз скрыт
        /// </summary>
        public bool IsConcealed;
        
        /// <summary>
        /// Груз открыт
        /// </summary>
        public bool IsExposed;
        
        /// <summary>
        /// Груз защищен
        /// </summary>
        public bool IsProtected;
        
        /// <summary>
        /// Груз закреплен
        /// </summary>
        public bool IsSecured;
        
        /// <summary>
        /// Груз освобожден
        /// </summary>
        public bool IsReleased;
        
        /// <summary>
        /// Груз свободен
        /// </summary>
        public bool IsFreed;
        
        /// <summary>
        /// Груз захвачен
        /// </summary>
        public bool IsCaptured;
        
        /// <summary>
        /// Груз сбежал
        /// </summary>
        public bool IsEscaped;
        
        /// <summary>
        /// Груз спасен
        /// </summary>
        public bool IsRescued;
        
        /// <summary>
        /// Груз брошен
        /// </summary>
        public bool IsAbandoned;
        
        /// <summary>
        /// Время загрузки
        /// </summary>
        public float LoadTime;
        
        /// <summary>
        /// Время разгрузки
        /// </summary>
        public float UnloadTime;
        
        /// <summary>
        /// Время доставки
        /// </summary>
        public float DeliveryTime;
        
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
    /// Типы груза
    /// </summary>
    public enum CargoType
    {
        General,        // Общий груз
        Fragile,        // Хрупкий груз
        Hazardous,      // Опасный груз
        Valuable,       // Ценный груз
        Heavy,          // Тяжелый груз
        Light,          // Легкий груз
        Liquid,         // Жидкий груз
        Solid,          // Твердый груз
        Gas,            // Газообразный груз
        Special         // Специальный груз
    }
}