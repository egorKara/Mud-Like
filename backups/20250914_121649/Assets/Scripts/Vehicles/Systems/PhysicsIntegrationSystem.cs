using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;
using Unity.Collections;
using Unity.Burst;
using MudLike.Vehicles.Components;

namespace MudLike.Vehicles.Systems
{
    /// <summary>
    /// Система интеграции с Unity Physics
    /// </summary>
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile(CompileSynchronously = true)]
    public partial class PhysicsIntegrationSystem : SystemBase
    {
        private PhysicsWorld _physicsWorld;
        
        protected override void OnCreate()
        {
            RequireForUpdate<PhysicsWorldSingleton>();
        }
        
        /// <summary>
        /// Обрабатывает интеграцию с Unity Physics
        /// </summary>
        protected override void OnUpdate()
        {
            float deltaTime = SystemAPI.Time.fixedDeltaTime;
            var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().PhysicsWorld;
            
            Entities
                .WithAll<VehicleTag, PhysicsBody>()
                .ForEach((ref LocalTransform transform, 
                         ref PhysicsBody physicsBody, 
                         ref VehiclePhysics vehiclePhysics) =>
                {
                    ProcessPhysicsIntegration(ref transform, ref physicsBody, ref vehiclePhysics, deltaTime, physicsWorld);
                }).Schedule();
        }
        
        /// <summary>
        /// Обрабатывает интеграцию физики конкретного транспортного средства
        /// </summary>
        private void ProcessPhysicsIntegration(ref LocalTransform transform, 
                                            ref PhysicsBody physicsBody, 
                                            ref VehiclePhysics vehiclePhysics, 
                                            float deltaTime,
                                            PhysicsWorld physicsWorld)
        {
            // Применяем силы к физическому телу
            ApplyForces(ref physicsBody, vehiclePhysics, deltaTime);
            
            // Обновляем скорость
            UpdateVelocity(ref physicsBody, deltaTime);
            
            // Обновляем позицию и поворот
            UpdateTransform(ref transform, ref physicsBody, deltaTime);
            
            // Синхронизируем с VehiclePhysics
            SynchronizeWithVehiclePhysics(ref vehiclePhysics, physicsBody);
        }
        
        /// <summary>
        /// Применяет силы к физическому телу
        /// </summary>
        private static void ApplyForces(ref PhysicsBody physicsBody, in VehiclePhysics vehiclePhysics, float deltaTime)
        {
            // Применяем приложенную силу
            physicsBody.LinearAcceleration = vehiclePhysics.AppliedForce / physicsBody.Mass;
            
            // Применяем приложенный момент
            physicsBody.AngularAcceleration = vehiclePhysics.AppliedTorque / physicsBody.Mass;
            
            // Применяем сопротивление
            physicsBody.LinearAcceleration -= physicsBody.LinearVelocity * physicsBody.Drag;
            physicsBody.AngularAcceleration -= physicsBody.AngularVelocity * physicsBody.AngularDrag;
        }
        
        /// <summary>
        /// Обновляет скорость физического тела
        /// </summary>
        private static void UpdateVelocity(ref PhysicsBody physicsBody, float deltaTime)
        {
            // Обновляем линейную скорость
            physicsBody.LinearVelocity += physicsBody.LinearAcceleration * deltaTime;
            
            // Обновляем угловую скорость
            physicsBody.AngularVelocity += physicsBody.AngularAcceleration * deltaTime;
            
            // Ограничиваем максимальную скорость
            float maxSpeed = 100f; // Максимальная скорость
            if (math.length(physicsBody.LinearVelocity) > maxSpeed)
            {
                physicsBody.LinearVelocity = math.normalize(physicsBody.LinearVelocity) * maxSpeed;
            }
        }
        
        /// <summary>
        /// Обновляет трансформацию
        /// </summary>
        private static void UpdateTransform(ref LocalTransform transform, ref PhysicsBody physicsBody, float deltaTime)
        {
            // Обновляем позицию
            transform.Position += physicsBody.LinearVelocity * deltaTime;
            
            // Обновляем поворот
            if (math.length(physicsBody.AngularVelocity) > 0.001f)
            {
                float3 rotationAxis = math.normalize(physicsBody.AngularVelocity);
                float rotationAngle = math.length(physicsBody.AngularVelocity) * deltaTime;
                quaternion rotation = quaternion.RotateAxisAngle(rotationAxis, rotationAngle);
                transform.Rotation = math.mul(transform.Rotation, rotation);
            }
        }
        
        /// <summary>
        /// Синхронизирует с VehiclePhysics
        /// </summary>
        private static void SynchronizeWithVehiclePhysics(ref VehiclePhysics vehiclePhysics, in PhysicsBody physicsBody)
        {
            vehiclePhysics.Velocity = physicsBody.LinearVelocity;
            vehiclePhysics.AngularVelocity = physicsBody.AngularVelocity;
            vehiclePhysics.Acceleration = physicsBody.LinearAcceleration;
            vehiclePhysics.AngularAcceleration = physicsBody.AngularAcceleration;
        }
    }
}