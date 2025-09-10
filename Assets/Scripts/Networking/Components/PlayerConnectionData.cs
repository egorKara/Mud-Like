using Unity.Entities;
using Unity.NetCode;

namespace MudLike.Networking.Components
{
    /// <summary>
    /// Данные подключения игрока
    /// </summary>
    [GhostComponent(PrefabType = GhostPrefabType.AllPredicted)]
    public struct PlayerConnectionData : IComponentData
    {
        /// <summary>
        /// ID игрока
        /// </summary>
        [GhostField]
        public int PlayerID;
        
        /// <summary>
        /// Имя игрока
        /// </summary>
        [GhostField]
        public FixedString64Bytes PlayerName;
        
        /// <summary>
        /// Ping игрока в мс
        /// </summary>
        [GhostField]
        public int Ping;
        
        /// <summary>
        /// Статус подключения
        /// </summary>
        [GhostField]
        public PlayerConnectionStatus Status;
        
        /// <summary>
        /// Время подключения
        /// </summary>
        [GhostField]
        public double ConnectionTime;
        
        /// <summary>
        /// Последнее время активности
        /// </summary>
        [GhostField]
        public double LastActivityTime;
    }
    
    /// <summary>
    /// Статус подключения игрока
    /// </summary>
    public enum PlayerConnectionStatus : byte
    {
        Disconnected = 0,
        Connecting = 1,
        Connected = 2,
        InGame = 3,
        Disconnecting = 4
    }
}