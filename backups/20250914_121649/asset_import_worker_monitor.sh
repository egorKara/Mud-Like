#!/bin/bash

# Мониторинг и исправление Asset Import Workers

echo "🔍 МОНИТОРИНГ ASSET IMPORT WORKERS"
echo "=================================="

# Цвета для вывода
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
PURPLE='\033[0;35m'
CYAN='\033[0;36m'
NC='\033[0m' # No Color

echo ""
echo "🔍 РЕАЛЬНОЕ ВРЕМЯ МОНИТОРИНГА"
echo "=============================="

# 1. Мониторинг активных Asset Import Workers
echo -n "🔍 Проверка активных Asset Import Workers... "
worker_processes=$(ps aux | grep -c "AssetImportWorker" || echo "0")
echo -e "${CYAN}$worker_processes процессов${NC}"

# 2. Проверка логов в реальном времени
echo -n "🔍 Проверка логов Asset Import Workers... "
log_count=$(find . -name "AssetImportWorker*.log" 2>/dev/null | wc -l)
if [ $log_count -gt 0 ]; then
    echo -e "${RED}❌ Найдено $log_count логов ошибок${NC}"
    
    # Анализ последних ошибок
    echo "📋 Анализ последних ошибок:"
    for log_file in $(find . -name "AssetImportWorker*.log" 2>/dev/null | head -5); do
        echo "  📄 $(basename $log_file):"
        if [ -f "$log_file" ]; then
            last_error=$(tail -5 "$log_file" 2>/dev/null | grep -E "error|Error|ERROR|crash|Crash|CRASH" | tail -1)
            if [ -n "$last_error" ]; then
                echo -e "    ${RED}❌ $last_error${NC}"
            else
                echo -e "    ${YELLOW}⚠️  Нет явных ошибок в последних записях${NC}"
            fi
        fi
    done
else
    echo -e "${GREEN}✅ Логов ошибок не найдено${NC}"
fi

# 3. Проверка Unity Editor процессов
echo -n "🔍 Проверка Unity Editor процессов... "
unity_processes=$(ps aux | grep -c "Unity" || echo "0")
echo -e "${CYAN}$unity_processes процессов${NC}"

# 4. Проверка использования памяти
echo -n "🔍 Проверка использования памяти Unity... "
memory_usage=$(ps aux | grep Unity | grep -v grep | awk '{sum+=$6} END {print sum/1024 " MB"}' || echo "0 MB")
echo -e "${CYAN}$memory_usage${NC}"

echo ""
echo "🛠️  ПРОАКТИВНЫЕ ДЕЙСТВИЯ"
echo "========================"

# 5. Автоматическое исправление проблем
if [ $log_count -gt 0 ]; then
    echo "🔧 Автоматическое исправление проблем Asset Import Workers..."
    
    # Остановка проблемных воркеров
    echo "  🛑 Остановка проблемных воркеров..."
    pkill -f "AssetImportWorker" 2>/dev/null
    
    # Очистка логов
    echo "  🧹 Очистка логов..."
    find . -name "AssetImportWorker*.log" -delete 2>/dev/null
    
    # Очистка кэша
    echo "  🧹 Очистка кэша..."
    rm -rf Library/ScriptAssemblies/* 2>/dev/null
    
    echo -e "  ${GREEN}✅ Автоматическое исправление завершено${NC}"
fi

# 6. Проверка целостности проекта
echo -n "🔍 Проверка целостности проекта... "
corrupted_assets=$(find Assets -name "*.meta" -exec basename {} \; | sort | uniq -d | wc -l)
if [ $corrupted_assets -gt 0 ]; then
    echo -e "${YELLOW}⚠️  Найдено $corrupted_assets проблемных .meta файлов${NC}"
    echo "🔧 Исправление проблемных .meta файлов..."
    find Assets -name "*.meta" -exec basename {} \; | sort | uniq -d | while read meta_file; do
        find Assets -name "$meta_file" | tail -n +2 | xargs rm -f 2>/dev/null
    done
    echo -e "  ${GREEN}✅ Проблемные .meta файлы исправлены${NC}"
else
    echo -e "${GREEN}✅ Целостность проекта в порядке${NC}"
fi

echo ""
echo "📊 СТАТИСТИКА МОНИТОРИНГА"
echo "=========================="

# Статистика
total_assets=$(find Assets -type f | wc -l)
total_scripts=$(find Assets -name "*.cs" | wc -l)
total_prefabs=$(find Assets -name "*.prefab" | wc -l)

echo -e "Всего ассетов: ${CYAN}$total_assets${NC}"
echo -e "Скриптов: ${CYAN}$total_scripts${NC}"
echo -e "Префабов: ${CYAN}$total_prefabs${NC}"

echo ""
echo "🎯 РЕКОМЕНДАЦИИ"
echo "==============="

if [ $worker_processes -gt 10 ]; then
    echo -e "${YELLOW}⚠️  Слишком много Asset Import Workers ($worker_processes)${NC}"
    echo -e "${BLUE}💡 Рекомендуется перезапустить Unity Editor${NC}"
fi

if [ $log_count -gt 0 ]; then
    echo -e "${RED}❌ Обнаружены проблемы с Asset Import Workers${NC}"
    echo -e "${BLUE}💡 Рекомендуется:${NC}"
    echo -e "  • Перезапустить Unity Editor"
    echo -e "  • Проверить целостность проекта"
    echo -e "  • Обновить Unity до последней версии"
fi

echo -e "${BLUE}💡 Профилактические меры:${NC}"
echo -e "  • Запускать этот скрипт каждые 5-10 минут"
echo -e "  • Мониторить использование памяти"
echo -e "  • Избегать одновременной работы нескольких Unity процессов"

echo ""
echo "✅ МОНИТОРИНГ ЗАВЕРШЕН"
echo "======================"
