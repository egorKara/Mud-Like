using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;
using UnityEngine;
using MudLike.Vehicles.Components;
using MudLike.Core.Components;

namespace MudLike.Camera.Systems
{
    /// <summary>
    /// Система камеры для транспорта
    /// Поддерживает переключение между видами камеры и следование за транспортом
    /// </summary>
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    [BurstCompile]
    public partial class VehicleCameraSystem : SystemBase
    {
        private Camera _mainCamera;
        private VehicleCameraSettings _cameraSettings;
        
        protected override void OnCreate()
        {
            // Получаем главную камеру
            _mainCamera = Camera.main;
            if (_mainCamera == null)
            {
                _mainCamera = Object.FindObjectOfType<Camera>();
            }
            
            // Создаем настройки камеры
            _cameraSettings = new VehicleCameraSettings
            {
                ThirdPersonDistance = 8f,
                ThirdPersonHeight = 3f,
                FirstPersonHeight = 1.8f,
                CameraSmoothness = 5f,
                MouseSensitivity = 2f,
                CameraMode = CameraMode.ThirdPerson
            };
        }
        
        protected override void OnUpdate()
        {
            if (_mainCamera == null) return;
            
            float deltaTime = SystemAPI.Time.fixedDeltaTime;
            
            // Находим транспорт игрока
            Entity playerVehicle = GetPlayerVehicle();
            if (playerVehicle == Entity.Null) return;
            
            // Получаем данные транспорта
            var transform = SystemAPI.GetComponent<LocalTransform>(playerVehicle);
            var physics = SystemAPI.GetComponent<VehiclePhysics>(playerVehicle);
            var input = SystemAPI.GetComponent<PlayerInput>(playerVehicle);
            
            // Обрабатываем управление камерой
            ProcessCameraInput(ref _cameraSettings, input, deltaTime);
            
            // Обновляем позицию камеры
            UpdateCameraPosition(transform, physics, deltaTime);
            
            // Обновляем поворот камеры
            UpdateCameraRotation(transform, physics, input, deltaTime);
        }
        
        /// <summary>
        /// Находит транспорт игрока
        /// </summary>
        private Entity GetPlayerVehicle()
        {
            Entity playerVehicle = Entity.Null;
            
            Entities
                .WithAll<VehicleTag, PlayerTag>()
                .ForEach((Entity entity) =>
                {
                    playerVehicle = entity;
                }).WithoutBurst().Run();
            
            return playerVehicle;
        }
        
        /// <summary>
        /// Обрабатывает ввод для управления камерой
        /// </summary>
        [BurstCompile]
        private void ProcessCameraInput(ref VehicleCameraSettings settings, in PlayerInput input, float deltaTime)
        {
            // Переключение режима камеры (Tab)
            if (input.Action2)
            {
                settings.CameraMode = (settings.CameraMode == CameraMode.FirstPerson) 
                    ? CameraMode.ThirdPerson 
                    : CameraMode.FirstPerson;
            }
            
            // Регулировка расстояния камеры (колесико мыши)
            if (input.CameraZoom != 0f)
            {
                settings.ThirdPersonDistance = math.clamp(
                    settings.ThirdPersonDistance + input.CameraZoom * 2f * deltaTime,
                    3f, 15f);
            }
        }
        
        /// <summary>
        /// Обновляет позицию камеры
        /// </summary>
        [BurstCompile]
        private void UpdateCameraPosition(in LocalTransform vehicleTransform, in VehiclePhysics physics, float deltaTime)
        {
            float3 targetPosition;
            
            switch (_cameraSettings.CameraMode)
            {
                case CameraMode.FirstPerson:
                    targetPosition = vehicleTransform.Position + new float3(0f, _cameraSettings.FirstPersonHeight, 0f);
                    break;
                    
                case CameraMode.ThirdPerson:
                    // Вычисляем позицию за транспортом
                    float3 forward = math.forward(vehicleTransform.Rotation);
                    float3 right = math.right(vehicleTransform.Rotation);
                    
                    // Учитываем движение транспорта для предсказания
                    float3 velocityDirection = math.normalize(physics.Velocity);
                    float3 lookDirection = math.length(physics.Velocity) > 0.1f ? velocityDirection : forward;
                    
                    targetPosition = vehicleTransform.Position + 
                                   new float3(0f, _cameraSettings.ThirdPersonHeight, 0f) -
                                   lookDirection * _cameraSettings.ThirdPersonDistance;
                    break;
                    
                default:
                    targetPosition = _mainCamera.transform.position;
                    break;
            }
            
            // Плавное перемещение камеры
            Vector3 currentPosition = _mainCamera.transform.position;
            Vector3 smoothPosition = Vector3.Lerp(currentPosition, targetPosition, 
                _cameraSettings.CameraSmoothness * deltaTime);
            
            _mainCamera.transform.position = smoothPosition;
        }
        
        /// <summary>
        /// Обновляет поворот камеры
        /// </summary>
        [BurstCompile]
        private void UpdateCameraRotation(in LocalTransform vehicleTransform, in VehiclePhysics physics, 
                                        in PlayerInput input, float deltaTime)
        {
            Quaternion targetRotation;
            
            switch (_cameraSettings.CameraMode)
            {
                case CameraMode.FirstPerson:
                    // Камера от первого лица следует за поворотом транспорта
                    targetRotation = vehicleTransform.Rotation;
                    
                    // Добавляем поворот мыши
                    if (math.abs(input.CameraLook.x) > 0.1f || math.abs(input.CameraLook.y) > 0.1f)
                    {
                        float mouseX = input.CameraLook.x * _cameraSettings.MouseSensitivity;
                        float mouseY = input.CameraLook.y * _cameraSettings.MouseSensitivity;
                        
                        Quaternion mouseRotation = Quaternion.Euler(-mouseY, mouseX, 0f);
                        targetRotation = targetRotation * mouseRotation;
                    }
                    break;
                    
                case CameraMode.ThirdPerson:
                    // Камера от третьего лица смотрит на транспорт
                    float3 lookDirection = vehicleTransform.Position - _mainCamera.transform.position;
                    lookDirection.y = 0f; // Убираем вертикальную составляющую
                    
                    if (math.length(lookDirection) > 0.1f)
                    {
                        targetRotation = Quaternion.LookRotation(lookDirection);
                    }
                    else
                    {
                        targetRotation = _mainCamera.transform.rotation;
                    }
                    break;
                    
                default:
                    targetRotation = _mainCamera.transform.rotation;
                    break;
            }
            
            // Плавный поворот камеры
            Quaternion smoothRotation = Quaternion.Lerp(_mainCamera.transform.rotation, targetRotation,
                _cameraSettings.CameraSmoothness * deltaTime);
            
            _mainCamera.transform.rotation = smoothRotation;
        }
    }
    
    /// <summary>
    /// Настройки камеры транспорта
    /// </summary>
    public struct VehicleCameraSettings
    {
        public float ThirdPersonDistance;    // Расстояние камеры от транспорта
        public float ThirdPersonHeight;      // Высота камеры
        public float FirstPersonHeight;      // Высота камеры от первого лица
        public float CameraSmoothness;       // Плавность движения камеры
        public float MouseSensitivity;       // Чувствительность мыши
        public CameraMode CameraMode;        // Режим камеры
    }
    
    /// <summary>
    /// Режимы камеры
    /// </summary>
    public enum CameraMode
    {
        FirstPerson,     // От первого лица
        ThirdPerson      // От третьего лица
    }
}
