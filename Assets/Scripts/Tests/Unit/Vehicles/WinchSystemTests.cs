using NUnit.Framework;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using MudLike.Vehicles.Components;

namespace MudLike.Tests.Unit.Vehicles
{
    /// <summary>
    /// Unit тесты для WinchSystem
    /// </summary>
    [TestFixture]
    public class WinchSystemTests : ECSTestFixture
    {
        private WinchSystem _winchSystem;
        private Entity _winchEntity;
        private Entity _vehicleEntity;
        
        [SetUp]
        public override void Setup()
        {
            base.Setup();
            
            // Создаем систему
            _winchSystem = World.CreateSystemManaged<WinchSystem>();
            
            // Создаем лебедку
            _winchEntity = EntityManager.CreateEntity();
            EntityManager.AddComponentData(_winchEntity, new WinchData
            {
                IsActive = true,
                IsDeployed = false,
                IsConnected = false,
                CableLength = 0f,
                MaxCableLength = 50f,
                WinchForce = 0f,
                MaxWinchForce = 10000f,
                CableSpeed = 5f,
                AttachmentPoint = new float3(0f, 0f, 0f),
                ConnectionPoint = new float3(0f, 0f, 0f),
                CableDirection = new float3(0f, 0f, 1f),
                CableTension = 0f,
                CableStrength = 1f,
                CableWear = 0f,
                NeedsUpdate = false
            });
            EntityManager.AddComponentData(_winchEntity, new LocalTransform
            {
                Position = new float3(0f, 0f, 0f),
                Rotation = quaternion.identity
            });
            
            // Создаем транспорт
            _vehicleEntity = EntityManager.CreateEntity();
            EntityManager.AddComponentData(_vehicleEntity, new VehiclePhysics
            {
                Velocity = new float3(0f, 0f, 0f),
                AngularVelocity = new float3(0f, 0f, 0f),
                AppliedForce = new float3(0f, 0f, 0f),
                AppliedTorque = new float3(0f, 0f, 0f)
            });
            EntityManager.AddComponentData(_vehicleEntity, new LocalTransform
            {
                Position = new float3(0f, 0f, 0f),
                Rotation = quaternion.identity
            });
        }
        
        [Test]
        public void WinchSystem_UpdatesAttachmentPointCorrectly()
        {
            // Arrange
            var transform = EntityManager.GetComponentData<LocalTransform>(_winchEntity);
            transform.Position = new float3(10f, 5f, 15f);
            EntityManager.SetComponentData(_winchEntity, transform);
            
            // Act
            _winchSystem.Update();
            
            // Assert
            var winchData = EntityManager.GetComponentData<WinchData>(_winchEntity);
            Assert.AreEqual(new float3(10f, 5f, 15f), winchData.AttachmentPoint);
        }
        
        [Test]
        public void WinchSystem_CalculatesCableLengthWhenConnected()
        {
            // Arrange
            var winchData = EntityManager.GetComponentData<WinchData>(_winchEntity);
            winchData.IsDeployed = true;
            winchData.IsConnected = true;
            winchData.ConnectionPoint = new float3(10f, 0f, 0f);
            EntityManager.SetComponentData(_winchEntity, winchData);
            
            // Act
            _winchSystem.Update();
            
            // Assert
            var updatedWinchData = EntityManager.GetComponentData<WinchData>(_winchEntity);
            Assert.AreEqual(10f, updatedWinchData.CableLength, 0.1f);
        }
        
        [Test]
        public void WinchSystem_UsesMaxCableLengthWhenNotConnected()
        {
            // Arrange
            var winchData = EntityManager.GetComponentData<WinchData>(_winchEntity);
            winchData.IsDeployed = true;
            winchData.IsConnected = false;
            EntityManager.SetComponentData(_winchEntity, winchData);
            
            // Act
            _winchSystem.Update();
            
            // Assert
            var updatedWinchData = EntityManager.GetComponentData<WinchData>(_winchEntity);
            Assert.AreEqual(50f, updatedWinchData.CableLength);
        }
        
