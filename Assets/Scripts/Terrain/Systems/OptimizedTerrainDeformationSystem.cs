using Unity.Entities;
using Unity.Burst;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using MudLike.Core.Constants;

namespace MudLike.Terrain.Systems
{
    /// <summary>
    /// Оптимизированная система деформации террейна с Job System и Burst
    /// Критично для реалистичной деформации MudRunner-like
    /// </summary>
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile(CompileSynchronously = true)]
    public partial class OptimizedTerrainDeformationSystem : SystemBase
    {
        private EntityQuery _terrainQuery;
        private EntityQuery _wheelQuery;
        
        protected override void OnCreate()
        {
            // Запрос для террейна
            _terrainQuery = GetEntityQuery(
                ComponentType.ReadWrite<TerrainData>(),
                ComponentType.ReadOnly<TerrainChunk>()
            );
            
            // Запрос для колес
            _wheelQuery = GetEntityQuery(
                ComponentType.ReadOnly<WheelData>(),
                ComponentType.ReadOnly<LocalTransform>()
            );
        }
        
        protected override void OnUpdate()
        {
            var terrainEntities = _terrainQuery.ToEntityArray(Allocator.TempJob);
            var wheelEntities = _wheelQuery.ToEntityArray(Allocator.TempJob);
            
            if (terrainEntities.Length == 0 || wheelEntities.Length == 0)
            {
                terrainEntities.Dispose();
                wheelEntities.Dispose();
                return;
            }
            
            // Создание Job для деформации террейна
            var deformationJob = new TerrainDeformationJob
            {
                TerrainEntities = terrainEntities,
                WheelEntities = wheelEntities,
                TerrainDataLookup = GetComponentLookup<TerrainData>(),
                WheelDataLookup = GetComponentLookup<WheelData>(),
                TransformLookup = GetComponentLookup<LocalTransform>(),
                DeltaTime = SystemAPI.Time.fixedDeltaTime
            };
            
            // Запуск Job с зависимостями
            var jobHandle = deformationJob.ScheduleParallel(
                terrainEntities.Length,
                SystemConstants.TERRAIN_DEFAULT_RESOLUTION / 64, // Оптимальный batch size
                Dependency
            );
            
            Dependency = jobHandle;
            
            // Освобождение временных массивов
            terrainEntities.Dispose();
            wheelEntities.Dispose();
        }
    }
    
    /// <summary>
    /// Job для параллельной деформации террейна
    /// Оптимизирован для максимальной производительности
    /// </summary>
    [BurstCompile]
    public struct TerrainDeformationJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<Entity> TerrainEntities;
        [ReadOnly] public NativeArray<Entity> WheelEntities;
        
        public ComponentLookup<TerrainData> TerrainDataLookup;
        [ReadOnly] public ComponentLookup<WheelData> WheelDataLookup;
        [ReadOnly] public ComponentLookup<LocalTransform> TransformLookup;
        
        public float DeltaTime;
        
        public void Execute(int index)
        {
            if (index >= TerrainEntities.Length) return;
            
            var terrainEntity = TerrainEntities[index];
            var terrainData = TerrainDataLookup[terrainEntity];
            
            // Применение деформации от всех колес
            for (int i = 0; i < WheelEntities.Length; i++)
            {
                var wheelEntity = WheelEntities[i];
                var wheelData = WheelDataLookup[wheelEntity];
                var wheelTransform = TransformLookup[wheelEntity];
                
                // Вычисление деформации от колеса
                var deformation = CalculateWheelDeformation(
                    wheelData,
                    wheelTransform,
                    terrainData,
                    DeltaTime
                );
                
                // Применение деформации к террейну
                ApplyDeformationToTerrain(ref terrainData, deformation);
            }
            
            // Обновление данных террейна
            TerrainDataLookup[terrainEntity] = terrainData;
        }
        
        /// <summary>
        /// Вычисление деформации от колеса
        /// </summary>
        private float CalculateWheelDeformation(
            WheelData wheelData,
            LocalTransform wheelTransform,
            TerrainData terrainData,
            float deltaTime)
        {
            // Вычисление давления колеса на террейн
            var wheelPressure = wheelData.Weight / (math.PI * wheelData.Radius * wheelData.Radius);
            
            // Вычисление деформации на основе давления и твердости террейна
            var deformation = wheelPressure * SystemConstants.TERRAIN_DEFAULT_DEFORMATION_RATE * deltaTime;
            
            // Ограничение деформации максимальными значениями
            deformation = math.clamp(
                deformation,
                SystemConstants.TERRAIN_DEFAULT_MIN_DEPTH,
                SystemConstants.TERRAIN_DEFAULT_MAX_DEPTH
            );
            
            return deformation;
        }
        
        /// <summary>
        /// Применение деформации к террейну
        /// </summary>
        private void ApplyDeformationToTerrain(ref TerrainData terrainData, float deformation)
        {
            // Обновление высотных данных
            terrainData.HeightMap = ApplyHeightDeformation(terrainData.HeightMap, deformation);
            
            // Обновление твердости террейна
            terrainData.Hardness = math.max(
                terrainData.Hardness - deformation * 0.1f,
                SystemConstants.TERRAIN_DEFAULT_HARDNESS * 0.1f
            );
        }
        
        /// <summary>
        /// Применение деформации к высотной карте
        /// </summary>
        private NativeArray<float> ApplyHeightDeformation(NativeArray<float> heightMap, float deformation)
        {
            // Простое применение деформации (можно оптимизировать дальше)
            for (int i = 0; i < heightMap.Length; i++)
            {
                heightMap[i] += deformation;
            }
            
            return heightMap;
        }
    }
    
    /// <summary>
    /// Компонент данных террейна
    /// </summary>
    public struct TerrainData : IComponentData
    {
        public NativeArray<float> HeightMap;
        public float Hardness;
        public float DeformationRate;
        public float RecoveryRate;
    }
    
    /// <summary>
    /// Компонент данных колеса
    /// </summary>
    public struct WheelData : IComponentData
    {
        public float Weight;
        public float Radius;
        public float Width;
        public float Pressure;
    }
}
