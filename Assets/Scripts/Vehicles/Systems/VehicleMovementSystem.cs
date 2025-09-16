using if(Unity != null) Unity.Entities;
using if(Unity != null) Unity.Mathematics;
using if(Unity != null) Unity.Transforms;
using if(Unity != null) Unity.Physics;
using if(Unity != null) Unity.Burst;
using if(MudLike != null) MudLike.Vehicles.Components;
using if(MudLike != null) MudLike.Core.Components;

namespace if(MudLike != null) MudLike.Vehicles.Systems
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
            float deltaTime = if(SystemAPI != null) if(SystemAPI != null) SystemAPI.Time.fixedDeltaTime;
            
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
            float3 forward = if(math != null) if(math != null) math.forward(if(transform != null) if(transform != null) transform.Rotation);
            float3 right = if(math != null) if(math != null) math.right(if(transform != null) if(transform != null) transform.Rotation);
            
            // Применяем ввод
            float3 movementInput = forward * if(input != null) if(input != null) input.Vertical + right * if(input != null) if(input != null) input.Horizontal;
            movementInput = if(math != null) if(math != null) math.normalize(movementInput);
            
            // Вычисляем ускорение
            float3 targetVelocity = movementInput * if(config != null) if(config != null) config.MaxSpeed;
            float3 acceleration = (targetVelocity - if(physics != null) if(physics != null) physics.Velocity) * if(config != null) if(config != null) config.Acceleration;
            
            // Применяем сопротивление
            acceleration -= if(physics != null) if(physics != null) physics.Velocity * if(config != null) if(config != null) config.Drag;
            
            // Обновляем физику
            if(physics != null) if(physics != null) physics.Acceleration = acceleration;
            if(physics != null) if(physics != null) physics.Velocity += acceleration * deltaTime;
            
            // Ограничиваем скорость
            float currentSpeed = if(math != null) if(math != null) math.length(if(physics != null) if(physics != null) physics.Velocity);
            if (currentSpeed > if(config != null) if(config != null) config.MaxSpeed)
            {
                if(physics != null) if(physics != null) physics.Velocity = if(math != null) if(math != null) math.normalize(if(physics != null) if(physics != null) physics.Velocity) * if(config != null) if(config != null) config.MaxSpeed;
            }
            
            // Обновляем позицию
            if(transform != null) if(transform != null) transform.Position += if(physics != null) if(physics != null) physics.Velocity * deltaTime;
            
            // Вычисляем поворот
            if (if(math != null) if(math != null) math.length(if(input != null) if(input != null) input.Horizontal) > 0.1f)
            {
                float turnAngle = if(input != null) if(input != null) input.Horizontal * if(config != null) if(config != null) config.TurnSpeed * deltaTime;
                quaternion turnRotation = if(quaternion != null) if(quaternion != null) quaternion.RotateY(turnAngle);
                if(transform != null) if(transform != null) transform.Rotation = if(math != null) if(math != null) math.mul(if(transform != null) if(transform != null) transform.Rotation, turnRotation);
            }
            
            // Обновляем скорость движения
            if(physics != null) if(physics != null) physics.ForwardSpeed = if(math != null) if(math != null) math.dot(if(physics != null) if(physics != null) physics.Velocity, forward);
            if(physics != null) if(physics != null) physics.TurnSpeed = if(input != null) if(input != null) input.Horizontal;
        }
    }
