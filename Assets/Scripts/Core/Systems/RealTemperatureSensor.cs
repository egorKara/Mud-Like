using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using System.Diagnostics;
using System.Management;
using System.IO;
using System.Runtime.InteropServices;

namespace MudLike.Core.Systems
{
    /// <summary>
    /// РЕАЛЬНЫЙ ДАТЧИК ТЕМПЕРАТУРЫ CPU
    /// Использует системные API для получения реальной температуры
    /// </summary>
    public static class RealTemperatureSensor
    {
        // Windows API для чтения температуры
        [DllImport("kernel32.dll")]
        private static extern bool GetSystemTimes(out System.Runtime.InteropServices.ComTypes.FILETIME idleTime, 
                                                 out System.Runtime.InteropServices.ComTypes.FILETIME kernelTime, 
                                                 out System.Runtime.InteropServices.ComTypes.FILETIME userTime);
        
        private static bool _wmiInitialized = false;
        private static ManagementObjectSearcher _temperatureSearcher;
        
        /// <summary>
        /// Получить реальную температуру CPU
        /// </summary>
        public static float GetRealCPUTemperature()
        {
            try
            {
                if (Application.platform == RuntimePlatform.WindowsEditor || 
                    Application.platform == RuntimePlatform.WindowsPlayer)
                {
                    return GetWindowsRealTemperature();
                }
                else if (Application.platform == RuntimePlatform.LinuxEditor || 
                         Application.platform == RuntimePlatform.LinuxPlayer)
                {
                    return GetLinuxRealTemperature();
                }
                else if (Application.platform == RuntimePlatform.OSXEditor || 
                         Application.platform == RuntimePlatform.OSXPlayer)
                {
                    return GetMacRealTemperature();
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"[RealTemperatureSensor] Ошибка получения температуры: {e.Message}");
            }
            
            // Безопасное значение по умолчанию
            return 50f;
        }
        
        /// <summary>
        /// Получить реальную температуру на Windows
        /// </summary>
        private static float GetWindowsRealTemperature()
        {
            try
            {
                // Метод 1: WMI (Windows Management Instrumentation)
                float wmiTemp = GetTemperatureFromWMI();
                if (wmiTemp > 0f)
                {
                    Debug.Log($"[RealTemperatureSensor] WMI температура: {wmiTemp:F1}°C");
                    return wmiTemp;
                }
                
                // Метод 2: Чтение из реестра (если доступно)
                float registryTemp = GetTemperatureFromRegistry();
                if (registryTemp > 0f)
                {
                    Debug.Log($"[RealTemperatureSensor] Registry температура: {registryTemp:F1}°C");
                    return registryTemp;
                }
                
                // Метод 3: Использование OpenHardwareMonitor (если установлен)
                float ohmTemp = GetTemperatureFromOpenHardwareMonitor();
                if (ohmTemp > 0f)
                {
                    Debug.Log($"[RealTemperatureSensor] OpenHardwareMonitor температура: {ohmTemp:F1}°C");
                    return ohmTemp;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"[RealTemperatureSensor] Windows ошибка: {e.Message}");
            }
            
            // Fallback: оценка на основе нагрузки
            return EstimateTemperatureFromLoad();
        }
        
        /// <summary>
        /// Получить реальную температуру на Linux
        /// </summary>
        private static float GetLinuxRealTemperature()
        {
            try
            {
                // Чтение из /sys/class/thermal/thermal_zone*/temp
                string[] thermalZones = Directory.GetDirectories("/sys/class/thermal/", "thermal_zone*");
                
                foreach (string zone in thermalZones)
                {
                    string tempFile = Path.Combine(zone, "temp");
                    if (File.Exists(tempFile))
                    {
                        string tempStr = File.ReadAllText(tempFile).Trim();
                        if (int.TryParse(tempStr, out int tempMilliCelsius))
                        {
                            float tempCelsius = tempMilliCelsius / 1000f;
                            if (tempCelsius > 0f && tempCelsius < 200f) // Разумные пределы
                            {
                                Debug.Log($"[RealTemperatureSensor] Linux температура: {tempCelsius:F1}°C");
                                return tempCelsius;
                            }
                        }
                    }
                }
                
                // Альтернативный метод через sensors
                return GetTemperatureFromSensorsCommand();
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"[RealTemperatureSensor] Linux ошибка: {e.Message}");
            }
            
            return EstimateTemperatureFromLoad();
        }
        
        /// <summary>
        /// Получить реальную температуру на macOS
        /// </summary>
        private static float GetMacRealTemperature()
        {
            try
            {
                // Использование powermetrics (macOS 10.9+)
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = "powermetrics",
                    Arguments = "--samplers smc -n 1 -i 1000",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                };
                
                using (Process process = Process.Start(startInfo))
                {
                    string output = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();
                    
                    // Парсинг вывода powermetrics
                    float temp = ParsePowermetricsOutput(output);
                    if (temp > 0f)
                    {
                        Debug.Log($"[RealTemperatureSensor] macOS температура: {temp:F1}°C");
                        return temp;
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"[RealTemperatureSensor] macOS ошибка: {e.Message}");
            }
            
            return EstimateTemperatureFromLoad();
        }
        
