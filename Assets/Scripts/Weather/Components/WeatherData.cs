using Unity.Entities;
using Unity.Mathematics;

namespace MudLike.Weather.Components
{
    /// <summary>
    /// Данные погоды
    /// </summary>
    public struct WeatherData : IComponentData
    {
        /// <summary>
        /// Тип погоды
        /// </summary>
        public WeatherType Type;
        
        /// <summary>
        /// Температура воздуха (°C)
        /// </summary>
        public float Temperature;
        
        /// <summary>
        /// Влажность воздуха (0-1)
        /// </summary>
        public float Humidity;
        
        /// <summary>
        /// Скорость ветра (м/с)
        /// </summary>
        public float WindSpeed;
        
        /// <summary>
        /// Направление ветра (градусы)
        /// </summary>
        public float WindDirection;
        
        /// <summary>
        /// Интенсивность дождя (0-1)
        /// </summary>
        public float RainIntensity;
        
        /// <summary>
        /// Интенсивность снега (0-1)
        /// </summary>
        public float SnowIntensity;
        
        /// <summary>
        /// Толщина снежного покрова (см)
        /// </summary>
        public float SnowDepth;
        
        /// <summary>
        /// Толщина льда (см)
        /// </summary>
        public float IceThickness;
        
        /// <summary>
        /// Видимость (м)
        /// </summary>
        public float Visibility;
        
        /// <summary>
        /// Атмосферное давление (кПа)
        /// </summary>
        public float AtmosphericPressure;
        
        /// <summary>
        /// УФ-индекс (0-11)
        /// </summary>
        public float UVIndex;
        
        /// <summary>
        /// Время суток (0-24 часа)
        /// </summary>
        public float TimeOfDay;
        
        /// <summary>
        /// Сезон
        /// </summary>
        public Season Season;
        
        /// <summary>
        /// Время последнего обновления
        /// </summary>
        public float LastUpdateTime;
        
        /// <summary>
        /// Требует обновления
        /// </summary>
        public bool NeedsUpdate;
    }
    
    /// <summary>
    /// Типы погоды
    /// </summary>
    public enum WeatherType
    {
        Clear,      // Ясно
        Cloudy,     // Облачно
        Rainy,      // Дождливо
        Snowy,      // Снежно
        Foggy,      // Туманно
        Stormy,     // Грозово
        Windy,      // Ветрено
        Hot,        // Жарко
        Cold,       // Холодно
        Icy         // Ледяно
    }
    
    /// <summary>
    /// Сезоны
    /// </summary>
    public enum Season
    {
        Spring,     // Весна
        Summer,     // Лето
        Autumn,     // Осень
        Winter      // Зима
    }
}