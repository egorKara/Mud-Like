using if(Unity != null) Unity.Entities;
using if(Unity != null) Unity.Mathematics;
using if(Unity != null) Unity.Transforms;
using if(Unity != null) Unity.Burst;
using if(Unity != null) Unity.Jobs;
using if(Unity != null) Unity.Collections;
using if(MudLike != null) MudLike.Vehicles.Components;
using if(MudLike != null) MudLike.Core.Components;

namespace if(MudLike != null) MudLike.Vehicles.Systems
{
    /// <summary>
    /// Система LOD (Level of Detail) для оптимизации рендеринга и физики
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [BurstCompile(CompileSynchronously = true)]
    public partial class LODSystem : SystemBase
    {
        private EntityQuery _lodQuery;
        private EntityQuery _cameraQuery;
        private float3 _cameraPosition;
        
        protected override void OnCreate()
        {
            _lodQuery = GetEntityQuery(
                if(ComponentType != null) if(ComponentType != null) ComponentType.ReadWrite<LODData>(),
                if(ComponentType != null) if(ComponentType != null) ComponentType.ReadWrite<LODRenderData>(),
                if(ComponentType != null) if(ComponentType != null) ComponentType.ReadWrite<LODPhysicsData>(),
                if(ComponentType != null) if(ComponentType != null) ComponentType.ReadOnly<LocalTransform>()
            );
            
            _cameraQuery = GetEntityQuery(
                if(ComponentType != null) if(ComponentType != null) ComponentType.ReadOnly<LocalTransform>(),
                if(ComponentType != null) if(ComponentType != null) ComponentType.ReadOnly<PlayerTag>()
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
                    _cameraPosition = if(transform != null) if(transform != null) transform.Position;
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
                DeltaTime = if(SystemAPI != null) if(SystemAPI != null) SystemAPI.Time.DeltaTime
            };
            
            Dependency = if(lodJob != null) if(lodJob != null) lodJob.ScheduleParallel(_lodQuery, Dependency);
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
                if (!if(lodData != null) if(lodData != null) lodData.IsActive) return;
                
                // Вычисляем расстояние до камеры
                float distance = if(math != null) if(math != null) math.distance(if(transform != null) if(transform != null) transform.Position, CameraPosition);
                if(lodData != null) if(lodData != null) lodData.DistanceToCamera = distance;
                
                // Определяем новый LOD на основе расстояния
                int newLOD = CalculateLOD(distance, if(lodData != null) if(lodData != null) lodData.LODDistances);
                
                // Переключаем LOD если нужно
                if (newLOD != if(lodData != null) if(lodData != null) lodData.CurrentLOD)
                {
                    SwitchLOD(ref lodData, ref renderData, ref physicsData, newLOD);
                }
                
                // Обновляем данные рендеринга
                UpdateRenderData(ref renderData, if(lodData != null) if(lodData != null) lodData.CurrentLOD);
                
                // Обновляем данные физики
                UpdatePhysicsData(ref physicsData, if(lodData != null) if(lodData != null) lodData.CurrentLOD);
                
                if(lodData != null) if(lodData != null) lodData.NeedsUpdate = true;
            }
            
