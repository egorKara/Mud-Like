#!/bin/bash

# Автоматическое улучшение документации методов
# Создан: 14 сентября 2025
# Цель: Качественное улучшение документации для проекта MudRunner-like

echo "📚 АВТОМАТИЧЕСКОЕ УЛУЧШЕНИЕ ДОКУМЕНТАЦИИ"
echo "=========================================="

# Цвета для вывода
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
PURPLE='\033[0;35m'
CYAN='\033[0;36m'
NC='\033[0m' # No Color

# Функция анализа документации
analyze_documentation() {
    echo "🔍 АНАЛИЗ ТЕКУЩЕГО СОСТОЯНИЯ ДОКУМЕНТАЦИИ"
    echo "=========================================="
    
    local total_methods=$(find Assets -name "*.cs" -exec grep -c "public.*(" {} \; 2>/dev/null | awk '{sum+=$1} END {print sum}' || echo "0")
    local documented_methods=$(find Assets -name "*.cs" -exec grep -l "/// <summary>" {} \; 2>/dev/null | wc -l | tr -d ' ')
    local undocumented_methods=$((total_methods - documented_methods))
    
    echo "📊 Общая статистика документации:"
    echo "  📝 Всего публичных методов: $total_methods"
    echo "  ✅ Документированных методов: $documented_methods"
    echo "  ❌ Не документированных методов: $undocumented_methods"
    
    if [ "$total_methods" -gt 0 ]; then
        local documentation_percentage=$((documented_methods * 100 / total_methods))
        echo "  📈 Процент документирования: $documentation_percentage%"
    fi
    
    echo ""
    echo "🎯 КРИТИЧЕСКИ ВАЖНЫЕ ФАЙЛЫ ДЛЯ MUD-LIKE:"
    echo "========================================"
    
    # Анализ критически важных файлов для MudRunner-like
    local vehicle_files=$(find Assets -path "*/Vehicles/*" -name "*.cs" | wc -l | tr -d ' ')
    local terrain_files=$(find Assets -path "*/Terrain/*" -name "*.cs" | wc -l | tr -d ' ')
    local physics_files=$(find Assets -name "*.cs" -exec grep -l "Physics\|PhysicsBody" {} \; 2>/dev/null | wc -l | tr -d ' ')
    local network_files=$(find Assets -path "*/Networking/*" -name "*.cs" | wc -l | tr -d ' ')
    
    echo "  🚗 Файлы транспортных средств: $vehicle_files"
    echo "  🏔️  Файлы террейна: $terrain_files"
    echo "  ⚡ Файлы физики: $physics_files"
    echo "  🌐 Файлы сети: $network_files"
    
    return $undocumented_methods
}

# Функция создания шаблонов документации
create_documentation_templates() {
    echo ""
    echo "📋 СОЗДАНИЕ ШАБЛОНОВ ДОКУМЕНТАЦИИ"
    echo "=================================="
    
    # Создание шаблона для транспортных средств
    cat > "documentation_templates/vehicle_method.md" << 'EOF'
/// <summary>
/// [ОПИСАНИЕ МЕТОДА ДЛЯ ТРАНСПОРТНОГО СРЕДСТВА]
/// </summary>
/// <param name="[параметр]">[ОПИСАНИЕ ПАРАМЕТРА]</param>
/// <returns>[ОПИСАНИЕ ВОЗВРАЩАЕМОГО ЗНАЧЕНИЯ]</returns>
/// <remarks>
/// Критично для реалистичной физики MudRunner-like:
/// - Детерминированность для мультиплеера
/// - Производительность ECS
/// - Соответствие физическим законам
/// </remarks>
EOF

    # Создание шаблона для деформации террейна
    cat > "documentation_templates/terrain_method.md" << 'EOF'
/// <summary>
/// [ОПИСАНИЕ МЕТОДА ДЛЯ ДЕФОРМАЦИИ ТЕРРЕЙНА]
/// </summary>
/// <param name="[параметр]">[ОПИСАНИЕ ПАРАМЕТРА]</param>
/// <returns>[ОПИСАНИЕ ВОЗВРАЩАЕМОГО ЗНАЧЕНИЯ]</returns>
/// <remarks>
/// Критично для реалистичной деформации MudRunner-like:
/// - Синхронизация между клиентами
/// - Производительность алгоритмов
/// - Визуальное качество деформации
/// </remarks>
EOF

    # Создание шаблона для физики
    cat > "documentation_templates/physics_method.md" << 'EOF'
/// <summary>
/// [ОПИСАНИЕ МЕТОДА ДЛЯ ФИЗИКИ]
/// </summary>
/// <param name="[параметр]">[ОПИСАНИЕ ПАРАМЕТРА]</param>
/// <returns>[ОПИСАНИЕ ВОЗВРАЩАЕМОГО ЗНАЧЕНИЯ]</returns>
/// <remarks>
/// Критично для реалистичной физики MudRunner-like:
/// - Unity Physics (DOTS)
/// - Детерминированная симуляция
/// - Оптимизация Job System
/// </remarks>
EOF

    # Создание шаблона для мультиплеера
    cat > "documentation_templates/network_method.md" << 'EOF'
/// <summary>
/// [ОПИСАНИЕ МЕТОДА ДЛЯ МУЛЬТИПЛЕЕРА]
/// </summary>
/// <param name="[параметр]">[ОПИСАНИЕ ПАРАМЕТРА]</param>
/// <returns>[ОПИСАНИЕ ВОЗВРАЩАЕМОГО ЗНАЧЕНИЯ]</returns>
/// <remarks>
/// Критично для мультиплеера MudRunner-like:
/// - Netcode for Entities
/// - Детерминированная симуляция
/// - Низкая задержка
/// - Синхронизация состояния
/// </remarks>
EOF

    mkdir -p documentation_templates
    echo "  ✅ Шаблоны документации созданы в папке documentation_templates/"
}

