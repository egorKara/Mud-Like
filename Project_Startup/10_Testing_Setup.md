# 🧪 Mud-Like Testing Setup

## 🎯 **ОБЗОР ТЕСТИРОВАНИЯ**

### **Цель тестирования**
Обеспечить качество, стабильность и производительность проекта Mud-Like через многоуровневое тестирование.

### **Принципы тестирования**
- **Многоуровневость** - Unit, Integration, Functional, Performance
- **Автоматизация** - CI/CD интеграция
- **Покрытие** - Code Coverage 100%
- **Качество** - с первого дня разработки

## 📊 **ТИПЫ ТЕСТИРОВАНИЯ**

### **1. Unit Testing (Модульное тестирование)**
**Цель:** Проверка изолированной логики без зависимостей от Unity.

**Инструменты:**
- Unity Test Framework (UTF)
- NUnit
- Code Coverage package

**Примеры:**
```csharp
// Тестирование математических функций
[Test]
public void CalculateDeformationForce_ValidInput_ReturnsCorrectForce()
{
    // Arrange
    var position = new float3(0, 0, 0);
    var radius = 5f;
    var depth = 2f;
    
    // Act
    var result = TerrainDeformationSystem.CalculateDeformationForce(position, radius, depth);
    
    // Assert
    Assert.AreEqual(expectedForce, result);
}
```

### **2. Integration Testing (Интеграционное тестирование)**
**Цель:** Проверка взаимодействия между компонентами и системами.

**Инструменты:**
- Unity Test Framework (UTF)
- Code Coverage package

**Примеры:**
```csharp
// Тестирование взаимодействия систем
[UnityTest]
public IEnumerator PlayerMovement_InputReceived_MovesPlayer()
{
    // Arrange
    var player = CreatePlayer();
    var input = new Vector2(1, 0);
    
    // Act
    Input.SetAxis("Horizontal", input.x);
    yield return new WaitForFixedUpdate();
    
    // Assert
    Assert.Greater(player.transform.position.x, 0);
}
```

### **3. Functional Testing (Функциональное тестирование)**
**Цель:** Проверка полных рабочих процессов.

**Инструменты:**
- Unity Test Framework (UTF)
- Unity Gaming Services (UGS)

**Примеры:**
```csharp
// Тестирование полного игрового процесса
[UnityTest]
public IEnumerator Gameplay_PlayerJoins_MovesAndInteracts()
{
    // Arrange
    var gameManager = CreateGameManager();
    var player = CreatePlayer();
    
    // Act
    yield return new WaitForSeconds(1f); // Игрок присоединяется
    Input.SetAxis("Horizontal", 1f); // Движение
    yield return new WaitForSeconds(2f);
    Input.SetButtonDown("Interact"); // Взаимодействие
    yield return new WaitForSeconds(1f);
    
    // Assert
    Assert.IsTrue(player.HasInteracted);
    Assert.Greater(player.transform.position.x, 0);
}
```

### **4. Performance Testing (Тестирование производительности)**
**Цель:** Анализ производительности и выявление узких мест.

**Инструменты:**
- Unity Profiler
- Load tests
- Stress tests

**Примеры:**
```csharp
// Тестирование производительности
[Test]
public void TerrainDeformation_PerformanceTest_CompletesInTime()
{
    // Arrange
    var stopwatch = Stopwatch.StartNew();
    var deformationCount = 1000;
    
    // Act
    for (int i = 0; i < deformationCount; i++)
    {
        ApplyDeformation(GetRandomPosition(), 5f, 2f);
    }
    stopwatch.Stop();
    
    // Assert
    Assert.Less(stopwatch.ElapsedMilliseconds, 100); // < 100ms
}
```

## 🛠️ **НАСТРОЙКА ТЕСТИРОВАНИЯ**

### **1. Установка пакетов**
```json
{
  "dependencies": {
    "com.unity.test-framework": "1.5.1",
    "com.unity.testtools.codecoverage": "1.2.6"
  }
}
```

### **2. Настройка Code Coverage**
```json
// ProjectSettings/EditorSettings.asset
m_CodeCoverageEnabled: 1
m_CodeCoveragePackagePath: Packages/com.unity.testtools.codecoverage
```

### **3. Создание структуры тестов**
```
Tests/
├── Unit/                     # Модульные тесты
│   ├── MovementSystemTests.cs
│   ├── TerrainDeformationTests.cs
│   └── NetworkSyncTests.cs
├── Integration/              # Интеграционные тесты
│   ├── VehiclePhysicsTests.cs
│   ├── TerrainInteractionTests.cs
│   └── NetworkCommunicationTests.cs
├── Performance/              # Тесты производительности
│   ├── PerformanceTests.cs
│   ├── MemoryTests.cs
│   └── NetworkPerformanceTests.cs
└── PlayMode/                 # PlayMode тесты
    ├── GameplayTests.cs
    ├── MultiplayerTests.cs
    └── UITests.cs
```

## 🧪 **UNIT ТЕСТЫ**

