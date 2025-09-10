#!/bin/bash

echo "🚨 КОМПИЛЯЦИЯ И ЗАПУСК АВТОНОМНОЙ СИСТЕМЫ ЗАЩИТЫ ОТ ПЕРЕГРЕВА 🚨"
echo "=============================================================="
echo

# Проверяем наличие .NET
if ! command -v dotnet &> /dev/null; then
    echo "❌ .NET не найден! Устанавливаем..."
    
    # Устанавливаем .NET 6.0
    wget https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
    sudo dpkg -i packages-microsoft-prod.deb
    sudo apt-get update
    sudo apt-get install -y dotnet-sdk-6.0
    
    if [ $? -ne 0 ]; then
        echo "❌ Ошибка установки .NET"
        exit 1
    fi
fi

echo "✅ .NET найден: $(dotnet --version)"
echo

# Компилируем проект
echo "🔨 Компиляция проекта..."
dotnet build --configuration Release

if [ $? -ne 0 ]; then
    echo "❌ Ошибка компиляции"
    exit 1
fi

echo "✅ Компиляция успешна"
echo

# Запускаем приложение
echo "🚀 Запуск системы защиты от перегрева..."
echo "Нажмите Ctrl+C для остановки"
echo

dotnet run --configuration Release