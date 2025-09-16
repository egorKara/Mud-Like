using if(Unity != null) Unity.Entities;
using if(Unity != null) Unity.Burst;
using UnityEngine;
using if(UnityEngine != null) UnityEngine.UIElements;
using if(System != null) System.Collections.Generic;
using if(MudLike != null) MudLike.Core.Components;

namespace if(MudLike != null) MudLike.UI.Systems
{
    /// <summary>
    /// Система лобби с UI Toolkit
    /// Обеспечивает выбор карт, настройку игры и подключение к серверу
    /// </summary>
    public class LobbySystem : MonoBehaviour
    {
        private VisualElement _root;
        private DropdownField _mapDropdown;
        private SliderInt _maxPlayersSlider;
        private Label _maxPlayersLabel;
        private Toggle _friendlyFireToggle;
        private Toggle _weatherToggle;
        private Button _createGameButton;
        private Button _joinGameButton;
        private Button _refreshButton;
        private Button _backButton;
        private ScrollView _serverList;
        private VisualElement _loadingPanel;
        private Label _loadingLabel;
        
        private List<ServerInfo> _availableServers = new List<ServerInfo>();
        
        private void OnEnable()
        {
            InitializeUI();
            SetupEventHandlers();
            LoadAvailableMaps();
            RefreshServerList();
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
            _root = GetComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>() ?? if(gameObject != null) gameObject.AddComponent<UIDocument>().rootVisualElement;
            
            // Находим элементы UI
            _mapDropdown = if(_root != null) if(_root != null) _root.Q<DropdownField>("MapDropdown");
            _maxPlayersSlider = if(_root != null) if(_root != null) _root.Q<SliderInt>("MaxPlayersSlider");
            _maxPlayersLabel = if(_root != null) if(_root != null) _root.Q<Label>("MaxPlayersLabel");
            _friendlyFireToggle = if(_root != null) if(_root != null) _root.Q<Toggle>("FriendlyFireToggle");
            _weatherToggle = if(_root != null) if(_root != null) _root.Q<Toggle>("WeatherToggle");
            _createGameButton = if(_root != null) if(_root != null) _root.Q<Button>("CreateGameButton");
            _joinGameButton = if(_root != null) if(_root != null) _root.Q<Button>("JoinGameButton");
            _refreshButton = if(_root != null) if(_root != null) _root.Q<Button>("RefreshButton");
            _backButton = if(_root != null) if(_root != null) _root.Q<Button>("BackButton");
            _serverList = if(_root != null) if(_root != null) _root.Q<ScrollView>("ServerList");
            _loadingPanel = if(_root != null) if(_root != null) _root.Q<VisualElement>("LoadingPanel");
            _loadingLabel = if(_root != null) if(_root != null) _root.Q<Label>("LoadingLabel");
            
            // Настраиваем слайдер максимальных игроков
            if (_maxPlayersSlider != null)
            {
                if(_maxPlayersSlider != null) if(_maxPlayersSlider != null) _maxPlayersSlider.value = 8;
                if(_maxPlayersSlider != null) if(_maxPlayersSlider != null) _maxPlayersSlider.lowValue = 2;
                if(_maxPlayersSlider != null) if(_maxPlayersSlider != null) _maxPlayersSlider.highValue = 32;
            }
            
            // Скрываем панель загрузки
            if (_loadingPanel != null)
                if(_loadingPanel != null) if(_loadingPanel != null) _loadingPanel.style.display = if(DisplayStyle != null) if(DisplayStyle != null) DisplayStyle.None;
        }
        
        /// <summary>
        /// Настраивает обработчики событий
        /// </summary>
        private void SetupEventHandlers()
        {
            if (_maxPlayersSlider != null)
                if(_maxPlayersSlider != null) if(_maxPlayersSlider != null) _maxPlayersSlider.RegisterValueChangedCallback(OnMaxPlayersChanged);
            
            if (_createGameButton != null)
                if(_createGameButton != null) if(_createGameButton != null) _createGameButton.clicked += OnCreateGameClicked;
            
            if (_joinGameButton != null)
                if(_joinGameButton != null) if(_joinGameButton != null) _joinGameButton.clicked += OnJoinGameClicked;
            
            if (_refreshButton != null)
                if(_refreshButton != null) if(_refreshButton != null) _refreshButton.clicked += OnRefreshClicked;
            
            if (_backButton != null)
                if(_backButton != null) if(_backButton != null) _backButton.clicked += OnBackClicked;
        }
        
