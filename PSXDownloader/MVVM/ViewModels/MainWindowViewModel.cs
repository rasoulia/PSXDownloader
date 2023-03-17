using PSXDownloader.MVVM.Commands;
using System.Windows;
using System.Windows.Input;

namespace PSXDownloader.MVVM.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private ICommand? _exit;
        private ICommand? _maximize;
        private ICommand? _minimize;
        private WindowState _windowState;

        public double MaxWidth => SystemParameters.WorkArea.Width;
        public double MaxHeight => SystemParameters.WorkArea.Height;

        public WindowState WindowState
        {
            get => _windowState;
            set
            {
                _windowState = value;
                OnPropertyChanged();
            }
        }

        public ICommand? Exit
        {
            get
            {
                _exit ??= new RelayCommand(ExitCommand);
                return _exit;
            }
        }

        private void ExitCommand(object? obj)
        {
            Application.Current.Shutdown();
        }

        public ICommand? Maximize
        {
            get
            {
                _maximize ??= new RelayCommand(MaximizeCommand);
                return _maximize;
            }
        }

        private void MaximizeCommand(object? obj)
        {
            WindowState = WindowState == WindowState.Normal ? WindowState.Maximized : WindowState.Normal;
        }

        public ICommand? Minimize
        {
            get
            {
                _minimize ??= new RelayCommand(MinimizeCommand);
                return _minimize;
            }
        }

        private void MinimizeCommand(object? obj)
        {
            WindowState = WindowState.Minimized;
        }
    }
}
