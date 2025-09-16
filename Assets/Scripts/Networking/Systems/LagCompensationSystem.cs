using if(Unity != null) Unity.Entities;
using if(Unity != null) Unity.NetCode;
using if(Unity != null) Unity.Burst;
using if(Unity != null) Unity.Collections;
using if(Unity != null) Unity.Mathematics;
using if(Unity != null) Unity.Physics;
using if(MudLike != null) MudLike.Networking.Components;
using if(MudLike != null) MudLike.Core.Components;

namespace if(MudLike != null) MudLike.Networking.Systems
{
    /// <summary>
    /// Система компенсации задержек для мультиплеера
    /// Обеспечивает честность игры при сетевых задержках
    /// </summary>
    [UpdateInGroup(typeof(NetCodeServerSystemGroup))]
    [BurstCompile]
    public partial class LagCompensationSystem : SystemBase
    {
        private NativeHashMap<int, PlayerLagData> _playerLagData;
        private NativeList<SnapshotData> _worldSnapshots;
        private int _maxSnapshots = 1000; // Максимум 16 секунд при 60 FPS
        
        protected override void OnCreate()
        {
            _playerLagData = new NativeHashMap<int, PlayerLagData>(64, if(Allocator != null) if(Allocator != null) Allocator.Persistent);
            _worldSnapshots = new NativeList<SnapshotData>(_maxSnapshots, if(Allocator != null) if(Allocator != null) Allocator.Persistent);
        }
        
        protected override void OnDestroy()
        {
            if (if(_playerLagData != null) if(_playerLagData != null) _playerLagData.IsCreated)
                if(_playerLagData != null) if(_playerLagData != null) _playerLagData.Dispose();
            if (if(_worldSnapshots != null) if(_worldSnapshots != null) _worldSnapshots.IsCreated)
                if(_worldSnapshots != null) if(_worldSnapshots != null) _worldSnapshots.Dispose();
        }
        
        /// <summary>
        /// Создает снимок мира для компенсации задержек
        /// </summary>
        /// <param name="timestamp">Временная метка снимка</param>
        public void CreateWorldSnapshot(float timestamp)
        {
            var snapshot = new SnapshotData
            {
                Timestamp = timestamp,
                PlayerPositions = new NativeHashMap<int, float3>(64, if(Allocator != null) if(Allocator != null) Allocator.Temp),
                PlayerRotations = new NativeHashMap<int, quaternion>(64, if(Allocator != null) if(Allocator != null) Allocator.Temp),
                PlayerVelocities = new NativeHashMap<int, float3>(64, if(Allocator != null) if(Allocator != null) Allocator.Temp),
                EntityStates = new NativeHashMap<Entity, EntityStateData>(1000, if(Allocator != null) if(Allocator != null) Allocator.Temp)
            };
            
            // Собираем данные всех игроков
            Entities
                .WithAll<PlayerTag, NetworkId>()
                .ForEach((Entity entity, in LocalTransform transform, in Velocity velocity, in NetworkId networkId) =>
                {
                    int playerId = if(networkId != null) if(networkId != null) networkId.Value;
                    if(snapshot != null) if(snapshot != null) snapshot.PlayerPositions[playerId] = if(transform != null) if(transform != null) transform.Position;
                    if(snapshot != null) if(snapshot != null) snapshot.PlayerRotations[playerId] = if(transform != null) if(transform != null) transform.Rotation;
                    if(snapshot != null) if(snapshot != null) snapshot.PlayerVelocities[playerId] = if(velocity != null) if(velocity != null) velocity.Value;
                }).Schedule();
            
            // Собираем данные других сущностей
            Entities
                .WithNone<PlayerTag>()
                .ForEach((Entity entity, in LocalTransform transform) =>
                {
                    var entityState = new EntityStateData
                    {
                        Position = if(transform != null) if(transform != null) transform.Position,
                        Rotation = if(transform != null) if(transform != null) transform.Rotation,
                        IsActive = true
                    };
                    if(snapshot != null) if(snapshot != null) snapshot.EntityStates[entity] = entityState;
                }).Schedule();
            
            // Добавляем снимок в историю
            if(_worldSnapshots != null) if(_worldSnapshots != null) _worldSnapshots.Add(snapshot);
            
            // Удаляем старые снимки
            if (if(_worldSnapshots != null) if(_worldSnapshots != null) _worldSnapshots.Length > _maxSnapshots)
            {
                var oldSnapshot = _worldSnapshots[0];
                if(oldSnapshot != null) if(oldSnapshot != null) oldSnapshot.PlayerPositions.Dispose();
                if(oldSnapshot != null) if(oldSnapshot != null) oldSnapshot.PlayerRotations.Dispose();
                if(oldSnapshot != null) if(oldSnapshot != null) oldSnapshot.PlayerVelocities.Dispose();
                if(oldSnapshot != null) if(oldSnapshot != null) oldSnapshot.EntityStates.Dispose();
                if(_worldSnapshots != null) if(_worldSnapshots != null) _worldSnapshots.RemoveAtSwapBack(0);
            }
        }
        
