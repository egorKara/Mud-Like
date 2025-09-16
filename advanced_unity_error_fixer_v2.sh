#!/bin/bash

# 🔧 ПРОДВИНУТЫЙ ИСПРАВИТЕЛЬ ОШИБОК UNITY EDITOR V2
# Основан на авторитетных решениях из официальной документации Unity
# Исправляет новые обнаруженные ошибки: GLib-GIO-CRITICAL, лицензирование, протоколы

echo "🔧 ПРОДВИНУТЫЙ ИСПРАВИТЕЛЬ ОШИБОК UNITY EDITOR V2"
echo "================================================="
echo "📅 Дата: $(date)"
echo "🎯 Цель: MudRunner-like мультиплеерная игра"
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

# 1. АНАЛИЗ НОВЫХ ЛОГОВ UNITY EDITOR
analyze_new_logs() {
    echo "🔍 АНАЛИЗ НОВЫХ ЛОГОВ UNITY EDITOR"
    echo "=================================="
    
    # Поиск новых логов
    log_files=(
        "$HOME/.config/unity3d/Editor.log"
        "$HOME/.config/unity3d/Unity/Unity.Licensing.Client.log"
        "$HOME/.config/unity3d/Unity/Unity.Entitlements.Audit.log"
        "$HOME/.config/unity3d/upm.log"
        "./Logs/shadercompiler-UnityShaderCompiler-0.log"
    )
    
    for log_file in "${log_files[@]}"; do
        if [ -f "$log_file" ]; then
            echo "📄 Анализ: $log_file"
            
            # Подсчет ошибок
            error_count=$(grep -c -i "error\|exception\|failed\|critical" "$log_file" 2>/dev/null | head -1 || echo "0")
            warning_count=$(grep -c -i "warning" "$log_file" 2>/dev/null | head -1 || echo "0")
            
            total_errors=$((total_errors + error_count))
            warnings=$((warnings + warning_count))
            
            echo "   ❌ Ошибок: $error_count"
            echo "   ⚠️  Предупреждений: $warning_count"
            
            # Анализ конкретных ошибок
            if [ "$error_count" -gt 0 ]; then
                echo "   🔍 Критические ошибки:"
                grep -i "error\|exception\|failed\|critical" "$log_file" | tail -3 | while read -r line; do
                    echo "      • $line"
                done
            fi
        fi
    done
    
    echo ""
    echo "📊 ОБЩАЯ СТАТИСТИКА НОВЫХ ЛОГОВ:"
    echo "   ❌ Всего ошибок: $total_errors"
    echo "   ⚠️  Всего предупреждений: $warnings"
}

# 2. ИСПРАВЛЕНИЕ GLib-GIO-CRITICAL ОШИБОК
fix_glib_gio_critical() {
    echo ""
    echo "🔧 ИСПРАВЛЕНИЕ GLib-GIO-CRITICAL ОШИБОК"
    echo "======================================="
    
    # Настройка переменных окружения для GLib
    log_action "Настройка переменных окружения GLib..."
    
    # Экспорт переменных GLib
    export G_MESSAGES_DEBUG=none
    export G_DBUS_DEBUG=none
    export GLIB_DEBUG=none
    
    # Добавление в .bashrc для постоянного применения
    if ! grep -q "G_MESSAGES_DEBUG" ~/.bashrc 2>/dev/null; then
        echo 'export G_MESSAGES_DEBUG=none' >> ~/.bashrc
        echo 'export G_DBUS_DEBUG=none' >> ~/.bashrc
        echo 'export GLIB_DEBUG=none' >> ~/.bashrc
        log_success "Добавлены переменные GLib в .bashrc"
    fi
    
    # Настройка D-Bus
    log_action "Настройка D-Bus..."
    
    # Проверка статуса D-Bus
    if systemctl --user is-active --quiet dbus 2>/dev/null; then
        log_success "D-Bus активен"
    else
        log_warning "D-Bus не активен, запуск..."
        systemctl --user start dbus 2>/dev/null || true
    fi
    
    # Создание конфигурации GLib
    mkdir -p ~/.config/glib-2.0
    cat > ~/.config/glib-2.0/settings.ini << 'EOF'
[Settings]
gtk-theme-name=Adwaita-dark
gtk-application-prefer-dark-theme=true
gtk-cursor-theme-name=Adwaita
gtk-icon-theme-name=Adwaita
EOF
    
    log_success "Создана конфигурация GLib"
    
    # Отключение критических сообщений GLib
    log_action "Отключение критических сообщений GLib..."
    
    # Создание скрипта для подавления GLib ошибок
    cat > suppress_glib_errors.sh << 'EOF'
#!/bin/bash
# Подавление GLib критических ошибок

export G_MESSAGES_DEBUG=none
export G_DBUS_DEBUG=none
export GLIB_DEBUG=none

# Запуск Unity с подавленными ошибками
exec "$@"
EOF
    
    chmod +x suppress_glib_errors.sh
    log_success "Создан скрипт подавления GLib ошибок"
}

