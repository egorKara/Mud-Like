using Unity.Entities;
using Unity.Mathematics;

namespace MudLike.UI.Components
{
    /// <summary>
    /// Данные главного меню
    /// </summary>
    public struct UIMenuData : IComponentData
    {
        /// <summary>
        /// Меню активно
        /// </summary>
        public bool IsActive;
        
        /// <summary>
        /// Текущий экран меню
        /// </summary>
        public MenuScreen CurrentScreen;
        
        /// <summary>
        /// Игра на паузе
        /// </summary>
        public bool IsPaused;
        
        /// <summary>
        /// Показывать настройки
        /// </summary>
        public bool ShowSettings;
        
        /// <summary>
        /// Показывать инвентарь
        /// </summary>
        public bool ShowInventory;
        
        /// <summary>
        /// Показывать карту
        /// </summary>
        public bool ShowMap;
        
        /// <summary>
        /// Показывать чат
        /// </summary>
        public bool ShowChat;
        
        /// <summary>
        /// Показывать меню лебедки
        /// </summary>
        public bool ShowWinchMenu;
        
        /// <summary>
        /// Показывать меню грузов
        /// </summary>
        public bool ShowCargoMenu;
        
        /// <summary>
        /// Показывать меню миссий
        /// </summary>
        public bool ShowMissionMenu;
        
        /// <summary>
        /// Меню требует обновления
        /// </summary>
        public bool NeedsUpdate;
    }
    
    /// <summary>
    /// Экран меню
    /// </summary>
    public enum MenuScreen
    {
        Main,           // Главное меню
        SinglePlayer,   // Одиночная игра
        Multiplayer,    // Мультиплеер
        Settings,       // Настройки
        Inventory,      // Инвентарь
        Map,            // Карта
        Chat,           // Чат
        Winch,          // Лебедка
        Cargo,          // Грузы
        Mission,        // Миссии
        Pause,          // Пауза
        GameOver,       // Конец игры
        Loading         // Загрузка
    }
}