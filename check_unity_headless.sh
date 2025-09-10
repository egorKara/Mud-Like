#!/bin/bash

# Простая проверка Unity проекта в headless режиме
# Без графической оболочки для экономии ресурсов

echo "🎮 ПРОВЕРКА UNITY ПРОЕКТА БЕЗ ГРАФИКИ"
echo "===================================="
echo "Время: $(date)"
echo ""

# Проверяем наличие Unity
echo "🔍 Поиск Unity..."
UNITY_FOUND=false

# Проверяем различные возможные пути Unity
if command -v unity &> /dev/null; then
    UNITY_PATH="unity"
    UNITY_FOUND=true
    echo "✅ Unity найден в PATH: $UNITY_PATH"
elif [ -f "/opt/Unity/Editor/Unity" ]; then
    UNITY_PATH="/opt/Unity/Editor/Unity"
    UNITY_FOUND=true
    echo "✅ Unity найден: $UNITY_PATH"
elif [ -f "/usr/bin/unity" ]; then
    UNITY_PATH="/usr/bin/unity"
    UNITY_FOUND=true
    echo "✅ Unity найден: $UNITY_PATH"
else
    echo "❌ Unity не найден в стандартных местах"
    echo "🔍 Ищем Unity в системе..."
    
    # Ищем Unity в системе
    UNITY_SEARCH=$(find /opt /usr /home -name "Unity" -type f 2>/dev/null | head -1)
    if [ -n "$UNITY_SEARCH" ]; then
        UNITY_PATH="$UNITY_SEARCH"
        UNITY_FOUND=true
        echo "✅ Unity найден: $UNITY_PATH"
    else
        echo "❌ Unity не найден в системе"
        echo "💡 Установите Unity или укажите путь вручную"
        exit 1
    fi
fi

# Путь к проекту
PROJECT_PATH="/workspace"
echo "📁 Проект: $PROJECT_PATH"

# Проверяем структуру проекта
echo ""
echo "📂 Проверка структуры проекта..."
if [ -f "$PROJECT_PATH/ProjectSettings/ProjectVersion.txt" ]; then
    UNITY_VERSION=$(cat "$PROJECT_PATH/ProjectSettings/ProjectVersion.txt")
    echo "✅ Unity версия: $UNITY_VERSION"
else
    echo "❌ Файл ProjectVersion.txt не найден"
    echo "💡 Убедитесь, что это корректный Unity проект"
fi

if [ -d "$PROJECT_PATH/Assets" ]; then
    echo "✅ Папка Assets найдена"
else
    echo "❌ Папка Assets не найдена"
fi

if [ -d "$PROJECT_PATH/Assets/Scripts" ]; then
    SCRIPT_COUNT=$(find "$PROJECT_PATH/Assets/Scripts" -name "*.cs" | wc -l)
    echo "✅ Найдено $SCRIPT_COUNT C# скриптов"
else
    echo "❌ Папка Assets/Scripts не найдена"
fi

# Создаем простой тест без Unity
echo ""
echo "🧪 Запуск простых тестов без Unity..."

# Тест 1: Проверка C# файлов
echo "1️⃣ Проверка C# файлов..."
CS_ERRORS=0

# Проверяем основные файлы проекта
for file in "Assets/Scripts/Vehicles/Components/TruckData.cs" \
           "Assets/Scripts/Vehicles/Components/TruckControl.cs" \
           "Assets/Scripts/Vehicles/Systems/TruckMovementSystem.cs"; do
    if [ -f "$PROJECT_PATH/$file" ]; then
        echo "   ✅ $file"
    else
        echo "   ❌ $file - НЕ НАЙДЕН"
        CS_ERRORS=$((CS_ERRORS + 1))
    fi
done

# Тест 2: Проверка системы защиты от перегрева
echo "2️⃣ Проверка системы защиты от перегрева..."
if [ -f "overheat_protection.sh" ]; then
    echo "   ✅ overheat_protection.sh найден"
    
    # Проверяем, что система работает
    if ps aux | grep overheat_protection | grep -v grep > /dev/null; then
        echo "   ✅ Система защиты от перегрева активна"
    else
        echo "   ⚠️  Система защиты от перегрева не запущена"
    fi
else
    echo "   ❌ overheat_protection.sh не найден"
    CS_ERRORS=$((CS_ERRORS + 1))
fi

# Тест 3: Проверка документации
echo "3️⃣ Проверка документации..."
DOC_FILES=("README.md" "Project_Startup/README.md" "Project_Startup/17_Implementation_Status.md")
for doc in "${DOC_FILES[@]}"; do
    if [ -f "$PROJECT_PATH/$doc" ]; then
        echo "   ✅ $doc"
    else
        echo "   ❌ $doc - НЕ НАЙДЕН"
        CS_ERRORS=$((CS_ERRORS + 1))
    fi
done

# Итоговый результат
echo ""
echo "📊 РЕЗУЛЬТАТЫ ПРОВЕРКИ:"
echo "======================"

if [ $CS_ERRORS -eq 0 ]; then
    echo "✅ ВСЕ ПРОВЕРКИ ПРОЙДЕНЫ УСПЕШНО!"
    echo "🎮 Unity проект готов к headless тестированию"
    echo ""
    echo "💡 Для полного тестирования запустите:"
    echo "   ./test_unity_headless.sh"
else
    echo "❌ НАЙДЕНО $CS_ERRORS ОШИБОК"
    echo "🔧 Исправьте ошибки перед тестированием"
fi

echo ""
echo "🛡️ СИСТЕМА ЗАЩИТЫ ОТ ПЕРЕГРЕВА:"
echo "==============================="
if ps aux | grep overheat_protection | grep -v grep > /dev/null; then
    echo "✅ Активна и защищает систему"
else
    echo "⚠️  Не активна - рекомендуется запустить"
    echo "   ./overheat_protection.sh &"
fi