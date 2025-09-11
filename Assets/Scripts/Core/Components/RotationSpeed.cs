using Unity.Entities;

namespace MudLike.Core.Components
{
    /// <summary>
    /// Компонент скорости поворота игрока
    /// </summary>
    public struct RotationSpeed : IComponentData
    {
        /// <summary>
        /// Скорость поворота в градусах в секунду
        /// </summary>
        public float Value;
    }
}
