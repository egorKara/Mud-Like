using Unity.Entities;
using Unity.Mathematics;

namespace MudLike.Terrain.Components
{
    /// <summary>
    /// Данные деформации террейна
    /// </summary>
    public struct Deformation : IComponentData
    {
        /// <summary>
        /// Позиция деформации в мире
        /// </summary>
        public float3 Position;
        
        /// <summary>
        /// Радиус деформации
        /// </summary>
        public float Radius;
        
        /// <summary>
        /// Глубина деформации
        /// </summary>
        public float Depth;
        
        /// <summary>
        /// Сила деформации
        /// </summary>
        public float Force;
        
        /// <summary>
        /// Тип деформации
        /// </summary>
        public DeformationType Type;
        
        /// <summary>
        /// Время создания деформации
        /// </summary>
        public float Time;
        
        /// <summary>
        /// Активна ли деформация
        /// </summary>
        public bool IsActive;
        
        /// <summary>
        /// Применена ли деформация
        /// </summary>
        public bool IsApplied;
        
        /// <summary>
        /// ID источника деформации
        /// </summary>
        public int SourceId;
    }
    
    /// <summary>
    /// Тип деформации террейна
    /// </summary>
    public enum DeformationType : byte
    {
        /// <summary>
        /// Вдавление (от колес)
        /// </summary>
        Indentation,
        
        /// <summary>
        /// Поднятие (от взрыва)
        /// </summary>
        Elevation,
        
        /// <summary>
        /// Сглаживание (от времени)
        /// </summary>
        Smoothing,
        
        /// <summary>
        /// Эрозия (от воды)
        /// </summary>
        Erosion
    }
    
    /// <summary>
    /// Данные грязи в точке террейна
    /// </summary>
    public struct Mud : IComponentData
    {
        /// <summary>
        /// Позиция грязи
        /// </summary>
        public float3 Position;
        
        /// <summary>
        /// Уровень грязи (0-1)
        /// </summary>
        public float MudLevel;
        
        /// <summary>
        /// Вязкость грязи
        /// </summary>
        public float Viscosity;
        
        /// <summary>
        /// Сопротивление движению
        /// </summary>
        public float TractionModifier;
        
        /// <summary>
        /// Глубина погружения
        /// </summary>
        public float SinkDepth;
        
        /// <summary>
        /// Тип поверхности
        /// </summary>
        public SurfaceType SurfaceType;
        
        /// <summary>
        /// Время последнего обновления
        /// </summary>
        public float LastUpdateTime;
    }
    
    /// <summary>
    /// Тип поверхности
    /// </summary>
    public enum SurfaceType : byte
    {
        /// <summary>
        /// Сухая земля
        /// </summary>
        DryGround,
        
        /// <summary>
        /// Влажная земля
        /// </summary>
        WetGround,
        
        /// <summary>
        /// Грязь
        /// </summary>
        Mud,
        
        /// <summary>
        /// Глубокая грязь
        /// </summary>
        DeepMud,
        
        /// <summary>
        /// Вода
        /// </summary>
        Water,
        
        /// <summary>
        /// Камень
        /// </summary>
        Rock,
        
        /// <summary>
        /// Песок
        /// </summary>
        Sand
    }
}