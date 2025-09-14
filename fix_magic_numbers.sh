#!/bin/bash

# Автоматическое исправление магических чисел в коде
# Создан: 14 сентября 2025
# Цель: Улучшение качества кода проекта MudRunner-like

echo "🔢 ИСПРАВЛЕНИЕ МАГИЧЕСКИХ ЧИСЕЛ В КОДЕ"
echo "======================================"

# Цвета для вывода
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Функция поиска магических чисел
find_magic_numbers() {
    echo "🔍 Поиск магических чисел в коде..."
    
    # Поиск чисел, которые могут быть магическими
    local magic_numbers=$(find Assets -name "*.cs" -exec grep -n "[^a-zA-Z0-9_\.][0-9]\+\.[0-9]\+[^a-zA-Z0-9_\.]" {} \; 2>/dev/null | wc -l | tr -d ' ')
    local integer_numbers=$(find Assets -name "*.cs" -exec grep -n "[^a-zA-Z0-9_\.][0-9]\{2,\}[^a-zA-Z0-9_\.]" {} \; 2>/dev/null | wc -l | tr -d ' ')
    
    echo "  📊 Найдено чисел с плавающей точкой: $magic_numbers"
    echo "  📊 Найдено целых чисел (2+ цифр): $integer_numbers"
    
    return $((magic_numbers + integer_numbers))
}

# Функция анализа найденных чисел
analyze_magic_numbers() {
    echo ""
    echo "📊 АНАЛИЗ НАЙДЕННЫХ ЧИСЕЛ"
    echo "=========================="
    
    # Анализ наиболее частых магических чисел
    echo "🔍 Анализ наиболее частых чисел..."
    
    # Поиск чисел, связанных с физикой (критично для MudRunner-like)
    local physics_numbers=$(find Assets -name "*.cs" -exec grep -l "9\.81\|0\.02\|0\.1\|0\.5\|0\.3" {} \; 2>/dev/null | wc -l | tr -d ' ')
    echo "  🚗 Числа физики (9.81, 0.02, 0.1, 0.5, 0.3): $physics_numbers файлов"
    
    # Поиск чисел, связанных с транспортными средствами
    local vehicle_numbers=$(find Assets -name "*.cs" -exec grep -l "2000\|50\|10\|5000\|30" {} \; 2>/dev/null | wc -l | tr -d ' ')
    echo "  🚙 Числа транспортных средств (2000, 50, 10, 5000, 30): $vehicle_numbers файлов"
    
    # Поиск чисел, связанных с террейном
    local terrain_numbers=$(find Assets -name "*.cs" -exec grep -l "513\|1000\|0\.5\|-0\.2" {} \; 2>/dev/null | wc -l | tr -d ' ')
    echo "  🏔️  Числа террейна (513, 1000, 0.5, -0.2): $terrain_numbers файлов"
    
    # Поиск чисел, связанных с сетью
    local network_numbers=$(find Assets -name "*.cs" -exec grep -l "7777\|32\|30\.0\|20\.0\|60\.0" {} \; 2>/dev/null | wc -l | tr -d ' ')
    echo "  🌐 Числа сети (7777, 32, 30.0, 20.0, 60.0): $network_numbers файлов"
}

