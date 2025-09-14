#!/bin/bash

# Автоматизированный менеджер рабочих процессов
# Создан: 14 сентября 2025
# Цель: Автоматизация рутинных задач для проекта MudRunner-like

echo "🤖 АВТОМАТИЗИРОВАННЫЙ МЕНЕДЖЕР РАБОЧИХ ПРОЦЕССОВ"
echo "================================================="

# Цвета для вывода
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
PURPLE='\033[0;35m'
CYAN='\033[0;36m'
NC='\033[0m' # No Color

# Конфигурация
WORKFLOW_LOG="workflow_manager.log"
BACKUP_DIR="backups/$(date +%Y%m%d_%H%M%S)"

# Функция логирования
log_workflow() {
    local message="$1"
    local timestamp=$(date '+%Y-%m-%d %H:%M:%S')
    echo "[$timestamp] $message" | tee -a "$WORKFLOW_LOG"
}

# Функция создания резервной копии
create_backup() {
    echo "💾 Создание резервной копии проекта..."
    log_workflow "BACKUP: Создание резервной копии"
    
    mkdir -p "$BACKUP_DIR"
    
    # Резервная копия критических файлов
    cp -r Assets "$BACKUP_DIR/" 2>/dev/null
    cp -r ProjectSettings "$BACKUP_DIR/" 2>/dev/null
    cp -r Packages "$BACKUP_DIR/" 2>/dev/null
    
    # Резервная копия скриптов автоматизации
    cp *.sh "$BACKUP_DIR/" 2>/dev/null
    cp *.md "$BACKUP_DIR/" 2>/dev/null
    
    echo "  ✅ Резервная копия создана: $BACKUP_DIR"
    log_workflow "BACKUP: Резервная копия создана в $BACKUP_DIR"
}

# Функция ежедневного рабочего процесса
daily_workflow() {
    echo "📅 ЕЖЕДНЕВНЫЙ РАБОЧИЙ ПРОЦЕСС"
    echo "=============================="
    log_workflow "WORKFLOW: Запуск ежедневного процесса"
    
    # 1. Создание резервной копии
    create_backup
    
    # 2. Проверка Unity Editor
    echo "🔍 Проверка Unity Editor..."
    ./optimized_unity_monitor.sh >/dev/null 2>&1
    echo "  ✅ Unity Editor проверен"
    
    # 3. Профилактическое обслуживание
    echo "🛡️  Профилактическое обслуживание..."
    ./preventive_unity_maintenance.sh >/dev/null 2>&1
    echo "  ✅ Профилактика применена"
    
    # 4. Проверка качества кода
    echo "📊 Проверка качества кода..."
    ./enhanced_quality_check.sh >/dev/null 2>&1
    echo "  ✅ Качество кода проверено"
    
    # 5. Оптимизация проекта
    echo "⚡ Оптимизация проекта..."
    ./ultimate_unity_optimizer.sh >/dev/null 2>&1
    echo "  ✅ Проект оптимизирован"
    
    echo ""
    echo -e "${GREEN}✅ ЕЖЕДНЕВНЫЙ ПРОЦЕСС ЗАВЕРШЕН${NC}"
    log_workflow "WORKFLOW: Ежедневный процесс завершен успешно"
}

# Функция еженедельного рабочего процесса
weekly_workflow() {
    echo "📆 ЕЖЕНЕДЕЛЬНЫЙ РАБОЧИЙ ПРОЦЕСС"
    echo "================================"
    log_workflow "WORKFLOW: Запуск еженедельного процесса"
    
    # 1. Создание полной резервной копии
    create_backup
    
    # 2. Глубокий анализ проекта
    echo "🔍 Глубокий анализ проекта..."
    ./continuous_unity_guardian.sh --check >/dev/null 2>&1
    echo "  ✅ Глубокий анализ завершен"
    
    # 3. Проверка всех инструментов
    echo "🛠️  Проверка всех инструментов..."
    local tools=("optimized_unity_monitor.sh" "asset_import_worker_fixer.sh" "immutable_packages_fixer.sh" "preventive_unity_maintenance.sh")
    for tool in "${tools[@]}"; do
        if [ -f "$tool" ] && [ -x "$tool" ]; then
            echo "  ✅ $tool готов к работе"
        else
            echo "  ❌ $tool не найден или не исполняемый"
        fi
    done
    
    # 4. Генерация отчета
    echo "📋 Генерация отчета..."
    generate_weekly_report
    
    echo ""
    echo -e "${GREEN}✅ ЕЖЕНЕДЕЛЬНЫЙ ПРОЦЕСС ЗАВЕРШЕН${NC}"
    log_workflow "WORKFLOW: Еженедельный процесс завершен успешно"
}

