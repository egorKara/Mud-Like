using Unity.Entities;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Collections;
using Unity.NetCode;
using MudLike.Terrain.Components;
using MudLike.Networking.Components;

namespace MudLike.Terrain.Systems
{
    /// <summary>
    /// Система синхронизации террейна между клиентами
    /// Обеспечивает детерминированную деформацию в мультиплеере
    /// </summary>
    [UpdateInGroup(typeof(NetCodeClientAndServerSystemGroup))]
    [BurstCompile]
    public partial class TerrainSyncSystem : SystemBase
    {
        private NativeHashMap<int, TerrainSyncData> _terrainSyncData;
        private NativeList<TerrainUpdate> _pendingUpdates;
        private float _lastSyncTime = 0f;
        private float _syncInterval = 0.1f; // Синхронизация каждые 100ms
        
        protected override void OnCreate()
        {
            _terrainSyncData = new NativeHashMap<int, TerrainSyncData>(1000, Allocator.Persistent);
            _pendingUpdates = new NativeList<TerrainUpdate>(100, Allocator.Persistent);
        }
        
        protected override void OnDestroy()
        {
            if (_terrainSyncData.IsCreated)
                _terrainSyncData.Dispose();
            if (_pendingUpdates.IsCreated)
                _pendingUpdates.Dispose();
        }
        
        /// <summary>
        /// Синхронизирует деформацию террейна между клиентами
        /// </summary>
        /// <param name="deformation">Данные деформации</param>
        /// <param name="authoritative">Является ли сервер авторитетным</param>
        [BurstCompile]
        public void SyncTerrainDeformation(DeformationData deformation, bool authoritative)
        {
            // Создаем данные синхронизации
            var syncData = new TerrainSyncData
            {
                Position = deformation.Position,
                Radius = deformation.Radius,
                Depth = deformation.Depth,
                Force = deformation.Force,
                Type = deformation.Type,
                Timestamp = SystemAPI.Time.time,
                IsAuthoritative = authoritative,
                NeedsSync = true
            };
            
            // Добавляем в список обновлений
            var update = new TerrainUpdate
            {
                SyncData = syncData,
                Priority = CalculateSyncPriority(deformation)
            };
            
            _pendingUpdates.Add(update);
        }
        
        /// <summary>
        /// Применяет синхронизированную деформацию
        /// </summary>
        /// <param name="syncData">Данные синхронизации</param>
        [BurstCompile]
        public void ApplySynchronizedDeformation(TerrainSyncData syncData)
        {
            // Проверяем, нужно ли применять деформацию
            if (!ShouldApplyDeformation(syncData))
                return;
            
            // Создаем деформацию из данных синхронизации
            var deformation = new DeformationData
            {
                Position = syncData.Position,
                Radius = syncData.Radius,
                Depth = syncData.Depth,
                Force = syncData.Force,
                Type = syncData.Type,
                Time = syncData.Timestamp,
                IsActive = true,
                IsApplied = false
            };
            
            // Применяем деформацию к террейну
            ApplyDeformationToTerrain(deformation);
            
            // Обновляем данные синхронизации
            syncData.NeedsSync = false;
            syncData.LastAppliedTime = SystemAPI.Time.time;
        }
        
        /// <summary>
        /// Получает изменения террейна для синхронизации
        /// </summary>
        /// <returns>Список изменений для отправки</returns>
        [BurstCompile]
        public NativeList<TerrainSyncData> GetTerrainChangesForSync()
        {
            var changes = new NativeList<TerrainSyncData>(100, Allocator.Temp);
            
            // Собираем все изменения, требующие синхронизации
            for (int i = 0; i < _pendingUpdates.Length; i++)
            {
                var update = _pendingUpdates[i];
                if (update.SyncData.NeedsSync)
                {
                    changes.Add(update.SyncData);
                }
            }
            
            return changes;
        }
        
        /// <summary>
        /// Очищает отправленные изменения
        /// </summary>
        /// <param name="sentChanges">Список отправленных изменений</param>
        [BurstCompile]
        public void ClearSentChanges(NativeList<TerrainSyncData> sentChanges)
        {
            for (int i = 0; i < sentChanges.Length; i++)
            {
                var sentData = sentChanges[i];
                
                // Удаляем из списка ожидающих
                for (int j = _pendingUpdates.Length - 1; j >= 0; j--)
                {
                    if (IsSameDeformation(sentData, _pendingUpdates[j].SyncData))
                    {
                        _pendingUpdates.RemoveAtSwapBack(j);
                        break;
                    }
                }
            }
            
            sentChanges.Dispose();
        }
        
