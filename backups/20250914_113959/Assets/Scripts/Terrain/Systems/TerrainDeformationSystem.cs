using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Jobs;
using Unity.Burst;
using MudLike.Terrain.Components;
using MudLike.Vehicles.Components;

namespace MudLike.Terrain.Systems
{
    /// <summary>
    /// Система деформации террейна
    /// </summary>
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile(CompileSynchronously = true)]
    public partial class TerrainDeformationSystem : SystemBase
    {
        /// <summary>
        /// Обрабатывает деформацию террейна
        /// </summary>
        protected override void OnUpdate()
        {
            float deltaTime = SystemAPI.Time.fixedDeltaTime;
            
            // Обрабатываем деформации от колес
            ProcessWheelDeformations(deltaTime);
            
            // Обрабатываем деформации от других источников
            ProcessOtherDeformations(deltaTime);
            
            // Обновляем чанки террейна
            UpdateTerrainChunks(deltaTime);
        }
        
        /// <summary>
        /// Обрабатывает деформации от колес
        /// </summary>
        private void ProcessWheelDeformations(float deltaTime)
        {
            Entities
                .WithAll<WheelData>()
                .ForEach((in WheelData wheel, in LocalTransform wheelTransform) =>
                {
                    if (wheel.IsGrounded)
                    {
                        CreateWheelDeformation(wheelTransform.Position, wheel.Radius, wheel.SuspensionForce);
                    }
                }).Schedule();
        }
        
        /// <summary>
        /// Обрабатывает деформации от других источников
        /// </summary>
        private void ProcessOtherDeformations(float deltaTime)
        {
            Entities
                .WithAll<DeformationData>()
                .ForEach((ref DeformationData deformation) =>
                {
                    if (deformation.IsActive && !deformation.IsApplied)
                    {
                        ApplyDeformation(deformation);
                        deformation.IsApplied = true;
                    }
                }).Schedule();
        }
        
        /// <summary>
        /// Обновляет чанки террейна
        /// </summary>
        private void UpdateTerrainChunks(float deltaTime)
        {
            Entities
                .WithAll<TerrainChunk>()
                .ForEach((ref TerrainChunk chunk) =>
                {
                    if (chunk.IsDirty)
                    {
                        UpdateChunkHeights(ref chunk);
                        chunk.IsDirty = false;
                        chunk.NeedsSync = true;
                    }
                }).Schedule();
        }
        
        /// <summary>
        /// Создает деформацию от колеса
        /// </summary>
        private void CreateWheelDeformation(float3 position, float radius, float3 force)
        {
            // Вычисляем силу деформации
            float deformationForce = math.length(force);
            float deformationDepth = deformationForce * 0.001f; // Масштабируем силу
            
            // Создаем деформацию
            var deformation = new DeformationData
            {
                Position = position,
                Radius = radius,
                Depth = deformationDepth,
                Force = deformationForce,
                Type = DeformationType.Indentation,
                Time = 0f,
                IsActive = true,
                IsApplied = false
            };
            
            // Применяем деформацию
            ApplyDeformation(deformation);
        }
        
        /// <summary>
        /// Применяет деформацию к террейну
        /// </summary>
        [BurstCompile]
        private void ApplyDeformation(in DeformationData deformation)
        {
            // Находим затронутые чанки
            var affectedChunks = GetAffectedChunks(deformation.Position, deformation.Radius);
            
            // Применяем деформацию к каждому чанку
            foreach (var chunkIndex in affectedChunks)
            {
                ApplyDeformationToChunk(chunkIndex, deformation);
            }
        }
        
        /// <summary>
        /// Получает затронутые деформацией чанки
        /// </summary>
        private NativeList<int> GetAffectedChunks(float3 position, float radius)
        {
            var affectedChunks = new NativeList<int>(Allocator.Temp);
            
            // Получаем данные террейна
            var terrainData = GetSingleton<TerrainData>();
            
            // Вычисляем границы деформации
            float3 minBounds = position - radius;
            float3 maxBounds = position + radius;
            
            // Находим затронутые чанки
            for (int x = 0; x < terrainData.ChunkCountX; x++)
            {
                for (int z = 0; z < terrainData.ChunkCountZ; z++)
                {
                    float3 chunkPosition = new float3(x * terrainData.ChunkSize, 0, z * terrainData.ChunkSize);
                    
                    if (IsChunkAffected(chunkPosition, terrainData.ChunkSize, minBounds, maxBounds))
                    {
                        int chunkIndex = x * terrainData.ChunkCountZ + z;
                        affectedChunks.Add(chunkIndex);
                    }
                }
            }
            
            return affectedChunks;
        }
        
        /// <summary>
        /// Проверяет, затронут ли чанк деформацией
        /// </summary>
        private static bool IsChunkAffected(float3 chunkPosition, float chunkSize, float3 minBounds, float3 maxBounds)
        {
            float3 chunkMin = chunkPosition;
            float3 chunkMax = chunkPosition + chunkSize;
            
            return !(chunkMax.x < minBounds.x || chunkMin.x > maxBounds.x ||
                     chunkMax.z < minBounds.z || chunkMin.z > maxBounds.z);
        }
        
        /// <summary>
        /// Применяет деформацию к чанку
        /// </summary>
        [BurstCompile]
        private void ApplyDeformationToChunk(int chunkIndex, in DeformationData deformation)
        {
            // Получаем данные террейна
            var terrainData = GetSingleton<TerrainData>();
            
            // Вычисляем размеры чанка
            int chunkSize = terrainData.ChunkSize;
            int chunkX = chunkIndex / terrainData.ChunkCountZ;
            int chunkZ = chunkIndex % terrainData.ChunkCountZ;
            
            // Вычисляем границы деформации в локальных координатах чанка
            float3 chunkWorldPos = new float3(chunkX * chunkSize, 0, chunkZ * chunkSize);
            float3 localDeformationPos = deformation.Position - chunkWorldPos;
            
            // Применяем деформацию к высотам в чанке
            for (int x = 0; x < chunkSize; x++)
            {
                for (int z = 0; z < chunkSize; z++)
                {
                    float3 localPos = new float3(x, 0, z);
                    float distance = math.distance(localPos.xz, localDeformationPos.xz);
                    
                    // Вычисляем влияние деформации
                    if (distance <= deformation.Radius)
                    {
                        float influence = 1f - (distance / deformation.Radius);
                        float heightDeformation = deformation.Depth * influence * influence;
                        
                        // Применяем деформацию к высоте
                        ApplyHeightDeformation(chunkIndex, x, z, heightDeformation);
                        
                        // Обновляем уровень грязи
                        UpdateMudLevel(chunkIndex, x, z, deformation.Force * influence);
                    }
                }
            }
        }
        
        /// <summary>
        /// Обновляет высоты чанка
        /// </summary>
        [BurstCompile]
        private void UpdateChunkHeights(ref TerrainChunk chunk)
        {
            // Пересчитываем нормали для плавности
            RecalculateNormals(ref chunk);
            
            // Обновляем физические коллайдеры
            UpdatePhysicsColliders(chunk);
            
            // Синхронизируем с Unity Terrain
            SyncWithUnityTerrain(chunk);
        }
        
        /// <summary>
        /// Применяет деформацию к высоте в конкретной точке
        /// </summary>
        [BurstCompile]
        private void ApplyHeightDeformation(int chunkIndex, int x, int z, float deformation)
        {
            // Получаем текущую высоту
            float currentHeight = GetChunkHeight(chunkIndex, x, z);
            
            // Применяем деформацию
            float newHeight = currentHeight - deformation;
            
            // Ограничиваем минимальную высоту
            newHeight = math.max(newHeight, 0f);
            
            // Сохраняем новую высоту
            SetChunkHeight(chunkIndex, x, z, newHeight);
        }
        
        /// <summary>
        /// Обновляет уровень грязи в точке
        /// </summary>
        [BurstCompile]
        private void UpdateMudLevel(int chunkIndex, int x, int z, float force)
        {
            float currentMud = GetChunkMudLevel(chunkIndex, x, z);
            float mudIncrease = force * 0.001f; // Масштабируем силу
            float newMud = math.min(currentMud + mudIncrease, 1f);
            
            SetChunkMudLevel(chunkIndex, x, z, newMud);
        }
    }
}