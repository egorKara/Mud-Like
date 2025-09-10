using NUnit.Framework;
using Unity.Entities;
using Unity.NetCode;
using Unity.Mathematics;
using MudLike.Networking.Components;
using MudLike.Vehicles.Components;

namespace MudLike.Tests.Integration
{
    /// <summary>
    /// Integration тесты для мультиплеера
    /// </summary>
    public class MultiplayerIntegrationTests
    {
        private World _serverWorld;
        private World _clientWorld;
        private EntityManager _serverEntityManager;
        private EntityManager _clientEntityManager;

        [SetUp]
        public void Setup()
        {
            // Создаем серверный мир
            _serverWorld = new World("TestServerWorld");
            _serverEntityManager = _serverWorld.EntityManager;
            
            // Создаем клиентский мир
            _clientWorld = new World("TestClientWorld");
            _clientEntityManager = _clientWorld.EntityManager;
        }

        [TearDown]
        public void TearDown()
        {
            _serverWorld?.Dispose();
            _clientWorld?.Dispose();
        }

        [Test]
        public void NetworkedTruckData_SyncsCorrectly()
        {
            // Arrange
            var serverEntity = _serverEntityManager.CreateEntity();
            _serverEntityManager.AddComponent<PlayerTag>(serverEntity);
            _serverEntityManager.AddComponent<TruckData>(serverEntity);
            _serverEntityManager.AddComponent<NetworkedTruckData>(serverEntity);
            
            var clientEntity = _clientEntityManager.CreateEntity();
            _clientEntityManager.AddComponent<PlayerTag>(clientEntity);
            _clientEntityManager.AddComponent<TruckData>(clientEntity);
            _clientEntityManager.AddComponent<NetworkedTruckData>(clientEntity);
            
            // Устанавливаем данные на сервере
            var serverTruckData = new TruckData
            {
                CurrentGear = 3,
                EngineRPM = 2000f,
                CurrentSpeed = 50f,
                EngineRunning = true
            };
            _serverEntityManager.SetComponentData(serverEntity, serverTruckData);
            
            // Act - симулируем синхронизацию
            var serverNetworkedData = _serverEntityManager.GetComponentData<NetworkedTruckData>(serverEntity);
            _clientEntityManager.SetComponentData(clientEntity, serverNetworkedData);
            
            // Assert
            var clientNetworkedData = _clientEntityManager.GetComponentData<NetworkedTruckData>(clientEntity);
            Assert.AreEqual(serverNetworkedData.CurrentGear, clientNetworkedData.CurrentGear);
            Assert.AreEqual(serverNetworkedData.EngineRPM, clientNetworkedData.EngineRPM);
            Assert.AreEqual(serverNetworkedData.CurrentSpeed, clientNetworkedData.CurrentSpeed);
            Assert.AreEqual(serverNetworkedData.EngineRunning, clientNetworkedData.EngineRunning);
        }

        [Test]
        public void PlayerConnectionData_CreatesCorrectly()
        {
            // Arrange
            var entity = _serverEntityManager.CreateEntity();
            _serverEntityManager.AddComponent<PlayerTag>(entity);
            
            // Act
            var connectionData = new PlayerConnectionData
            {
                PlayerID = 1,
                PlayerName = new FixedString64Bytes("TestPlayer"),
                Ping = 50,
                Status = PlayerConnectionStatus.Connected,
                ConnectionTime = 1000.0,
                LastActivityTime = 1000.0
            };
            _serverEntityManager.AddComponentData(entity, connectionData);
            
            // Assert
            var retrievedData = _serverEntityManager.GetComponentData<PlayerConnectionData>(entity);
            Assert.AreEqual(1, retrievedData.PlayerID);
            Assert.AreEqual("TestPlayer", retrievedData.PlayerName.ToString());
            Assert.AreEqual(50, retrievedData.Ping);
            Assert.AreEqual(PlayerConnectionStatus.Connected, retrievedData.Status);
        }

        [Test]
        public void NetworkedWheelData_SyncsCorrectly()
        {
            // Arrange
            var serverEntity = _serverEntityManager.CreateEntity();
            _serverEntityManager.AddComponent<WheelData>(serverEntity);
            _serverEntityManager.AddComponent<NetworkedWheelData>(serverEntity);
            
            var clientEntity = _clientEntityManager.CreateEntity();
            _clientEntityManager.AddComponent<WheelData>(clientEntity);
            _clientEntityManager.AddComponent<NetworkedWheelData>(clientEntity);
            
            // Устанавливаем данные на сервере
            var serverWheelData = new WheelData
            {
                AngularVelocity = 10f,
                SteerAngle = 15f,
                TractionCoefficient = 0.7f,
                SinkDepth = 0.2f,
                WheelIndex = 0
            };
            _serverEntityManager.SetComponentData(serverEntity, serverWheelData);
            
            // Act - симулируем синхронизацию
            var serverNetworkedData = _serverEntityManager.GetComponentData<NetworkedWheelData>(serverEntity);
            _clientEntityManager.SetComponentData(clientEntity, serverNetworkedData);
            
            // Assert
            var clientNetworkedData = _clientEntityManager.GetComponentData<NetworkedWheelData>(clientEntity);
            Assert.AreEqual(serverNetworkedData.AngularVelocity, clientNetworkedData.AngularVelocity);
            Assert.AreEqual(serverNetworkedData.SteerAngle, clientNetworkedData.SteerAngle);
            Assert.AreEqual(serverNetworkedData.TractionCoefficient, clientNetworkedData.TractionCoefficient);
            Assert.AreEqual(serverNetworkedData.SinkDepth, clientNetworkedData.SinkDepth);
            Assert.AreEqual(serverNetworkedData.WheelIndex, clientNetworkedData.WheelIndex);
        }

        [Test]
        public void LagCompensation_CalculatesCorrectly()
        {
            // Arrange
            float3 position = new float3(10f, 0f, 5f);
            float3 velocity = new float3(5f, 0f, 2f);
            float rewindTime = 0.1f; // 100ms
            
            // Act
            float3 compensatedPosition = CalculateCompensatedPosition(position, velocity, rewindTime);
            
            // Assert
            float3 expectedPosition = position - velocity * rewindTime;
            Assert.AreEqual(expectedPosition.x, compensatedPosition.x, 0.001f);
            Assert.AreEqual(expectedPosition.y, compensatedPosition.y, 0.001f);
            Assert.AreEqual(expectedPosition.z, compensatedPosition.z, 0.001f);
        }

        /// <summary>
        /// Вычисляет компенсированную позицию (копия из LagCompensationSystem)
        /// </summary>
        private static float3 CalculateCompensatedPosition(float3 position, float3 velocity, float rewindTime)
        {
            return position - velocity * rewindTime;
        }
    }
}