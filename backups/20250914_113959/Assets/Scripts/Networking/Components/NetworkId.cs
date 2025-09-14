using Unity.Entities;

namespace MudLike.Networking.Components
{
    /// <summary>
    /// Сетевой идентификатор сущности
    /// </summary>
    public struct NetworkId : IComponentData
    {
        /// <summary>
        /// Уникальный идентификатор в сети
        /// </summary>
        public int Value;
        
        /// <summary>
        /// Время последнего обновления
        /// </summary>
        public float LastUpdateTime;
        
        /// <summary>
        /// Создает новый сетевой ID
        /// </summary>
        /// <param name="id">Значение идентификатора</param>
        /// <returns>Новый NetworkId</returns>
        public static NetworkId Create(int id)
        {
            return new NetworkId
            {
                Value = id,
                LastUpdateTime = 0f
            };
        }
    }
}