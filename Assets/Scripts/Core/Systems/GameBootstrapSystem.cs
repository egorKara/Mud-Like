using Unity.Entities;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Transforms;
using MudLike.Core.Components;
using MudLike.Vehicles.Components;

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
                    EntityManager.RemoveComponent<GameBootstrapTag>(entity);
                }).WithoutBurst().Run();
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
            
            Debug.Log("Game initialized successfully");
        }
        
        /// <summary>
        /// Создает игрока с необходимыми компонентами
        /// </summary>
        private Entity CreatePlayer()
        {
            // Создаем сущность игрока
            Entity playerEntity = EntityManager.CreateEntity();
            
            // Добавляем основные компоненты
            EntityManager.AddComponent<PlayerTag>(playerEntity);
            EntityManager.AddComponent<PlayerInput>(playerEntity);
            // EntityManager.AddComponent<NetworkId>(playerEntity); // Требует Networking модуль
            EntityManager.AddComponent<LocalTransform>(playerEntity);
            
            // Настраиваем NetworkId (закомментировано - требует Networking модуль)
            // var networkId = new NetworkId
            // {
            //     Value = _playerIdCounter++,
            //     LastUpdateTime = 0f
            // };
            // EntityManager.SetComponentData(playerEntity, networkId);
            
            // Настраиваем начальную трансформацию
            var transform = new LocalTransform
            {
                Position = float3.zero,
                Rotation = quaternion.identity,
                Scale = 1f
            };
            EntityManager.SetComponentData(playerEntity, transform);
            
            // Настраиваем начальный ввод
            var playerInput = new PlayerInput();
            EntityManager.SetComponentData(playerEntity, playerInput);
            
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
            
            Debug.Log("Initial game state setup completed");
        }
    }
    
    /// <summary>
    /// Тег для запуска системы бутстрапа
    /// </summary>
    public struct GameBootstrapTag : IComponentData
    {
    }
}
