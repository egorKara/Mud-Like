using Unity.Entities;
using Unity.Mathematics;

namespace MudLike.Networking
{
    /// <summary>
    /// Система сетевого взаимодействия
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class NetworkingSystem : SystemBase
    {
        protected override void OnCreate()
        {
            // Инициализация сетевой системы
        }

        protected override void OnUpdate()
        {
            // Обновление сетевого взаимодействия
        }
    }
}
