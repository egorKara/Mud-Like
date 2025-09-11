using Unity.Entities;
using Unity.Mathematics;

namespace MudLike.Terrain.Components
{
    /// <summary>
    /// ECS компонент для деформации местности
    /// </summary>
    public struct DeformationComponent : IComponentData
    {
        /// <summary>
        /// Позиция деформации в мировых координатах
        /// </summary>
        public float3 worldPosition;
        
        /// <summary>
        /// Радиус деформации (м)
        /// </summary>
        public float radius;
        
        /// <summary>
        /// Глубина деформации (м)
        /// </summary>
        public float depth;
        
        /// <summary>
        /// Максимальная глубина деформации (м)
        /// </summary>
        public float maxDepth;
        
        /// <summary>
        /// Сила деформации (Н)
        /// </summary>
        public float force;
        
        /// <summary>
        /// Максимальная сила деформации (Н)
        /// </summary>
        public float maxForce;
        
        /// <summary>
        /// Скорость деформации (м/с)
        /// </summary>
        public float deformationSpeed;
        
        /// <summary>
        /// Время создания деформации (с)
        /// </summary>
        public float creationTime;
        
        /// <summary>
        /// Время жизни деформации (с)
        /// </summary>
        public float lifetime;
        
        /// <summary>
        /// Максимальное время жизни (с)
        /// </summary>
        public float maxLifetime;
        
        /// <summary>
        /// Скорость восстановления (м/с)
        /// </summary>
        public float recoveryRate;
        
        /// <summary>
        /// Прогресс восстановления (0-1)
        /// </summary>
        public float recoveryProgress;
        
        /// <summary>
        /// Флаг: активна ли деформация
        /// </summary>
        public bool isActive;
        
        /// <summary>
        /// Флаг: восстанавливается ли деформация
        /// </summary>
        public bool isRecovering;
        
        /// <summary>
        /// Флаг: постоянная ли деформация
        /// </summary>
        public bool isPermanent;
        
        /// <summary>
        /// Тип деформации
        /// </summary>
        public DeformationType deformationType;
        
        /// <summary>
        /// Материал поверхности в точке деформации
        /// </summary>
        public SurfaceMaterial surfaceMaterial;
        
        /// <summary>
        /// Коэффициент трения в точке деформации
        /// </summary>
        public float localFriction;
        
        /// <summary>
        /// Коэффициент сцепления в точке деформации
        /// </summary>
        public float localGrip;
        
        /// <summary>
        /// Энергия деформации
        /// </summary>
        public float deformationEnergy;
        
        /// <summary>
        /// Работа деформации
        /// </summary>
        public float deformationWork;
    }
    
    /// <summary>
    /// Типы деформации местности
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
        /// Деформация от эрозии
        /// </summary>
        Erosion,
        
        /// <summary>
        /// Деформация от воды
        /// </summary>
        Water,
        
        /// <summary>
        /// Деформация от взрыва
        /// </summary>
        Explosion,
        
        /// <summary>
        /// Деформация от инструмента
        /// </summary>
        Tool
    }
    
    /// <summary>
    /// Материалы поверхности
    /// </summary>
    public enum SurfaceMaterial : byte
    {
        /// <summary>
        /// Грязь
        /// </summary>
        Mud,
        
        /// <summary>
        /// Песок
        /// </summary>
        Sand,
        
        /// <summary>
        /// Глина
        /// </summary>
        Clay,
        
        /// <summary>
        /// Камень
        /// </summary>
        Rock,
        
        /// <summary>
        /// Трава
        /// </summary>
        Grass,
        
        /// <summary>
        /// Снег
        /// </summary>
        Snow,
        
        /// <summary>
        /// Лед
        /// </summary>
        Ice,
        
        /// <summary>
        /// Вода
        /// </summary>
        Water,
        
        /// <summary>
        /// Асфальт
        /// </summary>
        Asphalt,
        
        /// <summary>
        /// Бетон
        /// </summary>
        Concrete
    }
}
