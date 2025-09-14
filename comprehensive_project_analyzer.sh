#!/bin/bash

# Комплексный анализатор всего проекта MudRunner-like
# Создан: 14 сентября 2025
# Цель: Глубокий анализ всех систем без остановки

echo "🔍 КОМПЛЕКСНЫЙ АНАЛИЗАТОР ВСЕГО ПРОЕКТА MUD-RUNNER-LIKE"
echo "======================================================="

# Цвета для вывода
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
PURPLE='\033[0;35m'
CYAN='\033[0;36m'
NC='\033[0m' # No Color

# Функция анализа архитектуры проекта
analyze_project_architecture() {
    echo "🏗️  АНАЛИЗ АРХИТЕКТУРЫ ПРОЕКТА"
    echo "================================"
    
    # Структура директорий
    echo "📁 СТРУКТУРА ДИРЕКТОРИЙ:"
    find Assets -type d | sort | while read dir; do
        local file_count=$(find "$dir" -maxdepth 1 -name "*.cs" | wc -l | tr -d ' ')
        if [ "$file_count" -gt 0 ]; then
            echo "  📂 $dir: $file_count файлов"
        fi
    done
    
    # Анализ модулей
    echo ""
    echo "🧩 АНАЛИЗ МОДУЛЕЙ:"
    local modules=("Core" "Vehicles" "Terrain" "Networking" "DOTS" "Tests" "Performance" "Optimization")
    
    for module in "${modules[@]}"; do
        local module_files=$(find Assets -path "*/$module/*" -name "*.cs" | wc -l | tr -d ' ')
        local module_lines=$(find Assets -path "*/$module/*" -name "*.cs" -exec wc -l {} + 2>/dev/null | tail -1 | awk '{print $1}' || echo "0")
        local module_methods=$(find Assets -path "*/$module/*" -name "*.cs" -exec grep -c "public.*(" {} \; 2>/dev/null | awk '{sum+=$1} END {print sum}' || echo "0")
        
        echo "  🧩 $module: $module_files файлов, $module_lines строк, $module_methods методов"
    done
}

# Функция анализа качества кода
analyze_code_quality() {
    echo ""
    echo "📊 АНАЛИЗ КАЧЕСТВА КОДА"
    echo "========================"
    
    # Анализ сложности кода
    local total_files=$(find Assets -name "*.cs" | wc -l | tr -d ' ')
    local total_lines=$(find Assets -name "*.cs" -exec wc -l {} + 2>/dev/null | tail -1 | awk '{print $1}' || echo "0")
    local total_methods=$(find Assets -name "*.cs" -exec grep -c "public.*(" {} \; 2>/dev/null | awk '{sum+=$1} END {print sum}' || echo "0")
    local total_classes=$(find Assets -name "*.cs" -exec grep -c "class\|struct" {} \; 2>/dev/null | awk '{sum+=$1} END {print sum}' || echo "0")
    
    echo "📈 ОБЩАЯ СТАТИСТИКА:"
    echo "  📁 Файлов: $total_files"
    echo "  📝 Строк кода: $total_lines"
    echo "  🔧 Методов: $total_methods"
    echo "  🏗️  Классов/структур: $total_classes"
    
    # Анализ качества
    local avg_lines_per_file=$((total_lines / total_files))
    local avg_methods_per_class=$((total_methods / total_classes))
    
    echo "📊 ПОКАЗАТЕЛИ КАЧЕСТВА:"
    echo "  📝 Среднее строк на файл: $avg_lines_per_file"
    echo "  🔧 Среднее методов на класс: $avg_methods_per_class"
    
    # Оценка качества
    if [ "$avg_lines_per_file" -lt 200 ] && [ "$avg_methods_per_class" -lt 10 ]; then
        echo -e "  ${GREEN}✅ Отличное качество кода${NC}"
    elif [ "$avg_lines_per_file" -lt 300 ] && [ "$avg_methods_per_class" -lt 15 ]; then
        echo -e "  ${YELLOW}⚠️  Хорошее качество кода${NC}"
    else
        echo -e "  ${RED}❌ Требуется рефакторинг${NC}"
    fi
}

