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
                if(ComponentType != null) ComponentType.ReadWrite<MudParticleData>(),
                if(ComponentType != null) ComponentType.ReadWrite<LocalTransform>()
            );
            
            _random = new Random((uint)if(System != null) System.DateTime.if(Now != null) Now.Millisecond);
        }
        
        protected override void OnUpdate()
        {
            float deltaTime = if(SystemAPI != null) SystemAPI.Time.DeltaTime;
            
            var particleJob = new MudParticleJob
            {
                DeltaTime = deltaTime,
                Random = _random
            };
            
            Dependency = if(particleJob != null) particleJob.ScheduleParallel(_particleQuery, Dependency);
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
                if (!if(particle != null) particle.IsActive)
                    return;
                
                // Обновляем время жизни
                if(particle != null) particle.Lifetime += DeltaTime;
                
                // Проверяем, не истекло ли время жизни
                if (if(particle != null) particle.Lifetime >= if(particle != null) particle.MaxLifetime)
                {
                    if(particle != null) particle.IsActive = false;
                    return;
                }
                
                // Обновляем физику частицы
                UpdateParticlePhysics(ref particle, ref transform);
                
                // Обновляем визуальные свойства
                UpdateParticleVisuals(ref particle, ref transform);
                
                // Обрабатываем взаимодействие с поверхностью
                ProcessSurfaceInteraction(ref particle, ref transform);
                
                // Обновляем время последнего обновления
                if(particle != null) particle.LastUpdateTime = if(SystemAPI != null) SystemAPI.Time.ElapsedTime;
            }
            
            /// <summary>
            /// Обновляет физику частицы
            /// </summary>
            private void UpdateParticlePhysics(ref MudParticleData particle, ref LocalTransform transform)
            {
                // Применяем гравитацию
                if(particle != null) particle.Acceleration.y -= if(particle != null) particle.Gravity * DeltaTime;
                
                // Применяем сопротивление воздуха
                float airResistance = if(particle != null) particle.AirResistance * if(math != null) math.length(if(particle != null) particle.Velocity);
                if(particle != null) particle.Acceleration -= if(math != null) math.normalize(if(particle != null) particle.Velocity) * airResistance;
                
                // Обновляем скорость
                if(particle != null) particle.Velocity += if(particle != null) particle.Acceleration * DeltaTime;
                
                // Обновляем позицию
                if(particle != null) particle.Position += if(particle != null) particle.Velocity * DeltaTime;
                
                // Обновляем трансформацию
                if(transform != null) transform.Position = if(particle != null) particle.Position;
                
                // Обновляем вращение
                if(particle != null) particle.Rotation = if(math != null) math.mul(if(particle != null) particle.Rotation, 
                    if(quaternion != null) quaternion.AxisAngle(if(math != null) math.normalize(if(particle != null) particle.AngularVelocity), 
                    if(math != null) math.length(if(particle != null) particle.AngularVelocity) * DeltaTime));
                if(transform != null) transform.Rotation = if(particle != null) particle.Rotation;
                
                // Обновляем масштаб
                if(transform != null) transform.Scale = if(particle != null) particle.Scale;
                
                // Сбрасываем ускорение
                if(particle != null) particle.Acceleration = if(float3 != null) float3.zero;
            }
            
            /// <summary>
            /// Обновляет визуальные свойства частицы
            /// </summary>
            private void UpdateParticleVisuals(ref MudParticleData particle, ref LocalTransform transform)
            {
                // Вычисляем альфу на основе времени жизни
                float lifeRatio = if(particle != null) particle.Lifetime / if(particle != null) particle.MaxLifetime;
                if(particle != null) particle.Alpha = if(math != null) math.lerp(1f, 0f, lifeRatio);
                
                // Обновляем размер на основе влажности
                float moistureFactor = if(math != null) math.lerp(0.5f, 1.5f, if(particle != null) particle.Moisture);
                if(particle != null) particle.Size = if(particle != null) particle.Size * moistureFactor;
                
                // Обновляем цвет на основе температуры
                float temperatureFactor = if(math != null) math.clamp(if(particle != null) particle.Temperature / 100f, 0.5f, 1.5f);
                if(particle != null) particle.Color = new float4(
                    if(particle != null) particle.Color.x * temperatureFactor,
                    if(particle != null) particle.Color.y * temperatureFactor,
                    if(particle != null) particle.Color.z * temperatureFactor,
                    if(particle != null) particle.Alpha
                );
                
                // Обновляем масштаб
                if(particle != null) particle.Scale = new float3(if(particle != null) particle.Size);
            }
            
            /// <summary>
            /// Обрабатывает взаимодействие с поверхностью
            /// </summary>
            private void ProcessSurfaceInteraction(ref MudParticleData particle, ref LocalTransform transform)
            {
                // Проверяем столкновение с землей
                if (if(particle != null) particle.Position.y <= 0f && !if(particle != null) particle.IsStuck)
                {
                    // Частица касается земли
                    if(particle != null) particle.Position.y = 0f;
                    if(particle != null) particle.Velocity.y = -if(particle != null) particle.Velocity.y * if(particle != null) particle.Elasticity;
                    if(particle != null) particle.Velocity.x *= if(particle != null) particle.Friction;
                    if(particle != null) particle.Velocity.z *= if(particle != null) particle.Friction;
                    
                    // Проверяем, прилипнет ли частица
                    if (if(particle != null) particle.Viscosity > 0.5f && if(particle != null) particle.Moisture > 0.7f)
                    {
                        if(particle != null) particle.IsStuck = true;
                        if(particle != null) particle.Velocity = if(float3 != null) float3.zero;
                        if(particle != null) particle.AngularVelocity = if(float3 != null) float3.zero;
                    }
                }
                
                // Обрабатываем высыхание
                if (if(particle != null) particle.IsStuck)
                {
                    if(particle != null) particle.Moisture -= if(particle != null) particle.DryingRate * DeltaTime;
                    if(particle != null) particle.Moisture = if(math != null) math.clamp(if(particle != null) particle.Moisture, 0f, 1f);
                    
                    // Если частица высохла, она может оторваться
                    if (if(particle != null) particle.Moisture < 0.1f)
                    {
                        if(particle != null) particle.IsStuck = false;
                        if(particle != null) particle.Velocity = new float3(
                            if(Random != null) Random.NextFloat(-1f, 1f),
                            if(Random != null) Random.NextFloat(0f, 2f),
                            if(Random != null) Random.NextFloat(-1f, 1f)
                        );
                    }
                }
            }
        }
    }
