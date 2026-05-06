using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Globalization;
using System.Windows.Data;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Controls;

namespace FundForest.Helpers
{
    // =============================================
    // RelayCommand
    // =============================================
    public class RelayCommand : ICommand
    {
        private readonly Action<object?> _execute;
        private readonly Func<object?, bool>? _canExecute;

        public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
        {
            _execute    = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public RelayCommand(Action execute, Func<bool>? canExecute = null)
            : this(_ => execute(), canExecute == null ? null : _ => canExecute()) { }

        public event EventHandler? CanExecuteChanged
        {
            add    => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object? p) => _canExecute?.Invoke(p) ?? true;
        public void Execute(object? p)    => _execute(p);
        public void RaiseCanExecuteChanged() => CommandManager.InvalidateRequerySuggested();
    }

    // =============================================
    // BaseViewModel
    // =============================================
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? name = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(name);
            return true;
        }
    }

    // =============================================
    // BoolToVisibilityConverter
    // =============================================
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BoolToVisibilityConverter : MarkupExtension, IValueConverter
    {
        private static BoolToVisibilityConverter? _instance;
        public static BoolToVisibilityConverter Instance => _instance ??= new();
        public override object ProvideValue(IServiceProvider sp) => Instance;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool invert  = parameter?.ToString() == "Invert";
            bool boolVal = value is bool b && b;
            if (invert) boolVal = !boolVal;
            return boolVal ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => value is Visibility v && v == Visibility.Visible;
    }

    // =============================================
    // EnumToBoolConverter
    // =============================================
    [ValueConversion(typeof(string), typeof(bool))]
    public class EnumToBoolConverter : MarkupExtension, IValueConverter
    {
        private static EnumToBoolConverter? _instance;
        public override object ProvideValue(IServiceProvider sp) => _instance ??= new();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value?.ToString() == parameter?.ToString();

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => value is true ? parameter?.ToString()! : Binding.DoNothing;
    }

    // =============================================
    // PesoConverter
    // =============================================
    [ValueConversion(typeof(decimal), typeof(string))]
    public class PesoConverter : MarkupExtension, IValueConverter
    {
        private static PesoConverter? _instance;
        public override object ProvideValue(IServiceProvider sp) => _instance ??= new();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is decimal d ? $"\u20b1{d:N2}" : "\u20b10.00";

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => decimal.TryParse(value?.ToString()?.Replace("\u20b1", "").Replace(",", ""), out var r) ? r : 0m;
    }

    // =============================================
    // StatusToColorConverter
    // =============================================
    [ValueConversion(typeof(string), typeof(SolidColorBrush))]
    public class StatusToColorConverter : MarkupExtension, IValueConverter
    {
        private static StatusToColorConverter? _instance;
        public override object ProvideValue(IServiceProvider sp) => _instance ??= new();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString() switch
            {
                "Completed" => new SolidColorBrush(Color.FromRgb(0x52, 0xB7, 0x88)),
                "Pending"   => new SolidColorBrush(Color.FromRgb(0xF4, 0xA2, 0x61)),
                "Active"    => new SolidColorBrush(Color.FromRgb(0x2D, 0x6A, 0x4F)),
                "Archived"  => new SolidColorBrush(Color.FromRgb(0x9E, 0x9E, 0x9E)),
                _           => new SolidColorBrush(Color.FromRgb(0x8F, 0xAF, 0x8A)),
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => Binding.DoNothing;
    }

    // =============================================
    // StringNotEmptyToVisibilityConverter
    // =============================================
    [ValueConversion(typeof(string), typeof(Visibility))]
    public class StringNotEmptyToVisibilityConverter : MarkupExtension, IValueConverter
    {
        private static StringNotEmptyToVisibilityConverter? _instance;
        public override object ProvideValue(IServiceProvider sp) => _instance ??= new();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => string.IsNullOrWhiteSpace(value?.ToString()) ? Visibility.Collapsed : Visibility.Visible;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => Binding.DoNothing;
    }

    // =============================================
    // NullToVisibilityConverter
    // =============================================
    [ValueConversion(typeof(object), typeof(Visibility))]
    public class NullToVisibilityConverter : MarkupExtension, IValueConverter
    {
        private static NullToVisibilityConverter? _instance;
        public override object ProvideValue(IServiceProvider sp) => _instance ??= new();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value == null ? Visibility.Collapsed : Visibility.Visible;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => Binding.DoNothing;
    }
    
    [ValueConversion(typeof(DataGridRow), typeof(int))]
public class RowNumberConverter : MarkupExtension, IValueConverter
{
    private static RowNumberConverter? _instance;
    public override object ProvideValue(IServiceProvider sp) => _instance ??= new();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is DataGridRow row)
            return row.GetIndex() + 1;
        return string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
}
