#!/bin/bash

# Скрипт для автоматизации тестирования Mud-Like проекта
# Автор: Mud-Like Team
# Версия: 1.0.0

set -e  # Выход при ошибке

# Цвета для вывода
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Функция для логирования
log() {
    echo -e "${BLUE}[$(date +'%Y-%m-%d %H:%M:%S')]${NC} $1"
}

error() {
    echo -e "${RED}[ERROR]${NC} $1" >&2
}

success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1"
}

warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

# Функция для проверки зависимостей
check_dependencies() {
    log "Проверка зависимостей..."
    
    if ! command -v unity &> /dev/null; then
        error "Unity не найден в PATH"
        exit 1
    fi
    
    if ! command -v python3 &> /dev/null; then
        error "Python3 не найден в PATH"
        exit 1
    fi
    
    success "Все зависимости найдены"
}

# Функция для очистки кэша
clean_cache() {
    log "Очистка кэша Unity..."
    
    if [ -d "Library" ]; then
        rm -rf Library
        log "Удалена папка Library"
    fi
    
    if [ -d "Temp" ]; then
        rm -rf Temp
        log "Удалена папка Temp"
    fi
    
    if [ -d "obj" ]; then
        rm -rf obj
        log "Удалена папка obj"
    fi
    
    if [ -d "bin" ]; then
        rm -rf bin
        log "Удалена папка bin"
    fi
    
    success "Кэш очищен"
}

# Функция для запуска Unit тестов
run_unit_tests() {
    log "Запуск Unit тестов..."
    
    local test_log="unit-tests-$(date +'%Y%m%d-%H%M%S').log"
    
    if unity -projectPath . -batchmode -quit -runTests -testResults "artifacts/unit-test-results.xml" -logFile "$test_log"; then
        success "Unit тесты завершены успешно"
        return 0
    else
        error "Unit тесты провалились"
        if [ -f "$test_log" ]; then
            error "Лог ошибок: $test_log"
        fi
        return 1
    fi
}

# Функция для запуска интеграционных тестов
run_integration_tests() {
    log "Запуск интеграционных тестов..."
    
    local test_log="integration-tests-$(date +'%Y%m%d-%H%M%S').log"
    
    if unity -projectPath . -batchmode -quit -runTests -testFilter "MudLike.Tests.Integration" -testResults "artifacts/integration-test-results.xml" -logFile "$test_log"; then
        success "Интеграционные тесты завершены успешно"
        return 0
    else
        error "Интеграционные тесты провалились"
        if [ -f "$test_log" ]; then
            error "Лог ошибок: $test_log"
        fi
        return 1
        fi
}

# Функция для запуска тестов с покрытием кода
run_coverage_tests() {
    log "Запуск тестов с покрытием кода..."
    
    local test_log="coverage-tests-$(date +'%Y%m%d-%H%M%S').log"
    
    if unity -projectPath . -batchmode -quit -runTests -enableCodeCoverage -coverageResultsPath "artifacts/coverage-results" -testResults "artifacts/coverage-test-results.xml" -logFile "$test_log"; then
        success "Тесты с покрытием кода завершены успешно"
        return 0
    else
        error "Тесты с покрытием кода провалились"
        if [ -f "$test_log" ]; then
            error "Лог ошибок: $test_log"
        fi
        return 1
    fi
}

# Функция для генерации отчетов
generate_reports() {
    log "Генерация отчетов..."
    
    if [ ! -d "artifacts" ]; then
        error "Папка artifacts не найдена"
        return 1
    fi
    
    if command -v python3 &> /dev/null; then
        if [ -f "Scripts/generate-test-reports.py" ]; then
            python3 Scripts/generate-test-reports.py
            success "Отчеты сгенерированы"
        else
            warning "Скрипт генерации отчетов не найден"
        fi
    else
        warning "Python3 не найден, отчеты не сгенерированы"
    fi
}

# Функция для отправки уведомлений
send_notifications() {
    local status=$1
    local message=$2
    
    log "Отправка уведомлений..."
    
    # Здесь можно добавить отправку уведомлений в Slack, Discord, email и т.д.
    # Например:
    # curl -X POST -H 'Content-type: application/json' \
    #     --data "{\"text\":\"$message\"}" \
    #     $SLACK_WEBHOOK_URL
    
    success "Уведомления отправлены"
}

