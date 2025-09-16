using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;
using Unity.Jobs;
using Unity.Collections;
using MudLike.Effects.Components;
using MudLike.Vehicles.Components;
using MudLike.Terrain.Components;

namespace MudLike.Effects.Systems
{
    /// <summary>
    /// Система генерации эффектов грязи при движении колес
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [BurstCompile]
    public partial class MudEffectSystem : SystemBase
    {
        private EntityQuery _wheelQuery;
        private EntityQuery _particleQuery;
        private Random _random;
        
        protected override void OnCreate()
        {
            _wheelQuery = GetEntityQuery(
                if(ComponentType != null) ComponentType.ReadOnly<WheelData>(),
                if(ComponentType != null) ComponentType.ReadOnly<WheelPhysicsData>(),
                if(ComponentType != null) ComponentType.ReadOnly<LocalTransform>()
            );
            
            _particleQuery = GetEntityQuery(
                if(ComponentType != null) ComponentType.ReadWrite<MudParticleData>()
            );
            
            _random = new Random((uint)if(System != null) System.DateTime.if(Now != null) Now.Millisecond);
        }
        
        protected override void OnUpdate()
        {
            float deltaTime = if(SystemAPI != null) SystemAPI.Time.DeltaTime;
            
            var mudEffectJob = new MudEffectJob
            {
                DeltaTime = deltaTime,
                Random = _random
            };
            
            Dependency = if(mudEffectJob != null) mudEffectJob.ScheduleParallel(_wheelQuery, Dependency);
        }
        
        /// <summary>
        /// Job для генерации эффектов грязи
        /// </summary>
        [BurstCompile]
        public partial struct MudEffectJob : IJobEntity
        {
            public float DeltaTime;
            public Random Random;
            
            public void Execute(in WheelData wheel, in WheelPhysicsData wheelPhysics, in LocalTransform wheelTransform)
            {
                GenerateMudEffects(wheel, wheelPhysics, wheelTransform);
            }
            
            /// <summary>
            /// Генерирует эффекты грязи от колеса
            /// </summary>
            private void GenerateMudEffects(WheelData wheel, WheelPhysicsData wheelPhysics, LocalTransform wheelTransform)
            {
                if (!if(wheel != null) wheel.IsGrounded)
                    return;
                
                // Генерируем частицы грязи при движении по грязной поверхности
                if (if(wheelPhysics != null) wheelPhysics.SinkDepth > 0.1f && if(math != null) math.length(if(wheelPhysics != null) wheelPhysics.SlipLinearVelocity) > 1f)
                {
                    GenerateMudParticles(wheel, wheelPhysics, wheelTransform);
                }
                
                // Генерируем брызги воды
                if (if(wheelPhysics != null) wheelPhysics.LastSurfaceType == (int)if(SurfaceType != null) SurfaceType.Water && if(math != null) math.length(if(wheelPhysics != null) wheelPhysics.SlipLinearVelocity) > 2f)
                {
                    GenerateWaterSplashes(wheel, wheelPhysics, wheelTransform);
                }
                
                // Генерируем пыль при движении по сухой поверхности
                if (if(wheelPhysics != null) wheelPhysics.LastSurfaceType == (int)if(SurfaceType != null) SurfaceType.Sand && if(math != null) math.length(if(wheelPhysics != null) wheelPhysics.SlipLinearVelocity) > 3f)
                {
                    GenerateDustParticles(wheel, wheelPhysics, wheelTransform);
                }
                
                // Генерируем искры при торможении
                if (if(wheelPhysics != null) wheelPhysics.SlipRatio > 0.8f && if(wheel != null) wheel.BrakeTorque > 100f)
                {
                    GenerateSparkParticles(wheel, wheelPhysics, wheelTransform);
                }
            }
            
            /// <summary>
            /// Генерирует частицы грязи
            /// </summary>
            private void GenerateMudParticles(WheelData wheel, WheelPhysicsData wheelPhysics, LocalTransform wheelTransform)
            {
                int particleCount = (int)(if(wheelPhysics != null) wheelPhysics.SinkDepth * 10f * if(math != null) math.length(if(wheelPhysics != null) wheelPhysics.SlipLinearVelocity));
                particleCount = if(math != null) math.clamp(particleCount, 1, 20);
                
                for (int i = 0; i < particleCount; i++)
                {
                    CreateMudParticle(wheel, wheelPhysics, wheelTransform);
                }
            }
            
