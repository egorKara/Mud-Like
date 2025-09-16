using Unity.Entities;
using Unity.Mathematics;

namespace MudLike.Networking.Components
{
    /// <summary>
    /// Компонент данных сетевого менеджера
    /// Содержит информацию о состоянии сети и соединения
    /// </summary>
    public struct NetworkManagerData : IComponentData
    {
        // Основные параметры соединения
        public bool IsConnected;          // Подключен ли к серверу
        public bool IsHost;               // Является ли хостом
        public bool IsServer;             // Является ли сервером
        public bool IsClient;             // Является ли клиентом
        
        // Параметры сети
        public int Ping;                  // Пинг в миллисекундах
        public float Bandwidth;           // Пропускная способность (бит/с)
        public float PacketLoss;          // Потеря пакетов (0-1)
        public float Jitter;              // Дрожание сети (мс)
        
        // Параметры синхронизации
        public float SyncRate;            // Частота синхронизации (Гц)
        public float InterpolationDelay;  // Задержка интерполяции (с)
        public float ExtrapolationTime;   // Время экстраполяции (с)
        
        // Параметры качества
        public int MaxPlayers;            // Максимальное количество игроков
        public int CurrentPlayers;        // Текущее количество игроков
        public float ServerTickRate;      // Частота тиков сервера (Гц)
        public float ClientTickRate;      // Частота тиков клиента (Гц)
        
        // Параметры оптимизации
        public bool EnableCompression;    // Включено ли сжатие
        public bool EnableEncryption;     // Включено ли шифрование
        public float CompressionRatio;    // Коэффициент сжатия
        public int MaxPacketSize;         // Максимальный размер пакета (байт)
        
        // Статистика сети
        public int PacketsSent;           // Отправлено пакетов
        public int PacketsReceived;       // Получено пакетов
        public int PacketsLost;           // Потеряно пакетов
        public float BytesSent;           // Отправлено байт
        public float BytesReceived;       // Получено байт
        
        // Временные параметры
        public float LastPingUpdate;      // Время последнего обновления пинга
        public float LastSyncTime;        // Время последней синхронизации
        public float ConnectionTime;      // Время подключения
        
        // Параметры отказоустойчивости
        public bool EnableLagCompensation; // Включена ли компенсация задержки
        public bool EnablePrediction;     // Включено ли предсказание
        public bool EnableReconciliation; // Включена ли реконсилиация
        public float MaxLagCompensation;  // Максимальная компенсация задержки (с)
        
        // Конструктор с значениями по умолчанию
        public static NetworkManagerData Default => new NetworkManagerData
        {
            IsConnected = false,
            IsHost = false,
            IsServer = false,
            IsClient = false,
            Ping = 0,
            Bandwidth = 1000000f,         // 1 Мбит/с
            PacketLoss = 0f,
            Jitter = 0f,
            SyncRate = 20f,               // 20 Гц
            InterpolationDelay = 0.1f,    // 100 мс
            ExtrapolationTime = 0.2f,     // 200 мс
            MaxPlayers = 50,
            CurrentPlayers = 0,
            ServerTickRate = 60f,         // 60 Гц
            ClientTickRate = 60f,         // 60 Гц
            EnableCompression = true,
            EnableEncryption = false,
            CompressionRatio = 0.7f,      // 70% сжатие
            MaxPacketSize = 1200,         // 1200 байт
            PacketsSent = 0,
            PacketsReceived = 0,
            PacketsLost = 0,
            BytesSent = 0f,
            BytesReceived = 0f,
            LastPingUpdate = 0f,
            LastSyncTime = 0f,
            ConnectionTime = 0f,
            EnableLagCompensation = true,
            EnablePrediction = true,
            EnableReconciliation = true,
            MaxLagCompensation = 0.5f     // 500 мс
        };
        
        /// <summary>
        /// Вычисляет качество соединения (0-1)
        /// </summary>
        public float GetConnectionQuality()
        {
            float pingFactor = if(math != null) math.max(0f, 1f - (Ping / 200f)); // Хороший пинг < 200мс
            float lossFactor = if(math != null) math.max(0f, 1f - PacketLoss * 10f); // Хорошая потеря < 10%
            float jitterFactor = if(math != null) math.max(0f, 1f - (Jitter / 50f)); // Хороший джиттер < 50мс
            
            return (pingFactor + lossFactor + jitterFactor) / 3f;
        }
        
        /// <summary>
        /// Проверяет, является ли соединение стабильным
        /// </summary>
        public bool IsConnectionStable()
        {
            return Ping < 100f && PacketLoss < 0.05f && Jitter < 30f;
        }
        
        /// <summary>
        /// Вычисляет эффективность использования пропускной способности
        /// </summary>
        public float GetBandwidthEfficiency()
        {
            if (Bandwidth <= 0f) return 0f;
            float totalBytes = BytesSent + BytesReceived;
            return totalBytes / Bandwidth;
        }
        
        /// <summary>
        /// Проверяет, нужна ли оптимизация сети
        /// </summary>
        public bool NeedsNetworkOptimization()
        {
            return Ping > 150f || PacketLoss > 0.1f || Jitter > 50f;
        }
    }
}
