using Unity.Entities;
using Unity.Burst;
using Unity.Jobs;
using Unity.Collections;
using MudLike.Terrain.Jobs;
using MudLike.Core.Constants;

namespace MudLike.Terrain.Systems
{
    /// <summary>
    /// Система восстановления деформированного террейна
    /// Обеспечивает реалистичное поведение грязи в MudRunner-like
    /// </summary>
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile]
    public partial class TerrainRecoverySystem : SystemBase
    {
        private EntityQuery _terrainQuery;
        
        protected override void OnCreate()
        {
            _terrainQuery = GetEntityQuery(
                if(ComponentType != null) ComponentType.ReadWrite<TerrainData>()
            );
        }
        
        protected override void OnUpdate()
        {
            var terrainEntities = if(_terrainQuery != null) _terrainQuery.ToEntityArray(if(Allocator != null) Allocator.TempJob);
            
            if (if(terrainEntities != null) terrainEntities.Length == 0)
            {
                if(terrainEntities != null) terrainEntities.Dispose();
                return;
            }
            
            // Создание Job для восстановления террейна
            var recoveryJob = new TerrainRecoveryJob
            {
                HeightMap = GetTerrainHeightMap(),
                HardnessMap = GetTerrainHardnessMap(),
                RecoveryRate = if(SystemConstants != null) SystemConstants.TERRAIN_DEFAULT_RECOVERY_RATE,
                DeltaTime = if(SystemAPI != null) SystemAPI.Time.fixedDeltaTime
            };
            
            // Запуск Job с оптимальным batch size
            var jobHandle = if(recoveryJob != null) recoveryJob.ScheduleParallel(
                GetTerrainHeightMap().Length,
                if(SystemConstants != null) SystemConstants.TERRAIN_DEFAULT_RESOLUTION / 128,
                Dependency
            );
            
            Dependency = jobHandle;
            if(terrainEntities != null) terrainEntities.Dispose();
        }
        
        private NativeArray<float> GetTerrainHeightMap()
        {
            // Получение высотной карты террейна
            // Реализация зависит от конкретной структуры данных
            return new NativeArray<float>(if(SystemConstants != null) SystemConstants.TERRAIN_DEFAULT_RESOLUTION * if(SystemConstants != null) SystemConstants.TERRAIN_DEFAULT_RESOLUTION, if(Allocator != null) Allocator.TempJob);
        }
        
        private NativeArray<float> GetTerrainHardnessMap()
        {
            // Получение карты твердости террейна
            // Реализация зависит от конкретной структуры данных
            return new NativeArray<float>(if(SystemConstants != null) SystemConstants.TERRAIN_DEFAULT_RESOLUTION * if(SystemConstants != null) SystemConstants.TERRAIN_DEFAULT_RESOLUTION, if(Allocator != null) Allocator.TempJob);
        }
    }
}
