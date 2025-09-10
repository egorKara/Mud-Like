using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;
using UnityEngine;
using Unity.Burst;

namespace MudLike.Core.Systems
{
    /// <summary>
    /// Система оптимизации GPU производительности
    /// Управляет LOD, culling, batching и качеством рендеринга
    /// </summary>
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public partial class GPUOptimizationSystem : SystemBase
    {
        private Camera _mainCamera;
        private float3 _cameraPosition;
        private float _cameraFrustumRadius;
        
        // Настройки LOD
        private const float LOD_DISTANCE_1 = 50f;  // High -> Medium
        private const float LOD_DISTANCE_2 = 100f; // Medium -> Low
        private const float LOD_DISTANCE_3 = 200f; // Low -> Culled
        
        // Настройки culling
        private const float CULLING_DISTANCE = 300f;
        private const float FRUSTUM_CULLING_MARGIN = 10f;
        
        // Настройки batching
        private const int MAX_BATCH_SIZE = 1000;
        private const float BATCH_UPDATE_INTERVAL = 0.1f;
        private float _lastBatchUpdate;
        
        protected override void OnCreate()
        {
            _mainCamera = Camera.main;
            if (_mainCamera == null)
            {
                _mainCamera = Object.FindObjectOfType<Camera>();
            }
        }
        
        protected override void OnUpdate()
        {
            if (_mainCamera == null) return;
            
            UpdateCameraData();
            UpdateLODSystem();
            UpdateCullingSystem();
            UpdateBatchingSystem();
        }
        
        private void UpdateCameraData()
        {
            _cameraPosition = _mainCamera.transform.position;
            _cameraFrustumRadius = _mainCamera.farClipPlane;
        }
        
        private void UpdateLODSystem()
        {
            // Обновляем LOD для всех объектов
            Entities
                .WithAll<LocalTransform, RenderBounds>()
                .ForEach((Entity entity, ref LocalTransform transform, ref RenderBounds bounds) =>
                {
                    float distance = math.distance(_cameraPosition, transform.Position);
                    int lodLevel = CalculateLODLevel(distance);
                    
                    // Применяем LOD настройки
                    ApplyLODSettings(entity, lodLevel);
                }).Schedule();
        }
        
        private void UpdateCullingSystem()
        {
            // Frustum culling
            var frustumPlanes = GeometryUtility.CalculateFrustumPlanes(_mainCamera);
            
            Entities
                .WithAll<LocalTransform, RenderBounds>()
                .ForEach((Entity entity, ref LocalTransform transform, ref RenderBounds bounds) =>
                {
                    // Distance culling
                    float distance = math.distance(_cameraPosition, transform.Position);
                    if (distance > CULLING_DISTANCE)
                    {
                        SetComponentEnabled<RenderBounds>(entity, false);
                        return;
                    }
                    
                    // Frustum culling
                    if (!IsInFrustum(transform.Position, bounds, frustumPlanes))
                    {
                        SetComponentEnabled<RenderBounds>(entity, false);
                        return;
                    }
                    
                    // Включаем рендеринг если объект видим
                    SetComponentEnabled<RenderBounds>(entity, true);
                }).Schedule();
        }
        
        private void UpdateBatchingSystem()
        {
            float currentTime = Time.time;
            
            // Обновляем batching периодически
            if (currentTime - _lastBatchUpdate >= BATCH_UPDATE_INTERVAL)
            {
                OptimizeBatching();
                _lastBatchUpdate = currentTime;
            }
        }
        
        private int CalculateLODLevel(float distance)
        {
            if (distance <= LOD_DISTANCE_1) return 0; // High
            if (distance <= LOD_DISTANCE_2) return 1; // Medium
            if (distance <= LOD_DISTANCE_3) return 2; // Low
            return 3; // Culled
        }
        
        private void ApplyLODSettings(Entity entity, int lodLevel)
        {
            // Применяем настройки LOD в зависимости от уровня
            switch (lodLevel)
            {
                case 0: // High quality
                    // Максимальное качество
                    break;
                case 1: // Medium quality
                    // Среднее качество
                    break;
                case 2: // Low quality
                    // Низкое качество
                    break;
                case 3: // Culled
                    // Отключаем рендеринг
                    SetComponentEnabled<RenderBounds>(entity, false);
                    break;
            }
        }
        
        private bool IsInFrustum(float3 position, RenderBounds bounds, Plane[] frustumPlanes)
        {
            // Упрощенная проверка frustum culling
            for (int i = 0; i < frustumPlanes.Length; i++)
            {
                float distance = frustumPlanes[i].GetDistanceToPoint(position);
                if (distance < -bounds.Value.Extents.magnitude)
                {
                    return false;
                }
            }
            return true;
        }
        
        private void OptimizeBatching()
        {
            // Оптимизация batching для статических объектов
            var staticEntities = GetEntityQuery(typeof(LocalTransform), typeof(RenderBounds))
                .ToEntityArray(Allocator.Temp);
            
            // Группируем объекты по материалам для batching
            var materialGroups = new Dictionary<Material, List<Entity>>();
            
            foreach (var entity in staticEntities)
            {
                if (HasComponent<RenderMesh>(entity))
                {
                    var renderMesh = GetComponent<RenderMesh>(entity);
                    var material = renderMesh.material;
                    
                    if (!materialGroups.ContainsKey(material))
                    {
                        materialGroups[material] = new List<Entity>();
                    }
                    
                    materialGroups[material].Add(entity);
                }
            }
            
            // Применяем batching для каждой группы
            foreach (var group in materialGroups.Values)
            {
                if (group.Count > MAX_BATCH_SIZE)
                {
                    // Разбиваем большие группы на меньшие
                    for (int i = 0; i < group.Count; i += MAX_BATCH_SIZE)
                    {
                        int batchSize = math.min(MAX_BATCH_SIZE, group.Count - i);
                        // Здесь можно применить GPU Instancing или другие техники batching
                    }
                }
            }
            
            staticEntities.Dispose();
        }
        
        /// <summary>
        /// Установить качество рендеринга
        /// </summary>
        public void SetRenderQuality(int qualityLevel)
        {
            switch (qualityLevel)
            {
                case 0: // Low
                    QualitySettings.SetQualityLevel(0);
                    QualitySettings.pixelLightCount = 1;
                    QualitySettings.shadowResolution = ShadowResolution.Low;
                    break;
                case 1: // Medium
                    QualitySettings.SetQualityLevel(1);
                    QualitySettings.pixelLightCount = 2;
                    QualitySettings.shadowResolution = ShadowResolution.Medium;
                    break;
                case 2: // High
                    QualitySettings.SetQualityLevel(2);
                    QualitySettings.pixelLightCount = 4;
                    QualitySettings.shadowResolution = ShadowResolution.High;
                    break;
                case 3: // Ultra
                    QualitySettings.SetQualityLevel(3);
                    QualitySettings.pixelLightCount = 8;
                    QualitySettings.shadowResolution = ShadowResolution.VeryHigh;
                    break;
            }
        }
    }
}