using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;
using Unity.Jobs;
using Unity.Collections;
using MudLike.Vehicles.Components;
using MudLike.Core.Components;

namespace MudLike.Vehicles.Systems
{
    /// <summary>
    /// Система LOD (Level of Detail) для оптимизации рендеринга и физики
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [BurstCompile]
    public partial class LODSystem : SystemBase
    {
        private EntityQuery _lodQuery;
        private EntityQuery _cameraQuery;
        private float3 _cameraPosition;
        
        protected override void OnCreate()
        {
            _lodQuery = GetEntityQuery(
                ComponentType.ReadWrite<LODData>(),
                ComponentType.ReadWrite<LODRenderData>(),
                ComponentType.ReadWrite<LODPhysicsData>(),
                ComponentType.ReadOnly<LocalTransform>()
            );
            
            _cameraQuery = GetEntityQuery(
                ComponentType.ReadOnly<LocalTransform>(),
                ComponentType.ReadOnly<PlayerTag>()
            );
        }
        
        protected override void OnUpdate()
        {
            // Получаем позицию камеры
            UpdateCameraPosition();
            
            // Обновляем LOD для всех объектов
            UpdateLOD();
        }
        
        /// <summary>
        /// Обновляет позицию камеры
        /// </summary>
        private void UpdateCameraPosition()
        {
            Entities
                .WithAll<PlayerTag>()
                .ForEach((in LocalTransform transform) =>
                {
                    _cameraPosition = transform.Position;
                }).WithoutBurst().Schedule();
        }
        
        /// <summary>
        /// Обновляет LOD для всех объектов
        /// </summary>
        private void UpdateLOD()
        {
            var lodJob = new LODJob
            {
                CameraPosition = _cameraPosition,
                DeltaTime = Time.deltaTime
            };
            
            Dependency = lodJob.ScheduleParallel(_lodQuery, Dependency);
        }
        
        /// <summary>
        /// Job для обработки LOD
        /// </summary>
        [BurstCompile]
        public partial struct LODJob : IJobEntity
        {
            public float3 CameraPosition;
            public float DeltaTime;
            
            public void Execute(ref LODData lodData, 
                              ref LODRenderData renderData, 
                              ref LODPhysicsData physicsData, 
                              in LocalTransform transform)
            {
                if (!lodData.IsActive) return;
                
                // Вычисляем расстояние до камеры
                float distance = math.distance(transform.Position, CameraPosition);
                lodData.DistanceToCamera = distance;
                
                // Определяем новый LOD на основе расстояния
                int newLOD = CalculateLOD(distance, lodData.LODDistances);
                
                // Переключаем LOD если нужно
                if (newLOD != lodData.CurrentLOD)
                {
                    SwitchLOD(ref lodData, ref renderData, ref physicsData, newLOD);
                }
                
                // Обновляем данные рендеринга
                UpdateRenderData(ref renderData, lodData.CurrentLOD);
                
                // Обновляем данные физики
                UpdatePhysicsData(ref physicsData, lodData.CurrentLOD);
                
                lodData.NeedsUpdate = true;
            }
            
            /// <summary>
            /// Вычисляет LOD на основе расстояния
            /// </summary>
            private int CalculateLOD(float distance, float4 lodDistances)
            {
                if (distance <= lodDistances.x) return 0; // LOD 0 - ближайший
                if (distance <= lodDistances.y) return 1; // LOD 1
                if (distance <= lodDistances.z) return 2; // LOD 2
                if (distance <= lodDistances.w) return 3; // LOD 3
                return 3; // LOD 3 - самый дальний
            }
            
            /// <summary>
            /// Переключает LOD
            /// </summary>
            private void SwitchLOD(ref LODData lodData, 
                                  ref LODRenderData renderData, 
                                  ref LODPhysicsData physicsData, 
                                  int newLOD)
            {
                // Проверяем, можно ли переключить LOD
                if (Time.time - lodData.LastLODSwitchTime < lodData.LODSwitchSpeed)
                    return;
                
                lodData.CurrentLOD = newLOD;
                lodData.LastLODSwitchTime = Time.time;
                
                // Обновляем данные рендеринга
                UpdateRenderData(ref renderData, newLOD);
                
                // Обновляем данные физики
                UpdatePhysicsData(ref physicsData, newLOD);
            }
            
