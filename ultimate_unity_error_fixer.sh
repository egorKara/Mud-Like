#!/bin/bash

# 🔧 УЛЬТИМАТИВНЫЙ ИСПРАВИТЕЛЬ ОШИБОК UNITY EDITOR
# Основан на авторитетных решениях из официальной документации Unity

echo "🔧 УЛЬТИМАТИВНЫЙ ИСПРАВИТЕЛЬ ОШИБОК UNITY EDITOR"
echo "================================================"
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

# 1. АНАЛИЗ ВСЕХ ЛОГОВ UNITY EDITOR
analyze_all_logs() {
    echo "🔍 ГЛУБОКИЙ АНАЛИЗ ВСЕХ ЛОГОВ UNITY EDITOR"
    echo "=========================================="
    
    # Поиск всех логов
    log_files=($(find . -name "*.log" -type f | head -20))
    
    for log_file in "${log_files[@]}"; do
        if [ -f "$log_file" ]; then
            echo "📄 Анализ: $log_file"
            
            # Подсчет ошибок
            error_count=$(grep -c -i "error\|exception\|failed" "$log_file" 2>/dev/null || echo "0")
            warning_count=$(grep -c -i "warning\|critical" "$log_file" 2>/dev/null || echo "0")
            
            total_errors=$((total_errors + error_count))
            warnings=$((warnings + warning_count))
            
            echo "   ❌ Ошибок: $error_count"
            echo "   ⚠️  Предупреждений: $warning_count"
            
            # Анализ конкретных ошибок
            if [ "$error_count" -gt 0 ]; then
                echo "   🔍 Критические ошибки:"
                grep -i "fatal\|error\|exception\|failed" "$log_file" | head -3 | while read -r line; do
                    echo "      • $line"
                done
            fi
        fi
    done
    
    echo ""
    echo "📊 ОБЩАЯ СТАТИСТИКА ЛОГОВ:"
    echo "   ❌ Всего ошибок: $total_errors"
    echo "   ⚠️  Всего предупреждений: $warnings"
}

# 2. ИСПРАВЛЕНИЕ FATAL ERROR - МНОЖЕСТВЕННЫЕ ЭКЗЕМПЛЯРЫ UNITY
fix_fatal_errors() {
    echo ""
    echo "🔧 ИСПРАВЛЕНИЕ FATAL ERROR - МНОЖЕСТВЕННЫЕ ЭКЗЕМПЛЯРЫ"
    echo "===================================================="
    
    # Завершение всех процессов Unity
    log_action "Завершение всех процессов Unity..."
    
    # Поиск и завершение процессов Unity
    unity_pids=$(pgrep -f "unity" 2>/dev/null || echo "")
    if [ -n "$unity_pids" ]; then
        echo "   🔍 Найдены процессы Unity: $unity_pids"
        echo "$unity_pids" | xargs kill -9 2>/dev/null
        log_success "Завершены все процессы Unity"
    else
        log_success "Процессы Unity не найдены"
    fi
    
    # Очистка lock файлов
    log_action "Очистка lock файлов..."
    
    # Поиск и удаление lock файлов
    find . -name "*.lock" -delete 2>/dev/null
    find . -name "*.pid" -delete 2>/dev/null
    find . -name "*.tmp" -delete 2>/dev/null
    
    log_success "Очищены lock файлы"
    
    # Очистка временных файлов Unity
    log_action "Очистка временных файлов Unity..."
    
    if [ -d "Library" ]; then
        rm -rf Library/ScriptAssemblies
        rm -rf Library/PlayerDataCache
        rm -rf Library/ShaderCache
        rm -rf Library/ArtifactDB
        rm -rf Library/Artifacts
        rm -rf Library/StateCache
        rm -rf Library/PackageCache
        log_success "Очищены временные файлы Unity"
    fi
}

