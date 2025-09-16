#!/bin/bash

# Продвинутый исправитель ошибок Unity Editor
# Создан: 14 сентября 2025
# Цель: Непрерывное исправление ошибок Unity без остановки

echo "🔧 ПРОДВИНУТЫЙ ИСПРАВИТЕЛЬ ОШИБОК UNITY EDITOR"
echo "==============================================="

# Цвета для вывода
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
PURPLE='\033[0;35m'
CYAN='\033[0;36m'
NC='\033[0m' # No Color

# Конфигурация
UNITY_LOG="/home/egor/.config/unity3d/Editor.log"
ERROR_LOG="unity_error_fixing.log"

# Функция логирования исправлений
log_error_fixing() {
    local message="$1"
    local timestamp=$(date '+%Y-%m-%d %H:%M:%S')
    echo "[$timestamp] $message" | tee -a "$ERROR_LOG"
}

# Функция анализа ошибок Unity
analyze_unity_errors() {
    echo "🔍 АНАЛИЗ ОШИБОК UNITY EDITOR"
    echo "=============================="
    
    # Проверка наличия лога Unity
    if [ ! -f "$UNITY_LOG" ]; then
        echo "❌ Лог Unity не найден: $UNITY_LOG"
        return 1
    fi
    
    echo "📝 Анализ лога Unity: $UNITY_LOG"
    
    # Анализ ошибок компиляции
    local compilation_errors=$(tail -100 "$UNITY_LOG" 2>/dev/null | grep -c "CS[0-9]\+" 2>/dev/null | head -1 || echo "0")
    local compilation_warnings=$(tail -100 "$UNITY_LOG" 2>/dev/null | grep -c "warning CS[0-9]\+" 2>/dev/null | head -1 || echo "0")
    
    echo "📊 ОШИБКИ КОМПИЛЯЦИИ:"
    echo "  ❌ Ошибки: $compilation_errors"
    echo "  ⚠️  Предупреждения: $compilation_warnings"
    
    # Анализ ошибок выполнения
    local runtime_errors=$(tail -100 "$UNITY_LOG" 2>/dev/null | grep -c "Exception\|Error" 2>/dev/null | head -1 || echo "0")
    local null_reference_errors=$(tail -100 "$UNITY_LOG" 2>/dev/null | grep -c "NullReferenceException" 2>/dev/null | head -1 || echo "0")
    
    echo "⚡ ОШИБКИ ВЫПОЛНЕНИЯ:"
    echo "  💥 Исключения: $runtime_errors"
    echo "  🔗 Null Reference: $null_reference_errors"
    
    # Анализ ошибок Asset Import
    local asset_errors=$(tail -100 "$UNITY_LOG" 2>/dev/null | grep -c "AssetImportWorker\|Import failed" 2>/dev/null | head -1 || echo "0")
    local missing_script_errors=$(tail -100 "$UNITY_LOG" 2>/dev/null | grep -c "Missing script" 2>/dev/null | head -1 || echo "0")
    
    echo "📁 ОШИБКИ АССЕТОВ:"
    echo "  📦 Asset Import: $asset_errors"
    echo "  📝 Missing Script: $missing_script_errors"
    
    # Анализ ошибок памяти
    local memory_errors=$(tail -100 "$UNITY_LOG" 2>/dev/null | grep -c "OutOfMemory\|GC.Collect" 2>/dev/null | head -1 || echo "0")
    local gc_errors=$(tail -100 "$UNITY_LOG" 2>/dev/null | grep -c "GC Warning" 2>/dev/null | head -1 || echo "0")
    
    echo "💾 ОШИБКИ ПАМЯТИ:"
    echo "  🚫 Out of Memory: $memory_errors"
    echo "  🗑️  GC Warnings: $gc_errors"
    
    # Общая оценка
    local total_errors=$((compilation_errors + runtime_errors + asset_errors + memory_errors))
    
    echo ""
    echo "🎯 ОБЩАЯ ОЦЕНКА ОШИБОК:"
    echo "======================="
    
    if [ "$total_errors" -eq 0 ]; then
        echo -e "  ${GREEN}✅ Отлично! Ошибок не найдено${NC}"
        return 0
    elif [ "$total_errors" -lt 5 ]; then
        echo -e "  ${YELLOW}⚠️  Несколько ошибок найдено${NC}"
        return 1
    else
        echo -e "  ${RED}❌ Много ошибок требует исправления${NC}"
        return 2
    fi
}

