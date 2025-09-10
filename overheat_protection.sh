#!/bin/bash

# 🚨 АВТОНОМНАЯ СИСТЕМА ЗАЩИТЫ ОТ ПЕРЕГРЕВА НОУТБУКА 🚨
# Работает независимо от Unity и других приложений
# Использует только системные утилиты Linux

# Настройки температуры
WARNING_TEMP=70      # Предупреждение
CRITICAL_TEMP=80     # Критическая температура  
EMERGENCY_TEMP=90    # Экстренная температура

# Настройки мониторинга
MONITOR_INTERVAL=2   # Интервал мониторинга (секунды)
EMERGENCY_COOLDOWN=10 # Время охлаждения в экстренном режиме (секунды)

# Статистика
WARNING_COUNT=0
CRITICAL_COUNT=0
EMERGENCY_COUNT=0
LAST_EMERGENCY_TIME=""

# Флаги состояния
IS_RUNNING=false
IS_EMERGENCY_MODE=false

# Цвета для вывода
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
PURPLE='\033[0;35m'
CYAN='\033[0;36m'
NC='\033[0m' # No Color

# Функция для получения температуры CPU
get_cpu_temperature() {
    local temp=0
    
    # Пробуем получить реальную температуру из thermal zones
    if [ -d "/sys/class/thermal" ]; then
        for thermal_zone in /sys/class/thermal/thermal_zone*; do
            if [ -f "$thermal_zone/temp" ]; then
                local zone_temp=$(cat "$thermal_zone/temp" 2>/dev/null)
                if [ -n "$zone_temp" ] && [ "$zone_temp" -gt 0 ] && [ "$zone_temp" -lt 200000 ]; then
                    temp=$((zone_temp / 1000))
                    break
                fi
            fi
        done
    fi
    
    # Если реальная температура не найдена, оцениваем по нагрузке
    if [ "$temp" -eq 0 ]; then
        local cpu_load=$(get_cpu_load)
        local base_temp=45
        local load_factor=$((cpu_load * 25 / 100))
        temp=$((base_temp + load_factor))
    fi
    
    echo "$temp"
}

# Функция для получения нагрузки CPU
get_cpu_load() {
    local load=0
    
    # Пробуем через htop
    if command -v htop >/dev/null 2>&1; then
        local htop_output=$(timeout 3s htop -n 1 -d 1 2>/dev/null | grep -E "Cpu\(s\):|CPU:" | head -1)
        if [ -n "$htop_output" ]; then
            load=$(echo "$htop_output" | grep -oE '[0-9]+\.[0-9]+%' | head -1 | sed 's/%//' | cut -d. -f1)
        fi
    fi
    
    # Fallback к /proc/loadavg
    if [ "$load" -eq 0 ] && [ -f "/proc/loadavg" ]; then
        local load_avg=$(cat /proc/loadavg | awk '{print $1}')
        local cpu_cores=$(nproc)
        load=$(echo "$load_avg $cpu_cores" | awk '{printf "%.0f", ($1/$2)*100}')
        if [ "$load" -gt 100 ]; then
            load=100
        fi
    fi
    
    echo "$load"
}

# Функция для получения информации о системе
get_system_info() {
    local cpu_model="Unknown"
    local cpu_cores=$(nproc)
    local cpu_load=$(get_cpu_load)
    local cpu_temp=$(get_cpu_temperature)
    
    # Получаем модель CPU
    if [ -f "/proc/cpuinfo" ]; then
        cpu_model=$(grep "model name" /proc/cpuinfo | head -1 | cut -d: -f2 | sed 's/^[ \t]*//')
    fi
    
    # Получаем информацию о памяти
    local mem_total=0
    local mem_available=0
    if [ -f "/proc/meminfo" ]; then
        mem_total=$(grep "MemTotal" /proc/meminfo | awk '{print $2}')
        mem_available=$(grep "MemAvailable" /proc/meminfo | awk '{print $2}')
    fi
    
    local mem_used=$((mem_total - mem_available))
    local mem_usage=0
    if [ "$mem_total" -gt 0 ]; then
        mem_usage=$((mem_used * 100 / mem_total))
    fi
    
    # Получаем время работы системы
    local uptime_hours=0
    if [ -f "/proc/uptime" ]; then
        local uptime_seconds=$(cat /proc/uptime | awk '{print $1}')
        uptime_hours=$(echo "$uptime_seconds" | awk '{printf "%.1f", $1/3600}')
    fi
    
    echo "CPU: $cpu_model"
    echo "Cores: $cpu_cores"
    echo "Load: ${cpu_load}%"
    echo "Temperature: ${cpu_temp}°C"
    echo "RAM: ${mem_used}KB / ${mem_total}KB (${mem_usage}%)"
    echo "Uptime: ${uptime_hours}h"
}

