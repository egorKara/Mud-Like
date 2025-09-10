using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using MudLike.Core.Components;

namespace MudLike.Core.Systems
{
    /// <summary>
    /// Система обработки ввода игрока в ECS архитектуре
    /// </summary>
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class InputSystem : SystemBase
    {
        /// <summary>
        /// Обрабатывает ввод всех игроков
        /// </summary>
        protected override void OnUpdate()
        {
            // Получаем ввод с клавиатуры
            float2 movement = new float2(
                Input.GetAxis("Horizontal"),
                Input.GetAxis("Vertical")
            );
            
            bool jump = Input.GetKeyDown(KeyCode.Space);
            bool brake = Input.GetKey(KeyCode.LeftShift);
            
            // Обновляем компоненты ввода для всех игроков
            Entities
                .WithAll<PlayerTag>()
                .ForEach((ref PlayerInput input) =>
                {
                    input.Movement = movement;
                    input.Jump = jump;
                    input.Brake = brake;
                }).WithoutBurst().Run();
        }
    }
}