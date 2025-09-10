using System;
using System.Threading;
using System.Threading.Tasks;

namespace StandaloneOverheatProtection
{
    /// <summary>
    /// Главный класс автономной системы защиты от перегрева
    /// </summary>
    class Program
    {
        private static OverheatProtectionSystem _overheatSystem;
        private static bool _isRunning = true;
        
        static void Main(string[] args)
        {
            Console.WriteLine("🚨 АВТОНОМНАЯ СИСТЕМА ЗАЩИТЫ ОТ ПЕРЕГРЕВА НОУТБУКА 🚨");
            Console.WriteLine("==================================================");
            Console.WriteLine();
            
            // Инициализация системы
            _overheatSystem = OverheatProtectionSystem.Instance;
            
            // Подписка на события
            _overheatSystem.OnTemperatureWarning += OnTemperatureWarning;
            _overheatSystem.OnTemperatureCritical += OnTemperatureCritical;
            _overheatSystem.OnTemperatureEmergency += OnTemperatureEmergency;
            _overheatSystem.OnSystemAction += OnSystemAction;
            
            // Проверка доступности системных утилит
            CheckSystemUtilities();
            
            // Запуск системы
            _overheatSystem.Start();
            
            // Основной цикл приложения
            RunMainLoop();
            
            // Остановка системы
            _overheatSystem.Stop();
            
            Console.WriteLine("👋 Система защиты от перегрева завершена");
        }
        
        /// <summary>
        /// Проверить доступность системных утилит
        /// </summary>
        private static void CheckSystemUtilities()
        {
            Console.WriteLine("🔍 Проверка доступности системных утилит...");
            
            var utilities = SystemInfoReader.Instance.GetAvailableUtilities();
            Console.WriteLine($"Доступные утилиты: {utilities}");
            
            if (string.IsNullOrEmpty(utilities))
            {
                Console.WriteLine("⚠️ ВНИМАНИЕ: Системные утилиты не найдены! Система может работать некорректно.");
            }
            else
            {
                Console.WriteLine("✅ Системные утилиты найдены");
            }
            
            Console.WriteLine();
        }
        
        /// <summary>
        /// Основной цикл приложения
        /// </summary>
        private static void RunMainLoop()
        {
            Console.WriteLine("🎮 Управление системой:");
            Console.WriteLine("  'q' - Выход");
            Console.WriteLine("  's' - Показать статистику");
            Console.WriteLine("  'r' - Сбросить статистику");
            Console.WriteLine("  'i' - Показать информацию о системе");
            Console.WriteLine("  'h' - Показать справку");
            Console.WriteLine();
            
            while (_isRunning)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true);
                    
                    switch (key.KeyChar)
                    {
                        case 'q':
                        case 'Q':
                            Console.WriteLine("🛑 Завершение работы...");
                            _isRunning = false;
                            break;
                            
                        case 's':
                        case 'S':
                            ShowStats();
                            break;
                            
                        case 'r':
                        case 'R':
                            ResetStats();
                            break;
                            
                        case 'i':
                        case 'I':
                            ShowSystemInfo();
                            break;
                            
                        case 'h':
                        case 'H':
                            ShowHelp();
                            break;
                            
                        default:
                            // Игнорируем неизвестные клавиши
                            break;
                    }
                }
                
