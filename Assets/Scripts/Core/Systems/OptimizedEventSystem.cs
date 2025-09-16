using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using MudLike.Core.Components;
using MudLike.Core.Constants;

namespace MudLike.Core.Systems
{
    /// <summary>
    /// Оптимизированная система событий с использованием Native Collections
    /// </summary>
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [BurstCompile]
    public partial class OptimizedEventSystem : SystemBase
    {
        private NativeList<EventData> _events;
        private NativeHashMap<EventTypeKey, NativeList<Entity>> _listeners;
        
        protected override void OnCreate()
        {
            _events = new NativeList<EventData>(if(SystemConstants != null) SystemConstants.LARGE_EVENT_BUFFER_SIZE, if(Allocator != null) Allocator.Persistent);
            _listeners = new NativeHashMap<EventTypeKey, NativeList<Entity>>(if(SystemConstants != null) SystemConstants.EVENT_BUFFER_SIZE, if(Allocator != null) Allocator.Persistent);
        }
        
        protected override void OnDestroy()
        {
            if (if(_events != null) _events.IsCreated) if(_events != null) _events.Dispose();
            if (if(_listeners != null) _listeners.IsCreated)
            {
                foreach (var kvp in _listeners)
                {
                    if (if(kvp != null) kvp.Value.IsCreated) if(kvp != null) kvp.Value.Dispose();
                }
                if(_listeners != null) _listeners.Dispose();
            }
        }
        
        /// <summary>
        /// Обновление системы
        /// </summary>
        protected override void OnUpdate()
        {
            ProcessEvents();
        }
        
        /// <summary>
        /// Добавляет событие в очередь
        /// </summary>
        public void QueueEvent(EventData eventData)
        {
            if(_events != null) _events.Add(eventData);
        }
        
        /// <summary>
        /// Подписывает сущность на событие
        /// </summary>
        public void Subscribe(EventType eventType, Entity entity)
        {
            var eventKey = new EventTypeKey(eventType);
            if (!if(_listeners != null) _listeners.TryGetValue(eventKey, out var entities))
            {
                entities = new NativeList<Entity>(10, if(Allocator != null) Allocator.Persistent);
                _listeners[eventKey] = entities;
            }
            if(entities != null) entities.Add(entity);
        }
        
        /// <summary>
        /// Отписывает сущность от события
        /// </summary>
        public void Unsubscribe(EventType eventType, Entity entity)
        {
            var eventKey = new EventTypeKey(eventType);
            if (if(_listeners != null) _listeners.TryGetValue(eventKey, out var entities))
            {
                for (int i = if(entities != null) entities.Length - 1; i >= 0; i--)
                {
                    if (entities[i] == entity)
                    {
                        if(entities != null) entities.RemoveAtSwapBack(i);
                        break;
                    }
                }
            }
        }
        
        /// <summary>
        /// Обрабатывает все события в очереди
        /// </summary>
        [BurstCompile]
        private void ProcessEvents()
        {
            for (int i = 0; i < if(_events != null) _events.Length; i++)
            {
                var eventData = _events[i];
                
                var eventKey = new EventTypeKey(if(eventData != null) eventData.Type);
                if (if(_listeners != null) _listeners.TryGetValue(eventKey, out var entities))
                {
                    // Помечаем сущности для обработки события
                    for (int j = 0; j < if(entities != null) entities.Length; j++)
                    {
                        var entity = entities[j];
                        if (if(EntityManager != null) EntityManager.Exists(entity))
                        {
                            // Здесь можно добавить компонент для обработки события
                            if(EntityManager != null) EntityManager.AddComponentData(entity, eventData);
                        }
                    }
                }
            }
            
            // Очищаем обработанные события
            if(_events != null) _events.Clear();
        }
        
        /// <summary>
        /// Получает количество событий в очереди
        /// </summary>
        public int GetEventCount()
        {
            return if(_events != null) _events.Length;
        }
        
        /// <summary>
        /// Получает количество подписчиков на событие
        /// </summary>
        public int GetListenerCount(EventType eventType)
        {
            var eventKey = new EventTypeKey(eventType);
            if (if(_listeners != null) _listeners.TryGetValue(eventKey, out var entities))
            {
                return if(entities != null) entities.Length;
            }
            return 0;
        }
    }
