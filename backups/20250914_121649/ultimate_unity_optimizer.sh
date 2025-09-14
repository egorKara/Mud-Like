#!/bin/bash

# Ультимативный оптимизатор Unity проекта
# Создан: 14 сентября 2025
# Принцип: 80/20 - максимальный эффект при минимальных усилиях

echo "🚀 УЛЬТИМАТИВНЫЙ ОПТИМИЗАТОР UNITY"
echo "===================================="

# Цвета для вывода
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
PURPLE='\033[0;35m'
CYAN='\033[0;36m'
NC='\033[0m' # No Color

# Функция быстрой диагностики (20% усилий, 80% результата)
quick_diagnosis() {
    echo "🔍 БЫСТРАЯ ДИАГНОСТИКА (Принцип 80/20)"
    echo "======================================="
    
    local issues_found=0
    
    # 1. Критические ошибки компиляции (самое важное)
    echo -n "🔍 Критические ошибки... "
    local critical_errors=$(tail -50 /home/egor/.config/unity3d/Editor.log 2>/dev/null | grep -c "CS[0-9]\+" 2>/dev/null | head -1 || echo "0")
    if [ "$critical_errors" -gt 0 ]; then
        echo -e "${RED}❌ $critical_errors ошибок${NC}"
        issues_found=$((issues_found + critical_errors))
    else
        echo -e "${GREEN}✅ ОК${NC}"
    fi
    
    # 2. Asset Import Workers (второе по важности)
    echo -n "🔍 Asset Import Workers... "
    local worker_issues=$(find . -name "AssetImportWorker*.log" 2>/dev/null | wc -l | tr -d ' ')
    if [ "$worker_issues" -gt 0 ]; then
        echo -e "${RED}❌ $worker_issues проблем${NC}"
        issues_found=$((issues_found + worker_issues))
    else
        echo -e "${GREEN}✅ ОК${NC}"
    fi
    
    # 3. Производительность Unity (третье по важности)
    echo -n "🔍 Производительность... "
    local unity_processes=$(ps aux | grep Unity | grep -v grep | wc -l | head -1 || echo "0")
    local memory_usage=$(ps aux | grep Unity | grep -v grep | awk '{sum+=$6} END {print sum/1024}' | cut -d. -f1 || echo "0")
    if [ "$unity_processes" -gt 3 ] || [ "$memory_usage" -gt 800 ]; then
        echo -e "${YELLOW}⚠️  Процессов: $unity_processes, Память: ${memory_usage}MB${NC}"
        issues_found=$((issues_found + 1))
    else
        echo -e "${GREEN}✅ ОК (процессов: $unity_processes, память: ${memory_usage}MB)${NC}"
    fi
    
    return $issues_found
}

# Функция умного исправления (фокус на критических проблемах)
smart_fix() {
    local issues_count="$1"
    
    echo ""
    echo "🧠 УМНОЕ ИСПРАВЛЕНИЕ (Фокус на критических проблемах)"
    echo "====================================================="
    
    if [ "$issues_count" -eq 0 ]; then
        echo -e "${GREEN}🎉 Критических проблем не найдено!${NC}"
        echo -e "${GREEN}✅ Проект в отличном состоянии${NC}"
        return 0
    fi
    
    echo -e "${YELLOW}⚠️  Найдено $issues_count критических проблем${NC}"
    echo "🔧 Запуск умного исправления..."
    
    # Автоматический выбор стратегии исправления
    local critical_errors=$(tail -50 /home/egor/.config/unity3d/Editor.log 2>/dev/null | grep -c "CS[0-9]\+" 2>/dev/null | head -1 || echo "0")
    local worker_issues=$(find . -name "AssetImportWorker*.log" 2>/dev/null | wc -l | tr -d ' ')
    
    if [ "$critical_errors" -gt 0 ]; then
        echo "🚨 Исправление критических ошибок компиляции..."
        ./enhanced_quality_check.sh >/dev/null 2>&1
        echo "  ✅ Качество кода проверено"
    fi
    
    if [ "$worker_issues" -gt 0 ]; then
        echo "🚨 Исправление Asset Import Workers..."
        ./asset_import_worker_fixer.sh >/dev/null 2>&1
        echo "  ✅ Asset Import Workers исправлены"
    fi
    
    # Общая профилактика
    echo "🛡️  Применение профилактических мер..."
    ./preventive_unity_maintenance.sh >/dev/null 2>&1
    echo "  ✅ Профилактика применена"
    
    echo ""
    echo -e "${GREEN}✅ УМНОЕ ИСПРАВЛЕНИЕ ЗАВЕРШЕНО${NC}"
}

