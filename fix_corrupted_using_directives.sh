#!/bin/bash

# 🔧 ИСПРАВИТЕЛЬ ПОВРЕЖДЕННЫХ USING ДИРЕКТИВ
# Исправляет некорректные using директивы типа "using if(Unity != null) Unity.Entities;"

echo "🔧 ИСПРАВЛЕНИЕ ПОВРЕЖДЕННЫХ USING ДИРЕКТИВ"
echo "=========================================="

# Счетчики
fixed_files=0

# Функция исправления файла
fix_file() {
    local file="$1"
    local temp_file="${file}.tmp"
    local has_changes=false
    
    echo "🔍 Обработка: $file"
    
    # Создаем временный файл с исправлениями
    while IFS= read -r line; do
        # Исправляем поврежденные using директивы
        if echo "$line" | grep -q "^using if("; then
            # Извлекаем правильную директиву после "if(...) "
            new_using=$(echo "$line" | sed 's/using if([^)]*) /using /')
            echo "$new_using"
            has_changes=true
        elif echo "$line" | grep -q "^namespace if("; then
            # Исправляем поврежденные namespace
            new_namespace=$(echo "$line" | sed 's/namespace if([^)]*) /namespace /')
            echo "$new_namespace"
            has_changes=true
        else
            echo "$line"
        fi
    done < "$file" > "$temp_file"
    
    # Если были изменения, заменяем оригинальный файл
    if [ "$has_changes" = true ]; then
        mv "$temp_file" "$file"
        echo "✅ Исправлен: $file"
        ((fixed_files++))
    else
        rm -f "$temp_file"
    fi
}

echo "🔍 Поиск файлов с поврежденными using директивами..."

# Находим все файлы с поврежденными using директивами
find Assets/Scripts -name "*.cs" -type f | while read -r file; do
    if grep -q "using if(" "$file" || grep -q "namespace if(" "$file"; then
        fix_file "$file"
    fi
done

echo ""
echo "📊 РЕЗУЛЬТАТЫ ИСПРАВЛЕНИЯ:"
echo "========================="
echo "🔧 Исправлено файлов: $fixed_files"

if [ $fixed_files -gt 0 ]; then
    echo ""
    echo "✅ ИСПРАВЛЕНИЕ ЗАВЕРШЕНО УСПЕШНО!"
    echo "🎯 Все поврежденные using директивы исправлены"
    echo "🚀 Проект готов к компиляции"
else
    echo ""
    echo "ℹ️  Поврежденных файлов не найдено"
fi