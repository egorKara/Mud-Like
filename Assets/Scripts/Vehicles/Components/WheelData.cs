using Unity.Entities;
using Unity.Mathematics;

namespace MudLike.Vehicles.Components
{
    /// <summary>
    /// Данные колеса грузовика
    /// </summary>
    public struct WheelData : IComponentData
    {
        /// <summary>
        /// Позиция колеса относительно центра грузовика
        /// </summary>
        public float3 LocalPosition;
        
        /// <summary>
        /// Радиус колеса в метрах
        /// </summary>
        public float Radius;
        
        /// <summary>
        /// Ширина колеса в метрах
        /// </summary>
        public float Width;
        
        /// <summary>
        /// Угловая скорость колеса в рад/с
        /// </summary>
        public float AngularVelocity;
        
        /// <summary>
        /// Угол поворота колеса в радианах
        /// </summary>
        public float SteerAngle;
        
        /// <summary>
        /// Крутящий момент, приложенный к колесу
        /// </summary>
        public float Torque;
        
        /// <summary>
        /// Тормозной момент
        /// </summary>
        public float BrakeTorque;
        
        /// <summary>
        /// Коэффициент сцепления с поверхностью
        /// </summary>
        public float TractionCoefficient;
        
        /// <summary>
        /// Глубина погружения в грязь
        /// </summary>
        public float SinkDepth;
        
        /// <summary>
        /// Сила сцепления с поверхностью
        /// </summary>
        public float3 TractionForce;
        
        /// <summary>
        /// Скорость скольжения колеса
        /// </summary>
        public float SlipRatio;
        
        /// <summary>
        /// Является ли колесо ведущим
        /// </summary>
        public bool IsDriven;
        
        /// <summary>
        /// Является ли колесо поворотным
        /// </summary>
        public bool IsSteerable;
        
        /// <summary>
        /// Индекс колеса (0-5 для КРАЗ)
        /// </summary>
        public int WheelIndex;
    }
}