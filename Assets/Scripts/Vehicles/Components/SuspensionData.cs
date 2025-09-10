using Unity.Entities;
using Unity.Mathematics;

namespace MudLike.Vehicles.Components
{
    /// <summary>
    /// Данные подвески колеса
    /// </summary>
    public struct SuspensionData : IComponentData
    {
        /// <summary>
        /// Длина подвески в метрах
        /// </summary>
        public float Length;
        
        /// <summary>
        /// Текущая длина подвески
        /// </summary>
        public float CurrentLength;
        
        /// <summary>
        /// Жесткость пружины (Н/м)
        /// </summary>
        public float SpringForce;
        
        /// <summary>
        /// Коэффициент демпфирования
        /// </summary>
        public float DampingForce;
        
        /// <summary>
        /// Целевая позиция подвески
        /// </summary>
        public float3 TargetPosition;
        
        /// <summary>
        /// Скорость сжатия/растяжения подвески
        /// </summary>
        public float CompressionVelocity;
        
        /// <summary>
        /// Максимальное сжатие подвески
        /// </summary>
        public float MaxCompression;
        
        /// <summary>
        /// Максимальное растяжение подвески
        /// </summary>
        public float MaxExtension;
        
        /// <summary>
        /// Сила, приложенная к подвеске
        /// </summary>
        public float3 SuspensionForce;
        
        /// <summary>
        /// Индекс подвески (соответствует индексу колеса)
        /// </summary>
        public int SuspensionIndex;
    }
}