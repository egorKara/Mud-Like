using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;
using Unity.Burst;
using MudLike.Vehicles.Components;
using MudLike.Terrain.Components;

namespace MudLike.Vehicles.Systems
{
    /// <summary>
    /// Продвинутая система физики колес
    /// </summary>
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile]
    public partial class AdvancedWheelPhysicsSystem : SystemBase
    {
        private EntityQuery _wheelQuery;
        private EntityQuery _surfaceQuery;
        
        protected override void OnCreate()
        {
            RequireForUpdate<PhysicsWorldSingleton>();
            
            _wheelQuery = GetEntityQuery(
                ComponentType.ReadWrite<WheelData>(),
                ComponentType.ReadWrite<WheelPhysicsData>(),
                ComponentType.ReadOnly<LocalTransform>(),
                ComponentType.ReadOnly<VehiclePhysics>()
            );
            
            _surfaceQuery = GetEntityQuery(
                ComponentType.ReadOnly<SurfaceData>()
            );
        }
        
        protected override void OnUpdate()
        {
            float deltaTime = SystemAPI.Time.fixedDeltaTime;
            var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().PhysicsWorld;
            
            var wheelPhysicsJob = new AdvancedWheelPhysicsJob
            {
                DeltaTime = deltaTime,
                PhysicsWorld = physicsWorld,
                SurfaceData = GetSurfaceData()
            };
            
            Dependency = wheelPhysicsJob.ScheduleParallel(_wheelQuery, Dependency);
        }
        
        private NativeArray<SurfaceData> GetSurfaceData()
        {
            var surfaceEntities = _surfaceQuery.ToEntityArray(Unity.Collections.Allocator.Temp);
            var surfaceData = new NativeArray<SurfaceData>(surfaceEntities.Length, Unity.Collections.Allocator.Temp);
            
            for (int i = 0; i < surfaceEntities.Length; i++)
            {
                surfaceData[i] = EntityManager.GetComponentData<SurfaceData>(surfaceEntities[i]);
            }
            
            surfaceEntities.Dispose();
            return surfaceData;
        }
    }
    
    /// <summary>
    /// Job для обработки физики колес
    /// </summary>
    [BurstCompile]
    public struct AdvancedWheelPhysicsJob : IJobEntity
    {
        public float DeltaTime;
        public PhysicsWorld PhysicsWorld;
        public NativeArray<SurfaceData> SurfaceData;
        
        public void Execute(ref WheelData wheel, ref WheelPhysicsData physics, in LocalTransform transform, in VehiclePhysics vehiclePhysics)
        {
            // Обновляем физику колеса
            UpdateWheelPhysics(ref wheel, ref physics, transform, vehiclePhysics, DeltaTime);
        }
        
        private void UpdateWheelPhysics(ref WheelData wheel, ref WheelPhysicsData physics, in LocalTransform transform, in VehiclePhysics vehiclePhysics, float deltaTime)
        {
            // Простая реализация физики колеса
            // В реальной реализации здесь будет сложная физика
            
            // Обновляем температуру колеса
            physics.WheelTemperature += physics.HeatingRate * deltaTime;
            physics.WheelTemperature -= physics.CoolingRate * deltaTime;
            physics.WheelTemperature = math.clamp(physics.WheelTemperature, 20f, 100f);
            
            // Обновляем износ протектора
            physics.TreadWear += 0.001f * deltaTime;
            physics.TreadWear = math.clamp(physics.TreadWear, 0f, 1f);
            
            // Обновляем время контакта
            if (wheel.IsGrounded)
            {
                physics.ContactTime += deltaTime;
            }
            else
            {
                physics.ContactTime = 0f;
            }
            
            // Обновляем время последнего обновления
            physics.LastUpdateTime += deltaTime;
            physics.NeedsUpdate = false;
        }
    }
}