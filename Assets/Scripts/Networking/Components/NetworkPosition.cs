using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;

namespace MudLike.Networking.Components
{
    /// <summary>
    /// Сетевая позиция сущности для Unity 6
    /// Оптимизирована для новых возможностей NFE
    /// </summary>
    [GhostComponent(PrefabType = GhostPrefabType.All, SendTypeOptimization = GhostSendType.AllClients)]
    public struct NetworkPosition : IComponentData
    {
        /// <summary>
        /// Позиция сущности
        /// </summary>
        [GhostField(Quantization = 1000, Interpolate = true)]
        public float3 Value;
        
        /// <summary>
        /// Поворот сущности
        /// </summary>
        [GhostField(Quantization = 1000, Interpolate = true)]
        public quaternion Rotation;
        
        /// <summary>
        /// Скорость сущности
        /// </summary>
        [GhostField(Quantization = 100, Interpolate = true)]
        public float3 Velocity;
        
        /// <summary>
        /// Угловая скорость сущности
        /// </summary>
        [GhostField(Quantization = 100, Interpolate = true)]
        public float3 AngularVelocity;
        
        /// <summary>
        /// Время последнего обновления
        /// </summary>
        [GhostField(Quantization = 1000)]
        public float LastUpdateTime;
        
        /// <summary>
        /// Позиция изменилась
        /// </summary>
        [GhostField]
        public bool HasChanged;
        
        /// <summary>
        /// Тик команды
        /// </summary>
        [GhostField]
        public uint Tick;
        
        /// <summary>
        /// Приоритет синхронизации (Unity 6)
        /// </summary>
        [GhostField]
        public byte SyncPriority;
        
        /// <summary>
        /// Флаг интерполяции (Unity 6)
        /// </summary>
        [GhostField]
        public bool EnableInterpolation;
    }
}