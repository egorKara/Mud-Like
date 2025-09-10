using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;
using MudLike.Terrain.Components;
using MudLike.Vehicles.Components;

namespace MudLike.Terrain.Systems
{
    /// <summary>
    /// Менеджер грязи - основная система деформации террейна
    /// </summary>
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial class MudManager : SystemBase
    {
        /// <summary>
        /// Размер блока террейна
        /// </summary>
        private const float BLOCK_SIZE = 16f;
        
        /// <summary>
        /// Максимальная глубина погружения
        /// </summary>
        private const float MAX_SINK_DEPTH = 2f;
        
        /// <summary>
        /// Обрабатывает деформацию террейна
        /// </summary>
        protected override void OnUpdate()
        {
            float deltaTime = SystemAPI.Time.fixedDeltaTime;
            
            // Обрабатываем все колеса
            Entities
                .WithAll<WheelData>()
                .ForEach((ref WheelData wheel, in LocalTransform transform) =>
                {
                    ProcessWheelTerrainInteraction(ref wheel, transform, deltaTime);
                }).Schedule();
        }
        
        /// <summary>
        /// Обрабатывает взаимодействие колеса с террейном
        /// </summary>
        private static void ProcessWheelTerrainInteraction(ref WheelData wheel, in LocalTransform transform, float deltaTime)
        {
            // Получаем позицию колеса
            float3 wheelWorldPos = transform.Position + math.mul(transform.Rotation, wheel.LocalPosition);
            
            // Вычисляем координаты блока
            int2 blockCoords = GetBlockCoordinates(wheelWorldPos);
            
            // Запрашиваем данные грязи
            var mudQuery = QueryContact(wheelWorldPos, wheel.Radius);
            
            // Обновляем данные колеса
            wheel.SinkDepth = mudQuery.SinkDepth;
            wheel.TractionCoefficient = mudQuery.TractionModifier;
            
            // Применяем деформацию
            ApplyTerrainDeformation(blockCoords, wheelWorldPos, wheel.Radius, wheel.SinkDepth, deltaTime);
        }
        
        /// <summary>
        /// Запрашивает контакт с грязью
        /// </summary>
        /// <param name="wheelPosition">Позиция колеса</param>
        /// <param name="wheelRadius">Радиус колеса</param>
        /// <returns>Данные контакта с грязью</returns>
        public static MudContact QueryContact(float3 wheelPosition, float wheelRadius)
        {
            // Упрощенная реализация - в реальной игре здесь будет сложная логика
            var result = new MudContact
            {
                SinkDepth = 0f,
                TractionModifier = 0.8f,
                Viscosity = 0.5f,
                Density = 1.2f
            };
            
            // Вычисляем глубину погружения на основе позиции Y
            float terrainHeight = GetTerrainHeight(wheelPosition.xz);
            float sinkDepth = terrainHeight - wheelPosition.y;
            
            if (sinkDepth > 0)
            {
                result.SinkDepth = math.min(sinkDepth, MAX_SINK_DEPTH);
                result.TractionModifier = math.lerp(0.8f, 0.2f, result.SinkDepth / MAX_SINK_DEPTH);
            }
            
            return result;
        }
        
        /// <summary>
        /// Получает координаты блока для позиции
        /// </summary>
        private static int2 GetBlockCoordinates(float3 worldPosition)
        {
            return new int2(
                (int)math.floor(worldPosition.x / BLOCK_SIZE),
                (int)math.floor(worldPosition.z / BLOCK_SIZE)
            );
        }
        
        /// <summary>
        /// Получает высоту террейна в точке
        /// </summary>
        private static float GetTerrainHeight(float2 xzPosition)
        {
            // Упрощенная реализация - в реальной игре здесь будет обращение к TerrainData
            return 0f; // Плоский террейн
        }
        
        /// <summary>
        /// Применяет деформацию террейна
        /// </summary>
        private static void ApplyTerrainDeformation(int2 blockCoords, float3 wheelPosition, float wheelRadius, float sinkDepth, float deltaTime)
        {
            // Упрощенная реализация деформации
            // В реальной игре здесь будет обновление высотной карты террейна
        }
    }
    
    /// <summary>
    /// Структура данных контакта с грязью
    /// </summary>
    public struct MudContact
    {
        public float SinkDepth;
        public float TractionModifier;
        public float Viscosity;
        public float Density;
    }
}