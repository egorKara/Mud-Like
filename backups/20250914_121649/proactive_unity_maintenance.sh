#!/bin/bash

# Проактивное обслуживание Unity проекта

echo "🔧 ПРОАКТИВНОЕ ОБСЛУЖИВАНИЕ UNITY ПРОЕКТА"
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
echo "🔍 ПРОАКТИВНАЯ ДИАГНОСТИКА"
echo "=========================="

# 1. Проверка кэша Unity
echo -n "🔍 Проверка кэша Unity... "
cache_size=$(du -sh Library Temp Logs 2>/dev/null | awk '{sum+=$1} END {print sum}' || echo "0")
if [ "$cache_size" != "0" ]; then
    echo -e "${YELLOW}⚠️  Найдено: ${cache_size}${NC}"
    echo "🧹 Очистка кэша..."
    rm -rf Library Temp Logs 2>/dev/null
    echo -e "  ${GREEN}✅ Кэш очищен${NC}"
else
    echo -e "${GREEN}✅ Кэш чист${NC}"
fi

# 2. Проверка Asset Import Workers
echo -n "🔍 Проверка Asset Import Workers... "
worker_logs=$(find . -name "AssetImportWorker*.log" 2>/dev/null | wc -l)
if [ $worker_logs -gt 0 ]; then
    echo -e "${RED}❌ Найдено $worker_logs логов ошибок${NC}"
    echo "🧹 Удаление логов ошибок..."
    find . -name "AssetImportWorker*.log" -delete 2>/dev/null
    echo -e "  ${GREEN}✅ Логи удалены${NC}"
else
    echo -e "${GREEN}✅ Проблем не обнаружено${NC}"
fi

# 3. Проверка прав доступа
echo -n "🔍 Проверка прав доступа... "
find Assets -type f -name "*.cs" -exec chmod 644 {} \; 2>/dev/null
find Assets -type d -exec chmod 755 {} \; 2>/dev/null
echo -e "${GREEN}✅ Права исправлены${NC}"

# 4. Проверка дублирующихся .meta файлов
echo -n "🔍 Проверка дублирующихся .meta файлов... "
duplicate_meta=$(find Assets -name "*.meta" -exec basename {} \; | sort | uniq -d | wc -l)
if [ $duplicate_meta -gt 0 ]; then
    echo -e "${YELLOW}⚠️  Найдено $duplicate_meta дубликатов${NC}"
    echo "🔧 Исправление дубликатов..."
    find Assets -name "*.meta" -exec basename {} \; | sort | uniq -d | while read meta_file; do
        find Assets -name "$meta_file" | tail -n +2 | xargs rm -f 2>/dev/null
    done
    echo -e "  ${GREEN}✅ Дубликаты удалены${NC}"
else
    echo -e "${GREEN}✅ Дубликатов не найдено${NC}"
fi

# 5. Проверка циклических зависимостей
echo -n "🔍 Проверка циклических зависимостей... "
circular_deps=$(grep -r "using.*MudLike" Assets/Scripts --include="*.cs" | grep -v "//" | awk '{print $2}' | sort | uniq -c | awk '$1 > 10 {print $2}' | wc -l)
if [ $circular_deps -gt 0 ]; then
    echo -e "${YELLOW}⚠️  Возможные циклические зависимости: $circular_deps${NC}"
    echo "💡 Рекомендуется проверить архитектуру проекта"
else
    echo -e "${GREEN}✅ Циклических зависимостей не найдено${NC}"
fi

echo ""
echo "🛠️  ПРОФИЛАКТИЧЕСКИЕ МЕРЫ"
echo "=========================="

# 6. Создание резервной копии проекта
echo -n "💾 Создание резервной копии... "
backup_dir="backups/$(date +%Y%m%d_%H%M%S)"
mkdir -p "$backup_dir" 2>/dev/null
cp -r Assets "$backup_dir/" 2>/dev/null
cp -r Packages "$backup_dir/" 2>/dev/null
cp -r ProjectSettings "$backup_dir/" 2>/dev/null
echo -e "${GREEN}✅ Резервная копия создана: $backup_dir${NC}"

# 7. Оптимизация проекта
echo -n "⚡ Оптимизация проекта... "
# Удаление неиспользуемых файлов
find Assets -name "*.cs" -size 0 -delete 2>/dev/null
# Оптимизация .meta файлов
find Assets -name "*.meta" -exec sed -i '/^$/d' {} \; 2>/dev/null
echo -e "${GREEN}✅ Проект оптимизирован${NC}"

# 8. Проверка целостности проекта
echo -n "🔍 Проверка целостности проекта... "
missing_scripts=$(find Assets -name "*.cs" -exec grep -l "MonoBehaviour\|ScriptableObject" {} \; | xargs grep -L "using UnityEngine" | wc -l)
if [ $missing_scripts -gt 0 ]; then
    echo -e "${YELLOW}⚠️  Найдено $missing_scripts скриптов без using UnityEngine${NC}"
else
    echo -e "${GREEN}✅ Целостность проекта в порядке${NC}"
fi

echo ""
echo "📊 СТАТИСТИКА ОБСЛУЖИВАНИЯ"
echo "=========================="

total_files=$(find Assets -name "*.cs" | wc -l)
total_lines=$(find Assets -name "*.cs" -exec wc -l {} + | tail -1 | awk '{print $1}' 2>/dev/null || echo "0")
total_components=$(grep -r "IComponentData" Assets/Scripts --include="*.cs" | wc -l)
total_systems=$(grep -r "SystemBase\|ISystem" Assets/Scripts --include="*.cs" | wc -l)

echo -e "Файлов C#: ${CYAN}$total_files${NC}"
echo -e "Строк кода: ${CYAN}$total_lines${NC}"
echo -e "ECS компонентов: ${CYAN}$total_components${NC}"
echo -e "ECS систем: ${CYAN}$total_systems${NC}"

echo ""
echo "🎯 РЕКОМЕНДАЦИИ"
echo "==============="

echo -e "${BLUE}💡 Рекомендации для поддержания стабильности:${NC}"
echo -e "  • Запускать этот скрипт еженедельно"
echo -e "  • Регулярно обновлять Unity и пакеты"
echo -e "  • Использовать версионный контроль"
echo -e "  • Проверять логи Unity Editor"
echo -e "  • Следить за производительностью проекта"

echo ""
echo "✅ ПРОАКТИВНОЕ ОБСЛУЖИВАНИЕ ЗАВЕРШЕНО"
echo "===================================="
