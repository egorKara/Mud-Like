using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;
using Unity.Jobs;
using Unity.Collections;
using MudLike.Vehicles.Components;

namespace MudLike.Vehicles.Systems
{
    /// <summary>
    /// Система управления миссиями
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [BurstCompile]
    public partial class MissionSystem : SystemBase
    {
        private EntityQuery _missionQuery;
        private EntityQuery _objectiveQuery;
        private EntityQuery _rewardQuery;
        private EntityQuery _vehicleQuery;
        
        protected override void OnCreate()
        {
            _missionQuery = GetEntityQuery(
                ComponentType.ReadWrite<MissionData>()
            );
            
            _objectiveQuery = GetEntityQuery(
                ComponentType.ReadWrite<MissionObjectiveData>()
            );
            
            _rewardQuery = GetEntityQuery(
                ComponentType.ReadWrite<MissionRewardData>()
            );
            
            _vehicleQuery = GetEntityQuery(
                ComponentType.ReadOnly<VehiclePhysics>(),
                ComponentType.ReadOnly<LocalTransform>()
            );
        }
        
        protected override void OnUpdate()
        {
            float deltaTime = Time.deltaTime;
            
            // Обновляем миссии
            UpdateMissions(deltaTime);
            
            // Обновляем цели
            UpdateObjectives(deltaTime);
            
            // Обновляем награды
            UpdateRewards(deltaTime);
        }
        
        /// <summary>
        /// Обновляет миссии
        /// </summary>
        private void UpdateMissions(float deltaTime)
        {
            var missionJob = new MissionJob
            {
                DeltaTime = deltaTime
            };
            
            Dependency = missionJob.ScheduleParallel(_missionQuery, Dependency);
        }
        
        /// <summary>
        /// Обновляет цели
        /// </summary>
        private void UpdateObjectives(float deltaTime)
        {
            var objectiveJob = new ObjectiveJob
            {
                DeltaTime = deltaTime
            };
            
            Dependency = objectiveJob.ScheduleParallel(_objectiveQuery, Dependency);
        }
        
        /// <summary>
        /// Обновляет награды
        /// </summary>
        private void UpdateRewards(float deltaTime)
        {
            var rewardJob = new RewardJob
            {
                DeltaTime = deltaTime
            };
            
            Dependency = rewardJob.ScheduleParallel(_rewardQuery, Dependency);
        }
        
        /// <summary>
        /// Job для обработки миссий
        /// </summary>
        [BurstCompile]
        public partial struct MissionJob : IJobEntity
        {
            public float DeltaTime;
            
            public void Execute(ref MissionData missionData)
            {
                // Обновляем миссию
                UpdateMission(ref missionData);
            }
            
            /// <summary>
            /// Обновляет миссию
            /// </summary>
            private void UpdateMission(ref MissionData missionData)
            {
                if (!missionData.IsActive || missionData.IsCompleted || missionData.IsFailed) return;
                
                // Обновляем оставшееся время
                missionData.RemainingTime -= DeltaTime;
                missionData.RemainingTime = math.max(missionData.RemainingTime, 0f);
                
                // Проверяем, не истекло ли время
                if (missionData.RemainingTime <= 0f)
                {
                    missionData.IsFailed = true;
                    missionData.Status = MissionStatus.Failed;
                    missionData.IsActive = false;
                }
                
                // Обновляем прогресс
                UpdateProgress(ref missionData);
                
                // Проверяем выполнение миссии
                CheckMissionCompletion(ref missionData);
                
                missionData.NeedsUpdate = true;
            }
            
            /// <summary>
            /// Обновляет прогресс миссии
            /// </summary>
            [BurstCompile]
            private void UpdateProgress(ref MissionData missionData)
            {
                // Проверяем каждую цель миссии
                for (int i = 0; i < missionData.Objectives.Length; i++)
                {
                    var objective = missionData.Objectives[i];
                    
                    if (!objective.IsCompleted)
                    {
                        // Проверяем выполнение цели на основе типа
                        switch (objective.Type)
                        {
                            case ObjectiveType.ReachLocation:
                                UpdateLocationObjective(ref objective, missionData.PlayerPosition);
                                break;
                            case ObjectiveType.CollectItems:
                                UpdateCollectObjective(ref objective, missionData.CollectedItems);
                                break;
                            case ObjectiveType.DeliverCargo:
                                UpdateDeliveryObjective(ref objective, missionData.DeliveredCargo);
                                break;
                        }
                        
                        missionData.Objectives[i] = objective;
                    }
                }
                
                // Пересчитываем общий прогресс
                missionData.Progress = CalculateMissionProgress(missionData.Objectives);
            }
            
            /// <summary>
            /// Проверяет выполнение миссии
            /// </summary>
            [BurstCompile]
            private void CheckMissionCompletion(ref MissionData missionData)
            {
                // Проверяем, все ли цели выполнены
                bool allObjectivesCompleted = true;
                for (int i = 0; i < missionData.Objectives.Length; i++)
                {
                    if (!missionData.Objectives[i].IsCompleted)
                    {
                        allObjectivesCompleted = false;
                        break;
                    }
                }
                
                // Проверяем дополнительные условия миссии
                bool additionalConditionsMet = CheckAdditionalConditions(missionData);
                
                // Миссия завершена, если все цели выполнены и дополнительные условия соблюдены
                if (allObjectivesCompleted && additionalConditionsMet && !missionData.IsCompleted)
                {
                    missionData.IsCompleted = true;
                    missionData.CompletionTime = SystemAPI.Time.time;
                    
                    // Выдаем награду за выполнение миссии
                    AwardMissionCompletion(missionData);
                }
            }
        }
        
        /// <summary>
        /// Job для обработки целей
        /// </summary>
        [BurstCompile]
        public partial struct ObjectiveJob : IJobEntity
        {
            public float DeltaTime;
            
            public void Execute(ref MissionObjectiveData objectiveData)
            {
                // Обновляем цель
                UpdateObjective(ref objectiveData);
            }
            
            /// <summary>
            /// Обновляет цель
            /// </summary>
            private void UpdateObjective(ref MissionObjectiveData objectiveData)
            {
                if (!objectiveData.IsActive || objectiveData.IsCompleted) return;
                
                // Проверяем выполнение цели
                CheckObjectiveCompletion(ref objectiveData);
                
                objectiveData.NeedsUpdate = true;
            }
            
            /// <summary>
            /// Проверяет выполнение цели
            /// </summary>
            private void CheckObjectiveCompletion(ref MissionObjectiveData objectiveData)
            {
                // Проверяем выполнение цели
                CheckObjectiveCompletion(ref objective, missionData);
                // Например, проверка расстояния до цели, количества собранных предметов
            }
        }
        
        /// <summary>
        /// Job для обработки наград
        /// </summary>
        [BurstCompile]
        public partial struct RewardJob : IJobEntity
        {
            public float DeltaTime;
            
            public void Execute(ref MissionRewardData rewardData)
            {
                // Обновляем награду
                UpdateReward(ref rewardData);
            }
            
            /// <summary>
            /// Обновляет награду
            /// </summary>
            private void UpdateReward(ref MissionRewardData rewardData)
            {
                if (rewardData.IsRewarded) return;
                
                // Выдаем награду за выполнение цели
                AwardObjectiveCompletion(ref objective, missionData);
                // Например, добавление денег, опыта, предметов
                
                rewardData.NeedsUpdate = true;
            }
        }
    }
}