### **1. Тесты ECS систем**
```csharp
// Tests/Unit/MovementSystemTests.cs
using NUnit.Framework;
using Unity.Entities;
using Unity.Mathematics;

public class MovementSystemTests
{
    [Test]
    public void MovementSystem_ValidInput_MovesEntity()
    {
        // Arrange
        var entity = CreateTestEntity();
        var input = new float2(1, 0);
        
        // Act
        var movementSystem = new MovementSystem();
        movementSystem.ProcessInput(entity, input);
        
        // Assert
        Assert.Greater(entity.Position.x, 0);
    }
    
    [Test]
    public void MovementSystem_ZeroInput_DoesNotMove()
    {
        // Arrange
        var entity = CreateTestEntity();
        var input = float2.zero;
        
        // Act
        var movementSystem = new MovementSystem();
        movementSystem.ProcessInput(entity, input);
        
        // Assert
        Assert.AreEqual(float3.zero, entity.Position);
    }
}
```

### **2. Тесты математических функций**
```csharp
// Tests/Unit/MathUtilsTests.cs
using NUnit.Framework;
using Unity.Mathematics;

public class MathUtilsTests
{
    [Test]
    public void CalculateDistance_ValidPoints_ReturnsCorrectDistance()
    {
        // Arrange
        var point1 = new float3(0, 0, 0);
        var point2 = new float3(3, 4, 0);
        
        // Act
        var distance = MathUtils.CalculateDistance(point1, point2);
        
        // Assert
        Assert.AreEqual(5f, distance, 0.001f);
    }
    
    [Test]
    public void NormalizeVector_ValidVector_ReturnsNormalizedVector()
    {
        // Arrange
        var vector = new float3(3, 4, 0);
        
        // Act
        var normalized = MathUtils.Normalize(vector);
        
        // Assert
        Assert.AreEqual(1f, math.length(normalized), 0.001f);
    }
}
```

### **3. Тесты компонентов**
```csharp
// Tests/Unit/ComponentTests.cs
using NUnit.Framework;
using Unity.Entities;

public class ComponentTests
{
    [Test]
    public void PositionComponent_DefaultValue_IsZero()
    {
        // Arrange & Act
        var position = new Position();
        
        // Assert
        Assert.AreEqual(float3.zero, position.Value);
    }
    
    [Test]
    public void HealthComponent_DefaultValue_IsMaxHealth()
    {
        // Arrange & Act
        var health = new Health { MaxValue = 100f };
        
        // Assert
        Assert.AreEqual(100f, health.MaxValue);
        Assert.AreEqual(100f, health.Value);
    }
}
```

## 🔗 **INTEGRATION ТЕСТЫ**

### **1. Тесты взаимодействия систем**
```csharp
// Tests/Integration/SystemInteractionTests.cs
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class SystemInteractionTests
{
    [UnityTest]
    public IEnumerator PlayerMovement_InputSystemAndMovementSystem_WorksTogether()
    {
        // Arrange
        var player = CreatePlayer();
        var input = new Vector2(1, 0);
        
        // Act
        Input.SetAxis("Horizontal", input.x);
        yield return new WaitForFixedUpdate();
        
        // Assert
        Assert.Greater(player.transform.position.x, 0);
    }
    
    [UnityTest]
    public IEnumerator VehiclePhysics_WheelSystemAndMovementSystem_WorksTogether()
    {
        // Arrange
        var vehicle = CreateVehicle();
        var input = new Vector2(0, 1);
        
        // Act
        Input.SetAxis("Vertical", input.y);
        yield return new WaitForFixedUpdate();
        
        // Assert
        Assert.Greater(vehicle.transform.position.z, 0);
    }
}
```

### **2. Тесты сетевого взаимодействия**
```csharp
// Tests/Integration/NetworkCommunicationTests.cs
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class NetworkCommunicationTests
{
    [UnityTest]
    public IEnumerator NetworkSync_ClientSendsPosition_ServerReceives()
    {
        // Arrange
        var client = CreateClient();
        var server = CreateServer();
        var position = new float3(1, 2, 3);
        
        // Act
        client.SendPosition(position);
        yield return new WaitForSeconds(0.1f);
        
        // Assert
        Assert.AreEqual(position, server.GetLastReceivedPosition());
    }
}
```

## 🎮 **FUNCTIONAL ТЕСТЫ**

### **1. Тесты игрового процесса**
```csharp
// Tests/Functional/GameplayTests.cs
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class GameplayTests
{
    [UnityTest]
    public IEnumerator Gameplay_PlayerJoinsAndMoves_CompletesSuccessfully()
    {
        // Arrange
        var gameManager = CreateGameManager();
        var player = CreatePlayer();
        
        // Act
        yield return new WaitForSeconds(1f); // Игрок присоединяется
        Input.SetAxis("Horizontal", 1f); // Движение
        yield return new WaitForSeconds(2f);
        Input.SetButtonDown("Interact"); // Взаимодействие
        yield return new WaitForSeconds(1f);
        
        // Assert
        Assert.IsTrue(player.HasInteracted);
        Assert.Greater(player.transform.position.x, 0);
    }
    
    [UnityTest]
    public IEnumerator Multiplayer_TwoPlayersJoin_CanSeeEachOther()
    {
        // Arrange
        var gameManager = CreateMultiplayerGame();
        var player1 = CreatePlayer();
        var player2 = CreatePlayer();
        
        // Act
        yield return new WaitForSeconds(2f); // Игроки присоединяются
        player1.MoveTo(new Vector3(1, 0, 0));
        yield return new WaitForSeconds(1f);
        
        // Assert
        Assert.IsTrue(player2.CanSee(player1));
    }
}
```

