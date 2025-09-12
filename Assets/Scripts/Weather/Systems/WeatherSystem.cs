using Unity.Entities;
using Unity.Mathematics;
using Unity.Burst;
using MudLike.Weather.Components;

namespace MudLike.Weather.Systems
{
    /// <summary>
    /// Система погоды
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [BurstCompile]
    public partial class WeatherSystem : SystemBase
    {
        private EntityQuery _weatherQuery;
        
        protected override void OnCreate()
        {
            _weatherQuery = GetEntityQuery(
                ComponentType.ReadWrite<WeatherData>()
            );
        }
        
        protected override void OnUpdate()
        {
            float deltaTime = SystemAPI.Time.DeltaTime;
            
            // Обновляем погоду
            UpdateWeather(deltaTime);
        }
        
        private void UpdateWeather(float deltaTime)
        {
            // Обновляем каждую погодную систему
            Entities
                .WithAll<WeatherData>()
                .ForEach((ref WeatherData weather) =>
                {
                    UpdateWeatherData(ref weather, deltaTime);
                }).Schedule();
        }
        
        private void UpdateWeatherData(ref WeatherData weather, float deltaTime)
        {
            // Простая реализация погоды
            // В реальной реализации здесь будет сложная погодная система
            
            // Обновляем время суток
            weather.TimeOfDay += deltaTime * 0.1f; // Ускоренное время
            if (weather.TimeOfDay >= 24f)
            {
                weather.TimeOfDay = 0f;
            }
            
            // Обновляем температуру на основе времени суток
            float dayNightCycle = math.sin(weather.TimeOfDay * math.PI / 12f);
            weather.Temperature = 20f + dayNightCycle * 10f;
            
            // Обновляем время последнего обновления
            weather.LastUpdateTime += deltaTime;
            weather.NeedsUpdate = false;
        }
    }
}