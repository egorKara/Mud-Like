using Unity.Entities;
using Unity.NetCode;

namespace MudLike.Networking.Components
{
    /// <summary>
    /// Сетевой идентификатор сущности для Unity 6
    /// Интегрирован с GhostSystem
    /// </summary>
    [GhostComponent(PrefabType = GhostPrefabType.All, SendTypeOptimization = GhostSendType.AllClients)]
    public struct NetworkId : IComponentData
    {
        /// <summary>
        /// Уникальный идентификатор в сети
        /// </summary>
        [GhostField]
        public int Value;
        
        /// <summary>
        /// Время последнего обновления
        /// </summary>
        [GhostField(Quantization = 1000)]
        public float LastUpdateTime;
        
        /// <summary>
        /// Тип сущности (Unity 6)
        /// </summary>
        [GhostField]
        public byte EntityType;
        
        /// <summary>
        /// Флаг авторитета (Unity 6)
        /// </summary>
        [GhostField]
        public bool IsAuthoritative;
        
        /// <summary>
        /// Приоритет обновления (Unity 6)
        /// </summary>
        [GhostField]
        public byte UpdatePriority;
        
        /// <summary>
        /// Создает новый сетевой ID
        /// </summary>
        /// <param name="id">Значение идентификатора</param>
        /// <param name="entityType">Тип сущности</param>
        /// <param name="isAuthoritative">Авторитетность</param>
        /// <returns>Новый NetworkId</returns>
        public static NetworkId Create(int id, byte entityType = 0, bool isAuthoritative = false)
        {
            return new NetworkId
            {
                Value = id,
                LastUpdateTime = 0f,
                EntityType = entityType,
                IsAuthoritative = isAuthoritative,
                UpdatePriority = 0
            };
        }
        
        /// <summary>
        /// Создает авторитетный сетевой ID
        /// </summary>
        /// <param name="id">Значение идентификатора</param>
        /// <param name="entityType">Тип сущности</param>
        /// <returns>Новый авторитетный NetworkId</returns>
        public static NetworkId CreateAuthoritative(int id, byte entityType = 0)
        {
            return new NetworkId
            {
                Value = id,
                LastUpdateTime = 0f,
                EntityType = entityType,
                IsAuthoritative = true,
                UpdatePriority = 255 // Высший приоритет
            };
        }
    }
}