using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;
using Unity.Jobs;
using MudLike.Audio.Components;
using MudLike.Vehicles.Components;

namespace MudLike.Audio.Systems
{
    /// <summary>
    /// Система звука двигателя
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [BurstCompile]
    public partial class EngineAudioSystem : SystemBase
    {
        private EntityQuery _engineAudioQuery;
        private EntityQuery _vehicleQuery;
        
        protected override void OnCreate()
        {
            _engineAudioQuery = GetEntityQuery(
                ComponentType.ReadWrite<EngineAudioData>()
            );
            
            _vehicleQuery = GetEntityQuery(
                ComponentType.ReadOnly<VehiclePhysics>(),
                ComponentType.ReadOnly<EngineData>(),
                ComponentType.ReadOnly<LocalTransform>()
            );
        }
        
        protected override void OnUpdate()
        {
            float deltaTime = Time.deltaTime;
            
            var engineAudioJob = new EngineAudioJob
            {
                DeltaTime = deltaTime
            };
            
            Dependency = engineAudioJob.ScheduleParallel(_engineAudioQuery, Dependency);
        }
        
        /// <summary>
        /// Job для обработки звука двигателя
        /// </summary>
        [BurstCompile]
        public partial struct EngineAudioJob : IJobEntity
        {
            public float DeltaTime;
            
            public void Execute(ref EngineAudioData audioData)
            {
                // Обновляем звук двигателя
                UpdateEngineAudio(ref audioData);
            }
            
            /// <summary>
            /// Обновляет звук двигателя
            /// </summary>
            private void UpdateEngineAudio(ref EngineAudioData audioData)
            {
                // Получаем данные двигателя
                var engineData = GetEngineData();
                if (!engineData.HasValue) return;
                
                // Обновляем параметры звука
                audioData.RPM = engineData.Value.RPM;
                audioData.Power = engineData.Value.Power;
                audioData.Temperature = engineData.Value.Temperature;
                
                // Вычисляем громкость на основе RPM
                audioData.Volume = CalculateEngineVolume(audioData.RPM);
                
                // Вычисляем высоту на основе RPM
                audioData.Pitch = CalculateEnginePitch(audioData.RPM);
                
                // Определяем, должен ли звук играть
                audioData.IsPlaying = audioData.RPM > 0.1f;
                
                audioData.NeedsUpdate = true;
            }
            
            /// <summary>
            /// Получает данные двигателя
            /// </summary>
            private EngineData? GetEngineData()
            {
                // TODO: Реализовать получение данных двигателя
                return new EngineData
                {
                    RPM = 1000f,
                    Power = 0.5f,
                    Temperature = 0.3f
                };
            }
            
            /// <summary>
            /// Вычисляет громкость двигателя
            /// </summary>
            private float CalculateEngineVolume(float rpm)
            {
                // Нормализуем RPM (0-6000)
                float normalizedRPM = math.clamp(rpm / 6000f, 0f, 1f);
                
                // Базовая громкость
                float baseVolume = 0.3f;
                
                // Увеличиваем громкость с RPM
                float rpmVolume = normalizedRPM * 0.7f;
                
                return math.clamp(baseVolume + rpmVolume, 0f, 1f);
            }
            
            /// <summary>
            /// Вычисляет высоту звука двигателя
            /// </summary>
            private float CalculateEnginePitch(float rpm)
            {
                // Нормализуем RPM (0-6000)
                float normalizedRPM = math.clamp(rpm / 6000f, 0f, 1f);
                
                // Базовая высота
                float basePitch = 0.5f;
                
                // Увеличиваем высоту с RPM
                float rpmPitch = normalizedRPM * 0.5f;
                
                return math.clamp(basePitch + rpmPitch, 0.1f, 2f);
            }
        }
        
        /// <summary>
        /// Данные двигателя для аудио
        /// </summary>
        private struct EngineData
        {
            public float RPM;
            public float Power;
            public float Temperature;
        }
    }
}