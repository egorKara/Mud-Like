using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;
using MudLike.Effects.Components;

namespace MudLike.Effects.Systems
{
    /// <summary>
    /// Система частиц грязи
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [BurstCompile]
    public partial class MudParticleSystem : SystemBase
    {
        private EntityQuery _particleQuery;
        
        protected override void OnCreate()
        {
            _particleQuery = GetEntityQuery(
                ComponentType.ReadWrite<MudParticleData>(),
                ComponentType.ReadWrite<LocalTransform>(),
                ComponentType.ReadOnly<ActiveParticleTag>()
            );
        }
        
        protected override void OnUpdate()
        {
            float deltaTime = SystemAPI.Time.DeltaTime;
            
            // Обновляем частицы грязи
            UpdateMudParticles(deltaTime);
        }
        
        private void UpdateMudParticles(float deltaTime)
        {
            // Обновляем каждую частицу грязи
            Entities
                .WithAll<ActiveParticleTag>()
                .ForEach((ref MudParticleData particle, ref LocalTransform transform) =>
                {
                    UpdateParticle(ref particle, ref transform, deltaTime);
                }).Schedule();
        }
        
        private void UpdateParticle(ref MudParticleData particle, ref LocalTransform transform, float deltaTime)
        {
            if (!particle.IsActive)
                return;
            
            // Обновляем время жизни
            particle.Lifetime += deltaTime;
            
            // Проверяем, не истекло ли время жизни
            if (particle.Lifetime >= 5f)
            {
                particle.IsActive = false;
                return;
            }
            
            // Применяем гравитацию
            particle.Velocity.y -= 9.81f * deltaTime;
            
            // Применяем сопротивление воздуха
            particle.Velocity *= 0.99f;
            
            // Обновляем позицию
            particle.Position += particle.Velocity * deltaTime;
            transform.Position = particle.Position;
            
            // Обновляем размер на основе времени жизни
            float lifeRatio = particle.Lifetime / 5f;
            float sizeMultiplier = 1f - (lifeRatio * 0.5f);
            transform.Scale = particle.Size * sizeMultiplier;
        }
    }
}