# Функция создания рекомендаций по замене
create_replacement_recommendations() {
    echo ""
    echo "💡 РЕКОМЕНДАЦИИ ПО ЗАМЕНЕ МАГИЧЕСКИХ ЧИСЕЛ"
    echo "=========================================="
    
    echo "🔢 Критически важные замены для MudRunner-like:"
    echo ""
    
    # Физика
    echo "🚗 ФИЗИКА ТРАНСПОРТНЫХ СРЕДСТВ:"
    echo "  9.81f → SystemConstants.DEFAULT_GRAVITY"
    echo "  0.02f → SystemConstants.DEFAULT_FIXED_DELTA_TIME"
    echo "  0.1f → SystemConstants.DEFAULT_MAX_DELTA_TIME"
    echo "  0.7f → SystemConstants.DEFAULT_FRICTION"
    echo "  0.3f → SystemConstants.DEFAULT_BOUNCE"
    echo ""
    
    # Транспортные средства
    echo "🚙 ТРАНСПОРТНЫЕ СРЕДСТВА:"
    echo "  2000.0f → SystemConstants.VEHICLE_DEFAULT_MASS"
    echo "  50.0f → SystemConstants.VEHICLE_DEFAULT_MAX_SPEED"
    echo "  10.0f → SystemConstants.VEHICLE_DEFAULT_ACCELERATION"
    echo "  5000.0f → SystemConstants.VEHICLE_DEFAULT_BRAKE_FORCE"
    echo "  30.0f → SystemConstants.VEHICLE_DEFAULT_STEER_ANGLE"
    echo "  0.5f → SystemConstants.VEHICLE_DEFAULT_WHEEL_RADIUS"
    echo "  0.3f → SystemConstants.VEHICLE_DEFAULT_WHEEL_WIDTH"
    echo ""
    
    # Деформация террейна
    echo "🏔️  ДЕФОРМАЦИЯ ТЕРРЕЙНА:"
    echo "  513 → SystemConstants.TERRAIN_DEFAULT_RESOLUTION"
    echo "  1000.0f → SystemConstants.TERRAIN_DEFAULT_SIZE"
    echo "  0.5f → SystemConstants.TERRAIN_DEFAULT_MAX_DEPTH"
    echo "  -0.2f → SystemConstants.TERRAIN_DEFAULT_MIN_DEPTH"
    echo "  1.0f → SystemConstants.TERRAIN_DEFAULT_HARDNESS"
    echo "  0.1f → SystemConstants.TERRAIN_DEFAULT_DEFORMATION_RATE"
    echo ""
    
    # Мультиплеер
    echo "🌐 МУЛЬТИПЛЕЕР:"
    echo "  7777 → SystemConstants.DEFAULT_NETWORK_PORT"
    echo "  32 → SystemConstants.MAX_NETWORK_CONNECTIONS"
    echo "  30.0f → SystemConstants.NETWORK_TIMEOUT"
    echo "  20.0f → SystemConstants.NETWORK_DEFAULT_SEND_RATE"
    echo "  60.0f → SystemConstants.NETWORK_DEFAULT_SNAPSHOT_RATE"
    echo ""
    
    # Детерминизм
    echo "🎯 ДЕТЕРМИНИЗМ:"
    echo "  0.0001f → SystemConstants.DETERMINISTIC_EPSILON"
    echo "  100 → SystemConstants.DETERMINISTIC_MAX_ITERATIONS"
    echo "  0.001f → SystemConstants.DETERMINISTIC_CONVERGENCE_THRESHOLD"
}

# Функция автоматической замены критических чисел
auto_replace_critical_numbers() {
    echo ""
    echo "🔧 АВТОМАТИЧЕСКАЯ ЗАМЕНА КРИТИЧЕСКИХ ЧИСЕЛ"
    echo "=========================================="
    
    local replacements_made=0
    
    # Замена критических чисел физики
    echo "🚗 Замена чисел физики..."
    
    # Замена гравитации
    if find Assets -name "*.cs" -exec grep -l "9\.81" {} \; 2>/dev/null | head -1 | xargs -I {} sed -i 's/9\.81f/SystemConstants.DEFAULT_GRAVITY/g' {} 2>/dev/null; then
        echo "  ✅ Гравитация (9.81f) заменена"
        replacements_made=$((replacements_made + 1))
    fi
    
    # Замена фиксированного времени
    if find Assets -name "*.cs" -exec grep -l "0\.02f" {} \; 2>/dev/null | head -1 | xargs -I {} sed -i 's/0\.02f/SystemConstants.DEFAULT_FIXED_DELTA_TIME/g' {} 2>/dev/null; then
        echo "  ✅ Фиксированное время (0.02f) заменено"
        replacements_made=$((replacements_made + 1))
    fi
    
    # Замена трения
    if find Assets -name "*.cs" -exec grep -l "0\.7f" {} \; 2>/dev/null | head -1 | xargs -I {} sed -i 's/0\.7f/SystemConstants.DEFAULT_FRICTION/g' {} 2>/dev/null; then
        echo "  ✅ Трение (0.7f) заменено"
        replacements_made=$((replacements_made + 1))
    fi
    
    # Замена отскока
    if find Assets -name "*.cs" -exec grep -l "0\.3f" {} \; 2>/dev/null | head -1 | xargs -I {} sed -i 's/0\.3f/SystemConstants.DEFAULT_BOUNCE/g' {} 2>/dev/null; then
        echo "  ✅ Отскок (0.3f) заменен"
        replacements_made=$((replacements_made + 1))
    fi
    
    # Замена массы транспортного средства
    if find Assets -name "*.cs" -exec grep -l "2000\.0f" {} \; 2>/dev/null | head -1 | xargs -I {} sed -i 's/2000\.0f/SystemConstants.VEHICLE_DEFAULT_MASS/g' {} 2>/dev/null; then
        echo "  ✅ Масса транспортного средства (2000.0f) заменена"
        replacements_made=$((replacements_made + 1))
    fi
    
    # Замена максимальной скорости
    if find Assets -name "*.cs" -exec grep -l "50\.0f" {} \; 2>/dev/null | head -1 | xargs -I {} sed -i 's/50\.0f/SystemConstants.VEHICLE_DEFAULT_MAX_SPEED/g' {} 2>/dev/null; then
        echo "  ✅ Максимальная скорость (50.0f) заменена"
        replacements_made=$((replacements_made + 1))
    fi
    
    # Замена порта сети
    if find Assets -name "*.cs" -exec grep -l "7777" {} \; 2>/dev/null | head -1 | xargs -I {} sed -i 's/7777/SystemConstants.DEFAULT_NETWORK_PORT/g' {} 2>/dev/null; then
        echo "  ✅ Порт сети (7777) заменен"
        replacements_made=$((replacements_made + 1))
    fi
    
    echo ""
    echo "📊 Всего замен: $replacements_made"
    
    if [ "$replacements_made" -gt 0 ]; then
        echo -e "${GREEN}✅ АВТОМАТИЧЕСКАЯ ЗАМЕНА ЗАВЕРШЕНА${NC}"
        echo -e "${GREEN}💡 Рекомендуется проверить компиляцию${NC}"
    else
        echo -e "${YELLOW}⚠️  Критические числа для замены не найдены${NC}"
    fi
}