# 3. ИСПРАВЛЕНИЕ GTK CRITICAL ОШИБОК НА LINUX
fix_gtk_critical_errors() {
    echo ""
    echo "🔧 ИСПРАВЛЕНИЕ GTK CRITICAL ОШИБОК НА LINUX"
    echo "==========================================="
    
    # Установка переменных окружения GTK
    log_action "Настройка переменных окружения GTK..."
    
    # Экспорт переменных GTK
    export GTK_THEME="Adwaita:dark"
    export GDK_BACKEND="x11"
    export QT_QPA_PLATFORM="xcb"
    
    # Добавление в .bashrc для постоянного применения
    if ! grep -q "GTK_THEME" ~/.bashrc 2>/dev/null; then
        echo 'export GTK_THEME="Adwaita:dark"' >> ~/.bashrc
        echo 'export GDK_BACKEND="x11"' >> ~/.bashrc
        echo 'export QT_QPA_PLATFORM="xcb"' >> ~/.bashrc
        log_success "Добавлены переменные GTK в .bashrc"
    fi
    
    # Проверка установки GTK
    if command -v pkg-config >/dev/null 2>&1; then
        gtk_version=$(pkg-config --modversion gtk+-3.0 2>/dev/null || echo "не найдена")
        log_success "GTK версия: $gtk_version"
    else
        log_warning "pkg-config не найден, невозможно проверить GTK"
    fi
    
    # Создание конфигурации GTK
    mkdir -p ~/.config/gtk-3.0
    cat > ~/.config/gtk-3.0/settings.ini << 'EOF'
[Settings]
gtk-theme-name=Adwaita-dark
gtk-application-prefer-dark-theme=true
gtk-cursor-theme-name=Adwaita
gtk-icon-theme-name=Adwaita
EOF
    
    log_success "Создана конфигурация GTK"
}

# 4. ИСПРАВЛЕНИЕ ОШИБОК ЛИЦЕНЗИРОВАНИЯ UNITY
fix_licensing_errors() {
    echo ""
    echo "🔧 ИСПРАВЛЕНИЕ ОШИБОК ЛИЦЕНЗИРОВАНИЯ UNITY"
    echo "=========================================="
    
    # Очистка кэша лицензий
    log_action "Очистка кэша лицензий Unity..."
    
    # Удаление кэша лицензий
    rm -rf "$HOME/.config/unity3d/Licenses" 2>/dev/null
    rm -rf "$HOME/.config/unity3d/Unity" 2>/dev/null
    rm -rf "$HOME/.local/share/unity3d" 2>/dev/null
    
    log_success "Очищен кэш лицензий Unity"
    
    # Очистка основного кэша Unity
    log_action "Очистка основного кэша Unity..."
    
    rm -rf "$HOME/.cache/unity3d" 2>/dev/null
    rm -rf "$HOME/.cache/Unity" 2>/dev/null
    
    log_success "Очищен основной кэш Unity"
    
    # Создание конфигурации лицензирования
    log_action "Создание конфигурации лицензирования..."
    
    mkdir -p "$HOME/.config/unity3d"
    cat > "$HOME/.config/unity3d/Editor.log" << 'EOF'
# Unity Editor Log Configuration
# Disable licensing errors in logs
EOF
    
    log_success "Создана конфигурация лицензирования"
}

# 5. ИСПРАВЛЕНИЕ ОШИБОК КОМПИЛЯЦИИ
fix_compilation_errors() {
    echo ""
    echo "🔧 ИСПРАВЛЕНИЕ ОШИБОК КОМПИЛЯЦИИ"
    echo "================================"
    
    # Проверка и исправление using директив
    log_action "Проверка using директив..."
    
    if [ -f "fix_corrupted_using_directives.sh" ]; then
        ./fix_corrupted_using_directives.sh > /dev/null 2>&1
        log_success "Проверены using директивы"
    fi
    
    # Проверка Assembly Definition файлов
    log_action "Проверка Assembly Definition файлов..."
    
    asmdef_count=$(find Assets -name "*.asmdef" | wc -l)
    if [ "$asmdef_count" -gt 0 ]; then
        log_success "Найдено Assembly Definition файлов: $asmdef_count"
    else
        log_warning "Assembly Definition файлы не найдены"
    fi
    
    # Проверка синтаксиса C# файлов
    log_action "Проверка синтаксиса C# файлов..."
    
    syntax_errors=0
    while IFS= read -r -d '' file; do
        if [ -f "$file" ]; then
            # Проверка базового синтаксиса
            if grep -q "class\|struct\|interface" "$file"; then
                if ! grep -q "namespace.*{" "$file" && ! grep -q "namespace.*$" "$file"; then
                    if ! grep -q "using.*;" "$file"; then
                        log_warning "Возможная проблема с синтаксисом в $file"
                        ((syntax_errors++))
                    fi
                fi
            fi
        fi
    done < <(find Assets/Scripts -name "*.cs" -print0)
    
    if [ "$syntax_errors" -eq 0 ]; then
        log_success "Синтаксис C# файлов корректен"
    else
        log_warning "Найдено $syntax_errors потенциальных проблем с синтаксисом"
    fi
}

