using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;
using Unity.Jobs;
using Unity.Collections;
using MudLike.Core.Components;
using static MudLike.Core.Components.Position;

namespace MudLike.Core.ECS
{
    /// <summary>
    /// Примеры использования контекстных промптов для ECS модулей
    /// Демонстрирует, как использовать @context для оптимизации работы с AI ассистентами
    /// </summary>
    public static class ECSPromptExamples
    {
        #region Примеры использования контекстных промптов
        
        /// <summary>
        /// Пример 1: Создание транспортной системы с контекстом
        /// </summary>
        public static string CreateVehicleSystemExample()
        {
            return @"
@context Используй VehicleECSPromptContext.VehicleSystemContext

Создай систему движения транспорта с следующими требованиями:
- Используй VehiclePhysics, VehicleConfig, VehicleInput компоненты
- Реализуй детерминированное движение
- Добавь поддержку рулевого управления
- Используй BurstCompile для производительности

Система должна обрабатывать:
1. Движение вперед/назад на основе ввода
2. Поворот на основе рулевого управления
3. Применение сопротивления и инерции
4. Ограничение максимальной скорости
";
        }
        
        /// <summary>
        /// Пример 2: Создание системы физики колес с контекстом
        /// </summary>
        public static string CreateWheelPhysicsSystemExample()
        {
            return @"
@context Используй VehicleECSPromptContext.WheelPhysicsContext

Создай систему физики колес с следующими требованиями:
- Используй WheelData, WheelPhysicsData компоненты
- Реализуй raycast для определения контакта с землей
- Добавь систему подвески
- Используй Job System для параллельной обработки

Система должна обрабатывать:
1. Raycast для определения контакта с землей
2. Расчет сил подвески (пружина + демпфер)
3. Расчет трения и сцепления
4. Обновление угловой скорости колеса
";
        }
        
        /// <summary>
        /// Пример 3: Создание сетевой системы с контекстом
        /// </summary>
        public static string CreateNetworkSystemExample()
        {
            return @"
@context Используй NetworkECSPrompt.NetworkSystemContext

Создай систему синхронизации позиций в сети с следующими требованиями:
- Используй NetworkPosition, NetworkId компоненты
- Реализуй синхронизацию на сервере и клиенте
- Добавь интерполяцию для плавного движения
- Используй детерминированные вычисления

Система должна обрабатывать:
1. Синхронизацию позиций между клиентами
2. Интерполяцию позиций для плавности
3. Валидацию позиций для античита
4. Компенсацию задержек сети
";
        }
        
        /// <summary>
        /// Пример 4: Создание террейновой системы с контекстом
        /// </summary>
        public static string CreateTerrainSystemExample()
        {
            return @"
@context Используй TerrainECSPromptContext.TerrainSystemContext

Создай систему деформации террейна с следующими требованиями:
- Используй TerrainData, DeformationData компоненты
- Реализуй деформацию на основе воздействия колес
- Добавь восстановление террейна со временем
- Используй MudManager API для взаимодействия с грязью

Система должна обрабатывать:
1. Деформацию террейна от воздействия колес
2. Изменение уровня грязи
3. Синхронизацию изменений террейна
4. Восстановление террейна со временем
";
        }
        
        #endregion
        
        #region Шаблоны промптов
        
        /// <summary>
        /// Шаблон для создания ECS системы
        /// </summary>
        public static string GetSystemCreationTemplate()
        {
            return @"
@context Используй ECSPromptContext.GetCompleteECSContext()

Создай ECS систему для [ОПИСАНИЕ ЗАДАЧИ] с следующими требованиями:

Компоненты:
- [СПИСОК КОМПОНЕНТОВ]

Функциональность:
- [СПИСОК ФУНКЦИЙ]

Требования:
- Используй ECS архитектуру
- Добавь BurstCompile для производительности
- Используй Job System для параллельной обработки
- Используй FixedStepSimulationSystemGroup для детерминизма
- Используй using static для прямого доступа к компонентам

Система должна:
1. [ТРЕБОВАНИЕ 1]
2. [ТРЕБОВАНИЕ 2]
3. [ТРЕБОВАНИЕ 3]
";
        }
        
        /// <summary>
        /// Шаблон для создания ECS компонента
        /// </summary>
        public static string GetComponentCreationTemplate()
        {
            return @"
@context Используй ECSPromptContext.CoreComponentsContext

Создай ECS компонент [ИМЯ_КОМПОНЕНТА] с следующими требованиями:

Описание:
- [ОПИСАНИЕ КОМПОНЕНТА]

Поля:
- [СПИСОК ПОЛЕЙ]

Требования:
- Реализуй IComponentData
- Добавь XML документацию
- Используй подходящие типы Unity.Mathematics
- Следуй конвенциям именования MudLike

Пример использования:
- [ПРИМЕР ИСПОЛЬЗОВАНИЯ]
";
        }
        
