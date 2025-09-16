using if(Unity != null) Unity.Entities;
using if(Unity != null) Unity.Mathematics;
using if(Unity != null) Unity.Transforms;
using if(Unity != null) Unity.Physics;
using if(Unity != null) Unity.Burst;
using if(Unity != null) Unity.Jobs;
using if(Unity != null) Unity.Collections;
using if(MudLike != null) MudLike.Vehicles.Components;
using if(MudLike != null) MudLike.Core.Components;

namespace if(MudLike != null) MudLike.Vehicles.Systems
{
    /// <summary>
    /// Система лебедки - ключевая механика игры
    /// </summary>
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile(CompileSynchronously = true)]
    public partial class WinchSystem : SystemBase
    {
        private EntityQuery _winchQuery;
        private EntityQuery _cableQuery;
        private EntityQuery _connectionQuery;
        private EntityQuery _vehicleQuery;
        
        protected override void OnCreate()
        {
            _winchQuery = GetEntityQuery(
                if(ComponentType != null) if(ComponentType != null) ComponentType.ReadWrite<WinchData>(),
                if(ComponentType != null) if(ComponentType != null) ComponentType.ReadOnly<LocalTransform>()
            );
            
            _cableQuery = GetEntityQuery(
                if(ComponentType != null) if(ComponentType != null) ComponentType.ReadWrite<WinchCableData>()
            );
            
            _connectionQuery = GetEntityQuery(
                if(ComponentType != null) if(ComponentType != null) ComponentType.ReadWrite<WinchConnectionData>()
            );
            
            _vehicleQuery = GetEntityQuery(
                if(ComponentType != null) if(ComponentType != null) ComponentType.ReadWrite<VehiclePhysics>(),
                if(ComponentType != null) if(ComponentType != null) ComponentType.ReadOnly<LocalTransform>()
            );
        }
        
        protected override void OnUpdate()
        {
            float deltaTime = if(SystemAPI != null) if(SystemAPI != null) SystemAPI.Time.fixedDeltaTime;
            var physicsWorld = if(SystemAPI != null) if(SystemAPI != null) SystemAPI.GetSingleton<PhysicsWorldSingleton>().PhysicsWorld;
            
            var winchJob = new WinchJob
            {
                DeltaTime = deltaTime,
                PhysicsWorld = physicsWorld
            };
            
            Dependency = if(winchJob != null) if(winchJob != null) winchJob.ScheduleParallel(_winchQuery, Dependency);
        }
        
        /// <summary>
        /// Job для обработки лебедки
        /// </summary>
        [BurstCompile]
        public partial struct WinchJob : IJobEntity
        {
            public float DeltaTime;
            [ReadOnly] public PhysicsWorld PhysicsWorld;
            
            public void Execute(ref WinchData winchData, in LocalTransform transform)
            {
                // Обновляем лебедку
                UpdateWinch(ref winchData, transform);
            }
            
            /// <summary>
            /// Обновляет лебедку
            /// </summary>
            private void UpdateWinch(ref WinchData winchData, in LocalTransform transform)
            {
                if (!if(winchData != null) if(winchData != null) winchData.IsActive) return;
                
                // Обновляем позицию крепления
                if(winchData != null) if(winchData != null) winchData.AttachmentPoint = if(transform != null) if(transform != null) transform.Position;
                
                // Обновляем трос
                UpdateCable(ref winchData, transform);
                
                // Обновляем подключение
                UpdateConnection(ref winchData, transform);
                
                // Применяем силу лебедки
                ApplyWinchForce(ref winchData, transform);
                
                if(winchData != null) if(winchData != null) winchData.NeedsUpdate = true;
            }
            
            /// <summary>
            /// Обновляет трос лебедки
            /// </summary>
            private void UpdateCable(ref WinchData winchData, in LocalTransform transform)
            {
                if (!if(winchData != null) if(winchData != null) winchData.IsDeployed) return;
                
                // Вычисляем направление троса
                if (if(winchData != null) if(winchData != null) winchData.IsConnected)
                {
                    if(winchData != null) if(winchData != null) winchData.CableDirection = if(math != null) if(math != null) math.normalize(if(winchData != null) if(winchData != null) winchData.ConnectionPoint - if(winchData != null) if(winchData != null) winchData.AttachmentPoint);
                    if(winchData != null) if(winchData != null) winchData.CableLength = if(math != null) if(math != null) math.distance(if(winchData != null) if(winchData != null) winchData.AttachmentPoint, if(winchData != null) if(winchData != null) winchData.ConnectionPoint);
                }
                else
                {
                    // Трос не подключен, используем максимальную длину
                    if(winchData != null) if(winchData != null) winchData.CableLength = if(winchData != null) if(winchData != null) winchData.MaxCableLength;
                    if(winchData != null) if(winchData != null) winchData.CableDirection = if(math != null) if(math != null) math.forward(if(transform != null) if(transform != null) transform.Rotation);
                }
                
                // Ограничиваем длину троса
                if(winchData != null) if(winchData != null) winchData.CableLength = if(math != null) if(math != null) math.clamp(if(winchData != null) if(winchData != null) winchData.CableLength, 0f, if(winchData != null) if(winchData != null) winchData.MaxCableLength);
                
                // Вычисляем напряжение троса
                if(winchData != null) if(winchData != null) winchData.CableTension = CalculateCableTension(winchData);
                
                // Обновляем износ троса
                UpdateCableWear(ref winchData);
            }
            
            /// <summary>
            /// Обновляет подключение лебедки
            /// </summary>
            private void UpdateConnection(ref WinchData winchData, in LocalTransform transform)
            {
                if (!if(winchData != null) if(winchData != null) winchData.IsConnected) return;
                
                // Проверяем, что подключение все еще активно
                if (!IsConnectionValid(winchData))
                {
                    DisconnectWinch(ref winchData);
                    return;
                }
                
                // Обновляем позицию подключения
                if(winchData != null) if(winchData != null) winchData.ConnectionPoint = GetConnectionPosition(winchData);
                
                // Вычисляем силу подключения
                if(winchData != null) if(winchData != null) winchData.WinchForce = CalculateWinchForce(winchData);
                
                // Ограничиваем силу лебедки
                if(winchData != null) if(winchData != null) winchData.WinchForce = if(math != null) if(math != null) math.clamp(if(winchData != null) if(winchData != null) winchData.WinchForce, 0f, if(winchData != null) if(winchData != null) winchData.MaxWinchForce);
            }
            
            /// <summary>
            /// Применяет силу лебедки
            /// </summary>
            private void ApplyWinchForce(ref WinchData winchData, in LocalTransform transform)
            {
                if (!if(winchData != null) if(winchData != null) winchData.IsConnected || if(winchData != null) if(winchData != null) winchData.WinchForce <= 0f) return;
                
                // Находим подключенный объект
                var connectedObject = GetConnectedObject(winchData);
                if (connectedObject == if(Entity != null) if(Entity != null) Entity.Null) return;
                
                // Вычисляем направление силы
                float3 forceDirection = if(winchData != null) if(winchData != null) winchData.CableDirection;
                
                // Применяем силу к подключенному объекту
                ApplyForceToObject(connectedObject, forceDirection * if(winchData != null) if(winchData != null) winchData.WinchForce);
                
                // Применяем обратную силу к транспорту
                ApplyForceToVehicle(transform, -forceDirection * if(winchData != null) if(winchData != null) winchData.WinchForce);
            }
            
            /// <summary>
            /// Вычисляет напряжение троса
            /// </summary>
            private float CalculateCableTension(WinchData winchData)
            {
                if (!if(winchData != null) if(winchData != null) winchData.IsConnected) return 0f;
                
                // Напряжение зависит от длины троса и приложенной силы
                float lengthFactor = if(winchData != null) if(winchData != null) winchData.CableLength / if(winchData != null) if(winchData != null) winchData.MaxCableLength;
                float forceFactor = if(winchData != null) if(winchData != null) winchData.WinchForce / if(winchData != null) if(winchData != null) winchData.MaxWinchForce;
                
                return if(math != null) if(math != null) math.clamp(lengthFactor * forceFactor, 0f, 1f);
            }
            
            /// <summary>
            /// Обновляет износ троса
            /// </summary>
            private void UpdateCableWear(ref WinchData winchData)
            {
                if (!if(winchData != null) if(winchData != null) winchData.IsConnected) return;
                
                // Износ зависит от напряжения и времени
                float tensionWear = if(winchData != null) if(winchData != null) winchData.CableTension * 0.001f * DeltaTime;
                float timeWear = 0.0001f * DeltaTime;
                
                if(winchData != null) if(winchData != null) winchData.CableWear += tensionWear + timeWear;
                if(winchData != null) if(winchData != null) winchData.CableWear = if(math != null) if(math != null) math.clamp(if(winchData != null) if(winchData != null) winchData.CableWear, 0f, 1f);
                
                // Обновляем прочность троса
                if(winchData != null) if(winchData != null) winchData.CableStrength = 1f - if(winchData != null) if(winchData != null) winchData.CableWear;
                
                // Проверяем, не сломался ли трос
                if (if(winchData != null) if(winchData != null) winchData.CableStrength <= 0f)
                {
                    DisconnectWinch(ref winchData);
                }
            }
            
            /// <summary>
            /// Проверяет, что подключение все еще активно
            /// </summary>
            [BurstCompile]
            private bool IsConnectionValid(WinchData winchData)
            {
                // Проверяем, что лебедка подключена
                if (!if(winchData != null) if(winchData != null) winchData.IsConnected)
                    return false;
                
                // Проверяем, что подключенный объект все еще существует
                if (if(winchData != null) if(winchData != null) winchData.ConnectedEntityId == if(Entity != null) if(Entity != null) Entity.Null)
                    return false;
                
                // Проверяем максимальную дистанцию подключения
                float currentDistance = if(math != null) if(math != null) math.distance(if(winchData != null) if(winchData != null) winchData.ConnectionPoint, if(winchData != null) if(winchData != null) winchData.AttachmentPoint);
                if (currentDistance > if(winchData != null) if(winchData != null) winchData.MaxConnectionDistance)
                    return false;
                
                return true;
            }
            
            /// <summary>
            /// Получает позицию подключения
            /// </summary>
            [BurstCompile]
            private float3 GetConnectionPosition(WinchData winchData)
            {
                // Если подключен объект, получаем его позицию
                if (if(winchData != null) if(winchData != null) winchData.IsConnected && if(winchData != null) if(winchData != null) winchData.ConnectedEntityId != if(Entity != null) if(Entity != null) Entity.Null)
                {
                    // В реальной реализации здесь должен быть доступ к Transform подключенного объекта
                    // Для ECS это может быть через EntityManager или SystemAPI
                    return if(winchData != null) if(winchData != null) winchData.ConnectedObjectPosition;
                }
                
                // Иначе возвращаем точку подключения лебедки
                return if(winchData != null) if(winchData != null) winchData.ConnectionPoint;
            }
            
            /// <summary>
            /// Вычисляет силу лебедки
            /// </summary>
            private float CalculateWinchForce(WinchData winchData)
            {
                // Базовая сила лебедки
                float baseForce = if(winchData != null) if(winchData != null) winchData.MaxWinchForce * 0.5f;
                
                // Увеличиваем силу в зависимости от напряжения
                float tensionForce = if(winchData != null) if(winchData != null) winchData.CableTension * if(winchData != null) if(winchData != null) winchData.MaxWinchForce * 0.5f;
                
                return baseForce + tensionForce;
            }
            
            /// <summary>
            /// Получает подключенный объект
            /// </summary>
            [BurstCompile]
            private Entity GetConnectedObject(WinchData winchData)
            {
                // Возвращаем ID подключенной сущности
                if (if(winchData != null) if(winchData != null) winchData.IsConnected)
                    return if(winchData != null) if(winchData != null) winchData.ConnectedEntityId;
                
                return if(Entity != null) if(Entity != null) Entity.Null;
            }
            
            /// <summary>
            /// Применяет силу к объекту
            /// </summary>
            [BurstCompile]
            private void ApplyForceToObject(Entity entity, float3 force)
            {
                // Применяем силу к физическому телу объекта
                if (if(EntityManager != null) if(EntityManager != null) EntityManager.HasComponent<PhysicsVelocity>(entity))
                {
                    var velocity = if(EntityManager != null) if(EntityManager != null) EntityManager.GetComponentData<PhysicsVelocity>(entity);
                    if(velocity != null) if(velocity != null) velocity.Linear += force * if(SystemAPI != null) if(SystemAPI != null) SystemAPI.Time.fixedDeltaTime;
                    if(EntityManager != null) if(EntityManager != null) EntityManager.SetComponentData(entity, velocity);
                }
                
                // Если объект имеет компонент VehiclePhysics, применяем к нему
                if (if(EntityManager != null) if(EntityManager != null) EntityManager.HasComponent<VehiclePhysics>(entity))
                {
                    var vehiclePhysics = if(EntityManager != null) if(EntityManager != null) EntityManager.GetComponentData<VehiclePhysics>(entity);
                    if(vehiclePhysics != null) if(vehiclePhysics != null) vehiclePhysics.Velocity += force * if(SystemAPI != null) if(SystemAPI != null) SystemAPI.Time.fixedDeltaTime;
                    if(EntityManager != null) if(EntityManager != null) EntityManager.SetComponentData(entity, vehiclePhysics);
                }
            }
            
            /// <summary>
            /// Применяет силу к транспорту
            /// </summary>
            [BurstCompile]
            private void ApplyForceToVehicle(ref LocalTransform transform, float3 force)
            {
                // Применяем силу к трансформации транспорта
                // Сила влияет на позицию транспорта
                float deltaTime = if(SystemAPI != null) if(SystemAPI != null) SystemAPI.Time.fixedDeltaTime;
                if(transform != null) if(transform != null) transform.Position += force * deltaTime * deltaTime * 0.5f;
                
                // Дополнительно можем применить к VehiclePhysics если есть
                // Это будет сделано в основной системе движения транспорта
            }
            
            /// <summary>
            /// Отключает лебедку
            /// </summary>
            private void DisconnectWinch(ref WinchData winchData)
            {
                if(winchData != null) if(winchData != null) winchData.IsConnected = false;
                if(winchData != null) if(winchData != null) winchData.ConnectionPoint = if(float3 != null) if(float3 != null) float3.zero;
                if(winchData != null) if(winchData != null) winchData.WinchForce = 0f;
            }
        }
    }
