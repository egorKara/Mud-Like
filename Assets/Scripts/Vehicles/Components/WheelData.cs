using Unity.Entities;
using Unity.Mathematics;

namespace MudLike.Vehicles.Components
{
    /// <summary>
    /// Данные колеса транспортного средства
    /// </summary>
    public struct WheelData : IComponentData
    {
        /// <summary>
        /// Позиция колеса относительно транспорта
        /// </summary>
        public float3 LocalPosition;
        
        /// <summary>
        /// Радиус колеса
        /// </summary>
        public float Radius;
        
        /// <summary>
        /// Ширина колеса
        /// </summary>
        public float Width;
        
        /// <summary>
        /// Длина подвески
        /// </summary>
        public float SuspensionLength;
        
        /// <summary>
        /// Сила пружины подвески
        /// </summary>
        public float SpringForce;
        
        /// <summary>
        /// Сила демпфера подвески
        /// </summary>
        public float DampingForce;
        
        /// <summary>
        /// Целевая позиция подвески
        /// </summary>
        public float TargetPosition;
        
        /// <summary>
        /// Текущая позиция подвески
        /// </summary>
        public float CurrentPosition;
        
        /// <summary>
        /// Скорость подвески
        /// </summary>
        public float SuspensionVelocity;
        
        /// <summary>
        /// Колесо касается земли
        /// </summary>
        public bool IsGrounded;
        
        /// <summary>
        /// Точка касания с землей
        /// </summary>
        public float3 GroundPoint;
        
        /// <summary>
        /// Нормаль поверхности в точке касания
        /// </summary>
        public float3 GroundNormal;
        
        /// <summary>
        /// Расстояние до земли
        /// </summary>
        public float GroundDistance;
        
        /// <summary>
        /// Сила сцепления с поверхностью
        /// </summary>
        public float Traction;
        
        /// <summary>
        /// Угловая скорость колеса
        /// </summary>
        public float AngularVelocity;
        
        /// <summary>
        /// Угол поворота колеса
        /// </summary>
        public float SteerAngle;
        
        /// <summary>
        /// Максимальный угол поворота колеса
        /// </summary>
        public float MaxSteerAngle;
        
        /// <summary>
        /// Крутящий момент на колесе
        /// </summary>
        public float MotorTorque;
        
        /// <summary>
        /// Тормозной момент на колесе
        /// </summary>
        public float BrakeTorque;
        
        /// <summary>
        /// Сила трения колеса
        /// </summary>
        public float3 FrictionForce;
        
        /// <summary>
        /// Сила подвески
        /// </summary>
        public float3 SuspensionForce;
    }
}