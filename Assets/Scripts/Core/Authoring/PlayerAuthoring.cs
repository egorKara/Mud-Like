using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using MudLike.Core.Components;

namespace MudLike.Core.Authoring
{
    /// <summary>
    /// Авторинг компонент для создания игрока в ECS
    /// Временно упрощен без Baker для тестирования
    /// </summary>
    public class PlayerAuthoring : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float maxSpeed = 10f;
        [SerializeField] private float acceleration = 5f;
        [SerializeField] private float deceleration = 8f;
        
        [Header("Rotation Settings")]
        [SerializeField] private float rotationSpeed = 180f;
        
        /// <summary>
        /// Получить настройки движения
        /// </summary>
        public MovementSpeed GetMovementSpeed()
        {
            return new MovementSpeed
            {
                Value = maxSpeed,
                MaxSpeed = maxSpeed,
                Acceleration = acceleration,
                Deceleration = deceleration
            };
        }
        
        /// <summary>
        /// Получить настройки поворота
        /// </summary>
        public RotationSpeed GetRotationSpeed()
        {
            return new RotationSpeed
            {
                Value = rotationSpeed
            };
        }
    }
}