#!/bin/bash
# Продвинутый мониторинг здоровья Unity Editor

echo "🔍 ПРОДВИНУТЫЙ МОНИТОРИНГ ЗДОРОВЬЯ UNITY EDITOR"
echo "==============================================="

# Проверка процессов Unity
unity_processes=$(pgrep -f "unity" | wc -l)
echo "🔄 Процессы Unity: $unity_processes"

# Проверка логов на новые ошибки
log_files=(
    "$HOME/.config/unity3d/Editor.log"
    "$HOME/.config/unity3d/Unity/Unity.Licensing.Client.log"
    "$HOME/.config/unity3d/Unity/Unity.Entitlements.Audit.log"
    "./Logs/shadercompiler-UnityShaderCompiler-0.log"
)

total_errors=0
total_warnings=0

for log_file in "${log_files[@]}"; do
    if [ -f "$log_file" ]; then
        error_count=$(grep -c -i "error\|exception\|failed\|critical" "$log_file" 2>/dev/null || echo "0")
        warning_count=$(grep -c -i "warning" "$log_file" 2>/dev/null || echo "0")
        
        total_errors=$((total_errors + error_count))
        total_warnings=$((total_warnings + warning_count))
        
        if [ "$error_count" -gt 0 ] || [ "$warning_count" -gt 0 ]; then
            echo "📄 $log_file: $error_count ошибок, $warning_count предупреждений"
        fi
    fi
done

echo "📊 Общая статистика: $total_errors ошибок, $total_warnings предупреждений"

if [ "$total_errors" -gt 0 ]; then
    echo "🚨 ОБНАРУЖЕНЫ ОШИБКИ!"
    echo "🔧 Запуск продвинутого исправителя..."
    ./advanced_unity_error_fixer_v2.sh
else
    echo "✅ Ошибок не обнаружено"
fi

# Проверка кэша
if [ -d "Library" ] && [ -d "Library/ScriptAssemblies" ]; then
    echo "✅ Кэш: ОК"
else
    echo "⚠️  Кэш: Требуется очистка"
fi

# Проверка переменных окружения
if [ -n "$G_MESSAGES_DEBUG" ] && [ "$G_MESSAGES_DEBUG" = "none" ]; then
    echo "✅ GLib: Настроен"
else
    echo "⚠️  GLib: Требуется настройка"
fi

if [ -n "$UNITY_LICENSE_TYPE" ]; then
    echo "✅ Лицензирование: Настроено"
else
    echo "⚠️  Лицензирование: Требуется настройка"
fi

echo "🎯 Продвинутый мониторинг завершен"
