using Unity.Collections;

namespace MudLike.Core.Constants
{
    /// <summary>
    /// Константы для систем проекта Mud-Like
    /// </summary>
    public static class SystemConstants
    {
        // Размеры буферов для событий
        public const int EVENT_BUFFER_SIZE = 100;
        public const int LARGE_EVENT_BUFFER_SIZE = 1000;
        
        // Размеры буферов для физики
        public const int PHYSICS_BUFFER_SIZE = 512;
        public const int LARGE_PHYSICS_BUFFER_SIZE = 2048;
        
        // Размеры буферов для аудио
        public const int AUDIO_BUFFER_SIZE = 64;
        public const int LARGE_AUDIO_BUFFER_SIZE = 256;
        
        // Размеры буферов для рендеринга
        public const int RENDER_BUFFER_SIZE = 128;
        public const int LARGE_RENDER_BUFFER_SIZE = 1024;
        
        // Размеры буферов для сетевого взаимодействия
        public const int NETWORK_BUFFER_SIZE = 256;
        public const int LARGE_NETWORK_BUFFER_SIZE = 4096;
        
        // Размеры буферов для AI
        public const int AI_BUFFER_SIZE = 32;
        public const int LARGE_AI_BUFFER_SIZE = 128;
        
        // Размеры буферов для UI
        public const int UI_BUFFER_SIZE = 16;
        public const int LARGE_UI_BUFFER_SIZE = 64;
        
        // Размеры буферов для тестирования
        public const int TEST_BUFFER_SIZE = 8;
        public const int LARGE_TEST_BUFFER_SIZE = 32;
        
        // Константы для производительности
        public const int MAX_ENTITIES_PER_CHUNK = 128;
        public const int MAX_JOBS_PER_FRAME = 16;
        public const int MAX_MEMORY_ALLOCATIONS_PER_FRAME = 32;
        
        // Константы для физики
        public const float DEFAULT_GRAVITY = 9.81f;
        public const float DEFAULT_FRICTION = 0.7f;
        public const float DEFAULT_BOUNCE = 0.3f;
        
        // Константы для времени
        public const float DEFAULT_FIXED_DELTA_TIME = 0.02f;
        public const float DEFAULT_MAX_DELTA_TIME = 0.1f;
        
        // Константы для сетевого взаимодействия
        public const int DEFAULT_NETWORK_PORT = 7777;
        public const int MAX_NETWORK_CONNECTIONS = 32;
        public const float NETWORK_TIMEOUT = 30.0f;
        
        // Константы для аудио
        public const float DEFAULT_AUDIO_VOLUME = 1.0f;
        public const float DEFAULT_AUDIO_PITCH = 1.0f;
        public const int DEFAULT_AUDIO_SAMPLE_RATE = 44100;
        
        // Константы для рендеринга
        public const int DEFAULT_SCREEN_WIDTH = 1920;
        public const int DEFAULT_SCREEN_HEIGHT = 1080;
        public const float DEFAULT_FIELD_OF_VIEW = 60.0f;
        
        // Константы для AI
        public const float DEFAULT_AI_UPDATE_INTERVAL = 0.1f;
        public const int DEFAULT_AI_MEMORY_SIZE = 1024;
        public const float DEFAULT_AI_LEARNING_RATE = 0.01f;
        
        // Константы для UI
        public const float DEFAULT_UI_ANIMATION_DURATION = 0.3f;
        public const float DEFAULT_UI_FADE_DURATION = 0.5f;
        public const int DEFAULT_UI_MAX_ELEMENTS = 100;
        
        // Константы для тестирования
        public const int DEFAULT_TEST_ITERATIONS = 1000;
        public const float DEFAULT_TEST_TIMEOUT = 5.0f;
        public const int DEFAULT_TEST_SAMPLE_SIZE = 100;
    }
}