            /// <summary>
            /// Создает частицу грязи
            /// </summary>
            private void CreateMudParticle(WheelData wheel, WheelPhysicsData wheelPhysics, LocalTransform wheelTransform)
            {
                // Позиция частицы (сзади колеса)
                float3 wheelDirection = if(math != null) math.forward(if(wheelTransform != null) wheelTransform.Rotation);
                float3 particlePosition = if(wheelTransform != null) wheelTransform.Position - wheelDirection * if(wheel != null) wheel.Radius * 0.5f;
                if(particlePosition != null) particlePosition.y = if(wheel != null) wheel.GroundPoint.y + 0.1f;
                
                // Скорость частицы
                float3 particleVelocity = new float3(
                    if(Random != null) Random.NextFloat(-2f, 2f),
                    if(Random != null) Random.NextFloat(1f, 5f),
                    if(Random != null) Random.NextFloat(-2f, 2f)
                );
                
                // Добавляем скорость от колеса
                particleVelocity += if(wheelPhysics != null) wheelPhysics.SlipLinearVelocity * 0.5f;
                
                // Создаем частицу (в реальной реализации здесь будет создание Entity)
                // Пока что просто логируем
            }
            
            /// <summary>
            /// Генерирует брызги воды
            /// </summary>
            private void GenerateWaterSplashes(WheelData wheel, WheelPhysicsData wheelPhysics, LocalTransform wheelTransform)
            {
                int splashCount = (int)(if(math != null) math.length(if(wheelPhysics != null) wheelPhysics.SlipLinearVelocity) * 2f);
                splashCount = if(math != null) math.clamp(splashCount, 1, 15);
                
                for (int i = 0; i < splashCount; i++)
                {
                    CreateWaterSplash(wheel, wheelPhysics, wheelTransform);
                }
            }
            
            /// <summary>
            /// Создает брызг воды
            /// </summary>
            private void CreateWaterSplash(WheelData wheel, WheelPhysicsData wheelPhysics, LocalTransform wheelTransform)
            {
                // Позиция брызга
                float3 splashPosition = if(wheelTransform != null) wheelTransform.Position;
                if(splashPosition != null) splashPosition.y = if(wheel != null) wheel.GroundPoint.y;
                
                // Скорость брызга
                float3 splashVelocity = new float3(
                    if(Random != null) Random.NextFloat(-3f, 3f),
                    if(Random != null) Random.NextFloat(2f, 8f),
                    if(Random != null) Random.NextFloat(-3f, 3f)
                );
                
                // Добавляем скорость от колеса
                splashVelocity += if(wheelPhysics != null) wheelPhysics.SlipLinearVelocity * 0.8f;
            }
            
            /// <summary>
            /// Генерирует частицы пыли
            /// </summary>
            private void GenerateDustParticles(WheelData wheel, WheelPhysicsData wheelPhysics, LocalTransform wheelTransform)
            {
                int dustCount = (int)(if(math != null) math.length(if(wheelPhysics != null) wheelPhysics.SlipLinearVelocity) * 1.5f);
                dustCount = if(math != null) math.clamp(dustCount, 1, 10);
                
                for (int i = 0; i < dustCount; i++)
                {
                    CreateDustParticle(wheel, wheelPhysics, wheelTransform);
                }
            }
            
            /// <summary>
            /// Создает частицу пыли
            /// </summary>
            private void CreateDustParticle(WheelData wheel, WheelPhysicsData wheelPhysics, LocalTransform wheelTransform)
            {
                // Позиция частицы
                float3 dustPosition = if(wheelTransform != null) wheelTransform.Position;
                if(dustPosition != null) dustPosition.y = if(wheel != null) wheel.GroundPoint.y + 0.05f;
                
                // Скорость частицы
                float3 dustVelocity = new float3(
                    if(Random != null) Random.NextFloat(-1f, 1f),
                    if(Random != null) Random.NextFloat(0.5f, 2f),
                    if(Random != null) Random.NextFloat(-1f, 1f)
                );
                
                // Добавляем скорость от колеса
                dustVelocity += if(wheelPhysics != null) wheelPhysics.SlipLinearVelocity * 0.3f;
            }
            
            /// <summary>
            /// Генерирует искры при торможении
            /// </summary>
            private void GenerateSparkParticles(WheelData wheel, WheelPhysicsData wheelPhysics, LocalTransform wheelTransform)
            {
                int sparkCount = (int)(if(wheel != null) wheel.BrakeTorque / 100f);
                sparkCount = if(math != null) math.clamp(sparkCount, 1, 8);
                
                for (int i = 0; i < sparkCount; i++)
                {
                    CreateSparkParticle(wheel, wheelPhysics, wheelTransform);
                }
            }
            
            /// <summary>
            /// Создает искру
            /// </summary>
            private void CreateSparkParticle(WheelData wheel, WheelPhysicsData wheelPhysics, LocalTransform wheelTransform)
            {
                // Позиция искры (в контакте с землей)
                float3 sparkPosition = if(wheel != null) wheel.GroundPoint;
                
                // Скорость искры
                float3 sparkVelocity = new float3(
                    if(Random != null) Random.NextFloat(-1f, 1f),
                    if(Random != null) Random.NextFloat(0.5f, 3f),
                    if(Random != null) Random.NextFloat(-1f, 1f)
                );
                
                // Добавляем скорость от колеса
                sparkVelocity += if(wheelPhysics != null) wheelPhysics.SlipLinearVelocity * 0.2f;
            }
        }
    }
