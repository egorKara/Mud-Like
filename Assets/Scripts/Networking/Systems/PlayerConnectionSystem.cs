using Unity.Entities;
using Unity.NetCode;
using Unity.Collections;
using MudLike.Networking.Components;

namespace MudLike.Networking.Systems
{
    /// <summary>
    /// Система управления подключениями игроков
    /// </summary>
    [UpdateInGroup(typeof(NetCodeReceiveSystemGroup))]
    public partial class PlayerConnectionSystem : SystemBase
    {
        /// <summary>
        /// Обрабатывает подключения игроков
        /// </summary>
        protected override void OnUpdate()
        {
            if (!HasSingleton<NetworkStreamInGame>()) return;
            
            // Обрабатываем новые подключения
            ProcessNewConnections();
            
            // Обрабатываем отключения
            ProcessDisconnections();
            
            // Обновляем статус игроков
            UpdatePlayerStatus();
        }
        
        /// <summary>
        /// Обрабатывает новые подключения
        /// </summary>
        private void ProcessNewConnections()
        {
            // Находим игроков без данных подключения
            Entities
                .WithAll<PlayerTag>()
                .WithNone<PlayerConnectionData>()
                .ForEach((Entity entity) =>
                {
                    // Добавляем данные подключения
                    var connectionData = new PlayerConnectionData
                    {
                        PlayerID = entity.Index,
                        PlayerName = new FixedString64Bytes($"Player_{entity.Index}"),
                        Ping = 0,
                        Status = PlayerConnectionStatus.Connected,
                        ConnectionTime = SystemAPI.Time.ElapsedTime,
                        LastActivityTime = SystemAPI.Time.ElapsedTime
                    };
                    
                    EntityManager.AddComponentData(entity, connectionData);
                }).WithoutBurst().Run();
        }
        
        /// <summary>
        /// Обрабатывает отключения
        /// </summary>
        private void ProcessDisconnections()
        {
            // Находим игроков с истекшим временем активности
            double currentTime = SystemAPI.Time.ElapsedTime;
            double timeout = 30.0; // 30 секунд таймаут
            
            Entities
                .WithAll<PlayerConnectionData>()
                .ForEach((Entity entity, ref PlayerConnectionData connectionData) =>
                {
                    if (currentTime - connectionData.LastActivityTime > timeout)
                    {
                        connectionData.Status = PlayerConnectionStatus.Disconnected;
                        // В реальной игре здесь будет логика удаления игрока
                    }
                }).WithoutBurst().Run();
        }
        
        /// <summary>
        /// Обновляет статус игроков
        /// </summary>
        private void UpdatePlayerStatus()
        {
            double currentTime = SystemAPI.Time.ElapsedTime;
            
            Entities
                .WithAll<PlayerConnectionData>()
                .ForEach((ref PlayerConnectionData connectionData) =>
                {
                    // Обновляем время последней активности
                    connectionData.LastActivityTime = currentTime;
                    
                    // Обновляем ping (упрощенно)
                    connectionData.Ping = (int)(SystemAPI.Time.DeltaTime * 1000);
                }).WithoutBurst().Run();
        }
    }
}