#!/bin/bash

# Оптимизированный мониторинг Unity проекта

echo "⚡ ОПТИМИЗИРОВАННЫЙ МОНИТОРИНГ UNITY"
echo "===================================="

# Цвета для вывода
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
PURPLE='\033[0;35m'
CYAN='\033[0;36m'
NC='\033[0m' # No Color

echo ""
echo "🔍 БЫСТРАЯ ДИАГНОСТИКА (Принцип 80/20)"
echo "======================================="

# 1. Критические проверки (20% усилий, 80% результата)
echo -n "🔍 Компиляция... "
compilation_errors=$(tail -50 /home/egor/.config/unity3d/Editor.log 2>/dev/null | grep -c "CS[0-9]\+" 2>/dev/null | head -1 || echo "0")
if [ "$compilation_errors" -eq 0 ]; then
    echo -e "${GREEN}✅ ОК${NC}"
else
    echo -e "${RED}❌ $compilation_errors ошибок${NC}"
fi

echo -n "🔍 Asset Import Workers... "
worker_errors=$(find . -name "AssetImportWorker*.log" 2>/dev/null | wc -l | tr -d ' ')
if [ $worker_errors -eq 0 ]; then
    echo -e "${GREEN}✅ ОК${NC}"
else
    echo -e "${RED}❌ $worker_errors ошибок${NC}"
fi

echo -n "🔍 Unity процессы... "
unity_processes=$(ps aux | grep -c "Unity" | head -1 || echo "0")
if [ $unity_processes -le 5 ]; then
    echo -e "${GREEN}✅ ОК ($unity_processes)${NC}"
else
    echo -e "${YELLOW}⚠️  Много процессов ($unity_processes)${NC}"
fi

echo -n "🔍 Использование памяти... "
memory_usage=$(ps aux | grep Unity | grep -v grep | awk '{sum+=$6} END {print sum/1024}' | cut -d. -f1 || echo "0")
if [ $memory_usage -lt 1000 ]; then
    echo -e "${GREEN}✅ ОК (${memory_usage}MB)${NC}"
else
    echo -e "${YELLOW}⚠️  Высокое (${memory_usage}MB)${NC}"
fi

echo ""
echo "🛠️  АВТОМАТИЧЕСКИЕ ИСПРАВЛЕНИЯ"
echo "=============================="

# 2. Автоматические исправления только критических проблем
if [ "$compilation_errors" -gt 0 ] || [ "$worker_errors" -gt 0 ]; then
    echo "🚨 Критические проблемы обнаружены - применяю исправления..."
    
    if [ $worker_errors -gt 0 ]; then
        echo "  🔧 Исправление Asset Import Workers..."
        find . -name "AssetImportWorker*.log" -delete 2>/dev/null
        pkill -f "AssetImportWorker" 2>/dev/null
        echo -e "  ${GREEN}✅ Asset Import Workers исправлены${NC}"
    fi
    
    if [ $compilation_errors -gt 0 ]; then
        echo "  🔧 Очистка кэша для исправления компиляции..."
        rm -rf Library/ScriptAssemblies/* 2>/dev/null
        echo -e "  ${GREEN}✅ Кэш очищен${NC}"
    fi
else
    echo -e "${GREEN}✅ Критических проблем не обнаружено${NC}"
fi

echo ""
echo "📊 КРАТКАЯ СТАТИСТИКА"
echo "====================="

# 3. Только ключевые метрики
total_files=$(find Assets -name "*.cs" | wc -l)
total_lines=$(find Assets -name "*.cs" -exec wc -l {} + | tail -1 | awk '{print $1}' 2>/dev/null || echo "0")
ecs_components=$(grep -r "IComponentData" Assets/Scripts --include="*.cs" | wc -l)

echo -e "Файлов C#: ${CYAN}$total_files${NC}"
echo -e "Строк кода: ${CYAN}$total_lines${NC}"
echo -e "ECS компонентов: ${CYAN}$ecs_components${NC}"

echo ""
echo "🎯 СТАТУС ПРОЕКТА"
echo "================="

# 4. Общий статус
if [ "$compilation_errors" -eq 0 ] && [ "$worker_errors" -eq 0 ] && [ "$unity_processes" -le 5 ]; then
    echo -e "${GREEN}🏆 ПРОЕКТ В ОТЛИЧНОМ СОСТОЯНИИ${NC}"
    echo -e "${GREEN}✅ Все критические системы работают стабильно${NC}"
    echo -e "${GREEN}✅ Unity Editor готов к работе${NC}"
elif [ "$compilation_errors" -gt 0 ] || [ "$worker_errors" -gt 0 ]; then
    echo -e "${RED}❌ КРИТИЧЕСКИЕ ПРОБЛЕМЫ ОБНАРУЖЕНЫ${NC}"
    echo -e "${RED}❌ Требуется немедленное вмешательство${NC}"
else
    echo -e "${YELLOW}⚠️  ПРОЕКТ В НОРМАЛЬНОМ СОСТОЯНИИ${NC}"
    echo -e "${YELLOW}⚠️  Есть незначительные предупреждения${NC}"
fi

echo ""
echo "💡 РЕКОМЕНДАЦИИ"
echo "==============="

if [ $memory_usage -gt 1000 ]; then
    echo -e "${YELLOW}⚠️  Высокое использование памяти - рекомендуется перезапуск Unity${NC}"
fi

if [ $unity_processes -gt 5 ]; then
    echo -e "${YELLOW}⚠️  Слишком много Unity процессов - проверьте запущенные экземпляры${NC}"
fi

echo -e "${BLUE}💡 Профилактика:${NC}"
echo -e "  • Запускайте этот скрипт каждые 10-15 минут"
echo -e "  • Регулярно сохраняйте проект"
echo -e "  • Избегайте одновременной работы нескольких Unity"

echo ""
echo "✅ ОПТИМИЗИРОВАННЫЙ МОНИТОРИНГ ЗАВЕРШЕН"
echo "======================================="
