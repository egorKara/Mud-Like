#!/usr/bin/env python3
"""
Бэкграунд агент для Unity Editor
Может запускать Unity в headless режиме и выполнять задачи
"""

import os
import sys
import subprocess
import time
import json
import signal
import threading
from datetime import datetime
from pathlib import Path
import logging

class UnityBackgroundAgent:
    def __init__(self, project_path="/home/egor/github/Mud-Like", unity_path=None):
        self.project_path = Path(project_path)
        self.unity_path = unity_path or self._find_unity_path()
        self.agent_id = f"unity-agent-{os.getpid()}"
        self.running = False
        self.tasks = []
        
        # Настройка логирования
        self.setup_logging()
        
        # Настройка окружения для headless режима
        self.setup_environment()
        
    def setup_logging(self):
        """Настройка логирования"""
        log_dir = self.project_path / "Logs" / "Agents"
        log_dir.mkdir(parents=True, exist_ok=True)
        
        logging.basicConfig(
            level=logging.INFO,
            format='%(asctime)s - %(name)s - %(levelname)s - %(message)s',
            handlers=[
                logging.FileHandler(log_dir / f"{self.agent_id}.log"),
                logging.StreamHandler()
            ]
        )
        self.logger = logging.getLogger(self.agent_id)
        
    def setup_environment(self):
        """Настройка окружения для headless режима"""
        # Установка переменных окружения для headless режима
        os.environ['DISPLAY'] = ':99'
        os.environ['UNITY_HEADLESS'] = '1'
        os.environ['UNITY_BATCHMODE'] = '1'
        os.environ['UNITY_QUIT'] = '1'
        
        # Создание виртуального дисплея если нужно
        self._ensure_virtual_display()
        
    def _find_unity_path(self):
        """Поиск пути к Unity Editor"""
        possible_paths = [
            "/opt/Unity/Editor/Unity",
            "/Applications/Unity/Unity.app/Contents/MacOS/Unity",
            "/usr/bin/Unity",
            "/usr/local/bin/Unity"
        ]
        
        # Поиск через which
        try:
            result = subprocess.run(['which', 'Unity'], capture_output=True, text=True)
            if result.returncode == 0:
                return result.stdout.strip()
        except:
            pass
            
        # Поиск по стандартным путям
        for path in possible_paths:
            if os.path.exists(path):
                return path
                
        # Поиск через Unity Hub
        try:
            hub_paths = [
                os.path.expanduser("~/Unity/Hub/Editor/*/Editor/Unity"),
                "/Applications/Unity Hub.app/Contents/MacOS/Unity Hub"
            ]
            for pattern in hub_paths:
                import glob
                matches = glob.glob(pattern)
                if matches:
                    return matches[0]
        except:
            pass
            
        self.logger.warning("Unity Editor не найден, попробуйте указать путь вручную")
        return None
        
    def _ensure_virtual_display(self):
        """Обеспечение наличия виртуального дисплея"""
        try:
            # Проверка, запущен ли Xvfb
            result = subprocess.run(['pgrep', '-f', 'Xvfb'], capture_output=True)
            if result.returncode != 0:
                self.logger.info("Запуск виртуального дисплея...")
                # Запуск Xvfb в фоне
                subprocess.Popen([
                    'Xvfb', ':99', '-screen', '0', '1024x768x24', '-ac'
                ], stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL)
                time.sleep(2)  # Даем время на запуск
        except Exception as e:
            self.logger.warning(f"Не удалось запустить виртуальный дисплей: {e}")
            
    def start(self):
        """Запуск агента"""
        self.running = True
        self.logger.info(f"Запуск бэкграунд агента: {self.agent_id}")
        
        # Регистрация обработчиков сигналов
        signal.signal(signal.SIGINT, self._signal_handler)
        signal.signal(signal.SIGTERM, self._signal_handler)
        
        # Запуск основного цикла
        self._main_loop()
        
    def stop(self):
        """Остановка агента"""
        self.running = False
        self.logger.info("Остановка бэкграунд агента")
        
    def _signal_handler(self, signum, frame):
        """Обработчик сигналов"""
        self.logger.info(f"Получен сигнал {signum}, остановка агента...")
        self.stop()
        
    def _main_loop(self):
        """Основной цикл агента"""
        while self.running:
            try:
                # Проверка задач
                self._check_tasks()
                
                # Выполнение задач
                self._execute_tasks()
                
                # Ожидание
                time.sleep(5)
                
            except KeyboardInterrupt:
                break
            except Exception as e:
                self.logger.error(f"Ошибка в основном цикле: {e}")
                time.sleep(10)
                
    def _check_tasks(self):
        """Проверка новых задач"""
        tasks_file = self.project_path / "agent-tasks.json"
        
        if tasks_file.exists():
            try:
                with open(tasks_file, 'r') as f:
                    new_tasks = json.load(f)
                    
                for task in new_tasks:
                    if task not in self.tasks:
                        self.tasks.append(task)
                        self.logger.info(f"Новая задача: {task}")
                        
            except Exception as e:
                self.logger.error(f"Ошибка чтения задач: {e}")
                
    def _execute_tasks(self):
        """Выполнение задач"""
        for task in self.tasks[:]:  # Копия списка для безопасного удаления
            try:
                self._execute_task(task)
                self.tasks.remove(task)
            except Exception as e:
                self.logger.error(f"Ошибка выполнения задачи {task}: {e}")
                
    def _execute_task(self, task):
        """Выполнение конкретной задачи"""
        task_type = task.get('type')
        task_params = task.get('params', {})
        
        self.logger.info(f"Выполнение задачи: {task_type}")
        
        if task_type == 'build':
            self._execute_build(task_params)
        elif task_type == 'test':
            self._execute_test(task_params)
        elif task_type == 'compile':
            self._execute_compile(task_params)
        elif task_type == 'import':
            self._execute_import(task_params)
        else:
            self.logger.warning(f"Неизвестный тип задачи: {task_type}")
            
    def _execute_build(self, params):
        """Выполнение сборки"""
        platform = params.get('platform', 'Linux64')
        build_path = params.get('build_path', 'Builds')
        
        cmd = [
            self.unity_path,
            '-batchmode',
            '-quit',
            '-projectPath', str(self.project_path),
            '-buildTarget', platform,
            '-buildPath', str(self.project_path / build_path),
            '-logFile', str(self.project_path / "Logs" / f"build-{datetime.now().strftime('%Y%m%d-%H%M%S')}.log")
        ]
        
        self.logger.info(f"Выполнение сборки: {' '.join(cmd)}")
        result = subprocess.run(cmd, capture_output=True, text=True)
        
        if result.returncode == 0:
            self.logger.info("Сборка выполнена успешно")
        else:
            self.logger.error(f"Ошибка сборки: {result.stderr}")
            
    def _execute_test(self, params):
        """Выполнение тестов"""
        test_filter = params.get('filter', '')
        
        cmd = [
            self.unity_path,
            '-batchmode',
            '-quit',
            '-projectPath', str(self.project_path),
            '-runTests',
            '-testResults', str(self.project_path / "Logs" / f"test-results-{datetime.now().strftime('%Y%m%d-%H%M%S')}.xml"),
            '-logFile', str(self.project_path / "Logs" / f"test-{datetime.now().strftime('%Y%m%d-%H%M%S')}.log")
        ]
        
        if test_filter:
            cmd.extend(['-testFilter', test_filter])
            
        self.logger.info(f"Выполнение тестов: {' '.join(cmd)}")
        result = subprocess.run(cmd, capture_output=True, text=True)
        
        if result.returncode == 0:
            self.logger.info("Тесты выполнены успешно")
        else:
            self.logger.error(f"Ошибка тестов: {result.stderr}")
            
    def _execute_compile(self, params):
        """Выполнение компиляции"""
        cmd = [
            self.unity_path,
            '-batchmode',
            '-quit',
            '-projectPath', str(self.project_path),
            '-logFile', str(self.project_path / "Logs" / f"compile-{datetime.now().strftime('%Y%m%d-%H%M%S')}.log")
        ]
        
        self.logger.info(f"Выполнение компиляции: {' '.join(cmd)}")
        result = subprocess.run(cmd, capture_output=True, text=True)
        
        if result.returncode == 0:
            self.logger.info("Компиляция выполнена успешно")
        else:
            self.logger.error(f"Ошибка компиляции: {result.stderr}")
            
    def _execute_import(self, params):
        """Выполнение импорта ассетов"""
        asset_path = params.get('asset_path', '')
        
        cmd = [
            self.unity_path,
            '-batchmode',
            '-quit',
            '-projectPath', str(self.project_path),
            '-importPackage', asset_path,
            '-logFile', str(self.project_path / "Logs" / f"import-{datetime.now().strftime('%Y%m%d-%H%M%S')}.log")
        ]
        
        self.logger.info(f"Выполнение импорта: {' '.join(cmd)}")
        result = subprocess.run(cmd, capture_output=True, text=True)
        
        if result.returncode == 0:
            self.logger.info("Импорт выполнен успешно")
        else:
            self.logger.error(f"Ошибка импорта: {result.stderr}")
            
    def add_task(self, task_type, params=None):
        """Добавление задачи"""
        task = {
            'type': task_type,
            'params': params or {},
            'timestamp': datetime.now().isoformat(),
            'agent_id': self.agent_id
        }
        
        self.tasks.append(task)
        self.logger.info(f"Добавлена задача: {task_type}")
        
    def get_status(self):
        """Получение статуса агента"""
        return {
            'agent_id': self.agent_id,
            'running': self.running,
            'tasks_count': len(self.tasks),
            'unity_path': self.unity_path,
            'project_path': str(self.project_path),
            'timestamp': datetime.now().isoformat()
        }

def main():
    """Основная функция"""
    import argparse
    
    parser = argparse.ArgumentParser(description='Unity Background Agent')
    parser.add_argument('--project-path', default='/home/egor/github/Mud-Like',
                       help='Путь к проекту Unity')
    parser.add_argument('--unity-path', help='Путь к Unity Editor')
    parser.add_argument('--daemon', action='store_true',
                       help='Запуск в режиме демона')
    
    args = parser.parse_args()
    
    # Создание агента
    agent = UnityBackgroundAgent(args.project_path, args.unity_path)
    
    if args.daemon:
        # Запуск в режиме демона
        import daemon
        with daemon.DaemonContext():
            agent.start()
    else:
        # Обычный запуск
        try:
            agent.start()
        except KeyboardInterrupt:
            agent.stop()

if __name__ == "__main__":
    main()
