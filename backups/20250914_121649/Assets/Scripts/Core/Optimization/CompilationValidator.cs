using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Unity.Entities;
using Unity.Burst;
using Unity.Jobs;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Compilation;
#endif

namespace MudLike.Core.Optimization
{
    /// <summary>
    /// Автоматический валидатор компиляции для предотвращения ошибок
    /// </summary>
    #if UNITY_EDITOR
    [InitializeOnLoad]
    #endif
    public static class CompilationValidator
    {
        #if UNITY_EDITOR
        static CompilationValidator()
        {
            // Подписываемся на события компиляции
            CompilationPipeline.compilationStarted += OnCompilationStarted;
            CompilationPipeline.compilationFinished += OnCompilationFinished;
        }
        
        private static void OnCompilationStarted(object obj)
        {
            UnityEngine.Debug.Log("[CompilationValidator] Начата компиляция - запуск валидации...");
            ValidateProjectBeforeCompilation();
        }
        
        private static void OnCompilationFinished(object obj)
        {
            UnityEngine.Debug.Log("[CompilationValidator] Компиляция завершена");
        }
        #endif
        
        /// <summary>
        /// Валидирует проект перед компиляцией
        /// </summary>
        private static void ValidateProjectBeforeCompilation()
        {
            var errors = new List<string>();
            
            // Проверяем основные паттерны ошибок
            errors.AddRange(CheckForVoidInGenericTypes());
            errors.AddRange(CheckForUnsafeCodeWithoutBurst());
            errors.AddRange(CheckForDeprecatedUnityAPI());
            errors.AddRange(CheckForECSBestPractices());
            errors.AddRange(CheckForUsingDirectiveOrder());
            
            if (errors.Count > 0)
            {
                var errorMessage = "Обнаружены потенциальные ошибки компиляции:\n\n" + 
                                 string.Join("\n", errors);
                
                UnityEngine.Debug.LogError($"[CompilationValidator] {errorMessage}");
                
                #if UNITY_EDITOR
                EditorUtility.DisplayDialog("Ошибки валидации", errorMessage, "OK");
                #endif
            }
            else
            {
                UnityEngine.Debug.Log("[CompilationValidator] Валидация пройдена успешно");
            }
        }
        
        /// <summary>
        /// Проверяет использование void в generic типах
        /// </summary>
        private static List<string> CheckForVoidInGenericTypes()
        {
            var errors = new List<string>();
            var pattern = @"System\.FuncRef<[^,>]+,\s*void\s*>";
            
            #if UNITY_EDITOR
            var scripts = AssetDatabase.FindAssets("t:Script", new[] { "Assets/Scripts" });
            
            foreach (var scriptGuid in scripts)
            {
                var path = AssetDatabase.GUIDToAssetPath(scriptGuid);
                var content = File.ReadAllText(path);
                
                if (Regex.IsMatch(content, pattern))
                {
                    errors.Add($"Файл {path}: Использование 'void' в generic типах запрещено");
                }
            }
            #endif
            
            return errors;
        }
        
        /// <summary>
        /// Проверяет небезопасный код без Burst атрибута
        /// </summary>
        private static List<string> CheckForUnsafeCodeWithoutBurst()
        {
            var errors = new List<string>();
            var pattern = @"unsafe\s+(?!.*\[BurstCompile\])";
            
            #if UNITY_EDITOR
            var scripts = AssetDatabase.FindAssets("t:Script", new[] { "Assets/Scripts" });
            
            foreach (var scriptGuid in scripts)
            {
                var path = AssetDatabase.GUIDToAssetPath(scriptGuid);
                var content = File.ReadAllText(path);
                
                if (Regex.IsMatch(content, pattern))
                {
                    errors.Add($"Файл {path}: Небезопасный код должен быть помечен [BurstCompile]");
                }
            }
            #endif
            
            return errors;
        }
        
