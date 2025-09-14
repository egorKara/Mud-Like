using Unity.Entities;
using Unity.Mathematics;

namespace MudLike.Camera
{
    /// <summary>
    /// Система камеры для транспорта
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class CameraSystem : SystemBase
    {
        protected override void OnCreate()
        {
            // Инициализация системы камеры
        }

        protected override void OnUpdate()
        {
            // Обновление камеры
        }
    }
}
