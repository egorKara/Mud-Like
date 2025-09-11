using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;
using Unity.Burst;
using Unity.Jobs;
using Unity.Collections;
using MudLike.Vehicles.Components;
using MudLike.Core.Components;

namespace MudLike.Vehicles.Systems
{
    /// <summary>
    /// Система лебедки - ключевая механика игры
    /// </summary>
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile]
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
            float deltaTime = Time.fixedDeltaTime;
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
            private bool IsConnectionValid(WinchData winchData)
            {
                // TODO: Реализовать проверку подключения
                return true;
            }
            
            /// <summary>
            /// Получает позицию подключения
            /// </summary>
            private float3 GetConnectionPosition(WinchData winchData)
            {
                // TODO: Реализовать получение позиции подключения
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
            private Entity GetConnectedObject(WinchData winchData)
            {
                // TODO: Реализовать получение подключенного объекта
                return Entity.Null;
            }
            
            /// <summary>
            /// Применяет силу к объекту
            /// </summary>
            private void ApplyForceToObject(Entity entity, float3 force)
            {
                // TODO: Реализовать применение силы к объекту
            }
            
            /// <summary>
            /// Применяет силу к транспорту
            /// </summary>
            private void ApplyForceToVehicle(LocalTransform transform, float3 force)
            {
                // TODO: Реализовать применение силы к транспорту
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