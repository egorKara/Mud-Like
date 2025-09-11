using Unity.Entities;
using Unity.Mathematics;

namespace MudLike.Vehicles.Components
{
    /// <summary>
    /// Данные о повреждениях транспортного средства
    /// </summary>
    public struct VehicleDamageData : IComponentData
    {
        /// <summary>
        /// Общее состояние транспортного средства (0-1, где 1 = идеальное состояние)
        /// </summary>
        public float OverallCondition;
        
        /// <summary>
        /// Состояние двигателя (0-1)
        /// </summary>
        public float EngineCondition;
        
        /// <summary>
        /// Состояние трансмиссии (0-1)
        /// </summary>
        public float TransmissionCondition;
        
        /// <summary>
        /// Состояние подвески (0-1)
        /// </summary>
        public float SuspensionCondition;
        
        /// <summary>
        /// Состояние тормозов (0-1)
        /// </summary>
        public float BrakeCondition;
        
        /// <summary>
        /// Состояние рулевого управления (0-1)
        /// </summary>
        public float SteeringCondition;
        
        /// <summary>
        /// Состояние колес (0-1)
        /// </summary>
        public float WheelCondition;
        
        /// <summary>
        /// Состояние кузова (0-1)
        /// </summary>
        public float BodyCondition;
        
        /// <summary>
        /// Состояние кабины (0-1)
        /// </summary>
        public float CabinCondition;
        
        /// <summary>
        /// Состояние грузового отсека (0-1)
        /// </summary>
        public float CargoAreaCondition;
        
        /// <summary>
        /// Состояние топливной системы (0-1)
        /// </summary>
        public float FuelSystemCondition;
        
        /// <summary>
        /// Состояние электрической системы (0-1)
        /// </summary>
        public float ElectricalSystemCondition;
        
        /// <summary>
        /// Состояние системы охлаждения (0-1)
        /// </summary>
        public float CoolingSystemCondition;
        
        /// <summary>
        /// Состояние системы смазки (0-1)
        /// </summary>
        public float LubricationSystemCondition;
        
        /// <summary>
        /// Состояние системы зажигания (0-1)
        /// </summary>
        public float IgnitionSystemCondition;
        
        /// <summary>
        /// Состояние системы впуска (0-1)
        /// </summary>
        public float IntakeSystemCondition;
        
        /// <summary>
        /// Состояние системы выпуска (0-1)
        /// </summary>
        public float ExhaustSystemCondition;
        
        /// <summary>
        /// Состояние системы кондиционирования (0-1)
        /// </summary>
        public float AirConditioningCondition;
        
        /// <summary>
        /// Состояние системы отопления (0-1)
        /// </summary>
        public float HeatingSystemCondition;
        
        /// <summary>
        /// Состояние системы освещения (0-1)
        /// </summary>
        public float LightingSystemCondition;
        
        /// <summary>
        /// Состояние системы сигнализации (0-1)
        /// </summary>
        public float SignalingSystemCondition;
        
        /// <summary>
        /// Состояние системы безопасности (0-1)
        /// </summary>
        public float SafetySystemCondition;
        
        /// <summary>
        /// Состояние системы навигации (0-1)
        /// </summary>
        public float NavigationSystemCondition;
        
        /// <summary>
        /// Состояние системы связи (0-1)
        /// </summary>
        public float CommunicationSystemCondition;
        
        /// <summary>
        /// Состояние системы мониторинга (0-1)
        /// </summary>
        public float MonitoringSystemCondition;
        
        /// <summary>
        /// Состояние системы диагностики (0-1)
        /// </summary>
        public float DiagnosticSystemCondition;
        
        /// <summary>
        /// Состояние системы управления (0-1)
        /// </summary>
        public float ControlSystemCondition;
        
        /// <summary>
        /// Состояние системы стабилизации (0-1)
        /// </summary>
        public float StabilizationSystemCondition;
        
        /// <summary>
        /// Состояние системы контроля тяги (0-1)
        /// </summary>
        public float TractionControlSystemCondition;
        
        /// <summary>
        /// Состояние системы помощи при торможении (0-1)
        /// </summary>
        public float BrakeAssistSystemCondition;
        
        /// <summary>
        /// Состояние антиблокировочной системы (0-1)
        /// </summary>
        public float ABSSystemCondition;
        
        /// <summary>
        /// Состояние системы помощи при старте в гору (0-1)
        /// </summary>
        public float HillStartAssistSystemCondition;
        
        /// <summary>
        /// Состояние системы помощи при спуске (0-1)
        /// </summary>
        public float HillDescentControlSystemCondition;
        
        /// <summary>
        /// Состояние системы контроля давления в шинах (0-1)
        /// </summary>
        public float TirePressureMonitoringSystemCondition;
        
        /// <summary>
        /// Состояние системы контроля температуры двигателя (0-1)
        /// </summary>
        public float EngineTemperatureMonitoringSystemCondition;
        
        /// <summary>
        /// Состояние системы контроля уровня масла (0-1)
        /// </summary>
        public float OilLevelMonitoringSystemCondition;
        
        /// <summary>
        /// Состояние системы контроля уровня охлаждающей жидкости (0-1)
        /// </summary>
        public float CoolantLevelMonitoringSystemCondition;
        
        /// <summary>
        /// Состояние системы контроля уровня тормозной жидкости (0-1)
        /// </summary>
        public float BrakeFluidLevelMonitoringSystemCondition;
        
        /// <summary>
        /// Состояние системы контроля уровня топлива (0-1)
        /// </summary>
        public float FuelLevelMonitoringSystemCondition;
        