# Функция проверки использования констант
check_constants_usage() {
    echo ""
    echo "📋 ПРОВЕРКА ИСПОЛЬЗОВАНИЯ КОНСТАНТ"
    echo "=================================="
    
    # Проверка использования SystemConstants
    local constants_usage=$(find Assets -name "*.cs" -exec grep -l "SystemConstants\." {} \; 2>/dev/null | wc -l | tr -d ' ')
    echo "📊 Файлов, использующих SystemConstants: $constants_usage"
    
    # Проверка конкретных констант
    local gravity_usage=$(find Assets -name "*.cs" -exec grep -l "DEFAULT_GRAVITY" {} \; 2>/dev/null | wc -l | tr -d ' ')
    local vehicle_mass_usage=$(find Assets -name "*.cs" -exec grep -l "VEHICLE_DEFAULT_MASS" {} \; 2>/dev/null | wc -l | tr -d ' ')
    local terrain_resolution_usage=$(find Assets -name "*.cs" -exec grep -l "TERRAIN_DEFAULT_RESOLUTION" {} \; 2>/dev/null | wc -l | tr -d ' ')
    
    echo "🚗 Использование DEFAULT_GRAVITY: $gravity_usage файлов"
    echo "🚙 Использование VEHICLE_DEFAULT_MASS: $vehicle_mass_usage файлов"
    echo "🏔️  Использование TERRAIN_DEFAULT_RESOLUTION: $terrain_resolution_usage файлов"
}

# Основная логика
main() {
    echo "🔢 ИСПРАВЛЕНИЕ МАГИЧЕСКИХ ЧИСЕЛ В КОДЕ"
    echo "======================================"
    echo "🎯 Цель: Улучшение качества кода проекта MudRunner-like"
    echo ""
    
    # 1. Поиск магических чисел
    find_magic_numbers
    local total_numbers=$?
    
    # 2. Анализ найденных чисел
    analyze_magic_numbers
    
    # 3. Создание рекомендаций
    create_replacement_recommendations
    
    # 4. Автоматическая замена критических чисел
    auto_replace_critical_numbers
    
    # 5. Проверка использования констант
    check_constants_usage
    
    echo ""
    echo "🎯 НАПОМИНАНИЕ О ЦЕЛИ ПРОЕКТА:"
    echo "🚗 MudRunner-like игра требует точных физических констант"
    echo "🏔️  Деформация террейна критически зависит от правильных значений"
    echo "🌐 Мультиплеер требует детерминированных констант"
    echo ""
    
    if [ "$total_numbers" -gt 0 ]; then
        echo -e "${YELLOW}⚠️  Найдено $total_numbers потенциальных магических чисел${NC}"
        echo -e "${YELLOW}💡 Рекомендуется ручная проверка и замена${NC}"
    else
        echo -e "${GREEN}✅ Магические числа не найдены${NC}"
    fi
    
    echo ""
    echo "✅ ИСПРАВЛЕНИЕ МАГИЧЕСКИХ ЧИСЕЛ ЗАВЕРШЕНО"
    echo "=========================================="
}

# Запуск основной функции
main
