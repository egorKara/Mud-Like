using Unity.Entities;
using Unity.Mathematics;

namespace MudLike.Vehicles.Components
{
    /// <summary>
    /// ECS компонент для подвески колеса
    /// </summary>
    public struct SuspensionComponent : IComponentData
    {
        /// <summary>
        /// Длина подвески в покое (м)
        /// </summary>
        public float restLength;
        
        /// <summary>
        /// Максимальная длина подвески (м)
        /// </summary>
        public float maxLength;
        
        /// <summary>
        /// Минимальная длина подвески (м)
        /// </summary>
        public float minLength;
        
        /// <summary>
        /// Текущая длина подвески (м)
        /// </summary>
        public float currentLength;
        
        /// <summary>
        /// Жесткость пружины (Н/м)
        /// </summary>
        public float springStiffness;
        
        /// <summary>
        /// Демпфирование (Н·с/м)
        /// </summary>
        public float damping;
        
        /// <summary>
        /// Максимальное демпфирование (Н·с/м)
        /// </summary>
        public float maxDamping;
        
        /// <summary>
        /// Сила пружины
        /// </summary>
        public float springForce;
        
        /// <summary>
        /// Сила демпфера
        /// </summary>
        public float damperForce;
        
        /// <summary>
        /// Скорость сжатия/растяжения подвески (м/с)
        /// </summary>
        public float compressionVelocity;
        
        /// <summary>
        /// Коэффициент сжатия (0-1)
        /// </summary>
        public float compressionRatio;
        
        /// <summary>
        /// Локальная позиция точки крепления подвески
        /// </summary>
        public float3 mountPoint;
        
        /// <summary>
        /// Направление подвески (обычно вниз)
        /// </summary>
        public float3 suspensionDirection;
        
        /// <summary>
        /// Флаг: активна ли подвеска
        /// </summary>
        public bool isActive;
        
        /// <summary>
        /// Флаг: достигнут ли лимит сжатия
        /// </summary>
        public bool isCompressed;
        
        /// <summary>
        /// Флаг: достигнут ли лимит растяжения
        /// </summary>
        public bool isExtended;
        
        /// <summary>
        /// Прогресс сжатия (0-1)
        /// </summary>
        public float compressionProgress;
        
        /// <summary>
        /// Энергия подвески
        /// </summary>
        public float energy;
        
        /// <summary>
        /// Работа подвески за кадр
        /// </summary>
        public float work;
    }
}