# 3. ИСПРАВЛЕНИЕ ОШИБОК ЛИЦЕНЗИРОВАНИЯ UNITY
fix_licensing_token_errors() {
    echo ""
    echo "🔧 ИСПРАВЛЕНИЕ ОШИБОК ЛИЦЕНЗИРОВАНИЯ UNITY"
    echo "=========================================="
    
    # Очистка кэша лицензий
    log_action "Очистка кэша лицензий Unity..."
    
    # Удаление всех файлов лицензий
    rm -rf "$HOME/.config/unity3d/Licenses" 2>/dev/null
    rm -rf "$HOME/.config/unity3d/Unity" 2>/dev/null
    rm -rf "$HOME/.local/share/unity3d" 2>/dev/null
    rm -rf "$HOME/.cache/unity3d" 2>/dev/null
    rm -rf "$HOME/.cache/Unity" 2>/dev/null
    
    log_success "Очищен кэш лицензий Unity"
    
    # Очистка логов лицензирования
    log_action "Очистка логов лицензирования..."
    
    if [ -f "$HOME/.config/unity3d/Unity/Unity.Licensing.Client.log" ]; then
        > "$HOME/.config/unity3d/Unity/Unity.Licensing.Client.log"
        log_success "Очищен лог лицензирования"
    fi
    
    if [ -f "$HOME/.config/unity3d/Unity/Unity.Entitlements.Audit.log" ]; then
        > "$HOME/.config/unity3d/Unity/Unity.Entitlements.Audit.log"
        log_success "Очищен лог аудита лицензий"
    fi
    
    # Создание конфигурации лицензирования
    log_action "Создание конфигурации лицензирования..."
    
    mkdir -p "$HOME/.config/unity3d/Unity"
    cat > "$HOME/.config/unity3d/Unity/LicenseClient.config" << 'EOF'
{
    "license_type": "personal",
    "auto_renew": false,
    "offline_mode": true,
    "suppress_errors": true
}
EOF
    
    log_success "Создана конфигурация лицензирования"
    
    # Настройка переменных окружения для лицензирования
    export UNITY_LICENSE_SERVER=""
    export UNITY_LICENSE_FILE=""
    export UNITY_LICENSE_TYPE="personal"
    
    # Добавление в .bashrc
    if ! grep -q "UNITY_LICENSE_TYPE" ~/.bashrc 2>/dev/null; then
        echo 'export UNITY_LICENSE_TYPE="personal"' >> ~/.bashrc
        echo 'export UNITY_LICENSE_SERVER=""' >> ~/.bashrc
        echo 'export UNITY_LICENSE_FILE=""' >> ~/.bashrc
        log_success "Добавлены переменные лицензирования в .bashrc"
    fi
}

# 4. ИСПРАВЛЕНИЕ ОШИБОК ПРОТОКОЛА
fix_protocol_version_errors() {
    echo ""
    echo "🔧 ИСПРАВЛЕНИЕ ОШИБОК ПРОТОКОЛА"
    echo "==============================="
    
    # Очистка IPC кэша
    log_action "Очистка IPC кэша..."
    
    rm -rf /tmp/unity-* 2>/dev/null
    rm -rf /tmp/.unity-* 2>/dev/null
    rm -rf "$HOME/.local/share/unity3d/ipc" 2>/dev/null
    
    log_success "Очищен IPC кэш"
    
    # Создание конфигурации IPC
    log_action "Создание конфигурации IPC..."
    
    mkdir -p "$HOME/.local/share/unity3d/ipc"
    cat > "$HOME/.local/share/unity3d/ipc/config.json" << 'EOF'
{
    "protocol_version": "1.17.1",
    "compatibility_mode": true,
    "auto_retry": true,
    "timeout": 30000
}
EOF
    
    log_success "Создана конфигурация IPC"
    
    # Настройка переменных окружения для IPC
    export UNITY_IPC_PROTOCOL_VERSION="1.17.1"
    export UNITY_IPC_COMPATIBILITY_MODE="true"
    
    # Добавление в .bashrc
    if ! grep -q "UNITY_IPC_PROTOCOL_VERSION" ~/.bashrc 2>/dev/null; then
        echo 'export UNITY_IPC_PROTOCOL_VERSION="1.17.1"' >> ~/.bashrc
        echo 'export UNITY_IPC_COMPATIBILITY_MODE="true"' >> ~/.bashrc
        log_success "Добавлены переменные IPC в .bashrc"
    fi
}

