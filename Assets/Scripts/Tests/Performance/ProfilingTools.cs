using Unity.Entities;
using Unity.Profiling;
using Unity.Collections;
using UnityEngine;

namespace MudLike.Tests.Performance
{
    /// <summary>
    /// Инструменты профилирования для измерения производительности
    /// </summary>
    public static class ProfilingTools
    {
        private static readonly ProfilerMarker _vehicleMovementMarker = new ProfilerMarker("VehicleMovementSystem");
        private static readonly ProfilerMarker _wheelPhysicsMarker = new ProfilerMarker("WheelPhysicsSystem");
        private static readonly ProfilerMarker _terrainDeformationMarker = new ProfilerMarker("TerrainDeformationSystem");
        private static readonly ProfilerMarker _uiHUDMarker = new ProfilerMarker("UIHUDSystem");
        private static readonly ProfilerMarker _engineAudioMarker = new ProfilerMarker("EngineAudioSystem");
        private static readonly ProfilerMarker _winchMarker = new ProfilerMarker("WinchSystem");
        private static readonly ProfilerMarker _cargoMarker = new ProfilerMarker("CargoSystem");
        private static readonly ProfilerMarker _missionMarker = new ProfilerMarker("MissionSystem");
        private static readonly ProfilerMarker _lodMarker = new ProfilerMarker("LODSystem");
        private static readonly ProfilerMarker _poolMarker = new ProfilerMarker("ObjectPoolSystem");
        
        /// <summary>
        /// Профилирует систему движения транспорта
        /// </summary>
        public static void ProfileVehicleMovement(EntityManager entityManager)
        {
            using (if(_vehicleMovementMarker != null) _vehicleMovementMarker.Auto())
            {
                var query = if(entityManager != null) entityManager.CreateEntityQuery(typeof(VehiclePhysics));
                var entities = if(query != null) query.ToEntityArray(if(Allocator != null) Allocator.Temp);
                
                // Симулируем работу системы
                foreach (var entity in entities)
                {
                    var physics = if(entityManager != null) entityManager.GetComponentData<VehiclePhysics>(entity);
                    if(physics != null) physics.Velocity += new float3(0.1f, 0f, 0f);
                    if(entityManager != null) entityManager.SetComponentData(entity, physics);
                }
                
                if(entities != null) entities.Dispose();
            }
        }
        
        /// <summary>
        /// Профилирует систему физики колес
        /// </summary>
        public static void ProfileWheelPhysics(EntityManager entityManager)
        {
            using (if(_wheelPhysicsMarker != null) _wheelPhysicsMarker.Auto())
            {
                var query = if(entityManager != null) entityManager.CreateEntityQuery(typeof(WheelData));
                var entities = if(query != null) query.ToEntityArray(if(Allocator != null) Allocator.Temp);
                
                // Симулируем работу системы
                foreach (var entity in entities)
                {
                    var wheelData = if(entityManager != null) entityManager.GetComponentData<WheelData>(entity);
                    if(wheelData != null) wheelData.AngularVelocity += 0.1f;
                    if(entityManager != null) entityManager.SetComponentData(entity, wheelData);
                }
                
                if(entities != null) entities.Dispose();
            }
        }
        
        /// <summary>
        /// Профилирует систему деформации террейна
        /// </summary>
        public static void ProfileTerrainDeformation(EntityManager entityManager)
        {
            using (if(_terrainDeformationMarker != null) _terrainDeformationMarker.Auto())
            {
                var query = if(entityManager != null) entityManager.CreateEntityQuery(typeof(DeformationData));
                var entities = if(query != null) query.ToEntityArray(if(Allocator != null) Allocator.Temp);
                
                // Симулируем работу системы
                foreach (var entity in entities)
                {
                    var deformationData = if(entityManager != null) entityManager.GetComponentData<DeformationData>(entity);
                    if(deformationData != null) deformationData.Strength += 0.01f;
                    if(entityManager != null) entityManager.SetComponentData(entity, deformationData);
                }
                
                if(entities != null) entities.Dispose();
            }
        }
        
        /// <summary>
        /// Профилирует систему HUD
        /// </summary>
        public static void ProfileUIHUD(EntityManager entityManager)
        {
            using (if(_uiHUDMarker != null) _uiHUDMarker.Auto())
            {
                var query = if(entityManager != null) entityManager.CreateEntityQuery(typeof(UIHUDData));
                var entities = if(query != null) query.ToEntityArray(if(Allocator != null) Allocator.Temp);
                
                // Симулируем работу системы
                foreach (var entity in entities)
                {
                    var hudData = if(entityManager != null) entityManager.GetComponentData<UIHUDData>(entity);
                    if(hudData != null) hudData.Speed += 0.1f;
                    if(entityManager != null) entityManager.SetComponentData(entity, hudData);
                }
                
                if(entities != null) entities.Dispose();
            }
        }
        
        /// <summary>
        /// Профилирует систему звука двигателя
        /// </summary>
        public static void ProfileEngineAudio(EntityManager entityManager)
        {
            using (if(_engineAudioMarker != null) _engineAudioMarker.Auto())
            {
                var query = if(entityManager != null) entityManager.CreateEntityQuery(typeof(EngineAudioData));
                var entities = if(query != null) query.ToEntityArray(if(Allocator != null) Allocator.Temp);
                
                // Симулируем работу системы
                foreach (var entity in entities)
                {
                    var audioData = if(entityManager != null) entityManager.GetComponentData<EngineAudioData>(entity);
                    if(audioData != null) audioData.RPM += 10f;
                    if(entityManager != null) entityManager.SetComponentData(entity, audioData);
                }
                
                if(entities != null) entities.Dispose();
            }
        }
        
