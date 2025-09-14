#!/bin/bash

# Продвинутый анализатор кода Unity для проекта Mud-Like

echo "🔬 Продвинутый анализатор кода Unity"
echo "===================================="

# Цвета для вывода
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Функция для проверки условной компиляции
check_conditional_compilation() {
    local file="$1"
    local pattern="$2"
    
    # Проверяем, находится ли паттерн в блоке условной компиляции
    local in_conditional=false
    local found_pattern=false
    
    while IFS= read -r line; do
        # Проверяем начало условной компиляции
        if [[ "$line" =~ ^[[:space:]]*#if.*UNITY_EDITOR ]]; then
            in_conditional=true
        elif [[ "$line" =~ ^[[:space:]]*#endif ]]; then
            in_conditional=false
        elif [[ "$line" =~ $pattern ]]; then
            if [ "$in_conditional" = true ]; then
                return 1  # Найдено в условной компиляции - это нормально
            else
                found_pattern=true
            fi
        fi
    done < "$file"
    
    if [ "$found_pattern" = true ]; then
        return 0  # Найдено вне условной компиляции - это проблема
    else
        return 1  # Не найдено или найдено в условной компиляции - это нормально
    fi
}

# Функция для проверки комментариев
check_in_comments() {
    local file="$1"
    local pattern="$2"
    
    # Проверяем, находится ли паттерн в комментариях
    while IFS= read -r line; do
        # Убираем пробелы в начале
        line=$(echo "$line" | sed 's/^[[:space:]]*//')
        
        # Проверяем, является ли строка комментарием
        if [[ "$line" =~ ^//.*$pattern ]]; then
            return 1  # Найдено в комментарии - это нормально
        fi
    done < "$file"
    
    return 0  # Не найдено в комментариях
}

echo ""
echo "📊 АНАЛИЗ КРИТИЧЕСКИХ ПРОБЛЕМ"
echo "============================="

# Проверка Time API
echo "🔍 Анализ Time API..."
time_api_issues=0
while IFS= read -r file; do
    if check_conditional_compilation "$file" "Time\.(deltaTime|fixedDeltaTime|time)" && check_in_comments "$file" "Time\." && ! grep -q "SystemAPI\.Time" "$file"; then
        echo -e "  ${RED}❌ $file${NC}"
        time_api_issues=$((time_api_issues + 1))
    fi
done < <(find Assets/Scripts -name "*.cs" -type f)

if [ $time_api_issues -eq 0 ]; then
    echo -e "  ${GREEN}✅ Time API корректно используется${NC}"
else
    echo -e "  ${RED}❌ Найдено $time_api_issues проблем с Time API${NC}"
fi

# Проверка Debug.Log
echo ""
echo "🔍 Анализ Debug.Log..."
debug_log_issues=0
while IFS= read -r file; do
    if check_conditional_compilation "$file" "Debug\.Log" && check_in_comments "$file" "Debug\.Log"; then
        echo -e "  ${RED}❌ $file${NC}"
        debug_log_issues=$((debug_log_issues + 1))
    fi
done < <(find Assets/Scripts/Vehicles/Systems -name "*.cs" -type f)

if [ $debug_log_issues -eq 0 ]; then
    echo -e "  ${GREEN}✅ Debug.Log корректно используется${NC}"
else
    echo -e "  ${RED}❌ Найдено $debug_log_issues проблем с Debug.Log${NC}"
fi

echo ""
echo "📊 ИТОГОВЫЙ СТАТУС"
echo "=================="

total_issues=$((time_api_issues + debug_log_issues))

if [ $total_issues -eq 0 ]; then
    echo -e "${GREEN}🏆 ВСЕ ПРОВЕРКИ ПРОЙДЕНЫ УСПЕШНО!${NC}"
    echo -e "${GREEN}✅ Проект готов к продакшену${NC}"
    exit 0
else
    echo -e "${RED}❌ ОБНАРУЖЕНО $total_issues ПРОБЛЕМ${NC}"
    echo -e "${YELLOW}💡 Требуется исправление для достижения высокого качества${NC}"
    exit 1
fi
