using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;
using MudLike.Vehicles.Components;
using MudLike.Core.Components;

namespace MudLike.Gameplay.Systems
{
    /// <summary>
    /// Система спавна транспорта для игроков
    /// Создает транспорт и привязывает к игрокам
    /// </summary>
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [BurstCompile]
    public partial class VehicleSpawningSystem : SystemBase
    {
        private EntityArchetype _vehicleArchetype;
        private EntityArchetype _playerArchetype;
        
        protected override void OnCreate()
        {
            // Создаем архетип для транспорта
            _vehicleArchetype = EntityManager.CreateArchetype(
                typeof(LocalTransform),
                typeof(VehicleTag),
                typeof(VehiclePhysics),
                typeof(VehicleConfig),
                typeof(EngineData),
                typeof(TransmissionData),
                typeof(VehicleDamageData),
                typeof(VehicleFuelData)
            );
            
            // Создаем архетип для игрока
            _playerArchetype = EntityManager.CreateArchetype(
                typeof(PlayerTag),
                typeof(PlayerInput),
                typeof(NetworkId)
            );
        }
        
        protected override void OnUpdate()
        {
            // Проверяем, нужно ли создать транспорт для игроков
            if (ShouldSpawnVehicles())
            {
                SpawnVehiclesForPlayers();
            }
        }
        
        /// <summary>
        /// Проверяет, нужно ли создать транспорт
        /// </summary>
        private bool ShouldSpawnVehicles()
        {
            // Считаем количество игроков без транспорта
            int playersWithoutVehicles = 0;
            
            Entities
                .WithAll<PlayerTag>()
                .WithNone<VehicleTag>()
                .ForEach((Entity entity) =>
                {
                    playersWithoutVehicles++;
                }).WithoutBurst().Run();
            
            return playersWithoutVehicles > 0;
        }
        
        /// <summary>
        /// Создает транспорт для всех игроков без транспорта
        /// </summary>
        private void SpawnVehiclesForPlayers()
        {
            Entities
                .WithAll<PlayerTag>()
                .WithNone<VehicleTag>()
                .ForEach((Entity playerEntity) =>
                {
                    // Создаем транспорт для игрока
                    Entity vehicleEntity = CreateVehicleForPlayer(playerEntity);
                    
                    // Привязываем игрока к транспорту
                    LinkPlayerToVehicle(playerEntity, vehicleEntity);
                    
                }).WithoutBurst().Run();
        }
        
        /// <summary>
        /// Создает транспорт для конкретного игрока
        /// </summary>
        private Entity CreateVehicleForPlayer(Entity playerEntity)
        {
            // Создаем сущность транспорта
            Entity vehicleEntity = EntityManager.CreateEntity(_vehicleArchetype);
            
            // Настраиваем позицию спавна
            float3 spawnPosition = GetSpawnPosition(playerEntity);
            
            // Настраиваем компоненты транспорта
            SetupVehicleComponents(vehicleEntity, spawnPosition);
            
            return vehicleEntity;
        }
        
        /// <summary>
        /// Получает позицию спавна для транспорта
        /// </summary>
        private float3 GetSpawnPosition(Entity playerEntity)
        {
            // Получаем ID игрока для уникальной позиции
            var networkId = SystemAPI.GetComponent<NetworkId>(playerEntity);
            
            // Создаем позицию спавна на основе ID игрока
            float angle = networkId.Value * 45f * math.PI / 180f; // 45 градусов между игроками
            float distance = 10f; // Расстояние от центра
            
            float3 spawnPosition = new float3(
                math.cos(angle) * distance,
                0f,
                math.sin(angle) * distance
            );
            
            return spawnPosition;
        }
        
        /// <summary>
        /// Настраивает компоненты транспорта
        /// </summary>
        private void SetupVehicleComponents(Entity vehicleEntity, float3 spawnPosition)
        {
            // Настраиваем трансформацию
            var transform = new LocalTransform
            {
                Position = spawnPosition,
                Rotation = quaternion.identity,
                Scale = 1f
            };
            SystemAPI.SetComponent(vehicleEntity, transform);
            
            // Настраиваем физику транспорта
            var physics = new VehiclePhysics
            {
                Velocity = float3.zero,
                AngularVelocity = float3.zero,
                Mass = 2000f,
                MaxSpeed = 30f,
                Acceleration = 8f,
                Deceleration = 12f,
                MaxSteeringAngle = 30f,
                TurnSpeedMultiplier = 2f,
                EngineBraking = 3f,
                SteeringReturnSpeed = 5f
            };
            SystemAPI.SetComponent(vehicleEntity, physics);
            
            // Настраиваем конфигурацию транспорта
            var config = new VehicleConfig
            {
                VehicleType = VehicleType.Truck,
                MaxEnginePower = 200f,
                MaxTorque = 800f,
                WheelCount = 6,
                FourWheelDrive = true,
                DifferentialLock = false
            };
            SystemAPI.SetComponent(vehicleEntity, config);
            
            // Настраиваем двигатель
            var engine = new EngineData
            {
                IsRunning = false,
                RPM = 0f,
                MaxRPM = 6000f,
                Torque = 0f,
                Power = 0f,
                Temperature = 20f,
                MaxTemperature = 100f,
                FuelConsumption = 0f,
                IsStarting = false,
                TimeSinceStart = 0f
            };
            SystemAPI.SetComponent(vehicleEntity, engine);
            
            // Настраиваем трансмиссию
            var transmission = new TransmissionData
            {
                CurrentGear = 0, // Нейтральная передача
                MaxGears = 6,
                IsShifting = false,
                ShiftTime = 0.5f,
                TimeSinceShift = 0f,
                GearRatios = new float4(3.2f, 1.9f, 1.3f, 1.0f, 0.8f, 0.6f),
                FinalDriveRatio = 4.1f,
                AutomaticTransmission = true
            };
            SystemAPI.SetComponent(vehicleEntity, transmission);
            
            // Привязываем ввод игрока к транспорту (используем PlayerInput)
            // VehicleInput не нужен, так как используется PlayerInput
            
            // Настраиваем повреждения
            var damage = new VehicleDamageData
            {
                Health = 100f,
                MaxHealth = 100f,
                EngineHealth = 100f,
                TransmissionHealth = 100f,
                WheelHealth = new float4(100f, 100f, 100f, 100f)
            };
            SystemAPI.SetComponent(vehicleEntity, damage);
            
            // Настраиваем топливо
            var fuel = new VehicleFuelData
            {
                CurrentFuel = 100f,
                MaxFuel = 100f,
                FuelConsumptionRate = 0.1f,
                LowFuelThreshold = 20f
            };
            SystemAPI.SetComponent(vehicleEntity, fuel);
        }
        
        /// <summary>
        /// Привязывает игрока к транспорту
        /// </summary>
        private void LinkPlayerToVehicle(Entity playerEntity, Entity vehicleEntity)
        {
            // Добавляем тег игрока к транспорту
            SystemAPI.AddComponent<PlayerTag>(vehicleEntity);
            
            // Создаем связь между игроком и транспортом
            // (В реальной реализации можно использовать компонент связи)
            
            Debug.Log($"Player {playerEntity.Index} linked to vehicle {vehicleEntity.Index}");
        }
    }
    
    /// <summary>
    /// Типы транспорта
    /// </summary>
    public enum VehicleType
    {
        Truck,          // Грузовик
        OffRoad,        // Вездеход
        Trailer,        // Прицеп
        Specialized     // Специализированная техника
    }
}