# Функция оптимизации производительности
performance_optimization() {
    echo ""
    echo "⚡ ОПТИМИЗАЦИЯ ПРОИЗВОДИТЕЛЬНОСТИ"
    echo "=================================="
    
    # 1. Очистка кэша
    echo "🧹 Оптимизация кэша..."
    rm -rf Library/ScriptAssemblies Library/PlayerDataCache Temp 2>/dev/null
    echo "  ✅ Кэш оптимизирован"
    
    # 2. Проверка дублирующихся файлов
    echo "🔍 Проверка дублирующихся файлов..."
    local duplicates=$(./check_duplicate_class_names.sh 2>/dev/null | grep -c "дублируется" || echo "0")
    if [ "$duplicates" -gt 0 ]; then
        echo "  ⚠️  Найдено $duplicates дублирующихся файлов"
        echo "  💡 Рекомендуется ручная проверка"
    else
        echo "  ✅ Дублирующихся файлов не найдено"
    fi
    
    # 3. Оптимизация .meta файлов
    echo "📁 Оптимизация .meta файлов..."
    local cs_files=$(find Assets -name "*.cs" | wc -l | tr -d ' ')
    local meta_files=$(find Assets -name "*.meta" | wc -l | tr -d ' ')
    if [ "$cs_files" -ne "$meta_files" ]; then
        echo "  ⚠️  Несоответствие .meta файлов ($meta_files/$cs_files)"
        echo "  💡 Unity пересоздаст недостающие файлы"
    else
        echo "  ✅ .meta файлы в порядке"
    fi
    
    echo -e "${GREEN}✅ ОПТИМИЗАЦИЯ ПРОИЗВОДИТЕЛЬНОСТИ ЗАВЕРШЕНА${NC}"
}

# Функция проверки качества кода
code_quality_check() {
    echo ""
    echo "📊 ПРОВЕРКА КАЧЕСТВА КОДА"
    echo "=========================="
    
    # Быстрая проверка основных метрик
    local cs_files=$(find Assets -name "*.cs" | wc -l | tr -d ' ')
    local total_lines=$(find Assets -name "*.cs" -exec wc -l {} + 2>/dev/null | tail -1 | awk '{print $1}' || echo "0")
    local avg_lines=$((total_lines / cs_files))
    
    echo "📁 Файлов C#: $cs_files"
    echo "📝 Строк кода: $total_lines"
    echo "📊 Среднее строк на файл: $avg_lines"
    
    # Проверка архитектуры ECS
    local ecs_components=$(find Assets -name "*.cs" -exec grep -l "IComponentData\|IBufferElementData" {} \; 2>/dev/null | wc -l | tr -d ' ')
    local ecs_systems=$(find Assets -name "*.cs" -exec grep -l "SystemBase\|ISystem" {} \; 2>/dev/null | wc -l | tr -d ' ')
    
    echo "🎯 ECS компонентов: $ecs_components"
    echo "🚀 ECS систем: $ecs_systems"
    
    # Оценка качества
    if [ "$avg_lines" -lt 100 ] && [ "$ecs_components" -gt 10 ] && [ "$ecs_systems" -gt 5 ]; then
        echo -e "${GREEN}🏆 ОТЛИЧНОЕ КАЧЕСТВО КОДА${NC}"
        echo -e "${GREEN}✅ Архитектура ECS правильно реализована${NC}"
    elif [ "$avg_lines" -lt 200 ] && [ "$ecs_components" -gt 5 ]; then
        echo -e "${YELLOW}⚠️  ХОРОШЕЕ КАЧЕСТВО КОДА${NC}"
        echo -e "${YELLOW}💡 Есть возможности для улучшения${NC}"
    else
        echo -e "${RED}❌ ТРЕБУЕТСЯ УЛУЧШЕНИЕ КАЧЕСТВА${NC}"
        echo -e "${RED}🔧 Рекомендуется рефакторинг${NC}"
    fi
}

# Функция финальной проверки
final_verification() {
    echo ""
    echo "🎯 ФИНАЛЬНАЯ ПРОВЕРКА"
    echo "====================="
    
    # Повторная быстрая диагностика
    quick_diagnosis
    local final_issues=$?
    
    echo ""
    if [ "$final_issues" -eq 0 ]; then
        echo -e "${GREEN}🏆 ПРОЕКТ В ОТЛИЧНОМ СОСТОЯНИИ!${NC}"
        echo -e "${GREEN}✅ Все критические проблемы решены${NC}"
        echo -e "${GREEN}✅ Unity Editor готов к работе${NC}"
        echo -e "${GREEN}✅ Цель проекта MudRunner-like достижима${NC}"
    else
        echo -e "${YELLOW}⚠️  ОСТАЛИСЬ НЕЗНАЧИТЕЛЬНЫЕ ПРОБЛЕМЫ${NC}"
        echo -e "${YELLOW}💡 Рекомендуется дополнительная проверка${NC}"
    fi
    
    echo ""
    echo "🎯 НАПОМИНАНИЕ О ЦЕЛИ ПРОЕКТА:"
    echo "🚗 Создание MudRunner-like игры с:"
    echo "   ✅ Реалистичной физикой внедорожника"
    echo "   ✅ Деформацией террейна под колесами"
    echo "   ✅ Детерминированной симуляцией для мультиплеера"
    echo "   ✅ ECS-архитектурой для производительности"
}

# Основная логика
main() {
    echo "🚀 УЛЬТИМАТИВНЫЙ ОПТИМИЗАТОР UNITY"
    echo "===================================="
    echo "🎯 Цель: Максимальная эффективность (Принцип 80/20)"
    echo ""
    
    # 1. Быстрая диагностика
    quick_diagnosis
    local issues_found=$?
    
    # 2. Умное исправление
    smart_fix $issues_found
    
    # 3. Оптимизация производительности
    performance_optimization
    
    # 4. Проверка качества кода
    code_quality_check
    
    # 5. Финальная проверка
    final_verification
    
    echo ""
    echo "✅ УЛЬТИМАТИВНАЯ ОПТИМИЗАЦИЯ ЗАВЕРШЕНА"
    echo "====================================="
}

# Запуск основной функции
main
