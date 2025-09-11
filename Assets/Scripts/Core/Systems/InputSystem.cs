using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using MudLike.Core.Components;

namespace MudLike.Core.Systems
{
    /// <summary>
    /// Система обработки ввода игрока
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
            float2 input = GetKeyboardInput();
            
            // Обновляем компонент ввода для всех игроков
            Entities
                .WithAll<PlayerTag>()
                .ForEach((ref PlayerInput playerInput) =>
                {
                    playerInput.Movement = input;
                    playerInput.Jump = UnityEngine.Input.GetKey(UnityEngine.KeyCode.Space);
                    playerInput.Brake = UnityEngine.Input.GetKey(UnityEngine.KeyCode.LeftShift);
                }).Run();
        }
        
        /// <summary>
        /// Получает ввод с клавиатуры
        /// </summary>
        /// <returns>Вектор движения</returns>
        private static float2 GetKeyboardInput()
        {
            float2 input = float2.zero;
            
            // Движение по горизонтали (A/D или стрелки влево/вправо)
            if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.A) || 
                UnityEngine.Input.GetKey(UnityEngine.KeyCode.LeftArrow))
            {
                input.x = -1f;
            }
            else if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.D) || 
                     UnityEngine.Input.GetKey(UnityEngine.KeyCode.RightArrow))
            {
                input.x = 1f;
            }
            
            // Движение по вертикали (W/S или стрелки вверх/вниз)
            if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.W) || 
                UnityEngine.Input.GetKey(UnityEngine.KeyCode.UpArrow))
            {
                input.y = 1f;
            }
            else if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.S) || 
                     UnityEngine.Input.GetKey(UnityEngine.KeyCode.DownArrow))
            {
                input.y = -1f;
            }
            
            return input;
        }
    }
}
