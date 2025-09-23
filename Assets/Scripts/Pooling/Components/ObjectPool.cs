using Unity.Entities;
using Unity.Mathematics;

namespace MudLike.Pooling.Components
{
    /// <summary>
    /// Данные пула объектов
    /// </summary>
    public struct Pool : IComponentData
    {
        /// <summary>
        /// ID пула
        /// </summary>
        public int PoolId;
        
        /// <summary>
        /// Тип объекта в пуле
        /// </summary>
        public PoolObjectType ObjectType;
        
        /// <summary>
        /// Максимальный размер пула
        /// </summary>
        public int MaxPoolSize;
        
        /// <summary>
        /// Текущий размер пула
        /// </summary>
        public int CurrentPoolSize;
        
        /// <summary>
        /// Количество активных объектов
        /// </summary>
        public int ActiveCount;
        
        /// <summary>
        /// Количество неактивных объектов
        /// </summary>
        public int InactiveCount;
        
        /// <summary>
        /// Время жизни объекта (секунды)
        /// </summary>
        public float ObjectLifetime;
        
        /// <summary>
        /// Автоматическое уничтожение объектов
        /// </summary>
        public bool AutoDestroy;
        
        /// <summary>
        /// Пул активен
        /// </summary>
        public bool IsActive;
        
        /// <summary>
        /// Пул требует обновления
        /// </summary>
        public bool NeedsUpdate;
    }
    
    /// <summary>
    /// Данные объекта в пуле
    /// </summary>
    public struct PooledObject : IComponentData
    {
        /// <summary>
        /// ID пула
        /// </summary>
        public int PoolId;
        
        /// <summary>
        /// ID объекта в пуле
        /// </summary>
        public int ObjectId;
        
        /// <summary>
        /// Объект активен
        /// </summary>
        public bool IsActive;
        
        /// <summary>
        /// Время создания объекта
        /// </summary>
        public float CreationTime;
        
        /// <summary>
        /// Время последнего использования
        /// </summary>
        public float LastUsedTime;
        
        /// <summary>
        /// Количество использований
        /// </summary>
        public int UseCount;
        
        /// <summary>
        /// Объект требует обновления
        /// </summary>
        public bool NeedsUpdate;
    }
    
    /// <summary>
    /// Данные частицы в пуле
    /// </summary>
    public struct PooledParticle : IComponentData
    {
        /// <summary>
        /// ID пула частиц
        /// </summary>
        public int ParticlePoolId;
        
        /// <summary>
        /// Тип частицы
        /// </summary>
        public ParticleType ParticleType;
        
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
        /// Цвет частицы
        /// </summary>
        public float4 Color;
        
        /// <summary>
        /// Прозрачность частицы
        /// </summary>
        public float Alpha;
        
        /// <summary>
        /// Время жизни частицы
        /// </summary>
        public float Lifetime;
        
        /// <summary>
        /// Максимальное время жизни
        /// </summary>
        public float MaxLifetime;
        
        /// <summary>
        /// Частица активна
        /// </summary>
        public bool IsActive;
        
        /// <summary>
        /// Частица требует обновления
        /// </summary>
        public bool NeedsUpdate;
    }
    
    /// <summary>
    /// Тип объекта в пуле
    /// </summary>
    public enum PoolObjectType
    {
        Particle,       // Частица
        Effect,         // Эффект
        Sound,          // Звук
        UI,             // UI элемент
        Physics,        // Физический объект
        Render,         // Рендер объект
        Animation,      // Анимация
        Light,          // Свет
        Camera,         // Камера
        Trigger         // Триггер
    }
    
    /// <summary>
    /// Тип частицы
    /// </summary>
    public enum ParticleType
    {
        Mud,            // Грязь
        Water,          // Вода
        Sand,           // Песок
        Grass,          // Трава
        Stone,          // Камень
        Dust,           // Пыль
        Smoke,          // Дым
        Sparks,         // Искры
        Fire,           // Огонь
        Explosion       // Взрыв
    }
}