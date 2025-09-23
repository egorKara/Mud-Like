using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;
using Unity.Jobs;
using Unity.Collections;
using MudLike.Vehicles.Components;
using MudLike.Core.Components;
using static MudLike.Core.Components.Position;

namespace MudLike.Core.ECS
{
    /// <summary>
    /// Контекстные промпты для работы с транспортными ECS модулями
    /// </summary>
    public static class VehicleECSPromptContext
    {
        /// <summary>
        /// Контекст для создания транспортных систем
        /// </summary>
        public const string VehicleSystemContext = @"
@context Vehicle System Creation:
Используй следующие компоненты для транспортных систем:

Основные компоненты:
- VehiclePhysics: физика транспорта (Velocity, EnginePower, SteeringAngle, Rotation)
- VehicleConfig: конфигурация (MaxSpeed, Acceleration, TurnSpeed, MaxEnginePower)
- VehicleInput: ввод (Vertical, Horizontal, Brake, Accelerate, Steering)
- LocalTransform: позиция и поворот транспорта
- VehicleTag: тег транспорта

Системы транспорта:
- VehicleMovementSystem: основное движение
- VehicleControlSystem: управление игроком
- OptimizedVehicleMovementSystem: оптимизированное движение
- AdvancedVehicleSystem: продвинутые системы

Пример создания системы:
```csharp
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[BurstCompile(CompileSynchronously = true)]
public partial class YourVehicleSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = SystemAPI.Time.fixedDeltaTime;
        
        Entities
            .WithAll<VehicleTag>()
            .ForEach((ref LocalTransform transform, 
                     ref VehiclePhysics physics, 
                     in VehicleConfig config, 
                     in VehicleInput input) =>
            {
                // Ваша логика здесь
            }).Schedule();
    }
}
```

Всегда используй:
- FixedStepSimulationSystemGroup для детерминизма
- BurstCompile для производительности
- SystemAPI.Time.fixedDeltaTime для детерминированных вычислений
- LocalTransform.Position для позиций
- VehiclePhysics.Velocity для скорости
";

        /// <summary>
        /// Контекст для создания систем физики колес
        /// </summary>
        public const string WheelPhysicsContext = @"
@context Wheel Physics System:
Используй следующие компоненты для физики колес:

Компоненты колес:
- WheelData: данные колеса (Position, Radius, Width, SuspensionLength, SpringForce, DampingForce)
- WheelPhysicsData: физика колеса (IsGrounded, GroundPoint, GroundNormal, SuspensionForce, FrictionForce)
- LocalTransform: позиция колеса в мире

Системы колес:
- WheelPhysicsSystem: основная физика колес
- OptimizedWheelPhysicsSystem: оптимизированная физика
- AdvancedWheelPhysicsSystem: продвинутая физика

Пример создания системы:
```csharp
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[BurstCompile(CompileSynchronously = true)]
public partial class YourWheelSystem : SystemBase
{
    protected override void OnCreate()
    {
        RequireForUpdate<PhysicsWorldSingleton>();
    }
    
    protected override void OnUpdate()
    {
        float deltaTime = SystemAPI.Time.fixedDeltaTime;
        var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().PhysicsWorld;
        
        var job = new WheelPhysicsJob
        {
            DeltaTime = deltaTime,
            PhysicsWorld = physicsWorld
        };
        
        Dependency = job.ScheduleParallel(Dependency);
    }
}
```

Всегда используй:
- PhysicsWorldSingleton для raycast операций
- BurstCompile для производительности
- Job System для параллельной обработки
- Raycast для определения контакта с землей
";

        /// <summary>
        /// Контекст для создания систем лебедки
        /// </summary>
        public const string WinchSystemContext = @"
@context Winch System:
Используй следующие компоненты для системы лебедки:

Компоненты лебедки:
- WinchData: данные лебедки (IsActive, IsDeployed, IsConnected, MaxCableLength, WinchForce)
- WinchCableData: данные троса (Length, Tension, Wear, Strength)
- WinchConnectionData: данные подключения (ConnectedEntity, ConnectionPoint, AttachmentPoint)

Система лебедки:
- WinchSystem: основная система лебедки

Пример создания системы:
```csharp
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[BurstCompile(CompileSynchronously = true)]
public partial class YourWinchSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = SystemAPI.Time.fixedDeltaTime;
        
        Entities
            .WithAll<WinchData>()
            .ForEach((ref WinchData winchData, in LocalTransform transform) =>
            {
                if (!winchData.IsActive) return;
                
                // Обновляем позицию крепления
                winchData.AttachmentPoint = transform.Position;
                
                // Обновляем трос
                UpdateCable(ref winchData, transform);
                
                // Применяем силу лебедки
                ApplyWinchForce(ref winchData, transform);
            }).Schedule();
    }
}
```

Всегда используй:
- Проверку IsActive перед обработкой
- Обновление AttachmentPoint на основе transform.Position
- Расчет длины троса через math.distance
- Ограничение силы через math.clamp
";

        /// <summary>
        /// Контекст для создания оптимизированных систем
        /// </summary>
        public const string OptimizedSystemContext = @"
@context Optimized Vehicle System:
Используй следующие техники для оптимизации:

Оптимизированные системы:
- OptimizedVehicleMovementSystem: оптимизированное движение
- ChunkOptimizedVehicleMovementSystem: chunk-based обработка
- SIMDOptimizedVehicleMovementSystem: SIMD оптимизация

Пример оптимизированной системы:
```csharp
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[BurstCompile(CompileSynchronously = true)]
public partial class YourOptimizedSystem : SystemBase
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
        var job = new OptimizedVehicleJob
        {
            DeltaTime = SystemAPI.Time.fixedDeltaTime
        };
        
        Dependency = job.ScheduleParallel(_vehicleQuery, Dependency);
    }
    
    [BurstCompile]
    public partial struct OptimizedVehicleJob : IJobEntity
    {
        public float DeltaTime;
        
        public void Execute(ref VehiclePhysics physics, 
                          in VehicleConfig config, 
                          in VehicleInput input,
                          ref LocalTransform transform)
        {
            // Оптимизированная логика здесь
        }
    }
}
```

Всегда используй:
- EntityQuery для оптимизированных запросов
- IJobEntity для параллельной обработки
- BurstCompile для нативной компиляции
- Dependency для правильного порядка выполнения
- Предвычисленные константы
- Быстрые математические операции
";
    }
}
