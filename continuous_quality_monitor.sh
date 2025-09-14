#!/bin/bash

# Непрерывный мониторинг качества для MudRunner-like
# Создан: 14 сентября 2025
# Цель: Постоянное поддержание высокого качества проекта

echo "📊 НЕПРЕРЫВНЫЙ МОНИТОРИНГ КАЧЕСТВА MUD-LIKE"
echo "==========================================="

# Цвета для вывода
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
PURPLE='\033[0;35m'
CYAN='\033[0;36m'
NC='\033[0m' # No Color

# Конфигурация мониторинга
QUALITY_LOG="quality_monitor.log"
MONITOR_INTERVAL=60  # Интервал мониторинга в секундах

# Функция логирования качества
log_quality() {
    local message="$1"
    local timestamp=$(date '+%Y-%m-%d %H:%M:%S')
    echo "[$timestamp] $message" | tee -a "$QUALITY_LOG"
}

# Функция мониторинга качества кода
monitor_code_quality() {
    echo "🔍 МОНИТОРИНГ КАЧЕСТВА КОДА"
    echo "============================"
    
    # Быстрая проверка качества
    local compilation_errors=$(tail -50 /home/egor/.config/unity3d/Editor.log 2>/dev/null | grep -c "CS[0-9]\+" 2>/dev/null | head -1 || echo "0")
    local cs_files=$(find Assets -name "*.cs" | wc -l | tr -d ' ')
    local total_lines=$(find Assets -name "*.cs" -exec wc -l {} + 2>/dev/null | tail -1 | awk '{print $1}' || echo "0")
    
    echo "📊 Статистика кода:"
    echo "  📁 Файлов C#: $cs_files"
    echo "  📝 Строк кода: $total_lines"
    echo "  ❌ Ошибок компиляции: $compilation_errors"
    
    # Анализ критических систем MudRunner-like
    local vehicle_files=$(find Assets -path "*/Vehicles/*" -name "*.cs" | wc -l | tr -d ' ')
    local terrain_files=$(find Assets -path "*/Terrain/*" -name "*.cs" | wc -l | tr -d ' ')
    local network_files=$(find Assets -path "*/Networking/*" -name "*.cs" | wc -l | tr -d ' ')
    
    echo "🎯 Критические системы MudRunner-like:"
    echo "  🚗 Транспортные средства: $vehicle_files файлов"
    echo "  🏔️  Деформация террейна: $terrain_files файлов"
    echo "  🌐 Мультиплеер: $network_files файлов"
    
    # Оценка качества
    if [ "$compilation_errors" -eq 0 ] && [ "$cs_files" -gt 100 ]; then
        echo -e "  ${GREEN}✅ Отличное качество кода${NC}"
        log_quality "QUALITY: Отличное качество кода - $cs_files файлов, $total_lines строк"
    elif [ "$compilation_errors" -eq 0 ]; then
        echo -e "  ${YELLOW}⚠️  Хорошее качество кода${NC}"
        log_quality "QUALITY: Хорошее качество кода - $cs_files файлов, $total_lines строк"
    else
        echo -e "  ${RED}❌ Требуется улучшение качества${NC}"
        log_quality "ERROR: Требуется улучшение качества - $compilation_errors ошибок"
    fi
    
    return $compilation_errors
}

