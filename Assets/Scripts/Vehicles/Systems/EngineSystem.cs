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
            float deltaTime = SystemAPI.Time.fixedDeltaTime;
            
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
            if (input.EngineToggle)
            {
                engine.IsRunning = !engine.IsRunning;
                if (engine.IsRunning)
                {
                    engine.IsStarting = true;
                    engine.TimeSinceStart = 0f;
                }
            }
            
            // Обработка запуска двигателя
            if (engine.IsStarting)
            {
                engine.TimeSinceStart += deltaTime;
                if (engine.TimeSinceStart >= 2f) // 2 секунды на запуск
                {
                    engine.IsStarting = false;
                    engine.IsStalled = false;
                }
            }
            
            // Обработка работы двигателя
            if (engine.IsRunning && !engine.IsStarting)
            {
                // Обновляем время работы
                engine.RunningTime += deltaTime;
                
                // Вычисляем положение дроссельной заслонки
                engine.ThrottlePosition = input.Vertical;
                engine.GasPedal = input.Vertical;
                
                // Вычисляем целевые обороты
                float targetRPM = CalculateTargetRPM(engine, input, config);
                engine.TargetRPM = targetRPM;
                
                // Плавное изменение оборотов
                float rpmChange = (targetRPM - engine.CurrentRPM) * engine.RPMSpeed * deltaTime;
                engine.CurrentRPM += rpmChange;
                
                // Ограничиваем обороты
                engine.CurrentRPM = math.clamp(engine.CurrentRPM, engine.MinRPM, engine.MaxRPM);
                
                // Вычисляем мощность и крутящий момент
                engine.CurrentPower = CalculateEnginePower(engine);
                engine.CurrentTorque = CalculateEngineTorque(engine);
                
                // Обновляем физику
                physics.EngineRPM = engine.CurrentRPM;
                physics.EnginePower = engine.CurrentPower;
                physics.EngineTorque = engine.CurrentTorque;
                
                // Вычисляем расход топлива
                engine.FuelConsumption = CalculateFuelConsumption(engine);
                engine.FuelLevel -= engine.FuelConsumption * deltaTime;
                
                // Проверяем уровень топлива
                if (engine.FuelLevel <= 0f)
                {
                    engine.IsRunning = false;
                    engine.IsStalled = true;
                }
                
                // Обновляем температуру
                UpdateEngineTemperature(ref engine, deltaTime);
            }
            else
            {
                // Двигатель выключен - снижаем обороты до холостого хода
                engine.TargetRPM = engine.IdleRPM;
                float rpmChange = (engine.IdleRPM - engine.CurrentRPM) * engine.RPMSpeed * 0.5f * deltaTime;
                engine.CurrentRPM += rpmChange;
                
                // Обнуляем мощность и крутящий момент
                engine.CurrentPower = 0f;
                engine.CurrentTorque = 0f;
                physics.EngineRPM = engine.CurrentRPM;
                physics.EnginePower = 0f;
                physics.EngineTorque = 0f;
            }
        }
        
        /// <summary>
        /// Вычисляет целевые обороты двигателя
        /// </summary>
        private static float CalculateTargetRPM(in EngineData engine, in VehicleInput input, in VehicleConfig config)
        {
            if (!engine.IsRunning)
                return engine.IdleRPM;
            
            // Базовые обороты
            float baseRPM = engine.IdleRPM;
            
            // Влияние педали газа
            float throttleRPM = (engine.MaxRPM - engine.IdleRPM) * input.Vertical;
            
            // Влияние скорости движения
            float speedRPM = math.length(physics.Velocity) * 10f; // Простая формула
            
            return baseRPM + throttleRPM + speedRPM;
        }
        
        /// <summary>
        /// Вычисляет мощность двигателя
        /// </summary>
        private static float CalculateEnginePower(in EngineData engine)
        {
            if (!engine.IsRunning)
                return 0f;
            
            // Используем кривую мощности
            float normalizedRPM = (engine.CurrentRPM - engine.MinRPM) / (engine.MaxRPM - engine.MinRPM);
            float powerCurve = math.lerp(engine.PowerCurve.x, engine.PowerCurve.y, normalizedRPM);
            
            return engine.MaxPower * powerCurve * engine.ThrottlePosition;
        }
        
        /// <summary>
        /// Вычисляет крутящий момент двигателя
        /// </summary>
        private static float CalculateEngineTorque(in EngineData engine)
        {
            if (!engine.IsRunning)
                return 0f;
            
            // Используем кривую крутящего момента
            float normalizedRPM = (engine.CurrentRPM - engine.MinRPM) / (engine.MaxRPM - engine.MinRPM);
            float torqueCurve = math.lerp(engine.TorqueCurve.x, engine.TorqueCurve.y, normalizedRPM);
            
            return engine.MaxTorque * torqueCurve * engine.ThrottlePosition;
        }
        
        /// <summary>
        /// Вычисляет расход топлива
        /// </summary>
        private static float CalculateFuelConsumption(in EngineData engine)
        {
            if (!engine.IsRunning)
                return 0f;
            
            // Базовый расход на холостом ходу
            float baseConsumption = 0.1f;
            
            // Дополнительный расход от оборотов
            float rpmConsumption = (engine.CurrentRPM - engine.IdleRPM) * 0.001f;
            
            // Дополнительный расход от дроссельной заслонки
            float throttleConsumption = engine.ThrottlePosition * 0.5f;
            
            return baseConsumption + rpmConsumption + throttleConsumption;
        }
        
        /// <summary>
        /// Обновляет температуру двигателя
        /// </summary>
        private static void UpdateEngineTemperature(ref EngineData engine, float deltaTime)
        {
            if (!engine.IsRunning)
            {
                // Охлаждение при выключенном двигателе
                engine.Temperature -= 10f * deltaTime;
            }
            else
            {
                // Нагрев при работе
                float heatGeneration = engine.CurrentRPM * 0.01f + engine.ThrottlePosition * 0.1f;
                engine.Temperature += heatGeneration * deltaTime;
            }
            
            // Ограничиваем температуру
            engine.Temperature = math.clamp(engine.Temperature, 20f, engine.MaxTemperature);
        }
    }
}