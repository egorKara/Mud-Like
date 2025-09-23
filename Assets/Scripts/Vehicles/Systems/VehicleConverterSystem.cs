using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;
using MudLike.Vehicles.Components;
using MudLike.Core.Components;

namespace MudLike.Vehicles.Systems
{
    /// <summary>
    /// ECS система для конвертации транспортных средств
    /// Заменяет MonoBehaviour VehicleConverter
    /// </summary>
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class VehicleConverterSystem : SystemBase
    {
        private EntityQuery _conversionQuery;
        
        protected override void OnCreate()
        {
            // Создаем запрос для сущностей, требующих конвертации
            _conversionQuery = GetEntityQuery(typeof(ConversionRequest), typeof(VehicleTag));
        }
        
        protected override void OnUpdate()
        {
            // Обрабатываем запросы на конвертацию
            ProcessConversionRequests();
        }
        
        /// <summary>
        /// Обрабатывает запросы на конвертацию
        /// </summary>
        private void ProcessConversionRequests()
        {
            Entities
                .WithAll<ConversionRequest, VehicleTag>()
                .ForEach((Entity entity, ref ConversionRequest request) =>
                {
                    if (request.ShouldConvert)
                    {
                        ConvertVehicle(entity, request);
                        request.ShouldConvert = false;
                    }
                }).WithoutBurst().Run();
        }
        
        /// <summary>
        /// Конвертирует транспортное средство
        /// </summary>
        private void ConvertVehicle(Entity entity, ConversionRequest request)
        {
            // Добавляем основные компоненты
            EntityManager.AddComponentData(entity, new LocalTransform
            {
                Position = request.Position,
                Rotation = request.Rotation,
                Scale = request.Scale
            });
            
            // Конфигурация транспортного средства
            EntityManager.AddComponentData(entity, new VehicleConfig
            {
                MaxSpeed = request.MaxSpeed,
                Acceleration = request.Acceleration,
                TurnSpeed = request.TurnSpeed,
                Mass = request.Mass,
                Drag = request.Drag,
                AngularDrag = request.AngularDrag,
                TurnRadius = request.TurnRadius,
                CenterOfMassHeight = request.CenterOfMassHeight
            });
            
            // Физика транспортного средства
            EntityManager.AddComponentData(entity, new VehiclePhysics
            {
                Velocity = float3.zero,
                AngularVelocity = float3.zero,
                Acceleration = float3.zero,
                AngularAcceleration = float3.zero,
                CenterOfMass = new float3(0, request.CenterOfMassHeight, 0),
                Mass = request.Mass,
                Drag = request.Drag,
                AngularDrag = request.AngularDrag
            });
            
            // Ввод игрока
            EntityManager.AddComponentData(entity, new VehicleInput
            {
                Throttle = 0f,
                Brake = 0f,
                Steering = 0f,
                Handbrake = false,
                GearUp = false,
                GearDown = false
            });
            
            // Данные двигателя
            EntityManager.AddComponentData(entity, new EngineData
            {
                CurrentRPM = request.IdleRPM,
                MaxRPM = request.MaxRPM,
                IdleRPM = request.IdleRPM,
                Torque = request.Torque,
                Power = request.Power,
                ThrottleInput = 0f,
                IsRunning = true
            });
            
            // Данные трансмиссии
            EntityManager.AddComponentData(entity, new TransmissionData
            {
                CurrentGear = 1,
                GearCount = request.GearCount,
                GearRatios = new Unity.Collections.NativeArray<float>(request.GearRatios, Unity.Collections.Allocator.Persistent),
                FinalDriveRatio = request.FinalDriveRatio,
                GearRatio = request.GearRatios[0],
                IsAutomatic = true,
                ShiftTime = 0.5f
            });
            
            // Физическое тело
            EntityManager.AddComponentData(entity, new PhysicsBody
            {
                Mass = request.Mass,
                Drag = request.Drag,
                AngularDrag = request.AngularDrag,
                CenterOfMass = new float3(0, request.CenterOfMassHeight, 0),
                Velocity = float3.zero,
                AngularVelocity = float3.zero
            });
            
            // Физический коллайдер
            EntityManager.AddComponentData(entity, new PhysicsCollider
            {
                ColliderType = ColliderType.Box,
                Size = new float3(2f, 1f, 4f),
                Center = float3.zero,
                Material = PhysicsMaterial.Default
            });
            
            // Сетевые компоненты
            EntityManager.AddComponentData(entity, new MudLike.Networking.Components.NetworkId
            {
                Value = (uint)entity.Index
            });
            
            EntityManager.AddComponentData(entity, new MudLike.Networking.Components.NetworkPosition
            {
                Value = request.Position,
                Rotation = request.Rotation,
                HasChanged = true,
                LastUpdateTime = 0f,
                Tick = 0
            });
            
            EntityManager.AddComponentData(entity, new MudLike.Networking.Components.NetworkVehicle
            {
                Config = new VehicleConfig
                {
                    MaxSpeed = request.MaxSpeed,
                    Acceleration = request.Acceleration,
                    TurnSpeed = request.TurnSpeed,
                    Mass = request.Mass,
                    Drag = request.Drag,
                    AngularDrag = request.AngularDrag,
                    TurnRadius = request.TurnRadius,
                    CenterOfMassHeight = request.CenterOfMassHeight
                },
                Physics = new VehiclePhysics
                {
                    Velocity = float3.zero,
                    AngularVelocity = float3.zero,
                    Acceleration = float3.zero,
                    AngularAcceleration = float3.zero,
                    CenterOfMass = new float3(0, request.CenterOfMassHeight, 0),
                    Mass = request.Mass,
                    Drag = request.Drag,
                    AngularDrag = request.AngularDrag
                },
                HasChanged = true,
                LastUpdateTime = 0f,
                Tick = 0
            });
        }
    }
    
    /// <summary>
    /// Запрос на конвертацию транспортного средства
    /// </summary>
    public struct ConversionRequest : IComponentData
    {
        public bool ShouldConvert;
        public float3 Position;
        public quaternion Rotation;
        public float Scale;
        
        // Vehicle Configuration
        public float MaxSpeed;
        public float Acceleration;
        public float TurnSpeed;
        public float Mass;
        public float Drag;
        public float AngularDrag;
        public float TurnRadius;
        public float CenterOfMassHeight;
        
        // Engine Configuration
        public float MaxRPM;
        public float IdleRPM;
        public float Torque;
        public float Power;
        
        // Transmission Configuration
        public int GearCount;
        public float FinalDriveRatio;
        
        // Wheel Configuration
        public float WheelRadius;
        public float SuspensionLength;
        public float SpringForce;
        public float DampingForce;
    }
}
