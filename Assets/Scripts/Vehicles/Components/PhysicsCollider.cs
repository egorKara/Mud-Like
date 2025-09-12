using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

namespace MudLike.Vehicles.Components
{
    /// <summary>
    /// Коллайдер транспортного средства
    /// </summary>
    public struct PhysicsCollider : IComponentData
    {
        /// <summary>
        /// Тип коллайдера
        /// </summary>
        public ColliderType Type;
        
        /// <summary>
        /// Размеры коллайдера
        /// </summary>
        public float3 Size;
        
        /// <summary>
        /// Радиус коллайдера (для сферы/капсулы)
        /// </summary>
        public float Radius;
        
        /// <summary>
        /// Высота коллайдера (для капсулы/цилиндра)
        /// </summary>
        public float Height;
        
        /// <summary>
        /// Материал коллайдера
        /// </summary>
        public PhysicsMaterial Material;
        
        /// <summary>
        /// Коллайдер является триггером
        /// </summary>
        public bool IsTrigger;
        
        /// <summary>
        /// Коллайдер активен
        /// </summary>
        public bool IsActive;
        
        /// <summary>
        /// Слой коллайдера
        /// </summary>
        public int Layer;
        
        /// <summary>
        /// Маска коллизий
        /// </summary>
        public int CollisionMask;
    }
    
    /// <summary>
    /// Тип коллайдера
    /// </summary>
    public enum ColliderType
    {
        Box,
        Sphere,
        Capsule,
        Cylinder,
        Mesh
    }
}