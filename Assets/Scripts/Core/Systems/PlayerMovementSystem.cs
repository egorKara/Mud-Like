using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using MudLike.Core.Components;

namespace MudLike.Core.Systems
{
    /// <summary>
    /// Система движения игрока в ECS архитектуре
    /// </summary>
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial class PlayerMovementSystem : SystemBase
    {
        /// <summary>
        /// Обрабатывает движение всех игроков
        /// </summary>
        protected override void OnUpdate()
        {
            float deltaTime = SystemAPI.Time.fixedDeltaTime;

            Entities
                .WithAll<PlayerTag>()
                .ForEach((ref LocalTransform transform, ref Velocity velocity, 
                         in PlayerInput input, in MovementSpeed movementSpeed) =>
                {
                    ProcessMovement(ref transform, ref velocity, input, movementSpeed, deltaTime);
                }).Schedule();
        }

        /// <summary>
        /// Обрабатывает движение конкретного игрока
        /// </summary>
        /// <param name="transform">Трансформация игрока</param>
        /// <param name="velocity">Скорость игрока</param>
        /// <param name="input">Ввод игрока</param>
        /// <param name="movementSpeed">Параметры скорости движения</param>
        /// <param name="deltaTime">Время с последнего обновления</param>
        private static void ProcessMovement(ref LocalTransform transform, ref Velocity velocity,
                                          in PlayerInput input, in MovementSpeed movementSpeed, float deltaTime)
        {
            // Вычисляем желаемое направление движения
            float3 desiredDirection = CalculateMovementDirection(input);
            
            // Вычисляем желаемую скорость
            float3 desiredVelocity = desiredDirection * movementSpeed.MaxSpeed;
            
            // Применяем ускорение или торможение
            if (math.length(desiredDirection) > 0.1f)
            {
                // Ускорение
                velocity.Value = math.lerp(velocity.Value, desiredVelocity, 
                                         movementSpeed.Acceleration * deltaTime);
            }
            else
            {
                // Торможение
                velocity.Value = math.lerp(velocity.Value, float3.zero, 
                                         movementSpeed.Deceleration * deltaTime);
            }
            
            // Применяем движение
            transform.Position += velocity.Value * deltaTime;
            
            // Поворот в направлении движения
            if (math.length(desiredDirection) > 0.1f)
            {
                quaternion targetRotation = quaternion.LookRotation(desiredDirection, math.up());
                transform.Rotation = math.slerp(transform.Rotation, targetRotation, 
                                              movementSpeed.Value * deltaTime);
            }
        }

        /// <summary>
        /// Вычисляет направление движения на основе ввода
        /// </summary>
        /// <param name="input">Ввод игрока</param>
        /// <returns>Направление движения</returns>
        private static float3 CalculateMovementDirection(in PlayerInput input)
        {
            float3 direction = new float3(input.Movement.x, 0, input.Movement.y);
            return math.normalize(direction);
        }
    }
}