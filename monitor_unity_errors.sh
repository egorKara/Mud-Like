#!/bin/bash
# Мониторинг ошибок Unity Editor

echo "🔍 МОНИТОРИНГ ОШИБОК UNITY EDITOR"
echo "================================="

# Проверка логов на новые ошибки
if [ -d "Logs" ]; then
    latest_log=$(find Logs -name "*.log" -type f -printf '%T@ %p\n' | sort -n | tail -1 | cut -d' ' -f2-)
    if [ -n "$latest_log" ]; then
        error_count=$(grep -c -i "error\|exception\|failed" "$latest_log" 2>/dev/null || echo "0")
        warning_count=$(grep -c -i "warning\|critical" "$latest_log" 2>/dev/null || echo "0")
        
        echo "📄 Последний лог: $latest_log"
        echo "❌ Ошибок: $error_count"
        echo "⚠️  Предупреждений: $warning_count"
        
        if [ "$error_count" -gt 0 ]; then
            echo "🚨 ОБНАРУЖЕНЫ ОШИБКИ!"
            echo "🔧 Запуск исправителя..."
            ./advanced_unity_log_analyzer.sh
        else
            echo "✅ Ошибок не обнаружено"
        fi
    fi
else
    echo "ℹ️  Папка Logs не найдена"
fi
