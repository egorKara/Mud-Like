using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;
using Unity.Jobs;
using Unity.Collections;
using MudLike.Core.Components;
using static MudLike.Core.Components.Position;

namespace MudLike.Core.ECS
{
    /// <summary>
    /// Контекстные промпты для работы с ECS модулями
    /// Используется с @context для оптимизации работы с AI ассистентами
    /// </summary>
    public static class ECSPromptContext
    {
        #region Core ECS Components Context
        
        /// <summary>
        /// Контекст для работы с основными ECS компонентами
        /// </summary>
        public const string CoreComponentsContext = @"
@context Core ECS Components:
- Position: IComponentData with float3 Value - позиция в мире
- Velocity: IComponentData with float3 Value - скорость движения
- PlayerTag: IComponentData - тег игрока
- PlayerInput: IComponentData with float2 VehicleMovement, bool Accelerate, bool Brake, float Steering - ввод игрока
- VehicleTag: IComponentData - тег транспорта
- GameBootstrapTag: IComponentData - тег инициализации игры

Используй using static MudLike.Core.Components.Position; для прямого доступа к Value.
Всегда используй ECS архитектуру, НЕ MonoBehaviour.
Всегда используй BurstCompile для производительности.
Всегда используй FixedStepSimulationSystemGroup для детерминизма.
";
        
        #endregion
        
        #region Vehicle ECS Context
        
        /// <summary>
        /// Контекст для работы с транспортными ECS компонентами
        /// </summary>
        public const string VehicleComponentsContext = @"
@context Vehicle ECS Components:
- VehiclePhysics: IComponentData - физика транспорта (Velocity, EnginePower, SteeringAngle, etc.)
- VehicleConfig: IComponentData - конфигурация транспорта (MaxSpeed, Acceleration, TurnSpeed, etc.)
- VehicleInput: IComponentData - ввод транспорта (Vertical, Horizontal, Brake, etc.)
- WheelData: IComponentData - данные колеса (Position, Radius, Width, SuspensionLength, etc.)
- EngineData: IComponentData - данные двигателя (RPM, Power, Torque, etc.)
- TransmissionData: IComponentData - данные трансмиссии (Gear, GearRatio, etc.)

Системы транспорта:
- VehicleMovementSystem: основное движение транспорта
- VehicleControlSystem: управление транспортом игроком
- OptimizedVehicleMovementSystem: оптимизированное движение
- AdvancedVehicleSystem: продвинутые системы транспорта
- WheelPhysicsSystem: физика колес
- WinchSystem: система лебедки

Всегда используй LocalTransform для позиций транспорта.
Всегда используй BurstCompile для физических вычислений.
Всегда используй Job System для параллельной обработки.
";
        
        #endregion
        
        #region Networking ECS Context
        
        /// <summary>
        /// Контекст для работы с сетевыми ECS компонентами
        /// </summary>
        public const string NetworkingComponentsContext = @"
@context Networking ECS Components:
- NetworkId: IComponentData - сетевой идентификатор
- NetworkPosition: IComponentData - сетевая позиция (Value, Rotation, HasChanged, LastUpdateTime)
- NetworkVehicle: IComponentData - сетевые данные транспорта
- NetworkDeformation: IComponentData - сетевые данные деформации
- NetworkMud: IComponentData - сетевые данные грязи

Сетевые системы:
- NetworkSyncSystem: синхронизация сетевых данных
- NetworkPositionInterpolationSystem: интерполяция позиций
- NetworkPositionPredictionSystem: предсказание позиций
- NetworkPositionValidationSystem: валидация позиций
- LagCompensationSystem: компенсация задержек
- AntiCheatSystem: античит система

Используй NetCodeClientAndServerSystemGroup для сетевых систем.
Всегда проверяй HasSingleton<NetworkStreamInGame>() перед сетевыми операциями.
Всегда используй детерминированные вычисления для синхронизации.
";
        
        #endregion
        
        #region Terrain ECS Context
        
        /// <summary>
        /// Контекст для работы с террейном ECS компонентами
        /// </summary>
        public const string TerrainComponentsContext = @"
@context Terrain ECS Components:
- TerrainData: IComponentData - данные террейна (worldPosition, height, etc.)
- DeformationData: IComponentData - данные деформации (depth, radius, position)
- MudData: IComponentData - данные грязи (level, viscosity, etc.)
- SurfaceData: IComponentData - данные поверхности (type, friction, etc.)

Террейновые системы:
- TerrainDeformationSystem: деформация террейна
- MudManagerSystem: управление грязью
- TerrainSyncSystem: синхронизация террейна
- WorldGridSystem: система мировых координат

Используй MudManager API для взаимодействия с грязью.
Всегда синхронизируй TerrainData + TerrainCollider.
Используй heightfield per block для деформации.
";
        
