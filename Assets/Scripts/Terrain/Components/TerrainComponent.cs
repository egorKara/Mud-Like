using Unity.Entities;
using Unity.Mathematics;

namespace MudLike.Terrain.Components
{
    /// <summary>
    /// ECS компонент для деформируемой местности
    /// </summary>
    public struct TerrainComponent : IComponentData
    {
        /// <summary>
        /// Размер сетки местности (количество точек по X)
        /// </summary>
        public int gridSizeX;
        
        /// <summary>
        /// Размер сетки местности (количество точек по Z)
        /// </summary>
        public int gridSizeZ;
        
        /// <summary>
        /// Размер одной ячейки сетки (м)
        /// </summary>
        public float cellSize;
        
        /// <summary>
        /// Общий размер местности по X (м)
        /// </summary>
        public float terrainSizeX;
        
        /// <summary>
        /// Общий размер местности по Z (м)
        /// </summary>
        public float terrainSizeZ;
        
        /// <summary>
        /// Минимальная высота местности (м)
        /// </summary>
        public float minHeight;
        
        /// <summary>
        /// Максимальная высота местности (м)
        /// </summary>
        public float maxHeight;
        
        /// <summary>
        /// Жесткость поверхности (Н/м)
        /// </summary>
        public float surfaceStiffness;
        
        /// <summary>
        /// Демпфирование поверхности (Н·с/м)
        /// </summary>
        public float surfaceDamping;
        
        /// <summary>
        /// Коэффициент трения поверхности
        /// </summary>
        public float surfaceFriction;
        
        /// <summary>
        /// Коэффициент сцепления с поверхностью
        /// </summary>
        public float surfaceGrip;
        
        /// <summary>
        /// Плотность материала поверхности (кг/м³)
        /// </summary>
        public float materialDensity;
        
        /// <summary>
        /// Прочность поверхности (Па)
        /// </summary>
        public float materialStrength;
        
        /// <summary>
        /// Пластичность материала (0-1)
        /// </summary>
        public float materialPlasticity;
        
        /// <summary>
        /// Вязкость материала (Па·с)
        /// </summary>
        public float materialViscosity;
        
        /// <summary>
        /// Флаг: активна ли деформация
        /// </summary>
        public bool deformationEnabled;
        
        /// <summary>
        /// Флаг: активна ли эрозия
        /// </summary>
        public bool erosionEnabled;
        
        /// <summary>
        /// Скорость восстановления поверхности (м/с)
        /// </summary>
        public float recoveryRate;
        
        /// <summary>
        /// Время восстановления (с)
        /// </summary>
        public float recoveryTime;
        
        /// <summary>
        /// Максимальная глубина деформации (м)
        /// </summary>
        public float maxDeformationDepth;
        
        /// <summary>
        /// Радиус влияния деформации (м)
        /// </summary>
        public float deformationRadius;
        
        /// <summary>
        /// Сила деформации
        /// </summary>
        public float deformationForce;
        
        /// <summary>
        /// Количество активных деформаций
        /// </summary>
        public int activeDeformations;
        
        /// <summary>
        /// Общая энергия деформации
        /// </summary>
        public float totalDeformationEnergy;
    }
}
