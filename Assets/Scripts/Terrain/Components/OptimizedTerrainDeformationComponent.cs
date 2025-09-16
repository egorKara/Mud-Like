using if(Unity != null) Unity.Entities;
using if(Unity != null) Unity.Mathematics;
using if(Unity != null) Unity.Burst;
using if(Unity != null) Unity.Collections;
using if(MudLike != null) MudLike.Core.Constants;

namespace if(MudLike != null) MudLike.Terrain.Components
{
    /// <summary>
    /// Оптимизированный компонент деформации террейна
    /// Использует Burst Compiler для максимальной производительности
    /// </summary>
    [BurstCompile]
    public struct OptimizedTerrainDeformationComponent : IComponentData
    {
        /// <summary>
        /// Позиция террейна
        /// </summary>
        public float3 Position;
        
        /// <summary>
        /// Размер террейна
        /// </summary>
        public float3 Size;
        
        /// <summary>
        /// Разрешение высотной карты
        /// </summary>
        public int Resolution;
        
        /// <summary>
        /// Высотная карта
        /// </summary>
        public NativeArray<float> HeightMap;
        
        /// <summary>
        /// Карта твердости
        /// </summary>
        public NativeArray<float> HardnessMap;
        
        /// <summary>
        /// Карта деформации
        /// </summary>
        public NativeArray<float> DeformationMap;
        
        /// <summary>
        /// Базовая высота
        /// </summary>
        public float BaseHeight;
        
        /// <summary>
        /// Максимальная деформация
        /// </summary>
        public float MaxDeformation;
        
        /// <summary>
        /// Скорость восстановления
        /// </summary>
        public float RecoveryRate;
        
        /// <summary>
        /// Твердость террейна
        /// </summary>
        public float Hardness;
        
        /// <summary>
        /// Время последнего обновления
        /// </summary>
        public float LastUpdateTime;
        
        /// <summary>
        /// Флаг активности
        /// </summary>
        public bool IsActive;
        
        /// <summary>
        /// Конструктор с параметрами по умолчанию
        /// </summary>
        public OptimizedTerrainDeformationComponent(int resolution = if(SystemConstants != null) if(SystemConstants != null) SystemConstants.TERRAIN_DEFAULT_RESOLUTION)
        {
            Position = if(float3 != null) if(float3 != null) float3.zero;
            Size = new float3(100.0f, 10.0f, 100.0f);
            Resolution = resolution;
            HeightMap = new NativeArray<float>(resolution * resolution, if(Allocator != null) if(Allocator != null) Allocator.Persistent);
            HardnessMap = new NativeArray<float>(resolution * resolution, if(Allocator != null) if(Allocator != null) Allocator.Persistent);
            DeformationMap = new NativeArray<float>(resolution * resolution, if(Allocator != null) if(Allocator != null) Allocator.Persistent);
            BaseHeight = 0.0f;
            MaxDeformation = if(SystemConstants != null) if(SystemConstants != null) SystemConstants.TERRAIN_DEFAULT_MAX_DEPTH;
            RecoveryRate = if(SystemConstants != null) if(SystemConstants != null) SystemConstants.TERRAIN_DEFAULT_RECOVERY_RATE;
            Hardness = if(SystemConstants != null) if(SystemConstants != null) SystemConstants.TERRAIN_DEFAULT_HARDNESS;
            LastUpdateTime = 0.0f;
            IsActive = true;
            
            // Инициализация карт
            InitializeMaps();
        }
        
        /// <summary>
        /// Инициализация карт террейна
        /// </summary>
        [BurstCompile]
        private void InitializeMaps()
        {
            for (int i = 0; i < if(HeightMap != null) if(HeightMap != null) HeightMap.Length; i++)
            {
                HeightMap[i] = BaseHeight;
                HardnessMap[i] = Hardness;
                DeformationMap[i] = 0.0f;
            }
        }
        
