using Unity.Entities;
using Unity.Mathematics;
using MudLike.UI.Components;

namespace MudLike.UI.Systems
{
    /// <summary>
    /// Система управления меню
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class UIMenuSystem : SystemBase
    {
        private EntityQuery _menuQuery;
        private EntityQuery _inputQuery;
        
        protected override void OnCreate()
        {
            _menuQuery = GetEntityQuery(
                ComponentType.ReadWrite<UIMenuData>()
            );
            
            _inputQuery = GetEntityQuery(
                ComponentType.ReadWrite<UIInputData>()
            );
        }
        
        protected override void OnUpdate()
        {
            // Обрабатываем ввод меню
            ProcessMenuInput();
            
            // Обновляем состояние меню
            UpdateMenuState();
        }
        
        /// <summary>
        /// Обрабатывает ввод меню
        /// </summary>
        private void ProcessMenuInput()
        {
            Entities
                .WithAll<UIInputData>()
                .ForEach((ref UIInputData input) =>
                {
                    if (!input.NeedsProcessing) return;
                    
                    // Обрабатываем команды меню
                    if (input.ToggleHUD)
                    {
                        ToggleHUD();
                        input.ToggleHUD = false;
                    }
                    
                    if (input.ToggleMenu)
                    {
                        ToggleMenu();
                        input.ToggleMenu = false;
                    }
                    
                    if (input.ToggleMap)
                    {
                        ToggleMap();
                        input.ToggleMap = false;
                    }
                    
                    if (input.ToggleChat)
                    {
                        ToggleChat();
                        input.ToggleChat = false;
                    }
                    
                    if (input.ToggleInventory)
                    {
                        ToggleInventory();
                        input.ToggleInventory = false;
                    }
                    
                    if (input.ToggleWinchMenu)
                    {
                        ToggleWinchMenu();
                        input.ToggleWinchMenu = false;
                    }
                    
                    if (input.ToggleCargoMenu)
                    {
                        ToggleCargoMenu();
                        input.ToggleCargoMenu = false;
                    }
                    
                    if (input.ToggleMissionMenu)
                    {
                        ToggleMissionMenu();
                        input.ToggleMissionMenu = false;
                    }
                    
                    if (input.TogglePause)
                    {
                        TogglePause();
                        input.TogglePause = false;
                    }
                    
                    if (input.QuitGame)
                    {
                        QuitGame();
                        input.QuitGame = false;
                    }
                    
                    if (input.RestartGame)
                    {
                        RestartGame();
                        input.RestartGame = false;
                    }
                    
                    if (input.SaveGame)
                    {
                        SaveGame();
                        input.SaveGame = false;
                    }
                    
                    if (input.LoadGame)
                    {
                        LoadGame();
                        input.LoadGame = false;
                    }
                    
                    input.NeedsProcessing = false;
                }).Schedule();
        }
        
        /// <summary>
        /// Обновляет состояние меню
        /// </summary>
        private void UpdateMenuState()
        {
            Entities
                .WithAll<UIMenuData>()
                .ForEach((ref UIMenuData menu) =>
                {
                    // Обновляем состояние меню
                    menu.NeedsUpdate = true;
                }).Schedule();
        }
        
        /// <summary>
        /// Переключает HUD
        /// </summary>
        private void ToggleHUD()
        {
            // Переключаем видимость HUD
            ToggleHUDVisibility();
        }
        
        /// <summary>
        /// Переключает меню
        /// </summary>
        private void ToggleMenu()
        {
            Entities
                .WithAll<UIMenuData>()
                .ForEach((ref UIMenuData menu) =>
                {
                    menu.IsActive = !menu.IsActive;
                    menu.CurrentScreen = menu.IsActive ? MenuScreen.Main : MenuScreen.Main;
                }).Schedule();
        }
        
        /// <summary>
        /// Переключает карту
        /// </summary>
        private void ToggleMap()
        {
            Entities
                .WithAll<UIMenuData>()
                .ForEach((ref UIMenuData menu) =>
                {
                    menu.ShowMap = !menu.ShowMap;
                    menu.CurrentScreen = menu.ShowMap ? MenuScreen.Map : MenuScreen.Main;
                }).Schedule();
        }
        
        /// <summary>
        /// Переключает чат
        /// </summary>
        private void ToggleChat()
        {
            Entities
                .WithAll<UIMenuData>()
                .ForEach((ref UIMenuData menu) =>
                {
                    menu.ShowChat = !menu.ShowChat;
                    menu.CurrentScreen = menu.ShowChat ? MenuScreen.Chat : MenuScreen.Main;
                }).Schedule();
        }
        
        /// <summary>
        /// Переключает инвентарь
        /// </summary>
        private void ToggleInventory()
        {
            Entities
                .WithAll<UIMenuData>()
                .ForEach((ref UIMenuData menu) =>
                {
                    menu.ShowInventory = !menu.ShowInventory;
                    menu.CurrentScreen = menu.ShowInventory ? MenuScreen.Inventory : MenuScreen.Main;
                }).Schedule();
        }
        
        /// <summary>
        /// Переключает меню лебедки
        /// </summary>
        private void ToggleWinchMenu()
        {
            Entities
                .WithAll<UIMenuData>()
                .ForEach((ref UIMenuData menu) =>
                {
                    menu.ShowWinchMenu = !menu.ShowWinchMenu;
                    menu.CurrentScreen = menu.ShowWinchMenu ? MenuScreen.Winch : MenuScreen.Main;
                }).Schedule();
        }
        
        /// <summary>
        /// Переключает меню грузов
        /// </summary>
        private void ToggleCargoMenu()
        {
            Entities
                .WithAll<UIMenuData>()
                .ForEach((ref UIMenuData menu) =>
                {
                    menu.ShowCargoMenu = !menu.ShowCargoMenu;
                    menu.CurrentScreen = menu.ShowCargoMenu ? MenuScreen.Cargo : MenuScreen.Main;
                }).Schedule();
        }
        
        /// <summary>
        /// Переключает меню миссий
        /// </summary>
        private void ToggleMissionMenu()
        {
            Entities
                .WithAll<UIMenuData>()
                .ForEach((ref UIMenuData menu) =>
                {
                    menu.ShowMissionMenu = !menu.ShowMissionMenu;
                    menu.CurrentScreen = menu.ShowMissionMenu ? MenuScreen.Mission : MenuScreen.Main;
                }).Schedule();
        }
        
        /// <summary>
        /// Переключает паузу
        /// </summary>
        private void TogglePause()
        {
            Entities
                .WithAll<UIMenuData>()
                .ForEach((ref UIMenuData menu) =>
                {
                    menu.IsPaused = !menu.IsPaused;
                    menu.CurrentScreen = menu.IsPaused ? MenuScreen.Pause : MenuScreen.Main;
                }).Schedule();
        }
        
        /// <summary>
        /// Выход из игры
        /// </summary>
        private void QuitGame()
        {
            // Выходим из игры
            Application.Quit();
            UnityEngine.Application.Quit();
        }
        
        /// <summary>
        /// Перезапуск игры
        /// </summary>
        private void RestartGame()
        {
            // Перезапускаем текущую сцену
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        
        /// <summary>
        /// Сохранение игры
        /// </summary>
        private void SaveGame()
        {
            // Сохраняем игру
            SaveGame();
        }
        
        /// <summary>
        /// Загрузка игры
        /// </summary>
        private void LoadGame()
        {
            // Загружаем игру
            LoadGame();
        }
    }
}