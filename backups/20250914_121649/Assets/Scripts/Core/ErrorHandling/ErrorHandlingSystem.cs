using Unity.Entities;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Collections;
using UnityEngine;
using System;

namespace MudLike.Core.ErrorHandling
{
    /// <summary>
    /// Централизованная система обработки ошибок и валидации данных
    /// Обеспечивает надежность и стабильность ECS систем
    /// </summary>
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [BurstCompile]
    public partial class ErrorHandlingSystem : SystemBase
    {
        private NativeHashMap<int, ErrorData> _errorHistory;
        private NativeQueue<ErrorEvent> _errorQueue;
        private int _errorCount;
        private const int MAX_ERROR_HISTORY = 1000;
        
        protected override void OnCreate()
        {
            _errorHistory = new NativeHashMap<int, ErrorData>(MAX_ERROR_HISTORY, Allocator.Persistent);
            _errorQueue = new NativeQueue<ErrorEvent>(Allocator.Persistent);
            _errorCount = 0;
        }
        
        protected override void OnDestroy()
        {
            if (_errorHistory.IsCreated)
                _errorHistory.Dispose();
            if (_errorQueue.IsCreated)
                _errorQueue.Dispose();
        }
        
        protected override void OnUpdate()
        {
            // Обрабатываем очередь ошибок
            ProcessErrorQueue();
            
            // Проверяем критические условия
            ValidateSystemHealth();
        }
        
        /// <summary>
        /// Обрабатывает очередь ошибок
        /// </summary>
        private void ProcessErrorQueue()
        {
            while (_errorQueue.TryDequeue(out var errorEvent))
            {
                HandleError(errorEvent);
            }
        }
        
        /// <summary>
        /// Обрабатывает ошибку
        /// </summary>
        private void HandleError(ErrorEvent errorEvent)
        {
            var errorData = new ErrorData
            {
                ErrorType = errorEvent.ErrorType,
                Message = errorEvent.Message,
                Timestamp = SystemAPI.Time.time,
                SystemName = errorEvent.SystemName,
                Severity = errorEvent.Severity
            };
            
            // Добавляем в историю
            _errorHistory[_errorCount] = errorData;
            _errorCount = (_errorCount + 1) % MAX_ERROR_HISTORY;
            
            // Логируем в зависимости от серьезности
            switch (errorEvent.Severity)
            {
                case ErrorSeverity.Info:
                    Debug.Log($"[{errorEvent.SystemName}] {errorEvent.Message}");
                    break;
                case ErrorSeverity.Warning:
                    Debug.LogWarning($"[{errorEvent.SystemName}] {errorEvent.Message}");
                    break;
                case ErrorSeverity.Error:
                    Debug.LogError($"[{errorEvent.SystemName}] {errorEvent.Message}");
                    break;
                case ErrorSeverity.Critical:
                    Debug.LogError($"[CRITICAL] [{errorEvent.SystemName}] {errorEvent.Message}");
                    break;
            }
        }
        
        /// <summary>
        /// Проверяет здоровье систем
        /// </summary>
        private void ValidateSystemHealth()
        {
            // Проверяем количество ошибок за последнюю минуту
            float currentTime = SystemAPI.Time.time;
            int recentErrors = 0;
            
            for (int i = 0; i < _errorHistory.Count; i++)
            {
                if (_errorHistory.TryGetValue(i, out var errorData))
                {
                    if (currentTime - errorData.Timestamp < 60f) // Последняя минута
                    {
                        recentErrors++;
                    }
                }
            }
            
            // Если слишком много ошибок - предупреждение
            if (recentErrors > 100)
            {
                Debug.LogWarning($"[ErrorHandling] High error rate detected: {recentErrors} errors in the last minute");
            }
        }
        
        /// <summary>
        /// Регистрирует ошибку
        /// </summary>
        public void LogError(ErrorType errorType, string message, string systemName, ErrorSeverity severity = ErrorSeverity.Error)
        {
            var errorEvent = new ErrorEvent
            {
                ErrorType = errorType,
                Message = message,
                SystemName = systemName,
                Severity = severity,
                Timestamp = SystemAPI.Time.time
            };
            
            _errorQueue.Enqueue(errorEvent);
        }
        
