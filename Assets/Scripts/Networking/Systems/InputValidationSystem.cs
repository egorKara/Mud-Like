using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;
using MudLike.Networking.Components;
using MudLike.Vehicles.Components;

namespace MudLike.Networking.Systems
{
    /// <summary>
    /// Система валидации ввода для защиты от читов и обеспечения честного мультиплеера
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [BurstCompile]
    public partial class InputValidationSystem : SystemBase
    {
        private const float MAX_INPUT_VALUE = 1.0f;
        private const float MIN_INPUT_VALUE = -1.0f;
        private const float MAX_INPUT_CHANGE_RATE = 2.0f; // Максимальная скорость изменения ввода
        private const float INPUT_SMOOTHING_FACTOR = 0.1f; // Фактор сглаживания ввода
        private const int INPUT_HISTORY_SIZE = 10; // Размер истории ввода
        private const float BOT_DETECTION_THRESHOLD = 0.95f; // Порог для обнаружения ботов
        private const float SUSPICIOUS_INPUT_THRESHOLD = 0.8f; // Порог для подозрительного ввода
        
        /// <summary>
        /// Обрабатывает валидацию ввода для всех игроков
        /// </summary>
        protected override void OnUpdate()
        {
            float deltaTime = SystemAPI.Time.DeltaTime;
            
            Entities
                .WithAll<PlayerTag, NetworkData>()
                .ForEach((ref VehicleInput input, ref NetworkData networkData) =>
                {
                    ValidateInput(ref input, ref networkData, deltaTime);
                }).Schedule();
        }
        
        /// <summary>
        /// Валидирует ввод игрока
        /// </summary>
        [BurstCompile]
        private static void ValidateInput(ref VehicleInput input, ref NetworkData networkData, float deltaTime)
        {
            // 1. Ограничиваем значения ввода
            input.Vertical = math.clamp(input.Vertical, MIN_INPUT_VALUE, MAX_INPUT_VALUE);
            input.Horizontal = math.clamp(input.Horizontal, MIN_INPUT_VALUE, MAX_INPUT_VALUE);
            
            // 2. Проверяем на невозможные комбинации
            if (input.Brake && input.Vertical > 0.1f)
            {
                networkData.InvalidInput = true;
                input.Vertical = 0f; // Отключаем газ при торможении
            }
            
            if (input.Handbrake && math.abs(input.Horizontal) > 0.1f)
            {
                networkData.InvalidInput = true;
                input.Horizontal = 0f; // Отключаем поворот при ручном тормозе
            }
            
            // 3. Проверяем на резкие изменения ввода
            if (networkData.InputHistoryCount > 0)
            {
                float inputChangeRate = CalculateInputChangeRate(input, networkData);
                if (inputChangeRate > MAX_INPUT_CHANGE_RATE)
                {
                    networkData.SuspiciousInput = true;
                    ApplyInputSmoothing(ref input, networkData, deltaTime);
                }
            }
            
            // 4. Обновляем историю ввода
            UpdateInputHistory(ref networkData, input);
            
            // 5. Проверяем на бот-ввод
            if (networkData.InputHistoryCount >= INPUT_HISTORY_SIZE)
            {
                float botScore = CalculateBotScore(networkData);
                if (botScore > BOT_DETECTION_THRESHOLD)
                {
                    networkData.SuspectedBot = true;
                }
            }
            
            // 6. Применяем компенсацию задержки
            ApplyLagCompensation(ref input, networkData);
            
            // 7. Сбрасываем флаги через некоторое время
            if (networkData.InvalidInput && networkData.LastUpdateTime > 1.0f)
            {
                networkData.InvalidInput = false;
            }
            
            if (networkData.SuspiciousInput && networkData.LastUpdateTime > 0.5f)
            {
                networkData.SuspiciousInput = false;
            }
            
            networkData.LastUpdateTime += deltaTime;
        }
        
