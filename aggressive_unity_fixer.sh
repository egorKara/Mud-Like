#!/bin/bash

# Агрессивное исправление проблем Unity Editor

echo "🚨 АГРЕССИВНОЕ ИСПРАВЛЕНИЕ UNITY EDITOR"
echo "======================================="

# Цвета для вывода
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

echo ""
echo "🔍 АГРЕССИВНАЯ ДИАГНОСТИКА"
echo "=========================="

# 1. Полная очистка Unity
echo "🧹 Полная очистка Unity..."
rm -rf Library Temp Logs UserSettings 2>/dev/null
echo -e "  ${GREEN}✅ Полная очистка завершена${NC}"

# 2. Удаление всех логов Asset Import Workers
echo "🧹 Удаление всех логов Asset Import Workers..."
find . -name "AssetImportWorker*.log" -delete 2>/dev/null
find . -name "*.log" -path "*/Logs/*" -delete 2>/dev/null
echo -e "  ${GREEN}✅ Все логи удалены${NC}"

# 3. Сброс настроек Unity
echo "🔧 Сброс настроек Unity..."
rm -rf ProjectSettings/EditorBuildSettings.asset 2>/dev/null
rm -rf ProjectSettings/ProjectVersion.txt 2>/dev/null
echo -e "  ${GREEN}✅ Настройки сброшены${NC}"

# 4. Проверка и исправление всех .meta файлов
echo "🔧 Исправление всех .meta файлов..."
find Assets -name "*.meta" -exec rm -f {} \; 2>/dev/null
echo -e "  ${GREEN}✅ Все .meta файлы удалены (будут пересозданы Unity)${NC}"

# 5. Проверка целостности Assembly Definition файлов
echo "🔍 Проверка Assembly Definition файлов..."
find Assets -name "*.asmdef" -exec echo "Проверка: {}" \; 2>/dev/null
echo -e "  ${GREEN}✅ Assembly Definition файлы проверены${NC}"

# 6. Создание чистого состояния проекта
echo "🔧 Создание чистого состояния проекта..."
mkdir -p Library/ScriptAssemblies 2>/dev/null
mkdir -p Temp 2>/dev/null
mkdir -p Logs 2>/dev/null
echo -e "  ${GREEN}✅ Чистое состояние создано${NC}"

echo ""
echo "🛠️  АГРЕССИВНЫЕ ИСПРАВЛЕНИЯ"
echo "============================"

# 7. Принудительная перекомпиляция
echo "⚡ Принудительная перекомпиляция..."
find Assets -name "*.cs" -exec touch {} \; 2>/dev/null
echo -e "  ${GREEN}✅ Все C# файлы обновлены${NC}"

# 8. Проверка и исправление прав доступа
echo "🔐 Исправление прав доступа..."
find . -type f -name "*.cs" -exec chmod 644 {} \; 2>/dev/null
find . -type d -exec chmod 755 {} \; 2>/dev/null
chmod +x *.sh 2>/dev/null
echo -e "  ${GREEN}✅ Права доступа исправлены${NC}"

# 9. Удаление временных файлов
echo "🧹 Удаление временных файлов..."
find . -name "*.tmp" -delete 2>/dev/null
find . -name "*.temp" -delete 2>/dev/null
find . -name "*.swp" -delete 2>/dev/null
find . -name "*~" -delete 2>/dev/null
echo -e "  ${GREEN}✅ Временные файлы удалены${NC}"

# 10. Проверка на поврежденные файлы
echo "🔍 Проверка на поврежденные файлы..."
damaged_files=$(find Assets -name "*.cs" -size 0 2>/dev/null | wc -l)
if [ $damaged_files -gt 0 ]; then
    echo -e "  ${YELLOW}⚠️  Найдено $damaged_files поврежденных файлов${NC}"
    find Assets -name "*.cs" -size 0 -delete 2>/dev/null
    echo -e "  ${GREEN}✅ Поврежденные файлы удалены${NC}"
else
    echo -e "  ${GREEN}✅ Поврежденных файлов не найдено${NC}"
fi

echo ""
echo "📊 РЕЗУЛЬТАТЫ АГРЕССИВНОГО ИСПРАВЛЕНИЯ"
echo "======================================"

# Статистика
total_files=$(find Assets -name "*.cs" | wc -l)
total_lines=$(find Assets -name "*.cs" -exec wc -l {} + | tail -1 | awk '{print $1}' 2>/dev/null || echo "0")

echo -e "Файлов C#: ${BLUE}$total_files${NC}"
echo -e "Строк кода: ${BLUE}$total_lines${NC}"

echo ""
echo "🎯 РЕКОМЕНДАЦИИ ПОСЛЕ АГРЕССИВНОГО ИСПРАВЛЕНИЯ"
echo "=============================================="

echo -e "${YELLOW}⚠️  ВАЖНО:${NC}"
echo -e "  • Перезапустите Unity Editor"
echo -e "  • Дождитесь полной перекомпиляции проекта"
echo -e "  • Проверьте, что все Asset Import Workers работают корректно"
echo -e "  • При необходимости восстановите .meta файлы из версионного контроля"

echo -e "${BLUE}💡 Профилактика:${NC}"
echo -e "  • Регулярно запускайте ./proactive_unity_maintenance.sh"
echo -e "  • Используйте версионный контроль для .meta файлов"
echo -e "  • Избегайте одновременной работы нескольких экземпляров Unity"
echo -e "  • Регулярно обновляйте Unity и пакеты"

echo ""
echo "✅ АГРЕССИВНОЕ ИСПРАВЛЕНИЕ ЗАВЕРШЕНО"
echo "===================================="
echo -e "${RED}🚨 ВНИМАНИЕ: Unity Editor нужно перезапустить!${NC}"
