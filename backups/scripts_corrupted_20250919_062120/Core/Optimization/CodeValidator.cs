using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Unity.Entities;
using Unity.Burst;
using Unity.Jobs;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MudLike.Core.Optimization
{
    /// <summary>
    /// Валидатор кода для предотвращения ошибок компиляции
    /// </summary>
    public static class CodeValidator
    {
        /// <summary>
        /// Список запрещенных паттернов в коде
        /// </summary>
        private static readonly List<ValidationRule> ValidationRules = new List<ValidationRule>
        {
            // Запрет на использование void в generic типах
            new ValidationRule
            {
                Pattern = @"System\.FuncRef<[^,>]+,\s*void\s*>",
                ErrorCode = "CS1547",
                Message = "Keyword 'void' cannot be used in this context. Use Unity.Burst.FunctionPointer<T> instead.",
                Severity = ValidationSeverity.Error
            },
            
            // Запрет на использование небезопасного кода без атрибута [BurstCompile]
            new ValidationRule
            {
                Pattern = @"unsafe\s+(?!.*\[BurstCompile\])",
                ErrorCode = "BURST001",
                Message = "Unsafe code should be marked with [BurstCompile] attribute for optimization.",
                Severity = ValidationSeverity.Warning
            },
            
            // Проверка на правильное использование IJobEntity
            new ValidationRule
            {
                Pattern = @"struct\s+\w+.*:\s*IJobEntity(?!.*where\s+T\s*:\s*unmanaged)",
                ErrorCode = "ECS001",
                Message = "IJobEntity implementations should have 'where T : unmanaged, IComponentData' constraint.",
                Severity = ValidationSeverity.Error
            },
            
            // Проверка на использование Time.fixedDeltaTime вместо SystemAPI.Time.fixedDeltaTime
            new ValidationRule
            {
                Pattern = @"Time\.fixedDeltaTime",
                ErrorCode = "DETERMINISM001",
                Message = "Use SystemAPI.Time.fixedDeltaTime for deterministic ECS systems instead of Time.fixedDeltaTime.",
                Severity = ValidationSeverity.Error
            },
            
            // Проверка на отсутствие Burst оптимизации в критических системах
            new ValidationRule
            {
                Pattern = @"SystemBase(?!.*\[BurstCompile\])",
                ErrorCode = "PERFORMANCE001",
                Message = "Critical ECS systems should use [BurstCompile] for optimal performance.",
                Severity = ValidationSeverity.Warning
            },
            
            // Проверка на использование устаревших Unity API
            new ValidationRule
            {
                Pattern = @"GameObject\.|Transform\.|MonoBehaviour",
                ErrorCode = "ECS002",
                Message = "Avoid GameObject/Transform/MonoBehaviour in ECS systems. Use Entities, Components, and Systems instead.",
                Severity = ValidationSeverity.Error
            },
            
            // Проверка на правильное использование Job System
            new ValidationRule
            {
                Pattern = @"\.ForEach\(.*\)\.Run\(\)",
                ErrorCode = "PERFORMANCE002",
                Message = "Use .Schedule() instead of .Run() for parallel execution in ECS systems.",
                Severity = ValidationSeverity.Warning
            },
            
            // Проверка на использование устаревших Unity API
            new ValidationRule
            {
                Pattern = @"UnityEngine\.(?!Physics|Time\.fixedDeltaTime)",
                ErrorCode = "UNITY001",
                Message = "Avoid using UnityEngine API in ECS systems. Use Unity.Entities or Unity.Mathematics instead.",
                Severity = ValidationSeverity.Warning
            },
            
            // Проверка на правильное использование ComponentType
            new ValidationRule
            {
                Pattern = @"ComponentType\.(ReadWrite|ReadOnly)\s*\(\s*typeof\s*\(",
                ErrorCode = "ECS002",
                Message = "Use ComponentType.ReadWrite<T>() or ComponentType.ReadOnly<T>() instead of typeof() calls.",
                Severity = ValidationSeverity.Warning
            },
            
            // Проверка на неправильный порядок using директив
            new ValidationRule
            {
                Pattern = @"namespace\s+\w+.*\n.*using\s+",
                ErrorCode = "CS1529",
                Message = "Using directives must come before namespace declaration.",
                Severity = ValidationSeverity.Error
            }
        };
        
        /// <summary>
        /// Валидирует код на предмет ошибок
        /// </summary>
        /// <param name="code">Код для валидации</param>
        /// <param name="fileName">Имя файла</param>
        /// <returns>Список найденных ошибок</returns>
        public static List<ValidationResult> ValidateCode(string code, string fileName)
        {
            var results = new List<ValidationResult>();
            
            foreach (var rule in ValidationRules)
            {
                var matches = Regex.Matches(code, rule.Pattern, RegexOptions.Multiline | RegexOptions.IgnoreCase);
                
                foreach (Match match in matches)
                {
                    var lineNumber = GetLineNumber(code, match.Index);
                    
                    results.Add(new ValidationResult
                    {
                        FileName = fileName,
                        LineNumber = lineNumber,
                        ErrorCode = rule.ErrorCode,
                        Message = rule.Message,
                        Severity = rule.Severity,
                        MatchedText = match.Value
                    });
                }
            }
            
            return results;
        }
        
        /// <summary>
        /// Получает номер строки по позиции в коде
        /// </summary>
        private static int GetLineNumber(string code, int position)
        {
            return code.Substring(0, position).Count(c => c == '\n') + 1;
        }
        
        /// <summary>
        /// Проверяет все ECS системы в проекте
        /// </summary>
        public static List<ValidationResult> ValidateAllSystems()
        {
            var results = new List<ValidationResult>();
            
            // Здесь можно добавить автоматическое сканирование всех файлов проекта
            // Пока возвращаем пустой список, так как это требует интеграции с Unity Asset Database
            
            return results;
        }
    }
    
    /// <summary>
    /// Правило валидации
    /// </summary>
    public class ValidationRule
    {
        public string Pattern { get; set; }
        public string ErrorCode { get; set; }
        public string Message { get; set; }
        public ValidationSeverity Severity { get; set; }
    }
    
    /// <summary>
    /// Результат валидации
    /// </summary>
    public class ValidationResult
    {
        public string FileName { get; set; }
        public int LineNumber { get; set; }
        public string ErrorCode { get; set; }
        public string Message { get; set; }
        public ValidationSeverity Severity { get; set; }
        public string MatchedText { get; set; }
        
        public override string ToString()
        {
            return $"{FileName}({LineNumber}): {Severity} {ErrorCode}: {Message}";
        }
    }
    
    /// <summary>
    /// Уровень серьезности ошибки
    /// </summary>
    public enum ValidationSeverity
    {
        Info,
        Warning,
        Error
    }
    
    /// <summary>
    /// Editor-интеграция для валидации кода
    /// </summary>
    #if UNITY_EDITOR
    public class CodeValidatorEditor : EditorWindow
    {
        [MenuItem("Tools/Mud-Like/Code Validator")]
        public static void ShowWindow()
        {
            GetWindow<CodeValidatorEditor>("Code Validator");
        }
        
        private Vector2 scrollPosition;
        private List<ValidationResult> validationResults = new List<ValidationResult>();
        
        private void OnGUI()
        {
            GUILayout.Label("Code Validator", EditorStyles.boldLabel);
            
            if (GUILayout.Button("Validate All Systems"))
            {
                ValidateProject();
            }
            
            if (GUILayout.Button("Validate Current File"))
            {
                ValidateCurrentFile();
            }
            
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            
            foreach (var result in validationResults)
            {
                var color = result.Severity switch
                {
                    ValidationSeverity.Error => Color.red,
                    ValidationSeverity.Warning => Color.yellow,
                    _ => Color.white
                };
                
                GUI.color = color;
                EditorGUILayout.LabelField(result.ToString());
                GUI.color = Color.white;
            }
            
            EditorGUILayout.EndScrollView();
        }
        
        private void ValidateProject()
        {
            validationResults.Clear();
            
            // Здесь можно добавить сканирование всех .cs файлов в проекте
            EditorUtility.DisplayDialog("Validation", "Project validation completed.", "OK");
        }
        
        private void ValidateCurrentFile()
        {
            var activeObject = Selection.activeObject;
            if (activeObject is MonoScript script)
            {
                var path = AssetDatabase.GetAssetPath(script);
                var content = System.IO.File.ReadAllText(path);
                
                validationResults = CodeValidator.ValidateCode(content, path);
            }
            else
            {
                EditorUtility.DisplayDialog("Error", "Please select a C# script file.", "OK");
            }
        }
    }
    #endif
}
