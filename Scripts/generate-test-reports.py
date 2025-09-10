#!/usr/bin/env python3
"""
Скрипт для генерации отчетов о тестах и покрытии кода
"""

import os
import json
import xml.etree.ElementTree as ET
from datetime import datetime
from pathlib import Path

class TestReportGenerator:
    def __init__(self, artifacts_path="artifacts", output_path="reports"):
        self.artifacts_path = Path(artifacts_path)
        self.output_path = Path(output_path)
        self.output_path.mkdir(exist_ok=True)
        
    def generate_test_report(self):
        """Генерация отчета о тестах"""
        test_results = self._parse_test_results()
        coverage_results = self._parse_coverage_results()
        
        report = {
            "timestamp": datetime.now().isoformat(),
            "test_results": test_results,
            "coverage_results": coverage_results,
            "summary": self._generate_summary(test_results, coverage_results)
        }
        
        # Сохранение JSON отчета
        with open(self.output_path / "test-report.json", "w") as f:
            json.dump(report, f, indent=2)
            
        # Генерация HTML отчета
        self._generate_html_report(report)
        
        # Генерация Markdown отчета
        self._generate_markdown_report(report)
        
        return report
        
    def _parse_test_results(self):
        """Парсинг результатов тестов"""
        test_results = {
            "total_tests": 0,
            "passed": 0,
            "failed": 0,
            "skipped": 0,
            "duration": 0,
            "test_suites": []
        }
        
        # Поиск файлов результатов тестов
        for test_file in self.artifacts_path.glob("**/TestResults.xml"):
            try:
                tree = ET.parse(test_file)
                root = tree.getroot()
                
                # Парсинг результатов
                for test_suite in root.findall(".//testsuite"):
                    suite_data = {
                        "name": test_suite.get("name", "Unknown"),
                        "tests": int(test_suite.get("tests", 0)),
                        "failures": int(test_suite.get("failures", 0)),
                        "errors": int(test_suite.get("errors", 0)),
                        "skipped": int(test_suite.get("skipped", 0)),
                        "time": float(test_suite.get("time", 0)),
                        "test_cases": []
                    }
                    
                    # Парсинг тестовых случаев
                    for test_case in test_suite.findall(".//testcase"):
                        case_data = {
                            "name": test_case.get("name", "Unknown"),
                            "class": test_case.get("classname", "Unknown"),
                            "time": float(test_case.get("time", 0)),
                            "status": "passed"
                        }
                        
                        # Проверка на ошибки
                        if test_case.find("failure") is not None:
                            case_data["status"] = "failed"
                            case_data["failure"] = test_case.find("failure").text
                        elif test_case.find("error") is not None:
                            case_data["status"] = "error"
                            case_data["error"] = test_case.find("error").text
                        elif test_case.find("skipped") is not None:
                            case_data["status"] = "skipped"
                            
                        suite_data["test_cases"].append(case_data)
                    
                    test_results["test_suites"].append(suite_data)
                    
                    # Обновление общих счетчиков
                    test_results["total_tests"] += suite_data["tests"]
                    test_results["passed"] += suite_data["tests"] - suite_data["failures"] - suite_data["errors"] - suite_data["skipped"]
                    test_results["failed"] += suite_data["failures"] + suite_data["errors"]
                    test_results["skipped"] += suite_data["skipped"]
                    test_results["duration"] += suite_data["time"]
                    
            except Exception as e:
                print(f"Ошибка при парсинге {test_file}: {e}")
                
        return test_results
        
    def _parse_coverage_results(self):
        """Парсинг результатов покрытия кода"""
        coverage_results = {
            "total_coverage": 0,
            "line_coverage": 0,
            "branch_coverage": 0,
            "function_coverage": 0,
            "files": []
        }
        
        # Поиск файлов покрытия
        for coverage_file in self.artifacts_path.glob("**/coverage.xml"):
            try:
                tree = ET.parse(coverage_file)
                root = tree.getroot()
                
                # Парсинг общей статистики
                if root.tag == "coverage":
                    coverage_results["total_coverage"] = float(root.get("line-rate", 0)) * 100
                    coverage_results["line_coverage"] = float(root.get("line-rate", 0)) * 100
                    coverage_results["branch_coverage"] = float(root.get("branch-rate", 0)) * 100
                    coverage_results["function_coverage"] = float(root.get("function-rate", 0)) * 100
                    
                    # Парсинг файлов
                    for package in root.findall(".//package"):
                        for file_elem in package.findall(".//class"):
                            file_data = {
                                "name": file_elem.get("filename", "Unknown"),
                                "line_coverage": float(file_elem.get("line-rate", 0)) * 100,
                                "branch_coverage": float(file_elem.get("branch-rate", 0)) * 100,
                                "function_coverage": float(file_elem.get("function-rate", 0)) * 100
                            }
                            coverage_results["files"].append(file_data)
                            
            except Exception as e:
                print(f"Ошибка при парсинге {coverage_file}: {e}")
                
        return coverage_results
        
    def _generate_summary(self, test_results, coverage_results):
        """Генерация сводки"""
        total_tests = test_results["total_tests"]
        passed_tests = test_results["passed"]
        failed_tests = test_results["failed"]
        skipped_tests = test_results["skipped"]
        
        success_rate = (passed_tests / total_tests * 100) if total_tests > 0 else 0
        coverage_rate = coverage_results["total_coverage"]
        
        return {
            "success_rate": success_rate,
            "coverage_rate": coverage_rate,
            "status": "PASS" if failed_tests == 0 and coverage_rate >= 80 else "FAIL",
            "recommendations": self._generate_recommendations(test_results, coverage_results)
        }
        
    def _generate_recommendations(self, test_results, coverage_results):
        """Генерация рекомендаций"""
        recommendations = []
        
        if test_results["failed"] > 0:
            recommendations.append("Исправить проваленные тесты")
            
        if coverage_results["total_coverage"] < 80:
            recommendations.append("Увеличить покрытие кода до 80%")
            
        if test_results["skipped"] > 0:
            recommendations.append("Рассмотреть возможность реализации пропущенных тестов")
            
        if not recommendations:
            recommendations.append("Все тесты проходят успешно, покрытие кода в норме")
            
        return recommendations
        
    def _generate_html_report(self, report):
        """Генерация HTML отчета"""
        html_content = f"""
<!DOCTYPE html>
<html lang="ru">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Отчет о тестах - Mud-Like</title>
    <style>
        body {{ font-family: Arial, sans-serif; margin: 20px; }}
        .header {{ background-color: #f0f0f0; padding: 20px; border-radius: 5px; }}
        .summary {{ display: flex; gap: 20px; margin: 20px 0; }}
        .metric {{ background-color: #e8f4f8; padding: 15px; border-radius: 5px; flex: 1; }}
        .status-pass {{ color: green; font-weight: bold; }}
        .status-fail {{ color: red; font-weight: bold; }}
        .test-suite {{ margin: 20px 0; border: 1px solid #ddd; border-radius: 5px; }}
        .test-suite-header {{ background-color: #f5f5f5; padding: 10px; font-weight: bold; }}
        .test-case {{ padding: 5px 10px; border-bottom: 1px solid #eee; }}
        .test-case.passed {{ background-color: #d4edda; }}
        .test-case.failed {{ background-color: #f8d7da; }}
        .test-case.skipped {{ background-color: #fff3cd; }}
    </style>
</head>
<body>
    <div class="header">
        <h1>Отчет о тестах - Mud-Like</h1>
        <p>Сгенерировано: {report['timestamp']}</p>
    </div>
    
    <div class="summary">
        <div class="metric">
            <h3>Общая статистика</h3>
            <p>Всего тестов: {report['test_results']['total_tests']}</p>
            <p>Прошло: {report['test_results']['passed']}</p>
            <p>Провалено: {report['test_results']['failed']}</p>
            <p>Пропущено: {report['test_results']['skipped']}</p>
        </div>
        <div class="metric">
            <h3>Покрытие кода</h3>
            <p>Общее покрытие: {report['coverage_results']['total_coverage']:.1f}%</p>
            <p>Покрытие строк: {report['coverage_results']['line_coverage']:.1f}%</p>
            <p>Покрытие веток: {report['coverage_results']['branch_coverage']:.1f}%</p>
        </div>
        <div class="metric">
            <h3>Статус</h3>
            <p class="status-{'pass' if report['summary']['status'] == 'PASS' else 'fail'}">
                {report['summary']['status']}
            </p>
            <p>Успешность: {report['summary']['success_rate']:.1f}%</p>
        </div>
    </div>
    
    <h2>Рекомендации</h2>
    <ul>
        {''.join(f'<li>{rec}</li>' for rec in report['summary']['recommendations'])}
    </ul>
    
    <h2>Детали тестов</h2>
    {self._generate_test_suites_html(report['test_results']['test_suites'])}
</body>
</html>
        """
        
        with open(self.output_path / "test-report.html", "w", encoding="utf-8") as f:
            f.write(html_content)
            
    def _generate_test_suites_html(self, test_suites):
        """Генерация HTML для тестовых наборов"""
        html = ""
        for suite in test_suites:
            html += f"""
            <div class="test-suite">
                <div class="test-suite-header">
                    {suite['name']} - {suite['tests']} тестов, {suite['time']:.2f}с
                </div>
                {''.join(f'<div class="test-case {case["status"]}">{case["name"]} ({case["time"]:.3f}с)</div>' for case in suite['test_cases'])}
            </div>
            """
        return html
        
    def _generate_markdown_report(self, report):
        """Генерация Markdown отчета"""
        markdown_content = f"""# Отчет о тестах - Mud-Like

**Сгенерировано:** {report['timestamp']}

## 📊 Общая статистика

| Метрика | Значение |
|---------|----------|
| Всего тестов | {report['test_results']['total_tests']} |
| Прошло | {report['test_results']['passed']} |
| Провалено | {report['test_results']['failed']} |
| Пропущено | {report['test_results']['skipped']} |
| Успешность | {report['summary']['success_rate']:.1f}% |
| Время выполнения | {report['test_results']['duration']:.2f}с |

## 📈 Покрытие кода

| Тип покрытия | Процент |
|--------------|---------|
| Общее покрытие | {report['coverage_results']['total_coverage']:.1f}% |
| Покрытие строк | {report['coverage_results']['line_coverage']:.1f}% |
| Покрытие веток | {report['coverage_results']['branch_coverage']:.1f}% |
| Покрытие функций | {report['coverage_results']['function_coverage']:.1f}% |

## 🎯 Статус

**Статус:** {report['summary']['status']}

## 💡 Рекомендации

{chr(10).join(f'- {rec}' for rec in report['summary']['recommendations'])}

## 📋 Детали тестов

{self._generate_test_suites_markdown(report['test_results']['test_suites'])}
        """
        
        with open(self.output_path / "test-report.md", "w", encoding="utf-8") as f:
            f.write(markdown_content)
            
    def _generate_test_suites_markdown(self, test_suites):
        """Генерация Markdown для тестовых наборов"""
        markdown = ""
        for suite in test_suites:
            markdown += f"""
### {suite['name']}

- **Тестов:** {suite['tests']}
- **Время:** {suite['time']:.2f}с
- **Провалено:** {suite['failures']}
- **Ошибки:** {suite['errors']}
- **Пропущено:** {suite['skipped']}

#### Тестовые случаи

{chr(10).join(f'- {case["name"]} ({case["status"]}, {case["time"]:.3f}с)' for case in suite['test_cases'])}
            """
        return markdown

def main():
    """Основная функция"""
    generator = TestReportGenerator()
    report = generator.generate_test_report()
    
    print("✅ Отчет о тестах сгенерирован успешно!")
    print(f"📁 Файлы сохранены в: {generator.output_path}")
    print(f"📊 Статус: {report['summary']['status']}")
    print(f"🎯 Успешность: {report['summary']['success_rate']:.1f}%")
    print(f"📈 Покрытие: {report['summary']['coverage_rate']:.1f}%")

if __name__ == "__main__":
    main()
