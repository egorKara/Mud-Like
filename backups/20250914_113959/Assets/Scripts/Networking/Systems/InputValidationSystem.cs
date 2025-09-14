using Unity.Entities;
using Unity.NetCode;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using MudLike.Networking.Components;
using MudLike.Core.Components;

namespace MudLike.Networking.Systems
{
    /// <summary>
    /// Система валидации ввода для мультиплеера
    /// Обеспечивает серверную валидацию всех действий игроков
    /// </summary>
    [UpdateInGroup(typeof(NetCodeServerSystemGroup))]
    [BurstCompile]
    public partial class InputValidationSystem : SystemBase
    {
        private NativeHashMap<int, PlayerValidationData> _playerValidationData;
        private NativeHashMap<int, InputHistory> _inputHistory;
        
        protected override void OnCreate()
        {
            _playerValidationData = new NativeHashMap<int, PlayerValidationData>(64, Allocator.Persistent);
            _inputHistory = new NativeHashMap<int, InputHistory>(64, Allocator.Persistent);
        }
        
        protected override void OnDestroy()
        {
            if (_playerValidationData.IsCreated)
                _playerValidationData.Dispose();
            if (_inputHistory.IsCreated)
                _inputHistory.Dispose();
        }
        
        /// <summary>
        /// Валидирует ввод игрока на сервере
        /// </summary>
        /// <param name="playerId">ID игрока</param>
        /// <param name="input">Ввод игрока</param>
        /// <param name="timestamp">Временная метка</param>
        /// <returns>Результат валидации</returns>
        [BurstCompile]
        public ValidationResult ValidatePlayerInput(int playerId, PlayerInput input, float timestamp)
        {
            var result = new ValidationResult { IsValid = true, Reason = ValidationReason.None };
            
            // 1. Проверяем частоту ввода (Rate Limiting)
            if (!ValidateInputRate(playerId, timestamp, ref result))
                return result;
            
            // 2. Проверяем корректность значений
            if (!ValidateInputValues(input, ref result))
                return result;
            
            // 3. Проверяем физическую возможность действия
            if (!ValidatePhysicalPossibility(playerId, input, ref result))
                return result;
            
            // 4. Проверяем на подозрительную активность
            if (!ValidateBehavioralPatterns(playerId, input, ref result))
                return result;
            
            // Обновляем историю ввода
            UpdateInputHistory(playerId, input, timestamp);
            
            return result;
        }
        
        /// <summary>
        /// Валидирует частоту ввода (Rate Limiting)
        /// </summary>
        [BurstCompile]
        private bool ValidateInputRate(int playerId, float timestamp, ref ValidationResult result)
        {
            if (!_playerValidationData.TryGetValue(playerId, out var validationData))
            {
                // Создаем новые данные для игрока
                validationData = new PlayerValidationData
                {
                    LastInputTime = timestamp,
                    InputCount = 1,
                    SuspiciousActivityCount = 0,
                    IsBanned = false
                };
                _playerValidationData[playerId] = validationData;
                return true;
            }
            
            // Проверяем минимальный интервал между вводами
            float timeSinceLastInput = timestamp - validationData.LastInputTime;
            float minInputInterval = 0.016f; // ~60 FPS
            
            if (timeSinceLastInput < minInputInterval)
            {
                result.IsValid = false;
                result.Reason = ValidationReason.TooFrequentInput;
                result.Details = $"Input too frequent: {timeSinceLastInput:F3}s";
                return false;
            }
            
            // Проверяем максимальную частоту ввода
            float inputWindow = 1.0f; // 1 секунда
            if (timeSinceLastInput < inputWindow)
            {
                validationData.InputCount++;
                
                int maxInputsPerSecond = 100; // Максимум 100 вводов в секунду
                if (validationData.InputCount > maxInputsPerSecond)
                {
                    result.IsValid = false;
                    result.Reason = ValidationReason.RateLimitExceeded;
                    result.Details = $"Rate limit exceeded: {validationData.InputCount} inputs per second";
                    
                    // Увеличиваем счетчик подозрительной активности
                    validationData.SuspiciousActivityCount++;
                    _playerValidationData[playerId] = validationData;
                    return false;
                }
            }
            else
            {
                // Сбрасываем счетчик если прошла секунда
                validationData.InputCount = 1;
            }
            
            validationData.LastInputTime = timestamp;
            _playerValidationData[playerId] = validationData;
            
            return true;
        }
        
