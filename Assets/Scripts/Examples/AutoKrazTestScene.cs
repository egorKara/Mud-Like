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
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void CreateKrazTestScene()
        {
            Debug.Log("🚗 Автоматическое создание тестовой сцены с КРАЗом...");
            
            // Создаем тестовую сцену
            GameObject sceneCreator = new GameObject("KrazTestScene");
            var testScene = sceneCreator.AddComponent<KrazTestScene>();
            
            // Настраиваем параметры
            testScene.spawnPosition = new Vector3(0, 2, 0);
            testScene.followKraz = true;
            testScene.cameraDistance = 15f;
            testScene.cameraHeight = 8f;
            
            Debug.Log("✅ Тестовая сцена с КРАЗом создана автоматически!");
            Debug.Log("🎮 Управление: E - двигатель, WASD - движение, Пробел - тормоз");
        }
    }
}
