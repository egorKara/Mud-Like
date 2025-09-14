using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Burst;
using Unity.Jobs;
using UnityEngine;
using MudLike.Vehicles.Components;

namespace MudLike.Core.Performance
{
    /// <summary>
    /// GPU-ускоренные вычисления с использованием Compute Shaders
    /// </summary>
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial class ComputeShaderAccelerator : SystemBase
    {
        private ComputeShader _vehiclePhysicsCompute;
        private ComputeShader _wheelPhysicsCompute;
        private ComputeShader _terrainCompute;
        
        private int _vehiclePhysicsKernel;
        private int _wheelPhysicsKernel;
        private int _terrainKernel;
        
        private ComputeBuffer _vehicleBuffer;
        private ComputeBuffer _wheelBuffer;
        private ComputeBuffer _terrainBuffer;
        private ComputeBuffer _resultBuffer;
        
        private EntityQuery _vehicleQuery;
        private EntityQuery _wheelQuery;
        private EntityQuery _terrainQuery;
        
        private const int BUFFER_SIZE = 10000;
        private const int THREAD_GROUP_SIZE = 64;
        
        protected override void OnCreate()
        {
            // Загрузка Compute Shaders
            _vehiclePhysicsCompute = Resources.Load<ComputeShader>("VehiclePhysicsCompute");
            _wheelPhysicsCompute = Resources.Load<ComputeShader>("WheelPhysicsCompute");
            _terrainCompute = Resources.Load<ComputeShader>("TerrainCompute");
            
            // Получение kernel handles
            if (_vehiclePhysicsCompute != null)
                _vehiclePhysicsKernel = _vehiclePhysicsCompute.FindKernel("CSMain");
            if (_wheelPhysicsCompute != null)
                _wheelPhysicsKernel = _wheelPhysicsCompute.FindKernel("CSMain");
            if (_terrainCompute != null)
                _terrainKernel = _terrainCompute.FindKernel("CSMain");
            
            // Создание Compute Buffers
            _vehicleBuffer = new ComputeBuffer(BUFFER_SIZE, sizeof(float) * 32); // VehiclePhysics + VehicleConfig
            _wheelBuffer = new ComputeBuffer(BUFFER_SIZE, sizeof(float) * 24); // WheelData + WheelPhysicsData
            _terrainBuffer = new ComputeBuffer(BUFFER_SIZE, sizeof(float) * 16); // TerrainData
            _resultBuffer = new ComputeBuffer(BUFFER_SIZE, sizeof(float) * 32); // Results
            
            // Создание запросов
            _vehicleQuery = GetEntityQuery(
                ComponentType.ReadWrite<VehiclePhysics>()
            );
            
            _wheelQuery = GetEntityQuery(
                ComponentType.ReadWrite<WheelPhysicsData>()
            );
            
            _terrainQuery = GetEntityQuery(
                ComponentType.ReadWrite<MudLike.Terrain.Components.TerrainData>()
            );
        }
        
        protected override void OnDestroy()
        {
            // Освобождение Compute Buffers
            _vehicleBuffer?.Dispose();
            _wheelBuffer?.Dispose();
            _terrainBuffer?.Dispose();
            _resultBuffer?.Dispose();
        }
        
        protected override void OnUpdate()
        {
            // GPU-ускоренные вычисления для транспортных средств
            if (_vehiclePhysicsCompute != null)
            {
                ProcessVehiclePhysicsOnGPU();
            }
            
            // GPU-ускоренные вычисления для колес
            if (_wheelPhysicsCompute != null)
            {
                ProcessWheelPhysicsOnGPU();
            }
            
            // GPU-ускоренные вычисления для террейна
            if (_terrainCompute != null)
            {
                ProcessTerrainOnGPU();
            }
        }
        
        /// <summary>
        /// Обрабатывает физику транспортных средств на GPU
        /// </summary>
        private void ProcessVehiclePhysicsOnGPU()
        {
            // Загрузка данных на GPU
            UploadVehicleDataToGPU();
            
            // Настройка параметров Compute Shader
            _vehiclePhysicsCompute.SetBuffer(_vehiclePhysicsKernel, "VehicleBuffer", _vehicleBuffer);
            _vehiclePhysicsCompute.SetBuffer(_vehiclePhysicsKernel, "ResultBuffer", _resultBuffer);
            _vehiclePhysicsCompute.SetFloat("DeltaTime", SystemAPI.Time.DeltaTime);
            _vehiclePhysicsCompute.SetInt("VehicleCount", _vehicleQuery.CalculateEntityCount());
            
            // Выполнение Compute Shader
            int threadGroups = Mathf.CeilToInt(_vehicleQuery.CalculateEntityCount() / (float)THREAD_GROUP_SIZE);
            _vehiclePhysicsCompute.Dispatch(_vehiclePhysicsKernel, threadGroups, 1, 1);
            
            // Загрузка результатов обратно
            DownloadVehicleDataFromGPU();
        }
        
        /// <summary>
        /// Обрабатывает физику колес на GPU
        /// </summary>
        private void ProcessWheelPhysicsOnGPU()
        {
            // Загрузка данных на GPU
            UploadWheelDataToGPU();
            
            // Настройка параметров Compute Shader
            _wheelPhysicsCompute.SetBuffer(_wheelPhysicsKernel, "WheelBuffer", _wheelBuffer);
            _wheelPhysicsCompute.SetBuffer(_wheelPhysicsKernel, "ResultBuffer", _resultBuffer);
            _wheelPhysicsCompute.SetFloat("DeltaTime", SystemAPI.Time.DeltaTime);
            _wheelPhysicsCompute.SetInt("WheelCount", _wheelQuery.CalculateEntityCount());
            
            // Выполнение Compute Shader
            int threadGroups = Mathf.CeilToInt(_wheelQuery.CalculateEntityCount() / (float)THREAD_GROUP_SIZE);
            _wheelPhysicsCompute.Dispatch(_wheelPhysicsKernel, threadGroups, 1, 1);
            
            // Загрузка результатов обратно
            DownloadWheelDataFromGPU();
        }
        
        /// <summary>
        /// Обрабатывает террейн на GPU
        /// </summary>
        private void ProcessTerrainOnGPU()
        {
            // Загрузка данных на GPU
            UploadTerrainDataToGPU();
            
            // Настройка параметров Compute Shader
            _terrainCompute.SetBuffer(_terrainKernel, "TerrainBuffer", _terrainBuffer);
            _terrainCompute.SetBuffer(_terrainKernel, "ResultBuffer", _resultBuffer);
            _terrainCompute.SetFloat("DeltaTime", SystemAPI.Time.DeltaTime);
            _terrainCompute.SetInt("TerrainCount", _terrainQuery.CalculateEntityCount());
            
            // Выполнение Compute Shader
            int threadGroups = Mathf.CeilToInt(_terrainQuery.CalculateEntityCount() / (float)THREAD_GROUP_SIZE);
            _terrainCompute.Dispatch(_terrainKernel, threadGroups, 1, 1);
            
            // Загрузка результатов обратно
            DownloadTerrainDataFromGPU();
        }
        
        /// <summary>
        /// Загружает данные транспортных средств на GPU
        /// </summary>
        private void UploadVehicleDataToGPU()
        {
            var vehiclePhysicsArray = _vehicleQuery.ToComponentDataArray<VehiclePhysics>(Allocator.Temp);
            // var vehicleConfigArray = _vehicleQuery.ToComponentDataArray<VehicleConfig>(Allocator.Temp);
            
            // Создание массива для GPU
            var gpuData = new NativeArray<float>(BUFFER_SIZE * 32, Allocator.Temp);
            
            for (int i = 0; i < vehiclePhysicsArray.Length && i < BUFFER_SIZE; i++)
            {
                int baseIndex = i * 32;
                
                // VehiclePhysics данные
                gpuData[baseIndex + 0] = vehiclePhysicsArray[i].Velocity.x;
                gpuData[baseIndex + 1] = vehiclePhysicsArray[i].Velocity.y;
                gpuData[baseIndex + 2] = vehiclePhysicsArray[i].Velocity.z;
                gpuData[baseIndex + 3] = vehiclePhysicsArray[i].AngularVelocity.x;
                gpuData[baseIndex + 4] = vehiclePhysicsArray[i].AngularVelocity.y;
                gpuData[baseIndex + 5] = vehiclePhysicsArray[i].AngularVelocity.z;
                gpuData[baseIndex + 6] = vehiclePhysicsArray[i].LinearAcceleration.x;
                gpuData[baseIndex + 7] = vehiclePhysicsArray[i].LinearAcceleration.y;
                gpuData[baseIndex + 8] = vehiclePhysicsArray[i].LinearAcceleration.z;
                gpuData[baseIndex + 9] = vehiclePhysicsArray[i].AngularAcceleration.x;
                gpuData[baseIndex + 10] = vehiclePhysicsArray[i].AngularAcceleration.y;
                gpuData[baseIndex + 11] = vehiclePhysicsArray[i].AngularAcceleration.z;
                gpuData[baseIndex + 12] = vehiclePhysicsArray[i].AppliedForce.x;
                gpuData[baseIndex + 13] = vehiclePhysicsArray[i].AppliedForce.y;
                gpuData[baseIndex + 14] = vehiclePhysicsArray[i].AppliedForce.z;
                gpuData[baseIndex + 15] = vehiclePhysicsArray[i].AppliedTorque.x;
                gpuData[baseIndex + 16] = vehiclePhysicsArray[i].AppliedTorque.y;
                gpuData[baseIndex + 17] = vehiclePhysicsArray[i].AppliedTorque.z;
                gpuData[baseIndex + 18] = vehiclePhysicsArray[i].ForwardSpeed;
                gpuData[baseIndex + 19] = vehiclePhysicsArray[i].TurnSpeed;
                gpuData[baseIndex + 20] = vehiclePhysicsArray[i].CurrentGear;
                gpuData[baseIndex + 21] = vehiclePhysicsArray[i].EngineRPM;
                gpuData[baseIndex + 22] = vehiclePhysicsArray[i].EnginePower;
                gpuData[baseIndex + 23] = vehiclePhysicsArray[i].EngineTorque;
                
                // VehicleConfig данные
                gpuData[baseIndex + 24] = vehicleConfigArray[i].MaxSpeed;
                gpuData[baseIndex + 25] = vehicleConfigArray[i].Acceleration;
                gpuData[baseIndex + 26] = vehicleConfigArray[i].TurnSpeed;
                gpuData[baseIndex + 27] = vehicleConfigArray[i].Mass;
                gpuData[baseIndex + 28] = vehicleConfigArray[i].Drag;
                gpuData[baseIndex + 29] = vehicleConfigArray[i].AngularDrag;
                gpuData[baseIndex + 30] = vehicleConfigArray[i].TurnRadius;
                gpuData[baseIndex + 31] = vehicleConfigArray[i].CenterOfMassHeight;
            }
            
            // Загрузка данных в Compute Buffer
            _vehicleBuffer.SetData(gpuData);
            
            // Освобождение временных массивов
            vehiclePhysicsArray.Dispose();
            vehicleConfigArray.Dispose();
            gpuData.Dispose();
        }
        
        /// <summary>
        /// Загружает данные колес на GPU
        /// </summary>
        private void UploadWheelDataToGPU()
        {
            var wheelDataArray = _wheelQuery.ToComponentDataArray<WheelPhysicsData>(Allocator.Temp);
            // var wheelPhysicsArray = _wheelQuery.ToComponentDataArray<WheelPhysicsData>(Allocator.Temp);
            
            // Создание массива для GPU
            var gpuData = new NativeArray<float>(BUFFER_SIZE * 24, Allocator.Temp);
            
            for (int i = 0; i < wheelDataArray.Length && i < BUFFER_SIZE; i++)
            {
                int baseIndex = i * 24;
                
                // WheelData данные
                gpuData[baseIndex + 0] = wheelDataArray[i].LocalPosition.x;
                gpuData[baseIndex + 1] = wheelDataArray[i].LocalPosition.y;
                gpuData[baseIndex + 2] = wheelDataArray[i].LocalPosition.z;
                gpuData[baseIndex + 3] = wheelDataArray[i].Radius;
                gpuData[baseIndex + 4] = wheelDataArray[i].Width;
                gpuData[baseIndex + 5] = wheelDataArray[i].SuspensionLength;
                gpuData[baseIndex + 6] = wheelDataArray[i].SpringForce;
                gpuData[baseIndex + 7] = wheelDataArray[i].DampingForce;
                gpuData[baseIndex + 8] = wheelDataArray[i].IsGrounded ? 1f : 0f;
                gpuData[baseIndex + 9] = wheelDataArray[i].Traction;
                gpuData[baseIndex + 10] = wheelDataArray[i].AngularVelocity;
                gpuData[baseIndex + 11] = wheelDataArray[i].SteerAngle;
                
                // WheelPhysicsData данные
                gpuData[baseIndex + 12] = wheelPhysicsArray[i].SlipRatio;
                gpuData[baseIndex + 13] = wheelPhysicsArray[i].SlipAngle;
                gpuData[baseIndex + 14] = wheelPhysicsArray[i].SurfaceTraction;
                gpuData[baseIndex + 15] = wheelPhysicsArray[i].SinkDepth;
                gpuData[baseIndex + 16] = wheelPhysicsArray[i].RollingResistance;
                gpuData[baseIndex + 17] = wheelPhysicsArray[i].ViscousResistance;
                gpuData[baseIndex + 18] = wheelPhysicsArray[i].BuoyancyForce;
                gpuData[baseIndex + 19] = wheelPhysicsArray[i].WheelTemperature;
                gpuData[baseIndex + 20] = wheelPhysicsArray[i].TreadWear;
                gpuData[baseIndex + 21] = wheelPhysicsArray[i].TirePressure;
                gpuData[baseIndex + 22] = wheelPhysicsArray[i].MudMass;
                gpuData[baseIndex + 23] = wheelPhysicsArray[i].MudParticleCount;
            }
            
            // Загрузка данных в Compute Buffer
            _wheelBuffer.SetData(gpuData);
            
            // Освобождение временных массивов
            wheelDataArray.Dispose();
            wheelPhysicsArray.Dispose();
            gpuData.Dispose();
        }
        
        /// <summary>
        /// Загружает данные террейна на GPU
        /// </summary>
        private void UploadTerrainDataToGPU()
        {
            var terrainDataArray = _terrainQuery.ToComponentDataArray<MudLike.Terrain.Components.TerrainData>(Allocator.Temp);
            
            // Создание массива для GPU
            var gpuData = new NativeArray<float>(BUFFER_SIZE * 16, Allocator.Temp);
            
            for (int i = 0; i < terrainDataArray.Length && i < BUFFER_SIZE; i++)
            {
                int baseIndex = i * 16;
                
                // TerrainData данные
                gpuData[baseIndex + 0] = terrainDataArray[i].Size.x;
                gpuData[baseIndex + 1] = terrainDataArray[i].Size.y;
                gpuData[baseIndex + 2] = terrainDataArray[i].Size.z;
                gpuData[baseIndex + 3] = terrainDataArray[i].Resolution;
                gpuData[baseIndex + 4] = terrainDataArray[i].ChunkSize;
                gpuData[baseIndex + 5] = terrainDataArray[i].ChunkCountX;
                gpuData[baseIndex + 6] = terrainDataArray[i].ChunkCountZ;
                gpuData[baseIndex + 7] = terrainDataArray[i].MaxHeight;
                gpuData[baseIndex + 8] = terrainDataArray[i].MinHeight;
                gpuData[baseIndex + 9] = terrainDataArray[i].HeightScale;
                gpuData[baseIndex + 10] = terrainDataArray[i].IsDirty ? 1f : 0f;
                gpuData[baseIndex + 11] = terrainDataArray[i].NeedsSync ? 1f : 0f;
                gpuData[baseIndex + 12] = terrainDataArray[i].LastUpdateTime;
                gpuData[baseIndex + 13] = terrainDataArray[i].UpdateCount;
                gpuData[baseIndex + 14] = terrainDataArray[i].DeformationCount;
                gpuData[baseIndex + 15] = terrainDataArray[i].IsActive ? 1f : 0f;
            }
            
            // Загрузка данных в Compute Buffer
            _terrainBuffer.SetData(gpuData);
            
            // Освобождение временных массивов
            terrainDataArray.Dispose();
            gpuData.Dispose();
        }
        
        /// <summary>
        /// Загружает результаты транспортных средств с GPU
        /// </summary>
        private void DownloadVehicleDataFromGPU()
        {
            // Получение результатов с GPU
            var resultData = new NativeArray<float>(BUFFER_SIZE * 32, Allocator.Temp);
            _resultBuffer.GetData(resultData.AsArray());
            
            // Обновление компонентов
            var vehiclePhysicsArray = _vehicleQuery.ToComponentDataArray<VehiclePhysics>(Allocator.Temp);
            
            for (int i = 0; i < vehiclePhysicsArray.Length && i < BUFFER_SIZE; i++)
            {
                int baseIndex = i * 32;
                
                var physics = vehiclePhysicsArray[i];
                physics.Velocity = new float3(resultData[baseIndex + 0], resultData[baseIndex + 1], resultData[baseIndex + 2]);
                physics.AngularVelocity = new float3(resultData[baseIndex + 3], resultData[baseIndex + 4], resultData[baseIndex + 5]);
                physics.Acceleration = new float3(resultData[baseIndex + 6], resultData[baseIndex + 7], resultData[baseIndex + 8]);
                physics.AngularAcceleration = new float3(resultData[baseIndex + 9], resultData[baseIndex + 10], resultData[baseIndex + 11]);
                physics.AppliedForce = new float3(resultData[baseIndex + 12], resultData[baseIndex + 13], resultData[baseIndex + 14]);
                physics.AppliedTorque = new float3(resultData[baseIndex + 15], resultData[baseIndex + 16], resultData[baseIndex + 17]);
                physics.ForwardSpeed = resultData[baseIndex + 18];
                physics.TurnSpeed = resultData[baseIndex + 19];
                physics.CurrentGear = (int)resultData[baseIndex + 20];
                physics.EngineRPM = resultData[baseIndex + 21];
                physics.EnginePower = resultData[baseIndex + 22];
                physics.EngineTorque = resultData[baseIndex + 23];
                
                vehiclePhysicsArray[i] = physics;
            }
            
            // Обновление компонентов в ECS
            _vehicleQuery.CopyFromComponentDataArray(vehiclePhysicsArray);
            
            // Освобождение временных массивов
            vehiclePhysicsArray.Dispose();
            resultData.Dispose();
        }
        
        /// <summary>
        /// Загружает результаты колес с GPU
        /// </summary>
        private void DownloadWheelDataFromGPU()
        {
            // Получение результатов с GPU
            var resultData = new NativeArray<float>(BUFFER_SIZE * 24, Allocator.Temp);
            _resultBuffer.GetData(resultData.AsArray());
            
            // Обновление компонентов
            var wheelDataArray = _wheelQuery.ToComponentDataArray<WheelPhysicsData>(Allocator.Temp);
            // var wheelPhysicsArray = _wheelQuery.ToComponentDataArray<WheelPhysicsData>(Allocator.Temp);
            
            for (int i = 0; i < wheelDataArray.Length && i < BUFFER_SIZE; i++)
            {
                int baseIndex = i * 24;
                
                var wheelData = wheelDataArray[i];
                wheelData.LocalPosition = new float3(resultData[baseIndex + 0], resultData[baseIndex + 1], resultData[baseIndex + 2]);
                wheelData.Radius = resultData[baseIndex + 3];
                wheelData.Width = resultData[baseIndex + 4];
                wheelData.SuspensionLength = resultData[baseIndex + 5];
                wheelData.SpringForce = resultData[baseIndex + 6];
                wheelData.DampingForce = resultData[baseIndex + 7];
                wheelData.IsGrounded = resultData[baseIndex + 8] > 0.5f;
                wheelData.Traction = resultData[baseIndex + 9];
                wheelData.AngularVelocity = resultData[baseIndex + 10];
                wheelData.SteerAngle = resultData[baseIndex + 11];
                
                var wheelPhysics = wheelPhysicsArray[i];
                wheelPhysics.SlipRatio = resultData[baseIndex + 12];
                wheelPhysics.SlipAngle = resultData[baseIndex + 13];
                wheelPhysics.SurfaceTraction = resultData[baseIndex + 14];
                wheelPhysics.SinkDepth = resultData[baseIndex + 15];
                wheelPhysics.RollingResistance = resultData[baseIndex + 16];
                wheelPhysics.ViscousResistance = resultData[baseIndex + 17];
                wheelPhysics.BuoyancyForce = resultData[baseIndex + 18];
                wheelPhysics.WheelTemperature = resultData[baseIndex + 19];
                wheelPhysics.TreadWear = resultData[baseIndex + 20];
                wheelPhysics.TirePressure = resultData[baseIndex + 21];
                wheelPhysics.MudMass = resultData[baseIndex + 22];
                wheelPhysics.MudParticleCount = (int)resultData[baseIndex + 23];
                
                wheelDataArray[i] = wheelData;
                wheelPhysicsArray[i] = wheelPhysics;
            }
            
            // Обновление компонентов в ECS
            _wheelQuery.CopyFromComponentDataArray(wheelDataArray);
            _wheelQuery.CopyFromComponentDataArray(wheelPhysicsArray);
            
            // Освобождение временных массивов
            wheelDataArray.Dispose();
            wheelPhysicsArray.Dispose();
            resultData.Dispose();
        }
        
        /// <summary>
        /// Загружает результаты террейна с GPU
        /// </summary>
        private void DownloadTerrainDataFromGPU()
        {
            // Получение результатов с GPU
            var resultData = new NativeArray<float>(BUFFER_SIZE * 16, Allocator.Temp);
            _resultBuffer.GetData(resultData.AsArray());
            
            // Обновление компонентов
            var terrainDataArray = _terrainQuery.ToComponentDataArray<MudLike.Terrain.Components.TerrainData>(Allocator.Temp);
            
            for (int i = 0; i < terrainDataArray.Length && i < BUFFER_SIZE; i++)
            {
                int baseIndex = i * 16;
                
                var terrainData = terrainDataArray[i];
                terrainData.Size = new float3(resultData[baseIndex + 0], resultData[baseIndex + 1], resultData[baseIndex + 2]);
                terrainData.Resolution = (int)resultData[baseIndex + 3];
                terrainData.ChunkSize = resultData[baseIndex + 4];
                terrainData.ChunkCountX = (int)resultData[baseIndex + 5];
                terrainData.ChunkCountZ = (int)resultData[baseIndex + 6];
                terrainData.MaxHeight = resultData[baseIndex + 7];
                terrainData.MinHeight = resultData[baseIndex + 8];
                terrainData.HeightScale = resultData[baseIndex + 9];
                terrainData.IsDirty = resultData[baseIndex + 10] > 0.5f;
                terrainData.NeedsSync = resultData[baseIndex + 11] > 0.5f;
                terrainData.LastUpdateTime = resultData[baseIndex + 12];
                terrainData.UpdateCount = (int)resultData[baseIndex + 13];
                terrainData.DeformationCount = (int)resultData[baseIndex + 14];
                terrainData.IsActive = resultData[baseIndex + 15] > 0.5f;
                
                terrainDataArray[i] = terrainData;
            }
            
            // Обновление компонентов в ECS
            _terrainQuery.CopyFromComponentDataArray(terrainDataArray);
            
            // Освобождение временных массивов
            terrainDataArray.Dispose();
            resultData.Dispose();
        }
    }
}