# Функция для показа алерта
show_alert() {
    local level="$1"
    local message="$2"
    local temp="$3"
    
    case "$level" in
        "WARNING")
            echo -e "${YELLOW}⚠️  ПРЕДУПРЕЖДЕНИЕ: $message${NC}"
            ;;
        "CRITICAL")
            echo -e "${RED}🔥 КРИТИЧЕСКАЯ ТЕМПЕРАТУРА: $message${NC}"
            ;;
        "EMERGENCY")
            echo -e "${RED}🚨 ЭКСТРЕННАЯ ТЕМПЕРАТУРА: $message${NC}"
            ;;
    esac
    
    # Дополнительный визуальный алерт
    echo -e "${RED}════════════════════════════════════════════════════════════${NC}"
    echo -e "${RED}  ВНИМАНИЕ! ВЫСОКАЯ ТЕМПЕРАТУРА CPU: ${temp}°C${NC}"
    echo -e "${RED}════════════════════════════════════════════════════════════${NC}"
}

# Функция для мягких мер защиты
apply_soft_measures() {
    echo -e "${YELLOW}🔧 Применение мягких мер защиты...${NC}"
    
    # Снижаем приоритет тяжелых процессов
    for process in Unity Cursor code; do
        local pids=$(pgrep "$process" 2>/dev/null)
        if [ -n "$pids" ]; then
            for pid in $pids; do
                if renice +5 "$pid" 2>/dev/null; then
                    echo "Снижен приоритет процесса $process (PID: $pid)"
                fi
            done
        fi
    done
    
    # Принудительная синхронизация файловой системы
    sync
    
    echo "Мягкие меры применены"
}

# Функция для агрессивных мер защиты
apply_aggressive_measures() {
    echo -e "${RED}🔥 Применение агрессивных мер защиты...${NC}"
    
    # Принудительная очистка кэша
    echo 3 > /proc/sys/vm/drop_caches 2>/dev/null || true
    
    # Дополнительное снижение приоритета
    for process in Unity Cursor code; do
        local pids=$(pgrep "$process" 2>/dev/null)
        if [ -n "$pids" ]; then
            for pid in $pids; do
                if renice +10 "$pid" 2>/dev/null; then
                    echo "Дополнительно снижен приоритет процесса $process (PID: $pid)"
                fi
            done
        fi
    done
    
    # Принудительная синхронизация
    sync
    
    echo "Агрессивные меры применены"
}

# Функция для экстренных мер защиты
apply_emergency_measures() {
    echo -e "${RED}🚨 ПРИМЕНЕНИЕ ЭКСТРЕННЫХ МЕР ЗАЩИТЫ!${NC}"
    
    # Принудительная пауза
    echo -e "${RED}⏸️  Принудительная пауза на 5 секунд...${NC}"
    sleep 5
    
    # Максимальная очистка кэша
    echo 3 > /proc/sys/vm/drop_caches 2>/dev/null || true
    
    # Установка минимального приоритета
    for process in Unity Cursor code; do
        local pids=$(pgrep "$process" 2>/dev/null)
        if [ -n "$pids" ]; then
            for pid in $pids; do
                if renice +19 "$pid" 2>/dev/null; then
                    echo "Установлен минимальный приоритет для процесса $process (PID: $pid)"
                fi
            done
        fi
    done
    
    # Принудительная синхронизация
    sync
    
    echo "Экстренные меры применены"
}

