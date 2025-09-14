#!/bin/bash
# Скрипт проверки отсутствующих файлов в проекте Unity

echo "🔍 Проверка отсутствующих файлов в проекте Mud-Like..."
echo "=================================================="

check_missing_files() {
    echo "📋 Анализ логов компиляции на предмет отсутствующих файлов..."
    
    # Список логов для анализа
    log_files=("unity_build.log" "prototype_build.log" "prototype_build2.log" "prototype_build3.log" "test_build.log")
    
    missing_files_count=0
    found_files_count=0
    
    for log_file in "${log_files[@]}"; do
        if [ -f "$log_file" ]; then
            echo "📄 Анализ лога: $log_file"
            
            # Извлечение путей к файлам из ошибок компиляции
            referenced_files=$(grep -o "Assets/[^:]*\.cs" "$log_file" 2>/dev/null | sort -u)
            
            for file_path in $referenced_files; do
                if [ ! -f "$file_path" ]; then
                    echo "❌ ОТСУТСТВУЕТ: $file_path"
                    missing_files_count=$((missing_files_count + 1))
                else
                    echo "✅ НАЙДЕН: $file_path"
                    found_files_count=$((found_files_count + 1))
                fi
            done
        else
            echo "⚠️  Лог не найден: $log_file"
        fi
    done
    
    echo ""
    echo "📊 СТАТИСТИКА:"
    echo "   Найденных файлов: $found_files_count"
    echo "   Отсутствующих файлов: $missing_files_count"
    
    if [ $missing_files_count -gt 0 ]; then
        echo "🚫 НАЙДЕНЫ ОТСУТСТВУЮЩИЕ ФАЙЛЫ!"
        return 1
    else
        echo "✅ ВСЕ ФАЙЛЫ НАЙДЕНЫ!"
        return 0
    fi
}

check_baker_classes() {
    echo ""
    echo "🔍 Проверка Baker классов на корректность using директив..."
    echo "=========================================================="
    
    error_count=0
    
    for file in $(find Assets/Scripts -name "*.cs" -type f); do
        if [ -f "$file" ]; then
            # Проверка наличия Baker класса без using Unity.Entities
            if grep -q "Baker<" "$file" && ! grep -q "using Unity.Entities" "$file"; then
                echo "❌ $file: Baker класс без using Unity.Entities"
                error_count=$((error_count + 1))
            fi
            
            # Проверка наличия IConvertGameObjectToEntity без using Unity.Entities
            if grep -q "IConvertGameObjectToEntity" "$file" && ! grep -q "using Unity.Entities" "$file"; then
                echo "❌ $file: IConvertGameObjectToEntity без using Unity.Entities"
                error_count=$((error_count + 1))
            fi
        fi
    done
    
    if [ $error_count -gt 0 ]; then
        echo "🚫 НАЙДЕНЫ ОШИБКИ В $error_count ФАЙЛАХ!"
        echo "💡 Добавьте using Unity.Entities; в начало файлов"
        return 1
    else
        echo "✅ ВСЕ BAKER КЛАССЫ КОРРЕКТНЫ!"
        return 0
    fi
}

check_assembly_definitions() {
    echo ""
    echo "🔍 Проверка Assembly Definitions..."
    echo "=================================="
    
    error_count=0
    
    for asmdef in $(find Assets -name "*.asmdef" -type f); do
        echo "📋 Проверка: $asmdef"
        
        # Проверка наличия ссылки на Unity.Entities в файлах с Baker
        if grep -q "Baker<\|IConvertGameObjectToEntity" Assets/Scripts/$(basename "$asmdef" .asmdef)/*.cs 2>/dev/null; then
            if ! grep -q "Unity.Entities" "$asmdef"; then
                echo "❌ $asmdef: Отсутствует ссылка на Unity.Entities"
                error_count=$((error_count + 1))
            else
                echo "✅ $asmdef: Ссылка на Unity.Entities найдена"
            fi
        fi
    done
    
    if [ $error_count -gt 0 ]; then
        echo "🚫 НАЙДЕНЫ ОШИБКИ В $error_count ASSEMBLY DEFINITIONS!"
        return 1
    else
        echo "✅ ВСЕ ASSEMBLY DEFINITIONS КОРРЕКТНЫ!"
        return 0
    fi
}

# Запуск всех проверок
echo "🚀 Запуск комплексной проверки..."
echo ""

total_errors=0

if ! check_missing_files; then
    total_errors=$((total_errors + 1))
fi

if ! check_baker_classes; then
    total_errors=$((total_errors + 1))
fi

if ! check_assembly_definitions; then
    total_errors=$((total_errors + 1))
fi

echo ""
echo "🎯 ИТОГОВЫЙ РЕЗУЛЬТАТ:"
if [ $total_errors -eq 0 ]; then
    echo "🎉 ВСЕ ПРОВЕРКИ ПРОЙДЕНЫ УСПЕШНО!"
    echo "✅ Проект готов к компиляции"
    exit 0
else
    echo "🚫 НАЙДЕНЫ ОШИБКИ В $total_errors КАТЕГОРИЯХ!"
    echo "❌ Требуется исправление перед компиляцией"
    exit 1
fi
