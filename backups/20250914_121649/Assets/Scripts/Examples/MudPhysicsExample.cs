using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;
using UnityEngine;
using MudLike.Vehicles.Components;
using MudLike.Terrain.Components;
using MudLike.Effects.Components;

namespace MudLike.Examples
{
    /// <summary>
    /// Пример использования системы физики грязи
    /// </summary>
    public class MudPhysicsExample : MonoBehaviour
    {
        [Header("Настройки поверхности")]
        public SurfaceType SurfaceType = SurfaceType.Mud;
        public float SurfaceTemperature = 15f;
        public float SurfaceMoisture = 0.8f;
        
        [Header("Настройки колеса")]
        public float WheelRadius = 0.4f;
        public float TirePressure = 250f; // кПа
        public float MaxTirePressure = 300f;
        public float MinTirePressure = 200f;
        
        [Header("Настройки эффектов")]
        public bool EnableMudParticles = true;
        public bool EnableWaterSplashes = true;
        public bool EnableDustParticles = true;
        public bool EnableSparkParticles = true;
        
        [Header("Отладочная информация")]
        public bool ShowDebugInfo = true;
        
        private Entity _wheelEntity;
        private EntityManager _entityManager;
        
        void Start()
        {
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            CreateWheelEntity();
        }
        
        void Update()
        {
            if (_wheelEntity != Entity.Null)
            {
                UpdateWheelPhysics();
                
                if (ShowDebugInfo)
                {
                    DisplayDebugInfo();
                }
            }
        }
        
        /// <summary>
        /// Создает сущность колеса для тестирования
        /// </summary>
        private void CreateWheelEntity()
        {
            _wheelEntity = _entityManager.CreateEntity();
            
            // Добавляем компоненты колеса
            _entityManager.AddComponentData(_wheelEntity, new WheelData
            {
                LocalPosition = transform.position,
                Radius = WheelRadius,
                Width = 0.2f,
                SuspensionLength = 0.5f,
                SpringForce = 50000f,
                DampingForce = 5000f,
                IsGrounded = true,
                GroundPoint = transform.position - Vector3.up * 0.1f,
                GroundNormal = Vector3.up,
                GroundDistance = 0.1f,
                Traction = 1f,
                AngularVelocity = 0f,
                SteerAngle = 0f,
                MaxSteerAngle = 30f,
                MotorTorque = 0f,
                BrakeTorque = 0f,
                FrictionForce = float3.zero,
                SuspensionForce = float3.zero
            });
            
            // Добавляем расширенные данные физики
            _entityManager.AddComponentData(_wheelEntity, new WheelPhysicsData
            {
                SlipRatio = 0f,
                SlipAngle = 0f,
                SurfaceTraction = 1f,
                SinkDepth = 0f,
                RollingResistance = 0f,
                ViscousResistance = 0f,
                BuoyancyForce = 0f,
                SteeringResistance = 0f,
                WheelTemperature = 20f,
                TreadWear = 0f,
                TirePressure = TirePressure,
                MaxTirePressure = MaxTirePressure,
                MinTirePressure = MinTirePressure,
                HeatingRate = 0.1f,
                CoolingRate = 0.05f,
                ContactTime = 0f,
                LastSurfaceType = (int)SurfaceType,
                MudParticleCount = 0,
                MudMass = 0f,
                CleaningRate = 0.1f,
                CriticalSlipSpeed = 10f,
                MaxTractionForce = 1000f,
                CurrentTractionForce = 0f,
                LateralTractionForce = 0f,
                LongitudinalTractionForce = 0f,
                SlipAngularVelocity = 0f,
                SlipLinearVelocity = 0f,
                SlipDirection = float3.zero,
                SlipEnergy = 0f,
                LastUpdateTime = 0f,
                NeedsUpdate = true
            });
            
            // Добавляем трансформацию
            _entityManager.AddComponentData(_wheelEntity, new LocalTransform
            {
                Position = transform.position,
                Rotation = transform.rotation,
                Scale = 1f
            });
            
            // Добавляем данные поверхности
            var surfaceData = SurfaceProperties.GetSurfaceProperties(SurfaceType);
            surfaceData.Temperature = SurfaceTemperature;
            surfaceData.Moisture = SurfaceMoisture;
            _entityManager.AddComponentData(_wheelEntity, surfaceData);
        }
        
        /// <summary>
        /// Обновляет физику колеса
        /// </summary>
        private void UpdateWheelPhysics()
        {
            if (!_entityManager.Exists(_wheelEntity))
                return;
            
            // Получаем данные колеса
            var wheelData = _entityManager.GetComponentData<WheelData>(_wheelEntity);
            var wheelPhysics = _entityManager.GetComponentData<WheelPhysicsData>(_wheelEntity);
            var surfaceData = _entityManager.GetComponentData<SurfaceData>(_wheelEntity);
            
            // Симулируем движение колеса
            SimulateWheelMovement(ref wheelData, ref wheelPhysics, surfaceData);
            
            // Обновляем компоненты
            _entityManager.SetComponentData(_wheelEntity, wheelData);
            _entityManager.SetComponentData(_wheelEntity, wheelPhysics);
        }
        
