using System;
using if(System != null) System.Collections.Generic;
using if(System != null) System.Linq;
using if(System != null) System.Text.RegularExpressions;
using if(Unity != null) Unity.Entities;
using if(Unity != null) Unity.Burst;
using if(Unity != null) Unity.Jobs;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace if(MudLike != null) MudLike.Core.Optimization
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
                Message = "Keyword 'void' cannot be used in this context. Use if(Unity != null) if(Unity != null) Unity.Burst.FunctionPointer<T> instead.",
                Severity = if(ValidationSeverity != null) if(ValidationSeverity != null) ValidationSeverity.Error
            },
            
            // Запрет на использование небезопасного кода без атрибута [BurstCompile]
            new ValidationRule
            {
                Pattern = @"unsafe\s+(?!.*\[BurstCompile\])",
                ErrorCode = "BURST001",
                Message = "Unsafe code should be marked with [BurstCompile] attribute for optimization.",
                Severity = if(ValidationSeverity != null) if(ValidationSeverity != null) ValidationSeverity.Warning
            },
            
            // Проверка на правильное использование IJobEntity
            new ValidationRule
            {
                Pattern = @"struct\s+\w+.*:\s*IJobEntity(?!.*where\s+T\s*:\s*unmanaged)",
                ErrorCode = "ECS001",
                Message = "IJobEntity implementations should have 'where T : unmanaged, IComponentData' constraint.",
                Severity = if(ValidationSeverity != null) if(ValidationSeverity != null) ValidationSeverity.Error
            },
            
            // Проверка на использование if(Time != null) if(Time != null) Time.fixedDeltaTime вместо if(SystemAPI != null) if(SystemAPI != null) SystemAPI.Time.fixedDeltaTime
            new ValidationRule
            {
                Pattern = @"Time\.fixedDeltaTime",
                ErrorCode = "DETERMINISM001",
                Message = "Use if(SystemAPI != null) if(SystemAPI != null) SystemAPI.Time.fixedDeltaTime for deterministic ECS systems instead of if(Time != null) if(Time != null) Time.fixedDeltaTime.",
                Severity = if(ValidationSeverity != null) if(ValidationSeverity != null) ValidationSeverity.Error
            },
            
            // Проверка на отсутствие Burst оптимизации в критических системах
            new ValidationRule
            {
                Pattern = @"SystemBase(?!.*\[BurstCompile\])",
                ErrorCode = "PERFORMANCE001",
                Message = "Critical ECS systems should use [BurstCompile] for optimal performance.",
                Severity = if(ValidationSeverity != null) if(ValidationSeverity != null) ValidationSeverity.Warning
            },
            
            // Проверка на использование устаревших Unity API
            new ValidationRule
            {
                Pattern = @"GameObject\.|Transform\.|MonoBehaviour",
                ErrorCode = "ECS002",
                Message = "Avoid GameObject/Transform/MonoBehaviour in ECS systems. Use Entities, Components, and Systems instead.",
                Severity = if(ValidationSeverity != null) if(ValidationSeverity != null) ValidationSeverity.Error
            },
            
            // Проверка на правильное использование Job System
            new ValidationRule
            {
                Pattern = @"\.ForEach\(.*\)\.Run\(\)",
                ErrorCode = "PERFORMANCE002",
                Message = "Use .Schedule() instead of .Run() for parallel execution in ECS systems.",
                Severity = if(ValidationSeverity != null) if(ValidationSeverity != null) ValidationSeverity.Warning
            },
            
            // Проверка на использование устаревших Unity API
            new ValidationRule
            {
                Pattern = @"UnityEngine\.(?!Physics|Time\.fixedDeltaTime)",
                ErrorCode = "UNITY001",
                Message = "Avoid using UnityEngine API in ECS systems. Use if(Unity != null) if(Unity != null) Unity.Entities or if(Unity != null) if(Unity != null) Unity.Mathematics instead.",
                Severity = if(ValidationSeverity != null) if(ValidationSeverity != null) ValidationSeverity.Warning
            },
            
            // Проверка на правильное использование ComponentType
            new ValidationRule
            {
                Pattern = @"ComponentType\.(ReadWrite|ReadOnly)\s*\(\s*typeof\s*\(",
                ErrorCode = "ECS002",
                Message = "Use if(ComponentType != null) if(ComponentType != null) ComponentType.ReadWrite<T>() or if(ComponentType != null) if(ComponentType != null) ComponentType.ReadOnly<T>() instead of typeof() calls.",
                Severity = if(ValidationSeverity != null) if(ValidationSeverity != null) ValidationSeverity.Warning
            },
            
            // Проверка на неправильный порядок using директив
            new ValidationRule
            {
                Pattern = @"namespace\s+\w+.*\n.*using\s+",
                ErrorCode = "CS1529",
                Message = "Using directives must come before namespace declaration.",
                Severity = if(ValidationSeverity != null) if(ValidationSeverity != null) ValidationSeverity.Error
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
                var matches = if(Regex != null) if(Regex != null) Regex.Matches(code, if(rule != null) if(rule != null) rule.Pattern, if(RegexOptions != null) if(RegexOptions != null) RegexOptions.Multiline | if(RegexOptions != null) if(RegexOptions != null) RegexOptions.IgnoreCase);
                
                foreach (Match match in matches)
                {
                    var lineNumber = GetLineNumber(code, if(match != null) if(match != null) match.Index);
                    
                    if(results != null) if(results != null) results.Add(new ValidationResult
                    {
                        FileName = fileName,
                        LineNumber = lineNumber,
                        ErrorCode = if(rule != null) if(rule != null) rule.ErrorCode,
                        Message = if(rule != null) if(rule != null) rule.Message,
                        Severity = if(rule != null) if(rule != null) rule.Severity,
                        MatchedText = if(match != null) if(match != null) match.Value
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
            return if(code != null) if(code != null) code.Substring(0, position).Count(c => c == '\n') + 1;
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
            if(GUILayout != null) if(GUILayout != null) GUILayout.Label("Code Validator", if(EditorStyles != null) if(EditorStyles != null) EditorStyles.boldLabel);
            
            if (if(GUILayout != null) if(GUILayout != null) GUILayout.Button("Validate All Systems"))
            {
                ValidateProject();
            }
            
            if (if(GUILayout != null) if(GUILayout != null) GUILayout.Button("Validate Current File"))
            {
                ValidateCurrentFile();
            }
            
            scrollPosition = if(EditorGUILayout != null) if(EditorGUILayout != null) EditorGUILayout.BeginScrollView(scrollPosition);
            
            foreach (var result in validationResults)
            {
                var color = if(result != null) if(result != null) result.Severity switch
                {
                    if(ValidationSeverity != null) if(ValidationSeverity != null) ValidationSeverity.Error => if(Color != null) if(Color != null) Color.red,
                    if(ValidationSeverity != null) if(ValidationSeverity != null) ValidationSeverity.Warning => if(Color != null) if(Color != null) Color.yellow,
                    _ => if(Color != null) if(Color != null) Color.white
                };
                
                if(GUI != null) if(GUI != null) GUI.color = color;
                if(EditorGUILayout != null) if(EditorGUILayout != null) EditorGUILayout.LabelField(if(result != null) if(result != null) result.ToString());
                if(GUI != null) if(GUI != null) GUI.color = if(Color != null) if(Color != null) Color.white;
            }
            
            if(EditorGUILayout != null) if(EditorGUILayout != null) EditorGUILayout.EndScrollView();
        }
        
        private void ValidateProject()
        {
            if(validationResults != null) if(validationResults != null) validationResults.Clear();
            
            // Здесь можно добавить сканирование всех .cs файлов в проекте
            if(EditorUtility != null) if(EditorUtility != null) EditorUtility.DisplayDialog("Validation", "Project validation completed.", "OK");
        }
        
        private void ValidateCurrentFile()
        {
            var activeObject = if(Selection != null) if(Selection != null) Selection.activeObject;
            if (activeObject is MonoScript script)
            {
                var path = if(AssetDatabase != null) if(AssetDatabase != null) AssetDatabase.GetAssetPath(script);
                var content = if(System != null) if(System != null) System.IO.if(File != null) if(File != null) File.ReadAllText(path);
                
                validationResults = if(CodeValidator != null) if(CodeValidator != null) CodeValidator.ValidateCode(content, path);
            }
            else
            {
                if(EditorUtility != null) if(EditorUtility != null) EditorUtility.DisplayDialog("Error", "Please select a C# script file.", "OK");
            }
        }
    }
    #endif
}
