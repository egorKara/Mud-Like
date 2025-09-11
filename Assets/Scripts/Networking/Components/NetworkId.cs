using Unity.Entities;
using Unity.NetCode;

namespace MudLike.Networking.Components
{
    /// <summary>
    /// Сетевой идентификатор сущности
    /// </summary>
    public struct NetworkId : IComponentData
    {
        /// <summary>
        /// Уникальный идентификатор
        /// </summary>
        public int Value;
        
        /// <summary>
        /// Тип сущности
        /// </summary>
        public NetworkEntityType EntityType;
        
        /// <summary>
        /// Владелец сущности
        /// </summary>
        public int OwnerId;
        
        /// <summary>
        /// Сущность синхронизируется
        /// </summary>
        public bool IsSynced;
    }
    
    /// <summary>
    /// Тип сетевой сущности
    /// </summary>
    public enum NetworkEntityType
    {
        Player,
        Vehicle,
        Terrain,
        Deformation,
        Mud
    }
}