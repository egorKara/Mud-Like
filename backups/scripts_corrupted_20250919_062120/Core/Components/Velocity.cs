using Unity.Entities;
using Unity.Mathematics;

namespace MudLike.Core.Components
{
    /// <summary>
    /// Компонент скорости движения
    /// </summary>
    public struct Velocity : IComponentData
    {
        public float3 Value;
    }
}
