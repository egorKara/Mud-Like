using Unity.Entities;
using Unity.Mathematics;
using Unity.Burst;
using UnityEngine;
using MudLike.Vehicles.Components;
using MudLike.Core.Components;

namespace MudLike.Input.Systems
{
    /// <summary>
    /// Система обработки ввода для управления транспортом
    /// Преобразует ввод Unity в ECS компоненты
    /// </summary>
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [BurstCompile]
    public partial class VehicleInputSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            // Обрабатываем ввод для всех игроков
            Entities
                .WithAll<PlayerTag>()
                .ForEach((ref PlayerInput playerInput) =>
                {
                    ProcessVehicleInput(ref playerInput);
                }).WithoutBurst().Run();
        }
        
        /// <summary>
        /// Обрабатывает ввод управления транспортом
        /// </summary>
        private void ProcessVehicleInput(ref PlayerInput playerInput)
        {
            // Движение транспорта (WASD)
            playerInput.VehicleMovement = new float2(
                Input.GetAxis("Horizontal"),    // A/D - руль
                Input.GetAxis("Vertical")       // W/S - газ/тормоз
            );
            
            // Ускорение и торможение
            playerInput.Accelerate = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
            playerInput.Brake = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);
            
            // Ручной тормоз
            playerInput.Handbrake = Input.GetKey(KeyCode.Space);
            
            // Управление рулем (A/D или Left/Right)
            playerInput.Steering = Input.GetAxis("Horizontal");
            
            // Дополнительные действия
            playerInput.Action1 = Input.GetKey(KeyCode.E);        // Лебедка
            playerInput.Action2 = Input.GetKeyDown(KeyCode.Tab);  // Переключение камеры
            playerInput.Action3 = Input.GetKeyDown(KeyCode.F);    // Полный привод
            playerInput.Action4 = Input.GetKeyDown(KeyCode.G);    // Блокировка дифференциала
            
            // Функции транспорта
            playerInput.EngineToggle = Input.GetKeyDown(KeyCode.I);    // Включение/выключение двигателя
            playerInput.ShiftUp = Input.GetKeyDown(KeyCode.LeftShift); // Переключение передачи вверх
            playerInput.ShiftDown = Input.GetKeyDown(KeyCode.LeftControl); // Переключение передачи вниз
            playerInput.Neutral = Input.GetKeyDown(KeyCode.N);         // Нейтральная передача
            
            // Ввод камеры (мышь)
            playerInput.CameraLook = new float2(
                Input.GetAxis("Mouse X"),
                Input.GetAxis("Mouse Y")
            );
            
            // Зум камеры (колесико мыши)
            playerInput.CameraZoom = Input.GetAxis("Mouse ScrollWheel");
        }
    }
}
