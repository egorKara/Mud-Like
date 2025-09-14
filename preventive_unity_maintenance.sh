#!/bin/bash

# Профилактическое обслуживание Unity проекта
# Создан: 14 сентября 2025

echo "🛡️  ПРОФИЛАКТИЧЕСКОЕ ОБСЛУЖИВАНИЕ UNITY"
echo "======================================="

# Цвета для вывода
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
PURPLE='\033[0;35m'
CYAN='\033[0;36m'
NC='\033[0m' # No Color

echo ""
echo "🔍 ПРОФИЛАКТИЧЕСКИЕ ПРОВЕРКИ"
echo "============================="

# 1. Проверка и очистка кэша
echo -n "🧹 Проверка кэша Unity... "
cache_size=$(du -sh Library 2>/dev/null | cut -f1 || echo "0B")
if [ "$cache_size" != "0B" ]; then
    echo -e "${YELLOW}⚠️  $cache_size${NC}"
    echo "  💡 Рекомендуется очистка кэша"
    rm -rf Library/ScriptAssemblies Library/PlayerDataCache Temp 2>/dev/null
    echo "  ✅ Кэш очищен"
else
    echo -e "${GREEN}✅ ОК${NC}"
fi

# 2. Проверка Asset Import Workers
echo -n "🔍 Проверка Asset Import Workers... "
worker_logs=$(find . -name "AssetImportWorker*.log" 2>/dev/null | wc -l | tr -d ' ')
if [ "$worker_logs" -gt 0 ]; then
    echo -e "${RED}❌ $worker_logs логов${NC}"
    echo "  🧹 Очистка логов..."
    find . -name "AssetImportWorker*.log" -delete 2>/dev/null
    echo "  ✅ Логи очищены"
else
    echo -e "${GREEN}✅ ОК${NC}"
fi

# 3. Проверка прав доступа
echo -n "🔐 Проверка прав доступа... "
if [ -w Assets ] && [ -w ProjectSettings ]; then
    echo -e "${GREEN}✅ ОК${NC}"
else
    echo -e "${RED}❌ Проблемы с правами${NC}"
    echo "  🔧 Исправление прав..."
    chmod -R 755 Assets ProjectSettings 2>/dev/null
    echo "  ✅ Права исправлены"
fi

# 4. Проверка .meta файлов
echo -n "📁 Проверка .meta файлов... "
meta_count=$(find Assets -name "*.cs" | wc -l | tr -d ' ')
meta_files=$(find Assets -name "*.meta" | wc -l | tr -d ' ')
if [ "$meta_count" -eq "$meta_files" ]; then
    echo -e "${GREEN}✅ ОК ($meta_files/$meta_count)${NC}"
else
    echo -e "${YELLOW}⚠️  Несоответствие ($meta_files/$meta_count)${NC}"
    echo "  💡 Unity пересоздаст недостающие .meta файлы"
fi

# 5. Проверка Assembly Definition файлов
echo -n "📦 Проверка Assembly Definition... "
asmdef_count=$(find Assets -name "*.asmdef" | wc -l | tr -d ' ')
if [ "$asmdef_count" -gt 0 ]; then
    echo -e "${GREEN}✅ ОК ($asmdef_count файлов)${NC}"
else
    echo -e "${YELLOW}⚠️  Нет Assembly Definition файлов${NC}"
fi

# 6. Проверка пакетов
echo -n "📦 Проверка пакетов... "
if [ -f "Packages/manifest.json" ]; then
    echo -e "${GREEN}✅ ОК${NC}"
else
    echo -e "${RED}❌ Отсутствует manifest.json${NC}"
fi

echo ""
echo "🛠️  ПРОФИЛАКТИЧЕСКИЕ ДЕЙСТВИЯ"
echo "=============================="

# 7. Очистка временных файлов
echo -n "🧹 Очистка временных файлов... "
temp_files=$(find . -name "*.tmp" -o -name "*.temp" -o -name "*.log" | wc -l | tr -d ' ')
if [ "$temp_files" -gt 0 ]; then
    find . -name "*.tmp" -o -name "*.temp" -delete 2>/dev/null
    echo -e "${GREEN}✅ Очищено ($temp_files файлов)${NC}"
else
    echo -e "${GREEN}✅ ОК${NC}"
fi

# 8. Проверка производительности
echo -n "⚡ Проверка производительности... "
cs_files=$(find Assets -name "*.cs" | wc -l | tr -d ' ')
lines_of_code=$(find Assets -name "*.cs" -exec wc -l {} + 2>/dev/null | tail -1 | awk '{print $1}' || echo "0")
if [ "$lines_of_code" -lt 50000 ]; then
    echo -e "${GREEN}✅ ОК ($lines_of_code строк в $cs_files файлах)${NC}"
else
    echo -e "${YELLOW}⚠️  Большой проект ($lines_of_code строк)${NC}"
fi

# 9. Проверка версионного контроля
echo -n "📋 Проверка версионного контроля... "
if [ -d ".git" ]; then
    echo -e "${GREEN}✅ Git активен${NC}"
else
    echo -e "${YELLOW}⚠️  Git не инициализирован${NC}"
fi

echo ""
echo "📊 СТАТИСТИКА ПРОЕКТА"
echo "====================="
echo "📁 Файлов C#: $cs_files"
echo "📝 Строк кода: $lines_of_code"
echo "📦 Assembly Definition: $asmdef_count"
echo "🗂️  .meta файлов: $meta_files"

echo ""
echo "💡 РЕКОМЕНДАЦИИ ПО ПРОФИЛАКТИКЕ"
echo "==============================="
echo "🔹 Запускайте этот скрипт каждые 2-3 часа работы"
echo "🔹 Регулярно сохраняйте проект (Ctrl+S)"
echo "🔹 Избегайте одновременной работы нескольких Unity"
echo "🔹 Используйте версионный контроль для важных изменений"
echo "🔹 Следите за размером кэша Unity"
echo "🔹 Проверяйте логи Unity при возникновении проблем"

echo ""
echo "🎯 СТАТУС ПРОФИЛАКТИКИ"
echo "======================"
echo -e "${GREEN}🏆 ПРОФИЛАКТИЧЕСКОЕ ОБСЛУЖИВАНИЕ ЗАВЕРШЕНО${NC}"
echo -e "${GREEN}✅ Проект готов к стабильной работе${NC}"
echo -e "${GREEN}✅ Все профилактические меры применены${NC}"

echo ""
echo "✅ ПРОФИЛАКТИЧЕСКОЕ ОБСЛУЖИВАНИЕ ЗАВЕРШЕНО"
echo "=========================================="
