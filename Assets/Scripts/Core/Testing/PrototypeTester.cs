using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using MudLike.Core.Components;
using MudLike.Core.Systems;

namespace MudLike.Core.Testing
{
    /// <summary>
    /// Тестер для проверки работы ECS прототипа
    /// </summary>
    public class PrototypeTester : MonoBehaviour
    {
        [Header("Test Settings")]
        [SerializeField] private bool createTestPlayer = true;
        [SerializeField] private Vector3 playerPosition = Vector3.zero;
        
        private EntityManager entityManager;
        private Entity testPlayerEntity;
        
        void Start()
        {
            // Получаем EntityManager
            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            
            if (createTestPlayer)
            {
                CreateTestPlayer();
            }
            
            // Выводим информацию о системах
            LogSystemInfo();
        }
        
        void Update()
        {
            // Проверяем состояние тестового игрока
            if (testPlayerEntity != Entity.Null && entityManager.Exists(testPlayerEntity))
            {
                CheckPlayerState();
            }
        }
        
        /// <summary>
        /// Создает тестового игрока
        /// </summary>
        private void CreateTestPlayer()
        {
            // Создаем Entity
            testPlayerEntity = entityManager.CreateEntity();
            
            // Добавляем компоненты
            entityManager.AddComponent<PlayerTag>(testPlayerEntity);
            entityManager.AddComponent<PlayerInput>(testPlayerEntity);
            entityManager.AddComponent<Velocity>(testPlayerEntity);
            entityManager.AddComponent<Position>(testPlayerEntity);
            entityManager.AddComponent<MovementSpeed>(testPlayerEntity);
            entityManager.AddComponent<RotationSpeed>(testPlayerEntity);
            entityManager.AddComponent<LocalTransform>(testPlayerEntity);
            
            // Устанавливаем начальные значения
            entityManager.SetComponentData(testPlayerEntity, new Position { Value = playerPosition });
            entityManager.SetComponentData(testPlayerEntity, new Velocity { Value = float3.zero });
            entityManager.SetComponentData(testPlayerEntity, new MovementSpeed 
            { 
                Value = 10f, 
                MaxSpeed = 10f, 
                Acceleration = 5f, 
                Deceleration = 8f 
            });
            entityManager.SetComponentData(testPlayerEntity, new RotationSpeed { Value = 180f });
            entityManager.SetComponentData(testPlayerEntity, new LocalTransform 
            { 
                Position = playerPosition, 
                Rotation = quaternion.identity, 
                Scale = 1f 
            });
            
            Debug.Log($"✅ Тестовый игрок создан: Entity {testPlayerEntity.Index}");
        }
        
        /// <summary>
        /// Проверяет состояние игрока
        /// </summary>
        private void CheckPlayerState()
        {
            if (entityManager.HasComponent<LocalTransform>(testPlayerEntity))
            {
                var transform = entityManager.GetComponentData<LocalTransform>(testPlayerEntity);
                var velocity = entityManager.GetComponentData<Velocity>(testPlayerEntity);
                var input = entityManager.GetComponentData<PlayerInput>(testPlayerEntity);
                
                // Выводим информацию каждые 60 кадров
                if (Time.frameCount % 60 == 0)
                {
                    Debug.Log($"🎮 Игрок - Позиция: {transform.Position}, Скорость: {velocity.Value}, Ввод: {input.Movement}");
                }
            }
        }
        
        /// <summary>
        /// Выводит информацию о системах
        /// </summary>
        private void LogSystemInfo()
        {
            var world = World.DefaultGameObjectInjectionWorld;
            
            Debug.Log("🔧 ECS Системы:");
            Debug.Log($"   - InputSystem: {world.GetExistingSystem<InputSystem>() != null}");
            Debug.Log($"   - PlayerMovementSystem: {world.GetExistingSystem<PlayerMovementSystem>() != null}");
            Debug.Log($"   - PlayerRotationSystem: {world.GetExistingSystem<PlayerRotationSystem>() != null}");
            
            // Проверяем количество игроков
            var playerQuery = entityManager.CreateEntityQuery(typeof(PlayerTag));
            Debug.Log($"👥 Количество игроков: {playerQuery.CalculateEntityCount()}");
        }
        
        /// <summary>
        /// Создает дополнительного игрока для тестирования
        /// </summary>
        [ContextMenu("Create Additional Player")]
        public void CreateAdditionalPlayer()
        {
            var newEntity = entityManager.CreateEntity();
            entityManager.AddComponent<PlayerTag>(newEntity);
            entityManager.AddComponent<PlayerInput>(newEntity);
            entityManager.AddComponent<Velocity>(newEntity);
            entityManager.AddComponent<Position>(newEntity);
            entityManager.AddComponent<MovementSpeed>(newEntity);
            entityManager.AddComponent<RotationSpeed>(newEntity);
            entityManager.AddComponent<LocalTransform>(newEntity);
            
            entityManager.SetComponentData(newEntity, new Position { Value = new float3(5, 0, 5) });
            entityManager.SetComponentData(newEntity, new Velocity { Value = float3.zero });
            entityManager.SetComponentData(newEntity, new MovementSpeed 
            { 
                Value = 8f, 
                MaxSpeed = 8f, 
                Acceleration = 4f, 
                Deceleration = 6f 
            });
            entityManager.SetComponentData(newEntity, new RotationSpeed { Value = 150f });
            entityManager.SetComponentData(newEntity, new LocalTransform 
            { 
                Position = new float3(5, 0, 5), 
                Rotation = quaternion.identity, 
                Scale = 1f 
            });
            
            Debug.Log($"✅ Дополнительный игрок создан: Entity {newEntity.Index}");
        }
        
        /// <summary>
        /// Удаляет всех игроков
        /// </summary>
        [ContextMenu("Clear All Players")]
        public void ClearAllPlayers()
        {
            var playerQuery = entityManager.CreateEntityQuery(typeof(PlayerTag));
            entityManager.DestroyEntity(playerQuery);
            testPlayerEntity = Entity.Null;
            Debug.Log("🗑️ Все игроки удалены");
        }
    }
}
