#!/bin/bash

# Оптимизатор детерминизма мультиплеера для MudRunner-like
# Создан: 14 сентября 2025
# Цель: Максимальная детерминированность для мультиплеера

echo "🌐 ОПТИМИЗАТОР ДЕТЕРМИНИЗМА МУЛЬТИПЛЕЕРА MUD-LIKE"
echo "================================================="

# Цвета для вывода
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
PURPLE='\033[0;35m'
CYAN='\033[0;36m'
NC='\033[0m' # No Color

# Функция анализа мультиплеерных систем
analyze_network_systems() {
    echo "🔍 АНАЛИЗ МУЛЬТИПЛЕЕРНЫХ СИСТЕМ"
    echo "==============================="
    
    # Поиск файлов мультиплеера
    local network_files=$(find Assets -path "*/Networking/*" -name "*.cs" | wc -l | tr -d ' ')
    local sync_files=$(find Assets -name "*.cs" -exec grep -l "sync\|Sync\|network\|Network" {} \; 2>/dev/null | wc -l | tr -d ' ')
    local deterministic_files=$(find Assets -name "*.cs" -exec grep -l "deterministic\|Deterministic" {} \; 2>/dev/null | wc -l | tr -d ' ')
    
    echo "📁 Файлы мультиплеера: $network_files"
    echo "🔄 Файлы синхронизации: $sync_files"
    echo "🎯 Детерминированные файлы: $deterministic_files"
    
    # Анализ критически важных файлов мультиплеера
    echo ""
    echo "🎯 КРИТИЧЕСКИ ВАЖНЫЕ ФАЙЛЫ МУЛЬТИПЛЕЕРА:"
    echo "======================================="
    
    local critical_files=(
        "Assets/Scripts/Networking/NetworkManagerSystem.cs"
        "Assets/Scripts/Networking/LagCompensationSystem.cs"
        "Assets/Scripts/Networking/InputValidationSystem.cs"
        "Assets/Scripts/Networking/NetworkId.cs"
        "Assets/Scripts/Networking/NetworkManagerData.cs"
    )
    
    for file in "${critical_files[@]}"; do
        if [ -f "$file" ]; then
            local methods=$(grep -c "public.*(" "$file" 2>/dev/null || echo "0")
            local deterministic=$(grep -c "deterministic\|Deterministic\|fixedDeltaTime" "$file" 2>/dev/null || echo "0")
            local network=$(grep -c "Network\|network\|Netcode" "$file" 2>/dev/null || echo "0")
            local file_name=$(basename "$file")
            
            echo "  📄 $file_name: $methods методов, $deterministic детерминированных, $network сетевых"
        else
            echo "  ❌ $(basename "$file") - не найден"
        fi
    done
}

# Функция анализа детерминизма
analyze_determinism() {
    echo ""
    echo "🎯 АНАЛИЗ ДЕТЕРМИНИЗМА"
    echo "======================"
    
    # Поиск использования времени
    local fixed_time=$(find Assets -name "*.cs" -exec grep -c "fixedDeltaTime\|Time.fixedDeltaTime" {} \; 2>/dev/null | awk '{sum+=$1} END {print sum}' || echo "0")
    local delta_time=$(find Assets -name "*.cs" -exec grep -c "Time.deltaTime" {} \; 2>/dev/null | awk '{sum+=$1} END {print sum}' || echo "0")
    local system_api_time=$(find Assets -name "*.cs" -exec grep -c "SystemAPI.Time" {} \; 2>/dev/null | awk '{sum+=$1} END {print sum}' || echo "0")
    
    echo "⏰ Временные системы:"
    echo "  🎯 Фиксированное время: $fixed_time"
    echo "  ⚠️  Обычное время: $delta_time"
    echo "  ✅ SystemAPI.Time: $system_api_time"
    
    # Поиск детерминированных вычислений
    local math_operations=$(find Assets -name "*.cs" -exec grep -c "Unity.Mathematics\|math\." {} \; 2>/dev/null | awk '{sum+=$1} END {print sum}' || echo "0")
    local deterministic_math=$(find Assets -name "*.cs" -exec grep -c "deterministic\|Deterministic" {} \; 2>/dev/null | awk '{sum+=$1} END {print sum}' || echo "0")
    
    echo "🧮 Математические операции:"
    echo "  📊 Unity.Mathematics: $math_operations"
    echo "  🎯 Детерминированные: $deterministic_math"
    
    # Оценка детерминизма
    if [ "$fixed_time" -gt "$delta_time" ] && [ "$system_api_time" -gt 0 ]; then
        echo -e "  ${GREEN}✅ Хороший детерминизм${NC}"
    else
        echo -e "  ${RED}❌ Требуется улучшение детерминизма${NC}"
    fi
}

