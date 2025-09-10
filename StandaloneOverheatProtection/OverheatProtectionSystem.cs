using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace StandaloneOverheatProtection
{
    /// <summary>
    /// Автономная система защиты от перегрева
    /// Работает независимо от Unity
    /// </summary>
    public class OverheatProtectionSystem
    {
        private static readonly object _lock = new object();
        private static OverheatProtectionSystem _instance;
        
        public static OverheatProtectionSystem Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                            _instance = new OverheatProtectionSystem();
                    }
                }
                return _instance;
            }
        }
        
        // Настройки температуры
        private const float WARNING_TEMP = 70f;      // Предупреждение
        private const float CRITICAL_TEMP = 80f;     // Критическая температура
        private const float EMERGENCY_TEMP = 90f;    // Экстренная температура
        
        // Настройки мониторинга
        private const int MONITOR_INTERVAL_MS = 2000;  // Интервал мониторинга (2 секунды)
        private const int EMERGENCY_COOLDOWN_MS = 10000; // Время охлаждения в экстренном режиме (10 секунд)
        
        // Состояние системы
        private bool _isRunning = false;
        private bool _isEmergencyMode = false;
        private CancellationTokenSource _cancellationTokenSource;
        private Task _monitoringTask;
        
        // Статистика
        private int _warningCount = 0;
        private int _criticalCount = 0;
        private int _emergencyCount = 0;
        private DateTime _lastEmergencyTime = DateTime.MinValue;
        
        // События
        public event Action<float> OnTemperatureWarning;
        public event Action<float> OnTemperatureCritical;
        public event Action<float> OnTemperatureEmergency;
        public event Action<string> OnSystemAction;
        
        private OverheatProtectionSystem() { }
        
        /// <summary>
        /// Запустить систему защиты от перегрева
        /// </summary>
        public void Start()
        {
            if (_isRunning)
            {
                Console.WriteLine("[OverheatProtection] Система уже запущена");
                return;
            }
            
            Console.WriteLine("[OverheatProtection] 🚨 Запуск системы защиты от перегрева...");
            
            _isRunning = true;
            _cancellationTokenSource = new CancellationTokenSource();
            _monitoringTask = Task.Run(MonitorTemperature, _cancellationTokenSource.Token);
            
            Console.WriteLine("[OverheatProtection] ✅ Система защиты от перегрева запущена");
        }
        
        /// <summary>
        /// Остановить систему защиты от перегрева
        /// </summary>
        public void Stop()
        {
            if (!_isRunning)
            {
                Console.WriteLine("[OverheatProtection] Система не запущена");
                return;
            }
            
            Console.WriteLine("[OverheatProtection] 🛑 Остановка системы защиты от перегрева...");
            
            _isRunning = false;
            _cancellationTokenSource?.Cancel();
            _monitoringTask?.Wait(5000); // Ждем завершения максимум 5 секунд
            
            Console.WriteLine("[OverheatProtection] ✅ Система защиты от перегрева остановлена");
        }
        
        /// <summary>
        /// Основной цикл мониторинга температуры
        /// </summary>
        private async Task MonitorTemperature()
        {
            Console.WriteLine("[OverheatProtection] Начало мониторинга температуры...");
            
            while (_isRunning && !_cancellationTokenSource.Token.IsCancellationRequested)
            {
                try
                {
                    var systemInfo = SystemInfoReader.Instance.GetSystemInfo();
                    float temperature = systemInfo.CPUTemperature;
                    
                    // Проверяем температуру и принимаем меры
                    CheckTemperature(temperature);
                    
                    // Если в экстренном режиме, ждем дольше
                    if (_isEmergencyMode)
                    {
                        await Task.Delay(EMERGENCY_COOLDOWN_MS, _cancellationTokenSource.Token);
                    }
                    else
                    {
                        await Task.Delay(MONITOR_INTERVAL_MS, _cancellationTokenSource.Token);
                    }
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("[OverheatProtection] Мониторинг отменен");
                    break;
                }
                catch (Exception e)
                {
                    Console.WriteLine($"[OverheatProtection] Ошибка мониторинга: {e.Message}");
                    await Task.Delay(MONITOR_INTERVAL_MS, _cancellationTokenSource.Token);
                }
            }
        }
        
        /// <summary>
        /// Проверить температуру и принять меры
        /// </summary>
        private void CheckTemperature(float temperature)
        {
            if (temperature <= 0f)
            {
                Console.WriteLine("[OverheatProtection] ⚠️ Не удалось получить температуру CPU");
                return;
            }
            
            Console.WriteLine($"[OverheatProtection] Температура CPU: {temperature:F1}°C");
            
            if (temperature >= EMERGENCY_TEMP)
            {
                HandleEmergencyTemperature(temperature);
            }
            else if (temperature >= CRITICAL_TEMP)
            {
                HandleCriticalTemperature(temperature);
            }
            else if (temperature >= WARNING_TEMP)
            {
                HandleWarningTemperature(temperature);
            }
            else
            {
                // Температура в норме
                if (_isEmergencyMode)
                {
                    Console.WriteLine("[OverheatProtection] 🌡️ Температура нормализовалась, выход из экстренного режима");
                    _isEmergencyMode = false;
                }
            }
        }
        
        /// <summary>
        /// Обработка предупреждающей температуры
        /// </summary>
        private void HandleWarningTemperature(float temperature)
        {
            _warningCount++;
            Console.WriteLine($"[OverheatProtection] ⚠️ ПРЕДУПРЕЖДЕНИЕ: Температура CPU {temperature:F1}°C (счетчик: {_warningCount})");
            
            OnTemperatureWarning?.Invoke(temperature);
            
            // Мягкие меры
            TakeSoftMeasures();
        }
        
        /// <summary>
        /// Обработка критической температуры
        /// </summary>
        private void HandleCriticalTemperature(float temperature)
        {
            _criticalCount++;
            Console.WriteLine($"[OverheatProtection] 🔥 КРИТИЧЕСКАЯ ТЕМПЕРАТУРА: {temperature:F1}°C (счетчик: {_criticalCount})");
            
            OnTemperatureCritical?.Invoke(temperature);
            
            // Агрессивные меры
            TakeAggressiveMeasures();
        }
        
        /// <summary>
        /// Обработка экстренной температуры
        /// </summary>
        private void HandleEmergencyTemperature(float temperature)
        {
            _emergencyCount++;
            _lastEmergencyTime = DateTime.Now;
            _isEmergencyMode = true;
            
            Console.WriteLine($"[OverheatProtection] 🚨 ЭКСТРЕННАЯ ТЕМПЕРАТУРА: {temperature:F1}°C (счетчик: {_emergencyCount})");
            
            OnTemperatureEmergency?.Invoke(temperature);
            
            // Экстренные меры
            TakeEmergencyMeasures();
        }
        
        /// <summary>
        /// Мягкие меры защиты
        /// </summary>
        private void TakeSoftMeasures()
        {
            Console.WriteLine("[OverheatProtection] Применение мягких мер защиты...");
            
            // Ограничение процессов
            try
            {
                // Снижаем приоритет тяжелых процессов
                var processes = Process.GetProcesses();
                foreach (var process in processes)
                {
                    try
                    {
                        if (process.ProcessName.Contains("Unity") || 
                            process.ProcessName.Contains("Cursor") ||
                            process.ProcessName.Contains("code"))
                        {
                            if (process.PriorityClass != ProcessPriorityClass.BelowNormal)
                            {
                                process.PriorityClass = ProcessPriorityClass.BelowNormal;
                                Console.WriteLine($"[OverheatProtection] Снижен приоритет процесса: {process.ProcessName}");
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        // Игнорируем ошибки доступа к процессам
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"[OverheatProtection] Ошибка применения мягких мер: {e.Message}");
            }
            
            OnSystemAction?.Invoke("Soft measures applied");
        }
        
        /// <summary>
        /// Агрессивные меры защиты
        /// </summary>
        private void TakeAggressiveMeasures()
        {
            Console.WriteLine("[OverheatProtection] Применение агрессивных мер защиты...");
            
            // Принудительная сборка мусора
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            
            // Дополнительное снижение приоритета
            try
            {
                var currentProcess = Process.GetCurrentProcess();
                if (currentProcess.PriorityClass != ProcessPriorityClass.BelowNormal)
                {
                    currentProcess.PriorityClass = ProcessPriorityClass.BelowNormal;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"[OverheatProtection] Ошибка изменения приоритета: {e.Message}");
            }
            
            OnSystemAction?.Invoke("Aggressive measures applied");
        }
        
        /// <summary>
        /// Экстренные меры защиты
        /// </summary>
        private void TakeEmergencyMeasures()
        {
            Console.WriteLine("[OverheatProtection] 🚨 ПРИМЕНЕНИЕ ЭКСТРЕННЫХ МЕР ЗАЩИТЫ!");
            
            // Принудительная пауза
            Console.WriteLine("[OverheatProtection] Принудительная пауза на 5 секунд...");
            Thread.Sleep(5000);
            
            // Максимальная сборка мусора
            for (int i = 0; i < 3; i++)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            
            // Снижение приоритета до минимума
            try
            {
                var currentProcess = Process.GetCurrentProcess();
                currentProcess.PriorityClass = ProcessPriorityClass.Idle;
            }
            catch (Exception e)
            {
                Console.WriteLine($"[OverheatProtection] Ошибка установки минимального приоритета: {e.Message}");
            }
            
            OnSystemAction?.Invoke("Emergency measures applied");
        }
        
        /// <summary>
        /// Получить статистику системы
        /// </summary>
        public SystemStats GetStats()
        {
            return new SystemStats
            {
                IsRunning = _isRunning,
                IsEmergencyMode = _isEmergencyMode,
                WarningCount = _warningCount,
                CriticalCount = _criticalCount,
                EmergencyCount = _emergencyCount,
                LastEmergencyTime = _lastEmergencyTime
            };
        }
        
        /// <summary>
        /// Сбросить статистику
        /// </summary>
        public void ResetStats()
        {
            _warningCount = 0;
            _criticalCount = 0;
            _emergencyCount = 0;
            _lastEmergencyTime = DateTime.MinValue;
            Console.WriteLine("[OverheatProtection] Статистика сброшена");
        }
    }
    
    /// <summary>
    /// Статистика системы защиты от перегрева
    /// </summary>
    public struct SystemStats
    {
        public bool IsRunning;
        public bool IsEmergencyMode;
        public int WarningCount;
        public int CriticalCount;
        public int EmergencyCount;
        public DateTime LastEmergencyTime;
    }
}