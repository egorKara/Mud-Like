using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using MudLike.Vehicles.Components;

namespace MudLike.Networking.Components
{
    /// <summary>
    /// Сетевые данные транспортного средства
    /// </summary>
    public struct NetworkVehicle : IComponentData, ICommandData
    {
        /// <summary>
        /// Конфигурация транспорта
        /// </summary>
        public VehicleConfig Config;
        
        /// <summary>
        /// Физические данные транспорта
        /// </summary>
        public VehiclePhysics Physics;
        
        /// <summary>
        /// Ввод транспорта
        /// </summary>
        public VehicleInput Input;
        
        /// <summary>
        /// Данные двигателя
        /// </summary>
        public EngineData Engine;
        
        /// <summary>
        /// Данные трансмиссии
        /// </summary>
        public TransmissionData Transmission;
        
        /// <summary>
        /// Транспорт изменился
        /// </summary>
        public bool HasChanged;
        
        /// <summary>
        /// Тик команды
        /// </summary>
        public uint Tick => 0; // Будет установлен Netcode
    }
}