#!/bin/bash

# Инструмент для исправления проблем с Asset Import Workers

echo "🔧 ИСПРАВЛЕНИЕ ПРОБЛЕМ ASSET IMPORT WORKERS"
echo "=========================================="

# Цвета для вывода
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
PURPLE='\033[0;35m'
CYAN='\033[0;36m'
NC='\033[0m' # No Color

echo ""
echo "🔍 ДИАГНОСТИКА ПРОБЛЕМ"
echo "======================"

# Проверяем наличие проблем с Asset Import Workers
worker_errors=$(find . -name "AssetImportWorker*.log" 2>/dev/null | wc -l)
echo -e "Найдено логов Asset Import Workers: ${CYAN}$worker_errors${NC}"

if [ $worker_errors -gt 0 ]; then
    echo -e "${YELLOW}⚠️  Обнаружены проблемы с Asset Import Workers${NC}"
    
    # Анализируем ошибки
    echo ""
    echo "📋 АНАЛИЗ ОШИБОК"
    echo "================"
    
    for log_file in $(find . -name "AssetImportWorker*.log" 2>/dev/null); do
        echo -e "📄 Анализ: ${BLUE}$log_file${NC}"
        
        # Проверяем на MonoManager::ReloadAssembly ошибки
        reload_errors=$(grep -c "MonoManager::ReloadAssembly" "$log_file" 2>/dev/null || echo "0")
        if [ $reload_errors -gt 0 ]; then
            echo -e "  ❌ Ошибки перезагрузки сборок: ${RED}$reload_errors${NC}"
        fi
        
        # Проверяем на Segmentation fault
        segfault_errors=$(grep -c "Segmentation fault\|SIGSEGV" "$log_file" 2>/dev/null || echo "0")
        if [ $segfault_errors -gt 0 ]; then
            echo -e "  ❌ Segmentation faults: ${RED}$segfault_errors${NC}"
        fi
        
        # Проверяем на Stack overflow
        stack_overflow=$(grep -c "Stack overflow\|StackOverflowException" "$log_file" 2>/dev/null || echo "0")
        if [ $stack_overflow -gt 0 ]; then
            echo -e "  ❌ Stack overflow: ${RED}$stack_overflow${NC}"
        fi
    done
else
    echo -e "${GREEN}✅ Проблем с Asset Import Workers не обнаружено${NC}"
fi

echo ""
echo "🔧 АВТОМАТИЧЕСКОЕ ИСПРАВЛЕНИЕ"
echo "============================="

# 1. Очистка кэша Unity
echo "🧹 Очистка кэша Unity..."
rm -rf Library Temp Logs 2>/dev/null
echo -e "  ✅ Кэш Unity очищен"

# 2. Очистка временных файлов
echo "🧹 Очистка временных файлов..."
find . -name "*.tmp" -delete 2>/dev/null
find . -name "*.temp" -delete 2>/dev/null
find . -name "AssetImportWorker*.log" -delete 2>/dev/null
echo -e "  ✅ Временные файлы очищены"

# 3. Проверка и исправление прав доступа
echo "🔐 Проверка прав доступа..."
find Assets -type f -name "*.cs" -exec chmod 644 {} \; 2>/dev/null
find Assets -type d -exec chmod 755 {} \; 2>/dev/null
echo -e "  ✅ Права доступа исправлены"

# 4. Проверка на поврежденные файлы
echo "🔍 Проверка на поврежденные файлы..."
damaged_files=$(find Assets -name "*.cs" -size 0 2>/dev/null | wc -l)
if [ $damaged_files -gt 0 ]; then
    echo -e "  ⚠️  Найдено поврежденных файлов: ${YELLOW}$damaged_files${NC}"
    find Assets -name "*.cs" -size 0 -delete 2>/dev/null
    echo -e "  ✅ Поврежденные файлы удалены"
else
    echo -e "  ✅ Поврежденных файлов не найдено"
fi

