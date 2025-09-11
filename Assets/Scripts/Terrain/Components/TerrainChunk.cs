using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;

namespace MudLike.Terrain.Components
{
    /// <summary>
    /// Чанк террейна
    /// </summary>
    public struct TerrainChunk : IComponentData
    {
        /// <summary>
        /// Индекс чанка
        /// </summary>
        public int ChunkIndex;
        
        /// <summary>
        /// Позиция чанка в сетке
        /// </summary>
        public int2 GridPosition;
        
        /// <summary>
        /// Мировая позиция чанка
        /// </summary>
        public float3 WorldPosition;
        
        /// <summary>
        /// Размер чанка
        /// </summary>
        public float ChunkSize;
        
        /// <summary>
        /// Разрешение чанка
        /// </summary>
        public int Resolution;
        
        /// <summary>
        /// Высоты чанка
        /// </summary>
        public NativeArray<float> Heights;
        
        /// <summary>
        /// Нормали чанка
        /// </summary>
        public NativeArray<float3> Normals;
        
        /// <summary>
        /// Уровень грязи чанка
        /// </summary>
        public NativeArray<float> MudLevel;
        
        /// <summary>
        /// Чанк изменен
        /// </summary>
        public bool IsDirty;
        
        /// <summary>
        /// Чанк активен
        /// </summary>
        public bool IsActive;
        
        /// <summary>
        /// Чанк загружен
        /// </summary>
        public bool IsLoaded;
        
        /// <summary>
        /// Чанк требует синхронизации
        /// </summary>
        public bool NeedsSync;
    }
}