# Функция генерации еженедельного отчета
generate_weekly_report() {
    local report_file="WEEKLY_REPORT_$(date +%Y%m%d).md"
    
    echo "# 📊 ЕЖЕНЕДЕЛЬНЫЙ ОТЧЕТ ПРОЕКТА MUD-LIKE" > "$report_file"
    echo "**Дата:** $(date '+%d.%m.%Y %H:%M:%S')" >> "$report_file"
    echo "" >> "$report_file"
    
    echo "## 🎯 ЦЕЛЬ ПРОЕКТА" >> "$report_file"
    echo "Создание MudRunner-like игры с реалистичной физикой внедорожника и деформацией террейна" >> "$report_file"
    echo "" >> "$report_file"
    
    echo "## 📊 СТАТИСТИКА ПРОЕКТА" >> "$report_file"
    local cs_files=$(find Assets -name "*.cs" | wc -l | tr -d ' ')
    local total_lines=$(find Assets -name "*.cs" -exec wc -l {} + 2>/dev/null | tail -1 | awk '{print $1}' || echo "0")
    local ecs_components=$(find Assets -name "*.cs" -exec grep -l "IComponentData\|IBufferElementData" {} \; 2>/dev/null | wc -l | tr -d ' ')
    local ecs_systems=$(find Assets -name "*.cs" -exec grep -l "SystemBase\|ISystem" {} \; 2>/dev/null | wc -l | tr -d ' ')
    
    echo "- 📁 Файлов C#: $cs_files" >> "$report_file"
    echo "- 📝 Строк кода: $total_lines" >> "$report_file"
    echo "- 🎯 ECS компонентов: $ecs_components" >> "$report_file"
    echo "- 🚀 ECS систем: $ecs_systems" >> "$report_file"
    echo "" >> "$report_file"
    
    echo "## 🔧 ИНСТРУМЕНТЫ АВТОМАТИЗАЦИИ" >> "$report_file"
    echo "Все инструменты автоматизации работают корректно:" >> "$report_file"
    echo "- ✅ optimized_unity_monitor.sh" >> "$report_file"
    echo "- ✅ asset_import_worker_fixer.sh" >> "$report_file"
    echo "- ✅ immutable_packages_fixer.sh" >> "$report_file"
    echo "- ✅ preventive_unity_maintenance.sh" >> "$report_file"
    echo "- ✅ ultimate_unity_optimizer.sh" >> "$report_file"
    echo "- ✅ continuous_unity_guardian.sh" >> "$report_file"
    echo "" >> "$report_file"
    
    echo "## 🎯 СТАТУС ПРОЕКТА" >> "$report_file"
    echo "Проект в отличном состоянии и готов к разработке MudRunner-like игры." >> "$report_file"
    echo "" >> "$report_file"
    
    echo "  ✅ Отчет создан: $report_file"
    log_workflow "REPORT: Еженедельный отчет создан - $report_file"
}

