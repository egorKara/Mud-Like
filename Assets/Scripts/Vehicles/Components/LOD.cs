using Unity.Entities;
using Unity.Mathematics;

namespace MudLike.Vehicles.Components
{
    /// <summary>
    /// Данные системы LOD (Level of Detail)
    /// </summary>
    public struct LOD : IComponentData
    {
        /// <summary>
        /// Текущий уровень детализации (0-3)
        /// </summary>
        public int CurrentLOD;
        
        /// <summary>
        /// Максимальный уровень детализации
        /// </summary>
        public int MaxLOD;
        
        /// <summary>
        /// Расстояние до камеры
        /// </summary>
        public float DistanceToCamera;
        
        /// <summary>
        /// Пороги расстояния для переключения LOD
        /// </summary>
        public float4 LODDistances;
        
        /// <summary>
        /// Скорость переключения LOD
        /// </summary>
        public float LODSwitchSpeed;
        
        /// <summary>
        /// Время последнего переключения LOD
        /// </summary>
        public float LastLODSwitchTime;
        
        /// <summary>
        /// LOD активен
        /// </summary>
        public bool IsActive;
        
        /// <summary>
        /// LOD требует обновления
        /// </summary>
        public bool NeedsUpdate;
    }
    
    /// <summary>
    /// Данные LOD для рендеринга
    /// </summary>
    public struct LODRender : IComponentData
    {
        /// <summary>
        /// Количество полигонов для текущего LOD
        /// </summary>
        public int PolygonCount;
        
        /// <summary>
        /// Количество текстур для текущего LOD
        /// </summary>
        public int TextureCount;
        
        /// <summary>
        /// Размер текстур для текущего LOD
        /// </summary>
        public int TextureSize;
        
        /// <summary>
        /// Количество анимаций для текущего LOD
        /// </summary>
        public int AnimationCount;
        
        /// <summary>
        /// Качество теней для текущего LOD
        /// </summary>
        public ShadowQuality ShadowQuality;
        
        /// <summary>
        /// Качество отражений для текущего LOD
        /// </summary>
        public ReflectionQuality ReflectionQuality;
        
        /// <summary>
        /// Качество частиц для текущего LOD
        /// </summary>
        public ParticleQuality ParticleQuality;
        
        /// <summary>
        /// Рендеринг активен
        /// </summary>
        public bool IsRendering;
        
        /// <summary>
        /// Рендеринг требует обновления
        /// </summary>
        public bool NeedsUpdate;
    }
    
    /// <summary>
    /// Данные LOD для физики
    /// </summary>
    public struct LODPhysics : IComponentData
    {
        /// <summary>
        /// Частота обновления физики (Гц)
        /// </summary>
        public float PhysicsUpdateRate;
        
        /// <summary>
        /// Количество итераций физики
        /// </summary>
        public int PhysicsIterations;
        
        /// <summary>
        /// Точность коллизий
        /// </summary>
        public CollisionAccuracy CollisionAccuracy;
        
        /// <summary>
        /// Детализация подвески
        /// </summary>
        public SuspensionDetail SuspensionDetail;
        
        /// <summary>
        /// Детализация колес
        /// </summary>
        public WheelDetail WheelDetail;
        
        /// <summary>
        /// Физика активна
        /// </summary>
        public bool IsActive;
        
        /// <summary>
        /// Физика требует обновления
        /// </summary>
        public bool NeedsUpdate;
    }
    
    /// <summary>
    /// Качество теней
    /// </summary>
    public enum ShadowQuality
    {
        None,           // Без теней
        Low,            // Низкое качество
        Medium,         // Среднее качество
        High,           // Высокое качество
        Ultra           // Ультра качество
    }
    
    /// <summary>
    /// Качество отражений
    /// </summary>
    public enum ReflectionQuality
    {
        None,           // Без отражений
        Low,            // Низкое качество
        Medium,         // Среднее качество
        High,           // Высокое качество
        Ultra           // Ультра качество
    }
    
    /// <summary>
    /// Качество частиц
    /// </summary>
    public enum ParticleQuality
    {
        None,           // Без частиц
        Low,            // Низкое качество
        Medium,         // Среднее качество
        High,           // Высокое качество
        Ultra           // Ультра качество
    }
    
    /// <summary>
    /// Точность коллизий
    /// </summary>
    public enum CollisionAccuracy
    {
        Low,            // Низкая точность
        Medium,         // Средняя точность
        High,           // Высокая точность
        Ultra           // Ультра точность
    }
    
    /// <summary>
    /// Детализация подвески
    /// </summary>
    public enum SuspensionDetail
    {
        None,           // Без подвески
        Basic,          // Базовая подвеска
        Advanced,       // Продвинутая подвеска
        Realistic       // Реалистичная подвеска
    }
    
    /// <summary>
    /// Детализация колес
    /// </summary>
    public enum WheelDetail
    {
        None,           // Без колес
        Basic,          // Базовые колеса
        Advanced,       // Продвинутые колеса
        Realistic       // Реалистичные колеса
    }
}