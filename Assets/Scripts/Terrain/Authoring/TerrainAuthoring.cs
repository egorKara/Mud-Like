using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using MudLike.Terrain.Components;

namespace MudLike.Terrain.Authoring
{
    /// <summary>
    /// Авторинг компонент для создания террейна в ECS
    /// </summary>
    public class TerrainAuthoring : MonoBehaviour
    {
        [Header("Terrain Settings")]
        [SerializeField] private float blockSize = 16f;
        [SerializeField] private int2 heightResolution = new int2(64, 64);
        [SerializeField] private float minHeight = -2f;
        [SerializeField] private float maxHeight = 2f;
        
        [Header("Mud Settings")]
        [SerializeField] private float mudViscosity = 0.5f;
        [SerializeField] private float mudDensity = 1.2f;
        [SerializeField] private float mudTractionModifier = 0.3f;
        
        [Header("Deformation Settings")]
        [SerializeField] private float maxSinkDepth = 2f;
        [SerializeField] private float deformationRate = 0.1f;
        [SerializeField] private float recoveryRate = 0.05f;
        
        /// <summary>
        /// Bake компонент для создания ECS сущности
        /// </summary>
        private class TerrainBaker : Baker<TerrainAuthoring>
        {
            public override void Bake(TerrainAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                
                // Добавляем основные компоненты террейна
                AddComponent(entity, new TerrainBlockData
                {
                    GridPosition = int2.zero,
                    BlockSize = authoring.blockSize,
                    HeightResolution = authoring.heightResolution,
                    MinHeight = authoring.minHeight,
                    MaxHeight = authoring.maxHeight,
                    IsActive = true,
                    LastUpdateTime = 0f
                });
                
                AddComponent(entity, new MudData
                {
                    Height = 0f,
                    TractionModifier = authoring.mudTractionModifier,
                    Viscosity = authoring.mudViscosity,
                    Density = authoring.mudDensity,
                    Moisture = 0.8f,
                    LastUpdateTime = 0f,
                    IsDirty = false
                });
                
                // Создаем дополнительные блоки террейна
                CreateTerrainBlocks(entity, authoring);
            }
            
            /// <summary>
            /// Создает блоки террейна
            /// </summary>
            private void CreateTerrainBlocks(Entity terrainEntity, TerrainAuthoring authoring)
            {
                // Создаем сетку блоков 3x3
                for (int x = -1; x <= 1; x++)
                {
                    for (int z = -1; z <= 1; z++)
                    {
                        var blockEntity = CreateAdditionalEntity(TransformUsageFlags.Dynamic);
                        
                        // Добавляем компоненты блока
                        AddComponent(blockEntity, new TerrainBlockData
                        {
                            GridPosition = new int2(x, z),
                            BlockSize = authoring.blockSize,
                            HeightResolution = authoring.heightResolution,
                            MinHeight = authoring.minHeight,
                            MaxHeight = authoring.maxHeight,
                            IsActive = true,
                            LastUpdateTime = 0f
                        });
                        
                        AddComponent(blockEntity, new MudData
                        {
                            Height = 0f,
                            TractionModifier = authoring.mudTractionModifier,
                            Viscosity = authoring.mudViscosity,
                            Density = authoring.mudDensity,
                            Moisture = 0.8f,
                            LastUpdateTime = 0f,
                            IsDirty = false
                        });
                        
                        // Связываем блок с основным террейном
                        AddComponent(blockEntity, new Parent
                        {
                            Value = terrainEntity
                        });
                    }
                }
            }
        }
    }
}