        /// <summary>
        /// Валидирует корректность значений ввода управления транспортом
        /// </summary>
        [BurstCompile]
        private bool ValidateInputValues(PlayerInput input, ref ValidationResult result)
        {
            // Проверяем диапазон движения транспорта
            float maxMovement = 1.0f;
            if (math.length(input.VehicleMovement) > maxMovement)
            {
                result.IsValid = false;
                result.Reason = ValidationReason.InvalidInputValues;
                result.Details = $"Vehicle movement magnitude too high: {math.length(input.VehicleMovement):F3}";
                return false;
            }
            
            // Проверяем диапазон управления рулем
            if (math.abs(input.Steering) > 1.0f)
            {
                result.IsValid = false;
                result.Reason = ValidationReason.InvalidInputValues;
                result.Details = $"Steering value out of range: {input.Steering:F3}";
                return false;
            }
            
            // Проверяем на NaN и Infinity
            if (math.any(math.isnan(input.VehicleMovement)) || math.any(math.isinf(input.VehicleMovement)))
            {
                result.IsValid = false;
                result.Reason = ValidationReason.InvalidInputValues;
                result.Details = "Vehicle movement contains NaN or Infinity values";
                return false;
            }
            
            if (math.isnan(input.Steering) || math.isinf(input.Steering))
            {
                result.IsValid = false;
                result.Reason = ValidationReason.InvalidInputValues;
                result.Details = "Steering contains NaN or Infinity values";
                return false;
            }
            
            return true;
        }
        
        /// <summary>
        /// Валидирует физическую возможность действия
        /// </summary>
        [BurstCompile]
        private bool ValidatePhysicalPossibility(int playerId, PlayerInput input, ref ValidationResult result)
        {
            // Здесь должна быть проверка физической возможности действия
            // Например, игрок не может двигаться если он застрял
            
            // Получаем данные игрока
            if (!GetPlayerData(playerId, out var playerData))
            {
                result.IsValid = false;
                result.Reason = ValidationReason.PlayerNotFound;
                result.Details = $"Player {playerId} not found";
                return false;
            }
            
            // Проверяем, может ли игрок двигаться
            if (playerData.IsStuck && math.length(input.Movement) > 0.1f)
            {
                // Игрок застрял, но пытается двигаться - подозрительно
                result.IsValid = false;
                result.Reason = ValidationReason.PhysicallyImpossible;
                result.Details = "Player is stuck but trying to move";
                return false;
            }
            
            // Проверяем скорость движения
            float maxPossibleSpeed = playerData.MaxSpeed * 1.1f; // 10% допуск
            float inputSpeed = math.length(input.Movement) * playerData.MaxSpeed;
            
            if (inputSpeed > maxPossibleSpeed)
            {
                result.IsValid = false;
                result.Reason = ValidationReason.PhysicallyImpossible;
                result.Details = $"Speed too high: {inputSpeed:F2} > {maxPossibleSpeed:F2}";
                return false;
            }
            
            return true;
        }
        
        /// <summary>
        /// Валидирует поведенческие паттерны на подозрительную активность
        /// </summary>
        [BurstCompile]
        private bool ValidateBehavioralPatterns(int playerId, PlayerInput input, ref ValidationResult result)
        {
            if (!_inputHistory.TryGetValue(playerId, out var history))
            {
                // Создаем новую историю
                history = new InputHistory
                {
                    LastInputs = new NativeArray<PlayerInput>(10, Allocator.Persistent),
                    InputCount = 0,
                    LastUpdateTime = SystemAPI.Time.time
                };
                _inputHistory[playerId] = history;
            }
            
            // Проверяем на повторяющиеся паттерны (боты)
            if (DetectRepeatingPatterns(input, history))
            {
                result.IsValid = false;
                result.Reason = ValidationReason.SuspiciousBehavior;
                result.Details = "Detected repeating input patterns";
                
                // Увеличиваем счетчик подозрительной активности
                if (_playerValidationData.TryGetValue(playerId, out var validationData))
                {
                    validationData.SuspiciousActivityCount++;
                    _playerValidationData[playerId] = validationData;
                }
                
                return false;
            }
            
            // Проверяем на слишком точные движения (автоматизация)
            if (DetectAutomatedMovement(input, history))
            {
                result.IsValid = false;
                result.Reason = ValidationReason.SuspiciousBehavior;
                result.Details = "Detected automated movement patterns";
                return false;
            }
            
            return true;
        }
        
        /// <summary>
        /// Обновляет историю ввода
        /// </summary>
        [BurstCompile]
        private void UpdateInputHistory(int playerId, PlayerInput input, float timestamp)
        {
            if (_inputHistory.TryGetValue(playerId, out var history))
            {
                // Добавляем новый ввод в историю
                int index = history.InputCount % history.LastInputs.Length;
                history.LastInputs[index] = input;
                history.InputCount++;
                history.LastUpdateTime = timestamp;
                _inputHistory[playerId] = history;
            }
        }
        
