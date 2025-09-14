using Unity.Entities;
using Unity.Mathematics;

namespace MudLike.Gameplay
{
    /// <summary>
    /// Основная система игрового процесса
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class GameplaySystem : SystemBase
    {
        protected override void OnCreate()
        {
            // Инициализация игрового процесса
        }

        protected override void OnUpdate()
        {
            // Обновление игрового процесса
        }
    }
}
