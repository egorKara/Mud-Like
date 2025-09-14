#!/bin/bash

echo "🚀 Запуск проверки директив препроцессора..."
echo "================================================"

# Функция проверки одного файла
check_file() {
    local file="$1"
    echo "🔍 Проверка файла: $file"
    
    # Подсчитать количество #if, #ifdef, #ifndef
    local if_count=$(grep -c "#if\|#ifdef\|#ifndef" "$file" 2>/dev/null || echo "0")
    # Подсчитать количество #endif
    local endif_count=$(grep -c "#endif" "$file" 2>/dev/null || echo "0")
    # Подсчитать количество #region
    local region_count=$(grep -c "#region" "$file" 2>/dev/null || echo "0")
    # Подсчитать количество #endregion
    local endregion_count=$(grep -c "#endregion" "$file" 2>/dev/null || echo "0")
    
    echo "📊 Директивы #if: $if_count"
    echo "📊 Директивы #endif: $endif_count"
    echo "📊 Директивы #region: $region_count"
    echo "📊 Директивы #endregion: $endregion_count"
    
    local file_errors=0
    
    if [ "$if_count" -ne "$endif_count" ]; then
        echo "❌ ОШИБКА: Непарные директивы препроцессора!"
        echo "   #if: $if_count, #endif: $endif_count"
        file_errors=$((file_errors + 1))
    fi
    
    if [ "$region_count" -ne "$endregion_count" ]; then
        echo "❌ ОШИБКА: Непарные директивы регионов!"
        echo "   #region: $region_count, #endregion: $endregion_count"
        file_errors=$((file_errors + 1))
    fi
    
    if [ "$file_errors" -eq 0 ]; then
        echo "✅ Все директивы препроцессора и регионов парные"
        return 0
    else
        return 1
    fi
}

# Проверка всех C# файлов
GLOBAL_ERROR_COUNT=0

find Assets/Scripts -name "*.cs" | while read -r file; do
    check_file "$file"
    if [ $? -ne 0 ]; then
        GLOBAL_ERROR_COUNT=$((GLOBAL_ERROR_COUNT + 1))
    fi
    echo "----------------------------------------"
done

echo "📊 ИТОГОВАЯ СТАТИСТИКА:"
echo "   Всего файлов: $(find Assets/Scripts -name "*.cs" | wc -l)"
echo "   Файлов с ошибками: $GLOBAL_ERROR_COUNT"
echo "   Файлов без ошибок: $(( $(find Assets/Scripts -name "*.cs" | wc -l) - GLOBAL_ERROR_COUNT ))"

if [ "$GLOBAL_ERROR_COUNT" -gt 0 ]; then
    echo "🚫 НАЙДЕНЫ ОШИБКИ В $GLOBAL_ERROR_COUNT ФАЙЛАХ!"
    echo "💡 Рекомендации:"
    echo "   - Проверьте парность директив #if/#endif"
    echo "   - Проверьте парность директив #region/#endregion"
    echo "   - Используйте IDE для подсветки парных директив"
    exit 1
else
    echo "✅ ВСЕ ФАЙЛЫ КОРРЕКТНЫ!"
    exit 0
fi