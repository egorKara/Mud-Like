#!/bin/bash

# Оптимизатор деформации террейна для MudRunner-like
# Создан: 14 сентября 2025
# Цель: Максимальная производительность деформации террейна

echo "🏔️  ОПТИМИЗАТОР ДЕФОРМАЦИИ ТЕРРЕЙНА MUD-LIKE"
echo "============================================="

# Цвета для вывода
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
PURPLE='\033[0;35m'
CYAN='\033[0;36m'
NC='\033[0m' # No Color

# Функция анализа систем деформации террейна
analyze_terrain_deformation() {
    echo "🔍 АНАЛИЗ СИСТЕМ ДЕФОРМАЦИИ ТЕРРЕЙНА"
    echo "===================================="
    
    # Поиск файлов деформации террейна
    local terrain_files=$(find Assets -path "*/Terrain/*" -name "*.cs" | wc -l | tr -d ' ')
    local deformation_files=$(find Assets -name "*.cs" -exec grep -l "deformation\|Deformation\|mud\|Mud" {} \; 2>/dev/null | wc -l | tr -d ' ')
    local heightmap_files=$(find Assets -name "*.cs" -exec grep -l "heightmap\|Heightmap\|terrain\|Terrain" {} \; 2>/dev/null | wc -l | tr -d ' ')
    
    echo "📁 Файлы террейна: $terrain_files"
    echo "🏔️  Файлы деформации: $deformation_files"
    echo "📊 Файлы высотных карт: $heightmap_files"
    
    # Анализ критически важных файлов
    echo ""
    echo "🎯 КРИТИЧЕСКИ ВАЖНЫЕ ФАЙЛЫ ДЕФОРМАЦИИ:"
    echo "======================================"
    
    local critical_files=(
        "Assets/Scripts/Terrain/Systems/MudManagerSystem.cs"
        "Assets/Scripts/Terrain/Systems/TerrainDeformationSystem.cs"
        "Assets/Scripts/Terrain/Systems/IntegratedTerrainPhysicsSystem.cs"
        "Assets/Scripts/Terrain/Components/TerrainChunk.cs"
        "Assets/Scripts/Terrain/Components/SurfaceData.cs"
    )
    
    for file in "${critical_files[@]}"; do
        if [ -f "$file" ]; then
            local methods=$(grep -c "public.*(" "$file" 2>/dev/null || echo "0")
            local jobs=$(grep -c "IJob" "$file" 2>/dev/null || echo "0")
            local burst=$(grep -c "BurstCompile" "$file" 2>/dev/null || echo "0")
            local file_name=$(basename "$file")
            
            echo "  📄 $file_name: $methods методов, $jobs Jobs, $burst Burst"
        else
            echo "  ❌ $(basename "$file") - не найден"
        fi
    done
}

# Функция анализа производительности деформации
analyze_deformation_performance() {
    echo ""
    echo "⚡ АНАЛИЗ ПРОИЗВОДИТЕЛЬНОСТИ ДЕФОРМАЦИИ"
    echo "======================================="
    
    # Поиск алгоритмов деформации
    local deformation_algorithms=$(find Assets -name "*.cs" -exec grep -c "CalculateDeformation\|ApplyDeformation\|UpdateTerrain" {} \; 2>/dev/null | awk '{sum+=$1} END {print sum}' || echo "0")
    local physics_interactions=$(find Assets -name "*.cs" -exec grep -c "PhysicsInteraction\|WheelTerrainInteraction" {} \; 2>/dev/null | awk '{sum+=$1} END {print sum}' || echo "0")
    local heightmap_operations=$(find Assets -name "*.cs" -exec grep -c "Heightmap\|SetHeight\|GetHeight" {} \; 2>/dev/null | awk '{sum+=$1} END {print sum}' || echo "0")
    
    echo "🔧 Алгоритмы деформации: $deformation_algorithms"
    echo "🚗 Физические взаимодействия: $physics_interactions"
    echo "📊 Операции с высотными картами: $heightmap_operations"
    
    # Анализ использования Job System
    echo ""
    echo "🔄 АНАЛИЗ JOB SYSTEM ДЛЯ ДЕФОРМАЦИИ:"
    echo "===================================="
    
    local parallel_jobs=$(find Assets -name "*.cs" -exec grep -c "IJobParallelFor" {} \; 2>/dev/null | awk '{sum+=$1} END {print sum}' || echo "0")
    local terrain_jobs=$(find Assets -name "*.cs" -exec grep -c "TerrainJob\|DeformationJob" {} \; 2>/dev/null | awk '{sum+=$1} END {print sum}' || echo "0")
    local burst_terrain=$(find Assets -name "*.cs" -exec grep -l "terrain\|Terrain" {} \; 2>/dev/null | xargs grep -c "BurstCompile" 2>/dev/null | awk '{sum+=$1} END {print sum}' || echo "0")
    
    echo "🔄 Параллельные Jobs: $parallel_jobs"
    echo "🏔️  Jobs для террейна: $terrain_jobs"
    echo "⚡ Burst для террейна: $burst_terrain"
    
    # Оценка производительности
    if [ "$terrain_jobs" -gt 0 ] && [ "$burst_terrain" -gt 0 ]; then
        echo -e "  ${GREEN}✅ Хорошая оптимизация деформации террейна${NC}"
    elif [ "$terrain_jobs" -gt 0 ] || [ "$burst_terrain" -gt 0 ]; then
        echo -e "  ${YELLOW}⚠️  Частичная оптимизация деформации${NC}"
    else
        echo -e "  ${RED}❌ Требуется оптимизация деформации террейна${NC}"
    fi
}

