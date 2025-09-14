using Unity.Entities;
using Unity.Mathematics;
using System;

namespace MudLike.Core.Components
{
    /// <summary>
    /// Базовый компонент для событий
    /// </summary>
    public struct EventData : IComponentData
    {
        /// <summary>
        /// Тип события
        /// </summary>
        public EventType Type;
        
        /// <summary>
        /// Источник события
        /// </summary>
        public Entity Source;
        
        /// <summary>
        /// Цель события
        /// </summary>
        public Entity Target;
        
        /// <summary>
        /// Позиция события
        /// </summary>
        public float3 Position;
        
        /// <summary>
        /// Значение события
        /// </summary>
        public float Value;
        
        /// <summary>
        /// Время события
        /// </summary>
        public float Time;
        
        /// <summary>
        /// Приоритет события
        /// </summary>
        public int Priority;
        
        /// <summary>
        /// Обработано ли событие
        /// </summary>
        public bool IsProcessed;
    }
    
    /// <summary>
    /// Типы событий
    /// </summary>
    public enum EventType : byte
    {
        // События транспорта
        VehicleSpawned,
        VehicleDestroyed,
        VehicleDamaged,
        VehicleRepaired,
        VehicleFuelEmpty,
        VehicleEngineStarted,
        VehicleEngineStopped,
        VehicleGearChanged,
        VehicleSpeedChanged,
        VehicleCollision,
        VehicleExplosion,
        VehicleFire,
        VehicleSmoke,
        VehicleSteam,
        VehicleSparks,
        VehicleDebris,
        VehicleWreckage,
        VehicleSalvage,
        VehicleScrap,
        VehicleParts,
        
        // События колес
        WheelTireBurst,
        WheelTireFlat,
        WheelTireWorn,
        WheelTireReplaced,
        WheelSuspensionBroken,
        WheelSuspensionRepaired,
        WheelBrakeWorn,
        WheelBrakeReplaced,
        WheelSteeringBroken,
        WheelSteeringRepaired,
        
        // События поверхности
        SurfaceChanged,
        SurfaceWet,
        SurfaceDry,
        SurfaceFrozen,
        SurfaceMelted,
        SurfaceMuddy,
        SurfaceClean,
        SurfaceRough,
        SurfaceSmooth,
        SurfaceSlippery,
        SurfaceSticky,
        SurfaceSoft,
        SurfaceHard,
        SurfaceElastic,
        SurfaceRigid,
        SurfaceFlexible,
        SurfaceStiff,
        SurfaceDense,
        SurfaceLoose,
        SurfaceCompact,
        
        // События погоды
        WeatherChanged,
        RainStarted,
        RainStopped,
        SnowStarted,
        SnowStopped,
        FogStarted,
        FogCleared,
        WindStarted,
        WindStopped,
        StormStarted,
        StormEnded,
        TemperatureChanged,
        HumidityChanged,
        PressureChanged,
        VisibilityChanged,
        
        // События игрока
        PlayerJoined,
        PlayerLeft,
        PlayerDied,
        PlayerRespawned,
        PlayerLevelUp,
        PlayerExperienceGained,
        PlayerMoneyEarned,
        PlayerMoneySpent,
        PlayerItemPickedUp,
        PlayerItemDropped,
        PlayerItemUsed,
        PlayerItemCrafted,
        PlayerItemRepaired,
        PlayerItemUpgraded,
        PlayerItemSold,
        PlayerItemBought,
        
        // События миссий
        MissionStarted,
        MissionCompleted,
        MissionFailed,
        MissionAbandoned,
        MissionPaused,
        MissionResumed,
        MissionRestarted,
        MissionUpdated,
        MissionObjectiveCompleted,
        MissionObjectiveFailed,
        MissionRewardEarned,
        MissionBonusEarned,
        MissionPenaltyApplied,
        
        // События груза
        CargoLoaded,
        CargoUnloaded,
        CargoDamaged,
        CargoLost,
        CargoDelivered,
        CargoStolen,
        CargoFound,
        CargoHidden,
        CargoDiscovered,
        CargoRevealed,
        CargoConcealed,
        CargoExposed,
        CargoProtected,
        CargoSecured,
        CargoReleased,
        CargoFreed,
        CargoCaptured,
        CargoEscaped,
        CargoRescued,
        CargoAbandoned,
        
        // События лебедки
        WinchAttached,
        WinchDetached,
        WinchPulling,
        WinchReleasing,
        WinchBroken,
        WinchRepaired,
        WinchOverloaded,
        WinchStuck,
        WinchFreed,
        WinchTensionChanged,
        WinchLengthChanged,
        WinchAngleChanged,
        WinchSpeedChanged,
        WinchForceChanged,
        WinchPowerChanged,
        WinchEfficiencyChanged,
        WinchWearChanged,
        WinchTemperatureChanged,
        WinchLubricationChanged,
        WinchMaintenanceRequired,
        
        // События системы
        SystemStarted,
        SystemStopped,
        SystemPaused,
        SystemResumed,
        SystemRestarted,
        SystemUpdated,
        SystemError,
        SystemWarning,
        SystemInfo,
        SystemDebug,
        SystemTrace,
        SystemLog,
        SystemException,
        SystemCrash,
        SystemRecovery,
        SystemBackup,
        SystemRestore,
        SystemReset,
        SystemShutdown,
        SystemStartup
    }
    
    /// <summary>
    /// Обертка для EventType, реализующая IEquatable для использования в NativeHashMap
    /// </summary>
    public struct EventTypeKey : IEquatable<EventTypeKey>
    {
        public EventType Value;
        
        public EventTypeKey(EventType value)
        {
            Value = value;
        }
        
        public bool Equals(EventTypeKey other)
        {
            return Value == other.Value;
        }
        
        public override bool Equals(object obj)
        {
            return obj is EventTypeKey other && Equals(other);
        }
        
        public override int GetHashCode()
        {
            return ((int)Value).GetHashCode();
        }
        
        public static implicit operator EventTypeKey(EventType value)
        {
            return new EventTypeKey(value);
        }
        
        public static implicit operator EventType(EventTypeKey key)
        {
            return key.Value;
        }
    }
}