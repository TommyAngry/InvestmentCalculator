using CommunityToolkit.Mvvm.Input;
using InvestmentCalculator.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;

namespace InvestmentCalculator.ViewModels;

public class MainViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    // Приватные поля
    private double _initialAmount = 100000;
    private double _monthlyContribution = 10000;
    private double _interestRate = 12;
    private int _years = 5;
    private double _futureValue;
    private double _totalInvested;
    private double _totalInterest;

    // Публичные свойства с защитой от пустых строк
    public double InitialAmount
    {
        get => _initialAmount;
        set
        {
            if (_initialAmount != value)
            {
                _initialAmount = value;
                OnPropertyChanged();
            }
        }
    }

    public double MonthlyContribution
    {
        get => _monthlyContribution;
        set
        {
            if (_monthlyContribution != value)
            {
                _monthlyContribution = value;
                OnPropertyChanged();
            }
        }
    }

    public double InterestRate
    {
        get => _interestRate;
        set
        {
            if (_interestRate != value)
            {
                _interestRate = value;
                OnPropertyChanged();
            }
        }
    }

    public int Years
    {
        get => _years;
        set
        {
            // Защита от отрицательных значений и пустых строк (через привязку)
            if (value < 0) value = 0;
            if (_years != value)
            {
                _years = value;
                OnPropertyChanged();
            }
        }
    }

    public double FutureValue
    {
        get => _futureValue;
        set
        {
            if (_futureValue != value)
            {
                _futureValue = value;
                OnPropertyChanged();
            }
        }
    }

    public double TotalInvested
    {
        get => _totalInvested;
        set
        {
            if (_totalInvested != value)
            {
                _totalInvested = value;
                OnPropertyChanged();
            }
        }
    }

    public double TotalInterest
    {
        get => _totalInterest;
        set
        {
            if (_totalInterest != value)
            {
                _totalInterest = value;
                OnPropertyChanged();
            }
        }
    }

    // Данные для графика
    private List<(int Year, double Value)> _chartData = new();
    public List<(int Year, double Value)> ChartData
    {
        get => _chartData;
        set
        {
            if (_chartData != value)
            {
                _chartData = value;
                OnPropertyChanged();
            }
        }
    }

    // Команды
    public IRelayCommand CalculateCommand { get; }
    public IAsyncRelayCommand SaveAsyncCommand { get; }
    public IRelayCommand OpenHistoryCommand { get; }

    public MainViewModel()
    {
        CalculateCommand = new RelayCommand(Calculate);
        SaveAsyncCommand = new AsyncRelayCommand(SaveAsync);
        OpenHistoryCommand = new RelayCommand(OpenHistory);
    }

    private void Calculate()
    {
        try
        {
            // Очищаем старые данные графика
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
        var newData = new List<(int Year, double Value)>();
        if (Years <= 0)
        {
            ChartData = newData;
            return;
        }

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
            newData.Add((year, Math.Round(valueAtYear, 2)));
        }

        ChartData = newData; // через свойство, чтобы вызвать OnPropertyChanged
    }

    private async Task SaveAsync()
    {
        try
        {
            // Если расчёт не выполнялся, делаем его
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

    private void OpenHistory()
    {
        var historyWindow = new Views.HistoryWindow();
        historyWindow.ShowDialog();
    }
}