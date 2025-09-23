using Unity.Entities;
using Unity.Mathematics;

namespace MudLike.Terrain.Components
{
    /// <summary>
    /// Данные о типе поверхности
    /// </summary>
    public struct Surface : IComponentData
    {
        /// <summary>
        /// Тип поверхности
        /// </summary>
        public SurfaceType SurfaceType;
        
        /// <summary>
        /// Коэффициент трения (0-1)
        /// </summary>
        public float FrictionCoefficient;
        
        /// <summary>
        /// Коэффициент сцепления (0-1)
        /// </summary>
        public float TractionCoefficient;
        
        /// <summary>
        /// Сопротивление качению (0-1)
        /// </summary>
        public float RollingResistance;
        
        /// <summary>
        /// Вязкость поверхности (0-1)
        /// </summary>
        public float Viscosity;
        
        /// <summary>
        /// Плотность поверхности (кг/м³)
        /// </summary>
        public float Density;
        
        /// <summary>
        /// Глубина проникновения (0-1)
        /// </summary>
        public float PenetrationDepth;
        
        /// <summary>
        /// Скорость высыхания (0-1 в секунду)
        /// </summary>
        public float DryingRate;
        
        /// <summary>
        /// Температура замерзания (°C)
        /// </summary>
        public float FreezingPoint;
        
        /// <summary>
        /// Текущая температура поверхности (°C)
        /// </summary>
        public float Temperature;
        
        /// <summary>
        /// Влажность поверхности (0-1)
        /// </summary>
        public float Moisture;
        
        /// <summary>
        /// Требует ли обновления
        /// </summary>
        public bool NeedsUpdate;
    }
    
    /// <summary>
    /// Типы поверхностей
    /// </summary>
    public enum SurfaceType
    {
        Asphalt,     // Асфальт
        Concrete,    // Бетон
        Dirt,        // Грязь
        Mud,         // Глубокая грязь
        Sand,        // Песок
        Grass,       // Трава
        Water,       // Вода
        Ice,         // Лед
        Snow,        // Снег
        Rock,        // Камень
        Gravel,      // Гравий
        Swamp        // Болото
    }
    
