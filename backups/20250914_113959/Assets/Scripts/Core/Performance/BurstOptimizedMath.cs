using Unity.Burst;
using Unity.Mathematics;
using System.Runtime.CompilerServices;

namespace MudLike.Core.Performance
{
    /// <summary>
    /// Burst-оптимизированные математические операции
    /// </summary>
    [BurstCompile]
    public static class BurstOptimizedMath
    {
        /// <summary>
        /// Быстрое вычисление квадратного корня
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float FastSqrt(float value)
        {
            return math.sqrt(value);
        }
        
        /// <summary>
        /// Быстрая нормализация вектора
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 FastNormalize(float3 vector)
        {
            return math.normalize(vector);
        }
        
        /// <summary>
        /// Быстрое скалярное произведение
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float FastDot(float3 a, float3 b)
        {
            return math.dot(a, b);
        }
        
        /// <summary>
        /// Быстрое векторное произведение
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 FastCross(float3 a, float3 b)
        {
            return math.cross(a, b);
        }
        
        /// <summary>
        /// Быстрое ограничение значения
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float FastClamp(float value, float min, float max)
        {
            return math.clamp(value, min, max);
        }
        
        /// <summary>
        /// Быстрая линейная интерполяция
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float FastLerp(float a, float b, float t)
        {
            return math.lerp(a, b, t);
        }
        
        /// <summary>
        /// Быстрая интерполяция вектора
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 FastLerp(float3 a, float3 b, float t)
        {
            return math.lerp(a, b, t);
        }
        
        /// <summary>
        /// Быстрое вычисление длины вектора
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float FastLength(float3 vector)
        {
            return math.length(vector);
        }
        
        /// <summary>
        /// Быстрое вычисление квадрата длины вектора (без извлечения корня)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float FastLengthSquared(float3 vector)
        {
            return math.lengthsq(vector);
        }
        
        /// <summary>
        /// Быстрое вычисление расстояния между точками
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float FastDistance(float3 a, float3 b)
        {
            return math.distance(a, b);
        }
        
        /// <summary>
        /// Быстрое вычисление квадрата расстояния между точками
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float FastDistanceSquared(float3 a, float3 b)
        {
            return math.distancesq(a, b);
        }
        
        /// <summary>
        /// Быстрое вычисление угла между векторами
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float FastAngle(float3 a, float3 b)
        {
            return math.acos(FastClamp(FastDot(FastNormalize(a), FastNormalize(b)), -1f, 1f));
        }
        
        /// <summary>
        /// Быстрое вычисление поворота вокруг оси
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static quaternion FastRotateAroundAxis(float3 axis, float angle)
        {
            return quaternion.AxisAngle(axis, angle);
        }
        
        /// <summary>
        /// Быстрое вычисление поворота вокруг Y оси
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static quaternion FastRotateY(float angle)
        {
            return quaternion.RotateY(angle);
        }
        
        /// <summary>
        /// Быстрое вычисление поворота вокруг X оси
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static quaternion FastRotateX(float angle)
        {
            return quaternion.RotateX(angle);
        }
        
        /// <summary>
        /// Быстрое вычисление поворота вокруг Z оси
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static quaternion FastRotateZ(float angle)
        {
            return quaternion.RotateZ(angle);
        }
        
        /// <summary>
        /// Быстрое умножение кватернионов
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static quaternion FastMul(quaternion a, quaternion b)
        {
            return math.mul(a, b);
        }
        
        /// <summary>
        /// Быстрое преобразование вектора кватернионом
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 FastTransform(quaternion rotation, float3 vector)
        {
            return math.mul(rotation, vector);
        }
        
        /// <summary>
        /// Быстрое вычисление обратного кватерниона
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static quaternion FastInverse(quaternion rotation)
        {
            return math.inverse(rotation);
        }
        
        /// <summary>
        /// Быстрое вычисление экспоненты
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float FastExp(float value)
        {
            return math.exp(value);
        }
        
        /// <summary>
        /// Быстрое вычисление логарифма
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float FastLog(float value)
        {
            return math.log(value);
        }
        
        /// <summary>
        /// Быстрое вычисление степени
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float FastPow(float value, float power)
        {
            return math.pow(value, power);
        }
        
