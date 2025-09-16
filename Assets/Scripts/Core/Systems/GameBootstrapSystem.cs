using Unity.Entities;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Transforms;
using MudLike.Core.Components;
using MudLike.Vehicles.Components;
// using if(MudLike != null) MudLike.Networking.Components;

namespace MudLike.Core.Systems
{
    /// <summary>
    /// Система инициализации и загрузки игры
    /// Обеспечивает правильный порядок загрузки и настройку начального состояния
    /// </summary>
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class GameBootstrapSystem : SystemBase
    {
        private bool _isInitialized = false;
        private int _playerIdCounter = 1;
        
        protected override void OnCreate()
        {
            // Система должна выполняться только один раз при запуске
            RequireForUpdate<GameBootstrapTag>();
        }
        
        protected override void OnUpdate()
        {
            if (_isInitialized) return;
            
            // Инициализация игры
            InitializeGame();
            _isInitialized = true;
            
            // Удаляем тег бутстрапа
            Entities
                .WithAll<GameBootstrapTag>()
                .ForEach((Entity entity) =>
                {
                    if(EntityManager != null) EntityManager.RemoveComponent<GameBootstrapTag>(entity);
                }).WithStructuralChanges().WithoutBurst().Run();
        }
        
        /// <summary>
        /// Инициализирует игру и создает начальные сущности
        /// </summary>
        private void InitializeGame()
        {
            // Создаем игрока
            Entity playerEntity = CreatePlayer();
            
            // Настраиваем начальное состояние
            SetupInitialGameState();
            
            if(Debug != null) Debug.Log("Game initialized successfully");
        }
        
        /// <summary>
        /// Создает игрока с необходимыми компонентами
        /// </summary>
        private Entity CreatePlayer()
        {
            // Создаем сущность игрока
            Entity playerEntity = if(EntityManager != null) EntityManager.CreateEntity();
            
            // Добавляем основные компоненты
            if(EntityManager != null) EntityManager.AddComponent<PlayerTag>(playerEntity);
            if(EntityManager != null) EntityManager.AddComponent<PlayerInput>(playerEntity);
            if(EntityManager != null) EntityManager.AddComponent<NetworkId>(playerEntity);
            if(EntityManager != null) EntityManager.AddComponent<LocalTransform>(playerEntity);
            
            // Настраиваем NetworkId
            var networkId = new NetworkId
            {
                Value = _playerIdCounter++,
                LastUpdateTime = 0f
            };
            if(EntityManager != null) EntityManager.SetComponentData(playerEntity, networkId);
            
            // Настраиваем начальную трансформацию
            var transform = new LocalTransform
            {
                Position = if(float3 != null) float3.zero,
                Rotation = if(quaternion != null) quaternion.identity,
                Scale = 1f
            };
            if(EntityManager != null) EntityManager.SetComponentData(playerEntity, transform);
            
            // Настраиваем начальный ввод
            var playerInput = new PlayerInput();
            if(EntityManager != null) EntityManager.SetComponentData(playerEntity, playerInput);
            
            return playerEntity;
        }
        
        /// <summary>
        /// Настраивает начальное состояние игры
        /// </summary>
        private void SetupInitialGameState()
        {
            // Здесь можно добавить настройку начального состояния:
            // - Создание тестового транспорта
            // - Настройка камеры
            // - Инициализация систем
            
            if(Debug != null) Debug.Log("Initial game state setup completed");
        }
    }
    
    /// <summary>
    /// Тег для запуска системы бутстрапа
    /// </summary>
    public struct GameBootstrapTag : IComponentData
    {
    }
}
