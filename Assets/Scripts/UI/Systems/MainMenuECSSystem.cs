using Unity.Entities;
using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using MudLike.Core.Components;

namespace MudLike.UI.Systems
{
    /// <summary>
    /// ECS система главного меню
    /// Заменяет MonoBehaviour MainMenuSystem
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class MainMenuSystem : SystemBase
    {
        private EntityQuery _buttonQuery;
        private EntityQuery _panelQuery;
        private EntityQuery _dialogQuery;
        private EntityQuery _loadingQuery;
        
        protected override void OnCreate()
        {
            // Создаем запросы для UI компонентов
            _buttonQuery = GetEntityQuery(typeof(UIButton), typeof(UIElement));
            _panelQuery = GetEntityQuery(typeof(UIPanel), typeof(UIElement));
            _dialogQuery = GetEntityQuery(typeof(UIDialog), typeof(UIElement));
            _loadingQuery = GetEntityQuery(typeof(UILoading), typeof(UIElement));
        }
        
        protected override void OnUpdate()
        {
            // Обрабатываем нажатия кнопок
            ProcessButtonClicks();
            
            // Обрабатываем панели
            ProcessPanels();
            
            // Обрабатываем диалоги
            ProcessDialogs();
            
            // Обрабатываем загрузку
            ProcessLoading();
        }
        
        /// <summary>
        /// Обрабатывает нажатия кнопок
        /// </summary>
        private void ProcessButtonClicks()
        {
            Entities
                .WithAll<UIButton, UIElement>()
                .ForEach((Entity entity, ref UIButton button, ref UIElement element) =>
                {
                    if (button.IsClicked)
                    {
                        HandleButtonClick(button.ButtonId);
                        button.IsClicked = false;
                    }
                }).WithoutBurst().Run();
        }
        
        /// <summary>
        /// Обрабатывает панели
        /// </summary>
        private void ProcessPanels()
        {
            Entities
                .WithAll<UIPanel, UIElement>()
                .ForEach((ref UIPanel panel, ref UIElement element) =>
                {
                    if (element.IsVisible != panel.IsVisible)
                    {
                        element.IsVisible = panel.IsVisible;
                        UpdatePanelVisibility(panel.PanelId, panel.IsVisible);
                    }
                }).WithoutBurst().Run();
        }
        
        /// <summary>
        /// Обрабатывает диалоги
        /// </summary>
        private void ProcessDialogs()
        {
            Entities
                .WithAll<UIDialog, UIElement>()
                .ForEach((ref UIDialog dialog, ref UIElement element) =>
                {
                    if (dialog.IsOpen)
                    {
                        // Обрабатываем таймаут диалога
                        if (dialog.Timeout > 0)
                        {
                            dialog.Timeout -= SystemAPI.Time.DeltaTime;
                            if (dialog.Timeout <= 0)
                            {
                                dialog.IsOpen = false;
                                element.IsVisible = false;
                            }
                        }
                    }
                }).WithoutBurst().Run();
        }
        
        /// <summary>
        /// Обрабатывает загрузку
        /// </summary>
        private void ProcessLoading()
        {
            Entities
                .WithAll<UILoading, UIElement>()
                .ForEach((ref UILoading loading, ref UIElement element) =>
                {
                    if (loading.IsLoading)
                    {
                        // Обновляем прогресс загрузки
                        loading.Progress += SystemAPI.Time.DeltaTime / loading.Duration;
                        if (loading.Progress >= 1f)
                        {
                            loading.IsLoading = false;
                            element.IsVisible = false;
                        }
                    }
                }).WithoutBurst().Run();
        }
        
        /// <summary>
        /// Обрабатывает нажатие кнопки
        /// </summary>
        private void HandleButtonClick(int buttonId)
        {
            switch (buttonId)
            {
                case 1: // Start Game
                    StartGame();
                    break;
                case 2: // Multiplayer
                    StartMultiplayer();
                    break;
                case 3: // Settings
                    OpenSettings();
                    break;
                case 4: // Exit
                    ExitGame();
                    break;
            }
        }
        
        /// <summary>
        /// Начинает игру
        /// </summary>
        private void StartGame()
        {
            // Создаем сущность загрузки
            var loadingEntity = EntityManager.CreateEntity();
            EntityManager.AddComponentData(loadingEntity, new UILoading
            {
                IsLoading = true,
                Progress = 0f,
                Duration = 2f,
                LoadingId = 1
            });
            EntityManager.AddComponentData(loadingEntity, new UIElement
            {
                ElementId = 1,
                IsVisible = true,
                IsInteractive = false
            });
            
            // Загружаем сцену игры
            SceneManager.LoadSceneAsync("GameScene").completed += (operation) =>
            {
                // Удаляем сущность загрузки
                EntityManager.DestroyEntity(loadingEntity);
            };
        }
        
        /// <summary>
        /// Начинает мультиплеер
        /// </summary>
        private void StartMultiplayer()
        {
            SceneManager.LoadScene("LobbyScene");
        }
        
        /// <summary>
        /// Открывает настройки
        /// </summary>
        private void OpenSettings()
        {
            // Создаем сущность диалога настроек
            var settingsEntity = EntityManager.CreateEntity();
            EntityManager.AddComponentData(settingsEntity, new UIDialog
            {
                DialogId = 1,
                IsOpen = true,
                IsModal = true,
                Timeout = 0f
            });
            EntityManager.AddComponentData(settingsEntity, new UIElement
            {
                ElementId = 2,
                IsVisible = true,
                IsInteractive = true
            });
        }
        
        /// <summary>
        /// Выходит из игры
        /// </summary>
        private void ExitGame()
        {
            // Создаем сущность диалога подтверждения
            var confirmEntity = EntityManager.CreateEntity();
            EntityManager.AddComponentData(confirmEntity, new UIDialog
            {
                DialogId = 2,
                IsOpen = true,
                IsModal = true,
                Timeout = 0f
            });
            EntityManager.AddComponentData(confirmEntity, new UIElement
            {
                ElementId = 3,
                IsVisible = true,
                IsInteractive = true
            });
            
            // Подтверждаем выход
            Application.Quit();
        }
        
        /// <summary>
        /// Обновляет видимость панели
        /// </summary>
        private void UpdatePanelVisibility(int panelId, bool isVisible)
        {
            // Здесь можно добавить логику обновления видимости панели
            Debug.Log($"Panel {panelId} visibility: {isVisible}");
        }
    }
}