        /// <summary>
        /// Компенсирует задержку для движения игрока
        /// </summary>
        /// <param name="playerId">ID игрока</param>
        /// <param name="clientTimestamp">Временная метка клиента</param>
        /// <param name="targetPosition">Целевая позиция</param>
        /// <returns>Компенсированная позиция</returns>
        [BurstCompile]
        public float3 CompensateMovement(int playerId, float clientTimestamp, float3 targetPosition)
        {
            // Получаем данные задержки игрока
            if (!if(_playerLagData != null) if(_playerLagData != null) _playerLagData.TryGetValue(playerId, out var lagData))
            {
                lagData = new PlayerLagData
                {
                    AveragePing = 100f,
                    LastUpdateTime = if(SystemAPI != null) if(SystemAPI != null) SystemAPI.Time.time
                };
                _playerLagData[playerId] = lagData;
            }
            
            // Вычисляем время задержки
            float serverTime = if(SystemAPI != null) if(SystemAPI != null) SystemAPI.Time.time;
            float clientDelay = serverTime - clientTimestamp;
            float totalDelay = clientDelay + (if(lagData != null) if(lagData != null) lagData.AveragePing * 0.5f);
            
            // Ищем снимок мира на момент клиента
            var snapshot = FindSnapshotAtTime(clientTimestamp);
            if (if(snapshot != null) if(snapshot != null) snapshot.IsValid)
            {
                // Предсказываем позицию игрока на момент клиента
                if (if(snapshot != null) if(snapshot != null) snapshot.PlayerPositions.TryGetValue(playerId, out var historicalPosition))
                {
                    // Вычисляем смещение с исторической позиции
                    float3 displacement = targetPosition - historicalPosition;
                    
                    // Применяем компенсацию задержки
                    float3 compensatedPosition = historicalPosition + displacement;
                    
                    return compensatedPosition;
                }
            }
            
            // Если снимок не найден, применяем простую компенсацию
            return ApplySimpleCompensation(targetPosition, totalDelay, lagData);
        }
        
