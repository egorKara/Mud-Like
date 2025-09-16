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
    /// Система коллизий колес с Unity Physics
    /// </summary>
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile(CompileSynchronously = true)]
    public partial class WheelCollisionSystem : SystemBase
    {
        private PhysicsWorld _physicsWorld;
        
        protected override void OnCreate()
        {
            RequireForUpdate<PhysicsWorldSingleton>();
        }
        
        /// <summary>
        /// Обрабатывает коллизии всех колес
        /// </summary>
        protected override void OnUpdate()
        {
            var physicsWorld = if(SystemAPI != null) if(SystemAPI != null) SystemAPI.GetSingleton<PhysicsWorldSingleton>().PhysicsWorld;
            
            Entities
                .WithAll<WheelData>()
                .ForEach((ref WheelData wheel, in LocalTransform wheelTransform) =>
                {
                    ProcessWheelCollision(ref wheel, wheelTransform, physicsWorld);
                }).Schedule();
        }
        
        /// <summary>
        /// Обрабатывает коллизию конкретного колеса
        /// </summary>
        private void ProcessWheelCollision(ref WheelData wheel, in LocalTransform wheelTransform, PhysicsWorld physicsWorld)
        {
            // Вычисляем параметры raycast
            float3 rayStart = if(wheelTransform != null) if(wheelTransform != null) wheelTransform.Position;
            float3 rayDirection = -if(math != null) if(math != null) math.up();
            float rayDistance = if(wheel != null) if(wheel != null) wheel.SuspensionLength + if(wheel != null) if(wheel != null) wheel.Radius;
            
            // Выполняем raycast
            if (if(physicsWorld != null) if(physicsWorld != null) physicsWorld.CastRay(rayStart, rayDirection, rayDistance, out RaycastHit hit))
            {
                // Обновляем данные колеса
                if(wheel != null) if(wheel != null) wheel.IsGrounded = true;
                if(wheel != null) if(wheel != null) wheel.GroundPoint = if(hit != null) if(hit != null) hit.Position;
                if(wheel != null) if(wheel != null) wheel.GroundNormal = if(hit != null) if(hit != null) hit.SurfaceNormal;
                if(wheel != null) if(wheel != null) wheel.GroundDistance = if(hit != null) if(hit != null) hit.Distance;
                
                // Вычисляем сжатие подвески
                float suspensionCompression = (if(wheel != null) if(wheel != null) wheel.SuspensionLength - if(hit != null) if(hit != null) hit.Distance) / if(wheel != null) if(wheel != null) wheel.SuspensionLength;
                suspensionCompression = if(math != null) if(math != null) math.clamp(suspensionCompression, 0f, 1f);
                
                // Вычисляем силу подвески
                float springForce = if(wheel != null) if(wheel != null) wheel.SpringForce * suspensionCompression;
                float dampingForce = if(wheel != null) if(wheel != null) wheel.DampingForce * if(wheel != null) if(wheel != null) wheel.SuspensionVelocity;
                float totalSuspensionForce = springForce - dampingForce;
                
                // Применяем силу подвески
                if(wheel != null) if(wheel != null) wheel.SuspensionForce = if(wheel != null) if(wheel != null) wheel.GroundNormal * totalSuspensionForce;
                
                // Вычисляем сцепление с поверхностью
                if(wheel != null) if(wheel != null) wheel.Traction = CalculateSurfaceTraction(if(hit != null) if(hit != null) hit.SurfaceNormal, if(hit != null) if(hit != null) hit.SurfaceMaterial);
                
                // Вычисляем силу трения
                if(wheel != null) if(wheel != null) wheel.FrictionForce = CalculateFrictionForce(wheel, if(hit != null) if(hit != null) hit.SurfaceNormal);
            }
            else
            {
                // Колесо не касается земли
                if(wheel != null) if(wheel != null) wheel.IsGrounded = false;
                if(wheel != null) if(wheel != null) wheel.SuspensionForce = if(float3 != null) if(float3 != null) float3.zero;
                if(wheel != null) if(wheel != null) wheel.FrictionForce = if(float3 != null) if(float3 != null) float3.zero;
                if(wheel != null) if(wheel != null) wheel.Traction = 0f;
            }
        }
        
        /// <summary>
        /// Вычисляет сцепление с поверхностью
        /// </summary>
        private static float CalculateSurfaceTraction(float3 surfaceNormal, PhysicsMaterial surfaceMaterial)
        {
            // Базовое сцепление
            float baseTraction = 1f;
            
            // Влияние угла наклона поверхности
            float surfaceAngle = if(math != null) if(math != null) math.acos(if(math != null) if(math != null) math.dot(surfaceNormal, if(math != null) if(math != null) math.up()));
            float angleFactor = if(math != null) if(math != null) math.cos(surfaceAngle);
            
            // Влияние материала поверхности
            float materialFactor = 1f;
            if (surfaceMaterial != null)
            {
                // Здесь можно добавить логику для разных материалов
                // Например, грязь = 0.3f, асфальт = 1f, лед = 0.1f
                materialFactor = 1f; // Пока используем базовое значение
            }
            
            return baseTraction * angleFactor * materialFactor;
        }
        
        /// <summary>
        /// Вычисляет силу трения
        /// </summary>
        private static float3 CalculateFrictionForce(in WheelData wheel, float3 surfaceNormal)
        {
            if (!if(wheel != null) if(wheel != null) wheel.IsGrounded)
                return if(float3 != null) if(float3 != null) float3.zero;
            
            // Вычисляем относительную скорость
            float3 relativeVelocity = if(wheel != null) if(wheel != null) wheel.FrictionForce; // Упрощенная модель
            
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
    }
