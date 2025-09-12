using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;
using MudLike.Vehicles.Components;
using MudLike.Core.Components;

namespace MudLike.Vehicles.Converters
{
    /// <summary>
    /// Конвертер для создания ECS сущности транспортного средства
    /// </summary>
    public class VehicleConverter : MonoBehaviour, IConvertGameObjectToEntity
    {
        [Header("Vehicle Configuration")]
        public float maxSpeed = 50f;
        public float acceleration = 10f;
        public float turnSpeed = 2f;
        public float mass = 1500f;
        public float drag = 0.3f;
        public float angularDrag = 5f;
        public float turnRadius = 5f;
        public float centerOfMassHeight = 0.5f;
        
        [Header("Engine Configuration")]
        public float maxRPM = 6000f;
        public float idleRPM = 800f;
        public float torque = 300f;
        public float power = 200f;
        
        [Header("Transmission Configuration")]
        public int gearCount = 5;
        public float[] gearRatios = { 3.5f, 2.1f, 1.4f, 1.0f, 0.8f };
        public float finalDriveRatio = 3.5f;
        
        [Header("Wheel Configuration")]
        public float wheelRadius = 0.3f;
        public float suspensionLength = 0.5f;
        public float springForce = 35000f;
        public float dampingForce = 4500f;
        
        /// <summary>
        /// Конвертирует GameObject в ECS сущность
        /// </summary>
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            // Добавляем основные компоненты
            dstManager.AddComponentData(entity, new VehicleTag());
            dstManager.AddComponentData(entity, new LocalTransform
            {
                Position = transform.position,
                Rotation = transform.rotation,
                Scale = transform.localScale.x
            });
            
            // Конфигурация транспортного средства
            dstManager.AddComponentData(entity, new VehicleConfig
            {
                MaxSpeed = maxSpeed,
                Acceleration = acceleration,
                TurnSpeed = turnSpeed,
                Mass = mass,
                Drag = drag,
                AngularDrag = angularDrag,
                TurnRadius = turnRadius,
                CenterOfMassHeight = centerOfMassHeight
            });
            
            // Физика транспортного средства
            dstManager.AddComponentData(entity, new VehiclePhysics
            {
                Velocity = float3.zero,
                AngularVelocity = float3.zero,
                Acceleration = float3.zero,
                AngularAcceleration = float3.zero,
                CenterOfMass = new float3(0, centerOfMassHeight, 0),
                Mass = mass,
                Drag = drag,
                AngularDrag = angularDrag
            });
            
            // Ввод игрока
            dstManager.AddComponentData(entity, new VehicleInput
            {
                Throttle = 0f,
                Brake = 0f,
                Steering = 0f,
                Handbrake = false,
                GearUp = false,
                GearDown = false
            });
            
            // Данные двигателя
            dstManager.AddComponentData(entity, new EngineData
            {
                CurrentRPM = idleRPM,
                MaxRPM = maxRPM,
                IdleRPM = idleRPM,
                Torque = torque,
                Power = power,
                ThrottleInput = 0f,
                IsRunning = true
            });
            
            // Данные трансмиссии
            dstManager.AddComponentData(entity, new TransmissionData
            {
                CurrentGear = 1,
                GearCount = gearCount,
                GearRatios = new Unity.Collections.NativeArray<float>(gearRatios, Unity.Collections.Allocator.Persistent),
                FinalDriveRatio = finalDriveRatio,
                GearRatio = gearRatios[0],
                IsAutomatic = true,
                ShiftTime = 0.5f
            });
            
            // Физическое тело
            dstManager.AddComponentData(entity, new PhysicsBody
            {
                Mass = mass,
                Drag = drag,
                AngularDrag = angularDrag,
                CenterOfMass = new float3(0, centerOfMassHeight, 0),
                Velocity = float3.zero,
                AngularVelocity = float3.zero
            });
            
            // Физический коллайдер
            dstManager.AddComponentData(entity, new PhysicsCollider
            {
                ColliderType = ColliderType.Box,
                Size = new float3(2f, 1f, 4f),
                Center = float3.zero,
                Material = PhysicsMaterial.Default
            });
            
            // Сетевые компоненты
            dstManager.AddComponentData(entity, new MudLike.Networking.Components.NetworkId
            {
                Value = (uint)entity.Index
            });
            
            dstManager.AddComponentData(entity, new MudLike.Networking.Components.NetworkPosition
            {
                Value = transform.position,
                Rotation = transform.rotation,
                HasChanged = true,
                LastUpdateTime = 0f,
                Tick = 0
            });
            
            dstManager.AddComponentData(entity, new MudLike.Networking.Components.NetworkVehicle
            {
                Config = new VehicleConfig
                {
                    MaxSpeed = maxSpeed,
                    Acceleration = acceleration,
                    TurnSpeed = turnSpeed,
                    Mass = mass,
                    Drag = drag,
                    AngularDrag = angularDrag,
                    TurnRadius = turnRadius,
                    CenterOfMassHeight = centerOfMassHeight
                },
                Physics = new VehiclePhysics
                {
                    Velocity = float3.zero,
                    AngularVelocity = float3.zero,
                    Acceleration = float3.zero,
                    AngularAcceleration = float3.zero,
                    CenterOfMass = new float3(0, centerOfMassHeight, 0),
                    Mass = mass,
                    Drag = drag,
                    AngularDrag = angularDrag
                },
                HasChanged = true,
                LastUpdateTime = 0f,
                Tick = 0
            });
        }
    }
}