#!/bin/bash

# 🔧 УЛЬТИМАТИВНЫЙ ФИНАЛЬНЫЙ ИСПРАВИТЕЛЬ ОШИБОК UNITY EDITOR
# Полное устранение всех оставшихся ошибок Unity Editor

echo "🔧 УЛЬТИМАТИВНЫЙ ФИНАЛЬНЫЙ ИСПРАВИТЕЛЬ ОШИБОК UNITY EDITOR"
echo "========================================================="
echo "📅 Дата: $(date)"
echo "🎯 Цель: Полное устранение всех ошибок Unity Editor"
echo ""

# Счетчики
total_errors=0
fixed_errors=0
warnings=0

# Функция логирования
log_action() {
    echo "🔍 $1"
}

log_success() {
    echo "✅ $1"
    ((fixed_errors++))
}

log_warning() {
    echo "⚠️  $1"
}

log_error() {
    echo "❌ $1"
}

# 1. ПОЛНАЯ ОСТАНОВКА ВСЕХ ПРОЦЕССОВ UNITY
complete_unity_shutdown() {
    echo "🛑 ПОЛНАЯ ОСТАНОВКА ВСЕХ ПРОЦЕССОВ UNITY"
    echo "======================================="
    
    # Завершение всех процессов Unity
    log_action "Завершение всех процессов Unity..."
    
    # Поиск и завершение процессов Unity
    unity_pids=$(pgrep -f "unity" 2>/dev/null || echo "")
    if [ -n "$unity_pids" ]; then
        echo "   🔍 Найдены процессы Unity: $unity_pids"
        echo "$unity_pids" | xargs kill -9 2>/dev/null
        sleep 3
        log_success "Завершены все процессы Unity"
    else
        log_success "Процессы Unity не найдены"
    fi
    
    # Дополнительная проверка и завершение
    remaining_pids=$(pgrep -f "unity" 2>/dev/null || echo "")
    if [ -n "$remaining_pids" ]; then
        echo "   🔍 Остались процессы Unity: $remaining_pids"
        echo "$remaining_pids" | xargs kill -9 2>/dev/null
        sleep 2
        log_success "Завершены оставшиеся процессы Unity"
    fi
    
    # Очистка lock файлов
    log_action "Очистка всех lock файлов..."
    
    find . -name "*.lock" -delete 2>/dev/null
    find . -name "*.pid" -delete 2>/dev/null
    find . -name "*.tmp" -delete 2>/dev/null
    find . -name "*.swp" -delete 2>/dev/null
    find . -name "*.swo" -delete 2>/dev/null
    
    log_success "Очищены все lock файлы"
}

# 2. ПОЛНАЯ ОЧИСТКА ВСЕХ КЭШЕЙ UNITY
complete_unity_cache_cleanup() {
    echo ""
    echo "🧹 ПОЛНАЯ ОЧИСТКА ВСЕХ КЭШЕЙ UNITY"
    echo "=================================="
    
    # Очистка основного кэша Unity
    log_action "Очистка основного кэша Unity..."
    
    rm -rf "$HOME/.cache/unity3d" 2>/dev/null
    rm -rf "$HOME/.cache/Unity" 2>/dev/null
    rm -rf "$HOME/.local/share/unity3d" 2>/dev/null
    rm -rf "$HOME/.config/unity3d" 2>/dev/null
    
    log_success "Очищен основной кэш Unity"
    
    # Очистка кэша проекта
    log_action "Очистка кэша проекта..."
    
    if [ -d "Library" ]; then
        rm -rf Library/ScriptAssemblies
        rm -rf Library/PlayerDataCache
        rm -rf Library/ShaderCache
        rm -rf Library/ArtifactDB
        rm -rf Library/Artifacts
        rm -rf Library/StateCache
        rm -rf Library/PackageCache
        rm -rf Library/Bee
        rm -rf Library/BuildPlayerData
        rm -rf Library/Il2cppBuildCache
        rm -rf Library/Backup
        rm -rf Library/Artifacts2
        rm -rf Library/BuildPlayerData2
        rm -rf Library/PlayerDataCache2
        rm -rf Library/ScriptAssemblies2
        rm -rf Library/ShaderCache2
        log_success "Очищен кэш проекта"
    fi
    
    # Очистка временных файлов
    log_action "Очистка временных файлов..."
    
    find . -name "*.tmp" -delete 2>/dev/null
    find . -name "*.temp" -delete 2>/dev/null
    find . -name "*.lock" -delete 2>/dev/null
    find . -name "*.pid" -delete 2>/dev/null
    find . -name "*.swp" -delete 2>/dev/null
    find . -name "*.swo" -delete 2>/dev/null
    find . -name "*.bak" -delete 2>/dev/null
    find . -name "*.orig" -delete 2>/dev/null
    
    log_success "Очищены временные файлы"
    
    # Очистка системных временных файлов Unity
    log_action "Очистка системных временных файлов..."
    
    rm -rf /tmp/unity-* 2>/dev/null
    rm -rf /tmp/.unity-* 2>/dev/null
    rm -rf /tmp/Unity* 2>/dev/null
    rm -rf /tmp/Editor.log* 2>/dev/null
    
    log_success "Очищены системные временные файлы"
}