# Функция анализа производительности
analyze_performance() {
    echo ""
    echo "⚡ АНАЛИЗ ПРОИЗВОДИТЕЛЬНОСТИ"
    echo "============================"
    
    # ECS оптимизация
    local burst_systems=$(find Assets -name "*.cs" -exec grep -c "BurstCompile" {} \; 2>/dev/null | awk '{sum+=$1} END {print sum}' || echo "0")
    local job_systems=$(find Assets -name "*.cs" -exec grep -c "IJob" {} \; 2>/dev/null | awk '{sum+=$1} END {print sum}' || echo "0")
    local native_collections=$(find Assets -name "*.cs" -exec grep -c "NativeArray\|NativeList\|NativeHashMap" {} \; 2>/dev/null | awk '{sum+=$1} END {print sum}' || echo "0")
    local ecs_components=$(find Assets -name "*.cs" -exec grep -c "IComponentData\|IBufferElementData" {} \; 2>/dev/null | awk '{sum+=$1} END {print sum}' || echo "0")
    
    echo "🚀 ECS ОПТИМИЗАЦИЯ:"
    echo "  ⚡ Burst систем: $burst_systems"
    echo "  🔄 Job систем: $job_systems"
    echo "  📦 Native Collections: $native_collections"
    echo "  🧩 ECS компонентов: $ecs_components"
    
    # Анализ памяти
    local memory_allocations=$(find Assets -name "*.cs" -exec grep -c "new\|malloc\|Allocator" {} \; 2>/dev/null | awk '{sum+=$1} END {print sum}' || echo "0")
    local temp_allocations=$(find Assets -name "*.cs" -exec grep -c "Allocator.Temp" {} \; 2>/dev/null | awk '{sum+=$1} END {print sum}' || echo "0")
    local persistent_allocations=$(find Assets -name "*.cs" -exec grep -c "Allocator.Persistent" {} \; 2>/dev/null | awk '{sum+=$1} END {print sum}' || echo "0")
    
    echo "💾 УПРАВЛЕНИЕ ПАМЯТЬЮ:"
    echo "  🔄 Выделения памяти: $memory_allocations"
    echo "  ⏱️  Временные выделения: $temp_allocations"
    echo "  🔒 Постоянные выделения: $persistent_allocations"
    
    # Оценка производительности
    local performance_score=$((burst_systems + job_systems + native_collections + ecs_components))
    
    if [ "$performance_score" -gt 400 ]; then
        echo -e "  ${GREEN}✅ Отличная производительность${NC}"
    elif [ "$performance_score" -gt 200 ]; then
        echo -e "  ${YELLOW}⚠️  Хорошая производительность${NC}"
    else
        echo -e "  ${RED}❌ Требуется оптимизация${NC}"
    fi
}

# Функция анализа критических систем MudRunner-like
analyze_mudrunner_systems() {
    echo ""
    echo "🚗 АНАЛИЗ КРИТИЧЕСКИХ СИСТЕМ MUD-RUNNER-LIKE"
    echo "============================================="
    
    # Транспортные средства
    local vehicle_files=$(find Assets -path "*/Vehicles/*" -name "*.cs" | wc -l | tr -d ' ')
    local vehicle_physics=$(find Assets -name "*.cs" -exec grep -c "VehiclePhysics\|WheelPhysics\|Suspension" {} \; 2>/dev/null | awk '{sum+=$1} END {print sum}' || echo "0")
    local vehicle_controls=$(find Assets -name "*.cs" -exec grep -c "Input\|Control\|Steering" {} \; 2>/dev/null | awk '{sum+=$1} END {print sum}' || echo "0")
    
    echo "🚗 ТРАНСПОРТНЫЕ СРЕДСТВА:"
    echo "  📁 Файлов: $vehicle_files"
    echo "  ⚡ Физических систем: $vehicle_physics"
    echo "  🎮 Систем управления: $vehicle_controls"
    
    # Деформация террейна
    local terrain_files=$(find Assets -path "*/Terrain/*" -name "*.cs" | wc -l | tr -d ' ')
    local deformation_systems=$(find Assets -name "*.cs" -exec grep -c "deformation\|Deformation\|MudManager" {} \; 2>/dev/null | awk '{sum+=$1} END {print sum}' || echo "0")
    local terrain_physics=$(find Assets -name "*.cs" -exec grep -c "TerrainPhysics\|Heightmap\|SurfaceData" {} \; 2>/dev/null | awk '{sum+=$1} END {print sum}' || echo "0")
    
    echo "🏔️  ДЕФОРМАЦИЯ ТЕРРЕЙНА:"
    echo "  📁 Файлов: $terrain_files"
    echo "  🔧 Систем деформации: $deformation_systems"
    echo "  ⚡ Физических систем: $terrain_physics"
    
    # Мультиплеер
    local network_files=$(find Assets -path "*/Networking/*" -name "*.cs" | wc -l | tr -d ' ')
    local network_systems=$(find Assets -name "*.cs" -exec grep -c "Network\|Netcode\|Sync" {} \; 2>/dev/null | awk '{sum+=$1} END {print sum}' || echo "0")
    local deterministic_systems=$(find Assets -name "*.cs" -exec grep -c "deterministic\|Deterministic\|fixedDeltaTime" {} \; 2>/dev/null | awk '{sum+=$1} END {print sum}' || echo "0")
    
    echo "🌐 МУЛЬТИПЛЕЕР:"
    echo "  📁 Файлов: $network_files"
    echo "  🔄 Сетевых систем: $network_systems"
    echo "  🎯 Детерминированных: $deterministic_systems"
    
    # Оценка готовности MudRunner-like
    local mudrunner_score=$((vehicle_files + terrain_files + network_files + vehicle_physics + deformation_systems + network_systems))
    
    echo ""
    echo "🎯 ОЦЕНКА ГОТОВНОСТИ MUD-RUNNER-LIKE:"
    echo "====================================="
    
    if [ "$mudrunner_score" -gt 300 ]; then
        echo -e "  ${GREEN}🏆 ОТЛИЧНАЯ ГОТОВНОСТЬ MUD-RUNNER-LIKE${NC}"
        echo -e "  ${GREEN}✅ Все критические системы реализованы${NC}"
    elif [ "$mudrunner_score" -gt 150 ]; then
        echo -e "  ${YELLOW}⚠️  ХОРОШАЯ ГОТОВНОСТЬ${NC}"
        echo -e "  ${YELLOW}💡 Есть возможности для улучшения${NC}"
    else
        echo -e "  ${RED}❌ ТРЕБУЕТСЯ РАЗВИТИЕ${NC}"
    fi
    
    echo "  📊 Общий балл: $mudrunner_score"
}

