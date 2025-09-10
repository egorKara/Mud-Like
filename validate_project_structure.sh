#!/bin/bash

# Валидация структуры Unity проекта без Unity Editor
# Проверяет корректность файлов и готовность к headless тестированию

echo "🔍 ВАЛИДАЦИЯ СТРУКТУРЫ UNITY ПРОЕКТА"
echo "===================================="
echo "Время: $(date)"
echo ""

PROJECT_PATH="/workspace"
ERRORS=0
WARNINGS=0

# Функция для подсчета ошибок
add_error() {
    echo "❌ $1"
    ERRORS=$((ERRORS + 1))
}

add_warning() {
    echo "⚠️  $1"
    WARNINGS=$((WARNINGS + 1))
}

add_success() {
    echo "✅ $1"
}

# Проверка 1: Основная структура проекта
echo "📂 1. ПРОВЕРКА ОСНОВНОЙ СТРУКТУРЫ"
echo "================================="

# ProjectSettings
if [ -d "$PROJECT_PATH/ProjectSettings" ]; then
    add_success "Папка ProjectSettings найдена"
    
    if [ -f "$PROJECT_PATH/ProjectSettings/ProjectVersion.txt" ]; then
        UNITY_VERSION=$(cat "$PROJECT_PATH/ProjectSettings/ProjectVersion.txt")
        add_success "Unity версия: $UNITY_VERSION"
    else
        add_error "ProjectVersion.txt не найден"
    fi
    
    if [ -f "$PROJECT_PATH/ProjectSettings/ProjectSettings.asset" ]; then
        add_success "ProjectSettings.asset найден"
    else
        add_error "ProjectSettings.asset не найден"
    fi
else
    add_error "Папка ProjectSettings не найдена"
fi

# Assets
if [ -d "$PROJECT_PATH/Assets" ]; then
    add_success "Папка Assets найдена"
else
    add_error "Папка Assets не найдена"
fi

# Packages
if [ -f "$PROJECT_PATH/Packages/manifest.json" ]; then
    add_success "Packages/manifest.json найден"
    
    # Проверяем ключевые пакеты
    if grep -q "com.unity.entities" "$PROJECT_PATH/Packages/manifest.json"; then
        add_success "Entities пакет найден"
    else
        add_warning "Entities пакет не найден (требуется для ECS)"
    fi
    
    if grep -q "com.unity.netcode.gameobjects" "$PROJECT_PATH/Packages/manifest.json"; then
        add_success "Netcode for GameObjects найден"
    else
        add_warning "Netcode for GameObjects не найден (требуется для мультиплеера)"
    fi
else
    add_error "Packages/manifest.json не найден"
fi

echo ""

# Проверка 2: ECS компоненты
echo "🧩 2. ПРОВЕРКА ECS КОМПОНЕНТОВ"
echo "=============================="

# TruckData
if [ -f "$PROJECT_PATH/Assets/Scripts/Vehicles/Components/TruckData.cs" ]; then
    add_success "TruckData.cs найден"
    
    # Проверяем содержимое
    if grep -q "struct TruckData : IComponentData" "$PROJECT_PATH/Assets/Scripts/Vehicles/Components/TruckData.cs"; then
        add_success "TruckData корректно наследует IComponentData"
    else
        add_error "TruckData не наследует IComponentData"
    fi
    
    if grep -q "LockFrontDifferential" "$PROJECT_PATH/Assets/Scripts/Vehicles/Components/TruckData.cs"; then
        add_success "Блокировка дифференциалов реализована"
    else
        add_warning "Блокировка дифференциалов не найдена"
    fi
else
    add_error "TruckData.cs не найден"
fi

# TruckControl
if [ -f "$PROJECT_PATH/Assets/Scripts/Vehicles/Components/TruckControl.cs" ]; then
    add_success "TruckControl.cs найден"
else
    add_error "TruckControl.cs не найден"
fi

# WheelData
if [ -f "$PROJECT_PATH/Assets/Scripts/Vehicles/Components/WheelData.cs" ]; then
    add_success "WheelData.cs найден"
else
    add_error "WheelData.cs не найден"
fi

echo ""

# Проверка 3: ECS системы
echo "⚙️  3. ПРОВЕРКА ECS СИСТЕМ"
echo "========================="

# TruckMovementSystem
if [ -f "$PROJECT_PATH/Assets/Scripts/Vehicles/Systems/TruckMovementSystem.cs" ]; then
    add_success "TruckMovementSystem.cs найден"
    
    if grep -q "BurstCompile" "$PROJECT_PATH/Assets/Scripts/Vehicles/Systems/TruckMovementSystem.cs"; then
        add_success "Burst Compiler оптимизация включена"
    else
        add_warning "Burst Compiler оптимизация не найдена"
    fi
