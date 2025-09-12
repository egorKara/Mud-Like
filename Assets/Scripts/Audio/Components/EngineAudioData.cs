using Unity.Entities;

namespace MudLike.Audio.Components
{
    /// <summary>
    /// Данные звука двигателя
    /// </summary>
    public struct EngineAudioData : IComponentData
    {
        /// <summary>
        /// Обороты двигателя
        /// </summary>
        public float RPM;
        
        /// <summary>
        /// Мощность двигателя
        /// </summary>
        public float Power;
        
        /// <summary>
        /// Температура двигателя
        /// </summary>
        public float Temperature;
        
        /// <summary>
        /// Громкость звука
        /// </summary>
        public float Volume;
        
        /// <summary>
        /// Высота звука
        /// </summary>
        public float Pitch;
        
        /// <summary>
        /// Звук воспроизводится
        /// </summary>
        public bool IsPlaying;
        
        /// <summary>
        /// Требует обновления
        /// </summary>
        public bool NeedsUpdate;
    }
}