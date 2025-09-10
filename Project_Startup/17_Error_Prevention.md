# 🛡️ Mud-Like Error Prevention Guide

## 📋 **ОБЗОР**

Данный документ содержит меры предотвращения ошибок, выявленных в процессе разработки проекта Mud-Like.

## 🚨 **ВЫЯВЛЕННЫЕ ОШИБКИ**

### **1. Ошибки компиляции C#**

#### **CS0116: Namespace cannot directly contain members**
- **Причина**: Дефисы в именах namespace
- **Решение**: Использовать только валидные символы C#

#### **CS0246: Type or namespace name could not be found**
- **Причина**: Отсутствие ссылки на Unity.Transforms в Assembly Definition
- **Решение**: Добавить Unity.Transforms в references

#### **DC0005: Entities.ForEach Lambda expression parameter not supported**
- **Причина**: Использование устаревшего компонента Translation
- **Решение**: Заменить на LocalTransform

#### **CS0619: ComponentSystemBase.Time is obsolete**
- **Причина**: Устаревший API Time в Unity DOTS
- **Решение**: Заменить на SystemAPI.Time

#### **CS0246: Type or namespace name could not be found**
- **Причина**: Отсутствие необходимых references в Assembly Definition
- **Решение**: Добавить Unity.Entities, Unity.Mathematics, Unity.Transforms

#### **CS0234: The type or namespace name does not exist in the namespace 'Unity'**
- **Причина**: Unity не может найти DOTS пакеты (Entities, Mathematics, Transforms)
- **Решение**: Очистить кэш Unity, проверить Package Manager

### **2. Ошибки дублирования Assembly Definition**
- **Причина**: Наличие старых файлов с дефисами
- **Решение**: Удаление дублирующих файлов

### **3. Предупреждения о символических ссылках**
- **Причина**: Unity обнаруживает symlinks в папке Packages
- **Решение**: Использовать Package Manager вместо symlinks
- **Предотвращение**: Установка пакетов только через Unity Package Manager

### **4. Пустые Assembly Definition файлы**
- **Причина**: Assembly Definition файлы без связанных скриптов
- **Решение**: Создать заглушки или удалить пустые .asmdef файлы
- **Предотвращение**: Создавать .asmdef только при наличии скриптов

## 🛠️ **МЕРЫ ПРЕДОТВРАЩЕНИЯ**

### **1. Соглашения по именованию**

#### **Namespace имена:**
```csharp
// ✅ ПРАВИЛЬНО
namespace MudLike.Core.Components
namespace MudLike.Core.Systems

// ❌ НЕПРАВИЛЬНО
namespace Mud-Like.Core.Components  // Дефис недопустим
```

#### **Assembly Definition файлы:**
```json
{
    "name": "MudLike.Core",           // ✅ Без дефисов
    "rootNamespace": "MudLike.Core"   // ✅ Без дефисов
}
```

### **2. Обязательные ссылки для DOTS**

#### **Core Assembly Definition:**
```json
{
    "references": [
        "Unity.Entities",
        "Unity.Collections", 
        "Unity.Jobs",
        "Unity.Burst",
        "Unity.Mathematics",
        "Unity.Transforms"  // ✅ Обязательно для LocalTransform
    ]
}
```

### **3. Современные DOTS компоненты**

#### **Использование LocalTransform вместо Translation:**
```csharp
// ✅ ПРАВИЛЬНО (Unity 6000+)
using Unity.Transforms;

Entities.ForEach((ref LocalTransform transform) => {
    transform.Position += movement * deltaTime;
}).Schedule();

// ❌ УСТАРЕЛО
Entities.ForEach((ref Translation translation) => {
    translation.Value += movement * deltaTime;
}).Schedule();
```

#### **Использование SystemAPI.Time вместо Time:**
```csharp
// ✅ ПРАВИЛЬНО (Unity 6000+)
float deltaTime = SystemAPI.Time.fixedDeltaTime;

// ❌ УСТАРЕЛО
float deltaTime = Time.fixedDeltaTime;
```

### **4. Процедуры очистки**

#### **При переименовании файлов:**
1. Удалить старые файлы с дефисами
2. Удалить соответствующие .meta файлы
3. Очистить кэш Unity (Library/, Temp/)
4. Перезапустить Unity Editor

#### **При ошибках CS0234 (DOTS пакеты не найдены):**
1. Очистить кэш Unity (Library/, Temp/)
2. Проверить Package Manager в Unity
3. Убедиться, что пакеты установлены через Package Manager
4. Перезапустить Unity Editor

#### **Команды очистки:**
```bash
# Удаление кэша Unity
rm -rf Library/
rm -rf Temp/

# Удаление старых файлов
find . -name "*Mud-Like*" -type f -delete
```

### **5. Проверочный список**

#### **Перед коммитом:**
- [ ] Все namespace без дефисов
- [ ] Assembly Definition файлы переименованы
- [ ] Добавлены все необходимые references в .asmdef
- [ ] Используются современные DOTS компоненты
- [ ] Используется SystemAPI.Time вместо Time
- [ ] Нет дублирующих файлов
- [ ] Нет символических ссылок в Packages
- [ ] Нет ошибок CS0234 (DOTS пакеты найдены)
- [ ] Пустые Assembly Definition файлы обработаны
- [ ] Кэш Unity очищен

#### **При создании новых файлов:**
- [ ] Имя файла без дефисов
- [ ] Namespace соответствует структуре проекта
- [ ] Добавлены необходимые using директивы
- [ ] Assembly Definition обновлен при необходимости

## 📚 **СПРАВОЧНАЯ ИНФОРМАЦИЯ**

### **Валидные символы для C# идентификаторов:**
- Буквы (a-z, A-Z)
- Цифры (0-9)
- Подчеркивания (_)
- **НЕ ДОПУСТИМЫ**: дефисы (-), пробелы, специальные символы

### **Современные DOTS компоненты (Unity 6000+):**
- `LocalTransform` вместо `Translation`
- `LocalToWorld` для мировых координат
- `LocalRotation` для поворотов
- `LocalScale` для масштабирования

### **Современные DOTS API (Unity 6000+):**
- `SystemAPI.Time` вместо `Time`
- `SystemAPI.GetComponent` вместо `GetComponent`
- `SystemAPI.SetComponent` вместо `SetComponent`

### **Обязательные пакеты для DOTS:**
- `com.unity.entities`
- `com.unity.transforms`
- `com.unity.collections`
- `com.unity.jobs`
- `com.unity.burst`
- `com.unity.mathematics`

## 🔄 **ПРОЦЕДУРЫ ОБНОВЛЕНИЯ**

### **При обновлении Unity:**
1. Проверить совместимость DOTS компонентов
2. Обновить устаревшие компоненты
3. Проверить Assembly Definition references
4. Протестировать компиляцию

### **При добавлении новых пакетов:**
1. Установить через Unity Package Manager
2. Избегать символических ссылок
3. Обновить references в Assembly Definition
4. Обновить using директивы
5. Протестировать компиляцию

### **При создании Assembly Definition файлов:**
1. Создавать .asmdef только при наличии скриптов
2. Избегать пустых Assembly Definition файлов
3. Удалять .asmdef файлы без связанных скриптов
4. Проверять компиляцию после изменений

## 📝 **ЗАКЛЮЧЕНИЕ**

Соблюдение данных мер предотвращения обеспечит:
- Стабильную компиляцию проекта
- Совместимость с современными версиями Unity
- Отсутствие конфликтов Assembly Definition
- Корректную работу DOTS систем

**Помните**: Профилактика ошибок эффективнее их исправления!
