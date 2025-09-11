using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;
using MudLike.Terrain.Components;
using MudLike.Vehicles.Components;

namespace MudLike.Terrain.Systems
{
    /// <summary>
    /// Система управления грязью (MudManager API)
    /// </summary>
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial class MudManagerSystem : SystemBase
    {
        /// <summary>
        /// Обрабатывает взаимодействие с грязью
        /// </summary>
        protected override void OnUpdate()
        {
            // Обрабатываем взаимодействие колес с грязью
            ProcessWheelMudInteraction();
            
            // Обновляем состояние грязи
            UpdateMudState();
        }
        
        /// <summary>
        /// Обрабатывает взаимодействие колес с грязью
        /// </summary>
        private void ProcessWheelMudInteraction()
        {
            Entities
                .WithAll<WheelData>()
                .ForEach((ref WheelData wheel, in LocalTransform wheelTransform) =>
                {
                    if (wheel.IsGrounded)
                    {
                        // Запрашиваем данные о грязи
                        var mudInfo = QueryMudContact(wheelTransform.Position, wheel.Radius);
                        
                        // Обновляем данные колеса
                        wheel.Traction *= mudInfo.TractionModifier;
                        wheel.FrictionForce *= mudInfo.FrictionModifier;
                    }
                }).Schedule();
        }
        
        /// <summary>
        /// Обновляет состояние грязи
        /// </summary>
        private void UpdateMudState()
        {
            Entities
                .WithAll<MudData>()
                .ForEach((ref MudData mud) =>
                {
                    if (mud.IsActive && mud.NeedsUpdate)
                    {
                        UpdateMudProperties(ref mud);
                        mud.NeedsUpdate = false;
                    }
                }).Schedule();
        }
        
        /// <summary>
        /// Запрашивает контакт с грязью (MudManager API)
        /// </summary>
        /// <param name="wheelPosition">Позиция колеса</param>
        /// <param name="radius">Радиус колеса</param>
        /// <returns>Информация о контакте с грязью</returns>
        public static MudContactInfo QueryMudContact(float3 wheelPosition, float radius)
        {
            // Ищем ближайшую грязь
            var nearestMud = FindNearestMud(wheelPosition, radius);
            
            if (nearestMud.IsActive)
            {
                // Вычисляем параметры контакта
                float distance = math.distance(wheelPosition, nearestMud.Position);
                float influence = 1f - (distance / radius);
                influence = math.clamp(influence, 0f, 1f);
                
                return new MudContactInfo
                {
                    SinkDepth = nearestMud.Level * influence * 0.5f,
                    TractionModifier = 1f - (nearestMud.Resistance * influence),
                    FrictionModifier = 1f + (nearestMud.Viscosity * influence),
                    MudLevel = nearestMud.Level * influence
                };
            }
            
            return new MudContactInfo
            {
                SinkDepth = 0f,
                TractionModifier = 1f,
                FrictionModifier = 1f,
                MudLevel = 0f
            };
        }
        
        /// <summary>
        /// Находит ближайшую грязь
        /// </summary>
        private static MudData FindNearestMud(float3 position, float radius)
        {
            // Упрощенная реализация - в реальности нужно искать в пространственных структурах
            return new MudData
            {
                Position = position,
                Radius = radius,
                Level = 0.5f,
                Viscosity = 0.3f,
                Density = 1.2f,
                Resistance = 0.4f,
                IsActive = true,
                NeedsUpdate = false
            };
        }
        
        /// <summary>
        /// Обновляет свойства грязи
        /// </summary>
        private static void UpdateMudProperties(ref MudData mud)
        {
            // Здесь может быть логика изменения свойств грязи со временем
            // Например, высыхание, уплотнение, размывание
        }
    }
    
    /// <summary>
    /// Информация о контакте с грязью
    /// </summary>
    public struct MudContactInfo
    {
        /// <summary>
        /// Глубина погружения
        /// </summary>
        public float SinkDepth;
        
        /// <summary>
        /// Модификатор сцепления
        /// </summary>
        public float TractionModifier;
        
        /// <summary>
        /// Модификатор трения
        /// </summary>
        public float FrictionModifier;
        
        /// <summary>
        /// Уровень грязи
        /// </summary>
        public float MudLevel;
    }
}