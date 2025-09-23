using Unity.Entities;
using Unity.Mathematics;

namespace MudLike.UI.Components
{
    /// <summary>
    /// Данные ввода UI
    /// </summary>
    public struct UIInput : IComponentData
    {
        /// <summary>
        /// Показать/скрыть HUD
        /// </summary>
        public bool ToggleHUD;
        
        /// <summary>
        /// Показать/скрыть меню
        /// </summary>
        public bool ToggleMenu;
        
        /// <summary>
        /// Показать/скрыть карту
        /// </summary>
        public bool ToggleMap;
        
        /// <summary>
        /// Показать/скрыть чат
        /// </summary>
        public bool ToggleChat;
        
        /// <summary>
        /// Показать/скрыть инвентарь
        /// </summary>
        public bool ToggleInventory;
        
        /// <summary>
        /// Показать/скрыть меню лебедки
        /// </summary>
        public bool ToggleWinchMenu;
        
        /// <summary>
        /// Показать/скрыть меню грузов
        /// </summary>
        public bool ToggleCargoMenu;
        
        /// <summary>
        /// Показать/скрыть меню миссий
        /// </summary>
        public bool ToggleMissionMenu;
        
        /// <summary>
        /// Пауза/возобновление игры
        /// </summary>
        public bool TogglePause;
        
        /// <summary>
        /// Выход из игры
        /// </summary>
        public bool QuitGame;
        
        /// <summary>
        /// Перезапуск игры
        /// </summary>
        public bool RestartGame;
        
        /// <summary>
        /// Сохранение игры
        /// </summary>
        public bool SaveGame;
        
        /// <summary>
        /// Загрузка игры
        /// </summary>
        public bool LoadGame;
        
        /// <summary>
        /// Ввод требует обработки
        /// </summary>
        public bool NeedsProcessing;
    }
}