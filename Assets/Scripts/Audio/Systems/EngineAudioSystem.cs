using Unity.Entities;
using Unity.Mathematics;
using Unity.Burst;
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
        private EntityQuery _audioQuery;
        private EntityQuery _engineQuery;
        
        protected override void OnCreate()
        {
            _audioQuery = GetEntityQuery(
                ComponentType.ReadWrite<EngineAudioData>()
            );
            
            _engineQuery = GetEntityQuery(
                ComponentType.ReadOnly<EngineData>(),
                ComponentType.ReadOnly<VehiclePhysics>()
            );
        }
        
        protected override void OnUpdate()
        {
            float deltaTime = SystemAPI.Time.DeltaTime;
            
            // Обновляем звук двигателя
            UpdateEngineAudio(deltaTime);
        }
        
        private void UpdateEngineAudio(float deltaTime)
        {
            // Обновляем звук для каждого двигателя
            Entities
                .WithAll<EngineAudioData>()
                .ForEach((ref EngineAudioData audio) =>
                {
                    UpdateAudioData(ref audio, deltaTime);
                }).Schedule();
        }
        
        private void UpdateAudioData(ref EngineAudioData audio, float deltaTime)
        {
            // Простая реализация звука двигателя
            // В реальной реализации здесь будет сложная аудио система
            
            // Обновляем громкость на основе RPM
            audio.Volume = math.clamp(audio.RPM / 6000f, 0f, 1f);
            
            // Обновляем высоту звука на основе RPM
            audio.Pitch = math.clamp(audio.RPM / 3000f, 0.5f, 2f);
            
            // Определяем, должен ли звук воспроизводиться
            audio.IsPlaying = audio.RPM > 100f;
            
            // Обновляем статус
            audio.NeedsUpdate = false;
        }
    }
}