        /// <summary>
        /// Вычисляет приоритет синхронизации деформации
        /// </summary>
        [BurstCompile]
        private int CalculateSyncPriority(DeformationData deformation)
        {
            int priority = 0;
            
            // Приоритет на основе силы деформации
            if (deformation.Force > 1000f) priority += 3;
            else if (deformation.Force > 500f) priority += 2;
            else if (deformation.Force > 100f) priority += 1;
            
            // Приоритет на основе типа деформации
            switch (deformation.Type)
            {
                case DeformationType.Indentation: priority += 2; break;
                case DeformationType.Elevation: priority += 3; break;
                case DeformationType.Smoothing: priority += 1; break;
                case DeformationType.Erosion: priority += 1; break;
            }
            
            // Приоритет на основе размера области
            if (deformation.Radius > 5f) priority += 2;
            else if (deformation.Radius > 2f) priority += 1;
            
            return priority;
        }
        
        /// <summary>
        /// Проверяет, нужно ли применять деформацию
        /// </summary>
        [BurstCompile]
        private bool ShouldApplyDeformation(TerrainSyncData syncData)
        {
            // Проверяем авторитетность
            if (!syncData.IsAuthoritative && HasAuthoritativeVersion(syncData))
                return false;
            
            // Проверяем время (избегаем старых обновлений)
            float currentTime = SystemAPI.Time.time;
            if (currentTime - syncData.Timestamp > 1.0f) // Максимум 1 секунда задержки
                return false;
            
            // Проверяем, не применяли ли уже эту деформацию
            if (currentTime - syncData.LastAppliedTime < 0.1f) // Минимум 100ms между применениями
                return false;
            
            return true;
        }
        
        /// <summary>
        /// Проверяет, есть ли авторитетная версия деформации
        /// </summary>
        [BurstCompile]
        private bool HasAuthoritativeVersion(TerrainSyncData syncData)
        {
            // Проверяем, есть ли в списке ожидающих авторитетная версия
            for (int i = 0; i < _pendingUpdates.Length; i++)
            {
                var update = _pendingUpdates[i];
                if (IsSameDeformation(update.SyncData, syncData) && update.SyncData.IsAuthoritative)
                    return true;
            }
            
            return false;
        }
        
        /// <summary>
        /// Применяет деформацию к террейну
        /// </summary>
        [BurstCompile]
        private void ApplyDeformationToTerrain(DeformationData deformation)
        {
            // Здесь должна быть интеграция с TerrainDeformationSystem
            // Для простоты создаем Entity с деформацией
            var entity = EntityManager.CreateEntity();
            EntityManager.AddComponentData(entity, deformation);
        }
        
        /// <summary>
        /// Проверяет, является ли деформация той же самой
        /// </summary>
        [BurstCompile]
        private bool IsSameDeformation(TerrainSyncData a, TerrainSyncData b)
        {
            float positionTolerance = 0.1f;
            float radiusTolerance = 0.1f;
            float timeTolerance = 0.1f;
            
            return math.distance(a.Position, b.Position) < positionTolerance &&
                   math.abs(a.Radius - b.Radius) < radiusTolerance &&
                   math.abs(a.Timestamp - b.Timestamp) < timeTolerance;
        }
        
        /// <summary>
        /// Синхронизирует состояние террейна с сервером
        /// </summary>
        [BurstCompile]
        private void SynchronizeWithServer()
        {
            // Получаем изменения для отправки
            var changes = GetTerrainChangesForSync();
            
            if (changes.Length > 0)
            {
                // Отправляем изменения на сервер
                SendTerrainChangesToServer(changes);
                
                // Очищаем отправленные изменения
                ClearSentChanges(changes);
            }
        }
        
        /// <summary>
        /// Отправляет изменения террейна на сервер
        /// </summary>
        [BurstCompile]
        private void SendTerrainChangesToServer(NativeList<TerrainSyncData> changes)
        {
            // Здесь должна быть интеграция с Netcode for Entities
            // Отправляем RPC с изменениями террейна
            for (int i = 0; i < changes.Length; i++)
            {
                var change = changes[i];
                // SendRPC(new TerrainUpdateRPC { Data = change });
            }
        }
        
        protected override void OnUpdate()
        {
            float currentTime = SystemAPI.Time.time;
            
            // Синхронизируем изменения террейна
            if (currentTime - _lastSyncTime >= _syncInterval)
            {
                SynchronizeWithServer();
                _lastSyncTime = currentTime;
            }
            
            // Применяем ожидающие деформации
            for (int i = _pendingUpdates.Length - 1; i >= 0; i--)
            {
                var update = _pendingUpdates[i];
                if (update.SyncData.NeedsSync)
                {
                    ApplySynchronizedDeformation(update.SyncData);
                }
            }
        }
    }
    
    /// <summary>
    /// Данные синхронизации террейна
    /// </summary>
    public struct TerrainSyncData
    {
        public float3 Position;
        public float Radius;
        public float Depth;
        public float Force;
        public DeformationType Type;
        public float Timestamp;
        public bool IsAuthoritative;
        public bool NeedsSync;
        public float LastAppliedTime;
    }
    
    /// <summary>
    /// Обновление террейна с приоритетом
    /// </summary>
    public struct TerrainUpdate
    {
        public TerrainSyncData SyncData;
        public int Priority;
    }
}
