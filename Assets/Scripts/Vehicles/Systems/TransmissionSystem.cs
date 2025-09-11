using Unity.Entities;
using Unity.Mathematics;
using MudLike.Vehicles.Components;

namespace MudLike.Vehicles.Systems
{
    /// <summary>
    /// Система трансмиссии транспортного средства
    /// </summary>
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial class TransmissionSystem : SystemBase
    {
        /// <summary>
        /// Обрабатывает трансмиссию всех транспортных средств
        /// </summary>
        protected override void OnUpdate()
        {
            float deltaTime = Time.fixedDeltaTime;
            
            Entities
                .WithAll<VehicleTag>()
                .ForEach((ref TransmissionData transmission, 
                         ref VehiclePhysics physics, 
                         in VehicleInput input, 
                         in EngineData engine) =>
                {
                    ProcessTransmission(ref transmission, ref physics, input, engine, deltaTime);
                }).Schedule();
        }
        
        /// <summary>
        /// Обрабатывает трансмиссию конкретного транспортного средства
        /// </summary>
        private static void ProcessTransmission(ref TransmissionData transmission, 
                                              ref VehiclePhysics physics, 
                                              in VehicleInput input, 
                                              in EngineData engine, 
                                              float deltaTime)
        {
            // Обработка переключения передач
            if (input.ShiftUp && !transmission.IsShifting)
            {
                ShiftGearUp(ref transmission);
            }
            else if (input.ShiftDown && !transmission.IsShifting)
            {
                ShiftGearDown(ref transmission);
            }
            else if (input.Neutral && !transmission.IsShifting)
            {
                SetNeutralGear(ref transmission);
            }
            else if (input.Reverse && !transmission.IsShifting)
            {
                SetReverseGear(ref transmission);
            }
            
            // Обработка автоматического переключения передач
            if (!transmission.IsShifting)
            {
                ProcessAutomaticShifting(ref transmission, engine);
            }
            
            // Обработка процесса переключения передачи
            if (transmission.IsShifting)
            {
                ProcessGearShifting(ref transmission, deltaTime);
            }
            
            // Вычисляем передаточное отношение
            float gearRatio = GetCurrentGearRatio(transmission);
            
            // Вычисляем выходной крутящий момент
            transmission.OutputTorque = engine.CurrentTorque * gearRatio * transmission.Efficiency;
            
            // Вычисляем выходную мощность
            transmission.OutputPower = engine.CurrentPower * transmission.Efficiency;
            
            // Обновляем физику
            physics.CurrentGear = transmission.CurrentGear;
        }
        
        /// <summary>
        /// Переключает передачу вверх
        /// </summary>
        private static void ShiftGearUp(ref TransmissionData transmission)
        {
            if (transmission.CurrentGear < transmission.MaxGear)
            {
                transmission.TargetGear = transmission.CurrentGear + 1;
                transmission.IsShifting = true;
                transmission.CurrentShiftTime = 0f;
            }
        }
        
        /// <summary>
        /// Переключает передачу вниз
        /// </summary>
        private static void ShiftGearDown(ref TransmissionData transmission)
        {
            if (transmission.CurrentGear > transmission.MinGear)
            {
                transmission.TargetGear = transmission.CurrentGear - 1;
                transmission.IsShifting = true;
                transmission.CurrentShiftTime = 0f;
            }
        }
        
        /// <summary>
        /// Устанавливает нейтральную передачу
        /// </summary>
        private static void SetNeutralGear(ref TransmissionData transmission)
        {
            transmission.TargetGear = transmission.NeutralGear;
            transmission.IsShifting = true;
            transmission.CurrentShiftTime = 0f;
        }
        
        /// <summary>
        /// Устанавливает передачу заднего хода
        /// </summary>
        private static void SetReverseGear(ref TransmissionData transmission)
        {
            transmission.TargetGear = transmission.ReverseGear;
            transmission.IsShifting = true;
            transmission.CurrentShiftTime = 0f;
        }
        
        /// <summary>
        /// Обрабатывает автоматическое переключение передач
        /// </summary>
        private static void ProcessAutomaticShifting(ref TransmissionData transmission, in EngineData engine)
        {
            // Переключение вверх
            if (engine.CurrentRPM > transmission.UpshiftRPM && transmission.CurrentGear < transmission.MaxGear)
            {
                ShiftGearUp(ref transmission);
            }
            // Переключение вниз
            else if (engine.CurrentRPM < transmission.DownshiftRPM && transmission.CurrentGear > transmission.MinGear)
            {
                ShiftGearDown(ref transmission);
            }
        }
        
        /// <summary>
        /// Обрабатывает процесс переключения передачи
        /// </summary>
        private static void ProcessGearShifting(ref TransmissionData transmission, float deltaTime)
        {
            transmission.CurrentShiftTime += deltaTime;
            
            if (transmission.CurrentShiftTime >= transmission.ShiftTime)
            {
                // Завершаем переключение
                transmission.CurrentGear = transmission.TargetGear;
                transmission.IsShifting = false;
                transmission.CurrentShiftTime = 0f;
            }
        }
        
        /// <summary>
        /// Получает текущее передаточное отношение
        /// </summary>
        private static float GetCurrentGearRatio(in TransmissionData transmission)
        {
            if (transmission.CurrentGear == transmission.NeutralGear)
                return 0f;
            
            if (transmission.CurrentGear == transmission.ReverseGear)
                return -transmission.FinalDriveRatio;
            
            // Получаем передаточное число для текущей передачи
            float gearRatio = 0f;
            switch (transmission.CurrentGear)
            {
                case 1:
                    gearRatio = transmission.GearRatios.x;
                    break;
                case 2:
                    gearRatio = transmission.GearRatios.y;
                    break;
                case 3:
                    gearRatio = transmission.GearRatios.z;
                    break;
                case 4:
                    gearRatio = transmission.GearRatios.w;
                    break;
            }
            
            return gearRatio * transmission.FinalDriveRatio;
        }
    }
}