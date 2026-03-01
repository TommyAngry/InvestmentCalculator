using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InvestmentCalculator.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;

namespace InvestmentCalculator.ViewModels;

public partial class HistoryViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<Calculation> _calculations = new();

    [ObservableProperty]
    private Calculation? _selectedCalculation;

    public HistoryViewModel()
    {
        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        try
        {
            await using var db = new AppDbContext();
            var list = await db.Calculations
                              .OrderByDescending(c => c.CalculationDate)
                              .ToListAsync();
            Calculations.Clear();
            foreach (var item in list)
                Calculations.Add(item);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка загрузки истории: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    [RelayCommand]
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
                Calculations.Remove(SelectedCalculation);
                SelectedCalculation = null;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        await LoadDataAsync();
    }
}