        #endregion
        
        #region Job System Context
        
        /// <summary>
        /// Контекст для работы с Job System
        /// </summary>
        public const string JobSystemContext = @"
@context Job System:
- IJobEntity: для обработки сущностей с компонентами
- IJobChunk: для обработки чанков сущностей
- IJobParallelFor: для параллельной обработки массивов

Обязательные атрибуты:
- [BurstCompile]: для компиляции в нативный код
- [ReadOnly]: для компонентов только для чтения
- [WriteOnly]: для компонентов только для записи

Примеры Job структур:
- VehiclePhysicsJob: обработка физики транспорта
- WheelPhysicsJob: обработка физики колес
- TerrainDeformationJob: обработка деформации террейна
- NetworkSyncJob: синхронизация сетевых данных

Всегда используй Dependency для правильного порядка выполнения.
Всегда используй ScheduleParallel для параллельной обработки.
Всегда используй Allocator.TempJob для временных массивов.
";
        
        #endregion
        
        #region System Groups Context
        
        /// <summary>
        /// Контекст для работы с группами систем
        /// </summary>
        public const string SystemGroupsContext = @"
@context System Groups:
- InitializationSystemGroup: инициализация (выполняется первыми)
- FixedStepSimulationSystemGroup: основная симуляция (детерминированная)
- SimulationSystemGroup: обычная симуляция
- LateSimulationSystemGroup: поздняя симуляция
- PresentationSystemGroup: презентация (UI, рендеринг)

Порядок выполнения:
1. InitializationSystemGroup: GameBootstrapSystem, VehicleInputSystem
2. FixedStepSimulationSystemGroup: VehicleControlSystem, EngineSystem, Physics
3. LateSimulationSystemGroup: VehicleCameraSystem
4. PresentationSystemGroup: UI системы

Атрибуты систем:
- [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]: для детерминированных систем
- [UpdateAfter(typeof(OtherSystem))]: для порядка выполнения
- [UpdateBefore(typeof(OtherSystem))]: для порядка выполнения

Всегда используй FixedStepSimulationSystemGroup для физики.
Всегда используй SystemAPI.Time.fixedDeltaTime для детерминизма.
";
        
        #endregion
        
        #region Performance Context
        
        /// <summary>
        /// Контекст для оптимизации производительности
        /// </summary>
        public const string PerformanceContext = @"
@context Performance Optimization:
- BurstCompile: компиляция в нативный код для максимальной производительности
- Job System: параллельная обработка на всех ядрах CPU
- EntityQuery: оптимизированные запросы сущностей
- Chunk-based processing: обработка по чанкам для кэш-эффективности
- SIMD operations: векторные операции для ускорения вычислений

Оптимизированные системы:
- OptimizedVehicleMovementSystem: оптимизированное движение
- OptimizedWheelPhysicsSystem: оптимизированная физика колес
- ChunkOptimizedVehicleMovementSystem: chunk-based обработка
- SIMDOptimizedVehicleMovementSystem: SIMD оптимизация

Всегда используй BurstOptimizedMath для быстрых математических операций.
Всегда используй NativeArray для больших данных.
Всегда профилируй критический код.
";
        
        #endregion
        
        #region Complete ECS Context
        
        /// <summary>
        /// Полный контекст для работы с ECS
        /// </summary>
        public static string GetCompleteECSContext()
        {
            return $@"
{CoreComponentsContext}

{VehicleComponentsContext}

{NetworkingComponentsContext}

{TerrainComponentsContext}

{JobSystemContext}

{SystemGroupsContext}

{PerformanceContext}

@context ECS Best Practices:
- ВСЕГДА используй ECS архитектуру, НЕ MonoBehaviour
- ВСЕГДА используй BurstCompile для производительности
- ВСЕГДА используй Job System для параллельной обработки
- ВСЕГДА используй FixedStepSimulationSystemGroup для детерминизма
- ВСЕГДА используй using static для прямого доступа к компонентам
- ВСЕГДА используй EntityQuery для оптимизированных запросов
- ВСЕГДА используй Dependency для правильного порядка выполнения
- ВСЕГДА используй SystemAPI.Time.fixedDeltaTime для детерминированных вычислений
- ВСЕГДА используй LocalTransform для позиций в Unity DOTS
- ВСЕГДА используй Namespace MudLike.Module.Submodule для организации кода
";
        }
        
        #endregion
    }
}
