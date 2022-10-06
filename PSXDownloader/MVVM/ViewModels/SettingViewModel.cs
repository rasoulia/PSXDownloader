using PSXDLL;
using PSXDLL.Model;
using PSXDownloader.MVVM.Commands;
using PSXDownloader.MVVM.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PSXDownloader.MVVM.ViewModels
{
    public class SettingViewModel:ViewModelBase
    {
        private ICommand? _filePath;
        private ICommand? _saveSetting;

        private readonly SettingRepository _repository;


        public SettingViewModel()
        {
            _repository = new();
            AppConfig? setting = _repository.LoadSetting(AppConfig.Instance());
            Rule = setting?.Rule;
            IsAutoFind = setting!.IsAutoFindFile;
            LocalFileDirectory = setting?.LocalFileDirectory;
        }

        public string? Rule
        {
            get => AppConfig.Instance().Rule;
            set
            {
                AppConfig.Instance().Rule = value;
                OnPropertyChanged();
            }
        }

        public string? LocalFileDirectory
        {
            get => AppConfig.Instance().LocalFileDirectory;
            set
            {
                AppConfig.Instance().LocalFileDirectory = value;
                OnPropertyChanged();
            }
        }

        public bool IsAutoFind
        {
            get => AppConfig.Instance().IsAutoFindFile;
            set
            {
                AppConfig.Instance().IsAutoFindFile = value;
                OnPropertyChanged();
            }
        }

        public ICommand? FilePath
        {
            get
            {
                _filePath ??= new RelayCommands(FilePathMethod);
                return _filePath;
            }
        }

        private void FilePathMethod(object obj)
        {
            string? path = _repository.LocalFilePath();
            if (path?.Length > 0)
            {
                LocalFileDirectory = path;
            }

        }

        public ICommand? SaveSetting
        {
            get
            {
                _saveSetting ??= new RelayCommands(SaveSettingMethod);
                return _saveSetting;
            }
        }

        private void SaveSettingMethod(object obj)
        {
            _repository.SaveSetting(AppConfig.Instance());
        }
    }
}
