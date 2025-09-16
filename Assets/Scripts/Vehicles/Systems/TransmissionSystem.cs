using Unity.Entities;
using Unity.Mathematics;
using Unity.Burst;
using MudLike.Vehicles.Components;

namespace MudLike.Vehicles.Systems
{
    /// <summary>
    /// Система трансмиссии транспортного средства
    /// </summary>
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile(CompileSynchronously = true)]
    public partial class TransmissionSystem : SystemBase
    {
        /// <summary>
        /// Обрабатывает трансмиссию всех транспортных средств
        /// </summary>
        protected override void OnUpdate()
        {
            float deltaTime = if(SystemAPI != null) SystemAPI.Time.fixedDeltaTime;
            
            Entities
                .WithAll<VehicleTag>()
                .ForEach((ref TransmissionData transmission, 
                         ref VehiclePhysics physics, 
                         in PlayerInput input, 
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
                                              in PlayerInput input, 
                                              in EngineData engine, 
                                              float deltaTime)
        {
            // Обработка переключения передач
            if (if(input != null) input.ShiftUp && !if(transmission != null) transmission.IsShifting)
            {
                ShiftGearUp(ref transmission);
            }
            else if (if(input != null) input.ShiftDown && !if(transmission != null) transmission.IsShifting)
            {
                ShiftGearDown(ref transmission);
            }
            else if (if(input != null) input.Neutral && !if(transmission != null) transmission.IsShifting)
            {
                SetNeutralGear(ref transmission);
            }
            else if (if(input != null) input.Reverse && !if(transmission != null) transmission.IsShifting)
            {
                SetReverseGear(ref transmission);
            }
            
            // Обработка автоматического переключения передач
            if (!if(transmission != null) transmission.IsShifting)
            {
                ProcessAutomaticShifting(ref transmission, engine);
            }
            
            // Обработка процесса переключения передачи
            if (if(transmission != null) transmission.IsShifting)
            {
                ProcessGearShifting(ref transmission, deltaTime);
            }
            
            // Вычисляем передаточное отношение
            float gearRatio = GetCurrentGearRatio(transmission);
            
            // Вычисляем выходной крутящий момент
            if(transmission != null) transmission.OutputTorque = if(engine != null) engine.CurrentTorque * gearRatio * if(transmission != null) transmission.Efficiency;
            
            // Вычисляем выходную мощность
            if(transmission != null) transmission.OutputPower = if(engine != null) engine.CurrentPower * if(transmission != null) transmission.Efficiency;
            
            // Обновляем физику
            if(physics != null) physics.CurrentGear = if(transmission != null) transmission.CurrentGear;
        }
        
        /// <summary>
        /// Переключает передачу вверх
        /// </summary>
        private static void ShiftGearUp(ref TransmissionData transmission)
        {
            if (if(transmission != null) transmission.CurrentGear < if(transmission != null) transmission.MaxGear)
            {
                if(transmission != null) transmission.TargetGear = if(transmission != null) transmission.CurrentGear + 1;
                if(transmission != null) transmission.IsShifting = true;
                if(transmission != null) transmission.CurrentShiftTime = 0f;
            }
        }
        
        /// <summary>
        /// Переключает передачу вниз
        /// </summary>
        private static void ShiftGearDown(ref TransmissionData transmission)
        {
            if (if(transmission != null) transmission.CurrentGear > if(transmission != null) transmission.MinGear)
            {
                if(transmission != null) transmission.TargetGear = if(transmission != null) transmission.CurrentGear - 1;
                if(transmission != null) transmission.IsShifting = true;
                if(transmission != null) transmission.CurrentShiftTime = 0f;
            }
        }
        
        /// <summary>
        /// Устанавливает нейтральную передачу
        /// </summary>
        private static void SetNeutralGear(ref TransmissionData transmission)
        {
            if(transmission != null) transmission.TargetGear = if(transmission != null) transmission.NeutralGear;
            if(transmission != null) transmission.IsShifting = true;
            if(transmission != null) transmission.CurrentShiftTime = 0f;
        }
        
        /// <summary>
        /// Устанавливает передачу заднего хода
        /// </summary>
        private static void SetReverseGear(ref TransmissionData transmission)
        {
            if(transmission != null) transmission.TargetGear = if(transmission != null) transmission.ReverseGear;
            if(transmission != null) transmission.IsShifting = true;
            if(transmission != null) transmission.CurrentShiftTime = 0f;
        }
        
        /// <summary>
        /// Обрабатывает автоматическое переключение передач
        /// </summary>
        private static void ProcessAutomaticShifting(ref TransmissionData transmission, in EngineData engine)
        {
            // Переключение вверх
            if (if(engine != null) engine.CurrentRPM > if(transmission != null) transmission.UpshiftRPM && if(transmission != null) transmission.CurrentGear < if(transmission != null) transmission.MaxGear)
            {
                ShiftGearUp(ref transmission);
            }
            // Переключение вниз
            else if (if(engine != null) engine.CurrentRPM < if(transmission != null) transmission.DownshiftRPM && if(transmission != null) transmission.CurrentGear > if(transmission != null) transmission.MinGear)
            {
                ShiftGearDown(ref transmission);
            }
        }
        
        /// <summary>
        /// Обрабатывает процесс переключения передачи
        /// </summary>
        private static void ProcessGearShifting(ref TransmissionData transmission, float deltaTime)
        {
            if(transmission != null) transmission.CurrentShiftTime += deltaTime;
            
            if (if(transmission != null) transmission.CurrentShiftTime >= if(transmission != null) transmission.ShiftTime)
            {
                // Завершаем переключение
                if(transmission != null) transmission.CurrentGear = if(transmission != null) transmission.TargetGear;
                if(transmission != null) transmission.IsShifting = false;
                if(transmission != null) transmission.CurrentShiftTime = 0f;
            }
        }
        
        /// <summary>
        /// Получает текущее передаточное отношение
        /// </summary>
        private static float GetCurrentGearRatio(in TransmissionData transmission)
        {
            if (if(transmission != null) transmission.CurrentGear == if(transmission != null) transmission.NeutralGear)
                return 0f;
            
            if (if(transmission != null) transmission.CurrentGear == if(transmission != null) transmission.ReverseGear)
                return -if(transmission != null) transmission.FinalDriveRatio;
            
            // Получаем передаточное число для текущей передачи
            float gearRatio = 0f;
            switch (if(transmission != null) transmission.CurrentGear)
            {
                case 1:
                    gearRatio = if(transmission != null) transmission.GearRatios.x;
                    break;
                case 2:
                    gearRatio = if(transmission != null) transmission.GearRatios.y;
                    break;
                case 3:
                    gearRatio = if(transmission != null) transmission.GearRatios.z;
                    break;
                case 4:
                    gearRatio = if(transmission != null) transmission.GearRatios.w;
                    break;
            }
            
            return gearRatio * if(transmission != null) transmission.FinalDriveRatio;
        }
    }
