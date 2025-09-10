using Unity.Entities;
using Unity.NetCode;
using Unity.Mathematics;
using MudLike.Vehicles.Components;

namespace MudLike.Networking.Components
{
    /// <summary>
    /// Сетевые данные грузовика для синхронизации
    /// </summary>
    [GhostComponent(PrefabType = GhostPrefabType.AllPredicted)]
    public struct NetworkedTruckData : IComponentData
    {
        /// <summary>
        /// Позиция грузовика
        /// </summary>
        [GhostField(Quantization = 1000)] // Точность до 1мм
        public float3 Position;
        
        /// <summary>
        /// Поворот грузовика
        /// </summary>
        [GhostField(Quantization = 1000)]
        public quaternion Rotation;
        
        /// <summary>
        /// Скорость грузовика
        /// </summary>
        [GhostField(Quantization = 100)]
        public float3 Velocity;
        
        /// <summary>
        /// Угловая скорость
        /// </summary>
        [GhostField(Quantization = 100)]
        public float3 AngularVelocity;
        
        /// <summary>
        /// Текущая передача
        /// </summary>
        [GhostField]
        public int CurrentGear;
        
        /// <summary>
        /// Обороты двигателя
        /// </summary>
        [GhostField(Quantization = 10)]
        public float EngineRPM;
        
        /// <summary>
        /// Текущая скорость в км/ч
        /// </summary>
        [GhostField(Quantization = 10)]
        public float CurrentSpeed;
        
        /// <summary>
        /// Угол поворота руля
        /// </summary>
        [GhostField(Quantization = 100)]
        public float SteeringAngle;
        
        /// <summary>
        /// Состояние двигателя
        /// </summary>
        [GhostField]
        public bool EngineRunning;
        
        /// <summary>
        /// Состояние ручного тормоза
        /// </summary>
        [GhostField]
        public bool HandbrakeOn;
        
        /// <summary>
        /// Блокировка дифференциалов
        /// </summary>
        [GhostField]
        public bool LockFrontDifferential;
        
        [GhostField]
        public bool LockMiddleDifferential;
        
        [GhostField]
        public bool LockRearDifferential;
        
        [GhostField]
        public bool LockCenterDifferential;
        
        /// <summary>
        /// Уровень топлива
        /// </summary>
        [GhostField(Quantization = 1000)]
        public float FuelLevel;
    }
}