### **2. Тесты UI**
```csharp
// Tests/Functional/UITests.cs
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class UITests
{
    [UnityTest]
    public IEnumerator MainMenu_StartButtonClicked_LoadsGame()
    {
        // Arrange
        var mainMenu = CreateMainMenu();
        var startButton = mainMenu.GetStartButton();
        
        // Act
        startButton.Click();
        yield return new WaitForSeconds(1f);
        
        // Assert
        Assert.IsTrue(SceneManager.GetActiveScene().name == "Game");
    }
}
```

## ⚡ **PERFORMANCE ТЕСТЫ**

### **1. Тесты производительности**
```csharp
// Tests/Performance/PerformanceTests.cs
using NUnit.Framework;
using System.Diagnostics;

public class PerformanceTests
{
    [Test]
    public void TerrainDeformation_PerformanceTest_CompletesInTime()
    {
        // Arrange
        var stopwatch = Stopwatch.StartNew();
        var deformationCount = 1000;
        
        // Act
        for (int i = 0; i < deformationCount; i++)
        {
            ApplyDeformation(GetRandomPosition(), 5f, 2f);
        }
        stopwatch.Stop();
        
        // Assert
        Assert.Less(stopwatch.ElapsedMilliseconds, 100); // < 100ms
    }
    
    [Test]
    public void NetworkSync_PerformanceTest_HandlesManyPlayers()
    {
        // Arrange
        var stopwatch = Stopwatch.StartNew();
        var playerCount = 100;
        
        // Act
        for (int i = 0; i < playerCount; i++)
        {
            SyncPlayerPosition(CreatePlayer());
        }
        stopwatch.Stop();
        
        // Assert
        Assert.Less(stopwatch.ElapsedMilliseconds, 50); // < 50ms
    }
}
```

### **2. Тесты памяти**
```csharp
// Tests/Performance/MemoryTests.cs
using NUnit.Framework;
using UnityEngine;

public class MemoryTests
{
    [Test]
    public void ObjectPooling_MemoryTest_NoMemoryLeaks()
    {
        // Arrange
        var initialMemory = GC.GetTotalMemory(false);
        
        // Act
        for (int i = 0; i < 1000; i++)
        {
            var obj = ObjectPool.Get();
            ObjectPool.Return(obj);
        }
        GC.Collect();
        var finalMemory = GC.GetTotalMemory(false);
        
        // Assert
        Assert.Less(finalMemory - initialMemory, 1024 * 1024); // < 1MB
    }
}
```

## 🔄 **CI/CD ИНТЕГРАЦИЯ**

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
          
      - name: Upload Coverage
        uses: codecov/codecov-action@v3
        with:
          file: ./coverage.xml
```

### **2. Локальная настройка**
```bash
# Запуск тестов локально
unity -batchmode -quit -projectPath . -runTests -testResults results.xml

# Запуск с покрытием кода
unity -batchmode -quit -projectPath . -runTests -testResults results.xml -enableCodeCoverage
```

## 📊 **МЕТРИКИ КАЧЕСТВА**

### **1. Code Coverage**
- **Цель:** >80% покрытия кода
- **Инструмент:** Code Coverage package
- **Отчеты:** HTML, XML, JSON

### **2. Performance Metrics**
- **FPS:** 60+ на целевой аппаратуре
- **Memory:** <2GB для 100 игроков
- **Network:** <100ms задержка

### **3. Quality Gates**
- **Все тесты** должны проходить
- **Code Coverage** >80%
- **Performance** в пределах нормы
- **Нет критических** ошибок

## 🎯 **РЕЗУЛЬТАТ НАСТРОЙКИ**

### **После настройки тестирования:**
- ✅ **Unit тесты** покрывают основную логику
- ✅ **Integration тесты** проверяют взаимодействия
- ✅ **Functional тесты** тестируют полные процессы
- ✅ **Performance тесты** обеспечивают производительность
- ✅ **CI/CD** автоматизирует тестирование
- ✅ **Code Coverage** 100%

### **Готовность к разработке:**
- ✅ **Система тестирования** настроена
- ✅ **Автоматизация** работает
- ✅ **Метрики качества** отслеживаются
- ✅ **Регрессии** обнаруживаются быстро

---

**Правильная настройка тестирования критически важна для качества проекта Mud-Like. Следуйте этому руководству для создания надежной системы тестирования.**
