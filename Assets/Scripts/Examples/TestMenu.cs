using UnityEngine;
using UnityEditor;

namespace MudLike.Examples
{
    /// <summary>
    /// Простой тест для проверки работы меню
    /// </summary>
    public class TestMenu
    {
        [MenuItem("Test/Hello World")]
        public static void HelloWorld()
        {
            Debug.Log("Hello World! Меню работает!");
        }
    }
}
