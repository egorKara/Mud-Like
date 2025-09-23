using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;
using Unity.Burst;
using Unity.Jobs;
using Unity.Collections;
using MudLike.Vehicles.Components;
using MudLike.Core.Components;
using static MudLike.Core.Components.Position;

namespace MudLike.Vehicles.Systems
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
                ComponentType.ReadWrite<WinchData>(),
                ComponentType.ReadOnly<LocalTransform>()
            );
            
            _cableQuery = GetEntityQuery(
                ComponentType.ReadWrite<WinchCableData>()
            );
            
            _connectionQuery = GetEntityQuery(
                ComponentType.ReadWrite<WinchConnectionData>()
            );
            
            _vehicleQuery = GetEntityQuery(
                ComponentType.ReadWrite<VehiclePhysics>(),
                ComponentType.ReadOnly<LocalTransform>()
            );
        }
        
        protected override void OnUpdate()
        {
            float deltaTime = SystemAPI.Time.fixedDeltaTime;
            var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().PhysicsWorld;
            
            var winchJob = new WinchJob
            {
                DeltaTime = deltaTime,
                PhysicsWorld = physicsWorld
            };
            
            Dependency = winchJob.ScheduleParallel(_winchQuery, Dependency);
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
                if (!winchData.IsActive) return;
                
                // Обновляем позицию крепления
                winchData.AttachmentPoint = transform.Position;
                
                // Обновляем трос
                UpdateCable(ref winchData, transform);
                
                // Обновляем подключение
                UpdateConnection(ref winchData, transform);
                
                // Применяем силу лебедки
                ApplyWinchForce(ref winchData, transform);
                
                winchData.NeedsUpdate = true;
            }
            
            /// <summary>
            /// Обновляет трос лебедки
            /// </summary>
            private void UpdateCable(ref WinchData winchData, in LocalTransform transform)
            {
                if (!winchData.IsDeployed) return;
                
                // Вычисляем направление троса
                if (winchData.IsConnected)
                {
                    winchData.CableDirection = math.normalize(winchData.ConnectionPoint - winchData.AttachmentPoint);
                    winchData.CableLength = math.distance(winchData.AttachmentPoint, winchData.ConnectionPoint);
                }
                else
                {
                    // Трос не подключен, используем максимальную длину
                    winchData.CableLength = winchData.MaxCableLength;
                    winchData.CableDirection = math.forward(transform.Rotation);
                }
                
                // Ограничиваем длину троса
                winchData.CableLength = math.clamp(winchData.CableLength, 0f, winchData.MaxCableLength);
                
                // Вычисляем напряжение троса
                winchData.CableTension = CalculateCableTension(winchData);
                
                // Обновляем износ троса
                UpdateCableWear(ref winchData);
            }
            
            /// <summary>
            /// Обновляет подключение лебедки
            /// </summary>
            private void UpdateConnection(ref WinchData winchData, in LocalTransform transform)
            {
                if (!winchData.IsConnected) return;
                
                // Проверяем, что подключение все еще активно
                if (!IsConnectionValid(winchData))
                {
                    DisconnectWinch(ref winchData);
                    return;
                }
                
                // Обновляем позицию подключения
                winchData.ConnectionPoint = GetConnectionPosition(winchData);
                
                // Вычисляем силу подключения
                winchData.WinchForce = CalculateWinchForce(winchData);
                
                // Ограничиваем силу лебедки
                winchData.WinchForce = math.clamp(winchData.WinchForce, 0f, winchData.MaxWinchForce);
            }
            
            /// <summary>
            /// Применяет силу лебедки
            /// </summary>
            private void ApplyWinchForce(ref WinchData winchData, in LocalTransform transform)
            {
                if (!winchData.IsConnected || winchData.WinchForce <= 0f) return;
                
                // Находим подключенный объект
                var connectedObject = GetConnectedObject(winchData);
                if (connectedObject == Entity.Null) return;
                
                // Вычисляем направление силы
                float3 forceDirection = winchData.CableDirection;
                
                // Применяем силу к подключенному объекту
                ApplyForceToObject(connectedObject, forceDirection * winchData.WinchForce);
                
                // Применяем обратную силу к транспорту
                ApplyForceToVehicle(transform, -forceDirection * winchData.WinchForce);
            }
            
            /// <summary>
            /// Вычисляет напряжение троса
            /// </summary>
            private float CalculateCableTension(WinchData winchData)
            {
                if (!winchData.IsConnected) return 0f;
                
                // Напряжение зависит от длины троса и приложенной силы
                float lengthFactor = winchData.CableLength / winchData.MaxCableLength;
                float forceFactor = winchData.WinchForce / winchData.MaxWinchForce;
                
                return math.clamp(lengthFactor * forceFactor, 0f, 1f);
            }
            
            /// <summary>
            /// Обновляет износ троса
            /// </summary>
            private void UpdateCableWear(ref WinchData winchData)
            {
                if (!winchData.IsConnected) return;
                
                // Износ зависит от напряжения и времени
                float tensionWear = winchData.CableTension * 0.001f * DeltaTime;
                float timeWear = 0.0001f * DeltaTime;
                
                winchData.CableWear += tensionWear + timeWear;
                winchData.CableWear = math.clamp(winchData.CableWear, 0f, 1f);
                
                // Обновляем прочность троса
                winchData.CableStrength = 1f - winchData.CableWear;
                
                // Проверяем, не сломался ли трос
                if (winchData.CableStrength <= 0f)
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
                if (!winchData.IsConnected)
                    return false;
                
                // Проверяем, что подключенный объект все еще существует
                if (winchData.ConnectedEntityId == Entity.Null)
                    return false;
                
                // Проверяем максимальную дистанцию подключения
                float currentDistance = math.distance(winchData.ConnectionPoint, winchData.AttachmentPoint);
                if (currentDistance > winchData.MaxConnectionDistance)
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
                if (winchData.IsConnected && winchData.ConnectedEntityId != Entity.Null)
                {
                    // В реальной реализации здесь должен быть доступ к Transform подключенного объекта
                    // Для ECS это может быть через EntityManager или SystemAPI
                    return winchData.ConnectedObjectPosition;
                }
                
                // Иначе возвращаем точку подключения лебедки
                return winchData.ConnectionPoint;
            }
            
            /// <summary>
            /// Вычисляет силу лебедки
            /// </summary>
            private float CalculateWinchForce(WinchData winchData)
            {
                // Базовая сила лебедки
                float baseForce = winchData.MaxWinchForce * 0.5f;
                
                // Увеличиваем силу в зависимости от напряжения
                float tensionForce = winchData.CableTension * winchData.MaxWinchForce * 0.5f;
                
                return baseForce + tensionForce;
            }
            
            /// <summary>
            /// Получает подключенный объект
            /// </summary>
            [BurstCompile]
            private Entity GetConnectedObject(WinchData winchData)
            {
                // Возвращаем ID подключенной сущности
                if (winchData.IsConnected)
                    return winchData.ConnectedEntityId;
                
                return Entity.Null;
            }
            
            /// <summary>
            /// Применяет силу к объекту
            /// </summary>
            [BurstCompile]
            private void ApplyForceToObject(Entity entity, float3 force)
            {
                // Применяем силу к физическому телу объекта
                if (EntityManager.HasComponent<PhysicsVelocity>(entity))
                {
                    var velocity = EntityManager.GetComponentData<PhysicsVelocity>(entity);
                    velocity.Linear += force * SystemAPI.Time.fixedDeltaTime;
                    EntityManager.SetComponentData(entity, velocity);
                }
                
                // Если объект имеет компонент VehiclePhysics, применяем к нему
                if (EntityManager.HasComponent<VehiclePhysics>(entity))
                {
                    var vehiclePhysics = EntityManager.GetComponentData<VehiclePhysics>(entity);
                    vehiclePhysics.Velocity += force * SystemAPI.Time.fixedDeltaTime;
                    EntityManager.SetComponentData(entity, vehiclePhysics);
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
                float deltaTime = SystemAPI.Time.fixedDeltaTime;
                transform.Position += force * deltaTime * deltaTime * 0.5f;
                
                // Дополнительно можем применить к VehiclePhysics если есть
                // Это будет сделано в основной системе движения транспорта
            }
            
            /// <summary>
            /// Отключает лебедку
            /// </summary>
            private void DisconnectWinch(ref WinchData winchData)
            {
                winchData.IsConnected = false;
                winchData.ConnectionPoint = float3.zero;
                winchData.WinchForce = 0f;
            }
        }
    }
}