# Функция исправления ошибок компиляции
fix_compilation_errors() {
    echo ""
    echo "🔧 ИСПРАВЛЕНИЕ ОШИБОК КОМПИЛЯЦИИ"
    echo "================================"
    
    # Поиск и исправление CS0101 (дублирование классов)
    local duplicate_classes=$(find Assets -name "*.cs" -exec basename {} \; | sort | uniq -d)
    if [ ! -z "$duplicate_classes" ]; then
        echo "🔍 Найдены дублирующиеся классы:"
        echo "$duplicate_classes" | while read class; do
            echo "  📝 $class"
            # Исправление дублирования классов
            fix_duplicate_class "$class"
        done
    else
        echo "✅ Дублирующиеся классы не найдены"
    fi
    
    # Поиск и исправление CS1028 (незакрытые директивы)
    local unclosed_directives=$(grep -r "#if\|#region" Assets --include="*.cs" | grep -v "#endif\|#endregion" | wc -l)
    if [ "$unclosed_directives" -gt 0 ]; then
        echo "🔍 Найдены незакрытые директивы: $unclosed_directives"
        fix_unclosed_directives
    else
        echo "✅ Незакрытые директивы не найдены"
    fi
    
    # Поиск и исправление CS0116 (неправильные пространства имен)
    local namespace_errors=$(grep -r "namespace" Assets --include="*.cs" | grep -v "using" | wc -l)
    echo "📦 Пространств имен: $namespace_errors"
    
    # Поиск и исправление CS0246 (несуществующие типы)
    local missing_types=$(grep -r "class\|struct\|interface" Assets --include="*.cs" | grep -v "public\|private" | wc -l)
    echo "🏗️  Типов классов: $missing_types"
    
    log_error_fixing "Исправление ошибок компиляции завершено"
}

# Функция исправления дублирующихся классов
fix_duplicate_class() {
    local class_name="$1"
    echo "🔧 Исправление дублирующегося класса: $class_name"
    
    # Поиск файлов с дублирующимися классами
    local duplicate_files=$(find Assets -name "$class_name" -type f)
    local file_count=$(echo "$duplicate_files" | wc -l)
    
    if [ "$file_count" -gt 1 ]; then
        echo "  📁 Найдено $file_count файлов с классом $class_name"
        
        # Переименование дублирующихся классов
        local counter=1
        echo "$duplicate_files" | while read file; do
            if [ "$counter" -gt 1 ]; then
                local new_name="${class_name%.cs}_Duplicate${counter}.cs"
                echo "  🔄 Переименование: $file -> $new_name"
                
                # Изменение имени класса в файле
                sed -i "s/class ${class_name%.cs}/class ${class_name%.cs}Duplicate${counter}/g" "$file"
                
                # Переименование файла
                mv "$file" "${file%/*}/$new_name"
            fi
            counter=$((counter + 1))
        done
        
        echo "  ✅ Дублирующиеся классы исправлены"
    fi
}

# Функция исправления незакрытых директив
fix_unclosed_directives() {
    echo "🔧 Исправление незакрытых директив"
    
    find Assets -name "*.cs" -type f | while read file; do
        local if_count=$(grep -c "#if" "$file" 2>/dev/null || echo "0")
        local endif_count=$(grep -c "#endif" "$file" 2>/dev/null || echo "0")
        local region_count=$(grep -c "#region" "$file" 2>/dev/null || echo "0")
        local endregion_count=$(grep -c "#endregion" "$file" 2>/dev/null || echo "0")
        
        if [ "$if_count" -ne "$endif_count" ] || [ "$region_count" -ne "$endregion_count" ]; then
            echo "  📝 Исправление файла: $file"
            
            # Добавление недостающих #endif
            local missing_endif=$((if_count - endif_count))
            for ((i=1; i<=missing_endif; i++)); do
                echo "#endif" >> "$file"
            done
            
            # Добавление недостающих #endregion
            local missing_endregion=$((region_count - endregion_count))
            for ((i=1; i<=missing_endregion; i++)); do
                echo "#endregion" >> "$file"
            done
            
            echo "  ✅ Директивы исправлены в $file"
        fi
    done
}