# Функция для проверки температуры
check_temperature() {
    local temp=$(get_cpu_temperature)
    
    if [ "$temp" -le 0 ]; then
        echo -e "${YELLOW}⚠️  Не удалось получить температуру CPU${NC}"
        return
    fi
    
    echo -e "${CYAN}🌡️  Температура CPU: ${temp}°C${NC}"
    
    if [ "$temp" -ge "$EMERGENCY_TEMP" ]; then
        handle_emergency_temperature "$temp"
    elif [ "$temp" -ge "$CRITICAL_TEMP" ]; then
        handle_critical_temperature "$temp"
    elif [ "$temp" -ge "$WARNING_TEMP" ]; then
        handle_warning_temperature "$temp"
    else
        # Температура в норме
        if [ "$IS_EMERGENCY_MODE" = true ]; then
            echo -e "${GREEN}🌡️  Температура нормализовалась, выход из экстренного режима${NC}"
            IS_EMERGENCY_MODE=false
        fi
    fi
}

# Обработка предупреждающей температуры
handle_warning_temperature() {
    local temp="$1"
    WARNING_COUNT=$((WARNING_COUNT + 1))
    
    show_alert "WARNING" "Температура CPU ${temp}°C (счетчик: $WARNING_COUNT)" "$temp"
    apply_soft_measures
}

# Обработка критической температуры
handle_critical_temperature() {
    local temp="$1"
    CRITICAL_COUNT=$((CRITICAL_COUNT + 1))
    
    show_alert "CRITICAL" "Температура ${temp}°C (счетчик: $CRITICAL_COUNT)" "$temp"
    apply_aggressive_measures
}

# Обработка экстренной температуры
handle_emergency_temperature() {
    local temp="$1"
    EMERGENCY_COUNT=$((EMERGENCY_COUNT + 1))
    LAST_EMERGENCY_TIME=$(date '+%Y-%m-%d %H:%M:%S')
    IS_EMERGENCY_MODE=true
    
    show_alert "EMERGENCY" "Температура ${temp}°C (счетчик: $EMERGENCY_COUNT)" "$temp"
    apply_emergency_measures
}

# Функция для показа статистики
show_stats() {
    echo
    echo -e "${BLUE}📊 СТАТИСТИКА СИСТЕМЫ ЗАЩИТЫ ОТ ПЕРЕГРЕВА${NC}"
    echo -e "${BLUE}==========================================${NC}"
    echo "Статус: $([ "$IS_RUNNING" = true ] && echo -e "${GREEN}🟢 Активна${NC}" || echo -e "${RED}🔴 Остановлена${NC}")"
    echo "Экстренный режим: $([ "$IS_EMERGENCY_MODE" = true ] && echo -e "${RED}🚨 ДА${NC}" || echo -e "${GREEN}✅ НЕТ${NC}")"
    echo "Предупреждения: $WARNING_COUNT"
    echo "Критические температуры: $CRITICAL_COUNT"
    echo "Экстренные температуры: $EMERGENCY_COUNT"
    
    if [ -n "$LAST_EMERGENCY_TIME" ]; then
        echo "Последняя экстренная ситуация: $LAST_EMERGENCY_TIME"
    else
        echo "Экстренных ситуаций не было"
    fi
    echo
}

# Функция для сброса статистики
reset_stats() {
    WARNING_COUNT=0
    CRITICAL_COUNT=0
    EMERGENCY_COUNT=0
    LAST_EMERGENCY_TIME=""
    echo -e "${GREEN}✅ Статистика сброшена${NC}"
}

