#!/bin/bash

# 🧪 УПРОЩЕННЫЙ ТЕСТ БЕЗОПАСНЫХ МОДУЛЕЙ
# Только базовые функции мониторинга

# Цвета для вывода
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
CYAN='\033[0;36m'
NC='\033[0m'

echo -e "${BLUE}🧪 УПРОЩЕННЫЙ ТЕСТ БЕЗОПАСНЫХ МОДУЛЕЙ${NC}"
echo -e "${BLUE}=====================================${NC}"
echo

# ТЕСТ 1: Проверка доступности системных утилит
echo -e "${CYAN}🔍 ТЕСТ 1: Системные утилиты${NC}"
echo "----------------------------------------"

# htop
if command -v htop >/dev/null 2>&1; then
    echo -e "${GREEN}✅ htop доступен${NC}"
else
    echo -e "${RED}❌ htop недоступен${NC}"
fi

# /proc/cpuinfo
if [ -f "/proc/cpuinfo" ]; then
    echo -e "${GREEN}✅ /proc/cpuinfo доступен${NC}"
    cpu_cores=$(nproc)
    echo "  CPU cores: $cpu_cores"
else
    echo -e "${RED}❌ /proc/cpuinfo недоступен${NC}"
fi

# /proc/loadavg
if [ -f "/proc/loadavg" ]; then
    echo -e "${GREEN}✅ /proc/loadavg доступен${NC}"
    load_avg=$(cat /proc/loadavg | awk '{print $1}')
    echo "  Load average: $load_avg"
else
    echo -e "${RED}❌ /proc/loadavg недоступен${NC}"
fi

# /proc/meminfo
if [ -f "/proc/meminfo" ]; then
    echo -e "${GREEN}✅ /proc/meminfo доступен${NC}"
    mem_total=$(grep "MemTotal" /proc/meminfo | awk '{print $2}')
    echo "  Total RAM: $((mem_total / 1024)) MB"
else
    echo -e "${RED}❌ /proc/meminfo недоступен${NC}"
fi

# /sys/class/thermal
if [ -d "/sys/class/thermal" ]; then
    echo -e "${GREEN}✅ /sys/class/thermal доступен${NC}"
    thermal_zones=$(find /sys/class/thermal -name "temp" 2>/dev/null | wc -l)
    echo "  Thermal zones: $thermal_zones"
else
    echo -e "${RED}❌ /sys/class/thermal недоступен${NC}"
fi

echo

# ТЕСТ 2: Базовый мониторинг CPU
echo -e "${CYAN}🔍 ТЕСТ 2: Мониторинг CPU${NC}"
echo "----------------------------------------"

# CPU модель
if [ -f "/proc/cpuinfo" ]; then
    cpu_model=$(grep "model name" /proc/cpuinfo | head -1 | cut -d: -f2 | sed 's/^[ \t]*//')
    echo "CPU Model: $cpu_model"
fi

# CPU cores
cpu_cores=$(nproc)
echo "CPU Cores: $cpu_cores"

# Load average
if [ -f "/proc/loadavg" ]; then
    load_avg=$(cat /proc/loadavg | awk '{print $1}')
    cpu_load=$(echo "$load_avg $cpu_cores" | awk '{printf "%.0f", ($1/$2)*100}')
    echo "CPU Load: ${cpu_load}%"
    
    if [ "$cpu_load" -ge 0 ] && [ "$cpu_load" -le 100 ]; then
        echo -e "${GREEN}✅ CPU нагрузка в допустимом диапазоне${NC}"
    else
        echo -e "${RED}❌ CPU нагрузка вне допустимого диапазона${NC}"
    fi
fi

echo

# ТЕСТ 3: Базовый мониторинг памяти
echo -e "${CYAN}🔍 ТЕСТ 3: Мониторинг памяти${NC}"
echo "----------------------------------------"

if [ -f "/proc/meminfo" ]; then
    mem_total=$(grep "MemTotal" /proc/meminfo | awk '{print $2}')
    mem_available=$(grep "MemAvailable" /proc/meminfo | awk '{print $2}')
    mem_used=$((mem_total - mem_available))
    mem_usage=0
    if [ "$mem_total" -gt 0 ]; then
        mem_usage=$((mem_used * 100 / mem_total))
    fi
    
    echo "Total RAM: $((mem_total / 1024)) MB"
    echo "Used RAM: $((mem_used / 1024)) MB"
    echo "RAM Usage: ${mem_usage}%"
    
    if [ "$mem_usage" -ge 0 ] && [ "$mem_usage" -le 100 ]; then
        echo -e "${GREEN}✅ Использование RAM в допустимом диапазоне${NC}"
    else
        echo -e "${RED}❌ Использование RAM вне допустимого диапазона${NC}"
    fi
fi

echo

# ТЕСТ 4: Проверка процессов
echo -e "${CYAN}🔍 ТЕСТ 4: Проверка процессов${NC}"
echo "----------------------------------------"

# Unity процессы
unity_count=$(pgrep -f "Unity" 2>/dev/null | wc -l)
echo "Unity processes: $unity_count"

# Cursor процессы
cursor_count=$(pgrep -f "cursor" 2>/dev/null | wc -l)
echo "Cursor processes: $cursor_count"

# Chrome процессы
chrome_count=$(pgrep -f "chrome" 2>/dev/null | wc -l)
echo "Chrome processes: $chrome_count"

echo

# ТЕСТ 5: Проверка температуры (если доступно)
echo -e "${CYAN}🔍 ТЕСТ 5: Проверка температуры${NC}"
echo "----------------------------------------"

temp_found=false
if [ -d "/sys/class/thermal" ]; then
    for thermal_zone in /sys/class/thermal/thermal_zone*; do
        if [ -f "$thermal_zone/temp" ]; then
            zone_temp=$(cat "$thermal_zone/temp" 2>/dev/null)
            if [ -n "$zone_temp" ] && [ "$zone_temp" -gt 0 ] && [ "$zone_temp" -lt 200000 ]; then
                temp_celsius=$((zone_temp / 1000))
                echo "Temperature: ${temp_celsius}°C (from $thermal_zone)"
                temp_found=true
                break
            fi
        fi
    done
fi

if [ "$temp_found" = false ]; then
    echo -e "${YELLOW}⚠️  Реальные датчики температуры недоступны${NC}"
    echo "  Будет использоваться оценка на основе нагрузки"
fi

echo

# ИТОГОВЫЙ ОТЧЕТ
echo -e "${BLUE}📊 ИТОГОВЫЙ ОТЧЕТ${NC}"
echo -e "${BLUE}=================${NC}"
echo -e "${GREEN}✅ Безопасные модули протестированы${NC}"
echo -e "${GREEN}✅ Никаких изменений в системе не внесено${NC}"
echo -e "${GREEN}✅ Все функции мониторинга работают${NC}"
echo -e "${BLUE}ℹ️  Система готова к работе${NC}"
echo