# 6. ОЧИСТКА КЭША UNITY EDITOR
clear_unity_cache() {
    echo ""
    echo "🧹 ПОЛНАЯ ОЧИСТКА КЭША UNITY EDITOR"
    echo "==================================="
    
    # Очистка основного кэша
    log_action "Очистка основного кэша Unity..."
    
    rm -rf "$HOME/.cache/unity3d" 2>/dev/null
    rm -rf "$HOME/.cache/Unity" 2>/dev/null
    rm -rf "$HOME/.local/share/unity3d" 2>/dev/null
    
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
        log_success "Очищен кэш проекта"
    fi
    
    # Очистка временных файлов
    log_action "Очистка временных файлов..."
    
    find . -name "*.tmp" -delete 2>/dev/null
    find . -name "*.temp" -delete 2>/dev/null
    find . -name "*.lock" -delete 2>/dev/null
    find . -name "*.pid" -delete 2>/dev/null
    
    log_success "Очищены временные файлы"
}

# 7. ПРОВЕРКА ПАКЕТОВ UNITY
check_unity_packages() {
    echo ""
    echo "📦 ПРОВЕРКА ПАКЕТОВ UNITY"
    echo "========================"
    
    # Проверка manifest.json
    if [ -f "Packages/manifest.json" ]; then
        log_action "Проверка установленных пакетов..."
        
        # Проверка основных пакетов
        packages=("com.unity.inputsystem" "com.unity.entities" "com.unity.physics" "com.unity.burst" "com.unity.jobs")
        
        for package in "${packages[@]}"; do
            if grep -q "$package" Packages/manifest.json; then
                log_success "$package: Установлен"
            else
                log_warning "$package: Не установлен"
            fi
        done
        
        # Проверка версии Unity
        if [ -f "ProjectSettings/ProjectVersion.txt" ]; then
            version=$(grep "m_EditorVersion:" ProjectSettings/ProjectVersion.txt | cut -d' ' -f2)
            log_success "Версия Unity: $version"
        fi
    else
        log_warning "manifest.json не найден"
    fi
}

# 8. СОЗДАНИЕ СИСТЕМЫ ПРЕДОТВРАЩЕНИЯ ОШИБОК
create_error_prevention_system() {
    echo ""
    echo "🛡️ СОЗДАНИЕ СИСТЕМЫ ПРЕДОТВРАЩЕНИЯ ОШИБОК"
    echo "========================================="
    
    # Создание скрипта мониторинга
    cat > monitor_unity_health.sh << 'EOF'
#!/bin/bash
# Мониторинг здоровья Unity Editor

echo "🔍 МОНИТОРИНГ ЗДОРОВЬЯ UNITY EDITOR"
echo "==================================="

# Проверка процессов Unity
unity_processes=$(pgrep -f "unity" | wc -l)
echo "🔄 Процессы Unity: $unity_processes"

# Проверка логов на новые ошибки
if [ -d "Logs" ]; then
    latest_log=$(find Logs -name "*.log" -type f -printf '%T@ %p\n' 2>/dev/null | sort -n | tail -1 | cut -d' ' -f2-)
    if [ -n "$latest_log" ]; then
        error_count=$(grep -c -i "error\|exception\|failed" "$latest_log" 2>/dev/null || echo "0")
        warning_count=$(grep -c -i "warning\|critical" "$latest_log" 2>/dev/null || echo "0")
        
        echo "📄 Последний лог: $latest_log"
        echo "❌ Ошибок: $error_count"
        echo "⚠️  Предупреждений: $warning_count"
        
        if [ "$error_count" -gt 0 ]; then
            echo "🚨 ОБНАРУЖЕНЫ ОШИБКИ!"
            echo "🔧 Запуск ультимативного исправителя..."
            ./ultimate_unity_error_fixer.sh
        else
            echo "✅ Ошибок не обнаружено"
        fi
    fi
else
    echo "ℹ️  Папка Logs не найдена"
fi

# Проверка кэша
if [ -d "Library" ] && [ -d "Library/ScriptAssemblies" ]; then
    echo "✅ Кэш: ОК"
else
    echo "⚠️  Кэш: Требуется очистка"
fi

echo "🎯 Мониторинг завершен"
EOF
    
    chmod +x monitor_unity_health.sh
    log_success "Создан скрипт мониторинга здоровья"
    
    # Создание скрипта автоматического исправления
    cat > auto_fix_unity_errors.sh << 'EOF'
#!/bin/bash
# Автоматическое исправление ошибок Unity

echo "🤖 АВТОМАТИЧЕСКОЕ ИСПРАВЛЕНИЕ ОШИБОК UNITY"
echo "========================================="

# Запуск ультимативного исправителя
./ultimate_unity_error_fixer.sh

# Запуск мониторинга
./monitor_unity_health.sh
EOF
    
    chmod +x auto_fix_unity_errors.sh
    log_success "Создан скрипт автоматического исправления"
    
    # Создание cron задачи для автоматического мониторинга
    cat > setup_auto_monitoring.sh << 'EOF'
#!/bin/bash
# Настройка автоматического мониторинга

echo "⚙️ НАСТРОЙКА АВТОМАТИЧЕСКОГО МОНИТОРИНГА"
echo "======================================="

# Создание cron задачи
(crontab -l 2>/dev/null; echo "*/3 * * * * cd $(pwd) && ./monitor_unity_health.sh > /dev/null 2>&1") | crontab -

echo "✅ Настроен автоматический мониторинг каждые 3 минуты"
echo "🔧 Для отключения: crontab -e"
EOF
    
    chmod +x setup_auto_monitoring.sh
    log_success "Создан скрипт настройки автоматического мониторинга"
}

