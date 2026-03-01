using InvestmentCalculator.ViewModels;
using ScottPlot; // Для доступа к сигнатуре
using System;
using System.Linq;
using System.Windows;

namespace InvestmentCalculator
{
    public partial class MainWindow : Window
    {
        private MainViewModel? _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = DataContext as MainViewModel;
            if (_viewModel != null)
            {
                // Подписываемся на событие изменения всех свойств ViewModel
                _viewModel.PropertyChanged += OnViewModelPropertyChanged;
            }
        }

        private void OnViewModelPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            // Если изменились данные графика, обновляем его
            if (e.PropertyName == nameof(MainViewModel.ChartData) && _viewModel != null)
            {
                UpdatePlot(_viewModel.ChartData);
            }
        }

        private void UpdatePlot(System.Collections.Generic.List<(int Year, double Value)> data)
        {
            if (data == null || data.Count == 0) return;

            // Очищаем график
            InvestmentPlot.Plot.Clear();

            // Преобразуем данные в массивы для ScottPlot
            double[] xs = data.Select(d => (double)d.Year).ToArray();
            double[] ys = data.Select(d => d.Value).ToArray();

            // Строим линию
            InvestmentPlot.Plot.Add.Scatter(xs, ys);
            InvestmentPlot.Plot.Title("Рост инвестиций по годам");
            InvestmentPlot.Plot.XLabel("Год");
            InvestmentPlot.Plot.YLabel("Стоимость (₽)");

            // Автоматически подбираем масштаб
            InvestmentPlot.Plot.Axes.AutoScale();

            // Обновляем элемент управления
            InvestmentPlot.Refresh();
        }
    }
}