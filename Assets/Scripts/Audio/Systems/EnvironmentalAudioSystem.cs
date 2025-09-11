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
                ComponentType.ReadWrite<EnvironmentalAudioData>()
            );
            
            _weatherQuery = GetEntityQuery(
                ComponentType.ReadOnly<WeatherData>()
            );
        }
        
        protected override void OnUpdate()
        {
            float deltaTime = Time.deltaTime;
            
            var environmentalAudioJob = new EnvironmentalAudioJob
            {
                DeltaTime = deltaTime
            };
            
            Dependency = environmentalAudioJob.ScheduleParallel(_environmentalAudioQuery, Dependency);
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
                if (!weatherData.HasValue) return;
                
                // Обновляем параметры звука
                audioData.WeatherType = weatherData.Value.WeatherType;
                audioData.RainIntensity = weatherData.Value.RainIntensity;
                audioData.SnowIntensity = weatherData.Value.SnowIntensity;
                audioData.WindSpeed = weatherData.Value.WindSpeed;
                
                // Вычисляем громкость на основе погоды
                audioData.Volume = CalculateEnvironmentalVolume(audioData.WeatherType, 
                                                               audioData.RainIntensity, 
                                                               audioData.SnowIntensity, 
                                                               audioData.WindSpeed);
                
                // Определяем, должен ли звук играть
                audioData.IsPlaying = audioData.Volume > 0.1f;
                
                audioData.NeedsUpdate = true;
            }
            
            /// <summary>
            /// Получает данные погоды
            /// </summary>
            private WeatherData? GetWeatherData()
            {
                // TODO: Реализовать получение данных погоды
                return new WeatherData
                {
                    WeatherType = WeatherType.Clear,
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
                volume += math.clamp(windSpeed / 20f, 0f, 1f) * 0.4f;
                
                return math.clamp(volume, 0f, 1f);
            }
            
            /// <summary>
            /// Получает базовую громкость для типа погоды
            /// </summary>
            private float GetWeatherBaseVolume(WeatherType weatherType)
            {
                return weatherType switch
                {
                    WeatherType.Clear => 0.1f,
                    WeatherType.Cloudy => 0.2f,
                    WeatherType.Rainy => 0.3f,
                    WeatherType.Snowy => 0.2f,
                    WeatherType.Foggy => 0.4f,
                    WeatherType.Stormy => 0.6f,
                    WeatherType.Windy => 0.3f,
                    WeatherType.Hot => 0.1f,
                    WeatherType.Cold => 0.2f,
                    WeatherType.Icy => 0.3f,
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
}