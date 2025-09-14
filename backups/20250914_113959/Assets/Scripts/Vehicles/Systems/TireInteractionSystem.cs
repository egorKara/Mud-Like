using Unity.Entities;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Jobs;
using Unity.Collections;
using MudLike.Vehicles.Components;
using MudLike.Terrain.Components;
using MudLike.Weather.Components;

namespace MudLike.Vehicles.Systems
{
    /// <summary>
    /// Система взаимодействия шин с поверхностями и погодой
    /// </summary>
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile(CompileSynchronously = true)]
    public partial class TireInteractionSystem : SystemBase
    {
        private EntityQuery _tireQuery;
        private EntityQuery _surfaceQuery;
        private EntityQuery _weatherQuery;
        
        protected override void OnCreate()
        {
            RequireForUpdate<PhysicsWorldSingleton>();
            
            _tireQuery = GetEntityQuery(
                ComponentType.ReadWrite<TireData>(),
                ComponentType.ReadOnly<WheelData>(),
                ComponentType.ReadOnly<LocalTransform>()
            );
            
            _surfaceQuery = GetEntityQuery(
                ComponentType.ReadOnly<SurfaceData>()
            );
            
            _weatherQuery = GetEntityQuery(
                ComponentType.ReadOnly<WeatherData>()
            );
        }
        
        protected override void OnUpdate()
        {
            float deltaTime = SystemAPI.Time.fixedDeltaTime;
            
            var tireInteractionJob = new TireInteractionJob
            {
                DeltaTime = deltaTime,
                SurfaceData = GetSurfaceData(),
                WeatherData = GetWeatherData()
            };
            
            Dependency = tireInteractionJob.ScheduleParallel(_tireQuery, Dependency);
        }
        
        /// <summary>
        /// Получает данные о поверхностях
        /// </summary>
        private NativeArray<SurfaceData> GetSurfaceData()
        {
            var surfaceData = new NativeArray<SurfaceData>(11, Allocator.Temp);
            
            for (int i = 0; i < 11; i++)
            {
                surfaceData[i] = SurfaceProperties.GetSurfaceProperties((SurfaceType)i);
            }
            
            return surfaceData;
        }
        
        /// <summary>
        /// Получает данные о погоде
        /// </summary>
        private NativeArray<WeatherData> GetWeatherData()
        {
            var weatherData = new NativeArray<WeatherData>(10, Allocator.Temp);
            
            for (int i = 0; i < 10; i++)
            {
                weatherData[i] = WeatherProperties.GetWeatherProperties((WeatherType)i);
            }
            
            return weatherData;
        }
        
        /// <summary>
        /// Job для взаимодействия шин с поверхностями и погодой
        /// </summary>
        [BurstCompile]
        public partial struct TireInteractionJob : IJobEntity
        {
            public float DeltaTime;
            [ReadOnly] public NativeArray<SurfaceData> SurfaceData;
            [ReadOnly] public NativeArray<WeatherData> WeatherData;
            
            public void Execute(ref TireData tire, in WheelData wheel, in LocalTransform wheelTransform)
            {
                ProcessTireInteraction(ref tire, wheel, wheelTransform);
            }
            
            /// <summary>
            /// Обрабатывает взаимодействие шины с поверхностью и погодой
            /// </summary>
            private void ProcessTireInteraction(ref TireData tire, WheelData wheel, LocalTransform wheelTransform)
            {
                if (!wheel.IsGrounded)
                    return;
                
                // Определяем тип поверхности и погоды
                SurfaceType surfaceType = DetermineSurfaceType(wheel);
                WeatherType weatherType = DetermineWeatherType();
                
                SurfaceData surface = SurfaceData[(int)surfaceType];
                WeatherData weather = WeatherData[(int)weatherType];
                
                // Обрабатываем взаимодействие с поверхностью
                ProcessSurfaceInteraction(ref tire, surface, wheel);
                
                // Обрабатываем взаимодействие с погодой
                ProcessWeatherInteraction(ref tire, weather, wheel);
                
                // Обрабатываем комбинированные эффекты
                ProcessCombinedEffects(ref tire, surface, weather, wheel);
            }
            
            /// <summary>
            /// Определяет тип поверхности
            /// </summary>
            private SurfaceType DetermineSurfaceType(WheelData wheel)
            {
                // В реальной реализации здесь будет определение по материалам
                return (SurfaceType)((int)(SystemAPI.Time.ElapsedTime * 0.1f) % 11);
            }
            
            /// <summary>
            /// Определяет тип погоды
            /// </summary>
            private WeatherType DetermineWeatherType()
            {
                // В реальной реализации здесь будет получение данных о погоде
                return (WeatherType)((int)(SystemAPI.Time.ElapsedTime * 0.05f) % 10);
            }
            
            /// <summary>
            /// Обрабатывает взаимодействие с поверхностью
            /// </summary>
            private void ProcessSurfaceInteraction(ref TireData tire, SurfaceData surface, WheelData wheel)
            {
                // Влияние типа поверхности на износ
                float surfaceWearEffect = GetSurfaceWearEffect(surface.SurfaceType, tire.Type);
                tire.WearRate *= surfaceWearEffect;
                
                // Влияние поверхности на температуру
                float surfaceTempEffect = GetSurfaceTemperatureEffect(surface, tire);
                tire.Temperature += surfaceTempEffect * DeltaTime;
                
                // Влияние поверхности на давление
                float surfacePressureEffect = GetSurfacePressureEffect(surface, tire);
                tire.CurrentPressure += surfacePressureEffect * DeltaTime;
                
                // Влияние поверхности на сцепление
                float surfaceTractionEffect = GetSurfaceTractionEffect(surface, tire);
                tire.TractionCoefficient *= surfaceTractionEffect;
                
                // Влияние поверхности на сопротивление качению
                float surfaceRollingEffect = GetSurfaceRollingEffect(surface, tire);
                tire.RollingResistance *= surfaceRollingEffect;
            }
            
            /// <summary>
            /// Обрабатывает взаимодействие с погодой
            /// </summary>
            private void ProcessWeatherInteraction(ref TireData tire, WeatherData weather, WheelData wheel)
            {
                // Влияние температуры на шину
                float tempEffect = GetTemperatureEffect(weather.Temperature, tire);
                tire.Temperature += tempEffect * DeltaTime;
                
                // Влияние влажности на шину
                float humidityEffect = GetHumidityEffect(weather.Humidity, tire);
                tire.Moisture += humidityEffect * DeltaTime;
                
                // Влияние дождя на шину
                float rainEffect = GetRainEffect(weather.RainIntensity, tire);
                tire.Moisture += rainEffect * DeltaTime;
                
                // Влияние снега на шину
                float snowEffect = GetSnowEffect(weather.SnowIntensity, tire);
                tire.Moisture += snowEffect * DeltaTime;
                
                // Влияние ветра на шину
                float windEffect = GetWindEffect(weather.WindSpeed, tire);
                tire.Temperature += windEffect * DeltaTime;
            }
            
            /// <summary>
            /// Обрабатывает комбинированные эффекты
            /// </summary>
            private void ProcessCombinedEffects(ref TireData tire, SurfaceData surface, WeatherData weather, WheelData wheel)
            {
                // Комбинированный эффект температуры
                float combinedTempEffect = GetCombinedTemperatureEffect(tire, surface, weather);
                tire.Temperature += combinedTempEffect * DeltaTime;
                
                // Комбинированный эффект влажности
                float combinedMoistureEffect = GetCombinedMoistureEffect(tire, surface, weather);
                tire.Moisture += combinedMoistureEffect * DeltaTime;
                
                // Комбинированный эффект износа
                float combinedWearEffect = GetCombinedWearEffect(tire, surface, weather);
                tire.WearRate *= combinedWearEffect;
                
                // Комбинированный эффект сцепления
                float combinedTractionEffect = GetCombinedTractionEffect(tire, surface, weather);
                tire.TractionCoefficient *= combinedTractionEffect;
            }
            
            /// <summary>
            /// Получает эффект износа от поверхности
            /// </summary>
            private float GetSurfaceWearEffect(SurfaceType surfaceType, TireType tireType)
            {
                return (surfaceType, tireType) switch
                {
                    (SurfaceType.Asphalt, _) => 1.0f,
                    (SurfaceType.Concrete, _) => 1.1f,
                    (SurfaceType.Dirt, _) => 1.5f,
                    (SurfaceType.Mud, _) => 2.0f,
                    (SurfaceType.Sand, _) => 1.8f,
                    (SurfaceType.Grass, _) => 1.2f,
                    (SurfaceType.Water, _) => 0.8f,
                    (SurfaceType.Ice, _) => 0.5f,
                    (SurfaceType.Snow, _) => 1.3f,
                    (SurfaceType.Rock, _) => 2.5f,
                    (SurfaceType.Gravel, _) => 1.7f,
                    (SurfaceType.Swamp, _) => 2.2f,
                    _ => 1.0f
                };
            }
            
            /// <summary>
            /// Получает эффект температуры от поверхности
            /// </summary>
            private float GetSurfaceTemperatureEffect(SurfaceData surface, TireData tire)
            {
                float tempDifference = surface.Temperature - tire.Temperature;
                float heatTransfer = tempDifference * 0.01f; // Коэффициент теплопередачи
                
                return heatTransfer;
            }
            
            /// <summary>
            /// Получает эффект давления от поверхности
            /// </summary>
            private float GetSurfacePressureEffect(SurfaceData surface, TireData tire)
            {
                float pressureChange = 0f;
                
                // Влияние плотности поверхности
                if (surface.Density > 2000f) // Твердые поверхности
                {
                    pressureChange = 0.1f; // Небольшое увеличение давления
                }
                else if (surface.Density < 1000f) // Мягкие поверхности
                {
                    pressureChange = -0.1f; // Небольшое снижение давления
                }
                
                return pressureChange;
            }
            
            /// <summary>
            /// Получает эффект сцепления от поверхности
            /// </summary>
            private float GetSurfaceTractionEffect(SurfaceData surface, TireData tire)
            {
                float baseTraction = surface.TractionCoefficient;
                
                // Влияние типа шины
                float tireEffect = GetTireTypeEffect(tire.Type, surface.SurfaceType);
                
                return baseTraction * tireEffect;
            }
            
            /// <summary>
            /// Получает эффект сопротивления качению от поверхности
            /// </summary>
            private float GetSurfaceRollingEffect(SurfaceData surface, TireData tire)
            {
                float baseResistance = surface.RollingResistance;
                
                // Влияние типа шины
                float tireEffect = GetTireTypeResistanceEffect(tire.Type, surface.SurfaceType);
                
                return baseResistance * tireEffect;
            }
            
            /// <summary>
            /// Получает эффект температуры от погоды
            /// </summary>
            private float GetTemperatureEffect(float weatherTemp, TireData tire)
            {
                float tempDifference = weatherTemp - tire.Temperature;
                float heatTransfer = tempDifference * 0.005f; // Коэффициент теплопередачи
                
                return heatTransfer;
            }
            
            /// <summary>
            /// Получает эффект влажности от погоды
            /// </summary>
            private float GetHumidityEffect(float humidity, TireData tire)
            {
                float moistureGain = humidity * 0.01f; // Накопление влажности
                float moistureLoss = tire.Moisture * 0.02f; // Потеря влажности
                
                return moistureGain - moistureLoss;
            }
            
            /// <summary>
            /// Получает эффект дождя
            /// </summary>
            private float GetRainEffect(float rainIntensity, TireData tire)
            {
                return rainIntensity * 0.05f; // Накопление влажности от дождя
            }
            
            /// <summary>
            /// Получает эффект снега
            /// </summary>
            private float GetSnowEffect(float snowIntensity, TireData tire)
            {
                return snowIntensity * 0.03f; // Накопление влажности от снега
            }
            
            /// <summary>
            /// Получает эффект ветра
            /// </summary>
            private float GetWindEffect(float windSpeed, TireData tire)
            {
                return -windSpeed * 0.01f; // Охлаждение от ветра
            }
            
            /// <summary>
            /// Получает комбинированный эффект температуры
            /// </summary>
            private float GetCombinedTemperatureEffect(TireData tire, SurfaceData surface, WeatherData weather)
            {
                float surfaceEffect = GetSurfaceTemperatureEffect(surface, tire);
                float weatherEffect = GetTemperatureEffect(weather.Temperature, tire);
                
                return (surfaceEffect + weatherEffect) * 0.5f;
            }
            
            /// <summary>
            /// Получает комбинированный эффект влажности
            /// </summary>
            private float GetCombinedMoistureEffect(TireData tire, SurfaceData surface, WeatherData weather)
            {
                float surfaceEffect = surface.Moisture * 0.01f;
                float weatherEffect = GetHumidityEffect(weather.Humidity, tire);
                
                return (surfaceEffect + weatherEffect) * 0.5f;
            }
            
            /// <summary>
            /// Получает комбинированный эффект износа
            /// </summary>
            private float GetCombinedWearEffect(TireData tire, SurfaceData surface, WeatherData weather)
            {
                float surfaceEffect = GetSurfaceWearEffect(surface.SurfaceType, tire.Type);
                float weatherEffect = GetWeatherWearEffect(weather, tire);
                
                return (surfaceEffect + weatherEffect) * 0.5f;
            }
            
            /// <summary>
            /// Получает комбинированный эффект сцепления
            /// </summary>
            private float GetCombinedTractionEffect(TireData tire, SurfaceData surface, WeatherData weather)
            {
                float surfaceEffect = GetSurfaceTractionEffect(surface, tire);
                float weatherEffect = GetWeatherTractionEffect(weather, tire);
                
                return (surfaceEffect + weatherEffect) * 0.5f;
            }
            
            /// <summary>
            /// Получает эффект износа от погоды
            /// </summary>
            private float GetWeatherWearEffect(WeatherData weather, TireData tire)
            {
                float wearEffect = 1.0f;
                
                // Влияние температуры
                if (weather.Temperature < 0f)
                {
                    wearEffect *= 1.2f; // Холод увеличивает износ
                }
                else if (weather.Temperature > 30f)
                {
                    wearEffect *= 1.3f; // Жара увеличивает износ
                }
                
                // Влияние влажности
                wearEffect *= (1f + weather.Humidity * 0.2f);
                
                // Влияние дождя
                wearEffect *= (1f + weather.RainIntensity * 0.3f);
                
                // Влияние снега
                wearEffect *= (1f + weather.SnowIntensity * 0.2f);
                
                return wearEffect;
            }
            
            /// <summary>
            /// Получает эффект сцепления от погоды
            /// </summary>
            private float GetWeatherTractionEffect(WeatherData weather, TireData tire)
            {
                float tractionEffect = 1.0f;
                
                // Влияние дождя
                tractionEffect *= (1f - weather.RainIntensity * 0.4f);
                
                // Влияние снега
                tractionEffect *= (1f - weather.SnowIntensity * 0.3f);
                
                // Влияние льда
                tractionEffect *= (1f - weather.IceThickness * 0.5f);
                
                // Влияние температуры
                if (weather.Temperature < 0f)
                {
                    tractionEffect *= 0.8f; // Холод снижает сцепление
                }
                
                return tractionEffect;
            }
            
            /// <summary>
            /// Получает эффект типа шины на сцепление
            /// </summary>
            private float GetTireTypeEffect(TireType tireType, SurfaceType surfaceType)
            {
                return (tireType, surfaceType) switch
                {
                    (TireType.Summer, SurfaceType.Asphalt) => 1.2f,
                    (TireType.Summer, SurfaceType.Concrete) => 1.1f,
                    (TireType.Summer, SurfaceType.Dirt) => 0.8f,
                    (TireType.Summer, SurfaceType.Mud) => 0.4f,
                    (TireType.Summer, SurfaceType.Sand) => 0.6f,
                    (TireType.Summer, SurfaceType.Grass) => 0.7f,
                    (TireType.Summer, SurfaceType.Water) => 0.3f,
                    (TireType.Summer, SurfaceType.Ice) => 0.1f,
                    (TireType.Summer, SurfaceType.Snow) => 0.2f,
                    
                    (TireType.Winter, SurfaceType.Asphalt) => 0.9f,
                    (TireType.Winter, SurfaceType.Concrete) => 0.8f,
                    (TireType.Winter, SurfaceType.Dirt) => 0.7f,
                    (TireType.Winter, SurfaceType.Mud) => 0.5f,
                    (TireType.Winter, SurfaceType.Sand) => 0.6f,
                    (TireType.Winter, SurfaceType.Grass) => 0.8f,
                    (TireType.Winter, SurfaceType.Water) => 0.4f,
                    (TireType.Winter, SurfaceType.Ice) => 0.8f,
                    (TireType.Winter, SurfaceType.Snow) => 1.0f,
                    
                    (TireType.OffRoad, SurfaceType.Asphalt) => 0.8f,
                    (TireType.OffRoad, SurfaceType.Concrete) => 0.7f,
                    (TireType.OffRoad, SurfaceType.Dirt) => 1.2f,
                    (TireType.OffRoad, SurfaceType.Mud) => 1.0f,
                    (TireType.OffRoad, SurfaceType.Sand) => 1.1f,
                    (TireType.OffRoad, SurfaceType.Grass) => 1.0f,
                    (TireType.OffRoad, SurfaceType.Water) => 0.6f,
                    (TireType.OffRoad, SurfaceType.Ice) => 0.3f,
                    (TireType.OffRoad, SurfaceType.Snow) => 0.7f,
                    
                    (TireType.Mud, SurfaceType.Asphalt) => 0.6f,
                    (TireType.Mud, SurfaceType.Concrete) => 0.5f,
                    (TireType.Mud, SurfaceType.Dirt) => 1.1f,
                    (TireType.Mud, SurfaceType.Mud) => 1.3f,
                    (TireType.Mud, SurfaceType.Sand) => 0.9f,
                    (TireType.Mud, SurfaceType.Grass) => 1.0f,
                    (TireType.Mud, SurfaceType.Water) => 0.5f,
                    (TireType.Mud, SurfaceType.Ice) => 0.2f,
                    (TireType.Mud, SurfaceType.Snow) => 0.6f,
                    
                    _ => 1.0f
                };
            }
            
            /// <summary>
            /// Получает эффект типа шины на сопротивление качению
            /// </summary>
            private float GetTireTypeResistanceEffect(TireType tireType, SurfaceType surfaceType)
            {
                return (tireType, surfaceType) switch
                {
                    (TireType.Summer, _) => 1.0f,
                    (TireType.Winter, _) => 1.2f,
                    (TireType.OffRoad, _) => 1.5f,
                    (TireType.Mud, _) => 1.8f,
                    (TireType.Street, _) => 0.9f,
                    _ => 1.0f
                };
            }
        }
    }
}