        /// <summary>
        /// Быстрое вычисление абсолютного значения
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float FastAbs(float value)
        {
            return math.abs(value);
        }
        
        /// <summary>
        /// Быстрое вычисление знака числа
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float FastSign(float value)
        {
            return math.sign(value);
        }
        
        /// <summary>
        /// Быстрое вычисление минимума
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float FastMin(float a, float b)
        {
            return math.min(a, b);
        }
        
        /// <summary>
        /// Быстрое вычисление максимума
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float FastMax(float a, float b)
        {
            return math.max(a, b);
        }
        
        /// <summary>
        /// Быстрое вычисление минимума для вектора
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 FastMin(float3 a, float3 b)
        {
            return math.min(a, b);
        }
        
        /// <summary>
        /// Быстрое вычисление максимума для вектора
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 FastMax(float3 a, float3 b)
        {
            return math.max(a, b);
        }
        
        /// <summary>
        /// Быстрое вычисление floor
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float FastFloor(float value)
        {
            return math.floor(value);
        }
        
        /// <summary>
        /// Быстрое вычисление ceil
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float FastCeil(float value)
        {
            return math.ceil(value);
        }
        
        /// <summary>
        /// Быстрое вычисление round
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float FastRound(float value)
        {
            return math.round(value);
        }
        
        /// <summary>
        /// Быстрое вычисление дробной части
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float FastFrac(float value)
        {
            return math.frac(value);
        }
        
        /// <summary>
        /// Быстрое вычисление синуса
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float FastSin(float value)
        {
            return math.sin(value);
        }
        
        /// <summary>
        /// Быстрое вычисление косинуса
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float FastCos(float value)
        {
            return math.cos(value);
        }
        
        /// <summary>
        /// Быстрое вычисление тангенса
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float FastTan(float value)
        {
            return math.tan(value);
        }
        
        /// <summary>
        /// Быстрое вычисление арксинуса
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float FastAsin(float value)
        {
            return math.asin(value);
        }
        
        /// <summary>
        /// Быстрое вычисление арккосинуса
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float FastAcos(float value)
        {
            return math.acos(value);
        }
        
        /// <summary>
        /// Быстрое вычисление арктангенса
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float FastAtan(float value)
        {
            return math.atan(value);
        }
        
        /// <summary>
        /// Быстрое вычисление арктангенса 2
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float FastAtan2(float y, float x)
        {
            return math.atan2(y, x);
        }
        
        /// <summary>
        /// Быстрое вычисление гипотенузы
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float FastHypot(float x, float y)
        {
            return math.sqrt(x * x + y * y);
        }
        
        /// <summary>
        /// Быстрое вычисление гипотенузы для 3D
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float FastHypot3D(float x, float y, float z)
        {
            return math.sqrt(x * x + y * y + z * z);
        }
        
        /// <summary>
        /// Быстрое вычисление smoothstep
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float FastSmoothStep(float edge0, float edge1, float x)
        {
            return math.smoothstep(edge0, edge1, x);
        }
        
        /// <summary>
        /// Быстрое вычисление step
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float FastStep(float edge, float x)
        {
            return math.step(edge, x);
        }
        
        /// <summary>
        /// Быстрое вычисление select (условный выбор)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float FastSelect(float condition, float trueValue, float falseValue)
        {
            return math.select(falseValue, trueValue, condition > 0f);
        }
        
        /// <summary>
        /// Быстрое вычисление select для вектора
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 FastSelect(float condition, float3 trueValue, float3 falseValue)
        {
            return math.select(falseValue, trueValue, condition > 0f);
        }
        
        /// <summary>
        /// Быстрое вычисление all (все компоненты true)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool FastAll(bool3 value)
        {
            return math.all(value);
        }
        
        /// <summary>
        /// Быстрое вычисление any (любой компонент true)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool FastAny(bool3 value)
        {
            return math.any(value);
        }
        
        /// <summary>
        /// Быстрое вычисление isfinite
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool FastIsFinite(float value)
        {
            return math.isfinite(value);
        }
        
        /// <summary>
        /// Быстрое вычисление isinf
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool FastIsInf(float value)
        {
            return math.isinf(value);
        }
        
        /// <summary>
        /// Быстрое вычисление isnan
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool FastIsNaN(float value)
        {
            return math.isnan(value);
        }
    }
}