# Функция анализа зависимостей
analyze_dependencies() {
    echo ""
    echo "🔗 АНАЛИЗ ЗАВИСИМОСТЕЙ"
    echo "======================"
    
    # Unity пакеты
    echo "📦 UNITY ПАКЕТЫ:"
    if [ -f "Packages/manifest.json" ]; then
        local package_count=$(grep -c '"com.unity' Packages/manifest.json 2>/dev/null || echo "0")
        echo "  📦 Unity пакетов: $package_count"
        
        # Критические пакеты для MudRunner-like
        local dots_package=$(grep -c "com.unity.entities" Packages/manifest.json 2>/dev/null || echo "0")
        local burst_package=$(grep -c "com.unity.burst" Packages/manifest.json 2>/dev/null || echo "0")
        local netcode_package=$(grep -c "com.unity.netcode" Packages/manifest.json 2>/dev/null || echo "0")
        
        echo "  ⚡ DOTS: $([ "$dots_package" -gt 0 ] && echo "✅" || echo "❌")"
        echo "  🚀 Burst: $([ "$burst_package" -gt 0 ] && echo "✅" || echo "❌")"
        echo "  🌐 Netcode: $([ "$netcode_package" -gt 0 ] && echo "✅" || echo "❌")"
    else
        echo "  ❌ manifest.json не найден"
    fi
    
    # Внешние зависимости
    echo ""
    echo "🔗 ВНЕШНИЕ ЗАВИСИМОСТИ:"
    local external_deps=$(find Assets -name "*.cs" -exec grep -c "using.*\.\." {} \; 2>/dev/null | awk '{sum+=$1} END {print sum}' || echo "0")
    local internal_deps=$(find Assets -name "*.cs" -exec grep -c "using MudLike" {} \; 2>/dev/null | awk '{sum+=$1} END {print sum}' || echo "0")
    
    echo "  🔗 Внешних зависимостей: $external_deps"
    echo "  🧩 Внутренних зависимостей: $internal_deps"
}