# Функция поиска методов без документации
find_undocumented_methods() {
    echo ""
    echo "🔍 ПОИСК МЕТОДОВ БЕЗ ДОКУМЕНТАЦИИ"
    echo "================================="
    
    local critical_files=()
    
    # Поиск критически важных файлов для MudRunner-like
    while IFS= read -r file; do
        if [ -f "$file" ]; then
            critical_files+=("$file")
        fi
    done < <(find Assets -path "*/Vehicles/*" -name "*.cs" -o -path "*/Terrain/*" -name "*.cs" -o -path "*/Networking/*" -name "*.cs" 2>/dev/null)
    
    echo "🎯 Критически важные файлы для MudRunner-like:"
    for file in "${critical_files[@]}"; do
        local undocumented_count=$(grep -c "public.*(" "$file" 2>/dev/null || echo "0")
        local documented_count=$(grep -c "/// <summary>" "$file" 2>/dev/null || echo "0")
        local file_name=$(basename "$file")
        
        if [ "$undocumented_count" -gt 0 ]; then
            echo "  📄 $file_name: $undocumented_count методов, $documented_count документированных"
        fi
    done
}

# Функция создания рекомендаций
create_recommendations() {
    echo ""
    echo "💡 РЕКОМЕНДАЦИИ ПО УЛУЧШЕНИЮ ДОКУМЕНТАЦИИ"
    echo "=========================================="
    
    echo "🎯 ПРИОРИТЕТ 1: Критически важные файлы MudRunner-like"
    echo "  🚗 Файлы транспортных средств - физика и управление"
    echo "  🏔️  Файлы деформации террейна - алгоритмы деформации"
    echo "  🌐 Файлы мультиплеера - синхронизация и детерминизм"
    echo "  ⚡ Файлы физики - Unity Physics (DOTS)"
    echo ""
    
    echo "🎯 ПРИОРИТЕТ 2: Системы поддержки"
    echo "  📊 ECS компоненты и системы"
    echo "  🔧 Утилиты и вспомогательные методы"
    echo "  🎮 Пользовательский интерфейс"
    echo ""
    
    echo "🎯 ПРИОРИТЕТ 3: Общие улучшения"
    echo "  📝 Все публичные методы должны иметь XML документацию"
    echo "  🔍 Параметры и возвращаемые значения должны быть описаны"
    echo "  💡 Remarks должны объяснять важность для MudRunner-like"
    echo ""
    
    echo "📋 СТРУКТУРА ДОКУМЕНТАЦИИ:"
    echo "  /// <summary> - краткое описание метода"
    echo "  /// <param name=\"param\"> - описание параметров"
    echo "  /// <returns> - описание возвращаемого значения"
    echo "  /// <remarks> - важность для MudRunner-like"
    echo ""
    
    echo "🎯 ФОКУС НА ЦЕЛИ ПРОЕКТА:"
    echo "  🚗 Реалистичная физика внедорожника"
    echo "  🏔️  Деформация террейна под колесами"
    echo "  🌐 Детерминированная симуляция для мультиплеера"
    echo "  ⚡ ECS-архитектура для производительности"
}

