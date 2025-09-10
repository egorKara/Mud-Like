using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;
using Unity.Jobs;
using Unity.Collections;
using MudLike.Vehicles.Components;

namespace MudLike.Vehicles.Systems
{
    /// <summary>
    /// Система движения грузовика КРАЗ
    /// Оптимизирована с использованием Burst Compiler и Job System
    /// </summary>
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile]
    public partial class TruckMovementSystem : SystemBase
    {
        /// <summary>
        /// Обрабатывает движение грузовика
        /// </summary>
        protected override void OnUpdate()
        {
            float deltaTime = SystemAPI.Time.fixedDeltaTime;
            
            Entities
                .WithAll<TruckData>()
                .ForEach((ref LocalTransform transform, ref TruckData truck, in TruckControl input) =>
                {
                    ProcessTruckMovement(ref transform, ref truck, input, deltaTime);
                }).Schedule();
        }
        
        /// <summary>
        /// Обрабатывает движение конкретного грузовика
        /// </summary>
        private static void ProcessTruckMovement(ref LocalTransform transform, ref TruckData truck, in TruckControl input, float deltaTime)
        {
            // Обновляем состояние двигателя
            UpdateEngine(ref truck, input, deltaTime);
            
            // Обновляем трансмиссию
            UpdateTransmission(ref truck, input, deltaTime);
            
            // Вычисляем силу тяги
            float3 tractionForce = CalculateTractionForce(truck, input);
            
            // Применяем физику движения
            ApplyPhysics(ref transform, ref truck, tractionForce, deltaTime);
        }
        
        /// <summary>
        /// Обновляет состояние двигателя
        /// </summary>
        private static void UpdateEngine(ref TruckData truck, in TruckControl input, float deltaTime)
        {
            // Переключение двигателя
            if (input.ToggleEngine)
            {
                truck.EngineRunning = !truck.EngineRunning;
            }
            
            if (!truck.EngineRunning)
            {
                truck.EngineRPM = 0f;
                return;
            }
            
            // Вычисляем обороты двигателя
            float targetRPM = CalculateTargetRPM(truck, input);
            truck.EngineRPM = math.lerp(truck.EngineRPM, targetRPM, deltaTime * 2f);
            
            // Ограничиваем обороты
            truck.EngineRPM = math.clamp(truck.EngineRPM, 800f, 2500f);
        }
        
        /// <summary>
        /// Обновляет трансмиссию
        /// </summary>
        private static void UpdateTransmission(ref TruckData truck, in TruckControl input, float deltaTime)
        {
            // Переключение передач
            if (input.ShiftUp && truck.CurrentGear < 6)
            {
                truck.CurrentGear++;
            }
            else if (input.ShiftDown && truck.CurrentGear > 1)
            {
                truck.CurrentGear--;
            }
            
            // Обновляем блокировки дифференциалов
            truck.LockFrontDifferential = input.LockFrontDifferential;
            truck.LockMiddleDifferential = input.LockMiddleDifferential;
            truck.LockRearDifferential = input.LockRearDifferential;
            truck.LockCenterDifferential = input.LockCenterDifferential;
        }
        
        /// <summary>
        /// Вычисляет целевую частоту вращения двигателя
        /// </summary>
        private static float CalculateTargetRPM(TruckData truck, in TruckControl input)
        {
            if (input.Throttle <= 0f)
            {
                return 800f; // Холостой ход
            }
            
            // Базовые обороты + влияние газа
            float baseRPM = 1000f;
            float throttleRPM = input.Throttle * 1500f;
            
            return baseRPM + throttleRPM;
        }
        
        /// <summary>
        /// Вычисляет силу тяги
        /// </summary>
        private static float3 CalculateTractionForce(TruckData truck, in TruckControl input)
        {
            if (!truck.EngineRunning || truck.HandbrakeOn)
            {
                return float3.zero;
            }
            
            // Вычисляем крутящий момент двигателя
            float engineTorque = CalculateEngineTorque(truck, input);
            
            // Применяем передаточное число
            float gearRatio = GetGearRatio(truck.CurrentGear);
            float wheelTorque = engineTorque * gearRatio * truck.TractionCoefficient;
            
            // Преобразуем в силу тяги
            float tractionForce = wheelTorque / (truck.Mass * 0.5f); // Упрощенная формула
            
            // Направление движения
            float3 forward = math.forward(quaternion.identity);
            return forward * tractionForce * input.Throttle;
        }
        
        /// <summary>
        /// Вычисляет крутящий момент двигателя
        /// </summary>
        private static float CalculateEngineTorque(TruckData truck, in TruckControl input)
        {
            // Кривая крутящего момента (упрощенная)
            float rpmFactor = truck.EngineRPM / 2000f;
            float torqueFactor = math.sin(rpmFactor * math.PI) * 0.8f + 0.2f;
            
            return truck.MaxTorque * torqueFactor * input.Throttle;
        }
        
        /// <summary>
        /// Получает передаточное число для передачи
        /// </summary>
        private static float GetGearRatio(int gear)
        {
            switch (gear)
            {
                case 1: return 3.5f;
                case 2: return 2.1f;
                case 3: return 1.4f;
                case 4: return 1.0f;
                case 5: return 0.8f;
                case 6: return 0.6f;
                default: return 0f;
            }
        }
        
        /// <summary>
        /// Применяет физику движения
        /// </summary>
        private static void ApplyPhysics(ref LocalTransform transform, ref TruckData truck, float3 tractionForce, float deltaTime)
        {
            // Вычисляем ускорение
            float3 acceleration = tractionForce / truck.Mass;
            
            // Обновляем скорость
            float3 velocity = math.forward(transform.Rotation) * truck.CurrentSpeed / 3.6f; // км/ч в м/с
            velocity += acceleration * deltaTime;
            
            // Применяем сопротивление воздуха
            float airResistance = 0.5f * truck.CurrentSpeed * truck.CurrentSpeed * 0.01f;
            velocity *= (1f - airResistance * deltaTime);
            
            // Обновляем позицию
            transform.Position += velocity * deltaTime;
            
            // Обновляем скорость в км/ч
            truck.CurrentSpeed = math.length(velocity) * 3.6f;
            
            // Ограничиваем максимальную скорость
            truck.CurrentSpeed = math.min(truck.CurrentSpeed, truck.MaxSpeed);
        }
    }
}