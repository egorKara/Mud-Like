using Unity.Entities;
using Unity.Mathematics;

namespace MudLike.Vehicles.Components
{
    /// <summary>
    /// Данные о техническом обслуживании транспортного средства
    /// </summary>
    public struct VehicleMaintenance : IComponentData
    {
        /// <summary>
        /// Время последнего обслуживания
        /// </summary>
        public float LastMaintenanceTime;
        
        /// <summary>
        /// Интервал обслуживания
        /// </summary>
        public float MaintenanceInterval;
        
        /// <summary>
        /// Пробег с последнего обслуживания
        /// </summary>
        public float MileageSinceLastMaintenance;
        
        /// <summary>
        /// Максимальный пробег между обслуживаниями
        /// </summary>
        public float MaxMileageBetweenMaintenance;
        
        /// <summary>
        /// Время работы двигателя с последнего обслуживания
        /// </summary>
        public float EngineHoursSinceLastMaintenance;
        
        /// <summary>
        /// Максимальное время работы двигателя между обслуживаниями
        /// </summary>
        public float MaxEngineHoursBetweenMaintenance;
        
        /// <summary>
        /// Уровень масла (0-1)
        /// </summary>
        public float OilLevel;
        
        /// <summary>
        /// Качество масла (0-1)
        /// </summary>
        public float OilQuality;
        
        /// <summary>
        /// Уровень охлаждающей жидкости (0-1)
        /// </summary>
        public float CoolantLevel;
        
        /// <summary>
        /// Качество охлаждающей жидкости (0-1)
        /// </summary>
        public float CoolantQuality;
        
        /// <summary>
        /// Уровень тормозной жидкости (0-1)
        /// </summary>
        public float BrakeFluidLevel;
        
        /// <summary>
        /// Качество тормозной жидкости (0-1)
        /// </summary>
        public float BrakeFluidQuality;
        
        /// <summary>
        /// Уровень топлива (0-1)
        /// </summary>
        public float FuelLevel;
        
        /// <summary>
        /// Качество топлива (0-1)
        /// </summary>
        public float FuelQuality;
        
        /// <summary>
        /// Уровень заряда аккумулятора (0-1)
        /// </summary>
        public float BatteryCharge;
        
        /// <summary>
        /// Состояние аккумулятора (0-1)
        /// </summary>
        public float BatteryCondition;
        
        /// <summary>
        /// Уровень напряжения (В)
        /// </summary>
        public float VoltageLevel;
        
        /// <summary>
        /// Температура аккумулятора (°C)
        /// </summary>
        public float BatteryTemperature;
        
        /// <summary>
        /// Уровень давления в шинах (кПа)
        /// </summary>
        public float4 TirePressure;
        
        /// <summary>
        /// Температура шин (°C)
        /// </summary>
        public float4 TireTemperature;
        
        /// <summary>
        /// Износ протектора шин (0-1)
        /// </summary>
        public float4 TireTreadWear;
        
        /// <summary>
        /// Балансировка колес (0-1)
        /// </summary>
        public float4 WheelBalance;
        
        /// <summary>
        /// Выравнивание колес (0-1)
        /// </summary>
        public float4 WheelAlignment;
        
        /// <summary>
        /// Состояние подвески (0-1)
        /// </summary>
        public float SuspensionCondition;
        
        /// <summary>
        /// Состояние амортизаторов (0-1)
        /// </summary>
        public float4 ShockAbsorberCondition;
        
        /// <summary>
        /// Состояние пружин (0-1)
        /// </summary>
        public float4 SpringCondition;
        
        /// <summary>
        /// Состояние стабилизаторов (0-1)
        /// </summary>
        public float4 StabilizerCondition;
        
        /// <summary>
        /// Состояние рулевого управления (0-1)
        /// </summary>
        public float SteeringCondition;
        
        /// <summary>
        /// Состояние рулевой рейки (0-1)
        /// </summary>
        public float SteeringRackCondition;
        
        /// <summary>
        /// Состояние рулевых тяг (0-1)
        /// </summary>
        public float4 SteeringTieRodCondition;
        
        /// <summary>
        /// Состояние рулевых наконечников (0-1)
        /// </summary>
        public float4 SteeringEndCondition;
        
        /// <summary>
        /// Состояние рулевых подшипников (0-1)
        /// </summary>
        public float4 SteeringBearingCondition;
        
        /// <summary>
        /// Состояние рулевых сальников (0-1)
        /// </summary>
        public float4 SteeringSealCondition;
        
        /// <summary>
        /// Состояние рулевых уплотнений (0-1)
        /// </summary>
        public float4 SteeringGasketCondition;
        
        /// <summary>
        /// Состояние рулевых прокладок (0-1)
        /// </summary>
        public float4 SteeringGasketCondition2;
        
        /// <summary>
        /// Состояние рулевых втулок (0-1)
        /// </summary>
        public float4 SteeringBushingCondition;
        
