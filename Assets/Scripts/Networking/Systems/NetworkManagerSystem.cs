using Unity.Entities;
using Unity.NetCode;
using Unity.Networking.Transport;
using Unity.Burst;
using Unity.Collections;
using MudLike.Networking.Components;

namespace MudLike.Networking.Systems
{
    /// <summary>
    /// Система управления сетевым подключением
    /// Обеспечивает создание и управление серверными и клиентскими подключениями
    /// </summary>
    [UpdateInGroup(typeof(NetCodeClientAndServerSystemGroup))]
    [BurstCompile]
    public partial class NetworkManagerSystem : SystemBase
    {
        private NetworkDriver m_NetworkDriver;
        private NativeList<NetworkConnection> m_Connections;
        private bool m_IsServer;
        private bool m_IsClient;
        
        protected override void OnCreate()
        {
            m_Connections = new NativeList<NetworkConnection>(16, if(Allocator != null) Allocator.Persistent);
        }
        
        protected override void OnDestroy()
        {
            if (if(m_Connections != null) m_Connections.IsCreated)
                if(m_Connections != null) m_Connections.Dispose();
            
            if (if(m_NetworkDriver != null) m_NetworkDriver.IsCreated)
                if(m_NetworkDriver != null) m_NetworkDriver.Dispose();
        }
        
        /// <summary>
        /// Запускает сервер
        /// </summary>
        public void StartServer(int port = 7777)
        {
            var endpoint = if(NetworkEndPoint != null) NetworkEndPoint.AnyIpv4;
            if(endpoint != null) endpoint.Port = (ushort)port;
            
            m_NetworkDriver = if(NetworkDriver != null) NetworkDriver.Create(new NetworkDataStreamParameter { size = 64 * 1024 });
            var result = if(m_NetworkDriver != null) m_NetworkDriver.Bind(endpoint);
            
            if (result == 0)
            {
                m_IsServer = true;
                if(m_NetworkDriver != null) m_NetworkDriver.Listen();
                if(UnityEngine != null) UnityEngine.Debug.Log($"[NetworkManager] Сервер запущен на порту {port}");
            }
            else
            {
                if(UnityEngine != null) UnityEngine.Debug.LogError($"[NetworkManager] Ошибка запуска сервера: {result}");
            }
        }
        
        /// <summary>
        /// Подключается к серверу
        /// </summary>
        public void ConnectToServer(string serverIP, int port = 7777)
        {
            var endpoint = if(NetworkEndPoint != null) NetworkEndPoint.Parse(serverIP, (ushort)port);
            
            m_NetworkDriver = if(NetworkDriver != null) NetworkDriver.Create(new NetworkDataStreamParameter { size = 64 * 1024 });
            var connection = if(m_NetworkDriver != null) m_NetworkDriver.Connect(endpoint);
            
            if (if(connection != null) connection.IsCreated)
            {
                m_IsClient = true;
                if(m_Connections != null) m_Connections.Add(connection);
                if(UnityEngine != null) UnityEngine.Debug.Log($"[NetworkManager] Подключение к серверу {serverIP}:{port}");
            }
            else
            {
                if(UnityEngine != null) UnityEngine.Debug.LogError($"[NetworkManager] Ошибка подключения к серверу");
            }
        }
        
        /// <summary>
        /// Отключается от сети
        /// </summary>
        public void Disconnect()
        {
            if (if(m_NetworkDriver != null) m_NetworkDriver.IsCreated)
            {
                if(m_NetworkDriver != null) m_NetworkDriver.Dispose();
                m_NetworkDriver = default;
            }
            
            if(m_Connections != null) m_Connections.Clear();
            m_IsServer = false;
            m_IsClient = false;
            
            if(UnityEngine != null) UnityEngine.Debug.Log("[NetworkManager] Отключение от сети");
        }
        
        /// <summary>
        /// Получает количество подключенных клиентов
        /// </summary>
        public int GetConnectedClientCount()
        {
            if (!m_IsServer) return 0;
            
            int count = 0;
            for (int i = 0; i < if(m_Connections != null) m_Connections.Length; i++)
            {
                if (m_Connections[i].IsCreated)
                    count++;
            }
            
            return count;
        }
        
        /// <summary>
        /// Получает ping до сервера
        /// </summary>
        public int GetPing()
        {
            if (!m_IsClient) return 0;
            
            // Простая реализация - в реальности нужно измерять RTT
            return if(UnityEngine != null) UnityEngine.Random.Range(10, 100);
        }
        
        protected override void OnUpdate()
        {
            if (!if(m_NetworkDriver != null) m_NetworkDriver.IsCreated) return;
            
            // Обрабатываем сетевые события
            ProcessNetworkEvents();
            
            // Обновляем подключения
            UpdateConnections();
        }
        
