# 🌐 Интеграционные тесты для Netcode for Entities (NFE)

## 📋 **ОБЗОР**

Комплексная система интеграционных тестов для Netcode for Entities в проекте Mud-Like. Тестирует полную интеграцию сетевых систем, включая синхронизацию, команды, компенсацию задержек и античит.

## 🚀 **СТРУКТУРА ТЕСТОВ**

```
Assets/Scripts/Tests/Integration/
├── NFEIntegrationTests.cs          # Базовые интеграционные тесты
├── NetworkPositionSyncTests.cs     # Тесты синхронизации позиций
├── NetworkCommandTests.cs          # Тесты сетевых команд
├── LagCompensationTests.cs         # Тесты компенсации задержек
├── AntiCheatTests.cs               # Тесты античит системы
├── NFEPerformanceTests.cs          # Тесты производительности
└── README_NFEIntegrationTests.md   # Документация
```

## 🎯 **ТИПЫ ТЕСТОВ**

### **1. Базовые интеграционные тесты (NFEIntegrationTests.cs)**
- Создание сетевых сущностей
- Инициализация сетевых компонентов
- Синхронизация позиций между клиентом и сервером
- Создание и выполнение сетевых команд
- Компенсация задержек
- Валидация позиций для античита

### **2. Тесты синхронизации позиций (NetworkPositionSyncTests.cs)**
- Синхронизация позиций от клиента к серверу
- Синхронизация позиций от сервера к клиенту
- Синхронизация с пороговыми значениями
- Синхронизация скорости и угловой скорости
- Синхронизация множественных сущностей
- Синхронизация различных типов сущностей
- Производительность синхронизации

### **3. Тесты сетевых команд (NetworkCommandTests.cs)**
- Создание команд движения
- Создание команд изменения скорости
- Создание команд управления транспортом
- Обработка команд движения
- Обработка команд изменения скорости
- Обработка команд управления транспортом
- Обработка множественных команд
- Обработка команд различных типов
- Валидация команд с недопустимыми значениями
- Валидация команд с устаревшим временем
- Валидация команд с несуществующей целью
- Производительность обработки команд

### **4. Тесты компенсации задержек (LagCompensationTests.cs)**
- Компенсация задержек для позиций
- Компенсация задержек для поворота
- Компенсация задержек с различными RTT
- Предсказание позиций на основе скорости
- Предсказание позиций с ускорением
- Предсказание позиций с ограничениями
- Интерполяция между позициями
- Интерполяция с различными скоростями
- Производительность компенсации
- Валидация компенсированных позиций

### **5. Тесты античит системы (AntiCheatTests.cs)**
- Обнаружение телепортации
- Обнаружение превышения скорости
- Обнаружение полета
- Обнаружение прохождения через стены
- Обнаружение макросов
- Обнаружение ботов
- Обнаружение манипуляций со временем
- Обнаружение ускорения времени
- Обнаружение недопустимых значений
- Обнаружение бесконечных значений
- Производительность античита
- Отсутствие ложных срабатываний

### **6. Тесты производительности (NFEPerformanceTests.cs)**
- Производительность синхронизации позиций
- Производительность с частыми обновлениями
- Производительность с различными типами сущностей
- Производительность компенсации задержек
- Производительность компенсации с различными RTT
- Производительность античит системы
- Производительность с подозрительными игроками
- Производительность обработки сетевых команд
- Производительность обработки команд различных типов
- Производительность памяти
- Производительность сетевого трафика
- Производительность сжатия сетевых данных

## 🛠️ **ЗАПУСК ТЕСТОВ**

### **Запуск всех интеграционных тестов**
```bash
# В Unity Test Runner
Window > General > Test Runner > Integration Tests > Run All

# Или через командную строку
Unity -batchmode -quit -projectPath . -runTests -testPlatform playmode -testCategory Integration
```

### **Запуск конкретных тестов**
```bash
# Тесты синхронизации позиций
Unity -batchmode -quit -projectPath . -runTests -testPlatform playmode -testCategory "PositionSync"

# Тесты античит системы
Unity -batchmode -quit -projectPath . -runTests -testPlatform playmode -testCategory "AntiCheat"

# Тесты производительности
Unity -batchmode -quit -projectPath . -runTests -testPlatform playmode -testCategory "Performance"
```

## 📊 **МЕТРИКИ ПРОИЗВОДИТЕЛЬНОСТИ**

### **Целевые показатели производительности**
- **Синхронизация позиций**: < 50мс для 1000 сущностей
- **Компенсация задержек**: < 30мс для 1000 сущностей
- **Античит система**: < 100мс для 1000 игроков
- **Обработка команд**: < 200мс для 1000 команд
- **Использование памяти**: < 100MB для 10000 сущностей

### **Метрики качества**
- **Покрытие тестами**: > 90% для сетевых систем
- **Время выполнения**: < 5 минут для всех тестов
- **Стабильность**: 0% ложных срабатываний
- **Точность**: 100% обнаружение читерства

## 🔧 **НАСТРОЙКА ТЕСТОВ**

### **Конфигурация тестовой среды**
```csharp
[SetUp]
public override void SetUp()
{
    base.SetUp();
    
    // Настройка тестовой среды
    ConfigureTestEnvironment();
    
    // Создание тестовых сущностей
    CreateTestEntities();
    
    // Инициализация систем
    InitializeSystems();
}
```

### **Настройка сетевых параметров**
```csharp
// Настройка пороговых значений
const float positionThreshold = 0.01f;
const float rotationThreshold = 0.01f;
const float maxSpeed = 100f;
const float maxHeight = 1000f;

// Настройка RTT для тестирования
var rttValues = new float[] { 0.05f, 0.1f, 0.2f, 0.5f };
```

