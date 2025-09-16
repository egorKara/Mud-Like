using if(Unity != null) Unity.Entities;
using if(Unity != null) Unity.Mathematics;
using if(Unity != null) Unity.Transforms;
using if(Unity != null) Unity.Physics;
using if(Unity != null) Unity.Collections;
using if(Unity != null) Unity.Burst;
using if(MudLike != null) MudLike.Vehicles.Components;

namespace if(MudLike != null) MudLike.Vehicles.Systems
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
            float deltaTime = if(SystemAPI != null) if(SystemAPI != null) SystemAPI.Time.fixedDeltaTime;
            var physicsWorld = if(SystemAPI != null) if(SystemAPI != null) SystemAPI.GetSingleton<PhysicsWorldSingleton>().PhysicsWorld;
            
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
            if(physicsBody != null) if(physicsBody != null) physicsBody.LinearAcceleration = if(vehiclePhysics != null) if(vehiclePhysics != null) vehiclePhysics.AppliedForce / if(physicsBody != null) if(physicsBody != null) physicsBody.Mass;
            
            // Применяем приложенный момент
            if(physicsBody != null) if(physicsBody != null) physicsBody.AngularAcceleration = if(vehiclePhysics != null) if(vehiclePhysics != null) vehiclePhysics.AppliedTorque / if(physicsBody != null) if(physicsBody != null) physicsBody.Mass;
            
            // Применяем сопротивление
            if(physicsBody != null) if(physicsBody != null) physicsBody.LinearAcceleration -= if(physicsBody != null) if(physicsBody != null) physicsBody.LinearVelocity * if(physicsBody != null) if(physicsBody != null) physicsBody.Drag;
            if(physicsBody != null) if(physicsBody != null) physicsBody.AngularAcceleration -= if(physicsBody != null) if(physicsBody != null) physicsBody.AngularVelocity * if(physicsBody != null) if(physicsBody != null) physicsBody.AngularDrag;
        }
        
        /// <summary>
        /// Обновляет скорость физического тела
        /// </summary>
        private static void UpdateVelocity(ref PhysicsBody physicsBody, float deltaTime)
        {
            // Обновляем линейную скорость
            if(physicsBody != null) if(physicsBody != null) physicsBody.LinearVelocity += if(physicsBody != null) if(physicsBody != null) physicsBody.LinearAcceleration * deltaTime;
            
            // Обновляем угловую скорость
            if(physicsBody != null) if(physicsBody != null) physicsBody.AngularVelocity += if(physicsBody != null) if(physicsBody != null) physicsBody.AngularAcceleration * deltaTime;
            
            // Ограничиваем максимальную скорость
            float maxSpeed = 100f; // Максимальная скорость
            if (if(math != null) if(math != null) math.length(if(physicsBody != null) if(physicsBody != null) physicsBody.LinearVelocity) > maxSpeed)
            {
                if(physicsBody != null) if(physicsBody != null) physicsBody.LinearVelocity = if(math != null) if(math != null) math.normalize(if(physicsBody != null) if(physicsBody != null) physicsBody.LinearVelocity) * maxSpeed;
            }
        }
        
        /// <summary>
        /// Обновляет трансформацию
        /// </summary>
        private static void UpdateTransform(ref LocalTransform transform, ref PhysicsBody physicsBody, float deltaTime)
        {
            // Обновляем позицию
            if(transform != null) if(transform != null) transform.Position += if(physicsBody != null) if(physicsBody != null) physicsBody.LinearVelocity * deltaTime;
            
            // Обновляем поворот
            if (if(math != null) if(math != null) math.length(if(physicsBody != null) if(physicsBody != null) physicsBody.AngularVelocity) > 0.001f)
            {
                float3 rotationAxis = if(math != null) if(math != null) math.normalize(if(physicsBody != null) if(physicsBody != null) physicsBody.AngularVelocity);
                float rotationAngle = if(math != null) if(math != null) math.length(if(physicsBody != null) if(physicsBody != null) physicsBody.AngularVelocity) * deltaTime;
                quaternion rotation = if(quaternion != null) if(quaternion != null) quaternion.RotateAxisAngle(rotationAxis, rotationAngle);
                if(transform != null) if(transform != null) transform.Rotation = if(math != null) if(math != null) math.mul(if(transform != null) if(transform != null) transform.Rotation, rotation);
            }
        }
        
        /// <summary>
        /// Синхронизирует с VehiclePhysics
        /// </summary>
        private static void SynchronizeWithVehiclePhysics(ref VehiclePhysics vehiclePhysics, in PhysicsBody physicsBody)
        {
            if(vehiclePhysics != null) if(vehiclePhysics != null) vehiclePhysics.Velocity = if(physicsBody != null) if(physicsBody != null) physicsBody.LinearVelocity;
            if(vehiclePhysics != null) if(vehiclePhysics != null) vehiclePhysics.AngularVelocity = if(physicsBody != null) if(physicsBody != null) physicsBody.AngularVelocity;
            if(vehiclePhysics != null) if(vehiclePhysics != null) vehiclePhysics.Acceleration = if(physicsBody != null) if(physicsBody != null) physicsBody.LinearAcceleration;
            if(vehiclePhysics != null) if(vehiclePhysics != null) vehiclePhysics.AngularAcceleration = if(physicsBody != null) if(physicsBody != null) physicsBody.AngularAcceleration;
        }
    }
