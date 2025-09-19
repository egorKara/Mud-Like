using UnityEngine;
using UnityEditor;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using MudLike.Vehicles.Components;
using MudLike.Vehicles.Converters;
using MudLike.Examples;

namespace MudLike.Examples
{
    /// <summary>
    /// Запуск тестовой сцены с КРАЗом в Unity Editor
    /// </summary>
    public class KrazTestSceneLauncher
    {
        [MenuItem("Mud-Like/🚗 Создать и запустить тестовую сцену с КРАЗом")]
        public static void CreateAndRunKrazTestScene()
        {
            Debug.Log("🚗 Создание тестовой сцены с КРАЗом...");
            
            // Создаем новую сцену
            var newScene = UnityEngine.SceneManagement.SceneManager.CreateScene("KrazTestScene");
            UnityEngine.SceneManagement.SceneManager.SetActiveScene(newScene);
            
            // Создаем террейн
            CreateTerrain();
            
            // Создаем КРАЗ
            CreateKraz();
            
            // Настраиваем камеру
            SetupCamera();
            
            // Создаем освещение
            SetupLighting();
            
            Debug.Log("✅ Тестовая сцена с КРАЗом создана и готова к запуску!");
            Debug.Log("🎮 Управление: E - двигатель, WASD - движение, Пробел - тормоз");
            Debug.Log("▶️ Нажмите Play для запуска!");
        }
        
        private static void CreateTerrain()
        {
            // Создаем GameObject для террейна
            GameObject terrainGO = new GameObject("Test Terrain");
            
            // Добавляем компонент Terrain
            Terrain terrain = terrainGO.AddComponent<Terrain>();
            TerrainCollider terrainCollider = terrainGO.AddComponent<TerrainCollider>();
            
            // Создаем TerrainData
            TerrainData terrainData = new TerrainData();
            terrainData.heightmapResolution = 513;
            terrainData.size = new Vector3(100, 1, 100);
            
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
            
            // Позиционируем террейн
            terrainGO.transform.position = new Vector3(-50, 0, -50);
            
            Debug.Log("🌍 Террейн создан");
        }
        
        private static void CreateKraz()
        {
            // Создаем КРАЗ
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
            kraz.transform.position = new Vector3(0, 2, 0);
            kraz.transform.rotation = Quaternion.identity;
            
            // Добавляем конвертер ECS
            var converter = kraz.AddComponent<KrazVehicleConverter>();
            converter.enablePlayerControl = true;
            converter.enableEngineSound = true;
            converter.enableMudEffects = true;
            
            Debug.Log("🚗 КРАЗ создан и готов к управлению!");
        }
        
        private static void CreateWheel(GameObject parent, string name, Vector3 position)
        {
            GameObject wheel = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            wheel.name = name;
            wheel.transform.SetParent(parent.transform);
            wheel.transform.localPosition = position;
            wheel.transform.localScale = new Vector3(1.2f, 0.2f, 1.2f);
            wheel.transform.localRotation = Quaternion.Euler(0, 0, 90);
            wheel.GetComponent<Renderer>().material.color = Color.black;
        }
        
        private static void SetupCamera()
        {
            // Находим основную камеру
            Camera mainCamera = Camera.main;
            if (mainCamera == null)
            {
                GameObject cameraGO = new GameObject("Main Camera");
                mainCamera = cameraGO.AddComponent<Camera>();
                cameraGO.tag = "MainCamera";
            }
            
            // Позиционируем камеру
            mainCamera.transform.position = new Vector3(-15, 8, 0);
            mainCamera.transform.rotation = Quaternion.Euler(20, 90, 0);
            
            Debug.Log("📷 Камера настроена");
        }
        
        private static void SetupLighting()
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
    }
}
