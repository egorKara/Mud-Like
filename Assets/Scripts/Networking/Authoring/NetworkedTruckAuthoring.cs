using Unity.Entities;
using Unity.NetCode;
using Unity.Mathematics;
using MudLike.Vehicles.Components;
using MudLike.Networking.Components;

namespace MudLike.Networking.Authoring
{
    /// <summary>
    /// Авторинг компонент для создания сетевого грузовика
    /// </summary>
    public class NetworkedTruckAuthoring : MonoBehaviour
    {
        [Header("Network Settings")]
        [SerializeField] private bool isPredicted = true;
        [SerializeField] private bool isInterpolated = true;
        
        /// <summary>
        /// Bake компонент для создания сетевой ECS сущности
        /// </summary>
        private class NetworkedTruckBaker : Baker<NetworkedTruckAuthoring>
        {
            public override void Bake(NetworkedTruckAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                
                // Добавляем основные компоненты грузовика
                AddComponent(entity, new PlayerTag());
                AddComponent(entity, new TruckData
                {
                    Mass = 8000f,
                    EnginePower = 300f,
                    MaxTorque = 1200f,
                    MaxSpeed = 80f,
                    MaxSteeringAngle = 35f,
                    CurrentGear = 1,
                    EngineRPM = 800f,
                    CurrentSpeed = 0f,
                    SteeringAngle = 0f,
                    TractionCoefficient = 0.8f,
                    FuelLevel = 1f,
                    EngineRunning = false,
                    HandbrakeOn = false,
                    LockFrontDifferential = false,
                    LockMiddleDifferential = false,
                    LockRearDifferential = false,
                    LockCenterDifferential = false
                });
                
                AddComponent(entity, new TruckControl
                {
                    Throttle = 0f,
                    Brake = 0f,
                    Steering = 0f,
                    Handbrake = false,
                    ShiftUp = false,
                    ShiftDown = false,
                    ToggleEngine = false,
                    Clutch = 0f,
                    FourWheelDrive = false,
                    LockFrontDifferential = false,
                    LockMiddleDifferential = false,
                    LockRearDifferential = false,
                    LockCenterDifferential = false
                });
                
                // Добавляем сетевые компоненты
                AddComponent(entity, new NetworkedTruckData
                {
                    Position = float3.zero,
                    Rotation = quaternion.identity,
                    Velocity = float3.zero,
                    AngularVelocity = float3.zero,
                    CurrentGear = 1,
                    EngineRPM = 800f,
                    CurrentSpeed = 0f,
                    SteeringAngle = 0f,
                    EngineRunning = false,
                    HandbrakeOn = false,
                    LockFrontDifferential = false,
                    LockMiddleDifferential = false,
                    LockRearDifferential = false,
                    LockCenterDifferential = false,
                    FuelLevel = 1f
                });
                
                // Добавляем компоненты Netcode
                if (authoring.isPredicted)
                {
                    AddComponent(entity, new PredictedGhost());
                }
                
                if (authoring.isInterpolated)
                {
                    AddComponent(entity, new InterpolatedGhost());
                }
                
                // Добавляем компонент для синхронизации
                AddComponent(entity, new GhostOwnerComponent());
                
                // Создаем сетевые колеса
                CreateNetworkedWheels(entity);
            }
            
            /// <summary>
            /// Создает сетевые колеса
            /// </summary>
            private void CreateNetworkedWheels(Entity truckEntity)
            {
                // Позиции колес (6 колес для КРАЗ)
                var wheelPositions = new float3[]
                {
                    new float3(-1.2f, 0.8f, 2.5f),   // Переднее левое
                    new float3(1.2f, 0.8f, 2.5f),    // Переднее правое
                    new float3(-1.2f, 0.8f, 0f),     // Среднее левое
                    new float3(1.2f, 0.8f, 0f),      // Среднее правое
                    new float3(-1.2f, 0.8f, -2.5f),  // Заднее левое
                    new float3(1.2f, 0.8f, -2.5f)    // Заднее правое
                };
                
                for (int i = 0; i < wheelPositions.Length; i++)
                {
                    var wheelEntity = CreateAdditionalEntity(TransformUsageFlags.Dynamic);
                    
                    // Добавляем компоненты колеса
                    AddComponent(wheelEntity, new WheelData
                    {
                        LocalPosition = wheelPositions[i],
                        Radius = 0.5f,
                        Width = 0.3f,
                        AngularVelocity = 0f,
                        SteerAngle = 0f,
                        Torque = 0f,
                        BrakeTorque = 0f,
                        TractionCoefficient = 0.8f,
                        SinkDepth = 0f,
                        TractionForce = float3.zero,
                        SlipRatio = 0f,
                        IsDriven = true,
                        IsSteerable = i < 2,
                        WheelIndex = i
                    });
                    
                    // Добавляем сетевые компоненты колеса
                    AddComponent(wheelEntity, new NetworkedWheelData
                    {
                        Position = wheelPositions[i],
                        Rotation = quaternion.identity,
                        AngularVelocity = 0f,
                        SteerAngle = 0f,
                        Torque = 0f,
                        BrakeTorque = 0f,
                        TractionCoefficient = 0.8f,
                        SinkDepth = 0f,
                        SlipRatio = 0f,
                        WheelIndex = i
                    });
                    
                    // Добавляем компоненты Netcode
                    AddComponent(wheelEntity, new PredictedGhost());
                    AddComponent(wheelEntity, new InterpolatedGhost());
                    AddComponent(wheelEntity, new GhostOwnerComponent());
                    
                    // Связываем колесо с грузовиком
                    AddComponent(wheelEntity, new Parent
                    {
                        Value = truckEntity
                    });
                }
            }
        }
    }
}