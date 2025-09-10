using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using MudLike.Vehicles.Components;

namespace MudLike.Vehicles.Systems
{
    /// <summary>
    /// ECS система для подвески колес
    /// </summary>
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial class SuspensionSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            float deltaTime = SystemAPI.Time.fixedDeltaTime;
            
            // Обновление подвески
            Entities.ForEach((ref SuspensionComponent suspension, in LocalTransform transform) =>
            {
                // Обновление длины подвески
                UpdateSuspensionLengthStatic(ref suspension, transform, deltaTime);
                
                // Расчет сил пружины и демпфера
                CalculateSuspensionForcesStatic(ref suspension, deltaTime);
                
                // Обновление состояния подвески
                UpdateSuspensionStateStatic(ref suspension);
                
                // Расчет энергии подвески
                CalculateSuspensionEnergyStatic(ref suspension, deltaTime);
                
            }).Schedule();
        }
        
        /// <summary>
        /// Обновление длины подвески
        /// </summary>
        private static void UpdateSuspensionLengthStatic(ref SuspensionComponent suspension, LocalTransform transform, float deltaTime)
        {
            if (!suspension.isActive)
            {
                suspension.currentLength = suspension.restLength;
                return;
            }
            
            // Получение позиции точки крепления подвески
            float3 mountPosition = transform.Position + math.mul(transform.Rotation, suspension.mountPoint);
            
            // Направление подвески в мировых координатах
            float3 suspensionDirection = math.mul(transform.Rotation, suspension.suspensionDirection);
            
            // Raycast для определения длины подвески
            // Здесь должен быть реальный raycast с Unity Physics
            // Пока используем упрощенную логику
            float rayDistance = suspension.maxLength;
            float hitDistance = rayDistance; // Упрощение
            
            // Обновление текущей длины
            suspension.currentLength = math.clamp(hitDistance, suspension.minLength, suspension.maxLength);
            
            // Расчет скорости сжатия/растяжения
            float lengthChange = suspension.currentLength - suspension.restLength;
            suspension.compressionVelocity = lengthChange / deltaTime;
            
            // Расчет коэффициента сжатия
            float compressionRange = suspension.maxLength - suspension.minLength;
            if (compressionRange > 0)
            {
                suspension.compressionRatio = (suspension.currentLength - suspension.minLength) / compressionRange;
            }
            else
            {
                suspension.compressionRatio = 0f;
            }
        }
        
        /// <summary>
        /// Расчет сил пружины и демпфера
        /// </summary>
        private static void CalculateSuspensionForcesStatic(ref SuspensionComponent suspension, float deltaTime)
        {
            if (!suspension.isActive)
            {
                suspension.springForce = 0f;
                suspension.damperForce = 0f;
                return;
            }
            
            // Расчет силы пружины (закон Гука)
            float compression = suspension.restLength - suspension.currentLength;
            suspension.springForce = compression * suspension.springStiffness;
            
            // Расчет силы демпфера
            suspension.damperForce = -suspension.compressionVelocity * suspension.damping;
            
            // Ограничение силы демпфера
            float maxDamperForce = suspension.maxDamping * math.abs(suspension.compressionVelocity);
            suspension.damperForce = math.clamp(suspension.damperForce, -maxDamperForce, maxDamperForce);
        }
        
        /// <summary>
        /// Обновление состояния подвески
        /// </summary>
        private static void UpdateSuspensionStateStatic(ref SuspensionComponent suspension)
        {
            // Проверка лимитов сжатия и растяжения
            suspension.isCompressed = suspension.currentLength <= suspension.minLength + 0.001f;
            suspension.isExtended = suspension.currentLength >= suspension.maxLength - 0.001f;
            
            // Расчет прогресса сжатия
            float compressionRange = suspension.maxLength - suspension.minLength;
            if (compressionRange > 0)
            {
                suspension.compressionProgress = (suspension.currentLength - suspension.minLength) / compressionRange;
            }
            else
            {
                suspension.compressionProgress = 0f;
            }
        }
        
        /// <summary>
        /// Расчет энергии подвески
        /// </summary>
        private static void CalculateSuspensionEnergyStatic(ref SuspensionComponent suspension, float deltaTime)
        {
            // Потенциальная энергия пружины
            float compression = suspension.restLength - suspension.currentLength;
            float potentialEnergy = 0.5f * suspension.springStiffness * compression * compression;
            
            // Кинетическая энергия демпфера
            float kineticEnergy = 0.5f * suspension.damping * suspension.compressionVelocity * suspension.compressionVelocity;
            
            // Общая энергия
            suspension.energy = potentialEnergy + kineticEnergy;
            
            // Работа за кадр
            suspension.work = (suspension.springForce + suspension.damperForce) * suspension.compressionVelocity * deltaTime;
        }
    }
}
