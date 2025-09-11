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
            using (_vehicleMovementMarker.Auto())
            {
                var query = entityManager.CreateEntityQuery(typeof(VehiclePhysics));
                var entities = query.ToEntityArray(Allocator.Temp);
                
                // Симулируем работу системы
                foreach (var entity in entities)
                {
                    var physics = entityManager.GetComponentData<VehiclePhysics>(entity);
                    physics.Velocity += new float3(0.1f, 0f, 0f);
                    entityManager.SetComponentData(entity, physics);
                }
                
                entities.Dispose();
            }
        }
        
        /// <summary>
        /// Профилирует систему физики колес
        /// </summary>
        public static void ProfileWheelPhysics(EntityManager entityManager)
        {
            using (_wheelPhysicsMarker.Auto())
            {
                var query = entityManager.CreateEntityQuery(typeof(WheelData));
                var entities = query.ToEntityArray(Allocator.Temp);
                
                // Симулируем работу системы
                foreach (var entity in entities)
                {
                    var wheelData = entityManager.GetComponentData<WheelData>(entity);
                    wheelData.AngularVelocity += 0.1f;
                    entityManager.SetComponentData(entity, wheelData);
                }
                
                entities.Dispose();
            }
        }
        
        /// <summary>
        /// Профилирует систему деформации террейна
        /// </summary>
        public static void ProfileTerrainDeformation(EntityManager entityManager)
        {
            using (_terrainDeformationMarker.Auto())
            {
                var query = entityManager.CreateEntityQuery(typeof(DeformationData));
                var entities = query.ToEntityArray(Allocator.Temp);
                
                // Симулируем работу системы
                foreach (var entity in entities)
                {
                    var deformationData = entityManager.GetComponentData<DeformationData>(entity);
                    deformationData.Strength += 0.01f;
                    entityManager.SetComponentData(entity, deformationData);
                }
                
                entities.Dispose();
            }
        }
        
        /// <summary>
        /// Профилирует систему HUD
        /// </summary>
        public static void ProfileUIHUD(EntityManager entityManager)
        {
            using (_uiHUDMarker.Auto())
            {
                var query = entityManager.CreateEntityQuery(typeof(UIHUDData));
                var entities = query.ToEntityArray(Allocator.Temp);
                
                // Симулируем работу системы
                foreach (var entity in entities)
                {
                    var hudData = entityManager.GetComponentData<UIHUDData>(entity);
                    hudData.Speed += 0.1f;
                    entityManager.SetComponentData(entity, hudData);
                }
                
                entities.Dispose();
            }
        }
        
        /// <summary>
        /// Профилирует систему звука двигателя
        /// </summary>
        public static void ProfileEngineAudio(EntityManager entityManager)
        {
            using (_engineAudioMarker.Auto())
            {
                var query = entityManager.CreateEntityQuery(typeof(EngineAudioData));
                var entities = query.ToEntityArray(Allocator.Temp);
                
                // Симулируем работу системы
                foreach (var entity in entities)
                {
                    var audioData = entityManager.GetComponentData<EngineAudioData>(entity);
                    audioData.RPM += 10f;
                    entityManager.SetComponentData(entity, audioData);
                }
                
                entities.Dispose();
            }
        }
        
        /// <summary>
        /// Профилирует систему лебедки
        /// </summary>
        public static void ProfileWinch(EntityManager entityManager)
        {
            using (_winchMarker.Auto())
            {
                var query = entityManager.CreateEntityQuery(typeof(WinchData));
                var entities = query.ToEntityArray(Allocator.Temp);
                
                // Симулируем работу системы
                foreach (var entity in entities)
                {
                    var winchData = entityManager.GetComponentData<WinchData>(entity);
                    winchData.CableLength += 0.1f;
                    entityManager.SetComponentData(entity, winchData);
                }
                
                entities.Dispose();
            }
        }
        
        /// <summary>
        /// Профилирует систему грузов
        /// </summary>
        public static void ProfileCargo(EntityManager entityManager)
        {
            using (_cargoMarker.Auto())
            {
                var query = entityManager.CreateEntityQuery(typeof(CargoData));
                var entities = query.ToEntityArray(Allocator.Temp);
                
                // Симулируем работу системы
                foreach (var entity in entities)
                {
                    var cargoData = entityManager.GetComponentData<CargoData>(entity);
                    cargoData.Condition -= 0.001f;
                    entityManager.SetComponentData(entity, cargoData);
                }
                
                entities.Dispose();
            }
        }
        
        /// <summary>
        /// Профилирует систему миссий
        /// </summary>
        public static void ProfileMission(EntityManager entityManager)
        {
            using (_missionMarker.Auto())
            {
                var query = entityManager.CreateEntityQuery(typeof(MissionData));
                var entities = query.ToEntityArray(Allocator.Temp);
                
                // Симулируем работу системы
                foreach (var entity in entities)
                {
                    var missionData = entityManager.GetComponentData<MissionData>(entity);
                    missionData.Progress += 0.01f;
                    entityManager.SetComponentData(entity, missionData);
                }
                
                entities.Dispose();
            }
        }
        
        /// <summary>
        /// Профилирует систему LOD
        /// </summary>
        public static void ProfileLOD(EntityManager entityManager)
        {
            using (_lodMarker.Auto())
            {
                var query = entityManager.CreateEntityQuery(typeof(LODData));
                var entities = query.ToEntityArray(Allocator.Temp);
                
                // Симулируем работу системы
                foreach (var entity in entities)
                {
                    var lodData = entityManager.GetComponentData<LODData>(entity);
                    lodData.DistanceToCamera += 0.1f;
                    entityManager.SetComponentData(entity, lodData);
                }
                
                entities.Dispose();
            }
        }
        
        /// <summary>
        /// Профилирует систему пула объектов
        /// </summary>
        public static void ProfileObjectPool(EntityManager entityManager)
        {
            using (_poolMarker.Auto())
            {
                var query = entityManager.CreateEntityQuery(typeof(ObjectPoolData));
                var entities = query.ToEntityArray(Allocator.Temp);
                
                // Симулируем работу системы
                foreach (var entity in entities)
                {
                    var poolData = entityManager.GetComponentData<ObjectPoolData>(entity);
                    poolData.ActiveCount++;
                    entityManager.SetComponentData(entity, poolData);
                }
                
                entities.Dispose();
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
                FPS = 1f / Time.deltaTime,
                MemoryUsage = Profiler.GetTotalAllocatedMemory(false),
                GCMemory = Profiler.GetMonoUsedSize(),
                DrawCalls = Profiler.GetRuntimeMemorySize(null),
                Triangles = 0, // TODO: Получить количество треугольников
                Vertices = 0,  // TODO: Получить количество вершин
                Entities = 0   // TODO: Получить количество сущностей
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
}