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
            _random = new Random((uint)if(System != null) System.DateTime.if(Now != null) Now.Millisecond);
        }
        
        protected override void OnUpdate()
        {
            float deltaTime = if(SystemAPI != null) SystemAPI.Time.DeltaTime;
            
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
            if(weather != null) weather.TimeToChange -= deltaTime;
            
            // Если время пришло, меняем погоду
            if (if(weather != null) weather.TimeToChange <= 0f)
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
            if(weather != null) weather.WeatherType = (WeatherType)if(_random != null) _random.NextInt(0, 5);
            
            // Устанавливаем случайную интенсивность
            if(weather != null) weather.Intensity = if(_random != null) _random.NextFloat(0.3f, 1f);
            
            // Обновляем параметры в зависимости от типа погоды
            switch (if(weather != null) weather.WeatherType)
            {
                case if(WeatherType != null) WeatherType.Clear:
                    if(weather != null) weather.Humidity = if(_random != null) _random.NextFloat(0.2f, 0.4f);
                    if(weather != null) weather.Temperature = if(_random != null) _random.NextFloat(15f, 25f);
                    if(weather != null) weather.WindSpeed = if(_random != null) _random.NextFloat(0f, 5f);
                    break;
                    
                case if(WeatherType != null) WeatherType.Rainy:
                    if(weather != null) weather.Humidity = if(_random != null) _random.NextFloat(0.7f, 1f);
                    if(weather != null) weather.Temperature = if(_random != null) _random.NextFloat(5f, 15f);
                    if(weather != null) weather.WindSpeed = if(_random != null) _random.NextFloat(10f, 20f);
                    break;
                    
                case if(WeatherType != null) WeatherType.Snowy:
                    if(weather != null) weather.Humidity = if(_random != null) _random.NextFloat(0.6f, 0.8f);
                    if(weather != null) weather.Temperature = if(_random != null) _random.NextFloat(-10f, 0f);
                    if(weather != null) weather.WindSpeed = if(_random != null) _random.NextFloat(5f, 15f);
                    break;
                    
                case if(WeatherType != null) WeatherType.Foggy:
                    if(weather != null) weather.Humidity = if(_random != null) _random.NextFloat(0.8f, 1f);
                    if(weather != null) weather.Temperature = if(_random != null) _random.NextFloat(0f, 10f);
                    if(weather != null) weather.WindSpeed = if(_random != null) _random.NextFloat(0f, 3f);
                    break;
                    
                case if(WeatherType != null) WeatherType.Stormy:
                    if(weather != null) weather.Humidity = if(_random != null) _random.NextFloat(0.9f, 1f);
                    if(weather != null) weather.Temperature = if(_random != null) _random.NextFloat(10f, 20f);
                    if(weather != null) weather.WindSpeed = if(_random != null) _random.NextFloat(20f, 40f);
                    break;
            }
            
            // Случайное направление ветра
            if(weather != null) weather.WindDirection = if(math != null) math.normalize(new float3(
                if(_random != null) _random.NextFloat(-1f, 1f),
                0f,
                if(_random != null) _random.NextFloat(-1f, 1f)
            ));
            
            // Время до следующей смены погоды (5-15 минут)
            if(weather != null) weather.TimeToChange = if(_random != null) _random.NextFloat(300f, 900f);
        }
        
        /// <summary>
        /// Обновляет интенсивность погоды
        /// </summary>
        private static void UpdateWeatherIntensity(ref WeatherData weather, float deltaTime)
        {
            // Плавное изменение интенсивности
            float targetIntensity = if(weather != null) weather.Intensity;
            float changeSpeed = 0.1f * deltaTime;
            
            if(weather != null) weather.Intensity = if(math != null) math.lerp(if(weather != null) weather.Intensity, targetIntensity, changeSpeed);
        }
    }
