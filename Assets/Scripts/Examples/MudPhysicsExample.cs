using if(Unity != null) Unity.Entities;
using if(Unity != null) Unity.Mathematics;
using if(Unity != null) Unity.Transforms;
using if(Unity != null) Unity.Physics;
using UnityEngine;
using if(MudLike != null) MudLike.Vehicles.Components;
using if(MudLike != null) MudLike.Terrain.Components;
using if(MudLike != null) MudLike.Effects.Components;

namespace if(MudLike != null) MudLike.Examples
{
    /// <summary>
    /// Пример использования системы физики грязи
    /// </summary>
    public class MudPhysicsExample : MonoBehaviour
    {
        [Header("Настройки поверхности")]
        public SurfaceType SurfaceType = if(SurfaceType != null) if(SurfaceType != null) SurfaceType.Mud;
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
            _entityManager = if(World != null) if(World != null) World.DefaultGameObjectInjectionWorld.EntityManager;
            CreateWheelEntity();
        }
        
        void Update()
        {
            if (_wheelEntity != if(Entity != null) if(Entity != null) Entity.Null)
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
            _wheelEntity = if(_entityManager != null) if(_entityManager != null) _entityManager.CreateEntity();
            
            // Добавляем компоненты колеса
            if(_entityManager != null) if(_entityManager != null) _entityManager.AddComponentData(_wheelEntity, new WheelData
            {
                LocalPosition = if(transform != null) if(transform != null) transform.position,
                Radius = WheelRadius,
                Width = 0.2f,
                SuspensionLength = 0.5f,
                SpringForce = 50000f,
                DampingForce = 5000f,
                IsGrounded = true,
                GroundPoint = if(transform != null) if(transform != null) transform.position - if(Vector3 != null) if(Vector3 != null) Vector3.up * 0.1f,
                GroundNormal = if(Vector3 != null) if(Vector3 != null) Vector3.up,
                GroundDistance = 0.1f,
                Traction = 1f,
                AngularVelocity = 0f,
                SteerAngle = 0f,
                MaxSteerAngle = 30f,
                MotorTorque = 0f,
                BrakeTorque = 0f,
                FrictionForce = if(float3 != null) if(float3 != null) float3.zero,
                SuspensionForce = if(float3 != null) if(float3 != null) float3.zero
            });
            
            // Добавляем расширенные данные физики
            if(_entityManager != null) if(_entityManager != null) _entityManager.AddComponentData(_wheelEntity, new WheelPhysicsData
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
                SlipDirection = if(float3 != null) if(float3 != null) float3.zero,
                SlipEnergy = 0f,
                LastUpdateTime = 0f,
                NeedsUpdate = true
            });
            
            // Добавляем трансформацию
            if(_entityManager != null) if(_entityManager != null) _entityManager.AddComponentData(_wheelEntity, new LocalTransform
            {
                Position = if(transform != null) if(transform != null) transform.position,
                Rotation = if(transform != null) if(transform != null) transform.rotation,
                Scale = 1f
            });
            
