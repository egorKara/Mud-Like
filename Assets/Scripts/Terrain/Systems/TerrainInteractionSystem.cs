using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using MudLike.Terrain.Components;

namespace MudLike.Terrain.Systems
{
    /// <summary>
    /// ECS система для взаимодействия с местностью
    /// </summary>
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial class TerrainInteractionSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            float deltaTime = SystemAPI.Time.fixedDeltaTime;
            
            // Обновление взаимодействий с местностью
            Entities.ForEach((ref TerrainInteractionComponent interaction, in LocalTransform transform) =>
            {
                // Обновление позиции взаимодействия
                UpdateInteractionPosition(ref interaction, transform);
                
                // Получение свойств поверхности
                GetSurfaceProperties(ref interaction);
                
                // Расчет сил взаимодействия
                CalculateInteractionForces(ref interaction, deltaTime);
                
                // Создание деформации при необходимости
                if (interaction.createsDeformation)
                {
                    CreateDeformationFromInteraction(ref interaction);
                }
                
                // Обновление энергии взаимодействия
                UpdateInteractionEnergy(ref interaction, deltaTime);
                
            }).Schedule();
            
            // Обработка взаимодействий колес с местностью
            ProcessWheelTerrainInteractions(deltaTime);
        }
        
        /// <summary>
        /// Обновление позиции взаимодействия
        /// </summary>
        private static void UpdateInteractionPosition(ref TerrainInteractionComponent interaction, LocalTransform transform)
        {
            interaction.worldPosition = transform.Position;
            interaction.lastUpdateTime = 0f; // Временная заглушка
        }
        
        /// <summary>
        /// Получение свойств поверхности
        /// </summary>
        private static void GetSurfaceProperties(ref TerrainInteractionComponent interaction)
        {
            // Здесь должна быть логика получения свойств поверхности
            // с учетом всех активных деформаций и материалов
            
            // Пока используем упрощенные значения
            interaction.surfaceHeight = 0f;
            interaction.surfaceNormal = new float3(0, 1, 0);
            interaction.surfaceMaterial = SurfaceMaterial.Mud;
            
            // Получение локальных свойств материала
            GetMaterialProperties(interaction.surfaceMaterial, out interaction.localFriction, 
                out interaction.localGrip, out interaction.localStiffness, out interaction.localDamping);
        }
        
        /// <summary>
        /// Получение свойств материала
        /// </summary>
        private static void GetMaterialProperties(SurfaceMaterial material, out float friction, 
            out float grip, out float stiffness, out float damping)
        {
            switch (material)
            {
                case SurfaceMaterial.Mud:
                    friction = 0.3f;
                    grip = 0.2f;
                    stiffness = 1000f;
                    damping = 50f;
                    break;
                    
                case SurfaceMaterial.Sand:
                    friction = 0.4f;
                    grip = 0.3f;
                    stiffness = 2000f;
                    damping = 100f;
                    break;
                    
                case SurfaceMaterial.Clay:
                    friction = 0.5f;
                    grip = 0.4f;
                    stiffness = 5000f;
                    damping = 200f;
                    break;
                    
                case SurfaceMaterial.Rock:
                    friction = 0.8f;
                    grip = 0.9f;
                    stiffness = 50000f;
                    damping = 1000f;
                    break;
                    
                case SurfaceMaterial.Grass:
                    friction = 0.6f;
                    grip = 0.5f;
                    stiffness = 3000f;
                    damping = 150f;
                    break;
                    
                case SurfaceMaterial.Snow:
                    friction = 0.2f;
                    grip = 0.1f;
                    stiffness = 500f;
                    damping = 25f;
                    break;
                    
                case SurfaceMaterial.Ice:
                    friction = 0.1f;
                    grip = 0.05f;
                    stiffness = 10000f;
                    damping = 500f;
                    break;
                    
                case SurfaceMaterial.Water:
                    friction = 0.05f;
                    grip = 0.02f;
                    stiffness = 100f;
                    damping = 10f;
                    break;
                    
                case SurfaceMaterial.Asphalt:
                    friction = 0.9f;
                    grip = 0.95f;
                    stiffness = 100000f;
                    damping = 2000f;
                    break;
                    
                case SurfaceMaterial.Concrete:
                    friction = 0.85f;
                    grip = 0.9f;
                    stiffness = 80000f;
                    damping = 1500f;
                    break;
                    
                default:
                    friction = 0.5f;
                    grip = 0.5f;
                    stiffness = 5000f;
                    damping = 250f;
                    break;
            }
        }
        
        /// <summary>
        /// Расчет сил взаимодействия
        /// </summary>
        private static void CalculateInteractionForces(ref TerrainInteractionComponent interaction, float deltaTime)
        {
            if (!interaction.isActive)
                return;
            
            // Расчет силы взаимодействия на основе скорости и свойств поверхности
            float velocityMagnitude = math.length(interaction.interactionVelocity);
            
            if (velocityMagnitude > 0.001f)
            {
                // Сила трения
                float frictionForce = interaction.interactionForce * interaction.localFriction;
                
                // Сила сцепления
                float gripForce = interaction.interactionForce * interaction.localGrip;
                
                // Общая сила взаимодействия
                float totalForce = frictionForce + gripForce;
                
                // Обновление энергии взаимодействия
                interaction.interactionEnergy = totalForce * velocityMagnitude * deltaTime;
                interaction.interactionWork += interaction.interactionEnergy;
            }
        }
        
        /// <summary>
        /// Создание деформации из взаимодействия
        /// </summary>
        private static void CreateDeformationFromInteraction(ref TerrainInteractionComponent interaction)
        {
            if (interaction.interactionForce < 100f) // Минимальная сила для деформации
                return;
            
            // Создание деформации
            DeformationType deformationType = GetDeformationTypeFromInteraction(interaction.interactionType);
            
            // Здесь должна быть логика создания деформации
            // Пока используем заглушку
        }
        
        /// <summary>
        /// Получение типа деформации из типа взаимодействия
        /// </summary>
        private static DeformationType GetDeformationTypeFromInteraction(InteractionType interactionType)
        {
            switch (interactionType)
            {
                case InteractionType.WheelContact:
                case InteractionType.Rolling:
                    return DeformationType.WheelTrack;
                    
                case InteractionType.Impact:
                    return DeformationType.Impact;
                    
                case InteractionType.Pressure:
                    return DeformationType.Tool;
                    
                default:
                    return DeformationType.WheelTrack;
            }
        }
        
        /// <summary>
        /// Обновление энергии взаимодействия
        /// </summary>
        private static void UpdateInteractionEnergy(ref TerrainInteractionComponent interaction, float deltaTime)
        {
            // Расчет энергии на основе силы и скорости
            float velocityMagnitude = math.length(interaction.interactionVelocity);
            interaction.interactionEnergy = interaction.interactionForce * velocityMagnitude * deltaTime;
        }
        
        /// <summary>
        /// Обработка взаимодействий колес с местностью
        /// </summary>
        private void ProcessWheelTerrainInteractions(float deltaTime)
        {
            // Здесь должна быть логика обработки взаимодействий колес с местностью
            // Пока используем заглушку
        }
        
        /// <summary>
        /// Создание взаимодействия для колеса
        /// </summary>
        public static void CreateWheelInteraction(
            ref TerrainInteractionComponent interaction,
            float3 position,
            float force,
            float3 velocity,
            float radius)
        {
            interaction.worldPosition = position;
            interaction.interactionForce = force;
            interaction.interactionVelocity = velocity;
            interaction.interactionRadius = radius;
            interaction.interactionType = InteractionType.WheelContact;
            interaction.isActive = true;
            interaction.createsDeformation = true;
            interaction.affectsPhysics = true;
            interaction.lastUpdateTime = 0f; // Временная заглушка
        }
    }
}