        /// <summary>
        /// Валидирует входные данные
        /// </summary>
        [BurstCompile]
        public bool ValidateInputData(float3 position, float radius, float force)
        {
            // Проверяем на NaN и Infinity
            if (math.any(math.isnan(position)) || math.any(math.isinf(position)))
            {
                LogError(ErrorType.InvalidInput, "Position contains NaN or Infinity values", "Validation");
                return false;
            }
            
            if (math.isnan(radius) || math.isinf(radius))
            {
                LogError(ErrorType.InvalidInput, "Radius contains NaN or Infinity values", "Validation");
                return false;
            }
            
            if (math.isnan(force) || math.isinf(force))
            {
                LogError(ErrorType.InvalidInput, "Force contains NaN or Infinity values", "Validation");
                return false;
            }
            
            // Проверяем разумные диапазоны
            if (radius <= 0f)
            {
                LogError(ErrorType.InvalidInput, "Radius must be positive", "Validation", ErrorSeverity.Warning);
                return false;
            }
            
            if (radius > 10f)
            {
                LogError(ErrorType.InvalidInput, "Radius too large", "Validation", ErrorSeverity.Warning);
                return false;
            }
            
            if (force < 0f)
            {
                LogError(ErrorType.InvalidInput, "Force cannot be negative", "Validation", ErrorSeverity.Warning);
                return false;
            }
            
            if (force > 100000f)
            {
                LogError(ErrorType.InvalidInput, "Force too large", "Validation", ErrorSeverity.Warning);
                return false;
            }
            
            return true;
        }
        
        /// <summary>
        /// Валидирует физические данные
        /// </summary>
        [BurstCompile]
        public bool ValidatePhysicsData(float3 velocity, float3 acceleration, float mass)
        {
            // Проверяем скорость на разумные значения
            float speed = math.length(velocity);
            if (speed > 100f) // 100 m/s = 360 km/h
            {
                LogError(ErrorType.PhysicsError, $"Velocity too high: {speed:F1} m/s", "PhysicsValidation", ErrorSeverity.Warning);
                return false;
            }
            
            // Проверяем ускорение
            float accelerationMagnitude = math.length(acceleration);
            if (accelerationMagnitude > 50f) // 50 m/s² = 5g
            {
                LogError(ErrorType.PhysicsError, $"Acceleration too high: {accelerationMagnitude:F1} m/s²", "PhysicsValidation", ErrorSeverity.Warning);
                return false;
            }
            
            // Проверяем массу
            if (mass <= 0f)
            {
                LogError(ErrorType.PhysicsError, "Mass must be positive", "PhysicsValidation", ErrorSeverity.Error);
                return false;
            }
            
            if (mass > 50000f) // 50 тонн
            {
                LogError(ErrorType.PhysicsError, "Mass too large", "PhysicsValidation", ErrorSeverity.Warning);
                return false;
            }
            
            return true;
        }
        
        /// <summary>
        /// Валидирует сетевые данные
        /// </summary>
        [BurstCompile]
        public bool ValidateNetworkData(int playerId, float timestamp, float3 position)
        {
            // Проверяем ID игрока
            if (playerId < 0 || playerId > 10000)
            {
                LogError(ErrorType.NetworkError, $"Invalid player ID: {playerId}", "NetworkValidation", ErrorSeverity.Error);
                return false;
            }
            
            // Проверяем временную метку
            float currentTime = SystemAPI.Time.time;
            if (math.abs(timestamp - currentTime) > 5f) // 5 секунд разницы
            {
                LogError(ErrorType.NetworkError, $"Timestamp too old: {timestamp:F1} vs {currentTime:F1}", "NetworkValidation", ErrorSeverity.Warning);
                return false;
            }
            
            // Проверяем позицию
            if (math.any(math.isnan(position)) || math.any(math.isinf(position)))
            {
                LogError(ErrorType.NetworkError, "Network position contains invalid values", "NetworkValidation", ErrorSeverity.Error);
                return false;
            }
            
            return true;
        }
        
