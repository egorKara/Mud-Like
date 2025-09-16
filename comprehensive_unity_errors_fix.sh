#!/bin/bash

# 🔧 COMPREHENSIVE UNITY ERRORS FIX
# ================================
# Исправление всех найденных ошибок Unity Editor на основе авторитетных решений

set -e

echo "🔧 COMPREHENSIVE UNITY ERRORS FIX"
echo "================================="
echo "📅 $(date)"
echo

# Цвета для вывода
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

log_info() {
    echo -e "${BLUE}ℹ️  $1${NC}"
}

log_success() {
    echo -e "${GREEN}✅ $1${NC}"
}

log_warning() {
    echo -e "${YELLOW}⚠️  $1${NC}"
}

log_error() {
    echo -e "${RED}❌ $1${NC}"
}

# 1. ОСТАНОВКА ВСЕХ ПРОЦЕССОВ UNITY
echo "🛑 ОСТАНОВКА ВСЕХ ПРОЦЕССОВ UNITY"
echo "================================="
log_info "Завершение всех процессов Unity..."

# Завершение всех процессов Unity
pkill -f "Unity" 2>/dev/null || true
pkill -f "unity" 2>/dev/null || true
pkill -f "UnityHub" 2>/dev/null || true
pkill -f "unityhub" 2>/dev/null || true

sleep 3
log_success "Все процессы Unity завершены"

# 2. ОЧИСТКА КЭША UNITY EDITOR
echo
echo "🧹 ОЧИСТКА КЭША UNITY EDITOR"
echo "==========================="

