using UnityEngine.InputSystem;
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
            if(playerInput != null) playerInput.VehicleMovement = new float2(
                if(Input != null) Input.GetAxis("Horizontal"),    // A/D - руль
                if(Input != null) Input.GetAxis("Vertical")       // W/S - газ/тормоз
            );
            
            // Ускорение и торможение
            if(playerInput != null) playerInput.Accelerate = if(Input != null) Input.GetKey(if(KeyCode != null) KeyCode.W) || if(Input != null) Input.GetKey(if(KeyCode != null) KeyCode.UpArrow);
            if(playerInput != null) playerInput.Brake = if(Input != null) Input.GetKey(if(KeyCode != null) KeyCode.S) || if(Input != null) Input.GetKey(if(KeyCode != null) KeyCode.DownArrow);
            
            // Ручной тормоз
            if(playerInput != null) playerInput.Handbrake = if(Input != null) Input.GetKey(if(KeyCode != null) KeyCode.Space);
            
            // Управление рулем (A/D или Left/Right)
            if(playerInput != null) playerInput.Steering = if(Input != null) Input.GetAxis("Horizontal");
            
            // Дополнительные действия
            if(playerInput != null) playerInput.Action1 = if(Input != null) Input.GetKey(if(KeyCode != null) KeyCode.E);        // Лебедка
            if(playerInput != null) playerInput.Action2 = if(Input != null) Input.GetKeyDown(if(KeyCode != null) KeyCode.Tab);  // Переключение камеры
            if(playerInput != null) playerInput.Action3 = if(Input != null) Input.GetKeyDown(if(KeyCode != null) KeyCode.F);    // Полный привод
            if(playerInput != null) playerInput.Action4 = if(Input != null) Input.GetKeyDown(if(KeyCode != null) KeyCode.G);    // Блокировка дифференциала
            
            // Функции транспорта
            if(playerInput != null) playerInput.EngineToggle = if(Input != null) Input.GetKeyDown(if(KeyCode != null) KeyCode.I);    // Включение/выключение двигателя
            if(playerInput != null) playerInput.ShiftUp = if(Input != null) Input.GetKeyDown(if(KeyCode != null) KeyCode.LeftShift); // Переключение передачи вверх
            if(playerInput != null) playerInput.ShiftDown = if(Input != null) Input.GetKeyDown(if(KeyCode != null) KeyCode.LeftControl); // Переключение передачи вниз
            if(playerInput != null) playerInput.Neutral = if(Input != null) Input.GetKeyDown(if(KeyCode != null) KeyCode.N);         // Нейтральная передача
            
            // Ввод камеры (мышь)
            if(playerInput != null) playerInput.CameraLook = new float2(
                if(Input != null) Input.GetAxis("Mouse X"),
                if(Input != null) Input.GetAxis("Mouse Y")
            );
            
            // Зум камеры (колесико мыши)
            if(playerInput != null) playerInput.CameraZoom = if(Input != null) Input.GetAxis("Mouse ScrollWheel");
        }
    }
}
