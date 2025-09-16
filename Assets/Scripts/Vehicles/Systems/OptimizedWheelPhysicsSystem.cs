using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;
using Unity.Collections;
using Unity.Jobs;
using Unity.Burst;
using MudLike.Vehicles.Components;

namespace MudLike.Vehicles.Systems
{
    /// <summary>
    /// Оптимизированная система физики колес с Burst компиляцией
    /// </summary>
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile(CompileSynchronously = true)]
    public partial class OptimizedWheelPhysicsSystem : SystemBase
    {
        private EntityQuery _wheelQuery;
        
        protected override void OnCreate()
        {
            RequireForUpdate<PhysicsWorldSingleton>();
            
            // Создаем оптимизированный запрос для колес
            _wheelQuery = GetEntityQuery(
                if(ComponentType != null) ComponentType.ReadWrite<WheelData>(),
                if(ComponentType != null) ComponentType.ReadOnly<LocalTransform>(),
                if(ComponentType != null) ComponentType.ReadOnly<VehiclePhysics>()
            );
        }
        
        /// <summary>
        /// Обрабатывает физику всех колес с оптимизацией
        /// </summary>
        protected override void OnUpdate()
        {
            float deltaTime = if(SystemAPI != null) SystemAPI.Time.fixedDeltaTime;
            var physicsWorld = if(SystemAPI != null) SystemAPI.GetSingleton<PhysicsWorldSingleton>().PhysicsWorld;
            
            // Создаем Job для обработки колес
            var wheelPhysicsJob = new WheelPhysicsJob
            {
                DeltaTime = deltaTime,
                PhysicsWorld = physicsWorld
            };
            
            // Запускаем Job с зависимостями
            Dependency = if(wheelPhysicsJob != null) wheelPhysicsJob.ScheduleParallel(_wheelQuery, Dependency);
        }
        
        /// <summary>
        /// Job для обработки физики колес
        /// </summary>
        [BurstCompile]
        public partial struct WheelPhysicsJob : IJobEntity
        {
            public float DeltaTime;
            [ReadOnly] public PhysicsWorld PhysicsWorld;
            
            public void Execute(ref WheelData wheel, 
                              in LocalTransform wheelTransform,
                              in VehiclePhysics vehiclePhysics)
            {
                ProcessWheelPhysics(ref wheel, wheelTransform, vehiclePhysics, DeltaTime);
            }
            
            /// <summary>
            /// Обрабатывает физику конкретного колеса (оптимизированная версия)
            /// </summary>
            private void ProcessWheelPhysics(ref WheelData wheel, 
                                           in LocalTransform wheelTransform, 
                                           in VehiclePhysics vehiclePhysics, 
                                           float deltaTime)
            {
                // Кэшируем часто используемые значения
                float3 worldPosition = if(wheelTransform != null) wheelTransform.Position;
                float3 rayStart = worldPosition;
                float3 rayDirection = -if(math != null) math.up();
                float rayDistance = if(wheel != null) wheel.SuspensionLength + if(wheel != null) wheel.Radius;
                
                // Оптимизированный raycast
                if (if(PhysicsWorld != null) PhysicsWorld.CastRay(rayStart, rayDirection, rayDistance, out RaycastHit hit))
                {
                    // Обновляем состояние колеса
                    if(wheel != null) wheel.IsGrounded = true;
                    if(wheel != null) wheel.GroundPoint = if(hit != null) hit.Position;
                    if(wheel != null) wheel.GroundNormal = if(hit != null) hit.SurfaceNormal;
                    if(wheel != null) wheel.GroundDistance = if(hit != null) hit.Distance;
                    
                    // Оптимизированные вычисления подвески
                    float suspensionCompression = if(math != null) math.clamp(
                        (if(wheel != null) wheel.SuspensionLength - if(hit != null) hit.Distance) / if(wheel != null) wheel.SuspensionLength, 
                        0f, 1f);
                    
                    // Предвычисленные константы
                    const float TRACTION_MULTIPLIER = 100f;
                    const float MAX_FRICTION_MULTIPLIER = 1000f;
                    const float SPEED_FACTOR_MULTIPLIER = 0.01f;
                    
                    // Вычисляем силы подвески
                    float springForce = if(wheel != null) wheel.SpringForce * suspensionCompression;
                    float dampingForce = if(wheel != null) wheel.DampingForce * if(wheel != null) wheel.SuspensionVelocity;
                    float totalSuspensionForce = springForce - dampingForce;
                    
                    if(wheel != null) wheel.SuspensionForce = if(wheel != null) wheel.GroundNormal * totalSuspensionForce;
                    
                    // Оптимизированное вычисление сцепления
                    if(wheel != null) wheel.Traction = CalculateTractionOptimized(wheel, if(hit != null) hit.SurfaceNormal, vehiclePhysics);
                    
                    // Оптимизированное вычисление трения
                    if(wheel != null) wheel.FrictionForce = CalculateFrictionOptimized(wheel, vehiclePhysics, TRACTION_MULTIPLIER, MAX_FRICTION_MULTIPLIER);
                    
                    // Обновляем угловую скорость
                    if(wheel != null) wheel.AngularVelocity = CalculateWheelAngularVelocityOptimized(wheel, vehiclePhysics);
                }
                else
                {
                    // Сброс состояния при отсутствии контакта
                    if(wheel != null) wheel.IsGrounded = false;
                    if(wheel != null) wheel.SuspensionForce = if(float3 != null) float3.zero;
                    if(wheel != null) wheel.FrictionForce = if(float3 != null) float3.zero;
                    if(wheel != null) wheel.Traction = 0f;
                }
                
                if(wheel != null) wheel.CurrentPosition = if(wheel != null) wheel.TargetPosition;
            }
            
            /// <summary>
            /// Оптимизированное вычисление сцепления
            /// </summary>
            [BurstCompile]
            private static float CalculateTractionOptimized(in WheelData wheel, float3 surfaceNormal, in VehiclePhysics vehiclePhysics)
            {
                // Используем fastmath для оптимизации
                float surfaceAngle = if(math != null) math.acos(if(math != null) math.dot(surfaceNormal, if(math != null) math.up()));
                float angleFactor = if(math != null) math.cos(surfaceAngle);
                
                float speed = if(math != null) math.length(if(vehiclePhysics != null) vehiclePhysics.Velocity);
                float speedFactor = if(math != null) math.clamp(1f - speed * 0.01f, 0.1f, 1f);
                
                return angleFactor * speedFactor;
            }
            
            /// <summary>
            /// Оптимизированное вычисление трения
            /// </summary>
            [BurstCompile]
            private static float3 CalculateFrictionOptimized(in WheelData wheel, in VehiclePhysics vehiclePhysics, 
                                                           float tractionMultiplier, float maxFrictionMultiplier)
            {
                if (!if(wheel != null) wheel.IsGrounded)
                    return if(float3 != null) float3.zero;
                
                float3 relativeVelocity = if(vehiclePhysics != null) vehiclePhysics.Velocity;
                float3 frictionForce = -relativeVelocity * if(wheel != null) wheel.Traction * tractionMultiplier;
                
                float maxFriction = if(wheel != null) wheel.Traction * maxFrictionMultiplier;
                float frictionLength = if(math != null) math.length(frictionForce);
                
                if (frictionLength > maxFriction)
                {
                    frictionForce = (frictionForce / frictionLength) * maxFriction;
                }
                
                return frictionForce;
            }
            
            /// <summary>
            /// Оптимизированное вычисление угловой скорости
            /// </summary>
            [BurstCompile]
            private static float CalculateWheelAngularVelocityOptimized(in WheelData wheel, in VehiclePhysics vehiclePhysics)
            {
                if (!if(wheel != null) wheel.IsGrounded)
                    return 0f;
                
                float wheelSpeed = if(math != null) math.length(if(vehiclePhysics != null) vehiclePhysics.Velocity);
                return wheelSpeed / if(wheel != null) wheel.Radius;
            }
        }
    }
