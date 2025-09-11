using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;

namespace MudLike.Networking.Components
{
    /// <summary>
    /// Сетевая позиция сущности
    /// </summary>
    public struct NetworkPosition : IComponentData, ICommandData
    {
        /// <summary>
        /// Позиция сущности
        /// </summary>
        public float3 Value;
        
        /// <summary>
        /// Поворот сущности
        /// </summary>
        public quaternion Rotation;
        
        /// <summary>
        /// Скорость сущности
        /// </summary>
        public float3 Velocity;
        
        /// <summary>
        /// Угловая скорость сущности
        /// </summary>
        public float3 AngularVelocity;
        
        /// <summary>
        /// Время последнего обновления
        /// </summary>
        public float LastUpdateTime;
        
        /// <summary>
        /// Позиция изменилась
        /// </summary>
        public bool HasChanged;
        
        /// <summary>
        /// Тик команды
        /// </summary>
        public uint Tick => 0; // Будет установлен Netcode
    }
}