using Unity.Entities;
using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using MudLike.Core.Components;

namespace MudLike.UI.Systems
{
    /// <summary>
    /// ECS система лобби
    /// Заменяет MonoBehaviour LobbySystem
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class LobbySystem : SystemBase
    {
        private EntityQuery _serverQuery;
        private EntityQuery _playerQuery;
        private EntityQuery _roomQuery;
        
        protected override void OnCreate()
        {
            // Создаем запросы для лобби компонентов
            _serverQuery = GetEntityQuery(typeof(ServerInfo), typeof(UIElement));
            _playerQuery = GetEntityQuery(typeof(PlayerInfo), typeof(UIElement));
            _roomQuery = GetEntityQuery(typeof(RoomInfo), typeof(UIElement));
        }
        
        protected override void OnUpdate()
        {
            // Обновляем список серверов
            UpdateServerList();
            
            // Обновляем список игроков
            UpdatePlayerList();
            
            // Обновляем комнаты
            UpdateRooms();
        }
        
        /// <summary>
        /// Обновляет список серверов
        /// </summary>
        private void UpdateServerList()
        {
            Entities
                .WithAll<ServerInfo, UIElement>()
                .ForEach((ref ServerInfo server, ref UIElement element) =>
                {
                    if (server.IsUpdated)
                    {
                        UpdateServerUI(server);
                        server.IsUpdated = false;
                    }
                }).WithoutBurst().Run();
        }
        
        /// <summary>
        /// Обновляет список игроков
        /// </summary>
        private void UpdatePlayerList()
        {
            Entities
                .WithAll<PlayerInfo, UIElement>()
                .ForEach((ref PlayerInfo player, ref UIElement element) =>
                {
                    if (player.IsUpdated)
                    {
                        UpdatePlayerUI(player);
                        player.IsUpdated = false;
                    }
                }).WithoutBurst().Run();
        }
        
        /// <summary>
        /// Обновляет комнаты
        /// </summary>
        private void UpdateRooms()
        {
            Entities
                .WithAll<RoomInfo, UIElement>()
                .ForEach((ref RoomInfo room, ref UIElement element) =>
                {
                    if (room.IsUpdated)
                    {
                        UpdateRoomUI(room);
                        room.IsUpdated = false;
                    }
                }).WithoutBurst().Run();
        }
        
        /// <summary>
        /// Обновляет UI сервера
        /// </summary>
        private void UpdateServerUI(ServerInfo server)
        {
            Debug.Log($"Server: {server.Name}, Players: {server.PlayerCount}/{server.MaxPlayers}, Ping: {server.Ping}ms");
        }
        
        /// <summary>
        /// Обновляет UI игрока
        /// </summary>
        private void UpdatePlayerUI(PlayerInfo player)
        {
            Debug.Log($"Player: {player.Name}, Ready: {player.IsReady}");
        }
        
        /// <summary>
        /// Обновляет UI комнаты
        /// </summary>
        private void UpdateRoomUI(RoomInfo room)
        {
            Debug.Log($"Room: {room.Name}, Players: {room.PlayerCount}/{room.MaxPlayers}");
        }
    }
    
    /// <summary>
    /// Информация о сервере
    /// </summary>
    public struct Server : IComponentData
    {
        public int ServerId;
        public bool IsUpdated;
        public bool IsOnline;
        public int PlayerCount;
        public int MaxPlayers;
        public int Ping;
        public float LastUpdateTime;
    }
    
    /// <summary>
    /// Информация об игроке
    /// </summary>
    public struct Player : IComponentData
    {
        public int PlayerId;
        public bool IsUpdated;
        public bool IsReady;
        public bool IsHost;
        public float LastUpdateTime;
    }
    
    /// <summary>
    /// Информация о комнате
    /// </summary>
    public struct Room : IComponentData
    {
        public int RoomId;
        public bool IsUpdated;
        public bool IsPrivate;
        public int PlayerCount;
        public int MaxPlayers;
        public float LastUpdateTime;
    }
}
