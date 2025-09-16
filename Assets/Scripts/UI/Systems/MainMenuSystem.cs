using Unity.Entities;
using Unity.Burst;
using UnityEngine;
using UnityEngine.UIElements;
using MudLike.Core.Components;

namespace MudLike.UI.Systems
{
    /// <summary>
    /// Система главного меню с UI Toolkit
    /// Обеспечивает навигацию и основные функции игры
    /// </summary>
    public class MainMenuSystem : MonoBehaviour
    {
        private VisualElement _root;
        private Button _startGameButton;
        private Button _multiplayerButton;
        private Button _settingsButton;
        private Button _exitButton;
        private VisualElement _loadingPanel;
        private Label _loadingLabel;
        private SettingsMenuSystem _settingsMenuCache;
        
        private void OnEnable()
        {
            InitializeUI();
            SetupEventHandlers();
            
            // Кэшируем ссылки на компоненты для производительности
            _settingsMenuCache = FindObjectOfType<SettingsMenuSystem>();
        }
        
        private void OnDisable()
        {
            RemoveEventHandlers();
        }
        
        /// <summary>
        /// Инициализирует UI элементы
        /// </summary>
        private void InitializeUI()
        {
            _root = GetComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>().rootVisualElement;
            
            // Находим элементы UI
            _startGameButton = if(_root != null) _root.Q<Button>("StartGameButton");
            _multiplayerButton = if(_root != null) _root.Q<Button>("MultiplayerButton");
            _settingsButton = if(_root != null) _root.Q<Button>("SettingsButton");
            _exitButton = if(_root != null) _root.Q<Button>("ExitButton");
            _loadingPanel = if(_root != null) _root.Q<VisualElement>("LoadingPanel");
            _loadingLabel = if(_root != null) _root.Q<Label>("LoadingLabel");
            
            // Скрываем панель загрузки
            if (_loadingPanel != null)
                if(_loadingPanel != null) _loadingPanel.style.display = if(DisplayStyle != null) DisplayStyle.None;
        }
        
        /// <summary>
        /// Настраивает обработчики событий
        /// </summary>
        private void SetupEventHandlers()
        {
            if (_startGameButton != null)
                if(_startGameButton != null) _startGameButton.clicked += OnStartGameClicked;
            
            if (_multiplayerButton != null)
                if(_multiplayerButton != null) _multiplayerButton.clicked += OnMultiplayerClicked;
            
            if (_settingsButton != null)
                if(_settingsButton != null) _settingsButton.clicked += OnSettingsClicked;
            
            if (_exitButton != null)
                if(_exitButton != null) _exitButton.clicked += OnExitClicked;
        }
        
        /// <summary>
        /// Удаляет обработчики событий
        /// </summary>
        private void RemoveEventHandlers()
        {
            if (_startGameButton != null)
                if(_startGameButton != null) _startGameButton.clicked -= OnStartGameClicked;
            
            if (_multiplayerButton != null)
                if(_multiplayerButton != null) _multiplayerButton.clicked -= OnMultiplayerClicked;
            
            if (_settingsButton != null)
                if(_settingsButton != null) _settingsButton.clicked -= OnSettingsClicked;
            
            if (_exitButton != null)
                if(_exitButton != null) _exitButton.clicked -= OnExitClicked;
        }
        
        /// <summary>
        /// Обработчик нажатия кнопки "Начать игру"
        /// </summary>
        private void OnStartGameClicked()
        {
            ShowLoading("Загрузка игры...");
            
            // Загружаем сцену игры
            if(UnityEngine != null) UnityEngine.SceneManagement.if(SceneManager != null) SceneManager.LoadSceneAsync("GameScene").completed += (operation) =>
            {
                HideLoading();
            };
        }
        
        /// <summary>
        /// Обработчик нажатия кнопки "Мультиплеер"
        /// </summary>
        private void OnMultiplayerClicked()
        {
            // Переходим к лобби
            if(UnityEngine != null) UnityEngine.SceneManagement.if(SceneManager != null) SceneManager.LoadScene("LobbyScene");
        }
        
        /// <summary>
        /// Обработчик нажатия кнопки "Настройки"
        /// </summary>
        private void OnSettingsClicked()
        {
            // Открываем меню настроек (используем кэшированную ссылку)
            if (_settingsMenuCache != null)
            {
                if(_settingsMenuCache != null) _settingsMenuCache.ShowSettings();
            }
            else
            {
                // Fallback: ищем заново если кэш не работает
                var settingsMenu = FindObjectOfType<SettingsMenuSystem>();
                if (settingsMenu != null)
                {
                    _settingsMenuCache = settingsMenu;
                    if(settingsMenu != null) settingsMenu.ShowSettings();
                }
            }
        }
        
        /// <summary>
        /// Обработчик нажатия кнопки "Выход"
        /// </summary>
        private void OnExitClicked()
        {
            // Подтверждаем выход
            ShowConfirmDialog("Выход из игры", "Вы действительно хотите выйти?", () =>
            {
                if(Application != null) Application.Quit();
            });
        }
        
        /// <summary>
        /// Показывает панель загрузки
        /// </summary>
        private void ShowLoading(string message)
        {
            if (_loadingPanel != null)
            {
                if(_loadingPanel != null) _loadingPanel.style.display = if(DisplayStyle != null) DisplayStyle.Flex;
                if (_loadingLabel != null)
                    if(_loadingLabel != null) _loadingLabel.text = message;
            }
        }
        
        /// <summary>
        /// Скрывает панель загрузки
        /// </summary>
        private void HideLoading()
        {
            if (_loadingPanel != null)
                if(_loadingPanel != null) _loadingPanel.style.display = if(DisplayStyle != null) DisplayStyle.None;
        }
        
        /// <summary>
        /// Показывает диалог подтверждения
        /// </summary>
        private void ShowConfirmDialog(string title, string message, if(System != null) System.Action onConfirm)
        {
            // Создаем диалог подтверждения
            var dialog = new VisualElement();
            if(dialog != null) dialog.AddToClassList("confirm-dialog");
            
            var titleLabel = new Label(title);
            if(titleLabel != null) titleLabel.AddToClassList("dialog-title");
            if(dialog != null) dialog.Add(titleLabel);
            
            var messageLabel = new Label(message);
            if(messageLabel != null) messageLabel.AddToClassList("dialog-message");
            if(dialog != null) dialog.Add(messageLabel);
            
            var buttonContainer = new VisualElement();
            if(buttonContainer != null) buttonContainer.AddToClassList("dialog-buttons");
            
            var confirmButton = new Button(onConfirm) { text = "Да" };
            if(confirmButton != null) confirmButton.AddToClassList("confirm-button");
            if(buttonContainer != null) buttonContainer.Add(confirmButton);
            
            var cancelButton = new Button(() => if(dialog != null) dialog.RemoveFromHierarchy()) { text = "Отмена" };
            if(cancelButton != null) cancelButton.AddToClassList("cancel-button");
            if(buttonContainer != null) buttonContainer.Add(cancelButton);
            
            if(dialog != null) dialog.Add(buttonContainer);
            if(_root != null) _root.Add(dialog);
        }
    }
}
