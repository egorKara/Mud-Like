#!/bin/bash

# 🎯 Mud-Like Naming Convention Fixer
# Автоматическое исправление нарушений правил именования

set -e

# Цвета для вывода
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

echo -e "${BLUE}🔧 Mud-Like Naming Convention Fixer${NC}"
echo "================================================"

# Функция для переименования файлов
rename_file() {
    local old_path="$1"
    local new_path="$2"
    
    if [ -f "$old_path" ] && [ ! -f "$new_path" ]; then
        mv "$old_path" "$new_path"
        echo -e "${GREEN}✅ Переименован:${NC} $old_path -> $new_path"
        return 0
    else
        echo -e "${YELLOW}⚠️  Пропущен:${NC} $old_path (файл не найден или уже существует)"
        return 1
    fi
}

# Исправление ECS систем
fix_ecs_systems() {
    echo -e "\n${BLUE}🔧 Исправление ECS систем...${NC}"
    
    # Поиск ECS систем без правильного суффикса
    find Assets/Scripts -name "*.cs" -exec grep -l "class.*System.*SystemBase" {} \; 2>/dev/null | while read file; do
        if [[ ! "$file" =~ System\.cs$ ]] && [[ ! "$file" =~ Manager\.cs$ ]] && [[ ! "$file" =~ Pool\.cs$ ]] && [[ ! "$file" =~ Job\.cs$ ]]; then
            local dir=$(dirname "$file")
            local basename=$(basename "$file" .cs)
            local new_name="${basename}System.cs"
            local new_path="$dir/$new_name"
            
            echo -e "${YELLOW}Найдена ECS система без суффикса:${NC} $file"
            echo -e "${BLUE}Предлагается переименовать в:${NC} $new_path"
            
            # Автоматическое переименование
            if rename_file "$file" "$new_path"; then
                # Обновление содержимого файла
                sed -i "s/class ${basename}/class ${basename}System/g" "$new_path"
                echo -e "${GREEN}✅ Обновлено содержимое файла${NC}"
            fi
        fi
    done
}

# Исправление менеджеров
fix_managers() {
    echo -e "\n${BLUE}🔧 Исправление менеджеров...${NC}"
    
    # Поиск менеджеров без правильного суффикса
    find Assets/Scripts -name "*.cs" -exec grep -l "class.*Manager" {} \; 2>/dev/null | while read file; do
        if [[ ! "$file" =~ Manager\.cs$ ]]; then
            local dir=$(dirname "$file")
            local basename=$(basename "$file" .cs)
            local new_name="${basename}Manager.cs"
            local new_path="$dir/$new_name"
            
            echo -e "${YELLOW}Найден менеджер без суффикса:${NC} $file"
            echo -e "${BLUE}Предлагается переименовать в:${NC} $new_path"
            
            # Автоматическое переименование
            if rename_file "$file" "$new_path"; then
                # Обновление содержимого файла
                sed -i "s/class ${basename}/class ${basename}Manager/g" "$new_path"
                echo -e "${GREEN}✅ Обновлено содержимое файла${NC}"
            fi
        fi
    done
}

# Исправление пулов
fix_pools() {
    echo -e "\n${BLUE}🔧 Исправление пулов...${NC}"
    
    # Поиск пулов без правильного суффикса
    find Assets/Scripts -name "*.cs" -exec grep -l "class.*Pool" {} \; 2>/dev/null | while read file; do
        if [[ ! "$file" =~ Pool\.cs$ ]]; then
            local dir=$(dirname "$file")
            local basename=$(basename "$file" .cs)
            local new_name="${basename}Pool.cs"
            local new_path="$dir/$new_name"
            
            echo -e "${YELLOW}Найден пул без суффикса:${NC} $file"
            echo -e "${BLUE}Предлагается переименовать в:${NC} $new_path"
            
            # Автоматическое переименование
            if rename_file "$file" "$new_path"; then
                # Обновление содержимого файла
                sed -i "s/class ${basename}/class ${basename}Pool/g" "$new_path"
                echo -e "${GREEN}✅ Обновлено содержимое файла${NC}"
            fi
        fi
    done
}

# Исправление Jobs
fix_jobs() {
    echo -e "\n${BLUE}🔧 Исправление Jobs...${NC}"
    
    # Поиск Jobs без правильного суффикса
    find Assets/Scripts -name "*.cs" -exec grep -l "struct.*Job.*IJob" {} \; 2>/dev/null | while read file; do
        if [[ ! "$file" =~ Job\.cs$ ]]; then
            local dir=$(dirname "$file")
            local basename=$(basename "$file" .cs)
            local new_name="${basename}Job.cs"
            local new_path="$dir/$new_name"
            
            echo -e "${YELLOW}Найден Job без суффикса:${NC} $file"
            echo -e "${BLUE}Предлагается переименовать в:${NC} $new_path"
            
            # Автоматическое переименование
            if rename_file "$file" "$new_path"; then
                # Обновление содержимого файла
                sed -i "s/struct ${basename}/struct ${basename}Job/g" "$new_path"
                echo -e "${GREEN}✅ Обновлено содержимое файла${NC}"
            fi
        fi
    done
}

# Основная функция
main() {
    # Проверяем, что мы в корне проекта Unity
    if [ ! -f "ProjectSettings/ProjectVersion.txt" ]; then
        echo -e "${RED}❌ ОШИБКА:${NC} Скрипт должен запускаться из корня проекта Unity"
        exit 1
    fi
    
    echo -e "${YELLOW}⚠️  ВНИМАНИЕ:${NC} Этот скрипт автоматически переименует файлы!"
    echo -e "${YELLOW}⚠️  Убедитесь, что у вас есть резервная копия проекта!${NC}"
    echo ""
    read -p "Продолжить? (y/N): " -n 1 -r
    echo ""
    
    if [[ ! $REPLY =~ ^[Yy]$ ]]; then
        echo "Отменено пользователем."
        exit 0
    fi
    
    fix_ecs_systems
    fix_managers
    fix_pools
    fix_jobs
    
    echo -e "\n${GREEN}🎉 Исправление завершено!${NC}"
    echo -e "${BLUE}Запустите проверку:${NC} ./Scripts/naming_check.sh"
}

# Запуск основной функции
main "$@"
