using Unity.Entities;
using Unity.Burst;
using UnityEngine;
using UnityEngine.SceneManagement;
using MudLike.Core.Components;

namespace MudLike.Gameplay.Systems
{
    /// <summary>
    /// Система управления сценами
    /// Обрабатывает переключение между сценами и загрузку уровней
    /// </summary>
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class SceneManagementSystem : SystemBase
    {
        private SceneTransitionData _transitionData;
        private bool _isTransitioning = false;
        
        protected override void OnCreate()
        {
            _transitionData = new SceneTransitionData
            {
                TransitionDuration = 1f,
                FadeInDuration = 0.5f,
                FadeOutDuration = 0.5f,
                LoadingProgress = 0f
            };
        }
        
        protected override void OnUpdate()
        {
            // Обрабатываем переходы между сценами
            if (_isTransitioning)
            {
                ProcessSceneTransition();
            }
            
            // Обрабатываем команды перехода
            ProcessSceneCommands();
        }
        
        /// <summary>
        /// Обрабатывает переход между сценами
        /// </summary>
        private void ProcessSceneTransition()
        {
            _transitionData.TransitionTime += SystemAPI.Time.fixedDeltaTime;
            _transitionData.LoadingProgress = math.min(_transitionData.TransitionTime / _transitionData.TransitionDuration, 1f);
            
            // Завершаем переход
            if (_transitionData.LoadingProgress >= 1f)
            {
                CompleteSceneTransition();
            }
        }
        
        /// <summary>
        /// Обрабатывает команды перехода между сценами
        /// </summary>
        private void ProcessSceneCommands()
        {
            // Проверяем ввод игроков для команд
            Entities
                .WithAll<PlayerTag>()
                .ForEach((in PlayerInput input) =>
                {
                    // ESC - главное меню
                    if (Input.GetKeyDown(KeyCode.Escape))
                    {
                        LoadMainMenu();
                    }
                    
                    // R - перезагрузка сцены
                    if (Input.GetKeyDown(KeyCode.R))
                    {
                        ReloadCurrentScene();
                    }
                    
                    // 1-5 - загрузка уровней
                    if (Input.GetKeyDown(KeyCode.Alpha1))
                    {
                        LoadScene("Level01");
                    }
                    else if (Input.GetKeyDown(KeyCode.Alpha2))
                    {
                        LoadScene("Level02");
                    }
                    else if (Input.GetKeyDown(KeyCode.Alpha3))
                    {
                        LoadScene("Level03");
                    }
                    else if (Input.GetKeyDown(KeyCode.Alpha4))
                    {
                        LoadScene("Level04");
                    }
                    else if (Input.GetKeyDown(KeyCode.Alpha5))
                    {
                        LoadScene("Level05");
                    }
                    
                }).WithoutBurst().Run();
        }
        
        /// <summary>
        /// Загружает главное меню
        /// </summary>
        public void LoadMainMenu()
        {
            StartSceneTransition("MainMenu");
        }
        
        /// <summary>
        /// Загружает указанную сцену
        /// </summary>
        /// <param name="sceneName">Имя сцены для загрузки</param>
        public void LoadScene(string sceneName)
        {
            StartSceneTransition(sceneName);
        }
        
        /// <summary>
        /// Перезагружает текущую сцену
        /// </summary>
        public void ReloadCurrentScene()
        {
            string currentSceneName = SceneManager.GetActiveScene().name;
            StartSceneTransition(currentSceneName);
        }
        
        /// <summary>
        /// Загружает тестовую сцену с КРАЗом
        /// </summary>
        public void LoadKrazTestScene()
        {
            StartSceneTransition("KrazTest");
        }
        
        /// <summary>
        /// Начинает переход между сценами
        /// </summary>
        /// <param name="targetSceneName">Имя целевой сцены</param>
        private void StartSceneTransition(string targetSceneName)
        {
            if (_isTransitioning) return;
            
            _isTransitioning = true;
            _transitionData.TransitionTime = 0f;
            _transitionData.LoadingProgress = 0f;
            _transitionData.TargetSceneName = targetSceneName;
            
            // Сохраняем состояние игры перед переходом
            SaveGameState();
            
            // Начинаем анимацию затемнения
            StartFadeOut();
            
            Debug.Log($"Starting transition to scene: {targetSceneName}");
        }
        
        /// <summary>
        /// Завершает переход между сценами
        /// </summary>
        private void CompleteSceneTransition()
        {
            // Загружаем целевую сцену
            SceneManager.LoadScene(_transitionData.TargetSceneName);
            
            // Завершаем переход
            _isTransitioning = false;
            _transitionData.TransitionTime = 0f;
            _transitionData.LoadingProgress = 0f;
            
            // Начинаем анимацию появления
            StartFadeIn();
            
            // Восстанавливаем состояние игры
            LoadGameState();
            
            Debug.Log($"Completed transition to scene: {_transitionData.TargetSceneName}");
        }
        
        /// <summary>
        /// Сохраняет состояние игры перед переходом
        /// </summary>
        private void SaveGameState()
        {
            // Сохраняем позиции игроков
            Entities
                .WithAll<PlayerTag>()
                .ForEach((in LocalTransform transform, in NetworkId networkId) =>
                {
                    PlayerPrefs.SetFloat($"Player_{networkId.Value}_X", transform.Position.x);
                    PlayerPrefs.SetFloat($"Player_{networkId.Value}_Y", transform.Position.y);
                    PlayerPrefs.SetFloat($"Player_{networkId.Value}_Z", transform.Position.z);
                }).WithoutBurst().Run();
            
            PlayerPrefs.Save();
        }
        
        /// <summary>
        /// Восстанавливает состояние игры после перехода
        /// </summary>
        private void LoadGameState()
        {
            // Восстанавливаем позиции игроков
            Entities
                .WithAll<PlayerTag>()
                .ForEach((ref LocalTransform transform, in NetworkId networkId) =>
                {
                    float x = PlayerPrefs.GetFloat($"Player_{networkId.Value}_X", transform.Position.x);
                    float y = PlayerPrefs.GetFloat($"Player_{networkId.Value}_Y", transform.Position.y);
                    float z = PlayerPrefs.GetFloat($"Player_{networkId.Value}_Z", transform.Position.z);
                    
                    transform.Position = new float3(x, y, z);
                }).WithoutBurst().Run();
        }
        
        /// <summary>
        /// Начинает анимацию затемнения
        /// </summary>
        private void StartFadeOut()
        {
            // В реальной реализации здесь была бы анимация UI
            Debug.Log("Starting fade out animation");
        }
        
        /// <summary>
        /// Начинает анимацию появления
        /// </summary>
        private void StartFadeIn()
        {
            // В реальной реализации здесь была бы анимация UI
            Debug.Log("Starting fade in animation");
        }
    }
    
    /// <summary>
    /// Данные перехода между сценами
    /// </summary>
    public struct SceneTransitionData
    {
        public string TargetSceneName;       // Имя целевой сцены
        public float TransitionDuration;     // Длительность перехода
        public float FadeInDuration;         // Длительность появления
        public float FadeOutDuration;        // Длительность затемнения
        public float TransitionTime;         // Время с начала перехода
        public float LoadingProgress;        // Прогресс загрузки (0-1)
    }
}
