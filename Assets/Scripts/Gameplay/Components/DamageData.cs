using Unity.Entities;
using Unity.Mathematics;

namespace MudLike.Gameplay.Components
{
    /// <summary>
    /// Данные о повреждениях транспортного средства
    /// </summary>
    public struct DamageData : IComponentData
    {
        /// <summary>
        /// Общее состояние здоровья (0-100)
        /// </summary>
        public float Health;
        
        /// <summary>
        /// Максимальное здоровье
        /// </summary>
        public float MaxHealth;
        
        /// <summary>
        /// Повреждения двигателя (0-100)
        /// </summary>
        public float EngineDamage;
        
        /// <summary>
        /// Повреждения колес (0-100)
        /// </summary>
        public float WheelDamage;
        
        /// <summary>
        /// Повреждения кузова (0-100)
        /// </summary>
        public float BodyDamage;
        
        /// <summary>
        /// Время последнего ремонта
        /// </summary>
        public float LastRepairTime;
        
        /// <summary>
        /// Стоимость ремонта
        /// </summary>
        public float RepairCost;
        
        /// <summary>
        /// Можно ли управлять транспортом
        /// </summary>
        public bool IsOperational => Health > 20f && EngineDamage < 80f;
        
        /// <summary>
        /// Процент повреждений
        /// </summary>
        public float DamagePercentage => (MaxHealth - Health) / MaxHealth * 100f;
    }
}