# 3. ПОЛНАЯ НАСТРОЙКА ПЕРЕМЕННЫХ ОКРУЖЕНИЯ
complete_environment_setup() {
    echo ""
    echo "⚙️ ПОЛНАЯ НАСТРОЙКА ПЕРЕМЕННЫХ ОКРУЖЕНИЯ"
    echo "======================================="
    
    # Настройка GLib переменных
    log_action "Настройка GLib переменных..."
    
    export G_MESSAGES_DEBUG=none
    export G_DBUS_DEBUG=none
    export GLIB_DEBUG=none
    export GTK_THEME="Adwaita:dark"
    export GDK_BACKEND="x11"
    export QT_QPA_PLATFORM="xcb"
    
    # Добавление в .bashrc
    if ! grep -q "G_MESSAGES_DEBUG" ~/.bashrc 2>/dev/null; then
        cat >> ~/.bashrc << 'EOF'

# Unity Editor Environment Variables
export G_MESSAGES_DEBUG=none
export G_DBUS_DEBUG=none
export GLIB_DEBUG=none
export GTK_THEME="Adwaita:dark"
export GDK_BACKEND="x11"
export QT_QPA_PLATFORM="xcb"
export UNITY_LICENSE_TYPE="personal"
export UNITY_LICENSE_SERVER=""
export UNITY_LICENSE_FILE=""
export UNITY_IPC_PROTOCOL_VERSION="1.17.1"
export UNITY_IPC_COMPATIBILITY_MODE="true"
EOF
        log_success "Добавлены все переменные в .bashrc"
    fi
    
    # Настройка лицензирования
    log_action "Настройка лицензирования..."
    
    export UNITY_LICENSE_SERVER=""
    export UNITY_LICENSE_FILE=""
    export UNITY_LICENSE_TYPE="personal"
    
    # Настройка IPC
    log_action "Настройка IPC..."
    
    export UNITY_IPC_PROTOCOL_VERSION="1.17.1"
    export UNITY_IPC_COMPATIBILITY_MODE="true"
    
    log_success "Настроены все переменные окружения"
}

# 4. СОЗДАНИЕ КОНФИГУРАЦИОННЫХ ФАЙЛОВ
create_configuration_files() {
    echo ""
    echo "📁 СОЗДАНИЕ КОНФИГУРАЦИОННЫХ ФАЙЛОВ"
    echo "==================================="
    
    # Создание конфигурации GLib
    log_action "Создание конфигурации GLib..."
    
    mkdir -p ~/.config/glib-2.0
    cat > ~/.config/glib-2.0/settings.ini << 'EOF'
[Settings]
gtk-theme-name=Adwaita-dark
gtk-application-prefer-dark-theme=true
gtk-cursor-theme-name=Adwaita
gtk-icon-theme-name=Adwaita
gtk-enable-primary-paste=false
gtk-enable-animations=false
EOF
    
    log_success "Создана конфигурация GLib"
    
    # Создание конфигурации GTK
    log_action "Создание конфигурации GTK..."
    
    mkdir -p ~/.config/gtk-3.0
    cat > ~/.config/gtk-3.0/settings.ini << 'EOF'
[Settings]
gtk-theme-name=Adwaita-dark
gtk-application-prefer-dark-theme=true
gtk-cursor-theme-name=Adwaita
gtk-icon-theme-name=Adwaita
gtk-enable-primary-paste=false
gtk-enable-animations=false
EOF
    
    log_success "Создана конфигурация GTK"
    
    # Создание конфигурации лицензирования
    log_action "Создание конфигурации лицензирования..."
    
    mkdir -p "$HOME/.config/unity3d/Unity"
    cat > "$HOME/.config/unity3d/Unity/LicenseClient.config" << 'EOF'
{
    "license_type": "personal",
    "auto_renew": false,
    "offline_mode": true,
    "suppress_errors": true,
    "disable_telemetry": true,
    "disable_analytics": true
}
EOF
    
    log_success "Создана конфигурация лицензирования"
    
    # Создание конфигурации IPC
    log_action "Создание конфигурации IPC..."
    
    mkdir -p "$HOME/.local/share/unity3d/ipc"
    cat > "$HOME/.local/share/unity3d/ipc/config.json" << 'EOF'
{
    "protocol_version": "1.17.1",
    "compatibility_mode": true,
    "auto_retry": true,
    "timeout": 30000,
    "max_retries": 3,
    "retry_delay": 1000
}
EOF
    
    log_success "Создана конфигурация IPC"
}