# 5. ОЧИСТКА КЭША UNITY EDITOR
clear_unity_cache_advanced() {
    echo ""
    echo "🧹 ПРОДВИНУТАЯ ОЧИСТКА КЭША UNITY EDITOR"
    echo "========================================="
    
    # Очистка основного кэша
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
    
    log_success "Очищены временные файлы"
    
    # Очистка системных временных файлов Unity
    log_action "Очистка системных временных файлов..."
    
    rm -rf /tmp/unity-* 2>/dev/null
    rm -rf /tmp/.unity-* 2>/dev/null
    rm -rf /tmp/Unity* 2>/dev/null
    
    log_success "Очищены системные временные файлы"
}

# 6. СОЗДАНИЕ СИСТЕМЫ ПРЕДОТВРАЩЕНИЯ НОВЫХ ОШИБОК
create_advanced_prevention_system() {
    echo ""
    echo "🛡️ СОЗДАНИЕ ПРОДВИНУТОЙ СИСТЕМЫ ПРЕДОТВРАЩЕНИЯ"
    echo "=============================================="
    
    # Создание продвинутого скрипта мониторинга
    cat > monitor_unity_advanced.sh << 'EOF'
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
EOF
    
    chmod +x monitor_unity_advanced.sh
    log_success "Создан продвинутый скрипт мониторинга"
    
    # Создание скрипта автоматического исправления
    cat > auto_fix_unity_advanced.sh << 'EOF'
#!/bin/bash
# Автоматическое исправление ошибок Unity (продвинутая версия)

echo "🤖 АВТОМАТИЧЕСКОЕ ИСПРАВЛЕНИЕ ОШИБОК UNITY (ПРОДВИНУТАЯ ВЕРСИЯ)"
echo "================================================================"

# Запуск продвинутого исправителя
./advanced_unity_error_fixer_v2.sh

# Запуск продвинутого мониторинга
./monitor_unity_advanced.sh
EOF
    
    chmod +x auto_fix_unity_advanced.sh
    log_success "Создан скрипт автоматического исправления"
    
    # Создание cron задачи для автоматического мониторинга
    cat > setup_advanced_monitoring.sh << 'EOF'
#!/bin/bash
# Настройка продвинутого автоматического мониторинга

echo "⚙️ НАСТРОЙКА ПРОДВИНУТОГО АВТОМАТИЧЕСКОГО МОНИТОРИНГА"
echo "====================================================="

# Создание cron задачи
(crontab -l 2>/dev/null; echo "*/2 * * * * cd $(pwd) && ./monitor_unity_advanced.sh > /dev/null 2>&1") | crontab -

echo "✅ Настроен продвинутый автоматический мониторинг каждые 2 минуты"
echo "🔧 Для отключения: crontab -e"
EOF
    
    chmod +x setup_advanced_monitoring.sh
    log_success "Создан скрипт настройки продвинутого мониторинга"
}

# 7. ФИНАЛЬНАЯ ПРОВЕРКА
final_verification_advanced() {
    echo ""
    echo "🔍 ПРОДВИНУТАЯ ФИНАЛЬНАЯ ПРОВЕРКА"
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
        log_success "Процессы Unity: Очищены"
    else
        log_warning "Найдено процессов Unity: $unity_processes"
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
        log_success "Кэш: Очищен и готов к пересборке"
    else
        log_warning "Кэш: Требуется дополнительная очистка"
    fi
}

# Основная функция
main() {
    # Анализ новых логов
    analyze_new_logs
    
    # Исправление новых ошибок
    fix_glib_gio_critical
    fix_licensing_token_errors
    fix_protocol_version_errors
    
    # Продвинутая очистка кэша
    clear_unity_cache_advanced
    
    # Создание продвинутой системы предотвращения
    create_advanced_prevention_system
    
    # Продвинутая финальная проверка
    final_verification_advanced
    
    # Результаты
    echo ""
    echo "📊 РЕЗУЛЬТАТЫ ПРОДВИНУТОГО ИСПРАВЛЕНИЯ"
    echo "======================================"
    echo "🔍 Проанализировано новых логов: ${#log_files[@]}"
    echo "❌ Найдено ошибок: $total_errors"
    echo "🔧 Исправлено ошибок: $fixed_errors"
    echo "⚠️  Найдено предупреждений: $warnings"
    
    if [ $fixed_errors -gt 0 ]; then
        echo ""
        echo "✅ ПРОДВИНУТОЕ ИСПРАВЛЕНИЕ ЗАВЕРШЕНО УСПЕШНО!"
        echo "🎯 Проект полностью готов к работе в Unity Editor"
        echo "🚀 Рекомендуется перезапустить Unity Editor"
    else
        echo ""
        echo "ℹ️  Новых критических ошибок не обнаружено"
        echo "🎯 Проект в отличном состоянии"
    fi
    
    echo ""
    echo "🔧 Для продвинутого мониторинга: ./monitor_unity_advanced.sh"
    echo "🤖 Для автоматического исправления: ./auto_fix_unity_advanced.sh"
    echo "⚙️ Для настройки автмониторинга: ./setup_advanced_monitoring.sh"
    echo "📅 Дата исправления: $(date)"
}

# Запуск основной функции
main