else
    add_error "TruckMovementSystem.cs не найден"
fi

# TruckControlSystem
if [ -f "$PROJECT_PATH/Assets/Scripts/Vehicles/Systems/TruckControlSystem.cs" ]; then
    add_success "TruckControlSystem.cs найден"
else
    add_error "TruckControlSystem.cs не найден"
fi

echo ""

# Проверка 4: Система защиты от перегрева
echo "🛡️  4. ПРОВЕРКА СИСТЕМЫ ЗАЩИТЫ ОТ ПЕРЕГРЕВА"
echo "=========================================="

if [ -f "overheat_protection.sh" ]; then
    add_success "overheat_protection.sh найден"
    
    if [ -x "overheat_protection.sh" ]; then
        add_success "overheat_protection.sh исполняемый"
    else
        add_error "overheat_protection.sh не исполняемый"
    fi
    
    # Проверяем, что система работает
    if ps aux | grep overheat_protection | grep -v grep > /dev/null; then
        add_success "Система защиты от перегрева активна"
    else
        add_warning "Система защиты от перегрева не запущена"
    fi
else
    add_error "overheat_protection.sh не найден"
fi

echo ""

# Проверка 5: Документация
echo "📚 5. ПРОВЕРКА ДОКУМЕНТАЦИИ"
echo "=========================="

DOC_FILES=(
    "README.md"
    "Project_Startup/README.md"
    "Project_Startup/17_Implementation_Status.md"
    "Project_Startup/20_Overheat_Protection_System.md"
)

for doc in "${DOC_FILES[@]}"; do
    if [ -f "$PROJECT_PATH/$doc" ]; then
        add_success "$doc найден"
    else
        add_warning "$doc не найден"
    fi
done

echo ""

# Проверка 6: Тесты
echo "🧪 6. ПРОВЕРКА ТЕСТОВ"
echo "===================="

if [ -d "$PROJECT_PATH/Assets/Scripts/Tests" ]; then
    add_success "Папка Tests найдена"
    
    TEST_COUNT=$(find "$PROJECT_PATH/Assets/Scripts/Tests" -name "*Test*.cs" | wc -l)
    if [ $TEST_COUNT -gt 0 ]; then
        add_success "Найдено $TEST_COUNT тестовых файлов"
    else
        add_warning "Тестовые файлы не найдены"
    fi
else
    add_warning "Папка Tests не найдена"
fi

echo ""

# Проверка 7: Производительность
echo "⚡ 7. ПРОВЕРКА ПРОИЗВОДИТЕЛЬНОСТИ"
echo "==============================="

# Проверяем использование Burst Compiler
BURST_FILES=$(find "$PROJECT_PATH/Assets/Scripts" -name "*.cs" -exec grep -l "BurstCompile" {} \; 2>/dev/null | wc -l)
if [ $BURST_FILES -gt 0 ]; then
    add_success "Burst Compiler используется в $BURST_FILES файлах"
else
    add_warning "Burst Compiler не используется"
fi

# Проверяем использование Job System
JOB_FILES=$(find "$PROJECT_PATH/Assets/Scripts" -name "*.cs" -exec grep -l "IJobEntity\|IJob" {} \; 2>/dev/null | wc -l)
if [ $JOB_FILES -gt 0 ]; then
    add_success "Job System используется в $JOB_FILES файлах"
else
    add_warning "Job System не используется"
fi

echo ""

# Итоговый результат
echo "📊 ИТОГОВЫЙ РЕЗУЛЬТАТ"
echo "===================="

echo "Ошибки: $ERRORS"
echo "Предупреждения: $WARNINGS"

if [ $ERRORS -eq 0 ]; then
    if [ $WARNINGS -eq 0 ]; then
        echo ""
        echo "🎉 ОТЛИЧНО! Проект полностью готов к headless тестированию!"
        echo "✅ Все компоненты на месте"
        echo "✅ ECS архитектура корректна"
        echo "✅ Система защиты от перегрева активна"
        echo ""
        echo "🚀 Рекомендации:"
        echo "   1. Установите Unity Editor для полного тестирования"
        echo "   2. Запустите ./test_unity_headless.sh после установки Unity"
        echo "   3. Продолжайте разработку с уверенностью!"
        exit 0
    else
        echo ""
        echo "✅ ХОРОШО! Проект готов с небольшими замечаниями"
        echo "⚠️  Исправьте $WARNINGS предупреждений для идеального состояния"
        exit 0
    fi
else
    echo ""
    echo "❌ ТРЕБУЕТСЯ ВНИМАНИЕ! Найдено $ERRORS критических ошибок"
    echo "🔧 Исправьте ошибки перед продолжением разработки"
    exit 1
fi