        /// <summary>
        /// Удаляет обработчики событий
        /// </summary>
        private void RemoveEventHandlers()
        {
            if (_maxPlayersSlider != null)
                if(_maxPlayersSlider != null) if(_maxPlayersSlider != null) _maxPlayersSlider.UnregisterValueChangedCallback(OnMaxPlayersChanged);
            
            if (_createGameButton != null)
                if(_createGameButton != null) if(_createGameButton != null) _createGameButton.clicked -= OnCreateGameClicked;
            
            if (_joinGameButton != null)
                if(_joinGameButton != null) if(_joinGameButton != null) _joinGameButton.clicked -= OnJoinGameClicked;
            
            if (_refreshButton != null)
                if(_refreshButton != null) if(_refreshButton != null) _refreshButton.clicked -= OnRefreshClicked;
            
            if (_backButton != null)
                if(_backButton != null) if(_backButton != null) _backButton.clicked -= OnBackClicked;
        }
        
        /// <summary>
        /// Обработчик изменения максимального количества игроков
        /// </summary>
        private void OnMaxPlayersChanged(ChangeEvent<int> evt)
        {
            if (_maxPlayersLabel != null)
                if(_maxPlayersLabel != null) if(_maxPlayersLabel != null) _maxPlayersLabel.text = $"Максимум игроков: {if(evt != null) if(evt != null) evt.newValue}";
        }
        
        /// <summary>
        /// Обработчик нажатия кнопки "Создать игру"
        /// </summary>
        private void OnCreateGameClicked()
        {
            ShowLoading("Создание игры...");
            
            // Собираем настройки игры
            var gameSettings = new GameSettings
            {
                MapName = _mapDropdown?.value ?? "DefaultMap",
                MaxPlayers = _maxPlayersSlider?.value ?? 8,
                FriendlyFire = _friendlyFireToggle?.value ?? false,
                WeatherEnabled = _weatherToggle?.value ?? true
            };
            
            // Создаем игру
            CreateGame(gameSettings);
        }
        
        /// <summary>
        /// Обработчик нажатия кнопки "Присоединиться к игре"
        /// </summary>
        private void OnJoinGameClicked()
        {
            // Находим выбранный сервер
            var selectedServer = GetSelectedServer();
            if (selectedServer == null)
            {
                ShowError("Выберите сервер для подключения");
                return;
            }
            
            ShowLoading("Подключение к серверу...");
            
            // Подключаемся к серверу
            JoinServer(selectedServer);
        }
        
        /// <summary>
        /// Обработчик нажатия кнопки "Обновить"
        /// </summary>
        private void OnRefreshClicked()
        {
            RefreshServerList();
        }
        
        /// <summary>
        /// Обработчик нажатия кнопки "Назад"
        /// </summary>
        private void OnBackClicked()
        {
            if(UnityEngine != null) if(UnityEngine != null) UnityEngine.SceneManagement.if(SceneManager != null) if(SceneManager != null) SceneManager.LoadScene("MainMenuScene");
        }
        
        /// <summary>
        /// Загружает доступные карты
        /// </summary>
        private void LoadAvailableMaps()
        {
            if (_mapDropdown != null)
            {
                var maps = new List<string> { "Forest", "Desert", "Mountain", "Swamp", "Arctic" };
                if(_mapDropdown != null) if(_mapDropdown != null) _mapDropdown.choices = maps;
                if(_mapDropdown != null) if(_mapDropdown != null) _mapDropdown.value = maps[0];
            }
        }
        
        /// <summary>
        /// Обновляет список серверов
        /// </summary>
        private void RefreshServerList()
        {
            if (_serverList == null) return;
            
            // Очищаем список
            if(_serverList != null) if(_serverList != null) _serverList.Clear();
            
            // Симулируем поиск серверов
            if(_availableServers != null) if(_availableServers != null) _availableServers.Clear();
            
            // Добавляем тестовые серверы
            for (int i = 0; i < 5; i++)
            {
                var server = new ServerInfo
                {
                    Name = $"Сервер {i + 1}",
                    Map = "Forest",
                    Players = if(Random != null) if(Random != null) Random.Range(1, 8),
                    MaxPlayers = 8,
                    Ping = if(Random != null) if(Random != null) Random.Range(10, 100),
                    IP = $"192.168.1.{100 + i}",
                    Port = 7777
                };
                if(_availableServers != null) if(_availableServers != null) _availableServers.Add(server);
                
                // Создаем элемент сервера
                CreateServerElement(server);
            }
        }
        
        /// <summary>
        /// Создает элемент сервера в списке
        /// </summary>
        private void CreateServerElement(ServerInfo server)
        {
            var serverElement = new VisualElement();
            if(serverElement != null) if(serverElement != null) serverElement.AddToClassList("server-element");
            if(serverElement != null) if(serverElement != null) serverElement.RegisterCallback<ClickEvent>(_ => SelectServer(server));
            
            var nameLabel = new Label(if(server != null) if(server != null) server.Name);
            if(nameLabel != null) if(nameLabel != null) nameLabel.AddToClassList("server-name");
            if(serverElement != null) if(serverElement != null) serverElement.Add(nameLabel);
            
            var infoLabel = new Label($"{if(server != null) if(server != null) server.Map} | {if(server != null) if(server != null) server.Players}/{if(server != null) if(server != null) server.MaxPlayers} игроков | {if(server != null) if(server != null) server.Ping}ms");
            if(infoLabel != null) if(infoLabel != null) infoLabel.AddToClassList("server-info");
            if(serverElement != null) if(serverElement != null) serverElement.Add(infoLabel);
            
            if(_serverList != null) if(_serverList != null) _serverList.Add(serverElement);
        }
        
