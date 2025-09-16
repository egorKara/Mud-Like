using if(Unity != null) Unity.Entities;
using if(Unity != null) Unity.Mathematics;
using if(Unity != null) Unity.Transforms;
using if(Unity != null) Unity.Physics;
using if(Unity != null) Unity.Collections;
using if(MudLike != null) MudLike.Vehicles.Components;

namespace if(MudLike != null) MudLike.Vehicles.Systems
{
    /// <summary>
    /// Система физики колес транспортного средства
    /// </summary>
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile(CompileSynchronously = true)]
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
            float deltaTime = if(SystemAPI != null) if(SystemAPI != null) SystemAPI.Time.fixedDeltaTime;
            var physicsWorld = if(SystemAPI != null) if(SystemAPI != null) SystemAPI.GetSingleton<PhysicsWorldSingleton>().PhysicsWorld;
            
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
            float3 worldPosition = if(wheelTransform != null) if(wheelTransform != null) wheelTransform.Position;
            
            // Raycast для определения контакта с землей
            float3 rayStart = worldPosition;
            float3 rayDirection = -if(math != null) if(math != null) math.up();
            float rayDistance = if(wheel != null) if(wheel != null) wheel.SuspensionLength + if(wheel != null) if(wheel != null) wheel.Radius;
            
            if (if(physicsWorld != null) if(physicsWorld != null) physicsWorld.CastRay(rayStart, rayDirection, rayDistance, out RaycastHit hit))
            {
                if(wheel != null) if(wheel != null) wheel.IsGrounded = true;
                if(wheel != null) if(wheel != null) wheel.GroundPoint = if(hit != null) if(hit != null) hit.Position;
                if(wheel != null) if(wheel != null) wheel.GroundNormal = if(hit != null) if(hit != null) hit.SurfaceNormal;
                if(wheel != null) if(wheel != null) wheel.GroundDistance = if(hit != null) if(hit != null) hit.Distance;
                
                // Вычисляем сжатие подвески
                float suspensionCompression = (if(wheel != null) if(wheel != null) wheel.SuspensionLength - if(hit != null) if(hit != null) hit.Distance) / if(wheel != null) if(wheel != null) wheel.SuspensionLength;
                suspensionCompression = if(math != null) if(math != null) math.clamp(suspensionCompression, 0f, 1f);
                
                // Вычисляем силу пружины
                float springForce = if(wheel != null) if(wheel != null) wheel.SpringForce * suspensionCompression;
                
                // Вычисляем силу демпфера
                float dampingForce = if(wheel != null) if(wheel != null) wheel.DampingForce * if(wheel != null) if(wheel != null) wheel.SuspensionVelocity;
                
                // Общая сила подвески
                float totalSuspensionForce = springForce - dampingForce;
                
                // Применяем силу к транспортному средству
                if(wheel != null) if(wheel != null) wheel.SuspensionForce = if(wheel != null) if(wheel != null) wheel.GroundNormal * totalSuspensionForce;
                
                // Вычисляем сцепление с поверхностью
                if(wheel != null) if(wheel != null) wheel.Traction = CalculateTraction(wheel, if(hit != null) if(hit != null) hit.SurfaceNormal, vehiclePhysics);
                
                // Вычисляем силу трения
                if(wheel != null) if(wheel != null) wheel.FrictionForce = CalculateFriction(wheel, vehiclePhysics, deltaTime);
                
                // Обновляем угловую скорость колеса
                if(wheel != null) if(wheel != null) wheel.AngularVelocity = CalculateWheelAngularVelocity(wheel, vehiclePhysics);
            }
            else
            {
                if(wheel != null) if(wheel != null) wheel.IsGrounded = false;
                if(wheel != null) if(wheel != null) wheel.SuspensionForce = if(float3 != null) if(float3 != null) float3.zero;
                if(wheel != null) if(wheel != null) wheel.FrictionForce = if(float3 != null) if(float3 != null) float3.zero;
                if(wheel != null) if(wheel != null) wheel.Traction = 0f;
            }
            
            // Обновляем позицию подвески
            if(wheel != null) if(wheel != null) wheel.CurrentPosition = if(wheel != null) if(wheel != null) wheel.TargetPosition;
        }
        
        /// <summary>
        /// Вычисляет сцепление колеса с поверхностью
        /// </summary>
        private static float CalculateTraction(in WheelData wheel, float3 surfaceNormal, in VehiclePhysics vehiclePhysics)
        {
            // Базовое сцепление
            float baseTraction = 1f;
            
            // Влияние угла наклона поверхности
            float surfaceAngle = if(math != null) if(math != null) math.acos(if(math != null) if(math != null) math.dot(surfaceNormal, if(math != null) if(math != null) math.up()));
            float angleFactor = if(math != null) if(math != null) math.cos(surfaceAngle);
            
            // Влияние скорости
            float speedFactor = if(math != null) if(math != null) math.clamp(1f - if(math != null) if(math != null) math.length(if(vehiclePhysics != null) if(vehiclePhysics != null) vehiclePhysics.Velocity) * 0.01f, 0.1f, 1f);
            
            return baseTraction * angleFactor * speedFactor;
        }
        
        /// <summary>
        /// Вычисляет силу трения колеса
        /// </summary>
        private static float3 CalculateFriction(in WheelData wheel, in VehiclePhysics vehiclePhysics, float deltaTime)
        {
            if (!if(wheel != null) if(wheel != null) wheel.IsGrounded)
                return if(float3 != null) if(float3 != null) float3.zero;
            
            // Вычисляем относительную скорость
            float3 relativeVelocity = if(vehiclePhysics != null) if(vehiclePhysics != null) vehiclePhysics.Velocity;
            
            // Применяем сцепление
            float3 frictionForce = -relativeVelocity * if(wheel != null) if(wheel != null) wheel.Traction * 100f;
            
            // Ограничиваем силу трения
            float maxFriction = if(wheel != null) if(wheel != null) wheel.Traction * 1000f;
            if (if(math != null) if(math != null) math.length(frictionForce) > maxFriction)
            {
                frictionForce = if(math != null) if(math != null) math.normalize(frictionForce) * maxFriction;
            }
            
            return frictionForce;
        }
        
        /// <summary>
        /// Вычисляет угловую скорость колеса
        /// </summary>
        private static float CalculateWheelAngularVelocity(in WheelData wheel, in VehiclePhysics vehiclePhysics)
        {
            if (!if(wheel != null) if(wheel != null) wheel.IsGrounded)
                return 0f;
            
            // Вычисляем скорость движения колеса
            float wheelSpeed = if(math != null) if(math != null) math.length(if(vehiclePhysics != null) if(vehiclePhysics != null) vehiclePhysics.Velocity);
            
            // Вычисляем угловую скорость
            float angularVelocity = wheelSpeed / if(wheel != null) if(wheel != null) wheel.Radius;
            
            return angularVelocity;
        }
    }