        /// <summary>
        /// Симулирует движение колеса
        /// </summary>
        private void SimulateWheelMovement(ref WheelData wheel, ref WheelPhysicsData wheelPhysics, SurfaceData surface)
        {
            // Симулируем ввод от игрока
            float throttle = Input.GetAxis("Vertical");
            float steer = Input.GetAxis("Horizontal");
            bool brake = Input.GetKey(KeyCode.Space);
            
            // Обновляем крутящий момент
            wheel.MotorTorque = throttle * 1000f;
            wheel.BrakeTorque = brake ? 2000f : 0f;
            wheel.SteerAngle = steer * wheel.MaxSteerAngle;
            
            // Вычисляем угловую скорость колеса
            float targetAngularVelocity = wheel.MotorTorque / (wheel.Radius * 100f);
            wheel.AngularVelocity = math.lerp(wheel.AngularVelocity, targetAngularVelocity, Time.deltaTime * 5f);
            
            // Вычисляем скорость проскальзывания
            float wheelSpeed = wheel.AngularVelocity * wheel.Radius;
            float vehicleSpeed = 10f; // Симулируем скорость транспорта
            wheelPhysics.SlipRatio = math.abs(wheelSpeed - vehicleSpeed) / math.max(wheelSpeed, 0.1f);
            
            // Вычисляем сцепление с поверхностью
            wheelPhysics.SurfaceTraction = CalculateSurfaceTraction(surface, wheelPhysics, wheel);
            
            // Вычисляем глубину погружения
            wheelPhysics.SinkDepth = CalculateSinkDepth(surface, wheel);
            
            // Вычисляем сопротивление качению
            wheelPhysics.RollingResistance = CalculateRollingResistance(surface, wheel);
            
            // Вычисляем вязкое сопротивление
            wheelPhysics.ViscousResistance = CalculateViscousResistance(surface, wheelPhysics, wheel);
            
            // Вычисляем выталкивающую силу
            wheelPhysics.BuoyancyForce = CalculateBuoyancyForce(surface, wheelPhysics, wheel);
            
            // Обновляем температуру колеса
            UpdateWheelTemperature(ref wheelPhysics, wheel);
            
            // Обрабатываем грязь на колесе
            ProcessMudOnWheel(ref wheelPhysics, surface, wheel);
            
            // Обновляем время
            wheelPhysics.LastUpdateTime = Time.time;
        }
        
        /// <summary>
        /// Вычисляет сцепление с поверхностью
        /// </summary>
        private float CalculateSurfaceTraction(SurfaceData surface, WheelPhysicsData wheelPhysics, WheelData wheel)
        {
            float baseTraction = surface.TractionCoefficient;
            
            // Влияние температуры
            float temperatureFactor = math.clamp(wheelPhysics.WheelTemperature / 100f, 0.5f, 1.5f);
            
            // Влияние износа протектора
            float wearFactor = 1f - wheelPhysics.TreadWear * 0.5f;
            
            // Влияние давления в шине
            float pressureFactor = math.clamp(wheelPhysics.TirePressure / wheelPhysics.MaxTirePressure, 0.7f, 1.2f);
            
            // Влияние скорости проскальзывания
            float slipFactor = math.clamp(1f - wheelPhysics.SlipRatio, 0.1f, 1f);
            
            // Влияние влажности
            float moistureFactor = math.clamp(1f - surface.Moisture * 0.5f, 0.3f, 1f);
            
            return baseTraction * temperatureFactor * wearFactor * pressureFactor * slipFactor * moistureFactor;
        }
        
        /// <summary>
        /// Вычисляет глубину погружения в поверхность
        /// </summary>
        private float CalculateSinkDepth(SurfaceData surface, WheelData wheel)
        {
            if (surface.PenetrationDepth <= 0f)
                return 0f;
            
            // Давление колеса на поверхность
            float wheelPressure = 1000f * 9.81f / (math.PI * wheel.Radius * wheel.Radius);
            
            // Глубина погружения зависит от давления и плотности поверхности
            float sinkDepth = wheelPressure / (surface.Density * 9.81f) * surface.PenetrationDepth;
            
            return math.clamp(sinkDepth, 0f, surface.PenetrationDepth);
        }
        
        /// <summary>
        /// Вычисляет сопротивление качению
        /// </summary>
        private float CalculateRollingResistance(SurfaceData surface, WheelData wheel)
        {
            float baseResistance = surface.RollingResistance;
            
            // Влияние скорости
            float speedFactor = 1f + wheel.AngularVelocity * wheel.Radius * 0.01f;
            
            // Влияние глубины погружения
            float sinkFactor = 1f + wheelPhysics.SinkDepth * 2f;
            
            return baseResistance * speedFactor * sinkFactor;
        }
        