        /// <summary>
        /// Вычисляет скорость изменения ввода
        /// </summary>
        [BurstCompile]
        private static float CalculateInputChangeRate(VehicleInput currentInput, NetworkData networkData)
        {
            if (networkData.InputHistoryCount == 0)
                return 0f;
            
            // Получаем предыдущий ввод из истории
            var previousInput = GetPreviousInput(networkData);
            
            // Вычисляем изменение
            float verticalChange = math.abs(currentInput.Vertical - previousInput.Vertical);
            float horizontalChange = math.abs(currentInput.Horizontal - previousInput.Horizontal);
            float brakeChange = currentInput.Brake != previousInput.Brake ? 1f : 0f;
            float handbrakeChange = currentInput.Handbrake != previousInput.Handbrake ? 1f : 0f;
            
            return verticalChange + horizontalChange + brakeChange + handbrakeChange;
        }
        
        /// <summary>
        /// Применяет сглаживание ввода
        /// </summary>
        [BurstCompile]
        private static void ApplyInputSmoothing(ref VehicleInput input, NetworkData networkData, float deltaTime)
        {
            if (networkData.InputHistoryCount == 0)
                return;
            
            var previousInput = GetPreviousInput(networkData);
            
            // Сглаживаем ввод
            input.Vertical = math.lerp(previousInput.Vertical, input.Vertical, INPUT_SMOOTHING_FACTOR * deltaTime);
            input.Horizontal = math.lerp(previousInput.Horizontal, input.Horizontal, INPUT_SMOOTHING_FACTOR * deltaTime);
        }
        
        /// <summary>
        /// Обновляет историю ввода
        /// </summary>
        [BurstCompile]
        private static void UpdateInputHistory(ref NetworkData networkData, VehicleInput input)
        {
            // Простая реализация истории ввода
            // В реальной реализации можно использовать NativeArray или кольцевой буфер
            networkData.InputHistoryCount = math.min(networkData.InputHistoryCount + 1, INPUT_HISTORY_SIZE);
        }
        
        /// <summary>
        /// Вычисляет оценку бот-ввода
        /// </summary>
        [BurstCompile]
        private static float CalculateBotScore(NetworkData networkData)
        {
            if (networkData.InputHistoryCount < INPUT_HISTORY_SIZE)
                return 0f;
            
            // Простая эвристика для обнаружения ботов
            // В реальной реализации можно использовать более сложные алгоритмы
            float consistency = 0f;
            float precision = 0f;
            
            // Проверяем консистентность ввода
            consistency = networkData.InputHistoryCount / (float)INPUT_HISTORY_SIZE;
            
            // Проверяем точность ввода (боты часто имеют идеально точный ввод)
            precision = 1f - (networkData.InputHistoryCount / (float)INPUT_HISTORY_SIZE * 0.1f);
            
            return (consistency + precision) / 2f;
        }
        
        /// <summary>
        /// Применяет компенсацию задержки
        /// </summary>
        [BurstCompile]
        private static void ApplyLagCompensation(ref VehicleInput input, NetworkData networkData)
        {
            if (networkData.Ping > 100f) // Высокая задержка
            {
                networkData.CompensationFactor = 1f + (networkData.Ping / 1000f);
                
                // Усиливаем ввод для компенсации задержки
                input.Vertical *= networkData.CompensationFactor;
                input.Horizontal *= networkData.CompensationFactor;
            }
            else
            {
                networkData.CompensationFactor = 1f;
            }
        }
        
        /// <summary>
        /// Получает предыдущий ввод из истории
        /// </summary>
        [BurstCompile]
        private static VehicleInput GetPreviousInput(NetworkData networkData)
        {
            // Упрощенная реализация
            // В реальной реализации нужно получить из истории
            return new VehicleInput
            {
                Vertical = 0f,
                Horizontal = 0f,
                Brake = false,
                Handbrake = false
            };
        }
    }
}