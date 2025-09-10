using Unity.Entities;
using Unity.NetCode;
using Unity.Mathematics;
using MudLike.Terrain.Components;

namespace MudLike.Networking.Components
{
    /// <summary>
    /// Сетевые данные террейна для синхронизации
    /// </summary>
    [GhostComponent(PrefabType = GhostPrefabType.AllPredicted)]
    public struct NetworkedTerrainData : IComponentData
    {
        /// <summary>
        /// Координаты блока террейна
        /// </summary>
        [GhostField]
        public int2 BlockCoordinates;
        
        /// <summary>
        /// Высота грязи в блоке
        /// </summary>
        [GhostField(Quantization = 1000)]
        public float MudHeight;
        
        /// <summary>
        /// Коэффициент сцепления с грязью
        /// </summary>
        [GhostField(Quantization = 1000)]
        public float TractionModifier;
        
        /// <summary>
        /// Вязкость грязи
        /// </summary>
        [GhostField(Quantization = 1000)]
        public float Viscosity;
        
        /// <summary>
        /// Плотность грязи
        /// </summary>
        [GhostField(Quantization = 1000)]
        public float Density;
        
        /// <summary>
        /// Влажность грязи
        /// </summary>
        [GhostField(Quantization = 1000)]
        public float Moisture;
        
        /// <summary>
        /// Флаг изменения блока
        /// </summary>
        [GhostField]
        public bool IsDirty;
        
        /// <summary>
        /// Время последнего обновления
        /// </summary>
        [GhostField(Quantization = 100)]
        public float LastUpdateTime;
    }
}