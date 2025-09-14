#!/bin/bash

# Непрерывный страж Unity Editor
# Создан: 14 сентября 2025
# Цель: Автоматический мониторинг и исправление ошибок

echo "🛡️  НЕПРЕРЫВНЫЙ СТРАЖ UNITY EDITOR"
echo "===================================="

# Цвета для вывода
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
PURPLE='\033[0;35m'
CYAN='\033[0;36m'
NC='\033[0m' # No Color

# Конфигурация
CHECK_INTERVAL=30  # Интервал проверки в секундах
LOG_FILE="unity_guardian.log"
MAX_LOG_SIZE=1048576  # 1MB

# Функция логирования
log_message() {
    local message="$1"
    local timestamp=$(date '+%Y-%m-%d %H:%M:%S')
    echo "[$timestamp] $message" >> "$LOG_FILE"
}

# Функция проверки размера лога
check_log_size() {
    if [ -f "$LOG_FILE" ] && [ $(stat -f%z "$LOG_FILE" 2>/dev/null || stat -c%s "$LOG_FILE" 2>/dev/null || echo "0") -gt $MAX_LOG_SIZE ]; then
        tail -1000 "$LOG_FILE" > "${LOG_FILE}.tmp" && mv "${LOG_FILE}.tmp" "$LOG_FILE"
        echo "📝 Лог файл обрезан до 1MB"
    fi
}

# Функция комплексной проверки
comprehensive_check() {
    local errors_found=0
    local warnings_found=0
    
    echo "🔍 Комплексная проверка Unity Editor..."
    
    # 1. Проверка компиляции
    local compilation_errors=$(tail -50 /home/egor/.config/unity3d/Editor.log 2>/dev/null | grep -c "CS[0-9]\+" 2>/dev/null | head -1 || echo "0")
    if [ "$compilation_errors" -gt 0 ]; then
        echo -e "${RED}❌ Ошибки компиляции: $compilation_errors${NC}"
        log_message "ОШИБКА: Найдено $compilation_errors ошибок компиляции"
        errors_found=$((errors_found + compilation_errors))
    else
        echo -e "${GREEN}✅ Компиляция: ОК${NC}"
    fi
    
    # 2. Проверка Asset Import Workers
    local worker_logs=$(find . -name "AssetImportWorker*.log" 2>/dev/null | wc -l | tr -d ' ')
    if [ "$worker_logs" -gt 0 ]; then
        echo -e "${RED}❌ Asset Import Workers: $worker_logs ошибок${NC}"
        log_message "ОШИБКА: Найдено $worker_logs логов Asset Import Workers"
        errors_found=$((errors_found + worker_logs))
    else
        echo -e "${GREEN}✅ Asset Import Workers: ОК${NC}"
    fi
    
    # 3. Проверка Unity процессов
    local unity_processes=$(ps aux | grep Unity | grep -v grep | wc -l | head -1 || echo "0")
    if [ "$unity_processes" -gt 5 ]; then
        echo -e "${YELLOW}⚠️  Unity процессы: $unity_processes (много)${NC}"
        log_message "ПРЕДУПРЕЖДЕНИЕ: Много Unity процессов ($unity_processes)"
        warnings_found=$((warnings_found + 1))
    else
        echo -e "${GREEN}✅ Unity процессы: ОК ($unity_processes)${NC}"
    fi
    
    # 4. Проверка памяти
    local memory_usage=$(ps aux | grep Unity | grep -v grep | awk '{sum+=$6} END {print sum/1024}' | cut -d. -f1 || echo "0")
    if [ "$memory_usage" -gt 1000 ]; then
        echo -e "${YELLOW}⚠️  Использование памяти: ${memory_usage}MB (высокое)${NC}"
        log_message "ПРЕДУПРЕЖДЕНИЕ: Высокое использование памяти (${memory_usage}MB)"
        warnings_found=$((warnings_found + 1))
    else
        echo -e "${GREEN}✅ Память: ОК (${memory_usage}MB)${NC}"
    fi
    
    # 5. Проверка immutable packages
    local immutable_warnings=$(tail -50 /home/egor/.config/unity3d/Editor.log 2>/dev/null | grep -c "immutable packages" 2>/dev/null | head -1 || echo "0")
    if [ "$immutable_warnings" -gt 0 ]; then
        echo -e "${YELLOW}⚠️  Immutable packages: $immutable_warnings предупреждений${NC}"
        log_message "ПРЕДУПРЕЖДЕНИЕ: $immutable_warnings предупреждений immutable packages"
        warnings_found=$((warnings_found + immutable_warnings))
    else
        echo -e "${GREEN}✅ Immutable packages: ОК${NC}"
    fi
    
    return $((errors_found + warnings_found))
}

