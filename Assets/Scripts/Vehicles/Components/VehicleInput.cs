using Unity.Entities;
using Unity.Mathematics;

namespace MudLike.Vehicles.Components
{
    /// <summary>
    /// Ввод для управления транспортным средством
    /// </summary>
    public struct VehicleInput : IComponentData
    {
        /// <summary>
        /// Ввод движения (горизонтальная ось)
        /// </summary>
        public float Horizontal;
        
        /// <summary>
        /// Ввод движения (вертикальная ось)
        /// </summary>
        public float Vertical;
        
        /// <summary>
        /// Ввод торможения
        /// </summary>
        public float Brake;
        
        /// <summary>
        /// Ввод ручного тормоза
        /// </summary>
        public float Handbrake;
        
        /// <summary>
        /// Ввод переключения передачи вверх
        /// </summary>
        public bool ShiftUp;
        
        /// <summary>
        /// Ввод переключения передачи вниз
        /// </summary>
        public bool ShiftDown;
        
        /// <summary>
        /// Ввод сброса передачи в нейтраль
        /// </summary>
        public bool Neutral;
        
        /// <summary>
        /// Ввод реверса
        /// </summary>
        public bool Reverse;
        
        /// <summary>
        /// Ввод включения/выключения двигателя
        /// </summary>
        public bool EngineToggle;
        
        /// <summary>
        /// Ввод включения/выключения фар
        /// </summary>
        public bool HeadlightsToggle;
        
        /// <summary>
        /// Ввод включения/выключения аварийных огней
        /// </summary>
        public bool HazardLightsToggle;
        
        /// <summary>
        /// Ввод включения/выключения звукового сигнала
        /// </summary>
        public bool Horn;
    }
}