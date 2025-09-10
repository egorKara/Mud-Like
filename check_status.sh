#!/bin/bash

# Быстрая проверка состояния системы защиты от перегрева

echo "🔍 БЫСТРАЯ ПРОВЕРКА СИСТЕМЫ ЗАЩИТЫ ОТ ПЕРЕГРЕВА"
echo "=============================================="
echo "Время: $(date)"
echo ""

# Проверяем процессы
echo "📊 Процессы:"
ps aux | grep -E "(overheat_protection|monitor_overheat)" | grep -v grep | while read line; do
    echo "   $line"
done

echo ""

# Проверяем логи системы защиты
if [ -f "overheat_protection.log" ]; then
    echo "📝 Система защиты от перегрева (последние 5 строк):"
    tail -5 overheat_protection.log | sed 's/^/   /'
else
    echo "❌ Лог системы защиты не найден"
fi

echo ""

# Проверяем логи мониторинга
if [ -f "monitor.log" ]; then
    echo "📊 Мониторинг (последние 3 строки):"
    tail -3 monitor.log | sed 's/^/   /'
else
    echo "❌ Лог мониторинга не найден"
fi

echo ""

# Системные ресурсы
echo "🖥️  Системные ресурсы:"
echo "   CPU Load: $(cat /proc/loadavg | awk '{print $1}')"
echo "   RAM Available: $(free -m | awk 'NR==2{printf "%.1f%%", $7/$2*100}')"

# Температура CPU
if [ -f "/sys/class/thermal/thermal_zone0/temp" ]; then
    temp_raw=$(cat /sys/class/thermal/thermal_zone0/temp)
    temp_celsius=$((temp_raw / 1000))
    echo "   CPU Temperature: ${temp_celsius}°C"
else
    echo "   CPU Temperature: недоступно"
fi

echo ""
echo "✅ Проверка завершена"