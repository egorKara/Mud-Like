#!/bin/bash

# Система предотвращения ошибок Unity Editor для проекта Mud-Like

echo "🛡️  Система предотвращения ошибок Unity Editor"
echo "=============================================="

# Цвета для вывода
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Счетчики
TOTAL_CHECKS=0
PASSED_CHECKS=0
FAILED_CHECKS=0
WARNINGS=0

# Функция для проверки
check_item() {
    local description="$1"
    local command="$2"
    local critical="$3"
    
    TOTAL_CHECKS=$((TOTAL_CHECKS + 1))
    echo -n "🔍 Проверка: $description... "
    
    if eval "$command" > /dev/null 2>&1; then
        echo -e "${GREEN}✅ ПРОЙДЕНА${NC}"
        PASSED_CHECKS=$((PASSED_CHECKS + 1))
        return 0
    else
        if [ "$critical" = "true" ]; then
            echo -e "${RED}❌ ОШИБКА${NC}"
            FAILED_CHECKS=$((FAILED_CHECKS + 1))
            return 1
        else
            echo -e "${YELLOW}⚠️  ПРЕДУПРЕЖДЕНИЕ${NC}"
            WARNINGS=$((WARNINGS + 1))
            return 2
        fi
    fi
}

echo ""
echo "📊 ВЫПОЛНЕНИЕ ПРОВЕРОК КАЧЕСТВА КОДА"
echo "====================================="

# 1. Проверка на устаревший Time API
check_item "Устаревший Time API" "grep -r 'Time\\.deltaTime\\|Time\\.fixedDeltaTime\\|Time\\.time' Assets/Scripts --include='*.cs' | grep -v 'SystemAPI.Time' | grep -v '// Time\\.' | grep -v 'DateTime.Now' | grep -v 'IssueType' | grep -v 'Contains' | grep -v 'UnityEngine.Time' | grep -v 'Examples' | wc -l | grep -q '^0$'" "true"

# 2. Проверка на Debug.Log в критических системах
check_item "Debug.Log в критических системах" "grep -r 'Debug\\.Log' Assets/Scripts/Vehicles/Systems --include='*.cs' | grep -v '#if UNITY_EDITOR' | grep -v '#endif' | wc -l | grep -q '^0$'" "true"

# 3. Проверка на неправильные аллокаторы
check_item "Неправильные аллокаторы памяти" "grep -r '= new.*Allocator\\.TempJob' Assets/Scripts --include='*.cs' | wc -l | grep -q '^0$'" "true"

# 4. Проверка на FindObjectOfType в критических системах
check_item "FindObjectOfType в критических системах" "grep -r 'FindObjectOfType' Assets/Scripts/Vehicles/Systems --include='*.cs' | wc -l | grep -q '^0$'" "true"

# 5. Проверка на foreach в критических системах
check_item "foreach в критических системах" "grep -r 'foreach' Assets/Scripts/Vehicles/Systems --include='*.cs' | grep -v 'OnDestroy' | wc -l | grep -q '^0$'" "false"

# 6. Проверка на LINQ в критических системах
check_item "LINQ в критических системах" "grep -r '\\.Where(\\|\\.Select(\\|\\.First(\\|\\.Last(\\|\\.Any(\\|\\.All(' Assets/Scripts/Vehicles/Systems --include='*.cs' | wc -l | grep -q '^0$'" "false"

# 7. Проверка на string concatenation в критических системах
check_item "String concatenation в критических системах" "grep -r 'string.*+' Assets/Scripts/Vehicles/Systems --include='*.cs' | wc -l | grep -q '^0$'" "false"

# 8. Проверка на boxing в критических системах
check_item "Boxing операции в критических системах" "grep -r 'ToString()' Assets/Scripts/Vehicles/Systems --include='*.cs' | wc -l | grep -q '^0$'" "false"

# 9. Проверка на Reflection в критических системах
check_item "Reflection в критических системах" "grep -r 'using System.Reflection' Assets/Scripts/Vehicles/Systems --include='*.cs' | wc -l | grep -q '^0$'" "false"

# 10. Проверка на Generics в Burst коде
check_item "Generics в Burst коде" "grep -r 'class.*<T>' Assets/Scripts/Vehicles/Systems --include='*.cs' | wc -l | grep -q '^0$'" "false"

# 11. Проверка на дублирующиеся имена классов
check_item "Дублирующиеся имена классов" "./check_duplicate_class_names.sh | grep -q 'ВСЕ ИМЕНА КЛАССОВ УНИКАЛЬНЫ'"

# 12. Проверка на ошибки компиляции
check_item "Ошибки компиляции" "tail -100 /home/egor/.config/unity3d/Editor.log | grep -E 'CS[0-9]+' | wc -l | grep -q '^0$'" "true"

echo ""
echo "📊 РЕЗУЛЬТАТЫ ПРОВЕРОК"
echo "====================="
echo -e "Всего проверок: ${BLUE}$TOTAL_CHECKS${NC}"
echo -e "Пройдено: ${GREEN}$PASSED_CHECKS${NC}"
echo -e "Ошибок: ${RED}$FAILED_CHECKS${NC}"
echo -e "Предупреждений: ${YELLOW}$WARNINGS${NC}"

echo ""
echo "🎯 СТАТУС КАЧЕСТВА КОДА"
echo "======================="

if [ $FAILED_CHECKS -eq 0 ]; then
    if [ $WARNINGS -eq 0 ]; then
        echo -e "${GREEN}🏆 ОТЛИЧНОЕ КАЧЕСТВО КОДА${NC}"
        echo -e "${GREEN}✅ Все критические проверки пройдены${NC}"
        echo -e "${GREEN}✅ Нет предупреждений${NC}"
        echo -e "${GREEN}✅ Проект готов к продакшену${NC}"
        exit 0
    else
        echo -e "${YELLOW}⚠️  ХОРОШЕЕ КАЧЕСТВО КОДА${NC}"
        echo -e "${GREEN}✅ Все критические проверки пройдены${NC}"
        echo -e "${YELLOW}⚠️  Есть $WARNINGS предупреждений (не критично)${NC}"
        echo -e "${BLUE}💡 Рекомендуется исправить предупреждения для улучшения качества${NC}"
        exit 0
    fi
else
    echo -e "${RED}❌ КРИТИЧЕСКИЕ ПРОБЛЕМЫ ОБНАРУЖЕНЫ${NC}"
    echo -e "${RED}❌ $FAILED_CHECKS критических ошибок требуют исправления${NC}"
    echo -e "${RED}❌ Проект НЕ готов к продакшену${NC}"
    echo -e "${BLUE}💡 Используйте инструменты исправления для устранения проблем${NC}"
    exit 1
fi
