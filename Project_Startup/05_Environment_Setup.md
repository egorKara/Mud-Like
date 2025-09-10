# ⚙️ Mud-Like Environment Setup

## 🎯 **СИСТЕМНЫЕ ТРЕБОВАНИЯ**

### **Минимальные требования**
- **OS:** Windows 10/11 или Linux (Ubuntu 20.04+)
- **CPU:** Intel i5-8400 / AMD Ryzen 5 2600
- **RAM:** 16 GB
- **GPU:** NVIDIA GTX 1060 / AMD RX 580
- **Storage:** 50 GB свободного места
- **Network:** Стабильное интернет-соединение

### **Рекомендуемые требования**
- **OS:** Windows 11 или Linux (Ubuntu 22.04+)
- **CPU:** Intel i7-10700K / AMD Ryzen 7 3700X
- **RAM:** 32 GB
- **GPU:** NVIDIA RTX 3070 / AMD RX 6700 XT
- **Storage:** 100 GB SSD
- **Network:** Высокоскоростное соединение

## 🛠️ **УСТАНОВКА UNITY**

### **1. Скачивание Unity Hub**
```bash
# Windows
# Скачать с https://unity.com/download

# Linux
wget https://public-cdn.cloud.unity3d.com/hub/prod/UnityHub.AppImage
chmod +x UnityHub.AppImage
./UnityHub.AppImage
```

### **2. Установка Unity Editor**
- **Версия:** Unity 6000.2.2f1
- **Модули:** Windows Build Support, Linux Build Support
- **Дополнительно:** Visual Studio Code Support

### **3. Настройка лицензии**
- **Personal License** для разработки
- **Pro License** для коммерческого использования
- **Активация** через Unity Hub

## 📦 **УСТАНОВКА ПАКЕТОВ**

### **1. Создание нового проекта**
```bash
# Создать новый 3D проект
# Название: Mud-Like
# Шаблон: 3D (URP)
# Версия: 6000.2.2f1
```

### **2. Настройка Package Manager**
```json
{
  "dependencies": {
    "com.unity.entities": "1.3.14",
    "com.unity.collections": "2.5.7",
    "com.unity.burst": "1.8.24",
    "com.unity.mathematics": "1.3.2",
    "com.unity.physics": "1.3.14",
    "com.unity.entities.graphics": "1.4.12",
    "com.unity.netcode": "1.6.2",
    "com.unity.test-framework": "1.5.1",
    "com.unity.testtools.codecoverage": "1.2.6",
    "com.unity.ui.builder": "2.0.0"
  }
}
```

### **3. Установка через Package Manager**
1. **Window → Package Manager**
2. **In Project → Entities** → Install
3. **In Project → Physics** → Install
4. **In Project → Netcode for Entities** → Install
5. **In Project → Test Framework** → Install
6. **In Project → UI Toolkit** → Install

## 🔧 **НАСТРОЙКА IDE**

### **1. Visual Studio Code / Cursor**
```bash
# Установка расширений
code --install-extension ms-vscode.csharp
code --install-extension unity.unity-debug
code --install-extension ms-dotnettools.csharp
code --install-extension ms-dotnettools.vscode-dotnet-runtime
```

### **2. Настройка C# IntelliSense**
```json
// .vscode/settings.json
{
    "omnisharp.useModernNet": true,
    "omnisharp.enableEditorConfigSupport": true,
    "omnisharp.enableImportCompletion": true,
    "omnisharp.enableRoslynAnalyzers": true
}
```

### **3. Настройка Unity Integration**
```json
// .vscode/launch.json
{
    "version": "0.2.0",
    "configurations": [
        {
            "name": "Unity Editor",
            "type": "unity",
            "request": "launch"
        },
        {
            "name": "Windows Player",
            "type": "unity",
            "request": "launch"
        },
        {
            "name": "OSX Player",
            "type": "unity",
            "request": "launch"
        },
        {
            "name": "Linux Player",
            "type": "unity",
            "request": "launch"
        }
    ]
}
```

## 🧪 **НАСТРОЙКА ТЕСТИРОВАНИЯ**

### **1. Unity Test Framework**
```csharp
// Tests/Editor/TestExample.cs
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TestExample
{
    [Test]
    public void TestExample_SimplePasses()
    {
        Assert.AreEqual(2 + 2, 4);
    }

    [UnityTest]
    public IEnumerator TestExample_WithEnumeratorPasses()
    {
        yield return null;
        Assert.AreEqual(1, 1);
    }
}
```

### **2. Code Coverage**
```json
// ProjectSettings/EditorSettings.asset
m_CodeCoverageEnabled: 1
m_CodeCoveragePackagePath: Packages/com.unity.testtools.codecoverage
```

### **3. Настройка Test Runner**
1. **Window → General → Test Runner**
2. **EditMode** для модульных тестов
3. **PlayMode** для интеграционных тестов
4. **Code Coverage** для измерения покрытия

## 🌐 **НАСТРОЙКА СЕТИ**

### **1. Netcode for Entities**
```csharp
// Scripts/Networking/NetworkManager.cs
using Unity.Networking.Transport;
using Unity.NetCode;

public class NetworkManager : MonoBehaviour
{
    public NetworkDriver driver;
    public NetworkPipeline pipeline;
    
    void Start()
    {
        driver = NetworkDriver.Create();
        pipeline = driver.CreatePipeline(typeof(ReliableSequencedPipelineStage));
    }
}
```

