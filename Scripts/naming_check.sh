#!/bin/bash

# 🎯 Mud-Like Naming Convention Checker
# Проверка соответствия правилам именования проекта Mud-Like

set -e

# Цвета для вывода
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Счетчики
ERRORS=0
WARNINGS=0
TOTAL_FILES=0

echo -e "${BLUE}🎯 Mud-Like Naming Convention Checker${NC}"
echo "================================================"

# Функция для вывода ошибок
error() {
    echo -e "${RED}❌ ERROR:${NC} $1"
    ((ERRORS++))
}

# Функция для вывода предупреждений
warning() {
    echo -e "${YELLOW}⚠️  WARNING:${NC} $1"
    ((WARNINGS++))
}

# Функция для вывода успеха
success() {
    echo -e "${GREEN}✅ OK:${NC} $1"
}

# Проверка ECS систем
check_ecs_systems() {
    echo -e "\n${BLUE}🔍 Проверка ECS систем...${NC}"
    
    # Поиск ECS систем без правильного суффикса
    local bad_systems=$(find Assets/Scripts -name "*.cs" -exec grep -l "class.*System.*SystemBase" {} \; 2>/dev/null | while read file; do
        if [[ ! "$file" =~ System\.cs$ ]]; then
            echo "$file"
        fi
    done)
    
    if [ -n "$bad_systems" ]; then
        while IFS= read -r file; do
            if [ -f "$file" ]; then
                error "ECS система должна заканчиваться на 'System.cs': $file"
            fi
        done <<< "$bad_systems"
    fi
    
    # Поиск конфликтующих имен ECS систем
    local conflict_systems=$(find Assets/Scripts -name "*.cs" -exec grep -l "class.*\(ObjectPool\|Pool\|Manager\|System\|Data\|Info\|Work\|Job\).*SystemBase" {} \; 2>/dev/null || true)
    
    if [ -n "$conflict_systems" ]; then
        while IFS= read -r file; do
            if [ -f "$file" ]; then
                error "Запрещенное имя ECS системы (конфликт с source generators): $file"
            fi
        done <<< "$conflict_systems"
    fi
}

# Проверка ECS компонентов
check_ecs_components() {
    echo -e "\n${BLUE}🔍 Проверка ECS компонентов...${NC}"
    
    # Поиск компонентов с запрещенными именами
    local bad_components=$(find Assets/Scripts -name "*.cs" -exec grep -l "struct.*\(Data\|Info\|Pool\|Work\|Job\).*IComponentData" {} \; 2>/dev/null || true)
    
    if [ -n "$bad_components" ]; then
        while IFS= read -r file; do
            if [ -f "$file" ]; then
                error "Запрещенное имя ECS компонента: $file"
            fi
        done <<< "$bad_components"
    fi
}

# Проверка менеджеров и пулов
check_managers_pools() {
    echo -e "\n${BLUE}🔍 Проверка менеджеров и пулов...${NC}"
    
    # Поиск менеджеров без правильного суффикса
    local bad_managers=$(find Assets/Scripts -name "*.cs" -exec grep -l "class.*Manager" {} \; 2>/dev/null | xargs grep -l "class.*Manager" 2>/dev/null | xargs grep -L "Manager\.cs$" 2>/dev/null || true)
    
    if [ -n "$bad_managers" ]; then
        while IFS= read -r file; do
            if [ -f "$file" ]; then
                warning "Менеджер должен заканчиваться на 'Manager.cs': $file"
            fi
        done <<< "$bad_managers"
    fi
    
    # Поиск пулов без правильного суффикса
    local bad_pools=$(find Assets/Scripts -name "*.cs" -exec grep -l "class.*Pool" {} \; 2>/dev/null | xargs grep -l "class.*Pool" 2>/dev/null | xargs grep -L "Pool\.cs$" 2>/dev/null || true)
    
    if [ -n "$bad_pools" ]; then
        while IFS= read -r file; do
            if [ -f "$file" ]; then
                warning "Пул должен заканчиваться на 'Pool.cs': $file"
            fi
        done <<< "$bad_pools"
    fi
}