# Функция мониторинга производительности
monitor_performance() {
    echo ""
    echo "⚡ МОНИТОРИНГ ПРОИЗВОДИТЕЛЬНОСТИ"
    echo "================================"
    
    # Проверка Unity процессов
    local unity_processes=$(ps aux | grep Unity | grep -v grep | wc -l | head -1 || echo "0")
    local memory_usage=$(ps aux | grep Unity | grep -v grep | awk '{sum+=$6} END {print sum/1024}' | cut -d. -f1 || echo "0")
    
    echo "🖥️  Системные ресурсы:"
    echo "  🔄 Unity процессов: $unity_processes"
    echo "  💾 Использование памяти: ${memory_usage}MB"
    
    # Проверка оптимизации ECS
    local burst_systems=$(find Assets -name "*.cs" -exec grep -c "BurstCompile" {} \; 2>/dev/null | awk '{sum+=$1} END {print sum}' || echo "0")
    local job_systems=$(find Assets -name "*.cs" -exec grep -c "IJob" {} \; 2>/dev/null | awk '{sum+=$1} END {print sum}' || echo "0")
    local native_collections=$(find Assets -name "*.cs" -exec grep -c "NativeArray\|NativeList" {} \; 2>/dev/null | awk '{sum+=$1} END {print sum}' || echo "0")
    
    echo "⚡ ECS оптимизация:"
    echo "  🚀 Burst систем: $burst_systems"
    echo "  🔄 Job систем: $job_systems"
    echo "  📦 Native Collections: $native_collections"
    
    # Оценка производительности
    if [ "$burst_systems" -gt 50 ] && [ "$job_systems" -gt 10 ] && [ "$memory_usage" -lt 1000 ]; then
        echo -e "  ${GREEN}✅ Отличная производительность${NC}"
        log_quality "PERFORMANCE: Отличная производительность - $burst_systems Burst, $job_systems Jobs"
    elif [ "$burst_systems" -gt 20 ] && [ "$memory_usage" -lt 1500 ]; then
        echo -e "  ${YELLOW}⚠️  Хорошая производительность${NC}"
        log_quality "PERFORMANCE: Хорошая производительность - $burst_systems Burst, $job_systems Jobs"
    else
        echo -e "  ${RED}❌ Требуется оптимизация производительности${NC}"
        log_quality "WARNING: Требуется оптимизация производительности - $memory_usage MB памяти"
    fi
}

# Функция мониторинга соответствия цели проекта
monitor_project_alignment() {
    echo ""
    echo "🎯 МОНИТОРИНГ СООТВЕТСТВИЯ ЦЕЛИ ПРОЕКТА"
    echo "======================================="
    
    # Проверка транспортных средств (MudRunner-like)
    local vehicle_physics=$(find Assets -name "*.cs" -exec grep -l "VehiclePhysics\|WheelPhysics" {} \; 2>/dev/null | wc -l | tr -d ' ')
    local realistic_physics=$(find Assets -name "*.cs" -exec grep -c "realistic\|Realistic\|mud\|Mud" {} \; 2>/dev/null | awk '{sum+=$1} END {print sum}' || echo "0")
    
    echo "🚗 Транспортные средства (MudRunner-like):"
    echo "  ⚡ Физика транспортных средств: $vehicle_physics файлов"
    echo "  🏔️  Реалистичная физика: $realistic_physics упоминаний"
    
    # Проверка деформации террейна
    local terrain_deformation=$(find Assets -name "*.cs" -exec grep -l "deformation\|Deformation\|Terrain" {} \; 2>/dev/null | wc -l | tr -d ' ')
    local mud_systems=$(find Assets -name "*.cs" -exec grep -c "MudManager\|mud\|Mud" {} \; 2>/dev/null | awk '{sum+=$1} END {print sum}' || echo "0")
    
    echo "🏔️  Деформация террейна:"
    echo "  🔧 Системы деформации: $terrain_deformation файлов"
    echo "  🏔️  Системы грязи: $mud_systems упоминаний"
    
    # Проверка мультиплеера
    local network_systems=$(find Assets -name "*.cs" -exec grep -l "Network\|network\|Netcode" {} \; 2>/dev/null | wc -l | tr -d ' ')
    local deterministic=$(find Assets -name "*.cs" -exec grep -c "deterministic\|Deterministic\|fixedDeltaTime" {} \; 2>/dev/null | awk '{sum+=$1} END {print sum}' || echo "0")
    
    echo "🌐 Мультиплеер:"
    echo "  🔄 Сетевые системы: $network_systems файлов"
    echo "  🎯 Детерминированные системы: $deterministic упоминаний"
    
    # Оценка соответствия цели
    local total_score=$((vehicle_physics + terrain_deformation + network_systems + realistic_physics + mud_systems + deterministic))
    
    if [ "$total_score" -gt 100 ]; then
        echo -e "  ${GREEN}✅ Отличное соответствие цели MudRunner-like${NC}"
        log_quality "ALIGNMENT: Отличное соответствие цели - $total_score баллов"
    elif [ "$total_score" -gt 50 ]; then
        echo -e "  ${YELLOW}⚠️  Хорошее соответствие цели${NC}"
        log_quality "ALIGNMENT: Хорошее соответствие цели - $total_score баллов"
    else
        echo -e "  ${RED}❌ Требуется улучшение соответствия цели${NC}"
        log_quality "WARNING: Требуется улучшение соответствия цели - $total_score баллов"
    fi
}

