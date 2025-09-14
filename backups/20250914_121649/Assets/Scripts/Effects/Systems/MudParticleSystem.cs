using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;
using Unity.Jobs;
using Unity.Collections;
using MudLike.Effects.Components;

namespace MudLike.Effects.Systems
{
    /// <summary>
    /// Система обработки частиц грязи
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [BurstCompile(CompileSynchronously = true)]
    public partial class MudParticleSystem : SystemBase
    {
        private EntityQuery _particleQuery;
        private Random _random;
        
        protected override void OnCreate()
        {
            _particleQuery = GetEntityQuery(
                ComponentType.ReadWrite<MudParticleData>(),
                ComponentType.ReadWrite<LocalTransform>()
            );
            
            _random = new Random((uint)System.DateTime.Now.Millisecond);
        }
        
        protected override void OnUpdate()
        {
            float deltaTime = SystemAPI.Time.DeltaTime;
            
            var particleJob = new MudParticleJob
            {
                DeltaTime = deltaTime,
                Random = _random
            };
            
            Dependency = particleJob.ScheduleParallel(_particleQuery, Dependency);
        }
        
        /// <summary>
        /// Job для обработки частиц грязи
        /// </summary>
        [BurstCompile]
        public partial struct MudParticleJob : IJobEntity
        {
            public float DeltaTime;
            public Random Random;
            
            public void Execute(ref MudParticleData particle, ref LocalTransform transform)
            {
                ProcessMudParticle(ref particle, ref transform);
            }
            
            /// <summary>
            /// Обрабатывает частицу грязи
            /// </summary>
            private void ProcessMudParticle(ref MudParticleData particle, ref LocalTransform transform)
            {
                if (!particle.IsActive)
                    return;
                
                // Обновляем время жизни
                particle.Lifetime += DeltaTime;
                
                // Проверяем, не истекло ли время жизни
                if (particle.Lifetime >= particle.MaxLifetime)
                {
                    particle.IsActive = false;
                    return;
                }
                
                // Обновляем физику частицы
                UpdateParticlePhysics(ref particle, ref transform);
                
                // Обновляем визуальные свойства
                UpdateParticleVisuals(ref particle, ref transform);
                
                // Обрабатываем взаимодействие с поверхностью
                ProcessSurfaceInteraction(ref particle, ref transform);
                
                // Обновляем время последнего обновления
                particle.LastUpdateTime = SystemAPI.Time.ElapsedTime;
            }
            
            /// <summary>
            /// Обновляет физику частицы
            /// </summary>
            private void UpdateParticlePhysics(ref MudParticleData particle, ref LocalTransform transform)
            {
                // Применяем гравитацию
                particle.Acceleration.y -= particle.Gravity * DeltaTime;
                
                // Применяем сопротивление воздуха
                float airResistance = particle.AirResistance * math.length(particle.Velocity);
                particle.Acceleration -= math.normalize(particle.Velocity) * airResistance;
                
                // Обновляем скорость
                particle.Velocity += particle.Acceleration * DeltaTime;
                
                // Обновляем позицию
                particle.Position += particle.Velocity * DeltaTime;
                
                // Обновляем трансформацию
                transform.Position = particle.Position;
                
                // Обновляем вращение
                particle.Rotation = math.mul(particle.Rotation, 
                    quaternion.AxisAngle(math.normalize(particle.AngularVelocity), 
                    math.length(particle.AngularVelocity) * DeltaTime));
                transform.Rotation = particle.Rotation;
                
                // Обновляем масштаб
                transform.Scale = particle.Scale;
                
                // Сбрасываем ускорение
                particle.Acceleration = float3.zero;
            }
            
            /// <summary>
            /// Обновляет визуальные свойства частицы
            /// </summary>
            private void UpdateParticleVisuals(ref MudParticleData particle, ref LocalTransform transform)
            {
                // Вычисляем альфу на основе времени жизни
                float lifeRatio = particle.Lifetime / particle.MaxLifetime;
                particle.Alpha = math.lerp(1f, 0f, lifeRatio);
                
                // Обновляем размер на основе влажности
                float moistureFactor = math.lerp(0.5f, 1.5f, particle.Moisture);
                particle.Size = particle.Size * moistureFactor;
                
                // Обновляем цвет на основе температуры
                float temperatureFactor = math.clamp(particle.Temperature / 100f, 0.5f, 1.5f);
                particle.Color = new float4(
                    particle.Color.x * temperatureFactor,
                    particle.Color.y * temperatureFactor,
                    particle.Color.z * temperatureFactor,
                    particle.Alpha
                );
                
                // Обновляем масштаб
                particle.Scale = new float3(particle.Size);
            }
            
            /// <summary>
            /// Обрабатывает взаимодействие с поверхностью
            /// </summary>
            private void ProcessSurfaceInteraction(ref MudParticleData particle, ref LocalTransform transform)
            {
                // Проверяем столкновение с землей
                if (particle.Position.y <= 0f && !particle.IsStuck)
                {
                    // Частица касается земли
                    particle.Position.y = 0f;
                    particle.Velocity.y = -particle.Velocity.y * particle.Elasticity;
                    particle.Velocity.x *= particle.Friction;
                    particle.Velocity.z *= particle.Friction;
                    
                    // Проверяем, прилипнет ли частица
                    if (particle.Viscosity > 0.5f && particle.Moisture > 0.7f)
                    {
                        particle.IsStuck = true;
                        particle.Velocity = float3.zero;
                        particle.AngularVelocity = float3.zero;
                    }
                }
                
                // Обрабатываем высыхание
                if (particle.IsStuck)
                {
                    particle.Moisture -= particle.DryingRate * DeltaTime;
                    particle.Moisture = math.clamp(particle.Moisture, 0f, 1f);
                    
                    // Если частица высохла, она может оторваться
                    if (particle.Moisture < 0.1f)
                    {
                        particle.IsStuck = false;
                        particle.Velocity = new float3(
                            Random.NextFloat(-1f, 1f),
                            Random.NextFloat(0f, 2f),
                            Random.NextFloat(-1f, 1f)
                        );
                    }
                }
            }
        }
    }
}