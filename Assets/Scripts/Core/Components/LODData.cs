using Unity.Entities;
using Unity.Mathematics;

namespace MudLike.Vehicles.Components
{
    /// <summary>
    /// Компонент данных LOD (Level of Detail) для оптимизации рендеринга и физики
    /// </summary>
    public struct LOD : IComponentData
    {
        // Основные параметры LOD
        public int CurrentLOD;            // Текущий уровень детализации (0-3)
        public int MaxLOD;                // Максимальный уровень детализации
        public float DistanceToCamera;    // Расстояние до камеры
        public float4 LODDistances;       // Расстояния переключения LOD (x=0-1, y=1-2, z=2-3, w=3+)
        
        // Параметры переключения
        public float LastLODSwitchTime;   // Время последнего переключения LOD
        public float LODSwitchSpeed;      // Скорость переключения LOD (секунды между переключениями)
        public bool IsActive;             // Активен ли LOD
        public bool NeedsUpdate;          // Требует ли обновления
        
        // Параметры производительности
        public float UpdateFrequency;     // Частота обновления LOD (Гц)
        public float LastUpdateTime;      // Время последнего обновления
        public bool EnableOptimization;   // Включена ли оптимизация
        
        // Конструктор с значениями по умолчанию
        public static LODData Default => new LODData
        {
            CurrentLOD = 0,
            MaxLOD = 3,
            DistanceToCamera = 0f,
            LODDistances = new float4(50f, 100f, 200f, 500f), // 50м, 100м, 200м, 500м
            LastLODSwitchTime = 0f,
            LODSwitchSpeed = 1f,          // 1 секунда между переключениями
            IsActive = true,
            NeedsUpdate = true,
            UpdateFrequency = 10f,        // 10 Гц
            LastUpdateTime = 0f,
            EnableOptimization = true
        };
        
        /// <summary>
        /// Проверяет, нужно ли переключить LOD
        /// </summary>
        public bool ShouldSwitchLOD(float newDistance)
        {
            int newLOD = CalculateLOD(newDistance);
            return newLOD != CurrentLOD;
        }
        
        /// <summary>
        /// Вычисляет LOD на основе расстояния
        /// </summary>
        public int CalculateLOD(float distance)
        {
            if (distance <= LODDistances.x) return 0;
            if (distance <= LODDistances.y) return 1;
            if (distance <= LODDistances.z) return 2;
            if (distance <= LODDistances.w) return 3;
            return MaxLOD;
        }
        
        /// <summary>
        /// Проверяет, можно ли переключить LOD сейчас
        /// </summary>
        public bool CanSwitchLOD(float currentTime)
        {
            return currentTime - LastLODSwitchTime >= LODSwitchSpeed;
        }
        
        /// <summary>
        /// Вычисляет приоритет обновления (0-1)
        /// </summary>
        public float GetUpdatePriority()
        {
            // Ближайшие объекты имеют больший приоритет
            float distanceFactor = math.max(0f, 1f - (DistanceToCamera / LODDistances.w));
            float lodFactor = 1f - (CurrentLOD / (float)MaxLOD);
            
            return (distanceFactor + lodFactor) / 2f;
        }
    }
    
    /// <summary>
    /// Компонент данных рендеринга LOD
    /// </summary>
    public struct LODRenderData : IComponentData
    {
        // Параметры рендеринга
        public int PolygonCount;          // Количество полигонов
        public int TextureCount;          // Количество текстур
        public int TextureSize;           // Размер текстуры
        public int AnimationCount;        // Количество анимаций
        public ShadowQuality ShadowQuality; // Качество теней
        public ReflectionQuality ReflectionQuality; // Качество отражений
        public ParticleQuality ParticleQuality; // Качество частиц
        
        // Параметры видимости
        public bool IsRendering;          // Рендерится ли объект
        public bool IsVisible;            // Видим ли объект
        public bool NeedsUpdate;          // Требует ли обновления
        public float RenderDistance;      // Расстояние рендеринга
        
        // Параметры оптимизации
        public bool EnableCulling;        // Включено ли отсечение
        public bool EnableOcclusion;      // Включена ли окклюзия
        public bool EnableLOD;            // Включен ли LOD рендеринг
        
        // Конструктор с значениями по умолчанию
        public static LODRenderData Default => new LODRenderData
        {
            PolygonCount = 10000,
            TextureCount = 8,
            TextureSize = 2048,
            AnimationCount = 20,
            ShadowQuality = ShadowQuality.High,
            ReflectionQuality = ReflectionQuality.High,
            ParticleQuality = ParticleQuality.High,
            IsRendering = true,
            IsVisible = true,
            NeedsUpdate = true,
            RenderDistance = 500f,
            EnableCulling = true,
            EnableOcclusion = true,
            EnableLOD = true
        };
    }
    
    /// <summary>
    /// Компонент данных физики LOD
    /// </summary>
    public struct LODPhysicsData : IComponentData
    {
        // Параметры физики
        public float PhysicsUpdateRate;   // Частота обновления физики (Гц)
        public int PhysicsIterations;     // Количество итераций физики
        public CollisionAccuracy CollisionAccuracy; // Точность коллизий
        public SuspensionDetail SuspensionDetail; // Детализация подвески
        public WheelDetail WheelDetail;   // Детализация колес
        
        // Параметры состояния
        public bool IsActive;             // Активна ли физика
        public bool NeedsUpdate;          // Требует ли обновления
        public float LastUpdateTime;      // Время последнего обновления
        
        // Параметры оптимизации
        public bool EnableSimplifiedPhysics; // Включена ли упрощенная физика
        public bool EnableCollisionOptimization; // Включена ли оптимизация коллизий
        public float PhysicsComplexity;   // Сложность физики (0-1)
        
        // Конструктор с значениями по умолчанию
        public static LODPhysicsData Default => new LODPhysicsData
        {
            PhysicsUpdateRate = 60f,
            PhysicsIterations = 10,
            CollisionAccuracy = CollisionAccuracy.High,
            SuspensionDetail = SuspensionDetail.Realistic,
            WheelDetail = WheelDetail.Realistic,
            IsActive = true,
            NeedsUpdate = true,
            LastUpdateTime = 0f,
            EnableSimplifiedPhysics = false,
            EnableCollisionOptimization = true,
            PhysicsComplexity = 1f
        };
    }
    
    /// <summary>
    /// Качество теней
    /// </summary>
    public enum ShadowQuality
    {
        None = 0,
        Low = 1,
        Medium = 2,
        High = 3,
        Ultra = 4
    }
    
    /// <summary>
    /// Качество отражений
    /// </summary>
    public enum ReflectionQuality
    {
        None = 0,
        Low = 1,
        Medium = 2,
        High = 3,
        Ultra = 4
    }
    
    /// <summary>
    /// Качество частиц
    /// </summary>
    public enum ParticleQuality
    {
        None = 0,
        Low = 1,
        Medium = 2,
        High = 3,
        Ultra = 4
    }
    
    /// <summary>
    /// Точность коллизий
    /// </summary>
    public enum CollisionAccuracy
    {
        Low = 0,
        Medium = 1,
        High = 2,
        Ultra = 3
    }
    
    /// <summary>
    /// Детализация подвески
    /// </summary>
    public enum SuspensionDetail
    {
        None = 0,
        Basic = 1,
        Advanced = 2,
        Realistic = 3
    }
    
    /// <summary>
    /// Детализация колес
    /// </summary>
    public enum WheelDetail
    {
        None = 0,
        Basic = 1,
        Advanced = 2,
        Realistic = 3
    }
}
