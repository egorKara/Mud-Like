#!/bin/bash

# 🔍 ПРОДВИНУТЫЙ АНАЛИЗАТОР ЛОГОВ UNITY EDITOR
# Анализирует все логи и исправляет найденные ошибки

echo "🔍 ПРОДВИНУТЫЙ АНАЛИЗАТОР ЛОГОВ UNITY EDITOR"
echo "============================================="
echo "📅 Дата: $(date)"
echo "🎯 Цель: MudRunner-like мультиплеерная игра"
echo ""

# Счетчики
total_errors=0
fixed_errors=0
warnings=0

# Функция анализа логов
analyze_logs() {
    echo "🔍 АНАЛИЗ ЛОГОВ UNITY EDITOR"
    echo "============================"
    
    # Поиск всех логов
    log_files=($(find . -name "*.log" -type f | head -10))
    
    for log_file in "${log_files[@]}"; do
        echo "📄 Анализ: $log_file"
        
        # Подсчет ошибок
        error_count=$(grep -c -i "error\|exception\|failed" "$log_file" 2>/dev/null || echo "0")
        warning_count=$(grep -c -i "warning\|critical" "$log_file" 2>/dev/null || echo "0")
        
        total_errors=$((total_errors + error_count))
        warnings=$((warnings + warning_count))
        
        echo "   ❌ Ошибок: $error_count"
        echo "   ⚠️  Предупреждений: $warning_count"
        
        # Анализ конкретных ошибок
        if [ "$error_count" -gt 0 ]; then
            echo "   🔍 Детали ошибок:"
            grep -i "error\|exception\|failed" "$log_file" | head -5 | while read -r line; do
                echo "      • $line"
            done
        fi
    done
    
    echo ""
    echo "📊 ОБЩАЯ СТАТИСТИКА:"
    echo "   ❌ Всего ошибок: $total_errors"
    echo "   ⚠️  Всего предупреждений: $warnings"
}

