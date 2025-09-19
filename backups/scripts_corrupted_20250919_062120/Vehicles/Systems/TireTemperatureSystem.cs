using Unity.Entities;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Jobs;
using Unity.Collections;
using MudLike.Vehicles.Components;
using MudLike.Weather.Components;

namespace MudLike.Vehicles.Systems
{
    /// <summary>
    /// Система управления температурой шин
    /// </summary>
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile(CompileSynchronously = true)]
    public partial class TireTemperatureSystem : SystemBase
    {
        private EntityQuery _tireQuery;
        private EntityQuery _weatherQuery;
        
        protected override void OnCreate()
        {
            _tireQuery = GetEntityQuery(
                ComponentType.ReadWrite<TireData>(),
                ComponentType.ReadOnly<WheelData>(),
                ComponentType.ReadOnly<VehiclePhysics>()
            );
            
            _weatherQuery = GetEntityQuery(
                ComponentType.ReadOnly<WeatherData>()
            );
        }
        
        protected override void OnUpdate()
        {
            float deltaTime = SystemAPI.Time.fixedDeltaTime;
            
            var tireTemperatureJob = new TireTemperatureJob
            {
                DeltaTime = deltaTime,
                WeatherData = GetWeatherData()
            };
            
            Dependency = tireTemperatureJob.ScheduleParallel(_tireQuery, Dependency);
        }
        
        /// <summary>
        /// Получает данные о погоде
        /// </summary>
        private NativeArray<WeatherData> GetWeatherData()
        {
            var weatherData = new NativeArray<WeatherData>(10, Allocator.TempJob);
            
            for (int i = 0; i < 10; i++)
            {
                weatherData[i] = WeatherProperties.GetWeatherProperties((WeatherType)i);
            }
            
            return weatherData;
        }
        
        /// <summary>
        /// Job для управления температурой шин
        /// </summary>
        [BurstCompile]
        public partial struct TireTemperatureJob : IJobEntity
        {
            public float DeltaTime;
            [ReadOnly] public NativeArray<WeatherData> WeatherData;
            
            public void Execute(ref TireData tire, in WheelData wheel, in VehiclePhysics vehiclePhysics)
            {
                ProcessTireTemperature(ref tire, wheel, vehiclePhysics);
            }
            
            /// <summary>
            /// Обрабатывает температуру шины
            /// </summary>
            private void ProcessTireTemperature(ref TireData tire, WheelData wheel, VehiclePhysics vehiclePhysics)
            {
                // Определяем тип погоды
                WeatherType weatherType = DetermineWeatherType();
                WeatherData weather = WeatherData[(int)weatherType];
                
                // Вычисляем изменения температуры
                float frictionHeating = CalculateFrictionHeating(tire, wheel, vehiclePhysics);
                float compressionHeating = CalculateCompressionHeating(tire, wheel, vehiclePhysics);
                float deformationHeating = CalculateDeformationHeating(tire, wheel, vehiclePhysics);
                float weatherCooling = CalculateWeatherCooling(tire, weather);
                float windCooling = CalculateWindCooling(tire, weather);
                float radiationCooling = CalculateRadiationCooling(tire, weather);
                
                // Обновляем температуру
                float totalChange = (frictionHeating + compressionHeating + deformationHeating + weatherCooling + windCooling + radiationCooling) * DeltaTime;
                tire.Temperature += totalChange;
                
                // Ограничиваем температуру
                tire.Temperature = math.clamp(tire.Temperature, -50f, tire.MaxTemperature);
                
                // Обновляем состояние шины
                UpdateTireCondition(ref tire);
            }
            
            /// <summary>
            /// Определяет тип погоды
            /// </summary>
            private WeatherType DetermineWeatherType()
            {
                // В реальной реализации здесь будет получение данных о погоде
                return (WeatherType)((int)(Time.time * 0.05f) % 10);
            }
            
            /// <summary>
            /// Вычисляет нагрев от трения
            /// </summary>
            private float CalculateFrictionHeating(TireData tire, WheelData wheel, VehiclePhysics vehiclePhysics)
            {
                // Базовая скорость нагрева от трения
                float baseHeating = 0.1f; // °C/с
                
                // Влияние скорости проскальзывания
                float slipRatio = math.abs(wheel.AngularVelocity * wheel.Radius - math.length(vehiclePhysics.Velocity)) / math.max(wheel.AngularVelocity * wheel.Radius, 0.1f);
                float slipEffect = 1f + slipRatio * 3f;
                
                // Влияние скорости движения
                float speedEffect = 1f + math.length(vehiclePhysics.Velocity) * 0.01f;
                
                // Влияние типа шины
                float tireEffect = GetTireFrictionHeatingEffect(tire.Type);
                
                // Влияние износа протектора
                float wearEffect = 1f + tire.TreadWear * 0.5f;
                
                return baseHeating * slipEffect * speedEffect * tireEffect * wearEffect;
            }
            
            /// <summary>
            /// Вычисляет нагрев от сжатия
            /// </summary>
            private float CalculateCompressionHeating(TireData tire, WheelData wheel, VehiclePhysics vehiclePhysics)
            {
                // Базовая скорость нагрева от сжатия
                float baseHeating = 0.05f; // °C/с
                
                // Влияние давления в шине
                float pressureEffect = tire.CurrentPressure / tire.RecommendedPressure;
                
                // Влияние нагрузки
                float loadEffect = vehiclePhysics.Mass / 1000f; // Нормализация
                
                // Влияние типа шины
                float tireEffect = GetTireCompressionHeatingEffect(tire.Type);
                
                return baseHeating * pressureEffect * loadEffect * tireEffect;
            }
            
            /// <summary>
            /// Вычисляет нагрев от деформации
            /// </summary>
            private float CalculateDeformationHeating(TireData tire, WheelData wheel, VehiclePhysics vehiclePhysics)
            {
                // Базовая скорость нагрева от деформации
                float baseHeating = 0.03f; // °C/с
                
                // Влияние скорости движения
                float speedEffect = 1f + math.length(vehiclePhysics.Velocity) * 0.005f;
                
                // Влияние типа шины
                float tireEffect = GetTireDeformationHeatingEffect(tire.Type);
                
                // Влияние износа протектора
                float wearEffect = 1f + tire.TreadWear * 0.3f;
                
                return baseHeating * speedEffect * tireEffect * wearEffect;
            }
            
            /// <summary>
            /// Вычисляет охлаждение от погоды
            /// </summary>
            private float CalculateWeatherCooling(TireData tire, WeatherData weather)
            {
                // Разность температур
                float tempDifference = tire.Temperature - weather.Temperature;
                
                // Коэффициент охлаждения
                float coolingCoefficient = 0.01f; // °C/с
                
                // Влияние типа шины
                float tireEffect = GetTireWeatherCoolingEffect(tire.Type);
                
                // Влияние влажности
                float humidityEffect = 1f + weather.Humidity * 0.2f;
                
                return -tempDifference * coolingCoefficient * tireEffect * humidityEffect;
            }
            
            /// <summary>
            /// Вычисляет охлаждение от ветра
            /// </summary>
            private float CalculateWindCooling(TireData tire, WeatherData weather)
            {
                // Базовая скорость охлаждения от ветра
                float baseCooling = 0.02f; // °C/с
                
                // Влияние скорости ветра
                float windEffect = 1f + weather.WindSpeed * 0.1f;
                
                // Влияние типа шины
                float tireEffect = GetTireWindCoolingEffect(tire.Type);
                
                // Влияние температуры
                float tempEffect = 1f + tire.Temperature / 100f * 0.5f;
                
                return -baseCooling * windEffect * tireEffect * tireEffect;
            }
            
            /// <summary>
            /// Вычисляет охлаждение от излучения
            /// </summary>
            private float CalculateRadiationCooling(TireData tire, WeatherData weather)
            {
                // Базовая скорость охлаждения от излучения
                float baseCooling = 0.01f; // °C/с
                
                // Влияние температуры
                float tempEffect = 1f + tire.Temperature / 100f * 0.3f;
                
                // Влияние типа шины
                float tireEffect = GetTireRadiationCoolingEffect(tire.Type);
                
                // Влияние влажности
                float humidityEffect = 1f + weather.Humidity * 0.1f;
                
                return -baseCooling * tempEffect * tireEffect * humidityEffect;
            }
            
            /// <summary>
            /// Получает эффект нагрева от трения для типа шины
            /// </summary>
            private float GetTireFrictionHeatingEffect(TireType tireType)
            {
                return tireType switch
                {
                    TireType.Summer => 1.0f,
                    TireType.Winter => 1.1f,
                    TireType.OffRoad => 1.2f,
                    TireType.Mud => 1.3f,
                    TireType.Street => 0.9f,
                    _ => 1.0f
                };
            }
            
            /// <summary>
            /// Получает эффект нагрева от сжатия для типа шины
            /// </summary>
            private float GetTireCompressionHeatingEffect(TireType tireType)
            {
                return tireType switch
                {
                    TireType.Summer => 1.0f,
                    TireType.Winter => 1.1f,
                    TireType.OffRoad => 1.2f,
                    TireType.Mud => 1.3f,
                    TireType.Street => 0.9f,
                    _ => 1.0f
                };
            }
            
            /// <summary>
            /// Получает эффект нагрева от деформации для типа шины
            /// </summary>
            private float GetTireDeformationHeatingEffect(TireType tireType)
            {
                return tireType switch
                {
                    TireType.Summer => 1.0f,
                    TireType.Winter => 1.1f,
                    TireType.OffRoad => 1.2f,
                    TireType.Mud => 1.3f,
                    TireType.Street => 0.9f,
                    _ => 1.0f
                };
            }
            
            /// <summary>
            /// Получает эффект охлаждения от погоды для типа шины
            /// </summary>
            private float GetTireWeatherCoolingEffect(TireType tireType)
            {
                return tireType switch
                {
                    TireType.Summer => 1.0f,
                    TireType.Winter => 1.1f,
                    TireType.OffRoad => 1.2f,
                    TireType.Mud => 1.3f,
                    TireType.Street => 0.9f,
                    _ => 1.0f
                };
            }
            
            /// <summary>
            /// Получает эффект охлаждения от ветра для типа шины
            /// </summary>
            private float GetTireWindCoolingEffect(TireType tireType)
            {
                return tireType switch
                {
                    TireType.Summer => 1.0f,
                    TireType.Winter => 1.1f,
                    TireType.OffRoad => 1.2f,
                    TireType.Mud => 1.3f,
                    TireType.Street => 0.9f,
                    _ => 1.0f
                };
            }
            
            /// <summary>
            /// Получает эффект охлаждения от излучения для типа шины
            /// </summary>
            private float GetTireRadiationCoolingEffect(TireType tireType)
            {
                return tireType switch
                {
                    TireType.Summer => 1.0f,
                    TireType.Winter => 1.1f,
                    TireType.OffRoad => 1.2f,
                    TireType.Mud => 1.3f,
                    TireType.Street => 0.9f,
                    _ => 1.0f
                };
            }
            
            /// <summary>
            /// Обновляет состояние шины
            /// </summary>
            private void UpdateTireCondition(ref TireData tire)
            {
                if (tire.Temperature >= tire.MaxTemperature)
                {
                    tire.Condition = TireCondition.Damaged;
                }
                else if (tire.Temperature >= tire.MaxTemperature * 0.9f)
                {
                    tire.Condition = TireCondition.Poor;
                }
                else if (tire.Temperature >= tire.MaxTemperature * 0.8f)
                {
                    tire.Condition = TireCondition.Fair;
                }
                else if (tire.Temperature >= tire.MaxTemperature * 0.7f)
                {
                    tire.Condition = TireCondition.Good;
                }
                else
                {
                    tire.Condition = TireCondition.New;
                }
            }
        }
    }
}