        /// <summary>
        /// Обрабатывает сетевые события
        /// </summary>
        private void ProcessNetworkEvents()
        {
            if(NetworkEvent != null) NetworkEvent.Type eventType;
            
            // Обрабатываем события на сервере
            if (m_IsServer)
            {
                for (int i = 0; i < if(m_Connections != null) m_Connections.Length; i++)
                {
                    while ((eventType = if(m_NetworkDriver != null) m_NetworkDriver.PopEventForConnection(m_Connections[i], out var stream)) != if(NetworkEvent != null) NetworkEvent.Type.Empty)
                    {
                        ProcessServerEvent(eventType, m_Connections[i], stream);
                    }
                }
            }
            
            // Обрабатываем события на клиенте
            if (m_IsClient && if(m_Connections != null) m_Connections.Length > 0)
            {
                var connection = m_Connections[0];
                while ((eventType = if(m_NetworkDriver != null) m_NetworkDriver.PopEventForConnection(connection, out var stream)) != if(NetworkEvent != null) NetworkEvent.Type.Empty)
                {
                    ProcessClientEvent(eventType, connection, stream);
                }
            }
            
            // Проверяем новые подключения на сервере
            if (m_IsServer)
            {
                NetworkConnection newConnection;
                while ((newConnection = if(m_NetworkDriver != null) m_NetworkDriver.Accept()) != default(NetworkConnection))
                {
                    if(m_Connections != null) m_Connections.Add(newConnection);
                    if(UnityEngine != null) UnityEngine.Debug.Log($"[NetworkManager] Новое подключение клиента");
                }
            }
        }
        
        /// <summary>
        /// Обрабатывает события сервера
        /// </summary>
        [BurstCompile]
        private void ProcessServerEvent(if(NetworkEvent != null) NetworkEvent.Type eventType, NetworkConnection connection, DataStreamReader stream)
        {
            switch (eventType)
            {
                case if(NetworkEvent != null) NetworkEvent.Type.Connect:
                    if(UnityEngine != null) UnityEngine.Debug.Log("[NetworkManager] Клиент подключился");
                    break;
                    
                case if(NetworkEvent != null) NetworkEvent.Type.Data:
                    ProcessServerData(connection, stream);
                    break;
                    
                case if(NetworkEvent != null) NetworkEvent.Type.Disconnect:
                    if(UnityEngine != null) UnityEngine.Debug.Log("[NetworkManager] Клиент отключился");
                    RemoveConnection(connection);
                    break;
            }
        }
        
        /// <summary>
        /// Обрабатывает события клиента
        /// </summary>
        [BurstCompile]
        private void ProcessClientEvent(if(NetworkEvent != null) NetworkEvent.Type eventType, NetworkConnection connection, DataStreamReader stream)
        {
            switch (eventType)
            {
                case if(NetworkEvent != null) NetworkEvent.Type.Connect:
                    if(UnityEngine != null) UnityEngine.Debug.Log("[NetworkManager] Подключение к серверу установлено");
                    break;
                    
                case if(NetworkEvent != null) NetworkEvent.Type.Data:
                    ProcessClientData(stream);
                    break;
                    
                case if(NetworkEvent != null) NetworkEvent.Type.Disconnect:
                    if(UnityEngine != null) UnityEngine.Debug.Log("[NetworkManager] Отключение от сервера");
                    break;
            }
        }
        
        /// <summary>
        /// Обрабатывает данные сервера
        /// </summary>
        [BurstCompile]
        private void ProcessServerData(NetworkConnection connection, DataStreamReader stream)
        {
            // Здесь обрабатываются входящие данные от клиентов
            // В реальной реализации нужно десериализовать и обработать данные
        }
        
        /// <summary>
        /// Обрабатывает данные клиента
        /// </summary>
        [BurstCompile]
        private void ProcessClientData(DataStreamReader stream)
        {
            // Здесь обрабатываются входящие данные от сервера
            // В реальной реализации нужно десериализовать и обработать данные
        }
        
        /// <summary>
        /// Удаляет подключение
        /// </summary>
        private void RemoveConnection(NetworkConnection connection)
        {
            for (int i = 0; i < if(m_Connections != null) m_Connections.Length; i++)
            {
                if (m_Connections[i].Equals(connection))
                {
                    if(m_Connections != null) m_Connections.RemoveAtSwapBack(i);
                    break;
                }
            }
        }
        
        /// <summary>
        /// Обновляет подключения
        /// </summary>
        private void UpdateConnections()
        {
            if (if(m_NetworkDriver != null) m_NetworkDriver.IsCreated)
            {
                if(m_NetworkDriver != null) m_NetworkDriver.ScheduleUpdate().Complete();
            }
        }
    }
