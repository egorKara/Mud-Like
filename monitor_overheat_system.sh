#!/bin/bash

# Скрипт мониторинга системы защиты от перегрева каждые 10 минут

echo "🔍 МОНИТОРИНГ СИСТЕМЫ ЗАЩИТЫ ОТ ПЕРЕГРЕВА"
echo "=========================================="
echo "Запуск: $(date)"
echo ""

# Функция для проверки состояния системы
check_system_status() {
    local timestamp=$(date '+%Y-%m-%d %H:%M:%S')
    echo "⏰ [$timestamp] Проверка состояния системы..."
    
    # Проверяем, работает ли система
    local overheat_pids=$(ps aux | grep overheat_protection | grep -v grep | awk '{print $2}')
    
    if [ -z "$overheat_pids" ]; then
        echo "❌ Система защиты от перегрева НЕ ЗАПУЩЕНА!"
        return 1
    fi
    
    echo "✅ Система защиты от перегрева работает (PID: $overheat_pids)"
    
    # Проверяем загрузку системы
    local cpu_usage=$(ps -p $overheat_pids -o %cpu --no-headers | tr -d ' ')
    local mem_usage=$(ps -p $overheat_pids -o %mem --no-headers | tr -d ' ')
    
    echo "📊 Загрузка системы: CPU ${cpu_usage}%, RAM ${mem_usage}%"
    
    # Проверяем логи на наличие ошибок
    if [ -f "overheat_protection.log" ]; then
        local log_size=$(wc -l < overheat_protection.log)
        local error_count=$(grep -c "ERROR\|CRITICAL\|EMERGENCY" overheat_protection.log 2>/dev/null || echo "0")
        local warning_count=$(grep -c "WARNING" overheat_protection.log 2>/dev/null || echo "0")
        
        echo "📝 Логи: ${log_size} строк, ${error_count} ошибок, ${warning_count} предупреждений"
        
        # Показываем последние 3 строки лога
        echo "📋 Последние записи:"
        tail -3 overheat_protection.log | sed 's/^/   /'
    else
        echo "⚠️  Файл логов не найден"
    fi
    
    # Проверяем системные ресурсы
    local cpu_load=$(cat /proc/loadavg | awk '{print $1}')
    local mem_available=$(free -m | awk 'NR==2{printf "%.1f", $7/$2*100}')
    local temp_file="/sys/class/thermal/thermal_zone0/temp"
    
    echo "🖥️  Системные ресурсы:"
    echo "   CPU Load: ${cpu_load}"
    echo "   RAM Available: ${mem_available}%"
    
    if [ -f "$temp_file" ]; then
        local temp_raw=$(cat "$temp_file")
        local temp_celsius=$((temp_raw / 1000))
        echo "   CPU Temperature: ${temp_celsius}°C"
    else
        echo "   CPU Temperature: недоступно"
    fi
    
    echo ""
    return 0
}

# Функция для отправки уведомления о проблемах
send_alert() {
    local message="$1"
    echo "🚨 АЛЕРТ: $message"
    # Здесь можно добавить отправку email, push-уведомлений и т.д.
}

# Основной цикл мониторинга
echo "🔄 Начинаю мониторинг каждые 10 минут..."
echo "Для остановки нажмите Ctrl+C"
echo ""

while true; do
    if ! check_system_status; then
        send_alert "Система защиты от перегрева остановлена!"
    fi
    
    echo "⏳ Ожидание 10 минут до следующей проверки..."
    sleep 600  # 10 минут = 600 секунд
done