# Функция непрерывного мониторинга
continuous_monitoring() {
    echo "🔄 НЕПРЕРЫВНЫЙ МОНИТОРИНГ КАЧЕСТВА"
    echo "=================================="
    echo "⏰ Интервал мониторинга: $MONITOR_INTERVAL секунд"
    echo "📝 Лог файл: $QUALITY_LOG"
    echo "🛑 Нажмите Ctrl+C для остановки"
    echo ""
    
    log_quality "MONITOR: Запуск непрерывного мониторинга качества"
    
    local monitor_count=0
    local quality_issues=0
    
    while true; do
        monitor_count=$((monitor_count + 1))
        local current_time=$(date '+%H:%M:%S')
        
        echo -n "[$current_time] Мониторинг #$monitor_count... "
        
        # Выполнение мониторинга
        monitor_code_quality
        local code_errors=$?
        
        monitor_performance
        monitor_project_alignment
        
        # Анализ результатов
        if [ "$code_errors" -eq 0 ]; then
            echo -e "${GREEN}✅ Качество отличное${NC}"
            if [ "$quality_issues" -gt 0 ]; then
                log_quality "IMPROVEMENT: Качество улучшилось - проблемы решены"
                quality_issues=0
            fi
        else
            echo -e "${YELLOW}⚠️  Найдены проблемы качества${NC}"
            quality_issues=$((quality_issues + code_errors))
            log_quality "ISSUE: Найдены проблемы качества - $code_errors ошибок"
        fi
        
        echo "⏳ Следующий мониторинг через $MONITOR_INTERVAL секунд..."
        sleep $MONITOR_INTERVAL
    done
}

# Функция показа статистики качества
show_quality_statistics() {
    echo "📊 СТАТИСТИКА КАЧЕСТВА"
    echo "======================"
    
    if [ -f "$QUALITY_LOG" ]; then
        echo "📝 Всего записей в логе: $(wc -l < "$QUALITY_LOG")"
        echo "✅ Успешных проверок: $(grep -c "QUALITY: Отличное\|PERFORMANCE: Отличная\|ALIGNMENT: Отличное" "$QUALITY_LOG" 2>/dev/null || echo "0")"
        echo "⚠️  Предупреждений: $(grep -c "WARNING:\|ISSUE:" "$QUALITY_LOG" 2>/dev/null || echo "0")"
        echo "🔧 Улучшений: $(grep -c "IMPROVEMENT:" "$QUALITY_LOG" 2>/dev/null || echo "0")"
        echo ""
        echo "📋 Последние 5 записей:"
        tail -5 "$QUALITY_LOG" | sed 's/^/  /'
    else
        echo "📊 Лог файл не найден. Запустите мониторинг для создания статистики."
    fi
}

# Основная логика
case "$1" in
    "--continuous"|"-c")
        continuous_monitoring
        ;;
    "--stats"|"-s")
        show_quality_statistics
        ;;
    "--monitor"|"-m")
        monitor_code_quality
        monitor_performance
        monitor_project_alignment
        ;;
    *)
        echo "📊 НЕПРЕРЫВНЫЙ МОНИТОРИНГ КАЧЕСТВА MUD-LIKE"
        echo "==========================================="
        echo ""
        echo "💡 ИСПОЛЬЗОВАНИЕ:"
        echo "  $0 --continuous    # Непрерывный мониторинг"
        echo "  $0 --stats         # Показать статистику"
        echo "  $0 --monitor       # Однократная проверка"
        echo ""
        echo "🎯 ЦЕЛЬ ПРОЕКТА: Создание MudRunner-like игры"
        echo "🚗 Фокус: Реалистичная физика и деформация террейна"
        echo ""
        echo "✅ МОНИТОРИНГ КАЧЕСТВА ГОТОВ К РАБОТЕ"
        ;;
esac
