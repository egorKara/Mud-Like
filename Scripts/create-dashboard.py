#!/usr/bin/env python3
"""
Скрипт для создания dashboard мониторинга тестов и качества кода
"""

import os
import json
import sqlite3
from datetime import datetime, timedelta
from pathlib import Path
import matplotlib.pyplot as plt
import pandas as pd

class TestDashboard:
    def __init__(self, db_path="test_metrics.db", output_dir="dashboard"):
        self.db_path = db_path
        self.output_dir = Path(output_dir)
        self.output_dir.mkdir(exist_ok=True)
        
        # Инициализация базы данных
        self.init_database()
        
    def init_database(self):
        """Инициализация базы данных для метрик"""
        conn = sqlite3.connect(self.db_path)
        cursor = conn.cursor()
        
        # Создание таблицы для метрик тестов
        cursor.execute('''
            CREATE TABLE IF NOT EXISTS test_metrics (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                timestamp DATETIME DEFAULT CURRENT_TIMESTAMP,
                test_type TEXT NOT NULL,
                total_tests INTEGER,
                passed_tests INTEGER,
                failed_tests INTEGER,
                skipped_tests INTEGER,
                duration REAL,
                success_rate REAL,
                coverage_rate REAL,
                status TEXT
            )
        ''')
        
        # Создание таблицы для метрик качества кода
        cursor.execute('''
            CREATE TABLE IF NOT EXISTS code_quality_metrics (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                timestamp DATETIME DEFAULT CURRENT_TIMESTAMP,
                lines_of_code INTEGER,
                cyclomatic_complexity REAL,
                code_duplication REAL,
                technical_debt REAL,
                security_hotspots INTEGER,
                bugs INTEGER,
                vulnerabilities INTEGER,
                code_smells INTEGER
            )
        ''')
        
        # Создание таблицы для метрик производительности
        cursor.execute('''
            CREATE TABLE IF NOT EXISTS performance_metrics (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                timestamp DATETIME DEFAULT CURRENT_TIMESTAMP,
                build_time REAL,
                test_execution_time REAL,
                memory_usage REAL,
                cpu_usage REAL,
                build_size REAL
            )
        ''')
        
        conn.commit()
        conn.close()
        
    def add_test_metrics(self, test_type, total_tests, passed_tests, failed_tests, 
                        skipped_tests, duration, success_rate, coverage_rate, status):
        """Добавление метрик тестов в базу данных"""
        conn = sqlite3.connect(self.db_path)
        cursor = conn.cursor()
        
        cursor.execute('''
            INSERT INTO test_metrics 
            (test_type, total_tests, passed_tests, failed_tests, skipped_tests, 
             duration, success_rate, coverage_rate, status)
            VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?)
        ''', (test_type, total_tests, passed_tests, failed_tests, skipped_tests,
              duration, success_rate, coverage_rate, status))
        
        conn.commit()
        conn.close()
        
    def add_code_quality_metrics(self, lines_of_code, cyclomatic_complexity, 
                                code_duplication, technical_debt, security_hotspots,
                                bugs, vulnerabilities, code_smells):
        """Добавление метрик качества кода в базу данных"""
        conn = sqlite3.connect(self.db_path)
        cursor = conn.cursor()
        
        cursor.execute('''
            INSERT INTO code_quality_metrics 
            (lines_of_code, cyclomatic_complexity, code_duplication, technical_debt,
             security_hotspots, bugs, vulnerabilities, code_smells)
            VALUES (?, ?, ?, ?, ?, ?, ?, ?)
        ''', (lines_of_code, cyclomatic_complexity, code_duplication, technical_debt,
              security_hotspots, bugs, vulnerabilities, code_smells))
        
        conn.commit()
        conn.close()
        
    def add_performance_metrics(self, build_time, test_execution_time, 
                               memory_usage, cpu_usage, build_size):
        """Добавление метрик производительности в базу данных"""
        conn = sqlite3.connect(self.db_path)
        cursor = conn.cursor()
        
        cursor.execute('''
            INSERT INTO performance_metrics 
            (build_time, test_execution_time, memory_usage, cpu_usage, build_size)
            VALUES (?, ?, ?, ?, ?)
        ''', (build_time, test_execution_time, memory_usage, cpu_usage, build_size))
        
        conn.commit()
        conn.close()
        
    def generate_test_trends_chart(self):
        """Генерация графика трендов тестов"""
        conn = sqlite3.connect(self.db_path)
        
        # Получение данных за последние 30 дней
        query = '''
            SELECT timestamp, test_type, success_rate, coverage_rate
            FROM test_metrics
            WHERE timestamp >= datetime('now', '-30 days')
            ORDER BY timestamp
        '''
        
        df = pd.read_sql_query(query, conn)
        conn.close()
        
        if df.empty:
            print("Нет данных для генерации графика трендов тестов")
            return
            
        # Создание графика
        plt.figure(figsize=(12, 8))
        
        # График успешности тестов
        plt.subplot(2, 1, 1)
        for test_type in df['test_type'].unique():
            type_data = df[df['test_type'] == test_type]
            plt.plot(type_data['timestamp'], type_data['success_rate'], 
                    label=f'{test_type} - Success Rate', marker='o')
        
        plt.title('Test Success Rate Trends')
        plt.xlabel('Date')
        plt.ylabel('Success Rate (%)')
        plt.legend()
        plt.grid(True)
        plt.xticks(rotation=45)
        
        # График покрытия кода
        plt.subplot(2, 1, 2)
        for test_type in df['test_type'].unique():
            type_data = df[df['test_type'] == test_type]
            plt.plot(type_data['timestamp'], type_data['coverage_rate'], 
                    label=f'{test_type} - Coverage Rate', marker='s')
        
        plt.title('Code Coverage Trends')
        plt.xlabel('Date')
        plt.ylabel('Coverage Rate (%)')
        plt.legend()
        plt.grid(True)
        plt.xticks(rotation=45)
        
        plt.tight_layout()
        plt.savefig(self.output_dir / 'test_trends.png', dpi=300, bbox_inches='tight')
        plt.close()
        
    def generate_quality_metrics_chart(self):
        """Генерация графика метрик качества кода"""
        conn = sqlite3.connect(self.db_path)
        
        # Получение последних данных
        query = '''
            SELECT timestamp, lines_of_code, cyclomatic_complexity, 
                   code_duplication, technical_debt, security_hotspots,
                   bugs, vulnerabilities, code_smells
            FROM code_quality_metrics
            ORDER BY timestamp DESC
            LIMIT 30
        '''
        
        df = pd.read_sql_query(query, conn)
        conn.close()
        
        if df.empty:
            print("Нет данных для генерации графика метрик качества")
            return
            
        # Создание графика
        fig, axes = plt.subplots(2, 2, figsize=(15, 10))
        
        # График строк кода
        axes[0, 0].plot(df['timestamp'], df['lines_of_code'], marker='o')
        axes[0, 0].set_title('Lines of Code')
        axes[0, 0].set_ylabel('Lines')
        axes[0, 0].grid(True)
        axes[0, 0].tick_params(axis='x', rotation=45)
        
        # График цикломатической сложности
        axes[0, 1].plot(df['timestamp'], df['cyclomatic_complexity'], marker='s', color='orange')
        axes[0, 1].set_title('Cyclomatic Complexity')
        axes[0, 1].set_ylabel('Complexity')
        axes[0, 1].grid(True)
        axes[0, 1].tick_params(axis='x', rotation=45)
        
        # График дублирования кода
        axes[1, 0].plot(df['timestamp'], df['code_duplication'], marker='^', color='green')
        axes[1, 0].set_title('Code Duplication')
        axes[1, 0].set_ylabel('Duplication (%)')
        axes[1, 0].grid(True)
        axes[1, 0].tick_params(axis='x', rotation=45)
        
        # График технического долга
        axes[1, 1].plot(df['timestamp'], df['technical_debt'], marker='d', color='red')
        axes[1, 1].set_title('Technical Debt')
        axes[1, 1].set_ylabel('Debt (hours)')
        axes[1, 1].grid(True)
        axes[1, 1].tick_params(axis='x', rotation=45)
        
        plt.tight_layout()
        plt.savefig(self.output_dir / 'quality_metrics.png', dpi=300, bbox_inches='tight')
        plt.close()
        
    def generate_performance_chart(self):
        """Генерация графика метрик производительности"""
        conn = sqlite3.connect(self.db_path)
        
        # Получение данных за последние 30 дней
        query = '''
            SELECT timestamp, build_time, test_execution_time, 
                   memory_usage, cpu_usage, build_size
            FROM performance_metrics
            WHERE timestamp >= datetime('now', '-30 days')
            ORDER BY timestamp
        '''
        
        df = pd.read_sql_query(query, conn)
        conn.close()
        
        if df.empty:
            print("Нет данных для генерации графика производительности")
            return
            
        # Создание графика
        fig, axes = plt.subplots(2, 2, figsize=(15, 10))
        
        # График времени сборки
        axes[0, 0].plot(df['timestamp'], df['build_time'], marker='o', color='blue')
        axes[0, 0].set_title('Build Time')
        axes[0, 0].set_ylabel('Time (seconds)')
        axes[0, 0].grid(True)
        axes[0, 0].tick_params(axis='x', rotation=45)
        
        # График времени выполнения тестов
        axes[0, 1].plot(df['timestamp'], df['test_execution_time'], marker='s', color='green')
        axes[0, 1].set_title('Test Execution Time')
        axes[0, 1].set_ylabel('Time (seconds)')
        axes[0, 1].grid(True)
        axes[0, 1].tick_params(axis='x', rotation=45)
        
        # График использования памяти
        axes[1, 0].plot(df['timestamp'], df['memory_usage'], marker='^', color='orange')
        axes[1, 0].set_title('Memory Usage')
        axes[1, 0].set_ylabel('Memory (MB)')
        axes[1, 0].grid(True)
        axes[1, 0].tick_params(axis='x', rotation=45)
        
        # График размера сборки
        axes[1, 1].plot(df['timestamp'], df['build_size'], marker='d', color='red')
        axes[1, 1].set_title('Build Size')
        axes[1, 1].set_ylabel('Size (MB)')
        axes[1, 1].grid(True)
        axes[1, 1].tick_params(axis='x', rotation=45)
        
        plt.tight_layout()
        plt.savefig(self.output_dir / 'performance_metrics.png', dpi=300, bbox_inches='tight')
        plt.close()
        
    def generate_dashboard_html(self):
        """Генерация HTML dashboard"""
        html_content = f"""
<!DOCTYPE html>
<html lang="ru">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Mud-Like Test Dashboard</title>
    <style>
        body {{ font-family: Arial, sans-serif; margin: 20px; background-color: #f5f5f5; }}
        .header {{ background-color: #2c3e50; color: white; padding: 20px; border-radius: 5px; text-align: center; }}
        .dashboard {{ display: grid; grid-template-columns: repeat(auto-fit, minmax(400px, 1fr)); gap: 20px; margin: 20px 0; }}
        .card {{ background-color: white; padding: 20px; border-radius: 5px; box-shadow: 0 2px 5px rgba(0,0,0,0.1); }}
        .card h3 {{ color: #2c3e50; margin-top: 0; }}
        .metric {{ display: flex; justify-content: space-between; margin: 10px 0; }}
        .metric-value {{ font-weight: bold; }}
        .status-pass {{ color: #27ae60; }}
        .status-fail {{ color: #e74c3c; }}
        .status-warning {{ color: #f39c12; }}
        .chart {{ text-align: center; margin: 20px 0; }}
        .chart img {{ max-width: 100%; height: auto; border: 1px solid #ddd; border-radius: 5px; }}
        .footer {{ text-align: center; margin-top: 40px; color: #7f8c8d; }}
        .refresh {{ text-align: center; margin: 20px 0; }}
        .refresh button {{ background-color: #3498db; color: white; border: none; padding: 10px 20px; border-radius: 5px; cursor: pointer; }}
        .refresh button:hover {{ background-color: #2980b9; }}
    </style>
    <script>
        function refreshDashboard() {{
            location.reload();
        }}
        
        // Автообновление каждые 5 минут
        setInterval(refreshDashboard, 300000);
    </script>
</head>
<body>
    <div class="header">
        <h1>🚗 Mud-Like Test Dashboard</h1>
        <p>Мониторинг качества кода и тестирования</p>
        <p>Последнее обновление: {datetime.now().strftime('%Y-%m-%d %H:%M:%S')}</p>
    </div>
    
    <div class="refresh">
        <button onclick="refreshDashboard()">🔄 Обновить Dashboard</button>
    </div>
    
    <div class="dashboard">
        <div class="card">
            <h3>📊 Текущие метрики тестов</h3>
            {self._get_current_test_metrics()}
        </div>
        
        <div class="card">
            <h3>🎯 Качество кода</h3>
            {self._get_current_quality_metrics()}
        </div>
        
        <div class="card">
            <h3>⚡ Производительность</h3>
            {self._get_current_performance_metrics()}
        </div>
        
        <div class="card">
            <h3>📈 Тренды тестов</h3>
            <div class="chart">
                <img src="test_trends.png" alt="Test Trends" />
            </div>
        </div>
        
        <div class="card">
            <h3>🔍 Метрики качества</h3>
            <div class="chart">
                <img src="quality_metrics.png" alt="Quality Metrics" />
            </div>
        </div>
        
        <div class="card">
            <h3>🚀 Производительность</h3>
            <div class="chart">
                <img src="performance_metrics.png" alt="Performance Metrics" />
            </div>
        </div>
    </div>
    
    <div class="footer">
        <p>Mud-Like Project Dashboard | Сгенерировано автоматически</p>
    </div>
</body>
</html>
        """
        
        with open(self.output_dir / 'index.html', 'w', encoding='utf-8') as f:
            f.write(html_content)
            
    def _get_current_test_metrics(self):
        """Получение текущих метрик тестов"""
        conn = sqlite3.connect(self.db_path)
        cursor = conn.cursor()
        
        cursor.execute('''
            SELECT test_type, total_tests, passed_tests, failed_tests, 
                   success_rate, coverage_rate, status
            FROM test_metrics
            ORDER BY timestamp DESC
            LIMIT 5
        ''')
        
        results = cursor.fetchall()
        conn.close()
        
        if not results:
            return "<p>Нет данных о тестах</p>"
            
        html = ""
        for row in results:
            test_type, total, passed, failed, success_rate, coverage, status = row
            status_class = "status-pass" if status == "PASS" else "status-fail"
            
            html += f"""
            <div class="metric">
                <span>{test_type}</span>
                <span class="metric-value {status_class}">{status}</span>
            </div>
            <div class="metric">
                <span>Тестов: {total}</span>
                <span class="metric-value">Прошло: {passed}, Провалено: {failed}</span>
            </div>
            <div class="metric">
                <span>Успешность</span>
                <span class="metric-value">{success_rate:.1f}%</span>
            </div>
            <div class="metric">
                <span>Покрытие</span>
                <span class="metric-value">{coverage:.1f}%</span>
            </div>
            <hr>
            """
            
        return html
        
    def _get_current_quality_metrics(self):
        """Получение текущих метрик качества кода"""
        conn = sqlite3.connect(self.db_path)
        cursor = conn.cursor()
        
        cursor.execute('''
            SELECT lines_of_code, cyclomatic_complexity, code_duplication,
                   technical_debt, security_hotspots, bugs, vulnerabilities, code_smells
            FROM code_quality_metrics
            ORDER BY timestamp DESC
            LIMIT 1
        ''')
        
        result = cursor.fetchone()
        conn.close()
        
        if not result:
            return "<p>Нет данных о качестве кода</p>"
            
        loc, complexity, duplication, debt, hotspots, bugs, vulns, smells = result
        
        return f"""
        <div class="metric">
            <span>Строк кода</span>
            <span class="metric-value">{loc:,}</span>
        </div>
        <div class="metric">
            <span>Цикломатическая сложность</span>
            <span class="metric-value">{complexity:.1f}</span>
        </div>
        <div class="metric">
            <span>Дублирование кода</span>
            <span class="metric-value">{duplication:.1f}%</span>
        </div>
        <div class="metric">
            <span>Технический долг</span>
            <span class="metric-value">{debt:.1f} ч</span>
        </div>
        <div class="metric">
            <span>Проблемы безопасности</span>
            <span class="metric-value status-warning">{hotspots}</span>
        </div>
        <div class="metric">
            <span>Ошибки</span>
            <span class="metric-value status-fail">{bugs}</span>
        </div>
        <div class="metric">
            <span>Уязвимости</span>
            <span class="metric-value status-fail">{vulns}</span>
        </div>
        <div class="metric">
            <span>Запахи кода</span>
            <span class="metric-value status-warning">{smells}</span>
        </div>
        """
        
    def _get_current_performance_metrics(self):
        """Получение текущих метрик производительности"""
        conn = sqlite3.connect(self.db_path)
        cursor = conn.cursor()
        
        cursor.execute('''
            SELECT build_time, test_execution_time, memory_usage, cpu_usage, build_size
            FROM performance_metrics
            ORDER BY timestamp DESC
            LIMIT 1
        ''')
        
        result = cursor.fetchone()
        conn.close()
        
        if not result:
            return "<p>Нет данных о производительности</p>"
            
        build_time, test_time, memory, cpu, build_size = result
        
        return f"""
        <div class="metric">
            <span>Время сборки</span>
            <span class="metric-value">{build_time:.1f} с</span>
        </div>
        <div class="metric">
            <span>Время тестов</span>
            <span class="metric-value">{test_time:.1f} с</span>
        </div>
        <div class="metric">
            <span>Использование памяти</span>
            <span class="metric-value">{memory:.1f} МБ</span>
        </div>
        <div class="metric">
            <span>Использование CPU</span>
            <span class="metric-value">{cpu:.1f}%</span>
        </div>
        <div class="metric">
            <span>Размер сборки</span>
            <span class="metric-value">{build_size:.1f} МБ</span>
        </div>
        """
        
    def generate_all_charts(self):
        """Генерация всех графиков"""
        print("Генерация графиков...")
        self.generate_test_trends_chart()
        self.generate_quality_metrics_chart()
        self.generate_performance_chart()
        self.generate_dashboard_html()
        print(f"✅ Dashboard создан в папке: {self.output_dir}")

def main():
    """Основная функция"""
    dashboard = TestDashboard()
    
    # Добавление тестовых данных (для демонстрации)
    dashboard.add_test_metrics("Unit Tests", 35, 35, 0, 0, 45.2, 100.0, 85.5, "PASS")
    dashboard.add_test_metrics("Integration Tests", 3, 3, 0, 0, 12.8, 100.0, 82.3, "PASS")
    dashboard.add_code_quality_metrics(2500, 15.2, 5.8, 12.5, 2, 0, 0, 8)
    dashboard.add_performance_metrics(120.5, 58.0, 512.3, 45.2, 89.7)
    
    # Генерация dashboard
    dashboard.generate_all_charts()
    
    print("🎯 Dashboard готов!")
    print(f"📁 Откройте файл: {dashboard.output_dir}/index.html")

if __name__ == "__main__":
    main()
