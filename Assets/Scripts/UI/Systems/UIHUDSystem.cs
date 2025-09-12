using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;
using MudLike.UI.Components;
using MudLike.Vehicles.Components;
using MudLike.Weather.Components;

namespace MudLike.UI.Systems
{
    /// <summary>
    /// Система HUD интерфейса
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [BurstCompile]
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
                ComponentType.ReadOnly<EngineData>(),
                ComponentType.ReadOnly<TransmissionData>()
            );
            
            _weatherQuery = GetEntityQuery(
                ComponentType.ReadOnly<WeatherData>()
            );
        }
        
        protected override void OnUpdate()
        {
            float deltaTime = SystemAPI.Time.DeltaTime;
            
            // Обновляем HUD
            UpdateHUD(deltaTime);
        }
        
        private void UpdateHUD(float deltaTime)
        {
            // Получаем данные о погоде
            var weatherData = GetWeatherData();
            
            // Обновляем HUD для каждого игрока
            Entities
                .WithAll<UIHUDData>()
                .ForEach((ref UIHUDData hud) =>
                {
                    UpdateHUDData(ref hud, weatherData, deltaTime);
                }).Schedule();
        }
        
        private WeatherInfo GetWeatherData()
        {
            var weatherEntities = _weatherQuery.ToEntityArray(Unity.Collections.Allocator.Temp);
            
            if (weatherEntities.Length > 0)
            {
                var weather = EntityManager.GetComponentData<WeatherData>(weatherEntities[0]);
                weatherEntities.Dispose();
                
                return new WeatherInfo
                {
                    Type = weather.Type,
                    Temperature = weather.Temperature,
                    Humidity = weather.Humidity
                };
            }
            
            weatherEntities.Dispose();
            
            return new WeatherInfo
            {
                Type = WeatherType.Clear,
                Temperature = 20f,
                Humidity = 0.5f
            };
        }
        
        private void UpdateHUDData(ref UIHUDData hud, WeatherInfo weatherInfo, float deltaTime)
        {
            // Обновляем информацию о погоде
            hud.WeatherInfo = weatherInfo;
            
            // Обновляем статус HUD
            hud.IsActive = true;
            hud.NeedsUpdate = false;
        }
    }
}