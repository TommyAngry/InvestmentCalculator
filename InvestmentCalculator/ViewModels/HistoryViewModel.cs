using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InvestmentCalculator.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace InvestmentCalculator.ViewModels;

public partial class HistoryViewModel : ObservableObject
{
    // Приватное поле для коллекции
    private ObservableCollection<Calculation> _calculations = new();

    // Публичное свойство только для чтения (сама коллекция уведомляет об изменениях)
    public ObservableCollection<Calculation> Calculations => _calculations;

    // Поле и свойство для выбранной записи с уведомлением
    private Calculation? _selectedCalculation;
    public Calculation? SelectedCalculation
    {
        get => _selectedCalculation;
        set
        {
            if (_selectedCalculation != value)
            {
                _selectedCalculation = value;
                OnPropertyChanged();
            }
        }
    }

    // Команды (объявляем как свойства только для чтения)
    public IAsyncRelayCommand LoadDataAsyncCommand { get; }
    public IAsyncRelayCommand DeleteAsyncCommand { get; }
    public IAsyncRelayCommand RefreshAsyncCommand { get; }

    public HistoryViewModel()
    {
        // Инициализируем команды в конструкторе
        LoadDataAsyncCommand = new AsyncRelayCommand(LoadDataAsync);
        DeleteAsyncCommand = new AsyncRelayCommand(DeleteAsync);
        RefreshAsyncCommand = new AsyncRelayCommand(RefreshAsync);

        // Загружаем данные при создании ViewModel
        LoadDataAsyncCommand.ExecuteAsync(null);
    }

    // Загрузка данных из БД
    private async Task LoadDataAsync()
    {
        try
        {
            await using var db = new AppDbContext();
            var list = await db.Calculations
                .OrderByDescending(c => c.CalculationDate)
                .ToListAsync();

            _calculations.Clear();
            foreach (var item in list)
                _calculations.Add(item);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка загрузки истории: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    // Удаление выбранной записи
    private async Task DeleteAsync()
    {
        if (SelectedCalculation == null)
        {
            MessageBox.Show("Выберите запись для удаления.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var result = MessageBox.Show($"Удалить расчёт от {SelectedCalculation.CalculationDate.ToShortDateString()}?",
            "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (result != MessageBoxResult.Yes)
            return;

        try
        {
            await using var db = new AppDbContext();
            var itemToRemove = await db.Calculations.FindAsync(SelectedCalculation.Id);
            if (itemToRemove != null)
            {
                db.Calculations.Remove(itemToRemove);
                await db.SaveChangesAsync();
                _calculations.Remove(SelectedCalculation);
                SelectedCalculation = null;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    // Обновление списка (просто вызывает загрузку)
    private async Task RefreshAsync()
    {
        await LoadDataAsync();
    }
}