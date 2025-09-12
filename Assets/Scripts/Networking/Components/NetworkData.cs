using Unity.Entities;

namespace MudLike.Networking.Components
{
    /// <summary>
    /// Сетевые данные игрока
    /// </summary>
    public struct NetworkData : IComponentData
    {
        /// <summary>
        /// Сетевой ID игрока
        /// </summary>
        public int NetworkId;
        
        /// <summary>
        /// Пинг игрока
        /// </summary>
        public float Ping;
        
        /// <summary>
        /// Время последнего обновления
        /// </summary>
        public float LastUpdateTime;
        
        /// <summary>
        /// Подозрительный ввод
        /// </summary>
        public bool SuspiciousInput;
        
        /// <summary>
        /// Неверный ввод
        /// </summary>
        public bool InvalidInput;
        
        /// <summary>
        /// Подозрение на бота
        /// </summary>
        public bool SuspectedBot;
        
        /// <summary>
        /// Количество истории ввода
        /// </summary>
        public int InputHistoryCount;
        
        /// <summary>
        /// Фактор компенсации задержки
        /// </summary>
        public float CompensationFactor;
    }
}