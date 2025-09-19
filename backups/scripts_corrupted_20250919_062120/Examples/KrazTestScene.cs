using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using MudLike.Vehicles.Components;
using MudLike.Core.Components;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MudLike.Examples
{
    /// <summary>
    /// Создает простую тестовую сцену с КРАЗом для езды по простой поверхности
    /// </summary>
    public class KrazTestScene : MonoBehaviour
    {
        [Header("🚗 Настройки КРАЗа")]
        [Tooltip("Префаб КРАЗа")]
        public GameObject krazPrefab;
        
        [Tooltip("Позиция спавна КРАЗа")]
        public Vector3 spawnPosition = new Vector3(0, 2, 0);
        
        [Tooltip("Поворот КРАЗа при спавне")]
        public Vector3 spawnRotation = Vector3.zero;
        
        [Header("🌍 Настройки террейна")]
        [Tooltip("Размер террейна")]
        public Vector2 terrainSize = new Vector2(100, 100);
        
        [Tooltip("Высота террейна")]
        public float terrainHeight = 1f;
        
        [Tooltip("Материал террейна")]
        public Material terrainMaterial;
        
        [Header("📷 Настройки камеры")]
        [Tooltip("Следовать за КРАЗом")]
        public bool followKraz = true;
        
        [Tooltip("Расстояние камеры от КРАЗа")]
        public float cameraDistance = 15f;
        
        [Tooltip("Высота камеры")]
        public float cameraHeight = 8f;
        
        [Tooltip("Скорость следования камеры")]
        public float cameraSpeed = 5f;
        
        private GameObject _krazInstance;
        private Camera _camera;
        private Transform _cameraTarget;
        
        /// <summary>
        /// Инициализация тестовой сцены
        /// </summary>
        void Start()
        {
            CreateTestScene();
        }
        
        /// <summary>
        /// Обновление камеры
        /// </summary>
        void Update()
        {
            if (followKraz && _krazInstance != null && _camera != null)
            {
                UpdateCamera();
            }
        }
        
        /// <summary>
        /// Создает тестовую сцену
        /// </summary>
        private void CreateTestScene()
        {
            Debug.Log("🚗 Создание тестовой сцены с КРАЗом...");
            
            // Создаем террейн
            CreateTerrain();
            
            // Создаем КРАЗ
            CreateKraz();
            
            // Настраиваем камеру
            SetupCamera();
            
            // Создаем освещение
            SetupLighting();
            
            Debug.Log("✅ Тестовая сцена с КРАЗом создана!");
        }
        
        /// <summary>
        /// Создает простой террейн
        /// </summary>
        private void CreateTerrain()
        {
            // Создаем GameObject для террейна
            GameObject terrainGO = new GameObject("Test Terrain");
            
            // Добавляем компонент Terrain
            Terrain terrain = terrainGO.AddComponent<Terrain>();
            TerrainCollider terrainCollider = terrainGO.AddComponent<TerrainCollider>();
            
            // Создаем TerrainData
            TerrainData terrainData = new TerrainData();
            terrainData.heightmapResolution = 513;
            terrainData.size = new Vector3(terrainSize.x, terrainHeight, terrainSize.y);
            
            // Создаем плоскую поверхность
            float[,] heights = new float[terrainData.heightmapResolution, terrainData.heightmapResolution];
            for (int x = 0; x < terrainData.heightmapResolution; x++)
            {
                for (int y = 0; y < terrainData.heightmapResolution; y++)
                {
                    heights[x, y] = 0.1f; // Небольшая высота для визуализации
                }
            }
            terrainData.SetHeights(0, 0, heights);
            
            // Применяем TerrainData
            terrain.terrainData = terrainData;
            terrainCollider.terrainData = terrainData;
            
            // Применяем материал
            if (terrainMaterial != null)
            {
                terrain.materialTemplate = terrainMaterial;
            }
            
            // Позиционируем террейн
            terrainGO.transform.position = new Vector3(-terrainSize.x/2, 0, -terrainSize.y/2);
            
            Debug.Log("🌍 Террейн создан");
        }
        
        /// <summary>
        /// Создает КРАЗ
        /// </summary>
        private void CreateKraz()
        {
            if (krazPrefab == null)
            {
                // Создаем простой КРАЗ если нет префаба
                _krazInstance = CreateSimpleKraz();
            }
            else
            {
                // Используем существующий префаб
                _krazInstance = Instantiate(krazPrefab, spawnPosition, Quaternion.Euler(spawnRotation));
            }
            
            _krazInstance.name = "КРАЗ-255";
            
            // Добавляем конвертер ECS если его нет
            if (_krazInstance.GetComponent<KrazVehicleConverter>() == null)
            {
                var converter = _krazInstance.AddComponent<KrazVehicleConverter>();
                converter.enablePlayerControl = true;
                converter.enableEngineSound = true;
                converter.enableMudEffects = true;
            }
            
            _cameraTarget = _krazInstance.transform;
            
            Debug.Log("🚗 КРАЗ создан и готов к управлению!");
        }
        
        /// <summary>
        /// Создает простой КРАЗ из примитивов
        /// </summary>
        private GameObject CreateSimpleKraz()
        {
            GameObject kraz = new GameObject("КРАЗ-255");
            
            // Кабина
            GameObject cabin = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cabin.name = "Cabin";
            cabin.transform.SetParent(kraz.transform);
            cabin.transform.localPosition = new Vector3(0, 1.2f, 0.5f);
            cabin.transform.localScale = new Vector3(2.5f, 1.5f, 2f);
            cabin.GetComponent<Renderer>().material.color = Color.red;
            
            // Кузов
            GameObject body = GameObject.CreatePrimitive(PrimitiveType.Cube);
            body.name = "Body";
            body.transform.SetParent(kraz.transform);
            body.transform.localPosition = new Vector3(0, 0.8f, -0.8f);
            body.transform.localScale = new Vector3(2.5f, 1.2f, 3.5f);
            body.GetComponent<Renderer>().material.color = Color.red;
            
            // Колеса (6 штук)
            CreateWheel(kraz, "FrontLeft", new Vector3(-1.2f, -0.3f, 1.8f));
            CreateWheel(kraz, "FrontRight", new Vector3(1.2f, -0.3f, 1.8f));
            CreateWheel(kraz, "MiddleLeft", new Vector3(-1.2f, -0.3f, 0f));
            CreateWheel(kraz, "MiddleRight", new Vector3(1.2f, -0.3f, 0f));
            CreateWheel(kraz, "RearLeft", new Vector3(-1.2f, -0.3f, -1.8f));
            CreateWheel(kraz, "RearRight", new Vector3(1.2f, -0.3f, -1.8f));
            
            // Позиционируем КРАЗ
            kraz.transform.position = spawnPosition;
            kraz.transform.rotation = Quaternion.Euler(spawnRotation);
            
            return kraz;
        }
        
        /// <summary>
        /// Создает колесо
        /// </summary>
        private void CreateWheel(GameObject parent, string name, Vector3 position)
        {
            GameObject wheel = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            wheel.name = name;
            wheel.transform.SetParent(parent.transform);
            wheel.transform.localPosition = position;
            wheel.transform.localScale = new Vector3(1.2f, 0.2f, 1.2f);
            wheel.transform.localRotation = Quaternion.Euler(0, 0, 90);
            wheel.GetComponent<Renderer>().material.color = Color.black;
        }
        
        /// <summary>
        /// Настраивает камеру
        /// </summary>
        private void SetupCamera()
        {
            _camera = Camera.main;
            if (_camera == null)
            {
                GameObject cameraGO = new GameObject("Main Camera");
                _camera = cameraGO.AddComponent<Camera>();
                cameraGO.tag = "MainCamera";
            }
            
            if (followKraz)
            {
                UpdateCamera();
            }
            
            Debug.Log("📷 Камера настроена");
        }
        
        /// <summary>
        /// Обновляет позицию камеры
        /// </summary>
        private void UpdateCamera()
        {
            if (_cameraTarget == null) return;
            
            Vector3 targetPosition = _cameraTarget.position + 
                                   _cameraTarget.right * -cameraDistance + 
                                   Vector3.up * cameraHeight;
            
            Vector3 lookDirection = (_cameraTarget.position - targetPosition).normalized;
            
            _camera.transform.position = Vector3.Lerp(_camera.transform.position, targetPosition, 
                                                    cameraSpeed * Time.deltaTime);
            _camera.transform.rotation = Quaternion.LookRotation(lookDirection);
        }
        
        /// <summary>
        /// Настраивает освещение
        /// </summary>
        private void SetupLighting()
        {
            // Создаем направленный свет (солнце)
            GameObject lightGO = new GameObject("Sun");
            Light sunLight = lightGO.AddComponent<Light>();
            sunLight.type = LightType.Directional;
            sunLight.color = Color.white;
            sunLight.intensity = 1.5f;
            sunLight.shadows = LightShadows.Soft;
            
            // Позиционируем солнце
            lightGO.transform.rotation = Quaternion.Euler(45f, 45f, 0f);
            
            Debug.Log("☀️ Освещение настроено");
        }
        
        #if UNITY_EDITOR
        
        /// <summary>
        /// Создает тестовую сцену в редакторе
        /// </summary>
        [MenuItem("Mud-Like/🚗 Создать тестовую сцену с КРАЗом")]
        public static void CreateKrazTestSceneInEditor()
        {
            // Создаем GameObject с нашим скриптом
            GameObject sceneCreator = new GameObject("KrazTestScene");
            sceneCreator.AddComponent<KrazTestScene>();
            
            Debug.Log("🚗 Тестовая сцена с КРАЗом создана в редакторе!");
        }
        
        #endif
    }
}
