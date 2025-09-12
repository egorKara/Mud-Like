using Unity.Entities;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Jobs;
using Unity.Collections;
using Unity.Transforms;
using MudLike.Vehicles.Components;
using MudLike.Core.Components;

namespace MudLike.Core.Performance
{
    /// <summary>
    /// Оптимизированная система Job для высокопроизводительных вычислений
    /// Использует Burst Compiler и Job System для максимальной производительности
    /// </summary>
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile(CompileSynchronously = true)]
    public partial class OptimizedJobSystem : SystemBase
    {
        private EntityQuery _vehicleQuery;
        private EntityQuery _physicsQuery;
        
        protected override void OnCreate()
        {
            _vehicleQuery = GetEntityQuery(
                ComponentType.ReadWrite<LocalTransform>(),
                ComponentType.ReadOnly<VehiclePhysics>(),
                ComponentType.ReadOnly<VehicleTag>()
            );
            
            _physicsQuery = GetEntityQuery(
                ComponentType.ReadWrite<VehiclePhysics>(),
                ComponentType.ReadOnly<PlayerInput>(),
                ComponentType.ReadOnly<VehicleConfig>()
            );
        }
        
        protected override void OnUpdate()
        {
            float deltaTime = SystemAPI.Time.fixedDeltaTime;
            
            // Параллельная обработка физики транспорта
            var physicsJob = new VehiclePhysicsJob
            {
                DeltaTime = deltaTime
            };
            
            // Параллельная обработка позиций
            var positionJob = new VehiclePositionJob
            {
                DeltaTime = deltaTime
            };
            
            // Запускаем Jobs параллельно
            Dependency = JobHandle.CombineDependencies(
                physicsJob.ScheduleParallel(_physicsQuery, Dependency),
                positionJob.ScheduleParallel(_vehicleQuery, Dependency)
            );
        }
        
        /// <summary>
        /// Job для обработки физики транспорта
        /// </summary>
        [BurstCompile]
        public partial struct VehiclePhysicsJob : IJobEntity
        {
            public float DeltaTime;
            
            public void Execute(ref VehiclePhysics physics, in PlayerInput input, in VehicleConfig config)
            {
                // Оптимизированная обработка ввода
                ProcessVehicleInput(ref physics, input, config, DeltaTime);
                
                // Обновление физических параметров
                UpdatePhysics(ref physics, config, DeltaTime);
            }
            
            [BurstCompile]
            private static void ProcessVehicleInput(ref VehiclePhysics physics, in PlayerInput input, in VehicleConfig config, float deltaTime)
            {
                // Обработка газа и тормоза
                float inputStrength = input.Accelerate ? 1f : (input.Brake ? -1f : 0f);
                
                // Плавное изменение мощности двигателя
                float targetPower = inputStrength * config.MaxEnginePower;
                physics.EnginePower = math.lerp(physics.EnginePower, targetPower, config.EngineResponse * deltaTime);
                
                // Обработка рулевого управления
                float targetSteering = input.Steering * config.MaxSteeringAngle;
                physics.SteeringAngle = math.lerp(physics.SteeringAngle, targetSteering, config.SteeringResponse * deltaTime);
                
                // Ограничиваем значения
                physics.EnginePower = math.clamp(physics.EnginePower, -config.MaxEnginePower * 0.5f, config.MaxEnginePower);
                physics.SteeringAngle = math.clamp(physics.SteeringAngle, -config.MaxSteeringAngle, config.MaxSteeringAngle);
            }
            
            [BurstCompile]
            private static void UpdatePhysics(ref VehiclePhysics physics, in VehicleConfig config, float deltaTime)
            {
                // Вычисляем скорость на основе мощности двигателя
                float targetSpeed = physics.EnginePower * config.MaxSpeed;
                float speedDifference = targetSpeed - math.length(physics.Velocity);
                
                // Применяем ускорение
                float acceleration = speedDifference * config.Acceleration * deltaTime;
                physics.Velocity += new float3(0, 0, acceleration);
                
                // Применяем сопротивление
                physics.Velocity *= (1f - config.Drag * deltaTime);
                
                // Ограничиваем максимальную скорость
                float currentSpeed = math.length(physics.Velocity);
                if (currentSpeed > config.MaxSpeed)
                {
                    physics.Velocity = math.normalize(physics.Velocity) * config.MaxSpeed;
                }
                
                // Обновляем поворот
                if (math.abs(physics.SteeringAngle) > 0.01f && currentSpeed > 0.1f)
                {
                    float turnRate = physics.SteeringAngle * currentSpeed * config.TurnSpeedMultiplier * deltaTime;
                    physics.Rotation = quaternion.RotateY(turnRate);
                }
            }
        }
        
        /// <summary>
        /// Job для обработки позиций транспорта
        /// </summary>
        [BurstCompile]
        public partial struct VehiclePositionJob : IJobEntity
        {
            public float DeltaTime;
            
            public void Execute(ref LocalTransform transform, in VehiclePhysics physics)
            {
                // Обновляем позицию на основе скорости
                transform.Position += physics.Velocity * DeltaTime;
                
                // Обновляем поворот
                transform.Rotation = physics.Rotation;
            }
        }
    }
    
    /// <summary>
    /// Оптимизированная система для обработки деформации террейна
    /// </summary>
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile(CompileSynchronously = true)]
    public partial class OptimizedTerrainSystem : SystemBase
    {
        private EntityQuery _deformationQuery;
        private NativeArray<float3> _terrainHeights;
        private NativeArray<float> _mudLevels;
        
        protected override void OnCreate()
        {
            _deformationQuery = GetEntityQuery(
                ComponentType.ReadOnly<WheelData>(),
                ComponentType.ReadOnly<VehiclePhysics>(),
                ComponentType.ReadOnly<LocalTransform>()
            );
            
            // Инициализируем массивы для террейна
            int terrainSize = 1024; // 1024x1024 точек
            _terrainHeights = new NativeArray<float3>(terrainSize * terrainSize, Allocator.Persistent);
            _mudLevels = new NativeArray<float>(terrainSize * terrainSize, Allocator.Persistent);
        }
        
        protected override void OnDestroy()
        {
            if (_terrainHeights.IsCreated)
                _terrainHeights.Dispose();
            if (_mudLevels.IsCreated)
                _mudLevels.Dispose();
        }
        
        protected override void OnUpdate()
        {
            var deformationJob = new TerrainDeformationJob
            {
                TerrainHeights = _terrainHeights,
                MudLevels = _mudLevels,
                DeltaTime = SystemAPI.Time.fixedDeltaTime,
                TerrainSize = 1024
            };
            
            Dependency = deformationJob.ScheduleParallel(_deformationQuery, Dependency);
        }
        
        /// <summary>
        /// Job для обработки деформации террейна
        /// </summary>
        [BurstCompile]
        public partial struct TerrainDeformationJob : IJobEntity
        {
            public NativeArray<float3> TerrainHeights;
            public NativeArray<float> MudLevels;
            public float DeltaTime;
            public int TerrainSize;
            
            public void Execute(in WheelData wheel, in VehiclePhysics physics, in LocalTransform transform)
            {
                // Вычисляем позицию колеса
                float3 wheelPosition = transform.Position + wheel.Position;
                
                // Получаем индекс в массиве террейна
                int index = GetTerrainIndex(wheelPosition);
                if (index < 0 || index >= TerrainHeights.Length) return;
                
                // Вычисляем силу воздействия
                float wheelForce = math.length(physics.Velocity) * physics.Mass;
                
                // Применяем деформацию
                ApplyDeformation(index, wheelForce, wheel.Radius, DeltaTime);
            }
            
            [BurstCompile]
            private void ApplyDeformation(int index, float force, float radius, float deltaTime)
            {
                // Простая модель деформации
                float deformationAmount = force * 0.001f * deltaTime;
                float mudIncrease = force * 0.0001f * deltaTime;
                
                // Обновляем высоту террейна
                float3 currentHeight = TerrainHeights[index];
                currentHeight.y -= deformationAmount;
                TerrainHeights[index] = currentHeight;
                
                // Обновляем уровень грязи
                float currentMud = MudLevels[index];
                currentMud += mudIncrease;
                MudLevels[index] = math.clamp(currentMud, 0f, 1f);
            }
            
            [BurstCompile]
            private int GetTerrainIndex(float3 position)
            {
                int x = (int)math.floor(position.x) + TerrainSize / 2;
                int z = (int)math.floor(position.z) + TerrainSize / 2;
                
                if (x < 0 || x >= TerrainSize || z < 0 || z >= TerrainSize)
                    return -1;
                
                return z * TerrainSize + x;
            }
        }
    }
}