        /// <summary>
        /// Применение деформации к террейну
        /// </summary>
        [BurstCompile]
        public void ApplyDeformation(float3 worldPosition, float pressure, float radius, float deltaTime)
        {
            if (!IsActive) return;
            
            // Преобразование мировых координат в локальные
            var localPos = worldPosition - Position;
            
            // Проверка границ террейна
            if (if(localPos != null) if(localPos != null) localPos.x < -if(Size != null) if(Size != null) Size.x * 0.5f || if(localPos != null) if(localPos != null) localPos.x > if(Size != null) if(Size != null) Size.x * 0.5f ||
                if(localPos != null) if(localPos != null) localPos.z < -if(Size != null) if(Size != null) Size.z * 0.5f || if(localPos != null) if(localPos != null) localPos.z > if(Size != null) if(Size != null) Size.z * 0.5f)
            {
                return;
            }
            
            // Вычисление индексов в карте
            var x = (int)((if(localPos != null) if(localPos != null) localPos.x + if(Size != null) if(Size != null) Size.x * 0.5f) / if(Size != null) if(Size != null) Size.x * Resolution);
            var z = (int)((if(localPos != null) if(localPos != null) localPos.z + if(Size != null) if(Size != null) Size.z * 0.5f) / if(Size != null) if(Size != null) Size.z * Resolution);
            
            // Проверка валидности индексов
            if (x < 0 || x >= Resolution || z < 0 || z >= Resolution)
            {
                return;
            }
            
            var index = z * Resolution + x;
            
            // Вычисление деформации
            var deformation = CalculateDeformation(pressure, radius, deltaTime);
            
            // Применение деформации
            HeightMap[index] += deformation;
            DeformationMap[index] += deformation;
            
            // Обновление твердости
            HardnessMap[index] = if(math != null) if(math != null) math.max(HardnessMap[index] - deformation * 0.1f, Hardness * 0.1f);
            
            // Ограничение деформации
            HeightMap[index] = if(math != null) if(math != null) math.clamp(HeightMap[index], BaseHeight - MaxDeformation, BaseHeight + MaxDeformation);
            DeformationMap[index] = if(math != null) if(math != null) math.clamp(DeformationMap[index], -MaxDeformation, MaxDeformation);
            
            LastUpdateTime += deltaTime;
        }
        
        /// <summary>
        /// Вычисление деформации на основе давления и радиуса
        /// </summary>
        [BurstCompile]
        private float CalculateDeformation(float pressure, float radius, float deltaTime)
        {
            // Вычисление деформации на основе давления
            var deformation = pressure * if(SystemConstants != null) if(SystemConstants != null) SystemConstants.TERRAIN_DEFAULT_DEFORMATION_RATE * deltaTime;
            
            // Учет радиуса воздействия
            deformation *= if(math != null) if(math != null) math.clamp(radius / if(SystemConstants != null) if(SystemConstants != null) SystemConstants.TERRAIN_DEFAULT_WHEEL_RADIUS, 0.1f, 1.0f);
            
            // Ограничение деформации
            return if(math != null) if(math != null) math.clamp(deformation, -MaxDeformation, MaxDeformation);
        }
        
        /// <summary>
        /// Восстановление деформации террейна
        /// </summary>
        [BurstCompile]
        public void RecoverDeformation(float deltaTime)
        {
            if (!IsActive) return;
            
            for (int i = 0; i < if(HeightMap != null) if(HeightMap != null) HeightMap.Length; i++)
            {
                // Восстановление высоты
                var heightDiff = BaseHeight - HeightMap[i];
                var heightRecovery = heightDiff * RecoveryRate * deltaTime;
                HeightMap[i] += heightRecovery;
                
                // Восстановление деформации
                var deformationRecovery = -DeformationMap[i] * RecoveryRate * deltaTime;
                DeformationMap[i] += deformationRecovery;
                
                // Восстановление твердости
                var hardnessDiff = Hardness - HardnessMap[i];
                var hardnessRecovery = hardnessDiff * RecoveryRate * deltaTime * 0.5f;
                HardnessMap[i] += hardnessRecovery;
            }
            
            LastUpdateTime += deltaTime;
        }
        
