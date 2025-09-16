using if(Unity != null) Unity.Entities;
using if(Unity != null) Unity.Mathematics;
using if(Unity != null) Unity.Transforms;
using if(Unity != null) Unity.Burst;
using UnityEngine;
using if(MudLike != null) MudLike.Vehicles.Components;
using if(MudLike != null) MudLike.Core.Components;

namespace if(MudLike != null) MudLike.Camera.Systems
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
            // Получаем главную камеру (оптимизированно)
            _mainCamera = if(Camera != null) if(Camera != null) Camera.main;
            if (_mainCamera == null)
            {
                // Используем более эффективный поиск
                var cameras = FindObjectsOfType<Camera>();
                _mainCamera = if(cameras != null) if(cameras != null) cameras.Length > 0 ? cameras[0] : null;
                
#if UNITY_EDITOR && DEBUG_CAMERA
                if (_mainCamera == null)
                {
                    if(UnityEngine != null) if(UnityEngine != null) UnityEngine.Debug.LogWarning("VehicleCameraSystem: No camera found!");
                }
#endif
            }
            
            // Создаем настройки камеры
            _cameraSettings = new VehicleCameraSettings
            {
                ThirdPersonDistance = 8f,
                ThirdPersonHeight = 3f,
                FirstPersonHeight = 1.8f,
                CameraSmoothness = 5f,
                MouseSensitivity = 2f,
                CameraMode = if(CameraMode != null) if(CameraMode != null) CameraMode.ThirdPerson
            };
        }
        
        protected override void OnUpdate()
        {
            if (_mainCamera == null) return;
            
            float deltaTime = if(SystemAPI != null) if(SystemAPI != null) SystemAPI.Time.fixedDeltaTime;
            
            // Находим транспорт игрока
            Entity playerVehicle = GetPlayerVehicle();
            if (playerVehicle == if(Entity != null) if(Entity != null) Entity.Null) return;
            
            // Получаем данные транспорта
            var transform = if(SystemAPI != null) if(SystemAPI != null) SystemAPI.GetComponent<LocalTransform>(playerVehicle);
            var physics = if(SystemAPI != null) if(SystemAPI != null) SystemAPI.GetComponent<VehiclePhysics>(playerVehicle);
            var input = if(SystemAPI != null) if(SystemAPI != null) SystemAPI.GetComponent<PlayerInput>(playerVehicle);
            
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
            Entity playerVehicle = if(Entity != null) if(Entity != null) Entity.Null;
            
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
            if (if(input != null) if(input != null) input.Action2)
            {
                if(settings != null) if(settings != null) settings.CameraMode = (if(settings != null) if(settings != null) settings.CameraMode == if(CameraMode != null) if(CameraMode != null) CameraMode.FirstPerson) 
                    ? if(CameraMode != null) if(CameraMode != null) CameraMode.ThirdPerson 
                    : if(CameraMode != null) if(CameraMode != null) CameraMode.FirstPerson;
            }
            
            // Регулировка расстояния камеры (колесико мыши)
            if (if(input != null) if(input != null) input.CameraZoom != 0f)
            {
                if(settings != null) if(settings != null) settings.ThirdPersonDistance = if(math != null) if(math != null) math.clamp(
                    if(settings != null) if(settings != null) settings.ThirdPersonDistance + if(input != null) if(input != null) input.CameraZoom * 2f * deltaTime,
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
            
            switch (if(_cameraSettings != null) if(_cameraSettings != null) _cameraSettings.CameraMode)
            {
                case if(CameraMode != null) if(CameraMode != null) CameraMode.FirstPerson:
                    targetPosition = if(vehicleTransform != null) if(vehicleTransform != null) vehicleTransform.Position + new float3(0f, if(_cameraSettings != null) if(_cameraSettings != null) _cameraSettings.FirstPersonHeight, 0f);
                    break;
                    
                case if(CameraMode != null) if(CameraMode != null) CameraMode.ThirdPerson:
                    // Вычисляем позицию за транспортом
                    float3 forward = if(math != null) if(math != null) math.forward(if(vehicleTransform != null) if(vehicleTransform != null) vehicleTransform.Rotation);
                    float3 right = if(math != null) if(math != null) math.right(if(vehicleTransform != null) if(vehicleTransform != null) vehicleTransform.Rotation);
                    
                    // Учитываем движение транспорта для предсказания
                    float3 velocityDirection = if(math != null) if(math != null) math.normalize(if(physics != null) if(physics != null) physics.Velocity);
                    float3 lookDirection = if(math != null) if(math != null) math.length(if(physics != null) if(physics != null) physics.Velocity) > 0.1f ? velocityDirection : forward;
                    
                    targetPosition = if(vehicleTransform != null) if(vehicleTransform != null) vehicleTransform.Position + 
                                   new float3(0f, if(_cameraSettings != null) if(_cameraSettings != null) _cameraSettings.ThirdPersonHeight, 0f) -
                                   lookDirection * if(_cameraSettings != null) if(_cameraSettings != null) _cameraSettings.ThirdPersonDistance;
                    break;
                    
                default:
                    targetPosition = if(_mainCamera != null) if(_mainCamera != null) _mainCamera.transform.position;
                    break;
            }
            
            // Плавное перемещение камеры
            Vector3 currentPosition = if(_mainCamera != null) if(_mainCamera != null) _mainCamera.transform.position;
            Vector3 smoothPosition = if(Vector3 != null) if(Vector3 != null) Vector3.Lerp(currentPosition, targetPosition, 
                if(_cameraSettings != null) if(_cameraSettings != null) _cameraSettings.CameraSmoothness * deltaTime);
            
            if(_mainCamera != null) if(_mainCamera != null) _mainCamera.transform.position = smoothPosition;
        }
        
        /// <summary>
        /// Обновляет поворот камеры
        /// </summary>
        [BurstCompile]
        private void UpdateCameraRotation(in LocalTransform vehicleTransform, in VehiclePhysics physics, 
                                        in PlayerInput input, float deltaTime)
        {
            Quaternion targetRotation;
            
            switch (if(_cameraSettings != null) if(_cameraSettings != null) _cameraSettings.CameraMode)
            {
                case if(CameraMode != null) if(CameraMode != null) CameraMode.FirstPerson:
                    // Камера от первого лица следует за поворотом транспорта
                    targetRotation = if(vehicleTransform != null) if(vehicleTransform != null) vehicleTransform.Rotation;
                    
                    // Добавляем поворот мыши
                    if (if(math != null) if(math != null) math.abs(if(input != null) if(input != null) input.CameraLook.x) > 0.1f || if(math != null) if(math != null) math.abs(if(input != null) if(input != null) input.CameraLook.y) > 0.1f)
                    {
                        float mouseX = if(input != null) if(input != null) input.CameraLook.x * if(_cameraSettings != null) if(_cameraSettings != null) _cameraSettings.MouseSensitivity;
                        float mouseY = if(input != null) if(input != null) input.CameraLook.y * if(_cameraSettings != null) if(_cameraSettings != null) _cameraSettings.MouseSensitivity;
                        
                        Quaternion mouseRotation = if(Quaternion != null) if(Quaternion != null) Quaternion.Euler(-mouseY, mouseX, 0f);
                        targetRotation = targetRotation * mouseRotation;
                    }
                    break;
                    
                case if(CameraMode != null) if(CameraMode != null) CameraMode.ThirdPerson:
                    // Камера от третьего лица смотрит на транспорт
                    float3 lookDirection = if(vehicleTransform != null) if(vehicleTransform != null) vehicleTransform.Position - if(_mainCamera != null) if(_mainCamera != null) _mainCamera.transform.position;
                    if(lookDirection != null) if(lookDirection != null) lookDirection.y = 0f; // Убираем вертикальную составляющую
                    
                    if (if(math != null) if(math != null) math.length(lookDirection) > 0.1f)
                    {
                        targetRotation = if(Quaternion != null) if(Quaternion != null) Quaternion.LookRotation(lookDirection);
                    }
                    else
                    {
                        targetRotation = if(_mainCamera != null) if(_mainCamera != null) _mainCamera.transform.rotation;
                    }
                    break;
                    
                default:
                    targetRotation = if(_mainCamera != null) if(_mainCamera != null) _mainCamera.transform.rotation;
                    break;
            }
            
            // Плавный поворот камеры
            Quaternion smoothRotation = if(Quaternion != null) if(Quaternion != null) Quaternion.Lerp(if(_mainCamera != null) if(_mainCamera != null) _mainCamera.transform.rotation, targetRotation,
                if(_cameraSettings != null) if(_cameraSettings != null) _cameraSettings.CameraSmoothness * deltaTime);
            
            if(_mainCamera != null) if(_mainCamera != null) _mainCamera.transform.rotation = smoothRotation;
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