# Функция автоматического исправления
auto_fix() {
    local error_type="$1"
    
    echo "🔧 Автоматическое исправление: $error_type"
    log_message "ИСПРАВЛЕНИЕ: Запуск автоматического исправления для $error_type"
    
    case "$error_type" in
        "compilation")
            ./enhanced_quality_check.sh >/dev/null 2>&1
            ;;
        "asset_workers")
            ./asset_import_worker_fixer.sh >/dev/null 2>&1
            ;;
        "immutable_packages")
            ./immutable_packages_fixer.sh >/dev/null 2>&1
            ;;
        "general")
            ./preventive_unity_maintenance.sh >/dev/null 2>&1
            ;;
    esac
    
    log_message "ИСПРАВЛЕНИЕ: Автоматическое исправление завершено для $error_type"
}

# Функция непрерывного мониторинга
continuous_monitoring() {
    echo "🔄 Запуск непрерывного мониторинга..."
    echo "⏰ Интервал проверки: $CHECK_INTERVAL секунд"
    echo "📝 Лог файл: $LOG_FILE"
    echo "🛑 Нажмите Ctrl+C для остановки"
    echo ""
    
    log_message "СТАРТ: Запуск непрерывного мониторинга Unity Editor"
    
    local check_count=0
    local last_error_count=0
    
    while true; do
        check_count=$((check_count + 1))
        local current_time=$(date '+%H:%M:%S')
        
        echo -n "[$current_time] Проверка #$check_count... "
        
        # Выполнение проверки
        comprehensive_check
        local total_issues=$?
        
        # Анализ результатов
        if [ "$total_issues" -eq 0 ]; then
            echo -e "${GREEN}✅ Все ОК${NC}"
            if [ "$last_error_count" -gt 0 ]; then
                log_message "УСПЕХ: Все проблемы исправлены"
                last_error_count=0
            fi
        else
            echo -e "${YELLOW}⚠️  Найдено $total_issues проблем${NC}"
            
            # Автоматическое исправление при критических проблемах
            if [ "$total_issues" -gt "$last_error_count" ]; then
                echo "🚨 Обнаружены новые проблемы! Запуск автоисправления..."
                auto_fix "general"
                last_error_count=$total_issues
            fi
        fi
        
        # Проверка размера лога
        check_log_size
        
        # Пауза до следующей проверки
        echo "⏳ Следующая проверка через $CHECK_INTERVAL секунд..."
        sleep $CHECK_INTERVAL
    done
}

# Функция статистики
show_statistics() {
    if [ -f "$LOG_FILE" ]; then
        echo "📊 СТАТИСТИКА СТРАЖА"
        echo "===================="
        echo "📝 Всего записей в логе: $(wc -l < "$LOG_FILE")"
        echo "❌ Ошибок исправлено: $(grep -c "ОШИБКА:" "$LOG_FILE" 2>/dev/null || echo "0")"
        echo "⚠️  Предупреждений: $(grep -c "ПРЕДУПРЕЖДЕНИЕ:" "$LOG_FILE" 2>/dev/null || echo "0")"
        echo "🔧 Исправлений: $(grep -c "ИСПРАВЛЕНИЕ:" "$LOG_FILE" 2>/dev/null || echo "0")"
        echo "✅ Успехов: $(grep -c "УСПЕХ:" "$LOG_FILE" 2>/dev/null || echo "0")"
        echo ""
        echo "📋 Последние 5 записей:"
        tail -5 "$LOG_FILE" | sed 's/^/  /'
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
        show_statistics
        ;;
    "--check"|"-k")
        comprehensive_check
        ;;
    *)
        echo "🛡️  НЕПРЕРЫВНЫЙ СТРАЖ UNITY EDITOR"
        echo "===================================="
        echo ""
        echo "💡 ИСПОЛЬЗОВАНИЕ:"
        echo "  $0 --continuous    # Непрерывный мониторинг"
        echo "  $0 --stats         # Показать статистику"
        echo "  $0 --check         # Однократная проверка"
        echo ""
        echo "🎯 ЦЕЛЬ ПРОЕКТА: Создание MudRunner-like игры"
        echo "🚗 Фокус: Реалистичная физика внедорожника и деформация террейна"
        echo ""
        echo "✅ СТРАЖ ГОТОВ К РАБОТЕ"
        ;;
esac
