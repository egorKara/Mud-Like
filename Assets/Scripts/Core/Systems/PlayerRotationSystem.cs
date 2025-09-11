using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using MudLike.Core.Components;

namespace MudLike.Core.Systems
{
    /// <summary>
    /// Система поворота игрока в ECS архитектуре
    /// </summary>
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial class PlayerRotationSystem : SystemBase
    {
        /// <summary>
        /// Обрабатывает поворот всех игроков
        /// </summary>
        protected override void OnUpdate()
        {
            float deltaTime = SystemAPI.Time.fixedDeltaTime;

            Entities
                .WithAll<PlayerTag>()
                .ForEach((ref LocalTransform transform, in PlayerInput input, in RotationSpeed rotationSpeed) =>
                {
                    ProcessRotation(ref transform, input, rotationSpeed, deltaTime);
                }).Schedule();
        }

        /// <summary>
        /// Обрабатывает поворот конкретного игрока
        /// </summary>
        /// <param name="transform">Трансформация игрока</param>
        /// <param name="input">Ввод игрока</param>
        /// <param name="rotationSpeed">Скорость поворота</param>
        /// <param name="deltaTime">Время с последнего обновления</param>
        private static void ProcessRotation(ref LocalTransform transform, in PlayerInput input, 
                                          in RotationSpeed rotationSpeed, float deltaTime)
        {
            // Поворот только если есть ввод
            if (math.length(input.Movement) > 0.1f)
            {
                // Вычисляем направление движения
                float3 movementDirection = new float3(input.Movement.x, 0, input.Movement.y);
                movementDirection = math.normalize(movementDirection);
                
                // Вычисляем целевой поворот
                quaternion targetRotation = quaternion.LookRotation(movementDirection, math.up());
                
                // Плавный поворот
                transform.Rotation = math.slerp(transform.Rotation, targetRotation, 
                                              rotationSpeed.Value * deltaTime);
            }
        }
    }
}
