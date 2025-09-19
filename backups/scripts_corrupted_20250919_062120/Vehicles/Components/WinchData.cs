using Unity.Entities;
using Unity.Mathematics;

namespace MudLike.Vehicles.Components
{
    /// <summary>
    /// Данные лебедки
    /// </summary>
    public struct WinchData : IComponentData
    {
        /// <summary>
        /// Лебедка активна
        /// </summary>
        public bool IsActive;
        
        /// <summary>
        /// Лебедка развернута
        /// </summary>
        public bool IsDeployed;
        
        /// <summary>
        /// Лебедка подключена к объекту
        /// </summary>
        public bool IsConnected;
        
        /// <summary>
        /// Длина троса (м)
        /// </summary>
        public float CableLength;
        
        /// <summary>
        /// Максимальная длина троса (м)
        /// </summary>
        public float MaxCableLength;
        
        /// <summary>
        /// Сила лебедки (Н)
        /// </summary>
        public float WinchForce;
        
        /// <summary>
        /// Максимальная сила лебедки (Н)
        /// </summary>
        public float MaxWinchForce;
        
        /// <summary>
        /// Скорость намотки троса (м/с)
        /// </summary>
        public float CableSpeed;
        
        /// <summary>
        /// Позиция крепления троса
        /// </summary>
        public float3 AttachmentPoint;
        
        /// <summary>
        /// Позиция подключения троса
        /// </summary>
        public float3 ConnectionPoint;
        
        /// <summary>
        /// Направление троса
        /// </summary>
        public float3 CableDirection;
        
        /// <summary>
        /// Напряжение троса (0-1)
        /// </summary>
        public float CableTension;
        
        /// <summary>
        /// Прочность троса (0-1)
        /// </summary>
        public float CableStrength;
        
        /// <summary>
        /// Износ троса (0-1)
        /// </summary>
        public float CableWear;
        
        /// <summary>
        /// Лебедка требует обновления
        /// </summary>
        public bool NeedsUpdate;
    }
    
    /// <summary>
    /// Данные троса лебедки
    /// </summary>
    public struct WinchCableData : IComponentData
    {
        /// <summary>
        /// Позиция начала троса
        /// </summary>
        public float3 StartPosition;
        
        /// <summary>
        /// Позиция конца троса
        /// </summary>
        public float3 EndPosition;
        
        /// <summary>
        /// Длина троса (м)
        /// </summary>
        public float Length;
        
        /// <summary>
        /// Напряжение троса (0-1)
        /// </summary>
        public float Tension;
        
        /// <summary>
        /// Прочность троса (0-1)
        /// </summary>
        public float Strength;
        
        /// <summary>
        /// Износ троса (0-1)
        /// </summary>
        public float Wear;
        
        /// <summary>
        /// Трос активен
        /// </summary>
        public bool IsActive;
        
        /// <summary>
        /// Трос требует обновления
        /// </summary>
        public bool NeedsUpdate;
    }
    
    /// <summary>
    /// Данные подключения лебедки
    /// </summary>
    public struct WinchConnectionData : IComponentData
    {
        /// <summary>
        /// ID подключенного объекта
        /// </summary>
        public int ConnectedObjectId;
        
        /// <summary>
        /// Тип подключенного объекта
        /// </summary>
        public WinchConnectionType ConnectionType;
        
        /// <summary>
        /// Позиция подключения
        /// </summary>
        public float3 ConnectionPosition;
        
        /// <summary>
        /// Сила подключения (Н)
        /// </summary>
        public float ConnectionForce;
        
        /// <summary>
        /// Максимальная сила подключения (Н)
        /// </summary>
        public float MaxConnectionForce;
        
        /// <summary>
        /// Прочность подключения (0-1)
        /// </summary>
        public float ConnectionStrength;
        
        /// <summary>
        /// Износ подключения (0-1)
        /// </summary>
        public float ConnectionWear;
        
        /// <summary>
        /// Подключение активно
        /// </summary>
        public bool IsActive;
        
        /// <summary>
        /// Подключение требует обновления
        /// </summary>
        public bool NeedsUpdate;
    }
    
    /// <summary>
    /// Тип подключения лебедки
    /// </summary>
    public enum WinchConnectionType
    {
        None,           // Нет подключения
        Vehicle,        // Подключение к транспорту
        Tree,           // Подключение к дереву
        Rock,           // Подключение к камню
        Building,       // Подключение к зданию
        Ground,         // Подключение к земле
        Water,          // Подключение к воде
        Player          // Подключение к игроку
    }
}