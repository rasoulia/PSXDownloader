using PSXDLL;
using PSXDownloader.MVVM.Commands;
using PSXDownloader.MVVM.Data;
using PSXDownloader.MVVM.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Windows.Input;
using System.Windows.Media;

namespace PSXDownloader.MVVM.ViewModels
{
    public class ConnectViewModel : ViewModelBase
    {
        private IPAddress? _address;
        private int _port;
        private string? _psnUrl;
        private string? _filePath;
        private SolidColorBrush? _connected;

        private ICommand? _connect;
        private ICommand? _clearLog;

        private readonly ConnectRepository _repository;

        public ConnectViewModel()
        {
            _repository = new();

            Port = 8080;
        }

        public ObservableCollection<KeyValuePair<string, IList<PsnUrlLog>>> PsnUrlList => new(_repository.GetAllUrl());

        public List<IPAddress> AddressList => PSXTools.GetHostIp();

        public IPAddress? Address
        {
            get => _address;
            set
            {
                _address = value;
                OnPropertyChanged();
            }
        }

        public int Port
        {
            get => _port;
            set
            {
                _port = value;
                OnPropertyChanged();
            }
        }

        public string? PsnUrl
        {
            get => _psnUrl;
            set
            {
                _psnUrl = value;
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

        public string? FilePath
        {
            get => _filePath;
            set
            {
                _filePath = value;
                OnPropertyChanged();
            }
        }

        public SolidColorBrush? Connected
        {
            get => _connected;
            set
            {
                _connected = value;
                OnPropertyChanged();
            }
        }

        public ICommand? Connect
        {
            get
            {
                _connect ??= new RelayCommand(ConnectCommand);
                return _connect;
            }
        }

        private void AddUrl(UrlInfo ui)
        {
            PsnUrl = ui.PsnUrl?.Split('?')[0];
            if (PSXTools.RegexUrl(PsnUrl))
            {
                Uri? uri = new(PsnUrl!);
                string titleID = Path.GetFileName(uri.LocalPath).Split('-')[1].Replace("_00", "");
                LocalFileDirectory = _repository.GetLocalPath(titleID).Result;
                string filePath = HashUrl.Instance().PsnLocalPath(PsnUrl!);
                FilePath = filePath;
                _repository.AddLog($"{titleID}", PsnUrl!, FilePath);
                OnPropertyChanged(nameof(PsnUrlList));
            }
        }

        private bool IsConnected = true;

        private void ConnectCommand(object? obj)
        {
            _repository.Connect(Address!, Port, AddUrl);
            if (IsConnected)
            {
                IsConnected = false;
                Connected = new((Color.FromRgb(73, 73, 73)));
                OnPropertyChanged(nameof(Connected));
            }
            else
            {
                IsConnected = true;
                Connected = new(Colors.Transparent);
                OnPropertyChanged(nameof(Connected));
            }
        }

        public ICommand? ClearLog
        {
            get
            {
                _clearLog ??= new RelayCommand(ClearLogCommand);
                return _clearLog;
            }
        }

        private void ClearLogCommand(object? obj)
        {
            _repository.ClearLog();
            OnPropertyChanged(nameof(PsnUrlList));
        }
    }
}
