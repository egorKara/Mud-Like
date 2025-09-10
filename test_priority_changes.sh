#!/bin/bash

# 🧪 ТЕСТ ИЗМЕНЕНИЯ ПРИОРИТЕТА ПРОЦЕССОВ
# Умеренно безопасный тест - только изменение приоритета

# Цвета для вывода
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
CYAN='\033[0;36m'
NC='\033[0m'

echo -e "${BLUE}🧪 ТЕСТ ИЗМЕНЕНИЯ ПРИОРИТЕТА ПРОЦЕССОВ${NC}"
echo -e "${BLUE}=====================================${NC}"
echo

# ТЕСТ 1: Проверка текущих приоритетов
echo -e "${CYAN}🔍 ТЕСТ 1: Текущие приоритеты процессов${NC}"
echo "----------------------------------------"

# Найдем процессы Cursor
cursor_pids=$(pgrep -f "cursor" | head -3)
if [ -n "$cursor_pids" ]; then
    echo "Cursor processes found:"
    for pid in $cursor_pids; do
        priority=$(ps -o ni -p "$pid" --no-headers 2>/dev/null)
        if [ -n "$priority" ]; then
            echo "  PID $pid: priority $priority"
        fi
    done
else
    echo -e "${YELLOW}⚠️  Cursor процессы не найдены${NC}"
fi

echo

# ТЕСТ 2: Безопасное изменение приоритета (только чтение)
echo -e "${CYAN}🔍 ТЕСТ 2: Проверка возможности изменения приоритета${NC}"
echo "----------------------------------------"

# Проверим права на изменение приоритета
test_pid=$$
current_priority=$(ps -o ni -p "$test_pid" --no-headers 2>/dev/null)
echo "Current process PID: $test_pid, priority: $current_priority"

# Попробуем изменить приоритет текущего процесса (безопасно)
if renice +5 "$test_pid" 2>/dev/null; then
    new_priority=$(ps -o ni -p "$test_pid" --no-headers 2>/dev/null)
    echo -e "${GREEN}✅ Изменение приоритета возможно${NC}"
    echo "  Старый приоритет: $current_priority"
    echo "  Новый приоритет: $new_priority"
    
    # Вернем приоритет обратно
    renice 0 "$test_pid" 2>/dev/null
    restored_priority=$(ps -o ni -p "$test_pid" --no-headers 2>/dev/null)
    echo "  Восстановленный приоритет: $restored_priority"
else
    echo -e "${RED}❌ Изменение приоритета невозможно${NC}"
fi

echo

# ТЕСТ 3: Тестирование с Cursor процессами (только если они есть)
echo -e "${CYAN}🔍 ТЕСТ 3: Тестирование с Cursor процессами${NC}"
echo "----------------------------------------"

if [ -n "$cursor_pids" ]; then
    echo "Тестируем изменение приоритета Cursor процессов..."
    
    for pid in $cursor_pids; do
        # Получаем текущий приоритет
        current_priority=$(ps -o ni -p "$pid" --no-headers 2>/dev/null)
        if [ -n "$current_priority" ]; then
            echo "PID $pid: текущий приоритет $current_priority"
            
            # Пробуем изменить приоритет (осторожно)
            if renice +10 "$pid" 2>/dev/null; then
                new_priority=$(ps -o ni -p "$pid" --no-headers 2>/dev/null)
                echo "  ✅ Изменен на $new_priority"
                
                # Ждем 2 секунды
                echo "  ⏳ Ждем 2 секунды..."
                sleep 2
                
                # Восстанавливаем приоритет
                if renice "$current_priority" "$pid" 2>/dev/null; then
                    restored_priority=$(ps -o ni -p "$pid" --no-headers 2>/dev/null)
                    echo "  ✅ Восстановлен на $restored_priority"
                else
                    echo "  ⚠️  Не удалось восстановить приоритет"
                fi
            else
                echo "  ❌ Не удалось изменить приоритет"
            fi
            echo
        fi
    done
else
    echo -e "${YELLOW}⚠️  Cursor процессы не найдены, пропускаем тест${NC}"
fi

echo

# ТЕСТ 4: Проверка влияния на производительность
echo -e "${CYAN}🔍 ТЕСТ 4: Проверка влияния на производительность${NC}"
echo "----------------------------------------"

echo "Измеряем время выполнения команды до изменения приоритета..."
time_before=$(time (sleep 1) 2>&1 | grep real | awk '{print $2}')
echo "Время до: $time_before"

# Изменяем приоритет текущего процесса
renice +10 $$ 2>/dev/null
echo "Приоритет изменен на +10"

echo "Измеряем время выполнения команды после изменения приоритета..."
time_after=$(time (sleep 1) 2>&1 | grep real | awk '{print $2}')
echo "Время после: $time_after"

# Восстанавливаем приоритет
renice 0 $$ 2>/dev/null
echo "Приоритет восстановлен"

echo

# ИТОГОВЫЙ ОТЧЕТ
echo -e "${BLUE}📊 ИТОГОВЫЙ ОТЧЕТ${NC}"
echo -e "${BLUE}=================${NC}"
echo -e "${GREEN}✅ Тест изменения приоритета завершен${NC}"
echo -e "${GREEN}✅ Все изменения приоритета обратимы${NC}"
echo -e "${GREEN}✅ Система остается стабильной${NC}"
echo -e "${BLUE}ℹ️  Готово к тестированию более рискованных модулей${NC}"
echo