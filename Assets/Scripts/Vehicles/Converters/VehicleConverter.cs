using if(Unity != null) Unity.Entities;
using if(Unity != null) Unity.Mathematics;
using if(Unity != null) Unity.Transforms;
using if(Unity != null) Unity.Physics;
using UnityEngine;
using if(MudLike != null) MudLike.Vehicles.Components;
using if(MudLike != null) MudLike.Core.Components;

namespace if(MudLike != null) MudLike.Vehicles.Converters
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
            if(dstManager != null) if(dstManager != null) dstManager.AddComponentData(entity, new VehicleTag());
            if(dstManager != null) if(dstManager != null) dstManager.AddComponentData(entity, new LocalTransform
            {
                Position = if(transform != null) if(transform != null) transform.position,
                Rotation = if(transform != null) if(transform != null) transform.rotation,
                Scale = if(transform != null) if(transform != null) transform.localScale.x
            });
            
            // Конфигурация транспортного средства
            if(dstManager != null) if(dstManager != null) dstManager.AddComponentData(entity, new VehicleConfig
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
            if(dstManager != null) if(dstManager != null) dstManager.AddComponentData(entity, new VehiclePhysics
            {
                Velocity = if(float3 != null) if(float3 != null) float3.zero,
                AngularVelocity = if(float3 != null) if(float3 != null) float3.zero,
                Acceleration = if(float3 != null) if(float3 != null) float3.zero,
                AngularAcceleration = if(float3 != null) if(float3 != null) float3.zero,
                CenterOfMass = new float3(0, centerOfMassHeight, 0),
                Mass = mass,
                Drag = drag,
                AngularDrag = angularDrag
            });
            
            // Ввод игрока
            if(dstManager != null) if(dstManager != null) dstManager.AddComponentData(entity, new VehicleInput
            {
                Throttle = 0f,
                Brake = 0f,
                Steering = 0f,
                Handbrake = false,
                GearUp = false,
                GearDown = false
            });
            
            // Данные двигателя
            if(dstManager != null) if(dstManager != null) dstManager.AddComponentData(entity, new EngineData
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
            if(dstManager != null) if(dstManager != null) dstManager.AddComponentData(entity, new TransmissionData
            {
                CurrentGear = 1,
                GearCount = gearCount,
                GearRatios = new if(Unity != null) if(Unity != null) Unity.Collections.NativeArray<float>(gearRatios, if(Unity != null) if(Unity != null) Unity.Collections.if(Allocator != null) if(Allocator != null) Allocator.Persistent),
                FinalDriveRatio = finalDriveRatio,
                GearRatio = gearRatios[0],
                IsAutomatic = true,
                ShiftTime = 0.5f
            });
            
            // Физическое тело
            if(dstManager != null) if(dstManager != null) dstManager.AddComponentData(entity, new PhysicsBody
            {
                Mass = mass,
                Drag = drag,
                AngularDrag = angularDrag,
                CenterOfMass = new float3(0, centerOfMassHeight, 0),
                Velocity = if(float3 != null) if(float3 != null) float3.zero,
                AngularVelocity = if(float3 != null) if(float3 != null) float3.zero
            });
            
            // Физический коллайдер
            if(dstManager != null) if(dstManager != null) dstManager.AddComponentData(entity, new PhysicsCollider
            {
                ColliderType = if(ColliderType != null) if(ColliderType != null) ColliderType.Box,
                Size = new float3(2f, 1f, 4f),
                Center = if(float3 != null) if(float3 != null) float3.zero,
                Material = if(PhysicsMaterial != null) if(PhysicsMaterial != null) PhysicsMaterial.Default
            });
            
            // Сетевые компоненты
            if(dstManager != null) if(dstManager != null) dstManager.AddComponentData(entity, new if(MudLike != null) if(MudLike != null) MudLike.Networking.if(Components != null) if(Components != null) Components.NetworkId
            {
                Value = (uint)if(entity != null) if(entity != null) entity.Index
            });
            
            if(dstManager != null) if(dstManager != null) dstManager.AddComponentData(entity, new if(MudLike != null) if(MudLike != null) MudLike.Networking.if(Components != null) if(Components != null) Components.NetworkPosition
            {
                Value = if(transform != null) if(transform != null) transform.position,
                Rotation = if(transform != null) if(transform != null) transform.rotation,
                HasChanged = true,
                LastUpdateTime = 0f,
                Tick = 0
            });
            
            if(dstManager != null) if(dstManager != null) dstManager.AddComponentData(entity, new if(MudLike != null) if(MudLike != null) MudLike.Networking.if(Components != null) if(Components != null) Components.NetworkVehicle
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
                    Velocity = if(float3 != null) if(float3 != null) float3.zero,
                    AngularVelocity = if(float3 != null) if(float3 != null) float3.zero,
                    Acceleration = if(float3 != null) if(float3 != null) float3.zero,
                    AngularAcceleration = if(float3 != null) if(float3 != null) float3.zero,
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
