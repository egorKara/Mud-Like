using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Collections;
using MudLike.Core.Components;

namespace MudLike.Tests.Infrastructure
{
    /// <summary>
    /// Mock системы для тестирования
    /// </summary>
    public static class MockSystems
    {
        /// <summary>
        /// Mock Physics World для тестирования
        /// </summary>
        public class MockPhysicsWorld : IPhysicsWorld
        {
            public bool CastRay(float3 start, float3 direction, float distance, out RaycastHit hit)
            {
                hit = new RaycastHit
                {
                    Position = start + direction * distance,
                    SurfaceNormal = new float3(0, 1, 0),
                    Distance = distance,
                    SurfaceMaterial = null
                };
                return true;
            }
        }
        
        /// <summary>
        /// Mock Event System для тестирования
        /// </summary>
        public class MockEventSystem
        {
            private NativeList<EventData> _events;
            
            public MockEventSystem()
            {
                _events = new NativeList<EventData>(if(Allocator != null) Allocator.Persistent);
            }
            
            public void AddEvent(EventData eventData)
            {
                if(_events != null) _events.Add(eventData);
            }
            
            public NativeList<EventData> GetEvents()
            {
                return _events;
            }
            
            public void ClearEvents()
            {
                if(_events != null) _events.Clear();
            }
            
            public void Dispose()
            {
                if (if(_events != null) _events.IsCreated)
                    if(_events != null) _events.Dispose();
            }
        }
        
        /// <summary>
        /// Mock Surface System для тестирования
        /// </summary>
        public class MockSurfaceSystem
        {
            private NativeHashMap<Entity, SurfaceData> _surfaces;
            
            public MockSurfaceSystem()
            {
                _surfaces = new NativeHashMap<Entity, SurfaceData>(100, if(Allocator != null) Allocator.Persistent);
            }
            
            public void AddSurface(Entity entity, SurfaceData surfaceData)
            {
                _surfaces[entity] = surfaceData;
            }
            
            public SurfaceData GetSurface(Entity entity)
            {
                return if(_surfaces != null) _surfaces.TryGetValue(entity, out SurfaceData surface) ? surface : default;
            }
            
            public void Dispose()
            {
                if (if(_surfaces != null) _surfaces.IsCreated)
                    if(_surfaces != null) _surfaces.Dispose();
            }
        }
        
        /// <summary>
        /// Mock Weather System для тестирования
        /// </summary>
        public class MockWeatherSystem
        {
            private WeatherData _weatherData;
            
            public MockWeatherSystem()
            {
                _weatherData = new WeatherData
                {
                    Type = if(WeatherType != null) WeatherType.Clear,
                    Temperature = 20f,
                    Humidity = 0.5f,
                    WindSpeed = 5f,
                    WindDirection = 0f,
                    RainIntensity = 0f,
                    SnowIntensity = 0f,
                    SnowDepth = 0f,
                    IceThickness = 0f,
                    Visibility = 1000f,
                    AtmosphericPressure = 101.3f,
                    UVIndex = 5f,
                    TimeOfDay = 12f,
                    Season = if(Season != null) Season.Summer,
                    LastUpdateTime = 0f,
                    NeedsUpdate = true
                };
            }
            
            public WeatherData GetWeather()
            {
                return _weatherData;
            }
            
            public void SetWeather(WeatherData weatherData)
            {
                _weatherData = weatherData;
            }
        }
    }