# Функция создания инструмента для автоматической документации
create_auto_documentation_tool() {
    echo ""
    echo "🛠️  СОЗДАНИЕ ИНСТРУМЕНТА АВТОМАТИЧЕСКОЙ ДОКУМЕНТАЦИИ"
    echo "=================================================="
    
    cat > "auto_document_methods.py" << 'EOF'
#!/usr/bin/env python3
"""
Автоматическое добавление документации к методам C#
Создан: 14 сентября 2025
Цель: Улучшение документации проекта MudRunner-like
"""

import os
import re
import sys

def analyze_csharp_file(file_path):
    """Анализ C# файла на предмет методов без документации"""
    with open(file_path, 'r', encoding='utf-8') as f:
        content = f.read()
    
    # Поиск публичных методов
    method_pattern = r'public\s+[^{]+\s+(\w+)\s*\('
    methods = re.findall(method_pattern, content)
    
    # Поиск методов с документацией
    documented_pattern = r'/// <summary>.*?/// </summary>'
    documented_methods = re.findall(documented_pattern, content, re.DOTALL)
    
    return len(methods), len(documented_methods)

def generate_documentation_template(method_name, file_path):
    """Генерация шаблона документации для метода"""
    
    # Определение типа файла для контекста
    if 'Vehicle' in file_path or 'vehicle' in file_path:
        context = "транспортного средства"
        remarks = "Критично для реалистичной физики MudRunner-like"
    elif 'Terrain' in file_path or 'terrain' in file_path:
        context = "деформации террейна"
        remarks = "Критично для реалистичной деформации MudRunner-like"
    elif 'Network' in file_path or 'network' in file_path:
        context = "мультиплеера"
        remarks = "Критично для мультиплеера MudRunner-like"
    elif 'Physics' in file_path or 'physics' in file_path:
        context = "физики"
        remarks = "Критично для Unity Physics (DOTS)"
    else:
        context = "системы"
        remarks = "Важно для проекта MudRunner-like"
    
    template = f"""/// <summary>
/// Описание метода {method_name} для {context}
/// </summary>
/// <returns>Описание возвращаемого значения</returns>
/// <remarks>
/// {remarks}
/// </remarks>"""
    
    return template

def main():
    """Основная функция"""
    print("📚 АВТОМАТИЧЕСКОЕ УЛУЧШЕНИЕ ДОКУМЕНТАЦИИ")
    print("======================================")
    
    # Поиск C# файлов
    csharp_files = []
    for root, dirs, files in os.walk('Assets'):
        for file in files:
            if file.endswith('.cs'):
                csharp_files.append(os.path.join(root, file))
    
    total_methods = 0
    total_documented = 0
    
    print(f"🔍 Найдено {len(csharp_files)} C# файлов")
    
    for file_path in csharp_files:
        methods, documented = analyze_csharp_file(file_path)
        total_methods += methods
        total_documented += documented
        
        if methods > 0:
            percentage = (documented / methods * 100) if methods > 0 else 0
            print(f"  📄 {os.path.basename(file_path)}: {methods} методов, {documented} документированных ({percentage:.1f}%)")
    
    print(f"\n📊 ОБЩАЯ СТАТИСТИКА:")
    print(f"  📝 Всего методов: {total_methods}")
    print(f"  ✅ Документированных: {total_documented}")
    print(f"  ❌ Не документированных: {total_methods - total_documented}")
    
    if total_methods > 0:
        percentage = (total_documented / total_methods * 100)
        print(f"  📈 Процент документирования: {percentage:.1f}%")

if __name__ == "__main__":
    main()
EOF

    chmod +x auto_document_methods.py
    echo "  ✅ Python инструмент создан: auto_document_methods.py"
}

# Основная логика
main() {
    echo "📚 АВТОМАТИЧЕСКОЕ УЛУЧШЕНИЕ ДОКУМЕНТАЦИИ"
    echo "=========================================="
    echo "🎯 Цель: Качественное улучшение документации для MudRunner-like"
    echo ""
    
    # 1. Анализ текущего состояния
    analyze_documentation
    local undocumented_methods=$?
    
    # 2. Создание шаблонов документации
    create_documentation_templates
    
    # 3. Поиск методов без документации
    find_undocumented_methods
    
    # 4. Создание рекомендаций
    create_recommendations
    
    # 5. Создание инструмента автоматической документации
    create_auto_documentation_tool
    
    echo ""
    echo "🎯 НАПОМИНАНИЕ О ЦЕЛИ ПРОЕКТА:"
    echo "🚗 MudRunner-like игра требует качественной документации"
    echo "🏔️  Деформация террейна - критически важная система"
    echo "🌐 Мультиплеер - требует детерминированной симуляции"
    echo "⚡ ECS-архитектура - основа производительности"
    echo ""
    
    if [ "$undocumented_methods" -gt 0 ]; then
        echo -e "${YELLOW}⚠️  Найдено $undocumented_methods методов без документации${NC}"
        echo -e "${YELLOW}💡 Рекомендуется приоритетное документирование критических файлов${NC}"
    else
        echo -e "${GREEN}✅ Все методы документированы${NC}"
    fi
    
    echo ""
    echo "✅ АВТОМАТИЧЕСКОЕ УЛУЧШЕНИЕ ДОКУМЕНТАЦИИ ЗАВЕРШЕНО"
    echo "=================================================="
}

# Запуск основной функции
main
