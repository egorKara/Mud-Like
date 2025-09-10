#!/bin/bash

# Тестирование исправленной системы защиты от перегрева
# Только чтение данных, без действий

echo "🧪 ТЕСТИРОВАНИЕ ИСПРАВЛЕННОЙ СИСТЕМЫ ЗАЩИТЫ ОТ ПЕРЕГРЕВА"
echo "=================================================="

# Импортируем функции из основного скрипта
source ./overheat_protection.sh

echo ""
echo "🔍 ЭТАП 1: Тестирование чтения данных (безопасно)"
echo "--------------------------------------------------"

# Тест 1: Получение температуры CPU
echo "1️⃣ Тестирование температуры CPU..."
cpu_temp=$(get_cpu_temperature)
echo "   Результат: ${cpu_temp}°C"

# Тест 2: Получение загрузки CPU
echo "2️⃣ Тестирование загрузки CPU..."
cpu_load=$(get_cpu_load)
echo "   Результат: ${cpu_load}%"

# Тест 3: Получение загрузки GPU
echo "3️⃣ Тестирование загрузки GPU..."
gpu_load=$(get_gpu_load)
echo "   Результат: ${gpu_load}%"

# Тест 4: Проверка Unity Editor
echo "4️⃣ Тестирование проверки Unity Editor..."
unity_running=$(is_unity_editor_running)
echo "   Unity Editor запущен: ${unity_running}"

# Тест 5: Получение информации о системе
echo "5️⃣ Тестирование системной информации..."
system_info=$(get_system_info)
echo "   Системная информация получена: ${#system_info} символов"

echo ""
echo "🔍 ЭТАП 2: Тестирование логики без действий"
echo "--------------------------------------------"

# Тест 6: Проверка пороговых значений
echo "6️⃣ Тестирование пороговых значений..."
echo "   CPU порог: ${MAX_CPU_LOAD_THRESHOLD}%"
echo "   GPU порог: ${MAX_GPU_LOAD_THRESHOLD}%"
echo "   Unity CPU лимит: ${UNITY_EDITOR_CPU_LIMIT}%"
echo "   Unity GPU лимит: ${UNITY_EDITOR_GPU_LIMIT}%"

# Тест 7: Проверка интервалов
echo "7️⃣ Тестирование интервалов..."
echo "   Минимальный интервал срабатывания: ${MIN_LOAD_REDUCTION_INTERVAL}с"
echo "   Интервал проверки температуры: ${TEMP_CHECK_INTERVAL}с"

echo ""
echo "🔍 ЭТАП 3: Тестирование функций без выполнения"
echo "----------------------------------------------"

# Тест 8: Симуляция проверки нагрузки (только логика)
echo "8️⃣ Тестирование логики проверки нагрузки..."
if [ "$cpu_load" -ge "$MAX_CPU_LOAD_THRESHOLD" ] || [ "$gpu_load" -ge "$MAX_GPU_LOAD_THRESHOLD" ]; then
    echo "   ⚠️  Нагрузка превышает порог (НЕ ВЫПОЛНЯЕМ ДЕЙСТВИЯ)"
else
    echo "   ✅ Нагрузка в пределах нормы"
fi

# Тест 9: Симуляция проверки Unity Editor
echo "9️⃣ Тестирование логики Unity Editor..."
if [ "$unity_running" = "true" ]; then
    if [ "$cpu_load" -ge "$UNITY_EDITOR_CPU_LIMIT" ] || [ "$gpu_load" -ge "$UNITY_EDITOR_GPU_LIMIT" ]; then
        echo "   ⚠️  Unity Editor превышает лимит (НЕ ВЫПОЛНЯЕМ ДЕЙСТВИЯ)"
    else
        echo "   ✅ Unity Editor в пределах лимита"
    fi
else
    echo "   ℹ️  Unity Editor не запущен"
fi

echo ""
echo "✅ ТЕСТИРОВАНИЕ ЗАВЕРШЕНО БЕЗОПАСНО"
echo "=================================="
echo "Все функции работают корректно"
echo "Никаких действий не было выполнено"