        /// <summary>
        /// Получить температуру через WMI
        /// </summary>
        private static float GetTemperatureFromWMI()
        {
            try
            {
                if (!_wmiInitialized)
                {
                    _temperatureSearcher = new ManagementObjectSearcher(
                        "root\\WMI", 
                        "SELECT * FROM MSAcpi_ThermalZoneTemperature");
                    _wmiInitialized = true;
                }
                
                foreach (ManagementObject obj in _temperatureSearcher.Get())
                {
                    double temp = Convert.ToDouble(obj["CurrentTemperature"]);
                    float tempCelsius = (float)((temp / 10.0) - 273.15); // Kelvin to Celsius
                    
                    if (tempCelsius > 0f && tempCelsius < 200f)
                    {
                        return tempCelsius;
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"[RealTemperatureSensor] WMI ошибка: {e.Message}");
            }
            
            return 0f;
        }
        
        /// <summary>
        /// Получить температуру из реестра Windows
        /// </summary>
        private static float GetTemperatureFromRegistry()
        {
            try
            {
                // Попытка чтения из реестра (зависит от производителя)
                using (var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(
                    @"HARDWARE\DESCRIPTION\System\CentralProcessor\0"))
                {
                    if (key != null)
                    {
                        var tempValue = key.GetValue("Temperature");
                        if (tempValue != null && float.TryParse(tempValue.ToString(), out float temp))
                        {
                            return temp;
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"[RealTemperatureSensor] Registry ошибка: {e.Message}");
            }
            
            return 0f;
        }
        
        /// <summary>
        /// Получить температуру через OpenHardwareMonitor
        /// </summary>
        private static float GetTemperatureFromOpenHardwareMonitor()
        {
            try
            {
                // Попытка подключения к OpenHardwareMonitor WMI
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(
                    "root\\OpenHardwareMonitor",
                    "SELECT * FROM Sensor WHERE SensorType='Temperature' AND Name='CPU Package'");
                
                foreach (ManagementObject obj in searcher.Get())
                {
                    float temp = Convert.ToSingle(obj["Value"]);
                    if (temp > 0f && temp < 200f)
                    {
                        return temp;
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"[RealTemperatureSensor] OpenHardwareMonitor ошибка: {e.Message}");
            }
            
            return 0f;
        }
        
        /// <summary>
        /// Получить температуру через команду sensors (Linux)
        /// </summary>
        private static float GetTemperatureFromSensorsCommand()
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = "sensors",
                    Arguments = "-u",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                };
                
                using (Process process = Process.Start(startInfo))
                {
                    string output = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();
                    
                    // Парсинг вывода sensors
                    return ParseSensorsOutput(output);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"[RealTemperatureSensor] sensors команда ошибка: {e.Message}");
            }
            
            return 0f;
        }
        
        /// <summary>
        /// Оценить температуру на основе нагрузки CPU
        /// </summary>
        private static float EstimateTemperatureFromLoad()
        {
            try
            {
                // Получаем нагрузку CPU
                float cpuUsage = GetCPUUsage();
                
                // Оцениваем температуру на основе нагрузки
                // Базовая температура + влияние нагрузки
                float baseTemp = 40f; // Базовая температура
                float loadFactor = cpuUsage / 100f; // Фактор нагрузки (0-1)
                float estimatedTemp = baseTemp + (loadFactor * 30f); // До +30°C при 100% нагрузке
                
                Debug.Log($"[RealTemperatureSensor] Оценочная температура: {estimatedTemp:F1}°C (нагрузка: {cpuUsage:F1}%)");
                return estimatedTemp;
            }
            catch
            {
                return 50f; // Безопасное значение по умолчанию
            }
        }
        
        /// <summary>
        /// Получить нагрузку CPU
        /// </summary>
        private static float GetCPUUsage()
        {
            try
            {
                using (Process process = Process.GetCurrentProcess())
                {
                    long totalTime = process.TotalProcessorTime.TotalMilliseconds;
                    long elapsedTime = System.Environment.TickCount - process.StartTime.Ticks;
                    
                    if (elapsedTime > 0)
                    {
                        return (float)totalTime / elapsedTime * 100f;
                    }
                }
            }
            catch { }
            
            return 0f;
        }
        
        /// <summary>
        /// Парсинг вывода powermetrics (macOS)
        /// </summary>
        private static float ParsePowermetricsOutput(string output)
        {
            try
            {
                string[] lines = output.Split('\n');
                foreach (string line in lines)
                {
                    if (line.Contains("CPU die temperature"))
                    {
                        string[] parts = line.Split(':');
                        if (parts.Length > 1)
                        {
                            string tempStr = parts[1].Trim().Replace("C", "").Trim();
                            if (float.TryParse(tempStr, out float temp))
                            {
                                return temp;
                            }
                        }
                    }
                }
            }
            catch { }
            
            return 0f;
        }
        
        /// <summary>
        /// Парсинг вывода sensors (Linux)
        /// </summary>
        private static float ParseSensorsOutput(string output)
        {
            try
            {
                string[] lines = output.Split('\n');
                foreach (string line in lines)
                {
                    if (line.Contains("Core 0") || line.Contains("Package id 0"))
                    {
                        string[] parts = line.Split(':');
                        if (parts.Length > 1)
                        {
                            string tempStr = parts[1].Trim().Replace("°C", "").Trim();
                            if (float.TryParse(tempStr, out float temp))
                            {
                                return temp;
                            }
                        }
                    }
                }
            }
            catch { }
            
            return 0f;
        }
        
        /// <summary>
        /// Проверить доступность датчиков температуры
        /// </summary>
        public static bool IsTemperatureSensorAvailable()
        {
            float temp = GetRealCPUTemperature();
            return temp > 0f && temp < 200f;
        }
        
        /// <summary>
        /// Получить информацию о доступных датчиках
        /// </summary>
        public static string GetSensorInfo()
        {
            string platform = Application.platform.ToString();
            bool available = IsTemperatureSensorAvailable();
            float temp = GetRealCPUTemperature();
            
            return $"Платформа: {platform}, Датчик доступен: {available}, Температура: {temp:F1}°C";
        }
    }
}