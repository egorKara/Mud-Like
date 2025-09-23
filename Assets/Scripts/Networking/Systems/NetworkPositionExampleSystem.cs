using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;
using Unity.NetCode;
using MudLike.Core.Components;
using MudLike.Networking.Components;
using static MudLike.Core.Components.Position;

namespace MudLike.Networking.Systems
{
    /// <summary>
    /// Пример системы, демонстрирующей использование Position в сетевых системах
    /// </summary>
    [UpdateInGroup(typeof(NetCodeClientAndServerSystemGroup))]
    [BurstCompile(CompileSynchronously = true)]
    public partial class NetworkPositionExampleSystem : SystemBase
    {
        /// <summary>
        /// Обрабатывает синхронизацию позиций в сети
        /// </summary>
        protected override void OnUpdate()
        {
            // Синхронизируем позиции на сервере
            if (HasSingleton<NetworkStreamInGame>())
            {
                SyncServerPositions();
            }
            
            // Синхронизируем позиции на клиенте
            if (HasSingleton<NetworkStreamInGame>() && !HasSingleton<NetworkStreamDriver>())
            {
                SyncClientPositions();
            }
        }
        
        /// <summary>
        /// Синхронизирует позиции на сервере
        /// </summary>
        private void SyncServerPositions()
        {
            Entities
                .WithAll<Position, NetworkId>()
                .ForEach((ref Position position, in NetworkPosition networkPosition, in NetworkId networkId) =>
                {
                    // Сервер авторитетен для позиций
                    // Обновляем локальную позицию на основе сетевой
                    if (networkPosition.HasChanged)
                    {
                        position.Value = networkPosition.Value;
                    }
                }).Schedule();
        }
        
        /// <summary>
        /// Синхронизирует позиции на клиенте
        /// </summary>
        private void SyncClientPositions()
        {
            Entities
                .WithAll<Position, NetworkId>()
                .ForEach((ref Position position, ref NetworkPosition networkPosition, in NetworkId networkId) =>
                {
                    // Клиент отправляет свою позицию на сервер
                    if (HasPositionChanged(position, networkPosition))
                    {
                        networkPosition.Value = position.Value;
                        networkPosition.HasChanged = true;
                        networkPosition.LastUpdateTime = (float)Time.time;
                    }
                }).Schedule();
        }
        
        /// <summary>
        /// Проверяет, изменилась ли позиция
        /// </summary>
        private static bool HasPositionChanged(in Position position, in NetworkPosition networkPosition)
        {
            const float threshold = 0.01f;
            return math.distance(position.Value, networkPosition.Value) > threshold;
        }
    }
    
    /// <summary>
    /// Пример системы для интерполяции позиций в сети
    /// </summary>
    [UpdateInGroup(typeof(NetCodeClientAndServerSystemGroup))]
    [BurstCompile(CompileSynchronously = true)]
    public partial class NetworkPositionInterpolationSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            if (!HasSingleton<NetworkStreamInGame>()) return;
            
            var job = new NetworkPositionInterpolationJob
            {
                DeltaTime = SystemAPI.Time.DeltaTime,
                InterpolationTime = 0.1f // 100ms интерполяция
            };
            
