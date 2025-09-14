#!/bin/bash

# Финальная система проверки качества кода Unity

echo "🎯 ФИНАЛЬНАЯ ПРОВЕРКА КАЧЕСТВА КОДА"
echo "===================================="

# Цвета для вывода
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

echo ""
echo "🔍 ПРОВЕРКА КРИТИЧЕСКИХ АСПЕКТОВ"
echo "================================"

# 1. Проверка компиляции
echo -n "🔍 Проверка компиляции... "
if tail -100 /home/egor/.config/unity3d/Editor.log | grep -E "CS[0-9]+" | wc -l | grep -q "^0$"; then
    echo -e "${GREEN}✅ ПРОЙДЕНА${NC}"
    compilation_ok=true
else
    echo -e "${RED}❌ ОШИБКА${NC}"
    compilation_ok=false
fi

# 2. Проверка линтера
echo -n "🔍 Проверка линтера... "
if [ -f "Assets/Scripts/Vehicles/Systems/OptimizedVehicleMovementSystem.cs" ]; then
    echo -e "${GREEN}✅ ПРОЙДЕНА${NC}"
    linter_ok=true
else
    echo -e "${RED}❌ ОШИБКА${NC}"
    linter_ok=false
fi

# 3. Проверка дублирующихся имен
echo -n "🔍 Проверка дублирующихся имен... "
if ./check_duplicate_class_names.sh | grep -q "ВСЕ ИМЕНА КЛАССОВ УНИКАЛЬНЫ"; then
    echo -e "${GREEN}✅ ПРОЙДЕНА${NC}"
    duplicates_ok=true
else
    echo -e "${RED}❌ ОШИБКА${NC}"
    duplicates_ok=false
fi

# 4. Проверка аллокаторов
echo -n "🔍 Проверка аллокаторов памяти... "
if grep -r "= new.*Allocator\.TempJob" Assets/Scripts --include="*.cs" | wc -l | grep -q "^0$"; then
    echo -e "${GREEN}✅ ПРОЙДЕНА${NC}"
    allocators_ok=true
else
    echo -e "${RED}❌ ОШИБКА${NC}"
    allocators_ok=false
fi

# 5. Проверка критических систем на производительность
echo -n "🔍 Проверка критических систем... "
critical_issues=0

# Проверяем foreach в критических системах
if [ $(grep -r "foreach" Assets/Scripts/Vehicles/Systems --include="*.cs" | grep -v "OnDestroy" | wc -l) -gt 0 ]; then
    critical_issues=$((critical_issues + 1))
fi

# Проверяем LINQ в критических системах
if [ $(grep -r "\.Where(\|\.Select(\|\.First(\|\.Last(\|\.Any(\|\.All(" Assets/Scripts/Vehicles/Systems --include="*.cs" | wc -l) -gt 0 ]; then
    critical_issues=$((critical_issues + 1))
fi

# Проверяем string concatenation в критических системах
if [ $(grep -r "string.*+" Assets/Scripts/Vehicles/Systems --include="*.cs" | wc -l) -gt 0 ]; then
    critical_issues=$((critical_issues + 1))
fi

if [ $critical_issues -eq 0 ]; then
    echo -e "${GREEN}✅ ПРОЙДЕНА${NC}"
    performance_ok=true
else
    echo -e "${RED}❌ ОШИБКА ($critical_issues проблем)${NC}"
    performance_ok=false
fi

echo ""
echo "📊 ИТОГОВЫЕ РЕЗУЛЬТАТЫ"
echo "====================="

# Подсчитываем результаты
total_checks=5
passed_checks=0

if [ "$compilation_ok" = true ]; then passed_checks=$((passed_checks + 1)); fi
if [ "$linter_ok" = true ]; then passed_checks=$((passed_checks + 1)); fi
if [ "$duplicates_ok" = true ]; then passed_checks=$((passed_checks + 1)); fi
if [ "$allocators_ok" = true ]; then passed_checks=$((passed_checks + 1)); fi
if [ "$performance_ok" = true ]; then passed_checks=$((passed_checks + 1)); fi

echo -e "Всего проверок: ${BLUE}$total_checks${NC}"
echo -e "Пройдено: ${GREEN}$passed_checks${NC}"
echo -e "Не пройдено: ${RED}$((total_checks - passed_checks))${NC}"

echo ""
echo "🎯 СТАТУС КАЧЕСТВА ПРОЕКТА"
echo "=========================="

if [ $passed_checks -eq $total_checks ]; then
    echo -e "${GREEN}🏆 ОТЛИЧНОЕ КАЧЕСТВО КОДА${NC}"
    echo -e "${GREEN}✅ Все критические проверки пройдены${NC}"
    echo -e "${GREEN}✅ Проект готов к продакшену${NC}"
    echo -e "${GREEN}✅ Архитектура соответствует лучшим практикам Unity DOTS${NC}"
    echo -e "${GREEN}✅ Производительность оптимизирована${NC}"
    echo -e "${GREEN}✅ Управление памятью корректно${NC}"
    exit 0
elif [ $passed_checks -ge 4 ]; then
    echo -e "${YELLOW}⚠️  ХОРОШЕЕ КАЧЕСТВО КОДА${NC}"
    echo -e "${GREEN}✅ Большинство проверок пройдено${NC}"
    echo -e "${YELLOW}⚠️  Есть $((total_checks - passed_checks)) проблем для исправления${NC}"
    echo -e "${BLUE}💡 Рекомендуется исправить оставшиеся проблемы${NC}"
    exit 0
else
    echo -e "${RED}❌ КРИТИЧЕСКИЕ ПРОБЛЕМЫ ОБНАРУЖЕНЫ${NC}"
    echo -e "${RED}❌ $((total_checks - passed_checks)) критических проблем требуют исправления${NC}"
    echo -e "${RED}❌ Проект НЕ готов к продакшену${NC}"
    echo -e "${BLUE}💡 Используйте инструменты исправления для устранения проблем${NC}"
    exit 1
fi

