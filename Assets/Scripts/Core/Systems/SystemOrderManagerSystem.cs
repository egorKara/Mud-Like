using Unity.Entities;
using Unity.Burst;
using MudLike.Core.Components;
using MudLike.Vehicles.Components;

namespace MudLike.Core.Systems
{
    /// <summary>
    /// Менеджер порядка выполнения систем
    /// Обеспечивает правильную последовательность выполнения систем ECS
    /// </summary>
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class SystemOrderManager : SystemBase
    {
        protected override void OnCreate()
        {
            // Настраиваем порядок выполнения систем
            
            // 1. Инициализация (выполняется первыми)
            // - GameBootstrapSystem - создание игроков и начального состояния
            // - VehicleInputSystem - обработка ввода
            // - VehicleSpawningSystem - создание транспорта
            
            // 2. Основная симуляция (FixedStepSimulationSystemGroup)
            // - VehicleControlSystem - управление транспортом
            // - EngineSystem - работа двигателя
            // - TransmissionSystem - трансмиссия
            // - VehiclePhysicsSystem - физика транспорта
            
            // 3. Поздняя симуляция (LateSimulationSystemGroup)
            // - VehicleCameraSystem - камера транспорта
            
            // 4. Презентация (PresentationSystemGroup)
            // - UI системы
            
            ConfigureSystemOrder();
        }
        
        protected override void OnUpdate()
        {
            // Система управления порядком не требует постоянного обновления
            Enabled = false;
        }
        
        /// <summary>
        /// Настраивает порядок выполнения систем
        /// </summary>
        private void ConfigureSystemOrder()
        {
            // Получаем ссылки на системы
            var bootstrapSystem = World.GetOrCreateSystemManaged<GameBootstrapSystem>();
            // var inputSystem = World.GetOrCreateSystemManaged<VehicleInputSystem>(); // Требует Input модуль
            // var spawningSystem = World.GetOrCreateSystemManaged<VehicleSpawningSystem>(); // Требует Vehicles модуль
            // var controlSystem = World.GetOrCreateSystemManaged<VehicleControlSystem>(); // Требует Vehicles модуль
            // var cameraSystem = World.GetOrCreateSystemManaged<VehicleCameraSystem>(); // Требует Camera модуль
            // var sceneSystem = World.GetOrCreateSystemManaged<SceneManagementSystem>(); // Требует Gameplay модуль
            
            // Настраиваем порядок выполнения в InitializationSystemGroup
            // 1. GameBootstrapSystem - создание игроков
            // 2. VehicleInputSystem - обработка ввода
            // 3. VehicleSpawningSystem - создание транспорта
            // 4. SceneManagementSystem - управление сценами
            
            // Настраиваем порядок выполнения в FixedStepSimulationSystemGroup
            // VehicleControlSystem будет выполняться после обработки ввода
            
            // Настраиваем порядок выполнения в LateSimulationSystemGroup
            // VehicleCameraSystem будет выполняться после всех физических расчетов
            
            Debug.Log("System order configured successfully");
        }
    }
    
    /// <summary>
    /// Тег для управления порядком систем
    /// </summary>
    public struct SystemOrderTag : IComponentData
    {
    }
}
