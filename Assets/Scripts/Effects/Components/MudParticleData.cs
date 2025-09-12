using Unity.Entities;
using Unity.Mathematics;

namespace MudLike.Effects.Components
{
    /// <summary>
    /// Данные частицы грязи
    /// </summary>
    public struct MudParticleData : IComponentData
    {
        /// <summary>
        /// Позиция частицы
        /// </summary>
        public float3 Position;
        
        /// <summary>
        /// Скорость частицы
        /// </summary>
        public float3 Velocity;
        
        /// <summary>
        /// Ускорение частицы
        /// </summary>
        public float3 Acceleration;
        
        /// <summary>
        /// Размер частицы
        /// </summary>
        public float Size;
        
        /// <summary>
        /// Масса частицы
        /// </summary>
        public float Mass;
        
        /// <summary>
        /// Время жизни частицы
        /// </summary>
        public float Lifetime;
        
        /// <summary>
        /// Максимальное время жизни
        /// </summary>
        public float MaxLifetime;
        
        /// <summary>
        /// Прозрачность частицы
        /// </summary>
        public float Alpha;
        
        /// <summary>
        /// Цвет частицы
        /// </summary>
        public float4 Color;
        
        /// <summary>
        /// Тип частицы
        /// </summary>
        public ParticleType Type;
        
        /// <summary>
        /// Активна ли частица
        /// </summary>
        public bool IsActive;
        
        /// <summary>
        /// Прилипла ли к поверхности
        /// </summary>
        public bool IsStuck;
        
        /// <summary>
        /// Скорость высыхания
        /// </summary>
        public float DryingRate;
        
        /// <summary>
        /// Влажность частицы
        /// </summary>
        public float Moisture;
        
        /// <summary>
        /// Температура частицы
        /// </summary>
        public float Temperature;
        
        /// <summary>
        /// Вязкость частицы
        /// </summary>
        public float Viscosity;
        
        /// <summary>
        /// Плотность частицы
        /// </summary>
        public float Density;
        
        /// <summary>
        /// Коэффициент трения
        /// </summary>
        public float Friction;
        
        /// <summary>
        /// Эластичность частицы
        /// </summary>
        public float Elasticity;
        
        /// <summary>
        /// Скорость вращения
        /// </summary>
        public float3 AngularVelocity;
        
        /// <summary>
        /// Угол поворота
        /// </summary>
        public quaternion Rotation;
        
        /// <summary>
        /// Масштаб частицы
        /// </summary>
        public float3 Scale;
        
        /// <summary>
        /// Сила притяжения к земле
        /// </summary>
        public float Gravity;
        
        /// <summary>
        /// Сопротивление воздуха
        /// </summary>
        public float AirResistance;
        
        /// <summary>
        /// Время последнего обновления
        /// </summary>
        public float LastUpdateTime;
    }
    
    /// <summary>
    /// Типы частиц
    /// </summary>
    public enum ParticleType
    {
        Mud,        // Грязь
        Water,      // Вода
        Sand,       // Песок
        Grass,      // Трава
        Stone,      // Камень
        Dust,       // Пыль
        Smoke,      // Дым
        Spark       // Искры
    }
}