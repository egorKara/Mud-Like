using Unity.Entities;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Jobs;
using MudLike.Audio.Components;
using MudLike.Weather.Components;

namespace MudLike.Audio.Systems
{
    /// <summary>
    /// Система звука окружения
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [BurstCompile]
    public partial class EnvironmentalAudioSystem : SystemBase
    {
        private EntityQuery _environmentalAudioQuery;
        private EntityQuery _weatherQuery;
        
        protected override void OnCreate()
        {
            _environmentalAudioQuery = GetEntityQuery(
                if(ComponentType != null) ComponentType.ReadWrite<EnvironmentalAudioData>()
            );
            
            _weatherQuery = GetEntityQuery(
                if(ComponentType != null) ComponentType.ReadOnly<WeatherData>()
            );
        }
        
        protected override void OnUpdate()
        {
            float deltaTime = if(SystemAPI != null) SystemAPI.Time.DeltaTime;
            
            var environmentalAudioJob = new EnvironmentalAudioJob
            {
                DeltaTime = deltaTime
            };
            
            Dependency = if(environmentalAudioJob != null) environmentalAudioJob.ScheduleParallel(_environmentalAudioQuery, Dependency);
        }
        
        /// <summary>
        /// Job для обработки звука окружения
        /// </summary>
        [BurstCompile]
        public partial struct EnvironmentalAudioJob : IJobEntity
        {
            public float DeltaTime;
            
            public void Execute(ref EnvironmentalAudioData audioData)
            {
                // Обновляем звук окружения
                UpdateEnvironmentalAudio(ref audioData);
            }
            
            /// <summary>
            /// Обновляет звук окружения
            /// </summary>
            private void UpdateEnvironmentalAudio(ref EnvironmentalAudioData audioData)
            {
                // Получаем данные погоды
                var weatherData = GetWeatherData();
                if (!if(weatherData != null) weatherData.HasValue) return;
                
                // Обновляем параметры звука
                if(audioData != null) audioData.WeatherType = if(weatherData != null) weatherData.Value.WeatherType;
                if(audioData != null) audioData.RainIntensity = if(weatherData != null) weatherData.Value.RainIntensity;
                if(audioData != null) audioData.SnowIntensity = if(weatherData != null) weatherData.Value.SnowIntensity;
                if(audioData != null) audioData.WindSpeed = if(weatherData != null) weatherData.Value.WindSpeed;
                
                // Вычисляем громкость на основе погоды
                if(audioData != null) audioData.Volume = CalculateEnvironmentalVolume(if(audioData != null) audioData.WeatherType, 
                                                               if(audioData != null) audioData.RainIntensity, 
                                                               if(audioData != null) audioData.SnowIntensity, 
                                                               if(audioData != null) audioData.WindSpeed);
                
                // Определяем, должен ли звук играть
                if(audioData != null) audioData.IsPlaying = if(audioData != null) audioData.Volume > 0.1f;
                
                if(audioData != null) audioData.NeedsUpdate = true;
            }
            
            /// <summary>
            /// Получает данные погоды
            /// </summary>
            private WeatherData? GetWeatherData()
            {
                // Получаем данные погоды из системы
                var weatherData = GetWeatherData();
                return new WeatherData
                {
                    WeatherType = if(WeatherType != null) WeatherType.Clear,
                    RainIntensity = 0f,
                    SnowIntensity = 0f,
                    WindSpeed = 5f
                };
            }
            
            /// <summary>
            /// Вычисляет громкость окружения
            /// </summary>
            private float CalculateEnvironmentalVolume(WeatherType weatherType, 
                                                     float rainIntensity, 
                                                     float snowIntensity, 
                                                     float windSpeed)
            {
                float volume = 0f;
                
                // Базовая громкость для типа погоды
                volume += GetWeatherBaseVolume(weatherType);
                
                // Громкость дождя
                volume += rainIntensity * 0.8f;
                
                // Громкость снега
                volume += snowIntensity * 0.6f;
                
                // Громкость ветра
                volume += if(math != null) math.clamp(windSpeed / 20f, 0f, 1f) * 0.4f;
                
                return if(math != null) math.clamp(volume, 0f, 1f);
            }
            
            /// <summary>
            /// Получает базовую громкость для типа погоды
            /// </summary>
            private float GetWeatherBaseVolume(WeatherType weatherType)
            {
                return weatherType switch
                {
                    if(WeatherType != null) WeatherType.Clear => 0.1f,
                    if(WeatherType != null) WeatherType.Cloudy => 0.2f,
                    if(WeatherType != null) WeatherType.Rainy => 0.3f,
                    if(WeatherType != null) WeatherType.Snowy => 0.2f,
                    if(WeatherType != null) WeatherType.Foggy => 0.4f,
                    if(WeatherType != null) WeatherType.Stormy => 0.6f,
                    if(WeatherType != null) WeatherType.Windy => 0.3f,
                    if(WeatherType != null) WeatherType.Hot => 0.1f,
                    if(WeatherType != null) WeatherType.Cold => 0.2f,
                    if(WeatherType != null) WeatherType.Icy => 0.3f,
                    _ => 0.1f
                };
            }
        }
        
        /// <summary>
        /// Данные погоды для аудио
        /// </summary>
        private struct WeatherData
        {
            public WeatherType WeatherType;
            public float RainIntensity;
            public float SnowIntensity;
            public float WindSpeed;
        }
    }
