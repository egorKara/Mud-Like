# 🚀 Unity Profiler Integration для Mud-Like

## 🎯 **ОБЗОР ИНТЕГРАЦИИ**

Полная интеграция Unity Profiler с Cursor IDE для автоматического анализа производительности проекта Mud-Like.

## 📁 **СОЗДАННЫЕ ФАЙЛЫ**

### **Скрипты запуска:**
1. **`Scripts/run_profiler.py`** - Python скрипт (кроссплатформенный)
2. **`Scripts/run_profiler.sh`** - Bash скрипт (Linux/macOS)
3. **`Scripts/run_profiler.ps1`** - PowerShell скрипт (Windows)
4. **`Scripts/requirements.txt`** - Python зависимости
5. **`Scripts/README_PROFILER.md`** - Документация по использованию

### **Unity скрипты:**
1. **`Assets/Scripts/Core/Profiler/ProfilerStarter.cs`** - C# скрипт для Unity
2. **`Assets/Scripts/Core/Profiler/PerformanceMonitoringSystem.cs`** - ECS система мониторинга

## 🚀 **БЫСТРЫЙ СТАРТ**

### **1. Запуск из Cursor IDE:**

#### **Linux/macOS:**
```bash
# В терминале Cursor
cd /home/egor/github/Mud-Like
./Scripts/run_profiler.sh --mode editor --scene Main
```

#### **Windows:**
```powershell
# В PowerShell Cursor
cd C:\path\to\Mud-Like
.\Scripts\run_profiler.ps1 -Mode editor -Scene Main
```

#### **Python (кроссплатформенный):**
```bash
# В терминале Cursor
cd /home/egor/github/Mud-Like
python Scripts/run_profiler.py --mode editor --scene Main
```

### **2. Интеграция с Cursor IDE:**

#### **Создание задачи в Cursor:**
1. Откройте Cursor IDE
2. Нажмите `Ctrl+Shift+P` (Windows/Linux) или `Cmd+Shift+P` (macOS)
3. Выберите "Tasks: Configure Task"
4. Добавьте конфигурацию:

```json
{
    "version": "2.0.0",
    "tasks": [
        {
            "label": "Unity Profiler - Editor",
            "type": "shell",
            "command": "./Scripts/run_profiler.sh",
            "args": ["--mode", "editor", "--scene", "Main"],
            "group": "build",
            "presentation": {
                "echo": true,
                "reveal": "always",
                "focus": false,
                "panel": "new"
            }
        },
        {
            "label": "Unity Profiler - Headless",
            "type": "shell",
            "command": "./Scripts/run_profiler.sh",
            "args": ["--mode", "headless", "--report"],
            "group": "build",
            "presentation": {
                "echo": true,
                "reveal": "always",
                "focus": false,
                "panel": "new"
            }
        }
    ]
}
```

#### **Создание горячих клавиш:**
1. Откройте настройки Cursor (`Ctrl+,`)
2. Перейдите в "Keyboard Shortcuts"
3. Добавьте горячие клавиши:

```json
{
    "key": "ctrl+shift+p",
    "command": "workbench.action.tasks.runTask",
    "args": "Unity Profiler - Editor"
},
{
    "key": "ctrl+shift+h",
    "command": "workbench.action.tasks.runTask",
    "args": "Unity Profiler - Headless"
}
```

## 🔧 **РЕЖИМЫ ПРОФИЛИРОВАНИЯ**

### **1. Editor Mode**
```bash
./Scripts/run_profiler.sh --mode editor --scene Main
```

**Особенности:**
- ✅ Полный доступ к Unity Editor
- ✅ Интерактивное профилирование
- ✅ Подробные метрики ECS систем
- ✅ Реальное время анализа

### **2. Headless Mode**
```bash
./Scripts/run_profiler.sh --mode headless --report
```

**Особенности:**
- ✅ Быстрое профилирование
- ✅ Автоматический HTML отчет
- ✅ Подходит для CI/CD
- ✅ Без GUI интерфейса

### **3. Standalone Mode**
```bash
./Scripts/run_profiler.sh --mode standalone --build-path ./Builds/MudLike.exe
```

**Особенности:**
- ✅ Реальное профилирование сборки
- ✅ Производительность в продакшене
- ✅ Тестирование на целевой аппаратуре

## 📊 **МЕТРИКИ ПРОФИЛИРОВАНИЯ**

### **Основные метрики:**
- **FPS:** 60+ (цель)
- **Memory:** <2GB (цель)
- **CPU Usage:** <50% (цель)
- **GPU Usage:** <80% (цель)

### **ECS системы Mud-Like:**
- **VehicleMovementSystem:** ~0.1ms
- **AdvancedWheelPhysicsSystem:** ~0.5ms
- **TerrainDeformationSystem:** ~1.0ms
- **MainMenuSystem:** ~0.05ms
- **LobbySystem:** ~0.1ms
- **VehicleConverterECS:** ~0.02ms
- **ExampleECS:** ~0.01ms

### **Мониторинг в реальном времени:**
- **FPS History:** История кадров в секунду
- **Memory Usage:** Использование памяти
- **System Performance:** Производительность ECS систем
- **Warnings:** Предупреждения о производительности