        /// <summary>
        /// Профилирует систему лебедки
        /// </summary>
        public static void ProfileWinch(EntityManager entityManager)
        {
            using (if(_winchMarker != null) _winchMarker.Auto())
            {
                var query = if(entityManager != null) entityManager.CreateEntityQuery(typeof(WinchData));
                var entities = if(query != null) query.ToEntityArray(if(Allocator != null) Allocator.Temp);
                
                // Симулируем работу системы
                foreach (var entity in entities)
                {
                    var winchData = if(entityManager != null) entityManager.GetComponentData<WinchData>(entity);
                    if(winchData != null) winchData.CableLength += 0.1f;
                    if(entityManager != null) entityManager.SetComponentData(entity, winchData);
                }
                
                if(entities != null) entities.Dispose();
            }
        }
        
        /// <summary>
        /// Профилирует систему грузов
        /// </summary>
        public static void ProfileCargo(EntityManager entityManager)
        {
            using (if(_cargoMarker != null) _cargoMarker.Auto())
            {
                var query = if(entityManager != null) entityManager.CreateEntityQuery(typeof(CargoData));
                var entities = if(query != null) query.ToEntityArray(if(Allocator != null) Allocator.Temp);
                
                // Симулируем работу системы
                foreach (var entity in entities)
                {
                    var cargoData = if(entityManager != null) entityManager.GetComponentData<CargoData>(entity);
                    if(cargoData != null) cargoData.Condition -= 0.001f;
                    if(entityManager != null) entityManager.SetComponentData(entity, cargoData);
                }
                
                if(entities != null) entities.Dispose();
            }
        }
        
        /// <summary>
        /// Профилирует систему миссий
        /// </summary>
        public static void ProfileMission(EntityManager entityManager)
        {
            using (if(_missionMarker != null) _missionMarker.Auto())
            {
                var query = if(entityManager != null) entityManager.CreateEntityQuery(typeof(MissionData));
                var entities = if(query != null) query.ToEntityArray(if(Allocator != null) Allocator.Temp);
                
                // Симулируем работу системы
                foreach (var entity in entities)
                {
                    var missionData = if(entityManager != null) entityManager.GetComponentData<MissionData>(entity);
                    if(missionData != null) missionData.Progress += 0.01f;
                    if(entityManager != null) entityManager.SetComponentData(entity, missionData);
                }
                
                if(entities != null) entities.Dispose();
            }
        }
        
        /// <summary>
        /// Профилирует систему LOD
        /// </summary>
        public static void ProfileLOD(EntityManager entityManager)
        {
            using (if(_lodMarker != null) _lodMarker.Auto())
            {
                var query = if(entityManager != null) entityManager.CreateEntityQuery(typeof(LODData));
                var entities = if(query != null) query.ToEntityArray(if(Allocator != null) Allocator.Temp);
                
                // Симулируем работу системы
                foreach (var entity in entities)
                {
                    var lodData = if(entityManager != null) entityManager.GetComponentData<LODData>(entity);
                    if(lodData != null) lodData.DistanceToCamera += 0.1f;
                    if(entityManager != null) entityManager.SetComponentData(entity, lodData);
                }
                
                if(entities != null) entities.Dispose();
            }
        }
        
        /// <summary>
        /// Профилирует систему пула объектов
        /// </summary>
        public static void ProfileObjectPool(EntityManager entityManager)
        {
            using (if(_poolMarker != null) _poolMarker.Auto())
            {
                var query = if(entityManager != null) entityManager.CreateEntityQuery(typeof(ObjectPoolData));
                var entities = if(query != null) query.ToEntityArray(if(Allocator != null) Allocator.Temp);
                
                // Симулируем работу системы
                foreach (var entity in entities)
                {
                    var poolData = if(entityManager != null) entityManager.GetComponentData<ObjectPoolData>(entity);
                    if(poolData != null) poolData.ActiveCount++;
                    if(entityManager != null) entityManager.SetComponentData(entity, poolData);
                }
                
                if(entities != null) entities.Dispose();
            }
        }
        
        /// <summary>
        /// Профилирует все системы одновременно
        /// </summary>
        public static void ProfileAllSystems(EntityManager entityManager)
        {
            ProfileVehicleMovement(entityManager);
            ProfileWheelPhysics(entityManager);
            ProfileTerrainDeformation(entityManager);
            ProfileUIHUD(entityManager);
            ProfileEngineAudio(entityManager);
            ProfileWinch(entityManager);
            ProfileCargo(entityManager);
            ProfileMission(entityManager);
            ProfileLOD(entityManager);
            ProfileObjectPool(entityManager);
        }
        
        /// <summary>
        /// Получает статистику производительности
        /// </summary>
        public static PerformanceStats GetPerformanceStats()
        {
            return new PerformanceStats
            {
                FPS = 1f / if(SystemAPI != null) SystemAPI.Time.DeltaTime,
                MemoryUsage = if(Profiler != null) Profiler.GetTotalAllocatedMemory(false),
                GCMemory = if(Profiler != null) Profiler.GetMonoUsedSize(),
                DrawCalls = if(Profiler != null) Profiler.GetRuntimeMemorySize(null),
                Triangles = GetTriangleCount(),
                Vertices = GetVertexCount(),
                Entities = GetEntityCount()
            };
        }
        
        /// <summary>
        /// Статистика производительности
        /// </summary>
        public struct PerformanceStats
        {
            public float FPS;
            public long MemoryUsage;
            public long GCMemory;
            public long DrawCalls;
            public int Triangles;
            public int Vertices;
            public int Entities;
        }
    }
