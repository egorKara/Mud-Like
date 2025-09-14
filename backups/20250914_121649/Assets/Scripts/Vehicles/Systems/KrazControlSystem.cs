using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;
using Unity.InputSystem;
using MudLike.Vehicles.Components;
using MudLike.Core.Components;

namespace MudLike.Vehicles.Systems
{
    /// <summary>
    /// Система управления КРАЗ-транспортом
    /// Обрабатывает ввод игрока и управление тяжелым грузовиком
    /// </summary>
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile(CompileSynchronously = true)]
    public partial class KrazControlSystem : SystemBase
    {
        private InputAction _moveAction;
        private InputAction _brakeAction;
        private InputAction _engineAction;
        
        /// <summary>
        /// Инициализация системы
        /// </summary>
        protected override void OnCreate()
        {
            // Создаем действия ввода
            _moveAction = new InputAction("Move", InputActionType.Value, "<Gamepad>/leftStick");
            _brakeAction = new InputAction("Brake", InputActionType.Button, "<Keyboard>/space");
            _engineAction = new InputAction("Engine", InputActionType.Button, "<Keyboard>/e");
            
            // Активируем действия
            _moveAction.Enable();
            _brakeAction.Enable();
            _engineAction.Enable();
        }
        
        /// <summary>
        /// Обработка управления КРАЗом
        /// </summary>
        protected override void OnUpdate()
        {
            float deltaTime = SystemAPI.Time.fixedDeltaTime;
            
            // Получаем ввод
            var moveInput = _moveAction.ReadValue<UnityEngine.Vector2>();
            bool brakeInput = _brakeAction.IsPressed();
            bool engineInput = _engineAction.WasPressedThisFrame();
            
            // Обрабатываем все КРАЗы под управлением игрока
            Entities
                .WithAll<KrazTag, PlayerVehicleTag>()
                .ForEach((ref VehicleInput input, 
                         ref EngineData engine, 
                         ref VehiclePhysics physics, 
                         in VehicleConfig config) =>
                {
                    ProcessKrazInput(ref input, ref engine, ref physics, config, 
                                   moveInput, brakeInput, engineInput, deltaTime);
                }).Schedule();
        }
        
        /// <summary>
        /// Обрабатывает ввод для КРАЗа
        /// </summary>
        [BurstCompile]
        private static void ProcessKrazInput(ref VehicleInput input, 
                                           ref EngineData engine, 
                                           ref VehiclePhysics physics, 
                                           in VehicleConfig config,
                                           UnityEngine.Vector2 moveInput, 
                                           bool brakeInput, 
                                           bool engineInput, 
                                           float deltaTime)
        {
            // Обработка запуска/остановки двигателя
            if (engineInput)
            {
                engine.IsRunning = !engine.IsRunning;
                if (engine.IsRunning)
                {
                    engine.RPM = 800f; // Холостой ход
                }
                else
                {
                    engine.RPM = 0f;
                    input.Vertical = 0f;
                    input.Horizontal = 0f;
                }
            }
            
            // Если двигатель не запущен, не обрабатываем управление
            if (!engine.IsRunning)
            {
                input.Vertical = 0f;
                input.Horizontal = 0f;
                input.Brake = false;
                return;
            }
            
            // Обработка движения (с учетом инерции тяжелого КРАЗа)
            float targetVertical = moveInput.y;
            float targetHorizontal = moveInput.x;
            
            // Плавное изменение ввода для реалистичности
            input.Vertical = math.lerp(input.Vertical, targetVertical, deltaTime * 3f);
            input.Horizontal = math.lerp(input.Horizontal, targetHorizontal, deltaTime * 2f);
            
            // Обработка торможения
            input.Brake = brakeInput;
            
            // Обработка газа (с учетом инерции КРАЗа)
            if (input.Vertical > 0.1f)
            {
                engine.Throttle = math.min(engine.Throttle + deltaTime * 2f, 1f);
            }
            else if (input.Vertical < -0.1f)
            {
                engine.Throttle = math.max(engine.Throttle - deltaTime * 3f, 0f);
            }
            else
            {
                // Плавное снижение газа до холостого хода
                engine.Throttle = math.lerp(engine.Throttle, 0.1f, deltaTime * 1f);
            }
            
            // Обновление оборотов двигателя
            UpdateEngineRPM(ref engine, input.Vertical, deltaTime);
            
            // Обновление температуры двигателя
            UpdateEngineTemperature(ref engine, deltaTime);
        }
        
        /// <summary>
        /// Обновляет обороты двигателя
        /// </summary>
        [BurstCompile]
        private static void UpdateEngineRPM(ref EngineData engine, float throttleInput, float deltaTime)
        {
            if (!engine.IsRunning)
            {
                engine.RPM = 0f;
                return;
            }
            
            // Целевые обороты на основе газа и нагрузки
            float targetRPM = 800f + (engine.Throttle * (engine.MaxRPM - 800f));
            
            // Имитация нагрузки от трансмиссии
            if (throttleInput > 0.1f)
            {
                targetRPM -= 200f; // Снижение оборотов под нагрузкой
            }
            
            // Плавное изменение оборотов
            engine.RPM = math.lerp(engine.RPM, targetRPM, deltaTime * 2f);
            
            // Ограничение оборотов
            engine.RPM = math.clamp(engine.RPM, 0f, engine.MaxRPM);
        }
        
        /// <summary>
        /// Обновляет температуру двигателя
        /// </summary>
        [BurstCompile]
        private static void UpdateEngineTemperature(ref EngineData engine, float deltaTime)
        {
            if (!engine.IsRunning)
            {
                // Остывание двигателя
                engine.Temperature = math.lerp(engine.Temperature, 20f, deltaTime * 0.5f);
                return;
            }
            
            // Нагрев от работы двигателя
            float heatGeneration = engine.RPM / engine.MaxRPM * 0.8f;
            float cooling = 0.3f; // Естественное охлаждение
            
            engine.Temperature += (heatGeneration - cooling) * deltaTime;
            engine.Temperature = math.clamp(engine.Temperature, 20f, engine.MaxTemperature);
            
            // Перегрев двигателя
            if (engine.Temperature > engine.MaxTemperature * 0.9f)
            {
                // Снижение мощности при перегреве
                engine.Power *= 0.8f;
            }
        }
        
        /// <summary>
        /// Очистка ресурсов при уничтожении системы
        /// </summary>
        protected override void OnDestroy()
        {
            _moveAction?.Disable();
            _brakeAction?.Disable();
            _engineAction?.Disable();
            
            _moveAction?.Dispose();
            _brakeAction?.Dispose();
            _engineAction?.Dispose();
        }
    }
}
