using NUnit.Framework;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using MudLike.Audio.Components;
using MudLike.Vehicles.Components;

namespace MudLike.Tests.Unit.Audio
{
    /// <summary>
    /// Unit тесты для EngineAudioSystem
    /// </summary>
    [TestFixture]
    public class EngineAudioSystemTests : ECSTestFixture
    {
        private EngineAudioSystem _audioSystem;
        private Entity _audioEntity;
        private Entity _vehicleEntity;
        
        [SetUp]
        public override void Setup()
        {
            base.Setup();
            
            // Создаем систему
            _audioSystem = World.CreateSystemManaged<EngineAudioSystem>();
            
            // Создаем аудио сущность
            _audioEntity = EntityManager.CreateEntity();
            EntityManager.AddComponentData(_audioEntity, new EngineAudioData
            {
                RPM = 0f,
                Power = 0f,
                Temperature = 0f,
                Volume = 0f,
                Pitch = 0f,
                IsPlaying = false,
                NeedsUpdate = false
            });
            
            // Создаем транспорт
            _vehicleEntity = EntityManager.CreateEntity();
            EntityManager.AddComponentData(_vehicleEntity, new VehiclePhysics
            {
                EngineRPM = 1000f,
                EnginePower = 0.5f,
                EngineTorque = 200f
            });
            EntityManager.AddComponentData(_vehicleEntity, new EngineData
            {
                MaxRPM = 6000f,
                IdleRPM = 800f,
                Torque = 300f,
                Power = 200f
            });
            EntityManager.AddComponentData(_vehicleEntity, new LocalTransform
            {
                Position = new float3(0f, 0f, 0f),
                Rotation = quaternion.identity
            });
        }
        
        [Test]
        public void EngineAudioSystem_UpdatesRPMCorrectly()
        {
            // Arrange
            var vehiclePhysics = EntityManager.GetComponentData<VehiclePhysics>(_vehicleEntity);
            vehiclePhysics.EngineRPM = 2500f;
            EntityManager.SetComponentData(_vehicleEntity, vehiclePhysics);
            
            // Act
            _audioSystem.Update();
            
            // Assert
            var audioData = EntityManager.GetComponentData<EngineAudioData>(_audioEntity);
            Assert.AreEqual(2500f, audioData.RPM);
        }
        
        [Test]
        public void EngineAudioSystem_UpdatesPowerCorrectly()
        {
            // Arrange
            var vehiclePhysics = EntityManager.GetComponentData<VehiclePhysics>(_vehicleEntity);
            vehiclePhysics.EnginePower = 0.7f;
            EntityManager.SetComponentData(_vehicleEntity, vehiclePhysics);
            
            // Act
            _audioSystem.Update();
            
            // Assert
            var audioData = EntityManager.GetComponentData<EngineAudioData>(_audioEntity);
            Assert.AreEqual(0.7f, audioData.Power);
        }
        
        [Test]
        public void EngineAudioSystem_CalculatesVolumeCorrectly()
        {
            // Arrange
            var vehiclePhysics = EntityManager.GetComponentData<VehiclePhysics>(_vehicleEntity);
            vehiclePhysics.EngineRPM = 3000f; // 50% от максимальных 6000
            EntityManager.SetComponentData(_vehicleEntity, vehiclePhysics);
            
            // Act
            _audioSystem.Update();
            
            // Assert
            var audioData = EntityManager.GetComponentData<EngineAudioData>(_audioEntity);
            // Базовая громкость 0.3 + RPM громкость 0.5 * 0.7 = 0.65
            Assert.AreEqual(0.65f, audioData.Volume, 0.01f);
        }
        
        [Test]
        public void EngineAudioSystem_CalculatesPitchCorrectly()
        {
            // Arrange
            var vehiclePhysics = EntityManager.GetComponentData<VehiclePhysics>(_vehicleEntity);
            vehiclePhysics.EngineRPM = 3000f; // 50% от максимальных 6000
            EntityManager.SetComponentData(_vehicleEntity, vehiclePhysics);
            
            // Act
            _audioSystem.Update();
            
            // Assert
            var audioData = EntityManager.GetComponentData<EngineAudioData>(_audioEntity);
            // Базовая высота 0.5 + RPM высота 0.5 * 0.5 = 0.75
            Assert.AreEqual(0.75f, audioData.Pitch, 0.01f);
        }
        
        [Test]
        public void EngineAudioSystem_SetsIsPlayingWhenRPMIsHigh()
        {
            // Arrange
            var vehiclePhysics = EntityManager.GetComponentData<VehiclePhysics>(_vehicleEntity);
            vehiclePhysics.EngineRPM = 1000f; // Выше порога 0.1f
            EntityManager.SetComponentData(_vehicleEntity, vehiclePhysics);
            
            // Act
            _audioSystem.Update();
            
            // Assert
            var audioData = EntityManager.GetComponentData<EngineAudioData>(_audioEntity);
            Assert.IsTrue(audioData.IsPlaying);
        }
        
        [Test]
        public void EngineAudioSystem_SetsIsPlayingFalseWhenRPMIsLow()
        {
            // Arrange
            var vehiclePhysics = EntityManager.GetComponentData<VehiclePhysics>(_vehicleEntity);
            vehiclePhysics.EngineRPM = 0.05f; // Ниже порога 0.1f
            EntityManager.SetComponentData(_vehicleEntity, vehiclePhysics);
            
            // Act
            _audioSystem.Update();
            
            // Assert
            var audioData = EntityManager.GetComponentData<EngineAudioData>(_audioEntity);
            Assert.IsFalse(audioData.IsPlaying);
        }
        
        [Test]
        public void EngineAudioSystem_SetsNeedsUpdateFlag()
        {
            // Act
            _audioSystem.Update();
            
            // Assert
            var audioData = EntityManager.GetComponentData<EngineAudioData>(_audioEntity);
            Assert.IsTrue(audioData.NeedsUpdate);
        }
        
        [Test]
        public void EngineAudioSystem_ClampsVolumeBetweenZeroAndOne()
        {
            // Arrange
            var vehiclePhysics = EntityManager.GetComponentData<VehiclePhysics>(_vehicleEntity);
            vehiclePhysics.EngineRPM = 10000f; // Очень высокий RPM
            EntityManager.SetComponentData(_vehicleEntity, vehiclePhysics);
            
            // Act
            _audioSystem.Update();
            
            // Assert
            var audioData = EntityManager.GetComponentData<EngineAudioData>(_audioEntity);
            Assert.GreaterOrEqual(audioData.Volume, 0f);
            Assert.LessOrEqual(audioData.Volume, 1f);
        }
        
        [Test]
        public void EngineAudioSystem_ClampsPitchBetweenZeroAndTwo()
        {
            // Arrange
            var vehiclePhysics = EntityManager.GetComponentData<VehiclePhysics>(_vehicleEntity);
            vehiclePhysics.EngineRPM = 10000f; // Очень высокий RPM
            EntityManager.SetComponentData(_vehicleEntity, vehiclePhysics);
            
            // Act
            _audioSystem.Update();
            
            // Assert
            var audioData = EntityManager.GetComponentData<EngineAudioData>(_audioEntity);
            Assert.GreaterOrEqual(audioData.Pitch, 0.1f);
            Assert.LessOrEqual(audioData.Pitch, 2f);
        }
    }
}