            Dependency = job.ScheduleParallel(Dependency);
        }
        
        /// <summary>
        /// Job для интерполяции сетевых позиций
        /// </summary>
        [BurstCompile]
        public partial struct NetworkPositionInterpolationJob : IJobEntity
        {
            public float DeltaTime;
            public float InterpolationTime;
            
            public void Execute(ref Position position, 
                              in NetworkPosition networkPosition, 
                              in NetworkId networkId)
            {
                // Интерполируем позицию для плавного движения
                InterpolatePosition(ref position, networkPosition);
            }
            
            /// <summary>
            /// Интерполирует позицию для плавного движения
            /// </summary>
            [BurstCompile]
            private void InterpolatePosition(ref Position position, in NetworkPosition networkPosition)
            {
                // Вычисляем целевую позицию
                float3 targetPosition = networkPosition.Value;
                
                // Вычисляем скорость интерполяции
                float interpolationSpeed = 1f / InterpolationTime;
                
                // Интерполируем позицию
                position.Value = math.lerp(position.Value, targetPosition, interpolationSpeed * DeltaTime);
            }
        }
    }
    
    /// <summary>
    /// Пример системы для предсказания позиций
    /// </summary>
    [UpdateInGroup(typeof(NetCodeClientAndServerSystemGroup))]
    [BurstCompile(CompileSynchronously = true)]
    public partial class NetworkPositionPredictionSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            if (!HasSingleton<NetworkStreamInGame>()) return;
            
            var job = new NetworkPositionPredictionJob
            {
                DeltaTime = SystemAPI.Time.fixedDeltaTime,
                PredictionTime = 0.05f // 50ms предсказание
            };
            
            Dependency = job.ScheduleParallel(Dependency);
        }
        
        /// <summary>
        /// Job для предсказания сетевых позиций
        /// </summary>
        [BurstCompile]
        public partial struct NetworkPositionPredictionJob : IJobEntity
        {
            public float DeltaTime;
            public float PredictionTime;
            
            public void Execute(ref Position position, 
                              in NetworkPosition networkPosition, 
                              in Velocity velocity,
                              in NetworkId networkId)
            {
                // Предсказываем позицию для компенсации задержки
                PredictPosition(ref position, networkPosition, velocity);
            }
            
            /// <summary>
            /// Предсказывает позицию для компенсации задержки
            /// </summary>
            [BurstCompile]
            private void PredictPosition(ref Position position, 
                                       in NetworkPosition networkPosition, 
                                       in Velocity velocity)
            {
                // Вычисляем предсказанную позицию
                float3 predictedPosition = networkPosition.Value + velocity.Value * PredictionTime;
                
                // Применяем предсказание с весовым коэффициентом
                float predictionWeight = 0.7f; // 70% предсказание, 30% текущая позиция
                position.Value = math.lerp(position.Value, predictedPosition, predictionWeight);
            }
        }
    }
    
    /// <summary>
    /// Пример системы для валидации позиций
    /// </summary>
    [UpdateInGroup(typeof(NetCodeClientAndServerSystemGroup))]
    [BurstCompile(CompileSynchronously = true)]
    public partial class NetworkPositionValidationSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            if (!HasSingleton<NetworkStreamInGame>()) return;
            
            var job = new NetworkPositionValidationJob
            {
                DeltaTime = SystemAPI.Time.fixedDeltaTime,
                MaxSpeed = 100f, // Максимальная скорость
                ValidationRadius = 1000f // Радиус валидации
            };
            
            Dependency = job.ScheduleParallel(Dependency);
        }
        
        /// <summary>
        /// Job для валидации сетевых позиций
        /// </summary>
        [BurstCompile]
        public partial struct NetworkPositionValidationJob : IJobEntity
        {
            public float DeltaTime;
            public float MaxSpeed;
            public float ValidationRadius;
            
            public void Execute(ref Position position, 
                              in NetworkPosition networkPosition, 
                              in NetworkId networkId)
            {
                // Валидируем позицию для предотвращения читерства
                ValidatePosition(ref position, networkPosition);
            }
            
            /// <summary>
            /// Валидирует позицию для предотвращения читерства
            /// </summary>
            [BurstCompile]
            private void ValidatePosition(ref Position position, in NetworkPosition networkPosition)
            {
                // Проверяем максимальную скорость
                float distance = math.distance(position.Value, networkPosition.Value);
                float maxDistance = MaxSpeed * DeltaTime;
                
                if (distance > maxDistance)
                {
                    // Позиция подозрительна, корректируем
                    position.Value = networkPosition.Value;
                }
                
                // Проверяем границы мира
                if (math.length(position.Value) > ValidationRadius)
                {
                    // Позиция за пределами мира, телепортируем в центр
                    position.Value = float3.zero;
                }
            }
        }
    }
}
