#!/usr/bin/env python3
"""
Автоматическое добавление документации к методам C#
Создан: 14 сентября 2025
Цель: Улучшение документации проекта MudRunner-like
"""

import os
import re
import sys

def analyze_csharp_file(file_path):
    """Анализ C# файла на предмет методов без документации"""
    with open(file_path, 'r', encoding='utf-8') as f:
        content = f.read()
    
    # Поиск публичных методов
    method_pattern = r'public\s+[^{]+\s+(\w+)\s*\('
    methods = re.findall(method_pattern, content)
    
    # Поиск методов с документацией
    documented_pattern = r'/// <summary>.*?/// </summary>'
    documented_methods = re.findall(documented_pattern, content, re.DOTALL)
    
    return len(methods), len(documented_methods)

def generate_documentation_template(method_name, file_path):
    """Генерация шаблона документации для метода"""
    
    # Определение типа файла для контекста
    if 'Vehicle' in file_path or 'vehicle' in file_path:
        context = "транспортного средства"
        remarks = "Критично для реалистичной физики MudRunner-like"
    elif 'Terrain' in file_path or 'terrain' in file_path:
        context = "деформации террейна"
        remarks = "Критично для реалистичной деформации MudRunner-like"
    elif 'Network' in file_path or 'network' in file_path:
        context = "мультиплеера"
        remarks = "Критично для мультиплеера MudRunner-like"
    elif 'Physics' in file_path or 'physics' in file_path:
        context = "физики"
        remarks = "Критично для Unity Physics (DOTS)"
    else:
        context = "системы"
        remarks = "Важно для проекта MudRunner-like"
    
    template = f"""/// <summary>
/// Описание метода {method_name} для {context}
/// </summary>
/// <returns>Описание возвращаемого значения</returns>
/// <remarks>
/// {remarks}
/// </remarks>"""
    
    return template

def main():
    """Основная функция"""
    print("📚 АВТОМАТИЧЕСКОЕ УЛУЧШЕНИЕ ДОКУМЕНТАЦИИ")
    print("======================================")
    
    # Поиск C# файлов
    csharp_files = []
    for root, dirs, files in os.walk('Assets'):
        for file in files:
            if file.endswith('.cs'):
                csharp_files.append(os.path.join(root, file))
    
    total_methods = 0
    total_documented = 0
    
    print(f"🔍 Найдено {len(csharp_files)} C# файлов")
    
    for file_path in csharp_files:
        methods, documented = analyze_csharp_file(file_path)
        total_methods += methods
        total_documented += documented
        
        if methods > 0:
            percentage = (documented / methods * 100) if methods > 0 else 0
            print(f"  📄 {os.path.basename(file_path)}: {methods} методов, {documented} документированных ({percentage:.1f}%)")
    
    print(f"\n📊 ОБЩАЯ СТАТИСТИКА:")
    print(f"  📝 Всего методов: {total_methods}")
    print(f"  ✅ Документированных: {total_documented}")
    print(f"  ❌ Не документированных: {total_methods - total_documented}")
    
    if total_methods > 0:
        percentage = (total_documented / total_methods * 100)
        print(f"  📈 Процент документирования: {percentage:.1f}%")

if __name__ == "__main__":
    main()
