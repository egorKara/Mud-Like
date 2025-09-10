using Unity.Entities;
using Unity.Mathematics;

namespace MudLike.Core.Components
{
    /// <summary>
    /// Компонент ввода игрока
    /// </summary>
    public struct PlayerInput : IComponentData
    {
        public float2 Movement;
        public bool Jump;
        public bool Brake;
    }
}