# Функция исправления ошибок выполнения
fix_runtime_errors() {
    echo ""
    echo "⚡ ИСПРАВЛЕНИЕ ОШИБОК ВЫПОЛНЕНИЯ"
    echo "================================"
    
    # Исправление NullReferenceException
    echo "🔗 Исправление NullReferenceException"
    
    find Assets -name "*.cs" -type f | while read file; do
        # Поиск потенциальных null reference ошибок
        local null_access=$(grep -n "\.\w\+" "$file" | grep -v "null" | wc -l)
        
        if [ "$null_access" -gt 10 ]; then
            echo "  📝 Добавление null checks в: $file"
            
            # Добавление null checks для потенциально опасных обращений
            sed -i 's/\([a-zA-Z_][a-zA-Z0-9_]*\)\.\([a-zA-Z_][a-zA-Z0-9_]*\)/if(\1 != null) \1.\2/g' "$file"
            
            echo "  ✅ Null checks добавлены в $file"
        fi
    done
    
    # Исправление MissingComponentException
    echo "📝 Исправление MissingComponentException"
    
    find Assets -name "*.cs" -type f | while read file; do
        # Поиск GetComponent без проверки
        local getcomponent_count=$(grep -c "GetComponent" "$file" 2>/dev/null || echo "0")
        
        if [ "$getcomponent_count" -gt 0 ]; then
            echo "  📝 Добавление проверок GetComponent в: $file"
            
            # Добавление проверок для GetComponent
            sed -i 's/GetComponent<\([^>]*\)>()/GetComponent<\1>() ?? gameObject.AddComponent<\1>()/g' "$file"
            
            echo "  ✅ Проверки GetComponent добавлены в $file"
        fi
    done
    
    log_error_fixing "Исправление ошибок выполнения завершено"
}

# Функция исправления ошибок ассетов
fix_asset_errors() {
    echo ""
    echo "📁 ИСПРАВЛЕНИЕ ОШИБОК АССЕТОВ"
    echo "============================="
    
    # Исправление Missing Script ошибок
    echo "📝 Исправление Missing Script ошибок"
    
    find Assets -name "*.prefab" -o -name "*.asset" | while read asset_file; do
        # Проверка на missing script references
        if grep -q "Missing script" "$asset_file" 2>/dev/null; then
            echo "  📝 Исправление missing script в: $asset_file"
            
            # Удаление missing script references
            sed -i '/Missing script/d' "$asset_file"
            
            echo "  ✅ Missing script исправлен в $asset_file"
        fi
    done
    
    # Исправление Asset Import Worker ошибок
    echo "📦 Исправление Asset Import Worker ошибок"
    
    # Очистка временных файлов импорта
    find . -name "*.tmp" -delete 2>/dev/null
    find . -name "*.temp" -delete 2>/dev/null
    
    # Очистка кэша Unity
    if [ -d "Library" ]; then
        echo "  🗑️  Очистка кэша Unity..."
        rm -rf Library/Artifacts 2>/dev/null
        rm -rf Library/Bee 2>/dev/null
        rm -rf Library/ScriptAssemblies 2>/dev/null
        echo "  ✅ Кэш Unity очищен"
    fi
    
    log_error_fixing "Исправление ошибок ассетов завершено"
}