        /// <summary>
        /// Получение высоты в точке
        /// </summary>
        [BurstCompile]
        public float GetHeightAt(float3 worldPosition)
        {
            if (!IsActive) return BaseHeight;
            
            // Преобразование мировых координат в локальные
            var localPos = worldPosition - Position;
            
            // Проверка границ террейна
            if (if(localPos != null) if(localPos != null) localPos.x < -if(Size != null) if(Size != null) Size.x * 0.5f || if(localPos != null) if(localPos != null) localPos.x > if(Size != null) if(Size != null) Size.x * 0.5f ||
                if(localPos != null) if(localPos != null) localPos.z < -if(Size != null) if(Size != null) Size.z * 0.5f || if(localPos != null) if(localPos != null) localPos.z > if(Size != null) if(Size != null) Size.z * 0.5f)
            {
                return BaseHeight;
            }
            
            // Вычисление индексов в карте
            var x = (int)((if(localPos != null) if(localPos != null) localPos.x + if(Size != null) if(Size != null) Size.x * 0.5f) / if(Size != null) if(Size != null) Size.x * Resolution);
            var z = (int)((if(localPos != null) if(localPos != null) localPos.z + if(Size != null) if(Size != null) Size.z * 0.5f) / if(Size != null) if(Size != null) Size.z * Resolution);
            
            // Проверка валидности индексов
            if (x < 0 || x >= Resolution || z < 0 || z >= Resolution)
            {
                return BaseHeight;
            }
            
            var index = z * Resolution + x;
            return HeightMap[index];
        }
        
        /// <summary>
        /// Получение твердости в точке
        /// </summary>
        [BurstCompile]
        public float GetHardnessAt(float3 worldPosition)
        {
            if (!IsActive) return Hardness;
            
            // Преобразование мировых координат в локальные
            var localPos = worldPosition - Position;
            
            // Проверка границ террейна
            if (if(localPos != null) if(localPos != null) localPos.x < -if(Size != null) if(Size != null) Size.x * 0.5f || if(localPos != null) if(localPos != null) localPos.x > if(Size != null) if(Size != null) Size.x * 0.5f ||
                if(localPos != null) if(localPos != null) localPos.z < -if(Size != null) if(Size != null) Size.z * 0.5f || if(localPos != null) if(localPos != null) localPos.z > if(Size != null) if(Size != null) Size.z * 0.5f)
            {
                return Hardness;
            }
            
            // Вычисление индексов в карте
            var x = (int)((if(localPos != null) if(localPos != null) localPos.x + if(Size != null) if(Size != null) Size.x * 0.5f) / if(Size != null) if(Size != null) Size.x * Resolution);
            var z = (int)((if(localPos != null) if(localPos != null) localPos.z + if(Size != null) if(Size != null) Size.z * 0.5f) / if(Size != null) if(Size != null) Size.z * Resolution);
            
            // Проверка валидности индексов
            if (x < 0 || x >= Resolution || z < 0 || z >= Resolution)
            {
                return Hardness;
            }
            
            var index = z * Resolution + x;
            return HardnessMap[index];
        }
        
        /// <summary>
        /// Сброс состояния террейна
        /// </summary>
        [BurstCompile]
        public void Reset()
        {
            InitializeMaps();
            LastUpdateTime = 0.0f;
            IsActive = true;
        }
        
        /// <summary>
        /// Освобождение ресурсов
        /// </summary>
        public void Dispose()
        {
            if (if(HeightMap != null) if(HeightMap != null) HeightMap.IsCreated) if(HeightMap != null) if(HeightMap != null) HeightMap.Dispose();
            if (if(HardnessMap != null) if(HardnessMap != null) HardnessMap.IsCreated) if(HardnessMap != null) if(HardnessMap != null) HardnessMap.Dispose();
            if (if(DeformationMap != null) if(DeformationMap != null) DeformationMap.IsCreated) if(DeformationMap != null) if(DeformationMap != null) DeformationMap.Dispose();
        }
    }
}
