using Unity.Entities;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Jobs;
using MudLike.Gameplay.Components;

namespace MudLike.Gameplay.Systems
{
    /// <summary>
    /// Система управления погодными условиями
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [BurstCompile]
    public partial class WeatherSystem : SystemBase
    {
        private Random _random;
        
        protected override void OnCreate()
        {
            _random = new Random((uint)System.DateTime.Now.Millisecond);
        }
        
        protected override void OnUpdate()
        {
            float deltaTime = SystemAPI.Time.DeltaTime;
            
            Entities
                .WithAll<WeatherData>()
                .ForEach((ref WeatherData weather) =>
                {
                    ProcessWeather(ref weather, deltaTime);
                }).Schedule();
        }
        
        /// <summary>
        /// Обрабатывает изменения погоды
        /// </summary>
        private void ProcessWeather(ref WeatherData weather, float deltaTime)
        {
            // Уменьшаем время до смены погоды
            weather.TimeToChange -= deltaTime;
            
            // Если время пришло, меняем погоду
            if (weather.TimeToChange <= 0f)
            {
                ChangeWeather(ref weather);
            }
            
            // Обновляем интенсивность погоды
            UpdateWeatherIntensity(ref weather, deltaTime);
        }
        
        /// <summary>
        /// Меняет погоду на случайную
        /// </summary>
        private void ChangeWeather(ref WeatherData weather)
        {
            // Выбираем случайный тип погоды
            weather.WeatherType = (WeatherType)_random.NextInt(0, 5);
            
            // Устанавливаем случайную интенсивность
            weather.Intensity = _random.NextFloat(0.3f, 1f);
            
            // Обновляем параметры в зависимости от типа погоды
            switch (weather.WeatherType)
            {
                case WeatherType.Clear:
                    weather.Humidity = _random.NextFloat(0.2f, 0.4f);
                    weather.Temperature = _random.NextFloat(15f, 25f);
                    weather.WindSpeed = _random.NextFloat(0f, 5f);
                    break;
                    
                case WeatherType.Rainy:
                    weather.Humidity = _random.NextFloat(0.7f, 1f);
                    weather.Temperature = _random.NextFloat(5f, 15f);
                    weather.WindSpeed = _random.NextFloat(10f, 20f);
                    break;
                    
                case WeatherType.Snowy:
                    weather.Humidity = _random.NextFloat(0.6f, 0.8f);
                    weather.Temperature = _random.NextFloat(-10f, 0f);
                    weather.WindSpeed = _random.NextFloat(5f, 15f);
                    break;
                    
                case WeatherType.Foggy:
                    weather.Humidity = _random.NextFloat(0.8f, 1f);
                    weather.Temperature = _random.NextFloat(0f, 10f);
                    weather.WindSpeed = _random.NextFloat(0f, 3f);
                    break;
                    
                case WeatherType.Stormy:
                    weather.Humidity = _random.NextFloat(0.9f, 1f);
                    weather.Temperature = _random.NextFloat(10f, 20f);
                    weather.WindSpeed = _random.NextFloat(20f, 40f);
                    break;
            }
            
            // Случайное направление ветра
            weather.WindDirection = math.normalize(new float3(
                _random.NextFloat(-1f, 1f),
                0f,
                _random.NextFloat(-1f, 1f)
            ));
            
            // Время до следующей смены погоды (5-15 минут)
            weather.TimeToChange = _random.NextFloat(300f, 900f);
        }
        
        /// <summary>
        /// Обновляет интенсивность погоды
        /// </summary>
        private static void UpdateWeatherIntensity(ref WeatherData weather, float deltaTime)
        {
            // Плавное изменение интенсивности
            float targetIntensity = weather.Intensity;
            float changeSpeed = 0.1f * deltaTime;
            
            weather.Intensity = math.lerp(weather.Intensity, targetIntensity, changeSpeed);
        }
    }
}