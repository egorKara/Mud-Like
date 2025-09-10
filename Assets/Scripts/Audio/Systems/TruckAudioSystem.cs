using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using MudLike.Audio.Components;
using MudLike.Vehicles.Components;

namespace MudLike.Audio.Systems
{
    /// <summary>
    /// Система звуков грузовика
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class TruckAudioSystem : SystemBase
    {
        /// <summary>
        /// ID звуков
        /// </summary>
        private const int ENGINE_SOUND_ID = 0;
        private const int WHEEL_SOUND_ID = 1;
        private const int MUD_SOUND_ID = 2;
        private const int BRAKE_SOUND_ID = 3;
        private const int GEAR_SOUND_ID = 4;
        
        /// <summary>
        /// Обрабатывает звуки грузовика
        /// </summary>
        protected override void OnUpdate()
        {
            float deltaTime = SystemAPI.Time.DeltaTime;
            
            // Обрабатываем звуки для всех грузовиков
            Entities
                .WithAll<TruckData, AudioSourceData>()
                .ForEach((ref AudioSourceData audio, in TruckData truck, in LocalTransform transform) =>
                {
                    UpdateTruckAudio(ref audio, truck, transform, deltaTime);
                }).WithoutBurst().Run();
        }
        
        /// <summary>
        /// Обновляет звуки грузовика
        /// </summary>
        private static void UpdateTruckAudio(ref AudioSourceData audio, in TruckData truck, in LocalTransform transform, float deltaTime)
        {
            // Звук двигателя
            if (truck.EngineRunning)
            {
                UpdateEngineSound(ref audio, truck, deltaTime);
            }
            else
            {
                StopEngineSound(ref audio);
            }
            
            // Звук колес
            UpdateWheelSound(ref audio, truck, deltaTime);
            
            // Звук грязи
            if (truck.CurrentSpeed > 0.1f)
            {
                UpdateMudSound(ref audio, truck, deltaTime);
            }
            else
            {
                StopMudSound(ref audio);
            }
            
            // Звук торможения
            if (truck.HandbrakeOn)
            {
                UpdateBrakeSound(ref audio, truck, deltaTime);
            }
            else
            {
                StopBrakeSound(ref audio);
            }
        }
        
        /// <summary>
        /// Обновляет звук двигателя
        /// </summary>
        private static void UpdateEngineSound(ref AudioSourceData audio, in TruckData truck, float deltaTime)
        {
            if (audio.SoundID != ENGINE_SOUND_ID)
            {
                audio.SoundID = ENGINE_SOUND_ID;
                audio.IsPlaying = true;
                audio.IsLooping = true;
                audio.Volume = 0.8f;
            }
            
            // Вычисляем высоту тона на основе оборотов двигателя
            float rpmFactor = truck.EngineRPM / 2500f; // Нормализуем к 0-1
            audio.Pitch = math.lerp(0.5f, 2f, rpmFactor);
            
            // Вычисляем громкость на основе газа
            float throttleFactor = truck.CurrentSpeed / truck.MaxSpeed;
            audio.Volume = math.lerp(0.3f, 1f, throttleFactor);
        }
        
        /// <summary>
        /// Останавливает звук двигателя
        /// </summary>
        private static void StopEngineSound(ref AudioSourceData audio)
        {
            if (audio.SoundID == ENGINE_SOUND_ID)
            {
                audio.IsPlaying = false;
                audio.Volume = 0f;
            }
        }
        
        /// <summary>
        /// Обновляет звук колес
        /// </summary>
        private static void UpdateWheelSound(ref AudioSourceData audio, in TruckData truck, float deltaTime)
        {
            if (truck.CurrentSpeed > 0.1f)
            {
                if (audio.SoundID != WHEEL_SOUND_ID)
                {
                    audio.SoundID = WHEEL_SOUND_ID;
                    audio.IsPlaying = true;
                    audio.IsLooping = true;
                    audio.Volume = 0.6f;
                }
                
                // Вычисляем высоту тона на основе скорости
                float speedFactor = truck.CurrentSpeed / truck.MaxSpeed;
                audio.Pitch = math.lerp(0.8f, 1.5f, speedFactor);
                
                // Вычисляем громкость на основе скорости
                audio.Volume = math.lerp(0.2f, 0.8f, speedFactor);
            }
            else
            {
                if (audio.SoundID == WHEEL_SOUND_ID)
                {
                    audio.IsPlaying = false;
                    audio.Volume = 0f;
                }
            }
        }
        
        /// <summary>
        /// Обновляет звук грязи
        /// </summary>
        private static void UpdateMudSound(ref AudioSourceData audio, in TruckData truck, float deltaTime)
        {
            // Проверяем, есть ли грязь (упрощенно)
            bool inMud = truck.TractionCoefficient < 0.6f;
            
            if (inMud)
            {
                if (audio.SoundID != MUD_SOUND_ID)
                {
                    audio.SoundID = MUD_SOUND_ID;
                    audio.IsPlaying = true;
                    audio.IsLooping = true;
                    audio.Volume = 0.7f;
                }
                
                // Вычисляем громкость на основе сцепления
                float mudFactor = 1f - truck.TractionCoefficient;
                audio.Volume = math.lerp(0.3f, 1f, mudFactor);
                
                // Вычисляем высоту тона на основе скорости
                float speedFactor = truck.CurrentSpeed / truck.MaxSpeed;
                audio.Pitch = math.lerp(0.7f, 1.3f, speedFactor);
            }
            else
            {
                if (audio.SoundID == MUD_SOUND_ID)
                {
                    audio.IsPlaying = false;
                    audio.Volume = 0f;
                }
            }
        }
        
        /// <summary>
        /// Останавливает звук грязи
        /// </summary>
        private static void StopMudSound(ref AudioSourceData audio)
        {
            if (audio.SoundID == MUD_SOUND_ID)
            {
                audio.IsPlaying = false;
                audio.Volume = 0f;
            }
        }
        
        /// <summary>
        /// Обновляет звук торможения
        /// </summary>
        private static void UpdateBrakeSound(ref AudioSourceData audio, in TruckData truck, float deltaTime)
        {
            if (audio.SoundID != BRAKE_SOUND_ID)
            {
                audio.SoundID = BRAKE_SOUND_ID;
                audio.IsPlaying = true;
                audio.IsLooping = true;
                audio.Volume = 0.9f;
            }
            
            // Вычисляем громкость на основе скорости торможения
            float brakeFactor = truck.CurrentSpeed / truck.MaxSpeed;
            audio.Volume = math.lerp(0.5f, 1f, brakeFactor);
        }
        
        /// <summary>
        /// Останавливает звук торможения
        /// </summary>
        private static void StopBrakeSound(ref AudioSourceData audio)
        {
            if (audio.SoundID == BRAKE_SOUND_ID)
            {
                audio.IsPlaying = false;
                audio.Volume = 0f;
            }
        }
    }
}