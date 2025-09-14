using Unity.Entities;

namespace MudLike.Vehicles.Components
{
    /// <summary>
    /// Данные топлива транспортного средства
    /// </summary>
    public struct VehicleFuelData : IComponentData
    {
        /// <summary>
        /// Текущее количество топлива
        /// </summary>
        public float CurrentFuel;
        
        /// <summary>
        /// Максимальное количество топлива
        /// </summary>
        public float MaxFuel;
        
        /// <summary>
        /// Скорость потребления топлива (литры в секунду)
        /// </summary>
        public float FuelConsumptionRate;
        
        /// <summary>
        /// Порог низкого топлива (процент от максимума)
        /// </summary>
        public float LowFuelThreshold;
        
        /// <summary>
        /// Предупреждение о низком топливе показано
        /// </summary>
        public bool LowFuelWarningShown;
    }
}
