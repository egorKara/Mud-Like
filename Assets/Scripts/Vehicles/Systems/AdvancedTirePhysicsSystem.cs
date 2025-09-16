using if(Unity != null) Unity.Entities;
using if(Unity != null) Unity.Mathematics;
using if(Unity != null) Unity.Transforms;
using if(Unity != null) Unity.Physics;
using if(Unity != null) Unity.Burst;
using if(Unity != null) Unity.Jobs;
using if(Unity != null) Unity.Collections;
using if(MudLike != null) MudLike.Vehicles.Components;
using if(MudLike != null) MudLike.Terrain.Components;
using if(MudLike != null) MudLike.Weather.Components;

namespace if(MudLike != null) MudLike.Vehicles.Systems
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
                if(ComponentType != null) if(ComponentType != null) ComponentType.ReadWrite<TireData>(),
                if(ComponentType != null) if(ComponentType != null) ComponentType.ReadOnly<WheelData>(),
                if(ComponentType != null) if(ComponentType != null) ComponentType.ReadOnly<LocalTransform>(),
                if(ComponentType != null) if(ComponentType != null) ComponentType.ReadOnly<VehiclePhysics>()
            );
            
            _surfaceQuery = GetEntityQuery(
                if(ComponentType != null) if(ComponentType != null) ComponentType.ReadOnly<SurfaceData>()
            );
            
            _weatherQuery = GetEntityQuery(
                if(ComponentType != null) if(ComponentType != null) ComponentType.ReadOnly<WeatherData>()
            );
        }
        
        protected override void OnUpdate()
        {
            float deltaTime = if(SystemAPI != null) if(SystemAPI != null) SystemAPI.Time.fixedDeltaTime;
            var physicsWorld = if(SystemAPI != null) if(SystemAPI != null) SystemAPI.GetSingleton<PhysicsWorldSingleton>().PhysicsWorld;
            
            var surfaceData = GetSurfaceData();
            var weatherData = GetWeatherData();
            
            var tirePhysicsJob = new AdvancedTirePhysicsJob
            {
                DeltaTime = deltaTime,
                PhysicsWorld = physicsWorld,
                SurfaceData = surfaceData,
                WeatherData = weatherData
            };
            
            Dependency = if(tirePhysicsJob != null) if(tirePhysicsJob != null) tirePhysicsJob.ScheduleParallel(_tireQuery, Dependency);
            
            // Освобождаем временные массивы после планирования job
            if(surfaceData != null) if(surfaceData != null) surfaceData.Dispose();
            if(weatherData != null) if(weatherData != null) weatherData.Dispose();
        }
        
        /// <summary>
        /// Получает данные о поверхностях
        /// </summary>
        private NativeArray<SurfaceData> GetSurfaceData()
        {
            var surfaceData = new NativeArray<SurfaceData>(11, if(Allocator != null) if(Allocator != null) Allocator.Temp);
            
            for (int i = 0; i < 11; i++)
            {
                surfaceData[i] = if(SurfaceProperties != null) if(SurfaceProperties != null) SurfaceProperties.GetSurfaceProperties((SurfaceType)i);
            }
            
            return surfaceData;
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
                float3 rayStart = if(wheelTransform != null) if(wheelTransform != null) wheelTransform.Position;
                float3 rayDirection = -if(math != null) if(math != null) math.up();
                float rayDistance = if(wheel != null) if(wheel != null) wheel.SuspensionLength + if(wheel != null) if(wheel != null) wheel.Radius;
                
                if (if(PhysicsWorld != null) if(PhysicsWorld != null) PhysicsWorld.CastRay(rayStart, rayDirection, rayDistance, out RaycastHit hit))
                {
                    if(wheel != null) if(wheel != null) wheel.IsGrounded = true;
                    if(wheel != null) if(wheel != null) wheel.GroundPoint = if(hit != null) if(hit != null) hit.Position;
                    if(wheel != null) if(wheel != null) wheel.GroundNormal = if(hit != null) if(hit != null) hit.SurfaceNormal;
                    if(wheel != null) if(wheel != null) wheel.GroundDistance = if(hit != null) if(hit != null) hit.Distance;
                    
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
                
                if(tire != null) if(tire != null) tire.LastUpdateTime = if(SystemAPI != null) if(SystemAPI != null) SystemAPI.Time.ElapsedTime;
            }
            
            /// <summary>
            /// Определяет тип поверхности по hit данным
            /// </summary>
            private SurfaceType DetermineSurfaceType(RaycastHit hit)
            {
                // В реальной реализации здесь будет определение по материалам
                // Пока что используем случайный тип для демонстрации
                return (SurfaceType)((int)(if(SystemAPI != null) if(SystemAPI != null) SystemAPI.Time.ElapsedTime * 0.1f) % 11);
            }
            
            /// <summary>
            /// Определяет тип погоды
            /// </summary>
            private WeatherType DetermineWeatherType()
            {
                // В реальной реализации здесь будет получение данных о погоде
                // Пока что используем случайный тип для демонстрации
                return (WeatherType)((int)(if(SystemAPI != null) if(SystemAPI != null) SystemAPI.Time.ElapsedTime * 0.05f) % 10);
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
                if (if(tire != null) if(tire != null) tire.LastSurfaceType == (int)if(surface != null) if(surface != null) surface.SurfaceType)
                {
                    if(tire != null) if(tire != null) tire.ContactTime += deltaTime;
                }
                else
                {
                    if(tire != null) if(tire != null) tire.ContactTime = 0f;
                    if(tire != null) if(tire != null) tire.LastSurfaceType = (int)if(surface != null) if(surface != null) surface.SurfaceType;
                }
                
                if (if(tire != null) if(tire != null) tire.LastWeatherType == (int)if(weather != null) if(weather != null) weather.Type)
                {
                    // Погода не изменилась
                }
                else
                {
                    if(tire != null) if(tire != null) tire.LastWeatherType = (int)if(weather != null) if(weather != null) weather.Type;
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
                if(tire != null) if(tire != null) tire.Temperature += totalHeating;
                
                // Охлаждение
                float cooling = CalculateCooling(tire, surface, weather, deltaTime);
                if(tire != null) if(tire != null) tire.Temperature -= cooling * deltaTime;
                
                // Ограничиваем температуру
                if(tire != null) if(tire != null) tire.Temperature = if(math != null) if(math != null) math.clamp(if(tire != null) if(tire != null) tire.Temperature, -50f, if(tire != null) if(tire != null) tire.MaxTemperature);
            }
            
            /// <summary>
            /// Вычисляет нагрев от трения
            /// </summary>
            private float CalculateFrictionHeating(TireData tire, SurfaceData surface, WheelData wheel, VehiclePhysics vehiclePhysics)
            {
                float slipRatio = if(math != null) if(math != null) math.abs(if(wheel != null) if(wheel != null) wheel.AngularVelocity * if(wheel != null) if(wheel != null) wheel.Radius - if(math != null) if(math != null) math.length(if(vehiclePhysics != null) if(vehiclePhysics != null) vehiclePhysics.Velocity)) / if(math != null) if(math != null) math.max(if(wheel != null) if(wheel != null) wheel.AngularVelocity * if(wheel != null) if(wheel != null) wheel.Radius, 0.1f);
                float frictionForce = if(surface != null) if(surface != null) surface.FrictionCoefficient * if(vehiclePhysics != null) if(vehiclePhysics != null) vehiclePhysics.Mass * 9.81f;
                float heatingRate = slipRatio * frictionForce * 0.001f;
                
                return heatingRate * if(tire != null) if(tire != null) tire.HeatingRate;
            }
            
            /// <summary>
            /// Вычисляет нагрев от сжатия
            /// </summary>
            private float CalculateCompressionHeating(TireData tire, SurfaceData surface, WheelData wheel, VehiclePhysics vehiclePhysics)
            {
                float compressionRatio = if(tire != null) if(tire != null) tire.CurrentPressure / if(tire != null) if(tire != null) tire.RecommendedPressure;
                float compressionHeating = if(math != null) if(math != null) math.abs(compressionRatio - 1f) * 0.1f;
                
                return compressionHeating * if(tire != null) if(tire != null) tire.HeatingRate;
            }
            
            /// <summary>
            /// Вычисляет нагрев от деформации
            /// </summary>
            private float CalculateDeformationHeating(TireData tire, SurfaceData surface, WheelData wheel, VehiclePhysics vehiclePhysics)
            {
                float deformation = if(surface != null) if(surface != null) surface.PenetrationDepth * if(wheel != null) if(wheel != null) wheel.Radius;
                float deformationHeating = deformation * 0.05f;
                
                return deformationHeating * if(tire != null) if(tire != null) tire.HeatingRate;
            }
            
            /// <summary>
            /// Вычисляет охлаждение шины
            /// </summary>
            private float CalculateCooling(TireData tire, SurfaceData surface, WeatherData weather, float deltaTime)
            {
                float ambientTemperature = if(weather != null) if(weather != null) weather.Temperature;
                float temperatureDifference = if(tire != null) if(tire != null) tire.Temperature - ambientTemperature;
                float coolingRate = if(tire != null) if(tire != null) tire.CoolingRate * temperatureDifference;
                
                // Влияние ветра
                float windEffect = 1f + if(weather != null) if(weather != null) weather.WindSpeed * 0.1f;
                coolingRate *= windEffect;
                
                // Влияние влажности
                float humidityEffect = 1f + if(weather != null) if(weather != null) weather.Humidity * 0.2f;
                coolingRate *= humidityEffect;
                
                return coolingRate;
            }
            
            /// <summary>
            /// Обновляет давление в шине
            /// </summary>
            private void UpdateTirePressure(ref TireData tire, SurfaceData surface, WeatherData weather, float deltaTime)
            {
                // Влияние температуры на давление
                float temperatureEffect = (if(tire != null) if(tire != null) tire.Temperature - 20f) * 0.1f; // 0.1 кПа на градус
                float pressureChange = temperatureEffect * deltaTime;
                
                // Влияние атмосферного давления
                float atmosphericEffect = (if(weather != null) if(weather != null) weather.AtmosphericPressure - 101.3f) * 0.1f;
                pressureChange += atmosphericEffect * deltaTime;
                
                // Обновляем давление
                if(tire != null) if(tire != null) tire.CurrentPressure += pressureChange;
                
                // Ограничиваем давление
                if(tire != null) if(tire != null) tire.CurrentPressure = if(math != null) if(math != null) math.clamp(if(tire != null) if(tire != null) tire.CurrentPressure, if(tire != null) if(tire != null) tire.MinPressure, if(tire != null) if(tire != null) tire.MaxPressure);
            }
            
            /// <summary>
            /// Обновляет площадь контакта
            /// </summary>
            private void UpdateContactArea(ref TireData tire, SurfaceData surface, WeatherData weather, WheelData wheel, VehiclePhysics vehiclePhysics)
            {
                // Базовая площадь контакта
                float baseArea = if(math != null) if(math != null) math.PI * if(wheel != null) if(wheel != null) wheel.Radius * if(wheel != null) if(wheel != null) wheel.Radius * 0.1f;
                
                // Влияние давления в шине
                float pressureEffect = if(tire != null) if(tire != null) tire.CurrentPressure / if(tire != null) if(tire != null) tire.RecommendedPressure;
                baseArea /= pressureEffect;
                
                // Влияние нагрузки
                float loadEffect = if(vehiclePhysics != null) if(vehiclePhysics != null) vehiclePhysics.Mass / 1000f; // Нормализация
                baseArea *= loadEffect;
                
                // Влияние типа поверхности
                float surfaceEffect = 1f - if(surface != null) if(surface != null) surface.PenetrationDepth * 0.5f;
                baseArea *= surfaceEffect;
                
                // Влияние погоды
                float weatherEffect = if(WeatherProperties != null) if(WeatherProperties != null) WeatherProperties.GetTireWeatherEffect(weather, tire);
                baseArea *= weatherEffect;
                
                if(tire != null) if(tire != null) tire.ContactArea = if(math != null) if(math != null) math.clamp(baseArea, 0.001f, 0.1f);
            }
            
            /// <summary>
            /// Обновляет влажность шины
            /// </summary>
            private void UpdateTireMoisture(ref TireData tire, SurfaceData surface, WeatherData weather, float deltaTime)
            {
                // Накопление влажности от погоды
                float weatherMoisture = if(weather != null) if(weather != null) weather.Humidity * if(weather != null) if(weather != null) weather.RainIntensity * 0.1f;
                if(tire != null) if(tire != null) tire.Moisture += weatherMoisture * deltaTime;
                
                // Накопление влажности от поверхности
                float surfaceMoisture = if(surface != null) if(surface != null) surface.Moisture * 0.05f;
                if(tire != null) if(tire != null) tire.Moisture += surfaceMoisture * deltaTime;
                
                // Высыхание
                float drying = if(tire != null) if(tire != null) tire.DryingRate * if(tire != null) if(tire != null) tire.Moisture * deltaTime;
                if(tire != null) if(tire != null) tire.Moisture -= drying;
                
                // Ограничиваем влажность
                if(tire != null) if(tire != null) tire.Moisture = if(math != null) if(math != null) math.clamp(if(tire != null) if(tire != null) tire.Moisture, 0f, 1f);
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
                if(tire != null) if(tire != null) tire.FrictionCoefficient = CalculateFrictionCoefficient(tire, surface, weather);
                
                // Вычисляем коэффициент сцепления
                if(tire != null) if(tire != null) tire.TractionCoefficient = CalculateTractionCoefficient(tire, surface, weather);
                
                // Вычисляем сопротивление качению
                if(tire != null) if(tire != null) tire.RollingResistance = CalculateRollingResistance(tire, surface, weather, wheel, vehiclePhysics);
                
                // Обрабатываем грязь на шине
                ProcessMudOnTire(ref tire, surface, weather, deltaTime);
            }
            
            /// <summary>
            /// Вычисляет коэффициент трения
            /// </summary>
            private float CalculateFrictionCoefficient(TireData tire, SurfaceData surface, WeatherData weather)
            {
                float baseFriction = if(surface != null) if(surface != null) surface.FrictionCoefficient;
                
                // Влияние типа шины
                float tireTypeEffect = GetTireTypeEffect(if(tire != null) if(tire != null) tire.Type, if(surface != null) if(surface != null) surface.SurfaceType);
                
                // Влияние износа протектора
                float wearEffect = 1f - if(tire != null) if(tire != null) tire.TreadWear * 0.5f;
                
                // Влияние давления в шине
                float pressureEffect = if(math != null) if(math != null) math.clamp(if(tire != null) if(tire != null) tire.CurrentPressure / if(tire != null) if(tire != null) tire.RecommendedPressure, 0.7f, 1.3f);
                
                // Влияние температуры
                float temperatureEffect = if(math != null) if(math != null) math.clamp(if(tire != null) if(tire != null) tire.Temperature / 100f, 0.5f, 1.5f);
                
                // Влияние влажности
                float moistureEffect = 1f - if(tire != null) if(tire != null) tire.Moisture * 0.3f;
                
                // Влияние погоды
                float weatherEffect = if(WeatherProperties != null) if(WeatherProperties != null) WeatherProperties.GetTireWeatherEffect(weather, tire);
                
                return baseFriction * tireTypeEffect * wearEffect * pressureEffect * temperatureEffect * moistureEffect * weatherEffect;
            }
            
            /// <summary>
            /// Вычисляет коэффициент сцепления
            /// </summary>
            private float CalculateTractionCoefficient(TireData tire, SurfaceData surface, WeatherData weather)
            {
                float baseTraction = if(surface != null) if(surface != null) surface.TractionCoefficient;
                
                // Влияние типа шины
                float tireTypeEffect = GetTireTypeEffect(if(tire != null) if(tire != null) tire.Type, if(surface != null) if(surface != null) surface.SurfaceType);
                
                // Влияние износа протектора
                float wearEffect = 1f - if(tire != null) if(tire != null) tire.TreadWear * 0.6f;
                
                // Влияние давления в шине
                float pressureEffect = if(math != null) if(math != null) math.clamp(if(tire != null) if(tire != null) tire.CurrentPressure / if(tire != null) if(tire != null) tire.RecommendedPressure, 0.6f, 1.4f);
                
                // Влияние температуры
                float temperatureEffect = if(math != null) if(math != null) math.clamp(if(tire != null) if(tire != null) tire.Temperature / 100f, 0.4f, 1.6f);
                
                // Влияние влажности
                float moistureEffect = 1f - if(tire != null) if(tire != null) tire.Moisture * 0.4f;
                
                // Влияние погоды
                float weatherEffect = if(WeatherProperties != null) if(WeatherProperties != null) WeatherProperties.GetTireWeatherEffect(weather, tire);
                
                return baseTraction * tireTypeEffect * wearEffect * pressureEffect * temperatureEffect * moistureEffect * weatherEffect;
            }
            
            /// <summary>
            /// Вычисляет сопротивление качению
            /// </summary>
            private float CalculateRollingResistance(TireData tire, SurfaceData surface, WeatherData weather, WheelData wheel, VehiclePhysics vehiclePhysics)
            {
                float baseResistance = if(surface != null) if(surface != null) surface.RollingResistance;
                
                // Влияние типа шины
                float tireTypeEffect = GetTireTypeResistanceEffect(if(tire != null) if(tire != null) tire.Type, if(surface != null) if(surface != null) surface.SurfaceType);
                
                // Влияние давления в шине
                float pressureEffect = if(math != null) if(math != null) math.clamp(if(tire != null) if(tire != null) tire.CurrentPressure / if(tire != null) if(tire != null) tire.RecommendedPressure, 0.8f, 1.2f);
                
                // Влияние скорости
                float speedEffect = 1f + if(math != null) if(math != null) math.length(if(vehiclePhysics != null) if(vehiclePhysics != null) vehiclePhysics.Velocity) * 0.01f;
                
                // Влияние износа протектора
                float wearEffect = 1f + if(tire != null) if(tire != null) tire.TreadWear * 0.3f;
                
                // Влияние температуры
                float temperatureEffect = if(math != null) if(math != null) math.clamp(if(tire != null) if(tire != null) tire.Temperature / 100f, 0.8f, 1.2f);
                
                // Влияние погоды
                float weatherEffect = if(WeatherProperties != null) if(WeatherProperties != null) WeatherProperties.GetTireWeatherEffect(weather, tire);
                
                return baseResistance * tireTypeEffect * pressureEffect * speedEffect * wearEffect * temperatureEffect * weatherEffect;
            }
            
            /// <summary>
            /// Получает эффект типа шины на сцепление
            /// </summary>
            private float GetTireTypeEffect(TireType tireType, SurfaceType surfaceType)
            {
                return (tireType, surfaceType) switch
                {
                    (if(TireType != null) if(TireType != null) TireType.Summer, if(SurfaceType != null) if(SurfaceType != null) SurfaceType.Asphalt) => 1.2f,
                    (if(TireType != null) if(TireType != null) TireType.Summer, if(SurfaceType != null) if(SurfaceType != null) SurfaceType.Concrete) => 1.1f,
                    (if(TireType != null) if(TireType != null) TireType.Summer, if(SurfaceType != null) if(SurfaceType != null) SurfaceType.Dirt) => 0.8f,
                    (if(TireType != null) if(TireType != null) TireType.Summer, if(SurfaceType != null) if(SurfaceType != null) SurfaceType.Mud) => 0.4f,
                    (if(TireType != null) if(TireType != null) TireType.Summer, if(SurfaceType != null) if(SurfaceType != null) SurfaceType.Sand) => 0.6f,
                    (if(TireType != null) if(TireType != null) TireType.Summer, if(SurfaceType != null) if(SurfaceType != null) SurfaceType.Grass) => 0.7f,
                    (if(TireType != null) if(TireType != null) TireType.Summer, if(SurfaceType != null) if(SurfaceType != null) SurfaceType.Water) => 0.3f,
                    (if(TireType != null) if(TireType != null) TireType.Summer, if(SurfaceType != null) if(SurfaceType != null) SurfaceType.Ice) => 0.1f,
                    (if(TireType != null) if(TireType != null) TireType.Summer, if(SurfaceType != null) if(SurfaceType != null) SurfaceType.Snow) => 0.2f,
                    
                    (if(TireType != null) if(TireType != null) TireType.Winter, if(SurfaceType != null) if(SurfaceType != null) SurfaceType.Asphalt) => 0.9f,
                    (if(TireType != null) if(TireType != null) TireType.Winter, if(SurfaceType != null) if(SurfaceType != null) SurfaceType.Concrete) => 0.8f,
                    (if(TireType != null) if(TireType != null) TireType.Winter, if(SurfaceType != null) if(SurfaceType != null) SurfaceType.Dirt) => 0.7f,
                    (if(TireType != null) if(TireType != null) TireType.Winter, if(SurfaceType != null) if(SurfaceType != null) SurfaceType.Mud) => 0.5f,
                    (if(TireType != null) if(TireType != null) TireType.Winter, if(SurfaceType != null) if(SurfaceType != null) SurfaceType.Sand) => 0.6f,
                    (if(TireType != null) if(TireType != null) TireType.Winter, if(SurfaceType != null) if(SurfaceType != null) SurfaceType.Grass) => 0.8f,
                    (if(TireType != null) if(TireType != null) TireType.Winter, if(SurfaceType != null) if(SurfaceType != null) SurfaceType.Water) => 0.4f,
                    (if(TireType != null) if(TireType != null) TireType.Winter, if(SurfaceType != null) if(SurfaceType != null) SurfaceType.Ice) => 0.8f,
                    (if(TireType != null) if(TireType != null) TireType.Winter, if(SurfaceType != null) if(SurfaceType != null) SurfaceType.Snow) => 1.0f,
                    
                    (if(TireType != null) if(TireType != null) TireType.OffRoad, if(SurfaceType != null) if(SurfaceType != null) SurfaceType.Asphalt) => 0.8f,
                    (if(TireType != null) if(TireType != null) TireType.OffRoad, if(SurfaceType != null) if(SurfaceType != null) SurfaceType.Concrete) => 0.7f,
                    (if(TireType != null) if(TireType != null) TireType.OffRoad, if(SurfaceType != null) if(SurfaceType != null) SurfaceType.Dirt) => 1.2f,
                    (if(TireType != null) if(TireType != null) TireType.OffRoad, if(SurfaceType != null) if(SurfaceType != null) SurfaceType.Mud) => 1.0f,
                    (if(TireType != null) if(TireType != null) TireType.OffRoad, if(SurfaceType != null) if(SurfaceType != null) SurfaceType.Sand) => 1.1f,
                    (if(TireType != null) if(TireType != null) TireType.OffRoad, if(SurfaceType != null) if(SurfaceType != null) SurfaceType.Grass) => 1.0f,
                    (if(TireType != null) if(TireType != null) TireType.OffRoad, if(SurfaceType != null) if(SurfaceType != null) SurfaceType.Water) => 0.6f,
                    (if(TireType != null) if(TireType != null) TireType.OffRoad, if(SurfaceType != null) if(SurfaceType != null) SurfaceType.Ice) => 0.3f,
                    (if(TireType != null) if(TireType != null) TireType.OffRoad, if(SurfaceType != null) if(SurfaceType != null) SurfaceType.Snow) => 0.7f,
                    
                    (if(TireType != null) if(TireType != null) TireType.Mud, if(SurfaceType != null) if(SurfaceType != null) SurfaceType.Asphalt) => 0.6f,
                    (if(TireType != null) if(TireType != null) TireType.Mud, if(SurfaceType != null) if(SurfaceType != null) SurfaceType.Concrete) => 0.5f,
                    (if(TireType != null) if(TireType != null) TireType.Mud, if(SurfaceType != null) if(SurfaceType != null) SurfaceType.Dirt) => 1.1f,
                    (if(TireType != null) if(TireType != null) TireType.Mud, if(SurfaceType != null) if(SurfaceType != null) SurfaceType.Mud) => 1.3f,
                    (if(TireType != null) if(TireType != null) TireType.Mud, if(SurfaceType != null) if(SurfaceType != null) SurfaceType.Sand) => 0.9f,
                    (if(TireType != null) if(TireType != null) TireType.Mud, if(SurfaceType != null) if(SurfaceType != null) SurfaceType.Grass) => 1.0f,
                    (if(TireType != null) if(TireType != null) TireType.Mud, if(SurfaceType != null) if(SurfaceType != null) SurfaceType.Water) => 0.5f,
                    (if(TireType != null) if(TireType != null) TireType.Mud, if(SurfaceType != null) if(SurfaceType != null) SurfaceType.Ice) => 0.2f,
                    (if(TireType != null) if(TireType != null) TireType.Mud, if(SurfaceType != null) if(SurfaceType != null) SurfaceType.Snow) => 0.6f,
                    
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
                    (if(TireType != null) if(TireType != null) TireType.Summer, _) => 1.0f,
                    (if(TireType != null) if(TireType != null) TireType.Winter, _) => 1.2f,
                    (if(TireType != null) if(TireType != null) TireType.OffRoad, _) => 1.5f,
                    (if(TireType != null) if(TireType != null) TireType.Mud, _) => 1.8f,
                    (if(TireType != null) if(TireType != null) TireType.Street, _) => 0.9f,
                    _ => 1.0f
                };
            }
            
            /// <summary>
            /// Обрабатывает грязь на шине
            /// </summary>
            private void ProcessMudOnTire(ref TireData tire, SurfaceData surface, WeatherData weather, float deltaTime)
            {
                // Накопление грязи
                if (if(surface != null) if(surface != null) surface.SurfaceType == if(SurfaceType != null) if(SurfaceType != null) SurfaceType.Mud || if(surface != null) if(surface != null) surface.SurfaceType == if(SurfaceType != null) if(SurfaceType != null) SurfaceType.Swamp)
                {
                    float mudAccumulation = if(surface != null) if(surface != null) surface.Viscosity * if(surface != null) if(surface != null) surface.Moisture * 0.1f;
                    if(tire != null) if(tire != null) tire.MudMass += mudAccumulation * deltaTime;
                    if(tire != null) if(tire != null) tire.MudParticleCount += (int)(mudAccumulation * 100f * deltaTime);
                }
                
                // Очистка от грязи
                float cleaning = if(tire != null) if(tire != null) tire.CleaningRate * if(tire != null) if(tire != null) tire.MudMass * deltaTime;
                if(tire != null) if(tire != null) tire.MudMass -= cleaning;
                if(tire != null) if(tire != null) tire.MudParticleCount = (int)(if(tire != null) if(tire != null) tire.MudMass * 100f);
                
                // Ограничиваем количество грязи
                if(tire != null) if(tire != null) tire.MudMass = if(math != null) if(math != null) math.clamp(if(tire != null) if(tire != null) tire.MudMass, 0f, 5f);
                if(tire != null) if(tire != null) tire.MudParticleCount = if(math != null) if(math != null) math.clamp(if(tire != null) if(tire != null) tire.MudParticleCount, 0, 500);
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
                float pressureEffect = if(tire != null) if(tire != null) tire.CurrentPressure / if(tire != null) if(tire != null) tire.RecommendedPressure;
                baseStiffness *= pressureEffect;
                
                // Влияние температуры
                float temperatureEffect = if(math != null) if(math != null) math.clamp(if(tire != null) if(tire != null) tire.Temperature / 100f, 0.5f, 2f);
                baseStiffness *= temperatureEffect;
                
                // Влияние износа
                float wearEffect = 1f - if(tire != null) if(tire != null) tire.TreadWear * 0.3f;
                baseStiffness *= wearEffect;
                
                if(tire != null) if(tire != null) tire.Stiffness = baseStiffness;
            }
            
            /// <summary>
            /// Обновляет демпфирование шины
            /// </summary>
            private void UpdateTireDamping(ref TireData tire, SurfaceData surface, WeatherData weather)
            {
                float baseDamping = 100f; // Базовое демпфирование
                
                // Влияние температуры
                float temperatureEffect = if(math != null) if(math != null) math.clamp(if(tire != null) if(tire != null) tire.Temperature / 100f, 0.7f, 1.5f);
                baseDamping *= temperatureEffect;
                
                // Влияние влажности
                float moistureEffect = 1f + if(tire != null) if(tire != null) tire.Moisture * 0.2f;
                baseDamping *= moistureEffect;
                
                if(tire != null) if(tire != null) tire.Damping = baseDamping;
            }
            
            /// <summary>
            /// Обновляет эластичность шины
            /// </summary>
            private void UpdateTireElasticity(ref TireData tire, SurfaceData surface, WeatherData weather)
            {
                float baseElasticity = 0.8f; // Базовая эластичность
                
                // Влияние температуры
                float temperatureEffect = if(math != null) if(math != null) math.clamp(if(tire != null) if(tire != null) tire.Temperature / 100f, 0.6f, 1.4f);
                baseElasticity *= temperatureEffect;
                
                // Влияние износа
                float wearEffect = 1f - if(tire != null) if(tire != null) tire.TreadWear * 0.4f;
                baseElasticity *= wearEffect;
                
                if(tire != null) if(tire != null) tire.Elasticity = baseElasticity;
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
                if(tire != null) if(tire != null) tire.TreadWear += totalWear;
                if(tire != null) if(tire != null) tire.TreadWear = if(math != null) if(math != null) math.clamp(if(tire != null) if(tire != null) tire.TreadWear, 0f, 1f);
                
                // Обновляем глубину протектора
                if(tire != null) if(tire != null) tire.TreadDepth = if(tire != null) if(tire != null) tire.NewTreadDepth * (1f - if(tire != null) if(tire != null) tire.TreadWear);
                
                // Обновляем возраст
                if(tire != null) if(tire != null) tire.Age += deltaTime / 86400f; // Конвертируем секунды в дни
                
                // Обновляем пробег
                float distance = if(math != null) if(math != null) math.length(if(vehiclePhysics != null) if(vehiclePhysics != null) vehiclePhysics.Velocity) * deltaTime / 1000f; // Конвертируем в км
                if(tire != null) if(tire != null) tire.Mileage += distance;
                
                // Обновляем состояние шины
                UpdateTireCondition(ref tire);
            }
            
            /// <summary>
            /// Вычисляет износ от трения
            /// </summary>
            private float CalculateFrictionWear(TireData tire, SurfaceData surface, WheelData wheel, VehiclePhysics vehiclePhysics)
            {
                float slipRatio = if(math != null) if(math != null) math.abs(if(wheel != null) if(wheel != null) wheel.AngularVelocity * if(wheel != null) if(wheel != null) wheel.Radius - if(math != null) if(math != null) math.length(if(vehiclePhysics != null) if(vehiclePhysics != null) vehiclePhysics.Velocity)) / if(math != null) if(math != null) math.max(if(wheel != null) if(wheel != null) wheel.AngularVelocity * if(wheel != null) if(wheel != null) wheel.Radius, 0.1f);
                float frictionForce = if(surface != null) if(surface != null) surface.FrictionCoefficient * if(vehiclePhysics != null) if(vehiclePhysics != null) vehiclePhysics.Mass * 9.81f;
                float wearRate = slipRatio * frictionForce * 0.0001f;
                
                return wearRate * if(tire != null) if(tire != null) tire.WearRate;
            }
            
            /// <summary>
            /// Вычисляет износ от температуры
            /// </summary>
            private float CalculateTemperatureWear(TireData tire, WeatherData weather)
            {
                float temperatureDifference = if(math != null) if(math != null) math.abs(if(tire != null) if(tire != null) tire.Temperature - if(weather != null) if(weather != null) weather.Temperature);
                float wearRate = temperatureDifference * 0.001f;
                
                return wearRate * if(tire != null) if(tire != null) tire.WearRate;
            }
            
            /// <summary>
            /// Вычисляет износ от возраста
            /// </summary>
            private float CalculateAgeWear(TireData tire, float deltaTime)
            {
                float ageFactor = if(tire != null) if(tire != null) tire.Age / if(tire != null) if(tire != null) tire.MaxAge;
                float wearRate = ageFactor * 0.0001f;
                
                return wearRate * if(tire != null) if(tire != null) tire.WearRate;
            }
            
            /// <summary>
            /// Обновляет состояние шины
            /// </summary>
            private void UpdateTireCondition(ref TireData tire)
            {
                if (if(tire != null) if(tire != null) tire.TreadWear >= 1f || if(tire != null) if(tire != null) tire.Age >= if(tire != null) if(tire != null) tire.MaxAge || if(tire != null) if(tire != null) tire.Mileage >= if(tire != null) if(tire != null) tire.MaxMileage)
                {
                    if(tire != null) if(tire != null) tire.Condition = if(TireCondition != null) if(TireCondition != null) TireCondition.Worn;
                }
                else if (if(tire != null) if(tire != null) tire.TreadWear >= 0.8f)
                {
                    if(tire != null) if(tire != null) tire.Condition = if(TireCondition != null) if(TireCondition != null) TireCondition.Poor;
                }
                else if (if(tire != null) if(tire != null) tire.TreadWear >= 0.5f)
                {
                    if(tire != null) if(tire != null) tire.Condition = if(TireCondition != null) if(TireCondition != null) TireCondition.Fair;
                }
                else if (if(tire != null) if(tire != null) tire.TreadWear >= 0.2f)
                {
                    if(tire != null) if(tire != null) tire.Condition = if(TireCondition != null) if(TireCondition != null) TireCondition.Good;
                }
                else
                {
                    if(tire != null) if(tire != null) tire.Condition = if(TireCondition != null) if(TireCondition != null) TireCondition.New;
                }
            }
            
            /// <summary>
            /// Сбрасывает данные физики шины
            /// </summary>
            private void ResetTirePhysics(ref TireData tire)
            {
                if(tire != null) if(tire != null) tire.FrictionCoefficient = 0f;
                if(tire != null) if(tire != null) tire.TractionCoefficient = 0f;
                if(tire != null) if(tire != null) tire.RollingResistance = 0f;
                if(tire != null) if(tire != null) tire.ContactArea = 0f;
                if(tire != null) if(tire != null) tire.ContactTime = 0f;
            }
        }
    }