    /// <summary>
    /// Статические данные о поверхностях
    /// </summary>
    public static class SurfaceProperties
    {
        /// <summary>
        /// Получает свойства поверхности по типу
        /// </summary>
        public static SurfaceData GetSurfaceProperties(SurfaceType surfaceType)
        {
            return surfaceType switch
            {
                SurfaceType.Asphalt => new SurfaceData
                {
                    SurfaceType = surfaceType,
                    FrictionCoefficient = 0.8f,
                    TractionCoefficient = 0.9f,
                    RollingResistance = 0.02f,
                    Viscosity = 0.0f,
                    Density = 2400f,
                    PenetrationDepth = 0.0f,
                    DryingRate = 1.0f,
                    FreezingPoint = 0f,
                    Temperature = 20f,
                    Moisture = 0.0f
                },
                
                SurfaceType.Concrete => new SurfaceData
                {
                    SurfaceType = surfaceType,
                    FrictionCoefficient = 0.75f,
                    TractionCoefficient = 0.85f,
                    RollingResistance = 0.025f,
                    Viscosity = 0.0f,
                    Density = 2500f,
                    PenetrationDepth = 0.0f,
                    DryingRate = 1.0f,
                    FreezingPoint = 0f,
                    Temperature = 20f,
                    Moisture = 0.0f
                },
                
                SurfaceType.Dirt => new SurfaceData
                {
                    SurfaceType = surfaceType,
                    FrictionCoefficient = 0.6f,
                    TractionCoefficient = 0.7f,
                    RollingResistance = 0.05f,
                    Viscosity = 0.1f,
                    Density = 1800f,
                    PenetrationDepth = 0.1f,
                    DryingRate = 0.3f,
                    FreezingPoint = 0f,
                    Temperature = 15f,
                    Moisture = 0.3f
                },
                
                SurfaceType.Mud => new SurfaceData
                {
                    SurfaceType = surfaceType,
                    FrictionCoefficient = 0.3f,
                    TractionCoefficient = 0.4f,
                    RollingResistance = 0.15f,
                    Viscosity = 0.8f,
                    Density = 2000f,
                    PenetrationDepth = 0.3f,
                    DryingRate = 0.1f,
                    FreezingPoint = 0f,
                    Temperature = 10f,
                    Moisture = 0.8f
                },
                
                SurfaceType.Sand => new SurfaceData
                {
                    SurfaceType = surfaceType,
                    FrictionCoefficient = 0.4f,
                    TractionCoefficient = 0.5f,
                    RollingResistance = 0.2f,
                    Viscosity = 0.0f,
                    Density = 1600f,
                    PenetrationDepth = 0.2f,
                    DryingRate = 0.8f,
                    FreezingPoint = 0f,
                    Temperature = 25f,
                    Moisture = 0.1f
                },
                
                SurfaceType.Grass => new SurfaceData
                {
                    SurfaceType = surfaceType,
                    FrictionCoefficient = 0.5f,
                    TractionCoefficient = 0.6f,
                    RollingResistance = 0.08f,
                    Viscosity = 0.0f,
                    Density = 1200f,
                    PenetrationDepth = 0.05f,
                    DryingRate = 0.5f,
                    FreezingPoint = 0f,
                    Temperature = 18f,
                    Moisture = 0.4f
                },
                
                SurfaceType.Water => new SurfaceData
                {
                    SurfaceType = surfaceType,
                    FrictionCoefficient = 0.1f,
                    TractionCoefficient = 0.2f,
                    RollingResistance = 0.3f,
                    Viscosity = 0.0f,
                    Density = 1000f,
                    PenetrationDepth = 0.5f,
                    DryingRate = 0.0f,
                    FreezingPoint = 0f,
                    Temperature = 10f,
                    Moisture = 1.0f
                },
                
                SurfaceType.Ice => new SurfaceData
                {
                    SurfaceType = surfaceType,
                    FrictionCoefficient = 0.1f,
                    TractionCoefficient = 0.15f,
                    RollingResistance = 0.05f,
                    Viscosity = 0.0f,
                    Density = 900f,
                    PenetrationDepth = 0.0f,
                    DryingRate = 0.0f,
                    FreezingPoint = 0f,
                    Temperature = -5f,
                    Moisture = 0.0f
                },
                
                SurfaceType.Snow => new SurfaceData
                {
                    SurfaceType = surfaceType,
                    FrictionCoefficient = 0.2f,
                    TractionCoefficient = 0.3f,
                    RollingResistance = 0.1f,
                    Viscosity = 0.0f,
                    Density = 300f,
                    PenetrationDepth = 0.4f,
                    DryingRate = 0.0f,
                    FreezingPoint = 0f,
                    Temperature = -2f,
                    Moisture = 0.0f
                },
                
                SurfaceType.Rock => new SurfaceData
                {
                    SurfaceType = surfaceType,
                    FrictionCoefficient = 0.7f,
                    TractionCoefficient = 0.8f,
                    RollingResistance = 0.03f,
                    Viscosity = 0.0f,
                    Density = 2800f,
                    PenetrationDepth = 0.0f,
                    DryingRate = 1.0f,
                    FreezingPoint = 0f,
                    Temperature = 12f,
                    Moisture = 0.1f
                },
                
                SurfaceType.Gravel => new SurfaceData
                {
                    SurfaceType = surfaceType,
                    FrictionCoefficient = 0.6f,
                    TractionCoefficient = 0.7f,
                    RollingResistance = 0.06f,
                    Viscosity = 0.0f,
                    Density = 2000f,
                    PenetrationDepth = 0.05f,
                    DryingRate = 0.7f,
                    FreezingPoint = 0f,
                    Temperature = 16f,
                    Moisture = 0.2f
                },
                
                SurfaceType.Swamp => new SurfaceData
                {
                    SurfaceType = surfaceType,
                    FrictionCoefficient = 0.2f,
                    TractionCoefficient = 0.25f,
                    RollingResistance = 0.25f,
                    Viscosity = 0.9f,
                    Density = 1500f,
                    PenetrationDepth = 0.6f,
                    DryingRate = 0.05f,
                    FreezingPoint = 0f,
                    Temperature = 8f,
                    Moisture = 0.95f
                },
                
                _ => new SurfaceData()
            };
        }
    }
}