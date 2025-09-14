using Unity.Entities;
using Unity.Mathematics;

namespace MudLike.Core.Components
{
    /// <summary>
    /// Входные данные игрока
    /// </summary>
    public struct PlayerInput : IComponentData
    {
        /// <summary>
        /// Движение вперед/назад
        /// </summary>
        public float Move;
        
        /// <summary>
        /// Поворот влево/вправо
        /// </summary>
        public float Turn;
        
        /// <summary>
        /// Торможение
        /// </summary>
        public float Brake;
    }
}
