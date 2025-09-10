#!/bin/bash

# Автоматическая настройка безопасности репозитория Mud-Like
# Выполнить когда интернет соединение восстановится

echo "🔒 Настройка безопасности репозитория Mud-Like..."

# Проверка аутентификации
if ! gh auth status > /dev/null 2>&1; then
    echo "❌ Необходима аутентификация в GitHub CLI"
    echo "Выполните: gh auth login --web"
    exit 1
fi

echo "✅ Аутентификация в GitHub CLI активна"

# 1. Включение Dependabot Alerts
echo "📦 Включаю Dependabot Alerts..."
gh api repos/egorKara/Mud-Like/vulnerability-alerts --method PUT
if [ $? -eq 0 ]; then
    echo "✅ Dependabot Alerts включены"
else
    echo "❌ Ошибка включения Dependabot Alerts"
fi

# 2. Включение Automated Security Fixes
echo "🔧 Включаю Automated Security Fixes..."
gh api repos/egorKara/Mud-Like/automated-security-fixes --method PUT
if [ $? -eq 0 ]; then
    echo "✅ Automated Security Fixes включены"
else
    echo "❌ Ошибка включения Automated Security Fixes"
fi

# 3. Настройка Branch Protection
echo "🛡️ Настраиваю Branch Protection для main..."
gh api repos/egorKara/Mud-Like/branches/main/protection --method PUT --input branch_protection.json
if [ $? -eq 0 ]; then
    echo "✅ Branch Protection настроена"
else
    echo "❌ Ошибка настройки Branch Protection"
fi

# 4. Проверка статуса
echo "📊 Проверяю статус безопасности..."
gh api repos/egorKara/Mud-Like/vulnerability-alerts
gh api repos/egorKara/Mud-Like/automated-security-fixes
gh api repos/egorKara/Mud-Like/branches/main/protection

echo "🎉 Настройка безопасности завершена!"
echo "📋 Проверьте настройки в GitHub UI:"
echo "   - Settings → Security & analysis"
echo "   - Settings → Branches"
echo "   - Settings → Actions → General"
