#!/bin/bash

# Автоматический генератор документации для Unity проекта

echo "📚 АВТОМАТИЧЕСКИЙ ГЕНЕРАТОР ДОКУМЕНТАЦИИ"
echo "======================================="

# Цвета для вывода
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
PURPLE='\033[0;35m'
CYAN='\033[0;36m'
NC='\033[0m' # No Color

# Статистика
total_files=0
documented_files=0
undocumented_files=0
total_methods=0
documented_methods=0
undocumented_methods=0

echo ""
echo "🔍 АНАЛИЗ ДОКУМЕНТАЦИИ"
echo "======================"

# Функция для анализа файла
analyze_file() {
    local file="$1"
    local filename=$(basename "$file")
    
    total_files=$((total_files + 1))
    
    # Проверяем наличие XML документации для класса
    if grep -q "/// <summary>" "$file"; then
        documented_files=$((documented_files + 1))
        echo -e "✅ $filename - ${GREEN}ДОКУМЕНТИРОВАН${NC}"
    else
        undocumented_files=$((undocumented_files + 1))
        echo -e "❌ $filename - ${RED}НЕ ДОКУМЕНТИРОВАН${NC}"
    fi
    
    # Анализируем методы
    local methods=$(grep -E "^\s*(public|private|protected)\s+[a-zA-Z_][a-zA-Z0-9_<>,\s]*\s+[a-zA-Z_][a-zA-Z0-9_]*\s*\(" "$file" | wc -l)
    local documented_methods_in_file=$(grep -A 5 -B 5 "^\s*(public|private|protected)\s+[a-zA-Z_][a-zA-Z0-9_<>,\s]*\s+[a-zA-Z_][a-zA-Z0-9_]*\s*\(" "$file" | grep -c "/// <summary>" || echo "0")
    
    total_methods=$((total_methods + methods))
    documented_methods=$((documented_methods + documented_methods_in_file))
    undocumented_methods=$((undocumented_methods + (methods - documented_methods_in_file)))
}

# Анализируем все C# файлы
echo "📁 Анализ файлов..."
find Assets/Scripts -name "*.cs" -type f | while read -r file; do
    analyze_file "$file"
done

echo ""
echo "📊 СТАТИСТИКА ДОКУМЕНТАЦИИ"
echo "=========================="

echo -e "Файлов всего: ${CYAN}$total_files${NC}"
echo -e "Документировано: ${GREEN}$documented_files${NC}"
echo -e "Не документировано: ${RED}$undocumented_files${NC}"

if [ $total_files -gt 0 ]; then
    documentation_percentage=$((documented_files * 100 / total_files))
    echo -e "Процент документирования: ${BLUE}$documentation_percentage%${NC}"
fi

echo ""
echo -e "Методов всего: ${CYAN}$total_methods${NC}"
echo -e "Документировано: ${GREEN}$documented_methods${NC}"
echo -e "Не документировано: ${RED}$undocumented_methods${NC}"

if [ $total_methods -gt 0 ]; then
    methods_documentation_percentage=$((documented_methods * 100 / total_methods))
    echo -e "Процент документирования методов: ${BLUE}$methods_documentation_percentage%${NC}"
fi

echo ""
echo "🔧 АВТОМАТИЧЕСКОЕ ИСПРАВЛЕНИЕ"
echo "============================="

# Создаем шаблоны документации для недокументированных файлов
echo "📝 Создание шаблонов документации..."

# Функция для создания документации для класса
create_class_documentation() {
    local file="$1"
    local class_name=$(grep -E "^\s*(public|private|protected)\s+class\s+[a-zA-Z_][a-zA-Z0-9_]*" "$file" | head -1 | sed 's/.*class\s\+\([a-zA-Z_][a-zA-Z0-9_]*\).*/\1/')
    
    if [ -n "$class_name" ] && ! grep -q "/// <summary>" "$file"; then
        echo "  📝 Добавление документации для класса: $class_name"
        
        # Создаем временный файл с документацией
        local temp_file=$(mktemp)
        
        # Добавляем документацию перед классом
        sed "/^\s*\(public\|private\|protected\)\s*class\s*$class_name/i\\
/// <summary>\\
/// $class_name - описание класса\\
/// </summary>\\
/// <remarks>\\
/// Подробное описание функциональности класса\\
/// </remarks>\\
" "$file" > "$temp_file"
        
        # Заменяем оригинальный файл
        mv "$temp_file" "$file"
    fi
}

