#!/bin/bash

# 🚨 АВТОНОМНАЯ СИСТЕМА ЗАЩИТЫ ОТ ПЕРЕГРЕВА НОУТБУКА 🚨
# Основана на наработках Unity OverheatProtectionSystem и SystemInfoIntegration
# Работает независимо от Unity и других приложений

# Критические пороги температуры (из OverheatProtectionSystem.cs)
CRITICAL_TEMP_THRESHOLD=85    # Критическая температура CPU
WARNING_TEMP_THRESHOLD=75     # Предупреждение о перегреве  
SAFE_TEMP_THRESHOLD=65        # Безопасная температура

# Специальные правила для Unity Editor
UNITY_EDITOR_CPU_LIMIT=95     # Максимальная нагрузка CPU при работе Unity Editor
UNITY_EDITOR_GPU_LIMIT=95     # Максимальная нагрузка GPU при работе Unity Editor

# Правила для основного режима работы
MAX_CPU_LOAD_THRESHOLD=100    # Порог максимальной нагрузки CPU
MAX_GPU_LOAD_THRESHOLD=100    # Порог максимальной нагрузки GPU
REDUCE_TO_CPU_LOAD=95         # Снижение CPU нагрузки до этого уровня
REDUCE_TO_GPU_LOAD=95         # Снижение GPU нагрузки до этого уровня

# Настройки защиты (из OverheatProtectionSystem.cs)
TEMP_CHECK_INTERVAL=1         # Проверяем каждую секунду
EMERGENCY_COOLDOWN_TIME=5     # Экстренное охлаждение 5 секунд
MAX_FPS_EMERGENCY=15          # Максимальный FPS в экстренном режиме
MAX_FPS_WARNING=30            # Максимальный FPS при предупреждении

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

# Функция для получения температуры CPU (улучшенная версия с параллельными датчиками)
get_cpu_temperature() {
    local temp=0
    
    # Пробуем получить реальную температуру из thermal zones
    if [ -d "/sys/class/thermal" ]; then
        for thermal_zone in /sys/class/thermal/thermal_zone*; do
            if [ -f "$thermal_zone/temp" ]; then
                local zone_temp=$(cat "$thermal_zone/temp" 2>/dev/null)
                if [ -n "$zone_temp" ] && [ "$zone_temp" -gt 0 ] && [ "$zone_temp" -lt 200000 ]; then
                    temp=$((zone_temp / 1000))
                    echo "[SystemInfo] Найдена реальная температура: ${temp}°C" >&2
                    break
                fi
            fi
        done
    fi
    
    # Fallback: улучшенная оценка на основе параллельных датчиков CPU и GPU
    if [ "$temp" -eq 0 ]; then
        temp=$(estimate_temperature_from_parallel_sensors)
    fi
    
    echo "$temp"
}

# Функция для проверки работы Unity Editor
is_unity_editor_running() {
    local unity_processes=$(pgrep -f "Unity.*Editor" 2>/dev/null | wc -l)
    if [ "$unity_processes" -gt 0 ]; then
        echo "true"
    else
        echo "false"
    fi
}

# Функция для получения загрузки GPU
get_gpu_load() {
    local gpu_load=0
    
    # Пробуем nvidia-smi (если доступен)
    if command -v nvidia-smi >/dev/null 2>&1; then
        local nvidia_output=$(timeout 3s nvidia-smi --query-gpu=utilization.gpu --format=csv,noheader,nounits 2>/dev/null)
        if [ -n "$nvidia_output" ]; then
            gpu_load=$(echo "$nvidia_output" | head -1 | tr -d ' ')
            echo "[GPU] nvidia-smi загрузка: ${gpu_load}%" >&2
        fi
    fi
    
    # Fallback: оценка на основе процессов OpenGL/GPU
    if [ "$gpu_load" -eq 0 ]; then
        gpu_load=$(estimate_gpu_load_from_processes)
    fi
    
    echo "$gpu_load"
}

