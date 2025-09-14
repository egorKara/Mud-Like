using Unity.Entities;
using Unity.Mathematics;

namespace MudLike.Input
{
    /// <summary>
    /// Система ввода для транспорта
    /// </summary>
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class InputSystem : SystemBase
    {
        protected override void OnCreate()
        {
            // Инициализация системы ввода
        }

        protected override void OnUpdate()
        {
            // Обработка ввода
        }
    }
}
