using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;
using Unity.Collections;
using MudLike.Vehicles.Components;

namespace MudLike.Vehicles.Systems
{
    /// <summary>
    /// Система физики колес транспортного средства
    /// </summary>
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial class WheelPhysicsSystem : SystemBase
    {
        private PhysicsWorld _physicsWorld;
        
        protected override void OnCreate()
        {
            // Получаем PhysicsWorld через SystemAPI для Unity 2022.3.62f1
            RequireForUpdate<PhysicsWorldSingleton>();
        }
        
        /// <summary>
        /// Обрабатывает физику всех колес
        /// </summary>
        protected override void OnUpdate()
        {
            float deltaTime = Time.fixedDeltaTime;
            var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().PhysicsWorld;
            
            Entities
                .WithAll<WheelData>()
                .ForEach((ref WheelData wheel, 
                         in LocalTransform wheelTransform,
                         in VehiclePhysics vehiclePhysics) =>
                {
                    ProcessWheelPhysics(ref wheel, wheelTransform, vehiclePhysics, deltaTime, physicsWorld);
                }).Schedule();
        }
        
        /// <summary>
        /// Обрабатывает физику конкретного колеса
        /// </summary>
        private void ProcessWheelPhysics(ref WheelData wheel, 
                                       in LocalTransform wheelTransform, 
                                       in VehiclePhysics vehiclePhysics, 
                                       float deltaTime,
                                       PhysicsWorld physicsWorld)
        {
            // Вычисляем мировую позицию колеса
            float3 worldPosition = wheelTransform.Position;
            
            // Raycast для определения контакта с землей
            float3 rayStart = worldPosition;
            float3 rayDirection = -math.up();
            float rayDistance = wheel.SuspensionLength + wheel.Radius;
            
            if (physicsWorld.CastRay(rayStart, rayDirection, rayDistance, out RaycastHit hit))
            {
                wheel.IsGrounded = true;
                wheel.GroundPoint = hit.Position;
                wheel.GroundNormal = hit.SurfaceNormal;
                wheel.GroundDistance = hit.Distance;
                
                // Вычисляем сжатие подвески
                float suspensionCompression = (wheel.SuspensionLength - hit.Distance) / wheel.SuspensionLength;
                suspensionCompression = math.clamp(suspensionCompression, 0f, 1f);
                
                // Вычисляем силу пружины
                float springForce = wheel.SpringForce * suspensionCompression;
                
                // Вычисляем силу демпфера
                float dampingForce = wheel.DampingForce * wheel.SuspensionVelocity;
                
                // Общая сила подвески
                float totalSuspensionForce = springForce - dampingForce;
                
                // Применяем силу к транспортному средству
                wheel.SuspensionForce = wheel.GroundNormal * totalSuspensionForce;
                
                // Вычисляем сцепление с поверхностью
                wheel.Traction = CalculateTraction(wheel, hit.SurfaceNormal, vehiclePhysics);
                
                // Вычисляем силу трения
                wheel.FrictionForce = CalculateFriction(wheel, vehiclePhysics, deltaTime);
                
                // Обновляем угловую скорость колеса
                wheel.AngularVelocity = CalculateWheelAngularVelocity(wheel, vehiclePhysics);
            }
            else
            {
                wheel.IsGrounded = false;
                wheel.SuspensionForce = float3.zero;
                wheel.FrictionForce = float3.zero;
                wheel.Traction = 0f;
            }
            
            // Обновляем позицию подвески
            wheel.CurrentPosition = wheel.TargetPosition;
        }
        
        /// <summary>
        /// Вычисляет сцепление колеса с поверхностью
        /// </summary>
        private static float CalculateTraction(in WheelData wheel, float3 surfaceNormal, in VehiclePhysics vehiclePhysics)
        {
            // Базовое сцепление
            float baseTraction = 1f;
            
            // Влияние угла наклона поверхности
            float surfaceAngle = math.acos(math.dot(surfaceNormal, math.up()));
            float angleFactor = math.cos(surfaceAngle);
            
            // Влияние скорости
            float speedFactor = math.clamp(1f - math.length(vehiclePhysics.Velocity) * 0.01f, 0.1f, 1f);
            
            return baseTraction * angleFactor * speedFactor;
        }
        
        /// <summary>
        /// Вычисляет силу трения колеса
        /// </summary>
        private static float3 CalculateFriction(in WheelData wheel, in VehiclePhysics vehiclePhysics, float deltaTime)
        {
            if (!wheel.IsGrounded)
                return float3.zero;
            
            // Вычисляем относительную скорость
            float3 relativeVelocity = vehiclePhysics.Velocity;
            
            // Применяем сцепление
            float3 frictionForce = -relativeVelocity * wheel.Traction * 100f;
            
            // Ограничиваем силу трения
            float maxFriction = wheel.Traction * 1000f;
            if (math.length(frictionForce) > maxFriction)
            {
                frictionForce = math.normalize(frictionForce) * maxFriction;
            }
            
            return frictionForce;
        }
        
        /// <summary>
        /// Вычисляет угловую скорость колеса
        /// </summary>
        private static float CalculateWheelAngularVelocity(in WheelData wheel, in VehiclePhysics vehiclePhysics)
        {
            if (!wheel.IsGrounded)
                return 0f;
            
            // Вычисляем скорость движения колеса
            float wheelSpeed = math.length(vehiclePhysics.Velocity);
            
            // Вычисляем угловую скорость
            float angularVelocity = wheelSpeed / wheel.Radius;
            
            return angularVelocity;
        }
    }
}