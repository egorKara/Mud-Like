using Unity.Entities;
using Unity.Mathematics;

namespace MudLike.Weather.Components
{
    /// <summary>
    /// Данные о погодных условиях
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
        /// Требует ли обновления
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
    
    /// <summary>
    /// Статические свойства погоды
    /// </summary>
    public static class WeatherProperties
    {
        /// <summary>
        /// Получает свойства погоды по типу
        /// </summary>
        public static WeatherData GetWeatherProperties(WeatherType weatherType)
        {
            return weatherType switch
            {
                WeatherType.Clear => new WeatherData
                {
                    Type = weatherType,
                    Temperature = 20f,
                    Humidity = 0.4f,
                    WindSpeed = 2f,
                    WindDirection = 0f,
                    RainIntensity = 0f,
                    SnowIntensity = 0f,
                    SnowDepth = 0f,
                    IceThickness = 0f,
                    Visibility = 10000f,
                    AtmosphericPressure = 101.3f,
                    UVIndex = 5f,
                    TimeOfDay = 12f,
                    Season = Season.Summer
                },
                
                WeatherType.Cloudy => new WeatherData
                {
                    Type = weatherType,
                    Temperature = 15f,
                    Humidity = 0.6f,
                    WindSpeed = 5f,
                    WindDirection = 45f,
                    RainIntensity = 0f,
                    SnowIntensity = 0f,
                    SnowDepth = 0f,
                    IceThickness = 0f,
                    Visibility = 8000f,
                    AtmosphericPressure = 100.5f,
                    UVIndex = 3f,
                    TimeOfDay = 12f,
                    Season = Season.Autumn
                },
                
                WeatherType.Rainy => new WeatherData
                {
                    Type = weatherType,
                    Temperature = 10f,
                    Humidity = 0.9f,
                    WindSpeed = 8f,
                    WindDirection = 90f,
                    RainIntensity = 0.7f,
                    SnowIntensity = 0f,
                    SnowDepth = 0f,
                    IceThickness = 0f,
                    Visibility = 3000f,
                    AtmosphericPressure = 99.8f,
                    UVIndex = 1f,
                    TimeOfDay = 12f,
                    Season = Season.Autumn
                },
                
                WeatherType.Snowy => new WeatherData
                {
                    Type = weatherType,
                    Temperature = -5f,
                    Humidity = 0.8f,
                    WindSpeed = 6f,
                    WindDirection = 180f,
                    RainIntensity = 0f,
                    SnowIntensity = 0.8f,
                    SnowDepth = 15f,
                    IceThickness = 0f,
                    Visibility = 2000f,
                    AtmosphericPressure = 102.1f,
                    UVIndex = 0f,
                    TimeOfDay = 12f,
                    Season = Season.Winter
                },
                
                WeatherType.Foggy => new WeatherData
                {
                    Type = weatherType,
                    Temperature = 5f,
                    Humidity = 0.95f,
                    WindSpeed = 1f,
                    WindDirection = 0f,
                    RainIntensity = 0f,
                    SnowIntensity = 0f,
                    SnowDepth = 0f,
                    IceThickness = 0f,
                    Visibility = 500f,
                    AtmosphericPressure = 101.0f,
                    UVIndex = 0f,
                    TimeOfDay = 6f,
                    Season = Season.Spring
                },
                
                WeatherType.Stormy => new WeatherData
                {
                    Type = weatherType,
                    Temperature = 8f,
                    Humidity = 0.95f,
                    WindSpeed = 15f,
                    WindDirection = 270f,
                    RainIntensity = 1.0f,
                    SnowIntensity = 0f,
                    SnowDepth = 0f,
                    IceThickness = 0f,
                    Visibility = 1000f,
                    AtmosphericPressure = 98.5f,
                    UVIndex = 0f,
                    TimeOfDay = 12f,
                    Season = Season.Summer
                },
                
                WeatherType.Windy => new WeatherData
                {
                    Type = weatherType,
                    Temperature = 12f,
                    Humidity = 0.5f,
                    WindSpeed = 12f,
                    WindDirection = 225f,
                    RainIntensity = 0f,
                    SnowIntensity = 0f,
                    SnowDepth = 0f,
                    IceThickness = 0f,
                    Visibility = 6000f,
                    AtmosphericPressure = 100.0f,
                    UVIndex = 4f,
                    TimeOfDay = 12f,
                    Season = Season.Spring
                },
                
                WeatherType.Hot => new WeatherData
                {
                    Type = weatherType,
                    Temperature = 35f,
                    Humidity = 0.3f,
                    WindSpeed = 3f,
                    WindDirection = 0f,
                    RainIntensity = 0f,
                    SnowIntensity = 0f,
                    SnowDepth = 0f,
                    IceThickness = 0f,
                    Visibility = 12000f,
                    AtmosphericPressure = 100.8f,
                    UVIndex = 10f,
                    TimeOfDay = 14f,
                    Season = Season.Summer
                },
                
                WeatherType.Cold => new WeatherData
                {
                    Type = weatherType,
                    Temperature = -15f,
                    Humidity = 0.6f,
                    WindSpeed = 4f,
                    WindDirection = 0f,
                    RainIntensity = 0f,
                    SnowIntensity = 0f,
                    SnowDepth = 5f,
                    IceThickness = 0f,
                    Visibility = 8000f,
                    AtmosphericPressure = 103.2f,
                    UVIndex = 1f,
                    TimeOfDay = 12f,
                    Season = Season.Winter
                },
                
                WeatherType.Icy => new WeatherData
                {
                    Type = weatherType,
                    Temperature = -10f,
                    Humidity = 0.7f,
                    WindSpeed = 2f,
                    WindDirection = 0f,
                    RainIntensity = 0f,
                    SnowIntensity = 0f,
                    SnowDepth = 0f,
                    IceThickness = 2f,
                    Visibility = 5000f,
                    AtmosphericPressure = 102.5f,
                    UVIndex = 0f,
                    TimeOfDay = 12f,
                    Season = Season.Winter
                },
                
                _ => new WeatherData()
            };
        }
        
        /// <summary>
        /// Вычисляет влияние погоды на шины
        /// </summary>
        public static float GetTireWeatherEffect(WeatherData weather, TireData tire)
        {
            float effect = 1f;
            
            // Влияние температуры
            float tempEffect = 1f;
            if (weather.Temperature < 0f)
            {
                // Холод делает резину жестче
                tempEffect = math.lerp(0.7f, 1f, (weather.Temperature + 20f) / 20f);
            }
            else if (weather.Temperature > 30f)
            {
                // Жара делает резину мягче
                tempEffect = math.lerp(1f, 1.3f, (weather.Temperature - 30f) / 20f);
            }
            
            // Влияние влажности
            float humidityEffect = math.lerp(1f, 0.8f, weather.Humidity);
            
            // Влияние дождя
            float rainEffect = math.lerp(1f, 0.6f, weather.RainIntensity);
            
            // Влияние снега
            float snowEffect = math.lerp(1f, 0.4f, weather.SnowIntensity);
            
            // Влияние льда
            float iceEffect = math.lerp(1f, 0.2f, weather.IceThickness / 5f);
            
            // Влияние ветра
            float windEffect = math.lerp(1f, 0.9f, weather.WindSpeed / 20f);
            
            effect = tempEffect * humidityEffect * rainEffect * snowEffect * iceEffect * windEffect;
            
            return math.clamp(effect, 0.1f, 2f);
        }
    }
}