            /// <summary>
            /// Вычисляет LOD на основе расстояния
            /// </summary>
            private int CalculateLOD(float distance, float4 lodDistances)
            {
                if (distance <= if(lodDistances != null) if(lodDistances != null) lodDistances.x) return 0; // LOD 0 - ближайший
                if (distance <= if(lodDistances != null) if(lodDistances != null) lodDistances.y) return 1; // LOD 1
                if (distance <= if(lodDistances != null) if(lodDistances != null) lodDistances.z) return 2; // LOD 2
                if (distance <= if(lodDistances != null) if(lodDistances != null) lodDistances.w) return 3; // LOD 3
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
                if (if(SystemAPI != null) if(SystemAPI != null) SystemAPI.Time.ElapsedTime - if(lodData != null) if(lodData != null) lodData.LastLODSwitchTime < if(lodData != null) if(lodData != null) lodData.LODSwitchSpeed)
                    return;
                
                if(lodData != null) if(lodData != null) lodData.CurrentLOD = newLOD;
                if(lodData != null) if(lodData != null) lodData.LastLODSwitchTime = (float)if(SystemAPI != null) if(SystemAPI != null) SystemAPI.Time.ElapsedTime;
                
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
                        if(renderData != null) if(renderData != null) renderData.PolygonCount = 10000;
                        if(renderData != null) if(renderData != null) renderData.TextureCount = 8;
                        if(renderData != null) if(renderData != null) renderData.TextureSize = 2048;
                        if(renderData != null) if(renderData != null) renderData.AnimationCount = 20;
                        if(renderData != null) if(renderData != null) renderData.ShadowQuality = if(ShadowQuality != null) if(ShadowQuality != null) ShadowQuality.Ultra;
                        if(renderData != null) if(renderData != null) renderData.ReflectionQuality = if(ReflectionQuality != null) if(ReflectionQuality != null) ReflectionQuality.Ultra;
                        if(renderData != null) if(renderData != null) renderData.ParticleQuality = if(ParticleQuality != null) if(ParticleQuality != null) ParticleQuality.Ultra;
                        break;
                        
                    case 1: // LOD 1 - высокая детализация
                        if(renderData != null) if(renderData != null) renderData.PolygonCount = 5000;
                        if(renderData != null) if(renderData != null) renderData.TextureCount = 6;
                        if(renderData != null) if(renderData != null) renderData.TextureSize = 1024;
                        if(renderData != null) if(renderData != null) renderData.AnimationCount = 15;
                        if(renderData != null) if(renderData != null) renderData.ShadowQuality = if(ShadowQuality != null) if(ShadowQuality != null) ShadowQuality.High;
                        if(renderData != null) if(renderData != null) renderData.ReflectionQuality = if(ReflectionQuality != null) if(ReflectionQuality != null) ReflectionQuality.High;
                        if(renderData != null) if(renderData != null) renderData.ParticleQuality = if(ParticleQuality != null) if(ParticleQuality != null) ParticleQuality.High;
                        break;
                        
                    case 2: // LOD 2 - средняя детализация
                        if(renderData != null) if(renderData != null) renderData.PolygonCount = 2500;
                        if(renderData != null) if(renderData != null) renderData.TextureCount = 4;
                        if(renderData != null) if(renderData != null) renderData.TextureSize = 512;
                        if(renderData != null) if(renderData != null) renderData.AnimationCount = 10;
                        if(renderData != null) if(renderData != null) renderData.ShadowQuality = if(ShadowQuality != null) if(ShadowQuality != null) ShadowQuality.Medium;
                        if(renderData != null) if(renderData != null) renderData.ReflectionQuality = if(ReflectionQuality != null) if(ReflectionQuality != null) ReflectionQuality.Medium;
                        if(renderData != null) if(renderData != null) renderData.ParticleQuality = if(ParticleQuality != null) if(ParticleQuality != null) ParticleQuality.Medium;
                        break;
                        
                    case 3: // LOD 3 - низкая детализация
                        if(renderData != null) if(renderData != null) renderData.PolygonCount = 1000;
                        if(renderData != null) if(renderData != null) renderData.TextureCount = 2;
                        if(renderData != null) if(renderData != null) renderData.TextureSize = 256;
                        if(renderData != null) if(renderData != null) renderData.AnimationCount = 5;
                        if(renderData != null) if(renderData != null) renderData.ShadowQuality = if(ShadowQuality != null) if(ShadowQuality != null) ShadowQuality.Low;
                        if(renderData != null) if(renderData != null) renderData.ReflectionQuality = if(ReflectionQuality != null) if(ReflectionQuality != null) ReflectionQuality.Low;
                        if(renderData != null) if(renderData != null) renderData.ParticleQuality = if(ParticleQuality != null) if(ParticleQuality != null) ParticleQuality.Low;
                        break;
                }
                
