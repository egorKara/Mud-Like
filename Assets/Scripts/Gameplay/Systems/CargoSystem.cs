using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;
using MudLike.Gameplay.Components;

namespace MudLike.Gameplay.Systems
{
    /// <summary>
    /// Система груза
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [BurstCompile]
    public partial class CargoSystem : SystemBase
    {
        private EntityQuery _cargoQuery;
        
        protected override void OnCreate()
        {
            _cargoQuery = GetEntityQuery(
                ComponentType.ReadWrite<CargoData>(),
                ComponentType.ReadOnly<LocalTransform>()
            );
        }
        
        protected override void OnUpdate()
        {
            float deltaTime = SystemAPI.Time.DeltaTime;
            
            // Обновляем груз
            UpdateCargo(deltaTime);
        }
        
        private void UpdateCargo(float deltaTime)
        {
            // Обновляем каждый груз
            Entities
                .WithAll<CargoData>()
                .ForEach((ref CargoData cargo, in LocalTransform transform) =>
                {
                    UpdateCargoData(ref cargo, transform, deltaTime);
                }).Schedule();
        }
        
        private void UpdateCargoData(ref CargoData cargo, in LocalTransform transform, float deltaTime)
        {
            // Простая реализация груза
            // В реальной реализации здесь будет сложная логика груза
            
            // Обновляем время последнего обновления
            cargo.LastUpdateTime += deltaTime;
            
            // Обновляем статус
            cargo.NeedsUpdate = false;
        }
    }
}