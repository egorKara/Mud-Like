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
    /// Система износа шин с учетом различных факторов
    /// </summary>
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile(CompileSynchronously = true)]
    public partial class TireWearSystem : SystemBase
    {
        private EntityQuery _tireQuery;
        private EntityQuery _surfaceQuery;
        private EntityQuery _weatherQuery;
        
        protected override void OnCreate()
        {
            _tireQuery = GetEntityQuery(
                ComponentType.ReadWrite<TireData>(),
                ComponentType.ReadOnly<WheelData>(),
                ComponentType.ReadOnly<VehiclePhysics>()
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
            
            var tireWearJob = new TireWearJob
            {
                DeltaTime = deltaTime,
                SurfaceData = GetSurfaceData(),
                WeatherData = GetWeatherData()
            };
            
            Dependency = tireWearJob.ScheduleParallel(_tireQuery, Dependency);
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
        /// Job для износа шин
        /// </summary>
        [BurstCompile]
        public partial struct TireWearJob : IJobEntity
        {
            public float DeltaTime;
            [ReadOnly] public NativeArray<SurfaceData> SurfaceData;
            [ReadOnly] public NativeArray<WeatherData> WeatherData;
            
            public void Execute(ref TireData tire, in WheelData wheel, in VehiclePhysics vehiclePhysics)
            {
                ProcessTireWear(ref tire, wheel, vehiclePhysics);
            }
            
            /// <summary>
            /// Обрабатывает износ шины
            /// </summary>
            private void ProcessTireWear(ref TireData tire, WheelData wheel, VehiclePhysics vehiclePhysics)
            {
                if (!wheel.IsGrounded)
                    return;
                
                // Определяем тип поверхности и погоды
                SurfaceType surfaceType = DetermineSurfaceType(wheel);
                WeatherType weatherType = DetermineWeatherType();
                
                SurfaceData surface = SurfaceData[(int)surfaceType];
                WeatherData weather = WeatherData[(int)weatherType];
                
                // Вычисляем различные типы износа
                float frictionWear = CalculateFrictionWear(tire, surface, wheel, vehiclePhysics);
                float temperatureWear = CalculateTemperatureWear(tire, weather);
                float pressureWear = CalculatePressureWear(tire);
                float ageWear = CalculateAgeWear(tire);
                float mileageWear = CalculateMileageWear(tire, wheel, vehiclePhysics);
                
                // Обновляем износ протектора
                float totalWear = (frictionWear + temperatureWear + pressureWear + ageWear + mileageWear) * DeltaTime;
                tire.TreadWear += totalWear;
                tire.TreadWear = math.clamp(tire.TreadWear, 0f, 1f);
                
                // Обновляем глубину протектора
                tire.TreadDepth = tire.NewTreadDepth * (1f - tire.TreadWear);
                
                // Обновляем состояние шины
                UpdateTireCondition(ref tire);
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
            /// Вычисляет износ от трения
            /// </summary>
            private float CalculateFrictionWear(TireData tire, SurfaceData surface, WheelData wheel, VehiclePhysics vehiclePhysics)
            {
                // Базовая скорость износа
                float baseWearRate = 0.001f; // мм/с
                
                // Влияние скорости проскальзывания
                float slipRatio = math.abs(wheel.AngularVelocity * wheel.Radius - math.length(vehiclePhysics.Velocity)) / math.max(wheel.AngularVelocity * wheel.Radius, 0.1f);
                float slipEffect = 1f + slipRatio * 2f;
                
                // Влияние силы трения
                float frictionForce = surface.FrictionCoefficient * vehiclePhysics.Mass * 9.81f;
                float frictionEffect = frictionForce / 10000f; // Нормализация
                
                // Влияние типа поверхности
                float surfaceEffect = GetSurfaceWearEffect(surface.SurfaceType);
                
                // Влияние типа шины
                float tireEffect = GetTireWearEffect(tire.Type, surface.SurfaceType);
                
                // Влияние износа протектора
                float wearEffect = 1f + tire.TreadWear * 0.5f;
                
                return baseWearRate * slipEffect * frictionEffect * surfaceEffect * tireEffect * wearEffect;
            }
            
            /// <summary>
            /// Вычисляет износ от температуры
            /// </summary>
            private float CalculateTemperatureWear(TireData tire, WeatherData weather)
            {
                // Базовая скорость износа от температуры
                float baseWearRate = 0.0001f; // мм/с
                
                // Влияние температуры шины
                float tempEffect = math.clamp(tire.Temperature / 100f, 0.5f, 2f);
                
                // Влияние разности температур
                float tempDifference = math.abs(tire.Temperature - weather.Temperature);
                float tempDiffEffect = 1f + tempDifference * 0.01f;
                
                // Влияние типа шины
                float tireEffect = GetTireTemperatureWearEffect(tire.Type);
                
                return baseWearRate * tempEffect * tempDiffEffect * tireEffect;
            }
            
            /// <summary>
            /// Вычисляет износ от давления
            /// </summary>
            private float CalculatePressureWear(TireData tire)
            {
                // Базовая скорость износа от давления
                float baseWearRate = 0.0002f; // мм/с
                
                // Влияние отклонения давления от рекомендуемого
                float pressureRatio = tire.CurrentPressure / tire.RecommendedPressure;
                float pressureEffect = 1f + math.abs(pressureRatio - 1f) * 2f;
                
                // Влияние типа шины
                float tireEffect = GetTirePressureWearEffect(tire.Type);
                
                return baseWearRate * pressureEffect * tireEffect;
            }
            
            /// <summary>
            /// Вычисляет износ от возраста
            /// </summary>
            private float CalculateAgeWear(TireData tire)
            {
                // Базовая скорость износа от возраста
                float baseWearRate = 0.00005f; // мм/с
                
                // Влияние возраста
                float ageEffect = 1f + tire.Age / tire.MaxAge * 2f;
                
                // Влияние типа шины
                float tireEffect = GetTireAgeWearEffect(tire.Type);
                
                return baseWearRate * ageEffect * tireEffect;
            }
            
            /// <summary>
            /// Вычисляет износ от пробега
            /// </summary>
            private float CalculateMileageWear(TireData tire, WheelData wheel, VehiclePhysics vehiclePhysics)
            {
                // Базовая скорость износа от пробега
                float baseWearRate = 0.0001f; // мм/с
                
                // Влияние скорости
                float speedEffect = 1f + math.length(vehiclePhysics.Velocity) * 0.01f;
                
                // Влияние пробега
                float mileageEffect = 1f + tire.Mileage / tire.MaxMileage * 1.5f;
                
                // Влияние типа шины
                float tireEffect = GetTireMileageWearEffect(tire.Type);
                
                return baseWearRate * speedEffect * mileageEffect * tireEffect;
            }
            
            /// <summary>
            /// Получает эффект износа от поверхности
            /// </summary>
            private float GetSurfaceWearEffect(SurfaceType surfaceType)
            {
                return surfaceType switch
                {
                    SurfaceType.Asphalt => 1.0f,
                    SurfaceType.Concrete => 1.1f,
                    SurfaceType.Dirt => 1.5f,
                    SurfaceType.Mud => 2.0f,
                    SurfaceType.Sand => 1.8f,
                    SurfaceType.Grass => 1.2f,
                    SurfaceType.Water => 0.8f,
                    SurfaceType.Ice => 0.5f,
                    SurfaceType.Snow => 1.3f,
                    SurfaceType.Rock => 2.5f,
                    SurfaceType.Gravel => 1.7f,
                    SurfaceType.Swamp => 2.2f,
                    _ => 1.0f
                };
            }
            
            /// <summary>
            /// Получает эффект износа от типа шины
            /// </summary>
            private float GetTireWearEffect(TireType tireType, SurfaceType surfaceType)
            {
                return (tireType, surfaceType) switch
                {
                    (TireType.Summer, SurfaceType.Asphalt) => 1.0f,
                    (TireType.Summer, SurfaceType.Concrete) => 1.1f,
                    (TireType.Summer, SurfaceType.Dirt) => 1.5f,
                    (TireType.Summer, SurfaceType.Mud) => 2.0f,
                    (TireType.Summer, SurfaceType.Sand) => 1.8f,
                    (TireType.Summer, SurfaceType.Grass) => 1.2f,
                    (TireType.Summer, SurfaceType.Water) => 0.8f,
                    (TireType.Summer, SurfaceType.Ice) => 0.5f,
                    (TireType.Summer, SurfaceType.Snow) => 1.3f,
                    
                    (TireType.Winter, SurfaceType.Asphalt) => 1.2f,
                    (TireType.Winter, SurfaceType.Concrete) => 1.3f,
                    (TireType.Winter, SurfaceType.Dirt) => 1.4f,
                    (TireType.Winter, SurfaceType.Mud) => 1.6f,
                    (TireType.Winter, SurfaceType.Sand) => 1.5f,
                    (TireType.Winter, SurfaceType.Grass) => 1.3f,
                    (TireType.Winter, SurfaceType.Water) => 1.0f,
                    (TireType.Winter, SurfaceType.Ice) => 0.8f,
                    (TireType.Winter, SurfaceType.Snow) => 1.0f,
                    
                    (TireType.OffRoad, SurfaceType.Asphalt) => 1.3f,
                    (TireType.OffRoad, SurfaceType.Concrete) => 1.4f,
                    (TireType.OffRoad, SurfaceType.Dirt) => 1.0f,
                    (TireType.OffRoad, SurfaceType.Mud) => 1.1f,
                    (TireType.OffRoad, SurfaceType.Sand) => 1.2f,
                    (TireType.OffRoad, SurfaceType.Grass) => 1.1f,
                    (TireType.OffRoad, SurfaceType.Water) => 1.0f,
                    (TireType.OffRoad, SurfaceType.Ice) => 0.9f,
                    (TireType.OffRoad, SurfaceType.Snow) => 1.0f,
                    
                    (TireType.Mud, SurfaceType.Asphalt) => 1.5f,
                    (TireType.Mud, SurfaceType.Concrete) => 1.6f,
                    (TireType.Mud, SurfaceType.Dirt) => 1.2f,
                    (TireType.Mud, SurfaceType.Mud) => 1.0f,
                    (TireType.Mud, SurfaceType.Sand) => 1.3f,
                    (TireType.Mud, SurfaceType.Grass) => 1.1f,
                    (TireType.Mud, SurfaceType.Water) => 1.0f,
                    (TireType.Mud, SurfaceType.Ice) => 0.8f,
                    (TireType.Mud, SurfaceType.Snow) => 1.0f,
                    
                    _ => 1.0f
                };
            }
            
            /// <summary>
            /// Получает эффект износа от температуры для типа шины
            /// </summary>
            private float GetTireTemperatureWearEffect(TireType tireType)
            {
                return tireType switch
                {
                    TireType.Summer => 1.0f,
                    TireType.Winter => 1.2f,
                    TireType.OffRoad => 1.1f,
                    TireType.Mud => 1.3f,
                    TireType.Street => 0.9f,
                    _ => 1.0f
                };
            }
            
            /// <summary>
            /// Получает эффект износа от давления для типа шины
            /// </summary>
            private float GetTirePressureWearEffect(TireType tireType)
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
            /// Получает эффект износа от возраста для типа шины
            /// </summary>
            private float GetTireAgeWearEffect(TireType tireType)
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
            /// Получает эффект износа от пробега для типа шины
            /// </summary>
            private float GetTireMileageWearEffect(TireType tireType)
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
                if (tire.TreadWear >= 1f)
                {
                    tire.Condition = TireCondition.Worn;
                }
                else if (tire.Age >= tire.MaxAge)
                {
                    tire.Condition = TireCondition.Worn;
                }
                else if (tire.Mileage >= tire.MaxMileage)
                {
                    tire.Condition = TireCondition.Worn;
                }
                else if (tire.CurrentPressure <= tire.MinPressure * 0.8f)
                {
                    tire.Condition = TireCondition.Damaged;
                }
                else if (tire.Temperature >= tire.MaxTemperature * 0.9f)
                {
                    tire.Condition = TireCondition.Damaged;
                }
                else if (tire.TreadWear >= 0.8f)
                {
                    tire.Condition = TireCondition.Poor;
                }
                else if (tire.TreadWear >= 0.5f)
                {
                    tire.Condition = TireCondition.Fair;
                }
                else if (tire.TreadWear >= 0.2f)
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