using Unity.Entities;
using Unity.Mathematics;
using MudLike.Weather.Components;

namespace MudLike.UI.Components
{
    /// <summary>
    /// Данные HUD интерфейса
    /// </summary>
    public struct UIHUDData : IComponentData
    {
        /// <summary>
        /// Скорость транспорта
        /// </summary>
        public float Speed;
        
        /// <summary>
        /// Обороты двигателя
        /// </summary>
        public float RPM;
        
        /// <summary>
        /// Текущая передача
        /// </summary>
        public int CurrentGear;
        
        /// <summary>
        /// Здоровье транспорта
        /// </summary>
        public float Health;
        
        /// <summary>
        /// Уровень топлива
        /// </summary>
        public float FuelLevel;
        
        /// <summary>
        /// Температура двигателя
        /// </summary>
        public float EngineTemperature;
        
        /// <summary>
        /// Информация о погоде
        /// </summary>
        public WeatherInfo WeatherInfo;
        
        /// <summary>
        /// HUD активен
        /// </summary>
        public bool IsActive;
        
        /// <summary>
        /// Требует обновления
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
        /// Температура
        /// </summary>
        public float Temperature;
        
        /// <summary>
        /// Влажность
        /// </summary>
        public float Humidity;
    }
}