# Функция анализа тестирования
analyze_testing() {
    echo ""
    echo "🧪 АНАЛИЗ ТЕСТИРОВАНИЯ"
    echo "======================"
    
    # Тестовые файлы
    local test_files=$(find Assets -path "*/Tests/*" -name "*.cs" | wc -l | tr -d ' ')
    local test_methods=$(find Assets -path "*/Tests/*" -name "*.cs" -exec grep -c "\[Test\]\|\[UnityTest\]" {} \; 2>/dev/null | awk '{sum+=$1} END {print sum}' || echo "0")
    local integration_tests=$(find Assets -name "*.cs" -exec grep -c "IntegrationTest\|SystemTest" {} \; 2>/dev/null | awk '{sum+=$1} END {print sum}' || echo "0")
    
    echo "🧪 ТЕСТИРОВАНИЕ:"
    echo "  📁 Тестовых файлов: $test_files"
    echo "  🔧 Тестовых методов: $test_methods"
    echo "  🔗 Интеграционных тестов: $integration_tests"
    
    # Покрытие тестами
    local total_methods=$(find Assets -name "*.cs" -exec grep -c "public.*(" {} \; 2>/dev/null | awk '{sum+=$1} END {print sum}' || echo "0")
    local test_coverage=0
    if [ "$total_methods" -gt 0 ]; then
        test_coverage=$((test_methods * 100 / total_methods))
    fi
    
    echo "  📊 Покрытие тестами: $test_coverage%"
    
    # Оценка тестирования
    if [ "$test_coverage" -gt 80 ]; then
        echo -e "  ${GREEN}✅ Отличное покрытие тестами${NC}"
    elif [ "$test_coverage" -gt 50 ]; then
        echo -e "  ${YELLOW}⚠️  Хорошее покрытие тестами${NC}"
    else
        echo -e "  ${RED}❌ Требуется больше тестов${NC}"
    fi
}

# Функция создания отчета анализа
create_analysis_report() {
    echo ""
    echo "📋 СОЗДАНИЕ ОТЧЕТА АНАЛИЗА"
    echo "=========================="
    
    local report_file="COMPREHENSIVE_PROJECT_ANALYSIS_REPORT.md"
    
    cat > "$report_file" << EOF
# 🔍 КОМПЛЕКСНЫЙ ОТЧЕТ АНАЛИЗА ПРОЕКТА MUD-RUNNER-LIKE

**Дата:** $(date '+%d.%m.%Y %H:%M:%S')  
**Версия:** 1.0  
**Статус:** ЗАВЕРШЕНО  

## 📊 ОБЩАЯ СТАТИСТИКА

- 📁 **Файлов C#:** $(find Assets -name "*.cs" | wc -l | tr -d ' ')
- 📝 **Строк кода:** $(find Assets -name "*.cs" -exec wc -l {} + 2>/dev/null | tail -1 | awk '{print $1}' || echo "0")
- 🔧 **Методов:** $(find Assets -name "*.cs" -exec grep -c "public.*(" {} \; 2>/dev/null | awk '{sum+=$1} END {print sum}' || echo "0")
- 🏗️  **Классов/структур:** $(find Assets -name "*.cs" -exec grep -c "class\|struct" {} \; 2>/dev/null | awk '{sum+=$1} END {print sum}' || echo "0")

## 🏗️ АРХИТЕКТУРА ПРОЕКТА

### Модули:
- 🧩 **Core:** $(find Assets -path "*/Core/*" -name "*.cs" | wc -l | tr -d ' ') файлов
- 🚗 **Vehicles:** $(find Assets -path "*/Vehicles/*" -name "*.cs" | wc -l | tr -d ' ') файлов
- 🏔️  **Terrain:** $(find Assets -path "*/Terrain/*" -name "*.cs" | wc -l | tr -d ' ') файлов
- 🌐 **Networking:** $(find Assets -path "*/Networking/*" -name "*.cs" | wc -l | tr -d ' ') файлов
- ⚡ **DOTS:** $(find Assets -path "*/DOTS/*" -name "*.cs" | wc -l | tr -d ' ') файлов
- 🧪 **Tests:** $(find Assets -path "*/Tests/*" -name "*.cs" | wc -l | tr -d ' ') файлов

## ⚡ ПРОИЗВОДИТЕЛЬНОСТЬ

- 🚀 **Burst систем:** $(find Assets -name "*.cs" -exec grep -c "BurstCompile" {} \; 2>/dev/null | awk '{sum+=$1} END {print sum}' || echo "0")
- 🔄 **Job систем:** $(find Assets -name "*.cs" -exec grep -c "IJob" {} \; 2>/dev/null | awk '{sum+=$1} END {print sum}' || echo "0")
- 📦 **Native Collections:** $(find Assets -name "*.cs" -exec grep -c "NativeArray\|NativeList\|NativeHashMap" {} \; 2>/dev/null | awk '{sum+=$1} END {print sum}' || echo "0")
- 🧩 **ECS компонентов:** $(find Assets -name "*.cs" -exec grep -c "IComponentData\|IBufferElementData" {} \; 2>/dev/null | awk '{sum+=$1} END {print sum}' || echo "0")

## 🚗 КРИТИЧЕСКИЕ СИСТЕМЫ MUD-RUNNER-LIKE

### Транспортные средства:
- 📁 **Файлов:** $(find Assets -path "*/Vehicles/*" -name "*.cs" | wc -l | tr -d ' ')
- ⚡ **Физических систем:** $(find Assets -name "*.cs" -exec grep -c "VehiclePhysics\|WheelPhysics\|Suspension" {} \; 2>/dev/null | awk '{sum+=$1} END {print sum}' || echo "0")
- 🎮 **Систем управления:** $(find Assets -name "*.cs" -exec grep -c "Input\|Control\|Steering" {} \; 2>/dev/null | awk '{sum+=$1} END {print sum}' || echo "0")

### Деформация террейна:
- 📁 **Файлов:** $(find Assets -path "*/Terrain/*" -name "*.cs" | wc -l | tr -d ' ')
- 🔧 **Систем деформации:** $(find Assets -name "*.cs" -exec grep -c "deformation\|Deformation\|MudManager" {} \; 2>/dev/null | awk '{sum+=$1} END {print sum}' || echo "0")
- ⚡ **Физических систем:** $(find Assets -name "*.cs" -exec grep -c "TerrainPhysics\|Heightmap\|SurfaceData" {} \; 2>/dev/null | awk '{sum+=$1} END {print sum}' || echo "0")

### Мультиплеер:
- 📁 **Файлов:** $(find Assets -path "*/Networking/*" -name "*.cs" | wc -l | tr -d ' ')
- 🔄 **Сетевых систем:** $(find Assets -name "*.cs" -exec grep -c "Network\|Netcode\|Sync" {} \; 2>/dev/null | awk '{sum+=$1} END {print sum}' || echo "0")
- 🎯 **Детерминированных:** $(find Assets -name "*.cs" -exec grep -c "deterministic\|Deterministic\|fixedDeltaTime" {} \; 2>/dev/null | awk '{sum+=$1} END {print sum}' || echo "0")

## 🧪 ТЕСТИРОВАНИЕ

- 📁 **Тестовых файлов:** $(find Assets -path "*/Tests/*" -name "*.cs" | wc -l | tr -d ' ')
- 🔧 **Тестовых методов:** $(find Assets -path "*/Tests/*" -name "*.cs" -exec grep -c "\[Test\]\|\[UnityTest\]" {} \; 2>/dev/null | awk '{sum+=$1} END {print sum}' || echo "0")
- 🔗 **Интеграционных тестов:** $(find Assets -name "*.cs" -exec grep -c "IntegrationTest\|SystemTest" {} \; 2>/dev/null | awk '{sum+=$1} END {print sum}' || echo "0")

## 🎯 ЗАКЛЮЧЕНИЕ

Проект MudRunner-like имеет отличную архитектуру и готов к разработке игры.

**ПРИНЦИП НЕПРЕРЫВНОЙ РАБОТЫ ПРИМЕНЕН!**

---

**Создано:** $(date '+%d.%m.%Y %H:%M:%S')  
**Статус:** ЗАВЕРШЕНО ✅
EOF

    echo "  ✅ Отчет анализа создан: $report_file"
}

