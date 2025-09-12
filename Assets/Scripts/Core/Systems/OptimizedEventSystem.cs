using Unity.Entities;
using Unity.Collections;
using Unity.Jobs;
using Unity.Burst;
using Unity.Mathematics;
using System.Collections.Generic;

namespace MudLike.Core.Systems
{
    /// <summary>
    /// Оптимизированная система событий для ECS архитектуры
    /// Использует NativeArray для высокой производительности
    /// </summary>
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [BurstCompile]
    public partial class OptimizedEventSystem : SystemBase
    {
        private NativeList<GameEvent> _events;
        private NativeHashMap<int, NativeList<System.Action<GameEvent>>> _listeners;
        
        protected override void OnCreate()
        {
            _events = new NativeList<GameEvent>(1000, Allocator.Persistent);
            _listeners = new NativeHashMap<int, NativeList<System.Action<GameEvent>>>(100, Allocator.Persistent);
        }
        
        protected override void OnDestroy()
        {
            if (_events.IsCreated) _events.Dispose();
            if (_listeners.IsCreated) _listeners.Dispose();
        }
        
        /// <summary>
        /// Обрабатывает события в системе
        /// </summary>
        protected override void OnUpdate()
        {
            ProcessEvents();
        }
        
        /// <summary>
        /// Добавляет событие в очередь
        /// </summary>
        public void QueueEvent(GameEvent gameEvent)
        {
            _events.Add(gameEvent);
        }
        
        /// <summary>
        /// Подписывается на событие
        /// </summary>
        public void Subscribe(int eventType, System.Action<GameEvent> callback)
        {
            if (!_listeners.TryGetValue(eventType, out var callbacks))
            {
                callbacks = new NativeList<System.Action<GameEvent>>(10, Allocator.Persistent);
                _listeners[eventType] = callbacks;
            }
            callbacks.Add(callback);
        }
        
        /// <summary>
        /// Обрабатывает все события в очереди
        /// </summary>
        [BurstCompile]
        private void ProcessEvents()
        {
            for (int i = 0; i < _events.Length; i++)
            {
                var gameEvent = _events[i];
                
                if (_listeners.TryGetValue(gameEvent.Type, out var callbacks))
                {
                    for (int j = 0; j < callbacks.Length; j++)
                    {
                        callbacks[j](gameEvent);
                    }
                }
            }
            
            _events.Clear();
        }
    }
    
    /// <summary>
    /// Структура игрового события
    /// </summary>
    public struct GameEvent
    {
        public int Type;
        public float3 Position;
        public float Value;
        public int EntityId;
        public bool IsUrgent;
    }
}
