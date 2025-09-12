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
    [BurstCompile]
    public partial class OptimizedWheelPhysicsSystem : SystemBase
    {
        private EntityQuery _wheelQuery;
        
        protected override void OnCreate()
        {
            RequireForUpdate<PhysicsWorldSingleton>();
            
            // Создаем оптимизированный запрос для колес
            _wheelQuery = GetEntityQuery(
                ComponentType.ReadWrite<WheelData>(),
                ComponentType.ReadOnly<LocalTransform>(),
                ComponentType.ReadOnly<VehiclePhysics>()
            );
        }
        
        /// <summary>
        /// Обрабатывает физику всех колес с оптимизацией
        /// </summary>
        protected override void OnUpdate()
        {
            float deltaTime = SystemAPI.Time.fixedDeltaTime;
            var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().PhysicsWorld;
            
            // Создаем Job для обработки колес
            var wheelPhysicsJob = new WheelPhysicsJob
            {
                DeltaTime = deltaTime,
                PhysicsWorld = physicsWorld
            };
            
            // Запускаем Job с зависимостями
            Dependency = wheelPhysicsJob.ScheduleParallel(_wheelQuery, Dependency);
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
                float3 worldPosition = wheelTransform.Position;
                float3 rayStart = worldPosition;
                float3 rayDirection = -math.up();
                float rayDistance = wheel.SuspensionLength + wheel.Radius;
                
                // Оптимизированный raycast
                if (PhysicsWorld.CastRay(rayStart, rayDirection, rayDistance, out RaycastHit hit))
                {
                    // Обновляем состояние колеса
                    wheel.IsGrounded = true;
                    wheel.GroundPoint = hit.Position;
                    wheel.GroundNormal = hit.SurfaceNormal;
                    wheel.GroundDistance = hit.Distance;
                    
                    // Оптимизированные вычисления подвески
                    float suspensionCompression = math.clamp(
                        (wheel.SuspensionLength - hit.Distance) / wheel.SuspensionLength, 
                        0f, 1f);
                    
                    // Предвычисленные константы
                    const float TRACTION_MULTIPLIER = 100f;
                    const float MAX_FRICTION_MULTIPLIER = 1000f;
                    const float SPEED_FACTOR_MULTIPLIER = 0.01f;
                    
                    // Вычисляем силы подвески
                    float springForce = wheel.SpringForce * suspensionCompression;
                    float dampingForce = wheel.DampingForce * wheel.SuspensionVelocity;
                    float totalSuspensionForce = springForce - dampingForce;
                    
                    wheel.SuspensionForce = wheel.GroundNormal * totalSuspensionForce;
                    
                    // Оптимизированное вычисление сцепления
                    wheel.Traction = CalculateTractionOptimized(wheel, hit.SurfaceNormal, vehiclePhysics);
                    
                    // Оптимизированное вычисление трения
                    wheel.FrictionForce = CalculateFrictionOptimized(wheel, vehiclePhysics, TRACTION_MULTIPLIER, MAX_FRICTION_MULTIPLIER);
                    
                    // Обновляем угловую скорость
                    wheel.AngularVelocity = CalculateWheelAngularVelocityOptimized(wheel, vehiclePhysics);
                }
                else
                {
                    // Сброс состояния при отсутствии контакта
                    wheel.IsGrounded = false;
                    wheel.SuspensionForce = float3.zero;
                    wheel.FrictionForce = float3.zero;
                    wheel.Traction = 0f;
                }
                
                wheel.CurrentPosition = wheel.TargetPosition;
            }
            
            /// <summary>
            /// Оптимизированное вычисление сцепления
            /// </summary>
            [BurstCompile]
            private static float CalculateTractionOptimized(in WheelData wheel, float3 surfaceNormal, in VehiclePhysics vehiclePhysics)
            {
                // Используем fastmath для оптимизации
                float surfaceAngle = math.acos(math.dot(surfaceNormal, math.up()));
                float angleFactor = math.cos(surfaceAngle);
                
                float speed = math.length(vehiclePhysics.Velocity);
                float speedFactor = math.clamp(1f - speed * 0.01f, 0.1f, 1f);
                
                return angleFactor * speedFactor;
            }
            
            /// <summary>
            /// Оптимизированное вычисление трения
            /// </summary>
            [BurstCompile]
            private static float3 CalculateFrictionOptimized(in WheelData wheel, in VehiclePhysics vehiclePhysics, 
                                                           float tractionMultiplier, float maxFrictionMultiplier)
            {
                if (!wheel.IsGrounded)
                    return float3.zero;
                
                float3 relativeVelocity = vehiclePhysics.Velocity;
                float3 frictionForce = -relativeVelocity * wheel.Traction * tractionMultiplier;
                
                float maxFriction = wheel.Traction * maxFrictionMultiplier;
                float frictionLength = math.length(frictionForce);
                
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
                if (!wheel.IsGrounded)
                    return 0f;
                
                float wheelSpeed = math.length(vehiclePhysics.Velocity);
                return wheelSpeed / wheel.Radius;
            }
        }
    }
}