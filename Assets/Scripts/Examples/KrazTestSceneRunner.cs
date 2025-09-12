using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using MudLike.Vehicles.Components;
using MudLike.Vehicles.Converters;

namespace MudLike.Examples
{
    /// <summary>
    /// Запуск тестовой сцены с КРАЗом через компонент
    /// </summary>
    public class KrazTestSceneRunner : MonoBehaviour
    {
        [Header("🚗 Настройки КРАЗа")]
        public bool autoCreateScene = true;
        public Vector3 spawnPosition = new Vector3(0, 2, 0);
        
        void Start()
        {
            if (autoCreateScene)
            {
                CreateKrazTestScene();
            }
        }
        
        [ContextMenu("Создать тестовую сцену с КРАЗом")]
        public void CreateKrazTestScene()
        {
            Debug.Log("🚗 Создание тестовой сцены с КРАЗом...");
            
            // Создаем террейн
            CreateTerrain();
            
            // Создаем КРАЗ
            CreateKraz();
            
            // Настраиваем камеру
            SetupCamera();
            
            Debug.Log("✅ Тестовая сцена с КРАЗом создана!");
            Debug.Log("🎮 Управление: E - двигатель, WASD - движение, Пробел - тормоз");
        }
        
        private void CreateTerrain()
        {
            GameObject terrainGO = new GameObject("Test Terrain");
            
            Terrain terrain = terrainGO.AddComponent<Terrain>();
            TerrainCollider terrainCollider = terrainGO.AddComponent<TerrainCollider>();
            
            TerrainData terrainData = new TerrainData();
            terrainData.heightmapResolution = 513;
            terrainData.size = new Vector3(100, 1, 100);
            
            float[,] heights = new float[terrainData.heightmapResolution, terrainData.heightmapResolution];
            for (int x = 0; x < terrainData.heightmapResolution; x++)
            {
                for (int y = 0; y < terrainData.heightmapResolution; y++)
                {
                    heights[x, y] = 0.1f;
                }
            }
            terrainData.SetHeights(0, 0, heights);
            
            terrain.terrainData = terrainData;
            terrainCollider.terrainData = terrainData;
            
            terrainGO.transform.position = new Vector3(-50, 0, -50);
            
            Debug.Log("🌍 Террейн создан");
        }
        
        private void CreateKraz()
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
            
            // Колеса
            CreateWheel(kraz, "FrontLeft", new Vector3(-1.2f, -0.3f, 1.8f));
            CreateWheel(kraz, "FrontRight", new Vector3(1.2f, -0.3f, 1.8f));
            CreateWheel(kraz, "MiddleLeft", new Vector3(-1.2f, -0.3f, 0f));
            CreateWheel(kraz, "MiddleRight", new Vector3(1.2f, -0.3f, 0f));
            CreateWheel(kraz, "RearLeft", new Vector3(-1.2f, -0.3f, -1.8f));
            CreateWheel(kraz, "RearRight", new Vector3(1.2f, -0.3f, -1.8f));
            
            kraz.transform.position = spawnPosition;
            kraz.transform.rotation = Quaternion.identity;
            
            // Добавляем конвертер ECS
            var converter = kraz.AddComponent<KrazVehicleConverter>();
            converter.enablePlayerControl = true;
            converter.enableEngineSound = true;
            converter.enableMudEffects = true;
            
            Debug.Log("🚗 КРАЗ создан и готов к управлению!");
        }
        
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
        
        private void SetupCamera()
        {
            Camera mainCamera = Camera.main;
            if (mainCamera == null)
            {
                GameObject cameraGO = new GameObject("Main Camera");
                mainCamera = cameraGO.AddComponent<Camera>();
                cameraGO.tag = "MainCamera";
            }
            
            mainCamera.transform.position = new Vector3(-15, 8, 0);
            mainCamera.transform.rotation = Quaternion.Euler(20, 90, 0);
            
            Debug.Log("📷 Камера настроена");
        }
    }
}