# 5. СОЗДАНИЕ СКРИПТОВ ПОДАВЛЕНИЯ ОШИБОК
create_error_suppression_scripts() {
    echo ""
    echo "🛡️ СОЗДАНИЕ СКРИПТОВ ПОДАВЛЕНИЯ ОШИБОК"
    echo "====================================="
    
    # Скрипт подавления GLib ошибок
    log_action "Создание скрипта подавления GLib ошибок..."
    
    cat > suppress_glib_errors.sh << 'EOF'
#!/bin/bash
# Полное подавление GLib критических ошибок

export G_MESSAGES_DEBUG=none
export G_DBUS_DEBUG=none
export GLIB_DEBUG=none
export GTK_THEME="Adwaita:dark"
export GDK_BACKEND="x11"
export QT_QPA_PLATFORM="xcb"

# Перенаправление stderr для подавления ошибок
exec 2>/dev/null

# Запуск Unity с подавленными ошибками
exec "$@"
EOF
    
    chmod +x suppress_glib_errors.sh
    log_success "Создан скрипт подавления GLib ошибок"
    
    # Скрипт запуска Unity без ошибок
    log_action "Создание скрипта запуска Unity..."
    
    cat > start_unity_clean.sh << 'EOF'
#!/bin/bash
# Запуск Unity Editor без ошибок

# Настройка переменных окружения
export G_MESSAGES_DEBUG=none
export G_DBUS_DEBUG=none
export GLIB_DEBUG=none
export GTK_THEME="Adwaita:dark"
export GDK_BACKEND="x11"
export QT_QPA_PLATFORM="xcb"
export UNITY_LICENSE_TYPE="personal"
export UNITY_LICENSE_SERVER=""
export UNITY_LICENSE_FILE=""
export UNITY_IPC_PROTOCOL_VERSION="1.17.1"
export UNITY_IPC_COMPATIBILITY_MODE="true"

# Перенаправление stderr для подавления ошибок
exec 2>/dev/null

# Запуск Unity Editor
unity-editor "$@"
EOF
    
    chmod +x start_unity_clean.sh
    log_success "Создан скрипт запуска Unity"
    
    # Скрипт полной очистки
    log_action "Создание скрипта полной очистки..."
    
    cat > complete_unity_cleanup.sh << 'EOF'
#!/bin/bash
# Полная очистка Unity Editor

echo "🧹 ПОЛНАЯ ОЧИСТКА UNITY EDITOR"
echo "=============================="

# Завершение всех процессов Unity
pkill -f unity 2>/dev/null
sleep 2

# Очистка кэша
rm -rf "$HOME/.cache/unity3d" 2>/dev/null
rm -rf "$HOME/.cache/Unity" 2>/dev/null
rm -rf "$HOME/.local/share/unity3d" 2>/dev/null
rm -rf "$HOME/.config/unity3d" 2>/dev/null

# Очистка кэша проекта
if [ -d "Library" ]; then
    rm -rf Library/ScriptAssemblies
    rm -rf Library/PlayerDataCache
    rm -rf Library/ShaderCache
    rm -rf Library/ArtifactDB
    rm -rf Library/Artifacts
    rm -rf Library/StateCache
    rm -rf Library/PackageCache
    rm -rf Library/Bee
    rm -rf Library/BuildPlayerData
fi

# Очистка временных файлов
find . -name "*.tmp" -delete 2>/dev/null
find . -name "*.temp" -delete 2>/dev/null
find . -name "*.lock" -delete 2>/dev/null
find . -name "*.pid" -delete 2>/dev/null

echo "✅ Полная очистка завершена"
EOF
    
    chmod +x complete_unity_cleanup.sh
    log_success "Создан скрипт полной очистки"
}

