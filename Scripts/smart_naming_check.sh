#!/bin/bash

# 🎯 Mud-Like Smart Naming Convention Checker
# Умная проверка соответствия правилам именования проекта Mud-Like

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

echo -e "${BLUE}🎯 Mud-Like Smart Naming Convention Checker${NC}"
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
    
    # Поиск файлов с ECS системами
    find Assets/Scripts -name "*.cs" -exec grep -l "class.*SystemBase" {} \; 2>/dev/null | while read file; do
        local basename=$(basename "$file" .cs)
        
        # Проверяем, является ли это ECS системой
        if grep -q "class.*SystemBase" "$file" 2>/dev/null; then
            # Проверяем, заканчивается ли имя файла на System.cs
            if [[ ! "$file" =~ System\.cs$ ]]; then
                error "ECS система должна заканчиваться на 'System.cs': $file"
            fi
            
            # Проверяем, заканчивается ли имя класса на System
            if ! grep -q "class.*System.*SystemBase" "$file" 2>/dev/null; then
                error "ECS система должна иметь имя класса, заканчивающееся на 'System': $file"
            fi
        fi
    done
}

# Проверка ECS компонентов
check_ecs_components() {
    echo -e "\n${BLUE}🔍 Проверка ECS компонентов...${NC}"
    
    # Поиск файлов с ECS компонентами
    find Assets/Scripts -name "*.cs" -exec grep -l "IComponentData" {} \; 2>/dev/null | while read file; do
        # Проверяем запрещенные имена компонентов
        if grep -q "struct.*\(Data\|Info\|Work\|Job\).*IComponentData" "$file" 2>/dev/null; then
            error "Запрещенное имя ECS компонента: $file"
        fi
    done
}

# Проверка менеджеров
check_managers() {
    echo -e "\n${BLUE}🔍 Проверка менеджеров...${NC}"
    
    # Поиск файлов с менеджерами (не ECS системы)
    find Assets/Scripts -name "*.cs" -exec grep -l "class.*Manager" {} \; 2>/dev/null | while read file; do
        # Проверяем, что это не ECS система
        if ! grep -q "SystemBase" "$file" 2>/dev/null; then
            # Проверяем, заканчивается ли имя файла на Manager.cs
            if [[ ! "$file" =~ Manager\.cs$ ]]; then
                warning "Менеджер должен заканчиваться на 'Manager.cs': $file"
            fi
        fi
    done
}

# Проверка пулов
check_pools() {
    echo -e "\n${BLUE}🔍 Проверка пулов...${NC}"
    
    # Поиск файлов с пулами (не ECS системы)
    find Assets/Scripts -name "*.cs" -exec grep -l "class.*Pool" {} \; 2>/dev/null | while read file; do
        # Проверяем, что это не ECS система
        if ! grep -q "SystemBase" "$file" 2>/dev/null; then
            # Проверяем, заканчивается ли имя файла на Pool.cs
            if [[ ! "$file" =~ Pool\.cs$ ]]; then
                warning "Пул должен заканчиваться на 'Pool.cs': $file"
            fi
        fi
    done
}

# Проверка Jobs
check_jobs() {
    echo -e "\n${BLUE}🔍 Проверка Jobs...${NC}"
    
    # Поиск файлов с Jobs
    find Assets/Scripts -name "*.cs" -exec grep -l "struct.*Job.*IJob" {} \; 2>/dev/null | while read file; do
        # Проверяем, заканчивается ли имя файла на Job.cs
        if [[ ! "$file" =~ Job\.cs$ ]]; then
            warning "Job должен заканчиваться на 'Job.cs': $file"
        fi
        
        # Проверяем запрещенные имена Jobs
        if grep -q "struct.*\(Job\|Work\).*IJob" "$file" 2>/dev/null; then
            error "Запрещенное имя Job (слишком общее): $file"
        fi
    done
}

# Проверка namespace
check_namespaces() {
    echo -e "\n${BLUE}🔍 Проверка namespace...${NC}"
    
    # Поиск файлов без namespace
    find Assets/Scripts -name "*.cs" -exec grep -L "namespace" {} \; 2>/dev/null | while read file; do
        warning "Файл без namespace: $file"
    done
    
    # Поиск неправильных namespace
    find Assets/Scripts -name "*.cs" -exec grep -l "namespace.*MudLike" {} \; 2>/dev/null | while read file; do
        if ! grep -q "namespace MudLike\." "$file" 2>/dev/null; then
            warning "Namespace должен быть в формате 'MudLike.Module.Submodule': $file"
        fi
    done
}

# Проверка Unity API конфликтов
check_unity_conflicts() {
    echo -e "\n${BLUE}🔍 Проверка конфликтов с Unity API...${NC}"
    
    # Поиск использования запрещенных Unity классов
    find Assets/Scripts -name "*.cs" -exec grep -l "class.*\(Transform\|GameObject\|MonoBehaviour\|Component\|Behaviour\)" {} \; 2>/dev/null | while read file; do
        error "Использование запрещенного Unity класса: $file"
    done
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
    check_managers
    check_pools
    check_jobs
    check_namespaces
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