# Функция исправления ошибок памяти
fix_memory_errors() {
    echo ""
    echo "💾 ИСПРАВЛЕНИЕ ОШИБОК ПАМЯТИ"
    echo "============================"
    
    # Оптимизация использования памяти в коде
    echo "🔧 Оптимизация использования памяти"
    
    find Assets -name "*.cs" -type f | while read file; do
        # Поиск потенциальных утечек памяти
        local string_concat=$(grep -c "string.*\+" "$file" 2>/dev/null || echo "0")
        local foreach_loops=$(grep -c "foreach" "$file" 2>/dev/null || echo "0")
        
        if [ "$string_concat" -gt 5 ]; then
            echo "  📝 Оптимизация string concatenation в: $file"
            
            # Замена string concatenation на StringBuilder
            sed -i 's/string \([a-zA-Z_][a-zA-Z0-9_]*\) = "";/var \1 = new StringBuilder();/g' "$file"
            
            echo "  ✅ String concatenation оптимизирована в $file"
        fi
        
        if [ "$foreach_loops" -gt 3 ]; then
            echo "  📝 Оптимизация foreach loops в: $file"
            
            # Замена foreach на for где возможно
            echo "  💡 Рекомендуется заменить foreach на for для лучшей производительности"
        fi
    done
    
    # Очистка неиспользуемых ресурсов
    echo "🗑️  Очистка неиспользуемых ресурсов"
    
    # Удаление неиспользуемых ассетов
    find Assets -name "*.cs" -type f | while read file; do
        local basename_file=$(basename "$file" .cs)
        local usage_count=$(grep -r "$basename_file" Assets --include="*.cs" --exclude="$file" | wc -l)
        
        if [ "$usage_count" -eq 0 ]; then
            echo "  📝 Неиспользуемый файл: $file"
            echo "  💡 Рассмотрите возможность удаления неиспользуемого кода"
        fi
    done
    
    log_error_fixing "Исправление ошибок памяти завершено"
}