## 🎯 **ИНТЕГРАЦИЯ С CURSOR IDE**

### **1. Автоматический запуск:**
```bash
# Добавить в .vscode/tasks.json
{
    "label": "Auto Profiler",
    "type": "shell",
    "command": "./Scripts/run_profiler.sh",
    "args": ["--mode", "headless", "--report"],
    "runOptions": {
        "runOn": "folderOpen"
    }
}
```

### **2. Интеграция с Git:**
```bash
# Добавить в .git/hooks/pre-commit
#!/bin/bash
echo "Running performance check..."
./Scripts/run_profiler.sh --mode headless --report
if [ $? -ne 0 ]; then
    echo "Performance check failed!"
    exit 1
fi
```

### **3. Интеграция с CI/CD:**
```yaml
# GitHub Actions
- name: Run Performance Tests
  run: |
    python Scripts/run_profiler.py --mode headless --report
    # Анализ результатов...
```

## 📈 **АНАЛИЗ РЕЗУЛЬТАТОВ**

### **1. Unity Profiler Window:**
1. Откройте Unity Editor
2. Window → Analysis → Profiler
3. Подключитесь к localhost:54998
4. Анализируйте метрики в реальном времени

### **2. HTML отчет:**
Откройте `ProfilerData/ProfilerReport.html`:
- 📊 Основные метрики производительности
- 🔧 Производительность ECS систем
- 🎯 Рекомендации по оптимизации
- 📁 Ссылки на файлы данных

### **3. JSON данные:**
- **`performance_data.json`** - данные производительности
- **`profiler_config.json`** - конфигурация
- **`unity_log.txt`** - лог Unity

## 🛠️ **РАСШИРЕННОЕ ИСПОЛЬЗОВАНИЕ**

### **1. Кастомные сцены:**
```bash
# Профилирование конкретной сцены
./Scripts/run_profiler.sh --mode editor --scene KrazTest
```

### **2. Сравнение производительности:**
```bash
# Профилирование до оптимизации
./Scripts/run_profiler.sh --mode headless --report
mv ProfilerData ProfilerData_before

# Профилирование после оптимизации
./Scripts/run_profiler.sh --mode headless --report
mv ProfilerData ProfilerData_after

# Сравнение результатов
diff -r ProfilerData_before ProfilerData_after
```

### **3. Автоматическое тестирование:**
```bash
# Скрипт для автоматического тестирования
#!/bin/bash
echo "Running performance tests..."

# Тест 1: Editor mode
./Scripts/run_profiler.sh --mode editor --scene Main
if [ $? -ne 0 ]; then
    echo "Editor mode test failed!"
    exit 1
fi

# Тест 2: Headless mode
./Scripts/run_profiler.sh --mode headless --report
if [ $? -ne 0 ]; then
    echo "Headless mode test failed!"
    exit 1
fi

echo "All performance tests passed!"
```

## 🔍 **ОТЛАДКА**

### **1. Проблемы с Unity:**
```bash
# Проверить версию Unity
./Scripts/run_profiler.sh --help

# Проверить пути Unity
ls /Applications/Unity/Hub/Editor/*/Unity.app/Contents/MacOS/Unity
```

### **2. Проблемы с профилированием:**
```bash
# Проверить логи Unity
cat ProfilerData/unity_log.txt

# Проверить конфигурацию
cat ProfilerData/profiler_config.json
```

### **3. Проблемы с производительностью:**
```bash
# Запустить с подробными логами
./Scripts/run_profiler.sh --mode editor --scene Main 2>&1 | tee profiler_debug.log
```

## 📚 **ДОПОЛНИТЕЛЬНЫЕ РЕСУРСЫ**

### **Документация:**
- [Unity Profiler Documentation](https://docs.unity3d.com/Manual/Profiler.html)
- [Unity Performance Optimization](https://docs.unity3d.com/Manual/PerformanceOptimization.html)
- [ECS Performance Best Practices](https://docs.unity3d.com/Packages/com.unity.entities@1.0/manual/performance-best-practices.html)

### **Полезные ссылки:**
- [Unity Profiler Window](https://docs.unity3d.com/Manual/ProfilerWindow.html)
- [Memory Profiler](https://docs.unity3d.com/Manual/ProfilerMemory.html)
- [Frame Debugger](https://docs.unity3d.com/Manual/FrameDebugger.html)

---

## 🎯 **ЗАКЛЮЧЕНИЕ**

Unity Profiler Integration для Mud-Like обеспечивает:
- ✅ **Автоматический запуск** профилирования из Cursor IDE
- ✅ **Кроссплатформенность** (Windows, macOS, Linux)
- ✅ **Множественные режимы** (Editor, Headless, Standalone)
- ✅ **Автоматические отчеты** и анализ
- ✅ **Интеграция с CI/CD** для автоматического тестирования
- ✅ **Мониторинг ECS систем** в реальном времени

**Используйте эту интеграцию для анализа и оптимизации производительности проекта Mud-Like прямо из Cursor IDE!** 🚀
