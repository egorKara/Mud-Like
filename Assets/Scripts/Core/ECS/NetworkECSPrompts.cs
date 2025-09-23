using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;
using Unity.Jobs;
using Unity.Collections;
using Unity.NetCode;
using MudLike.Networking.Components;
using MudLike.Core.Components;
using static MudLike.Core.Components.Position;

namespace MudLike.Core.ECS
{
    /// <summary>
    /// Контекстные промпты для работы с сетевыми ECS модулями
    /// </summary>
    public static class NetworkECSPrompt
    {
        /// <summary>
        /// Контекст для создания сетевых систем
        /// </summary>
        public const string NetworkSystemContext = @"
@context Network System Creation:
Используй следующие компоненты для сетевых систем:

Сетевые компоненты:
- NetworkId: сетевой идентификатор (Value, LastUpdateTime)
- NetworkPosition: сетевая позиция (Value, Rotation, HasChanged, LastUpdateTime)
- NetworkVehicle: сетевые данные транспорта (Config, Physics, Input, Engine, Transmission)
- NetworkDeformation: сетевые данные деформации (Deformation, HasChanged)
- NetworkMud: сетевые данные грязи (Mud, HasChanged)

Сетевые системы:
- NetworkSyncSystem: синхронизация сетевых данных
- NetworkPositionInterpolationSystem: интерполяция позиций
- NetworkPositionPredictionSystem: предсказание позиций
- NetworkPositionValidationSystem: валидация позиций
- LagCompensationSystem: компенсация задержек
- AntiCheatSystem: античит система

Пример создания сетевой системы:
```csharp
[UpdateInGroup(typeof(NetCodeClientAndServerSystemGroup))]
[BurstCompile(CompileSynchronously = true)]
public partial class YourNetworkSystem : SystemBase
{
    protected override void OnUpdate()
    {
        if (!HasSingleton<NetworkStreamInGame>()) return;
        
        // Синхронизируем на сервере
        if (HasSingleton<NetworkStreamDriver>())
        {
            SyncServerData();
        }
        // Синхронизируем на клиенте
        else
        {
            SyncClientData();
        }
    }
}
```

Всегда используй:
- NetCodeClientAndServerSystemGroup для сетевых систем
- HasSingleton<NetworkStreamInGame>() для проверки подключения
- HasSingleton<NetworkStreamDriver>() для проверки сервера
- Детерминированные вычисления для синхронизации
";

        /// <summary>
        /// Контекст для синхронизации позиций
        /// </summary>
        public const string PositionSyncContext = @"
@context Position Synchronization:
Используй следующие техники для синхронизации позиций:

Синхронизация позиций:
- NetworkPosition.Value: сетевая позиция
- LocalTransform.Position: локальная позиция
- HasChanged: флаг изменения
- LastUpdateTime: время последнего обновления

Пример синхронизации:
```csharp
private void SyncPositions()
{
    Entities
        .WithAll<NetworkPosition, NetworkId>()
        .ForEach((ref NetworkPosition networkPos, 
                 in LocalTransform transform, 
                 in NetworkId networkId) =>
        {
            // Проверяем, изменилась ли позиция
            if (HasPositionChanged(networkPos, transform))
            {
                // Обновляем сетевые данные
                networkPos.Value = transform.Position;
                networkPos.Rotation = transform.Rotation;
                networkPos.HasChanged = true;
                networkPos.LastUpdateTime = (float)Time.time;
            }
        }).Schedule();
}

private static bool HasPositionChanged(in NetworkPosition networkPos, in LocalTransform transform)
{
    const float threshold = 0.01f;
    return math.distance(networkPos.Value, transform.Position) > threshold;
}
```

Всегда используй:
- Пороговые значения для оптимизации
- Флаги HasChanged для отслеживания изменений
- Временные метки для валидации
- Детерминированные вычисления
";

