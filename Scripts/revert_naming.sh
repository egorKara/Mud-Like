#!/bin/bash

# 🎯 Mud-Like Naming Convention Reverter
# Откат неправильных переименований

set -e

# Цвета для вывода
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

echo -e "${BLUE}🔄 Mud-Like Naming Convention Reverter${NC}"
echo "================================================"

# Функция для переименования файлов
rename_file() {
    local old_path="$1"
    local new_path="$2"
    
    if [ -f "$old_path" ]; then
        mv "$old_path" "$new_path"
        echo -e "${GREEN}✅ Откат:${NC} $old_path -> $new_path"
        return 0
    else
        echo -e "${YELLOW}⚠️  Файл не найден:${NC} $old_path"
        return 1
    fi
}

# Откат неправильных переименований ECS систем
revert_ecs_systems() {
    echo -e "\n${BLUE}🔄 Откат ECS систем...${NC}"
    
    # Список файлов для отката
    local files_to_revert=(
        "Assets/Scripts/Core/Systems/SystemOrderManagerSystemManager.cs:Assets/Scripts/Core/Systems/SystemOrderManager.cs"
        "Assets/Scripts/Core/Optimization/SystemComposerSystem.cs:Assets/Scripts/Core/Optimization/SystemComposer.cs"
        "Assets/Scripts/Core/Performance/SystemPerformanceProfilerSystem.cs:Assets/Scripts/Core/Performance/SystemPerformanceProfiler.cs"
    )
    
    for file_pair in "${files_to_revert[@]}"; do
        IFS=':' read -r old_path new_path <<< "$file_pair"
        rename_file "$old_path" "$new_path"
    done
}

# Откат неправильных переименований менеджеров
revert_managers() {
    echo -e "\n${BLUE}🔄 Откат менеджеров...${NC}"
    
    # Список файлов для отката
    local files_to_revert=(
        "Assets/Scripts/Terrain/Systems/MudManagerSystemManager.cs:Assets/Scripts/Terrain/Systems/MudManagerSystem.cs"
        "Assets/Scripts/Networking/Systems/NetworkManagerSystemManager.cs:Assets/Scripts/Networking/Systems/NetworkManagerSystem.cs"
        "Assets/Scripts/Tests/Unit/Terrain/MudManagerSystemTestsManager.cs:Assets/Scripts/Tests/Unit/Terrain/MudManagerSystemTests.cs"
    )
    
    for file_pair in "${files_to_revert[@]}"; do
        IFS=':' read -r old_path new_path <<< "$file_pair"
        rename_file "$old_path" "$new_path"
    done
}

# Откат неправильных переименований пулов
revert_pools() {
    echo -e "\n${BLUE}🔄 Откат пулов...${NC}"
    
    # Список файлов для отката
    local files_to_revert=(
        "Assets/Scripts/Core/Performance/ObjectPoolManagerPool.cs:Assets/Scripts/Core/Performance/ObjectPoolManager.cs"
        "Assets/Scripts/Core/ECS/MemoryPoolSystemPool.cs:Assets/Scripts/Core/ECS/MemoryPoolSystem.cs"
        "Assets/Scripts/Pooling/Systems/MudParticlePoolSystemPool.cs:Assets/Scripts/Pooling/Systems/MudParticlePoolSystem.cs"
        "Assets/Scripts/Pooling/Systems/ObjectPoolSystemPool.cs:Assets/Scripts/Pooling/Systems/ObjectPoolSystem.cs"
        "Assets/Scripts/Tests/Unit/Pooling/ObjectPoolSystemTestsPool.cs:Assets/Scripts/Tests/Unit/Pooling/ObjectPoolSystemTests.cs"
        "Assets/Scripts/Tests/Unit/Performance/MemoryPoolTestsPool.cs:Assets/Scripts/Tests/Unit/Performance/MemoryPoolTests.cs"
        "Assets/Scripts/Tests/Unit/ECS/MemoryPoolSystemTestsPool.cs:Assets/Scripts/Tests/Unit/ECS/MemoryPoolSystemTests.cs"
    )
    
    for file_pair in "${files_to_revert[@]}"; do
        IFS=':' read -r old_path new_path <<< "$file_pair"
        rename_file "$old_path" "$new_path"
    done
}