        /// <summary>
        /// Вычисляет вязкое сопротивление
        /// </summary>
        private float CalculateViscousResistance(SurfaceData surface, WheelPhysicsData wheelPhysics, WheelData wheel)
        {
            if (surface.Viscosity <= 0f)
                return 0f;
            
            // Вязкое сопротивление пропорционально скорости и вязкости
            float velocity = wheel.AngularVelocity * wheel.Radius;
            return surface.Viscosity * velocity * wheel.Radius;
        }
        
        /// <summary>
        /// Вычисляет выталкивающую силу
        /// </summary>
        private float CalculateBuoyancyForce(SurfaceData surface, WheelPhysicsData wheelPhysics, WheelData wheel)
        {
            if (surface.Density <= 0f || wheelPhysics.SinkDepth <= 0f)
                return 0f;
            
            // Объем погруженной части колеса
            float submergedVolume = math.PI * wheel.Radius * wheel.Radius * wheelPhysics.SinkDepth;
            
            // Сила Архимеда
            return surface.Density * 9.81f * submergedVolume;
        }
        
        /// <summary>
        /// Обновляет температуру колеса
        /// </summary>
        private void UpdateWheelTemperature(ref WheelPhysicsData wheelPhysics, WheelData wheel)
        {
            // Нагрев от проскальзывания
            float heating = wheelPhysics.SlipRatio * wheel.AngularVelocity * 0.1f;
            wheelPhysics.WheelTemperature += heating * Time.deltaTime;
            
            // Охлаждение
            float cooling = wheelPhysics.CoolingRate * (wheelPhysics.WheelTemperature - 20f) * Time.deltaTime;
            wheelPhysics.WheelTemperature -= cooling * Time.deltaTime;
            
            // Ограничиваем температуру
            wheelPhysics.WheelTemperature = math.clamp(wheelPhysics.WheelTemperature, 20f, 200f);
        }
        
        /// <summary>
        /// Обрабатывает грязь на колесе
        /// </summary>
        private void ProcessMudOnWheel(ref WheelPhysicsData wheelPhysics, SurfaceData surface, WheelData wheel)
        {
            // Накопление грязи
            if (surface.SurfaceType == SurfaceType.Mud || surface.SurfaceType == SurfaceType.Swamp)
            {
                float mudAccumulation = surface.Viscosity * wheelPhysics.SinkDepth * Time.deltaTime;
                wheelPhysics.MudMass += mudAccumulation;
                wheelPhysics.MudParticleCount += (int)(mudAccumulation * 100f);
            }
            
            // Очистка от грязи
            float cleaning = wheelPhysics.CleaningRate * wheelPhysics.MudMass * Time.deltaTime;
            wheelPhysics.MudMass -= cleaning;
            wheelPhysics.MudParticleCount = (int)(wheelPhysics.MudMass * 100f);
            
            // Ограничиваем количество грязи
            wheelPhysics.MudMass = math.clamp(wheelPhysics.MudMass, 0f, 10f);
            wheelPhysics.MudParticleCount = math.clamp(wheelPhysics.MudParticleCount, 0, 1000);
        }
        
        /// <summary>
        /// Отображает отладочную информацию
        /// </summary>
        private void DisplayDebugInfo()
        {
            if (!_entityManager.Exists(_wheelEntity))
                return;
            
            var wheelPhysics = _entityManager.GetComponentData<WheelPhysicsData>(_wheelEntity);
            var surface = _entityManager.GetComponentData<SurfaceData>(_wheelEntity);
            
            Debug.Log($"=== МУД ФИЗИКА ОТЛАДКА ===");
            Debug.Log($"Тип поверхности: {surface.SurfaceType}");
            Debug.Log($"Сцепление: {wheelPhysics.SurfaceTraction:F2}");
            Debug.Log($"Проскальзывание: {wheelPhysics.SlipRatio:F2}");
            Debug.Log($"Глубина погружения: {wheelPhysics.SinkDepth:F3}м");
            Debug.Log($"Сопротивление качению: {wheelPhysics.RollingResistance:F2}Н");
            Debug.Log($"Вязкое сопротивление: {wheelPhysics.ViscousResistance:F2}Н");
            Debug.Log($"Выталкивающая сила: {wheelPhysics.BuoyancyForce:F2}Н");
            Debug.Log($"Температура колеса: {wheelPhysics.WheelTemperature:F1}°C");
            Debug.Log($"Износ протектора: {wheelPhysics.TreadWear:F2}");
            Debug.Log($"Давление в шине: {wheelPhysics.TirePressure:F0}кПа");
            Debug.Log($"Грязь на колесе: {wheelPhysics.MudMass:F2}кг ({wheelPhysics.MudParticleCount} частиц)");
            Debug.Log($"Время контакта: {wheelPhysics.ContactTime:F1}с");
        }
        
        void OnDestroy()
        {
            if (_entityManager != null && _entityManager.Exists(_wheelEntity))
            {
                _entityManager.DestroyEntity(_wheelEntity);
            }
        }
    }
}