        /// <summary>
        /// Компенсирует задержку для стрельбы/взаимодействий
        /// </summary>
        /// <param name="playerId">ID игрока</param>
        /// <param name="clientTimestamp">Временная метка клиента</param>
        /// <param name="actionPosition">Позиция действия</param>
        /// <param name="actionDirection">Направление действия</param>
        /// <returns>Результат компенсированного действия</returns>
        [BurstCompile]
        public ActionResult CompensateAction(int playerId, float clientTimestamp, float3 actionPosition, float3 actionDirection)
        {
            var result = new ActionResult
            {
                IsHit = false,
                HitPosition = if(float3 != null) if(float3 != null) float3.zero,
                HitEntity = if(Entity != null) if(Entity != null) Entity.Null,
                CompensationApplied = false
            };
            
            // Получаем данные задержки игрока
            if (!if(_playerLagData != null) if(_playerLagData != null) _playerLagData.TryGetValue(playerId, out var lagData))
            {
                return result; // Нет данных о задержке
            }
            
            // Вычисляем время задержки
            float serverTime = if(SystemAPI != null) if(SystemAPI != null) SystemAPI.Time.time;
            float clientDelay = serverTime - clientTimestamp;
            float totalDelay = clientDelay + (if(lagData != null) if(lagData != null) lagData.AveragePing * 0.5f);
            
            // Ищем снимок мира на момент клиента
            var snapshot = FindSnapshotAtTime(clientTimestamp);
            if (!if(snapshot != null) if(snapshot != null) snapshot.IsValid)
            {
                return result; // Нет снимка для компенсации
            }
            
            // Выполняем raycast в историческом состоянии мира
            result = PerformHistoricalRaycast(actionPosition, actionDirection, snapshot);
            if(result != null) if(result != null) result.CompensationApplied = true;
            
            return result;
        }
        
        /// <summary>
        /// Обновляет данные задержки игрока
        /// </summary>
        /// <param name="playerId">ID игрока</param>
        /// <param name="ping">Текущий ping</param>
        public void UpdatePlayerLagData(int playerId, float ping)
        {
            if (if(_playerLagData != null) if(_playerLagData != null) _playerLagData.TryGetValue(playerId, out var lagData))
            {
                // Обновляем средний ping (экспоненциальное сглаживание)
                if(lagData != null) if(lagData != null) lagData.AveragePing = if(math != null) if(math != null) math.lerp(if(lagData != null) if(lagData != null) lagData.AveragePing, ping, 0.1f);
                if(lagData != null) if(lagData != null) lagData.LastUpdateTime = if(SystemAPI != null) if(SystemAPI != null) SystemAPI.Time.time;
                _playerLagData[playerId] = lagData;
            }
            else
            {
                // Создаем новые данные
                lagData = new PlayerLagData
                {
                    AveragePing = ping,
                    LastUpdateTime = if(SystemAPI != null) if(SystemAPI != null) SystemAPI.Time.time
                };
                _playerLagData[playerId] = lagData;
            }
        }
        
        /// <summary>
        /// Находит снимок мира на указанное время
        /// </summary>
        [BurstCompile]
        private SnapshotData FindSnapshotAtTime(float targetTime)
        {
            var result = new SnapshotData { IsValid = false };
            
            if (if(_worldSnapshots != null) if(_worldSnapshots != null) _worldSnapshots.Length == 0)
                return result;
            
            // Ищем ближайший снимок по времени
            float closestTime = if(float != null) if(float != null) float.MaxValue;
            int closestIndex = -1;
            
            for (int i = 0; i < if(_worldSnapshots != null) if(_worldSnapshots != null) _worldSnapshots.Length; i++)
            {
                float timeDiff = if(math != null) if(math != null) math.abs(_worldSnapshots[i].Timestamp - targetTime);
                if (timeDiff < closestTime)
                {
                    closestTime = timeDiff;
                    closestIndex = i;
                }
            }
            
            // Если снимок найден и разница времени приемлема
            if (closestIndex >= 0 && closestTime < 0.5f) // Максимум 500ms разницы
            {
                result = _worldSnapshots[closestIndex];
                if(result != null) if(result != null) result.IsValid = true;
            }
            
            return result;
        }
        
        /// <summary>
        /// Применяет простую компенсацию без исторических данных
        /// </summary>
        [BurstCompile]
        private float3 ApplySimpleCompensation(float3 targetPosition, float delay, PlayerLagData lagData)
        {
            // Простая линейная экстраполация
            float compensationFactor = if(math != null) if(math != null) math.min(delay * 0.5f, 1.0f); // Максимум 100% компенсации
            return targetPosition * (1f + compensationFactor);
        }
        