        /// <summary>
        /// Состояние рулевых вкладышей (0-1)
        /// </summary>
        public float4 SteeringBushingCondition2;
        
        /// <summary>
        /// Состояние рулевых втулок (0-1)
        /// </summary>
        public float4 SteeringBushingCondition3;
        
        /// <summary>
        /// Состояние рулевых вкладышей (0-1)
        /// </summary>
        public float4 SteeringBushingCondition4;
        
        /// <summary>
        /// Состояние рулевых втулок (0-1)
        /// </summary>
        public float4 SteeringBushingCondition5;
        
        /// <summary>
        /// Состояние рулевых вкладышей (0-1)
        /// </summary>
        public float4 SteeringBushingCondition6;
        
        /// <summary>
        /// Состояние рулевых втулок (0-1)
        /// </summary>
        public float4 SteeringBushingCondition7;
        
        /// <summary>
        /// Состояние рулевых вкладышей (0-1)
        /// </summary>
        public float4 SteeringBushingCondition8;
        
        /// <summary>
        /// Состояние рулевых втулок (0-1)
        /// </summary>
        public float4 SteeringBushingCondition9;
        
        /// <summary>
        /// Состояние рулевых вкладышей (0-1)
        /// </summary>
        public float4 SteeringBushingCondition10;
        
        /// <summary>
        /// Состояние тормозов (0-1)
        /// </summary>
        public float4 BrakeCondition;
        
        /// <summary>
        /// Состояние тормозных дисков (0-1)
        /// </summary>
        public float4 BrakeDiscCondition;
        
        /// <summary>
        /// Состояние тормозных колодок (0-1)
        /// </summary>
        public float4 BrakePadCondition;
        
        /// <summary>
        /// Состояние тормозных суппортов (0-1)
        /// </summary>
        public float4 BrakeCaliperCondition;
        
        /// <summary>
        /// Состояние тормозных цилиндров (0-1)
        /// </summary>
        public float4 BrakeCylinderCondition;
        
        /// <summary>
        /// Состояние тормозных шлангов (0-1)
        /// </summary>
        public float4 BrakeHoseCondition;
        
        /// <summary>
        /// Состояние тормозных трубок (0-1)
        /// </summary>
        public float4 BrakeTubeCondition;
        
        /// <summary>
        /// Состояние тормозных магистралей (0-1)
        /// </summary>
        public float4 BrakeLineCondition;
        
        /// <summary>
        /// Состояние тормозных клапанов (0-1)
        /// </summary>
        public float4 BrakeValveCondition;
        
        /// <summary>
        /// Состояние тормозных насосов (0-1)
        /// </summary>
        public float4 BrakePumpCondition;
        
        /// <summary>
        /// Состояние тормозных компрессоров (0-1)
        /// </summary>
        public float4 BrakeCompressorCondition;
        
        /// <summary>
        /// Состояние тормозных ресиверов (0-1)
        /// </summary>
        public float4 BrakeReceiverCondition;
        
        /// <summary>
        /// Состояние тормозных регуляторов (0-1)
        /// </summary>
        public float4 BrakeRegulatorCondition;
        
        /// <summary>
        /// Состояние тормозных датчиков (0-1)
        /// </summary>
        public float4 BrakeSensorCondition;
        
        /// <summary>
        /// Состояние тормозных контроллеров (0-1)
        /// </summary>
        public float4 BrakeControllerCondition;
        
        /// <summary>
        /// Состояние тормозных модуляторов (0-1)
        /// </summary>
        public float4 BrakeModulatorCondition;
        
        /// <summary>
        /// Состояние тормозных актуаторов (0-1)
        /// </summary>
        public float4 BrakeActuatorCondition;
        
        /// <summary>
        /// Состояние тормозных сервоприводов (0-1)
        /// </summary>
        public float4 BrakeServoCondition;
        
        /// <summary>
        /// Состояние тормозных моторов (0-1)
        /// </summary>
        public float4 BrakeMotorCondition;
        
        /// <summary>
        /// Состояние тормозных генераторов (0-1)
        /// </summary>
        public float4 BrakeGeneratorCondition;
        
        /// <summary>
        /// Состояние тормозных инверторов (0-1)
        /// </summary>
        public float4 BrakeInverterCondition;
        
        /// <summary>
        /// Состояние тормозных преобразователей (0-1)
        /// </summary>
        public float4 BrakeConverterCondition;
        
        /// <summary>
        /// Состояние тормозных стабилизаторов (0-1)
        /// </summary>
        public float4 BrakeStabilizerCondition;
        
        /// <summary>
        /// Состояние тормозных фильтров (0-1)
        /// </summary>
        public float4 BrakeFilterCondition;
        
        /// <summary>
        /// Состояние тормозных сепараторов (0-1)
        /// </summary>
        public float4 BrakeSeparatorCondition;
        
