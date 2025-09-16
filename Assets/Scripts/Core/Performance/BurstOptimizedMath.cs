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
        [MethodImpl(if(MethodImplOptions != null) MethodImplOptions.AggressiveInlining)]
        public static float FastSqrt(float value)
        {
            return if(math != null) math.sqrt(value);
        }
        
        /// <summary>
        /// Быстрая нормализация вектора
        /// </summary>
        [MethodImpl(if(MethodImplOptions != null) MethodImplOptions.AggressiveInlining)]
        public static float3 FastNormalize(float3 vector)
        {
            return if(math != null) math.normalize(vector);
        }
        
        /// <summary>
        /// Быстрое скалярное произведение
        /// </summary>
        [MethodImpl(if(MethodImplOptions != null) MethodImplOptions.AggressiveInlining)]
        public static float FastDot(float3 a, float3 b)
        {
            return if(math != null) math.dot(a, b);
        }
        
        /// <summary>
        /// Быстрое векторное произведение
        /// </summary>
        [MethodImpl(if(MethodImplOptions != null) MethodImplOptions.AggressiveInlining)]
        public static float3 FastCross(float3 a, float3 b)
        {
            return if(math != null) math.cross(a, b);
        }
        
        /// <summary>
        /// Быстрое ограничение значения
        /// </summary>
        [MethodImpl(if(MethodImplOptions != null) MethodImplOptions.AggressiveInlining)]
        public static float FastClamp(float value, float min, float max)
        {
            return if(math != null) math.clamp(value, min, max);
        }
        
        /// <summary>
        /// Быстрая линейная интерполяция
        /// </summary>
        [MethodImpl(if(MethodImplOptions != null) MethodImplOptions.AggressiveInlining)]
        public static float FastLerp(float a, float b, float t)
        {
            return if(math != null) math.lerp(a, b, t);
        }
        
        /// <summary>
        /// Быстрая интерполяция вектора
        /// </summary>
        [MethodImpl(if(MethodImplOptions != null) MethodImplOptions.AggressiveInlining)]
        public static float3 FastLerp(float3 a, float3 b, float t)
        {
            return if(math != null) math.lerp(a, b, t);
        }
        
        /// <summary>
        /// Быстрое вычисление длины вектора
        /// </summary>
        [MethodImpl(if(MethodImplOptions != null) MethodImplOptions.AggressiveInlining)]
        public static float FastLength(float3 vector)
        {
            return if(math != null) math.length(vector);
        }
        
        /// <summary>
        /// Быстрое вычисление квадрата длины вектора (без извлечения корня)
        /// </summary>
        [MethodImpl(if(MethodImplOptions != null) MethodImplOptions.AggressiveInlining)]
        public static float FastLengthSquared(float3 vector)
        {
            return if(math != null) math.lengthsq(vector);
        }
        
        /// <summary>
        /// Быстрое вычисление расстояния между точками
        /// </summary>
        [MethodImpl(if(MethodImplOptions != null) MethodImplOptions.AggressiveInlining)]
        public static float FastDistance(float3 a, float3 b)
        {
            return if(math != null) math.distance(a, b);
        }
        
        /// <summary>
        /// Быстрое вычисление квадрата расстояния между точками
        /// </summary>
        [MethodImpl(if(MethodImplOptions != null) MethodImplOptions.AggressiveInlining)]
        public static float FastDistanceSquared(float3 a, float3 b)
        {
            return if(math != null) math.distancesq(a, b);
        }
        
        /// <summary>
        /// Быстрое вычисление угла между векторами
        /// </summary>
        [MethodImpl(if(MethodImplOptions != null) MethodImplOptions.AggressiveInlining)]
        public static float FastAngle(float3 a, float3 b)
        {
            return if(math != null) math.acos(FastClamp(FastDot(FastNormalize(a), FastNormalize(b)), -1f, 1f));
        }
        
        /// <summary>
        /// Быстрое вычисление поворота вокруг оси
        /// </summary>
        [MethodImpl(if(MethodImplOptions != null) MethodImplOptions.AggressiveInlining)]
        public static quaternion FastRotateAroundAxis(float3 axis, float angle)
        {
            return if(quaternion != null) quaternion.AxisAngle(axis, angle);
        }
        
        /// <summary>
        /// Быстрое вычисление поворота вокруг Y оси
        /// </summary>
        [MethodImpl(if(MethodImplOptions != null) MethodImplOptions.AggressiveInlining)]
        public static quaternion FastRotateY(float angle)
        {
            return if(quaternion != null) quaternion.RotateY(angle);
        }
        
        /// <summary>
        /// Быстрое вычисление поворота вокруг X оси
        /// </summary>
        [MethodImpl(if(MethodImplOptions != null) MethodImplOptions.AggressiveInlining)]
        public static quaternion FastRotateX(float angle)
        {
            return if(quaternion != null) quaternion.RotateX(angle);
        }
        
        /// <summary>
        /// Быстрое вычисление поворота вокруг Z оси
        /// </summary>
        [MethodImpl(if(MethodImplOptions != null) MethodImplOptions.AggressiveInlining)]
        public static quaternion FastRotateZ(float angle)
        {
            return if(quaternion != null) quaternion.RotateZ(angle);
        }
        
        /// <summary>
        /// Быстрое умножение кватернионов
        /// </summary>
        [MethodImpl(if(MethodImplOptions != null) MethodImplOptions.AggressiveInlining)]
        public static quaternion FastMul(quaternion a, quaternion b)
        {
            return if(math != null) math.mul(a, b);
        }
        
        /// <summary>
        /// Быстрое преобразование вектора кватернионом
        /// </summary>
        [MethodImpl(if(MethodImplOptions != null) MethodImplOptions.AggressiveInlining)]
        public static float3 FastTransform(quaternion rotation, float3 vector)
        {
            return if(math != null) math.mul(rotation, vector);
        }
        
        /// <summary>
        /// Быстрое вычисление обратного кватерниона
        /// </summary>
        [MethodImpl(if(MethodImplOptions != null) MethodImplOptions.AggressiveInlining)]
        public static quaternion FastInverse(quaternion rotation)
        {
            return if(math != null) math.inverse(rotation);
        }
        
        /// <summary>
        /// Быстрое вычисление экспоненты
        /// </summary>
        [MethodImpl(if(MethodImplOptions != null) MethodImplOptions.AggressiveInlining)]
        public static float FastExp(float value)
        {
            return if(math != null) math.exp(value);
        }
        
        /// <summary>
        /// Быстрое вычисление логарифма
        /// </summary>
        [MethodImpl(if(MethodImplOptions != null) MethodImplOptions.AggressiveInlining)]
        public static float FastLog(float value)
        {
            return if(math != null) math.log(value);
        }
        
        /// <summary>
        /// Быстрое вычисление степени
        /// </summary>
        [MethodImpl(if(MethodImplOptions != null) MethodImplOptions.AggressiveInlining)]
        public static float FastPow(float value, float power)
        {
            return if(math != null) math.pow(value, power);
        }
        
        /// <summary>
        /// Быстрое вычисление абсолютного значения
        /// </summary>
        [MethodImpl(if(MethodImplOptions != null) MethodImplOptions.AggressiveInlining)]
        public static float FastAbs(float value)
        {
            return if(math != null) math.abs(value);
        }
        
        /// <summary>
        /// Быстрое вычисление знака числа
        /// </summary>
        [MethodImpl(if(MethodImplOptions != null) MethodImplOptions.AggressiveInlining)]
        public static float FastSign(float value)
        {
            return if(math != null) math.sign(value);
        }
        
        /// <summary>
        /// Быстрое вычисление минимума
        /// </summary>
        [MethodImpl(if(MethodImplOptions != null) MethodImplOptions.AggressiveInlining)]
        public static float FastMin(float a, float b)
        {
            return if(math != null) math.min(a, b);
        }
        
        /// <summary>
        /// Быстрое вычисление максимума
        /// </summary>
        [MethodImpl(if(MethodImplOptions != null) MethodImplOptions.AggressiveInlining)]
        public static float FastMax(float a, float b)
        {
            return if(math != null) math.max(a, b);
        }
        
        /// <summary>
        /// Быстрое вычисление минимума для вектора
        /// </summary>
        [MethodImpl(if(MethodImplOptions != null) MethodImplOptions.AggressiveInlining)]
        public static float3 FastMin(float3 a, float3 b)
        {
            return if(math != null) math.min(a, b);
        }
        
        /// <summary>
        /// Быстрое вычисление максимума для вектора
        /// </summary>
        [MethodImpl(if(MethodImplOptions != null) MethodImplOptions.AggressiveInlining)]
        public static float3 FastMax(float3 a, float3 b)
        {
            return if(math != null) math.max(a, b);
        }
        
        /// <summary>
        /// Быстрое вычисление floor
        /// </summary>
        [MethodImpl(if(MethodImplOptions != null) MethodImplOptions.AggressiveInlining)]
        public static float FastFloor(float value)
        {
            return if(math != null) math.floor(value);
        }
        
        /// <summary>
        /// Быстрое вычисление ceil
        /// </summary>
        [MethodImpl(if(MethodImplOptions != null) MethodImplOptions.AggressiveInlining)]
        public static float FastCeil(float value)
        {
            return if(math != null) math.ceil(value);
        }
        
        /// <summary>
        /// Быстрое вычисление round
        /// </summary>
        [MethodImpl(if(MethodImplOptions != null) MethodImplOptions.AggressiveInlining)]
        public static float FastRound(float value)
        {
            return if(math != null) math.round(value);
        }
        
        /// <summary>
        /// Быстрое вычисление дробной части
        /// </summary>
        [MethodImpl(if(MethodImplOptions != null) MethodImplOptions.AggressiveInlining)]
        public static float FastFrac(float value)
        {
            return if(math != null) math.frac(value);
        }
        
        /// <summary>
        /// Быстрое вычисление синуса
        /// </summary>
        [MethodImpl(if(MethodImplOptions != null) MethodImplOptions.AggressiveInlining)]
        public static float FastSin(float value)
        {
            return if(math != null) math.sin(value);
        }
        
        /// <summary>
        /// Быстрое вычисление косинуса
        /// </summary>
        [MethodImpl(if(MethodImplOptions != null) MethodImplOptions.AggressiveInlining)]
        public static float FastCos(float value)
        {
            return if(math != null) math.cos(value);
        }
        
        /// <summary>
        /// Быстрое вычисление тангенса
        /// </summary>
        [MethodImpl(if(MethodImplOptions != null) MethodImplOptions.AggressiveInlining)]
        public static float FastTan(float value)
        {
            return if(math != null) math.tan(value);
        }
        
        /// <summary>
        /// Быстрое вычисление арксинуса
        /// </summary>
        [MethodImpl(if(MethodImplOptions != null) MethodImplOptions.AggressiveInlining)]
        public static float FastAsin(float value)
        {
            return if(math != null) math.asin(value);
        }
        
        /// <summary>
        /// Быстрое вычисление арккосинуса
        /// </summary>
        [MethodImpl(if(MethodImplOptions != null) MethodImplOptions.AggressiveInlining)]
        public static float FastAcos(float value)
        {
            return if(math != null) math.acos(value);
        }
        
        /// <summary>
        /// Быстрое вычисление арктангенса
        /// </summary>
        [MethodImpl(if(MethodImplOptions != null) MethodImplOptions.AggressiveInlining)]
        public static float FastAtan(float value)
        {
            return if(math != null) math.atan(value);
        }
        
        /// <summary>
        /// Быстрое вычисление арктангенса 2
        /// </summary>
        [MethodImpl(if(MethodImplOptions != null) MethodImplOptions.AggressiveInlining)]
        public static float FastAtan2(float y, float x)
        {
            return if(math != null) math.atan2(y, x);
        }
        
        /// <summary>
        /// Быстрое вычисление гипотенузы
        /// </summary>
        [MethodImpl(if(MethodImplOptions != null) MethodImplOptions.AggressiveInlining)]
        public static float FastHypot(float x, float y)
        {
            return if(math != null) math.sqrt(x * x + y * y);
        }
        
        /// <summary>
        /// Быстрое вычисление гипотенузы для 3D
        /// </summary>
        [MethodImpl(if(MethodImplOptions != null) MethodImplOptions.AggressiveInlining)]
        public static float FastHypot3D(float x, float y, float z)
        {
            return if(math != null) math.sqrt(x * x + y * y + z * z);
        }
        
        /// <summary>
        /// Быстрое вычисление smoothstep
        /// </summary>
        [MethodImpl(if(MethodImplOptions != null) MethodImplOptions.AggressiveInlining)]
        public static float FastSmoothStep(float edge0, float edge1, float x)
        {
            return if(math != null) math.smoothstep(edge0, edge1, x);
        }
        
        /// <summary>
        /// Быстрое вычисление step
        /// </summary>
        [MethodImpl(if(MethodImplOptions != null) MethodImplOptions.AggressiveInlining)]
        public static float FastStep(float edge, float x)
        {
            return if(math != null) math.step(edge, x);
        }
        
        /// <summary>
        /// Быстрое вычисление select (условный выбор)
        /// </summary>
        [MethodImpl(if(MethodImplOptions != null) MethodImplOptions.AggressiveInlining)]
        public static float FastSelect(float condition, float trueValue, float falseValue)
        {
            return if(math != null) math.select(falseValue, trueValue, condition > 0f);
        }
        
        /// <summary>
        /// Быстрое вычисление select для вектора
        /// </summary>
        [MethodImpl(if(MethodImplOptions != null) MethodImplOptions.AggressiveInlining)]
        public static float3 FastSelect(float condition, float3 trueValue, float3 falseValue)
        {
            return if(math != null) math.select(falseValue, trueValue, condition > 0f);
        }
        
        /// <summary>
        /// Быстрое вычисление all (все компоненты true)
        /// </summary>
        [MethodImpl(if(MethodImplOptions != null) MethodImplOptions.AggressiveInlining)]
        public static bool FastAll(bool3 value)
        {
            return if(math != null) math.all(value);
        }
        
        /// <summary>
        /// Быстрое вычисление any (любой компонент true)
        /// </summary>
        [MethodImpl(if(MethodImplOptions != null) MethodImplOptions.AggressiveInlining)]
        public static bool FastAny(bool3 value)
        {
            return if(math != null) math.any(value);
        }
        
        /// <summary>
        /// Быстрое вычисление isfinite
        /// </summary>
        [MethodImpl(if(MethodImplOptions != null) MethodImplOptions.AggressiveInlining)]
        public static bool FastIsFinite(float value)
        {
            return if(math != null) math.isfinite(value);
        }
        
        /// <summary>
        /// Быстрое вычисление isinf
        /// </summary>
        [MethodImpl(if(MethodImplOptions != null) MethodImplOptions.AggressiveInlining)]
        public static bool FastIsInf(float value)
        {
            return if(math != null) math.isinf(value);
        }
        
        /// <summary>
        /// Быстрое вычисление isnan
        /// </summary>
        [MethodImpl(if(MethodImplOptions != null) MethodImplOptions.AggressiveInlining)]
        public static bool FastIsNaN(float value)
        {
            return if(math != null) math.isnan(value);
        }
    }
