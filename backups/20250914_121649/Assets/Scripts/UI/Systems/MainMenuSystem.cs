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
            _root = GetComponent<UIDocument>().rootVisualElement;
            
            // Находим элементы UI
            _startGameButton = _root.Q<Button>("StartGameButton");
            _multiplayerButton = _root.Q<Button>("MultiplayerButton");
            _settingsButton = _root.Q<Button>("SettingsButton");
            _exitButton = _root.Q<Button>("ExitButton");
            _loadingPanel = _root.Q<VisualElement>("LoadingPanel");
            _loadingLabel = _root.Q<Label>("LoadingLabel");
            
            // Скрываем панель загрузки
            if (_loadingPanel != null)
                _loadingPanel.style.display = DisplayStyle.None;
        }
        
        /// <summary>
        /// Настраивает обработчики событий
        /// </summary>
        private void SetupEventHandlers()
        {
            if (_startGameButton != null)
                _startGameButton.clicked += OnStartGameClicked;
            
            if (_multiplayerButton != null)
                _multiplayerButton.clicked += OnMultiplayerClicked;
            
            if (_settingsButton != null)
                _settingsButton.clicked += OnSettingsClicked;
            
            if (_exitButton != null)
                _exitButton.clicked += OnExitClicked;
        }
        
        /// <summary>
        /// Удаляет обработчики событий
        /// </summary>
        private void RemoveEventHandlers()
        {
            if (_startGameButton != null)
                _startGameButton.clicked -= OnStartGameClicked;
            
            if (_multiplayerButton != null)
                _multiplayerButton.clicked -= OnMultiplayerClicked;
            
            if (_settingsButton != null)
                _settingsButton.clicked -= OnSettingsClicked;
            
            if (_exitButton != null)
                _exitButton.clicked -= OnExitClicked;
        }
        
        /// <summary>
        /// Обработчик нажатия кнопки "Начать игру"
        /// </summary>
        private void OnStartGameClicked()
        {
            ShowLoading("Загрузка игры...");
            
            // Загружаем сцену игры
            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("GameScene").completed += (operation) =>
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
            UnityEngine.SceneManagement.SceneManager.LoadScene("LobbyScene");
        }
        
        /// <summary>
        /// Обработчик нажатия кнопки "Настройки"
        /// </summary>
        private void OnSettingsClicked()
        {
            // Открываем меню настроек (используем кэшированную ссылку)
            if (_settingsMenuCache != null)
            {
                _settingsMenuCache.ShowSettings();
            }
            else
            {
                // Fallback: ищем заново если кэш не работает
                var settingsMenu = FindObjectOfType<SettingsMenuSystem>();
                if (settingsMenu != null)
                {
                    _settingsMenuCache = settingsMenu;
                    settingsMenu.ShowSettings();
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
                Application.Quit();
            });
        }
        
        /// <summary>
        /// Показывает панель загрузки
        /// </summary>
        private void ShowLoading(string message)
        {
            if (_loadingPanel != null)
            {
                _loadingPanel.style.display = DisplayStyle.Flex;
                if (_loadingLabel != null)
                    _loadingLabel.text = message;
            }
        }
        
        /// <summary>
        /// Скрывает панель загрузки
        /// </summary>
        private void HideLoading()
        {
            if (_loadingPanel != null)
                _loadingPanel.style.display = DisplayStyle.None;
        }
        
        /// <summary>
        /// Показывает диалог подтверждения
        /// </summary>
        private void ShowConfirmDialog(string title, string message, System.Action onConfirm)
        {
            // Создаем диалог подтверждения
            var dialog = new VisualElement();
            dialog.AddToClassList("confirm-dialog");
            
            var titleLabel = new Label(title);
            titleLabel.AddToClassList("dialog-title");
            dialog.Add(titleLabel);
            
            var messageLabel = new Label(message);
            messageLabel.AddToClassList("dialog-message");
            dialog.Add(messageLabel);
            
            var buttonContainer = new VisualElement();
            buttonContainer.AddToClassList("dialog-buttons");
            
            var confirmButton = new Button(onConfirm) { text = "Да" };
            confirmButton.AddToClassList("confirm-button");
            buttonContainer.Add(confirmButton);
            
            var cancelButton = new Button(() => dialog.RemoveFromHierarchy()) { text = "Отмена" };
            cancelButton.AddToClassList("cancel-button");
            buttonContainer.Add(cancelButton);
            
            dialog.Add(buttonContainer);
            _root.Add(dialog);
        }
    }
}
