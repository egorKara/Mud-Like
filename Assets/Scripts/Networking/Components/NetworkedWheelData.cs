using Unity.Entities;
using Unity.NetCode;
using Unity.Mathematics;
using MudLike.Vehicles.Components;

namespace MudLike.Networking.Components
{
    /// <summary>
    /// Сетевые данные колеса для синхронизации
    /// </summary>
    [GhostComponent(PrefabType = GhostPrefabType.AllPredicted)]
    public struct NetworkedWheelData : IComponentData
    {
        /// <summary>
        /// Позиция колеса
        /// </summary>
        [GhostField(Quantization = 1000)]
        public float3 Position;
        
        /// <summary>
        /// Поворот колеса
        /// </summary>
        [GhostField(Quantization = 1000)]
        public quaternion Rotation;
        
        /// <summary>
        /// Угловая скорость колеса
        /// </summary>
        [GhostField(Quantization = 100)]
        public float AngularVelocity;
        
        /// <summary>
        /// Угол поворота колеса
        /// </summary>
        [GhostField(Quantization = 100)]
        public float SteerAngle;
        
        /// <summary>
        /// Крутящий момент
        /// </summary>
        [GhostField(Quantization = 100)]
        public float Torque;
        
        /// <summary>
        /// Тормозной момент
        /// </summary>
        [GhostField(Quantization = 100)]
        public float BrakeTorque;
        
        /// <summary>
        /// Коэффициент сцепления
        /// </summary>
        [GhostField(Quantization = 1000)]
        public float TractionCoefficient;
        
        /// <summary>
        /// Глубина погружения в грязь
        /// </summary>
        [GhostField(Quantization = 1000)]
        public float SinkDepth;
        
        /// <summary>
        /// Скорость скольжения
        /// </summary>
        [GhostField(Quantization = 1000)]
        public float SlipRatio;
        
        /// <summary>
        /// Индекс колеса
        /// </summary>
        [GhostField]
        public int WheelIndex;
    }
}