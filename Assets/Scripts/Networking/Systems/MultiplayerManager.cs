using Unity.Entities;
using Unity.NetCode;
using Unity.Collections;
using UnityEngine;
using MudLike.Networking.Components;

namespace MudLike.Networking.Systems
{
    /// <summary>
    /// Менеджер мультиплеера
    /// </summary>
    public class MultiplayerManager : MonoBehaviour
    {
        [Header("Network Settings")]
        [SerializeField] private int maxPlayers = 8;
        [SerializeField] private int port = 7777;
        [SerializeField] private string serverIP = "127.0.0.1";
        
        [Header("Game Settings")]
        [SerializeField] private float gameStartDelay = 5f;
        [SerializeField] private float playerTimeout = 30f;
        
        private bool _isServer = false;
        private bool _isClient = false;
        private bool _isHost = false;
        
        /// <summary>
        /// Запускает сервер
        /// </summary>
        public void StartServer()
        {
            if (_isServer || _isClient) return;
            
            _isServer = true;
            _isHost = true;
            
            // Создаем сервер
            var serverWorld = ClientServerBootstrap.CreateServerWorld("ServerWorld");
            World.DefaultGameObjectInjectionWorld = serverWorld;
            
            Debug.Log($"Server started on port {port}");
        }
        
        /// <summary>
        /// Запускает клиент
        /// </summary>
        public void StartClient()
        {
            if (_isServer || _isClient) return;
            
            _isClient = true;
            
            // Создаем клиент
            var clientWorld = ClientServerBootstrap.CreateClientWorld("ClientWorld");
            World.DefaultGameObjectInjectionWorld = clientWorld;
            
            Debug.Log($"Client started, connecting to {serverIP}:{port}");
        }
        
        /// <summary>
        /// Запускает хост (сервер + клиент)
        /// </summary>
        public void StartHost()
        {
            if (_isServer || _isClient) return;
            
            _isHost = true;
            _isServer = true;
            _isClient = true;
            
            // Создаем хост
            var hostWorld = ClientServerBootstrap.CreateServerWorld("HostWorld");
            World.DefaultGameObjectInjectionWorld = hostWorld;
            
            Debug.Log($"Host started on port {port}");
        }
        
        /// <summary>
        /// Останавливает мультиплеер
        /// </summary>
        public void StopMultiplayer()
        {
            if (!_isServer && !_isClient) return;
            
            // Останавливаем все миры
            var worlds = World.All;
            foreach (var world in worlds)
            {
                if (world.Name.Contains("Server") || world.Name.Contains("Client") || world.Name.Contains("Host"))
                {
                    world.Dispose();
                }
            }
            
            _isServer = false;
            _isClient = false;
            _isHost = false;
            
            Debug.Log("Multiplayer stopped");
        }
        
        /// <summary>
        /// Получает количество подключенных игроков
        /// </summary>
        public int GetConnectedPlayersCount()
        {
            if (!_isServer && !_isHost) return 0;
            
            int count = 0;
            var world = World.DefaultGameObjectInjectionWorld;
            if (world != null)
            {
                var query = world.EntityManager.CreateEntityQuery(typeof(PlayerConnectionData));
                count = query.CalculateEntityCount();
            }
            
            return count;
        }
        
        /// <summary>
        /// Проверяет, является ли текущий мир сервером
        /// </summary>
        public bool IsServer()
        {
            return _isServer;
        }
        
        /// <summary>
        /// Проверяет, является ли текущий мир клиентом
        /// </summary>
        public bool IsClient()
        {
            return _isClient;
        }
        
        /// <summary>
        /// Проверяет, является ли текущий мир хостом
        /// </summary>
        public bool IsHost()
        {
            return _isHost;
        }
    }
}