# Функция для создания артефактов
create_artifacts() {
    log "Создание артефактов..."
    
    local timestamp=$(date +'%Y%m%d-%H%M%S')
    local artifacts_dir="artifacts-$timestamp"
    
    mkdir -p "$artifacts_dir"
    
    # Копирование логов
    if [ -d "artifacts" ]; then
        cp -r artifacts/* "$artifacts_dir/"
    fi
    
    # Копирование отчетов
    if [ -d "reports" ]; then
        cp -r reports/* "$artifacts_dir/"
    fi
    
    # Создание архива
    tar -czf "test-artifacts-$timestamp.tar.gz" "$artifacts_dir"
    
    success "Артефакты созданы: test-artifacts-$timestamp.tar.gz"
}

# Функция для отображения справки
show_help() {
    echo "Использование: $0 [ОПЦИИ]"
    echo ""
    echo "ОПЦИИ:"
    echo "  -h, --help              Показать эту справку"
    echo "  -c, --clean             Очистить кэш перед тестированием"
    echo "  -u, --unit              Запустить только Unit тесты"
    echo "  -i, --integration       Запустить только интеграционные тесты"
    echo "  -co, --coverage         Запустить тесты с покрытием кода"
    echo "  -a, --all               Запустить все тесты (по умолчанию)"
    echo "  -r, --reports           Генерировать отчеты"
    echo "  -n, --notifications     Отправлять уведомления"
    echo "  -v, --verbose           Подробный вывод"
    echo ""
    echo "ПРИМЕРЫ:"
    echo "  $0 --clean --all --reports"
    echo "  $0 --unit --coverage"
    echo "  $0 --integration --notifications"
}

# Основная функция
main() {
    local clean_cache=false
    local run_unit=false
    local run_integration=false
    local run_coverage=false
    local run_all=true
    local generate_reports_flag=false
    local send_notifications_flag=false
    local verbose=false
    
    # Парсинг аргументов
    while [[ $# -gt 0 ]]; do
        case $1 in
            -h|--help)
                show_help
                exit 0
                ;;
            -c|--clean)
                clean_cache=true
                shift
                ;;
            -u|--unit)
                run_unit=true
                run_all=false
                shift
                ;;
            -i|--integration)
                run_integration=true
                run_all=false
                shift
                ;;
            -co|--coverage)
                run_coverage=true
                run_all=false
                shift
                ;;
            -a|--all)
                run_all=true
                shift
                ;;
            -r|--reports)
                generate_reports_flag=true
                shift
                ;;
            -n|--notifications)
                send_notifications_flag=true
                shift
                ;;
            -v|--verbose)
                verbose=true
                shift
                ;;
            *)
                error "Неизвестная опция: $1"
                show_help
                exit 1
                ;;
        esac
    done
    
    # Настройка verbose режима
    if [ "$verbose" = true ]; then
        set -x
    fi
    
    log "Запуск автоматизированного тестирования Mud-Like..."
    
    # Проверка зависимостей
    check_dependencies
    
    # Очистка кэша
    if [ "$clean_cache" = true ]; then
        clean_cache
    fi
    
    # Создание папки для артефактов
    mkdir -p artifacts
    
    local test_status=0
    
    # Запуск тестов
    if [ "$run_all" = true ] || [ "$run_unit" = true ]; then
        if ! run_unit_tests; then
            test_status=1
        fi
    fi
    
    if [ "$run_all" = true ] || [ "$run_integration" = true ]; then
        if ! run_integration_tests; then
            test_status=1
        fi
    fi
    
    if [ "$run_all" = true ] || [ "$run_coverage" = true ]; then
        if ! run_coverage_tests; then
            test_status=1
        fi
    fi
    
    # Генерация отчетов
    if [ "$generate_reports_flag" = true ]; then
        generate_reports
    fi
    
    # Создание артефактов
    create_artifacts
    
    # Отправка уведомлений
    if [ "$send_notifications_flag" = true ]; then
        if [ $test_status -eq 0 ]; then
            send_notifications "SUCCESS" "Все тесты прошли успешно! ✅"
        else
            send_notifications "FAILURE" "Некоторые тесты провалились! ❌"
        fi
    fi
    
    # Итоговый статус
    if [ $test_status -eq 0 ]; then
        success "Все тесты завершены успешно!"
        exit 0
    else
        error "Некоторые тесты провалились!"
        exit 1
    fi
}

# Запуск основной функции
main "$@"
