using Unity.Entities;
using Unity.Mathematics;

namespace MudLike.Core.Components
{
    /// <summary>
    /// Компонент позиции в мире
    /// </summary>
    public struct Position : IComponentData
    {
        public float3 Value;
    }
}
