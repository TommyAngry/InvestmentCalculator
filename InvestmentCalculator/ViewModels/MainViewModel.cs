using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InvestmentCalculator.Models;
using Microsoft.EntityFrameworkCore;
using ScottPlot.TickGenerators.Financial;
using System;
using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks;
using System.Windows;

namespace InvestmentCalculator.ViewModels;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private double _initialAmount = 100000;

    [ObservableProperty]
    private double _monthlyContribution = 10000;

    [ObservableProperty]
    private double _interestRate = 12;

    [ObservableProperty]
    private int _years = 5;

    [ObservableProperty]
    private double _futureValue;

    [ObservableProperty]
    private double _totalInvested;

    [ObservableProperty]
    private double _totalInterest;

    public List<(int Year, double Value)> ChartData { get; set; } = new();

    [RelayCommand]
    private void Calculate()
    {
        try
        {
            ChartData.Clear();
            double rate = InterestRate / 100.0;
            TotalInvested = InitialAmount + MonthlyContribution * 12 * Years;

            if (rate > 0)
            {
                FutureValue = InitialAmount * Math.Pow(1 + rate, Years) +
                              (MonthlyContribution * 12) * (Math.Pow(1 + rate, Years) - 1) / rate;
            }
            else
            {
                FutureValue = InitialAmount + MonthlyContribution * 12 * Years;
            }

            FutureValue = Math.Round(FutureValue, 2);
            TotalInvested = Math.Round(TotalInvested, 2);
            TotalInterest = Math.Round(FutureValue - TotalInvested, 2);

            GenerateChartData(rate);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при расчёте: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void GenerateChartData(double rate)
    {
        ChartData.Clear();
        if (Years <= 0) return;

        for (int year = 1; year <= Years; year++)
        {
            double valueAtYear;
            if (rate > 0)
            {
                valueAtYear = InitialAmount * Math.Pow(1 + rate, year) +
                              (MonthlyContribution * 12) * (Math.Pow(1 + rate, year) - 1) / rate;
            }
            else
            {
                valueAtYear = InitialAmount + MonthlyContribution * 12 * year;
            }
            ChartData.Add((year, Math.Round(valueAtYear, 2)));
        }
        OnPropertyChanged(nameof(ChartData));
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (TotalInvested == 0 && TotalInterest == 0 && FutureValue == 0 && (InitialAmount != 0 || MonthlyContribution != 0))
        {
            Calculate();
        }

        var calculation = new Calculation
        {
            InitialAmount = InitialAmount,
            MonthlyContribution = MonthlyContribution,
            InterestRate = InterestRate,
            Years = Years,
            FutureValue = FutureValue,
            TotalInvested = TotalInvested,
            TotalInterest = TotalInterest
        };

        try
        {
            await using var db = new AppDbContext();
            await db.Calculations.AddAsync(calculation);
            await db.SaveChangesAsync();
            MessageBox.Show("Расчёт успешно сохранён!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при сохранении в БД: {ex.Message}", "Ошибка БД", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private void OpenHistory()
    {
        var historyWindow = new Views.HistoryWindow();
        historyWindow.ShowDialog();
    }
}
