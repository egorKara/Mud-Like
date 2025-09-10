using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using MudLike.Vehicles.Components;

namespace MudLike.Vehicles.Systems
{
    /// <summary>
    /// Система управления грузовиком КРАЗ
    /// </summary>
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class TruckControlSystem : SystemBase
    {
        /// <summary>
        /// Обрабатывает управление для всех грузовиков
        /// </summary>
        protected override void OnUpdate()
        {
            // Получаем ввод с клавиатуры
            var input = new TruckControl
            {
                Throttle = Input.GetAxis("Vertical"), // W/S или стрелки
                Brake = Input.GetKey(KeyCode.Space) ? 1f : 0f, // Пробел
                Steering = Input.GetAxis("Horizontal"), // A/D или стрелки
                Handbrake = Input.GetKey(KeyCode.LeftShift), // Левый Shift
                ShiftUp = Input.GetKeyDown(KeyCode.E), // E
                ShiftDown = Input.GetKeyDown(KeyCode.Q), // Q
                ToggleEngine = Input.GetKeyDown(KeyCode.R), // R
                Clutch = Input.GetKey(KeyCode.LeftControl) ? 1f : 0f, // Левый Ctrl
                FourWheelDrive = Input.GetKey(KeyCode.F), // F
                LockDifferential = Input.GetKey(KeyCode.L) // L
            };
            
            // Нормализуем ввод газа (только положительные значения)
            input.Throttle = math.max(0f, input.Throttle);
            
            // Обновляем компоненты управления для всех грузовиков
            Entities
                .WithAll<TruckData>()
                .ForEach((ref TruckControl truckControl) =>
                {
                    truckControl = input;
                }).WithoutBurst().Run();
        }
    }
}