        /// <summary>
        /// Получает статистику ошибок
        /// </summary>
        public ErrorStatistics GetErrorStatistics()
        {
            var statistics = new ErrorStatistics();
            float currentTime = SystemAPI.Time.time;
            
            for (int i = 0; i < _errorHistory.Count; i++)
            {
                if (_errorHistory.TryGetValue(i, out var errorData))
                {
                    statistics.TotalErrors++;
                    
                    // Подсчитываем по типам
                    switch (errorData.ErrorType)
                    {
                        case ErrorType.InvalidInput:
                            statistics.InvalidInputErrors++;
                            break;
                        case ErrorType.PhysicsError:
                            statistics.PhysicsErrors++;
                            break;
                        case ErrorType.NetworkError:
                            statistics.NetworkErrors++;
                            break;
                        case ErrorType.SystemError:
                            statistics.SystemErrors++;
                            break;
                    }
                    
                    // Подсчитываем по серьезности
                    switch (errorData.Severity)
                    {
                        case ErrorSeverity.Critical:
                            statistics.CriticalErrors++;
                            break;
                        case ErrorSeverity.Error:
                            statistics.Errors++;
                            break;
                        case ErrorSeverity.Warning:
                            statistics.Warnings++;
                            break;
                        case ErrorSeverity.Info:
                            statistics.InfoMessages++;
                            break;
                    }
                    
                    // Ошибки за последнюю минуту
                    if (currentTime - errorData.Timestamp < 60f)
                    {
                        statistics.ErrorsLastMinute++;
                    }
                }
            }
            
            return statistics;
        }
    }
    
    /// <summary>
    /// Типы ошибок
    /// </summary>
    public enum ErrorType : byte
    {
        None,
        InvalidInput,
        PhysicsError,
        NetworkError,
        SystemError,
        PerformanceError,
        MemoryError
    }
    
    /// <summary>
    /// Серьезность ошибки
    /// </summary>
    public enum ErrorSeverity : byte
    {
        Info,
        Warning,
        Error,
        Critical
    }
    
    /// <summary>
    /// Данные об ошибке
    /// </summary>
    public struct ErrorData
    {
        public ErrorType ErrorType;
        public FixedString512Bytes Message;
        public float Timestamp;
        public FixedString64Bytes SystemName;
        public ErrorSeverity Severity;
    }
    
    /// <summary>
    /// Событие ошибки
    /// </summary>
    public struct ErrorEvent
    {
        public ErrorType ErrorType;
        public FixedString512Bytes Message;
        public FixedString64Bytes SystemName;
        public ErrorSeverity Severity;
        public float Timestamp;
    }
    
    /// <summary>
    /// Статистика ошибок
    /// </summary>
    public struct ErrorStatistics
    {
        public int TotalErrors;
        public int InvalidInputErrors;
        public int PhysicsErrors;
        public int NetworkErrors;
        public int SystemErrors;
        public int CriticalErrors;
        public int Errors;
        public int Warnings;
        public int InfoMessages;
        public int ErrorsLastMinute;
        
        public override string ToString()
        {
            return $"Total: {TotalErrors} | Critical: {CriticalErrors} | Errors: {Errors} | Warnings: {Warnings} | LastMinute: {ErrorsLastMinute}";
        }
    }
    
    /// <summary>
    /// Расширения для валидации данных
    /// </summary>
    public static class ValidationExtensions
    {
        /// <summary>
        /// Валидирует float3 вектор
        /// </summary>
        [BurstCompile]
        public static bool IsValid(this float3 vector)
        {
            return !math.any(math.isnan(vector)) && !math.any(math.isinf(vector));
        }
        
        /// <summary>
        /// Валидирует float значение
        /// </summary>
        [BurstCompile]
        public static bool IsValid(this float value)
        {
            return !math.isnan(value) && !math.isinf(value);
        }
        
        /// <summary>
        /// Валидирует quaternion
        /// </summary>
        [BurstCompile]
        public static bool IsValid(this quaternion rotation)
        {
            float4 components = rotation.value;
            return !math.any(math.isnan(components)) && !math.any(math.isinf(components));
        }
        
        /// <summary>
        /// Проверяет, находится ли значение в разумном диапазоне
        /// </summary>
        [BurstCompile]
        public static bool IsInReasonableRange(this float value, float min, float max)
        {
            return value >= min && value <= max;
        }
    }
}
