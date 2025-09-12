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
    /// Система управления давлением в шинах
    /// </summary>
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile(CompileSynchronously = true)]
    public partial class TirePressureSystem : SystemBase
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
            
            var tirePressureJob = new TirePressureJob
            {
                DeltaTime = deltaTime,
                WeatherData = GetWeatherData()
            };
            
            Dependency = tirePressureJob.ScheduleParallel(_tireQuery, Dependency);
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
        /// Job для управления давлением в шинах
        /// </summary>
        [BurstCompile]
        public partial struct TirePressureJob : IJobEntity
        {
            public float DeltaTime;
            [ReadOnly] public NativeArray<WeatherData> WeatherData;
            
            public void Execute(ref TireData tire, in WheelData wheel, in VehiclePhysics vehiclePhysics)
            {
                ProcessTirePressure(ref tire, wheel, vehiclePhysics);
            }
            
            /// <summary>
            /// Обрабатывает давление в шине
            /// </summary>
            private void ProcessTirePressure(ref TireData tire, WheelData wheel, VehiclePhysics vehiclePhysics)
            {
                // Определяем тип погоды
                WeatherType weatherType = DetermineWeatherType();
                WeatherData weather = WeatherData[(int)weatherType];
                
                // Вычисляем изменения давления
                float temperatureChange = CalculateTemperaturePressureChange(tire, weather);
                float loadChange = CalculateLoadPressureChange(tire, wheel, vehiclePhysics);
                float speedChange = CalculateSpeedPressureChange(tire, wheel, vehiclePhysics);
                float leakageChange = CalculateLeakagePressureChange(tire);
                
                // Обновляем давление
                float totalChange = (temperatureChange + loadChange + speedChange + leakageChange) * DeltaTime;
                tire.CurrentPressure += totalChange;
                
                // Ограничиваем давление
                tire.CurrentPressure = math.clamp(tire.CurrentPressure, tire.MinPressure, tire.MaxPressure);
                
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
            /// Вычисляет изменение давления от температуры
            /// </summary>
            private float CalculateTemperaturePressureChange(TireData tire, WeatherData weather)
            {
                // Изменение температуры
                float tempChange = weather.Temperature - tire.Temperature;
                
                // Коэффициент изменения давления от температуры (0.1 кПа на градус)
                float pressureCoefficient = 0.1f;
                
                // Влияние типа шины
                float tireEffect = GetTireTemperaturePressureEffect(tire.Type);
                
                return tempChange * pressureCoefficient * tireEffect;
            }
            
            /// <summary>
            /// Вычисляет изменение давления от нагрузки
            /// </summary>
            private float CalculateLoadPressureChange(TireData tire, WheelData wheel, VehiclePhysics vehiclePhysics)
            {
                // Базовая нагрузка на шину
                float baseLoad = vehiclePhysics.Mass * 9.81f / 4f; // Предполагаем 4 колеса
                
                // Влияние нагрузки на давление
                float loadEffect = baseLoad / 10000f; // Нормализация
                
                // Влияние типа шины
                float tireEffect = GetTireLoadPressureEffect(tire.Type);
                
                return loadEffect * tireEffect * 0.01f; // Небольшое изменение
            }
            
            /// <summary>
            /// Вычисляет изменение давления от скорости
            /// </summary>
            private float CalculateSpeedPressureChange(TireData tire, WheelData wheel, VehiclePhysics vehiclePhysics)
            {
                // Скорость движения
                float speed = math.length(vehiclePhysics.Velocity);
                
                // Влияние скорости на давление
                float speedEffect = speed * 0.001f; // Небольшое увеличение с ростом скорости
                
                // Влияние типа шины
                float tireEffect = GetTireSpeedPressureEffect(tire.Type);
                
                return speedEffect * tireEffect;
            }
            
            /// <summary>
            /// Вычисляет изменение давления от утечек
            /// </summary>
            private float CalculateLeakagePressureChange(TireData tire)
            {
                // Базовая скорость утечки
                float baseLeakage = 0.001f; // кПа/с
                
                // Влияние возраста шины
                float ageEffect = 1f + tire.Age / tire.MaxAge * 2f;
                
                // Влияние износа протектора
                float wearEffect = 1f + tire.TreadWear * 1.5f;
                
                // Влияние температуры
                float tempEffect = 1f + tire.Temperature / 100f * 0.5f;
                
                // Влияние типа шины
                float tireEffect = GetTireLeakageEffect(tire.Type);
                
                return -baseLeakage * ageEffect * wearEffect * tempEffect * tireEffect;
            }
            
            /// <summary>
            /// Получает эффект температуры на давление для типа шины
            /// </summary>
            private float GetTireTemperaturePressureEffect(TireType tireType)
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
            /// Получает эффект нагрузки на давление для типа шины
            /// </summary>
            private float GetTireLoadPressureEffect(TireType tireType)
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
            /// Получает эффект скорости на давление для типа шины
            /// </summary>
            private float GetTireSpeedPressureEffect(TireType tireType)
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
            /// Получает эффект утечек для типа шины
            /// </summary>
            private float GetTireLeakageEffect(TireType tireType)
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
                if (tire.CurrentPressure <= tire.MinPressure * 0.8f)
                {
                    tire.Condition = TireCondition.Damaged;
                }
                else if (tire.CurrentPressure <= tire.MinPressure)
                {
                    tire.Condition = TireCondition.Poor;
                }
                else if (tire.CurrentPressure >= tire.MaxPressure)
                {
                    tire.Condition = TireCondition.Damaged;
                }
                else if (tire.CurrentPressure >= tire.MaxPressure * 0.9f)
                {
                    tire.Condition = TireCondition.Poor;
                }
                else if (tire.CurrentPressure >= tire.RecommendedPressure * 1.1f)
                {
                    tire.Condition = TireCondition.Fair;
                }
                else if (tire.CurrentPressure <= tire.RecommendedPressure * 0.9f)
                {
                    tire.Condition = TireCondition.Fair;
                }
                else
                {
                    tire.Condition = TireCondition.Good;
                }
            }
        }
    }
}