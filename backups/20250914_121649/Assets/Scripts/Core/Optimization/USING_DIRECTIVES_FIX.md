# 🔧 Исправление ошибки CS1529: Using clause order

## ❌ Проблема
**Ошибка CS1529:** A using clause must precede all other elements defined in the namespace except extern alias declarations

### Местоположение ошибки
- **Файл:** `Assets/Scripts/Core/Optimization/CodeValidator.cs`
- **Строка:** 165
- **Проблема:** `using UnityEditor;` размещена после `namespace`

### Неправильный код
```csharp
namespace MudLike.Core.Optimization
{
    /// <summary>
    /// Editor-интеграция для валидации кода
    /// </summary>
    #if UNITY_EDITOR
    using UnityEditor; // ❌ ОШИБКА - using после namespace
    
    public class CodeValidatorEditor : EditorWindow
```

## ✅ Решение

### Исправленный код
```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Unity.Entities;
using Unity.Burst;
using Unity.Jobs;
#if UNITY_EDITOR
using UnityEditor; // ✅ ИСПРАВЛЕНО - using в начале файла
#endif

namespace MudLike.Core.Optimization
{
    /// <summary>
    /// Editor-интеграция для валидации кода
    /// </summary>
    #if UNITY_EDITOR
    public class CodeValidatorEditor : EditorWindow
```

## 🛡️ Предотвращение повторения

### Добавлено в систему валидации
1. **Новое правило валидации** в `CodeValidator.cs`:
   ```csharp
   new ValidationRule
   {
       Pattern = @"namespace\s+\w+.*\n.*using\s+",
       ErrorCode = "CS1529",
       Message = "Using directives must come before namespace declaration.",
       Severity = ValidationSeverity.Error
   }
   ```

2. **Автоматическая проверка** в `CompilationValidator.cs`:
   ```csharp
   private static List<string> CheckForUsingDirectiveOrder()
   {
       var pattern = @"namespace\s+\w+.*\n.*using\s+";
       // Проверка всех .cs файлов в проекте
   }
   ```

### Правила C# для using директив
1. **Все using директивы** должны быть в начале файла
2. **Порядок:** using → namespace → class
3. **Условные using** можно обернуть в `#if UNITY_EDITOR`
4. **extern alias** могут быть только перед using

## 📋 Чек-лист для разработчиков

### ✅ Правильная структура файла C#
```csharp
// 1. extern alias (если нужны)
extern alias MyAlias;

// 2. using директивы
using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

// 3. namespace
namespace MyNamespace
{
    // 4. классы, структуры, интерфейсы
    public class MyClass
    {
        // код класса
    }
}
```

### ❌ Избегать
- using директивы внутри namespace
- using директивы после объявлений классов
- using директивы в середине файла

## 🎯 Результат

### ✅ Исправлено
- **1 ошибка CS1529** устранена
- **1 файл** исправлен
- **Система валидации** расширена
- **Документация** обновлена

### 🚀 Улучшения
- **Автоматическое обнаружение** подобных ошибок
- **Проактивная защита** от нарушений порядка using
- **Соблюдение C# стандартов** в проекте
- **Улучшенная читаемость** кода

## 🔮 Будущие улучшения

### Планируемые функции
1. **Автоматическое исправление** порядка using директив
2. **Сортировка using** по алфавиту
3. **Удаление неиспользуемых** using директив
4. **Группировка using** по типам (System, Unity, Third-party)

### Рекомендации
1. **Всегда размещайте using** в начале файла
2. **Используйте условные using** для Editor-специфичного кода
3. **Следуйте установленному порядку** using директив
4. **Регулярно проверяйте** код через валидатор
