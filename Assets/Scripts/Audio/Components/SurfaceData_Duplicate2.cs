using Unity.Entities;
using Unity.Mathematics;
using MudLike.Terrain.Components;

namespace MudLike.Audio.Components
{
    /// <summary>
    /// Данные поверхности для аудио системы
    /// </summary>
    public struct SurfaceData : IComponentData
    {
        /// <summary>
        /// Тип поверхности
        /// </summary>
        public SurfaceType SurfaceType;
        
        /// <summary>
        /// Позиция поверхности
        /// </summary>
        public float3 Position;
        
        /// <summary>
        /// Уровень грязи (0-1)
        /// </summary>
        public float MudLevel;
        
        /// <summary>
        /// Влажность поверхности
        /// </summary>
        public float Wetness;
        
        /// <summary>
        /// Твердость поверхности
        /// </summary>
        public float Hardness;
        
        /// <summary>
        /// Шероховатость поверхности
        /// </summary>
        public float Roughness;
    }
    
    /// <summary>
    /// Данные погоды для аудио системы
    /// </summary>
    public struct WeatherData : IComponentData
    {
        /// <summary>
        /// Тип погоды
        /// </summary>
        public WeatherType WeatherType;
        
        /// <summary>
        /// Интенсивность дождя (0-1)
        /// </summary>
        public float RainIntensity;
        
        /// <summary>
        /// Интенсивность снега (0-1)
        /// </summary>
        public float SnowIntensity;
        
        /// <summary>
        /// Скорость ветра
        /// </summary>
        public float WindSpeed;
        
        /// <summary>
        /// Направление ветра
        /// </summary>
        public float3 WindDirection;
        
        /// <summary>
        /// Температура
        /// </summary>
        public float Temperature;
        
        /// <summary>
        /// Влажность воздуха
        /// </summary>
        public float Humidity;
    }
    
    /// <summary>
    /// Тип погоды
    /// </summary>
    public enum WeatherType : byte
    {
        /// <summary>
        /// Ясно
        /// </summary>
        Clear,
        
        /// <summary>
        /// Облачно
        /// </summary>
        Cloudy,
        
        /// <summary>
        /// Дождь
        /// </summary>
        Rain,
        
        /// <summary>
        /// Снег
        /// </summary>
        Snow,
        
        /// <summary>
        /// Туман
        /// </summary>
        Fog,
        
        /// <summary>
        /// Буря
        /// </summary>
        Storm
    }
}
