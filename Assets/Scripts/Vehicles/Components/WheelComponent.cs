using Unity.Entities;
using Unity.Mathematics;

namespace MudLike.Vehicles.Components
{
    /// <summary>
    /// ECS компонент для колеса транспортного средства
    /// </summary>
    public struct WheelComponent : IComponentData
    {
        /// <summary>
        /// Радиус колеса в метрах
        /// </summary>
        public float radius;
        
        /// <summary>
        /// Ширина колеса в метрах
        /// </summary>
        public float width;
        
        /// <summary>
        /// Масса колеса в килограммах
        /// </summary>
        public float mass;
        
        /// <summary>
        /// Момент инерции колеса
        /// </summary>
        public float inertia;
        
        /// <summary>
        /// Угловая скорость колеса (рад/с)
        /// </summary>
        public float angularVelocity;
        
        /// <summary>
        /// Угол поворота колеса (рад)
        /// </summary>
        public float steerAngle;
        
        /// <summary>
        /// Максимальный угол поворота (рад)
        /// </summary>
        public float maxSteerAngle;
        
        /// <summary>
        /// Коэффициент трения колеса
        /// </summary>
        public float frictionCoefficient;
        
        /// <summary>
        /// Коэффициент сцепления с поверхностью
        /// </summary>
        public float gripCoefficient;
        
        /// <summary>
        /// Сила торможения
        /// </summary>
        public float brakeForce;
        
        /// <summary>
        /// Максимальная сила торможения
        /// </summary>
        public float maxBrakeForce;
        
        /// <summary>
        /// Локальная позиция колеса относительно центра масс
        /// </summary>
        public float3 localPosition;
        
        /// <summary>
        /// Флаг: является ли колесо ведущим
        /// </summary>
        public bool isDriven;
        
        /// <summary>
        /// Флаг: является ли колесо управляемым
        /// </summary>
        public bool isSteerable;
        
        /// <summary>
        /// Флаг: контактирует ли колесо с поверхностью
        /// </summary>
        public bool isGrounded;
        
        /// <summary>
        /// Расстояние до поверхности
        /// </summary>
        public float groundDistance;
        
        /// <summary>
        /// Нормаль поверхности в точке контакта
        /// </summary>
        public float3 groundNormal;
        
        /// <summary>
        /// Точка контакта с поверхностью
        /// </summary>
        public float3 contactPoint;
        
        /// <summary>
        /// Сила реакции поверхности
        /// </summary>
        public float3 groundReactionForce;
        
        /// <summary>
        /// Сила трения
        /// </summary>
        public float3 frictionForce;
        
        /// <summary>
        /// Скорость скольжения
        /// </summary>
        public float3 slipVelocity;
        
        /// <summary>
        /// Коэффициент скольжения
        /// </summary>
        public float slipRatio;
        
        /// <summary>
        /// Угол скольжения
        /// </summary>
        public float slipAngle;
    }
}
