using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;
using Unity.Burst;
using MudLike.Vehicles.Components;
using MudLike.Core.Components;

namespace MudLike.Vehicles.Systems
{
    /// <summary>
    /// Система движения транспортного средства
    /// </summary>
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile(CompileSynchronously = true)]
    public partial class VehicleMovementSystem : SystemBase
    {
        /// <summary>
        /// Обрабатывает движение всех транспортных средств
        /// </summary>
        protected override void OnUpdate()
        {
            float deltaTime = SystemAPI.Time.fixedDeltaTime;
            
            Entities
                .WithAll<VehicleTag>()
                .ForEach((ref LocalTransform transform, 
                         ref VehiclePhysics physics, 
                         in VehicleConfig config, 
                         in VehicleInput input) =>
                {
                    ProcessVehicleMovement(ref transform, ref physics, config, input, deltaTime);
                }).Schedule();
        }
        
        /// <summary>
        /// Обрабатывает движение конкретного транспортного средства
        /// </summary>
        [BurstCompile]
        private static void ProcessVehicleMovement(ref LocalTransform transform, 
                                                 ref VehiclePhysics physics, 
                                                 in VehicleConfig config, 
                                                 in VehicleInput input, 
                                                 float deltaTime)
        {
            // Вычисляем направление движения
            float3 forward = math.forward(transform.Rotation);
            float3 right = math.right(transform.Rotation);
            
            // Применяем ввод
            float3 movementInput = forward * input.Vertical + right * input.Horizontal;
            movementInput = math.normalize(movementInput);
            
            // Вычисляем ускорение
            float3 targetVelocity = movementInput * config.MaxSpeed;
            float3 acceleration = (targetVelocity - physics.Velocity) * config.Acceleration;
            
            // Применяем сопротивление
            acceleration -= physics.Velocity * config.Drag;
            
            // Обновляем физику
            physics.Acceleration = acceleration;
            physics.Velocity += acceleration * deltaTime;
            
            // Ограничиваем скорость
            float currentSpeed = math.length(physics.Velocity);
            if (currentSpeed > config.MaxSpeed)
            {
                physics.Velocity = math.normalize(physics.Velocity) * config.MaxSpeed;
            }
            
            // Обновляем позицию
            transform.Position += physics.Velocity * deltaTime;
            
            // Вычисляем поворот
            if (math.length(input.Horizontal) > 0.1f)
            {
                float turnAngle = input.Horizontal * config.TurnSpeed * deltaTime;
                quaternion turnRotation = quaternion.RotateY(turnAngle);
                transform.Rotation = math.mul(transform.Rotation, turnRotation);
            }
            
            // Обновляем скорость движения
            physics.ForwardSpeed = math.dot(physics.Velocity, forward);
            physics.TurnSpeed = input.Horizontal;
        }
    }
}