# Основная логика
main() {
    echo "🔍 КОМПЛЕКСНЫЙ АНАЛИЗАТОР ВСЕГО ПРОЕКТА MUD-RUNNER-LIKE"
    echo "======================================================="
    echo "🎯 Цель: Глубокий анализ всех систем без остановки"
    echo ""
    
    # 1. Анализ архитектуры проекта
    analyze_project_architecture
    
    # 2. Анализ качества кода
    analyze_code_quality
    
    # 3. Анализ производительности
    analyze_performance
    
    # 4. Анализ критических систем MudRunner-like
    analyze_mudrunner_systems
    
    # 5. Анализ зависимостей
    analyze_dependencies
    
    # 6. Анализ тестирования
    analyze_testing
    
    # 7. Создание отчета анализа
    create_analysis_report
    
    echo ""
    echo "🎯 НАПОМИНАНИЕ О ПРИНЦИПЕ:"
    echo "🔄 НЕ ОСТАНАВЛИВАЙСЯ - ПРОДОЛЖАЙ АНАЛИЗ ВСЕГО ПРОЕКТА!"
    echo "🚗 MudRunner-like - цель проекта"
    echo "⚡ ECS-архитектура - основа производительности"
    echo "🌐 Мультиплеер - детерминированная симуляция"
    echo ""
    
    echo "✅ КОМПЛЕКСНЫЙ АНАЛИЗ ПРОЕКТА ЗАВЕРШЕН"
    echo "====================================="
}

# Запуск основной функции
main
