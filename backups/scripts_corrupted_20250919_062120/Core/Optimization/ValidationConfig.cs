using System;
using System.Collections.Generic;
using UnityEngine;

namespace MudLike.Core.Optimization
{
    /// <summary>
    /// Конфигурация для системы валидации кода
    /// </summary>
    [CreateAssetMenu(fileName = "ValidationConfig", menuName = "Mud-Like/Validation Config")]
    public class ValidationConfig : ScriptableObject
    {
        [Header("Общие настройки")]
        [Tooltip("Включить автоматическую валидацию при компиляции")]
        public bool enableAutoValidation = true;
        
        [Tooltip("Включить автоматическое исправление проблем")]
        public bool enableAutoFix = false;
        
        [Tooltip("Показывать предупреждения как ошибки")]
        public bool treatWarningsAsErrors = false;
        
        [Header("Проверки компиляции")]
        [Tooltip("Проверять использование void в generic типах")]
        public bool checkVoidInGenerics = true;
        
        [Tooltip("Проверять небезопасный код без Burst")]
        public bool checkUnsafeWithoutBurst = true;
        
        [Tooltip("Проверять устаревшие Unity API")]
        public bool checkDeprecatedUnityAPI = true;
        
        [Tooltip("Проверять ECS best practices")]
        public bool checkECSBestPractices = true;
        
        [Header("Дополнительные проверки")]
        [Tooltip("Проверять производительность кода")]
        public bool checkPerformanceIssues = true;
        
        [Tooltip("Проверять безопасность памяти")]
        public bool checkMemorySafety = true;
        
        [Tooltip("Проверять соответствие coding standards")]
        public bool checkCodingStandards = true;
        
        [Header("Исключения")]
        [Tooltip("Файлы/папки для исключения из проверки")]
        public string[] excludedPaths = new string[]
        {
            "Assets/Scripts/Tests/",
            "Assets/Scripts/Examples/"
        };
        
        [Header("Настройки производительности")]
        [Tooltip("Максимальное время валидации (секунды)")]
        [Range(1f, 30f)]
        public float maxValidationTime = 10f;
        
        [Tooltip("Максимальное количество файлов для проверки за раз")]
        [Range(10, 1000)]
        public int maxFilesPerBatch = 100;
        
        /// <summary>
        /// Проверяет, должен ли файл быть исключен из валидации
        /// </summary>
        /// <param name="filePath">Путь к файлу</param>
        /// <returns>True если файл должен быть исключен</returns>
        public bool ShouldExcludeFile(string filePath)
        {
            if (excludedPaths == null) return false;
            
            foreach (var excludedPath in excludedPaths)
            {
                if (filePath.Contains(excludedPath))
                    return true;
            }
            
            return false;
        }
        
        /// <summary>
        /// Получает список активных проверок
        /// </summary>
        /// <returns>Список активных проверок</returns>
        public List<ValidationCheck> GetActiveChecks()
        {
            var checks = new List<ValidationCheck>();
            
            if (checkVoidInGenerics)
                checks.Add(ValidationCheck.VoidInGenerics);
            
            if (checkUnsafeWithoutBurst)
                checks.Add(ValidationCheck.UnsafeWithoutBurst);
            
            if (checkDeprecatedUnityAPI)
                checks.Add(ValidationCheck.DeprecatedUnityAPI);
            
            if (checkECSBestPractices)
                checks.Add(ValidationCheck.ECSBestPractices);
            
            if (checkPerformanceIssues)
                checks.Add(ValidationCheck.PerformanceIssues);
            
            if (checkMemorySafety)
                checks.Add(ValidationCheck.MemorySafety);
            
            if (checkCodingStandards)
                checks.Add(ValidationCheck.CodingStandards);
            
            return checks;
        }
    }
    
    /// <summary>
    /// Типы проверок валидации
    /// </summary>
    public enum ValidationCheck
    {
        VoidInGenerics,
        UnsafeWithoutBurst,
        DeprecatedUnityAPI,
        ECSBestPractices,
        PerformanceIssues,
        MemorySafety,
        CodingStandards
    }
    
    /// <summary>
    /// Результат валидации для статистики
    /// </summary>
    [Serializable]
    public class ValidationStats
    {
        public int totalFilesChecked;
        public int filesWithErrors;
        public int filesWithWarnings;
        public int totalErrors;
        public int totalWarnings;
        public float validationTime;
        public DateTime lastValidation;
        
        public float errorRate => totalFilesChecked > 0 ? (float)filesWithErrors / totalFilesChecked : 0f;
        public float warningRate => totalFilesChecked > 0 ? (float)filesWithWarnings / totalFilesChecked : 0f;
        
        public void Reset()
        {
            totalFilesChecked = 0;
            filesWithErrors = 0;
            filesWithWarnings = 0;
            totalErrors = 0;
            totalWarnings = 0;
            validationTime = 0f;
            lastValidation = DateTime.Now;
        }
    }
}
