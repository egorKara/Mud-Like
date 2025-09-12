using Unity.Entities;
using Unity.Mathematics;

namespace MudLike.Vehicles.Components
{
    /// <summary>
    /// Физические данные транспортного средства
    /// </summary>
    public struct VehiclePhysics : IComponentData
    {
        /// <summary>
        /// Линейная скорость транспорта
        /// </summary>
        public float3 Velocity;
        
        /// <summary>
        /// Угловая скорость транспорта
        /// </summary>
        public float3 AngularVelocity;
        
        /// <summary>
        /// Линейное ускорение транспорта
        /// </summary>
        public float3 Acceleration;
        
        /// <summary>
        /// Угловое ускорение транспорта
        /// </summary>
        public float3 AngularAcceleration;
        
        /// <summary>
        /// Приложенная сила к транспорту
        /// </summary>
        public float3 AppliedForce;
        
        /// <summary>
        /// Приложенный момент к транспорту
        /// </summary>
        public float3 AppliedTorque;
        
        /// <summary>
        /// Скорость движения вперед/назад
        /// </summary>
        public float ForwardSpeed;
        
        /// <summary>
        /// Скорость поворота
        /// </summary>
        public float TurnSpeed;
        
        /// <summary>
        /// Текущая передача
        /// </summary>
        public int CurrentGear;
        
        /// <summary>
        /// Обороты двигателя (RPM)
        /// </summary>
        public float EngineRPM;
        
        /// <summary>
        /// Мощность двигателя
        /// </summary>
        public float EnginePower;
        
        /// <summary>
        /// Крутящий момент двигателя
        /// </summary>
        public float EngineTorque;
    }
}