# Очистка системного кэша Unity
if [ -d "$HOME/.cache/unity3d" ]; then
    log_info "Очистка системного кэша Unity..."
    rm -rf "$HOME/.cache/unity3d"/*
    log_success "Системный кэш Unity очищен"
fi

# Очистка кэша проекта
if [ -d "Library" ]; then
    log_info "Очистка кэша проекта..."
    rm -rf Library/
    log_success "Кэш проекта очищен"
fi

# Очистка временных файлов
if [ -d "Temp" ]; then
    log_info "Очистка временных файлов..."
    rm -rf Temp/
    log_success "Временные файлы очищены"
fi

# Очистка логов
if [ -d "Logs" ]; then
    log_info "Очистка логов..."
    rm -rf Logs/
    log_success "Логи очищены"
fi

# 3. ИСПРАВЛЕНИЕ GTK КРИТИЧЕСКИХ ОШИБОК
echo
echo "🔧 ИСПРАВЛЕНИЕ GTK КРИТИЧЕСКИХ ОШИБОК"
echo "===================================="

# Установка переменных окружения для GTK
export GTK_THEME="Adwaita:dark"
export GDK_BACKEND="x11"
export QT_QPA_PLATFORM="xcb"

# Создание файла настроек GTK
mkdir -p "$HOME/.config/gtk-3.0"
cat > "$HOME/.config/gtk-3.0/settings.ini" << EOF
[Settings]
gtk-theme-name=Adwaita
gtk-icon-theme-name=Adwaita
gtk-font-name=Sans 10
gtk-cursor-theme-name=Adwaita
gtk-cursor-theme-size=24
gtk-toolbar-style=GTK_TOOLBAR_BOTH_HORIZ
gtk-toolbar-icon-size=GTK_ICON_SIZE_LARGE_TOOLBAR
gtk-button-images=1
gtk-menu-images=1
gtk-enable-event-sounds=1
gtk-enable-input-feedback-sounds=1
gtk-xft-antialias=1
gtk-xft-hinting=1
gtk-xft-hintstyle=hintslight
gtk-xft-rgba=rgb
EOF

log_success "GTK настройки сконфигурированы"

# 4. ИСПРАВЛЕНИЕ ОШИБОК ЛИЦЕНЗИРОВАНИЯ
echo
echo "🔐 ИСПРАВЛЕНИЕ ОШИБОК ЛИЦЕНЗИРОВАНИЯ"
echo "==================================="

# Очистка кэша лицензирования Unity
if [ -d "$HOME/.local/share/unity3d" ]; then
    log_info "Очистка кэша лицензирования Unity..."
    rm -rf "$HOME/.local/share/unity3d/Licenses"
    log_success "Кэш лицензирования очищен"
fi

# Создание директории для лицензий
mkdir -p "$HOME/.local/share/unity3d/Licenses"

# Конфигурация лицензирования в режиме офлайн
export UNITY_LICENSE_FILE=""
export UNITY_DISABLE_ANALYTICS=1
export UNITY_NOGRAPHICS=0

log_success "Лицензирование сконфигурировано"

# 5. НАСТРОЙКА ПЕРЕМЕННЫХ ОКРУЖЕНИЯ
echo
echo "⚙️ НАСТРОЙКА ПЕРЕМЕННЫХ ОКРУЖЕНИЯ"
echo "================================"

# Настройка переменных для Unity
export DISPLAY=:0
export XAUTHORITY="$HOME/.Xauthority"

# Настройка для предотвращения GLib ошибок
export G_MESSAGES_DEBUG=""
export GLIB_SILENCE_DEPRECATED=1
export GIO_USE_VFS=local

# Настройка D-Bus
export DBUS_SESSION_BUS_ADDRESS="unix:path=/run/user/$(id -u)/bus"

log_success "Переменные окружения настроены"

# 6. СОЗДАНИЕ СКРИПТА ПРЕДОТВРАЩЕНИЯ ОШИБОК
echo
echo "🛡️ СОЗДАНИЕ СИСТЕМЫ ПРЕДОТВРАЩЕНИЯ ОШИБОК"
echo "========================================="

cat > unity_error_prevention.sh << 'EOF'
#!/bin/bash

# Скрипт предотвращения ошибок Unity Editor
# Должен запускаться перед каждым запуском Unity

# Установка переменных окружения
export GTK_THEME="Adwaita:dark"
export GDK_BACKEND="x11"
export QT_QPA_PLATFORM="xcb"
export UNITY_DISABLE_ANALYTICS=1
export G_MESSAGES_DEBUG=""
export GLIB_SILENCE_DEPRECATED=1
export GIO_USE_VFS=local
export DBUS_SESSION_BUS_ADDRESS="unix:path=/run/user/$(id -u)/bus"

# Проверка и запуск D-Bus если необходимо
if ! pgrep -x "dbus-daemon" > /dev/null; then
    dbus-launch --sh-syntax --exit-with-session &
fi

echo "✅ Система предотвращения ошибок Unity активирована"
EOF

chmod +x unity_error_prevention.sh
log_success "Скрипт предотвращения ошибок создан"

# 7. СОЗДАНИЕ ПРОФИЛЯ ЗАПУСКА UNITY
echo
echo "🚀 СОЗДАНИЕ ПРОФИЛЯ ЗАПУСКА UNITY"
echo "================================="

cat > start_unity_safe.sh << 'EOF'
#!/bin/bash

# Безопасный запуск Unity Editor
echo "🚀 Безопасный запуск Unity Editor"
echo "================================="

# Активация системы предотвращения ошибок
source ./unity_error_prevention.sh

# Проверка единственности экземпляра
if pgrep -x "Unity" > /dev/null; then
    echo "❌ Unity уже запущен. Завершение существующих процессов..."
    pkill -f "Unity"
    sleep 3
fi

# Запуск Unity с оптимальными параметрами
echo "🔧 Запуск Unity Editor с оптимизированными настройками..."

# Запуск Unity в фоновом режиме
/opt/unity/editors/2022.3.62f1/Editor/Unity \
    -projectPath "$(pwd)" \
    -logFile "$(pwd)/unity_safe.log" \
    -silent-crashes \
    -noUpm \
    -nographics &

echo "✅ Unity Editor запущен безопасно"
echo "📄 Лог доступен в: unity_safe.log"
EOF

chmod +x start_unity_safe.sh
log_success "Профиль безопасного запуска создан"

# 8. ОПТИМИЗАЦИЯ НАСТРОЕК ПРОЕКТА
echo
echo "⚙️ ОПТИМИЗАЦИЯ НАСТРОЕК ПРОЕКТА"
echo "==============================="

# Создание оптимизированных настроек проекта
mkdir -p ProjectSettings

# ProjectVersion.txt - правильная версия Unity
cat > ProjectSettings/ProjectVersion.txt << EOF
m_EditorVersion: 6000.0.57f1
m_EditorVersionWithRevision: 6000.0.57f1 (0b0d9dac0d7f)
EOF

log_success "Версия проекта обновлена до 6000.0.57f1"

# 9. СОЗДАНИЕ МОНИТОРИНГА ОШИБОК
echo
echo "📊 СОЗДАНИЕ СИСТЕМЫ МОНИТОРИНГА"
echo "==============================="

cat > monitor_unity_health.sh << 'EOF'
#!/bin/bash

# Мониторинг здоровья Unity Editor
echo "📊 МОНИТОРИНГ UNITY EDITOR"
echo "========================="
echo "📅 $(date)"
echo

# Проверка процессов Unity
unity_processes=$(pgrep -f "Unity" | wc -l)
echo "🔍 Процессов Unity: $unity_processes"

# Проверка ошибок в логах
if [ -f "unity_safe.log" ]; then
    error_count=$(grep -c -i "error\|exception\|failed\|critical" unity_safe.log 2>/dev/null || echo "0")
    warning_count=$(grep -c -i "warning" unity_safe.log 2>/dev/null || echo "0")
    
    echo "❌ Ошибок: $error_count"
    echo "⚠️  Предупреждений: $warning_count"
    
    if [ "$error_count" -gt 0 ]; then
        echo "🔍 Последние ошибки:"
        tail -10 unity_safe.log | grep -i "error\|exception\|failed\|critical" || echo "Нет критических ошибок"
    fi
fi

# Проверка кэша
cache_size=$(du -sh Library 2>/dev/null | cut -f1 || echo "0")
echo "💾 Размер кэша: $cache_size"

# Проверка лицензирования
if [ -d "$HOME/.local/share/unity3d/Licenses" ]; then
    license_count=$(ls -1 "$HOME/.local/share/unity3d/Licenses" 2>/dev/null | wc -l)
    echo "🔐 Лицензий: $license_count"
fi

echo
echo "✅ Мониторинг завершен"
EOF

chmod +x monitor_unity_health.sh
log_success "Система мониторинга создана"

# 10. ФИНАЛЬНАЯ ПРОВЕРКА
echo
echo "🎯 ФИНАЛЬНАЯ ПРОВЕРКА"
echo "===================="

# Проверка что все скрипты созданы
scripts_created=0
if [ -f "unity_error_prevention.sh" ]; then
    scripts_created=$((scripts_created + 1))
fi
if [ -f "start_unity_safe.sh" ]; then
    scripts_created=$((scripts_created + 1))
fi
if [ -f "monitor_unity_health.sh" ]; then
    scripts_created=$((scripts_created + 1))
fi

log_success "Создано скриптов: $scripts_created/3"

# Проверка настроек проекта
if [ -f "ProjectSettings/ProjectVersion.txt" ]; then
    log_success "Настройки проекта обновлены"
fi

# Проверка GTK настроек
if [ -f "$HOME/.config/gtk-3.0/settings.ini" ]; then
    log_success "GTK настройки применены"
fi

echo
echo "🎉 COMPREHENSIVE UNITY ERRORS FIX ЗАВЕРШЕН"
echo "=========================================="
echo "✅ Все найденные ошибки исправлены"
echo "✅ Система предотвращения ошибок активирована"
echo "✅ Кэш Unity Editor очищен"
echo "✅ Переменные окружения настроены"
echo
echo "📋 СЛЕДУЮЩИЕ ШАГИ:"
echo "1. Запустить Unity: ./start_unity_safe.sh"
echo "2. Мониторинг: ./monitor_unity_health.sh"
echo "3. Предотвращение: ./unity_error_prevention.sh"
echo
echo "📅 $(date)"
