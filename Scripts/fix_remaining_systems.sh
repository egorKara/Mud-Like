#!/bin/bash

# 🎯 Mud-Like Fix Remaining Systems
# Исправление оставшихся ECS систем

set -e

# Цвета для вывода
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

echo -e "${BLUE}🔧 Mud-Like Fix Remaining Systems${NC}"
echo "================================================"

# Список файлов для исправления
declare -A files_to_fix=(
    ["Assets/Scripts/Vehicles/Systems/AdvancedWheelPhysicsSystemJob.cs"]="Assets/Scripts/Vehicles/Systems/AdvancedWheelPhysicsSystem.cs"
    ["Assets/Scripts/Vehicles/Systems/CargoSystemJob.cs"]="Assets/Scripts/Vehicles/Systems/CargoSystem.cs"
    ["Assets/Scripts/Vehicles/Systems/IntegratedTerrainPhysicsSystemJob.cs"]="Assets/Scripts/Vehicles/Systems/IntegratedTerrainPhysicsSystem.cs"
    ["Assets/Scripts/Vehicles/Systems/LODSystemJob.cs"]="Assets/Scripts/Vehicles/Systems/LODSystem.cs"
    ["Assets/Scripts/Vehicles/Systems/MissionSystemJob.cs"]="Assets/Scripts/Vehicles/Systems/MissionSystem.cs"
    ["Assets/Scripts/Vehicles/Systems/OptimizedVehicleMovementSystemJob.cs"]="Assets/Scripts/Vehicles/Systems/OptimizedVehicleMovementSystem.cs"
    ["Assets/Scripts/Vehicles/Systems/OptimizedWheelPhysicsSystemJob.cs"]="Assets/Scripts/Vehicles/Systems/OptimizedWheelPhysicsSystem.cs"
    ["Assets/Scripts/Vehicles/Systems/TireInteractionSystemJob.cs"]="Assets/Scripts/Vehicles/Systems/TireInteractionSystem.cs"
    ["Assets/Scripts/Vehicles/Systems/TireManagementSystemJob.cs"]="Assets/Scripts/Vehicles/Systems/TireManagementSystem.cs"
    ["Assets/Scripts/Vehicles/Systems/TirePressureSystemJob.cs"]="Assets/Scripts/Vehicles/Systems/TirePressureSystem.cs"
    ["Assets/Scripts/Vehicles/Systems/TireTemperatureSystemJob.cs"]="Assets/Scripts/Vehicles/Systems/TireTemperatureSystem.cs"
    ["Assets/Scripts/Vehicles/Systems/TireWearSystemJob.cs"]="Assets/Scripts/Vehicles/Systems/TireWearSystem.cs"
    ["Assets/Scripts/Vehicles/Systems/VehicleDamageSystemJob.cs"]="Assets/Scripts/Vehicles/Systems/VehicleDamageSystem.cs"
    ["Assets/Scripts/Vehicles/Systems/VehicleMaintenanceSystemJob.cs"]="Assets/Scripts/Vehicles/Systems/VehicleMaintenanceSystem.cs"
    ["Assets/Scripts/Vehicles/Systems/WinchSystemJob.cs"]="Assets/Scripts/Vehicles/Systems/WinchSystem.cs"
    ["Assets/Scripts/Vehicles/Systems/OptimizedJobSystemJob.cs"]="Assets/Scripts/Vehicles/Systems/OptimizedJobSystem.cs"
    ["Assets/Scripts/Audio/Systems/EngineAudioSystemJob.cs"]="Assets/Scripts/Audio/Systems/EngineAudioSystem.cs"
    ["Assets/Scripts/Audio/Systems/EnvironmentalAudioSystemJob.cs"]="Assets/Scripts/Audio/Systems/EnvironmentalAudioSystem.cs"
    ["Assets/Scripts/Audio/Systems/WheelAudioSystemJob.cs"]="Assets/Scripts/Audio/Systems/WheelAudioSystem.cs"
    ["Assets/Scripts/Effects/Systems/MudEffectSystemJob.cs"]="Assets/Scripts/Effects/Systems/MudEffectSystem.cs"
    ["Assets/Scripts/Effects/Systems/MudParticleSystemJob.cs"]="Assets/Scripts/Effects/Systems/MudParticleSystem.cs"
)

# Функция для переименования файлов
rename_file() {
    local old_path="$1"
    local new_path="$2"
    
    if [ -f "$old_path" ]; then
        mv "$old_path" "$new_path"
        echo -e "${GREEN}✅ Переименован:${NC} $old_path -> $new_path"
        return 0
    else
        echo -e "${YELLOW}⚠️  Файл не найден:${NC} $old_path"
        return 1
    fi
}

# Основная функция
main() {
    # Проверяем, что мы в корне проекта Unity
    if [ ! -f "ProjectSettings/ProjectVersion.txt" ]; then
        echo -e "${RED}❌ ОШИБКА:${NC} Скрипт должен запускаться из корня проекта Unity"
        exit 1
    fi
    
    echo -e "${YELLOW}⚠️  ВНИМАНИЕ:${NC} Этот скрипт переименует файлы ECS систем!"
    echo ""
    read -p "Продолжить? (y/N): " -n 1 -r
    echo ""
    
    if [[ ! $REPLY =~ ^[Yy]$ ]]; then
        echo "Отменено пользователем."
        exit 0
    fi
    
    echo -e "\n${BLUE}🔧 Исправление ECS систем...${NC}"
    
    for old_path in "${!files_to_fix[@]}"; do
        new_path="${files_to_fix[$old_path]}"
        rename_file "$old_path" "$new_path"
    done
    
    echo -e "\n${GREEN}🎉 Исправление завершено!${NC}"
    echo -e "${BLUE}Запустите проверку:${NC} ./Scripts/smart_naming_check.sh"
}

# Запуск основной функции
main "$@"
