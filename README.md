# 🚗 Mud-Like

Минимально рабочая версия «MudRunner-like» игры с реалистичной физикой внедорожника и деформируемой грязью.

## 🎯 **ЦЕЛЬ ПРОЕКТА**

Создать мультиплеерную игру с:
- ✅ **Реалистичной физикой** внедорожника
- ✅ **Деформацией террейна** под колесами
- ✅ **Детерминированной симуляцией** для мультиплеера
- ✅ **ECS-архитектурой** для производительности

## 🛠️ **ТЕХНОЛОГИИ**

- **Unity:** 6000.0.57f1
- **Архитектура:** ECS (Entity Component System)
- **Физика:** Unity Physics (DOTS)
- **Мультиплеер:** Netcode for Entities
- **Рендеринг:** Universal Render Pipeline (URP)

## 📁 **СТРУКТУРА ПРОЕКТА**

```
Assets/
├── Scripts/
│   ├── Core/           # Основные системы
│   ├── Vehicles/       # Транспорт (ECS + Unity Physics)
│   ├── Terrain/        # Террейн и деформация (DOTS)
│   ├── UI/             # Пользовательский интерфейс
│   ├── Networking/     # Мультиплеер (Netcode)
│   ├── Tests/          # Тесты
│   └── DOTS/           # DOTS системы
├── Prefabs/            # Префабы
├── Materials/          # Материалы
├── Scenes/             # Сцены
└── Settings/           # Настройки проекта
```

## 🚀 **БЫСТРЫЙ СТАРТ**

### **1. Требования**
- Unity 6000.0.57f1
- Git

### **2. Установка**
```bash
# Клонировать репозиторий
git clone https://github.com/ВАШ_USERNAME/Mud-Like.git
cd Mud-Like

# Открыть проект в Unity
# Unity автоматически установит все пакеты
```

### **3. Запуск**
1. Откройте проект в Unity 6000.0.57f1
2. Дождитесь установки пакетов
3. Откройте сцену `Assets/Scenes/Main.unity`
4. Нажмите Play

## 📚 **ДОКУМЕНТАЦИЯ**

### **Основная документация (Project_Startup/):**
- `01_Project_Overview.md` - Обзор проекта
- `02_Architecture_Design.md` - Архитектурный дизайн
- `03_Technology_Stack.md` - Технологический стек
- `05_Environment_Setup.md` - Настройка окружения
- `06_Package_Configuration.md` - Конфигурация пакетов
- `07_Project_Structure.md` - Структура проекта
- `08_ECS_Migration_Guide.md` - Руководство по ECS
- `09_First_Prototype.md` - Первый прототип

### **Документация критических систем:**
- `SYSTEMS_INTEGRATION_ARCHITECTURE.md` - Архитектура интеграции систем
- `Assets/Scripts/Terrain/Systems/MudManagerSystem.md` - API деформации террейна
- `Assets/Scripts/Networking/Systems/InputValidationSystem.md` - Валидация ввода
- `Assets/Scripts/Pooling/Systems/MudParticlePoolSystem.md` - Пулинг частиц
- `Assets/Scripts/Terrain/Systems/WorldGridSystem.md` - Управление загрузкой
- `Assets/Scripts/Networking/Systems/LagCompensationSystem.md` - Компенсация задержек

## 🧪 **ТЕСТИРОВАНИЕ**

```bash
# Запуск тестов
# В Unity: Window → General → Test Runner
# Или через командную строку:
unity -batchmode -quit -projectPath . -runTests -testResults results.xml
```

## 📊 **МЕТРИКИ КАЧЕСТВА**

- **Code Coverage:** >80%
- **FPS:** 60+ на целевой аппаратуре
- **Memory:** <2GB для 100 игроков
- **Network:** <100ms задержка

## 🤝 **РАЗРАБОТКА**

### **Правила кодирования**
- ✅ **Чистая ECS-архитектура** - полный переход от MonoBehaviour завершен
- ✅ **Детерминизм** - только SystemAPI.Time.fixedDeltaTime
- ✅ **Тестирование** - Code Coverage >80%
- ✅ **Документация** - XML комментарии для всех API

### **Архитектурные принципы**
- ✅ **Data-Oriented Technology Stack (DOTS)**
- ✅ **Clean Architecture**
- ✅ **Принцип "Сложное из простого"**
- ✅ **Разделение ответственности**

## 🎯 **КРИТИЧЕСКИЕ СИСТЕМЫ**

### **✅ Реализованные системы:**
- **MudManagerSystem** - API деформации террейна с QueryContact
- **InputValidationSystem** - серверная валидация ввода и защита от читов
- **LagCompensationSystem** - компенсация задержек для честного мультиплеера
- **WorldGridSystem** - управление загрузкой террейна блоками 16×16
- **MudParticlePoolSystem** - эффективный пулинг частиц грязи
- **TerrainSyncSystem** - синхронизация деформаций между клиентами

### **🔧 Интеграция систем:**
- **Полная ECS архитектура** - все системы используют Entity Component System
- **Детерминированная симуляция** - все вычисления детерминированы для мультиплеера
- **Burst Compiler оптимизация** - критические методы оптимизированы
- **Unity Physics интеграция** - использование Unity Physics (DOTS)

## 📄 **ЛИЦЕНЗИЯ**

MIT License - см. файл [LICENSE](LICENSE)

## 👥 **АВТОРЫ**

- **Разработчик**: Mud-Like Developer
- **Email**: egor@mud-like.dev

## 🎯 **СТАТУС ПРОЕКТА**

- ✅ **Документация**: Полная (18 файлов, 8000+ строк)
- ✅ **Unity проект**: Создан и настроен
- ✅ **ECS архитектура**: Базовая структура готова
- ✅ **Package configuration**: Настроена для Unity 2022.3.62f1
- 🔄 **Разработка**: В процессе
- ⏳ **Мультиплеер**: Планируется
- ⏳ **Деформация террейна**: Планируется

---

**Удачи в разработке Mud-Like! 🎮**
