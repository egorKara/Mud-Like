#!/bin/bash

# Точная проверка Time API в Unity DOTS

echo "🎯 Точная проверка Time API"
echo "==========================="

# Цвета для вывода
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

echo ""
echo "🔍 Поиск проблемных использований Time API..."

# Поиск реальных проблем
time_api_problems=0

while IFS= read -r file; do
    # Проверяем каждую строку файла
    line_number=0
    has_problems=false
    
    while IFS= read -r line; do
        line_number=$((line_number + 1))
        
        # Пропускаем комментарии
        if [[ "$line" =~ ^[[:space:]]*// ]]; then
            continue
        fi
        
        # Пропускаем директивы препроцессора
        if [[ "$line" =~ ^[[:space:]]*# ]]; then
            continue
        fi
        
        # Проверяем на проблемные использования Time API (исключая SystemAPI.Time)
        if [[ "$line" =~ Time\.(deltaTime|fixedDeltaTime|time) ]] && [[ ! "$line" =~ SystemAPI\.Time ]]; then
            # Проверяем, не находится ли это в условной компиляции
            # Ищем предыдущие строки на предмет #if
            local in_conditional=false
            local temp_line_num=1
            
            while IFS= read -r temp_line; do
                if [ $temp_line_num -eq $line_number ]; then
                    break
                fi
                
                if [[ "$temp_line" =~ ^[[:space:]]*#if.*UNITY_EDITOR ]]; then
                    in_conditional=true
                elif [[ "$temp_line" =~ ^[[:space:]]*#endif ]]; then
                    in_conditional=false
                fi
                
                temp_line_num=$((temp_line_num + 1))
            done < "$file"
            
            if [ "$in_conditional" = false ]; then
                echo -e "  ${RED}❌ $file:$line_number: $line${NC}"
                has_problems=true
                time_api_problems=$((time_api_problems + 1))
            fi
        fi
    done < "$file"
    
    if [ "$has_problems" = true ]; then
        echo ""
    fi
    
done < <(find Assets/Scripts -name "*.cs" -type f | grep -v Examples)

echo ""
echo "📊 РЕЗУЛЬТАТЫ ПРОВЕРКИ"
echo "====================="

if [ $time_api_problems -eq 0 ]; then
    echo -e "${GREEN}✅ ВСЕ ПРОВЕРКИ ПРОЙДЕНЫ!${NC}"
    echo -e "${GREEN}🎯 Time API корректно используется во всех файлах${NC}"
    exit 0
else
    echo -e "${RED}❌ ОБНАРУЖЕНО $time_api_problems ПРОБЛЕМ${NC}"
    echo -e "${YELLOW}💡 Требуется исправление устаревшего Time API${NC}"
    exit 1
fi
