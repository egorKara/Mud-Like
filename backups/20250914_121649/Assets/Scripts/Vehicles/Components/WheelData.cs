using Unity.Entities;
using Unity.Mathematics;

namespace MudLike.Vehicles.Components
{
    /// <summary>
    /// Данные колеса для физики и взаимодействия с террейном
    /// </summary>
    public struct WheelData : IComponentData
    {
        /// <summary>
        /// Позиция колеса относительно центра транспорта
        /// </summary>
        public float3 Position;
        
        /// <summary>
        /// Радиус колеса
        /// </summary>
        public float Radius;
        
        /// <summary>
        /// Ширина колеса
        /// </summary>
        public float Width;
        
        /// <summary>
        /// Масса колеса
        /// </summary>
        public float Mass;
        
        /// <summary>
        /// Момент инерции колеса
        /// </summary>
        public float Inertia;
        
        /// <summary>
        /// Угловая скорость колеса
        /// </summary>
        public float AngularVelocity;
        
        /// <summary>
        /// Угол поворота колеса
        /// </summary>
        public float SteerAngle;
        
        /// <summary>
        /// Максимальный угол поворота
        /// </summary>
        public float MaxSteerAngle;
        
        /// <summary>
        /// Сила трения колеса
        /// </summary>
        public float FrictionForce;
        
        /// <summary>
        /// Сила нормали колеса
        /// </summary>
        public float NormalForce;
        
        /// <summary>
        /// Сцепление с поверхностью
        /// </summary>
        public float Traction;
        
        /// <summary>
        /// Сопротивление качению
        /// </summary>
        public float RollingResistance;
        
        /// <summary>
        /// Деформация шины
        /// </summary>
        public float TireDeflection;
        
        /// <summary>
        /// Температура шины
        /// </summary>
        public float TireTemperature;
        
        /// <summary>
        /// Износ шины
        /// </summary>
        public float TireWear;
        
        /// <summary>
        /// Давление в шине
        /// </summary>
        public float TirePressure;
        
        /// <summary>
        /// Контактирует ли колесо с поверхностью
        /// </summary>
        public bool IsGrounded;
        
        /// <summary>
        /// Расстояние до поверхности
        /// </summary>
        public float GroundDistance;
        
        /// <summary>
        /// Нормаль поверхности в точке контакта
        /// </summary>
        public float3 GroundNormal;
        
        /// <summary>
        /// Скорость скольжения колеса
        /// </summary>
        public float3 SlipVelocity;
        
        /// <summary>
        /// Сила воздействия на террейн
        /// </summary>
        public float3 TerrainForce;
        
        /// <summary>
        /// Радиус деформации террейна
        /// </summary>
        public float DeformationRadius;
    }
}