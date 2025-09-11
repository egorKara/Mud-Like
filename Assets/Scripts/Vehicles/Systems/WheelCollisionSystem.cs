using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;
using Unity.Collections;
using MudLike.Vehicles.Components;

namespace MudLike.Vehicles.Systems
{
    /// <summary>
    /// Система коллизий колес с Unity Physics
    /// </summary>
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial class WheelCollisionSystem : SystemBase
    {
        private PhysicsWorld _physicsWorld;
        
        protected override void OnCreate()
        {
            _physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().PhysicsWorld;
        }
        
        /// <summary>
        /// Обрабатывает коллизии всех колес
        /// </summary>
        protected override void OnUpdate()
        {
            Entities
                .WithAll<WheelData>()
                .ForEach((ref WheelData wheel, in LocalTransform wheelTransform) =>
                {
                    ProcessWheelCollision(ref wheel, wheelTransform);
                }).Schedule();
        }
        
        /// <summary>
        /// Обрабатывает коллизию конкретного колеса
        /// </summary>
        private void ProcessWheelCollision(ref WheelData wheel, in LocalTransform wheelTransform)
        {
            // Вычисляем параметры raycast
            float3 rayStart = wheelTransform.Position;
            float3 rayDirection = -math.up();
            float rayDistance = wheel.SuspensionLength + wheel.Radius;
            
            // Выполняем raycast
            if (PhysicsWorld.CastRay(_physicsWorld, rayStart, rayDirection, rayDistance, out RaycastHit hit))
            {
                // Обновляем данные колеса
                wheel.IsGrounded = true;
                wheel.GroundPoint = hit.Position;
                wheel.GroundNormal = hit.SurfaceNormal;
                wheel.GroundDistance = hit.Distance;
                
                // Вычисляем сжатие подвески
                float suspensionCompression = (wheel.SuspensionLength - hit.Distance) / wheel.SuspensionLength;
                suspensionCompression = math.clamp(suspensionCompression, 0f, 1f);
                
                // Вычисляем силу подвески
                float springForce = wheel.SpringForce * suspensionCompression;
                float dampingForce = wheel.DampingForce * wheel.SuspensionVelocity;
                float totalSuspensionForce = springForce - dampingForce;
                
                // Применяем силу подвески
                wheel.SuspensionForce = wheel.GroundNormal * totalSuspensionForce;
                
                // Вычисляем сцепление с поверхностью
                wheel.Traction = CalculateSurfaceTraction(hit.SurfaceNormal, hit.SurfaceMaterial);
                
                // Вычисляем силу трения
                wheel.FrictionForce = CalculateFrictionForce(wheel, hit.SurfaceNormal);
            }
            else
            {
                // Колесо не касается земли
                wheel.IsGrounded = false;
                wheel.SuspensionForce = float3.zero;
                wheel.FrictionForce = float3.zero;
                wheel.Traction = 0f;
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
            float surfaceAngle = math.acos(math.dot(surfaceNormal, math.up()));
            float angleFactor = math.cos(surfaceAngle);
            
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
            if (!wheel.IsGrounded)
                return float3.zero;
            
            // Вычисляем относительную скорость
            float3 relativeVelocity = wheel.FrictionForce; // Упрощенная модель
            
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
    }
}