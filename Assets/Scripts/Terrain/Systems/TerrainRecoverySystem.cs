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
                ComponentType.ReadWrite<TerrainData>()
            );
        }
        
        protected override void OnUpdate()
        {
            var terrainEntities = _terrainQuery.ToEntityArray(Allocator.TempJob);
            
            if (terrainEntities.Length == 0)
            {
                terrainEntities.Dispose();
                return;
            }
            
            // Создание Job для восстановления террейна
            var recoveryJob = new TerrainRecoveryJob
            {
                HeightMap = GetTerrainHeightMap(),
                HardnessMap = GetTerrainHardnessMap(),
                RecoveryRate = SystemConstants.TERRAIN_DEFAULT_RECOVERY_RATE,
                DeltaTime = SystemAPI.Time.fixedDeltaTime
            };
            
            // Запуск Job с оптимальным batch size
            var jobHandle = recoveryJob.ScheduleParallel(
                GetTerrainHeightMap().Length,
                SystemConstants.TERRAIN_DEFAULT_RESOLUTION / 128,
                Dependency
            );
            
            Dependency = jobHandle;
            terrainEntities.Dispose();
        }
        
        private NativeArray<float> GetTerrainHeightMap()
        {
            // Получение высотной карты террейна
            // Реализация зависит от конкретной структуры данных
            return new NativeArray<float>(SystemConstants.TERRAIN_DEFAULT_RESOLUTION * SystemConstants.TERRAIN_DEFAULT_RESOLUTION, Allocator.TempJob);
        }
        
        private NativeArray<float> GetTerrainHardnessMap()
        {
            // Получение карты твердости террейна
            // Реализация зависит от конкретной структуры данных
            return new NativeArray<float>(SystemConstants.TERRAIN_DEFAULT_RESOLUTION * SystemConstants.TERRAIN_DEFAULT_RESOLUTION, Allocator.TempJob);
        }
    }
}
