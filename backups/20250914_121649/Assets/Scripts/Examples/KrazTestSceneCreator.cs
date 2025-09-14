using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using MudLike.Vehicles.Components;
using MudLike.Core.Components;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace MudLike.Examples
{
    /// <summary>
    /// Создает тестовую сцену с КРАЗом в Unity Editor
    /// </summary>
    public static class KrazTestSceneCreator
    {
        #if UNITY_EDITOR
        
        /// <summary>
        /// Создает тестовую сцену с КРАЗом
        /// </summary>
        [MenuItem("Mud-Like/🚗 Создать тестовую сцену с КРАЗом")]
        public static void CreateKrazTestScene()
        {
            Debug.Log("🚗 Начинаем создание тестовой сцены с КРАЗом...");
            
            // Создаем новую сцену
            var newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            
            // Создаем террейн
            CreateTerrain();
            
            // Создаем КРАЗ
            CreateKraz();
            
            // Настраиваем камеру
            SetupCamera();
            
            // Настраиваем освещение
            SetupLighting();
            
            // Сохраняем сцену
            string scenePath = "Assets/Scenes/KrazTestScene.unity";
            EditorSceneManager.SaveScene(newScene, scenePath);
            
            Debug.Log($"✅ Тестовая сцена с КРАЗом создана и сохранена: {scenePath}");
            Debug.Log("🎮 Управление КРАЗом: E - двигатель, WASD - движение, Пробел - тормоз");
        }
        
        /// <summary>
        /// Создает простой террейн
        /// </summary>
        private static void CreateTerrain()
        {
            Debug.Log("🌍 Создание террейна...");
            
            // Создаем GameObject для террейна
            GameObject terrainGO = new GameObject("Test Terrain");
            
            // Добавляем компонент Terrain
            Terrain terrain = terrainGO.AddComponent<Terrain>();
            TerrainCollider terrainCollider = terrainGO.AddComponent<TerrainCollider>();
            
            // Создаем TerrainData
            TerrainData terrainData = new TerrainData();
            terrainData.heightmapResolution = 513;
            terrainData.size = new Vector3(100, 2, 100); // 100x100 метров, высота 2м
            
            // Создаем слегка неровную поверхность
            float[,] heights = new float[terrainData.heightmapResolution, terrainData.heightmapResolution];
            for (int x = 0; x < terrainData.heightmapResolution; x++)
            {
                for (int y = 0; y < terrainData.heightmapResolution; y++)
                {
                    // Создаем небольшие неровности для реалистичности
                    float noise = Mathf.PerlinNoise(x * 0.01f, y * 0.01f) * 0.1f;
                    heights[x, y] = 0.1f + noise;
                }
            }
            terrainData.SetHeights(0, 0, heights);
            
            // Применяем TerrainData
            terrain.terrainData = terrainData;
            terrainCollider.terrainData = terrainData;
            
            // Позиционируем террейн
            terrainGO.transform.position = new Vector3(-50, 0, -50);
            
            Debug.Log("✅ Террейн создан");
        }
        
        /// <summary>
        /// Создает КРАЗ
        /// </summary>
        private static void CreateKraz()
        {
            Debug.Log("🚗 Создание КРАЗа...");
            
            // Создаем основной объект КРАЗа
            GameObject kraz = new GameObject("КРАЗ-255");
            kraz.transform.position = new Vector3(0, 2, 0);
            
            // Создаем визуальные компоненты КРАЗа
            CreateKrazVisual(kraz);
            
            // Добавляем конвертер ECS
            var converter = kraz.AddComponent<KrazVehicleConverter>();
            converter.enablePlayerControl = true;
            converter.enableEngineSound = true;
            converter.enableMudEffects = true;
            
            Debug.Log("✅ КРАЗ создан");
        }
        
        /// <summary>
        /// Создает визуальные компоненты КРАЗа
        /// </summary>
        private static void CreateKrazVisual(GameObject parent)
        {
            // Кабина
            GameObject cabin = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cabin.name = "Cabin";
            cabin.transform.SetParent(parent.transform);
            cabin.transform.localPosition = new Vector3(0, 1.2f, 0.5f);
            cabin.transform.localScale = new Vector3(2.5f, 1.5f, 2f);
            
            // Красный цвет для кабины
            var cabinRenderer = cabin.GetComponent<Renderer>();
            cabinRenderer.material = new Material(Shader.Find("Standard"));
            cabinRenderer.material.color = new Color(0.8f, 0.1f, 0.1f); // Красный
            
            // Кузов
            GameObject body = GameObject.CreatePrimitive(PrimitiveType.Cube);
            body.name = "Body";
            body.transform.SetParent(parent.transform);
            body.transform.localPosition = new Vector3(0, 0.8f, -0.8f);
            body.transform.localScale = new Vector3(2.5f, 1.2f, 3.5f);
            
            // Красный цвет для кузова
            var bodyRenderer = body.GetComponent<Renderer>();
            bodyRenderer.material = new Material(Shader.Find("Standard"));
            bodyRenderer.material.color = new Color(0.8f, 0.1f, 0.1f); // Красный
            
            // Создаем колеса
            CreateWheel(parent, "FrontLeft", new Vector3(-1.2f, -0.3f, 1.8f));
            CreateWheel(parent, "FrontRight", new Vector3(1.2f, -0.3f, 1.8f));
            CreateWheel(parent, "MiddleLeft", new Vector3(-1.2f, -0.3f, 0f));
            CreateWheel(parent, "MiddleRight", new Vector3(1.2f, -0.3f, 0f));
            CreateWheel(parent, "RearLeft", new Vector3(-1.2f, -0.3f, -1.8f));
            CreateWheel(parent, "RearRight", new Vector3(1.2f, -0.3f, -1.8f));
        }
        
        /// <summary>
        /// Создает колесо
        /// </summary>
        private static void CreateWheel(GameObject parent, string name, Vector3 position)
        {
            GameObject wheel = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            wheel.name = name;
            wheel.transform.SetParent(parent.transform);
            wheel.transform.localPosition = position;
            wheel.transform.localScale = new Vector3(1.2f, 0.2f, 1.2f);
            wheel.transform.localRotation = Quaternion.Euler(0, 0, 90);
            
            // Черный цвет для колес
            var wheelRenderer = wheel.GetComponent<Renderer>();
            wheelRenderer.material = new Material(Shader.Find("Standard"));
            wheelRenderer.material.color = Color.black;
        }
        
        /// <summary>
        /// Настраивает камеру
        /// </summary>
        private static void SetupCamera()
        {
            Debug.Log("📷 Настройка камеры...");
            
            // Создаем камеру если её нет
            Camera camera = Camera.main;
            if (camera == null)
            {
                GameObject cameraGO = new GameObject("Main Camera");
                camera = cameraGO.AddComponent<Camera>();
                cameraGO.tag = "MainCamera";
            }
            
            // Позиционируем камеру для обзора КРАЗа
            camera.transform.position = new Vector3(-15, 8, 0);
            camera.transform.rotation = Quaternion.Euler(20, 90, 0);
            
            // Добавляем скрипт следования за КРАЗом
            var followScript = camera.gameObject.GetComponent<KrazCameraFollow>();
            if (followScript == null)
            {
                followScript = camera.gameObject.AddComponent<KrazCameraFollow>();
            }
            
            Debug.Log("✅ Камера настроена");
        }
        
        /// <summary>
        /// Настраивает освещение
        /// </summary>
        private static void SetupLighting()
        {
            Debug.Log("☀️ Настройка освещения...");
            
            // Создаем направленный свет (солнце)
            GameObject lightGO = new GameObject("Sun");
            Light sunLight = lightGO.AddComponent<Light>();
            sunLight.type = LightType.Directional;
            sunLight.color = new Color(1f, 0.95f, 0.8f); // Теплый свет
            sunLight.intensity = 1.5f;
            sunLight.shadows = LightShadows.Soft;
            
            // Позиционируем солнце
            lightGO.transform.rotation = Quaternion.Euler(45f, 45f, 0f);
            
            // Создаем дополнительный свет для лучшего освещения
            GameObject fillLight = new GameObject("Fill Light");
            Light fill = fillLight.AddComponent<Light>();
            fill.type = LightType.Directional;
            fill.color = new Color(0.6f, 0.7f, 1f); // Холодный свет
            fill.intensity = 0.3f;
            fillLight.transform.rotation = Quaternion.Euler(-30f, -45f, 0f);
            
            Debug.Log("✅ Освещение настроено");
        }
        
        #endif
    }
    
    /// <summary>
    /// Скрипт для следования камеры за КРАЗом
    /// </summary>
    public class KrazCameraFollow : MonoBehaviour
    {
        [Header("Настройки камеры")]
        public float distance = 15f;
        public float height = 8f;
        public float followSpeed = 5f;
        
        private Transform krazTarget;
        
        void Start()
        {
            // Находим КРАЗ в сцене
            GameObject kraz = GameObject.Find("КРАЗ-255");
            if (kraz != null)
            {
                krazTarget = kraz.transform;
            }
        }
        
        void LateUpdate()
        {
            if (krazTarget == null) return;
            
            // Вычисляем позицию камеры
            Vector3 targetPosition = krazTarget.position + 
                                   krazTarget.right * -distance + 
                                   Vector3.up * height;
            
            // Плавно перемещаем камеру
            transform.position = Vector3.Lerp(transform.position, targetPosition, 
                                            followSpeed * Time.deltaTime);
            
            // Направляем камеру на КРАЗ
            Vector3 lookDirection = (krazTarget.position - transform.position).normalized;
            transform.rotation = Quaternion.LookRotation(lookDirection);
        }
    }
}
