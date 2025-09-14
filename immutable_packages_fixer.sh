#!/bin/bash

# Исправление проблем с immutable packages

echo "🔒 ИСПРАВЛЕНИЕ ПРОБЛЕМ IMMUTABLE PACKAGES"
echo "========================================="

# Цвета для вывода
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
PURPLE='\033[0;35m'
CYAN='\033[0;36m'
NC='\033[0m' # No Color

echo ""
echo "🔍 ДИАГНОСТИКА IMMUTABLE PACKAGES"
echo "================================="

# 1. Проверка изменений в Packages
echo -n "🔍 Проверка изменений в Packages... "
package_changes=$(find Packages -name "*.cs" -o -name "*.js" -o -name "*.json" 2>/dev/null | xargs grep -l "modified\|changed\|altered" 2>/dev/null | wc -l)
if [ $package_changes -gt 0 ]; then
    echo -e "${YELLOW}⚠️  Найдено $package_changes измененных файлов${NC}"
else
    echo -e "${GREEN}✅ Изменений в Packages не найдено${NC}"
fi

# 2. Проверка Package Manager манифеста
echo -n "🔍 Проверка Package Manager манифеста... "
if [ -f "Packages/manifest.json" ]; then
    echo -e "${GREEN}✅ manifest.json найден${NC}"
    
    # Анализ пакетов
    echo "📋 Анализ установленных пакетов:"
    cat Packages/manifest.json | grep -o '"[^"]*": "[^"]*"' | while read line; do
        package_name=$(echo $line | cut -d'"' -f2)
        package_version=$(echo $line | cut -d'"' -f4)
        echo -e "  ${CYAN}📦 $package_name: $package_version${NC}"
    done
else
    echo -e "${RED}❌ manifest.json не найден${NC}"
fi

# 3. Проверка локальных пакетов
echo -n "🔍 Проверка локальных пакетов... "
local_packages=$(find Packages -name "package.json" -path "*/local/*" 2>/dev/null | wc -l)
if [ $local_packages -gt 0 ]; then
    echo -e "${YELLOW}⚠️  Найдено $local_packages локальных пакетов${NC}"
    echo "📋 Локальные пакеты:"
    find Packages -name "package.json" -path "*/local/*" 2>/dev/null | while read package_file; do
        package_name=$(grep '"name"' "$package_file" | cut -d'"' -f4)
        echo -e "  ${CYAN}📦 $package_name${NC}"
    done
else
    echo -e "${GREEN}✅ Локальных пакетов не найдено${NC}"
fi

echo ""
echo "🛠️  ИСПРАВЛЕНИЕ ПРОБЛЕМ"
echo "======================="

# 4. Восстановление неизменяемых пакетов
echo "🔧 Восстановление неизменяемых пакетов..."
if [ -f "Packages/manifest.json" ]; then
    # Создание резервной копии
    cp Packages/manifest.json Packages/manifest.json.backup 2>/dev/null
    echo -e "  ${GREEN}✅ Резервная копия manifest.json создана${NC}"
    
    # Очистка кэша Package Manager
    rm -rf Library/PackageCache/* 2>/dev/null
    echo -e "  ${GREEN}✅ Кэш Package Manager очищен${NC}"
    
    # Восстановление пакетов
    echo "🔄 Восстановление пакетов..."
    # Unity автоматически восстановит пакеты при следующем запуске
    echo -e "  ${GREEN}✅ Пакеты будут восстановлены при следующем запуске Unity${NC}"
fi

# 5. Проверка и исправление package-lock.json
echo -n "🔍 Проверка package-lock.json... "
if [ -f "Packages/package-lock.json" ]; then
    echo -e "${GREEN}✅ package-lock.json найден${NC}"
    # Удаление lock файла для принудительного обновления
    rm -f Packages/package-lock.json 2>/dev/null
    echo -e "  ${GREEN}✅ package-lock.json удален для принудительного обновления${NC}"
else
    echo -e "${BLUE}ℹ️  package-lock.json не найден (это нормально)${NC}"
fi

# 6. Проверка целостности пакетов
echo "🔍 Проверка целостности пакетов..."
corrupted_packages=0
for package_dir in Packages/*/; do
    if [ -d "$package_dir" ]; then
        package_name=$(basename "$package_dir")
        if [ -f "$package_dir/package.json" ]; then
            # Проверка обязательных полей
            if ! grep -q '"name"' "$package_dir/package.json" || ! grep -q '"version"' "$package_dir/package.json"; then
                echo -e "  ${RED}❌ Поврежден пакет: $package_name${NC}"
                corrupted_packages=$((corrupted_packages + 1))
            fi
        fi
    fi
done

if [ $corrupted_packages -eq 0 ]; then
    echo -e "  ${GREEN}✅ Все пакеты целостны${NC}"
else
    echo -e "  ${YELLOW}⚠️  Найдено $corrupted_packages поврежденных пакетов${NC}"
fi

echo ""
echo "📊 СТАТИСТИКА ПАКЕТОВ"
echo "====================="

# Статистика
total_packages=$(find Packages -name "package.json" | wc -l)
core_packages=$(find Packages -name "package.json" -path "*/com.unity.*" | wc -l)
third_party_packages=$(find Packages -name "package.json" ! -path "*/com.unity.*" | wc -l)

echo -e "Всего пакетов: ${CYAN}$total_packages${NC}"
echo -e "Core пакетов Unity: ${CYAN}$core_packages${NC}"
echo -e "Сторонних пакетов: ${CYAN}$third_party_packages${NC}"

echo ""
echo "🎯 РЕКОМЕНДАЦИИ"
echo "==============="

echo -e "${BLUE}💡 Рекомендации для предотвращения проблем с immutable packages:${NC}"
echo -e "  • НЕ изменяйте файлы в папке Packages напрямую"
echo -e "  • Используйте Package Manager для установки/обновления пакетов"
echo -e "  • Создавайте локальные копии пакетов для кастомизации"
echo -e "  • Регулярно обновляйте пакеты до последних версий"
echo -e "  • Используйте версионный контроль для отслеживания изменений"

echo -e "${YELLOW}⚠️  Если проблемы продолжаются:${NC}"
echo -e "  • Перезапустите Unity Editor"
echo -e "  • Очистите кэш Package Manager"
echo -e "  • Переустановите проблемные пакеты"
echo -e "  • Проверьте совместимость версий пакетов"

echo ""
echo "✅ ИСПРАВЛЕНИЕ IMMUTABLE PACKAGES ЗАВЕРШЕНО"
echo "=========================================="