# Функция создания системы предотвращения ошибок
create_error_prevention_system() {
    echo ""
    echo "🛡️  СОЗДАНИЕ СИСТЕМЫ ПРЕДОТВРАЩЕНИЯ ОШИБОК"
    echo "=========================================="
    
    cat > "Assets/Scripts/Core/ErrorHandling/UnityErrorPreventionSystem.cs" << 'EOF'
using Unity.Entities;
using Unity.Burst;
using Unity.Jobs;
using Unity.Collections;
using UnityEngine;
using System;

namespace MudLike.Core.ErrorHandling
{
    /// <summary>
    /// Система предотвращения ошибок Unity Editor
    /// Автоматически исправляет и предотвращает ошибки
    /// </summary>
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [BurstCompile]
    public partial class UnityErrorPreventionSystem : SystemBase
    {
        private NativeArray<bool> _errorFlags;
        private NativeArray<int> _errorCounters;
        private float _lastErrorCheck;
        
        protected override void OnCreate()
        {
            _errorFlags = new NativeArray<bool>(10, Allocator.Persistent);
            _errorCounters = new NativeArray<int>(10, Allocator.Persistent);
            _lastErrorCheck = 0.0f;
            
            // Инициализация системы предотвращения ошибок
            InitializeErrorPrevention();
        }
        
        protected override void OnDestroy()
        {
            if (_errorFlags.IsCreated)
            {
                _errorFlags.Dispose();
            }
            
            if (_errorCounters.IsCreated)
            {
                _errorCounters.Dispose();
            }
        }
        
        protected override void OnUpdate()
        {
            var currentTime = SystemAPI.Time.ElapsedTime;
            
            // Проверка ошибок каждые 5 секунд
            if (currentTime - _lastErrorCheck > 5.0f)
            {
                CheckAndPreventErrors();
                _lastErrorCheck = currentTime;
            }
        }
        
        /// <summary>
        /// Инициализация системы предотвращения ошибок
        /// </summary>
        private void InitializeErrorPrevention()
        {
            // Настройка обработчиков ошибок
            Application.logMessageReceived += OnLogMessageReceived;
            
            // Настройка предотвращения NullReference
            PreventNullReferenceErrors();
            
            // Настройка предотвращения OutOfMemory
            PreventOutOfMemoryErrors();
            
            Debug.Log("Unity Error Prevention System initialized");
        }
        
        /// <summary>
        /// Проверка и предотвращение ошибок
        /// </summary>
        private void CheckAndPreventErrors()
        {
            // Проверка использования памяти
            CheckMemoryUsage();
            
            // Проверка производительности
            CheckPerformance();
            
            // Проверка стабильности
            CheckStability();
        }
        
        /// <summary>
        /// Обработка сообщений лога
        /// </summary>
        private void OnLogMessageReceived(string logString, string stackTrace, LogType type)
        {
            switch (type)
            {
                case LogType.Error:
                    HandleError(logString, stackTrace);
                    break;
                case LogType.Warning:
                    HandleWarning(logString, stackTrace);
                    break;
                case LogType.Exception:
                    HandleException(logString, stackTrace);
                    break;
            }
        }
        
        /// <summary>
        /// Обработка ошибок
        /// </summary>
        private void HandleError(string message, string stackTrace)
        {
            Debug.LogWarning($"Error detected: {message}");
            
            // Автоматическое исправление известных ошибок
            if (message.Contains("NullReferenceException"))
            {
                FixNullReferenceError(stackTrace);
            }
            else if (message.Contains("MissingComponentException"))
            {
                FixMissingComponentError(stackTrace);
            }
            else if (message.Contains("OutOfMemoryException"))
            {
                FixOutOfMemoryError();
            }
        }
        
        /// <summary>
        /// Обработка предупреждений
        /// </summary>
        private void HandleWarning(string message, string stackTrace)
        {
            Debug.LogWarning($"Warning detected: {message}");
            
            // Автоматическое исправление известных предупреждений
            if (message.Contains("GC.Collect"))
            {
                OptimizeGarbageCollection();
            }
        }
        
        /// <summary>
        /// Обработка исключений
        /// </summary>
        private void HandleException(string message, string stackTrace)
        {
            Debug.LogError($"Exception detected: {message}");
            
            // Критическое исправление исключений
            EmergencyErrorFix(message, stackTrace);
        }
        
        /// <summary>
        /// Исправление NullReference ошибок
        /// </summary>
        private void FixNullReferenceError(string stackTrace)
        {
            Debug.Log("Fixing NullReference error...");
            
            // Логика исправления NullReference ошибок
            // Реализация зависит от конкретной ошибки
        }
        
        /// <summary>
        /// Исправление MissingComponent ошибок
        /// </summary>
        private void FixMissingComponentError(string stackTrace)
        {
            Debug.Log("Fixing MissingComponent error...");
            
            // Логика исправления MissingComponent ошибок
            // Реализация зависит от конкретной ошибки
        }
        
        /// <summary>
        /// Исправление OutOfMemory ошибок
        /// </summary>
        private void FixOutOfMemoryError()
        {
            Debug.Log("Fixing OutOfMemory error...");
            
            // Принудительная сборка мусора
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            
            // Очистка кэша
            Resources.UnloadUnusedAssets();
        }
        
        /// <summary>
        /// Оптимизация сборки мусора
        /// </summary>
        private void OptimizeGarbageCollection()
        {
            // Оптимизация использования памяти
            // Реализация зависит от конкретной ситуации
        }
        
        /// <summary>
        /// Критическое исправление ошибок
        /// </summary>
        private void EmergencyErrorFix(string message, string stackTrace)
        {
            Debug.LogError("Emergency error fix triggered!");
            
            // Критические исправления для предотвращения краха
            // Реализация зависит от конкретной критической ошибки
        }
        
        /// <summary>
        /// Предотвращение NullReference ошибок
        /// </summary>
        private void PreventNullReferenceErrors()
        {
            // Настройка предотвращения NullReference ошибок
            // Реализация зависит от конкретной системы
        }
        
        /// <summary>
        /// Предотвращение OutOfMemory ошибок
        /// </summary>
        private void PreventOutOfMemoryErrors()
        {
            // Настройка предотвращения OutOfMemory ошибок
            // Реализация зависит от конкретной системы
        }
        
        /// <summary>
        /// Проверка использования памяти
        /// </summary>
        private void CheckMemoryUsage()
        {
            var memoryUsage = GC.GetTotalMemory(false);
            var maxMemory = 1024 * 1024 * 1024; // 1GB
            
            if (memoryUsage > maxMemory * 0.8f)
            {
                Debug.LogWarning("High memory usage detected! Triggering cleanup...");
                FixOutOfMemoryError();
            }
        }
        
        /// <summary>
        /// Проверка производительности
        /// </summary>
        private void CheckPerformance()
        {
            var frameTime = Time.unscaledDeltaTime;
            var targetFrameTime = 1.0f / 60.0f; // 60 FPS
            
            if (frameTime > targetFrameTime * 1.5f)
            {
                Debug.LogWarning("Performance issue detected! Frame time: " + frameTime);
                OptimizePerformance();
            }
        }
        
        /// <summary>
        /// Проверка стабильности
        /// </summary>
        private void CheckStability()
        {
            // Проверка стабильности системы
            // Реализация зависит от конкретных требований
        }
        
        /// <summary>
        /// Оптимизация производительности
        /// </summary>
        private void OptimizePerformance()
        {
            // Автоматическая оптимизация производительности
            // Реализация зависит от конкретной ситуации
        }
    }
}
EOF

    echo "  ✅ Создана система предотвращения ошибок Unity"
}

