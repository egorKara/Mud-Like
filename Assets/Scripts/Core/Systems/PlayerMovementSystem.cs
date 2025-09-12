using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;
using MudLike.Core.Components;

namespace MudLike.Core.Systems
{
    /// <summary>
    /// Система управления транспортом игрока в ECS архитектуре
    /// Игрок управляет только транспортом, не ходит пешком
    /// </summary>
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile]
    public partial class VehicleControlSystem : SystemBase
    {
        /// <summary>
        /// Обрабатывает управление транспортом всех игроков
        /// </summary>
        protected override void OnUpdate()
        {
            float deltaTime = SystemAPI.Time.fixedDeltaTime;
            
            Entities
                .WithAll<VehicleTag, PlayerTag>()
                .ForEach((ref LocalTransform transform, in PlayerInput input, ref VehiclePhysics physics) =>
                {
                    ProcessVehicleControl(ref transform, input, ref physics, deltaTime);
                }).Schedule();
        }
        
        /// <summary>
        /// Обрабатывает управление транспортом конкретного игрока
        /// </summary>
        /// <param name="transform">Трансформация транспорта</param>
        /// <param name="input">Ввод управления транспортом</param>
        /// <param name="physics">Физика транспорта</param>
        /// <param name="deltaTime">Время с последнего обновления</param>
        [BurstCompile]
        private static void ProcessVehicleControl(ref LocalTransform transform, in PlayerInput input, ref VehiclePhysics physics, float deltaTime)
        {
            // Применяем ввод к физике транспорта
            ApplyVehicleInput(ref physics, input, deltaTime);
            
            // Обновляем позицию транспорта через физику
            transform.Position += physics.Velocity * deltaTime;
            transform.Rotation = physics.Rotation;
        }
        
        /// <summary>
        /// Применяет ввод к физике транспорта
        /// </summary>
        /// <param name="physics">Физика транспорта</param>
        /// <param name="input">Ввод управления транспортом</param>
        /// <param name="deltaTime">Время с последнего обновления</param>
        [BurstCompile]
        private static void ApplyVehicleInput(ref VehiclePhysics physics, in PlayerInput input, float deltaTime)
        {
            // Применяем управление двигателем
            if (input.Accelerate)
            {
                physics.EnginePower = math.min(physics.EnginePower + physics.Acceleration * deltaTime, physics.MaxEnginePower);
            }
            else if (input.Brake)
            {
                physics.EnginePower = math.max(physics.EnginePower - physics.Deceleration * deltaTime, -physics.MaxEnginePower * 0.5f);
            }
            else
            {
                // Автоматическое торможение двигателем
                physics.EnginePower = math.lerp(physics.EnginePower, 0f, physics.EngineBraking * deltaTime);
            }
            
            // Применяем управление рулем
            if (math.abs(input.Steering) > 0.1f)
            {
                float steeringAngle = input.Steering * physics.MaxSteeringAngle * deltaTime;
                physics.SteeringAngle = math.clamp(physics.SteeringAngle + steeringAngle, -physics.MaxSteeringAngle, physics.MaxSteeringAngle);
            }
            else
            {
                // Возврат руля в нейтральное положение
                physics.SteeringAngle = math.lerp(physics.SteeringAngle, 0f, physics.SteeringReturnSpeed * deltaTime);
            }
            
            // Обновляем скорость на основе мощности двигателя
            float targetSpeed = physics.EnginePower * physics.MaxSpeed;
            physics.Velocity.x = math.lerp(physics.Velocity.x, targetSpeed, physics.Acceleration * deltaTime);
            
            // Применяем поворот
            if (math.abs(physics.SteeringAngle) > 0.1f && math.abs(physics.Velocity.x) > 0.1f)
            {
                float turnSpeed = physics.SteeringAngle * physics.Velocity.x * physics.TurnSpeedMultiplier;
                physics.Rotation = quaternion.RotateY(turnSpeed * deltaTime);
            }
        }
    }
}