        /// <summary>
        /// Контекст для интерполяции позиций
        /// </summary>
        public const string PositionInterpolationContext = @"
@context Position Interpolation:
Используй следующие техники для интерполяции позиций:

Интерполяция позиций:
- math.lerp: линейная интерполяция
- InterpolationTime: время интерполяции
- DeltaTime: время между кадрами

Пример интерполяции:
```csharp
[BurstCompile]
public partial struct PositionInterpolationJob : IJobEntity
{
    public float DeltaTime;
    public float InterpolationTime;
    
    public void Execute(ref Position position, in NetworkPosition networkPosition)
    {
        // Вычисляем целевую позицию
        float3 targetPosition = networkPosition.Value;
        
        // Вычисляем скорость интерполяции
        float interpolationSpeed = 1f / InterpolationTime;
        
        // Интерполируем позицию
        position.Value = math.lerp(position.Value, targetPosition, interpolationSpeed * DeltaTime);
    }
}
```

Всегда используй:
- Плавную интерполяцию для комфорта
- Настраиваемое время интерполяции
- Ограничение скорости интерполяции
- Проверку на валидность данных
";

        /// <summary>
        /// Контекст для предсказания позиций
        /// </summary>
        public const string PositionPredictionContext = @"
@context Position Prediction:
Используй следующие техники для предсказания позиций:

Предсказание позиций:
- Velocity: скорость для предсказания
- PredictionTime: время предсказания
- Lag compensation: компенсация задержки

Пример предсказания:
```csharp
[BurstCompile]
public partial struct PositionPredictionJob : IJobEntity
{
    public float DeltaTime;
    public float PredictionTime;
    
    public void Execute(ref Position position, 
                      in NetworkPosition networkPosition, 
                      in Velocity velocity)
    {
        // Вычисляем предсказанную позицию
        float3 predictedPosition = networkPosition.Value + velocity.Value * PredictionTime;
        
        // Применяем предсказание с весовым коэффициентом
        float predictionWeight = 0.7f;
        position.Value = math.lerp(position.Value, predictedPosition, predictionWeight);
    }
}
```

Всегда используй:
- Весовые коэффициенты для предсказания
- Ограничение времени предсказания
- Валидацию предсказанных данных
- Коррекцию при расхождении
";

        /// <summary>
        /// Контекст для валидации позиций
        /// </summary>
        public const string PositionValidationContext = @"
@context Position Validation:
Используй следующие техники для валидации позиций:

Валидация позиций:
- MaxSpeed: максимальная скорость
- ValidationRadius: радиус валидации
- Threshold: пороговые значения

Пример валидации:
```csharp
[BurstCompile]
public partial struct PositionValidationJob : IJobEntity
{
    public float DeltaTime;
    public float MaxSpeed;
    public float ValidationRadius;
    
    public void Execute(ref Position position, in NetworkPosition networkPosition)
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
            // Позиция за пределами мира
            position.Value = float3.zero;
        }
    }
}
```

Всегда используй:
- Проверку максимальной скорости
- Проверку границ мира
- Коррекцию подозрительных данных
- Логирование нарушений
";

        /// <summary>
        /// Контекст для компенсации задержек
        /// </summary>
        public const string LagCompensationContext = @"
@context Lag Compensation:
Используй следующие техники для компенсации задержек:

Компенсация задержек:
- ClientRTT: время отклика клиента
- ServerTime: время сервера
- Rollback: откат состояния

Пример компенсации:
```csharp
[BurstCompile]
public partial struct LagCompensationJob : IJobEntity
{
    public float ClientRTT;
    public float ServerTime;
    
    public void Execute(ref Position position, in NetworkPosition networkPosition)
    {
        // Вычисляем время компенсации
        float compensationTime = ClientRTT * 0.5f;
        
        // Применяем компенсацию
        float3 compensatedPosition = networkPosition.Value + networkPosition.Velocity * compensationTime;
        
        // Обновляем позицию
        position.Value = compensatedPosition;
    }
}
```

Всегда используй:
- Половину RTT для компенсации
- Валидацию времени компенсации
- Ограничение компенсации
- Сглаживание переходов
";
    }
}
