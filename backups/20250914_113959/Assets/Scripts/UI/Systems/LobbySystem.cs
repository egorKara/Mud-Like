using Unity.Entities;
using Unity.Burst;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using MudLike.Core.Components;

namespace MudLike.UI.Systems
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
            _root = GetComponent<UIDocument>().rootVisualElement;
            
            // Находим элементы UI
            _mapDropdown = _root.Q<DropdownField>("MapDropdown");
            _maxPlayersSlider = _root.Q<SliderInt>("MaxPlayersSlider");
            _maxPlayersLabel = _root.Q<Label>("MaxPlayersLabel");
            _friendlyFireToggle = _root.Q<Toggle>("FriendlyFireToggle");
            _weatherToggle = _root.Q<Toggle>("WeatherToggle");
            _createGameButton = _root.Q<Button>("CreateGameButton");
            _joinGameButton = _root.Q<Button>("JoinGameButton");
            _refreshButton = _root.Q<Button>("RefreshButton");
            _backButton = _root.Q<Button>("BackButton");
            _serverList = _root.Q<ScrollView>("ServerList");
            _loadingPanel = _root.Q<VisualElement>("LoadingPanel");
            _loadingLabel = _root.Q<Label>("LoadingLabel");
            
            // Настраиваем слайдер максимальных игроков
            if (_maxPlayersSlider != null)
            {
                _maxPlayersSlider.value = 8;
                _maxPlayersSlider.lowValue = 2;
                _maxPlayersSlider.highValue = 32;
            }
            
            // Скрываем панель загрузки
            if (_loadingPanel != null)
                _loadingPanel.style.display = DisplayStyle.None;
        }
        
        /// <summary>
        /// Настраивает обработчики событий
        /// </summary>
        private void SetupEventHandlers()
        {
            if (_maxPlayersSlider != null)
                _maxPlayersSlider.RegisterValueChangedCallback(OnMaxPlayersChanged);
            
            if (_createGameButton != null)
                _createGameButton.clicked += OnCreateGameClicked;
            
            if (_joinGameButton != null)
                _joinGameButton.clicked += OnJoinGameClicked;
            
            if (_refreshButton != null)
                _refreshButton.clicked += OnRefreshClicked;
            
            if (_backButton != null)
                _backButton.clicked += OnBackClicked;
        }
        
        /// <summary>
        /// Удаляет обработчики событий
        /// </summary>
        private void RemoveEventHandlers()
        {
            if (_maxPlayersSlider != null)
                _maxPlayersSlider.UnregisterValueChangedCallback(OnMaxPlayersChanged);
            
            if (_createGameButton != null)
                _createGameButton.clicked -= OnCreateGameClicked;
            
            if (_joinGameButton != null)
                _joinGameButton.clicked -= OnJoinGameClicked;
            
            if (_refreshButton != null)
                _refreshButton.clicked -= OnRefreshClicked;
            
            if (_backButton != null)
                _backButton.clicked -= OnBackClicked;
        }
        
        /// <summary>
        /// Обработчик изменения максимального количества игроков
        /// </summary>
        private void OnMaxPlayersChanged(ChangeEvent<int> evt)
        {
            if (_maxPlayersLabel != null)
                _maxPlayersLabel.text = $"Максимум игроков: {evt.newValue}";
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
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenuScene");
        }
        
        /// <summary>
        /// Загружает доступные карты
        /// </summary>
        private void LoadAvailableMaps()
        {
            if (_mapDropdown != null)
            {
                var maps = new List<string> { "Forest", "Desert", "Mountain", "Swamp", "Arctic" };
                _mapDropdown.choices = maps;
                _mapDropdown.value = maps[0];
            }
        }
        
        /// <summary>
        /// Обновляет список серверов
        /// </summary>
        private void RefreshServerList()
        {
            if (_serverList == null) return;
            
            // Очищаем список
            _serverList.Clear();
            
            // Симулируем поиск серверов
            _availableServers.Clear();
            
            // Добавляем тестовые серверы
            for (int i = 0; i < 5; i++)
            {
                var server = new ServerInfo
                {
                    Name = $"Сервер {i + 1}",
                    Map = "Forest",
                    Players = Random.Range(1, 8),
                    MaxPlayers = 8,
                    Ping = Random.Range(10, 100),
                    IP = $"192.168.1.{100 + i}",
                    Port = 7777
                };
                _availableServers.Add(server);
                
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
            serverElement.AddToClassList("server-element");
            serverElement.RegisterCallback<ClickEvent>(_ => SelectServer(server));
            
            var nameLabel = new Label(server.Name);
            nameLabel.AddToClassList("server-name");
            serverElement.Add(nameLabel);
            
            var infoLabel = new Label($"{server.Map} | {server.Players}/{server.MaxPlayers} игроков | {server.Ping}ms");
            infoLabel.AddToClassList("server-info");
            serverElement.Add(infoLabel);
            
            _serverList.Add(serverElement);
        }
        
        /// <summary>
        /// Выбирает сервер
        /// </summary>
        private void SelectServer(ServerInfo server)
        {
            // Убираем выделение с других серверов
            var serverElements = _serverList.Children();
            foreach (var element in serverElements)
            {
                element.RemoveFromClassList("selected");
            }
            
            // Выделяем выбранный сервер
            var selectedElement = _serverList.Children().FirstOrDefault(e => 
                e.Q<Label>("server-name")?.text == server.Name);
            if (selectedElement != null)
                selectedElement.AddToClassList("selected");
        }
        
        /// <summary>
        /// Получает выбранный сервер
        /// </summary>
        private ServerInfo GetSelectedServer()
        {
            var selectedElement = _serverList.Children().FirstOrDefault(e => 
                e.ClassListContains("selected"));
            
            if (selectedElement != null)
            {
                var nameLabel = selectedElement.Q<Label>("server-name");
                if (nameLabel != null)
                {
                    return _availableServers.Find(s => s.Name == nameLabel.text);
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
        private System.Collections.IEnumerator SimulateGameCreation(GameSettings settings)
        {
            yield return new WaitForSeconds(2f);
            
            HideLoading();
            
            // Переходим к игре
            UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
        }
        
        /// <summary>
        /// Симулирует подключение к серверу
        /// </summary>
        private System.Collections.IEnumerator SimulateServerConnection(ServerInfo server)
        {
            yield return new WaitForSeconds(1.5f);
            
            HideLoading();
            
            // Переходим к игре
            UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
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
        /// Показывает ошибку
        /// </summary>
        private void ShowError(string message)
        {
            Debug.LogError($"Lobby Error: {message}");
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
