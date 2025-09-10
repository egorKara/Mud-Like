using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using MudLike.Core.Components;
using MudLike.Vehicles.Components;

namespace MudLike.Core.Systems
{
    /// <summary>
    /// Система движения игрока (грузовик КРАЗ) в ECS архитектуре
    /// </summary>
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial class PlayerMovementSystem : SystemBase
    {
        /// <summary>
        /// Обрабатывает движение всех игроков (грузовиков)
        /// </summary>
        protected override void OnUpdate()
        {
            float deltaTime = SystemAPI.Time.fixedDeltaTime;
            
            // Обрабатываем грузовики с полной физикой
            Entities
                .WithAll<PlayerTag, TruckData>()
                .ForEach((ref LocalTransform transform, in TruckInput input) =>
                {
                    ProcessTruckMovement(ref transform, input, deltaTime);
                }).Schedule();
            
            // Обрабатываем простых игроков (для совместимости)
            Entities
                .WithAll<PlayerTag>()
                .WithNone<TruckData>()
                .ForEach((ref LocalTransform transform, in PlayerInput input) =>
                {
                    ProcessSimpleMovement(ref transform, input, deltaTime);
                }).Schedule();
        }
        
        /// <summary>
        /// Обрабатывает движение грузовика
        /// </summary>
        /// <param name="transform">Трансформация грузовика</param>
        /// <param name="input">Ввод грузовика</param>
        /// <param name="deltaTime">Время с последнего обновления</param>
        private static void ProcessTruckMovement(ref LocalTransform transform, in TruckInput input, float deltaTime)
        {
            // Упрощенное движение грузовика (основная логика в TruckMovementSystem)
            float3 movement = CalculateTruckMovement(input);
            transform.Position += movement * deltaTime;
            
            // Поворот грузовика
            if (math.abs(input.Steering) > 0.1f)
            {
                float rotationSpeed = 30f; // градусов в секунду
                float rotation = input.Steering * rotationSpeed * deltaTime;
                transform.Rotation = math.mul(transform.Rotation, quaternion.RotateY(math.radians(rotation)));
            }
        }
        
        /// <summary>
        /// Обрабатывает простое движение игрока (для совместимости)
        /// </summary>
        /// <param name="transform">Трансформация игрока</param>
        /// <param name="input">Ввод игрока</param>
        /// <param name="deltaTime">Время с последнего обновления</param>
        private static void ProcessSimpleMovement(ref LocalTransform transform, in PlayerInput input, float deltaTime)
        {
            float3 movement = CalculateMovement(input);
            transform.Position += movement * deltaTime;
        }
        
        /// <summary>
        /// Вычисляет направление движения грузовика
        /// </summary>
        /// <param name="input">Ввод грузовика</param>
        /// <returns>Направление движения</returns>
        private static float3 CalculateTruckMovement(in TruckInput input)
        {
            float3 direction = new float3(0, 0, input.Throttle);
            float speed = input.Throttle * 10f; // Скорость грузовика
            return direction * speed;
        }
        
        /// <summary>
        /// Вычисляет направление движения на основе ввода (для совместимости)
        /// </summary>
        /// <param name="input">Ввод игрока</param>
        /// <returns>Направление движения</returns>
        private static float3 CalculateMovement(in PlayerInput input)
        {
            float3 direction = new float3(input.Movement.x, 0, input.Movement.y);
            return math.normalize(direction) * 5f; // Скорость движения
        }
    }
}
