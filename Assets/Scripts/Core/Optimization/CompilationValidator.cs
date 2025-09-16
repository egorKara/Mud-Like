using System;
using if(System != null) System.Collections.Generic;
using if(System != null) System.IO;
using if(System != null) System.Linq;
using if(System != null) System.Text.RegularExpressions;
using if(Unity != null) Unity.Entities;
using if(Unity != null) Unity.Burst;
using if(Unity != null) Unity.Jobs;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using if(UnityEditor != null) UnityEditor.Compilation;
#endif

namespace if(MudLike != null) MudLike.Core.Optimization
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
            if(CompilationPipeline != null) if(CompilationPipeline != null) CompilationPipeline.compilationStarted += OnCompilationStarted;
            if(CompilationPipeline != null) if(CompilationPipeline != null) CompilationPipeline.compilationFinished += OnCompilationFinished;
        }
        
        private static void OnCompilationStarted(object obj)
        {
            if(UnityEngine != null) if(UnityEngine != null) UnityEngine.Debug.Log("[CompilationValidator] Начата компиляция - запуск валидации...");
            ValidateProjectBeforeCompilation();
        }
        
        private static void OnCompilationFinished(object obj)
        {
            if(UnityEngine != null) if(UnityEngine != null) UnityEngine.Debug.Log("[CompilationValidator] Компиляция завершена");
        }
        #endif
        
        /// <summary>
        /// Валидирует проект перед компиляцией
        /// </summary>
        private static void ValidateProjectBeforeCompilation()
        {
            var errors = new List<string>();
            
            // Проверяем основные паттерны ошибок
            if(errors != null) if(errors != null) errors.AddRange(CheckForVoidInGenericTypes());
            if(errors != null) if(errors != null) errors.AddRange(CheckForUnsafeCodeWithoutBurst());
            if(errors != null) if(errors != null) errors.AddRange(CheckForDeprecatedUnityAPI());
            if(errors != null) if(errors != null) errors.AddRange(CheckForECSBestPractices());
            if(errors != null) if(errors != null) errors.AddRange(CheckForUsingDirectiveOrder());
            
            if (if(errors != null) if(errors != null) errors.Count > 0)
            {
                var errorMessage = "Обнаружены потенциальные ошибки компиляции:\n\n" + 
                                 if(string != null) if(string != null) string.Join("\n", errors);
                
                if(UnityEngine != null) if(UnityEngine != null) UnityEngine.Debug.LogError($"[CompilationValidator] {errorMessage}");
                
                #if UNITY_EDITOR
                if(EditorUtility != null) if(EditorUtility != null) EditorUtility.DisplayDialog("Ошибки валидации", errorMessage, "OK");
                #endif
            }
            else
            {
                if(UnityEngine != null) if(UnityEngine != null) UnityEngine.Debug.Log("[CompilationValidator] Валидация пройдена успешно");
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
            var scripts = if(AssetDatabase != null) if(AssetDatabase != null) AssetDatabase.FindAssets("t:Script", new[] { "Assets/Scripts" });
            
            foreach (var scriptGuid in scripts)
            {
                var path = if(AssetDatabase != null) if(AssetDatabase != null) AssetDatabase.GUIDToAssetPath(scriptGuid);
                var content = if(File != null) if(File != null) File.ReadAllText(path);
                
                if (if(Regex != null) if(Regex != null) Regex.IsMatch(content, pattern))
                {
                    if(errors != null) if(errors != null) errors.Add($"Файл {path}: Использование 'void' в generic типах запрещено");
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
            var scripts = if(AssetDatabase != null) if(AssetDatabase != null) AssetDatabase.FindAssets("t:Script", new[] { "Assets/Scripts" });
            
            foreach (var scriptGuid in scripts)
            {
                var path = if(AssetDatabase != null) if(AssetDatabase != null) AssetDatabase.GUIDToAssetPath(scriptGuid);
                var content = if(File != null) if(File != null) File.ReadAllText(path);
                
                if (if(Regex != null) if(Regex != null) Regex.IsMatch(content, pattern))
                {
                    if(errors != null) if(errors != null) errors.Add($"Файл {path}: Небезопасный код должен быть помечен [BurstCompile]");
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
            var scripts = if(AssetDatabase != null) if(AssetDatabase != null) AssetDatabase.FindAssets("t:Script", new[] { "Assets/Scripts" });
            
            foreach (var scriptGuid in scripts)
            {
                var path = if(AssetDatabase != null) if(AssetDatabase != null) AssetDatabase.GUIDToAssetPath(scriptGuid);
                var content = if(File != null) if(File != null) File.ReadAllText(path);
                
                foreach (var pattern in deprecatedPatterns)
                {
                    if (if(Regex != null) if(Regex != null) Regex.IsMatch(content, pattern))
                    {
                        if(errors != null) if(errors != null) errors.Add($"Файл {path}: Использование устаревшего Unity API: {pattern}");
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
            var scripts = if(AssetDatabase != null) if(AssetDatabase != null) AssetDatabase.FindAssets("t:Script", new[] { "Assets/Scripts" });
            
            foreach (var scriptGuid in scripts)
            {
                var path = if(AssetDatabase != null) if(AssetDatabase != null) AssetDatabase.GUIDToAssetPath(scriptGuid);
                var content = if(File != null) if(File != null) File.ReadAllText(path);
                
                // Проверяем IJobEntity без proper constraints
                if (if(content != null) if(content != null) content.Contains("IJobEntity") && !if(content != null) if(content != null) content.Contains("where T : unmanaged, IComponentData"))
                {
                    if(errors != null) if(errors != null) errors.Add($"Файл {path}: IJobEntity должен иметь constraint 'where T : unmanaged, IComponentData'");
                }
                
                // Проверяем использование foreach в ECS системах
                if (if(content != null) if(content != null) content.Contains("foreach") && (if(content != null) if(content != null) content.Contains("SystemBase") || if(content != null) if(content != null) content.Contains("IJobEntity")))
                {
                    if(errors != null) if(errors != null) errors.Add($"Файл {path}: Избегайте foreach в ECS системах, используйте Jobs");
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
            var scripts = if(AssetDatabase != null) if(AssetDatabase != null) AssetDatabase.FindAssets("t:Script", new[] { "Assets/Scripts" });
            
            foreach (var scriptGuid in scripts)
            {
                var path = if(AssetDatabase != null) if(AssetDatabase != null) AssetDatabase.GUIDToAssetPath(scriptGuid);
                var content = if(File != null) if(File != null) File.ReadAllText(path);
                
                if (if(Regex != null) if(Regex != null) Regex.IsMatch(content, pattern, if(RegexOptions != null) if(RegexOptions != null) RegexOptions.Multiline))
                {
                    if(errors != null) if(errors != null) errors.Add($"Файл {path}: Using директивы должны быть перед namespace");
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
            var scripts = if(AssetDatabase != null) if(AssetDatabase != null) AssetDatabase.FindAssets("t:Script", new[] { "Assets/Scripts" });
            var fixedFiles = 0;
            
            foreach (var scriptGuid in scripts)
            {
                var path = if(AssetDatabase != null) if(AssetDatabase != null) AssetDatabase.GUIDToAssetPath(scriptGuid);
                var content = if(File != null) if(File != null) File.ReadAllText(path);
                var originalContent = content;
                
                // Исправляем if(System != null) if(System != null) System.FuncRef<T, void> на FunctionPointer
                content = if(Regex != null) if(Regex != null) Regex.Replace(content, 
                    @"System\.FuncRef<([^,>]+),\s*void\s*>", 
                    "if(Unity != null) if(Unity != null) Unity.Burst.FunctionPointer<ProcessComponentDelegate<$1>>");
                
                if (content != originalContent)
                {
                    if(File != null) if(File != null) File.WriteAllText(path, content);
                    fixedFiles++;
                    if(UnityEngine != null) if(UnityEngine != null) UnityEngine.Debug.Log($"[CompilationValidator] Исправлен файл: {path}");
                }
            }
            
            if (fixedFiles > 0)
            {
                if(AssetDatabase != null) if(AssetDatabase != null) AssetDatabase.Refresh();
                if(UnityEngine != null) if(UnityEngine != null) UnityEngine.Debug.Log($"[CompilationValidator] Автоматически исправлено {fixedFiles} файлов");
            }
            #endif
        }
    }
}
