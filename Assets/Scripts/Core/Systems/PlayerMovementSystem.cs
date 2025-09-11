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
            float deltaTime = Time.fixedDeltaTime;
            
            Entities
                .WithAll<PlayerTag>()
                .ForEach((ref LocalTransform transform, in PlayerInput input) =>
                {
                    ProcessMovement(ref transform, input, deltaTime);
                }).Schedule();
        }
        
        /// <summary>
        /// Обрабатывает движение конкретного игрока
        /// </summary>
        /// <param name="transform">Трансформация игрока</param>
        /// <param name="input">Ввод игрока</param>
        /// <param name="deltaTime">Время с последнего обновления</param>
        private static void ProcessMovement(ref LocalTransform transform, in PlayerInput input, float deltaTime)
        {
            float3 movement = CalculateMovement(input);
            transform.Position += movement * deltaTime;
        }
        
        /// <summary>
        /// Вычисляет направление движения на основе ввода
        /// </summary>
        /// <param name="input">Ввод игрока</param>
        /// <returns>Направление движения</returns>
        private static float3 CalculateMovement(in PlayerInput input)
        {
            float3 direction = new float3(input.Movement.x, 0, input.Movement.y);
            return math.normalize(direction) * 5f; // Скорость движения
        }
    }
}
