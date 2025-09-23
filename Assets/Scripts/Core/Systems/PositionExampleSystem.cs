using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;
using MudLike.Core.Components;
using static MudLike.Core.Components.Position;

namespace MudLike.Core.Systems
{
    /// <summary>
    /// Пример системы, демонстрирующей использование using static MudLike.Core.Components.Position
    /// </summary>
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile(CompileSynchronously = true)]
    public partial class PositionExampleSystem : SystemBase
    {
        /// <summary>
        /// Обрабатывает примеры использования Position компонента
        /// </summary>
        protected override void OnUpdate()
        {
            float deltaTime = SystemAPI.Time.fixedDeltaTime;
            
            // Пример 1: Использование Position компонента напрямую
            Entities
                .WithAll<Position>()
                .ForEach((ref Position position, in Velocity velocity) =>
                {
                    // Обновляем позицию на основе скорости
                    // Теперь можно обращаться к Value напрямую благодаря using static
                    position.Value += velocity.Value * deltaTime;
                }).Schedule();
            
            // Пример 2: Создание нового Position компонента
            Entities
                .WithAll<PlayerTag>()
                .WithNone<Position>()
                .ForEach((Entity entity) =>
                {
                    // Создаем новый Position компонент
                    var newPosition = new Position
                    {
                        Value = float3.zero
                    };
                    EntityManager.SetComponentData(entity, newPosition);
                }).WithoutBurst().Run();
            
            // Пример 3: Сравнение позиций
            Entities
                .WithAll<Position, TargetTag>()
                .ForEach((in Position currentPosition, in Position targetPosition) =>
                {
                    // Вычисляем расстояние между позициями
                    float distance = math.distance(currentPosition.Value, targetPosition.Value);
                    
                    // Проверяем, достигли ли цели
                    if (distance < 1.0f)
                    {
                        // Цель достигнута
                        // Здесь можно добавить логику достижения цели
                    }
                }).Schedule();
        }
    }
    
    /// <summary>
    /// Тег для объектов с целевой позицией
    /// </summary>
    public struct TargetTag : IComponentData
    {
    }
    
    /// <summary>
    /// Пример системы, демонстрирующей использование Position в Job
    /// </summary>
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile(CompileSynchronously = true)]
    public partial class PositionJobExampleSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var job = new PositionExampleJob
            {
                DeltaTime = SystemAPI.Time.fixedDeltaTime
            };
            
            Dependency = job.ScheduleParallel(Dependency);
        }
        
        /// <summary>
        /// Job для демонстрации использования Position в параллельных вычислениях
        /// </summary>
        [BurstCompile]
        public partial struct PositionExampleJob : IJobEntity
        {
            public float DeltaTime;
            
            public void Execute(ref Position position, in Velocity velocity, in PlayerTag playerTag)
            {
                // Обновляем позицию игрока
                UpdatePlayerPosition(ref position, velocity);
                
                // Применяем гравитацию
                ApplyGravity(ref position);
                
                // Ограничиваем позицию в пределах мира
                ClampPositionToWorldBounds(ref position);
            }
            
            /// <summary>
            /// Обновляет позицию игрока
            /// </summary>
            [BurstCompile]
            private static void UpdatePlayerPosition(ref Position position, in Velocity velocity)
            {
                // Благодаря using static можно обращаться к Value напрямую
                position.Value += velocity.Value * DeltaTime;
            }
            
            /// <summary>
            /// Применяет гравитацию
            /// </summary>
            [BurstCompile]
            private static void ApplyGravity(ref Position position)
            {
                const float gravity = -9.81f;
                position.Value.y += gravity * DeltaTime * DeltaTime * 0.5f;
            }
            
            /// <summary>
            /// Ограничивает позицию в пределах мира
            /// </summary>
            [BurstCompile]
            private static void ClampPositionToWorldBounds(ref Position position)
            {
                const float worldSize = 1000f;
                const float groundLevel = 0f;
                const float skyLevel = 500f;
                
                // Ограничиваем по X и Z
                position.Value.x = math.clamp(position.Value.x, -worldSize, worldSize);
                position.Value.z = math.clamp(position.Value.z, -worldSize, worldSize);
                
                // Ограничиваем по Y
                position.Value.y = math.clamp(position.Value.y, groundLevel, skyLevel);
            }
        }
    }
    
    /// <summary>
    /// Пример системы для работы с множественными позициями
    /// </summary>
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile(CompileSynchronously = true)]
    public partial class MultiplePositionExampleSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var job = new MultiplePositionJob
            {
                DeltaTime = SystemAPI.Time.fixedDeltaTime
            };
            
            Dependency = job.ScheduleParallel(Dependency);
        }
        
        /// <summary>
        /// Job для работы с множественными позициями
        /// </summary>
        [BurstCompile]
        public partial struct MultiplePositionJob : IJobEntity
        {
            public float DeltaTime;
            
            public void Execute(ref Position position, in WaypointData waypoints)
            {
                // Перемещаем объект по маршруту
                MoveAlongWaypoints(ref position, waypoints);
            }
            
            /// <summary>
            /// Перемещает объект по точкам маршрута
            /// </summary>
            [BurstCompile]
            private static void MoveAlongWaypoints(ref Position position, in WaypointData waypoints)
            {
                if (waypoints.WaypointCount <= 0) return;
                
                // Получаем текущую целевую точку
                int currentWaypoint = waypoints.CurrentWaypointIndex;
                if (currentWaypoint >= waypoints.WaypointCount) return;
                
                // Получаем позицию целевой точки
                float3 targetPosition = waypoints.Waypoints[currentWaypoint];
                
                // Вычисляем направление к цели
                float3 direction = math.normalize(targetPosition - position.Value);
                
                // Перемещаем объект
                float speed = waypoints.MoveSpeed;
                position.Value += direction * speed * DeltaTime;
                
                // Проверяем, достигли ли точки
                float distance = math.distance(position.Value, targetPosition);
                if (distance < waypoints.ArrivalDistance)
                {
                    // Переходим к следующей точке
                    waypoints.CurrentWaypointIndex = (currentWaypoint + 1) % waypoints.WaypointCount;
                }
            }
        }
    }
    
    /// <summary>
    /// Данные маршрута для перемещения объектов
    /// </summary>
    public struct Waypoint : IComponentData
    {
        public int CurrentWaypointIndex;
        public int WaypointCount;
        public float MoveSpeed;
        public float ArrivalDistance;
        public float3 Waypoint1;
        public float3 Waypoint2;
        public float3 Waypoint3;
        public float3 Waypoint4;
        
        // Используем массив для хранения точек маршрута
        public float3 Waypoint(int index)
        {
            switch (index)
            {
                case 0: return Waypoint1;
                case 1: return Waypoint2;
                case 2: return Waypoint3;
                case 3: return Waypoint4;
                default: return float3.zero;
            }
        }
        
        // Индексатор для удобства доступа
        public float3 this[int index] => Waypoint(index);
    }
}