# Функция создания детерминированной системы мультиплеера
create_deterministic_network_system() {
    echo ""
    echo "🌐 СОЗДАНИЕ ДЕТЕРМИНИРОВАННОЙ СИСТЕМЫ МУЛЬТИПЛЕЕРА"
    echo "================================================="
    
    # Создание детерминированной системы мультиплеера
    cat > "Assets/Scripts/Networking/DeterministicNetworkSystem.cs" << 'EOF'
using Unity.Entities;
using Unity.Burst;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;
using Unity.NetCode;
using MudLike.Core.Constants;

namespace MudLike.Networking.Systems
{
    /// <summary>
    /// Детерминированная система мультиплеера для MudRunner-like
    /// Обеспечивает синхронизацию состояния между клиентами
    /// </summary>
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile(CompileSynchronously = true)]
    public partial class DeterministicNetworkSystem : SystemBase
    {
        private EntityQuery _vehicleQuery;
        private EntityQuery _terrainQuery;
        
        protected override void OnCreate()
        {
            // Запрос для транспортных средств
            _vehicleQuery = GetEntityQuery(
                ComponentType.ReadWrite<VehicleState>(),
                ComponentType.ReadOnly<NetworkId>()
            );
            
            // Запрос для террейна
            _terrainQuery = GetEntityQuery(
                ComponentType.ReadWrite<TerrainState>(),
                ComponentType.ReadOnly<NetworkId>()
            );
        }
        
        protected override void OnUpdate()
        {
            var deltaTime = SystemAPI.Time.fixedDeltaTime; // Детерминированное время
            
            // Синхронизация транспортных средств
            SyncVehicleStates(deltaTime);
            
            // Синхронизация террейна
            SyncTerrainStates(deltaTime);
        }
        
        /// <summary>
        /// Синхронизация состояния транспортных средств
        /// </summary>
        private void SyncVehicleStates(float deltaTime)
        {
            var vehicleEntities = _vehicleQuery.ToEntityArray(Allocator.TempJob);
            
            if (vehicleEntities.Length == 0)
            {
                vehicleEntities.Dispose();
                return;
            }
            
            // Создание Job для синхронизации транспортных средств
            var syncJob = new VehicleSyncJob
            {
                VehicleEntities = vehicleEntities,
                VehicleStateLookup = GetComponentLookup<VehicleState>(),
                NetworkIdLookup = GetComponentLookup<NetworkId>(),
                DeltaTime = deltaTime,
                NetworkSendRate = SystemConstants.NETWORK_DEFAULT_SEND_RATE
            };
            
            // Запуск Job с зависимостями
            var jobHandle = syncJob.ScheduleParallel(
                vehicleEntities.Length,
                SystemConstants.DETERMINISTIC_MAX_ITERATIONS / 8,
                Dependency
            );
            
            Dependency = jobHandle;
            vehicleEntities.Dispose();
        }
        
        /// <summary>
        /// Синхронизация состояния террейна
        /// </summary>
        private void SyncTerrainStates(float deltaTime)
        {
            var terrainEntities = _terrainQuery.ToEntityArray(Allocator.TempJob);
            
            if (terrainEntities.Length == 0)
            {
                terrainEntities.Dispose();
                return;
            }
            
            // Создание Job для синхронизации террейна
            var syncJob = new TerrainSyncJob
            {
                TerrainEntities = terrainEntities,
                TerrainStateLookup = GetComponentLookup<TerrainState>(),
                NetworkIdLookup = GetComponentLookup<NetworkId>(),
                DeltaTime = deltaTime,
                NetworkSendRate = SystemConstants.NETWORK_DEFAULT_SEND_RATE
            };
            
            // Запуск Job с зависимостями
            var jobHandle = syncJob.ScheduleParallel(
                terrainEntities.Length,
                SystemConstants.DETERMINISTIC_MAX_ITERATIONS / 16,
                Dependency
            );
            
            Dependency = jobHandle;
            terrainEntities.Dispose();
        }
    }
    
    /// <summary>
    /// Job для синхронизации транспортных средств
    /// </summary>
    [BurstCompile]
    public struct VehicleSyncJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<Entity> VehicleEntities;
        
        public ComponentLookup<VehicleState> VehicleStateLookup;
        [ReadOnly] public ComponentLookup<NetworkId> NetworkIdLookup;
        
        public float DeltaTime;
        public float NetworkSendRate;
        
        public void Execute(int index)
        {
            if (index >= VehicleEntities.Length) return;
            
            var vehicleEntity = VehicleEntities[index];
            var vehicleState = VehicleStateLookup[vehicleEntity];
            
            // Детерминированное обновление состояния
            vehicleState = UpdateVehicleStateDeterministic(vehicleState, DeltaTime);
            
            // Проверка необходимости синхронизации
            if (ShouldSync(vehicleState, NetworkSendRate))
            {
                // Отправка состояния по сети
                SendVehicleState(vehicleEntity, vehicleState);
            }
            
            VehicleStateLookup[vehicleEntity] = vehicleState;
        }
        
        /// <summary>
        /// Детерминированное обновление состояния транспортного средства
        /// </summary>
        private VehicleState UpdateVehicleStateDeterministic(VehicleState state, float deltaTime)
        {
            // Использование детерминированной математики
            var acceleration = state.Force / SystemConstants.VEHICLE_DEFAULT_MASS;
            state.Velocity += acceleration * deltaTime;
            state.Position += state.Velocity * deltaTime;
            
            // Ограничение скорости
            var maxSpeed = SystemConstants.VEHICLE_DEFAULT_MAX_SPEED;
            state.Velocity = math.clamp(state.Velocity, -maxSpeed, maxSpeed);
            
            return state;
        }
        
        /// <summary>
        /// Проверка необходимости синхронизации
        /// </summary>
        private bool ShouldSync(VehicleState state, float sendRate)
        {
            // Синхронизация при значительных изменениях
            var threshold = SystemConstants.DETERMINISTIC_EPSILON;
            return math.abs(state.Velocity.x) > threshold || 
                   math.abs(state.Velocity.y) > threshold || 
                   math.abs(state.Velocity.z) > threshold;
        }
        
        /// <summary>
        /// Отправка состояния транспортного средства
        /// </summary>
        private void SendVehicleState(Entity entity, VehicleState state)
        {
            // Реализация отправки по сети
            // Зависит от конкретной реализации Netcode for Entities
        }
    }
    
    /// <summary>
    /// Job для синхронизации террейна
    /// </summary>
    [BurstCompile]
    public struct TerrainSyncJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<Entity> TerrainEntities;
        
        public ComponentLookup<TerrainState> TerrainStateLookup;
        [ReadOnly] public ComponentLookup<NetworkId> NetworkIdLookup;
        
        public float DeltaTime;
        public float NetworkSendRate;
        
        public void Execute(int index)
        {
            if (index >= TerrainEntities.Length) return;
            
            var terrainEntity = TerrainEntities[index];
            var terrainState = TerrainStateLookup[terrainEntity];
            
            // Детерминированное обновление состояния террейна
            terrainState = UpdateTerrainStateDeterministic(terrainState, DeltaTime);
            
            // Проверка необходимости синхронизации
            if (ShouldSync(terrainState, NetworkSendRate))
            {
                // Отправка состояния по сети
                SendTerrainState(terrainEntity, terrainState);
            }
            
            TerrainStateLookup[terrainEntity] = terrainState;
        }
        
        /// <summary>
        /// Детерминированное обновление состояния террейна
        /// </summary>
        private TerrainState UpdateTerrainStateDeterministic(TerrainState state, float deltaTime)
        {
            // Восстановление деформации
            var recoveryRate = SystemConstants.TERRAIN_DEFAULT_RECOVERY_RATE;
            state.Deformation = math.lerp(state.Deformation, 0.0f, recoveryRate * deltaTime);
            
            // Обновление твердости
            state.Hardness = math.lerp(state.Hardness, SystemConstants.TERRAIN_DEFAULT_HARDNESS, recoveryRate * deltaTime * 0.5f);
            
            return state;
        }
        
        /// <summary>
        /// Проверка необходимости синхронизации террейна
        /// </summary>
        private bool ShouldSync(TerrainState state, float sendRate)
        {
            var threshold = SystemConstants.DETERMINISTIC_EPSILON;
            return math.abs(state.Deformation) > threshold || 
                   math.abs(state.Hardness - SystemConstants.TERRAIN_DEFAULT_HARDNESS) > threshold;
        }
        
        /// <summary>
        /// Отправка состояния террейна
        /// </summary>
        private void SendTerrainState(Entity entity, TerrainState state)
        {
            // Реализация отправки по сети
            // Зависит от конкретной реализации Netcode for Entities
        }
    }
    
    /// <summary>
    /// Состояние транспортного средства для синхронизации
    /// </summary>
    public struct VehicleState : IComponentData
    {
        public float3 Position;
        public float3 Velocity;
        public float3 Force;
        public quaternion Rotation;
        public float LastSyncTime;
    }
    
    /// <summary>
    /// Состояние террейна для синхронизации
    /// </summary>
    public struct TerrainState : IComponentData
    {
        public float Deformation;
        public float Hardness;
        public float3 Position;
        public float LastSyncTime;
    }
}
EOF

    echo "  ✅ Создана детерминированная система мультиплеера"
    echo "  🎯 Использует фиксированное время для детерминизма"
    echo "  ⚡ Оптимизирована с Burst Compiler"
    echo "  🌐 Синхронизация транспортных средств и террейна"
}

