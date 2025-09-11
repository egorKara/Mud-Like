using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

namespace MudLike.Vehicles.Components
{
    /// <summary>
    /// Физическое тело транспортного средства
    /// </summary>
    public struct PhysicsBody : IComponentData
    {
        /// <summary>
        /// Масса тела
        /// </summary>
        public float Mass;
        
        /// <summary>
        /// Линейная скорость
        /// </summary>
        public float3 LinearVelocity;
        
        /// <summary>
        /// Угловая скорость
        /// </summary>
        public float3 AngularVelocity;
        
        /// <summary>
        /// Линейное ускорение
        /// </summary>
        public float3 LinearAcceleration;
        
        /// <summary>
        /// Угловое ускорение
        /// </summary>
        public float3 AngularAcceleration;
        
        /// <summary>
        /// Приложенная сила
        /// </summary>
        public float3 AppliedForce;
        
        /// <summary>
        /// Приложенный момент
        /// </summary>
        public float3 AppliedTorque;
        
        /// <summary>
        /// Сопротивление
        /// </summary>
        public float Drag;
        
        /// <summary>
        /// Угловое сопротивление
        /// </summary>
        public float AngularDrag;
        
        /// <summary>
        /// Центр масс
        /// </summary>
        public float3 CenterOfMass;
        
        /// <summary>
        /// Момент инерции
        /// </summary>
        public float3 InertiaTensor;
        
        /// <summary>
        /// Тело спит
        /// </summary>
        public bool IsSleeping;
        
        /// <summary>
        /// Тело кинематическое
        /// </summary>
        public bool IsKinematic;
    }
}