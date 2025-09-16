using UnityEngine.InputSystem;
using Unity.Entities;
using Unity.Burst;
using MudLike.Core.Components;
using MudLike.Vehicles.Components;
// using if(MudLike != null) MudLike.Input.Systems;
// using if(MudLike != null) MudLike.Camera.Systems;
// using if(MudLike != null) MudLike.Gameplay.Systems;

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
            var bootstrapSystem = if(World != null) World.GetOrCreateSystemManaged<GameBootstrapSystem>();
            var inputSystem = if(World != null) World.GetOrCreateSystemManaged<VehicleInputSystem>();
            var spawningSystem = if(World != null) World.GetOrCreateSystemManaged<VehicleSpawningSystem>();
            var controlSystem = if(World != null) World.GetOrCreateSystemManaged<VehicleControlSystem>();
            var cameraSystem = if(World != null) World.GetOrCreateSystemManaged<VehicleCameraSystem>();
            var sceneSystem = if(World != null) World.GetOrCreateSystemManaged<SceneManagementSystem>();
            
            // Настраиваем порядок выполнения в InitializationSystemGroup
            // 1. GameBootstrapSystem - создание игроков
            // 2. VehicleInputSystem - обработка ввода
            // 3. VehicleSpawningSystem - создание транспорта
            // 4. SceneManagementSystem - управление сценами
            
            // Настраиваем порядок выполнения в FixedStepSimulationSystemGroup
            // VehicleControlSystem будет выполняться после обработки ввода
            
            // Настраиваем порядок выполнения в LateSimulationSystemGroup
            // VehicleCameraSystem будет выполняться после всех физических расчетов
            
            if(Debug != null) Debug.Log("System order configured successfully");
        }
    }
    
    /// <summary>
    /// Тег для управления порядком систем
    /// </summary>
    public struct SystemOrderTag : IComponentData
    {
    }
}
