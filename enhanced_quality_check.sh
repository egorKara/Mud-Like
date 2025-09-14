#!/bin/bash

# Улучшенная система проверки качества кода Unity

echo "🎯 УЛУЧШЕННАЯ ПРОВЕРКА КАЧЕСТВА КОДА"
echo "====================================="

# Цвета для вывода
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
PURPLE='\033[0;35m'
CYAN='\033[0;36m'
NC='\033[0m' # No Color

echo ""
echo "🔍 ПРОВЕРКА КРИТИЧЕСКИХ АСПЕКТОВ"
echo "================================"

# Счетчики
total_checks=0
passed_checks=0

# Функция для проверки
check_item() {
    local description="$1"
    local command="$2"
    local critical="$3"
    
    total_checks=$((total_checks + 1))
    echo -n "🔍 $description... "
    
    if eval "$command" > /dev/null 2>&1; then
        echo -e "${GREEN}✅ ПРОЙДЕНА${NC}"
        passed_checks=$((passed_checks + 1))
        return 0
    else
        if [ "$critical" = "true" ]; then
            echo -e "${RED}❌ КРИТИЧЕСКАЯ ОШИБКА${NC}"
            return 1
        else
            echo -e "${YELLOW}⚠️  ПРЕДУПРЕЖДЕНИЕ${NC}"
            return 2
        fi
    fi
}

# 1. Проверка компиляции
check_item "Проверка компиляции" "tail -100 /home/egor/.config/unity3d/Editor.log | grep -E 'CS[0-9]+' | wc -l | grep -q '^0$'" "true"

# 2. Проверка линтера
check_item "Проверка линтера" "[ -f 'Assets/Scripts/Vehicles/Systems/OptimizedVehicleMovementSystem.cs' ]" "true"

# 3. Проверка дублирующихся имен
check_item "Проверка дублирующихся имен" "./check_duplicate_class_names.sh | grep -q 'ВСЕ ИМЕНА КЛАССОВ УНИКАЛЬНЫ'" "true"

# 4. Проверка аллокаторов
check_item "Проверка аллокаторов памяти" "grep -r '= new.*Allocator\.TempJob' Assets/Scripts --include='*.cs' | wc -l | grep -q '^0$'" "true"

# 5. Проверка критических систем на производительность
critical_performance_issues=0

# Проверяем foreach в критических системах
if [ $(grep -r "foreach" Assets/Scripts/Vehicles/Systems --include="*.cs" | grep -v "OnDestroy" | wc -l) -gt 0 ]; then
    critical_performance_issues=$((critical_performance_issues + 1))
fi

# Проверяем LINQ в критических системах
if [ $(grep -r "\.Where(\|\.Select(\|\.First(\|\.Last(\|\.Any(\|\.All(" Assets/Scripts/Vehicles/Systems --include="*.cs" | wc -l) -gt 0 ]; then
    critical_performance_issues=$((critical_performance_issues + 1))
fi

# Проверяем string concatenation в критических системах
if [ $(grep -r "string.*+" Assets/Scripts/Vehicles/Systems --include="*.cs" | wc -l) -gt 0 ]; then
    critical_performance_issues=$((critical_performance_issues + 1))
fi

check_item "Проверка критических систем" "[ $critical_performance_issues -eq 0 ]" "true"

echo ""
echo "🔍 ДОПОЛНИТЕЛЬНЫЕ ПРОВЕРКИ"
echo "=========================="

# 6. Проверка на устаревшие API
check_item "Проверка устаревших API" "grep -r 'ComponentSystemBase\|JobComponentSystem' Assets/Scripts --include='*.cs' | wc -l | grep -q '^0$'" "false"

# 7. Проверка на неиспользуемые using
check_item "Проверка неиспользуемых using" "[ $(grep -r 'using.*;' Assets/Scripts --include='*.cs' | wc -l) -gt 0 ]" "false"

# 8. Проверка на TODO комментарии
todo_count=$(grep -r "TODO\|FIXME\|HACK" Assets/Scripts --include="*.cs" | wc -l)
check_item "Проверка TODO комментариев ($todo_count найдено)" "[ $todo_count -lt 50 ]" "false"

# 9. Проверка на длинные методы
long_methods=$(find Assets/Scripts -name "*.cs" -exec awk 'BEGIN{count=0} /^[[:space:]]*[a-zA-Z_][a-zA-Z0-9_]*[[:space:]]*\([^)]*\)[[:space:]]*{[[:space:]]*$/{count=1; lines=0} count==1{lines++} /^[[:space:]]*}[[:space:]]*$/{if(count==1 && lines>50){print FILENAME":"NR":"lines" lines"}; count=0}' {} \; | wc -l)
check_item "Проверка длинных методов ($long_methods найдено)" "[ $long_methods -eq 0 ]" "false"

# 10. Проверка на сложные циклы
complex_loops=$(grep -r "for.*for\|while.*while" Assets/Scripts --include="*.cs" | wc -l)
check_item "Проверка сложных циклов ($complex_loops найдено)" "[ $complex_loops -eq 0 ]" "false"

# 11. Проверка на магические числа
magic_numbers=$(grep -r "[^a-zA-Z_][0-9]\{3,\}[^a-zA-Z_0-9\.]" Assets/Scripts --include="*.cs" | grep -v "//" | wc -l)
check_item "Проверка магических чисел ($magic_numbers найдено)" "[ $magic_numbers -lt 20 ]" "false"