# Функция для создания документации для методов
create_method_documentation() {
    local file="$1"
    
    # Находим недокументированные публичные методы
    grep -n -E "^\s*public\s+[a-zA-Z_][a-zA-Z0-9_<>,\s]*\s+[a-zA-Z_][a-zA-Z0-9_]*\s*\(" "$file" | while read -r line; do
        local line_num=$(echo "$line" | cut -d: -f1)
        local method_name=$(echo "$line" | sed 's/.*\s\+\([a-zA-Z_][a-zA-Z0-9_]*\)\s*(.*/\1/')
        
        # Проверяем, есть ли уже документация для этого метода
        if ! sed -n "$((line_num - 3)),$((line_num - 1))p" "$file" | grep -q "/// <summary>"; then
            echo "    📝 Добавление документации для метода: $method_name"
            
            # Создаем временный файл с документацией
            local temp_file=$(mktemp)
            
            # Добавляем документацию перед методом
            sed "${line_num}i\\
    /// <summary>\\
    /// $method_name - описание метода\\
    /// </summary>\\
    /// <param name=\"param1\">Описание параметра</param>\\
    /// <returns>Описание возвращаемого значения</returns>\\
" "$file" > "$temp_file"
            
            # Заменяем оригинальный файл
            mv "$temp_file" "$file"
        fi
    done
}

# Применяем автоматическое исправление к недокументированным файлам
find Assets/Scripts -name "*.cs" -type f | while read -r file; do
    if ! grep -q "/// <summary>" "$file"; then
        echo "🔧 Исправление: $(basename "$file")"
        create_class_documentation "$file"
        create_method_documentation "$file"
    fi
done

echo ""
echo "📋 РЕКОМЕНДАЦИИ ПО УЛУЧШЕНИЮ"
echo "============================="

if [ $undocumented_files -gt 0 ]; then
    echo -e "${YELLOW}⚠️  Найдено $undocumented_files недокументированных файлов${NC}"
    echo -e "${BLUE}💡 Рекомендации:${NC}"
    echo -e "   • Добавьте XML документацию для всех публичных классов"
    echo -e "   • Документируйте все публичные методы и свойства"
    echo -e "   • Используйте теги <summary>, <param>, <returns>, <remarks>"
    echo -e "   • Добавьте примеры использования в <example>"
fi

if [ $undocumented_methods -gt 0 ]; then
    echo -e "${YELLOW}⚠️  Найдено $undocumented_methods недокументированных методов${NC}"
    echo -e "${BLUE}💡 Рекомендации:${NC}"
    echo -e "   • Документируйте все публичные методы"
    echo -e "   • Описывайте параметры и возвращаемые значения"
    echo -e "   • Указывайте исключения в <exception>"
fi

echo ""
echo "🎯 СТАТУС ДОКУМЕНТАЦИИ"
echo "======================"

if [ "$documentation_percentage" -ge 90 ]; then
    echo -e "${GREEN}🏆 ПРЕВОСХОДНАЯ ДОКУМЕНТАЦИЯ${NC}"
    echo -e "${GREEN}✅ Проект хорошо документирован${NC}"
    echo -e "${GREEN}✅ Готов к продакшену${NC}"
elif [ "$documentation_percentage" -ge 70 ]; then
    echo -e "${YELLOW}⚠️  ХОРОШАЯ ДОКУМЕНТАЦИЯ${NC}"
    echo -e "${YELLOW}⚠️  Есть место для улучшения${NC}"
    echo -e "${BLUE}💡 Рекомендуется добавить документацию для оставшихся файлов${NC}"
else
    echo -e "${RED}❌ НЕДОСТАТОЧНАЯ ДОКУМЕНТАЦИЯ${NC}"
    echo -e "${RED}❌ Требуется значительная работа по документированию${NC}"
    echo -e "${BLUE}💡 Используйте этот инструмент для автоматического исправления${NC}"
fi

echo ""
echo "🔧 ИНСТРУМЕНТЫ ДЛЯ УЛУЧШЕНИЯ"
echo "============================"
echo -e "${CYAN}• Visual Studio:${NC} Автоматическая генерация XML документации"
echo -e "${CYAN}• JetBrains Rider:${NC} Встроенные инструменты документирования"
echo -e "${CYAN}• DocFX:${NC} Генерация веб-документации из XML комментариев"
echo -e "${CYAN}• Sandcastle:${NC} Создание справки из XML документации"

echo ""
echo "✅ АНАЛИЗ ДОКУМЕНТАЦИИ ЗАВЕРШЕН"
echo "=============================="