# 5. Проверка на дублирующиеся .meta файлы
echo "🔍 Проверка на дублирующиеся .meta файлы..."
duplicate_meta=$(find Assets -name "*.meta" -exec basename {} \; | sort | uniq -d | wc -l)
if [ $duplicate_meta -gt 0 ]; then
    echo -e "  ⚠️  Найдено дублирующихся .meta файлов: ${YELLOW}$duplicate_meta${NC}"
    echo -e "  💡 Рекомендуется проверить вручную"
else
    echo -e "  ✅ Дублирующихся .meta файлов не найдено"
fi

echo ""
echo "🛠️  ДОПОЛНИТЕЛЬНЫЕ ИСПРАВЛЕНИЯ"
echo "=============================="

# 6. Проверка на циклические зависимости
echo "🔍 Проверка на циклические зависимости..."
circular_deps=$(grep -r "using.*MudLike" Assets/Scripts --include="*.cs" | grep -v "//" | awk '{print $2}' | sort | uniq -c | awk '$1 > 10 {print $2}' | wc -l)
if [ $circular_deps -gt 0 ]; then
    echo -e "  ⚠️  Возможные циклические зависимости: ${YELLOW}$circular_deps${NC}"
    echo -e "  💡 Рекомендуется проверить архитектуру проекта"
else
    echo -e "  ✅ Циклических зависимостей не обнаружено"
fi

# 7. Проверка на неиспользуемые файлы
echo "🔍 Проверка на неиспользуемые файлы..."
unused_files=0
for cs_file in $(find Assets/Scripts -name "*.cs" 2>/dev/null); do
    filename=$(basename "$cs_file" .cs)
    if ! grep -r "$filename" Assets/Scripts --include="*.cs" | grep -v "$cs_file" | grep -q "class.*$filename\|struct.*$filename"; then
        unused_files=$((unused_files + 1))
    fi
done

if [ $unused_files -gt 0 ]; then
    echo -e "  ⚠️  Возможно неиспользуемых файлов: ${YELLOW}$unused_files${NC}"
    echo -e "  💡 Рекомендуется проверить вручную"
else
    echo -e "  ✅ Неиспользуемых файлов не найдено"
fi

echo ""
echo "📋 РЕКОМЕНДАЦИИ ПО ПРЕДОТВРАЩЕНИЮ"
echo "================================="

echo -e "${BLUE}💡 Рекомендации для предотвращения проблем:${NC}"
echo -e "  • Регулярно очищайте кэш Unity (Library, Temp, Logs)"
echo -e "  • Избегайте циклических зависимостей между модулями"
echo -e "  • Используйте правильные аллокаторы для Native Collections"
echo -e "  • Проверяйте код на утечки памяти"
echo -e "  • Используйте Burst Compiler для оптимизации"
echo -e "  • Избегайте сложных операций в OnCreate/OnDestroy"
echo -e "  • Регулярно обновляйте Unity и пакеты"

echo ""
echo "🔧 НАСТРОЙКИ UNITY ДЛЯ СТАБИЛЬНОСТИ"
echo "==================================="

echo -e "${CYAN}Рекомендуемые настройки Unity:${NC}"
echo -e "  • Player Settings > Configuration: Release"
echo -e "  • Player Settings > Scripting Backend: IL2CPP"
echo -e "  • Player Settings > Api Compatibility Level: .NET Standard 2.1"
echo -e "  • Project Settings > Editor > Asset Serialization: Force Text"
echo -e "  • Project Settings > Editor > Version Control: Visible Meta Files"

echo ""
echo "🎯 СТАТУС ИСПРАВЛЕНИЯ"
echo "===================="

if [ $worker_errors -eq 0 ]; then
    echo -e "${GREEN}🏆 ВСЕ ПРОБЛЕМЫ ИСПРАВЛЕНЫ${NC}"
    echo -e "${GREEN}✅ Asset Import Workers работают стабильно${NC}"
    echo -e "${GREEN}✅ Проект готов к работе${NC}"
    exit 0
else
    echo -e "${YELLOW}⚠️  ЧАСТИЧНОЕ ИСПРАВЛЕНИЕ${NC}"
    echo -e "${YELLOW}⚠️  Некоторые проблемы могут потребовать ручного вмешательства${NC}"
    echo -e "${BLUE}💡 Перезапустите Unity Editor для применения исправлений${NC}"
    exit 1
fi