        /// <summary>
        /// Проверяет использование устаревших Unity API
        /// </summary>
        private static List<string> CheckForDeprecatedUnityAPI()
        {
            var errors = new List<string>();
            var deprecatedPatterns = new[]
            {
                @"UnityEngine\.Transform",
                @"UnityEngine\.GameObject",
                @"UnityEngine\.MonoBehaviour",
                @"Time\.deltaTime"
            };
            
            #if UNITY_EDITOR
            var scripts = AssetDatabase.FindAssets("t:Script", new[] { "Assets/Scripts" });
            
            foreach (var scriptGuid in scripts)
            {
                var path = AssetDatabase.GUIDToAssetPath(scriptGuid);
                var content = File.ReadAllText(path);
                
                foreach (var pattern in deprecatedPatterns)
                {
                    if (Regex.IsMatch(content, pattern))
                    {
                        errors.Add($"Файл {path}: Использование устаревшего Unity API: {pattern}");
                    }
                }
            }
            #endif
            
            return errors;
        }
        
        /// <summary>
        /// Проверяет соблюдение ECS best practices
        /// </summary>
        private static List<string> CheckForECSBestPractices()
        {
            var errors = new List<string>();
            
            #if UNITY_EDITOR
            var scripts = AssetDatabase.FindAssets("t:Script", new[] { "Assets/Scripts" });
            
            foreach (var scriptGuid in scripts)
            {
                var path = AssetDatabase.GUIDToAssetPath(scriptGuid);
                var content = File.ReadAllText(path);
                
                // Проверяем IJobEntity без proper constraints
                if (content.Contains("IJobEntity") && !content.Contains("where T : unmanaged, IComponentData"))
                {
                    errors.Add($"Файл {path}: IJobEntity должен иметь constraint 'where T : unmanaged, IComponentData'");
                }
                
                // Проверяем использование foreach в ECS системах
                if (content.Contains("foreach") && (content.Contains("SystemBase") || content.Contains("IJobEntity")))
                {
                    errors.Add($"Файл {path}: Избегайте foreach в ECS системах, используйте Jobs");
                }
            }
            #endif
            
            return errors;
        }
        
        /// <summary>
        /// Проверяет порядок using директив
        /// </summary>
        private static List<string> CheckForUsingDirectiveOrder()
        {
            var errors = new List<string>();
            var pattern = @"namespace\s+\w+.*\n.*using\s+";
            
            #if UNITY_EDITOR
            var scripts = AssetDatabase.FindAssets("t:Script", new[] { "Assets/Scripts" });
            
            foreach (var scriptGuid in scripts)
            {
                var path = AssetDatabase.GUIDToAssetPath(scriptGuid);
                var content = File.ReadAllText(path);
                
                if (Regex.IsMatch(content, pattern, RegexOptions.Multiline))
                {
                    errors.Add($"Файл {path}: Using директивы должны быть перед namespace");
                }
            }
            #endif
            
            return errors;
        }
        
        /// <summary>
        /// Автоматически исправляет найденные проблемы
        /// </summary>
        public static void AutoFixCommonIssues()
        {
            #if UNITY_EDITOR
            var scripts = AssetDatabase.FindAssets("t:Script", new[] { "Assets/Scripts" });
            var fixedFiles = 0;
            
            foreach (var scriptGuid in scripts)
            {
                var path = AssetDatabase.GUIDToAssetPath(scriptGuid);
                var content = File.ReadAllText(path);
                var originalContent = content;
                
                // Исправляем System.FuncRef<T, void> на FunctionPointer
                content = Regex.Replace(content, 
                    @"System\.FuncRef<([^,>]+),\s*void\s*>", 
                    "Unity.Burst.FunctionPointer<ProcessComponentDelegate<$1>>");
                
                if (content != originalContent)
                {
                    File.WriteAllText(path, content);
                    fixedFiles++;
                    UnityEngine.Debug.Log($"[CompilationValidator] Исправлен файл: {path}");
                }
            }
            
            if (fixedFiles > 0)
            {
                AssetDatabase.Refresh();
                UnityEngine.Debug.Log($"[CompilationValidator] Автоматически исправлено {fixedFiles} файлов");
            }
            #endif
        }
    }
}