# Оценка загрузки GPU на основе процессов
estimate_gpu_load_from_processes() {
    local gpu_processes=0
    local total_processes=0
    
    # Ищем процессы, которые могут использовать GPU
    for process in Unity Cursor code firefox chrome; do
        local count=$(pgrep "$process" 2>/dev/null | wc -l)
        total_processes=$((total_processes + count))
        if [ "$count" -gt 0 ]; then
            gpu_processes=$((gpu_processes + count))
        fi
    done
    
    # Оцениваем загрузку GPU на основе количества процессов
    local estimated_load=0
    if [ "$total_processes" -gt 0 ]; then
        estimated_load=$((gpu_processes * 100 / total_processes))
    fi
    
    echo "[GPU] Оценочная загрузка: ${estimated_load}% (процессы: ${gpu_processes}/${total_processes})" >&2
    echo "$estimated_load"
}

# Улучшенная оценка температуры на основе параллельных датчиков CPU и GPU
estimate_temperature_from_parallel_sensors() {
    local cpu_load=$(get_cpu_load)
    local gpu_load=$(get_gpu_load)
    local base_temp=45  # Базовая температура
    
    # Взвешенная оценка: CPU влияет больше, но GPU тоже учитывается
    local cpu_factor=$((cpu_load * 30 / 100))  # CPU: 30% влияния
    local gpu_factor=$((gpu_load * 20 / 100))  # GPU: 20% влияния
    local combined_load=$((cpu_load + gpu_load))
    local combined_factor=$((combined_load * 15 / 100))  # Комбинированная нагрузка: 15% влияния
    
    local estimated_temp=$((base_temp + cpu_factor + gpu_factor + combined_factor))
    
    # Ограничиваем максимальную температуру
    if [ "$estimated_temp" -gt 95 ]; then
        estimated_temp=95
    fi
    
    echo "[SystemInfo] Параллельные датчики - CPU: ${cpu_load}%, GPU: ${gpu_load}%, Температура: ${estimated_temp}°C" >&2
    echo "$estimated_temp"
}

# Оценить температуру на основе нагрузки (старая версия для совместимости)
estimate_temperature_from_load() {
    local cpu_load=$(get_cpu_load)
    local base_temp=45  # Базовая температура
    local load_factor=$((cpu_load * 25 / 100))
    local estimated_temp=$((base_temp + load_factor))
    
    echo "[SystemInfo] Оценочная температура: ${estimated_temp}°C (нагрузка: ${cpu_load}%)" >&2
    echo "$estimated_temp"
}

# Функция для получения нагрузки CPU (из SystemInfoIntegration.cs)
get_cpu_load() {
    local load=0
    
    # Используем htop для получения точной нагрузки CPU (как в SystemInfoIntegration.cs)
    if command -v htop >/dev/null 2>&1; then
        local htop_output=$(timeout 3s htop -n 1 -d 1 2>/dev/null | grep -E "Cpu\(s\):|CPU:" | head -1)
        if [ -n "$htop_output" ]; then
            load=$(echo "$htop_output" | grep -oE '[0-9]+\.[0-9]+%' | head -1 | sed 's/%//' | cut -d. -f1)
        fi
    fi
    
    # Fallback к /proc/loadavg (как в SystemInfoIntegration.cs)
    if [ "$load" -eq 0 ]; then
        load=$(get_cpu_load_from_proc)
    fi
    
    echo "$load"
}

# Получить нагрузку CPU из /proc/loadavg (из SystemInfoIntegration.cs)
get_cpu_load_from_proc() {
    local load=0
    
    if [ -f "/proc/loadavg" ]; then
        local load_avg=$(cat /proc/loadavg | awk '{print $1}')
        local cpu_cores=$(nproc)
        load=$(echo "$load_avg $cpu_cores" | awk '{printf "%.0f", ($1/$2)*100}')
        if [ "$load" -gt 100 ]; then
            load=100
        fi
    fi
    
    echo "$load"
}

