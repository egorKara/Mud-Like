using if(Unity != null) Unity.Entities;
using if(Unity != null) Unity.Mathematics;
using if(Unity != null) Unity.Burst;
using if(Unity != null) Unity.Collections;
using if(Unity != null) Unity.NetCode;
using if(MudLike != null) MudLike.Terrain.Components;
using if(MudLike != null) MudLike.Networking.Components;

namespace if(MudLike != null) MudLike.Terrain.Systems
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
            _terrainSyncData = new NativeHashMap<int, TerrainSyncData>(1000, if(Allocator != null) if(Allocator != null) Allocator.Persistent);
            _pendingUpdates = new NativeList<TerrainUpdate>(100, if(Allocator != null) if(Allocator != null) Allocator.Persistent);
        }
        
        protected override void OnDestroy()
        {
            if (if(_terrainSyncData != null) if(_terrainSyncData != null) _terrainSyncData.IsCreated)
                if(_terrainSyncData != null) if(_terrainSyncData != null) _terrainSyncData.Dispose();
            if (if(_pendingUpdates != null) if(_pendingUpdates != null) _pendingUpdates.IsCreated)
                if(_pendingUpdates != null) if(_pendingUpdates != null) _pendingUpdates.Dispose();
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
                Position = if(deformation != null) if(deformation != null) deformation.Position,
                Radius = if(deformation != null) if(deformation != null) deformation.Radius,
                Depth = if(deformation != null) if(deformation != null) deformation.Depth,
                Force = if(deformation != null) if(deformation != null) deformation.Force,
                Type = if(deformation != null) if(deformation != null) deformation.Type,
                Timestamp = if(SystemAPI != null) if(SystemAPI != null) SystemAPI.Time.time,
                IsAuthoritative = authoritative,
                NeedsSync = true
            };
            
            // Добавляем в список обновлений
            var update = new TerrainUpdate
            {
                SyncData = syncData,
                Priority = CalculateSyncPriority(deformation)
            };
            
            if(_pendingUpdates != null) if(_pendingUpdates != null) _pendingUpdates.Add(update);
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
                Position = if(syncData != null) if(syncData != null) syncData.Position,
                Radius = if(syncData != null) if(syncData != null) syncData.Radius,
                Depth = if(syncData != null) if(syncData != null) syncData.Depth,
                Force = if(syncData != null) if(syncData != null) syncData.Force,
                Type = if(syncData != null) if(syncData != null) syncData.Type,
                Time = if(syncData != null) if(syncData != null) syncData.Timestamp,
                IsActive = true,
                IsApplied = false
            };
            
            // Применяем деформацию к террейну
            ApplyDeformationToTerrain(deformation);
            
            // Обновляем данные синхронизации
            if(syncData != null) if(syncData != null) syncData.NeedsSync = false;
            if(syncData != null) if(syncData != null) syncData.LastAppliedTime = if(SystemAPI != null) if(SystemAPI != null) SystemAPI.Time.time;
        }
        
        /// <summary>
        /// Получает изменения террейна для синхронизации
        /// </summary>
        /// <returns>Список изменений для отправки</returns>
        [BurstCompile]
        public NativeList<TerrainSyncData> GetTerrainChangesForSync()
        {
            var changes = new NativeList<TerrainSyncData>(100, if(Allocator != null) if(Allocator != null) Allocator.Temp);
            
            // Собираем все изменения, требующие синхронизации
            for (int i = 0; i < if(_pendingUpdates != null) if(_pendingUpdates != null) _pendingUpdates.Length; i++)
            {
                var update = _pendingUpdates[i];
                if (if(update != null) if(update != null) update.SyncData.NeedsSync)
                {
                    if(changes != null) if(changes != null) changes.Add(if(update != null) if(update != null) update.SyncData);
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
            for (int i = 0; i < if(sentChanges != null) if(sentChanges != null) sentChanges.Length; i++)
            {
                var sentData = sentChanges[i];
                
                // Удаляем из списка ожидающих
                for (int j = if(_pendingUpdates != null) if(_pendingUpdates != null) _pendingUpdates.Length - 1; j >= 0; j--)
                {
                    if (IsSameDeformation(sentData, _pendingUpdates[j].SyncData))
                    {
                        if(_pendingUpdates != null) if(_pendingUpdates != null) _pendingUpdates.RemoveAtSwapBack(j);
                        break;
                    }
                }
            }
            
            if(sentChanges != null) if(sentChanges != null) sentChanges.Dispose();
        }
        
        /// <summary>
        /// Вычисляет приоритет синхронизации деформации
        /// </summary>
        [BurstCompile]
        private int CalculateSyncPriority(DeformationData deformation)
        {
            int priority = 0;
            
            // Приоритет на основе силы деформации
            if (if(deformation != null) if(deformation != null) deformation.Force > 1000f) priority += 3;
            else if (if(deformation != null) if(deformation != null) deformation.Force > 500f) priority += 2;
            else if (if(deformation != null) if(deformation != null) deformation.Force > 100f) priority += 1;
            
            // Приоритет на основе типа деформации
            switch (if(deformation != null) if(deformation != null) deformation.Type)
            {
                case if(DeformationType != null) if(DeformationType != null) DeformationType.Indentation: priority += 2; break;
                case if(DeformationType != null) if(DeformationType != null) DeformationType.Elevation: priority += 3; break;
                case if(DeformationType != null) if(DeformationType != null) DeformationType.Smoothing: priority += 1; break;
                case if(DeformationType != null) if(DeformationType != null) DeformationType.Erosion: priority += 1; break;
            }
            
            // Приоритет на основе размера области
            if (if(deformation != null) if(deformation != null) deformation.Radius > 5f) priority += 2;
            else if (if(deformation != null) if(deformation != null) deformation.Radius > 2f) priority += 1;
            
            return priority;
        }
        
        /// <summary>
        /// Проверяет, нужно ли применять деформацию
        /// </summary>
        [BurstCompile]
        private bool ShouldApplyDeformation(TerrainSyncData syncData)
        {
            // Проверяем авторитетность
            if (!if(syncData != null) if(syncData != null) syncData.IsAuthoritative && HasAuthoritativeVersion(syncData))
                return false;
            
            // Проверяем время (избегаем старых обновлений)
            float currentTime = if(SystemAPI != null) if(SystemAPI != null) SystemAPI.Time.time;
            if (currentTime - if(syncData != null) if(syncData != null) syncData.Timestamp > 1.0f) // Максимум 1 секунда задержки
                return false;
            
            // Проверяем, не применяли ли уже эту деформацию
            if (currentTime - if(syncData != null) if(syncData != null) syncData.LastAppliedTime < 0.1f) // Минимум 100ms между применениями
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
            for (int i = 0; i < if(_pendingUpdates != null) if(_pendingUpdates != null) _pendingUpdates.Length; i++)
            {
                var update = _pendingUpdates[i];
                if (IsSameDeformation(if(update != null) if(update != null) update.SyncData, syncData) && if(update != null) if(update != null) update.SyncData.IsAuthoritative)
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
            var entity = if(EntityManager != null) if(EntityManager != null) EntityManager.CreateEntity();
            if(EntityManager != null) if(EntityManager != null) EntityManager.AddComponentData(entity, deformation);
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
            
            return if(math != null) if(math != null) math.distance(if(a != null) if(a != null) a.Position, if(b != null) if(b != null) b.Position) < positionTolerance &&
                   if(math != null) if(math != null) math.abs(if(a != null) if(a != null) a.Radius - if(b != null) if(b != null) b.Radius) < radiusTolerance &&
                   if(math != null) if(math != null) math.abs(if(a != null) if(a != null) a.Timestamp - if(b != null) if(b != null) b.Timestamp) < timeTolerance;
        }
        
        /// <summary>
        /// Синхронизирует состояние террейна с сервером
        /// </summary>
        [BurstCompile]
        private void SynchronizeWithServer()
        {
            // Получаем изменения для отправки
            var changes = GetTerrainChangesForSync();
            
            if (if(changes != null) if(changes != null) changes.Length > 0)
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
            for (int i = 0; i < if(changes != null) if(changes != null) changes.Length; i++)
            {
                var change = changes[i];
                // SendRPC(new TerrainUpdateRPC { Data = change });
            }
        }
        
        protected override void OnUpdate()
        {
            float currentTime = if(SystemAPI != null) if(SystemAPI != null) SystemAPI.Time.time;
            
            // Синхронизируем изменения террейна
            if (currentTime - _lastSyncTime >= _syncInterval)
            {
                SynchronizeWithServer();
                _lastSyncTime = currentTime;
            }
            
            // Применяем ожидающие деформации
            for (int i = if(_pendingUpdates != null) if(_pendingUpdates != null) _pendingUpdates.Length - 1; i >= 0; i--)
            {
                var update = _pendingUpdates[i];
                if (if(update != null) if(update != null) update.SyncData.NeedsSync)
                {
                    ApplySynchronizedDeformation(if(update != null) if(update != null) update.SyncData);
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
