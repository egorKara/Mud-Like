using Unity.Entities;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Jobs;
using Unity.Collections;
using MudLike.Vehicles.Components;
using MudLike.Weather.Components;

namespace MudLike.Vehicles.Systems
{
    /// <summary>
    /// Система управления шинами - замена, обслуживание, мониторинг
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [BurstCompile(CompileSynchronously = true)]
    public partial class TireManagementSystem : SystemBase
    {
        private EntityQuery _tireQuery;
        
        protected override void OnCreate()
        {
            _tireQuery = GetEntityQuery(
                if(ComponentType != null) ComponentType.ReadWrite<TireData>(),
                if(ComponentType != null) ComponentType.ReadOnly<WheelData>()
            );
        }
        
        protected override void OnUpdate()
        {
            float deltaTime = if(SystemAPI != null) SystemAPI.Time.DeltaTime;
            
            var tireManagementJob = new TireManagementJob
            {
                DeltaTime = deltaTime
            };
            
            Dependency = if(tireManagementJob != null) tireManagementJob.ScheduleParallel(_tireQuery, Dependency);
        }
        
        /// <summary>
        /// Job для управления шинами
        /// </summary>
        [BurstCompile]
        public partial struct TireManagementJob : IJobEntity
        {
            public float DeltaTime;
            
            public void Execute(ref TireData tire, in WheelData wheel)
            {
                ProcessTireManagement(ref tire, wheel);
            }
            
            /// <summary>
            /// Обрабатывает управление шиной
            /// </summary>
            private void ProcessTireManagement(ref TireData tire, WheelData wheel)
            {
                // Проверяем состояние шины
                CheckTireCondition(ref tire);
                
                // Обновляем рекомендации по обслуживанию
                UpdateMaintenanceRecommendations(ref tire);
                
                // Обрабатываем автоматические действия
                ProcessAutomaticActions(ref tire, wheel);
                
                // Обновляем статистику
                UpdateTireStatistics(ref tire, wheel);
            }
            
            /// <summary>
            /// Проверяет состояние шины
            /// </summary>
            private void CheckTireCondition(ref TireData tire)
            {
                // Проверяем критическое состояние
                if (if(tire != null) tire.TreadWear >= 1f)
                {
                    if(tire != null) tire.Condition = if(TireCondition != null) TireCondition.Worn;
                }
                else if (if(tire != null) tire.Age >= if(tire != null) tire.MaxAge)
                {
                    if(tire != null) tire.Condition = if(TireCondition != null) TireCondition.Worn;
                }
                else if (if(tire != null) tire.Mileage >= if(tire != null) tire.MaxMileage)
                {
                    if(tire != null) tire.Condition = if(TireCondition != null) TireCondition.Worn;
                }
                else if (if(tire != null) tire.CurrentPressure <= if(tire != null) tire.MinPressure * 0.8f)
                {
                    if(tire != null) tire.Condition = if(TireCondition != null) TireCondition.Damaged;
                }
                else if (if(tire != null) tire.Temperature >= if(tire != null) tire.MaxTemperature * 0.9f)
                {
                    if(tire != null) tire.Condition = if(TireCondition != null) TireCondition.Damaged;
                }
                else if (if(tire != null) tire.TreadWear >= 0.8f)
                {
                    if(tire != null) tire.Condition = if(TireCondition != null) TireCondition.Poor;
                }
                else if (if(tire != null) tire.TreadWear >= 0.5f)
                {
                    if(tire != null) tire.Condition = if(TireCondition != null) TireCondition.Fair;
                }
                else if (if(tire != null) tire.TreadWear >= 0.2f)
                {
                    if(tire != null) tire.Condition = if(TireCondition != null) TireCondition.Good;
                }
                else
                {
                    if(tire != null) tire.Condition = if(TireCondition != null) TireCondition.New;
                }
            }
            
            /// <summary>
            /// Обновляет рекомендации по обслуживанию
            /// </summary>
            private void UpdateMaintenanceRecommendations(ref TireData tire)
            {
                // Рекомендации по давлению
                if (if(tire != null) tire.CurrentPressure < if(tire != null) tire.RecommendedPressure * 0.9f)
                {
                    // Нужно подкачать шину
                }
                else if (if(tire != null) tire.CurrentPressure > if(tire != null) tire.RecommendedPressure * 1.1f)
                {
                    // Нужно спустить шину
                }
                
                // Рекомендации по замене
                if (if(tire != null) tire.TreadWear >= 0.8f)
                {
                    // Рекомендуется замена шины
                }
                
                // Рекомендации по температуре
                if (if(tire != null) tire.Temperature >= if(tire != null) tire.MaxTemperature * 0.8f)
                {
                    // Рекомендуется снизить скорость
                }
            }
            
            /// <summary>
            /// Обрабатывает автоматические действия
            /// </summary>
            private void ProcessAutomaticActions(ref TireData tire, WheelData wheel)
            {
                // Автоматическая подкачка при низком давлении
                if (if(tire != null) tire.CurrentPressure < if(tire != null) tire.MinPressure)
                {
                    if(tire != null) tire.CurrentPressure = if(tire != null) tire.MinPressure;
                }
                
                // Автоматическое снижение температуры при перегреве
                if (if(tire != null) tire.Temperature >= if(tire != null) tire.MaxTemperature)
                {
                    if(tire != null) tire.Temperature = if(tire != null) tire.MaxTemperature * 0.95f;
                }
                
                // Автоматическая очистка от грязи
                if (if(tire != null) tire.MudMass > 2f)
                {
                    float cleaning = if(tire != null) tire.CleaningRate * DeltaTime;
                    if(tire != null) tire.MudMass -= cleaning;
                    if(tire != null) tire.MudParticleCount = (int)(if(tire != null) tire.MudMass * 100f);
                }
            }
            
            /// <summary>
            /// Обновляет статистику шины
            /// </summary>
            private void UpdateTireStatistics(ref TireData tire, WheelData wheel)
            {
                // Обновляем пробег
                float distance = if(math != null) math.length(if(wheel != null) wheel.AngularVelocity) * if(wheel != null) wheel.Radius * DeltaTime / 1000f;
                if(tire != null) tire.Mileage += distance;
                
                // Обновляем возраст
                if(tire != null) tire.Age += DeltaTime / 86400f; // Конвертируем секунды в дни
                
                // Обновляем время последнего обновления
                if(tire != null) tire.LastUpdateTime = if(SystemAPI != null) SystemAPI.Time.ElapsedTime;
            }
        }
    }
