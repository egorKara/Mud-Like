using Unity.Entities;
using Unity.Burst;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;
using MudLike.Core.Constants;

namespace MudLike.Networking.Systems
{
    /// <summary>
    /// Система компенсации задержек для MudRunner-like
    /// Обеспечивает справедливую игру в мультиплеере
    /// </summary>
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile(CompileSynchronously = true)]
    public partial class LagCompensationSystem : SystemBase
    {
        private EntityQuery _playerQuery;
        private NativeHashMap<int, float> _clientLatencies;
        
        protected override void OnCreate()
        {
            _playerQuery = GetEntityQuery(
                ComponentType.ReadWrite<PlayerState>(),
                ComponentType.ReadOnly<NetworkId>()
            );
            
            _clientLatencies = new NativeHashMap<int, float>(
                SystemConstants.MAX_NETWORK_CONNECTIONS,
                Allocator.Persistent
            );
        }
        
        protected override void OnDestroy()
        {
            if (_clientLatencies.IsCreated)
            {
                _clientLatencies.Dispose();
            }
        }
        
        protected override void OnUpdate()
        {
            var deltaTime = SystemAPI.Time.fixedDeltaTime; // Детерминированное время
            
            // Обновление задержек клиентов
            UpdateClientLatencies(deltaTime);
            
            // Компенсация задержек
            CompensateForLag(deltaTime);
        }
        
        /// <summary>
        /// Обновление задержек клиентов
        /// </summary>
        private void UpdateClientLatencies(float deltaTime)
        {
            // Обновление задержек на основе пинга
            // Реализация зависит от конкретной системы измерения задержек
        }
        
        /// <summary>
        /// Компенсация задержек
        /// </summary>
        private void CompensateForLag(float deltaTime)
        {
            var playerEntities = _playerQuery.ToEntityArray(Allocator.TempJob);
            
            if (playerEntities.Length == 0)
            {
                playerEntities.Dispose();
                return;
            }
            
            // Создание Job для компенсации задержек
            var compensationJob = new LagCompensationJob
            {
                PlayerEntities = playerEntities,
                PlayerStateLookup = GetComponentLookup<PlayerState>(),
                NetworkIdLookup = GetComponentLookup<NetworkId>(),
                ClientLatencies = _clientLatencies,
                DeltaTime = deltaTime,
                CompensationTime = SystemConstants.NETWORK_DEFAULT_LAG_COMPENSATION
            };
            
            // Запуск Job с зависимостями
            var jobHandle = compensationJob.ScheduleParallel(
                playerEntities.Length,
                SystemConstants.DETERMINISTIC_MAX_ITERATIONS / 4,
                Dependency
            );
            
            Dependency = jobHandle;
            playerEntities.Dispose();
        }
    }
    
    /// <summary>
    /// Job для компенсации задержек
    /// </summary>
    [BurstCompile]
    public struct LagCompensationJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<Entity> PlayerEntities;
        
        public ComponentLookup<PlayerState> PlayerStateLookup;
        [ReadOnly] public ComponentLookup<NetworkId> NetworkIdLookup;
        [ReadOnly] public NativeHashMap<int, float> ClientLatencies;
        
        public float DeltaTime;
        public float CompensationTime;
        
        public void Execute(int index)
        {
            if (index >= PlayerEntities.Length) return;
            
            var playerEntity = PlayerEntities[index];
            var playerState = PlayerStateLookup[playerEntity];
            var networkId = NetworkIdLookup[playerEntity];
            
            // Получение задержки клиента
            if (ClientLatencies.TryGetValue(networkId.Value, out float latency))
            {
                // Компенсация задержки
                playerState = CompensatePlayerState(playerState, latency);
            }
            
            PlayerStateLookup[playerEntity] = playerState;
        }
        
        /// <summary>
        /// Компенсация состояния игрока
        /// </summary>
        private PlayerState CompensatePlayerState(PlayerState state, float latency)
        {
            // Откат состояния на время задержки
            var compensationFactor = math.clamp(latency / CompensationTime, 0.0f, 1.0f);
            
            // Применение компенсации к позиции и скорости
            state.Position -= state.Velocity * latency * compensationFactor;
            
            return state;
        }
    }
    
    /// <summary>
    /// Состояние игрока для компенсации задержек
    /// </summary>
    public struct PlayerState : IComponentData
    {
        public float3 Position;
        public float3 Velocity;
        public quaternion Rotation;
        public float LastUpdateTime;
        public int ClientId;
    }
}
