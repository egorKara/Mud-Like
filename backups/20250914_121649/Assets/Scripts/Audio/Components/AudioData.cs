using Unity.Entities;
using Unity.Mathematics;

namespace MudLike.Audio.Components
{
    /// <summary>
    /// Данные аудио системы
    /// </summary>
    public struct AudioData : IComponentData
    {
        /// <summary>
        /// Громкость двигателя (0-1)
        /// </summary>
        public float EngineVolume;
        
        /// <summary>
        /// Громкость колес (0-1)
        /// </summary>
        public float WheelVolume;
        
        /// <summary>
        /// Громкость окружения (0-1)
        /// </summary>
        public float EnvironmentalVolume;
        
        /// <summary>
        /// Общая громкость (0-1)
        /// </summary>
        public float MasterVolume;
        
        /// <summary>
        /// Аудио активно
        /// </summary>
        public bool IsActive;
        
        /// <summary>
        /// Аудио требует обновления
        /// </summary>
        public bool NeedsUpdate;
    }
    
    /// <summary>
    /// Данные звука двигателя
    /// </summary>
    public struct EngineAudioData : IComponentData
    {
        /// <summary>
        /// Обороты двигателя (RPM)
        /// </summary>
        public float RPM;
        
        /// <summary>
        /// Мощность двигателя (0-1)
        /// </summary>
        public float Power;
        
        /// <summary>
        /// Температура двигателя (0-1)
        /// </summary>
        public float Temperature;
        
        /// <summary>
        /// Громкость звука двигателя (0-1)
        /// </summary>
        public float Volume;
        
        /// <summary>
        /// Высота звука двигателя (0-1)
        /// </summary>
        public float Pitch;
        
        /// <summary>
        /// Звук двигателя активен
        /// </summary>
        public bool IsPlaying;
        
        /// <summary>
        /// Звук двигателя требует обновления
        /// </summary>
        public bool NeedsUpdate;
    }
    
    /// <summary>
    /// Данные звука колес
    /// </summary>
    public struct WheelAudioData : IComponentData
    {
        /// <summary>
        /// Скорость колеса (м/с)
        /// </summary>
        public float Speed;
        
        /// <summary>
        /// Сцепление с поверхностью (0-1)
        /// </summary>
        public float Traction;
        
        /// <summary>
        /// Тип поверхности
        /// </summary>
        public SurfaceType SurfaceType;
        
        /// <summary>
        /// Громкость звука колес (0-1)
        /// </summary>
        public float Volume;
        
        /// <summary>
        /// Высота звука колес (0-1)
        /// </summary>
        public float Pitch;
        
        /// <summary>
        /// Звук колес активен
        /// </summary>
        public bool IsPlaying;
        
        /// <summary>
        /// Звук колес требует обновления
        /// </summary>
        public bool NeedsUpdate;
    }
    
    /// <summary>
    /// Данные звука окружения
    /// </summary>
    public struct EnvironmentalAudioData : IComponentData
    {
        /// <summary>
        /// Тип погоды
        /// </summary>
        public WeatherType WeatherType;
        
        /// <summary>
        /// Интенсивность дождя (0-1)
        /// </summary>
        public float RainIntensity;
        
        /// <summary>
        /// Интенсивность снега (0-1)
        /// </summary>
        public float SnowIntensity;
        
        /// <summary>
        /// Скорость ветра (м/с)
        /// </summary>
        public float WindSpeed;
        
        /// <summary>
        /// Громкость звука окружения (0-1)
        /// </summary>
        public float Volume;
        
        /// <summary>
        /// Звук окружения активен
        /// </summary>
        public bool IsPlaying;
        
        /// <summary>
        /// Звук окружения требует обновления
        /// </summary>
        public bool NeedsUpdate;
    }
    
    /// <summary>
    /// Тип поверхности для аудио
    /// </summary>
    public enum SurfaceType
    {
        Asphalt,     // Асфальт
        Dirt,        // Грязь
        Sand,        // Песок
        Mud,         // Грязь
        Water,       // Вода
        Snow,        // Снег
        Ice,         // Лед
        Grass,       // Трава
        Rock,        // Камень
        Swamp,       // Болото
        Gravel,      // Гравий
        Concrete     // Бетон
    }
    
    /// <summary>
    /// Тип погоды для аудио
    /// </summary>
    public enum WeatherType
    {
        Clear,       // Ясно
        Cloudy,      // Облачно
        Rainy,       // Дождь
        Snowy,       // Снег
        Foggy,       // Туман
        Stormy,      // Буря
        Windy,       // Ветрено
        Hot,         // Жарко
        Cold,        // Холодно
        Icy          // Лед
    }
}