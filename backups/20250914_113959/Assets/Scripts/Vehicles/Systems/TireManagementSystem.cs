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
                ComponentType.ReadWrite<TireData>(),
                ComponentType.ReadOnly<WheelData>()
            );
        }
        
        protected override void OnUpdate()
        {
            float deltaTime = SystemAPI.Time.DeltaTime;
            
            var tireManagementJob = new TireManagementJob
            {
                DeltaTime = deltaTime
            };
            
            Dependency = tireManagementJob.ScheduleParallel(_tireQuery, Dependency);
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
                if (tire.TreadWear >= 1f)
                {
                    tire.Condition = TireCondition.Worn;
                }
                else if (tire.Age >= tire.MaxAge)
                {
                    tire.Condition = TireCondition.Worn;
                }
                else if (tire.Mileage >= tire.MaxMileage)
                {
                    tire.Condition = TireCondition.Worn;
                }
                else if (tire.CurrentPressure <= tire.MinPressure * 0.8f)
                {
                    tire.Condition = TireCondition.Damaged;
                }
                else if (tire.Temperature >= tire.MaxTemperature * 0.9f)
                {
                    tire.Condition = TireCondition.Damaged;
                }
                else if (tire.TreadWear >= 0.8f)
                {
                    tire.Condition = TireCondition.Poor;
                }
                else if (tire.TreadWear >= 0.5f)
                {
                    tire.Condition = TireCondition.Fair;
                }
                else if (tire.TreadWear >= 0.2f)
                {
                    tire.Condition = TireCondition.Good;
                }
                else
                {
                    tire.Condition = TireCondition.New;
                }
            }
            
            /// <summary>
            /// Обновляет рекомендации по обслуживанию
            /// </summary>
            private void UpdateMaintenanceRecommendations(ref TireData tire)
            {
                // Рекомендации по давлению
                if (tire.CurrentPressure < tire.RecommendedPressure * 0.9f)
                {
                    // Нужно подкачать шину
                }
                else if (tire.CurrentPressure > tire.RecommendedPressure * 1.1f)
                {
                    // Нужно спустить шину
                }
                
                // Рекомендации по замене
                if (tire.TreadWear >= 0.8f)
                {
                    // Рекомендуется замена шины
                }
                
                // Рекомендации по температуре
                if (tire.Temperature >= tire.MaxTemperature * 0.8f)
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
                if (tire.CurrentPressure < tire.MinPressure)
                {
                    tire.CurrentPressure = tire.MinPressure;
                }
                
                // Автоматическое снижение температуры при перегреве
                if (tire.Temperature >= tire.MaxTemperature)
                {
                    tire.Temperature = tire.MaxTemperature * 0.95f;
                }
                
                // Автоматическая очистка от грязи
                if (tire.MudMass > 2f)
                {
                    float cleaning = tire.CleaningRate * DeltaTime;
                    tire.MudMass -= cleaning;
                    tire.MudParticleCount = (int)(tire.MudMass * 100f);
                }
            }
            
            /// <summary>
            /// Обновляет статистику шины
            /// </summary>
            private void UpdateTireStatistics(ref TireData tire, WheelData wheel)
            {
                // Обновляем пробег
                float distance = math.length(wheel.AngularVelocity) * wheel.Radius * DeltaTime / 1000f;
                tire.Mileage += distance;
                
                // Обновляем возраст
                tire.Age += DeltaTime / 86400f; // Конвертируем секунды в дни
                
                // Обновляем время последнего обновления
                tire.LastUpdateTime = SystemAPI.Time.ElapsedTime;
            }
        }
    }
}