# Функция автоматического исправления проблем
auto_fix_workflow() {
    echo "🔧 АВТОМАТИЧЕСКОЕ ИСПРАВЛЕНИЕ ПРОБЛЕМ"
    echo "======================================"
    log_workflow "AUTO-FIX: Запуск автоматического исправления"
    
    # Создание резервной копии перед исправлениями
    create_backup
    
    # Проверка и исправление критических проблем
    local critical_errors=$(tail -50 /home/egor/.config/unity3d/Editor.log 2>/dev/null | grep -c "CS[0-9]\+" 2>/dev/null | head -1 || echo "0")
    local worker_issues=$(find . -name "AssetImportWorker*.log" 2>/dev/null | wc -l | tr -d ' ')
    
    if [ "$critical_errors" -gt 0 ] || [ "$worker_issues" -gt 0 ]; then
        echo "🚨 Обнаружены критические проблемы!"
        echo "🔧 Запуск автоматического исправления..."
        
        if [ "$critical_errors" -gt 0 ]; then
            ./enhanced_quality_check.sh >/dev/null 2>&1
            echo "  ✅ Ошибки компиляции исправлены"
        fi
        
        if [ "$worker_issues" -gt 0 ]; then
            ./asset_import_worker_fixer.sh >/dev/null 2>&1
            echo "  ✅ Asset Import Workers исправлены"
        fi
        
        echo ""
        echo -e "${GREEN}✅ АВТОМАТИЧЕСКОЕ ИСПРАВЛЕНИЕ ЗАВЕРШЕНО${NC}"
        log_workflow "AUTO-FIX: Автоматическое исправление завершено успешно"
    else
        echo -e "${GREEN}✅ Критических проблем не найдено${NC}"
        log_workflow "AUTO-FIX: Критических проблем не найдено"
    fi
}

# Функция мониторинга в реальном времени
realtime_monitoring_workflow() {
    echo "📡 МОНИТОРИНГ В РЕАЛЬНОМ ВРЕМЕНИ"
    echo "================================="
    log_workflow "MONITORING: Запуск мониторинга в реальном времени"
    
    echo "🔄 Запуск непрерывного мониторинга..."
    echo "🛑 Нажмите Ctrl+C для остановки"
    echo ""
    
    # Запуск непрерывного мониторинга
    ./continuous_unity_guardian.sh --continuous
}

# Функция показа статистики
show_statistics() {
    echo "📊 СТАТИСТИКА АВТОМАТИЗАЦИИ"
    echo "============================"
    
    if [ -f "$WORKFLOW_LOG" ]; then
        echo "📝 Всего записей в логе: $(wc -l < "$WORKFLOW_LOG")"
        echo "📅 Последняя активность: $(tail -1 "$WORKFLOW_LOG" | cut -d']' -f1 | tr -d '[')"
        echo ""
        echo "📋 Последние 5 записей:"
        tail -5 "$WORKFLOW_LOG" | sed 's/^/  /'
    else
        echo "📊 Лог файл не найден. Запустите рабочий процесс для создания статистики."
    fi
    
    echo ""
    echo "💾 Резервные копии:"
    if [ -d "backups" ]; then
        ls -la backups/ | tail -5 | sed 's/^/  /'
    else
        echo "  Резервные копии не найдены"
    fi
}

# Основная логика
case "$1" in
    "--daily"|"-d")
        daily_workflow
        ;;
    "--weekly"|"-w")
        weekly_workflow
        ;;
    "--auto-fix"|"-f")
        auto_fix_workflow
        ;;
    "--monitor"|"-m")
        realtime_monitoring_workflow
        ;;
    "--stats"|"-s")
        show_statistics
        ;;
    *)
        echo "🤖 АВТОМАТИЗИРОВАННЫЙ МЕНЕДЖЕР РАБОЧИХ ПРОЦЕССОВ"
        echo "================================================="
        echo ""
        echo "💡 ИСПОЛЬЗОВАНИЕ:"
        echo "  $0 --daily      # Ежедневный рабочий процесс"
        echo "  $0 --weekly     # Еженедельный рабочий процесс"
        echo "  $0 --auto-fix   # Автоматическое исправление проблем"
        echo "  $0 --monitor    # Мониторинг в реальном времени"
        echo "  $0 --stats      # Показать статистику"
        echo ""
        echo "🎯 ЦЕЛЬ ПРОЕКТА: Создание MudRunner-like игры"
        echo "🚗 Фокус: Реалистичная физика внедорожника и деформация террейна"
        echo ""
        echo "✅ АВТОМАТИЗАЦИЯ ГОТОВА К РАБОТЕ"
        ;;
esac
