using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;
using Unity.Collections;
using MudLike.Vehicles.Components;

namespace MudLike.Vehicles.Systems
{
    /// <summary>
    /// ECS система для физики колес
    /// </summary>
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial class WheelPhysicsSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            float deltaTime = SystemAPI.Time.fixedDeltaTime;
            
            // Обновление физики колес
            Entities.ForEach((ref WheelComponent wheel, in LocalTransform transform) =>
            {
                // Обновление угловой скорости колеса
                UpdateWheelAngularVelocityStatic(ref wheel, deltaTime);
                
                // Проверка контакта с поверхностью
                CheckGroundContactStatic(ref wheel, transform);
                
                // Расчет сил трения
                CalculateFrictionForcesStatic(ref wheel, deltaTime);
                
                // Обновление скольжения
                UpdateSlipStatic(ref wheel, deltaTime);
                
            }).Schedule();
        }
        
        /// <summary>
        /// Обновление угловой скорости колеса
        /// </summary>
        private static void UpdateWheelAngularVelocityStatic(ref WheelComponent wheel, float deltaTime)
        {
            // Применение тормозной силы
            if (wheel.brakeForce > 0)
            {
                float brakeTorque = wheel.brakeForce * wheel.radius;
                float brakeAcceleration = brakeTorque / wheel.inertia;
                wheel.angularVelocity -= brakeAcceleration * deltaTime;
            }
            
            // Ограничение угловой скорости
            float maxAngularVelocity = 1000f; // рад/с
            wheel.angularVelocity = math.clamp(wheel.angularVelocity, -maxAngularVelocity, maxAngularVelocity);
        }
        
        /// <summary>
        /// Проверка контакта с поверхностью
        /// </summary>
        private static void CheckGroundContactStatic(ref WheelComponent wheel, LocalTransform transform)
        {
            // Простая проверка контакта с поверхностью
            float3 wheelCenter = transform.Position + math.mul(transform.Rotation, wheel.localPosition);
            float3 rayDirection = math.mul(transform.Rotation, new float3(0, -1, 0));
            
            // Raycast вниз для проверки контакта
            float rayDistance = wheel.radius + 0.1f;
            
            // Здесь должен быть реальный raycast с Unity Physics
            // Пока используем упрощенную логику
            wheel.isGrounded = wheelCenter.y <= wheel.radius + 0.1f;
            
            if (wheel.isGrounded)
            {
                wheel.groundDistance = wheelCenter.y - wheel.radius;
                wheel.groundNormal = new float3(0, 1, 0);
                wheel.contactPoint = new float3(wheelCenter.x, wheel.radius, wheelCenter.z);
                
                // Создание взаимодействия с местностью
                CreateTerrainInteraction(wheelCenter, wheel.mass * 9.81f, wheel.angularVelocity * wheel.radius, wheel.radius);
            }
            else
            {
                wheel.groundDistance = float.MaxValue;
                wheel.groundNormal = float3.zero;
                wheel.contactPoint = float3.zero;
            }
        }
        
        /// <summary>
        /// Создание взаимодействия с местностью
        /// </summary>
        private static void CreateTerrainInteraction(float3 position, float force, float3 velocity, float radius)
        {
            // Здесь должна быть логика создания взаимодействия с местностью
            // Пока используем заглушку
        }
        
        /// <summary>
        /// Расчет сил трения
        /// </summary>
        private static void CalculateFrictionForcesStatic(ref WheelComponent wheel, float deltaTime)
        {
            if (!wheel.isGrounded)
            {
                wheel.groundReactionForce = float3.zero;
                wheel.frictionForce = float3.zero;
                return;
            }
            
            // Сила реакции поверхности
            float normalForce = wheel.mass * 9.81f; // Простое приближение
            wheel.groundReactionForce = wheel.groundNormal * normalForce;
            
            // Расчет силы трения
            float frictionMagnitude = normalForce * wheel.frictionCoefficient;
            
            // Направление силы трения (противоположно скорости скольжения)
            if (math.length(wheel.slipVelocity) > 0.001f)
            {
                float3 frictionDirection = math.normalize(wheel.slipVelocity);
                wheel.frictionForce = -frictionDirection * frictionMagnitude;
            }
            else
            {
                wheel.frictionForce = float3.zero;
            }
        }
        
        /// <summary>
        /// Обновление скольжения
        /// </summary>
        private static void UpdateSlipStatic(ref WheelComponent wheel, float deltaTime)
        {
            if (!wheel.isGrounded)
            {
                wheel.slipVelocity = float3.zero;
                wheel.slipRatio = 0f;
                wheel.slipAngle = 0f;
                return;
            }
            
            // Расчет скорости скольжения
            float3 wheelVelocity = new float3(0, 0, wheel.angularVelocity * wheel.radius);
            wheel.slipVelocity = wheelVelocity;
            
            // Расчет коэффициента скольжения
            if (math.length(wheelVelocity) > 0.001f)
            {
                wheel.slipRatio = math.length(wheel.slipVelocity) / math.length(wheelVelocity);
            }
            else
            {
                wheel.slipRatio = 0f;
            }
            
            // Расчет угла скольжения
            if (math.length(wheel.slipVelocity) > 0.001f)
            {
                wheel.slipAngle = math.atan2(wheel.slipVelocity.x, wheel.slipVelocity.z);
            }
            else
            {
                wheel.slipAngle = 0f;
            }
        }
    }
}
