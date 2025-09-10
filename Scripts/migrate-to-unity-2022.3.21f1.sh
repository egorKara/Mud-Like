#!/bin/bash

# Скрипт миграции проекта Mud-Like с Unity 6000.2.2f1 на Unity 2022.3.21f1
# Автор: AI Assistant
# Дата: 2025-09-10

set -e  # Остановка при ошибке

echo "🔄 Начинаем миграцию проекта Mud-Like на Unity 2022.3.21f1..."

# Проверка существования backup
if [ ! -d "../Mud-Like-Backup-6000.2.2f1" ]; then
    echo "❌ Backup не найден! Создайте backup перед миграцией."
    exit 1
fi

echo "✅ Backup найден: ../Mud-Like-Backup-6000.2.2f1"

# Создание backup текущего состояния
echo "📦 Создание backup текущего состояния..."
cp -r . ../Mud-Like-Backup-before-migration-$(date +%Y%m%d-%H%M%S)

# Обновление ProjectVersion.txt
echo "📝 Обновление ProjectVersion.txt..."
cat > ProjectSettings/ProjectVersion.txt << EOF
m_EditorVersion: 2022.3.21f1
m_EditorVersionWithRevision: 2022.3.21f1 (b3c551784ba3)
EOF

# Обновление manifest.json
echo "📦 Обновление manifest.json..."
if [ -f "Packages/manifest-2022.3.21f1.json" ]; then
    cp Packages/manifest-2022.3.21f1.json Packages/manifest.json
    echo "✅ manifest.json обновлен"
else
    echo "❌ Файл manifest-2022.3.21f1.json не найден!"
    exit 1
fi

# Очистка кэша Unity
echo "🧹 Очистка кэша Unity..."
rm -rf Library/
rm -rf Temp/
rm -rf obj/

# Обновление workflows
echo "🔧 Обновление GitHub workflows..."
find .github/workflows/ -name "*.yml" -exec sed -i 's/6000\.2\.2f1/2022.3.21f1/g' {} \;

echo "✅ Миграция завершена!"
echo ""
echo "📋 Следующие шаги:"
echo "1. Откройте проект в Unity 2022.3.21f1"
echo "2. Проверьте консоль на ошибки"
echo "3. Протестируйте функциональность"
echo "4. Запустите тесты"
echo ""
echo "🔄 Для отката используйте:"
echo "   cp -r ../Mud-Like-Backup-6000.2.2f1/* ."
echo ""
echo "🎯 Проект готов к работе с Unity 2022.3.21f1!"
