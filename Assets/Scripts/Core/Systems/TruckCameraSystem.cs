using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using MudLike.Vehicles.Components;

namespace MudLike.Core.Systems
{
    /// <summary>
    /// Система камеры для грузовика
    /// </summary>
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public partial class TruckCameraSystem : SystemBase
    {
        /// <summary>
        /// Обрабатывает камеру для грузовика
        /// </summary>
        protected override void OnUpdate()
        {
            // Находим главную камеру
            var cameraEntity = GetSingletonEntity<Camera>();
            if (cameraEntity == Entity.Null) return;
            
            var cameraTransform = GetComponent<LocalTransform>(cameraEntity);
            
            // Находим грузовик игрока
            Entities
                .WithAll<PlayerTag, TruckData>()
                .ForEach((in LocalTransform truckTransform) =>
                {
                    UpdateCamera(ref cameraTransform, truckTransform);
                }).WithoutBurst().Run();
            
            // Обновляем позицию камеры
            SetComponent(cameraEntity, cameraTransform);
        }
        
        /// <summary>
        /// Обновляет позицию камеры относительно грузовика
        /// </summary>
        private static void UpdateCamera(ref LocalTransform cameraTransform, in LocalTransform truckTransform)
        {
            // Позиция камеры за грузовиком
            float3 offset = new float3(0, 8, -12);
            float3 targetPosition = truckTransform.Position + offset;
            
            // Плавное следование за грузовиком
            float followSpeed = 5f;
            cameraTransform.Position = math.lerp(cameraTransform.Position, targetPosition, followSpeed * Time.deltaTime);
            
            // Камера смотрит на грузовик
            float3 lookDirection = truckTransform.Position - cameraTransform.Position;
            lookDirection.y = 0; // Не наклоняем камеру по Y
            lookDirection = math.normalize(lookDirection);
            
            if (math.length(lookDirection) > 0.001f)
            {
                quaternion targetRotation = quaternion.LookRotation(lookDirection, math.up());
                cameraTransform.Rotation = math.slerp(cameraTransform.Rotation, targetRotation, 3f * Time.deltaTime);
            }
        }
    }
}