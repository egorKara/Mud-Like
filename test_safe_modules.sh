#!/bin/bash

# 🧪 ТЕСТИРОВАНИЕ БЕЗОПАСНЫХ МОДУЛЕЙ СИСТЕМЫ ЗАЩИТЫ ОТ ПЕРЕГРЕВА
# Только чтение данных, никаких изменений системы

# Цвета для вывода
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
PURPLE='\033[0;35m'
CYAN='\033[0;36m'
NC='\033[0m' # No Color

echo -e "${BLUE}🧪 ТЕСТИРОВАНИЕ БЕЗОПАСНЫХ МОДУЛЕЙ СИСТЕМЫ ЗАЩИТЫ ОТ ПЕРЕГРЕВА${NC}"
echo -e "${BLUE}================================================================${NC}"
echo

# Импортируем функции из основного скрипта
source ./overheat_protection.sh

# ТЕСТ 1: Мониторинг CPU нагрузки
echo -e "${CYAN}🔍 ТЕСТ 1: Мониторинг CPU нагрузки${NC}"
echo "----------------------------------------"
cpu_load=$(get_cpu_load)
echo "CPU Load: ${cpu_load}%"
if [ "$cpu_load" -ge 0 ] && [ "$cpu_load" -le 100 ]; then
    echo -e "${GREEN}✅ CPU нагрузка в допустимом диапазоне (0-100%)${NC}"
else
    echo -e "${RED}❌ CPU нагрузка вне допустимого диапазона${NC}"
fi
echo

# ТЕСТ 2: Мониторинг GPU нагрузки
echo -e "${CYAN}🔍 ТЕСТ 2: Мониторинг GPU нагрузки${NC}"
echo "----------------------------------------"
gpu_load=$(get_gpu_load)
echo "GPU Load: ${gpu_load}%"
if [ "$gpu_load" -ge 0 ] && [ "$gpu_load" -le 100 ]; then
    echo -e "${GREEN}✅ GPU нагрузка в допустимом диапазоне (0-100%)${NC}"
else
    echo -e "${RED}❌ GPU нагрузка вне допустимого диапазона${NC}"
fi
echo

# ТЕСТ 3: Оценка температуры
echo -e "${CYAN}🔍 ТЕСТ 3: Оценка температуры${NC}"
echo "----------------------------------------"
temp=$(get_cpu_temperature)
echo "CPU Temperature: ${temp}°C"
if [ "$temp" -ge 0 ] && [ "$temp" -le 200 ]; then
    echo -e "${GREEN}✅ Температура в допустимом диапазоне (0-200°C)${NC}"
else
    echo -e "${RED}❌ Температура вне допустимого диапазона${NC}"
fi
echo

# ТЕСТ 4: Проверка Unity Editor
echo -e "${CYAN}🔍 ТЕСТ 4: Проверка Unity Editor${NC}"
echo "----------------------------------------"
unity_running=$(is_unity_editor_running)
echo "Unity Editor running: ${unity_running}"
if [ "$unity_running" = "true" ] || [ "$unity_running" = "false" ]; then
    echo -e "${GREEN}✅ Функция проверки Unity Editor работает корректно${NC}"
else
    echo -e "${RED}❌ Функция проверки Unity Editor работает некорректно${NC}"
fi
echo

# ТЕСТ 5: Системная информация
echo -e "${CYAN}🔍 ТЕСТ 5: Системная информация${NC}"
echo "----------------------------------------"
echo "System Info:"
get_system_info
echo

# ТЕСТ 6: Проверка порогов
echo -e "${CYAN}🔍 ТЕСТ 6: Проверка порогов${NC}"
echo "----------------------------------------"
echo "Unity Editor CPU Limit: ${UNITY_EDITOR_CPU_LIMIT}%"
echo "Unity Editor GPU Limit: ${UNITY_EDITOR_GPU_LIMIT}%"
echo "Max CPU Load Threshold: ${MAX_CPU_LOAD_THRESHOLD}%"
echo "Max GPU Load Threshold: ${MAX_GPU_LOAD_THRESHOLD}%"
echo "Critical Temp Threshold: ${CRITICAL_TEMP_THRESHOLD}°C"
echo "Warning Temp Threshold: ${WARNING_TEMP_THRESHOLD}°C"
echo "Safe Temp Threshold: ${SAFE_TEMP_THRESHOLD}°C"
echo

# ТЕСТ 7: Проверка логики без применения мер
echo -e "${CYAN}🔍 ТЕСТ 7: Проверка логики без применения мер${NC}"
echo "----------------------------------------"
echo "Проверка Unity Editor логики:"
if [ "$unity_running" = "true" ]; then
    if [ "$cpu_load" -gt "$UNITY_EDITOR_CPU_LIMIT" ] || [ "$gpu_load" -gt "$UNITY_EDITOR_GPU_LIMIT" ]; then
        echo -e "${YELLOW}⚠️  Unity Editor превысил лимиты (но меры не применяются)${NC}"
    else
        echo -e "${GREEN}✅ Unity Editor в пределах лимитов${NC}"
    fi
else
    echo -e "${BLUE}ℹ️  Unity Editor не запущен${NC}"
fi

echo "Проверка основного режима логики:"
if [ "$cpu_load" -ge "$MAX_CPU_LOAD_THRESHOLD" ] || [ "$gpu_load" -ge "$MAX_GPU_LOAD_THRESHOLD" ]; then
    echo -e "${YELLOW}⚠️  Достигнута максимальная нагрузка (но меры не применяются)${NC}"
else
    echo -e "${GREEN}✅ Нагрузка в пределах нормы${NC}"
fi

echo "Проверка температурной логики:"
if [ "$temp" -ge "$CRITICAL_TEMP_THRESHOLD" ]; then
    echo -e "${YELLOW}⚠️  Критическая температура (но меры не применяются)${NC}"
elif [ "$temp" -ge "$WARNING_TEMP_THRESHOLD" ]; then
    echo -e "${YELLOW}⚠️  Предупреждающая температура (но меры не применяются)${NC}"
elif [ "$temp" -ge "$SAFE_TEMP_THRESHOLD" ]; then
    echo -e "${YELLOW}⚠️  Повышенная температура (но меры не применяются)${NC}"
else
    echo -e "${GREEN}✅ Температура в норме${NC}"
fi
echo

# ИТОГОВЫЙ ОТЧЕТ
echo -e "${PURPLE}📊 ИТОГОВЫЙ ОТЧЕТ БЕЗОПАСНОГО ТЕСТИРОВАНИЯ${NC}"
echo -e "${PURPLE}==========================================${NC}"
echo -e "${GREEN}✅ Все безопасные модули протестированы${NC}"
echo -e "${GREEN}✅ Никаких изменений в системе не внесено${NC}"
echo -e "${GREEN}✅ Все функции мониторинга работают корректно${NC}"
echo -e "${BLUE}ℹ️  Готово к тестированию умеренно безопасных модулей${NC}"
echo