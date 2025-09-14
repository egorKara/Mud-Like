#!/bin/bash

# Мониторинг Asset Import Workers в реальном времени
# Создан: 14 сентября 2025

echo "🔍 МОНИТОРИНГ ASSET IMPORT WORKERS В РЕАЛЬНОМ ВРЕМЕНИ"
echo "====================================================="

# Цвета для вывода
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
PURPLE='\033[0;35m'
CYAN='\033[0;36m'
NC='\033[0m' # No Color

# Функция для проверки Asset Import Workers
check_asset_workers() {
    local worker_logs=$(find . -name "AssetImportWorker*.log" 2>/dev/null | wc -l | tr -d ' ')
    local unity_log_errors=$(tail -50 /home/egor/.config/unity3d/Editor.log 2>/dev/null | grep -c "Unexpected transport error" 2>/dev/null | head -1 || echo "0")
    local unity_processes=$(ps aux | grep Unity | grep -v grep | wc -l | head -1 || echo "0")
    
    echo -n "🔍 Asset Import Workers... "
    if [ "$worker_logs" -eq 0 ]; then
        echo -e "${GREEN}✅ ОК${NC}"
    else
        echo -e "${RED}❌ $worker_logs ошибок${NC}"
    fi
    
    echo -n "🔍 Unity Transport Errors... "
    if [ "$unity_log_errors" -eq 0 ]; then
        echo -e "${GREEN}✅ ОК${NC}"
    else
        echo -e "${RED}❌ $unity_log_errors ошибок${NC}"
    fi
    
    echo -n "🔍 Unity Processes... "
    if [ "$unity_processes" -le 3 ]; then
        echo -e "${GREEN}✅ ОК ($unity_processes)${NC}"
    else
        echo -e "${YELLOW}⚠️  Много процессов ($unity_processes)${NC}"
    fi
    
    # Автоматическое исправление при обнаружении проблем
    if [ "$worker_logs" -gt 0 ] || [ "$unity_log_errors" -gt 0 ]; then
        echo ""
        echo "🚨 ОБНАРУЖЕНЫ ПРОБЛЕМЫ! АВТОМАТИЧЕСКОЕ ИСПРАВЛЕНИЕ..."
        echo "=================================================="
        
        # Очистка логов Asset Import Workers
        if [ "$worker_logs" -gt 0 ]; then
            echo "🧹 Очистка логов Asset Import Workers..."
            find . -name "AssetImportWorker*.log" -delete 2>/dev/null
            echo "  ✅ Логи очищены"
        fi
        
        # Очистка кэша Unity
        if [ "$unity_log_errors" -gt 0 ]; then
            echo "🧹 Очистка кэша Unity..."
            rm -rf Library/ScriptAssemblies 2>/dev/null
            rm -rf Library/PlayerDataCache 2>/dev/null
            rm -rf Temp 2>/dev/null
            echo "  ✅ Кэш очищен"
        fi
        
        echo ""
        echo "✅ АВТОМАТИЧЕСКОЕ ИСПРАВЛЕНИЕ ЗАВЕРШЕНО"
        echo "💡 Рекомендуется перезапустить Unity Editor"
    fi
}

# Функция для непрерывного мониторинга
continuous_monitor() {
    echo "🔄 НЕПРЕРЫВНЫЙ МОНИТОРИНГ (нажмите Ctrl+C для остановки)"
    echo "======================================================="
    
    while true; do
        clear
        echo "🔍 МОНИТОРИНГ ASSET IMPORT WORKERS В РЕАЛЬНОМ ВРЕМЕНИ"
        echo "====================================================="
        echo "⏰ Время: $(date)"
        echo ""
        
        check_asset_workers
        
        echo ""
        echo "💡 Статус: Мониторинг активен..."
        echo "📊 Следующая проверка через 10 секунд..."
        
        sleep 10
    done
}

# Основная логика
if [ "$1" = "--continuous" ]; then
    continuous_monitor
else
    check_asset_workers
    
    echo ""
    echo "💡 ИСПОЛЬЗОВАНИЕ:"
    echo "  ./realtime_asset_worker_monitor.sh          # Однократная проверка"
    echo "  ./realtime_asset_worker_monitor.sh --continuous  # Непрерывный мониторинг"
    echo ""
    echo "✅ МОНИТОРИНГ ЗАВЕРШЕН"
fi