        /// <summary>
        /// Состояние системы контроля заряда аккумулятора (0-1)
        /// </summary>
        public float BatteryChargeMonitoringSystemCondition;
        
        /// <summary>
        /// Состояние системы контроля напряжения (0-1)
        /// </summary>
        public float VoltageMonitoringSystemCondition;
        
        /// <summary>
        /// Состояние системы контроля температуры аккумулятора (0-1)
        /// </summary>
        public float BatteryTemperatureMonitoringSystemCondition;
        
        /// <summary>
        /// Состояние системы контроля температуры трансмиссии (0-1)
        /// </summary>
        public float TransmissionTemperatureMonitoringSystemCondition;
        
        /// <summary>
        /// Состояние системы контроля температуры дифференциала (0-1)
        /// </summary>
        public float DifferentialTemperatureMonitoringSystemCondition;
        
        /// <summary>
        /// Состояние системы контроля температуры подвески (0-1)
        /// </summary>
        public float SuspensionTemperatureMonitoringSystemCondition;
        
        /// <summary>
        /// Состояние системы контроля температуры тормозов (0-1)
        /// </summary>
        public float BrakeTemperatureMonitoringSystemCondition;
        
        /// <summary>
        /// Состояние системы контроля температуры шин (0-1)
        /// </summary>
        public float TireTemperatureMonitoringSystemCondition;
        
        /// <summary>
        /// Состояние системы контроля износа шин (0-1)
        /// </summary>
        public float TireWearMonitoringSystemCondition;
        
        /// <summary>
        /// Состояние системы контроля балансировки колес (0-1)
        /// </summary>
        public float WheelBalanceMonitoringSystemCondition;
        
        /// <summary>
        /// Состояние системы контроля выравнивания колес (0-1)
        /// </summary>
        public float WheelAlignmentMonitoringSystemCondition;
        
        /// <summary>
        /// Состояние системы контроля подвески (0-1)
        /// </summary>
        public float SuspensionMonitoringSystemCondition;
        
        /// <summary>
        /// Состояние системы контроля амортизаторов (0-1)
        /// </summary>
        public float ShockAbsorberMonitoringSystemCondition;
        
        /// <summary>
        /// Состояние системы контроля пружин (0-1)
        /// </summary>
        public float SpringMonitoringSystemCondition;
        
        /// <summary>
        /// Состояние системы контроля стабилизаторов (0-1)
        /// </summary>
        public float StabilizerMonitoringSystemCondition;
        
        /// <summary>
        /// Состояние системы контроля рулевого управления (0-1)
        /// </summary>
        public float SteeringMonitoringSystemCondition;
        
        /// <summary>
        /// Состояние системы контроля рулевой рейки (0-1)
        /// </summary>
        public float SteeringRackMonitoringSystemCondition;
        
        /// <summary>
        /// Состояние системы контроля рулевых тяг (0-1)
        /// </summary>
        public float SteeringTieRodMonitoringSystemCondition;
        
        /// <summary>
        /// Состояние системы контроля рулевых наконечников (0-1)
        /// </summary>
        public float SteeringEndMonitoringSystemCondition;
        
        /// <summary>
        /// Состояние системы контроля рулевых подшипников (0-1)
        /// </summary>
        public float SteeringBearingMonitoringSystemCondition;
        
        /// <summary>
        /// Состояние системы контроля рулевых сальников (0-1)
        /// </summary>
        public float SteeringSealMonitoringSystemCondition;
        
        /// <summary>
        /// Состояние системы контроля рулевых уплотнений (0-1)
        /// </summary>
        public float SteeringGasketMonitoringSystemCondition;
        
        /// <summary>
        /// Состояние системы контроля рулевых прокладок (0-1)
        /// </summary>
        public float SteeringGasketMonitoringSystemCondition2;
        
        /// <summary>
        /// Состояние системы контроля рулевых втулок (0-1)
        /// </summary>
        public float SteeringBushingMonitoringSystemCondition;
        
        /// <summary>
        /// Состояние системы контроля рулевых вкладышей (0-1)
        /// </summary>
        public float SteeringBushingMonitoringSystemCondition2;
        
        /// <summary>
        /// Состояние системы контроля рулевых втулок (0-1)
        /// </summary>
        public float SteeringBushingMonitoringSystemCondition3;
        
        /// <summary>
        /// Состояние системы контроля рулевых вкладышей (0-1)
        /// </summary>
        public float SteeringBushingMonitoringSystemCondition4;
        
        /// <summary>
        /// Состояние системы контроля рулевых втулок (0-1)
        /// </summary>
        public float SteeringBushingMonitoringSystemCondition5;
        
        /// <summary>
        /// Состояние системы контроля рулевых вкладышей (0-1)
        /// </summary>
        public float SteeringBushingMonitoringSystemCondition6;
        
        /// <summary>
        /// Состояние системы контроля рулевых втулок (0-1)
        /// </summary>
        public float SteeringBushingMonitoringSystemCondition7;
        
        /// <summary>
        /// Состояние системы контроля рулевых вкладышей (0-1)
        /// </summary>
        public float SteeringBushingMonitoringSystemCondition8;
        
        /// <summary>
        /// Состояние системы контроля рулевых втулок (0-1)
        /// </summary>
        public float SteeringBushingMonitoringSystemCondition9;
        
        /// <summary>
        /// Состояние системы контроля рулевых вкладышей (0-1)
        /// </summary>
        public float SteeringBushingMonitoringSystemCondition10;
        
        /// <summary>
        /// Время последнего обновления
        /// </summary>
        public float LastUpdateTime;
        
        /// <summary>
        /// Требует ли обновления
        /// </summary>
        public bool NeedsUpdate;
    }
}