        /// <summary>
        /// Шаблон для создания Job структуры
        /// </summary>
        public static string GetJobCreationTemplate()
        {
            return @"
@context Используй ECSPromptContext.JobSystemContext

Создай Job структуру [ИМЯ_JOB] с следующими требованиями:

Описание:
- [ОПИСАНИЕ JOB]

Интерфейс:
- [IJobEntity/IJobChunk/IJobParallelFor]

Параметры:
- [СПИСОК ПАРАМЕТРОВ]

Требования:
- Добавь BurstCompile атрибут
- Используй подходящие атрибуты [ReadOnly]/[WriteOnly]
- Реализуй Execute метод
- Используй оптимизированные математические операции

Функциональность:
- [ОПИСАНИЕ ФУНКЦИОНАЛЬНОСТИ]
";
        }
        
        #endregion
        
        #region Специализированные промпты
        
        /// <summary>
        /// Промпт для оптимизации существующей системы
        /// </summary>
        public static string GetOptimizationPrompt(string systemName)
        {
            return $@"
@context Используй ECSPromptContext.PerformanceContext

Оптимизируй систему {systemName} с следующими требованиями:

Текущие проблемы:
- [ОПИСАНИЕ ПРОБЛЕМ]

Цели оптимизации:
- Увеличить производительность
- Добавить BurstCompile где возможно
- Использовать Job System для параллельной обработки
- Оптимизировать EntityQuery

Требования:
- Сохранить функциональность
- Улучшить производительность на [ЦЕЛЬ]%
- Использовать chunk-based обработку
- Добавить SIMD операции где возможно

Предложи:
1. Оптимизированную версию системы
2. Job структуры для параллельной обработки
3. Оптимизированные EntityQuery
4. Измерения производительности
";
        }
        
        /// <summary>
        /// Промпт для добавления сетевой функциональности
        /// </summary>
        public static string GetNetworkingPrompt(string systemName)
        {
            return $@"
@context Используй NetworkECSPrompt.NetworkSystemContext

Добавь сетевую функциональность к системе {systemName} с требованиями:

Текущая функциональность:
- [ОПИСАНИЕ ТЕКУЩЕЙ ФУНКЦИОНАЛЬНОСТИ]

Сетевые требования:
- Синхронизация данных между клиентами
- Интерполяция для плавного движения
- Валидация для античита
- Компенсация задержек

Компоненты для синхронизации:
- [СПИСОК КОМПОНЕНТОВ]

Реализуй:
1. Сетевые компоненты для синхронизации
2. Систему синхронизации
3. Интерполяцию данных
4. Валидацию данных
5. Компенсацию задержек
";
        }
        
        /// <summary>
        /// Промпт для добавления террейновой функциональности
        /// </summary>
        public static string GetTerrainPrompt(string systemName)
        {
            return $@"
@context Используй TerrainECSPromptContext.TerrainSystemContext

Добавь террейновую функциональность к системе {systemName} с требованиями:

Текущая функциональность:
- [ОПИСАНИЕ ТЕКУЩЕЙ ФУНКЦИОНАЛЬНОСТИ]

Террейновые требования:
- Взаимодействие с террейном
- Деформация террейна
- Управление грязью
- Синхронизация изменений

Используй:
- MudManager API для взаимодействия с грязью
- TerrainData для данных террейна
- DeformationData для деформации
- WorldGridData для координат

Реализуй:
1. Систему деформации террейна
2. Управление грязью
3. Синхронизацию изменений
4. Мировые координаты
";
        }
        
        #endregion
        
        #region Утилиты для работы с промптами
        
        /// <summary>
        /// Получает полный контекст для ECS модуля
        /// </summary>
        public static string GetFullContextForModule(string moduleName)
        {
            return moduleName.ToLower() switch
            {
                "vehicle" or "vehicles" => ECSPromptContext.GetCompleteECSContext() + "\n" + VehicleECSPromptContext.VehicleSystemContext,
                "network" or "networking" => ECSPromptContext.GetCompleteECSContext() + "\n" + NetworkECSPrompt.NetworkSystemContext,
                "terrain" => ECSPromptContext.GetCompleteECSContext() + "\n" + TerrainECSPromptContext.TerrainSystemContext,
                _ => ECSPromptContext.GetCompleteECSContext()
            };
        }
        
        /// <summary>
        /// Создает персонализированный промпт для задачи
        /// </summary>
        public static string CreatePersonalizedPrompt(string task, string module, string requirements)
        {
            return $@"
@context {GetFullContextForModule(module)}

Задача: {task}

Модуль: {module}

Требования:
{requirements}

Дополнительные указания:
- Используй ECS архитектуру
- Следуй принципам Mud-Like проекта
- Добавь XML документацию
- Используй BurstCompile для производительности
- Используй Job System для параллельной обработки
- Используй FixedStepSimulationSystemGroup для детерминизма
";
        }
        
        #endregion
    }
}
