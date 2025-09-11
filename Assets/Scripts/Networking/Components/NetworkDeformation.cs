using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using MudLike.Terrain.Components;

namespace MudLike.Networking.Components
{
    /// <summary>
    /// Сетевые данные деформации террейна
    /// </summary>
    public struct NetworkDeformation : IComponentData
    {
        /// <summary>
        /// Данные деформации
        /// </summary>
        public DeformationData Deformation;
        
        /// <summary>
        /// Индекс чанка террейна
        /// </summary>
        public int ChunkIndex;
        
        /// <summary>
        /// Позиция в чанке
        /// </summary>
        public float2 ChunkPosition;
        
        /// <summary>
        /// Деформация изменилась
        /// </summary>
        public bool HasChanged;
        
        /// <summary>
        /// Тик команды
        /// </summary>
        public uint Tick;
    }
}