        [Test]
        public void WinchSystem_ClampsCableLengthToMax()
        {
            // Arrange
            var winchData = EntityManager.GetComponentData<WinchData>(_winchEntity);
            winchData.IsDeployed = true;
            winchData.IsConnected = true;
            winchData.ConnectionPoint = new float3(100f, 0f, 0f); // Очень далеко
            EntityManager.SetComponentData(_winchEntity, winchData);
            
            // Act
            _winchSystem.Update();
            
            // Assert
            var updatedWinchData = EntityManager.GetComponentData<WinchData>(_winchEntity);
            Assert.AreEqual(50f, updatedWinchData.CableLength); // Ограничено максимумом
        }
        
        [Test]
        public void WinchSystem_CalculatesCableTensionCorrectly()
        {
            // Arrange
            var winchData = EntityManager.GetComponentData<WinchData>(_winchEntity);
            winchData.IsDeployed = true;
            winchData.IsConnected = true;
            winchData.CableLength = 25f; // 50% от максимума
            winchData.WinchForce = 5000f; // 50% от максимума
            EntityManager.SetComponentData(_winchEntity, winchData);
            
            // Act
            _winchSystem.Update();
            
            // Assert
            var updatedWinchData = EntityManager.GetComponentData<WinchData>(_winchEntity);
            // Напряжение = (длина/макс_длина) * (сила/макс_сила) = 0.5 * 0.5 = 0.25
            Assert.AreEqual(0.25f, updatedWinchData.CableTension, 0.01f);
        }
        
        [Test]
        public void WinchSystem_UpdatesCableWearOverTime()
        {
            // Arrange
            var winchData = EntityManager.GetComponentData<WinchData>(_winchEntity);
            winchData.IsDeployed = true;
            winchData.IsConnected = true;
            winchData.CableTension = 0.5f;
            winchData.CableWear = 0f;
            EntityManager.SetComponentData(_winchEntity, winchData);
            
            // Act
            _winchSystem.Update();
            
            // Assert
            var updatedWinchData = EntityManager.GetComponentData<WinchData>(_winchEntity);
            Assert.Greater(updatedWinchData.CableWear, 0f);
        }
        
        [Test]
        public void WinchSystem_UpdatesCableStrengthBasedOnWear()
        {
            // Arrange
            var winchData = EntityManager.GetComponentData<WinchData>(_winchEntity);
            winchData.IsDeployed = true;
            winchData.IsConnected = true;
            winchData.CableWear = 0.3f;
            EntityManager.SetComponentData(_winchEntity, winchData);
            
            // Act
            _winchSystem.Update();
            
            // Assert
            var updatedWinchData = EntityManager.GetComponentData<WinchData>(_winchEntity);
            Assert.AreEqual(0.7f, updatedWinchData.CableStrength, 0.01f); // 1 - 0.3
        }
        
        [Test]
        public void WinchSystem_DisconnectsWhenCableBreaks()
        {
            // Arrange
            var winchData = EntityManager.GetComponentData<WinchData>(_winchEntity);
            winchData.IsDeployed = true;
            winchData.IsConnected = true;
            winchData.CableWear = 1f; // Максимальный износ
            EntityManager.SetComponentData(_winchEntity, winchData);
            
            // Act
            _winchSystem.Update();
            
            // Assert
            var updatedWinchData = EntityManager.GetComponentData<WinchData>(_winchEntity);
            Assert.IsFalse(updatedWinchData.IsConnected);
            Assert.AreEqual(0f, updatedWinchData.WinchForce);
        }
        
        [Test]
        public void WinchSystem_SetsNeedsUpdateFlag()
        {
            // Act
            _winchSystem.Update();
            
            // Assert
            var winchData = EntityManager.GetComponentData<WinchData>(_winchEntity);
            Assert.IsTrue(winchData.NeedsUpdate);
        }
        
        [Test]
        public void WinchSystem_DoesNotUpdateWhenInactive()
        {
            // Arrange
            var winchData = EntityManager.GetComponentData<WinchData>(_winchEntity);
            winchData.IsActive = false;
            winchData.NeedsUpdate = false;
            EntityManager.SetComponentData(_winchEntity, winchData);
            
            // Act
            _winchSystem.Update();
            
            // Assert
            var updatedWinchData = EntityManager.GetComponentData<WinchData>(_winchEntity);
            Assert.IsFalse(updatedWinchData.NeedsUpdate);
        }
    }
}