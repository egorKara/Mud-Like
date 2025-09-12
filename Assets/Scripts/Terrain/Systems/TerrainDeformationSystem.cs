using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;
using MudLike.Terrain.Components;
using MudLike.Vehicles.Components;

namespace MudLike.Terrain.Systems
{
    /// <summary>
    /// Система деформации террейна
    /// </summary>
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile]
    public partial class TerrainDeformationSystem : SystemBase
    {
        private EntityQuery _terrainQuery;
        private EntityQuery _wheelQuery;
        
        protected override void OnCreate()
        {
            _terrainQuery = GetEntityQuery(
                ComponentType.ReadWrite<TerrainData>()
            );
            
            _wheelQuery = GetEntityQuery(
                ComponentType.ReadOnly<WheelData>(),
                ComponentType.ReadOnly<LocalTransform>()
            );
        }
        
        protected override void OnUpdate()
        {
            float deltaTime = SystemAPI.Time.fixedDeltaTime;
            
            // Обновляем деформацию террейна
            UpdateTerrainDeformation(deltaTime);
        }
        
        private void UpdateTerrainDeformation(float deltaTime)
        {
            // Получаем все колеса
            var wheelEntities = _wheelQuery.ToEntityArray(Unity.Collections.Allocator.Temp);
            var wheelData = _wheelQuery.ToComponentDataArray<WheelData>(Unity.Collections.Allocator.Temp);
            var wheelTransforms = _wheelQuery.ToComponentDataArray<LocalTransform>(Unity.Collections.Allocator.Temp);
            
            // Обновляем террейн для каждого колеса
            for (int i = 0; i < wheelEntities.Length; i++)
            {
                if (wheelData[i].IsGrounded)
                {
                    DeformTerrainAtPosition(wheelTransforms[i].Position, wheelData[i].SuspensionForce, deltaTime);
                }
            }
            
            // Освобождаем временные массивы
            wheelEntities.Dispose();
            wheelData.Dispose();
            wheelTransforms.Dispose();
        }
        
        private void DeformTerrainAtPosition(float3 position, float3 force, float deltaTime)
        {
            // Простая реализация деформации террейна
            // В реальной реализации здесь будет сложная физика деформации
            
            var terrainEntities = _terrainQuery.ToEntityArray(Unity.Collections.Allocator.Temp);
            
            for (int i = 0; i < terrainEntities.Length; i++)
            {
                var terrainData = EntityManager.GetComponentData<TerrainData>(terrainEntities[i]);
                
                // Проверяем, находится ли позиция в пределах террейна
                if (IsPositionInTerrain(position, terrainData))
                {
                    // Деформируем террейн
                    terrainData.NeedsUpdate = true;
                    terrainData.ColliderNeedsUpdate = true;
                    
                    EntityManager.SetComponentData(terrainEntities[i], terrainData);
                }
            }
            
            terrainEntities.Dispose();
        }
        
        private bool IsPositionInTerrain(float3 position, TerrainData terrainData)
        {
            // Простая проверка границ террейна
            return position.x >= 0 && position.x < terrainData.Width &&
                   position.z >= 0 && position.z < terrainData.Height;
        }
    }
}