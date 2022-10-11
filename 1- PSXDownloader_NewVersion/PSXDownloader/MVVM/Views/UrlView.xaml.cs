using PSXDownloader.MVVM.Commands;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PSXDownloader.MVVM.Views
{
    /// <summary>
    /// Interaction logic for UrlView.xaml
    /// </summary>
    public partial class UrlView : UserControl, INotifyPropertyChanged
    {
        public UrlView()
        {
            InitializeComponent();
        }
        private ICommand? _copy;
        public ICommand? Copy
        {
            get
            {
                _copy ??= new RelayCommand(CopyMethod);
                return _copy;
            }
        }

        private void CopyMethod(object? obj)
        {
            Clipboard.SetText(Url);
        }

        public string Url
        {
            get { return (string)GetValue(UrlProperty); }
            set { SetValue(UrlProperty, value); OnProperyChanged(); }
        }
        public string? FileName
        {
            get
            {
                Uri uri = new(Url);
                return Path.GetFileName(uri.LocalPath);
            }
        }
        // Using a DependencyProperty as the backing store for Url.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UrlProperty =
            DependencyProperty.Register(nameof(Url), typeof(string), typeof(UrlView), new PropertyMetadata(null));

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnProperyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
