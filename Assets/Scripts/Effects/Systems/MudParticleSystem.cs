using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using Unity.Jobs;
using Unity.Burst;
using MudLike.Effects.Components;
using MudLike.Vehicles.Components;

namespace MudLike.Effects.Systems
{
    /// <summary>
    /// Система частиц грязи
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [BurstCompile]
    public partial class MudParticleSystem : SystemBase
    {
        /// <summary>
        /// Максимальное количество частиц
        /// </summary>
        private const int MAX_PARTICLES = 1000;
        
        /// <summary>
        /// Скорость создания частиц
        /// </summary>
        private const float PARTICLE_SPAWN_RATE = 10f;
        
        /// <summary>
        /// Гравитация для частиц
        /// </summary>
        private const float GRAVITY = -9.81f;
        
        /// <summary>
        /// Обрабатывает частицы грязи
        /// </summary>
        protected override void OnUpdate()
        {
            float deltaTime = SystemAPI.Time.DeltaTime;
            
            // Создаем job для обновления частиц
            var particleJob = new MudParticleJob
            {
                DeltaTime = deltaTime,
                Gravity = GRAVITY
            };
            
            // Запускаем job
            var jobHandle = particleJob.ScheduleParallel(this);
            jobHandle.Complete();
            
            // Создаем новые частицы от колес
            SpawnParticlesFromWheels(deltaTime);
        }
        
        /// <summary>
        /// Создает частицы от колес
        /// </summary>
        private void SpawnParticlesFromWheels(float deltaTime)
        {
            // Находим все колеса
            Entities
                .WithAll<WheelData>()
                .ForEach((in WheelData wheel, in LocalTransform transform) =>
                {
                    if (wheel.SinkDepth > 0.1f && wheel.AngularVelocity > 1f)
                    {
                        SpawnMudParticles(transform.Position, wheel.Velocity, wheel.SinkDepth);
                    }
                }).WithoutBurst().Run();
        }
        
        /// <summary>
        /// Создает частицы грязи
        /// </summary>
        private void SpawnMudParticles(float3 position, float3 velocity, float sinkDepth)
        {
            // Создаем несколько частиц
            int particleCount = (int)(sinkDepth * PARTICLE_SPAWN_RATE);
            particleCount = math.min(particleCount, 5); // Максимум 5 частиц за раз
            
            for (int i = 0; i < particleCount; i++)
            {
                var particleEntity = EntityManager.CreateEntity();
                
                // Добавляем компоненты частицы
                EntityManager.AddComponent<MudParticleData>(particleEntity);
                EntityManager.AddComponent<LocalTransform>(particleEntity);
                
                // Инициализируем данные частицы
                var particleData = new MudParticleData
                {
                    Position = position + new float3(
                        (math.random() - 0.5f) * 2f,
                        math.random() * 0.5f,
                        (math.random() - 0.5f) * 2f
                    ),
                    Velocity = velocity + new float3(
                        (math.random() - 0.5f) * 5f,
                        math.random() * 3f,
                        (math.random() - 0.5f) * 5f
                    ),
                    Size = math.random() * 0.2f + 0.1f,
                    Lifetime = 0f,
                    MaxLifetime = math.random() * 3f + 1f,
                    Color = new float4(0.4f, 0.2f, 0.1f, 1f), // Коричневый цвет грязи
                    Mass = math.random() * 0.1f + 0.05f,
                    Viscosity = math.random() * 0.5f + 0.3f,
                    IsActive = true
                };
                
                EntityManager.SetComponentData(particleEntity, particleData);
                
                // Устанавливаем трансформацию
                var transform = new LocalTransform
                {
                    Position = particleData.Position,
                    Rotation = quaternion.identity,
                    Scale = particleData.Size
                };
                
                EntityManager.SetComponentData(particleEntity, transform);
            }
        }
    }
    
    /// <summary>
    /// Job для обновления частиц грязи
    /// </summary>
    [BurstCompile]
    public struct MudParticleJob : IJobEntity
    {
        public float DeltaTime;
        public float Gravity;
        
        public void Execute(ref MudParticleData particle, ref LocalTransform transform)
        {
            if (!particle.IsActive) return;
            
            // Обновляем время жизни
            particle.Lifetime += DeltaTime;
            
            if (particle.Lifetime >= particle.MaxLifetime)
            {
                particle.IsActive = false;
                return;
            }
            
            // Применяем гравитацию
            particle.Velocity.y += Gravity * DeltaTime;
            
            // Обновляем позицию
            particle.Position += particle.Velocity * DeltaTime;
            
            // Применяем сопротивление воздуха
            particle.Velocity *= 0.98f;
            
            // Обновляем трансформацию
            transform.Position = particle.Position;
            transform.Scale = particle.Size * (1f - particle.Lifetime / particle.MaxLifetime);
            
            // Обновляем альфу в зависимости от времени жизни
            float alpha = 1f - (particle.Lifetime / particle.MaxLifetime);
            particle.Color.w = alpha;
        }
    }
}