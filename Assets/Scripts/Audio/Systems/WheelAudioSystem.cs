using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;
using Unity.Jobs;
using MudLike.Audio.Components;
using MudLike.Vehicles.Components;
using MudLike.Terrain.Components;

namespace MudLike.Audio.Systems
{
    /// <summary>
    /// Система звука колес
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [BurstCompile]
    public partial class WheelAudioSystem : SystemBase
    {
        private EntityQuery _wheelAudioQuery;
        private EntityQuery _wheelQuery;
        private EntityQuery _surfaceQuery;
        
        protected override void OnCreate()
        {
            _wheelAudioQuery = GetEntityQuery(
                ComponentType.ReadWrite<WheelAudioData>()
            );
            
            _wheelQuery = GetEntityQuery(
                ComponentType.ReadOnly<WheelData>(),
                ComponentType.ReadOnly<LocalTransform>()
            );
            
            _surfaceQuery = GetEntityQuery(
                ComponentType.ReadOnly<SurfaceData>()
            );
        }
        
        protected override void OnUpdate()
        {
            float deltaTime = Time.deltaTime;
            
            var wheelAudioJob = new WheelAudioJob
            {
                DeltaTime = deltaTime
            };
            
            Dependency = wheelAudioJob.ScheduleParallel(_wheelAudioQuery, Dependency);
        }
        
        /// <summary>
        /// Job для обработки звука колес
        /// </summary>
        [BurstCompile]
        public partial struct WheelAudioJob : IJobEntity
        {
            public float DeltaTime;
            
            public void Execute(ref WheelAudioData audioData)
            {
                // Обновляем звук колес
                UpdateWheelAudio(ref audioData);
            }
            
            /// <summary>
            /// Обновляет звук колес
            /// </summary>
            private void UpdateWheelAudio(ref WheelAudioData audioData)
            {
                // Получаем данные колеса
                var wheelData = GetWheelData();
                if (!wheelData.HasValue) return;
                
                // Получаем данные поверхности
                var surfaceData = GetSurfaceData();
                if (!surfaceData.HasValue) return;
                
                // Обновляем параметры звука
                audioData.Speed = wheelData.Value.Speed;
                audioData.Traction = wheelData.Value.Traction;
                audioData.SurfaceType = surfaceData.Value.SurfaceType;
                
                // Вычисляем громкость на основе скорости и поверхности
                audioData.Volume = CalculateWheelVolume(audioData.Speed, audioData.SurfaceType);
                
                // Вычисляем высоту на основе скорости
                audioData.Pitch = CalculateWheelPitch(audioData.Speed);
                
                // Определяем, должен ли звук играть
                audioData.IsPlaying = audioData.Speed > 0.1f && wheelData.Value.IsGrounded;
                
                audioData.NeedsUpdate = true;
            }
            
            /// <summary>
            /// Получает данные колеса
            /// </summary>
            private WheelData? GetWheelData()
            {
                // Получаем данные колеса из ECS
                var wheelData = GetWheelData();
                if (wheelData.HasValue)
                return new WheelData
                {
                    Speed = 10f,
                    Traction = 0.8f,
                    IsGrounded = true
                };
            }
            
            /// <summary>
            /// Получает данные поверхности
            /// </summary>
            private SurfaceData? GetSurfaceData()
            {
                // Получаем данные поверхности из террейна
                var surfaceData = GetSurfaceData(wheelData.Value.Position);
                return new SurfaceData
                {
                    SurfaceType = SurfaceType.Dirt
                };
            }
            
            /// <summary>
            /// Вычисляет громкость колес
            /// </summary>
            private float CalculateWheelVolume(float speed, SurfaceType surfaceType)
            {
                // Базовая громкость
                float baseVolume = 0.2f;
                
                // Громкость на основе скорости
                float speedVolume = math.clamp(speed / 50f, 0f, 1f) * 0.6f;
                
                // Громкость на основе типа поверхности
                float surfaceVolume = GetSurfaceVolumeMultiplier(surfaceType);
                
                return math.clamp(baseVolume + speedVolume * surfaceVolume, 0f, 1f);
            }
            
            /// <summary>
            /// Вычисляет высоту звука колес
            /// </summary>
            private float CalculateWheelPitch(float speed)
            {
                // Базовая высота
                float basePitch = 0.5f;
                
                // Высота на основе скорости
                float speedPitch = math.clamp(speed / 50f, 0f, 1f) * 0.5f;
                
                return math.clamp(basePitch + speedPitch, 0.1f, 2f);
            }
            
            /// <summary>
            /// Получает множитель громкости для типа поверхности
            /// </summary>
            private float GetSurfaceVolumeMultiplier(SurfaceType surfaceType)
            {
                return surfaceType switch
                {
                    SurfaceType.Asphalt => 0.8f,
                    SurfaceType.Dirt => 1.0f,
                    SurfaceType.Sand => 0.9f,
                    SurfaceType.Mud => 1.2f,
                    SurfaceType.Water => 1.5f,
                    SurfaceType.Snow => 0.7f,
                    SurfaceType.Ice => 0.6f,
                    SurfaceType.Grass => 0.8f,
                    SurfaceType.Rock => 1.1f,
                    SurfaceType.Swamp => 1.3f,
                    SurfaceType.Gravel => 1.0f,
                    SurfaceType.Concrete => 0.9f,
                    _ => 1.0f
                };
            }
        }
        
        /// <summary>
        /// Данные колеса для аудио
        /// </summary>
        private struct WheelData
        {
            public float Speed;
            public float Traction;
            public bool IsGrounded;
        }
        
        /// <summary>
        /// Данные поверхности для аудио
        /// </summary>
        private struct SurfaceData
        {
            public SurfaceType SurfaceType;
        }
    }
}