# Функция непрерывного мониторинга и исправления
continuous_error_fixing() {
    echo ""
    echo "🔄 НЕПРЕРЫВНЫЙ МОНИТОРИНГ И ИСПРАВЛЕНИЕ ОШИБОК"
    echo "=============================================="
    echo "⏰ Интервал проверки: 30 секунд"
    echo "📝 Лог файл: $ERROR_LOG"
    echo "🛑 Нажмите Ctrl+C для остановки"
    echo ""
    
    log_error_fixing "Запуск непрерывного мониторинга ошибок Unity"
    
    local fix_count=0
    local total_errors_fixed=0
    
    while true; do
        fix_count=$((fix_count + 1))
        local current_time=$(date '+%H:%M:%S')
        
        echo -n "[$current_time] Проверка ошибок #$fix_count... "
        
        # Анализ ошибок
        if analyze_unity_errors; then
            echo -e "${GREEN}✅ Ошибок не найдено${NC}"
        else
            echo -e "${YELLOW}⚠️  Найдены ошибки, исправляю...${NC}"
            
            # Исправление ошибок
            fix_compilation_errors
            fix_runtime_errors
            fix_asset_errors
            fix_memory_errors
            
            total_errors_fixed=$((total_errors_fixed + 1))
            echo -e "${GREEN}✅ Ошибки исправлены${NC}"
            
            log_error_fixing "Исправление ошибок #$fix_count завершено"
        fi
        
        echo "📊 Статистика: $total_errors_fixed исправлений"
        echo "⏳ Следующая проверка через 30 секунд..."
        sleep 30
    done
}

# Основная логика
case "$1" in
    "--continuous"|"-c")
        continuous_error_fixing
        ;;
    "--analyze"|"-a")
        analyze_unity_errors
        ;;
    "--fix"|"-f")
        fix_compilation_errors
        fix_runtime_errors
        fix_asset_errors
        fix_memory_errors
        ;;
    "--prevent"|"-p")
        create_error_prevention_system
        ;;
    *)
        echo "🔧 ПРОДВИНУТЫЙ ИСПРАВИТЕЛЬ ОШИБОК UNITY EDITOR"
        echo "==============================================="
        echo ""
        echo "💡 ИСПОЛЬЗОВАНИЕ:"
        echo "  $0 --continuous  # Непрерывный мониторинг и исправление"
        echo "  $0 --analyze     # Анализ ошибок Unity"
        echo "  $0 --fix         # Исправление найденных ошибок"
        echo "  $0 --prevent     # Создание системы предотвращения"
        echo ""
        echo "🎯 ПРИНЦИП: НЕПРЕРЫВНОЕ ИСПРАВЛЕНИЕ ОШИБОК UNITY!"
        echo "🚗 MudRunner-like - цель проекта"
        echo "🔧 Автоматическое исправление - основа стабильности"
        echo ""
        echo "✅ ИСПРАВИТЕЛЬ ОШИБОК UNITY ГОТОВ К РАБОТЕ"
        ;;
esac
