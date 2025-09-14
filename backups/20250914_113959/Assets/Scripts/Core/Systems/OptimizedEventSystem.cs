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
            _events = new NativeList<EventData>(SystemConstants.LARGE_EVENT_BUFFER_SIZE, Allocator.Persistent);
            _listeners = new NativeHashMap<EventTypeKey, NativeList<Entity>>(SystemConstants.EVENT_BUFFER_SIZE, Allocator.Persistent);
        }
        
        protected override void OnDestroy()
        {
            if (_events.IsCreated) _events.Dispose();
            if (_listeners.IsCreated)
            {
                foreach (var kvp in _listeners)
                {
                    if (kvp.Value.IsCreated) kvp.Value.Dispose();
                }
                _listeners.Dispose();
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
            _events.Add(eventData);
        }
        
        /// <summary>
        /// Подписывает сущность на событие
        /// </summary>
        public void Subscribe(EventType eventType, Entity entity)
        {
            var eventKey = new EventTypeKey(eventType);
            if (!_listeners.TryGetValue(eventKey, out var entities))
            {
                entities = new NativeList<Entity>(10, Allocator.Persistent);
                _listeners[eventKey] = entities;
            }
            entities.Add(entity);
        }
        
        /// <summary>
        /// Отписывает сущность от события
        /// </summary>
        public void Unsubscribe(EventType eventType, Entity entity)
        {
            var eventKey = new EventTypeKey(eventType);
            if (_listeners.TryGetValue(eventKey, out var entities))
            {
                for (int i = entities.Length - 1; i >= 0; i--)
                {
                    if (entities[i] == entity)
                    {
                        entities.RemoveAtSwapBack(i);
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
            for (int i = 0; i < _events.Length; i++)
            {
                var eventData = _events[i];
                
                var eventKey = new EventTypeKey(eventData.Type);
                if (_listeners.TryGetValue(eventKey, out var entities))
                {
                    // Помечаем сущности для обработки события
                    for (int j = 0; j < entities.Length; j++)
                    {
                        var entity = entities[j];
                        if (EntityManager.Exists(entity))
                        {
                            // Здесь можно добавить компонент для обработки события
                            EntityManager.AddComponentData(entity, eventData);
                        }
                    }
                }
            }
            
            // Очищаем обработанные события
            _events.Clear();
        }
        
        /// <summary>
        /// Получает количество событий в очереди
        /// </summary>
        public int GetEventCount()
        {
            return _events.Length;
        }
        
        /// <summary>
        /// Получает количество подписчиков на событие
        /// </summary>
        public int GetListenerCount(EventType eventType)
        {
            var eventKey = new EventTypeKey(eventType);
            if (_listeners.TryGetValue(eventKey, out var entities))
            {
                return entities.Length;
            }
            return 0;
        }
    }
}