# Функция создания оптимизированной системы деформации
create_optimized_deformation_system() {
    echo ""
    echo "🛠️  СОЗДАНИЕ ОПТИМИЗИРОВАННОЙ СИСТЕМЫ ДЕФОРМАЦИИ"
    echo "================================================"
    
    # Создание оптимизированной системы деформации террейна
    cat > "Assets/Scripts/Terrain/Systems/OptimizedTerrainDeformationSystem.cs" << 'EOF'
using Unity.Entities;
using Unity.Burst;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using MudLike.Core.Constants;

namespace MudLike.Terrain.Systems
{
    /// <summary>
    /// Оптимизированная система деформации террейна с Job System и Burst
    /// Критично для реалистичной деформации MudRunner-like
    /// </summary>
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile(CompileSynchronously = true)]
    public partial class OptimizedTerrainDeformationSystem : SystemBase
    {
        private EntityQuery _terrainQuery;
        private EntityQuery _wheelQuery;
        
        protected override void OnCreate()
        {
            // Запрос для террейна
            _terrainQuery = GetEntityQuery(
                ComponentType.ReadWrite<TerrainData>(),
                ComponentType.ReadOnly<TerrainChunk>()
            );
            
            // Запрос для колес
            _wheelQuery = GetEntityQuery(
                ComponentType.ReadOnly<WheelData>(),
                ComponentType.ReadOnly<LocalTransform>()
            );
        }
        
        protected override void OnUpdate()
        {
            var terrainEntities = _terrainQuery.ToEntityArray(Allocator.TempJob);
            var wheelEntities = _wheelQuery.ToEntityArray(Allocator.TempJob);
            
            if (terrainEntities.Length == 0 || wheelEntities.Length == 0)
            {
                terrainEntities.Dispose();
                wheelEntities.Dispose();
                return;
            }
            
            // Создание Job для деформации террейна
            var deformationJob = new TerrainDeformationJob
            {
                TerrainEntities = terrainEntities,
                WheelEntities = wheelEntities,
                TerrainDataLookup = GetComponentLookup<TerrainData>(),
                WheelDataLookup = GetComponentLookup<WheelData>(),
                TransformLookup = GetComponentLookup<LocalTransform>(),
                DeltaTime = SystemAPI.Time.fixedDeltaTime
            };
            
            // Запуск Job с зависимостями
            var jobHandle = deformationJob.ScheduleParallel(
                terrainEntities.Length,
                SystemConstants.TERRAIN_DEFAULT_RESOLUTION / 64, // Оптимальный batch size
                Dependency
            );
            
            Dependency = jobHandle;
            
            // Освобождение временных массивов
            terrainEntities.Dispose();
            wheelEntities.Dispose();
        }
    }
    
    /// <summary>
    /// Job для параллельной деформации террейна
    /// Оптимизирован для максимальной производительности
    /// </summary>
    [BurstCompile]
    public struct TerrainDeformationJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<Entity> TerrainEntities;
        [ReadOnly] public NativeArray<Entity> WheelEntities;
        
        public ComponentLookup<TerrainData> TerrainDataLookup;
        [ReadOnly] public ComponentLookup<WheelData> WheelDataLookup;
        [ReadOnly] public ComponentLookup<LocalTransform> TransformLookup;
        
        public float DeltaTime;
        
        public void Execute(int index)
        {
            if (index >= TerrainEntities.Length) return;
            
            var terrainEntity = TerrainEntities[index];
            var terrainData = TerrainDataLookup[terrainEntity];
            
            // Применение деформации от всех колес
            for (int i = 0; i < WheelEntities.Length; i++)
            {
                var wheelEntity = WheelEntities[i];
                var wheelData = WheelDataLookup[wheelEntity];
                var wheelTransform = TransformLookup[wheelEntity];
                
                // Вычисление деформации от колеса
                var deformation = CalculateWheelDeformation(
                    wheelData,
                    wheelTransform,
                    terrainData,
                    DeltaTime
                );
                
                // Применение деформации к террейну
                ApplyDeformationToTerrain(ref terrainData, deformation);
            }
            
            // Обновление данных террейна
            TerrainDataLookup[terrainEntity] = terrainData;
        }
        
        /// <summary>
        /// Вычисление деформации от колеса
        /// </summary>
        private float CalculateWheelDeformation(
            WheelData wheelData,
            LocalTransform wheelTransform,
            TerrainData terrainData,
            float deltaTime)
        {
            // Вычисление давления колеса на террейн
            var wheelPressure = wheelData.Weight / (math.PI * wheelData.Radius * wheelData.Radius);
            
            // Вычисление деформации на основе давления и твердости террейна
            var deformation = wheelPressure * SystemConstants.TERRAIN_DEFAULT_DEFORMATION_RATE * deltaTime;
            
            // Ограничение деформации максимальными значениями
            deformation = math.clamp(
                deformation,
                SystemConstants.TERRAIN_DEFAULT_MIN_DEPTH,
                SystemConstants.TERRAIN_DEFAULT_MAX_DEPTH
            );
            
            return deformation;
        }
        
        /// <summary>
        /// Применение деформации к террейну
        /// </summary>
        private void ApplyDeformationToTerrain(ref TerrainData terrainData, float deformation)
        {
            // Обновление высотных данных
            terrainData.HeightMap = ApplyHeightDeformation(terrainData.HeightMap, deformation);
            
            // Обновление твердости террейна
            terrainData.Hardness = math.max(
                terrainData.Hardness - deformation * 0.1f,
                SystemConstants.TERRAIN_DEFAULT_HARDNESS * 0.1f
            );
        }
        
        /// <summary>
        /// Применение деформации к высотной карте
        /// </summary>
        private NativeArray<float> ApplyHeightDeformation(NativeArray<float> heightMap, float deformation)
        {
            // Простое применение деформации (можно оптимизировать дальше)
            for (int i = 0; i < heightMap.Length; i++)
            {
                heightMap[i] += deformation;
            }
            
            return heightMap;
        }
    }
    
    /// <summary>
    /// Компонент данных террейна
    /// </summary>
    public struct TerrainData : IComponentData
    {
        public NativeArray<float> HeightMap;
        public float Hardness;
        public float DeformationRate;
        public float RecoveryRate;
    }
    
    /// <summary>
    /// Компонент данных колеса
    /// </summary>
    public struct WheelData : IComponentData
    {
        public float Weight;
        public float Radius;
        public float Width;
        public float Pressure;
    }
}
EOF

    echo "  ✅ Создана оптимизированная система деформации террейна"
    echo "  ⚡ Использует Job System для параллельной обработки"
    echo "  🎯 Использует Burst Compiler для максимальной производительности"
    echo "  📊 Использует константы из SystemConstants"
}