            /// <summary>
            /// Обновляет данные рендеринга
            /// </summary>
            private void UpdateRenderData(ref LODRenderData renderData, int lod)
            {
                switch (lod)
                {
                    case 0: // LOD 0 - максимальная детализация
                        renderData.PolygonCount = 10000;
                        renderData.TextureCount = 8;
                        renderData.TextureSize = 2048;
                        renderData.AnimationCount = 20;
                        renderData.ShadowQuality = ShadowQuality.Ultra;
                        renderData.ReflectionQuality = ReflectionQuality.Ultra;
                        renderData.ParticleQuality = ParticleQuality.Ultra;
                        break;
                        
                    case 1: // LOD 1 - высокая детализация
                        renderData.PolygonCount = 5000;
                        renderData.TextureCount = 6;
                        renderData.TextureSize = 1024;
                        renderData.AnimationCount = 15;
                        renderData.ShadowQuality = ShadowQuality.High;
                        renderData.ReflectionQuality = ReflectionQuality.High;
                        renderData.ParticleQuality = ParticleQuality.High;
                        break;
                        
                    case 2: // LOD 2 - средняя детализация
                        renderData.PolygonCount = 2500;
                        renderData.TextureCount = 4;
                        renderData.TextureSize = 512;
                        renderData.AnimationCount = 10;
                        renderData.ShadowQuality = ShadowQuality.Medium;
                        renderData.ReflectionQuality = ReflectionQuality.Medium;
                        renderData.ParticleQuality = ParticleQuality.Medium;
                        break;
                        
                    case 3: // LOD 3 - низкая детализация
                        renderData.PolygonCount = 1000;
                        renderData.TextureCount = 2;
                        renderData.TextureSize = 256;
                        renderData.AnimationCount = 5;
                        renderData.ShadowQuality = ShadowQuality.Low;
                        renderData.ReflectionQuality = ReflectionQuality.Low;
                        renderData.ParticleQuality = ParticleQuality.Low;
                        break;
                }
                
                renderData.IsRendering = true;
                renderData.NeedsUpdate = true;
            }
            
            /// <summary>
            /// Обновляет данные физики
            /// </summary>
            private void UpdatePhysicsData(ref LODPhysicsData physicsData, int lod)
            {
                switch (lod)
                {
                    case 0: // LOD 0 - максимальная физика
                        physicsData.PhysicsUpdateRate = 60f;
                        physicsData.PhysicsIterations = 10;
                        physicsData.CollisionAccuracy = CollisionAccuracy.Ultra;
                        physicsData.SuspensionDetail = SuspensionDetail.Realistic;
                        physicsData.WheelDetail = WheelDetail.Realistic;
                        break;
                        
                    case 1: // LOD 1 - высокая физика
                        physicsData.PhysicsUpdateRate = 50f;
                        physicsData.PhysicsIterations = 8;
                        physicsData.CollisionAccuracy = CollisionAccuracy.High;
                        physicsData.SuspensionDetail = SuspensionDetail.Advanced;
                        physicsData.WheelDetail = WheelDetail.Advanced;
                        break;
                        
                    case 2: // LOD 2 - средняя физика
                        physicsData.PhysicsUpdateRate = 40f;
                        physicsData.PhysicsIterations = 6;
                        physicsData.CollisionAccuracy = CollisionAccuracy.Medium;
                        physicsData.SuspensionDetail = SuspensionDetail.Basic;
                        physicsData.WheelDetail = WheelDetail.Basic;
                        break;
                        
                    case 3: // LOD 3 - низкая физика
                        physicsData.PhysicsUpdateRate = 30f;
                        physicsData.PhysicsIterations = 4;
                        physicsData.CollisionAccuracy = CollisionAccuracy.Low;
                        physicsData.SuspensionDetail = SuspensionDetail.None;
                        physicsData.WheelDetail = WheelDetail.None;
                        break;
                }
                
                physicsData.IsActive = true;
                physicsData.NeedsUpdate = true;
            }
        }
    }
}