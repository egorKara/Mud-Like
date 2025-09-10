using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Jobs;
using Unity.Burst;
using MudLike.Terrain.Components;
using MudLike.Vehicles.Components;

namespace MudLike.Terrain.Systems
{
    /// <summary>
    /// Менеджер грязи - основная система деформации террейна
    /// </summary>
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile]
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
        /// Разрешение высоты блока
        /// </summary>
        private const int HEIGHT_RESOLUTION = 64;
        
        /// <summary>
        /// Максимальная деформация за кадр
        /// </summary>
        private const float MAX_DEFORMATION_PER_FRAME = 0.1f;
        
        /// <summary>
        /// Сетка блоков террейна
        /// </summary>
        private NativeHashMap<int2, Entity> _terrainBlocks;
        
        /// <summary>
        /// Кэш высот блока
        /// </summary>
        private NativeHashMap<int2, NativeArray<float>> _heightCache;
        
        /// <summary>
        /// Инициализация системы
        /// </summary>
        protected override void OnCreate()
        {
            _terrainBlocks = new NativeHashMap<int2, Entity>(1000, Allocator.Persistent);
            _heightCache = new NativeHashMap<int2, NativeArray<float>>(1000, Allocator.Persistent);
        }
        
        /// <summary>
        /// Очистка ресурсов
        /// </summary>
        protected override void OnDestroy()
        {
            if (_terrainBlocks.IsCreated)
            {
                _terrainBlocks.Dispose();
            }
            
            if (_heightCache.IsCreated)
            {
                foreach (var kvp in _heightCache)
                {
                    if (kvp.Value.IsCreated)
                    {
                        kvp.Value.Dispose();
                    }
                }
                _heightCache.Dispose();
            }
        }
        
        /// <summary>
        /// Обрабатывает деформацию террейна
        /// </summary>
        protected override void OnUpdate()
        {
            float deltaTime = SystemAPI.Time.fixedDeltaTime;
            
            // Создаем job для обработки деформации
            var deformationJob = new TerrainDeformationJob
            {
                TerrainBlocks = _terrainBlocks,
                HeightCache = _heightCache,
                BlockSize = BLOCK_SIZE,
                MaxSinkDepth = MAX_SINK_DEPTH,
                HeightResolution = HEIGHT_RESOLUTION,
                MaxDeformationPerFrame = MAX_DEFORMATION_PER_FRAME,
                DeltaTime = deltaTime
            };
            
            // Запускаем job
            var jobHandle = deformationJob.ScheduleParallel(this);
            jobHandle.Complete();
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
    /// Job для деформации террейна
    /// </summary>
    [BurstCompile]
    public struct TerrainDeformationJob : IJobEntity
    {
        public NativeHashMap<int2, Entity> TerrainBlocks;
        public NativeHashMap<int2, NativeArray<float>> HeightCache;
        public float BlockSize;
        public float MaxSinkDepth;
        public int HeightResolution;
        public float MaxDeformationPerFrame;
        public float DeltaTime;
        
        public void Execute(ref WheelData wheel, in LocalTransform transform)
        {
            // Получаем позицию колеса
            float3 wheelWorldPos = transform.Position + math.mul(transform.Rotation, wheel.LocalPosition);
            
            // Вычисляем координаты блока
            int2 blockCoords = GetBlockCoordinates(wheelWorldPos);
            
            // Запрашиваем данные грязи
            var mudQuery = QueryContact(wheelWorldPos, wheel.Radius, blockCoords);
            
            // Обновляем данные колеса
            wheel.SinkDepth = mudQuery.SinkDepth;
            wheel.TractionCoefficient = mudQuery.TractionModifier;
            
            // Применяем деформацию
            ApplyTerrainDeformation(blockCoords, wheelWorldPos, wheel.Radius, wheel.SinkDepth);
        }
        
        /// <summary>
        /// Получает координаты блока для позиции
        /// </summary>
        private int2 GetBlockCoordinates(float3 worldPosition)
        {
            return new int2(
                (int)math.floor(worldPosition.x / BlockSize),
                (int)math.floor(worldPosition.z / BlockSize)
            );
        }
        
        /// <summary>
        /// Запрашивает контакт с грязью
        /// </summary>
        private MudContact QueryContact(float3 wheelPosition, float wheelRadius, int2 blockCoords)
        {
            var result = new MudContact
            {
                SinkDepth = 0f,
                TractionModifier = 0.8f,
                Viscosity = 0.5f,
                Density = 1.2f
            };
            
            // Получаем высоту террейна в блоке
            float terrainHeight = GetTerrainHeight(wheelPosition.xz, blockCoords);
            float sinkDepth = terrainHeight - wheelPosition.y;
            
            if (sinkDepth > 0)
            {
                result.SinkDepth = math.min(sinkDepth, MaxSinkDepth);
                result.TractionModifier = math.lerp(0.8f, 0.2f, result.SinkDepth / MaxSinkDepth);
                
                // Влияние вязкости на сцепление
                float viscosityFactor = math.lerp(1f, 0.5f, result.Viscosity);
                result.TractionModifier *= viscosityFactor;
            }
            
            return result;
        }
        
        /// <summary>
        /// Получает высоту террейна в блоке
        /// </summary>
        private float GetTerrainHeight(float2 xzPosition, int2 blockCoords)
        {
            // Проверяем кэш
            if (HeightCache.TryGetValue(blockCoords, out var heightData))
            {
                // Вычисляем локальные координаты в блоке
                float2 localPos = xzPosition - blockCoords * BlockSize;
                float2 normalizedPos = localPos / BlockSize;
                
                // Билинейная интерполяция высоты
                return BilinearInterpolation(heightData, normalizedPos, HeightResolution);
            }
            
            // Если блока нет в кэше, возвращаем базовую высоту
            return 0f;
        }
        
        /// <summary>
        /// Билинейная интерполяция высоты
        /// </summary>
        private float BilinearInterpolation(NativeArray<float> heightData, float2 normalizedPos, int resolution)
        {
            float x = normalizedPos.x * (resolution - 1);
            float y = normalizedPos.y * (resolution - 1);
            
            int x1 = (int)math.floor(x);
            int y1 = (int)math.floor(y);
            int x2 = math.min(x1 + 1, resolution - 1);
            int y2 = math.min(y1 + 1, resolution - 1);
            
            float fx = x - x1;
            float fy = y - y1;
            
            float h11 = heightData[y1 * resolution + x1];
            float h21 = heightData[y1 * resolution + x2];
            float h12 = heightData[y2 * resolution + x1];
            float h22 = heightData[y2 * resolution + x2];
            
            float h1 = math.lerp(h11, h21, fx);
            float h2 = math.lerp(h12, h22, fx);
            
            return math.lerp(h1, h2, fy);
        }
        
        /// <summary>
        /// Применяет деформацию террейна
        /// </summary>
        private void ApplyTerrainDeformation(int2 blockCoords, float3 wheelPosition, float wheelRadius, float sinkDepth)
        {
            if (sinkDepth <= 0f) return;
            
            // Получаем или создаем данные высоты блока
            if (!HeightCache.TryGetValue(blockCoords, out var heightData))
            {
                heightData = new NativeArray<float>(HeightResolution * HeightResolution, Allocator.Persistent);
                HeightCache[blockCoords] = heightData;
            }
            
            // Вычисляем локальные координаты в блоке
            float2 localPos = wheelPosition.xz - blockCoords * BlockSize;
            float2 normalizedPos = localPos / BlockSize;
            
            // Применяем деформацию в радиусе колеса
            float deformationRadius = wheelRadius / BlockSize;
            int radiusInPixels = (int)math.ceil(deformationRadius * HeightResolution);
            
            for (int dy = -radiusInPixels; dy <= radiusInPixels; dy++)
            {
                for (int dx = -radiusInPixels; dx <= radiusInPixels; dx++)
                {
                    int2 pixelPos = new int2(
                        (int)(normalizedPos.x * HeightResolution) + dx,
                        (int)(normalizedPos.y * HeightResolution) + dy
                    );
                    
                    if (pixelPos.x < 0 || pixelPos.x >= HeightResolution ||
                        pixelPos.y < 0 || pixelPos.y >= HeightResolution)
                        continue;
                    
                    // Вычисляем расстояние от центра колеса
                    float2 pixelWorldPos = (pixelPos + 0.5f) / HeightResolution;
                    float distance = math.length(pixelWorldPos - normalizedPos);
                    
                    if (distance <= deformationRadius)
                    {
                        // Вычисляем силу деформации
                        float deformationStrength = 1f - (distance / deformationRadius);
                        float deformationAmount = sinkDepth * deformationStrength * MaxDeformationPerFrame;
                        
                        // Применяем деформацию
                        int index = pixelPos.y * HeightResolution + pixelPos.x;
                        heightData[index] = math.max(heightData[index] - deformationAmount, -MaxSinkDepth);
                    }
                }
            }
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