                Thread.Sleep(100); // Небольшая пауза для снижения нагрузки на CPU
            }
        }
        
        /// <summary>
        /// Показать статистику системы
        /// </summary>
        private static void ShowStats()
        {
            var stats = _overheatSystem.GetStats();
            
            Console.WriteLine();
            Console.WriteLine("📊 СТАТИСТИКА СИСТЕМЫ ЗАЩИТЫ ОТ ПЕРЕГРЕВА");
            Console.WriteLine("==========================================");
            Console.WriteLine($"Статус: {(stats.IsRunning ? "🟢 Активна" : "🔴 Остановлена")}");
            Console.WriteLine($"Экстренный режим: {(stats.IsEmergencyMode ? "🚨 ДА" : "✅ НЕТ")}");
            Console.WriteLine($"Предупреждения: {stats.WarningCount}");
            Console.WriteLine($"Критические температуры: {stats.CriticalCount}");
            Console.WriteLine($"Экстренные температуры: {stats.EmergencyCount}");
            
            if (stats.LastEmergencyTime != DateTime.MinValue)
            {
                Console.WriteLine($"Последняя экстренная ситуация: {stats.LastEmergencyTime:yyyy-MM-dd HH:mm:ss}");
            }
            else
            {
                Console.WriteLine("Экстренных ситуаций не было");
            }
            
            Console.WriteLine();
        }
        
        /// <summary>
        /// Сбросить статистику
        /// </summary>
        private static void ResetStats()
        {
            _overheatSystem.ResetStats();
            Console.WriteLine("✅ Статистика сброшена");
        }
        
        /// <summary>
        /// Показать информацию о системе
        /// </summary>
        private static void ShowSystemInfo()
        {
            Console.WriteLine();
            Console.WriteLine("💻 ИНФОРМАЦИЯ О СИСТЕМЕ");
            Console.WriteLine("=======================");
            
            var systemInfo = SystemInfoReader.Instance.GetSystemInfo();
            
            Console.WriteLine($"CPU: {systemInfo.CPUModel}");
            Console.WriteLine($"Ядра: {systemInfo.CPUCores}");
            Console.WriteLine($"Нагрузка CPU: {systemInfo.CPULoad:F1}%");
            Console.WriteLine($"Температура CPU: {systemInfo.CPUTemperature:F1}°C");
            Console.WriteLine($"RAM: {systemInfo.UsedRAM / 1024 / 1024}MB / {systemInfo.TotalRAM / 1024 / 1024}MB ({systemInfo.RAMUsage:F1}%)");
            Console.WriteLine($"Время работы системы: {systemInfo.Uptime:F1} часов");
            Console.WriteLine($"Средняя нагрузка: {systemInfo.LoadAverage:F2}");
            
            Console.WriteLine();
        }
        
        /// <summary>
        /// Показать справку
        /// </summary>
        private static void ShowHelp()
        {
            Console.WriteLine();
            Console.WriteLine("❓ СПРАВКА ПО СИСТЕМЕ ЗАЩИТЫ ОТ ПЕРЕГРЕВА");
            Console.WriteLine("==========================================");
            Console.WriteLine("Эта система автоматически мониторит температуру CPU и");
            Console.WriteLine("принимает меры для предотвращения перегрева ноутбука.");
            Console.WriteLine();
            Console.WriteLine("Уровни температуры:");
            Console.WriteLine("  ⚠️  Предупреждение: 70°C+ - Мягкие меры");
            Console.WriteLine("  🔥 Критическая: 80°C+ - Агрессивные меры");
            Console.WriteLine("  🚨 Экстренная: 90°C+ - Экстренные меры");
            Console.WriteLine();
            Console.WriteLine("Меры защиты:");
            Console.WriteLine("  • Снижение приоритета процессов");
            Console.WriteLine("  • Принудительная сборка мусора");
            Console.WriteLine("  • Принудительные паузы в экстренном режиме");
            Console.WriteLine();
        }
        
        /// <summary>
        /// Обработчик предупреждения о температуре
        /// </summary>
        private static void OnTemperatureWarning(float temperature)
        {
            Console.WriteLine($"⚠️ ПРЕДУПРЕЖДЕНИЕ: Температура CPU {temperature:F1}°C");
        }
        
        /// <summary>
        /// Обработчик критической температуры
        /// </summary>
        private static void OnTemperatureCritical(float temperature)
        {
            Console.WriteLine($"🔥 КРИТИЧЕСКАЯ ТЕМПЕРАТУРА: {temperature:F1}°C");
        }
        
        /// <summary>
        /// Обработчик экстренной температуры
        /// </summary>
        private static void OnTemperatureEmergency(float temperature)
        {
            Console.WriteLine($"🚨 ЭКСТРЕННАЯ ТЕМПЕРАТУРА: {temperature:F1}°C");
        }
        
        /// <summary>
        /// Обработчик действий системы
        /// </summary>
        private static void OnSystemAction(string action)
        {
            Console.WriteLine($"🔧 Действие системы: {action}");
        }
    }
}