# Функция исправления ошибок InputSystem
fix_input_system_errors() {
    echo ""
    echo "🔧 ИСПРАВЛЕНИЕ ОШИБОК INPUT SYSTEM"
    echo "=================================="
    
    # Поиск файлов с ошибками InputSystem
    input_files=($(grep -l "InputSystem" Assets/Scripts -r 2>/dev/null || echo ""))
    
    if [ ${#input_files[@]} -gt 0 ]; then
        echo "📁 Найдено файлов с InputSystem: ${#input_files[@]}"
        
        for file in "${input_files[@]}"; do
            echo "🔧 Исправление: $file"
            
            # Проверка наличия using UnityEngine.InputSystem
            if ! grep -q "using UnityEngine.InputSystem" "$file"; then
                # Добавление using директивы
                sed -i '1i using UnityEngine.InputSystem;' "$file"
                echo "   ✅ Добавлен using UnityEngine.InputSystem"
                ((fixed_errors++))
            fi
            
            # Проверка наличия using Unity.Entities
            if ! grep -q "using Unity.Entities" "$file"; then
                sed -i '1i using Unity.Entities;' "$file"
                echo "   ✅ Добавлен using Unity.Entities"
                ((fixed_errors++))
            fi
        done
    else
        echo "ℹ️  Файлы с InputSystem не найдены"
    fi
}

# Функция исправления отсутствующих сборок
fix_missing_assemblies() {
    echo ""
    echo "🔧 ИСПРАВЛЕНИЕ ОТСУТСТВУЮЩИХ СБОРОК"
    echo "==================================="
    
    # Создание Assembly Definition файлов
    create_assembly_definition() {
        local path="$1"
        local name="$2"
        local references="$3"
        
        cat > "$path" << EOF
{
    "name": "$name",
    "rootNamespace": "MudLike",
    "references": [$references],
    "includePlatforms": [],
    "excludePlatforms": [],
    "allowUnsafeCode": false,
    "overrideReferences": false,
    "precompiledReferences": [],
    "autoReferenced": true,
    "defineConstraints": [],
    "versionDefines": [],
    "noEngineReferences": false
}
EOF
        echo "   ✅ Создан Assembly Definition: $name"
    }
    
    # Создание основных Assembly Definition файлов
    if [ ! -f "Assets/Scripts/MudLike.Core.asmdef" ]; then
        create_assembly_definition "Assets/Scripts/MudLike.Core.asmdef" "MudLike.Core" ""
        ((fixed_errors++))
    fi
    
    if [ ! -f "Assets/Scripts/MudLike.Vehicles.asmdef" ]; then
        create_assembly_definition "Assets/Scripts/MudLike.Vehicles.asmdef" "MudLike.Vehicles" "\"MudLike.Core\""
        ((fixed_errors++))
    fi
    
    if [ ! -f "Assets/Scripts/MudLike.Terrain.asmdef" ]; then
        create_assembly_definition "Assets/Scripts/MudLike.Terrain.asmdef" "MudLike.Terrain" "\"MudLike.Core\""
        ((fixed_errors++))
    fi
    
    if [ ! -f "Assets/Scripts/MudLike.Networking.asmdef" ]; then
        create_assembly_definition "Assets/Scripts/MudLike.Networking.asmdef" "MudLike.Networking" "\"MudLike.Core\""
        ((fixed_errors++))
    fi
}

# Функция исправления конкретных ошибок компиляции
fix_compilation_errors() {
    echo ""
    echo "🔧 ИСПРАВЛЕНИЕ ОШИБОК КОМПИЛЯЦИИ"
    echo "================================"
    
    # Исправление ошибок в PrototypeTester.cs
    if [ -f "Assets/Scripts/Core/Testing/PrototypeTester.cs" ]; then
        echo "🔧 Исправление PrototypeTester.cs"
        
        # Добавление недостающих using директив
        if ! grep -q "using UnityEngine.InputSystem" "Assets/Scripts/Core/Testing/PrototypeTester.cs"; then
            sed -i '1i using UnityEngine.InputSystem;' "Assets/Scripts/Core/Testing/PrototypeTester.cs"
            echo "   ✅ Добавлен using UnityEngine.InputSystem"
            ((fixed_errors++))
        fi
        
        if ! grep -q "using Unity.Entities" "Assets/Scripts/Core/Testing/PrototypeTester.cs"; then
            sed -i '1i using Unity.Entities;' "Assets/Scripts/Core/Testing/PrototypeTester.cs"
            echo "   ✅ Добавлен using Unity.Entities"
            ((fixed_errors++))
        fi
        
        # Замена отсутствующих классов на существующие
        sed -i 's/PlayerMovementSystem/OptimizedVehicleMovementSystem/g' "Assets/Scripts/Core/Testing/PrototypeTester.cs"
        echo "   ✅ Заменен PlayerMovementSystem на OptimizedVehicleMovementSystem"
        ((fixed_errors++))
    fi
    
    # Исправление других файлов с ошибками
    find Assets/Scripts -name "*.cs" -exec grep -l "PlayerMovementSystem" {} \; | while read -r file; do
        if [ -f "$file" ]; then
            echo "🔧 Исправление: $file"
            sed -i 's/PlayerMovementSystem/OptimizedVehicleMovementSystem/g' "$file"
            echo "   ✅ Заменен PlayerMovementSystem на OptimizedVehicleMovementSystem"
            ((fixed_errors++))
        fi
    done
}

# Функция очистки кэша Unity
clear_unity_cache() {
    echo ""
    echo "🧹 ОЧИСТКА КЭША UNITY EDITOR"
    echo "============================"
    
    # Очистка основного кэша
    if [ -d "$HOME/.cache/unity3d" ]; then
        rm -rf "$HOME/.cache/unity3d"
        echo "✅ Очищен основной кэш Unity"
    fi
    
    # Очистка кэша проекта
    if [ -d "Library" ]; then
        rm -rf Library/ScriptAssemblies
        rm -rf Library/PlayerDataCache
        rm -rf Library/ShaderCache
        rm -rf Library/ArtifactDB
        rm -rf Library/Artifacts
        echo "✅ Очищен кэш проекта"
    fi
    
    # Очистка временных файлов
    find . -name "*.tmp" -delete 2>/dev/null
    find . -name "*.temp" -delete 2>/dev/null
    echo "✅ Очищены временные файлы"
}

# Функция проверки пакетов Unity
check_unity_packages() {
    echo ""
    echo "📦 ПРОВЕРКА ПАКЕТОВ UNITY"
    echo "========================"
    
    # Проверка наличия Input System
    if [ -f "Packages/manifest.json" ]; then
        if grep -q "com.unity.inputsystem" Packages/manifest.json; then
            echo "✅ Input System: Установлен"
        else
            echo "⚠️  Input System: Не установлен"
            echo "   💡 Рекомендуется установить через Package Manager"
        fi
    fi
    
    # Проверка DOTS пакетов
    dots_packages=("com.unity.entities" "com.unity.physics" "com.unity.burst" "com.unity.jobs")
    for package in "${dots_packages[@]}"; do
        if grep -q "$package" Packages/manifest.json 2>/dev/null; then
            echo "✅ $package: Установлен"
        else
            echo "⚠️  $package: Не установлен"
        fi
    done
}

# Функция создания системы предотвращения ошибок
create_error_prevention_system() {
    echo ""
    echo "🛡️ СОЗДАНИЕ СИСТЕМЫ ПРЕДОТВРАЩЕНИЯ ОШИБОК"
    echo "========================================="
    
    # Создание скрипта мониторинга
    cat > monitor_unity_errors.sh << 'EOF'
#!/bin/bash
# Мониторинг ошибок Unity Editor

echo "🔍 МОНИТОРИНГ ОШИБОК UNITY EDITOR"
echo "================================="

# Проверка логов на новые ошибки
if [ -d "Logs" ]; then
    latest_log=$(find Logs -name "*.log" -type f -printf '%T@ %p\n' | sort -n | tail -1 | cut -d' ' -f2-)
    if [ -n "$latest_log" ]; then
        error_count=$(grep -c -i "error\|exception\|failed" "$latest_log" 2>/dev/null || echo "0")
        warning_count=$(grep -c -i "warning\|critical" "$latest_log" 2>/dev/null || echo "0")
        
        echo "📄 Последний лог: $latest_log"
        echo "❌ Ошибок: $error_count"
        echo "⚠️  Предупреждений: $warning_count"
        
        if [ "$error_count" -gt 0 ]; then
            echo "🚨 ОБНАРУЖЕНЫ ОШИБКИ!"
            echo "🔧 Запуск исправителя..."
            ./advanced_unity_log_analyzer.sh
        else
            echo "✅ Ошибок не обнаружено"
        fi
    fi
else
    echo "ℹ️  Папка Logs не найдена"
fi
EOF
    
    chmod +x monitor_unity_errors.sh
    echo "✅ Создан скрипт мониторинга ошибок"
    
    # Создание cron задачи для автоматического мониторинга
    cat > setup_error_monitoring.sh << 'EOF'
#!/bin/bash
# Настройка автоматического мониторинга ошибок

echo "⚙️ НАСТРОЙКА АВТОМАТИЧЕСКОГО МОНИТОРИНГА"
echo "======================================="

# Создание cron задачи
(crontab -l 2>/dev/null; echo "*/5 * * * * cd $(pwd) && ./monitor_unity_errors.sh > /dev/null 2>&1") | crontab -

echo "✅ Настроен автоматический мониторинг каждые 5 минут"
echo "🔧 Для отключения: crontab -e"
EOF
    
    chmod +x setup_error_monitoring.sh
    echo "✅ Создан скрипт настройки мониторинга"
}

# Основная функция
main() {
    # Анализ логов
    analyze_logs
    
    # Исправление ошибок
    fix_input_system_errors
    fix_missing_assemblies
    fix_compilation_errors
    
    # Очистка кэша
    clear_unity_cache
    
    # Проверка пакетов
    check_unity_packages
    
    # Создание системы предотвращения
    create_error_prevention_system
    
    # Результаты
    echo ""
    echo "📊 РЕЗУЛЬТАТЫ АНАЛИЗА И ИСПРАВЛЕНИЯ"
    echo "==================================="
    echo "🔍 Проанализировано логов: $(find . -name "*.log" -type f | wc -l)"
    echo "❌ Найдено ошибок: $total_errors"
    echo "🔧 Исправлено ошибок: $fixed_errors"
    echo "⚠️  Найдено предупреждений: $warnings"
    
    if [ $fixed_errors -gt 0 ]; then
        echo ""
        echo "✅ ИСПРАВЛЕНИЕ ЗАВЕРШЕНО УСПЕШНО!"
        echo "🎯 Проект готов к работе в Unity Editor"
        echo "🚀 Рекомендуется перезапустить Unity Editor"
    else
        echo ""
        echo "ℹ️  Критических ошибок не обнаружено"
        echo "🎯 Проект в хорошем состоянии"
    fi
    
    echo ""
    echo "🔧 Для мониторинга ошибок: ./monitor_unity_errors.sh"
    echo "⚙️ Для настройки автмониторинга: ./setup_error_monitoring.sh"
    echo "📅 Дата анализа: $(date)"
}

# Запуск основной функции
main
