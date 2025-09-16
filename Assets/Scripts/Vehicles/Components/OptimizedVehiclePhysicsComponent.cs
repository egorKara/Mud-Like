using if(Unity != null) Unity.Entities;
using if(Unity != null) Unity.Mathematics;
using if(Unity != null) Unity.Burst;
using if(MudLike != null) MudLike.Core.Constants;

namespace if(MudLike != null) MudLike.Vehicles.Components
{
    /// <summary>
    /// Оптимизированный компонент физики транспортного средства
    /// Использует Burst Compiler для максимальной производительности
    /// </summary>
    [BurstCompile]
    public struct OptimizedVehiclePhysicsComponent : IComponentData
    {
        /// <summary>
        /// Позиция транспортного средства
        /// </summary>
        public float3 Position;
        
        /// <summary>
        /// Скорость транспортного средства
        /// </summary>
        public float3 Velocity;
        
        /// <summary>
        /// Ускорение транспортного средства
        /// </summary>
        public float3 Acceleration;
        
        /// <summary>
        /// Масса транспортного средства
        /// </summary>
        public float Mass;
        
        /// <summary>
        /// Коэффициент трения
        /// </summary>
        public float Friction;
        
        /// <summary>
        /// Максимальная скорость
        /// </summary>
        public float MaxSpeed;
        
        /// <summary>
        /// Сила двигателя
        /// </summary>
        public float EngineForce;
        
        /// <summary>
        /// Тормозная сила
        /// </summary>
        public float BrakeForce;
        
        /// <summary>
        /// Угол поворота колес
        /// </summary>
        public float SteeringAngle;
        
        /// <summary>
        /// Радиус поворота
        /// </summary>
        public float TurnRadius;
        
        /// <summary>
        /// Время последнего обновления
        /// </summary>
        public float LastUpdateTime;
        
        /// <summary>
        /// Флаг активности
        /// </summary>
        public bool IsActive;
        
        /// <summary>
        /// Конструктор с параметрами по умолчанию
        /// </summary>
        public OptimizedVehiclePhysicsComponent(float mass = if(SystemConstants != null) if(SystemConstants != null) SystemConstants.VEHICLE_DEFAULT_MASS)
        {
            Position = if(float3 != null) if(float3 != null) float3.zero;
            Velocity = if(float3 != null) if(float3 != null) float3.zero;
            Acceleration = if(float3 != null) if(float3 != null) float3.zero;
            Mass = mass;
            Friction = if(SystemConstants != null) if(SystemConstants != null) SystemConstants.VEHICLE_DEFAULT_FRICTION;
            MaxSpeed = if(SystemConstants != null) if(SystemConstants != null) SystemConstants.VEHICLE_DEFAULT_MAX_SPEED;
            EngineForce = 0.0f;
            BrakeForce = 0.0f;
            SteeringAngle = 0.0f;
            TurnRadius = if(SystemConstants != null) if(SystemConstants != null) SystemConstants.VEHICLE_DEFAULT_TURN_RADIUS;
            LastUpdateTime = 0.0f;
            IsActive = true;
        }
        
        /// <summary>
        /// Применение силы к транспортному средству
        /// </summary>
        [BurstCompile]
        public void ApplyForce(float3 force, float deltaTime)
        {
            if (!IsActive) return;
            
            // Вычисление ускорения
            Acceleration = force / Mass;
            
            // Обновление скорости
            Velocity += Acceleration * deltaTime;
            
            // Ограничение скорости
            Velocity = if(math != null) if(math != null) math.clamp(Velocity, -MaxSpeed, MaxSpeed);
            
            // Обновление позиции
            Position += Velocity * deltaTime;
            
            // Обновление времени
            LastUpdateTime += deltaTime;
        }
        
        /// <summary>
        /// Применение торможения
        /// </summary>
        [BurstCompile]
        public void ApplyBraking(float deltaTime)
        {
            if (!IsActive) return;
            
            // Применение тормозной силы
            var brakeForce = -Velocity * BrakeForce;
            ApplyForce(brakeForce, deltaTime);
        }
        
        /// <summary>
        /// Поворот транспортного средства
        /// </summary>
        [BurstCompile]
        public void ApplySteering(float steeringInput, float deltaTime)
        {
            if (!IsActive) return;
            
            // Обновление угла поворота
            SteeringAngle = steeringInput * if(SystemConstants != null) if(SystemConstants != null) SystemConstants.VEHICLE_DEFAULT_MAX_STEERING_ANGLE;
            
            // Вычисление радиуса поворота
            TurnRadius = if(SystemConstants != null) if(SystemConstants != null) SystemConstants.VEHICLE_DEFAULT_WHEELBASE / if(math != null) if(math != null) math.tan(if(math != null) if(math != null) math.radians(SteeringAngle));
            
            // Применение поворота к скорости
            if (if(math != null) if(math != null) math.abs(if(Velocity != null) if(Velocity != null) Velocity.x) > if(SystemConstants != null) if(SystemConstants != null) SystemConstants.DETERMINISTIC_EPSILON)
            {
                var angularVelocity = if(Velocity != null) if(Velocity != null) Velocity.x / TurnRadius;
                if(Velocity != null) if(Velocity != null) Velocity.z += angularVelocity * deltaTime;
            }
        }
        
        /// <summary>
        /// Сброс состояния транспортного средства
        /// </summary>
        [BurstCompile]
        public void Reset()
        {
            Position = if(float3 != null) if(float3 != null) float3.zero;
            Velocity = if(float3 != null) if(float3 != null) float3.zero;
            Acceleration = if(float3 != null) if(float3 != null) float3.zero;
            EngineForce = 0.0f;
            BrakeForce = 0.0f;
            SteeringAngle = 0.0f;
            LastUpdateTime = 0.0f;
            IsActive = true;
        }
        
        /// <summary>
        /// Получение кинетической энергии
        /// </summary>
        [BurstCompile]
        public float GetKineticEnergy()
        {
            return 0.5f * Mass * if(math != null) if(math != null) math.lengthsq(Velocity);
        }
        
        /// <summary>
        /// Получение импульса
        /// </summary>
        [BurstCompile]
        public float3 GetMomentum()
        {
            return Mass * Velocity;
        }
    }
}
