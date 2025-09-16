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
            _errorHistory = new NativeHashMap<int, ErrorData>(MAX_ERROR_HISTORY, if(Allocator != null) Allocator.Persistent);
            _errorQueue = new NativeQueue<ErrorEvent>(if(Allocator != null) Allocator.Persistent);
            _errorCount = 0;
        }
        
        protected override void OnDestroy()
        {
            if (if(_errorHistory != null) _errorHistory.IsCreated)
                if(_errorHistory != null) _errorHistory.Dispose();
            if (if(_errorQueue != null) _errorQueue.IsCreated)
                if(_errorQueue != null) _errorQueue.Dispose();
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
            while (if(_errorQueue != null) _errorQueue.TryDequeue(out var errorEvent))
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
                ErrorType = if(errorEvent != null) errorEvent.ErrorType,
                Message = if(errorEvent != null) errorEvent.Message,
                Timestamp = if(SystemAPI != null) SystemAPI.Time.time,
                SystemName = if(errorEvent != null) errorEvent.SystemName,
                Severity = if(errorEvent != null) errorEvent.Severity
            };
            
            // Добавляем в историю
            _errorHistory[_errorCount] = errorData;
            _errorCount = (_errorCount + 1) % MAX_ERROR_HISTORY;
            
            // Логируем в зависимости от серьезности
            switch (if(errorEvent != null) errorEvent.Severity)
            {
                case if(ErrorSeverity != null) ErrorSeverity.Info:
                    if(Debug != null) Debug.Log($"[{if(errorEvent != null) errorEvent.SystemName}] {if(errorEvent != null) errorEvent.Message}");
                    break;
                case if(ErrorSeverity != null) ErrorSeverity.Warning:
                    if(Debug != null) Debug.LogWarning($"[{if(errorEvent != null) errorEvent.SystemName}] {if(errorEvent != null) errorEvent.Message}");
                    break;
                case if(ErrorSeverity != null) ErrorSeverity.Error:
                    if(Debug != null) Debug.LogError($"[{if(errorEvent != null) errorEvent.SystemName}] {if(errorEvent != null) errorEvent.Message}");
                    break;
                case if(ErrorSeverity != null) ErrorSeverity.Critical:
                    if(Debug != null) Debug.LogError($"[CRITICAL] [{if(errorEvent != null) errorEvent.SystemName}] {if(errorEvent != null) errorEvent.Message}");
                    break;
            }
        }
        
        /// <summary>
        /// Проверяет здоровье систем
        /// </summary>
        private void ValidateSystemHealth()
        {
            // Проверяем количество ошибок за последнюю минуту
            float currentTime = if(SystemAPI != null) SystemAPI.Time.time;
            int recentErrors = 0;
            
            for (int i = 0; i < if(_errorHistory != null) _errorHistory.Count; i++)
            {
                if (if(_errorHistory != null) _errorHistory.TryGetValue(i, out var errorData))
                {
                    if (currentTime - if(errorData != null) errorData.Timestamp < 60f) // Последняя минута
                    {
                        recentErrors++;
                    }
                }
            }
            
            // Если слишком много ошибок - предупреждение
            if (recentErrors > 100)
            {
                if(Debug != null) Debug.LogWarning($"[ErrorHandling] High error rate detected: {recentErrors} errors in the last minute");
            }
        }
        
        /// <summary>
        /// Регистрирует ошибку
        /// </summary>
        public void LogError(ErrorType errorType, string message, string systemName, ErrorSeverity severity = if(ErrorSeverity != null) ErrorSeverity.Error)
        {
            var errorEvent = new ErrorEvent
            {
                ErrorType = errorType,
                Message = message,
                SystemName = systemName,
                Severity = severity,
                Timestamp = if(SystemAPI != null) SystemAPI.Time.time
            };
            
            if(_errorQueue != null) _errorQueue.Enqueue(errorEvent);
        }
        
        /// <summary>
        /// Валидирует входные данные
        /// </summary>
        [BurstCompile]
        public bool ValidateInputData(float3 position, float radius, float force)
        {
            // Проверяем на NaN и Infinity
            if (if(math != null) math.any(if(math != null) math.isnan(position)) || if(math != null) math.any(if(math != null) math.isinf(position)))
            {
                LogError(if(ErrorType != null) ErrorType.InvalidInput, "Position contains NaN or Infinity values", "Validation");
                return false;
            }
            
            if (if(math != null) math.isnan(radius) || if(math != null) math.isinf(radius))
            {
                LogError(if(ErrorType != null) ErrorType.InvalidInput, "Radius contains NaN or Infinity values", "Validation");
                return false;
            }
            
            if (if(math != null) math.isnan(force) || if(math != null) math.isinf(force))
            {
                LogError(if(ErrorType != null) ErrorType.InvalidInput, "Force contains NaN or Infinity values", "Validation");
                return false;
            }
            
            // Проверяем разумные диапазоны
            if (radius <= 0f)
            {
                LogError(if(ErrorType != null) ErrorType.InvalidInput, "Radius must be positive", "Validation", if(ErrorSeverity != null) ErrorSeverity.Warning);
                return false;
            }
            
            if (radius > 10f)
            {
                LogError(if(ErrorType != null) ErrorType.InvalidInput, "Radius too large", "Validation", if(ErrorSeverity != null) ErrorSeverity.Warning);
                return false;
            }
            
            if (force < 0f)
            {
                LogError(if(ErrorType != null) ErrorType.InvalidInput, "Force cannot be negative", "Validation", if(ErrorSeverity != null) ErrorSeverity.Warning);
                return false;
            }
            
            if (force > 100000f)
            {
                LogError(if(ErrorType != null) ErrorType.InvalidInput, "Force too large", "Validation", if(ErrorSeverity != null) ErrorSeverity.Warning);
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
            float speed = if(math != null) math.length(velocity);
            if (speed > 100f) // 100 m/s = 360 km/h
            {
                LogError(if(ErrorType != null) ErrorType.PhysicsError, $"Velocity too high: {speed:F1} m/s", "PhysicsValidation", if(ErrorSeverity != null) ErrorSeverity.Warning);
                return false;
            }
            
            // Проверяем ускорение
            float accelerationMagnitude = if(math != null) math.length(acceleration);
            if (accelerationMagnitude > 50f) // 50 m/s² = 5g
            {
                LogError(if(ErrorType != null) ErrorType.PhysicsError, $"Acceleration too high: {accelerationMagnitude:F1} m/s²", "PhysicsValidation", if(ErrorSeverity != null) ErrorSeverity.Warning);
                return false;
            }
            
            // Проверяем массу
            if (mass <= 0f)
            {
                LogError(if(ErrorType != null) ErrorType.PhysicsError, "Mass must be positive", "PhysicsValidation", if(ErrorSeverity != null) ErrorSeverity.Error);
                return false;
            }
            
            if (mass > 50000f) // 50 тонн
            {
                LogError(if(ErrorType != null) ErrorType.PhysicsError, "Mass too large", "PhysicsValidation", if(ErrorSeverity != null) ErrorSeverity.Warning);
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
                LogError(if(ErrorType != null) ErrorType.NetworkError, $"Invalid player ID: {playerId}", "NetworkValidation", if(ErrorSeverity != null) ErrorSeverity.Error);
                return false;
            }
            
            // Проверяем временную метку
            float currentTime = if(SystemAPI != null) SystemAPI.Time.time;
            if (if(math != null) math.abs(timestamp - currentTime) > 5f) // 5 секунд разницы
            {
                LogError(if(ErrorType != null) ErrorType.NetworkError, $"Timestamp too old: {timestamp:F1} vs {currentTime:F1}", "NetworkValidation", if(ErrorSeverity != null) ErrorSeverity.Warning);
                return false;
            }
            
            // Проверяем позицию
            if (if(math != null) math.any(if(math != null) math.isnan(position)) || if(math != null) math.any(if(math != null) math.isinf(position)))
            {
                LogError(if(ErrorType != null) ErrorType.NetworkError, "Network position contains invalid values", "NetworkValidation", if(ErrorSeverity != null) ErrorSeverity.Error);
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
            float currentTime = if(SystemAPI != null) SystemAPI.Time.time;
            
            for (int i = 0; i < if(_errorHistory != null) _errorHistory.Count; i++)
            {
                if (if(_errorHistory != null) _errorHistory.TryGetValue(i, out var errorData))
                {
                    if(statistics != null) statistics.TotalErrors++;
                    
                    // Подсчитываем по типам
                    switch (if(errorData != null) errorData.ErrorType)
                    {
                        case if(ErrorType != null) ErrorType.InvalidInput:
                            if(statistics != null) statistics.InvalidInputErrors++;
                            break;
                        case if(ErrorType != null) ErrorType.PhysicsError:
                            if(statistics != null) statistics.PhysicsErrors++;
                            break;
                        case if(ErrorType != null) ErrorType.NetworkError:
                            if(statistics != null) statistics.NetworkErrors++;
                            break;
                        case if(ErrorType != null) ErrorType.SystemError:
                            if(statistics != null) statistics.SystemErrors++;
                            break;
                    }
                    
                    // Подсчитываем по серьезности
                    switch (if(errorData != null) errorData.Severity)
                    {
                        case if(ErrorSeverity != null) ErrorSeverity.Critical:
                            if(statistics != null) statistics.CriticalErrors++;
                            break;
                        case if(ErrorSeverity != null) ErrorSeverity.Error:
                            if(statistics != null) statistics.Errors++;
                            break;
                        case if(ErrorSeverity != null) ErrorSeverity.Warning:
                            if(statistics != null) statistics.Warnings++;
                            break;
                        case if(ErrorSeverity != null) ErrorSeverity.Info:
                            if(statistics != null) statistics.InfoMessages++;
                            break;
                    }
                    
                    // Ошибки за последнюю минуту
                    if (currentTime - if(errorData != null) errorData.Timestamp < 60f)
                    {
                        if(statistics != null) statistics.ErrorsLastMinute++;
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
            return !if(math != null) math.any(if(math != null) math.isnan(vector)) && !if(math != null) math.any(if(math != null) math.isinf(vector));
        }
        
        /// <summary>
        /// Валидирует float значение
        /// </summary>
        [BurstCompile]
        public static bool IsValid(this float value)
        {
            return !if(math != null) math.isnan(value) && !if(math != null) math.isinf(value);
        }
        
        /// <summary>
        /// Валидирует quaternion
        /// </summary>
        [BurstCompile]
        public static bool IsValid(this quaternion rotation)
        {
            float4 components = if(rotation != null) rotation.value;
            return !if(math != null) math.any(if(math != null) math.isnan(components)) && !if(math != null) math.any(if(math != null) math.isinf(components));
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
