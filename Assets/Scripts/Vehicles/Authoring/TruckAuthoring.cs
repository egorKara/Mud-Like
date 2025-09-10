using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using MudLike.Vehicles.Components;

namespace MudLike.Vehicles.Authoring
{
    /// <summary>
    /// Авторинг компонент для создания грузовика КРАЗ в ECS
    /// </summary>
    public class TruckAuthoring : MonoBehaviour
    {
        [Header("Truck Settings")]
        [SerializeField] private float mass = 8000f;
        [SerializeField] private float enginePower = 300f;
        [SerializeField] private float maxTorque = 1200f;
        [SerializeField] private float maxSpeed = 80f;
        [SerializeField] private float maxSteeringAngle = 35f;
        
        [Header("Wheel Settings")]
        [SerializeField] private float wheelRadius = 0.5f;
        [SerializeField] private float wheelWidth = 0.3f;
        [SerializeField] private float suspensionLength = 0.8f;
        [SerializeField] private float springForce = 50000f;
        [SerializeField] private float dampingForce = 5000f;
        
        [Header("Transmission Settings")]
        [SerializeField] private float finalDriveRatio = 3.5f;
        [SerializeField] private float differentialRatio = 1.0f;
        [SerializeField] private bool automaticTransmission = true;
        
        /// <summary>
        /// Bake компонент для создания ECS сущности
        /// </summary>
        private class TruckBaker : Baker<TruckAuthoring>
        {
            public override void Bake(TruckAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                
                // Добавляем основные компоненты
                AddComponent(entity, new PlayerTag());
                AddComponent(entity, new TruckData
                {
                    Mass = authoring.mass,
                    EnginePower = authoring.enginePower,
                    MaxTorque = authoring.maxTorque,
                    MaxSpeed = authoring.maxSpeed,
                    MaxSteeringAngle = authoring.maxSteeringAngle,
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
                
                AddComponent(entity, new TransmissionData
                {
                    GearRatios = new float4x4(
                        3.5f, 2.1f, 1.4f, 1.0f,
                        0.8f, 0.6f, 0f, 0f,
                        0f, 0f, 0f, 0f,
                        0f, 0f, 0f, 0f
                    ),
                    CurrentGear = 1,
                    FinalDriveRatio = authoring.finalDriveRatio,
                    DifferentialRatio = authoring.differentialRatio,
                    Efficiency = 0.9f,
                    ShiftTime = 0.5f,
                    ShiftTimer = 0f,
                    AutomaticTransmission = authoring.automaticTransmission,
                    UpshiftRPM = 2200f,
                    DownshiftRPM = 1200f
                });
                
                // Создаем колеса
                CreateWheels(entity, authoring);
            }
            
            /// <summary>
            /// Создает колеса для грузовика
            /// </summary>
            private void CreateWheels(Entity truckEntity, TruckAuthoring authoring)
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
                        Radius = authoring.wheelRadius,
                        Width = authoring.wheelWidth,
                        AngularVelocity = 0f,
                        SteerAngle = 0f,
                        Torque = 0f,
                        BrakeTorque = 0f,
                        TractionCoefficient = 0.8f,
                        SinkDepth = 0f,
                        TractionForce = float3.zero,
                        SlipRatio = 0f,
                        IsDriven = true, // Все колеса ведущие
                        IsSteerable = i < 2, // Только передние колеса поворотные
                        WheelIndex = i
                    });
                    
                    // Добавляем компоненты подвески
                    AddComponent(wheelEntity, new SuspensionData
                    {
                        Length = authoring.suspensionLength,
                        CurrentLength = authoring.suspensionLength,
                        SpringForce = authoring.springForce,
                        DampingForce = authoring.dampingForce,
                        TargetPosition = float3.zero,
                        CompressionVelocity = 0f,
                        MaxCompression = 0.3f,
                        MaxExtension = 0.5f,
                        SuspensionForce = float3.zero,
                        SuspensionIndex = i
                    });
                    
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