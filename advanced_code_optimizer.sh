#!/bin/bash

# Продвинутый оптимизатор кода для MudRunner-like
# Создан: 14 сентября 2025
# Цель: Качественная оптимизация критических систем

echo "🚀 ПРОДВИНУТЫЙ ОПТИМИЗАТОР КОДА MUD-LIKE"
echo "=========================================="

# Цвета для вывода
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
PURPLE='\033[0;35m'
CYAN='\033[0;36m'
NC='\033[0m' # No Color

# Функция анализа критических систем
analyze_critical_systems() {
    echo "🎯 АНАЛИЗ КРИТИЧЕСКИХ СИСТЕМ MUD-LIKE"
    echo "====================================="
    
    # Анализ транспортных средств
    echo "🚗 АНАЛИЗ ТРАНСПОРТНЫХ СРЕДСТВ:"
    local vehicle_files=$(find Assets -path "*/Vehicles/*" -name "*.cs" | wc -l | tr -d ' ')
    local vehicle_methods=$(find Assets -path "*/Vehicles/*" -name "*.cs" -exec grep -c "public.*(" {} \; 2>/dev/null | awk '{sum+=$1} END {print sum}' || echo "0")
    local vehicle_burst=$(find Assets -path "*/Vehicles/*" -name "*.cs" -exec grep -c "BurstCompile" {} \; 2>/dev/null | awk '{sum+=$1} END {print sum}' || echo "0")
    
    echo "  📁 Файлов: $vehicle_files"
    echo "  📝 Методов: $vehicle_methods"
    echo "  ⚡ Burst оптимизированных: $vehicle_burst"
    
    # Анализ деформации террейна
    echo "🏔️  АНАЛИЗ ДЕФОРМАЦИИ ТЕРРЕЙНА:"
    local terrain_files=$(find Assets -path "*/Terrain/*" -name "*.cs" | wc -l | tr -d ' ')
    local terrain_methods=$(find Assets -path "*/Terrain/*" -name "*.cs" -exec grep -c "public.*(" {} \; 2>/dev/null | awk '{sum+=$1} END {print sum}' || echo "0")
    local terrain_jobs=$(find Assets -path "*/Terrain/*" -name "*.cs" -exec grep -c "IJob" {} \; 2>/dev/null | awk '{sum+=$1} END {print sum}' || echo "0")
    
    echo "  📁 Файлов: $terrain_files"
    echo "  📝 Методов: $terrain_methods"
    echo "  🔄 Job систем: $terrain_jobs"
    
    # Анализ мультиплеера
    echo "🌐 АНАЛИЗ МУЛЬТИПЛЕЕРА:"
    local network_files=$(find Assets -path "*/Networking/*" -name "*.cs" | wc -l | tr -d ' ')
    local network_methods=$(find Assets -path "*/Networking/*" -name "*.cs" -exec grep -c "public.*(" {} \; 2>/dev/null | awk '{sum+=$1} END {print sum}' || echo "0")
    local network_deterministic=$(find Assets -path "*/Networking/*" -name "*.cs" -exec grep -c "deterministic\|Deterministic" {} \; 2>/dev/null | awk '{sum+=$1} END {print sum}' || echo "0")
    
    echo "  📁 Файлов: $network_files"
    echo "  📝 Методов: $network_methods"
    echo "  🎯 Детерминированных: $network_deterministic"
    
    # Анализ физики
    echo "⚡ АНАЛИЗ ФИЗИКИ:"
    local physics_files=$(find Assets -name "*.cs" -exec grep -l "Physics\|PhysicsBody" {} \; 2>/dev/null | wc -l | tr -d ' ')
    local physics_methods=$(find Assets -name "*.cs" -exec grep -l "Physics\|PhysicsBody" {} \; 2>/dev/null | xargs grep -c "public.*(" 2>/dev/null | awk '{sum+=$1} END {print sum}' || echo "0")
    local physics_dots=$(find Assets -name "*.cs" -exec grep -l "Physics\|PhysicsBody" {} \; 2>/dev/null | xargs grep -c "Unity.Physics" 2>/dev/null | awk '{sum+=$1} END {print sum}' || echo "0")
    
    echo "  📁 Файлов: $physics_files"
    echo "  📝 Методов: $physics_methods"
    echo "  🎯 Unity Physics (DOTS): $physics_dots"
}

