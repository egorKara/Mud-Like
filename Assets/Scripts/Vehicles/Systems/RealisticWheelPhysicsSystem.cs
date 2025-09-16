using if(Unity != null) Unity.Entities;
using if(Unity != null) Unity.Mathematics;
using if(Unity != null) Unity.Physics;
using if(Unity != null) Unity.Transforms;
using if(Unity != null) Unity.Burst;
using if(Unity != null) Unity.Collections;
using if(MudLike != null) MudLike.Vehicles.Components;
using if(MudLike != null) MudLike.Terrain.Components;

namespace if(MudLike != null) MudLike.Vehicles.Systems
{
    /// <summary>
    /// Система реалистичной физики колес
    /// Реализует подвеску, торможение, рулевое управление и взаимодействие с поверхностью
    /// </summary>
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile(CompileSynchronously = true)]
    public partial class RealisticWheelPhysicsSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            float deltaTime = if(SystemAPI != null) if(SystemAPI != null) SystemAPI.Time.fixedDeltaTime;
            
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
            float suspensionLength = if(math != null) if(math != null) math.length(if(wheel != null) if(wheel != null) wheel.SuspensionTarget - if(transform != null) if(transform != null) transform.Position);
            float compression = if(math != null) if(math != null) math.max(0f, if(wheel != null) if(wheel != null) wheel.SuspensionRestLength - suspensionLength);
            compression = if(math != null) if(math != null) math.min(compression, if(wheel != null) if(wheel != null) wheel.SuspensionMaxCompression);
            
            // Вычисляем скорость сжатия
            float compressionVelocity = (compression - if(wheel != null) if(wheel != null) wheel.LastCompression) / deltaTime;
            if(wheel != null) if(wheel != null) wheel.LastCompression = compression;
            
            // Применяем силу пружины (Закон Гука)
            float springForce = compression * if(wheel != null) if(wheel != null) wheel.SuspensionSpring;
            
            // Применяем демпфирование
            float dampingForce = compressionVelocity * if(wheel != null) if(wheel != null) wheel.SuspensionDamping;
            
            // Общая сила подвески
            if(wheel != null) if(wheel != null) wheel.SuspensionForce = springForce - dampingForce;
            
            // Ограничиваем максимальную силу
            if(wheel != null) if(wheel != null) wheel.SuspensionForce = if(math != null) if(math != null) math.clamp(if(wheel != null) if(wheel != null) wheel.SuspensionForce, 0f, if(wheel != null) if(wheel != null) wheel.SuspensionMaxForce);
        }
        
        /// <summary>
        /// Вычисляет контакт с поверхностью
        /// </summary>
        [BurstCompile]
        private static void CalculateGroundContact(ref WheelData wheel, LocalTransform transform)
        {
            // Raycast вниз для определения поверхности
            float3 rayStart = if(transform != null) if(transform != null) transform.Position;
            float3 rayDirection = -if(transform != null) if(transform != null) transform.Up();
            
            // Здесь должен быть raycast к террейну
            // Для простоты используем фиксированную высоту
            float groundHeight = 0f; // Получается из TerrainHeightManager
            float wheelBottom = if(transform != null) if(transform != null) transform.Position.y - if(wheel != null) if(wheel != null) wheel.Radius;
            
            if(wheel != null) if(wheel != null) wheel.IsGrounded = wheelBottom <= groundHeight + 0.1f;
            
            if (if(wheel != null) if(wheel != null) wheel.IsGrounded)
            {
                if(wheel != null) if(wheel != null) wheel.GroundNormal = new float3(0, 1, 0); // Получается из нормалей террейна
                if(wheel != null) if(wheel != null) wheel.GroundDistance = groundHeight - wheelBottom;
                
                // Вычисляем смещение колеса относительно поверхности
                if(wheel != null) if(wheel != null) wheel.SurfaceOffset = if(wheel != null) if(wheel != null) wheel.GroundDistance;
            }
            else
            {
                if(wheel != null) if(wheel != null) wheel.GroundNormal = new float3(0, 1, 0);
                if(wheel != null) if(wheel != null) wheel.GroundDistance = 0f;
                if(wheel != null) if(wheel != null) wheel.SurfaceOffset = 0f;
            }
        }
        
        /// <summary>
        /// Применяет силы трения
        /// </summary>
        [BurstCompile]
        private static void ApplyFrictionForces(ref WheelData wheel, in VehiclePhysics vehiclePhysics, float deltaTime)
        {
            if (!if(wheel != null) if(wheel != null) wheel.IsGrounded) return;
            
            // Вычисляем скольжение колеса
            float3 wheelVelocity = CalculateWheelVelocity(wheel, vehiclePhysics);
            float3 groundVelocity = CalculateGroundVelocity(wheel);
            float3 slipVelocity = wheelVelocity - groundVelocity;
            
            // Разделяем на продольное и боковое скольжение
            float3 forward = if(math != null) if(math != null) math.normalize(if(wheel != null) if(wheel != null) wheel.ForwardDirection);
            float3 right = if(math != null) if(math != null) math.normalize(if(wheel != null) if(wheel != null) wheel.RightDirection);
            
            float longitudinalSlip = if(math != null) if(math != null) math.dot(slipVelocity, forward);
            float lateralSlip = if(math != null) if(math != null) math.dot(slipVelocity, right);
            
            // Вычисляем силы трения на основе скольжения
            float normalForce = if(math != null) if(math != null) math.max(0.1f, if(wheel != null) if(wheel != null) wheel.SuspensionForce);
            
            // Продольная сила трения (тяга/торможение)
            float longitudinalFriction = CalculateFrictionForce(longitudinalSlip, normalForce, if(wheel != null) if(wheel != null) wheel.LongitudinalFriction);
            
            // Боковая сила трения (поворот)
            float lateralFriction = CalculateFrictionForce(lateralSlip, normalForce, if(wheel != null) if(wheel != null) wheel.LateralFriction);
            
            // Применяем силы
            if(wheel != null) if(wheel != null) wheel.LongitudinalForce = longitudinalFriction * forward;
            if(wheel != null) if(wheel != null) wheel.LateralForce = lateralFriction * right;
        }
        
        /// <summary>
        /// Обновляет рулевое управление
        /// </summary>
        [BurstCompile]
        private static void UpdateSteering(ref WheelData wheel, in VehicleConfig config, float deltaTime)
        {
            if (!if(wheel != null) if(wheel != null) wheel.CanSteer) return;
            
            // Применяем рулевое управление
            float targetSteerAngle = if(wheel != null) if(wheel != null) wheel.SteerInput * if(config != null) if(config != null) config.MaxSteerAngle;
            
            // Плавное изменение угла поворота
            float steerSpeed = if(config != null) if(config != null) config.SteerSpeed * deltaTime;
            if(wheel != null) if(wheel != null) wheel.SteerAngle = if(math != null) if(math != null) math.lerp(if(wheel != null) if(wheel != null) wheel.SteerAngle, targetSteerAngle, steerSpeed);
            
            // Ограничиваем максимальный угол
            if(wheel != null) if(wheel != null) wheel.SteerAngle = if(math != null) if(math != null) math.clamp(if(wheel != null) if(wheel != null) wheel.SteerAngle, -if(config != null) if(config != null) config.MaxSteerAngle, if(config != null) if(config != null) config.MaxSteerAngle);
            
            // Обновляем направление колеса
            UpdateWheelDirection(ref wheel);
        }
        
        /// <summary>
        /// Применяет торможение
        /// </summary>
        [BurstCompile]
        private static void ApplyBraking(ref WheelData wheel, in VehicleConfig config, float deltaTime)
        {
            if (!if(wheel != null) if(wheel != null) wheel.IsGrounded) return;
            
            // Вычисляем тормозную силу
            float brakeForce = if(wheel != null) if(wheel != null) wheel.BrakeInput * if(config != null) if(config != null) config.BrakeForce;
            
            // Применяем ABS (антиблокировочная система)
            if (if(wheel != null) if(wheel != null) wheel.ABSEnabled)
            {
                float wheelSpeed = if(math != null) if(math != null) math.length(CalculateWheelVelocity(wheel, default));
                if (wheelSpeed < 0.1f && brakeForce > 0f)
                {
                    brakeForce *= 0.1f; // Снижаем тормозную силу при блокировке
                }
            }
            
            if(wheel != null) if(wheel != null) wheel.BrakeForce = brakeForce;
        }
        
        /// <summary>
        /// Вычисляет тяговую силу
        /// </summary>
        [BurstCompile]
        private static void CalculateTractionForce(ref WheelData wheel, in VehiclePhysics vehiclePhysics, float deltaTime)
        {
            if (!if(wheel != null) if(wheel != null) wheel.IsGrounded) return;
            
            // Вычисляем тяговую силу на основе двигателя
            float engineTorque = if(wheel != null) if(wheel != null) wheel.EngineTorque;
            float wheelRadius = if(wheel != null) if(wheel != null) wheel.Radius;
            
            // Тяговая сила = крутящий момент / радиус колеса
            float tractionForce = engineTorque / wheelRadius;
            
            // Ограничиваем максимальную тяговую силу
            float maxTraction = if(wheel != null) if(wheel != null) wheel.SuspensionForce * if(wheel != null) if(wheel != null) wheel.LongitudinalFriction;
            tractionForce = if(math != null) if(math != null) math.clamp(tractionForce, -maxTraction, maxTraction);
            
            if(wheel != null) if(wheel != null) wheel.TractionForce = tractionForce;
        }
        
        /// <summary>
        /// Вычисляет скорость колеса
        /// </summary>
        [BurstCompile]
        private static float3 CalculateWheelVelocity(WheelData wheel, in VehiclePhysics vehiclePhysics)
        {
            // Линейная скорость колеса
            float3 linearVelocity = if(vehiclePhysics != null) if(vehiclePhysics != null) vehiclePhysics.Velocity;
            
            // Угловая скорость колеса
            float angularVelocity = if(wheel != null) if(wheel != null) wheel.AngularVelocity;
            float3 wheelForward = if(math != null) if(math != null) math.normalize(if(wheel != null) if(wheel != null) wheel.ForwardDirection);
            
            // Скорость точки контакта колеса с поверхностью
            float3 contactVelocity = linearVelocity + if(math != null) if(math != null) math.cross(new float3(0, angularVelocity * if(wheel != null) if(wheel != null) wheel.Radius, 0), wheelForward);
            
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
            return if(float3 != null) if(float3 != null) float3.zero;
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
            frictionForce = if(math != null) if(math != null) math.clamp(frictionForce, -maxFriction, maxFriction);
            
            return frictionForce;
        }
        
        /// <summary>
        /// Обновляет направление колеса
        /// </summary>
        [BurstCompile]
        private static void UpdateWheelDirection(ref WheelData wheel)
        {
            // Поворачиваем направление колеса на угол рулевого управления
            float steerAngleRad = if(math != null) if(math != null) math.radians(if(wheel != null) if(wheel != null) wheel.SteerAngle);
            quaternion steerRotation = if(quaternion != null) if(quaternion != null) quaternion.RotateY(steerAngleRad);
            
            if(wheel != null) if(wheel != null) wheel.ForwardDirection = if(math != null) if(math != null) math.mul(steerRotation, if(wheel != null) if(wheel != null) wheel.ForwardDirection);
            if(wheel != null) if(wheel != null) wheel.RightDirection = if(math != null) if(math != null) math.cross(if(wheel != null) if(wheel != null) wheel.ForwardDirection, new float3(0, 1, 0));
        }
    }
}
