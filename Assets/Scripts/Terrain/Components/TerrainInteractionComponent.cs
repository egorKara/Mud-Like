using Unity.Entities;
using Unity.Mathematics;

namespace MudLike.Terrain.Components
{
    /// <summary>
    /// ECS компонент для взаимодействия с местностью
    /// </summary>
    public struct TerrainInteractionComponent : IComponentData
    {
        /// <summary>
        /// Позиция взаимодействия в мировых координатах
        /// </summary>
        public float3 worldPosition;
        
        /// <summary>
        /// Сила взаимодействия (Н)
        /// </summary>
        public float interactionForce;
        
        /// <summary>
        /// Радиус взаимодействия (м)
        /// </summary>
        public float interactionRadius;
        
        /// <summary>
        /// Скорость взаимодействия (м/с)
        /// </summary>
        public float3 interactionVelocity;
        
        /// <summary>
        /// Тип взаимодействия
        /// </summary>
        public InteractionType interactionType;
        
        /// <summary>
        /// Материал поверхности в точке взаимодействия
        /// </summary>
        public SurfaceMaterial surfaceMaterial;
        
        /// <summary>
        /// Коэффициент трения в точке взаимодействия
        /// </summary>
        public float localFriction;
        
        /// <summary>
        /// Коэффициент сцепления в точке взаимодействия
        /// </summary>
        public float localGrip;
        
        /// <summary>
        /// Жесткость поверхности в точке взаимодействия
        /// </summary>
        public float localStiffness;
        
        /// <summary>
        /// Демпфирование поверхности в точке взаимодействия
        /// </summary>
        public float localDamping;
        
        /// <summary>
        /// Высота поверхности в точке взаимодействия
        /// </summary>
        public float surfaceHeight;
        
        /// <summary>
        /// Нормаль поверхности в точке взаимодействия
        /// </summary>
        public float3 surfaceNormal;
        
        /// <summary>
        /// Флаг: активно ли взаимодействие
        /// </summary>
        public bool isActive;
        
        /// <summary>
        /// Флаг: создает ли деформацию
        /// </summary>
        public bool createsDeformation;
        
        /// <summary>
        /// Флаг: влияет ли на физику
        /// </summary>
        public bool affectsPhysics;
        
        /// <summary>
        /// Время последнего обновления
        /// </summary>
        public float lastUpdateTime;
        
        /// <summary>
        /// Интервал обновления (с)
        /// </summary>
        public float updateInterval;
        
        /// <summary>
        /// Энергия взаимодействия
        /// </summary>
        public float interactionEnergy;
        
        /// <summary>
        /// Работа взаимодействия
        /// </summary>
        public float interactionWork;
    }
    
    /// <summary>
    /// Типы взаимодействия с местностью
    /// </summary>
    public enum InteractionType : byte
    {
        /// <summary>
        /// Контакт колеса с поверхностью
        /// </summary>
        WheelContact,
        
        /// <summary>
        /// Удар по поверхности
        /// </summary>
        Impact,
        
        /// <summary>
        /// Давление на поверхность
        /// </summary>
        Pressure,
        
        /// <summary>
        /// Скольжение по поверхности
        /// </summary>
        Sliding,
        
        /// <summary>
        /// Качение по поверхности
        /// </summary>
        Rolling,
        
        /// <summary>
        /// Трение о поверхность
        /// </summary>
        Friction
    }
}