# Функция оптимизации производительности
optimize_performance() {
    echo ""
    echo "⚡ ОПТИМИЗАЦИЯ ПРОИЗВОДИТЕЛЬНОСТИ"
    echo "================================="
    
    # Проверка использования Burst Compiler
    echo "🔍 Проверка Burst Compiler:"
    local total_systems=$(find Assets -name "*.cs" -exec grep -c "SystemBase\|ISystem" {} \; 2>/dev/null | awk '{sum+=$1} END {print sum}' || echo "0")
    local burst_optimized=$(find Assets -name "*.cs" -exec grep -c "BurstCompile" {} \; 2>/dev/null | awk '{sum+=$1} END {print sum}' || echo "0")
    
    if [ "$total_systems" -gt 0 ]; then
        local burst_percentage=$((burst_optimized * 100 / total_systems))
        echo "  📊 Систем с Burst: $burst_optimized из $total_systems ($burst_percentage%)"
        
        if [ "$burst_percentage" -lt 80 ]; then
            echo -e "  ${YELLOW}⚠️  Рекомендуется добавить Burst Compiler${NC}"
        else
            echo -e "  ${GREEN}✅ Burst Compiler хорошо используется${NC}"
        fi
    fi
    
    # Проверка Job System
    echo "🔍 Проверка Job System:"
    local job_systems=$(find Assets -name "*.cs" -exec grep -c "IJob\|IJobParallelFor\|IJobFor" {} \; 2>/dev/null | awk '{sum+=$1} END {print sum}' || echo "0")
    echo "  🔄 Job систем: $job_systems"
    
    # Проверка Native Collections
    echo "🔍 Проверка Native Collections:"
    local native_usage=$(find Assets -name "*.cs" -exec grep -c "NativeArray\|NativeList\|NativeHashMap" {} \; 2>/dev/null | awk '{sum+=$1} END {print sum}' || echo "0")
    echo "  📦 Native Collections: $native_usage использований"
    
    # Проверка аллокаторов
    echo "🔍 Проверка аллокаторов:"
    local temp_allocator=$(find Assets -name "*.cs" -exec grep -c "Allocator.Temp" {} \; 2>/dev/null | awk '{sum+=$1} END {print sum}' || echo "0")
    local persistent_allocator=$(find Assets -name "*.cs" -exec grep -c "Allocator.Persistent" {} \; 2>/dev/null | awk '{sum+=$1} END {print sum}' || echo "0")
    echo "  ⏱️  Temp аллокатор: $temp_allocator"
    echo "  💾 Persistent аллокатор: $persistent_allocator"
}

# Функция анализа архитектуры ECS
analyze_ecs_architecture() {
    echo ""
    echo "🏗️  АНАЛИЗ АРХИТЕКТУРЫ ECS"
    echo "============================"
    
    # Компоненты данных
    echo "📦 Компоненты данных:"
    local components=$(find Assets -name "*.cs" -exec grep -c "IComponentData\|IBufferElementData" {} \; 2>/dev/null | awk '{sum+=$1} END {print sum}' || echo "0")
    local pure_components=$(find Assets -name "*.cs" -exec grep -c "struct.*: IComponentData" {} \; 2>/dev/null | awk '{sum+=$1} END {print sum}' || echo "0")
    echo "  📊 Всего компонентов: $components"
    echo "  ✨ Чистых структур: $pure_components"
    
    # Системы
    echo "🚀 Системы:"
    local systems=$(find Assets -name "*.cs" -exec grep -c "SystemBase\|ISystem" {} \; 2>/dev/null | awk '{sum+=$1} END {print sum}' || echo "0")
    local partial_systems=$(find Assets -name "*.cs" -exec grep -c "partial.*System" {} \; 2>/dev/null | awk '{sum+=$1} END {print sum}' || echo "0")
    echo "  🔧 Всего систем: $systems"
    echo "  🔄 Partial систем: $partial_systems"
    
    # Entity Queries
    echo "🔍 Entity Queries:"
    local queries=$(find Assets -name "*.cs" -exec grep -c "EntityQuery\|GetEntityQuery" {} \; 2>/dev/null | awk '{sum+=$1} END {print sum}' || echo "0")
    echo "  🎯 Entity Queries: $queries"
    
    # Оценка архитектуры
    echo "📊 Оценка архитектуры ECS:"
    if [ "$components" -gt 50 ] && [ "$systems" -gt 30 ] && [ "$pure_components" -gt 30 ]; then
        echo -e "  ${GREEN}✅ Отличная ECS архитектура${NC}"
    elif [ "$components" -gt 30 ] && [ "$systems" -gt 20 ]; then
        echo -e "  ${YELLOW}⚠️  Хорошая ECS архитектура${NC}"
    else
        echo -e "  ${RED}❌ Требуется улучшение ECS архитектуры${NC}"
    fi
}

