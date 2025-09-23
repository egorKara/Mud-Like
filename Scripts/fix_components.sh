#!/bin/bash

# 🎯 Mud-Like Fix Components
# Исправление ECS компонентов с запрещенными именами

set -e

# Цвета для вывода
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

echo -e "${BLUE}🔧 Mud-Like Fix Components${NC}"
echo "================================================"

# Функция для исправления компонента
fix_component() {
    local file="$1"
    local old_name="$2"
    local new_name="$3"
    
    if [ -f "$file" ]; then
        # Исправляем имя класса
        sed -i "s/struct ${old_name} : IComponentData/struct ${new_name} : IComponentData/g" "$file"
        
        # Переименовываем файл
        local dir=$(dirname "$file")
        local new_file="$dir/${new_name}.cs"
        mv "$file" "$new_file"
        
        echo -e "${GREEN}✅ Исправлен:${NC} $file -> $new_file"
    fi
}

# Основная функция
main() {
    # Проверяем, что мы в корне проекта Unity
    if [ ! -f "ProjectSettings/ProjectVersion.txt" ]; then
        echo -e "${RED}❌ ОШИБКА:${NC} Скрипт должен запускаться из корня проекта Unity"
        exit 1
    fi
    
    echo -e "${YELLOW}⚠️  ВНИМАНИЕ:${NC} Этот скрипт исправит ECS компоненты с запрещенными именами!"
    echo ""
    read -p "Продолжить? (y/N): " -n 1 -r
    echo ""
    
    if [[ ! $REPLY =~ ^[Yy]$ ]]; then
        echo "Отменено пользователем."
        exit 0
    fi
    
    echo -e "\n${BLUE}🔧 Исправление ECS компонентов...${NC}"
    
    # Исправляем компоненты с запрещенными именами
    fix_component "Assets/Scripts/Vehicles/Components/EngineData.cs" "EngineData" "Engine"
    fix_component "Assets/Scripts/Vehicles/Components/FuelData.cs" "FuelData" "Fuel"
    fix_component "Assets/Scripts/Vehicles/Components/TransmissionData.cs" "TransmissionData" "Transmission"
    fix_component "Assets/Scripts/Vehicles/Components/BrakeData.cs" "BrakeData" "Brake"
    fix_component "Assets/Scripts/Vehicles/Components/SteeringData.cs" "SteeringData" "Steering"
    fix_component "Assets/Scripts/Vehicles/Components/SuspensionData.cs" "SuspensionData" "Suspension"
    fix_component "Assets/Scripts/Vehicles/Components/WheelData.cs" "WheelData" "Wheel"
    fix_component "Assets/Scripts/Vehicles/Components/TireData.cs" "TireData" "Tire"
    fix_component "Assets/Scripts/Vehicles/Components/VehicleData.cs" "VehicleData" "Vehicle"
    fix_component "Assets/Scripts/Vehicles/Components/PlayerData.cs" "PlayerData" "Player"
    fix_component "Assets/Scripts/Vehicles/Components/AIData.cs" "AIData" "AI"
    fix_component "Assets/Scripts/Vehicles/Components/MissionData.cs" "MissionData" "Mission"
    fix_component "Assets/Scripts/Vehicles/Components/DamageData.cs" "DamageData" "Damage"
    fix_component "Assets/Scripts/Vehicles/Components/MaintenanceData.cs" "MaintenanceData" "Maintenance"
    fix_component "Assets/Scripts/Vehicles/Components/WinchData.cs" "WinchData" "Winch"
    fix_component "Assets/Scripts/Vehicles/Components/CargoData.cs" "CargoData" "Cargo"
    fix_component "Assets/Scripts/Vehicles/Components/LODData.cs" "LODData" "LOD"
    fix_component "Assets/Scripts/Vehicles/Components/EventData.cs" "EventData" "Event"
    fix_component "Assets/Scripts/Vehicles/Components/InfoData.cs" "InfoData" "Info"
    fix_component "Assets/Scripts/Vehicles/Components/WorkData.cs" "WorkData" "Work"
    fix_component "Assets/Scripts/Vehicles/Components/JobData.cs" "JobData" "Job"
    fix_component "Assets/Scripts/Vehicles/Components/PoolData.cs" "PoolData" "Pool"
    fix_component "Assets/Scripts/Vehicles/Components/ManagerData.cs" "ManagerData" "Manager"
    fix_component "Assets/Scripts/Vehicles/Components/SystemData.cs" "SystemData" "System"
    
    echo -e "\n${GREEN}🎉 Исправление завершено!${NC}"
    echo -e "${BLUE}Запустите проверку:${NC} ./Scripts/smart_naming_check.sh"
}

# Запуск основной функции
main "$@"
