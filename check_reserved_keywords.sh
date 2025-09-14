#!/bin/bash

echo "🔍 Проверка использования зарезервированных ключевых слов C#..."
echo "=============================================================="

# Список зарезервированных ключевых слов C#
RESERVED_KEYWORDS=(
    "abstract" "as" "base" "bool" "break" "byte" "case" "catch" "char" "checked" "class"
    "const" "continue" "decimal" "default" "delegate" "do" "double" "else" "enum" "event"
    "explicit" "extern" "false" "finally" "fixed" "float" "for" "foreach" "goto" "if"
    "implicit" "in" "int" "interface" "internal" "is" "lock" "long" "namespace" "new"
    "null" "object" "operator" "out" "override" "params" "private" "protected" "public"
    "readonly" "ref" "return" "sbyte" "sealed" "short" "sizeof" "stackalloc" "static"
    "string" "struct" "switch" "this" "throw" "true" "try" "typeof" "uint" "ulong"
    "unchecked" "unsafe" "ushort" "using" "virtual" "void" "volatile" "while"
)

ERROR_COUNT=0

# Функция проверки файла на использование зарезервированных слов как переменных
check_file() {
    local file="$1"
    local file_errors=0
    
    echo "📄 Проверка файла: $file"
    
    for keyword in "${RESERVED_KEYWORDS[@]}"; do
        # Ищем использование ключевого слова как переменную (int keyword =)
        if grep -n "int $keyword[[:space:]]*=" "$file" >/dev/null 2>&1; then
            echo "❌ ОШИБКА: Использование зарезервированного слова '$keyword' как переменной в $file"
            grep -n "int $keyword[[:space:]]*=" "$file"
            file_errors=$((file_errors + 1))
        fi
        
        # Ищем использование ключевого слова как переменную (var keyword =)
        if grep -n "var $keyword[[:space:]]*=" "$file" >/dev/null 2>&1; then
            echo "❌ ОШИБКА: Использование зарезервированного слова '$keyword' как переменной в $file"
            grep -n "var $keyword[[:space:]]*=" "$file"
            file_errors=$((file_errors + 1))
        fi
        
        # Ищем использование ключевого слова как переменную (float keyword =)
        if grep -n "float $keyword[[:space:]]*=" "$file" >/dev/null 2>&1; then
            echo "❌ ОШИБКА: Использование зарезервированного слова '$keyword' как переменной в $file"
            grep -n "float $keyword[[:space:]]*=" "$file"
            file_errors=$((file_errors + 1))
        fi
    done
    
    if [ "$file_errors" -eq 0 ]; then
        echo "✅ Файл корректен"
    else
        echo "🚫 Найдено $file_errors ошибок"
        ERROR_COUNT=$((ERROR_COUNT + file_errors))
    fi
    echo "----------------------------------------"
}

echo "🔍 Поиск C# файлов в проекте..."
find Assets/Scripts -name "*.cs" -type f | while read -r file; do
    check_file "$file"
done

echo ""
echo "📊 ИТОГОВАЯ СТАТИСТИКА:"
echo "   Всего файлов: $(find Assets/Scripts -name "*.cs" | wc -l)"
echo "   Файлов с ошибками: $ERROR_COUNT"

if [ "$ERROR_COUNT" -gt 0 ]; then
    echo "🚫 НАЙДЕНЫ ОШИБКИ ИСПОЛЬЗОВАНИЯ ЗАРЕЗЕРВИРОВАННЫХ СЛОВ!"
    echo "💡 Рекомендации:"
    echo "   - Замените зарезервированные слова на описательные имена"
    echo "   - Например: 'fixed' -> 'fixedCount', 'int' -> 'integerValue'"
    exit 1
else
    echo "✅ ВСЕ ФАЙЛЫ КОРРЕКТНЫ!"
    exit 0
fi
