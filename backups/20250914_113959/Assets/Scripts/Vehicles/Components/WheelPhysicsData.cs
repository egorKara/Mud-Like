using Unity.Entities;
using Unity.Mathematics;

namespace MudLike.Vehicles.Components
{
    /// <summary>
    /// Данные физики колеса для высокопроизводительных вычислений
    /// </summary>
    public struct WheelPhysicsData : IComponentData
    {
        /// <summary>
        /// Позиция колеса в мировых координатах
        /// </summary>
        public float3 WorldPosition;
        
        /// <summary>
        /// Скорость колеса в мировых координатах
        /// </summary>
        public float3 WorldVelocity;
        
        /// <summary>
        /// Угловая скорость колеса
        /// </summary>
        public float3 AngularVelocity;
        
        /// <summary>
        /// Угловое ускорение колеса
        /// </summary>
        public float3 AngularAcceleration;
        
        /// <summary>
        /// Линейное ускорение колеса
        /// </summary>
        public float3 LinearAcceleration;
        
        /// <summary>
        /// Сила трения колеса
        /// </summary>
        public float3 FrictionForce;
        
        /// <summary>
        /// Сила нормали колеса
        /// </summary>
        public float3 NormalForce;
        
        /// <summary>
        /// Момент силы колеса
        /// </summary>
        public float3 Torque;
        
        /// <summary>
        /// Масса колеса
        /// </summary>
        public float Mass;
        
        /// <summary>
        /// Момент инерции колеса
        /// </summary>
        public float3 Inertia;
        
        /// <summary>
        /// Радиус колеса
        /// </summary>
        public float Radius;
        
        /// <summary>
        /// Ширина колеса
        /// </summary>
        public float Width;
        
        /// <summary>
        /// Коэффициент трения
        /// </summary>
        public float FrictionCoefficient;
        
        /// <summary>
        /// Коэффициент сопротивления качению
        /// </summary>
        public float RollingResistanceCoefficient;
        
        /// <summary>
        /// Жесткость шины
        /// </summary>
        public float TireStiffness;
        
        /// <summary>
        /// Демпфирование шины
        /// </summary>
        public float TireDamping;
        
        /// <summary>
        /// Максимальная сила трения
        /// </summary>
        public float MaxFrictionForce;
        
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
        /// Коэффициент скольжения
        /// </summary>
        public float SlipRatio;
        
        /// <summary>
        /// Угол скольжения
        /// </summary>
        public float SlipAngle;
        
        /// <summary>
        /// Температура шины
        /// </summary>
        public float TireTemperature;
        
        /// <summary>
        /// Давление в шине
        /// </summary>
        public float TirePressure;
        
        /// <summary>
        /// Износ шины
        /// </summary>
        public float TireWear;
        
        /// <summary>
        /// Деформация шины
        /// </summary>
        public float TireDeflection;
        
        /// <summary>
        /// Сила воздействия на террейн
        /// </summary>
        public float3 TerrainForce;
        
        /// <summary>
        /// Радиус деформации террейна
        /// </summary>
        public float DeformationRadius;
        
        /// <summary>
        /// Глубина деформации террейна
        /// </summary>
        public float DeformationDepth;
        
        /// <summary>
        /// Время последнего обновления
        /// </summary>
        public float LastUpdateTime;
        
        /// <summary>
        /// Флаг необходимости обновления
        /// </summary>
        public bool NeedsUpdate;
    }
}