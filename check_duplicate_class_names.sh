#!/bin/bash

# Скрипт для проверки дублирующихся имен классов в проекте
# Предотвращает ошибки CS0101: already contains a definition

echo "🔍 Проверка дублирующихся имен классов..."
echo "========================================"

# Файл для хранения результатов
TEMP_FILE=$(mktemp)
ERRORS_FOUND=0
TOTAL_CLASSES=0

# Найти все определения классов и структур
echo "📊 Поиск всех определений классов и структур..."

find Assets/Scripts -name "*.cs" -type f | while read -r file; do
    # Извлечь все определения классов и структур с их namespace
    grep -n "^\s*\(public\|internal\|private\)\s*\(class\|struct\|interface\)\s\+[A-Za-z_][A-Za-z0-9_]*" "$file" | while read -r line; do
        line_number=$(echo "$line" | cut -d: -f1)
        content=$(echo "$line" | cut -d: -f2-)
        
        # Извлечь namespace из файла
        namespace=$(grep -B 50 "^\s*\(public\|internal\|private\)\s*\(class\|struct\|interface\)\s\+[A-Za-z_][A-Za-z0-9_]*" "$file" | grep "namespace" | tail -1 | sed 's/.*namespace\s\+\([^;{]*\).*/\1/' | tr -d ' ')
        
        # Извлечь имя класса/структуры
        class_name=$(echo "$content" | sed 's/.*\(class\|struct\|interface\)\s\+\([A-Za-z_][A-Za-z0-9_]*\).*/\2/')
        
        if [ -n "$class_name" ] && [ -n "$namespace" ]; then
            full_name="$namespace.$class_name"
            echo "$full_name|$file:$line_number|$content" >> "$TEMP_FILE"
        fi
    done
done

# Проверить дубликаты
echo "🔍 Проверка дублирующихся имен..."

# Создать временный файл для группировки
GROUPED_FILE=$(mktemp)

# Группировать по полному имени
sort "$TEMP_FILE" | cut -d'|' -f1 | uniq -c | sort -nr > "$GROUPED_FILE"

# Найти дубликаты
duplicates=$(awk '$1 > 1 {print $2}' "$GROUPED_FILE")

if [ -n "$duplicates" ]; then
    echo "❌ НАЙДЕНЫ ДУБЛИРУЮЩИЕСЯ ИМЕНА КЛАССОВ:"
    echo ""
    
    for duplicate in $duplicates; do
        echo "🔴 Дубликат: $duplicate"
        grep "^$duplicate|" "$TEMP_FILE" | while read -r line; do
            location=$(echo "$line" | cut -d'|' -f2)
            definition=$(echo "$line" | cut -d'|' -f3)
            echo "   📍 $location: $definition"
        done
        echo ""
        ERRORS_FOUND=$((ERRORS_FOUND + 1))
    done
    
    echo "========================================"
    echo "❌ ОШИБКА: Найдено $ERRORS_FOUND дублирующихся имен классов"
    echo "💡 Рекомендации:"
    echo "   1. Переименуйте конфликтующие классы"
    echo "   2. Используйте уникальные имена с префиксами"
    echo "   3. Проверьте автогенерированный код Unity"
    echo "   4. Избегайте имен, которые могут конфликтовать с Unity API"
    
else
    echo "✅ Дублирующихся имен классов не найдено"
fi

# Подсчитать общее количество классов
TOTAL_CLASSES=$(wc -l < "$TEMP_FILE")

# Показать статистику
echo ""
echo "📊 СТАТИСТИКА:"
echo "   Всего классов/структур: $TOTAL_CLASSES"
echo "   Дубликатов найдено: $ERRORS_FOUND"

# Показать потенциально проблемные имена
echo ""
echo "⚠️  ПОТЕНЦИАЛЬНО ПРОБЛЕМНЫЕ ИМЕНА (могут конфликтовать с Unity API):"
problematic_names=("MemoryPool" "ObjectPool" "ComponentSystem" "SystemBase" "IComponentData" "ISystem")

for name in "${problematic_names[@]}"; do
    if grep -q "$name" "$TEMP_FILE"; then
        echo "🔸 $name - может конфликтовать с Unity API"
        grep "$name" "$TEMP_FILE" | while read -r line; do
            location=$(echo "$line" | cut -d'|' -f2)
            echo "   📍 $location"
        done
    fi
done

# Очистка временных файлов
rm -f "$TEMP_FILE" "$GROUPED_FILE"

echo ""
echo "========================================"

if [ $ERRORS_FOUND -eq 0 ]; then
    echo "✅ ВСЕ ИМЕНА КЛАССОВ УНИКАЛЬНЫ!"
    exit 0
else
    echo "❌ НАЙДЕНЫ ДУБЛИРУЮЩИЕСЯ ИМЕНА!"
    exit 1
fi
