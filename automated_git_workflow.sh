#!/bin/bash

# Автоматизированный Git workflow для MudRunner-like
# Создан: 14 сентября 2025
# Цель: НЕ ЗАБЫВАТЬ ПУШИТЬ НА ГИТХАБ - автоматизация!

echo "🔄 АВТОМАТИЗИРОВАННЫЙ GIT WORKFLOW MUD-RUNNER-LIKE"
echo "=================================================="

# Цвета для вывода
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
PURPLE='\033[0;35m'
CYAN='\033[0;36m'
NC='\033[0m' # No Color

# Конфигурация Git
GIT_LOG="git_workflow.log"
AUTO_COMMIT_INTERVAL=300  # Интервал автокоммита в секундах (5 минут)

# Функция логирования Git операций
log_git_operation() {
    local message="$1"
    local timestamp=$(date '+%Y-%m-%d %H:%M:%S')
    echo "[$timestamp] $message" | tee -a "$GIT_LOG"
}

# Функция проверки статуса Git
check_git_status() {
    echo "🔍 ПРОВЕРКА СТАТУСА GIT"
    echo "========================"
    
    # Проверка инициализации Git
    if [ ! -d ".git" ]; then
        echo "❌ Git не инициализирован"
        echo "🔧 Инициализация Git..."
        git init
        git remote add origin https://github.com/egor/Mud-Like.git 2>/dev/null || echo "⚠️  Remote origin уже существует"
        log_git_operation "Git инициализирован"
    else
        echo "✅ Git инициализирован"
    fi
    
    # Проверка статуса
    local status=$(git status --porcelain 2>/dev/null)
    local untracked=$(git ls-files --others --exclude-standard 2>/dev/null | wc -l | tr -d ' ')
    local modified=$(git diff --name-only 2>/dev/null | wc -l | tr -d ' ')
    local staged=$(git diff --cached --name-only 2>/dev/null | wc -l | tr -d ' ')
    
    echo "📊 Статус Git:"
    echo "  📁 Неотслеживаемых файлов: $untracked"
    echo "  🔄 Измененных файлов: $modified"
    echo "  📋 Подготовленных файлов: $staged"
    
    if [ "$untracked" -gt 0 ] || [ "$modified" -gt 0 ] || [ "$staged" -gt 0 ]; then
        echo -e "  ${YELLOW}⚠️  Есть изменения для коммита${NC}"
        return 1
    else
        echo -e "  ${GREEN}✅ Рабочая директория чистая${NC}"
        return 0
    fi
}

# Функция автоматического коммита
auto_commit() {
    echo ""
    echo "💾 АВТОМАТИЧЕСКИЙ КОММИТ"
    echo "========================"
    
    local timestamp=$(date '+%Y-%m-%d %H:%M:%S')
    local commit_message="🚀 Автоматический коммит MudRunner-like - $timestamp"
    
    # Добавление всех изменений
    echo "📁 Добавление файлов..."
    git add . 2>/dev/null
    
    # Проверка наличия изменений для коммита
    local staged_files=$(git diff --cached --name-only 2>/dev/null | wc -l | tr -d ' ')
    
    if [ "$staged_files" -eq 0 ]; then
        echo "✅ Нет изменений для коммита"
        return 0
    fi
    
    echo "📝 Коммит: $staged_files файлов"
    
    # Создание коммита
    git commit -m "$commit_message" 2>/dev/null
    
    if [ $? -eq 0 ]; then
        echo -e "  ${GREEN}✅ Коммит создан: $commit_message${NC}"
        log_git_operation "Коммит создан: $staged_files файлов"
        return 0
    else
        echo -e "  ${RED}❌ Ошибка создания коммита${NC}"
        log_git_operation "ОШИБКА: Не удалось создать коммит"
        return 1
    fi
}

# Функция автоматического пуша
auto_push() {
    echo ""
    echo "🚀 АВТОМАТИЧЕСКИЙ ПУШ НА ГИТХАБ"
    echo "================================"
    
    # Проверка наличия коммитов для пуша
    local ahead=$(git rev-list --count @{u}..HEAD 2>/dev/null || echo "0")
    
    if [ "$ahead" -eq 0 ]; then
        echo "✅ Нет коммитов для пуша"
        return 0
    fi
    
    echo "📤 Пуш $ahead коммитов на GitHub..."
    
    # Пуш на GitHub
    git push origin main 2>/dev/null || git push origin master 2>/dev/null
    
    if [ $? -eq 0 ]; then
        echo -e "  ${GREEN}✅ Успешно запушено на GitHub!${NC}"
        log_git_operation "ПУШ НА GITHUB: $ahead коммитов"
        return 0
    else
        echo -e "  ${RED}❌ Ошибка пуша на GitHub${NC}"
        log_git_operation "ОШИБКА: Не удалось запушить на GitHub"
        return 1
    fi
}

