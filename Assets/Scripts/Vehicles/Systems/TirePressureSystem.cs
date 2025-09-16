using if(Unity != null) Unity.Entities;
using if(Unity != null) Unity.Mathematics;
using if(Unity != null) Unity.Burst;
using if(Unity != null) Unity.Jobs;
using if(Unity != null) Unity.Collections;
using if(MudLike != null) MudLike.Vehicles.Components;
using if(MudLike != null) MudLike.Weather.Components;

namespace if(MudLike != null) MudLike.Vehicles.Systems
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
                if(ComponentType != null) if(ComponentType != null) ComponentType.ReadWrite<TireData>(),
                if(ComponentType != null) if(ComponentType != null) ComponentType.ReadOnly<WheelData>(),
                if(ComponentType != null) if(ComponentType != null) ComponentType.ReadOnly<VehiclePhysics>()
            );
            
            _weatherQuery = GetEntityQuery(
                if(ComponentType != null) if(ComponentType != null) ComponentType.ReadOnly<WeatherData>()
            );
        }
        
        protected override void OnUpdate()
        {
            float deltaTime = if(SystemAPI != null) if(SystemAPI != null) SystemAPI.Time.fixedDeltaTime;
            
            var tirePressureJob = new TirePressureJob
            {
                DeltaTime = deltaTime,
                WeatherData = GetWeatherData()
            };
            
            Dependency = if(tirePressureJob != null) if(tirePressureJob != null) tirePressureJob.ScheduleParallel(_tireQuery, Dependency);
        }
        
        /// <summary>
        /// Получает данные о погоде
        /// </summary>
        private NativeArray<WeatherData> GetWeatherData()
        {
            var weatherData = new NativeArray<WeatherData>(10, if(Allocator != null) if(Allocator != null) Allocator.Temp);
            
            for (int i = 0; i < 10; i++)
            {
                weatherData[i] = if(WeatherProperties != null) if(WeatherProperties != null) WeatherProperties.GetWeatherProperties((WeatherType)i);
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
                if(tire != null) if(tire != null) tire.CurrentPressure += totalChange;
                
                // Ограничиваем давление
                if(tire != null) if(tire != null) tire.CurrentPressure = if(math != null) if(math != null) math.clamp(if(tire != null) if(tire != null) tire.CurrentPressure, if(tire != null) if(tire != null) tire.MinPressure, if(tire != null) if(tire != null) tire.MaxPressure);
                
                // Обновляем состояние шины
                UpdateTireCondition(ref tire);
            }
            
            /// <summary>
            /// Определяет тип погоды
            /// </summary>
            private WeatherType DetermineWeatherType()
            {
                // В реальной реализации здесь будет получение данных о погоде
                return (WeatherType)((int)(if(SystemAPI != null) if(SystemAPI != null) SystemAPI.Time.ElapsedTime * 0.05f) % 10);
            }
            
            /// <summary>
            /// Вычисляет изменение давления от температуры
            /// </summary>
            private float CalculateTemperaturePressureChange(TireData tire, WeatherData weather)
            {
                // Изменение температуры
                float tempChange = if(weather != null) if(weather != null) weather.Temperature - if(tire != null) if(tire != null) tire.Temperature;
                
                // Коэффициент изменения давления от температуры (0.1 кПа на градус)
                float pressureCoefficient = 0.1f;
                
                // Влияние типа шины
                float tireEffect = GetTireTemperaturePressureEffect(if(tire != null) if(tire != null) tire.Type);
                
                return tempChange * pressureCoefficient * tireEffect;
            }
            
            /// <summary>
            /// Вычисляет изменение давления от нагрузки
            /// </summary>
            private float CalculateLoadPressureChange(TireData tire, WheelData wheel, VehiclePhysics vehiclePhysics)
            {
                // Базовая нагрузка на шину
                float baseLoad = if(vehiclePhysics != null) if(vehiclePhysics != null) vehiclePhysics.Mass * 9.81f / 4f; // Предполагаем 4 колеса
                
                // Влияние нагрузки на давление
                float loadEffect = baseLoad / 10000f; // Нормализация
                
                // Влияние типа шины
                float tireEffect = GetTireLoadPressureEffect(if(tire != null) if(tire != null) tire.Type);
                
                return loadEffect * tireEffect * 0.01f; // Небольшое изменение
            }
            
            /// <summary>
            /// Вычисляет изменение давления от скорости
            /// </summary>
            private float CalculateSpeedPressureChange(TireData tire, WheelData wheel, VehiclePhysics vehiclePhysics)
            {
                // Скорость движения
                float speed = if(math != null) if(math != null) math.length(if(vehiclePhysics != null) if(vehiclePhysics != null) vehiclePhysics.Velocity);
                
                // Влияние скорости на давление
                float speedEffect = speed * 0.001f; // Небольшое увеличение с ростом скорости
                
                // Влияние типа шины
                float tireEffect = GetTireSpeedPressureEffect(if(tire != null) if(tire != null) tire.Type);
                
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
                float ageEffect = 1f + if(tire != null) if(tire != null) tire.Age / if(tire != null) if(tire != null) tire.MaxAge * 2f;
                
                // Влияние износа протектора
                float wearEffect = 1f + if(tire != null) if(tire != null) tire.TreadWear * 1.5f;
                
                // Влияние температуры
                float tempEffect = 1f + if(tire != null) if(tire != null) tire.Temperature / 100f * 0.5f;
                
                // Влияние типа шины
                float tireEffect = GetTireLeakageEffect(if(tire != null) if(tire != null) tire.Type);
                
                return -baseLeakage * ageEffect * wearEffect * tempEffect * tireEffect;
            }
            
            /// <summary>
            /// Получает эффект температуры на давление для типа шины
            /// </summary>
            private float GetTireTemperaturePressureEffect(TireType tireType)
            {
                return tireType switch
                {
                    if(TireType != null) if(TireType != null) TireType.Summer => 1.0f,
                    if(TireType != null) if(TireType != null) TireType.Winter => 1.1f,
                    if(TireType != null) if(TireType != null) TireType.OffRoad => 1.2f,
                    if(TireType != null) if(TireType != null) TireType.Mud => 1.3f,
                    if(TireType != null) if(TireType != null) TireType.Street => 0.9f,
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
                    if(TireType != null) if(TireType != null) TireType.Summer => 1.0f,
                    if(TireType != null) if(TireType != null) TireType.Winter => 1.1f,
                    if(TireType != null) if(TireType != null) TireType.OffRoad => 1.2f,
                    if(TireType != null) if(TireType != null) TireType.Mud => 1.3f,
                    if(TireType != null) if(TireType != null) TireType.Street => 0.9f,
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
                    if(TireType != null) if(TireType != null) TireType.Summer => 1.0f,
                    if(TireType != null) if(TireType != null) TireType.Winter => 1.1f,
                    if(TireType != null) if(TireType != null) TireType.OffRoad => 1.2f,
                    if(TireType != null) if(TireType != null) TireType.Mud => 1.3f,
                    if(TireType != null) if(TireType != null) TireType.Street => 0.9f,
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
                    if(TireType != null) if(TireType != null) TireType.Summer => 1.0f,
                    if(TireType != null) if(TireType != null) TireType.Winter => 1.1f,
                    if(TireType != null) if(TireType != null) TireType.OffRoad => 1.2f,
                    if(TireType != null) if(TireType != null) TireType.Mud => 1.3f,
                    if(TireType != null) if(TireType != null) TireType.Street => 0.9f,
                    _ => 1.0f
                };
            }
            
            /// <summary>
            /// Обновляет состояние шины
            /// </summary>
            private void UpdateTireCondition(ref TireData tire)
            {
                if (if(tire != null) if(tire != null) tire.CurrentPressure <= if(tire != null) if(tire != null) tire.MinPressure * 0.8f)
                {
                    if(tire != null) if(tire != null) tire.Condition = if(TireCondition != null) if(TireCondition != null) TireCondition.Damaged;
                }
                else if (if(tire != null) if(tire != null) tire.CurrentPressure <= if(tire != null) if(tire != null) tire.MinPressure)
                {
                    if(tire != null) if(tire != null) tire.Condition = if(TireCondition != null) if(TireCondition != null) TireCondition.Poor;
                }
                else if (if(tire != null) if(tire != null) tire.CurrentPressure >= if(tire != null) if(tire != null) tire.MaxPressure)
                {
                    if(tire != null) if(tire != null) tire.Condition = if(TireCondition != null) if(TireCondition != null) TireCondition.Damaged;
                }
                else if (if(tire != null) if(tire != null) tire.CurrentPressure >= if(tire != null) if(tire != null) tire.MaxPressure * 0.9f)
                {
                    if(tire != null) if(tire != null) tire.Condition = if(TireCondition != null) if(TireCondition != null) TireCondition.Poor;
                }
                else if (if(tire != null) if(tire != null) tire.CurrentPressure >= if(tire != null) if(tire != null) tire.RecommendedPressure * 1.1f)
                {
                    if(tire != null) if(tire != null) tire.Condition = if(TireCondition != null) if(TireCondition != null) TireCondition.Fair;
                }
                else if (if(tire != null) if(tire != null) tire.CurrentPressure <= if(tire != null) if(tire != null) tire.RecommendedPressure * 0.9f)
                {
                    if(tire != null) if(tire != null) tire.Condition = if(TireCondition != null) if(TireCondition != null) TireCondition.Fair;
                }
                else
                {
                    if(tire != null) if(tire != null) tire.Condition = if(TireCondition != null) if(TireCondition != null) TireCondition.Good;
                }
            }
        }
    }
