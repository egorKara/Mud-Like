using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;
using MudLike.Terrain.Components;

namespace MudLike.Terrain.Systems
{
    /// <summary>
    /// Система синхронизации деформаций террейна между клиентами
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [BurstCompile]
    public partial class TerrainSyncSystem : SystemBase
    {
        private EntityQuery _terrainQuery;
        
        protected override void OnCreate()
        {
            _terrainQuery = GetEntityQuery(
                ComponentType.ReadWrite<TerrainData>()
            );
        }
        
        protected override void OnUpdate()
        {
            float deltaTime = SystemAPI.Time.DeltaTime;
            
            // Синхронизируем деформации террейна
            SyncTerrainDeformations(deltaTime);
        }
        
        private void SyncTerrainDeformations(float deltaTime)
        {
            // Синхронизируем каждую деформацию террейна
            Entities
                .WithAll<TerrainData>()
                .ForEach((ref TerrainData terrain) =>
                {
                    SyncTerrainData(ref terrain, deltaTime);
                }).Schedule();
        }
        
        private void SyncTerrainData(ref TerrainData terrain, float deltaTime)
        {
            // Простая реализация синхронизации
            // В реальной реализации здесь будет сложная сетевая синхронизация
            
            if (terrain.NeedsUpdate)
            {
                // Синхронизируем деформацию
                terrain.ColliderNeedsUpdate = true;
                terrain.NeedsUpdate = false;
            }
        }
    }
}