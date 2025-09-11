using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using MudLike.Terrain.Components;

namespace MudLike.Networking.Components
{
    /// <summary>
    /// Сетевые данные грязи
    /// </summary>
    public struct NetworkMud : IComponentData
    {
        /// <summary>
        /// Данные грязи
        /// </summary>
        public MudData Mud;
        
        /// <summary>
        /// Индекс чанка террейна
        /// </summary>
        public int ChunkIndex;
        
        /// <summary>
        /// Позиция в чанке
        /// </summary>
        public float2 ChunkPosition;
        
        /// <summary>
        /// Грязь изменилась
        /// </summary>
        public bool HasChanged;
        
        /// <summary>
        /// Тик команды
        /// </summary>
        public uint Tick;
    }
}