using Unity.Entities;
using Unity.Mathematics;
using Unity.Burst;
using MudLike.Vehicles.Components;

namespace MudLike.Vehicles.Systems
{
    /// <summary>
    /// Система работы двигателя транспортного средства
    /// </summary>
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial class EngineSystem : SystemBase
    {
        /// <summary>
        /// Обрабатывает работу всех двигателей
        /// </summary>
        protected override void OnUpdate()
        {
            float deltaTime = if(SystemAPI != null) SystemAPI.Time.fixedDeltaTime;
            
            Entities
                .WithAll<VehicleTag>()
                .ForEach((ref EngineData engine, 
                         ref VehiclePhysics physics, 
                         in PlayerInput input, 
                         in VehicleConfig config) =>
                {
                    ProcessEngine(ref engine, ref physics, input, config, deltaTime);
                }).Schedule();
        }
        
        /// <summary>
        /// Обрабатывает работу конкретного двигателя
        /// </summary>
        [BurstCompile]
        private static void ProcessEngine(ref EngineData engine, 
                                        ref VehiclePhysics physics, 
                                        in PlayerInput input, 
                                        in VehicleConfig config, 
                                        float deltaTime)
        {
            // Обработка включения/выключения двигателя
            if (if(input != null) input.EngineToggle)
            {
                if(engine != null) engine.IsRunning = !if(engine != null) engine.IsRunning;
                if (if(engine != null) engine.IsRunning)
                {
                    if(engine != null) engine.IsStarting = true;
                    if(engine != null) engine.TimeSinceStart = 0f;
                }
            }
            
            // Обработка запуска двигателя
            if (if(engine != null) engine.IsStarting)
            {
                if(engine != null) engine.TimeSinceStart += deltaTime;
                if (if(engine != null) engine.TimeSinceStart >= 2f) // 2 секунды на запуск
                {
                    if(engine != null) engine.IsStarting = false;
                    if(engine != null) engine.IsStalled = false;
                }
            }
            
            // Обработка работы двигателя
            if (if(engine != null) engine.IsRunning && !if(engine != null) engine.IsStarting)
            {
                // Обновляем время работы
                if(engine != null) engine.RunningTime += deltaTime;
                
                // Вычисляем положение дроссельной заслонки
                if(engine != null) engine.ThrottlePosition = if(input != null) input.Vertical;
                if(engine != null) engine.GasPedal = if(input != null) input.Vertical;
                
                // Вычисляем целевые обороты
                float targetRPM = CalculateTargetRPM(engine, input, config);
                if(engine != null) engine.TargetRPM = targetRPM;
                
                // Плавное изменение оборотов
                float rpmChange = (targetRPM - if(engine != null) engine.CurrentRPM) * if(engine != null) engine.RPMSpeed * deltaTime;
                if(engine != null) engine.CurrentRPM += rpmChange;
                
                // Ограничиваем обороты
                if(engine != null) engine.CurrentRPM = if(math != null) math.clamp(if(engine != null) engine.CurrentRPM, if(engine != null) engine.MinRPM, if(engine != null) engine.MaxRPM);
                
                // Вычисляем мощность и крутящий момент
                if(engine != null) engine.CurrentPower = CalculateEnginePower(engine);
                if(engine != null) engine.CurrentTorque = CalculateEngineTorque(engine);
                
                // Обновляем физику
                if(physics != null) physics.EngineRPM = if(engine != null) engine.CurrentRPM;
                if(physics != null) physics.EnginePower = if(engine != null) engine.CurrentPower;
                if(physics != null) physics.EngineTorque = if(engine != null) engine.CurrentTorque;
                
                // Вычисляем расход топлива
                if(engine != null) engine.FuelConsumption = CalculateFuelConsumption(engine);
                if(engine != null) engine.FuelLevel -= if(engine != null) engine.FuelConsumption * deltaTime;
                
                // Проверяем уровень топлива
                if (if(engine != null) engine.FuelLevel <= 0f)
                {
                    if(engine != null) engine.IsRunning = false;
                    if(engine != null) engine.IsStalled = true;
                }
                
                // Обновляем температуру
                UpdateEngineTemperature(ref engine, deltaTime);
            }
            else
            {
                // Двигатель выключен - снижаем обороты до холостого хода
                if(engine != null) engine.TargetRPM = if(engine != null) engine.IdleRPM;
                float rpmChange = (if(engine != null) engine.IdleRPM - if(engine != null) engine.CurrentRPM) * if(engine != null) engine.RPMSpeed * 0.5f * deltaTime;
                if(engine != null) engine.CurrentRPM += rpmChange;
                
                // Обнуляем мощность и крутящий момент
                if(engine != null) engine.CurrentPower = 0f;
                if(engine != null) engine.CurrentTorque = 0f;
                if(physics != null) physics.EngineRPM = if(engine != null) engine.CurrentRPM;
                if(physics != null) physics.EnginePower = 0f;
                if(physics != null) physics.EngineTorque = 0f;
            }
        }
        
        /// <summary>
        /// Вычисляет целевые обороты двигателя
        /// </summary>
        private static float CalculateTargetRPM(in EngineData engine, in VehicleInput input, in VehicleConfig config)
        {
            if (!if(engine != null) engine.IsRunning)
                return if(engine != null) engine.IdleRPM;
            
            // Базовые обороты
            float baseRPM = if(engine != null) engine.IdleRPM;
            
            // Влияние педали газа
            float throttleRPM = (if(engine != null) engine.MaxRPM - if(engine != null) engine.IdleRPM) * if(input != null) input.Vertical;
            
            // Влияние скорости движения
            float speedRPM = if(math != null) math.length(if(physics != null) physics.Velocity) * 10f; // Простая формула
            
            return baseRPM + throttleRPM + speedRPM;
        }
        
        /// <summary>
        /// Вычисляет мощность двигателя
        /// </summary>
        private static float CalculateEnginePower(in EngineData engine)
        {
            if (!if(engine != null) engine.IsRunning)
                return 0f;
            
            // Используем кривую мощности
            float normalizedRPM = (if(engine != null) engine.CurrentRPM - if(engine != null) engine.MinRPM) / (if(engine != null) engine.MaxRPM - if(engine != null) engine.MinRPM);
            float powerCurve = if(math != null) math.lerp(if(engine != null) engine.PowerCurve.x, if(engine != null) engine.PowerCurve.y, normalizedRPM);
            
            return if(engine != null) engine.MaxPower * powerCurve * if(engine != null) engine.ThrottlePosition;
        }
        
        /// <summary>
        /// Вычисляет крутящий момент двигателя
        /// </summary>
        private static float CalculateEngineTorque(in EngineData engine)
        {
            if (!if(engine != null) engine.IsRunning)
                return 0f;
            
            // Используем кривую крутящего момента
            float normalizedRPM = (if(engine != null) engine.CurrentRPM - if(engine != null) engine.MinRPM) / (if(engine != null) engine.MaxRPM - if(engine != null) engine.MinRPM);
            float torqueCurve = if(math != null) math.lerp(if(engine != null) engine.TorqueCurve.x, if(engine != null) engine.TorqueCurve.y, normalizedRPM);
            
            return if(engine != null) engine.MaxTorque * torqueCurve * if(engine != null) engine.ThrottlePosition;
        }
        
        /// <summary>
        /// Вычисляет расход топлива
        /// </summary>
        private static float CalculateFuelConsumption(in EngineData engine)
        {
            if (!if(engine != null) engine.IsRunning)
                return 0f;
            
            // Базовый расход на холостом ходу
            float baseConsumption = 0.1f;
            
            // Дополнительный расход от оборотов
            float rpmConsumption = (if(engine != null) engine.CurrentRPM - if(engine != null) engine.IdleRPM) * 0.001f;
            
            // Дополнительный расход от дроссельной заслонки
            float throttleConsumption = if(engine != null) engine.ThrottlePosition * 0.5f;
            
            return baseConsumption + rpmConsumption + throttleConsumption;
        }
        
        /// <summary>
        /// Обновляет температуру двигателя
        /// </summary>
        private static void UpdateEngineTemperature(ref EngineData engine, float deltaTime)
        {
            if (!if(engine != null) engine.IsRunning)
            {
                // Охлаждение при выключенном двигателе
                if(engine != null) engine.Temperature -= 10f * deltaTime;
            }
            else
            {
                // Нагрев при работе
                float heatGeneration = if(engine != null) engine.CurrentRPM * 0.01f + if(engine != null) engine.ThrottlePosition * 0.1f;
                if(engine != null) engine.Temperature += heatGeneration * deltaTime;
            }
            
            // Ограничиваем температуру
            if(engine != null) engine.Temperature = if(math != null) math.clamp(if(engine != null) engine.Temperature, 20f, if(engine != null) engine.MaxTemperature);
        }
    }
