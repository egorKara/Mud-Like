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
                ComponentType.ReadOnly<WheelData>(),
                ComponentType.ReadOnly<WheelPhysicsData>(),
                ComponentType.ReadOnly<LocalTransform>()
            );
            
            _particleQuery = GetEntityQuery(
                ComponentType.ReadWrite<MudParticleData>()
            );
            
            _random = new Random((uint)System.DateTime.Now.Millisecond);
        }
        
        protected override void OnUpdate()
        {
            float deltaTime = SystemAPI.Time.DeltaTime;
            
            var mudEffectJob = new MudEffectJob
            {
                DeltaTime = deltaTime,
                Random = _random
            };
            
            Dependency = mudEffectJob.ScheduleParallel(_wheelQuery, Dependency);
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
                if (!wheel.IsGrounded)
                    return;
                
                // Генерируем частицы грязи при движении по грязной поверхности
                if (wheelPhysics.SinkDepth > 0.1f && math.length(wheelPhysics.SlipLinearVelocity) > 1f)
                {
                    GenerateMudParticles(wheel, wheelPhysics, wheelTransform);
                }
                
                // Генерируем брызги воды
                if (wheelPhysics.LastSurfaceType == (int)SurfaceType.Water && math.length(wheelPhysics.SlipLinearVelocity) > 2f)
                {
                    GenerateWaterSplashes(wheel, wheelPhysics, wheelTransform);
                }
                
                // Генерируем пыль при движении по сухой поверхности
                if (wheelPhysics.LastSurfaceType == (int)SurfaceType.Sand && math.length(wheelPhysics.SlipLinearVelocity) > 3f)
                {
                    GenerateDustParticles(wheel, wheelPhysics, wheelTransform);
                }
                
                // Генерируем искры при торможении
                if (wheelPhysics.SlipRatio > 0.8f && wheel.BrakeTorque > 100f)
                {
                    GenerateSparkParticles(wheel, wheelPhysics, wheelTransform);
                }
            }
            
            /// <summary>
            /// Генерирует частицы грязи
            /// </summary>
            private void GenerateMudParticles(WheelData wheel, WheelPhysicsData wheelPhysics, LocalTransform wheelTransform)
            {
                int particleCount = (int)(wheelPhysics.SinkDepth * 10f * math.length(wheelPhysics.SlipLinearVelocity));
                particleCount = math.clamp(particleCount, 1, 20);
                
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
                float3 wheelDirection = math.forward(wheelTransform.Rotation);
                float3 particlePosition = wheelTransform.Position - wheelDirection * wheel.Radius * 0.5f;
                particlePosition.y = wheel.GroundPoint.y + 0.1f;
                
                // Скорость частицы
                float3 particleVelocity = new float3(
                    Random.NextFloat(-2f, 2f),
                    Random.NextFloat(1f, 5f),
                    Random.NextFloat(-2f, 2f)
                );
                
                // Добавляем скорость от колеса
                particleVelocity += wheelPhysics.SlipLinearVelocity * 0.5f;
                
                // Создаем частицу (в реальной реализации здесь будет создание Entity)
                // Пока что просто логируем
            }
            
            /// <summary>
            /// Генерирует брызги воды
            /// </summary>
            private void GenerateWaterSplashes(WheelData wheel, WheelPhysicsData wheelPhysics, LocalTransform wheelTransform)
            {
                int splashCount = (int)(math.length(wheelPhysics.SlipLinearVelocity) * 2f);
                splashCount = math.clamp(splashCount, 1, 15);
                
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
                float3 splashPosition = wheelTransform.Position;
                splashPosition.y = wheel.GroundPoint.y;
                
                // Скорость брызга
                float3 splashVelocity = new float3(
                    Random.NextFloat(-3f, 3f),
                    Random.NextFloat(2f, 8f),
                    Random.NextFloat(-3f, 3f)
                );
                
                // Добавляем скорость от колеса
                splashVelocity += wheelPhysics.SlipLinearVelocity * 0.8f;
            }
            
            /// <summary>
            /// Генерирует частицы пыли
            /// </summary>
            private void GenerateDustParticles(WheelData wheel, WheelPhysicsData wheelPhysics, LocalTransform wheelTransform)
            {
                int dustCount = (int)(math.length(wheelPhysics.SlipLinearVelocity) * 1.5f);
                dustCount = math.clamp(dustCount, 1, 10);
                
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
                float3 dustPosition = wheelTransform.Position;
                dustPosition.y = wheel.GroundPoint.y + 0.05f;
                
                // Скорость частицы
                float3 dustVelocity = new float3(
                    Random.NextFloat(-1f, 1f),
                    Random.NextFloat(0.5f, 2f),
                    Random.NextFloat(-1f, 1f)
                );
                
                // Добавляем скорость от колеса
                dustVelocity += wheelPhysics.SlipLinearVelocity * 0.3f;
            }
            
            /// <summary>
            /// Генерирует искры при торможении
            /// </summary>
            private void GenerateSparkParticles(WheelData wheel, WheelPhysicsData wheelPhysics, LocalTransform wheelTransform)
            {
                int sparkCount = (int)(wheel.BrakeTorque / 100f);
                sparkCount = math.clamp(sparkCount, 1, 8);
                
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
                float3 sparkPosition = wheel.GroundPoint;
                
                // Скорость искры
                float3 sparkVelocity = new float3(
                    Random.NextFloat(-1f, 1f),
                    Random.NextFloat(0.5f, 3f),
                    Random.NextFloat(-1f, 1f)
                );
                
                // Добавляем скорость от колеса
                sparkVelocity += wheelPhysics.SlipLinearVelocity * 0.2f;
            }
        }
    }
}