using Unity.Entities;
using Unity.Mathematics;
using Unity.Burst;
using MudLike.Gameplay.Components;

namespace MudLike.Gameplay.Systems
{
    /// <summary>
    /// Система миссий
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [BurstCompile]
    public partial class MissionSystem : SystemBase
    {
        private EntityQuery _missionQuery;
        
        protected override void OnCreate()
        {
            _missionQuery = GetEntityQuery(
                ComponentType.ReadWrite<MissionData>()
            );
        }
        
        protected override void OnUpdate()
        {
            float deltaTime = SystemAPI.Time.DeltaTime;
            
            // Обновляем миссии
            UpdateMissions(deltaTime);
        }
        
        private void UpdateMissions(float deltaTime)
        {
            // Обновляем каждую миссию
            Entities
                .WithAll<MissionData>()
                .ForEach((ref MissionData mission) =>
                {
                    UpdateMission(ref mission, deltaTime);
                }).Schedule();
        }
        
        private void UpdateMission(ref MissionData mission, float deltaTime)
        {
            // Простая реализация миссий
            // В реальной реализации здесь будет сложная логика миссий
            
            // Обновляем время последнего обновления
            mission.LastUpdateTime += deltaTime;
            
            // Обновляем статус
            mission.NeedsUpdate = false;
        }
    }
}