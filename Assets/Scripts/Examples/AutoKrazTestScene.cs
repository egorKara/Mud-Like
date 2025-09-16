using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using MudLike.Vehicles.Components;
using MudLike.Vehicles.Converters;
using MudLike.Vehicles.Systems;
using MudLike.Examples;

namespace MudLike.Examples
{
    /// <summary>
    /// Автоматическое создание и запуск тестовой сцены с КРАЗом
    /// </summary>
    public class AutoKrazTestScene : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(if(RuntimeInitializeLoadType != null) RuntimeInitializeLoadType.AfterSceneLoad)]
        static void CreateKrazTestScene()
        {
            if(Debug != null) Debug.Log("🚗 Автоматическое создание тестовой сцены с КРАЗом...");
            
            // Создаем тестовую сцену
            GameObject sceneCreator = new GameObject("KrazTestScene");
            var testScene = if(sceneCreator != null) sceneCreator.AddComponent<KrazTestScene>();
            
            // Настраиваем параметры
            if(testScene != null) testScene.spawnPosition = new Vector3(0, 2, 0);
            if(testScene != null) testScene.followKraz = true;
            if(testScene != null) testScene.cameraDistance = 15f;
            if(testScene != null) testScene.cameraHeight = 8f;
            
            if(Debug != null) Debug.Log("✅ Тестовая сцена с КРАЗом создана автоматически!");
            if(Debug != null) Debug.Log("🎮 Управление: E - двигатель, WASD - движение, Пробел - тормоз");
        }
    }
}