# Проверка Jobs
check_jobs() {
    echo -e "\n${BLUE}🔍 Проверка Jobs...${NC}"
    
    # Поиск Jobs без правильного суффикса
    local bad_jobs=$(find Assets/Scripts -name "*.cs" -exec grep -l "struct.*Job.*IJob" {} \; 2>/dev/null | xargs grep -l "struct.*Job.*IJob" 2>/dev/null | xargs grep -L "Job\.cs$" 2>/dev/null || true)
    
    if [ -n "$bad_jobs" ]; then
        while IFS= read -r file; do
            if [ -f "$file" ]; then
                warning "Job должен заканчиваться на 'Job.cs': $file"
            fi
        done <<< "$bad_jobs"
    fi
    
    # Поиск Jobs с запрещенными именами
    local conflict_jobs=$(find Assets/Scripts -name "*.cs" -exec grep -l "struct.*\(Job\|Work\).*IJob" {} \; 2>/dev/null || true)
    
    if [ -n "$conflict_jobs" ]; then
        while IFS= read -r file; do
            if [ -f "$file" ]; then
                error "Запрещенное имя Job (слишком общее): $file"
            fi
        done <<< "$conflict_jobs"
    fi
}

# Проверка namespace
check_namespaces() {
    echo -e "\n${BLUE}🔍 Проверка namespace...${NC}"
    
    # Поиск файлов без namespace
    local no_namespace=$(find Assets/Scripts -name "*.cs" -exec grep -L "namespace" {} \; 2>/dev/null || true)
    
    if [ -n "$no_namespace" ]; then
        while IFS= read -r file; do
            if [ -f "$file" ]; then
                warning "Файл без namespace: $file"
            fi
        done <<< "$no_namespace"
    fi
    
    # Поиск неправильных namespace
    local bad_namespaces=$(find Assets/Scripts -name "*.cs" -exec grep -l "namespace.*MudLike" {} \; 2>/dev/null | xargs grep -L "namespace MudLike\." 2>/dev/null || true)
    
    if [ -n "$bad_namespaces" ]; then
        while IFS= read -r file; do
            if [ -f "$file" ]; then
                warning "Namespace должен быть в формате 'MudLike.Module.Submodule': $file"
            fi
        done <<< "$bad_namespaces"
    fi
}

# Проверка Assembly Definitions
check_assembly_definitions() {
    echo -e "\n${BLUE}🔍 Проверка Assembly Definitions...${NC}"
    
    # Поиск папок без Assembly Definition
    local folders_without_asmdef=$(find Assets/Scripts -type d -name "ECS" -o -name "Performance" -o -name "Gameplay" | while read dir; do
        if [ ! -f "$dir"/*.asmdef ]; then
            echo "$dir"
        fi
    done)
    
    if [ -n "$folders_without_asmdef" ]; then
        while IFS= read -r folder; do
            if [ -n "$folder" ]; then
                warning "Папка без Assembly Definition: $folder"
            fi
        done <<< "$folders_without_asmdef"
    fi
}

# Проверка Unity API конфликтов
check_unity_conflicts() {
    echo -e "\n${BLUE}🔍 Проверка конфликтов с Unity API...${NC}"
    
    # Поиск использования запрещенных Unity классов
    local unity_conflicts=$(find Assets/Scripts -name "*.cs" -exec grep -l "class.*\(Transform\|GameObject\|MonoBehaviour\|Component\|Behaviour\)" {} \; 2>/dev/null || true)
    
    if [ -n "$unity_conflicts" ]; then
        while IFS= read -r file; do
            if [ -f "$file" ]; then
                error "Использование запрещенного Unity класса: $file"
            fi
        done <<< "$unity_conflicts"
    fi
}

# Подсчет файлов
count_files() {
    TOTAL_FILES=$(find Assets/Scripts -name "*.cs" | wc -l)
}

# Основная функция
main() {
    # Проверяем, что мы в корне проекта Unity
    if [ ! -f "ProjectSettings/ProjectVersion.txt" ]; then
        error "Скрипт должен запускаться из корня проекта Unity"
        exit 1
    fi
    
    count_files
    echo "Найдено файлов для проверки: $TOTAL_FILES"
    
    check_ecs_systems
    check_ecs_components
    check_managers_pools
    check_jobs
    check_namespaces
    check_assembly_definitions
    check_unity_conflicts
    
    echo -e "\n${BLUE}📊 Результаты проверки:${NC}"
    echo "================================================"
    echo "Всего файлов проверено: $TOTAL_FILES"
    echo -e "Ошибок: ${RED}$ERRORS${NC}"
    echo -e "Предупреждений: ${YELLOW}$WARNINGS${NC}"
    
    if [ $ERRORS -eq 0 ] && [ $WARNINGS -eq 0 ]; then
        echo -e "\n${GREEN}🎉 Все проверки пройдены успешно!${NC}"
        exit 0
    elif [ $ERRORS -eq 0 ]; then
        echo -e "\n${YELLOW}⚠️  Есть предупреждения, но критических ошибок нет.${NC}"
        exit 0
    else
        echo -e "\n${RED}❌ Найдены критические ошибки!${NC}"
        exit 1
    fi
}

# Запуск основной функции
main "$@"