        /// <summary>
        /// Состояние тормозных конденсаторов (0-1)
        /// </summary>
        public float4 BrakeCapacitorCondition;
        
        /// <summary>
        /// Состояние тормозных резисторов (0-1)
        /// </summary>
        public float4 BrakeResistorCondition;
        
        /// <summary>
        /// Состояние тормозных катушек (0-1)
        /// </summary>
        public float4 BrakeCoilCondition;
        
        /// <summary>
        /// Состояние тормозных сердечников (0-1)
        /// </summary>
        public float4 BrakeCoreCondition;
        
        /// <summary>
        /// Состояние тормозных обмоток (0-1)
        /// </summary>
        public float4 BrakeWindingCondition;
        
        /// <summary>
        /// Состояние тормозных изоляторов (0-1)
        /// </summary>
        public float4 BrakeInsulatorCondition;
        
        /// <summary>
        /// Состояние тормозных проводников (0-1)
        /// </summary>
        public float4 BrakeConductorCondition;
        
        /// <summary>
        /// Состояние тормозных контактов (0-1)
        /// </summary>
        public float4 BrakeContactCondition;
        
        /// <summary>
        /// Состояние тормозных переключателей (0-1)
        /// </summary>
        public float4 BrakeSwitchCondition;
        
        /// <summary>
        /// Состояние тормозных реле (0-1)
        /// </summary>
        public float4 BrakeRelayCondition;
        
        /// <summary>
        /// Состояние тормозных предохранителей (0-1)
        /// </summary>
        public float4 BrakeFuseCondition;
        
        /// <summary>
        /// Состояние тормозных диодов (0-1)
        /// </summary>
        public float4 BrakeDiodeCondition;
        
        /// <summary>
        /// Состояние тормозных транзисторов (0-1)
        /// </summary>
        public float4 BrakeTransistorCondition;
        
        /// <summary>
        /// Состояние тормозных микросхем (0-1)
        /// </summary>
        public float4 BrakeChipCondition;
        
        /// <summary>
        /// Состояние тормозных плат (0-1)
        /// </summary>
        public float4 BrakeBoardCondition;
        
        /// <summary>
        /// Состояние тормозных разъемов (0-1)
        /// </summary>
        public float4 BrakeConnectorCondition;
        
        /// <summary>
        /// Состояние тормозных кабелей (0-1)
        /// </summary>
        public float4 BrakeCableCondition;
        
        /// <summary>
        /// Состояние тормозных проводов (0-1)
        /// </summary>
        public float4 BrakeWireCondition;
        
        /// <summary>
        /// Состояние тормозных клемм (0-1)
        /// </summary>
        public float4 BrakeTerminalCondition;
        
        /// <summary>
        /// Состояние тормозных зажимов (0-1)
        /// </summary>
        public float4 BrakeClampCondition;
        
        /// <summary>
        /// Состояние тормозных хомутов (0-1)
        /// </summary>
        public float4 BrakeHoseClampCondition;
        
        /// <summary>
        /// Состояние тормозных скоб (0-1)
        /// </summary>
        public float4 BrakeBracketCondition;
        
        /// <summary>
        /// Состояние тормозных кронштейнов (0-1)
        /// </summary>
        public float4 BrakeBracketCondition2;
        
        /// <summary>
        /// Состояние тормозных опор (0-1)
        /// </summary>
        public float4 BrakeSupportCondition;
        
        /// <summary>
        /// Состояние тормозных креплений (0-1)
        /// </summary>
        public float4 BrakeMountCondition;
        
        /// <summary>
        /// Состояние тормозных подвесов (0-1)
        /// </summary>
        public float4 BrakeHangerCondition;
        
        /// <summary>
        /// Состояние тормозных подвесок (0-1)
        /// </summary>
        public float4 BrakeHangerCondition2;
        
        /// <summary>
        /// Состояние тормозных подвесок (0-1)
        /// </summary>
        public float4 BrakeHangerCondition3;
        
        /// <summary>
        /// Состояние тормозных подвесок (0-1)
        /// </summary>
        public float4 BrakeHangerCondition4;
        
        /// <summary>
        /// Состояние тормозных подвесок (0-1)
        /// </summary>
        public float4 BrakeHangerCondition5;
        
        /// <summary>
        /// Состояние тормозных подвесок (0-1)
        /// </summary>
        public float4 BrakeHangerCondition6;
        
        /// <summary>
        /// Состояние тормозных подвесок (0-1)
        /// </summary>
        public float4 BrakeHangerCondition7;
        
        /// <summary>
        /// Состояние тормозных подвесок (0-1)
        /// </summary>
        public float4 BrakeHangerCondition8;
        
        /// <summary>
        /// Состояние тормозных подвесок (0-1)
        /// </summary>
        public float4 BrakeHangerCondition9;
        
        /// <summary>
        /// Состояние тормозных подвесок (0-1)
        /// </summary>
        public float4 BrakeHangerCondition10;
        
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