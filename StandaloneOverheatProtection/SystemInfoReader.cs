using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;

namespace StandaloneOverheatProtection
{
    /// <summary>
    /// Автономный читатель системной информации
    /// Работает независимо от Unity
    /// </summary>
    public class SystemInfoReader
    {
        private static readonly object _lock = new object();
        private static SystemInfoReader _instance;
        
        public static SystemInfoReader Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                            _instance = new SystemInfoReader();
                    }
                }
                return _instance;
            }
        }
        
        private SystemInfoReader() { }
        
        /// <summary>
        /// Получить полную информацию о системе
        /// </summary>
        public SystemInfo GetSystemInfo()
        {
            var info = new SystemInfo();
            
            try
            {
                // Получаем информацию о CPU
                info.CPUModel = GetCPUModel();
                info.CPUCores = GetCPUCores();
                info.CPULoad = GetCPULoad();
                
                // Получаем информацию о памяти
                info.TotalRAM = GetTotalRAM();
                info.UsedRAM = GetUsedRAM();
                info.RAMUsage = info.TotalRAM > 0 ? (float)info.UsedRAM / info.TotalRAM * 100f : 0f;
                
                // Получаем информацию о температуре
                info.CPUTemperature = GetCPUTemperature();
                
                // Получаем информацию о системе
                info.Uptime = GetSystemUptime();
                info.LoadAverage = GetLoadAverage();
                
                Console.WriteLine($"[SystemInfo] CPU: {info.CPUModel}, Cores: {info.CPUCores}, Load: {info.CPULoad:F1}%");
                Console.WriteLine($"[SystemInfo] RAM: {info.UsedRAM / 1024 / 1024}MB/{info.TotalRAM / 1024 / 1024}MB ({info.RAMUsage:F1}%)");
                Console.WriteLine($"[SystemInfo] Temperature: {info.CPUTemperature:F1}°C, Uptime: {info.Uptime:F1}h");
            }
            catch (Exception e)
            {
                Console.WriteLine($"[SystemInfo] Ошибка получения информации о системе: {e.Message}");
            }
            
            return info;
        }
        
        /// <summary>
        /// Получить модель CPU
        /// </summary>
        private string GetCPUModel()
        {
            try
            {
                if (File.Exists("/proc/cpuinfo"))
                {
                    string[] lines = File.ReadAllLines("/proc/cpuinfo");
                    foreach (string line in lines)
                    {
                        if (line.StartsWith("model name"))
                        {
                            return line.Split(':')[1].Trim();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"[SystemInfo] Ошибка получения модели CPU: {e.Message}");
            }
            
            return "Unknown CPU";
        }
        
        /// <summary>
        /// Получить количество ядер CPU
        /// </summary>
        private int GetCPUCores()
        {
            try
            {
                if (File.Exists("/proc/cpuinfo"))
                {
                    string[] lines = File.ReadAllLines("/proc/cpuinfo");
                    int cores = 0;
                    foreach (string line in lines)
                    {
                        if (line.StartsWith("processor"))
                        {
                            cores++;
                        }
                    }
                    return cores > 0 ? cores : 1;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"[SystemInfo] Ошибка получения количества ядер: {e.Message}");
            }
            
            return 1;
        }
        
        /// <summary>
        /// Получить нагрузку CPU через htop
        /// </summary>
        private float GetCPULoad()
        {
            try
            {
                // Используем htop для получения точной нагрузки CPU
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = "htop",
                    Arguments = "-n 1 -d 1",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                };
                
                using (Process process = Process.Start(startInfo))
                {
                    string output = process.StandardOutput.ReadToEnd();
                    process.WaitForExit(5000); // Таймаут 5 секунд
                    
                    // Парсим вывод htop для получения нагрузки CPU
                    return ParseHtopOutput(output);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"[SystemInfo] Ошибка получения нагрузки CPU через htop: {e.Message}");
                
                // Fallback к /proc/loadavg
                return GetCPULoadFromProc();
            }
        }
        
        /// <summary>
        /// Получить нагрузку CPU из /proc/loadavg
        /// </summary>
        private float GetCPULoadFromProc()
        {
            try
            {
                if (File.Exists("/proc/loadavg"))
                {
                    string loadAvg = File.ReadAllText("/proc/loadavg");
                    string[] parts = loadAvg.Split(' ');
                    if (parts.Length > 0 && float.TryParse(parts[0], out float load1))
                    {
                        // Конвертируем load average в процент (приблизительно)
                        int cores = GetCPUCores();
                        return Math.Min(load1 / cores * 100f, 100f);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"[SystemInfo] Ошибка получения нагрузки из /proc/loadavg: {e.Message}");
            }
            
            return 0f;
        }
        
        /// <summary>
        /// Получить общий объем RAM
        /// </summary>
        private long GetTotalRAM()
        {
            try
            {
                if (File.Exists("/proc/meminfo"))
                {
                    string[] lines = File.ReadAllLines("/proc/meminfo");
                    foreach (string line in lines)
                    {
                        if (line.StartsWith("MemTotal"))
                        {
                            string[] parts = line.Split(':');
                            if (parts.Length > 1)
                            {
                                string memStr = parts[1].Trim().Replace("kB", "").Trim();
                                if (long.TryParse(memStr, out long memKB))
                                {
                                    return memKB * 1024; // Конвертируем в байты
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"[SystemInfo] Ошибка получения общего объема RAM: {e.Message}");
            }
            
            return 0;
        }
        
        /// <summary>
        /// Получить используемую RAM
        /// </summary>
        private long GetUsedRAM()
        {
            try
            {
                if (File.Exists("/proc/meminfo"))
                {
                    string[] lines = File.ReadAllLines("/proc/meminfo");
                    long memTotal = 0;
                    long memAvailable = 0;
                    
                    foreach (string line in lines)
                    {
                        if (line.StartsWith("MemTotal"))
                        {
                            string[] parts = line.Split(':');
                            if (parts.Length > 1)
                            {
                                string memStr = parts[1].Trim().Replace("kB", "").Trim();
                                if (long.TryParse(memStr, out long memKB))
                                {
                                    memTotal = memKB * 1024;
                                }
                            }
                        }
                        else if (line.StartsWith("MemAvailable"))
                        {
                            string[] parts = line.Split(':');
                            if (parts.Length > 1)
                            {
                                string memStr = parts[1].Trim().Replace("kB", "").Trim();
                                if (long.TryParse(memStr, out long memKB))
                                {
                                    memAvailable = memKB * 1024;
                                }
                            }
                        }
                    }
                    
                    return memTotal - memAvailable;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"[SystemInfo] Ошибка получения используемой RAM: {e.Message}");
            }
            
            return 0;
        }
        
        /// <summary>
        /// Получить температуру CPU
        /// </summary>
        private float GetCPUTemperature()
        {
            try
            {
                // Проверяем доступность thermal zones
                if (Directory.Exists("/sys/class/thermal"))
                {
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
                                if (tempCelsius > 0f && tempCelsius < 200f)
                                {
                                    Console.WriteLine($"[SystemInfo] Найдена температура: {tempCelsius:F1}°C");
                                    return tempCelsius;
                                }
                            }
                        }
                    }
                }
                
                // Fallback: оценка на основе нагрузки
                return EstimateTemperatureFromLoad();
            }
            catch (Exception e)
            {
                Console.WriteLine($"[SystemInfo] Ошибка получения температуры: {e.Message}");
                return EstimateTemperatureFromLoad();
            }
        }
        
        /// <summary>
        /// Получить время работы системы
        /// </summary>
        private float GetSystemUptime()
        {
            try
            {
                if (File.Exists("/proc/uptime"))
                {
                    string uptimeStr = File.ReadAllText("/proc/uptime");
                    string[] parts = uptimeStr.Split(' ');
                    if (parts.Length > 0 && float.TryParse(parts[0], out float uptimeSeconds))
                    {
                        return uptimeSeconds / 3600f; // Конвертируем в часы
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"[SystemInfo] Ошибка получения времени работы: {e.Message}");
            }
            
            return 0f;
        }
        
        /// <summary>
        /// Получить среднюю нагрузку системы
        /// </summary>
        private float GetLoadAverage()
        {
            try
            {
                if (File.Exists("/proc/loadavg"))
                {
                    string loadAvg = File.ReadAllText("/proc/loadavg");
                    string[] parts = loadAvg.Split(' ');
                    if (parts.Length > 0 && float.TryParse(parts[0], out float load1))
                    {
                        return load1;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"[SystemInfo] Ошибка получения средней нагрузки: {e.Message}");
            }
            
            return 0f;
        }
        
        /// <summary>
        /// Парсинг вывода htop
        /// </summary>
        private float ParseHtopOutput(string output)
        {
            try
            {
                // Ищем строку с нагрузкой CPU в выводе htop
                string[] lines = output.Split('\n');
                foreach (string line in lines)
                {
                    if (line.Contains("Cpu(s):") || line.Contains("CPU:"))
                    {
                        // Ищем процент нагрузки
                        Match match = Regex.Match(line, @"(\d+\.?\d*)%");
                        if (match.Success && float.TryParse(match.Groups[1].Value, out float cpuPercent))
                        {
                            return cpuPercent;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"[SystemInfo] Ошибка парсинга htop: {e.Message}");
            }
            
            return 0f;
        }
        
        /// <summary>
        /// Оценить температуру на основе нагрузки
        /// </summary>
        private float EstimateTemperatureFromLoad()
        {
            try
            {
                float cpuLoad = GetCPULoadFromProc();
                float baseTemp = 45f; // Базовая температура
                float loadFactor = cpuLoad / 100f;
                float estimatedTemp = baseTemp + (loadFactor * 25f);
                
                Console.WriteLine($"[SystemInfo] Оценочная температура: {estimatedTemp:F1}°C (нагрузка: {cpuLoad:F1}%)");
                return estimatedTemp;
            }
            catch
            {
                return 50f; // Безопасное значение по умолчанию
            }
        }
        
        /// <summary>
        /// Проверить доступность системных утилит
        /// </summary>
        public string GetAvailableUtilities()
        {
            var utilities = new System.Collections.Generic.List<string>();
            
            if (File.Exists("/usr/bin/htop")) utilities.Add("htop");
            if (File.Exists("/usr/bin/top")) utilities.Add("top");
            if (File.Exists("/proc/cpuinfo")) utilities.Add("/proc/cpuinfo");
            if (File.Exists("/proc/meminfo")) utilities.Add("/proc/meminfo");
            if (File.Exists("/proc/loadavg")) utilities.Add("/proc/loadavg");
            if (File.Exists("/proc/uptime")) utilities.Add("/proc/uptime");
            if (Directory.Exists("/sys/class/thermal")) utilities.Add("/sys/class/thermal");
            
            return string.Join(", ", utilities);
        }
    }
    
    /// <summary>
    /// Структура с информацией о системе
    /// </summary>
    public struct SystemInfo
    {
        public string CPUModel;
        public int CPUCores;
        public float CPULoad;
        public long TotalRAM;
        public long UsedRAM;
        public float RAMUsage;
        public float CPUTemperature;
        public float Uptime;
        public float LoadAverage;
    }
}