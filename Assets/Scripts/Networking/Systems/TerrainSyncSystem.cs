using Unity.Entities;
using Unity.NetCode;
using MudLike.Terrain.Components;
using MudLike.Networking.Components;

namespace MudLike.Networking.Systems
{
    /// <summary>
    /// Система синхронизации террейна
    /// </summary>
    [UpdateInGroup(typeof(GhostUpdateSystemGroup))]
    public partial class TerrainSyncSystem : SystemBase
    {
        /// <summary>
        /// Синхронизирует данные террейна
        /// </summary>
        protected override void OnUpdate()
        {
            if (HasSingleton<NetworkStreamInGame>())
            {
                SyncTerrainData();
            }
        }
        
        /// <summary>
        /// Синхронизирует данные террейна
        /// </summary>
        private void SyncTerrainData()
        {
            Entities
                .WithAll<MudData, NetworkedTerrainData>()
                .ForEach((ref NetworkedTerrainData networkedData, in MudData mudData, in TerrainBlockData terrainData) =>
                {
                    // Обновляем сетевые данные из локальных данных
                    networkedData.BlockCoordinates = terrainData.GridPosition;
                    networkedData.MudHeight = mudData.Height;
                    networkedData.TractionModifier = mudData.TractionModifier;
                    networkedData.Viscosity = mudData.Viscosity;
                    networkedData.Density = mudData.Density;
                    networkedData.Moisture = mudData.Moisture;
                    networkedData.IsDirty = mudData.IsDirty;
                    networkedData.LastUpdateTime = mudData.LastUpdateTime;
                }).WithoutBurst().Run();
        }
    }
}