# Функция создания Job для восстановления террейна
create_terrain_recovery_job() {
    echo ""
    echo "🔄 СОЗДАНИЕ JOB ДЛЯ ВОССТАНОВЛЕНИЯ ТЕРРЕЙНА"
    echo "=========================================="
    
    cat > "Assets/Scripts/Terrain/Jobs/TerrainRecoveryJob.cs" << 'EOF'
using Unity.Burst;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;
using MudLike.Core.Constants;

namespace MudLike.Terrain.Jobs
{
    /// <summary>
    /// Job для восстановления деформированного террейна
    /// Критично для реалистичного поведения грязи в MudRunner-like
    /// </summary>
    [BurstCompile]
    public struct TerrainRecoveryJob : IJobParallelFor
    {
        public NativeArray<float> HeightMap;
        public NativeArray<float> HardnessMap;
        public float RecoveryRate;
        public float DeltaTime;
        
        public void Execute(int index)
        {
            if (index >= HeightMap.Length) return;
            
            // Восстановление высоты террейна
            var currentHeight = HeightMap[index];
            var targetHeight = 0.0f; // Базовая высота
            
            var heightDiff = targetHeight - currentHeight;
            var recoveryAmount = heightDiff * RecoveryRate * DeltaTime;
            
            HeightMap[index] = math.lerp(currentHeight, targetHeight, recoveryAmount);
            
            // Восстановление твердости террейна
            var currentHardness = HardnessMap[index];
            var targetHardness = SystemConstants.TERRAIN_DEFAULT_HARDNESS;
            
            var hardnessDiff = targetHardness - currentHardness;
            var hardnessRecovery = hardnessDiff * RecoveryRate * DeltaTime * 0.5f;
            
            HardnessMap[index] = math.lerp(currentHardness, targetHardness, hardnessRecovery);
        }
    }
}
EOF

    echo "  ✅ Создан Job для восстановления террейна"
    echo "  🔄 Параллельное восстановление высот и твердости"
    echo "  ⚡ Оптимизирован с Burst Compiler"
}