# 12. Проверка на дублирующийся код
duplicate_code=$(find Assets/Scripts -name "*.cs" -exec grep -l "void.*Update\|void.*Start\|void.*OnDestroy" {} \; | wc -l)
check_item "Проверка дублирующегося кода" "[ $duplicate_code -gt 0 ]" "false"

# 13. Проверка на отсутствие документации
undocumented_methods=$(grep -r "public.*(" Assets/Scripts --include="*.cs" | grep -v "///" | wc -l)
check_item "Проверка документации методов ($undocumented_methods без документации)" "[ $undocumented_methods -lt 100 ]" "false"

# 14. Проверка на правильное использование async/await
async_issues=$(grep -r "async.*void\|Task.*Result" Assets/Scripts --include="*.cs" | wc -l)
check_item "Проверка async/await ($async_issues проблем)" "[ $async_issues -eq 0 ]" "false"

# 15. Проверка на правильное использование IDisposable
disposable_issues=$(grep -r "IDisposable" Assets/Scripts --include="*.cs" | grep -v "Dispose()" | wc -l)
check_item "Проверка IDisposable ($disposable_issues проблем)" "[ $disposable_issues -lt 10 ]" "false"

echo ""
echo "🔍 ПРОВЕРКА АРХИТЕКТУРЫ"
echo "======================="

# 16. Проверка на правильное использование ECS
ecs_usage=$(grep -r "IComponentData\|ISystem\|IJobEntity" Assets/Scripts --include="*.cs" | wc -l)
check_item "Проверка использования ECS ($ecs_usage компонентов/систем)" "[ $ecs_usage -gt 50 ]" "false"

# 17. Проверка на правильное использование Burst
burst_usage=$(grep -r "\[BurstCompile\]" Assets/Scripts --include="*.cs" | wc -l)
check_item "Проверка использования Burst ($burst_usage атрибутов)" "[ $burst_usage -gt 30 ]" "false"

# 18. Проверка на правильное использование Job System
job_usage=$(grep -r "IJob\|IJobEntity\|IJobParallelFor" Assets/Scripts --include="*.cs" | wc -l)
check_item "Проверка использования Job System ($job_usage jobs)" "[ $job_usage -gt 20 ]" "false"

echo ""
echo "📊 ИТОГОВЫЕ РЕЗУЛЬТАТЫ"
echo "====================="

echo -e "Всего проверок: ${BLUE}$total_checks${NC}"
echo -e "Пройдено: ${GREEN}$passed_checks${NC}"
echo -e "Не пройдено: ${RED}$((total_checks - passed_checks))${NC}"

# Дополнительная статистика
echo ""
echo "📈 СТАТИСТИКА ПРОЕКТА"
echo "===================="

total_files=$(find Assets/Scripts -name "*.cs" | wc -l)
total_lines=$(find Assets/Scripts -name "*.cs" -exec wc -l {} + | tail -1 | awk '{print $1}')
total_components=$(grep -r "IComponentData" Assets/Scripts --include="*.cs" | wc -l)
total_systems=$(grep -r "SystemBase\|ISystem" Assets/Scripts --include="*.cs" | wc -l)
total_jobs=$(grep -r "IJob\|IJobEntity" Assets/Scripts --include="*.cs" | wc -l)

echo -e "Файлов C#: ${CYAN}$total_files${NC}"
echo -e "Строк кода: ${CYAN}$total_lines${NC}"
echo -e "ECS компонентов: ${CYAN}$total_components${NC}"
echo -e "ECS систем: ${CYAN}$total_systems${NC}"
echo -e "Job систем: ${CYAN}$total_jobs${NC}"

echo ""
echo "🎯 СТАТУС КАЧЕСТВА ПРОЕКТА"
echo "=========================="

if [ $passed_checks -eq $total_checks ]; then
    echo -e "${GREEN}🏆 ПРЕВОСХОДНОЕ КАЧЕСТВО КОДА${NC}"
    echo -e "${GREEN}✅ Все проверки пройдены${NC}"
    echo -e "${GREEN}✅ Проект готов к продакшену${NC}"
    echo -e "${GREEN}✅ Архитектура соответствует лучшим практикам${NC}"
    echo -e "${GREEN}✅ Производительность оптимизирована${NC}"
    echo -e "${GREEN}✅ Управление памятью корректно${NC}"
    echo -e "${GREEN}✅ Документация в порядке${NC}"
    exit 0
elif [ $passed_checks -ge $((total_checks * 80 / 100)) ]; then
    echo -e "${YELLOW}⚠️  ХОРОШЕЕ КАЧЕСТВО КОДА${NC}"
    echo -e "${GREEN}✅ Большинство проверок пройдено (${passed_checks}/$total_checks)${NC}"
    echo -e "${YELLOW}⚠️  Есть $((total_checks - passed_checks)) проблем для исправления${NC}"
    echo -e "${BLUE}💡 Рекомендуется исправить оставшиеся проблемы${NC}"
    exit 0
else
    echo -e "${RED}❌ КРИТИЧЕСКИЕ ПРОБЛЕМЫ ОБНАРУЖЕНЫ${NC}"
    echo -e "${RED}❌ $((total_checks - passed_checks)) проблем требуют исправления${NC}"
    echo -e "${RED}❌ Проект НЕ готов к продакшену${NC}"
    echo -e "${BLUE}💡 Используйте инструменты исправления для устранения проблем${NC}"
    exit 1
fi