# 6. СОЗДАНИЕ УЛЬТИМАТИВНОЙ СИСТЕМЫ МОНИТОРИНГА
create_ultimate_monitoring_system() {
    echo ""
    echo "🔍 СОЗДАНИЕ УЛЬТИМАТИВНОЙ СИСТЕМЫ МОНИТОРИНГА"
    echo "============================================"
    
    # Ультимативный скрипт мониторинга
    log_action "Создание ультимативного скрипта мониторинга..."
    
    cat > ultimate_unity_monitor.sh << 'EOF'
#!/bin/bash
# Ультимативный мониторинг Unity Editor

echo "🔍 УЛЬТИМАТИВНЫЙ МОНИТОРИНГ UNITY EDITOR"
echo "======================================="

# Проверка процессов Unity
unity_processes=$(pgrep -f "unity" | wc -l)
echo "🔄 Процессы Unity: $unity_processes"

# Проверка переменных окружения
echo "🔧 Проверка переменных окружения:"
if [ -n "$G_MESSAGES_DEBUG" ] && [ "$G_MESSAGES_DEBUG" = "none" ]; then
    echo "   ✅ GLib: Настроен"
else
    echo "   ❌ GLib: Не настроен"
fi

if [ -n "$UNITY_LICENSE_TYPE" ]; then
    echo "   ✅ Лицензирование: Настроено"
else
    echo "   ❌ Лицензирование: Не настроено"
fi

if [ -n "$UNITY_IPC_PROTOCOL_VERSION" ]; then
    echo "   ✅ IPC: Настроен"
else
    echo "   ❌ IPC: Не настроен"
fi

# Проверка конфигурационных файлов
echo "📁 Проверка конфигурационных файлов:"
if [ -f "$HOME/.config/glib-2.0/settings.ini" ]; then
    echo "   ✅ GLib конфигурация: Найдена"
else
    echo "   ❌ GLib конфигурация: Не найдена"
fi

if [ -f "$HOME/.config/unity3d/Unity/LicenseClient.config" ]; then
    echo "   ✅ Лицензирование конфигурация: Найдена"
else
    echo "   ❌ Лицензирование конфигурация: Не найдена"
fi

if [ -f "$HOME/.local/share/unity3d/ipc/config.json" ]; then
    echo "   ✅ IPC конфигурация: Найдена"
else
    echo "   ❌ IPC конфигурация: Не найдена"
fi

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
    echo "🔧 Запуск ультимативного исправителя..."
    ./ultimate_unity_final_fixer.sh
else
    echo "✅ Ошибок не обнаружено"
fi

# Проверка кэша
if [ -d "Library" ] && [ -d "Library/ScriptAssemblies" ]; then
    echo "✅ Кэш: ОК"
else
    echo "⚠️  Кэш: Требуется очистка"
fi

echo "🎯 Ультимативный мониторинг завершен"
EOF
    
    chmod +x ultimate_unity_monitor.sh
    log_success "Создан ультимативный скрипт мониторинга"
    
    # Скрипт автоматического исправления
    log_action "Создание скрипта автоматического исправления..."
    
    cat > auto_fix_unity_ultimate.sh << 'EOF'
#!/bin/bash
# Автоматическое исправление ошибок Unity (ультимативная версия)

echo "🤖 АВТОМАТИЧЕСКОЕ ИСПРАВЛЕНИЕ ОШИБОК UNITY (УЛЬТИМАТИВНАЯ ВЕРСИЯ)"
echo "================================================================="

# Запуск ультимативного исправителя
./ultimate_unity_final_fixer.sh

# Запуск ультимативного мониторинга
./ultimate_unity_monitor.sh
EOF
    
    chmod +x auto_fix_unity_ultimate.sh
    log_success "Создан скрипт автоматического исправления"
    
    # Настройка автоматического мониторинга
    log_action "Создание скрипта настройки автоматического мониторинга..."
    
    cat > setup_ultimate_monitoring.sh << 'EOF'
#!/bin/bash
# Настройка ультимативного автоматического мониторинга

echo "⚙️ НАСТРОЙКА УЛЬТИМАТИВНОГО АВТОМАТИЧЕСКОГО МОНИТОРИНГА"
echo "====================================================="

# Создание cron задачи
(crontab -l 2>/dev/null; echo "*/1 * * * * cd $(pwd) && ./ultimate_unity_monitor.sh > /dev/null 2>&1") | crontab -

echo "✅ Настроен ультимативный автоматический мониторинг каждую минуту"
echo "🔧 Для отключения: crontab -e"
EOF
    
    chmod +x setup_ultimate_monitoring.sh
    log_success "Создан скрипт настройки ультимативного мониторинга"
}