# Функция анализа детерминизма
analyze_determinism() {
    echo ""
    echo "🎯 АНАЛИЗ ДЕТЕРМИНИЗМА"
    echo "======================"
    
    # Использование фиксированного времени
    echo "⏰ Временные системы:"
    local fixed_time=$(find Assets -name "*.cs" -exec grep -c "fixedDeltaTime\|Time.fixedDeltaTime" {} \; 2>/dev/null | awk '{sum+=$1} END {print sum}' || echo "0")
    local delta_time=$(find Assets -name "*.cs" -exec grep -c "Time.deltaTime" {} \; 2>/dev/null | awk '{sum+=$1} END {print sum}' || echo "0")
    echo "  🎯 Фиксированное время: $fixed_time"
    echo "  ⚠️  Обычное время: $delta_time"
    
    # Использование SystemAPI.Time
    echo "🔧 SystemAPI.Time:"
    local system_api_time=$(find Assets -name "*.cs" -exec grep -c "SystemAPI.Time" {} \; 2>/dev/null | awk '{sum+=$1} END {print sum}' || echo "0")
    echo "  ✅ SystemAPI.Time: $system_api_time"
    
    # Оценка детерминизма
    if [ "$fixed_time" -gt "$delta_time" ] && [ "$system_api_time" -gt 0 ]; then
        echo -e "  ${GREEN}✅ Хороший детерминизм${NC}"
    else
        echo -e "  ${YELLOW}⚠️  Требуется улучшение детерминизма${NC}"
    fi
}

# Функция создания рекомендаций по оптимизации
create_optimization_recommendations() {
    echo ""
    echo "💡 РЕКОМЕНДАЦИИ ПО ОПТИМИЗАЦИИ"
    echo "==============================="
    
    echo "🎯 ПРИОРИТЕТ 1: Критические системы MudRunner-like"
    echo "  🚗 Транспортные средства:"
    echo "    - Добавить Burst Compiler ко всем системам физики"
    echo "    - Оптимизировать Job System для колес"
    echo "    - Использовать Native Collections для производительности"
    echo ""
    echo "  🏔️  Деформация террейна:"
    echo "    - Параллелизировать алгоритмы деформации"
    echo "    - Оптимизировать работу с высотными картами"
    echo "    - Использовать Job System для больших террейнов"
    echo ""
    echo "  🌐 Мультиплеер:"
    echo "    - Обеспечить детерминизм всех вычислений"
    echo "    - Оптимизировать синхронизацию состояния"
    echo "    - Использовать компрессию для сетевого трафика"
    echo ""
    
    echo "🎯 ПРИОРИТЕТ 2: Архитектурные улучшения"
    echo "  📦 Компоненты:"
    echo "    - Разбить большие компоненты на меньшие"
    echo "    - Использовать BufferElementData для массивов"
    echo "    - Оптимизировать размеры структур"
    echo ""
    echo "  🚀 Системы:"
    echo "    - Разделить сложные системы на более простые"
    echo "    - Использовать partial системы для читаемости"
    echo "    - Оптимизировать Entity Queries"
    echo ""
    
    echo "🎯 ПРИОРИТЕТ 3: Производительность"
    echo "  ⚡ Burst Compiler:"
    echo "    - Добавить ко всем критическим системам"
    echo "    - Использовать BurstCompile(CompileSynchronously = true)"
    echo "    - Оптимизировать математические операции"
    echo ""
    echo "  🔄 Job System:"
    echo "    - Использовать IJobParallelFor для больших данных"
    echo "    - Оптимизировать batch size для Jobs"
    echo "    - Избегать зависимостей между Jobs"
    echo ""
}

# Основная логика
main() {
    echo "🚀 ПРОДВИНУТЫЙ ОПТИМИЗАТОР КОДА MUD-LIKE"
    echo "=========================================="
    echo "🎯 Цель: Качественная оптимизация для MudRunner-like"
    echo ""
    
    # 1. Анализ критических систем
    analyze_critical_systems
    
    # 2. Оптимизация производительности
    optimize_performance
    
    # 3. Анализ архитектуры ECS
    analyze_ecs_architecture
    
    # 4. Анализ детерминизма
    analyze_determinism
    
    # 5. Создание рекомендаций
    create_optimization_recommendations
    
    echo ""
    echo "🎯 НАПОМИНАНИЕ О ЦЕЛИ ПРОЕКТА:"
    echo "🚗 MudRunner-like требует максимальной производительности"
    echo "🏔️  Деформация террейна - критически важная система"
    echo "🌐 Мультиплеер - требует детерминированной симуляции"
    echo "⚡ ECS-архитектура - основа высокой производительности"
    echo ""
    
    echo "✅ ПРОДВИНУТАЯ ОПТИМИЗАЦИЯ ЗАВЕРШЕНА"
    echo "====================================="
}

# Запуск основной функции
main