        /// <summary>
        /// Обнаруживает повторяющиеся паттерны ввода
        /// </summary>
        [BurstCompile]
        private bool DetectRepeatingPatterns(PlayerInput currentInput, InputHistory history)
        {
            if (history.InputCount < 5) return false; // Недостаточно данных
            
            int patternLength = 3;
            int matches = 0;
            
            // Проверяем последние patternLength вводов на повторение
            for (int i = 0; i < patternLength; i++)
            {
                int currentIndex = (history.InputCount - 1 - i) % history.LastInputs.Length;
                int previousIndex = (history.InputCount - 1 - i - patternLength) % history.LastInputs.Length;
                
                if (InputsAreEqual(history.LastInputs[currentIndex], history.LastInputs[previousIndex]))
                {
                    matches++;
                }
            }
            
            // Если большинство вводов повторяются - подозрительно
            return matches >= patternLength * 0.8f;
        }
        
        /// <summary>
        /// Обнаруживает автоматизированные движения транспорта
        /// </summary>
        [BurstCompile]
        private bool DetectAutomatedMovement(PlayerInput currentInput, InputHistory history)
        {
            if (history.InputCount < 3) return false;
            
            // Проверяем на слишком точные углы управления рулем
            float steeringAngle = currentInput.Steering;
            
            // Проверяем на точные значения рулевого управления
            float tolerance = 0.01f;
            for (float i = -1.0f; i <= 1.0f; i += 0.1f)
            {
                if (math.abs(steeringAngle - i) < tolerance)
                {
                    return true; // Подозрительно точное значение руля
                }
            }
            
            // Проверяем на слишком точные паттерны движения
            float movementAngle = math.atan2(currentInput.VehicleMovement.y, currentInput.VehicleMovement.x);
            float degrees = math.degrees(movementAngle);
            
            // Проверяем на точные углы движения
            for (int i = 0; i <= 360; i += 15)
            {
                if (math.abs(degrees - i) < tolerance)
                {
                    return true; // Подозрительно точный угол движения
                }
            }
            
            return false;
        }
        
        /// <summary>
        /// Проверяет равенство двух вводов управления транспортом
        /// </summary>
        [BurstCompile]
        private bool InputsAreEqual(PlayerInput a, PlayerInput b)
        {
            float tolerance = 0.01f;
            return math.distance(a.VehicleMovement, b.VehicleMovement) < tolerance &&
                   math.abs(a.Steering - b.Steering) < tolerance &&
                   a.Accelerate == b.Accelerate && 
                   a.Brake == b.Brake &&
                   a.Handbrake == b.Handbrake;
        }
        
        /// <summary>
        /// Получает данные игрока
        /// </summary>
        [BurstCompile]
        private bool GetPlayerData(int playerId, out PlayerData playerData)
        {
            // Здесь должна быть логика получения данных игрока из ECS
            // Пока возвращаем заглушку
            playerData = new PlayerData
            {
                MaxSpeed = 10f,
                IsStuck = false,
                Position = float3.zero
            };
            return true;
        }
        
        protected override void OnUpdate()
        {
            // Система работает по требованию через API методы
        }
    }
    
    /// <summary>
    /// Результат валидации ввода
    /// </summary>
    public struct ValidationResult
    {
        public bool IsValid;
        public ValidationReason Reason;
        public FixedString128Bytes Details;
    }
    
    /// <summary>
    /// Причина невалидности ввода
    /// </summary>
    public enum ValidationReason : byte
    {
        None,
        TooFrequentInput,
        RateLimitExceeded,
        InvalidInputValues,
        PhysicallyImpossible,
        SuspiciousBehavior,
        PlayerNotFound
    }
    
    /// <summary>
    /// Данные валидации игрока
    /// </summary>
    public struct PlayerValidationData
    {
        public float LastInputTime;
        public int InputCount;
        public int SuspiciousActivityCount;
        public bool IsBanned;
    }
    
    /// <summary>
    /// История ввода игрока
    /// </summary>
    public struct InputHistory
    {
        public NativeArray<PlayerInput> LastInputs;
        public int InputCount;
        public float LastUpdateTime;
    }
    
    /// <summary>
    /// Данные игрока для валидации
    /// </summary>
    public struct PlayerData
    {
        public float MaxSpeed;
        public bool IsStuck;
        public float3 Position;
    }
}