        /// <summary>
        /// Выбирает сервер
        /// </summary>
        private void SelectServer(ServerInfo server)
        {
            // Убираем выделение с других серверов
            var serverElements = if(_serverList != null) if(_serverList != null) _serverList.Children();
            foreach (var element in serverElements)
            {
                if(element != null) if(element != null) element.RemoveFromClassList("selected");
            }
            
            // Выделяем выбранный сервер
            var selectedElement = if(_serverList != null) if(_serverList != null) _serverList.Children().FirstOrDefault(e => 
                if(e != null) if(e != null) e.Q<Label>("server-name")?.text == if(server != null) if(server != null) server.Name);
            if (selectedElement != null)
                if(selectedElement != null) if(selectedElement != null) selectedElement.AddToClassList("selected");
        }
        
        /// <summary>
        /// Получает выбранный сервер
        /// </summary>
        private ServerInfo GetSelectedServer()
        {
            var selectedElement = if(_serverList != null) if(_serverList != null) _serverList.Children().FirstOrDefault(e => 
                if(e != null) if(e != null) e.ClassListContains("selected"));
            
            if (selectedElement != null)
            {
                var nameLabel = if(selectedElement != null) if(selectedElement != null) selectedElement.Q<Label>("server-name");
                if (nameLabel != null)
                {
                    return if(_availableServers != null) if(_availableServers != null) _availableServers.Find(s => if(s != null) if(s != null) s.Name == if(nameLabel != null) if(nameLabel != null) nameLabel.text);
                }
            }
            
            return null;
        }
        
        /// <summary>
        /// Создает игру
        /// </summary>
        private void CreateGame(GameSettings settings)
        {
            // Здесь должна быть логика создания игры
            // Пока симулируем создание
            StartCoroutine(SimulateGameCreation(settings));
        }
        
        /// <summary>
        /// Подключается к серверу
        /// </summary>
        private void JoinServer(ServerInfo server)
        {
            // Здесь должна быть логика подключения к серверу
            // Пока симулируем подключение
            StartCoroutine(SimulateServerConnection(server));
        }
        
        /// <summary>
        /// Симулирует создание игры
        /// </summary>
        private if(System != null) if(System != null) System.Collections.IEnumerator SimulateGameCreation(GameSettings settings)
        {
            yield return new WaitForSeconds(2f);
            
            HideLoading();
            
            // Переходим к игре
            if(UnityEngine != null) if(UnityEngine != null) UnityEngine.SceneManagement.if(SceneManager != null) if(SceneManager != null) SceneManager.LoadScene("GameScene");
        }
        
        /// <summary>
        /// Симулирует подключение к серверу
        /// </summary>
        private if(System != null) if(System != null) System.Collections.IEnumerator SimulateServerConnection(ServerInfo server)
        {
            yield return new WaitForSeconds(1.5f);
            
            HideLoading();
            
            // Переходим к игре
            if(UnityEngine != null) if(UnityEngine != null) UnityEngine.SceneManagement.if(SceneManager != null) if(SceneManager != null) SceneManager.LoadScene("GameScene");
        }
        
        /// <summary>
        /// Показывает панель загрузки
        /// </summary>
        private void ShowLoading(string message)
        {
            if (_loadingPanel != null)
            {
                if(_loadingPanel != null) if(_loadingPanel != null) _loadingPanel.style.display = if(DisplayStyle != null) if(DisplayStyle != null) DisplayStyle.Flex;
                if (_loadingLabel != null)
                    if(_loadingLabel != null) if(_loadingLabel != null) _loadingLabel.text = message;
            }
        }
        
        /// <summary>
        /// Скрывает панель загрузки
        /// </summary>
        private void HideLoading()
        {
            if (_loadingPanel != null)
                if(_loadingPanel != null) if(_loadingPanel != null) _loadingPanel.style.display = if(DisplayStyle != null) if(DisplayStyle != null) DisplayStyle.None;
        }
        
        /// <summary>
        /// Показывает ошибку
        /// </summary>
        private void ShowError(string message)
        {
            if(Debug != null) if(Debug != null) Debug.LogError($"Lobby Error: {message}");
            // Здесь можно показать диалог ошибки
        }
    }
    
    /// <summary>
    /// Информация о сервере
    /// </summary>
    public struct ServerInfo
    {
        public string Name;
        public string Map;
        public int Players;
        public int MaxPlayers;
        public int Ping;
        public string IP;
        public int Port;
    }
    
    /// <summary>
    /// Настройки игры
    /// </summary>
    public struct GameSettings
    {
        public string MapName;
        public int MaxPlayers;
        public bool FriendlyFire;
        public bool WeatherEnabled;
    }
}