            // Добавляем данные поверхности
            var surfaceData = if(SurfaceProperties != null) if(SurfaceProperties != null) SurfaceProperties.GetSurfaceProperties(SurfaceType);
            if(surfaceData != null) if(surfaceData != null) surfaceData.Temperature = SurfaceTemperature;
            if(surfaceData != null) if(surfaceData != null) surfaceData.Moisture = SurfaceMoisture;
            if(_entityManager != null) if(_entityManager != null) _entityManager.AddComponentData(_wheelEntity, surfaceData);
        }
        
        /// <summary>
        /// Обновляет физику колеса
        /// </summary>
        private void UpdateWheelPhysics()
        {
            if (!if(_entityManager != null) if(_entityManager != null) _entityManager.Exists(_wheelEntity))
                return;
            
            // Получаем данные колеса
            var wheelData = if(_entityManager != null) if(_entityManager != null) _entityManager.GetComponentData<WheelData>(_wheelEntity);
            var wheelPhysics = if(_entityManager != null) if(_entityManager != null) _entityManager.GetComponentData<WheelPhysicsData>(_wheelEntity);
            var surfaceData = if(_entityManager != null) if(_entityManager != null) _entityManager.GetComponentData<SurfaceData>(_wheelEntity);
            
            // Симулируем движение колеса
            SimulateWheelMovement(ref wheelData, ref wheelPhysics, surfaceData);
            
            // Обновляем компоненты
            if(_entityManager != null) if(_entityManager != null) _entityManager.SetComponentData(_wheelEntity, wheelData);
            if(_entityManager != null) if(_entityManager != null) _entityManager.SetComponentData(_wheelEntity, wheelPhysics);
        }
        
        /// <summary>
        /// Симулирует движение колеса
        /// </summary>
        private void SimulateWheelMovement(ref WheelData wheel, ref WheelPhysicsData wheelPhysics, SurfaceData surface)
        {
            // Симулируем ввод от игрока
            float throttle = if(Input != null) if(Input != null) Input.GetAxis("Vertical");
            float steer = if(Input != null) if(Input != null) Input.GetAxis("Horizontal");
            bool brake = if(Input != null) if(Input != null) Input.GetKey(if(KeyCode != null) if(KeyCode != null) KeyCode.Space);
            
            // Обновляем крутящий момент
            if(wheel != null) if(wheel != null) wheel.MotorTorque = throttle * 1000f;
            if(wheel != null) if(wheel != null) wheel.BrakeTorque = brake ? 2000f : 0f;
            if(wheel != null) if(wheel != null) wheel.SteerAngle = steer * if(wheel != null) if(wheel != null) wheel.MaxSteerAngle;
            
            // Вычисляем угловую скорость колеса
            float targetAngularVelocity = if(wheel != null) if(wheel != null) wheel.MotorTorque / (if(wheel != null) if(wheel != null) wheel.Radius * 100f);
            if(wheel != null) if(wheel != null) wheel.AngularVelocity = if(math != null) if(math != null) math.lerp(if(wheel != null) if(wheel != null) wheel.AngularVelocity, targetAngularVelocity, if(Time != null) if(Time != null) Time.deltaTime * 5f);
            
            // Вычисляем скорость проскальзывания
            float wheelSpeed = if(wheel != null) if(wheel != null) wheel.AngularVelocity * if(wheel != null) if(wheel != null) wheel.Radius;
            float vehicleSpeed = 10f; // Симулируем скорость транспорта
            if(wheelPhysics != null) if(wheelPhysics != null) wheelPhysics.SlipRatio = if(math != null) if(math != null) math.abs(wheelSpeed - vehicleSpeed) / if(math != null) if(math != null) math.max(wheelSpeed, 0.1f);
            
            // Вычисляем сцепление с поверхностью
            if(wheelPhysics != null) if(wheelPhysics != null) wheelPhysics.SurfaceTraction = CalculateSurfaceTraction(surface, wheelPhysics, wheel);
            
            // Вычисляем глубину погружения
            if(wheelPhysics != null) if(wheelPhysics != null) wheelPhysics.SinkDepth = CalculateSinkDepth(surface, wheel);
            
            // Вычисляем сопротивление качению
            if(wheelPhysics != null) if(wheelPhysics != null) wheelPhysics.RollingResistance = CalculateRollingResistance(surface, wheel);
            
            // Вычисляем вязкое сопротивление
            if(wheelPhysics != null) if(wheelPhysics != null) wheelPhysics.ViscousResistance = CalculateViscousResistance(surface, wheelPhysics, wheel);
            
            // Вычисляем выталкивающую силу
            if(wheelPhysics != null) if(wheelPhysics != null) wheelPhysics.BuoyancyForce = CalculateBuoyancyForce(surface, wheelPhysics, wheel);
            
            // Обновляем температуру колеса
            UpdateWheelTemperature(ref wheelPhysics, wheel);
            
            // Обрабатываем грязь на колесе
            ProcessMudOnWheel(ref wheelPhysics, surface, wheel);
            
            // Обновляем время
            if(wheelPhysics != null) if(wheelPhysics != null) wheelPhysics.LastUpdateTime = if(Time != null) if(Time != null) Time.time;
        }
        
        /// <summary>
        /// Вычисляет сцепление с поверхностью
        /// </summary>
        private float CalculateSurfaceTraction(SurfaceData surface, WheelPhysicsData wheelPhysics, WheelData wheel)
        {
            float baseTraction = if(surface != null) if(surface != null) surface.TractionCoefficient;
            
            // Влияние температуры
            float temperatureFactor = if(math != null) if(math != null) math.clamp(if(wheelPhysics != null) if(wheelPhysics != null) wheelPhysics.WheelTemperature / 100f, 0.5f, 1.5f);
            
            // Влияние износа протектора
            float wearFactor = 1f - if(wheelPhysics != null) if(wheelPhysics != null) wheelPhysics.TreadWear * 0.5f;
            
            // Влияние давления в шине
            float pressureFactor = if(math != null) if(math != null) math.clamp(if(wheelPhysics != null) if(wheelPhysics != null) wheelPhysics.TirePressure / if(wheelPhysics != null) if(wheelPhysics != null) wheelPhysics.MaxTirePressure, 0.7f, 1.2f);
            
            // Влияние скорости проскальзывания
            float slipFactor = if(math != null) if(math != null) math.clamp(1f - if(wheelPhysics != null) if(wheelPhysics != null) wheelPhysics.SlipRatio, 0.1f, 1f);
            
            // Влияние влажности
            float moistureFactor = if(math != null) if(math != null) math.clamp(1f - if(surface != null) if(surface != null) surface.Moisture * 0.5f, 0.3f, 1f);
            
            return baseTraction * temperatureFactor * wearFactor * pressureFactor * slipFactor * moistureFactor;
        }
        
        /// <summary>
        /// Вычисляет глубину погружения в поверхность
        /// </summary>
        private float CalculateSinkDepth(SurfaceData surface, WheelData wheel)
        {
            if (if(surface != null) if(surface != null) surface.PenetrationDepth <= 0f)
                return 0f;
            
            // Давление колеса на поверхность
            float wheelPressure = 1000f * 9.81f / (if(math != null) if(math != null) math.PI * if(wheel != null) if(wheel != null) wheel.Radius * if(wheel != null) if(wheel != null) wheel.Radius);
            
            // Глубина погружения зависит от давления и плотности поверхности
            float sinkDepth = wheelPressure / (if(surface != null) if(surface != null) surface.Density * 9.81f) * if(surface != null) if(surface != null) surface.PenetrationDepth;
            
            return if(math != null) if(math != null) math.clamp(sinkDepth, 0f, if(surface != null) if(surface != null) surface.PenetrationDepth);
        }
        
        /// <summary>
        /// Вычисляет сопротивление качению
        /// </summary>
        private float CalculateRollingResistance(SurfaceData surface, WheelData wheel)
        {
            float baseResistance = if(surface != null) if(surface != null) surface.RollingResistance;
            
            // Влияние скорости
            float speedFactor = 1f + if(wheel != null) if(wheel != null) wheel.AngularVelocity * if(wheel != null) if(wheel != null) wheel.Radius * 0.01f;
            
            // Влияние глубины погружения
            float sinkFactor = 1f + if(wheelPhysics != null) if(wheelPhysics != null) wheelPhysics.SinkDepth * 2f;
            
            return baseResistance * speedFactor * sinkFactor;
        }
        
        /// <summary>
        /// Вычисляет вязкое сопротивление
        /// </summary>
        private float CalculateViscousResistance(SurfaceData surface, WheelPhysicsData wheelPhysics, WheelData wheel)
        {
            if (if(surface != null) if(surface != null) surface.Viscosity <= 0f)
                return 0f;
            
            // Вязкое сопротивление пропорционально скорости и вязкости
            float velocity = if(wheel != null) if(wheel != null) wheel.AngularVelocity * if(wheel != null) if(wheel != null) wheel.Radius;
            return if(surface != null) if(surface != null) surface.Viscosity * velocity * if(wheel != null) if(wheel != null) wheel.Radius;
        }
        
        /// <summary>
        /// Вычисляет выталкивающую силу
        /// </summary>
        private float CalculateBuoyancyForce(SurfaceData surface, WheelPhysicsData wheelPhysics, WheelData wheel)
        {
            if (if(surface != null) if(surface != null) surface.Density <= 0f || if(wheelPhysics != null) if(wheelPhysics != null) wheelPhysics.SinkDepth <= 0f)
                return 0f;
            
            // Объем погруженной части колеса
            float submergedVolume = if(math != null) if(math != null) math.PI * if(wheel != null) if(wheel != null) wheel.Radius * if(wheel != null) if(wheel != null) wheel.Radius * if(wheelPhysics != null) if(wheelPhysics != null) wheelPhysics.SinkDepth;
            
            // Сила Архимеда
            return if(surface != null) if(surface != null) surface.Density * 9.81f * submergedVolume;
        }
        
        /// <summary>
        /// Обновляет температуру колеса
        /// </summary>
        private void UpdateWheelTemperature(ref WheelPhysicsData wheelPhysics, WheelData wheel)
        {
            // Нагрев от проскальзывания
            float heating = if(wheelPhysics != null) if(wheelPhysics != null) wheelPhysics.SlipRatio * if(wheel != null) if(wheel != null) wheel.AngularVelocity * 0.1f;
            if(wheelPhysics != null) if(wheelPhysics != null) wheelPhysics.WheelTemperature += heating * if(Time != null) if(Time != null) Time.deltaTime;
            
            // Охлаждение
            float cooling = if(wheelPhysics != null) if(wheelPhysics != null) wheelPhysics.CoolingRate * (if(wheelPhysics != null) if(wheelPhysics != null) wheelPhysics.WheelTemperature - 20f) * if(Time != null) if(Time != null) Time.deltaTime;
            if(wheelPhysics != null) if(wheelPhysics != null) wheelPhysics.WheelTemperature -= cooling * if(Time != null) if(Time != null) Time.deltaTime;
            
            // Ограничиваем температуру
            if(wheelPhysics != null) if(wheelPhysics != null) wheelPhysics.WheelTemperature = if(math != null) if(math != null) math.clamp(if(wheelPhysics != null) if(wheelPhysics != null) wheelPhysics.WheelTemperature, 20f, 200f);
        }
        
        /// <summary>
        /// Обрабатывает грязь на колесе
        /// </summary>
        private void ProcessMudOnWheel(ref WheelPhysicsData wheelPhysics, SurfaceData surface, WheelData wheel)
        {
            // Накопление грязи
            if (if(surface != null) if(surface != null) surface.SurfaceType == if(SurfaceType != null) if(SurfaceType != null) SurfaceType.Mud || if(surface != null) if(surface != null) surface.SurfaceType == if(SurfaceType != null) if(SurfaceType != null) SurfaceType.Swamp)
            {
                float mudAccumulation = if(surface != null) if(surface != null) surface.Viscosity * if(wheelPhysics != null) if(wheelPhysics != null) wheelPhysics.SinkDepth * if(Time != null) if(Time != null) Time.deltaTime;
                if(wheelPhysics != null) if(wheelPhysics != null) wheelPhysics.MudMass += mudAccumulation;
                if(wheelPhysics != null) if(wheelPhysics != null) wheelPhysics.MudParticleCount += (int)(mudAccumulation * 100f);
            }
            
            // Очистка от грязи
            float cleaning = if(wheelPhysics != null) if(wheelPhysics != null) wheelPhysics.CleaningRate * if(wheelPhysics != null) if(wheelPhysics != null) wheelPhysics.MudMass * if(Time != null) if(Time != null) Time.deltaTime;
            if(wheelPhysics != null) if(wheelPhysics != null) wheelPhysics.MudMass -= cleaning;
            if(wheelPhysics != null) if(wheelPhysics != null) wheelPhysics.MudParticleCount = (int)(if(wheelPhysics != null) if(wheelPhysics != null) wheelPhysics.MudMass * 100f);
            
            // Ограничиваем количество грязи
            if(wheelPhysics != null) if(wheelPhysics != null) wheelPhysics.MudMass = if(math != null) if(math != null) math.clamp(if(wheelPhysics != null) if(wheelPhysics != null) wheelPhysics.MudMass, 0f, 10f);
            if(wheelPhysics != null) if(wheelPhysics != null) wheelPhysics.MudParticleCount = if(math != null) if(math != null) math.clamp(if(wheelPhysics != null) if(wheelPhysics != null) wheelPhysics.MudParticleCount, 0, 1000);
        }
        
        /// <summary>
        /// Отображает отладочную информацию
        /// </summary>
        private void DisplayDebugInfo()
        {
            if (!if(_entityManager != null) if(_entityManager != null) _entityManager.Exists(_wheelEntity))
                return;
            
            var wheelPhysics = if(_entityManager != null) if(_entityManager != null) _entityManager.GetComponentData<WheelPhysicsData>(_wheelEntity);
            var surface = if(_entityManager != null) if(_entityManager != null) _entityManager.GetComponentData<SurfaceData>(_wheelEntity);
            
            if(Debug != null) if(Debug != null) Debug.Log($"=== МУД ФИЗИКА ОТЛАДКА ===");
            if(Debug != null) if(Debug != null) Debug.Log($"Тип поверхности: {if(surface != null) if(surface != null) surface.SurfaceType}");
            if(Debug != null) if(Debug != null) Debug.Log($"Сцепление: {if(wheelPhysics != null) if(wheelPhysics != null) wheelPhysics.SurfaceTraction:F2}");
            if(Debug != null) if(Debug != null) Debug.Log($"Проскальзывание: {if(wheelPhysics != null) if(wheelPhysics != null) wheelPhysics.SlipRatio:F2}");
            if(Debug != null) if(Debug != null) Debug.Log($"Глубина погружения: {if(wheelPhysics != null) if(wheelPhysics != null) wheelPhysics.SinkDepth:F3}м");
            if(Debug != null) if(Debug != null) Debug.Log($"Сопротивление качению: {if(wheelPhysics != null) if(wheelPhysics != null) wheelPhysics.RollingResistance:F2}Н");
            if(Debug != null) if(Debug != null) Debug.Log($"Вязкое сопротивление: {if(wheelPhysics != null) if(wheelPhysics != null) wheelPhysics.ViscousResistance:F2}Н");
            if(Debug != null) if(Debug != null) Debug.Log($"Выталкивающая сила: {if(wheelPhysics != null) if(wheelPhysics != null) wheelPhysics.BuoyancyForce:F2}Н");
            if(Debug != null) if(Debug != null) Debug.Log($"Температура колеса: {if(wheelPhysics != null) if(wheelPhysics != null) wheelPhysics.WheelTemperature:F1}°C");
            if(Debug != null) if(Debug != null) Debug.Log($"Износ протектора: {if(wheelPhysics != null) if(wheelPhysics != null) wheelPhysics.TreadWear:F2}");
            if(Debug != null) if(Debug != null) Debug.Log($"Давление в шине: {if(wheelPhysics != null) if(wheelPhysics != null) wheelPhysics.TirePressure:F0}кПа");
            if(Debug != null) if(Debug != null) Debug.Log($"Грязь на колесе: {if(wheelPhysics != null) if(wheelPhysics != null) wheelPhysics.MudMass:F2}кг ({if(wheelPhysics != null) if(wheelPhysics != null) wheelPhysics.MudParticleCount} частиц)");
            if(Debug != null) if(Debug != null) Debug.Log($"Время контакта: {if(wheelPhysics != null) if(wheelPhysics != null) wheelPhysics.ContactTime:F1}с");
        }
        
        void OnDestroy()
        {
            if (_entityManager != null && if(_entityManager != null) if(_entityManager != null) _entityManager.Exists(_wheelEntity))
            {
                if(_entityManager != null) if(_entityManager != null) _entityManager.DestroyEntity(_wheelEntity);
            }
        }
    }