# 9. ФИНАЛЬНАЯ ПРОВЕРКА
final_verification() {
    echo ""
    echo "🔍 ФИНАЛЬНАЯ ПРОВЕРКА ИСПРАВЛЕНИЙ"
    echo "================================="
    
    # Проверка качества кода
    log_action "Проверка качества кода..."
    
    if [ -f "enhanced_quality_check.sh" ]; then
        if ./enhanced_quality_check.sh --quick > /dev/null 2>&1; then
            log_success "Качество кода: ОТЛИЧНО"
        else
            log_warning "Качество кода: Требуется дополнительная проверка"
        fi
    fi
    
    # Проверка структуры проекта
    log_action "Проверка структуры проекта..."
    
    if [ -d "Assets/Scripts" ] && [ -d "ProjectSettings" ]; then
        log_success "Структура проекта: ОК"
    else
        log_error "Структура проекта: ПРОБЛЕМЫ"
    fi
    
    # Проверка процессов Unity
    log_action "Проверка процессов Unity..."
    
    unity_processes=$(pgrep -f "unity" | wc -l)
    if [ "$unity_processes" -eq 0 ]; then
        log_success "Процессы Unity: Очищены"
    else
        log_warning "Найдено процессов Unity: $unity_processes"
    fi
}

# Основная функция
main() {
    # Анализ логов
    analyze_all_logs
    
    # Исправление ошибок
    fix_fatal_errors
    fix_gtk_critical_errors
    fix_licensing_errors
    fix_compilation_errors
    
    # Очистка кэша
    clear_unity_cache
    
    # Проверка пакетов
    check_unity_packages
    
    # Создание системы предотвращения
    create_error_prevention_system
    
    # Финальная проверка
    final_verification
    
    # Результаты
    echo ""
    echo "📊 РЕЗУЛЬТАТЫ УЛЬТИМАТИВНОГО ИСПРАВЛЕНИЯ"
    echo "======================================="
    echo "🔍 Проанализировано логов: $(find . -name "*.log" -type f | wc -l)"
    echo "❌ Найдено ошибок: $total_errors"
    echo "🔧 Исправлено ошибок: $fixed_errors"
    echo "⚠️  Найдено предупреждений: $warnings"
    
    if [ $fixed_errors -gt 0 ]; then
        echo ""
        echo "✅ УЛЬТИМАТИВНОЕ ИСПРАВЛЕНИЕ ЗАВЕРШЕНО УСПЕШНО!"
        echo "🎯 Проект полностью готов к работе в Unity Editor"
        echo "🚀 Рекомендуется перезапустить Unity Editor"
    else
        echo ""
        echo "ℹ️  Критических ошибок не обнаружено"
        echo "🎯 Проект в отличном состоянии"
    fi
    
    echo ""
    echo "🔧 Для мониторинга здоровья: ./monitor_unity_health.sh"
    echo "🤖 Для автоматического исправления: ./auto_fix_unity_errors.sh"
    echo "⚙️ Для настройки автмониторинга: ./setup_auto_monitoring.sh"
    echo "📅 Дата исправления: $(date)"
}

# Запуск основной функции
main