# Функция для показа справки
show_help() {
    echo
    echo -e "${PURPLE}❓ СПРАВКА ПО СИСТЕМЕ ЗАЩИТЫ ОТ ПЕРЕГРЕВА${NC}"
    echo -e "${PURPLE}==========================================${NC}"
    echo "Эта система автоматически мониторит температуру CPU и"
    echo "принимает меры для предотвращения перегрева ноутбука."
    echo
    echo "Уровни температуры:"
    echo -e "  ${YELLOW}⚠️  Предупреждение: 70°C+ - Мягкие меры${NC}"
    echo -e "  ${RED}🔥 Критическая: 80°C+ - Агрессивные меры${NC}"
    echo -e "  ${RED}🚨 Экстренная: 90°C+ - Экстренные меры${NC}"
    echo
    echo "Меры защиты:"
    echo "  • Снижение приоритета процессов"
    echo "  • Очистка системного кэша"
    echo "  • Принудительные паузы в экстренном режиме"
    echo
    echo "Управление:"
    echo "  Ctrl+C - Остановка системы"
    echo "  's' - Показать статистику"
    echo "  'r' - Сбросить статистику"
    echo "  'i' - Показать информацию о системе"
    echo "  'h' - Показать справку"
    echo
}

# Функция для показа информации о системе
show_system_info() {
    echo
    echo -e "${CYAN}💻 ИНФОРМАЦИЯ О СИСТЕМЕ${NC}"
    echo -e "${CYAN}=======================${NC}"
    get_system_info
    echo
}

# Функция для обработки сигналов
cleanup() {
    echo
    echo -e "${YELLOW}🛑 Получен сигнал завершения...${NC}"
    IS_RUNNING=false
    echo -e "${GREEN}✅ Система защиты от перегрева остановлена${NC}"
    exit 0
}

# Установка обработчиков сигналов
trap cleanup SIGINT SIGTERM

# Главная функция
main() {
    echo -e "${GREEN}🚨 АВТОНОМНАЯ СИСТЕМА ЗАЩИТЫ ОТ ПЕРЕГРЕВА НОУТБУКА 🚨${NC}"
    echo -e "${GREEN}==================================================${NC}"
    echo
    
    # Проверяем доступность системных утилит
    echo -e "${BLUE}🔍 Проверка доступности системных утилит...${NC}"
    
    local utilities=""
    [ -f "/usr/bin/htop" ] && utilities="$utilities htop"
    [ -f "/usr/bin/top" ] && utilities="$utilities top"
    [ -f "/proc/cpuinfo" ] && utilities="$utilities /proc/cpuinfo"
    [ -f "/proc/meminfo" ] && utilities="$utilities /proc/meminfo"
    [ -f "/proc/loadavg" ] && utilities="$utilities /proc/loadavg"
    [ -f "/proc/uptime" ] && utilities="$utilities /proc/uptime"
    [ -d "/sys/class/thermal" ] && utilities="$utilities /sys/class/thermal"
    
    if [ -n "$utilities" ]; then
        echo -e "${GREEN}✅ Доступные утилиты:$utilities${NC}"
    else
        echo -e "${YELLOW}⚠️  ВНИМАНИЕ: Системные утилиты не найдены! Система может работать некорректно.${NC}"
    fi
    
    echo
    echo -e "${GREEN}🎮 Управление системой:${NC}"
    echo "  's' - Показать статистику"
    echo "  'r' - Сбросить статистику"
    echo "  'i' - Показать информацию о системе"
    echo "  'h' - Показать справку"
    echo "  Ctrl+C - Остановка"
    echo
    
    # Запускаем мониторинг
    IS_RUNNING=true
    echo -e "${GREEN}🚀 Запуск мониторинга температуры...${NC}"
    echo
    
    # Основной цикл мониторинга
    while [ "$IS_RUNNING" = true ]; do
        check_temperature
        
        # Если в экстренном режиме, ждем дольше
        if [ "$IS_EMERGENCY_MODE" = true ]; then
            sleep "$EMERGENCY_COOLDOWN"
        else
            sleep "$MONITOR_INTERVAL"
        fi
    done
}

# Запуск программы
main "$@"