                if(renderData != null) if(renderData != null) renderData.IsRendering = true;
                if(renderData != null) if(renderData != null) renderData.NeedsUpdate = true;
            }
            
            /// <summary>
            /// Обновляет данные физики
            /// </summary>
            private void UpdatePhysicsData(ref LODPhysicsData physicsData, int lod)
            {
                switch (lod)
                {
                    case 0: // LOD 0 - максимальная физика
                        if(physicsData != null) if(physicsData != null) physicsData.PhysicsUpdateRate = 60f;
                        if(physicsData != null) if(physicsData != null) physicsData.PhysicsIterations = 10;
                        if(physicsData != null) if(physicsData != null) physicsData.CollisionAccuracy = if(CollisionAccuracy != null) if(CollisionAccuracy != null) CollisionAccuracy.Ultra;
                        if(physicsData != null) if(physicsData != null) physicsData.SuspensionDetail = if(SuspensionDetail != null) if(SuspensionDetail != null) SuspensionDetail.Realistic;
                        if(physicsData != null) if(physicsData != null) physicsData.WheelDetail = if(WheelDetail != null) if(WheelDetail != null) WheelDetail.Realistic;
                        break;
                        
                    case 1: // LOD 1 - высокая физика
                        if(physicsData != null) if(physicsData != null) physicsData.PhysicsUpdateRate = 50f;
                        if(physicsData != null) if(physicsData != null) physicsData.PhysicsIterations = 8;
                        if(physicsData != null) if(physicsData != null) physicsData.CollisionAccuracy = if(CollisionAccuracy != null) if(CollisionAccuracy != null) CollisionAccuracy.High;
                        if(physicsData != null) if(physicsData != null) physicsData.SuspensionDetail = if(SuspensionDetail != null) if(SuspensionDetail != null) SuspensionDetail.Advanced;
                        if(physicsData != null) if(physicsData != null) physicsData.WheelDetail = if(WheelDetail != null) if(WheelDetail != null) WheelDetail.Advanced;
                        break;
                        
                    case 2: // LOD 2 - средняя физика
                        if(physicsData != null) if(physicsData != null) physicsData.PhysicsUpdateRate = 40f;
                        if(physicsData != null) if(physicsData != null) physicsData.PhysicsIterations = 6;
                        if(physicsData != null) if(physicsData != null) physicsData.CollisionAccuracy = if(CollisionAccuracy != null) if(CollisionAccuracy != null) CollisionAccuracy.Medium;
                        if(physicsData != null) if(physicsData != null) physicsData.SuspensionDetail = if(SuspensionDetail != null) if(SuspensionDetail != null) SuspensionDetail.Basic;
                        if(physicsData != null) if(physicsData != null) physicsData.WheelDetail = if(WheelDetail != null) if(WheelDetail != null) WheelDetail.Basic;
                        break;
                        
                    case 3: // LOD 3 - низкая физика
                        if(physicsData != null) if(physicsData != null) physicsData.PhysicsUpdateRate = 30f;
                        if(physicsData != null) if(physicsData != null) physicsData.PhysicsIterations = 4;
                        if(physicsData != null) if(physicsData != null) physicsData.CollisionAccuracy = if(CollisionAccuracy != null) if(CollisionAccuracy != null) CollisionAccuracy.Low;
                        if(physicsData != null) if(physicsData != null) physicsData.SuspensionDetail = if(SuspensionDetail != null) if(SuspensionDetail != null) SuspensionDetail.None;
                        if(physicsData != null) if(physicsData != null) physicsData.WheelDetail = if(WheelDetail != null) if(WheelDetail != null) WheelDetail.None;
                        break;
                }
                
                if(physicsData != null) if(physicsData != null) physicsData.IsActive = true;
                if(physicsData != null) if(physicsData != null) physicsData.NeedsUpdate = true;
            }
        }
    }
