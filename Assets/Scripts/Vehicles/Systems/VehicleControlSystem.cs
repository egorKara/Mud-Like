using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using MudLike.Vehicles.Components;

namespace MudLike.Vehicles.Systems
{
    /// <summary>
    /// ECS система для управления транспортным средством
    /// </summary>
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial class VehicleControlSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            float deltaTime = SystemAPI.Time.fixedDeltaTime;
            
            // Обновление управления транспортным средством
            Entities.ForEach((ref VehicleComponent vehicle, ref VehicleControlComponent control, 
                in LocalTransform transform) =>
            {
                // Обработка ввода управления
                ProcessControlInput(ref control, deltaTime);
                
                // Обновление двигателя
                UpdateEngine(ref vehicle, ref control, deltaTime);
                
                // Обновление передач
                UpdateTransmission(ref vehicle, ref control, deltaTime);
                
                // Обновление тормозов
                UpdateBrakes(ref vehicle, ref control, deltaTime);
                
                // Обновление рулевого управления
                UpdateSteering(ref vehicle, ref control, deltaTime);
                
                // Расчет сил тяги
                CalculateTractionForces(ref vehicle, ref control, deltaTime);
                
            }).Schedule();
            
            // Обновление колес на основе управления
            UpdateWheelsFromControl(deltaTime);
        }
        
        /// <summary>
        /// Обработка ввода управления
        /// </summary>
        private static void ProcessControlInput(ref VehicleControlComponent control, float deltaTime)
        {
            // Здесь должна быть логика получения ввода от игрока
            // Пока используем заглушку
            
            control.lastInputTime = 0f; // Временная заглушка
            control.isControlActive = true;
        }
        
        /// <summary>
        /// Обновление двигателя
        /// </summary>
        private static void UpdateEngine(ref VehicleComponent vehicle, ref VehicleControlComponent control, float deltaTime)
        {
            // Включение/выключение двигателя
            if (control.engineToggleInput)
            {
                vehicle.engineOn = !vehicle.engineOn;
                control.engineToggleInput = false;
            }
            
            if (!vehicle.engineOn)
            {
                vehicle.enginePower = 0f;
                vehicle.engineTorque = 0f;
                vehicle.engineRPM = 0f;
                return;
            }
            
            // Расчет мощности двигателя на основе ввода газа
            float targetPower = control.throttleInput * vehicle.maxEnginePower;
            vehicle.enginePower = math.lerp(vehicle.enginePower, targetPower, deltaTime * 5f);
            
            // Расчет крутящего момента
            float targetTorque = control.throttleInput * vehicle.maxEngineTorque;
            vehicle.engineTorque = math.lerp(vehicle.engineTorque, targetTorque, deltaTime * 5f);
            
            // Расчет оборотов двигателя
            float targetRPM = control.throttleInput * vehicle.maxEngineRPM;
            vehicle.engineRPM = math.lerp(vehicle.engineRPM, targetRPM, deltaTime * 3f);
        }
        
        /// <summary>
        /// Обновление передач
        /// </summary>
        private static void UpdateTransmission(ref VehicleComponent vehicle, ref VehicleControlComponent control, float deltaTime)
        {
            // Переключение передач
            if (control.gearUpInput && vehicle.gear < vehicle.gearCount)
            {
                vehicle.gear++;
                control.gearUpInput = false;
            }
            
            if (control.gearDownInput && vehicle.gear > 1)
            {
                vehicle.gear--;
                control.gearDownInput = false;
            }
            
            // Расчет передаточного отношения
            vehicle.gearRatio = GetGearRatio(vehicle.gear);
        }
        
        /// <summary>
        /// Получение передаточного отношения для передачи
        /// </summary>
        private static float GetGearRatio(int gear)
        {
            switch (gear)
            {
                case 1: return 3.5f;  // Первая передача
                case 2: return 2.1f;  // Вторая передача
                case 3: return 1.4f;  // Третья передача
                case 4: return 1.0f;  // Четвертая передача
                case 5: return 0.8f;  // Пятая передача
                case 6: return 0.6f;  // Шестая передача
                default: return 1.0f;
            }
        }
        
        /// <summary>
        /// Обновление тормозов
        /// </summary>
        private static void UpdateBrakes(ref VehicleComponent vehicle, ref VehicleControlComponent control, float deltaTime)
        {
            // Обновление ручного тормоза
            vehicle.handbrakeOn = control.handbrakeInput > 0.1f;
            
            // Расчет общей силы торможения
            float brakeForce = control.brakeInput * vehicle.maxEnginePower * 0.5f;
            if (vehicle.handbrakeOn)
            {
                brakeForce += control.handbrakeInput * vehicle.maxEnginePower * 0.3f;
            }
            
            vehicle.totalBrakeForce = new float3(0, 0, brakeForce);
        }
        
        /// <summary>
        /// Обновление рулевого управления
        /// </summary>
        private static void UpdateSteering(ref VehicleComponent vehicle, ref VehicleControlComponent control, float deltaTime)
        {
            // Обновление полного привода
            vehicle.fourWheelDrive = control.fourWheelDriveInput;
            
            // Обновление блокировки дифференциала
            vehicle.lockedDifferential = control.differentialLockInput;
        }
        
        /// <summary>
        /// Расчет сил тяги
        /// </summary>
        private static void CalculateTractionForces(ref VehicleComponent vehicle, ref VehicleControlComponent control, float deltaTime)
        {
            if (!vehicle.engineOn)
            {
                vehicle.totalTractionForce = float3.zero;
                return;
            }
            
            // Расчет силы тяги на основе мощности двигателя и передач
            float tractionForce = vehicle.enginePower * vehicle.gearRatio * vehicle.finalDriveRatio;
            
            // Применение ввода газа
            tractionForce *= control.throttleInput;
            
            // Направление силы тяги (вперед)
            vehicle.totalTractionForce = new float3(0, 0, tractionForce);
        }
        
        /// <summary>
        /// Обновление колес на основе управления
        /// </summary>
        private void UpdateWheelsFromControl(float deltaTime)
        {
            // Здесь должна быть логика обновления колес на основе управления
            // Пока используем заглушку
        }
    }
}
