using Unity.Entities;
using Unity.Mathematics;

namespace MudLike.UI.Components
{
    /// <summary>
    /// Данные HUD интерфейса
    /// </summary>
    public struct UIHUDData : IComponentData
    {
        /// <summary>
        /// Скорость транспорта (км/ч)
        /// </summary>
        public float Speed;
        
        /// <summary>
        /// Обороты двигателя (RPM)
        /// </summary>
        public float RPM;
        
        /// <summary>
        /// Текущая передача
        /// </summary>
        public int CurrentGear;
        
        /// <summary>
        /// Здоровье транспорта (0-1)
        /// </summary>
        public float Health;
        
        /// <summary>
        /// Уровень топлива (0-1)
        /// </summary>
        public float FuelLevel;
        
        /// <summary>
        /// Температура двигателя (0-1)
        /// </summary>
        public float EngineTemperature;
        
        /// <summary>
        /// Информация о погоде
        /// </summary>
        public WeatherInfo WeatherInfo;
        
        /// <summary>
        /// Позиция на карте
        /// </summary>
        public float2 MapPosition;
        
        /// <summary>
        /// Направление движения
        /// </summary>
        public float Heading;
        
        /// <summary>
        /// Время игры
        /// </summary>
        public float GameTime;
        
        /// <summary>
        /// Количество игроков в сети
        /// </summary>
        public int PlayerCount;
        
        /// <summary>
        /// Ping в сети (мс)
        /// </summary>
        public int Ping;
        
        /// <summary>
        /// HUD активен
        /// </summary>
        public bool IsActive;
        
        /// <summary>
        /// HUD требует обновления
        /// </summary>
        public bool NeedsUpdate;
    }
    
    /// <summary>
    /// Информация о погоде для HUD
    /// </summary>
    public struct WeatherInfo
    {
        /// <summary>
        /// Тип погоды
        /// </summary>
        public WeatherType Type;
        
        /// <summary>
        /// Температура (°C)
        /// </summary>
        public float Temperature;
        
        /// <summary>
        /// Влажность (0-1)
        /// </summary>
        public float Humidity;
        
        /// <summary>
        /// Скорость ветра (м/с)
        /// </summary>
        public float WindSpeed;
        
        /// <summary>
        /// Интенсивность дождя (0-1)
        /// </summary>
        public float RainIntensity;
        
        /// <summary>
        /// Интенсивность снега (0-1)
        /// </summary>
        public float SnowIntensity;
        
        /// <summary>
        /// Видимость (0-1)
        /// </summary>
        public float Visibility;
    }
    
    /// <summary>
    /// Тип погоды
    /// </summary>
    public enum WeatherType
    {
        Clear,      // Ясно
        Cloudy,     // Облачно
        Rainy,      // Дождь
        Snowy,      // Снег
        Foggy,      // Туман
        Stormy,     // Буря
        Windy,      // Ветрено
        Hot,        // Жарко
        Cold,       // Холодно
        Icy         // Лед
    }
}