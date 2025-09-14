#!/bin/bash

# Скрипт для автоматического исправления использования устаревшего Time API в Unity DOTS

echo "🔧 Исправление использования устаревшего Time API в Unity DOTS..."
echo "=============================================================="

# Файл для хранения результатов
TEMP_FILE=$(mktemp)
FIXES_APPLIED=0
TOTAL_FILES=0

# Найти все файлы .cs в проекте
echo "📊 Поиск файлов .cs..."

find Assets/Scripts -name "*.cs" -type f | while read -r file; do
    TOTAL_FILES=$((TOTAL_FILES + 1))
    
    # Проверить, содержит ли файл устаревшие Time API
    if grep -q "Time\.\(deltaTime\|fixedDeltaTime\)" "$file"; then
        echo "🔍 Найден файл с устаревшим Time API: $file"
        
        # Создать резервную копию
        cp "$file" "$file.backup"
        
        # Исправить Time.deltaTime на SystemAPI.Time.DeltaTime
        sed -i 's/Time\.deltaTime/SystemAPI.Time.DeltaTime/g' "$file"
        
        # Исправить Time.fixedDeltaTime на SystemAPI.Time.DeltaTime (в ECS системах)
        if grep -q "SystemBase" "$file"; then
            sed -i 's/Time\.fixedDeltaTime/SystemAPI.Time.DeltaTime/g' "$file"
        fi
        
        # Проверить, были ли внесены изменения
        if ! diff -q "$file" "$file.backup" > /dev/null; then
            echo "✅ Исправлен: $file"
            FIXES_APPLIED=$((FIXES_APPLIED + 1))
            echo "$file" >> "$TEMP_FILE"
        else
            echo "ℹ️  Изменения не потребовались: $file"
            rm "$file.backup"
        fi
    fi
done

echo ""
echo "📊 СТАТИСТИКА ИСПРАВЛЕНИЙ:"
echo "   Всего файлов проверено: $TOTAL_FILES"
echo "   Файлов исправлено: $FIXES_APPLIED"

if [ $FIXES_APPLIED -gt 0 ]; then
    echo ""
    echo "✅ ИСПРАВЛЕННЫЕ ФАЙЛЫ:"
    while read -r file; do
        echo "   📝 $file"
    done < "$TEMP_FILE"
    
    echo ""
    echo "💡 РЕКОМЕНДАЦИИ:"
    echo "   1. Проверьте исправленные файлы вручную"
    echo "   2. Убедитесь, что SystemAPI.Time импортирован"
    echo "   3. Запустите тесты для проверки корректности"
    echo "   4. Удалите резервные копии после проверки: find . -name '*.backup' -delete"
else
    echo "✅ Все файлы уже используют правильный SystemAPI.Time API"
fi

# Очистка временных файлов
rm -f "$TEMP_FILE"

echo ""
echo "=============================================================="

if [ $FIXES_APPLIED -gt 0 ]; then
    echo "🎯 ИСПРАВЛЕНИЯ ПРИМЕНЕНЫ УСПЕШНО!"
    exit 0
else
    echo "✅ ВСЕ ФАЙЛЫ УЖЕ КОРРЕКТНЫ!"
    exit 0
fi
