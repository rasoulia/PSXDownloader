using PSXDownloader.MVVM.Commands;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PSXDownloader.MVVM.Views
{
    /// <summary>
    /// Interaction logic for UrlView.xaml
    /// </summary>
    public partial class UrlView : UserControl
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
                _copy ??= new RelayCommand(CopyCommand);
                return _copy;
            }
        }

        private void CopyCommand(object? obj)
        {
            Clipboard.SetText(Url);
        }

        private ICommand? _download;
        public ICommand? Download
        {
            get
            {
                _download ??= new RelayCommand(DownloadCommand);
                return _download;
            }
        }

        private void DownloadCommand(object? obj)
        {
            string idm32 = @"C:\Program Files\Internet Download Manager\IDMan.exe";
            string idm64 = @"C:\Program Files (x86)\Internet Download Manager\IDMan.exe";
            if (System.Environment.Is64BitOperatingSystem)
            {
                Process cmd = new();
                Process.Start(idm64, $"/d {Url} /a");
            }
            else
            {
                Process.Start(idm32, $"/d {Url} /a");
            }
        }

        public string Url
        {
            get => (string)GetValue(UrlProperty);
            set => SetValue(UrlProperty, value);
        }

        // Using a DependencyProperty as the backing store for Url.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UrlProperty =
            DependencyProperty.Register(nameof(Url), typeof(string), typeof(UrlView), new PropertyMetadata(null));



        public string FilePath
        {
            get => (string)GetValue(FilePathProperty);
            set => SetValue(FilePathProperty, value);
        }

        // Using a DependencyProperty as the backing store for FilePath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FilePathProperty =
            DependencyProperty.Register("FilePath", typeof(string), typeof(UrlView), new PropertyMetadata(null));


    }
}
