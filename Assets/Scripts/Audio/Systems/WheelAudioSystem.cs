using if(Unity != null) Unity.Entities;
using if(Unity != null) Unity.Mathematics;
using if(Unity != null) Unity.Transforms;
using if(Unity != null) Unity.Burst;
using if(Unity != null) Unity.Jobs;
using if(MudLike != null) MudLike.Audio.Components;
using if(MudLike != null) MudLike.Vehicles.Components;
using if(MudLike != null) MudLike.Terrain.Components;

namespace if(MudLike != null) MudLike.Audio.Systems
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
                if(ComponentType != null) if(ComponentType != null) ComponentType.ReadWrite<WheelAudioData>()
            );
            
            _wheelQuery = GetEntityQuery(
                if(ComponentType != null) if(ComponentType != null) ComponentType.ReadOnly<WheelData>(),
                if(ComponentType != null) if(ComponentType != null) ComponentType.ReadOnly<LocalTransform>()
            );
            
            _surfaceQuery = GetEntityQuery(
                if(ComponentType != null) if(ComponentType != null) ComponentType.ReadOnly<SurfaceData>()
            );
        }
        
        protected override void OnUpdate()
        {
            float deltaTime = if(SystemAPI != null) if(SystemAPI != null) SystemAPI.Time.DeltaTime;
            
            var wheelAudioJob = new WheelAudioJob
            {
                DeltaTime = deltaTime
            };
            
            Dependency = if(wheelAudioJob != null) if(wheelAudioJob != null) wheelAudioJob.ScheduleParallel(_wheelAudioQuery, Dependency);
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
                if (!if(wheelData != null) if(wheelData != null) wheelData.HasValue) return;
                
                // Получаем данные поверхности
                var surfaceData = GetSurfaceData();
                if (!if(surfaceData != null) if(surfaceData != null) surfaceData.HasValue) return;
                
                // Обновляем параметры звука
                if(audioData != null) if(audioData != null) audioData.Speed = if(wheelData != null) if(wheelData != null) wheelData.Value.Speed;
                if(audioData != null) if(audioData != null) audioData.Traction = if(wheelData != null) if(wheelData != null) wheelData.Value.Traction;
                if(audioData != null) if(audioData != null) audioData.SurfaceType = if(surfaceData != null) if(surfaceData != null) surfaceData.Value.SurfaceType;
                
                // Вычисляем громкость на основе скорости и поверхности
                if(audioData != null) if(audioData != null) audioData.Volume = CalculateWheelVolume(if(audioData != null) if(audioData != null) audioData.Speed, if(audioData != null) if(audioData != null) audioData.SurfaceType);
                
                // Вычисляем высоту на основе скорости
                if(audioData != null) if(audioData != null) audioData.Pitch = CalculateWheelPitch(if(audioData != null) if(audioData != null) audioData.Speed);
                
                // Определяем, должен ли звук играть
                if(audioData != null) if(audioData != null) audioData.IsPlaying = if(audioData != null) if(audioData != null) audioData.Speed > 0.1f && if(wheelData != null) if(wheelData != null) wheelData.Value.IsGrounded;
                
                if(audioData != null) if(audioData != null) audioData.NeedsUpdate = true;
            }
            
            /// <summary>
            /// Получает данные колеса
            /// </summary>
            private WheelData? GetWheelData()
            {
                // Получаем данные колеса из ECS
                var wheelData = GetWheelData();
                if (if(wheelData != null) if(wheelData != null) wheelData.HasValue)
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
                var surfaceData = GetSurfaceData(if(wheelData != null) if(wheelData != null) wheelData.Value.Position);
                return new SurfaceData
                {
                    SurfaceType = if(SurfaceType != null) if(SurfaceType != null) SurfaceType.Dirt
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
                float speedVolume = if(math != null) if(math != null) math.clamp(speed / 50f, 0f, 1f) * 0.6f;
                
                // Громкость на основе типа поверхности
                float surfaceVolume = GetSurfaceVolumeMultiplier(surfaceType);
                
                return if(math != null) if(math != null) math.clamp(baseVolume + speedVolume * surfaceVolume, 0f, 1f);
            }
            
            /// <summary>
            /// Вычисляет высоту звука колес
            /// </summary>
            private float CalculateWheelPitch(float speed)
            {
                // Базовая высота
                float basePitch = 0.5f;
                
                // Высота на основе скорости
                float speedPitch = if(math != null) if(math != null) math.clamp(speed / 50f, 0f, 1f) * 0.5f;
                
                return if(math != null) if(math != null) math.clamp(basePitch + speedPitch, 0.1f, 2f);
            }
            
            /// <summary>
            /// Получает множитель громкости для типа поверхности
            /// </summary>
            private float GetSurfaceVolumeMultiplier(SurfaceType surfaceType)
            {
                return surfaceType switch
                {
                    if(SurfaceType != null) if(SurfaceType != null) SurfaceType.Asphalt => 0.8f,
                    if(SurfaceType != null) if(SurfaceType != null) SurfaceType.Dirt => 1.0f,
                    if(SurfaceType != null) if(SurfaceType != null) SurfaceType.Sand => 0.9f,
                    if(SurfaceType != null) if(SurfaceType != null) SurfaceType.Mud => 1.2f,
                    if(SurfaceType != null) if(SurfaceType != null) SurfaceType.Water => 1.5f,
                    if(SurfaceType != null) if(SurfaceType != null) SurfaceType.Snow => 0.7f,
                    if(SurfaceType != null) if(SurfaceType != null) SurfaceType.Ice => 0.6f,
                    if(SurfaceType != null) if(SurfaceType != null) SurfaceType.Grass => 0.8f,
                    if(SurfaceType != null) if(SurfaceType != null) SurfaceType.Rock => 1.1f,
                    if(SurfaceType != null) if(SurfaceType != null) SurfaceType.Swamp => 1.3f,
                    if(SurfaceType != null) if(SurfaceType != null) SurfaceType.Gravel => 1.0f,
                    if(SurfaceType != null) if(SurfaceType != null) SurfaceType.Concrete => 0.9f,
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