## 📝 **ПРИМЕРЫ ИСПОЛЬЗОВАНИЯ**

### **Пример 1: Тест синхронизации позиций**
```csharp
[Test]
public void SyncPosition_ClientToServer_ShouldUpdateServerPosition()
{
    // Arrange
    var clientPosition = new float3(100f, 5f, 200f);
    var clientRotation = quaternion.RotateY(math.radians(45f));
    
    SetClientPosition(_clientEntity, clientPosition, clientRotation);
    
    // Act
    _networkSyncSystem.Update();
    
    // Assert
    var serverNetworkPosition = EntityManager.GetComponentData<NetworkPosition>(_serverEntity);
    Assert.AreEqual(clientPosition, serverNetworkPosition.Value);
    Assert.AreEqual(clientRotation, serverNetworkPosition.Rotation);
    Assert.IsTrue(serverNetworkPosition.HasChanged);
}
```

### **Пример 2: Тест компенсации задержек**
```csharp
[Test]
public void CompensateLag_ForPosition_ShouldCompensateCorrectly()
{
    // Arrange
    var networkPosition = new NetworkPosition
    {
        Value = new float3(0f, 0f, 0f),
        Velocity = new float3(10f, 0f, 5f),
        LastUpdateTime = (float)Time.time - 0.1f, // 100мс задержка
        HasChanged = true
    };
    
    EntityManager.SetComponentData(_clientEntity, networkPosition);
    
    // Act
    _lagCompensationSystem.Update();
    
    // Assert
    var compensatedPosition = EntityManager.GetComponentData<NetworkPosition>(_clientEntity);
    Assert.IsTrue(compensatedPosition.Value.x > 0f);
    Assert.IsTrue(compensatedPosition.Value.z > 0f);
}
```

### **Пример 3: Тест античит системы**
```csharp
[Test]
public void DetectTeleportation_WithLargePositionChange_ShouldFlagAsSuspicious()
{
    // Arrange
    var networkPosition = new NetworkPosition
    {
        Value = new float3(0f, 0f, 0f),
        LastUpdateTime = (float)Time.time - 0.1f,
        HasChanged = true
    };
    
    EntityManager.SetComponentData(_playerEntity, networkPosition);
    
    // Симулируем телепортацию
    var teleportedPosition = new NetworkPosition
    {
        Value = new float3(1000f, 0f, 1000f), // Очень далеко
        LastUpdateTime = (float)Time.time,
        HasChanged = true
    };
    
    EntityManager.SetComponentData(_playerEntity, teleportedPosition);
    
    // Act
    _antiCheatSystem.Update();
    
    // Assert
    Assert.IsTrue(EntityManager.HasComponent<AntiCheatFlag>(_playerEntity));
    var flag = EntityManager.GetComponentData<AntiCheatFlag>(_playerEntity);
    Assert.IsTrue(flag.IsSuspicious);
    Assert.AreEqual("Teleportation detected", flag.Reason);
}
```

## 🎯 **ЛУЧШИЕ ПРАКТИКИ**

### **1. Структура тестов**
- Используйте четкие имена тестов
- Группируйте связанные тесты в классы
- Используйте SetUp и TearDown для инициализации
- Добавляйте комментарии к сложным тестам

### **2. Управление данными**
- Создавайте тестовые данные в SetUp
- Очищайте данные в TearDown
- Используйте константы для магических чисел
- Группируйте связанные данные в структуры

### **3. Проверки (Assertions)**
- Используйте конкретные проверки
- Проверяйте все важные свойства
- Добавляйте информативные сообщения об ошибках
- Используйте Assert.IsTrue с описательными сообщениями

### **4. Производительность**
- Устанавливайте разумные лимиты времени
- Тестируйте с различными размерами данных
- Измеряйте использование памяти
- Профилируйте медленные тесты

## 🔍 **ОТЛАДКА ТЕСТОВ**

### **Общие проблемы**
1. **Тесты падают с таймаутом**
   - Увеличьте лимиты времени
   - Проверьте производительность систем
   - Оптимизируйте код

2. **Ложные срабатывания**
   - Проверьте пороговые значения
   - Убедитесь в корректности тестовых данных
   - Проверьте логику тестов

3. **Проблемы с памятью**
   - Проверьте утечки памяти
   - Очищайте ресурсы в TearDown
   - Используйте профилирование памяти

### **Инструменты отладки**
- Unity Profiler для анализа производительности
- Unity Memory Profiler для анализа памяти
- Unity Test Runner для запуска тестов
- Console для просмотра логов

## 📚 **ДОПОЛНИТЕЛЬНЫЕ РЕСУРСЫ**

### **Документация Unity**
- [Unity Test Framework](https://docs.unity3d.com/Packages/com.unity.test-framework@latest/)
- [Netcode for Entities](https://docs.unity3d.com/Packages/com.unity.netcode@latest/)
- [Unity ECS](https://docs.unity3d.com/Packages/com.unity.entities@latest/)

### **Полезные ссылки**
- [Unity Testing Best Practices](https://docs.unity3d.com/Manual/testing-best-practices.html)
- [Performance Testing](https://docs.unity3d.com/Manual/performance-testing.html)
- [Memory Profiler](https://docs.unity3d.com/Manual/ProfilerMemory.html)

## 🎯 **ЗАКЛЮЧЕНИЕ**

Интеграционные тесты для NFE обеспечивают:

- 🚀 **Надежность** - полное тестирование сетевых систем
- 🎯 **Качество** - обнаружение проблем на раннем этапе
- 📊 **Производительность** - контроль производительности
- 🔒 **Безопасность** - защита от читерства
- 📈 **Масштабируемость** - тестирование с большими нагрузками

**Используйте эти тесты для обеспечения стабильной и производительной сетевой игры!**
