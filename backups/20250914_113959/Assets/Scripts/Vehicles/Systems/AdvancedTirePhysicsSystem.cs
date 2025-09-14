using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;
using Unity.Burst;
using Unity.Jobs;
using Unity.Collections;
using MudLike.Vehicles.Components;
using MudLike.Terrain.Components;
using MudLike.Weather.Components;

namespace MudLike.Vehicles.Systems
{
    /// <summary>
    /// Продвинутая система физики шин с учетом погоды и поверхностей
    /// </summary>
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile]
    public partial class AdvancedTirePhysicsSystem : SystemBase
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
                ComponentType.ReadOnly<LocalTransform>(),
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
            var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().PhysicsWorld;
            
            var surfaceData = GetSurfaceData();
            var weatherData = GetWeatherData();
            
            var tirePhysicsJob = new AdvancedTirePhysicsJob
            {
                DeltaTime = deltaTime,
                PhysicsWorld = physicsWorld,
                SurfaceData = surfaceData,
                WeatherData = weatherData
            };
            
            Dependency = tirePhysicsJob.ScheduleParallel(_tireQuery, Dependency);
            
            // Освобождаем временные массивы после планирования job
            surfaceData.Dispose();
            weatherData.Dispose();
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
        /// Job для продвинутой физики шин
        /// </summary>
        [BurstCompile]
        public partial struct AdvancedTirePhysicsJob : IJobEntity
        {
            public float DeltaTime;
            [ReadOnly] public PhysicsWorld PhysicsWorld;
            [ReadOnly] public NativeArray<SurfaceData> SurfaceData;
            [ReadOnly] public NativeArray<WeatherData> WeatherData;
            
            public void Execute(ref TireData tire, 
                              in WheelData wheel, 
                              in LocalTransform wheelTransform,
                              in VehiclePhysics vehiclePhysics)
            {
                ProcessAdvancedTirePhysics(ref tire, wheel, wheelTransform, vehiclePhysics);
            }
            
            /// <summary>
            /// Обрабатывает продвинутую физику шины
            /// </summary>
            private void ProcessAdvancedTirePhysics(ref TireData tire,
                                                 WheelData wheel,
                                                 LocalTransform wheelTransform,
                                                 VehiclePhysics vehiclePhysics)
            {
                // Выполняем raycast для определения поверхности
                float3 rayStart = wheelTransform.Position;
                float3 rayDirection = -math.up();
                float rayDistance = wheel.SuspensionLength + wheel.Radius;
                
                if (PhysicsWorld.CastRay(rayStart, rayDirection, rayDistance, out RaycastHit hit))
                {
                    wheel.IsGrounded = true;
                    wheel.GroundPoint = hit.Position;
                    wheel.GroundNormal = hit.SurfaceNormal;
                    wheel.GroundDistance = hit.Distance;
                    
                    // Определяем тип поверхности и погоды
                    SurfaceType surfaceType = DetermineSurfaceType(hit);
                    WeatherType weatherType = DetermineWeatherType();
                    
                    SurfaceData surface = SurfaceData[(int)surfaceType];
                    WeatherData weather = WeatherData[(int)weatherType];
                    
                    // Обновляем данные шины
                    UpdateTireData(ref tire, surface, weather, wheel, vehiclePhysics, DeltaTime);
                    
                    // Вычисляем взаимодействие с поверхностью
                    CalculateSurfaceInteraction(ref tire, surface, weather, wheel, vehiclePhysics, DeltaTime);
                    
                    // Обновляем физические свойства шины
                    UpdateTirePhysics(ref tire, surface, weather, wheel, vehiclePhysics, DeltaTime);
                    
                    // Обрабатываем износ и старение
                    ProcessTireWearAndAging(ref tire, surface, weather, wheel, vehiclePhysics, DeltaTime);
                }
                else
                {
                    // Шина не касается земли
                    ResetTirePhysics(ref tire);
                }
                
                tire.LastUpdateTime = SystemAPI.Time.ElapsedTime;
            }
            
            /// <summary>
            /// Определяет тип поверхности по hit данным
            /// </summary>
            private SurfaceType DetermineSurfaceType(RaycastHit hit)
            {
                // В реальной реализации здесь будет определение по материалам
                // Пока что используем случайный тип для демонстрации
                return (SurfaceType)((int)(SystemAPI.Time.ElapsedTime * 0.1f) % 11);
            }
            
            /// <summary>
            /// Определяет тип погоды
            /// </summary>
            private WeatherType DetermineWeatherType()
            {
                // В реальной реализации здесь будет получение данных о погоде
                // Пока что используем случайный тип для демонстрации
                return (WeatherType)((int)(SystemAPI.Time.ElapsedTime * 0.05f) % 10);
            }
            
            /// <summary>
            /// Обновляет данные шины
            /// </summary>
            private void UpdateTireData(ref TireData tire,
                                     SurfaceData surface,
                                     WeatherData weather,
                                     WheelData wheel,
                                     VehiclePhysics vehiclePhysics,
                                     float deltaTime)
            {
                // Обновляем температуру шины
                UpdateTireTemperature(ref tire, surface, weather, wheel, vehiclePhysics, deltaTime);
                
                // Обновляем давление в шине
                UpdateTirePressure(ref tire, surface, weather, deltaTime);
                
                // Обновляем площадь контакта
                UpdateContactArea(ref tire, surface, weather, wheel, vehiclePhysics);
                
                // Обновляем влажность шины
                UpdateTireMoisture(ref tire, surface, weather, deltaTime);
                
                // Обновляем время контакта
                if (tire.LastSurfaceType == (int)surface.SurfaceType)
                {
                    tire.ContactTime += deltaTime;
                }
                else
                {
                    tire.ContactTime = 0f;
                    tire.LastSurfaceType = (int)surface.SurfaceType;
                }
                
                if (tire.LastWeatherType == (int)weather.Type)
                {
                    // Погода не изменилась
                }
                else
                {
                    tire.LastWeatherType = (int)weather.Type;
                }
            }
            
            /// <summary>
            /// Обновляет температуру шины
            /// </summary>
            private void UpdateTireTemperature(ref TireData tire,
                                            SurfaceData surface,
                                            WeatherData weather,
                                            WheelData wheel,
                                            VehiclePhysics vehiclePhysics,
                                            float deltaTime)
            {
                // Нагрев от трения
                float frictionHeating = CalculateFrictionHeating(tire, surface, wheel, vehiclePhysics);
                
                // Нагрев от сжатия
                float compressionHeating = CalculateCompressionHeating(tire, surface, wheel, vehiclePhysics);
                
                // Нагрев от деформации
                float deformationHeating = CalculateDeformationHeating(tire, surface, wheel, vehiclePhysics);
                
                // Общий нагрев
                float totalHeating = (frictionHeating + compressionHeating + deformationHeating) * deltaTime;
                tire.Temperature += totalHeating;
                
                // Охлаждение
                float cooling = CalculateCooling(tire, surface, weather, deltaTime);
                tire.Temperature -= cooling * deltaTime;
                
                // Ограничиваем температуру
                tire.Temperature = math.clamp(tire.Temperature, -50f, tire.MaxTemperature);
            }
            
            /// <summary>
            /// Вычисляет нагрев от трения
            /// </summary>
            private float CalculateFrictionHeating(TireData tire, SurfaceData surface, WheelData wheel, VehiclePhysics vehiclePhysics)
            {
                float slipRatio = math.abs(wheel.AngularVelocity * wheel.Radius - math.length(vehiclePhysics.Velocity)) / math.max(wheel.AngularVelocity * wheel.Radius, 0.1f);
                float frictionForce = surface.FrictionCoefficient * vehiclePhysics.Mass * 9.81f;
                float heatingRate = slipRatio * frictionForce * 0.001f;
                
                return heatingRate * tire.HeatingRate;
            }
            
            /// <summary>
            /// Вычисляет нагрев от сжатия
            /// </summary>
            private float CalculateCompressionHeating(TireData tire, SurfaceData surface, WheelData wheel, VehiclePhysics vehiclePhysics)
            {
                float compressionRatio = tire.CurrentPressure / tire.RecommendedPressure;
                float compressionHeating = math.abs(compressionRatio - 1f) * 0.1f;
                
                return compressionHeating * tire.HeatingRate;
            }
            
            /// <summary>
            /// Вычисляет нагрев от деформации
            /// </summary>
            private float CalculateDeformationHeating(TireData tire, SurfaceData surface, WheelData wheel, VehiclePhysics vehiclePhysics)
            {
                float deformation = surface.PenetrationDepth * wheel.Radius;
                float deformationHeating = deformation * 0.05f;
                
                return deformationHeating * tire.HeatingRate;
            }
            
            /// <summary>
            /// Вычисляет охлаждение шины
            /// </summary>
            private float CalculateCooling(TireData tire, SurfaceData surface, WeatherData weather, float deltaTime)
            {
                float ambientTemperature = weather.Temperature;
                float temperatureDifference = tire.Temperature - ambientTemperature;
                float coolingRate = tire.CoolingRate * temperatureDifference;
                
                // Влияние ветра
                float windEffect = 1f + weather.WindSpeed * 0.1f;
                coolingRate *= windEffect;
                
                // Влияние влажности
                float humidityEffect = 1f + weather.Humidity * 0.2f;
                coolingRate *= humidityEffect;
                
                return coolingRate;
            }
            
            /// <summary>
            /// Обновляет давление в шине
            /// </summary>
            private void UpdateTirePressure(ref TireData tire, SurfaceData surface, WeatherData weather, float deltaTime)
            {
                // Влияние температуры на давление
                float temperatureEffect = (tire.Temperature - 20f) * 0.1f; // 0.1 кПа на градус
                float pressureChange = temperatureEffect * deltaTime;
                
                // Влияние атмосферного давления
                float atmosphericEffect = (weather.AtmosphericPressure - 101.3f) * 0.1f;
                pressureChange += atmosphericEffect * deltaTime;
                
                // Обновляем давление
                tire.CurrentPressure += pressureChange;
                
                // Ограничиваем давление
                tire.CurrentPressure = math.clamp(tire.CurrentPressure, tire.MinPressure, tire.MaxPressure);
            }
            
            /// <summary>
            /// Обновляет площадь контакта
            /// </summary>
            private void UpdateContactArea(ref TireData tire, SurfaceData surface, WeatherData weather, WheelData wheel, VehiclePhysics vehiclePhysics)
            {
                // Базовая площадь контакта
                float baseArea = math.PI * wheel.Radius * wheel.Radius * 0.1f;
                
                // Влияние давления в шине
                float pressureEffect = tire.CurrentPressure / tire.RecommendedPressure;
                baseArea /= pressureEffect;
                
                // Влияние нагрузки
                float loadEffect = vehiclePhysics.Mass / 1000f; // Нормализация
                baseArea *= loadEffect;
                
                // Влияние типа поверхности
                float surfaceEffect = 1f - surface.PenetrationDepth * 0.5f;
                baseArea *= surfaceEffect;
                
                // Влияние погоды
                float weatherEffect = WeatherProperties.GetTireWeatherEffect(weather, tire);
                baseArea *= weatherEffect;
                
                tire.ContactArea = math.clamp(baseArea, 0.001f, 0.1f);
            }
            
            /// <summary>
            /// Обновляет влажность шины
            /// </summary>
            private void UpdateTireMoisture(ref TireData tire, SurfaceData surface, WeatherData weather, float deltaTime)
            {
                // Накопление влажности от погоды
                float weatherMoisture = weather.Humidity * weather.RainIntensity * 0.1f;
                tire.Moisture += weatherMoisture * deltaTime;
                
                // Накопление влажности от поверхности
                float surfaceMoisture = surface.Moisture * 0.05f;
                tire.Moisture += surfaceMoisture * deltaTime;
                
                // Высыхание
                float drying = tire.DryingRate * tire.Moisture * deltaTime;
                tire.Moisture -= drying;
                
                // Ограничиваем влажность
                tire.Moisture = math.clamp(tire.Moisture, 0f, 1f);
            }
            
            /// <summary>
            /// Вычисляет взаимодействие с поверхностью
            /// </summary>
            private void CalculateSurfaceInteraction(ref TireData tire,
                                                   SurfaceData surface,
                                                   WeatherData weather,
                                                   WheelData wheel,
                                                   VehiclePhysics vehiclePhysics,
                                                   float deltaTime)
            {
                // Вычисляем коэффициент трения
                tire.FrictionCoefficient = CalculateFrictionCoefficient(tire, surface, weather);
                
                // Вычисляем коэффициент сцепления
                tire.TractionCoefficient = CalculateTractionCoefficient(tire, surface, weather);
                
                // Вычисляем сопротивление качению
                tire.RollingResistance = CalculateRollingResistance(tire, surface, weather, wheel, vehiclePhysics);
                
                // Обрабатываем грязь на шине
                ProcessMudOnTire(ref tire, surface, weather, deltaTime);
            }
            
            /// <summary>
            /// Вычисляет коэффициент трения
            /// </summary>
            private float CalculateFrictionCoefficient(TireData tire, SurfaceData surface, WeatherData weather)
            {
                float baseFriction = surface.FrictionCoefficient;
                
                // Влияние типа шины
                float tireTypeEffect = GetTireTypeEffect(tire.Type, surface.SurfaceType);
                
                // Влияние износа протектора
                float wearEffect = 1f - tire.TreadWear * 0.5f;
                
                // Влияние давления в шине
                float pressureEffect = math.clamp(tire.CurrentPressure / tire.RecommendedPressure, 0.7f, 1.3f);
                
                // Влияние температуры
                float temperatureEffect = math.clamp(tire.Temperature / 100f, 0.5f, 1.5f);
                
                // Влияние влажности
                float moistureEffect = 1f - tire.Moisture * 0.3f;
                
                // Влияние погоды
                float weatherEffect = WeatherProperties.GetTireWeatherEffect(weather, tire);
                
                return baseFriction * tireTypeEffect * wearEffect * pressureEffect * temperatureEffect * moistureEffect * weatherEffect;
            }
            
            /// <summary>
            /// Вычисляет коэффициент сцепления
            /// </summary>
            private float CalculateTractionCoefficient(TireData tire, SurfaceData surface, WeatherData weather)
            {
                float baseTraction = surface.TractionCoefficient;
                
                // Влияние типа шины
                float tireTypeEffect = GetTireTypeEffect(tire.Type, surface.SurfaceType);
                
                // Влияние износа протектора
                float wearEffect = 1f - tire.TreadWear * 0.6f;
                
                // Влияние давления в шине
                float pressureEffect = math.clamp(tire.CurrentPressure / tire.RecommendedPressure, 0.6f, 1.4f);
                
                // Влияние температуры
                float temperatureEffect = math.clamp(tire.Temperature / 100f, 0.4f, 1.6f);
                
                // Влияние влажности
                float moistureEffect = 1f - tire.Moisture * 0.4f;
                
                // Влияние погоды
                float weatherEffect = WeatherProperties.GetTireWeatherEffect(weather, tire);
                
                return baseTraction * tireTypeEffect * wearEffect * pressureEffect * temperatureEffect * moistureEffect * weatherEffect;
            }
            
            /// <summary>
            /// Вычисляет сопротивление качению
            /// </summary>
            private float CalculateRollingResistance(TireData tire, SurfaceData surface, WeatherData weather, WheelData wheel, VehiclePhysics vehiclePhysics)
            {
                float baseResistance = surface.RollingResistance;
                
                // Влияние типа шины
                float tireTypeEffect = GetTireTypeResistanceEffect(tire.Type, surface.SurfaceType);
                
                // Влияние давления в шине
                float pressureEffect = math.clamp(tire.CurrentPressure / tire.RecommendedPressure, 0.8f, 1.2f);
                
                // Влияние скорости
                float speedEffect = 1f + math.length(vehiclePhysics.Velocity) * 0.01f;
                
                // Влияние износа протектора
                float wearEffect = 1f + tire.TreadWear * 0.3f;
                
                // Влияние температуры
                float temperatureEffect = math.clamp(tire.Temperature / 100f, 0.8f, 1.2f);
                
                // Влияние погоды
                float weatherEffect = WeatherProperties.GetTireWeatherEffect(weather, tire);
                
                return baseResistance * tireTypeEffect * pressureEffect * speedEffect * wearEffect * temperatureEffect * weatherEffect;
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
            
            /// <summary>
            /// Обрабатывает грязь на шине
            /// </summary>
            private void ProcessMudOnTire(ref TireData tire, SurfaceData surface, WeatherData weather, float deltaTime)
            {
                // Накопление грязи
                if (surface.SurfaceType == SurfaceType.Mud || surface.SurfaceType == SurfaceType.Swamp)
                {
                    float mudAccumulation = surface.Viscosity * surface.Moisture * 0.1f;
                    tire.MudMass += mudAccumulation * deltaTime;
                    tire.MudParticleCount += (int)(mudAccumulation * 100f * deltaTime);
                }
                
                // Очистка от грязи
                float cleaning = tire.CleaningRate * tire.MudMass * deltaTime;
                tire.MudMass -= cleaning;
                tire.MudParticleCount = (int)(tire.MudMass * 100f);
                
                // Ограничиваем количество грязи
                tire.MudMass = math.clamp(tire.MudMass, 0f, 5f);
                tire.MudParticleCount = math.clamp(tire.MudParticleCount, 0, 500);
            }
            
            /// <summary>
            /// Обновляет физические свойства шины
            /// </summary>
            private void UpdateTirePhysics(ref TireData tire,
                                        SurfaceData surface,
                                        WeatherData weather,
                                        WheelData wheel,
                                        VehiclePhysics vehiclePhysics,
                                        float deltaTime)
            {
                // Обновляем жесткость шины
                UpdateTireStiffness(ref tire, surface, weather);
                
                // Обновляем демпфирование шины
                UpdateTireDamping(ref tire, surface, weather);
                
                // Обновляем эластичность шины
                UpdateTireElasticity(ref tire, surface, weather);
            }
            
            /// <summary>
            /// Обновляет жесткость шины
            /// </summary>
            private void UpdateTireStiffness(ref TireData tire, SurfaceData surface, WeatherData weather)
            {
                float baseStiffness = 1000f; // Базовая жесткость
                
                // Влияние давления в шине
                float pressureEffect = tire.CurrentPressure / tire.RecommendedPressure;
                baseStiffness *= pressureEffect;
                
                // Влияние температуры
                float temperatureEffect = math.clamp(tire.Temperature / 100f, 0.5f, 2f);
                baseStiffness *= temperatureEffect;
                
                // Влияние износа
                float wearEffect = 1f - tire.TreadWear * 0.3f;
                baseStiffness *= wearEffect;
                
                tire.Stiffness = baseStiffness;
            }
            
            /// <summary>
            /// Обновляет демпфирование шины
            /// </summary>
            private void UpdateTireDamping(ref TireData tire, SurfaceData surface, WeatherData weather)
            {
                float baseDamping = 100f; // Базовое демпфирование
                
                // Влияние температуры
                float temperatureEffect = math.clamp(tire.Temperature / 100f, 0.7f, 1.5f);
                baseDamping *= temperatureEffect;
                
                // Влияние влажности
                float moistureEffect = 1f + tire.Moisture * 0.2f;
                baseDamping *= moistureEffect;
                
                tire.Damping = baseDamping;
            }
            
            /// <summary>
            /// Обновляет эластичность шины
            /// </summary>
            private void UpdateTireElasticity(ref TireData tire, SurfaceData surface, WeatherData weather)
            {
                float baseElasticity = 0.8f; // Базовая эластичность
                
                // Влияние температуры
                float temperatureEffect = math.clamp(tire.Temperature / 100f, 0.6f, 1.4f);
                baseElasticity *= temperatureEffect;
                
                // Влияние износа
                float wearEffect = 1f - tire.TreadWear * 0.4f;
                baseElasticity *= wearEffect;
                
                tire.Elasticity = baseElasticity;
            }
            
            /// <summary>
            /// Обрабатывает износ и старение шины
            /// </summary>
            private void ProcessTireWearAndAging(ref TireData tire,
                                              SurfaceData surface,
                                              WeatherData weather,
                                              WheelData wheel,
                                              VehiclePhysics vehiclePhysics,
                                              float deltaTime)
            {
                // Износ от трения
                float frictionWear = CalculateFrictionWear(tire, surface, wheel, vehiclePhysics);
                
                // Износ от температуры
                float temperatureWear = CalculateTemperatureWear(tire, weather);
                
                // Износ от возраста
                float ageWear = CalculateAgeWear(tire, deltaTime);
                
                // Общий износ
                float totalWear = (frictionWear + temperatureWear + ageWear) * deltaTime;
                tire.TreadWear += totalWear;
                tire.TreadWear = math.clamp(tire.TreadWear, 0f, 1f);
                
                // Обновляем глубину протектора
                tire.TreadDepth = tire.NewTreadDepth * (1f - tire.TreadWear);
                
                // Обновляем возраст
                tire.Age += deltaTime / 86400f; // Конвертируем секунды в дни
                
                // Обновляем пробег
                float distance = math.length(vehiclePhysics.Velocity) * deltaTime / 1000f; // Конвертируем в км
                tire.Mileage += distance;
                
                // Обновляем состояние шины
                UpdateTireCondition(ref tire);
            }
            
            /// <summary>
            /// Вычисляет износ от трения
            /// </summary>
            private float CalculateFrictionWear(TireData tire, SurfaceData surface, WheelData wheel, VehiclePhysics vehiclePhysics)
            {
                float slipRatio = math.abs(wheel.AngularVelocity * wheel.Radius - math.length(vehiclePhysics.Velocity)) / math.max(wheel.AngularVelocity * wheel.Radius, 0.1f);
                float frictionForce = surface.FrictionCoefficient * vehiclePhysics.Mass * 9.81f;
                float wearRate = slipRatio * frictionForce * 0.0001f;
                
                return wearRate * tire.WearRate;
            }
            
            /// <summary>
            /// Вычисляет износ от температуры
            /// </summary>
            private float CalculateTemperatureWear(TireData tire, WeatherData weather)
            {
                float temperatureDifference = math.abs(tire.Temperature - weather.Temperature);
                float wearRate = temperatureDifference * 0.001f;
                
                return wearRate * tire.WearRate;
            }
            
            /// <summary>
            /// Вычисляет износ от возраста
            /// </summary>
            private float CalculateAgeWear(TireData tire, float deltaTime)
            {
                float ageFactor = tire.Age / tire.MaxAge;
                float wearRate = ageFactor * 0.0001f;
                
                return wearRate * tire.WearRate;
            }
            
            /// <summary>
            /// Обновляет состояние шины
            /// </summary>
            private void UpdateTireCondition(ref TireData tire)
            {
                if (tire.TreadWear >= 1f || tire.Age >= tire.MaxAge || tire.Mileage >= tire.MaxMileage)
                {
                    tire.Condition = TireCondition.Worn;
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
            
            /// <summary>
            /// Сбрасывает данные физики шины
            /// </summary>
            private void ResetTirePhysics(ref TireData tire)
            {
                tire.FrictionCoefficient = 0f;
                tire.TractionCoefficient = 0f;
                tire.RollingResistance = 0f;
                tire.ContactArea = 0f;
                tire.ContactTime = 0f;
            }
        }
    }
}