# Функция автоматического Git workflow
automated_git_workflow() {
    echo "🔄 АВТОМАТИЗИРОВАННЫЙ GIT WORKFLOW"
    echo "==================================="
    echo "⏰ Интервал автокоммита: $AUTO_COMMIT_INTERVAL секунд"
    echo "📝 Лог файл: $GIT_LOG"
    echo "🛑 Нажмите Ctrl+C для остановки"
    echo ""
    
    log_git_operation "Запуск автоматизированного Git workflow"
    
    local workflow_count=0
    local successful_commits=0
    local successful_pushes=0
    
    while true; do
        workflow_count=$((workflow_count + 1))
        local current_time=$(date '+%H:%M:%S')
        
        echo -n "[$current_time] Git workflow #$workflow_count... "
        
        # 1. Проверка статуса Git
        if check_git_status; then
            echo -e "${GREEN}✅ Рабочая директория чистая${NC}"
        else
            # 2. Автоматический коммит
            if auto_commit; then
                successful_commits=$((successful_commits + 1))
                
                # 3. Автоматический пуш
                if auto_push; then
                    successful_pushes=$((successful_pushes + 1))
                    echo -e "${GREEN}✅ Изменения запушены на GitHub!${NC}"
                else
                    echo -e "${YELLOW}⚠️  Коммит создан, но пуш не удался${NC}"
                fi
            else
                echo -e "${RED}❌ Не удалось создать коммит${NC}"
            fi
        fi
        
        echo "📊 Статистика: $successful_commits коммитов, $successful_pushes пушей"
        echo "⏳ Следующий Git workflow через $AUTO_COMMIT_INTERVAL секунд..."
        sleep $AUTO_COMMIT_INTERVAL
    done
}

# Функция ручного Git workflow
manual_git_workflow() {
    echo "🔄 РУЧНОЙ GIT WORKFLOW"
    echo "======================"
    
    # 1. Проверка статуса
    check_git_status
    local has_changes=$?
    
    if [ $has_changes -eq 0 ]; then
        echo "✅ Рабочая директория чистая - нечего коммитить"
        return 0
    fi
    
    # 2. Автоматический коммит
    if auto_commit; then
        # 3. Автоматический пуш
        if auto_push; then
            echo -e "${GREEN}🎉 Успешно! Изменения запушены на GitHub!${NC}"
            log_git_operation "РУЧНОЙ WORKFLOW: Успешно завершен"
            return 0
        else
            echo -e "${YELLOW}⚠️  Коммит создан, но пуш не удался${NC}"
            return 1
        fi
    else
        echo -e "${RED}❌ Не удалось создать коммит${NC}"
        return 1
    fi
}

# Функция показа статистики Git
show_git_statistics() {
    echo "📊 СТАТИСТИКА GIT"
    echo "================="
    
    if [ -f "$GIT_LOG" ]; then
        echo "📝 Всего операций в логе: $(wc -l < "$GIT_LOG")"
        echo "✅ Успешных коммитов: $(grep -c "Коммит создан" "$GIT_LOG" 2>/dev/null || echo "0")"
        echo "🚀 Успешных пушей: $(grep -c "ПУШ НА GITHUB" "$GIT_LOG" 2>/dev/null || echo "0")"
        echo "❌ Ошибок: $(grep -c "ОШИБКА" "$GIT_LOG" 2>/dev/null || echo "0")"
        echo ""
        echo "📋 Последние 5 операций:"
        tail -5 "$GIT_LOG" | sed 's/^/  /'
    else
        echo "📊 Лог файл не найден. Запустите Git workflow для создания статистики."
    fi
    
    # Статистика репозитория
    echo ""
    echo "📊 СТАТИСТИКА РЕПОЗИТОРИЯ:"
    local total_commits=$(git rev-list --count HEAD 2>/dev/null || echo "0")
    local total_files=$(git ls-files 2>/dev/null | wc -l | tr -d ' ')
    local total_size=$(du -sh . 2>/dev/null | cut -f1 || echo "0")
    
    echo "  📝 Всего коммитов: $total_commits"
    echo "  📁 Отслеживаемых файлов: $total_files"
    echo "  💾 Размер репозитория: $total_size"
}

# Функция настройки Git
setup_git() {
    echo "🔧 НАСТРОЙКА GIT"
    echo "================"
    
    # Настройка пользователя Git
    echo "👤 Настройка пользователя Git..."
    git config --global user.name "MudRunner-Like Developer" 2>/dev/null
    git config --global user.email "developer@mudrunner-like.com" 2>/dev/null
    
    # Настройка автоперевода переносов строк
    echo "📝 Настройка автоперевода переносов строк..."
    git config --global core.autocrlf input 2>/dev/null
    
    # Настройка безопасных директорий
    echo "🔒 Настройка безопасных директорий..."
    git config --global --add safe.directory "$(pwd)" 2>/dev/null
    
    echo "✅ Git настроен"
}

# Основная логика
case "$1" in
    "--auto"|"-a")
        setup_git
        automated_git_workflow
        ;;
    "--manual"|"-m")
        setup_git
        manual_git_workflow
        ;;
    "--stats"|"-s")
        show_git_statistics
        ;;
    "--setup"|"-i")
        setup_git
        ;;
    *)
        echo "🔄 АВТОМАТИЗИРОВАННЫЙ GIT WORKFLOW MUD-RUNNER-LIKE"
        echo "=================================================="
        echo ""
        echo "💡 ИСПОЛЬЗОВАНИЕ:"
        echo "  $0 --auto     # Автоматический Git workflow"
        echo "  $0 --manual   # Ручной Git workflow"
        echo "  $0 --stats    # Показать статистику Git"
        echo "  $0 --setup    # Настройка Git"
        echo ""
        echo "🎯 ПРИНЦИП: НЕ ЗАБЫВАТЬ ПУШИТЬ НА ГИТХАБ!"
        echo "🚗 MudRunner-like - цель проекта"
        echo "🔄 Автоматизация Git - основа безопасности"
        echo ""
        echo "✅ GIT WORKFLOW ГОТОВ К РАБОТЕ"
        ;;
esac
