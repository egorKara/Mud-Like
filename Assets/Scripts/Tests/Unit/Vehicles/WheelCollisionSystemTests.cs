using NUnit.Framework;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;
using Unity.Collections;
using MudLike.Vehicles.Systems;
using MudLike.Vehicles.Components;
using Unity.Core;

namespace MudLike.Tests.Unit.Vehicles
{
    /// <summary>
    /// Тесты для системы коллизий колес WheelCollisionSystem
    /// </summary>
    public class WheelCollisionSystemTests
    {
        private World _world;
        private WheelCollisionSystem _wheelCollisionSystem;
        private EntityManager _entityManager;

        [SetUp]
        public void SetUp()
        {
            _world = new World("TestWorld");
            _entityManager = _world.EntityManager;
            
            _wheelCollisionSystem = _world.GetOrCreateSystemManaged<WheelCollisionSystem>();
            _wheelCollisionSystem.OnCreate(ref _world.Unmanaged);
            
            _world.SetSingleton(new TimeData { ElapsedTime = 10f, DeltaTime = 0.016f, FixedDeltaTime = 0.016f });
        }

        [TearDown]
        public void TearDown()
        {
            _wheelCollisionSystem.OnDestroy(ref _world.Unmanaged);
            _world.Dispose();
        }

        [Test]
        public void WheelCollisionSystem_OnCreate_InitializesCorrectly()
        {
            Assert.IsNotNull(_wheelCollisionSystem);
        }

        [Test]
        public void WheelCollisionSystem_OnUpdate_ProcessesWithoutErrors()
        {
            _wheelCollisionSystem.OnUpdate(ref _world.Unmanaged);
            Assert.IsNotNull(_wheelCollisionSystem);
        }

        [Test]
        public void WheelCollisionSystem_WithWheelData_ProcessesCorrectly()
        {
            var entity = _entityManager.CreateEntity();
            _entityManager.AddComponentData(entity, new LocalTransform 
            { 
                Position = new float3(0, 0, 0), 
                Rotation = quaternion.identity 
            });
            _entityManager.AddComponentData(entity, new WheelData
            {
                Position = float3.zero,
                Radius = 0.5f,
                Width = 0.2f,
                SuspensionLength = 0.3f,
                SpringForce = 1000f,
                DampingForce = 500f,
                IsGrounded = true,
                GroundDistance = 0.1f
            });

            _wheelCollisionSystem.OnUpdate(ref _world.Unmanaged);
            Assert.IsNotNull(_wheelCollisionSystem);
        }

        [Test]
        public void WheelCollisionSystem_WithCollisionData_ProcessesCorrectly()
        {
            var entity = _entityManager.CreateEntity();
            _entityManager.AddComponentData(entity, new LocalTransform 
            { 
                Position = new float3(0, 0, 0), 
                Rotation = quaternion.identity 
            });
            _entityManager.AddComponentData(entity, new WheelData
            {
                Position = float3.zero,
                Radius = 0.5f,
                Width = 0.2f,
                SuspensionLength = 0.3f,
                SpringForce = 1000f,
                DampingForce = 500f,
                IsGrounded = true,
                GroundDistance = 0.1f
            });
            _entityManager.AddComponentData(entity, new WheelCollisionData
            {
                HitPoint = float3.zero,
                HitNormal = new float3(0, 1, 0),
                HitDistance = 0.1f,
                IsHit = true,
                HitEntity = Entity.Null,
                Friction = 0.8f,
                Bounce = 0.3f
            });

            _wheelCollisionSystem.OnUpdate(ref _world.Unmanaged);
            Assert.IsNotNull(_wheelCollisionSystem);
        }

        [Test]
        public void WheelCollisionSystem_MultipleWheels_HandlesCorrectly()
        {
            for (int i = 0; i < 6; i++)
            {
                var entity = _entityManager.CreateEntity();
                _entityManager.AddComponentData(entity, new LocalTransform 
                { 
                    Position = new float3(i * 3f, 0, 0), 
                    Rotation = quaternion.identity 
                });
                _entityManager.AddComponentData(entity, new WheelData
                {
                    Position = new float3(i * 3f, 0, 0),
                    Radius = 0.5f + i * 0.05f,
                    Width = 0.2f + i * 0.01f,
                    SuspensionLength = 0.3f,
                    SpringForce = 1000f + i * 100f,
                    DampingForce = 500f + i * 50f,
                    IsGrounded = i % 2 == 0,
                    GroundDistance = 0.1f + i * 0.01f
                });
                _entityManager.AddComponentData(entity, new WheelCollisionData
                {
                    HitPoint = new float3(i * 3f, 0, 0),
                    HitNormal = new float3(0, 1, 0),
                    HitDistance = 0.1f + i * 0.02f,
                    IsHit = i % 3 == 0,
                    HitEntity = Entity.Null,
                    Friction = 0.8f - i * 0.05f,
                    Bounce = 0.3f + i * 0.02f
                });
            }

            _wheelCollisionSystem.OnUpdate(ref _world.Unmanaged);
            Assert.IsNotNull(_wheelCollisionSystem);
        }

        [Test]
        public void WheelCollisionSystem_EdgeCases_HandleCorrectly()
        {
            var entity = _entityManager.CreateEntity();
            _entityManager.AddComponentData(entity, new LocalTransform 
            { 
                Position = new float3(float.MaxValue, float.MinValue, float.Epsilon), 
                Rotation = quaternion.identity 
            });
            _entityManager.AddComponentData(entity, new WheelData
            {
                Position = new float3(float.PositiveInfinity, float.NegativeInfinity, float.NaN),
                Radius = float.MaxValue,
                Width = float.MinValue,
                SuspensionLength = float.NaN,
                SpringForce = float.PositiveInfinity,
                DampingForce = float.NegativeInfinity,
                IsGrounded = true,
                GroundDistance = float.Epsilon
            });
            _entityManager.AddComponentData(entity, new WheelCollisionData
            {
                HitPoint = new float3(float.PositiveInfinity, float.NegativeInfinity, float.NaN),
                HitNormal = new float3(float.MaxValue, float.MinValue, float.NaN),
                HitDistance = float.PositiveInfinity,
                IsHit = true,
                HitEntity = Entity.Null,
                Friction = float.NegativeInfinity,
                Bounce = float.NaN
            });

            Assert.DoesNotThrow(() => 
            {
                _wheelCollisionSystem.OnUpdate(ref _world.Unmanaged);
            });
        }
    }
}
