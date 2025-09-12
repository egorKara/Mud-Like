using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using Unity.Burst;
using Unity.Collections;
using MudLike.Vehicles.Components;
using MudLike.Terrain.Components;

namespace MudLike.Vehicles.Systems
{
    /// <summary>
    /// Система реалистичной физики колес
    /// Реализует подвеску, торможение, рулевое управление и взаимодействие с поверхностью
    /// </summary>
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile]
    public partial class RealisticWheelPhysicsSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            float deltaTime = SystemAPI.Time.fixedDeltaTime;
            
            Entities
                .WithAll<WheelData, LocalTransform>()
                .ForEach((ref WheelData wheel, ref LocalTransform transform, 
                         in VehiclePhysics vehiclePhysics, in VehicleConfig config) =>
                {
                    ProcessWheelPhysics(ref wheel, ref transform, vehiclePhysics, config, deltaTime);
                }).Schedule();
        }
        
        /// <summary>
        /// Обрабатывает физику колеса
        /// </summary>
        [BurstCompile]
        private static void ProcessWheelPhysics(ref WheelData wheel, ref LocalTransform transform,
                                              in VehiclePhysics vehiclePhysics, in VehicleConfig config, float deltaTime)
        {
            // 1. Обновляем подвеску
            UpdateSuspension(ref wheel, transform, deltaTime);
            
            // 2. Вычисляем контакт с поверхностью
            CalculateGroundContact(ref wheel, transform);
            
            // 3. Применяем силы трения
            ApplyFrictionForces(ref wheel, vehiclePhysics, deltaTime);
            
            // 4. Обновляем рулевое управление
            UpdateSteering(ref wheel, config, deltaTime);
            
            // 5. Применяем торможение
            ApplyBraking(ref wheel, config, deltaTime);
            
            // 6. Вычисляем тяговую силу
            CalculateTractionForce(ref wheel, vehiclePhysics, deltaTime);
        }
        
        /// <summary>
        /// Обновляет подвеску колеса
        /// </summary>
        [BurstCompile]
        private static void UpdateSuspension(ref WheelData wheel, LocalTransform transform, float deltaTime)
        {
            // Вычисляем сжатие подвески
            float suspensionLength = math.length(wheel.SuspensionTarget - transform.Position);
            float compression = math.max(0f, wheel.SuspensionRestLength - suspensionLength);
            compression = math.min(compression, wheel.SuspensionMaxCompression);
            
            // Вычисляем скорость сжатия
            float compressionVelocity = (compression - wheel.LastCompression) / deltaTime;
            wheel.LastCompression = compression;
            
            // Применяем силу пружины (Закон Гука)
            float springForce = compression * wheel.SuspensionSpring;
            
            // Применяем демпфирование
            float dampingForce = compressionVelocity * wheel.SuspensionDamping;
            
            // Общая сила подвески
            wheel.SuspensionForce = springForce - dampingForce;
            
            // Ограничиваем максимальную силу
            wheel.SuspensionForce = math.clamp(wheel.SuspensionForce, 0f, wheel.SuspensionMaxForce);
        }
        
        /// <summary>
        /// Вычисляет контакт с поверхностью
        /// </summary>
        [BurstCompile]
        private static void CalculateGroundContact(ref WheelData wheel, LocalTransform transform)
        {
            // Raycast вниз для определения поверхности
            float3 rayStart = transform.Position;
            float3 rayDirection = -transform.Up();
            
            // Здесь должен быть raycast к террейну
            // Для простоты используем фиксированную высоту
            float groundHeight = 0f; // Получается из TerrainHeightManager
            float wheelBottom = transform.Position.y - wheel.Radius;
            
            wheel.IsGrounded = wheelBottom <= groundHeight + 0.1f;
            
            if (wheel.IsGrounded)
            {
                wheel.GroundNormal = new float3(0, 1, 0); // Получается из нормалей террейна
                wheel.GroundDistance = groundHeight - wheelBottom;
                
                // Вычисляем смещение колеса относительно поверхности
                wheel.SurfaceOffset = wheel.GroundDistance;
            }
            else
            {
                wheel.GroundNormal = new float3(0, 1, 0);
                wheel.GroundDistance = 0f;
                wheel.SurfaceOffset = 0f;
            }
        }
        
        /// <summary>
        /// Применяет силы трения
        /// </summary>
        [BurstCompile]
        private static void ApplyFrictionForces(ref WheelData wheel, in VehiclePhysics vehiclePhysics, float deltaTime)
        {
            if (!wheel.IsGrounded) return;
            
            // Вычисляем скольжение колеса
            float3 wheelVelocity = CalculateWheelVelocity(wheel, vehiclePhysics);
            float3 groundVelocity = CalculateGroundVelocity(wheel);
            float3 slipVelocity = wheelVelocity - groundVelocity;
            
            // Разделяем на продольное и боковое скольжение
            float3 forward = math.normalize(wheel.ForwardDirection);
            float3 right = math.normalize(wheel.RightDirection);
            
            float longitudinalSlip = math.dot(slipVelocity, forward);
            float lateralSlip = math.dot(slipVelocity, right);
            
            // Вычисляем силы трения на основе скольжения
            float normalForce = math.max(0.1f, wheel.SuspensionForce);
            
            // Продольная сила трения (тяга/торможение)
            float longitudinalFriction = CalculateFrictionForce(longitudinalSlip, normalForce, wheel.LongitudinalFriction);
            
            // Боковая сила трения (поворот)
            float lateralFriction = CalculateFrictionForce(lateralSlip, normalForce, wheel.LateralFriction);
            
            // Применяем силы
            wheel.LongitudinalForce = longitudinalFriction * forward;
            wheel.LateralForce = lateralFriction * right;
        }
        
        /// <summary>
        /// Обновляет рулевое управление
        /// </summary>
        [BurstCompile]
        private static void UpdateSteering(ref WheelData wheel, in VehicleConfig config, float deltaTime)
        {
            if (!wheel.CanSteer) return;
            
            // Применяем рулевое управление
            float targetSteerAngle = wheel.SteerInput * config.MaxSteerAngle;
            
            // Плавное изменение угла поворота
            float steerSpeed = config.SteerSpeed * deltaTime;
            wheel.SteerAngle = math.lerp(wheel.SteerAngle, targetSteerAngle, steerSpeed);
            
            // Ограничиваем максимальный угол
            wheel.SteerAngle = math.clamp(wheel.SteerAngle, -config.MaxSteerAngle, config.MaxSteerAngle);
            
            // Обновляем направление колеса
            UpdateWheelDirection(ref wheel);
        }
        
        /// <summary>
        /// Применяет торможение
        /// </summary>
        [BurstCompile]
        private static void ApplyBraking(ref WheelData wheel, in VehicleConfig config, float deltaTime)
        {
            if (!wheel.IsGrounded) return;
            
            // Вычисляем тормозную силу
            float brakeForce = wheel.BrakeInput * config.BrakeForce;
            
            // Применяем ABS (антиблокировочная система)
            if (wheel.ABSEnabled)
            {
                float wheelSpeed = math.length(CalculateWheelVelocity(wheel, default));
                if (wheelSpeed < 0.1f && brakeForce > 0f)
                {
                    brakeForce *= 0.1f; // Снижаем тормозную силу при блокировке
                }
            }
            
            wheel.BrakeForce = brakeForce;
        }
        
        /// <summary>
        /// Вычисляет тяговую силу
        /// </summary>
        [BurstCompile]
        private static void CalculateTractionForce(ref WheelData wheel, in VehiclePhysics vehiclePhysics, float deltaTime)
        {
            if (!wheel.IsGrounded) return;
            
            // Вычисляем тяговую силу на основе двигателя
            float engineTorque = wheel.EngineTorque;
            float wheelRadius = wheel.Radius;
            
            // Тяговая сила = крутящий момент / радиус колеса
            float tractionForce = engineTorque / wheelRadius;
            
            // Ограничиваем максимальную тяговую силу
            float maxTraction = wheel.SuspensionForce * wheel.LongitudinalFriction;
            tractionForce = math.clamp(tractionForce, -maxTraction, maxTraction);
            
            wheel.TractionForce = tractionForce;
        }
        
        /// <summary>
        /// Вычисляет скорость колеса
        /// </summary>
        [BurstCompile]
        private static float3 CalculateWheelVelocity(WheelData wheel, in VehiclePhysics vehiclePhysics)
        {
            // Линейная скорость колеса
            float3 linearVelocity = vehiclePhysics.Velocity;
            
            // Угловая скорость колеса
            float angularVelocity = wheel.AngularVelocity;
            float3 wheelForward = math.normalize(wheel.ForwardDirection);
            
            // Скорость точки контакта колеса с поверхностью
            float3 contactVelocity = linearVelocity + math.cross(new float3(0, angularVelocity * wheel.Radius, 0), wheelForward);
            
            return contactVelocity;
        }
        
        /// <summary>
        /// Вычисляет скорость поверхности
        /// </summary>
        [BurstCompile]
        private static float3 CalculateGroundVelocity(WheelData wheel)
        {
            // В реальной реализации здесь должна быть скорость движения террейна/грязи
            // Пока возвращаем ноль (неподвижная поверхность)
            return float3.zero;
        }
        
        /// <summary>
        /// Вычисляет силу трения
        /// </summary>
        [BurstCompile]
        private static float CalculateFrictionForce(float slip, float normalForce, float frictionCoefficient)
        {
            // Простая модель трения
            float frictionForce = -slip * frictionCoefficient * normalForce;
            
            // Ограничиваем максимальную силу трения
            float maxFriction = normalForce * frictionCoefficient;
            frictionForce = math.clamp(frictionForce, -maxFriction, maxFriction);
            
            return frictionForce;
        }
        
        /// <summary>
        /// Обновляет направление колеса
        /// </summary>
        [BurstCompile]
        private static void UpdateWheelDirection(ref WheelData wheel)
        {
            // Поворачиваем направление колеса на угол рулевого управления
            float steerAngleRad = math.radians(wheel.SteerAngle);
            quaternion steerRotation = quaternion.RotateY(steerAngleRad);
            
            wheel.ForwardDirection = math.mul(steerRotation, wheel.ForwardDirection);
            wheel.RightDirection = math.cross(wheel.ForwardDirection, new float3(0, 1, 0));
        }
    }
}
