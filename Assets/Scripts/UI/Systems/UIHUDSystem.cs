using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using MudLike.UI.Components;
using MudLike.Vehicles.Components;
using MudLike.Weather.Components;
using MudLike.Core.Components;

namespace MudLike.UI.Systems
{
    /// <summary>
    /// Система обновления HUD интерфейса
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class UIHUDSystem : SystemBase
    {
        private EntityQuery _hudQuery;
        private EntityQuery _vehicleQuery;
        private EntityQuery _weatherQuery;
        
        protected override void OnCreate()
        {
            _hudQuery = GetEntityQuery(
                ComponentType.ReadWrite<UIHUDData>()
            );
            
            _vehicleQuery = GetEntityQuery(
                ComponentType.ReadOnly<VehiclePhysics>(),
                ComponentType.ReadOnly<VehicleConfig>(),
                ComponentType.ReadOnly<LocalTransform>()
            );
            
            _weatherQuery = GetEntityQuery(
                ComponentType.ReadOnly<WeatherData>()
            );
        }
        
        protected override void OnUpdate()
        {
            // Обновляем HUD данные
            UpdateHUDData();
        }
        
        /// <summary>
        /// Обновляет данные HUD
        /// </summary>
        private void UpdateHUDData()
        {
            Entities
                .WithAll<UIHUDData>()
                .ForEach((ref UIHUDData hudData) =>
                {
                    // Получаем данные транспорта
                    var vehicleData = GetVehicleData();
                    if (vehicleData.HasValue)
                    {
                        hudData.Speed = vehicleData.Value.Speed;
                        hudData.RPM = vehicleData.Value.RPM;
                        hudData.CurrentGear = vehicleData.Value.CurrentGear;
                        hudData.Health = vehicleData.Value.Health;
                        hudData.FuelLevel = vehicleData.Value.FuelLevel;
                        hudData.EngineTemperature = vehicleData.Value.EngineTemperature;
                        hudData.MapPosition = vehicleData.Value.MapPosition;
                        hudData.Heading = vehicleData.Value.Heading;
                    }
                    
                    // Получаем данные погоды
                    var weatherData = GetWeatherData();
                    if (weatherData.HasValue)
                    {
                        hudData.WeatherInfo = weatherData.Value;
                    }
                    
                    // Обновляем время игры
                    hudData.GameTime += SystemAPI.Time.DeltaTime;
                    
                    // Обновляем сетевую информацию
                    hudData.PlayerCount = GetPlayerCount();
                    hudData.Ping = GetPing();
                    
                    hudData.NeedsUpdate = true;
                }).Schedule();
        }
        
        /// <summary>
        /// Получает данные транспорта
        /// </summary>
        private VehicleHUDData? GetVehicleData()
        {
            var vehicleData = new VehicleHUDData();
            bool found = false;
            
            Entities
                .WithAll<VehiclePhysics, VehicleConfig, LocalTransform>()
                .ForEach((in VehiclePhysics physics, in VehicleConfig config, in LocalTransform transform) =>
                {
                    vehicleData.Speed = math.length(physics.Velocity) * 3.6f; // м/с в км/ч
                    vehicleData.RPM = physics.EngineRPM;
                    vehicleData.CurrentGear = physics.CurrentGear;
                    vehicleData.Health = GetVehicleHealth(transform.Position);
                    vehicleData.FuelLevel = GetVehicleFuelLevel(transform.Position);
                    vehicleData.EngineTemperature = GetEngineTemperature(transform.Position);
                    vehicleData.MapPosition = new float2(transform.Position.x, transform.Position.z);
                    vehicleData.Heading = math.degrees(math.atan2(transform.Rotation.value.x, transform.Rotation.value.z));
                    found = true;
                }).Schedule();
            
            return found ? vehicleData : null;
        }
        
        /// <summary>
        /// Получает данные погоды
        /// </summary>
        private WeatherInfo? GetWeatherData()
        {
            var weatherInfo = new WeatherInfo();
            bool found = false;
            
            Entities
                .WithAll<WeatherData>()
                .ForEach((in WeatherData weather) =>
                {
                    weatherInfo.Type = (WeatherType)weather.Type;
                    weatherInfo.Temperature = weather.Temperature;
                    weatherInfo.Humidity = weather.Humidity;
                    weatherInfo.WindSpeed = weather.WindSpeed;
                    weatherInfo.RainIntensity = weather.RainIntensity;
                    weatherInfo.SnowIntensity = weather.SnowIntensity;
                    weatherInfo.Visibility = weather.Visibility;
                    found = true;
                }).Schedule();
            
            return found ? weatherInfo : null;
        }
        
        /// <summary>
        /// Получает количество игроков
        /// </summary>
        private int GetPlayerCount()
        {
            int playerCount = GetNetworkPlayerCount();
            return 1;
        }
        
        /// <summary>
        /// Получает ping
        /// </summary>
        private int GetPing()
        {
            int ping = GetNetworkPing();
            return 0;
        }
        
        /// <summary>
        /// Получает здоровье транспорта
        /// </summary>
        private float GetVehicleHealth(float3 position)
        {
            // Ищем ближайший транспорт
            float health = 1.0f;
            Entities
                .WithAll<VehicleTag>()
                .ForEach((in VehicleDamageData damage) =>
                {
                    health = damage.Health;
                }).Schedule();
            
            return health;
        }
        
        /// <summary>
        /// Получает уровень топлива транспорта
        /// </summary>
        private float GetVehicleFuelLevel(float3 position)
        {
            // Ищем ближайший транспорт
            float fuelLevel = 1.0f;
            Entities
                .WithAll<VehicleTag>()
                .ForEach((in VehicleFuelData fuel) =>
                {
                    fuelLevel = fuel.CurrentFuel / fuel.MaxFuel;
                }).Schedule();
            
            return fuelLevel;
        }
        
        /// <summary>
        /// Получает температуру двигателя
        /// </summary>
        private float GetEngineTemperature(float3 position)
        {
            // Ищем ближайший транспорт
            float temperature = 0.5f;
            Entities
                .WithAll<VehicleTag>()
                .ForEach((in EngineData engine) =>
                {
                    temperature = engine.Temperature / engine.MaxTemperature;
                }).Schedule();
            
            return temperature;
        }
        
        /// <summary>
        /// Получает количество игроков в сети
        /// </summary>
        private int GetNetworkPlayerCount()
        {
            int count = 0;
            Entities
                .WithAll<PlayerTag, NetworkId>()
                .ForEach((in NetworkId networkId) =>
                {
                    count++;
                }).Schedule();
            
            return count;
        }
        
        /// <summary>
        /// Получает ping сети
        /// </summary>
        private int GetNetworkPing()
        {
            // Получаем ping из NetworkManager если доступен
            var networkManager = SystemAPI.GetSingleton<NetworkManagerData>();
            return networkManager.Ping;
        }
        
        /// <summary>
        /// Данные транспорта для HUD
        /// </summary>
        private struct VehicleHUDData
        {
            public float Speed;
            public float RPM;
            public int CurrentGear;
            public float Health;
            public float FuelLevel;
            public float EngineTemperature;
            public float2 MapPosition;
            public float Heading;
        }
    }
}