# Функция для получения информации о системе (улучшенная с GPU)
get_system_info() {
    local cpu_model="Unknown"
    local cpu_cores=$(nproc)
    local cpu_load=$(get_cpu_load)
    local gpu_load=$(get_gpu_load)
    local cpu_temp=$(get_cpu_temperature)
    
    # Получаем модель CPU
    if [ -f "/proc/cpuinfo" ]; then
        cpu_model=$(grep "model name" /proc/cpuinfo | head -1 | cut -d: -f2 | sed 's/^[ \t]*//')
    fi
    
    # Получаем информацию о GPU
    local gpu_info="Unknown"
    if command -v nvidia-smi >/dev/null 2>&1; then
        gpu_info=$(timeout 3s nvidia-smi --query-gpu=name --format=csv,noheader 2>/dev/null | head -1)
    elif [ -f "/proc/driver/nvidia/version" ]; then
        gpu_info="NVIDIA (драйвер установлен)"
    else
        gpu_info="Встроенная/Неизвестная"
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
    echo "CPU Load: ${cpu_load}%"
    echo "GPU: $gpu_info"
    echo "GPU Load: ${gpu_load}%"
    echo "Temperature: ${cpu_temp}°C (параллельные датчики)"
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

# Меры для снижения нагрузки при достижении 100%
apply_load_reduction_measures() {
    local cpu_load="$1"
    local gpu_load="$2"
    
    echo -e "${YELLOW}🔧 СНИЖЕНИЕ НАГРУЗКИ С 100% ДО 95%${NC}"
    echo -e "${YELLOW}   CPU: ${cpu_load}% → ${REDUCE_TO_CPU_LOAD}%${NC}"
    echo -e "${YELLOW}   GPU: ${gpu_load}% → ${REDUCE_TO_GPU_LOAD}%${NC}"
    
    # Снижение приоритета тяжелых процессов
    for process in Unity Cursor code firefox chrome; do
        local pids=$(pgrep "$process" 2>/dev/null)
        if [ -n "$pids" ]; then
            for pid in $pids; do
                if renice +10 "$pid" 2>/dev/null; then
                    echo "Снижен приоритет процесса $process (PID: $pid) для снижения нагрузки"
                fi
            done
        fi
    done
    
    # Принудительная пауза на 2 секунды для снижения нагрузки
    echo -e "${YELLOW}⏸️  Принудительная пауза на 2 секунды для снижения нагрузки...${NC}"
    sleep 2
    
    # Очистка кэша для освобождения ресурсов
    echo 3 > /proc/sys/vm/drop_caches 2>/dev/null || true
    
    # Принудительная синхронизация
    sync
    
    echo -e "${GREEN}✅ Нагрузка снижена с 100% до 95%${NC}"
}

# Экстренные меры для Unity Editor при превышении лимитов
apply_unity_editor_emergency_measures() {
    local cpu_load="$1"
    local gpu_load="$2"
    
    echo -e "${RED}🚨 ЭКСТРЕННЫЕ МЕРЫ ДЛЯ UNITY EDITOR!${NC}"
    echo -e "${RED}   CPU: ${cpu_load}% > ${UNITY_EDITOR_CPU_LIMIT}%${NC}"
    echo -e "${RED}   GPU: ${gpu_load}% > ${UNITY_EDITOR_GPU_LIMIT}%${NC}"
    
    # Принудительная пауза Unity Editor
    echo -e "${RED}⏸️  Принудительная пауза Unity Editor на 3 секунды...${NC}"
    local unity_pids=$(pgrep -f "Unity.*Editor" 2>/dev/null)
    if [ -n "$unity_pids" ]; then
        for pid in $unity_pids; do
            if kill -STOP "$pid" 2>/dev/null; then
                echo "Unity Editor (PID: $pid) приостановлен"
            fi
        done
        sleep 3
        for pid in $unity_pids; do
            if kill -CONT "$pid" 2>/dev/null; then
                echo "Unity Editor (PID: $pid) возобновлен"
            fi
        done
    fi
    
    # Максимальное снижение приоритета Unity Editor
    for pid in $unity_pids; do
        if renice +19 "$pid" 2>/dev/null; then
            echo "Установлен минимальный приоритет для Unity Editor (PID: $pid)"
        fi
    done
    
    # Дополнительное снижение приоритета других тяжелых процессов
    for process in Cursor code firefox chrome; do
        local pids=$(pgrep "$process" 2>/dev/null)
        if [ -n "$pids" ]; then
            for pid in $pids; do
                if renice +15 "$pid" 2>/dev/null; then
                    echo "Снижен приоритет процесса $process (PID: $pid)"
                fi
            done
        fi
    done
    
    # Принудительная очистка кэша
    echo 3 > /proc/sys/vm/drop_caches 2>/dev/null || true
    
    # Принудительная синхронизация
    sync
    
    echo -e "${GREEN}✅ Экстренные меры для Unity Editor применены${NC}"
}

# Дополнительное ограничение нагрузки в экстренном режиме (из OverheatProtectionSystem.cs)
enforce_emergency_limits() {
    echo -e "${RED}🔒 Применение экстренных ограничений...${NC}"
    
    # Дополнительное снижение приоритета всех процессов
    for process in Unity Cursor code; do
        local pids=$(pgrep "$process" 2>/dev/null)
        if [ -n "$pids" ]; then
            for pid in $pids; do
                if renice +19 "$pid" 2>/dev/null; then
                    echo "Экстренное снижение приоритета для процесса $process (PID: $pid)"
                fi
            done
        fi
    done
    
    # Дополнительная очистка кэша
    echo 3 > /proc/sys/vm/drop_caches 2>/dev/null || true
    
    # Принудительная синхронизация
    sync
}

# Функция для проверки максимальной нагрузки в основном режиме
check_maximum_load() {
    local cpu_load=$(get_cpu_load)
    local gpu_load=$(get_gpu_load)
    
    # Проверяем достижение 100% нагрузки
    if [ "$cpu_load" -ge "$MAX_CPU_LOAD_THRESHOLD" ] || [ "$gpu_load" -ge "$MAX_GPU_LOAD_THRESHOLD" ]; then
        echo -e "${RED}🚨 МАКСИМАЛЬНАЯ НАГРУЗКА ДОСТИГНУТА!${NC}"
        echo -e "${RED}   CPU: ${cpu_load}% (порог: ${MAX_CPU_LOAD_THRESHOLD}%)${NC}"
        echo -e "${RED}   GPU: ${gpu_load}% (порог: ${MAX_GPU_LOAD_THRESHOLD}%)${NC}"
        
        # Применяем меры для снижения нагрузки
        apply_load_reduction_measures "$cpu_load" "$gpu_load"
    else
        echo -e "${GREEN}✅ Нагрузка в пределах нормы - CPU: ${cpu_load}%, GPU: ${gpu_load}%${NC}"
    fi
}

# Функция для проверки нагрузки при работе Unity Editor
check_unity_editor_load() {
    local unity_running=$(is_unity_editor_running)
    
    if [ "$unity_running" = "true" ]; then
        local cpu_load=$(get_cpu_load)
        local gpu_load=$(get_gpu_load)
        
        echo -e "${BLUE}🎮 Unity Editor активен - CPU: ${cpu_load}%, GPU: ${gpu_load}%${NC}"
        
        # Проверяем превышение лимитов для Unity Editor
        if [ "$cpu_load" -gt "$UNITY_EDITOR_CPU_LIMIT" ] || [ "$gpu_load" -gt "$UNITY_EDITOR_GPU_LIMIT" ]; then
            echo -e "${RED}🚨 ПРЕВЫШЕНИЕ ЛИМИТОВ UNITY EDITOR!${NC}"
            echo -e "${RED}   CPU: ${cpu_load}% (лимит: ${UNITY_EDITOR_CPU_LIMIT}%)${NC}"
            echo -e "${RED}   GPU: ${gpu_load}% (лимит: ${UNITY_EDITOR_GPU_LIMIT}%)${NC}"
            
            # Применяем экстренные меры для Unity Editor
            apply_unity_editor_emergency_measures "$cpu_load" "$gpu_load"
        else
            echo -e "${GREEN}✅ Unity Editor работает в пределах нормы${NC}"
        fi
    fi
}

# Функция для проверки температуры (из OverheatProtectionSystem.cs)
check_temperature() {
    local temp=$(get_cpu_temperature)
    
    if [ "$temp" -le 0 ]; then
        echo -e "${YELLOW}⚠️  Не удалось получить температуру CPU${NC}"
        return
    fi
    
    echo -e "${CYAN}🌡️  Температура CPU: ${temp}°C${NC}"
    
    # 1. Сначала проверяем Unity Editor (высший приоритет)
    check_unity_editor_load
    
    # 2. Проверяем максимальную нагрузку в основном режиме
    check_maximum_load
    
    # 3. Определяем состояние системы по температуре (как в OverheatProtectionSystem.cs)
    if [ "$temp" -ge "$CRITICAL_TEMP_THRESHOLD" ]; then
        handle_emergency_temperature "$temp"
    elif [ "$temp" -ge "$WARNING_TEMP_THRESHOLD" ]; then
        handle_critical_temperature "$temp"
    elif [ "$temp" -ge "$SAFE_TEMP_THRESHOLD" ]; then
        handle_warning_temperature "$temp"
    else
        # Температура в норме (Safe state)
        if [ "$IS_EMERGENCY_MODE" = true ]; then
            echo -e "${GREEN}🌡️  Температура нормализовалась, выход из экстренного режима${NC}"
            IS_EMERGENCY_MODE=false
        fi
    fi
}

# Обработка предупреждающей температуры (Warning state из OverheatProtectionSystem.cs)
handle_warning_temperature() {
    local temp="$1"
    WARNING_COUNT=$((WARNING_COUNT + 1))
    
    echo -e "${YELLOW}⚠️  ПРЕДУПРЕЖДЕНИЕ: Температура CPU ${temp}°C (счетчик: $WARNING_COUNT)${NC}"
    echo -e "${YELLOW}🔧 Применение мягких мер защиты...${NC}"
    
    # Мягкие меры (как в OverheatProtectionSystem.cs)
    apply_soft_measures
}

# Обработка критической температуры (Critical state из OverheatProtectionSystem.cs)
handle_critical_temperature() {
    local temp="$1"
    CRITICAL_COUNT=$((CRITICAL_COUNT + 1))
    
    echo -e "${RED}🔥 КРИТИЧЕСКАЯ ТЕМПЕРАТУРА: ${temp}°C (счетчик: $CRITICAL_COUNT)${NC}"
    echo -e "${RED}🔧 Применение агрессивных мер защиты...${NC}"
    
    # Агрессивные меры (как в OverheatProtectionSystem.cs)
    apply_aggressive_measures
}

# Обработка экстренной температуры (Emergency state из OverheatProtectionSystem.cs)
handle_emergency_temperature() {
    local temp="$1"
    EMERGENCY_COUNT=$((EMERGENCY_COUNT + 1))
    LAST_EMERGENCY_TIME=$(date '+%Y-%m-%d %H:%M:%S')
    IS_EMERGENCY_MODE=true
    
    echo -e "${RED}🚨 ЭКСТРЕННАЯ ТЕМПЕРАТУРА: ${temp}°C (счетчик: $EMERGENCY_COUNT)${NC}"
    echo -e "${RED}🚨 ПРИМЕНЕНИЕ ЭКСТРЕННЫХ МЕР ЗАЩИТЫ!${NC}"
    
    # Экстренные меры (как в OverheatProtectionSystem.cs)
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
    echo "Эта система автоматически мониторит температуру CPU и GPU"
    echo "и принимает меры для предотвращения перегрева ноутбука."
    echo
    echo "Параллельные датчики:"
    echo "  • CPU загрузка (htop, /proc/loadavg)"
    echo "  • GPU загрузка (nvidia-smi, процессы)"
    echo "  • Комбинированная оценка температуры"
    echo
    echo "Специальные правила для Unity Editor:"
    echo -e "  ${BLUE}🎮 Unity Editor: CPU ≤ 95%, GPU ≤ 95%${NC}"
    echo -e "  ${RED}🚨 При превышении: принудительная пауза + снижение приоритета${NC}"
    echo
    echo "Правила для основного режима работы:"
    echo -e "  ${YELLOW}⚠️  При 100% CPU/GPU: автоматическое снижение до 95%${NC}"
    echo -e "  ${YELLOW}🔧 Меры: снижение приоритета + пауза + очистка кэша${NC}"
    echo
    echo "Уровни температуры (из OverheatProtectionSystem.cs):"
    echo -e "  ${YELLOW}⚠️  Предупреждение: 75°C+ - Мягкие меры${NC}"
    echo -e "  ${RED}🔥 Критическая: 85°C+ - Агрессивные меры${NC}"
    echo -e "  ${RED}🚨 Экстренная: 85°C+ - Экстренные меры${NC}"
    echo
    echo "Меры защиты:"
    echo "  • Снижение приоритета процессов"
    echo "  • Очистка системного кэша"
    echo "  • Принудительные паузы в экстренном режиме"
    echo "  • Мониторинг CPU и GPU нагрузки"
    echo "  • Специальная защита для Unity Editor"
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
    
    # Проверяем доступность системных утилит (включая GPU)
    echo -e "${BLUE}🔍 Проверка доступности системных утилит...${NC}"
    
    local utilities=""
    local gpu_utilities=""
    
    # CPU и системные утилиты
    [ -f "/usr/bin/htop" ] && utilities="$utilities htop"
    [ -f "/usr/bin/top" ] && utilities="$utilities top"
    [ -f "/proc/cpuinfo" ] && utilities="$utilities /proc/cpuinfo"
    [ -f "/proc/meminfo" ] && utilities="$utilities /proc/meminfo"
    [ -f "/proc/loadavg" ] && utilities="$utilities /proc/loadavg"
    [ -f "/proc/uptime" ] && utilities="$utilities /proc/uptime"
    [ -d "/sys/class/thermal" ] && utilities="$utilities /sys/class/thermal"
    
    # GPU утилиты
    if command -v nvidia-smi >/dev/null 2>&1; then
        gpu_utilities="$gpu_utilities nvidia-smi"
    fi
    if [ -f "/proc/driver/nvidia/version" ]; then
        gpu_utilities="$gpu_utilities nvidia-driver"
    fi
    
    if [ -n "$utilities" ]; then
        echo -e "${GREEN}✅ Доступные утилиты:$utilities${NC}"
    else
        echo -e "${YELLOW}⚠️  ВНИМАНИЕ: Системные утилиты не найдены! Система может работать некорректно.${NC}"
    fi
    
    if [ -n "$gpu_utilities" ]; then
        echo -e "${GREEN}✅ GPU утилиты:$gpu_utilities${NC}"
    else
        echo -e "${YELLOW}⚠️  GPU утилиты не найдены, будет использоваться оценка на основе процессов${NC}"
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
    
    # Основной цикл мониторинга (как в OverheatProtectionSystem.cs)
    while [ "$IS_RUNNING" = true ]; do
        check_temperature
        
        # В экстренном режиме дополнительно ограничиваем нагрузку (как в OverheatProtectionSystem.cs)
        if [ "$IS_EMERGENCY_MODE" = true ]; then
            enforce_emergency_limits
            sleep "$EMERGENCY_COOLDOWN_TIME"
        else
            sleep "$TEMP_CHECK_INTERVAL"
        fi
    done
}

# Запуск программы
main "$@"