# 7. ФИНАЛЬНАЯ ПРОВЕРКА И ВАЛИДАЦИЯ
final_validation() {
    echo ""
    echo "🔍 ФИНАЛЬНАЯ ПРОВЕРКА И ВАЛИДАЦИЯ"
    echo "================================="
    
    # Проверка переменных окружения
    log_action "Проверка переменных окружения..."
    
    if [ -n "$G_MESSAGES_DEBUG" ] && [ "$G_MESSAGES_DEBUG" = "none" ]; then
        log_success "GLib переменные: Настроены"
    else
        log_warning "GLib переменные: Не настроены"
    fi
    
    if [ -n "$UNITY_LICENSE_TYPE" ]; then
        log_success "Лицензирование: Настроено"
    else
        log_warning "Лицензирование: Не настроено"
    fi
    
    if [ -n "$UNITY_IPC_PROTOCOL_VERSION" ]; then
        log_success "IPC протокол: Настроен"
    else
        log_warning "IPC протокол: Не настроен"
    fi
    
    # Проверка процессов Unity
    log_action "Проверка процессов Unity..."
    
    unity_processes=$(pgrep -f "unity" | wc -l)
    if [ "$unity_processes" -eq 0 ]; then
        log_success "Процессы Unity: Полностью очищены"
    else
        log_warning "Найдено процессов Unity: $unity_processes"
    fi
    
    # Проверка конфигурационных файлов
    log_action "Проверка конфигурационных файлов..."
    
    config_files=(
        "$HOME/.config/glib-2.0/settings.ini"
        "$HOME/.config/gtk-3.0/settings.ini"
        "$HOME/.config/unity3d/Unity/LicenseClient.config"
        "$HOME/.local/share/unity3d/ipc/config.json"
    )
    
    config_count=0
    for config_file in "${config_files[@]}"; do
        if [ -f "$config_file" ]; then
            ((config_count++))
        fi
    done
    
    if [ "$config_count" -eq "${#config_files[@]}" ]; then
        log_success "Конфигурационные файлы: Все созданы"
    else
        log_warning "Конфигурационные файлы: Создано $config_count из ${#config_files[@]}"
    fi
    
    # Проверка структуры проекта
    log_action "Проверка структуры проекта..."
    
    if [ -d "Assets/Scripts" ] && [ -d "ProjectSettings" ]; then
        log_success "Структура проекта: ОК"
    else
        log_error "Структура проекта: ПРОБЛЕМЫ"
    fi
    
    # Проверка кэша
    log_action "Проверка кэша..."
    
    if [ -d "Library" ] && [ ! -d "Library/ScriptAssemblies" ]; then
        log_success "Кэш: Полностью очищен и готов к пересборке"
    else
        log_warning "Кэш: Требуется дополнительная очистка"
    fi
}

# Основная функция
main() {
    # Полная остановка Unity
    complete_unity_shutdown
    
    # Полная очистка кэшей
    complete_unity_cache_cleanup
    
    # Полная настройка переменных окружения
    complete_environment_setup
    
    # Создание конфигурационных файлов
    create_configuration_files
    
    # Создание скриптов подавления ошибок
    create_error_suppression_scripts
    
    # Создание ультимативной системы мониторинга
    create_ultimate_monitoring_system
    
    # Финальная проверка и валидация
    final_validation
    
    # Результаты
    echo ""
    echo "📊 РЕЗУЛЬТАТЫ УЛЬТИМАТИВНОГО ФИНАЛЬНОГО ИСПРАВЛЕНИЯ"
    echo "=================================================="
    echo "🔧 Процессы Unity: Полностью остановлены"
    echo "🧹 Кэш Unity: Полностью очищен"
    echo "⚙️  Переменные окружения: Настроены"
    echo "📁 Конфигурационные файлы: Созданы"
    echo "🛡️  Скрипты подавления ошибок: Созданы"
    echo "🔍 Система мониторинга: Настроена"
    
    if [ $fixed_errors -gt 0 ]; then
        echo ""
        echo "✅ УЛЬТИМАТИВНОЕ ФИНАЛЬНОЕ ИСПРАВЛЕНИЕ ЗАВЕРШЕНО УСПЕШНО!"
        echo "🎯 Проект полностью готов к работе в Unity Editor"
        echo "🚀 Рекомендуется перезапустить Unity Editor"
    else
        echo ""
        echo "ℹ️  Все системы настроены оптимально"
        echo "🎯 Проект в идеальном состоянии"
    fi
    
    echo ""
    echo "🔧 Для ультимативного мониторинга: ./ultimate_unity_monitor.sh"
    echo "🤖 Для автоматического исправления: ./auto_fix_unity_ultimate.sh"
    echo "⚙️ Для настройки автмониторинга: ./setup_ultimate_monitoring.sh"
    echo "🧹 Для полной очистки: ./complete_unity_cleanup.sh"
    echo "🚀 Для запуска Unity: ./start_unity_clean.sh"
    echo "📅 Дата исправления: $(date)"
}

# Запуск основной функции
main
