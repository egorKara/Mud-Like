using Unity.Entities;
using Unity.Mathematics;
using Unity.Burst;
using Unity.NetCode;
using MudLike.Core.Constants;

namespace MudLike.Networking.Components
{
    /// <summary>
    /// Оптимизированный компонент синхронизации сети
    /// Использует Burst Compiler для максимальной производительности
    /// </summary>
    [BurstCompile]
    public struct OptimizedNetworkSyncComponent : IComponentData
    {
        /// <summary>
        /// ID сети
        /// </summary>
        public int NetworkId;
        
        /// <summary>
        /// Позиция для синхронизации
        /// </summary>
        public float3 Position;
        
        /// <summary>
        /// Скорость для синхронизации
        /// </summary>
        public float3 Velocity;
        
        /// <summary>
        /// Вращение для синхронизации
        /// </summary>
        public quaternion Rotation;
        
        /// <summary>
        /// Время последней синхронизации
        /// </summary>
        public float LastSyncTime;
        
        /// <summary>
        /// Интервал синхронизации
        /// </summary>
        public float SyncInterval;
        
        /// <summary>
        /// Порог изменения для синхронизации
        /// </summary>
        public float SyncThreshold;
        
        /// <summary>
        /// Флаг необходимости синхронизации
        /// </summary>
        public bool NeedsSync;
        
        /// <summary>
        /// Флаг активности
        /// </summary>
        public bool IsActive;
        
        /// <summary>
        /// Конструктор с параметрами по умолчанию
        /// </summary>
        public OptimizedNetworkSyncComponent(int networkId = 0)
        {
            NetworkId = networkId;
            Position = float3.zero;
            Velocity = float3.zero;
            Rotation = quaternion.identity;
            LastSyncTime = 0.0f;
            SyncInterval = SystemConstants.NETWORK_DEFAULT_SEND_RATE;
            SyncThreshold = SystemConstants.DETERMINISTIC_EPSILON;
            NeedsSync = false;
            IsActive = true;
        }
        
        /// <summary>
        /// Обновление состояния для синхронизации
        /// </summary>
        [BurstCompile]
        public void UpdateState(float3 newPosition, float3 newVelocity, quaternion newRotation, float currentTime)
        {
            if (!IsActive) return;
            
            // Проверка необходимости синхронизации
            var positionChanged = math.distance(Position, newPosition) > SyncThreshold;
            var velocityChanged = math.distance(Velocity, newVelocity) > SyncThreshold;
            var rotationChanged = math.distance(Rotation.value, newRotation.value) > SyncThreshold;
            var timeElapsed = currentTime - LastSyncTime > SyncInterval;
            
            if (positionChanged || velocityChanged || rotationChanged || timeElapsed)
            {
                Position = newPosition;
                Velocity = newVelocity;
                Rotation = newRotation;
                LastSyncTime = currentTime;
                NeedsSync = true;
            }
        }
        
        /// <summary>
        /// Проверка необходимости синхронизации
        /// </summary>
        [BurstCompile]
        public bool ShouldSync(float currentTime)
        {
            if (!IsActive) return false;
            
            return NeedsSync || (currentTime - LastSyncTime) > SyncInterval;
        }
        
        /// <summary>
        /// Отметка синхронизации как выполненной
        /// </summary>
        [BurstCompile]
        public void MarkSynced()
        {
            NeedsSync = false;
        }
        
        /// <summary>
        /// Интерполяция к целевому состоянию
        /// </summary>
        [BurstCompile]
        public void InterpolateTo(OptimizedNetworkSyncComponent target, float factor)
        {
            if (!IsActive) return;
            
            Position = math.lerp(Position, target.Position, factor);
            Velocity = math.lerp(Velocity, target.Velocity, factor);
            Rotation = math.slerp(Rotation, target.Rotation, factor);
        }
        
        /// <summary>
        /// Экстраполяция состояния
        /// </summary>
        [BurstCompile]
        public void Extrapolate(float deltaTime)
        {
            if (!IsActive) return;
            
            Position += Velocity * deltaTime;
        }
        
        /// <summary>
        /// Получение данных для отправки
        /// </summary>
        [BurstCompile]
        public NetworkSyncData GetSyncData()
        {
            return new NetworkSyncData
            {
                NetworkId = NetworkId,
                Position = Position,
                Velocity = Velocity,
                Rotation = Rotation,
                Timestamp = LastSyncTime
            };
        }
        
        /// <summary>
        /// Установка данных из сети
        /// </summary>
        [BurstCompile]
        public void SetSyncData(NetworkSyncData data, float currentTime)
        {
            if (!IsActive) return;
            
            Position = data.Position;
            Velocity = data.Velocity;
            Rotation = data.Rotation;
            LastSyncTime = currentTime;
            NeedsSync = false;
        }
        
        /// <summary>
        /// Сброс состояния синхронизации
        /// </summary>
        [BurstCompile]
        public void Reset()
        {
            Position = float3.zero;
            Velocity = float3.zero;
            Rotation = quaternion.identity;
            LastSyncTime = 0.0f;
            NeedsSync = false;
            IsActive = true;
        }
    }
    
    /// <summary>
    /// Структура данных для синхронизации сети
    /// </summary>
    [BurstCompile]
    public struct NetworkSyncData
    {
        public int NetworkId;
        public float3 Position;
        public float3 Velocity;
        public quaternion Rotation;
        public float Timestamp;
    }
}
