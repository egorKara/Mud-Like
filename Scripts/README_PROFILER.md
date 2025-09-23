# 🚗 Unity Profiler Runner для Mud-Like

## 📋 **ОБЗОР**

Автоматический запуск Unity Profiler из Cursor IDE для анализа производительности проекта Mud-Like.

## 🚀 **БЫСТРЫЙ СТАРТ**

### **Linux/macOS:**
```bash
# Запуск профилирования в Unity Editor
./Scripts/run_profiler.sh --mode editor --scene Main

# Запуск headless профилирования
./Scripts/run_profiler.sh --mode headless --report

# Запуск профилирования standalone сборки
./Scripts/run_profiler.sh --mode standalone --build-path ./Builds/MudLike.exe
```

### **Windows:**
```powershell
# Запуск профилирования в Unity Editor
.\Scripts\run_profiler.ps1 -Mode editor -Scene Main

# Запуск headless профилирования
.\Scripts\run_profiler.ps1 -Mode headless -Report

# Запуск профилирования standalone сборки
.\Scripts\run_profiler.ps1 -Mode standalone -BuildPath ".\Builds\MudLike.exe"
```

### **Python (кроссплатформенный):**
```bash
# Запуск профилирования в Unity Editor
python Scripts/run_profiler.py --mode editor --scene Main

# Запуск headless профилирования
python Scripts/run_profiler.py --mode headless --report

# Запуск профилирования standalone сборки
python Scripts/run_profiler.py --mode standalone --build-path ./Builds/MudLike.exe
```

## 📁 **ФАЙЛЫ**

### **Скрипты:**
- **`run_profiler.py`** - Python скрипт (кроссплатформенный)
- **`run_profiler.sh`** - Bash скрипт (Linux/macOS)
- **`run_profiler.ps1`** - PowerShell скрипт (Windows)

### **Unity скрипты:**
- **`ProfilerStarter.cs`** - C# скрипт для Unity
- **`PerformanceMonitoringSystem.cs`** - ECS система мониторинга

## 🔧 **РЕЖИМЫ ПРОФИЛИРОВАНИЯ**

### **1. Editor Mode**
Запускает профилирование в Unity Editor:
```bash
./Scripts/run_profiler.sh --mode editor --scene Main
```

**Особенности:**
- ✅ Полный доступ к Unity Editor
- ✅ Интерактивное профилирование
- ✅ Подробные метрики
- ❌ Требует Unity Editor

### **2. Headless Mode**
Запускает профилирование без GUI:
```bash
./Scripts/run_profiler.sh --mode headless --report
```

**Особенности:**
- ✅ Быстрое профилирование
- ✅ Автоматический отчет
- ✅ Подходит для CI/CD
- ❌ Ограниченная функциональность

### **3. Standalone Mode**
Запускает профилирование standalone сборки:
```bash
./Scripts/run_profiler.sh --mode standalone --build-path ./Builds/MudLike.exe
```

**Особенности:**
- ✅ Реальное профилирование
- ✅ Производительность сборки
- ✅ Подходит для тестирования
- ❌ Требует предварительную сборку

## 📊 **МЕТРИКИ ПРОФИЛИРОВАНИЯ**

### **Основные метрики:**
- **FPS** - кадры в секунду (цель: 60+)
- **Memory** - использование памяти (цель: <2GB)
- **CPU Usage** - использование процессора (цель: <50%)
- **GPU Usage** - использование видеокарты (цель: <80%)

### **ECS системы:**
- **VehicleMovementSystem** - ~0.1ms
- **AdvancedWheelPhysicsSystem** - ~0.5ms
- **TerrainDeformationSystem** - ~1.0ms
- **MainMenuSystem** - ~0.05ms
- **LobbySystem** - ~0.1ms

## 🎯 **НАСТРОЙКА**

### **1. Требования:**
- **Unity 6000.0.57f1** или новее
- **Python 3.7+** (для Python скрипта)
- **PowerShell 5.0+** (для Windows)
- **Bash 4.0+** (для Linux/macOS)

### **2. Установка:**
```bash
# Сделать скрипты исполняемыми
chmod +x Scripts/run_profiler.sh

# Установить зависимости Python (если нужно)
pip install -r Scripts/requirements.txt
```

### **3. Конфигурация:**
Скрипты автоматически создают конфигурацию в `ProfilerData/profiler_config.json`:
```json
{
  "profiler_settings": {
    "enable_profiler": true,
    "profiler_port": 54998,
    "profiler_ip": "127.0.0.1",
    "profiler_connection_mode": "Local"
  },
  "performance_settings": {
    "target_fps": 60,
    "vsync_count": 1,
    "quality_level": 2
  }
}
```

## 📈 **АНАЛИЗ РЕЗУЛЬТАТОВ**

### **1. Unity Profiler:**
1. Откройте Unity Editor
2. Window → Analysis → Profiler
3. Подключитесь к localhost:54998
4. Анализируйте метрики

### **2. HTML отчет:**
Откройте `ProfilerData/ProfilerReport.html` в браузере:
- 📊 Основные метрики производительности
- 🔧 Производительность ECS систем
- 🎯 Рекомендации по оптимизации
- 📁 Ссылки на файлы данных

### **3. JSON данные:**
- **`performance_data.json`** - данные производительности
- **`profiler_config.json`** - конфигурация профилирования
- **`unity_log.txt`** - лог Unity

## 🛠️ **РАСШИРЕННОЕ ИСПОЛЬЗОВАНИЕ**

### **1. Кастомные сцены:**
```bash
# Профилирование конкретной сцены
./Scripts/run_profiler.sh --mode editor --scene KrazTest
```

### **2. Генерация отчета:**
```bash
# Автоматическая генерация HTML отчета
./Scripts/run_profiler.sh --mode headless --report
```

### **3. Кастомные сборки:**
```bash
# Профилирование кастомной сборки
./Scripts/run_profiler.sh --mode standalone --build-path ./CustomBuilds/MudLike.exe
```

### **4. Интеграция с CI/CD:**
```yaml
# GitHub Actions пример
- name: Run Profiler
  run: |
    python Scripts/run_profiler.py --mode headless --report
    # Анализ результатов...
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

Unity Profiler Runner для Mud-Like обеспечивает:
- ✅ **Автоматический запуск** профилирования
- ✅ **Кроссплатформенность** (Windows, macOS, Linux)
- ✅ **Множественные режимы** (Editor, Headless, Standalone)
- ✅ **Автоматические отчеты** и анализ
- ✅ **Интеграция с CI/CD** для автоматического тестирования

**Используйте эти скрипты для анализа и оптимизации производительности проекта Mud-Like!** 🚀
