using Unity.Entities;
using Unity.Mathematics;

namespace MudLike.Terrain.Components
{
    /// <summary>
    /// Данные деформации террейна
    /// </summary>
    public struct DeformationData : IComponentData
    {
        /// <summary>
        /// Позиция деформации
        /// </summary>
        public float3 Position;
        
        /// <summary>
        /// Сила деформации
        /// </summary>
        public float Force;
        
        /// <summary>
        /// Радиус деформации
        /// </summary>
        public float Radius;
        
        /// <summary>
        /// Глубина деформации
        /// </summary>
        public float Depth;
        
        /// <summary>
        /// Время создания деформации
        /// </summary>
        public float Time;
        
        /// <summary>
        /// Длительность деформации
        /// </summary>
        public float Duration;
        
        /// <summary>
        /// Тип деформации
        /// </summary>
        public DeformationType Type;
        
        /// <summary>
        /// Интенсивность деформации
        /// </summary>
        public float Intensity;
        
        /// <summary>
        /// Восстановление деформации
        /// </summary>
        public float RecoveryRate;
        
        /// <summary>
        /// Активна ли деформация
        /// </summary>
        public bool IsActive;
    }
    
    /// <summary>
    /// Типы деформации террейна
    /// </summary>
    public enum DeformationType : byte
    {
        /// <summary>
        /// Деформация от колеса
        /// </summary>
        WheelTrack,
        
        /// <summary>
        /// Деформация от удара
        /// </summary>
        Impact,
        
        /// <summary>
        /// Деформация от воды
        /// </summary>
        WaterErosion,
        
        /// <summary>
        /// Деформация от ветра
        /// </summary>
        WindErosion,
        
        /// <summary>
        /// Деформация от взрыва
        /// </summary>
        Explosion,
        
        /// <summary>
        /// Деформация от строительства
        /// </summary>
        Construction,
        
        /// <summary>
        /// Естественная деформация
        /// </summary>
        Natural,
        
        /// <summary>
        /// Временная деформация
        /// </summary>
        Temporary
    }
}