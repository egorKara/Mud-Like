using Unity.Entities;
using Unity.Mathematics;

namespace MudLike.Audio.Components
{
    /// <summary>
    /// Данные аудио источника
    /// </summary>
    public struct AudioSourceData : IComponentData
    {
        /// <summary>
        /// ID звука
        /// </summary>
        public int SoundID;
        
        /// <summary>
        /// Громкость звука
        /// </summary>
        public float Volume;
        
        /// <summary>
        /// Скорость воспроизведения
        /// </summary>
        public float Pitch;
        
        /// <summary>
        /// Зациклен ли звук
        /// </summary>
        public bool IsLooping;
        
        /// <summary>
        /// Воспроизводится ли звук
        /// </summary>
        public bool IsPlaying;
        
        /// <summary>
        /// Время воспроизведения
        /// </summary>
        public float PlayTime;
        
        /// <summary>
        /// Максимальное расстояние слышимости
        /// </summary>
        public float MaxDistance;
        
        /// <summary>
        /// Минимальное расстояние слышимости
        /// </summary>
        public float MinDistance;
    }
}