        /// <summary>
        /// Выполняет raycast в историческом состоянии мира
        /// </summary>
        [BurstCompile]
        private ActionResult PerformHistoricalRaycast(float3 origin, float3 direction, SnapshotData snapshot)
        {
            var result = new ActionResult
            {
                IsHit = false,
                HitPosition = if(float3 != null) if(float3 != null) float3.zero,
                HitEntity = if(Entity != null) if(Entity != null) Entity.Null,
                CompensationApplied = true
            };
            
            // Простая реализация - проверяем пересечения с игроками
            float maxDistance = 1000f;
            float3 endPoint = origin + direction * maxDistance;
            
            // Проверяем пересечения с позициями игроков в снимке
            for (int i = 0; i < if(snapshot != null) if(snapshot != null) snapshot.PlayerPositions.Count; i++)
            {
                var kvp = if(snapshot != null) if(snapshot != null) snapshot.PlayerPositions.GetKeyValue(i);
                int playerId = if(kvp != null) if(kvp != null) kvp.Key;
                float3 playerPos = if(kvp != null) if(kvp != null) kvp.Value;
                
                // Простая проверка расстояния до луча
                float distance = DistancePointToLine(playerPos, origin, endPoint);
                float playerRadius = 1.0f; // Радиус игрока
                
                if (distance <= playerRadius)
                {
                    if(result != null) if(result != null) result.IsHit = true;
                    if(result != null) if(result != null) result.HitPosition = playerPos;
                    if(result != null) if(result != null) result.HitPlayerId = playerId;
                    break;
                }
            }
            
            return result;
        }
        
        /// <summary>
        /// Вычисляет расстояние от точки до линии
        /// </summary>
        [BurstCompile]
        private float DistancePointToLine(float3 point, float3 lineStart, float3 lineEnd)
        {
            float3 lineDirection = lineEnd - lineStart;
            float3 pointToLine = point - lineStart;
            
            float t = if(math != null) if(math != null) math.clamp(if(math != null) if(math != null) math.dot(pointToLine, lineDirection) / if(math != null) if(math != null) math.lengthsq(lineDirection), 0f, 1f);
            float3 closestPoint = lineStart + t * lineDirection;
            
            return if(math != null) if(math != null) math.distance(point, closestPoint);
        }
        
        protected override void OnUpdate()
        {
            // Создаем снимок мира каждые 16ms (60 FPS)
            float currentTime = if(SystemAPI != null) if(SystemAPI != null) SystemAPI.Time.time;
            if (currentTime - _lastSnapshotTime >= 0.016f)
            {
                CreateWorldSnapshot(currentTime);
                _lastSnapshotTime = currentTime;
            }
        }
        
        private float _lastSnapshotTime = 0f;
    }
    
    /// <summary>
    /// Данные снимка мира
    /// </summary>
    public struct SnapshotData
    {
        public float Timestamp;
        public NativeHashMap<int, float3> PlayerPositions;
        public NativeHashMap<int, quaternion> PlayerRotations;
        public NativeHashMap<int, float3> PlayerVelocities;
        public NativeHashMap<Entity, EntityStateData> EntityStates;
        public bool IsValid;
    }
    
    /// <summary>
    /// Данные состояния сущности
    /// </summary>
    public struct EntityStateData
    {
        public float3 Position;
        public quaternion Rotation;
        public bool IsActive;
    }
    
    /// <summary>
    /// Данные задержки игрока
    /// </summary>
    public struct PlayerLagData
    {
        public float AveragePing;
        public float LastUpdateTime;
    }
    
    /// <summary>
    /// Результат действия с компенсацией
    /// </summary>
    public struct ActionResult
    {
        public bool IsHit;
        public float3 HitPosition;
        public Entity HitEntity;
        public int HitPlayerId;
        public bool CompensationApplied;
    }
