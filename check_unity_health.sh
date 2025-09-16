#!/bin/bash
# Автоматическая проверка здоровья Unity проекта

echo "🔍 ПРОВЕРКА ЗДОРОВЬЯ UNITY ПРОЕКТА"
echo "=================================="

# Проверка компиляции
if ./enhanced_quality_check.sh --quick > /dev/null 2>&1; then
    echo "✅ Компиляция: ОК"
else
    echo "❌ Компиляция: ОШИБКИ"
fi

# Проверка кэша
if [ -d "Library" ] && [ -d "Library/ScriptAssemblies" ]; then
    echo "✅ Кэш: ОК"
else
    echo "⚠️  Кэш: Требуется очистка"
fi

# Проверка версии Unity
if [ -f "ProjectSettings/ProjectVersion.txt" ]; then
    version=$(grep "m_EditorVersion:" ProjectSettings/ProjectVersion.txt | cut -d' ' -f2)
    echo "✅ Версия Unity: $version"
else
    echo "❌ Версия Unity: Не определена"
fi

echo "🎯 Проверка завершена"
