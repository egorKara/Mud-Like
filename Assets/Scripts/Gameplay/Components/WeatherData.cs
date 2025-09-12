using Unity.Entities;
using Unity.Mathematics;

namespace MudLike.Gameplay.Components
{
    /// <summary>
    /// Данные о погодных условиях
    /// </summary>
    public struct WeatherData : IComponentData
    {
        /// <summary>
        /// Тип погоды
        /// </summary>
        public WeatherType WeatherType;
        
        /// <summary>
        /// Интенсивность погоды (0-1)
        /// </summary>
        public float Intensity;
        
        /// <summary>
        /// Влажность (0-1)
        /// </summary>
        public float Humidity;
        
        /// <summary>
        /// Температура в градусах Цельсия
        /// </summary>
        public float Temperature;
        
        /// <summary>
        /// Скорость ветра
        /// </summary>
        public float WindSpeed;
        
        /// <summary>
        /// Направление ветра
        /// </summary>
        public float3 WindDirection;
        
        /// <summary>
        /// Время до смены погоды
        /// </summary>
        public float TimeToChange;
    }
    
    /// <summary>
    /// Типы погоды
    /// </summary>
    public enum WeatherType
    {
        Clear,      // Ясно
        Rainy,      // Дождь
        Snowy,      // Снег
        Foggy,      // Туман
        Stormy      // Буря
    }
}