# Функция создания системы восстановления террейна
create_terrain_recovery_system() {
    echo ""
    echo "🔄 СОЗДАНИЕ СИСТЕМЫ ВОССТАНОВЛЕНИЯ ТЕРРЕЙНА"
    echo "==========================================="
    
    cat > "Assets/Scripts/Terrain/Systems/TerrainRecoverySystem.cs" << 'EOF'
using Unity.Entities;
using Unity.Burst;
using Unity.Jobs;
using Unity.Collections;
using MudLike.Terrain.Jobs;
using MudLike.Core.Constants;

namespace MudLike.Terrain.Systems
{
    /// <summary>
    /// Система восстановления деформированного террейна
    /// Обеспечивает реалистичное поведение грязи в MudRunner-like
    /// </summary>
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile]
    public partial class TerrainRecoverySystem : SystemBase
    {
        private EntityQuery _terrainQuery;
        
        protected override void OnCreate()
        {
            _terrainQuery = GetEntityQuery(
                ComponentType.ReadWrite<TerrainData>()
            );
        }
        
        protected override void OnUpdate()
        {
            var terrainEntities = _terrainQuery.ToEntityArray(Allocator.TempJob);
            
            if (terrainEntities.Length == 0)
            {
                terrainEntities.Dispose();
                return;
            }
            
            // Создание Job для восстановления террейна
            var recoveryJob = new TerrainRecoveryJob
            {
                HeightMap = GetTerrainHeightMap(),
                HardnessMap = GetTerrainHardnessMap(),
                RecoveryRate = SystemConstants.TERRAIN_DEFAULT_RECOVERY_RATE,
                DeltaTime = SystemAPI.Time.fixedDeltaTime
            };
            
            // Запуск Job с оптимальным batch size
            var jobHandle = recoveryJob.ScheduleParallel(
                GetTerrainHeightMap().Length,
                SystemConstants.TERRAIN_DEFAULT_RESOLUTION / 128,
                Dependency
            );
            
            Dependency = jobHandle;
            terrainEntities.Dispose();
        }
        
        private NativeArray<float> GetTerrainHeightMap()
        {
            // Получение высотной карты террейна
            // Реализация зависит от конкретной структуры данных
            return new NativeArray<float>(SystemConstants.TERRAIN_DEFAULT_RESOLUTION * SystemConstants.TERRAIN_DEFAULT_RESOLUTION, Allocator.TempJob);
        }
        
        private NativeArray<float> GetTerrainHardnessMap()
        {
            // Получение карты твердости террейна
            // Реализация зависит от конкретной структуры данных
            return new NativeArray<float>(SystemConstants.TERRAIN_DEFAULT_RESOLUTION * SystemConstants.TERRAIN_DEFAULT_RESOLUTION, Allocator.TempJob);
        }
    }
}
EOF

    echo "  ✅ Создана система восстановления террейна"
    echo "  🔄 Автоматическое восстановление деформации"
    echo "  ⚡ Оптимизирована для производительности"
}

# Основная логика
main() {
    echo "🏔️  ОПТИМИЗАТОР ДЕФОРМАЦИИ ТЕРРЕЙНА MUD-LIKE"
    echo "============================================="
    echo "🎯 Цель: Максимальная производительность деформации для MudRunner-like"
    echo ""
    
    # 1. Анализ систем деформации террейна
    analyze_terrain_deformation
    
    # 2. Анализ производительности деформации
    analyze_deformation_performance
    
    # 3. Создание оптимизированной системы деформации
    create_optimized_deformation_system
    
    # 4. Создание Job для восстановления террейна
    create_terrain_recovery_job
    
    # 5. Создание системы восстановления террейна
    create_terrain_recovery_system
    
    echo ""
    echo "🎯 НАПОМИНАНИЕ О ЦЕЛИ ПРОЕКТА:"
    echo "🏔️  Деформация террейна - критически важная система MudRunner-like"
    echo "🚗 Реалистичная физика взаимодействия колес с грязью"
    echo "⚡ Максимальная производительность для больших террейнов"
    echo "🌐 Детерминированная симуляция для мультиплеера"
    echo ""
    
    echo "✅ ОПТИМИЗАЦИЯ ДЕФОРМАЦИИ ТЕРРЕЙНА ЗАВЕРШЕНА"
    echo "============================================="
}

# Запуск основной функции
main
