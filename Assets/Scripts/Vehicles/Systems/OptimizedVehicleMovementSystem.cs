using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;
using Unity.Jobs;
using Unity.Collections;
using MudLike.Vehicles.Components;
using MudLike.Core.Components;
using MudLike.Core.Performance;
using static MudLike.Core.Components.Position;

namespace MudLike.Vehicles.Systems
{
    /// <summary>
    /// Оптимизированная система движения транспортных средств с Burst и Job System
    /// </summary>
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile]
    public partial class OptimizedVehicleMovementSystem : SystemBase
    {
        private EntityQuery _vehicleQuery;
        private MemoryPool _memoryPool;
        
        protected override void OnCreate()
        {
            // Создание запроса для транспортных средств
            _vehicleQuery = GetEntityQuery(
                ComponentType.ReadWrite<VehiclePhysics>(),
                ComponentType.ReadOnly<VehicleConfig>(),
                ComponentType.ReadOnly<VehicleInput>(),
                ComponentType.ReadWrite<LocalTransform>()
            );
            
            // Получение пула памяти
            _memoryPool = World.GetExistingSystemManaged<MemoryPool>();
        }
        
        protected override void OnUpdate()
        {
            // Создание и выполнение оптимизированного job
            var job = new OptimizedVehicleMovementJob
            {
                DeltaTime = Time.fixedDeltaTime,
                MemoryPool = _memoryPool
            };
            
            Dependency = job.ScheduleParallel(_vehicleQuery, Dependency);
        }
        
        /// <summary>
        /// Оптимизированный job для движения транспортных средств
        /// </summary>
        [BurstCompile(CompileSynchronously = true)]
        public partial struct OptimizedVehicleMovementJob : IJobEntity
        {
            [ReadOnly] public float DeltaTime;
            [ReadOnly] public MemoryPool MemoryPool;
            
            public void Execute(ref VehiclePhysics physics, 
                              in VehicleConfig config, 
                              in VehicleInput input,
                              ref LocalTransform transform)
            {
                ProcessOptimizedVehicleMovement(ref physics, config, input, ref transform);
            }
            
            /// <summary>
            /// Обрабатывает оптимизированное движение транспортного средства
            /// </summary>
            [BurstCompile]
            private static void ProcessOptimizedVehicleMovement(ref VehiclePhysics physics,
                                                              in VehicleConfig config,
                                                              in VehicleInput input,
                                                              ref LocalTransform transform)
            {
                // Вычисляем направление движения с использованием Burst-оптимизированных функций
                float3 forward = BurstOptimizedMath.FastTransform(transform.Rotation, new float3(0, 0, 1));
                float3 right = BurstOptimizedMath.FastTransform(transform.Rotation, new float3(1, 0, 0));
                
                // Применяем ввод с оптимизированными вычислениями
                float3 movementInput = forward * input.Vertical + right * input.Horizontal;
                movementInput = BurstOptimizedMath.FastNormalize(movementInput);
                
                // Вычисляем ускорение с использованием быстрых математических операций
                float3 targetVelocity = movementInput * config.MaxSpeed;
                float3 acceleration = (targetVelocity - physics.Velocity) * config.Acceleration;
                
                // Применяем сопротивление с оптимизированными вычислениями
                acceleration -= physics.Velocity * config.Drag;
                
                // Обновляем физику
                physics.Acceleration = acceleration;
                physics.Velocity += acceleration * DeltaTime;
                
                // Ограничиваем скорость с использованием быстрых операций
                float currentSpeed = BurstOptimizedMath.FastLength(physics.Velocity);
                if (currentSpeed > config.MaxSpeed)
                {
                    physics.Velocity = BurstOptimizedMath.FastNormalize(physics.Velocity) * config.MaxSpeed;
                }
                
                // Обновляем позицию
                transform.Position += physics.Velocity * DeltaTime;
                
                // Вычисляем поворот с оптимизированными операциями
                if (BurstOptimizedMath.FastAbs(input.Horizontal) > 0.1f)
                {
                    float turnAngle = input.Horizontal * config.TurnSpeed * DeltaTime;
                    quaternion turnRotation = BurstOptimizedMath.FastRotateY(turnAngle);
                    transform.Rotation = BurstOptimizedMath.FastMul(transform.Rotation, turnRotation);
                }
                
                // Обновляем скорость движения с использованием быстрых операций
                physics.ForwardSpeed = BurstOptimizedMath.FastDot(physics.Velocity, forward);
                physics.TurnSpeed = input.Horizontal;
            }
        }
    }
    
    /// <summary>
    /// Chunk-based оптимизированная система движения транспортных средств
    /// </summary>
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile]
    public partial class ChunkOptimizedVehicleMovementSystem : SystemBase
    {
        private EntityQuery _vehicleQuery;
        
        protected override void OnCreate()
        {
            _vehicleQuery = GetEntityQuery(
                ComponentType.ReadWrite<VehiclePhysics>(),
                ComponentType.ReadOnly<VehicleConfig>(),
                ComponentType.ReadOnly<VehicleInput>(),
                ComponentType.ReadWrite<LocalTransform>()
            );
        }
        
        protected override void OnUpdate()
        {
            var job = new ChunkOptimizedVehicleMovementJob
            {
                DeltaTime = Time.fixedDeltaTime
            };
            
            Dependency = job.ScheduleParallel(_vehicleQuery, Dependency);
        }
        
        /// <summary>
        /// Chunk-based оптимизированный job
        /// </summary>
        [BurstCompile(CompileSynchronously = true)]
        public partial struct ChunkOptimizedVehicleMovementJob : IJobChunk
        {
            [ReadOnly] public float DeltaTime;
            
            public ComponentTypeHandle<VehiclePhysics> VehiclePhysicsType;
            [ReadOnly] public ComponentTypeHandle<VehicleConfig> VehicleConfigType;
            [ReadOnly] public ComponentTypeHandle<VehicleInput> VehicleInputType;
            public ComponentTypeHandle<LocalTransform> LocalTransformType;
            
            public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, 
                               bool useEnabledMask, in v128 chunkEnabledMask)
            {
                // Получение массивов компонентов
                var vehiclePhysicsArray = chunk.GetNativeArray(ref VehiclePhysicsType);
                var vehicleConfigArray = chunk.GetNativeArray(ref VehicleConfigType);
                var vehicleInputArray = chunk.GetNativeArray(ref VehicleInputType);
                var localTransformArray = chunk.GetNativeArray(ref LocalTransformType);
                
                // Обработка всех сущностей в чанке
                for (int i = 0; i < chunk.Count; i++)
                {
                    ProcessVehicleInChunk(ref vehiclePhysicsArray[i],
                                        vehicleConfigArray[i],
                                        vehicleInputArray[i],
                                        ref localTransformArray[i]);
                }
            }
            
            /// <summary>
            /// Обрабатывает транспортное средство в чанке
            /// </summary>
            [BurstCompile]
            private static void ProcessVehicleInChunk(ref VehiclePhysics physics,
                                                    in VehicleConfig config,
                                                    in VehicleInput input,
                                                    ref LocalTransform transform)
            {
                // Аналогичная логика, но оптимизированная для chunk-based обработки
                float3 forward = BurstOptimizedMath.FastTransform(transform.Rotation, new float3(0, 0, 1));
                float3 right = BurstOptimizedMath.FastTransform(transform.Rotation, new float3(1, 0, 0));
                
                float3 movementInput = forward * input.Vertical + right * input.Horizontal;
                movementInput = BurstOptimizedMath.FastNormalize(movementInput);
                
                float3 targetVelocity = movementInput * config.MaxSpeed;
                float3 acceleration = (targetVelocity - physics.Velocity) * config.Acceleration;
                
                acceleration -= physics.Velocity * config.Drag;
                
                physics.Acceleration = acceleration;
                physics.Velocity += acceleration * DeltaTime;
                
                float currentSpeed = BurstOptimizedMath.FastLength(physics.Velocity);
                if (currentSpeed > config.MaxSpeed)
                {
                    physics.Velocity = BurstOptimizedMath.FastNormalize(physics.Velocity) * config.MaxSpeed;
                }
                
                transform.Position += physics.Velocity * DeltaTime;
                
                if (BurstOptimizedMath.FastAbs(input.Horizontal) > 0.1f)
                {
                    float turnAngle = input.Horizontal * config.TurnSpeed * DeltaTime;
                    quaternion turnRotation = BurstOptimizedMath.FastRotateY(turnAngle);
                    transform.Rotation = BurstOptimizedMath.FastMul(transform.Rotation, turnRotation);
                }
                
                physics.ForwardSpeed = BurstOptimizedMath.FastDot(physics.Velocity, forward);
                physics.TurnSpeed = input.Horizontal;
            }
        }
    }
    
    /// <summary>
    /// SIMD-оптимизированная система движения транспортных средств
    /// </summary>
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile]
    public partial class SIMDOptimizedVehicleMovementSystem : SystemBase
    {
        private EntityQuery _vehicleQuery;
        
        protected override void OnCreate()
        {
            _vehicleQuery = GetEntityQuery(
                ComponentType.ReadWrite<VehiclePhysics>(),
                ComponentType.ReadOnly<VehicleConfig>(),
                ComponentType.ReadOnly<VehicleInput>(),
                ComponentType.ReadWrite<LocalTransform>()
            );
        }
        
        protected override void OnUpdate()
        {
            var job = new SIMDOptimizedVehicleMovementJob
            {
                DeltaTime = Time.fixedDeltaTime
            };
            
            Dependency = job.ScheduleParallel(_vehicleQuery, Dependency);
        }
        
        /// <summary>
        /// SIMD-оптимизированный job
        /// </summary>
        [BurstCompile(CompileSynchronously = true)]
        public partial struct SIMDOptimizedVehicleMovementJob : IJobEntity
        {
            [ReadOnly] public float DeltaTime;
            
            public void Execute(ref VehiclePhysics physics, 
                              in VehicleConfig config, 
                              in VehicleInput input,
                              ref LocalTransform transform)
            {
                ProcessSIMDVehicleMovement(ref physics, config, input, ref transform);
            }
            
            /// <summary>
            /// Обрабатывает SIMD-оптимизированное движение транспортного средства
            /// </summary>
            [BurstCompile]
            private static void ProcessSIMDVehicleMovement(ref VehiclePhysics physics,
                                                         in VehicleConfig config,
                                                         in VehicleInput input,
                                                         ref LocalTransform transform)
            {
                // Использование SIMD операций для векторных вычислений
                float4 inputVector = new float4(input.Vertical, input.Horizontal, 0f, 0f);
                float4 configVector = new float4(config.MaxSpeed, config.TurnSpeed, config.Acceleration, config.Drag);
                
                // SIMD вычисления для направления движения
                float3 forward = BurstOptimizedMath.FastTransform(transform.Rotation, new float3(0, 0, 1));
                float3 right = BurstOptimizedMath.FastTransform(transform.Rotation, new float3(1, 0, 0));
                
                // Векторизованные вычисления
                float3 movementInput = forward * input.Vertical + right * input.Horizontal;
                movementInput = BurstOptimizedMath.FastNormalize(movementInput);
                
                // SIMD вычисления для ускорения
                float3 targetVelocity = movementInput * config.MaxSpeed;
                float3 acceleration = (targetVelocity - physics.Velocity) * config.Acceleration;
                acceleration -= physics.Velocity * config.Drag;
                
                // Обновление физики с SIMD операциями
                physics.Acceleration = acceleration;
                physics.Velocity += acceleration * DeltaTime;
                
                // SIMD ограничение скорости
                float currentSpeed = BurstOptimizedMath.FastLength(physics.Velocity);
                if (currentSpeed > config.MaxSpeed)
                {
                    physics.Velocity = BurstOptimizedMath.FastNormalize(physics.Velocity) * config.MaxSpeed;
                }
                
                // Обновление позиции
                transform.Position += physics.Velocity * DeltaTime;
                
                // SIMD вычисления для поворота
                if (BurstOptimizedMath.FastAbs(input.Horizontal) > 0.1f)
                {
                    float turnAngle = input.Horizontal * config.TurnSpeed * DeltaTime;
                    quaternion turnRotation = BurstOptimizedMath.FastRotateY(turnAngle);
                    transform.Rotation = BurstOptimizedMath.FastMul(transform.Rotation, turnRotation);
                }
                
                // SIMD обновление скорости движения
                physics.ForwardSpeed = BurstOptimizedMath.FastDot(physics.Velocity, forward);
                physics.TurnSpeed = input.Horizontal;
            }
        }
    }
}