# Откат неправильных переименований Jobs
revert_jobs() {
    echo -e "\n${BLUE}🔄 Откат Jobs...${NC}"
    
    # Список файлов для отката
    local files_to_revert=(
        "Assets/Scripts/Vehicles/Systems/AdvancedTirePhysicsSystemJob.cs:Assets/Scripts/Vehicles/Systems/AdvancedTirePhysicsSystem.cs"
        "Assets/Scripts/Vehicles/Systems/AdvancedVehicleSystemJob.cs:Assets/Scripts/Vehicles/Systems/AdvancedVehicleSystem.cs"
        "Assets/Scripts/Vehicles/Systems/AdvancedWheelPhysicsSystemJob.cs:Assets/Scripts/Vehicles/Systems/AdvancedWheelPhysicsSystem.cs"
        "Assets/Scripts/Vehicles/Systems/CargoSystemJob.cs:Assets/Scripts/Vehicles/Systems/CargoSystem.cs"
        "Assets/Scripts/Vehicles/Systems/IntegratedTerrainPhysicsSystemJob.cs:Assets/Scripts/Vehicles/Systems/IntegratedTerrainPhysicsSystem.cs"
        "Assets/Scripts/Vehicles/Systems/LODSystemJob.cs:Assets/Scripts/Vehicles/Systems/LODSystem.cs"
        "Assets/Scripts/Vehicles/Systems/MissionSystemJob.cs:Assets/Scripts/Vehicles/Systems/MissionSystem.cs"
        "Assets/Scripts/Vehicles/Systems/OptimizedVehicleMovementSystemJob.cs:Assets/Scripts/Vehicles/Systems/OptimizedVehicleMovementSystem.cs"
        "Assets/Scripts/Vehicles/Systems/OptimizedWheelPhysicsSystemJob.cs:Assets/Scripts/Vehicles/Systems/OptimizedWheelPhysicsSystem.cs"
        "Assets/Scripts/Vehicles/Systems/TireInteractionSystemJob.cs:Assets/Scripts/Vehicles/Systems/TireInteractionSystem.cs"
        "Assets/Scripts/Vehicles/Systems/TireManagementSystemJob.cs:Assets/Scripts/Vehicles/Systems/TireManagementSystem.cs"
        "Assets/Scripts/Vehicles/Systems/TirePressureSystemJob.cs:Assets/Scripts/Vehicles/Systems/TirePressureSystem.cs"
        "Assets/Scripts/Vehicles/Systems/TireTemperatureSystemJob.cs:Assets/Scripts/Vehicles/Systems/TireTemperatureSystem.cs"
        "Assets/Scripts/Vehicles/Systems/TireWearSystemJob.cs:Assets/Scripts/Vehicles/Systems/TireWearSystem.cs"
        "Assets/Scripts/Vehicles/Systems/VehicleDamageSystemJob.cs:Assets/Scripts/Vehicles/Systems/VehicleDamageSystem.cs"
        "Assets/Scripts/Vehicles/Systems/VehicleMaintenanceSystemJob.cs:Assets/Scripts/Vehicles/Systems/VehicleMaintenanceSystem.cs"
        "Assets/Scripts/Vehicles/Systems/WinchSystemJob.cs:Assets/Scripts/Vehicles/Systems/WinchSystem.cs"
        "Assets/Scripts/Vehicles/Systems/OptimizedJobSystemJob.cs:Assets/Scripts/Vehicles/Systems/OptimizedJobSystem.cs"
        "Assets/Scripts/Pooling/Systems/ObjectPoolSystemPoolJob.cs:Assets/Scripts/Pooling/Systems/ObjectPoolSystem.cs"
        "Assets/Scripts/Audio/Systems/EngineAudioSystemJob.cs:Assets/Scripts/Audio/Systems/EngineAudioSystem.cs"
        "Assets/Scripts/Audio/Systems/EnvironmentalAudioSystemJob.cs:Assets/Scripts/Audio/Systems/EnvironmentalAudioSystem.cs"
        "Assets/Scripts/Audio/Systems/WheelAudioSystemJob.cs:Assets/Scripts/Audio/Systems/WheelAudioSystem.cs"
        "Assets/Scripts/Effects/Systems/MudEffectSystemJob.cs:Assets/Scripts/Effects/Systems/MudEffectSystem.cs"
        "Assets/Scripts/Effects/Systems/MudParticleSystemJob.cs:Assets/Scripts/Effects/Systems/MudParticleSystem.cs"
    )
    
    for file_pair in "${files_to_revert[@]}"; do
        IFS=':' read -r old_path new_path <<< "$file_pair"
        rename_file "$old_path" "$new_path"
    done
}

# Основная функция
main() {
    # Проверяем, что мы в корне проекта Unity
    if [ ! -f "ProjectSettings/ProjectVersion.txt" ]; then
        echo -e "${RED}❌ ОШИБКА:${NC} Скрипт должен запускаться из корня проекта Unity"
        exit 1
    fi
    
    echo -e "${YELLOW}⚠️  ВНИМАНИЕ:${NC} Этот скрипт откатит неправильные переименования!"
    echo ""
    read -p "Продолжить? (y/N): " -n 1 -r
    echo ""
    
    if [[ ! $REPLY =~ ^[Yy]$ ]]; then
        echo "Отменено пользователем."
        exit 0
    fi
    
    revert_ecs_systems
    revert_managers
    revert_pools
    revert_jobs
    
    echo -e "\n${GREEN}🎉 Откат завершен!${NC}"
    echo -e "${BLUE}Запустите проверку:${NC} ./Scripts/naming_check.sh"
}

# Запуск основной функции
main "$@"
