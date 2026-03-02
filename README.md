# InvestmentCalculator
![C#](https://img.shields.io/badge/c%23-%23239120.svg?style=for-the-badge&logo=csharp&logoColor=white)
![.Net](https://img.shields.io/badge/.NET-5C2D91?style=for-the-badge&logo=.net&logoColor=white)
![SQLite](https://img.shields.io/badge/sqlite-%2307405e.svg?style=for-the-badge&logo=sqlite&logoColor=white)

## Возможности
- Постройка графиков
- Отслеживание изменений
- Сохранение истории инвестиций
- Сохранение дат и значений каждую инвестицию

## Структура проекта
Проект построен по шаблону MVVM - Model View ViewModel
- Migrations:  файлы миграций БД
- Models:
    - AppDbContext.cs Конфигурация и настройка БД
    - Calculation.cs Описание сущности Calculation
- View: 
    - HistoryWindow xaml/xaml.cs: Интерфейс и логика взаимодействия
- ViewModels: 
    - HistoryViewModel.cs History **CRUD**
    - MainViewModel.cs Логика всего приложения
- MainWindow xaml/xaml.cs: Интерфейс, входные точки и начальное описание/инициализация сущностей приложения

## Сущности БД
### Calculation

| Поле                | Тип      | Описание                          |
|---------------------|----------|-----------------------------------|
| Id                  | int      | Первичный ключ                    |
| InitialAmount       | double   | Стартовый капитал                 |
| MonthlyContribution | double   | Ежемесячное пополнение            |
| InterestRate        | double   | Процентная ставка (%)             |
| Years               | int      | Срок инвестирования (лет)         |
| FutureValue         | double   | Итоговая сумма                    |
| TotalInvested       | double   | Всего вложено собственных средств |
| TotalInterest       | double   | Заработанные проценты             |
| CalculationDate     | DateTime | Дата проведения расчета           |