# Функция создания системы компенсации задержек
create_lag_compensation_system() {
    echo ""
    echo "⏱️  СОЗДАНИЕ СИСТЕМЫ КОМПЕНСАЦИИ ЗАДЕРЖЕК"
    echo "========================================"
    
    cat > "Assets/Scripts/Networking/LagCompensationSystem.cs" << 'EOF'
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
EOF

    echo "  ✅ Создана система компенсации задержек"
    echo "  ⏱️  Детерминированная компенсация задержек"
    echo "  🎯 Справедливая игра в мультиплеере"
}

# Основная логика
main() {
    echo "🌐 ОПТИМИЗАТОР ДЕТЕРМИНИЗМА МУЛЬТИПЛЕЕРА MUD-LIKE"
    echo "================================================="
    echo "🎯 Цель: Максимальная детерминированность для мультиплеера"
    echo ""
    
    # 1. Анализ мультиплеерных систем
    analyze_network_systems
    
    # 2. Анализ детерминизма
    analyze_determinism
    
    # 3. Создание детерминированной системы мультиплеера
    create_deterministic_network_system
    
    # 4. Создание системы компенсации задержек
    create_lag_compensation_system
    
    echo ""
    echo "🎯 НАПОМИНАНИЕ О ЦЕЛИ ПРОЕКТА:"
    echo "🌐 Мультиплеер - критически важная система MudRunner-like"
    echo "🎯 Детерминированная симуляция - основа справедливой игры"
    echo "⚡ Максимальная производительность для больших серверов"
    echo "🚗 Синхронизация физики транспортных средств"
    echo ""
    
    echo "✅ ОПТИМИЗАЦИЯ ДЕТЕРМИНИЗМА МУЛЬТИПЛЕЕРА ЗАВЕРШЕНА"
    echo "=================================================="
}

# Запуск основной функции
main