### **2. Настройка World**
```csharp
// Scripts/Networking/WorldSetup.cs
using Unity.Entities;
using Unity.NetCode;

public class WorldSetup : MonoBehaviour
{
    void Start()
    {
        // Создание HostGameWorld
        var hostWorld = new World("HostGameWorld");
        World.DefaultGameObjectInjectionWorld = hostWorld;
        
        // Создание ServerGameWorld
        var serverWorld = new World("ServerGameWorld");
        serverWorld.GetOrCreateSystem<ServerSimulationSystemGroup>();
    }
}
```

## 📁 **СТРУКТУРА ПРОЕКТА**

### **1. Создание папок**
```bash
# Создание структуры папок
mkdir -p Assets/Scripts/Core
mkdir -p Assets/Scripts/Vehicles
mkdir -p Assets/Scripts/Terrain
mkdir -p Assets/Scripts/UI
mkdir -p Assets/Scripts/Pooling
mkdir -p Assets/Scripts/Networking
mkdir -p Assets/Scripts/Audio
mkdir -p Assets/Scripts/Effects
mkdir -p Assets/Scripts/Tests
mkdir -p Assets/Scripts/DOTS
mkdir -p Assets/Prefabs
mkdir -p Assets/Materials
mkdir -p Assets/Textures
mkdir -p Assets/Audio
mkdir -p Assets/Scenes
mkdir -p Assets/Tests
```

### **2. Настройка Assembly Definitions**
```json
// Assets/Scripts/Core/Mud-Like.Core.asmdef
{
    "name": "Mud-Like.Core",
    "rootNamespace": "Mud-Like.Core",
    "references": [
        "Unity.Entities",
        "Unity.Collections",
        "Unity.Jobs",
        "Unity.Burst"
    ],
    "includePlatforms": [],
    "excludePlatforms": [],
    "allowUnsafeCode": true
}
```

## 🎮 **НАСТРОЙКА ИГРОВЫХ СИСТЕМ**

### **1. URP (Universal Render Pipeline)**
```csharp
// Assets/Settings/URP-HighFidelity-Renderer.asset
// Настройка для высокой производительности
// LOD Bias: 1.0
// Maximum LOD Level: 0
// Shadow Distance: 150
```

### **2. Physics Settings**
```csharp
// Project Settings → Physics
// Gravity: (0, -9.81, 0)
// Default Material: Mud-Like Physics Material
// Bounce Threshold: 2
// Sleep Threshold: 0.005
// Default Solver Iterations: 6
// Default Solver Velocity Iterations: 1
```

### **3. Input System**
```csharp
// Assets/Input/PlayerInput.inputactions
// Настройка ввода для движения, стрельбы, взаимодействия
// Action Maps: Player, UI, Vehicle
// Actions: Movement, Jump, Brake, Shoot, Interact
```

## 🔍 **НАСТРОЙКА ПРОФИЛИРОВАНИЯ**

### **1. Unity Profiler**
```csharp
// Scripts/Debug/ProfilerSetup.cs
using UnityEngine.Profiling;

public class ProfilerSetup : MonoBehaviour
{
    void Start()
    {
        // Включение профилирования в Development Build
        if (Debug.isDebugBuild)
        {
            Profiler.enabled = true;
        }
    }
}
```

### **2. Memory Profiler**
```csharp
// Scripts/Debug/MemoryProfilerSetup.cs
using Unity.MemoryProfiler;

public class MemoryProfilerSetup : MonoBehaviour
{
    void Start()
    {
        // Настройка Memory Profiler
        MemoryProfiler.TakeSnapshot();
    }
}
```

## 🧪 **НАСТРОЙКА CI/CD**

### **1. GitHub Actions**
```yaml
# .github/workflows/ci-cd-pipeline.yml
name: CI/CD Pipeline
on: [push, pull_request]
jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      - name: Setup Unity
        uses: game-ci/unity-setup@v2
        with:
          unity-version: 6000.2.2f1
      - name: Run Tests
        uses: game-ci/unity-test-runner@v2
        with:
          testMode: all
          coverageOptions: enableCodeCoverage
```

### **2. Локальная настройка**
```bash
# Установка Unity CLI
# Windows
# Скачать Unity Hub и установить через него

# Linux
wget https://public-cdn.cloud.unity3d.com/hub/prod/UnityHub.AppImage
chmod +x UnityHub.AppImage
./UnityHub.AppImage
```

## ✅ **ПРОВЕРКА НАСТРОЙКИ**

### **1. Проверка Unity**
- [ ] Unity 6000.2.2f1 установлен
- [ ] Все пакеты DOTS установлены
- [ ] Проект создан и открывается
- [ ] Нет ошибок в Console

### **2. Проверка IDE**
- [ ] Visual Studio Code / Cursor установлен
- [ ] C# расширения установлены
- [ ] IntelliSense работает
- [ ] Unity Integration настроен

### **3. Проверка тестирования**
- [ ] Test Runner открывается
- [ ] Тесты запускаются
- [ ] Code Coverage работает
- [ ] CI/CD настроен

### **4. Проверка сети**
- [ ] Netcode for Entities установлен
- [ ] World создается без ошибок
- [ ] NetworkManager работает
- [ ] Сетевая конфигурация корректна

## 🎯 **СЛЕДУЮЩИЕ ШАГИ**

После завершения настройки окружения:

1. **Изучите** [06_Package_Configuration.md](06_Package_Configuration.md)
2. **Настройте** структуру проекта
3. **Создайте** первый ECS прототип
4. **Настройте** систему тестирования

---

**Правильная настройка окружения критически важна для успешной разработки Mud-Like. Следуйте этому руководству шаг за шагом для создания стабильной и производительной среды разработки.**
