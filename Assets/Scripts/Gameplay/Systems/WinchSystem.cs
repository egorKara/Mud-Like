using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;
using MudLike.Gameplay.Components;

namespace MudLike.Gameplay.Systems
{
    /// <summary>
    /// Система лебедки
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [BurstCompile]
    public partial class WinchSystem : SystemBase
    {
        private EntityQuery _winchQuery;
        
        protected override void OnCreate()
        {
            _winchQuery = GetEntityQuery(
                ComponentType.ReadWrite<WinchData>(),
                ComponentType.ReadOnly<LocalTransform>()
            );
        }
        
        protected override void OnUpdate()
        {
            float deltaTime = SystemAPI.Time.DeltaTime;
            
            // Обновляем лебедки
            UpdateWinches(deltaTime);
        }
        
        private void UpdateWinches(float deltaTime)
        {
            // Обновляем каждую лебедку
            Entities
                .WithAll<WinchData>()
                .ForEach((ref WinchData winch, in LocalTransform transform) =>
                {
                    UpdateWinch(ref winch, transform, deltaTime);
                }).Schedule();
        }
        
        private void UpdateWinch(ref WinchData winch, in LocalTransform transform, float deltaTime)
        {
            // Простая реализация лебедки
            // В реальной реализации здесь будет сложная физика лебедки
            
            // Обновляем температуру лебедки
            winch.Temperature += 0.1f * deltaTime;
            winch.Temperature = math.clamp(winch.Temperature, 20f, 100f);
            
            // Обновляем износ лебедки
            if (winch.IsPulling || winch.IsReleasing)
            {
                winch.Wear += 0.001f * deltaTime;
                winch.Wear = math.clamp(winch.Wear, 0f, 1f);
            }
            
            // Проверяем необходимость обслуживания
            winch.MaintenanceRequired = winch.Wear > 0.8f;
            
            // Обновляем статус
            winch.IsActive = true;
            winch.NeedsUpdate = false;
        }
    }
}