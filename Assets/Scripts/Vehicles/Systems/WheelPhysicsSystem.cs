using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using MudLike.Vehicles.Components;

namespace MudLike.Vehicles.Systems
{
    /// <summary>
    /// Система физики колес грузовика
    /// </summary>
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateAfter(typeof(TruckMovementSystem))]
    public partial class WheelPhysicsSystem : SystemBase
    {
        /// <summary>
        /// Обрабатывает физику всех колес
        /// </summary>
        protected override void OnUpdate()
        {
            float deltaTime = SystemAPI.Time.fixedDeltaTime;
            
            Entities
                .WithAll<WheelData>()
                .ForEach((ref WheelData wheel, ref SuspensionData suspension, in LocalTransform transform) =>
                {
                    ProcessWheelPhysics(ref wheel, ref suspension, transform, deltaTime);
                }).Schedule();
        }
        
        /// <summary>
        /// Обрабатывает физику конкретного колеса
        /// </summary>
        private static void ProcessWheelPhysics(ref WheelData wheel, ref SuspensionData suspension, in LocalTransform transform, float deltaTime)
        {
            // Обновляем подвеску
            UpdateSuspension(ref wheel, ref suspension, transform, deltaTime);
            
            // Вычисляем сцепление с поверхностью
            CalculateTraction(ref wheel, suspension);
            
            // Обновляем угловую скорость колеса
            UpdateWheelRotation(ref wheel, deltaTime);
            
            // Вычисляем силу сцепления
            CalculateTractionForce(ref wheel, suspension);
        }
        
        /// <summary>
        /// Обновляет подвеску колеса
        /// </summary>
        private static void UpdateSuspension(ref WheelData wheel, ref SuspensionData suspension, in LocalTransform transform, float deltaTime)
        {
            // Вычисляем целевую длину подвески
            float3 wheelWorldPos = transform.Position + math.mul(transform.Rotation, wheel.LocalPosition);
            float3 targetPos = wheelWorldPos - new float3(0, suspension.Length, 0);
            
            // Обновляем позицию подвески
            suspension.TargetPosition = targetPos;
            suspension.CurrentLength = math.distance(wheelWorldPos, targetPos);
            
            // Вычисляем скорость сжатия
            float compressionVelocity = (suspension.Length - suspension.CurrentLength) / deltaTime;
            suspension.CompressionVelocity = compressionVelocity;
            
            // Вычисляем силу пружины
            float springForce = suspension.SpringForce * (suspension.Length - suspension.CurrentLength);
            float dampingForce = suspension.DampingForce * compressionVelocity;
            
            suspension.SuspensionForce = new float3(0, springForce - dampingForce, 0);
        }
        
        /// <summary>
        /// Вычисляет сцепление с поверхностью
        /// </summary>
        private static void CalculateTraction(ref WheelData wheel, in SuspensionData suspension)
        {
            // Базовый коэффициент сцепления
            float baseTraction = 0.8f;
            
            // Влияние погружения в грязь
            float mudFactor = math.max(0.1f, 1f - wheel.SinkDepth * 0.5f);
            
            // Влияние скорости скольжения
            float slipFactor = math.max(0.1f, 1f - wheel.SlipRatio * 0.3f);
            
            wheel.TractionCoefficient = baseTraction * mudFactor * slipFactor;
        }
        
        /// <summary>
        /// Обновляет вращение колеса
        /// </summary>
        private static void UpdateWheelRotation(ref WheelData wheel, float deltaTime)
        {
            // Вычисляем угловую скорость на основе крутящего момента
            float angularAcceleration = wheel.Torque / (wheel.Radius * wheel.Radius * 0.5f);
            wheel.AngularVelocity += angularAcceleration * deltaTime;
            
            // Применяем торможение
            if (wheel.BrakeTorque > 0f)
            {
                float brakeAcceleration = -wheel.BrakeTorque / (wheel.Radius * wheel.Radius * 0.5f);
                wheel.AngularVelocity += brakeAcceleration * deltaTime;
            }
            
            // Сопротивление качению
            wheel.AngularVelocity *= 0.99f;
            
            // Ограничиваем максимальную угловую скорость
            float maxAngularVelocity = 50f; // рад/с
            wheel.AngularVelocity = math.clamp(wheel.AngularVelocity, -maxAngularVelocity, maxAngularVelocity);
        }
        
        /// <summary>
        /// Вычисляет силу сцепления колеса
        /// </summary>
        private static void CalculateTractionForce(ref WheelData wheel, in SuspensionData suspension)
        {
            // Вычисляем скорость скольжения
            float linearVelocity = wheel.AngularVelocity * wheel.Radius;
            wheel.SlipRatio = math.abs(linearVelocity - wheel.AngularVelocity * wheel.Radius) / math.max(0.1f, math.abs(linearVelocity));
            
            // Вычисляем силу сцепления
            float normalForce = math.length(suspension.SuspensionForce);
            float tractionForce = normalForce * wheel.TractionCoefficient;
            
            // Направление силы сцепления
            float3 forward = new float3(0, 0, 1); // Упрощенно
            wheel.TractionForce = forward * tractionForce;
        }
    }
}