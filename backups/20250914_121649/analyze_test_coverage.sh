#!/bin/bash

# Скрипт для анализа покрытия тестами проекта

echo "🧪 Анализ покрытия тестами проекта Mud-Like..."
echo "=============================================="

# Подсчет файлов
TOTAL_SCRIPTS=$(find Assets/Scripts -name "*.cs" | wc -l)
TEST_FILES=$(find Assets/Scripts/Tests -name "*.cs" | wc -l)
NON_TEST_SCRIPTS=$((TOTAL_SCRIPTS - TEST_FILES))

echo "📊 СТАТИСТИКА ФАЙЛОВ:"
echo "   Всего файлов .cs: $TOTAL_SCRIPTS"
echo "   Тестовых файлов: $TEST_FILES"
echo "   Основных файлов: $NON_TEST_SCRIPTS"

# Расчет покрытия
if [ $NON_TEST_SCRIPTS -gt 0 ]; then
    COVERAGE_PERCENT=$((TEST_FILES * 100 / NON_TEST_SCRIPTS))
else
    COVERAGE_PERCENT=0
fi

echo "   Покрытие тестами: $COVERAGE_PERCENT%"

# Анализ типов тестов
echo ""
echo "🔍 АНАЛИЗ ТИПОВ ТЕСТОВ:"

UNIT_TESTS=$(find Assets/Scripts/Tests/Unit -name "*.cs" | wc -l)
INTEGRATION_TESTS=$(find Assets/Scripts/Tests/Integration -name "*.cs" | wc -l)
PERFORMANCE_TESTS=$(find Assets/Scripts/Tests/Performance -name "*.cs" | wc -l)
BEHAVIOR_TESTS=$(find Assets/Scripts/Tests/BehaviorDriven -name "*.cs" | wc -l)
PROPERTY_TESTS=$(find Assets/Scripts/Tests/PropertyBased -name "*.cs" | wc -l)

echo "   Unit тесты: $UNIT_TESTS"
echo "   Integration тесты: $INTEGRATION_TESTS"
echo "   Performance тесты: $PERFORMANCE_TESTS"
echo "   Behavior-Driven тесты: $BEHAVIOR_TESTS"
echo "   Property-Based тесты: $PROPERTY_TESTS"

# Анализ покрытия по модулям
echo ""
echo "📁 ПОКРЫТИЕ ПО МОДУЛЯМ:"

# Основные модули
MODULES=("Core" "Vehicles" "Terrain" "Audio" "Effects" "Networking" "UI")

for module in "${MODULES[@]}"; do
    MODULE_SCRIPTS=$(find Assets/Scripts/$module -name "*.cs" | wc -l)
    MODULE_TESTS=$(find Assets/Scripts/Tests -name "*.cs" -exec grep -l "$module" {} \; | wc -l)
    
    if [ $MODULE_SCRIPTS -gt 0 ]; then
        MODULE_COVERAGE=$((MODULE_TESTS * 100 / MODULE_SCRIPTS))
        echo "   $module: $MODULE_COVERAGE% ($MODULE_TESTS/$MODULE_SCRIPTS)"
    fi
done

# Анализ систем без тестов
echo ""
echo "❌ СИСТЕМЫ БЕЗ ТЕСТОВ:"

# Найти все системы
find Assets/Scripts -name "*System.cs" -not -path "*/Tests/*" | while read -r system_file; do
    system_name=$(basename "$system_file" .cs)
    
    # Проверить, есть ли тест для этой системы
    if ! find Assets/Scripts/Tests -name "*${system_name}*Test*.cs" | grep -q .; then
        echo "   🔴 $system_name"
    fi
done

# Анализ компонентов без тестов
echo ""
echo "❌ КОМПОНЕНТЫ БЕЗ ТЕСТОВ:"

find Assets/Scripts -path "*/Components/*.cs" -not -path "*/Tests/*" | while read -r component_file; do
    component_name=$(basename "$component_file" .cs)
    
    # Проверить, есть ли тест для этого компонента
    if ! find Assets/Scripts/Tests -name "*${component_name}*Test*.cs" | grep -q .; then
        echo "   🔴 $component_name"
    fi
done

# Рекомендации
echo ""
echo "💡 РЕКОМЕНДАЦИИ:"

if [ $COVERAGE_PERCENT -lt 50 ]; then
    echo "   🔴 Критическое покрытие ($COVERAGE_PERCENT%)"
    echo "   📋 Приоритет: Создать тесты для критических систем"
elif [ $COVERAGE_PERCENT -lt 80 ]; then
    echo "   🟡 Среднее покрытие ($COVERAGE_PERCENT%)"
    echo "   📋 Приоритет: Увеличить покрытие до 80%"
else
    echo "   🟢 Хорошее покрытие ($COVERAGE_PERCENT%)"
    echo "   📋 Приоритет: Поддерживать качество тестов"
fi

echo ""
echo "📋 ПЛАН УЛУЧШЕНИЯ:"
echo "   1. Создать тесты для систем без покрытия"
echo "   2. Добавить Integration тесты для критических путей"
echo "   3. Создать Performance тесты для оптимизации"
echo "   4. Настроить автоматический запуск тестов"
echo "   5. Добавить Code Coverage анализ"

# Проверка качества тестов
echo ""
echo "🔍 АНАЛИЗ КАЧЕСТВА ТЕСТОВ:"

# Проверить наличие основных типов тестов
if [ $UNIT_TESTS -gt 0 ]; then
    echo "   ✅ Unit тесты присутствуют"
else
    echo "   ❌ Unit тесты отсутствуют"
fi

if [ $INTEGRATION_TESTS -gt 0 ]; then
    echo "   ✅ Integration тесты присутствуют"
else
    echo "   ❌ Integration тесты отсутствуют"
fi

if [ $PERFORMANCE_TESTS -gt 0 ]; then
    echo "   ✅ Performance тесты присутствуют"
else
    echo "   ❌ Performance тесты отсутствуют"
fi

echo ""
echo "=============================================="

if [ $COVERAGE_PERCENT -ge 80 ]; then
    echo "🎯 ОТЛИЧНОЕ ПОКРЫТИЕ ТЕСТАМИ!"
    exit 0
elif [ $COVERAGE_PERCENT -ge 50 ]; then
    echo "📈 ХОРОШЕЕ ПОКРЫТИЕ, ЕСТЬ КУДА РАСТИ"
    exit 0
else
    echo "⚠️  НУЖНО УЛУЧШИТЬ ПОКРЫТИЕ ТЕСТАМИ"
    exit 1
fi
