using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using MudLike.Vehicles.Components;

namespace MudLike.Networking.Components
{
    /// <summary>
    /// Сетевые данные транспорта для Unity 6
    /// Оптимизированы для GhostSystem
    /// </summary>
    [GhostComponent(PrefabType = GhostPrefabType.All, SendTypeOptimization = GhostSendType.AllClients)]
    public struct NetworkVehicle : IComponentData
    {
        /// <summary>
        /// Конфигурация транспорта
        /// </summary>
        [GhostField]
        public VehicleConfig Config;
        
        /// <summary>
        /// Физика транспорта
        /// </summary>
        [GhostField(Quantization = 100, Interpolate = true)]
        public VehiclePhysics Physics;
        
        /// <summary>
        /// Ввод транспорта
        /// </summary>
        [GhostField]
        public VehicleInput Input;
        
        /// <summary>
        /// Данные двигателя
        /// </summary>
        [GhostField(Quantization = 1000)]
        public EngineData Engine;
        
        /// <summary>
        /// Данные трансмиссии
        /// </summary>
        [GhostField]
        public TransmissionData Transmission;
        
        /// <summary>
        /// Время последнего обновления
        /// </summary>
        [GhostField(Quantization = 1000)]
        public float LastUpdateTime;
        
        /// <summary>
        /// Данные изменились
        /// </summary>
        [GhostField]
        public bool HasChanged;
        
        /// <summary>
        /// Приоритет синхронизации
        /// </summary>
        [GhostField]
        public byte SyncPriority;
        
        /// <summary>
        /// Флаг интерполяции
        /// </summary>
        [GhostField]
        public bool EnableInterpolation;
        
        /// <summary>
        /// Тип транспорта
        /// </summary>
        [GhostField]
        public byte VehicleType;
        
        /// <summary>
        /// Состояние транспорта
        /// </summary>
        [GhostField]
        public byte VehicleState;
    }
    
    /// <summary>
    /// Сетевые данные деформации для Unity 6
    /// </summary>
    [GhostComponent(PrefabType = GhostPrefabType.All, SendTypeOptimization = GhostSendType.AllClients)]
    public struct NetworkDeformation : IComponentData
    {
        /// <summary>
        /// Данные деформации
        /// </summary>
        [GhostField(Quantization = 1000)]
        public DeformationData Deformation;
        
        /// <summary>
        /// Время последнего обновления
        /// </summary>
        [GhostField(Quantization = 1000)]
        public float LastUpdateTime;
        
        /// <summary>
        /// Деформация изменилась
        /// </summary>
        [GhostField]
        public bool HasChanged;
        
        /// <summary>
        /// Приоритет синхронизации
        /// </summary>
        [GhostField]
        public byte SyncPriority;
    }
    
    /// <summary>
    /// Сетевые данные грязи для Unity 6
    /// </summary>
    [GhostComponent(PrefabType = GhostPrefabType.All, SendTypeOptimization = GhostSendType.AllClients)]
    public struct NetworkMud : IComponentData
    {
        /// <summary>
        /// Данные грязи
        /// </summary>
        [GhostField(Quantization = 1000)]
        public MudData Mud;
        
        /// <summary>
        /// Время последнего обновления
        /// </summary>
        [GhostField(Quantization = 1000)]
        public float LastUpdateTime;
        
        /// <summary>
        /// Грязь изменилась
        /// </summary>
        [GhostField]
        public bool HasChanged;
        
        /// <summary>
        /// Приоритет синхронизации
        /// </summary>
        [GhostField]
        public byte SyncPriority;
    }
}