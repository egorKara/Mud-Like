using Unity.Entities;
using Unity.Mathematics;

namespace MudLike.Vehicles.Components
{
    /// <summary>
    /// ECS компонент для транспортного средства
    /// </summary>
    public struct VehicleComponent : IComponentData
    {
        /// <summary>
        /// Масса транспортного средства (кг)
        /// </summary>
        public float mass;
        
        /// <summary>
        /// Момент инерции относительно центра масс
        /// </summary>
        public float3 inertia;
        
        /// <summary>
        /// Центр масс относительно трансформа
        /// </summary>
        public float3 centerOfMass;
        
        /// <summary>
        /// Скорость транспортного средства (м/с)
        /// </summary>
        public float3 velocity;
        
        /// <summary>
        /// Угловая скорость (рад/с)
        /// </summary>
        public float3 angularVelocity;
        
        /// <summary>
        /// Ускорение (м/с²)
        /// </summary>
        public float3 acceleration;
        
        /// <summary>
        /// Угловое ускорение (рад/с²)
        /// </summary>
        public float3 angularAcceleration;
        
        /// <summary>
        /// Мощность двигателя (Вт)
        /// </summary>
        public float enginePower;
        
        /// <summary>
        /// Максимальная мощность двигателя (Вт)
        /// </summary>
        public float maxEnginePower;
        
        /// <summary>
        /// Крутящий момент двигателя (Н·м)
        /// </summary>
        public float engineTorque;
        
        /// <summary>
        /// Максимальный крутящий момент (Н·м)
        /// </summary>
        public float maxEngineTorque;
        
        /// <summary>
        /// Обороты двигателя (об/мин)
        /// </summary>
        public float engineRPM;
        
        /// <summary>
        /// Максимальные обороты двигателя (об/мин)
        /// </summary>
        public float maxEngineRPM;
        
        /// <summary>
        /// Передача (1-6)
        /// </summary>
        public int gear;
        
        /// <summary>
        /// Количество передач
        /// </summary>
        public int gearCount;
        
        /// <summary>
        /// Передаточное отношение текущей передачи
        /// </summary>
        public float gearRatio;
        
        /// <summary>
        /// Передаточное отношение главной передачи
        /// </summary>
        public float finalDriveRatio;
        
        /// <summary>
        /// Коэффициент дифференциала
        /// </summary>
        public float differentialRatio;
        
        /// <summary>
        /// Коэффициент трения качения
        /// </summary>
        public float rollingResistance;
        
        /// <summary>
        /// Коэффициент аэродинамического сопротивления
        /// </summary>
        public float dragCoefficient;
        
        /// <summary>
        /// Площадь лобового сопротивления (м²)
        /// </summary>
        public float frontalArea;
        
        /// <summary>
        /// Плотность воздуха (кг/м³)
        /// </summary>
        public float airDensity;
        
        /// <summary>
        /// Сила аэродинамического сопротивления
        /// </summary>
        public float3 dragForce;
        
        /// <summary>
        /// Сила трения качения
        /// </summary>
        public float3 rollingResistanceForce;
        
        /// <summary>
        /// Общая сила тяги
        /// </summary>
        public float3 totalTractionForce;
        
        /// <summary>
        /// Общая сила торможения
        /// </summary>
        public float3 totalBrakeForce;
        
        /// <summary>
        /// Флаг: включен ли двигатель
        /// </summary>
        public bool engineOn;
        
        /// <summary>
        /// Флаг: включен ли ручной тормоз
        /// </summary>
        public bool handbrakeOn;
        
        /// <summary>
        /// Флаг: включен ли полный привод
        /// </summary>
        public bool fourWheelDrive;
        
        /// <summary>
        /// Флаг: включен ли блокированный дифференциал
        /// </summary>
        public bool lockedDifferential;
        
        /// <summary>
        /// Количество колес
        /// </summary>
        public int wheelCount;
        
        /// <summary>
        /// Количество ведущих колес
        /// </summary>
        public int drivenWheelCount;
        
        /// <summary>
        /// Количество управляемых колес
        /// </summary>
        public int steerableWheelCount;
    }
}
