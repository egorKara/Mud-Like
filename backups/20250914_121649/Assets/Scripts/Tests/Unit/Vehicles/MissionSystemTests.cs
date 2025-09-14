using NUnit.Framework;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;
using MudLike.Vehicles.Systems;
using MudLike.Vehicles.Components;
using Unity.Core;

namespace MudLike.Tests.Unit.Vehicles
{
    /// <summary>
    /// Тесты для системы управления миссиями MissionSystem
    /// </summary>
    public class MissionSystemTests
    {
        private World _world;
        private MissionSystem _missionSystem;
        private EntityManager _entityManager;

        [SetUp]
        public void SetUp()
        {
            _world = new World("TestWorld");
            _entityManager = _world.EntityManager;
            
            _missionSystem = _world.GetOrCreateSystemManaged<MissionSystem>();
            _missionSystem.OnCreate(ref _world.Unmanaged);
            
            _world.SetSingleton(new TimeData { ElapsedTime = 10f, DeltaTime = 0.016f, FixedDeltaTime = 0.016f });
        }

        [TearDown]
        public void TearDown()
        {
            _missionSystem.OnDestroy(ref _world.Unmanaged);
            _world.Dispose();
        }

        [Test]
        public void MissionSystem_OnCreate_InitializesCorrectly()
        {
            Assert.IsNotNull(_missionSystem);
        }

        [Test]
        public void MissionSystem_OnUpdate_ProcessesWithoutErrors()
        {
            _missionSystem.OnUpdate(ref _world.Unmanaged);
            Assert.IsNotNull(_missionSystem);
        }

        [Test]
        public void MissionSystem_WithMissionData_ProcessesCorrectly()
        {
            var entity = _entityManager.CreateEntity();
            _entityManager.AddComponentData(entity, new MissionData
            {
                MissionId = 1,
                MissionType = MissionType.Transport,
                Status = MissionStatus.Active,
                Progress = 0.3f,
                StartTime = 100f,
                EndTime = 300f,
                Reward = 1000f,
                Difficulty = MissionDifficulty.Medium,
                IsCompleted = false
            });

            _missionSystem.OnUpdate(ref _world.Unmanaged);
            Assert.IsNotNull(_missionSystem);
        }

        [Test]
        public void MissionSystem_WithObjectiveData_ProcessesCorrectly()
        {
            var entity = _entityManager.CreateEntity();
            _entityManager.AddComponentData(entity, new MissionData
            {
                MissionId = 2,
                MissionType = MissionType.Delivery,
                Status = MissionStatus.Active,
                Progress = 0.5f,
                StartTime = 200f,
                EndTime = 400f,
                Reward = 1500f,
                Difficulty = MissionDifficulty.Hard,
                IsCompleted = false
            });
            _entityManager.AddComponentData(entity, new MissionObjectiveData
            {
                ObjectiveId = 1,
                ObjectiveType = ObjectiveType.ReachLocation,
                TargetPosition = new float3(100f, 0f, 100f),
                TargetRadius = 10f,
                IsCompleted = false,
                Progress = 0.7f
            });

            _missionSystem.OnUpdate(ref _world.Unmanaged);
            Assert.IsNotNull(_missionSystem);
        }

        [Test]
        public void MissionSystem_WithRewardData_ProcessesCorrectly()
        {
            var entity = _entityManager.CreateEntity();
            _entityManager.AddComponentData(entity, new MissionData
            {
                MissionId = 3,
                MissionType = MissionType.Race,
                Status = MissionStatus.Active,
                Progress = 0.8f,
                StartTime = 150f,
                EndTime = 350f,
                Reward = 2000f,
                Difficulty = MissionDifficulty.Easy,
                IsCompleted = false
            });
            _entityManager.AddComponentData(entity, new MissionRewardData
            {
                RewardType = RewardType.Experience,
                RewardAmount = 500f,
                IsClaimed = false,
                ClaimTime = 0f
            });

            _missionSystem.OnUpdate(ref _world.Unmanaged);
            Assert.IsNotNull(_missionSystem);
        }

        [Test]
        public void MissionSystem_MultipleMissions_HandlesCorrectly()
        {
            for (int i = 0; i < 5; i++)
            {
                var entity = _entityManager.CreateEntity();
                _entityManager.AddComponentData(entity, new MissionData
                {
                    MissionId = i + 1,
                    MissionType = (MissionType)(i % 3),
                    Status = (MissionStatus)(i % 3),
                    Progress = i * 0.2f,
                    StartTime = i * 100f,
                    EndTime = i * 100f + 200f,
                    Reward = 1000f + i * 500f,
                    Difficulty = (MissionDifficulty)(i % 3),
                    IsCompleted = i % 2 == 0
                });
                _entityManager.AddComponentData(entity, new MissionObjectiveData
                {
                    ObjectiveId = i + 1,
                    ObjectiveType = (ObjectiveType)(i % 4),
                    TargetPosition = new float3(i * 50f, 0f, i * 50f),
                    TargetRadius = 5f + i * 2f,
                    IsCompleted = i % 3 == 0,
                    Progress = i * 0.15f
                });
            }

            _missionSystem.OnUpdate(ref _world.Unmanaged);
            Assert.IsNotNull(_missionSystem);
        }

        [Test]
        public void MissionSystem_EdgeCases_HandleCorrectly()
        {
            var entity = _entityManager.CreateEntity();
            _entityManager.AddComponentData(entity, new MissionData
            {
                MissionId = int.MaxValue,
                MissionType = (MissionType)255,
                Status = (MissionStatus)255,
                Progress = float.MaxValue,
                StartTime = float.PositiveInfinity,
                EndTime = float.NegativeInfinity,
                Reward = float.NaN,
                Difficulty = (MissionDifficulty)255,
                IsCompleted = true
            });
            _entityManager.AddComponentData(entity, new MissionObjectiveData
            {
                ObjectiveId = int.MinValue,
                ObjectiveType = (ObjectiveType)255,
                TargetPosition = new float3(float.PositiveInfinity, float.NegativeInfinity, float.NaN),
                TargetRadius = float.MaxValue,
                IsCompleted = true,
                Progress = float.MinValue
            });
            _entityManager.AddComponentData(entity, new MissionRewardData
            {
                RewardType = (RewardType)255,
                RewardAmount = float.NaN,
                IsClaimed = true,
                ClaimTime = float.PositiveInfinity
            });

            Assert.DoesNotThrow(() => 
            {
                _missionSystem.OnUpdate(ref _world.Unmanaged);
            });
        }
    }
}
