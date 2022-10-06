using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PSXDownloader.MVVM.ViewModels
{
    public class MainWindowViewModel
    {
        public double MaxHeight => SystemParameters.WorkArea.Height;
        public double MaxWidth => SystemParameters.WorkArea.Width;
    }
}
