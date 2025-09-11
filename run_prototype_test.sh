#!/bin/bash

# 🧪 Скрипт для быстрого тестирования прототипа ECS

echo "🎮 Тестирование прототипа ECS системы движения Mud-Like"
echo "=================================================="

# Проверка Unity
echo "🔍 Проверка Unity..."
if ! command -v unity &> /dev/null; then
    echo "❌ Unity не найден в PATH"
    exit 1
fi

UNITY_VERSION=$(unity --version 2>/dev/null)
echo "✅ Unity версия: $UNITY_VERSION"

# Проверка проекта
echo "📁 Проверка проекта..."
if [ ! -f "Assets/Scripts/Core/Systems/PlayerMovementSystem.cs" ]; then
    echo "❌ Файлы прототипа не найдены"
    exit 1
fi
echo "✅ Файлы прототипа найдены"

# Компиляция
echo "🔨 Компиляция проекта..."
unity -projectPath . -batchmode -quit -logFile test_compile.log

if [ $? -eq 0 ]; then
    echo "✅ Компиляция успешна"
else
    echo "❌ Ошибка компиляции"
    echo "Проверьте test_compile.log для деталей"
    exit 1
fi

# Проверка сборок
echo "📦 Проверка сборок..."
if [ -f "Library/ScriptAssemblies/MudLike.Core.dll" ]; then
    SIZE=$(stat -c%s "Library/ScriptAssemblies/MudLike.Core.dll")
    echo "✅ MudLike.Core.dll: $SIZE bytes"
else
    echo "❌ MudLike.Core.dll не найден"
    exit 1
fi

# Проверка сцен
echo "🎬 Проверка сцен..."
if [ -f "Assets/Scenes/TestScene.unity" ]; then
    echo "✅ TestScene.unity найдена"
else
    echo "❌ TestScene.unity не найдена"
    exit 1
fi

# Проверка тестового скрипта
echo "🧪 Проверка тестового скрипта..."
if [ -f "Assets/Scripts/Core/Testing/PrototypeTester.cs" ]; then
    echo "✅ PrototypeTester.cs найден"
else
    echo "❌ PrototypeTester.cs не найден"
    exit 1
fi

echo ""
echo "🎉 ВСЕ ПРОВЕРКИ ПРОЙДЕНЫ!"
echo ""
echo "📋 Следующие шаги:"
echo "1. Откройте Unity Editor: unity -projectPath ."
echo "2. Загрузите сцену: Assets/Scenes/TestScene.unity"
echo "3. Нажмите Play для тестирования"
echo "4. Используйте WASD для управления"
echo "5. Проверьте Console для логов"
echo ""
echo "📖 Подробные инструкции: TESTING_INSTRUCTIONS.md"
echo ""
echo "🎮 Удачного тестирования!"
