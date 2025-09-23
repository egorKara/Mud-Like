using Unity.Entities;
using Unity.Collections;
using MudLike.Core.Performance;

namespace MudLike.Core.ECS
{
    /// <summary>
    /// ECS система для управления пулом памяти
    /// </summary>
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial class MemoryPoolSystemPool : SystemBase
    {
        private MudLikeMemoryPool _memoryPool;
        private float _lastCleanupTime;
        private const float CLEANUP_INTERVAL = 16.67f; // ~60 FPS
        
        protected override void OnCreate()
        {
            _memoryPool = new MudLikeMemoryPool();
            _memoryPool.Initialize();
        }
        
        protected override void OnDestroy()
        {
            _memoryPool?.Dispose();
        }
        
        protected override void OnUpdate()
        {
            // Очистка неиспользуемых объектов периодически
            if (SystemAPI.Time.ElapsedTime - _lastCleanupTime > CLEANUP_INTERVAL)
            {
                _memoryPool.Update();
                _lastCleanupTime = (float)SystemAPI.Time.ElapsedTime;
            }
        }
        
        /// <summary>
        /// Получает экземпляр пула памяти
        /// </summary>
        public MudLikeMemoryPool GetMemoryPool()
        {
            return _memoryPool;
        }
    }
}
